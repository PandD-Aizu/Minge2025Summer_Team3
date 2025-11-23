using System;
using Minge2025Summer.Scripts.InGame.PlayerStatusScript;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EnemyScript.Boss
{
    public class BossAttackModel : MonoBehaviour
    {
        [Header("依存関係")] 
        [SerializeField] private Transform bossTransform;
        [SerializeField] private Animator animator;
        [SerializeField, Tooltip("攻撃アニメーションのTrigger名")] private string attackTriggerName = "AttackTrigger";
        
        [Header("攻撃設定")] 
        [SerializeField, Tooltip("近接攻撃が届く距離")] private float attackRange = 2.0f;
        [SerializeField, Tooltip("1回の攻撃ダメージ")] private float attackDamage = 20.0f;
        [SerializeField, Tooltip("攻撃のクールダウン(秒)")] private float attackCooldown = 1.0f;
        [SerializeField, Tooltip("攻撃時に少しだけ前進させる量")] private float lungeDistance = 0.0f;
        
        private readonly Subject<Unit> onAttack = new Subject<Unit>();
        public IObservable<Unit> OnAttack => onAttack;

        private Transform pendingTarget;
        private float lastAttackTime = -1.0f;
        private bool damageAppliedThisAttack;

        public float AttackRange => attackRange;

        public bool IsInRange(Transform target)
        {
            if (target == null || bossTransform == null)
                return false;

            return Vector3.Distance(bossTransform.position, target.position) <= attackRange;
        }

        public bool CanAttack()
        {
            return Time.time >= lastAttackTime + attackCooldown;
        }

        public bool TryAttack(Transform target)
        {
            if (target == null)
                return false;

            if (!IsInRange(target) || !CanAttack())
                return false;

            // 前進処理
            if (lungeDistance > 0.0f && bossTransform != null)
            {
                Vector3 fwd = Vector3.ProjectOnPlane((target.position - bossTransform.position).normalized, Vector3.up);
                bossTransform.position += fwd * Mathf.Min(lungeDistance, attackRange * 0.25f);
            }

            pendingTarget = target;
            damageAppliedThisAttack = false;
            lastAttackTime = Time.time;
            onAttack.OnNext(Unit.Default);

            // ダメージ適用: PlayerHpModel が見つかれば安全に減算する
            var playerHp = pendingTarget.GetComponentInChildren<PlayerHpModel>();
            if (playerHp != null)
            {
                playerHp.CurrentHp -= attackDamage;
                damageAppliedThisAttack = true;
            }
            else
            {
                Debug.LogWarning("BossAttackModel: ターゲットに PlayerHpModel が見つかりませんでした。ダメージは適用されません。", this);
            }

            return true;
        }
    }
}