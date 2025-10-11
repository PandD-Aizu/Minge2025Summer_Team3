using FMODUnity;
using UnityEngine;

namespace Workspace.koto_thing
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