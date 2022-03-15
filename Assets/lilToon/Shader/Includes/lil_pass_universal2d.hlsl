#ifndef LIL_PASS_UNIVERSAL2D_INCLUDED
#define LIL_PASS_UNIVERSAL2D_INCLUDED

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

float4 frag(v2f input) : SV_Target
{
    LIL_SETUP_INSTANCE_ID(input);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    lilFragData fd = lilInitFragData();

    BEFORE_UNPACK_V2F
    OVERRIDE_UNPACK_V2F

    BEFORE_ANIMATE_MAIN_UV
    OVERRIDE_ANIMATE_MAIN_UV

    BEFORE_MAIN
    OVERRIDE_MAIN
    #if LIL_RENDER == 1
        #ifdef LIL_LITE
            clip(fd.col.a - _Cutoff);
        #else
            fd.col.a = saturate((fd.col.a - _Cutoff) / max(fwidth(fd.col.a), 0.0001) + 0.5);
        #endif
    #elif LIL_RENDER == 2
        clip(fd.col.a - _Cutoff);
    #endif
    return fd.col;
}

#endif