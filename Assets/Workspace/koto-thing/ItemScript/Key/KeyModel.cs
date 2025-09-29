using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class KeyModel : MonoBehaviour, IKey, IItem
    {
        [Header("鍵のアイテム設定")]
        [SerializeField, Tooltip("取得量")] private int amount;
        [SerializeField, Tooltip("鍵の表示名")] private string displayName;
        [SerializeField, Tooltip("鍵の説明文"), TextArea] private string description;

        [Header("フラグ")] 
        [SerializeField, Tooltip("取得したかどうか")] private bool isGet;
        [SerializeField, Tooltip("アイテムが使用されたかどうか")] private bool isApplied;

        [Header("鍵の2D画像")] 
        [SerializeField, Tooltip("インベントリに表示する画像")] private Sprite keySprite;
        
        [Header("鍵のID設定")]
        [SerializeField] private string keyID;
        
        private readonly Subject<Unit> onApplied = new ();
        private bool getIsApplied;
        public IObservable<Unit> OnApplied => onApplied;

        public int GetAmount => amount;
        public string GetDisplayName => displayName;
        public string GetDescription => description;
        public bool SetIsGet { get => isGet; set => isGet = value; }
        public bool GetIsApplied => isApplied;
        public Sprite GetSprite => keySprite;
        public string KeyID => keyID;

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