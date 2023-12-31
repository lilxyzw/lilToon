#ifndef LIL_MACRO_INCLUDED
#define LIL_MACRO_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Setting

// The version of SRP is automatically determined, but an error may occur in a specific version.
// In that case, define the version.
// Example: HDRP 4.8.0
// #define LIL_SRP_VERSION_MAJOR 4
// #define LIL_SRP_VERSION_MINOR 8

// Transparent mode on subpass (Default : 0)
// 0 : Cutout
// 1 : Dither
#define LIL_SUBPASS_TRANSPARENT_MODE 0

// Refraction blur
#define LIL_REFRACTION_SAMPNUM 8
#define LIL_REFRACTION_GAUSDIST(i) exp(-(float)i*(float)i/(LIL_REFRACTION_SAMPNUM*LIL_REFRACTION_SAMPNUM/2.0))

// Antialias mode (Default : 1)
// 0 : Off
// 1 : On
#define LIL_ANTIALIAS_MODE 1

// Light Probe Proxy Volumes (Default : 0)
#define LIL_LPPV_MODE 0
// 0 : Off
// 1 : On

// Additional Lights Mode (Default : 3 or 4)
// 0 : Off
// 1 : In Vertex Shader
// 2 : In Fragment Shader
// 3 : Add to main light
// 4 : Add to main light with direction
// 5 : Add to main light with direction in Fragment Shader
#if defined(LIL_BRP)
    #define LIL_ADDITIONAL_LIGHT_MODE 3
    #define LIL_ADDITIONAL_LIGHT_STRENGTH _VertexLightStrength
#elif defined(LIL_HDRP)
    #define LIL_ADDITIONAL_LIGHT_MODE 5
    #define LIL_ADDITIONAL_LIGHT_STRENGTH 1
#elif defined(USE_CLUSTERED_LIGHTING) && USE_CLUSTERED_LIGHTING || defined(USE_FORWARD_PLUS) && USE_FORWARD_PLUS
    #define LIL_ADDITIONAL_LIGHT_MODE 5
    #define LIL_ADDITIONAL_LIGHT_STRENGTH 1
#else
    #define LIL_ADDITIONAL_LIGHT_MODE 4
    #define LIL_ADDITIONAL_LIGHT_STRENGTH 1
#endif

// Near clip threshold for clipping canceller (Default : 0.1)
#define LIL_NEARCLIP_THRESHOLD 0.1

//------------------------------------------------------------------------------------------------------------------------------
// Version
#if !defined(LIL_SRP_VERSION_MAJOR)
    #if UNITY_VERSION < 201810
        #define LIL_SRP_VERSION_MAJOR 0
    #elif UNITY_VERSION < 201820
        #define LIL_SRP_VERSION_MAJOR 1
    #elif UNITY_VERSION < 201830
        #define LIL_SRP_VERSION_MAJOR 2
    #elif UNITY_VERSION < 201840
        #define LIL_SRP_VERSION_MAJOR 3
    #elif UNITY_VERSION < 201910
        #define LIL_SRP_VERSION_MAJOR 4
    #elif UNITY_VERSION < 201920
        #define LIL_SRP_VERSION_MAJOR 5
    #elif UNITY_VERSION < 201930
        #define LIL_SRP_VERSION_MAJOR 6
    #elif UNITY_VERSION < 201940
        #define LIL_SRP_VERSION_MAJOR 7
    #elif UNITY_VERSION < 202010
        #define LIL_SRP_VERSION_MAJOR 8
    #elif UNITY_VERSION < 202020
        #define LIL_SRP_VERSION_MAJOR 9
    #elif UNITY_VERSION < 202030
        #define LIL_SRP_VERSION_MAJOR 10
    #elif UNITY_VERSION < 202110
        #define LIL_SRP_VERSION_MAJOR 11
    #elif UNITY_VERSION < 202120
        #define LIL_SRP_VERSION_MAJOR 12
    #elif UNITY_VERSION < 202210
        #define LIL_SRP_VERSION_MAJOR 13
    #else
        #define LIL_SRP_VERSION_MAJOR 14
    #endif
#endif
#if !defined(LIL_SRP_VERSION_MINOR)
    #define LIL_SRP_VERSION_MINOR 99
#endif
#if !defined(LIL_SRP_VERSION_GREATER_EQUAL)
    #define LIL_SRP_VERSION_GREATER_EQUAL(major, minor) ((LIL_SRP_VERSION_MAJOR > major) || ((LIL_SRP_VERSION_MAJOR == major) && (LIL_SRP_VERSION_MINOR >= minor)))
    #define LIL_SRP_VERSION_LOWER(major, minor) ((LIL_SRP_VERSION_MAJOR < major) || ((LIL_SRP_VERSION_MAJOR == major) && (LIL_SRP_VERSION_MINOR < minor)))
    #define LIL_SRP_VERSION_EQUAL(major, minor) ((LIL_SRP_VERSION_MAJOR == major) && (LIL_SRP_VERSION_MINOR == minor))
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Replace Macro
#define LIL_BRANCH
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
    float lilLuminance(float3 rgb) { return dot(rgb, float3(0.22, 0.707, 0.071)); }
#else
    float lilLuminance(float3 rgb) { return dot(rgb, float3(0.0396819152, 0.458021790, 0.00609653955)); }
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
    #if LIL_ADDITIONAL_LIGHT_MODE == 1
        #define LIL_USE_ADDITIONALLIGHT_VS
    #elif LIL_ADDITIONAL_LIGHT_MODE == 2
        #define LIL_USE_ADDITIONALLIGHT_PS
    #elif LIL_ADDITIONAL_LIGHT_MODE == 3
        #define LIL_USE_ADDITIONALLIGHT_MAIN
    #elif LIL_ADDITIONAL_LIGHT_MODE == 4
        #define LIL_USE_ADDITIONALLIGHT_MAINDIR
    #elif LIL_ADDITIONAL_LIGHT_MODE == 5
        #define LIL_USE_ADDITIONALLIGHT_MAINDIR_PS
    #endif
#endif

// Lightmap
#if defined(LIGHTMAP_ON)
    #define LIL_USE_LIGHTMAP
#endif
#if defined(DYNAMICLIGHTMAP_ON) && !(defined(LIL_URP) && LIL_SRP_VERSION_LOWER(12, 0))
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
#if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND) || defined(LIL_FEATURE_ANISOTROPY) || defined(LIL_FEATURE_MatCapBumpMap) || defined(LIL_FEATURE_MatCap2ndBumpMap) || defined(LIL_FEATURE_EMISSION_1ST) || defined(LIL_FEATURE_EMISSION_2ND) || defined(LIL_FEATURE_PARALLAX)
    #define LIL_SHOULD_TBN
#endif

// tangentOS (vertex input)
#if defined(LIL_SHOULD_TBN) || (defined(LIL_FEATURE_MAIN2ND) || defined(LIL_FEATURE_MAIN3RD)) && defined(LIL_FEATURE_DECAL)
    #define LIL_SHOULD_TANGENT
#endif

// normalOS (vertex input)
#if defined(LIL_SHOULD_TANGENT) || defined(LIL_FEATURE_SHADOW) || defined(LIL_FEATURE_RIMSHADE) || defined(LIL_FEATURE_REFLECTION) || defined(LIL_FEATURE_MATCAP) || defined(LIL_FEATURE_MATCAP_2ND) || defined(LIL_FEATURE_RIMLIGHT) || defined(LIL_FEATURE_GLITTER) || defined(LIL_FEATURE_BACKLIGHT) || defined(LIL_FEATURE_DISTANCE_FADE) || defined(LIL_FEATURE_AUDIOLINK) || defined(LIL_REFRACTION) || (defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SUBTRACTIVE)) || defined(LIL_HDRP)
    #define LIL_SHOULD_NORMAL
#endif

// positionOS
#if (defined(LIL_FEATURE_MAIN2ND) || defined(LIL_FEATURE_MAIN3RD)) && defined(LIL_FEATURE_LAYER_DISSOLVE) || defined(LIL_FEATURE_GLITTER) || defined(LIL_FEATURE_DISSOLVE) || defined(LIL_FEATURE_AUDIOLINK)
    #define LIL_SHOULD_POSITION_OS
#endif

// positionWS
#if defined(SHADOWS_SCREEN) || defined(LIL_PASS_FORWARDADD) || defined(LIL_FEATURE_MAIN2ND) || defined(LIL_FEATURE_MAIN3RD) || defined(LIL_FEATURE_ANISOTROPY) || defined(LIL_FEATURE_RECEIVE_SHADOW) || defined(LIL_FEATURE_RIMSHADE) || defined(LIL_FEATURE_REFLECTION) || defined(LIL_FEATURE_MATCAP) || defined(LIL_FEATURE_MATCAP_2ND) || defined(LIL_FEATURE_RIMLIGHT) || defined(LIL_FEATURE_GLITTER) || defined(LIL_FEATURE_BACKLIGHT) || defined(LIL_FEATURE_EMISSION_1ST) || defined(LIL_FEATURE_EMISSION_2ND) || defined(LIL_FEATURE_PARALLAX) || defined(LIL_FEATURE_DISTANCE_FADE) || defined(LIL_REFRACTION) || !defined(LIL_BRP) || defined(LIL_USE_LPPV)
    #define LIL_SHOULD_POSITION_WS
#endif

// uv1
#if defined(LIL_FEATURE_MATCAP) || defined(LIL_FEATURE_MATCAP_2ND) || defined(LIL_FEATURE_GLITTER)
    #define LIL_SHOULD_UV1
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Screen params
#if defined(LIL_URP)
    #define LIL_SCREENPARAMS    _ScaledScreenParams
#else
    #define LIL_SCREENPARAMS    _ScreenParams
#endif

//------------------------------------------------------------------------------------------------------------------------------
// API Macro
#if defined(TEXTURE2D)
    #undef TEXTURE2D
#endif
#if defined(TEXTURE2D_FLOAT)
    #undef TEXTURE2D_FLOAT
#endif
#if defined(TEXTURE2D_ARRAY)
    #undef TEXTURE2D_ARRAY
#endif
#if defined(TEXTURE3D)
    #undef TEXTURE3D
#endif
#if defined(TEXTURECUBE)
    #undef TEXTURECUBE
#endif
#if defined(SAMPLER)
    #undef SAMPLER
#endif

#if defined(SHADER_API_D3D11_9X)
    #define LIL_NOPERSPECTIVE
    #define LIL_CENTROID
#else
    #define LIL_NOPERSPECTIVE noperspective
    #define LIL_CENTROID centroid
#endif
#define LIL_VECTOR_INTERPOLATION

#if defined(SHADER_API_D3D9)
    #undef LIL_ANTIALIAS_MODE
    #define LIL_ANTIALIAS_MODE 0
#endif

#if defined(SHADER_API_D3D11_9X)
    #define LIL_VFACE(facing)
    #define LIL_COPY_VFACE(o)
    #undef LIL_USE_LIGHTMAP
#elif defined(SHADER_API_GLCORE) || defined(SHADER_API_GLES) || defined(SHADER_API_D3D9)
    #define LIL_VFACE(facing) , float facing : VFACE
    #define LIL_COPY_VFACE(o) o = facing
#else
    #define LIL_VFACE(facing) , bool isFrontFace : SV_IsFrontFace
    #define LIL_COPY_VFACE(o) o = isFrontFace ? 1 : -1
#endif

#if defined(SHADER_API_MOBILE) || defined(SHADER_API_GLES)
    #define LIL_NOT_SUPPORT_VERTEXID
#endif

