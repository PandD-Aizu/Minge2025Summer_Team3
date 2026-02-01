using UnityEngine;
using UnityEngine.AI;

namespace Minge2025Summer.Scripts.InGame.EnemyScript
{
    public class PoliceMoveModel : MonoBehaviour
    {
        [Header("依存関係")]
        [SerializeField] private Transform policeTransform;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Animator animator;
        [SerializeField] private NavMeshAgent agent;

        [Header("警官の速度関係")]
        [SerializeField] private float speed = 3.5f;
        [SerializeField] private float speedChangeRate = 10.0f;
        [SerializeField] private float rotationSpeed = 10.0f;

        [Header("スロー効果(脚被弾)")]
        [SerializeField, Tooltip("現在適用中の移動速度倍率(1=通常)")] private float currentSlowMultiplier = 1f;
        private float slowExpireTime;

        [Header("一時速度ブースト設定")]
        [SerializeField] private bool enableOccasionalSpeedBoost = true;
        [SerializeField] private float minBoostInterval = 5f;
        [SerializeField] private float maxBoostInterval = 20f;
        [SerializeField] private float boostDuration = 3f;
        [SerializeField] private float boostMultiplier = 2f;

        private float currentSpeedMultiplier = 1f;
        private float speedBoostExpireTime;
        private float nextBoostTimer;

        private Vector3 planarVelocity;
        private float verticalVelocity;
        
        private float baseAnimatorSpeed = 1f;

        public NavMeshAgent GetAgent => agent;
        public float GetSpeed => planarVelocity.magnitude;

        private void Start()
        {
            nextBoostTimer = Random.Range(minBoostInterval, maxBoostInterval);
            if (animator != null)
            {
                baseAnimatorSpeed = animator.speed;
            }
            else
            {
                baseAnimatorSpeed = 1f;
            }
        }

        private void Update()
        {
            if (enableOccasionalSpeedBoost)
            {
                nextBoostTimer -= Time.deltaTime;
                if (nextBoostTimer <= 0f)
                {
                    ApplySpeedBoost(boostMultiplier, boostDuration);
                    nextBoostTimer = Random.Range(minBoostInterval, maxBoostInterval);
                }
            }

            // アニメーション速度を現在の実効倍率に合わせて更新
            UpdateAnimatorSpeed();
        }

        /// <summary>
        /// 速度を更新する
        /// </summary>
        public void UpdatePlanarVelocity()
        {
            Vector3 desiredVelocity = agent != null ? agent.desiredVelocity : Vector3.zero;
            desiredVelocity.y = 0.0f;

            float effectiveSpeed = speed * CurrentSlowMultiplier * CurrentSpeedMultiplier;

            Vector3 targetPlanarVelocity = desiredVelocity.sqrMagnitude > 0.01f
                ? Vector3.ClampMagnitude(desiredVelocity.normalized * effectiveSpeed, effectiveSpeed)
                : Vector3.zero;

            planarVelocity = Vector3.MoveTowards(planarVelocity, targetPlanarVelocity, Time.deltaTime * speedChangeRate);
        }

