namespace Minge2025Summer.Scripts.InGame.ItemScript.Interface
{
    public interface IItemUseStrategy
    {
        public bool CanUse(IItem item);
        public void Use(IItem item);
    }
}