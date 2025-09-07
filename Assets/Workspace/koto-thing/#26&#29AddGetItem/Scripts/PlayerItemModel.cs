using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerItemModel : MonoBehaviour
    {
        [Header("インタラクト可能な最大距離")]
        [SerializeField] private float maxDistance;
        
        private Dictionary<IItem, int> itemList = new ();
        private IItem currentItem;

        /// <summary>
        /// アイテムを取得する
        /// </summary>
        public void GetItem()
        {
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, maxDistance);

            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent<IItem>(out var item))
                {
                    item.SetIsGet(true);
                    AddItem(item);
                }
            }
        }

        public void UpdateItemList()
        {
            if (itemList == null || itemList.Count == 0) return;

            // 変更が必要なアイテムを記録するためのリスト
            var itemsToUpdate = new Dictionary<IItem, int>();
            var itemsToRemove = new List<IItem>();

            // 使用済みアイテムをチェックし、変更内容を記録
            foreach (var pair in itemList)
            {
                IItem item = pair.Key;
                if (item.GetIsApplied())
                {
                    int newCount = pair.Value - 1;
                    if (newCount > 0)
                        itemsToUpdate[item] = newCount;
                    else
                        itemsToRemove.Add(item);
                }
            }

            // 記録した内容に基づいてリストを更新
            foreach (var pair in itemsToUpdate)
            {
                itemList[pair.Key] = pair.Value;
            }

            // 記録した内容に基づいてアイテムを削除
            foreach (var item in itemsToRemove)
            {
                itemList.Remove(item);
            }
        }

        /// <summary>
        /// アイテムを適用する
        /// </summary>
        public void ApplyItem()
        {
            currentItem.ApplyItem();
        }
        
        /* ---以下ヘルパー関数--- */
        
        /// <summary>
        /// アイテムを追加する
        /// </summary>
        /// <param name="item">追加するアイテム</param>
        public void AddItem(IItem item)
        {
            if (itemList.ContainsKey(item))
                itemList[item]++;
            else
                itemList[item] = 1;
        }
    }
}