#if defined(SHADER_API_D3D9) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER)) || defined(SHADER_TARGET_SURFACE_ANALYSIS)
    #define LIL_SAMPLE_1D(tex,samp,uv)                      tex2D(tex,float2(uv,0.5))
    #define LIL_SAMPLE_1D_LOD(tex,samp,uv,lod)              tex2Dlod(tex,float4(uv,0.5,0,lod))
    #define LIL_SAMPLE_2D(tex,samp,uv)                      tex2D(tex,uv)
    #define LIL_SAMPLE_2D_CS(tex,uv)                        tex2D(tex,uv/LIL_SCREENPARAMS.xy)
    #define LIL_SAMPLE_2D_ST(tex,samp,uv)                   tex2D(tex,uv*tex##_ST.xy+tex##_ST.zw)
    #define LIL_SAMPLE_2D_LOD(tex,samp,uv,lod)              tex2Dlod(tex,float4(uv,0,lod))
    #define LIL_SAMPLE_2D_BIAS(tex,samp,uv,bias)            tex2Dbias(tex,float4(uv,0,bias))
    #define LIL_SAMPLE_2D_GRAD(tex,samp,uv,dx,dy)           tex2Dgrad(tex,uv,dx,dy)
    #define LIL_SAMPLE_2D_ARRAY(tex,samp,uv,index)          tex2DArray(tex,float3(uv,index))
    #define LIL_SAMPLE_2D_ARRAY_CS(tex,uv,index)            tex2DArray(tex,float3(uv/LIL_SCREENPARAMS.xy,index))
    #define LIL_SAMPLE_2D_ARRAY_LOD(tex,samp,uv,index,lod)  tex2DArraylod(tex,float4(uv,index,lod))
    #define LIL_SAMPLE_3D(tex,samp,uv)                      tex3D(tex,uv)
    #define LIL_SAMPLE_CUBE_LOD(tex,samp,uv,lod)            texCUBElod(tex,float4(uv,0,lod))
    #define TEXTURE2D(tex)                                  sampler2D tex
    #define TEXTURE2D_FLOAT(tex)                            sampler2D tex
    #define TEXTURE2D_ARRAY(tex)                            sampler2DArray tex
    #define TEXTURE3D(tex)                                  sampler3D tex
    #define TEXTURECUBE(tex)                                samplerCUBE tex
    #define SAMPLER(samp)
    #define LIL_SAMP_IN_FUNC(samp)
    #define LIL_SAMP_IN(samp)
    #define LIL_LWTEX

    bool IsEmpty(TEXTURECUBE(tex))
    {
        return false;
    }

    bool IsEmpty(TEXTURE2D(tex))
    {
        return false;
    }

    bool IsEmpty(TEXTURE2D_ARRAY(tex))
    {
        return false;
    }

    bool IsScreenTex(TEXTURE2D(tex))
    {
        return false;
    }

    bool IsScreenTex(TEXTURE2D_ARRAY(tex))
    {
        return false;
    }

    float lilSampleDither(TEXTURE3D(tex), float2 positionCS, float alpha)
    {
        return tex3D(tex, float3(positionCS*0.25,alpha*0.9375)).a;
    }

    float4 lilSamplePointRepeat(TEXTURE2D(tex), float2 positionCS, float2 size)
    {
        uint2 uv = (uint2)positionCS.xy%(uint2)size;
        return tex2D(tex, uv/size);
    }

    float2 lilGetWidthAndHeight(TEXTURE2D(tex))
    {
        return float2(0, 0);
    }

    float2 lilGetWidthAndHeight(TEXTURE2D_ARRAY(tex))
    {
        return float2(0, 0);
    }
#else
    #define LIL_SAMPLE_1D(tex,samp,uv)                      tex.Sample(samp,uv)
    #define LIL_SAMPLE_1D_LOD(tex,samp,uv,lod)              tex.SampleLevel(samp,uv,lod)
    #define LIL_SAMPLE_2D(tex,samp,uv)                      tex.Sample(samp,uv)
    #define LIL_SAMPLE_2D_CS(tex,uv)                        tex[uint2(uv)]
    #define LIL_SAMPLE_2D_ST(tex,samp,uv)                   tex.Sample(samp,uv*tex##_ST.xy+tex##_ST.zw)
    #define LIL_SAMPLE_2D_LOD(tex,samp,uv,lod)              tex.SampleLevel(samp,uv,lod)
    #define LIL_SAMPLE_2D_BIAS(tex,samp,uv,bias)            tex.SampleBias(samp,uv,bias)
    #define LIL_SAMPLE_2D_GRAD(tex,samp,uv,dx,dy)           tex.SampleGrad(samp,uv,dx,dy)
    #define LIL_SAMPLE_2D_ARRAY(tex,samp,uv,index)          tex.Sample(samp,float3(uv,index))
    #define LIL_SAMPLE_2D_ARRAY_CS(tex,uv,index)            tex[uint3(uv,index)]
    #define LIL_SAMPLE_2D_ARRAY_LOD(tex,samp,uv,index,lod)  tex.SampleLevel(samp,float3(uv,index),lod)
    #define LIL_SAMPLE_3D(tex,samp,coord)                   tex.Sample(samp,coord)
    #define LIL_SAMPLE_CUBE_LOD(tex,samp,uv,lod)            tex.SampleLevel(samp,uv,lod)
    #define TEXTURE2D(tex)                                  Texture2D tex
    #define TEXTURE2D_FLOAT(tex)                            Texture2D<float4> tex
    #define TEXTURE2D_ARRAY(tex)                            Texture2DArray tex
    #define TEXTURE3D(tex)                                  Texture3D tex
    #define TEXTURECUBE(tex)                                TextureCube tex
    #define SAMPLER(samp)                                   SamplerState samp
    #define LIL_SAMP_IN_FUNC(samp)                          , SamplerState samp
    #define LIL_SAMP_IN(samp)                               , samp

    bool IsEmpty(TEXTURECUBE(tex))
    {
        uint width, height, levels;
        tex.GetDimensions(0, width, height, levels);
        return width < 15;
    }

    bool IsEmpty(TEXTURE2D(tex))
    {
        uint width, height;
        tex.GetDimensions(width, height);
        return width < 15;
    }

    bool IsEmpty(TEXTURE2D_ARRAY(tex))
    {
        uint width, height, element;
        tex.GetDimensions(width, height, element);
        return width < 15;
    }

    bool IsScreenTex(TEXTURE2D(tex))
    {
        uint width, height;
        tex.GetDimensions(width, height);
        return (abs(width - LIL_SCREENPARAMS.x) + abs(height - LIL_SCREENPARAMS.y)) < 1;
    }

    bool IsScreenTex(TEXTURE2D_ARRAY(tex))
    {
        uint width, height, element;
        tex.GetDimensions(width, height, element);
        return (abs(width - LIL_SCREENPARAMS.x) + abs(height - LIL_SCREENPARAMS.y)) < 1;
    }

    float lilSampleDither(TEXTURE3D(tex), float2 positionCS, float alpha)
    {
        uint3 uv = uint3(positionCS, alpha*0.9375*16);
        uv.xy = uv.xy % 4;
        return tex[uv].a;
    }

    float4 lilSamplePointRepeat(TEXTURE2D(tex), float2 positionCS, float2 size)
    {
        uint2 uv = (uint2)positionCS.xy%(uint2)size;
        return tex[uv];
    }

    float2 lilGetWidthAndHeight(TEXTURE2D(tex))
    {
        uint width, height;
        tex.GetDimensions(width, height);
        return float2(width, height);
    }

    float2 lilGetWidthAndHeight(TEXTURE2D_ARRAY(tex))
    {
        uint width, height, element;
        tex.GetDimensions(width, height, element);
        return float2(width, height);
    }
#endif

#if defined(LIL_FEATURE_PARALLAX) && defined(LIL_FEATURE_POM)
    #define LIL_SAMPLE_2D_POM(tex,samp,uv,dx,dy)    LIL_SAMPLE_2D_GRAD(tex,samp,uv,dx,dy)
#else
    #define LIL_SAMPLE_2D_POM(tex,samp,uv,dx,dy)    LIL_SAMPLE_2D(tex,samp,uv)
#endif

#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
    #define TEXTURE2D_SCREEN(tex)                   TEXTURE2D_ARRAY(tex)
    #define LIL_SAMPLE_SCREEN(tex,samp,uv)          LIL_SAMPLE_2D_ARRAY(tex,samp,uv,(float)unity_StereoEyeIndex)
    #define LIL_SAMPLE_SCREEN_LOD(tex,samp,uv,lod)  LIL_SAMPLE_2D_ARRAY_LOD(tex,samp,uv,(float)unity_StereoEyeIndex,lod)
    #define LIL_SAMPLE_SCREEN_CS(tex,uv)            LIL_SAMPLE_2D_ARRAY_CS(tex,uv,unity_StereoEyeIndex)

