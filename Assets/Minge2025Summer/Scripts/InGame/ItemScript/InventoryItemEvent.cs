using Minge2025Summer.Scripts.InGame.ItemScript.Interface;

namespace Minge2025Summer.Scripts.InGame.ItemScript
{
    public struct InventoryItemEvent
    {
        public IItem Item;
		public int Amount;
		public bool Removed;

		public InventoryItemEvent(IItem item, int amount, bool removed = false)
		{
			Item = item;
			Amount = amount;
			Removed = removed;
		}
    }
}