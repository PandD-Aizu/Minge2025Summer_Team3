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

        private readonly Subject<Unit> onDoorOpened = new ();
        public IObservable<Unit> OnDoorOpened => onDoorOpened;
        private readonly Subject<Unit> onDoorOpenFailed = new ();
        public IObservable<Unit> OnDoorOpenFailed => onDoorOpenFailed;

        public string RequiredKeyID => requiredKeyID;
        public bool IsUnLocked => isUnlocked;

        public void UnLock()
        {
            if (isUnlocked)
                return;

            isUnlocked = true;
        }
    }
}