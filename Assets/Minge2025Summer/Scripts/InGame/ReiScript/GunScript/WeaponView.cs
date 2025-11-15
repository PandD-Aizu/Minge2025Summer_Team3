using System;
using Cysharp.Threading.Tasks;
using Minge2025Summer.Scripts.InGame.GunScript.Interface;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

namespace Minge2025Summer.Scripts.InGame.ReiScript.GunScript
{
    public class WeaponView : MonoBehaviour
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

        public Light MuzzleFlashLight { get => muzzleFlashLight; set => muzzleFlashLight = value; }
        
        /// <summary>
        /// 現在の弾薬数を表示する
        /// </summary>
        /// <param name="currentAmmo">現在マガジンに入っている弾薬数</param>
        /// <param name="maxAmmo">現在持っているすべての弾薬</param>
        /// <param name="magCapacity">マガジンの容量</param>
        public void UpdateAmmoText(int currentAmmo, int maxAmmo, int magCapacity)
        {
            if (ammoText == null)
                return;

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
        /// <param name="weapon">現在装備している銃（IWeapon）</param>
        /// <param name="isAiming">覗き込み状態か</param>
        public void UpdateReticle(IWeapon weapon, bool isAiming)
        {
            if (dotReticle == null || circleReticle == null)
                return;

            if (weapon == null)
            {
                dotReticle.SetActive(false);
                circleReticle.SetActive(false);
                return;
            }
            
            // 反動込みの実効拡散角
            float finalSpreadDeg = weapon.GetFinalSpreadAngleDeg();
            const float dotMaxSpreadDeg = 0.2f; // これ以下ならドット表示

            // ドット/サークルの切替
            bool showDot = isAiming && finalSpreadDeg <= dotMaxSpreadDeg;
            dotReticle.SetActive(showDot);
            circleReticle.SetActive(!showDot);

            if (showDot)
            {
                var rt = dotReticle.GetComponent<RectTransform>();
                if (rt != null)
                    rt.sizeDelta = new Vector2(dotSizePx, dotSizePx);
                return;
            }

            // サークルのサイズ計算（最終拡散角）
            float spreadRad = finalSpreadDeg * Mathf.Deg2Rad;
            float fovRad = Camera.main != null ? Camera.main.fieldOfView * Mathf.Deg2Rad : (60f * Mathf.Deg2Rad);
            float radiusPx = Mathf.Tan(spreadRad) / Mathf.Tan(fovRad * 0.5f) * (Screen.height * 0.5f);
            float diameterPx = Mathf.Max(minCirclePx, radiusPx * 2.0f);
            
            var crt = circleReticle.GetComponent<RectTransform>();
            if (crt != null)
                crt.sizeDelta = new Vector2(diameterPx, diameterPx);
        }
        
        /// <summary>
        /// マズルフラッシュのVFXを再生する
        /// </summary>
        public void PlayMuzzleFlash()
        {
            if (muzzleFlashVFX == null)
                return;
            try
            {
                muzzleFlashVFX.SendEvent("OnPlay");
            }
            catch
            {
                // 保護: VFX 設定異常でも落とさない
            }
        }

        /// <summary>
        /// マズルフラッシュのライトを一瞬だけ点灯させる
        /// </summary>
        public async UniTask PlayMuzzleFlashLight()
        {
            try
            {
                if (muzzleFlashLight == null)
                {
                    muzzleFlashLight = GetComponentInChildren<Light>(true);
                    if (muzzleFlashLight == null)
                        return;
                }

                // 保存して復元
                bool originalEnabled = muzzleFlashLight.enabled;
                float originalIntensity = muzzleFlashLight.intensity;
                float originalRange = muzzleFlashLight.range;
                bool originalActive = muzzleFlashLight.gameObject.activeSelf;

                // 非アクティブなら一時的に有効化
                if (!originalActive)
                    muzzleFlashLight.gameObject.SetActive(true);

                // ベイク専用ではないか確認（サイレント）
                #if UNITY_2018_1_OR_NEWER
                try
                {
                    var bakeType = muzzleFlashLight.lightmapBakeType;
                    if (bakeType != LightmapBakeType.Realtime)
                    {
                        // silent: 動作に影響するがログは削除済み
                    }
                }
                catch { }
                #endif

                if (muzzleFlashLight.intensity <= 0f)
                    muzzleFlashLight.intensity = Mathf.Max(1f, originalIntensity);
                if (muzzleFlashLight.range <= 0f)
                    muzzleFlashLight.range = Mathf.Max(1f, originalRange);

                muzzleFlashLight.enabled = true;

                await UniTask.Delay(80);

                // 復元
                muzzleFlashLight.enabled = originalEnabled;
                muzzleFlashLight.intensity = originalIntensity;
                muzzleFlashLight.range = originalRange;
                if (!originalActive)
                    muzzleFlashLight.gameObject.SetActive(false);
            }
            catch
            {
                // 保護: 例外は無視して動作継続
            }
        }
    }
}