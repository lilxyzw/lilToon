#ifndef LIL_STRUCT_FUR_INCLUDED
#define LIL_STRUCT_FUR_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Structure

// v2g
#define LIL_V2G_TEXCOORD0
#define LIL_V2G_POSITION_WS
#if defined(LIL_V2G_FORCE_NORMAL_WS) || (!defined(LIL_PASS_FORWARDADD) && defined(LIL_SHOULD_NORMAL))
    #define LIL_V2G_NORMAL_WS
#endif
#if defined(LIL_V2G_FORCE_TEXCOORD1) || defined(LIL_USE_LIGHTMAP_UV)
    #define LIL_V2G_TEXCOORD1
#endif
#define LIL_V2G_MAINLIGHT
#define LIL_V2G_VERTEXLIGHT
#define LIL_V2G_FOG
#define LIL_V2G_FURVECTOR

// g2f
#define LIL_V2F_POSITION_CS
#define LIL_V2F_TEXCOORD0

#if defined(LIL_V2F_FORCE_TEXCOORD1) || defined(LIL_USE_LIGHTMAP_UV)
    #define LIL_V2F_TEXCOORD1
#endif

#if defined(LIL_V2F_FORCE_POSITION_WS) || defined(LIL_PASS_FORWARDADD) || defined(LIL_FEATURE_DISTANCE_FADE) || !defined(LIL_BRP) || defined(LIL_USE_LPPV)
    #define LIL_V2F_POSITION_WS
#endif

#if defined(LIL_V2F_FORCE_NORMAL_WS) || !defined(LIL_PASS_FORWARDADD) && defined(LIL_SHOULD_NORMAL)
    #define LIL_V2F_NORMAL_WS
#endif
#define LIL_V2F_MAINLIGHT
#define LIL_V2F_VERTEXLIGHT
#define LIL_V2F_FOG
#define LIL_V2F_FURLAYER


struct v2g
{
    float2 uv           : TEXCOORD0;
    #if defined(LIL_V2G_TEXCOORD1)
        float2 uv1          : TEXCOORD1;
    #endif
    float3 positionWS   : TEXCOORD2;
    float3 furVector    : TEXCOORD3;
    #if defined(LIL_V2G_NORMAL_WS)
        float3 normalWS     : TEXCOORD4;
    #endif
    LIL_LIGHTCOLOR_COORDS(5)
    LIL_LIGHTDIRECTION_COORDS(6)
    LIL_INDLIGHTCOLOR_COORDS(7)
    LIL_VERTEXLIGHT_COORDS(8)
    LIL_FOG_COORDS(9)
    LIL_VERTEX_INPUT_INSTANCE_ID
    LIL_VERTEX_OUTPUT_STEREO
};

struct v2f
{
    float4 positionCS   : SV_POSITION;
    float2 uv           : TEXCOORD0;
    #if defined(LIL_V2F_TEXCOORD1)
        float2 uv1              : TEXCOORD1;
    #endif
    #if defined(LIL_V2F_POSITION_WS)
        float3 positionWS       : TEXCOORD2;
    #endif
    #if defined(LIL_V2F_NORMAL_WS)
        float3 normalWS         : TEXCOORD3;
    #endif
    float furLayer          : TEXCOORD4;
    LIL_LIGHTCOLOR_COORDS(5)
    LIL_LIGHTDIRECTION_COORDS(6)
    LIL_INDLIGHTCOLOR_COORDS(7)
    LIL_VERTEXLIGHT_COORDS(8)
    LIL_FOG_COORDS(9)
    LIL_VERTEX_INPUT_INSTANCE_ID
    LIL_VERTEX_OUTPUT_STEREO
};

#endif