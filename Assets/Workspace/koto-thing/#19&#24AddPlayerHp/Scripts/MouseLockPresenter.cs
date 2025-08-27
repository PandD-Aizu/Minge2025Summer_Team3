using UnityEngine;

namespace Workspace.koto_thing
{
    public class MouseLockPresenter : MonoBehaviour
    {
        [SerializeField] private MouseLockModel model;
        [SerializeField] private MouseLockView view;

        private void Start()
        {
            if (model.IsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = true;
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
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                model.IsLocked = !model.IsLocked;
            }
        }
    }
}