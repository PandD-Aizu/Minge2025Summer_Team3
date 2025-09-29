using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerItemModel : MonoBehaviour
    {
        [Header("インタラクト可能な最大距離")]
        [SerializeField] private float maxDistance;
        
        private Dictionary<IItem, int> itemList = new ();                     // アイテムとその数量の辞書
        private Dictionary<IItem, IDisposable> appliedSubscriptions = new (); // IAppliableの購読管理用辞書
        private IItem currentItem;
        
        public Subject<InventoryItemEvent> OnItemChanged = new ();

        /// <summary>
        /// アイテムを取得する
        /// </summary>
        public void GetItem()
        {
            // カメラの正面にあるアイテムをレイキャストで取得
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, maxDistance);
            if (hit.collider != null)
            {
                // IItemを持っているなら取得
                if (hit.collider.TryGetComponent<IItem>(out var item))
                {
                    // アイテム取得フラグを立てる
                    item.SetIsGet = true;
                    if (item is MonoBehaviour mb)
                    {
                        var view = mb.GetComponentInChildren<IItemView>();
                        if (view != null)
                            view.Hide();
                    }
                    
                    // インベントリに追加
                    AddItem(item);
                    SubscribeApplied(item);
                    OnItemChanged.OnNext(new InventoryItemEvent(item, item.GetAmount));
                }
            }
        }

        /// <summary>
        /// インベントリ内の鍵に特定のIDがあるかどうかをチェック
        /// </summary>
        /// <param name="keyID">鍵のID</param>
        /// <returns></returns>
        public bool HasKey(string keyID)
        {
            return itemList.Keys.OfType<IKey>()
                .Any(k => k.KeyID == keyID);
        }

        /// <summary>
        /// 特殊アイテムを列挙
        /// </summary>
        /// <returns>特殊アイテムのリスト</returns>
        public IEnumerable<ISpecialItem> EnumerateSpecialItems()
        {
            return itemList.Keys.OfType<ISpecialItem>();
        }
        
        /* ---以下ヘルパー関数--- */
        
        /// <summary>
        /// アイテムを追加する
        /// </summary>
        /// <param name="item">追加するアイテム</param>
        private int AddItem(IItem item)
        {
            // 特殊アイテムの重複チェック
            if (item is ISpecialItem si && si.IsUnique)
            {
                // 既に同じIDの特殊アイテムがある場合は追加しない
                if (itemList.Keys.OfType<ISpecialItem>()
                    .Any(x => x.SpecialID == si.SpecialID))
                {
                    // 重複した場合は追加せずに破棄
                    if (item is MonoBehaviour duplicateMb)
                        Destroy(duplicateMb.gameObject);

                    return 0;
                }
            }
            
            // 既存スタック探索
            foreach (var existing in itemList.Keys)
            {
                if (CanStack(existing, item))
                {
                    var before = existing.GetAmount;
                    existing.AddAmount(item.GetAmount);
                    itemList[existing] = existing.GetAmount;
                    if (item is MonoBehaviour mb && existing is MonoBehaviour emb && mb.gameObject != emb.gameObject)
                        Destroy(mb.gameObject);
                    
                    return itemList[existing];
                }
            }
            
            // 新規スタック追加
            itemList[item] = item.GetAmount;
            SubscribeApplied(item);
            
            return itemList[item];
        }

        /// <summary>
        /// スタック可能かどうかチェック
        /// </summary>
        /// <param name="a">アイテム</param>
        /// <param name="b">アイテム</param>
        /// <returns>スタック可能ならtrueを返す</returns>
        private bool CanStack(IItem a, IItem b)
        {
            // null チェック
            if (a == b) 
                return true;
            
            // 型が違うならスタック不可
            if (a.GetType() != b.GetType()) 
                return false;

            // 特殊アイテムの場合はID一致かつCanStackフラグが両方ともtrueでないとスタック不可
            if (a is ISpecialItem || b is ISpecialItem)
            {
                // 両方ともISpecialItemでなければスタック不可
                if (a is ISpecialItem sa && b is ISpecialItem sb)
                    return sa.SpecialID == sb.SpecialID && sa.CanStack && sb.CanStack;

                return false;
            }
            
            // 鍵の場合は ID 一致でスタック
            if (a is IKey ka && b is IKey kb)
                return ka.KeyID == kb.KeyID;
            
            return true; // 同型は基本スタック
        }

        /// <summary>
        /// アイテムの使用を監視
        /// </summary>
        /// <param name="item">アイテム</param>
        private void SubscribeApplied(IItem item)
        {
            // IAppliableでなければ監視不要
            if (item is not IAppliable appliable) 
                return;
            
            // 既に監視済みならスキップ
            if (appliedSubscriptions.ContainsKey(item)) 
                return;
            
            // 使用されたら数量更新
            var d = appliable.OnApplied.Subscribe(_ =>
            {
                // 数量が内部で減少済みなので辞書同期
                if (itemList.ContainsKey(item))
                {
                    // 0個以下なら辞書から削除
                    itemList[item] = item.GetAmount;
                    if (item.GetAmount <= 0)
                    {
                        // アイテム消費完了
                        itemList.Remove(item);
                        if (appliedSubscriptions.TryGetValue(item, out var disp))
                        {
                            disp.Dispose();
                            appliedSubscriptions.Remove(item);
                        }
                        
                        OnItemChanged.OnNext(new InventoryItemEvent(item, 0, true));
                    }
                    // まだ残っているなら数量更新
                    else
                    {
                        OnItemChanged.OnNext(new InventoryItemEvent(item, item.GetAmount, false));
                    }
                }
            });
            
            appliedSubscriptions[item] = d;
        }

        /// <summary>
        /// アイテムを消費する
        /// </summary>
        /// <param name="item">アイテム</param>
        /// <returns>アイテムを正常に消費できたらtrueを返す</returns>
        public bool ConsumeItem(IItem item)
        {
            if (item == null) 
                return false;
            
            if (!itemList.ContainsKey(item)) 
                return false;
            
            // アイテムの数量を1減らす
            var becameZero = item.ConsumeOne();
            if (becameZero)
            {
                // アイテム消費完了
                itemList.Remove(item);
                if (appliedSubscriptions.TryGetValue(item, out var disp))
                {
                    disp.Dispose(); 
                    appliedSubscriptions.Remove(item);
                }                
                
                OnItemChanged.OnNext(new InventoryItemEvent(item, 0, true));
            }
            // まだ残っているなら数量更新
            else
            {
                itemList[item] = item.GetAmount;
                OnItemChanged.OnNext(new InventoryItemEvent(item, item.GetAmount, false));
            }
            
            return true;
        }

        /// <summary>
        /// 鍵を消費する
        /// </summary>
        /// <param name="keyID">鍵のID</param>
        /// <returns>正常に消費できたらtrueを返す</returns>
        public bool TryConsumeKey(string keyID)
        {
            // KeyIDが空なら失敗
            if (string.IsNullOrEmpty(keyID)) 
                return false;
            
            // 対象鍵を検索(同一 KeyID の任意の1スタック)
            var target = itemList.Keys.OfType<IKey>().FirstOrDefault(k => k.KeyID == keyID);
            if (target == null)
                return false;
            
            // IKeyもIItem 実装なのでそのまま消費
            return ConsumeItem((IItem)target);
        }

        /// <summary>
        /// 特殊アイテムを使用する
        /// </summary>
        /// <param name="item">特殊アイテム</param>
        /// <param name="context">特殊アイテムの情報</param>
        /// <param name="failReason">実行失敗時のテキスト</param>
        /// <returns>使用出来たらtrueを返す</returns>
        public bool TryUseSpecial(ISpecialItem item, SpecialItemContext context, out string failReason)
        {
            failReason = null;
            if (item == null || !itemList.ContainsKey(item))
                return false;

            if (!item.CanUse(context, out failReason))
                return false;

            // アイテムを使用
            if (item.IsConsumable)
            {
                ConsumeItem(item);
            }
            // 消費しない場合は ApplyItem を呼び出すだけ
            else
            {
                item.ApplyItem();
                OnItemChanged.OnNext(new InventoryItemEvent(item, item.GetAmount, false));
            }

            return true;
        }
    }
}