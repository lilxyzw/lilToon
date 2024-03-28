#ifndef LIL_REPLACE_KEYWORDS_INCLUDED
#define LIL_REPLACE_KEYWORDS_INCLUDED

//----------------------------------------------------------------------------------------------------------------------
// AvatarEncryption
//#define LIL_FEATURE_ENCRYPTION

//----------------------------------------------------------------------------------------------------------------------
// Ignore shader setting
#define LIL_IGNORE_SHADERSETTING

//----------------------------------------------------------------------------------------------------------------------
// Shader keyword list

// Built-in keyword                     Replace
// ------------------------------------ --------------------------------------------------------------------------------
// UNITY_UI_ALPHACLIP                   LIL_RENDER 1
// UNITY_UI_CLIP_RECT                   LIL_RENDER 2
// ------------------------------------ --------------------------------------------------------------------------------
// EFFECT_HUE_VARIATION                 LIL_FEATURE_MAIN_GRADATION_MAP LIL_FEATURE_MAIN_TONE_CORRECTION
// _COLORADDSUBDIFF_ON                  LIL_FEATURE_MAIN2ND
// _COLORCOLOR_ON                       LIL_FEATURE_MAIN3RD
// _SUNDISK_NONE                        LIL_FEATURE_ANIMATE_DECAL
// GEOM_TYPE_FROND                      LIL_FEATURE_LAYER_DISSOLVE
// _COLOROVERLAY_ON                     LIL_FEATURE_ALPHAMASK
// ------------------------------------ --------------------------------------------------------------------------------
// _REQUIRE_UV2                         LIL_FEATURE_SHADOW
// AUTO_KEY_VALUE                       LIL_FEATURE_RIMSHADE
// ANTI_FLICKER                         LIL_FEATURE_BACKLIGHT
// _EMISSION                            LIL_FEATURE_EMISSION_1ST
// GEOM_TYPE_BRANCH                     LIL_FEATURE_EMISSION_2ND
// _SUNDISK_SIMPLE                      LIL_FEATURE_EmissionBlendMask LIL_FEATURE_Emission2ndBlendMask
// ------------------------------------ --------------------------------------------------------------------------------
// _NORMALMAP                           LIL_FEATURE_NORMAL_1ST
// EFFECT_BUMP                          LIL_FEATURE_NORMAL_2ND
// SOURCE_GBUFFER                       LIL_FEATURE_ANISOTROPY
// _GLOSSYREFLECTIONS_OFF               LIL_FEATURE_REFLECTION
// _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A LIL_FEATURE_MATCAP
// _SPECULARHIGHLIGHTS_OFF              LIL_FEATURE_MATCAP_2ND
// GEOM_TYPE_MESH                       LIL_FEATURE_MatCapBumpMap LIL_FEATURE_MatCap2ndBumpMap
// _METALLICGLOSSMAP                    LIL_FEATURE_RIMLIGHT
// GEOM_TYPE_LEAF                       LIL_FEATURE_RIMLIGHT_DIRECTION
// _SPECGLOSSMAP                        LIL_FEATURE_GLITTER
// ------------------------------------ --------------------------------------------------------------------------------
// _PARALLAXMAP                         LIL_FEATURE_PARALLAX
// PIXELSNAP_ON                         LIL_FEATURE_POM
// _FADING_ON                           LIL_FEATURE_DISTANCE_FADE
// _MAPPING_6_FRAMES_LAYOUT             LIL_FEATURE_AUDIOLINK
// _SUNDISK_HIGH_QUALITY                LIL_FEATURE_AUDIOLINK_LOCAL
// GEOM_TYPE_BRANCH_DETAIL              LIL_FEATURE_DISSOLVE
// ETC1_EXTERNAL_ALPHA                  LIL_FEATURE_DITHER
// ------------------------------------ --------------------------------------------------------------------------------
// _DETAIL_MULX2                        LIL_FEATURE_OUTLINE_TONE_CORRECTION


// removed
// BILLBOARD_FACE_CAMERA_POS            LIL_FEATURE_CLIPPING_CANCELLER
// ETC1_EXTERNAL_ALPHA                  LIL_MULTI_OUTLINE

//----------------------------------------------------------------------------------------------------------------------
// Replace keyword to transparent mode
#if defined(UNITY_UI_CLIP_RECT) || defined(LIL_REFRACTION)
    #define LIL_RENDER 2
#elif defined(UNITY_UI_ALPHACLIP) || defined(LIL_FUR)
    #define LIL_RENDER 1
#else
    #define LIL_RENDER 0
#endif

#if defined(UNITY_UI_CLIP_RECT)
    #undef UNITY_UI_CLIP_RECT
#endif

