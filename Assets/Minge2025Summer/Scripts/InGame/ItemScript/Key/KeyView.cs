using Minge2025Summer.Scripts.InGame.ItemScript.Interface;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.ItemScript.Key
{
    public class KeyView : MonoBehaviour, IItemView
    {
        [SerializeField] private GameObject parentObject;

        /// <summary>
        /// オブジェクトを非表示にする
        /// </summary>
        public void Hide()
        {
            parentObject.SetActive(false);
        }
    }
}