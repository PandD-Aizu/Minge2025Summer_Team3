using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class DocumentScreenPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private DocumentScreenModel model;
        [SerializeField] private DocumentScreenView view;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            SubscribeEvents();
        }

        private void Update()
        {
            view.UpdateHighlight(model.PageIndex);
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