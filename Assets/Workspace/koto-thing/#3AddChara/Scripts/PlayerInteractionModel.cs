using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerInteractionModel : MonoBehaviour
    {
        [Header("インタラクトできる距離")] 
        [SerializeField] private float interactDistance = 3.0f;
        
        // TODO: ReactivePropertyの型をIInteractableに変更する
        private ReactiveProperty<bool> isInteracting = new (false);
        public IObservable<bool> InteractObserver => isInteracting.AsObservable();

        /// <summary>
        /// オブジェクトにインタラクトする
        /// </summary>
        public void Interact()
        {
            // Eキーが押されたときにインタラクトを試みる
            Physics.Raycast(Camera.main.transform.position, 
                Camera.main.transform.forward, 
                out RaycastHit hitInfo, 
                interactDistance);

            if (hitInfo.collider != null &&
                hitInfo.collider.CompareTag("Interactable"))
            {
                
            }
        }
    }
}