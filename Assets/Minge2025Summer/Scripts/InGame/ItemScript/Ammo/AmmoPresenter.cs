using System;
using Minge2025Summer.Scripts.InGame.GunScript;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ItemScript.Ammo
{
    public class AmmoPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private AmmoModel model;
        [SerializeField] private AmmoView view;
        [SerializeField] private GunModel gunModel;
        
        private CompositeDisposable disposable = new ();

        private void Start()
        {
            if (gunModel == null)
            {
                gunModel = FindFirstObjectByType<GunModel>();
            }
            SubscribeEvents();
        }

        private void Update()
        {
            
        }

        private void SubscribeEvents()
        {
            model.OnApplied
                .Subscribe(_ =>
                {
                    gunModel.AddAmmo(model.GetAmmoType, model.GetAmount);
                    view.Hide();
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