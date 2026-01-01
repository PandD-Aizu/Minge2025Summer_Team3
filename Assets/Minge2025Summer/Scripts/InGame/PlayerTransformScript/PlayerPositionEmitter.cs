using System;
using FMODUnity;
using Minge2025Summer.Scripts.InGame.AcousticsScript;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.PlayerTransformScript
{
    public class PlayerPositionEmitter : MonoBehaviour
    {
        [SerializeField] private StudioEventEmitter footStepEmitter;

        [SerializeField] private CharacterController characterController;

        [Header("足音設定")] 
        [SerializeField] private float referenceSpeed = 5.0f;
        [SerializeField] private float stepIntervalAtRefSpeed = 0.5f;
        [SerializeField] private float minHorizontalSpeed = 0.1f;
        [SerializeField] private float minStepInterval = 0.2f;
        [SerializeField] private float runningSoundRadius = 3.0f;

        private float stepTimer;
        private bool wasMoving;
        
        private readonly Subject<SoundEvent> onSoundEmitted = new Subject<SoundEvent>();
        public IObservable<SoundEvent> OnSoundEmitted => onSoundEmitted;

        /// <summary>
        /// 足音を再生する
        /// </summary>
        /// <param name="speed">プレイヤーのスピード</param>
        /// <param name="isRunning">走っているかどうか</param>
        public void PlayFootStep(float speed, bool isRunning)
        {
            if (speed > minHorizontalSpeed)
            {
                float interval = Mathf.Max(stepIntervalAtRefSpeed * (referenceSpeed / speed), minStepInterval);
                stepTimer += Time.deltaTime;

                if (stepTimer >= interval)
                {
                    footStepEmitter.Play();
                    
                    // 走っている場合、敵に聞こえる音イベントを発行
                    if (isRunning)
                    {
                        var soundEvent = new SoundEvent(transform.position, runningSoundRadius, SoundType.Footstep, gameObject);
                        onSoundEmitted.OnNext(soundEvent);
                    }
                    
                    stepTimer = 0.0f;
                }
            }
            else
            {
                stepTimer = 0.0f;
            }
        }
    }
}