using UnityEngine;

public class PlayerGimmickController : MonoBehaviour
{
    [Header("インタラクト可能な最大距離")]
    [SerializeField] private float maxDistance;

    [Space(20)]
    [SerializeField] private GimmickCameraManager camManager;
    private bool onGimmick;
    public bool OnGimmick
    {
        get { return onGimmick; }
        set { onGimmick = value; }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnGimmick = false;
        //camManager = GetComponent<GimmickCameraManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !OnGimmick)
        {
            ContactGimmick();
        }

        if (Input.GetKeyDown(KeyCode.T) && OnGimmick)
        {
            camManager.ChangeCameraToMain();
        }
    }

    private void ContactGimmick()
    {
        var cam = Camera.main;
        if (cam == null) return; 
        Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit, maxDistance);

        //光線を飛ばしてギミックに当たっていたら、カメラを切り替える
        if (hit.collider != null && hit.collider.name == "Gimmick")
        {
            //camManager = hit.collider.gameObject.GetComponent<GimmickCameraManager>();
            //if(camManager == null) Debug.Log("can't find gimmick camera manager");
            camManager.ChangeCameraToGimmick();
        }
    }

    //ギミック中の画面から戻るボタンを押した時に呼び出される
    public void ExitGimmick()
    {
        camManager.ChangeCameraToMain();
    }
}
