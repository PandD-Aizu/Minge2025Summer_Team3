using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minge2025Summer.Main.InGame
{
    public class ObjectiveTextView : MonoBehaviour
    {
        [Header("UI 参照")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI subTitleText;
        [SerializeField] private TextMeshProUGUI bodyText;
        [SerializeField] private Image illustrationImage;

        /// <summary>
        /// データを受け取りUIをまとめて更新する。
        /// </summary>
        /// <param name="data">JSONデータ</param>>
        public void UpdateView(ObjectiveTextData data)
        {
            if (data == null)
                return;
            
            if (titleText != null) 
                titleText.text = data.Title ?? string.Empty;
            
            if (subTitleText != null)
                subTitleText.text = data.SubTitle ?? string.Empty;
            
            if (bodyText != null)
                bodyText.text = data.Body ?? string.Empty;
            
            // イラストの設定
            if (illustrationImage != null)
            {
                illustrationImage.sprite = data.IllustrationSprite;
                illustrationImage.enabled = illustrationImage.sprite != null;
            }
        }
    }
}