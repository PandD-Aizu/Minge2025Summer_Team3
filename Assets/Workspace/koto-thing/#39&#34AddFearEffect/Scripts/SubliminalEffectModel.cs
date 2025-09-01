using System.Collections;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Workspace.koto_thing
{
    public class SubliminalEffectModel : MonoBehaviour
    {
        [Header("タイミング設定")] 
        [SerializeField] private float minDisplayTime = 0.016f;
        [SerializeField] private float maxDisplayTime = 0.064f;
        [SerializeField] private float minWaitTime = 5.0f;
        [SerializeField] private float maxWaitTime = 20.0f;
        
        public Subject<int> OnSelectImage { get; } = new Subject<int>();
        public Subject<bool> OnSwitchImage { get; } = new Subject<bool>();

        public async UniTask ShowEffectRoutine(int subliminalImageCount)
        {
            while (true)
            {
                // 次に表示するまでの時間をランダムに決める
                float waitTime = Random.Range(minWaitTime, maxWaitTime);
                float displayTime = Random.Range(minDisplayTime, maxDisplayTime);
                await UniTask.WaitForSeconds(waitTime);
            
                // 表示する画像をランダムに選ぶ
                int randomIndex = Random.Range(0, subliminalImageCount);
                OnSelectImage.OnNext(randomIndex);
            
                // 画像を表示
                OnSwitchImage.OnNext(true);
            
                // 指定した時間だけ表示
                await UniTask.WaitForSeconds(displayTime);
            
                // 画像を非表示に戻す
                OnSwitchImage.OnNext(false);
            }
        }
    }
}