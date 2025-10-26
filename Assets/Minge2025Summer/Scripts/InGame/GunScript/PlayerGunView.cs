using Minge2025Summer.Scripts.InGame.GunScript.Interface;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.GunScript
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