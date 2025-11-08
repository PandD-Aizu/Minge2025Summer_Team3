using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript
{
    public class ReiItemInventoryController : MonoBehaviour, IDisposable
    {
        [SerializeField] private ReiItemInventoryModel model;
        [SerializeField] private ReiItemInventoryView view;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
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
                var IDAndType = view.GetSelectedItemIDAndType();
                if (!string.IsNullOrEmpty(IDAndType.Item1) && IDAndType.Item2 != typeof(IKeyItem))
                {
                    model.UseItem(IDAndType.Item1, IDAndType.Item2);
                }
            }
        }

        private void SubscribeEvents()
        {
            model.OnInventoryChanged
                .Subscribe(_ =>
                {
                    var slotDataList = new List<ItemSlotData>();

                    foreach (var kvp in model.GetConsumableItemInventory)
                    {
                        var itemID = kvp.Key;
                        var amount = kvp.Value;
                        var icon = model.GetItem(itemID, typeof(IConsumableItem)).GetIcon;
                        
                        slotDataList.Add(new ItemSlotData
                        {
                            ItemID = itemID,
                            ItemType = typeof(IConsumableItem),
                            Icon = icon,
                            Amount = amount
                        });
                    }

                    foreach (var kvp in model.GetKeyItemInventory)
                    {
                        var itemID = kvp.Key;
                        var amount = kvp.Value;
                        var icon = model.GetItem(itemID, typeof(IKeyItem)).GetIcon;
                        
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