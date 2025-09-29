using UnityEngine;

namespace Workspace.koto_thing
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