Shader "Hidden/ltsother_bakeramp"
{
    Properties
    {
        _ShadowStrength             ("sStrength", Range(0, 1)) = 1
        _ShadowColor                ("Shadow Color", Color) = (0.82,0.76,0.85,1.0)
        _ShadowBorder               ("sBorder", Range(0, 1)) = 0.5
        _ShadowBlur                 ("sBlur", Range(0, 1)) = 0.1
        _Shadow2ndColor             ("2nd Color", Color) = (0.68,0.66,0.79,1)
        _Shadow2ndBorder            ("sBorder", Range(0, 1)) = 0.15
        _Shadow2ndBlur              ("sBlur", Range(0, 1)) = 0.1
        _Shadow3rdColor             ("3rd Color", Color) = (0,0,0,0)
        _Shadow3rdBorder            ("sBorder", Range(0, 1)) = 0.25
        _Shadow3rdBlur              ("sBlur", Range(0, 1)) = 0.1
        _ShadowBorderColor          ("sShadowBorderColor", Color) = (1,0.1,0,1)
        _ShadowBorderRange          ("sShadowBorderRange", Range(0, 1)) = 0.08
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4  _ShadowColor;
            float4  _Shadow2ndColor;
            float4  _Shadow3rdColor;
            float4  _ShadowBorderColor;
            float   _ShadowStrength;
            float   _ShadowBorder;
            float   _ShadowBlur;
            float   _Shadow2ndBorder;
            float   _Shadow2ndBlur;
            float   _Shadow3rdBorder;
            float   _Shadow3rdBlur;
            float   _ShadowBorderRange;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float lilTooningScale(float aascale, float value, float border, float blur)
            {
                float borderMin = saturate(border - blur * 0.5);
                float borderMax = saturate(border + blur * 0.5);
                return saturate((value - borderMin) / saturate(borderMax - borderMin));
            }

            float lilTooningScale(float aascale, float value, float border, float blur, float borderRange)
            {
                float borderMin = saturate(border - blur * 0.5 - borderRange);
                float borderMax = saturate(border + blur * 0.5);
                return saturate((value - borderMin) / saturate(borderMax - borderMin));
            }

            float4 frag(v2f i) : SV_Target
            {
                // Shade
                float4 lns = i.uv.x;

                // Blur Scale
                float shadowBlur = _ShadowBlur;
                float shadow2ndBlur = _Shadow2ndBlur;
                float shadow3rdBlur = _Shadow3rdBlur;

                // AO Map & Toon
                lns.w = lns.x;
                lns.x = lilTooningScale(0, lns.x, _ShadowBorder, shadowBlur);
                lns.y = lilTooningScale(0, lns.y, _Shadow2ndBorder, shadow2ndBlur);
                lns.z = lilTooningScale(0, lns.z, _Shadow3rdBorder, shadow3rdBlur);
                lns.w = lilTooningScale(0, lns.w, _ShadowBorder, shadowBlur, _ShadowBorderRange);

                // Strength
                float shadowStrength = _ShadowStrength;
                #ifdef LIL_COLORSPACE_GAMMA
                    shadowStrength = lilSRGBToLinear(shadowStrength);
                #endif
                lns.x = lerp(1.0, lns.x, shadowStrength);

                // Shadow Color 1
                float3 indirectCol = _ShadowColor.rgb;

                // Shadow Color 2
                lns.y = _Shadow2ndColor.a - lns.y * _Shadow2ndColor.a;
                indirectCol = lerp(indirectCol, _Shadow2ndColor.rgb, lns.y);

                // Shadow Color 3
                lns.z = _Shadow3rdColor.a - lns.z * _Shadow3rdColor.a;
                indirectCol = lerp(indirectCol, _Shadow3rdColor.rgb, lns.z);

                // Gradation
                indirectCol = lerp(indirectCol, 1, lns.w * _ShadowBorderColor.rgb);

                // Mix
                return float4(lerp(indirectCol, 1, lns.x), 1);
            }
            ENDCG
        }
    }
}
