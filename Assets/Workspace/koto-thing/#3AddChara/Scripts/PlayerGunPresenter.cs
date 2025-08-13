using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerGunPresenter : MonoBehaviour
    {
        [SerializeField] private PlayerGunModel model;

        private void Start()
        {
            
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                model.ShootGun();
            }
        }
    }
}