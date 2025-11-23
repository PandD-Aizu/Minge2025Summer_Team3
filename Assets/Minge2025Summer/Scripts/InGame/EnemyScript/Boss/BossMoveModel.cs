using UnityEngine;
using UnityEngine.AI;

namespace Minge2025Summer.Scripts.InGame.EnemyScript.Boss
{
    public class BossMoveModel : MonoBehaviour
    {
        [Header("依存関係")]
        [SerializeField] private Transform policeTransform;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Animator animator;
        [SerializeField] private NavMeshAgent agent;
        [Header("パトロール関連 (外部 BossPatrolModel を使用)")]
        [SerializeField] private BossPatrolModel patrolModel;

        [Header("ボスの速度関係")]
        [SerializeField] private float speed = 3.5f;
        [SerializeField] private float speedChangeRate = 10.0f;
        [SerializeField] private float rotationSpeed = 10.0f;
        
        // ... (スロー効果、ブースト関連の変数はそのまま維持) ...
        [Header("スロー効果(脚被弾)")]
        [SerializeField] private float currentSlowMultiplier = 1f;
        private float slowExpireTime;
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
        public float GetSpeed => speed;

        private void Start()
        {
            nextBoostTimer = Random.Range(minBoostInterval, maxBoostInterval);
            if (animator != null) baseAnimatorSpeed = animator.speed;
            else baseAnimatorSpeed = 1f;
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
            
            UpdateAnimatorSpeed();
        }

        /// <summary>
        /// パトロールが可能か（エリアサイズが設定されているか）
        /// </summary>
        public bool CanPatrol()
        {
            // BossPatrolModel側で AreaSize > 0 なら true を返す想定
            return patrolModel != null && patrolModel.HasPatrolConfigured();
        }

        /// <summary>
        /// 次のランダムなパトロール地点へ移動する
        /// </summary>
        public void GoToNextPatrolPoint()
        {
            var navAgent = GetAgent;
            if (patrolModel != null && navAgent != null)
                patrolModel.GoToNextPoint(navAgent); // ここでランダム座標が計算される
        }

        /// <summary>
        /// パトロール（ランダム巡回）を開始する
        /// </summary>
        public void StartPatrol()
        {
            var navAgent = GetAgent;
            if (patrolModel != null && navAgent != null && navAgent.isOnNavMesh)
                patrolModel.StartPatrol(navAgent);
        }

        /// <summary>
        /// パトロールを停止する
        /// </summary>
        public void StopPatrol()
        {
            if (patrolModel != null)
                patrolModel.StopPatrol();
        }

        /// <summary>
        /// 現在の目的地（ランダム地点）に到着したかどうか
        /// </summary>
        public bool IsAtPatrolDestination()
        {
            return patrolModel != null && patrolModel.IsAtDestination(GetAgent);
        }

        public void ApplyGravity()
        {
            if (characterController != null && !characterController.isGrounded)
                verticalVelocity += Physics.gravity.y * Time.deltaTime;
            else
                verticalVelocity = -1.0f;
        }

        public void ApplyMovement()
        {
            if (characterController == null) return;
            Vector3 displacement = (planarVelocity + Vector3.up * verticalVelocity) * Time.deltaTime;
            characterController.Move(displacement);
            if (agent != null) agent.nextPosition = agent.transform.position;
        }

        private void OnAnimatorMove()
        {
            if (animator != null && animator.applyRootMotion) ApplyMovement();
        }
        
        public float CurrentSlowMultiplier
        {
            get
            {
                if (currentSlowMultiplier < 1f && Time.time >= slowExpireTime) currentSlowMultiplier = 1f;
                return currentSlowMultiplier;
            }
        }

        public float CurrentSpeedMultiplier
        {
            get
            {
                if (currentSpeedMultiplier > 1f && Time.time >= speedBoostExpireTime) currentSpeedMultiplier = 1f;
                return currentSpeedMultiplier;
            }
        }

        public void ApplyLegSlow(float multiplier, float duration, bool refreshIfEqual = true)
        {
            multiplier = Mathf.Clamp(multiplier, 0.05f, 1f);
            if (multiplier < currentSlowMultiplier) { currentSlowMultiplier = multiplier; slowExpireTime = Time.time + duration; }
            else if (refreshIfEqual && Mathf.Approximately(multiplier, currentSlowMultiplier)) { slowExpireTime = Time.time + duration; }
            else if (currentSlowMultiplier >= 1f) { currentSlowMultiplier = multiplier; slowExpireTime = Time.time + duration; }
        }

        public void ApplySpeedBoost(float multiplier, float duration, bool refreshIfEqual = true)
        {
            multiplier = Mathf.Max(1f, multiplier);
            if (multiplier > currentSpeedMultiplier) { currentSpeedMultiplier = multiplier; speedBoostExpireTime = Time.time + duration; }
            else if (refreshIfEqual && Mathf.Approximately(multiplier, currentSpeedMultiplier)) { speedBoostExpireTime = Time.time + duration; }
        }

        public void TriggerSpeedDouble(float duration) => ApplySpeedBoost(2f, duration);

        private void UpdateAnimatorSpeed()
        {
            if (animator == null) return;
            float effectiveMultiplier = CurrentSpeedMultiplier * CurrentSlowMultiplier;
            effectiveMultiplier = Mathf.Max(0.01f, effectiveMultiplier);
            animator.speed = baseAnimatorSpeed * effectiveMultiplier;
        }

        public void SetDestination(Vector3 worldPosition)
        {
            if (agent != null && agent.isOnNavMesh) agent.SetDestination(worldPosition);
        }

        public void StopMovement()
        {
            if (agent != null && agent.isOnNavMesh) agent.SetDestination(policeTransform.position);
        }

        public void ForceStopImmediate()
        {
            planarVelocity = Vector3.zero;
            if (agent != null && agent.isOnNavMesh) { agent.ResetPath(); agent.velocity = Vector3.zero; }
        }

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

        public void UpdateRotation()
        {
            Vector3 lookDirection = planarVelocity;
            lookDirection.y = 0.0f;
            if (lookDirection.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                if (policeTransform != null)
                    policeTransform.rotation = Quaternion.Slerp(policeTransform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
}