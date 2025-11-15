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
        private int currentSelectedIndex;
        
        private Color selectedIconOriginalColor = Color.white;
        private bool selectedIconColorCached;

        public IObservable<Unit> OnInventorySelected => onInventorySelected;
        public GameObject GetInventoryPanel=> inventoryPanel;

        public void Initialize()
        {
            if (selectedSlotIcon != null)
            {
                selectedIconOriginalColor = selectedSlotIcon.color;
                selectedIconColorCached = true;
            }
        }
        
        /// <summary>
        /// インベントリパネルの表示・非表示を切り替える
        /// </summary>
        public void ToggleInventoryPanel()
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            if (inventoryPanel.activeSelf)
            {
                // スロットが無ければ SelectSlot を呼ばない
                if (itemSlots != null && itemSlots.Count > 0)
                    SelectSlot(0);
                else
                    ClearSelection();
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
            if (systemText == null) return;
            systemText.text = $"{item.GetItemName} を使った。";
        }

        /// <summary>
        /// インベントリの内容を更新する
        /// </summary>
        /// <param name="slotDatas">スロットのデータ</param>
        public void UpdateInventory(List<ItemSlotData> slotDatas)
        {
            if (itemSlots == null) return;

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

            // 現在の選択インデックスが更新後に範囲外になっていれば修正
            if (currentSelectedIndex >= itemSlots.Count)
                currentSelectedIndex = 0;
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
            // 範囲チェック
            if (itemSlots == null || itemSlots.Count == 0) return;
            if (index < 0 || index >= itemSlots.Count) index = 0;

            ClearSelection();
            currentSelectedIndex = index;
            itemSlots[currentSelectedIndex].SetSelected(true);

            // 選択アイコンの更新
            if (selectedSlotIcon != null)
            {
                var slotIcon = itemSlots[currentSelectedIndex].GetItemIcon;
                if (slotIcon != null && slotIcon.sprite != null && slotIcon.enabled)
                {
                    selectedSlotIcon.sprite = slotIcon.sprite;
                    selectedSlotIcon.enabled = true;
                    // 元色がキャッシュされていれば元のアルファに戻す
                    if (!selectedIconColorCached)
                    {
                        selectedIconOriginalColor = selectedSlotIcon.color;
                        selectedIconColorCached = true;
                    }
                    selectedSlotIcon.color = new Color(selectedIconOriginalColor.r, selectedIconOriginalColor.g, selectedIconOriginalColor.b, selectedIconOriginalColor.a);
                }
                else
                {
                    selectedSlotIcon.sprite = null;
                    selectedSlotIcon.enabled = true;
                    var c = selectedIconOriginalColor;
                    selectedSlotIcon.color = new Color(c.r, c.g, c.b, 0f);
                }
            }

            // メインの説明文は一旦クリア
            if (mainItemNameText != null) mainItemNameText.text = string.Empty;
            if (mainItemDescText != null) mainItemDescText.text = string.Empty;

            onInventorySelected.OnNext(Unit.Default);
        }

        /// <summary>
        /// 全てのスロットの選択状態をクリアする
        /// </summary>
        private void ClearSelection()
        {
            if (itemSlots == null) return;
            foreach (var slot in itemSlots)
            {
                slot.SetSelected(false);
            }

            // 選択アイコンも透明にする（spriteはnull）
            if (selectedSlotIcon != null)
            {
                selectedSlotIcon.sprite = null;
                selectedSlotIcon.enabled = true; // 表示領域は確保して透明化
                var c = selectedIconOriginalColor;
                selectedSlotIcon.color = new Color(c.r, c.g, c.b, 0f);
            }
        }
        
        public void SetMainItemText(string displayName, string description)
        {
            if (mainItemNameText != null) mainItemNameText.text = displayName;
            if (mainItemDescText != null) mainItemDescText.text = description;
        }
    }
}