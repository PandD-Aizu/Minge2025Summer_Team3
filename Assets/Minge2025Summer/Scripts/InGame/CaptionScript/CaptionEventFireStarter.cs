using UnityEngine;

namespace Minge2025Summer.Scripts.InGame
{
    public class CaptionEventFireStarter : MonoBehaviour
    {
        [SerializeField, Tooltip("表示する字幕のテキスト")] 
        private string captionText;

        [SerializeField, Tooltip("キャプションテキストのモデルクラス")]
        private CaptionModel model;
        
        private bool isAlreadyTriggered = false;

        private void Start()
        {
            if (model == null)
                model = FindFirstObjectByType<CaptionModel>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && !isAlreadyTriggered)
            {
                model.RaiseShow(captionText);
                
                isAlreadyTriggered = true;
            }
        }
    }
}