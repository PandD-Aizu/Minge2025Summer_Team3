using Minge2025Summer.Scripts.InGame.ItemScript.Interface;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Minge2025Summer.Scripts.InGame.ItemScript
{
    public class SpecialItemListEntry : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField, Tooltip("選択時のテキストカラー")] private Color selectedColor = Color.red;

        private ISpecialItem bound;
        private Color originalColor;
        private bool initializedColor;

        public ISpecialItem BoundItem => bound;
        public GameObject SelectableObject => gameObject; // 外部が初期選択で使用

        public void Bind(ISpecialItem item)
        {
            bound = item;
            if (nameText)
            {
                nameText.text = item.GetDisplayName;
                if (!initializedColor)
                {
                    originalColor = nameText.color;
                    initializedColor = true;
                }
                nameText.color = originalColor;
            }
        }

        public void SetHighlighted(bool on)
        {
            if (nameText)
                nameText.color = on ? selectedColor : originalColor;
        }

        public void OnSelect(BaseEventData eventData)
        {
            SetHighlighted(true);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            SetHighlighted(false);
        }
    }
}