using UnityEngine;

namespace Workspace.koto_thing
{
    public class PillView : MonoBehaviour
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