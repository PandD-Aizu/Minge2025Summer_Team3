using System;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Workspace.koto_thing
{
    public class FlashLightFlickerPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private FlashLightFlickerModel model;
        [SerializeField] private FlashLightFlickerView view;
        [SerializeField] private FlashLightFlickerEmitter emitter;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            model.RandomOffset = Random.Range(0.0f, 1000.0f);
            model.SetFlickerState(model.GetCurrentState, view.GetFlashLight);
        }

        private void Update()
        {
            if (model.GetCurrentState == FlickerState.STABLE || model.GetCurrentState == FlickerState.OFF)
                emitter.StopFlickerSound();
            else 
                emitter.PlayFlickerSound();
            
            if (model.GetCurrentState == FlickerState.NORMALFLICKER)
                view.NormalFlicker(model.GetNormalFlickerSpeed, model.RandomOffset, model.GetMinIntensity, model.GetMaxIntensity);
            else if (model.GetCurrentState == FlickerState.INTENSEFLICKER)
                view.IntenseFlicker(model.GetIntenseFlickerSpeed, model.RandomOffset, model.GetIntenseMinIntensity, model.GetIntenseMaxIntensity);
        }

        private void SubscribeEvents()
        {
            
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