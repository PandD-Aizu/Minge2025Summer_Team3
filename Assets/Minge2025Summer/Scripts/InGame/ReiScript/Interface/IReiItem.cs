using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript
{
    public interface IReiItem
    {
        public string GetItemName { get; }
        public string GetDisplayName { get; }
        public Sprite GetIcon { get; }
        public bool ApplyItem();
    }
}