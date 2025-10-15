using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.PlayerInputControllerScript
{
    public class MouseLockModel : MonoBehaviour
    {
        [SerializeField] private bool isLocked;

        public bool IsLocked { get => isLocked; set => isLocked = value; }
    }
}