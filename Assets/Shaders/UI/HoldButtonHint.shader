// Modified from /UI/Default
Shader "Procool/UI/HoldButtonHint" {
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _RingWidthRatio ("Ring Width Ratio", Range(0, 1)) = 0.2

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #define PI (3.14159265358979323846264338327950288419716939937510)

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            float _RingWidthRatio;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color.rgb = v.color.rgb * _Color.rgb;
                OUT.color.a = v.color.a;
                return OUT;
            }

            float4 sample(float2 uv, float progress)
            {
                uv = uv.xy * 2 - 1;

                float r = length(uv);
                float ang = atan2(uv.y, uv.x);
                ang -= PI / 2;
                if(ang > 0)
                    ang -= 2 * PI;
                ang = -ang;

                float t = ang / (2 * PI);

                float4 color = 1;

                if(t > progress)
                    color = 0;
                else if(r < 1 - _RingWidthRatio)
                    color = 0;
                else if(r > 1)
                    color = 0;

                return color;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float progress = IN.color.a;
                float2 uv = IN.texcoord;
                
                float2 delta = abs(float2(ddx(IN.texcoord.x), ddy(IN.texcoord.y)));
                float2 sign = float2(1, -1);

                float4 color = 0;
                color += sample(uv + delta / 4 * sign.xx, progress);
                color += sample(uv + delta / 4 * sign.xy, progress);
                color += sample(uv + delta / 4 * sign.yx, progress);
                color += sample(uv + delta / 4 * sign.yy, progress);
                color /= 4;
                color.rgb *= IN.color.rgb;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                return color;
            }
        ENDCG
        }
    }
}