using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerPositionModel : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private CharacterController characterController;
        
        [Header("キャラクターの速度関係")]
        [SerializeField] private float moveSpeed = 5.0f;
        [SerializeField] private float acceleration = 10.0f;
        [SerializeField] private float deceleration = 20.0f;

        private Vector3 currentVelocity;

        /// <summary>
        /// 水平方向の移動を行う
        /// </summary>
        /// <param name="input">xz平面の向き</param>
        public void Move(Vector2 input)
        {
            // 現在の水平方向の速度を取得
            Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

            // 目標とする移動方向と速度を計算
            Vector3 moveDirection = (transform.forward * input.y + transform.right * input.x).normalized;
            Vector3 targetVelocity = moveDirection * moveSpeed;

            // 使用する加速度を決定
            float accel = input.magnitude > 0 ? acceleration : deceleration;

            // 水平方向の速度を目標速度に向かって滑らかに変化させる
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, accel * Time.deltaTime);

            // 計算した水平速度を現在の速度に反映
            currentVelocity.x = horizontalVelocity.x;
            currentVelocity.z = horizontalVelocity.z;
            
            characterController.Move(currentVelocity * Time.deltaTime);
        }
        
        /// <summary>
        /// 重力を適用する
        /// </summary>
        public void ApplyGravity()
        {
            if (!IsGrounded())
                currentVelocity.y += Physics.gravity.y * Time.deltaTime;
            else
                currentVelocity.y = -1.0f;
        }

        /// <summary>
        /// プレイヤーが地面と接地しているかどうか
        /// </summary>
        /// <returns>プレイヤーが地面と接地していたらture, していなかったらfalse</returns>
        public bool IsGrounded()
        {
            return characterController.isGrounded;
        }
    }
}