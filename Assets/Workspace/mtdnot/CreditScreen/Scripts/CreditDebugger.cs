using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// クレジット画面のデバッグ用クラス（開発時のみ使用）
/// </summary>
public class CreditDebugger : MonoBehaviour
{
    [SerializeField] private Text testText;
    [SerializeField] private ScrollRect scrollRect;
    
    /// <summary>
    /// 初期化処理でテストテキストとスクロール設定を行う
    /// </summary>
    private void Start()
    {
        Debug.Log("CreditDebugger Started");
        
        if (testText != null)
        {
            // 長いテキストを作成
            string longText = "";
            for (int i = 1; i <= 50; i++)
            {
                longText += $"Credit Line {i}\n";
            }
            testText.text = longText;
            Debug.Log("Text set successfully");
            
            // Contentのサイズを手動設定
            RectTransform textRect = testText.GetComponent<RectTransform>();
            if (textRect != null)
            {
                textRect.sizeDelta = new Vector2(textRect.sizeDelta.x, 2000);
                Debug.Log("Text height set to 2000");
            }
            
            // ContentのRectTransformも設定
            Transform content = testText.transform.parent;
            if (content != null)
            {
                RectTransform contentRect = content.GetComponent<RectTransform>();
                if (contentRect != null)
                {
                    contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, 2000);
                    Debug.Log("Content height set to 2000");
                }
            }
        }
        else
        {
            Debug.LogError("TestText is null!");
        }
        
        if (scrollRect != null)
        {
            Debug.Log("ScrollRect found");
            Debug.Log($"ScrollRect content size: {scrollRect.content.sizeDelta}");
            Debug.Log($"ScrollRect viewport size: {scrollRect.viewport.sizeDelta}");
            
            // ViewportとScrollViewのサイズを修正
            RectTransform scrollRectTransform = scrollRect.GetComponent<RectTransform>();
            if (scrollRectTransform.sizeDelta.x == 0 || scrollRectTransform.sizeDelta.y == 0)
            {
                scrollRectTransform.sizeDelta = new Vector2(800, 600);
                Debug.Log("ScrollRect size fixed to 800x600");
            }
            
            // Viewportのサイズを修正
            if (scrollRect.viewport.sizeDelta.y < 0)
            {
                scrollRect.viewport.anchorMin = Vector2.zero;
                scrollRect.viewport.anchorMax = Vector2.one;
                scrollRect.viewport.offsetMin = Vector2.zero;
                scrollRect.viewport.offsetMax = Vector2.zero;
                Debug.Log("Viewport anchors fixed");
            }
        }
        else
        {
            Debug.LogError("ScrollRect is null!");
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space pressed - testing scroll");
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition -= 0.1f;
            }
        }
    }
}