#if defined(UNITY_UI_ALPHACLIP)
    #undef UNITY_UI_ALPHACLIP
#endif

//----------------------------------------------------------------------------------------------------------------------
// Replace keyword to shader setting
#if defined(EFFECT_HUE_VARIATION)
    #define LIL_FEATURE_MAIN_TONE_CORRECTION
    #define LIL_FEATURE_MAIN_GRADATION_MAP
    #undef EFFECT_HUE_VARIATION
#endif

#if defined(_COLORADDSUBDIFF_ON)
    #define LIL_FEATURE_MAIN2ND
    #undef _COLORADDSUBDIFF_ON
#endif

#if defined(_COLORCOLOR_ON)
    #define LIL_FEATURE_MAIN3RD
    #undef _COLORCOLOR_ON
#endif

#if defined(_SUNDISK_NONE)
    #define LIL_FEATURE_ANIMATE_DECAL
    #undef _SUNDISK_NONE
#endif

#if defined(GEOM_TYPE_FROND)
    #define LIL_FEATURE_LAYER_DISSOLVE
    #undef GEOM_TYPE_FROND
#endif

#if defined(_COLOROVERLAY_ON)
    #define LIL_FEATURE_ALPHAMASK
    #undef _COLOROVERLAY_ON
#endif

#if defined(_REQUIRE_UV2)
    #define LIL_FEATURE_SHADOW
    #undef _REQUIRE_UV2
#endif

#if defined(AUTO_KEY_VALUE)
    #define LIL_FEATURE_RIMSHADE
    #undef AUTO_KEY_VALUE
#endif

#if defined(ANTI_FLICKER)
    #define LIL_FEATURE_BACKLIGHT
    #undef ANTI_FLICKER
#endif

#if defined(_EMISSION)
    #define LIL_FEATURE_EMISSION_1ST
    #undef _EMISSION
#endif

#if defined(GEOM_TYPE_BRANCH)
    #define LIL_FEATURE_EMISSION_2ND
    #undef GEOM_TYPE_BRANCH
#endif

#if defined(_SUNDISK_SIMPLE)
    #define LIL_FEATURE_EmissionBlendMask
    #define LIL_FEATURE_Emission2ndBlendMask
    #undef _SUNDISK_SIMPLE
#endif

#if defined(_NORMALMAP)
    #define LIL_FEATURE_NORMAL_1ST
    #undef _NORMALMAP
#endif

#if defined(EFFECT_BUMP)
    #define LIL_FEATURE_NORMAL_2ND
    #undef EFFECT_BUMP
#endif

#if defined(SOURCE_GBUFFER)
    #define LIL_FEATURE_ANISOTROPY
    #undef SOURCE_GBUFFER
#endif

#if defined(_GLOSSYREFLECTIONS_OFF)
    #define LIL_FEATURE_REFLECTION
    #undef _GLOSSYREFLECTIONS_OFF
#endif

#if defined(_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A)
    #define LIL_FEATURE_MATCAP
    #undef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
#endif

#if defined(_SPECULARHIGHLIGHTS_OFF)
    #define LIL_FEATURE_MATCAP_2ND
    #undef _SPECULARHIGHLIGHTS_OFF
#endif

#if defined(GEOM_TYPE_MESH)
    #define LIL_FEATURE_MatCapBumpMap
    #define LIL_FEATURE_MatCap2ndBumpMap
    #undef GEOM_TYPE_MESH
#endif

#if defined(_METALLICGLOSSMAP)
    #define LIL_FEATURE_RIMLIGHT
    #undef _METALLICGLOSSMAP
#endif

#if defined(GEOM_TYPE_LEAF)
    #define LIL_FEATURE_RIMLIGHT_DIRECTION
    #undef GEOM_TYPE_LEAF
#endif

#if defined(_SPECGLOSSMAP)
    #define LIL_FEATURE_GLITTER
    #undef _SPECGLOSSMAP
#endif

#if defined(_PARALLAXMAP)
    #define LIL_FEATURE_PARALLAX
    #undef _PARALLAXMAP
#endif

#if defined(PIXELSNAP_ON)
    #define LIL_FEATURE_POM
    #undef PIXELSNAP_ON
#endif

#if defined(_FADING_ON)
    #define LIL_FEATURE_DISTANCE_FADE
    #undef _FADING_ON
#endif

#if defined(_MAPPING_6_FRAMES_LAYOUT)
    #define LIL_FEATURE_AUDIOLINK
    #undef _MAPPING_6_FRAMES_LAYOUT
#endif

#if defined(_SUNDISK_HIGH_QUALITY)
    #define LIL_FEATURE_AUDIOLINK_LOCAL
    #undef _SUNDISK_HIGH_QUALITY
#endif

