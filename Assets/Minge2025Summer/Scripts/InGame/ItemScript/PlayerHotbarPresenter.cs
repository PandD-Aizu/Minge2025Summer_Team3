using System;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ItemScript
{
    public class PlayerHotbarPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private PlayerHotbarModel model;
        [SerializeField] private PlayerHotbarView view;
        [SerializeField] private PlayerItemModel playerItemModel;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            model.Initialize();
            SubscribeEvents();
        }

        private void Update()
        {
            var slots = model.Slots;
            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                if (slot.key != KeyCode.None && Input.GetKeyDown(slot.key))
                {
                    model.UseSlot(i);
                }
            }
        }

        private void SubscribeEvents()
        {
            // スロット更新
            model.Bus.OnAssigned
                .Subscribe(e =>
                {
                    view?.UpdateSlot(e.SlotIndex, e.Item);
                })
                .AddTo(disposables);
            
            // スロット使用
            model.Bus.OnUsed
                .Subscribe(e =>
                {
                    view?.PlayUseFeedback(e.SlotIndex, e.Consumed);

                    if (e.Consumed && e.Item != null)
                    {
                        if (e.Item.GetAmount <= 0)
                            model.Unassign(e.SlotIndex);
                        else
                            view?.UpdateSlot(e.SlotIndex, e.Item);
                    }
                })
                .AddTo(disposables);

            // アイテム取得時に自動割り当て
            playerItemModel.OnItemChanged
                .Subscribe(ev =>
                {
                    if (!ev.Removed)
                        model.AutoAssignFirstEmptySlot(ev.Item);
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