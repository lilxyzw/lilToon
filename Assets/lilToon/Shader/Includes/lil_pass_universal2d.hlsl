#ifndef LIL_PASS_UNIVERSAL2D_INCLUDED
#define LIL_PASS_UNIVERSAL2D_INCLUDED

#include "Includes/lil_pipeline.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Struct
#define LIL_V2F_POSITION_CS
#define LIL_V2F_TEXCOORD0

struct v2f
{
    float4 positionCS   : SV_POSITION;
    float2 uv           : TEXCOORD0;
};

//------------------------------------------------------------------------------------------------------------------------------
// Shader
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
    BEFORE_ANIMATE_MAIN_UV
    OVERRIDE_ANIMATE_MAIN_UV

    float4 col = 1.0;
    BEFORE_MAIN
    OVERRIDE_MAIN
    #if LIL_RENDER == 1
        #ifdef LIL_LITE
            clip(col.a - _Cutoff);
        #else
            col.a = saturate((col.a - _Cutoff) / max(fwidth(col.a), 0.0001) + 0.5);
        #endif
    #elif LIL_RENDER == 2
        clip(col.a - _Cutoff);
    #endif
    return col;
}

#endif