using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EnemyScript.Boss
{
    public class BossCollisionDetectionModel : MonoBehaviour
    {
        [Header("依存関係")]
        [SerializeField, Tooltip("警官(ボス)のTransform")] 
        private Transform policeTransform;
        
        [Header("感知範囲の設定")] 
        [SerializeField, Tooltip("敵の視野角"), Range(0.0f, 360.0f)] 
        private float visionAngle = 90.0f;

        [SerializeField, Tooltip("1段階目の感知距離(近距離・デバッグ描画用)")]
        private float closeDistance = 30.0f;

        [SerializeField, Tooltip("2段階目の感知距離(遠距離・最大)")]
        private float farDistance = 100.0f;
        
        [Header("レイヤーマスク設定")]
        [SerializeField, Tooltip("障害物とみなすレイヤー（Wall, Defaultなど。Playerは含めない！）")]
        private LayerMask obstacleLayerMask;

        [SerializeField, Tooltip("検知対象(Player)のレイヤー（ここに指定したレイヤーのみを探索します）")]
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

        /// <summary>
        /// 最後に目撃した位置を設定する
        /// </summary>
        public void SetLastKnownPosition(Vector3 position)
        {
            lastKnownPosition = position;
            hasLastKnownPosition = true;
        }
        
        private Transform forcedPlayer;
        private float forcedUntilTime;

        /// <summary>
        /// 一定時間、可視判定をスキップしてプレイヤーを検知済みとして扱う
        /// </summary>
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

        /// <summary>
        /// 視界内のプレイヤーを検出する
        /// </summary>
        public void FindPlayerInVision()
        {
            // 強制検知のチェック
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

            // 周辺探索
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
                // 念のためタグもチェック（Playerレイヤーに他のものがないなら不要だが安全策）
                if (col != null && col.CompareTag("Player"))
                {
                    targetPlayer = col.transform;
                    break;
                }
            }

            // 範囲内にターゲットがいなければ終了
            if (targetPlayer == null) return;

            // 視野角判定
            Vector3 directionToPlayer = (targetPlayer.position - policeTransform.position).normalized;
            
            Vector3 forwardFlat = Vector3.ProjectOnPlane(policeTransform.forward, Vector3.up).normalized;
            Vector3 dirFlat = Vector3.ProjectOnPlane(directionToPlayer, Vector3.up).normalized;

            if (Vector3.Angle(forwardFlat, dirFlat) < visionAngle / 2)
            {
                // 視線判定
                float distanceToPlayer = Vector3.Distance(policeTransform.position, targetPlayer.position);
                Vector3 eyePosition = policeTransform.position + Vector3.up * 4.5f; 
                Vector3 targetCenter = targetPlayer.position + Vector3.up * 1.0f;
                Vector3 rayDirection = (targetCenter - eyePosition).normalized;

                // 障害物に遮られていなければ発見
                if (!Physics.Raycast(eyePosition, rayDirection, distanceToPlayer, obstacleLayerMask))
                {
                    PlayerTransform = targetPlayer.transform;
                    hasLastKnownPosition = true; 
                    lastKnownPosition = targetPlayer.position;
                }
            }
        }

        /// <summary>
        /// Editor上で視野範囲を可視化する
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (policeTransform == null) return;
            
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(policeTransform.position, farDistance);
            
            // ボスの体の向きに合わせてGizmoを回転させる
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