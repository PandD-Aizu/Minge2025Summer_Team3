using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Test.RandomWalk;

// 生成された部屋の情報を保持するクラス
public class PlacedRoom
{
    public Vector2Int position; // グリッド上の左下の座標
    public Room roomPrefab;     // どの部屋プレハブか
    public Vector2Int size;     // 部屋のサイズ
}

// ウォーカーの状態を保持するクラス
public class Walker
{
    public Vector2Int position;
    public Vector2Int direction;      // 現在の進行方向 (up, down, left, right)
    public int stepsTakenInDirection; // 現在の方向で進んだ歩数
    public bool isActive;             // ウォーカーがまだ活動中か
}

public class RandomWalkMapGenerator : MonoBehaviour
{
    [Header("マップ設定")]
    public int mapWidth = 100;
    public int mapHeight = 100;

    [Header("入口と出口の座標指定")]
    public Vector2Int entranceGridPos = new Vector2Int(10, 10);
    public Vector2Int exitGridPos = new Vector2Int(80, 80);

    [Header("ダンジョン生成設定（迷路の複雑さ）")]
    public int maxWalkers = 10;
    public int minStraightPath = 3;
    public int maxStraightPath = 10;
    [Range(0, 1)]
    public float branchChance = 0.1f;
    [Range(0, 1)]
    public float turnChance = 0.3f;
    [Range(0, 1)]
    public float roomSpawnChance = 0.05f;
    
    [Header("トンネル詳細設定")]
    public int tunnelWidth = 3;
    public float tunnelWallHeight = 4.0f;

    [Header("プレハブ設定")]
    public List<GameObject> roomPrefabs;
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    
    [Header("デバッグ設定")]
    [Tooltip("入口/出口の目印となる床タイルに名前を付ける範囲（半径）")]
    public float specialTileNamingRadius = 5.0f;

    // --- 内部データ ---
    // 0:壁, 1:通路の床, 2:部屋の床
    private int[,] mapGrid;
    private List<PlacedRoom> placedRooms = new List<PlacedRoom>();
    private List<Vector2Int> mainPath = new List<Vector2Int>();

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        ClearExistingMap();
        InitializeGrid();

        // ★変更点：部屋を配置する処理を削除し、座標を直接次のメソッドに渡す
        GenerateMazeFromEndpoints(entranceGridPos, exitGridPos);

