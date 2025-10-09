using System.Collections.Generic;
using UniRx;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Minge2025Summer.Main.InGame
{
    public class InformationScreenPresenter : MonoBehaviour
    {
        [SerializeField] private InformationScreenModel model;
        [SerializeField] private InformationScreenView view;

        [Header("フェード設定 (0以下で無効)")]
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float fadeOutDuration = 0.25f;
        [SerializeField] private Ease fadeEase = Ease.OutQuad;
        [SerializeField, Tooltip("フェード対象にする Graphic。未指定なら view 内の主要要素を自動収集")] private List<Graphic> fadeGraphics = new();

        [Header("表示切替")]
        [SerializeField, Tooltip("表示中に GameObject をアクティブ化し、非表示時に非アクティブ化する")]
        private bool toggleActive = true;

        private readonly CompositeDisposable disposables = new();
        private Sequence fadeSequence;
        private bool isVisible;

        private void Start()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            if (fadeGraphics.Count == 0)
            {
                fadeGraphics.AddRange(GetComponentsInChildren<Graphic>(true));
            }

            model.OnShow
                .Subscribe(data =>
                {
                    view.UpdateView(data);
                    if (toggleActive) gameObject.SetActive(true);
                    Fade(true);
                })
                .AddTo(disposables);

            model.OnHide
                .Subscribe(_ => Fade(false))
                .AddTo(disposables);
        }

        private void OnDisable()
        {
            disposables.Clear();
            KillFade();
        }

        private void OnDestroy()
        {
            disposables.Dispose();
            KillFade();
        }

        private void Fade(bool show)
        {
            if (fadeInDuration <= 0f && fadeOutDuration <= 0f)
            {
                SetAlpha(show ? 1f : 0f);
                isVisible = show;
                if (toggleActive && !show) 
                    gameObject.SetActive(false);
                
                return;
            }
            
            KillFade();
            
            float target = show ? 1f : 0f;
            float duration = show ? Mathf.Max(0f, fadeInDuration) : Mathf.Max(0f, fadeOutDuration);
            fadeSequence = DOTween.Sequence();
            foreach (var g in fadeGraphics)
            {
                if (g == null) 
                    continue;
                
                float start = g.color.a;
                fadeSequence.Join(DOTween.To(() => start, v =>
                {
                    start = v;
                    var c = g.color; c.a = v; g.color = c;
                }, target, duration).SetEase(fadeEase));
            }
            
            fadeSequence.OnComplete(() =>
            {
                isVisible = show;
                if (toggleActive && !show) 
                    gameObject.SetActive(false);
            });
        }

        /// <summary>
        /// アルファ値を直接設定
        /// </summary>
        /// <param name="a">設定するアルファ値</param>
        private void SetAlpha(float a)
        {
            foreach (var g in fadeGraphics)
            {
                if (g == null) continue;
                var c = g.color; c.a = a; g.color = c;
            }
        }

        /// <summary>
        /// 動作中のフェードを強制停止
        /// </summary>
        private void KillFade()
        {
            if (fadeSequence != null && fadeSequence.IsActive()) fadeSequence.Kill();
        }
    }
}
