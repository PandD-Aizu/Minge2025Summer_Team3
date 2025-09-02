using UnityEngine;

public class PoliceDetectionArea : MonoBehaviour
{
    [SerializeField] GameObject Police;
    Police_Model modelScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        modelScript = Police.GetComponent<Police_Model>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
