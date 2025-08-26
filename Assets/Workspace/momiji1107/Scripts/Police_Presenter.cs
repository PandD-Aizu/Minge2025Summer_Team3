using UnityEngine;

public class Police_Presenter : MonoBehaviour
{
    [SerializeField] GameObject PoliceDetectionArea;
    public GameObject player;
    Police_Model modelScript;
    Police_View viewScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        modelScript = this.GetComponent<Police_Model>();
        viewScript = this.GetComponent<Police_View>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //プレイヤーが範囲内に入っている時
    public void BattleDetect()
    {
        modelScript.switchBattleflag();
    }

    //プレイヤーが範囲外に出た時
    public void BattleEnd()
    {
        Debug.Log("exit");
        modelScript.switchBattleflag();
    }
}
