using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace Minge2025Summer.Scripts.InGame.TimelineScript.ImagePlayableTrack
{
    [TrackColor(1f, 0f, 0f)]
    [TrackClipType(typeof(ImagePlayableAsset))]
    [TrackBindingType(typeof(Image))]
    public class ImageTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<ImageTrackMixerBehaviour>.Create(graph, inputCount);
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            Image trackBinding = director.GetGenericBinding(this) as Image;

            // If there is no binding for this track, nothing to gather
            if (trackBinding == null)
                return;

            // Register the serialized properties we intend to animate on the bound Image
            driver.AddFromName<Image>(trackBinding.gameObject, "m_Sprite");
            driver.AddFromName<Image>(trackBinding.gameObject, "m_Color");
            driver.AddFromName<Image>(trackBinding.gameObject, "m_Material");
        }
    }
}