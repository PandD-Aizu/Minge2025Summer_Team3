Shader "Unlit/PickupHintMarkerSprite"
{
    Properties
    {
        [PerRendererData]_MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint (RGBA)", Color) = (1,1,0,1)
        _Emission ("Emission Intensity", Float) = 2
        _PulseSpeed ("Pulse Speed", Float) = 2
        _PulseAmplitude ("Pulse Amplitude", Float) = 0.35
        _KeepScreenSize ("Keep Screen Size (0/1)", Float) = 1
        _ScreenSize ("Target Screen Size (px)", Float) = 64
        _Scale ("World Scale Multiplier", Float) = 1
        _DisplayMode ("Depth Mode (0=Normal 1=ForceFront 2=WriteDepth)", Float) = 0
        _Fade ("Global Alpha Multiplier", Range(0,1)) = 1
    }

    SubShader
    {
        // Transparent より後ろで確実に最後寄りに描く
        Tags{
            "Queue"="Transparent+400"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "HINTMARKER"
            // めり込み防止で少し前へ
            Offset -1,-1

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Emission;
            float _PulseSpeed;
            float _PulseAmplitude;
            float _KeepScreenSize;
            float _ScreenSize;
            float _Scale;
            float _DisplayMode;
            float _Fade;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color  : COLOR;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;

                // 中心
                float3 worldCenter = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;

                // uv を -0.5..0.5
                float2 quad = v.uv * 2 - 1;

                // カメラ面ビルボード
                float3 camPos = _WorldSpaceCameraPos;
                float3 toCam = normalize(worldCenter - camPos);
                float3 upDir = float3(0,1,0);
                if (abs(dot(upDir, toCam)) > 0.95) upDir = float3(0,0,1);
                float3 right = normalize(cross(upDir, toCam));
                upDir = normalize(cross(toCam, right));

                // 距離
                float3 camForward = UNITY_MATRIX_V[2].xyz * -1.0;
                float dist = dot(worldCenter - camPos, camForward);
                dist = max(dist, 0.0001);

                // 目標スクリーンサイズ
                float fovFactor = UNITY_MATRIX_P[1][1];
                float targetWorld = (_ScreenSize / max(_ScreenParams.y,1)) * (dist / fovFactor);
                float size = lerp(1.0, targetWorld, saturate(_KeepScreenSize)) * _Scale;

                float3 worldPos = worldCenter
                    + right * (quad.x * size)
                    + upDir  * (quad.y * size);

                o.pos = UnityWorldToClipPos(worldPos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;

                #ifdef PIXELSNAP_ON
                o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed pulse = 1.0 + _PulseAmplitude * sin(_Time.y * _PulseSpeed);
                fixed4 col = i.color * tex;
                col.rgb *= (_Emission * pulse);
                col.a *= _Fade;
                return col;
            }
            ENDCG

            // 動的キーワードにせず、シンプルな分岐: CommandBuffer 等で SetFloat 変更後に再適用
            // 深度モード適用 (FixedFunction で分岐できないため Pass 分離が最も確実)
        }

        // Mode 1: 常に前(ZTest Always)
        Pass
        {
            Name "HINTMARKER_FORCE_FRONT"
            Tags{ "LightMode"="Always" }
            Cull Off
            Lighting Off
            ZTest Always
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask 0
        }

        // Mode 2: 深度書き込み用 (ZWrite On で他半透明に埋もれない)
        Pass
        {
            Name "HINTMARKER_WRITE_DEPTH"
            Tags{ "LightMode"="DepthOnly" }
            Cull Off
            ZTest LEqual
            ZWrite On
            ColorMask 0
        }
    }

    FallBack Off
}
