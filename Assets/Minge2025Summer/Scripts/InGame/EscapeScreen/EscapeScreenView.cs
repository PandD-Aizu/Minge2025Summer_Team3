using System.Collections.Generic;
using DG.Tweening;
using Minge2025Summer.Scripts.InGame.EscapeScreen.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Minge2025Summer.Scripts.InGame.EscapeScreen
{
    public class EscapeScreenView : MonoBehaviour
    {
        [Header("パネル")] 
        [SerializeField] private GameObject escapeScreen;
        [SerializeField] private GameObject confirmQuitPanel;

        [Header("ボタン")] 
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button optionButton;
        [SerializeField] private Button backToTitleButton;
        [SerializeField] private Button quitGameButton;
        [SerializeField] private Button applyButton;
        [SerializeField] private Button cancelButton;

        private ConfirmState currentConfirmState;
        
        public Button ResumeButton => resumeButton;
        public Button OptionButton => optionButton;
        public Button BackToTitleButton => backToTitleButton;
        public Button QuitGameButton => quitGameButton;
        public Button ApplyButton => applyButton;
        public Button CancelButton => cancelButton;

        /// <summary>
        /// メニュー画面の表示・非表示切り替え
        /// </summary>
        public void ToggleEscapeScreen()
        {
            if (escapeScreen.activeSelf)
            {
                List<Graphic> uiElements = new List<Graphic>(escapeScreen.GetComponentsInChildren<Graphic>(true));
                foreach (var uiElement in uiElements)
                {
                    uiElement.DOFade(0f, 0.2f).OnComplete(() =>
                    {
                        escapeScreen.SetActive(false);
                    });
                }
                
                HideConfirmQuit();
            }
            else
            {
                List<Graphic> uiElements = new List<Graphic>(escapeScreen.GetComponentsInChildren<Graphic>(true));
                escapeScreen.SetActive(true);
                foreach (var uiElement in uiElements)
                {
                    uiElement.DOFade(1f, 0.2f);
                }
            }
        }

        /// <summary>
        /// 確認画面の表示・非表示切り替え
        /// </summary>
        /// <param name="message">表示させるテキスト</param>
        public void ShowConfirmQuit(string message, ConfirmState confirmState)
        {
            if (!confirmQuitPanel.activeSelf)
            {
                List<Graphic> uiElements = new List<Graphic>(confirmQuitPanel.GetComponentsInChildren<Graphic>(true));
                foreach (var uiElement in uiElements)
                {
                    uiElement.DOFade(1f, 0.2f);
                    confirmQuitPanel.SetActive(true);

                    uiElements[0].TryGetComponent<TextMeshProUGUI>(out var component);
                    if (component != null)
                        component.text = message;
                }
                
                currentConfirmState = confirmState;
                ChangeApplyListener(currentConfirmState);
            }
        }
        
        /// <summary>
        /// 確認画面を非表示にする
        /// </summary>
        public void HideConfirmQuit()
        {
            if (confirmQuitPanel.activeSelf)
            {
                List<Graphic> uiElements = new List<Graphic>(confirmQuitPanel.GetComponentsInChildren<Graphic>(true));
                foreach (var uiElement in uiElements)
                {
                    uiElement.DOFade(0f, 0.2f).OnComplete(() =>
                    {
                        confirmQuitPanel.SetActive(false);
                    });
                }
            }
        }

        /// <summary>
        /// リスナーを変更する
        /// </summary>
        /// <param name="state"></param>
        private void ChangeApplyListener(ConfirmState state)
        {
            switch (state)
            {
                case ConfirmState.BACK_TO_TITLE:
                    applyButton.onClick.RemoveAllListeners();
                    applyButton.onClick.AddListener(() =>
                    {
                        Addressables.LoadSceneAsync("TitleScene").Completed += _ =>
                        {
                            // シーン移動後の処理があればここに追加
                        };
                    });
                    break;
                
                case ConfirmState.QUIT_GAME:
                    applyButton.onClick.RemoveAllListeners();
                    applyButton.onClick.AddListener(() =>
                    {
                        #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
                        #else
                        Application.Quit();
                        #endif
                    });
                    break;
            }
        }
    }
}