using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EnemyScript
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