using Minge2025Summer.Scripts.InGame.ItemScript.Interface;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ItemScript.Pill
{
    public class PillView : MonoBehaviour, IItemView
    {
        [SerializeField] private GameObject parentObject;

        /// <summary>
        /// Itemを非表示にする
        /// </summary>
        public void Hide()
        {
            parentObject.SetActive(false);
        }
    }
}