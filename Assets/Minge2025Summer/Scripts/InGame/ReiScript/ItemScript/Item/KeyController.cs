using System;
using FMODUnity;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Item
{
    public class KeyController : MonoBehaviour, IDisposable
    {
        [SerializeField] Key model;

        private StudioEventEmitter emitter;
        private CompositeDisposable disposables = new();

        private void Start()
        {
            emitter = GameObject.Find("KeyApplyEmitter").GetComponent<StudioEventEmitter>();
            
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            model.OnGetItem
                .Subscribe(_ =>
                {
                    model.HideItem();
                })
                .AddTo(disposables);
            
            model.OnApplyItem
                .Subscribe(_ =>
                {
                    emitter.Play();
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