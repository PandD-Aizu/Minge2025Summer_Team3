using Minge2025Summer.Scripts.InGame.FlashLightScript;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EnemyScript.Fly
{
    public class FlyCollisionDetectionModel : MonoBehaviour
    {
        [Header("ハエのTransform")]
        [SerializeField] private Transform flyTransform;

        [Header("感知範囲の設定")] 
        [SerializeField] private float range;

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
            int hitCount = Physics.OverlapSphereNonAlloc(
                flyTransform.position,
                range,
                colliders,
                targetLayerMask
            );
            
            for (int i = 0; i < hitCount; i++)
            {
                var collider = colliders[i];
                if (collider != null && collider.CompareTag("Player"))
                {
                    if (collider.TryGetComponent<FlashLightFlickerView>(out var flashLightFlickerView))
                    {
                        if (flashLightFlickerView.GetFlashLight != null)
                        {
                            targetPlayer = collider.transform;
                            break;
                        }    
                    }
                }
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