

#ifndef LIL_COMMON_INCLUDED
#define LIL_COMMON_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Debug

// Show all warning
//#pragma warning(default: 3200 3201 3202 3203 3204 3205 3206 3207 3208 3209)
//#pragma warning(default: 3550 3551 3552 3553 3554 3555 3556 3557 3558 3559 3560 3561 3562 3563 3564 3565 3566 3567 3568 3569 3570 3571 3572 3573 3574 3575 3576 3577 3578 3579 3580 3581 3582 3583 3584 3585 3586 3587 3588)
//#pragma warning(default: 4700 4701 4702 4703 4704 4705 4706 4707 4708 4710 4711 4712 4713 4714 4715 4716 4717)
//#pragma warning(disable: 3571)

// Ignore unknown pragma (for old Unity version)
#pragma warning(disable: 3568)

//------------------------------------------------------------------------------------------------------------------------------
// Common

#if !defined(LIL_CUSTOM_SHADER) && !defined(LIL_LITE) && !defined(LIL_MULTI) && !defined(LIL_IGNORE_SHADERSETTING)
#include "../../../lilToonSetting/lil_setting.hlsl"
#endif
#include "Includes/lil_common_macro.hlsl"
#include "Includes/lil_common_input.hlsl"

// Omission of if statement
// lilToonMulti branches using shader keywords
#if defined(LIL_MULTI)
    #define _UseMain2ndTex true
    #define _UseMain3rdTex true
    #define _UseShadow true
    #define _UseBacklight true
    #define _UseBumpMap true
    #define _UseBump2ndMap true
    #define _UseAnisotropy true
    #define _UseReflection true
    #define _UseMatCap true
    #define _UseMatCap2nd true
    #define _UseRim true
    #define _UseGlitter true
    #define _UseEmission true
    #define _UseEmission2nd true
    #define _UseParallax true
    #define _UseAudioLink true
    #define _AudioLinkAsLocal true
    #undef LIL_BRANCH
    #define LIL_BRANCH
#endif

#include "Includes/lil_common_functions.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Structure for fragment shader
struct lilFragData
{
    // Color
    float4 col;
    float3 albedo;
    float3 emissionColor;

    // Light Color
    float3 lightColor;
    float3 indLightColor;
    float3 addLightColor;
    float attenuation;
    float3 invLighting;

    // UV
    float2 uv0;
    float2 uv1;
    float2 uv2;
    float2 uv3;
    float2 uvMain;
    float2 uvMat;
    float2 uvRim;
    float2 uvPanorama;
    float2 uvScn;
    bool isRightHand;

    // Position
    float3 positionOS;
    float3 positionWS;
    float4 positionCS;
    float4 positionSS;
    float depth;

    // Vector
    float3x3 TBN;
    float3 T;
    float3 B;
    float3 N;
    float3 V;
    float3 L;
    float3 origN;
    float3 origL;
    float3 headV;
    float3 reflectionN;
    float3 matcapN;
    float3 matcap2ndN;
    float facing;

    // Dot
    float vl;
    float hl;
    float ln;
    float nv;
    float nvabs;

    // Shading Data
    float4 triMask;
    float3 parallaxViewDirection;
    float2 parallaxOffset;
    float anisotropy;
    float smoothness;
    float roughness;
    float perceptualRoughness;
    float shadowmix;
    float audioLinkValue;

    // HDRP Data
    uint renderingLayers;
    uint featureFlags;
    uint2 tileIndex;
};

lilFragData lilInitFragData()
{
    lilFragData fd;

    fd.col = 1.0;
    fd.albedo = 1.0;
    fd.emissionColor = 0.0;

    fd.lightColor = 1.0;
    fd.indLightColor = 0.0;
    fd.addLightColor = 0.0;
    fd.attenuation = 1.0;
    fd.invLighting = 0.0;

    fd.uv0 = 0.0;
    fd.uv1 = 0.0;
    fd.uv2 = 0.0;
    fd.uv3 = 0.0;
    fd.uvMain = 0.0;
    fd.uvMat = 0.0;
    fd.uvRim = 0.0;
    fd.uvPanorama = 0.0;
    fd.uvScn = 0.0;
    fd.isRightHand = true;

    fd.positionOS = 0.0;
    fd.positionWS = 0.0;
    fd.positionCS = 0.0;
    fd.positionSS = 0.0;
    fd.depth = 0.0;

    fd.TBN = float3x3(
        1.0,0.0,0.0,
        0.0,1.0,0.0,
        0.0,0.0,1.0);
    fd.T = 0.0;
    fd.B = 0.0;
    fd.N = 0.0;
    fd.V = 0.0;
    fd.L = float3(0.0, 1.0, 0.0);
    fd.origN = 0.0;
    fd.origL = float3(0.0, 1.0, 0.0);
    fd.headV = 0.0;
    fd.reflectionN = 0.0;
    fd.matcapN = 0.0;
    fd.matcap2ndN = 0.0;
    fd.facing = 1.0;

    fd.vl = 0.0;
    fd.hl = 0.0;
    fd.ln = 0.0;
    fd.nv = 0.0;
    fd.nvabs = 0.0;

    fd.triMask = 1.0;
    fd.parallaxViewDirection = 0.0;
    fd.parallaxOffset = 0.0;
    fd.anisotropy = 0.0;
    fd.smoothness = 1.0;
    fd.roughness = 1.0;
    fd.perceptualRoughness = 1.0;
    fd.shadowmix = 1.0;
    fd.audioLinkValue = 1.0;

    fd.renderingLayers = 0;
    fd.featureFlags = 0;
    fd.tileIndex = 0;

    return fd;
}

#endif