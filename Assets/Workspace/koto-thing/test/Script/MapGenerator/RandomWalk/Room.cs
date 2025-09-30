using System.Collections.Generic;
using UnityEngine;

namespace Test.RandomWalk
{
    public class Room : MonoBehaviour
    {
        [SerializeField, Tooltip("部屋のサイズ(グリッド単位)")]
        public Vector2Int size = new  Vector2Int(5, 5);
        
        [SerializeField, Tooltip("出入り口のローカル座標リスト(部屋の左下を原点とする)")]
        public List<Vector2Int> doorPositions = new List<Vector2Int>();
    }
}