using GameSetting;
using UnityEngine;
using UnityEngine.UI;

namespace Title
{
    public class CameraOptionController : MonoBehaviour
    {
        [SerializeField] private Button cameraInvertButton;
        [SerializeField] private Button cameraAimingInvertButton;
        [SerializeField] private Slider cameraSensitivitySlider;
        [SerializeField] private Slider cameraAimingSensitivitySlider;
        [SerializeField] private Slider mouseSensitivitySlider;
        
        private bool isCameraInvert;
        private bool isCameraAimingInvert;
        private float cameraSensitivity;
        private float cameraAimingSensitivity;
        private float mouseSensitivity;

        private void Start()
        {
            isCameraInvert = GameController.Instance.gameSettingsController.gameSettings.cameraInvert;
            isCameraAimingInvert = GameController.Instance.gameSettingsController.gameSettings.cameraAimingInvert;
            cameraSensitivity = GameController.Instance.gameSettingsController.gameSettings.cameraSensitivity;
            cameraAimingSensitivity = GameController.Instance.gameSettingsController.gameSettings.cameraAimingSensitivity;
            mouseSensitivity = GameController.Instance.gameSettingsController.gameSettings.mouseSensitivity;
            
            cameraInvertButton.GetComponentInChildren<SpriteRenderer>().enabled = isCameraInvert;
            cameraAimingInvertButton.GetComponentInChildren<SpriteRenderer>().enabled = isCameraAimingInvert;
            cameraSensitivitySlider.value = cameraSensitivity;
            cameraAimingSensitivitySlider.value = cameraAimingSensitivity;
            mouseSensitivitySlider.value = mouseSensitivity;
            
            cameraInvertButton.onClick.AddListener(() =>
            {
                isCameraInvert = !isCameraInvert;
                cameraInvertButton.GetComponentInChildren<SpriteRenderer>().enabled = isCameraInvert;
                GameController.Instance.gameSettingsController.gameSettings.cameraInvert = isCameraInvert;
            });
            
            cameraAimingInvertButton.onClick.AddListener(() =>
            {
                isCameraAimingInvert = !isCameraAimingInvert;
                cameraAimingInvertButton.GetComponentInChildren<SpriteRenderer>().enabled = isCameraAimingInvert;
                GameController.Instance.gameSettingsController.gameSettings.cameraAimingInvert = isCameraAimingInvert;
            });
            
            cameraSensitivitySlider.onValueChanged.AddListener(value =>
            {
                cameraSensitivity = value;
                GameController.Instance.gameSettingsController.gameSettings.cameraSensitivity = (int)cameraSensitivity;
            });
            
            cameraAimingSensitivitySlider.onValueChanged.AddListener(value =>
            {
                cameraAimingSensitivity = value;
                GameController.Instance.gameSettingsController.gameSettings.cameraAimingSensitivity = (int)cameraAimingSensitivity;
            });
            
            mouseSensitivitySlider.onValueChanged.AddListener(value =>
            {
                mouseSensitivity = value;
                GameController.Instance.gameSettingsController.gameSettings.mouseSensitivity = (int)mouseSensitivity;
            });
        }
    }
}