using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.PlayerInputControllerScript
{
    public class MouseLockModel : MonoBehaviour
    {
        [SerializeField] private bool isLocked;

        public bool IsLocked { get => isLocked; set => isLocked = value; }
        
        public static void SetCursorState(bool isLocked)
        {
            if (isLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}