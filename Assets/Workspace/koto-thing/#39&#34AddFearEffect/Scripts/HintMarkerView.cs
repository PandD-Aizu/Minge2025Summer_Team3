using UnityEngine;

namespace Workspace.koto_thing
{
    public class HintMarkerView : MonoBehaviour
    {
        [Header("表示するスプライトレンダラー")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        /// <summary>
        /// 表示するかどうかを切り替える
        /// </summary>
        /// <param name="isVisible"></param>
        public void SwitchVisibility(bool isVisible)
        {
            spriteRenderer.enabled = isVisible;
        }
        
        /// <summary>
        /// カメラの方向へ回転する
        /// </summary>
        public void RotateTowardsCamera()
        {
            if (Camera.main == null) 
                return;
            
            Vector3 direction = Camera.main.transform.position - transform.position;
            
            // 零ベクトルを避けるためのチェック
            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5.0f);
            }
        }

        /// <summary>
        /// プレイヤーからの距離に基づいて透明度を更新する
        /// </summary>
        /// <param name="playerTransform">プレイヤーのTransform</param>
        /// <param name="maxDistance">最大距離</param>
        /// <param name="minDistance">最小距離</param>
        public void UpdateAlphaByDistance(Transform playerTransform, float maxDistance, float minDistance = 0.0f)
        {
            if (playerTransform == null || spriteRenderer == null)
                return;

            // オブジェクトとプレイヤーとの距離を計算
            float distance = Vector3.Distance(transform.position, playerTransform.position);

            // 距離を0から1の範囲に正規化する
            // minDistanceで0、maxDistanceで1
            float normalizedDistance = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));
            
            Debug.Log("Distance: " + distance + ", Normalized: " + normalizedDistance);
        
            // 距離が遠いほど透明度を下げるため、正規化された値を1から引く
            float newAlpha = 1.0f - normalizedDistance;

            // SpriteRendererの色を取得し、アルファ値だけを更新する
            Color currentColor = spriteRenderer.color;
            currentColor.a = newAlpha;
            spriteRenderer.color = currentColor;
        }
    }
}