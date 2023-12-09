#ifndef LIL_PASS_SHADOWCASTER_INCLUDED
#define LIL_PASS_SHADOWCASTER_INCLUDED

#include "lil_common.hlsl"
#include "lil_common_appdata.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Structure
#if !defined(LIL_CUSTOM_V2F_MEMBER)
    #define LIL_CUSTOM_V2F_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7)
#endif

#if defined(LIL_V2F_FORCE_TEXCOORD0) || (LIL_RENDER > 0)
    #if defined(LIL_FUR)
        #define LIL_V2F_TEXCOORD0
    #else
        #define LIL_V2F_PACKED_TEXCOORD01
        #define LIL_V2F_PACKED_TEXCOORD23
    #endif
#endif
#if defined(LIL_V2F_FORCE_POSITION_OS) || ((LIL_RENDER > 0) && !defined(LIL_LITE) && defined(LIL_FEATURE_DISSOLVE))
    #define LIL_V2F_POSITION_OS
#endif
#if defined(LIL_V2F_FORCE_POSITION_WS) || (LIL_RENDER > 0) && defined(LIL_FEATURE_DISTANCE_FADE)
    #define LIL_V2F_POSITION_WS
#endif
#define LIL_V2F_SHADOW_CASTER

struct v2f
{
    LIL_V2F_SHADOW_CASTER_OUTPUT
    #if defined(LIL_V2F_TEXCOORD0)
        float2 uv0         : TEXCOORD1;
    #endif
    #if defined(LIL_V2F_PACKED_TEXCOORD01)
        float4 uv01         : TEXCOORD1;
    #endif
    #if defined(LIL_V2F_PACKED_TEXCOORD23)
        float4 uv23         : TEXCOORD2;
    #endif
    #if defined(LIL_V2F_POSITION_OS)
        float4 positionOSdissolve   : TEXCOORD3;
    #endif
    #if defined(LIL_V2F_POSITION_WS)
        float3 positionWS   : TEXCOORD4;
    #endif
    LIL_CUSTOM_V2F_MEMBER(5,6,7,8,9,10,11,12)
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

    #include "lil_common_frag_alpha.hlsl"

    LIL_SHADOW_CASTER_FRAGMENT(input);
}

#endif
