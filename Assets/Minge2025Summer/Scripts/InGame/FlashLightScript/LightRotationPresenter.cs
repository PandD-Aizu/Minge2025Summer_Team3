using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.FlashLightScript
{
    public class LightRotationPresenter : MonoBehaviour 
    {
        [SerializeField] private LightRotationModel model;

        private void Start()
        {
            
        }

        private void Update()
        {
            model.AlignLightWithCamera();
        }
    }
}