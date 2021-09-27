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
    #define LIL_V2F_POSITION_CS
    #define LIL_V2F_TEXCOORD0
    #if defined(LIL_V2F_FORCE_TEXCOORD1) || defined(LIL_USE_LIGHTMAP_UV)
        #define LIL_V2F_TEXCOORD1
    #endif
    #if defined(LIL_V2F_FORCE_POSITION_OS) || defined(LIL_SHOULD_POSITION_OS)
        #define LIL_V2F_POSITION_OS
    #endif
    #if defined(LIL_V2F_FORCE_POSITION_WS) || defined(LIL_PASS_FORWARDADD) || defined(LIL_FEATURE_DISTANCE_FADE) || !defined(LIL_BRP) || defined(LIL_USE_LPPV)
        #define LIL_V2F_POSITION_WS
    #endif
    #if defined(LIL_V2F_FORCE_NORMAL) || defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SUBTRACTIVE) || defined(LIL_HDRP)
        #define LIL_V2F_NORMAL_WS
    #endif
    #if !defined(LIL_PASS_FORWARDADD)
        #define LIL_V2F_LIGHTCOLOR
        #define LIL_V2F_VERTEXLIGHT
    #endif
    #define LIL_V2F_FOG

    struct v2f
    {
        float4 positionCS       : SV_POSITION;
        float2 uv               : TEXCOORD0;
        #if defined(LIL_V2F_TEXCOORD1)
            float2 uv1              : TEXCOORD1;
        #endif
        #if defined(LIL_V2F_POSITION_OS)
            float3 positionOS       : TEXCOORD2;
        #endif
        #if defined(LIL_V2F_POSITION_WS)
            float3 positionWS       : TEXCOORD3;
        #endif
        #if defined(LIL_V2F_NORMAL_WS)
            float3 normalWS         : TEXCOORD4;
        #endif
        LIL_LIGHTCOLOR_COORDS(5)
        LIL_VERTEXLIGHT_COORDS(6)
        LIL_FOG_COORDS(7)
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };
#elif defined(LIL_FUR)
    #define LIL_V2F_POSITION_CS
    #define LIL_V2F_TEXCOORD0
    #if defined(LIL_V2F_FORCE_TEXCOORD1) || defined(LIL_USE_LIGHTMAP_UV)
        #define LIL_V2F_TEXCOORD1
    #endif
    #if defined(LIL_V2F_FORCE_POSITION_WS) || defined(LIL_PASS_FORWARDADD) || defined(LIL_FEATURE_DISTANCE_FADE) || !defined(LIL_BRP) || defined(LIL_USE_LPPV)
        #define LIL_V2F_POSITION_WS
    #endif
    #if defined(LIL_V2F_FORCE_NORMAL) || !defined(LIL_PASS_FORWARDADD) &&  defined(LIL_SHOULD_NORMAL)
        #define LIL_V2F_NORMAL_WS
    #endif
    #if !defined(LIL_PASS_FORWARDADD)
        #define LIL_V2F_LIGHTCOLOR
        #define LIL_V2F_LIGHTDIRECTION
        #define LIL_V2F_VERTEXLIGHT
        #if defined(LIL_FEATURE_SHADOW)
            #define LIL_V2F_INDLIGHTCOLOR
        #endif
    #endif
    #define LIL_V2F_FOG

    struct v2f
    {
        float4 positionCS       : SV_POSITION;
        float2 uv               : TEXCOORD0;
        #if defined(LIL_V2F_TEXCOORD1)
            float2 uv1              : TEXCOORD1;
        #endif
        #if defined(LIL_V2F_POSITION_WS)
            float3 positionWS       : TEXCOORD2;
        #endif
        #if defined(LIL_V2F_NORMAL_WS)
            float3 normalWS         : TEXCOORD3;
        #endif
        LIL_LIGHTCOLOR_COORDS(4)
        LIL_LIGHTDIRECTION_COORDS(5)
        LIL_INDLIGHTCOLOR_COORDS(6)
        LIL_VERTEXLIGHT_COORDS(7)
        LIL_FOG_COORDS(8)
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };
#else
    #define LIL_V2F_POSITION_CS
    #define LIL_V2F_TEXCOORD0
    #if defined(LIL_V2F_FORCE_TEXCOORD1) || defined(LIL_USE_LIGHTMAP_UV) || defined(LIL_SHOULD_UV1)
        #define LIL_V2F_TEXCOORD1
    #endif
    #if defined(LIL_V2F_FORCE_POSITION_OS) || defined(LIL_SHOULD_POSITION_OS)
        #define LIL_V2F_POSITION_OS
    #endif
    #if defined(LIL_V2F_FORCE_POSITION_WS) || defined(LIL_SHOULD_POSITION_WS)
        #define LIL_V2F_POSITION_WS
    #endif
    #if defined(LIL_V2F_FORCE_POSITION_SS) || defined(LIL_REFRACTION)
        #define LIL_V2F_POSITION_SS
    #endif
    #if defined(LIL_V2F_FORCE_NORMAL) || defined(LIL_SHOULD_NORMAL)
        #define LIL_V2F_NORMAL_WS
    #endif
    #if defined(LIL_V2F_FORCE_TANGENT) || defined(LIL_SHOULD_TBN)
        #define LIL_V2F_TANGENT_WS
    #endif
    #if defined(LIL_V2F_FORCE_BITANGENT) || defined(LIL_SHOULD_TBN)
        #define LIL_V2F_BITANGENT_WS
    #endif
    #if !defined(LIL_PASS_FORWARDADD)
        #define LIL_V2F_LIGHTCOLOR
        #define LIL_V2F_LIGHTDIRECTION
        #define LIL_V2F_VERTEXLIGHT
        #if defined(LIL_FEATURE_SHADOW)
            #define LIL_V2F_INDLIGHTCOLOR
            #define LIL_V2F_SHADOW
        #endif
    #endif
    #define LIL_V2F_FOG

    struct v2f
    {
        float4 positionCS       : SV_POSITION;
        float2 uv               : TEXCOORD0;
        #if defined(LIL_V2F_TEXCOORD1)
            float2 uv1          : TEXCOORD1;
        #endif
        #if defined(LIL_V2F_POSITION_OS)
            float3 positionOS       : TEXCOORD2;
        #endif
        #if defined(LIL_V2F_POSITION_WS)
            float3 positionWS       : TEXCOORD3;
        #endif
        #if defined(LIL_V2F_NORMAL_WS)
            float3 normalWS         : TEXCOORD4;
        #endif
        #if defined(LIL_V2F_TANGENT_WS)
            float4 tangentWS        : TEXCOORD5;
        #endif
        #if defined(LIL_V2F_BITANGENT_WS)
            float3 bitangentWS      : TEXCOORD6;
        #endif
        #if defined(LIL_V2F_POSITION_SS)
            float4 positionSS       : TEXCOORD7;
        #endif
        LIL_LIGHTCOLOR_COORDS(8)
        LIL_LIGHTDIRECTION_COORDS(9)
        LIL_INDLIGHTCOLOR_COORDS(10)
        LIL_VERTEXLIGHT_COORDS(11)
        LIL_FOG_COORDS(12)
        LIL_SHADOW_COORDS(13)
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };
#endif

#endif