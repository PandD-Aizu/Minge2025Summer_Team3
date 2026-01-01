using System;
using Minge2025Summer.Scripts.InGame.InteractableObjects.Interface;
using Minge2025Summer.Scripts.InGame.ReiScript.ItemScript;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.KeySystemScript
{
    public class PlayerKeySysModel : MonoBehaviour
    {
        [SerializeField] private float interactDistance = 20.0f;

        private Subject<IDoor> onDoorOpened = new  Subject<IDoor>();
        public IObservable<IDoor> OnDoorOpened => onDoorOpened;
        private Subject<Unit> onDoorOpenFailed = new Subject<Unit>();
        public IObservable<Unit> OnDoorOpenFailed => onDoorOpenFailed;

        /// <summary>
        /// 鍵付き扉に触れたときの処理
        /// </summary>
        public void TryInteractKey(ReiItemInventoryModel inventoryModel)
        {
            // Camera.main の存在チェック
            if (Camera.main == null)
            {
                Debug.LogWarning("[PlayerKeySysModel] Main Camera not found. Cannot raycast to interact with doors.");
                return;
            }

            // Raycast
            if (!Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, interactDistance))
            {
                Debug.Log("[PlayerKeySysModel] Raycast did not hit anything.");
                return;
            }

            if (hit.collider == null)
            {
                Debug.Log("[PlayerKeySysModel] Raycast hit but collider is null.");
                return;
            }

            var hitObj = hit.collider.gameObject;
            Debug.Log($"[PlayerKeySysModel] Raycast hit object: {hitObj.name}");

            if (!hit.collider.TryGetComponent<IDoor>(out var door))
            {
                Debug.Log("[PlayerKeySysModel] Hit object is not a door.");
                return;
            }

            InteractWithDoor(door, inventoryModel);
        }

        public void InteractWithDoor(IDoor door, ReiItemInventoryModel inventoryModel)
        {
            // 鍵がかかっている場合
            if (!door.IsUnLocked)
            {
                var keyID = door.RequiredKeyID ?? string.Empty;
                Debug.Log($"[PlayerKeySysModel] Door requires key: '{keyID}'");

                if (inventoryModel == null)
                {
                    Debug.LogWarning("[PlayerKeySysModel] inventoryModel is null. Cannot check keys.");
                    onDoorOpenFailed.OnNext(Unit.Default);
                    return;
                }

                // Try unlock
                bool consumed = inventoryModel.TryConsumeKey(keyID);

                // フォールバック: 大文字小文字や余分な空白の差分で見つからない場合にのみ探索して再試行
                if (!consumed)
                {
                    try
                    {
                        var keyInv = inventoryModel.GetKeyItemInventory;
                        foreach (var k in keyInv.Keys)
                        {
                            if (string.Equals(k.Trim(), keyID.Trim(), StringComparison.OrdinalIgnoreCase))
                            {
                                consumed = inventoryModel.TryConsumeKey(k);
                                if (consumed) break;
                            }
                        }
                    }
                    catch
                    {
                        /* Not implement */
                    }
                }

                if (consumed)
                {
                    door.UnLock();
                    onDoorOpened.OnNext(door);
                }
                else
                {
                    onDoorOpenFailed.OnNext(Unit.Default);
                }
            }
            else
            {
                door.TryOpen();
            }
        }
    }
}