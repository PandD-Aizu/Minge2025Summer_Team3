using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.Sample
{
    public class PlayerHPModel : MonoBehaviour
    {
        [SerializeField] private int maxHP = 100;
        [SerializeField] private int currentHP = 100;
    
        // HPが変化したときに通知するためのイベント
        public Subject<int> OnHPChanged = new ();

        /// <summary>
        /// Playerにダメージを与える
        /// </summary>
        /// <param name="damage">受けるダメージ</param>
        public void TakeDamage(int damage)
        {
            currentHP -= damage;
            if (currentHP < 0) 
                currentHP = 0;
        
            OnHPChanged.OnNext(currentHP);
        }
    }
}