#else
    #define TEXTURE2D_SCREEN(tex)                   TEXTURE2D(tex)
    #define LIL_SAMPLE_SCREEN(tex,samp,uv)          LIL_SAMPLE_2D(tex,samp,uv)
    #define LIL_SAMPLE_SCREEN_LOD(tex,samp,uv,lod)  LIL_SAMPLE_2D_LOD(tex,samp,uv,lod)
    #define LIL_SAMPLE_SCREEN_CS(tex,uv)            LIL_SAMPLE_2D_CS(tex,uv)
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Macro to absorb pipeline differences

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
        return mul(LIL_MATRIX_M, float4(positionOS, 1.0)).xyz;
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

    float4 lilTransformCStoSSFrag(float4 positionCS)
    {
        float4 positionSS = float4(positionCS.xyz * positionCS.w, positionCS.w);
        positionSS.xy = positionSS.xy / LIL_SCREENPARAMS.xy;
        return positionSS;
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
        return mul(LIL_MATRIX_M, float4(positionOS, 1.0)).xyz;
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

    float4 lilTransformCStoSSFrag(float4 positionCS)
    {
        float4 positionSS = float4(positionCS.xyz * positionCS.w, positionCS.w);
        positionSS.xy = positionSS.xy / LIL_SCREENPARAMS.xy;
        return positionSS;
    }
#endif

// Stereo
#if defined(LIL_HDRP)
    #if LIL_SRP_VERSION_GREATER_EQUAL(7, 1)
        #define LIL_STEREO_MATRIX_V(i)     _XRViewMatrix[i]
        #define LIL_STEREO_CAMERA_POS(i)   _XRWorldSpaceCameraPos[i]
    #else
        #define LIL_STEREO_MATRIX_V(i)     _XRViewConstants[i].viewMatrix
        #define LIL_STEREO_CAMERA_POS(i)   _XRViewConstants[i].worldSpaceCameraPos
    #endif
#else
    #define LIL_STEREO_MATRIX_V(i)     unity_StereoMatrixV[i]
    #define LIL_STEREO_CAMERA_POS(i)   unity_StereoWorldSpaceCameraPos[i]
#endif

float3 lilToAbsolutePositionWS(float3 positionRWS)
{
    #if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
        return positionRWS + _WorldSpaceCameraPos.xyz;
    #else
        return positionRWS;
    #endif
}

float3 lilToRelativePositionWS(float3 positionWS)
{
    #if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
        return positionWS - _WorldSpaceCameraPos.xyz;
    #else
        return positionWS;
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

bool lilIsPerspective()
{
    #if defined(LIL_HDRP) && defined(SHADERPASS) && (SHADERPASS == SHADERPASS_SHADOWS)
        return LIL_MATRIX_P._m33 == 0;
    #else
        return unity_OrthoParams.w == 0;
    #endif
}

float3 lilViewDirection(float3 positionWS)
{
    return _WorldSpaceCameraPos.xyz - positionWS;
}

float3 lilHeadDirection(float3 positionWS)
{
    #if defined(USING_STEREO_MATRICES)
        return (LIL_STEREO_CAMERA_POS(0).xyz + LIL_STEREO_CAMERA_POS(1).xyz) * 0.5 - positionWS;
    #else
        return lilViewDirection(positionWS);
    #endif
}

float3 lilCameraDirection()
{
    #if defined(USING_STEREO_MATRICES)
        return normalize(LIL_STEREO_MATRIX_V(0)._m20_m21_m22 + LIL_STEREO_MATRIX_V(1)._m20_m21_m22);
    #else
        return LIL_MATRIX_V._m20_m21_m22;
    #endif
}

float3 lilCameraUp()
{
    return LIL_MATRIX_V._m10_m11_m12;
}

float3 lilCameraRight()
{
    #if defined(USING_STEREO_MATRICES)
        return cross(lilCameraDirection(), lilCameraUp());
    #else
        return LIL_MATRIX_V._m00_m01_m02;
    #endif
}

float3 lilViewDirectionOS(float3 positionOS)
{
    #if defined(LIL_HDRP)
        return lilTransformWStoOS(lilToRelativePositionWS(_WorldSpaceCameraPos.xyz)) - positionOS;
    #else
        return lilTransformWStoOS(_WorldSpaceCameraPos.xyz) - positionOS;
    #endif
}

float3 lilHeadDirectionOS(float3 positionOS)
{
    #if defined(USING_STEREO_MATRICES)
        return lilTransformWStoOS((LIL_STEREO_CAMERA_POS(0).xyz + LIL_STEREO_CAMERA_POS(1).xyz) * 0.5) - positionOS;
    #else
        return lilViewDirectionOS(positionOS);
    #endif
}

float2 lilCStoGrabUV(float4 positionCS)
{
    float2 uvScn = positionCS.xy / LIL_SCREENPARAMS.xy;
    #if defined(UNITY_SINGLE_PASS_STEREO)
        uvScn.xy = TransformStereoScreenSpaceTex(uvScn.xy, 1.0);
    #endif
    return uvScn;
}

float3 lilTransformDirWStoVSCenter(float3 directionWS, bool doNormalize)
{
    #if defined(USING_STEREO_MATRICES)
        if(doNormalize) return normalize(mul((float3x3)LIL_STEREO_MATRIX_V(0), directionWS) + mul((float3x3)LIL_STEREO_MATRIX_V(1), directionWS));
        else            return mul((float3x3)LIL_STEREO_MATRIX_V(0), directionWS) + mul((float3x3)LIL_STEREO_MATRIX_V(1), directionWS);
    #else
        if(doNormalize) return normalize(mul((float3x3)LIL_MATRIX_V, directionWS));
        else            return mul((float3x3)LIL_MATRIX_V, directionWS);
    #endif
}

float3 lilTransformDirWStoVSCenter(float3 directionWS)
{
    return lilTransformDirWStoVSCenter(directionWS, false);
}

float3 lilBlendVRParallax(float3 a, float3 b, float c)
{
    #if defined(USING_STEREO_MATRICES)
        return lerp(a, b, c);
    #else
        return b;
    #endif
}

float lilLinearEyeDepth(float z)
{
    //return LIL_MATRIX_P._m23 / (z - LIL_MATRIX_P._m22 / LIL_MATRIX_P._m32);
    return LIL_MATRIX_P._m23 / (z + LIL_MATRIX_P._m22);
}

float lilLinearEyeDepth(float z, float2 positionCS)
{
    float2 pos = positionCS / LIL_SCREENPARAMS.xy * 2.0 - 1.0;
    #if UNITY_UV_STARTS_AT_TOP
        pos.y = -pos.y;
    #endif
    return LIL_MATRIX_P._m23 / (z + LIL_MATRIX_P._m22
        - LIL_MATRIX_P._m20 / LIL_MATRIX_P._m00 * (pos.x +LIL_MATRIX_P._m02)
        - LIL_MATRIX_P._m21 / LIL_MATRIX_P._m11 * (pos.y +LIL_MATRIX_P._m12)
    );
}

float2 lilCameraDepthTexel(float2 positionCS)
{
    float2 uv = positionCS.xy;
    #if UNITY_UV_STARTS_AT_TOP
        if(_ProjectionParams.x > 0) uv.y = LIL_SCREENPARAMS.y - uv.y;
    #else
        if(_ProjectionParams.x < 0) uv.y = LIL_SCREENPARAMS.y - uv.y;
    #endif
    return uv;
}

float3 lilGetObjectPosition()
{
    return lilTransformOStoWS(float3(0,0,0));
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
    #define LIL_GET_HDRPDATA(input,fd)
    #define LIL_HDRP_DEEXPOSURE(col)
    #define LIL_HDRP_INVDEEXPOSURE(col)

    // Main light
    #define LIL_MAINLIGHT_COLOR                         _LightColor0.rgb
    #define LIL_MAINLIGHT_DIRECTION                     _WorldSpaceLightPos0.xyz

    // Shadow
    #if defined(LIL_USE_SHADOW) && !defined(LIL_PASS_FORWARDADD)
        #define LIL_SHADOW_COORDS(idx)                  UNITY_SHADOW_COORDS(idx)
        #define LIL_TRANSFER_SHADOW(vi,uv,o) \
            DummyStructure v; \
            v.vertex = input.positionOS; \
            BRPShadowCoords brpShadowCoords; \
            LIL_INITIALIZE_STRUCT(BRPShadowCoords, brpShadowCoords); \
            brpShadowCoords.pos = vi.positionCS; \
            UNITY_TRANSFER_LIGHTING(brpShadowCoords, uv) \
            o._ShadowCoord = brpShadowCoords._ShadowCoord
        #define LIL_LIGHT_ATTENUATION(atten,i) \
            BRPShadowCoords brpShadowCoords; \
            brpShadowCoords.pos = i.positionCS; \
            brpShadowCoords._ShadowCoord = i._ShadowCoord; \
            UNITY_LIGHT_ATTENUATION(attenuationOrig, brpShadowCoords, i.positionWS); \
            atten = attenuationOrig
    #elif (UNITY_VERSION < 201820) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS) || !defined(LIL_PASS_FORWARDADD)
        #define LIL_SHADOW_COORDS(idx)
        #define LIL_TRANSFER_SHADOW(vi,uv,o)
        #define LIL_LIGHT_ATTENUATION(atten,i)
    #else
        #if defined(POINT)
            #define LIL_CALC_LIGHT_COORDS(o,i) o._LightCoord = mul(unity_WorldToLight, float4(i.positionWS, 1.0)).xyz;
        #elif defined(SPOT)
            #define LIL_CALC_LIGHT_COORDS(o,i) o._LightCoord = mul(unity_WorldToLight, float4(i.positionWS, 1.0));
        #elif defined(POINT_COOKIE)
            #define LIL_CALC_LIGHT_COORDS(o,i) o._LightCoord = mul(unity_WorldToLight, float4(i.positionWS, 1.0)).xyz;
        #elif defined(DIRECTIONAL_COOKIE)
            #define LIL_CALC_LIGHT_COORDS(o,i) o._LightCoord = mul(unity_WorldToLight, float4(i.positionWS, 1.0)).xy;
        #else
            #define LIL_CALC_LIGHT_COORDS(o,i)
        #endif

        #define LIL_SHADOW_COORDS(idx) UNITY_SHADOW_COORDS(idx)
        #if defined(SHADOWS_DEPTH) && defined(SPOT) || defined(SHADOWS_SCREEN) && defined(UNITY_NO_SCREENSPACE_SHADOWS)
            #define LIL_TRANSFER_SHADOW(vi,uv,o) o._ShadowCoord = mul(unity_WorldToShadow[0], float4(vi.positionWS.xyz, 1));
        #elif defined(SHADOWS_CUBE)
            #define LIL_TRANSFER_SHADOW(vi,uv,o) o._ShadowCoord.xyz = fd.positionWS.xyz - _LightPositionRange.xyz;
        #elif defined(SHADOWS_SCREEN)
            #define LIL_TRANSFER_SHADOW(vi,uv,o) o._ShadowCoord = float4(vi.positionCS.xy / LIL_SCREENPARAMS.xy, 1, 1);
        #else
            #define LIL_TRANSFER_SHADOW(vi,uv,o)
        #endif

        #define LIL_LIGHT_ATTENUATION(atten,i) \
            BRPShadowCoords brpShadowCoords; \
            brpShadowCoords.pos = i.positionCS; \
            LIL_CALC_LIGHT_COORDS(brpShadowCoords,i) \
            LIL_TRANSFER_SHADOW(fd,fd.uv1,brpShadowCoords) \
            UNITY_LIGHT_ATTENUATION(attenuationOrig, brpShadowCoords, i.positionWS); \
            atten = attenuationOrig
    #endif
    struct BRPShadowCoords
    {
        float4 pos;
        LIL_SHADOW_COORDS(0)
        #if defined(DECLARE_LIGHT_COORDS)
            DECLARE_LIGHT_COORDS(1)
        #elif defined(POINT)
            unityShadowCoord3 _LightCoord : TEXCOORD1;
        #elif defined(SPOT)
            unityShadowCoord4 _LightCoord : TEXCOORD1;
        #elif defined(POINT_COOKIE)
            unityShadowCoord3 _LightCoord : TEXCOORD1;
        #elif defined(DIRECTIONAL_COOKIE)
            unityShadowCoord2 _LightCoord : TEXCOORD1;
        #endif
    };
    struct DummyStructure
    {
        float4 vertex;
    };


    // Shadow caster
    #define LIL_V2F_SHADOW_CASTER_OUTPUT            V2F_SHADOW_CASTER_NOPOS float4 positionCS : SV_POSITION;
    #if defined(SHADOWS_CUBE) && !defined(SHADOWS_CUBE_IN_DEPTH_TEX)
        #define LIL_TRANSFER_SHADOW_CASTER(v,o) \
            o.vec = mul(unity_ObjectToWorld, v.positionOS).xyz - _LightPositionRange.xyz; \
            o.positionCS = UnityObjectToClipPos(v.positionOS)
    #else
        float4 lilClipSpaceShadowCasterPos(float4 positionOS, float3 normalOS, float bias)
        {
            #if defined(SHADOWS_DEPTH)
                if(LIL_MATRIX_P._m33 == 0.0) bias = 0;
            #endif
            float4 positionWS = mul(unity_ObjectToWorld, float4(positionOS.xyz, 1));
            float3 L = normalize(UnityWorldSpaceLightDir(positionWS.xyz));
            positionWS.xyz -= L * bias;

            if(unity_LightShadowBias.z != 0.0)
            {
                float3 normalWS = UnityObjectToWorldNormal(normalOS);
                float shadowCos = dot(normalWS, L);
                float shadowSine = sqrt(1-shadowCos*shadowCos);
                float normalBias = unity_LightShadowBias.z * shadowSine;

                positionWS.xyz -= normalWS * normalBias;
            }

            return mul(UNITY_MATRIX_VP, positionWS);
        }
        #define LIL_TRANSFER_SHADOW_CASTER(v,o) \
            o.positionCS = lilClipSpaceShadowCasterPos(v.positionOS, v.normalOS, _lilShadowCasterBias); \
            o.positionCS = UnityApplyLinearShadowBias(o.positionCS)
    #endif
    #define LIL_SHADOW_CASTER_FRAGMENT(i)           SHADOW_CASTER_FRAGMENT(i)

    // Additional Light
    void lilGetAdditionalLights(float3 positionWS, float4 positionCS, float strength, inout float3 lightColor, inout float3 lightDirection)
    {
        #if defined(LIGHTPROBE_SH) && defined(VERTEXLIGHT_ON)
        float4 toLightX = unity_4LightPosX0 - positionWS.x;
        float4 toLightY = unity_4LightPosY0 - positionWS.y;
        float4 toLightZ = unity_4LightPosZ0 - positionWS.z;

        float4 lengthSq = toLightX * toLightX + 0.000001;
        lengthSq += toLightY * toLightY;
        lengthSq += toLightZ * toLightZ;

        //float4 atten = 1.0 / (1.0 + lengthSq * unity_4LightAtten0);
        float4 atten = saturate(saturate((25.0 - lengthSq * unity_4LightAtten0) * 0.111375) / (0.987725 + lengthSq * unity_4LightAtten0)) * strength;

        lightColor += unity_LightColor[0].rgb * atten.x;
        lightColor += unity_LightColor[1].rgb * atten.y;
        lightColor += unity_LightColor[2].rgb * atten.z;
        lightColor += unity_LightColor[3].rgb * atten.w;

        lightDirection += lilLuminance(unity_LightColor[0].rgb) * atten.x / sqrt(lengthSq.x) * float3(toLightX.x, toLightY.x, toLightZ.x);
        lightDirection += lilLuminance(unity_LightColor[1].rgb) * atten.y / sqrt(lengthSq.y) * float3(toLightX.y, toLightY.y, toLightZ.y);
        lightDirection += lilLuminance(unity_LightColor[2].rgb) * atten.z / sqrt(lengthSq.z) * float3(toLightX.z, toLightY.z, toLightZ.z);
        lightDirection += lilLuminance(unity_LightColor[3].rgb) * atten.w / sqrt(lengthSq.w) * float3(toLightX.w, toLightY.w, toLightZ.w);
        #endif
    }

    float3 lilGetAdditionalLights(float3 positionWS, float4 positionCS, float strength)
    {
        float3 lightColor = 0.0;
        float3 lightDirection = 0.0;
        lilGetAdditionalLights(positionWS, positionCS, strength, lightColor, lightDirection);
        return saturate(lightColor);
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
        ((IsEmpty(unity_SpecCube0) || unity_SpecCube0_HDR.x == 0 || _ReflectionCubeOverride) ? \
        lilCustomReflection(_ReflectionCubeTex, _ReflectionCubeTex_HDR, viewDirection, normalDirection, perceptualRoughness) * _ReflectionCubeColor.rgb * lerp(1.0, fd.lightColor, _ReflectionCubeEnableLighting) : \
        lilGetEnvReflection(viewDirection,normalDirection,perceptualRoughness,positionWS))

    // Fog
    #if defined(LIL_PASS_FORWARDADD)
        #define LIL_FOG_COLOR   float4(0,0,0,0)
    #else
        #define LIL_FOG_COLOR   unity_FogColor
    #endif
    #if LIL_RENDER == 2
        #define LIL_APPLY_FOG_BASE(col,fogCoord)                 UNITY_FOG_LERP_COLOR(col,LIL_FOG_COLOR*col.a,fogCoord)
        #define LIL_APPLY_FOG_COLOR_BASE(col,fogCoord,fogColor)  UNITY_FOG_LERP_COLOR(col,fogColor*col.a,fogCoord)
    #else
        #define LIL_APPLY_FOG_BASE(col,fogCoord)                 UNITY_FOG_LERP_COLOR(col,LIL_FOG_COLOR,fogCoord)
        #define LIL_APPLY_FOG_COLOR_BASE(col,fogCoord,fogColor)  UNITY_FOG_LERP_COLOR(col,fogColor,fogCoord)
    #endif
    float lilCalcFogFactor(float depth)
    {
        #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
            UNITY_CALC_FOG_FACTOR(depth);
            return unityFogFactor;
        #else
            return 1.0;
        #endif
    }

    // Meta
    #define MetaInput                               UnityMetaInput
    #define MetaFragment(input)                     UnityMetaFragment(input)
    #define MetaVertexPosition(pos,uv1,uv2,l,d)     UnityMetaVertexPosition(pos,uv1,uv2,l,d)
#elif defined(LIL_HDRP)
    #define LIGHT_SIMULATE_HQ

    // Support for old version
    #if LIL_SRP_VERSION_LOWER(4, 1)
        #define LIL_HDRP_IGNORE_LIGHTDIMMER
        float4 EvaluateAtmosphericScattering(PositionInputs posInput, float3 viewDirection, float4 col)
        {
            return EvaluateAtmosphericScattering(posInput, col);
        }
    #endif

    #if LIL_SRP_VERSION_LOWER(4, 2)
        float GetDirectionalShadowAttenuation(HDShadowContext shadowContext, float2 positionSS, float3 positionWS, float3 normalWS, int shadowIndex, float3 L)
        {
            return GetDirectionalShadowAttenuation(shadowContext, positionWS, normalWS, shadowIndex, L, positionSS);
        }
    #endif

    #if LIL_SRP_VERSION_LOWER(5, 3)
        float GetCurrentExposureMultiplier()
        {
            return 1.0;
        }
    #endif

    #if LIL_SRP_VERSION_LOWER(6, 6)
        float3 SampleCameraColor(float2 uv, float lod)
        {
            return LIL_SAMPLE_2D_LOD(_ColorPyramidTexture, s_trilinear_clamp_sampler, uv, lod).rgb;
        }
    #endif

    #if LIL_SRP_VERSION_LOWER(6, 8)
        float4 EvaluateLight_Directional(LightLoopContext lightLoopContext, PositionInputs posInput, DirectionalLightData light)
        {
            float4 color = float4(light.color.rgb, 1.0);

            #if LIL_SRP_VERSION_GREATER_EQUAL(4, 1)
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

    #if LIL_SRP_VERSION_LOWER(7, 1)
        float3 TransformPreviousObjectToWorld(float3 previousPositionOS)
        {
            float4x4 previousModelMatrix = ApplyCameraTranslationToMatrix(unity_MatrixPreviousM);
            return mul(previousModelMatrix, float4(previousPositionOS, 1.0)).xyz;
        }
    #endif

    float3 lilSelectPreviousPosition(float3 previousPositionOS, float3 positionOS)
    {
        return unity_MotionVectorsParams.x > 0 ? previousPositionOS : positionOS;
    }

    float3 lilTransformPreviousObjectToWorld(float3 previousPositionOS)
    {
        return TransformPreviousObjectToWorld(previousPositionOS);
    }

    float2 lilCalculateMotionVector(float4 positionCS, float4 previousPositionCS)
    {
        if(unity_MotionVectorsParams.y == 0) return float2(2.0, 0.0);

        positionCS.xy = positionCS.xy / LIL_SCREENPARAMS.xy * 2.0 - 1.0;
        #if UNITY_UV_STARTS_AT_TOP
            positionCS.y = -positionCS.y;
        #endif
        previousPositionCS.xy = previousPositionCS.xy / previousPositionCS.w;
        float2 motionVec = (positionCS.xy - previousPositionCS.xy);

        float2 microThreshold = 0.01f * _ScreenSize.zw;
        motionVec.x = abs(motionVec.x) < microThreshold.x ? 0 : motionVec.x;
        motionVec.y = abs(motionVec.y) < microThreshold.y ? 0 : motionVec.y;
        motionVec = clamp(motionVec, -1.0f + microThreshold, 1.0f - microThreshold);
        motionVec *= 0.5;

        #if UNITY_UV_STARTS_AT_TOP
            motionVec.y = -motionVec.y;
        #endif
        return motionVec;
    }

    #define LIL_MATRIX_PREV_VP UNITY_MATRIX_PREV_VP

    #if LIL_SRP_VERSION_LOWER(11, 0)
        #define LIL_HDRP_DEEXPOSURE(col)
        #define LIL_HDRP_INVDEEXPOSURE(col)
    #else
        #define LIL_HDRP_DEEXPOSURE(col)    col.rgb *= _DeExposureMultiplier
        #define LIL_HDRP_INVDEEXPOSURE(col) col.rgb /= _DeExposureMultiplier
    #endif

    float4 SampleEnv(LightLoopContext lightLoopContext, PositionInputs posInput, EnvLightData lightData, float3 reflUVW, float lod)
    {
        #if LIL_SRP_VERSION_GREATER_EQUAL(10, 1)
            float4 reflectionCol = SampleEnv(lightLoopContext, lightData.envIndex, reflUVW, lod * lightData.roughReflections, lightData.rangeCompressionFactorCompensation, posInput.positionNDC);
        #elif LIL_SRP_VERSION_GREATER_EQUAL(7, 1)
            float4 reflectionCol = SampleEnv(lightLoopContext, lightData.envIndex, reflUVW, lod, lightData.rangeCompressionFactorCompensation);
        #else
            float4 reflectionCol = SampleEnv(lightLoopContext, lightData.envIndex, reflUVW, lod);
        #endif
        LIL_HDRP_INVDEEXPOSURE(reflectionCol);
        return reflectionCol;
    }

    //------------------------------------------------------------------------------------------------------------------------------
    // HDRP Data
    uint lilGetRenderingLayer()
    {
        #if !defined(DEFAULT_LIGHT_LAYERS)
            return _EnableLightLayers ? asuint(unity_RenderingLayer.x) & RENDERING_LAYERS_MASK : RENDERING_LAYERS_MASK;
        #elif defined(RENDERING_LIGHT_LAYERS_MASK)
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
        #if LIL_SRP_VERSION_GREATER_EQUAL(6, 7)
            real contactShadowFade;
        #endif
        #if LIL_SRP_VERSION_GREATER_EQUAL(12, 1)
            real splineVisibility;
        #endif
        #if defined(APPLY_FOG_ON_SKY_REFLECTIONS)
            lightLoopContext.positionWS = 0;
        #endif
        return lightLoopContext;
    }

    #define LIL_GET_HDRPDATA(input,fd) \
        fd.renderingLayers = lilGetRenderingLayer(); \
        fd.featureFlags = LIGHT_FEATURE_MASK_FLAGS_OPAQUE; \
        fd.tileIndex = uint2(0,0); \
        PositionInputs posInput = GetPositionInput(input.positionCS.xy, _ScreenSize.zw, input.positionCS.z, input.positionCS.w, input.positionWS, fd.tileIndex)

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

    float lilGetDirectionalShadow(PositionInputs posInput, float3 normalWS, uint featureFlags, float bias)
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
                        float3 positionWS = posInput.positionWS + L * bias;
                        attenuation = GetDirectionalShadowAttenuation(shadowContext, posInput.positionSS, positionWS, normalWS, light.shadowIndex, L);
                    }
                }
            }
        }
        return attenuation;
    }

    float lilGetDirectionalShadow(PositionInputs posInput, float3 normalWS, uint featureFlags)
    {
        return lilGetDirectionalShadow(posInput, normalWS, featureFlags, 0.0);
    }

    lilNPRLightingData lilGetDirectionalLightSum(PositionInputs posInput, uint renderingLayers, uint featureFlags)
    {
        lilNPRLightingData lightingData;
        lightingData.color = 0.0;
        lightingData.direction = 0.0;
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

        lightingData.direction = dot(lightingData.direction,lightingData.direction) < 0.000001 ? 0 : normalize(lightingData.direction);

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
        float4 color = float4(light.color.rgb, 1.0);
        color.a *= PunctualLightAttenuation(distances, light.rangeAttenuationScale, light.rangeAttenuationBias, light.angleScale, light.angleOffset);

        #if !defined(LIGHT_EVALUATION_NO_HEIGHT_FOG) && LIL_SRP_VERSION_GREATER_EQUAL(4, 1)
            float cosZenithAngle = L.y;
            float distToLight = (light.lightType == GPULIGHTTYPE_PROJECTOR_BOX) ? distances.w : distances.x;
            float fragmentHeight = positionWS.y;
            color.a *= TransmittanceHeightFog(_HeightFogBaseExtinction, _HeightFogBaseHeight, _HeightFogExponents, cosZenithAngle, fragmentHeight, distToLight);
        #endif

        #if LIL_SRP_VERSION_LOWER(7, 2)
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

    void lilGetPunctualLightColor(inout lilNPRLightingData dst, float3 positionWS, uint renderingLayers, uint featureFlags)
    {
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
                        lilNPRLightingData src = lilGetNPRPunctualLight(positionWS, lightData);
                        lilBlendlilNPRLightingData(dst, src);
                    }
                }
            }
        }
    }

    //------------------------------------------------------------------------------------------------------------------------------
    // Area Light (Line / Rectangle)
    lilNPRLightingData lilGetLineLightColor(float3 positionWS, LightData lightData)
    {
        lilNPRLightingData lighting = (lilNPRLightingData)0;
        float3 unL = lightData.positionRWS - positionWS;
        float intensity = EllipsoidalDistanceAttenuation(
            unL,
            lightData.right,
            saturate(lightData.range / (lightData.range + 0.5 * lightData.size.x)),
            lightData.rangeAttenuationScale,
            lightData.rangeAttenuationBias);
            #if !defined(LIL_HDRP_IGNORE_LIGHTDIMMER)
                intensity *= lightData.diffuseDimmer;
            #endif
        lighting.color = lightData.color * intensity;

        float halfLength = 0.5 * lightData.size.x;
        //float3 nearestPoint = lightData.positionRWS - lightData.right * clamp(dot(unL, lightData.right), -halfLength, halfLength);
        float3 nearestPoint = lightData.positionRWS - lightData.right * clamp(dot(unL, lightData.right), -halfLength, halfLength) * 0.75;
        lighting.direction = normalize(nearestPoint - positionWS);

        return lighting;
    }

    lilNPRLightingData lilGetRectLightColor(float3 positionWS, LightData lightData)
    {
        lilNPRLightingData lighting = (lilNPRLightingData)0;
        float3 lightColor = 0.0;
        #if SHADEROPTIONS_BARN_DOOR
            RectangularLightApplyBarnDoor(lightData, positionWS);
        #endif
        float3 unL = lightData.positionRWS - positionWS;
        if(dot(lightData.forward, unL) < FLT_EPS)
        {
            float3x3 lightToWorld = float3x3(lightData.right, lightData.up, -lightData.forward);
            float3 unL2 = mul(unL, transpose(lightToWorld));
            float halfWidth  = lightData.size.x * 0.5;
            float halfHeight = lightData.size.y * 0.5;

            float3 invHalfDim = rcp(float3(lightData.range + halfWidth, lightData.range + halfHeight, lightData.range));
            #ifdef ELLIPSOIDAL_ATTENUATION
                float intensity = EllipsoidalDistanceAttenuation(unL2, invHalfDim, lightData.rangeAttenuationScale, lightData.rangeAttenuationBias);
            #else
                float intensity = BoxDistanceAttenuation(unL2, invHalfDim, lightData.rangeAttenuationScale, lightData.rangeAttenuationBias);
            #endif
            #if !defined(LIL_HDRP_IGNORE_LIGHTDIMMER)
                intensity *= lightData.diffuseDimmer;
            #endif
            lighting.color = lightData.color * intensity;

            float2 halfLength = 0.5 * lightData.size.xy;
            float3 nearestPoint = lightData.positionRWS
                - lightData.right * clamp(dot(unL, lightData.right), -halfLength.x, halfLength.x) * 0.75
                - lightData.up * clamp(dot(unL, lightData.up), -halfLength.y, halfLength.y) * 0.75;
            lighting.direction = normalize(nearestPoint - positionWS);
        }
        return lighting;
    }

    void lilGetAreaLightColor(inout lilNPRLightingData dst, float3 positionWS, uint renderingLayers, uint featureFlags)
    {
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
                            lilNPRLightingData lighting = lilGetLineLightColor(positionWS, lightData);
                            lilBlendlilNPRLightingData(dst, lighting);
                        }
                        lightData = FetchLight(_PunctualLightCount, min(++i, last));
                    }

                    while(i <= last)
                    {
                        lightData.lightType = GPULIGHTTYPE_RECTANGLE;
                        if((lightData.lightLayers & renderingLayers) != 0)
                        {
                            lilNPRLightingData lighting = lilGetRectLightColor(positionWS, lightData);
                            lilBlendlilNPRLightingData(dst, lighting);
                        }
                        lightData = FetchLight(_PunctualLightCount, min(++i, last));
                    }
                }
            }
        #endif
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
        #define LIL_LIGHT_ATTENUATION(atten,i)      atten = lilGetDirectionalShadow(posInput, i.normalWS, fd.featureFlags, _lilShadowCasterBias)
    #else
        #define LIL_LIGHT_ATTENUATION(atten,i)
    #endif

    // Shadow caster
    #define LIL_V2F_SHADOW_CASTER_OUTPUT
    #define LIL_TRANSFER_SHADOW_CASTER(v,o)
    #define LIL_SHADOW_CASTER_FRAGMENT(i)

    // Additional Light
    void lilGetAdditionalLights(float3 positionWS, float4 positionCS, float strength, inout float3 lightColor, inout float3 lightDirection)
    {
        uint renderingLayers = lilGetRenderingLayer();
        uint featureFlags = LIGHT_FEATURE_MASK_FLAGS_OPAQUE;

        lilNPRLightingData lighting;
        LIL_INITIALIZE_STRUCT(lilNPRLightingData, lighting);
        lilGetPunctualLightColor(lighting, positionWS, renderingLayers, featureFlags);
        lilGetAreaLightColor(lighting, positionWS, renderingLayers, featureFlags);
        lightColor += lighting.color * 0.75 * GetCurrentExposureMultiplier() * strength;
        lightDirection += lighting.direction * 0.75 * strength;
    }

    float3 lilGetAdditionalLights(float3 positionWS, float4 positionCS, float strength)
    {
        float3 lightColor = 0.0;
        float3 lightDirection = 0.0;
        lilGetAdditionalLights(positionWS, positionCS, strength, lightColor, lightDirection);
        return lightColor;
    }

    // Lightmap
    #define LIL_DECODE_LIGHTMAP(lm)                     DecodeLightmap(lm, float4(LIGHTMAP_HDR_MULTIPLIER,LIGHTMAP_HDR_EXPONENT,0.0,0.0))
    #define LIL_DECODE_DYNAMICLIGHTMAP(lm)              lm.rgb

    // Environment reflection
    #define LIL_GET_ENVIRONMENT_REFLECTION(viewDirection,normalDirection,perceptualRoughness,positionWS) \
        (_ReflectionCubeOverride ? \
        lilCustomReflection(_ReflectionCubeTex, _ReflectionCubeTex_HDR, viewDirection, normalDirection, perceptualRoughness) * _ReflectionCubeColor.rgb * lerp(1.0, fd.lightColor, _ReflectionCubeEnableLighting) : \
        lilGetReflectionSum(viewDirection,normalDirection,perceptualRoughness,posInput,fd.renderingLayers,fd.featureFlags))

    // Fog
    #define LIL_APPLY_FOG_BASE(col,fogCoord)                 col = EvaluateAtmosphericScattering(posInput, fd.V, col)
    #define LIL_APPLY_FOG_COLOR_BASE(col,fogCoord,fogColor)  col = EvaluateAtmosphericScattering(posInput, fd.V, col)
    float lilCalcFogFactor(float depth)
    {
        return 0.0;
    }
