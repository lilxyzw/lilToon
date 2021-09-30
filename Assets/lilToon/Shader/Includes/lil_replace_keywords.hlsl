#ifndef LIL_REPLACE_KEYWORDS_INCLUDED
#define LIL_REPLACE_KEYWORDS_INCLUDED

//----------------------------------------------------------------------------------------------------------------------
// Ignore shader setting
#define LIL_IGNORE_SHADERSETTING

//----------------------------------------------------------------------------------------------------------------------
// Replace keyword to transparent mode and outline
#if defined(ETC1_EXTERNAL_ALPHA)
    #define LIL_MULTI_OUTLINE
#endif

#if defined(UNITY_UI_ALPHACLIP)
    #define LIL_RENDER 1
#elif defined(UNITY_UI_CLIP_RECT)
    #define LIL_RENDER 2
#else
    #define LIL_RENDER 0
#endif

//----------------------------------------------------------------------------------------------------------------------
// Replace keyword to shader setting
#if defined(EFFECT_HUE_VARIATION)
    #define LIL_FEATURE_MAIN_TONE_CORRECTION
    #define LIL_FEATURE_MAIN_GRADATION_MAP
#endif

#if defined(_COLORADDSUBDIFF_ON)
    #define LIL_FEATURE_MAIN2ND
#endif

#if defined(_COLORCOLOR_ON)
    #define LIL_FEATURE_MAIN3RD
#endif

#if defined(_SUNDISK_NONE)
    #define LIL_FEATURE_ANIMATE_DECAL
#endif

#if defined(GEOM_TYPE_FROND)
    #define LIL_FEATURE_LAYER_DISSOLVE
#endif

#if defined(_COLOROVERLAY_ON)
    #define LIL_FEATURE_ALPHAMASK
#endif

#if defined(_REQUIRE_UV2)
    #define LIL_FEATURE_SHADOW
#endif

#if defined(_EMISSION)
    #define LIL_FEATURE_EMISSION_1ST
#endif

#if defined(GEOM_TYPE_BRANCH)
    #define LIL_FEATURE_EMISSION_2ND
#endif

#if defined(_SUNDISK_SIMPLE)
    #define LIL_FEATURE_TEX_EMISSION_MASK
#endif

#if defined(_NORMALMAP)
    #define LIL_FEATURE_NORMAL_1ST
#endif

#if defined(EFFECT_BUMP)
    #define LIL_FEATURE_NORMAL_2ND
#endif

#if defined(_GLOSSYREFLECTIONS_OFF)
    #define LIL_FEATURE_REFLECTION
#endif

#if defined(_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A)
    #define LIL_FEATURE_MATCAP
#endif

#if defined(_SPECULARHIGHLIGHTS_OFF)
    #define LIL_FEATURE_MATCAP_2ND
#endif

#if defined(GEOM_TYPE_MESH)
    #define LIL_FEATURE_TEX_MATCAP_NORMALMAP
#endif

#if defined(_METALLICGLOSSMAP)
    #define LIL_FEATURE_RIMLIGHT
#endif

#if defined(GEOM_TYPE_LEAF)
    #define LIL_FEATURE_RIMLIGHT_DIRECTION
#endif

#if defined(_SPECGLOSSMAP)
    #define LIL_FEATURE_GLITTER
#endif

#if defined(_PARALLAXMAP)
    #define LIL_FEATURE_PARALLAX
#endif

#if defined(PIXELSNAP_ON)
    #define LIL_FEATURE_POM
#endif

#if defined(BILLBOARD_FACE_CAMERA_POS)
    #define LIL_FEATURE_CLIPPING_CANCELLER
#endif

#if defined(_FADING_ON)
    #define LIL_FEATURE_DISTANCE_FADE
#endif

#if defined(_MAPPING_6_FRAMES_LAYOUT)
    #define LIL_FEATURE_AUDIOLINK
#endif

#if defined(_SUNDISK_HIGH_QUALITY)
    #define LIL_FEATURE_AUDIOLINK_LOCAL
#endif

#if defined(GEOM_TYPE_BRANCH_DETAIL)
    #define LIL_FEATURE_DISSOLVE
#endif

#if defined(_DETAIL_MULX2)
    #define LIL_FEATURE_OUTLINE_TONE_CORRECTION
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