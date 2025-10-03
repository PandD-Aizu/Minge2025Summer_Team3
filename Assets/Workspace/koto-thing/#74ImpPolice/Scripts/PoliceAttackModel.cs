using System;
using UniRx;
using Unity.Cinemachine;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class PoliceAttackModel : MonoBehaviour
    {
        [Header("依存関係")] 
        [SerializeField] private Transform policeTransform;
        [SerializeField] private Animator animator;

        [Header("攻撃設定")] 
        [SerializeField, Tooltip("近接攻撃が届く距離")] private float attackRange = 2.0f;
        [SerializeField, Tooltip("1回の攻撃ダメージ")] private float attackDamage = 20.0f;
        [SerializeField, Tooltip("攻撃のクールダウン(秒)")] private float attackCooldown = 1.0f;
        [SerializeField, Tooltip("攻撃時に少しだけ前進させる量")] private float lungeDistance = 0.0f;

        private float lastAttackTime = -999f;
        private static readonly int AttackHash = Animator.StringToHash("Attack");

        private readonly Subject<Unit> onAttack = new Subject<Unit>();
        public IObservable<Unit> OnAttack => onAttack;

        public float AttackRange => attackRange;

        /// <summary>
        /// 攻撃対象が攻撃範囲内にいるかどうかをチェック
        /// </summary>
        /// <param name="target">攻撃対象</param>
        /// <returns>攻撃できるかどうか</returns>
        public bool IsInRange(Transform target)
        {
            if (target == null || policeTransform == null) return false;
            return Vector3.Distance(policeTransform.position, target.position) <= attackRange;
        }

        /// <summary>
        /// 攻撃が可能かどうかチェック
        /// </summary>
        /// <returns>攻撃可能かどうか</returns>
        public bool CanAttack()
        {
            return Time.time >= lastAttackTime + attackCooldown;
        }

        /// <summary>
        /// 攻撃する
        /// </summary>
        /// <param name="target">攻撃対象</param>
        /// <returns>攻撃できたかどうか</returns>
        public bool TryAttack(Transform target)
        {
            if (target == null) return false;
            if (!IsInRange(target) || !CanAttack()) return false;
            
            if (animator != null)
            {
                animator.SetTrigger(AttackHash);
            }
            
            if (lungeDistance > 0f)
            {
                Vector3 fwd = Vector3.ProjectOnPlane((target.position - policeTransform.position).normalized, Vector3.up);
                policeTransform.position += fwd * Mathf.Min(lungeDistance, attackRange * 0.25f);
            }

            // ダメージ適用
            var hp = target.GetComponentInChildren<PlayerHpModel>();
            if (hp != null)
                hp.CurrentHp -= attackDamage;

            lastAttackTime = Time.time;
            onAttack.OnNext(Unit.Default);
            return true;
        }
    }
}