#else
    // Support for old version
    // HDRP Data
    #if LIL_SRP_VERSION_GREATER_EQUAL(14, 0)
        uint lilGetRenderingLayer()
        {
            return asuint(unity_RenderingLayer.x);
        }
    #elif LIL_SRP_VERSION_GREATER_EQUAL(12, 0)
        uint lilGetRenderingLayer()
        {
            #if defined(_LIGHT_LAYERS)
                return (asuint(unity_RenderingLayer.x) & RENDERING_LIGHT_LAYERS_MASK) >> RENDERING_LIGHT_LAYERS_MASK_SHIFT;
            #else
                return DEFAULT_LIGHT_LAYERS;
            #endif
        }
    #else
        uint lilGetRenderingLayer()
        {
            return 0;
        }
    #endif
    #define LIL_GET_HDRPDATA(input,fd)
    #define LIL_HDRP_DEEXPOSURE(col)
    #define LIL_HDRP_INVDEEXPOSURE(col)

    #if LIL_SRP_VERSION_GREATER_EQUAL(12, 0)
        #define LIL_MATRIX_PREV_VP _PrevViewProjMatrix
        float3 lilSelectPreviousPosition(float3 previousPositionOS, float3 positionOS)
        {
            return unity_MotionVectorsParams.x > 0 ? previousPositionOS : positionOS;
        }

        float3 lilTransformPreviousObjectToWorld(float3 previousPositionOS)
        {
            return mul(UNITY_PREV_MATRIX_M, float4(previousPositionOS,1)).xyz;
        }

        float2 lilCalculateMotionVector(float4 positionCS, float4 previousPositionCS)
        {
            if(unity_MotionVectorsParams.y == 0) return float2(0.0, 0.0);

            #if LIL_SRP_VERSION_GREATER_EQUAL(16, 0)
                positionCS.xy = positionCS.xy / positionCS.w;
            #else
                positionCS.xy = (positionCS.xy / LIL_SCREENPARAMS.xy - 0.5) * 2.0;

                #if UNITY_UV_STARTS_AT_TOP
                    positionCS.y = -positionCS.y;
                #endif
            #endif

            previousPositionCS.xy = previousPositionCS.xy / previousPositionCS.w;

            #if defined(_FOVEATED_RENDERING_NON_UNIFORM_RASTER)
                float2 posUV = RemapFoveatedRenderingResolve(positionCS.xy * 0.5 + 0.5);
                float2 prevPosUV = RemapFoveatedRenderingPrevFrameLinearToNonUniform(previousPositionCS.xy * 0.5 + 0.5);
                float2 motionVec = posUV - prevPosUV;
            #else
                float2 motionVec = (positionCS.xy - previousPositionCS.xy) * 0.5;
            #endif

            #if UNITY_UV_STARTS_AT_TOP
                motionVec.y = -motionVec.y;
            #endif
            return motionVec;
        }

        void lilApplyMotionVectorZBias(inout float4 positionCS)
        {
            #if defined(UNITY_REVERSED_Z)
                positionCS.z -= unity_MotionVectorsParams.z * positionCS.w;
            #else
                positionCS.z += unity_MotionVectorsParams.z * positionCS.w;
            #endif
        }
    #else
        #define LIL_MATRIX_PREV_VP LIL_MATRIX_VP
        float3 lilSelectPreviousPosition(float3 previousPositionOS, float3 positionOS)
        {
            return previousPositionOS;
        }

        float3 lilTransformPreviousObjectToWorld(float3 previousPositionOS)
        {
            return 0;
        }

        float2 lilCalculateMotionVector(float4 positionCS, float4 previousPositionCS)
        {
            return 0;
        }

        void lilMotionVectorOffsetCS(inout float4 positionCS)
        {
        }
    #endif

    // Main light
    #if LIL_SRP_VERSION_GREATER_EQUAL(12, 0) && defined(_LIGHT_LAYERS)
        #define LIL_MAINLIGHT_COLOR                         ((_MainLightLayerMask & lilGetRenderingLayer()) != 0 ? _MainLightColor.rgb : 0.0)
    #else
        #define LIL_MAINLIGHT_COLOR                         _MainLightColor.rgb
    #endif
    #define LIL_MAINLIGHT_DIRECTION                     _MainLightPosition.xyz

    // Shadow
    float4 GetShadowCoord(float3 positionWS, float4 positionCS)
    {
        VertexPositionInputs vertexInput = (VertexPositionInputs)0;
        vertexInput.positionWS = positionWS;
        vertexInput.positionCS = positionCS;
        return GetShadowCoord(vertexInput);
    }

    #if defined(LIL_USE_SHADOW)
        #if defined(_MAIN_LIGHT_SHADOWS_SCREEN)
            #define LIL_SHADOW_COORDS(idx)              float4 shadowCoord : TEXCOORD##idx;
            #define LIL_TRANSFER_SHADOW(vi,uv,o)        o.shadowCoord = GetShadowCoord(vi.positionWS, vi.positionCS);
            #define LIL_LIGHT_ATTENUATION(atten,i)      atten = MainLightRealtimeShadow(i.shadowCoord)
        #elif defined(_MAIN_LIGHT_SHADOWS_CASCADE) && !defined(_MAIN_LIGHT_SHADOWS)
            #define LIL_SHADOW_COORDS(idx)
            #define LIL_TRANSFER_SHADOW(vi,uv,o)
            #define LIL_LIGHT_ATTENUATION(atten,i)      atten = MainLightRealtimeShadow(TransformWorldToShadowCoord(i.positionWS))
        #else
            #define LIL_SHADOW_COORDS(idx)              float4 shadowCoord : TEXCOORD##idx;
            #define LIL_TRANSFER_SHADOW(vi,uv,o)        o.shadowCoord = GetShadowCoord(vi.positionWS, vi.positionCS);
            #define LIL_LIGHT_ATTENUATION(atten,i)      atten = MainLightRealtimeShadow(i.shadowCoord)
        #endif
    #else
        #define LIL_SHADOW_COORDS(idx)
        #define LIL_TRANSFER_SHADOW(vi,uv,o)
        #define LIL_LIGHT_ATTENUATION(atten,i)
    #endif

    // Shadow caster
    float3 _LightDirection;
    float3 _LightPosition;
    #if LIL_SRP_VERSION_LOWER(5, 1)
        float4 _ShadowBias;
    #endif
    float4 URPShadowPos(float4 positionOS, float3 normalOS, float bias)
    {
        float3 positionWS = TransformObjectToWorld(positionOS.xyz);
        float3 normalWS = TransformObjectToWorldNormal(normalOS);

        #if _CASTING_PUNCTUAL_LIGHT_SHADOW
            float3 lightDirectionWS = normalize(_LightPosition - positionWS);
        #else
            float3 lightDirectionWS = _LightDirection;
        #endif

        positionWS -= lightDirectionWS * bias;

        #if LIL_SRP_VERSION_GREATER_EQUAL(5, 1)
            float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));
        #else
            float biasN = _ShadowBias.y - saturate(dot(lightDirectionWS, normalWS)) * _ShadowBias.y;
            float4 positionCS = TransformWorldToHClip(positionWS + lightDirectionWS * _ShadowBias.x + normalWS * biasN);
        #endif

        #if UNITY_REVERSED_Z
            positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
        #else
            positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
        #endif

        return positionCS;
    }
    #define LIL_V2F_SHADOW_CASTER_OUTPUT        float4 positionCS : SV_POSITION;
    #define LIL_TRANSFER_SHADOW_CASTER(v,o)     o.positionCS = URPShadowPos(v.positionOS, v.normalOS, _lilShadowCasterBias)
    #define LIL_SHADOW_CASTER_FRAGMENT(i)       return 0

    // Additional Light
    void lilGetAdditionalLights(float3 positionWS, float4 positionCS, float strength, inout float3 lightColor, inout float3 lightDirection)
    {
        uint renderingLayers = lilGetRenderingLayer();
        float3 objPositionWS = lilGetObjectPosition();

        #if defined(_ADDITIONAL_LIGHTS) || defined(_ADDITIONAL_LIGHTS_VERTEX)
            uint lightsCount = GetAdditionalLightsCount();
            #if defined(LIGHT_LOOP_BEGIN)
                InputData inputData = (InputData)0;
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(positionCS);
                inputData.positionWS = positionWS;
                LIGHT_LOOP_BEGIN(lightsCount)
            #else
                for(uint lightIndex = 0; lightIndex < lightsCount; lightIndex++)
                {
            #endif

                Light light = GetAdditionalLight(lightIndex, positionWS);
                #if LIL_SRP_VERSION_GREATER_EQUAL(12, 0) && defined(_LIGHT_LAYERS)
                    if((light.layerMask & renderingLayers) != 0)
                #endif
                {
                    lightColor += light.color.rgb * light.distanceAttenuation * strength;
                    lightDirection += dot(light.color.rgb, float3(1.0/3.0, 1.0/3.0, 1.0/3.0)) * light.distanceAttenuation * strength * light.direction;
                }

            #if defined(LIGHT_LOOP_END)
                LIGHT_LOOP_END
            #else
                }
            #endif
        #endif

        #if defined(_ADDITIONAL_LIGHTS) && (defined(USE_CLUSTERED_LIGHTING) && USE_CLUSTERED_LIGHTING || defined(USE_FORWARD_PLUS) && USE_FORWARD_PLUS)
            #if defined(URP_FP_DIRECTIONAL_LIGHTS_COUNT)
                for(uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++)
            #else
                for(uint lightIndex = 0; lightIndex < min(_AdditionalLightsDirectionalCount, MAX_VISIBLE_LIGHTS); lightIndex++)
            #endif
            {
                Light light = GetAdditionalLight(lightIndex, positionWS);
                #if LIL_SRP_VERSION_GREATER_EQUAL(12, 0) && defined(_LIGHT_LAYERS)
                    if((light.layerMask & renderingLayers) != 0)
                #endif
                lightColor += light.color.rgb * light.distanceAttenuation * strength;
                lightDirection += dot(light.color.rgb, float3(1.0/3.0, 1.0/3.0, 1.0/3.0)) * light.distanceAttenuation * strength * light.direction;
            }
        #endif
    }

    float3 lilGetAdditionalLights(float3 positionWS, float4 positionCS, float strength)
    {
        float3 lightColor = 0.0;
        float3 lightDirection = 0.0;
        lilGetAdditionalLights(positionWS, positionCS, strength, lightColor, lightDirection);
        return lightColor;
    }

    // Lightmap
    #define LIL_DECODE_LIGHTMAP(lm)                     DecodeLightmap(lm, float4(LIGHTMAP_HDR_MULTIPLIER,LIGHTMAP_HDR_EXPONENT,0.0,0.0))
    #define LIL_DECODE_DYNAMICLIGHTMAP(lm)              lm.rgb

    // Environment reflection
    #define LIL_GET_ENVIRONMENT_REFLECTION(viewDirection,normalDirection,perceptualRoughness,positionWS) \
        ((IsEmpty(unity_SpecCube0) || unity_SpecCube0_HDR.x == 0 || _ReflectionCubeOverride) ? \
        lilCustomReflection(_ReflectionCubeTex, _ReflectionCubeTex_HDR, viewDirection, normalDirection, perceptualRoughness) * _ReflectionCubeColor.rgb * lerp(1.0, fd.lightColor, _ReflectionCubeEnableLighting) : \
        GlossyEnvironmentReflection(reflect(-viewDirection,normalDirection), perceptualRoughness, 1.0))

    // Fog
    #if LIL_RENDER == 2
        #define LIL_APPLY_FOG_BASE(col,fogCoord)                 col.rgb = lerp(unity_FogColor.rgb*col.a,col.rgb,fogCoord)
        #define LIL_APPLY_FOG_COLOR_BASE(col,fogCoord,fogColor)  col.rgb = lerp(fogColor.rgb*col.a,col.rgb,fogCoord)
    #else
        #define LIL_APPLY_FOG_BASE(col,fogCoord)                 col.rgb = lerp(unity_FogColor.rgb,col.rgb,fogCoord)
        #define LIL_APPLY_FOG_COLOR_BASE(col,fogCoord,fogColor)  col.rgb = lerp(fogColor.rgb,col.rgb,fogCoord)
    #endif
    #if LIL_SRP_VERSION_GREATER_EQUAL(7, 1)
        float lilCalcFogFactor(float depth)
        {
            #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
                return ComputeFogIntensity(ComputeFogFactor(depth));
            #else
                return 1.0;
            #endif
        }
    #else
        float lilCalcFogFactor(float depth)
        {
            #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
                float factor = ComputeFogFactor(depth);
                #if defined(FOG_EXP)
                    return saturate(exp2(-factor));
                #elif defined(FOG_EXP2)
                    return saturate(exp2(-factor*factor));
                #elif defined(FOG_LINEAR)
                    return factor;
                #else
                    return 0.0;
                #endif
            #else
                return 1.0;
            #endif
        }
    #endif
