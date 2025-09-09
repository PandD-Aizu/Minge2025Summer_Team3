using UnityEngine;

namespace Workspace.koto_thing
{
    public class EnemyCollisionDetectionPresenter : MonoBehaviour
    {
        [SerializeField] private EnemyCollisionDetectionModel model;
        
        private void Update()
        {
            model.FindPlayerInVision();
        }
    }
}