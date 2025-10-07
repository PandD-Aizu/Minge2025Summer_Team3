using System;
using UnityEngine;
using UniRx;

namespace Workspace.koto_thing
{
    public class AmmoModel : MonoBehaviour, IItem, IAppliable, IStackable
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
        public bool CanStack => true; // 弾薬は従来通りスタック可能

        /// <summary>
        /// 取得時に即座に適用済み扱いにしたい場合に呼ぶ（OnAppliedは発火しない）
        /// </summary>
        public void MarkAppliedOnPickup()
        {
            isApplied = true;
        }

        /// <summary>
        /// アイテムを適用する
        /// </summary>
        public void ApplyItem()
        {
            if (!isGet) return;
            if (isApplied) return;
            if (ammoCount <= 0) return;
            
            onApplied.OnNext(Unit.Default);
            onApplied.OnCompleted();
            isApplied = true;
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

            ammoCount--;
            return true;
        }

        /// <summary>
        /// 弾数を外部から絶対値で設定する
        /// </summary>
        /// <param name="value">新しい弾数(0未満なら0)</param>
        public void SetAmountAbsolute(int value)
        {
            ammoCount = Mathf.Max(0, value);
            if (ammoCount <= 0)
                isApplied = false; // 0 の場合は再適用不可状態リセット(運用に応じ調整可)
        }
    }
}