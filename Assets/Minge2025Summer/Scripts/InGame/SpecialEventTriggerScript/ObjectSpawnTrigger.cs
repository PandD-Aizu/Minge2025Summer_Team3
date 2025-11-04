using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Minge2025Summer.Scripts.InGame.SpecialEventTriggerScript
{
    public class ObjectSpawnTrigger : MonoBehaviour
    {
        [Header("Spawn")]
        [SerializeField, Tooltip("Addressables のアドレス")]
        private string spawnObjectAddress;

        [SerializeField, Tooltip("スポーン位置")]
        private Transform spawnPointTransform;

        [Header("Visibility Check")]
        [SerializeField, Tooltip("可視判定に使うカメラ")]
        private Camera targetCamera;

        [SerializeField, Tooltip("生成予定物の見かけの大きさ")]
        private Vector3 wallBoundsSize = new Vector3(1f, 2f, 0.2f);

        [SerializeField, Tooltip("遮蔽物として扱うレイヤー")]
        private LayerMask occlusionMask = ~0;

        [Header("Strict Visibility")]
        [SerializeField, Min(0f), Tooltip("フラスタム判定を厳しくするための境界拡張")]
        private float frustumPadding = 1f;

        [SerializeField, Min(0f), Tooltip("不可視がこの秒数継続したら生成")]
        private float invisibleHoldSeconds = 0.1f;

        [Header("Player Gate")]
        [SerializeField, Tooltip("プレイヤーがこの距離以上離れたら生成を許可")]
        private float minDistanceFromPlayer = 0f;

        private bool entered;
        private bool spawned;
        private float invisibleTimer;
        private Transform player;
        private Collider selfCollider;

        private void Start()
        {
            if (targetCamera == null)
                targetCamera = Camera.main;

            selfCollider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                entered = true;
                player = other.transform;
            }
        }

        private void Update()
        {
            if (spawned || !entered || targetCamera == null)
                return;

            Vector3 spawnPos = spawnPointTransform ? spawnPointTransform.position : transform.position;
            Quaternion spawnRot = spawnPointTransform ? spawnPointTransform.rotation : transform.rotation;

            if (player != null && Vector3.Distance(player.position, spawnPos) < minDistanceFromPlayer)
                return;

            Bounds futureBounds = new Bounds(spawnPos, wallBoundsSize);

            bool visible = IsVisibleFromCamera(targetCamera, futureBounds);
            if (!visible)
            {
                invisibleTimer += Time.deltaTime;
                if (invisibleTimer >= invisibleHoldSeconds)
                {
                    Addressables.InstantiateAsync(spawnObjectAddress, spawnPos, spawnRot);
                    spawned = true;
                    enabled = false;
                }
            }
            else
            {
                invisibleTimer = 0f;
            }
        }

        /// <summary>
        /// 指定したバウンディングボックスがカメラから見えているかどうかを判定する
        /// </summary>
        /// <param name="cam">判定に使うカメラ</param>
        /// <param name="bounds">境界</param>
        /// <returns>見えていたらtrue、見えていなかったらfalse</returns>
        private bool IsVisibleFromCamera(Camera cam, Bounds bounds)
        {
            // フラスタム用に拡張
            Bounds padded = bounds;
            if (frustumPadding > 0f)
                padded.Expand(frustumPadding * 2f);

            var planes = GeometryUtility.CalculateFrustumPlanes(cam);
            bool inFrustum = GeometryUtility.TestPlanesAABB(planes, padded);
            if (!inFrustum)
                return false;

            // 中心 + 8コーナーのいずれかが見えていれば可視
            Vector3[] samples = GetBoundsSamples(padded);
            Vector3 camPos = cam.transform.position;

            for (int i = 0; i < samples.Length; i++)
            {
                if (!IsOccluded(camPos, samples[i]))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// カメラ位置からターゲット位置への視線が遮蔽されているかどうかを判定する
        /// </summary>
        /// <param name="camPos">カメラ位置</param>
        /// <param name="targetPos">ターゲットの位置</param>
        /// <returns>遮蔽されていたらtrue、遮蔽されていなかったらfalse</returns>
        private bool IsOccluded(Vector3 camPos, Vector3 targetPos)
        {
            Vector3 dir = targetPos - camPos;
            float dist = dir.magnitude;
            if (dist <= 0.001f) return false;

            var hits = Physics.RaycastAll(camPos, dir.normalized, dist - 0.01f, occlusionMask, QueryTriggerInteraction.Ignore);
            foreach (var h in hits)
            {
                if (selfCollider != null && h.collider == selfCollider)
                    continue;

                if (player != null && h.transform.IsChildOf(player))
                    continue;
                
                return true;
            }
            return false;
        }

        private static Vector3[] GetBoundsSamples(Bounds b)
        {
            Vector3 c = b.center;
            Vector3 e = b.extents;
            return new Vector3[]
            {
                c, // 中心
                c + new Vector3( e.x,  e.y,  e.z),
                c + new Vector3( e.x,  e.y, -e.z),
                c + new Vector3( e.x, -e.y,  e.z),
                c + new Vector3( e.x, -e.y, -e.z),
                c + new Vector3(-e.x,  e.y,  e.z),
                c + new Vector3(-e.x,  e.y, -e.z),
                c + new Vector3(-e.x, -e.y,  e.z),
                c + new Vector3(-e.x, -e.y, -e.z),
            };
        }
    }
}
