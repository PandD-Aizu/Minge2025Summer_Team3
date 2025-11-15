using System;
using System.Collections.Generic;
using Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Interface;
using Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Struct;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript.ItemScript
{
    public class ReiItemInventoryController : MonoBehaviour, IDisposable
    {
        [SerializeField] private ReiItemInventoryModel model;
        [SerializeField] private ReiItemInventoryView view;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            view.Initialize();
            
            SubscribeEvents();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                view.ToggleInventoryPanel();
            }

            if (!view.GetInventoryPanel.activeSelf)
                return;
            
            if (Input.GetKeyDown(KeyCode.W))
                view.NavigateSlot(-1);
            else if (Input.GetKeyDown(KeyCode.S))
                view.NavigateSlot(1);
            else if (Input.GetKeyDown(KeyCode.A))
                view.NavigateSlot(-2);
            else if (Input.GetKeyDown(KeyCode.D))
                view.NavigateSlot(2);

            if (Input.GetKeyDown(KeyCode.E))
            {
                // キーアイテムはインベントリから使用できないようにする
                var idAndType = view.GetSelectedItemIDAndType();
                if (!string.IsNullOrEmpty(idAndType.Item1) && idAndType.Item2 != typeof(IKeyItem))
                {
                    model.UseItem(idAndType.Item1, idAndType.Item2);
                }
            }
            
            
        }

        private void SubscribeEvents()
        {
            model.OnInventoryChanged
                .Subscribe(_ =>
                {
                    Debug.Log("[ReiItemInventoryController] Inventory changed, updating view.");
                    var slotDataList = new List<ItemSlotData>();

                    foreach (var kvp in model.GetConsumableItemInventory)
                    {
                        var itemID = kvp.Key;
                        var amount = kvp.Value;
                        if (amount <= 0) continue;
                        var item = model.GetItem(itemID, typeof(IConsumableItem));
                        var icon = item != null ? item.GetIcon : null;

                        slotDataList.Add(new ItemSlotData
                        {
                            ItemID = itemID,
                            ItemType = typeof(IConsumableItem),
                            Icon = icon,
                            Amount = amount
                        });
                    }
                    
                    foreach (var kvp in model.GetAmmoItemInventory)
                    {
                        var itemID = kvp.Key;
                        var amount = kvp.Value;
                        if (amount <= 0) continue;
                        var item = model.GetItem(itemID, typeof(IAmmoItem));
                        var icon = item != null ? item.GetIcon : null;

                        slotDataList.Add(new ItemSlotData
                        {
                            ItemID = itemID,
                            ItemType = typeof(IAmmoItem),
                            Icon = icon,
                            Amount = amount
                        });
                    }

                    foreach (var kvp in model.GetKeyItemInventory)
                    {
                        var itemID = kvp.Key;
                        var amount = kvp.Value;
                        if (amount <= 0) continue;
                        var item = model.GetItem(itemID, typeof(IKeyItem));
                        var icon = item != null ? item.GetIcon : null;

                        slotDataList.Add(new ItemSlotData
                        {
                            ItemID = itemID,
                            ItemType = typeof(IKeyItem),
                            Icon = icon,
                            Amount = amount
                        });
                    }

                    view.UpdateInventory(slotDataList);
                })
                .AddTo(disposables);
            
            model.OnUseItem
                .Subscribe(item =>
                {
                    view.NotifyItemUsed(item);
                })
                .AddTo(disposables);

            view.OnInventorySelected
                .Subscribe(_ =>
                {
                    if (view == null || model == null)
                    {
                        Debug.LogError("[ReiItemInventoryController] View or Model is null in OnInventorySelected");
                        return;
                    }

                    var (id, type) = view.GetSelectedItemIDAndType();

                    if (string.IsNullOrEmpty(id) || type == null)
                    {
                        view.SetMainItemText(string.Empty, string.Empty);
                        return;
                    }

                    var item = model.GetItem(id, type);
                    if (item == null)
                    {
                        Debug.LogWarning("[ReiItemInventoryController] Selected item not found in model: " + id);
                        view.SetMainItemText(string.Empty, string.Empty);
                        return;
                    }

                    var displayName = item.GetDisplayName ?? string.Empty;
                    var desc = item.GetItemDescription ?? string.Empty;
                    view.SetMainItemText(displayName, desc);
                })
                .AddTo(disposables);
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}