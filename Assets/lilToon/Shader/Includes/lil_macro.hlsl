#ifndef LIL_MACRO_INCLUDED
#define LIL_MACRO_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Setting

// Dither shadow (Default : 1)
// 0 : Off
// 1 : On
#define LIL_SHADOW_DITHER 1

// Premultiply on ForwardAdd (Default : 1)
// 0 : Off
// 1 : On (for BlendOp Max)
#define LIL_PREMULTIPLY_FA 1

// Light direction mode (Default : 1)
// 0 : Directional light Only
// 1 : Blend SH light
#define LIL_LIGHT_DIRECTION_MODE 1

// Vertex light mode (Default : 3)
// 0 : Off (VRChat Mobile Toon Lit / MnMrShader / Reflex Shader / MToon / UTS2)
// 1 : Simple
// 2 : Accurate
// 3 : Approximate value of _LightTextureB0
// 4 : Lookup _LightTextureB0
#define LIL_VERTEXLIGHT_MODE 3

// ForwardAdd
// 0 : Off (VRChat Mobile Toon Lit / UnlitWF)
// 1 : On (All others)
// In UnlitWF, ForwardAdd passes are handled by the vertex shader instead.

// Refraction blur
#define LIL_REFRACTION_SAMPNUM 8
#define LIL_REFRACTION_GAUSDIST(i) exp(-(float)i*(float)i/(LIL_REFRACTION_SAMPNUM*LIL_REFRACTION_SAMPNUM/2.0))

// Vertex light mode (Default : 0)
// 0 : BRP Specular
// 1 : URP Specular
// 2 : Fast Specular
#define LIL_SPECULAR_MODE 0

// MatCap mode (Default : 1)
// 0 : Simple
// 1 : Fix Z-Rotation
#define LIL_MATCAP_MODE 1

// Antialias mode (Default : 1)
// 0 : Off
// 1 : On
#define LIL_ANTIALIAS_MODE 1

// Light Probe Proxy Volumes
#define LIL_LPPV_MODE 0
// 0 : Off
// 1 : On

// Transform Optimization
#define LIL_OPTIMIZE_TRANSFORM 0
// 0 : Off
// 1 : On

//------------------------------------------------------------------------------------------------------------------------------
// Replace Macro
#define LIL_BRANCH                                  UNITY_BRANCH
#define LIL_VERTEX_INPUT_INSTANCE_ID                UNITY_VERTEX_INPUT_INSTANCE_ID
#define LIL_VERTEX_OUTPUT_STEREO                    UNITY_VERTEX_OUTPUT_STEREO
#define LIL_SETUP_INSTANCE_ID(i)                    UNITY_SETUP_INSTANCE_ID(i)
#define LIL_TRANSFER_INSTANCE_ID(i,o)               UNITY_TRANSFER_INSTANCE_ID(i,o)
#define LIL_INITIALIZE_VERTEX_OUTPUT_STEREO(o)      UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o)
#define LIL_TRANSFER_VERTEX_OUTPUT_STEREO(i,o)      UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(i,o)
#define LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i)   UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i)

// Gamma
#if defined(UNITY_COLORSPACE_GAMMA)
    #define LIL_COLORSPACE_GAMMA
#endif

// Initialize struct
#if defined(UNITY_INITIALIZE_OUTPUT)
    #define LIL_INITIALIZE_STRUCT(type,name) UNITY_INITIALIZE_OUTPUT(type,name)
#else
    #define LIL_INITIALIZE_STRUCT(type,name) name = (type)0
#endif

// Vertex light
#if (defined(UNITY_SHOULD_SAMPLE_SH) || defined(_ADDITIONAL_LIGHTS_VERTEX)) && LIL_VERTEXLIGHT_MODE
    #define LIL_USE_VERTEXLIGHT
#endif

// Lightmap
#if defined(LIGHTMAP_ON)
    #define LIL_USE_LIGHTMAP
#endif
#if defined(DYNAMICLIGHTMAP_ON)
    #define LIL_USE_DYNAMICLIGHTMAP
#endif
#if defined(DIRLIGHTMAP_COMBINED)
    #define LIL_USE_DIRLIGHTMAP
#endif
#if defined(SHADOWS_SHADOWMASK)
    #define LIL_LIGHTMODE_SHADOWMASK
#endif
#if defined(LIGHTMAP_SHADOW_MIXING)
    #define LIL_LIGHTMODE_SUBTRACTIVE
#endif

// DOTS instancing
#if defined(UNITY_DOTS_INSTANCING_ENABLED)
    #define LIL_USE_DOTS_INSTANCING
#endif

// Conbine
#if (defined(SHADOWS_SCREEN) || defined(_MAIN_LIGHT_SHADOWS) || defined(_MAIN_LIGHT_SHADOWS_CASCADE) || defined(_MAIN_LIGHT_SHADOWS_SCREEN)) && defined(LIL_FEATURE_RECEIVE_SHADOW)
    #define LIL_USE_SHADOW
#endif
#if defined(LIL_USE_LIGHTMAP) || defined(LIL_USE_DYNAMICLIGHTMAP) || defined(LIL_USE_DIRLIGHTMAP) || defined(LIL_LIGHTMODE_SHADOWMASK)
    #define LIL_USE_LIGHTMAP_UV
