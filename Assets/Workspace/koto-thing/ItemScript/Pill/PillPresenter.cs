using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
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
                    playerHpModel.CurrentHp += model.GetAmount;
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