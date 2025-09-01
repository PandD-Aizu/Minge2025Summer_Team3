using FMODUnity;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class SubliminalEffectEmitter : MonoBehaviour
    {
        [SerializeField] private StudioEventEmitter whiteNoiseEmitter;

        public void PlayWhiteNoise() => whiteNoiseEmitter.Play();
        public void StopWhiteNoise() => whiteNoiseEmitter.Stop();
    }
}