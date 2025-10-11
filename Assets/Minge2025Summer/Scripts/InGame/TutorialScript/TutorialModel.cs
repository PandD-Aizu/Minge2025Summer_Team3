using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Main.InGame
{
    public class TutorialModel : MonoBehaviour
    {
        [Header("チュートリアルテキスト")]
        [SerializeField, Tooltip("フラッシュライトのチュートリアルテキスト")] private string flashLightTutorialText;
        [SerializeField, Tooltip("移動操作のチュートリアルテキスト")] private string moveTutorialText;
        [SerializeField, Tooltip("ダッシュ操作のチュートリアルテキスト")] private string dashTutorialText;
        [SerializeField, Tooltip("しゃがみ操作のチュートリアルテキスト")] private string crouchTutorialText;
        [SerializeField, Tooltip("インタラクト操作のチュートリアルテキスト")] private string interactTutorialText;
        [SerializeField, Tooltip("射撃のチュートリアルテキスト")] private string shootTutorialText;
        [SerializeField, Tooltip("照準操作のチュートリアルテキスト")] private string ascendTutorialText;
        [SerializeField, Tooltip("インベントリ操作のチュートリアルテキスト")] private string inventoryTutorialText;
        [SerializeField, Tooltip("情報画面のチュートリアルテキスト")] private string infoTutorialText;
        [SerializeField, Tooltip("リロード操作のチュートリアルテキスト")] private string reloadTutorialText;

        private readonly Dictionary<TutorialType, string> textTable = new();
        private bool initialized;

        // 表示/非表示イベント
        private readonly Subject<TutorialType> showSubject = new();
        private readonly Subject<TutorialType> hideSubject = new();
        public IObservable<TutorialType> OnShow => showSubject;
        public IObservable<TutorialType> OnHide => hideSubject;

        /// <summary>
        /// 初期化（シリアライズされたテキストを辞書へロード）
        /// </summary>
        public void Initialize()
        {
            if (initialized) 
                return;
            
            textTable[TutorialType.FLASHLIGHT] = flashLightTutorialText;
            textTable[TutorialType.DASH] = dashTutorialText;
            textTable[TutorialType.RELOAD] = reloadTutorialText;
            textTable[TutorialType.MOVE] = moveTutorialText;
            textTable[TutorialType.CROUCH] = crouchTutorialText;
            textTable[TutorialType.INTERACT] = interactTutorialText;
            textTable[TutorialType.SHOOT] = shootTutorialText;
            textTable[TutorialType.ASCEND] = ascendTutorialText;
            textTable[TutorialType.INVENTORY] = inventoryTutorialText;
            textTable[TutorialType.INFO] = infoTutorialText;
            
            initialized = true;
        }

        /// <summary>
        /// 対象種別のテキストを取得する。
        /// </summary>
        /// <param name="type">チュートリアルの種類</param>
        public string GetText(TutorialType type)
        {
            Initialize();
            return textTable.TryGetValue(type, out var txt) ? txt : string.Empty;
        }

        /// <summary>
        /// 指定チュートリアルの表示イベントを発火。
        /// </summary>
        public void RaiseShow(TutorialType type) => showSubject.OnNext(type);

        /// <summary>
        /// 指定チュートリアルの非表示イベントを発火。
        /// </summary>
        public void RaiseHide(TutorialType type) => hideSubject.OnNext(type);
    }
}