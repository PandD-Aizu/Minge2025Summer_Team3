using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.Cinemachine;
using UnityEngine;
using Workspace.momiji1107;
using Random = UnityEngine.Random;

namespace Workspace.koto_thing
{
    public class Pistol : MonoBehaviour, IGun
    {
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
        
        public Subject<Unit> OnFire { get; } = new ();

        private float aimTimer;
        private float nextFireTime;

        public void Equip()
        {
            // 装備時の処理
        }

        public void Reload(int bulletsToReload)
        {
            ammoInMag += bulletsToReload;
        }
        
        public void Fire()
        {
            if (Time.time < nextFireTime)
                return;

            nextFireTime = Time.time + 1.0f / fireRate;
            
            Vector3 shootDirection = GetShootDirection();
            RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position, shootDirection, range);
            if (hits.Length > 0)
            {
                hits = hits.OrderBy(hit => hit.distance).ToArray();

                int penetratedEnemyCount = 0;
                HashSet<IEnemyStatus> damagedEnemies = new HashSet<IEnemyStatus>();

                foreach (RaycastHit hit in hits)
                {
                    // 貫通可能な敵の数に達したら終了
                    if (penetratedEnemyCount >= penetrationCount)
                        break;

                    // 敵にヒットした場合
                    if (hit.collider.CompareTag("EnemyShootable"))
                    {
                        IEnemyStatus enemyStatus = hit.collider.GetComponentInChildren<IEnemyStatus>();

                        // まだダメージを与えていない敵にのみダメージを与える
                        if (enemyStatus != null && !damagedEnemies.Contains(enemyStatus))
                        {
                            float finalDamage = attackPower;
                            if (hit.distance <= pointBlankDistance)
                                finalDamage *= pointBlankMultiplier;
                            
                            enemyStatus.ReceiveDamage(finalDamage);
                            damagedEnemies.Add(enemyStatus);
                            penetratedEnemyCount++;
                        }
                    }
                }
            }
            
            Debug.DrawRay(Camera.main.transform.position, shootDirection.normalized * range, Color.red, 2.0f);
            
            ammoInMag--;
            impulseSource.GenerateImpulse();
            OnFire.OnNext(Unit.Default);
        }
        
        public void Aim()
        {
            aimTimer += Time.deltaTime;
        }

        public void ResetAccuracy()
        {
            aimTimer = 0f;
        }
        
        /* 以下ヘルパー関数 */

        private Vector3 GetShootDirection()
        {
            Vector3 forward = Camera.main.transform.forward;
            float currentSpread = GetCurrentSpreadAngleDeg();
            Vector2 randomPointInCircle = Random.insideUnitCircle * currentSpread;
            
            Quaternion randomRotation = Quaternion.Euler(randomPointInCircle.y, randomPointInCircle.x, 0.0f);
            
            return randomRotation * forward;
        }
        
        public AmmoType GetAmmoType() => ammoType;
        public int GetMagCapacity() => magCapacity;
        public int GetAmmoInMag() => ammoInMag;

        public float CurrentAccuracy()
        {
            return Mathf.Clamp01(aimTimer / timeToAim);
        }

        public float GetCurrentSpreadAngleDeg()
        {
            return Mathf.Lerp(maxSpreadAngle, 0.0f, CurrentAccuracy());
        }

        public float GetHipFireSpreadAngleDeg()
        {
            return maxSpreadAngle;
        }
    }
}