using System;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript.ItemScript
{
    public class ReiItemInteractionController : MonoBehaviour, IDisposable
    {
        [SerializeField] private ReiItemInteractionModel model;
        [SerializeField] private ReiItemInventoryModel inventoryModel;
        [SerializeField] private ReiItemInteractionView view;
        [SerializeField] private ReiItemInteractionEmitter emitter;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private void Start()
        {
            SubscribeEvents();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                model.Interact(inventoryModel);
            }
        }

        private void SubscribeEvents()
        {
            model.OnInteractItem
                .Subscribe(itemName =>
                {
                    view.Notify(itemName);
                    emitter.PlayItemGetSound();
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