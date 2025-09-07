using UnityEngine;

namespace Test
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class VoxelMapGenerator : MonoBehaviour
    {
        [Header("マップ設定")]
        public int mapWidth = 50;
        public int mapHeight = 30;
        public int mapDepth = 50;
        public float noiseScale = 20f; // ノイズのスケール（値が小さいほど複雑に）
        [Range(0, 1)]
        public float surfaceThreshold = 0.5f; // 地表となるしきい値

        [Header("通路の生成設定")]
        public bool createGuaranteedPath = true; // スタートからゴールへの道を必ず作るか
        public int pathRadius = 2; // 道の太さ

        private float[,,] data; // 各地点の密度を格納するボクセルデータ
        private MeshFilter meshFilter;

        // --- Marching Cubesに必要なデータテーブル ---
        // (省略... MarchingCubes.csに実装)

        /// <summary>
        /// マップ生成のメイン処理
        /// </summary>
        public void GenerateMap()
        {
            meshFilter = GetComponent<MeshFilter>();

            // 1. ノイズで地形データを生成
            GenerateVoxelData();

            // 2. (オプション) スタートからゴールへの道を確保
            if (createGuaranteedPath)
            {
                CarveGuaranteedPath();
            }

            // 3. Marching Cubesでメッシュを生成
            MarchingCubes mc = new MarchingCubes();
            Mesh mesh = mc.CreateMesh(data, surfaceThreshold);

            // 4. メッシュを適用し、コライダーも更新
            meshFilter.mesh = mesh;
            gameObject.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard")); // デフォルトマテリアル
            
            // 既存のMeshColliderを削除して新しく追加
            if(GetComponent<MeshCollider>())
            {
                DestroyImmediate(GetComponent<MeshCollider>());
            }
            gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
        }

        /// <summary>
        /// 3Dパーリンノイズを使ってボクセルデータを生成する
        /// </summary>
        void GenerateVoxelData()
        {
            data = new float[mapWidth, mapHeight, mapDepth];
            float seedX = Random.Range(0f, 100f);
            float seedY = Random.Range(0f, 100f);
            float seedZ = Random.Range(0f, 100f);

            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    for (int z = 0; z < mapDepth; z++)
                    {
                        float nx = seedX + (float)x / mapWidth * noiseScale;
                        float ny = seedY + (float)y / mapHeight * noiseScale;
                        float nz = seedZ + (float)z / mapDepth * noiseScale;

                        // 3DパーリンノイズはUnityにないので、2Dを3回組み合わせて擬似的に生成
                        float noiseValue = (PerlinNoise3D(nx, ny, nz) + 1f) / 2f;
                        data[x, y, z] = noiseValue;
                    }
                }
            }
        }

        /// <summary>
        /// 2Dパーリンノイズを3回使って擬似的な3Dノイズを生成
        /// </summary>
        float PerlinNoise3D(float x, float y, float z)
        {
            float ab = Mathf.PerlinNoise(x, y);
            float bc = Mathf.PerlinNoise(y, z);
            float ac = Mathf.PerlinNoise(x, z);

            float ba = Mathf.PerlinNoise(y, x);
            float cb = Mathf.PerlinNoise(z, y);
            float ca = Mathf.PerlinNoise(z, x);

            float abc = ab + bc + ac + ba + cb + ca;
            return abc / 6f;
        }


        /// <summary>
        /// ランダムウォークで道を掘る
        /// </summary>
        void CarveGuaranteedPath()
        {
            Vector3Int startPos = new Vector3Int(pathRadius, pathRadius, pathRadius);
            Vector3Int goalPos = new Vector3Int(mapWidth - 1 - pathRadius, mapHeight - 1 - pathRadius, mapDepth - 1 - pathRadius);
            Vector3Int currentPos = startPos;

            while (Vector3.Distance(currentPos, goalPos) > pathRadius)
            {
                // 現在地周辺を掘る（密度を0にする）
                for (int x = -pathRadius; x <= pathRadius; x++)
                {
                    for (int y = -pathRadius; y <= pathRadius; y++)
                    {
                        for (int z = -pathRadius; z <= pathRadius; z++)
                        {
                            if (x * x + y * y + z * z <= pathRadius * pathRadius)
                            {
                                Vector3Int carvePos = currentPos + new Vector3Int(x, y, z);
                                data[carvePos.x, carvePos.y, carvePos.z] = 0;
                            }
                        }
                    }
                }
                
                // --- 次の移動方向を決定 ---
                Vector3 directionToGoal = ((Vector3)(goalPos - currentPos)).normalized;
                Vector3Int[] moveDirections = {
                    Vector3Int.up, Vector3Int.down, Vector3Int.left,
                    Vector3Int.right, new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1)
                };
                Vector3Int nextMove = Vector3Int.zero;
                float bestScore = -2f;

                if (Random.value < 0.8f)
                {
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
                    nextMove = moveDirections[Random.Range(0, moveDirections.Length)];
                }

                currentPos += nextMove;

                currentPos.x = Mathf.Clamp(currentPos.x, pathRadius, mapWidth - 1 - pathRadius);
                currentPos.y = Mathf.Clamp(currentPos.y, pathRadius, mapHeight - 1 - pathRadius);
                currentPos.z = Mathf.Clamp(currentPos.z, pathRadius, mapDepth - 1 - pathRadius);
            }
        }
    }
}