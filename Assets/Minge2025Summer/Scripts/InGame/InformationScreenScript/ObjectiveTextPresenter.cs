using System.Collections.Generic;
using UniRx;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Minge2025Summer.Main.InGame
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
