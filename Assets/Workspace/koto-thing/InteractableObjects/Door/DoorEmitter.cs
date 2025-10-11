using FMODUnity;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class DoorEmitter : MonoBehaviour
    {
        [SerializeField, Tooltip("ドアの開閉音")]
        private StudioEventEmitter doorSoundEmitter;
        
        public void PlayDoorSound() => doorSoundEmitter.Play();
    }
}