#endif

// Meta
#if !defined(LIL_BRP) && (LIL_SRP_VERSION_LOWER(5, 14))
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
    #define LIL_IS_MIRROR           (dot(cross(LIL_MATRIX_V[0].xyz, LIL_MATRIX_V[1].xyz), LIL_MATRIX_V[2].xyz) > 0)
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
#if (defined(LIL_LITE) || defined(LIL_FUR) || defined(LIL_GEM)) && !defined(LIL_PASS_FORWARDADD)
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
    #define LIL_LIGHTMAP_DIR_TEX                unity_LightmapsInd
    #define LIL_SAMPLE_LIGHTMAP(tex,samp,uv)    LIL_SAMPLE_2D_ARRAY(tex,samp,uv,unity_LightmapIndex.x)
#else
    #define LIL_SHADOWMAP_TEX                   unity_ShadowMask
    #define LIL_SHADOWMAP_SAMP                  samplerunity_ShadowMask
    #define LIL_LIGHTMAP_TEX                    unity_Lightmap
    #define LIL_LIGHTMAP_SAMP                   samplerunity_Lightmap
    #define LIL_LIGHTMAP_DIR_TEX                unity_LightmapInd
    #define LIL_SAMPLE_LIGHTMAP(tex,samp,uv)    LIL_SAMPLE_2D(tex,samp,uv)
