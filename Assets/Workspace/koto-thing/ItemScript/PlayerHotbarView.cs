using UnityEngine;
using UnityEngine.UI;

namespace Workspace.koto_thing
{
    public class PlayerHotbarView : MonoBehaviour
    {
        [SerializeField] private Transform slotParent;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color usedColor = Color.gray;
        [SerializeField] private float flashDuration = 0.15f;

        private Image[] slotImages;

        public void UpdateSlot(int index, IItem item)
        {
            if (!Valid(index))
                return;

            var image = slotImages[index];
            if (item == null)
            {
                image.sprite = null;
                image.color = normalColor;
            }
            else
            {
                image.sprite = item.GetSprite;
                image.color = normalColor;
            }
        }

        public void PlayUseFeedback(int index, bool consumed)
        {
            if (!Valid(index))
                return;

            var image = slotImages[index];
            if (consumed)
            {
                image.color = usedColor;
                CancelInvoke(nameof(ResetColor));
                Invoke(nameof(ResetColor), flashDuration);
            }

            void ResetColor()
            {
                if (image != null)
                    image.color = normalColor;
            }
        }

        private bool Valid(int i) => slotImages != null && i >= 0 && i < slotImages.Length;
    }
}