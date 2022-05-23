#if UNITY_EDITOR
using UnityEngine;

public class lilToonSetting : ScriptableObject
{
    public bool LIL_FEATURE_ANIMATE_MAIN_UV = true;
    public bool LIL_FEATURE_MAIN_TONE_CORRECTION = true;
    public bool LIL_FEATURE_MAIN_GRADATION_MAP = true;
    public bool LIL_FEATURE_MAIN2ND = true;
    public bool LIL_FEATURE_MAIN3RD = true;
    public bool LIL_FEATURE_DECAL = true;
    public bool LIL_FEATURE_ANIMATE_DECAL = true;
    public bool LIL_FEATURE_LAYER_DISSOLVE = true;
    public bool LIL_FEATURE_ALPHAMASK = true;
    public bool LIL_FEATURE_SHADOW = true;
    public bool LIL_FEATURE_RECEIVE_SHADOW = true;
    public bool LIL_FEATURE_SHADOW_3RD = true;
    public bool LIL_FEATURE_EMISSION_1ST = true;
    public bool LIL_FEATURE_EMISSION_2ND = true;
    public bool LIL_FEATURE_ANIMATE_EMISSION_UV = true;
    public bool LIL_FEATURE_ANIMATE_EMISSION_MASK_UV = true;
    public bool LIL_FEATURE_EMISSION_GRADATION = true;
    public bool LIL_FEATURE_NORMAL_1ST = true;
    public bool LIL_FEATURE_NORMAL_2ND = true;
    public bool LIL_FEATURE_ANISOTROPY = true;
    public bool LIL_FEATURE_REFLECTION = true;
    public bool LIL_FEATURE_MATCAP = true;
    public bool LIL_FEATURE_MATCAP_2ND = true;
    public bool LIL_FEATURE_RIMLIGHT = true;
    public bool LIL_FEATURE_RIMLIGHT_DIRECTION = true;
    public bool LIL_FEATURE_GLITTER = true;
    public bool LIL_FEATURE_BACKLIGHT = true;
    public bool LIL_FEATURE_PARALLAX = true;
    public bool LIL_FEATURE_POM = false;
    public bool LIL_FEATURE_CLIPPING_CANCELLER = false;
    public bool LIL_FEATURE_DISTANCE_FADE = true;
    public bool LIL_FEATURE_AUDIOLINK = true;
    public bool LIL_FEATURE_AUDIOLINK_VERTEX = true;
    public bool LIL_FEATURE_AUDIOLINK_LOCAL = true;
    public bool LIL_FEATURE_DISSOLVE = true;
    public bool LIL_FEATURE_ENCRYPTION = false;
    public bool LIL_FEATURE_ANIMATE_OUTLINE_UV = true;
    public bool LIL_FEATURE_OUTLINE_TONE_CORRECTION = true;
    public bool LIL_FEATURE_FUR_COLLISION = true;
    public bool LIL_FEATURE_TEX_LAYER_MASK = true;
    public bool LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE = true;
    public bool LIL_FEATURE_TEX_SHADOW_BLUR = true;
    public bool LIL_FEATURE_TEX_SHADOW_BORDER = true;
    public bool LIL_FEATURE_TEX_SHADOW_STRENGTH = true;
    public bool LIL_FEATURE_TEX_SHADOW_1ST = true;
    public bool LIL_FEATURE_TEX_SHADOW_2ND = true;
    public bool LIL_FEATURE_TEX_SHADOW_3RD = true;
    public bool LIL_FEATURE_TEX_EMISSION_MASK = true;
    public bool LIL_FEATURE_TEX_NORMAL_MASK = true;
    public bool LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS = true;
    public bool LIL_FEATURE_TEX_REFLECTION_METALLIC = true;
    public bool LIL_FEATURE_TEX_REFLECTION_COLOR = true;
    public bool LIL_FEATURE_TEX_MATCAP_MASK = true;
    public bool LIL_FEATURE_TEX_MATCAP_NORMALMAP = true;
    public bool LIL_FEATURE_TEX_RIMLIGHT_COLOR = true;
    public bool LIL_FEATURE_TEX_DISSOLVE_NOISE = true;
    public bool LIL_FEATURE_TEX_OUTLINE_COLOR = true;
    public bool LIL_FEATURE_TEX_OUTLINE_WIDTH = true;
    public bool LIL_FEATURE_TEX_OUTLINE_NORMAL = true;
    public bool LIL_FEATURE_TEX_FUR_NORMAL = true;
    public bool LIL_FEATURE_TEX_FUR_MASK = true;
    public bool LIL_FEATURE_TEX_FUR_LENGTH = true;
    public bool LIL_FEATURE_TEX_TESSELLATION = true;

    public bool LIL_OPTIMIZE_APPLY_SHADOW_FA = true;
    public bool LIL_OPTIMIZE_USE_FORWARDADD = true;
    public bool LIL_OPTIMIZE_USE_VERTEXLIGHT = true;
    public bool LIL_OPTIMIZE_USE_LIGHTMAP = false;

    public bool isLocked = false;
    public bool isDebugOptimize = false;

    public float defaultAsUnlit = 0.0f;
    public float defaultVertexLightStrength = 0.0f;
    public float defaultLightMinLimit = 0.05f;
    public float defaultLightMaxLimit = 1.0f;
    public float defaultBeforeExposureLimit = 10000.0f;
    public float defaultMonochromeLighting = 0.0f;
    public float defaultlilDirectionalLightStrength = 1.0f;

    public lilToonPreset presetSkin;
    public lilToonPreset presetFace;
    public lilToonPreset presetHair;
    public lilToonPreset presetCloth;
}
#endif