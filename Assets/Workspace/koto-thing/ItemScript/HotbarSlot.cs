using UnityEngine;

namespace Workspace.koto_thing
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