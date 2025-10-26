using System;
using System.Collections.Generic;
using UnityEngine;

namespace CreditScreen
{
    [CreateAssetMenu(fileName = "CreditData", menuName = "CreditScreen/CreditData")]
    public class CreditData : ScriptableObject
    {
        [Serializable]
        public class CreditSection
        {
            public string sectionTitle;
            public List<string> credits;
        }

        [Header("Credit Information")]
        public string gameTitle = "";
        
        [Header("Credit Sections")]
        public List<CreditSection> creditSections = new List<CreditSection>();
        
        [Header("Scroll Settings")]
        public float scrollSpeed = 50f;
        public float delayBeforeStart = 2f;
        public float delayAfterEnd = 3f;
    }
}