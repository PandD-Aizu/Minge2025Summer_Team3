using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.SpecialEventTriggerScript
{
    public class PlayerWarpEvent : MonoBehaviour
    {
        [SerializeField] private Transform warpPoint;
        [SerializeField] private GameObject playerObject;
        [SerializeField] private float delaySeconds = 0f;

        private bool hasPlayed = false;

        private async void OnTriggerEnter(Collider other)
        {
            try
            {
                if (!other.CompareTag("Player") || hasPlayed)
                    return;

                if (warpPoint == null || playerObject == null)
                    return;

                await UniTask.WaitForSeconds(delaySeconds);

                // CharacterController を使っている場合は一時的に無効化して位置を設定
                var controller = playerObject.GetComponent<CharacterController>();
                if (controller != null)
                {
                    controller.enabled = false;
                    playerObject.transform.SetPositionAndRotation(warpPoint.position, warpPoint.rotation);
                    await UniTask.Yield();
                    controller.enabled = true;
                }
                else
                {
                    playerObject.transform.SetPositionAndRotation(warpPoint.position, warpPoint.rotation);
                }

                hasPlayed = true;
            }
            catch (Exception e)
            {
                throw new Exception("[PlayerWarpEvent] プレイヤーのワープ処理中に例外が発生しました。", e);
            }
        }
    }
}