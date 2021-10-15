
#ifndef LIL_PASS_FORWARD_REFLACTION_BLUR_INCLUDED
#define LIL_PASS_FORWARD_REFLACTION_BLUR_INCLUDED

#include "Includes/lil_pipeline.hlsl"
#include "Includes/lil_common_appdata.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Structure
#if !defined(LIL_CUSTOM_V2F_MEMBER)
    #define LIL_CUSTOM_V2F_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7)
#endif

#define LIL_V2F_POSITION_CS
#define LIL_V2F_TEXCOORD0
#define LIL_V2F_POSITION_SS

struct v2f
{
    float4 positionCS       : SV_POSITION;
    float2 uv               : TEXCOORD0;
    float4 positionSS       : TEXCOORD1;
    LIL_CUSTOM_V2F_MEMBER(2,3,4,5,6,7,8,9)
    LIL_VERTEX_INPUT_INSTANCE_ID
    LIL_VERTEX_OUTPUT_STEREO
};

//------------------------------------------------------------------------------------------------------------------------------
// Shader
#include "Includes/lil_common_vert.hlsl"
#include "Includes/lil_common_frag.hlsl"

float4 frag(v2f input LIL_VFACE(facing)) : SV_Target
{
    LIL_VFACE_FALLBACK(facing);
    LIL_SETUP_INSTANCE_ID(input);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    #if defined(LIL_FEATURE_REFLECTION)
        BEFORE_ANIMATE_MAIN_UV
        OVERRIDE_ANIMATE_MAIN_UV
        float2 scnUV = input.positionSS.xy/input.positionSS.w;
        float3 refractCol = 0;
        float sum = 0;
        float smoothness = _Smoothness;
        if(Exists_SmoothnessTex) smoothness *= LIL_SAMPLE_2D(_SmoothnessTex, sampler_linear_repeat, uvMain).r;
        float perceptualRoughness = 1.0 - smoothness;
        float roughness = perceptualRoughness * perceptualRoughness;
        float blurOffset = perceptualRoughness / input.positionSS.z * _lilBackgroundTexture_TexelSize.x / _lilBackgroundTexture_TexelSize.y * (0.0005 / LIL_REFRACTION_SAMPNUM);
        for(int j = -LIL_REFRACTION_SAMPNUM; j <= LIL_REFRACTION_SAMPNUM; j++)
        {
            refractCol += LIL_SAMPLE_2D(_lilBackgroundTexture, sampler_lilBackgroundTexture, scnUV + float2(j*blurOffset,0)).rgb * LIL_REFRACTION_GAUSDIST(j);
            sum += LIL_REFRACTION_GAUSDIST(j);
        }
        refractCol /= sum;
        return float4(refractCol,1.0);
    #else
        float2 scnUV = input.positionSS.xy/input.positionSS.w;
        float3 refractCol = LIL_SAMPLE_2D(_lilBackgroundTexture, sampler_lilBackgroundTexture, scnUV).rgb;
        return float4(refractCol,1.0);
    #endif
}

#endif