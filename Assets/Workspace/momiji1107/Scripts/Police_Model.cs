using UnityEngine;

public class Police_Model : MonoBehaviour
{
    [SerializeField] float PoliceMoveSpeed = 0.01f; //警官の移動速度
    [SerializeField] GameObject player;
    public bool Battleflag = false; //プレイヤーが範囲内にいるかどうか
    public float PoliceHP = 50f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Battleflag == true)
        {
            //プレイヤーに向かって進み続ける
            transform.LookAt(player.transform);
            transform.position += transform.forward * PoliceMoveSpeed * Time.deltaTime;
        }

        //警官がやられた時の処理
        if(PoliceHP <= 0)
        {
            Debug.Log("PoliceDead");
            Destroy(this.gameObject);
        }
    }

    //フラグの切り替え
    public void OnBattleflag()
    {
        Battleflag = true;
    }

    public void OffBattleflag()
    {
        Battleflag = false;
    }

    public void Damage(float damage)
    {
        PoliceHP -= damage;
    }
}
