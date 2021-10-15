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
        LIL_CUSTOM_V2F_MEMBER(8,9,10,11,12,13,14,15)
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
        LIL_CUSTOM_V2F_MEMBER(9,10,11,12,13,14,15,16)
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
        #endif
        #if defined(LIL_FEATURE_SHADOW) || defined(LIL_FEATURE_BACKLIGHT)
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
        LIL_CUSTOM_V2F_MEMBER(14,15,16,17,18,19,20,21)
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
    float3 lightDirection = float3(0.0, 1.0, 0.0);
    float3 lightColor = 1.0;
    float3 addLightColor = 0.0;
    float attenuation = 1.0;

    float4 col = 1.0;
    float3 albedo = 1.0;
    float3 emissionColor = 0.0;

    float3 normalDirection = 0.0;
    float3 viewDirection = 0.0;
    float3 headDirection = 0.0;
    float3x3 tbnWS = 0.0;
    float depth = 0.0;
    float3 parallaxViewDirection = 0.0;
    float2 parallaxOffset = 0.0;

    float vl = 0.0;
    float hl = 0.0;
    float ln = 0.0;
    float nv = 0.0;
    float nvabs = 0.0;

    bool isRightHand = true;
    float shadowmix = 1.0;
    float audioLinkValue = 1.0;
    float3 invLighting = 0.0;

    LIL_VFACE_FALLBACK(facing);
    LIL_SETUP_INSTANCE_ID(input);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    LIL_GET_HDRPDATA(input);
    #if defined(LIL_V2F_LIGHTDIRECTION)
        lightDirection = input.lightDirection;
    #endif
    LIL_GET_MAINLIGHT(input, lightColor, lightDirection, attenuation);
    LIL_GET_ADDITIONALLIGHT(input, addLightColor);
    invLighting = saturate((1.0 - lightColor) * sqrt(lightColor));

    //------------------------------------------------------------------------------------------------------------------------------
    // View Direction
    #if defined(LIL_V2F_POSITION_WS)
        depth = length(lilViewDirection(input.positionWS));
        viewDirection = normalize(lilViewDirection(input.positionWS));
        headDirection = normalize(lilHeadDirection(input.positionWS));
        vl = dot(viewDirection, lightDirection);
        hl = dot(headDirection, lightDirection);
    #endif
    #if defined(LIL_V2F_NORMAL_WS) && defined(LIL_V2F_TANGENT_WS) && defined(LIL_V2F_BITANGENT_WS)
        tbnWS = float3x3(input.tangentWS.xyz, input.bitangentWS, input.normalWS);
        #if defined(LIL_V2F_POSITION_WS)
            parallaxViewDirection = mul(tbnWS, viewDirection);
            parallaxOffset = (parallaxViewDirection.xy / (parallaxViewDirection.z+0.5));
        #endif
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
            col.a = saturate((col.a - _Cutoff) / max(fwidth(col.a), 0.0001) + 0.5);
        #elif LIL_RENDER == 2 && !defined(LIL_REFRACTION)
            // Transparent
            clip(col.a - _Cutoff);
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Copy
        albedo = col.rgb;

        //------------------------------------------------------------------------------------------------------------------------------
        // Lighting
        col.rgb = lerp(col.rgb, col.rgb * min(lightColor + addLightColor, _LightMaxLimit), _OutlineEnableLighting);
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
            col.a = saturate((col.a - _Cutoff) / max(fwidth(col.a), 0.0001) + 0.5);
        #elif LIL_RENDER == 2 && !defined(LIL_REFRACTION)
            // Transparent
            clip(col.a - _Cutoff);
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Fur AO
        #if LIL_RENDER == 1
            col.rgb *= 1.0-_FurAO;
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Copy
        albedo = col.rgb;

        BEFORE_SHADOW
        #ifndef LIL_PASS_FORWARDADD
            //------------------------------------------------------------------------------------------------------------------------------
            // Lighting
            #if defined(LIL_FEATURE_SHADOW)
                normalDirection = normalize(input.normalWS);
                normalDirection = facing < (_FlipNormal-1.0) ? -normalDirection : normalDirection;
                ln = dot(lightDirection, normalDirection);
                OVERRIDE_SHADOW
            #else
                col.rgb *= lightColor;
            #endif
            col.rgb += albedo * addLightColor;
            col.rgb = min(col.rgb, albedo * _LightMaxLimit);
        #else
            col.rgb *= lightColor;
            // Premultiply for ForwardAdd
            #if LIL_RENDER == 2 && LIL_PREMULTIPLY_FA
                col.rgb *= col.a;
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
            float2 ddxMain = ddx(uvMain);
            float2 ddyMain = ddy(uvMain);
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
            col.a = saturate((col.a - _Cutoff) / max(fwidth(col.a), 0.0001) + 0.5);
        #elif LIL_RENDER == 2 && !defined(LIL_REFRACTION)
            // Transparent
            clip(col.a - _Cutoff);
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

                normalDirection = normalize(mul(normalmap, tbnWS));
                normalDirection = facing < (_FlipNormal-1.0) ? -normalDirection : normalDirection;
            #else
                normalDirection = normalize(input.normalWS);
                normalDirection = facing < (_FlipNormal-1.0) ? -normalDirection : normalDirection;
            #endif
            ln = dot(lightDirection, normalDirection);
            #if defined(LIL_V2F_POSITION_WS)
                nv = saturate(dot(normalDirection, viewDirection));
                nvabs = abs(dot(normalDirection, viewDirection));
            #endif
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
            isRightHand = input.tangentWS.w > 0.0;
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
        albedo = col.rgb;

        //------------------------------------------------------------------------------------------------------------------------------
        // Lighting
        BEFORE_SHADOW
        #ifndef LIL_PASS_FORWARDADD
            #if defined(LIL_FEATURE_SHADOW)
                OVERRIDE_SHADOW
            #else
                col.rgb *= lightColor;
            #endif

            lightColor += addLightColor;
            shadowmix += lilLuminance(addLightColor);
            col.rgb += albedo * addLightColor;

            lightColor = min(lightColor, _LightMaxLimit);
            shadowmix = saturate(shadowmix);
            col.rgb = min(col.rgb, albedo * _LightMaxLimit);

            #if defined(LIL_FEATURE_MAIN2ND)
                if(_UseMain2ndTex) col.rgb = lilBlendColor(col.rgb, color2nd.rgb, color2nd.a - color2nd.a * _Main2ndEnableLighting, _Main2ndTexBlendMode);
            #endif
            #if defined(LIL_FEATURE_MAIN3RD)
                if(_UseMain3rdTex) col.rgb = lilBlendColor(col.rgb, color3rd.rgb, color3rd.a - color3rd.a * _Main3rdEnableLighting, _Main3rdTexBlendMode);
            #endif
        #else
            col.rgb *= lightColor;
            #if defined(LIL_FEATURE_MAIN2ND)
                if(_UseMain2ndTex) col.rgb = lerp(col.rgb, 0, color2nd.a - color2nd.a * _Main2ndEnableLighting);
            #endif
            #if defined(LIL_FEATURE_MAIN3RD)
                if(_UseMain3rdTex) col.rgb = lerp(col.rgb, 0, color3rd.a - color3rd.a * _Main3rdEnableLighting);
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
        #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
            col.rgb *= col.a;
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Refraction
        BEFORE_REFRACTION
        #if defined(LIL_REFRACTION) && !defined(LIL_PASS_FORWARDADD)
            #if defined(LIL_REFRACTION_BLUR2) && defined(LIL_FEATURE_REFLECTION)
                float smoothness = _Smoothness;
                if(Exists_SmoothnessTex) smoothness *= LIL_SAMPLE_2D(_SmoothnessTex, sampler_MainTex, uvMain).r;
                float perceptualRoughness = 1.0 - smoothness;
                float roughness = perceptualRoughness * perceptualRoughness;
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
                    emissionColor += _Main2ndDissolveColor.rgb * main2ndDissolveAlpha;
                #endif
                #if defined(LIL_FEATURE_MAIN3RD)
                    emissionColor += _Main3rdDissolveColor.rgb * main3rdDissolveAlpha;
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