#ifndef LIL_INPUT_INCLUDED
#define LIL_INPUT_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Variables from Unity
#if !defined(SPOT) && !defined(POINT_COOKIE) && LIL_VERTEXLIGHT_MODE == 4
    TEXTURE2D(_LightTextureB0);
    SAMPLER(sampler_LightTextureB0);
#endif

#if !defined(SHADER_API_GLES)
    TEXTURE3D(_DitherMaskLOD);
    SAMPLER(sampler_DitherMaskLOD);
#endif

SAMPLER(sampler_trilinear_repeat);
//SAMPLER(sampler_linear_repeat);
#define sampler_linear_repeat sampler_trilinear_repeat
SAMPLER(sampler_linear_clamp);

#if defined(LIL_BRP)
    TEXTURE2D_SCREEN(_lilBackgroundTexture);
    TEXTURE2D_SCREEN(_GrabTexture);
    SAMPLER(sampler_lilBackgroundTexture);
    SAMPLER(sampler_GrabTexture);
    float4 _lilBackgroundTexture_TexelSize;
    #define LIL_GET_BG_TEX(uv,lod) LIL_SAMPLE_SCREEN(_lilBackgroundTexture, sampler_lilBackgroundTexture, uv)
    #define LIL_GET_GRAB_TEX(uv,lod) LIL_SAMPLE_SCREEN(_GrabTexture, sampler_GrabTexture, uv)
#elif defined(LIL_HDRP)
    #define LIL_GET_BG_TEX(uv,lod) SampleCameraColor(uv,lod)
    #define LIL_GET_GRAB_TEX(uv,lod) SampleCameraColor(uv,lod)
#else
    TEXTURE2D_SCREEN(_CameraOpaqueTexture);
    SAMPLER(sampler_CameraOpaqueTexture);
    #define LIL_GET_BG_TEX(uv,lod) LIL_SAMPLE_SCREEN_LOD(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv, lod)
    #define LIL_GET_GRAB_TEX(uv,lod) LIL_SAMPLE_SCREEN_LOD(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv, lod)
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Texture Exists
#if defined(LIL_FEATURE_TEX_LAYER_MASK)
    #define Exists_Main2ndBlendMask     true
    #define Exists_Main3rdBlendMask     true
#else
    #define Exists_Main2ndBlendMask     false
    #define Exists_Main3rdBlendMask     false
#endif

#if defined(LIL_FEATURE_LAYER_DISSOLVE)
    #define Exists_Main2ndDissolveMask    true
    #define Exists_Main3rdDissolveMask    true
#else
    #define Exists_Main2ndDissolveMask    false
    #define Exists_Main3rdDissolveMask    false
#endif

#if defined(LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE)
    #define Exists_Main2ndDissolveNoiseMask    true
    #define Exists_Main3rdDissolveNoiseMask    true
#else
    #define Exists_Main2ndDissolveNoiseMask    false
    #define Exists_Main3rdDissolveNoiseMask    false
#endif

#if defined(LIL_FEATURE_TEX_NORMAL_MASK)
    #define Exists_Bump2ndScaleMask     true
#else
    #define Exists_Bump2ndScaleMask     false
#endif

#if defined(LIL_FEATURE_TEX_SHADOW_BORDER)
    #define Exists_ShadowBorderMask     true
#else
    #define Exists_ShadowBorderMask     false
#endif

#if defined(LIL_FEATURE_TEX_SHADOW_BLUR)
    #define Exists_ShadowBlurMask       true
#else
    #define Exists_ShadowBlurMask       false
#endif

#if defined(LIL_FEATURE_TEX_SHADOW_STRENGTH)
    #define Exists_ShadowStrengthMask   true
#else
    #define Exists_ShadowStrengthMask   false
#endif

#if defined(LIL_FEATURE_TEX_SHADOW_1ST)
    #define Exists_ShadowColorTex       true
#else
    #define Exists_ShadowColorTex       false
#endif

#if defined(LIL_FEATURE_TEX_SHADOW_2ND)
    #define Exists_Shadow2ndColorTex    true
#else
    #define Exists_Shadow2ndColorTex    false
#endif

#if defined(LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS)
    #define Exists_SmoothnessTex        true
#else
    #define Exists_SmoothnessTex        true
#endif

#if defined(LIL_FEATURE_TEX_REFLECTION_METALLIC)
    #define Exists_MetallicGlossMap     true
#else
    #define Exists_MetallicGlossMap     false
#endif

#if defined(LIL_FEATURE_TEX_REFLECTION_COLOR)
    #define Exists_ReflectionColorTex   true
#else
    #define Exists_ReflectionColorTex   false
#endif

#if defined(LIL_FEATURE_TEX_MATCAP_MASK)
    #define Exists_MatCapBlendMask      true
    #define Exists_MatCap2ndBlendMask   true
#else
    #define Exists_MatCapBlendMask      false
    #define Exists_MatCap2ndBlendMask   false
#endif

#if defined(LIL_FEATURE_TEX_RIMLIGHT_COLOR)
    #define Exists_RimColorTex          true
#else
    #define Exists_RimColorTex          false
#endif

#if defined(LIL_FEATURE_GLITTER)
    #define Exists_GlitterColorTex      true
#else
    #define Exists_GlitterColorTex      false
#endif

#if defined(LIL_FEATURE_TEX_EMISSION_MASK)
    #define Exists_EmissionBlendMask    true
    #define Exists_Emission2ndBlendMask true
#else
    #define Exists_EmissionBlendMask    false
    #define Exists_Emission2ndBlendMask false
#endif

#if defined(LIL_FEATURE_AUDIOLINK)
    #define Exists_AudioLinkMask        true
#else
    #define Exists_AudioLinkMask        false
#endif

#if defined(LIL_FEATURE_TEX_DISSOLVE_NOISE)
    #define Exists_DissolveNoiseMask    true
#else
    #define Exists_DissolveNoiseMask    false
#endif

#if defined(LIL_FEATURE_TEX_OUTLINE_COLOR)
    #define Exists_OutlineTex           true
#else
    #define Exists_OutlineTex           false
#endif

#if defined(LIL_FEATURE_TEX_OUTLINE_WIDTH)
    #define Exists_OutlineWidthMask     true
#else
    #define Exists_OutlineWidthMask     false
#endif

#if defined(LIL_FEATURE_TEX_OUTLINE_NORMAL)
    #define Exists_OutlineVectorTex     true
#else
    #define Exists_OutlineVectorTex     false
#endif

#if defined(LIL_FEATURE_TEX_FUR_MASK)
    #define Exists_FurMask              true
#else
    #define Exists_FurMask              false
#endif

#if defined(LIL_FEATURE_TEX_FUR_LENGTH)
    #define Exists_FurLengthMask        true
#else
    #define Exists_FurLengthMask        false
#endif

#if defined(LIL_FEATURE_TEX_FUR_NORMAL)
    #define Exists_FurVectorTex         true
#else
    #define Exists_FurVectorTex         false
#endif

