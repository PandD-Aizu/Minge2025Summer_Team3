using System;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.AcousticsScript
{
    public class ReverbSnapShotChangerPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private ReverbSnapShotChangerModel model;
        [SerializeField] private ReverbSnapShotChangerView view;
        
        private void Start()
        {
            
        }

        private void Update()
        {
            model.UpdateReverbEnvironment();
            model.DebugRayCast();
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
            
        }
    }
}