using System;
using Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Enum;
using Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Interface;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Item
{
    public class Ammo : MonoBehaviour, IAmmoItem
    {
        [SerializeField] private string itemName;
        [SerializeField] private string displayName;
        [SerializeField, TextArea] private string itemDescription;
        [SerializeField] private int amount;
        [SerializeField] private Sprite icon;
        [SerializeField] private AmmoType ammoType;
        
        private Subject<Unit> onGetItem = new ();
        public IObservable<Unit> OnGetItem => onGetItem;
        private Subject<Unit> onApplyItem = new ();
        public IObservable<Unit> OnApplyItem => onApplyItem;

        public string GetItemName => itemName;
        public string GetDisplayName => displayName;
        public string GetItemDescription => itemDescription;
        public int GetItemAmount => amount;
        public Sprite GetIcon => icon;
        public AmmoType GetAmmoType => ammoType;

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