#if defined(GEOM_TYPE_BRANCH_DETAIL)
    #define LIL_FEATURE_DISSOLVE
    #undef GEOM_TYPE_BRANCH_DETAIL
#endif

#if defined(ETC1_EXTERNAL_ALPHA)
    #define LIL_FEATURE_DITHER
    #undef ETC1_EXTERNAL_ALPHA
#endif

#if defined(_DETAIL_MULX2)
    #define LIL_FEATURE_OUTLINE_TONE_CORRECTION
    #undef _DETAIL_MULX2
#endif

//----------------------------------------------------------------------------------------------------------------------
// Always defined keywords
#define LIL_FEATURE_ANIMATE_MAIN_UV
#define LIL_FEATURE_DECAL
#define LIL_FEATURE_SHADOW_3RD
#define LIL_FEATURE_SHADOW_LUT
#define LIL_FEATURE_RECEIVE_SHADOW
#define LIL_FEATURE_EMISSION_UV
#define LIL_FEATURE_ANIMATE_EMISSION_UV
#define LIL_FEATURE_EMISSION_MASK_UV
#define LIL_FEATURE_ANIMATE_EMISSION_MASK_UV
#define LIL_FEATURE_EMISSION_GRADATION
#define LIL_FEATURE_AUDIOLINK_VERTEX
#define LIL_FEATURE_CLIPPING_CANCELLER
#define LIL_FEATURE_IDMASK
#define LIL_FEATURE_UDIMDISCARD
#define LIL_FEATURE_ANIMATE_OUTLINE_UV
#define LIL_FEATURE_OUTLINE_RECEIVE_SHADOW
#define LIL_FEATURE_FUR_COLLISION

#define LIL_FEATURE_MainGradationTex
#define LIL_FEATURE_MainColorAdjustMask
#define LIL_FEATURE_Main2ndTex
#define LIL_FEATURE_Main2ndBlendMask
#define LIL_FEATURE_Main2ndDissolveMask
#define LIL_FEATURE_Main2ndDissolveNoiseMask
#define LIL_FEATURE_Main3rdTex
#define LIL_FEATURE_Main3rdBlendMask
#define LIL_FEATURE_Main3rdDissolveMask
#define LIL_FEATURE_Main3rdDissolveNoiseMask
#define LIL_FEATURE_AlphaMask
#define LIL_FEATURE_BumpMap
#define LIL_FEATURE_Bump2ndMap
#define LIL_FEATURE_Bump2ndScaleMask
#define LIL_FEATURE_AnisotropyTangentMap
#define LIL_FEATURE_AnisotropyScaleMask
#define LIL_FEATURE_AnisotropyShiftNoiseMask
#define LIL_FEATURE_ShadowBorderMask
#define LIL_FEATURE_ShadowBlurMask
#define LIL_FEATURE_ShadowStrengthMask
#define LIL_FEATURE_ShadowColorTex
#define LIL_FEATURE_Shadow2ndColorTex
#define LIL_FEATURE_Shadow3rdColorTex
#define LIL_FEATURE_RimShadeMask
#define LIL_FEATURE_BacklightColorTex
#define LIL_FEATURE_SmoothnessTex
#define LIL_FEATURE_MetallicGlossMap
#define LIL_FEATURE_ReflectionColorTex
#define LIL_FEATURE_ReflectionCubeTex
#define LIL_FEATURE_MatCapTex
#define LIL_FEATURE_MatCapBlendMask
#define LIL_FEATURE_MatCapBumpMap
#define LIL_FEATURE_MatCap2ndTex
#define LIL_FEATURE_MatCap2ndBlendMask
#define LIL_FEATURE_MatCap2ndBumpMap
#define LIL_FEATURE_RimColorTex
#define LIL_FEATURE_GlitterColorTex
#define LIL_FEATURE_GlitterShapeTex
#define LIL_FEATURE_EmissionMap
#define LIL_FEATURE_EmissionBlendMask
#define LIL_FEATURE_EmissionGradTex
#define LIL_FEATURE_Emission2ndMap
#define LIL_FEATURE_Emission2ndBlendMask
#define LIL_FEATURE_Emission2ndGradTex
#define LIL_FEATURE_ParallaxMap
#define LIL_FEATURE_AudioLinkMask
#define LIL_FEATURE_AudioLinkLocalMap
#define LIL_FEATURE_DissolveMask
#define LIL_FEATURE_DissolveNoiseMask
#define LIL_FEATURE_OutlineTex
#define LIL_FEATURE_OutlineWidthMask
#define LIL_FEATURE_OutlineVectorTex
#define LIL_FEATURE_FurNoiseMask
#define LIL_FEATURE_FurMask
#define LIL_FEATURE_FurLengthMask
#define LIL_FEATURE_FurVectorTex
#endif