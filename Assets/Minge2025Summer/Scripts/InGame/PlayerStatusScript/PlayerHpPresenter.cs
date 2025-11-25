using System;
using Minge2025Summer.Scripts.InGame.AcousticsScript;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.PlayerStatusScript
{
    public class PlayerHpPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private PlayerHpModel model;
        [SerializeField] private PlayerHpView view;
        [SerializeField] private PlayerHpEmitter emitter;
        [SerializeField] private BattleBGMController battleBGMController;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            view.Initialize();
            
            SubscribeEvents();
            model.CurrentHp = model.GetMaxHp;
            model.PreviousHp = model.GetMaxHp;
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
                    if (hp < model.PreviousHp)
                        emitter.PlayDamageSound();
                    
                    model.PreviousHp = hp;
                    
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