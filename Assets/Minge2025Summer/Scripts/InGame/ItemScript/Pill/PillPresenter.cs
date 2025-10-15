using System;
using Minge2025Summer.Scripts.InGame.PlayerStatusScript;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ItemScript.Pill
{
    public class PillPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private PillModel model;
        [SerializeField] private PillView view;
        [SerializeField] private PlayerHpModel playerHpModel;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            playerHpModel = FindFirstObjectByType<PlayerHpModel>();
            
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            model.OnApplied
                .Subscribe(_ =>
                {
                    playerHpModel.CurrentHp += model.GetHealAmount;
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