#define Exists_MainTex              true
#define Exists_Main2ndTex           true
#define Exists_Main3rdTex           true
#define Exists_BumpMap              true
#define Exists_Bump2ndMap           true
#define Exists_MatCapTex            true
#define Exists_MatCap2ndTex         true
#define Exists_EmissionMap          true
#define Exists_EmissionGradTex      true
#define Exists_Emission2ndMap       true
#define Exists_Emission2ndGradTex   true
#define Exists_ParallaxMap          true
#define Exists_DissolveMask         true
#define Exists_FurNoiseMask         true
#define Exists_TriMask              true

//------------------------------------------------------------------------------------------------------------------------------
// Input

// ST               x:scale y:scale z:offset w:offset
// ScrollRotate     x:scroll y:scroll z:angle w:rotation
// Blink            x:strength y:type z:speed w:offset
// HSVG             x:hue y:saturation z:value w:gamma
// BlendMode        0:normal 1:add 2:screen 3:multiply

// bool does not work in cbuffer
//#define lilBool bool
#define lilBool uint

#if defined(LIL_LITE)
    CBUFFER_START(UnityPerMaterial)
    float4  _LightDirectionOverride;
    float4  _Color;
    float4  _MainTex_ST;
    float4  _MainTex_ScrollRotate;
    float4  _ShadowBorderColor;
    float4  _MatCapTex_ST;
    float4  _MatCapBlendUV1;
    float4  _RimColor;
    float4  _EmissionColor;
    float4  _EmissionBlink;
    float4  _EmissionMap_ST;
    float4  _EmissionMap_ScrollRotate;
    float4  _OutlineColor;
    float4  _OutlineTex_ST;
    float4  _OutlineTex_ScrollRotate;
    float   _AsUnlit;
    float   _Cutoff;
    float   _SubpassCutoff;
    float   _FlipNormal;
    float   _ShiftBackfaceUV;
    float   _VertexLightStrength;
    float   _LightMinLimit;
    float   _LightMaxLimit;
    float   _BeforeExposureLimit;
    float   _MonochromeLighting;
    float   _lilDirectionalLightStrength;
    float   _BackfaceForceShadow;
    float   _ShadowBorder;
    float   _ShadowBlur;
    float   _Shadow2ndBorder;
    float   _Shadow2ndBlur;
    float   _ShadowEnvStrength;
    float   _ShadowBorderRange;
    float   _MatCapVRParallaxStrength;
    float   _RimBorder;
    float   _RimBlur;
    float   _RimFresnelPower;
    float   _RimShadowMask;
    float   _OutlineWidth;
    float   _OutlineEnableLighting;
    uint    _Cull;
    uint    _OutlineCull;
    uint    _EmissionMap_UVMode;
    lilBool _Invisible;
    lilBool _UseShadow;
    lilBool _UseMatCap;
    lilBool _MatCapMul;
    lilBool _MatCapPerspective;
    lilBool _MatCapZRotCancel;
    lilBool _UseRim;
    lilBool _UseEmission;
    lilBool _OutlineFixWidth;
    lilBool _OutlineVertexR2Width;
    #if defined(LIL_CUSTOM_PROPERTIES)
        LIL_CUSTOM_PROPERTIES
    #endif
    CBUFFER_END
#elif defined(LIL_BAKER)
    CBUFFER_START(UnityPerMaterial)
    float4  _Color;
    float4  _MainTex_ST;
    float4  _MainTexHSVG;
    float4  _Color2nd;
    float4  _Main2ndTex_ST;
    float4  _Color3rd;
    float4  _Main3rdTex_ST;
    float   _MainGradationStrength;
    float   _Main2ndTexAngle;
    float   _Main3rdTexAngle;
    float   _AlphaMaskScale;
    float   _AlphaMaskValue;
    uint    _Main2ndTexBlendMode;
    uint    _Main3rdTexBlendMode;
    uint    _AlphaMaskMode;
    lilBool _UseMain2ndTex;
    lilBool _Main2ndTexIsDecal;
    lilBool _Main2ndTexIsLeftOnly;
    lilBool _Main2ndTexIsRightOnly;
    lilBool _Main2ndTexShouldCopy;
    lilBool _Main2ndTexShouldFlipMirror;
    lilBool _Main2ndTexShouldFlipCopy;
    lilBool _Main2ndTexIsMSDF;
    lilBool _UseMain3rdTex;
    lilBool _Main3rdTexIsDecal;
    lilBool _Main3rdTexIsLeftOnly;
    lilBool _Main3rdTexIsRightOnly;
    lilBool _Main3rdTexShouldCopy;
    lilBool _Main3rdTexShouldFlipMirror;
    lilBool _Main3rdTexShouldFlipCopy;
    lilBool _Main3rdTexIsMSDF;
    CBUFFER_END
