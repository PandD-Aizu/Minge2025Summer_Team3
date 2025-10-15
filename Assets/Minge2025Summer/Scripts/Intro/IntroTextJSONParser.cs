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
                    var data = JsonUtility.FromJson<IntroTextData>(op.Result.text);
                    onComplete?.Invoke(new List<IntroTextEntry>(data.entries));
                }
                else
                {
                    Debug.LogError("JSONロード失敗");
                    onComplete?.Invoke(new List<IntroTextEntry>());
                }
            };
        }
    }
}