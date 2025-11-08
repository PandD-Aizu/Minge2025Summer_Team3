namespace Minge2025Summer.Scripts.InGame.ReiScript
{
    public interface IConsumableItem : IReiItem
    {
        public string GetItemName { get; }
        public string GetDisplayName { get; }
        public int GetItemAmount { get; }
    }
}