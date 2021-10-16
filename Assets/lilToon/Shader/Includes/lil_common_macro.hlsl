#ifndef LIL_MACRO_INCLUDED
#define LIL_MACRO_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Setting

// The version of SRP is automatically determined, but an error may occur in a specific version.
// In that case, define the version.
// Example: HDRP 4.8.0
// #define SHADER_LIBRARY_VERSION_MAJOR 4
// #define SHADER_LIBRARY_VERSION_MINOR 8

// Transparent mode on subpass (Default : 0)
// 0 : Cutout
// 1 : Dither
#define LIL_SUBPASS_TRANSPARENT_MODE 0

// Premultiply on ForwardAdd (Default : 1)
// 0 : Off
// 1 : On (for BlendOp Max)
#define LIL_PREMULTIPLY_FA 1

// Light direction mode (Default : 1)
// 0 : Directional light Only
// 1 : Blend SH light
#define LIL_LIGHT_DIRECTION_MODE 1

// Refraction blur
#define LIL_REFRACTION_SAMPNUM 8
#define LIL_REFRACTION_GAUSDIST(i) exp(-(float)i*(float)i/(LIL_REFRACTION_SAMPNUM*LIL_REFRACTION_SAMPNUM/2.0))

// Specular mode (Default : 0)
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

// Additional Lights Mode
#define LIL_ADDITIONAL_LIGHT_MODE 1
// 0 : Off
// 1 : In Vertex Shader
// 2 : In Fragment Shader

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

// Additional Light
#if (!defined(LIL_PASS_FORWARDADD) && defined(UNITY_SHOULD_SAMPLE_SH)) || defined(_ADDITIONAL_LIGHTS) || defined(_ADDITIONAL_LIGHTS_VERTEX) || defined(LIL_HDRP)
    #define LIL_USE_ADDITIONALLIGHT
#endif
#if defined(LIL_USE_ADDITIONALLIGHT) && (LIL_ADDITIONAL_LIGHT_MODE == 1)
    #define LIL_USE_ADDITIONALLIGHT_VS
#elif defined(LIL_USE_ADDITIONALLIGHT) && (LIL_ADDITIONAL_LIGHT_MODE == 2)
    #define LIL_USE_ADDITIONALLIGHT_PS
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
#if defined(SHADOWS_SCREEN) || defined(_MAIN_LIGHT_SHADOWS) || defined(_MAIN_LIGHT_SHADOWS_CASCADE) || defined(_MAIN_LIGHT_SHADOWS_SCREEN) || defined(SHADOW_LOW) || defined(SHADOW_MEDIUM) || defined(SHADOW_HIGH)
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

// tangentWS / bitangentWS / normalWS
#if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND) || defined(LIL_FEATURE_TEX_MATCAP_NORMALMAP) || defined(LIL_FEATURE_EMISSION_1ST) || defined(LIL_FEATURE_EMISSION_2ND) || defined(LIL_FEATURE_PARALLAX)
    #define LIL_SHOULD_TBN
#endif

// tangentOS (vertex input)
#if defined(LIL_SHOULD_TBN) || (defined(LIL_FEATURE_MAIN2ND) || defined(LIL_FEATURE_MAIN3RD)) && defined(LIL_FEATURE_DECAL)
    #define LIL_SHOULD_TANGENT
#endif

// normalOS (vertex input)
#if defined(LIL_SHOULD_TANGENT) || defined(LIL_FEATURE_SHADOW) || defined(LIL_FEATURE_REFLECTION) || defined(LIL_FEATURE_MATCAP) || defined(LIL_FEATURE_MATCAP_2ND) || defined(LIL_FEATURE_RIMLIGHT) || defined(LIL_FEATURE_GLITTER) || defined(LIL_FEATURE_BACKLIGHT) || defined(LIL_FEATURE_AUDIOLINK) || defined(LIL_REFRACTION) || (defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SUBTRACTIVE)) || defined(LIL_HDRP)
    #define LIL_SHOULD_NORMAL
#endif

// positionOS
#if (defined(LIL_FEATURE_MAIN2ND) || defined(LIL_FEATURE_MAIN3RD)) && defined(LIL_FEATURE_LAYER_DISSOLVE) || defined(LIL_FEATURE_GLITTER) || defined(LIL_FEATURE_DISSOLVE)
    #define LIL_SHOULD_POSITION_OS
#endif

// positionWS
#if defined(LIL_PASS_FORWARDADD) || defined(LIL_FEATURE_MAIN2ND) || defined(LIL_FEATURE_MAIN3RD) || defined(LIL_FEATURE_RECEIVE_SHADOW) || defined(LIL_FEATURE_REFLECTION) || defined(LIL_FEATURE_MATCAP) || defined(LIL_FEATURE_MATCAP_2ND) || defined(LIL_FEATURE_RIMLIGHT) || defined(LIL_FEATURE_GLITTER) || defined(LIL_FEATURE_BACKLIGHT) || defined(LIL_FEATURE_EMISSION_1ST) || defined(LIL_FEATURE_EMISSION_2ND) || defined(LIL_FEATURE_PARALLAX) || defined(LIL_FEATURE_DISTANCE_FADE) || defined(LIL_REFRACTION) || !defined(LIL_BRP) || defined(LIL_USE_LPPV)
    #define LIL_SHOULD_POSITION_WS
#endif

// uv1
#if defined(LIL_FEATURE_GLITTER)
    #define LIL_SHOULD_UV1
#endif

//------------------------------------------------------------------------------------------------------------------------------
// API Macro
#if defined(TEXTURE2D)
    #undef TEXTURE2D
#endif
#if defined(TEXTURE2D_FLOAT)
    #undef TEXTURE2D_FLOAT
#endif
#if defined(TEXTURE3D)
    #undef TEXTURE3D
#endif
#if defined(SAMPLER)
    #undef SAMPLER
#endif

#if defined(SHADER_API_VULKAN) && UNITY_VERSION < 201800 && defined(LIL_TESSELLATION)
    #if defined(POSITION)
        #undef POSITION
    #endif
    #define POSITION gl_Position
#endif

#if defined(SHADER_API_D3D11_9X) || (UNITY_VERSION < 201800 && defined(SHADER_API_GLES))
    #define LIL_NOPERSPECTIVE
#else
    #define LIL_NOPERSPECTIVE noperspective
#endif

#if defined(SHADER_API_D3D9)
    #undef LIL_ANTIALIAS_MODE
    #define LIL_ANTIALIAS_MODE 0
    #undef LIL_BRANCH
    #define LIL_BRANCH
#endif

#if defined(SHADER_API_D3D11_9X)
    #define LIL_VFACE(facing)
    #define LIL_VFACE_FALLBACK(facing) float facing = 1.0
    #undef LIL_USE_LIGHTMAP
    #undef LIL_BRANCH
    #define LIL_BRANCH
#else
    #define LIL_VFACE(facing) , float facing : VFACE
    #define LIL_VFACE_FALLBACK(facing)
#endif

#if defined(SHADER_API_D3D9) || (UNITY_VERSION < 201800 && defined(SHADER_API_GLES)) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER)) || defined(SHADER_TARGET_SURFACE_ANALYSIS)
    #define LIL_SAMPLE_1D(tex,samp,uv)              tex2D(tex,float2(uv,0.5))
    #define LIL_SAMPLE_1D_LOD(tex,samp,uv,lod)      tex2Dlod(tex,float4(uv,0.5,0,lod))
    #define LIL_SAMPLE_2D(tex,samp,uv)              tex2D(tex,uv)
    #define LIL_SAMPLE_2D_ST(tex,samp,uv)           tex2D(tex,uv*tex##_ST.xy+tex##_ST.zw)
    #define LIL_SAMPLE_2D_LOD(tex,samp,uv,lod)      tex2Dlod(tex,float4(uv,0,lod))
    #define LIL_SAMPLE_2D_BIAS(tex,samp,uv,bias)    tex2Dbias(tex,float4(uv,0,bias))
    #define LIL_SAMPLE_2D_GRAD(tex,samp,uv,dx,dy)   tex2Dgrad(tex,float4(uv,dx,dy))
    #define LIL_SAMPLE_2D_ARRAY(tex,samp,uv,index)  tex2DArray(tex,float3(uv,index))
    #define LIL_SAMPLE_3D(tex,samp,uv)              tex3D(tex,uv)
    #define TEXTURE2D(tex)                          sampler2D tex
    #define TEXTURE2D_FLOAT(tex)                    sampler2D tex
    #define TEXTURE3D(tex)                          sampler3D tex
    #define SAMPLER(samp)
    #define LIL_SAMP_IN_FUNC(samp)
    #define LIL_SAMP_IN(samp)
