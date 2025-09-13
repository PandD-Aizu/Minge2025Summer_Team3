using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneController
{
    private static AsyncOperation currentAsyncOperation;
    public static AsyncOperation GetCurrentAsyncOperation => currentAsyncOperation;
    
    /// <summary>
    /// 同期的にシーンをロード
    /// </summary>
    /// <param name="sceneName">ロードするシーンの名前</param>
    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 非同期でシーンをロード
    /// </summary>
    /// <param name="sceneName">ロードするシーンの名前</param>
    public static void LoadSceneAsync(string sceneName)
    {
        currentAsyncOperation = SceneManager.LoadSceneAsync(sceneName);
    }
    
    /// <summary>
    /// 非同期でシーンをアンロード
    /// </summary>
    /// <param name="sceneName">アンロードするシーンの名前</param>
    public static void UnLoadSceneAsync(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }
}