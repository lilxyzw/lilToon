#ifndef LIL_PASS_FORWARD_FUR_INCLUDED
#define LIL_PASS_FORWARD_FUR_INCLUDED

#include "Includes/lil_pipeline.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Structure
// v2g
#define LIL_V2G_TEXCOORD0
#define LIL_V2G_POSITION_WS
#if defined(LIL_V2G_FORCE_NORMAL_WS) || (!defined(LIL_PASS_FORWARDADD) && defined(LIL_SHOULD_NORMAL))
    #define LIL_V2G_NORMAL_WS
#endif
#if defined(LIL_V2G_FORCE_TEXCOORD1) || defined(LIL_USE_LIGHTMAP_UV)
    #define LIL_V2G_TEXCOORD1
#endif
#if !defined(LIL_PASS_FORWARDADD)
    #define LIL_V2G_LIGHTCOLOR
    #define LIL_V2G_LIGHTDIRECTION
    #define LIL_V2G_VERTEXLIGHT
    #if defined(LIL_FEATURE_SHADOW)
        #define LIL_V2G_INDLIGHTCOLOR
    #endif
#endif
#define LIL_V2G_FOG
#define LIL_V2G_FURVECTOR

// g2f
#define LIL_V2F_POSITION_CS
#define LIL_V2F_TEXCOORD0

#if defined(LIL_V2F_FORCE_TEXCOORD1) || defined(LIL_USE_LIGHTMAP_UV)
    #define LIL_V2F_TEXCOORD1
#endif

#if defined(LIL_V2F_FORCE_POSITION_WS) || defined(LIL_PASS_FORWARDADD) || defined(LIL_FEATURE_DISTANCE_FADE) || !defined(LIL_BRP) || defined(LIL_USE_LPPV)
    #define LIL_V2F_POSITION_WS
#endif

#if defined(LIL_V2F_FORCE_NORMAL_WS) || !defined(LIL_PASS_FORWARDADD) && defined(LIL_SHOULD_NORMAL)
    #define LIL_V2F_NORMAL_WS
#endif
#if !defined(LIL_PASS_FORWARDADD)
    #define LIL_V2F_LIGHTCOLOR
    #define LIL_V2F_LIGHTDIRECTION
    #define LIL_V2F_VERTEXLIGHT
    #if defined(LIL_FEATURE_SHADOW)
        #define LIL_V2F_INDLIGHTCOLOR
    #endif
#endif
#define LIL_V2F_FOG
#define LIL_V2F_FURLAYER

struct v2g
{
    float2 uv           : TEXCOORD0;
    #if defined(LIL_V2G_TEXCOORD1)
        float2 uv1          : TEXCOORD1;
    #endif
    float3 positionWS   : TEXCOORD2;
    float3 furVector    : TEXCOORD3;
    #if defined(LIL_V2G_NORMAL_WS)
        float3 normalWS     : TEXCOORD4;
    #endif
    LIL_LIGHTCOLOR_COORDS(5)
    LIL_LIGHTDIRECTION_COORDS(6)
    LIL_INDLIGHTCOLOR_COORDS(7)
    LIL_VERTEXLIGHT_COORDS(8)
    LIL_FOG_COORDS(9)
    LIL_VERTEX_INPUT_INSTANCE_ID
    LIL_VERTEX_OUTPUT_STEREO
};

struct v2f
{
    float4 positionCS   : SV_POSITION;
    float2 uv           : TEXCOORD0;
    #if defined(LIL_V2F_TEXCOORD1)
        float2 uv1              : TEXCOORD1;
    #endif
    #if defined(LIL_V2F_POSITION_WS)
        float3 positionWS       : TEXCOORD2;
    #endif
    #if defined(LIL_V2F_NORMAL_WS)
        float3 normalWS         : TEXCOORD3;
    #endif
    float furLayer          : TEXCOORD4;
    LIL_LIGHTCOLOR_COORDS(5)
    LIL_LIGHTDIRECTION_COORDS(6)
    LIL_INDLIGHTCOLOR_COORDS(7)
    LIL_VERTEXLIGHT_COORDS(8)
    LIL_FOG_COORDS(9)
    LIL_VERTEX_INPUT_INSTANCE_ID
    LIL_VERTEX_OUTPUT_STEREO
};

//------------------------------------------------------------------------------------------------------------------------------
// Shader
#include "Includes/lil_common_vert_fur.hlsl"
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
            float ln = dot(lightDirection, normalDirection);
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