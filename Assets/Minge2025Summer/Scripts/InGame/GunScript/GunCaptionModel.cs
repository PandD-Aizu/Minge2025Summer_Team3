using System.Collections.Generic;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.GunScript
{
    public class GunCaptionModel : MonoBehaviour
    {
        [Header("表示するテキスト")] 
        [SerializeField] private List<string> reloadTexts;
        
        /// <summary>
        /// ランダムなリロードテキストを取得する
        /// </summary>
        /// <returns>リロードテキスト</returns>
        public string GetRandomReloadText()
        {
            if (reloadTexts == null || reloadTexts.Count == 0)
                return "(リロードしないと...)";
            
            int randomIndex = Random.Range(0, reloadTexts.Count);
            return reloadTexts[randomIndex];
        }
    }
}