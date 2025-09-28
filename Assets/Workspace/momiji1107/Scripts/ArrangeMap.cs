using UnityEngine;
using Unity.AI.Navigation;

public class ArrangeMap : MonoBehaviour
{
    [SerializeField] private int mapSize; //マップの縦横の広さ(奇数で設定)
    [SerializeField] private GameObject StageRoot;
    [SerializeField] private GameObject[] PrefabMaps; //Prefabのオブジェクト群

    private int mapNum; //マップ選択用のランダム値
    private int mapRotate; //マップの向き選択用のランダム値
    private GameObject Map;　//配置するマップ

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //mapSizeが偶数の場合ログを表示
        if ((mapSize / 2) == 0) Debug.Log("mapSize is not suitable.");

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                //マップと向きを選択
                mapNum = Random.Range(0, PrefabMaps.Length);
                mapRotate = Random.Range(0, 3);

                //マップを配置
                GameObject newObj = Instantiate(PrefabMaps[mapNum], new Vector3(20.0f * i, -1.0f, 20.0f * j), Quaternion.Euler(0, 90 * mapRotate, 0));
                newObj.transform.parent = StageRoot.gameObject.transform;
                Debug.Log("(" + i + "," + j + "), mapNum=" + mapNum + ", mapRotate=" + mapRotate);
            }
        }

        StageRoot.GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
     
    }

}
