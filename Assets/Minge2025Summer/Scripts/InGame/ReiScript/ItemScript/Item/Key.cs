using System;
using Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Interface;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Item
{
    public class Key : MonoBehaviour, IKeyItem
    {
        [Header("鍵のアイテム設定")] 
        [SerializeField, Tooltip("取得量")] private int amount;
        [SerializeField, Tooltip("鍵の名前")] private string itemName;
        [SerializeField, Tooltip("鍵の表示名")] private string displayName;
        [SerializeField, Tooltip("鍵の説明文"), TextArea] private string description;
        
        [Header("鍵の2D画像")]
        [SerializeField, Tooltip("インベントリに表示する画像")] private Sprite keySprite;
        
        private Subject<Unit> onGetItem = new ();
        public IObservable<Unit> OnGetItem => onGetItem;
        private Subject<Unit> onApplyItem = new ();
        public IObservable<Unit> OnApplyItem => onApplyItem;
        
        public int GetItemAmount => amount;
        public string GetItemName => itemName;
        public string GetDisplayName => displayName;
        public string GetItemDescription => description;
        public Sprite GetIcon => keySprite;

        public void GetItem()
        {
            onGetItem.OnNext(Unit.Default);
        }
        
        public bool ApplyItem()
        {
            onApplyItem.OnNext(Unit.Default);
            return true;
        }
        
        public void HideItem()
        {
            transform.SetParent(null);
            gameObject.SetActive(false);
        }
    }
}