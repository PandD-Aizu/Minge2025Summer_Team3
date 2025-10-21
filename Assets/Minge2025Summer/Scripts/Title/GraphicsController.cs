using Minge2025Summer.Scripts.GameSetting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

namespace Minge2025Summer.Scripts.Title
{
    public class GraphicsController : MonoBehaviour
    {
        [Header("UI参照")] 
        [SerializeField] private TMP_Dropdown qualityDropdown;
        [SerializeField] private TMP_Dropdown textureQualityDropdown;
        [SerializeField] private TMP_Dropdown antiAliasingDropdown;
        [SerializeField] private Toggle shadowsToggle;
        [SerializeField] private Slider shadowDistanceSlider;
        [SerializeField] private Toggle ssaoToggle;

        [Header("メインカメラ参照")] 
        [SerializeField] private Camera mainCamera;
        
        private UniversalRenderPipelineAsset currentURPAsset;
        private UniversalAdditionalCameraData cameraData;
        private ScriptableRendererData rendererData;

        private float savedShadowDistance;

        private const string SSAO_FEATURE_NAME = "ScreenSpaceAmbientOcclusion";

        private void Start()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            if (mainCamera != null)
                cameraData = mainCamera.GetComponent<UniversalAdditionalCameraData>();
            
            qualityDropdown.onValueChanged.AddListener(SetQualityPreset);
            textureQualityDropdown.onValueChanged.AddListener(SetTextureQuality);
            antiAliasingDropdown.onValueChanged.AddListener(SetAntiAliasing);
            shadowsToggle.onValueChanged.AddListener(SetShadows);
            shadowDistanceSlider.onValueChanged.AddListener(SetShadowDistance);
            ssaoToggle.onValueChanged.AddListener(SetSSAO);

            LoadExistingSettings();
        }

        private void LoadExistingSettings()
        {
            var settings = GameController.Instance.gameSettingsController.gameSettings;

            int qualityIndex = settings.presetIndex;
            qualityDropdown.value = qualityIndex;
            QualitySettings.SetQualityLevel(qualityIndex, true);

            UpdateURPAssetReferences();

            int textureIndex = settings.textureIndex;
            textureQualityDropdown.value = textureIndex;
            SetTextureQuality(textureIndex);

            int aaIndex = settings.antiAliasingIndex;
            antiAliasingDropdown.value = aaIndex;
            SetAntiAliasing(aaIndex);

            bool shadowsEnabled = settings.shadowsEnabled;
            shadowsToggle.isOn = shadowsEnabled;
            SetShadows(shadowsEnabled);

            float shadowDistance = settings.shadowDistance;
            shadowDistanceSlider.value = shadowDistance;
            SetShadowDistance(shadowDistance);
            
            bool ssaoEnabled = settings.ambientOcclusionEnabled;
            ssaoToggle.isOn = ssaoEnabled;
            SetSSAO(ssaoEnabled);

            RefreshALLUI();
        }

        private void UpdateURPAssetReferences()
        {
            currentURPAsset = QualitySettings.GetRenderPipelineAssetAt(QualitySettings.GetQualityLevel()) as UniversalRenderPipelineAsset;
            if (currentURPAsset != null)
                rendererData = currentURPAsset.rendererDataList[0] as ScriptableRendererData;
        }

        private void RefreshALLUI()
        {
            var settings = GameController.Instance.gameSettingsController.gameSettings;
            if (currentURPAsset == null || rendererData == null || cameraData == null)
            {
                Debug.LogError("参照が正しく設定されてないよ");
                return;
            }

            qualityDropdown.value = QualitySettings.GetQualityLevel();
            
            textureQualityDropdown.value = QualitySettings.globalTextureMipmapLimit;

            if (cameraData.antialiasing == AntialiasingMode.FastApproximateAntialiasing)
                antiAliasingDropdown.value = 1;
            else if (currentURPAsset.msaaSampleCount == 2)
                antiAliasingDropdown.value = 2;
            else if (currentURPAsset.msaaSampleCount == 4)
                antiAliasingDropdown.value = 3;
            else
                antiAliasingDropdown.value = 0;

            shadowsToggle.isOn = settings.shadowsEnabled;
            shadowDistanceSlider.value = currentURPAsset.shadowDistance;
            shadowDistanceSlider.interactable = currentURPAsset.supportsMainLightShadows;
            
            ssaoToggle.isOn = IsRendererFeatureActive(SSAO_FEATURE_NAME);
        }

