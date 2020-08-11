Shader "Procool/Trail" {
    SubShader{

        ZTest Less
        ZWrite Off
        
        Pass {

            HLSLPROGRAM

            #include "UnityCG.cginc"
            
            #pragma vertex vert
            #pragma fragment frag
            
            struct v2f {
                float4 vertex: SV_POSITION;
                float4 color: TEXCOORD0;
            };

            v2f vert(appdata_full i)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.color = i.color;
                return o;
            }

            float4 frag(v2f i) : SV_TARGET
            {
                return i.color;
            }

            ENDHLSL

        }
    }
}