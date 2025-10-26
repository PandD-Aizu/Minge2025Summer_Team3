using UnityEngine;

namespace CreditScreen
{
    public class CreditScreenModel
    {
        private CreditData creditData;
        private float currentScrollPosition;
        private bool isScrolling;
        private float scrollTimer;
        
        public CreditData CreditData => creditData;
        public float CurrentScrollPosition => currentScrollPosition;
        public bool IsScrolling => isScrolling;
        public float ScrollTimer => scrollTimer;
        
        public void Initialize(CreditData data)
        {
            creditData = data;
            currentScrollPosition = 0f;
            isScrolling = false;
            scrollTimer = 0f;
        }
        
        public void StartScrolling()
        {
            isScrolling = true;
            scrollTimer = 0f;
        }
        
        public void StopScrolling()
        {
            isScrolling = false;
        }
        
        public void UpdateScrollPosition(float deltaTime)
        {
            if (!isScrolling) return;
            
            scrollTimer += deltaTime;
            
            if (scrollTimer > creditData.delayBeforeStart)
            {
                currentScrollPosition += creditData.scrollSpeed * deltaTime;
            }
        }
        
        public void ResetScroll()
        {
            currentScrollPosition = 0f;
            scrollTimer = 0f;
        }
        
        public bool HasReachedEnd(float maxScrollPosition)
        {
            return currentScrollPosition >= maxScrollPosition;
        }
    }
}