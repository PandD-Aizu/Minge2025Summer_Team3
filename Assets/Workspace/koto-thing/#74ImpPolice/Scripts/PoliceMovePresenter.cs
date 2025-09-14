using System;
using UniRx;
using UnityEngine;
using Workspace.momiji1107;

namespace Workspace.koto_thing
{
    public class PoliceMovePresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private PoliceMoveModel model;
        [SerializeField] private PoliceEmitter emitter;
        [SerializeField] private PoliceCollisionDetectionModel detectionModel;
        [SerializeField] private PoliceStatusModel statusModel;
        [SerializeField] private PoliceAIBehaviour aiBehaviourModel;
        [SerializeField] private BattleBGMController battleBGMController;
        [SerializeField] private PoliceAttackModel attackModel;

        private CompositeDisposable disposables = new CompositeDisposable();

        private void Start()
        {
            model.GetAgent.updatePosition = false;
            model.GetAgent.updateRotation = false;
            
            battleBGMController = GameObject.Find("BattleBGM").GetComponent<BattleBGMController>();
            
            aiBehaviourModel.CurrentState = AIState.Idle;
            SubscribeEvents();
        }

        private void Update()
        {
            // 検知更新
            detectionModel.FindPlayerInVision();

            // 状態判定(優先度: Attacking > Chasing > Searching > Idle)
            var target = detectionModel.PlayerTransform;
            bool canSee = target != null;
            bool hasLkp = detectionModel.HasLastKnownPosition;

            if (canSee && attackModel != null && attackModel.IsInRange(target))
            {
                aiBehaviourModel.CurrentState = AIState.Attacking;
                aiBehaviourModel.HandleAttackState(target, model, attackModel);
            }
            else if (canSee)
            {
                aiBehaviourModel.CurrentState = AIState.Chasing;
                aiBehaviourModel.HandleChaseState(target, model);
            }
            else if (hasLkp)
            {
                aiBehaviourModel.CurrentState = AIState.Searching;
                aiBehaviourModel.HandleSearchState(detectionModel.LastKnownPosition, model);

                // 到達判定
                var agent = model.GetAgent;
                if (agent != null && agent.isOnNavMesh && !agent.pathPending)
                {
                    float arriveThreshold = Mathf.Max(agent.stoppingDistance, 0.2f);
                    if (agent.remainingDistance <= arriveThreshold)
                    {
                        detectionModel.ClearLastKnownPosition();
                        aiBehaviourModel.CurrentState = AIState.Idle;
                        aiBehaviourModel.HandleIdleState(model);
                    }
                }
            }
            else
            {
                aiBehaviourModel.CurrentState = AIState.Idle;
                aiBehaviourModel.HandleIdleState(model);
            }

            // モーション更新と適用
            model.UpdatePlanarVelocity();
            model.UpdateRotation();
            model.ApplyGravity();
            model.ApplyMovement();
            
            emitter.PlayFootStep(model.GetSpeed);
            emitter.UpdateMoanTimer();
        }

        private void SubscribeEvents()
        {
            aiBehaviourModel.CurrentStateObservable
                .Subscribe(nextState =>
                {
                    switch (nextState)
                    {
                        case AIState.Idle:
                            aiBehaviourModel.HandleIdleState(model);
                            battleBGMController.StopBattleBGM();
                            break;
                        case AIState.Chasing:
                            aiBehaviourModel.HandleChaseState(detectionModel.PlayerTransform, model);
                            battleBGMController.PlayBattleBGM();
                            break;
                        case AIState.Searching:
                            aiBehaviourModel.HandleSearchState(detectionModel.LastKnownPosition, model);
                            battleBGMController.StopBattleBGM();
                            break;
                        case AIState.Attacking:
                            aiBehaviourModel.HandleAttackState(detectionModel.PlayerTransform, model, attackModel);
                            battleBGMController.PlayBattleBGM();
                            break;
                    }
                })
                .AddTo(disposables);
        }
        
        private void OnDestroy()
        {
            Dispose();
            battleBGMController.StopBattleBGM();
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}