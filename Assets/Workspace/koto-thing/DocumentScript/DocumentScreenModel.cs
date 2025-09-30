using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class DocumentScreenModel : MonoBehaviour
    {
        [SerializeField, Tooltip("ボディをページに自動分割する区切り文字")] private string autoPageSeparator = "\n---\n";
        
        private DocumentData documentData;
        private string[] pages = Array.Empty<string>();
        private int pageIndex;
        
        public Subject<DocumentScreenModel> OnChanged = new ();
        
        public bool IsLoaded => documentData != null;
        public string Title => documentData?.title ?? "Error: No Title";
        public int PageIndex => pageIndex;
        public int TotalPages => pages.Length;
        public bool HasMultiplePages => pages.Length > 1;
        public string CurrentPageText => pages.Length == 0 ? string.Empty : pages[Mathf.Clamp(pageIndex, 0, pages.Length - 1)];

        /// <summary>
        /// 新しい <see cref="DocumentData"/> を読み込み、ページ配列とページインデックスを初期化。
        /// 成功するとページインデックスは 0 にリセットされ、<see cref="OnChanged"/> が通知。
        /// </summary>
        /// <param name="newData">読み込むドキュメントデータ。null の場合は空状態として初期化。</param>
        public void Load(DocumentData newData)
        {
            documentData = newData;
            pages = documentData == null ? Array.Empty<string>() : documentData.GetEffectivePages(autoPageSeparator);
            pageIndex = 0;
            OnChanged.OnNext(this);
        }

        /// <summary>
        /// 次のページへ進みます。末尾ページの場合は何もしない。
        /// ページが 2 ページ未満（<see cref="HasMultiplePages"/> が false）の場合も処理されない。
        /// 成功時に <see cref="OnChanged"/> を通知。
        /// </summary>
        public void NextPage()
        {
            if (!HasMultiplePages) 
                return;
            
            if (pageIndex < pages.Length - 1)
            {
                pageIndex++;
                OnChanged.OnNext(this);
            }
        }

        /// <summary>
        /// 前のページへ戻る。先頭ページの場合は何もしない。
        /// ページが 2 ページ未満（<see cref="HasMultiplePages"/> が false）の場合も処理されない。
        /// 成功時に <see cref="OnChanged"/> を通知する。
        /// </summary>
        public void PrevPage()
        {
            if (!HasMultiplePages) 
                return;
            
            if (pageIndex > 0)
            {
                pageIndex--;
                OnChanged.OnNext(this);
            }
        }

        /// <summary>
        /// 指定インデックスのページを表示する。インデックスは自動で 0 ～ (総ページ数-1) にクランプされる。
        /// 現在ページと同じインデックスを指定した場合は通知しない。
        /// </summary>
        /// <param name="index">移動したいページ（0 始まり）。</param>
        public void SetPage(int index)
        {
            if (pages.Length == 0) 
                return;

            var clamped = Mathf.Clamp(index, 0, pages.Length - 1);
            if (clamped != pageIndex)
            {
                pageIndex = clamped;
                OnChanged.OnNext(this);
            }
        }

        /// <summary>
        /// MonoBehaviour 破棄時に通知ストリームを完了しリソースを解放。
        /// </summary>
        private void OnDestroy()
        {
            OnChanged?.OnCompleted();
            OnChanged?.Dispose();
        }
    }
}