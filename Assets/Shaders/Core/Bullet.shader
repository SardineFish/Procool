Shader "Procool/Bullet" {
    Properties {}

    HLSLINCLUDE

    #include "UnityCG.cginc"

    struct BulletData {
        float3 position;
        float size;
        float4 primaryColor;
        float4 secondaryColor;
    };


    StructuredBuffer<BulletData> BulletDataBuffer;

    struct v2f {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
        float size : TEXCOORD1;
        float4 primaryColor : TEXCOORD2;
        float4 secondaryColor : TEXCOORD3;
    };

    v2f vert(appdata_base v, uint instanceID : SV_INSTANCEID)
    {
        v2f o;

        BulletData data = BulletDataBuffer[instanceID];

        float3 worldPos = v.vertex.xyz * data.size + data.position;
        o.pos = mul(UNITY_MATRIX_VP, float4(worldPos.xyz, 1));
        o.uv = v.texcoord;
        o.size = data.size;
        o.primaryColor = data.primaryColor;
        o.secondaryColor = data.secondaryColor;

        return o;
    }

    float4 frag(v2f i) : SV_TARGET
    {
        float2 p = i.uv * 2 - 1;

        float r = length(p);

        if(r > 1)
            return 0;
        else if (r > .6)
            return i.primaryColor;
        else
            return i.secondaryColor;

        return i.primaryColor;
    }


    ENDHLSL

    SubShader {
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            ZTest Off
            ZWrite Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5


            ENDHLSL
        }
    }
}