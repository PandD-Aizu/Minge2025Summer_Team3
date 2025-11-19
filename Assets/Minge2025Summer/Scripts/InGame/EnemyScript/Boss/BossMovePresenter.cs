using System;
using Minge2025Summer.Scripts.InGame.AcousticsScript;
using Minge2025Summer.Scripts.InGame.EnemyScript.Enum;
using Minge2025Summer.Scripts.InGame.RandomMapGeneratorScript;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

namespace Minge2025Summer.Scripts.InGame.EnemyScript.Boss
{
    public class BossMovePresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private BossMoveModel model;
        [SerializeField] private BossEmitter emitter;
        [SerializeField] private BossCollisionDetectionModel detectionModel;
        [SerializeField] private BossAIBehaviour aiBehaviourModel;
        [SerializeField] private BattleBGMController battleBGMController;
        [SerializeField] private BossAttackModel attackModel;
        [SerializeField] private BossAnimationController bossAnimationController;
        
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
                .DelayFrame(1)
                .Subscribe(_ => TryInitializeAfterNavMesh())
                .AddTo(disposables);
            
            if (MapGenerator.NavMeshReady)
            {
                Observable.NextFrame()
                    .Subscribe(_ => TryInitializeAfterNavMesh())
                    .AddTo(disposables);
            }
        }

        private void TryInitializeAfterNavMesh()
        {
            if (navMeshInitialized) return;
            if (model == null || model.GetAgent == null) return;

            var agent = model.GetAgent;
            if (!agent.enabled) agent.enabled = true;
            
            if (!agent.isOnNavMesh)
            {
                if (NavMesh.SamplePosition(agent.transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                    agent.Warp(hit.position);
                else
                {
                    Vector3 fixPos = agent.transform.position;
                    fixPos.y = 0; 
                    if (NavMesh.SamplePosition(fixPos, out NavMeshHit hit2, 10f, NavMesh.AllAreas))
                        agent.Warp(hit2.position);
                }
            }

            agent.updatePosition = false;
            agent.updateRotation = false;

            if (battleBGMController == null)
                battleBGMController = GameObject.Find("BattleBGM")?.GetComponent<BattleBGMController>();

            SubscribeEvents();
            
            if (aiBehaviourModel != null)
            {
                if (model.CanPatrol())
                    aiBehaviourModel.CurrentState = AIState.Patrolling;
                else
                    aiBehaviourModel.CurrentState = AIState.Idle;
            }
            
            navMeshInitialized = true;
        }

        private void Update()
        {
            if (!navMeshInitialized) return;

            if (detectionModel != null) detectionModel.FindPlayerInVision();

            var target = detectionModel != null ? detectionModel.PlayerTransform : null;
            bool canSee = target != null;
            bool hasLkp = detectionModel != null && detectionModel.HasLastKnownPosition;
            bool inRange = canSee && attackModel != null && attackModel.IsInRange(target);
            bool canAttackNow = inRange && attackModel != null && attackModel.CanAttack();

            // 攻撃優先のステート遷移
            if (canAttackNow)
            {
                if (aiBehaviourModel != null) aiBehaviourModel.CurrentState = AIState.Attacking;
                aiBehaviourModel?.HandleAttackState(target, model, attackModel);
            }
            // 視認優先のステート遷移
            else if (canSee)
            {
                if (aiBehaviourModel != null) aiBehaviourModel.CurrentState = AIState.Chasing;
                aiBehaviourModel?.HandleChaseState(target, model);
            }
            // 最終目撃位置優先のステート遷移
            else if (hasLkp)
            {
                if (aiBehaviourModel != null) aiBehaviourModel.CurrentState = AIState.Searching;
                aiBehaviourModel?.HandleSearchState(detectionModel.LastKnownPosition, model);
                
                // 捜索地点到着判定
                var agent = model?.GetAgent;
                if (agent != null && agent.isOnNavMesh && !agent.pathPending)
                {
                    if (agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, 0.5f))
                    {
                        detectionModel.ClearLastKnownPosition();
                        // 捜索終了後、パトロール設定があればパトロールへ戻る
                        if (aiBehaviourModel != null)
                        {
                            aiBehaviourModel.CurrentState = model != null && model.CanPatrol() 
                                ? AIState.Patrolling 
                                : AIState.Idle;
                        }
                    }
                }
            }
            // ターゲット無し
            else
            {
                // ターゲット無し
                if (aiBehaviourModel != null)
                {
                    // 現在がIdle/Patrolling以外（Chasing/Searchingからの復帰時など）ならステートを戻す
                    if (aiBehaviourModel.CurrentState == AIState.Chasing || 
                        aiBehaviourModel.CurrentState == AIState.Searching ||
                        aiBehaviourModel.CurrentState == AIState.Attacking)
                    {
                        aiBehaviourModel.CurrentState = model != null && model.CanPatrol() 
                            ? AIState.Patrolling 
                            : AIState.Idle;
                    }
                    // 既に Patrolling や Idle なら何もしない（イベント購読側で処理済み）
                }
            }

            // ステートごとの継続的な更新処理
            if (aiBehaviourModel != null && aiBehaviourModel.CurrentState == AIState.Patrolling)
            {
                // 目的地に着いたら次のランダム地点へ
                if (model != null && model.IsAtPatrolDestination())
                {
                    model.GoToNextPatrolPoint();
                }
            }
            else if (aiBehaviourModel != null && aiBehaviourModel.CurrentState == AIState.Idle)
            {
                // 停止し続ける
                aiBehaviourModel.HandleIdleState(model);
            }

            // 物理移動の反映
            if (model != null)
            {
                // 攻撃中以外は動く
                if (aiBehaviourModel != null && aiBehaviourModel.CurrentState != AIState.Attacking)
                {
                    model.UpdatePlanarVelocity();
                    model.UpdateRotation();
                    model.ApplyGravity();
                    model.ApplyMovement();
                }
                else
                {
                    // 攻撃中は停止
                    model.ForceStopImmediate();
                }
            }
            
            if (emitter != null) emitter.PlayFootStep(model != null ? model.GetSpeed : 0f);
            if (emitter != null) emitter.UpdateMoanTimer();
        }
        
        private void SubscribeEvents()
        {
            if (aiBehaviourModel != null)
            {
                aiBehaviourModel.CurrentStateObservable
                    .Subscribe(nextState =>
                    {
                        switch (nextState)
                        {
                            case AIState.Idle:
                                model?.StopPatrol();
                                aiBehaviourModel.HandleIdleState(model);
                                battleBGMController?.StopBattleBGM();
                                bossAnimationController?.PlayIdle();
                                emitter?.StopMoaning();
                                break;

                            case AIState.Patrolling:
                                model?.StartPatrol(); 
                                battleBGMController?.StopBattleBGM();
                                emitter?.StopMoaning();
                                break;

                            case AIState.Chasing:
                                model?.StopPatrol();
                                aiBehaviourModel.HandleChaseState(detectionModel?.PlayerTransform, model);
                                battleBGMController?.PlayBattleBGM();
                                bossAnimationController?.PlayChase();
                                emitter?.StartMoaning();
                                break;

                            case AIState.Searching:
                                model?.StopPatrol();
                                aiBehaviourModel.HandleSearchState(detectionModel?.LastKnownPosition ?? Vector3.zero, model);
                                battleBGMController?.StopBattleBGM();
                                bossAnimationController?.PlaySearch();
                                emitter?.StartMoaning();
                                break;

                            case AIState.Attacking:
                                model?.StopPatrol();
                                aiBehaviourModel.HandleAttackState(detectionModel?.PlayerTransform, model, attackModel);
                                battleBGMController?.PlayBattleBGM();
                                emitter?.StartMoaning();
                                break;
                        }
                    })
                    .AddTo(disposables);
            }

            if (attackModel != null && bossAnimationController != null)
            {
                attackModel.OnAttack
                    .Subscribe(_ => bossAnimationController.PlayAttack())
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
            disposables?.Dispose();
        }
    }
}