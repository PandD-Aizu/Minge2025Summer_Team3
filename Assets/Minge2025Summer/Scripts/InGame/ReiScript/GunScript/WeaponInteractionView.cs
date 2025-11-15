using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript.GunScript
{
    public class WeaponInteractionView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI systemText;

        private CancellationTokenSource hideCts;

        /// <summary>
        /// メッセージを表示し、指定秒数後に非表示にする（デフォルト 2 秒）。
        /// </summary>
        public void ShowSystemText(string message, float duration = 2f)
        {
            // 既存の自動非表示をキャンセル
            hideCts?.Cancel();
            hideCts?.Dispose();
            hideCts = new CancellationTokenSource();

            systemText.text = message;
            systemText.gameObject.SetActive(true);

            HideAfterAsync(duration, hideCts.Token).Forget();
        }

        /// <summary>
        /// 即時で非表示にする
        /// </summary>
        public void HideSystemText()
        {
            systemText.text = string.Empty;
            systemText.gameObject.SetActive(false);

            hideCts?.Cancel();
            hideCts?.Dispose();
            hideCts = null;
        }

        private async UniTaskVoid HideAfterAsync(float seconds, CancellationToken token)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(seconds), cancellationToken: token);
                if (!token.IsCancellationRequested)
                {
                    HideSystemText();
                }
            }
            catch (OperationCanceledException)
            {
                // キャンセルは無視
            }
        }

        private void OnDestroy()
        {
            hideCts?.Cancel();
            hideCts?.Dispose();
            hideCts = null;
        }
    }
}