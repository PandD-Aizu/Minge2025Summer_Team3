using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class DoorPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private DoorModel model;
        [SerializeField] private DoorView view;
        [SerializeField] private DoorEmitter emitter;

        private readonly CompositeDisposable disposables = new();

        private void Start()
        {
            view.Initialize();
            SubscribeEvents();
            if (model != null && view != null && model.IsOpen)
            {
                view.SetInstantOpen(model);
            }
        }

        private void SubscribeEvents()
        {
            model.OnDoorOpened
                .Subscribe(_ =>
                {
                    view.PlayOpen(model);
                    emitter.PlayDoorSound();
                })
                .AddTo(disposables);

            model.OnDoorOpenFailed
                .Subscribe(_ =>
                {
                    view.PlayClose(model.OpenDuration);
                    emitter.PlayDoorSound();
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