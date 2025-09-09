using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class EnemyCollisionDetectionModel : MonoBehaviour
    {
        [Header("警官のTransform")]
        [SerializeField] 
        private Transform policeTransform;
        
        [Header("感知範囲の設定")] 
        [SerializeField, Tooltip("敵の視野角"), Range(0.0f, 360.0f)] 
        private float visionAngle = 90.0f;

        [SerializeField, Tooltip("1段階目の感知距離(近距離)")]
        private float closeDistance = 30.0f;

        [SerializeField, Tooltip("2段階目の感知距離(遠距離・最大)")]
        private float farDistance = 100.0f;
        
        [Header("レイヤーマスク")]
        [SerializeField, Tooltip("障害物のレイヤーマスク")]
        private LayerMask obstacleLayerMask;

        public Transform PlayerTransform { get; set; }

        /// <summary>
        /// 視界内のプレイヤーを検出する
        /// </summary>
        public void FindPlayerInVision()
        {
            PlayerTransform = null;

            // 識別可能な範囲内のすべてのコライダーを取得(警官の足元から半径farDistanceの球体範囲)
            Collider[] collidersInRange = Physics.OverlapSphere(policeTransform.position, farDistance);

            Transform targetPlayer = null;

            // プレイヤーのコライダーを探す
            foreach (var collider in collidersInRange)
            {
                if (collider.CompareTag("Player"))
                {
                    targetPlayer = collider.transform;
                    break;
                }
            }

            // プレイヤーが見つからなかった場合は終了
            if (targetPlayer == null)
                return;

            // プレイヤーが視野内にいるかどうかを確認
            Vector3 directionToPlayer = (targetPlayer.position - policeTransform.position).normalized;
            if (Vector3.Angle(policeTransform.forward, directionToPlayer) < visionAngle / 2)
            {
                float distanceToPlayer = Vector3.Distance(policeTransform.position, targetPlayer.position);
                Vector3 eyePosition = policeTransform.position + Vector3.up * 1.6f; // 警官の目の位置を調整
                
                // 障害物がないかどうかを確認
                if (!Physics.Raycast(eyePosition, directionToPlayer, distanceToPlayer, obstacleLayerMask))
                {
                    if (distanceToPlayer <= closeDistance)
                        PlayerTransform = targetPlayer.transform;
                    else if (distanceToPlayer <= farDistance)
                        PlayerTransform = targetPlayer.transform;
                }
            }
        }

        /// <summary>
        /// Editor上で視野範囲を可視化する
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(policeTransform.position, farDistance);
            
            float halfAngle = visionAngle / 2;
            Quaternion leftRayRotation = Quaternion.AngleAxis(-halfAngle, Vector3.up);
            Quaternion rightRayRotation = Quaternion.AngleAxis(halfAngle, Vector3.up);
            
            Vector3 leftRayDirection = leftRayRotation * Vector3.forward;
            Vector3 rightRayDirection = rightRayRotation * Vector3.forward;

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(policeTransform.position, leftRayDirection * closeDistance);
            Gizmos.DrawRay(policeTransform.position, rightRayDirection * closeDistance);
            Gizmos.DrawLine(policeTransform.position + leftRayDirection * closeDistance, policeTransform.position + rightRayDirection * closeDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(policeTransform.position, leftRayDirection * farDistance);
            Gizmos.DrawRay(policeTransform.position, rightRayDirection * farDistance);
            Gizmos.DrawLine(policeTransform.position + leftRayDirection * farDistance, policeTransform.position + rightRayDirection * farDistance);
        }
    }
}