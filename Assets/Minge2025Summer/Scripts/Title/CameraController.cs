using Minge2025Summer.Scripts.GameSetting;
using UnityEngine;
using UnityEngine.UI;

namespace Minge2025Summer.Scripts.Title
{
    public class CameraController : MonoBehaviour
    {
        #region SerializeFields

        [Header("カメラ操作反転設定")] 
        [SerializeField] private string iconName = "Icon";
        [SerializeField] private Button cameraInvertButton;
        [SerializeField] private Button cameraAimingInvertButton;
        
        [Header("カメラ感度設定")]
        [SerializeField] private Slider cameraSensitivitySlider;
        [SerializeField] private Slider cameraAimingSensitivitySlider;
        [SerializeField] private Slider mouseSensitivitySlider;
        #endregion
        
        private bool isCameraInverted;
        private bool isCameraAimingInverted;

        private void Start()
        {
            LoadExistingsSettings();
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            cameraInvertButton.onClick.AddListener(() =>
            {
                isCameraInverted = !isCameraInverted;
                var iconImage = cameraInvertButton.transform.Find(iconName)?.GetComponent<Image>();
                if (iconImage != null)
                    iconImage.enabled = isCameraInverted;
                GameController.Instance.gameSettingsController.gameSettings.mouseCursorLock = isCameraInverted;
            });
            
            cameraAimingInvertButton.onClick.AddListener(() =>
            {
                isCameraAimingInverted = !isCameraAimingInverted;
                var iconImage = cameraAimingInvertButton.transform.Find(iconName)?.GetComponent<Image>();
                if (iconImage != null)
                    iconImage.enabled = isCameraAimingInverted;
                GameController.Instance.gameSettingsController.gameSettings.cameraAimingInvert = isCameraAimingInverted;
            });
            
            cameraSensitivitySlider.onValueChanged.AddListener(value =>
            {
                GameController.Instance.gameSettingsController.gameSettings.cameraSensitivity = value;
            });
            
            cameraAimingSensitivitySlider.onValueChanged.AddListener(value =>
            {
                GameController.Instance.gameSettingsController.gameSettings.cameraAimingSensitivity = value;
            });
            
            mouseSensitivitySlider.onValueChanged.AddListener(value =>
            {
                GameController.Instance.gameSettingsController.gameSettings.mouseSensitivity = value;
            });
        }

        private void LoadExistingsSettings()
        {
            if (GameController.Instance == null ||
                GameController.Instance.gameSettingsController == null ||
                GameController.Instance.gameSettingsController.gameSettings == null)
            {
                Debug.LogWarning("GameControllerまたはGameSettingsController、またはGameSettingsが見つかりません。既存の設定を読み込めませんでした。");
                return;
            }
            
            var settings = GameController.Instance.gameSettingsController.gameSettings;
            isCameraInverted = settings.cameraInvert;
            isCameraAimingInverted = settings.cameraAimingInvert;
            cameraSensitivitySlider.value = settings.cameraSensitivity;
            cameraAimingSensitivitySlider.value = settings.cameraAimingSensitivity;
            mouseSensitivitySlider.value = settings.mouseSensitivity;
            
            var cameraInvertIcon = cameraInvertButton?.transform.Find(iconName)?.GetComponent<Image>();
            if (cameraInvertIcon != null) cameraInvertIcon.enabled = isCameraInverted;
            var cameraAimingInvertIcon = cameraAimingInvertButton?.transform.Find(iconName)?.GetComponent<Image>();
            if (cameraAimingInvertIcon != null) cameraAimingInvertIcon.enabled = isCameraAimingInverted;
        }
    }
}
