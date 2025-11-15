using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Minge2025Summer.Scripts.RenderingScript.HamBarEffect
{
    public class HamBarEffectPass : ScriptableRenderPass
    {
        public class PassData
        {
            internal TextureHandle source;
            internal Material material;
            internal float amplitude;
            internal float frequency;
        }

        private HamBarEffectVolume m_Volume;
        private readonly Material m_Material;
        private readonly ProfilingSampler m_ProfilingSampler;

        public HamBarEffectPass(Material material)
        {
            m_Material = material;
            m_ProfilingSampler = new ProfilingSampler(nameof(HamBarEffectPass));
        }

        public void Setup(HamBarEffectVolume volume)
        {
            m_Volume = volume;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (m_Material == null || m_Volume == null)
                return;
            
            // マテリアルプロパティの設定
            m_Material.SetFloat("_Amplitude", m_Volume.amplitude.value);
            m_Material.SetFloat("_Frequency", m_Volume.frequency.value);

            var urpResources = frameData.Get<UniversalResourceData>();
            var cameraColor = urpResources.activeColorTexture;

            var tempDesc = renderGraph.GetTextureDesc(cameraColor);
            tempDesc.name = "HamBarEffectTempTexture";
            var tempTexture = renderGraph.CreateTexture(tempDesc);
            
            // ハムバー効果のエフェクトパス
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("HamBarEffect", out var passData, m_ProfilingSampler))
            {
                // パスデータの設定
                passData.material = m_Material;
                passData.source = cameraColor;
                
                // リソースの使用を宣言
                builder.UseTexture(passData.source, AccessFlags.Read);
                builder.SetRenderAttachment(tempTexture, 0, AccessFlags.Write);
                
                // レンダリング関数の設定
                builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
                {
                    Blitter.BlitTexture(ctx.cmd, data.source, new Vector4(1, 1, 0, 0), data.material, 0);
                });
            }
            
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Copy to Camera Target", out var passData, m_ProfilingSampler))
            {
                passData.source = tempTexture;
                builder.UseTexture(passData.source, AccessFlags.Read);
                builder.SetRenderAttachment(cameraColor, 0, AccessFlags.Write);

                builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
                {
                    Blitter.BlitTexture(ctx.cmd, data.source, new Vector4(1, 1, 0, 0), 0.0f, false);
                });
            }
        }
    }
}