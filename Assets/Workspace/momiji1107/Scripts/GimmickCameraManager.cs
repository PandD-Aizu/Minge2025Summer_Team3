using Unity.Cinemachine;
using UnityEngine;

public class GimmickCameraManager : MonoBehaviour
{
    [SerializeField] private GameObject fpsCamera;
    [SerializeField] private GameObject thisGimmickCamera; //このギミック用のカメラ
    [SerializeField] private PlayerGimmickController gimmickController;
    [SerializeField, Tooltip("ギミックからのカメラの距離")] private float cameraDistance;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thisGimmickCamera.SetActive(false);
        thisGimmickCamera.GetComponent<CinemachinePositionComposer>().CameraDistance = cameraDistance;
    }

    //gimmickCameraに切り替える
    public void ChangeCameraToGimmick()
    {
        gimmickController.OnGimmick = true;
        thisGimmickCamera.SetActive(true);
    }
    
    //fpsCameraに切り替える
    public void ChangeCameraToMain()
    {
        gimmickController.OnGimmick = false;
        thisGimmickCamera.SetActive(false);
    }
}
