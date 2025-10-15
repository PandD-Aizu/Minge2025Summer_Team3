using FMODUnity;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.InteractableObjects.Door
{
    public class DoorEmitter : MonoBehaviour
    {
        [SerializeField, Tooltip("ドアの開閉音")]
        private StudioEventEmitter doorSoundEmitter;
        
        public void PlayDoorSound() => doorSoundEmitter.Play();
    }
}