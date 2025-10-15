using Minge2025Summer.Scripts.GameSetting;
using UnityEngine;
using UnityEngine.UI;

namespace Minge2025Summer.Scripts.Title
{
    public class ControlsController : MonoBehaviour
    {
        #region Serialized Fields
        [Header("マウス操作関係")]
        [SerializeField, Tooltip("マウスカーソルをロックするか")] 
        private Button mouseCursorLockButton;
        [SerializeField, Tooltip("マウスのY軸を反転するか")]
        private Button invertMouseCursorYButton;
        [SerializeField, Tooltip("マウスホイール操作を反転するか")]
        private Button invertMouseWheel;
        [SerializeField, Tooltip("チェックマークのオブジェクト名")]
        private string checkmarkObjectName = "Icon";
        
        [Header("キーボード操作関係")]
        [SerializeField, Tooltip("前進キーを変更するボタン")] 
        private Button moveForwardKeyButton;
        [SerializeField, Tooltip("後退キーを変更するボタン")] 
        private Button moveBackKeyButton;
        [SerializeField, Tooltip("左移動キーを変更するボタン")] 
        private Button moveLeftKeyButton;
        [SerializeField, Tooltip("右移動キーを変更するボタン")] 
        private Button moveRightKeyButton;
        [SerializeField, Tooltip("リロードキーを変更するボタン")] 
        private Button reloadKeyButton;
        [SerializeField, Tooltip("インタラクトキーを変更するボタン")] 
        private Button interactKeyButton;
        [SerializeField, Tooltip("ダッシュキーを変更するボタン")] 
        private Button sprintKeyButton;
        [SerializeField, Tooltip("しゃがみキーを変更するボタン")] 
        private Button crouchKeyButton;
        [SerializeField, Tooltip("インベントリキーを変更するボタン")] 
        private Button inventoryKeyButton;
        [SerializeField, Tooltip("重要アイテムキーを変更するボタン")] 
        private Button importantKeyButton;
        #endregion

        private bool isWaitingForKey = false;
        
        private bool isMouseCursorLocked = true;
        private bool isMouseCursorYInverted = false;
        private bool isMouseWheelInverted = false;
        
        private void Start()
        {
            SubscribeUIEvents();
            LoadExistingSettings();
        }
        
        private void SubscribeUIEvents()
        {
            mouseCursorLockButton.onClick.AddListener(() =>
            {
                isMouseCursorLocked = !isMouseCursorLocked;
                var iconImage = mouseCursorLockButton.transform.Find(checkmarkObjectName)?.GetComponent<Image>();
                if (iconImage != null)
                    iconImage.enabled = isMouseCursorLocked;
                GameController.Instance.gameSettingsController.gameSettings.mouseCursorLock = isMouseCursorLocked;
            });

            invertMouseCursorYButton.onClick.AddListener(() => {
                isMouseCursorYInverted = !isMouseCursorYInverted;
                var iconImage = invertMouseCursorYButton.transform.Find(checkmarkObjectName)?.GetComponent<Image>();
                if (iconImage != null)
                    iconImage.enabled = isMouseCursorYInverted;
                GameController.Instance.gameSettingsController.gameSettings.mouseInvertY = isMouseCursorYInverted;
            });

            invertMouseWheel.onClick.AddListener(() => {
                isMouseWheelInverted = !isMouseWheelInverted;
                var iconImage = invertMouseWheel.transform.Find(checkmarkObjectName)?.GetComponent<Image>();
                if (iconImage != null)
                    iconImage.enabled = isMouseWheelInverted;
                GameController.Instance.gameSettingsController.gameSettings.mouseWheelInvert = isMouseWheelInverted;
            });
        }

        private void LoadExistingSettings()
        {
            var settings = GameController.Instance.gameSettingsController.gameSettings;
            isMouseCursorLocked = settings.mouseCursorLock;
            isMouseCursorYInverted = settings.mouseInvertY;
            isMouseWheelInverted = settings.mouseWheelInvert;
        }
    }
}