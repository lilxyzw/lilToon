#ifndef LIL_PASS_FORWARD_FUR_INCLUDED
#define LIL_PASS_FORWARD_FUR_INCLUDED

#include "lil_common.hlsl"
#include "lil_common_appdata.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Structure
#if !defined(LIL_CUSTOM_V2G_MEMBER)
    #define LIL_CUSTOM_V2G_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7)
#endif

#if !defined(LIL_CUSTOM_V2F_MEMBER)
    #define LIL_CUSTOM_V2F_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7)
#endif

// v2g
#define LIL_V2G_TEXCOORD0
#define LIL_V2G_POSITION_WS
#define LIL_V2G_NORMAL_WS
#if !defined(LIL_PASS_FORWARDADD)
    #define LIL_V2G_LIGHTCOLOR
    #define LIL_V2G_LIGHTDIRECTION
    #if defined(LIL_FEATURE_SHADOW) && (defined(LIL_BRP) || defined(LIL_HDRP))
        #define LIL_V2G_INDLIGHTCOLOR
    #endif
#endif
#define LIL_V2G_VERTEXLIGHT_FOG
#define LIL_V2G_FURVECTOR
#define LIL_V2G_VERTEXID

// g2f
#define LIL_V2F_POSITION_CS
#define LIL_V2F_POSITION_WS
#define LIL_V2F_TEXCOORD0
#define LIL_V2F_NORMAL_WS

#if !defined(LIL_PASS_FORWARDADD)
    #define LIL_V2F_LIGHTCOLOR
    #define LIL_V2F_LIGHTDIRECTION
    #if defined(LIL_BRP) || defined(LIL_HDRP)
        #define LIL_V2F_INDLIGHTCOLOR
    #endif
#endif
#define LIL_V2F_VERTEXLIGHT_FOG
#define LIL_V2F_FURLAYER

struct v2g
{
    float2 uv0          : TEXCOORD0;
    float3 positionWS   : TEXCOORD1;
    float3 furVector    : TEXCOORD2;
    uint vertexID       : TEXCOORD3;
    float3 normalWS     : TEXCOORD4;
    LIL_LIGHTCOLOR_COORDS(5)
    LIL_LIGHTDIRECTION_COORDS(6)
    LIL_INDLIGHTCOLOR_COORDS(7)
    LIL_VERTEXLIGHT_FOG_COORDS(8)
    LIL_CUSTOM_V2G_MEMBER(9,10,11,12,13,14,15,16)
    LIL_VERTEX_INPUT_INSTANCE_ID
    LIL_VERTEX_OUTPUT_STEREO
};

struct v2f
{
    float4 positionCS   : SV_POSITION;
    float2 uv0          : TEXCOORD0;
    float3 positionWS   : TEXCOORD1;
    LIL_VECTOR_INTERPOLATION float3 normalWS     : TEXCOORD2;
    float furLayer      : TEXCOORD3;
    LIL_LIGHTCOLOR_COORDS(4)
    LIL_LIGHTDIRECTION_COORDS(5)
    LIL_INDLIGHTCOLOR_COORDS(6)
    LIL_VERTEXLIGHT_FOG_COORDS(7)
    LIL_CUSTOM_V2F_MEMBER(8,9,10,11,12,13,14,15)
    LIL_VERTEX_INPUT_INSTANCE_ID
    LIL_VERTEX_OUTPUT_STEREO
};

//------------------------------------------------------------------------------------------------------------------------------
// Shader
#include "lil_common_vert_fur.hlsl"
#include "lil_common_frag.hlsl"

