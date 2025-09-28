using UnityEngine;

namespace Workspace.koto_thing
{
    public class BatteryLevelView : MonoBehaviour
    {
        [SerializeField] private Light flashLight;
        
        public Light GetFlashLight => flashLight;
    }
}