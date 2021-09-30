#ifndef LIL_PASS_FORWARD_LITE_INCLUDED
#define LIL_PASS_FORWARD_LITE_INCLUDED

#include "Includes/lil_pipeline.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Structure
#if defined(LIL_OUTLINE)
    #define LIL_V2F_POSITION_CS
    #define LIL_V2F_TEXCOORD0
    #if defined(LIL_V2F_FORCE_TEXCOORD1) || defined(LIL_USE_LIGHTMAP_UV)
        #define LIL_V2F_TEXCOORD1
    #endif
    #if defined(LIL_V2F_FORCE_POSITION_WS) || defined(LIL_PASS_FORWARDADD) || !defined(LIL_BRP) || defined(LIL_USE_LPPV)
        #define LIL_V2F_POSITION_WS
    #endif
    #if defined(LIL_V2F_FORCE_NORMAL) || defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SUBTRACTIVE) || defined(LIL_HDRP)
        #define LIL_V2F_NORMAL_WS
    #endif
    #if !defined(LIL_PASS_FORWARDADD)
        #define LIL_V2F_LIGHTCOLOR
        #define LIL_V2F_VERTEXLIGHT
    #endif
    #define LIL_V2F_FOG

    struct v2f
    {
        float4 positionCS       : SV_POSITION;
        float2 uv               : TEXCOORD0;
        #if defined(LIL_V2F_TEXCOORD1)
            float2 uv1              : TEXCOORD1;
        #endif
        #if defined(LIL_V2F_POSITION_WS)
            float3 positionWS       : TEXCOORD2;
        #endif
        #if defined(LIL_V2F_NORMAL_WS)
            float3 normalWS         : TEXCOORD3;
        #endif
        LIL_LIGHTCOLOR_COORDS(4)
        LIL_VERTEXLIGHT_COORDS(5)
        LIL_FOG_COORDS(6)
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };
#else
    #define LIL_V2F_POSITION_WS
    #define LIL_V2F_POSITION_CS
    #define LIL_V2F_TEXCOORD0
    #if defined(LIL_V2F_FORCE_TEXCOORD1) || defined(LIL_USE_LIGHTMAP_UV)
        #define LIL_V2F_TEXCOORD1
    #endif
    #define LIL_V2F_NORMAL_WS
    #define LIL_V2F_UVMAT
    #if !defined(LIL_PASS_FORWARDADD)
        #define LIL_V2F_LIGHTCOLOR
        #define LIL_V2F_LIGHTDIRECTION
        #define LIL_V2F_INDLIGHTCOLOR
        #define LIL_V2F_VERTEXLIGHT
    #endif
    #define LIL_V2F_FOG

    struct v2f
    {
        float4 positionCS       : SV_POSITION;
        float2 uv               : TEXCOORD0;
        #if defined(LIL_V2F_TEXCOORD1)
            float2 uv1              : TEXCOORD1;
        #endif
        float2 uvMat            : TEXCOORD2;
        float3 normalWS         : TEXCOORD3;
        float3 positionWS       : TEXCOORD4;
        LIL_LIGHTCOLOR_COORDS(5)
        LIL_LIGHTDIRECTION_COORDS(6)
        LIL_INDLIGHTCOLOR_COORDS(7)
        LIL_VERTEXLIGHT_COORDS(8)
        LIL_FOG_COORDS(9)
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Shader
#include "Includes/lil_common_vert.hlsl"
#include "Includes/lil_common_frag.hlsl"

#if defined(LIL_CUSTOM_V2F)
float4 frag(LIL_CUSTOM_V2F inputCustom LIL_VFACE(facing)) : SV_Target
{
    v2f input = inputCustom.base;
#else
float4 frag(v2f input LIL_VFACE(facing)) : SV_Target
{
#endif
    LIL_VFACE_FALLBACK(facing);
    LIL_SETUP_INSTANCE_ID(input);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    LIL_GET_HDRPDATA(input);
    #if defined(LIL_V2F_LIGHTDIRECTION)
        float3 lightDirection = input.lightDirection;
    #else
        float3 lightDirection = float3(0.0, 1.0, 0.0);
    #endif
    LIL_GET_MAINLIGHT(input, lightColor, lightDirection, attenuation);
    LIL_GET_VERTEXLIGHT(input, vertexLightColor);
    LIL_GET_ADDITIONALLIGHT(input.positionWS, additionalLightColor);
    #if !defined(LIL_PASS_FORWARDADD)
        #if defined(LIL_USE_LIGHTMAP)
            lightColor = clamp(lightColor, _LightMinLimit, _LightMaxLimit);
            lightColor = lerp(lightColor, lilGray(lightColor), _MonochromeLighting);
            lightColor = lerp(lightColor, 1.0, _AsUnlit);
        #endif
        #if defined(LIL_HDRP)
            float3 addLightColor = lerp(additionalLightColor, 0.0, _AsUnlit);
        #elif defined(_ADDITIONAL_LIGHTS)
            float3 addLightColor = vertexLightColor + lerp(additionalLightColor, 0.0, _AsUnlit);
        #else
            float3 addLightColor = vertexLightColor;
        #endif
        addLightColor = lerp(addLightColor, lilGray(addLightColor), _MonochromeLighting);
    #else
        lightColor = lerp(lightColor, lilGray(lightColor), _MonochromeLighting);
        lightColor = lerp(lightColor, 0.0, _AsUnlit);
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Apply Matelial & Lighting
    #if defined(LIL_OUTLINE)
        //------------------------------------------------------------------------------------------------------------------------------
        // UV
        BEFORE_ANIMATE_OUTLINE_UV
        OVERRIDE_ANIMATE_OUTLINE_UV

        //------------------------------------------------------------------------------------------------------------------------------
        // Main Color
        float4 col = 1.0;
        BEFORE_OUTLINE_COLOR
        OVERRIDE_OUTLINE_COLOR

        //------------------------------------------------------------------------------------------------------------------------------
        // Alpha
        #if LIL_RENDER == 0
            // Opaque
        #elif LIL_RENDER == 1
            // Cutout
            clip(col.a - _Cutoff);
        #elif LIL_RENDER == 2
            // Transparent
            clip(col.a - _Cutoff);
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Copy
        float3 albedo = col.rgb;

        //------------------------------------------------------------------------------------------------------------------------------
        // Lighting
        col.rgb = lerp(col.rgb, col.rgb * min(lightColor + addLightColor, _LightMaxLimit), _OutlineEnableLighting);

        #if defined(LIL_HDRP)
            float3 viewDirection = normalize(LIL_GET_VIEWDIR_WS(input.positionWS.xyz));
        #endif
    #else
        //------------------------------------------------------------------------------------------------------------------------------
        // UV
        BEFORE_ANIMATE_MAIN_UV
        OVERRIDE_ANIMATE_MAIN_UV

        //------------------------------------------------------------------------------------------------------------------------------
        // Main Color
        float4 col = 1.0;
        BEFORE_MAIN
        OVERRIDE_MAIN
        float4 triMask = 1.0;
        triMask = LIL_SAMPLE_2D(_TriMask, sampler_MainTex, uvMain);

        //------------------------------------------------------------------------------------------------------------------------------
        // Alpha
        #if LIL_RENDER == 0
            // Opaque
        #elif LIL_RENDER == 1
            // Cutout
            clip(col.a - _Cutoff);
        #elif LIL_RENDER == 2
            // Transparent
            clip(col.a - _Cutoff);
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Normal
        float3 normalDirection = normalize(input.normalWS);
        normalDirection = facing < (_FlipNormal-1.0) ? -normalDirection : normalDirection;

        //------------------------------------------------------------------------------------------------------------------------------
        // MatCap
        BEFORE_MATCAP
        OVERRIDE_MATCAP

        //------------------------------------------------------------------------------------------------------------------------------
        // Copy
        float3 albedo = col.rgb;

        //------------------------------------------------------------------------------------------------------------------------------
        // Lighting
        float shadowmix = 1.0;
        BEFORE_SHADOW
        #ifndef LIL_PASS_FORWARDADD
            OVERRIDE_SHADOW

            lightColor += addLightColor;
            shadowmix += lilLuminance(addLightColor);
            col.rgb += albedo * addLightColor;

            lightColor = min(lightColor, _LightMaxLimit);
            shadowmix = saturate(shadowmix);
            col.rgb = min(col.rgb, albedo * _LightMaxLimit);
        #else
            col.rgb *= lightColor;
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Rim light
        float3 viewDirection = normalize(LIL_GET_VIEWDIR_WS(input.positionWS.xyz));
        float nvabs = abs(dot(normalDirection, viewDirection));
        BEFORE_RIMLIGHT
        OVERRIDE_RIMLIGHT

        BEFORE_EMISSION_1ST
        #ifndef LIL_PASS_FORWARDADD
            OVERRIDE_EMISSION_1ST
        #endif
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Fog
    BEFORE_FOG
    OVERRIDE_FOG

    BEFORE_OUTPUT
    OVERRIDE_OUTPUT
}

#endif