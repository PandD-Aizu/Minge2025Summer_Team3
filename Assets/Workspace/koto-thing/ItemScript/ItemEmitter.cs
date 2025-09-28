using FMODUnity;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class ItemEmitter : MonoBehaviour
    {
        [SerializeField] private StudioEventEmitter pickUpEmitter;

        public void PlayPickUp() => pickUpEmitter.Play();
    }
}