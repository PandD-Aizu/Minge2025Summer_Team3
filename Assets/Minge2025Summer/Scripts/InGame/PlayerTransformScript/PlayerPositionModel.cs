using System;
using UniRx;
using Unity.Cinemachine;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.PlayerTransformScript
{
    public class PlayerPositionModel : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private CharacterController characterController;
        
        [Header("キャラクターの速度関係")]
        [SerializeField] private float moveSpeed = 5.0f;
        [SerializeField] private float runSpeedMultiplier = 2.0f;
        [SerializeField] private float crouchSpeedMultiplier = 0.5f;
        [SerializeField] private float puddleSpeedMultiplier = 0.5f;
        [SerializeField] private Vector3 currentVelocity;
        [SerializeField] private float speedChangeRate = 10.0f;

        [Header("しゃがみ関係")] 
        [SerializeField] private CapsuleCollider playerCollider;
        [SerializeField] private float standingHeight = 2.0f;
        [SerializeField] private float crouchingHeight = 1.0f;
        [SerializeField] private float heightChangeSpeed = 5.0f;

        [Header("Cinemachine関係")] 
        [SerializeField] private CinemachineBasicMultiChannelPerlin noiseSetting;
        [SerializeField] private Vector2 stoppingNoiseValue;
        [SerializeField] private Vector2 walkingNoiseValue;
        [SerializeField] private Vector2 runningNoiseValue;
        [SerializeField] private Vector2 crouchingNoiseValue;
        [SerializeField] private float amplitudeChangeSpeed = 5.0f;
        [SerializeField] private float frequencyChangeSpeed = 5.0f;

        public ReactiveProperty<bool> isCrouching = new ();
        public IObservable<bool> IsCrouchingObservable => isCrouching.AsObservable();
        
        public bool ForceCrouch { get; set; }
        
        private float horizontalSpeed;
        private bool isRunning;
        private bool isPuddling;
        private float currentHeight;
        private float targetHeight;
        
        // カメラの揺れ
        private float targetAmplitude;
        private float targetFrequency;
        
        /* プロパティ */
        public CharacterController GetCharacterController => characterController;
        public bool IsRunning { get => isRunning; set => isRunning = value; }
        public bool IsCrouching { get => isCrouching.Value; set => isCrouching.Value = value; }
        public bool IsPuddling { get => isPuddling; set => isPuddling = value; } //Puddleから操作

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
            if (IsPuddling)
                targetSpeed *= puddleSpeedMultiplier;

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
        /// カメラのブレ設定を変更
        /// </summary>
        /// <param name="input">移動入力</param>
        public void ChangeNoiseSetting(Vector2 input)
        {
            if (isRunning)
            {
                targetAmplitude = runningNoiseValue.x;
                targetFrequency = runningNoiseValue.y;
            }
            else if (IsCrouching)
            {
                targetAmplitude = crouchingNoiseValue.x;
                targetFrequency = crouchingNoiseValue.y;
            }
            else if (input != Vector2.zero)
            {
                targetAmplitude = walkingNoiseValue.x;
                targetFrequency = walkingNoiseValue.y;
            }
            else
            {
                targetAmplitude = stoppingNoiseValue.x;
                targetFrequency = stoppingNoiseValue.y;
            }
            
            

            if (noiseSetting != null)
            {
                noiseSetting.AmplitudeGain = 
                    Mathf.Lerp(noiseSetting.AmplitudeGain, targetAmplitude, Time.deltaTime * amplitudeChangeSpeed);
                
                noiseSetting.FrequencyGain = 
                    Mathf.Lerp(noiseSetting.FrequencyGain, targetFrequency, Time.deltaTime * frequencyChangeSpeed);
            }

        }
        
        public void ChangeColliderHeight()
        {
            targetHeight = IsCrouching ? crouchingHeight : standingHeight;
            currentHeight = Mathf.Lerp(playerCollider.height, targetHeight, Time.deltaTime * heightChangeSpeed);
            playerCollider.height = currentHeight;
            characterController.height = currentHeight;
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