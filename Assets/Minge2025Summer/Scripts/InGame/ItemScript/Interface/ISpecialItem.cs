using System.Collections.Generic;

namespace Minge2025Summer.Scripts.InGame.ItemScript.Interface
{
    public interface ISpecialItem : IItem
    {
        public string SpecialID { get; }
        public bool IsUnique { get; }
        public bool CanStack { get; }
        public bool IsConsumable { get; }
        public bool CanUse(SpecialItemContext context, out string failReason);
    }

    public struct SpecialItemContext
    {
        public string SceneName;
        public string StoryPhase;
        public IReadOnlyCollection<string> Flags;
    }
}