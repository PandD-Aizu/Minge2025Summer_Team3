using Minge2025Summer.Scripts.InGame.ItemScript.Interface;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ItemScript.FriendPhone
{
    public class FriendsPhoneModel : MonoBehaviour, ISpecialItem
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private string displayName = "友達のスマホ";
        [SerializeField, TextArea] private string description;

        private bool isGet;
        private int amount = 1;
        private bool applied;

        public string SpecialID => "FriendsPhone";
        public bool IsUnique => true;
        public bool CanStack => false;
        public bool IsConsumable => false;

        public int GetAmount => amount;
        public string GetDisplayName => displayName;
        public string GetDescription => description;
        public bool SetIsGet { get => isGet; set => isGet = value; }
        public bool GetIsApplied => applied;
        public Sprite GetSprite => icon;

        public bool CanUse(SpecialItemContext context, out string failReason)
        {
            if (context.SceneName == "FinalScene")
            {
                failReason = null;
                return true;
            }

            failReason = "ここでは使えない";
            return false;
        }

        public void ApplyItem()
        {
            applied = true;
        }

        public void AddAmount(int delta)
        {
            amount += delta;
            if (amount < 1)
                amount = 1;
        }

        public bool ConsumeOne()
        {
            return false;
        }
    }
}