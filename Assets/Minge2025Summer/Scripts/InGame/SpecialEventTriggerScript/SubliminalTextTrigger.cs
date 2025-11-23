using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.SpecialEventTriggerScript
{
    public class SubliminalTextTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject subliminalEffectScreen;
        [SerializeField] private TextMeshProUGUI subliminalText;
        [SerializeField] private string[] textToDisplay;
        [SerializeField] private float displayDuration = 0.5f;

        private bool isTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            if (isTriggered)
                return;

            isTriggered = true;

            if (subliminalEffectScreen != null)
                subliminalEffectScreen.SetActive(true);

            DisplaySubliminalTextSequence();
        }

        private async UniTaskVoid DisplaySubliminalTextSequence()
        {
            if (subliminalText == null || textToDisplay == null || textToDisplay.Length == 0)
            {
                if (subliminalEffectScreen != null)
                    subliminalEffectScreen.SetActive(false);
                return;
            }

            foreach (var msg in textToDisplay)
            {
                if (string.IsNullOrEmpty(msg))
                    continue;

                subliminalText.text = msg;
                subliminalText.gameObject.SetActive(true);

                await UniTask.Delay((int)(displayDuration * 1000));

                subliminalText.gameObject.SetActive(false);
            }

            if (subliminalEffectScreen != null)
                subliminalEffectScreen.SetActive(false);
        }
    }
}