#else
    #define LIL_SAMPLE_1D(tex,samp,uv)              tex.Sample(samp,uv)
    #define LIL_SAMPLE_1D_LOD(tex,samp,uv,lod)      tex.SampleLevel(samp,uv,lod)
    #define LIL_SAMPLE_2D(tex,samp,uv)              tex.Sample(samp,uv)
    #define LIL_SAMPLE_2D_ST(tex,samp,uv)           tex.Sample(samp,uv*tex##_ST.xy+tex##_ST.zw)
    #define LIL_SAMPLE_2D_LOD(tex,samp,uv,lod)      tex.SampleLevel(samp,uv,lod)
    #define LIL_SAMPLE_2D_BIAS(tex,samp,uv,bias)    tex.SampleBias(samp,uv,bias)
    #define LIL_SAMPLE_2D_GRAD(tex,samp,uv,dx,dy)   tex.SampleGrad(samp,uv,dx,dy)
    #define LIL_SAMPLE_2D_ARRAY(tex,samp,uv,index)  tex.Sample(samp,float3(uv,index))
    #define LIL_SAMPLE_3D(tex,samp,coord)           tex.Sample(samp,coord)
    #define TEXTURE2D(tex)                          Texture2D tex
    #define TEXTURE2D_FLOAT(tex)                    Texture2D<float4> tex
    #define TEXTURE3D(tex)                          Texture3D tex
    #define SAMPLER(samp)                           SamplerState samp
    #define LIL_SAMP_IN_FUNC(samp)                  , SamplerState samp
    #define LIL_SAMP_IN(samp)                       , samp
#endif

#if defined(LIL_FEATURE_PARALLAX) && defined(LIL_FEATURE_POM)
    #define LIL_SAMPLE_2D_POM(tex,samp,uv,dx,dy)    LIL_SAMPLE_2D_GRAD(tex,samp,uv,dx,dy)
#else
    #define LIL_SAMPLE_2D_POM(tex,samp,uv,dx,dy)    LIL_SAMPLE_2D(tex,samp,uv)
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Macro to absorb pipeline differences
// Version
#if !defined(SHADER_LIBRARY_VERSION_MAJOR)
    #if UNITY_VERSION < 201820
        #define SHADER_LIBRARY_VERSION_MAJOR 1
    #elif UNITY_VERSION < 201830
        #define SHADER_LIBRARY_VERSION_MAJOR 2
    #elif UNITY_VERSION < 201840
        #define SHADER_LIBRARY_VERSION_MAJOR 3
    #elif UNITY_VERSION < 201910
        #define SHADER_LIBRARY_VERSION_MAJOR 4
    #elif UNITY_VERSION < 201920
        #define SHADER_LIBRARY_VERSION_MAJOR 5
    #elif UNITY_VERSION < 201930
        #define SHADER_LIBRARY_VERSION_MAJOR 6
    #else
        #define SHADER_LIBRARY_VERSION_MAJOR 0
    #endif
#endif
#if !defined(SHADER_LIBRARY_VERSION_MINOR)
    #define SHADER_LIBRARY_VERSION_MINOR 99
#endif
#if !defined(VERSION_GREATER_EQUAL)
    #define VERSION_GREATER_EQUAL(major, minor) ((SHADER_LIBRARY_VERSION_MAJOR > major) || ((SHADER_LIBRARY_VERSION_MAJOR == major) && (SHADER_LIBRARY_VERSION_MINOR >= minor)))
    #define VERSION_LOWER(major, minor) ((SHADER_LIBRARY_VERSION_MAJOR < major) || ((SHADER_LIBRARY_VERSION_MAJOR == major) && (SHADER_LIBRARY_VERSION_MINOR < minor)))
    #define VERSION_EQUAL(major, minor) ((SHADER_LIBRARY_VERSION_MAJOR == major) && (SHADER_LIBRARY_VERSION_MINOR == minor))
#endif

// Transform
#if defined(LIL_BRP)
    #define LIL_MATRIX_M        unity_ObjectToWorld
    #define LIL_MATRIX_I_M      unity_WorldToObject
    #define LIL_MATRIX_V        UNITY_MATRIX_V
    #define LIL_MATRIX_VP       UNITY_MATRIX_VP
    #define LIL_MATRIX_P        UNITY_MATRIX_P
    #define LIL_NEGATIVE_SCALE  unity_WorldTransformParams.w

    float3 lilTransformOStoWS(float4 positionOS)
    {
        return mul(LIL_MATRIX_M, positionOS).xyz;
    }

    float3 lilTransformOStoWS(float3 positionOS)
    {
        return mul(LIL_MATRIX_M, float4(positionOS,1.0)).xyz;
    }

    float3 lilTransformWStoOS(float3 positionWS)
    {
        return mul(LIL_MATRIX_I_M, float4(positionWS, 1.0)).xyz;
    }

    float3 lilTransformWStoVS(float3 positionWS)
    {
        return UnityWorldToViewPos(positionWS).xyz;
    }

    float4 lilTransformWStoCS(float3 positionWS)
    {
        return UnityWorldToClipPos(positionWS);
    }

    float4 lilTransformVStoCS(float3 positionVS)
    {
        return UnityViewToClipPos(positionVS);
    }

    float4 lilTransformCStoSS(float4 positionCS)
    {
        return ComputeGrabScreenPos(positionCS);
    }
#else
    #if defined(SHADER_STAGE_RAY_TRACING)
        #define LIL_MATRIX_M        ObjectToWorld3x4()
        #define LIL_MATRIX_I_M      WorldToObject3x4()
    #else
        #define LIL_MATRIX_M        GetObjectToWorldMatrix()
        #define LIL_MATRIX_I_M      GetWorldToObjectMatrix()
    #endif
    #define LIL_MATRIX_V        GetWorldToViewMatrix()
    #define LIL_MATRIX_VP       GetWorldToHClipMatrix()
    #define LIL_MATRIX_P        GetViewToHClipMatrix()
    #define LIL_NEGATIVE_SCALE  GetOddNegativeScale()

    float3 lilTransformOStoWS(float4 positionOS)
    {
        return TransformObjectToWorld(positionOS.xyz).xyz;
    }

    float3 lilTransformOStoWS(float3 positionOS)
    {
        return mul(LIL_MATRIX_M, float4(positionOS,1.0)).xyz;
    }

    float3 lilTransformWStoOS(float3 positionWS)
    {
        return TransformWorldToObject(positionWS).xyz;
    }

    float3 lilTransformWStoVS(float3 positionWS)
    {
        return TransformWorldToView(positionWS).xyz;
    }

    float4 lilTransformWStoCS(float3 positionWS)
    {
        return TransformWorldToHClip(positionWS);
    }

    float4 lilTransformVStoCS(float3 positionVS)
    {
        return TransformWViewToHClip(positionVS);
    }

    float4 lilTransformCStoSS(float4 positionCS)
    {
        float4 positionSS = positionCS * 0.5f;
        positionSS.xy = float2(positionSS.x, positionSS.y * _ProjectionParams.x) + positionSS.w;
        positionSS.zw = positionCS.zw;
        return positionSS;
    }
#endif

float3 lilToAbsolutePositionWS(float3 positionRWS)
{
    #if defined(LIL_HDRP)
        return GetAbsolutePositionWS(positionRWS);
    #else
        return positionRWS;
    #endif
}

float3 lilTransformDirOStoWS(float3 directionOS, bool doNormalize)
{
    if(doNormalize) return normalize(mul((float3x3)LIL_MATRIX_M, directionOS));
    else            return mul((float3x3)LIL_MATRIX_M, directionOS);
}

float3 lilTransformDirWStoOS(float3 directionWS, bool doNormalize)
{
    if(doNormalize) return normalize(mul((float3x3)LIL_MATRIX_I_M, directionWS));
    else            return mul((float3x3)LIL_MATRIX_I_M, directionWS);
}

float3 lilTransformNormalOStoWS(float3 normalOS, bool doNormalize)
{
    #ifdef UNITY_ASSUME_UNIFORM_SCALING
        return lilTransformDirOStoWS(normalOS, doNormalize);
    #else
        if(doNormalize) return normalize(mul(normalOS, (float3x3)LIL_MATRIX_I_M));
        else            return mul(normalOS, (float3x3)LIL_MATRIX_I_M);
    #endif
}

float3 lilViewDirection(float3 positionWS)
{
    return _WorldSpaceCameraPos.xyz - positionWS;
}

float3 lilHeadDirection(float3 positionWS)
{
    #if defined(USING_STEREO_MATRICES)
        return (unity_StereoWorldSpaceCameraPos[0] + unity_StereoWorldSpaceCameraPos[1]) * 0.5 - positionWS;
    #else
        return lilViewDirection(positionWS);
    #endif
}

