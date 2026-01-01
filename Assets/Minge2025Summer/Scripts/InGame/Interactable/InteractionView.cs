using TMPro;
using UnityEngine;

namespace Minge2025Summer.InGame.Interactable
{
    public class InteractionView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject uiContainer;
        [SerializeField] private TextMeshProUGUI promptText;

        private void Awake()
        {
            // 初期状態は非表示
            Hide();
        }

        public void Show(string prompt)
        {
            if (uiContainer != null) uiContainer.SetActive(true);
            if (promptText != null) promptText.text = prompt;
        }

        public void Hide()
        {
            if (uiContainer != null) uiContainer.SetActive(false);
        }
    }
}
