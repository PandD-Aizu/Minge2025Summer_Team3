using UnityEngine;

public class Subliminal3DObjectDetectionArea_Model : MonoBehaviour
{
    [SerializeField] private GameObject this3DObject; //対応するオブジェクト
    public float DisplayTime = 3.0f; //表示する時間

    //非表示にする
    public void HideThisObject()
    {
        Debug.Log("HideObject");
        this3DObject.gameObject.SetActive(false);
    }

    //表示する
    public void AppearThisObject()
    {
        Debug.Log("appearObject");
        this3DObject.gameObject.SetActive(true);
    }

    //Deleteする
    public void DestroyThisObject()
    {
        Debug.Log("destroyObject");
        Destroy(this3DObject.gameObject);
    }
}
