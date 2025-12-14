using UnityEngine;
using FMODUnity;
using Minge2025Summer.Scripts.InGame.PlayerTransformScript;
using Minge2025Summer.Scripts.InGame.AcousticsScript;
using UniRx;

namespace Minge2025Summer.Scripts.InGame.StageGimmick
{
    public class Puddle : MonoBehaviour
    {
        [SerializeField] private PlayerPositionModel model;
        [SerializeField] private StudioEventEmitter puddleEmitter;
        [SerializeField, Tooltip("音が広がる距離")] private float soundRadius = 5.0f;

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            if (model.IsWalking)
            {
                model.IsPuddling = true;
                
                if (!puddleEmitter.IsPlaying())
                {
                    puddleEmitter.Play();
                }
                
                SoundEvent soundEvent = new SoundEvent(transform.position, soundRadius, SoundType.Puddle, gameObject);
                MessageBroker.Default.Publish(soundEvent);
            }
            else
            {
                puddleEmitter.Stop();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;
            
            puddleEmitter.Stop();
            
            model.IsPuddling = false;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, soundRadius);
        }
    }
}
