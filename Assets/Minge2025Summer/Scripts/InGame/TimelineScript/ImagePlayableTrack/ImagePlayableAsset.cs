using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Minge2025Summer.Scripts.InGame.TimelineScript.ImagePlayableTrack
{
    [Serializable]
    public class ImagePlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        [NotKeyable]
        public ImagePlayableBehaviour template = new ImagePlayableBehaviour();

        public ClipCaps clipCaps => ClipCaps.Blending;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<ImagePlayableBehaviour>.Create(graph, template);
        }
    }
}