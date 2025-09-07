using Unity.Cinemachine;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class PlayerCameraModel : MonoBehaviour
    {
        [SerializeField] private CinemachineFollow followComponent;
        [SerializeField] private float normalHeight;
        [SerializeField] private float crouchHeight;
        [SerializeField] private float heightChangeSpeed;

        private float targetHeight = 0.0f;

        public void UpdateCameraHeight()
        {
            var current = followComponent.FollowOffset.y;

            // 距離がある間だけ補間して更新
            if (Mathf.Abs(current - targetHeight) > 0.001f)
            {
                var newHeight = Mathf.MoveTowards(current, targetHeight, heightChangeSpeed * Time.deltaTime);
                var offset = followComponent.FollowOffset;
                offset.y = newHeight;
                followComponent.FollowOffset = offset;
            }
        }
        
        public void SetCameraHeight(bool isCrouching)
        {
            targetHeight = isCrouching ? crouchHeight : normalHeight;
        }
    }
}