#endif

// Directional Lightmap
#undef LIL_USE_DIRLIGHTMAP

// Light Probe Proxy Volumes
#if (LIL_LPPV_MODE != 0) && UNITY_LIGHT_PROBE_PROXY_VOLUME
    #define LIL_USE_LPPV
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Optimization Macro

// tangent / bitangent / normal
#if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND) || defined(LIL_FEATURE_TEX_MATCAP_NORMALMAP) || defined(LIL_FEATURE_EMISSION_1ST) || defined(LIL_FEATURE_EMISSION_2ND) || defined(LIL_FEATURE_PARALLAX)
    #define LIL_SHOULD_TBN
#endif

// tangentW
#if (defined(LIL_FEATURE_MAIN2ND) || defined(LIL_FEATURE_MAIN3RD)) && defined(LIL_FEATURE_DECAL)
    #define LIL_SHOULD_TANGENT_W
#endif

// tangent (vertex input)
#if defined(LIL_SHOULD_TBN) || defined(LIL_SHOULD_TANGENT_W)
    #define LIL_SHOULD_TANGENT
#endif

// normal (vertex input)
#if defined(LIL_SHOULD_TANGENT) || defined(LIL_FEATURE_SHADOW) || defined(LIL_FEATURE_REFLECTION) || defined(LIL_FEATURE_MATCAP) || defined(LIL_FEATURE_MATCAP_2ND) || defined(LIL_FEATURE_RIMLIGHT) || defined(LIL_FEATURE_GLITTER) || defined(LIL_FEATURE_AUDIOLINK) || defined(LIL_REFRACTION) || (defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SUBTRACTIVE))
    #define LIL_SHOULD_NORMAL
#endif

// positionWS
#if (defined(LIL_FEATURE_MAIN2ND) || defined(LIL_FEATURE_MAIN3RD)) && defined(LIL_FEATURE_LAYER_DISSOLVE) || defined(LIL_FEATURE_GLITTER) || defined(LIL_FEATURE_DISSOLVE)
    #define LIL_SHOULD_POSITION_OS
#endif

// positionWS
#if defined(LIL_PASS_FORWARDADD) || defined(LIL_FEATURE_MAIN2ND) || defined(LIL_FEATURE_MAIN3RD) || defined(LIL_FEATURE_REFLECTION) || defined(LIL_FEATURE_RIMLIGHT) || defined(LIL_FEATURE_GLITTER) || defined(LIL_FEATURE_EMISSION_1ST) || defined(LIL_FEATURE_EMISSION_2ND) || defined(LIL_FEATURE_PARALLAX) || defined(LIL_FEATURE_DISTANCE_FADE) || defined(LIL_REFRACTION) || !defined(LIL_BRP) || defined(LIL_USE_LPPV)
    #define LIL_SHOULD_POSITION_WS
#endif

// uv1
#if defined(LIL_FEATURE_GLITTER)
    #define LIL_SHOULD_UV1
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Macro

// Absorb pipeline differences
#if defined(LIL_LWRP)
    #define LIL_GET_VIEWDIR_WS(positionWS) GetCameraPositionWS() - positionWS
    #if defined(USING_STEREO_MATRICES)
        #define LIL_GET_HEADDIR_WS(positionWS)          ((unity_StereoWorldSpaceCameraPos[0] + unity_StereoWorldSpaceCameraPos[1]) * 0.5 - positionWS)
    #else
        #define LIL_GET_HEADDIR_WS(positionWS)          UnityWorldSpaceViewDir(positionWS)
    #endif
    float4 _ShadowBias;
    float3 ApplyShadowBias(float3 positionWS, float3 normalWS, float3 lightDirectionWS)
    {
        float invNdotL = 1.0 - saturate(dot(lightDirectionWS, normalWS));
        return normalWS * invNdotL * _ShadowBias.y + lightDirectionWS * _ShadowBias.xxx + positionWS;
    }
#elif defined(LIL_URP)
    #define LIL_GET_VIEWDIR_WS(positionWS) GetCameraPositionWS() - positionWS
    #if defined(USING_STEREO_MATRICES)
        #define LIL_GET_HEADDIR_WS(positionWS)          ((unity_StereoWorldSpaceCameraPos[0] + unity_StereoWorldSpaceCameraPos[1]) * 0.5 - positionWS)
    #else
        #define LIL_GET_HEADDIR_WS(positionWS)          UnityWorldSpaceViewDir(positionWS)
    #endif
#endif

