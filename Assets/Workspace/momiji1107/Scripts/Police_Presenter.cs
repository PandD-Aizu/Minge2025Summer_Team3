using UnityEngine;
using Workspace.momiji1107;

public class Police_Presenter : MonoBehaviour
{
    [SerializeField] GameObject PoliceDetectionArea;
    public GameObject player;
    
    private Police_Model modelScript;
    private Police_View viewScript;
    private PoliceEmitter emitterScript;
    
    void Start()
    {
        modelScript = this.GetComponent<Police_Model>();
        viewScript = this.GetComponent<Police_View>();
        emitterScript = this.GetComponent<PoliceEmitter>();
    }
    
    void Update()
    {
        emitterScript.PlayFootStep(modelScript.GetCharacterController.velocity.magnitude);
        modelScript.Move();
        modelScript.AttackPlayer();
        emitterScript.UpdateMoanTimer();
    }

    //プレイヤーが範囲内に入った時
    public void BattleStart()
    {
        Debug.Log("in");
        modelScript.OnBattleflag();
        emitterScript.StartMoaning();
    }

    //プレイヤーが範囲外に出た時
    public void BattleEnd()
    {
        Debug.Log("exit");
        modelScript.OffBattleflag();
        emitterScript.StopMoaning();
    }

}
