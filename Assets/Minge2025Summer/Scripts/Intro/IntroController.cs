using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Main.Intro
{
    public class IntroController : MonoBehaviour
    {
        [Header("イントロを表示させるメインのテキスト")]
        [SerializeField] private TextMeshProUGUI introText;

        [Header("イントロ情報が入ったJSONファイルのアドレス")] 
        [SerializeField] private string jsonAddress;

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
                yield return new WaitForSeconds(entry.time);
                yield return introText.DOFade(0.0f, 0.5f).WaitForCompletion();
            }
        }
    }
}