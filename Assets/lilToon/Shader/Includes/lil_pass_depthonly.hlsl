#ifndef LIL_PASS_DEPTHONLY_INCLUDED
#define LIL_PASS_DEPTHONLY_INCLUDED

#include "Includes/lil_pipeline.hlsl"
#include "Includes/lil_common_appdata.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Structure
#if !defined(LIL_CUSTOM_V2F_MEMBER)
    #define LIL_CUSTOM_V2F_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7)
#endif

#define LIL_V2F_POSITION_CS
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
    #if defined(LIL_V2F_TEXCOORD0)
        float2 uv0          : TEXCOORD0;
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
    LIL_CUSTOM_V2F_MEMBER(4,5,6,7,8,9,10,11)
    LIL_VERTEX_INPUT_INSTANCE_ID
    LIL_VERTEX_OUTPUT_STEREO
};

#if defined(LIL_FUR) && defined(LIL_HDRP)
    #define LIL_V2G_TEXCOORD0
    #define LIL_V2G_POSITION_WS
    #if defined(LIL_V2G_FORCE_NORMAL_WS) || defined(WRITE_NORMAL_BUFFER)
        #define LIL_V2G_NORMAL_WS
    #endif
    #define LIL_V2G_FURVECTOR

    struct v2g
    {
        float3 positionWS   : TEXCOORD0;
        float2 uv0          : TEXCOORD1;
        float3 furVector    : TEXCOORD2;
        #if defined(LIL_V2G_NORMAL_WS)
            float3 normalWS     : TEXCOORD3;
        #endif
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };
#elif defined(LIL_ONEPASS_OUTLINE)
    struct v2g
    {
        v2f base;
        float4 positionCSOL : TEXCOORD3;
    };
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Shader
#if defined(LIL_FUR) && defined(LIL_HDRP)
    #include "Includes/lil_common_vert_fur.hlsl"
#else
    #include "Includes/lil_common_vert.hlsl"
#endif
#include "Includes/lil_common_frag.hlsl"

void frag(v2f input
    LIL_VFACE(facing)
    #if defined(SCENESELECTIONPASS) || defined(SCENEPICKINGPASS) || !defined(LIL_HDRP)
    , out float4 outColor : SV_Target0
    #else
        #ifdef WRITE_MSAA_DEPTH
        , out float4 depthColor : SV_Target0
            #ifdef WRITE_NORMAL_BUFFER
            , out float4 outNormalBuffer : SV_Target1
            #endif
        #else
            #ifdef WRITE_NORMAL_BUFFER
            , out float4 outNormalBuffer : SV_Target0
            #endif
        #endif
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

    #if !defined(LIL_HDRP)
        outColor = 0;
    #elif defined(SCENESELECTIONPASS)
        outColor = float4(_ObjectId, _PassValue, 1.0, 1.0);
    #elif defined(SCENEPICKINGPASS)
        outColor = _SelectionID;
    #else
        #ifdef WRITE_MSAA_DEPTH
            depthColor = input.positionCS.z;
            #ifdef _ALPHATOMASK_ON
                #if LIL_RENDER > 0
                    depthColor.a = saturate((alpha - _Cutoff) / max(fwidth(alpha), 0.0001) + 0.5);
                #else
                    depthColor.a = 1.0;
                #endif
            #endif
        #endif

        #if defined(WRITE_NORMAL_BUFFER)
            float3 normalDirection = normalize(input.normalWS);
            normalDirection = facing < (_FlipNormal-1.0) ? -normalDirection : normalDirection;

            const float seamThreshold = 1.0 / 1024.0;
            normalDirection.z = CopySign(max(seamThreshold, abs(normalDirection.z)), normalDirection.z);
            float2 octNormalWS = PackNormalOctQuadEncode(normalDirection);
            float3 packNormalWS = PackFloat2To888(saturate(octNormalWS * 0.5 + 0.5));
            outNormalBuffer = float4(packNormalWS, 1.0);
        #endif
    #endif
}

#if defined(LIL_TESSELLATION)
    #include "Includes/lil_tessellation.hlsl"
#endif

#endif