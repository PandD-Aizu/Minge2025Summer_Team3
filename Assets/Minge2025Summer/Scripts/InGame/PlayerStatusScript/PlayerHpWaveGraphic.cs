using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Minge2025Summer.Scripts.InGame.PlayerStatusScript
{
    [DisallowMultipleComponent]
    public class PlayerHpWaveGraphic : MaskableGraphic
    {
        [Header("描画設定")]
        [SerializeField, Tooltip("ライン太さ(px)")] private float thickness = 4f;
        [SerializeField, Tooltip("スクロール速度(px/秒)")] private float scrollSpeed = 200f;
        [SerializeField, Tooltip("波形の縦倍率")] private float amplitudeScale = 1f;

        [Header("デバッグ")]
        [SerializeField, Tooltip("エディタ上で初期化ログを出すか")] private bool logInitialize;
        
        [SerializeField, Tooltip("波形のもと")] private float[] beatWaveform = { 0, -2, -10, 40, -25, 15, 0 };

        [Header("カラー拡張")]
        [SerializeField, Tooltip("横方向グラデーション(左 → 右)")] private Gradient horizontalGradient;
        [SerializeField, Tooltip("振幅(絶対値)に応じたカラーグラデーション")] private Gradient amplitudeGradient;
        [SerializeField, Tooltip("水平グラデーションを有効にする")] private bool useHorizontalGradient = true;
        [SerializeField, Tooltip("振幅グラデーションを有効にする")] private bool useAmplitudeGradient = true;
        [SerializeField, Tooltip("振幅/水平カラーの合成モード: 乗算 = true, 補間 = false")] private bool multiplyBlend = true;

        [Header("テールフェード/ハイライト")] 
        [SerializeField, Tooltip("通過後にフェードアウトする秒数(0で無効)")] private float tailFadeSeconds = 0.8f;
        [SerializeField, Tooltip("鼓動直後に適用するハイライト色")] private Color highlightColor = Color.white;
        [SerializeField, Range(0f,1.5f), Tooltip("ハイライト強度(1 = そのまま, > 1 で加算的に明るく)")] private float highlightIntensity = 1.15f;
        [SerializeField, Tooltip("ハイライトが持続する秒数")] private float highlightDuration = 0.18f;
        [SerializeField, Tooltip("フェード時にアルファだけでなく明度も下げる")] private bool dimOnFade = true;

        [SerializeField, Range(0f,1f), Tooltip("全体フェード用(外部制御)")] private float globalAlpha = 1f;
        
        private readonly List<float> points = new List<float>();    // 波形Y座標バッファ
        private readonly List<float> pointAges = new List<float>(); // 各ポイントの経過時間(フェード用)

        private float currentBeatInterval = 1f; // 現在の鼓動間隔(秒)
        private float timeSinceLastBeat;        // 最後の鼓動からの経過時間(秒)
        private float timeSinceScroll;          // スクロール用の経過時間(1秒で1pxスクロール)
        private int currentBeatStep = -1;       // 現在の鼓動波形のステップ(-1=非鼓動中)

        private float cachedWidth; // rectTransform.rect.width のキャッシュ
        private bool initialized;  // 初期化済みフラグ
        private bool flatline;     // フラット線モード(HP0)フラグ

        private float maxWaveAbs = 40f; // 正規化用(初期値は仮)

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            if (initialized)
                return;
            
            ClampParams();
            CacheMaxWaveAbs();
            AllocateBuffer();
            initialized = true;
            
            if (logInitialize) 
                Debug.Log($"[PlayerHpWaveGraphic] Initialize width={cachedWidth} points={points.Count}", this);
        }

        /// <summary>
        /// 現在の鼓動間隔を設定
        /// </summary>
        public void SetBeatInterval(float interval)
        {
            currentBeatInterval = Mathf.Max(0.05f, interval);
        }

        /// <summary>
        /// 表示色を外部から変更
        /// </summary>
        public void SetWaveColor(Color c)
        {
            color = c;
            SetVerticesDirty();
        }

        /// <summary>
        /// フラット線(HP0)
        /// </summary>
        public void EnterFlatline()
        {
            flatline = true;
            currentBeatStep = -1;
            timeSinceLastBeat = 0f;
            
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = 0f;
                pointAges[i] = 0f;
            }
            
            SetVerticesDirty();
        }

        /// <summary>
        /// 復帰(HP > 0)
        /// </summary>
        public void Revive()
        {
            if (!flatline) 
                return;
            
            flatline = false;
            currentBeatStep = -1;
            timeSinceLastBeat = 0f;
        }

        /// <summary>
        /// 1フレーム分の更新
        /// </summary>
        public void UpdateBeat(float deltaTime)
        {
            if (!initialized) 
                Initialize();
            
            if (deltaTime <= 0f) 
                return;

            // 横幅の変化を検出
            if (!Mathf.Approximately(rectTransform.rect.width, cachedWidth))
            {
                AllocateBuffer();
                SetVerticesDirty();
            }

            // スクロール & 新規ポイント追加
            if (!flatline)
            {
                timeSinceScroll += deltaTime * scrollSpeed;
                while (timeSinceScroll >= 1f)
                {
                    timeSinceScroll -= 1f;
                    
                    // 先頭ポイント削除
                    if (points.Count > 0)
                    {
                        points.RemoveAt(0);
                        pointAges.RemoveAt(0);
                    }

                    // 末尾に新規ポイント追加
                    float newY = 0f;
                    if (currentBeatStep >= 0)
                    {
                        newY = beatWaveform[currentBeatStep] * amplitudeScale;
                        currentBeatStep++;
                        if (currentBeatStep >= beatWaveform.Length)
                            currentBeatStep = -1;
                    }
                    
                    points.Add(newY);
                    pointAges.Add(0f); // 新規ポイント age = 0にする
                }

                // 経過時間進行 & フェード用 ageを加算
                if (tailFadeSeconds > 0f)
                {
                    float addAge = deltaTime;
                    for (int i = 0; i < pointAges.Count; i++)
                        pointAges[i] += addAge;
                }

                // 鼓動トリガー判定
                timeSinceLastBeat += deltaTime;
                if (timeSinceLastBeat >= currentBeatInterval && currentBeatStep < 0)
                {
                    timeSinceLastBeat = 0f;
                    currentBeatStep = 0;
                }
            }
            // フラット線モード
            else
            {
                // フラット中でもageだけは進行させて完全フェードさせる場合
                if (tailFadeSeconds > 0f)
                {
                    float addAge = deltaTime;
                    for (int i = 0; i < pointAges.Count; i++) 
                        pointAges[i] += addAge;
                }
            }

            SetVerticesDirty();
        }

        /// <summary>
        /// 最大の波形絶対値をキャッシュする
        /// </summary>
        private void CacheMaxWaveAbs()
        {
            // 最小値を1にしておく
            maxWaveAbs = 1f;
            
            // 配列が空でなければ最大値を調べる
            if (beatWaveform != null && beatWaveform.Length > 0)
            {
                for (int i = 0; i < beatWaveform.Length; i++)
                {
                    float a = Mathf.Abs(beatWaveform[i]);
                    if (a > maxWaveAbs) 
                        maxWaveAbs = a;
                }
            }
        }

        /// <summary>
        /// パラメータのクランプ
        /// </summary>
        private void ClampParams()
        {
            thickness = Mathf.Max(0.5f, thickness);
            scrollSpeed = Mathf.Max(1f, scrollSpeed);
            amplitudeScale = Mathf.Max(0.1f, amplitudeScale);
            if (beatWaveform == null || beatWaveform.Length == 0)
                beatWaveform = new float[] { 0, 10, -10, 0 };
        }

        /// <summary>
        /// バッファー確保
        /// </summary>
        private void AllocateBuffer()
        {
            cachedWidth = rectTransform.rect.width;
            int count = Mathf.Max(2, Mathf.RoundToInt(cachedWidth));
            points.Clear();
            pointAges.Clear();
            
            for (int i = 0; i < count; i++)
            {
                points.Add(0f);
                pointAges.Add(tailFadeSeconds > 0 ? tailFadeSeconds : 0f);
            }
        }

        /// <summary>
        /// 点の色を計算
        /// </summary>
        /// <param name="index">点のインデックス</param>
        /// <param name="count">点の個数</param>
        /// <returns>その地点の色</returns>
        private Color ComputePointColor(int index, int count)
        {
            Color baseColor = color; // SetWaveColor で設定されたベース

            // 横方向グラデーション & 振幅グラデーション
            if (useHorizontalGradient && horizontalGradient != null && count > 1)
            {
                float t = (float)index / (count - 1);
                Color hg = horizontalGradient.Evaluate(t);
                baseColor = multiplyBlend ? MultiplyColor(baseColor, hg) : Color.Lerp(baseColor, hg, hg.a);
            }

            // 振幅グラデーション
            if (useAmplitudeGradient && amplitudeGradient != null && maxWaveAbs > 0.001f)
            {
                float ampNorm = Mathf.Clamp01(Mathf.Abs(points[index]) / (maxWaveAbs * amplitudeScale));
                Color ag = amplitudeGradient.Evaluate(ampNorm);
                baseColor = multiplyBlend ? MultiplyColor(baseColor, ag) : Color.Lerp(baseColor, ag, ag.a);
            }

            // テールフェード
            if (tailFadeSeconds > 0f)
            {
                // ageに応じてアルファを下げる
                float age = pointAges[index];
                if (age > 0f)
                {
                    // 0 → 1に正規化して反転
                    float fade = Mathf.Clamp01(1f - age / tailFadeSeconds);
                    if (fade <= 0f)
                    {
                        baseColor.a = 0f;
                    }
                    // fadeが1に近い場合はそのまま
                    else
                    {
                        // アルファに適用
                        baseColor.a *= fade;
                        if (dimOnFade)
                        {
                            // 明度も下げる
                            baseColor = Color.Lerp(Color.black, baseColor, fade);
                        }
                    }
                }
            }

            // ハイライト
            if (highlightDuration > 0f && highlightIntensity > 0f)
            {
                // ageに応じてハイライトを適用
                float age = pointAges[index];
                if (age <= highlightDuration)
                {
                    float t = 1f - (age / highlightDuration);
                    Color hl = highlightColor;
                    // 強度でスケールする
                    float intensity = t * highlightIntensity;
                    baseColor = Color.Lerp(baseColor, hl, intensity * 0.5f);
                    baseColor *= (1f + intensity * 0.25f);
                }
            }

            baseColor.a *= globalAlpha;
            return baseColor;
        }

        /// <summary>
        /// 色を乗算する
        /// </summary>
        /// <param name="a">乗算する色</param>
        /// <param name="b">乗算する色</param>
        /// <returns>乗算された色</returns>
        private static Color MultiplyColor(Color a, Color b)
        {
            return new Color(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);
        }

        /// <summary>
        /// メッシュ生成
        /// </summary>
        /// <param name="vh"></param>
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (!initialized) 
                return;
            
            int count = points.Count;
            if (count < 2) 
                return;

            // 頂点生成
            float height = rectTransform.rect.height;
            for (int i = 0; i < count - 1; i++)
            {
                // 2点間の線分を作る
                Vector2 p1 = new Vector2(i, points[i] + height * 0.5f);
                Vector2 p2 = new Vector2(i + 1, points[i + 1] + height * 0.5f);
                
                // 線分の法線を求める
                Vector2 dir = p2 - p1;
                
                // dirがほぼ0ベクトルの場合は右向きにする
                if (dir.sqrMagnitude < 0.0001f) 
                    dir = Vector2.right;
                
                // 法線を正規化
                Vector2 normal = new Vector2(-dir.y, dir.x).normalized;
                float half = thickness * 0.5f;
                
                // 4頂点を求める
                Vector2 v1 = p1 + normal * half;
                Vector2 v2 = p1 - normal * half;
                Vector2 v3 = p2 - normal * half;
                Vector2 v4 = p2 + normal * half;

                // 中心を原点にする
                Vector2 offset = new Vector2(rectTransform.rect.width * 0.5f, rectTransform.rect.height * 0.5f);
                v1 -= offset;
                v2 -= offset;
                v3 -= offset;
                v4 -= offset;

                // 色を求める
                Color cLeft = ComputePointColor(i, count);
                Color cRight = ComputePointColor(i + 1, count);
                
                // クワッド追加
                AddQuad(vh, v1, v2, v3, v4, cLeft, cRight);
            }
        }

        /// <summary>
        /// クワッドを追加
        /// </summary>
        /// <param name="vh"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="v4"></param>
        /// <param name="cLeft"></param>
        /// <param name="cRight"></param>
        private static void AddQuad(VertexHelper vh, Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4, Color32 cLeft, Color32 cRight)
        {
            int start = vh.currentVertCount;
            UIVertex vert = UIVertex.simpleVert;

            vert.color = cLeft; vert.position = v1; vh.AddVert(vert);
            vert.color = cLeft; vert.position = v2; vh.AddVert(vert);
            vert.color = cRight; vert.position = v3; vh.AddVert(vert);
            vert.color = cRight; vert.position = v4; vh.AddVert(vert);

            vh.AddTriangle(start + 0, start + 1, start + 2);
            vh.AddTriangle(start + 2, start + 3, start + 0);
        }

        /// <summary>
        /// 水平/振幅グラデーション・合成モードを一括設定
        /// </summary>
        /// <param name="horizontal">水平グラデーション</param>
        /// <param name="amplitude">振幅グラデーション</param>
        /// <param name="useHorizontal">水平グラデーションを有効にするか</param>
        /// <param name="useAmplitude">振幅グラデーションを有効にするか</param>
        /// <param name="multiply">合成モード: 乗算 = true, 補間 = false</param>
        public void ConfigureGradients(Gradient horizontal, Gradient amplitude, bool useHorizontal = true, bool useAmplitude = true, bool multiply = true)
        {
            horizontalGradient = horizontal;
            amplitudeGradient = amplitude;
            useHorizontalGradient = useHorizontal && horizontalGradient != null;
            useAmplitudeGradient = useAmplitude && amplitudeGradient != null;
            multiplyBlend = multiply;
            SetVerticesDirty();
        }

        /// <summary>
        /// テールフェード設定 (0 で無効)
        /// </summary>
        /// <param name="seconds">フェードにかける秒数</param>
        /// <param name="dim">フェード時に明度も下げるか</param>
        public void SetTailFade(float seconds, bool dim)
        {
            tailFadeSeconds = Mathf.Max(0f, seconds);
            dimOnFade = dim;
            for (int i = 0; i < pointAges.Count; i++)
                pointAges[i] = 0f; // リセット
            SetVerticesDirty();
        }

        /// <summary>
        /// ハイライト設定 (durationが0以下で無効)
        /// </summary>
        /// <param name="highlightCol">ハイライト色</param>
        /// <param name="intensity">強度(1 = そのまま, > 1 で加算的に明るく)</param>
        /// <param name="duration">持続秒数</param>
        public void SetHighlight(Color highlightCol, float intensity, float duration)
        {
            highlightColor = highlightCol;
            highlightIntensity = Mathf.Max(0f, intensity);
            highlightDuration = Mathf.Max(0f, duration);
            SetVerticesDirty();
        }

        /// <summary>
        /// 波形サンプルを差し替え(次の鼓動から反映)
        /// </summary>
        /// <param name="samples">波形サンプル配列</param>
        public void SetWaveform(float[] samples)
        {
            if (samples == null || samples.Length == 0) return;
            beatWaveform = samples;
            CacheMaxWaveAbs();
        }

        /// <summary>
        /// 全体フェード設定(0で無効)
        /// </summary>
        /// <param name="a">0〜1</param>
        public void SetGlobalAlpha(float a)
        {
            float clamped = Mathf.Clamp01(a);
            if (Mathf.Approximately(globalAlpha, clamped)) return;
            globalAlpha = clamped;
            SetVerticesDirty();
        }
    }
}
