using TMPro;
using UnityEngine;
using DG.Tweening;

namespace Minge2025Summer.Scripts.InGame.ReiScript.ItemScript
{
    public class ReiItemInteractionView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI systemText;
        [SerializeField] private float fadeInDuration = 0.15f;
        [SerializeField] private float visibleDuration = 2f;
        [SerializeField] private float fadeOutDuration = 0.3f;

        private Tween currentTween;

        private void OnDestroy()
        {
            currentTween?.Kill();
            currentTween = null;
        }

        public void Notify(string itemName)
        {
            if (systemText == null)
            {
                Debug.LogWarning("[ReiItemInteractionView] systemText is not assigned.");
                return;
            }

            // キャンセル
            currentTween?.Kill();
            currentTween = null;

            // 表示テキスト設定
            systemText.text = $"{itemName} を入手した。";

            // 初期アルファを 0 にして表示
            var baseColor = systemText.color;
            var invisible = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
            var visible = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);
            systemText.color = invisible;
            systemText.gameObject.SetActive(true);

            // DOTweenでフェードイン->待機->フェードアウト
            var seq = DOTween.Sequence().SetLink(systemText.gameObject);
            seq.Append(DOTween.To(() => systemText.color, x => systemText.color = x, visible, fadeInDuration));
            seq.AppendInterval(visibleDuration);
            seq.Append(DOTween.To(() => systemText.color, x => systemText.color = x, invisible, fadeOutDuration));
            seq.OnComplete(() =>
            {
                // 終了時は非表示にしてアルファを0に固定
                systemText.color = invisible;
                systemText.gameObject.SetActive(false);
                currentTween = null;
            });

            currentTween = seq;
        }
    }
}