#if defined(LIL_BRP)
    // 2017
    #ifndef EDITORVIZ_TEXTURE
        #undef EDITOR_VISUALIZATION
    #endif
    #ifndef UNITY_TRANSFER_LIGHTING
        #define UNITY_TRANSFER_LIGHTING(a,b) TRANSFER_SHADOW(a)
    #endif

    // Environment reflection
    UnityGIInput lilSetupGIInput(float3 positionWS)
    {
        UnityGIInput data;
        LIL_INITIALIZE_STRUCT(UnityGIInput, data);
        data.worldPos = positionWS;
        data.probeHDR[0] = unity_SpecCube0_HDR;
        data.probeHDR[1] = unity_SpecCube1_HDR;
        #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
            data.boxMin[0] = unity_SpecCube0_BoxMin;
        #endif
        #ifdef UNITY_SPECCUBE_BOX_PROJECTION
            data.boxMax[0] = unity_SpecCube0_BoxMax;
            data.probePosition[0] = unity_SpecCube0_ProbePosition;
            data.boxMax[1] = unity_SpecCube1_BoxMax;
            data.boxMin[1] = unity_SpecCube1_BoxMin;
            data.probePosition[1] = unity_SpecCube1_ProbePosition;
        #endif
        return data;
    }
    Unity_GlossyEnvironmentData lilSetupGlossyEnvironmentData(float3 viewDirection, float3 normalDirection, float perceptualRoughness)
    {
        Unity_GlossyEnvironmentData glossIn;
        glossIn.roughness = perceptualRoughness;
        glossIn.reflUVW   = reflect(-viewDirection,normalDirection);
        return glossIn;
    }
    #define LIL_GET_ENVIRONMENT_REFLECTION(viewDirection,normalDirection,perceptualRoughness,positionWS,o) \
        UnityGIInput data = lilSetupGIInput(positionWS); \
        Unity_GlossyEnvironmentData glossIn = lilSetupGlossyEnvironmentData(viewDirection,normalDirection,perceptualRoughness); \
        float3 o = UnityGI_IndirectSpecular(data, 1.0, glossIn)

    // Fog
    #define LIL_FOG_COORDS(idx)                     UNITY_FOG_COORDS(idx)
    #define LIL_TRANSFER_FOG(i,o)                   UNITY_TRANSFER_FOG(o,i.positionCS)
    #define LIL_APPLY_FOG(col,fogCoord)             UNITY_APPLY_FOG(fogCoord,col)
    #define LIL_APPLY_FOG_COLOR(col,fogCoord,fogColor) UNITY_APPLY_FOG_COLOR(fogCoord,col,fogColor)

    // Lightmap
    #define LIL_DECODE_LIGHTMAP(lm)                 DecodeLightmap(lm)
    #define LIL_DECODE_DYNAMICLIGHTMAP(lm)          DecodeRealtimeLightmap(lm)

    // Lighting
    #if defined(LIL_USE_SHADOW) && !defined(LIL_PASS_FORWARDADD)
        #define LIL_SHADOW_COORDS(idx)                  UNITY_SHADOW_COORDS(idx)
        #define LIL_TRANSFER_SHADOW(vi,uv,o) \
            BRPShadowCoords brpShadowCoords; \
            brpShadowCoords.pos = vi.positionCS; \
            UNITY_TRANSFER_LIGHTING(brpShadowCoords, uv) \
            o._ShadowCoord = brpShadowCoords._ShadowCoord
        #define LIL_LIGHT_ATTENUATION(atten,i) \
            BRPShadowCoords brpShadowCoords; \
            brpShadowCoords.pos = i.positionCS; \
            brpShadowCoords._ShadowCoord = i._ShadowCoord; \
            UNITY_LIGHT_ATTENUATION(atten, brpShadowCoords, i.positionWS)
    #elif !defined(LIL_PASS_FORWARDADD)
        #define LIL_SHADOW_COORDS(idx)
        #define LIL_TRANSFER_SHADOW(vi,uv,o)
        #define LIL_LIGHT_ATTENUATION(atten,i)      float atten = 1.0
    #else
        #define LIL_SHADOW_COORDS(idx)
        #define LIL_TRANSFER_SHADOW(vi,uv,o)
        #define LIL_LIGHT_ATTENUATION(atten,i) \
            BRPShadowCoords brpShadowCoords; \
            brpShadowCoords.pos = i.positionCS; \
            UNITY_LIGHT_ATTENUATION(atten, brpShadowCoords, i.positionWS)
    #endif
    struct BRPShadowCoords
    {
        float4 pos;
        LIL_SHADOW_COORDS(0)
    };

    // Shadow caster
    #define LIL_V2F_SHADOW_CASTER                   V2F_SHADOW_CASTER_NOPOS float4 positionCS : SV_POSITION;
    #if defined(SHADOWS_CUBE) && !defined(SHADOWS_CUBE_IN_DEPTH_TEX)
        #define LIL_TRANSFER_SHADOW_CASTER(v,o) \
            o.vec = mul(unity_ObjectToWorld, v.positionOS).xyz - _LightPositionRange.xyz; \
            o.positionCS = UnityObjectToClipPos(v.positionOS)
    #else
        #define LIL_TRANSFER_SHADOW_CASTER(v,o) \
            o.positionCS = UnityClipSpaceShadowCasterPos(v.positionOS, v.normalOS); \
            o.positionCS = UnityApplyLinearShadowBias(o.positionCS)
    #endif
    #define LIL_SHADOW_CASTER_FRAGMENT(i)           SHADOW_CASTER_FRAGMENT(i)

    // Transform
    #define LIL_TRANSFORM_POS_OS_TO_WS(positionOS)  mul(unity_ObjectToWorld, float4(positionOS.xyz,1.0))
    #define LIL_TRANSFORM_POS_WS_TO_CS(positionWS)  UnityWorldToClipPos(positionWS)
    #define LIL_GET_VIEWDIR_WS(positionWS)          UnityWorldSpaceViewDir(positionWS)
    #if defined(USING_STEREO_MATRICES)
        #define LIL_GET_HEADDIR_WS(positionWS)          ((unity_StereoWorldSpaceCameraPos[0] + unity_StereoWorldSpaceCameraPos[1]) * 0.5 - positionWS)
    #else
        #define LIL_GET_HEADDIR_WS(positionWS)          UnityWorldSpaceViewDir(positionWS)
    #endif

    // Support
    #define _MainLightColor                         _LightColor0
    #define _MainLightPosition                      _WorldSpaceLightPos0
    #define SRGBToLinear(col)                       GammaToLinearSpace(col)
    #define LinearToSRGB(col)                       LinearToGammaSpace(col)
    #define UnpackNormalScale(normal,scale)         UnpackScaleNormal(normal,scale)
    #define SampleSH(normal)                        ShadeSH9(float4(normal,1.0))
    #define MetaInput                               UnityMetaInput
    #define MetaFragment(input)                     UnityMetaFragment(input)
    #define MetaVertexPosition(pos,uv1,uv2,l,d)     UnityMetaVertexPosition(pos,uv1,uv2,l,d)
    #define LIL_MATRIX_M                            unity_ObjectToWorld
    #define LIL_MATRIX_I_M                          unity_WorldToObject
    #define LIL_MATRIX_V                            unity_MatrixV
    #define LIL_MATRIX_VP                           unity_MatrixVP
    #define LIL_NEGATIVE_SCALE                      unity_WorldTransformParams.w
