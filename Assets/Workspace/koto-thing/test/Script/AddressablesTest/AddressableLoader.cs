using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Test
{
    public class AddressableLoader : MonoBehaviour
    {
        [SerializeField] private string assetAddress;

        private GameObject loadedAsset;

        private void Start()
        {
            Addressables.LoadAssetAsync<GameObject>(assetAddress)
                .Completed += OnAssetLoaded;
        }

        private void OnAssetLoaded(AsyncOperationHandle<GameObject> handle)
        {
            // ロードが完了したかチェックする
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                loadedAsset = Instantiate(handle.Result);
                Debug.Log("アセットの読み込みに成功しました: " + assetAddress);
            }
            else
            {
                Debug.LogError("アセットの読み込みに失敗しました: " + assetAddress);
            }
        }

        private void OnDestroy()
        {
            if (loadedAsset != null)
            {
                Addressables.ReleaseInstance(loadedAsset);
                Debug.Log("インスタンスを開放しました: " + assetAddress);
            }
        }
    }
}