using System.Collections.Generic;
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

        [Header("精度・反動回復")]
        [SerializeField, Tooltip("発砲時に追加される拡散(腰撃ち)")] private float fireSpreadKickHip = 3.0f;
        [SerializeField, Tooltip("発砲時に追加される拡散(構え)")] private float fireSpreadKickAim = 1.0f;
        [SerializeField, Tooltip("拡散の回復速度(度/秒)")] private float spreadRecoverSpeed = 10.0f;
        [SerializeField, Tooltip("拡散ペナルティの上限(度)")] private float maxSpreadPenalty = 10.0f;

        [Header("射撃カメラ/ヒットバッファ")]
        [SerializeField, Tooltip("RaycastNonAlloc用のバッファサイズ")] 
        private int bufferSize = 32;
        [SerializeField, Tooltip("RaycastNonAlloc用のヒット配列サイズ")] private int raycastBufferSize = 32;
        private RaycastHit[] raycastBuffer;

        private float spreadPenalty;
        
        public Subject<Unit> OnFire { get; } = new ();

        private float aimTimer;
        private float nextFireTime;

        // Presenterから毎フレーム呼び出す
        public void Tick(float deltaTime)
        {
            if (spreadPenalty > 0f)
                spreadPenalty = Mathf.Max(0f, spreadPenalty - spreadRecoverSpeed * deltaTime);
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

            nextFireTime = Time.time + 1.0f / fireRate;

            // 発砲時に現在の状態(腰撃ち〜構え)に応じて拡散ペナルティを加算
            float aimT = CurrentAccuracy();
            float kick = Mathf.Lerp(fireSpreadKickHip, fireSpreadKickAim, aimT);
            spreadPenalty = Mathf.Min(maxSpreadPenalty, spreadPenalty + kick);
            
            Vector3 shootDirection = GetShootDirection();
            Vector3 origin = Camera.main.transform.position;

            int hitCount = Physics.RaycastNonAlloc(origin, shootDirection, raycastBuffer, range);
            if (hitCount > 0)
            {
                // 近距離順に単純選択法で並べ替え（低コスト・小配列前提）
                for (int i = 0; i < hitCount - 1; i++)
                {
                    int minIndex = i;
                    float minDist = raycastBuffer[minIndex].distance;
                    for (int j = i + 1; j < hitCount; j++)
                    {
                        float d = raycastBuffer[j].distance;
                        if (d < minDist)
                        {
                            minDist = d;
                            minIndex = j;
                        }
                    }
                    if (minIndex != i)
                    {
                        var tmp = raycastBuffer[i];
                        raycastBuffer[i] = raycastBuffer[minIndex];
                        raycastBuffer[minIndex] = tmp;
                    }
                }

                int penetratedEnemyCount = 0;
                HashSet<IEnemyStatus> damagedEnemies = new HashSet<IEnemyStatus>();

                for (int i = 0; i < hitCount; i++)
                {
                    if (penetratedEnemyCount >= penetrationCount)
                        break;

                    var hit = raycastBuffer[i];
                    var col = hit.collider;
                    if (col != null && col.CompareTag("EnemyShootable"))
                    {
                        IEnemyStatus enemyStatus = col.GetComponentInChildren<IEnemyStatus>();
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
            
            Debug.DrawRay(origin, shootDirection.normalized * range, Color.red, 2.0f);
            
            ammoInMag--;
            if (impulseSource != null) impulseSource.GenerateImpulse();
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
            float finalSpread = GetFinalSpreadAngleDeg();
            Vector2 randomPointInCircle = Random.insideUnitCircle * finalSpread;
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

        public float GetFinalSpreadAngleDeg()
        {
            return GetCurrentSpreadAngleDeg() + spreadPenalty;
        }

        public float GetHipFireSpreadAngleDeg()
        {
            return maxSpreadAngle;
        }
    }
}