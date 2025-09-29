using UnityEngine;
using UnityEngine.UI;

namespace Workspace.koto_thing.PlayerStatusScript
{
    [DisallowMultipleComponent]
    public class PlayerHpView : MonoBehaviour
    {
        [Header("依存関係")]
        [SerializeField] private PlayerHpWaveGraphic waveGraphic;
        [SerializeField, Tooltip("背景となるImage(任意)")] private Image backgroundImage;

        [Header("心拍インターバル設定(秒)")]
        [SerializeField, Tooltip("最も速い鼓動 (低HP)")] private float minBeatInterval = 0.30f;
        [SerializeField, Tooltip("最も遅い鼓動 (高HP)")] private float maxBeatInterval = 1.20f;

        [Header("HPカラー設定")] 
        [SerializeField] private Color healthyColor = Color.green;
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color dangerColor = Color.red;

        [Header("フェード設定")] 
        [SerializeField, Tooltip("HP変化でフェードインに要する秒数")] private float fadeInDuration = 0.25f;
        [SerializeField, Tooltip("表示維持時間(この秒数経過後フェードアウト開始)")] private float visibleHoldSeconds = 2.5f;
        [SerializeField, Tooltip("フェードアウトに要する秒数")] private float fadeOutDuration = 0.6f;
        [SerializeField, Tooltip("開始時に非表示なら true")] private bool startHidden = true;
        [SerializeField, Tooltip("HP変化判定の最小差分")] private float hpChangeThreshold = 0.01f;

        private enum DisplayState { Hidden, FadingIn, Visible, FadingOut }
        private DisplayState displayState = DisplayState.Hidden;
        private float visibleTimer;
        private float previousHp = float.NaN;

        // フェード管理
        private float fadeAlpha = 0f; // 0 = 非表示 1 = 完全表示
        private Color currentWaveBaseColor = Color.white;
        private Color backgroundBaseColor = Color.white;

        private float currentBeatInterval;
        private bool isFlatline;
        private bool initialized;

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            // 多重初期化防止
            if (initialized)
                return;
            
            // 依存関係チェック
            if (!waveGraphic)
            {
                waveGraphic = GetComponentInChildren<PlayerHpWaveGraphic>();
                if (!waveGraphic)
                {
                    Debug.LogError("PlayerHpWaveGraphic が割り当てられていません", this);
                    return;
                }
            }
            
            // 背景初期化
            if (backgroundImage)
                backgroundBaseColor = backgroundImage.color;
            else
                backgroundBaseColor = new Color(0f,0f,0f,0.4f); // 任意のデフォルト (未設定時)

            waveGraphic.Initialize(); // 心拍グラフィック初期化
            currentBeatInterval = maxBeatInterval;
            isFlatline = false;
            ApplyColor(healthyColor); // これが currentWaveBaseColor を設定
            initialized = true;

            // 開始時の表示状態設定
            if (startHidden)
            {
                fadeAlpha = 0f;
                displayState = DisplayState.Hidden;
            }
            else
            {
                fadeAlpha = 1f;
                displayState = DisplayState.Visible;
                visibleTimer = visibleHoldSeconds;
            }
            
            // 画面に反映
            ApplyFadeAlpha();
        }

        /// <summary>
        /// 体力の描画情報更新
        /// </summary>
        /// <param name="maxHealth">体力最大値</param>
        /// <param name="currentHealth">現在の体力</param>
        public void UpdateHealth(float maxHealth, float currentHealth)
        {
            if (!initialized) 
                Initialize();
            
            if (!waveGraphic || maxHealth <= 0f) 
                return;

            // HP 変化検出
            bool changed = float.IsNaN(previousHp) || Mathf.Abs(currentHealth - previousHp) > hpChangeThreshold;
            previousHp = currentHealth;
            if (changed) 
                TriggerShow();

            // 心拍インターバルと色の更新
            float hpPercent = Mathf.Clamp01(currentHealth / maxHealth);
            currentBeatInterval = Mathf.Lerp(minBeatInterval, maxBeatInterval, hpPercent);
            waveGraphic.SetBeatInterval(currentBeatInterval);

            Color c = hpPercent > 0.5f
                ? Color.Lerp(warningColor, healthyColor, (hpPercent - 0.5f) * 2f)
                : Color.Lerp(dangerColor, warningColor, hpPercent * 2f);
            ApplyColor(c);

            // HPが0以下の処理
            if (currentHealth <= 0f)
            {
                if (!isFlatline)
                {
                    isFlatline = true;
                    waveGraphic.EnterFlatline();
                }
            }
            else if (isFlatline)
            {
                isFlatline = false;
                waveGraphic.Revive();
            }
        }

