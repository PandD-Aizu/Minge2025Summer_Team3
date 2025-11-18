using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Minge2025Summer.Scripts.InGame.AcousticsScript
{
    public class VolumetricAudioOcclusion : MonoBehaviour
    {
        [Header("FMOD Settings")] 
        [SerializeField] private StudioEventEmitter emitter;
        [SerializeField, Tooltip("FMOD Studioで設定したパラメータ名")] private string parameterName = "Occlusion";

        [Header("Raycast Settings")] 
        [SerializeField, Tooltip("音を遮るレイヤー")] private LayerMask layerMask;
        
        [SerializeField, Range(0.1f, 5.0f), Tooltip("音源の大きさ")] private float sourceSize = 1.0f;
        
        [SerializeField, Tooltip("毎フレーム飛ばすレイの本数")] private int rayCount = 10;
        
        [Header("Smoothing Settings")]
        [SerializeField, Tooltip("パラメータ変化のスムージング係数")] private float smoothing = 10.0f;

        [Header("Material Weighting Settings")] 
        [SerializeField, Tooltip("Tagごとの遮蔽重み付け設定")] private List<OcclusionMaterial> materials;

        private Dictionary<string, float> materialDictionary;

        [Serializable]
        public struct OcclusionMaterial
        {
            public string tag;
            [Range(0.0f, 1.0f)] public float weight;
        }

        private Transform listenerTransform;
        private float currentOcclusion = 0.0f;
        private float targetOcclusion = 0.0f;

        private Vector3[] randomOffsets;

        private void Start()
        {
            randomOffsets = new Vector3[rayCount];
            UpdateRandomOffsets();
            
            materialDictionary = new Dictionary<string, float>();
            foreach (var material in materials)
            {
                if (!materialDictionary.ContainsKey(material.tag))
                {
                    materialDictionary.Add(material.tag, material.weight);
                }
            }
        }

        private void Update()
        {
            if (emitter == null || !emitter.IsPlaying())
            {
                Debug.LogWarning("VolumetricAudioOcclusion: Emitter is null or not playing.", this);
                return;
            }

            if (listenerTransform == null)
            {
                var studioListener = FindObjectOfType<StudioListener>();
                if (studioListener) listenerTransform = studioListener.transform;
                else
                {
                    Debug.LogWarning("VolumetricAudioOcclusion: No StudioListener found in the scene.", this);
                    return;
                }
            }

            // 遮蔽率を計算
            targetOcclusion = CalculateOcclusionFactor();
            
            // 数値を滑らかに補間
            currentOcclusion = Mathf.Lerp(currentOcclusion, targetOcclusion, Time.deltaTime * smoothing);
            
            // FMODパラメーターに反映
            emitter.SetParameter(parameterName, 1 - currentOcclusion);
            
            // ランダムオフセットを更新
            UpdateRandomOffsets();
        }

        /// <summary>
        /// 遮蔽率を計算する
        /// </summary>
        /// <returns>遮蔽率</returns>
        private float CalculateOcclusionFactor()
        {
            float totalOcclusion = 0.0f;
            Vector3 listenerPos = listenerTransform.position;
            Vector3 sourceCenter = transform.position;

            for (int i = 0; i < rayCount; i++)
            {
                // 音源の中心 + ランダムなオフセット位置から体積を計算
                Vector3 origin = sourceCenter + randomOffsets[i];
                RaycastHit hit;
                
                if (Physics.Linecast(origin, listenerPos, out hit, layerMask))
                {
                    string hitTag = hit.collider.tag;

                    if (materialDictionary.TryGetValue(hitTag, out float weight))
                    {
                        totalOcclusion += weight;
                    }
                    else
                    {
                        totalOcclusion += 1.0f;
                    }
                    
                    #if UNITY_EDITOR
                    // デバッグ表示
                    float w = materialDictionary.ContainsKey(hitTag) ? materialDictionary[hitTag] : 1.0f;
                    Color debugColor = Color.Lerp(Color.yellow, Color.red, w);
                    Debug.DrawLine(origin, hit.point, debugColor);
                    Debug.DrawLine(hit.point, listenerPos, Color.green);
                    #endif
                }
                else
                {
                    #if UNITY_EDITOR
                    Debug.DrawLine(origin, listenerPos, Color.green);
                    #endif
                }
            }

            return totalOcclusion / rayCount;
        }

        /// <summary>
        /// ランダムなオフセットを更新する
        /// </summary>
        private void UpdateRandomOffsets()
        {
            for (int i = 0; i < rayCount; i++)
            {
                randomOffsets[i] = Random.insideUnitSphere * sourceSize;
            }
        }
    }
}