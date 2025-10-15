using Minge2025Summer.Scripts.InGame.EnemyScript.Enum;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EnemyScript.ColliderInfo
{
    public class EnemyColliderInfo : MonoBehaviour
    {
        [SerializeField] private EnemyBodyParts bodyParts;
        [SerializeField] private float damageMultiplier = 1.0f;
        
        public EnemyBodyParts BodyParts => bodyParts;
        public float DamageMultiplier => damageMultiplier; // 追加: 部位ごとのダメージ倍率
    }
}