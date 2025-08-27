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

        [Header("フラグ")] 
        [SerializeField] private bool isGet;
        [SerializeField] private bool isApplied;

        private readonly Subject<Unit> onApplied = new ();
        public IObservable<Unit> OnApplied => onApplied;
        
        /* プロパティ */
        public int GetAmmoCount => ammoCount;
        public AmmoType GetAmmoType => ammoType;
        public bool IsGet { get => isGet; set => isGet = value; }
        public bool IsApplied { get => isApplied; set => isApplied = value; }

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
        
        /// <summary>
        /// アイテムを取得したかどうかのフラグを設定する
        /// </summary>
        /// <param name="value">取得したかどうか</param>
        public void SetIsGet(bool value)
        {
            isGet = value;
        }
        
        /// <summary>
        /// アイテムが適用されたかどうかを取得する
        /// </summary>
        /// <returns>アイテムが適用されたかどうか</returns>
        public bool GetIsApplied()
        {
            return isApplied;
        }
    }
}