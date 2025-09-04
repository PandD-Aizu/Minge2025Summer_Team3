using System.Collections;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

namespace Workspace.koto_thing
{
    public class GunView : MonoBehaviour
    {
        [Header("弾薬数表示用テキスト")]
        [SerializeField] private TextMeshProUGUI ammoText;

        [Header("照準")]
        [SerializeField] private GameObject dotReticle;
        [SerializeField] private GameObject circleReticle;
        [SerializeField] private float dotSizePx = 6.0f;
        [SerializeField] private float minCirclePx = 12.0f;

        [Header("マズルフラッシュ")] 
        [SerializeField] private VisualEffect muzzleFlashVFX;
        [SerializeField] private Light muzzleFlashLight;
        
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

        /// <summary>
        /// 照準の描画を更新する
        /// </summary>
        /// <param name="gun">現在装備している銃</param>
        /// <param name="isAiming">覗き込み状態か</param>
        public void UpdateReticle(IGun gun, bool isAiming)
        {
            if (gun == null)
            {
                dotReticle.SetActive(false);
                circleReticle.SetActive(false);
                return;
            }
            
            // 精度が99%以上ならドット、そうでなければサークルを表示
            float accuracy = gun.CurrentAccuracy();
            if (isAiming && accuracy >= 0.99f)
            {
                dotReticle.SetActive(true);
                circleReticle.SetActive(false);
                
                if (dotReticle)
                    dotReticle.GetComponent<RectTransform>().sizeDelta = new Vector2(dotSizePx, dotSizePx);
            }
            else
            {
                dotReticle.SetActive(false);
                circleReticle.SetActive(true);

                float spreadDegree = isAiming ? gun.GetCurrentSpreadAngleDeg() : gun.GetHipFireSpreadAngleDeg();
                
                float spreadRad = spreadDegree * Mathf.Deg2Rad;
                float fovRad = Camera.main.fieldOfView * Mathf.Deg2Rad;
                float radiusPx = Mathf.Tan(spreadRad) / Mathf.Tan(fovRad * 0.5f) * (Screen.height * 0.5f);
                float diameterPx = Mathf.Max(minCirclePx, radiusPx * 2.0f);
                
                if (circleReticle)
                    circleReticle.GetComponent<RectTransform>().sizeDelta = new Vector2(diameterPx, diameterPx);
            }
        }
        
        /// <summary>
        /// マズルフラッシュのVFXを再生する
        /// </summary>
        public void PlayMuzzleFlash()
        {
            muzzleFlashVFX.SendEvent("OnPlay");
        }

        /// <summary>
        /// マズルフラッシュのライトを一瞬だけ点灯させる
        /// </summary>
        /// <returns></returns>
        public async UniTask PlayMuzzleFlashLight()
        {
            muzzleFlashLight.enabled = true;
            await UniTask.Delay(50);
            muzzleFlashLight.enabled = false;
        }
    }
}