/*
// Built-in RP
#define UnityWorldToViewPos(positionWS)                     lilTransformWStoVS(positionWS)
#define UnityWorldToClipPos(positionWS)                     lilTransformWStoCS(positionWS)
#define UnityViewToClipPos(positionVS)                      lilTransformVStoCS(positionVS)
#define ComputeGrabScreenPos(positionCS)                    lilTransformCStoSS(positionCS)
#define UnityWorldSpaceViewDir(positionWS)                  lilViewDirection(positionWS)
#define UnityObjectToWorldDir(directionOS)                  lilTransformDirOStoWS(directionOS)
#define UnityWorldToObjectDir(directionWS)                  lilTransformDirWStoOS(directionWS)
#define UnityObjectToWorldNormal(normalOS)                  lilTransformNormalOStoWS(normalOS)
#define UnityWorldSpaceViewDir(positionWS)                  lilViewDirection(positionWS)

// SRP
#define TransformObjectToWorld(positionOS)                  lilTransformOStoWS(positionOS)
#define TransformWorldToObject(positionWS)                  lilTransformWStoOS(positionWS)
#define TransformWorldToView(positionWS)                    lilTransformWStoVS(positionWS)
#define TransformWorldToHClip(positionWS)                   lilTransformWStoCS(positionWS)
#define TransformWViewToHClip(positionVS)                   lilTransformVStoCS(positionVS)
#define TransformObjectToWorldDir(directionOS,doNormalize)  lilTransformDirOStoWS(directionOS,doNormalize)
#define TransformWorldToObjectDir(directionWS,doNormalize)  lilTransformDirWStoOS(directionWS,doNormalize)
#define TransformObjectToWorldNormal(normalOS,doNormalize)  lilTransformNormalOStoWS(normalOS,doNormalize)
#define GetAbsolutePositionWS(positionRWS)                  lilToAbsolutePositionWS(float3 positionRWS)
*/

// Lighting
#if defined(LIL_BRP)
    // 2017
    #ifndef EDITORVIZ_TEXTURE
        #undef EDITOR_VISUALIZATION
    #endif
    #ifndef UNITY_TRANSFER_LIGHTING
        #define UNITY_TRANSFER_LIGHTING(a,b) TRANSFER_SHADOW(a)
    #endif

    // HDRP Data
    uint lilGetRenderingLayer()
    {
        return 0;
    }
    #define LIL_GET_HDRPDATA(input)

    // Main light
    #define LIL_MAINLIGHT_COLOR                         _LightColor0.rgb
    #define LIL_MAINLIGHT_DIRECTION                     _WorldSpaceLightPos0.xyz

    // Shadow
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
            UNITY_LIGHT_ATTENUATION(attenuationOrig, brpShadowCoords, i.positionWS); \
            atten = attenuationOrig
    #elif !defined(LIL_PASS_FORWARDADD)
        #define LIL_SHADOW_COORDS(idx)
        #define LIL_TRANSFER_SHADOW(vi,uv,o)
        #define LIL_LIGHT_ATTENUATION(atten,i)
    #else
        #define LIL_SHADOW_COORDS(idx)
        #define LIL_TRANSFER_SHADOW(vi,uv,o)
        #define LIL_LIGHT_ATTENUATION(atten,i) \
            BRPShadowCoords brpShadowCoords; \
            brpShadowCoords.pos = i.positionCS; \
            UNITY_LIGHT_ATTENUATION(attenuationOrig, brpShadowCoords, i.positionWS); \
            atten = attenuationOrig
    #endif
    struct BRPShadowCoords
    {
        float4 pos;
        LIL_SHADOW_COORDS(0)
    };

    // Shadow caster
    #define LIL_V2F_SHADOW_CASTER_OUTPUT            V2F_SHADOW_CASTER_NOPOS float4 positionCS : SV_POSITION;
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

    // Additional Light
    float3 lilGetAdditionalLights(float3 positionWS)
    {
        float4 toLightX = unity_4LightPosX0 - positionWS.x;
        float4 toLightY = unity_4LightPosY0 - positionWS.y;
        float4 toLightZ = unity_4LightPosZ0 - positionWS.z;

        float4 lengthSq = toLightX * toLightX + 0.000001;
        lengthSq += toLightY * toLightY;
        lengthSq += toLightZ * toLightZ;

        //float4 atten = 1.0 / (1.0 + lengthSq * unity_4LightAtten0);
        float4 atten = saturate(saturate((25.0 - lengthSq * unity_4LightAtten0) * 0.111375) / (0.987725 + lengthSq * unity_4LightAtten0));

        float3 additionalLightColor;
        additionalLightColor =                                 unity_LightColor[0].rgb * atten.x;
        additionalLightColor =          additionalLightColor + unity_LightColor[1].rgb * atten.y;
        additionalLightColor =          additionalLightColor + unity_LightColor[2].rgb * atten.z;
        additionalLightColor = saturate(additionalLightColor + unity_LightColor[3].rgb * atten.w);

        return additionalLightColor;
    }

    // Lightmap
    #define LIL_DECODE_LIGHTMAP(lm)                     DecodeLightmap(lm)
    #define LIL_DECODE_DYNAMICLIGHTMAP(lm)              DecodeRealtimeLightmap(lm)

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
    float3 lilGetEnvReflection(float3 viewDirection, float3 normalDirection, float perceptualRoughness, float3 positionWS)
    {
        UnityGIInput data = lilSetupGIInput(positionWS);
        Unity_GlossyEnvironmentData glossIn = lilSetupGlossyEnvironmentData(viewDirection,normalDirection,perceptualRoughness);
        return UnityGI_IndirectSpecular(data, 1.0, glossIn);
    }
    #define LIL_GET_ENVIRONMENT_REFLECTION(viewDirection,normalDirection,perceptualRoughness,positionWS) \
        lilGetEnvReflection(viewDirection,normalDirection,perceptualRoughness,positionWS)

    // Fog
    #define LIL_FOG_COORDS(idx)                         UNITY_FOG_COORDS(idx)
    #define LIL_TRANSFER_FOG(i,o)                       UNITY_TRANSFER_FOG(o,i.positionCS)
    #define LIL_APPLY_FOG(col,fogCoord)                 UNITY_APPLY_FOG(fogCoord,col)
    #define LIL_APPLY_FOG_COLOR(col,fogCoord,fogColor)  UNITY_APPLY_FOG_COLOR(fogCoord,col,fogColor)

    // Meta
    #define UnpackNormalScale(normal,scale)         UnpackScaleNormal(normal,scale)
    #define MetaInput                               UnityMetaInput
    #define MetaFragment(input)                     UnityMetaFragment(input)
    #define MetaVertexPosition(pos,uv1,uv2,l,d)     UnityMetaVertexPosition(pos,uv1,uv2,l,d)
