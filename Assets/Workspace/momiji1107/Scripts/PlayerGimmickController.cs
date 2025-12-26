using UnityEngine;

public class PlayerGimmickController : MonoBehaviour
{
    [Header("インタラクト可能な最大距離")]
    [SerializeField] private float maxDistance;
    
    private GimmickCameraManager camManager; //接触したギミックについているGimmickCameraManagerコンポーネント
    private bool onGimmick;　//ギミック中かどうか
    public bool OnGimmick { get { return onGimmick; } set { onGimmick = value; } }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnGimmick = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Eキーでギミック開始
        if (Input.GetKeyDown(KeyCode.E) && !OnGimmick)
        {
            ContactGimmick();
        }

        //（テスト用、後で消す）Tキーでギミック終了
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
            camManager = hit.collider.gameObject.GetComponentInChildren<GimmickCameraManager>();
            if(camManager == null) Debug.Log("can't find gimmick camera manager");
            camManager.ChangeCameraToGimmick();
        }
    }

    //ギミック中の画面から戻るボタンを押した時に呼び出される
    public void ExitGimmick()
    {
        Debug.Log("ExitGimmick");
        camManager.ChangeCameraToMain();
    }
}
