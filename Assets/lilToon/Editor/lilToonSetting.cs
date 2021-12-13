#if UNITY_EDITOR
using UnityEngine;

public class lilToonSetting : ScriptableObject
{
    public bool LIL_FEATURE_ANIMATE_MAIN_UV;
    public bool LIL_FEATURE_MAIN_TONE_CORRECTION;
    public bool LIL_FEATURE_MAIN_GRADATION_MAP;
    public bool LIL_FEATURE_MAIN2ND;
    public bool LIL_FEATURE_MAIN3RD;
    public bool LIL_FEATURE_DECAL;
    public bool LIL_FEATURE_ANIMATE_DECAL;
    public bool LIL_FEATURE_LAYER_DISSOLVE;
    public bool LIL_FEATURE_ALPHAMASK;
    public bool LIL_FEATURE_SHADOW;
    public bool LIL_FEATURE_RECEIVE_SHADOW;
    public bool LIL_FEATURE_EMISSION_1ST;
    public bool LIL_FEATURE_EMISSION_2ND;
    public bool LIL_FEATURE_ANIMATE_EMISSION_UV;
    public bool LIL_FEATURE_ANIMATE_EMISSION_MASK_UV;
    public bool LIL_FEATURE_EMISSION_GRADATION;
    public bool LIL_FEATURE_NORMAL_1ST;
    public bool LIL_FEATURE_NORMAL_2ND;
    public bool LIL_FEATURE_ANISOTROPY;
    public bool LIL_FEATURE_REFLECTION;
    public bool LIL_FEATURE_MATCAP;
    public bool LIL_FEATURE_MATCAP_2ND;
    public bool LIL_FEATURE_RIMLIGHT;
    public bool LIL_FEATURE_RIMLIGHT_DIRECTION;
    public bool LIL_FEATURE_GLITTER;
    public bool LIL_FEATURE_BACKLIGHT;
    public bool LIL_FEATURE_PARALLAX;
    public bool LIL_FEATURE_POM;
    public bool LIL_FEATURE_CLIPPING_CANCELLER;
    public bool LIL_FEATURE_DISTANCE_FADE;
    public bool LIL_FEATURE_AUDIOLINK;
    public bool LIL_FEATURE_AUDIOLINK_VERTEX;
    public bool LIL_FEATURE_AUDIOLINK_LOCAL;
    public bool LIL_FEATURE_DISSOLVE;
    public bool LIL_FEATURE_ENCRYPTION;
    public bool LIL_FEATURE_ANIMATE_OUTLINE_UV;
    public bool LIL_FEATURE_OUTLINE_TONE_CORRECTION;
    public bool LIL_FEATURE_TEX_LAYER_MASK;
    public bool LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE;
    public bool LIL_FEATURE_TEX_SHADOW_BLUR;
    public bool LIL_FEATURE_TEX_SHADOW_BORDER;
    public bool LIL_FEATURE_TEX_SHADOW_STRENGTH;
    public bool LIL_FEATURE_TEX_SHADOW_1ST;
    public bool LIL_FEATURE_TEX_SHADOW_2ND;
    public bool LIL_FEATURE_TEX_EMISSION_MASK;
    public bool LIL_FEATURE_TEX_NORMAL_MASK;
    public bool LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS;
    public bool LIL_FEATURE_TEX_REFLECTION_METALLIC;
    public bool LIL_FEATURE_TEX_REFLECTION_COLOR;
    public bool LIL_FEATURE_TEX_MATCAP_MASK;
    public bool LIL_FEATURE_TEX_MATCAP_NORMALMAP;
    public bool LIL_FEATURE_TEX_RIMLIGHT_COLOR;
    public bool LIL_FEATURE_TEX_DISSOLVE_NOISE;
    public bool LIL_FEATURE_TEX_OUTLINE_COLOR;
    public bool LIL_FEATURE_TEX_OUTLINE_WIDTH;
    public bool LIL_FEATURE_TEX_OUTLINE_NORMAL;
    public bool LIL_FEATURE_TEX_FUR_NORMAL;
    public bool LIL_FEATURE_TEX_FUR_MASK;
    public bool LIL_FEATURE_TEX_FUR_LENGTH;
    public bool LIL_FEATURE_TEX_TESSELLATION;
    public bool isLocked;
    public bool shouldNotScan;
}
#endif