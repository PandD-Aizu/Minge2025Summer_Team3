using UnityEngine;

namespace Workspace.koto_thing
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