#elif defined(LIL_MULTI)
    CBUFFER_START(UnityPerMaterial)
    float4  _LightDirectionOverride;
    float4  _Color;
    float4  _MainTex_ST;
    float4  _MainTex_ScrollRotate;
    #if defined(LIL_MULTI_INPUTS_MAIN_TONECORRECTION)
        float4  _MainTexHSVG;
    #endif
    #if defined(LIL_MULTI_INPUTS_MAIN2ND)
        float4  _Color2nd;
        float4  _Main2ndTex_ST;
        float4  _Main2ndDistanceFade;
        float4  _Main2ndTexDecalAnimation;
        float4  _Main2ndTexDecalSubParam;
        float4  _Main2ndDissolveMask_ST;
        float4  _Main2ndDissolveColor;
        float4  _Main2ndDissolveParams;
        float4  _Main2ndDissolvePos;
        float4  _Main2ndDissolveNoiseMask_ST;
        float4  _Main2ndDissolveNoiseMask_ScrollRotate;
    #endif
    #if defined(LIL_MULTI_INPUTS_MAIN3RD)
        float4  _Color3rd;
        float4  _Main3rdTex_ST;
        float4  _Main3rdDistanceFade;
        float4  _Main3rdTexDecalAnimation;
        float4  _Main3rdTexDecalSubParam;
        float4  _Main3rdDissolveMask_ST;
        float4  _Main3rdDissolveColor;
        float4  _Main3rdDissolveParams;
        float4  _Main3rdDissolvePos;
        float4  _Main3rdDissolveNoiseMask_ST;
        float4  _Main3rdDissolveNoiseMask_ScrollRotate;
    #endif
    #if defined(LIL_MULTI_INPUTS_SHADOW)
        float4  _ShadowColor;
        float4  _Shadow2ndColor;
        float4  _ShadowBorderColor;
        float4  _ShadowAOShift;
    #endif
    #if defined(LIL_MULTI_INPUTS_BACKLIGHT)
        float4  _BacklightColor;
        float4  _BacklightColorTex_ST;
    #endif
    #if defined(LIL_MULTI_INPUTS_EMISSION)
        float4  _EmissionColor;
        float4  _EmissionBlink;
        float4  _EmissionMap_ST;
        float4  _EmissionMap_ScrollRotate;
        float4  _EmissionBlendMask_ST;
        float4  _EmissionBlendMask_ScrollRotate;
    #endif
    #if defined(LIL_MULTI_INPUTS_EMISSION_2ND)
        float4  _Emission2ndColor;
        float4  _Emission2ndBlink;
        float4  _Emission2ndMap_ST;
        float4  _Emission2ndMap_ScrollRotate;
        float4  _Emission2ndBlendMask_ST;
        float4  _Emission2ndBlendMask_ScrollRotate;
    #endif
    #if defined(LIL_MULTI_INPUTS_NORMAL)
        float4  _BumpMap_ST;
    #endif
    #if defined(LIL_MULTI_INPUTS_NORMAL_2ND)
        float4  _Bump2ndMap_ST;
        float4  _Bump2ndScaleMask_ST;
    #endif
    #if defined(LIL_MULTI_INPUTS_ANISOTROPY)
        float4  _AnisotropyTangentMap_ST;
        float4  _AnisotropyScaleMask_ST;
        float4  _AnisotropyShiftNoiseMask_ST;
    #endif
    #if defined(LIL_MULTI_INPUTS_REFLECTION)
        float4  _ReflectionColor;
        float4  _SmoothnessTex_ST;
        float4  _MetallicGlossMap_ST;
        float4  _ReflectionColorTex_ST;
    #endif
    #if defined(LIL_MULTI_INPUTS_MATCAP)
        float4  _MatCapColor;
        float4  _MatCapTex_ST;
        float4  _MatCapBlendMask_ST;
        float4  _MatCapBlendUV1;
        float4  _MatCapBumpMap_ST;
    #endif
    #if defined(LIL_MULTI_INPUTS_MATCAP_2ND)
        float4  _MatCap2ndColor;
        float4  _MatCap2ndTex_ST;
        float4  _MatCap2ndBlendMask_ST;
        float4  _MatCap2ndBlendUV1;
        float4  _MatCap2ndBumpMap_ST;
    #endif
    #if defined(LIL_MULTI_INPUTS_RIM)
        float4  _RimColor;
        float4  _RimColorTex_ST;
        float4  _RimIndirColor;
    #endif
    #if defined(LIL_MULTI_INPUTS_GLITTER)
        float4  _GlitterColor;
        float4  _GlitterColorTex_ST;
        float4  _GlitterParams1;
        float4  _GlitterParams2;
    #endif
    #if defined(LIL_MULTI_INPUTS_DISTANCE_FADE)
        float4  _DistanceFade;
        float4  _DistanceFadeColor;
    #endif
    #if defined(LIL_MULTI_INPUTS_AUDIOLINK)
        float4  _AudioLinkDefaultValue;
        float4  _AudioLinkUVParams;
        float4  _AudioLinkStart;
        float4  _AudioLinkVertexUVParams;
        float4  _AudioLinkVertexStart;
        float4  _AudioLinkVertexStrength;
        float4  _AudioLinkLocalMapParams;
    #endif
    #if defined(LIL_MULTI_INPUTS_DISSOLVE)
        float4  _DissolveMask_ST;
        float4  _DissolveColor;
        float4  _DissolveParams;
        float4  _DissolvePos;
        float4  _DissolveNoiseMask_ST;
        float4  _DissolveNoiseMask_ScrollRotate;
    #endif
    #if defined(LIL_FEATURE_ENCRYPTION)
        float4  _Keys;
    #endif
    #if defined(LIL_MULTI_INPUTS_OUTLINE)
        float4  _OutlineColor;
        float4  _OutlineTex_ST;
        float4  _OutlineTex_ScrollRotate;
        float4  _OutlineTexHSVG;
    #endif
    #if defined(LIL_FUR)
        float4  _FurNoiseMask_ST;
        float4  _FurVector;
    #endif
    #if defined(LIL_REFRACTION) || defined(LIL_GEM)
        float4  _RefractionColor;
    #endif
    #if defined(LIL_GEM)
        float4  _GemParticleColor;
        float4  _GemEnvColor;
    #endif
    float   _AsUnlit;
    float   _Cutoff;
    float   _SubpassCutoff;
    float   _FlipNormal;
    float   _ShiftBackfaceUV;
    float   _VertexLightStrength;
    float   _LightMinLimit;
    float   _LightMaxLimit;
    float   _BeforeExposureLimit;
    float   _MonochromeLighting;
    float   _lilDirectionalLightStrength;
    #if defined(LIL_MULTI_INPUTS_MAIN_TONECORRECTION)
        float   _MainGradationStrength;
    #endif
    #if defined(LIL_MULTI_INPUTS_MAIN2ND)
        float   _Main2ndTexAngle;
        float   _Main2ndEnableLighting;
        float   _Main2ndDissolveNoiseStrength;
    #endif
    #if defined(LIL_MULTI_INPUTS_MAIN3RD)
        float   _Main3rdTexAngle;
        float   _Main3rdEnableLighting;
        float   _Main3rdDissolveNoiseStrength;
    #endif
    #if defined(LIL_MULTI_INPUTS_ALPHAMASK)
        float   _AlphaMaskScale;
        float   _AlphaMaskValue;
    #endif
    float   _BackfaceForceShadow;
    #if defined(LIL_MULTI_INPUTS_SHADOW)
        float   _ShadowStrength;
        float   _ShadowNormalStrength;
        float   _ShadowBorder;
        float   _ShadowBlur;
        float   _Shadow2ndNormalStrength;
        float   _Shadow2ndBorder;
        float   _Shadow2ndBlur;
        float   _ShadowMainStrength;
        float   _ShadowEnvStrength;
        float   _ShadowBorderRange;
    #endif
    #if defined(LIL_MULTI_INPUTS_BACKLIGHT)
        float   _BacklightNormalStrength;
        float   _BacklightBorder;
        float   _BacklightBlur;
        float   _BacklightDirectivity;
        float   _BacklightViewStrength;
        float   _BacklightBackfaceMask;
    #endif
    #if defined(LIL_MULTI_INPUTS_NORMAL)
        float   _BumpScale;
    #endif
    #if defined(LIL_MULTI_INPUTS_NORMAL_2ND)
        float   _Bump2ndScale;
    #endif
    #if defined(LIL_MULTI_INPUTS_ANISOTROPY)
        float   _AnisotropyScale;
        float   _AnisotropyTangentWidth;
        float   _AnisotropyBitangentWidth;
        float   _AnisotropyShift;
        float   _AnisotropyShiftNoiseScale;
        float   _AnisotropySpecularStrength;
        float   _Anisotropy2ndTangentWidth;
        float   _Anisotropy2ndBitangentWidth;
        float   _Anisotropy2ndShift;
        float   _Anisotropy2ndShiftNoiseScale;
        float   _Anisotropy2ndSpecularStrength;
    #endif
    #if defined(LIL_MULTI_INPUTS_REFLECTION) || defined(LIL_GEM)
        float   _Smoothness;
        float   _Reflectance;
        float   _SpecularNormalStrength;
        float   _SpecularBorder;
        float   _SpecularBlur;
        float   _ReflectionNormalStrength;
    #endif
    #if defined(LIL_MULTI_INPUTS_REFLECTION)
        float   _Metallic;
    #endif
    #if defined(LIL_MULTI_INPUTS_MATCAP)
        float   _MatCapBlend;
        float   _MatCapEnableLighting;
        float   _MatCapShadowMask;
        float   _MatCapVRParallaxStrength;
        float   _MatCapBackfaceMask;
        float   _MatCapLod;
        float   _MatCapNormalStrength;
        float   _MatCapBumpScale;
    #endif
    #if defined(LIL_MULTI_INPUTS_MATCAP_2ND)
        float   _MatCap2ndBlend;
        float   _MatCap2ndEnableLighting;
        float   _MatCap2ndShadowMask;
        float   _MatCap2ndVRParallaxStrength;
        float   _MatCap2ndBackfaceMask;
        float   _MatCap2ndLod;
        float   _MatCap2ndNormalStrength;
        float   _MatCap2ndBumpScale;
    #endif
    #if defined(LIL_MULTI_INPUTS_RIM)
        float   _RimNormalStrength;
        float   _RimBorder;
        float   _RimBlur;
        float   _RimFresnelPower;
        float   _RimEnableLighting;
        float   _RimShadowMask;
        float   _RimVRParallaxStrength;
        float   _RimDirStrength;
        float   _RimDirRange;
        float   _RimIndirRange;
        float   _RimIndirBorder;
        float   _RimIndirBlur;
        float   _RimBackfaceMask;
    #endif
    #if defined(LIL_MULTI_INPUTS_GLITTER)
        float   _GlitterMainStrength;
        float   _GlitterNormalStrength;
        float   _GlitterEnableLighting;
        float   _GlitterShadowMask;
        float   _GlitterVRParallaxStrength;
        float   _GlitterBackfaceMask;
    #endif
    #if defined(LIL_MULTI_INPUTS_EMISSION)
        float   _EmissionBlend;
        float   _EmissionParallaxDepth;
        float   _EmissionFluorescence;
        float   _EmissionGradSpeed;
    #endif
    #if defined(LIL_MULTI_INPUTS_EMISSION_2ND)
        float   _Emission2ndBlend;
        float   _Emission2ndParallaxDepth;
        float   _Emission2ndFluorescence;
        float   _Emission2ndGradSpeed;
    #endif
    #if defined(LIL_MULTI_INPUTS_PARALLAX)
        float   _Parallax;
        float   _ParallaxOffset;
    #endif
    #if defined(LIL_MULTI_INPUTS_AUDIOLINK)
        float   _AudioLink2EmissionGrad;
        float   _AudioLink2Emission2ndGrad;
    #endif
    #if defined(LIL_MULTI_INPUTS_DISSOLVE)
        float   _DissolveNoiseStrength;
    #endif
    #if defined(LIL_MULTI_INPUTS_OUTLINE)
        float   _OutlineWidth;
        float   _OutlineEnableLighting;
        float   _OutlineVectorScale;
    #endif
    #if defined(LIL_FUR)
        float   _FurVectorScale;
        float   _FurGravity;
        float   _FurAO;
        float   _FurRootOffset;
    #endif
    #if defined(LIL_REFRACTION) || defined(LIL_GEM)
        float   _RefractionStrength;
        float   _RefractionFresnelPower;
    #endif
    #if defined(LIL_TESSELLATION)
        float   _TessEdge;
        float   _TessStrength;
        float   _TessShrink;
        float   _TessFactorMax;
    #endif
    #if defined(LIL_GEM)
        float   _GemChromaticAberration;
        float   _GemParticleLoop;
        float   _GemEnvContrast;
        float   _GemVRParallaxStrength;
    #endif
    uint    _Cull;
    #if defined(LIL_MULTI_INPUTS_OUTLINE)
        uint    _OutlineCull;
    #endif
    #if defined(LIL_MULTI_INPUTS_MAIN2ND)
        uint    _Main2ndTexBlendMode;
    #endif
    #if defined(LIL_MULTI_INPUTS_MAIN3RD)
        uint    _Main3rdTexBlendMode;
    #endif
    #if defined(LIL_MULTI_INPUTS_ALPHAMASK)
        uint    _AlphaMaskMode;
    #endif
    #if defined(LIL_MULTI_INPUTS_MATCAP)
        uint    _MatCapBlendMode;
    #endif
    #if defined(LIL_MULTI_INPUTS_MATCAP_2ND)
        uint    _MatCap2ndBlendMode;
    #endif
    #if defined(LIL_MULTI_INPUTS_GLITTER)
        uint    _GlitterUVMode;
    #endif
    #if defined(LIL_MULTI_INPUTS_EMISSION)
        uint    _EmissionMap_UVMode;
    #endif
    #if defined(LIL_MULTI_INPUTS_EMISSION_2ND)
        uint    _Emission2ndMap_UVMode;
    #endif
    #if defined(LIL_MULTI_INPUTS_AUDIOLINK)
        uint    _AudioLinkUVMode;
        uint    _AudioLinkVertexUVMode;
    #endif
    #if defined(LIL_FUR)
        uint    _FurLayerNum;
    #endif
    lilBool _Invisible;
    #if defined(LIL_MULTI_INPUTS_MAIN2ND)
        lilBool _Main2ndTexIsMSDF;
        lilBool _Main2ndTexIsDecal;
        lilBool _Main2ndTexIsLeftOnly;
        lilBool _Main2ndTexIsRightOnly;
        lilBool _Main2ndTexShouldCopy;
        lilBool _Main2ndTexShouldFlipMirror;
        lilBool _Main2ndTexShouldFlipCopy;
    #endif
    #if defined(LIL_MULTI_INPUTS_MAIN3RD)
        lilBool _Main3rdTexIsMSDF;
        lilBool _Main3rdTexIsDecal;
        lilBool _Main3rdTexIsLeftOnly;
        lilBool _Main3rdTexIsRightOnly;
        lilBool _Main3rdTexShouldCopy;
        lilBool _Main3rdTexShouldFlipMirror;
        lilBool _Main3rdTexShouldFlipCopy;
    #endif
    #if defined(LIL_MULTI_INPUTS_SHADOW)
        lilBool _ShadowReceive;
    #endif
    #if defined(LIL_MULTI_INPUTS_BACKLIGHT)
        lilBool _BacklightReceiveShadow;
    #endif
    #if defined(LIL_MULTI_INPUTS_ANISOTROPY)
        lilBool _Anisotropy2Reflection;
        lilBool _Anisotropy2MatCap;
        lilBool _Anisotropy2MatCap2nd;
    #endif
    #if defined(LIL_MULTI_INPUTS_REFLECTION)
        lilBool _ApplySpecular;
        lilBool _ApplySpecularFA;
        lilBool _ApplyReflection;
        lilBool _SpecularToon;
        lilBool _ReflectionApplyTransparency;
    #endif
    #if defined(LIL_MULTI_INPUTS_MATCAP)
        lilBool _MatCapApplyTransparency;
        lilBool _MatCapPerspective;
        lilBool _MatCapZRotCancel;
        lilBool _MatCapCustomNormal;
    #endif
    #if defined(LIL_MULTI_INPUTS_MATCAP_2ND)
        lilBool _MatCap2ndApplyTransparency;
        lilBool _MatCap2ndPerspective;
        lilBool _MatCap2ndZRotCancel;
        lilBool _MatCap2ndCustomNormal;
    #endif
    #if defined(LIL_MULTI_INPUTS_RIM)
        lilBool _RimApplyTransparency;
    #endif
    #if defined(LIL_MULTI_INPUTS_GLITTER)
        lilBool _GlitterApplyTransparency;
    #endif
    #if defined(LIL_MULTI_INPUTS_EMISSION)
        lilBool _EmissionUseGrad;
    #endif
    #if defined(LIL_MULTI_INPUTS_EMISSION_2ND)
        lilBool _Emission2ndUseGrad;
    #endif
    #if defined(LIL_MULTI_INPUTS_AUDIOLINK)
        lilBool _AudioLink2Main2nd;
        lilBool _AudioLink2Main3rd;
        lilBool _AudioLink2Emission;
        lilBool _AudioLink2Emission2nd;
        lilBool _AudioLink2Vertex;
    #endif
    #if defined(LIL_FEATURE_ENCRYPTION)
        lilBool _IgnoreEncryption;
    #endif
    #if defined(LIL_MULTI_INPUTS_OUTLINE)
        lilBool _OutlineFixWidth;
        lilBool _OutlineVertexR2Width;
    #endif
    #if defined(LIL_FUR)
        lilBool _VertexColor2FurVector;
    #endif
    #if defined(LIL_REFRACTION)
        lilBool _RefractionColorFromMain;
    #endif

    #if defined(LIL_CUSTOM_PROPERTIES)
        LIL_CUSTOM_PROPERTIES
    #endif

    CBUFFER_END
