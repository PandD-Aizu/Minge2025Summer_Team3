using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerItemPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private PlayerItemModel model;
        [SerializeField] private PlayerItemView view;
        [SerializeField] private ItemEmitter emitter;
        [SerializeField] private GunModel gunModel;

        private CompositeDisposable disposable = new ();

        private void Start()
        {
            SubscribeEvents();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                model.GetItem();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                view.SwitchInventoryScreen();
            }
            
            if (view.IsOpen)
            {
                if (Input.GetKeyDown(KeyCode.W)) view.MoveSelection(Vector2Int.down);
                if (Input.GetKeyDown(KeyCode.S)) view.MoveSelection(Vector2Int.up);
                if (Input.GetKeyDown(KeyCode.A)) view.MoveSelection(Vector2Int.left);
                if (Input.GetKeyDown(KeyCode.D)) view.MoveSelection(Vector2Int.right);
            }
        }

        private void SubscribeEvents()
        {
            model.OnItemChanged
                .Subscribe(itemChangeEvent =>
                {
                    if (itemChangeEvent.Removed)
                        view.RemoveItemSlot(itemChangeEvent.Item);
                    else
                        view.UpdateItemSlot(itemChangeEvent.Item, itemChangeEvent.Amount);
                    
                    emitter.PlayPickUp();
                })
                .AddTo(disposable);

            gunModel.CurrentEquippedGun?.OnFire
                .Subscribe(_ =>
                {
                    view.ConsumeOneAmmoForEquippedGun(gunModel);
                })
                .AddTo(disposable);
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}