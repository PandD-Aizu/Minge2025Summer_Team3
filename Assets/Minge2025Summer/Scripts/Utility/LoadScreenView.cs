using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Minge2025Summer.Scripts.Utility
{
    public class LoadScreenView : MonoBehaviour
    {
        [SerializeField] private GameObject loadScreenPanel;
        [SerializeField] private Slider loadSlider;

        /// <summary>
        /// パネルのアクティブ状態を切り替える
        /// </summary>
        public void ChangeActive()
        {
            loadScreenPanel.SetActive(!loadScreenPanel.activeSelf);
        }
        
        /// <summary>
        /// ロードスライダーの値を更新する
        /// </summary>
        /// <param name="progress">現在の進捗</param>
        public void UpdateLoadSlider(float progress)
        {
            loadSlider.DOValue(progress, 0.2f);
        }
    }
}