#elif defined(LIL_HDRP)
    #define LIGHT_SIMULATE_HQ

    // Support for old version
    #if VERSION_LOWER(4, 1)
        #define LIL_HDRP_IGNORE_LIGHTDIMMER
        float4 EvaluateAtmosphericScattering(PositionInputs posInput, float3 viewDirection, float4 col)
        {
            return EvaluateAtmosphericScattering(posInput, col);
        }
    #endif

    #if VERSION_LOWER(4, 2)
        float GetDirectionalShadowAttenuation(HDShadowContext shadowContext, float2 positionSS, float3 positionWS, float3 normalWS, int shadowIndex, float3 L)
        {
            return GetDirectionalShadowAttenuation(shadowContext, positionWS, normalWS, shadowIndex, L, positionSS);
        }
    #endif

    #if VERSION_LOWER(5, 3)
        float GetCurrentExposureMultiplier()
        {
            return 1.0;
        }
    #endif

    #if VERSION_LOWER(6, 6)
        float3 SampleCameraColor(float2 uv, float lod)
        {
            return LIL_SAMPLE_2D_LOD(_ColorPyramidTexture, s_trilinear_clamp_sampler, uv, lod).rgb;
        }
    #endif

    #if VERSION_LOWER(6, 8)
        float4 EvaluateLight_Directional(LightLoopContext lightLoopContext, PositionInputs posInput, DirectionalLightData light)
        {
            float4 color = float4(light.color, 1.0);

            #if VERSION_GREATER_EQUAL(4, 1)
                float cosZenithAngle = -light.forward.y;
                float fragmentHeight = posInput.positionWS.y;
                color.a = TransmittanceHeightFog(_HeightFogBaseExtinction, _HeightFogBaseHeight, _HeightFogExponents, cosZenithAngle, fragmentHeight);
            #endif

            if(light.cookieIndex >= 0)
            {
                float3 lightToSample = posInput.positionWS - light.positionRWS;
                float3 cookie = EvaluateCookie_Directional(lightLoopContext, light, lightToSample);
                color.rgb *= cookie;
            }

            return color;
        }

        void GetPunctualLightVectors(float3 positionWS, LightData light, out float3 L, out float4 distances)
        {
            float3 lightToSample;
            GetPunctualLightVectors(positionWS, light, L, lightToSample, distances);
        }
    #endif

    #if VERSION_LOWER(7, 0)
        real3 UnpackNormalScale(real4 packedNormal, real bumpScale)
        {
            #if defined(UNITY_NO_DXT5nm)
                return UnpackNormalRGB(packedNormal, bumpScale);
            #else
                return UnpackNormalmapRGorAG(packedNormal, bumpScale);
            #endif
        }
    #endif

    #if VERSION_LOWER(7, 1)
        float3 TransformPreviousObjectToWorld(float3 positionOS)
        {
            float4x4 previousModelMatrix = ApplyCameraTranslationToMatrix(unity_MatrixPreviousM);
            return mul(previousModelMatrix, float4(positionOS, 1.0)).xyz;
        }
    #endif

    float4 SampleEnv(LightLoopContext lightLoopContext, PositionInputs posInput, EnvLightData lightData, float3 reflUVW, float lod)
    {
        #if VERSION_GREATER_EQUAL(10, 1)
            return SampleEnv(lightLoopContext, lightData.envIndex, reflUVW, lod * lightData.roughReflections, lightData.rangeCompressionFactorCompensation, posInput.positionNDC);
        #elif VERSION_GREATER_EQUAL(7, 1)
            return SampleEnv(lightLoopContext, lightData.envIndex, reflUVW, lod, lightData.rangeCompressionFactorCompensation);
        #else
            return SampleEnv(lightLoopContext, lightData.envIndex, reflUVW, lod);
        #endif
    }

    //------------------------------------------------------------------------------------------------------------------------------
    // HDRP Data
    uint lilGetRenderingLayer()
    {
        #if defined(RENDERING_LIGHT_LAYERS_MASK)
            return _EnableLightLayers ? (asuint(unity_RenderingLayer.x) & RENDERING_LIGHT_LAYERS_MASK) >> RENDERING_LIGHT_LAYERS_MASK_SHIFT : DEFAULT_LIGHT_LAYERS;
        #else
            return _EnableLightLayers ? asuint(unity_RenderingLayer.x) : DEFAULT_LIGHT_LAYERS;
        #endif
    }

    struct lilNPRLightingData
    {
        float3 color;
        float3 direction;
    };

    LightLoopContext lilInitLightLoopContext()
    {
        LightLoopContext lightLoopContext;
        lightLoopContext.shadowContext    = InitShadowContext();
        lightLoopContext.shadowValue      = 1;
        lightLoopContext.sampleReflection = 0;
        lightLoopContext.contactShadow    = 0;
        return lightLoopContext;
    }

    #define LIL_GET_HDRPDATA(input) \
        uint renderingLayers = lilGetRenderingLayer(); \
        uint featureFlags = LIGHT_FEATURE_MASK_FLAGS_OPAQUE; \
        uint2 tileIndex = uint2(0,0); \
        float4 positionSS = input.positionCS; \
        PositionInputs posInput = GetPositionInput(positionSS.xy, _ScreenSize.zw, positionSS.z, positionSS.w, input.positionWS, tileIndex); \
        input.positionWS = lilToAbsolutePositionWS(input.positionWS)

    //------------------------------------------------------------------------------------------------------------------------------
    // Direction Light
    bool lilUseScreenSpaceShadow(int screenSpaceShadowIndex)
    {
        #if defined(SCREEN_SPACE_SHADOW_INDEX_MASK) && defined(INVALID_SCREEN_SPACE_SHADOW)
            return (screenSpaceShadowIndex & SCREEN_SPACE_SHADOW_INDEX_MASK) != INVALID_SCREEN_SPACE_SHADOW;
        #else
            return screenSpaceShadowIndex >= 0;
        #endif
    }

    float4 lilGetDirectionalLightColor(PositionInputs posInput, DirectionalLightData light)
    {
        LightLoopContext lightLoopContext = lilInitLightLoopContext();
        return EvaluateLight_Directional(lightLoopContext, posInput, light);
    }

    lilNPRLightingData lilGetNPRDirectionalLight(PositionInputs posInput, DirectionalLightData light)
    {
        lilNPRLightingData lighting = (lilNPRLightingData)0;
        float3 L = -light.forward;
        #if !defined(LIL_HDRP_IGNORE_LIGHTDIMMER)
        if(light.lightDimmer > 0)
        #endif
        {
            float4 lightColor = lilGetDirectionalLightColor(posInput, light);
            lightColor.rgb *= lightColor.a;

            lighting.direction = L;
            lighting.color = lightColor.rgb;
        }
        return lighting;
    }

    void lilBlendlilNPRLightingData(inout lilNPRLightingData dst, lilNPRLightingData src)
    {
        dst.color += src.color;
        dst.direction += src.direction * Luminance(src.color);
    }

    float lilGetDirectionalShadow(PositionInputs posInput, float3 normalWS, uint featureFlags)
    {
        float attenuation = 1.0;
        if(featureFlags & LIGHTFEATUREFLAGS_DIRECTIONAL)
        {
            HDShadowContext shadowContext = InitShadowContext();
            if(_DirectionalShadowIndex >= 0)
            {
                DirectionalLightData light = _DirectionalLightDatas[_DirectionalShadowIndex];
                #if defined(SCREEN_SPACE_SHADOWS_ON)
                if(lilUseScreenSpaceShadow(light.screenSpaceShadowIndex))
                {
                    attenuation = GetScreenSpaceShadow(posInput, light.screenSpaceShadowIndex);
                }
                else
                #endif
                {
                    float3 L = -light.forward;
                    #if !defined(LIL_HDRP_IGNORE_LIGHTDIMMER)
                    if((light.lightDimmer > 0) && (light.shadowDimmer > 0))
                    #endif
                    {
                        attenuation = GetDirectionalShadowAttenuation(shadowContext, posInput.positionSS, posInput.positionWS, normalWS, light.shadowIndex, L);
                    }
                }
            }
        }
        return attenuation;
    }

    lilNPRLightingData lilGetDirectionalLightSum(PositionInputs posInput, uint renderingLayers, uint featureFlags)
    {
        lilNPRLightingData lightingData;
        lightingData.color = 0.0;
        lightingData.direction = float3(0.0, 0.001, 0.0);
        if(featureFlags & LIGHTFEATUREFLAGS_DIRECTIONAL)
        {
            for(uint i = 0; i < _DirectionalLightCount; ++i)
            {
                if((_DirectionalLightDatas[i].lightLayers & renderingLayers) != 0)
                {
                    lilNPRLightingData lighting = lilGetNPRDirectionalLight(posInput, _DirectionalLightDatas[i]);
                    lilBlendlilNPRLightingData(lightingData, lighting);
                }
            }
        }

        lightingData.direction = normalize(lightingData.direction);

        #ifdef LIGHT_SIMULATE_HQ
            lightingData.color = 0.0;
            if(featureFlags & LIGHTFEATUREFLAGS_DIRECTIONAL)
            {
                for(uint i = 0; i < _DirectionalLightCount; ++i)
                {
                    if((_DirectionalLightDatas[i].lightLayers & renderingLayers) != 0)
                    {
                        lilNPRLightingData lighting = lilGetNPRDirectionalLight(posInput, _DirectionalLightDatas[i]);
                        lightingData.color += lighting.color * saturate(dot(lightingData.direction, lighting.direction));
                    }
                }
            }
        #endif

        return lightingData;
    }

    void lilGetLightDirectionAndColor(out float3 lightDirection, out float3 lightColor, PositionInputs posInput)
    {
        uint renderingLayers = lilGetRenderingLayer();
        uint featureFlags = LIGHT_FEATURE_MASK_FLAGS_OPAQUE;

        lilNPRLightingData lightingData = lilGetDirectionalLightSum(posInput, renderingLayers, featureFlags);
        lightDirection = lightingData.direction;
        lightColor = lightingData.color;
    }

    //------------------------------------------------------------------------------------------------------------------------------
    // Punctual Light (Point / Spot)
    float4 EvaluateLight_Punctual(LightLoopContext lightLoopContext, float3 positionWS, LightData light, float3 L, float4 distances)
    {
        float4 color = float4(light.color, 1.0);
        color.a *= PunctualLightAttenuation(distances, light.rangeAttenuationScale, light.rangeAttenuationBias, light.angleScale, light.angleOffset);

        #if !defined(LIGHT_EVALUATION_NO_HEIGHT_FOG) && VERSION_GREATER_EQUAL(4, 1)
            float cosZenithAngle = L.y;
            float distToLight = (light.lightType == GPULIGHTTYPE_PROJECTOR_BOX) ? distances.w : distances.x;
            float fragmentHeight = positionWS.y;
            color.a *= TransmittanceHeightFog(_HeightFogBaseExtinction, _HeightFogBaseHeight, _HeightFogExponents, cosZenithAngle, fragmentHeight, distToLight);
        #endif

        #if VERSION_LOWER(7, 2)
            if(light.cookieIndex >= 0)
        #else
            if(light.cookieMode != COOKIEMODE_NONE)
        #endif
        {
            float3 lightToSample = positionWS - light.positionRWS;
            float4 cookie = EvaluateCookie_Punctual(lightLoopContext, light, lightToSample);
            color *= cookie;
        }

        return color;
    }

    lilNPRLightingData lilGetNPRPunctualLight(float3 positionWS, LightData light)
    {
        lilNPRLightingData lighting = (lilNPRLightingData)0;
        float3 L;
        float4 distances;
        GetPunctualLightVectors(positionWS, light, L, distances);
        #if !defined(LIL_HDRP_IGNORE_LIGHTDIMMER)
        if(light.lightDimmer > 0)
        #endif
        {
            LightLoopContext lightLoopContext;
            lightLoopContext.shadowContext    = InitShadowContext();
            lightLoopContext.shadowValue      = 1;
            lightLoopContext.sampleReflection = 0;
            lightLoopContext.contactShadow    = 0;

            float4 lightColor = EvaluateLight_Punctual(lightLoopContext, positionWS, light, L, distances);
            #if !defined(LIL_HDRP_IGNORE_LIGHTDIMMER)
                lightColor.a *= light.diffuseDimmer;
            #endif
            lightColor.rgb *= lightColor.a;

            lighting.direction = L;
            lighting.color = lightColor.rgb;
        }
        return lighting;
    }

    float3 lilGetPunctualLightColor(float3 positionWS, uint renderingLayers, uint featureFlags)
    {
        float3 lightColor = 0.0;
        if(featureFlags & LIGHTFEATUREFLAGS_PUNCTUAL)
        {
            uint lightStart = 0;
            bool fastPath = false;
            #if SCALARIZE_LIGHT_LOOP
                uint lightStartLane0;
                fastPath = IsFastPath(lightStart, lightStartLane0);
                if(fastPath) lightStart = lightStartLane0;
            #endif

            uint lightListOffset = 0;
            while(lightListOffset < _PunctualLightCount)
            {
                uint v_lightIdx = FetchIndex(lightStart, lightListOffset);
                #if SCALARIZE_LIGHT_LOOP
                    uint s_lightIdx = ScalarizeElementIndex(v_lightIdx, fastPath);
                #else
                    uint s_lightIdx = v_lightIdx;
                #endif
                if(s_lightIdx == -1) break;

                LightData lightData = FetchLight(s_lightIdx, 0);

                if(s_lightIdx >= v_lightIdx)
                {
                    lightListOffset++;
                    if((lightData.lightLayers & renderingLayers) != 0)
                    {
                        lilNPRLightingData lighting = lilGetNPRPunctualLight(positionWS, lightData);
                        lightColor += lighting.color;
                    }
                }
            }
        }

        return lightColor;
    }

    //------------------------------------------------------------------------------------------------------------------------------
    // Area Light (Line / Rectangle)
    float3 lilGetLineLightColor(float3 positionWS, LightData lightData)
    {
        float3 lightColor = 0.0;
        float intensity = EllipsoidalDistanceAttenuation(
            lightData.positionRWS - positionWS,
            lightData.right,
            saturate(lightData.range / (lightData.range + (0.5 * lightData.size.x))),
            lightData.rangeAttenuationScale,
            lightData.rangeAttenuationBias);
            #if !defined(LIL_HDRP_IGNORE_LIGHTDIMMER)
                intensity *= lightData.diffuseDimmer;
            #endif
        lightColor = lightData.color * intensity;
        return lightColor;
    }

    float3 lilGetRectLightColor(float3 positionWS, LightData lightData)
    {
        float3 lightColor = 0.0;
        #if SHADEROPTIONS_BARN_DOOR
            RectangularLightApplyBarnDoor(lightData, positionWS);
        #endif
        float3 unL = lightData.positionRWS - positionWS;
        if(dot(lightData.forward, unL) < FLT_EPS)
        {
            float3x3 lightToWorld = float3x3(lightData.right, lightData.up, -lightData.forward);
            unL = mul(unL, transpose(lightToWorld));
            float halfWidth  = lightData.size.x * 0.5;
            float halfHeight = lightData.size.y * 0.5;
            float3 invHalfDim = rcp(float3(lightData.range + halfWidth, lightData.range + halfHeight, lightData.range));
            #ifdef ELLIPSOIDAL_ATTENUATION
                float intensity = EllipsoidalDistanceAttenuation(unL, invHalfDim, lightData.rangeAttenuationScale, lightData.rangeAttenuationBias);
            #else
                float intensity = BoxDistanceAttenuation(unL, invHalfDim, lightData.rangeAttenuationScale, lightData.rangeAttenuationBias);
            #endif
            #if !defined(LIL_HDRP_IGNORE_LIGHTDIMMER)
                intensity *= lightData.diffuseDimmer;
            #endif
            lightColor = lightData.color * intensity;
        }
        return lightColor;
    }

    float3 lilGetAreaLightColor(float3 positionWS, uint renderingLayers, uint featureFlags)
    {
        float3 lightColor = 0.0;
        #if SHADEROPTIONS_AREA_LIGHTS
            if(featureFlags & LIGHTFEATUREFLAGS_AREA)
            {
                if(_AreaLightCount > 0)
                {
                    uint i = 0;
                    uint last = _AreaLightCount - 1;
                    LightData lightData = FetchLight(_PunctualLightCount, i);

                    while(i <= last && lightData.lightType == GPULIGHTTYPE_TUBE)
                    {
                        lightData.lightType = GPULIGHTTYPE_TUBE;
                        #if defined(COOKIEMODE_NONE)
                            lightData.cookieMode = COOKIEMODE_NONE;
                        #endif
                        if((lightData.lightLayers & renderingLayers) != 0)
                        {
                            lightColor += lilGetLineLightColor(positionWS, lightData);
                        }
                        lightData = FetchLight(_PunctualLightCount, min(++i, last));
                    }

                    while(i <= last)
                    {
                        lightData.lightType = GPULIGHTTYPE_RECTANGLE;
                        if((lightData.lightLayers & renderingLayers) != 0)
                        {
                            lightColor += lilGetRectLightColor(positionWS, lightData);
                        }
                        lightData = FetchLight(_PunctualLightCount, min(++i, last));
                    }
                }
            }
        #endif
        return lightColor;
    }

    //------------------------------------------------------------------------------------------------------------------------------
    // Reflection / Refraction
    float3 lilGetReflectionColor(
        LightLoopContext lightLoopContext, PositionInputs posInput, float3 reflUVW, float perceptualRoughness, float3 normalDirection,
        EnvLightData lightData, int influenceShapeType, inout float hierarchyWeight)
    {
        float weight = 1.0;
        EvaluateLight_EnvIntersection(posInput.positionWS, normalDirection, lightData, influenceShapeType, reflUVW, weight);
        float4 preLD = SampleEnv(lightLoopContext, posInput, lightData, reflUVW, PerceptualRoughnessToMipmapLevel(perceptualRoughness));
        weight *= preLD.a;
        UpdateLightingHierarchyWeights(hierarchyWeight, weight);
        return preLD.rgb * weight * lightData.multiplier;
    }

    float3 lilGetReflectionSum(float3 viewDirection, float3 normalDirection, float perceptualRoughness, PositionInputs posInput, uint renderingLayers, uint featureFlags)
    {
        float3 reflUVW = reflect(-viewDirection, normalDirection);
        LightLoopContext lightLoopContext = lilInitLightLoopContext();
        float3 specular = 0.0;
        if(featureFlags & (LIGHTFEATUREFLAGS_ENV | LIGHTFEATUREFLAGS_SKY))
        {
            float reflectionHierarchyWeight = 0.0;
            uint envLightStart = 0;

            bool fastPath = false;
            #if SCALARIZE_LIGHT_LOOP
                uint envStartFirstLane;
                fastPath = IsFastPath(envLightStart, envStartFirstLane);
            #endif

            EnvLightData envLightData;
            if(_EnvLightCount > 0)  envLightData = FetchEnvLight(envLightStart, 0);
            else                    envLightData = InitSkyEnvLightData(0);

            if(featureFlags & LIGHTFEATUREFLAGS_ENV)
            {
                lightLoopContext.sampleReflection = SINGLE_PASS_CONTEXT_SAMPLE_REFLECTION_PROBES;

                #if SCALARIZE_LIGHT_LOOP
                    if(fastPath) envLightStart = envStartFirstLane;
                #endif

                uint v_envLightListOffset = 0;
                uint v_envLightIdx = envLightStart;
                while(v_envLightListOffset < _EnvLightCount)
                {
                    v_envLightIdx = FetchIndex(envLightStart, v_envLightListOffset);
                    #if SCALARIZE_LIGHT_LOOP
                        uint s_envLightIdx = ScalarizeElementIndex(v_envLightIdx, fastPath);
                    #else
                        uint s_envLightIdx = v_envLightIdx;
                    #endif
                    if(s_envLightIdx == -1) break;

                    EnvLightData s_envLightData = FetchEnvLight(s_envLightIdx, 0);
                    if(s_envLightIdx >= v_envLightIdx)
                    {
                        v_envLightListOffset++;
                        if((reflectionHierarchyWeight < 1.0) && ((s_envLightData.lightLayers & renderingLayers) != 0))
                        {
                            specular += lilGetReflectionColor(lightLoopContext, posInput, reflUVW, perceptualRoughness, normalDirection, s_envLightData, s_envLightData.influenceShapeType, reflectionHierarchyWeight);
                        }
                    }
                }
            }

            if((featureFlags & LIGHTFEATUREFLAGS_SKY) && _EnvLightSkyEnabled)
            {
                lightLoopContext.sampleReflection = SINGLE_PASS_CONTEXT_SAMPLE_SKY;
                EnvLightData envLightSky = InitSkyEnvLightData(0);
                if(reflectionHierarchyWeight < 1.0)
                {
                    specular += lilGetReflectionColor(lightLoopContext, posInput, reflUVW, perceptualRoughness, normalDirection, envLightSky, envLightSky.influenceShapeType, reflectionHierarchyWeight);
                }
            }
        }
        return specular * GetCurrentExposureMultiplier();
    }

    // Main light
    #define LIL_MAINLIGHT_COLOR                         float3(1,1,1)
    #define LIL_MAINLIGHT_DIRECTION                     float3(0,1,0)

    // Shadow
    #define LIL_SHADOW_COORDS(idx)
    #define LIL_TRANSFER_SHADOW(vi,uv,o)
    #if defined(LIL_USE_SHADOW)
        #define LIL_LIGHT_ATTENUATION(atten,i)      atten = lilGetDirectionalShadow(posInput, i.normalWS, featureFlags)
    #else
        #define LIL_LIGHT_ATTENUATION(atten,i)
    #endif

    // Shadow caster
    #define LIL_V2F_SHADOW_CASTER_OUTPUT
    #define LIL_TRANSFER_SHADOW_CASTER(v,o)
    #define LIL_SHADOW_CASTER_FRAGMENT(i)

    // Additional Light
    float3 lilGetAdditionalLights(float3 positionWS)
    {
        uint renderingLayers = lilGetRenderingLayer();
        uint featureFlags = LIGHT_FEATURE_MASK_FLAGS_OPAQUE;

        float3 additionalLightColor = 0.0;
        additionalLightColor += lilGetPunctualLightColor(positionWS, renderingLayers, featureFlags);
        additionalLightColor += lilGetAreaLightColor(positionWS, renderingLayers, featureFlags);
        additionalLightColor *= 0.75 * GetCurrentExposureMultiplier();

        return additionalLightColor;
    }

    // Lightmap
    #define LIL_DECODE_LIGHTMAP(lm)                     DecodeLightmap(lm, float4(LIGHTMAP_HDR_MULTIPLIER,LIGHTMAP_HDR_EXPONENT,0.0,0.0))
    #define LIL_DECODE_DYNAMICLIGHTMAP(lm)              DecodeLightmap(lm, float4(LIGHTMAP_HDR_MULTIPLIER,LIGHTMAP_HDR_EXPONENT,0.0,0.0))

    // Environment reflection
    #define LIL_GET_ENVIRONMENT_REFLECTION(viewDirection,normalDirection,perceptualRoughness,positionWS) \
        lilGetReflectionSum(viewDirection,normalDirection,perceptualRoughness,posInput,renderingLayers,featureFlags)

    // Fog
    #define LIL_FOG_COORDS(idx)
    #define LIL_TRANSFER_FOG(i,o)
    #define LIL_APPLY_FOG(col,fogCoord)                 col = EvaluateAtmosphericScattering(posInput, viewDirection, col)
    #define LIL_APPLY_FOG_COLOR(col,fogCoord,fogColor)  col = EvaluateAtmosphericScattering(posInput, viewDirection, col)
