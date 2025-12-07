using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.AcousticsScript
{
    public enum SoundType
    {
        Gunshot,
        Glass,
        Footstep,
        Impact,
        Puddle,
        Other,
    }

    public struct SoundEvent
    {
        public Vector3 Position { get; }
        public float Radius { get; }
        public SoundType Type { get; }
        public GameObject Source { get; }

        public SoundEvent(Vector3 position, float radius, SoundType type, GameObject source)
        {
            Position = position;
            Radius = radius;
            Type = type;
            Source = source;
        }
    }
}
