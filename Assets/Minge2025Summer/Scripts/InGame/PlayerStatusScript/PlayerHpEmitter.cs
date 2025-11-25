using FMODUnity;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.PlayerStatusScript
{
    public class PlayerHpEmitter : MonoBehaviour
    {
        [SerializeField] private StudioEventEmitter damageEmitter;
        
        public void PlayDamageSound()
        {
            damageEmitter.Play();
        }
    }
}