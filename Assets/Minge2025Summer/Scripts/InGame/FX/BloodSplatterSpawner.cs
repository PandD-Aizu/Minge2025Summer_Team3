using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.VFX;

namespace Minge2025Summer.Scripts.InGame.FX
{
    public sealed class BloodSplatterSpawner : MonoBehaviour
    {
        private static BloodSplatterSpawner instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (instance != null) 
                return;
            
            var handleObject = new GameObject("BloodSplatterSpawner");
            DontDestroyOnLoad(handleObject);
            instance = handleObject.AddComponent<BloodSplatterSpawner>();
        }

        /// <summary>
        /// エフェクトをスポーンする
        /// </summary>
        /// <param name="addressKey">Addressablesに登録したキー</param>
        /// <param name="position">生成位置</param>
        /// <param name="normal">生成面の法線</param>
        /// <param name="speed">速度</param>
        /// <param name="lifetime">存続時間</param>
        /// <param name="surfaceOffset">生成面のオフセット</param>
        public static void Spawn(string addressKey, Vector3 position, Vector3 normal, float speed = 8.0f,
            float lifetime = 2.5f, float surfaceOffset = 0.01f)
        {
            if (instance == null) 
                Bootstrap();

            instance.StartCoroutine(instance.SpawnRoutine(addressKey, position + normal.normalized * surfaceOffset,
                normal, speed, lifetime));
            
        }
        
        /// <summary>
        /// VFXのスポーン処理コルーチン
        /// </summary>
        /// <param name="addressKey">Addressablesに登録したキー</param>
        /// <param name="position">生成位置</param>
        /// <param name="normal">生成面の法線の向き</param>
        /// <param name="speed">速度</param>
        /// <param name="lifetime">存続時間</param>
        /// <returns></returns>
        private IEnumerator SpawnRoutine(string addressKey, Vector3 position, Vector3 normal, float speed, float lifetime)
        {
            var rotation = Quaternion.LookRotation(normal.normalized, Vector3.up);
            var handle = Addressables.InstantiateAsync(addressKey, position, rotation);
            yield return handle;

            var handleObject = handle.Result;
            if (handleObject == null)
                yield break;
            
            // VFX Graphがあるなら速度を設定
            if (handleObject.TryGetComponent<VisualEffect>(out var vfx))
            {
                if (vfx.HasVector3("_velocity"))
                    vfx.SetVector3("_velocity", normal.normalized * speed);
                else
                    handleObject.transform.rotation = Quaternion.LookRotation(normal.normalized, Vector3.up);
            }
            // Rigidbodyがあるなら速度を設定
            else if (handleObject.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody.linearVelocity = normal.normalized * speed;
            }
            // それ以外なら向きを法線方向に設定
            else
            {
                handleObject.transform.rotation = Quaternion.LookRotation(normal.normalized, Vector3.up);
            }

            // 指定時間後に破棄
            if (lifetime > 0)
            {
                yield return new WaitForSeconds(lifetime);
                if (handleObject != null)
                    Addressables.ReleaseInstance(handleObject);
            }
        }
    }
}