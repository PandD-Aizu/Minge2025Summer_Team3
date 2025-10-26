using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace CreditScreen
{
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
        
        private void FixScrollRectSizes()
        {
            if (scrollRect == null) return;
            
            // ScrollRectのサイズを修正
            RectTransform scrollRectTransform = scrollRect.GetComponent<RectTransform>();
            if (scrollRectTransform.sizeDelta.x == 0 || scrollRectTransform.sizeDelta.y == 0)
            {
                scrollRectTransform.sizeDelta = new Vector2(800, 600);
            }
            
            // Viewportのサイズを修正
            if (scrollRect.viewport != null && scrollRect.viewport.sizeDelta.y < 0)
            {
                scrollRect.viewport.anchorMin = Vector2.zero;
                scrollRect.viewport.anchorMax = Vector2.one;
                scrollRect.viewport.offsetMin = Vector2.zero;
                scrollRect.viewport.offsetMax = Vector2.zero;
            }
        }
        
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
        
        private void CalculateMaxScrollPosition()
        {
            if (contentTransform == null || scrollRect == null) return;
            
            float contentHeight = contentTransform.rect.height;
            float viewportHeight = scrollRect.viewport.rect.height;
            maxScrollPosition = Mathf.Max(0, contentHeight - viewportHeight);
        }
        
        public void UpdateScrollPosition(float normalizedPosition)
        {
            if (scrollRect == null) return;
            
            float clampedPosition = Mathf.Clamp01(normalizedPosition);
            scrollRect.verticalNormalizedPosition = 1f - clampedPosition;
        }
        
        public float GetMaxScrollPosition()
        {
            return maxScrollPosition;
        }
        
        public void ResetScrollPosition()
        {
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = 1f;
            }
        }
        
        public void EnableInteraction(bool enable)
        {
            if (scrollRect != null)
            {
                scrollRect.enabled = enable;
            }
        }
    }
}