using Minge2025Summer.Scripts.InGame.ItemScript.Interface;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ItemScript
{
    [System.Serializable]
    public class HotbarSlot
    {
        public KeyCode key;
        public IItem item;

        public bool IsEmpty => item == null;

        public HotbarSlot(KeyCode key)
        {
            this.key = key;
        }

        public void SetItem(IItem newItem)
        {
            item = newItem;
        }

        public void Clear()
        {
            item = null;
        }
    }
}