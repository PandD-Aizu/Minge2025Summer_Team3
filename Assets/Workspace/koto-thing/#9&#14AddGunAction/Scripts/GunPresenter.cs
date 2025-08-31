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
            if (Input.GetKeyUp(KeyCode.R) && model.GetCurrentMagCapacity() != model.GetCurrentAmmoInMag())
            {
                model.PreReload();
            }
            
            if (Input.GetMouseButtonDown(1))
            {
                emitter.PlayAimSound();
            }

            if (Input.GetMouseButton(1))
            {
                if (Input.GetMouseButtonDown(0) && model.GetCurrentAmmoInMag() > 0)
                {
                    model.GetCurrentEquippedGun.Fire();
                    emitter.PlayFireSound();
                }
                else if (Input.GetMouseButtonDown(0) && model.GetCurrentAmmoInMag() == 0)
                {
                    emitter.PlayEmptyFireSound();   
                }
            }
            
            view.UpdateAmmoText(model.GetCurrentAmmoInMag(), model.GetCurrentAmmo(), model.GetCurrentMagCapacity());
        }

        private void SubscribeEvents()
        {
            // リロード処理
            model.OnReload
                .SelectMany(isEmptyReload => emitter.PlayReloadAndWait(isEmptyReload))
                .Subscribe(_ => model.Reload())
                .AddTo(disposables);
        }

        public void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            
        }
    }
}