#ifndef LIL_APPDATA_INCLUDED
#define LIL_APPDATA_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Appdata
#if defined(LIL_REQUIRE_APP_PREVPOS) || defined(LIL_PASS_MOTIONVECTOR_INCLUDED)
    #define LIL_APP_PREVPOS
#endif

#if defined(LIL_REQUIRE_APP_PREVEL) || defined(_ADD_PRECOMPUTED_VELOCITY)
    #define LIL_APP_PREVEL
#endif

#define LIL_APP_POSITION
#define LIL_APP_TEXCOORD0
#define LIL_APP_TEXCOORD1
#define LIL_APP_TEXCOORD2
#define LIL_APP_TEXCOORD3

/*
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
*/

#if !defined(LIL_APP_PREVPOS) && (defined(LIL_REQUIRE_APP_TEXCOORD4) || defined(LIL_FEATURE_IDMASK) && !defined(LIL_LITE))
    #define LIL_APP_TEXCOORD4
#endif

#if !defined(LIL_APP_PREVEL) && (defined(LIL_REQUIRE_APP_TEXCOORD5) || defined(LIL_FEATURE_IDMASK) && !defined(LIL_LITE))
    #define LIL_APP_TEXCOORD5
#endif

#if defined(LIL_REQUIRE_APP_TEXCOORD6) || (defined(LIL_FEATURE_IDMASK) || defined(LIL_FEATURE_ENCRYPTION)) && !defined(LIL_LITE)
    #define LIL_APP_TEXCOORD6
#endif

#if defined(LIL_REQUIRE_APP_TEXCOORD7) || (defined(LIL_FEATURE_IDMASK) || defined(LIL_FEATURE_ENCRYPTION)) && !defined(LIL_LITE)
    #define LIL_APP_TEXCOORD7
#endif

#if defined(LIL_REQUIRE_APP_COLOR) || defined(LIL_OUTLINE) || defined(LIL_ONEPASS_OUTLINE) || (!defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) && defined(LIL_FUR))
    #define LIL_APP_COLOR
#endif

#if defined(LIL_REQUIRE_APP_NORMAL) || defined(LIL_SHOULD_NORMAL) || defined(LIL_FEATURE_ENCRYPTION) || defined(LIL_OUTLINE) || defined(LIL_LITE) || defined(LIL_GEM) || defined(LIL_PASS_FORWARD_FUR_INCLUDED) || ((defined(LIL_PASS_DEPTHONLY_INCLUDED) || defined(LIL_PASS_MOTIONVECTOR_INCLUDED)) && defined(LIL_FUR)) || defined(LIL_TESSELLATION) || defined(LIL_PASS_DEPTHNORMAL_INCLUDED) || defined(WRITE_NORMAL_BUFFER) || defined(LIL_PASS_SHADOWCASTER_INCLUDED)
    #define LIL_APP_NORMAL
#endif

#if defined(LIL_REQUIRE_APP_TANGENT) || ((defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) || defined(LIL_GEM)) && defined(LIL_SHOULD_TANGENT)) || defined(LIL_OUTLINE) || defined(LIL_PASS_FORWARD_FUR_INCLUDED) || (!defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) && defined(LIL_FUR)) || defined(LIL_BAKER)
    #define LIL_APP_TANGENT
#endif

#if !defined(LIL_NOT_SUPPORT_VERTEXID) && (defined(LIL_REQUIRE_APP_VERTEXID) || defined(LIL_FEATURE_IDMASK) || defined(LIL_FUR))
    #define LIL_APP_VERTEXID
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

struct appdataCopy
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
        uint vertexID       : TEXCOORD8; // avoid error
    #endif
    #if defined(LIL_APP_PREVPOS)
        float3 previousPositionOS : TEXCOORD4;
    #endif
    #if defined(LIL_APP_PREVEL)
        float3 precomputedVelocity : TEXCOORD5;
    #endif
    LIL_VERTEX_INPUT_INSTANCE_ID
};

appdataCopy appdataOriginalToCopy(appdata i)
{
    appdataCopy o;
    #if defined(LIL_APP_POSITION)
        o.positionOS = i.positionOS;
    #endif
    #if defined(LIL_APP_TEXCOORD0)
        o.uv0 = i.uv0;
    #endif
    #if defined(LIL_APP_TEXCOORD1)
        o.uv1 = i.uv1;
    #endif
    #if defined(LIL_APP_TEXCOORD2)
        o.uv2 = i.uv2;
    #endif
    #if defined(LIL_APP_TEXCOORD3)
        o.uv3 = i.uv3;
    #endif
    #if defined(LIL_APP_TEXCOORD4)
        o.uv4 = i.uv4;
    #endif
    #if defined(LIL_APP_TEXCOORD5)
        o.uv5 = i.uv5;
    #endif
    #if defined(LIL_APP_TEXCOORD6)
        o.uv6 = i.uv6;
    #endif
    #if defined(LIL_APP_TEXCOORD7)
        o.uv7 = i.uv7;
    #endif
    #if defined(LIL_APP_COLOR)
        o.color = i.color;
    #endif
    #if defined(LIL_APP_NORMAL)
        o.normalOS = i.normalOS;
    #endif
    #if defined(LIL_APP_TANGENT)
        o.tangentOS = i.tangentOS;
    #endif
    #if defined(LIL_APP_VERTEXID)
        o.vertexID = i.vertexID;
    #endif
    #if defined(LIL_APP_PREVPOS)
        o.previousPositionOS = i.previousPositionOS;
    #endif
    #if defined(LIL_APP_PREVEL)
        o.precomputedVelocity = i.precomputedVelocity;
    #endif
    LIL_TRANSFER_INSTANCE_ID(i, o);
    return o;
}

appdata appdataCopyToOriginal(appdataCopy i)
{
    appdata o;
    #if defined(LIL_APP_POSITION)
        o.positionOS = i.positionOS;
    #endif
    #if defined(LIL_APP_TEXCOORD0)
        o.uv0 = i.uv0;
    #endif
    #if defined(LIL_APP_TEXCOORD1)
        o.uv1 = i.uv1;
    #endif
    #if defined(LIL_APP_TEXCOORD2)
        o.uv2 = i.uv2;
    #endif
    #if defined(LIL_APP_TEXCOORD3)
        o.uv3 = i.uv3;
    #endif
    #if defined(LIL_APP_TEXCOORD4)
        o.uv4 = i.uv4;
    #endif
    #if defined(LIL_APP_TEXCOORD5)
        o.uv5 = i.uv5;
    #endif
    #if defined(LIL_APP_TEXCOORD6)
        o.uv6 = i.uv6;
    #endif
    #if defined(LIL_APP_TEXCOORD7)
        o.uv7 = i.uv7;
    #endif
    #if defined(LIL_APP_COLOR)
        o.color = i.color;
    #endif
    #if defined(LIL_APP_NORMAL)
        o.normalOS = i.normalOS;
    #endif
    #if defined(LIL_APP_TANGENT)
        o.tangentOS = i.tangentOS;
    #endif
    #if defined(LIL_APP_VERTEXID)
        o.vertexID = i.vertexID;
    #endif
    #if defined(LIL_APP_PREVPOS)
        o.previousPositionOS = i.previousPositionOS;
    #endif
    #if defined(LIL_APP_PREVEL)
        o.precomputedVelocity = i.precomputedVelocity;
    #endif
    LIL_TRANSFER_INSTANCE_ID(i, o);
    return o;
}

#endif