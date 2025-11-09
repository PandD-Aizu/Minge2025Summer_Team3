using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Minge2025Summer.Scripts.InGame.GunScript;
using Minge2025Summer.Scripts.InGame.GunScript.Enum;
using Minge2025Summer.Scripts.InGame.ItemScript.Ammo;
using Minge2025Summer.Scripts.InGame.ItemScript.Interface;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minge2025Summer.Scripts.InGame.ItemScript
{
    public class PlayerItemView : MonoBehaviour
    {
        [SerializeField] private GameObject inventoryScreen;
        [SerializeField] private ItemSlotUI[] itemSlots;

        [SerializeField] private TextMeshProUGUI captionText; // 獲得したアイテムの名前を表示する

        [Header("アイテムプレビュースロット")] 
        [SerializeField] private Image selectedItemIcon;
        [SerializeField] private TextMeshProUGUI selectedItemNameText;
        [SerializeField] private TextMeshProUGUI selectedItemDescriptionText;

        [Header("アイテムスロット")] 
        [SerializeField] private int columns = 6;
        [SerializeField] private int rows = 2;
        
        private readonly Dictionary<IItem, ItemSlotUI> map = new();

        private int selectedIndex = 0;
        public bool IsOpen => inventoryScreen != null && inventoryScreen.activeSelf;

        public void SwitchInventoryScreen()
        {
            inventoryScreen.SetActive(!inventoryScreen.activeSelf);
            if (IsOpen)
            {
                ApplySelectionVisuals();
                UpdateSelectedPreview();
            }
        }

        public void UpdateItemSlot(IItem item, int amount)
        {
            if (map.TryGetValue(item, out var slot))
            {
                slot.UpdateAmount(amount);
                if (itemSlots[selectedIndex] == slot)
                    UpdateSelectedPreview();
                return;
            }

            foreach (var eachSlot in itemSlots)
            {
                if (eachSlot.IsEmpty)
                {
                    eachSlot.SetItem(item, amount);
                    map[item] = eachSlot;

                    if (itemSlots[selectedIndex].IsEmpty)
                    {
                        selectedIndex = IndexOf(eachSlot);
                        ApplySelectionVisuals();
                    }

                    UpdateSelectedPreview();
                    
                    return;
                }
            }
        }

        public void RemoveItemSlot(IItem item)
        {
            if (!map.TryGetValue(item, out var slot))
                return;

            bool wasSelected = (itemSlots[selectedIndex] == slot);
            
            slot.ClearSlot();
            map.Remove(item);

            if (wasSelected)
            {
                int newIndex = FindFirstNonEmptyIndex();
                if (newIndex >= 0)
                    selectedIndex = newIndex;
                
                ApplySelectionVisuals();
                UpdateSelectedPreview();
            }
        }

        public void MoveSelection(Vector2Int dir)
        {
            if (itemSlots == null || itemSlots.Length == 0)
                return;

            int currentRow = selectedIndex / columns;
            int currentCol = selectedIndex % columns;

            int nextRow = Mathf.Clamp(currentRow + dir.y, 0, rows - 1);
            int nextCol = Mathf.Clamp(currentCol + dir.x, 0, columns - 1);

            int nextIndex = nextRow * columns + nextCol;
            nextIndex = Mathf.Clamp(nextIndex, 0, itemSlots.Length - 1);

            if (nextIndex == selectedIndex)
                return;

            selectedIndex = nextIndex;
            ApplySelectionVisuals();
            UpdateSelectedPreview();
        }
        
        /* 以下ヘルパー関数 */

        private void ApplySelectionVisuals()
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                itemSlots[i].SetSelected(i == selectedIndex);
            }
        }

        private void UpdateSelectedPreview()
        {
            if (itemSlots == null || itemSlots.Length == 0)
                return;

            var slot = itemSlots[selectedIndex];
            var item = slot.AssignedItem;
            
            // アイコン
            if (selectedItemIcon != null)
            {
                selectedItemIcon.sprite = item != null ? item.GetSprite : null;
                selectedItemIcon.enabled = selectedItemIcon.sprite != null;
            }
            
            // 表示名
            if (selectedItemNameText != null)
            {
                selectedItemNameText.text = item != null ? item.GetDisplayName : null;
            }
            
            // 説明文
            if (selectedItemDescriptionText != null)
            {
                selectedItemDescriptionText.text = item != null ? item.GetDescription : null;
            }
        }

        private int IndexOf(ItemSlotUI itemSlot)
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (itemSlots[i] == itemSlot)
                    return i;
            }

            return -1;
        }

        private int FindFirstNonEmptyIndex()
        {
            for (int i = 0 ; i < itemSlots.Length; i++)
            {
                if (!itemSlots[i].IsEmpty)
                    return i;
            }

            return -1;
        }
        
        // TODO: 要リファクタリング
        /// 銃が要求する弾薬種の弾を1発消費
        public void ConsumeOneAmmoForEquippedGun(GunModel gunModel)
        {
            if (!TryGetRequiredAmmoType(gunModel, out var ammoType))
                return;

            if (!TryFindAmmoItemAndSlot(ammoType, out var ammoItem, out var slot))
                return;

            DecreaseAmmo(slot, ammoItem, 1);
        }

        /// 銃が要求するAmmoTypeを取得
        private bool TryGetRequiredAmmoType(GunModel gunModel, out AmmoType ammoType)
        {
            ammoType = default;
            var gun = gunModel.CurrentEquippedGun;
            if (gun == null) return false;

            try
            {
                ammoType = gun.GetAmmoType(); // 実装に合わせて修正
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// mapからAmmoModelかつ指定AmmoTypeのスロットを探す
        private bool TryFindAmmoItemAndSlot(AmmoType type, out IItem ammoItem, out ItemSlotUI slot)
        {
            foreach (var kv in map)
            {
                if (kv.Key is AmmoModel ammo && ammo.GetAmmoType == type)
                {
                    ammoItem = kv.Key;
                    slot = kv.Value;
                    return true;
                }
            }
            
            ammoItem = null;
            slot = null;
            
            return false;
        }

        /// スロットの所持弾数を減らす。0 になったら削除。
        private void DecreaseAmmo(ItemSlotUI slot, IItem ammoItem, int amount)
        {
            var current = slot.Amount;
            var next = Mathf.Max(0, current - amount);

            slot.UpdateAmount(next); // ItemSlotUI のUI/所持数更新

            if (next == 0)
            {
                RemoveItemSlot(ammoItem); // 既存のスロット削除処理
            }
        }

        public async void PickUpItemText(float seconds, string itemName)
        {
            captionText.text = itemName + "を手に入れた";
            await UniTask.WaitForSeconds(seconds);
            captionText.text = "";
        }
    }
}