using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerGunView : MonoBehaviour
    {
        [Header("銃の描画位置")]
        [SerializeField, Tooltip("銃の親オブジェクト")] private Transform gunTransform;

        public void ShowGun(IGun gun)
        {
            if (gun is MonoBehaviour gunMono)
            {
                gunMono.transform.SetParent(gunTransform);
            }
        }
    }
}