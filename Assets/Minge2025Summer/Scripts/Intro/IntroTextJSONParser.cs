using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Minge2025Summer.Scripts.Intro
{
    public class IntroTextJSONParser
    {
        [Serializable]
        public class IntroTextEntry
        {
            public float time;
            public string text;
        }

        [Serializable]
        public class IntroTextData
        {
            public IntroTextEntry[] entries;
        }

        /// <summary>
        /// JSONファイルを非同期で読み込み、パースする
        /// </summary>
        /// <param name="jsonAddress">読み込むJSONのアドレス</param>
        /// <param name="onComplete"></param>
        public void ParseAsync(string jsonAddress, Action<List<IntroTextEntry>> onComplete)
        {
            Addressables.LoadAssetAsync<TextAsset>(jsonAddress).Completed += op =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    var textAsset = op.Result;
                    if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
                    {
                        Debug.LogError($"[IntroTextJSONParser] TextAsset is null or empty: {jsonAddress}");
                        onComplete?.Invoke(new List<IntroTextEntry>());
                        return;
                    }

                    IntroTextData data = null;
                    try
                    {
                        // パース前の生 JSON をログ出力して確認
                        Debug.Log($"[IntroTextJSONParser] Raw JSON:\n{textAsset.text}");
    
                        data = JsonUtility.FromJson<IntroTextData>(textAsset.text);
    
                        // パース後の data の状態を確認
                        Debug.Log($"[IntroTextJSONParser] data is null: {data == null}");
                        if (data != null)
                        {
                            Debug.Log($"[IntroTextJSONParser] data.entries is null: {data.entries == null}");
                            if (data.entries != null)
                            {
                                Debug.Log($"[IntroTextJSONParser] entries.Length: {data.entries.Length}");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[IntroTextJSONParser] JSON parse exception: {e.Message}");
                        onComplete?.Invoke(new List<IntroTextEntry>());
                        return;
                    }

                    if (data == null || data.entries == null || data.entries.Length == 0)
                    {
                        Debug.LogWarning($"[IntroTextJSONParser] Parsed entries are null or empty: {jsonAddress}");
                        onComplete?.Invoke(new List<IntroTextEntry>());
                        return;
                    }

                    onComplete?.Invoke(new List<IntroTextEntry>(data.entries));
                }
                else
                {
                    Debug.LogError($"[IntroTextJSONParser] JSONロード失敗: {jsonAddress}");
                    onComplete?.Invoke(new List<IntroTextEntry>());
                }
            };
        }
    }
}
