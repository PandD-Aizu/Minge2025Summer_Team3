using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EnemyScript.Boss
{
    public class BossCollisionDetectionPresenter : MonoBehaviour
    {
        [SerializeField] private BossCollisionDetectionModel model;

        private void Update()
        {
            model.FindPlayerInVision();
        }
    }
}