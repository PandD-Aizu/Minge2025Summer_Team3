using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minge2025Summer.Scripts.InGame.ReiScript
{
    public class ReiItemSlotUI : MonoBehaviour
    {
        [SerializeField] private Image itemFrame;
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemAmountText;

        private string currentItemID;
        private Type currentItemType;

        public string GetCurrentItemID => currentItemID;
        public Type GetCurrentItemType => currentItemType;
        
        public void SetItem(string itemID, Type itemType, Sprite icon, int amount)
        {
            currentItemID = itemID;
            currentItemType = itemType;
            itemIcon.sprite = icon;
            itemIcon.enabled = true;
            itemAmountText.text = amount > 1 ? amount.ToString() : "";
        }

        public void Clear()
        {
            currentItemID = null;
            itemIcon.sprite = null;
            itemIcon.enabled = false;
            itemAmountText.text = "";
        }
        
        public void SetSelected(bool selected)
        {
            if (itemFrame != null)
            {
                itemFrame.color = selected ? Color.red : Color.white;
            }
        }
    }
}