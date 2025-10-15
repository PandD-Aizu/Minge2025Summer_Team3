using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.AcousticsScript
{
    public class ReverbMaterial : MonoBehaviour
    {
        [SerializeField] private ReverbMaterialType reverbMaterialType;
        
        public ReverbMaterialType GetReverbMaterialType => reverbMaterialType;
    }
}