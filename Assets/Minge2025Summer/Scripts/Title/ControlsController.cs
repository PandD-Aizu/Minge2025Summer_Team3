using System;
using System.Collections;
using Minge2025Summer.Scripts.GameSetting;
using TMPro;
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
        
        [Header("デフォルトに戻す")]
        [SerializeField] private Button resetToDefaultButton;
        #endregion

        private bool isWaitingForKey;
        
        private bool isMouseCursorLocked = true;
        private bool isMouseCursorYInverted;
        private bool isMouseWheelInverted;
        
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
            
            moveForwardKeyButton.onClick.AddListener(() =>
            {
                if (isWaitingForKey) return;
                isWaitingForKey = true;
                moveForwardKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
                StartCoroutine(WaitForKeyPress((newKey) =>
                {
                    moveForwardKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = newKey.ToString();
                    GameController.Instance.gameSettingsController.gameSettings.keyMoveForward = newKey;
                    isWaitingForKey = false;
                }));
            });
            
            moveBackKeyButton.onClick.AddListener(() =>
            {
                if (isWaitingForKey) return;
                isWaitingForKey = true;
                moveBackKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
                StartCoroutine(WaitForKeyPress((newKey) =>
                {
                    moveBackKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = newKey.ToString();
                    GameController.Instance.gameSettingsController.gameSettings.keyMoveBackward = newKey;
                    isWaitingForKey = false;
                }));
            });
            
            moveLeftKeyButton.onClick.AddListener(() =>
            {
                if (isWaitingForKey) return;
                isWaitingForKey = true;
                moveLeftKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
                StartCoroutine(WaitForKeyPress((newKey) =>
                {
                    moveLeftKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = newKey.ToString();
                    GameController.Instance.gameSettingsController.gameSettings.keyMoveLeft = newKey;
                    isWaitingForKey = false;
                }));
            });
            
            moveRightKeyButton.onClick.AddListener(() =>
            {
                if (isWaitingForKey) return;
                isWaitingForKey = true;
                moveRightKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
                StartCoroutine(WaitForKeyPress((newKey) =>
                {
                    moveRightKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = newKey.ToString();
                    GameController.Instance.gameSettingsController.gameSettings.keyMoveRight = newKey;
                    isWaitingForKey = false;
                }));
            });
            
            reloadKeyButton.onClick.AddListener(() =>
            {
                if (isWaitingForKey) return;
                isWaitingForKey = true;
                reloadKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
                StartCoroutine(WaitForKeyPress((newKey) =>
                {
                    reloadKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = newKey.ToString();
                    GameController.Instance.gameSettingsController.gameSettings.keyReload = newKey;
                    isWaitingForKey = false;
                }));
            });
            
            interactKeyButton.onClick.AddListener(() =>
            {
                if (isWaitingForKey) return;
                isWaitingForKey = true;
                interactKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
                StartCoroutine(WaitForKeyPress((newKey) =>
                {
                    interactKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = newKey.ToString();
                    GameController.Instance.gameSettingsController.gameSettings.keyInteract = newKey;
                    isWaitingForKey = false;
                }));
            });
            
            sprintKeyButton.onClick.AddListener(() =>
            {
                if (isWaitingForKey) return;
                isWaitingForKey = true;
                sprintKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
                StartCoroutine(WaitForKeyPress((newKey) =>
                {
                    sprintKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = newKey.ToString();
                    GameController.Instance.gameSettingsController.gameSettings.keySprint = newKey;
                    isWaitingForKey = false;
                }));
            });
            
            crouchKeyButton.onClick.AddListener(() =>
            {
                if (isWaitingForKey) return;
                isWaitingForKey = true;
                crouchKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
                StartCoroutine(WaitForKeyPress((newKey) =>
                {
                    crouchKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = newKey.ToString();
                    GameController.Instance.gameSettingsController.gameSettings.keyCrouch = newKey;
                    isWaitingForKey = false;
                }));
            });
            
            inventoryKeyButton.onClick.AddListener(() =>
            {
                if (isWaitingForKey) return;
                isWaitingForKey = true;
                inventoryKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
                StartCoroutine(WaitForKeyPress((newKey) =>
                {
                    inventoryKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = newKey.ToString();
                    GameController.Instance.gameSettingsController.gameSettings.keyInventory = newKey;
                    isWaitingForKey = false;
                }));
            });
            
            importantKeyButton.onClick.AddListener(() =>
            {
                if (isWaitingForKey) return;
                isWaitingForKey = true;
                importantKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
                StartCoroutine(WaitForKeyPress((newKey) =>
                {
                    importantKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = newKey.ToString();
                    GameController.Instance.gameSettingsController.gameSettings.keyImportant = newKey;
                    isWaitingForKey = false;
                }));
            });
            
            resetToDefaultButton.onClick.AddListener(() =>
            {
                GameController.Instance.gameSettingsController.gameSettings.ResetKeyBindings();
                LoadExistingSettings();
            });
        }
        
        #region Helper Functions
        /// <summary>
        /// 設定が既に存在する場合、それを読み込んでUIに反映する。
        /// </summary>
        private void LoadExistingSettings()
        {
            if (GameController.Instance == null ||
                GameController.Instance.gameSettingsController == null ||
                GameController.Instance.gameSettingsController.gameSettings == null)
            {
                Debug.LogWarning("GameControllerまたはGameSettingsController、またはGameSettingsが見つかりません。既存の設定を読み込めませんでした。");
                return;
            }
            
            var settings = GameController.Instance.gameSettingsController.gameSettings;
            isMouseCursorLocked = settings.mouseCursorLock;
            isMouseCursorYInverted = settings.mouseInvertY;
            isMouseWheelInverted = settings.mouseWheelInvert;

            // 初期表示を反映する
            var lockIcon = mouseCursorLockButton?.transform.Find(checkmarkObjectName)?.GetComponent<Image>();
            if (lockIcon != null) lockIcon.enabled = isMouseCursorLocked;

            var invertYIcon = invertMouseCursorYButton?.transform.Find(checkmarkObjectName)?.GetComponent<Image>();
            if (invertYIcon != null) invertYIcon.enabled = isMouseCursorYInverted;

            var wheelIcon = invertMouseWheel?.transform.Find(checkmarkObjectName)?.GetComponent<Image>();
            if (wheelIcon != null) wheelIcon.enabled = isMouseWheelInverted;
            
            moveForwardKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = settings.keyMoveForward.ToString();
            moveBackKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = settings.keyMoveBackward.ToString();
            moveLeftKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = settings.keyMoveLeft.ToString();
            moveRightKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = settings.keyMoveRight.ToString();
            reloadKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = settings.keyReload.ToString();
            interactKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = settings.keyInteract.ToString();
            sprintKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = settings.keySprint.ToString();
            crouchKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = settings.keyCrouch.ToString();
            inventoryKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = settings.keyInventory.ToString();
            importantKeyButton.GetComponentInChildren<TextMeshProUGUI>().text = settings.keyImportant.ToString();
        }

        /// <summary>
        /// 指定されたコールバックに対して次に押されたキーを返すコルーチン。
        /// </summary>
        private IEnumerator WaitForKeyPress(Action<KeyCode> onKeySelected)
        {
            if (onKeySelected == null)
            {
                yield break;
            }

            // 一番簡単な手法として全 KeyCode をループして押下を検出する
            while (true)
            {
                foreach (KeyCode kc in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(kc))
                    {
                        onKeySelected(kc);
                        yield break;
                    }
                }
                yield return null;
            }
        }
        
        #endregion
    }
}