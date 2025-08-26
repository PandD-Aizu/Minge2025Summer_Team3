using System.Collections.Generic;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class GunPresenter : MonoBehaviour
    {
        [Header("依存関係")] 
        [SerializeField] private GunModel model;
        [SerializeField] private GunView view;

        private void Start()
        {
            
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.R) && model.GetCurrentMagCapacity() != model.GetCurrentAmmoInMag())
            {
                model.Reload();
            }

            if (Input.GetMouseButton(1))
            {
                if (Input.GetMouseButtonDown(0) && model.GetCurrentAmmoInMag() > 0)
                {
                    model.GetCurrentEquippedGun.Fire();
                }
            }
            
            view.UpdateAmmoText(model.GetCurrentAmmoInMag(), model.GetCurrentAmmo(), model.GetCurrentMagCapacity());
        }
    }
}