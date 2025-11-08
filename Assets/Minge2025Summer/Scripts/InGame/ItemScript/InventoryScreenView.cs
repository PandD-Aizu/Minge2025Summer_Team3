using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ItemScript
{
    public class InventoryScreenView : MonoBehaviour
    {
        public bool IsVisibleInventoryScreen
        {
            get => gameObject.activeSelf;
        }
    }
}