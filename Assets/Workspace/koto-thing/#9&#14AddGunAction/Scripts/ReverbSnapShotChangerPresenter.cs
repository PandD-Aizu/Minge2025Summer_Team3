using System;
using UnityEngine;

namespace Acoustics
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
            model.CheckEnvironment();
            model.ChangeSnapShot();
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