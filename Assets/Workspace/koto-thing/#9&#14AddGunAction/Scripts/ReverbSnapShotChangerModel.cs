using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;

namespace Acoustics
{
    public class ReverbSnapShotChangerModel : MonoBehaviour
    {
        [Header("RayCastの設定")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float rayDistance = 50.0f;
        [SerializeField] private float checkInterval = 1.0f;

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
                    Debug.Log($"Reverb changed to: {currentReverbMaterialType}");
                }
                
                checkTimer = 0.0f;
            }
        }

        /// <summary>
        /// DEBUG用: Rayの可視化と現在のReverbMaterialの表示
        /// </summary>
        public void DebugRayCast()
        {
            Debug.DrawRay(playerTransform.position, playerTransform.forward * rayDistance, Color.red);
            Debug.DrawRay(playerTransform.position, -playerTransform.forward * rayDistance, Color.red);
            Debug.DrawRay(playerTransform.position, playerTransform.up * rayDistance, Color.red);
            Debug.DrawRay(playerTransform.position, -playerTransform.up * rayDistance, Color.red);
            Debug.DrawRay(playerTransform.position, playerTransform.right * rayDistance, Color.red);
            Debug.DrawRay(playerTransform.position, -playerTransform.right * rayDistance, Color.red);
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
            
            // プレイヤーの前後左右上下にRayを飛ばして、当たったオブジェクトのReverbMaterialを取得する
            Physics.Raycast(playerTransform.position, playerTransform.forward, out RaycastHit hitForward, rayDistance);
            Physics.Raycast(playerTransform.position, -playerTransform.forward, out RaycastHit hitBack, rayDistance);
            Physics.Raycast(playerTransform.position, playerTransform.up, out RaycastHit hitUp, rayDistance);
            Physics.Raycast(playerTransform.position, -playerTransform.up, out RaycastHit hitDown, rayDistance);
            Physics.Raycast(playerTransform.position, playerTransform.right, out RaycastHit hitRight, rayDistance);
            Physics.Raycast(playerTransform.position, -playerTransform.right, out RaycastHit hitLeft, rayDistance);

            // 当たったオブジェクトのReverbMaterialを取得し、リストに追加する
            reverbMaterialTypes.Add(GetMaterialTypeFromHit(hitForward));
            reverbMaterialTypes.Add(GetMaterialTypeFromHit(hitBack));
            reverbMaterialTypes.Add(GetMaterialTypeFromHit(hitUp));
            reverbMaterialTypes.Add(GetMaterialTypeFromHit(hitDown));
            reverbMaterialTypes.Add(GetMaterialTypeFromHit(hitRight));
            reverbMaterialTypes.Add(GetMaterialTypeFromHit(hitLeft));

            // リストの中で最も多く出現したReverbMaterialを現在のReverbMaterialとして設定する
            currentReverbMaterialType = reverbMaterialTypes
                .GroupBy(type => type)
                .OrderByDescending(group => group.Count())
                .Select(group => group.Key)
                .FirstOrDefault();
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