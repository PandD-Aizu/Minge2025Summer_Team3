using System;
using Minge2025Summer.Scripts.InGame.DocumentScript;
using Minge2025Summer.Scripts.InGame.ItemScript;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.PlayerTransformScript
{
    public class PlayerPositionPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private PlayerPositionModel model;
        [SerializeField] private PlayerPositionView view;
        [SerializeField] private PlayerPositionEmitter emitter;
        [SerializeField] private PlayerItemView itemView;
        [SerializeField] private PlayerDocumentModel documentModel;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            SubscribeEvents();
        }

        private void Update()
        {
            // 入力処理
            Vector2 input = Vector2.zero;
            
            if (model.ForceCrouch)
                model.IsCrouching = true;
            else
                model.IsCrouching = Input.GetKey(KeyCode.Space);

            if (itemView != null && itemView.IsOpen) return;
            if (documentModel != null && documentModel.IsDocumentOpen) return;
            
            if (Input.GetKey(KeyCode.W)) 
                input.y += 1.0f;
            if (Input.GetKey(KeyCode.S)) 
                input.y -= 1.0f;
            if (Input.GetKey(KeyCode.D)) 
                input.x += 1.0f;
            if (Input.GetKey(KeyCode.A)) 
                input.x -= 1.0f;
            
            model.IsRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            
            model.Move(input);
            model.ChangeNoiseSetting(input);
            model.ChangeColliderHeight();
            
            float speed = model.GetCharacterController.velocity.magnitude;
            emitter.PlayFootStep(speed);
        }

        private void SubscribeEvents()
        {
            model.IsCrouchingObservable
                .Subscribe(_ =>
                {

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