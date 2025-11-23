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
            // 小さなノイズで鳴らないようにマージンを付けて閾値判定
            const float epsilon = 0.01f;
            if (speed <= minHorizontalSpeed + epsilon)
            {
                stepTimer = 0.0f;
                return;
            }

            // ゼロ割り防止
            float safeSpeed = Mathf.Max(speed, 0.0001f);

            // 間隔が極端に大きくならないよう上限を設定（必要に応じて調整）
            float interval = Mathf.Clamp(stepIntervalAtRefSpeed * (referenceSpeed / safeSpeed), minStepInterval, 5.0f);

            stepTimer += Time.deltaTime;
            if (stepTimer >= interval)
            {
                footStepEmitter?.Play();
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