using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class LoadScreenPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private LoadScreenModel model;
        [SerializeField] private LoadScreenView view;
        
        private CompositeDisposable disposables = new ();
        
        private void Start()
        {
            SubscribeEvents();
            gameObject.SetActive(true);
        }

        private void Update()
        {
           model.UpdateProgressBar();
        }

        private void SubscribeEvents()
        {
            model.LoadProgressObservable
                .Subscribe(progress =>
                {
                    model.CheckFinishLoading();
                    view.UpdateLoadSlider(progress);
                })
                .AddTo(disposables);

            model.IsFinishLoadingObservable
                .Subscribe(isFinishLoading =>
                {
                    if (isFinishLoading)
                    {
                        view.ChangeActive();
                    }
                });
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