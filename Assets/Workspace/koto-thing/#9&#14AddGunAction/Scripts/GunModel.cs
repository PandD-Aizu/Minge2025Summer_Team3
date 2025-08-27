using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class GunModel : MonoBehaviour
    {
        private IGun currentEquippedGun;
        
        private Dictionary<AmmoType, int> ammoInventory = new ();

        public IGun GetCurrentEquippedGun => currentEquippedGun;
        
        public Dictionary<AmmoType, int> GetAmmoInventory => ammoInventory;
        
        // TODO: テスト用
        private void Awake()
        {
            currentEquippedGun = GetComponent<Pistol>();
            
            if (!ammoInventory.ContainsKey(AmmoType.Pistol))
                ammoInventory.Add(AmmoType.Pistol, 60);
        }

        /// <summary>
        /// 銃のリロードを行う
        /// </summary>
        public void Reload()
        {
            if (currentEquippedGun == null)
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

            // 所持弾薬を更新し、銃に弾薬を補充する
            ammoInventory[ammoType] -= bulletsToReload;
            currentEquippedGun.Reload(bulletsToReload);
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
    }
}