using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class HintMarkerPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private HintMarkerModel model;
        [SerializeField] private HintMarkerView view;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            SubscribeEvents();
        }

        private void Update()
        {
            view.RotateTowardsCamera();
            view.UpdateAlphaByDistance(model.PlayerTransform, model.GetMaxDistance, model.GetMinDistance);
        }
        
        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                view.SwitchVisibility(true);
                model.PlayerTransform = other.transform;
            }
        }
        
        public void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                view.SwitchVisibility(false);
                model.PlayerTransform = null;
            }
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