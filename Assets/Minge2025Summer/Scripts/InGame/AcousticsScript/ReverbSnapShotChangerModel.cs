using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.AcousticsScript
{
    public class ReverbSnapShotChangerModel : MonoBehaviour
    {
        [Header("RayCastの設定")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float rayDistance = 50.0f;
        [SerializeField] private float checkInterval = 1.0f;

        [Header("球状サンプリング")]
        [SerializeField, Range(4, 64)] private int horizontalRays = 12;
        [SerializeField, Range(2, 32)] private int verticalRays = 6;

        [Header("Reverbの設定")]
        [SerializeField] private ReverbMaterialType currentReverbMaterialType;
        [SerializeField] private List<ReverbMaterialType> reverbMaterialTypes = new ();
        
        [Header("SnapShotの設定")]
        [SerializeField] private List<StudioEventEmitter> reverbSnapShots = new ();

        private float checkTimer = 0.0f;
        private ReverbMaterialType previousReverbMaterialType = ReverbMaterialType.NONE;

        /// <summary>
        /// 環境をチェックし、変更があった場合のみSnapShotを切り替える
        /// </summary>
        public void UpdateReverbEnvironment()
        {
            checkTimer += Time.deltaTime;

            if (checkTimer >= checkInterval)
            {
                CheckEnvironment();
                
                if (currentReverbMaterialType != previousReverbMaterialType)
                {
                    ChangeSnapShot();
                    previousReverbMaterialType = currentReverbMaterialType;
                }
                
                checkTimer = 0.0f;
            }
        }

        /// <summary>
        /// DEBUG用: Rayの可視化と現在のReverbMaterialの表示
        /// </summary>
        public void DebugRayCast()
        {
            if (playerTransform == null)
                return;

            var dirs = GetRayDirections();
            foreach (var dir in dirs)
            {
                Debug.DrawRay(playerTransform.position, dir * rayDistance, Color.red);
            }
        }
        
        /* ---ヘルパー関数--- */
        private ReverbMaterialType GetMaterialTypeFromHit(RaycastHit hit)
        {
            return hit.collider?.GetComponent<ReverbMaterial>()?.GetReverbMaterialType ?? ReverbMaterialType.NONE;
        }
        
                /// <summary>
        /// 現在の環境をチェックし、最も多く存在するReverbMaterialをcurrentReverbMaterialTypeに設定する
        /// </summary>
        private void CheckEnvironment()
        {
            reverbMaterialTypes.Clear();

            if (playerTransform == null)
                return;

            var dirs = GetRayDirections();
            foreach (var dir in dirs)
            {
                Physics.Raycast(playerTransform.position, dir, out RaycastHit hit, rayDistance);
                reverbMaterialTypes.Add(GetMaterialTypeFromHit(hit));
            }

            currentReverbMaterialType = reverbMaterialTypes
                .GroupBy(type => type)
                .OrderByDescending(group => group.Count())
                .Select(group => group.Key)
                .FirstOrDefault();
        }
             
        /// <summary>
        /// 球状に均等分布したRayの方向リストを取得する
        /// </summary>
        /// <returns>レイの方向</returns>
        private List<Vector3> GetRayDirections()
        {
            var dirs = new List<Vector3>();
            if (playerTransform == null)
                return dirs;

            // 緯度ループ: -90（下）から +90（上）まで均等に
            for (int v = 0; v < Mathf.Max(1, verticalRays); v++)
            {
                float t = verticalRays == 1 ? 0.5f : (float)v / (verticalRays - 1); // 0..1
                float lat = Mathf.Lerp(-90f, 90f, t); // 緯度（ピッチ）

                for (int h = 0; h < Mathf.Max(1, horizontalRays); h++)
                {
                    float lon = 360f * h / Mathf.Max(1, horizontalRays); // 経度（ヨー）
                    // ローカル前方を基準にピッチ・ヨーを適用
                    var localDir = Quaternion.Euler(lat, lon, 0f) * Vector3.forward;
                    var worldDir = playerTransform.TransformDirection(localDir).normalized;
                    dirs.Add(worldDir);
                }
            }

            return dirs;
        }

        /// <summary>
        /// 現在のReverbMaterialに応じたSnapShotに切り替える
        /// </summary>
        private void ChangeSnapShot()
        {
            switch (currentReverbMaterialType)
            {
                case ReverbMaterialType.NONE:
                    reverbSnapShots.ForEach(element => element.Stop());
                    reverbSnapShots.Find(element => String.Compare("Reverb_NONE", element.name) == 0)?.Play();
                    break;
                
                case ReverbMaterialType.WOOD:
                    reverbSnapShots.ForEach(element => element.Stop());
                    reverbSnapShots.Find(element => String.Compare("Reverb_WOOD", element.name) == 0)?.Play();
                    break;
                
                case ReverbMaterialType.TUNNEL_TYPE0:
                    reverbSnapShots.ForEach(element => element.Stop());
                    reverbSnapShots.Find(element => String.Compare("Reverb_TUNNEL_TYPE0", element.name) == 0)?.Play();
                    break;
                
                case ReverbMaterialType.TUNNEL_TYPE1:
                    reverbSnapShots.ForEach(element => element.Stop());
                    reverbSnapShots.Find(element => String.Compare("Reverb_TUNNEL_TYPE1", element.name) == 0)?.Play();
                    break;
            }
        }
    }
}