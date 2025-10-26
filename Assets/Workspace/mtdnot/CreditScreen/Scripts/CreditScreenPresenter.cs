using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace CreditScreen
{
    /// <summary>
    /// クレジット画面のロジックを制御するプレゼンタークラス
    /// </summary>
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
        
        /// <summary>
        /// クレジット画面を初期化する
        /// </summary>
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
        
        /// <summary>
        /// クレジットのスクロールが終了したときの処理
        /// </summary>
        private void OnCreditsEnd()
        {
            model.StopScrolling();
            
            if (endDelayCoroutine != null)
            {
                StopCoroutine(endDelayCoroutine);
            }
            
            endDelayCoroutine = StartCoroutine(EndDelayCoroutine());
        }
        
        /// <summary>
        /// クレジット終了後の遅延処理を行うコルーチン
        /// </summary>
        /// <returns>コルーチン</returns>
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
        
        /// <summary>
        /// クレジットを最初から再開する
        /// </summary>
        private void RestartCredits()
        {
            model.ResetScroll();
            view.ResetScrollPosition();
            model.StartScrolling();
        }
        
        /// <summary>
        /// クレジットをスキップする
        /// </summary>
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
        
        /// <summary>
        /// 次のシーンをロードする
        /// </summary>
        private void LoadNextScene()
        {
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
        
        /// <summary>
        /// クレジットデータを設定する
        /// </summary>
        /// <param name="data">設定するクレジットデータ</param>
        public void SetCreditData(CreditData data)
        {
            creditData = data;
            Initialize();
        }
        
        /// <summary>
        /// クレジットのスクロールを一時停止する
        /// </summary>
        public void PauseCredits()
        {
            model?.StopScrolling();
        }
        
        /// <summary>
        /// クレジットのスクロールを再開する
        /// </summary>
        public void ResumeCredits()
        {
            model?.StartScrolling();
        }
    }
}