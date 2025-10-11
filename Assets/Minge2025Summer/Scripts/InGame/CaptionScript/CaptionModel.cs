using System;
using Minge2025Summer.Main.InGame;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame
{
    public class CaptionModel : MonoBehaviour
    {
        [SerializeField] private float displayDuration = 3.0f;
        
        public float DisplayDuration => displayDuration;
        
        private readonly Subject<string> showSubject = new();
        private readonly Subject<string> hideSubject = new();
        public IObservable<string> OnShow => showSubject;
        public IObservable<string> OnHide => hideSubject;
        
        /// <summary>
        /// 指定チュートリアルの表示イベントを発火。
        /// </summary>
        public void RaiseShow(string text) => showSubject.OnNext(text);

        /// <summary>
        /// 指定チュートリアルの非表示イベントを発火。
        /// </summary>
        public void RaiseHide(string text) => hideSubject.OnNext(text);
    }
}