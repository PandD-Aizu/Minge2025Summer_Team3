using UnityEngine;
using UnityEngine.UI;
using Minge2025Summer.Scripts.InGame.FlashLightScript;

namespace Minge2025Summer.Scripts.RenderingScript
{
    public class DissolveController : MonoBehaviour
    {
        [Header("依存関係")] 
        [SerializeField] private BatteryLevelModel batteryLevelModel;
        
        [Header("Dissolve設定")]
        [SerializeField] private Image targetImage;
        [SerializeField] private Material dissolveMaterial;
        [SerializeField] private float duration = 3.0f;
        [SerializeField] private float maxDissolveProgress = 0.6f;
        
        private float progress = 0.0f;

        private void Start()
        {
            if (targetImage != null)
            {
                dissolveMaterial = new Material(targetImage.material);
                targetImage.material = dissolveMaterial;
            }
            
            dissolveMaterial.SetFloat("_Progress", progress);
        }

        private void Update()
        {
            float targetProgress = 0.0f;

            if (batteryLevelModel != null)
            {
                float max = batteryLevelModel.GetMaxBatteryLevel <= 0.0f ? 1.0f : batteryLevelModel.GetMaxBatteryLevel;
                float normalized = Mathf.Clamp01(batteryLevelModel.GetBatteryLevel / max);

                const float threshold = 0.1f;
                if (normalized <= threshold)
                {
                    float t = 1.0f - (normalized / threshold);
                    targetProgress = Mathf.Clamp01(t) * maxDissolveProgress;
                }
                else
                {
                    targetProgress = 0.0f;
                }
            }
            else
            {
                if (progress < 1.0f)
                {
                    targetProgress = Mathf.Min(1.0f, progress + Time.deltaTime / duration);
                }
            }

            float step = (duration > 0.0f) ? (Time.deltaTime / duration) : 1.0f;
            progress = Mathf.MoveTowards(progress, targetProgress, step);
            
            dissolveMaterial.SetFloat("_Progress", progress);
        }
    }
}