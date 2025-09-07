using UnityEngine;

namespace Workspace.koto_thing
{
    public class MouseLockModel : MonoBehaviour
    {
        [SerializeField] private bool isLocked;

        public bool IsLocked { get => isLocked; set => isLocked = value; }
    }
}