using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class AmmoPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private AmmoModel model;
        [SerializeField] private AmmoView view;
        [SerializeField] private GunModel gunModel;
        
        private CompositeDisposable disposable = new ();

        private void Start()
        {
            SubscribeEvents();
        }

        private void Update()
        {
            model.ApplyItem();
        }

        private void SubscribeEvents()
        {
            model.OnApplied
                .Subscribe(_ =>
                {
                    gunModel.AddAmmo(model.GetAmmoType, model.GetAmmoCount);
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