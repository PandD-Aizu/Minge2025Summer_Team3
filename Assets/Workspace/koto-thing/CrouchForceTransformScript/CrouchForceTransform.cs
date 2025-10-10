using System;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class CrouchForceTransform : MonoBehaviour
    {
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                var crouchController = other.gameObject.GetComponentInChildren<PlayerPositionModel>();
                if (crouchController != null)
                {
                    crouchController.ForceCrouch = true;
                    crouchController.IsCrouching = true;
                    crouchController.ChangeColliderHeight();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                var crouchController = other.gameObject.GetComponentInChildren<PlayerPositionModel>();
                if (crouchController != null)
                    crouchController.ForceCrouch = false;
            }
        }
    }
}