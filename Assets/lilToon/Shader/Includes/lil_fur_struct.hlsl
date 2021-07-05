#ifndef LIL_STRUCT_FUR_INCLUDED
#define LIL_STRUCT_FUR_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Structure

// positionOS           : Object space position
// normalOS             : Object space normal
// tangentOS            : Object space tangent
// uv                   : UV
// uv1                  : UV1
// color                : Vertex color
// LIL_VERTEX_INPUT_INSTANCE_ID

// positionCS           : Clip space position
// positionWS           : World space position
// positionSS           : Screen space coordinates
// uv                   : UV
// uvMat                : MatCap UV
// normalWS             : World space normal
// tangentWS            : World space tangent
// bitangentWS          : World space bitangent
// tangentW             : Handedness
// vl                   : Vertex lighting
// furLayer             : Fur Layer (in:0 out:1)
// LIL_FOG_COORDS()     : Fog
// LIL_SHADOW_COORDS()  : Shadow
// LIL_LIGHTMAP_COORDS(): Lightmap
// LIL_VERTEX_INPUT_INSTANCE_ID
// LIL_VERTEX_OUTPUT_STEREO

struct appdata
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    float4 tangentOS    : TANGENT;
    float4 color        : COLOR;
    float2 uv           : TEXCOORD0;
    LIL_VERTEX_INPUT_LIGHTMAP_UV
    LIL_VERTEX_INPUT_INSTANCE_ID
};

struct v2g
{
    float2 uv               : TEXCOORD0;
    float3 positionWS       : TEXCOORD1;
    float3 furVector        : TEXCOORD2;
    #if !defined(LIL_PASS_FORWARDADD) && defined(LIL_SHOULD_NORMAL)
        float3 normalWS         : TEXCOORD3;
    #endif
    LIL_VERTEXLIGHT_COORDS(4)
    LIL_FOG_COORDS(5)
    LIL_LIGHTMAP_COORDS(6)
    LIL_VERTEX_INPUT_INSTANCE_ID
    LIL_VERTEX_OUTPUT_STEREO
};

struct g2f
{
    float4 positionCS       : SV_POSITION;
    float2 uv               : TEXCOORD0;
    #if defined(LIL_PASS_FORWARDADD) || defined(LIL_FEATURE_DISTANCE_FADE) || !defined(LIL_BRP)
        float3 positionWS       : TEXCOORD1;
    #endif
    #if !defined(LIL_PASS_FORWARDADD) && defined(LIL_SHOULD_NORMAL)
        float3 normalWS         : TEXCOORD2;
    #endif
    float furLayer          : TEXCOORD3;
    LIL_VERTEXLIGHT_COORDS(4)
    LIL_FOG_COORDS(5)
    LIL_LIGHTMAP_COORDS(6)
    LIL_VERTEX_INPUT_INSTANCE_ID
    LIL_VERTEX_OUTPUT_STEREO
};

#endif