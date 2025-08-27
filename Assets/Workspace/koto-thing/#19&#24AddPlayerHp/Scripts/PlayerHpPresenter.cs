using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerHpPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private PlayerHpModel model;
        [SerializeField] private PlayerHpView view;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            SubscribeEvents();
            model.CurrentHp = model.GetMaxHp; // 初期化
        }

        private void Update()
        {
            
        }

        private void SubscribeEvents()
        {
            model.CurrentHpObservable
                .Subscribe(hp => view.UpdateHpText(hp, model.GetMaxHp))
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