using System;
using Minge2025Summer.Scripts.InGame.ItemScript.Interface;
using Minge2025Summer.Scripts.InGame.KeySystemScript.Interface;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ItemScript.Key
{
    public class KeyModel : MonoBehaviour, IKey, IItem, IAppliable
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

        public void AddAmount(int delta)
        {
            if (delta <= 0) 
                return;
            
            amount += delta;
            isGet = true;
            
            if (amount > 0) 
                isApplied = false;
        }

        public bool ConsumeOne()
        {
            if (amount <= 0) 
                return false;
            
            amount--;
            if (amount <= 0)
            {
                amount = 0;
                isApplied = true;
                onApplied.OnNext(Unit.Default);
            }
            
            return true;
        }

        public void ApplyItem()
        {
            if (!isGet || amount <= 0)
                return;
            
            ConsumeOne();
        }
    }
}