using UnityEngine;
using UnityEngine.AI;

namespace Workspace.koto_thing
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

        private Vector3 planarVelocity;
        private float verticalVelocity;

        public NavMeshAgent GetAgent => agent;
        public float GetSpeed => speed;
        
        /// <summary>
        /// 速度を更新する
        /// </summary>
        public void UpdatePlanarVelocity()
        {
            Vector3 desiredVelocity = agent != null ? agent.desiredVelocity : Vector3.zero;
            desiredVelocity.y = 0.0f;

            float effectiveSpeed = speed * CurrentSlowMultiplier; // スロー反映

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
            if (characterController == null) return;
            Vector3 displacement = (planarVelocity + Vector3.up * verticalVelocity) * Time.deltaTime;
            characterController.Move(displacement);
            if (agent != null) agent.nextPosition = agent.transform.position;
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
        
        /* 以下ヘルパー関数 */
        
        /// <summary>
        /// 行き先を設定する
        /// </summary>
        /// <param name="worldPosition">ワールド座標</param>
        public void SetDestination(Vector3 worldPosition)
        {
            if (agent != null && agent.isOnNavMesh)
            {
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
        /// 攻撃中など、直ちに完全停止させたい場合に使用。Planar速度を即ゼロ化しパスも破棄。
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

        /// <summary>現在の実効速度倍率(スロー解除時間を過ぎれば1に戻る)</summary>
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
        /// 脚被弾によるスローを適用
        /// </summary>
        /// <param name="multiplier">0~1 の速度倍率</param>
        /// <param name="duration">継続時間(秒)</param>
        /// <param name="refreshIfEqual">同一強度再適用で残り時間をリフレッシュするか</param>
        public void ApplyLegSlow(float multiplier, float duration, bool refreshIfEqual = true)
        {
            multiplier = Mathf.Clamp(multiplier, 0.05f, 1f);
            // 既により強い(=小さい)スロー中なら無視。
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
    }
}