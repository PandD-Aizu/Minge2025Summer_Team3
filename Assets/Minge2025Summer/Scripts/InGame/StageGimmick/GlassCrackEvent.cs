using FMODUnity;
using Minge2025Summer.Scripts.InGame.AcousticsScript;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.StageGimmick
{
    public class GlassCrackEvent : MonoBehaviour
    {
        [SerializeField] private StudioEventEmitter glassBreakEmitter;
        [SerializeField, Tooltip("音が広がる距離")] private float soundRadius = 5.0f;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            Debug.Log("GlassCrackEvent: Player entered trigger, playing glass break sound.");
            glassBreakEmitter.Play();
            SoundEvent soundEvent = new SoundEvent(transform.position, soundRadius, SoundType.Glass, gameObject);
            MessageBroker.Default.Publish(soundEvent);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, soundRadius);
        }
    }
}