using UnityEngine;
using UnityEngine.UI;

namespace Workspace.koto_thing
{
    public class GameOverButtonController : MonoBehaviour
    {
        [Header("依存")]
        [SerializeField] private GameOverModel gameOverModel;
        
        [Header("ボタンUI")]
        [SerializeField] private Button continueButton;
        [SerializeField] private Button backToTitleButton;

        private void Start()
        {
            continueButton.onClick.AddListener(SubscribeContinueEvent);
            backToTitleButton.onClick.AddListener(SubscribeBackToTitleEvent);
        }

        /// <summary>
        /// コンティニューボタンを押したときの処理
        /// </summary>
        private void SubscribeContinueEvent()
        {
            gameOverModel.ContinueGame();
        }

        /// <summary>
        /// タイトル画面に戻るボタンを押したときの処理
        /// </summary>
        private void SubscribeBackToTitleEvent()
        {
            gameOverModel.LoadTitleScene();
        }
    }
}