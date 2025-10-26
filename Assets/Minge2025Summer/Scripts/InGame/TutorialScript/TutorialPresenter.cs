using System;
using Minge2025Summer.Scripts.InGame.TutorialScript.Enum;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.TutorialScript
{
    public class TutorialPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private TutorialModel model;
        [SerializeField] private TutorialView view;

        private readonly CompositeDisposable disposables = new();

        private void Start()
        {
            SubscribeEvents();
            
            model.Initialize();
            model.RaiseShow(TutorialType.MOVE);
        }

        private void SubscribeEvents()
        {
            model.OnShow
                .Subscribe(type =>
                {
                    var text = model.GetText(type);
                    view.Show(text, 3.0f);
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
