using System;
using FMODUnity;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.GunScript
{
    public class GunEmitter : MonoBehaviour
    {
        [Header("依存関係")]
        [SerializeField] private StudioEventEmitter aimEmitter;
        [SerializeField] private StudioEventEmitter reloadEmitter;
        [SerializeField] private StudioEventEmitter emptyReloadEmitter;
        [SerializeField] private StudioEventEmitter unEquipEmitter;
        [SerializeField] private StudioEventEmitter fireEmitter;
        [SerializeField] private StudioEventEmitter emptyFireEmitter;
        
        public void PlayAimSound() => aimEmitter.Play();
        public void PlayReloadSound() => reloadEmitter.Play();
        public void PlayEmptyReloadSound() => emptyReloadEmitter.Play();
        public void PlayUnEquipSound() => unEquipEmitter.Play();
        public void PlayFireSound() => fireEmitter.Play();
        public void PlayEmptyFireSound() => emptyFireEmitter.Play();

        /// <summary>
        /// FMODのリロード効果音（空マガジン／通常）を再生し、再生完了まで非同期に待機するストリームを返す。
        /// 呼び出し時点でイベントを再生開始。
        /// </summary>
        /// <param name="isEmptyReload">弾倉が空からのリロードならtrue、通常リロードならfalse。</param>
        /// <returns>
        /// 再生終了時にOnNext→OnCompletedが発行される(IObservable)。
        /// 対応するエミッターが未設定の場合は即時に完了。
        /// </returns>
        /// <remarks>
        /// 毎フレーム再生状態を監視し、PLAYBACK_STATE.STOPPEDで完了。
        /// 本オブジェクト破棄時はTakeUntilDestroy(this)により待機が中断。
        /// </remarks>
        public IObservable<Unit> PlayReloadAndWait(bool isEmptyReload)
        {
            var emitter = !isEmptyReload ? reloadEmitter : emptyReloadEmitter;
            if (emitter == null)
                return Observable.ReturnUnit();
            
            emitter.Play();
            
            return Observable.EveryUpdate()
                .TakeUntilDestroy(this)
                .Select(_ =>
                {
                    FMOD.Studio.PLAYBACK_STATE state;
                    emitter.EventInstance.getPlaybackState(out state);
                    return state;
                })
                .First(state => state == FMOD.Studio.PLAYBACK_STATE.STOPPED)
                .AsUnitObservable();
        }
    }
}