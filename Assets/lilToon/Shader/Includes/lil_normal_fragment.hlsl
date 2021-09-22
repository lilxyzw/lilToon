#ifndef LIL_FRAGMENT_INCLUDED
#define LIL_FRAGMENT_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Fragment shader
#include "Includes/lil_common_frag.hlsl"

#if defined(LIL_CUSTOM_V2F)
float4 frag(LIL_CUSTOM_V2F inputCustom, float facing : VFACE) : SV_Target
{
    v2f input = inputCustom.base;
#else
float4 frag(v2f input, float facing : VFACE) : SV_Target
{
#endif
    LIL_SETUP_INSTANCE_ID(input);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    LIL_GET_HDRPDATA(input);
    LIL_GET_MAINLIGHT(input, lightColor, lightDirection, attenuation);
    LIL_GET_VERTEXLIGHT(input, vertexLightColor);
    LIL_GET_ADDITIONALLIGHT(input.positionWS, additionalLightColor);
    #if !defined(LIL_PASS_FORWARDADD)
        #if defined(LIL_USE_LIGHTMAP)
            lightColor = clamp(lightColor, _LightMinLimit, _LightMaxLimit);
            lightColor = lerp(lightColor, lilGray(lightColor), _MonochromeLighting);
            lightColor = lerp(lightColor, 1.0, _AsUnlit);
        #endif
        #if defined(LIL_HDRP)
            float3 addLightColor = lerp(additionalLightColor, 0.0, _AsUnlit);
        #elif defined(_ADDITIONAL_LIGHTS)
            float3 addLightColor = vertexLightColor + lerp(additionalLightColor, 0.0, _AsUnlit);
        #else
            float3 addLightColor = vertexLightColor;
        #endif
        addLightColor = lerp(addLightColor, lilGray(addLightColor), _MonochromeLighting);
    #else
        lightColor = lerp(lightColor, lilGray(lightColor), _MonochromeLighting);
        lightColor = lerp(lightColor, 0.0, _AsUnlit);
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
        float4 col = 1.0;
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
        float3 albedo = col.rgb;

        //------------------------------------------------------------------------------------------------------------------------------
        // Lighting
        col.rgb = lerp(col.rgb, col.rgb * min(lightColor + addLightColor, _LightMaxLimit), _OutlineEnableLighting);

        #if defined(LIL_HDRP)
            float3 viewDirection = normalize(LIL_GET_VIEWDIR_WS(input.positionWS.xyz));
        #endif
    #elif defined(LIL_FUR)
        //------------------------------------------------------------------------------------------------------------------------------
        // UV
        BEFORE_ANIMATE_MAIN_UV
        OVERRIDE_ANIMATE_MAIN_UV

        //------------------------------------------------------------------------------------------------------------------------------
        // Main Color
        float4 col = 1.0;
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
        float3 albedo = col.rgb;

        BEFORE_SHADOW
        #ifndef LIL_PASS_FORWARDADD
            //------------------------------------------------------------------------------------------------------------------------------
            // Lighting
            #if defined(LIL_FEATURE_SHADOW)
                float3 normalDirection = normalize(input.normalWS);
                normalDirection = facing < (_FlipNormal-1.0) ? -normalDirection : normalDirection;
                float shadowmix = 1.0;
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

        #if defined(LIL_HDRP)
            float3 viewDirection = normalize(LIL_GET_VIEWDIR_WS(input.positionWS.xyz));
        #endif
    #else
        //------------------------------------------------------------------------------------------------------------------------------
        // UV
        BEFORE_ANIMATE_MAIN_UV
        OVERRIDE_ANIMATE_MAIN_UV

        //------------------------------------------------------------------------------------------------------------------------------
        // View Direction
        #if defined(LIL_SHOULD_POSITION_WS)
            float depth = length(LIL_GET_VIEWDIR_WS(input.positionWS.xyz));
            float3 viewDirection = normalize(LIL_GET_VIEWDIR_WS(input.positionWS.xyz));
            float3 headDirection = normalize(LIL_GET_HEADDIR_WS(input.positionWS.xyz));
        #endif
        #if defined(LIL_SHOULD_TBN)
            float3x3 tbnWS = float3x3(input.tangentWS, input.bitangentWS, input.normalWS);
            #if defined(LIL_SHOULD_POSITION_WS)
                float3 parallaxViewDirection = mul(tbnWS, viewDirection);
                float2 parallaxOffset = (parallaxViewDirection.xy / (parallaxViewDirection.z+0.5));
            #endif
        #endif

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
        float4 col = 1.0;
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
        #if defined(LIL_SHOULD_NORMAL)
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

                float3 normalDirection = normalize(mul(normalmap, tbnWS));
                normalDirection = facing < (_FlipNormal-1.0) ? -normalDirection : normalDirection;
            #else
                float3 normalDirection = normalize(input.normalWS);
                normalDirection = facing < (_FlipNormal-1.0) ? -normalDirection : normalDirection;
            #endif
            #if defined(LIL_SHOULD_POSITION_WS)
                float nv = saturate(dot(normalDirection, viewDirection));
                float nvabs = abs(dot(normalDirection, viewDirection));
            #else
                float nv = 1.0;
                float nvabs = 1.0;
            #endif
        #else
            float nv = 1.0;
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // AudioLink (https://github.com/llealloo/vrc-udon-audio-link)
        float audioLinkValue = 1.0;
        BEFORE_AUDIOLINK
        #if defined(LIL_FEATURE_AUDIOLINK)
            OVERRIDE_AUDIOLINK
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Layer Color
        #if defined(LIL_SHOULD_TANGENT_W)
            bool isRightHand = input.tangentW > 0.0;
        #else
            bool isRightHand = true;
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
        float3 albedo = col.rgb;

        //------------------------------------------------------------------------------------------------------------------------------
        // Lighting
        float shadowmix = 1.0;
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
            float3 invLighting = saturate((1.0 - lightColor) * sqrt(lightColor));
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
                    col.rgb += _Main2ndDissolveColor.rgb * main2ndDissolveAlpha;
                #endif
                #if defined(LIL_FEATURE_MAIN3RD)
                    col.rgb += _Main3rdDissolveColor.rgb * main3rdDissolveAlpha;
                #endif
            #endif
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

#endif