#else
    // HDRP Data
    uint lilGetRenderingLayer()
    {
        return 0;
    }
    #define LIL_GET_HDRPDATA(input)

    // Main light
    #define LIL_MAINLIGHT_COLOR                         _MainLightColor.rgb
    #define LIL_MAINLIGHT_DIRECTION                     _MainLightPosition.xyz

    // Shadow
    #if defined(LIL_USE_SHADOW)
        #if defined(_MAIN_LIGHT_SHADOWS_SCREEN)
            #define LIL_SHADOW_COORDS(idx)              float4 shadowCoord : TEXCOORD##idx;
            #define LIL_TRANSFER_SHADOW(vi,uv,o)        o.shadowCoord = ComputeScreenPos(vi.positionCS);
            #define LIL_LIGHT_ATTENUATION(atten,i) \
                atten = MainLightRealtimeShadow(i.shadowCoord)
        #elif defined(_MAIN_LIGHT_SHADOWS)
            #define LIL_SHADOW_COORDS(idx)              float4 shadowCoord : TEXCOORD##idx;
            #define LIL_TRANSFER_SHADOW(vi,uv,o)        o.shadowCoord = TransformWorldToShadowCoord(vi.positionWS);
            #define LIL_LIGHT_ATTENUATION(atten,i) \
                atten = MainLightRealtimeShadow(i.shadowCoord)
        #else
            #define LIL_SHADOW_COORDS(idx)
            #define LIL_TRANSFER_SHADOW(vi,uv,o)
            #define LIL_LIGHT_ATTENUATION(atten,i) \
                float4 shadowCoord = TransformWorldToShadowCoord(i.positionWS); \
                atten = MainLightRealtimeShadow(shadowCoord)
        #endif
    #else
        #define LIL_SHADOW_COORDS(idx)
        #define LIL_TRANSFER_SHADOW(vi,uv,o)
        #define LIL_LIGHT_ATTENUATION(atten,i)
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

        #if VERSION_GREATER_EQUAL(5, 1)
            float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));
        #else
            float4 positionCS = TransformWorldToHClip(positionWS);
        #endif

        #if UNITY_REVERSED_Z
            positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
        #else
            positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
        #endif

        return positionCS;
    }
    #define LIL_V2F_SHADOW_CASTER_OUTPUT        float4 positionCS : SV_POSITION;
    #define LIL_TRANSFER_SHADOW_CASTER(v,o)     o.positionCS = URPShadowPos(v.positionOS, v.normalOS)
    #define LIL_SHADOW_CASTER_FRAGMENT(i)       return 0

    // Additional Light
    float3 lilGetAdditionalLights(float3 positionWS)
    {
        float3 additionalLightColor = 0.0;

        #if defined(_ADDITIONAL_LIGHTS) || defined(_ADDITIONAL_LIGHTS_VERTEX)
            uint lightsCount = GetAdditionalLightsCount();
            for (uint lightIndex = 0; lightIndex < lightsCount; lightIndex++)
            {
                Light light = GetAdditionalLight(lightIndex, positionWS);
                additionalLightColor += light.color * light.distanceAttenuation;
            }
            additionalLightColor = saturate(additionalLightColor);
        #endif

        return additionalLightColor;
    }

    // Lightmap
    #define LIL_DECODE_LIGHTMAP(lm)                     DecodeLightmap(lm, float4(LIGHTMAP_HDR_MULTIPLIER,LIGHTMAP_HDR_EXPONENT,0.0,0.0))
    #define LIL_DECODE_DYNAMICLIGHTMAP(lm)              DecodeLightmap(lm, float4(LIGHTMAP_HDR_MULTIPLIER,LIGHTMAP_HDR_EXPONENT,0.0,0.0))

    // Environment reflection
    #define LIL_GET_ENVIRONMENT_REFLECTION(viewDirection,normalDirection,perceptualRoughness,positionWS) \
        GlossyEnvironmentReflection(reflect(-viewDirection,normalDirection), perceptualRoughness, 1.0)

    // Fog
    #define LIL_FOG_COORDS(idx)                         float fogCoord : TEXCOORD##idx;
    #define LIL_TRANSFER_FOG(i,o)                       o.fogCoord = ComputeFogFactor(i.positionCS.z)
    #define LIL_APPLY_FOG(col,fogCoord)                 col.rgb = MixFog(col.rgb,fogCoord)
    #define LIL_APPLY_FOG_COLOR(col,fogCoord,fogColor)  col.rgb = MixFogColor(col.rgb,fogColor.rgb,fogCoord)
