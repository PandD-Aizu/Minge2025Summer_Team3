using UnityEngine;

public class PoliceDetectionArea : MonoBehaviour
{
    [SerializeField] GameObject Police;
    Police_Presenter presenterScript;
    Police_Model modelScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        presenterScript = Police.GetComponent<Police_Presenter>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //プレイヤーが範囲内に入ったことを通知する
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) presenterScript.BattleStart();
    }

    //プレイヤーが範囲外に出たことを通知する
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) presenterScript.BattleEnd();
    }
}
