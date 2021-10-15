#ifndef LIL_PASS_UNIVERSAL2D_INCLUDED
#define LIL_PASS_UNIVERSAL2D_INCLUDED

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
    float2 uv           : TEXCOORD0;
    LIL_CUSTOM_V2F_MEMBER(1,2,3,4,5,6,7,8)
};

//------------------------------------------------------------------------------------------------------------------------------
// Shader
#include "Includes/lil_common_vert.hlsl"
#include "Includes/lil_common_frag.hlsl"

float4 frag(v2f input) : SV_Target
{
    float facing = 1.0;
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