using Minge2025Summer.Scripts.InGame.ItemScript.Interface;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minge2025Summer.Scripts.InGame.ItemScript
{
    public class ItemSlotUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image slotImage;
        [SerializeField] private TextMeshProUGUI amountText;

        private int amount;
        public int Amount => amount;
        
        public IItem AssignedItem { get; private set; }
        public bool IsEmpty => icon.sprite == null;

        /// <summary>
        /// アイテムをスロットにセットする
        /// </summary>
        /// <param name="item">セットするアイテム</param>
        /// <param name="amount">個数</param>
        public void SetItem(IItem item, int amount)
        {
            AssignedItem = item;
            icon.sprite = item.GetSprite;
            icon.enabled = icon.sprite != null;
            UpdateAmount(amount);
        }

        /// <summary>
        /// 個数を更新する
        /// </summary>
        /// <param name="amount">更新する個数</param>
        public void UpdateAmount(int amount)
        {
            this.amount = Mathf.Max(0, amount);
            amountText.text = "x " + this.amount;
        }

        /// <summary>
        /// スロットをクリアする
        /// </summary>
        public void ClearSlot()
        {
            AssignedItem = null;
            amount = 0;
            icon.sprite = null;
            icon.enabled = false;
            amountText.text = string.Empty;
        }

        /// <summary>
        /// スロットの選択状態を設定する
        /// </summary>
        /// <param name="selected">選択されているかどうか</param>
        public void SetSelected(bool selected)
        {
            if (slotImage != null)
                slotImage.color = selected ? Color.red : Color.white;
        }
    }
}