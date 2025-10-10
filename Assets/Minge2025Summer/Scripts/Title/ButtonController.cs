using UnityEngine;
using DG.Tweening;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Title
{
    public class ButtonController : MonoBehaviour
    {
        [Header("依存関係")] 
        [SerializeField, Tooltip("タイトルのフェードコントローラ")] private FadeController titleFadeController;
        [SerializeField, Tooltip("オプションのフェードコントローラ")] private FadeController optionFadeController;
        
        [Header("STARTボタン")] 
        [SerializeField] private string gameSceneAddress;
        [SerializeField, Tooltip("画面全体を覆う画像")] private Image screenBackgroundImage;
        [SerializeField, Tooltip("フェード秒数")] private float fadeDuration = 0.5f;
        
        [Header("パネル")] 
        [SerializeField] private GameObject titlePanel;
        [SerializeField] private GameObject optionPanel;

        [Header("オプションに表示するオブジェクトグループ")] 
        [SerializeField] private GameObject controlOptionObject;
        [SerializeField] private GameObject cameraOptionObject;
        [SerializeField] private GameObject gameSettingOptionObject;
        [SerializeField] private GameObject graphicOptionObject;
        [SerializeField] private GameObject audioOptionObject;
        [SerializeField] private GameObject languageOptionObject;
        [SerializeField] private GameObject accessibilityOptionObject;

        private GameObject currentOptionObject;

        private void Start()
        {
            currentOptionObject = controlOptionObject;
            optionPanel.SetActive(false);
            screenBackgroundImage.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// STARTボタンを押したときの処理
        /// </summary>
        public void StartGame()
        {
            screenBackgroundImage.gameObject.SetActive(true);
            var c = screenBackgroundImage.color;
            c.a = 0f;
            screenBackgroundImage.color = c;
            var target = c; target.a = 1f;
            DOTween.To(
                () => screenBackgroundImage.color,
                col => screenBackgroundImage.color = col,
                target,
                fadeDuration
            ).OnComplete(() =>
            {
                AsyncOperationHandle handle = Addressables.LoadSceneAsync(gameSceneAddress, LoadSceneMode.Single);
                handle.Completed += OnSceneLoaded;
            });
        }

        /// <summary>
        /// オプション画面を開く
        /// </summary>
        public void OpenOptions()
        {
            optionPanel.SetActive(true);
            titlePanel.SetActive(false);
            optionFadeController.Play();
        }

        /// <summary>
        /// オプション画面を閉じる
        /// </summary>
        public void CloseOptions()
        {
            optionPanel.SetActive(false);
            titlePanel.SetActive(true);
            titleFadeController.Play();
        }

        /// <summary>
        /// ゲームを終了する
        /// </summary>
        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        ///<summary>
        /// シーンをロードしたときの処理
        /// </summary>> 
        private void OnSceneLoaded(AsyncOperationHandle handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
                Debug.Log("Load Complete");
            else 
                Debug.LogError("Load Failed");
        }
        
        /* オプションのボタン関数 */
        public void AlignControlOption() => ShowOptionObject(controlOptionObject);
        public void AlignCameraOption() => ShowOptionObject(cameraOptionObject);
        public void AlignGameSettingOption() => ShowOptionObject(gameSettingOptionObject);
        public void AlignGraphicOption() => ShowOptionObject(graphicOptionObject);
        public void AlignAudioOption() => ShowOptionObject(audioOptionObject);
        public void AlignLanguageOption() => ShowOptionObject(languageOptionObject);
        public void AlignAccessibilityOption() => ShowOptionObject(accessibilityOptionObject);

        ///<summary>
        /// オプションボタンの指定パネルを切り替え
        /// </summary>> 
        private void ShowOptionObject(GameObject targetObject)
        {
            currentOptionObject?.SetActive(false);
            targetObject.SetActive(true);
            currentOptionObject = targetObject;
        }
    }
}