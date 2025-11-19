using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.AI;

namespace Minge2025Summer.Scripts.InGame.EnemyScript.Boss
{
    public class BossPatrolModel : MonoBehaviour
    {
        [Header("巡回種別")]
        [SerializeField, Tooltip("ウェイポイント配列を使う場合 true")]
        private bool useWaypoints = false;

        [Header("ウェイポイント設定 (useWaypoints=true のとき使用)")]
        [SerializeField] private Transform[] waypoints;

        [Header("ランダム領域設定 (useWaypoints=false のとき使用)")]
        [SerializeField, Tooltip("ランダム領域の中心。未設定ならこのオブジェクト位置")]
        private Transform patrolAreaCenter;
        [SerializeField, Tooltip("ランダム領域のサイズ(X,Z)")] private Vector2 patrolAreaSize = new Vector2(20f, 20f);
        [SerializeField, Tooltip("NavMesh.SamplePosition の最大距離")]
        private float navSampleDistance = 2f;

        [Header("到着閾値")]
        [SerializeField, Tooltip("到着とみなす距離")]
        private float arrivalThreshold = 0.5f;

        private int waypointIndex;
        private Vector3 upcomingDestination;

        public Transform PatrolAreaCenter { get => patrolAreaCenter; set => patrolAreaCenter = value; }
        public Vector2 PatrolAreaSize { get => patrolAreaSize; set => patrolAreaSize = value; }
        public float NavSampleDistance { get => navSampleDistance; set => navSampleDistance = value; }
        public bool IsPatrolling { get; private set; }

        /// <summary>
        /// パトロールを開始する（NavMeshAgent が必要）
        /// </summary>
        public void StartPatrol(NavMeshAgent agent)
        {
            if (agent == null || !agent.isOnNavMesh) return;
            IsPatrolling = true;
            GoToNextPoint(agent);
        }

        /// <summary>
        /// パトロールを停止する
        /// </summary>
        public void StopPatrol()
        {
            IsPatrolling = false;
        }

        /// <summary>
        /// 次の目的地へ移動（agent に対して目的地をセットする）
        /// </summary>
        public void GoToNextPoint(NavMeshAgent agent)
        {
            if (agent == null || !agent.isOnNavMesh) return;

            if (useWaypoints)
            {
                if (waypoints == null || waypoints.Length == 0) return;
                upcomingDestination = waypoints[waypointIndex].position;
                agent.SetDestination(upcomingDestination);
                waypointIndex = (waypointIndex + 1) % waypoints.Length;
                return;
            }

            // ランダム領域から NavMesh 上の点を探す
            Vector3 center = patrolAreaCenter != null ? patrolAreaCenter.position : transform.position;
            if (TryGetRandomPointOnNavMesh(center, patrolAreaSize, navSampleDistance, out Vector3 result))
            {
                upcomingDestination = result;
                agent.SetDestination(result);
            }
        }

        /// <summary>
        /// 到着判定
        /// </summary>
        public bool IsAtDestination(NavMeshAgent agent)
        {
            if (agent == null || !agent.isOnNavMesh) return false;
            if (!agent.hasPath) return false;
            return agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, arrivalThreshold);
        }

        /// <summary>
        /// パトロールポイントの有無(waypoints)
        /// </summary>
        public bool HasWaypoints() => waypoints != null && waypoints.Length > 0;

        /// <summary>
        /// パトロールが構成されているか（ウェイポイントまたはランダム領域）
        /// </summary>
        public bool HasPatrolConfigured()
        {
            if (useWaypoints)
                return HasWaypoints();
            return patrolAreaSize.x > 0f && patrolAreaSize.y > 0f;
        }

        /// <summary>
        /// Gizmos 表示用: upcoming destination
        /// </summary>
        public Vector3 GetUpcomingDestination() => upcomingDestination;
        
        /// <summary>
        /// 互換性のための簡易 API 名
        /// </summary>
        public bool HasPatrolPoints() => HasPatrolConfigured();

        /// <summary>
        /// 指定領域内から NavMesh 上のランダムな点を取得する
        /// </summary>
        private bool TryGetRandomPointOnNavMesh(Vector3 center, Vector2 size, float maxSampleDistance, out Vector3 result)
        {
            result = Vector3.zero;
            const int maxAttempts = 20;

            // 安全な最大小半径を決める（inspector の maxSampleDistance が小さければ補正）
            float baseRadius = Mathf.Max(1f, maxSampleDistance);
            float heightTrialsOffset = 10f; // 上から降りてくるように高さを変えて試す

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                // ランダムにXZをサンプリング
                float rx = Random.Range(center.x - size.x * 0.5f, center.x + size.x * 0.5f);
                float rz = Random.Range(center.z - size.y * 0.5f, center.z + size.y * 0.5f);

                // 試行ごとに高さを変えて上からサンプリングする（高いところから下へ探索）
                float height = center.y + Mathf.Lerp(2f, heightTrialsOffset, attempt / (float)maxAttempts);
                Vector3 samplePos = new Vector3(rx, height, rz);

                // サンプリング半径も徐々に拡張
                float searchRadius = baseRadius + (attempt * 0.5f);

                if (NavMesh.SamplePosition(samplePos, out NavMeshHit hit, searchRadius, NavMesh.AllAreas))
                {
                    // ヒットした NavMesh の座標を返す
                    result = hit.position;
                    return true;
                }
            }

            // 最後に中心点近傍を念入りに探す（グリッド状）
            int grid = 5;
            for (int ix = -grid; ix <= grid; ix++)
            {
                for (int iz = -grid; iz <= grid; iz++)
                {
                    Vector3 samplePos = new Vector3(center.x + ix * (size.x / (grid * 2f)), center.y + 5f, center.z + iz * (size.y / (grid * 2f)));
                    if (NavMesh.SamplePosition(samplePos, out NavMeshHit hit, baseRadius * 2f, NavMesh.AllAreas))
                    {
                        result = hit.position;
                        return true;
                    }
                }
            }

            return false;
        }

        private void OnDrawGizmosSelected()
        {
            // 巡回領域とポイントの可視化
            Gizmos.color = Color.cyan;
            if (useWaypoints && HasWaypoints())
            {
                for (int i = 0; i < waypoints.Length; i++)
                {
                    var a = waypoints[i];
                    var b = waypoints[(i + 1) % waypoints.Length];
                    if (a == null || b == null) continue;

                    Gizmos.DrawSphere(a.position, 0.2f);
                    Gizmos.DrawLine(a.position, b.position);
                }

                Gizmos.color = Color.magenta;
                var upcoming = GetUpcomingDestination();
                if (upcoming != Vector3.zero)
                    Gizmos.DrawWireSphere(upcoming, 0.25f);
            }
            // ランダム領域
            else
            {
                Vector3 center = patrolAreaCenter != null ? patrolAreaCenter.position : transform.position;
                Vector3 size = new Vector3(patrolAreaSize.x, 0.1f, patrolAreaSize.y);
                Gizmos.DrawWireCube(center, size);

                Gizmos.color = Color.magenta;
                var upcoming = GetUpcomingDestination();
                if (upcoming != Vector3.zero)
                    Gizmos.DrawWireSphere(upcoming, 0.25f);
            }
        }
    }
}