using System;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.FearEffectScript
{
    public class SubliminalEffectPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private SubliminalEffectModel model;
        [SerializeField] private SubliminalEffectView view;
        [SerializeField] private SubliminalEffectEmitter emitter;

        private CompositeDisposable disposables = new CompositeDisposable();

        private async void Start()
        {
            SubscribeEvents();
            await model.ShowEffectRoutine(view.GetSubliminalImageCount);
        }

        private void Update()
        {
            
        }

        private void SubscribeEvents()
        {
            model.OnSelectImage
                .Subscribe(selectedIndex =>
                {
                    view.SelectImage(selectedIndex);
                })
                .AddTo(disposables);

            model.OnSwitchImage
                .Subscribe(isActive =>
                {
                    view.SwitchImage(isActive);
                    
                    if (isActive)
                        emitter.PlayWhiteNoise();
                    else
                        emitter.StopWhiteNoise();
                })
                .AddTo(disposables);
        }

        public void OnDestroy()
        {
            Dispose();   
        }

        public void Dispose()
        {
            disposables?.Dispose();
        }
    }
}