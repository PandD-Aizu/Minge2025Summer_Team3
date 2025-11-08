namespace Minge2025Summer.Scripts.InGame.ReiScript
{
    public interface IKeyItem : IReiItem
    {
        public string GetItemName { get; }
        public string GetDisplayName { get; }
    }
}