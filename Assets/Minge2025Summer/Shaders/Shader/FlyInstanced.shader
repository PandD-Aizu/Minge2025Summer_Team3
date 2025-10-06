Shader "Unlit/FlyInstanced"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing // インスタンシングを有効化

            #include "UnityCG.cginc"

            // 頂点シェーダーへの入力構造体
            struct appdata
            {
                float4 vertex : POSITION;
                // 修正点: マクロの代わりに、SV_InstanceIDセマンティクスでIDを直接受け取る
                uint instanceID : SV_InstanceID; 
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };
            
            // Compute Shaderから渡されるデータ構造体（変更なし）
            struct FlyData
            {
                float3 position;
                float3 velocity;
                float4x4 mat;
                int state;
            };

            StructuredBuffer<FlyData> boidDataBuffer;
            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                
                // 修正点: UNITY_SETUP_INSTANCE_ID(v); は不要になります

                // 修正点: v.instanceID を使ってバッファにアクセスする
                FlyData data = boidDataBuffer[v.instanceID];

                // 頂点位置を行列で変換
                o.vertex = mul(data.mat, v.vertex);
                // ビュープロジェクション変換
                o.vertex = mul(UNITY_MATRIX_VP, o.vertex);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}