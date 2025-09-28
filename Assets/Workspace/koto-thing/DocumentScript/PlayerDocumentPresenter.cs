using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerDocumentPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private PlayerDocumentModel model;
        [SerializeField] private PlayerDocumentView view;
        [SerializeField] private DocumentScreenModel documentScreenModel;
        [SerializeField] private DocumentScreenView documentScreenView;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            SubscribeEvents();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && !model.IsDocumentOpen)
            {
                model.TryInteract();
            }

            if (Input.GetKeyDown(KeyCode.E) && model.IsDocumentOpen)
            {
                documentScreenView.Hide();
                model.IsDocumentOpen = false;
            }

            if (model.IsDocumentOpen && Input.GetKeyDown(KeyCode.A))
            {
                documentScreenModel.PrevPage();
            }
            else if (model.IsDocumentOpen && Input.GetKeyDown(KeyCode.D))
            {
                documentScreenModel.NextPage();
            }
        }

        private void SubscribeEvents()
        {
            model.OnOpenDocument
                .Subscribe(documentData =>
                {
                    documentScreenModel.Load(documentData);
                    documentScreenView.Apply(documentScreenModel);
                    documentScreenView.Show();
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