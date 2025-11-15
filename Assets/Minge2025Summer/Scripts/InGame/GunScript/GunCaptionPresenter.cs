using System;
using Minge2025Summer.Scripts.InGame.ReiScript.GunScript;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.GunScript
{
    public class GunCaptionPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private GunCaptionModel model;
        [SerializeField] private GunCaptionView view;
        [SerializeField] private WeaponModel weaponModel;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            SubscribeEvents();
        }

        private void Update()
        {
            
        }

        private void SubscribeEvents()
        {
            weaponModel.OnNotifyReload
                .Subscribe(_ =>
                {
                    view.ShowCaption(model.GetRandomReloadText());
                })
                .AddTo(disposables);
        }

        public void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}