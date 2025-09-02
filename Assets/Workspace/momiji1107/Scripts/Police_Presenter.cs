using UnityEngine;

public class Police_Presenter : MonoBehaviour
{
    Police_Model modelScript;
    Police_View viewScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        modelScript = this.GetComponent<Police_Model>();
        viewScript = this.GetComponent<Police_View>();
        modelScript.ray = new Ray(transform.position, transform.forward);　//警官から前方にrayを飛ばす
    }

    // Update is called once per frame
    void Update()
    {
        //警官がプレイヤーに一定距離まで近づいたら攻撃する
        if(Physics.Raycast(modelScript.ray, out modelScript.hit, modelScript.battleDistance))
        {
            if (modelScript.hit.collider.CompareTag("Player"))
            {
                Debug.Log("Attack!!");
            }
        }

        if (modelScript.Battleflag == true)
        {
            //プレイヤーに向かって進み続ける
            transform.LookAt(modelScript.player.transform);
            transform.position += transform.forward * modelScript.PoliceMoveSpeed * Time.deltaTime;
        }

        //警官がやられた時の処理
        if (modelScript.PoliceHP <= 0)
        {
            Debug.Log("PoliceDead");
            Destroy(this.gameObject);
        }
    }

}
