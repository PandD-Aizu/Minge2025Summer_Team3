using System;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Main.InGame
{
    public class TutorialPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private TutorialModel model;
        [SerializeField] private TutorialView view;

        private readonly CompositeDisposable disposables = new();

        private void Start()
        {
            model.Initialize();
            
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            model.OnShow
                .Subscribe(type =>
                {
                    var text = model.GetText(type);
                    view.Show(text);
                })
                .AddTo(disposables);

            model.OnHide
                .Subscribe(_ =>
                {
                    view.Hide();
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
