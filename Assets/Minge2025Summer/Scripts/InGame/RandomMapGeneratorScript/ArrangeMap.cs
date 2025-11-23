using System;
using System.Collections.Generic;
using Minge2025Summer.Scripts.InGame.EnemyScript;
using Minge2025Summer.Scripts.InGame.EnemyScript.Boss;
using UniRx;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;

namespace Minge2025Summer.Scripts.InGame.RandomMapGeneratorScript
{
    public class MapGenerator : MonoBehaviour
    {
        private static readonly Subject<Unit> navMeshRebuiltSubject = new Subject<Unit>();
        public static bool NavMeshReady { get; private set; }
        public int NumberOfSpecialRoom { get; set; } //生成する特殊部屋の数
        public static IObservable<Unit> NavMeshReadyAsObservable()
        {
            return NavMeshReady ? Observable.Return(Unit.Default) : navMeshRebuiltSubject.AsObservable();
        }

        #region Inspector Fields
        [Header("マップ設定")]
        [Tooltip("マップの幅（必ず奇数を指定）")]
        [SerializeField] private int mapWidth = 21;

        [Tooltip("マップの高さ（必ず奇数を指定）")]
        [SerializeField] private int mapHeight = 21;

        [Tooltip("エリア（タイル）1つあたりのサイズ")]
        [SerializeField] private float areaSize = 5.0f;

        [Header("閉路設定")]
        [Tooltip("閉路を生成する割合")]
        [SerializeField, Range(0.0f, 1.0f)] private float extraConnectionRatio = 0.15f;

        [Header("オブジェクト設定")]
        [Tooltip("スタート地点の目印となるオブジェクト")]
        [SerializeField] private Transform startMarker;

        [Tooltip("特別な地点に配置するプレハブ")]
        [SerializeField] private GameObject specialPointPrefab;

        [Tooltip("生成されたマップの親オブジェクト")]
        [SerializeField] private Transform designatedParent;

        [Header("道プレハブ")]
        [Tooltip("カーブ（角）のプレハブ")]
        [SerializeField] private List<GameObject> cornerPrefabs;

        [Tooltip("直線のプレハブ")]
        [SerializeField] private List<GameObject> straightPrefabs;

        [Tooltip("T字路のプレハブ")]
        [SerializeField] private List<GameObject> tJunctionPrefabs;

        [Tooltip("十字路のプレハブ")]
        [SerializeField] private List<GameObject> crossroadsPrefabs;

        [Tooltip("行き止まりのプレハブ")]
        [SerializeField] private List<GameObject> deadEndPrefabs;

        [Header("NavMeshSurface設定")]
        [Tooltip("NavMeshSurfaceを持つオブジェクト")]
        [SerializeField] private List<NavMeshSurface> navMeshSurfaces;

        [Header("ボス生成設定")]
        [Tooltip("生成させるボス")]
        [SerializeField] private AssetReference bossAddressReference;

        [Tooltip("ボスを生成する際に加える固定オフセット（ワールド座標）")]
        [SerializeField] private Vector3 bossSpawnOffset = new Vector3(0f, 1.5f, 0f);

        #endregion

        #region FOR TESTING PURPOSES ONLY
        [SerializeField] private bool generateOnStart = false;
        #endregion

        // 以下省略せず既存実装を保持（クラス内部は元のまま）
        private class Area
        {
            public bool Visited = false;
            public bool North, East, South, West;
        }

        private Area[,] map;
        private GameObject mapContainer;
        private Vector3? reservedSpecialPosition;
        private float reservedSpecialRotation;

        public Transform StartMarker
        {
            get => startMarker;
            set => startMarker = value;
        }

        public List<NavMeshSurface> NavMeshSurfaces
        {
            get => navMeshSurfaces;
            set => navMeshSurfaces = value;
        }

        private void Start()
        {
            if (generateOnStart == true)
                GenerateMap();
        }

        public void GenerateMap()
        {
            NavMeshReady = false;

            if (mapContainer != null)
                Destroy(mapContainer);

            mapContainer = new GameObject("MapContainer");

            if (designatedParent != null)
                mapContainer.transform.parent = designatedParent;

            InitializeMap();
            ForceExternalConnections();
            GeneratePaths();
            AddExtraConnections();
            DetermineAndReserveSpecialPoint();
            InstantiatePrefabs();
            SpawnReservedSpecialPoint();
            PositionMapBasedOnMarker();
            RebuildNavMesh();
            SpawnEnemiesInMap();
            SpawnBoss();
        }

