using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class GunModel : MonoBehaviour
    {
        private IGun currentEquippedGun;
        
        private Dictionary<AmmoType, int> ammoInventory = new ();
        
        // リロード残留情報
        private bool hasPendingReload;
        private int pendingReloadCount;
        private AmmoType pendingAmmoType;

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

        public Dictionary<AmmoType, int> GetAmmoInventory => ammoInventory;

        /// <summary>
        /// リロードのための下準備をする
        /// </summary>
        public void PreReload()
        {
            if (currentEquippedGun == null || hasPendingReload)
                return;

            // リロードに必要な弾薬を計算する
            int bulletsNeeded = currentEquippedGun.GetMagCapacity() - currentEquippedGun.GetAmmoInMag();
            if (bulletsNeeded <= 0)
                return;
            
            // 所持弾薬から補充できる弾数を計算する
            var ammoType = currentEquippedGun.GetAmmoType();
            int bulletsAvailable = ammoInventory.GetValueOrDefault(ammoType);
            int bulletsToReload = Mathf.Min(bulletsNeeded, bulletsAvailable);
            if (bulletsToReload <= 0)
                return;

            bool isEmptyReload = currentEquippedGun.GetAmmoInMag() == 0;

            hasPendingReload = true;
            pendingReloadCount = bulletsToReload;
            pendingAmmoType = ammoType;
            
            // OnReloadイベントを発行
            OnReload.OnNext(isEmptyReload);
        }

        /// <summary>
        /// 銃に弾薬をリロードする
        /// </summary>
        public void Reload()
        {
            if (!hasPendingReload || currentEquippedGun == null)
                return;
            
            // 所持弾薬を更新して、銃に弾薬を補充する
            ammoInventory[pendingAmmoType] -= pendingReloadCount;
            currentEquippedGun.Reload(pendingReloadCount);

            hasPendingReload = false;
            pendingReloadCount = 0;
        }
        
        /// <summary>
        /// 弾丸を追加する
        /// </summary>
        /// <param name="type">追加する弾の種類</param>
        /// <param name="count">追加する弾数</param>
        public void AddAmmo(AmmoType type, int count)
        {
            if (ammoInventory.ContainsKey(type))
                ammoInventory[type] += count;
            else
                ammoInventory[type] = count;
        }

        /* ---以下ヘルパー関数--- */
        
        /// <summary>
        /// 現在装備している銃の総弾薬数を取得する
        /// </summary>
        /// <returns>現在装備している銃の総弾薬数</returns>
        public int GetCurrentAmmo()
        {
            return ammoInventory.GetValueOrDefault(currentEquippedGun.GetAmmoType());
        }
        
        /// <summary>
        /// 現在装備している銃のマガジン容量を取得する
        /// </summary>
        /// <returns>現在装備している銃のマガジン容量</returns>
        public int GetCurrentMagCapacity()
        {
            return currentEquippedGun.GetMagCapacity();
        }

        /// <summary>
        /// 現在装備している銃のマガジン内の弾薬数を取得する
        /// </summary>
        /// <returns>現在装備している銃のマガジン内の弾薬数</returns>
        public int GetCurrentAmmoInMag()
        {
            return currentEquippedGun.GetAmmoInMag();
        }

        public void CheckReload()
        {
            if (currentEquippedGun.GetAmmoInMag() == 0 && GetCurrentAmmo() > 0)
            {
                NotifyReload.OnNext(Unit.Default);
            }
        }
    }
}