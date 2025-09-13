using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Test.HybridMapGenerator
{
    public enum TileType { Empty, Floor, Wall, Door }
    
    public class Section
    {
        public Vector2Int gridPosition;
        public RoomTemplate roomTemplate;
        public bool isMainPath = false;
        public bool isStart = false;
        public bool isGoal = false;
        public bool doorN, doorE, doorS, doorW;

        public Section(int x, int y)
        {
            gridPosition = new Vector2Int(x, y);
        }
    }

    public class InstantiatedRoom
    {
        public Section section;
        public GameObject roomObject;
        public Dictionary<string, Transform> doors = new Dictionary<string, Transform>();
    }
    
    public class SectionBasedGenerator : MonoBehaviour
    {
        [Header("マップサイズ設定")] 
        [Range(3, 6)] 
        public int mapSize = 5;

        [Header("区画のタイルサイズ")] 
        public int sectionSizeInTiles = 20;

        [Header("通路の生成設定")] 
        [Range(0.0f, 1.0f)]
        public float sidePathChance = 0.5f;

        [Header("部屋テンプレート")] 
        public List<RoomTemplate> availableRooms;

        [Header("描画プレファブ")] 
        public GameObject floorPrefab;
        public GameObject wallPrefab;
        public GameObject doorPrefab;

        [Header("設定")] 
        public string doorPlaceholderName = "DoorPlaceholder";

        private Section[,] sections;
        private TileType[,] tileGrid;
        private List<InstantiatedRoom> instantiatedRooms = new List<InstantiatedRoom>();
        private Vector2Int startSectionPos;
        private Vector2Int goalSectionPos;

        private void Start()
        {
            GenerateMap();
        }

        private void GenerateMap()
        {
            // グリッドを決定
            InitializeSections();
            DetermineStartAndGoal();
            CreateMainPath();
            CreateSidePaths();
            PlaceRoomsInSections();

            // Prefabを配置し、ドアを置き換える
            InstantiateRoomsAndReplacePlaceholders();
            
            // 部屋同士を通路で接続する
            InitializeTileGridForCorridors();
            ConnectRoomDoorsWithCorridors();
            DrawCorridorsFromTileGrid();
            
            // デバッグ用に区画情報を出力
            DebugPrintSections();
        }

        private void InitializeSections()
        {
            sections = new Section[mapSize, mapSize];
            for (int y = 0; y < mapSize; y++) 
            {
                for (int x = 0; x < mapSize; x++) 
                {
                    sections[x, y] = new Section(x, y);
                }
            }
        }

        private void DetermineStartAndGoal()
        {
            int startX = (mapSize <= 4) ? 1 : 2;
            startSectionPos = new Vector2Int(startX, 0);
            sections[startSectionPos.x, startSectionPos.y].isStart = true;

            int goalX = Random.Range(0, mapSize);
            goalSectionPos = new Vector2Int(goalX, mapSize - 1);
            sections[goalSectionPos.x, goalSectionPos.y].isGoal = true;
        }

        /// <summary>
        /// スタートからゴールまでの主要な道筋を作成
        /// </summary>
        private void CreateMainPath()
        {
            Vector2Int currentPos = startSectionPos;
            List<Vector2Int> path = new List<Vector2Int> { currentPos };

            while (currentPos != goalSectionPos)
            {
                Vector2Int direction = goalSectionPos - currentPos;
                Vector2Int nextPos;

                // 80%の確率でゴールに近い方向へ、20%の確率でランダムな方向へ進み、道をくねらせる
                if (Random.value < 0.8f) 
                {
                    if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
                        nextPos = currentPos + new Vector2Int((int)Mathf.Sign(direction.x), 0);
                    } else {
                        nextPos = currentPos + new Vector2Int(0, (int)Mathf.Sign(direction.y));
                    }
                }
                else
                {
                    // ランダムな方向を選ぶが、後戻りはしないようにする
                    List<Vector2Int> possibleMoves = new List<Vector2Int>();
                    if (direction.x != 0) 
                        possibleMoves.Add(new Vector2Int((int)Mathf.Sign(direction.x), 0));
                    
                    if (direction.y != 0) 
                        possibleMoves.Add(new Vector2Int(0, (int)Mathf.Sign(direction.y)));
                    
                    if (possibleMoves.Count > 0) 
                    {
                        nextPos = currentPos + possibleMoves[Random.Range(0, possibleMoves.Count)];
                    } 
                    else 
                    {
                        break; // 行き止まり
                    }
                }
                
                SetDoorsBetweenSections(currentPos, nextPos);
                currentPos = nextPos;
                path.Add(currentPos);
            }

            foreach (var pos in path) 
            {
                sections[pos.x, pos.y].isMainPath = true;
            }
        }

        /// <summary>
        /// 2つの隣接する区画間のドアフラグを設定する
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private void SetDoorsBetweenSections(Vector2Int from, Vector2Int to)
        {
            if (to.x > from.x) { sections[from.x, from.y].doorE = true; sections[to.x, to.y].doorW = true; }
            else if (to.x < from.x) { sections[from.x, from.y].doorW = true; sections[to.x, to.y].doorE = true; }
            else if (to.y > from.y) { sections[from.x, from.y].doorN = true; sections[to.x, to.y].doorS = true; }
            else if (to.y < from.y) { sections[from.x, from.y].doorS = true; sections[to.x, to.y].doorN = true; }
        }

        /// <summary>
        /// 主要な道筋から分岐する形で、いくつかのサイドパスを作成
        /// </summary>
        private void CreateSidePaths()
        {
            List<Section> pathSections 
                = sections.Cast<Section>()
                    .Where(s => s.isMainPath)
                    .ToList();
            int iterations = (mapSize * mapSize); // 試行回数を増やす

            for (int i = 0; i < iterations; i++)
            {
                if (pathSections.Count == 0 || Random.value > sidePathChance) 
                    continue;

                Section randomPathSection = pathSections[Random.Range(0, pathSections.Count)];
                List<Vector2Int> nonPathNeighbors = GetNonPathNeighbors(randomPathSection.gridPosition);

                if (nonPathNeighbors.Count > 0)
                {
                    Vector2Int neighborPos = nonPathNeighbors[Random.Range(0, nonPathNeighbors.Count)];
                    SetDoorsBetweenSections(randomPathSection.gridPosition, neighborPos);
                    sections[neighborPos.x, neighborPos.y].isMainPath = true;
                    pathSections.Add(sections[neighborPos.x, neighborPos.y]);
                }
            }
        }

        private List<Vector2Int> GetNonPathNeighbors(Vector2Int pos)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            
            foreach (var dir in directions) 
            {
                Vector2Int neighborPos = pos + dir;
                if (neighborPos.x >= 0 && neighborPos.x < mapSize && neighborPos.y >= 0 && neighborPos.y < mapSize && !sections[neighborPos.x, neighborPos.y].isMainPath) 
                {
                    neighbors.Add(neighborPos);
                }
            }
            
            return neighbors;
        }

        /// <summary>
        /// 各区画に部屋テンプレートを割り当てる
        /// </summary>
        private void PlaceRoomsInSections()
        {
            foreach (var currentSection in sections)
            {
                if (currentSection.isMainPath)
                {
                    List<RoomTemplate> fittingRooms = availableRooms.Where(room =>
                        (!currentSection.doorN || CheckRoomHasDoorOnSide(room, "North")) &&
                        (!currentSection.doorS || CheckRoomHasDoorOnSide(room, "South")) &&
                        (!currentSection.doorE || CheckRoomHasDoorOnSide(room, "East")) &&
                        (!currentSection.doorW || CheckRoomHasDoorOnSide(room, "West"))
                    ).ToList();

                    if (fittingRooms.Count > 0)
                    {
                        currentSection.roomTemplate = fittingRooms[Random.Range(0, fittingRooms.Count)];
                    }
                    else
                    {
                        Debug.LogWarning($"区画({currentSection.gridPosition.x},{currentSection.gridPosition.y}) に合う部屋テンプレートが見つかりません。この区画は空になります。");
                        currentSection.roomTemplate = null;
                    }
                }
            }
        }

        private bool CheckRoomHasDoorOnSide(RoomTemplate room, string side)
        {
            if (room == null || room.potentialDoorPositions.Count == 0) 
                return false;
            
            return GetDoorPositionsOnSide(room, side).Count > 0;
        }

        private void InstantiateRoomsAndReplacePlaceholders()
        {
            instantiatedRooms.Clear();

            foreach (var section in sections)
            {
                if (section.roomTemplate == null || section.roomTemplate.layoutPrefab == null)
                    continue;
                
                // 区画のワールド座標を計算
                Vector3 sectionWorldPos = new Vector3(
                    section.gridPosition.x * sectionSizeInTiles,
                    0,
                    section.gridPosition.y * sectionSizeInTiles
                );

                // Prefabをインスタンス化
                GameObject roomObj = Instantiate(section.roomTemplate.layoutPrefab, sectionWorldPos, Quaternion.identity, this.transform);
                roomObj.name = $"Section_{section.gridPosition.x}_{section.gridPosition.y}_{section.roomTemplate.name}";

                var newInstantiatedRoom = new InstantiatedRoom { section = section, roomObject = roomObj };

                // Prefab内の全てのドアの目印を探す
                var placeholders = roomObj.transform.GetComponentsInChildren<Transform>()
                    .Where(t => t.name == doorPlaceholderName).ToList();

                foreach (var placeholder in placeholders)
                {
                    // 目印のローカル座標から、どの方角のドアかを判定
                    string side = GetSideFromLocalPosition(placeholder.localPosition, section.roomTemplate.size);
                    
                    // その方角にドアが必要かどうかをチェック
                    bool doorNeeded = (side == "North" && section.doorN) || (side == "South" && section.doorS) ||
                                      (side == "East" && section.doorE) || (side == "West" && section.doorW);
                    
                    if(doorNeeded)
                    {
                        // 目印の位置に本物のドアを生成
                        GameObject realDoor = Instantiate(doorPrefab, placeholder.position, placeholder.rotation, roomObj.transform);
                        // 生成したドアを記録
                        if (!newInstantiatedRoom.doors.ContainsKey(side))
                        {
                            newInstantiatedRoom.doors.Add(side, realDoor.transform);
                        }
                    }
                    
                    // 目印は不要になったので削除
                    Destroy(placeholder.gameObject);
                }
                instantiatedRooms.Add(newInstantiatedRoom);
            }
        }

        private void InitializeTileGridForCorridors()
        {
            int totalMapSize = mapSize * sectionSizeInTiles;
            
            // タイルグリッドをEmptyで初期化するだけにする
            tileGrid = new TileType[totalMapSize, totalMapSize];
        }

        private void ConnectRoomDoorsWithCorridors()
        {
            foreach (var room in instantiatedRooms)
            {
                var section = room.section;
                // 北側の接続をチェック
                if (section.doorN && section.gridPosition.y + 1 < mapSize)
                {
                    // 隣の区画に配置された部屋(InstantiatedRoom)を探す
                    var neighbor = instantiatedRooms.FirstOrDefault(r => r.section.gridPosition == section.gridPosition + Vector2Int.up);
                    // 自分と相手の両方に、接続に必要なドアが実際に生成されているかを確認
                    if (neighbor != null && room.doors.ContainsKey("North") && neighbor.doors.ContainsKey("South"))
                    {
                        // FindDoorInは使わず、記録済みのドアのTransformを直接使用
                        ConnectTwoDoors(room.doors["North"].position, neighbor.doors["South"].position);
                    }
                }
                // 東側の接続をチェック
                if (section.doorE && section.gridPosition.x + 1 < mapSize)
                {
                    var neighbor = instantiatedRooms.FirstOrDefault(r => r.section.gridPosition == section.gridPosition + Vector2Int.right);
                    if (neighbor != null && room.doors.ContainsKey("East") && neighbor.doors.ContainsKey("West"))
                    {
                        ConnectTwoDoors(room.doors["East"].position, neighbor.doors["West"].position);
                    }
                }
            }
        }

        private void DrawCorridorsFromTileGrid()
        {
            if (tileGrid == null) 
                return;
            
            int totalSize = mapSize * sectionSizeInTiles;
            for (int y = 0; y < totalSize; y++) 
            {
                for (int x = 0; x < totalSize; x++) 
                {
                    if (tileGrid[x, y] == TileType.Empty)
                        continue;

                    GameObject prefab = null;
                    switch (tileGrid[x, y])
                    {
                        case TileType.Floor: prefab = floorPrefab; 
                            break;
                        case TileType.Wall: prefab = wallPrefab; 
                            break;
                    }

                    if (prefab != null) 
                    {
                        float yPos = (tileGrid[x, y] == TileType.Wall) ? wallPrefab.transform.localScale.y / 2f : 0;
                        Instantiate(prefab, new Vector3(x, yPos, y), Quaternion.identity, this.transform);
                    }
                }
            }
        }

        private void PaintCorridorTile(Vector2Int pos)
        {
            if (IsWithinTileGrid(pos.x, pos.y) && tileGrid[pos.x, pos.y] == TileType.Empty)
            {
                tileGrid[pos.x, pos.y] = TileType.Floor;
                
                Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
                foreach(var direction in directions)
                {
                    Vector2Int neighbor = pos + direction;
                    if (IsWithinTileGrid(neighbor.x, neighbor.y) && tileGrid[neighbor.x, neighbor.y] == TileType.Empty)
                    {
                        tileGrid[neighbor.x, neighbor.y] = TileType.Wall;
                    }
                }
            }
        }
        
        /* ヘルパー関数 */
        
        /// <summary>
        /// Prefab内のローカル座標から、それがどの辺にあるかを判定する
        /// </summary>
        private string GetSideFromLocalPosition(Vector3 localPos, Vector2Int roomSize)
        {
            // 最も近い辺を探す
            float distToNorth = Mathf.Abs((roomSize.y - 1) - localPos.z);
            float distToSouth = Mathf.Abs(0 - localPos.z);
            float distToEast = Mathf.Abs((roomSize.x - 1) - localPos.x);
            float distToWest = Mathf.Abs(0 - localPos.x);

            float min = Mathf.Min(distToNorth, distToSouth, distToEast, distToWest);
            
            if (min == distToNorth) return "North";
            if (min == distToSouth) return "South";
            if (min == distToEast) return "East";
            return "West";
        }
        
        /// <summary>
        /// ２つのドア座標をL字型の通路でふさぐ
        /// </summary>
        /// <param name="doorA"></param>
        /// <param name="doorB"></param>
        private void ConnectTwoDoors(Vector2Int? doorA, Vector2Int? doorB)
        {
            if (!doorA.HasValue || !doorB.HasValue)
                return;

            Vector2Int current = doorA.Value;
            Vector2Int target = doorB.Value;

            while (current.y != target.y)
            {
                PaintCorridorTile(current);
                current.y += (int)Mathf.Sign(target.y - current.y);
            }

            while (current.x != target.x)
            {
                PaintCorridorTile(current);
                current.x += (int)Mathf.Sign(target.x - current.x);
            }

            PaintCorridorTile(target);
        }
        
        /// <summary>
        /// ２つのドア座標をL字型の通路でふさぐ
        /// </summary>
        /// <param name="posA">３次元のベクトル座標</param>
        /// <param name="posB">３次元のベクトル座標</param>
        private void ConnectTwoDoors(Vector3 posA, Vector3 posB)
        {
            Vector2Int tileA = new Vector2Int(Mathf.RoundToInt(posA.x), Mathf.RoundToInt(posA.z));
            Vector2Int tileB = new Vector2Int(Mathf.RoundToInt(posB.x), Mathf.RoundToInt(posB.z));
            ConnectTwoDoors(tileA, tileB);
        }
        
        /// <summary>
        /// 区画内での部屋の基点座標（左下）を計算する
        /// </summary>
        private Vector2Int GetRoomOriginInTileGrid(Section section)
        {
            RoomTemplate room = section.roomTemplate;
            int sectionOriginX = section.gridPosition.x * sectionSizeInTiles;
            int sectionOriginY = section.gridPosition.y * sectionSizeInTiles;

            // 区画内で部屋を中央に配置するためのオフセット
            int offsetX = (sectionSizeInTiles - room.size.x) / 2;
            int offsetY = (sectionSizeInTiles - room.size.y) / 2;

            return new Vector2Int(sectionOriginX + offsetX, sectionOriginY + offsetY);
        }

        /// <summary>
        /// 指定したタイル座標がグリッドの範囲内かチェック
        /// </summary>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        /// <returns>グリッドの範囲内か</returns>
        private bool IsWithinTileGrid(int x, int y)
        {
            int totalMapSize = mapSize * sectionSizeInTiles;
            return x >= 0 && x < totalMapSize && y >= 0 && y < totalMapSize;
        }
        
        /// <summary>
        /// テンプレートからドア候補を探す
        /// </summary>
        /// <param name="room"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        private List<Vector2Int> GetDoorPositionsOnSide(RoomTemplate room, string side)
        {
            if (room == null)
                return new  List<Vector2Int>();

            Vector2Int size = room.size;
            switch (side)
            {
                case "North":
                    return room.potentialDoorPositions.Where(p => p.y == size.y - 1).ToList();
                case "South":
                    return room.potentialDoorPositions.Where(p => p.y == 0).ToList();
                case "East":
                    return room.potentialDoorPositions.Where(p => p.x == size.x - 1).ToList();
                case "West":
                    return room.potentialDoorPositions.Where(p => p.x == 0).ToList();
                default:
                    return new List<Vector2Int>();
            }
        }

        private void DebugPrintSections()
        {
            string mapLayout = "";
            for (int y = mapSize - 1; y >= 0; y--)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    Section current = sections[x, y];
                    string s = " ";

                    if (current.roomTemplate != null)
                    {
                        if (current.isStart)
                            s = "S";
                        else if (current.isGoal)
                            s = "G";
                        else
                            s = "R";
                    }

                    mapLayout += "[" + s + "]";
                }

                mapLayout += "\n";
            }
            
            Debug.Log("区画グリッドのレイアウト (Rは部屋あり):\n" + mapLayout);
        }

        private void DebugPrintTileGrid()
        {
            if (tileGrid != null)
            {
                string tileLayout = "";
                int totalSize = mapSize * sectionSizeInTiles;
                for (int y = totalSize - 1; y >= 0; y--)
                {
                    for (int x = 0; x < totalSize; x++)
                    {
                        switch (tileGrid[x, y])
                        {
                            case TileType.Empty:
                                tileLayout += " ";
                                break;
                            case TileType.Floor:
                                tileLayout += ".";
                                break;
                            case TileType.Wall:
                                tileLayout += "#";
                                break;
                            case TileType.Door:
                                tileLayout += "D";
                                break;
                        }
                    }

                    tileLayout += "\n";
                }

                Debug.Log("タイルグリッドのレイアウト:\n" + tileLayout);
            }   
        }
    }
}