using Unity.Cinemachine;
using UnityEngine;

namespace Workspace.koto_thing
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