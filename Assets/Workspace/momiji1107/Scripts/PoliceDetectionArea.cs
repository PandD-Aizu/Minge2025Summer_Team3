using UnityEngine;
using Workspace.koto_thing;
using Workspace.momiji1107;

public class PoliceDetectionArea : MonoBehaviour
{
    [SerializeField] GameObject Police;
    Police_Model modelScript;
    PoliceEmitter emitterScript;
    
    void Start()
    {
        modelScript = Police.GetComponent<Police_Model>();
        emitterScript = Police.GetComponent<PoliceEmitter>();
    }

    //プレイヤーが範囲内に入ったことを通知する
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            modelScript.OnBattleflag();
            emitterScript.PlayCloseEnemySound();
        }
    }

    //プレイヤーが範囲外に出たことを通知する
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) modelScript.OffBattleflag();
    }
}
