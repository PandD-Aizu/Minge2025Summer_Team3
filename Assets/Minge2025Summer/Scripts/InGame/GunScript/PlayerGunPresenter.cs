using System;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.GunScript
{
    public class PlayerGunPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private PlayerGunModel model;
        [SerializeField] private PlayerGunView view;

        [SerializeField] private GunModel gunModel;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            SubscribeEvents();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
                model.TryGetGun();

            if (Input.GetAxis("Mouse ScrollWheel") > 0)
                model.ChangeCurrentEquippedGun(1);
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                model.ChangeCurrentEquippedGun(-1);
        }

        private void SubscribeEvents()
        {
            model.OnEquipGun
                .Subscribe(gun =>
                {
                    gunModel.CurrentEquippedGun = gun;
                    view.ShowGun(gun);
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