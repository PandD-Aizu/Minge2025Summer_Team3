using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace Minge2025Summer.Scripts.InGame.TimelineScript
{
    public class CinematicBarController : MonoBehaviour
    {
        [SerializeField] private RectTransform upperBar;
        [SerializeField] private RectTransform bottomBar;

        [SerializeField, Tooltip("バーの見た目の高さ（非ストレッチ時の移動距離の基準）")]
        private float barHeight = 200f;

        [SerializeField, Tooltip("表示・非表示にかける時間(秒)")]
        private float cinematicBarMoveTime = 0.5f;

        [SerializeField, Tooltip("補間イージング")]
        private Ease ease = Ease.InOutQuad;

        [SerializeField, Tooltip("Time.timeScaleの影響を受けない")]
        private bool useUnscaledTime = true;

        [SerializeField, Tooltip("画面外に逃がす追加量。符号は無視して絶対値が使用されます")]
        private float extraHideOffset = 64f;

        [SerializeField, Tooltip("OnDisableでも非表示アニメを再生する")]
        private bool animateOnDisable = true;

        private Sequence seq;
        private Coroutine initCo;

        // ストレッチ時の表示/非表示
        private bool upperStretch, bottomStretch;
        private float upperShowMinY, upperShowMaxY, upperHideMinY, upperHideMaxY;
        private float bottomShowMinY, bottomShowMaxY, bottomHideMinY, bottomHideMaxY;

        // 非ストレッチ時の表示/非表示
        private float upperShowAnchY, upperHideAnchY;
        private float bottomShowAnchY, bottomHideAnchY;

        private void OnEnable()
        {
            if (!upperBar || !bottomBar) return;
            KillSequence();
            if (initCo != null) StopCoroutine(initCo);
            initCo = StartCoroutine(InitializeAndShowNextFrame());
        }

        private void OnDisable()
        {
            if (initCo != null) { StopCoroutine(initCo); initCo = null; }

            if (!upperBar || !bottomBar)
            {
                KillSequence();
                return;
            }

            // 現在のレイアウトから位置を再計算
            BuildShowHideCache();

            if (animateOnDisable && gameObject.activeInHierarchy)
            {
                KillSequence();
                ToggleCinematicBar(false);
            }
            else
            {
                KillSequence();
            }
        }

        private IEnumerator InitializeAndShowNextFrame()
        {
            // レイアウト確定待ち
            yield return null;
            Canvas.ForceUpdateCanvases();

            BuildShowHideCache();

            // 画面外へ配置
            PlaceHiddenImmediate();

            // 画面外から表示
            ToggleCinematicBar(true);
            initCo = null;
        }

        private void BuildShowHideCache()
        {
            upperStretch  = IsVerticalStretch(upperBar);
            bottomStretch = IsVerticalStretch(bottomBar);

            float extra = Mathf.Abs(extraHideOffset);

            // 上バー
            if (upperStretch)
            {
                float H = GetParentHeight(upperBar);
                upperShowMinY = upperBar.offsetMin.y;
                upperShowMaxY = upperBar.offsetMax.y;
                float bottomEdge = upperShowMinY;
                float sUp = (H + extra) - bottomEdge;
                upperHideMinY = upperShowMinY + sUp;
                upperHideMaxY = upperShowMaxY + sUp;
            }
            else
            {
                upperShowAnchY = upperBar.anchoredPosition.y;
                float move = GetOffscreenShiftAmount(upperBar, extra);
                upperHideAnchY = upperShowAnchY + move;
            }

            // 下バー
            if (bottomStretch)
            {
                float H = GetParentHeight(bottomBar);
                bottomShowMinY = bottomBar.offsetMin.y;
                bottomShowMaxY = bottomBar.offsetMax.y;
                float topEdge = H + bottomShowMaxY;
                float sDown = -(topEdge + extra);
                bottomHideMinY = bottomShowMinY + sDown;
                bottomHideMaxY = bottomShowMaxY + sDown;
            }
            else
            {
                bottomShowAnchY = bottomBar.anchoredPosition.y;
                float move = GetOffscreenShiftAmount(bottomBar, extra);
                bottomHideAnchY = bottomShowAnchY - move;
            }
        }

        private void PlaceHiddenImmediate()
        {
            if (upperStretch)
                SetOffsetsY(upperBar, upperHideMinY, upperHideMaxY);
            else
                upperBar.anchoredPosition = new Vector2(upperBar.anchoredPosition.x, upperHideAnchY);

            if (bottomStretch)
                SetOffsetsY(bottomBar, bottomHideMinY, bottomHideMaxY);
            else
                bottomBar.anchoredPosition = new Vector2(bottomBar.anchoredPosition.x, bottomHideAnchY);
        }

        public void ToggleCinematicBar(bool isShow)
        {
            if (!upperBar || !bottomBar) return;

            KillSequence();
            float dur = Mathf.Max(0f, cinematicBarMoveTime);
            if (dur > 0f)
                seq = DOTween.Sequence().SetUpdate(useUnscaledTime).SetId(this);

            // 上バー
            if (upperStretch)
            {
                float tgtMin = isShow ? upperShowMinY : upperHideMinY;
                float tgtMax = isShow ? upperShowMaxY : upperHideMaxY;

                if (dur <= 0f)
                {
                    SetOffsetsY(upperBar, tgtMin, tgtMax);
                }
                else
                {
                    seq.Join(DOTween.To(() => upperBar.offsetMin, v => upperBar.offsetMin = v,
                        new Vector2(upperBar.offsetMin.x, tgtMin), dur).SetEase(ease).SetTarget(upperBar));
                    seq.Join(DOTween.To(() => upperBar.offsetMax, v => upperBar.offsetMax = v,
                        new Vector2(upperBar.offsetMax.x, tgtMax), dur).SetEase(ease).SetTarget(upperBar));
                }
            }
            else
            {
                float upY = isShow ? upperShowAnchY : upperHideAnchY;
                if (dur <= 0f) upperBar.anchoredPosition = new Vector2(upperBar.anchoredPosition.x, upY);
                else seq.Join(upperBar.DOAnchorPosY(upY, dur).SetEase(ease));
            }

            // 下バー
            if (bottomStretch)
            {
                float tgtMin = isShow ? bottomShowMinY : bottomHideMinY;
                float tgtMax = isShow ? bottomShowMaxY : bottomHideMaxY;

                if (dur <= 0f)
                {
                    SetOffsetsY(bottomBar, tgtMin, tgtMax);
                }
                else
                {
                    seq.Join(DOTween.To(() => bottomBar.offsetMin, v => bottomBar.offsetMin = v,
                        new Vector2(bottomBar.offsetMin.x, tgtMin), dur).SetEase(ease).SetTarget(bottomBar));
                    seq.Join(DOTween.To(() => bottomBar.offsetMax, v => bottomBar.offsetMax = v,
                        new Vector2(bottomBar.offsetMax.x, tgtMax), dur).SetEase(ease).SetTarget(bottomBar));
                }
            }
            else
            {
                float btY = isShow ? bottomShowAnchY : bottomHideAnchY;
                if (dur <= 0f) bottomBar.anchoredPosition = new Vector2(bottomBar.anchoredPosition.x, btY);
                else seq.Join(bottomBar.DOAnchorPosY(btY, dur).SetEase(ease));
            }

            if (dur <= 0f && seq != null) { seq.Kill(); seq = null; }
        }

        private static bool IsVerticalStretch(RectTransform rt)
            => Mathf.Approximately(rt.anchorMin.y, 0f) && Mathf.Approximately(rt.anchorMax.y, 1f);

        private static float GetParentHeight(RectTransform rt)
        {
            if (!rt || !(rt.parent is RectTransform p)) return 0f;
            float h = p.rect.height;
            if (h > 1f) return h;

            var canvas = rt.GetComponentInParent<Canvas>();
            if (canvas && canvas.rootCanvas)
                return canvas.rootCanvas.pixelRect.height / Mathf.Max(0.0001f, canvas.rootCanvas.scaleFactor);

            return Screen.height;
        }

        private static float GetOffscreenShiftAmount(RectTransform rt, float extraAbs)
        {
            float H = GetParentHeight(rt);
            float rectH = Mathf.Max(0f, rt.rect.height);
            return (H * 0.5f) + (rectH * 0.5f) + extraAbs;
        }

        private static void SetOffsetsY(RectTransform rt, float minY, float maxY)
        {
            var min = rt.offsetMin; min.y = minY; rt.offsetMin = min;
            var max = rt.offsetMax; max.y = maxY; rt.offsetMax = max;
        }

        private void KillSequence()
        {
            if (seq != null) { seq.Kill(); seq = null; }
            DOTween.Kill(upperBar);
            DOTween.Kill(bottomBar);
        }

        public void Show() => ToggleCinematicBar(true);
        public void Hide() => ToggleCinematicBar(false);
    }
}
