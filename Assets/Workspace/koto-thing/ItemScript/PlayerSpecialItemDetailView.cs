using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Workspace.koto_thing
{
    public class PlayerSpecialItemDetailView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;

        public void Show(ISpecialItem item)
        {
            if (icon)
            {
                icon.sprite = item.GetSprite;
                icon.gameObject.SetActive(icon.sprite != null);
            }
            if (nameText)
                nameText.text = item.GetDisplayName;
            
            gameObject.SetActive(true);
        }

        public void Clear()
        {
            if (icon)
            {
                icon.sprite = null;
                icon.gameObject.SetActive(false); // クリア時は常に非表示
            }
            if (nameText)
                nameText.text = "";

            gameObject.SetActive(false);
        }
    }
}