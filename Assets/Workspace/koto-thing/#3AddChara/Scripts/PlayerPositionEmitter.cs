using FMODUnity;
using UnityEngine;

namespace Workspace.koto_thing
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

        private float stepTimer;
        private bool wasMoving;

        /// <summary>
        /// 足音を再生する
        /// </summary>
        /// <param name="speed">プレイヤーのスピード</param>
        public void PlayFootStep(float speed)
        {
            if (speed > minHorizontalSpeed)
            {
                float interval = Mathf.Max(stepIntervalAtRefSpeed * (referenceSpeed / speed), minStepInterval);
                stepTimer += Time.deltaTime;

                if (stepTimer >= interval)
                {
                    footStepEmitter.Play();
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