#else
    // Environment reflection
    #define LIL_GET_ENVIRONMENT_REFLECTION(viewDirection,normalDirection,perceptualRoughness,positionWS,o) \
        float3 o = GlossyEnvironmentReflection(reflect(-viewDirection,normalDirection), perceptualRoughness, 1.0)

    // Fog
    #define LIL_FOG_COORDS(idx)                 float fogCoord : TEXCOORD##idx;
    #define LIL_TRANSFER_FOG(i,o)               o.fogCoord = ComputeFogFactor(i.positionCS.z)
    #define LIL_APPLY_FOG(col,fogCoord)         col.rgb = MixFog(col.rgb,fogCoord)
    #define LIL_APPLY_FOG_COLOR(col,fogCoord,fogColor) col.rgb = MixFogColor(col.rgb,fogColor.rgb,fogCoord)

    // Lightmap
    #define LIL_DECODE_LIGHTMAP(lm)             DecodeLightmap(lm, float4(LIGHTMAP_HDR_MULTIPLIER,LIGHTMAP_HDR_EXPONENT,0.0,0.0))
    #define LIL_DECODE_DYNAMICLIGHTMAP(lm)      DecodeLightmap(lm, float4(LIGHTMAP_HDR_MULTIPLIER,LIGHTMAP_HDR_EXPONENT,0.0,0.0))

    // Lighting
    #if defined(LIL_USE_SHADOW)
        #define LIL_SHADOW_COORDS(idx)              float4 shadowCoord : TEXCOORD##idx;
        #if defined(SHADOWS_SCREEN) || defined(_MAIN_LIGHT_SHADOWS_SCREEN)
            #define LIL_TRANSFER_SHADOW(vi,uv,o)        o.shadowCoord = ComputeScreenPos(vi.positionCS);
        #else
            #define LIL_TRANSFER_SHADOW(vi,uv,o)        o.shadowCoord = float4(vi.positionWS, 1.0);
        #endif
        #define LIL_LIGHT_ATTENUATION(atten,i) \
            float4 shadowCoord = TransformWorldToShadowCoord(i.shadowCoord); \
            float atten = MainLightRealtimeShadow(shadowCoord)
    #else
        #define LIL_SHADOW_COORDS(idx)
        #define LIL_TRANSFER_SHADOW(vi,uv,o)
        #define LIL_LIGHT_ATTENUATION(atten,i)      float atten = 1.0
    #endif

    // Shadow caster
    float3 _LightDirection;
    float3 _LightPosition;
    float4 URPShadowPos(float4 positionOS, float3 normalOS)
    {
        float3 positionWS = TransformObjectToWorld(positionOS.xyz);
        float3 normalWS = TransformObjectToWorldNormal(normalOS);

        #if _CASTING_PUNCTUAL_LIGHT_SHADOW
            float3 lightDirectionWS = normalize(_LightPosition - positionWS);
        #else
            float3 lightDirectionWS = _LightDirection;
        #endif

        float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

        #if UNITY_REVERSED_Z
            positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
        #else
            positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
        #endif

        return positionCS;
    }
    #define LIL_V2F_SHADOW_CASTER               float4 positionCS : SV_POSITION;
    #define LIL_TRANSFER_SHADOW_CASTER(v,o)     o.positionCS = URPShadowPos(v.positionOS, v.normalOS)
    #define LIL_SHADOW_CASTER_FRAGMENT(i)       return 0

    // Transform
    #define LIL_TRANSFORM_POS_OS_TO_WS(positionOS)                  TransformObjectToWorld(positionOS)
    #define LIL_TRANSFORM_POS_WS_TO_CS(positionWS)                  TransformWorldToHClip(positionWS)

    // Support
    #ifndef SHADER_STAGE_RAY_TRACING
        #define LIL_MATRIX_M                            GetObjectToWorldMatrix()
        #define LIL_MATRIX_I_M                          GetWorldToObjectMatrix()
        #define LIL_MATRIX_V                            GetWorldToViewMatrix()
        #define LIL_MATRIX_VP                           GetWorldToHClipMatrix()
    #else
        #define LIL_MATRIX_M                            ObjectToWorld3x4()
        #define LIL_MATRIX_I_M                          WorldToObject3x4()
        #define LIL_MATRIX_V                            GetWorldToViewMatrix()
        #define LIL_MATRIX_VP                           GetWorldToHClipMatrix()
    #endif
    #define LIL_NEGATIVE_SCALE                      GetOddNegativeScale()
