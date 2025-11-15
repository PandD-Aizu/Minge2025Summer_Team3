using Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Enum;
using UniRx;

namespace Minge2025Summer.Scripts.InGame.ReiScript.GunScript
{
    public interface IWeapon
    {
        public string GetWeaponName { get; }
        public AmmoType GetAmmoType { get; }
        public int GetMagCapacity { get; }
        public int GetAmmoInMag { get; }
        public float GetGunSoundVolume { get; }

        public Subject<Unit> OnFire { get; }

        public void UpdateWeapon(float deltaTime);
        public void Equip();
        public void Reload(int bulletsToReload);
        public void Fire();
        public void Aim();
        public void ResetAccuracy();

        public float GetCurrentAccuracy();
        public float GetCurrentSpreadAngleDeg();
        public float GetFinalSpreadAngleDeg();
        public float GetHipFireSpreadAngleDeg();
    }
}