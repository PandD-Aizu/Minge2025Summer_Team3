using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EnemyScript.Fly
{
    public class FlyMoveModel : MonoBehaviour
    {
        [Header("依存関係")] 
        [SerializeField] private Transform flyTransform;

        [Header("ハエの速度関係")]
        [SerializeField] private float speed = 3.5f;
        [SerializeField] private float maxSpeed = 3.5f;
        [SerializeField] private float rotationSpeed = 10.0f;

        private Vector3 originPos;
        private Vector3 destination;

        private Vector3 planarVelocity;
        private float verticalVelocity;

        private void Start()
        {
            if (flyTransform != null)
            {
                originPos = flyTransform.position;
                destination = originPos;
            }
        }

        /// <summary>
        /// ハエの速度を更新する
        /// </summary>
        public void UpdateVelocity()
        {
            // ターゲットへの方向を計算
            Vector3 direction = destination - flyTransform.position;
            if (direction.sqrMagnitude > 0.001f)
            {
                direction.Normalize();
            }

            // 速度を更新
            planarVelocity = Vector3.Lerp(planarVelocity, direction * speed, Time.deltaTime * 10.0f);

            // ハエの位置を更新
            flyTransform.position += (planarVelocity + new Vector3(0, verticalVelocity, 0)) * Time.deltaTime;
            
            // ハエの向きを更新
            if (planarVelocity.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(planarVelocity.normalized);
                flyTransform.rotation = Quaternion.Slerp(flyTransform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }

        /// <summary>
        /// 行き先を設定する
        /// </summary>
        /// <param name="worldPosition"></param>
        public void SetDestination(Vector3 worldPosition)
        {
            destination = worldPosition;
        }

        /// <summary>
        /// ハエを元の位置に戻す
        /// </summary>
        public void ResetPosition()
        {
            destination = originPos;
        }
        
        /// <summary>
        /// ハエの動きを即座に停止する
        /// </summary>
        public void ForceStopImmediate()
        {
            planarVelocity = Vector3.zero;
            verticalVelocity = 0;
            if (flyTransform != null)
                destination = flyTransform.position;
        }
    }
}