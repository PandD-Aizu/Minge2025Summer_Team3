using System;
using Minge2025Summer.Scripts.InGame.GunScript;
using Minge2025Summer.Scripts.InGame.ItemScript.Interface;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ItemScript
{
    public class PlayerItemPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private PlayerItemModel model;
        [SerializeField] private PlayerItemView view;
        [SerializeField] private ItemEmitter emitter;
        [SerializeField] private GunModel gunModel;

        private CompositeDisposable disposable = new ();
        private bool suppressGunSync = false; // GunModel->Inventory 反映中は逆方向同期を抑制

        private void Start()
        {
            SubscribeEvents();
            
            gunModel.SyncFromInventory(model.BuildAmmoSnapshot());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                model.GetItem();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                view.SwitchInventoryScreen();
            }
            
            if (view.IsOpen)
            {
                if (Input.GetKeyDown(KeyCode.W)) view.MoveSelection(Vector2Int.down);
                if (Input.GetKeyDown(KeyCode.S)) view.MoveSelection(Vector2Int.up);
                if (Input.GetKeyDown(KeyCode.A)) view.MoveSelection(Vector2Int.left);
                if (Input.GetKeyDown(KeyCode.D)) view.MoveSelection(Vector2Int.right);
            }
        }

        private void SubscribeEvents()
        {
            model.OnItemChanged
                .Subscribe(itemChangeEvent =>
                {
                    // 特殊アイテムは通常インベントリスロットに表示しない
                    if (itemChangeEvent.Item is ISpecialItem)
                    {
                        // 取得/消費サウンドなどは共通で鳴らす
                        emitter.PlayPickUp();
                        return;
                    }

                    if (itemChangeEvent.Removed)
                        view.RemoveItemSlot(itemChangeEvent.Item);
                    else
                        view.UpdateItemSlot(itemChangeEvent.Item, itemChangeEvent.Amount);
                    
                    // GunModel からの反映中でなければ Inventory をソースとして GunModel を再同期
                    if (!suppressGunSync && gunModel != null)
                    {
                        gunModel.SyncFromInventory(model.BuildAmmoSnapshot());
                    }

                    emitter.PlayPickUp();
                })
                .AddTo(disposable);

            // 発砲時のマガジン内視覚更新
            gunModel.CurrentEquippedGun?.OnFire
                .Subscribe(_ =>
                {
                    view.ConsumeOneAmmoForEquippedGun(gunModel);
                })
                .AddTo(disposable);

            // GunModel 側（リロードなど）で所持弾が変わった場合、Inventory / UI に反映
            gunModel.AmmoChanged
                .Subscribe(tuple =>
                {
                    if (gunModel == null) return;
                    // ループ防止フラグ
                    suppressGunSync = true;
                    var updated = model.UpdateAmmoFromGun(tuple.ammoType, tuple.count);
                    suppressGunSync = false;
                    // 更新されなかった(=インベントリに該当弾薬が無かった)場合、GunModel が残量0を出したシナリオなどは無視
                })
                .AddTo(disposable);
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}