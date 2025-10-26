using System;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.CaptionScript
{
    public class CaptionPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private CaptionModel model;
        [SerializeField] private CaptionView view;
        
        private CompositeDisposable disposables = new CompositeDisposable();

        private void Start()
        {
            SubscribeEvents();
            
            model.RaiseShow("ここが例の...");
        }

        private void SubscribeEvents()
        {
            model.OnShow
                .Subscribe(text => view.Show(text, model.DisplayDuration))
                .AddTo(disposables);
            
            model.OnHide
                .Subscribe(_ => view.Hide())
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