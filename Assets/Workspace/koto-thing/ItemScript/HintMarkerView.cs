using System.Linq;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class HintMarkerView : MonoBehaviour
    {
        [Header("表示するスプライトレンダラー")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Vector3 originalScale;

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            originalScale = transform.localScale;
            SwitchVisibility(false);
        }

        /// <summary>
        /// 可視設定を切り替え
        /// </summary>
        /// <param name="isVisible">可視化するかどうか</param>
        public void SwitchVisibility(bool isVisible)
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = isVisible;
        }

        /// <summary>
        /// ビルボードを回転
        /// </summary>
        /// <param name="enable">有効かどうか</param>
        public void RotateBillboard(bool enable)
        {
            if (!enable || Camera.main == null) return;
            var cam = Camera.main;
            transform.rotation = Quaternion.LookRotation(
                cam.transform.rotation * Vector3.forward,
                cam.transform.rotation * Vector3.up
            );
        }

        /// <summary>
        /// 距離によって透明度を更新
        /// </summary>
        /// <param name="playerTransform">プレイヤーのTransform</param>
        /// <param name="maxDistance">最大距離</param>
        /// <param name="minDistance">最小距離</param>
        public void UpdateAlphaByDistance(Transform playerTransform, float maxDistance, float minDistance = 0.0f)
        {
            if (playerTransform == null || spriteRenderer == null) return;
            if (maxDistance <= minDistance)
                maxDistance = minDistance + 0.001f;

            float distance = Vector3.Distance(transform.position, playerTransform.position);
            float normalized = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));
            float alpha = 1f - normalized;

            var c = spriteRenderer.color;
            c.a = alpha;
            spriteRenderer.color = c;
        }

        /// <summary>
        /// 座標とスケールを更新
        /// </summary>
        /// <param name="target"></param>
        /// <param name="verticalOffset"></param>
        /// <param name="useBounds"></param>
        /// <param name="extraHeight"></param>
        /// <param name="player"></param>
        /// <param name="maxDistance"></param>
        /// <param name="scaleWithDistance"></param>
        /// <param name="scaleCurve"></param>
        /// <param name="baseScale"></param>
        /// <param name="maxScaleMultiplier"></param>
        public void UpdatePositionAndScale(
            Transform target,
            float verticalOffset,
            bool useBounds,
            float extraHeight,
            Transform player,
            float maxDistance,
            bool scaleWithDistance,
            AnimationCurve scaleCurve,
            float baseScale,
            float maxScaleMultiplier
        )
        {
            if (target == null) return;

            // 高さ決定
            float topY = target.position.y;

            if (useBounds)
            {
                var renderers = target.GetComponentsInChildren<Renderer>();
                if (renderers.Length > 0)
                {
                    float maxY = float.MinValue;
                    foreach (var r in renderers)
                    {
                        if (!r.enabled) continue;
                        if (r.transform == transform || r.transform.IsChildOf(transform))
                            continue;

                        maxY = Mathf.Max(maxY, r.bounds.max.y);
                    }

                    if (maxY > float.MinValue)
                        topY = maxY;
                }
            }

            Vector3 pos = target.position;
            pos.y = topY + extraHeight + verticalOffset;
            transform.position = pos;

            // スケール
            if (player != null && scaleWithDistance && maxDistance > 0.01f)
            {
                float dist = Vector3.Distance(player.position, pos);
                float t = Mathf.Clamp01(dist / maxDistance);
                float eval = Mathf.Clamp01(scaleCurve.Evaluate(t));
                float mul = Mathf.Lerp(maxScaleMultiplier, 1f, eval);
                transform.localScale = originalScale * baseScale * mul;
            }
            else
            {
                transform.localScale = originalScale * baseScale;
            }
        }
    }
}
