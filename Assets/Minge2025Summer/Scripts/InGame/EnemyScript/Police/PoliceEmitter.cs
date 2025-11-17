using FMODUnity;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EnemyScript
{
    public class PoliceEmitter : MonoBehaviour
    {
        [Header("依存関係")]
        [SerializeField] private StudioEventEmitter enemyMoanEmitter;
        [SerializeField] private StudioEventEmitter footStepEmitter;
        [SerializeField] private StudioEventEmitter closeEnemyEmitter;

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
        private bool shouldPlayMoan = false;
        
        public void PlayEnemyMoan()
        {
            if (enemyMoanEmitter == null) return;
            enemyMoanEmitter.Stop();
            enemyMoanEmitter.Play();
        }
        
        public void PlayCloseEnemySound() => closeEnemyEmitter.Play();

        public void UpdateMoanTimer()
        {
            if (!shouldPlayMoan)
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
            if (shouldPlayMoan == true)
                return;
            
            shouldPlayMoan = true;
            SetNextMoanTime();
            moanTimer = 0.0f;
        }
        
        public void StopMoaning()
        {
            shouldPlayMoan = false;
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
            float min = Mathf.Max(0.05f, minMoanInterval);
            float max = Mathf.Max(min + 0.01f, maxMoanInterval);
            nextMoanTime = Random.Range(min, max);
        }
    }
}