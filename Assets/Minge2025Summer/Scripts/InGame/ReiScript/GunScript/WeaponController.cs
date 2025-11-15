using System;
using Cysharp.Threading.Tasks;
using Minge2025Summer.Scripts.GameSetting;
using Minge2025Summer.Scripts.InGame.AcousticsScript;
using Minge2025Summer.Scripts.InGame.GunScript;
using Minge2025Summer.Scripts.InGame.ReiScript.ItemScript;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript.GunScript
{
    public class WeaponController : MonoBehaviour, IDisposable
    {
        [SerializeField] private WeaponModel model;
        [SerializeField] private WeaponView view;
        [SerializeField] private GunEmitter emitter;
        [SerializeField] private ReiItemInventoryModel inventoryModel;

        private CompositeDisposable disposables = new ();
        private readonly SerialDisposable weaponFireSubscription = new ();
        private IWeapon lastEquippedWeapon;

        private void Start()
        {
            SubscribeEvents();
        }

        private void Update()
        {
            var gameSettings = GameController.Instance.gameSettingsController.gameSettings;
            var currentEquippedWeapon = model.CurrentEquippedWeapon;
            if (currentEquippedWeapon == null)
            {
                Debug.LogError("CurrentEquippedWeapon is null");
                return;
            }

            if (currentEquippedWeapon != lastEquippedWeapon)
            {
                lastEquippedWeapon = currentEquippedWeapon;
                weaponFireSubscription.Disposable = currentEquippedWeapon.OnFire
                    .Subscribe(_ =>
                    {
                        view.PlayMuzzleFlash();
                        emitter.PlayFireSound();
                        view.PlayMuzzleFlashLight().Forget();
                        
                        Vector3 pos = Camera.main != null ? Camera.main.transform.position : view.transform.position;
                        MessageBroker.Default.Publish(new SoundEvent(pos, currentEquippedWeapon.GetGunSoundVolume, SoundType.Gunshot, gameObject));
                    });
            }
            
            currentEquippedWeapon.UpdateWeapon(Time.deltaTime);
            
            if (Input.GetKeyDown(gameSettings.keyReload) && model.CurrentEquippedWeapon.GetAmmoInMag < model.CurrentEquippedWeapon.GetMagCapacity)
                model.PreReloadAction(inventoryModel);
            
            if (Input.GetMouseButtonDown(1))
                emitter.PlayAimSound();
            
            if (Input.GetMouseButton(1))
                currentEquippedWeapon.Aim();
            else
                currentEquippedWeapon.ResetAccuracy();
            
            if (Input.GetMouseButtonDown(0))
                if (model.CurrentEquippedWeapon.GetAmmoInMag > 0)
                    currentEquippedWeapon.Fire();
                else
                    emitter.PlayEmptyFireSound();
        }

        private void SubscribeEvents()
        {
            model.OnReload
                .SelectMany(isEmptyReload => emitter.PlayReloadAndWait(isEmptyReload))
                .Subscribe(_ => model.Reload(inventoryModel))
                .AddTo(disposables);
        }
        
        private void OnDestroy()
        {
            Dispose();
        }
        
        public void Dispose()
        {
            weaponFireSubscription.Dispose();
            disposables.Dispose();
        }
    }
}