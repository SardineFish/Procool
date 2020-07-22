Shader "Procool/Editor/HexagonGrid" {
    HLSLINCLUDE

    #include "UnityCG.cginc"

    #include "../Common/HexagonMath.hlsl"

    #include "../Common/Noise.hlsl"

    float2 WorldOrigin;
    float4 ViewWorldRect;
    float2 PixelWorldSize;
    float Size;
    int Layout;


    float4 frag(v2f_img i) : SV_TARGET
    {
        float2 worldPos = ViewWorldRect.xy + i.uv.xy * ViewWorldRect.zw - WorldOrigin;
        
        float2 subPos[] = {
            worldPos + PixelWorldSize / 2 * float2(-1, -1),
            worldPos + PixelWorldSize / 2 * float2( 1, -1),
            worldPos + PixelWorldSize / 2 * float2( 1,  1),
            worldPos + PixelWorldSize / 2 * float2(-1,  1),
        };
        float2 hexPos[] = {
            hexagon_round(world_to_hexagon(subPos[0], Size, Layout)),
            hexagon_round(world_to_hexagon(subPos[1], Size, Layout)),
            hexagon_round(world_to_hexagon(subPos[2], Size, Layout)),
            hexagon_round(world_to_hexagon(subPos[3], Size, Layout)),
        };

        int diffCount = 0;

        if(!hexagon_equal(hexPos[0], hexPos[1]))
            diffCount++;
        if(!hexagon_equal(hexPos[2], hexPos[0]) && !hexagon_equal(hexPos[2], hexPos[1]))
            diffCount++;
        if(!hexagon_equal(hexPos[3], hexPos[0]) && !hexagon_equal(hexPos[3], hexPos[1]) && !hexagon_equal(hexPos[3], hexPos[2]))
            diffCount++;
        
        float t = 1 - diffCount / 3.0;

        float2 hex = world_to_hexagon(worldPos, Size, Layout);
        float3 cube = axial_to_cube(hex);
        int3 cubeInt = hexagon_round(axial_to_cube(hex));

        float3 color = hash33(cubeInt);
        color *= t;

        float thickness = 0.01;
        float3 p = abs(frac(cube) - .5);

        return float4(.5, .5, .5, 1 - t);
    }

    ENDHLSL

    SubShader {
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM

            #pragma vertex vert_img

            #pragma fragment frag

            ENDHLSL
        }
    }
}