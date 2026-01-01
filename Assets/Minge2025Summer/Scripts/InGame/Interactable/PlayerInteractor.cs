using UnityEngine;
using UniRx;

namespace Minge2025Summer.InGame.Interactable
{
    public class PlayerInteractor : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private float interactionDistance = 2.0f;
        [SerializeField] private LayerMask interactableLayer;

        [Header("Dependencies")]
        [SerializeField] private Camera mainCamera;

        private readonly ReactiveProperty<IInteractable> _currentInteractable = new();
        public IReadOnlyReactiveProperty<IInteractable> CurrentInteractable => _currentInteractable;

        private void Update()
        {
            CheckForInteractable();
            HandleInteractionInput();
        }

        /// <summary>
        /// インタラクト可能なオブジェクトをチェックする
        /// </summary>
        private void CheckForInteractable()
        {
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableLayer))
            {
                // インタラクト可能なオブジェクトを取得
                IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

                if (interactable != null)
                {
                    if (interactable != _currentInteractable.Value)
                        _currentInteractable.Value = interactable;
                    
                    return;
                }
            }

            // インタラクト可能なオブジェクトが見つからなかった場合
            if (_currentInteractable.Value != null)
            {
                Debug.Log("No longer in range of an interactable.");
                _currentInteractable.Value = null;
            }
        }

        /// <summary>
        /// インタラクト入力を処理する
        /// </summary>
        private void HandleInteractionInput()
        {
            if (Input.GetKeyDown(KeyCode.E) && _currentInteractable.Value != null)
            {
                _currentInteractable.Value.Interact(gameObject);
            }
        }
        
        private void OnDrawGizmos()
        {
            if (mainCamera != null)
            {
                Gizmos.color = Color.yellow;
                Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
                Gizmos.DrawRay(ray.origin, ray.direction * interactionDistance);
            }
        }
    }
}