#endif

#define LIL_DYNAMICLIGHTMAP_TEX             unity_DynamicLightmap
#define LIL_DYNAMICLIGHTMAP_SAMP            samplerunity_DynamicLightmap
#define LIL_DYNAMICLIGHTMAP_DIR_TEX         unity_DynamicDirectionality

float3 lilGetLightMapColor(float2 uv1, float2 uv2)
{
    float3 outCol = 0;
    #if defined(LIL_USE_LIGHTMAP)
        float2 lightmapUV = uv1 * unity_LightmapST.xy + unity_LightmapST.zw;
        float4 lightmap = LIL_SAMPLE_LIGHTMAP(LIL_LIGHTMAP_TEX, LIL_LIGHTMAP_SAMP, lightmapUV);
        outCol += LIL_DECODE_LIGHTMAP(lightmap);
    #endif
    #if defined(LIL_USE_DYNAMICLIGHTMAP)
        float2 dynlightmapUV = uv2 * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
        float4 dynlightmap = LIL_SAMPLE_2D(LIL_DYNAMICLIGHTMAP_TEX, LIL_DYNAMICLIGHTMAP_SAMP, dynlightmapUV);
        outCol += LIL_DECODE_DYNAMICLIGHTMAP(dynlightmap);
    #endif
    return outCol;
}

