using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Workspace.momiji1107;

namespace Workspace.koto_thing
{
    public class PoliceHPModel : MonoBehaviour, IEnemyHP
    {
        [Header("依存関係")] 
        [SerializeField, Tooltip("警官の親GameObject")] private GameObject policeParentObject;
        
        [Header("ステータス")] 
        [SerializeField, Tooltip("体力")] private float hp = 1000.0f;

        [Header("当たり判定の設定")] 
        [SerializeField, Tooltip("当たり判定をもつオブジェクトの親")] private GameObject hitColliderObjectParent;
        [SerializeField, Tooltip("当たり判定")] private List<Collider> hitCollider;

        public void ReceiveDamage(float damage)
        {
            hp -= damage;
            if (hp <= 0)
            {
                Destroy(policeParentObject);
            }
        }
    }
}