namespace Workspace.koto_thing
{
    public interface IDoor
    {
        public string RequiredKeyID { get; }
        public bool IsUnLocked { get; }
        public void UnLock();
        public bool TryOpen();
    }
}