using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerGunView : MonoBehaviour
    {
        [Header("銃の描画位置")]
        [SerializeField, Tooltip("銃の親オブジェクト")] private Transform gunTransform;
        [SerializeField, Tooltip("銃の描画位置")] private Transform gunViewPoint;

        public void ShowGun(IGun gun)
        {
            if (gun is MonoBehaviour gunMono)
            {
                gunMono.transform.SetParent(gunTransform);
                gunMono.transform.localPosition = gunViewPoint.localPosition;
                gunMono.transform.localRotation = gunViewPoint.localRotation;
            }
        }
    }
}