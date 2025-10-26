using System;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.Utility
{
    public class LoadScreenModel : MonoBehaviour
    {
        private ReactiveProperty<bool> isFinishLoading = new ReactiveProperty<bool>(false);
        public bool IsFinishLoading => isFinishLoading.Value;
        
        private ReactiveProperty<float> loadProgress = new ReactiveProperty<float>(0f);
        public float GetLoadProgress => loadProgress.Value;
        
        public IObservable<bool> IsFinishLoadingObservable => isFinishLoading.AsObservable();
        public IObservable<float> LoadProgressObservable => loadProgress.AsObservable();
        
        /// <summary>
        /// 進捗バーの表示を更新する
        /// </summary>
        public void UpdateProgressBar()
        {
            if (SceneController.SceneController.GetCurrentAsyncOperation != null)
            {
                // シーンのロード進捗は0.0から0.9までの範囲で報告されるため、0.9で割って正規化する
                loadProgress.Value = Mathf.Clamp01(SceneController.SceneController.GetCurrentAsyncOperation.progress / 0.9f);
            }
        }

        /// <summary>
        /// ロードが終了したかどうかをチェックする
        /// </summary>
        public void CheckFinishLoading()
        {
            if (GetLoadProgress >= 1.0f)
                isFinishLoading.Value = true;
            else
                isFinishLoading.Value = false;
        }
    }
}