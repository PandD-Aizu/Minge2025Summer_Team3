namespace Minge2025Summer.Scripts.InGame.ObjectiveScreenScript
{
    /// <summary>
    /// 情報画面表示要求メッセージ。MessageBroker で Publish する。
    /// </summary>
    public struct ShowInformationScreen
    {
        public string Address; // JSON Addressables アドレス
        public ShowInformationScreen(string address) { Address = address; }
    }

    /// <summary>
    /// 情報画面非表示要求メッセージ。
    /// </summary>
    public struct HideInformationScreen { }
}

