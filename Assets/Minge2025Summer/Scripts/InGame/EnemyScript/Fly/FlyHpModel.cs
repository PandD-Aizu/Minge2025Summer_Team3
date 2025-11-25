using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EnemyScript.Fly
{
    public class FlyHpModel : MonoBehaviour
    {
        [Header("依存関係")] 
        [SerializeField, Tooltip("ハエの親GameObject")] private GameObject flyGameObject;

        [Header("ステータス")] 
        [SerializeField, Tooltip("体力")] private float hp = 1.0f;

        public void ReceiveDamage(float damage)
        {
            hp -= damage;
            if (hp <= 0)
            {
                Destroy(flyGameObject);
            }
        }
    }
}