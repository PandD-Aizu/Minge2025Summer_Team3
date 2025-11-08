using System;
using System.Collections.Generic;
using Minge2025Summer.Scripts.InGame.EnemyScript;
using UniRx;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Minge2025Summer.Scripts.InGame.RandomMapGeneratorScript
{
    public class MapGenerator : MonoBehaviour
    {
        private static readonly Subject<Unit> navMeshRebuiltSubject = new Subject<Unit>();
        public static bool NavMeshReady { get; private set; }
        public static IObservable<Unit> NavMeshReadyAsObservable()
        {
            // 既に完了しているなら即値を返し、未完了なら完了時のストリームを返す
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
        [SerializeField] private NavMeshSurface navMeshSurface;

        #endregion

        // 各エリアの接続情報などを保持する内部クラス
        private class Area
        {
            public bool Visited = false;
            // 北, 東, 南, 西 の順で道が開いているか
            public bool North, East, South, West;
        }

        private Area[,] map;             // マップデータ
        private GameObject mapContainer; // 生成されたマップ全体をまとめるコンテナ
        private Vector3? reservedSpecialPosition;
        private float reservedSpecialRotation;

        public Transform StartMarker
        {
            get => startMarker;
            set => startMarker = value;
        }
        
        public NavMeshSurface NavMeshSurface 
        {
            get => navMeshSurface;
            set => navMeshSurface = value;
        }

        /// <summary>
        /// マップ生成のメイン処理
        /// </summary>
        public void GenerateMap() // Remark: 外部から呼び出せるようにしておく
        {
            NavMeshReady = false; // 再生成開始でリセット

            // 既存のマップがあれば削除
            if (mapContainer != null)
                Destroy(mapContainer);
        
            mapContainer = new GameObject("MapContainer");
        
            // 指定された親オブジェクトがあればそこに配置
            if (designatedParent != null)
                mapContainer.transform.parent = designatedParent;

            InitializeMap();                   // マップデータを初期化
            ForceExternalConnections();        // スタート・ゴール地点の外部接続を強制
            GeneratePaths();                   // 深さ優先探索で迷路を生成
            AddExtraConnections();             // 木構造に辺を追加して閉路を生成
            DetermineAndReserveSpecialPoint(); // 外周に特別な地点を配置する位置を決定
            InstantiatePrefabs();              // マップデータに基づいてプレハブをインスタンス化
            SpawnReservedSpecialPoint();       // スタート・ゴール以外の外周に特別な地点を配置
            PositionMapBasedOnMarker();        // マーカーの位置に基づいてマップ全体を移動
            RebuildNavMesh();                  // NavMeshを再構築
            SpawnEnemiesInMap();               // マップ内に敵を出現させる
        }

        /// <summary>
        /// マップデータを初期化
        /// </summary>
        private void InitializeMap()
        {
            // サイズを奇数に補正
            mapWidth = (mapWidth % 2 == 0) ? mapWidth + 1 : mapWidth;
            mapHeight = (mapHeight % 2 == 0) ? mapHeight + 1 : mapHeight;

            // マップ配列を初期化
            map = new Area[mapWidth, mapHeight];
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    map[x, y] = new Area();
                }
            }
        }
    
        /// <summary>
        /// マップの南北の境界線を外部に接続するために、強制的に道を開ける
        /// </summary>
        private void ForceExternalConnections()
        {
            // 最南端の中央（スタート地点）の南側を開ける
            int startX = mapWidth / 2;
            map[startX, 0].South = true;

            // 最北端の中央（ゴール地点）の北側を開ける
            int goalX = mapWidth / 2;
            map[goalX, mapHeight - 1].North = true;
        }

        /// <summary>
        /// 深さ優先探索アルゴリズムで、全てのエリアが連結された迷路を生成
        /// </summary>
        private void GeneratePaths()
        {
            Stack<Vector2Int> pathStack = new Stack<Vector2Int>();
            Vector2Int currentPos = new Vector2Int(Random.Range(0, mapWidth), Random.Range(0, mapHeight)); // ランダムな開始位置
            map[currentPos.x, currentPos.y].Visited = true; // 開始位置を訪問済みに設定
            pathStack.Push(currentPos); // スタックに開始位置を追加

            // スタックが空になるまで探索を続ける
            while (pathStack.Count > 0)
            {
                // 現在位置から未訪問の隣接エリアを取得
                currentPos = pathStack.Pop();
                List<Vector2Int> neighbors = GetUnvisitedNeighbors(currentPos);

                // 未訪問の隣接エリアがあれば、その中からランダムに1つ選んで道を繋げる
                if (neighbors.Count > 0)
                {
                    // 現在位置をスタックに戻す（バックトラックのため）
                    pathStack.Push(currentPos);

                    // ランダムに隣接エリアを選択
                    Vector2Int chosenNeighbor = neighbors[Random.Range(0, neighbors.Count)];
                
                    // 現在位置と選んだ隣接エリアの間の壁を取り壊す
                    BreakWall(currentPos, chosenNeighbor);

                    // 選んだ隣接エリアを訪問済みに設定し、スタックに追加
                    map[chosenNeighbor.x, chosenNeighbor.y].Visited = true;
                    pathStack.Push(chosenNeighbor);
                }
            }
        }

        /// <summary>
        /// 木構造に辺を追加して閉路を作成する
        /// </summary>
        private void AddExtraConnections()
        {
            if (extraConnectionRatio <= 0.0f)
                return;
        
            // 閉路を作成できるエリアペアのリストを作成
            List<(Vector2Int a, Vector2Int b)> closedPairs = new List<(Vector2Int, Vector2Int)>();
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    Area currentArea = map[x, y];
                    Vector2Int currentPos = new Vector2Int(x, y);

                    // 東側のエリアと閉路を作成できるか
                    if (x + 1 < mapWidth)
                    {
                        Area east = map[x + 1, y];
                        if (!currentArea.East && !east.West)
                            closedPairs.Add((currentPos, new Vector2Int(x + 1, y)));
                    }
                
                    // 北側のエリアと閉路を作成できるか
                    if (y + 1 < mapHeight)
                    {
                        Area north = map[x, y + 1];
                        if (!currentArea.North && !north.South)
                            closedPairs.Add((currentPos, new Vector2Int(x, y + 1)));
                    }
                }
            }

            // 閉路を作成できるペアがなければ終了
            if (closedPairs.Count == 0)
                return;

            int openCount = Mathf.RoundToInt(closedPairs.Count * Mathf.Clamp01(extraConnectionRatio));

            // フィッシャー - イェーツのシャッフルアルゴリズムでリストをシャッフル
            for (int i = closedPairs.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (closedPairs[i], closedPairs[randomIndex]) = (closedPairs[randomIndex], closedPairs[i]);
            }

            // 指定された数だけ閉路を開く
            for (int i = 0; i < openCount; i++)
            {
                var pair = closedPairs[i];
                BreakWall(pair.a, pair.b);
            }
        }

        /// <summary>
        /// 外周に特別な部屋を追加する
        /// </summary>
        private void DetermineAndReserveSpecialPoint()
        {
            // 既に予約されている場合はリセット
            reservedSpecialPosition = null;
            reservedSpecialRotation = 0f; // 回転も初期化
            if (specialPointPrefab == null)
                return;

            // スタート・ゴール地点を除く外周のセルをリストアップ
            int centerX = mapWidth / 2;
            Vector2Int startPos = new Vector2Int(centerX, 0);
            Vector2Int goalPos = new Vector2Int(centerX, mapHeight - 1);
    
            // 外周セルリストを作成
            List<Vector2Int> perimeterCells = new List<Vector2Int>();

            // 下側の外周（南側）
            for (int x = 0; x < mapWidth; x++)
            {
                if (x == startPos.x)
                    continue;
        
                perimeterCells.Add(new Vector2Int(x, 0));
            }

            // 上側の外周（北側）
            for (int x = 0; x < mapWidth; x++)
            {
                if (x == goalPos.x)
                    continue;
        
                perimeterCells.Add(new Vector2Int(x, mapHeight - 1));
            }

            // 左側の外周（西側）
            for (int y = 0; y < mapHeight; y++)
            {
                perimeterCells.Add(new Vector2Int(0, y));
            }

            // 右側の外周（東側）
            for (int y = 0; y < mapHeight; y++)
            {
                perimeterCells.Add(new Vector2Int(mapWidth - 1, y));
            }

            // 外周セルがなければ終了
            if (perimeterCells.Count == 0)
                return;

            // フィッシャー - イェーツのシャッフルアルゴリズムでリストをシャッフル
            for (int i = perimeterCells.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (perimeterCells[i], perimeterCells[randomIndex]) = (perimeterCells[randomIndex], perimeterCells[i]);
            }
    
            // ランダムに選んだ外周セルの中から、スタート・ゴール地点を避けて最初に見つかったセルに特別な部屋を配置
            foreach (var cell in perimeterCells)
            {
                Vector3 outsidePos;
                float yRotation = 0f;
                Area a = map[cell.x, cell.y];

                if (cell.y == 0 && cell.x != startPos.x) // 南側の外周
                {
                    a.South = true;
                    outsidePos = new Vector3(cell.x * areaSize, 0, -areaSize);
                    yRotation = 0f; // 北側入り口をそのまま北に向ける
                }
                else if (cell.y == mapHeight - 1 && cell.x != goalPos.x) // 北側の外周
                {
                    a.North = true;
                    outsidePos = new Vector3(cell.x * areaSize, 0, mapHeight * areaSize);
                    yRotation = 180f; // 北側入り口を南に向ける
                }
                else if (cell.x == 0) // 西側の外周
                {
                    a.West = true;
                    outsidePos = new Vector3(-areaSize, 0, cell.y * areaSize);
                    yRotation = 90f; // 北側入り口を東に向ける
                }
                else // 東側の外周
                {
                    a.East = true;
                    outsidePos = new Vector3(mapWidth * areaSize, 0, cell.y * areaSize);
                    yRotation = 270f; // 北側入り口を西に向ける
                }

                reservedSpecialPosition = outsidePos;
                reservedSpecialRotation = yRotation;
                break;
            }
        }


        /// <summary>
        /// マップデータに基づいてプレハブをインスタンス化
        /// </summary>
        private void InstantiatePrefabs()
        {
            // 各エリアの接続情報に基づいて適切なプレハブを選択し、インスタンス化
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    // エリアのワールド座標を計算
                    Vector3 position = new Vector3(x * areaSize, 0, y * areaSize);
                    Area currentArea = map[x, y];
                
                    // 接続情報に基づいてプレハブを選択
                    int connectionCount = 0;
                    if (currentArea.North) connectionCount++;
                    if (currentArea.East) connectionCount++;
                    if (currentArea.South) connectionCount++;
                    if (currentArea.West) connectionCount++;

                    // 道が1つもないエリアはスキップ
                    GameObject prefabToInstantiate = null;
                    float yRotation = 0f;

                    // 接続数に応じてプレハブを選択
                    switch (connectionCount)
                    {
                        case 1: // 行き止まり
                            prefabToInstantiate = GetRandom(deadEndPrefabs);
                            if (currentArea.North) yRotation = 0;
                            else if (currentArea.East) yRotation = 90;
                            else if (currentArea.South) yRotation = 180;
                            else if (currentArea.West) yRotation = 270;
                            break;
                        
                        case 2: // 直線 or カーブ
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

                        case 3: // T字路
                            prefabToInstantiate = GetRandom(tJunctionPrefabs);
                            if (!currentArea.West) yRotation = 0;         // N, E, S
                            else if (!currentArea.North) yRotation = 90;  // E, S, W
                            else if (!currentArea.East) yRotation = 180;  // S, W, N
                            else if (!currentArea.South) yRotation = 270; // W, N, E
                            break;

                        case 4: // 十字路
                            prefabToInstantiate = GetRandom(crossroadsPrefabs);
                            yRotation = 0;
                            break;
                    }

                    // プレハブをインスタンス化
                    if (prefabToInstantiate != null)
                    {
                        GameObject newArea = Instantiate(prefabToInstantiate, Vector3.zero, Quaternion.Euler(0, yRotation, 0));
                        newArea.transform.SetParent(mapContainer.transform);
                        newArea.transform.localPosition = position;
                    }
                }
            }
        }

        /// <summary>
        /// スタート・ゴール以外の外周に特別な地点を配置
        /// </summary>
        private void SpawnReservedSpecialPoint()
        {
            if (specialPointPrefab == null || !reservedSpecialPosition.HasValue) return;
    
            Quaternion rotation = Quaternion.Euler(0, reservedSpecialRotation, 0);
            Instantiate(specialPointPrefab, reservedSpecialPosition.Value, rotation, mapContainer.transform);
        }


        /// <summary>
        /// マーカーの位置に基づいてマップ全体を移動
        /// </summary>
        private void PositionMapBasedOnMarker()
        {
            if (startMarker == null || mapContainer == null) return;

            // マップのスタート地点（最南端中央）のローカル座標を計算
            Vector3 startAreaLocalPos = new Vector3((mapWidth / 2) * areaSize, 0, 0);

            // スタート地点をマーカーの位置に合わせてマップ全体を移動
            Vector3 startAreaWorldPos = mapContainer.transform.TransformPoint(startAreaLocalPos);
            Vector3 offset = startMarker.position - startAreaWorldPos;
            mapContainer.transform.position += offset;
            mapContainer.transform.position += new Vector3(0, 0, areaSize / 2.0f); // 境界線の中央に合わせる
        }

        // NavMeshを再構築
        public void RebuildNavMesh()
        {
            if (navMeshSurface != null)
                navMeshSurface.BuildNavMesh();
            NavMeshReady = true;
            navMeshRebuiltSubject.OnNext(Unit.Default); // UniRx通知
        }

        /// <summary>
        /// 敵を生成する
        /// </summary>
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
        
        #region Helper Methods

        /// <summary>
        /// 生成可能なプレハブリストからランダムに1つ取得
        /// </summary>
        /// <param name="list">生成可能なエリア部品のプレハブ素材たち</param>
        /// <returns>リスト内のランダムなプレハブ</returns>
        private GameObject GetRandom(List<GameObject> list)
        {
            if (list == null || list.Count == 0)
                return null;

            return list[Random.Range(0, list.Count)];
        }

        /// <summary>
        /// 指定された座標の未訪問の隣接エリアを取得
        /// </summary>
        private List<Vector2Int> GetUnvisitedNeighbors(Vector2Int pos)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            // 北
            if (pos.y + 1 < mapHeight && !map[pos.x, pos.y + 1].Visited) neighbors.Add(new Vector2Int(pos.x, pos.y + 1));
            // 東
            if (pos.x + 1 < mapWidth && !map[pos.x + 1, pos.y].Visited) neighbors.Add(new Vector2Int(pos.x + 1, pos.y));
            // 南
            if (pos.y - 1 >= 0 && !map[pos.x, pos.y - 1].Visited) neighbors.Add(new Vector2Int(pos.x, pos.y - 1));
            // 西
            if (pos.x - 1 >= 0 && !map[pos.x - 1, pos.y].Visited) neighbors.Add(new Vector2Int(pos.x - 1, pos.y));
        
            return neighbors;
        }

        /// <summary>
        /// 2つのエリア間の壁を取り壊し、道を繋げる
        /// </summary>
        private void BreakWall(Vector2Int current, Vector2Int next)
        {
            if (next.x > current.x) // 東へ移動
            {
                map[current.x, current.y].East = true;
                map[next.x, next.y].West = true;
            }
            else if (next.x < current.x) // 西へ移動
            {
                map[current.x, current.y].West = true;
                map[next.x, next.y].East = true;
            }
            else if (next.y > current.y) // 北へ移動
            {
                map[current.x, current.y].North = true;
                map[next.x, next.y].South = true;
            }
            else if (next.y < current.y) // 南へ移動
            {
                map[current.x, current.y].South = true;
                map[next.x, next.y].North = true;
            }
        }
        #endregion
    }
}