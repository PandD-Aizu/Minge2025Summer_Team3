using GameSetting;
using UnityEngine;

namespace Title
{
    public class GraphicsOptionController : MonoBehaviour
    {
        [SerializeField] private float brightness;
        [SerializeField] private bool allowHDR;
        [SerializeField] private int screenMode;
        [SerializeField] private Vector2 screenResolution;
        [SerializeField] private float screenFlashRate;
        [SerializeField] private int frameRateLimit;
        [SerializeField] private bool allowVSync;
        [SerializeField] private bool fidelityFX;
        [SerializeField] private bool renderingMethod;
        [SerializeField] private float renderScale;
        [SerializeField] private bool fidelityFXCasting;
        [SerializeField] private int antiAliasing;
        [SerializeField] private int textureQuality;
        [SerializeField] private int shadowQuality;
        [SerializeField] private int meshQuality;
        [SerializeField] private bool ambientOcclusion;
        [SerializeField] private bool screenSpaceReflection;
        [SerializeField] private bool subsurfaceScattering;
        [SerializeField] private bool bloom;
        [SerializeField] private bool lensFlare;
        [SerializeField] private bool filmGrain;
        [SerializeField] private bool depthOfField;
        [SerializeField] private bool lensDistortion;
        [SerializeField] private bool chromaticAberration;
        
        private void Start()
        {
            brightness = GameController.Instance.gameSettingsController.gameSettings.brightness;
            allowHDR = GameController.Instance.gameSettingsController.gameSettings.allowHDR;
            screenMode = GameController.Instance.gameSettingsController.gameSettings.screenMode;
            screenResolution = GameController.Instance.gameSettingsController.gameSettings.screenResolution;
            screenFlashRate = GameController.Instance.gameSettingsController.gameSettings.screenFlashRate;
            frameRateLimit = GameController.Instance.gameSettingsController.gameSettings.frameRateLimit;
            allowVSync = GameController.Instance.gameSettingsController.gameSettings.allowVSync;
            fidelityFX = GameController.Instance.gameSettingsController.gameSettings.fidelityFX;
            renderingMethod = GameController.Instance.gameSettingsController.gameSettings.renderingMethod;
            renderScale = GameController.Instance.gameSettingsController.gameSettings.renderScale;
            fidelityFXCasting = GameController.Instance.gameSettingsController.gameSettings.fidelityFXCasting;
            antiAliasing = GameController.Instance.gameSettingsController.gameSettings.antiAliasing;
            textureQuality = GameController.Instance.gameSettingsController.gameSettings.textureQuality;
            shadowQuality = GameController.Instance.gameSettingsController.gameSettings.shadowQuality;
            meshQuality = GameController.Instance.gameSettingsController.gameSettings.meshQuality;
            ambientOcclusion = GameController.Instance.gameSettingsController.gameSettings.ambientOcclusion;
            screenSpaceReflection = GameController.Instance.gameSettingsController.gameSettings.screenSpaceReflection;
            subsurfaceScattering = GameController.Instance.gameSettingsController.gameSettings.subsurfaceScattering;
            bloom = GameController.Instance.gameSettingsController.gameSettings.bloom;
            lensFlare = GameController.Instance.gameSettingsController.gameSettings.lensFlare;
            filmGrain = GameController.Instance.gameSettingsController.gameSettings.filmGrain;
            depthOfField = GameController.Instance.gameSettingsController.gameSettings.depthOfField;
            lensDistortion = GameController.Instance.gameSettingsController.gameSettings.lensDistortion;
            chromaticAberration = GameController.Instance.gameSettingsController.gameSettings.chromaticAberration;
        }
    }
}