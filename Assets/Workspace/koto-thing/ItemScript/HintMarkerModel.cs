using UnityEngine;

public class HintMarkerModel : MonoBehaviour
{
    [Header("距離フェード設定")]
    [SerializeField] private float maxDistance = 6f;
    [SerializeField] private float minDistance = 1.5f;

    [Header("表示位置調整")]
    [SerializeField] private Transform targetRoot;          // 追従するアイテム(省略時は自身)
    [SerializeField] private float verticalOffset = 0.15f;  // Bounds 上端からの追加オフセット
    [SerializeField] private bool useRendererBounds = true; // 高さ計算に Renderer Bounds を利用
    [SerializeField] private float extraHeight = 0.0f;      // 任意追加高さ

    [Header("スケール制御")]
    [SerializeField] private bool scaleWithDistance = true;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.Linear(0, 1, 1, 1); // x: (distance/maxDist)
    [SerializeField] private float baseScale = 1.0f;
    [SerializeField] private float maxScaleMultiplier = 1.0f;

    [Header("回転制御")]
    [SerializeField] private bool billboard = true;

    private Transform playerTransform;

    public float MaxDistance => maxDistance;
    public float MinDistance => minDistance;
    public Transform PlayerTransform { get => playerTransform; set => playerTransform = value; }

    public Transform TargetRoot
    {
        get => targetRoot != null ? targetRoot : transform.parent != null ? transform.parent : transform;
        set => targetRoot = value;
    }

    public float VerticalOffset => verticalOffset;
    public bool UseRendererBounds => useRendererBounds;
    public float ExtraHeight => extraHeight;
    public bool ScaleWithDistance => scaleWithDistance;
    public AnimationCurve ScaleCurve => scaleCurve;
    public float BaseScale => baseScale;
    public float MaxScaleMultiplier => maxScaleMultiplier;
    public bool Billboard => billboard;
}