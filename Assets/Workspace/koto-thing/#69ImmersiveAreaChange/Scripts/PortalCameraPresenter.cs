using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class PortalCameraPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private PortalCameraModel model;
        [SerializeField] private PortalCameraView view;

        private CompositeDisposable disposables = new();

        private void Start()
        {
            
        }

        private void Update()
        {
            
        }

        private void LateUpdate()
        {
            // プレイヤーカメラとポータル入口の相対位置と回転を計算
            Vector3 playerOffsetFromPortal = model.PlayerCamera.position - model.EntryPortal.position;
            Quaternion playerRotation = Quaternion.Inverse(model.EntryPortal.rotation) *
                                        model.PlayerCamera.rotation;
            
            // プレイヤーカメラとポータル出口の相対位置と回転を計算
            transform.position = model.ExitPortal.position +
                                 model.ExitPortal.rotation * playerOffsetFromPortal;
            transform.rotation = model.ExitPortal.rotation * playerRotation;
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