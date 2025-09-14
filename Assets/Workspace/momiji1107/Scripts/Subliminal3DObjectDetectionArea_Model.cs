using UnityEngine;

public class Subliminal3DObjectDetectionArea_Model : MonoBehaviour
{
    [SerializeField] private GameObject this3DObject; //対応するオブジェクト
    public float DisplayTime = 3.0f;

    public void HideThisObject()
    {
        Debug.Log("HideObject");
        this3DObject.gameObject.SetActive(false);
    }

    public void AppearThisObject()
    {
        Debug.Log("appearObject");
        this3DObject.gameObject.SetActive(true);
    }

    public void DestroyThisObject()
    {
        Debug.Log("destroyObject");
        Destroy(this3DObject.gameObject);
    }
}
