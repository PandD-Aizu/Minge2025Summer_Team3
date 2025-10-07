using UnityEngine;

namespace Workspace.koto_thing
{
    public class EnemyColliderInfo : MonoBehaviour
    {
        [SerializeField] private EnemyBodyParts bodyParts;
        [SerializeField] private float damageMultiplier = 1.0f;
        
        public EnemyBodyParts BodyParts => bodyParts;
        public float DamageMultiplier => damageMultiplier; // 追加: 部位ごとのダメージ倍率
    }
}