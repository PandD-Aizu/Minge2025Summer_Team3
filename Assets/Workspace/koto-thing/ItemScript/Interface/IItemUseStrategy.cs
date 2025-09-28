namespace Workspace.koto_thing
{
    public interface IItemUseStrategy
    {
        public bool CanUse(IItem item);
        public void Use(IItem item);
    }
}