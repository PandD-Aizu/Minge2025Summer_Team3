using UnityEngine;

namespace Workspace.koto_thing
{
    public class LightRotationModel : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform lightTransform;
        
        /// <summary>
        /// 懐中電灯の向きを設定する
        /// </summary>
        public void AlignLightWithCamera()
        {
            float cameraXAngle = mainCamera.transform.eulerAngles.x;
            float cameraYAngle = mainCamera.transform.eulerAngles.y;
            float cameraZAngle = mainCamera.transform.eulerAngles.z;
            
            lightTransform.rotation = Quaternion.Euler(cameraXAngle, cameraYAngle, cameraZAngle);
        }
    }
}