#endif

// Pi
#define LIL_PI              3.14159265359f
#define LIL_TWO_PI          6.28318530718f
#define LIL_FOUR_PI         12.56637061436f
#define LIL_INV_PI          0.31830988618f
#define LIL_INV_TWO_PI      0.15915494309f
#define LIL_INV_FOUR_PI     0.07957747155f
#define LIL_HALF_PI         1.57079632679f
#define LIL_INV_HALF_PI     0.636619772367f

// Time
#define LIL_TIME            _Time.y
#define LIL_INTER_TIME      lilIntervalTime(_TimeInterval)

// Interpolation for Tessellation
#define LIL_TRI_INTERPOLATION(i,o,bary,type) o.type = bary.x * i[0].type + bary.y * i[1].type + bary.z * i[2].type

// Specular dielectric
#ifdef LIL_COLORSPACE_GAMMA
    #define LIL_DIELECTRIC_SPECULAR float4(0.220916301, 0.220916301, 0.220916301, 1.0 - 0.220916301)
#else
    #define LIL_DIELECTRIC_SPECULAR float4(0.04, 0.04, 0.04, 1.0 - 0.04)
#endif

// Do not apply shadow
#if (defined(LIL_PASS_LITE_INCLUDED) || defined(LIL_OUTLINE) || defined(LIL_FUR) || defined(LIL_PASS_FUR_INCLUDED)) && !defined(LIL_PASS_FORWARDADD)
    #undef LIL_TRANSFER_SHADOW
    #undef LIL_LIGHT_ATTENUATION
    #define LIL_TRANSFER_SHADOW(vi,uv,o)
    #define LIL_LIGHT_ATTENUATION(atten,i)      float atten = 1.0
#endif

// Texture
#if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && !defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER))
    #define LIL_SAMPLE_1D(tex,samp,uv)              tex.Sample(samp,uv)
    #define LIL_SAMPLE_1D_LOD(tex,samp,uv,lod)      tex.SampleLevel(sampler_linear_repeat,uv,lod)
    #define LIL_SAMPLE_2D(tex,samp,uv)              tex.Sample(samp,uv)
    #define LIL_SAMPLE_2D_ST(tex,samp,uv)           tex.Sample(samp,uv*tex##_ST.xy+tex##_ST.zw)
    #define LIL_SAMPLE_2D_LOD(tex,samp,uv,lod)      tex.SampleLevel(sampler_linear_repeat,uv,lod)
    #define LIL_SAMPLE_2D_BIAS(tex,samp,uv,bias)    tex.SampleBias(samp,uv,bias)
    #define LIL_SAMPLE_2D_GRAD(tex,samp,uv,dx,dy)   tex.SampleGrad(samp,uv,dx,dy)
    #define LIL_SAMPLE_2D_ARRAY(tex,samp,uv,index)  tex.Sample(samp,float3(uv,index))
    #define LIL_SAMPLE_3D(tex,samp,coord)           tex.Sample(samp,coord)
    #if !defined TEXTURE2D
        #define TEXTURE2D(tex)                      Texture2D tex
    #endif
    #if !defined TEXTURE3D
        #define TEXTURE3D(tex)                      Texture3D tex
    #endif
    #if !defined SAMPLER
        #define SAMPLER(tex)                        SamplerState tex
    #endif
#else
    #define LIL_SAMPLE_1D(tex,samp,uv)              tex1D(tex,uv)
    #define LIL_SAMPLE_1D_LOD(tex,samp,uv,lod)      tex1Dlod(tex,float4(uv,0,0,lod))
    #define LIL_SAMPLE_2D(tex,samp,uv)              tex2D(tex,uv)
    #define LIL_SAMPLE_2D_ST(tex,samp,uv)           tex2D(tex,uv*tex##_ST.xy+tex##_ST.zw)
    #define LIL_SAMPLE_2D_LOD(tex,samp,uv,lod)      tex2Dlod(tex,float4(uv,0,lod))
    #define LIL_SAMPLE_2D_BIAS(tex,samp,uv,bias)    tex2Dbias(tex,float4(uv,0,bias))
    #define LIL_SAMPLE_2D_GRAD(tex,samp,uv,dx,dy)   tex2Dgrad(tex,float4(uv,dx,dy))
    #define LIL_SAMPLE_2D_ARRAY(tex,samp,uv,index)  tex2DArray(tex,float3(uv,index))
    #define LIL_SAMPLE_3D(tex,samp,uv)              tex3D(tex,uv)
    #if !defined TEXTURE2D
        #define TEXTURE2D(tex)                      sampler2D tex
    #endif
    #if !defined TEXTURE3D
        #define TEXTURE3D(tex)                      sampler3D tex
    #endif
    #if !defined SAMPLER
        #define SAMPLER(tex)
    #endif
