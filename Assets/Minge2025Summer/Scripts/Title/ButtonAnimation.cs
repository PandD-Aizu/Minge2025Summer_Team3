using UnityEngine;

namespace Title
{
    public class ButtonAnimation : MonoBehaviour
    {
        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// IsHoveredパラメータをtrueに設定
        /// </summary>
        public void OnPointerEnter()
        {
            if (animator != null)
                animator.SetBool("IsHovering", true);
        }

        /// <summary>
        /// IsHoveredパラメータをfalseに設定
        /// </summary>
        public void OnPointerExit()
        {
            if (animator != null)
                animator.SetBool("IsHovering", false);
        }
    }
}