        /// <summary>
        /// 心拍描画更新
        /// </summary>
        public void UpdateBeat()
        {
            if (!initialized || !waveGraphic)
                return;
            
            waveGraphic.UpdateBeat(Time.deltaTime);
        }

        /// <summary>
        /// 描画の状態を更新
        /// </summary>
        /// <param name="deltaTime">前のフレームからの時間</param>
        public void UpdateDisplay(float deltaTime)
        {
            if (!initialized) 
                return;
            
            switch (displayState)
            {
                // 非表示時
                case DisplayState.Hidden:
                    break;
                
                // フェードイン中
                case DisplayState.FadingIn:
                    // 即時フェードイン
                    if (fadeInDuration <= 0f)
                    {
                        fadeAlpha = 1f;
                        displayState = DisplayState.Visible;
                        visibleTimer = visibleHoldSeconds;
                        ApplyFadeAlpha();
                    }
                    // 通常フェードイン
                    else
                    {
                        fadeAlpha += deltaTime / fadeInDuration;
                        if (fadeAlpha >= 1f)
                        {
                            fadeAlpha = 1f;
                            displayState = DisplayState.Visible;
                            visibleTimer = visibleHoldSeconds;
                        }
                        
                        ApplyFadeAlpha();
                    }
                    break;
                
                // 表示中
                case DisplayState.Visible:
                    // 表示維持時間をカウントダウン
                    visibleTimer -= deltaTime;
                    if (visibleTimer <= 0f)
                        displayState = DisplayState.FadingOut;
                    break;
                
                // フェードアウト中
                case DisplayState.FadingOut:
                    // 即時フェードアウト
                    if (fadeOutDuration <= 0f)
                    {
                        fadeAlpha = 0f;
                        displayState = DisplayState.Hidden;
                        ApplyFadeAlpha();
                    }
                    // 通常フェードアウト
                    else
                    {
                        fadeAlpha -= deltaTime / fadeOutDuration;
                        if (fadeAlpha <= 0f)
                        {
                            fadeAlpha = 0f;
                            displayState = DisplayState.Hidden;
                        }
                        
                        ApplyFadeAlpha();
                    }
                    break;
            }
        }

        /// <summary>
        /// 表示をトリガーする
        /// </summary>
        private void TriggerShow()
        {
            visibleTimer = visibleHoldSeconds;
            
            // 非表示状態からのフェードイン開始
            if (displayState == DisplayState.Hidden)
            {
                // 即時フェードイン
                if (fadeInDuration <= 0f)
                {
                    fadeAlpha = 1f;
                    displayState = DisplayState.Visible;
                    ApplyFadeAlpha();
                }
                // 通常フェードイン
                else
                {
                    fadeAlpha = 0f;
                    displayState = DisplayState.FadingIn;
                    ApplyFadeAlpha();
                }
            }
            // フェードアウト中ならフェードインへ
            else if (displayState == DisplayState.FadingOut)
            {
                displayState = DisplayState.FadingIn; // 再度フェードインへ
            }
            // フェードイン中なら何もしない
            else if (displayState == DisplayState.Visible)
            {
                fadeAlpha = 1f;
                ApplyFadeAlpha();
            }
        }

        /// <summary>
        /// 色を適用する
        /// </summary>
        /// <param name="c">適用する色</param>
        private void ApplyColor(Color c)
        {
            // 常にベースカラーはフルアルファで保持、フェードはglobalAlphaで制御
            currentWaveBaseColor = new Color(c.r, c.g, c.b, 1f);
            UpdateWaveBaseColorOnly();
        }

        /// <summary>
        /// 心拍のベースカラーのみ更新する
        /// </summary>
        private void UpdateWaveBaseColorOnly()
        {
            if (waveGraphic)
                waveGraphic.SetWaveColor(currentWaveBaseColor);
        }

        /// <summary>
        /// フェードアルファを適用する
        /// </summary>
        private void ApplyFadeAlpha()
        {
            // 波形フェード
            if (waveGraphic)
            {
                waveGraphic.SetGlobalAlpha(fadeAlpha);
            }
            
            // 背景フェード
            if (backgroundImage)
            {
                var baseCol = backgroundBaseColor;
                backgroundImage.color = new Color(baseCol.r, baseCol.g, baseCol.b, baseCol.a * fadeAlpha);
            }
        }
    }
}
