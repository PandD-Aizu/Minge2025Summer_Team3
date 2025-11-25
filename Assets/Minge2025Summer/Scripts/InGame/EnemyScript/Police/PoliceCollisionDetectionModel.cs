using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EnemyScript
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
        [SerializeField, Tooltip("障害物のレイヤーマスク(Default, Wallなど。Playerは含めない)")]
        private LayerMask obstacleLayerMask;

        [SerializeField, Tooltip("検知対象(Player)のレイヤーマスク")] // ★追加
        private LayerMask targetLayerMask;
        
        private readonly Collider[] overlapBuffer = new Collider[32];

        // 最終目撃情報
        private bool hasLastKnownPosition;
        private Vector3 lastKnownPosition;
        public bool HasLastKnownPosition => hasLastKnownPosition;
        public Vector3 LastKnownPosition => lastKnownPosition;
        public Transform PlayerTransform { get; private set; }

        public void ClearLastKnownPosition() 
        { 
            hasLastKnownPosition = false; 
            PlayerTransform = null;
        }

        public void SetLastKnownPosition(Vector3 position)
        {
            lastKnownPosition = position;
            hasLastKnownPosition = true;
        }
        
        private Transform forcedPlayer;
        private float forcedUntilTime;

        public void ForceDetect(Transform player, float durationSeconds = 1.5f)
        {
            forcedPlayer = player;
            forcedUntilTime = Time.time + Mathf.Max(0.05f, durationSeconds);
            if (player != null)
            {
                PlayerTransform = player;
                hasLastKnownPosition = true;
                lastKnownPosition = player.position;
            }
        }

        public void FindPlayerInVision()
        {
            // 強制検知
            if (forcedPlayer != null)
            {
                if (Time.time <= forcedUntilTime && forcedPlayer.gameObject.activeInHierarchy)
                {
                    PlayerTransform = forcedPlayer;
                    hasLastKnownPosition = true;
                    lastKnownPosition = forcedPlayer.position;
                    return;
                }
                forcedPlayer = null;
            }

            PlayerTransform = null;
            if (policeTransform == null) return;

            // targetLayerMask を使ってプレイヤーだけをバッファに入れる
            int hitCount = Physics.OverlapSphereNonAlloc(
                policeTransform.position, 
                farDistance, 
                overlapBuffer,
                targetLayerMask
            );
            
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

            if (targetPlayer == null) return;

            // 視野角チェック
            Vector3 directionToPlayer = (targetPlayer.position - policeTransform.position).normalized;
            
            // XZ平面上の角度で判定
            Vector3 forwardFlat = Vector3.ProjectOnPlane(policeTransform.forward, Vector3.up).normalized;
            Vector3 dirFlat = Vector3.ProjectOnPlane(directionToPlayer, Vector3.up).normalized;

            if (Vector3.Angle(forwardFlat, dirFlat) < visionAngle / 2)
            {
                // 視線チェック
                float distanceToPlayer = Vector3.Distance(policeTransform.position, targetPlayer.position);
                Vector3 eyePosition = policeTransform.position + Vector3.up * 1.4f;
                
                // 相手の胸元あたりを狙う
                Vector3 targetCenter = targetPlayer.position + Vector3.up * 1.0f;
                Vector3 rayDirection = (targetCenter - eyePosition).normalized;

                if (!Physics.Raycast(eyePosition, rayDirection, distanceToPlayer, obstacleLayerMask))
                {
                    PlayerTransform = targetPlayer.transform;
                    hasLastKnownPosition = true;
                    lastKnownPosition = targetPlayer.position;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (policeTransform == null) return;
            
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(policeTransform.position, farDistance);
            
            float halfAngle = visionAngle / 2;
            Quaternion leftRayRotation = Quaternion.AngleAxis(-halfAngle, Vector3.up);
            Quaternion rightRayRotation = Quaternion.AngleAxis(halfAngle, Vector3.up);
            
            Vector3 leftRayDirection = leftRayRotation * policeTransform.forward;
            Vector3 rightRayDirection = rightRayRotation * policeTransform.forward;

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