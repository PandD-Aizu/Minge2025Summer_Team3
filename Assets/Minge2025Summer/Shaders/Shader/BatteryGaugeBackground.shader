Shader "Custom/LiquidWobble"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}                    // テクスチャ
        _FillAmount ("Fill Amount", Range(0.0, 1.0)) = 0.5       // 液面の高さ (0.0 - 1.0)
        _Amplitude ("Amplitude", Float) = 0.05                   // 波の振幅
        _Frequency ("Frequency", Float) = 10.0                   // 波の周波数
        _Speed ("Speed", Float) = 2.0                            // 波の速度
        _TopColor ("Top Color", Color) = (0.5, 0.5, 1, 1)     // 液面の色
        _BottomColor ("Bottom Color", Color) = (0, 0, 1, 1)   // 液体下部の色
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _FillAmount;
            float _Amplitude;
            float _Frequency;
            float _Speed;
            fixed4 _TopColor;
            fixed4 _BottomColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 波の計算
                float wave = sin(i.uv.x * _Frequency + _Time.y * _Speed) * _Amplitude;
                float surfaceY = _FillAmount + wave;

                if (i.uv.y < surfaceY)
                {
                    // グラデーションの計算
                    // i.uv.y(0からsurfaceYまで)を0から1の範囲に正規化する
                    float gradient = i.uv.y / surfaceY;
                    gradient = saturate(gradient); // 0-1の範囲にクランプ

                    // 2色を線形補間して色を決定
                    fixed4 liquidColor = lerp(_BottomColor, _TopColor, gradient);

                    fixed4 texColor = tex2D(_MainTex, i.uv);
                    return fixed4(liquidColor.rgb, texColor.a * liquidColor.a);
                }

                // 液体部分以外は透明
                return fixed4(0, 0, 0, 0);
            }
            ENDHLSL
        }
    }
}