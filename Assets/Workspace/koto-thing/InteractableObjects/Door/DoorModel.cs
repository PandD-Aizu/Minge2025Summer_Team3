using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class DoorModel : MonoBehaviour, IDoor
    {
        [Header("ドアの設定")]
        [SerializeField, Tooltip("必要とされているドアの設定")] private string requiredKeyID;
        [SerializeField, Tooltip("ドアが空いているかどうか")] private bool isUnlocked = false;
        [SerializeField, Tooltip("ドアタイプ")] private DoorType doorType = DoorType.Sliding;
        [SerializeField, Tooltip("スライド距離 / 開き戸なら無視")] private float openDistance = 2f;
        [SerializeField, Tooltip("開き戸の開き角度(度)")] private float openAngle = 90f;
        [SerializeField, Tooltip("開閉にかかる時間(秒)")] private float openDuration = 1.0f;
        [SerializeField, Tooltip("すでに開いた状態で開始するか")] private bool startOpened = false;

        private bool isOpen = false;

        private readonly Subject<Unit> onDoorOpened = new();
        public IObservable<Unit> OnDoorOpened => onDoorOpened;
        private readonly Subject<Unit> onDoorClosed = new();
        public IObservable<Unit> OnDoorOpenFailed => onDoorClosed;

        public string RequiredKeyID => requiredKeyID;
        public bool IsUnLocked => isUnlocked;
        public bool IsOpen => isOpen;
        public DoorType DoorType => doorType;
        public float OpenDistance => openDistance;
        public float OpenAngle => openAngle;
        public float OpenDuration => openDuration;

        private void Awake()
        {
            if (startOpened)
            {
                isOpen = true;
            }
        }

        public void UnLock()
        {
            if (isUnlocked) 
                return;
            
            isUnlocked = true;
        }

        /// <summary>
        /// ドアを開こうと試みる
        /// </summary>
        /// <param name="providedKeyID">所持鍵ID(不要ならnull)</param>
        /// <returns>開けたら true</returns>
        public bool TryOpen()
        {
            // 既に開いていたら閉じる
            if (isOpen)
            {
                onDoorClosed.OnNext(Unit.Default); // 扉を閉じる
                isOpen = false;
                return true; // 既に開いている
            }
            
            // 開く
            isOpen = true;
            onDoorOpened.OnNext(Unit.Default);
            return true;
        }

        /// <summary>
        /// 強制的に閉じた状態に戻す
        /// </summary>
        public void ForceClose()
        {
            isOpen = false;
        }
    }
}