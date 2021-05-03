
// lil's Toon Shader
// Copyright (c) 2020 lilxyzw
// This code is under MIT licence, see LICENSE
// https://github.com/lilxyzw/lil-s-Toon-Shader/blob/main/LICENSE

#ifndef LIL_PASS_REFLACTION_BLUR_INCLUDED
#define LIL_PASS_REFLACTION_BLUR_INCLUDED

#include "Includes/lil_pipeline.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Struct
struct appdata
{
    float4 positionOS       : POSITION;
    float2 uv               : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
    float4 positionCS       : SV_POSITION;
    float2 uv               : TEXCOORD0;
    float4 positionSS       : TEXCOORD1;
    UNITY_VERTEX_OUTPUT_STEREO
};

//------------------------------------------------------------------------------------------------------------------------------
// Shader
v2f vert(appdata input)
{
    LIL_VERTEX_POSITION_INPUTS(input.positionOS, vertexInput);
    v2f output;
    LIL_INITIALIZE_STRUCT(v2f, output);
    if(_Invisible) return output;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
    output.uv           = input.uv;
    output.positionCS   = vertexInput.positionCS;
    output.positionSS   = vertexInput.positionSS;
    return output;
}

float4 frag(v2f input) : SV_Target
{
    float2 uvMain = lilCalcUV(input.uv, _MainTex_ST, _MainTex_ScrollRotate);
    float2 scnUV = input.positionSS.xy/input.positionSS.w;
    float3 refractCol = 0;
    float sum = 0;
    float smoothness = LIL_SAMPLE_2D(_SmoothnessTex, sampler_linear_repeat, uvMain).r * _Smoothness;
    float perceptualRoughness = 1.0 - smoothness;
    float roughness = perceptualRoughness * perceptualRoughness;
    float blurOffset = perceptualRoughness / input.positionSS.z * _GrabTexture_TexelSize.x / _GrabTexture_TexelSize.y * (0.0005 / LIL_REFRACTION_SAMPNUM);
    for(int j = -LIL_REFRACTION_SAMPNUM; j <= LIL_REFRACTION_SAMPNUM; j++)
    {
        refractCol += LIL_SAMPLE_2D(_GrabTexture, sampler_GrabTexture, scnUV + float2(j*blurOffset,0)).rgb * LIL_REFRACTION_GAUSDIST(j);
        sum += LIL_REFRACTION_GAUSDIST(j);
    }
    refractCol /= sum;
    return float4(refractCol,1);
}

#endif