using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.AI;
using Minge2025Summer.Scripts.InGame.EnemyScript;

namespace Minge2025Summer.Scripts.InGame.SpecialEventTriggerScript
{
    public class EnemySpawnFireStarter : MonoBehaviour
    {
        [SerializeField, Tooltip("スポーンさせる敵のプレハブ")]
        private GameObject enemyPrefab;
        
        [SerializeField, Tooltip("スポーンさせる位置")]
        private Transform spawnPoint;

        [SerializeField, Tooltip("再生する音")]
        private List<StudioEventEmitter> spawnSoundEmitters;
        
        private bool isAlreadySpawned = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || isAlreadySpawned) 
                return;

            // 敵をスポーン
            var enemyObject = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            // NavMeshAgentを取得
            if (!enemyObject.TryGetComponent(out NavMeshAgent agent))
                agent = enemyObject.GetComponentInChildren<NavMeshAgent>();

            // エラー処理
            if (agent == null)
            {
                Debug.LogError("[EnemySpawnFireStarter] NavMeshAgent が見つかりません。");
                return;
            }

            // スポーン位置をNavMesh上に補正
            if (NavMesh.SamplePosition(spawnPoint.position, out var hit, 2f, NavMesh.AllAreas))
                agent.Warp(hit.position);
            else
                Debug.LogWarning("[EnemySpawnFireStarter] スポーン位置がNavMesh外です。近傍のNavMeshが見つかりません。");

            // プレイヤーを検知 & 発覚状態にする
            var playerTransform = other.transform;

            // 検知モデルに強制検知を指示して即追跡に入れる
            var detection = enemyObject.GetComponentInChildren<PoliceCollisionDetectionModel>();
            if (detection != null)
            {
                detection.ForceDetect(playerTransform, 2.0f);
            }

            // 既存のメッセージ経由の目的地設定(対応実装があれば併用)
            var alertMethods = new[] { "SetDestination" };
            foreach (var method in alertMethods)
                enemyObject.BroadcastMessage(method, playerTransform.position, SendMessageOptions.DontRequireReceiver);

            // NavMeshAgentでもプレイヤー方向へ移動開始
            agent.isStopped = false;
            var ok = agent.SetDestination(playerTransform.position);
            if (!ok) 
                Debug.LogWarning("[EnemySpawnFireStarter] SetDestination に失敗しました。（到達不能 or NavMesh外）");

            // 音再生
            foreach (var emitter in spawnSoundEmitters)
                emitter.Play();

            isAlreadySpawned = true;
        }
        
        private void OnDrawGizmos()
        {
            if (spawnPoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
                Gizmos.DrawLine(transform.position, spawnPoint.position);
            }
        }
    }
}