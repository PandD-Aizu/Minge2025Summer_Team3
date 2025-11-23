using FMODUnity;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.SpecialEventTriggerScript
{
    public class FMODOneTimeEventTrigger : MonoBehaviour
    {
        [SerializeField] private StudioEventEmitter eventEmitter;
        
        private bool hasTriggered = false;
        
        private void Start()
        {
            if (eventEmitter == null)
            {
                Debug.LogError("FMODOneTimeEventTrigger: EventEmitter is not assigned.");
                return;
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || hasTriggered)
                return;
            
            // イベントを再生
            eventEmitter.Play();
            hasTriggered = true;
        }
    }
}