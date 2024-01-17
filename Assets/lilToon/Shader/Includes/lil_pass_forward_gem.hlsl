#ifndef LIL_PASS_FORWARD_GEM_INCLUDED
#define LIL_PASS_FORWARD_GEM_INCLUDED

#include "lil_common.hlsl"
#include "lil_common_appdata.hlsl"

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
    #define LIL_V2F_VERTEXLIGHT_FOG

    struct v2f
    {
        float4 positionCS   : SV_POSITION;
        #if defined(LIL_V2F_POSITION_WS)
            float3 positionWS   : TEXCOORD0;
        #endif
        LIL_VERTEXLIGHT_FOG_COORDS(1)
        LIL_CUSTOM_V2F_MEMBER(2,3,4,5,6,7,8,9)
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
    #define LIL_V2F_POSITION_WS
    #define LIL_V2F_NORMAL_WS
    #if defined(LIL_V2F_FORCE_TANGENT) || defined(LIL_SHOULD_TBN)
        #define LIL_V2F_TANGENT_WS
    #endif
    #if !defined(LIL_PASS_FORWARDADD)
        #define LIL_V2F_LIGHTCOLOR
        #define LIL_V2F_LIGHTDIRECTION
    #endif
    #define LIL_V2F_VERTEXLIGHT_FOG

    struct v2f
    {
        float4 positionCS   : SV_POSITION;
        float4 uv01         : TEXCOORD0;
        float4 uv23         : TEXCOORD1;
        float3 positionWS   : TEXCOORD2;
        LIL_VECTOR_INTERPOLATION float3 normalWS     : TEXCOORD3;
        #if defined(LIL_V2F_TANGENT_WS)
            LIL_VECTOR_INTERPOLATION float4 tangentWS    : TEXCOORD4;
        #endif
        #if defined(LIL_V2F_POSITION_OS)
            float4 positionOSdissolve   : TEXCOORD5;
        #endif
        LIL_LIGHTCOLOR_COORDS(6)
        LIL_LIGHTDIRECTION_COORDS(7)
        LIL_VERTEXLIGHT_FOG_COORDS(8)
        LIL_CUSTOM_V2F_MEMBER(9,10,11,12,13,14,15,16)
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };
#endif

#include "lil_common_vert.hlsl"
#include "lil_common_frag.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Shader
#if defined(LIL_GEM_PRE)
    float4 frag(v2f input) : SV_Target
    {
        LIL_SETUP_INSTANCE_ID(input);
        LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        lilFragData fd = lilInitFragData();

        BEFORE_UNPACK_V2F
        OVERRIDE_UNPACK_V2F
        LIL_GET_HDRPDATA(input,fd);
        #if defined(LIL_HDRP)
            fd.V = normalize(lilViewDirection(input.positionWS));
        #endif
        fd.col = 0;
        OVERRIDE_FOG
        return fd.col;
    }
#else
    float4 frag(v2f input LIL_VFACE(facing)) : SV_Target
    {
        //------------------------------------------------------------------------------------------------------------------------------
        // Initialize
        LIL_SETUP_INSTANCE_ID(input);
        LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        lilFragData fd = lilInitFragData();

        BEFORE_UNPACK_V2F
        OVERRIDE_UNPACK_V2F
        LIL_COPY_VFACE(fd.facing);
        LIL_GET_HDRPDATA(input,fd);
        #if defined(LIL_V2F_SHADOW) || defined(LIL_PASS_FORWARDADD)
            LIL_LIGHT_ATTENUATION(fd.attenuation, input);
        #endif
        LIL_GET_LIGHTING_DATA(input,fd);

        //------------------------------------------------------------------------------------------------------------------------------
        // UDIM Discard
        #if defined(LIL_FEATURE_UDIMDISCARD)
            OVERRIDE_UDIMDISCARD
        #endif
        
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
        // Gem View Direction
        float3 gemViewDirection = lilBlendVRParallax(fd.headV, fd.V, _GemVRParallaxStrength);

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

            fd.N = mul(normalmap, fd.TBN);
            fd.N = fd.facing < 0.0 ? -fd.N - fd.V * 0.2 : fd.N;
            fd.N = normalize(fd.N);
        #else
            fd.N = input.normalWS;
            fd.N = fd.facing < 0.0 ? -fd.N - fd.V * 0.2 : fd.N;
            fd.N = normalize(fd.N);
        #endif
        fd.origN = normalize(input.normalWS);
        fd.uvMat = mul(fd.cameraMatrix, fd.N).xy * 0.5 + 0.5;
        fd.reflectionN = fd.N;
        fd.matcapN = fd.N;
        fd.matcap2ndN = fd.N;

        fd.nvabs = abs(dot(fd.N, fd.V));
        fd.nv = fd.nvabs;
        fd.uvRim = float2(fd.nvabs,fd.nvabs);
        float nv1 = abs(dot(fd.N, gemViewDirection));
        float nv2 = abs(dot(fd.N, gemViewDirection.yzx));
        float nv3 = abs(dot(fd.N, gemViewDirection.zxy));
        float invnv = 1-nv1;
        fd.ln = dot(fd.L, fd.N);

        //------------------------------------------------------------------------------------------------------------------------------
        // Anisotropy
        BEFORE_ANISOTROPY
        #if defined(LIL_FEATURE_ANISOTROPY)
            OVERRIDE_ANISOTROPY
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // AudioLink
        BEFORE_AUDIOLINK
        #if defined(LIL_FEATURE_AUDIOLINK)
            OVERRIDE_AUDIOLINK
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Lighting
        #ifndef LIL_PASS_FORWARDADD
            fd.shadowmix = saturate(fd.ln);
            fd.lightColor = saturate(fd.lightColor + fd.addLightColor);
            fd.shadowmix = saturate(fd.shadowmix + lilLuminance(fd.addLightColor));
        #endif

        fd.albedo = fd.col.rgb;
        fd.col.rgb *= fd.nv;
        float4 baseCol = fd.col;
        fd.col.rgb *= 0.75;

        //------------------------------------------------------------------------------------------------------------------------------
        // Refraction
        float2 ref = mul((float3x3)LIL_MATRIX_V, fd.N).xy;
        float nvRef = pow(saturate(1.0 - fd.nv), _RefractionFresnelPower);
        float3 refractColor;
        refractColor.r = LIL_GET_BG_TEX(fd.uvScn + (nvRef * _RefractionStrength) * ref, 0).r;
        refractColor.g = LIL_GET_BG_TEX(fd.uvScn + (nvRef * (_RefractionStrength + _GemChromaticAberration)) * ref, 0).g;
        refractColor.b = LIL_GET_BG_TEX(fd.uvScn + (nvRef * (_RefractionStrength + _GemChromaticAberration * 2)) * ref, 0).b;
        refractColor = pow(saturate(refractColor), _GemEnvContrast) * _GemEnvContrast;
        refractColor = lerp(dot(refractColor, float3(1.0/3.0,1.0/3.0,1.0/3.0)), refractColor, saturate(1.0/_GemEnvContrast));
        fd.col.rgb *= refractColor;

        //------------------------------------------------------------------------------------------------------------------------------
        // Reflection
        fd.smoothness = _Smoothness;
        #if defined(LIL_FEATURE_SmoothnessTex)
            fd.smoothness *= LIL_SAMPLE_2D(_SmoothnessTex, sampler_MainTex, fd.uvMain).r;
        #endif
        fd.perceptualRoughness = fd.perceptualRoughness - fd.smoothness * fd.perceptualRoughness;
        fd.roughness = fd.perceptualRoughness * fd.perceptualRoughness;

        float3 normalDirectionR = fd.N;
        float3 normalDirectionG = fd.facing < 0.0 ? normalize(fd.N + fd.V * invnv * _GemChromaticAberration) : fd.N;
        float3 normalDirectionB = fd.facing < 0.0 ? normalize(fd.N + fd.V * invnv * _GemChromaticAberration * 2) : fd.N;
        float envReflectionColorR = LIL_GET_ENVIRONMENT_REFLECTION(fd.V, normalDirectionR, fd.perceptualRoughness, fd.positionWS).r;
        float envReflectionColorG = LIL_GET_ENVIRONMENT_REFLECTION(fd.V, normalDirectionG, fd.perceptualRoughness, fd.positionWS).g;
        float envReflectionColorB = LIL_GET_ENVIRONMENT_REFLECTION(fd.V, normalDirectionB, fd.perceptualRoughness, fd.positionWS).b;

        float3 envReflectionColor = float3(envReflectionColorR, envReflectionColorG, envReflectionColorB);
        envReflectionColor = pow(saturate(envReflectionColor), _GemEnvContrast) * _GemEnvContrast * _GemEnvColor.rgb;
        envReflectionColor = lerp(dot(envReflectionColor, float3(1.0/3.0,1.0/3.0,1.0/3.0)), envReflectionColor, saturate(1.0/_GemEnvContrast));
        envReflectionColor = fd.facing < 0.0 ? envReflectionColor * baseCol.rgb : envReflectionColor;

        float oneMinusReflectivity = LIL_DIELECTRIC_SPECULAR.a;
        float grazingTerm = saturate(fd.smoothness + (1.0-oneMinusReflectivity));
        #ifdef LIL_COLORSPACE_GAMMA
            float surfaceReduction = 1.0 - 0.28 * fd.roughness * fd.perceptualRoughness;
        #else
            float surfaceReduction = 1.0 / (fd.roughness * fd.roughness + 1.0);
        #endif

        float particle = step(0.5, frac(nv1 * _GemParticleLoop)) * step(0.5, frac(nv2 * _GemParticleLoop)) * step(0.5, frac(nv3 * _GemParticleLoop));
        float3 particleColor = fd.facing < 0.0 ? 1.0 + particle * _GemParticleColor.rgb : 1.0;

        fd.col.rgb += (surfaceReduction * lilFresnelLerp(_Reflectance, grazingTerm, fd.nv) + 0.5) * 0.5 * particleColor * envReflectionColor;

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

        //------------------------------------------------------------------------------------------------------------------------------
        // Fix Color
        LIL_HDRP_DEEXPOSURE(fd.col);

        float4 fogColor = float4(0,0,0,0);
        BEFORE_FOG
        OVERRIDE_FOG

        BEFORE_OUTPUT
        OVERRIDE_OUTPUT
    }
#endif

#endif