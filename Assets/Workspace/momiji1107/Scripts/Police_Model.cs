using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using Workspace.koto_thing;
using Workspace.momiji1107;

public class Police_Model : MonoBehaviour, IEnemy
{
    [Header("Playerオブジェクト")]
    [SerializeField] private GameObject player; // プレイヤーオブジェクト

    [Header("CharacterController")] 
    [SerializeField] private CharacterController characterController; // CharacterControllerコンポーネント
    
    [Header("警官の移動速度")]
    [SerializeField] float policeMoveSpeed = 3.0f; // 警官の移動速度
    [SerializeField] float acceleration = 10.0f;   // 加速度
    [SerializeField] float deceleration = 15.0f;   // 減速度

    [Header("警官のステータス")] 
    [SerializeField] private float policeHp = 1000.0f;
    [SerializeField] private float policeAttack = 10.0f;  // 警官の攻撃力
    [SerializeField] private float attackCoolDown = 3.0f; // 攻撃のクールダウン時間
    [SerializeField] private float battleDistance = 1.0f; // 戦闘距離

    private float currentCoolDown = 0.0f;         // 現在のクールダウン時間
    private float currentSpeed = 0.0f;            // 現在の速度
    private Vector3 moveDirection = Vector3.zero; // 移動方向

    private bool isMoving = false;
    public ReactiveProperty<bool> Battleflag = new ReactiveProperty<bool>(false); //プレイヤーが範囲内にいるかどうか
    
    /* プロパティ */
    public CharacterController GetCharacterController => characterController;

    /// <summary>
    /// プレイヤーを追跡して移動する
    /// </summary>
    public void Move()
    {
        if (player != null && Battleflag.Value)
        {
            // 方向と速度を決定
            Vector3 direction = (player.transform.position - transform.position).normalized;
            float targetSpeed = policeMoveSpeed;

            // スピードを調整
            if (Vector3.Distance(transform.position, player.transform.position) > 0.1f)
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
            else
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0.0f, deceleration * Time.deltaTime);

            // 移動ベクトルを計算して移動
            currentSpeed = Mathf.Clamp(currentSpeed, 0.0f, policeMoveSpeed);
            moveDirection = direction * currentSpeed;

            // ここで向きと移動を適用
            transform.LookAt(player.transform);
            characterController.Move(moveDirection * Time.deltaTime);
        }
    }

    /// <summary>
    /// プレイヤーを攻撃する
    /// </summary>
    public void AttackPlayer()
    {
        currentCoolDown += Time.deltaTime;
    
        // レイキャストでプレイヤーを検出し、攻撃
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, battleDistance) &&
            hit.collider.CompareTag("Player") && 
            currentCoolDown >= attackCoolDown)
        {
            hit.collider.GetComponentInChildren<PlayerHpModel>().CurrentHp -= policeAttack;
            currentCoolDown = 0.0f;
        }
    }
   
    //範囲内に入った時
    public void OnBattleflag()
    {
        Debug.Log("police in");
        Battleflag.Value = true;
        currentSpeed = 0.0f;
    }

    //範囲外に出た時
    public void OffBattleflag()
    {
        Debug.Log("police exit");
        Battleflag.Value = false;
        currentSpeed = 0.0f;
    }

    //ダメージを受けた時
    public void ReceiveDamage(float damage)
    {
        policeHp -= damage;
        if (policeHp <= 0)
        {
            OffBattleflag();
            Destroy(gameObject);
        }
    }
}
