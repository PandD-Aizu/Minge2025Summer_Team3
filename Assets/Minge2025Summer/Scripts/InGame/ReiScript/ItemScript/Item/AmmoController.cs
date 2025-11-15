using System;
using FMODUnity;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Item
{
    public class AmmoController : MonoBehaviour, IDisposable
    {
        [SerializeField] private Ammo model;
        
        private CompositeDisposable disposables = new ();

        private void Start()
        {
            
            
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
                    model.HideItem();
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