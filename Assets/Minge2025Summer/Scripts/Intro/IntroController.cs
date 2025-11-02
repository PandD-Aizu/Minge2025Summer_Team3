using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Minge2025Summer.Scripts.Intro
{
    public class IntroController : MonoBehaviour
    {
        [Header("イントロを表示させるメインのテキスト")]
        [SerializeField] private TextMeshProUGUI introText;

        [Header("イントロ情報が入ったJSONファイルのアドレス")] 
        [SerializeField] private string jsonAddress;

        [Header("ロード関係")] 
        [SerializeField] private GameObject panel;
        [SerializeField, Tooltip("ロード先のアドレス")] private string inGameSceneAddress;
        [SerializeField, Tooltip("右下のプログレスバー")] private UnityEngine.UI.Slider progressBar;

        private void Start()
        {
            IntroTextJSONParser parser = new IntroTextJSONParser(); 
            
            parser.ParseAsync(jsonAddress, entries =>
            {
                StartCoroutine(ShowIntroSequence(entries));
            });
        }

        /// <summary>
        /// イントロシーケンスを表示するコルーチン
        /// </summary>
        /// <param name="entries">表示するエントリ</param>
        /// <returns></returns>
        private IEnumerator ShowIntroSequence(List<IntroTextJSONParser.IntroTextEntry> entries)
        {
            introText.alpha = 0.0f;

            foreach (var entry in entries)
            {
                introText.text = entry.text;
                yield return introText.DOFade(1.0f, 0.5f).WaitForCompletion();
                yield return StartCoroutine(WaitforClick());
                //yield return new WaitForSeconds(entry.time);
                yield return introText.DOFade(0.0f, 0.5f).WaitForCompletion();
            }

            yield return LoadInGameSceneWithAddressables();
        }

        private IEnumerator LoadInGameSceneWithAddressables()
        {
            if (panel != null) 
                panel.SetActive(true);

            if (progressBar != null)
            {
                progressBar.minValue = 0f;
                progressBar.maxValue = 1f;
                progressBar.value = 0f;
            }

            AsyncOperationHandle<SceneInstance> handle =
                Addressables.LoadSceneAsync(inGameSceneAddress, LoadSceneMode.Single, true);

            while (!handle.IsDone)
            {
                if (progressBar != null)
                {
                    progressBar.value = handle.PercentComplete; // 0.0f ～ 1.0f
                }
                yield return null;
            }
        }

        private IEnumerator WaitforClick()
        {
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }
    }
}