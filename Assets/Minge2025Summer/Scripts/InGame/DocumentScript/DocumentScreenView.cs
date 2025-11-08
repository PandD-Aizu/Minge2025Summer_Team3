using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.DocumentScript
{
    public class DocumentScreenView : MonoBehaviour
    {
        [Header("ドキュメントの本体画面")]
        [SerializeField, Tooltip("ドキュメント画面の親オブジェクト")] private GameObject documentScreenParent;

        [Header("テキスト関連")]
        [SerializeField, Tooltip("タイトルの文字列")] private TextMeshProUGUI titleText;
        [SerializeField, Tooltip("メインのテキスト")] private TextMeshProUGUI contentText;

        [Header("ページ送り関連")]
        [SerializeField, Tooltip("ページ送りの親オブジェクト")] private GameObject pageForwardingParent;
        [SerializeField, Tooltip("ページ送りグリッドの親オブジェクト")] private GameObject pageForwardingGridParent;
        [SerializeField, Tooltip("ページグリッドのプレハブ")] private GameObject pageGridPrefab;

        private readonly List<PageGridView> pageGrids = new ();
        
        /// <summary>
        /// ドキュメント画面の表示・非表示を切り替える
        /// </summary>
        public void Show()
        {
            if (documentScreenParent != null)
                documentScreenParent.SetActive(true);
        }

        /// <summary>
        /// ドキュメント画面の表示・非表示を切り替える
        /// </summary>
        public void Hide()
        {
            if (documentScreenParent != null)
                documentScreenParent.SetActive(false);
        }

        /// <summary>
        /// モデルの内容をビューに適用する
        /// </summary>
        /// <param name="model">モデル</param>
        public void Apply(DocumentScreenModel model)
        {
            if (model == null) return;

            if (titleText != null)
                titleText.text = string.IsNullOrEmpty(model.Title) ? "Error: No Title" : model.Title;

            if (contentText != null)
                contentText.text = model.CurrentPageText;

            var multi = model.HasMultiplePages;
            if (pageForwardingParent != null)
                pageForwardingParent.SetActive(multi);
            if (pageForwardingGridParent != null)
                pageForwardingGridParent.SetActive(multi);
            
            if (multi)
            {
                EnsureGrid(model.TotalPages);
                UpdateHighlight(model.PageIndex);
            }
            else
            {
                ClearGrids();
            }
        }

        private void EnsureGrid(int count)
        {
            if (pageGridPrefab == null || pageForwardingGridParent == null)
                return;

            if (pageGrids.Count == count)
                return;

            ClearGrids();

            var parentTransform = pageForwardingGridParent.transform;
            for (int i = 0; i < count; i++)
            {
                var instance = Instantiate(pageGridPrefab, parentTransform);
                instance.name = $"PageGrid_{i}";
                
                var gridView = instance.GetComponent<PageGridView>();
                if (gridView != null)
                {
                    gridView.GetFrameImage.enabled = false;
                    pageGrids.Add(gridView);
                }
            }
        }

        public void UpdateHighlight(int current)
        {
            for (int i = 0; i < pageGrids.Count; i++)
            {
                pageGrids[i].GetFrameImage.enabled = (i == current);
            }
        }

        private void ClearGrids()
        {
            foreach (var pageGrid in pageGrids)
            {
                if (pageGrid != null)
                    Destroy(pageGrid.gameObject);
            }
            
            pageGrids.Clear();
        }
    }
}