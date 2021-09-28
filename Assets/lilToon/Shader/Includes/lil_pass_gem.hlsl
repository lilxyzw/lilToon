#ifndef LIL_PASS_GEM_INCLUDED
#define LIL_PASS_GEM_INCLUDED

//------------------------------------------------------------------------------------------------------------------
// Pass for gem shader (Pre ForwardBase)

#include "Includes/lil_pipeline.hlsl"

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
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };

    #include "Includes/lil_common_vert.hlsl"
    #include "lil_common_frag.hlsl"

    #if defined(LIL_CUSTOM_V2F)
    float4 frag(LIL_CUSTOM_V2F inputCustom) : SV_Target
    {
        v2f input = inputCustom.base;
    #else
    float4 frag(v2f input) : SV_Target
    {
    #endif
        LIL_SETUP_INSTANCE_ID(input);
        LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        LIL_GET_HDRPDATA(input);
        #if defined(LIL_HDRP)
            float3 viewDirection = normalize(LIL_GET_VIEWDIR_WS(input.positionWS.xyz));
        #endif
        float4 col = 0;
        OVERRIDE_FOG
        return col;
    }
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
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };

    #include "Includes/lil_common_vert.hlsl"
    #include "Includes/lil_common_frag.hlsl"

    #if defined(LIL_CUSTOM_V2F)
    float4 frag(LIL_CUSTOM_V2F inputCustom LIL_VFACE(facing)) : SV_Target
    {
        v2f input = inputCustom.base;
    #else
    float4 frag(v2f input LIL_VFACE(facing)) : SV_Target
    {
    #endif
        LIL_VFACE_FALLBACK(facing);
        LIL_SETUP_INSTANCE_ID(input);
        LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        LIL_GET_HDRPDATA(input);
        #if defined(LIL_V2F_LIGHTDIRECTION)
            float3 lightDirection = input.lightDirection;
        #else
            float3 lightDirection = float3(0.0, 1.0, 0.0);
        #endif
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
        // UV
        BEFORE_ANIMATE_MAIN_UV
        OVERRIDE_ANIMATE_MAIN_UV

        //------------------------------------------------------------------------------------------------------------------------------
        // View Direction
        float3 viewDirection = normalize(LIL_GET_VIEWDIR_WS(input.positionWS.xyz));
        float3 headDirection = normalize(LIL_GET_HEADDIR_WS(input.positionWS.xyz));
        #if defined(USING_STEREO_MATRICES)
            float3 gemViewDirection = lerp(headDirection, viewDirection, _GemVRParallaxStrength);
        #else
            float3 gemViewDirection = viewDirection;
        #endif
        #if defined(LIL_V2F_NORMAL_WS) && defined(LIL_V2F_TANGENT_WS) && defined(LIL_V2F_BITANGENT_WS)
            float3x3 tbnWS = float3x3(input.tangentWS.xyz, input.bitangentWS, input.normalWS);
            float3 parallaxViewDirection = mul(tbnWS, viewDirection);
            float2 parallaxOffset = (parallaxViewDirection.xy / (parallaxViewDirection.z+0.5));
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Main Color
        float4 col = 1.0;
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

            float3 normalDirection = mul(normalmap, tbnWS);
            normalDirection = facing < 0.0 ? -normalDirection - viewDirection * 0.2 : normalDirection;
            normalDirection = normalize(normalDirection);
        #else
            float3 normalDirection = input.normalWS;
            normalDirection = facing < 0.0 ? -normalDirection - viewDirection * 0.2 : normalDirection;
            normalDirection = normalize(normalDirection);
        #endif
        float nvabs = abs(dot(normalDirection, viewDirection));
        float nv = nvabs;
        float nv1 = abs(dot(normalDirection, gemViewDirection));
        float nv2 = abs(dot(normalDirection, gemViewDirection.yzx));
        float nv3 = abs(dot(normalDirection, gemViewDirection.zxy));
        float invnv = 1-nv1;

        //------------------------------------------------------------------------------------------------------------------------------
        // AudioLink (https://github.com/llealloo/vrc-udon-audio-link)
        float audioLinkValue = 1.0;
        BEFORE_AUDIOLINK
        #if defined(LIL_FEATURE_AUDIOLINK)
            OVERRIDE_AUDIOLINK
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Lighting
        #ifndef LIL_PASS_FORWARDADD
            float shadowmix = saturate(dot(lightDirection, normalDirection));
            lightColor = saturate(lightColor + addLightColor);
            shadowmix = saturate(shadowmix + lilLuminance(addLightColor));
        #endif

        float3 albedo = col.rgb;
        col.rgb *= nv;
        float4 baseCol = col;
        col.rgb *= 0.75;

        //------------------------------------------------------------------------------------------------------------------------------
        // Refraction
        float2 scnUV = input.positionSS.xy/input.positionSS.w;
        float2 ref = mul((float3x3)UNITY_MATRIX_V, normalDirection).xy;
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

        float4 fogColor = float4(0,0,0,0);
        BEFORE_FOG
        OVERRIDE_FOG

        BEFORE_OUTPUT
        OVERRIDE_OUTPUT
    }
#endif

#endif