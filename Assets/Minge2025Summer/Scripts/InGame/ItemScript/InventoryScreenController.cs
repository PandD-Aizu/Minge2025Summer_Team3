using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ItemScript
{
    public class InventoryScreenController : MonoBehaviour
    {
        [SerializeField] private InventoryScreenView view;
        [SerializeField] private List<GameObject> playerMovementControllers;
        [SerializeField] private List<CinemachineInputAxisController> inputAxisControllers;

        private void OnEnable()
        {
            // 視点移動を無効化
            foreach (var cinemachineInputAxisController in inputAxisControllers)
                cinemachineInputAxisController.enabled = false;
            
            // プレイヤー移動を無効化
            foreach (var controller in playerMovementControllers)
                controller.SetActive(false);
        }
        
        private void OnDisable()
        {
            // 視点移動を有効化
            foreach (var cinemachineInputAxisController in inputAxisControllers)
                cinemachineInputAxisController.enabled = true;
            
            // プレイヤー移動を有効化
            foreach (var controller in playerMovementControllers)
                controller.SetActive(true);
        }
    }
}