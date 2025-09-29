using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class KeyPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private KeyModel model;
        [SerializeField] private KeyView view;

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
            model.OnApplied
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