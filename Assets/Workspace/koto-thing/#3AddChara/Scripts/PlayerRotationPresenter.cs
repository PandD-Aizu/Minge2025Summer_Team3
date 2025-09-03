using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerRotationPresenter : MonoBehaviour
    {
        [SerializeField] private PlayerRotationModel model;
        [SerializeField] private PlayerRotationView view;

        public void Update()
        {
            model.AlignYRotationWithCamera();
        }
    }
}