using System.Collections.Generic;
using UnityEngine;

namespace Test.HybridMapGenerator
{
    // 部屋の種類
    public enum RoomType
    {
        Normal,
    }
    
    // オブジェクトを配置可能な場所の種類
    public enum ObjectPlacementType
    {
        Edge,
        Center,
    }
    
    [CreateAssetMenu(fileName = "NewRoomTemplate", menuName = "Map Generation/Room Template")]
    public class RoomTemplate : ScriptableObject
    {
        [Header("基本設定")] 
        public string roomName = "New Room";
        public RoomType roomType;
        
        [Tooltip("部屋のサイズ(タイル数)")] 
        public GameObject layoutPrefab;
        
        [Header("レイアウトプレファブのサイズ")]
        public Vector2Int size = new Vector2Int(10, 10);
        
        [Header("ドア設定")]
        [Tooltip("ドアを配置可能な壁のタイル座標リスト(部屋の左下を原点とする)")]
        public List<Vector2Int> potentialDoorPositions = new List<Vector2Int>();
    }
}