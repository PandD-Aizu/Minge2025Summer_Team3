using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Minge2025Summer.Scripts.InGame.TimelineScript.ImagePlayableTrack
{
    [Serializable]
    public class ImagePlayableBehaviour : PlayableBehaviour
    {
        [Tooltip("The sprite to display")] 
        public Sprite sprite;
        
        [Tooltip("The color of the image")] 
        public Color color = new Color (0.0f, 0.0f, 0.0f, 0.0f);
        
        [Tooltip("The material to use")]
        public Material material;
    }
}