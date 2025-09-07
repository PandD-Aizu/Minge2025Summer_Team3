using System;
using UnityEngine;
using UniRx;

namespace Workspace.koto_thing
{
    public class PlayerPositionModel : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private CharacterController characterController;
        
        [Header("キャラクターの速度関係")]
        [SerializeField] private float moveSpeed = 5.0f;
        [SerializeField] private float runSpeedMultiplier = 2.0f;
        [SerializeField] private float crouchSpeedMultiplier = 0.5f;
        [SerializeField] private Vector3 currentVelocity;
        [SerializeField] private float speedChangeRate = 10.0f;
        
        [Header("しゃがみ関係")]
        [SerializeField] private float standingHeight = 2.0f;
        [SerializeField] private float crouchingHeight = 1.0f;
        [SerializeField] private float heightChangeSpeed = 5.0f;

        public ReactiveProperty<bool> isCrouching = new ();
        public IObservable<bool> IsCrouchingObservable => isCrouching.AsObservable();
        
        private float horizontalSpeed;
        private bool isRunning;
        private float currentHeight;
        private float targetHeight;
        
        /* プロパティ */
        public CharacterController GetCharacterController => characterController;
        public bool IsRunning { get => isRunning; set => isRunning = value; }
        public bool IsCrouching { get => isCrouching.Value; set => isCrouching.Value = value; }

        /// <summary>
        /// 水平方向の移動を行う
        /// characterController.Move()をラッピング
        /// </summary>
        /// <param name="input">xz平面の向き</param>
        public void Move(Vector2 input)
        {
            // 目標速度を計算
            float targetSpeed = input == Vector2.zero ? 0.0f : moveSpeed;

            if (IsCrouching)
                targetSpeed *= crouchSpeedMultiplier;
            else if (isRunning && input != Vector2.zero)
                targetSpeed *= runSpeedMultiplier;

            // 現在の水平速度を取得
            float currentHorizontalSpeed =
                new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;

            // Lerpを使って目標速度までスムーズに変化させる
            horizontalSpeed = 
                Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * speedChangeRate); 

            if (input != Vector2.zero)
            {
                // 入力方向を正規化
                Vector3 inputDirection = new Vector3(input.x, 0.0f, input.y).normalized;
                // カメラの向きを基準とした移動方向を計算
                Vector3 targetDirection = (transform.right * inputDirection.x + transform.forward * inputDirection.z).normalized;
                
                // 水平方向の移動ベクトルを設定
                Vector3 horizontalMovement = targetDirection * horizontalSpeed;
                currentVelocity.x = horizontalMovement.x;
                currentVelocity.z = horizontalMovement.z;
            }
            else
            {
                // 入力がない場合（減速時）は、現在の進行方向を維持したまま減速する
                Vector3 horizontalDir = new Vector3(currentVelocity.x, 0.0f, currentVelocity.z).normalized;
                Vector3 horizontalMovement = horizontalDir * horizontalSpeed;
                currentVelocity.x = horizontalMovement.x;
                currentVelocity.z = horizontalMovement.z;
            }
            
            ApplyGravity();
            
            characterController.Move(currentVelocity * Time.deltaTime);
        }
        
        /// <summary>
        /// 重力を適用する
        /// </summary>
        private void ApplyGravity()
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