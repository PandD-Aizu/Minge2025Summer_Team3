using UnityEngine;

namespace Workspace.koto_thing
{
    public class PortalCameraModel : MonoBehaviour
    {
        [SerializeField] private Transform playerCamera;
        [SerializeField] private Transform entryPortal;
        [SerializeField] private Transform exitPortal;
        
        public Transform PlayerCamera => playerCamera;
        public Transform EntryPortal => entryPortal;
        public Transform ExitPortal => exitPortal;
    }
}