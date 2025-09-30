using System;
using UnityEngine;
using UniRx;

namespace Workspace.koto_thing
{
    public class AmmoModel : MonoBehaviour, IItem, IAppliable
    {
        [Header("弾薬の設定")]
        [SerializeField] private int ammoCount;
        [SerializeField] private AmmoType ammoType;

        [Header("インベントリUIの設定")] 
        [SerializeField] private string displayName;
        [TextArea, SerializeField] private string description;

        [Header("フラグ")] 
        [SerializeField] private bool isGet;
        [SerializeField] private bool isApplied;

        [Header("弾薬2D画像")] 
        [SerializeField] private Sprite ammoSprite;

        private readonly Subject<Unit> onApplied = new ();
        public IObservable<Unit> OnApplied => onApplied;
        
        /* プロパティ */
        public int GetAmount => ammoCount;
        public string GetDisplayName => displayName;
        public string GetDescription => description;
        public AmmoType GetAmmoType => ammoType;
        public Sprite GetSprite { get => ammoSprite; }
        public bool SetIsGet { get => isGet; set => isGet = value; }
        public bool GetIsApplied => isApplied;

        /// <summary>
        /// アイテムを適用する
        /// </summary>
        public void ApplyItem()
        {
            if (!isGet || ammoCount <= 0)
                return;
            
            ConsumeOne();
        }

        public void AddAmount(int delta)
        {
            if (delta <= 0) 
                return;
            
            ammoCount += delta;
            if (ammoCount > 0) 
                isApplied = false;
        }

        public bool ConsumeOne()
        {
            if (ammoCount <= 0) 
                return false;
            
            onApplied.OnNext(Unit.Default); // 毎回発火
            ammoCount--;
            if (ammoCount <= 0)
            {
                isApplied = true;
                onApplied.OnCompleted();
                return true;
            }
            
            return false;
        }
    }
}