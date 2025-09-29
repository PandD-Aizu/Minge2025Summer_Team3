using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class BatteryLevelPresenter : MonoBehaviour, IDisposable 
    {
        [SerializeField] private BatteryLevelModel model;
        [SerializeField] private BatteryLevelView view;
        [SerializeField] private FlashLightFlickerModel lightFlicker;
        
        private CompositeDisposable disposables = new ();

        private void Start()
        {
            view.Initialize();
            
            SubscribeEvents();
        }

        private void Update()
        {
            model.DrainBattery();
            model.CheckFlicker();
            
            view.UpdateSegments(model.GetMaxBatteryLevel, model.GetBatteryLevel);
        }

        private void SubscribeEvents()
        {
            model.IsFlickeringObservable
                .Subscribe(isFlickering =>
                {
                    if (isFlickering)
                        lightFlicker.SetFlickerState(FlickerState.NORMALFLICKER, view.GetFlashLight);
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