using FMODUnity;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript.ItemScript
{
    public class ReiItemInteractionEmitter : MonoBehaviour
    {
        [SerializeField] private StudioEventEmitter itemGetEmitter;
        
        public void PlayItemGetSound() => itemGetEmitter.Play();
    }
}