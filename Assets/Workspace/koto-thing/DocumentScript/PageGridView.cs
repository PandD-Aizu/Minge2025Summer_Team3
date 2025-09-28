using UnityEngine;
using UnityEngine.UI;

namespace Workspace.koto_thing
{
    public class PageGridView : MonoBehaviour
    {
        [SerializeField, Tooltip("グリッド画像")] private Image gridImage;
        [SerializeField, Tooltip("フレーム画像")] private Image frameImage;
        
        public Image GetGridImage => gridImage;
        public Image GetFrameImage => frameImage;
    }
}