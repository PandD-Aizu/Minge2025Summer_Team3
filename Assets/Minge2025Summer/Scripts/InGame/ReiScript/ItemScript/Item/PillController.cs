using System;
using FMODUnity;
using Minge2025Summer.Scripts.InGame.PlayerStatusScript;
using UniRx;
using UnityEditor.EventSystems;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript.ItemScript.Item
{
    public class PillController : MonoBehaviour, IDisposable
    {
        [SerializeField] private Pill model;
        
        private PlayerHpModel playerHpModel;
        private StudioEventEmitter emitter;
        private CompositeDisposable disposables = new ();

        private void Start()
        {
            playerHpModel = FindFirstObjectByType<PlayerHpModel>();
            emitter = GameObject.Find("PillApplyEmitter").GetComponent<StudioEventEmitter>();
            
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            model.OnGetItem
                .Subscribe(_ =>
                {
                    model.HideItem();
                })
                .AddTo(this);
            
            model.OnApplyItem
                .Subscribe(healAmount =>
                {
                    playerHpModel.CurrentHp += Mathf.Clamp(healAmount, 0, playerHpModel.GetMaxHp);
                    emitter.Play();
                })
                .AddTo(this);
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}