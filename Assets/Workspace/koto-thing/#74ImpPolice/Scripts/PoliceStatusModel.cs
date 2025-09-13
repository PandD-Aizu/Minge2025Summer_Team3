using UnityEngine;
using Workspace.momiji1107;

namespace Workspace.koto_thing
{
    public class PoliceStatusModel : MonoBehaviour, IEnemyStatus
    {
        [Header("依存関係")] 
        [SerializeField, Tooltip("警官の親GameObject")]
        private GameObject policeParentObject;
        
        [Header("ステータス")] 
        [SerializeField, Tooltip("体力")]
        private float hp = 1000.0f;

        public void ReceiveDamage(float damage)
        {
            hp -= damage;
            
            // TODO: 敵が倒れるアニメーションとか
            if (hp <= 0)
            {
                Destroy(policeParentObject);
            }
        }
    }
}