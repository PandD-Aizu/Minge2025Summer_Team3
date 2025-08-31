using UnityEngine;
using Workspace.koto_thing;

public class Police_Model : MonoBehaviour
{
    [SerializeField] float PoliceMoveSpeed = 0.01f; //警官の移動速度
    [SerializeField] GameObject player;
    
    [Header("警官のステータス")]
    [SerializeField] private float policeAttack = 10.0f;
    [SerializeField] private float attackCoolDown = 3.0f;
    [SerializeField] private float battleDistance = 3.0f;

    private float currentCoolDown = 0.0f;
    
    public bool Battleflag = false; //プレイヤーが範囲内にいるかどうか
    
    void Start()
    {
        
    }
    
    void Update()
    {
        if(Battleflag == true)
        {
            //プレイヤーに向かって進み続ける
            transform.LookAt(player.transform);
            transform.position += transform.forward * PoliceMoveSpeed * Time.deltaTime;
        }
    }

    /// <summary>
    /// プレイヤーを攻撃する
    /// </summary>
    public void AttackPlayer()
    {
        currentCoolDown += Time.deltaTime;
    
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, battleDistance) &&
            hit.collider.CompareTag("Player") && 
            currentCoolDown >= attackCoolDown)
        {
            hit.collider.GetComponentInChildren<PlayerHpModel>().CurrentHp -= policeAttack;
            currentCoolDown = 0.0f;
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
}
