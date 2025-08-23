using TMPro;
using UnityEngine;

namespace Workspace.koto_thing
{
    public class GunView : MonoBehaviour
    {
        [Header("依存関係")]
        [SerializeField] private TextMeshProUGUI ammoText;

        /// <summary>
        /// 現在の弾薬数を表示する
        /// </summary>
        /// <param name="currentAmmo">現在マガジンに入っている弾薬数</param>
        /// <param name="maxAmmo">現在持っているすべての弾薬</param>
        /// <param name="magCapacity">マガジンの容量</param>
        public void UpdateAmmoText(int currentAmmo, int maxAmmo, int magCapacity)
        {
            if (currentAmmo == 0)
            {
                ammoText.text = $"<color=red>{currentAmmo}</color> / {maxAmmo}";
            }
            else if (currentAmmo == magCapacity)
            {
                ammoText.text = $"<color=green>{currentAmmo}</color> / {maxAmmo}";
            }
            else
            {
                ammoText.text = $"{currentAmmo} / {maxAmmo}";
            }
        }
    }
}