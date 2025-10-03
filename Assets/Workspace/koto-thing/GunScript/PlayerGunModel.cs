using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerGunModel : MonoBehaviour
    {
        [Header("取得関係")]
        [SerializeField] private float interactionRange = 20.0f;
        
        private List<IGun> equippedGuns = new();
        public IReadOnlyList<IGun> EquippedGuns => equippedGuns.AsReadOnly();

        private Subject<IGun> onEquipGun = new();
        public IObservable<IGun> OnEquipGun => onEquipGun.AsObservable();
        
        
        private int currentGunIndex = 0;

        /// <summary>
        /// 銃の取得を試みる
        /// </summary>
        /// <returns></returns>
        public bool TryGetGun()
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interactionRange))
            {
                if (hit.collider.TryGetComponent<IGun>(out var gun))
                {
                    onEquipGun.OnNext(gun);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 装備中の銃を変更する
        /// </summary>
        /// <param name="direction">マウスホイールの方向</param>
        public void ChangeCurrentEquippedGun(int direction)
        {
            if (direction == 0) return;
            
            currentGunIndex += direction;
            if (currentGunIndex < 0)
                currentGunIndex = equippedGuns.Count - 1;
            else if (currentGunIndex >= equippedGuns.Count)
                currentGunIndex = 0;
            
            onEquipGun.OnNext(equippedGuns[currentGunIndex]);
        }
    }
}