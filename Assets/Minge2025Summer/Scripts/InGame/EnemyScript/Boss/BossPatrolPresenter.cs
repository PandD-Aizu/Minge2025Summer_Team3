using System;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

namespace Minge2025Summer.Scripts.InGame.EnemyScript.Boss
{
    public class BossPatrolPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private BossPatrolModel model;
        [SerializeField] private NavMeshAgent navMeshAgent;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            if (navMeshAgent == null)
                navMeshAgent = GetComponentInParent<NavMeshAgent>();

            if (navMeshAgent == null || !navMeshAgent.isOnNavMesh)
            {
                Debug.LogWarning("BossPatrolPresenter: NavMeshAgent is not ready or not on NavMesh.", this);
                return;
            }

            model.PatrolAreaCenter ??= transform;
            model?.GoToNextPoint(navMeshAgent);
        }

        private void Update()
        {
            if (navMeshAgent == null)
                return;

            if (!navMeshAgent.isOnNavMesh)
                return;

            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= 0.5f)
                model?.GoToNextPoint(navMeshAgent);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}