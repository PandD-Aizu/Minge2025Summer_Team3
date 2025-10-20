using System.Collections.Generic;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EnemyScript
{
    public class EnemySpawn : MonoBehaviour
    {
        [Header("出現位置")] 
        [SerializeField] private List<Transform> spawnPoints;

        [Header("出現させる敵のプレハブ")] 
        [SerializeField] private List<GameObject> enemyPrefabs;

        /// <summary>
        /// 敵を出現させる
        /// </summary>
        public void SpawnEnemies()
        {
            // ログで現在の状態を出力
            Debug.Log($"[EnemySpawn] Spawn called on '{gameObject.name}'. spawnPoints={spawnPoints?.Count ?? 0}, enemyPrefabs={enemyPrefabs?.Count ?? 0}");

            if (spawnPoints == null || spawnPoints.Count == 0)
            {
                Debug.LogWarning($"[EnemySpawn] No spawn points set on '{gameObject.name}'.");
                return;
            }

            if (enemyPrefabs == null || enemyPrefabs.Count == 0)
            {
                Debug.LogWarning($"[EnemySpawn] No enemy prefabs assigned on '{gameObject.name}'.");
                return;
            }

            // 親オブジェクトを取得。MapContainer を親に使えるならそれを使う（より高い階層にある可能性があるため探索する）
            Transform parentTransform = transform.parent;
            while (parentTransform != null && parentTransform.name != "MapContainer")
            {
                parentTransform = parentTransform.parent;
            }

            if (parentTransform == null)
            {
                // MapContainer が見つからなければ直上の親を使う
                parentTransform = transform.parent;
            }

            if (parentTransform == null)
            {
                Debug.LogWarning($"[EnemySpawn] No parent transform found for '{gameObject.name}'. Enemies will be created at root.");
            }

            foreach (var spawnPoint in spawnPoints)
            {
                if (spawnPoint == null)
                {
                    Debug.LogWarning($"[EnemySpawn] Null spawnPoint in '{gameObject.name}', skipping.");
                    continue;
                }

                if (enemyPrefabs.Count == 0) break;

                // ランダムに敵のプレハブを選択
                var randomIndex = Random.Range(0, enemyPrefabs.Count);
                var enemyPrefab = enemyPrefabs[randomIndex];

                if (enemyPrefab == null)
                {
                    Debug.LogWarning($"[EnemySpawn] Selected enemy prefab is null in '{gameObject.name}', skipping.");
                    continue;
                }

                // 敵を出現させる
                if (parentTransform != null)
                    Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation, parentTransform);
                else
                    Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

                Debug.Log($"[EnemySpawn] Spawned '{enemyPrefab.name}' at {spawnPoint.position} (parent: {(parentTransform != null ? parentTransform.name : "scene root")}).");
            }
        }
        
        private void OnDrawGizmos()
        {
            // 出現位置をシーンビューに表示
            Gizmos.color = Color.red;
            if (spawnPoints == null) return;
            foreach (var spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawSphere(spawnPoint.position, 0.3f);
                }
            }
        }
    }
}