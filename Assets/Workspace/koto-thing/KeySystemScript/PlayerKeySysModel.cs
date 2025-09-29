using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerKeySysModel : MonoBehaviour
    {
        [SerializeField] private float interactDistance = 20.0f;

        private Subject<Unit> onDoorOpened = new  Subject<Unit>();
        public IObservable<Unit> OnDoorOpened => onDoorOpened;
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
            
            Debug.Log("ドアに触れました");

            // ドアを開ける
            if (TryOpenDoor(door, playerItemModel))
                onDoorOpened.OnNext(Unit.Default);
            else
                onDoorOpenFailed.OnNext(Unit.Default);
        }

        /// <summary>
        /// ドアを開ける
        /// </summary>
        /// <param name="door">ドアのモデル</param>
        /// <param name="playerItemModel">アイテム管理のモデル</param>
        /// <returns></returns>
        private bool TryOpenDoor(IDoor door, PlayerItemModel playerItemModel = null)
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