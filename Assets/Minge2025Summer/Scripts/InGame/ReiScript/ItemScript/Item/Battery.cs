using System;
using Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Interface;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Item
{
    public class Battery : MonoBehaviour, IConsumableItem
    {
        [Header("バッテリーのアイテム設定")]
        [SerializeField, Tooltip("取得量")] private int amount;
        [SerializeField, Tooltip("バッテリーの表示名")] private string displayName;
        [SerializeField, Tooltip("バッテリーの説明文"), TextArea] private string description;
        [SerializeField, Tooltip("回復量")] private float illuminationAmount;
         
        [Header("バッテリー2D画像")] 
        [SerializeField, Tooltip("インベントリに表示させる画像")] private Sprite batterySprite;
        
        private Subject<Unit> onGetItem = new ();
        public IObservable<Unit> OnGetItem => onGetItem;
        private Subject<float> onApplyItem = new ();
        public IObservable<float> OnApplyItem => onApplyItem;
        
        public int GetItemAmount => amount;
        public string GetItemName => "バッテリー";
        public string GetDisplayName => displayName;
        public string GetItemDescription => description;
        public float GetIlluminationAmount => illuminationAmount;
        public Sprite GetIcon => batterySprite;

        public void GetItem()
        {
            onGetItem.OnNext(Unit.Default);
        }
        
        public bool ApplyItem()
        {
            onApplyItem.OnNext(illuminationAmount);
            return true;
        }
        
        public void HideItem()
        {
            gameObject.SetActive(false);
        }
    }
}