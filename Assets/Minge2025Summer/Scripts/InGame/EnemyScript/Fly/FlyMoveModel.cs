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

        private Transform origin;
        private Transform target;
        private readonly float currentSpeedMultiplier = 1f;
        private float speedBoostExpireTime;
        private Vector3 planarVelocity;
        private float verticalVelocity;

        private void Start()
        {
            origin = transform;
        }

        /// <summary>
        /// ハエの速度を更新する
        /// </summary>
        public void UpdateVelocity()
        {
            // ターゲットへの方向を計算
            Vector3 direction = flyTransform.position - target.position;
            direction.Normalize();

            // 速度を更新
            planarVelocity = Vector3.Lerp(planarVelocity, direction * speed * currentSpeedMultiplier, Time.deltaTime * 10f);

            // ハエの位置を更新
            Vector3 totalVelocity = planarVelocity + new Vector3(0, verticalVelocity, 0);
            target.position += totalVelocity * Time.deltaTime;

            // ハエの回転を更新
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                target.rotation = Quaternion.Slerp(target.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// 行き先を設定する
        /// </summary>
        /// <param name="worldPosition"></param>
        public void SetDestination(Vector3 worldPosition)
        {
            target.position = worldPosition;
        }

        /// <summary>
        /// ハエを元の位置に戻す
        /// </summary>
        public void ResetPosition()
        {
            target.position = origin.position;
        }
        
        /// <summary>
        /// ハエの動きを即座に停止する
        /// </summary>
        public void ForceStopImmediate()
        {
            planarVelocity = Vector3.zero;
            verticalVelocity = 0f;
        }
    }
}