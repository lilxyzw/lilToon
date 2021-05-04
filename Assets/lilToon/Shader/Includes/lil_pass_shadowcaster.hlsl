#ifndef LIL_PASS_SHADOWCASTER_INCLUDED
#define LIL_PASS_SHADOWCASTER_INCLUDED

#include "Includes/lil_pipeline.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Struct
struct appdata
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    #if LIL_RENDER > 0
        float2 uv           : TEXCOORD0;
    #endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
    LIL_V2F_SHADOW_CASTER
    #if LIL_RENDER > 0
        float2 uv       : TEXCOORD0;
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

    LIL_TRANSFER_SHADOW_CASTER(input,output);
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
            half alphaRef = LIL_SAMPLE_3D(_DitherMaskLOD, sampler_DitherMaskLOD, float3(input.positionCS.xy*0.25,alpha*0.9375)).a;
            clip(alphaRef - 0.01);
        #endif
    #endif

    LIL_SHADOW_CASTER_FRAGMENT(input);
}

#endif