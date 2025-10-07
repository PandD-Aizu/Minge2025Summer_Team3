using UnityEngine;

namespace Workspace.koto_thing
{
    public class PolicePartHitResponder : MonoBehaviour, IEnemyPartHitReceiver
    {
        [Header("依存関係")]
        [SerializeField] private PoliceMoveModel moveModel;

        [Header("脚被弾スロー設定")]
        [SerializeField, Tooltip("脚に被弾した際の速度倍率(0~1)")] private float legShotSlowMultiplier = 0.5f;
        [SerializeField, Tooltip("脚被弾スロー継続時間(秒)")] private float legShotSlowDuration = 2.0f;
        [SerializeField, Tooltip("同じ強度再被弾で残り時間をリフレッシュするか")] private bool refreshIfEqual = true;

        private void Start()
        {
            if (moveModel == null)
            {
                // 近傍から自動取得 (階層構造に柔軟対応)
                moveModel = GetComponent<PoliceMoveModel>()
                           ?? GetComponentInChildren<PoliceMoveModel>()
                           ?? GetComponentInParent<PoliceMoveModel>();
            }
        }

        /// <summary>
        /// 部位被弾イベント (GunDamageModel 経由で呼ばれる)
        /// </summary>
        public void OnPartHit(EnemyBodyParts part, float finalDamage)
        {
            Debug.Log("PolicePartHitResponder: OnPartHit " + part + " Damage: " + finalDamage, this);
            
            if (part == EnemyBodyParts.LEFT_LEG || part == EnemyBodyParts.RIGHT_LEG)
            {
                if (moveModel != null)
                {
                    moveModel.ApplyLegSlow(legShotSlowMultiplier, legShotSlowDuration, refreshIfEqual);
                }
            }
        }
    }
}
