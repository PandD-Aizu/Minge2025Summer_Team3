using System;
using System.Collections.Generic;
using Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Enum;
using Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Interface;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript.ItemScript
{
    public class ReiItemInventoryModel : MonoBehaviour
    {
        private Dictionary<string, int> consumableItemInventory = new ();
        private Dictionary<string, int> ammoItemInventory = new ();
        private Dictionary<string, int> keyItemInventory = new ();
        
        private Dictionary<string, IConsumableItem> consumableItemDatabase = new ();
        private Dictionary<string, IAmmoItem> ammoItemDatabase = new ();
        private Dictionary<string, IKeyItem> keyItemDatabase = new ();

        public Dictionary<string, int> GetConsumableItemInventory => consumableItemInventory;
        public Dictionary<string, int> GetAmmoItemInventory => ammoItemInventory;
        public Dictionary<string, int> GetKeyItemInventory => keyItemInventory;

        private Subject<Unit> onInventoryChanged = new ();
        public IObservable<Unit> OnInventoryChanged => onInventoryChanged;
        private Subject<IReiItem> onUseItem = new ();
        public IObservable<IReiItem> OnUseItem => onUseItem;

        public void AddItem(IReiItem item)
        {
            var itemName = item.GetItemName;

            // インタンス型で判定して正しく登録する
            if (item is IConsumableItem consumable)
            {
                if (consumableItemInventory.ContainsKey(itemName))
                {
                    consumableItemInventory[itemName] += consumable.GetItemAmount;
                }
                else
                {
                    consumableItemInventory[itemName] = consumable.GetItemAmount;
                    consumableItemDatabase[itemName] = consumable;
                }
                
                onInventoryChanged.OnNext(Unit.Default);
            }
            // 弾薬アイテムの追加処理
            else if (item is IAmmoItem ammo)
            {
                if (ammoItemInventory.ContainsKey(itemName))
                {
                    ammoItemInventory[itemName] += ammo.GetItemAmount;
                }
                else
                {
                    ammoItemInventory[itemName] = ammo.GetItemAmount;
                    ammoItemDatabase[itemName] = ammo;
                }
                
                onInventoryChanged.OnNext(Unit.Default);
            }
            // 鍵アイテムの追加処理
            else if (item is IKeyItem keyItem)
            {
                if (keyItemInventory.ContainsKey(itemName))
                {
                    keyItemInventory[itemName]++;
                }
                else
                {
                    keyItemInventory[itemName] = 1;
                    keyItemDatabase[itemName] = keyItem;
                }
                
                onInventoryChanged.OnNext(Unit.Default);
            }
        }

        public bool UseItem(string itemID, Type itemType)
        {
            // 通常の消費アイテムの使用処理
            if (itemType == typeof(IConsumableItem))
            {
                if (consumableItemInventory.ContainsKey(itemID) && consumableItemInventory[itemID] > 0)
                {
                    if (consumableItemDatabase.TryGetValue(itemID, out var item))
                    {
                        item.ApplyItem();
                        consumableItemInventory[itemID]--;

                        if (consumableItemInventory[itemID] < 0)
                        {
                            consumableItemInventory.Remove(itemID);
                        }
                        
                        onInventoryChanged.OnNext(Unit.Default);
                        onUseItem.OnNext(item);
                        return true;
                    }
                }
            }
            // 弾薬アイテムの使用処理
            else if (itemType == typeof(IAmmoItem))
            {
                if (ammoItemInventory.ContainsKey(itemID) && ammoItemInventory[itemID] > 0)
                {
                    if (ammoItemDatabase.TryGetValue(itemID, out var item))
                    {
                        onInventoryChanged.OnNext(Unit.Default);
                        onUseItem.OnNext(item);
                        return true;
                    }
                }   
            }
            // 鍵アイテムの使用処理
            else if (itemType == typeof(IKeyItem))
            {
                if (keyItemInventory.ContainsKey(itemID) && keyItemInventory[itemID] > 0)
                {
                    if (keyItemDatabase.TryGetValue(itemID, out var item))
                    {
                        // 鍵アイテムの適用に失敗したら使用しない
                        if (!item.ApplyItem())
                            return false;
                        
                        keyItemInventory[itemID]--;

                        if (keyItemInventory[itemID] <= 0)
                        {
                            keyItemInventory.Remove(itemID);
                        }
                    }
                    
                    onInventoryChanged.OnNext(Unit.Default);
                    onUseItem.OnNext(item);
                    return true;
                }
            }

            return false;
        }

        public IReiItem GetItem(string itemID, Type itemType)
        {
            if (itemType == typeof(IConsumableItem))
            {
                if (consumableItemDatabase.TryGetValue(itemID, out var item))
                {
                    return item;
                }
            }
            else if (itemType == typeof(IAmmoItem))
            {
                if (ammoItemDatabase.TryGetValue(itemID, out var item))
                {
                    return item;
                }
            }
            else if (itemType == typeof(IKeyItem))
            {
                if (keyItemDatabase.TryGetValue(itemID, out var item))
                {
                    return item;
                }
            }

            return null;
        }
        
        #region Function For AmmoManagement
        public int GetAmmoCount(AmmoType ammoType)
        {
            int total = 0;
            foreach (var kvp in ammoItemInventory)
            {
                var id = kvp.Key;
                var count = kvp.Value;
                if (count <= 0) continue;
                if (ammoItemDatabase.TryGetValue(id, out var item))
                {
                    if (item.GetAmmoType == ammoType)
                        total += count;
                }
            }

            return total;
        }

        public int TryConsumeAmmo(AmmoType ammoType, int amount)
        {
            if (amount <= 0) return 0;
            int remaining = amount;
            // コピーしたキーでループ（辞書の変更を許容）
            var keys = new List<string>(ammoItemInventory.Keys);
            foreach (var id in keys)
            {
                if (remaining <= 0) break;
                if (!ammoItemInventory.TryGetValue(id, out var available) || available <= 0) continue;
                if (!ammoItemDatabase.TryGetValue(id, out var ammoItem)) continue;
                if (ammoItem.GetAmmoType != ammoType) continue;

                int take = Math.Min(available, remaining);
                int newCount = available - take;
                if (newCount > 0)
                    ammoItemInventory[id] = newCount;
                else
                    ammoItemInventory.Remove(id);

                remaining -= take;
            }

            int consumed = amount - remaining;
            if (consumed > 0)
                onInventoryChanged.OnNext(Unit.Default);

            return consumed;
        }
        
        public void AddAmmo(AmmoType ammoType, int amount)
        {
            if (amount <= 0) return;

            string targetId = null;
            foreach (var kvp in ammoItemDatabase)
            {
                if (kvp.Value.GetAmmoType == ammoType)
                {
                    targetId = kvp.Key;
                    break;
                }
            }

            if (targetId == null) return; // データベースに該当タイプのアイテムがない場合は追加できない

            if (ammoItemInventory.ContainsKey(targetId))
                ammoItemInventory[targetId] += amount;
            else
                ammoItemInventory[targetId] = amount;

            onInventoryChanged.OnNext(Unit.Default);
        }
        #endregion
    }
}