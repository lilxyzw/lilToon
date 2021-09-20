#ifndef LIL_FRAGMENT_FUR_INCLUDED
#define LIL_FRAGMENT_FUR_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Fragment shader
#include "Includes/lil_common_frag.hlsl"

#if defined(LIL_CUSTOM_V2F)
float4 frag(LIL_CUSTOM_V2F inputCustom) : SV_Target
{
    v2f input = inputCustom.base;
#else
float4 frag(v2f input) : SV_Target
{
#endif
    LIL_SETUP_INSTANCE_ID(input);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    LIL_GET_HDRPDATA(input);
    LIL_GET_MAINLIGHT(input, lightColor, lightDirection, attenuation);
    LIL_GET_VERTEXLIGHT(input, vertexLightColor);
    LIL_GET_ADDITIONALLIGHT(input.positionWS, additionalLightColor);
    #if !defined(LIL_PASS_FORWARDADD)
        #if defined(LIL_USE_LIGHTMAP)
            lightColor = clamp(lightColor, _LightMinLimit, _LightMaxLimit);
            lightColor = lerp(lightColor, lilMonoColor(lightColor), _MonochromeLighting);
            lightColor = lerp(lightColor, 1.0, _AsUnlit);
        #endif
        #if defined(LIL_HDRP)
            float3 addLightColor = lerp(additionalLightColor, 0.0, _AsUnlit);
        #elif defined(_ADDITIONAL_LIGHTS)
            float3 addLightColor = vertexLightColor + lerp(additionalLightColor, 0.0, _AsUnlit);
        #else
            float3 addLightColor = vertexLightColor;
        #endif
        addLightColor = lerp(addLightColor, lilMonoColor(addLightColor), _MonochromeLighting);
    #else
        lightColor = lerp(lightColor, lilMonoColor(lightColor), _MonochromeLighting);
        lightColor = lerp(lightColor, 0.0, _AsUnlit);
    #endif

    float facing = 1.0;

    //------------------------------------------------------------------------------------------------------------------------------
    // UV
    BEFORE_ANIMATE_MAIN_UV
    OVERRIDE_ANIMATE_MAIN_UV

    //------------------------------------------------------------------------------------------------------------------------------
    // Main Color
    float4 col = 1.0;
    BEFORE_MAIN
    OVERRIDE_MAIN

    //------------------------------------------------------------------------------------------------------------------------------
    // Fur
    BEFORE_FUR
    OVERRIDE_FUR

    //------------------------------------------------------------------------------------------------------------------------------
    // Copy
    float3 albedo = col.rgb;

    //------------------------------------------------------------------------------------------------------------------------------
    // Alpha
    #if LIL_RENDER == 1
        // Cutout
        col.a = saturate(col.a*5.0-2.0);
    #else
        // Transparent
        clip(col.a - _Cutoff);
    #endif

    BEFORE_SHADOW
    #ifndef LIL_PASS_FORWARDADD
        //------------------------------------------------------------------------------------------------------------------------------
        // Lighting
        #if defined(LIL_FEATURE_SHADOW)
            float3 normalDirection = normalize(input.normalWS);
            float shadowmix = 1.0;
            OVERRIDE_SHADOW
            col.rgb += albedo * addLightColor;
            col.rgb = min(col.rgb, albedo * _LightMaxLimit);
        #else
            col.rgb *= saturate(lightColor + addLightColor);
        #endif
    #else
        col.rgb *= lightColor;
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Distance Fade
    BEFORE_DISTANCE_FADE
    #if defined(LIL_FEATURE_DISTANCE_FADE)
        OVERRIDE_DISTANCE_FADE
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Fog
    #if defined(LIL_HDRP)
        float3 viewDirection = normalize(LIL_GET_VIEWDIR_WS(input.positionWS.xyz));
    #endif
    BEFORE_FOG
    OVERRIDE_FOG

    BEFORE_OUTPUT
    OVERRIDE_OUTPUT
}

#endif