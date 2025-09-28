using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
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
            model.Bus.OnAssigned
                .Subscribe(e =>
                {
                    view?.UpdateSlot(e.SlotIndex, e.Item);
                })
                .AddTo(disposables);
            
            model.Bus.OnUsed
                .Subscribe(e =>
                {
                    view?.PlayUseFeedback(e.SlotIndex, e.Consumed);
                    if (e.Consumed && e.Item != null && e.Item.GetIsApplied)
                    {
                        model.Unassign(e.SlotIndex);
                    }
                })
                .AddTo(disposables);

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