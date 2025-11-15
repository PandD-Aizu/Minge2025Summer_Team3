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
        public IWeapon CurrentEquippedWeapon => currentEquippedWeapon;
        
        private Subject<bool> onReload = new Subject<bool>();
        public IObservable<bool> OnReload => onReload;
        private Subject<(AmmoType, int)> onAmmoApplied = new Subject<(AmmoType, int)>();
        public IObservable<(AmmoType, int)> OnAmmoApplied => onAmmoApplied;
        
        // リロード残留情報
        private bool hasPendingReload;
        private int pendingReloadCount;
        private AmmoType pendingAmmoType;

        public void PreReloadAction(ReiItemInventoryModel inventoryModel)
        {
            // リロード中かどうかのフラグ
            if (currentEquippedWeapon == null || hasPendingReload)
            {
                Debug.LogWarning("No gun equipped or reload already in progress.");
                return;
            }

            // リロードに必要な弾数を計算
            int bulletsNeeded = currentEquippedWeapon.GetMagCapacity - currentEquippedWeapon.GetAmmoInMag;
            if (bulletsNeeded <= 0)
            {
                Debug.LogWarning("No bullets needed to reload.");
                return;
            }

            // インベントリから利用可能な弾数を取得
            var ammoType = currentEquippedWeapon.GetAmmoType;
            int bulletsAvailable = inventoryModel.GetAmmoCount(ammoType);
            int bulletsToReload = Mathf.Min(bulletsNeeded, bulletsAvailable);
            if (bulletsToReload <= 0)
            {
                Debug.LogWarning("No bullets available to reload.");
                return;
            }

            bool isEmptyReload = currentEquippedWeapon.GetAmmoInMag == 0;

            hasPendingReload = true;
            pendingReloadCount = bulletsToReload;
            pendingAmmoType = ammoType;

            onReload.OnNext(isEmptyReload);
        }

        public void Reload(ReiItemInventoryModel inventoryModel)
        {
            if (!hasPendingReload || currentEquippedWeapon == null)
            {
                Debug.LogError("No pending reload or no gun equipped.");
                return;
            }

            int appliedAmmo = pendingReloadCount;
            currentEquippedWeapon.Reload(pendingReloadCount);
            inventoryModel.TryConsumeAmmo(pendingAmmoType, pendingReloadCount);

            onAmmoApplied.OnNext((pendingAmmoType, appliedAmmo));
            
            hasPendingReload = false;
            pendingReloadCount = 0;
        }
    }
}