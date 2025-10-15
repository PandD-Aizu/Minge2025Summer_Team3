using Minge2025Summer.Scripts.InGame.PlayerTransformScript;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.CrouchForceTransformScript
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