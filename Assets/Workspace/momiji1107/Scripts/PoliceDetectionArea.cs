using UnityEngine;

public class PoliceDetectionArea : MonoBehaviour
{
    [SerializeField] GameObject Police;
    Police_Model modelScript;
    
    void Start()
    {
        modelScript = Police.GetComponent<Police_Model>();
    }

    //プレイヤーが範囲内に入ったことを通知する
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) modelScript.OnBattleflag();
    }

    //プレイヤーが範囲外に出たことを通知する
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) modelScript.OffBattleflag();
    }
}
