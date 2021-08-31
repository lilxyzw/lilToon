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
    #if !defined(LIL_LITE) && defined(LIL_FEATURE_ENCRYPTION)
        float2 uv6          : TEXCOORD6;
        float2 uv7          : TEXCOORD7;
    #endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
    float4 positionCS   : SV_POSITION;
    #if LIL_RENDER > 0
        float2 uv           : TEXCOORD0;
        #if !defined(LIL_LITE) && !defined(LIL_FUR) && defined(LIL_FEATURE_DISSOLVE)
            float3 positionOS   : TEXCOORD1;
        #endif
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

    LIL_BRANCH
    if(_Invisible) return output;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    //----------------------------------------------------------------------------------------------------------------------
    // Encryption
    #if !defined(LIL_LITE) && defined(LIL_FEATURE_ENCRYPTION)
        input.positionOS = vertexDecode(input.positionOS, input.normalOS, input.uv6, input.uv7);
    #endif

    LIL_VERTEX_POSITION_INPUTS(input.positionOS, vertexInput);

    #if defined(LIL_OUTLINE)
        LIL_VERTEX_NORMAL_INPUTS(input.normalOS, vertexNormalInput);
        float2 uvMain = input.uv * _MainTex_ST.xy + _MainTex_ST.zw;
        float outlineWidth = _OutlineWidth * 0.01;
        if(Exists_OutlineWidthMask) outlineWidth *= LIL_SAMPLE_2D_LOD(_OutlineWidthMask, sampler_MainTex, uvMain, 0).r;
        if(_OutlineVertexR2Width) outlineWidth *= input.color.r;
        if(_OutlineFixWidth) outlineWidth *= saturate(length(LIL_GET_VIEWDIR_WS(vertexInput.positionWS)));
        vertexInput.positionWS += vertexNormalInput.normalWS * outlineWidth;
        output.positionCS = LIL_TRANSFORM_POS_WS_TO_CS(vertexInput.positionWS);
    #else
        output.positionCS = vertexInput.positionCS;
    #endif
    #if LIL_RENDER > 0
        output.uv = input.uv;
        #if !defined(LIL_LITE) && !defined(LIL_FUR) && defined(LIL_FEATURE_DISSOLVE)
            output.positionOS = input.positionOS.xyz;
        #endif
    #endif

    return output;
}

float4 frag(v2f input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    #if LIL_RENDER > 0
        #if defined(LIL_FEATURE_ANIMATE_MAIN_UV)
            float2 uvMain = lilCalcUV(input.uv, _MainTex_ST, _MainTex_ScrollRotate);
        #else
            float2 uvMain = lilCalcUV(input.uv, _MainTex_ST);
        #endif

        //--------------------------------------------------------------------------------------------------------------------------
        // Main Color
        float alpha = _Color.a;
        if(Exists_MainTex) alpha *= LIL_SAMPLE_2D(_MainTex, sampler_MainTex, uvMain).a;

        //----------------------------------------------------------------------------------------------------------------------
        // Dissolve
        #if !defined(LIL_LITE) && !defined(LIL_FUR) && defined(LIL_FEATURE_DISSOLVE)
            float dissolveAlpha = 0.0;
            #if defined(LIL_FEATURE_TEX_DISSOLVE_NOISE)
                lilCalcDissolveWithNoise(
                    alpha,
                    dissolveAlpha,
                    input.uv,
                    input.positionOS,
                    _DissolveParams,
                    _DissolvePos,
                    _DissolveMask,
                    _DissolveMask_ST,
                    _DissolveNoiseMask,
                    _DissolveNoiseMask_ST,
                    _DissolveNoiseMask_ScrollRotate,
                    _DissolveNoiseStrength,
                    sampler_MainTex
                );
            #else
                lilCalcDissolve(
                    alpha,
                    dissolveAlpha,
                    input.uv,
                    input.positionOS,
                    _DissolveParams,
                    _DissolvePos,
                    _DissolveMask,
                    _DissolveMask_ST,
                    sampler_MainTex
                );
            #endif
        #endif

        //----------------------------------------------------------------------------------------------------------------------
        // Alpha Mask
        #if !defined(LIL_LITE) && !defined(LIL_FUR) && defined(LIL_FEATURE_ALPHAMASK)
            if(_AlphaMaskMode)
            {
                float alphaMask = LIL_SAMPLE_2D(_AlphaMask, sampler_MainTex, uvMain).r;
                alphaMask = saturate(alphaMask + _AlphaMaskValue);
                alpha = _AlphaMaskMode == 1 ? alphaMask : alpha * alphaMask;
            }
        #endif

        clip(alpha - _Cutoff);
        #if LIL_RENDER == 2
            half alphaRef = LIL_SAMPLE_3D(_DitherMaskLOD, sampler_DitherMaskLOD, float3(input.positionCS.xy*0.25,alpha*0.9375)).a;
            clip(alphaRef - 0.01);
        #endif
    #endif
    return 0;
}

#endif