using System;
using System.Collections.Generic;
using Minge2025Summer.Scripts.InGame.ItemScript;
using TMPro;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript
{
    public class ReiItemInventoryView : MonoBehaviour
    {
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private List<ReiItemSlotUI> itemSlots;

        [SerializeField] private int gridRows = 2;
        [SerializeField] private int gridColumns = 6;

        [SerializeField] private TextMeshProUGUI systemText;

        private int currentSelectedIndex = 0;

        public GameObject GetInventoryPanel=> inventoryPanel;

        public void ToggleInventoryPanel()
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            if (inventoryPanel.activeSelf)
            {
                SelectSlot(0);
            }
            else
            {
                ClearSelection();
            }
        }

        public void NotifyItemUsed(IReiItem item)
        {
            systemText.text = $"{item.GetItemName} を使った。";
        }

        public void UpdateInventory(List<ItemSlotData> slotDatas)
        {
            for (int i = 0; i < slotDatas.Count; i++)
            {
                if (i < slotDatas.Count)
                {
                    var data = slotDatas[i];
                    itemSlots[i].SetItem(data.ItemID, data.ItemType, data.Icon, data.Amount);
                }
                else
                {
                    itemSlots[i].Clear();
                }
            }
        }

        public void NavigateSlot(int direction)
        {
            int newIndex = currentSelectedIndex;
            switch (direction)
            {
                case -1: // W (上)
                    newIndex -= gridColumns;
                    break;
                case 1: // S (下)
                    newIndex += gridColumns;
                    break;
                case -2: // A (左)
                    if (currentSelectedIndex % gridColumns != 0)
                        newIndex--;
                    break;
                case 2: // D (右)
                    if (currentSelectedIndex % gridColumns != gridColumns - 1)
                        newIndex++;
                    break;
            }

            newIndex = Mathf.Clamp(newIndex, 0, itemSlots.Count - 1);
            SelectSlot(newIndex);
        }

        public (string, Type) GetSelectedItemIDAndType()
        {
            if (currentSelectedIndex >= 0 && currentSelectedIndex < itemSlots.Count)
            {
                return (itemSlots[currentSelectedIndex].GetCurrentItemID, itemSlots[currentSelectedIndex].GetCurrentItemType);
            }

            return (null, null);
        }

        private void SelectSlot(int index)
        {
            ClearSelection();
            currentSelectedIndex = index;
            itemSlots[currentSelectedIndex].SetSelected(true);
        }

        private void ClearSelection()
        {
            foreach (var slot in itemSlots)
            {
                slot.SetSelected(false);
            }
        }
    }
}