using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Minge2025Summer.Main.InGame
{
    public class ObjectiveTextModel : MonoBehaviour
    {
        [Serializable]
        private class JsonSchema
        {
            public string title;
            public string subTitle;
            public string body;
            public string illustrationSpriteAddress; 
        }
        
        private readonly Dictionary<string, ObjectiveTextData> cache = new();

        private readonly Subject<ObjectiveTextData> showSubject = new();
        public IObservable<ObjectiveTextData> OnShow => showSubject;

        [SerializeField, Tooltip("同一アドレスの再リクエスト時にキャッシュを使うか")]
        private bool enableCache = true;

        /// <summary>
        /// JSON アドレスを指定して表示データを読み込み要求。
        /// </summary>
        /// <param name="jsonAddress">JSONのAddressablesアドレス</param>
        public void RequestShow(string jsonAddress)
        {
            if (string.IsNullOrWhiteSpace(jsonAddress))
                return;

            // キャッシュが有効でキャッシュに存在するならそれを使う
            if (enableCache && cache.TryGetValue(jsonAddress, out var cached))
            {
                showSubject.OnNext(cached);
                return;
            }

            // JSONをAddressablesから非同期ロード
            Addressables.LoadAssetAsync<TextAsset>(jsonAddress).Completed += OnJsonLoaded;

            // ロード完了コールバック
            void OnJsonLoaded(AsyncOperationHandle<TextAsset> op)
            {
                // ロード失敗
                if (op.Status != AsyncOperationStatus.Succeeded || op.Result == null)
                {
                    Debug.LogError($"InformationScreenModel: JSON load failed: {jsonAddress}");
                    return;
                }

                // JSONパース
                JsonSchema schema;
                try
                {
                    schema = JsonUtility.FromJson<JsonSchema>(op.Result.text);
                }
                catch (Exception e)
                {
                    Debug.LogError($"InformationScreenModel: JSON parse error: {e}\n{op.Result.text}");
                    return;
                }
                
                var bullets = schema.subTitle != null ? new string(schema.body) : new string("");

                // イラストが指定されていればそれもロード
                if (!string.IsNullOrWhiteSpace(schema.illustrationSpriteAddress))
                {
                    Addressables.LoadAssetAsync<Sprite>(schema.illustrationSpriteAddress).Completed += handle =>
                    {
                        Sprite sprite = handle.Status == AsyncOperationStatus.Succeeded ? handle.Result : null;
                        var data = new ObjectiveTextData(schema.title, schema.body, bullets, sprite);
                        if (enableCache) cache[jsonAddress] = data;
                        showSubject.OnNext(data);
                    };
                }
                else
                {
                    var data = new ObjectiveTextData(schema.title, schema.body, bullets, null);
                    if (enableCache) cache[jsonAddress] = data;
                    showSubject.OnNext(data);
                }
            }
        }
    }
}

