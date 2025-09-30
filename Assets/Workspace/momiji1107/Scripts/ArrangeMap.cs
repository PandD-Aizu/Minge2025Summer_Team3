using System;
using UnityEngine;
using Unity.AI.Navigation;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;

public class ArrangeMap : MonoBehaviour
{
    [SerializeField] private int mapSize; // マップの縦横の広さ(奇数で設定推奨)
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject StageRoot; // 親オブジェクト
    [SerializeField] private GameObject StartRoom; // スタート地点
    [SerializeField] private GameObject GoalRoom; // ゴール地点
    [SerializeField] private GameObject[] PrefabMaps; // Prefabのオブジェクト群
    
    [Header("Async Settings")] 
    [SerializeField] private int instantiateBatchSize = 16; // 何個生成したら1フレームYieldするか
    [SerializeField] private int destroyBatchSize = 24; // 破棄バッチサイズ
    [SerializeField] private int destroyDelayFrame = 1; // バッチ毎の遅延フレーム数

    private RouteJudge routeJudgeScript;
    private int mapNum; // マップ選択用のランダム値
    private int mapRotate; // マップの向き選択用のランダム値
    private const float mapWidth = 20.0f; // マップ配置の幅
    private readonly List<GameObject> Map = new (); // 配置するマップ
    private int count;

    private bool isShuffleComplete = true; // 初期は生成していないので true 扱い
    private CancellationTokenSource shuffleCts;
    private UniTask shuffleRunningTask; // 現在進行中のShuffleタスク

    public bool IsShuffling => !isShuffleComplete;

    async void Start()
    {
        routeJudgeScript = GetComponent<RouteJudge>();

        if (mapSize % 2 == 0)
            Debug.LogWarning("mapSize is even; odd size is recommended for symmetry.");

        Player.transform.position = new Vector3(mapSize * mapWidth, 5.0f, ((mapSize - 1) / 2f) * mapWidth);
        StartRoom.transform.position = new Vector3(mapSize * mapWidth - 0.5f, 0.0f, ((mapSize - 1) / 2f) * mapWidth);
        GoalRoom.transform.position = new Vector3(-mapWidth + 0.5f, 0.0f, ((mapSize - 1) / 2f) * mapWidth);

        shuffleCts = new CancellationTokenSource();
        await ShuffleMap(shuffleCts.Token);
    }

    void OnDestroy()
    {
        shuffleCts?.Cancel();
        shuffleCts?.Dispose();
    }

    /// <summary>
    /// マップ生成(非同期)要求。進行中であれば既存タスクを返す。
    /// </summary>
    public UniTask ShuffleMap(CancellationToken token = default)
    {
        // 進行中ならそのタスクを返す
        if (shuffleRunningTask.Status == UniTaskStatus.Pending)
            return shuffleRunningTask;
        shuffleRunningTask = InternalShuffleMap(token);
        return shuffleRunningTask;
    }

    private async UniTask InternalShuffleMap(CancellationToken token)
    {
        Debug.Log("Shuffle map start");
        isShuffleComplete = false;
        count = 0;

        try
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    token.ThrowIfCancellationRequested();
                    mapNum = UnityEngine.Random.Range(0, PrefabMaps.Length);
                    mapRotate = UnityEngine.Random.Range(0, 4);
                    await MakeMapAsync(i, j, token);
                    count++;
                    if (count % instantiateBatchSize == 0)
                        await UniTask.Yield(PlayerLoopTiming.Update, token);
                }
            }

            await UniTask.SwitchToMainThread();
            StageRoot.GetComponent<NavMeshSurface>().BuildNavMesh();
            routeJudgeScript.CheckMap();
            Debug.Log("Shuffle map complete");
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Shuffle map canceled");
        }
        finally
        {
            isShuffleComplete = true; // キャンセルでもtrueにして再要求可能
        }
    }

    /// <summary>
    /// マップを段階的に破棄する(非同期)。
    /// </summary>
    public async UniTask DestroyMapAsync(int batchSize = -1, int delayFrame = -1, CancellationToken token = default)
    {
        if (Map.Count == 0) return;
        if (batchSize <= 0) batchSize = destroyBatchSize;
        if (delayFrame < 0) delayFrame = destroyDelayFrame;
        
        int destroyed = 0;
        var list = new List<GameObject>(Map);
        Map.Clear();

        foreach (var go in list)
        {
            token.ThrowIfCancellationRequested();
            if (go != null) Destroy(go);
            destroyed++;
            if (destroyed % batchSize == 0)
                await UniTask.DelayFrame(delayFrame, PlayerLoopTiming.Update, token);
        }
        await Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// マップ即時破棄（テスト/デバッグ用途）。
    /// </summary>
    public void DestroyMap()
    {
        Debug.Log("Destroy map (immediate)");
        foreach (var map in Map)
            if (map != null)
                Destroy(map);
        Map.Clear();
    }

    /// <summary>
    /// 進行中の生成をキャンセルし安全に再生成。
    /// </summary>
    public async UniTask RegenerateAsync(CancellationToken token = default)
    {
        shuffleCts?.Cancel();
        if (shuffleRunningTask.Status == UniTaskStatus.Pending)
        {
            try { await shuffleRunningTask; } catch (OperationCanceledException) { }
        }
        shuffleCts?.Dispose();
        shuffleCts = new CancellationTokenSource();

        // 既存破棄
        await DestroyMapAsync(token: token);

        using var linked = CancellationTokenSource.CreateLinkedTokenSource(token, shuffleCts.Token);
        await ShuffleMap(linked.Token);
    }

    /// <summary>
    /// 1セル分のPrefab生成。
    /// </summary>
    private async UniTask MakeMapAsync(int i, int j, CancellationToken token)
    {
        await UniTask.SwitchToMainThread();
        var prefab = PrefabMaps.Length > 0 ? PrefabMaps[mapNum] : null;
        if (prefab == null)
        {
            Debug.LogWarning("Prefab is null - skipped");
            return;
        }
        var instance = Instantiate(prefab, new Vector3(mapWidth * i, 0.0f, mapWidth * j), Quaternion.Euler(0, 90 * mapRotate, 0));
        Map.Add(instance);
        instance.transform.SetParent(StageRoot.transform, worldPositionStays: true);
    }
}