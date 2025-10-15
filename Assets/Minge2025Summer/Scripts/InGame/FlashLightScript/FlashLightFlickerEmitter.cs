using FMODUnity;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.FlashLightScript
{
    public class FlashLightFlickerEmitter : MonoBehaviour
    {
        [SerializeField] private StudioEventEmitter flickerEmitter;
        [SerializeField] private StudioEventEmitter switchEmitter;

        public void PlayFlickerSound()
        {
            if (flickerEmitter != null && !flickerEmitter.IsPlaying())
                flickerEmitter.Play();
        }
        
        public void StopFlickerSound()
        {
            if (flickerEmitter != null && flickerEmitter.IsPlaying())
                flickerEmitter.Stop();
        }
        
        public void PlaySwitchSound()
        {
            if (switchEmitter != null)
                switchEmitter.Play();
        }
    }
}