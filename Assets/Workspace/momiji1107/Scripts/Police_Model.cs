using UnityEngine;

public class Police_Model : MonoBehaviour
{
    [SerializeField] float PoliceMoveSpeed = 3.0f; //警官の移動速度
    [SerializeField] GameObject player;
    //Vector3 policePos;
    //Vector3 playerPos;
    //Vector3 target;
    //private float distance = 10f;

    public bool OnBattleflag = false; //プレイヤーが範囲内にいるかどうか
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Invoke(nameof(GetPosition), 3);
        //target = playerPos - (playerPos - policePos).normalized;
        if(OnBattleflag == true)
        {
            //プレイヤーに向かって進み続ける
            //transform.position = Vector3.MoveTowards(policePos, target, PoliceMoveSpeed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, PoliceMoveSpeed * Time.deltaTime);
        }
    }

    //フラグの切り替え
    public void switchBattleflag()
    {
        if (OnBattleflag == false) OnBattleflag = true;
        else OnBattleflag = false;
    }

    /*public void GetPosition()
    {
        policePos = transform.position;
        playerPos = player.transform.position;
    }*/
}
