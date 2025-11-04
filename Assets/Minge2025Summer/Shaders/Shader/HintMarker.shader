Shader "Custom/PickupHintMarkerSprite"
{
    Properties
    {
        [PerRendererData]_MainTex ("Sprite Texture", 2D) = "white" {}
        [PerRendererData]_RendererColor ("Renderer Color", Color) = (1,1,1,1)
        [PerRendererData]_Flip ("Flip", Vector) = (1,1,0,0)

        _Color ("Tint (RGBA)", Color) = (1,1,0,1)
        _Emission ("Emission Intensity", Float) = 2
        _PulseSpeed ("Pulse Speed", Float) = 2
        _PulseAmplitude ("Pulse Amplitude", Float) = 0.35
        _KeepScreenSize ("Keep Screen Size (0/1)", Float) = 1
        _TargetScreenSize ("Target Screen Size (px)", Float) = 64
        _Scale ("World Scale Multiplier", Float) = 1
        _DisplayMode ("Depth Mode (0=Normal 1=ForceFront 2=WriteDepth)", Float) = 0
        _Fade ("Global Alpha Multiplier", Range(0,1)) = 1
        _AllowBloom ("Allow Bloom (0/1)", Float) = 0
    }
    SubShader
    {
        Tags{
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent+400"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "HINTMARKER_URP"
            Tags{ "LightMode"="SRPDefaultUnlit" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 2.0
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"

            #ifndef UnityPixelSnap
                #define UnityPixelSnap(pos) (pos)
            #endif

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _Emission;
                float _PulseSpeed;
                float _PulseAmplitude;
                float _KeepScreenSize;
                float _TargetScreenSize;
                float _Scale;
                float _DisplayMode;
                float _Fade;
                float _AllowBloom;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            #if defined(UNITY_INSTANCING_ENABLED)
            UNITY_INSTANCING_BUFFER_START(UnityPerDrawSprite)
                UNITY_DEFINE_INSTANCED_PROP(float4, _RendererColor)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Flip) // xy: scale(1|-1), zw: offset(0|1)
            UNITY_INSTANCING_BUFFER_END(UnityPerDrawSprite)
            #else
            float4 _RendererColor;
            float4 _Flip;
            #endif

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 color       : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings Vert (Attributes IN)
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                Varyings OUT;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                const float4 rc   = UNITY_ACCESS_INSTANCED_PROP(UnityPerDrawSprite, _RendererColor);
                const float4 flip = UNITY_ACCESS_INSTANCED_PROP(UnityPerDrawSprite, _Flip);

                // ST の符号を Flip に吸収して、反転は flip のみで制御
                float2 stScale  = _MainTex_ST.xy;
                float2 stOffset = _MainTex_ST.zw;
                float2 scale = flip.xy;
                float2 offs  = flip.zw;
                if (stScale.x < 0) { scale.x *= -1; offs.x = 1.0 - offs.x; }
                if (stScale.y < 0) { scale.y *= -1; offs.y = 1.0 - offs.y; }

                // クアッドは ST 符号反映後のUVから生成
                float2 uvFlip = IN.uv * scale + offs;
                float2 quad   = uvFlip * 2.0 - 1.0;

                float3 worldCenter = TransformObjectToWorld(float3(0,0,0));

                float3 camPos = GetCameraPositionWS();
                float3 toCam = normalize(worldCenter - camPos);
                float3 upDir = float3(0,1,0);
                if (abs(dot(upDir, toCam)) > 0.95) upDir = float3(0,0,1);
                float3 right = normalize(cross(upDir, toCam));
                upDir = normalize(cross(toCam, right));

                float3 camForward = -UNITY_MATRIX_V[2].xyz;
                float dist = dot(worldCenter - camPos, camForward);
                dist = max(dist, 0.0001);

                float fovFactor = UNITY_MATRIX_P[1][1];
                float targetWorld = (_TargetScreenSize / max(_ScreenParams.y, 1.0)) * (dist / fovFactor);
                float size = lerp(1.0, targetWorld, saturate(_KeepScreenSize)) * _Scale;

                float3 worldPos = worldCenter
                    + right * (quad.x * size)
                    + upDir  * (quad.y * size);

                OUT.positionHCS = TransformWorldToHClip(worldPos);
                #ifdef PIXELSNAP_ON
                OUT.positionHCS = UnityPixelSnap(OUT.positionHCS);
                #endif

                // サンプリングは ST の絶対値スケール＋オフセット
                float2 stScaleAbs  = abs(stScale);
                float2 stOffsetPos = stOffset + min(0, stScale);
                
                OUT.uv    = uvFlip * stScaleAbs + stOffsetPos;
                OUT.color = IN.color * _Color * rc;
                return OUT;
            }

            half4 Frag (Varyings IN) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half pulse = 1.0h + _PulseAmplitude * sin(_TimeParameters.y * _PulseSpeed);
                half4 col = IN.color * tex;
                col.rgb *= (_Emission * pulse);
                col.a   *= _Fade;
                return col;
            }
            ENDHLSL
        }
    }

    FallBack Off
}
