using System.Collections.Generic;
using UnityEngine;

namespace EditorUtility
{
    [CreateAssetMenu(fileName = "CullingObjectGroup", menuName = "ScriptableObjects/CullingObjectGroup", order = 1)]
    public class CullingObjectGroup : ScriptableObject
    {
        public List<GameObject> CullingObjects = new List<GameObject>();        
    }
}