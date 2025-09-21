using UnityEngine;

namespace Workspace.koto_thing
{
    public class FlashLightFlickerView : MonoBehaviour
    {
        [Header("懐中電灯のライト")]
        [SerializeField] private Light flashLight;
        
        /* プロパティ */
        public Light GetFlashLight => flashLight;

        /// <summary>
        /// パーリンノイズを使用した通常のライト点滅
        /// </summary>
        /// <param name="flickerSpeed">フリックするスピード</param>
        /// <param name="randomOffset">ランダムなオフセット</param>
        /// <param name="minIntensity">最小の光の強さ</param>
        /// <param name="maxIntensity">最大の光の強さ</param>
        public void NormalFlicker(float flickerSpeed, float randomOffset, float minIntensity, float maxIntensity)
        {
            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, randomOffset);
            flashLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
        }
        
        /// <summary>
        /// 通常のライト点滅よりも激しいライト点滅
        /// </summary>
        /// <param name="flickerSpeed">フリックするスピード</param>
        /// <param name="randomOffset">ランダムなオフセット</param>
        /// <param name="minIntensity">最小の光の強さ</param>
        /// <param name="maxIntensity">最大の光の強さ</param>
        public void IntenseFlicker(float flickerSpeed, float randomOffset, float minIntensity, float maxIntensity)
        {
            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, randomOffset);
            noise = noise * noise;
            flashLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
        }
    }
}