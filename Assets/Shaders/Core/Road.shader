Shader "Procool/Road"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (.2, .2, .2, 1)
        _DashLineColor ("Dash Line Color", Color) = (1, 1, 1, 1)
        _DashRatio ("Dash Ratio", Range(0, 1)) = 0.5
        _DashLineWidth ("Dash Line Width", Float) = 0.1
        _DashLineDistance ("Dash Line Distance", Float) = 1
        _BikeLaneWidth ("Bike Lane Width", Float) = 2
        _StopLineGap ("Stop Line Gap", Float) = 0.2
        _CrosswalkWidth ("Crosswalk Width", Float) = 1
        _CrosswalkLineDistance ("Crosswalk Line Distance", Float) = 0.15
        _CrosswalkLineRatio ("Crosswalk Line Ratio", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        ZTest Less
        ZWrite Off

        Pass
        {
            Tags { "LightMode"="OpaqueWithLOD" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 data : TEXCOORD1; // (width, length, type)
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 data : TEXCOORD1; // (width, length, type)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _DashLineColor;
            float _DashRatio;
            float _DashLineWidth;
            float _DashLineDistance;
            float _StopLineGap;
            float _CrosswalkLineDistance;
            float _CrosswalkWidth;
            float _CrosswalkLineRatio;
            float _BikeLaneWidth;
            int _GlobalLOD;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.data = v.data;
                return o;
            }

            float4 DrawCrossRoad()
            {
                return 0;
            }

            float4 DrawCrossWalk(float2 roadPos)
            {
                if(frac(roadPos.x / _CrosswalkLineDistance) < _CrosswalkLineRatio)
                    return _DashLineColor;
                return 0;
            }

            float4 DrawStreet(float2 uv, float2 size)
            {
                if(uv.x < 0 || uv.x > 1)
                    return 0;
                float longitudinalPos = uv.x * size.y;
                float lateralPos = (uv.y - .5) * size.x * 2;
                float length = size.y;
                float width = size.x;

                // Crosswalk
                if(longitudinalPos < _CrosswalkWidth || size.y - longitudinalPos < _CrosswalkWidth)
                    return DrawCrossWalk(float2(lateralPos, longitudinalPos));
                
                longitudinalPos -= (_StopLineGap + _CrosswalkWidth);
                length -= 2 *  (_StopLineGap + _CrosswalkWidth);
                if(longitudinalPos < 0 || longitudinalPos > length)
                    return 0;
                // Dash line
                if(frac(longitudinalPos / _DashLineDistance) < _DashRatio && abs(lateralPos) < _DashLineWidth)
                    return _DashLineColor;
                // Stop line
                else if(abs(longitudinalPos) < _DashLineWidth && lateralPos > -_DashLineWidth || size.y - longitudinalPos < 2 * _DashLineWidth && lateralPos.x < _DashLineWidth)
                    return _DashLineColor;

                return 0;
            }

            float4 DrawArterial(float2 uv, float2 size)
            {
                if(uv.x < 0 || uv.x > 1)
                    return 0;
                float longitudinalPos = uv.x * size.y;
                float lateralPos = (uv.y - .5) * size.x * 2;
                float length = size.y;
                float width = size.x;

                // Crosswalk
                if(longitudinalPos < _CrosswalkWidth || size.y - longitudinalPos < _CrosswalkWidth)
                    return DrawCrossWalk(float2(lateralPos, longitudinalPos));
                
                longitudinalPos -= (_StopLineGap + _CrosswalkWidth);
                length -= 2 *  (_StopLineGap + _CrosswalkWidth);
                if(longitudinalPos < 0 || longitudinalPos > length)
                    return 0;
                    
                // Dash line
                if(frac(longitudinalPos / _DashLineDistance) < _DashRatio && abs(lateralPos) < _DashLineWidth)
                    return _DashLineColor;
                else if (abs(lateralPos - width + _BikeLaneWidth) < _DashLineWidth/2 || abs(lateralPos + width - _BikeLaneWidth) < _DashLineWidth/2)
                    return _DashLineColor;
                // Stop line
                else if(abs(longitudinalPos) < _DashLineWidth && lateralPos > -_DashLineWidth || size.y - longitudinalPos < 2 * _DashLineWidth && lateralPos.x < _DashLineWidth)
                    return _DashLineColor;

                return 0;
            }

            float4 DrawExpressWay(float2 uv, float2 size)
            {
                if(uv.x < 0 || uv.x > 1)
                    return 0;
                float longitudinalPos = uv.x * size.y;
                float lateralPos = (uv.y - .5) * size.x * 2;
                float length = size.y;
                float width = size.x;

                
                // longitudinalPos -= (_StopLineGap + _CrosswalkWidth);
                // length -= 2 *  (_StopLineGap + _CrosswalkWidth);
                // if(longitudinalPos < 0 || longitudinalPos > length)
                //     return 0;
                // Dash line
                if(frac(longitudinalPos / _DashLineDistance) < _DashRatio && abs(abs(lateralPos) - width / 2) < _DashLineWidth)
                    return _DashLineColor;
                if(abs(abs(lateralPos) - _DashLineWidth * 2) < _DashLineWidth)
                    return _DashLineColor;
                // Stop line
                // else if(abs(longitudinalPos) < _DashLineWidth && lateralPos > _DashLineWidth || size.y - longitudinalPos < 2 * _DashLineWidth && lateralPos.x < -_DashLineWidth)
                //     return _DashLineColor;
                    
                return 0;
            }

            float4 DrawHighway(float2 uv, float2 size)
            {
                return 0;
            }



            #define ROAD_TYPE_CORSSROAD 0
            #define ROAD_TYPE_ALLEY 1
            #define ROAD_TYPE_STREET 2
            #define ROAD_TYPE_ARTERIAL 3
            #define ROAD_TYPE_EXPRESS_WAY 4
            #define ROAD_TYPE_HIGH_WAY 5

            float4 DrawRoadMarking(v2f i)
            {
                float4 baseColor = tex2D(_MainTex, i.uv) * _Color.rgba;

                float4 overlayColor = 0;
                int type = round(i.data.z);
                float2 size = i.data.xy;
                switch(type)
                {
                    case ROAD_TYPE_STREET:
                        overlayColor = DrawStreet(i.uv, size);
                        break;
                    case ROAD_TYPE_ARTERIAL:
                        overlayColor = DrawArterial(i.uv, size);
                        break;
                    case ROAD_TYPE_EXPRESS_WAY:
                        overlayColor = DrawExpressWay(i.uv, size);
                        break;
                    case ROAD_TYPE_HIGH_WAY:
                        overlayColor = DrawHighway(i.uv, size);
                        break;
                    default:
                        overlayColor = 0;
                        break;
                }

                float3 color = overlayColor.rgb * overlayColor.a + baseColor.rgb * (1 - overlayColor.a);
                return float4(color.rgb, 1);
            }

            float4 DrawRoadColor(v2f i)
            {
                return tex2D(_MainTex, i.uv) * _Color.rgba;
            }

            float4 frag (v2f i) : SV_Target
            {
                switch(_GlobalLOD)
                {
                    case 0:
                        return DrawRoadMarking(i);
                    case 1:
                        return DrawRoadColor(i);
                }
                return 0;
            }
            ENDCG
        }
    }
}
