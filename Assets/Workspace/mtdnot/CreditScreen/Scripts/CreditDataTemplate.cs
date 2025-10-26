using UnityEngine;
using System.Collections.Generic;

namespace CreditScreen
{
    /// <summary>
    /// クレジットデータのテンプレートを提供するユーティリティクラス
    /// </summary>
    public static class CreditDataTemplate
    {
        /// <summary>
        /// デフォルトのクレジットテンプレートを設定する
        /// </summary>
        /// <param name="creditData">設定対象のクレジットデータ</param>
        public static void SetupDefaultTemplate(CreditData creditData)
        {
            if (creditData == null) return;
            
            creditData.gameTitle = "Your Game Title";
            
            creditData.creditSections = new List<CreditData.CreditSection>
            {
                new CreditData.CreditSection
                {
                    sectionTitle = "DIRECTOR",
                    credits = new List<string> { "Director Name" }
                },
                new CreditData.CreditSection
                {
                    sectionTitle = "PROGRAMMERS",
                    credits = new List<string> { "Programmer 1", "Programmer 2" }
                },
                new CreditData.CreditSection
                {
                    sectionTitle = "ARTISTS",
                    credits = new List<string> { "Artist 1", "Artist 2" }
                },
                new CreditData.CreditSection
                {
                    sectionTitle = "SOUND",
                    credits = new List<string> { "Sound Designer" }
                },
                new CreditData.CreditSection
                {
                    sectionTitle = "SPECIAL THANKS",
                    credits = new List<string> { "Special Thanks 1", "Special Thanks 2" }
                }
            };
        }
    }
}