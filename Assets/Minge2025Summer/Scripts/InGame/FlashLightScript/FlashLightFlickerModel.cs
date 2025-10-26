using Minge2025Summer.Scripts.InGame.FlashLightScript.Enum;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.FlashLightScript
{
    public class FlashLightFlickerModel : MonoBehaviour
    {
        [Header("点滅の状態")]
        [SerializeField] private FlickerState currentState;

        [Header("通常の点滅設定")]
        [SerializeField] private float minIntensity = 0.8f;
        [SerializeField] private float maxIntensity = 1.5f;
        [SerializeField, Range(0.1f, 100.0f)] private float normalFlickerSpeed = 50.0f;

        [Header("激しい点滅設定")] 
        [SerializeField] private float intenseMinIntensity = 0.5f;
        [SerializeField] private float intenseMaxIntensity = 2.0f;
        [SerializeField, Range(0.1f, 200.0f)] private float intenseFlickerSpeed = 100.0f;
        
        private float randomOffset;
        private Coroutine intenseFlickerCoroutine;
        
        /* プロパティ */
        public FlickerState GetCurrentState => currentState;
        public float GetMinIntensity => minIntensity;
        public float GetMaxIntensity => maxIntensity;
        public float GetNormalFlickerSpeed => normalFlickerSpeed;
        public float GetIntenseMinIntensity => intenseMinIntensity;
        public float GetIntenseMaxIntensity => intenseMaxIntensity;
        public float GetIntenseFlickerSpeed => intenseFlickerSpeed;
        public float RandomOffset { get => randomOffset; set => randomOffset = value; }

        /// <summary>
        /// 点滅の状態を設定する
        /// </summary>
        /// <param name="newState">次の点滅の状態</param>
        /// <param name="flashLight">懐中電灯</param>
        public void SetFlickerState(FlickerState newState, Light flashLight)
        {
            if (currentState == newState)
                return;

            currentState = newState;

            if (intenseFlickerCoroutine != null)
            {
                StopCoroutine(intenseFlickerCoroutine);
                intenseFlickerCoroutine = null;
            }

            switch (currentState)
            {
                case FlickerState.STABLE:
                    flashLight.enabled = true;
                    flashLight.intensity = maxIntensity;
                    break;
                
                case FlickerState.NORMALFLICKER:
                    flashLight.enabled = true;
                    break;
                
                case FlickerState.INTENSEFLICKER:
                    flashLight.enabled = true;
                    break;
                
                case FlickerState.OFF:
                    flashLight.enabled = false;
                    flashLight.intensity = 0.0f;
                    break;
            }
        }
    }
}