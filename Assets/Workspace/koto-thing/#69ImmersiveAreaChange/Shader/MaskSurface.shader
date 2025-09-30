Shader "Custom/MaskSurfaceShader"
{
    Properties
    {
        _Color("Base Color", Color) = (1,1,1,1)
        _MaskID("Mask ID", Int) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Geometry" }
        
        ZWrite Off
        Cull Off
        ColorMask 0
        
        Stencil
        {
            Ref [_MaskID]
            Comp Always
            Pass Replace
        }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct appdata
            {
                float4 vertex : POSITION;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            
            half4 frag(v2f i) : SV_Target
            {
                return half4(0,0,0,0);
            }
            ENDCG
        }
    } 
}

