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
        [SerializeField, Tooltip("攻撃アニメーションのTrigger名")] private string attackTriggerName = "AttackTrigger";

        [Header("攻撃設定")] 
        [SerializeField, Tooltip("近接攻撃が届く距離")] private float attackRange = 2.0f;
        [SerializeField, Tooltip("1回の攻撃ダメージ")] private float attackDamage = 20.0f;
        [SerializeField, Tooltip("攻撃のクールダウン(秒)")] private float attackCooldown = 1.0f;
        [SerializeField, Tooltip("攻撃時に少しだけ前進させる量")] private float lungeDistance = 0.0f;

        private float lastAttackTime = -999f;

        private readonly Subject<Unit> onAttack = new Subject<Unit>();
        public IObservable<Unit> OnAttack => onAttack; // 実際に攻撃動作が行われた瞬間

        private Transform pendingTarget;           // 現在進行中攻撃のターゲット
        private bool damageAppliedThisAttack;      // ダメージを既に適用したか

        public float AttackRange => attackRange;

        /// <summary>
        /// 攻撃対象が攻撃範囲内にいるかどうか
        /// </summary>
        public bool IsInRange(Transform target)
        {
            if (target == null || policeTransform == null) return false;
            return Vector3.Distance(policeTransform.position, target.position) <= attackRange;
        }

        /// <summary>
        /// クールダウン経過で攻撃可能か
        /// </summary>
        public bool CanAttack()
        {
            return Time.time >= lastAttackTime + attackCooldown;
        }

        /// <summary>
        /// 攻撃試行し、成功したらイベント発火
        /// </summary>
        public bool TryAttack(Transform target)
        {
            if (target == null) return false;
            if (!IsInRange(target) || !CanAttack()) return false;
            
            if (lungeDistance > 0f && policeTransform != null)
            {
                Vector3 fwd = Vector3.ProjectOnPlane((target.position - policeTransform.position).normalized, Vector3.up);
                policeTransform.position += fwd * Mathf.Min(lungeDistance, attackRange * 0.25f);
            }

            // 攻撃開始
            pendingTarget = target;
            damageAppliedThisAttack = false;
            lastAttackTime = Time.time;
            onAttack.OnNext(Unit.Default);
            return true;
        }
    }
}
