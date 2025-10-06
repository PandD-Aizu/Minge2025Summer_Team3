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

            Vector3 targetPlanarVelocity = desiredVelocity.sqrMagnitude > 0.01f
                ? Vector3.ClampMagnitude(desiredVelocity.normalized * speed, speed)
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
    }
}