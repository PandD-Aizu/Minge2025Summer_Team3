using System;
using UnityEngine;
using UniRx;

namespace Workspace.koto_thing
{
    public class AmmoModel : MonoBehaviour, IItem
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
        private bool getIsApplied;
        public IObservable<Unit> OnApplied => onApplied;
        
        /* プロパティ */
        public int GetAmount => ammoCount;
        public string GetDisplayName => displayName;
        public string GetDescription => description;
        public AmmoType GetAmmoType => ammoType;
        public Sprite GetSprite { get => ammoSprite; }
        public bool SetIsGet { get => isGet; set => isGet = value; }
        public bool GetIsApplied => getIsApplied;

        /// <summary>
        /// アイテムを適用する
        /// </summary>
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