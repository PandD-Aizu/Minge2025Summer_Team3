using System;
using Minge2025Summer.Scripts.InGame.FlashLightScript.Enum;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.FlashLightScript
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
            model.DrainBattery(lightFlicker.GetCurrentState);
            model.CheckFlicker();

            if (model.GetBatteryLevel <= 0)
            {
                lightFlicker.SetFlickerState(FlickerState.OFF, view.GetFlashLight);
            }
                
            
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