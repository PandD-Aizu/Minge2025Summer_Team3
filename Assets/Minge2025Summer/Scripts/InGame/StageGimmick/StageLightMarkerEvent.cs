using FMODUnity;
using Minge2025Summer.InGame.Interactable;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.StageGimmick
{
    public class StageLightMarkerEvent : MonoBehaviour, IInteractable
    {
        [Header("Light Settings")]
        [SerializeField] private Light markerLight;
        
        [Header("Sound Settings")]
        [SerializeField] private StudioEventEmitter lightMarkerEmitter;
        [SerializeField] private float soundRadius = 3.0f;

        [Header("Interaction Settings")]
        [SerializeField] private string interactionPrompt = "ライトを切り替える";

        private bool isLightOn = false;
        
        public string InteractionPrompt => interactionPrompt;

        /// <summary>
        /// ライトのオンオフを切り替える
        /// </summary>
        public void Interact(GameObject instigator = null)
        {
            isLightOn = !isLightOn;

            if (markerLight != null)
            {
                markerLight.enabled = isLightOn;
            }
            
            if (lightMarkerEmitter != null)
            {
                lightMarkerEmitter.Play();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, soundRadius);
        }
    }
}
