using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Minge2025Summer.Scripts.RenderingScript.HamBarEffect
{
    public class HamBarEffectFeature : ScriptableRendererFeature
    {
        [Serializable]
        public class Settings
        {
            public Shader m_HamBarEffectShader;
            public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public Settings settings = new Settings();

        private HamBarEffectPass m_ScriptablePass;
        private Material m_Material;

        public override void Create()
        {
            if (settings.m_HamBarEffectShader != null)
                m_Material = CoreUtils.CreateEngineMaterial(settings.m_HamBarEffectShader);
            
            m_ScriptablePass = new HamBarEffectPass(m_Material);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (m_ScriptablePass == null || m_Material == null)
            {
                Debug.LogError("HamBarEffectPass or Material is null.");
                return;
            }

            var stack = VolumeManager.instance.stack;
            var customVolume = stack.GetComponent<HamBarEffectVolume>();

            if (customVolume != null && customVolume.IsActive())
            {
                m_ScriptablePass.Setup(customVolume);
                m_ScriptablePass.renderPassEvent = settings.renderPassEvent;
                renderer.EnqueuePass(m_ScriptablePass);
            }
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(m_Material);
            m_Material = null;
        }
    }
}