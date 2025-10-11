using UnityEngine;

namespace Minge2025Summer.Main.InGame
{
    public class TutorialEventFireStarter : MonoBehaviour
    {
        [SerializeField, Tooltip("表示するチュートリアルの種類")] private TutorialType tutorialType;
        [SerializeField, Tooltip("チュートリアルのモデルクラス")] private TutorialModel model;
        
        private bool isAlreadyTriggered = false;
        
        private void Start()
        {
            if (model == null)
                model = FindFirstObjectByType<TutorialModel>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !isAlreadyTriggered)
            {
                model.RaiseShow(tutorialType);
                isAlreadyTriggered = true;
            }
        }
    }
}