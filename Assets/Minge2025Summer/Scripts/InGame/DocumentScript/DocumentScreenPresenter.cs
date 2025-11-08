using System;
using System.Collections.Generic;
using UniRx;
using Unity.Cinemachine;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.DocumentScript
{
    public class DocumentScreenPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private DocumentScreenModel model;
        [SerializeField] private DocumentScreenView view;
        [SerializeField] private List<GameObject> playerMovementControllers;
        [SerializeField] private List<CinemachineInputAxisController> cinemachineInputAxisControllers;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            SubscribeEvents();
        }

        private void Update()
        {
            view.UpdateHighlight(model.PageIndex);
        }

        private void OnEnable()
        {
            // 視点移動を無効化
            foreach (var axisController in cinemachineInputAxisControllers)
            {
                axisController.enabled = false;
            }
            
            // プレイヤー移動を無効化
            foreach (var controller in playerMovementControllers)
            {
                controller.SetActive(false);
            }
        }
        
        private void OnDisable()
        {
            // 視点移動を有効化
            foreach (var axisController in cinemachineInputAxisControllers)
            {
                axisController.enabled = true;
            }

            // プレイヤー移動を有効化
            foreach (var controller in playerMovementControllers)
            {
                controller.SetActive(true);
            }
        }

        private void SubscribeEvents()
        {
            model.OnChanged
                .Subscribe(model =>
                {
                    view.Apply(model);
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