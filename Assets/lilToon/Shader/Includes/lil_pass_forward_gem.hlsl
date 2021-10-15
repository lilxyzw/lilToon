#ifndef LIL_PASS_FORWARD_GEM_INCLUDED
#define LIL_PASS_FORWARD_GEM_INCLUDED

#include "Includes/lil_pipeline.hlsl"
#include "Includes/lil_common_appdata.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Structure
#if !defined(LIL_CUSTOM_V2F_MEMBER)
    #define LIL_CUSTOM_V2F_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7)
#endif

#if defined(LIL_GEM_PRE)
    #define LIL_V2F_POSITION_CS
    #if defined(LIL_V2F_FORCE_POSITION_WS) || defined(LIL_HDRP)
        #define LIL_V2F_POSITION_WS
    #endif
    #define LIL_V2F_FOG

    struct v2f
    {
        float4 positionCS : SV_POSITION;
        #if defined(LIL_V2F_POSITION_WS)
            float3 positionWS : TEXCOORD0;
        #endif
        LIL_FOG_COORDS(1)
        LIL_CUSTOM_V2F_MEMBER(2,3,4,5,6,7,8,9)
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
    #define LIL_V2F_POSITION_WS
    #define LIL_V2F_POSITION_SS
    #define LIL_V2F_NORMAL_WS
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
    #endif
    #define LIL_V2F_FOG

    struct v2f
    {
        float4 positionCS : SV_POSITION;
        float2 uv : TEXCOORD0;
        #if defined(LIL_V2F_TEXCOORD1)
            float2 uv1          : TEXCOORD1;
        #endif
        float3 normalWS : TEXCOORD2;
        float3 positionWS : TEXCOORD3;
        float4 positionSS : TEXCOORD4;
        #if defined(LIL_V2F_TANGENT_WS)
            float4 tangentWS        : TEXCOORD5;
        #endif
        #if defined(LIL_V2F_BITANGENT_WS)
            float3 bitangentWS      : TEXCOORD6;
        #endif
        #if defined(LIL_V2F_POSITION_OS)
            float3 positionOS       : TEXCOORD7;
        #endif
        LIL_LIGHTCOLOR_COORDS(8)
        LIL_LIGHTDIRECTION_COORDS(9)
        LIL_VERTEXLIGHT_COORDS(10)
        LIL_FOG_COORDS(11)
        LIL_CUSTOM_V2F_MEMBER(12,13,14,15,16,17,18,19)
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };
#endif

#include "Includes/lil_common_vert.hlsl"
#include "Includes/lil_common_frag.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Shader
#if defined(LIL_GEM_PRE)
    float4 frag(v2f input) : SV_Target
    {
        LIL_SETUP_INSTANCE_ID(input);
        LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        LIL_GET_HDRPDATA(input);
        #if defined(LIL_HDRP)
            float3 viewDirection = normalize(lilViewDirection(input.positionWS));
        #endif
        float4 col = 0;
        OVERRIDE_FOG
        return col;
    }
#else
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
        // UV
        BEFORE_ANIMATE_MAIN_UV
        OVERRIDE_ANIMATE_MAIN_UV

        //------------------------------------------------------------------------------------------------------------------------------
        // Gem View Direction
        #if defined(USING_STEREO_MATRICES)
            float3 gemViewDirection = lerp(headDirection, viewDirection, _GemVRParallaxStrength);
        #else
            float3 gemViewDirection = viewDirection;
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Main Color
        BEFORE_MAIN
        OVERRIDE_MAIN

        //------------------------------------------------------------------------------------------------------------------------------
        // Normal
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

            normalDirection = mul(normalmap, tbnWS);
            normalDirection = facing < 0.0 ? -normalDirection - viewDirection * 0.2 : normalDirection;
            normalDirection = normalize(normalDirection);
        #else
            normalDirection = input.normalWS;
            normalDirection = facing < 0.0 ? -normalDirection - viewDirection * 0.2 : normalDirection;
            normalDirection = normalize(normalDirection);
        #endif
        nvabs = abs(dot(normalDirection, viewDirection));
        nv = nvabs;
        float nv1 = abs(dot(normalDirection, gemViewDirection));
        float nv2 = abs(dot(normalDirection, gemViewDirection.yzx));
        float nv3 = abs(dot(normalDirection, gemViewDirection.zxy));
        float invnv = 1-nv1;
        ln = dot(lightDirection, normalDirection);

        //------------------------------------------------------------------------------------------------------------------------------
        // AudioLink (https://github.com/llealloo/vrc-udon-audio-link)
        BEFORE_AUDIOLINK
        #if defined(LIL_FEATURE_AUDIOLINK)
            OVERRIDE_AUDIOLINK
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Lighting
        #ifndef LIL_PASS_FORWARDADD
            shadowmix = saturate(ln);
            lightColor = saturate(lightColor + addLightColor);
            shadowmix = saturate(shadowmix + lilLuminance(addLightColor));
        #endif

        albedo = col.rgb;
        col.rgb *= nv;
        float4 baseCol = col;
        col.rgb *= 0.75;

        //------------------------------------------------------------------------------------------------------------------------------
        // Refraction
        float2 scnUV = input.positionSS.xy/input.positionSS.w;
        float2 ref = mul((float3x3)LIL_MATRIX_V, normalDirection).xy;
        float nvRef = pow(saturate(1.0 - nv), _RefractionFresnelPower);
        float3 refractColor;
        refractColor.r = LIL_GET_BG_TEX(scnUV + (nvRef * _RefractionStrength) * ref, 0).r;
        refractColor.g = LIL_GET_BG_TEX(scnUV + (nvRef * (_RefractionStrength + _GemChromaticAberration)) * ref, 0).g;
        refractColor.b = LIL_GET_BG_TEX(scnUV + (nvRef * (_RefractionStrength + _GemChromaticAberration * 2)) * ref, 0).b;
        refractColor = pow(saturate(refractColor), _GemEnvContrast) * _GemEnvContrast;
        refractColor = lerp(dot(refractColor, float3(1.0/3.0,1.0/3.0,1.0/3.0)), refractColor, saturate(1.0/_GemEnvContrast));
        col.rgb *= refractColor;

        //------------------------------------------------------------------------------------------------------------------------------
        // Reflection
        float smoothness = _Smoothness;
        if(Exists_SmoothnessTex) smoothness *= LIL_SAMPLE_2D(_SmoothnessTex, sampler_MainTex, uvMain).r;
        float perceptualRoughness = 1.0 - smoothness;
        float roughness = perceptualRoughness * perceptualRoughness;

        float3 normalDirectionR = normalDirection;
        float3 normalDirectionG = facing < 0.0 ? normalize(normalDirection + viewDirection * invnv * _GemChromaticAberration) : normalDirection;
        float3 normalDirectionB = facing < 0.0 ? normalize(normalDirection + viewDirection * invnv * _GemChromaticAberration * 2) : normalDirection;
        float envReflectionColorR = LIL_GET_ENVIRONMENT_REFLECTION(viewDirection, normalDirectionR, perceptualRoughness, input.positionWS).r;
        float envReflectionColorG = LIL_GET_ENVIRONMENT_REFLECTION(viewDirection, normalDirectionG, perceptualRoughness, input.positionWS).g;
        float envReflectionColorB = LIL_GET_ENVIRONMENT_REFLECTION(viewDirection, normalDirectionB, perceptualRoughness, input.positionWS).b;

        float3 envReflectionColor = float3(envReflectionColorR, envReflectionColorG, envReflectionColorB);
        envReflectionColor = pow(saturate(envReflectionColor), _GemEnvContrast) * _GemEnvContrast * _GemEnvColor.rgb;
        envReflectionColor = lerp(dot(envReflectionColor, float3(1.0/3.0,1.0/3.0,1.0/3.0)), envReflectionColor, saturate(1.0/_GemEnvContrast));
        envReflectionColor = facing < 0.0 ? envReflectionColor * baseCol.rgb : envReflectionColor;

        float oneMinusReflectivity = LIL_DIELECTRIC_SPECULAR.a;
        float grazingTerm = saturate(smoothness + (1.0-oneMinusReflectivity));
        #ifdef LIL_COLORSPACE_GAMMA
            float surfaceReduction = 1.0 - 0.28 * roughness * perceptualRoughness;
        #else
            float surfaceReduction = 1.0 / (roughness * roughness + 1.0);
        #endif

        float particle = step(0.5, frac(nv1 * _GemParticleLoop)) * step(0.5, frac(nv2 * _GemParticleLoop)) * step(0.5, frac(nv3 * _GemParticleLoop));
        float3 particleColor = facing < 0.0 ? 1.0 + particle * _GemParticleColor.rgb : 1.0;

        col.rgb += (surfaceReduction * lilFresnelLerp(_Reflectance, grazingTerm, nv) + 0.5) * 0.5 * particleColor * envReflectionColor;

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

        BEFORE_BLEND_EMISSION
        OVERRIDE_BLEND_EMISSION

        float4 fogColor = float4(0,0,0,0);
        BEFORE_FOG
        OVERRIDE_FOG

        BEFORE_OUTPUT
        OVERRIDE_OUTPUT
    }
#endif

#endif