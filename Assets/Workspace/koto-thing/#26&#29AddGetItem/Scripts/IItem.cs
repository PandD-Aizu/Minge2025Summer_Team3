using UnityEngine;

namespace Workspace.koto_thing
{
    public interface IItem
    {
        public int GetAmount { get; }
        public string GetDisplayName { get; }
        public string GetDescription { get; }
        public bool SetIsGet { set; }
        public bool GetIsApplied { get; }
        public Sprite GetSprite { get; }
        
        public void ApplyItem();
    }
}
