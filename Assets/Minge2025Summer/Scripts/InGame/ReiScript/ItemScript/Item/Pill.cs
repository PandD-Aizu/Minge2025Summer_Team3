using System;
using Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Interface;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Item
{
    public class Pill : MonoBehaviour, IConsumableItem
    {
        [Header("回復アイテムの設定")] 
        [SerializeField, Tooltip("得られる回復アイテムの個数")] private int pillCount;
        [SerializeField, Tooltip("回復量")] private int healAmount;

        [Header("インベントリUIの設定")] 
        [SerializeField, Tooltip("アイテム名")] private string itemName;
        [SerializeField, Tooltip("表示名")] private string displayName;
        [SerializeField, Tooltip("説明文"), TextArea] private string description;

        [Header("回復アイテム２D画像")] 
        [SerializeField, Tooltip("インベントリに表示させる画像")] private Sprite pillSprite;
        
        private Subject<Unit> onGetItem = new ();
        public IObservable<Unit> OnGetItem => onGetItem;
        private Subject<float> onApplyItem = new ();
        public IObservable<float> OnApplyItem => onApplyItem;
        
        public string GetItemName => itemName;
        public string GetDisplayName => displayName;
        public string GetItemDescription => description;
        public int GetItemAmount => pillCount;
        public Sprite GetIcon => pillSprite;
        public int GetHealAmount => healAmount;

        public void GetItem()
        {
            onGetItem.OnNext(Unit.Default);
        }
        
        public bool ApplyItem()
        {
            onApplyItem.OnNext(healAmount);
            return true;
        }
        
        public void HideItem()
        {
            transform.SetParent(null);
            gameObject.SetActive(false);
        }
    }
}