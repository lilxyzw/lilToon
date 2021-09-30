#ifndef LIL_PASS_DEPTHNORMAL_INCLUDED
#define LIL_PASS_DEPTHNORMAL_INCLUDED

#include "Includes/lil_pipeline.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Structure
#define LIL_V2F_POSITION_CS
#define LIL_V2F_NORMAL_WS
#if defined(LIL_V2F_FORCE_UV0) || (LIL_RENDER > 0)
    #define LIL_V2F_TEXCOORD0
#endif
#if defined(LIL_V2F_FORCE_POSITION_OS) || ((LIL_RENDER > 0) && !defined(LIL_LITE) && !defined(LIL_FUR) && defined(LIL_FEATURE_DISSOLVE))
    #define LIL_V2F_POSITION_OS
#endif

struct v2f
{
    float4 positionCS   : SV_POSITION;
    float3 normalWS     : TEXCOORD0;
    #if defined(LIL_V2F_TEXCOORD0)
        float2 uv       : TEXCOORD1;
    #endif
    #if defined(LIL_V2F_POSITION_OS)
        float3 positionOS   : TEXCOORD2;
    #endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

//------------------------------------------------------------------------------------------------------------------------------
// Shader
#define LIL_NORMALIZE_NORMAL_IN_VS
#include "Includes/lil_common_vert.hlsl"
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

    #include "Includes/lil_common_frag_alpha.hlsl"

    return float4(PackNormalOctRectEncode(normalize(mul((float3x3)LIL_MATRIX_V, input.normalWS))), 0.0, 0.0);
}

#endif