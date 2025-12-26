using System.Threading;
using Cysharp.Threading.Tasks;
using Minge2025Summer.Scripts.InGame.PlayerTransformScript;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.StageGimmick
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
    public class SpiderWebEvent : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField, Tooltip("糸が切れるまでの限界距離")] private float breakDistance = 3.0f;
        [SerializeField, Tooltip("引っ張られる影響範囲")] private float effectRadius = 2.0f;
        [SerializeField, Tooltip("引っ張りカーブ")] private AnimationCurve pullFalloff = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
        [SerializeField, Tooltip("戻る時間")] private float recoilDuration = 0.25f;
        [SerializeField, Tooltip("反動カーブ")] private AnimationCurve recoilCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("プレイヤー側に影響する速度低下量")]
        [SerializeField] private float playerSpeedReduction = 0.5f;
        
        private Mesh mesh;
        private Vector3[] originalVertices;
        private Vector3[] displacedVertices;
        private Collider webCollider; // 自身のコライダー
        
        private Transform stuckTarget;
        private Vector3 worldAnchorPoint;
        private bool isStuck = false;
        private bool isRecoiling = false;
        
        private PlayerPositionModel playerPositionModel;

        private CancellationTokenSource recoilCts;

        private void Awake()
        {
            // メッシュのセットアップ
            mesh = GetComponent<MeshFilter>().mesh;
            mesh.MarkDynamic();

            originalVertices = mesh.vertices;
            displacedVertices = new Vector3[originalVertices.Length];
            System.Array.Copy(originalVertices, displacedVertices, originalVertices.Length);
            
            // 自身のコライダーを取得（ClosestPoint計算用）
            webCollider = GetComponent<Collider>();
            
            // 安全対策：Triggerになっていなければ強制的にONにする
            if (webCollider != null)
            {
                webCollider.isTrigger = true;
            }
        }

        private void Update()
        {
            if (!isStuck) return;

            // ターゲット消失対策
            if (stuckTarget == null)
            {
                BreakWeb();
                return;
            }

            // 距離判定
            float currentDistance = Vector3.Distance(worldAnchorPoint, stuckTarget.position);
            if (currentDistance > breakDistance)
            {
                BreakWeb();
                return;
            }
            
            DeformMesh();
        }

        // Triggerで検知（弾き飛ばされない）
        private void OnTriggerEnter(Collider other)
        {
            if (isStuck || isRecoiling) return;

            if (other.CompareTag("Player"))
            {
                // エラー修正ポイント:
                // Triggerには「衝突点(contacts)」情報がないため、計算で求める。
                // 「クモの巣(自分)の表面上で、プレイヤーに一番近い点」を取得
                Vector3 hitPoint = webCollider.ClosestPoint(other.transform.position);
                playerPositionModel = other.GetComponentInChildren<PlayerPositionModel>();
                if (playerPositionModel != null) 
                    playerPositionModel.MoveSpeed *= playerSpeedReduction;
                
                StickToTarget(other.transform, hitPoint);
            }
        }

        void OnDestroy()
        {
            // 安全なキャンセル処理
            recoilCts?.Cancel();
            recoilCts?.Dispose();
        }

        private void StickToTarget(Transform target, Vector3 hitPoint)
        {
            // 前のタスクがあればキャンセル
            if (recoilCts != null)
            {
                recoilCts.Cancel();
                recoilCts.Dispose();
                recoilCts = null;
            }

            isStuck = true;
            isRecoiling = false;
            stuckTarget = target;
            worldAnchorPoint = hitPoint;
            
            Debug.Log("Web Stuck!");
        }

        private void DeformMesh()
        {
            // ターゲット（プレイヤー）の現在位置から、最初にくっついた点へのベクトル
            Vector3 pullOffsetWorld = stuckTarget.position - worldAnchorPoint;
            
            // ローカル座標系に変換
            Vector3 pullOffsetLocal = transform.InverseTransformVector(pullOffsetWorld);
            Vector3 localAnchorPoint = transform.InverseTransformPoint(worldAnchorPoint);

            for (int i = 0; i < originalVertices.Length; i++)
            {
                Vector3 originalPos = originalVertices[i];
                float dist = Vector3.Distance(originalPos, localAnchorPoint);

                // 距離に応じたウェイト計算
                float normalizedDist = Mathf.Clamp01(dist / effectRadius);
                float weight = pullFalloff.Evaluate(normalizedDist);

                // 頂点移動
                displacedVertices[i] = originalPos + (pullOffsetLocal * weight);
            }

            ApplyMeshChanges();
        }

        private void BreakWeb()
        {
            if (playerPositionModel != null)
            {
                playerPositionModel.MoveSpeed /= playerSpeedReduction;
            }
            
            isStuck = false;
            stuckTarget = null;

            // Destroy時のトークンを使って非同期処理を開始
            RecoilAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTaskVoid RecoilAsync(CancellationToken token)
        {
            isRecoiling = true;
            
            // CancellationTokenSourceの再生成
            recoilCts?.Dispose();
            recoilCts = CancellationTokenSource.CreateLinkedTokenSource(token);
            CancellationToken ct = recoilCts.Token;

            float elapsed = 0.0f;

            // アニメーション開始時の形状を保存
            Vector3[] startShapeVertices = new Vector3[originalVertices.Length];
            System.Array.Copy(displacedVertices, startShapeVertices, originalVertices.Length);

            try
            {
                while (elapsed < recoilDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / recoilDuration);
                    float curveValue = recoilCurve.Evaluate(t);

                    for (int i = 0; i < originalVertices.Length; i++)
                    {
                        displacedVertices[i] = Vector3.Lerp(startShapeVertices[i], originalVertices[i], curveValue);
                    }

                    ApplyMeshChanges();

                    await UniTask.NextFrame(ct);
                }

                // 最後にきっちり元に戻す
                System.Array.Copy(originalVertices, displacedVertices, originalVertices.Length);
                ApplyMeshChanges();
            }
            catch (System.OperationCanceledException)
            {
                // キャンセル時は何もしない
            }
            finally
            {
                isRecoiling = false;
            }
        }

        private void ApplyMeshChanges()
        {
            mesh.vertices = displacedVertices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds(); // バウンディングボックスの更新（表示抜け防止）
        }
    }
}