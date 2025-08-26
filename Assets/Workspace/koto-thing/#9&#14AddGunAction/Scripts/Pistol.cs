using UnityEngine;

namespace Workspace.koto_thing
{
    public class Pistol : MonoBehaviour, IGun
    {
        [Header("弾丸関連")] 
        [SerializeField] private AmmoType ammoType = AmmoType.Pistol;
        [SerializeField] private int magCapacity = 12;
        [SerializeField] private int ammoInMag;

        [Header("銃の性能")] 
        [SerializeField] private float fireRate = 0.5f;
        [SerializeField] private float reloadTime = 1.5f;
        [SerializeField] private float damage = 10f;

        public void Equip()
        {
            
        }

        public void Reload(int bulletsToReload)
        {
            ammoInMag += bulletsToReload;
        }
        
        public void Fire()
        {
            Physics.Raycast(Camera.main.transform.position, 
                Camera.main.transform.forward, 
                out RaycastHit hit,
                100.0f);
            
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("EnemyShootable"))
                {
                    Debug.Log($"Hit {hit.collider.name}");
                }
            }
            
            ammoInMag--;
        }
        
        public void Aim()
        {
            
        }
        
        /* 以下ヘルパー関数 */
        public AmmoType GetAmmoType()
        {
            return ammoType;
        }
        
        public int GetMagCapacity()
        {
            return magCapacity;
        }
        
        public int GetAmmoInMag()
        {
            return ammoInMag;
        }
    }
}