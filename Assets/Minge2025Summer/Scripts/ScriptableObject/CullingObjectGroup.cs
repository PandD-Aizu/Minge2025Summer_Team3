using System.Collections.Generic;
using UnityEngine;

namespace Minge2025Summer.Scripts.ScriptableObject
{
    [CreateAssetMenu(fileName = "CullingObjectGroup", menuName = "ScriptableObjects/CullingObjectGroup", order = 1)]
    public class CullingObjectGroup : UnityEngine.ScriptableObject
    {
        public List<GameObject> CullingObjects = new List<GameObject>();        
    }
}