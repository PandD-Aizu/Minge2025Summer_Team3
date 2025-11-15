using System;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript.GunScript
{
    public class WeaponInteractionModel : MonoBehaviour
    {
        [SerializeField, Tooltip("インタラクション可能な距離")]
        private float interactionRange = 2.0f;
        
        [SerializeField, Tooltip("インタラクション可能なレイヤーマスク")]
        private LayerMask interactionLayerMask = ~0;

        [SerializeField, Tooltip("武器をつけるアタッチポイント")]
        private Transform weaponAttachPoint;

        private Subject<IWeapon> onWeaponGet = new ();
        public IObservable<IWeapon> OnWeaponGet => onWeaponGet;
        
        public void Interact()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                Debug.LogWarning("[WeaponInteractionModel] Main camera not found");
                return;
            }

            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out var hit, interactionRange, interactionLayerMask))
            {
                if (hit.collider == null)
                    return;

                if (hit.collider.TryGetComponent<IWeapon>(out var weapon))
                {
                    var interactableObject = hit.collider.gameObject;

                    if (weaponAttachPoint != null)
                    {
                        interactableObject.transform.SetParent(weaponAttachPoint);
                        interactableObject.transform.localPosition = Vector3.zero;
                        interactableObject.transform.localRotation = Quaternion.identity;
                    }
                    else
                    {
                        Debug.LogWarning("[WeaponInteractionModel] weaponAttachPoint is null. Skipping parenting.");
                    }
                    
                    onWeaponGet.OnNext(weapon);
                }
            }
        }
    }
}