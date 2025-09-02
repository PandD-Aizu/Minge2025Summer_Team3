using GameSetting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Title
{
    public class ControlOptionController : MonoBehaviour
    {
        [SerializeField] private Button mouseCursorLockButton;
        [SerializeField] private Button mouseInvertButton;
        [SerializeField] private Button mouseWheelInvertButton;
        [SerializeField] private Button keyMoveForward;
        [SerializeField] private Button keyMoveBackward;
        [SerializeField] private Button keyMoveLeft;
        [SerializeField] private Button keyMoveRight;
        [SerializeField] private Button keyReload;
        [SerializeField] private Button keyInteract;
        [SerializeField] private Button keySprint;

        private bool isMouseCursorLock;
        private bool isMouseInvert;
        private bool isMouseWheelInvert;
        private KeyCode moveForwardKey;
        private KeyCode moveBackwardKey;
        private KeyCode moveLeftKey;
        private KeyCode moveRightKey;
        private KeyCode reloadKey;
        private KeyCode interactKey;
        private KeyCode sprintKey;

        private bool isWaitingForKey = false;
        private System.Action<KeyCode> onKeyReceived;
        private Button waitingButton;
        private string originalButtonText;
        
        private void Start()
        {
            isMouseCursorLock = GameController.Instance.gameSettingsController.gameSettings.mouseCursorLock;
            isMouseInvert = GameController.Instance.gameSettingsController.gameSettings.mouseInvert;
            isMouseWheelInvert = GameController.Instance.gameSettingsController.gameSettings.mouseWheelInvert;
            moveForwardKey = GameController.Instance.gameSettingsController.gameSettings.keyMoveForward;
            moveBackwardKey = GameController.Instance.gameSettingsController.gameSettings.keyMoveBackward;
            moveLeftKey = GameController.Instance.gameSettingsController.gameSettings.keyMoveLeft;
            moveRightKey = GameController.Instance.gameSettingsController.gameSettings.keyMoveRight;
            reloadKey = GameController.Instance.gameSettingsController.gameSettings.keyReload;
            interactKey = GameController.Instance.gameSettingsController.gameSettings.keyInteract;
            sprintKey = GameController.Instance.gameSettingsController.gameSettings.keySprint;
            
            mouseCursorLockButton.GetComponentInChildren<SpriteRenderer>().enabled = isMouseCursorLock;
            mouseInvertButton.GetComponentInChildren<SpriteRenderer>().enabled = isMouseInvert;
            mouseWheelInvertButton.GetComponentInChildren<SpriteRenderer>().enabled = isMouseWheelInvert;
            keyMoveForward.GetComponentInChildren<TextMeshProUGUI>().text = moveForwardKey.ToString();
            keyMoveBackward.GetComponentInChildren<TextMeshProUGUI>().text = moveBackwardKey.ToString();
            keyMoveLeft.GetComponentInChildren<TextMeshProUGUI>().text = moveLeftKey.ToString();
            keyMoveRight.GetComponentInChildren<TextMeshProUGUI>().text = moveRightKey.ToString();
            keyReload.GetComponentInChildren<TextMeshProUGUI>().text = reloadKey.ToString();
            keyInteract.GetComponentInChildren<TextMeshProUGUI>().text = interactKey.ToString();
            keySprint.GetComponentInChildren<TextMeshProUGUI>().text = sprintKey.ToString();
            
            mouseCursorLockButton.onClick.AddListener(OnClickMouseCursorLock);
            mouseInvertButton.onClick.AddListener(OnClickMouseInvert);
            mouseWheelInvertButton.onClick.AddListener(OnClickMouseWheelInvert);
            keyMoveForward.onClick.AddListener(OnClickKeyMoveForward);
            keyMoveBackward.onClick.AddListener(OnClickKeyMoveBackward);
            keyMoveLeft.onClick.AddListener(OnClickKeyMoveLeft);
            keyMoveRight.onClick.AddListener(OnClickKeyMoveRight);
            keyReload.onClick.AddListener(OnClickKeyReload);
            keyInteract.onClick.AddListener(OnClickKeyInteract);
            keySprint.onClick.AddListener(OnClickKeySprint);
        }

        private void Update()
        {
            if (isWaitingForKey)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    isWaitingForKey = false;
                    onKeyReceived = null;
                    
                    if (waitingButton != null)
                    {
                        var text = waitingButton.GetComponentInChildren<TextMeshProUGUI>();
                        if (text != null) 
                            text.text = originalButtonText;
                        
                        waitingButton = null;
                    }
                    
                    return;
                }

                foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        isWaitingForKey = false;
                        onKeyReceived?.Invoke(keyCode);
                        waitingButton = null;
                        break;
                    }
                }
            }
        }

        private void WaitForKeyInput(System.Action<KeyCode> callback, Button button)
        {
            isWaitingForKey = true;
            onKeyReceived = (keyCode) =>
            {
                var text = button.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    // 入力完了時に新しいキー名を表示
                    text.text = keyCode.ToString();
                }
                callback(keyCode);
            };

            waitingButton = button;

            // 元テキストを保存し、テキストを変更
            var tmp = button.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
            {
                originalButtonText = tmp.text;
                tmp.text = "キー入力待ち...";
            }
        }
        
        private void OnClickMouseCursorLock()
        {
            if (isMouseCursorLock)
            {
                isMouseCursorLock = false;
                mouseCursorLockButton.GetComponentInChildren<SpriteRenderer>().enabled = false;
            }
            else
            {
                isMouseCursorLock = true;
                mouseCursorLockButton.GetComponentInChildren<SpriteRenderer>().enabled = true;
            }
            
            GameController.Instance.gameSettingsController.gameSettings.mouseCursorLock = isMouseCursorLock;
        }
        
        private void OnClickMouseInvert()
        {
            if (isMouseInvert)
            {
                isMouseInvert = false;
                mouseInvertButton.GetComponentInChildren<SpriteRenderer>().enabled = false;
            }
            else
            {
                isMouseInvert = true;
                mouseInvertButton.GetComponentInChildren<SpriteRenderer>().enabled = true;
            }
            
            GameController.Instance.gameSettingsController.gameSettings.mouseInvert = isMouseInvert;
        }
        
        private void OnClickMouseWheelInvert()
        {
            if (isMouseWheelInvert)
            {
                isMouseWheelInvert = false;
                mouseWheelInvertButton.GetComponentInChildren<SpriteRenderer>().enabled = false;
            }
            else
            {
                isMouseWheelInvert = true;
                mouseWheelInvertButton.GetComponentInChildren<SpriteRenderer>().enabled = true;
            }
            
            GameController.Instance.gameSettingsController.gameSettings.mouseWheelInvert = isMouseWheelInvert;
        }

        private void OnClickKeyMoveForward()
        {
            WaitForKeyInput((keyCode) =>
            {
                moveForwardKey = keyCode;
                GameController.Instance.gameSettingsController.gameSettings.keyMoveForward = moveForwardKey;
            }, keyMoveForward);
        }
        
        private void OnClickKeyMoveBackward()
        {
            WaitForKeyInput((keyCode) =>
            {
                moveBackwardKey = keyCode;
                GameController.Instance.gameSettingsController.gameSettings.keyMoveBackward = moveBackwardKey;
            }, keyMoveBackward);
        }
        
        private void OnClickKeyMoveLeft()
        {
            WaitForKeyInput((keyCode) =>
            {
                moveLeftKey = keyCode;
                GameController.Instance.gameSettingsController.gameSettings.keyMoveLeft = moveLeftKey;
            }, keyMoveLeft);
        }
        
        private void OnClickKeyMoveRight()
        {
            WaitForKeyInput((keyCode) =>
            {
                moveRightKey = keyCode;
                GameController.Instance.gameSettingsController.gameSettings.keyMoveRight = moveRightKey;
            }, keyMoveRight);
        }
        
        private void OnClickKeyReload()
        {
            WaitForKeyInput((keyCode) =>
            {
                reloadKey = keyCode;
                GameController.Instance.gameSettingsController.gameSettings.keyReload = reloadKey;
            }, keyReload);
        }
        
        private void OnClickKeyInteract()
        {
            WaitForKeyInput((keyCode) =>
            {
                interactKey = keyCode;
                GameController.Instance.gameSettingsController.gameSettings.keyInteract = interactKey;
            }, keyInteract);
        }
        
        private void OnClickKeySprint()
        {
            WaitForKeyInput((keyCode) =>
            {
                sprintKey = keyCode;
                GameController.Instance.gameSettingsController.gameSettings.keySprint = sprintKey;
            }, keySprint);
        }
    }
}