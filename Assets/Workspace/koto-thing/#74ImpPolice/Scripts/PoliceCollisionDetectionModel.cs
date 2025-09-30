using UnityEngine;

namespace Workspace.koto_thing
{
    public class PoliceCollisionDetectionModel : MonoBehaviour
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
        
        private Collider[] overlapBuffer = new Collider[32];

        // 最終目撃情報
        private bool hasLastKnownPosition;
        private Vector3 lastKnownPosition;
        public bool HasLastKnownPosition => hasLastKnownPosition;
        public Vector3 LastKnownPosition => lastKnownPosition;
        public void ClearLastKnownPosition() { hasLastKnownPosition = false; }
        public Transform PlayerTransform { get; set; }

        /// <summary>
        /// 最後に目撃した位置を設定する
        /// </summary>
        /// <param name="position">最後に目撃した位置</param>
        public void SetLastKnownPosition(Vector3 position)
        {
            lastKnownPosition = position;
            hasLastKnownPosition = true;
        }

        /// <summary>
        /// 視界内のプレイヤーを検出する
        /// </summary>
        public void FindPlayerInVision()
        {
            PlayerTransform = null;
            if (policeTransform == null) 
                return;

            int hitCount = Physics.OverlapSphereNonAlloc(policeTransform.position, farDistance, overlapBuffer);
            Transform targetPlayer = null;
            for (int i = 0; i < hitCount; i++)
            {
                var col = overlapBuffer[i];
                if (col != null && col.CompareTag("Player"))
                {
                    targetPlayer = col.transform;
                    break;
                }
            }

            if (targetPlayer == null) 
                return;

            Vector3 directionToPlayer = (targetPlayer.position - policeTransform.position).normalized;
            if (Vector3.Angle(policeTransform.forward, directionToPlayer) < visionAngle / 2)
            {
                float distanceToPlayer = Vector3.Distance(policeTransform.position, targetPlayer.position);
                Vector3 eyePosition = policeTransform.position + Vector3.up * 1.6f;
                if (!Physics.Raycast(eyePosition, directionToPlayer, distanceToPlayer, obstacleLayerMask))
                {
                    PlayerTransform = targetPlayer.transform;
                    hasLastKnownPosition = true; // 最終目撃地点を更新
                    lastKnownPosition = targetPlayer.position;
                }
            }
        }

        /// <summary>
        /// Editor上で視野範囲を可視化する
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (policeTransform == null) 
                return;
            
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