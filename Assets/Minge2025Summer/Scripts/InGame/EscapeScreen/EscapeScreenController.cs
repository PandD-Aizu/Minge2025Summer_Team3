using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EscapeScreen
{
    public class EscapeScreenController : MonoBehaviour
    {
        [SerializeField] private List<CinemachineInputAxisController> cinemachineControllers;
        [SerializeField] private List<GameObject> playerPositionControllers;

        private void OnEnable()
        {
            foreach (var cinemachineController in cinemachineControllers)
                cinemachineController.enabled = false;
            
            foreach (var playerPositionController in playerPositionControllers)
                playerPositionController.SetActive(false);
        }

        private void OnDisable()
        {
            foreach (var cinemachineController in cinemachineControllers)
            {
                if (cinemachineController != null)
                    cinemachineController.enabled = true;
            }
            
            foreach (var playerPositionController in playerPositionControllers)
            {
                if (playerPositionController != null)
                    playerPositionController.SetActive(true);
            }
                
        }
    }
}