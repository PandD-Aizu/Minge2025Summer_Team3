using System.Collections.Generic;
using Minge2025Summer.Scripts.InGame.GunScript.Enum;
using Minge2025Summer.Scripts.InGame.GunScript.Interface;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.GunScript
{
    public class GunModel : MonoBehaviour
    {
        private IGun currentEquippedGun;
        
        private Dictionary<AmmoType, int> ammoInventory = new ();
        
        // リロード残留情報
        private bool hasPendingReload;
        private int pendingReloadCount;
        private AmmoType pendingAmmoType;

        // 弾残量変更通知 (取得/消費/同期)
        public Subject<(AmmoType ammoType, int count)> AmmoChanged { get; } = new();
        // リロードでマガジンへ弾を適用した瞬間のみ通知 (適用数)
        public Subject<(AmmoType ammoType, int appliedCount)> AmmoApplied { get; } = new();

        /* プロパティ */
        public Subject<bool> OnReload { get; } = new ();
        public Subject<Unit> NotifyReload { get; } = new ();
        public IGun CurrentEquippedGun
        {
            get => currentEquippedGun;
            set 
            {
                currentEquippedGun = value;
                currentEquippedGun.Equip();
                
                // リロード残留情報をリセット
                hasPendingReload = false;
                pendingReloadCount = 0;
            }
        }

        public IReadOnlyDictionary<AmmoType, int> GetAmmoInventory => ammoInventory; // 外部からは読み取り専用

        /// <summary>
        /// 外部インベントリのスナップショットから完全同期する(これが唯一のソースオブトゥルースである前提)
        /// 既存値をクリアして上書きし、差分を AmmoChanged で通知する
        /// </summary>
        /// <param name="snapshot">(AmmoType type, int count) の列挙</param>
        public void SyncFromInventory(IEnumerable<(AmmoType type, int count)> snapshot)
        {
            // 変更前のコピーを取得し差分検出に使用
            var old = new Dictionary<AmmoType, int>(ammoInventory);
            ammoInventory.Clear();
            foreach (var (type, count) in snapshot)
            {
                if (count > 0)
                    ammoInventory[type] = count;
            }
            
            // 差分通知
            foreach (var kv in ammoInventory)
            {
                if (!old.TryGetValue(kv.Key, out var prev) || prev != kv.Value)
                {
                    AmmoChanged.OnNext((kv.Key, kv.Value));
                }
            }
            
            foreach (var kv in old)
            {
                if (!ammoInventory.ContainsKey(kv.Key))
                {
                    AmmoChanged.OnNext((kv.Key, 0));
                }
            }
        }

        /// <summary>
        /// 指定タイプの弾数を絶対値で設定 (0 以下で削除)
        /// UI同期
        /// </summary>
        public void SetAmmoAbsolute(AmmoType ammoType, int count)
        {
            int before = ammoInventory.GetValueOrDefault(ammoType, -1);
            if (count <= 0)
            {
                if (ammoInventory.Remove(ammoType) || before != -1)
                {
                    AmmoChanged.OnNext((ammoType, 0));
                }
                return;
            }
            ammoInventory[ammoType] = count;
            if (before != count)
            {
                AmmoChanged.OnNext((ammoType, count));
            }
        }
        
        /// <summary>
        /// 弾丸を加算 (アイテム取得時など)。内部で SetAmmoAbsolute を使用しイベント発行を統一
        /// </summary>
        public void AddAmmo(AmmoType type, int count)
        {
            if (count == 0) return;
            int current = ammoInventory.GetValueOrDefault(type);
            SetAmmoAbsolute(type, current + count);
        }

        /// <summary>
        /// 現在の内部スナップショットを返す
        /// </summary>
        public Dictionary<AmmoType, int> GetAmmoSnapshot()
        {
            return new Dictionary<AmmoType, int>(ammoInventory);
        }

        /// <summary>
        /// リロードのための下準備をする(マガジンに追加予定の弾数を算出してイベント通知)
        /// </summary>
        public void PreReload()
        {
            if (currentEquippedGun == null || hasPendingReload)
                return;

            int bulletsNeeded = currentEquippedGun.GetMagCapacity() - currentEquippedGun.GetAmmoInMag();
            if (bulletsNeeded <= 0)
                return;
            
            var ammoType = currentEquippedGun.GetAmmoType();
            int bulletsAvailable = ammoInventory.GetValueOrDefault(ammoType);
            int bulletsToReload = Mathf.Min(bulletsNeeded, bulletsAvailable);
            if (bulletsToReload <= 0)
                return;

            bool isEmptyReload = currentEquippedGun.GetAmmoInMag() == 0;

            hasPendingReload = true;
            pendingReloadCount = bulletsToReload;
            pendingAmmoType = ammoType;
            
            OnReload.OnNext(isEmptyReload);
        }

        /// <summary>
        /// 事前計算したpendingReloadCountを用いてリロードを確定させる。
        /// 弾数を減算しイベントを通知。
        /// </summary>
        public void Reload()
        {
            if (!hasPendingReload || currentEquippedGun == null)
                return;
            
            int current = ammoInventory.GetValueOrDefault(pendingAmmoType);
            int newValue = current - pendingReloadCount;
            if (newValue < 0) newValue = 0; // 安全
            SetAmmoAbsolute(pendingAmmoType, newValue);

            int applied = pendingReloadCount;
            currentEquippedGun.Reload(pendingReloadCount);

            AmmoApplied.OnNext((pendingAmmoType, applied));
            
            hasPendingReload = false;
            pendingReloadCount = 0;
        }

        /* ---以下ヘルパー関数--- */
        /// <summary>
        /// 現在装備銃タイプの総所持弾薬数を取得。
        /// </summary>
        public int GetCurrentAmmo()
        {
            return ammoInventory.GetValueOrDefault(currentEquippedGun.GetAmmoType());
        }
        
        /// <summary>
        /// 現在装備銃のマガジン容量。
        /// </summary>
        public int GetCurrentMagCapacity()
        {
            return currentEquippedGun.GetMagCapacity();
        }

        /// <summary>
        /// 現在装備銃のマガジン内弾数。
        /// </summary>
        public int GetCurrentAmmoInMag()
        {
            return currentEquippedGun.GetAmmoInMag();
        }

        /// <summary>
        /// マガジンが空 & 所持弾ありならリロード UIを促すイベント送出。
        /// </summary>
        public void CheckReload()
        {
            if (currentEquippedGun.GetAmmoInMag() == 0 && GetCurrentAmmo() > 0)
            {
                NotifyReload.OnNext(Unit.Default);
            }
        }
    }
}