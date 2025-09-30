using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerRotationModel : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform playerTransform;

        /// <summary>
        /// メインカメラの向きに合わせてプレイヤーのY軸回転を調整する。
        /// </summary>
        public void AlignYRotationWithCamera()
        {
            float cameraYAngle = mainCamera.transform.eulerAngles.y;

            playerTransform.rotation = Quaternion.Euler(0.0f, cameraYAngle, 0.0f);
        }
    }
}