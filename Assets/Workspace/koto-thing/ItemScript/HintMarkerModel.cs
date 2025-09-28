using System;
using UniRx;
using UnityEngine;

public class HintMarkerModel : MonoBehaviour
{
    [Header("ヒントが透明・不透明になる最大・最小距離")] 
    [SerializeField] private float maxDistance;
    [SerializeField] private float minDistance;
    
    private Transform playerTransform;
    
    public float GetMinDistance => minDistance;
    public float GetMaxDistance => maxDistance;
    public Transform PlayerTransform { get => playerTransform; set => playerTransform = value; }
}
