#ifndef LIL_PASS_FORWARD_FUR_INCLUDED
#define LIL_PASS_FORWARD_FUR_INCLUDED

#include "Includes/lil_pipeline.hlsl"
#include "Includes/lil_common_appdata.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Structure
#if !defined(LIL_CUSTOM_V2F_MEMBER)
    #define LIL_CUSTOM_V2F_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7)
#endif

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
    LIL_CUSTOM_V2F_MEMBER(10,11,12,13,14,15,16,17)
    LIL_VERTEX_INPUT_INSTANCE_ID
    LIL_VERTEX_OUTPUT_STEREO
};

//------------------------------------------------------------------------------------------------------------------------------
// Shader
#include "Includes/lil_common_vert_fur.hlsl"
#include "Includes/lil_common_frag.hlsl"

float4 frag(v2f input) : SV_Target
{
    //------------------------------------------------------------------------------------------------------------------------------
    // Initialize
    float3 lightDirection = float3(0.0, 1.0, 0.0);
    float3 lightColor = 1.0;
    float3 addLightColor = 0.0;
    float attenuation = 1.0;

    float4 col = 1.0;
    float3 albedo = 1.0;
    float3 emissionColor = 0.0;

    float3 normalDirection = 0.0;
    float3 viewDirection = 0.0;
    float3 headDirection = 0.0;
    float3x3 tbnWS = 0.0;
    float depth = 0.0;
    float3 parallaxViewDirection = 0.0;
    float2 parallaxOffset = 0.0;

    float vl = 0.0;
    float hl = 0.0;
    float ln = 0.0;
    float nv = 0.0;
    float nvabs = 0.0;

    bool isRightHand = true;
    float shadowmix = 1.0;
    float audioLinkValue = 1.0;
    float3 invLighting = 0.0;

    float facing = 1.0;
    LIL_SETUP_INSTANCE_ID(input);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    LIL_GET_HDRPDATA(input);
    #if defined(LIL_V2F_LIGHTDIRECTION)
        lightDirection = input.lightDirection;
    #endif
    LIL_GET_MAINLIGHT(input, lightColor, lightDirection, attenuation);
    LIL_GET_ADDITIONALLIGHT(input, addLightColor);
    invLighting = saturate((1.0 - lightColor) * sqrt(lightColor));

    //------------------------------------------------------------------------------------------------------------------------------
    // View Direction
    #if defined(LIL_V2F_POSITION_WS)
        depth = length(lilViewDirection(input.positionWS));
        viewDirection = normalize(lilViewDirection(input.positionWS));
        headDirection = normalize(lilHeadDirection(input.positionWS));
        vl = dot(viewDirection, lightDirection);
        hl = dot(headDirection, lightDirection);
    #endif
    #if defined(LIL_V2F_NORMAL_WS) && defined(LIL_V2F_TANGENT_WS) && defined(LIL_V2F_BITANGENT_WS)
        tbnWS = float3x3(input.tangentWS.xyz, input.bitangentWS, input.normalWS);
        #if defined(LIL_V2F_POSITION_WS)
            parallaxViewDirection = mul(tbnWS, viewDirection);
            parallaxOffset = (parallaxViewDirection.xy / (parallaxViewDirection.z+0.5));
        #endif
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // UV
    BEFORE_ANIMATE_MAIN_UV
    OVERRIDE_ANIMATE_MAIN_UV

    //------------------------------------------------------------------------------------------------------------------------------
    // Main Color
    BEFORE_MAIN
    OVERRIDE_MAIN

    //------------------------------------------------------------------------------------------------------------------------------
    // Fur
    BEFORE_FUR
    OVERRIDE_FUR

    //------------------------------------------------------------------------------------------------------------------------------
    // Copy
    albedo = col.rgb;

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
            normalDirection = normalize(input.normalWS);
            ln = dot(lightDirection, normalDirection);
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
    BEFORE_FOG
    OVERRIDE_FOG

    BEFORE_OUTPUT
    OVERRIDE_OUTPUT
}

#endif