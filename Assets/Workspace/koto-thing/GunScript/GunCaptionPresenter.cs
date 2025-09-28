using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class GunCaptionPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private GunCaptionModel model;
        [SerializeField] private GunCaptionView view;
        [SerializeField] private GunModel gunModel;

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
            gunModel.NotifyReload
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