#endif

#if defined(LIL_FEATURE_PARALLAX) && defined(LIL_FEATURE_POM)
    #define LIL_SAMPLE_2D_POM(tex,samp,uv,dx,dy)    LIL_SAMPLE_2D_GRAD(tex,samp,uv,dx,dy)
#else
    #define LIL_SAMPLE_2D_POM(tex,samp,uv,dx,dy)    LIL_SAMPLE_2D(tex,samp,uv)
#endif

// Transform
#define LIL_VERTEX_POSITION_INPUTS(positionOS,o)                lilVertexPositionInputs o = lilGetVertexPositionInputs(positionOS)
#define LIL_VERTEX_NORMAL_INPUTS(normalOS,o)                    lilVertexNormalInputs o = lilGetVertexNormalInputs(normalOS)
#define LIL_VERTEX_NORMAL_TANGENT_INPUTS(normalOS,tangentOS,o)  lilVertexNormalInputs o = lilGetVertexNormalInputs(normalOS,tangentOS)

// Lightmap
#if defined(LIL_USE_DOTS_INSTANCING)
    #define LIL_SHADOWMAP_TEX                   unity_ShadowMasks
    #define LIL_SHADOWMAP_SAMP                  samplerunity_ShadowMasks
    #define LIL_LIGHTMAP_TEX                    unity_Lightmaps
    #define LIL_LIGHTMAP_SAMP                   samplerunity_Lightmaps
    #define LIL_DYNAMICLIGHTMAP_TEX             unity_DynamicLightmaps
    #define LIL_DYNAMICLIGHTMAP_SAMP            samplerunity_DynamicLightmaps
    #define LIL_DIRLIGHTMAP_TEX                 unity_LightmapsInd
    #define LIL_SAMPLE_LIGHTMAP(tex,samp,uv)    LIL_SAMPLE_2D_ARRAY(tex,samp,uv,unity_LightmapIndex.x)
#else
    #define LIL_SHADOWMAP_TEX                   unity_ShadowMask
    #define LIL_SHADOWMAP_SAMP                  samplerunity_ShadowMask
    #define LIL_LIGHTMAP_TEX                    unity_Lightmap
    #define LIL_LIGHTMAP_SAMP                   samplerunity_Lightmap
    #define LIL_DYNAMICLIGHTMAP_TEX             unity_DynamicLightmap
    #define LIL_DYNAMICLIGHTMAP_SAMP            samplerunity_DynamicLightmap
    #define LIL_DIRLIGHTMAP_TEX                 unity_LightmapInd
    #define LIL_SAMPLE_LIGHTMAP(tex,samp,uv)    LIL_SAMPLE_2D(tex,samp,uv)
#endif

// Lightmap uv
#if defined(LIL_USE_LIGHTMAP_UV)
    #define LIL_VERTEX_INPUT_LIGHTMAP_UV    float2 uv1 : TEXCOORD1;
    #define LIL_LIGHTMAP_COORDS(idx)        float2 uvLM : TEXCOORD##idx;
    #define LIL_TRANSFER_LIGHTMAPUV(uv1,o)  o.uvLM = uv1 * unity_LightmapST.xy + unity_LightmapST.zw
#else
    #define LIL_VERTEX_INPUT_LIGHTMAP_UV
    #define LIL_LIGHTMAP_COORDS(idx)
    #define LIL_TRANSFER_LIGHTMAPUV(uv1,o)
#endif

// Main Light Coords
#if defined(LIL_PASS_FORWARDADD)
    #define LIL_LIGHTCOLOR_COORDS(idx)
    #define LIL_LIGHTDIRECTION_COORDS(idx)
#else
    #define LIL_LIGHTCOLOR_COORDS(idx)      noperspective float3 lightColor : TEXCOORD##idx;
    #define LIL_LIGHTDIRECTION_COORDS(idx)  noperspective float3 lightDirection : TEXCOORD##idx;
#endif

#if !defined(LIL_PASS_FORWARDADD) && (defined(LIL_FEATURE_SHADOW) || defined(LIL_LITE))
    #define LIL_INDLIGHTCOLOR_COORDS(idx)   noperspective float3 indLightColor : TEXCOORD##idx;
#else
    #define LIL_INDLIGHTCOLOR_COORDS(idx)
#endif

// Dir light & indir light
#if defined(LIL_USE_LPPV) && (defined(LIL_FEATURE_SHADOW) || defined(LIL_LITE))
    #define LIL_CALC_TWOLIGHT(i,o) lilGetLightColorDouble(o.lightDirection, _ShadowEnvStrength, i.positionWS, o.lightColor, o.indLightColor)
#elif defined(LIL_FEATURE_SHADOW) || defined(LIL_LITE)
    #define LIL_CALC_TWOLIGHT(i,o) lilGetLightColorDouble(o.lightDirection, _ShadowEnvStrength, o.lightColor, o.indLightColor)
