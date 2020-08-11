Shader "Procool/Color" {
    Properties {
        _Color ("Color", Color) = (1, 1, 1, 1)
        [ToggleOff] _ZWrite ("ZWrite", Float) = 1.0
        _ZTest ("ZTest", Float) = 1.0
    }
    SubShader {
        ZTest [_ZTest]
        ZWrite [_ZWrite]

        Pass {

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_TARGET
            {
                return _Color;
            }
            

            ENDHLSL
        }
    }
}