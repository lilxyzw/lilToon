#ifndef LIL_PASS_FORWARD_NORMAL_INCLUDED
#define LIL_PASS_FORWARD_NORMAL_INCLUDED

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
    #endif
    #define LIL_V2F_VERTEXLIGHT_FOG

    struct v2f
    {
        float4 positionCS   : SV_POSITION;
        float4 uv01         : TEXCOORD0;
        float4 uv23         : TEXCOORD1;
        #if defined(LIL_V2F_POSITION_OS)
            float3 positionOS   : TEXCOORD2;
        #endif
        #if defined(LIL_V2F_POSITION_WS)
            float3 positionWS   : TEXCOORD3;
        #endif
        #if defined(LIL_V2F_NORMAL_WS)
            float3 normalWS     : TEXCOORD4;
        #endif
        LIL_LIGHTCOLOR_COORDS(5)
        LIL_VERTEXLIGHT_FOG_COORDS(6)
        LIL_CUSTOM_V2F_MEMBER(7,8,9,10,11,12,13,14)
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };
#elif defined(LIL_FUR)
    #define LIL_V2F_POSITION_CS
    #define LIL_V2F_PACKED_TEXCOORD01
    #define LIL_V2F_PACKED_TEXCOORD23
    #if defined(LIL_V2F_FORCE_POSITION_WS) || defined(LIL_PASS_FORWARDADD) || defined(LIL_FEATURE_DISTANCE_FADE) || !defined(LIL_BRP) || defined(LIL_USE_LPPV)
        #define LIL_V2F_POSITION_WS
    #endif
    #if defined(LIL_V2F_FORCE_NORMAL) || !defined(LIL_PASS_FORWARDADD) &&  defined(LIL_SHOULD_NORMAL)
        #define LIL_V2F_NORMAL_WS
    #endif
    #if !defined(LIL_PASS_FORWARDADD)
        #define LIL_V2F_LIGHTCOLOR
        #define LIL_V2F_LIGHTDIRECTION
        #if defined(LIL_FEATURE_SHADOW)
            #define LIL_V2F_INDLIGHTCOLOR
        #endif
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
        LIL_LIGHTDIRECTION_COORDS(5)
        LIL_INDLIGHTCOLOR_COORDS(6)
        LIL_VERTEXLIGHT_FOG_COORDS(7)
        LIL_CUSTOM_V2F_MEMBER(8,9,10,11,12,13,14,15)
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };
#else
    #define LIL_V2F_POSITION_CS
    #define LIL_V2F_PACKED_TEXCOORD01
    #define LIL_V2F_PACKED_TEXCOORD23
    #if defined(LIL_V2F_FORCE_POSITION_OS) || defined(LIL_SHOULD_POSITION_OS)
        #define LIL_V2F_POSITION_OS
    #endif
    #if defined(LIL_V2F_FORCE_POSITION_WS) || defined(LIL_SHOULD_POSITION_WS)
        #define LIL_V2F_POSITION_WS
    #endif
    #if defined(LIL_V2F_FORCE_NORMAL) || defined(LIL_SHOULD_NORMAL)
        #define LIL_V2F_NORMAL_WS
    #endif
    #if defined(LIL_V2F_FORCE_TANGENT) || defined(LIL_SHOULD_TBN)
        #define LIL_V2F_TANGENT_WS
    #endif
    #if !defined(LIL_PASS_FORWARDADD)
        #define LIL_V2F_LIGHTCOLOR
        #define LIL_V2F_LIGHTDIRECTION
        #if defined(LIL_FEATURE_SHADOW)
            #define LIL_V2F_INDLIGHTCOLOR
        #endif
        #if defined(LIL_FEATURE_SHADOW) || defined(LIL_FEATURE_BACKLIGHT)
            #define LIL_V2F_SHADOW
        #endif
    #endif
    #define LIL_V2F_VERTEXLIGHT_FOG

    struct v2f
    {
        float4 positionCS   : SV_POSITION;
        float4 uv01         : TEXCOORD0;
        float4 uv23         : TEXCOORD1;
        #if defined(LIL_V2F_POSITION_OS)
            float3 positionOS   : TEXCOORD2;
        #endif
        #if defined(LIL_V2F_POSITION_WS)
            float3 positionWS   : TEXCOORD3;
        #endif
        #if defined(LIL_V2F_NORMAL_WS)
            float3 normalWS     : TEXCOORD4;
        #endif
        #if defined(LIL_V2F_TANGENT_WS)
            float4 tangentWS    : TEXCOORD5;
        #endif
        LIL_LIGHTCOLOR_COORDS(6)
        LIL_LIGHTDIRECTION_COORDS(7)
        LIL_INDLIGHTCOLOR_COORDS(8)
        LIL_VERTEXLIGHT_FOG_COORDS(9)
        LIL_SHADOW_COORDS(10)
        LIL_CUSTOM_V2F_MEMBER(11,12,13,14,15,16,17,18)
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
        // Alpha Mask
        BEFORE_ALPHAMASK
        #if defined(LIL_FEATURE_ALPHAMASK) && LIL_RENDER != 0
            OVERRIDE_ALPHAMASK
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Dissolve
        BEFORE_DISSOLVE
        #if defined(LIL_FEATURE_DISSOLVE) && LIL_RENDER != 0
            float dissolveAlpha = 0.0;
            OVERRIDE_DISSOLVE
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Alpha
        #if LIL_RENDER == 0
            // Opaque
        #elif LIL_RENDER == 1
            // Cutout
            fd.col.a = saturate((fd.col.a - _Cutoff) / max(fwidth(fd.col.a), 0.0001) + 0.5);
        #elif LIL_RENDER == 2 && !defined(LIL_REFRACTION)
            // Transparent
            clip(fd.col.a - _Cutoff);
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Copy
        fd.albedo = fd.col.rgb;

        //------------------------------------------------------------------------------------------------------------------------------
        // Lighting
        fd.col.rgb = lerp(fd.col.rgb, fd.col.rgb * min(fd.lightColor + fd.addLightColor, _LightMaxLimit), _OutlineEnableLighting);
    #elif defined(LIL_FUR)
        //------------------------------------------------------------------------------------------------------------------------------
        // UV
        BEFORE_ANIMATE_MAIN_UV
        OVERRIDE_ANIMATE_MAIN_UV

        //------------------------------------------------------------------------------------------------------------------------------
        // Main Color
        BEFORE_MAIN
        OVERRIDE_MAIN

        //------------------------------------------------------------------------------------------------------------------------------
        // Alpha
        #if LIL_RENDER == 0
            // Opaque
        #elif LIL_RENDER == 1
            // Cutout
            fd.col.a = saturate((fd.col.a - _Cutoff) / max(fwidth(fd.col.a), 0.0001) + 0.5);
        #elif LIL_RENDER == 2 && !defined(LIL_REFRACTION)
            // Transparent
            clip(fd.col.a - _Cutoff);
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Fur AO
        #if LIL_RENDER == 1
            fd.col.rgb *= 1.0-_FurAO;
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Copy
        fd.albedo = fd.col.rgb;

        BEFORE_SHADOW
        #ifndef LIL_PASS_FORWARDADD
            //------------------------------------------------------------------------------------------------------------------------------
            // Lighting
            #if defined(LIL_FEATURE_SHADOW)
                fd.N = normalize(input.normalWS);
                fd.N = fd.facing < (_FlipNormal-1.0) ? -fd.N : fd.N;
                fd.ln = dot(fd.L, fd.N);
                OVERRIDE_SHADOW
            #else
                fd.col.rgb *= fd.lightColor;
            #endif
            fd.col.rgb += fd.albedo * fd.addLightColor;
            fd.col.rgb = min(fd.col.rgb, fd.albedo * _LightMaxLimit);
        #else
            fd.col.rgb *= fd.lightColor;
            // Premultiply for ForwardAdd
            #if LIL_RENDER == 2 && LIL_PREMULTIPLY_FA
                fd.col.rgb *= fd.col.a;
            #endif
        #endif
    #else
        //------------------------------------------------------------------------------------------------------------------------------
        // UV
        BEFORE_ANIMATE_MAIN_UV
        OVERRIDE_ANIMATE_MAIN_UV

        //------------------------------------------------------------------------------------------------------------------------------
        // Parallax
        BEFORE_PARALLAX
        #if defined(LIL_FEATURE_PARALLAX)
            float2 ddxMain = ddx(fd.uvMain);
            float2 ddyMain = ddy(fd.uvMain);
            OVERRIDE_PARALLAX
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Main Color
        BEFORE_MAIN
        OVERRIDE_MAIN

        //------------------------------------------------------------------------------------------------------------------------------
        // Alpha Mask
        BEFORE_ALPHAMASK
        #if defined(LIL_FEATURE_ALPHAMASK) && LIL_RENDER != 0
            OVERRIDE_ALPHAMASK
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Dissolve
        BEFORE_DISSOLVE
        #if defined(LIL_FEATURE_DISSOLVE) && LIL_RENDER != 0
            float dissolveAlpha = 0.0;
            OVERRIDE_DISSOLVE
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Alpha
        #if LIL_RENDER == 0
            // Opaque
        #elif LIL_RENDER == 1
            // Cutout
            fd.col.a = saturate((fd.col.a - _Cutoff) / max(fwidth(fd.col.a), 0.0001) + 0.5);
        #elif LIL_RENDER == 2 && !defined(LIL_REFRACTION)
            // Transparent
            clip(fd.col.a - _Cutoff);
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Normal
        #if defined(LIL_V2F_NORMAL_WS)
            #if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)
                float3 normalmap = float3(0.0,0.0,1.0);

                // 1st
                BEFORE_NORMAL_1ST
                #if defined(LIL_FEATURE_NORMAL_1ST)
                    OVERRIDE_NORMAL_1ST
                #endif

                // 2nd
                BEFORE_NORMAL_2ND
                #if defined(LIL_FEATURE_NORMAL_2ND)
                    OVERRIDE_NORMAL_2ND
                #endif

                fd.N = normalize(mul(normalmap, fd.TBN));
                fd.N = fd.facing < (_FlipNormal-1.0) ? -fd.N : fd.N;
            #else
                fd.N = normalize(input.normalWS);
                fd.N = fd.facing < (_FlipNormal-1.0) ? -fd.N : fd.N;
            #endif
            fd.ln = dot(fd.L, fd.N);
            #if defined(LIL_V2F_POSITION_WS)
                fd.nv = saturate(dot(fd.N, fd.V));
                fd.nvabs = abs(dot(fd.N, fd.V));
                fd.uvRim = float2(fd.nvabs,fd.nvabs);
            #endif
            fd.origN = normalize(input.normalWS);
        #endif
        fd.reflectionN = fd.N;
        fd.matcapN = fd.N;
        fd.matcap2ndN = fd.N;

        //------------------------------------------------------------------------------------------------------------------------------
        // Anisotropy
        BEFORE_ANISOTROPY
        #if defined(LIL_FEATURE_ANISOTROPY)
            OVERRIDE_ANISOTROPY
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // AudioLink (https://github.com/llealloo/vrc-udon-audio-link)
        BEFORE_AUDIOLINK
        #if defined(LIL_FEATURE_AUDIOLINK)
            OVERRIDE_AUDIOLINK
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Layer Color
        #if defined(LIL_V2F_TANGENT_WS)
            fd.isRightHand = input.tangentWS.w > 0.0;
        #endif
        // 2nd
        BEFORE_MAIN2ND
        #if defined(LIL_FEATURE_MAIN2ND)
            float main2ndDissolveAlpha = 0.0;
            float4 color2nd = 1.0;
            OVERRIDE_MAIN2ND
        #endif

        // 3rd
        BEFORE_MAIN3RD
        #if defined(LIL_FEATURE_MAIN3RD)
            float main3rdDissolveAlpha = 0.0;
            float4 color3rd = 1.0;
            OVERRIDE_MAIN3RD
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Copy
        fd.albedo = fd.col.rgb;

        //------------------------------------------------------------------------------------------------------------------------------
        // Lighting
        BEFORE_SHADOW
        #ifndef LIL_PASS_FORWARDADD
            #if defined(LIL_FEATURE_SHADOW)
                OVERRIDE_SHADOW
            #else
                fd.col.rgb *= fd.lightColor;
            #endif

            fd.lightColor += fd.addLightColor;
            fd.shadowmix += lilLuminance(fd.addLightColor);
            fd.col.rgb += fd.albedo * fd.addLightColor;

            fd.lightColor = min(fd.lightColor, _LightMaxLimit);
            fd.shadowmix = saturate(fd.shadowmix);
            fd.col.rgb = min(fd.col.rgb, fd.albedo * _LightMaxLimit);

            #if defined(LIL_FEATURE_MAIN2ND)
                if(_UseMain2ndTex) fd.col.rgb = lilBlendColor(fd.col.rgb, color2nd.rgb, color2nd.a - color2nd.a * _Main2ndEnableLighting, _Main2ndTexBlendMode);
            #endif
            #if defined(LIL_FEATURE_MAIN3RD)
                if(_UseMain3rdTex) fd.col.rgb = lilBlendColor(fd.col.rgb, color3rd.rgb, color3rd.a - color3rd.a * _Main3rdEnableLighting, _Main3rdTexBlendMode);
            #endif
        #else
            fd.col.rgb *= fd.lightColor;
            #if defined(LIL_FEATURE_MAIN2ND)
                if(_UseMain2ndTex) fd.col.rgb = lerp(fd.col.rgb, 0, color2nd.a - color2nd.a * _Main2ndEnableLighting);
            #endif
            #if defined(LIL_FEATURE_MAIN3RD)
                if(_UseMain3rdTex) fd.col.rgb = lerp(fd.col.rgb, 0, color3rd.a - color3rd.a * _Main3rdEnableLighting);
            #endif
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Backlight
        BEFORE_BACKLIGHT
        #if !defined(LIL_PASS_FORWARDADD)
            #if defined(LIL_FEATURE_BACKLIGHT)
                OVERRIDE_BACKLIGHT
            #endif
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Premultiply
        #if LIL_RENDER == 2
            fd.col.rgb *= fd.col.a;
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Refraction
        BEFORE_REFRACTION
        #if defined(LIL_REFRACTION) && !defined(LIL_PASS_FORWARDADD)
            #if defined(LIL_REFRACTION_BLUR2) && defined(LIL_FEATURE_REFLECTION)
                fd.smoothness = _Smoothness;
                if(Exists_SmoothnessTex) fd.smoothness *= LIL_SAMPLE_2D_ST(_SmoothnessTex, sampler_MainTex, fd.uvMain).r;
                fd.perceptualRoughness = fd.perceptualRoughness - fd.smoothness * fd.perceptualRoughness;
                fd.roughness = fd.perceptualRoughness * fd.perceptualRoughness;
            #endif
            OVERRIDE_REFRACTION
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Reflection
        BEFORE_REFLECTION
        #if defined(LIL_FEATURE_REFLECTION)
            OVERRIDE_REFLECTION
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // MatCap
        BEFORE_MATCAP
        #if defined(LIL_FEATURE_MATCAP)
            OVERRIDE_MATCAP
        #endif

        BEFORE_MATCAP_2ND
        #if defined(LIL_FEATURE_MATCAP_2ND)
            OVERRIDE_MATCAP_2ND
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Rim light
        BEFORE_RIMLIGHT
        #if defined(LIL_FEATURE_RIMLIGHT)
            OVERRIDE_RIMLIGHT
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Glitter
        BEFORE_GLITTER
        #if defined(LIL_FEATURE_GLITTER)
            OVERRIDE_GLITTER
        #endif

        #ifndef LIL_PASS_FORWARDADD
            //------------------------------------------------------------------------------------------------------------------------------
            // Emission
            BEFORE_EMISSION_1ST
            #if defined(LIL_FEATURE_EMISSION_1ST)
                OVERRIDE_EMISSION_1ST
            #endif

            // Emission2nd
            BEFORE_EMISSION_2ND
            #if defined(LIL_FEATURE_EMISSION_2ND)
                OVERRIDE_EMISSION_2ND
            #endif

            //------------------------------------------------------------------------------------------------------------------------------
            // Dissolve
            #if defined(LIL_FEATURE_DISSOLVE) && LIL_RENDER != 0
                OVERRIDE_DISSOLVE_ADD
            #endif

            #if defined(LIL_FEATURE_LAYER_DISSOLVE)
                #if defined(LIL_FEATURE_MAIN2ND)
                    fd.emissionColor += _Main2ndDissolveColor.rgb * main2ndDissolveAlpha;
                #endif
                #if defined(LIL_FEATURE_MAIN3RD)
                    fd.emissionColor += _Main3rdDissolveColor.rgb * main3rdDissolveAlpha;
                #endif
            #endif

            BEFORE_BLEND_EMISSION
            OVERRIDE_BLEND_EMISSION
        #endif
    #endif

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

#if defined(LIL_TESSELLATION)
    #include "Includes/lil_tessellation.hlsl"
#endif

#endif