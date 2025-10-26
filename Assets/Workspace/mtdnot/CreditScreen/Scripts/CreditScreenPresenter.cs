using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace CreditScreen
{
    public class CreditScreenPresenter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CreditScreenView view;
        [SerializeField] private CreditData creditData;
        
        [Header("Scene Transition")]
        [SerializeField] private string nextSceneName = "";
        [SerializeField] private bool loopCredits = false;
        [SerializeField] private bool allowSkip = true;
        [SerializeField] private KeyCode skipKey = KeyCode.Escape;
        
        private CreditScreenModel model;
        private Coroutine endDelayCoroutine;
        
        private void Awake()
        {
            if (view == null)
            {
                view = GetComponent<CreditScreenView>();
                if (view == null)
                {
                    view = GetComponentInChildren<CreditScreenView>();
                }
            }
            
            model = new CreditScreenModel();
        }
        
        private void Start()
        {
            Initialize();
        }
        
        private void Initialize()
        {
            if (creditData == null)
            {
                Debug.LogError("CreditData is not assigned!");
                return;
            }
            
            model.Initialize(creditData);
            view.SetCreditText(creditData);
            view.ResetScrollPosition();
            view.EnableInteraction(false);
            
            model.StartScrolling();
        }
        
        private void Update()
        {
            if (model == null || !model.IsScrolling) return;
            
            model.UpdateScrollPosition(Time.deltaTime);
            
            float maxScroll = view.GetMaxScrollPosition();
            if (maxScroll > 0)
            {
                float normalizedPosition = model.CurrentScrollPosition / maxScroll;
                view.UpdateScrollPosition(normalizedPosition);
                
                if (model.HasReachedEnd(maxScroll))
                {
                    OnCreditsEnd();
                }
            }
            
            if (allowSkip && Input.GetKeyDown(skipKey))
            {
                SkipCredits();
            }
        }
        
        private void OnCreditsEnd()
        {
            model.StopScrolling();
            
            if (endDelayCoroutine != null)
            {
                StopCoroutine(endDelayCoroutine);
            }
            
            endDelayCoroutine = StartCoroutine(EndDelayCoroutine());
        }
        
        private IEnumerator EndDelayCoroutine()
        {
            yield return new WaitForSeconds(creditData.delayAfterEnd);
            
            if (loopCredits)
            {
                RestartCredits();
            }
            else if (!string.IsNullOrEmpty(nextSceneName))
            {
                LoadNextScene();
            }
        }
        
        private void RestartCredits()
        {
            model.ResetScroll();
            view.ResetScrollPosition();
            model.StartScrolling();
        }
        
        private void SkipCredits()
        {
            if (endDelayCoroutine != null)
            {
                StopCoroutine(endDelayCoroutine);
            }
            
            model.StopScrolling();
            
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                LoadNextScene();
            }
            else if (loopCredits)
            {
                RestartCredits();
            }
        }
        
        private void LoadNextScene()
        {
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
        
        public void SetCreditData(CreditData data)
        {
            creditData = data;
            Initialize();
        }
        
        public void PauseCredits()
        {
            model?.StopScrolling();
        }
        
        public void ResumeCredits()
        {
            model?.StartScrolling();
        }
    }
}