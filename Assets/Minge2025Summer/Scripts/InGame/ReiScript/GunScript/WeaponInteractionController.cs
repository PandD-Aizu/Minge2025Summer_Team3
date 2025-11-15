using System;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript.GunScript
{
    public class WeaponInteractionController : MonoBehaviour, IDisposable
    {
        [SerializeField] private WeaponInteractionModel model;
        [SerializeField] private WeaponInteractionView view;
        [SerializeField] private WeaponModel weaponModel;
        [SerializeField] private WeaponView weaponView;
        
        private CompositeDisposable disposables = new ();

        private void Start()
        {
            SubscribeEvents();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                model?.Interact();
            }
        }

        private void SubscribeEvents()
        {
            model.OnWeaponGet
                .Subscribe(weapon =>
                {
                    if (weapon == null)
                        return;

                    if (weaponModel != null)
                    {
                        weaponModel.CurrentEquippedWeapon = weapon;
                    }

                    // weapon の GameObject からライトを探して View にセット
                    var weaponGo = (weapon as Component)?.gameObject;
                    if (weaponGo != null && weaponView != null)
                    {
                        var muzzleLight = weaponGo.GetComponentInChildren<Light>(true);
                        if (muzzleLight != null)
                        {
                            weaponView.MuzzleFlashLight = muzzleLight;
                        }
                    }

                    view?.ShowSystemText((weapon.GetWeaponName ?? "武器") + "を取得した");
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