#endif

// Meta
#if !defined(LIL_BRP) && (VERSION_LOWER(5, 14))
    #define LIL_TRANSFER_METAPASS(input,output) \
        output.positionCS = MetaVertexPosition(input.positionOS, input.uv1, input.uv2, unity_LightmapST)
#else
    #define LIL_TRANSFER_METAPASS(input,output) \
        output.positionCS = MetaVertexPosition(input.positionOS, input.uv1, input.uv2, unity_LightmapST, unity_DynamicLightmapST)
#endif

#if defined(LIL_HDRP)
    #define LIL_IS_MIRROR           false
    #define LIL_LIGHTDIRECTION_ORIG lightDirection
#else
    #define LIL_IS_MIRROR           unity_CameraProjection._m20 != 0.0 || unity_CameraProjection._m21 != 0.0
    #define LIL_LIGHTDIRECTION_ORIG LIL_MAINLIGHT_DIRECTION
#endif

//------------------------------------------------------------------------------------------------------------------------------
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
#if (defined(LIL_LITE) || defined(LIL_OUTLINE) || defined(LIL_FUR) || defined(LIL_GEM)) && !defined(LIL_PASS_FORWARDADD)
    #undef LIL_SHADOW_COORDS
    #undef LIL_TRANSFER_SHADOW
    #undef LIL_LIGHT_ATTENUATION
    #define LIL_SHADOW_COORDS(idx)
    #define LIL_TRANSFER_SHADOW(vi,uv,o)
    #define LIL_LIGHT_ATTENUATION(atten,i)
