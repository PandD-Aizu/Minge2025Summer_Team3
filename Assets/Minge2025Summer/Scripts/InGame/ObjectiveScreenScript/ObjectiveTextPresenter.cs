using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ObjectiveScreenScript
{
    public class ObjectiveTextPresenter : MonoBehaviour
    {
        [SerializeField] private ObjectiveTextModel model;
        [SerializeField] private ObjectiveTextView view;

        private readonly CompositeDisposable disposables = new();

        private void Start()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            model.OnShow
                .Subscribe(data =>
                {
                    view.UpdateView(data);
                })
                .AddTo(disposables);
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}
