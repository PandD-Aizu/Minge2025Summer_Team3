using FMODUnity;
using UnityEngine;

namespace Workspace.momiji1107
{
    public class PoliceEmitter : MonoBehaviour
    {
        [Header("依存関係")]
        [SerializeField] private StudioEventEmitter enemyMoanEmitter;
        [SerializeField] private StudioEventEmitter footStepEmitter;

        [Header("うめき声設定")]
        [SerializeField] private float minMoanInterval = 2.0f;
        [SerializeField] private float maxMoanInterval = 5.0f;
        
        [Header("足音設定")] 
        [SerializeField] private float referenceSpeed = 5.0f;
        [SerializeField] private float stepIntervalAtRefSpeed = 0.5f;
        [SerializeField] private float minHorizontalSpeed = 0.1f;
        [SerializeField] private float minStepInterval = 0.2f;

        private float moanTimer = 0.0f;
        private float stepTimer = 0.0f;
        private float nextMoanTime;
        private bool sholdPlayMoan = false;
        
        public void PlayEnemyMoan() => enemyMoanEmitter.Play();

        public void UpdateMoanTimer()
        {
            if (!sholdPlayMoan)
                return;

            moanTimer += Time.deltaTime;

            if (moanTimer >= nextMoanTime)
            {
                PlayEnemyMoan();
                SetNextMoanTime();
                moanTimer = 0.0f;
            }
        }

        public void StartMoaning()
        {
            sholdPlayMoan = true;
            SetNextMoanTime();
            moanTimer = 0.0f;
        }
        
        public void StopMoaning()
        {
            sholdPlayMoan = false;
            moanTimer = 0.0f;
        }
        
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
        
        /* ---以下ヘルパー関数--- */
        private void SetNextMoanTime()
        {
            nextMoanTime = Random.Range(minMoanInterval, maxMoanInterval);
        }
    }
}