using System;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Minge2025Summer.Scripts.InGame.GameOverScript
{
    public class GameOverModel : MonoBehaviour
    {
        [SerializeField, Tooltip("インゲームのシーン名")] private string inGameSceneAddress;
        [SerializeField, Tooltip("タイトル画面のシーン名")] private string titleSceneAddress;
        
        private readonly Subject<Unit> onContinueGame = new ();
        public IObservable<Unit> OnContinueGame => onContinueGame.AsObservable();

        /// <summary>
        /// コンティニュー時の処理
        /// </summary>
        public void ContinueGame()
        {
            Addressables.LoadSceneAsync(inGameSceneAddress);
        }
        
        /// <summary>
        /// タイトルシーンをロードする
        /// </summary>
        public void LoadTitleScene()
        {
            Addressables.LoadSceneAsync(titleSceneAddress);
        }
    }
}
