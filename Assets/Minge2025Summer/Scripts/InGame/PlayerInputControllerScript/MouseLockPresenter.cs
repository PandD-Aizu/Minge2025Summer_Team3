using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.PlayerInputControllerScript
{
    public class MouseLockPresenter : MonoBehaviour
    {
        [SerializeField] private MouseLockModel model;
        [SerializeField] private MouseLockView view;

        private void Start()
        {
            model.IsLocked = true;
            
            if (model.IsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void Update()
        {
            if (model.IsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.L))
            {
                model.IsLocked = !model.IsLocked;
            }
        }
    }
}