using Unity.Cinemachine;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.PlayerCameraControllerScript
{
    public class PlayerCameraNoiseModel : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera virtualCamera;
        [SerializeField] private NoiseSettings idleNoise;
        [SerializeField] private NoiseSettings walkingNoise;
        [SerializeField] private NoiseSettings runningNoise;
        
        private CinemachineBasicMultiChannelPerlin noisePerlin;
    }
}