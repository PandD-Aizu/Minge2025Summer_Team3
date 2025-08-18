using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    // --- インスペクターで設定する項目 ---
    [Header("マップのサイズ")]
    public int mapWidth = 20;
    public int mapHeight = 10;
    public int mapDepth = 50;

    [Header("部屋の生成設定")]
    public int roomCount = 5; // 生成する部屋の数
    public int roomRadius = 2; // 部屋の半径（中心からの距離）

    [Header("可視化設定")]
    public GameObject cubePrefab; // マップを構成するキューブのプレハブ
    public Transform mapContainer; // 生成したキューブを格納する親オブジェクト

    // --- 内部データ ---
    // セルの種類を定義
    private enum CellType
    {
        Empty, // 何もない空間
        Path,  // 通路
        Room   // 部屋
    }
    // 3次元グリッド
    private CellType[,,] grid;

    /// <summary>
    /// マップ生成のメイン処理
    /// </summary>
    public void GenerateMap()
    {
        // 既存のマップを削除
        if (mapContainer != null)
        {
            // 子オブジェクトを逆順で削除（順方向だとインデックスがずれるため）
            for (int i = mapContainer.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(mapContainer.GetChild(i).gameObject);
            }
        }

        // 1. グリッドの初期化
        InitializeGrid();

        // 2. メイン通路の生成
        List<Vector3Int> mainPath = CreateMainPath();

        // 3. 小部屋の生成
        CreateRooms(mainPath);

        // 4. グリッド情報をもとにキューブを配置
        InstantiateMap();
    }

    /// <summary>
    /// グリッドをすべて「Empty」で埋める
    /// </summary>
    private void InitializeGrid()
    {
        grid = new CellType[mapWidth, mapHeight, mapDepth];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int z = 0; z < mapDepth; z++)
                {
                    grid[x, y, z] = CellType.Empty;
                }
            }
        }
    }

    /// <summary>
    /// スタートからゴールまでのメイン通路を生成する (ランダムウォーク法)
    /// </summary>
    /// <returns>通路の座標リスト</returns>
    private List<Vector3Int> CreateMainPath()
    {
        List<Vector3Int> path = new List<Vector3Int>();

        // スタート地点とゴール地点を対角に設定
        Vector3Int startPos = new Vector3Int(0, 0, 0);
        Vector3Int goalPos = new Vector3Int(mapWidth - 1, mapHeight - 1, mapDepth - 1);
        Vector3Int currentPos = startPos;

        // ゴールにたどり着くまでループ
        while (currentPos != goalPos)
        {
            // 現在地を通路として記録
            if (grid[currentPos.x, currentPos.y, currentPos.z] == CellType.Empty)
            {
                grid[currentPos.x, currentPos.y, currentPos.z] = CellType.Path;
                path.Add(currentPos);
            }

            // --- 次の移動方向を決定 ---
            // ゴールへの方向ベクトルを計算
            Vector3 directionToGoal = ((Vector3)(goalPos - currentPos)).normalized;

            // 移動方向の候補（上下左右前後）
            Vector3Int[] moveDirections = {
                Vector3Int.up, Vector3Int.down, Vector3Int.left,
                Vector3Int.right, new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1)
            };

            Vector3Int nextMove = Vector3Int.zero;
            float bestScore = -2f; // ゴールに最も近づく方向を見つけるためのスコア

            // 80%の確率でゴールに近い方向を、20%の確率でランダムな方向を選ぶ
            if (Random.value < 0.8f)
            {
                // 最もゴールに近づく方向を選ぶ
                foreach (var dir in moveDirections)
                {
                    float score = Vector3.Dot(((Vector3)dir).normalized, directionToGoal);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        nextMove = dir;
                    }
                }
            }
            else
            {
                // ランダムな方向を選ぶ
                nextMove = moveDirections[Random.Range(0, moveDirections.Length)];
            }

            // 次の位置へ移動
            currentPos += nextMove;

            // マップの範囲外に出ないように座標を制限 (Clamp)
            currentPos.x = Mathf.Clamp(currentPos.x, 0, mapWidth - 1);
            currentPos.y = Mathf.Clamp(currentPos.y, 0, mapHeight - 1);
            currentPos.z = Mathf.Clamp(currentPos.z, 0, mapDepth - 1);
        }
        // ゴール地点も通路として記録
        grid[goalPos.x, goalPos.y, goalPos.z] = CellType.Path;
        path.Add(goalPos);

        return path;
    }

    /// <summary>
    /// メイン通路の途中に小部屋を生成する
    /// </summary>
    /// <param name="path">メイン通路の座標リスト</param>
    private void CreateRooms(List<Vector3Int> path)
    {
        for (int i = 0; i < roomCount; i++)
        {
            // 通路上のランダムな地点を部屋の中心として選ぶ
            Vector3Int roomCenter = path[Random.Range(0, path.Count)];

            // 中心から指定された半径の範囲を部屋にする
            for (int x = -roomRadius; x <= roomRadius; x++)
            {
                for (int y = -roomRadius; y <= roomRadius; y++)
                {
                    for (int z = -roomRadius; z <= roomRadius; z++)
                    {
                        // 球状に部屋を生成するための判定
                        if (x * x + y * y + z * z > roomRadius * roomRadius) continue;

                        Vector3Int pos = roomCenter + new Vector3Int(x, y, z);

                        // グリッドの範囲内かチェック
                        if (pos.x >= 0 && pos.x < mapWidth &&
                            pos.y >= 0 && pos.y < mapHeight &&
                            pos.z >= 0 && pos.z < mapDepth)
                        {
                            grid[pos.x, pos.y, pos.z] = CellType.Room;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// グリッド情報をもとに、シーンにキューブを配置する
    /// </summary>
    private void InstantiateMap()
    {
        if (cubePrefab == null || mapContainer == null)
        {
            Debug.LogError("Cube Prefab または Map Container が設定されていません。");
            return;
        }

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int z = 0; z < mapDepth; z++)
                {
                    // Path または Room のセルにキューブを配置
                    if (grid[x, y, z] != CellType.Empty)
                    {
                        Vector3 position = new Vector3(x, y, z);
                        GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
                        cube.transform.SetParent(mapContainer);
                    }
                }
            }
        }
    }
}
