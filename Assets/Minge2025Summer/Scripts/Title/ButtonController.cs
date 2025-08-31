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
        
        public void StartGame()
        {
            AsyncOperationHandle handle = Addressables.LoadSceneAsync(gameSceneAddress, LoadSceneMode.Single);
            handle.Completed += OnSceneLoaded;
        }

        public void OpenOptions()
        {
            
        }

        public void CloseOptions()
        {
            
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
    }
}