#elif defined(LIL_USE_LPPV)
    #define LIL_CALC_TWOLIGHT(i,o) o.lightColor = lilGetLightColor(i.positionWS)
#else
    #define LIL_CALC_TWOLIGHT(i,o) o.lightColor = lilGetLightColor()
#endif

// Main Light in VS (Color / Direction)
#if defined(LIL_PASS_FORWARDADD)
    #define LIL_CALC_MAINLIGHT(i,o)
#elif defined(LIL_USE_LIGHTMAP)
    #define LIL_CALC_MAINLIGHT(i,o) \
        o.lightDirection = lilGetLightDirection(); \
        LIL_CALC_TWOLIGHT(i,o)
#else
    #define LIL_CALC_MAINLIGHT(i,o) \
        o.lightDirection = lilGetLightDirection(); \
        LIL_CALC_TWOLIGHT(i,o); \
        o.lightColor = max(o.lightColor, _LightMinLimit); \
        o.lightColor = lerp(o.lightColor, 1.0, _AsUnlit)
#endif

// Main Light in PS (Color / Direction / Attenuation)
#if defined(LIL_PASS_FORWARDADD)
    // Point Light & Spot Light (ForwardAdd)
    #define LIL_GET_MAINLIGHT(input,lightColor,lightDirection,atten) \
        LIL_LIGHT_ATTENUATION(atten, input); \
        float3 lightColor = saturate(_MainLightColor.rgb * atten); \
        float3 lightDirection = lilGetLightDirection(input.positionWS)
#elif defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SHADOWMASK)
    // Mixed Lightmap (Shadowmask)
    #define LIL_GET_MAINLIGHT(input,lightColor,lightDirection,atten) \
        LIL_LIGHT_ATTENUATION(atten, input); \
        float3 lightColor = input.lightColor; \
        float3 lightDirection = input.lightDirection; \
        float3 lightmapColor = lilGetLightMapColor(input.uvLM); \
        lightColor = max(lightColor, lightmapColor); \
        atten = min(atten, LIL_SAMPLE_LIGHTMAP(LIL_SHADOWMAP_TEX,LIL_LIGHTMAP_SAMP,input.uvLM).r)
#elif defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SUBTRACTIVE) && defined(LIL_USE_DYNAMICLIGHTMAP)
    // Mixed Lightmap (Subtractive)
    // Use Lightmap as Shadowmask
    #undef LIL_USE_DYNAMICLIGHTMAP
    #define LIL_GET_MAINLIGHT(input,lightColor,lightDirection,atten) \
        LIL_LIGHT_ATTENUATION(atten, input); \
        float3 lightColor = input.lightColor; \
        float3 lightDirection = input.lightDirection; \
        float3 lightmapColor = lilGetLightMapColor(input.uvLM); \
        lightColor = max(lightColor, lightmapColor); \
        float3 lightmapShadowThreshold = _MainLightColor.rgb*0.5; \
        float3 lightmapS = (lightmapColor - lightmapShadowThreshold) / (_MainLightColor.rgb - lightmapShadowThreshold); \
        float lightmapAttenuation = saturate((lightmapS.r+lightmapS.g+lightmapS.b)/3.0); \
        atten = min(atten, lightmapAttenuation)
#elif defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SUBTRACTIVE)
    // Mixed Lightmap (Subtractive)
    // Use Lightmap as Shadowmask
    #define LIL_GET_MAINLIGHT(input,lightColor,lightDirection,atten) \
        LIL_LIGHT_ATTENUATION(atten, input); \
        float3 lightColor = input.lightColor; \
        float3 lightDirection = input.lightDirection; \
        float3 lightmapColor = lilGetLightMapColor(input.uvLM); \
        lightColor = max(lightColor, lightmapColor); \
        float3 lightmapS = (lightmapColor - SampleSH(input.normalWS)) / _MainLightColor.rgb; \
        float lightmapAttenuation = saturate((lightmapS.r+lightmapS.g+lightmapS.b)/3.0); \
        atten = min(atten, lightmapAttenuation)
#elif defined(LIL_USE_LIGHTMAP) && defined(LIL_USE_DIRLIGHTMAP)
    // Lightmap (Directional)
    #define LIL_GET_MAINLIGHT(input,lightColor,lightDirection,atten) \
        LIL_LIGHT_ATTENUATION(atten, input); \
        float3 lightColor = input.lightColor; \
        float3 lightDirection = input.lightDirection; \
        float3 lightmapColor = lilGetLightMapColor(input.uvLM); \
        float3 lightmapDirection = lilGetLightMapDirection(input.uvLM); \
        lightColor = saturate(lightColor + lightmapColor); \
        lightDirection = normalize(lightDirection + lightmapDirection * lilLuminance(lightmapColor))
#elif defined(LIL_USE_LIGHTMAP) && defined(LIL_USE_SHADOW)
    // Mixed Lightmap (Baked Indirect) with shadow
    #define LIL_GET_MAINLIGHT(input,lightColor,lightDirection,atten) \
        LIL_LIGHT_ATTENUATION(atten, input); \
        float3 lightColor = _MainLightColor.rgb; \
        float3 lightDirection = input.lightDirection; \
        float3 lightmapColor = lilGetLightMapColor(input.uvLM); \
        lightColor = saturate(lightColor + max(lightmapColor,lilGetSHToon()))