        /// <summary>
        /// 回転を更新する
        /// </summary>
        public void UpdateRotation()
        {
            Vector3 lookDirection = planarVelocity;
            lookDirection.y = 0.0f;

            if (lookDirection.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                policeTransform.rotation = Quaternion.Slerp(policeTransform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }

        /// <summary>
        /// 重力を適用する
        /// </summary>
        public void ApplyGravity()
        {
            if (characterController != null && !characterController.isGrounded)
                verticalVelocity += Physics.gravity.y * Time.deltaTime;
            else
                verticalVelocity = -1.0f;
        }

        /// <summary>
        /// 移動を適用する（CharacterControllerを使用）
        /// </summary>
        public void ApplyMovement()
        {
            if (characterController == null)
                return;

            Vector3 displacement = (planarVelocity + Vector3.up * verticalVelocity) * Time.deltaTime;
            characterController.Move(displacement);

            if (agent != null)
                agent.nextPosition = agent.transform.position;
        }

        /// <summary>
        /// アニメーターの動きに上書きして移動を適用する（root motion使用時のみ）
        /// </summary>
        private void OnAnimatorMove()
        {
            if (animator != null && animator.applyRootMotion)
            {
                ApplyMovement();
            }
        }
        
        /// <summary>
        /// 現在の実効速度倍率(スロー解除時間を過ぎれば1に戻る)
        /// </summary>
        public float CurrentSlowMultiplier
        {
            get
            {
                if (currentSlowMultiplier < 1f && Time.time >= slowExpireTime)
                {
                    currentSlowMultiplier = 1f;
                }
                return currentSlowMultiplier;
            }
        }

        /// <summary>
        /// 現在のスピードブースト倍率(1=通常)
        /// </summary>
        public float CurrentSpeedMultiplier
        {
            get
            {
                if (currentSpeedMultiplier > 1f && Time.time >= speedBoostExpireTime)
                {
                    currentSpeedMultiplier = 1f;
                }
                return currentSpeedMultiplier;
            }
        }

        /// <summary>
        /// 脚被弾によるスローを適用
        /// </summary>
        public void ApplyLegSlow(float multiplier, float duration, bool refreshIfEqual = true)
        {
            multiplier = Mathf.Clamp(multiplier, 0.05f, 1f);
            if (multiplier < currentSlowMultiplier)
            {
                currentSlowMultiplier = multiplier;
                slowExpireTime = Time.time + duration;
            }
            else if (refreshIfEqual && Mathf.Approximately(multiplier, currentSlowMultiplier))
            {
                slowExpireTime = Time.time + duration;
            }
            else if (currentSlowMultiplier >= 1f)
            {
                currentSlowMultiplier = multiplier;
                slowExpireTime = Time.time + duration;
            }
        }

        /// <summary>
        /// 一時的な速度ブーストを適用（内部的に multiplier >= 1 を想定）
        /// </summary>
        public void ApplySpeedBoost(float multiplier, float duration, bool refreshIfEqual = true)
        {
            multiplier = Mathf.Max(1f, multiplier);
            if (multiplier > currentSpeedMultiplier)
            {
                currentSpeedMultiplier = multiplier;
                speedBoostExpireTime = Time.time + duration;
            }
            else if (refreshIfEqual && Mathf.Approximately(multiplier, currentSpeedMultiplier))
            {
                speedBoostExpireTime = Time.time + duration;
            }
        }

        /// <summary>
        /// 指定秒だけ速度を2倍にする
        /// </summary>
        public void TriggerSpeedDouble(float duration)
        {
            ApplySpeedBoost(2f, duration);
        }

        /* アニメーター速度更新 */
        private void UpdateAnimatorSpeed()
        {
            if (animator == null) return;

            float effectiveMultiplier = CurrentSpeedMultiplier * CurrentSlowMultiplier;
            // 最低値を確保してゼロや極小値による問題を防ぐ
            effectiveMultiplier = Mathf.Max(0.01f, effectiveMultiplier);

            animator.speed = baseAnimatorSpeed * effectiveMultiplier;
        }

        /* 以下ヘルパー関数 */

        /// <summary>
        /// 行き先を設定する
        /// </summary>
        /// <param name="worldPosition">ワールド座標</param>
        public void SetDestination(Vector3 worldPosition)
        {
            if (agent != null && agent.isOnNavMesh)
            {
                if (agent.hasPath && Vector3.Distance(agent.destination, worldPosition) < 0.5f)
                    return;
                
                if (NavMesh.SamplePosition(worldPosition, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
                    agent.SetDestination(hit.position);
                else
                    agent.SetDestination(worldPosition);
            }
        }

        /// <summary>
        /// 行き先を警官の位置に設定して停止させる
        /// </summary>
        public void StopMovement()
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.SetDestination(policeTransform.position);
            }
        }

        /// <summary>
        /// 攻撃中など、直ちに完全停止させたい場合に使用。
        /// Planar速度を即ゼロ化しパスも破棄。
        /// </summary>
        public void ForceStopImmediate()
        {
            planarVelocity = Vector3.zero;
            if (agent != null && agent.isOnNavMesh)
            {
                agent.ResetPath();
                agent.velocity = Vector3.zero;
            }
        }
        
    }
}
