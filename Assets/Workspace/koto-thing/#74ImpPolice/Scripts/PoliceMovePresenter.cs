using UnityEngine;

namespace Workspace.koto_thing
{
    public class PoliceMovePresenter : MonoBehaviour
    {
        [SerializeField] private PoliceMoveModel model;
        [SerializeField] private EnemyCollisionDetectionModel detectionModel;

        private void Start()
        {
            model.GetAgent.updatePosition = false;
            model.GetAgent.updateRotation = false;
        }

        private void Update()
        {
            if (detectionModel.PlayerTransform != null)
            {
                Debug.Log("Player Detected");
                model.SetDestination(detectionModel.PlayerTransform.position);
            }
            else 
            {
                Debug.Log("Player Not Detected");
                model.StopMovement();
            }
            
            model.UpdatePlanarVelocity();
            model.UpdateRotation();
            model.ApplyGravity();
        }
    }
}