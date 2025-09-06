using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class PortalTrigger : MonoBehaviour
    {
        [SerializeField, Tooltip("対になるポータル")]
        private Transform otherPortal;
        
        [SerializeField, Tooltip("レイヤーとマテリアルを入れ替えるスクリプト")] 
        private WorldLayerSwapper layerSwapper;

        [SerializeField, Tooltip("Playerオブジェクト")]
        private Transform player;

        private static bool isTeleporting = false;

        private bool isPlayerOverlapping = false;
        private Vector3 previousPlayerPosition;

        private async void LateUpdate()
        {
            if (isPlayerOverlapping)
            {
                // プレイヤーの現在位置を取得
                Vector3 currentPlayerPosition = player.position;
                
                // ポータルのローカル空間での位置を計算
                Vector3 lastPosInPortalSpace = transform.InverseTransformPoint(previousPlayerPosition);
                Vector3 currentPosInPortalSpace = transform.InverseTransformPoint(currentPlayerPosition);
                
                // プレイヤーがポータルを通過したかどうかをチェック
                if (lastPosInPortalSpace.z < 0 && currentPosInPortalSpace.z >= 0)
                {
                    await TeleportPlayer();
                }

                // 前回の位置を更新
                previousPlayerPosition = currentPlayerPosition;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !isTeleporting)
            {
                isPlayerOverlapping = true;
                previousPlayerPosition = player.position;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                isPlayerOverlapping = false;
        }

        private UniTask TeleportPlayer()
        {
            isTeleporting = true;
            isPlayerOverlapping = false;
            
            Transform playerTransform = player;
            CharacterController characterController = player.GetComponent<CharacterController>();

            if (characterController != null)
            {
                characterController.enabled = false;
            }

            // プレイヤーの位置と回転をポータルに合わせて変換
            Vector3 playerOffset = transform.InverseTransformPoint(playerTransform.position);
            playerTransform.position = otherPortal.TransformPoint(playerOffset);
            
            Quaternion correctiveRotation = Quaternion.Euler(0, 180, 0);

            // 回転の変換
            Quaternion playerRotation = Quaternion.Inverse(transform.rotation) * playerTransform.rotation;
            playerTransform.rotation = otherPortal.rotation * correctiveRotation * playerRotation;
            
            if (characterController != null)
            {
                characterController.enabled = true;
            }
            
            // レイヤーとマテリアルの入れ替え
            layerSwapper.SwapAllChildLayers();
            Debug.Log("Teleported through portal and swapped layers.");
            return UniTask.WaitForSeconds(0.2f);
        }
    }
}