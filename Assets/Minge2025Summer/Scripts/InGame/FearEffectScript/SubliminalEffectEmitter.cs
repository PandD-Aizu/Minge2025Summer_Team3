using FMODUnity;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.FearEffectScript
{
    public class SubliminalEffectEmitter : MonoBehaviour
    {
        [SerializeField] private StudioEventEmitter whiteNoiseEmitter;

        public void PlayWhiteNoise() => whiteNoiseEmitter.Play();
        public void StopWhiteNoise() => whiteNoiseEmitter.Stop();
    }
}