        private void InitializeMap()
        {
            mapWidth = (mapWidth % 2 == 0) ? mapWidth + 1 : mapWidth;
            mapHeight = (mapHeight % 2 == 0) ? mapHeight + 1 : mapHeight;

            map = new Area[mapWidth, mapHeight];
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    map[x, y] = new Area();
                }
            }
        }

        private void ForceExternalConnections()
        {
            int startX = mapWidth / 2;
            map[startX, 0].South = true;

            int goalX = mapWidth / 2;
            map[goalX, mapHeight - 1].North = true;
        }

        private void GeneratePaths()
        {
            Stack<Vector2Int> pathStack = new Stack<Vector2Int>();
            Vector2Int currentPos = new Vector2Int(Random.Range(0, mapWidth), Random.Range(0, mapHeight));
            map[currentPos.x, currentPos.y].Visited = true;
            pathStack.Push(currentPos);

            while (pathStack.Count > 0)
            {
                currentPos = pathStack.Pop();
                List<Vector2Int> neighbors = GetUnvisitedNeighbors(currentPos);

                if (neighbors.Count > 0)
                {
                    pathStack.Push(currentPos);
                    Vector2Int chosenNeighbor = neighbors[Random.Range(0, neighbors.Count)];
                    BreakWall(currentPos, chosenNeighbor);
                    map[chosenNeighbor.x, chosenNeighbor.y].Visited = true;
                    pathStack.Push(chosenNeighbor);
                }
            }
        }

        private void AddExtraConnections()
        {
            if (extraConnectionRatio <= 0.0f)
                return;

            List<(Vector2Int a, Vector2Int b)> closedPairs = new List<(Vector2Int, Vector2Int)>();
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    Area currentArea = map[x, y];
                    Vector2Int currentPos = new Vector2Int(x, y);

                    if (x + 1 < mapWidth)
                    {
                        Area east = map[x + 1, y];
                        if (!currentArea.East && !east.West)
                            closedPairs.Add((currentPos, new Vector2Int(x + 1, y)));
                    }

                    if (y + 1 < mapHeight)
                    {
                        Area north = map[x, y + 1];
                        if (!currentArea.North && !north.South)
                            closedPairs.Add((currentPos, new Vector2Int(x, y + 1)));
                    }
                }
            }

            if (closedPairs.Count == 0)
                return;

            int openCount = Mathf.RoundToInt(closedPairs.Count * Mathf.Clamp01(extraConnectionRatio));

            for (int i = closedPairs.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (closedPairs[i], closedPairs[randomIndex]) = (closedPairs[randomIndex], closedPairs[i]);
            }

            for (int i = 0; i < openCount; i++)
            {
                var pair = closedPairs[i];
                BreakWall(pair.a, pair.b);
            }
        }

        private void DetermineAndReserveSpecialPoint()
        {
            reservedSpecialPosition = null;
            reservedSpecialRotation = 0f;
            if (specialPointPrefab == null)
                return;

            int centerX = mapWidth / 2;
            Vector2Int startPos = new Vector2Int(centerX, 0);
            Vector2Int goalPos = new Vector2Int(centerX, mapHeight - 1);

            List<Vector2Int> perimeterCells = new List<Vector2Int>();

            for (int x = 0; x < mapWidth; x++)
            {
                if (x == startPos.x)
                    continue;

                perimeterCells.Add(new Vector2Int(x, 0));
            }

            for (int x = 0; x < mapWidth; x++)
            {
                if (x == goalPos.x)
                    continue;

                perimeterCells.Add(new Vector2Int(x, mapHeight - 1));
            }

            for (int y = 0; y < mapHeight; y++)
            {
                perimeterCells.Add(new Vector2Int(0, y));
            }

            for (int y = 0; y < mapHeight; y++)
            {
                perimeterCells.Add(new Vector2Int(mapWidth - 1, y));
            }

            if (perimeterCells.Count == 0)
                return;

            for (int i = perimeterCells.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (perimeterCells[i], perimeterCells[randomIndex]) = (perimeterCells[randomIndex], perimeterCells[i]);
            }

            foreach (var cell in perimeterCells)
            {
                Vector3 outsidePos;
                float yRotation = 0f;
                Area a = map[cell.x, cell.y];

                if (cell.y == 0 && cell.x != startPos.x)
                {
                    a.South = true;
                    outsidePos = new Vector3(cell.x * areaSize, 0, -areaSize);
                    yRotation = 0f;
                }
                else if (cell.y == mapHeight - 1 && cell.x != goalPos.x)
                {
                    a.North = true;
                    outsidePos = new Vector3(cell.x * areaSize, 0, mapHeight * areaSize);
                    yRotation = 180f;
                }
                else if (cell.x == 0)
                {
                    a.West = true;
                    outsidePos = new Vector3(-areaSize, 0, cell.y * areaSize);
                    yRotation = 90f;
                }
                else
                {
                    a.East = true;
                    outsidePos = new Vector3(mapWidth * areaSize, 0, cell.y * areaSize);
                    yRotation = 270f;
                }

                reservedSpecialPosition = outsidePos;
                reservedSpecialRotation = yRotation;
                break;
            }
        }

        private void InstantiatePrefabs()
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    Vector3 position = new Vector3(x * areaSize, 0, y * areaSize);
                    Area currentArea = map[x, y];

                    int connectionCount = 0;
                    if (currentArea.North) connectionCount++;
                    if (currentArea.East) connectionCount++;
                    if (currentArea.South) connectionCount++;
                    if (currentArea.West) connectionCount++;

                    GameObject prefabToInstantiate = null;
                    float yRotation = 0f;

                    switch (connectionCount)
                    {
                        case 1:
                            prefabToInstantiate = GetRandom(deadEndPrefabs);
                            if (currentArea.North) yRotation = 0;
                            else if (currentArea.East) yRotation = 90;
                            else if (currentArea.South) yRotation = 180;
                            else if (currentArea.West) yRotation = 270;
                            break;
                        case 2:
                            if (currentArea.North && currentArea.South)
                            {
                                prefabToInstantiate = GetRandom(straightPrefabs);
                                yRotation = 0;
                            }
                            else if (currentArea.East && currentArea.West)
                            {
                                prefabToInstantiate = GetRandom(straightPrefabs);
                                yRotation = 90;
                            }
                            else
                            {
                                prefabToInstantiate = GetRandom(cornerPrefabs);
                                if (currentArea.North && currentArea.East) yRotation = 0;
                                else if (currentArea.East && currentArea.South) yRotation = 90;
                                else if (currentArea.South && currentArea.West) yRotation = 180;
                                else if (currentArea.West && currentArea.North) yRotation = 270;
                            }
                            break;
                        case 3:
                            prefabToInstantiate = GetRandom(tJunctionPrefabs);
                            if (!currentArea.West) yRotation = 0;
                            else if (!currentArea.North) yRotation = 90;
                            else if (!currentArea.East) yRotation = 180;
                            else if (!currentArea.South) yRotation = 270;
                            break;
                        case 4:
                            prefabToInstantiate = GetRandom(crossroadsPrefabs);
                            yRotation = 0;
                            break;
                    }

                    if (prefabToInstantiate != null)
                    {
                        GameObject newArea = Instantiate(prefabToInstantiate, Vector3.zero, Quaternion.Euler(0, yRotation, 0));
                        newArea.transform.SetParent(mapContainer.transform);
                        newArea.transform.localPosition = position;
                    }
                }
            }
        }

        private void SpawnReservedSpecialPoint()
        {
            if (specialPointPrefab == null || !reservedSpecialPosition.HasValue) return;

            Quaternion rotation = Quaternion.Euler(0, reservedSpecialRotation, 0);
            Instantiate(specialPointPrefab, reservedSpecialPosition.Value, rotation, mapContainer.transform);
        }

        private void PositionMapBasedOnMarker()
        {
            if (startMarker == null || mapContainer == null) return;

            Vector3 startAreaLocalPos = new Vector3((mapWidth / 2) * areaSize, 0, 0);

            Vector3 startAreaWorldPos = mapContainer.transform.TransformPoint(startAreaLocalPos);
            Vector3 offset = startMarker.position - startAreaWorldPos;
            mapContainer.transform.position += offset;
            mapContainer.transform.position += new Vector3(0, 0, areaSize / 2.0f);
        }

        public void RebuildNavMesh()
        {
            if (navMeshSurfaces.Count != 0)
            {
                Physics.SyncTransforms();
                foreach (NavMeshSurface surf in navMeshSurfaces)
                    surf.BuildNavMesh();
            }
                
            NavMeshReady = true;
            navMeshRebuiltSubject.OnNext(Unit.Default);
        }

        public void SpawnEnemiesInMap()
        {
            if (mapContainer == null)
                return;

            var spawners = mapContainer.GetComponentsInChildren<EnemySpawn>(true);
            foreach(var area in spawners)
            {
                area.SpawnEnemies();
            }
        }

        public void SpawnBoss()
        {
            if (bossAddressReference == null)
            {
                Debug.LogWarning("MapGenerator: Boss AssetReference is not assigned.", this);
                return;
            }

            if (mapContainer == null)
            {
                Debug.LogError("MapGenerator: mapContainer is null, cannot spawn boss.", this);
                return;
            }

            // マップの論理的な中心座標（XZ平面）を計算
            // マップ全体のサイズ = (幅 * タイルサイズ)
            int centerIndexX = mapWidth / 2;
            int centerIndexY = mapHeight / 2;

            // ローカル座標への変換
            float centerX = centerIndexX * areaSize;
            float centerZ = centerIndexY * areaSize;
            
            // MapContainer基準のローカル座標
            Vector3 centerLocal = new Vector3(centerX, 0f, centerZ);
            Vector3 searchCenterWorld = mapContainer.transform.TransformPoint(centerLocal);

            // Addressablesで生成
            var handle = bossAddressReference.InstantiateAsync();
            
            handle.Completed += (AsyncOperationHandle<GameObject> h) =>
            {
                if (h.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogWarning($"MapGenerator: Boss instantiate failed: {h.OperationException}", this);
                    return;
                }

                var bossObj = h.Result;
                bossObj.name = "Boss";

                // 中心座標から「最も近いNavMesh上の点」を探す
                float searchRadius = Mathf.Max(mapWidth, mapHeight) * areaSize; 
                Vector3 targetPosition = searchCenterWorld; // デフォルトは計算上の中心

                // AllAreas を指定して、とにかく乗れる場所を探す
                if (UnityEngine.AI.NavMesh.SamplePosition(searchCenterWorld, out UnityEngine.AI.NavMeshHit hit, searchRadius, UnityEngine.AI.NavMesh.AllAreas))
                {
                    targetPosition = hit.position;
                }
                else
                {
                    Debug.LogWarning($"MapGenerator: Could not find any NavMesh near center. Using raw center coordinates.", this);
                }

                // 親を設定
                bossObj.transform.SetParent(mapContainer.transform, true);

                // エージェントの設定と配置
                var agent = bossObj.GetComponentInChildren<UnityEngine.AI.NavMeshAgent>(true);
                
                if (agent != null)
                {
                    agent.enabled = false;
                    bossObj.transform.position = targetPosition;
                    agent.enabled = true;
                    if (agent.Warp(targetPosition))
                    {
                        Debug.Log($"Boss spawned at: {targetPosition} (Center was: {searchCenterWorld})");
                    }
                    else
                    {
                        Debug.LogWarning("First Warp failed. Retrying...");
                        agent.Warp(targetPosition + Vector3.up * 0.5f);
                    }
                }
                else
                {
                    // Agentがない場合は単純移動
                    bossObj.transform.position = targetPosition;
                }

                // PatrolModelへのエリア情報伝達
                var patrolComponent = bossObj.GetComponentInChildren<BossPatrolModel>(true);
                if (patrolComponent != null)
                {
                    Transform patrolCenter = mapContainer.transform.Find("PatrolAreaCenter");
                    if (patrolCenter == null)
                    {
                        var go = new GameObject("PatrolAreaCenter");
                        go.transform.SetParent(mapContainer.transform, false);
                        go.transform.localPosition = centerLocal;
                        patrolCenter = go.transform;
                    }

                    patrolComponent.PatrolAreaCenter = patrolCenter;
                    patrolComponent.PatrolAreaSize = new Vector2(mapWidth * areaSize, mapHeight * areaSize);
                }
            };
        }

        #region Helper Methods
        private GameObject GetRandom(List<GameObject> list)
        {
            if (list == null || list.Count == 0)
                return null;

            return list[Random.Range(0, list.Count)];
        }

        private List<Vector2Int> GetUnvisitedNeighbors(Vector2Int pos)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            if (pos.y + 1 < mapHeight && !map[pos.x, pos.y + 1].Visited) neighbors.Add(new Vector2Int(pos.x, pos.y + 1));
            if (pos.x + 1 < mapWidth && !map[pos.x + 1, pos.y].Visited) neighbors.Add(new Vector2Int(pos.x + 1, pos.y));
            if (pos.y - 1 >= 0 && !map[pos.x, pos.y - 1].Visited) neighbors.Add(new Vector2Int(pos.x, pos.y - 1));
            if (pos.x - 1 >= 0 && !map[pos.x - 1, pos.y].Visited) neighbors.Add(new Vector2Int(pos.x - 1, pos.y));
            return neighbors;
        }

        private void BreakWall(Vector2Int current, Vector2Int next)
        {
            if (next.x > current.x)
            {
                map[current.x, current.y].East = true;
                map[next.x, next.y].West = true;
            }
            else if (next.x < current.x)
            {
                map[current.x, current.y].West = true;
                map[next.x, next.y].East = true;
            }
            else if (next.y > current.y)
            {
                map[current.x, current.y].North = true;
                map[next.x, next.y].South = true;
            }
            else if (next.y < current.y)
            {
                map[current.x, current.y].South = true;
                map[next.x, next.y].North = true;
            }
        }
        #endregion
    }
}