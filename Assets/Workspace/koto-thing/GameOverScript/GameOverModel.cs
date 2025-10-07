using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class GameOverModel : MonoBehaviour
    {
        [SerializeField, Tooltip("タイトル画面のシーン名")] private string titleSceneAddress;

        private Subject<Unit> onContinueGame = new ();
        public IObservable<Unit> OnContinueGame => onContinueGame.AsObservable();

        /// <summary>
        /// コンティニュー時の処理
        /// </summary>
        public void ContinueGame()
        {
            onContinueGame.OnNext(Unit.Default);   
        }
        
        /// <summary>
        /// タイトルシーンをロードする
        /// </summary>
        public void LoadTitleScene()
        {
            SceneController.LoadSceneAsync(titleSceneAddress);
        }
    }
}
