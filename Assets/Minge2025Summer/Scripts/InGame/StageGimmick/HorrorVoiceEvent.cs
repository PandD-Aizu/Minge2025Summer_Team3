using FMODUnity;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.StageGimmick
{
    public class HorrorVoiceEvent : MonoBehaviour
    {
        [SerializeField] private StudioEventEmitter eventEmitter;

        private bool hasTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || hasTriggered)
                return;

            // イベントをプレイヤーオブジェクトの後ろに移動して再生する
            eventEmitter.gameObject.transform.position = other.transform.position;
            eventEmitter.gameObject.transform.position += Vector3.back;
            
            eventEmitter.Play();
            hasTriggered = true;
        }
    }
}