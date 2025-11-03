using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Minge2025Summer.Scripts.InGame.SpecialEventTriggerScript
{
    public class TimelineStartEvent : MonoBehaviour
    {
        [SerializeField, Tooltip("再生対象")]
        private PlayableDirector director;

        [SerializeField, Tooltip("再生時にアセット上書きするか")]
        private TimelineAsset overrideAsset;

        [SerializeField, Tooltip("一度だけ再生するかどうか")]
        private bool playOnce = true;

        private bool hasPlayed = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            if (playOnce && hasPlayed)
                return;

            if (director == null)
            {
                Debug.LogError("PlayableDirectorが設定されていません。");
                return;
            }

            if (overrideAsset != null && director.playableAsset != overrideAsset)
                director.playableAsset = overrideAsset;

            director.time = 0;
            director.Play();

            hasPlayed = true;
            if (playOnce)
                enabled = false;
        }
    }
}