float4 frag(v2f input) : SV_Target
{
    //------------------------------------------------------------------------------------------------------------------------------
    // Initialize
    LIL_FORCE_SCENE_LIGHT;
    LIL_SETUP_INSTANCE_ID(input);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    lilFragData fd = lilInitFragData();

    // For VRCLV
    #if defined(LIL_BRP) && defined(LIL_PASS_FORWARD) && defined(VRC_LIGHT_VOLUMES_INCLUDED) && (defined(LIL_INPUT_OPTIMIZED) || defined(LIL_MULTI))
        if(_UdonLightVolumeEnabled)
        {
            lilVertexPositionInputs vertexInput = lilGetVertexPositionInputsFromWS(input.positionWS);
            lilVertexNormalInputs vertexNormalInput = lilGetVertexNormalInputsFromWS(input.normalWS);
            LIL_UNPACK_TEXCOORD1(input,fd);
            #define input fd
            LIL_CALC_MAINLIGHT(vertexInput, lightdataInput);
            #undef input
            #if defined(LIL_V2F_LIGHTCOLOR)
                input.lightColor     = lightdataInput.lightColor;
            #endif
            #if defined(LIL_V2F_LIGHTDIRECTION)
                input.lightDirection = lightdataInput.lightDirection;
            #endif
            #if defined(LIL_V2F_INDLIGHTCOLOR)
                input.indLightColor  = lightdataInput.indLightColor;
            #endif
        }
    #endif

    BEFORE_UNPACK_V2F
    OVERRIDE_UNPACK_V2F
    LIL_GET_HDRPDATA(input,fd);
    #if defined(LIL_V2F_SHADOW) || defined(LIL_PASS_FORWARDADD)
        LIL_LIGHT_ATTENUATION(fd.attenuation, input);
    #endif

    LIL_GET_LIGHTING_DATA(input,fd);

    //------------------------------------------------------------------------------------------------------------------------------
    // View Direction
    #if defined(LIL_V2F_POSITION_WS)
        LIL_GET_POSITION_WS_DATA(input,fd);
    #endif
    #if defined(LIL_V2F_NORMAL_WS) && defined(LIL_V2F_TANGENT_WS)
        LIL_GET_TBN_DATA(input,fd);
    #endif
    #if defined(LIL_V2F_NORMAL_WS) && defined(LIL_V2F_TANGENT_WS) && defined(LIL_V2F_POSITION_WS)
        LIL_GET_PARALLAX_DATA(input,fd);
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // UV
    BEFORE_ANIMATE_MAIN_UV
    OVERRIDE_ANIMATE_MAIN_UV
    BEFORE_CALC_DDX_DDY
    OVERRIDE_CALC_DDX_DDY

    //------------------------------------------------------------------------------------------------------------------------------
    // Main Color
    BEFORE_MAIN
    OVERRIDE_MAIN

    //------------------------------------------------------------------------------------------------------------------------------
    // Fur
    BEFORE_FUR
    OVERRIDE_FUR

    //------------------------------------------------------------------------------------------------------------------------------
    // Copy
    fd.albedo = fd.col.rgb;

    //------------------------------------------------------------------------------------------------------------------------------
    // Alpha
    #if LIL_RENDER == 1 || defined(LIL_FUR_PRE)
        // Cutout
        fd.col.a = saturate(fd.col.a*5.0-2.0);
        if(fd.col.a == 0) discard;
    #else
        // Transparent
        clip(fd.col.a - _Cutoff);
    #endif

    fd.origN = normalize(input.normalWS);
    fd.N = normalize(input.normalWS);
    fd.ln = dot(fd.L, fd.N);

    #if defined(LIL_BRP) && defined(LIL_PASS_FORWARD) && defined(VRC_LIGHT_VOLUMES_INCLUDED)
    if(_UdonLightVolumeEnabled)
    {
        LIL_CORRECT_LIGHTCOLOR_PS(fd.indLightColor);
        LIL_CORRECT_LIGHTCOLOR_VS(fd.indLightColor);
        float borderMin = _EnvRimBorder - _EnvRimBlur * 0.5;
        float borderMax = _EnvRimBorder + _EnvRimBlur * 0.5;
        float fakerim = saturate((fd.ln - fd.vl - borderMin) / saturate(borderMax - borderMin + fwidth(fd.ln - fd.vl) * _AAStrength));
        fd.lightColor += saturate(fd.indLightColor - fd.lightColor) * fakerim;
        fd.indLightColor = 0;
    }
    #endif

    BEFORE_SHADOW
    #ifndef LIL_PASS_FORWARDADD
        //------------------------------------------------------------------------------------------------------------------------------
        // Lighting
        #if defined(LIL_FEATURE_SHADOW)
            fd.ln = dot(fd.L, fd.N);
            OVERRIDE_SHADOW
            fd.col.rgb += fd.albedo * fd.addLightColor;
            fd.col.rgb = min(fd.col.rgb, fd.albedo * _LightMaxLimit);
        #else
            fd.col.rgb *= saturate(fd.lightColor + fd.addLightColor);
        #endif
    #else
        #if defined(LIL_FEATURE_SHADOW) && defined(LIL_OPTIMIZE_APPLY_SHADOW_FA)
            fd.ln = dot(fd.L, fd.N);
            OVERRIDE_SHADOW
        #else
            fd.col.rgb *= fd.lightColor;
        #endif

        #if LIL_RENDER == 2 && !defined(LIL_FUR_PRE)
            fd.col.rgb *= saturate(fd.col.a * _AlphaBoostFA);
        #endif
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Rim Shade
    BEFORE_RIMSHADE
    #if defined(LIL_FEATURE_RIMSHADE)
        OVERRIDE_RIMSHADE
    #endif

    fd.col.rgb += input.furLayer * pow((1-abs(dot(normalize(fd.N), fd.V))), _FurRimFresnelPower) * lerp(1,lilGray(fd.invLighting), _FurRimAntiLight) * _FurRimColor.rgb * fd.lightColor;

    //------------------------------------------------------------------------------------------------------------------------------
    // Distance Fade
    BEFORE_DISTANCE_FADE
    #if defined(LIL_FEATURE_DISTANCE_FADE)
        OVERRIDE_DISTANCE_FADE
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Fix Color
    LIL_HDRP_DEEXPOSURE(fd.col);

    //------------------------------------------------------------------------------------------------------------------------------
    // Fog
    BEFORE_FOG
    OVERRIDE_FOG

    BEFORE_OUTPUT
    OVERRIDE_OUTPUT
}

#endif