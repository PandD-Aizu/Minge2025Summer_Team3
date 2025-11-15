using System;
using System.Collections.Generic;
using Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Interface;
using Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Struct;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Minge2025Summer.Scripts.InGame.ReiScript.ItemScript
{
    public class ReiItemInventoryView : MonoBehaviour
    {
        [Header("インベントリパネルとアイテムスロット")]
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private List<ReiItemSlotUI> itemSlots;
        [SerializeField] private Image selectedSlotIcon;
        [SerializeField] private TextMeshProUGUI mainItemNameText;
        [SerializeField] private TextMeshProUGUI mainItemDescText;

        [Header("列と行の設定")]
        [SerializeField] private int gridRows = 2;
        [SerializeField] private int gridColumns = 6;

        [Header("システムメッセージ表示用テキスト")]
        [SerializeField] private TextMeshProUGUI systemText;

        private Subject<Unit> onInventorySelected = new Subject<Unit>();
        private int currentSelectedIndex = 0;

        public IObservable<Unit> OnInventorySelected => onInventorySelected;
        public GameObject GetInventoryPanel=> inventoryPanel;

        /// <summary>
        /// インベントリパネルの表示・非表示を切り替える
        /// </summary>
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

        /// <summary>
        /// アイテムを使用したことを通知する
        /// </summary>
        /// <param name="item">使用したアイテム</param>
        public void NotifyItemUsed(IReiItem item)
        {
            systemText.text = $"{item.GetItemName} を使った。";
        }

        /// <summary>
        /// インベントリの内容を更新する
        /// </summary>
        /// <param name="slotDatas">スロットのデータ</param>
        public void UpdateInventory(List<ItemSlotData> slotDatas)
        {
            for (int i = 0; i < itemSlots.Count; i++)
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

        /// <summary>
        /// アイテムスロットのナビゲーション
        /// </summary>
        /// <param name="direction">WASDの方向</param>
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

        /// <summary>
        /// 選択されているアイテムのIDとタイプを取得する
        /// </summary>
        /// <returns>アイテムIDと型</returns>
        public (string, Type) GetSelectedItemIDAndType()
        {
            if (currentSelectedIndex >= 0 && currentSelectedIndex < itemSlots.Count)
            {
                return (itemSlots[currentSelectedIndex].GetCurrentItemID, itemSlots[currentSelectedIndex].GetCurrentItemType);
            }

            return (null, null);
        }

        /// <summary>
        /// 指定したインデックスのスロットを選択状態にする
        /// </summary>
        /// <param name="index">指定するインデックス</param>
        private void SelectSlot(int index)
        {
            ClearSelection();
            currentSelectedIndex = index;
            itemSlots[currentSelectedIndex].SetSelected(true);
            selectedSlotIcon = itemSlots[currentSelectedIndex].GetItemIcon;
            mainItemNameText.text = string.Empty;
            mainItemDescText.text = string.Empty;
            onInventorySelected.OnNext(Unit.Default);
        }

        /// <summary>
        /// 全てのスロットの選択状態をクリアする
        /// </summary>
        private void ClearSelection()
        {
            foreach (var slot in itemSlots)
            {
                slot.SetSelected(false);
            }
        }
        
        public void SetMainItemText(string name, string description)
        {
            mainItemNameText.text = name;
            mainItemDescText.text = description;
        }
    }
}