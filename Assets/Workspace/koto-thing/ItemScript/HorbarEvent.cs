using UniRx;

namespace Workspace.koto_thing
{
    public struct HotbarAssignEvent
    {
        public int SlotIndex;
        public IItem Item;

        public HotbarAssignEvent(int index, IItem item)
        {
            SlotIndex = index;
            Item = item;
        }
    }

    public struct HotbarUseEvent
    {
        public int SlotIndex;
        public IItem Item;
        public bool Consumed;

        public HotbarUseEvent(int index, IItem item, bool consumed)
        {
            SlotIndex = index;
            Item = item;
            Consumed = consumed;
        }
    }

    public class HotbarReactiveBus
    {
        public readonly Subject<HotbarAssignEvent> OnAssigned = new ();
        public readonly Subject<HotbarUseEvent> OnUsed = new ();
    }
}