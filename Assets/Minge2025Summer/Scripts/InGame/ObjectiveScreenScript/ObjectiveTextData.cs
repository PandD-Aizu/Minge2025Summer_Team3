using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ObjectiveScreenScript
{
    /// <summary>
    /// 情報画面で表示する最終的なデータ（パース後、Sprite 解決済み）。
    /// </summary>
    public class ObjectiveTextData
    {
        public string Title { get; }
        public string SubTitle { get; }
        public string Body { get; }
        public Sprite IllustrationSprite { get; }

        public ObjectiveTextData(string title, string subTitle, string body, Sprite sprite)
        {
            Title = title;
            SubTitle = subTitle;
            Body = body;
            IllustrationSprite = sprite;
        }
    }
}

