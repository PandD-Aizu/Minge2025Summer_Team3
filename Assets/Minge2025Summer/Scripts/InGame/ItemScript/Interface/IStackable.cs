namespace Minge2025Summer.Scripts.InGame.ItemScript.Interface
{
    /// <summary>
    /// インベントリ内でスタック可能なアイテムが実装するインターフェース。
    /// 実装しなければ同型でも別スロットに個別保持される。
    /// </summary>
    public interface IStackable
    {
        /// <summary>
        /// スタックを許可するかどうか。
        /// false の場合、同型でも別インスタンスとして扱う。
        /// </summary>
        bool CanStack { get; }

        /// <summary>
        /// （任意）最大スタック数。0 または負数で無制限扱い。
        /// 現状未使用だが将来拡張用。
        /// </summary>
        int MaxStack => 0;
    }
}

