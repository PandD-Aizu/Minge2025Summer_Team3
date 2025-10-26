using System;
using Minge2025Summer.Scripts.InGame.InteractableObjects.Interface;
using Minge2025Summer.Scripts.InGame.ItemScript;
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
        public void TryInteractKey(PlayerItemModel playerItemModel)
        {
            if (!Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, interactDistance))
                return;

            if (!hit.collider.TryGetComponent<IDoor>(out var door))
                return;

            // 鍵がかかっている場合
            if (!door.IsUnLocked)
            {
                if (TryUnlockKey(door, playerItemModel))
                    onDoorOpened.OnNext(door);
                else
                    onDoorOpenFailed.OnNext(Unit.Default);
            }
            else
            {
                door.TryOpen();
            }
        }

        /// <summary>
        /// 解錠する
        /// </summary>
        /// <param name="door">鍵つきオブジェクトのモデルクラス</param>
        /// <param name="playerItemModel">アイテム管理のモデルクラス</param>
        /// <returns>解錠できたかどうか</returns>
        private bool TryUnlockKey(IDoor door, PlayerItemModel playerItemModel = null)
        {
            if (door.IsUnLocked)
                return true;

            if (playerItemModel == null)
                return false;

            // 必要鍵IDを保持しているか
            var keyID = door.RequiredKeyID;
            if (string.IsNullOrEmpty(keyID))
            {
                door.UnLock();
                return true;
            }

            if (playerItemModel.HasKey(keyID))
            {
                // 消費に成功した場合のみ解錠
                if (playerItemModel.TryConsumeKey(keyID))
                {
                    door.UnLock();
                    return true;
                }
            }

            return false;
        }
    }
}