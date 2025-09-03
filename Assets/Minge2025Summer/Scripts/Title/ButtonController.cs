using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace Title
{
    public class ButtonController : MonoBehaviour
    {
        [Header("STARTボタン")]
        [SerializeField] private string gameSceneAddress;

        [Header("OPTIONボタン")] 
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
        }
        
        public void StartGame()
        {
            AsyncOperationHandle handle = Addressables.LoadSceneAsync(gameSceneAddress, LoadSceneMode.Single);
            handle.Completed += OnSceneLoaded;
        }

        public void OpenOptions()
        {
            optionPanel.SetActive(true);
        }

        public void CloseOptions()
        {
            optionPanel.SetActive(false);
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        private void OnSceneLoaded(AsyncOperationHandle handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
                Debug.Log("Load Complete");
            else 
                Debug.LogError("Load Failed");
        }
        
        /* オプションのボタン関数 */
        public void AlignControlOption()
        {
            ShowOptionObject(controlOptionObject);
        }

        public void AlignCameraOption()
        {
            ShowOptionObject(cameraOptionObject);
        }
        
        public void AlignGameSettingOption()
        {
            ShowOptionObject(gameSettingOptionObject);
        }
        
        public void AlignGraphicOption()
        {
            ShowOptionObject(graphicOptionObject);
        }
        
        public void AlignAudioOption()
        {
            ShowOptionObject(audioOptionObject);
        }
        
        public void AlignLanguageOption()
        {
            ShowOptionObject(languageOptionObject);
        }
        
        public void AlignAccessibilityOption()
        {
            ShowOptionObject(accessibilityOptionObject);
        }

        private void ShowOptionObject(GameObject targetObject)
        {
            currentOptionObject?.SetActive(false);
            targetObject.SetActive(true);
            currentOptionObject = targetObject;
        }
    }
}