using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame
{
    public class CaptionView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI captionText;
        [SerializeField] private float fadeInDuration = 0.25f;
        [SerializeField] private float fadeOutDuration = 0.25f;
        [SerializeField] private bool useUnscaledTime = true;
        
        private Sequence sequence;
        
        public void Show(string text, float displayDuration)
        {
            if (string.IsNullOrEmpty(text) || captionText == null) return;

            // 進行中を停止
            if (sequence != null && sequence.IsActive()) sequence.Kill();
            captionText.DOKill();

            captionText.gameObject.SetActive(true);
            captionText.text = text;

            // 透明から開始
            var c = captionText.color;
            c.a = 0f;
            captionText.color = c;

            // フェードイン → 待機 → フェードアウト
            sequence = DOTween.Sequence()
                .SetUpdate(useUnscaledTime)
                .SetAutoKill(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .Append(captionText.DOFade(1f, Mathf.Max(0f, fadeInDuration)))
                .AppendInterval(Mathf.Max(0f, displayDuration))
                .Append(captionText.DOFade(0f, Mathf.Max(0f, fadeOutDuration)))
                .OnKill(() => sequence = null);
        }

        public void Hide()
        {
            if (captionText == null) return;

            if (sequence != null && sequence.IsActive()) sequence.Kill();
            captionText.DOKill();

            captionText
                .DOFade(0f, Mathf.Max(0f, fadeOutDuration))
                .SetUpdate(useUnscaledTime)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }

        private void OnDisable()
        {
            captionText?.DOKill();
        }

        private void OnDestroy()
        {
            if (sequence != null && sequence.IsActive()) sequence.Kill();
            captionText?.DOKill();
        }
    }
}