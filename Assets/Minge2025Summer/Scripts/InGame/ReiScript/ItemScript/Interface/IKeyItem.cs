namespace Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Interface
{
    public interface IKeyItem : IReiItem
    {
        public string GetItemName { get; }
        public string GetDisplayName { get; }
    }
}