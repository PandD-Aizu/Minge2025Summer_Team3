using Minge2025Summer.Scripts.InGame.ItemScript.Interface;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ItemScript.Battery
{
    public class BatteryView : MonoBehaviour, IItemView
    {
        [SerializeField] private GameObject parentObject;

        public void Hide()
        {
            parentObject.SetActive(false);
        }
    }
}