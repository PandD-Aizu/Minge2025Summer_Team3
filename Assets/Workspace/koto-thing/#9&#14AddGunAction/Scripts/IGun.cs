using UniRx;

namespace Workspace.koto_thing
{
    public interface IGun
    {
        public Subject<Unit> OnFire { get; }
        public void Equip();
        public void Reload(int bulletsToReload);
        public void Fire();
        public void Aim();
        public AmmoType GetAmmoType();
        public int GetMagCapacity();
        public int GetAmmoInMag();
        public float CurrentAccuracy();
        public float GetCurrentSpreadAngleDeg();
        public float GetHipFireSpreadAngleDeg();
        public void ResetAccuracy();
    }
}