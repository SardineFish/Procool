Shader "Procool/VFX/Blur" {
    Properties {
        _MainTex ("Main Tex", 2D) = "white" {}
    }
    HLSLINCLUDE 

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    float4 downScale(v2f_img i) : SV_TARGET
    {   
        float2 delta = float2(1, -1);

        float4 color = 0;
        color += tex2D(_MainTex, i.uv.xy + _MainTex_TexelSize.xy * delta.xx);
        color += tex2D(_MainTex, i.uv.xy + _MainTex_TexelSize.xy * delta.xy);
        color += tex2D(_MainTex, i.uv.xy + _MainTex_TexelSize.xy * delta.yx);
        color += tex2D(_MainTex, i.uv.xy + _MainTex_TexelSize.xy * delta.yy);

        return color / 4;
    }

    float4 upScale(v2f_img i) : SV_TARGET
    {
        float2 delta = float2(.5, -.5);

        float4 color = 0;
        color += tex2D(_MainTex, i.uv.xy + _MainTex_TexelSize.xy * delta.xx);
        color += tex2D(_MainTex, i.uv.xy + _MainTex_TexelSize.xy * delta.xy);
        color += tex2D(_MainTex, i.uv.xy + _MainTex_TexelSize.xy * delta.yx);
        color += tex2D(_MainTex, i.uv.xy + _MainTex_TexelSize.xy * delta.yy);

        return color / 4;
    }

    ENDHLSL

    SubShader {
        Pass {
            HLSLPROGRAM

            #pragma vertex vert_img
            #pragma fragment downScale

            ENDHLSL
        }

        Pass {
            HLSLPROGRAM

            #pragma vertex vert_img
            #pragma fragment upScale

            ENDHLSL
        }
    }
}