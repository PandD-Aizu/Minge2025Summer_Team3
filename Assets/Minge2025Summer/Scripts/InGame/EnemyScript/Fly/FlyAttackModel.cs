using System;
using Minge2025Summer.Scripts.InGame.PlayerStatusScript;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EnemyScript.Fly
{
    public class FlyAttackModel : MonoBehaviour
    {
        [Header("依存関係")]
        [SerializeField] private Transform flyTransform;

        [Header("攻撃設定")] 
        [SerializeField, Tooltip("近接攻撃が届く距離")] private float attackRange = 0.1f;
        [SerializeField, Tooltip("１回の攻撃ダメージ")] private float attackDamage = 1.0f;
        [SerializeField, Tooltip("攻撃のクールダウン(秒)")] private float attackCooldown = 1.0f;
        [SerializeField, Tooltip("攻撃時に少しだけ前進させる量")] private float lungeDistance = 0.0f;

        private float lastAttackTime = -999.0f;

        private readonly Subject<Unit> onAttack = new Subject<Unit>();
        public IObservable<Unit> OnAttack => onAttack;

        private Transform pendingTarget;
        private bool damageAppliedThisAttack;

        public float AttackRange => attackRange;

        // 攻撃対象が攻撃範囲内にいるかどうか
        public bool IsInRange(Transform target)
        {
            if (target == null || flyTransform == null)
                return false;

            return Vector3.Distance(flyTransform.position, target.position) <= attackRange;
        }

        // クールダウン経過で攻撃可能か
        public bool CanAttack()
        {
            return Time.time >= lastAttackTime + attackCooldown;
        }

        // 攻撃試行し、成功したらイベント発火
        public bool TryAttack(Transform target)
        {
            // 攻撃可能か確認
            if (target == null)
                return false;

            // 攻撃範囲内かつクールダウン経過しているか
            if (!IsInRange(target) || !CanAttack())
                return false;

            // 突進処理
            if (lungeDistance > 0.0f && flyTransform != null)
            {
                Vector3 fwd = Vector3.ProjectOnPlane((target.position - flyTransform.position).normalized, Vector3.up);
                flyTransform.position += fwd * Mathf.Min(lungeDistance, attackRange * 0.25f);
            }

            // 攻撃開始
            pendingTarget = target;
            damageAppliedThisAttack = false;
            lastAttackTime = Time.time;
            onAttack.OnNext(Unit.Default);
            pendingTarget.GetComponentInChildren<PlayerHpModel>().CurrentHp -= attackDamage;
            return true;
        }
    }
}