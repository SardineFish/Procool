Shader "Procool/Color" {
    Properties {
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader {
        Pass {
            Tags { "LightMode"="BlockRender" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float4 _Color;
            int2 _HexCoord;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_TARGET
            {
                return 0;
            }
            

            ENDHLSL
        }
    }
}