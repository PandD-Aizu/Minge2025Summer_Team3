using UnityEngine;

namespace Acoustics
{
    public class ReverbMaterial : MonoBehaviour
    {
        [SerializeField] private ReverbMaterialType reverbMaterialType;
        
        public ReverbMaterialType GetReverbMaterialType => reverbMaterialType;
    }
}