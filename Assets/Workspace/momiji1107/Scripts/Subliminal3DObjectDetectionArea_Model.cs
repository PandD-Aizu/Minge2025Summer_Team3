using UnityEngine;

public class Subliminal3DObjectDetectionArea_Model : MonoBehaviour
{
    [SerializeField] private GameObject this3DObject; //対応するオブジェクト
    public float DisplayTime = 3.0f;

    public void HideThisObject()
    {
        if (this3DObject != null)
            this3DObject.gameObject.SetActive(false);
    }

    public void AppearThisObject()
    {
        if (this3DObject != null)
            this3DObject.gameObject.SetActive(true);
    }

    public void DestroyThisObject()
    {
        if (this3DObject != null)
            Destroy(this3DObject.gameObject);
    }
}
