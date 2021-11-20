#ifndef LIL_PASS_FORWARD_LITE_INCLUDED
#define LIL_PASS_FORWARD_LITE_INCLUDED

#include "Includes/lil_pipeline.hlsl"
#include "Includes/lil_common_appdata.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Structure
#if !defined(LIL_CUSTOM_V2F_MEMBER)
    #define LIL_CUSTOM_V2F_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7)
#endif

#if defined(LIL_OUTLINE)
    #define LIL_V2F_POSITION_CS
    #define LIL_V2F_PACKED_TEXCOORD01
    #define LIL_V2F_PACKED_TEXCOORD23
    #if defined(LIL_V2F_FORCE_POSITION_WS) || defined(LIL_PASS_FORWARDADD) || !defined(LIL_BRP) || defined(LIL_USE_LPPV)
        #define LIL_V2F_POSITION_WS
    #endif
    #if defined(LIL_V2F_FORCE_NORMAL) || defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SUBTRACTIVE) || defined(LIL_HDRP)
        #define LIL_V2F_NORMAL_WS
    #endif
    #if !defined(LIL_PASS_FORWARDADD)
        #define LIL_V2F_LIGHTCOLOR
    #endif
    #define LIL_V2F_VERTEXLIGHT_FOG

    struct v2f
    {
        float4 positionCS   : SV_POSITION;
        float4 uv01         : TEXCOORD0;
        float4 uv23         : TEXCOORD1;
        #if defined(LIL_V2F_POSITION_WS)
            float3 positionWS   : TEXCOORD2;
        #endif
        #if defined(LIL_V2F_NORMAL_WS)
            float3 normalWS     : TEXCOORD3;
        #endif
        LIL_LIGHTCOLOR_COORDS(4)
        LIL_VERTEXLIGHT_FOG_COORDS(5)
        LIL_CUSTOM_V2F_MEMBER(6,7,8,9,10,11,12,13)
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };
#else
    #define LIL_V2F_POSITION_WS
    #define LIL_V2F_POSITION_CS
    #define LIL_V2F_PACKED_TEXCOORD01
    #define LIL_V2F_PACKED_TEXCOORD23
    #define LIL_V2F_NORMAL_WS
    #define LIL_V2F_UVMAT
    #if !defined(LIL_PASS_FORWARDADD)
        #define LIL_V2F_LIGHTCOLOR
        #define LIL_V2F_LIGHTDIRECTION
        #define LIL_V2F_INDLIGHTCOLOR
    #endif
    #define LIL_V2F_VERTEXLIGHT_FOG

    struct v2f
    {
        float4 positionCS   : SV_POSITION;
        float4 uv01         : TEXCOORD0;
        float4 uv23         : TEXCOORD1;
        float2 uvMat        : TEXCOORD2;
        float3 normalWS     : TEXCOORD3;
        float3 positionWS   : TEXCOORD4;
        LIL_LIGHTCOLOR_COORDS(5)
        LIL_LIGHTDIRECTION_COORDS(6)
        LIL_INDLIGHTCOLOR_COORDS(7)
        LIL_VERTEXLIGHT_FOG_COORDS(8)
        LIL_CUSTOM_V2F_MEMBER(9,10,11,12,13,14,15,16)
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Shader
#include "Includes/lil_common_vert.hlsl"
#include "Includes/lil_common_frag.hlsl"

float4 frag(v2f input LIL_VFACE(facing)) : SV_Target
{
    //------------------------------------------------------------------------------------------------------------------------------
    // Initialize
    lilFragData fd = lilInitFragData();

    BEFORE_UNPACK_V2F
    OVERRIDE_UNPACK_V2F
    LIL_COPY_VFACE(fd.facing);
    LIL_SETUP_INSTANCE_ID(input);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    LIL_GET_HDRPDATA(input,fd);
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
    // Apply Matelial & Lighting
    #if defined(LIL_OUTLINE)
        //------------------------------------------------------------------------------------------------------------------------------
        // UV
        BEFORE_ANIMATE_OUTLINE_UV
        OVERRIDE_ANIMATE_OUTLINE_UV

        //------------------------------------------------------------------------------------------------------------------------------
        // Main Color
        BEFORE_OUTLINE_COLOR
        OVERRIDE_OUTLINE_COLOR

        //------------------------------------------------------------------------------------------------------------------------------
        // Alpha
        #if LIL_RENDER == 0
            // Opaque
        #elif LIL_RENDER == 1
            // Cutout
            clip(fd.col.a - _Cutoff);
        #elif LIL_RENDER == 2
            // Transparent
            clip(fd.col.a - _Cutoff);
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Copy
        fd.albedo = fd.col.rgb;

        //------------------------------------------------------------------------------------------------------------------------------
        // Lighting
        fd.col.rgb = lerp(fd.col.rgb, fd.col.rgb * min(fd.lightColor + fd.addLightColor, _LightMaxLimit), _OutlineEnableLighting);
    #else
        //------------------------------------------------------------------------------------------------------------------------------
        // UV
        BEFORE_ANIMATE_MAIN_UV
        OVERRIDE_ANIMATE_MAIN_UV

        //------------------------------------------------------------------------------------------------------------------------------
        // Main Color
        BEFORE_MAIN
        OVERRIDE_MAIN
        fd.triMask = LIL_SAMPLE_2D(_TriMask, sampler_MainTex, fd.uvMain);

        //------------------------------------------------------------------------------------------------------------------------------
        // Alpha
        #if LIL_RENDER == 0
            // Opaque
        #elif LIL_RENDER == 1
            // Cutout
            clip(fd.col.a - _Cutoff);
        #elif LIL_RENDER == 2
            // Transparent
            clip(fd.col.a - _Cutoff);
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Normal
        fd.N = normalize(input.normalWS);
        fd.N = fd.facing < (_FlipNormal-1.0) ? -fd.N : fd.N;
        fd.ln = dot(fd.L, fd.N);

        //------------------------------------------------------------------------------------------------------------------------------
        // MatCap
        BEFORE_MATCAP
        OVERRIDE_MATCAP

        //------------------------------------------------------------------------------------------------------------------------------
        // Copy
        fd.albedo = fd.col.rgb;

        //------------------------------------------------------------------------------------------------------------------------------
        // Lighting
        BEFORE_SHADOW
        #ifndef LIL_PASS_FORWARDADD
            OVERRIDE_SHADOW

            fd.lightColor += fd.addLightColor;
            fd.shadowmix += lilLuminance(fd.addLightColor);
            fd.col.rgb += fd.albedo * fd.addLightColor;

            fd.lightColor = min(fd.lightColor, _LightMaxLimit);
            fd.shadowmix = saturate(fd.shadowmix);
            fd.col.rgb = min(fd.col.rgb, fd.albedo * _LightMaxLimit);
        #else
            fd.col.rgb *= fd.lightColor;
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Rim light
        fd.nvabs = abs(dot(fd.N, fd.V));
        fd.uvRim = float2(fd.nvabs,fd.nvabs);
        BEFORE_RIMLIGHT
        OVERRIDE_RIMLIGHT

        #ifndef LIL_PASS_FORWARDADD
            BEFORE_EMISSION_1ST
            OVERRIDE_EMISSION_1ST

            BEFORE_BLEND_EMISSION
            OVERRIDE_BLEND_EMISSION
        #endif
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