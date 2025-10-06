using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class PoliceAIBehaviour : MonoBehaviour
    {
        private ReactiveProperty<AIState> currentState = new  ReactiveProperty<AIState>();
        public IObservable<AIState> CurrentStateObservable => currentState.AsObservable();
        
        public AIState CurrentState { get => currentState.Value; set => currentState.Value = value; }
        
        public void HandleIdleState(PoliceMoveModel moveModel)
        {
            if (moveModel != null)
            {
                moveModel.StopMovement();
            }
        }

        public void HandleWarningState()
        {
            
        }

        public void HandlePatrolState()
        {
            
        }
        
        public void HandleChaseState(Transform target, PoliceMoveModel moveModel)
        {
            if (target == null)
            {
                CurrentState = AIState.Idle;
                return;
            }
            
            moveModel.SetDestination(target.position);
        }

        public void HandleSearchState(Vector3 lastKnownPosition, PoliceMoveModel moveModel)
        {
            moveModel.SetDestination(lastKnownPosition);
        }
        
        public void HandleAttackState(Transform target, PoliceMoveModel moveModel, PoliceAttackModel attackModel)
        {
            if (target == null)
            {
                CurrentState = AIState.Idle;
                return;
            }

            // 目的地を現在地にして足を止める(回頭はMoveModel側)
            moveModel.StopMovement();

            // 正面を向く
            Vector3 toTarget = target.position - transform.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude > 0.001f)
            {
                Quaternion look = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 10f);
            }

            // 攻撃実行
            attackModel?.TryAttack(target);
        }
    }
}