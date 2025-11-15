namespace Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Interface
{
    public interface IConsumableItem : IReiItem
    {
        public string GetItemName { get; }
        public string GetDisplayName { get; }
        public int GetItemAmount { get; }
    }
}