#else
    CBUFFER_START(UnityPerMaterial)
    //------------------------------------------------------------------------------------------------------------------------------
    // Vector
    float4  _LightDirectionOverride;
    // Main
    float4  _Color;
    float4  _MainTex_ST;
    #if defined(LIL_FEATURE_ANIMATE_MAIN_UV)
        float4  _MainTex_ScrollRotate;
    #endif
    #if defined(LIL_FEATURE_MAIN_TONE_CORRECTION)
        float4  _MainTexHSVG;
    #endif

    // Main2nd
    #if defined(LIL_FEATURE_MAIN2ND)
        float4  _Color2nd;
        float4  _Main2ndTex_ST;
        float4  _Main2ndDistanceFade;
        #if defined(LIL_FEATURE_DECAL) && defined(LIL_FEATURE_ANIMATE_DECAL)
            float4  _Main2ndTexDecalAnimation;
            float4  _Main2ndTexDecalSubParam;
        #endif
        #if defined(LIL_FEATURE_LAYER_DISSOLVE)
            float4  _Main2ndDissolveMask_ST;
            float4  _Main2ndDissolveColor;
            float4  _Main2ndDissolveParams;
            float4  _Main2ndDissolvePos;
            #if defined(LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE)
                float4  _Main2ndDissolveNoiseMask_ST;
                float4  _Main2ndDissolveNoiseMask_ScrollRotate;
            #endif
        #endif
    #endif

    // Main3rd
    #if defined(LIL_FEATURE_MAIN3RD)
        float4  _Color3rd;
        float4  _Main3rdTex_ST;
        float4  _Main3rdDistanceFade;
        #if defined(LIL_FEATURE_DECAL) && defined(LIL_FEATURE_ANIMATE_DECAL)
            float4  _Main3rdTexDecalAnimation;
            float4  _Main3rdTexDecalSubParam;
        #endif
        #if defined(LIL_FEATURE_LAYER_DISSOLVE)
            float4  _Main3rdDissolveMask_ST;
            float4  _Main3rdDissolveColor;
            float4  _Main3rdDissolveParams;
            float4  _Main3rdDissolvePos;
            #if defined(LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE)
                float4  _Main3rdDissolveNoiseMask_ST;
                float4  _Main3rdDissolveNoiseMask_ScrollRotate;
            #endif
        #endif
    #endif

    // Shadow
    #if defined(LIL_FEATURE_SHADOW)
        float4  _ShadowColor;
        float4  _Shadow2ndColor;
        float4  _ShadowBorderColor;
        float4  _ShadowAOShift;
    #endif

    // Backlight
    #if defined(LIL_FEATURE_BACKLIGHT)
        float4  _BacklightColor;
        float4  _BacklightColorTex_ST;
    #endif

    // Emission
    #if defined(LIL_FEATURE_EMISSION_1ST)
        float4  _EmissionColor;
        float4  _EmissionBlink;
        float4  _EmissionMap_ST;
        #if defined(LIL_FEATURE_ANIMATE_EMISSION_UV)
            float4  _EmissionMap_ScrollRotate;
        #endif
        float4  _EmissionBlendMask_ST;
        #if defined(LIL_FEATURE_ANIMATE_EMISSION_MASK_UV)
            float4  _EmissionBlendMask_ScrollRotate;
        #endif
    #endif

    // Emission 2nd
    #if defined(LIL_FEATURE_EMISSION_2ND)
        float4  _Emission2ndColor;
        float4  _Emission2ndBlink;
        float4  _Emission2ndMap_ST;
        #if defined(LIL_FEATURE_ANIMATE_EMISSION_UV)
            float4  _Emission2ndMap_ScrollRotate;
        #endif
        float4  _Emission2ndBlendMask_ST;
        #if defined(LIL_FEATURE_ANIMATE_EMISSION_MASK_UV)
            float4  _Emission2ndBlendMask_ScrollRotate;
        #endif
    #endif

    // Normal Map
    #if defined(LIL_FEATURE_NORMAL_1ST)
        float4  _BumpMap_ST;
    #endif

    // Normal Map 2nd
    #if defined(LIL_FEATURE_NORMAL_2ND)
        float4  _Bump2ndMap_ST;
        float4  _Bump2ndScaleMask_ST;
    #endif

    // Anisotropy
    #if defined(LIL_FEATURE_ANISOTROPY)
        float4  _AnisotropyTangentMap_ST;
        float4  _AnisotropyScaleMask_ST;
        float4  _AnisotropyShiftNoiseMask_ST;
    #endif

    // Reflection
    #if defined(LIL_FEATURE_REFLECTION)
        float4  _ReflectionColor;
        float4  _SmoothnessTex_ST;
        float4  _MetallicGlossMap_ST;
        float4  _ReflectionColorTex_ST;
    #endif

    // MatCap
    #if defined(LIL_FEATURE_MATCAP)
        float4  _MatCapColor;
        float4  _MatCapTex_ST;
        float4  _MatCapBlendMask_ST;
        float4  _MatCapBlendUV1;
        #if defined(LIL_FEATURE_TEX_MATCAP_NORMALMAP)
            float4  _MatCapBumpMap_ST;
        #endif
    #endif

    // MatCap 2nd
    #if defined(LIL_FEATURE_MATCAP_2ND)
        float4  _MatCap2ndColor;
        float4  _MatCap2ndTex_ST;
        float4  _MatCap2ndBlendMask_ST;
        float4  _MatCap2ndBlendUV1;
        #if defined(LIL_FEATURE_TEX_MATCAP_NORMALMAP)
            float4  _MatCap2ndBumpMap_ST;
        #endif
    #endif

    // Rim Light
    #if defined(LIL_FEATURE_RIMLIGHT)
        float4  _RimColor;
        float4  _RimColorTex_ST;
        #if defined(LIL_FEATURE_RIMLIGHT_DIRECTION)
            float4  _RimIndirColor;
        #endif
    #endif

    // Glitter
    #if defined(LIL_FEATURE_GLITTER)
        float4  _GlitterColor;
        float4  _GlitterColorTex_ST;
        float4  _GlitterParams1;
        float4  _GlitterParams2;
    #endif

    // Distance Fade
    #if defined(LIL_FEATURE_DISTANCE_FADE)
        float4  _DistanceFade;
        float4  _DistanceFadeColor;
    #endif

    // AudioLink
    #if defined(LIL_FEATURE_AUDIOLINK)
        float4  _AudioLinkDefaultValue;
        float4  _AudioLinkUVParams;
        float4  _AudioLinkStart;
        #if defined(LIL_FEATURE_AUDIOLINK_VERTEX)
            float4  _AudioLinkVertexUVParams;
            float4  _AudioLinkVertexStart;
            float4  _AudioLinkVertexStrength;
        #endif
        #if defined(LIL_FEATURE_AUDIOLINK_LOCAL)
            float4  _AudioLinkLocalMapParams;
        #endif
    #endif

    // Dissolve
    #if defined(LIL_FEATURE_DISSOLVE)
        float4  _DissolveMask_ST;
        float4  _DissolveColor;
        float4  _DissolveParams;
        float4  _DissolvePos;
        #if defined(LIL_FEATURE_TEX_DISSOLVE_NOISE)
            float4  _DissolveNoiseMask_ST;
            float4  _DissolveNoiseMask_ScrollRotate;
        #endif
    #endif

    // Encryption
    #if defined(LIL_FEATURE_ENCRYPTION)
        float4  _Keys;
    #endif

    // Outline
    float4  _OutlineColor;
    float4  _OutlineTex_ST;
    #if defined(LIL_FEATURE_ANIMATE_OUTLINE_UV)
        float4  _OutlineTex_ScrollRotate;
    #endif
    #if defined(LIL_FEATURE_TEX_OUTLINE_COLOR)
        #if defined(LIL_FEATURE_OUTLINE_TONE_CORRECTION)
            float4  _OutlineTexHSVG;
        #endif
    #endif

    // Fur
    #if defined(LIL_FUR)
        float4  _FurNoiseMask_ST;
        float4  _FurVector;
    #endif

    // Refraction
    #if defined(LIL_REFRACTION)
        float4  _RefractionColor;
    #endif

    // Gem
    #if defined(LIL_GEM)
        float4  _GemParticleColor;
        float4  _GemEnvColor;
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Float
    float   _AsUnlit;
    float   _Cutoff;
    float   _SubpassCutoff;
    float   _FlipNormal;
    float   _ShiftBackfaceUV;
    float   _VertexLightStrength;
    float   _LightMinLimit;
    float   _LightMaxLimit;
    float   _BeforeExposureLimit;
    float   _MonochromeLighting;
    #if defined(LIL_HDRP)
        float   _lilDirectionalLightStrength;
    #endif
    #if defined(LIL_FEATURE_MAIN_GRADATION_MAP)
        float   _MainGradationStrength;
    #endif
    #if defined(LIL_FEATURE_MAIN2ND)
        float   _Main2ndTexAngle;
        float   _Main2ndEnableLighting;
        #if defined(LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE)
            float   _Main2ndDissolveNoiseStrength;
        #endif
    #endif
    #if defined(LIL_FEATURE_MAIN3RD)
        float   _Main3rdTexAngle;
        float   _Main3rdEnableLighting;
        #if defined(LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE)
            float   _Main3rdDissolveNoiseStrength;
        #endif
    #endif
    #if defined(LIL_FEATURE_ALPHAMASK)
        float   _AlphaMaskScale;
        float   _AlphaMaskValue;
    #endif
    #if defined(LIL_FEATURE_SHADOW)
        float   _BackfaceForceShadow;
        float   _ShadowStrength;
        float   _ShadowNormalStrength;
        float   _ShadowBorder;
        float   _ShadowBlur;
        float   _Shadow2ndNormalStrength;
        float   _Shadow2ndBorder;
        float   _Shadow2ndBlur;
        float   _ShadowMainStrength;
        float   _ShadowEnvStrength;
        float   _ShadowBorderRange;
    #endif
    #if defined(LIL_FEATURE_BACKLIGHT)
        float   _BacklightNormalStrength;
        float   _BacklightBorder;
        float   _BacklightBlur;
        float   _BacklightDirectivity;
        float   _BacklightViewStrength;
        float   _BacklightBackfaceMask;
    #endif
    #if defined(LIL_FEATURE_NORMAL_1ST)
        float   _BumpScale;
    #endif
    #if defined(LIL_FEATURE_NORMAL_2ND)
        float   _Bump2ndScale;
    #endif
    #if defined(LIL_FEATURE_ANISOTROPY)
        float   _AnisotropyScale;
        float   _AnisotropyTangentWidth;
        float   _AnisotropyBitangentWidth;
        float   _AnisotropyShift;
        float   _AnisotropyShiftNoiseScale;
        float   _AnisotropySpecularStrength;
        float   _Anisotropy2ndTangentWidth;
        float   _Anisotropy2ndBitangentWidth;
        float   _Anisotropy2ndShift;
        float   _Anisotropy2ndShiftNoiseScale;
        float   _Anisotropy2ndSpecularStrength;
    #endif
    #if defined(LIL_FEATURE_REFLECTION) || defined(LIL_GEM)
        float   _Smoothness;
        float   _Reflectance;
        float   _SpecularNormalStrength;
        float   _SpecularBorder;
        float   _SpecularBlur;
        float   _ReflectionNormalStrength;
    #endif
    #if defined(LIL_FEATURE_REFLECTION)
        float   _Metallic;
    #endif
    #if defined(LIL_FEATURE_MATCAP)
        float   _MatCapBlend;
        float   _MatCapEnableLighting;
        float   _MatCapShadowMask;
        float   _MatCapVRParallaxStrength;
        float   _MatCapBackfaceMask;
        float   _MatCapLod;
        float   _MatCapNormalStrength;
        #if defined(LIL_FEATURE_TEX_MATCAP_NORMALMAP)
            float   _MatCapBumpScale;
        #endif
    #endif
    #if defined(LIL_FEATURE_MATCAP_2ND)
        float   _MatCap2ndBlend;
        float   _MatCap2ndEnableLighting;
        float   _MatCap2ndShadowMask;
        float   _MatCap2ndVRParallaxStrength;
        float   _MatCap2ndBackfaceMask;
        float   _MatCap2ndLod;
        float   _MatCap2ndNormalStrength;
        #if defined(LIL_FEATURE_TEX_MATCAP_NORMALMAP)
            float   _MatCap2ndBumpScale;
        #endif
    #endif
    #if defined(LIL_FEATURE_RIMLIGHT)
        float   _RimNormalStrength;
        float   _RimBorder;
        float   _RimBlur;
        float   _RimFresnelPower;
        float   _RimEnableLighting;
        float   _RimShadowMask;
        float   _RimVRParallaxStrength;
        float   _RimBackfaceMask;
        #if defined(LIL_FEATURE_RIMLIGHT_DIRECTION)
            float   _RimDirStrength;
            float   _RimDirRange;
            float   _RimIndirRange;
            float   _RimIndirBorder;
            float   _RimIndirBlur;
        #endif
    #endif
    #if defined(LIL_FEATURE_GLITTER)
        float   _GlitterMainStrength;
        float   _GlitterNormalStrength;
        float   _GlitterEnableLighting;
        float   _GlitterShadowMask;
        float   _GlitterVRParallaxStrength;
        float   _GlitterBackfaceMask;
    #endif
    #if defined(LIL_FEATURE_EMISSION_1ST)
        float   _EmissionBlend;
        float   _EmissionParallaxDepth;
        float   _EmissionFluorescence;
        #if defined(LIL_FEATURE_EMISSION_GRADATION)
            float   _EmissionGradSpeed;
        #endif
    #endif
    #if defined(LIL_FEATURE_EMISSION_2ND)
        float   _Emission2ndBlend;
        float   _Emission2ndParallaxDepth;
        float   _Emission2ndFluorescence;
        #if defined(LIL_FEATURE_EMISSION_GRADATION)
            float   _Emission2ndGradSpeed;
        #endif
    #endif
    #if defined(LIL_FEATURE_PARALLAX)
        float   _Parallax;
        float   _ParallaxOffset;
    #endif
    #if defined(LIL_FEATURE_AUDIOLINK)
        float   _AudioLink2EmissionGrad;
        float   _AudioLink2Emission2ndGrad;
    #endif
    #if defined(LIL_FEATURE_DISSOLVE) &&  defined(LIL_FEATURE_TEX_DISSOLVE_NOISE)
        float   _DissolveNoiseStrength;
    #endif

    float   _OutlineWidth;
    float   _OutlineEnableLighting;
    float   _OutlineVectorScale;

    #if defined(LIL_FUR)
        float   _FurVectorScale;
        float   _FurGravity;
        float   _FurAO;
        float   _FurRootOffset;
    #endif
    #if defined(LIL_REFRACTION)
        float   _RefractionStrength;
        float   _RefractionFresnelPower;
    #endif
    #if defined(LIL_TESSELLATION)
        float   _TessEdge;
        float   _TessStrength;
        float   _TessShrink;
        float   _TessFactorMax;
    #endif
    #if defined(LIL_GEM)
        float   _GemChromaticAberration;
        float   _GemParticleLoop;
        float   _RefractionStrength;
        float   _RefractionFresnelPower;
        float   _GemEnvContrast;
        float   _GemVRParallaxStrength;
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Int
    uint    _Cull;
    uint    _OutlineCull;
    #if defined(LIL_FEATURE_MAIN2ND)
        uint    _Main2ndTexBlendMode;
    #endif
    #if defined(LIL_FEATURE_MAIN3RD)
        uint    _Main3rdTexBlendMode;
    #endif
    #if defined(LIL_FEATURE_ALPHAMASK)
        uint    _AlphaMaskMode;
    #endif
    #if defined(LIL_FEATURE_MATCAP)
        uint    _MatCapBlendMode;
    #endif
    #if defined(LIL_FEATURE_MATCAP_2ND)
        uint    _MatCap2ndBlendMode;
    #endif
    #if defined(LIL_FEATURE_GLITTER)
        uint    _GlitterUVMode;
    #endif
    #if defined(LIL_FEATURE_EMISSION_1ST)
        uint    _EmissionMap_UVMode;
    #endif
    #if defined(LIL_FEATURE_EMISSION_2ND)
        uint    _Emission2ndMap_UVMode;
    #endif
    #if defined(LIL_FEATURE_AUDIOLINK)
        uint    _AudioLinkUVMode;
        #if defined(LIL_FEATURE_AUDIOLINK_VERTEX)
            uint    _AudioLinkVertexUVMode;
        #endif
    #endif
    #if defined(LIL_FUR)
        uint    _FurLayerNum;
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Bool
    lilBool _Invisible;
    #if defined(LIL_FEATURE_MAIN2ND)
        lilBool _UseMain2ndTex;
        lilBool _Main2ndTexIsMSDF;
        #if defined(LIL_FEATURE_DECAL)
            lilBool _Main2ndTexIsDecal;
            lilBool _Main2ndTexIsLeftOnly;
            lilBool _Main2ndTexIsRightOnly;
            lilBool _Main2ndTexShouldCopy;
            lilBool _Main2ndTexShouldFlipMirror;
            lilBool _Main2ndTexShouldFlipCopy;
        #endif
    #endif
    #if defined(LIL_FEATURE_MAIN3RD)
        lilBool _UseMain3rdTex;
        lilBool _Main3rdTexIsMSDF;
        #if defined(LIL_FEATURE_DECAL)
            lilBool _Main3rdTexIsDecal;
            lilBool _Main3rdTexIsLeftOnly;
            lilBool _Main3rdTexIsRightOnly;
            lilBool _Main3rdTexShouldCopy;
            lilBool _Main3rdTexShouldFlipMirror;
            lilBool _Main3rdTexShouldFlipCopy;
        #endif
    #endif
    #if defined(LIL_FEATURE_SHADOW)
        lilBool _UseShadow;
        #if defined(LIL_FEATURE_RECEIVE_SHADOW)
            lilBool _ShadowReceive;
        #endif
    #endif
    #if defined(LIL_FEATURE_BACKLIGHT)
        lilBool _UseBacklight;
        lilBool _BacklightReceiveShadow;
    #endif
    #if defined(LIL_FEATURE_NORMAL_1ST)
        lilBool _UseBumpMap;
    #endif
    #if defined(LIL_FEATURE_NORMAL_2ND)
        lilBool _UseBump2ndMap;
    #endif
    #if defined(LIL_FEATURE_ANISOTROPY)
        lilBool _UseAnisotropy;
        lilBool _Anisotropy2Reflection;
        lilBool _Anisotropy2MatCap;
        lilBool _Anisotropy2MatCap2nd;
    #endif
    #if defined(LIL_FEATURE_REFLECTION)
        lilBool _UseReflection;
        lilBool _ApplySpecular;
        lilBool _ApplySpecularFA;
        lilBool _ApplyReflection;
        lilBool _SpecularToon;
        lilBool _ReflectionApplyTransparency;
    #endif
    #if defined(LIL_FEATURE_MATCAP)
        lilBool _UseMatCap;
        lilBool _MatCapApplyTransparency;
        lilBool _MatCapPerspective;
        lilBool _MatCapZRotCancel;
        #if defined(LIL_FEATURE_TEX_MATCAP_NORMALMAP)
            lilBool _MatCapCustomNormal;
        #endif
    #endif
    #if defined(LIL_FEATURE_MATCAP_2ND)
        lilBool _UseMatCap2nd;
        lilBool _MatCap2ndApplyTransparency;
        lilBool _MatCap2ndPerspective;
        lilBool _MatCap2ndZRotCancel;
        #if defined(LIL_FEATURE_TEX_MATCAP_NORMALMAP)
            lilBool _MatCap2ndCustomNormal;
        #endif
    #endif
    #if defined(LIL_FEATURE_RIMLIGHT)
        lilBool _UseRim;
        lilBool _RimApplyTransparency;
    #endif
    #if defined(LIL_FEATURE_GLITTER)
        lilBool _UseGlitter;
        lilBool _GlitterApplyTransparency;
    #endif
    #if defined(LIL_FEATURE_EMISSION_1ST)
        lilBool _UseEmission;
        #if defined(LIL_FEATURE_EMISSION_GRADATION)
            lilBool _EmissionUseGrad;
        #endif
    #endif
    #if defined(LIL_FEATURE_EMISSION_2ND)
        lilBool _UseEmission2nd;
        #if defined(LIL_FEATURE_EMISSION_GRADATION)
            lilBool _Emission2ndUseGrad;
        #endif
    #endif
    #if defined(LIL_FEATURE_PARALLAX)
        lilBool _UseParallax;
    #endif
    #if defined(LIL_FEATURE_AUDIOLINK)
        lilBool _UseAudioLink;
        #if defined(LIL_FEATURE_MAIN2ND)
            lilBool _AudioLink2Main2nd;
        #endif
        #if defined(LIL_FEATURE_MAIN3RD)
            lilBool _AudioLink2Main3rd;
        #endif
        #if defined(LIL_FEATURE_EMISSION_1ST)
            lilBool _AudioLink2Emission;
        #endif
        #if defined(LIL_FEATURE_EMISSION_2ND)
            lilBool _AudioLink2Emission2nd;
        #endif
        #if defined(LIL_FEATURE_AUDIOLINK_VERTEX)
            lilBool _AudioLink2Vertex;
        #endif
        #if defined(LIL_FEATURE_AUDIOLINK_LOCAL)
            lilBool _AudioLinkAsLocal;
        #endif
    #endif
    #if defined(LIL_FEATURE_ENCRYPTION)
        lilBool _IgnoreEncryption;
    #endif

    lilBool _OutlineFixWidth;
    lilBool _OutlineVertexR2Width;

    #if defined(LIL_FUR)
        lilBool _VertexColor2FurVector;
    #endif
    #if defined(LIL_REFRACTION)
        lilBool _RefractionColorFromMain;
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Custom properties
    #if defined(LIL_CUSTOM_PROPERTIES)
        LIL_CUSTOM_PROPERTIES
    #endif

    CBUFFER_END
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Texture
TEXTURE2D(_MainTex);
TEXTURE2D(_MainGradationTex);
TEXTURE2D(_MainColorAdjustMask);
TEXTURE2D(_Main2ndTex);
TEXTURE2D(_Main2ndBlendMask);
TEXTURE2D(_Main2ndDissolveMask);
TEXTURE2D(_Main2ndDissolveNoiseMask);
TEXTURE2D(_Main3rdTex);
TEXTURE2D(_Main3rdBlendMask);
TEXTURE2D(_Main3rdDissolveMask);
TEXTURE2D(_Main3rdDissolveNoiseMask);
TEXTURE2D(_AlphaMask);
TEXTURE2D(_BumpMap);
TEXTURE2D(_Bump2ndMap);
TEXTURE2D(_Bump2ndScaleMask);
TEXTURE2D(_AnisotropyTangentMap);
TEXTURE2D(_AnisotropyScaleMask);
TEXTURE2D(_AnisotropyShiftNoiseMask);
TEXTURE2D(_ShadowBorderMask);
TEXTURE2D(_ShadowBlurMask);
TEXTURE2D(_ShadowStrengthMask);
TEXTURE2D(_ShadowColorTex);
TEXTURE2D(_Shadow2ndColorTex);
TEXTURE2D(_BacklightColorTex);
TEXTURE2D(_SmoothnessTex);
TEXTURE2D(_MetallicGlossMap);
TEXTURE2D(_ReflectionColorTex);
TEXTURE2D(_MatCapTex);
TEXTURE2D(_MatCapBlendMask);
TEXTURE2D(_MatCapBumpMap);
TEXTURE2D(_MatCap2ndTex);
TEXTURE2D(_MatCap2ndBlendMask);
TEXTURE2D(_MatCap2ndBumpMap);
TEXTURE2D(_RimColorTex);
TEXTURE2D(_GlitterColorTex);
TEXTURE2D(_EmissionMap);
TEXTURE2D(_EmissionBlendMask);
TEXTURE2D(_EmissionGradTex);
TEXTURE2D(_Emission2ndMap);
TEXTURE2D(_Emission2ndBlendMask);
TEXTURE2D(_Emission2ndGradTex);
TEXTURE2D(_ParallaxMap);
TEXTURE2D(_AudioLinkMask);
TEXTURE2D(_AudioLinkLocalMap);
TEXTURE2D(_DissolveMask);
TEXTURE2D(_DissolveNoiseMask);
TEXTURE2D(_OutlineTex);
TEXTURE2D(_OutlineWidthMask);
TEXTURE2D(_OutlineVectorTex);
TEXTURE2D(_FurNoiseMask);
TEXTURE2D(_FurMask);
TEXTURE2D(_FurLengthMask);
TEXTURE2D(_FurVectorTex);
TEXTURE2D(_TriMask);
SAMPLER(sampler_MainTex);
SAMPLER(sampler_Main2ndTex);
SAMPLER(sampler_Main3rdTex);
SAMPLER(sampler_EmissionMap);
SAMPLER(sampler_Emission2ndMap);
SAMPLER(sampler_OutlineTex);

// AudioLink
#if defined(LIL_FEATURE_AUDIOLINK)
TEXTURE2D_FLOAT(_AudioTexture);
float4 _AudioTexture_TexelSize;
#endif

#if defined(LIL_OUTLINE)
    #define sampler_MainTex sampler_OutlineTex
#endif
#if !defined(LIL_FEATURE_TEX_OUTLINE_COLOR)
    #define sampler_OutlineTex sampler_linear_repeat
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Custom properties
#if defined(LIL_CUSTOM_TEXTURES)
    LIL_CUSTOM_TEXTURES
#endif

#endif