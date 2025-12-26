using System.Linq;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using Minge2025Summer.Scripts.InGame.ShootableObject.Interface;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ShootableObject
{
    public class WoodCrate : MonoBehaviour, IShootableObject
    {
        [SerializeField, Tooltip("破壊時の効果音")] private StudioEventEmitter destroySound;
        [SerializeField] private float destroyDelay = 2.0f;
        
        /// <summary>
        /// オブジェクトを破壊する
        /// </summary>
        public async void Feedback()
        {
            gameObject.GetComponentsInChildren<Rigidbody>()
                .ToList()
                .ForEach(r => {
                    r.isKinematic = false;
                    r.transform.SetParent(null);
                    
                    var vect = new Vector3(
                        Random.Range(-3f, 3f),
                        Random.Range(0f, 3f),
                        Random.Range(-3f, 3f)
                    );
                    r.AddForce(vect, ForceMode.Impulse);
                    r.AddTorque(vect, ForceMode.Impulse);
                    
                    Destroy(r.gameObject, destroyDelay);
                });
            
            // 効果音の再生が終わるまで待機
            destroySound.Play();
            await UniTask.DelayFrame(1);
            
            // 効果音の再生が終わるまで待機
            try
            {
                var instance = destroySound.EventInstance;
                // EventInstanceが有効であれば再生状態をポーリングして待機
                if (instance.isValid())
                {
                    PLAYBACK_STATE state;
                    do
                    {
                        instance.getPlaybackState(out state);
                        await UniTask.Yield(); // 次のフレームまで待つ
                    } while (state != PLAYBACK_STATE.STOPPED);
                }
                else
                {
                    // EventInstanceが取得できない場合はフォールバックで destroyDelay 秒待機
                    await UniTask.Delay((int)(destroyDelay * 1000));
                }
            }
            catch
            {
                await UniTask.Delay((int)(destroyDelay * 1000));
            }
            
            // 親のオブジェクトを含めて破壊
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}