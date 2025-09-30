using System;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerHpModel : MonoBehaviour
    {
        [SerializeField] private float maxHp = 100;
        
        public float GetMaxHp => maxHp;

        private ReactiveProperty<float> currentHp = new ReactiveProperty<float>();
        public float CurrentHp { get => currentHp.Value; set => currentHp.Value = Mathf.Clamp(value, 0, maxHp); }

        public IObservable<float> CurrentHpObservable => currentHp.AsObservable();
    }
}
