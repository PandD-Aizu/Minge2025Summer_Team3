using System;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript
{
    public class ConsumableItem : MonoBehaviour, IConsumableItem
    {
        [SerializeField] private string itemName;
        [SerializeField] private string displayName;
        [SerializeField] private int amount;
        [SerializeField] private Sprite icon;

        public string GetItemName => itemName;
        public string GetDisplayName => displayName;
        public int GetItemAmount => amount;
        public Sprite GetIcon => icon;

        private Subject<Unit> onGetItem = new ();
        public IObservable<Unit> OnGetItem => onGetItem;
        private Subject<Unit> onApplyItem = new ();
        public IObservable<Unit> OnApplyItem => onApplyItem;
        
        public bool ApplyItem()
        {
            return true;
        }

        private void HideItem()
        {
            gameObject.SetActive(false);
        }
    }
}