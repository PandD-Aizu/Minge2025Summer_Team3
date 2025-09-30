using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerKeySysView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI captionText;
        [SerializeField] private List<string> captionMessages = new ();
        
        private bool isShowingCaption = false;

        /// <summary>
        /// ドアがあかないことを伝える画面下部のキャプションを更新する
        /// </summary>
        public void UpdateCaptionText()
        {
            // 既にキャプションを表示中の場合は無視
            if (isShowingCaption)
                return;

            isShowingCaption = true;

            // 既存のTweenを停止
            captionText.DOKill();

            // テキストを設定し、初期状態を透明にする
            captionText.text = captionMessages[Random.Range(0, captionMessages.Count)];
            Color color = captionText.color;
            color.a = 0.0f;
            captionText.color = color;

            var sequence = DOTween.Sequence();
            sequence
                .Append(captionText.DOFade(1.0f, 0.5f))  // フェードイン
                .AppendInterval(2.0f)                                   // 2秒間表示
                .Append(captionText.DOFade(0.0f, 0.5f))  // フェードアウト
                .OnComplete(() => isShowingCaption = false);            // アニメーション完了時にフラグをリセット
        }
    }
}