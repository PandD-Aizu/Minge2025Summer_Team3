using System;
using Minge2025Summer.Scripts.InGame.ItemScript.Interface;
using UniRx;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ItemScript
{
    public class PlayerSpecialItemPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField] private PlayerItemModel model;
        [SerializeField] private PlayerSpecialItemListView listView;
        [SerializeField] private PlayerSpecialItemDetailView detailView;
        [SerializeField] private PlayerSpecialItemDescriptionView descriptionView;

        private int currentIndex = -1;
        private CompositeDisposable disposables = new ();

        private void Start()
        {
            listView.RebuildAll(model.EnumerateSpecialItems());
            SelectInitial();
            SubscribeEvents();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W)) Move(-1);
            if (Input.GetKeyDown(KeyCode.S)) Move(1);
        }

        private void SubscribeEvents()
        {
            model.OnItemChanged
                .Subscribe(e =>
                {
                    if (e.Item is ISpecialItem si)
                        listView.OnModelChanged(si, e.Amount, e.Removed);
                })
                .AddTo(disposables);
        }
        
        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
        
        /* 以下ヘルパー関数 */

        private void SelectInitial()
        {
            if (listView.Items.Count > 0)
                SetIndex(0);
            else
                SetIndex(-1);
        }

        private void Move(int delta)
        {
            if (listView.Items.Count == 0)
                return;

            int next = Mathf.Clamp(currentIndex + delta, 0, listView.Items.Count - 1);
            if (next != currentIndex)
                SetIndex(next);
        }

        private void SetIndex(int index)
        {
            currentIndex = index;
            listView.SetSelection(index);

            if (index < 0)
            {
                detailView.Clear();
                descriptionView.Clear();
                return;
            }

            var item = listView.Items[index];
            detailView.Show(item);
            descriptionView.ShowDescription(item.GetDescription);
        }
    }
}