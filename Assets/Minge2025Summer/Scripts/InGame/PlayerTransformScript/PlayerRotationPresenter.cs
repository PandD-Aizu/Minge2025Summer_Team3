using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.PlayerTransformScript
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