        // マップの描画
        VisualizeMap();
    }
    
    void GenerateMazeFromEndpoints(Vector2Int startPos, Vector2Int endPos)
    {
        List<Walker> entranceWalkers = new List<Walker>();
        List<Walker> exitWalkers = new List<Walker>();

        // ウォーカーの初期化
        entranceWalkers.Add(CreateNewWalker(startPos));
        exitWalkers.Add(CreateNewWalker(endPos));

        bool isConnected = false;
        int maxIterations = mapWidth * mapHeight; // 無限ループ防止
        int currentIteration = 0;

        while (!isConnected && currentIteration < maxIterations)
        {
            // 入口側ウォーカーを1ステップ進める
            isConnected = ProcessWalkers(entranceWalkers, 1, 2);
            if (isConnected) break;

            // 出口側ウォーカーを1ステップ進める
            isConnected = ProcessWalkers(exitWalkers, 2, 1);
            if (isConnected) break;

            currentIteration++;
        }

        if(!isConnected) {
            Debug.LogWarning("入口と出口が接続できませんでした。マップを広げるか、設定を調整してください。");
        }
    }
    
    bool ProcessWalkers(List<Walker> walkers, int paintType, int checkType)
    {
        List<Walker> newWalkers = new List<Walker>();

        foreach (var walker in walkers.ToList())
        {
            if (CheckForConnection(walker.position + walker.direction, checkType))
            {
                PaintTunnelFloor(walker.position, paintType);
                return true;
            }
            
            PaintTunnelFloor(walker.position, paintType);

            // ★ここからが追加する部屋生成ロジック
            // 設定した確率で、現在のウォーカーの位置に部屋の生成を試みる
            if (Random.value < roomSpawnChance)
            {
                TryPlaceNormalRoom(walker.position);
            }
            // ★追加ロジックはここまで

            if (walkers.Count + newWalkers.Count < maxWalkers && Random.value < branchChance)
            {
                newWalkers.Add(CreateNewWalker(walker.position, walker.direction));
            }

            if (!MoveWalker(walker))
            {
                walker.isActive = false; 
            }
        }
        
        walkers.AddRange(newWalkers);
        walkers.RemoveAll(w => !w.isActive);

        if(walkers.Count == 0)
        {
            Vector2Int? restartPos = FindRandomPathPoint(paintType);
            if(restartPos.HasValue) {
                walkers.Add(CreateNewWalker(restartPos.Value));
            }
        }
        
        return false;
    }
    
    bool CheckForConnection(Vector2Int pos, int checkType)
    {
        int extent = (tunnelWidth - 1) / 2;
        for (int x = -extent; x <= extent; x++) {
            for (int y = -extent; y <= extent; y++) {
                Vector2Int checkPos = new Vector2Int(pos.x + x, pos.y + y);
                if (IsWithinBounds(checkPos, 1) && mapGrid[checkPos.x, checkPos.y] == checkType) {
                    return true;
                }
            }
        }
        return false;
    }
    
    void InitializeGrid()
    {
        mapGrid = new int[mapWidth, mapHeight];
        placedRooms.Clear();
        mainPath.Clear();
    }
    
    void ClearExistingMap()
    {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
    }
    
    void ForcePlaceRoom(Vector2Int origin, Room roomData, int type)
    {
        for (int x = 0; x < roomData.size.x; x++)
        {
            for (int y = 0; y < roomData.size.y; y++)
            {
                Vector2Int currentPos = new Vector2Int(origin.x + x, origin.y + y);
                if (IsWithinBounds(currentPos, 1))
                {
                    mapGrid[currentPos.x, currentPos.y] = type;
                }
            }
        }
        placedRooms.Add(new PlacedRoom { position = origin, roomPrefab = roomData, size = roomData.size });
    }
    
    int PaintTunnelFloor(Vector2Int centerPos, int type)
    {
        int tilesWritten = 0;
        int extent = (tunnelWidth - 1) / 2;
        for (int x = -extent; x <= extent; x++)
        {
            for (int y = -extent; y <= extent; y++)
            {
                Vector2Int paintPos = new Vector2Int(centerPos.x + x, centerPos.y + y);

                if (IsWithinBounds(paintPos, 1)) // 個別タイルチェック
                {
                    // まだ床になっていない場所のみ掘る
                    if (mapGrid[paintPos.x, paintPos.y] == 0)
                    {
                        mapGrid[paintPos.x, paintPos.y] = type;
                        tilesWritten++;
                    }
                }
            }
        }
        return tilesWritten;
    }
    
    void VisualizeMap()
    {
        // 部屋を配置
        foreach (var roomInfo in placedRooms)
        {
            Vector3 worldPos = new Vector3(roomInfo.position.x, 0, roomInfo.position.y);
            Instantiate(roomInfo.roomPrefab.gameObject, worldPos, Quaternion.identity, this.transform);
        }

        // 通路と壁を配置
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // 入口側(1)と出口側(2)の両方の通路を描画
                if (mapGrid[x, y] == 1 || mapGrid[x, y] == 2)
                {
                    // --- ここからが変更点 ---
                    Vector3 pos = new Vector3(x, 0, y);
                    GameObject floorTile = Instantiate(floorPrefab, pos, Quaternion.identity, this.transform);
                    Vector2Int currentPos = new Vector2Int(x, y);

                    // 入口からの距離をチェックして名前を変更
                    if (Vector2.Distance(currentPos, entranceGridPos) <= specialTileNamingRadius)
                    {
                        floorTile.name = $"Entrance_Floor_{x}_{y}";
                    }
                    // 出口からの距離をチェックして名前を変更
                    else if (Vector2.Distance(currentPos, exitGridPos) <= specialTileNamingRadius)
                    {
                        floorTile.name = $"Exit_Floor_{x}_{y}";
                    }
                }
                else if (mapGrid[x, y] == 0)
                {
                    if (IsAdjacentToAnyFloor(x, y))
                    {
                        // 壁を生成
                        GameObject wall = Instantiate(wallPrefab, new Vector3(x, tunnelWallHeight / 2f, y), Quaternion.identity, this.transform);
                        wall.transform.localScale = new Vector3(1, tunnelWallHeight, 1);
                    }
                }
            }
        }
    }
    
    bool IsAdjacentToAnyFloor(int x, int y)
    {
        // 上下左右に 1, 2, 3 のいずれかがあるか
        for(int dx = -1; dx <= 1; dx++) {
            for(int dy = -1; dy <= 1; dy++) {
                if (Mathf.Abs(dx) + Mathf.Abs(dy) != 1) continue;
                int checkX = x + dx;
                int checkY = y + dy;
                if(checkX >= 0 && checkX < mapWidth && checkY >= 0 && checkY < mapHeight) {
                    if(mapGrid[checkX, checkY] > 0) return true;
                }
            }
        }
        return false;
    }
    
    bool MoveWalker(Walker walker)
    {
        // まっすぐ進む最小歩数を満たしていない場合
        if (walker.stepsTakenInDirection < minStraightPath)
        {
            Vector2Int nextPos = walker.position + walker.direction;
            if (IsWithinBounds(nextPos, tunnelWidth))
            {
                walker.position = nextPos;
                walker.stepsTakenInDirection++;
                return true;
            }
            return false; // 範囲外なら移動失敗
        }
        else // 最小歩数を満たした後は、曲がるか、まっすぐ進み続けるか
        {
            if (Random.value < turnChance || walker.stepsTakenInDirection >= maxStraightPath)
            {
                // 新しい方向を見つける（現在の方向と逆方向は避ける）
                Vector2Int newDir = GetRandomCardinalDirection(-walker.direction);
                if (newDir != Vector2Int.zero)
                {
                    walker.direction = newDir;
                    walker.stepsTakenInDirection = 0; // 方向転換したら歩数をリセット
                    Vector2Int nextPos = walker.position + walker.direction;
                    if (IsWithinBounds(nextPos, tunnelWidth))
                    {
                        walker.position = nextPos;
                        walker.stepsTakenInDirection++;
                        return true;
                    }
                    return false;
                }
            }
            // 曲がらずにまっすぐ進む
            Vector2Int newNextPos = walker.position + walker.direction;
            if (IsWithinBounds(newNextPos, tunnelWidth))
            {
                walker.position = newNextPos;
                walker.stepsTakenInDirection++;
                return true;
            }
            return false;
        }
    }
    
    Walker CreateNewWalker(Vector2Int position, Vector2Int? avoidDirection = null)
    {
        return new Walker
        {
            position = position,
            direction = avoidDirection.HasValue ? GetRandomCardinalDirection(-avoidDirection.Value) : GetRandomCardinalDirection(),
            stepsTakenInDirection = 0,
            isActive = true
        };
    }
    
    Vector2Int? FindRandomPathPoint(int pathType)
    {
        List<Vector2Int> points = new List<Vector2Int>();
        for (int x = 0; x < mapWidth; x++) {
            for (int y = 0; y < mapHeight; y++) {
                if(mapGrid[x, y] == pathType) {
                    points.Add(new Vector2Int(x, y));
                }
            }
        }
        if (points.Count > 0) return points[Random.Range(0, points.Count)];
        return null;
    }
    
    bool IsWithinBounds(Vector2Int pos, int objectSize)
    {
        int halfSize = (objectSize - 1) / 2;
        return pos.x - halfSize >= 1 && pos.x + halfSize < mapWidth - 1 &&
               pos.y - halfSize >= 1 && pos.y + halfSize < mapHeight - 1;
    }

    // ランダムなカーディナル方向（上下左右）を取得
    Vector2Int GetRandomCardinalDirection()
    {
        int dir = Random.Range(0, 4);
        switch (dir)
        {
            case 0: return Vector2Int.up;
            case 1: return Vector2Int.down;
            case 2: return Vector2Int.left;
            default: return Vector2Int.right;
        }
    }
    
    // 特定の方向を避けてランダムなカーディナル方向を取得
    Vector2Int GetRandomCardinalDirection(Vector2Int avoidDirection)
    {
        List<Vector2Int> possibleDirs = new List<Vector2Int>
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };
        // 避ける方向と逆方向も避ける
        possibleDirs.Remove(avoidDirection);
        possibleDirs.Remove(-avoidDirection);

        if (possibleDirs.Count > 0)
        {
            return possibleDirs[Random.Range(0, possibleDirs.Count)];
        }
        return Vector2Int.zero; // 有効な方向がない場合
    }
    
    bool TryPlaceNormalRoom(Vector2Int connectPos)
    {
        if (roomPrefabs.Count == 0) return false;
        
        // ランダムな部屋プレハブを選択
        GameObject randomRoomPrefabGO = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
        Room roomData = randomRoomPrefabGO.GetComponent<Room>();
        if (roomData == null) return false;
        
        // 部屋の配置位置を計算 (通路の中心に部屋の中心が来るように)
        Vector2Int roomOrigin = new Vector2Int(
            connectPos.x - roomData.size.x / 2,
            connectPos.y - roomData.size.y / 2
        );

        // 配置可能かチェックして、可能なら配置
        if (CanPlaceRoom(roomOrigin, roomData.size))
        {
            ForcePlaceRoom(roomOrigin, roomData, 2); // 2は部屋の床
            return true;
        }
        return false;
    }
    
    bool CanPlaceRoom(Vector2Int pos, Vector2Int size)
    {
        // マップ範囲外チェック
        if (!IsWithinBounds(pos, 1) || !IsWithinBounds(pos + size, 1))
        {
            return false;
        }

        // 他のオブジェクトと重なっていないかチェック (周囲1マスのマージンも見る)
        for (int x = pos.x - 1; x < pos.x + size.x + 1; x++)
        {
            for (int y = pos.y - 1; y < pos.y + size.y + 1; y++)
            {
                if (IsWithinBounds(new Vector2Int(x, y), 1) && mapGrid[x, y] != 0)
                {
                    return false; // 既に何かが配置されている
                }
            }
        }
        return true;
    }
}