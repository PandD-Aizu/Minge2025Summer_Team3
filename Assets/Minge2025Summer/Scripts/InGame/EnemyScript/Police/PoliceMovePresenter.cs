using System;
using Minge2025Summer.Scripts.InGame.AcousticsScript;
using Minge2025Summer.Scripts.InGame.EnemyScript.Enum;
using Minge2025Summer.Scripts.InGame.RandomMapGeneratorScript;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

namespace Minge2025Summer.Scripts.InGame.EnemyScript
{
    public class PoliceMovePresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private PoliceMoveModel model;
        [SerializeField] private PoliceEmitter emitter;
        [SerializeField] private PoliceCollisionDetectionModel detectionModel;
        [SerializeField] private PoliceHPModel hpModel;
        [SerializeField] private PoliceAIBehaviour aiBehaviourModel;
        [SerializeField] private BattleBGMController battleBGMController;
        [SerializeField] private PoliceAttackModel attackModel;
        [SerializeField] private PoliceAnimationController policeAnimationController;

        private CompositeDisposable disposables = new CompositeDisposable();
        private bool navMeshInitialized;

        private void Awake()
        {
            if (model != null && model.GetAgent != null && !MapGenerator.NavMeshReady)
                model.GetAgent.enabled = false;
        }

        private void Start()
        {
            MapGenerator.NavMeshReadyAsObservable()
                .Take(1)
                .Subscribe(_ => TryInitializeAfterNavMesh())
                .AddTo(disposables);
            
            if (MapGenerator.NavMeshReady)
            {
                TryInitializeAfterNavMesh();
            }
        }

        private void TryInitializeAfterNavMesh()
        {
            if (navMeshInitialized) return;
            if (model == null || model.GetAgent == null) return;
            if (!MapGenerator.NavMeshReady) return;

            var agent = model.GetAgent;
            if (!agent.enabled)
            {
                agent.enabled = true;
            }

            if (!agent.isOnNavMesh)
            {
                if (NavMesh.SamplePosition(agent.transform.position, out NavMeshHit hit, 5f, agent.areaMask))
                {
                    agent.Warp(hit.position);
                }
                else
                {
                    Debug.LogWarning($"PoliceMovePresenter: NavMesh上に配置できませんでした: {agent.name}");
                }
            }

            agent.updatePosition = false;
            agent.updateRotation = false;

            battleBGMController = GameObject.Find("BattleBGM")?.GetComponent<BattleBGMController>();
            if (battleBGMController == null)
                Debug.LogWarning("BattleBGMController がシーンに見つかりません");

            aiBehaviourModel.CurrentState = AIState.Idle;
            SubscribeEvents();
            navMeshInitialized = true;
        }

        private void Update()
        {
            if (!navMeshInitialized) return;

            detectionModel.FindPlayerInVision();
            var target = detectionModel.PlayerTransform;
            bool canSee = target != null;
            bool hasLkp = detectionModel.HasLastKnownPosition;
            bool inRange = canSee && attackModel != null && attackModel.IsInRange(target);
            bool canAttackNow = inRange && attackModel != null && attackModel.CanAttack();

            // 状態判定(優先度: Attacking > Chasing > Searching > Idle)
            if (canAttackNow)
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

            if (aiBehaviourModel.CurrentState == AIState.Attacking)
            {
                model.ForceStopImmediate();
            }
            else
            {
                model.UpdatePlanarVelocity();
                model.UpdateRotation();
                model.ApplyGravity();
                model.ApplyMovement();
                emitter.PlayFootStep(model.GetSpeed);
            }

            emitter.UpdateMoanTimer();
        }

        private void SubscribeEvents()
        {
            if (disposables.Count > 1) return;

            aiBehaviourModel.CurrentStateObservable
                .Subscribe(nextState =>
                {
                    Debug.Log("Next PoliceState: " + nextState);
                    switch (nextState)
                    {
                        case AIState.Idle:
                            aiBehaviourModel.HandleIdleState(model);
                            battleBGMController?.StopBattleBGM();
                            policeAnimationController.PlayIdle();
                            emitter?.StopMoaning();
                            break;
                        case AIState.Chasing:
                            aiBehaviourModel.HandleChaseState(detectionModel.PlayerTransform, model);
                            battleBGMController?.PlayBattleBGM();
                            policeAnimationController.PlayChase();
                            emitter?.StartMoaning();
                            break;
                        case AIState.Searching:
                            aiBehaviourModel.HandleSearchState(detectionModel.LastKnownPosition, model);
                            battleBGMController?.StopBattleBGM();
                            policeAnimationController.PlaySearch();
                            emitter?.StartMoaning();
                            break;
                        case AIState.Attacking:
                            aiBehaviourModel.HandleAttackState(detectionModel.PlayerTransform, model, attackModel);
                            battleBGMController?.PlayBattleBGM();
                            emitter?.StartMoaning();
                            break;
                    }
                })
                .AddTo(disposables);

            if (attackModel != null)
            {
                attackModel.OnAttack
                    .Subscribe(_ => policeAnimationController.PlayAttack())
                    .AddTo(disposables);
            }
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