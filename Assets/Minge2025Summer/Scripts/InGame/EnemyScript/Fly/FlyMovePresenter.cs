using System;
using Minge2025Summer.Scripts.InGame.AcousticsScript;
using Minge2025Summer.Scripts.InGame.EnemyScript.Enum;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EnemyScript.Fly
{
    public class FlyMovePresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private FlyMoveModel model;
        [SerializeField] private FlyEmitter emitter;
        [SerializeField] private FlyCollisionDetectionModel detectionModel;
        [SerializeField] private FlyHpModel hpModel;
        [SerializeField] private FlyAIBehaviour aiBehaviourModel;
        [SerializeField] private BattleBGMController battleBGMController;
        [SerializeField] private FlyAttackModel attackModel;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            battleBGMController = GameObject.Find("BattleBGM")?.GetComponent<BattleBGMController>();
            if (battleBGMController == null)
                Debug.LogWarning("BattleBGMController がシーンに見つかりません");

            SubscribeEvents();
        }

        private void Update()
        {
            detectionModel.FindPlayerInLights();
            var target = detectionModel.TargetPlayer;
            bool inRange = attackModel != null && attackModel.IsInRange(target);
            bool canAttackNow = inRange && attackModel != null && attackModel.CanAttack();

            if (canAttackNow)
            {
                aiBehaviourModel.CurrentState = AIState.Attacking;
                aiBehaviourModel.HandleAttackState(target, model, attackModel);
            }
            else if (target != null)
            {
                aiBehaviourModel.CurrentState = AIState.Chasing;
                aiBehaviourModel.HandleChaseState(target, model);
            }
            else
            {
                aiBehaviourModel.CurrentState = AIState.Idle;
                aiBehaviourModel.HandleIdleState(model);
            }

            if (aiBehaviourModel.CurrentState == AIState.Attacking)
            {
                model.ForceStopImmediate();
            }
            else
            {
                model.UpdateVelocity();
            }
            
            emitter.PlayFlyFlySound();
        }

        public void SubscribeEvents()
        {
            aiBehaviourModel.CurrentStateObservable
                .Subscribe(state =>
                {
                    switch (state)
                    {
                        case AIState.Idle:
                            aiBehaviourModel.HandleIdleState(model);
                            battleBGMController?.StopBattleBGM();
                            break;
                        case AIState.Chasing:
                            aiBehaviourModel.HandleChaseState(detectionModel.TargetPlayer, model);
                            battleBGMController?.PlayBattleBGM();
                            break;
                        case AIState.Attacking:
                            aiBehaviourModel.HandleAttackState(detectionModel.TargetPlayer, model, attackModel);
                            battleBGMController?.PlayBattleBGM();
                            break;
                    }
                })
                .AddTo(disposables);
        }

        private void OnDestroy()
        {
            Dispose();
            battleBGMController?.StopBattleBGM();
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}