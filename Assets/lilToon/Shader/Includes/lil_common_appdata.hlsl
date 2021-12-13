#ifndef LIL_APPDATA_INCLUDED
#define LIL_APPDATA_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Appdata
#define LIL_APP_POSITION
#define LIL_APP_TEXCOORD0
#define LIL_APP_TEXCOORD1
#define LIL_APP_TEXCOORD2
#define LIL_APP_TEXCOORD3

#if defined(LIL_REQUIRE_APP_POSITION)
#endif

#if defined(LIL_REQUIRE_APP_TEXCOORD0)
#endif

#if defined(LIL_REQUIRE_APP_TEXCOORD1)
#endif

#if defined(LIL_REQUIRE_APP_TEXCOORD2)
#endif

#if defined(LIL_REQUIRE_APP_TEXCOORD3)
#endif

#if defined(LIL_REQUIRE_APP_TEXCOORD4)
    #define LIL_APP_TEXCOORD4
#endif

#if defined(LIL_REQUIRE_APP_TEXCOORD5)
    #define LIL_APP_TEXCOORD5
#endif

#if defined(LIL_REQUIRE_APP_TEXCOORD6) || defined(LIL_FEATURE_ENCRYPTION) && !defined(LIL_LITE)
    #define LIL_APP_TEXCOORD6
#endif

#if defined(LIL_REQUIRE_APP_TEXCOORD7) || defined(LIL_FEATURE_ENCRYPTION) && !defined(LIL_LITE)
    #define LIL_APP_TEXCOORD7
#endif

#if defined(LIL_REQUIRE_APP_COLOR) || defined(LIL_OUTLINE) || defined(LIL_ONEPASS_OUTLINE) || (!defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) && defined(LIL_FUR))
    #define LIL_APP_COLOR
#endif

#if defined(LIL_REQUIRE_APP_NORMAL) || defined(LIL_SHOULD_NORMAL) || defined(LIL_OUTLINE) || defined(LIL_LITE) || defined(LIL_GEM) || defined(LIL_PASS_FORWARD_FUR_INCLUDED) || ((defined(LIL_PASS_DEPTHONLY_INCLUDED) || defined(LIL_PASS_MOTIONVECTOR_INCLUDED)) && defined(LIL_FUR)) || defined(LIL_TESSELLATION) || defined(LIL_PASS_DEPTHNORMAL_INCLUDED) || defined(WRITE_NORMAL_BUFFER) || defined(LIL_PASS_SHADOWCASTER_INCLUDED)
    #define LIL_APP_NORMAL
#endif

#if defined(LIL_REQUIRE_APP_TANGENT) || ((defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) || defined(LIL_GEM)) && defined(LIL_SHOULD_TANGENT) && !defined(LIL_OUTLINE) && !defined(LIL_FUR)) || defined(LIL_OUTLINE) && defined(LIL_FEATURE_TEX_OUTLINE_NORMAL) || defined(LIL_PASS_FORWARD_FUR_INCLUDED) || (!defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) && defined(LIL_FUR)) || defined(LIL_BAKER)
    #define LIL_APP_TANGENT
#endif

#if defined(LIL_REQUIRE_APP_VERTEXID)
    #define LIL_APP_VERTEXID
#endif

#if defined(LIL_REQUIRE_APP_PREVPOS) || defined(LIL_PASS_MOTIONVECTOR_INCLUDED)
    #define LIL_APP_PREVPOS
#endif

#if defined(LIL_REQUIRE_APP_PREVEL) || defined(_ADD_PRECOMPUTED_VELOCITY)
    #define LIL_APP_PREVEL
#endif

struct appdata
{
    #if defined(LIL_APP_POSITION)
        float4 positionOS   : POSITION;
    #endif
    #if defined(LIL_APP_TEXCOORD0)
        float2 uv0          : TEXCOORD0;
    #endif
    #if defined(LIL_APP_TEXCOORD1)
        float2 uv1          : TEXCOORD1;
    #endif
    #if defined(LIL_APP_TEXCOORD2)
        float2 uv2          : TEXCOORD2;
    #endif
    #if defined(LIL_APP_TEXCOORD3)
        float2 uv3          : TEXCOORD3;
    #endif
    #if defined(LIL_APP_TEXCOORD4)
        float2 uv4          : TEXCOORD4;
    #endif
    #if defined(LIL_APP_TEXCOORD5)
        float2 uv5          : TEXCOORD5;
    #endif
    #if defined(LIL_APP_TEXCOORD6)
        float2 uv6          : TEXCOORD6;
    #endif
    #if defined(LIL_APP_TEXCOORD7)
        float2 uv7          : TEXCOORD7;
    #endif
    #if defined(LIL_APP_COLOR)
        float4 color        : COLOR;
    #endif
    #if defined(LIL_APP_NORMAL)
        float3 normalOS     : NORMAL;
    #endif
    #if defined(LIL_APP_TANGENT)
        float4 tangentOS    : TANGENT;
    #endif
    #if defined(LIL_APP_VERTEXID)
		uint vertexID       : SV_VertexID;
    #endif
    #if defined(LIL_APP_PREVPOS)
        float3 previousPositionOS : TEXCOORD4;
    #endif
    #if defined(LIL_APP_PREVEL)
        float3 precomputedVelocity : TEXCOORD5;
    #endif
    LIL_VERTEX_INPUT_INSTANCE_ID
};

#endif