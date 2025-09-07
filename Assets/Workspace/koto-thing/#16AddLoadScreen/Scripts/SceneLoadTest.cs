using UnityEngine;

namespace Workspace.koto_thing
{
    public class SceneLoadTest : MonoBehaviour
    {
        public void LoadSceneViaButton()
        {
            SceneController.LoadSceneAsync("LoadScreenTest");
        }
    }
}