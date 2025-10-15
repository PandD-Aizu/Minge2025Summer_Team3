using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Minge2025Summer.Scripts.InGame.DocumentScript
{
    public class DocumentSource : MonoBehaviour
    {
        [Header("JSON割り当て")] 
        [SerializeField] private TextAsset json;

        [Header("Addressable割り当て")] 
        [SerializeField] private AssetReference jsonReference;

        private DocumentData cached;
        private AsyncOperationHandle<TextAsset>? handle;

        public DocumentData GetData()
        {
            if (cached != null)
                return cached;

            if (json != null)
                cached = JsonUtility.FromJson<DocumentData>(json.text);

            return cached;
        }

        public async Task<DocumentData> LoadDataAsync(CancellationToken token = default)
        {
            if (cached != null)
                return cached;

            if (json != null)
            {
                cached = JsonUtility.FromJson<DocumentData>(json.text);
                return cached;
            }

            if (jsonReference != null && jsonReference.RuntimeKeyIsValid())
            {
                handle = jsonReference.LoadAssetAsync<TextAsset>();
                while (!handle.Value.IsDone)
                {
                    if (token.IsCancellationRequested)
                        break;
                    
                    await Task.Yield();
                }
                
                if (handle.Value.Status == AsyncOperationStatus.Succeeded && handle.Value.Result != null)
                    cached = JsonUtility.FromJson<DocumentData>(handle.Value.Result.text);
            }

            return cached;
        }

        private void OnDestroy()
        {
            if (handle.HasValue && handle.Value.IsValid())
                Addressables.Release(handle.Value);
        }
    }
}