#elif defined(LIL_USE_LIGHTMAP) && defined(LIL_USE_DYNAMICLIGHTMAP)
    // Mixed Lightmap (Baked Indirect) or Lightmap (Non-Directional)
    #undef LIL_USE_DYNAMICLIGHTMAP
    #define LIL_GET_MAINLIGHT(input,lightColor,lightDirection,atten) \
        LIL_LIGHT_ATTENUATION(atten, input); \
        float3 lightColor = input.lightColor; \
        float3 lightDirection = input.lightDirection; \
        float3 lightmapColor = lilGetLightMapColor(input.uvLM); \
        lightColor = saturate(lightColor + lightmapColor)
#elif defined(LIL_USE_LIGHTMAP)
    // Mixed Lightmap (Baked Indirect) or Lightmap (Non-Directional)
    #define LIL_GET_MAINLIGHT(input,lightColor,lightDirection,atten) \
        LIL_LIGHT_ATTENUATION(atten, input); \
        float3 lightColor = _MainLightColor.rgb; \
        float3 lightDirection = input.lightDirection; \
        float3 lightmapColor = lilGetLightMapColor(input.uvLM); \
        lightColor = saturate(lightColor + lightmapColor)
#else
    // Realtime
    #define LIL_GET_MAINLIGHT(input,lightColor,lightDirection,atten) \
        LIL_LIGHT_ATTENUATION(atten, input); \
        float3 lightColor = input.lightColor; \
        float3 lightDirection = input.lightDirection;
#endif

// Vertex Light
#if defined(LIL_USE_VERTEXLIGHT) && (defined(_ADDITIONAL_LIGHTS_VERTEX) || defined(VERTEXLIGHT_ON) || defined(LIL_TESSELLATION))
    #define LIL_VERTEXLIGHT_COORDS(idx) float3 vl : TEXCOORD##idx;
    #define LIL_CALC_VERTEXLIGHT(i,o) o.vl = lilGetVertexLights(i.positionWS,_VertexLightStrength)
    #define LIL_GET_VERTEXLIGHT(i,o) float3 o = i.vl
#elif defined(LIL_USE_VERTEXLIGHT)
    #define LIL_VERTEXLIGHT_COORDS(idx) float3 vl : TEXCOORD##idx;
    #define LIL_CALC_VERTEXLIGHT(i,o)
    #define LIL_GET_VERTEXLIGHT(i,o) float3 o = i.vl
#else
    #define LIL_VERTEXLIGHT_COORDS(idx)
    #define LIL_CALC_VERTEXLIGHT(i,o)
    #define LIL_GET_VERTEXLIGHT(i,o) float3 o = 0
#endif

// Additional Light
#if defined(_ADDITIONAL_LIGHTS)
    #define LIL_GET_ADDITIONALLIGHT(positionWS,o) float3 o = lilGetAdditionalLights(positionWS)
#else
    #define LIL_GET_ADDITIONALLIGHT(positionWS,o) float3 o = 0
#endif

// Main Color & Emission
#if defined(LIL_WITHOUT_ANIMATION)
    #define LIL_GET_SUBTEX(tex,uv)  lilGetSubTexWithoutAnimation(Exists##tex, tex, tex##_ST, tex##Angle, uv, 1, sampler##tex, tex##IsDecal, tex##IsLeftOnly, tex##IsRightOnly, tex##ShouldCopy, tex##ShouldFlipMirror, tex##ShouldFlipCopy, tex##IsMSDF, isRightHand)
    #define LIL_GET_EMITEX(tex,uv)  LIL_SAMPLE_2D(tex, sampler##tex, lilCalcUVWithoutAnimation(uv, tex##_ST, tex##_ScrollRotate))
    #define LIL_GET_EMIMASK(tex,uv) LIL_SAMPLE_2D(tex, sampler_MainTex, lilCalcUVWithoutAnimation(uv, tex##_ST, tex##_ScrollRotate))
#else
    #define LIL_GET_SUBTEX(tex,uv)  lilGetSubTex(Exists##tex, tex, tex##_ST, tex##Angle, uv, nv, sampler##tex, tex##IsDecal, tex##IsLeftOnly, tex##IsRightOnly, tex##ShouldCopy, tex##ShouldFlipMirror, tex##ShouldFlipCopy, tex##IsMSDF, isRightHand, tex##DecalAnimation, tex##DecalSubParam)
    #define LIL_GET_EMITEX(tex,uv)  LIL_SAMPLE_2D(tex, sampler##tex, lilCalcUV(uv, tex##_ST, tex##_ScrollRotate))
    #define LIL_GET_EMIMASK(tex,uv) LIL_SAMPLE_2D(tex, sampler_MainTex, lilCalcUV(uv, tex##_ST, tex##_ScrollRotate))
#endif

// Meta
#define LIL_TRANSFER_METAPASS(input,output) \
    output.positionCS = MetaVertexPosition(input.positionOS, input.uv1, input.uv2, unity_LightmapST, unity_DynamicLightmapST)

#endif