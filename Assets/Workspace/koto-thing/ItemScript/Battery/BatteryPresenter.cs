using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing.Battery
{
    public class BatteryPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private BatteryModel model;
        [SerializeField] private BatteryView view;
        [SerializeField] private BatteryLevelModel batteryLevelModel;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            batteryLevelModel = FindFirstObjectByType<BatteryLevelModel>();
            
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
                    batteryLevelModel.RechargeBattery(model.GetIlluminationTime);
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