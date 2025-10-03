using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Workspace.koto_thing
{
    public class GameOverView : MonoBehaviour
    {
        [Header("ゲームオーバー時のUI")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Image gameOverBackground;

        [Header("バックグラウンドのフェード時間")] 
        [SerializeField] private float fadeDuration = 1.0f;

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            gameOverPanel.SetActive(false);
            gameOverBackground.color = new Color(0, 0, 0, 0);
        }
        
        /// <summary>
        /// ゲームオーバー時の画面を表示
        /// </summary>
        public void ShowGameOverPanel()
        {
            gameOverPanel.SetActive(true);
            gameOverBackground.DOFade(0.8f, fadeDuration);
        }

        /// <summary>
        /// ゲームオーバー時の画面を非表示
        /// </summary>
        public void HideGameOverPanel()
        {
            gameOverBackground.DOFade(0.0f, fadeDuration)
                .OnComplete(() => gameOverPanel.SetActive(false));
        }
    }
}