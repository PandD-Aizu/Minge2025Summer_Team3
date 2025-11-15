using System;
using Minge2025Summer.Scripts.InGame.ReiScript.GunScript.PureC_;
using Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Enum;
using UniRx;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Minge2025Summer.Scripts.InGame.ReiScript.GunScript.Weapon
{
    public class Pistol : MonoBehaviour, IWeapon
    {
        #region Serialized Fields
        [Header("共有ダメージモデル")] 
        [SerializeField] private WeaponDamageModel weaponDamageModel;
        
        [Header("弾丸関連")] 
        [SerializeField, Tooltip("弾丸の種類")] private AmmoType ammoType = AmmoType.Pistol;
        [SerializeField, Tooltip("弾倉に入る最大弾薬数")] private int magCapacity = 12;
        [SerializeField, Tooltip("弾倉内に入っている現在の弾薬数")] private int ammoInMag;

        [Header("銃の性能")] 
        [SerializeField, Tooltip("最大拡散角度")] private float maxSpreadAngle = 5.0f;
        [SerializeField, Tooltip("覗き込みまでの時間")] private float timeToAim = 0.3f;
        [SerializeField, Tooltip("射程")] private float range = 1000.0f;
        [SerializeField, Tooltip("発射レート(発/秒)")] private float fireRate = 0.5f;
        [SerializeField, Tooltip("攻撃力")] private float attackPower = 100.0f;
        [SerializeField, Tooltip("貫通可能な敵の数")] private int penetrationCount = 1;
        [SerializeField, Tooltip("近距離ボーナスが発生する最大距離")] private float pointBlankDistance = 3.0f;
        [SerializeField, Tooltip("近距離ボーナスのダメージ倍率")] private float pointBlankMultiplier = 1.5f;
        
        [Header("反動設定")]
        [SerializeField] private CinemachineImpulseSource impulseSource;
        
        [Header("銃の音が聞こえる距離")]
        [SerializeField] private float gunSoundHearingDistance = 50.0f;

        [Header("精度・反動回復")]
        [SerializeField, Tooltip("発砲時に追加される拡散(腰撃ち)")] private float fireSpreadKickHip = 3.0f;
        [SerializeField, Tooltip("発砲時に追加される拡散(構え)")] private float fireSpreadKickAim = 1.0f;
        [SerializeField, Tooltip("拡散の回復速度(度/秒)")] private float spreadRecoverSpeed = 10.0f;
        [SerializeField, Tooltip("拡散ペナルティの上限(度)")] private float maxSpreadPenalty = 10.0f;

        [Header("射撃カメラ/ヒットバッファ")]
        [SerializeField, Tooltip("レイキャストのレイヤーマスク")] 
        private LayerMask raycastLayerMask = ~0;
        [SerializeField, Tooltip("RaycastNonAlloc用のバッファサイズ")] 
        private int bufferSize = 32;
        [SerializeField, Tooltip("RaycastNonAlloc用のヒット配列サイズ")] private int raycastBufferSize = 32;
        private RaycastHit[] raycastBuffer;
        #endregion

        private Subject<Unit> onFire = new ();
        private float spreadPenalty;
        private float aimTimer;
        private float nextFireTime;
        
        public Subject<Unit> OnFire => onFire;
        public string GetWeaponName => "Pistol";
        public AmmoType GetAmmoType => ammoType;
        public int GetMagCapacity => magCapacity;
        public int GetAmmoInMag => ammoInMag;
        public float GetGunSoundVolume => gunSoundHearingDistance;
        public bool IsAiming { get; set; }

        public void UpdateWeapon(float deltaTime)
        {
            // 野底込みの進行をフレームで補完
            float targetTime = IsAiming ? timeToAim : 0.0f;
            aimTimer = Mathf.MoveTowards(aimTimer, targetTime, Time.deltaTime);
            
            // 発射ペナルティの回復
            if (spreadPenalty > 0.0f)
            {
                spreadPenalty = Mathf.Max(0.0f, spreadPenalty - spreadRecoverSpeed * deltaTime);
            }
        }

        public void Equip()
        {
            raycastBufferSize = Mathf.Max(8, bufferSize);
            raycastBuffer = new RaycastHit[raycastBufferSize];
        }

        public void Reload(int bulletsToReload)
        {
            ammoInMag += bulletsToReload;
        }

        public void Fire()
        {
            if (Time.time < nextFireTime)
                return;

            // 次の発砲可能時間を設定
            nextFireTime = Time.time + 1.0f / fireRate;
            
            // 発砲時に現在の状態に応じて拡散ペナルティを加算
            float aimT = GetCurrentAccuracy();
            float kick = Mathf.Lerp(fireSpreadKickHip, fireSpreadKickAim, aimT);
            spreadPenalty = Mathf.Min(maxSpreadPenalty, spreadPenalty + kick);
            
            // 向きと発射元を決定
            Vector3 shootDirection = GetShootDirection();
            Vector3 origin = Camera.main.transform.position;
            
            // レイキャストしてヒットを処理
            int hitCount = Physics.RaycastNonAlloc(origin, shootDirection, raycastBuffer, range, raycastLayerMask);
            if (hitCount > 0)
            {
                weaponDamageModel.ProcessHits(
                    raycastBuffer,
                    hitCount,
                    attackPower,
                    range,
                    pointBlankDistance,
                    pointBlankMultiplier,
                    penetrationCount
                );
            }

            ammoInMag--;
            OnFire.OnNext(Unit.Default);
            if (impulseSource != null)
                impulseSource.GenerateImpulse();
        }

        public void Aim()
        {
            IsAiming = true;
        }
        
        public void ResetAccuracy()
        {
            IsAiming = false;
        }

        private Vector3 GetShootDirection()
        {
            Vector3 forward = Camera.main.transform.forward;
            float finalSpread = GetFinalSpreadAngleDeg();
            Vector2 randomPointInCircle = Random.insideUnitCircle * finalSpread;
            Quaternion randomRotation = Quaternion.Euler(randomPointInCircle.y, randomPointInCircle.x, 0.0f);

            return randomRotation * forward;
        }

        public float GetCurrentAccuracy()
        {
            return Mathf.Clamp01(aimTimer / timeToAim);
        }

        public float GetCurrentSpreadAngleDeg()
        {
            return Mathf.Lerp(maxSpreadAngle, 0.0f, GetCurrentAccuracy());
        }

        public float GetFinalSpreadAngleDeg()
        {
            float penalty = spreadPenalty * (1.0f - GetCurrentAccuracy());
            return Mathf.Max(0.0f, GetCurrentSpreadAngleDeg() + penalty);
        }

        public float GetHipFireSpreadAngleDeg()
        {
            return maxSpreadAngle;
        }
    }
}