using UnityEngine;
using Unity.AI.Navigation;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class ArrangeMap : MonoBehaviour
{
    [SerializeField] private int mapSize; //マップの縦横の広さ(奇数で設定)
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject StageRoot; //親オブジェクト
    [SerializeField] private GameObject StartRoom; //スタート地点
    [SerializeField] private GameObject GoalRoom; //ゴール地点
    [SerializeField] private GameObject[] PrefabMaps; //Prefabのオブジェクト群

    private RouteJudge routeJudgeScript;
    private int mapNum; //マップ選択用のランダム値
    private int mapRotate; //マップの向き選択用のランダム値
    private const float mapWidth = 20.0f; //マップ配置の幅
    private List<GameObject> Map = new List<GameObject>();　//配置するマップ
    private int count;

    private bool isShuffleComplete;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        routeJudgeScript = gameObject.GetComponent<RouteJudge>();

        //mapSizeが偶数の場合ログを表示
        if ((mapSize / 2) == 0) Debug.Log("mapSize is not suitable.");

        Player.gameObject.transform.position = new Vector3(mapSize * mapWidth, 5.0f, ((mapSize - 1) / 2) * mapWidth);

        StartRoom.gameObject.transform.position = new Vector3(mapSize * mapWidth - 0.5f, 0.0f, ((mapSize - 1) / 2) * mapWidth);
        GoalRoom.gameObject.transform.position = new Vector3(-mapWidth + 0.5f, 0.0f, ((mapSize - 1) / 2) * mapWidth);

        await ShuffleMap();

    }

    // Update is called once per frame
    void Update()
    {
     
    }

    //マップを生成
    public async UniTask ShuffleMap()
    {
        Debug.Log("Shuffle map");
        count = 0;
        
        var tasks = new List<UniTask>();
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                //マップと向きを選択
                mapNum = Random.Range(0, PrefabMaps.Length);
                mapRotate = Random.Range(0, 3);

                var instance = await InstantiateAsync(PrefabMaps[mapNum], new Vector3(mapWidth * i, 0.0f, mapWidth * j), Quaternion.Euler(0, 90 * mapRotate, 0)).ToUniTask());

                //マップを配置
                Map.Add(instance);
                instance.transform.parent = StageRoot.transform;
                //Debug.Log("(" + i + "," + j + "), mapNum=" + mapNum + ", mapRotate=" + mapRotate);

                count++;
            }
        }

        UniTask.WhenAll(tasks);

        //NavMeshSurfaceを更新
        StageRoot.GetComponent<NavMeshSurface>().BuildNavMesh();

        routeJudgeScript.CheckMap();
    }

    //マップを削除する
    public void DestroyMap()
    {
        Debug.Log("Destroy map");
        foreach (GameObject map in Map) Destroy(map);
        Map.Clear();
    }

    private async UniTask MakeMap(int i, int j)
    {
        //マップを配置
        GameObject instance = InstantiateAsync(PrefabMaps[mapNum], new Vector3(mapWidth * i, 0.0f, mapWidth * j), Quaternion.Euler(0, 90 * mapRotate, 0)).Result[0];
        Map.Add(instance);
        instance.transform.parent = StageRoot.transform;
        //Debug.Log("(" + i + "," + j + "), mapNum=" + mapNum + ", mapRotate=" + mapRotate);
    }

}