#endif

// Transform
#define LIL_VERTEX_POSITION_INPUTS(positionOS,o)                lilVertexPositionInputs o = lilGetVertexPositionInputs(positionOS.xyz)
#define LIL_RE_VERTEX_POSITION_INPUTS(o)                        o = lilReGetVertexPositionInputs(o)
#define LIL_VERTEX_NORMAL_INPUTS(normalOS,o)                    lilVertexNormalInputs o = lilGetVertexNormalInputs(normalOS)
#define LIL_VERTEX_NORMAL_TANGENT_INPUTS(normalOS,tangentOS,o)  lilVertexNormalInputs o = lilGetVertexNormalInputs(normalOS,tangentOS)

// Lightmap
#if defined(LIL_USE_DOTS_INSTANCING)
    #define LIL_SHADOWMAP_TEX                   unity_ShadowMasks
    #define LIL_SHADOWMAP_SAMP                  samplerunity_ShadowMasks
    #define LIL_LIGHTMAP_TEX                    unity_Lightmaps
    #define LIL_LIGHTMAP_SAMP                   samplerunity_Lightmaps
    #define LIL_DYNAMICLIGHTMAP_TEX             unity_DynamicLightmap
    #define LIL_DYNAMICLIGHTMAP_SAMP            samplerunity_DynamicLightmap
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

// Main Light Coords
#if defined(LIL_PASS_FORWARDADD)
    #define LIL_LIGHTCOLOR_COORDS(idx)
    #define LIL_LIGHTDIRECTION_COORDS(idx)
#else
    #define LIL_LIGHTCOLOR_COORDS(idx)      LIL_NOPERSPECTIVE float3 lightColor : TEXCOORD##idx;
    #define LIL_LIGHTDIRECTION_COORDS(idx)  LIL_NOPERSPECTIVE float3 lightDirection : TEXCOORD##idx;
#endif

#if !defined(LIL_PASS_FORWARDADD) && (defined(LIL_FEATURE_SHADOW) || defined(LIL_LITE))
    #define LIL_INDLIGHTCOLOR_COORDS(idx)   LIL_NOPERSPECTIVE float3 indLightColor : TEXCOORD##idx;
#else
    #define LIL_INDLIGHTCOLOR_COORDS(idx)
#endif

// Dir light & indir light
#if defined(LIL_USE_LPPV) && (defined(LIL_FEATURE_SHADOW) || defined(LIL_LITE))
    #define LIL_CALC_TWOLIGHT(i,o) lilGetLightColorDouble(o.lightDirection, i.positionWS, o.lightColor, o.indLightColor)
#elif defined(LIL_FEATURE_SHADOW) || defined(LIL_LITE)
    #define LIL_CALC_TWOLIGHT(i,o) lilGetLightColorDouble(o.lightDirection, o.lightColor, o.indLightColor)
#elif defined(LIL_USE_LPPV)
    #define LIL_CALC_TWOLIGHT(i,o) o.lightColor = lilGetLightColor(i.positionWS)
#else
    #define LIL_CALC_TWOLIGHT(i,o) o.lightColor = lilGetLightColor()
#endif

// Main Light in VS (Color / Direction)
struct lilLightData
{
    float3 lightDirection;
    float3 lightColor;
    float3 indLightColor;
};


// Main Light in VS
#if defined(LIL_USE_LIGHTMAP)
    #define LIL_CORRECT_LIGHTCOLOR_VS(lightColor)
    #define LIL_CORRECT_LIGHTCOLOR_PS(lightColor) \
        lightColor = clamp(lightColor, _LightMinLimit, _LightMaxLimit); \
        lightColor = lerp(lightColor, lilGray(lightColor), _MonochromeLighting); \
        lightColor = lerp(lightColor, 1.0, _AsUnlit)
#else
    #define LIL_CORRECT_LIGHTCOLOR_VS(lightColor) \
        lightColor = clamp(lightColor, _LightMinLimit, _LightMaxLimit); \
        lightColor = lerp(lightColor, lilGray(lightColor), _MonochromeLighting); \
        lightColor = lerp(lightColor, 1.0, _AsUnlit)
    #define LIL_CORRECT_LIGHTCOLOR_PS(lightColor)
#endif

#if defined(LIL_PASS_FORWARDADD)
    #define LIL_CALC_MAINLIGHT(i,o)
#elif defined(LIL_HDRP)
    #define LIL_CALC_MAINLIGHT(i,o) \
        lilLightData o; \
        lilGetLightDirectionAndColor(o.lightDirection, o.lightColor, posInput); \
        o.lightColor *= _lilDirectionalLightStrength; \
        float3 lightDirectionCopy = o.lightDirection; \
        o.lightDirection = normalize(o.lightDirection * Luminance(o.lightColor) + unity_SHAr.xyz * 0.333333 + unity_SHAg.xyz * 0.333333 + unity_SHAb.xyz * 0.333333 + _LightDirectionOverride.xyz); \
        float3 shLightColor = lilShadeSH9(float4(o.lightDirection * 0.666666, 1.0)); \
        o.lightColor += shLightColor; \
        o.indLightColor = lilShadeSH9(float4(-o.lightDirection * 0.666666, 1.0)); \
        o.indLightColor = saturate(o.indLightColor / Luminance(o.lightColor)); \
        o.lightColor = min(o.lightColor, _BeforeExposureLimit); \
        o.lightColor *= GetCurrentExposureMultiplier(); \
        LIL_CORRECT_LIGHTCOLOR_VS(o.lightColor)
#else
    #define LIL_CALC_MAINLIGHT(i,o) \
        lilLightData o; \
        o.lightDirection = lilGetLightDirection(_LightDirectionOverride); \
        LIL_CALC_TWOLIGHT(i,o); \
        LIL_CORRECT_LIGHTCOLOR_VS(o.lightColor)
#endif

// Main Light in PS (Color / Direction / Attenuation)
#if defined(LIL_PASS_FORWARDADD)
    // Point Light & Spot Light (ForwardAdd)
    #define LIL_GET_MAINLIGHT(input,lightColor,lightDirection,atten) \
        LIL_LIGHT_ATTENUATION(atten, input); \
        lightDirection = lilGetLightDirection(input.positionWS); \
        lightColor = saturate(LIL_MAINLIGHT_COLOR * atten); \
        lightColor = lerp(lightColor, lilGray(lightColor), _MonochromeLighting); \
        lightColor = lerp(lightColor, 0.0, _AsUnlit)
#elif defined(LIL_HDRP) && defined(LIL_USE_LIGHTMAP)
    // HDRP with lightmap
    #define LIL_GET_MAINLIGHT(input,lightColor,lightDirection,atten) \
        LIL_LIGHT_ATTENUATION(atten, input); \
        lightColor = input.lightColor; \
        float3 lightmapColor = lilGetLightMapColor(input.uv1); \
        lightColor += lightmapColor * GetCurrentExposureMultiplier(); \
        LIL_CORRECT_LIGHTCOLOR_PS(lightColor)
