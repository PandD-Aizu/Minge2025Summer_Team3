using System;
using Minge2025Summer.Scripts.InGame.FlashLightScript.Enum;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.FlashLightScript
{
    public class BatteryLevelModel : MonoBehaviour
    {
        [Header("懐中電灯のバッテリー設定")]
        [SerializeField, Tooltip("残りの照射可能時間(s)")] private float batteryLevel = 300.0f;
        [SerializeField, Tooltip("バッテリー容量の最大値")] private float maxBatteryLevel = 300.0f;
        [SerializeField, Tooltip("バッテリーが減る速さ(= drainRate * Time.deltaTime)")] private float drainRate = 1.0f;
        [SerializeField, Tooltip("フリックし始める時間")] private float flickerThreshold = 20f;

        private ReactiveProperty<bool> isFlickering = new (false);
        public IObservable<bool> IsFlickeringObservable => isFlickering;
        public bool IsFlickering { get => isFlickering.Value; set => isFlickering.Value = value; }
        
        private Subject<Unit> onBatteryDepleted = new ();
        public IObservable<Unit> OnBatteryDepleted => onBatteryDepleted.AsObservable();

        public float GetBatteryLevel => batteryLevel;
        public float GetMaxBatteryLevel => maxBatteryLevel;

        /// <summary>
        /// バッテリーを減らす
        /// </summary>
        public void DrainBattery(FlickerState currentFlickerState)
        {
            if (batteryLevel > 0 && currentFlickerState != FlickerState.OFF)
            {
                batteryLevel -= drainRate * Time.deltaTime;
                batteryLevel = Mathf.Clamp(batteryLevel, 0, maxBatteryLevel);
            }
            
            if (batteryLevel <= 0)
            {
                onBatteryDepleted.OnNext(Unit.Default);
            }
        }

        /// <summary>
        /// フリックするかどうかの判定
        /// </summary>
        public void CheckFlicker()
        {
            if (batteryLevel <= flickerThreshold && !IsFlickering)
                IsFlickering = true;
            else if (batteryLevel > flickerThreshold && IsFlickering)
                IsFlickering = false;
        }

        /// <summary>
        /// バッテリーを充電する
        /// </summary>
        /// <param name="amount">増やす照射時間</param>
        public void RechargeBattery(float amount)
        {
            batteryLevel += amount;
            batteryLevel = Mathf.Clamp(batteryLevel, 0, maxBatteryLevel);
        }
    }
}