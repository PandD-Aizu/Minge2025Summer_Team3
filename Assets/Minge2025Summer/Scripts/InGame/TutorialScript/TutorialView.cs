using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if TMP_PRESENT
using TMPro;
#endif

namespace Minge2025Summer.Main.InGame
{
    /// <summary>
    /// チュートリアルテキストのフェード表示/非表示を担うビュー。
    /// </summary>
    public class TutorialView : MonoBehaviour
    {
        [Header("UIの参照")]
        [SerializeField] private TextMeshProUGUI tutorialText;
        [SerializeField, Tooltip("背景など一緒にフェードさせたいGraphic群")] private List<Graphic> additionalGraphics = new();

        [Header("フェード設定")]
        [SerializeField] private float fadeInDuration = 0.35f;
        [SerializeField] private float fadeOutDuration = 0.25f;
        [SerializeField] private Ease fadeEase = Ease.OutQuad;

        private Sequence currentSequence;
        private bool isVisible;

        /// <summary>
        /// テキストを設定しフェードイン表示
        /// </summary>
        /// <param name="text">表示するテキスト</param>
        public void Show(string text)
        {
            if (tutorialText == null)
                return;
            
            // 動作しているシーケンスを停止
            KillSequence();
            tutorialText.text = text;
            
            // 即座に透明化してからフェードイン
            PrepareAlpha(0f);
            
            currentSequence = DOTween.Sequence().SetUpdate(true); // 時間停止中も動作するように
            AppendFade(1f, fadeInDuration);                 // フェードイン追加
            currentSequence.OnComplete(() => isVisible = true);   // 完了時に表示中フラグを立てる
        }

        /// <summary>
        /// 現在表示中のテキストをフェードアウト
        /// </summary>
        public void Hide()
        {
            if (!isVisible) 
                return;
            
            // 動作しているシーケンスを停止
            KillSequence();
            
            currentSequence = DOTween.Sequence().SetUpdate(true); // 時間停止中も動作するように
            AppendFade(0f, fadeOutDuration);                // フェードアウト追加
            currentSequence.OnComplete(() => isVisible = false);  // 完了時に非表示フラグを立てる
        }
        
        /*以下ヘルパー関数*/
        private void PrepareAlpha(float a)
        {
            if (tutorialText is Graphic tg)
            {
                var c = tg.color; c.a = a; tg.color = c;
            }
            foreach (var g in additionalGraphics)
            {
                if (g == null) continue;
                var c = g.color; c.a = a; g.color = c;
            }
        }

        private void AppendFade(float target, float duration)
        {
            if (tutorialText is Graphic tg)
            {
                currentSequence.Join(CreateFadeTween(tg, target, duration));
            }
            foreach (var g in additionalGraphics)
            {
                if (g == null) continue;
                currentSequence.Join(CreateFadeTween(g, target, duration));
            }
        }

        private Tween CreateFadeTween(Graphic g, float target, float duration)
        {
            float start = g.color.a;
            return DOTween.To(() => start, val =>
            {
                start = val; // 更新保持
                var c = g.color; c.a = val; g.color = c;
            }, target, duration).SetEase(fadeEase);
        }

        private void KillSequence()
        {
            if (currentSequence != null && currentSequence.IsActive())
            {
                currentSequence.Kill();
            }
        }

        private void OnDisable() => KillSequence();
    }
}
