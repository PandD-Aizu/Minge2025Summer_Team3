using UnityEngine;

public class Police_Presenter : MonoBehaviour
{
    [SerializeField] GameObject PoliceDetectionArea;
    public GameObject player;
    Police_Model modelScript;
    Police_View viewScript;

    Ray ray;                                      //警官からプレイヤーまでの距離を測るray
    RaycastHit hit;                               //rayが衝突したオブジェクト
    [SerializeField] float battleDistance = 3.0f; //警官が攻撃をするプレイヤーまでの距離
    
    void Start()
    {
        modelScript = this.GetComponent<Police_Model>();
        viewScript = this.GetComponent<Police_View>();
        ray = new Ray(transform.position, transform.forward);　//警官から前方にrayを飛ばす
    }
    
    void Update()
    {
        modelScript.AttackPlayer();
    }

    //プレイヤーが範囲内に入った時
    public void BattleStart()
    {
        Debug.Log("in");
        modelScript.OnBattleflag();
    }

    //プレイヤーが範囲外に出た時
    public void BattleEnd()
    {
        Debug.Log("exit");
        modelScript.OffBattleflag();
    }
}
