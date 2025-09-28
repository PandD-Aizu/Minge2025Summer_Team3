using UnityEngine;

namespace Workspace.koto_thing.Battery
{
    public class BatteryView : MonoBehaviour
    {
        [SerializeField] private GameObject parentObject;

        public void Hide()
        {
            parentObject.SetActive(false);
        }
    }
}