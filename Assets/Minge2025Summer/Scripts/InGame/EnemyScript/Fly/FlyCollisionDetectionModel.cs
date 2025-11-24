using Minge2025Summer.Scripts.InGame.FlashLightScript;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EnemyScript.Fly
{
    public class FlyCollisionDetectionModel : MonoBehaviour
    {
        [Header("ハエのTransform")]
        [SerializeField] private Transform flyTransform;

        [Header("感知範囲の設定")] 
        [SerializeField] private float range = 10.0f;

        [Header("レイヤーマスク")] 
        [SerializeField, Tooltip("障害物のレイヤーマスク")] private LayerMask obstacleLayerMask;
        [SerializeField, Tooltip("検知対象のレイヤーマスク")] private LayerMask targetLayerMask;

        private readonly Collider[] colliders = new Collider[32];
        private Transform targetPlayer;
        
        public Transform TargetPlayer => targetPlayer;

        /// <summary>
        /// 当たり判定内でライトがついている状態のプレイヤーを探す
        /// </summary>
        public void FindPlayerInLights()
        {
            // 初期化
            targetPlayer = null;
            if (flyTransform == null)
                return;
            
            // 指定範囲内のオブジェクトを検知する
            int hitCount = Physics.OverlapSphereNonAlloc(
                flyTransform.position,
                range,
                colliders,
                targetLayerMask
            );
            
            // 検知したオブジェクトを確認する
            for (int i = 0; i < hitCount; i++)
            {
                // nullチェック
                var collider = colliders[i];
                if (collider == null)
                    continue;

                // プレイヤーか確認
                if (!collider.CompareTag("Player"))
                    continue;

                // FlashLightFlickerViewコンポーネントを取得
                FlashLightFlickerView flashView = null;
                flashView = collider.GetComponentInChildren<FlashLightFlickerView>();

                // コンポーネントが存在するか確認
                if (flashView == null)
                {
                    Debug.LogWarning("FlyCollisionDetectionModel: FlashLightFlickerView component not found on Player.");
                    continue;
                }

                // ライトが点灯しているか確認
                var flashLight = flashView.GetFlashLight;
                if (flashLight == null || !flashLight.enabled || !flashLight.gameObject.activeInHierarchy)
                {
                    Debug.LogWarning("FlyCollisionDetectionModel: FlashLightFlickerView component not found on Player.");
                    continue;
                }

                // 距離を計算
                Vector3 toPlayer = (collider.transform.position - flyTransform.position);
                float dist = toPlayer.magnitude;
                if (dist <= Mathf.Epsilon)
                {
                    targetPlayer = collider.transform;
                    break;
                }
                
                // 方向ベクトルを計算
                Vector3 dir = toPlayer / dist;

                // 障害物に遮られているか確認
                if (Physics.Raycast(flyTransform.position, dir, out RaycastHit hit, dist, obstacleLayerMask))
                {
                    Debug.LogWarning("FlyCollisionDetectionModel: RaycastHit object not in flash light.");
                    continue;
                }

                // プレイヤーを発見
                targetPlayer = collider.transform;
                break;
            }
        }
        
        /// <summary>
        /// 検知範囲をGizmosで表示
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (flyTransform != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(flyTransform.position, range);
            }
        }
    }
}