float3 lilGetLightMapDirection(float2 uv)
{
    float3 lightmapDir = 0.0;
    #if defined(LIL_USE_LIGHTMAP) && defined(LIL_USE_DIRLIGHTMAP)
        float4 lightmapDirection = LIL_SAMPLE_LIGHTMAP(LIL_DIRLIGHTMAP_TEX,  LIL_LIGHTMAP_SAMP, uv);
        lightmapDir = lightmapDirection.xyz * 2.0 - 1.0;
    #endif
    #if defined(LIL_USE_DYNAMICLIGHTMAP) && defined(LIL_USE_DIRLIGHTMAP)
        float4 lightmapDirection = LIL_SAMPLE_LIGHTMAP(LIL_DYNAMICLIGHTMAP_DIR_TEX,  LIL_DYNAMICLIGHTMAP_SAMP, uv);
        lightmapDir = lightmapDirection.xyz * 2.0 - 1.0;
    #endif
    return lightmapDir;
}

// Main Light Coords
#if defined(LIL_PASS_FORWARDADD)
    #define LIL_LIGHTCOLOR_COORDS(idx)
    #define LIL_LIGHTDIRECTION_COORDS(idx)
#else
    #define LIL_LIGHTCOLOR_COORDS(idx)      float3 lightColor : TEXCOORD##idx;
    #define LIL_LIGHTDIRECTION_COORDS(idx)  float3 lightDirection : TEXCOORD##idx;
#endif

#if !defined(LIL_PASS_FORWARDADD) && (defined(LIL_FEATURE_SHADOW) || defined(LIL_LITE))
    #define LIL_INDLIGHTCOLOR_COORDS(idx)   float3 indLightColor : TEXCOORD##idx;
    #define LIL_GET_INDLIGHTCOLOR(i,o)      o.indLightColor = i.indLightColor
#else
    #define LIL_INDLIGHTCOLOR_COORDS(idx)
    #define LIL_GET_INDLIGHTCOLOR(i,o)
#endif

// Dir light & indir light
#if defined(LIL_USE_LPPV) && (defined(LIL_FEATURE_SHADOW) || defined(LIL_LITE))
    #define LIL_CALC_TWOLIGHT(i,o) lilGetLightColorDouble(i.positionWS, o.lightColor, o.indLightColor)
#elif defined(LIL_FEATURE_SHADOW) || defined(LIL_LITE)
    #define LIL_CALC_TWOLIGHT(i,o) lilGetLightColorDouble(o.lightColor, o.indLightColor)
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
#if defined(LIL_USE_ADDITIONALLIGHT_MAIN)
    #define LIL_APPLY_ADDITIONALLIGHT_TO_MAIN(i,o) \
        float3 additionalLightDirection = 0.0; \
        lilGetAdditionalLights(i.positionWS, i.positionCS/float4(i.positionCS.www,1.0)*float4(LIL_SCREENPARAMS.xy,1.0,1.0), LIL_ADDITIONAL_LIGHT_STRENGTH, o.lightColor, additionalLightDirection)
    #define LIL_CORRECT_LIGHTDIRECTION_PS(lightDirection) lightDirection = normalize(lightDirection)
#elif defined(LIL_USE_ADDITIONALLIGHT_MAINDIR)
    #define LIL_APPLY_ADDITIONALLIGHT_TO_MAIN(i,o) \
        lilGetAdditionalLights(i.positionWS, i.positionCS/float4(i.positionCS.www,1.0)*float4(LIL_SCREENPARAMS.xy,1.0,1.0), LIL_ADDITIONAL_LIGHT_STRENGTH, o.lightColor, o.lightDirection)
    #define LIL_CORRECT_LIGHTDIRECTION_PS(lightDirection) lightDirection = normalize(lightDirection)
#elif defined(LIL_USE_ADDITIONALLIGHT_MAINDIR_PS)
    #define LIL_APPLY_ADDITIONALLIGHT_TO_MAIN(i,o)
    #define LIL_CORRECT_LIGHTDIRECTION_PS(lightDirection) lightDirection = normalize(lightDirection)
#elif defined(LIL_URP)
    #define LIL_APPLY_ADDITIONALLIGHT_TO_MAIN(i,o) o.lightDirection = normalize(o.lightDirection)
    #define LIL_CORRECT_LIGHTDIRECTION_PS(lightDirection)
#else
    #define LIL_APPLY_ADDITIONALLIGHT_TO_MAIN(i,o)
    #define LIL_CORRECT_LIGHTDIRECTION_PS(lightDirection)
#endif

#if defined(LIL_USE_LIGHTMAP) || defined(LIL_USE_ADDITIONALLIGHT_MAINDIR_PS)
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
        o.lightDirection = o.lightDirection * Luminance(o.lightColor) + unity_SHAr.xyz * 0.333333 + unity_SHAg.xyz * 0.333333 + unity_SHAb.xyz * 0.333333; \
        float3 lightDirectionSH = dot(o.lightDirection,o.lightDirection) < 0.000001 ? 0 : normalize(o.lightDirection); \
        o.lightDirection += lilGetCustomLightDirection(_LightDirectionOverride); \
        o.lightColor += lilShadeSH9(float4(lightDirectionSH * 0.666666, 1.0)); \
        o.indLightColor = lilShadeSH9(float4(-lightDirectionSH * 0.666666, 1.0)); \
        o.indLightColor = saturate(o.indLightColor / Luminance(o.lightColor)); \
        o.lightColor = min(o.lightColor, _BeforeExposureLimit); \
        o.lightColor *= GetCurrentExposureMultiplier(); \
        LIL_APPLY_ADDITIONALLIGHT_TO_MAIN(i,o); \
        LIL_CORRECT_LIGHTCOLOR_VS(o.lightColor)
#elif defined(LIL_BRP)
    #define LIL_CALC_MAINLIGHT(i,o) \
        OpenLitLightDatas lightDatas; \
        ComputeLights(lightDatas, _LightDirectionOverride); \
        lilLightData o; \
        o.lightDirection    = lightDatas.lightDirection; \
        o.lightColor        = lightDatas.directLight; \
        o.indLightColor     = lightDatas.indirectLight; \
        LIL_APPLY_ADDITIONALLIGHT_TO_MAIN(i,o); \
        LIL_CORRECT_LIGHTCOLOR_VS(o.lightColor)
#else
    #define LIL_CALC_MAINLIGHT(i,o) \
        lilLightData o; \
        o.lightDirection = normalize(LIL_MAINLIGHT_DIRECTION * lilLuminance(LIL_MAINLIGHT_COLOR) + unity_SHAr.xyz * 0.333333 + unity_SHAg.xyz * 0.333333 + unity_SHAb.xyz * 0.333333); \
        LIL_CALC_TWOLIGHT(i,o); \
        o.lightDirection = lilGetFixedLightDirection(_LightDirectionOverride, false); \
        LIL_APPLY_ADDITIONALLIGHT_TO_MAIN(i,o); \
        LIL_CORRECT_LIGHTCOLOR_VS(o.lightColor)
#endif

// Main Light in PS (Color / Direction / Attenuation)
#if defined(LIL_PASS_FORWARDADD)
    // Point Light & Spot Light (ForwardAdd)
    #define LIL_GET_MAINLIGHT(input,lc,ld,atten) \
        ld = lilGetLightDirection(input.positionWS); \
        lc = min(LIL_MAINLIGHT_COLOR * atten, _LightMaxLimit); \
        lc = lerp(lc, lilGray(lc), _MonochromeLighting); \
        lc = lerp(lc, 0.0, _AsUnlit)
#elif defined(LIL_HDRP) && defined(LIL_USE_LIGHTMAP)
    // HDRP with lightmap
    #define LIL_GET_MAINLIGHT(input,lc,ld,atten) \
        lc = input.lightColor; \
        float3 lightmapColor = lilGetLightMapColor(fd.uv1,fd.uv2); \
        lc += lightmapColor * GetCurrentExposureMultiplier(); \
        LIL_CORRECT_LIGHTCOLOR_PS(lc); \
        LIL_CORRECT_LIGHTDIRECTION_PS(ld)
#elif defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SHADOWMASK)
    // Mixed Lightmap (Shadowmask)
    #define LIL_GET_MAINLIGHT(input,lc,ld,atten) \
        lc = input.lightColor; \
        float3 lightmapColor = lilGetLightMapColor(fd.uv1,fd.uv2); \
        lc = max(lc, lightmapColor); \
        LIL_CORRECT_LIGHTCOLOR_PS(lc); \
        LIL_CORRECT_LIGHTDIRECTION_PS(ld); \
        atten = min(atten, LIL_SAMPLE_LIGHTMAP(LIL_SHADOWMAP_TEX,LIL_LIGHTMAP_SAMP,fd.uv1).r)
#elif defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SUBTRACTIVE) && defined(LIL_USE_DYNAMICLIGHTMAP)
    // Mixed Lightmap (Subtractive)
    // Use Lightmap as Shadowmask
    #undef LIL_USE_DYNAMICLIGHTMAP
    #define LIL_GET_MAINLIGHT(input,lc,ld,atten) \
        lc = input.lightColor; \
        float3 lightmapColor = lilGetLightMapColor(fd.uv1,fd.uv2); \
        lc = max(lc, lightmapColor); \
        LIL_CORRECT_LIGHTCOLOR_PS(lc); \
        LIL_CORRECT_LIGHTDIRECTION_PS(ld); \
        float3 lightmapShadowThreshold = LIL_MAINLIGHT_COLOR*0.5; \
        float3 lightmapS = (lightmapColor - lightmapShadowThreshold) / (LIL_MAINLIGHT_COLOR - lightmapShadowThreshold); \
        float lightmapAttenuation = saturate((lightmapS.r+lightmapS.g+lightmapS.b)/3.0); \
        atten = min(atten, lightmapAttenuation)
#elif defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SUBTRACTIVE)
    // Mixed Lightmap (Subtractive)
    // Use Lightmap as Shadowmask
    #define LIL_GET_MAINLIGHT(input,lc,ld,atten) \
        lc = input.lightColor; \
        float3 lightmapColor = lilGetLightMapColor(fd.uv1,fd.uv2); \
        lc = max(lc, lightmapColor); \
        LIL_CORRECT_LIGHTCOLOR_PS(lc); \
        LIL_CORRECT_LIGHTDIRECTION_PS(ld); \
        float3 lightmapS = (lightmapColor - lilShadeSH9(input.normalWS)) / LIL_MAINLIGHT_COLOR; \
        float lightmapAttenuation = saturate((lightmapS.r+lightmapS.g+lightmapS.b)/3.0); \
        atten = min(atten, lightmapAttenuation)
#elif defined(LIL_USE_LIGHTMAP) && defined(LIL_USE_DIRLIGHTMAP)
    // Lightmap (Directional)
    #define LIL_GET_MAINLIGHT(input,lc,ld,atten) \
        lc = input.lightColor; \
        float3 lightmapColor = lilGetLightMapColor(fd.uv1,fd.uv2); \
        float3 lightmapDirection = lilGetLightMapDirection(input.uv1); \
        lc = saturate(lc + lightmapColor); \
        LIL_CORRECT_LIGHTCOLOR_PS(lc); \
        ld = normalize(ld + lightmapDirection * lilLuminance(lightmapColor)); \
        LIL_CORRECT_LIGHTDIRECTION_PS(ld)
