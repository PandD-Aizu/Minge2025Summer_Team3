using System;
using UnityEngine.Rendering;

namespace Minge2025Summer.Scripts.RenderingScript.HamBarEffect
{
    [Serializable, VolumeComponentMenu("HamBarEffect")]
    public class HamBarEffectVolume : VolumeComponent, IPostProcessComponent
    {
        public BoolParameter isActive = new BoolParameter(false);
        
        public ClampedFloatParameter amplitude = new ClampedFloatParameter(0.02f, 0f, 0.1f);
        public ClampedFloatParameter frequency = new ClampedFloatParameter(20.0f, 1.0f, 100.0f);
        public ClampedFloatParameter speed = new ClampedFloatParameter(2.0f, -10.0f, 10.0f);

        public bool IsActive()
        {
            return isActive.value;
        }

        public bool IsTileCompatible() => false;
    }
}