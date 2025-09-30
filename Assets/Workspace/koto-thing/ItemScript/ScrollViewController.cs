using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Workspace.koto_thing
{
    public class ScrollViewController : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform contentPanel;
        
        private RectTransform selectedRectTransform;
        private GameObject lastSelected;

        private void Start()
        {
            if (scrollRect == null)
                scrollRect = GetComponentInChildren<ScrollRect>();
            
            if (contentPanel == null && scrollRect != null)
                contentPanel = scrollRect.content;
        }

        private void Update()
        {
            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
            if (currentSelected == null || currentSelected == lastSelected) return;

            if (contentPanel == null) return;
            // contentPanel の子孫でなければ無視
            if (!currentSelected.transform.IsChildOf(contentPanel)) return;

            // 最上位の(contentPanel直下)の子RectTransformをターゲットにする
            Transform t = currentSelected.transform;
            while (t.parent != null && t.parent != contentPanel)
            {
                t = t.parent;
            }
            selectedRectTransform = t as RectTransform;
            if (selectedRectTransform == null) return;

            ScrollToView(selectedRectTransform);

            lastSelected = currentSelected;
        }

        /// <summary>
        /// 目標位置までスクロールする
        /// </summary>
        /// <param name="target">スクロールする目標位置</param>
        private void ScrollToView(RectTransform target)
        {
            // コンテンツとビューポートの高さを取得
            float contentHeight = contentPanel.rect.height;
            float viewportHeight = scrollRect.viewport.rect.height;

            // ターゲットの位置をコンテンツ内でのY座標に変換
            float targetPosY = -target.anchoredPosition.y;

            // ターゲットがビューポートの中央に来るようにスクロール位置を計算
            float targetNormalizedPos = (targetPosY - viewportHeight * 0.5f) / (contentHeight - viewportHeight);

            // スクロール位置を0から1の範囲にクランプして適用
            targetNormalizedPos = Mathf.Clamp01(targetNormalizedPos);
            
            // UnityのScrollRectは垂直方向のスクロール位置が逆なので1から引く
            scrollRect.verticalNormalizedPosition = 1 - targetNormalizedPos;
        }
    }
}