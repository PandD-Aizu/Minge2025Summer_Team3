using System;
using Minge2025Summer.Scripts.InGame.EnemyScript.Enum;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EnemyScript.Boss
{
    public class BossAIBehaviour : MonoBehaviour
    {
        private ReactiveProperty<AIState> currentState = new ReactiveProperty<AIState>();
        public IObservable<AIState> CurrentStateObservable => currentState.AsObservable();
        public AIState CurrentState { get => currentState.Value; set => currentState.Value = value; }

        public void HandleIdleState(BossMoveModel moveModel)
        {
            if (moveModel != null)
                moveModel.StopMovement();
        }

        public void HandleWarningState()
        {
            // 未実装のため空のプレースホルダ
        }

        public void HandlePatrolState()
        {
            // 未実装のため空のプレースホルダ
        }

        public void HandleChaseState(Transform target, BossMoveModel moveModel)
        {
            if (moveModel == null || target == null)
                return;
            
            moveModel.SetDestination(target.position);
        }

        public void HandleSearchState(Vector3 lastKnownPosition, BossMoveModel moveModel)
        {
            if (moveModel == null) 
                return;
            
            moveModel.SetDestination(lastKnownPosition);
        }

        public void HandleAttackState(Transform target, BossMoveModel moveModel, BossAttackModel attackModel)
        {
            if (attackModel == null || moveModel == null || target == null) 
                return;
            
            // 近距離なら攻撃を試みる
            if (attackModel.IsInRange(target))
            {
                attackModel.TryAttack(target);
            }
            else
            {
                moveModel.SetDestination(target.position);
            }
        }
    }
}