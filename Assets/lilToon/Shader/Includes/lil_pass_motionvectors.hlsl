#ifndef LIL_PASS_MOTIONVECTOR_INCLUDED
#define LIL_PASS_MOTIONVECTOR_INCLUDED

#include "lil_common.hlsl"
#include "lil_common_appdata.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Structure
#if !defined(LIL_CUSTOM_V2F_MEMBER)
    #define LIL_CUSTOM_V2F_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7)
#endif

#define LIL_V2F_POSITION_CS
#define LIL_V2F_PREV_POSITION_CS
#if defined(LIL_URP) && LIL_SRP_VERSION_GREATER_EQUAL(16, 0)
    #define LIL_V2F_POSITION_CS_NO_JITTER
#endif
#if defined(LIL_V2F_FORCE_TEXCOORD0) || (LIL_RENDER > 0)
    #if defined(LIL_FUR)
        #define LIL_V2F_TEXCOORD0
    #else
        #define LIL_V2F_PACKED_TEXCOORD01
        #define LIL_V2F_PACKED_TEXCOORD23
    #endif
#endif
#if defined(LIL_V2F_FORCE_POSITION_OS) || ((LIL_RENDER > 0) && !defined(LIL_LITE) && defined(LIL_FEATURE_DISSOLVE))
    #define LIL_V2F_POSITION_OS
#endif
#if defined(LIL_V2F_FORCE_POSITION_WS) || (LIL_RENDER > 0) && defined(LIL_FEATURE_DISTANCE_FADE)
    #define LIL_V2F_POSITION_WS
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
    float4 previousPositionCS : TEXCOORD0;
    #if defined(LIL_V2F_POSITION_CS_NO_JITTER)
        float4 positionCSNoJitter : POSITION_CS_NO_JITTER;
    #endif
    #if defined(LIL_V2F_TEXCOORD0)
        float2 uv0         : TEXCOORD1;
    #endif
    #if defined(LIL_V2F_PACKED_TEXCOORD01)
        float4 uv01         : TEXCOORD1;
    #endif
    #if defined(LIL_V2F_PACKED_TEXCOORD23)
        float4 uv23         : TEXCOORD2;
    #endif
    #if defined(LIL_V2F_POSITION_OS)
        float4 positionOSdissolve   : TEXCOORD3;
    #endif
    #if defined(LIL_V2F_POSITION_WS)
        float3 positionWS   : TEXCOORD4;
    #endif
    #if defined(LIL_V2F_NORMAL_WS)
        float3 normalWS     : TEXCOORD5;
    #endif
    #if defined(LIL_FUR)
        float furLayer      : TEXCOORD6;
    #endif
    LIL_CUSTOM_V2F_MEMBER(9,10,11,12,13,14,15,16)
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
    #define LIL_V2G_VERTEXID
    #define LIL_V2G_PREV_POSITION_WS

    struct v2g
    {
        float3 positionWS   : TEXCOORD0;
        float2 uv0          : TEXCOORD1;
        float3 furVector    : TEXCOORD2;
        uint vertexID       : TEXCOORD3;
        #if defined(LIL_V2G_NORMAL_WS)
            float3 normalWS     : TEXCOORD4;
        #endif
        float3 previousPositionWS : TEXCOORD5;
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };
#elif defined(LIL_ONEPASS_OUTLINE)
    struct v2g
    {
        v2f base;
        float4 positionCSOL : TEXCOORD7;
        float4 previousPositionCSOL : TEXCOORD8;
    };
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Shader
#if defined(LIL_FUR)
    #include "lil_common_vert_fur.hlsl"
#else
    #include "lil_common_vert.hlsl"
#endif
#include "lil_common_frag.hlsl"

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
    LIL_SETUP_INSTANCE_ID(input);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    lilFragData fd = lilInitFragData();

    BEFORE_UNPACK_V2F
    OVERRIDE_UNPACK_V2F
    LIL_COPY_VFACE(fd.facing);

    #include "lil_common_frag_alpha.hlsl"

    #if defined(LIL_V2F_POSITION_CS_NO_JITTER)
        float2 motionVector = lilCalculateMotionVector(input.positionCSNoJitter, input.previousPositionCS);
    #else
        float2 motionVector = lilCalculateMotionVector(input.positionCS, input.previousPositionCS);
    #endif
    outMotionVector = float4(motionVector, 0.0, 0.0);

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
        fd.N = fd.facing < (_FlipNormal-1.0) ? -fd.N : fd.N;

        const float seamThreshold = 1.0 / 1024.0;
        fd.N.z = CopySign(max(seamThreshold, abs(fd.N.z)), fd.N.z);
        float2 octNormalWS = PackNormalOctQuadEncode(fd.N);
        float3 packNormalWS = PackFloat2To888(saturate(octNormalWS * 0.5 + 0.5));
        outNormalBuffer = float4(packNormalWS, 1.0);
    #endif
}

#if defined(LIL_TESSELLATION)
    #include "lil_tessellation.hlsl"
#endif

#endif