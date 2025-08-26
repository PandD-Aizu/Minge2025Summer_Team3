namespace Workspace.koto_thing
{
    public interface IGun
    {
        public void Equip();
        public void Reload(int bulletsToReload);
        public void Fire();
        public void Aim();
        public AmmoType GetAmmoType();
        public int GetMagCapacity();
        public int GetAmmoInMag();
    }
}