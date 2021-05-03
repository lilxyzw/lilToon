// lil's Toon Shader
// Copyright (c) 2020 lilxyzw
// This code is under MIT licence, see LICENSE
// https://github.com/lilxyzw/lil-s-Toon-Shader/blob/main/LICENSE

#ifndef LIL_PASS_DEPTHONLY_INCLUDED
#define LIL_PASS_DEPTHONLY_INCLUDED

#include "Includes/lil_pipeline.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Struct
struct appdata
{
    float4 positionOS   : POSITION;
    #if defined(LIL_OUTLINE)
        float3 normalOS     : NORMAL;
    #endif
    #if LIL_RENDER > 0 || defined(LIL_OUTLINE)
        float2 uv           : TEXCOORD0;
    #endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
    float4 positionCS   : SV_POSITION;
    #if LIL_RENDER > 0
        float2 uv           : TEXCOORD0;
    #endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

//------------------------------------------------------------------------------------------------------------------------------
// Shader
v2f vert(appdata input)
{
    v2f output;
    LIL_INITIALIZE_STRUCT(v2f, output);

    if(_Invisible) return output;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    LIL_VERTEX_POSITION_INPUTS(input.positionOS, vertexInput);

    #if defined(LIL_OUTLINE)
        LIL_VERTEX_NORMAL_INPUTS(input.normalOS, vertexNormalInput);
        float2 uvMain = input.uv * _MainTex_ST.xy + _MainTex_ST.zw;
        _OutlineWidth *= LIL_SAMPLE_2D_LOD(_OutlineWidthMask, sampler_MainTex, uvMain, 0).r * 0.01;
        vertexInput.positionWS += vertexNormalInput.normalWS * _OutlineWidth;
        output.positionCS = LIL_TRANSFORM_POS_WS_TO_CS(vertexInput.positionWS);
    #else
        output.positionCS = vertexInput.positionCS;
    #endif
    #if LIL_RENDER > 0
        output.uv = input.uv;
    #endif

    return output;
}

float4 frag(v2f input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    #if LIL_RENDER > 0
        float2 uvMain = lilCalcUV(input.uv, _MainTex_ST, _MainTex_ScrollRotate);
        float alpha = LIL_SAMPLE_2D(_MainTex, sampler_MainTex, uvMain).a * _Color.a;
        #if LIL_RENDER == 1
            clip(alpha - _Cutoff);
        #else
            clip(alpha - 0.5);
        #endif
    #endif
    return 0;
}

#endif