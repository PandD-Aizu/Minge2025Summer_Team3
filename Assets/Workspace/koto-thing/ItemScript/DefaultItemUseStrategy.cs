namespace Workspace.koto_thing
{
    public class DefaultItemUseStrategy : IItemUseStrategy
    {
        /// <summary>
        /// アイテムを使えるかどうか
        /// </summary>
        /// <param name="item">アイテム</param>
        /// <returns>使えるならtrueを返す</returns>
        public bool CanUse(IItem item)
        {
            return item != null && item.SetIsGet && !item.GetIsApplied && item.GetAmount > 0;
        }

        /// <summary>
        /// アイテムを使用する
        /// </summary>
        /// <param name="item">アイテム</param>
        public void Use(IItem item)
        {
            item?.ApplyItem();
        }
    }
}