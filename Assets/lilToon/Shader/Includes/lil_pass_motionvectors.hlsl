#ifndef LIL_PASS_MOTIONVECTOR_INCLUDED
#define LIL_PASS_MOTIONVECTOR_INCLUDED

#include "Includes/lil_pipeline.hlsl"
#include "Includes/lil_common_appdata.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Motion Vector
float2 lilCalculateMotionVector(float4 positionCS, float4 previousPositionCS)
{
    positionCS.xy = positionCS.xy / _ScreenParams.xy * 2.0 - 1.0;
    #if UNITY_UV_STARTS_AT_TOP
        positionCS.y = -positionCS.y;
    #endif
    previousPositionCS.xy = previousPositionCS.xy / previousPositionCS.w;
    float2 motionVec = (positionCS.xy - previousPositionCS.xy);

    float2 microThreshold = 0.01f * _ScreenSize.zw;
    motionVec.x = abs(motionVec.x) < microThreshold.x ? 0 : motionVec.x;
    motionVec.y = abs(motionVec.y) < microThreshold.y ? 0 : motionVec.y;

    motionVec = clamp(motionVec, -1.0f + microThreshold, 1.0f - microThreshold);

    #if UNITY_UV_STARTS_AT_TOP
        motionVec.y = -motionVec.y;
    #endif
    return motionVec;
}

//------------------------------------------------------------------------------------------------------------------------------
// Structure
#if !defined(LIL_CUSTOM_V2F_MEMBER)
    #define LIL_CUSTOM_V2F_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7)
#endif

#define LIL_V2F_POSITION_CS
#define LIL_V2F_PREV_POSITION_CS
#if defined(LIL_V2F_FORCE_TEXCOORD0) || (LIL_RENDER > 0)
    #define LIL_V2F_TEXCOORD0
#endif
#if defined(LIL_V2F_FORCE_POSITION_OS) || ((LIL_RENDER > 0) && !defined(LIL_LITE) && !defined(LIL_FUR) && defined(LIL_FEATURE_DISSOLVE))
    #define LIL_V2F_POSITION_OS
#endif
#if defined(LIL_V2F_FORCE_NORMAL) || defined(WRITE_NORMAL_BUFFER)
    #define LIL_V2F_NORMAL_WS
#endif
#if defined(LIL_FUR)
    #define LIL_V2F_FURLAYER
#endif

struct v2f
{
    float4 positionCS   : SV_POSITION;
    float4 previousPositionCS : TEXCOORD4;
    #if defined(LIL_V2F_TEXCOORD0)
        float2 uv0      : TEXCOORD0;
    #endif
    #if defined(LIL_V2F_POSITION_OS)
        float3 positionOS   : TEXCOORD1;
    #endif
    #if defined(LIL_V2F_NORMAL_WS)
        float3 normalWS     : TEXCOORD2;
    #endif
    #if defined(LIL_FUR)
        float furLayer      : TEXCOORD3;
    #endif
    LIL_CUSTOM_V2F_MEMBER(7,8,9,10,11,12,13,14)
    LIL_VERTEX_INPUT_INSTANCE_ID
    LIL_VERTEX_OUTPUT_STEREO
};

#if defined(LIL_FUR)
    #define LIL_V2G_TEXCOORD0
    #define LIL_V2G_POSITION_WS
    #if defined(LIL_V2G_FORCE_NORMAL_WS) || defined(WRITE_NORMAL_BUFFER)
        #define LIL_V2G_NORMAL_WS
    #endif
    #define LIL_V2G_FURVECTOR
    #define LIL_V2G_PREV_POSITION_WS

    struct v2g
    {
        float3 positionWS   : TEXCOORD0;
        float2 uv0          : TEXCOORD1;
        float3 furVector    : TEXCOORD2;
        #if defined(LIL_V2G_NORMAL_WS)
            float3 normalWS     : TEXCOORD3;
        #endif
        float3 previousPositionWS : TEXCOORD4;
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };
#elif defined(LIL_ONEPASS_OUTLINE)
    struct v2g
    {
        v2f base;
        float4 positionCSOL : TEXCOORD5;
        float4 previousPositionCSOL : TEXCOORD6;
    };
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Shader
#if defined(LIL_FUR)
    #include "Includes/lil_common_vert_fur.hlsl"
#else
    #include "Includes/lil_common_vert.hlsl"
#endif
#include "Includes/lil_common_frag.hlsl"

#if defined(WRITE_MSAA_DEPTH)
#define SV_TARGET_NORMAL SV_Target2
#else
#define SV_TARGET_NORMAL SV_Target1
#endif

void frag(v2f input
    LIL_VFACE(facing)
    #ifdef WRITE_MSAA_DEPTH
    , out float4 depthColor : SV_Target0
    , out float4 outMotionVector : SV_Target1
    #else
    , out float4 outMotionVector : SV_Target0
    #endif

    #ifdef WRITE_NORMAL_BUFFER
    , out float4 outNormalBuffer : SV_TARGET_NORMAL
    #endif
)
{
    lilFragData fd = lilInitFragData();

    BEFORE_UNPACK_V2F
    OVERRIDE_UNPACK_V2F
    LIL_COPY_VFACE(fd.facing);
    LIL_SETUP_INSTANCE_ID(input);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    #include "Includes/lil_common_frag_alpha.hlsl"

    float2 motionVector = lilCalculateMotionVector(input.positionCS, input.previousPositionCS);
    outMotionVector = float4(motionVector * 0.5, 0.0, 0.0);

    bool forceNoMotion = unity_MotionVectorsParams.y == 0.0;
    if(forceNoMotion) outMotionVector = float4(2.0, 0.0, 0.0, 0.0);

    #ifdef WRITE_MSAA_DEPTH
        depthColor = fd.positionCS.z;
        #ifdef _ALPHATOMASK_ON
            #if LIL_RENDER > 0
                depthColor.a = saturate((fd.col.a - _Cutoff) / max(fwidth(fd.col.a), 0.0001) + 0.5);
            #else
                depthColor.a = 1.0;
            #endif
        #endif
    #endif

    #if defined(WRITE_NORMAL_BUFFER)
        fd.N = normalize(input.normalWS);
        fd.N = facing < (_FlipNormal-1.0) ? -fd.N : fd.N;

        const float seamThreshold = 1.0 / 1024.0;
        fd.N.z = CopySign(max(seamThreshold, abs(fd.N.z)), fd.N.z);
        float2 octNormalWS = PackNormalOctQuadEncode(fd.N);
        float3 packNormalWS = PackFloat2To888(saturate(octNormalWS * 0.5 + 0.5));
        outNormalBuffer = float4(packNormalWS, 1.0);
    #endif
}

#if defined(LIL_TESSELLATION)
    #include "Includes/lil_tessellation.hlsl"
#endif

#endif