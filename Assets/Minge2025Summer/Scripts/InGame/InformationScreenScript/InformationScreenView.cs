using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minge2025Summer.Main.InGame
{
    /// <summary>
    /// 情報画面の UI を更新するビュー。プレゼンターから呼ばれる。
    /// </summary>
    public class InformationScreenView : MonoBehaviour
    {
        [Header("UI 参照")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI bodyText;
        [SerializeField] private Image illustrationImage;
        [SerializeField, Tooltip("箇条書きの1項目を先頭に付ける文字列")] private string bulletPrefix = "• ";

        /// <summary>
        /// データを受け取りUIをまとめて更新する。
        /// </summary>
        /// <param name="data">JOSONデータ</param>>
        public void UpdateView(InformationScreenData data)
        {
            if (data == null)
                return;
            
            if (titleText != null) 
                titleText.text = data.Title ?? string.Empty;
            
            // 本文と箇条書きの組み立て
            if (bodyText != null)
            {
                if (data.BulletPoints != null && data.BulletPoints.Count > 0)
                {
                    var sb = new StringBuilder();
                    if (!string.IsNullOrEmpty(data.Body)) 
                        sb.AppendLine(data.Body).AppendLine();
                    
                    foreach (var bp in data.BulletPoints)
                    {
                        if (string.IsNullOrWhiteSpace(bp)) 
                            continue;
                        
                        sb.Append(bulletPrefix).AppendLine(bp.Trim());
                    }
                    bodyText.text = sb.ToString();
                }
                else
                {
                    bodyText.text = data.Body ?? string.Empty;
                }
            }
            
            // イラストの設定
            if (illustrationImage != null)
            {
                illustrationImage.sprite = data.IllustrationSprite;
                illustrationImage.enabled = illustrationImage.sprite != null;
            }
        }
    }
}