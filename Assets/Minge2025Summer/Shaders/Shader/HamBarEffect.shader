Shader "Custom/HamBarEffect"
{
    Properties
    {
        _Amplitude ("Vertical Glitch Amount", Range(0, 1)) = 0.05
        _Frequency ("Vertical Glitch Frequency", Range(1, 100)) = 10
    }
    SubShader
    {
        LOD 100
        Pass
        {
            ZWrite Off
            Cull Off
            ZTest Always

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            SAMPLER(sampler_BlitTexture);
            
            float _Amplitude;
            float _Frequency;
            
            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                float verticalOffset = 0.0;

                // グリッチのトリガーを計算
                float glitchTrigger = sin(_Time.y * _Frequency);

                // グリッチを発生させる
                if (glitchTrigger > 0.99) 
                {
                    // 瞬時にUVを上下ランダムにずらす
                    verticalOffset = (rand(uv.y + _Time.y) - 0.5) * _Amplitude;
                }
                
                // UVを歪ませる
                float2 distortedUV = float2(uv.x, frac(uv.y + verticalOffset));
                
                // サンプリング
                half4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, distortedUV);
                
                return color;
            }

            ENDHLSL
        }
    }
}