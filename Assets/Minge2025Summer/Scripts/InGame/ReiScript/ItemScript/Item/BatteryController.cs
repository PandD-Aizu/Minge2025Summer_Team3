using System;
using FMODUnity;
using Minge2025Summer.Scripts.InGame.FlashLightScript;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Item
{
    public class BatteryController : MonoBehaviour, IDisposable
    {
        [SerializeField] private Battery model;

        private BatteryLevelModel batteryLevelModel;
        private StudioEventEmitter emitter;
        private CompositeDisposable disposables = new();

        private void Start()
        {
            batteryLevelModel = FindFirstObjectByType<BatteryLevelModel>();
            emitter = GameObject.Find("BatteryApplyEmitter").GetComponent<StudioEventEmitter>();
            
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
                .Subscribe(illuminationAmount =>
                {
                    batteryLevelModel.RechargeBattery(illuminationAmount);
                    emitter.Play();
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