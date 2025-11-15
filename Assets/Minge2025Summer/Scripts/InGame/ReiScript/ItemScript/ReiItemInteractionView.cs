using TMPro;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ReiScript.ItemScript
{
    public class ReiItemInteractionView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI systemText;

        public void Notify(string itemName)
        {
            systemText.text = $"{itemName} を入手した。";
        }
    }
}