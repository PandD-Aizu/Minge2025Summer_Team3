using System.Collections.Generic;
using Minge2025Summer.Scripts.InGame.EnemyScript.Interface;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EnemyScript.Boss
{
    public class BossHpModel : MonoBehaviour, IEnemyHP
    {
        [Header("依存関係")] 
        [SerializeField, Tooltip("ボスの親のGameObject")] private GameObject policeParentObject;
        
        [Header("ステータス")]
        [SerializeField, Tooltip("体力")] private float hp = float.MaxValue;

        [Header("当たり判定の設定")] 
        [SerializeField, Tooltip("当たり判定をもつオブジェクトの親")] private GameObject hitColliderObjectParent;
        [SerializeField, Tooltip("当たり判定")] private List<Collider> hitColliders;

        public void ReceiveDamage(float damage)
        {
            hp -= damage;
        }
    }
}