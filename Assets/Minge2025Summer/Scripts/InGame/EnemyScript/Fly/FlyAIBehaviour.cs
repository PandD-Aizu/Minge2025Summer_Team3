using System;
using Minge2025Summer.Scripts.InGame.EnemyScript.Enum;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EnemyScript.Fly
{
    public class FlyAIBehaviour : MonoBehaviour
    {
        private ReactiveProperty<AIState> currentState = new ReactiveProperty<AIState>();
        public IObservable<AIState> CurrentStateObservable => currentState.AsObservable();
        public AIState CurrentState { get => currentState.Value; set => currentState.Value = value; }

        public void HandleIdleState(FlyMoveModel moveModel)
        {
            if (moveModel != null)
            {
                moveModel.ResetPosition();
            }
        }

        public void HandleWarningState()
        {
            
        }

        public void HandlePatrolState()
        {
            
        }

        public void HandleChaseState(Transform target, FlyMoveModel moveModel)
        {
            if (target != null)
            {
                CurrentState = AIState.Idle;
                return;
            }
            
            moveModel.SetDestination(target.position);
        }
        
        public void HandleAttackState(Transform target, FlyMoveModel moveModel, FlyAttackModel attackModel)
        {
            if (target == null)
            {
                CurrentState = AIState.Idle;
                return;
            }

            moveModel.ResetPosition();

            Vector3 toTarget = target.position - transform.position;
            toTarget.y = 0.0f;
            if (toTarget.sqrMagnitude > 0.001f)
            {
                Quaternion look = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 10.0f);
            }

            attackModel?.TryAttack(target);
        }
    }
}