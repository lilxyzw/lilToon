#ifndef LIL_PASS_UNIVERSAL2D_INCLUDED
#define LIL_PASS_UNIVERSAL2D_INCLUDED

#include "Includes/lil_pipeline.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Struct
struct appdata
{
    float4 positionOS   : POSITION;
    float2 uv           : TEXCOORD0;
};

struct v2f
{
    float4 positionCS   : SV_POSITION;
    float2 uv           : TEXCOORD0;
};

//------------------------------------------------------------------------------------------------------------------------------
// Shader
v2f vert(appdata input)
{
    v2f output;
    LIL_INITIALIZE_STRUCT(v2f, output);

    if(_Invisible) return output;

    LIL_VERTEX_POSITION_INPUTS(input.positionOS, vertexInput);
    output.positionCS = vertexInput.positionCS;
    output.uv = input.uv;

    return output;
}

float4 frag(v2f input) : SV_Target
{
    float2 uvMain = lilCalcUV(input.uv, _MainTex_ST, _MainTex_ScrollRotate);
    float4 col = LIL_SAMPLE_2D(_MainTex, sampler_MainTex, uvMain) * _Color;
    #if LIL_RENDER == 1
        #ifdef LIL_LITE
            clip(col.a - _Cutoff);
        #else
            col.a = saturate((col.a - _Cutoff) / max(fwidth(col.a), 0.0001) + 0.5);
        #endif
    #else
        clip(col.a - _Cutoff);
    #endif
    return col;
}

#endif