using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class BatteryModel : MonoBehaviour, IItem
    {
        [Header("バッテリーアイテムの設定")] 
        [SerializeField, Tooltip("得られるアイテムの個数")] private int batteryCount;
        [SerializeField, Tooltip("バッテリーによって得られる照射時間")] private float illuminationTime;

        [Header("インベントリUIの設定")] 
        [SerializeField, Tooltip("表示名")] private string displayName;
        [SerializeField, Tooltip("説明文"), TextArea] private string description;

        [Header("フラグ")] 
        [SerializeField] private bool isGet;
        [SerializeField] private bool isApplied;
        
        [Header("バッテリー2D画像")]
        [SerializeField, Tooltip("インベントリに表示させる画像")] private Sprite batterySprite;

        private readonly Subject<Unit> onApplied = new ();
        public IObservable<Unit> OnApplied => onApplied;
        
        public int GetAmount => batteryCount;
        public float GetIlluminationTime => illuminationTime;
        public string GetDisplayName => displayName;
        public string GetDescription => description;
        public Sprite GetSprite => batterySprite;

        public bool SetIsGet { get => isGet; set => isGet = value; }

        public bool GetIsApplied => isApplied;
        
        public void ApplyItem()
        {
            if (isApplied || !isGet)
                return;

            isApplied = true;
        }
    }
}