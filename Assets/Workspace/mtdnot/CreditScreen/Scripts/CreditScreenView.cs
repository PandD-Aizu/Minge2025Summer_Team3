using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace CreditScreen
{
    /// <summary>
    /// クレジット画面のUI表示を管理するビュークラス
    /// </summary>
    public class CreditScreenView : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Text creditText;
        [SerializeField] private RectTransform contentTransform;
        [SerializeField] private Image backgroundPanel;
        
        [Header("Visual Settings")]
        [SerializeField] private Color backgroundColor = Color.black;
        [SerializeField] private Color textColor = Color.white;
        [SerializeField] private float lineSpacing = 1.5f;
        [SerializeField] private float sectionSpacing = 50f;
        
        private float maxScrollPosition;
        
        private void Awake()
        {
            InitializeVisuals();
        }
        
        /// <summary>
        /// ビジュアル要素を初期化する
        /// </summary>
        private void InitializeVisuals()
        {
            if (backgroundPanel != null)
            {
                backgroundPanel.color = backgroundColor;
            }
            
            if (creditText != null)
            {
                creditText.color = textColor;
                creditText.lineSpacing = lineSpacing;
                creditText.alignment = TextAnchor.UpperCenter;
            }
            
            if (scrollRect != null)
            {
                scrollRect.vertical = true;
                scrollRect.horizontal = false;
                
                // ScrollRectとViewportのサイズ修正（CreditDebuggerから移植）
                FixScrollRectSizes();
            }
        }
        
        /// <summary>
        /// ScrollRectとその関連要素のサイズを画面全体に修正する
        /// </summary>
        private void FixScrollRectSizes()
        {
            if (scrollRect == null) return;
            
            // ScrollRectを画面全体に拡大
            RectTransform scrollRectTransform = scrollRect.GetComponent<RectTransform>();
            
            // アンカーを画面全体に設定
            scrollRectTransform.anchorMin = Vector2.zero;
            scrollRectTransform.anchorMax = Vector2.one;
            scrollRectTransform.offsetMin = Vector2.zero;
            scrollRectTransform.offsetMax = Vector2.zero;
            
            // 親のCanvasも画面全体に設定
            Canvas parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();
                canvasRect.anchorMin = Vector2.zero;
                canvasRect.anchorMax = Vector2.one;
                canvasRect.offsetMin = Vector2.zero;
                canvasRect.offsetMax = Vector2.zero;
            }
            
            // BackgroundPanelも画面全体に設定
            if (backgroundPanel != null)
            {
                RectTransform bgRect = backgroundPanel.GetComponent<RectTransform>();
                bgRect.anchorMin = Vector2.zero;
                bgRect.anchorMax = Vector2.one;
                bgRect.offsetMin = Vector2.zero;
                bgRect.offsetMax = Vector2.zero;
            }
            
            // Viewportも画面全体に設定
            if (scrollRect.viewport != null)
            {
                scrollRect.viewport.anchorMin = Vector2.zero;
                scrollRect.viewport.anchorMax = Vector2.one;
                scrollRect.viewport.offsetMin = Vector2.zero;
                scrollRect.viewport.offsetMax = Vector2.zero;
            }
        }
        
        /// <summary>
        /// クレジットテキストを設定する
        /// </summary>
        /// <param name="creditData">表示するクレジット情報</param>
        public void SetCreditText(CreditData creditData)
        {
            if (creditText == null || creditData == null) return;
            
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine($"<b>{creditData.gameTitle}</b>");
            sb.AppendLine();
            sb.AppendLine();
            
            foreach (var section in creditData.creditSections)
            {
                sb.AppendLine($"<b>{section.sectionTitle}</b>");
                sb.AppendLine();
                
                foreach (var credit in section.credits)
                {
                    sb.AppendLine(credit);
                }
                
                sb.AppendLine();
                sb.AppendLine();
            }
            
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("Thank You For Playing!");
            
            creditText.text = sb.ToString();
            
            // テキストサイズを強制設定
            ForceContentSize();
            
            Canvas.ForceUpdateCanvases();
            CalculateMaxScrollPosition();
            
        }
        
        /// <summary>
        /// Contentのサイズを強制的に設定する
        /// </summary>
        private void ForceContentSize()
        {
            if (creditText == null || contentTransform == null) return;
            
            // Textの自動サイズ計算を使用
            RectTransform textRect = creditText.GetComponent<RectTransform>();
            if (textRect != null)
            {
                // Textを一時的に幅を固定して高さを自動計算
                float textWidth = textRect.rect.width;
                if (textWidth <= 0) textWidth = contentTransform.rect.width;
                if (textWidth <= 0) textWidth = 400; // フォールバック
                
                // PreferredHeightを使って正確な高さを取得
                float preferredHeight = creditText.preferredHeight;
                
                // 最小でも800pxは確保（短いクレジット用）
                float requiredHeight = Mathf.Max(preferredHeight, 800f);
                
                // ContentとTextのサイズを設定
                contentTransform.sizeDelta = new Vector2(contentTransform.sizeDelta.x, requiredHeight);
                textRect.sizeDelta = new Vector2(textRect.sizeDelta.x, requiredHeight);
                
                // Content Size Fitterがあれば一時的に無効化
                var contentSizeFitter = contentTransform.GetComponent<ContentSizeFitter>();
                if (contentSizeFitter != null)
                {
                    contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                }
            }
        }
        
        /// <summary>
        /// スクロール可能な最大位置を計算する
        /// </summary>
        private void CalculateMaxScrollPosition()
        {
            if (contentTransform == null || scrollRect == null) return;
            
            float contentHeight = contentTransform.rect.height;
            float viewportHeight = scrollRect.viewport.rect.height;
            maxScrollPosition = Mathf.Max(0, contentHeight - viewportHeight);
        }
        
        /// <summary>
        /// スクロール位置を更新する
        /// </summary>
        /// <param name="normalizedPosition">0～1の正規化された位置</param>
        public void UpdateScrollPosition(float normalizedPosition)
        {
            if (scrollRect == null) return;
            
            float clampedPosition = Mathf.Clamp01(normalizedPosition);
            scrollRect.verticalNormalizedPosition = 1f - clampedPosition;
        }
        
        /// <summary>
        /// スクロール可能な最大位置を取得する
        /// </summary>
        /// <returns>最大スクロール位置</returns>
        public float GetMaxScrollPosition()
        {
            return maxScrollPosition;
        }
        
        /// <summary>
        /// スクロール位置を初期状態にリセットする
        /// </summary>
        public void ResetScrollPosition()
        {
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = 1f;
            }
        }
        
        /// <summary>
        /// ユーザー操作の有効/無効を設定する
        /// </summary>
        /// <param name="enable">有効にする場合はtrue</param>
        public void EnableInteraction(bool enable)
        {
            if (scrollRect != null)
            {
                scrollRect.enabled = enable;
            }
        }
    }
}