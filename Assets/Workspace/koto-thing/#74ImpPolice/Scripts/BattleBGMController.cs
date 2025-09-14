using Cysharp.Threading.Tasks;
using FMODUnity;
using UnityEngine;
using System;
using System.Threading;

namespace Workspace.koto_thing
{
    public class BattleBGMController : MonoBehaviour
    {
        [SerializeField] private StudioEventEmitter battleBGMEmitter;
        [SerializeField] private string bgmParameterName = "BattleHealthFlag";
        
        [SerializeField, Tooltip("BGMが停止する秒数")]
        private float defaultStopDelaySeconds = 5.0f;

        private CancellationTokenSource stopDelayCts;
        
        /// <summary>
        /// 戦闘時のBGMを再生する
        /// </summary>
        public void PlayBattleBGM()
        {
            stopDelayCts?.Cancel();
            stopDelayCts?.Dispose();
            stopDelayCts = null;

            if (!battleBGMEmitter.IsPlaying())
                battleBGMEmitter.Play();
        }
        
        /// <summary>
        /// すべてのBGMを停止する
        /// </summary>
        public void StopBattleBGM()
        {
            StopBattleBGM(defaultStopDelaySeconds).Forget();
        }

        /// <summary>
        /// 既定の秒数後にBGMを停止する
        /// </summary>
        /// <param name="delaySeconds">どれくらい待つか</param>
        public async UniTaskVoid StopBattleBGM(float delaySeconds)
        {
            stopDelayCts?.Cancel();
            stopDelayCts?.Dispose();
            stopDelayCts = new CancellationTokenSource();
            var token = stopDelayCts.Token;

            try
            {
                if (delaySeconds > 0f)
                    await UniTask.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken: token);

                if (token.IsCancellationRequested) 
                    return;

                if (battleBGMEmitter != null && battleBGMEmitter.IsPlaying())
                    battleBGMEmitter.Stop();
            }
            catch (OperationCanceledException)
            {
                
            }
        }

        /// <summary>
        /// BGMのパラメータを変更する
        /// </summary>
        /// <param name="currentHp">現在のPlayerのHP</param>
        /// <param name="maxHp">Playerの最大HP</param>
        public void ChangeBGM(float currentHp, float maxHp)
        {
            if (maxHp <= 0f) return;

            float ratio = Mathf.Clamp01(currentHp / maxHp);
            int nextValue = (ratio >= 0.6f) ? 0
                : (ratio >= 0.3f) ? 1
                : 2;
            
            RuntimeManager.StudioSystem.setParameterByName(bgmParameterName, nextValue);
        }

        private void OnDestroy()
        {
            stopDelayCts?.Cancel();
            stopDelayCts?.Dispose();
            stopDelayCts = null;
        }
    }
}