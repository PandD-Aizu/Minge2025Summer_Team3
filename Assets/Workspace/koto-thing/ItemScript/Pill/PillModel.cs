using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class PillModel : MonoBehaviour, IItem
    {
        [Header("回復アイテムの設定")] 
        [SerializeField, Tooltip("得られる回復アイテムの個数")] private int pillCount; 
        [SerializeField, Tooltip("回復量")] private int healAmount;

        [Header("インベントリUIの設定")] 
        [SerializeField, Tooltip("表示名")] private string displayName;
        [SerializeField, Tooltip("説明文"), TextArea] private string description;

        [Header("フラグ")] 
        [SerializeField, Tooltip("取得済みか")] private bool isGet;
        [SerializeField, Tooltip("使用済みか")] private bool isApplied;
        
        [Header("回復アイテム2D画像")] 
        [SerializeField] private Sprite pillSprite;
        
        private Subject<Unit> onApplied = new ();
        public IObservable<Unit> OnApplied => onApplied;

        public int GetAmount => healAmount;
        public string GetDisplayName => displayName;
        public string GetDescription => description;
        public Sprite GetSprite => pillSprite;
        public bool SetIsGet { get => isGet; set => isGet = value; }
        public bool GetIsApplied => isApplied;

        public void ApplyItem()
        {
            if (isApplied || !isGet)
                return;

            isApplied = true;
            onApplied.OnNext(Unit.Default);
            onApplied.OnCompleted();
        }
    }
}
