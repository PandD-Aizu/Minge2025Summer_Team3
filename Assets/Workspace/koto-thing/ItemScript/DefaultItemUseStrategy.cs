namespace Workspace.koto_thing
{
    public class DefaultItemUseStrategy : IItemUseStrategy
    {
        public bool CanUse(IItem item)
        {
            return item != null && item.SetIsGet && !item.GetIsApplied;
        }

        public void Use(IItem item)
        {
            item?.ApplyItem();
        }
    }
}