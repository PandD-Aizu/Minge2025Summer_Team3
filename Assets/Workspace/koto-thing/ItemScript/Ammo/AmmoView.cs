using UnityEngine;

namespace Workspace.koto_thing
{
    public class AmmoView : MonoBehaviour, IItemView
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