using System;
using Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Interface;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript.ItemScript
{
    public class ReiItemInteractionModel : MonoBehaviour
    {
        [SerializeField, Tooltip("インタラクト可能な距離")] private float interactionRange = 3.0f;
        [SerializeField, Tooltip("インタラクト可能なレイヤー")] private LayerMask interactionLayer = ~0;

        private Subject<string> onInteractItem = new ();
        public IObservable<string> OnInteractItem => onInteractItem;
        
        public void Interact(ReiItemInventoryModel inventory)
        {
            var camera = Camera.main;
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hitInfo, interactionRange, interactionLayer))
            {
                if (hitInfo.collider.TryGetComponent<IReiItem>(out var reiItem))
                {
                    inventory.AddItem(reiItem);
                    reiItem.GetItem();
                    onInteractItem.OnNext(reiItem.GetItemName);
                }
            }
        }
    }
}