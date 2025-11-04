using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.TimelineScript
{
    public class TextTyper : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMeshPro;

        [Header("タイピング速度設定")] 
        [SerializeField]
        private float typeDelay = 1.0f;

        [Header("消滅設定")] 
        [SerializeField] private float destroyDelay = 1.0f;

        private string fullText;

        public void OnEnable()
        {
            StartTyping();
        }

        public void StartTyping(string newText = null)
        {
            if (newText != null)
                textMeshPro.text = newText;

            fullText = textMeshPro.text;
            
            StopAllCoroutines();
            TypeTextAsync().Forget();
        }
        
        private async UniTask TypeTextAsync()
        {
            textMeshPro.text = string.Empty;
            for (int i = 0; i <= fullText.Length; i++)
            {
                textMeshPro.text = fullText.Substring(0, i);
                await UniTask.Delay(System.TimeSpan.FromSeconds(typeDelay));
            }

            await UniTask.Delay(System.TimeSpan.FromSeconds(destroyDelay));
            gameObject.SetActive(false);
        }
    }
}