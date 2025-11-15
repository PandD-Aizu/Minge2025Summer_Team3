using System;
using Minge2025Summer.Scripts.InGame.ReiScript.ItemScript;
using Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Enum;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript.GunScript
{
    public class WeaponModel : MonoBehaviour
    {
        private IWeapon currentEquippedWeapon;
        // プロパティをバックフィールドに紐づける
        public IWeapon CurrentEquippedWeapon
        {
            get => currentEquippedWeapon;
            set
            {
                currentEquippedWeapon = value;
                // 装備時に武器側の初期化を呼ぶ
                currentEquippedWeapon?.Equip();
                // リロード残留情報をリセット
                hasPendingReload = false;
                pendingReloadCount = 0;
            }
        }

        private Subject<bool> onReload = new Subject<bool>();
        public IObservable<bool> OnReload => onReload;
        private Subject<Unit> notifyReload = new Subject<Unit>();
        public IObservable<Unit> OnNotifyReload => notifyReload;
        private Subject<(AmmoType, int)> onAmmoApplied = new Subject<(AmmoType, int)>();
        public IObservable<(AmmoType, int)> OnAmmoApplied => onAmmoApplied;
        
        // リロード残留情報
        private bool hasPendingReload;
        private int pendingReloadCount;
        private AmmoType pendingAmmoType;

        public void PreReloadAction(ReiItemInventoryModel inventoryModel)
        {
            // リロード中かどうかのフラグ
            if (CurrentEquippedWeapon == null || hasPendingReload)
            {
                return;
            }

            // リロードに必要な弾数を計算
            int bulletsNeeded = CurrentEquippedWeapon.GetMagCapacity - CurrentEquippedWeapon.GetAmmoInMag;
            if (bulletsNeeded <= 0)
            {
                return;
            }

            // インベントリから利用可能な弾数を取得
            var ammoType = CurrentEquippedWeapon.GetAmmoType;
            int bulletsAvailable = inventoryModel.GetAmmoCount(ammoType);
            int bulletsToReload = Mathf.Min(bulletsNeeded, bulletsAvailable);
            if (bulletsToReload <= 0)
            {
                return;
            }

            bool isEmptyReload = CurrentEquippedWeapon.GetAmmoInMag == 0;

            hasPendingReload = true;
            pendingReloadCount = bulletsToReload;
            pendingAmmoType = ammoType;

            onReload.OnNext(isEmptyReload);
        }

        public void Reload(ReiItemInventoryModel inventoryModel)
        {
            if (!hasPendingReload || CurrentEquippedWeapon == null)
            {
                return;
            }

            int appliedAmmo = pendingReloadCount;
            CurrentEquippedWeapon.Reload(pendingReloadCount);
            inventoryModel.TryConsumeAmmo(pendingAmmoType, pendingReloadCount);

            onAmmoApplied.OnNext((pendingAmmoType, appliedAmmo));
            
            hasPendingReload = false;
            pendingReloadCount = 0;
        }

        public void CheckReload()
        {
            if (CurrentEquippedWeapon == null)
                return;
            
            if (CurrentEquippedWeapon.GetAmmoInMag >= 0)
            {
                notifyReload.OnNext(Unit.Default);
            }
        }
    }
}