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
        
        private readonly Dictionary<ISpecialItem, int> specialItemList = new();

        public Subject<InventoryItemEvent> OnItemChanged = new ();

        /// <summary>
        /// アイテムを取得する
        /// </summary>
        public void GetItem()
        {
            var cam = Camera.main;
            if (cam == null) return;
            Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, maxDistance);
            if (hit.collider != null && hit.collider.TryGetComponent<IItem>(out var picked))
            {
                picked.SetIsGet = true;
                if (picked is MonoBehaviour pickedMb)
                {
                    var view = pickedMb.GetComponentInChildren<IItemView>();
                    if (view != null) view.Hide();
                }

                // --- 特殊アイテムは通常インベントリに入れず専用リストへ ---
                if (picked is ISpecialItem special)
                {
                    var storedSpecial = AddSpecialItem(special);
                    OnItemChanged.OnNext(new InventoryItemEvent(storedSpecial, storedSpecial.GetAmount));
                    return; // 通常インベントリ処理は行わない
                }

                // 実際にインベントリに保持される参照 (スタック時は既存アイテム参照)
                var storedItem = AddItem(picked);
                // Ammo: 新規スロットとして追加された場合のみ適用済みフラグを付与
                if (picked is AmmoModel pickedAmmo && ReferenceEquals(storedItem, pickedAmmo))
                {
                    pickedAmmo.MarkAppliedOnPickup();
                }
                // 必要なら適用監視（既存の場合は内部で二重購読防止）
                SubscribeApplied(storedItem);
                OnItemChanged.OnNext(new InventoryItemEvent(storedItem, storedItem.GetAmount));
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
            // 旧仕様: itemList.Keys.OfType<ISpecialItem>()
            // 新仕様: 専用コレクションから列挙
            return specialItemList.Keys;
        }
        
        /* ---以下ヘルパー関数--- */
        
        /// <summary>
        /// アイテムを追加する
        /// </summary>
        /// <param name="item">追加するアイテム</param>
        private IItem AddItem(IItem item)
        {
            // 特殊アイテムはここでは扱わない（GetItemで分岐済）
            if (item is ISpecialItem) return item;

            foreach (var existing in itemList.Keys.ToList())
            {
                if (CanStack(existing, item))
                {
                    existing.AddAmount(item.GetAmount);
                    itemList[existing] = existing.GetAmount;
                    if (item is MonoBehaviour mb && existing is MonoBehaviour emb && mb.gameObject != emb.gameObject)
                    {
                        Destroy(mb.gameObject);
                    }
                    return existing; // 既存スタック参照を返す
                }
            }

            // 新規スタック
            itemList[item] = item.GetAmount;
            SubscribeApplied(item);
            return item;
        }

        /// <summary>
        /// スタック可能かどうかチェック
        /// </summary>
        /// <param name="a">アイテム</param>
        /// <param name="b">アイテム</param>
        /// <returns>スタック可能ならtrueを返す</returns>
        private bool CanStack(IItem a, IItem b)
        {
            if (a == b) return true; // 同一参照はマージ許可（安全策）
            if (a.GetType() != b.GetType()) return false; // 型違いは不可

            // 特殊アイテム: 既存仕様を尊重（同一ID & 双方CanStackなら）
            if (a is ISpecialItem sa && b is ISpecialItem sb)
            {
                return sa.SpecialID == sb.SpecialID && sa.CanStack && sb.CanStack;
            }

            // 鍵: 既存仕様 (KeyID一致でスタック) ※必要ならIStackable化も検討
            if (a is IKey ka && b is IKey kb)
            {
                return ka.KeyID == kb.KeyID; // 鍵は従来通り
            }

            // 新仕様: 双方が IStackable を実装し、かつ CanStack true のときのみスタック
            var saStack = a as IStackable;
            var sbStack = b as IStackable;
            if (saStack != null && sbStack != null)
            {
                if (saStack.CanStack && sbStack.CanStack)
                {
                    return true;
                }
            }

            // それ以外は同型でも別インスタンスとして扱う
            return false;
        }

        /// <summary>
        /// アイテムの使用を監視
        /// </summary>
        /// <param name="item">アイテム</param>
        private void SubscribeApplied(IItem item)
        {
            // IsAppliableでなければ監視不要
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
                        OnItemChanged.OnNext(new InventoryItemEvent(item, item.GetAmount));
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
                OnItemChanged.OnNext(new InventoryItemEvent(item, item.GetAmount));
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
            if (item == null || !specialItemList.ContainsKey(item))
                return false;

            if (!item.CanUse(context, out failReason))
                return false;

            if (item.IsConsumable)
            {
                var becameZero = item.ConsumeOne();
                if (becameZero || item.GetAmount <= 0)
                {
                    specialItemList.Remove(item);
                    OnItemChanged.OnNext(new InventoryItemEvent(item, 0, true));
                }
                else
                {
                    specialItemList[item] = item.GetAmount;
                    OnItemChanged.OnNext(new InventoryItemEvent(item, item.GetAmount));
                }
            }
            else
            {
                item.ApplyItem();
                OnItemChanged.OnNext(new InventoryItemEvent(item, item.GetAmount));
            }
            return true;
        }

        // --- 特殊アイテム追加ヘルパー（通常インベントリとは独立） ---
        private ISpecialItem AddSpecialItem(ISpecialItem item)
        {
            if (item.IsUnique)
            {
                if (specialItemList.Keys.Any(x => x.SpecialID == item.SpecialID))
                {
                    if (item is MonoBehaviour dup) Destroy(dup.gameObject);
                    return item;
                }
            }

            foreach (var existing in specialItemList.Keys.ToList())
            {
                if (existing.SpecialID == item.SpecialID && existing.CanStack && item.CanStack)
                {
                    existing.AddAmount(item.GetAmount);
                    specialItemList[existing] = existing.GetAmount;
                    if (item is MonoBehaviour mb && existing is MonoBehaviour emb && mb.gameObject != emb.gameObject)
                    {
                        Destroy(mb.gameObject);
                    }
                    return existing;
                }
            }

            specialItemList[item] = item.GetAmount;
            return item;
        }

        /// <summary>
        /// 弾丸同期用スナップショットを構築する
        /// </summary>
        public IEnumerable<(AmmoType type, int count)> BuildAmmoSnapshot()
        {
            foreach (var kv in itemList)
            {
                if (kv.Key is AmmoModel ammo)
                {
                    yield return (ammo.GetAmmoType, ammo.GetAmount);
                }
            }
        }

        /// <summary>
        /// GunModel 側で弾薬所持数が変化した際にインベントリ内 AmmoModel を同期する
        /// </summary>
        public bool UpdateAmmoFromGun(AmmoType ammoType, int newCount)
        {
            foreach (var key in itemList.Keys.ToList())
            {
                if (key is AmmoModel ammo && ammo.GetAmmoType == ammoType)
                {
                    if (newCount <= 0)
                    {
                        itemList.Remove(key);
                        if (appliedSubscriptions.TryGetValue(key, out var disp))
                        {
                            disp.Dispose();
                            appliedSubscriptions.Remove(key);
                        }
                        OnItemChanged.OnNext(new InventoryItemEvent(key, 0, true));
                    }
                    else
                    {
                        ammo.SetAmountAbsolute(newCount);
                        itemList[key] = newCount;
                        OnItemChanged.OnNext(new InventoryItemEvent(key, newCount));
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
