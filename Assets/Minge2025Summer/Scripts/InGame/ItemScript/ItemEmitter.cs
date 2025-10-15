using FMODUnity;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ItemScript
{
    public class ItemEmitter : MonoBehaviour
    {
        [SerializeField] private StudioEventEmitter pickUpEmitter;

        public void PlayPickUp() => pickUpEmitter.Play();
    }
}