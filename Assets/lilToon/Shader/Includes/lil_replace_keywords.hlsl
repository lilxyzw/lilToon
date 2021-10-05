#ifndef LIL_REPLACE_KEYWORDS_INCLUDED
#define LIL_REPLACE_KEYWORDS_INCLUDED

//----------------------------------------------------------------------------------------------------------------------
// AvatarEncryption
//#define LIL_FEATURE_ENCRYPTION

//----------------------------------------------------------------------------------------------------------------------
// Ignore shader setting
#define LIL_IGNORE_SHADERSETTING

//----------------------------------------------------------------------------------------------------------------------
// Replace keyword to transparent mode and outline
#if defined(ETC1_EXTERNAL_ALPHA)
    #define LIL_MULTI_OUTLINE
    #undef ETC1_EXTERNAL_ALPHA
#endif

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
    #define LIL_FEATURE_TEX_EMISSION_MASK
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
    #define LIL_FEATURE_TEX_MATCAP_NORMALMAP
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

#if defined(BILLBOARD_FACE_CAMERA_POS)
    #define LIL_FEATURE_CLIPPING_CANCELLER
    #undef BILLBOARD_FACE_CAMERA_POS
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

#if defined(_DETAIL_MULX2)
    #define LIL_FEATURE_OUTLINE_TONE_CORRECTION
    #undef _DETAIL_MULX2
#endif

//----------------------------------------------------------------------------------------------------------------------
// Always defined keywords
#define LIL_FEATURE_ANIMATE_MAIN_UV
#define LIL_FEATURE_DECAL
#define LIL_FEATURE_TEX_LAYER_MASK
#define LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE
#define LIL_FEATURE_RECEIVE_SHADOW
#define LIL_FEATURE_TEX_SHADOW_BLUR
#define LIL_FEATURE_TEX_SHADOW_BORDER
#define LIL_FEATURE_TEX_SHADOW_STRENGTH
#define LIL_FEATURE_TEX_SHADOW_1ST
#define LIL_FEATURE_TEX_SHADOW_2ND
#define LIL_FEATURE_EMISSION_UV
#define LIL_FEATURE_ANIMATE_EMISSION_UV
#define LIL_FEATURE_EMISSION_MASK_UV
#define LIL_FEATURE_ANIMATE_EMISSION_MASK_UV
#define LIL_FEATURE_EMISSION_GRADATION
#define LIL_FEATURE_TEX_NORMAL_MASK
#define LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS
#define LIL_FEATURE_TEX_REFLECTION_METALLIC
#define LIL_FEATURE_TEX_REFLECTION_COLOR
#define LIL_FEATURE_TEX_MATCAP_MASK
#define LIL_FEATURE_TEX_RIMLIGHT_COLOR
#define LIL_FEATURE_AUDIOLINK_VERTEX
#define LIL_FEATURE_TEX_AUDIOLINK_MASK
#define LIL_FEATURE_TEX_DISSOLVE_NOISE
#define LIL_FEATURE_TEX_OUTLINE_COLOR
#define LIL_FEATURE_ANIMATE_OUTLINE_UV
#define LIL_FEATURE_TEX_OUTLINE_WIDTH
#define LIL_FEATURE_TEX_FUR_NORMAL
#define LIL_FEATURE_TEX_FUR_MASK
#define LIL_FEATURE_TEX_FUR_LENGTH

#endif