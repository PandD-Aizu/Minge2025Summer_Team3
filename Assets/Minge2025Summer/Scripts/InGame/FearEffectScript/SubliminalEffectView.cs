using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Minge2025Summer.Scripts.InGame.FearEffectScript
{
    public class SubliminalEffectView : MonoBehaviour
    {
        [Header("サブリミナル効果の設定")]
        [SerializeField] private Image subliminalImage;
        [SerializeField] private List<Sprite> subliminalSprites;

        [SerializeField] private Material subliminalMaterial;
        
        /* プロパティ */
        public int GetSubliminalImageCount => subliminalSprites.Count;

        /// <summary>
        /// ランダムなイメージを選択する
        /// </summary>
        /// <param name="index">ランダムなインデックス</param>
        public void SelectImage(int index)
        {
            subliminalMaterial.SetTexture("_MainTex", subliminalSprites[index].texture);
        }

        /// <summary>
        /// イメージのオンオフを切り替える
        /// </summary>
        /// <param name="isActive">オンかオフか</param>
        public void SwitchImage(bool isActive)
        {
            if (isActive)
                subliminalImage.color = new Color(1, 1, 1, 0.3f);
            else
                subliminalImage.color = new Color(1, 1, 1, 0);
            
            subliminalImage.enabled = isActive;
        }
    }
}