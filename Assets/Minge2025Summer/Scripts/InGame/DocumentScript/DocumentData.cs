using System;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.DocumentScript
{
    [Serializable]
    public class DocumentData
    {
        public string title;
        [TextArea] public string body;
        public string[] pages;

        /* ---ヘルパー関数--- */
        
        /// <summary>
        /// ページ分割されたテキストを取得する
        /// </summary>
        /// <returns>分割されたテキスト</returns>
        public string[] GetEffectivePages(string separator)
        {
            if (pages != null && pages.Length > 0)
                return pages;
            
            if (string.IsNullOrEmpty(body))
                return Array.Empty<string>();
            
            if (string.IsNullOrEmpty(separator)) 
                return new[] { body };

            return body.Split(new[] { separator }, StringSplitOptions.None);
        }
    }
}