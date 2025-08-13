using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerInteractionPresenter : MonoBehaviour
    {
        [SerializeField] private PlayerInteractionModel model;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.E))
            {
                model.Interact();
            }
        }

        private void SubscribeEvents()
        {
            model.InteractObserver
                .Skip(1)
                .Subscribe((isIntaract) =>
                {
                    
                })
                .AddTo(disposables);
        }

        public void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}