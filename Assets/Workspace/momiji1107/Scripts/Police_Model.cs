using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using Minge2025Summer.Scripts.InGame;
using Minge2025Summer.Scripts.InGame.EnemyScript.Enum;
using Minge2025Summer.Scripts.InGame.EnemyScript.Interface;
using Minge2025Summer.Scripts.InGame.PlayerStatusScript;
using Workspace.koto_thing;

public class Police_Model : MonoBehaviour, IEnemyHP, IEnemyPartHitReceiver
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

    [Header("被弾デバフ(足)")]
    [SerializeField, Tooltip("足(脚)に被弾した際の移動速度倍率")] private float legShotSlowMultiplier = 0.5f;
    [SerializeField, Tooltip("足被弾スロー継続時間(秒)")] private float legShotSlowDuration = 2.0f;
    [SerializeField, Tooltip("足に再度被弾したら効果時間をリフレッシュするか")] private bool refreshLegSlow = true;

    private float currentCoolDown = 0.0f;         // 現在のクールダウン時間
    private float currentSpeed = 0.0f;            // 現在の速度
    private Vector3 moveDirection = Vector3.zero; // 移動方向

    private float currentSlowMultiplier = 1f;      // 現在のスロー倍率(1=通常)
    private float slowExpireTime = 0f;             // スロー終了時刻

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
            // スロー時間更新判定
            if (currentSlowMultiplier < 1f && Time.time >= slowExpireTime)
            {
                currentSlowMultiplier = 1f; // スロー解除
            }

            // 方向と速度を決定
            Vector3 direction = (player.transform.position - transform.position).normalized;
            float targetSpeed = policeMoveSpeed * currentSlowMultiplier; // 足スロー考慮

            // スピードを調整
            if (Vector3.Distance(transform.position, player.transform.position) > 0.1f)
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
            else
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0.0f, deceleration * Time.deltaTime);

            // 移動ベクトルを計算して移動 (クランプもスロー後の上限で)
            currentSpeed = Mathf.Clamp(currentSpeed, 0.0f, targetSpeed);
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
    
        if (currentCoolDown < attackCoolDown)
            return; // クールダウン未完了

        // プレイヤー方向へ少し上からレイを飛ばす
        var origin = transform.position + Vector3.up * 1.0f;
        var dir = (player.transform.position + Vector3.up * 0.9f) - origin;
        var dist = Mathf.Max(0.1f, battleDistance);

        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dist))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // 親方向優先でPlayerHpModelを取得
                var hp = hit.collider.GetComponentInParent<PlayerHpModel>() ?? hit.collider.GetComponentInChildren<PlayerHpModel>();
                if (hp != null)
                {
                    hp.CurrentHp -= policeAttack;
                    currentCoolDown = 0.0f;
                }
            }
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

    /// <summary>
    /// 部位被弾コールバック (足に当たったらスロー)
    /// </summary>
    public void OnPartHit(EnemyBodyParts part, float finalDamage)
    {
        if (part == EnemyBodyParts.LEFT_LEG || part == EnemyBodyParts.RIGHT_LEG)
        {
            // 既により強い(=小さい)スローがかかっている場合は保持 / リフレッシュ設定
            bool apply = currentSlowMultiplier <= 0f || legShotSlowMultiplier < currentSlowMultiplier;
            if (apply)
            {
                currentSlowMultiplier = Mathf.Clamp(legShotSlowMultiplier, 0.05f, 1f);
                slowExpireTime = Time.time + legShotSlowDuration;
            }
            else if (refreshLegSlow && currentSlowMultiplier <= legShotSlowMultiplier + 0.0001f)
            {
                // 同じ強さのスローをリフレッシュ
                slowExpireTime = Time.time + legShotSlowDuration;
            }
        }
    }
}
