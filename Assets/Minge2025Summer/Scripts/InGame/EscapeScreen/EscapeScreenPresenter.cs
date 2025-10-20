using System;
using Minge2025Summer.Scripts.InGame.EscapeScreen.Enum;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EscapeScreen
{
    public class EscapeScreenPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private EscapeScreenModel model;
        [SerializeField] private EscapeScreenView view;

        private CompositeDisposable disposables = new ();

        private void Start()
        {
            SubscribeEvents();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                view.ToggleEscapeScreen();
            }
        }

        private void SubscribeEvents()
        {
            view.ResumeButton.onClick
                .AddListener(() =>
                {
                    view.ToggleEscapeScreen();
                });
            
            view.OptionButton.onClick
                .AddListener(() =>
                {
                    // not implemented
                });
            
            view.BackToTitleButton.onClick
                .AddListener(() =>
                {
                    view.ShowConfirmQuit("タイトルに戻りますか？", ConfirmState.BACK_TO_TITLE);
                });

            view.QuitGameButton.onClick
                .AddListener(() =>
                {
                    view.ShowConfirmQuit("ゲームを終了しますか？", ConfirmState.QUIT_GAME);
                });
            
            view.ApplyButton.onClick
                .AddListener(() =>
                {
                    view.HideConfirmQuit();
                });
            
            view.CancelButton.onClick
                .AddListener(() =>
                {
                    view.HideConfirmQuit();
                });
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            disposables.Dispose();
            view.ResumeButton.onClick.RemoveAllListeners();
            view.OptionButton.onClick.RemoveAllListeners();
            view.BackToTitleButton.onClick.RemoveAllListeners();
            view.QuitGameButton.onClick.RemoveAllListeners();
        }
    }
}