#elif defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SHADOWMASK)
    // Mixed Lightmap (Shadowmask)
    #define LIL_GET_MAINLIGHT(input,lightColor,lightDirection,atten) \
        LIL_LIGHT_ATTENUATION(atten, input); \
        lightColor = input.lightColor; \
        float3 lightmapColor = lilGetLightMapColor(input.uv1); \
        lightColor = max(lightColor, lightmapColor); \
        LIL_CORRECT_LIGHTCOLOR_PS(lightColor); \
        atten = min(atten, LIL_SAMPLE_LIGHTMAP(LIL_SHADOWMAP_TEX,LIL_LIGHTMAP_SAMP,input.uv1).r)
#elif defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SUBTRACTIVE) && defined(LIL_USE_DYNAMICLIGHTMAP)
    // Mixed Lightmap (Subtractive)
    // Use Lightmap as Shadowmask
    #undef LIL_USE_DYNAMICLIGHTMAP
    #define LIL_GET_MAINLIGHT(input,lightColor,lightDirection,atten) \
        LIL_LIGHT_ATTENUATION(atten, input); \
        lightColor = input.lightColor; \
        float3 lightmapColor = lilGetLightMapColor(input.uv1); \
        lightColor = max(lightColor, lightmapColor); \
        LIL_CORRECT_LIGHTCOLOR_PS(lightColor); \
        float3 lightmapShadowThreshold = LIL_MAINLIGHT_COLOR*0.5; \
        float3 lightmapS = (lightmapColor - lightmapShadowThreshold) / (LIL_MAINLIGHT_COLOR - lightmapShadowThreshold); \
        float lightmapAttenuation = saturate((lightmapS.r+lightmapS.g+lightmapS.b)/3.0); \
        atten = min(atten, lightmapAttenuation)
#elif defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SUBTRACTIVE)
    // Mixed Lightmap (Subtractive)
    // Use Lightmap as Shadowmask
    #define LIL_GET_MAINLIGHT(input,lightColor,lightDirection,atten) \
        LIL_LIGHT_ATTENUATION(atten, input); \
        lightColor = input.lightColor; \
        float3 lightmapColor = lilGetLightMapColor(input.uv1); \
        lightColor = max(lightColor, lightmapColor); \
        LIL_CORRECT_LIGHTCOLOR_PS(lightColor); \
        float3 lightmapS = (lightmapColor - lilShadeSH9(input.normalWS)) / LIL_MAINLIGHT_COLOR; \
        float lightmapAttenuation = saturate((lightmapS.r+lightmapS.g+lightmapS.b)/3.0); \
        atten = min(atten, lightmapAttenuation)
#elif defined(LIL_USE_LIGHTMAP) && defined(LIL_USE_DIRLIGHTMAP)
    // Lightmap (Directional)
    #define LIL_GET_MAINLIGHT(input,lightColor,lightDirection,atten) \
        LIL_LIGHT_ATTENUATION(atten, input); \
        lightColor = input.lightColor; \
        float3 lightmapColor = lilGetLightMapColor(input.uv1); \
        float3 lightmapDirection = lilGetLightMapDirection(input.uv1); \
        lightColor = saturate(lightColor + lightmapColor); \
        LIL_CORRECT_LIGHTCOLOR_PS(lightColor); \
        lightDirection = normalize(lightDirection + lightmapDirection * lilLuminance(lightmapColor))
#elif defined(LIL_USE_LIGHTMAP) && defined(LIL_USE_SHADOW)
    // Mixed Lightmap (Baked Indirect) with shadow
    #define LIL_GET_MAINLIGHT(input,lightColor,lightDirection,atten) \
        LIL_LIGHT_ATTENUATION(atten, input); \
        lightColor = LIL_MAINLIGHT_COLOR; \
        float3 lightmapColor = lilGetLightMapColor(input.uv1); \
        lightColor = saturate(lightColor + max(lightmapColor,lilGetSHToon(_LightDirectionOverride))); \
        LIL_CORRECT_LIGHTCOLOR_PS(lightColor)
#elif defined(LIL_USE_LIGHTMAP) && defined(LIL_USE_DYNAMICLIGHTMAP)
    // Mixed Lightmap (Baked Indirect) or Lightmap (Non-Directional)
    #undef LIL_USE_DYNAMICLIGHTMAP
    #define LIL_GET_MAINLIGHT(input,lightColor,lightDirection,atten) \
        LIL_LIGHT_ATTENUATION(atten, input); \
        lightColor = input.lightColor; \
        float3 lightmapColor = lilGetLightMapColor(input.uv1); \
        lightColor = saturate(lightColor + lightmapColor); \
        LIL_CORRECT_LIGHTCOLOR_PS(lightColor)
#elif defined(LIL_USE_LIGHTMAP)
    // Mixed Lightmap (Baked Indirect) or Lightmap (Non-Directional)
    #define LIL_GET_MAINLIGHT(input,lightColor,lightDirection,atten) \
        LIL_LIGHT_ATTENUATION(atten, input); \
        lightColor = LIL_MAINLIGHT_COLOR; \
        float3 lightmapColor = lilGetLightMapColor(input.uv1); \
        lightColor = saturate(lightColor + lightmapColor); \
        LIL_CORRECT_LIGHTCOLOR_PS(lightColor)
#else
    // Realtime
    #define LIL_GET_MAINLIGHT(input,lightColor,lightDirection,atten) \
        LIL_LIGHT_ATTENUATION(atten, input); \
        lightColor = input.lightColor;
#endif

// Additional Light VS
#if defined(LIL_USE_ADDITIONALLIGHT_VS) && (defined(VERTEXLIGHT_ON) || defined(LIL_TESSELLATION) || !defined(LIL_BRP))
    #define LIL_VERTEXLIGHT_COORDS(idx) float3 vl : TEXCOORD##idx;
    #define LIL_CALC_VERTEXLIGHT(i,o) \
        o.vl = lilGetAdditionalLights(i.positionWS); \
        o.vl = lerp(o.vl, lilGray(o.vl), _MonochromeLighting); \
        o.vl = lerp(o.vl, 0.0, _AsUnlit)
#elif defined(LIL_USE_ADDITIONALLIGHT_VS)
    #define LIL_VERTEXLIGHT_COORDS(idx) float3 vl : TEXCOORD##idx;
    #define LIL_CALC_VERTEXLIGHT(i,o)
#else
    #define LIL_VERTEXLIGHT_COORDS(idx)
    #define LIL_CALC_VERTEXLIGHT(i,o)
#endif

// Additional Light PS
#if defined(LIL_USE_ADDITIONALLIGHT_PS)
    #define LIL_GET_ADDITIONALLIGHT(i,o) \
        o = lilGetAdditionalLights(i.positionWS); \
        o = lerp(o, lilGray(o), _MonochromeLighting); \
        o = lerp(o, 0.0, _AsUnlit)
#elif defined(LIL_USE_ADDITIONALLIGHT_VS)
    #define LIL_GET_ADDITIONALLIGHT(i,o) \
        o = i.vl
#else
    #define LIL_GET_ADDITIONALLIGHT(i,o) \
        o = 0
#endif

// Main Color & Emission
#if defined(LIL_WITHOUT_ANIMATION)
    #define LIL_GET_SUBTEX(tex,uv)  lilGetSubTexWithoutAnimation(Exists##tex, tex, tex##_ST, tex##Angle, uv, 1, tex##IsDecal, tex##IsLeftOnly, tex##IsRightOnly, tex##ShouldCopy, tex##ShouldFlipMirror, tex##ShouldFlipCopy, tex##IsMSDF, isRightHand LIL_SAMP_IN(sampler##tex))
    #define LIL_GET_EMITEX(tex,uv)  LIL_SAMPLE_2D(tex, sampler##tex, lilCalcUVWithoutAnimation(uv, tex##_ST, tex##_ScrollRotate))
    #define LIL_GET_EMIMASK(tex,uv) LIL_SAMPLE_2D(tex, sampler_MainTex, lilCalcUVWithoutAnimation(uv, tex##_ST, tex##_ScrollRotate))
#else
    #define LIL_GET_SUBTEX(tex,uv)  lilGetSubTex(Exists##tex, tex, tex##_ST, tex##Angle, uv, nv, tex##IsDecal, tex##IsLeftOnly, tex##IsRightOnly, tex##ShouldCopy, tex##ShouldFlipMirror, tex##ShouldFlipCopy, tex##IsMSDF, isRightHand, tex##DecalAnimation, tex##DecalSubParam LIL_SAMP_IN(sampler##tex))
    #define LIL_GET_EMITEX(tex,uv)  LIL_SAMPLE_2D(tex, sampler##tex, lilCalcUV(uv, tex##_ST, tex##_ScrollRotate))
    #define LIL_GET_EMIMASK(tex,uv) LIL_SAMPLE_2D(tex, sampler_MainTex, lilCalcUV(uv, tex##_ST, tex##_ScrollRotate))
#endif

#endif