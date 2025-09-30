using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing.PlayerStatusScript
{
    public class PlayerHpPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private PlayerHpModel model;
        [SerializeField] private PlayerHpView view;
        [SerializeField] private BattleBGMController battleBGMController;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            view.Initialize();
            
            SubscribeEvents();
            model.CurrentHp = model.GetMaxHp;
        }

        private void Update()
        {
            view.UpdateHealth(model.GetMaxHp, model.CurrentHp);
            view.UpdateBeat();
            view.UpdateDisplay(Time.deltaTime);
        }

        private void SubscribeEvents()
        {
            model.CurrentHpObservable
                .Subscribe(hp =>
                {
                    battleBGMController.ChangeBGM(hp, model.GetMaxHp);
                })
                .AddTo(disposables);
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