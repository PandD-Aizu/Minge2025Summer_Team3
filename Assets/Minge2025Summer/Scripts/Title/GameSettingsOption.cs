using GameSetting;
using UnityEngine;
using UnityEngine.UI;

namespace Title
{
    public class GameSettingsOption : MonoBehaviour
    {
        [SerializeField] private Button aimAssistButton;
        [SerializeField] private Button tutorialEnabledButton;
        [SerializeField] private Button displayHUDButton;
        [SerializeField] private Button displayCrosshairButton;
        [SerializeField] private InputField crossHairColorInputFieldR;
        [SerializeField] private InputField crossHairColorInputFieldG;
        [SerializeField] private InputField crossHairColorInputFieldB;
        
        private bool isAimAssist;
        private bool isTutorialEnabled;
        private bool isDisplayHUD;
        private bool isDisplayCrosshair;
        private Color crossHairColor;

        private void Start()
        {
            isAimAssist = GameController.Instance.gameSettingsController.gameSettings.aimAssist;
            isTutorialEnabled = GameController.Instance.gameSettingsController.gameSettings.tutorialEnabled;
            isDisplayHUD = GameController.Instance.gameSettingsController.gameSettings.displayHUD;
            isDisplayCrosshair = GameController.Instance.gameSettingsController.gameSettings.displayCrosshair;
            crossHairColor = GameController.Instance.gameSettingsController.gameSettings.crosshairColor;
            
            aimAssistButton.GetComponentInChildren<SpriteRenderer>().enabled = isAimAssist;
            tutorialEnabledButton.GetComponentInChildren<SpriteRenderer>().enabled = isTutorialEnabled;
            displayHUDButton.GetComponentInChildren<SpriteRenderer>().enabled = isDisplayHUD;
            displayCrosshairButton.GetComponentInChildren<SpriteRenderer>().enabled = isDisplayCrosshair;
            //crossHairColorInputField.text = crosshairColor;
            
            aimAssistButton.onClick.AddListener(() =>
            {
                isAimAssist = !isAimAssist;
                aimAssistButton.GetComponentInChildren<SpriteRenderer>().enabled = isAimAssist;
                GameController.Instance.gameSettingsController.gameSettings.aimAssist = isAimAssist;
            });
            
            tutorialEnabledButton.onClick.AddListener(() =>
            {
                isTutorialEnabled = !isTutorialEnabled;
                tutorialEnabledButton.GetComponentInChildren<SpriteRenderer>().enabled = isTutorialEnabled;
                GameController.Instance.gameSettingsController.gameSettings.tutorialEnabled = isTutorialEnabled;
            });
            
            displayHUDButton.onClick.AddListener(() =>
            {
                isDisplayHUD = !isDisplayHUD;
                displayHUDButton.GetComponentInChildren<SpriteRenderer>().enabled = isDisplayHUD;
                GameController.Instance.gameSettingsController.gameSettings.displayHUD = isDisplayHUD;
            });
            
            displayCrosshairButton.onClick.AddListener(() =>
            {
                isDisplayCrosshair = !isDisplayCrosshair;
                displayCrosshairButton.GetComponentInChildren<SpriteRenderer>().enabled = isDisplayCrosshair;
                GameController.Instance.gameSettingsController.gameSettings.displayCrosshair = isDisplayCrosshair;
            });
            
            crossHairColorInputFieldR.text = ((int)(crossHairColor.r * 255)).ToString();
            crossHairColorInputFieldG.text = ((int)(crossHairColor.g * 255)).ToString();
            crossHairColorInputFieldB.text = ((int)(crossHairColor.b * 255)).ToString();
            crossHairColorInputFieldR.onEndEdit.AddListener(value =>
            {
                if (int.TryParse(value, out int r))
                {
                    r = Mathf.Clamp(r, 0, 255);
                    crossHairColor.r = r / 255f;
                    GameController.Instance.gameSettingsController.gameSettings.crosshairColor = crossHairColor;
                }
            });
            crossHairColorInputFieldG.onEndEdit.AddListener(value =>
            {
                if (int.TryParse(value, out int g))
                {
                    g = Mathf.Clamp(g, 0, 255);
                    crossHairColor.g = g / 255f;
                    GameController.Instance.gameSettingsController.gameSettings.crosshairColor = crossHairColor;
                }
            });
            crossHairColorInputFieldB.onEndEdit.AddListener(value =>
            {
                if (int.TryParse(value, out int b))
                {
                    b = Mathf.Clamp(b, 0, 255);
                    crossHairColor.b = b / 255f;
                    GameController.Instance.gameSettingsController.gameSettings.crosshairColor = crossHairColor;
                }
            });
        }
    }
}