        private void SetQualityPreset(int index)
        {
            var settings = GameController.Instance.gameSettingsController.gameSettings;
            QualitySettings.SetQualityLevel(index, true);
            settings.presetIndex = index;
            
            UpdateURPAssetReferences();
            RefreshALLUI();
        }

        private void SetTextureQuality(int index)
        {
            var settings = GameController.Instance.gameSettingsController.gameSettings;
            QualitySettings.globalTextureMipmapLimit = index;
            settings.textureIndex = index;
        }

        private void SetAntiAliasing(int index)
        {
            var settings = GameController.Instance.gameSettingsController.gameSettings;
            if (currentURPAsset == null || cameraData == null)
                return;

            cameraData.antialiasing = AntialiasingMode.None;
            currentURPAsset.msaaSampleCount = 1;

            switch (index)
            {
                case 0:
                    break;
                case 1:
                    cameraData.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
                    break;
                case 2:
                    currentURPAsset.msaaSampleCount = 2;
                    break;
                case 3:
                    currentURPAsset.msaaSampleCount = 4;
                    break;
            }
            
            settings.antiAliasingIndex = index;
        }

        private void SetShadows(bool isEnabled)
        {
            if (currentURPAsset != null)
            {
                if (isEnabled)
                {
                    // 設定値または直前の値を復元
                    var settings = GameController.Instance.gameSettingsController.gameSettings;
                    float restore = savedShadowDistance > 0f ? savedShadowDistance : settings.shadowDistance;
                    currentURPAsset.shadowDistance = Mathf.Max(restore, 0f);
                    shadowDistanceSlider.interactable = true;
                }
                else
                {
                    // 現在の距離を保存して0に
                    savedShadowDistance = currentURPAsset.shadowDistance;
                    currentURPAsset.shadowDistance = 0f;
                    shadowDistanceSlider.interactable = false;
                }
            }

            var s = GameController.Instance.gameSettingsController.gameSettings;
            s.shadowsEnabled = isEnabled;
        }

        private void SetShadowDistance(float distance)
        {
            if (currentURPAsset != null)
            {
                currentURPAsset.shadowDistance = distance;
            }
            
            var settings = GameController.Instance.gameSettingsController.gameSettings;
            settings.shadowDistance = distance;
        }

        private void SetSSAO(bool isEnabled)
        {
            SetRendererFeatureActive(SSAO_FEATURE_NAME, isEnabled);
            
            var settings = GameController.Instance.gameSettingsController.gameSettings;
            settings.ambientOcclusionEnabled = isEnabled;
        }

        #region Helper Functions
        
        /// <summary>
        /// レンダラーフィーチャーが有効かどうかを取得する
        /// </summary>
        /// <param name="featureName">レンダラーフィーチャーの名前</param>
        /// <returns>有効かどうかを返す</returns>
        private bool IsRendererFeatureActive(string featureName)
        {
            if (rendererData == null)
                return false;

            foreach (var feature in rendererData.rendererFeatures)
            {
                if (feature != null && feature.name == featureName)
                    return feature.isActive;
            }

            return false;
        }

        /// <summary>
        /// レンダラーフィーチャーの有効/無効を設定する
        /// </summary>
        /// <param name="featureName">レンダラーフィーチャーの名前</param>
        /// <param name="isEnabled">有効無効どちらにするか</param>
        private void SetRendererFeatureActive(string featureName, bool isEnabled)
        {
            if (rendererData == null)
                return;

            foreach (var feature in rendererData.rendererFeatures)
            {
                if (feature != null && feature.name == featureName)
                {
                    feature.SetActive(isEnabled);
                    break;
                }
            }
        }
        
        #endregion
    }
}