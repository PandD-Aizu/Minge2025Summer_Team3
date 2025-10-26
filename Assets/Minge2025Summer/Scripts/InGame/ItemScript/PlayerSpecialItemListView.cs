using System.Collections.Generic;
using Minge2025Summer.Scripts.InGame.ItemScript.Interface;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ItemScript
{
    public class PlayerSpecialItemListView : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private SpecialItemListEntry entryPrefab;

        private readonly List<ISpecialItem> items = new ();
        private readonly List<SpecialItemListEntry> entries = new ();

        public IReadOnlyList<ISpecialItem> Items => items;

        public void SetSelection(int index)
        {
            for (int i = 0; i < entries.Count; i++)
                entries[i].SetHighlighted(i == index);
        }
        
        public void OnModelChanged(ISpecialItem item, int amount, bool removed)
        {
            if (removed || amount <= 0)
                RemoveItem(item);
            else if (!items.Contains(item))
                AddItem(item);
        }

        public void RebuildAll(IEnumerable<ISpecialItem> source)
        {
            Clear();
            foreach (var it in source)
                AddItem(it);
        }
        
        /* 以下ヘルパー関数 */

        private void AddItem(ISpecialItem item)
        {
            items.Add(item);

            var entry = Instantiate(entryPrefab, content);
            entry.Bind(item);
            entries.Add(entry);
        }

        private void RemoveItem(ISpecialItem item)
        {
            var index = items.IndexOf(item);
            if (index < 0)
                return;
            
            Destroy(entries[index].gameObject);
            items.RemoveAt(index);
            entries.RemoveAt(index);
        }

        private void Clear()
        {
            foreach(var e in entries)
                Destroy(e.gameObject);
            
            items.Clear();
            entries.Clear();
        }
    }
}