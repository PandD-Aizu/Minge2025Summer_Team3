using UnityEngine;

namespace Workspace.koto_thing
{
    public class PoliceCollisionDetectionPresenter : MonoBehaviour
    {
        [SerializeField] private PoliceCollisionDetectionModel model;
        
        private void Update()
        {
            model.FindPlayerInVision();
        }
    }
}