using FMODUnity;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.SpecialEventTriggerScript
{
    public class FMODEventReferenceChangeTrigger : MonoBehaviour
    {
        [SerializeField] private string eventName;
        [SerializeField] private StudioEventEmitter eventEmitter;

        private bool hasTriggered = false;

        private void Start()
        {
            if (eventEmitter == null)
            {
                Debug.LogError("FMODEventReferenceChangeTrigger: EventEmitter is not assigned.");
                return;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || hasTriggered)
                return;
            
            // イベントを変更
            eventEmitter.EventReference = FMODUnity.RuntimeManager.PathToEventReference(eventName);
            hasTriggered = true;
        }
    } 
}