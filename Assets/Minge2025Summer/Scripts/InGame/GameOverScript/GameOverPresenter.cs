using System;
using Minge2025Summer.Scripts.InGame.FlashLightScript;
using Minge2025Summer.Scripts.InGame.PlayerStatusScript;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.GameOverScript
{
    public class GameOverPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private GameOverModel model;
        [SerializeField] private GameOverView view;

        [SerializeField] private PlayerHpModel playerHpModel;
        [SerializeField] private BatteryLevelModel batteryLevelModel;

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
            
            batteryLevelModel.OnBatteryDepleted
                .Subscribe(_ =>
                {
                    view.ShowGameOverPanel();
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