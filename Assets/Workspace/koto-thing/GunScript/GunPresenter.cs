using System;
using Cysharp.Threading.Tasks;
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
        [SerializeField, Tooltip("射撃レイを飛ばす参照カメラ。未設定ならCamera.main")] private Camera fireCamera;

        [Header("聴覚(銃声)設定")] 
        [SerializeField, Tooltip("敵が銃声を聞き取れる半径(メートル)")] private float gunshotHearRadius = 35f;

        private CompositeDisposable disposables = new ();
        private readonly SerialDisposable gunFireSubscription = new();
        private IGun lastGun;

        private void Start()
        {
            SubscribeEvents();
        }

        private void Update()
        {
            var gun = model.CurrentEquippedGun;
            if (gun == null)
                return;

            // 銃が切り替わったら OnFire 購読を差し替え
            if (gun != lastGun)
            {
                lastGun = gun;
                gunFireSubscription.Disposable = gun.OnFire.Subscribe(_ => OnGunFired());
            }

            // 毎フレーム更新（精度回復など）
            gun.Tick(Time.deltaTime);
            
            if (Input.GetKeyDown(KeyCode.R) && model.GetCurrentMagCapacity() != model.GetCurrentAmmoInMag())
                model.PreReload();
            
            if (Input.GetMouseButtonDown(1))
                emitter.PlayAimSound();

            if (Input.GetMouseButton(1))
                gun.Aim();
            else
                gun.ResetAccuracy();

            if (Input.GetMouseButtonDown(0))
            {
                if (model.GetCurrentAmmoInMag() > 0)
                    gun.Fire();
                else
                    emitter.PlayEmptyFireSound();
            }
            
            model.CheckReload();
            view.UpdateAmmoText(model.GetCurrentAmmoInMag(), model.GetCurrentAmmo(), model.GetCurrentMagCapacity());
            view.UpdateReticle(gun, Input.GetMouseButton(1));
        }

        private void OnGunFired()
        {
            view.PlayMuzzleFlash();
            emitter.PlayFireSound();
            view.PlayMuzzleFlashLight().Forget();

            var cam = fireCamera != null ? fireCamera : Camera.main;
            Vector3 pos = cam != null ? cam.transform.position : view.transform.position;
            MessageBroker.Default.Publish(new SoundEvent(pos, gunshotHearRadius, SoundType.Gunshot, gameObject));
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
            gunFireSubscription.Dispose();
            disposables.Dispose();
        }
    }
}