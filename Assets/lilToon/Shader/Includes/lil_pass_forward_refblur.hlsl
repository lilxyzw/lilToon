
#ifndef LIL_PASS_FORWARD_REFLACTION_BLUR_INCLUDED
#define LIL_PASS_FORWARD_REFLACTION_BLUR_INCLUDED

#include "lil_common.hlsl"
#include "lil_common_appdata.hlsl"

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
#include "lil_common_vert.hlsl"
#include "lil_common_frag.hlsl"

float4 frag(v2f input LIL_VFACE(facing)) : SV_Target
{
    LIL_SETUP_INSTANCE_ID(input);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    lilFragData fd = lilInitFragData();

    BEFORE_UNPACK_V2F
    OVERRIDE_UNPACK_V2F
    LIL_COPY_VFACE(fd.facing);

    BEFORE_ANIMATE_MAIN_UV
    OVERRIDE_ANIMATE_MAIN_UV
    float3 refractCol = 0;
    float sum = 0;
    fd.smoothness = _Smoothness;
    #if defined(LIL_FEATURE_SmoothnessTex)
        fd.smoothness *= LIL_SAMPLE_2D_ST(_SmoothnessTex, lil_sampler_linear_repeat, fd.uvMain).r;
    #endif
    float perceptualRoughness = 1.0 - fd.smoothness;
    float roughness = perceptualRoughness * perceptualRoughness;
    #if !defined(LIL_LWTEX)
        float2 bgRes = lilGetWidthAndHeight(_lilBackgroundTexture);
        float aspect = bgRes.y / bgRes.x;
    #else
        float aspect = _lilBackgroundTexture_TexelSize.x * _lilBackgroundTexture_TexelSize.w;
    #endif
    float blurOffset = perceptualRoughness / sqrt(fd.positionSS.w) * aspect * (0.03 / LIL_REFRACTION_SAMPNUM) * LIL_MATRIX_P._m11;
    for(int j = -LIL_REFRACTION_SAMPNUM; j <= LIL_REFRACTION_SAMPNUM; j++)
    {
        refractCol += LIL_GET_BG_TEX(fd.uvScn + float2(j*blurOffset,0), 0).rgb * LIL_REFRACTION_GAUSDIST(j);
        sum += LIL_REFRACTION_GAUSDIST(j);
    }
    refractCol /= sum;
    return float4(refractCol,1.0);
}

#endif