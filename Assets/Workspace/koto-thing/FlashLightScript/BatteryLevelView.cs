using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Workspace.koto_thing
{
    public class BatteryLevelView : MonoBehaviour
    {
        [Header("懐中電灯のライト設定")]
        [SerializeField] private Light flashLight;
        
        [Header("画面左上のバッテリゲージ設定")]
        [SerializeField, Tooltip("バッテリゲージオブジェクト(左から順に)")] private List<GameObject> batteryGaugeObjects = new List<GameObject>();
        [SerializeField, Tooltip("マテリアルのFillAmountプロパティ名")] private string fillAmountPropertyName = "_FillAmount";
        [SerializeField, Tooltip("マテリアルのTopColorプロパティ名")] private string topColorPropertyName = "_TopColor";
        [SerializeField, Tooltip("マテリアルのBottomColorプロパティ名")] private string bottomColorPropertyName = "_BottomColor";
        [SerializeField, Range(0.0f, 0.01f)] private float hideThreshold = 0.0001f;
        [SerializeField, Tooltip("背景マテリアルをインスタンス化するか")] private bool instantiateBackgroundMaterial = true;
        
        [Header("Tween設定")]
        [SerializeField] private float showDuration = 0.25f;
        [SerializeField] private float hideDuration = 0.25f;
        [SerializeField] private Ease showEase = Ease.OutQuad;
        [SerializeField] private Ease hideEase = Ease.InQuad;

        private class Segment
        {
            public GameObject gameObject;
            public Image[] images;
            public Image backgroundImage;
            public Color[] baseColors;
            public Material backgroundMaterial;
            public Color topBaseColor;
            public Color bottomBaseColor;
            public float currentAlpha;
            public bool visible;
            public Tweener tween;
        }
        
        private int fillPropID;
        private int topColorPropID;
        private int bottomColorPropID;
        private Segment[] segments;
        
        public Light GetFlashLight => flashLight;

        /// <summary>
        /// 初期化処理を実行します。二重呼び出しは無視
        /// セグメントの構築とシェーダープロパティIDのキャッシュを行う
        /// </summary>
        public void Initialize()
        {
            GetPropertyID();
            BuildSegments();
        }

        /// <summary>
        /// バッテリー残量に基づき各セグメントの FillAmount と表示状態(フェードイン/アウト) を更新
        /// </summary>
        /// <param name="maxBatteryLevel">バッテリーの最大値 (0以下の場合は1として扱う)</param>
        /// <param name="currentBatteryLevel">現在のバッテリー値</param>
        public void UpdateSegments(float maxBatteryLevel, float currentBatteryLevel)
        {
            if (segments == null)
                return;
            
            float max = maxBatteryLevel <= 0.0f ? 1.0f : maxBatteryLevel;
            float normalized = Mathf.Clamp01(currentBatteryLevel / max);
            int total = segments.Length;

            for (int i = 0; i < total; i++)
            {
                var segment = segments[i];
                if (segment == null)
                    continue;

                float segmentStart = (float)i / total;
                float segmentEnd = (float)(i + 1) / total;
                float segmentFill = (normalized - segmentStart) / (segmentEnd - segmentStart);
                segmentFill = Mathf.Clamp01(segmentFill);
                
                if (segment.backgroundMaterial != null)
                    segment.backgroundMaterial.SetFloat(fillPropID, segmentFill);

                bool shouldShow = normalized > segmentStart + hideThreshold;

                if (segment.visible != shouldShow)
                    ApplySegmentVisibility(segment, shouldShow);
            }
        }
        
        /* 以下ヘルパー関数 */
        
        /// <summary>
        /// シェーダープロパティ名から整数IDを取得しキャッシュ
        /// </summary>
        private void GetPropertyID()
        {
            fillPropID = Shader.PropertyToID(fillAmountPropertyName);
            topColorPropID = Shader.PropertyToID(topColorPropertyName);
            bottomColorPropID = Shader.PropertyToID(bottomColorPropertyName);
        }

        /// <summary>
        /// セグメント(GameObject)配列を走査し、背景用マテリアルと前景 Image を判別・保持
        /// 必要に応じてマテリアルをインスタンス化し、初期アルファを 0 に設定
        /// </summary>
        private void BuildSegments()
        {
            int count = batteryGaugeObjects.Count;
            segments = new Segment[count];
            for (int i = 0; i < count; i++)
            {
                var root = batteryGaugeObjects[i];
                if (root == null)
                {
                    segments[i] = null;
                    continue;
                }

                var images = root.GetComponentsInChildren<Image>(true);
                if (images.Length == 0)
                {
                    segments[i] = null;
                    continue;
                }

                Image background = null;
                foreach (var img in images)
                {
                    var mat = img.material;
                    if (mat != null && mat.HasProperty(fillPropID))
                    {
                        background = img;
                        break;
                    }
                }
                if (background == null)
                {
                    foreach (var img in images)
                    {
                        var n = img.name.ToLower();
                        if (n.Contains("back"))
                        {
                            background = img;
                            break;
                        }
                    }
                }
                if (background == null)
                    background = images[images.Length - 1];

                Image foreground = null;
                foreach (var img in images)
                {
                    if (img != background)
                    {
                        foreground = img;
                        break;
                    }
                }

                var ordered = foreground != null ? new[] { foreground, background } : new[] { background };

                Material bgMat = background.material;
                if (instantiateBackgroundMaterial && bgMat != null)
                {
                    bgMat = Instantiate(bgMat);
                    background.material = bgMat;
                }

                Color topBase = Color.white;
                Color bottomBase = Color.white;
                if (bgMat != null)
                {
                    if (bgMat.HasProperty(topColorPropID))
                        topBase = bgMat.GetColor(topColorPropID);
                    if (bgMat.HasProperty(bottomColorPropID))
                        bottomBase = bgMat.GetColor(bottomColorPropID);
                }

                var segment = new Segment
                {
                    gameObject = root,
                    images = ordered,
                    backgroundImage = background,
                    baseColors = Array.ConvertAll(ordered, img => img.color),
                    backgroundMaterial = bgMat,
                    topBaseColor = topBase,
                    bottomBaseColor = bottomBase,
                    currentAlpha = 0.0f,
                    visible = false,
                    tween = null
                };

                // 初期は非表示状態（アルファ0）
                ApplyAlpha(segment, 0f);

                segments[i] = segment;
            }
        }

        /// <summary>
        /// 指定セグメントの表示状態を DOTween を用いてフェードイン/アウト
        /// </summary>
        /// <param name="segment">対象セグメント</param>
        /// <param name="shouldShow">表示すべきか (true: フェードイン / false: フェードアウト)</param>
        private void ApplySegmentVisibility(Segment segment, bool shouldShow)
        {
            segment.tween?.Kill();
            segment.tween = null;

            if (shouldShow)
            {
                if (!segment.gameObject.activeSelf)
                    segment.gameObject.SetActive(true);

                float from = segment.currentAlpha;
                float to = 1.0f;

                segment.tween = DOTween.To(() => from, v =>
                    {
                        from = v;
                        segment.currentAlpha = v;
                        ApplyAlpha(segment, v);
                    }, to, showDuration).SetEase(showEase)
                    .OnComplete(() =>
                    {
                        segment.visible = true;
                        segment.tween = null;
                    });
            }
            else
            {
                float from = segment.currentAlpha;
                float to = 0.0f;

                segment.tween = DOTween.To(() => from, v =>
                    {
                        from = v;
                        segment.currentAlpha = v;
                        ApplyAlpha(segment, v);
                    }, to, hideDuration).SetEase(hideEase)
                    .OnComplete(() =>
                    {
                        segment.visible = false;
                        segment.gameObject.SetActive(false);
                        segment.tween = null;
                    });
            }
        }

        /// <summary>
        /// セグメントに属する全ての Image と背景マテリアル(_TopColor/_BottomColor) のアルファを一括適用
        /// </summary>
        /// <param name="segment">対象セグメント</param>
        /// <param name="alpha">適用するアルファ値 (0～1)</param>
        private void ApplyAlpha(Segment segment, float alpha)
        {
            if (segment.images != null)
            {
                for (int i = 0; i < segment.images.Length; i++)
                {
                    var img = segment.images[i];
                    if (img == null) continue;
                    var baseColor = segment.baseColors[i];
                    baseColor.a = alpha;
                    img.color = baseColor;
                }
            }
            
            if (segment.backgroundMaterial != null)
            {
                if (segment.backgroundMaterial.HasProperty(topColorPropID))
                {
                    var colTop = segment.topBaseColor;
                    colTop.a = alpha;
                    segment.backgroundMaterial.SetColor(topColorPropID, colTop);
                }
                if (segment.backgroundMaterial.HasProperty(bottomColorPropID))
                {
                    var colBottom = segment.bottomBaseColor;
                    colBottom.a = alpha;
                    segment.backgroundMaterial.SetColor(bottomColorPropID, colBottom);
                }
            }
        }

        /// <summary>
        /// 生成したTweenを破棄しリークを防ぐ
        /// </summary>
        private void OnDestroy()
        {
            if (segments == null)
                return;
            
            foreach (var segment in segments)
                segment?.tween?.Kill();
        }
    }
}