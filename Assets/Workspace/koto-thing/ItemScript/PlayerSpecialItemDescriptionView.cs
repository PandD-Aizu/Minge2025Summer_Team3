using TMPro;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerSpecialItemDescriptionView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI descriptionText;

        public void ShowDescription(string text)
        {
            if (descriptionText)
                descriptionText.text = text;
        }

        public void Clear()
        {
            if (descriptionText)
                descriptionText.text = "";
        }
    }
}