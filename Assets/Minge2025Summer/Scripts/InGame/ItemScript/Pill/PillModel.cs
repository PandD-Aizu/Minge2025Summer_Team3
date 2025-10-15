using System;
using Minge2025Summer.Scripts.InGame.ItemScript.Interface;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ItemScript.Pill
{
    public class PillModel : MonoBehaviour, IItem, IAppliable
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
        [SerializeField, Tooltip("インベントリに表示させる画像")] private Sprite pillSprite;
        
        private readonly Subject<Unit> onApplied = new ();
        public IObservable<Unit> OnApplied => onApplied;

        public int GetAmount => pillCount;
        public int GetHealAmount => healAmount;
        public string GetDisplayName => displayName;
        public string GetDescription => description;
        public Sprite GetSprite => pillSprite;
        public bool SetIsGet { get => isGet; set => isGet = value; }
        public bool GetIsApplied => isApplied;

        public void AddAmount(int delta)
        {
            if (delta <= 0) 
                return;
            
            pillCount += delta;
            if (pillCount > 0) 
                isApplied = false;
        }

        public bool ConsumeOne()
        {
            if (pillCount <= 0) 
                return false;
            
            onApplied.OnNext(Unit.Default); // 毎回発火
            pillCount--;
            if (pillCount <= 0)
            {
                isApplied = true;
                onApplied.OnCompleted();
                return true;
            }
            
            return false;
        }

        public void ApplyItem()
        {
            if (!isGet || pillCount <= 0)
                return;
            
            ConsumeOne();
        }
    }
}
