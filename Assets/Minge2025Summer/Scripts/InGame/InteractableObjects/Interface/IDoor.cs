namespace Minge2025Summer.Scripts.InGame.InteractableObjects.Interface
{
    public interface IDoor
    {
        public string RequiredKeyID { get; }
        public bool IsUnLocked { get; }
        public void UnLock();
        public bool TryOpen();
    }
}