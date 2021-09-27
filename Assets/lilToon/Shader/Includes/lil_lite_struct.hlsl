#ifndef LIL_STRUCT_LITE_INCLUDED
#define LIL_STRUCT_LITE_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Structure

#ifdef LIL_OUTLINE
    #define LIL_V2F_POSITION_CS
    #define LIL_V2F_TEXCOORD0
    #if defined(LIL_V2F_FORCE_TEXCOORD1) || defined(LIL_USE_LIGHTMAP_UV)
        #define LIL_V2F_TEXCOORD1
    #endif
    #if defined(LIL_V2F_FORCE_POSITION_WS) || defined(LIL_PASS_FORWARDADD) || !defined(LIL_BRP) || defined(LIL_USE_LPPV)
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
        #if defined(LIL_V2F_POSITION_WS)
            float3 positionWS       : TEXCOORD2;
        #endif
        #if defined(LIL_V2F_NORMAL_WS)
            float3 normalWS         : TEXCOORD3;
        #endif
        LIL_LIGHTCOLOR_COORDS(4)
        LIL_VERTEXLIGHT_COORDS(5)
        LIL_FOG_COORDS(6)
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };
#else
    #define LIL_V2F_POSITION_WS
    #define LIL_V2F_POSITION_CS
    #define LIL_V2F_TEXCOORD0
    #if defined(LIL_V2F_FORCE_TEXCOORD1) || defined(LIL_USE_LIGHTMAP_UV)
        #define LIL_V2F_TEXCOORD1
    #endif
    #define LIL_V2F_NORMAL_WS
    #define LIL_V2F_UVMAT
    #if !defined(LIL_PASS_FORWARDADD)
        #define LIL_V2F_LIGHTCOLOR
        #define LIL_V2F_LIGHTDIRECTION
        #define LIL_V2F_INDLIGHTCOLOR
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
        float2 uvMat            : TEXCOORD2;
        float3 normalWS         : TEXCOORD3;
        float3 positionWS       : TEXCOORD4;
        LIL_LIGHTCOLOR_COORDS(5)
        LIL_LIGHTDIRECTION_COORDS(6)
        LIL_INDLIGHTCOLOR_COORDS(7)
        LIL_VERTEXLIGHT_COORDS(8)
        LIL_FOG_COORDS(9)
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };
#endif

#endif