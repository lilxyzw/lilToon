#ifndef LIL_PASS_SHADOWCASTER_INCLUDED
#define LIL_PASS_SHADOWCASTER_INCLUDED

#include "Includes/lil_pipeline.hlsl"
#include "Includes/lil_common_appdata.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Structure
#if !defined(LIL_CUSTOM_V2F_MEMBER)
    #define LIL_CUSTOM_V2F_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7)
#endif

#if defined(LIL_V2F_FORCE_TEXCOORD0) || (LIL_RENDER > 0)
    #define LIL_V2F_TEXCOORD0
#endif
#if defined(LIL_V2F_FORCE_POSITION_OS) || ((LIL_RENDER > 0) && !defined(LIL_LITE) && !defined(LIL_FUR) && defined(LIL_FEATURE_DISSOLVE))
    #define LIL_V2F_POSITION_OS
#endif
#define LIL_V2F_SHADOW_CASTER

struct v2f
{
    LIL_V2F_SHADOW_CASTER_OUTPUT
    #if defined(LIL_V2F_TEXCOORD0)
        float2 uv0          : TEXCOORD1;
    #endif
    #if defined(LIL_V2F_POSITION_OS)
        float3 positionOS   : TEXCOORD2;
    #endif
    LIL_CUSTOM_V2F_MEMBER(3,4,5,6,7,8,9,10)
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

    #include "Includes/lil_common_frag_alpha.hlsl"

    LIL_SHADOW_CASTER_FRAGMENT(input);
}

#endif
