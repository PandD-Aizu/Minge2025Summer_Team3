using System;
using Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Interface;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript.ItemScript
{
    public class ReiItemInteractionModel : MonoBehaviour
    {
        [SerializeField, Tooltip("インタラクト可能な距離")] private float interactionRange = 3.0f;

        private Subject<string> onInteractItem = new ();
        public IObservable<string> OnInteractItem => onInteractItem;
        
        public void Interact(ReiItemInventoryModel inventory)
        {
            var camera = Camera.main;
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hitInfo, interactionRange))
            {
                if (hitInfo.collider.TryGetComponent<IConsumableItem>(out var reiItem))
                {
                    inventory.AddItem(reiItem);
                    
                    onInteractItem.OnNext(reiItem.GetItemName);
                }
            }
        }
    }
}