using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerGunModel : MonoBehaviour
    {
        [Header("取得関係")] 
        [SerializeField] private float interactionRange = 40.0f;
        [SerializeField] private LayerMask interactionLayerMask = ~0; // 全レイヤー
        [SerializeField, Tooltip("拾った銃を保持する親")] private Transform gunHoldPoint;
        [SerializeField, Tooltip("取得時にデバッグRayを表示")] private bool debugRay = true;
        
        private List<IGun> equippedGuns = new();
        public IReadOnlyList<IGun> EquippedGuns => equippedGuns.AsReadOnly();
        public IGun CurrentGun => (equippedGuns.Count == 0) ? null : equippedGuns[currentGunIndex];

        private Subject<IGun> onEquipGun = new();
        public IObservable<IGun> OnEquipGun => onEquipGun.AsObservable();
        
        private int currentGunIndex = 0;

        /// <summary>
        /// 銃の取得を試みる
        /// </summary>
        public bool TryGetGun()
        {
            var cam = Camera.main;
            if (cam == null) 
                return false;
            
            Vector3 origin = cam.transform.position;
            Vector3 dir = cam.transform.forward;

            if (debugRay)
                Debug.DrawRay(origin, dir * interactionRange, Color.cyan, 1.0f);

            if (!Physics.Raycast(origin, dir, out RaycastHit hit, interactionRange, interactionLayerMask))
                return false;

            if (!TryResolveGun(hit.collider, out var gun))
                return false;

            AddGunIfNew(gun);
            return true;
        }

        private bool TryResolveGun(Collider col, out IGun gun)
        {
            // 直接 or 親階層 or 子階層
            if (col.TryGetComponent<IGun>(out gun)) 
                return true;
            
            gun = col.GetComponentInParent<IGun>();
            if (gun != null) 
                return true;
            
            gun = col.GetComponentInChildren<IGun>();
            
            return gun != null;
        }

        /// <summary>
        /// 装備中の銃を変更する（マウスホイール方向など）
        /// </summary>
        /// <param name="direction">-1 or +1 (0は無視)</param>
        public void ChangeCurrentEquippedGun(int direction)
        {
            if (direction == 0) 
                return;
            
            if (equippedGuns.Count == 0) 
                return;
            
            if (equippedGuns.Count == 1) 
                return;
            
            currentGunIndex = (currentGunIndex + direction) % equippedGuns.Count;
            if (currentGunIndex < 0) 
                currentGunIndex += equippedGuns.Count;
            
            onEquipGun.OnNext(equippedGuns[currentGunIndex]);
        }

        /// <summary>
        /// 新しい銃ならリストに追加し装備イベントを発行
        /// </summary>
        private void AddGunIfNew(IGun gun)
        {
            if (equippedGuns.Contains(gun))
            {
                currentGunIndex = equippedGuns.IndexOf(gun);
                onEquipGun.OnNext(equippedGuns[currentGunIndex]);
                return;
            }

            equippedGuns.Add(gun);
            currentGunIndex = equippedGuns.Count - 1;

            var gunBehaviour = gun as MonoBehaviour;
            if (gunBehaviour != null && gunHoldPoint != null)
            {
                var t = gunBehaviour.transform;
                t.SetParent(gunHoldPoint, worldPositionStays: false);
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                
                // その他のColliderは無効化（既存処理）
                if (t.TryGetComponent<Collider>(out var col)) 
                    col.enabled = false;

                if (t.TryGetComponent<Light>(out var lightComponent))
                    lightComponent.enabled = false;
                
                if (t.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.isKinematic = true;
                    rb.detectCollisions = false;
                }
            }

            onEquipGun.OnNext(gun);
        }
    }
}