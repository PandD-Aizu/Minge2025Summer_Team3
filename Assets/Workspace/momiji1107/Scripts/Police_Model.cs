using UnityEngine;

public class Police_Model : MonoBehaviour
{
    [SerializeField] public float PoliceMoveSpeed = 0.01f; //警官の移動速度
    [SerializeField] public GameObject player;
    public bool Battleflag = false; //プレイヤーが範囲内にいるかどうか
    public float PoliceHP = 50f;

    public Ray ray; //警官からプレイヤーまでの距離を測るray
    public RaycastHit hit; //rayが衝突したオブジェクト
    [SerializeField] public float battleDistance = 3.0f; //警官が攻撃をするプレイヤーまでの距離
   
    //範囲内に入った時
    public void OnBattleflag()
    {
        Debug.Log("police in");
        Battleflag = true;
    }

    //範囲外に出た時
    public void OffBattleflag()
    {
        Debug.Log("police exit");
        Battleflag = false;
    }

    //ダメージを受けた時
    public void Damage(float damage)
    {
        PoliceHP -= damage;
    }
}
