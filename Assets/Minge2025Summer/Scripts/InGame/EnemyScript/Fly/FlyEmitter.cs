using FMODUnity;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EnemyScript.Fly
{
    public class FlyEmitter : MonoBehaviour
    {
        [Header("依存関係")] 
        [SerializeField] private StudioEventEmitter flyFlyEmitter;
        
        public void PlayFlyFlySound()
        {
            if (flyFlyEmitter == null) 
                return;
            
            flyFlyEmitter.Stop();
            flyFlyEmitter.Play();
        }
    }
}