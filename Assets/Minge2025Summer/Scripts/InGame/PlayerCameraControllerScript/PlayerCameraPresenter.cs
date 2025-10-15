using System;
using Minge2025Summer.Scripts.InGame.PlayerTransformScript;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.PlayerCameraControllerScript
{
    public class PlayerCameraPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private PlayerCameraModel model;
        [SerializeField] private PlayerCameraView view;
        [SerializeField] private PlayerPositionModel positionModel;
        
        private CompositeDisposable disposables = new ();

        private void Start()
        {
            SubscribeEvents();
        }

        private void Update()
        {
            model.UpdateCameraHeight();
        }

        private void SubscribeEvents()
        {
            positionModel.IsCrouchingObservable
                .Subscribe(isCrouching =>
                {
                    model.SetCameraHeight(isCrouching);
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