#elif defined(LIL_USE_LIGHTMAP) && defined(LIL_USE_SHADOW)
    // Mixed Lightmap (Baked Indirect) with shadow
    #define LIL_GET_MAINLIGHT(input,lc,ld,atten) \
        lc = LIL_MAINLIGHT_COLOR; \
        float3 lightmapColor = lilGetLightMapColor(fd.uv1,fd.uv2); \
        lc = saturate(lc + max(lightmapColor,lilGetSHToon())); \
        LIL_CORRECT_LIGHTCOLOR_PS(lc); \
        LIL_CORRECT_LIGHTDIRECTION_PS(ld)
#elif defined(LIL_USE_LIGHTMAP) && defined(LIL_USE_DYNAMICLIGHTMAP)
    // Mixed Lightmap (Baked Indirect) or Lightmap (Non-Directional)
    #undef LIL_USE_DYNAMICLIGHTMAP
    #define LIL_GET_MAINLIGHT(input,lc,ld,atten) \
        lc = input.lightColor; \
        float3 lightmapColor = lilGetLightMapColor(fd.uv1,fd.uv2); \
        lc = saturate(lc + lightmapColor); \
        LIL_CORRECT_LIGHTCOLOR_PS(lc); \
        LIL_CORRECT_LIGHTDIRECTION_PS(ld)
#elif defined(LIL_USE_LIGHTMAP)
    // Mixed Lightmap (Baked Indirect) or Lightmap (Non-Directional)
    #define LIL_GET_MAINLIGHT(input,lc,ld,atten) \
        lc = LIL_MAINLIGHT_COLOR; \
        float3 lightmapColor = lilGetLightMapColor(fd.uv1,fd.uv2); \
        lc = saturate(lc + lightmapColor); \
        LIL_CORRECT_LIGHTCOLOR_PS(lc); \
        LIL_CORRECT_LIGHTDIRECTION_PS(ld)
#elif defined(LIL_USE_ADDITIONALLIGHT_MAINDIR_PS)
    // Realtime
    #define LIL_GET_MAINLIGHT(input,lc,ld,atten) \
        lc = input.lightColor; \
        lilGetAdditionalLights(input.positionWS, input.positionCS, LIL_ADDITIONAL_LIGHT_STRENGTH, lc, ld); \
        LIL_CORRECT_LIGHTCOLOR_PS(lc); \
        LIL_CORRECT_LIGHTDIRECTION_PS(ld)
#elif defined(LIL_USE_ADDITIONALLIGHT_MAINDIR)
    // Realtime
    #define LIL_GET_MAINLIGHT(input,lc,ld,atten) \
        lc = input.lightColor; \
        LIL_CORRECT_LIGHTDIRECTION_PS(ld)
#else
    // Realtime
    #define LIL_GET_MAINLIGHT(input,lc,ld,atten) \
        lc = input.lightColor; \
        LIL_CORRECT_LIGHTDIRECTION_PS(ld)
#endif

// Additional Light VS and Fog
#if defined(LIL_USE_ADDITIONALLIGHT_VS)
    #define LIL_VERTEXLIGHT_FOG_TYPE            float4
    #define LIL_VERTEXLIGHT_FOG_COORDS(idx)     float4 vlf : TEXCOORD##idx;
    #define LIL_TRANSFER_FOG(i,o)               o.vlf.w = lilCalcFogFactor(i.positionCS.z)
    #define LIL_APPLY_FOG(col,i)                LIL_APPLY_FOG_BASE(col,i.vlf.w)
    #define LIL_APPLY_FOG_COLOR(col,i,fogColor) LIL_APPLY_FOG_COLOR_BASE(col,i.vlf.w,fogColor)
#elif defined(LIL_HDRP)
    #define LIL_VERTEXLIGHT_FOG_TYPE
    #define LIL_VERTEXLIGHT_FOG_COORDS(idx)
    #define LIL_TRANSFER_FOG(i,o)
    #define LIL_APPLY_FOG(col,i)                LIL_APPLY_FOG_BASE(col,i.vlf)
    #define LIL_APPLY_FOG_COLOR(col,i,fogColor) LIL_APPLY_FOG_COLOR_BASE(col,i.vlf,fogColor)
#else
    #define LIL_VERTEXLIGHT_FOG_TYPE            float
    #define LIL_VERTEXLIGHT_FOG_COORDS(idx)     float vlf : TEXCOORD##idx;
    #define LIL_TRANSFER_FOG(i,o)               o.vlf = lilCalcFogFactor(i.positionCS.z)
    #define LIL_APPLY_FOG(col,i)                LIL_APPLY_FOG_BASE(col,i.vlf)
    #define LIL_APPLY_FOG_COLOR(col,i,fogColor) LIL_APPLY_FOG_COLOR_BASE(col,i.vlf,fogColor)
#endif

#if defined(LIL_USE_ADDITIONALLIGHT_VS) && (defined(VERTEXLIGHT_ON) || !defined(LIL_BRP))
    #define LIL_CALC_VERTEXLIGHT(i,o) \
        o.vlf.rgb = lilGetAdditionalLights(i.positionWS, i.positionCS/float4(i.positionCS.www,1.0)*float4(LIL_SCREENPARAMS.xy,1.0,1.0), LIL_ADDITIONAL_LIGHT_STRENGTH); \
        o.vlf.rgb = lerp(o.vlf.rgb, lilGray(o.vlf.rgb), _MonochromeLighting); \
        o.vlf.rgb = lerp(o.vlf.rgb, 0.0, _AsUnlit)
#elif defined(LIL_USE_ADDITIONALLIGHT_VS)
    #define LIL_CALC_VERTEXLIGHT(i,o)
#elif defined(LIL_USE_ADDITIONALLIGHT_MAIN)
    #define LIL_CALC_VERTEXLIGHT(i,o)
#else
    #define LIL_CALC_VERTEXLIGHT(i,o)
#endif

// Additional Light PS
#if defined(LIL_USE_ADDITIONALLIGHT_PS)
    #define LIL_GET_ADDITIONALLIGHT(i,o) \
        o = lilGetAdditionalLights(i.positionWS, i.positionCS, LIL_ADDITIONAL_LIGHT_STRENGTH); \
        o = lerp(o, lilGray(o), _MonochromeLighting); \
        o = lerp(o, 0.0, _AsUnlit)
#elif defined(LIL_USE_ADDITIONALLIGHT_VS)
    #define LIL_GET_ADDITIONALLIGHT(i,o) \
        o = i.vlf.rgb
#elif defined(LIL_USE_ADDITIONALLIGHT_MAIN)
    #define LIL_GET_ADDITIONALLIGHT(i,o) \
        o = 0
#else
    #define LIL_GET_ADDITIONALLIGHT(i,o) \
        o = 0
#endif

// Fragment Macro
#if defined(LIL_HDRP)
    #define LIL_GET_LIGHTING_DATA(input,fd) \
        LIL_GET_MAINLIGHT(input, fd.lightColor, fd.L, fd.attenuation); \
        fd.origL = fd.L; \
        LIL_GET_ADDITIONALLIGHT(input, fd.addLightColor); \
        fd.invLighting = saturate((1.0 - fd.lightColor) * sqrt(fd.lightColor))
#else
    #define LIL_GET_LIGHTING_DATA(input,fd) \
        LIL_GET_MAINLIGHT(input, fd.lightColor, fd.L, fd.attenuation); \
        LIL_GET_ADDITIONALLIGHT(input, fd.addLightColor); \
        fd.invLighting = saturate((1.0 - fd.lightColor) * sqrt(fd.lightColor))
#endif

#define LIL_GET_POSITION_WS_DATA(input,fd) \
    fd.depth = length(lilHeadDirection(fd.positionWS)); \
    fd.depthObject = length(lilHeadDirection(lilTransformOStoWS(float3(0,0,0)))); \
    fd.V = normalize(lilViewDirection(fd.positionWS)); \
    fd.headV = normalize(lilHeadDirection(fd.positionWS)); \
    fd.vl = dot(fd.V, fd.L); \
    fd.hl = dot(fd.headV, fd.L); \
    fd.uvPanorama = lilGetPanoramaUV(fd.V)

#define LIL_GET_TBN_DATA(input,fd) \
    float3 bitangentWS = cross(input.normalWS, input.tangentWS.xyz) * (input.tangentWS.w * LIL_NEGATIVE_SCALE); \
    fd.TBN = float3x3(input.tangentWS.xyz, bitangentWS, input.normalWS)

#define LIL_GET_PARALLAX_DATA(input,fd) \
    fd.parallaxViewDirection = mul(fd.TBN, fd.V); \
    fd.parallaxOffset = (fd.parallaxViewDirection.xy / (fd.parallaxViewDirection.z+0.5))

// Main Color & Emission
#if defined(LIL_BAKER)
    #define LIL_GET_SUBTEX(tex,uv)  lilGetSubTexWithoutAnimation(tex, tex##_ST, tex##_ScrollRotate, tex##Angle, uv, 1, tex##IsDecal, tex##IsLeftOnly, tex##IsRightOnly, tex##ShouldCopy, tex##ShouldFlipMirror, tex##ShouldFlipCopy, tex##IsMSDF, isRightHand LIL_SAMP_IN(sampler##tex))
    #define LIL_GET_EMITEX(tex,uv)  LIL_SAMPLE_2D(tex, sampler##tex, lilCalcUVWithoutAnimation(uv, tex##_ST, tex##_ScrollRotate))
    #define LIL_GET_EMIMASK(tex,uv) LIL_SAMPLE_2D(tex, sampler_MainTex, lilCalcUVWithoutAnimation(uv, tex##_ST, tex##_ScrollRotate))
#elif defined(LIL_WITHOUT_ANIMATION)
    #define LIL_GET_SUBTEX(tex,uv)  lilGetSubTexWithoutAnimation(tex, tex##_ST, tex##_ScrollRotate, tex##Angle, uv, 1, tex##IsDecal, tex##IsLeftOnly, tex##IsRightOnly, tex##ShouldCopy, tex##ShouldFlipMirror, tex##ShouldFlipCopy, tex##IsMSDF, fd.isRightHand LIL_SAMP_IN(sampler##tex))
    #define LIL_GET_EMITEX(tex,uv)  LIL_SAMPLE_2D(tex, sampler##tex, lilCalcUVWithoutAnimation(uv, tex##_ST, tex##_ScrollRotate))
    #define LIL_GET_EMIMASK(tex,uv) LIL_SAMPLE_2D(tex, sampler_MainTex, lilCalcUVWithoutAnimation(uv, tex##_ST, tex##_ScrollRotate))
#else
    #define LIL_GET_SUBTEX(tex,uv)  lilGetSubTex(tex, tex##_ST, tex##_ScrollRotate, tex##Angle, uv, fd.nv, tex##IsDecal, tex##IsLeftOnly, tex##IsRightOnly, tex##ShouldCopy, tex##ShouldFlipMirror, tex##ShouldFlipCopy, tex##IsMSDF, fd.isRightHand, tex##DecalAnimation, tex##DecalSubParam LIL_SAMP_IN(sampler##tex))
    #define LIL_GET_EMITEX(tex,uv)  LIL_SAMPLE_2D(tex, sampler##tex, lilCalcUV(uv, tex##_ST, tex##_ScrollRotate))
    #define LIL_GET_EMIMASK(tex,uv) LIL_SAMPLE_2D(tex, sampler_MainTex, lilCalcUV(uv, tex##_ST, tex##_ScrollRotate))
#endif

// Fallback
#define UnpackNormalScale(normal,scale) lilUnpackNormalScale(normal,scale)

#endif