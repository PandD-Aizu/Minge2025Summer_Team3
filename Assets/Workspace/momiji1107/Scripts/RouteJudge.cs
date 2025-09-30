using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class RouteJudge : MonoBehaviour
{
    [SerializeField] private ArrangeMap arrangeMapScript;
    [SerializeField] private GameObject searchMarker;
    [SerializeField] private GameObject[] marker;
    [SerializeField] private NavMeshSurface navMeshSurface;

    [Header("Regenerate")]
    [SerializeField] private int maxRegenerateAttempts = 5;

    [Header("Sampling / Validation")]
    [SerializeField] private float startSampleRadius = 1.0f;
    [SerializeField] private float endSampleRadius = 0.25f;     // 終点は極力小さく
    [SerializeField] private float maxEndDeviation = 0.5f;      // 最終 corner と終点サンプル距離
    [SerializeField] private float maxHeightDiff = 0.6f;        // 高さ差
    [SerializeField] private bool strictJudge = true;

    [Header("Debug")]
    [SerializeField] private bool drawPath = true;
    [SerializeField] private Color pathColor = Color.red;
    [SerializeField] private float pathDrawTime = 2f;
    [SerializeField] private bool verboseLog = true;

    private readonly NavMeshPath path = new();
    private bool isGoodMap;
    private int attemptCount;

    void Start()
    {
        if (arrangeMapScript == null)
            arrangeMapScript = GetComponent<ArrangeMap>();
        if (arrangeMapScript != null)
            arrangeMapScript.MapReady += OnMapReady;
    }

    private void OnDestroy()
    {
        if (arrangeMapScript != null)
            arrangeMapScript.MapReady -= OnMapReady;
    }

    private void OnMapReady()
    {
        CheckMap();
        if (!isGoodMap)
        {
            attemptCount++;
            if (attemptCount <= maxRegenerateAttempts)
            {
                Debug.Log($"[RouteJudge] 再生成試行 {attemptCount}/{maxRegenerateAttempts}");
                _ = arrangeMapScript.RegenerateAsync();
            }
            else
            {
                Debug.LogError("[RouteJudge] 最大試行回数に達したため停止");
            }
        }
        else
        {
            attemptCount = 0;
        }
    }

    public void CheckMap()
    {
        isGoodMap = true;

        if (searchMarker == null || marker == null || marker.Length == 0)
        {
            Debug.LogWarning("[RouteJudge] マーカー設定不正");
            isGoodMap = false;
            return;
        }

        Vector3 rawStart = searchMarker.transform.position;

        for (int i = 0; i < marker.Length; i++)
        {
            var m = marker[i];
            if (m == null)
            {
                Debug.LogWarning($"[RouteJudge] マーカー {i} が null");
                isGoodMap = false;
                break;
            }

            string reason;
            if (!IsReachable(rawStart, m.transform.position, out reason))
            {
                Debug.Log($"[RouteJudge] マーカー {i}: 到達不可 -> {reason}");
                isGoodMap = false;
                break;
            }

            Debug.Log($"[RouteJudge] マーカー {i}: 到達可能");
        }

        if (isGoodMap)
            Debug.Log("[RouteJudge] 全マーカー到達可能(確定)");
    }

    private bool IsReachable(Vector3 startRaw, Vector3 endRaw, out string reason)
    {
        reason = "";

        // 始点サンプリング
        if (!NavMesh.SamplePosition(startRaw, out var startHit, startSampleRadius, NavMesh.AllAreas))
        {
            reason = "始点がNavMesh外";
            return false;
        }

        // 終点サンプリング(小さめ)
        if (!NavMesh.SamplePosition(endRaw, out var endHit, endSampleRadius, NavMesh.AllAreas))
        {
            reason = "終点がNavMesh外(精密)";
            return false;
        }

        // 高さ差チェック
        if (Mathf.Abs(endHit.position.y - endRaw.y) > maxHeightDiff)
        {
            reason = $"終点高さ差過大 rawY={endRaw.y:F2} hitY={endHit.position.y:F2}";
            if (strictJudge) return false;
        }

        // パス計算
        if (!NavMesh.CalculatePath(startHit.position, endHit.position, NavMesh.AllAreas, path))
        {
            reason = "CalculatePath失敗";
            return false;
        }

        if (path.status != NavMeshPathStatus.PathComplete)
        {
            reason = $"PathStatus={path.status}";
            return false;
        }

        if (path.corners == null || path.corners.Length < 2)
        {
            reason = "corner不足";
            return false;
        }

        // 最終 corner と終点サンプル点の距離
        var lastCorner = path.corners[path.corners.Length - 1];
        float endDeviation = Vector3.Distance(lastCorner, endHit.position);
        if (endDeviation > maxEndDeviation)
        {
            reason = $"終点偏差 {endDeviation:F2} > {maxEndDeviation:F2}";
            if (strictJudge) return false;
        }

        // corner→実際の rawEnd の遮蔽チェック(高さ差容認しつつ)
        if (NavMesh.Raycast(lastCorner, endHit.position, out var rayHit, NavMesh.AllAreas))
        {
            reason = $"終点直線上に遮蔽 hitDist={Vector3.Distance(lastCorner, rayHit.position):F2}";
            if (strictJudge) return false;
        }

        if (drawPath)
        {
            for (int i = 0; i < path.corners.Length - 1; i++)
                Debug.DrawLine(path.corners[i], path.corners[i + 1], pathColor, pathDrawTime);
        }

        if (verboseLog)
        {
            float pathLen = 0f;
            for (int i = 0; i < path.corners.Length - 1; i++)
                pathLen += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            Debug.Log($"[RouteJudge] pathLen={pathLen:F2} endDev={endDeviation:F2}");
        }

        return true;
    }
}
