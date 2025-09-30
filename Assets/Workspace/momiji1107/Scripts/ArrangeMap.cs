using System;
using UnityEngine;
using Unity.AI.Navigation;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;

public class ArrangeMap : MonoBehaviour
{
    [SerializeField] private int mapSize;
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject StageRoot;
    [SerializeField] private GameObject StartRoom;
    [SerializeField] private GameObject GoalRoom;
    [SerializeField] private GameObject[] PrefabMaps;

    [Header("Async Settings")]
    [SerializeField] private int instantiateBatchSize = 16;
    [SerializeField] private int destroyBatchSize = 24;
    [SerializeField] private int destroyDelayFrame = 1;

    private RouteJudge routeJudgeScript;
    private int mapNum;
    private int mapRotate;
    private const float mapWidth = 20.0f;
    private readonly List<GameObject> Map = new();
    private int count;

    private bool isShuffleComplete = true;
    private CancellationTokenSource shuffleCts;
    private UniTask shuffleRunningTask;

    public bool IsShuffling => !isShuffleComplete;

    // マップ生成完了(ベイク後)通知イベント
    public event Action MapReady;

    async void Start()
    {
        routeJudgeScript = GetComponent<RouteJudge>();
        if (mapSize % 2 == 0)
            Debug.LogWarning("mapSize is even; odd size is recommended.");

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

    public UniTask ShuffleMap(CancellationToken token = default)
    {
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

            // ここで直接判定せずイベント発火
            MapReady?.Invoke();

            Debug.Log("Shuffle map complete");
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Shuffle map canceled");
        }
        finally
        {
            isShuffleComplete = true;
        }
    }

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

    public void DestroyMap()
    {
        Debug.Log("Destroy map (immediate)");
        foreach (var map in Map)
            if (map != null)
                Destroy(map);
        Map.Clear();
    }

    public async UniTask RegenerateAsync(CancellationToken token = default)
    {
        shuffleCts?.Cancel();
        if (shuffleRunningTask.Status == UniTaskStatus.Pending)
        {
            try { await shuffleRunningTask; } catch (OperationCanceledException) { }
        }
        shuffleCts?.Dispose();
        shuffleCts = new CancellationTokenSource();

        await DestroyMapAsync(token: token);

        using var linked = CancellationTokenSource.CreateLinkedTokenSource(token, shuffleCts.Token);
        await ShuffleMap(linked.Token);
    }

    private async UniTask MakeMapAsync(int i, int j, CancellationToken token)
    {
        await UniTask.SwitchToMainThread();
        var prefab = PrefabMaps.Length > 0 ? PrefabMaps[mapNum] : null;
        if (prefab == null)
        {
            Debug.LogWarning("Prefab is null - skipped");
            return;
        }
        var instance = Instantiate(prefab, new Vector3(mapWidth * i, 0.0f, mapWidth * j),
            Quaternion.Euler(0, 90 * mapRotate, 0));
        Map.Add(instance);
        instance.transform.SetParent(StageRoot.transform, true);
    }
}
