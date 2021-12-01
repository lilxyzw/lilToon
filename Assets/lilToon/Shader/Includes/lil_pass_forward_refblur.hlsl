
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

struct v2f
{
    float4 positionCS   : SV_POSITION;
    float2 uv0          : TEXCOORD0;
    LIL_CUSTOM_V2F_MEMBER(1,2,3,4,5,6,7,8)
    LIL_VERTEX_INPUT_INSTANCE_ID
    LIL_VERTEX_OUTPUT_STEREO
};

//------------------------------------------------------------------------------------------------------------------------------
// Shader
#include "Includes/lil_common_vert.hlsl"
#include "Includes/lil_common_frag.hlsl"

float4 frag(v2f input LIL_VFACE(facing)) : SV_Target
{
    lilFragData fd = lilInitFragData();

    BEFORE_UNPACK_V2F
    OVERRIDE_UNPACK_V2F
    LIL_COPY_VFACE(fd.facing);
    LIL_SETUP_INSTANCE_ID(input);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    #if defined(LIL_FEATURE_REFLECTION)
        BEFORE_ANIMATE_MAIN_UV
        OVERRIDE_ANIMATE_MAIN_UV
        float3 refractCol = 0;
        float sum = 0;
        fd.smoothness = _Smoothness;
        if(Exists_SmoothnessTex) fd.smoothness *= LIL_SAMPLE_2D_ST(_SmoothnessTex, sampler_linear_repeat, fd.uvMain).r;
        float perceptualRoughness = 1.0 - fd.smoothness;
        float roughness = perceptualRoughness * perceptualRoughness;
        float blurOffset = perceptualRoughness / fd.positionSS.z * _lilBackgroundTexture_TexelSize.x / _lilBackgroundTexture_TexelSize.y * (0.0005 / LIL_REFRACTION_SAMPNUM);
        for(int j = -LIL_REFRACTION_SAMPNUM; j <= LIL_REFRACTION_SAMPNUM; j++)
        {
            refractCol += LIL_SAMPLE_2D(_lilBackgroundTexture, sampler_lilBackgroundTexture, fd.uvScn + float2(j*blurOffset,0)).rgb * LIL_REFRACTION_GAUSDIST(j);
            sum += LIL_REFRACTION_GAUSDIST(j);
        }
        refractCol /= sum;
        return float4(refractCol,1.0);
    #else
        float3 refractCol = LIL_SAMPLE_2D(_lilBackgroundTexture, sampler_lilBackgroundTexture, fd.uvScn).rgb;
        return float4(refractCol,1.0);
    #endif
}

#endif