using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript
{
    public class ReiItemInventoryModel : MonoBehaviour
    {
        private Dictionary<string, int> consumableItemInventory = new ();
        private Dictionary<string, int> keyItemInventory = new ();
        
        private Dictionary<string, IConsumableItem> consumableItemDatabase = new ();
        private Dictionary<string, IKeyItem> keyItemDatabase = new ();

        public Dictionary<string, int> GetConsumableItemInventory => consumableItemInventory;
        public Dictionary<string, int> GetKeyItemInventory => keyItemInventory;

        private Subject<Unit> onInventoryChanged = new ();
        public IObservable<Unit> OnInventoryChanged => onInventoryChanged;
        private Subject<IReiItem> onUseItem = new ();
        public IObservable<IReiItem> OnUseItem => onUseItem;

        public void AddItem(IReiItem item)
        {
            var itemName = item.GetItemName; // アイテムの名前をIDとして使用
            var itemType = item.GetType();   // アイテムの型を取得
            
            // 通常の消費アイテムの追加処理
            if (itemType == typeof(IConsumableItem))
            {
                // 通常の消費アイテムの追加処理
                if (consumableItemInventory.ContainsKey(itemName))
                {
                    consumableItemInventory[itemName] += (item as IConsumableItem)?.GetItemAmount ?? 1;
                }
                // 新しいアイテムの場合、データベースに登録
                else
                {
                    consumableItemInventory[itemName] = (item as IConsumableItem)?.GetItemAmount ?? 1;
                    consumableItemDatabase[itemName] = (IConsumableItem)item;
                }
                
                onInventoryChanged.OnNext(Unit.Default);
            }
            // 鍵アイテムの追加処理
            else if (itemType == typeof(IKeyItem))
            {
                if (keyItemInventory.ContainsKey(itemName))
                {
                    keyItemInventory[itemName]++;
                }
                else
                {
                    keyItemInventory[itemName] = 1;
                    keyItemDatabase[itemName] = (IKeyItem)item;
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
            // 鍵アイテムの使用処理
            else if (itemType == typeof(IKeyItem))
            {
                if (keyItemInventory.ContainsKey(itemID) && keyItemInventory[itemID] > 0)
                {
                    if (keyItemDatabase.TryGetValue(itemID, out var item))
                    {
                        // 鍵アイテムの適用に失敗したら使用しない
                        if (item.ApplyItem())
                            return false;
                        
                        keyItemInventory[itemID]--;

                        if (keyItemInventory[itemID] < 0)
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
            else if (itemType == typeof(IKeyItem))
            {
                if (keyItemDatabase.TryGetValue(itemID, out var item))
                {
                    return item;
                }
            }

            return null;
        }
    }
}