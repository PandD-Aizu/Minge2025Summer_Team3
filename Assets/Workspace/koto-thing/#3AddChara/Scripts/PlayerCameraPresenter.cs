using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
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