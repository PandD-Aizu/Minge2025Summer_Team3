using System;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EquipmentScript
{
    public class EquipmentPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private EquipmentModel model;
        [SerializeField] private EquipmentView view;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            SubscribeEvents();
        }

        private void Update()
        {
            
        }
        
        private void SubscribeEvents()
        {
            
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