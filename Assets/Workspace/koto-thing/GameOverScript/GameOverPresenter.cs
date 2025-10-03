using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class GameOverPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private GameOverModel model;
        [SerializeField] private GameOverView view;

        [SerializeField] private PlayerHpModel playerHpModel;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            view.Initialize();
            
            SubscribeEvents();
        }

        private void Update()
        {
            
        }

        private void SubscribeEvents()
        {
            model.OnContinueGame
                .Subscribe(_ =>
                {
                    playerHpModel.CurrentHp = playerHpModel.GetMaxHp;
                    view.HideGameOverPanel();
                })
                .AddTo(disposables);
            
            playerHpModel.CurrentHpObservable
                .Skip(1)
                .Where(hp => hp <= 0)
                .Subscribe(_ =>
                {
                    view.ShowGameOverPanel();
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