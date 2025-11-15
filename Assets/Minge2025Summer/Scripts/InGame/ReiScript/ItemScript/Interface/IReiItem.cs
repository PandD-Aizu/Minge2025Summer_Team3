using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Interface
{
    public interface IReiItem
    {
        public string GetItemName { get; }
        public string GetDisplayName { get; }
        public string GetItemDescription { get; }
        public Sprite GetIcon { get; }
        public int GetItemAmount { get; }

        public void GetItem();
        public bool ApplyItem();
    }
}