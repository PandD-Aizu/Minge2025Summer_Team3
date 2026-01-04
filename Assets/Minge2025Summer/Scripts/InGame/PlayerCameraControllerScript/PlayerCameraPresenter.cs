using System;
using Minge2025Summer.Scripts.GameSetting;
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
            // マウス感度を設定
            if (GameController.Instance != null)
            {
                model.CameraSensitivity = GameController.Instance.gameSettingsController.gameSettings.cameraSensitivity;
                foreach (var controller in model.InputAxisController.Controllers)
                {
                    Debug.Log("Controller Name: " + controller.Name);
                    if (controller.Name == "Look X (Pan)")
                    {
                        controller.Input.Gain = model.CameraSensitivity;
                    }

                    if (controller.Name == "Look Y (Tilt)")
                    {
                        controller.Input.Gain = -model.CameraSensitivity;
                    }
                }
            }
            
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