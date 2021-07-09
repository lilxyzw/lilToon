#ifndef LIL_STRUCT_INCLUDED
#define LIL_STRUCT_INCLUDED

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
// positionOS           : Object space position
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

#if defined(LIL_OUTLINE)
    struct appdata
    {
        float4 positionOS   : POSITION;
        float3 normalOS     : NORMAL;
        float2 uv           : TEXCOORD0;
        #if defined(LIL_FEATURE_ENCRYPTION)
            float2 uv6          : TEXCOORD6;
            float2 uv7          : TEXCOORD7;
        #endif
        float4 color        : COLOR;
        LIL_VERTEX_INPUT_LIGHTMAP_UV
        LIL_VERTEX_INPUT_INSTANCE_ID
    };

    struct v2f
    {
        float4 positionCS       : SV_POSITION;
        float2 uv               : TEXCOORD0;
        #if defined(LIL_PASS_FORWARDADD) || defined(LIL_FEATURE_DISTANCE_FADE) || !defined(LIL_BRP)
            float3 positionWS       : TEXCOORD1;
        #endif
        #if defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SUBTRACTIVE)
            float3 normalWS         : TEXCOORD2;
        #endif
        LIL_VERTEXLIGHT_COORDS(3)
        LIL_FOG_COORDS(4)
        LIL_LIGHTMAP_COORDS(5)
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };
#elif defined(LIL_FUR)
    struct appdata
    {
        float4 positionOS   : POSITION;
        #if defined(LIL_SHOULD_NORMAL) || defined(LIL_TESSELLATION)
            float3 normalOS     : NORMAL;
        #endif
        #if defined(LIL_FEATURE_ENCRYPTION)
            float2 uv6          : TEXCOORD6;
            float2 uv7          : TEXCOORD7;
        #endif
        float2 uv           : TEXCOORD0;
        LIL_VERTEX_INPUT_LIGHTMAP_UV
        LIL_VERTEX_INPUT_INSTANCE_ID
    };

    struct v2f
    {
        float4 positionCS       : SV_POSITION;
        float2 uv               : TEXCOORD0;
        #if defined(LIL_PASS_FORWARDADD) || defined(LIL_FEATURE_DISTANCE_FADE) || !defined(LIL_BRP)
            float3 positionWS       : TEXCOORD1;
        #endif
        #if !defined(LIL_PASS_FORWARDADD) &&  defined(LIL_SHOULD_NORMAL)
            float3 normalWS         : TEXCOORD2;
        #endif
        LIL_VERTEXLIGHT_COORDS(3)
        LIL_FOG_COORDS(4)
        LIL_LIGHTMAP_COORDS(5)
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };
#else
    struct appdata
    {
        float4 positionOS   : POSITION;
        #if defined(LIL_SHOULD_NORMAL) || defined(LIL_TESSELLATION)
            float3 normalOS     : NORMAL;
        #endif
        #if defined(LIL_SHOULD_TANGENT)
            float4 tangentOS    : TANGENT;
        #endif
        #if defined(LIL_FEATURE_ENCRYPTION)
            float2 uv6          : TEXCOORD6;
            float2 uv7          : TEXCOORD7;
        #endif
        float2 uv           : TEXCOORD0;
        LIL_VERTEX_INPUT_LIGHTMAP_UV
        LIL_VERTEX_INPUT_INSTANCE_ID
    };

    struct v2f
    {
        float4 positionCS       : SV_POSITION;
        float2 uv               : TEXCOORD0;
        #if defined(LIL_SHOULD_POSITION_OS)
            float3 positionOS       : TEXCOORD1;
        #endif
        #if defined(LIL_SHOULD_POSITION_WS)
            float3 positionWS       : TEXCOORD2;
        #endif
        #if defined(LIL_SHOULD_NORMAL)
            float3 normalWS         : TEXCOORD3;
        #endif
        #if defined(LIL_SHOULD_TBN)
            float3 tangentWS        : TEXCOORD4;
            float3 bitangentWS      : TEXCOORD5;
        #endif
        #if defined(LIL_SHOULD_TANGENT_W)
            float  tangentW         : TEXCOORD6;
        #endif
        #ifdef LIL_REFRACTION
            float4 positionSS      : TEXCOORD7;
        #endif
        LIL_VERTEXLIGHT_COORDS(8)
        LIL_FOG_COORDS(9)
        LIL_SHADOW_COORDS(10)
        LIL_LIGHTMAP_COORDS(11)
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };
#endif

#endif