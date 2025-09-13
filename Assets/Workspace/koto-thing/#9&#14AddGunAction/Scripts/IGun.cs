using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public interface IGun
    {
        public Subject<Unit> OnFire { get; }
        public void Tick(float deltaTime);
        
        public void Equip();
        public void Reload(int bulletsToReload);
        public void Fire();
        public void Aim();
        public AmmoType GetAmmoType();
        public int GetMagCapacity();
        public int GetAmmoInMag();
        public float CurrentAccuracy();
        public float GetCurrentSpreadAngleDeg();
        public float GetFinalSpreadAngleDeg();
        public float GetHipFireSpreadAngleDeg();
        public void ResetAccuracy();
    }
}