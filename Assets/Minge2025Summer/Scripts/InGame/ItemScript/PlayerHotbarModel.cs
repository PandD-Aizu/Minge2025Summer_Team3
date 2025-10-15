using System.Collections.Generic;
using Minge2025Summer.Scripts.InGame.ItemScript.Interface;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ItemScript
{
    public class PlayerHotbarModel : MonoBehaviour
    {
        [SerializeField] private int slotCount = 5;
        [SerializeField] private List<KeyCode> defaultKeys;

        public IReadOnlyList<HotbarSlot> Slots => slots;
        public HotbarReactiveBus Bus { get; private set; } = new();

        private List<HotbarSlot> slots;
        private IItemUseStrategy useStrategy;

        public void Initialize(IItemUseStrategy strategy = null)
        {
            useStrategy = strategy ?? new DefaultItemUseStrategy();
            slots = new List<HotbarSlot>(slotCount);
            for (int i = 0; i < slotCount; i++)
            {
                var key = i < defaultKeys.Count ? defaultKeys[i] : KeyCode.None;
                slots.Add(new HotbarSlot(key));
            }
        }

        public bool AssignItem(int slotIndex, IItem item)
        {
            if (!IsValidSlot(slotIndex))
                return false;
            
            slots[slotIndex].SetItem(item);
            Bus.OnAssigned.OnNext(new HotbarAssignEvent(slotIndex, item));
            return true;
        }

        public bool AutoAssignFirstEmptySlot(IItem item)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].IsEmpty)
                {
                    AssignItem(i, item);
                    return true;
                }
            }

            return false;
        }

        public bool UseSlot(int slotIndex)
        {
            if (!IsValidSlot(slotIndex))
                return false;

            var slot = slots[slotIndex];
            if (slot.IsEmpty)
                return false;

            bool can = useStrategy.CanUse(slot.item);
            if (can)
            {
                useStrategy.Use(slot.item);
                Bus.OnUsed.OnNext(new HotbarUseEvent(slotIndex, slot.item, true));
                return true;
            }

            Bus.OnUsed.OnNext(new HotbarUseEvent(slotIndex, slot.item, false));
            return false;
        }

        public void Unassign(int slotIndex)
        {
            if (!IsValidSlot(slotIndex))
                return;
            
            slots[slotIndex].Clear();
            Bus.OnAssigned.OnNext(new HotbarAssignEvent(slotIndex, null));
        }

        public void RebindKey(int slotIndex, KeyCode newKey)
        {
            if (!IsValidSlot(slotIndex))
                return;

            slots[slotIndex].key = newKey;
        }

        private bool IsValidSlot(int index) => index >= 0 && index < slotCount;
    }
}