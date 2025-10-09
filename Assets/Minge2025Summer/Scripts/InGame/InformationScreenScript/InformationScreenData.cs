using System.Collections.Generic;
using UnityEngine;

namespace Minge2025Summer.Main.InGame
{
    /// <summary>
    /// 情報画面で表示する最終的なデータ（パース後、Sprite 解決済み）。
    /// </summary>
    public class InformationScreenData
    {
        public string Title { get; }
        public string Body { get; }
        public List<string> BulletPoints { get; }
        public Sprite IllustrationSprite { get; }

        public InformationScreenData(string title, string body, List<string> bulletPoints, Sprite sprite)
        {
            Title = title;
            Body = body;
            BulletPoints = bulletPoints ?? new List<string>();
            IllustrationSprite = sprite;
        }
    }
}

