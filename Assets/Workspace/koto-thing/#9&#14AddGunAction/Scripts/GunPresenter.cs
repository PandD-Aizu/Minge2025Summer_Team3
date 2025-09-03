using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class GunPresenter : MonoBehaviour, IDisposable
    {
        [Header("依存関係")] 
        [SerializeField] private GunModel model;
        [SerializeField] private GunView view;
        [SerializeField] private GunEmitter emitter;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            SubscribeEvents();
        }

        private void Update()
        {
            if (model.GetCurrentEquippedGun == null)
                return;
            
            if (Input.GetKeyDown(KeyCode.R) && model.GetCurrentMagCapacity() != model.GetCurrentAmmoInMag())
                model.PreReload();
            
            if (Input.GetMouseButtonDown(1))
                emitter.PlayAimSound();

            if (Input.GetMouseButton(1))
                model.GetCurrentEquippedGun.Aim();
            else
                model.GetCurrentEquippedGun.ResetAccuracy();

            if (Input.GetMouseButtonDown(0))
            {
                if (model.GetCurrentAmmoInMag() > 0)
                    model.GetCurrentEquippedGun.Fire();
                else
                    emitter.PlayEmptyFireSound();
            }
            
            model.CheckReload();
            view.UpdateAmmoText(model.GetCurrentAmmoInMag(), model.GetCurrentAmmo(), model.GetCurrentMagCapacity());
            view.UpdateReticle(model.GetCurrentEquippedGun, Input.GetMouseButton(1));
        }

        private void SubscribeEvents()
        {
            // リロード処理
            model.OnReload
                .SelectMany(isEmptyReload => emitter.PlayReloadAndWait(isEmptyReload))
                .Subscribe(_ => model.Reload())
                .AddTo(disposables);

            model.GetCurrentEquippedGun.OnFire
                .Subscribe(_ =>
                {
                    view.PlayMuzzleFlash();
                    emitter.PlayFireSound();
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