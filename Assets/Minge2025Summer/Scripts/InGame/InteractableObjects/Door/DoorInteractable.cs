using Minge2025Summer.InGame.Interactable;
using Minge2025Summer.Scripts.InGame.InteractableObjects.Interface;
using Minge2025Summer.Scripts.InGame.KeySystemScript;
using Minge2025Summer.Scripts.InGame.ReiScript.ItemScript;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.InteractableObjects.Door
{
    public class DoorInteractable : MonoBehaviour, IInteractable
    {
        private IDoor door;

        private void Awake()
        {
            door = GetComponent<IDoor>();
        }

        public string InteractionPrompt
        {
            get
            {
                if (door == null) return "Interact";
                return door.IsUnLocked ? "開ける" : "閉じる";
            }
        }

        public void Interact(GameObject instigator = null)
        {
            if (door == null || instigator == null) return;

            var keySys = instigator.GetComponent<PlayerKeySysModel>() ?? instigator.GetComponentInChildren<PlayerKeySysModel>() ?? instigator.GetComponentInParent<PlayerKeySysModel>();
            var inventory = instigator.GetComponent<ReiItemInventoryModel>() ?? instigator.GetComponentInChildren<ReiItemInventoryModel>() ?? instigator.GetComponentInParent<ReiItemInventoryModel>();

            if (keySys != null)
            {
                keySys.InteractWithDoor(door, inventory);
            }
            else
            {
                Debug.LogWarning("PlayerKeySysModel not found on instigator.");
            }
        }
    }
}
