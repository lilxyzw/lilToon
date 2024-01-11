#if !LILTOON_VRCSDK3_AVATARS && !LILTOON_VRCSDK3_WORLDS && VRC_SDK_VRCSDK3
    #if UDON
        #define LILTOON_VRCSDK3_WORLDS
    #else
        #define LILTOON_VRCSDK3_AVATARS
    #endif
#endif
#if UNITY_EDITOR
using lilToon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
#if UNITY_2022_1_OR_NEWER || (UNITY_2023_1_OR_NEWER && !UNITY_2023_2_OR_NEWER)
    using System.Text.RegularExpressions;
#endif

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
    public bool LIL_FEATURE_SHADOW_LUT = true;
    public bool LIL_FEATURE_RIMSHADE = true;
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
    public bool LIL_FEATURE_IDMASK = true;
    public bool LIL_FEATURE_UDIMDISCARD = true;
    public bool LIL_FEATURE_DITHER = true;
    public bool LIL_FEATURE_ENCRYPTION = false;
    public bool LIL_FEATURE_ANIMATE_OUTLINE_UV = true;
    public bool LIL_FEATURE_OUTLINE_TONE_CORRECTION = true;
    public bool LIL_FEATURE_OUTLINE_RECEIVE_SHADOW = true;
    public bool LIL_FEATURE_FUR_COLLISION = true;

    public bool LIL_FEATURE_MainGradationTex = true;
    public bool LIL_FEATURE_MainColorAdjustMask = true;
    public bool LIL_FEATURE_Main2ndTex = true;
    public bool LIL_FEATURE_Main2ndBlendMask = true;
    public bool LIL_FEATURE_Main2ndDissolveMask = true;
    public bool LIL_FEATURE_Main2ndDissolveNoiseMask = true;
    public bool LIL_FEATURE_Main3rdTex = true;
    public bool LIL_FEATURE_Main3rdBlendMask = true;
    public bool LIL_FEATURE_Main3rdDissolveMask = true;
    public bool LIL_FEATURE_Main3rdDissolveNoiseMask = true;
    public bool LIL_FEATURE_AlphaMask = true;
    public bool LIL_FEATURE_BumpMap = true;
    public bool LIL_FEATURE_Bump2ndMap = true;
    public bool LIL_FEATURE_Bump2ndScaleMask = true;
    public bool LIL_FEATURE_AnisotropyTangentMap = true;
    public bool LIL_FEATURE_AnisotropyScaleMask = true;
    public bool LIL_FEATURE_AnisotropyShiftNoiseMask = true;
    public bool LIL_FEATURE_ShadowBorderMask = true;
    public bool LIL_FEATURE_ShadowBlurMask = true;
    public bool LIL_FEATURE_ShadowStrengthMask = true;
    public bool LIL_FEATURE_ShadowColorTex = true;
    public bool LIL_FEATURE_Shadow2ndColorTex = true;
    public bool LIL_FEATURE_Shadow3rdColorTex = true;
    public bool LIL_FEATURE_BacklightColorTex = true;
    public bool LIL_FEATURE_SmoothnessTex = true;
    public bool LIL_FEATURE_MetallicGlossMap = true;
    public bool LIL_FEATURE_ReflectionColorTex = true;
    public bool LIL_FEATURE_ReflectionCubeTex = true;
    public bool LIL_FEATURE_MatCapTex = true;
    public bool LIL_FEATURE_MatCapBlendMask = true;
    public bool LIL_FEATURE_MatCapBumpMap = true;
    public bool LIL_FEATURE_MatCap2ndTex = true;
    public bool LIL_FEATURE_MatCap2ndBlendMask = true;
    public bool LIL_FEATURE_MatCap2ndBumpMap = true;
    public bool LIL_FEATURE_RimColorTex = true;
    public bool LIL_FEATURE_GlitterColorTex = true;
    public bool LIL_FEATURE_GlitterShapeTex = true;
    public bool LIL_FEATURE_EmissionMap = true;
    public bool LIL_FEATURE_EmissionBlendMask = true;
    public bool LIL_FEATURE_EmissionGradTex = true;
    public bool LIL_FEATURE_Emission2ndMap = true;
    public bool LIL_FEATURE_Emission2ndBlendMask = true;
    public bool LIL_FEATURE_Emission2ndGradTex = true;
    public bool LIL_FEATURE_ParallaxMap = true;
    public bool LIL_FEATURE_AudioLinkMask = true;
    public bool LIL_FEATURE_AudioLinkLocalMap = true;
    public bool LIL_FEATURE_DissolveMask = true;
    public bool LIL_FEATURE_DissolveNoiseMask = true;
    public bool LIL_FEATURE_OutlineTex = true;
    public bool LIL_FEATURE_OutlineWidthMask = true;
    public bool LIL_FEATURE_OutlineVectorTex = true;
    public bool LIL_FEATURE_FurNoiseMask = true;
    public bool LIL_FEATURE_FurMask = true;
    public bool LIL_FEATURE_FurLengthMask = true;
    public bool LIL_FEATURE_FurVectorTex = true;

    public bool LIL_OPTIMIZE_APPLY_SHADOW_FA = true;
    public bool LIL_OPTIMIZE_USE_FORWARDADD = true;
    public bool LIL_OPTIMIZE_USE_FORWARDADD_SHADOW = false;
    public bool LIL_OPTIMIZE_USE_VERTEXLIGHT = true;
    public bool LIL_OPTIMIZE_USE_LIGHTMAP = false;
    public bool LIL_OPTIMIZE_DEFFERED = false;

    public bool isLocked = false;
    public bool isDebugOptimize = false;
    public bool isOptimizeInTestBuild = false;
    public bool isMigrateInStartUp = true;

    public float defaultAsUnlit = 0.0f;
    public float defaultVertexLightStrength = 0.0f;
    public float defaultLightMinLimit = 0.05f;
    public float defaultLightMaxLimit = 1.0f;
    public float defaultBeforeExposureLimit = 10000.0f;
    public float defaultMonochromeLighting = 0.0f;
    public float defaultlilDirectionalLightStrength = 1.0f;

    public string mainLightModeName = "";
    public string outlineLightModeName = "";
    public string preLightModeName = "";
    public string furLightModeName = "";
    public string furPreLightModeName = "";
    public string gemPreLightModeName = "";

    public lilToonPreset presetSkin;
    public lilToonPreset presetFace;
    public lilToonPreset presetHair;
    public lilToonPreset presetCloth;

    // This is not a shader setting, but the version number is stored here for material migration.
    public int previousVersion = 0;

    // Lock
    internal static void SaveLockedSetting(lilToonSetting shaderSetting)
    {
        string path = lilDirectoryManager.GetSettingLockPath();
        File.WriteAllText(path, JsonUtility.ToJson(shaderSetting, true));
        if(!path.Contains("Packages")) AssetDatabase.Refresh();
    }

    internal static void LoadLockedSetting(ref lilToonSetting shaderSetting)
    {
        var lockedSetting = CreateInstance<lilToonSetting>();
        string path = lilDirectoryManager.GetSettingLockPath();
        if(File.Exists(path))
        {
            JsonUtility.FromJsonOverwrite(File.ReadAllText(path), lockedSetting);
            shaderSetting.LIL_OPTIMIZE_APPLY_SHADOW_FA       = lockedSetting.LIL_OPTIMIZE_APPLY_SHADOW_FA;
            shaderSetting.LIL_OPTIMIZE_USE_FORWARDADD        = lockedSetting.LIL_OPTIMIZE_USE_FORWARDADD;
            shaderSetting.LIL_OPTIMIZE_USE_FORWARDADD_SHADOW = lockedSetting.LIL_OPTIMIZE_USE_FORWARDADD_SHADOW;
            shaderSetting.LIL_OPTIMIZE_USE_LIGHTMAP          = lockedSetting.LIL_OPTIMIZE_USE_LIGHTMAP;
            shaderSetting.isDebugOptimize                    = lockedSetting.isDebugOptimize;
            shaderSetting.isOptimizeInTestBuild              = lockedSetting.isOptimizeInTestBuild;
            shaderSetting.isMigrateInStartUp                 = lockedSetting.isMigrateInStartUp;
            shaderSetting.mainLightModeName                  = lockedSetting.mainLightModeName;
            shaderSetting.outlineLightModeName               = lockedSetting.outlineLightModeName;
            shaderSetting.preLightModeName                   = lockedSetting.preLightModeName;
            shaderSetting.furLightModeName                   = lockedSetting.furLightModeName;
            shaderSetting.furPreLightModeName                = lockedSetting.furPreLightModeName;
            shaderSetting.gemPreLightModeName                = lockedSetting.gemPreLightModeName;
        }
    }

    internal static void DeleteLockedSetting()
    {
        string path = lilDirectoryManager.GetSettingLockPath();
        if(File.Exists(path))         File.Delete(path);
        if(File.Exists(path+".meta")) File.Delete(path+".meta");
        if(!path.Contains("Packages")) AssetDatabase.Refresh();
    }

    // Save and Load
    public static void SaveShaderSetting(lilToonSetting shaderSetting)
    {
        string shaderSettingPath = lilDirectoryManager.GetShaderSettingPath();
        var sw = new StreamWriter(shaderSettingPath, false);
        sw.Write(JsonUtility.ToJson(shaderSetting, true));
        sw.Close();
    }

    internal static void LoadShaderSetting(ref lilToonSetting shaderSetting)
    {
        string shaderSettingPath = lilDirectoryManager.GetShaderSettingPath();
        if(shaderSetting == null) shaderSetting = CreateInstance<lilToonSetting>();
        if(File.Exists(shaderSettingPath)) JsonUtility.FromJsonOverwrite(File.ReadAllText(shaderSettingPath), shaderSetting);
        LoadLockedSetting(ref shaderSetting);
    }

    internal static void InitializeShaderSetting(ref lilToonSetting shaderSetting)
    {
        if(shaderSetting != null) return;
        LoadShaderSetting(ref shaderSetting);
        if(shaderSetting == null)
        {
            foreach(var path in lilDirectoryManager.FindAssetsPath("t:lilToonSetting"))
            {
                var shaderSettingOld = AssetDatabase.LoadAssetAtPath<lilToonSetting>(path);
                shaderSetting = Instantiate(shaderSettingOld);
                if(shaderSetting != null)
                {
                    Debug.Log("[lilToon] Migrate settings from: " + path);
                    TurnOnAllShaderSetting(ref shaderSetting);
                    ApplyShaderSetting(shaderSetting);
                    AssetDatabase.Refresh();
                    return;
                }
            }
            shaderSetting = CreateInstance<lilToonSetting>();
            SaveShaderSetting(shaderSetting);
            AssetDatabase.Refresh();
        }
    }

    public static void TurnOffAllShaderSetting(ref lilToonSetting shaderSetting)
    {
        if(shaderSetting == null) return;
        shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV = false;
        shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION = false;
        shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP = false;
        shaderSetting.LIL_FEATURE_MAIN2ND = false;
        shaderSetting.LIL_FEATURE_MAIN3RD = false;
        shaderSetting.LIL_FEATURE_DECAL = false;
        shaderSetting.LIL_FEATURE_ANIMATE_DECAL = false;
        shaderSetting.LIL_FEATURE_LAYER_DISSOLVE = false;
        shaderSetting.LIL_FEATURE_ALPHAMASK = false;
        shaderSetting.LIL_FEATURE_SHADOW = false;
        shaderSetting.LIL_FEATURE_SHADOW_3RD = false;
        shaderSetting.LIL_FEATURE_SHADOW_LUT = false;
        shaderSetting.LIL_FEATURE_RECEIVE_SHADOW = false;
        shaderSetting.LIL_FEATURE_RIMSHADE = false;
        shaderSetting.LIL_FEATURE_EMISSION_1ST = false;
        shaderSetting.LIL_FEATURE_EMISSION_2ND = false;
        shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV = false;
        shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV = false;
        shaderSetting.LIL_FEATURE_EMISSION_GRADATION = false;
        shaderSetting.LIL_FEATURE_NORMAL_1ST = false;
        shaderSetting.LIL_FEATURE_NORMAL_2ND = false;
        shaderSetting.LIL_FEATURE_ANISOTROPY = false;
        shaderSetting.LIL_FEATURE_REFLECTION = false;
        shaderSetting.LIL_FEATURE_MATCAP = false;
        shaderSetting.LIL_FEATURE_MATCAP_2ND = false;
        shaderSetting.LIL_FEATURE_RIMLIGHT = false;
        shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION = false;
        shaderSetting.LIL_FEATURE_GLITTER = false;
        shaderSetting.LIL_FEATURE_BACKLIGHT = false;
        shaderSetting.LIL_FEATURE_PARALLAX = false;
        shaderSetting.LIL_FEATURE_POM = false;
        //shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER = false;
        shaderSetting.LIL_FEATURE_DISTANCE_FADE = false;
        shaderSetting.LIL_FEATURE_AUDIOLINK = false;
        shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX = false;
        shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL = false;
        shaderSetting.LIL_FEATURE_DISSOLVE = false;
        shaderSetting.LIL_FEATURE_DITHER = false;
        shaderSetting.LIL_FEATURE_IDMASK = false;
        shaderSetting.LIL_FEATURE_UDIMDISCARD = false;
        shaderSetting.LIL_FEATURE_ENCRYPTION = lilDirectoryManager.ExistsEncryption();
        shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV = false;
        shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION = false;
        shaderSetting.LIL_FEATURE_OUTLINE_RECEIVE_SHADOW = false;
        shaderSetting.LIL_FEATURE_FUR_COLLISION = false;

        shaderSetting.LIL_FEATURE_MainGradationTex = false;
        shaderSetting.LIL_FEATURE_MainColorAdjustMask = false;
        shaderSetting.LIL_FEATURE_Main2ndTex = false;
        shaderSetting.LIL_FEATURE_Main2ndBlendMask = false;
        shaderSetting.LIL_FEATURE_Main2ndDissolveMask = false;
        shaderSetting.LIL_FEATURE_Main2ndDissolveNoiseMask = false;
        shaderSetting.LIL_FEATURE_Main3rdTex = false;
        shaderSetting.LIL_FEATURE_Main3rdBlendMask = false;
        shaderSetting.LIL_FEATURE_Main3rdDissolveMask = false;
        shaderSetting.LIL_FEATURE_Main3rdDissolveNoiseMask = false;
        shaderSetting.LIL_FEATURE_AlphaMask = false;
        shaderSetting.LIL_FEATURE_BumpMap = false;
        shaderSetting.LIL_FEATURE_Bump2ndMap = false;
        shaderSetting.LIL_FEATURE_Bump2ndScaleMask = false;
        shaderSetting.LIL_FEATURE_AnisotropyTangentMap = false;
        shaderSetting.LIL_FEATURE_AnisotropyScaleMask = false;
        shaderSetting.LIL_FEATURE_AnisotropyShiftNoiseMask = false;
        shaderSetting.LIL_FEATURE_ShadowBorderMask = false;
        shaderSetting.LIL_FEATURE_ShadowBlurMask = false;
        shaderSetting.LIL_FEATURE_ShadowStrengthMask = false;
        shaderSetting.LIL_FEATURE_ShadowColorTex = false;
        shaderSetting.LIL_FEATURE_Shadow2ndColorTex = false;
        shaderSetting.LIL_FEATURE_Shadow3rdColorTex = false;
        shaderSetting.LIL_FEATURE_BacklightColorTex = false;
        shaderSetting.LIL_FEATURE_SmoothnessTex = false;
        shaderSetting.LIL_FEATURE_MetallicGlossMap = false;
        shaderSetting.LIL_FEATURE_ReflectionColorTex = false;
        shaderSetting.LIL_FEATURE_ReflectionCubeTex = false;
        shaderSetting.LIL_FEATURE_MatCapTex = false;
        shaderSetting.LIL_FEATURE_MatCapBlendMask = false;
        shaderSetting.LIL_FEATURE_MatCapBumpMap = false;
        shaderSetting.LIL_FEATURE_MatCap2ndTex = false;
        shaderSetting.LIL_FEATURE_MatCap2ndBlendMask = false;
        shaderSetting.LIL_FEATURE_MatCap2ndBumpMap = false;
        shaderSetting.LIL_FEATURE_RimColorTex = false;
        shaderSetting.LIL_FEATURE_GlitterColorTex = false;
        shaderSetting.LIL_FEATURE_GlitterShapeTex = false;
        shaderSetting.LIL_FEATURE_EmissionMap = false;
        shaderSetting.LIL_FEATURE_EmissionBlendMask = false;
        shaderSetting.LIL_FEATURE_EmissionGradTex = false;
        shaderSetting.LIL_FEATURE_Emission2ndMap = false;
        shaderSetting.LIL_FEATURE_Emission2ndBlendMask = false;
        shaderSetting.LIL_FEATURE_Emission2ndGradTex = false;
        shaderSetting.LIL_FEATURE_ParallaxMap = false;
        shaderSetting.LIL_FEATURE_AudioLinkMask = false;
        shaderSetting.LIL_FEATURE_AudioLinkLocalMap = false;
        shaderSetting.LIL_FEATURE_DissolveMask = false;
        shaderSetting.LIL_FEATURE_DissolveNoiseMask = false;
        shaderSetting.LIL_FEATURE_OutlineTex = false;
        shaderSetting.LIL_FEATURE_OutlineWidthMask = false;
        shaderSetting.LIL_FEATURE_OutlineVectorTex = false;
        shaderSetting.LIL_FEATURE_FurNoiseMask = false;
        shaderSetting.LIL_FEATURE_FurMask = false;
        shaderSetting.LIL_FEATURE_FurLengthMask = false;
        shaderSetting.LIL_FEATURE_FurVectorTex = false;
        shaderSetting.LIL_OPTIMIZE_USE_VERTEXLIGHT = false;
        EditorUtility.SetDirty(shaderSetting);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void TurnOnAllShaderSetting(ref lilToonSetting shaderSetting)
    {
        if(shaderSetting == null) return;
        shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV = true;
        shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION = true;
        shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP = true;
        shaderSetting.LIL_FEATURE_MAIN2ND = true;
        shaderSetting.LIL_FEATURE_MAIN3RD = true;
        shaderSetting.LIL_FEATURE_DECAL = true;
        shaderSetting.LIL_FEATURE_ANIMATE_DECAL = true;
        shaderSetting.LIL_FEATURE_LAYER_DISSOLVE = true;
        shaderSetting.LIL_FEATURE_ALPHAMASK = true;
        shaderSetting.LIL_FEATURE_SHADOW = true;
        shaderSetting.LIL_FEATURE_SHADOW_3RD = true;
        shaderSetting.LIL_FEATURE_SHADOW_LUT = true;
        shaderSetting.LIL_FEATURE_RECEIVE_SHADOW = true;
        shaderSetting.LIL_FEATURE_RIMSHADE = true;
        shaderSetting.LIL_FEATURE_EMISSION_1ST = true;
        shaderSetting.LIL_FEATURE_EMISSION_2ND = true;
        shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV = true;
        shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV = true;
        shaderSetting.LIL_FEATURE_EMISSION_GRADATION = true;
        shaderSetting.LIL_FEATURE_NORMAL_1ST = true;
        shaderSetting.LIL_FEATURE_NORMAL_2ND = true;
        shaderSetting.LIL_FEATURE_ANISOTROPY = true;
        shaderSetting.LIL_FEATURE_REFLECTION = true;
        shaderSetting.LIL_FEATURE_MATCAP = true;
        shaderSetting.LIL_FEATURE_MATCAP_2ND = true;
        shaderSetting.LIL_FEATURE_RIMLIGHT = true;
        shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION = true;
        shaderSetting.LIL_FEATURE_GLITTER = true;
        shaderSetting.LIL_FEATURE_BACKLIGHT = true;
        shaderSetting.LIL_FEATURE_PARALLAX = true;
        shaderSetting.LIL_FEATURE_POM = true;
        //shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER = true;
        shaderSetting.LIL_FEATURE_DISTANCE_FADE = true;
        shaderSetting.LIL_FEATURE_AUDIOLINK = true;
        shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX = true;
        shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL = true;
        shaderSetting.LIL_FEATURE_DISSOLVE = true;
        shaderSetting.LIL_FEATURE_DITHER = true;
        shaderSetting.LIL_FEATURE_IDMASK = true;
        shaderSetting.LIL_FEATURE_UDIMDISCARD = true;
        shaderSetting.LIL_FEATURE_ENCRYPTION = lilDirectoryManager.ExistsEncryption();
        shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV = true;
        shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION = true;
        shaderSetting.LIL_FEATURE_OUTLINE_RECEIVE_SHADOW = true;
        shaderSetting.LIL_FEATURE_FUR_COLLISION = true;
        shaderSetting.LIL_OPTIMIZE_USE_VERTEXLIGHT = true;

        if(!lilShaderAPI.IsTextureLimitedAPI())
        {
            shaderSetting.LIL_FEATURE_MainGradationTex = true;
            shaderSetting.LIL_FEATURE_MainColorAdjustMask = true;
            shaderSetting.LIL_FEATURE_Main2ndTex = true;
            shaderSetting.LIL_FEATURE_Main2ndBlendMask = true;
            shaderSetting.LIL_FEATURE_Main2ndDissolveMask = true;
            shaderSetting.LIL_FEATURE_Main2ndDissolveNoiseMask = true;
            shaderSetting.LIL_FEATURE_Main3rdTex = true;
            shaderSetting.LIL_FEATURE_Main3rdBlendMask = true;
            shaderSetting.LIL_FEATURE_Main3rdDissolveMask = true;
            shaderSetting.LIL_FEATURE_Main3rdDissolveNoiseMask = true;
            shaderSetting.LIL_FEATURE_AlphaMask = true;
            shaderSetting.LIL_FEATURE_BumpMap = true;
            shaderSetting.LIL_FEATURE_Bump2ndMap = true;
            shaderSetting.LIL_FEATURE_Bump2ndScaleMask = true;
            shaderSetting.LIL_FEATURE_AnisotropyTangentMap = true;
            shaderSetting.LIL_FEATURE_AnisotropyScaleMask = true;
            shaderSetting.LIL_FEATURE_AnisotropyShiftNoiseMask = true;
            shaderSetting.LIL_FEATURE_ShadowBorderMask = true;
            shaderSetting.LIL_FEATURE_ShadowBlurMask = true;
            shaderSetting.LIL_FEATURE_ShadowStrengthMask = true;
            shaderSetting.LIL_FEATURE_ShadowColorTex = true;
            shaderSetting.LIL_FEATURE_Shadow2ndColorTex = true;
            shaderSetting.LIL_FEATURE_Shadow3rdColorTex = true;
            shaderSetting.LIL_FEATURE_BacklightColorTex = true;
            shaderSetting.LIL_FEATURE_SmoothnessTex = true;
            shaderSetting.LIL_FEATURE_MetallicGlossMap = true;
            shaderSetting.LIL_FEATURE_ReflectionColorTex = true;
            shaderSetting.LIL_FEATURE_ReflectionCubeTex = true;
            shaderSetting.LIL_FEATURE_MatCapTex = true;
            shaderSetting.LIL_FEATURE_MatCapBlendMask = true;
            shaderSetting.LIL_FEATURE_MatCapBumpMap = true;
            shaderSetting.LIL_FEATURE_MatCap2ndTex = true;
            shaderSetting.LIL_FEATURE_MatCap2ndBlendMask = true;
            shaderSetting.LIL_FEATURE_MatCap2ndBumpMap = true;
            shaderSetting.LIL_FEATURE_RimColorTex = true;
            shaderSetting.LIL_FEATURE_GlitterColorTex = true;
            shaderSetting.LIL_FEATURE_GlitterShapeTex = true;
            shaderSetting.LIL_FEATURE_EmissionMap = true;
            shaderSetting.LIL_FEATURE_EmissionBlendMask = true;
            shaderSetting.LIL_FEATURE_EmissionGradTex = true;
            shaderSetting.LIL_FEATURE_Emission2ndMap = true;
            shaderSetting.LIL_FEATURE_Emission2ndBlendMask = true;
            shaderSetting.LIL_FEATURE_Emission2ndGradTex = true;
            shaderSetting.LIL_FEATURE_ParallaxMap = true;
            shaderSetting.LIL_FEATURE_AudioLinkMask = true;
            shaderSetting.LIL_FEATURE_AudioLinkLocalMap = true;
            shaderSetting.LIL_FEATURE_DissolveMask = true;
            shaderSetting.LIL_FEATURE_DissolveNoiseMask = true;
            shaderSetting.LIL_FEATURE_OutlineTex = true;
            shaderSetting.LIL_FEATURE_OutlineWidthMask = true;
            shaderSetting.LIL_FEATURE_OutlineVectorTex = true;
            shaderSetting.LIL_FEATURE_FurNoiseMask = true;
            shaderSetting.LIL_FEATURE_FurMask = true;
            shaderSetting.LIL_FEATURE_FurLengthMask = true;
            shaderSetting.LIL_FEATURE_FurVectorTex = true;
        }
    }

    internal static void ApplyShaderSetting(lilToonSetting shaderSetting, string reportTitle = null)
    {
        SaveShaderSetting(shaderSetting);
        string shaderSettingString = BuildShaderSettingString(shaderSetting, true);

        string shaderFolderPath = lilDirectoryManager.GetShaderFolderPath();
        string baseShaderFolderPath = lilDirectoryManager.GetBaseShaderFolderPath();
        foreach(var baseShaderPath in lilDirectoryManager.FindAssetsPath("", new[] {baseShaderFolderPath}).Where(p => p.Contains(".lilinternal")))
        {
            string shaderPath = shaderFolderPath + Path.AltDirectorySeparatorChar + Path.GetFileNameWithoutExtension(baseShaderPath) + ".shader";
            File.WriteAllText(shaderPath, lilShaderContainer.UnpackContainer(baseShaderPath));
        }
        foreach(var folder in lilDirectoryManager.FindAssetsPath("t:shader").Where(p => p.Contains(".lilcontainer")).Select(p => Path.GetDirectoryName(p)).Distinct())
        {
            AssetDatabase.ImportAsset(folder, ImportAssetOptions.ImportRecursive);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if(!string.IsNullOrEmpty(reportTitle))
        {
            Debug.Log(reportTitle + Environment.NewLine + shaderSettingString);
        }
    }

    internal static void ApplyShaderSetting(lilToonSetting shaderSetting, string reportTitle, List<Shader> shaders, bool doOptimize = false)
    {
        if(shaders == null || shaders.Count() == 0)
        {
            ApplyShaderSetting(shaderSetting, reportTitle);
            return;
        }

        SaveShaderSetting(shaderSetting);
        string shaderSettingString = BuildShaderSettingString(shaderSetting, true);

        string baseShaderFolderPath = lilDirectoryManager.GetBaseShaderFolderPath();
        var shaderPathes = new List<string>();
        foreach(var shader in shaders)
        {
            string shaderPath = AssetDatabase.GetAssetPath(shader);
            if(string.IsNullOrEmpty(shaderPath)) continue;
            if(!shaderPathes.Contains(shaderPath)) shaderPathes.Add(shaderPath);
            if(shaderPath.Contains(".lilcontainer")) continue;

            string baseShaderPath = baseShaderFolderPath + Path.AltDirectorySeparatorChar + Path.GetFileNameWithoutExtension(shaderPath) + ".lilinternal";
            if(File.Exists(baseShaderPath)) File.WriteAllText(shaderPath, lilShaderContainer.UnpackContainer(baseShaderPath, null, doOptimize));
        }
        foreach(var shaderPath in shaderPathes)
        {
            AssetDatabase.ImportAsset(shaderPath, ImportAssetOptions.ForceSynchronousImport);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if(!string.IsNullOrEmpty(reportTitle))
        {
            Debug.Log(reportTitle + Environment.NewLine + shaderSettingString);
        }
    }

    public static string BuildShaderSettingString(lilToonSetting shaderSetting, bool isFile)
    {
        var sb = new StringBuilder();
        if(isFile)
        {
            sb.AppendLine("#ifndef LIL_SETTING_INCLUDED");
            sb.AppendLine("#define LIL_SETTING_INCLUDED");
            sb.AppendLine("");
        }
        if(shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV) sb.AppendLine("#define LIL_FEATURE_ANIMATE_MAIN_UV");
        if(shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION) sb.AppendLine("#define LIL_FEATURE_MAIN_TONE_CORRECTION");
        if(shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP) sb.AppendLine("#define LIL_FEATURE_MAIN_GRADATION_MAP");
        if(shaderSetting.LIL_FEATURE_MAIN2ND) sb.AppendLine("#define LIL_FEATURE_MAIN2ND");
        if(shaderSetting.LIL_FEATURE_MAIN3RD) sb.AppendLine("#define LIL_FEATURE_MAIN3RD");
        if(shaderSetting.LIL_FEATURE_MAIN2ND || shaderSetting.LIL_FEATURE_MAIN3RD)
        {
            if(shaderSetting.LIL_FEATURE_DECAL) sb.AppendLine("#define LIL_FEATURE_DECAL");
            if(shaderSetting.LIL_FEATURE_ANIMATE_DECAL) sb.AppendLine("#define LIL_FEATURE_ANIMATE_DECAL");
            if(shaderSetting.LIL_FEATURE_LAYER_DISSOLVE) sb.AppendLine("#define LIL_FEATURE_LAYER_DISSOLVE");
        }

        if(shaderSetting.LIL_FEATURE_ALPHAMASK) sb.AppendLine("#define LIL_FEATURE_ALPHAMASK");

        if(shaderSetting.LIL_FEATURE_SHADOW)
        {
            sb.AppendLine("#define LIL_FEATURE_SHADOW");
            if(shaderSetting.LIL_FEATURE_RECEIVE_SHADOW) sb.AppendLine("#define LIL_FEATURE_RECEIVE_SHADOW");
            if(shaderSetting.LIL_FEATURE_SHADOW_3RD) sb.AppendLine("#define LIL_FEATURE_SHADOW_3RD");
            if(shaderSetting.LIL_FEATURE_SHADOW_LUT) sb.AppendLine("#define LIL_FEATURE_SHADOW_LUT");
        }
        if(shaderSetting.LIL_FEATURE_RIMSHADE) sb.AppendLine("#define LIL_FEATURE_RIMSHADE");

        if(shaderSetting.LIL_FEATURE_EMISSION_1ST) sb.AppendLine("#define LIL_FEATURE_EMISSION_1ST");
        if(shaderSetting.LIL_FEATURE_EMISSION_2ND) sb.AppendLine("#define LIL_FEATURE_EMISSION_2ND");
        if(shaderSetting.LIL_FEATURE_EMISSION_1ST || shaderSetting.LIL_FEATURE_EMISSION_2ND)
        {
            if(shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV) sb.AppendLine("#define LIL_FEATURE_ANIMATE_EMISSION_UV");
            if(shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV) sb.AppendLine("#define LIL_FEATURE_ANIMATE_EMISSION_MASK_UV");
            if(shaderSetting.LIL_FEATURE_EMISSION_GRADATION) sb.AppendLine("#define LIL_FEATURE_EMISSION_GRADATION");
        }
        if(shaderSetting.LIL_FEATURE_NORMAL_1ST) sb.AppendLine("#define LIL_FEATURE_NORMAL_1ST");
        if(shaderSetting.LIL_FEATURE_NORMAL_2ND) sb.AppendLine("#define LIL_FEATURE_NORMAL_2ND");
        if(shaderSetting.LIL_FEATURE_ANISOTROPY) sb.AppendLine("#define LIL_FEATURE_ANISOTROPY");
        if(shaderSetting.LIL_FEATURE_REFLECTION) sb.AppendLine("#define LIL_FEATURE_REFLECTION");
        if(shaderSetting.LIL_FEATURE_MATCAP) sb.AppendLine("#define LIL_FEATURE_MATCAP");
        if(shaderSetting.LIL_FEATURE_MATCAP_2ND) sb.AppendLine("#define LIL_FEATURE_MATCAP_2ND");
        if(shaderSetting.LIL_FEATURE_RIMLIGHT)
        {
            sb.AppendLine("#define LIL_FEATURE_RIMLIGHT");
            if(shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION) sb.AppendLine("#define LIL_FEATURE_RIMLIGHT_DIRECTION");
        }
        if(shaderSetting.LIL_FEATURE_GLITTER) sb.AppendLine("#define LIL_FEATURE_GLITTER");
        if(shaderSetting.LIL_FEATURE_BACKLIGHT) sb.AppendLine("#define LIL_FEATURE_BACKLIGHT");
        if(shaderSetting.LIL_FEATURE_PARALLAX)
        {
            sb.AppendLine("#define LIL_FEATURE_PARALLAX");
            if(shaderSetting.LIL_FEATURE_POM) sb.AppendLine("#define LIL_FEATURE_POM");
        }
        if(shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER) sb.AppendLine("#define LIL_FEATURE_CLIPPING_CANCELLER");
        if(shaderSetting.LIL_FEATURE_DISTANCE_FADE) sb.AppendLine("#define LIL_FEATURE_DISTANCE_FADE");
        if(shaderSetting.LIL_FEATURE_AUDIOLINK)
        {
            sb.AppendLine("#define LIL_FEATURE_AUDIOLINK");
            if(shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX) sb.AppendLine("#define LIL_FEATURE_AUDIOLINK_VERTEX");
            if(shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL) sb.AppendLine("#define LIL_FEATURE_AUDIOLINK_LOCAL");
        }
        if(shaderSetting.LIL_FEATURE_DISSOLVE) sb.AppendLine("#define LIL_FEATURE_DISSOLVE");
        if(shaderSetting.LIL_FEATURE_DITHER) sb.AppendLine("#define LIL_FEATURE_DITHER");
        if(shaderSetting.LIL_FEATURE_IDMASK) sb.AppendLine("#define LIL_FEATURE_IDMASK");
        if(shaderSetting.LIL_FEATURE_UDIMDISCARD) sb.AppendLine("#define LIL_FEATURE_UDIMDISCARD");
        if(shaderSetting.LIL_FEATURE_ENCRYPTION) sb.AppendLine("#define LIL_FEATURE_ENCRYPTION");
        if(shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION) sb.AppendLine("#define LIL_FEATURE_OUTLINE_TONE_CORRECTION");
        if(shaderSetting.LIL_FEATURE_OUTLINE_RECEIVE_SHADOW) sb.AppendLine("#define LIL_FEATURE_OUTLINE_RECEIVE_SHADOW");
        if(shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV) sb.AppendLine("#define LIL_FEATURE_ANIMATE_OUTLINE_UV");
        if(shaderSetting.LIL_FEATURE_FUR_COLLISION) sb.AppendLine("#define LIL_FEATURE_FUR_COLLISION");

        if(shaderSetting.LIL_FEATURE_MainGradationTex)           sb.AppendLine("#define LIL_FEATURE_MainGradationTex");
        if(shaderSetting.LIL_FEATURE_MainColorAdjustMask)        sb.AppendLine("#define LIL_FEATURE_MainColorAdjustMask");
        if(shaderSetting.LIL_FEATURE_Main2ndTex)                 sb.AppendLine("#define LIL_FEATURE_Main2ndTex");
        if(shaderSetting.LIL_FEATURE_Main2ndBlendMask)           sb.AppendLine("#define LIL_FEATURE_Main2ndBlendMask");
        if(shaderSetting.LIL_FEATURE_Main2ndDissolveMask)        sb.AppendLine("#define LIL_FEATURE_Main2ndDissolveMask");
        if(shaderSetting.LIL_FEATURE_Main2ndDissolveNoiseMask)   sb.AppendLine("#define LIL_FEATURE_Main2ndDissolveNoiseMask");
        if(shaderSetting.LIL_FEATURE_Main3rdTex)                 sb.AppendLine("#define LIL_FEATURE_Main3rdTex");
        if(shaderSetting.LIL_FEATURE_Main3rdBlendMask)           sb.AppendLine("#define LIL_FEATURE_Main3rdBlendMask");
        if(shaderSetting.LIL_FEATURE_Main3rdDissolveMask)        sb.AppendLine("#define LIL_FEATURE_Main3rdDissolveMask");
        if(shaderSetting.LIL_FEATURE_Main3rdDissolveNoiseMask)   sb.AppendLine("#define LIL_FEATURE_Main3rdDissolveNoiseMask");
        if(shaderSetting.LIL_FEATURE_AlphaMask)                  sb.AppendLine("#define LIL_FEATURE_AlphaMask");
        if(shaderSetting.LIL_FEATURE_BumpMap)                    sb.AppendLine("#define LIL_FEATURE_BumpMap");
        if(shaderSetting.LIL_FEATURE_Bump2ndMap)                 sb.AppendLine("#define LIL_FEATURE_Bump2ndMap");
        if(shaderSetting.LIL_FEATURE_Bump2ndScaleMask)           sb.AppendLine("#define LIL_FEATURE_Bump2ndScaleMask");
        if(shaderSetting.LIL_FEATURE_AnisotropyTangentMap)       sb.AppendLine("#define LIL_FEATURE_AnisotropyTangentMap");
        if(shaderSetting.LIL_FEATURE_AnisotropyScaleMask)        sb.AppendLine("#define LIL_FEATURE_AnisotropyScaleMask");
        if(shaderSetting.LIL_FEATURE_AnisotropyShiftNoiseMask)   sb.AppendLine("#define LIL_FEATURE_AnisotropyShiftNoiseMask");
        if(shaderSetting.LIL_FEATURE_ShadowBorderMask)           sb.AppendLine("#define LIL_FEATURE_ShadowBorderMask");
        if(shaderSetting.LIL_FEATURE_ShadowBlurMask)             sb.AppendLine("#define LIL_FEATURE_ShadowBlurMask");
        if(shaderSetting.LIL_FEATURE_ShadowStrengthMask)         sb.AppendLine("#define LIL_FEATURE_ShadowStrengthMask");
        if(shaderSetting.LIL_FEATURE_ShadowColorTex)             sb.AppendLine("#define LIL_FEATURE_ShadowColorTex");
        if(shaderSetting.LIL_FEATURE_Shadow2ndColorTex)          sb.AppendLine("#define LIL_FEATURE_Shadow2ndColorTex");
        if(shaderSetting.LIL_FEATURE_Shadow3rdColorTex)          sb.AppendLine("#define LIL_FEATURE_Shadow3rdColorTex");
        if(shaderSetting.LIL_FEATURE_BacklightColorTex)          sb.AppendLine("#define LIL_FEATURE_BacklightColorTex");
        if(shaderSetting.LIL_FEATURE_SmoothnessTex)              sb.AppendLine("#define LIL_FEATURE_SmoothnessTex");
        if(shaderSetting.LIL_FEATURE_MetallicGlossMap)           sb.AppendLine("#define LIL_FEATURE_MetallicGlossMap");
        if(shaderSetting.LIL_FEATURE_ReflectionColorTex)         sb.AppendLine("#define LIL_FEATURE_ReflectionColorTex");
        if(shaderSetting.LIL_FEATURE_ReflectionCubeTex)          sb.AppendLine("#define LIL_FEATURE_ReflectionCubeTex");
        if(shaderSetting.LIL_FEATURE_MatCapTex)                  sb.AppendLine("#define LIL_FEATURE_MatCapTex");
        if(shaderSetting.LIL_FEATURE_MatCapBlendMask)            sb.AppendLine("#define LIL_FEATURE_MatCapBlendMask");
        if(shaderSetting.LIL_FEATURE_MatCapBumpMap)              sb.AppendLine("#define LIL_FEATURE_MatCapBumpMap");
        if(shaderSetting.LIL_FEATURE_MatCap2ndTex)               sb.AppendLine("#define LIL_FEATURE_MatCap2ndTex");
        if(shaderSetting.LIL_FEATURE_MatCap2ndBlendMask)         sb.AppendLine("#define LIL_FEATURE_MatCap2ndBlendMask");
        if(shaderSetting.LIL_FEATURE_MatCap2ndBumpMap)           sb.AppendLine("#define LIL_FEATURE_MatCap2ndBumpMap");
        if(shaderSetting.LIL_FEATURE_RimColorTex)                sb.AppendLine("#define LIL_FEATURE_RimColorTex");
        if(shaderSetting.LIL_FEATURE_GlitterColorTex)            sb.AppendLine("#define LIL_FEATURE_GlitterColorTex");
        if(shaderSetting.LIL_FEATURE_GlitterShapeTex)            sb.AppendLine("#define LIL_FEATURE_GlitterShapeTex");
        if(shaderSetting.LIL_FEATURE_EmissionMap)                sb.AppendLine("#define LIL_FEATURE_EmissionMap");
        if(shaderSetting.LIL_FEATURE_EmissionBlendMask)          sb.AppendLine("#define LIL_FEATURE_EmissionBlendMask");
        if(shaderSetting.LIL_FEATURE_EmissionGradTex)            sb.AppendLine("#define LIL_FEATURE_EmissionGradTex");
        if(shaderSetting.LIL_FEATURE_Emission2ndMap)             sb.AppendLine("#define LIL_FEATURE_Emission2ndMap");
        if(shaderSetting.LIL_FEATURE_Emission2ndBlendMask)       sb.AppendLine("#define LIL_FEATURE_Emission2ndBlendMask");
        if(shaderSetting.LIL_FEATURE_Emission2ndGradTex)         sb.AppendLine("#define LIL_FEATURE_Emission2ndGradTex");
        if(shaderSetting.LIL_FEATURE_ParallaxMap)                sb.AppendLine("#define LIL_FEATURE_ParallaxMap");
        if(shaderSetting.LIL_FEATURE_AudioLinkMask)              sb.AppendLine("#define LIL_FEATURE_AudioLinkMask");
        if(shaderSetting.LIL_FEATURE_AudioLinkLocalMap)          sb.AppendLine("#define LIL_FEATURE_AudioLinkLocalMap");
        if(shaderSetting.LIL_FEATURE_DissolveMask)               sb.AppendLine("#define LIL_FEATURE_DissolveMask");
        if(shaderSetting.LIL_FEATURE_DissolveNoiseMask)          sb.AppendLine("#define LIL_FEATURE_DissolveNoiseMask");
        if(shaderSetting.LIL_FEATURE_OutlineTex)                 sb.AppendLine("#define LIL_FEATURE_OutlineTex");
        if(shaderSetting.LIL_FEATURE_OutlineWidthMask)           sb.AppendLine("#define LIL_FEATURE_OutlineWidthMask");
        if(shaderSetting.LIL_FEATURE_OutlineVectorTex)           sb.AppendLine("#define LIL_FEATURE_OutlineVectorTex");
        if(shaderSetting.LIL_FEATURE_FurNoiseMask)               sb.AppendLine("#define LIL_FEATURE_FurNoiseMask");
        if(shaderSetting.LIL_FEATURE_FurMask)                    sb.AppendLine("#define LIL_FEATURE_FurMask");
        if(shaderSetting.LIL_FEATURE_FurLengthMask)              sb.AppendLine("#define LIL_FEATURE_FurLengthMask");
        if(shaderSetting.LIL_FEATURE_FurVectorTex)               sb.AppendLine("#define LIL_FEATURE_FurVectorTex");

        if(shaderSetting.LIL_OPTIMIZE_APPLY_SHADOW_FA) sb.AppendLine("#define LIL_OPTIMIZE_APPLY_SHADOW_FA");
        if(shaderSetting.LIL_OPTIMIZE_USE_FORWARDADD) sb.AppendLine("#define LIL_OPTIMIZE_USE_FORWARDADD");
        if(shaderSetting.LIL_OPTIMIZE_USE_FORWARDADD_SHADOW) sb.AppendLine("#define LIL_OPTIMIZE_USE_FORWARDADD_SHADOW");
        if(shaderSetting.LIL_OPTIMIZE_USE_VERTEXLIGHT) sb.AppendLine("#define LIL_OPTIMIZE_USE_VERTEXLIGHT");
        if(shaderSetting.LIL_OPTIMIZE_USE_LIGHTMAP) sb.AppendLine("#define LIL_OPTIMIZE_USE_LIGHTMAP");
        if(isFile)
        {
            sb.AppendLine("");
            sb.AppendLine("#endif");
        }

        if(!isFile)
        {
            if(!shaderSetting.LIL_FEATURE_REFLECTION) sb.AppendLine("#pragma lil_skip_variants_reflections");
            if(!shaderSetting.LIL_OPTIMIZE_USE_VERTEXLIGHT) sb.AppendLine("#pragma lil_skip_variants_addlight");
            if(!shaderSetting.LIL_OPTIMIZE_USE_LIGHTMAP) sb.AppendLine("#pragma lil_skip_variants_lightmaps");
        }
        return sb.ToString();
    }

    public static string BuildShaderSettingString(bool isFile)
    {
        lilToonSetting shaderSetting = null;
        InitializeShaderSetting(ref shaderSetting);
        return BuildShaderSettingString(shaderSetting, isFile);
    }

    public static string BuildShaderSettingString(bool isFile, ref bool useBaseShadow, ref bool useOutlineShadow)
    {
        lilToonSetting shaderSetting = null;
        InitializeShaderSetting(ref shaderSetting);
        useBaseShadow = (shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW) || shaderSetting.LIL_FEATURE_BACKLIGHT;
        useOutlineShadow = shaderSetting.LIL_FEATURE_OUTLINE_RECEIVE_SHADOW;
        string shaderSettingString = BuildShaderSettingString(shaderSetting, isFile);
        return shaderSettingString;
    }

    internal static void WalkAllSceneReferencedAssets(Action<UnityEngine.Object> callback)
    {
        var toVisit = new Queue<UnityEngine.Object>();
        var visited = new HashSet<UnityEngine.Object>();

        foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            toVisit.Enqueue(root.transform);
            visited.Add(root.transform);
        }

        // use unity reflection to walk all properties in all objects referenced from the scene
        while (toVisit.Count > 0)
        {
            var next = toVisit.Dequeue();
            if (next == null) continue;
            
            callback.Invoke(next);

            if (next is Transform)
            {
                var t = (Transform)next;
                foreach (Transform child in t)
                {
                    if (!visited.Contains(child))
                    {
                        toVisit.Enqueue(child);
                        visited.Add(child);
                    }
                }

                foreach (var c in t.GetComponents(typeof(Component)))
                {
                    if (!(c is Transform) && !visited.Contains(c))
                    {
                        toVisit.Enqueue(c);
                        visited.Add(c);
                    }
                }
            }
            else
            {
                var so = new SerializedObject(next);
                var prop = so.GetIterator();

                bool enterChildren = true;
                while (prop.Next(enterChildren))
                {
                    enterChildren = true;

                    switch (prop.propertyType)
                    {
                        case SerializedPropertyType.String:
                            enterChildren = false;
                            break;
                        case SerializedPropertyType.ObjectReference:
                        {
                            var obj = prop.objectReferenceValue;
                            if (obj != null && !visited.Contains(obj))
                            {
                                toVisit.Enqueue(obj);
                                visited.Add(obj);
                            }

                            break;
                        }
                    }
                }
            }
        }
    }
    
    internal static void ApplyShaderSettingOptimized(List<Shader> shaders = null)
    {
        lilToonSetting shaderSetting = null;
        InitializeShaderSetting(ref shaderSetting);
        TurnOffAllShaderSetting(ref shaderSetting);

        WalkAllSceneReferencedAssets(obj =>
        {
            if (obj is Material)
            {
                SetupShaderSettingFromMaterial((Material)obj, ref shaderSetting);
            } else if (obj is AnimationClip)
            {
                SetupShaderSettingFromAnimationClip((AnimationClip)obj, ref shaderSetting);
            }
        });

        // Apply
        ApplyShaderSetting(shaderSetting, "[lilToon] PreprocessBuild", shaders);
        AssetDatabase.Refresh();
    }
    
    internal static void GetOptimizedSetting(Material[] materials, AnimationClip[] clips, out string usedShaders, out string optimizedHLSL, out string shaderSettingText)
    {
        usedShaders = null;
        optimizedHLSL = null;
        shaderSettingText = null;

        var shaders = GetShaderListFromGameObject(materials, clips);
        if(shaders.Count() == 0) return;

        usedShaders = string.Join(Environment.NewLine, shaders.Select(s => s.name).Distinct().ToArray());

        lilToonSetting shaderSetting = null;
        InitializeShaderSetting(ref shaderSetting);
        TurnOffAllShaderSetting(ref shaderSetting);

        optimizedHLSL = lilOptimizer.GetOptimizedText(materials, clips);

        // Get materials
        foreach(var material in materials)
        {
            SetupShaderSettingFromMaterial(material, ref shaderSetting);
        }

        // Get animations
        foreach(var clip in clips)
        {
            SetupShaderSettingFromAnimationClip(clip, ref shaderSetting, true);
        }

        shaderSettingText = BuildShaderSettingString(shaderSetting, false);
    }

    #if UNITY_2022_1_OR_NEWER || (UNITY_2023_1_OR_NEWER && !UNITY_2023_2_OR_NEWER)
    private static bool WorkaroundForUsePassBug()
    {
        // Normally, there is no problem if you update Unity.
        // This workaround exists for unusual cases.
        // https://issuetracker.unity3d.com/issues/crash-on-malloc-internal-when-recompiling-a-shadergraph-used-by-another-shader-via-usepass
        var regex = new Regex(@"(\d*)\.(\d*)\.(\d*)");
        var match = regex.Match(Application.unityVersion);

        if(!match.Success) return true;
        var major = int.Parse(match.Groups[1].Value);
        var minor = int.Parse(match.Groups[2].Value);
        var patch = int.Parse(match.Groups[3].Value);

        if(major == 2022 && (minor < 3 || patch < 14)) return true;
        if(major == 2023 && patch < 20) return true;

        return false;
    }
    #endif

    internal static void SetShaderSettingBeforeBuild(Material[] materials, AnimationClip[] clips)
    {
        #if !LILTOON_DISABLE_OPTIMIZATION
        #if UNITY_2022_1_OR_NEWER || (UNITY_2023_1_OR_NEWER && !UNITY_2023_2_OR_NEWER)
            if(WorkaroundForUsePassBug()){ Debug.Log("[lilToon] Skip Optimization"); return; }
        #endif
        try
        {
            #if UNITY_2022_1_OR_NEWER
                var materialParents = new HashSet<Material>();
                foreach(var m in materials)
                {
                    GetMaterialParents(materialParents, m);
                }
                materials = materials.Union(materialParents).ToArray();
            #endif
            if(!ShouldOptimization()) return;
            var shaders = GetShaderListFromGameObject(materials, clips);
            if(shaders.Count() == 0) return;

            lilEditorParameters.instance.modifiedShaders = string.Join(",", shaders.Select(s => s.name).Distinct().ToArray());

            lilToonSetting shaderSetting = null;
            InitializeShaderSetting(ref shaderSetting);
            TurnOffAllShaderSetting(ref shaderSetting);

            lilOptimizer.OptimizeInputHLSL(materials, clips);

            // Get materials
            foreach(var material in materials)
            {
                SetupShaderSettingFromMaterial(material, ref shaderSetting);
            }

            // Get animations
            foreach(var clip in clips)
            {
                SetupShaderSettingFromAnimationClip(clip, ref shaderSetting, true);
            }

            // Apply
            ApplyShaderSetting(shaderSetting, "[lilToon] PreprocessBuild", shaders, true);
            AssetDatabase.Refresh();
        }
        catch(Exception e)
        {
            Debug.LogException(e);
            Debug.Log("[lilToon] SetShaderSettingBeforeBuild() failed");
        }
        #endif
    }

    #if UNITY_2022_1_OR_NEWER
    private static void GetMaterialParents(HashSet<Material> parents, Material material)
    {
        var p = material.parent;
        if(p == null) return;
        parents.Add(p);
        GetMaterialParents(parents, p);
    }
    #endif

    internal static void SetShaderSettingBeforeBuild()
    {
        #if !LILTOON_DISABLE_OPTIMIZATION
        #if UNITY_2022_1_OR_NEWER || (UNITY_2023_1_OR_NEWER && !UNITY_2023_2_OR_NEWER)
            if(WorkaroundForUsePassBug()){ Debug.Log("[lilToon] Skip Optimization"); return; }
        #endif
        try
        {
            if(!ShouldOptimization()) return;
            var shaders = GetShaderListFromProject();
            lilEditorParameters.instance.modifiedShaders = string.Join(",", shaders.Select(s => s.name).Distinct().ToArray());
            ApplyShaderSettingOptimized(shaders);
        }
        catch(Exception e)
        {
            Debug.LogException(e);
            Debug.Log("[lilToon] Optimization failed");
        }
        #endif
    }

    internal static void SetShaderSettingAfterBuild()
    {
        try
        {
            if(string.IsNullOrEmpty(lilEditorParameters.instance.modifiedShaders)) return;
            var shaders = lilEditorParameters.instance.modifiedShaders.Split(',').Select(n => Shader.Find(n)).Where(s => s != null).ToList();
            lilEditorParameters.instance.modifiedShaders = "";
            if(!ShouldOptimization()) return;
            lilEditorParameters.instance.forceOptimize = false;
            lilToonSetting shaderSetting = null;
            InitializeShaderSetting(ref shaderSetting);

            lilOptimizer.ResetInputHLSL();

            if(shaderSetting.isDebugOptimize)
            {
                ApplyShaderSettingOptimized();
            }
            else
            {
                TurnOnAllShaderSetting(ref shaderSetting);
                ApplyShaderSetting(shaderSetting, "[lilToon] PostprocessBuild", shaders, false);
            }
        }
        catch(Exception e)
        {
            Debug.LogException(e);
            Debug.Log("[lilToon] SetShaderSettingAfterBuild() failed");
        }
    }

    internal static void SetupShaderSettingFromMaterial(Material material, ref lilToonSetting shaderSetting)
    {
        if(material == null || material.shader == null) return;
        if(material.shader.name.Contains("Lite") || material.shader.name.Contains("Multi")) return;
        if(!lilMaterialUtils.CheckShaderIslilToon(material.shader)) return;

        if(!shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV && material.HasProperty("_MainTex_ScrollRotate") && material.GetVector("_MainTex_ScrollRotate") != lilConstants.defaultScrollRotate)
        {
            Debug.Log("[lilToon] LIL_FEATURE_ANIMATE_MAIN_UV : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV = true;
        }
        if(!shaderSetting.LIL_FEATURE_SHADOW && material.HasProperty("_UseShadow") && material.GetFloat("_UseShadow") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_SHADOW : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_SHADOW = true;
        }
        if(!shaderSetting.LIL_FEATURE_RIMSHADE && material.HasProperty("_UseRimShade") && material.GetFloat("_UseRimShade") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_RIMSHADE : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_RIMSHADE = true;
        }
        if(!shaderSetting.LIL_FEATURE_RECEIVE_SHADOW && material.HasProperty("_UseShadow") && material.GetFloat("_UseShadow") != 0.0f && (
            (material.HasProperty("_ShadowReceive") && material.GetFloat("_ShadowReceive") > 0.0f) ||
            (material.HasProperty("_Shadow2ndReceive") && material.GetFloat("_Shadow2ndReceive") > 0.0f) ||
            (material.HasProperty("_Shadow3rdReceive") && material.GetFloat("_Shadow3rdReceive") > 0.0f))
        )
        {
            Debug.Log("[lilToon] LIL_FEATURE_RECEIVE_SHADOW : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_RECEIVE_SHADOW = true;
        }
        if(!shaderSetting.LIL_FEATURE_DISTANCE_FADE && material.HasProperty("_DistanceFade") && material.GetVector("_DistanceFade").z != lilConstants.defaultDistanceFadeParams.z)
        {
            Debug.Log("[lilToon] LIL_FEATURE_DISTANCE_FADE : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_DISTANCE_FADE = true;
        }
        if(!shaderSetting.LIL_FEATURE_SHADOW_3RD && material.HasProperty("_Shadow3rdColor") && material.GetColor("_Shadow3rdColor").a != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_SHADOW_3RD : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_SHADOW_3RD = true;
        }
        if(!shaderSetting.LIL_FEATURE_SHADOW_LUT && material.HasProperty("_ShadowColorType") && material.GetFloat("_ShadowColorType") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_SHADOW_LUT : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_SHADOW_LUT = true;
        }

        if(material.shader.name.Contains("Fur"))
        {
            if(!shaderSetting.LIL_FEATURE_FUR_COLLISION && material.HasProperty("_FurTouchStrength") && material.GetFloat("_FurTouchStrength") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_FUR_COLLISION : " + AssetDatabase.GetAssetPath(material));
                Debug.Log("[lilToon] LIL_OPTIMIZE_USE_VERTEXLIGHT : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_FUR_COLLISION = true;
                shaderSetting.LIL_OPTIMIZE_USE_VERTEXLIGHT = true;
            }
        }

        if(!shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION && material.HasProperty("_MainTexHSVG") && material.GetVector("_MainTexHSVG") != lilConstants.defaultHSVG)
        {
            Debug.Log("[lilToon] LIL_FEATURE_MAIN_TONE_CORRECTION : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION = true;
        }
        if(!shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP && material.HasProperty("_MainGradationStrength") && material.GetFloat("_MainGradationStrength") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_MAIN_GRADATION_MAP : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP = true;
        }
        if(!shaderSetting.LIL_FEATURE_MAIN2ND && material.HasProperty("_UseMain2ndTex") && material.GetFloat("_UseMain2ndTex") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_MAIN2ND : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_MAIN2ND = true;
        }
        if(!shaderSetting.LIL_FEATURE_MAIN3RD && material.HasProperty("_UseMain3rdTex") && material.GetFloat("_UseMain3rdTex") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_MAIN3RD : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_MAIN3RD = true;
        }
        if(!shaderSetting.LIL_FEATURE_DECAL && (
            (material.HasProperty("_Main2ndTexIsDecal") && material.GetFloat("_Main2ndTexIsDecal") != 0.0f) ||
            (material.HasProperty("_Main3rdTexIsDecal") && material.GetFloat("_Main3rdTexIsDecal") != 0.0f))
        )
        {
            Debug.Log("[lilToon] LIL_FEATURE_DECAL : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_DECAL = true;
        }
        if(!shaderSetting.LIL_FEATURE_ANIMATE_DECAL && (
            (material.HasProperty("_Main2ndTexDecalAnimation") && material.GetVector("_Main2ndTexDecalAnimation") != lilConstants.defaultDecalAnim) ||
            (material.HasProperty("_Main3rdTexDecalAnimation") && material.GetVector("_Main3rdTexDecalAnimation") != lilConstants.defaultDecalAnim))
        )
        {
            Debug.Log("[lilToon] LIL_FEATURE_ANIMATE_DECAL : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_ANIMATE_DECAL = true;
        }
        if(!shaderSetting.LIL_FEATURE_LAYER_DISSOLVE && (
            (material.HasProperty("_Main2ndDissolveParams") && material.GetVector("_Main2ndDissolveParams").x != lilConstants.defaultDissolveParams.x) ||
            (material.HasProperty("_Main3rdDissolveParams") && material.GetVector("_Main3rdDissolveParams").x != lilConstants.defaultDissolveParams.x))
        )
        {
            Debug.Log("[lilToon] LIL_FEATURE_LAYER_DISSOLVE : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_LAYER_DISSOLVE = true;
        }
        if(!shaderSetting.LIL_FEATURE_ALPHAMASK && material.HasProperty("_AlphaMaskMode") && material.GetFloat("_AlphaMaskMode") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_ALPHAMASK : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_ALPHAMASK = true;
        }
        if(!shaderSetting.LIL_FEATURE_EMISSION_1ST && material.HasProperty("_UseEmission") && material.GetFloat("_UseEmission") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_EMISSION_1ST : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_EMISSION_1ST = true;
        }
        if(!shaderSetting.LIL_FEATURE_EMISSION_2ND && material.HasProperty("_UseEmission2nd") && material.GetFloat("_UseEmission2nd") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_EMISSION_2ND : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_EMISSION_2ND = true;
        }
        if(!shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV && (
            (material.HasProperty("_EmissionMap_ScrollRotate") && material.GetVector("_EmissionMap_ScrollRotate") != lilConstants.defaultScrollRotate) ||
            (material.HasProperty("_Emission2ndMap_ScrollRotate") && material.GetVector("_Emission2ndMap_ScrollRotate") != lilConstants.defaultScrollRotate))
        )
        {
            Debug.Log("[lilToon] LIL_FEATURE_ANIMATE_EMISSION_UV : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV = true;
        }
        if(!shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV && (
            (material.HasProperty("_EmissionBlendMask_ScrollRotate") && material.GetVector("_EmissionBlendMask_ScrollRotate") != lilConstants.defaultScrollRotate) ||
            (material.HasProperty("_Emission2ndBlendMask_ScrollRotate") && material.GetVector("_Emission2ndBlendMask_ScrollRotate") != lilConstants.defaultScrollRotate))
        )
        {
            Debug.Log("[lilToon] LIL_FEATURE_ANIMATE_EMISSION_MASK_UV : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV = true;
        }
        if(!shaderSetting.LIL_FEATURE_EMISSION_GRADATION && (
            (material.HasProperty("_EmissionUseGrad") && material.GetFloat("_EmissionUseGrad") != 0.0f) ||
            (material.HasProperty("_Emission2ndUseGrad") && material.GetFloat("_Emission2ndUseGrad") != 0.0f))
        )
        {
            Debug.Log("[lilToon] LIL_FEATURE_EMISSION_GRADATION : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_EMISSION_GRADATION = true;
        }
        if(!shaderSetting.LIL_FEATURE_NORMAL_1ST && material.HasProperty("_UseBumpMap") && material.GetFloat("_UseBumpMap") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_NORMAL_1ST : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_NORMAL_1ST = true;
        }
        if(!shaderSetting.LIL_FEATURE_NORMAL_2ND && material.HasProperty("_UseBump2ndMap") && material.GetFloat("_UseBump2ndMap") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_NORMAL_2ND : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_NORMAL_2ND = true;
        }
        if(!shaderSetting.LIL_FEATURE_ANISOTROPY && material.HasProperty("_UseAnisotropy") && material.GetFloat("_UseAnisotropy") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_ANISOTROPY : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_ANISOTROPY = true;
        }
        if(!shaderSetting.LIL_FEATURE_REFLECTION && material.HasProperty("_UseReflection") && material.GetFloat("_UseReflection") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_REFLECTION : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_REFLECTION = true;
        }
        if(!shaderSetting.LIL_FEATURE_MATCAP && material.HasProperty("_UseMatCap") && material.GetFloat("_UseMatCap") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_MATCAP : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_MATCAP = true;
        }
        if(!shaderSetting.LIL_FEATURE_MATCAP_2ND && material.HasProperty("_UseMatCap2nd") && material.GetFloat("_UseMatCap2nd") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_MATCAP_2ND : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_MATCAP_2ND = true;
        }
        if(!shaderSetting.LIL_FEATURE_RIMLIGHT && material.HasProperty("_UseRim") && material.GetFloat("_UseRim") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_RIMLIGHT : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_RIMLIGHT = true;
        }
        if(!shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION && material.HasProperty("_RimDirStrength") && material.GetFloat("_RimDirStrength") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_RIMLIGHT_DIRECTION : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION = true;
        }
        if(!shaderSetting.LIL_FEATURE_GLITTER && material.HasProperty("_UseGlitter") && material.GetFloat("_UseGlitter") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_GLITTER : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_GLITTER = true;
        }
        if(!shaderSetting.LIL_FEATURE_BACKLIGHT && material.HasProperty("_UseBacklight") && material.GetFloat("_UseBacklight") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_BACKLIGHT : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_BACKLIGHT = true;
        }
        if(!shaderSetting.LIL_FEATURE_PARALLAX && material.HasProperty("_UseParallax") && material.GetFloat("_UseParallax") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_PARALLAX : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_PARALLAX = true;
            if(!shaderSetting.LIL_FEATURE_POM && material.HasProperty("_UsePOM") && material.GetFloat("_UsePOM") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_POM : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_POM = true;
            }
        }
        if(!shaderSetting.LIL_FEATURE_AUDIOLINK && material.HasProperty("_UseAudioLink") && material.GetFloat("_UseAudioLink") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_AUDIOLINK : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_AUDIOLINK = true;
        }
        if(!shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX && material.HasProperty("_AudioLink2Vertex") && material.GetFloat("_AudioLink2Vertex") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_AUDIOLINK_VERTEX : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX = true;
        }
        if(!shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL && material.HasProperty("_AudioLinkAsLocal") && material.GetFloat("_AudioLinkAsLocal") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_AUDIOLINK_LOCAL : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL = true;
        }
        if(!shaderSetting.LIL_FEATURE_DISSOLVE && material.HasProperty("_DissolveParams") && material.GetVector("_DissolveParams").x != lilConstants.defaultDissolveParams.x)
        {
            Debug.Log("[lilToon] LIL_FEATURE_DISSOLVE : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_DISSOLVE = true;
        }
        if(!shaderSetting.LIL_FEATURE_DITHER && material.HasProperty("_UseDither") && material.GetFloat("_UseDither") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_DITHER : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_DITHER = true;
        }
        if(!shaderSetting.LIL_FEATURE_IDMASK && (
            material.HasProperty("_IDMask1") && material.GetFloat("_IDMask1") != 0.0f ||
            material.HasProperty("_IDMask2") && material.GetFloat("_IDMask2") != 0.0f ||
            material.HasProperty("_IDMask3") && material.GetFloat("_IDMask3") != 0.0f ||
            material.HasProperty("_IDMask4") && material.GetFloat("_IDMask4") != 0.0f ||
            material.HasProperty("_IDMask5") && material.GetFloat("_IDMask5") != 0.0f ||
            material.HasProperty("_IDMask6") && material.GetFloat("_IDMask6") != 0.0f ||
            material.HasProperty("_IDMask7") && material.GetFloat("_IDMask7") != 0.0f ||
            material.HasProperty("_IDMask8") && material.GetFloat("_IDMask8") != 0.0f ||
            material.HasProperty("_IDMaskIsBitmap") && material.GetFloat("_IDMaskIsBitmap") != 0.0f ||
            material.HasProperty("_IDMaskCompile") && material.GetFloat("_IDMaskCompile") != 0.0f
        ))
        {
            Debug.Log("[lilToon] LIL_FEATURE_IDMASK : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_IDMASK = true;
        }

        if(!shaderSetting.LIL_FEATURE_UDIMDISCARD && material.HasProperty("_UDIMDiscardCompile") && material.GetFloat("_UDIMDiscardCompile") != 0.0f) {
            Debug.Log("[lilToon] LIL_FEATURE_UDIMDISCARD : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_UDIMDISCARD = true;
        }
           
        // Outline
        if(material.shader.name.Contains("Outline"))
        {
            if(!shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV && material.HasProperty("_OutlineTex_ScrollRotate") && material.GetVector("_OutlineTex_ScrollRotate") != lilConstants.defaultScrollRotate)
            {
                Debug.Log("[lilToon] LIL_FEATURE_ANIMATE_OUTLINE_UV : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV = true;
            }
            if(!shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION && material.HasProperty("_OutlineTexHSVG") && material.GetVector("_OutlineTexHSVG") != lilConstants.defaultHSVG)
            {
                Debug.Log("[lilToon] LIL_FEATURE_OUTLINE_TONE_CORRECTION : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION = true;
            }
            if(!shaderSetting.LIL_FEATURE_OUTLINE_RECEIVE_SHADOW && material.HasProperty("_OutlineLitShadowReceive") && material.GetFloat("_OutlineLitShadowReceive") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_OUTLINE_RECEIVE_SHADOW : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_OUTLINE_RECEIVE_SHADOW = true;
            }
        }

        if(!shaderSetting.LIL_OPTIMIZE_USE_VERTEXLIGHT && material.HasProperty("_VertexLightStrength") && material.GetFloat("_VertexLightStrength") != 0.0f)
        {
            Debug.Log("[lilToon] LIL_OPTIMIZE_USE_VERTEXLIGHT : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_OPTIMIZE_USE_VERTEXLIGHT = true;
        }

        // Texture
        CheckTextures(ref shaderSetting, material);
    }

    private static void SetupShaderSettingFromAnimationClip(AnimationClip clip, ref lilToonSetting shaderSetting, bool shouldCheckMaterial = false)
    {
        if(clip == null) return;

        if(shouldCheckMaterial)
        {
            foreach(var frame in AnimationUtility.GetObjectReferenceCurveBindings(clip).SelectMany(b => AnimationUtility.GetObjectReferenceCurve(clip, b)).Where(f => f.value is Material))
            {
                SetupShaderSettingFromMaterial((Material)frame.value, ref shaderSetting);
            }
        }

        foreach(var binding in AnimationUtility.GetCurveBindings(clip))
        {
            string propname = binding.propertyName;
            if(string.IsNullOrEmpty(propname) || !propname.Contains("material.")) continue;

            shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV = shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV || propname.Contains("_MainTex_ScrollRotate");
            shaderSetting.LIL_FEATURE_SHADOW = shaderSetting.LIL_FEATURE_SHADOW || propname.Contains("_UseShadow");
            shaderSetting.LIL_FEATURE_RECEIVE_SHADOW = shaderSetting.LIL_FEATURE_RECEIVE_SHADOW || propname.Contains("_ShadowReceive") || propname.Contains("_Shadow2ndReceive") || propname.Contains("_Shadow3rdReceive");
            shaderSetting.LIL_FEATURE_DISTANCE_FADE = shaderSetting.LIL_FEATURE_DISTANCE_FADE || propname.Contains("_DistanceFade");
            shaderSetting.LIL_FEATURE_SHADOW_3RD = shaderSetting.LIL_FEATURE_SHADOW_3RD || propname.Contains("_Shadow3rdColor");
            shaderSetting.LIL_FEATURE_SHADOW_LUT = shaderSetting.LIL_FEATURE_SHADOW_LUT || propname.Contains("_ShadowColorType");
            shaderSetting.LIL_FEATURE_RIMSHADE = shaderSetting.LIL_FEATURE_RIMSHADE || propname.Contains("_UseRimShade");

            shaderSetting.LIL_FEATURE_FUR_COLLISION = shaderSetting.LIL_FEATURE_FUR_COLLISION || propname.Contains("_FurTouchStrength");

            shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION = shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION || propname.Contains("_MainTexHSVG");
            shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP = shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP || propname.Contains("_MainGradationStrength");
            shaderSetting.LIL_FEATURE_MAIN2ND = shaderSetting.LIL_FEATURE_MAIN2ND || propname.Contains("_UseMain2ndTex");
            shaderSetting.LIL_FEATURE_MAIN3RD = shaderSetting.LIL_FEATURE_MAIN3RD || propname.Contains("_UseMain3rdTex");
            shaderSetting.LIL_FEATURE_DECAL = shaderSetting.LIL_FEATURE_DECAL || propname.Contains("_Main2ndTexIsDecal") || propname.Contains("_Main3rdTexIsDecal");
            shaderSetting.LIL_FEATURE_ANIMATE_DECAL = shaderSetting.LIL_FEATURE_ANIMATE_DECAL || propname.Contains("_Main2ndTexDecalAnimation") || propname.Contains("_Main3rdTexDecalAnimation");
            shaderSetting.LIL_FEATURE_LAYER_DISSOLVE = shaderSetting.LIL_FEATURE_LAYER_DISSOLVE || propname.Contains("_Main2ndDissolveParams") || propname.Contains("_Main3rdDissolveParams");
            shaderSetting.LIL_FEATURE_ALPHAMASK = shaderSetting.LIL_FEATURE_ALPHAMASK || propname.Contains("_AlphaMaskMode");
            shaderSetting.LIL_FEATURE_EMISSION_1ST = shaderSetting.LIL_FEATURE_EMISSION_1ST || propname.Contains("_UseEmission");
            shaderSetting.LIL_FEATURE_EMISSION_2ND = shaderSetting.LIL_FEATURE_EMISSION_2ND || propname.Contains("_UseEmission2nd");
            shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV = shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV || propname.Contains("_EmissionMap_ScrollRotate") || propname.Contains("_Emission2ndMap_ScrollRotate");
            shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV = shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV || propname.Contains("_EmissionBlendMask_ScrollRotate") || propname.Contains("_Emission2ndBlendMask_ScrollRotate");
            shaderSetting.LIL_FEATURE_EMISSION_GRADATION = shaderSetting.LIL_FEATURE_EMISSION_GRADATION || propname.Contains("_EmissionUseGrad");
            shaderSetting.LIL_FEATURE_NORMAL_1ST = shaderSetting.LIL_FEATURE_NORMAL_1ST || propname.Contains("_UseBumpMap");
            shaderSetting.LIL_FEATURE_NORMAL_2ND = shaderSetting.LIL_FEATURE_NORMAL_2ND || propname.Contains("_UseBump2ndMap");
            shaderSetting.LIL_FEATURE_ANISOTROPY = shaderSetting.LIL_FEATURE_ANISOTROPY || propname.Contains("_UseAnisotropy");
            shaderSetting.LIL_FEATURE_REFLECTION = shaderSetting.LIL_FEATURE_REFLECTION || propname.Contains("_UseReflection");
            shaderSetting.LIL_FEATURE_MATCAP = shaderSetting.LIL_FEATURE_MATCAP || propname.Contains("_UseMatCap");
            shaderSetting.LIL_FEATURE_MATCAP_2ND = shaderSetting.LIL_FEATURE_MATCAP_2ND || propname.Contains("_UseMatCap2nd");
            shaderSetting.LIL_FEATURE_RIMLIGHT = shaderSetting.LIL_FEATURE_RIMLIGHT || propname.Contains("_UseRim");
            shaderSetting.LIL_FEATURE_GLITTER = shaderSetting.LIL_FEATURE_GLITTER || propname.Contains("_UseGlitter");
            shaderSetting.LIL_FEATURE_BACKLIGHT = shaderSetting.LIL_FEATURE_BACKLIGHT || propname.Contains("_UseBacklight");
            shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION = shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION || propname.Contains("_RimDirStrength");
            shaderSetting.LIL_FEATURE_PARALLAX = shaderSetting.LIL_FEATURE_PARALLAX || propname.Contains("_UseParallax");
            shaderSetting.LIL_FEATURE_POM = shaderSetting.LIL_FEATURE_POM || propname.Contains("_UsePOM");
            shaderSetting.LIL_FEATURE_AUDIOLINK = shaderSetting.LIL_FEATURE_AUDIOLINK || propname.Contains("_UseAudioLink");
            shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX = shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX || propname.Contains("_AudioLink2Vertex");
            shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL = shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL || propname.Contains("_AudioLinkAsLocal");
            shaderSetting.LIL_FEATURE_DISSOLVE = shaderSetting.LIL_FEATURE_DISSOLVE || propname.Contains("_DissolveParams");
            shaderSetting.LIL_FEATURE_DITHER = shaderSetting.LIL_FEATURE_DITHER || propname.Contains("_UseDither");

            if(!shaderSetting.LIL_FEATURE_IDMASK && (
                propname.Contains("_IDMask1") || propname.Contains("_IDMaskIndex1") ||
                propname.Contains("_IDMask2") || propname.Contains("_IDMaskIndex2") ||
                propname.Contains("_IDMask3") || propname.Contains("_IDMaskIndex3") ||
                propname.Contains("_IDMask4") || propname.Contains("_IDMaskIndex4") ||
                propname.Contains("_IDMask5") || propname.Contains("_IDMaskIndex5") ||
                propname.Contains("_IDMask6") || propname.Contains("_IDMaskIndex6") ||
                propname.Contains("_IDMask7") || propname.Contains("_IDMaskIndex7") ||
                propname.Contains("_IDMask8") || propname.Contains("_IDMaskIndex8")
            ))
            {
                shaderSetting.LIL_FEATURE_IDMASK = true;
            }

            shaderSetting.LIL_FEATURE_UDIMDISCARD = shaderSetting.LIL_FEATURE_UDIMDISCARD || propname.Contains("_UDIMDiscardCompile");
            
            shaderSetting.LIL_FEATURE_ENCRYPTION = shaderSetting.LIL_FEATURE_ENCRYPTION || propname.Contains("_BitKey0");

            shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV = shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV || propname.Contains("_OutlineTex_ScrollRotate");
            shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION = shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION || propname.Contains("_OutlineTexHSVG");
            shaderSetting.LIL_FEATURE_OUTLINE_RECEIVE_SHADOW = shaderSetting.LIL_FEATURE_OUTLINE_RECEIVE_SHADOW || propname.Contains("_OutlineLitShadowReceive");

            shaderSetting.LIL_OPTIMIZE_USE_VERTEXLIGHT = shaderSetting.LIL_OPTIMIZE_USE_VERTEXLIGHT || shaderSetting.LIL_FEATURE_FUR_COLLISION || propname.Contains("_VertexLightStrength");

            // Texture
            CheckTextures(ref shaderSetting, propname);
        }
    }

    internal static void CheckTextures(ref lilToonSetting shaderSetting)
    {
        // Get materials
        foreach(var material in lilDirectoryManager.FindAssets<Material>("t:material"))
        {
            CheckTextures(ref shaderSetting, material);
        }

        // Get animations
        foreach(var propname in lilDirectoryManager.FindAssets<AnimationClip>("t:animationclip").SelectMany(c => AnimationUtility.GetCurveBindings(c)).Select(b => b.propertyName).Where(n => !string.IsNullOrEmpty(n) && n.Contains("material.")))
        {
            CheckTextures(ref shaderSetting, propname);
        }
    }

    internal static void CheckTextures(ref lilToonSetting shaderSetting, Material material)
    {
        CheckTexture(ref shaderSetting.LIL_FEATURE_MainGradationTex          , "_MainGradationTex", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_MainColorAdjustMask       , "_MainColorAdjustMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_Main2ndTex                , "_Main2ndTex", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_Main2ndBlendMask          , "_Main2ndBlendMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_Main2ndDissolveMask       , "_Main2ndDissolveMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_Main2ndDissolveNoiseMask  , "_Main2ndDissolveNoiseMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_Main3rdTex                , "_Main3rdTex", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_Main3rdBlendMask          , "_Main3rdBlendMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_Main3rdDissolveMask       , "_Main3rdDissolveMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_Main3rdDissolveNoiseMask  , "_Main3rdDissolveNoiseMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_AlphaMask                 , "_AlphaMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_BumpMap                   , "_BumpMap", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_Bump2ndMap                , "_Bump2ndMap", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_Bump2ndScaleMask          , "_Bump2ndScaleMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_AnisotropyTangentMap      , "_AnisotropyTangentMap", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_AnisotropyScaleMask       , "_AnisotropyScaleMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_AnisotropyShiftNoiseMask  , "_AnisotropyShiftNoiseMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_ShadowBorderMask          , "_ShadowBorderMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_ShadowBlurMask            , "_ShadowBlurMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_ShadowStrengthMask        , "_ShadowStrengthMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_ShadowColorTex            , "_ShadowColorTex", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_Shadow2ndColorTex         , "_Shadow2ndColorTex", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_Shadow3rdColorTex         , "_Shadow3rdColorTex", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_BacklightColorTex         , "_BacklightColorTex", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_SmoothnessTex             , "_SmoothnessTex", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_MetallicGlossMap          , "_MetallicGlossMap", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_ReflectionColorTex        , "_ReflectionColorTex", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_ReflectionCubeTex         , "_ReflectionCubeTex", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_MatCapTex                 , "_MatCapTex", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_MatCapBlendMask           , "_MatCapBlendMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_MatCapBumpMap             , "_MatCapBumpMap", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_MatCap2ndTex              , "_MatCap2ndTex", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_MatCap2ndBlendMask        , "_MatCap2ndBlendMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_MatCap2ndBumpMap          , "_MatCap2ndBumpMap", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_RimColorTex               , "_RimColorTex", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_GlitterColorTex           , "_GlitterColorTex", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_GlitterShapeTex           , "_GlitterShapeTex", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_EmissionMap               , "_EmissionMap", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_EmissionBlendMask         , "_EmissionBlendMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_EmissionGradTex           , "_EmissionGradTex", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_Emission2ndMap            , "_Emission2ndMap", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_Emission2ndBlendMask      , "_Emission2ndBlendMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_Emission2ndGradTex        , "_Emission2ndGradTex", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_ParallaxMap               , "_ParallaxMap", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_AudioLinkMask             , "_AudioLinkMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_AudioLinkLocalMap         , "_AudioLinkLocalMap", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_DissolveMask              , "_DissolveMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_DissolveNoiseMask         , "_DissolveNoiseMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_OutlineTex                , "_OutlineTex", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_OutlineWidthMask          , "_OutlineWidthMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_OutlineVectorTex          , "_OutlineVectorTex", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_FurNoiseMask              , "_FurNoiseMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_FurMask                   , "_FurMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_FurLengthMask             , "_FurLengthMask", material);
        CheckTexture(ref shaderSetting.LIL_FEATURE_FurVectorTex              , "_FurVectorTex", material);
    }

    internal static void CheckTextures(ref lilToonSetting shaderSetting, string propname)
    {
        shaderSetting.LIL_FEATURE_MainGradationTex           = shaderSetting.LIL_FEATURE_MainGradationTex         || propname.Contains("_MainGradationTex");
        shaderSetting.LIL_FEATURE_MainColorAdjustMask        = shaderSetting.LIL_FEATURE_MainColorAdjustMask      || propname.Contains("_MainColorAdjustMask");
        shaderSetting.LIL_FEATURE_Main2ndTex                 = shaderSetting.LIL_FEATURE_Main2ndTex               || propname.Contains("_Main2ndTex");
        shaderSetting.LIL_FEATURE_Main2ndBlendMask           = shaderSetting.LIL_FEATURE_Main2ndBlendMask         || propname.Contains("_Main2ndBlendMask");
        shaderSetting.LIL_FEATURE_Main2ndDissolveMask        = shaderSetting.LIL_FEATURE_Main2ndDissolveMask      || propname.Contains("_Main2ndDissolveMask");
        shaderSetting.LIL_FEATURE_Main2ndDissolveNoiseMask   = shaderSetting.LIL_FEATURE_Main2ndDissolveNoiseMask || propname.Contains("_Main2ndDissolveNoiseMask");
        shaderSetting.LIL_FEATURE_Main3rdTex                 = shaderSetting.LIL_FEATURE_Main3rdTex               || propname.Contains("_Main3rdTex");
        shaderSetting.LIL_FEATURE_Main3rdBlendMask           = shaderSetting.LIL_FEATURE_Main3rdBlendMask         || propname.Contains("_Main3rdBlendMask");
        shaderSetting.LIL_FEATURE_Main3rdDissolveMask        = shaderSetting.LIL_FEATURE_Main3rdDissolveMask      || propname.Contains("_Main3rdDissolveMask");
        shaderSetting.LIL_FEATURE_Main3rdDissolveNoiseMask   = shaderSetting.LIL_FEATURE_Main3rdDissolveNoiseMask || propname.Contains("_Main3rdDissolveNoiseMask");
        shaderSetting.LIL_FEATURE_AlphaMask                  = shaderSetting.LIL_FEATURE_AlphaMask                || propname.Contains("_AlphaMask");
        shaderSetting.LIL_FEATURE_BumpMap                    = shaderSetting.LIL_FEATURE_BumpMap                  || propname.Contains("_BumpMap");
        shaderSetting.LIL_FEATURE_Bump2ndMap                 = shaderSetting.LIL_FEATURE_Bump2ndMap               || propname.Contains("_Bump2ndMap");
        shaderSetting.LIL_FEATURE_Bump2ndScaleMask           = shaderSetting.LIL_FEATURE_Bump2ndScaleMask         || propname.Contains("_Bump2ndScaleMask");
        shaderSetting.LIL_FEATURE_AnisotropyTangentMap       = shaderSetting.LIL_FEATURE_AnisotropyTangentMap     || propname.Contains("_AnisotropyTangentMap");
        shaderSetting.LIL_FEATURE_AnisotropyScaleMask        = shaderSetting.LIL_FEATURE_AnisotropyScaleMask      || propname.Contains("_AnisotropyScaleMask");
        shaderSetting.LIL_FEATURE_AnisotropyShiftNoiseMask   = shaderSetting.LIL_FEATURE_AnisotropyShiftNoiseMask || propname.Contains("_AnisotropyShiftNoiseMask");
        shaderSetting.LIL_FEATURE_ShadowBorderMask           = shaderSetting.LIL_FEATURE_ShadowBorderMask         || propname.Contains("_ShadowBorderMask");
        shaderSetting.LIL_FEATURE_ShadowBlurMask             = shaderSetting.LIL_FEATURE_ShadowBlurMask           || propname.Contains("_ShadowBlurMask");
        shaderSetting.LIL_FEATURE_ShadowStrengthMask         = shaderSetting.LIL_FEATURE_ShadowStrengthMask       || propname.Contains("_ShadowStrengthMask");
        shaderSetting.LIL_FEATURE_ShadowColorTex             = shaderSetting.LIL_FEATURE_ShadowColorTex           || propname.Contains("_ShadowColorTex");
        shaderSetting.LIL_FEATURE_Shadow2ndColorTex          = shaderSetting.LIL_FEATURE_Shadow2ndColorTex        || propname.Contains("_Shadow2ndColorTex");
        shaderSetting.LIL_FEATURE_Shadow3rdColorTex          = shaderSetting.LIL_FEATURE_Shadow3rdColorTex        || propname.Contains("_Shadow3rdColorTex");
        shaderSetting.LIL_FEATURE_BacklightColorTex          = shaderSetting.LIL_FEATURE_BacklightColorTex        || propname.Contains("_BacklightColorTex");
        shaderSetting.LIL_FEATURE_SmoothnessTex              = shaderSetting.LIL_FEATURE_SmoothnessTex            || propname.Contains("_SmoothnessTex");
        shaderSetting.LIL_FEATURE_MetallicGlossMap           = shaderSetting.LIL_FEATURE_MetallicGlossMap         || propname.Contains("_MetallicGlossMap");
        shaderSetting.LIL_FEATURE_ReflectionColorTex         = shaderSetting.LIL_FEATURE_ReflectionColorTex       || propname.Contains("_ReflectionColorTex");
        shaderSetting.LIL_FEATURE_ReflectionCubeTex          = shaderSetting.LIL_FEATURE_ReflectionCubeTex        || propname.Contains("_ReflectionCubeTex");
        shaderSetting.LIL_FEATURE_MatCapTex                  = shaderSetting.LIL_FEATURE_MatCapTex                || propname.Contains("_MatCapTex");
        shaderSetting.LIL_FEATURE_MatCapBlendMask            = shaderSetting.LIL_FEATURE_MatCapBlendMask          || propname.Contains("_MatCapBlendMask");
        shaderSetting.LIL_FEATURE_MatCapBumpMap              = shaderSetting.LIL_FEATURE_MatCapBumpMap            || propname.Contains("_MatCapBumpMap");
        shaderSetting.LIL_FEATURE_MatCap2ndTex               = shaderSetting.LIL_FEATURE_MatCap2ndTex             || propname.Contains("_MatCap2ndTex");
        shaderSetting.LIL_FEATURE_MatCap2ndBlendMask         = shaderSetting.LIL_FEATURE_MatCap2ndBlendMask       || propname.Contains("_MatCap2ndBlendMask");
        shaderSetting.LIL_FEATURE_MatCap2ndBumpMap           = shaderSetting.LIL_FEATURE_MatCap2ndBumpMap         || propname.Contains("_MatCap2ndBumpMap");
        shaderSetting.LIL_FEATURE_RimColorTex                = shaderSetting.LIL_FEATURE_RimColorTex              || propname.Contains("_RimColorTex");
        shaderSetting.LIL_FEATURE_GlitterColorTex            = shaderSetting.LIL_FEATURE_GlitterColorTex          || propname.Contains("_GlitterColorTex");
        shaderSetting.LIL_FEATURE_GlitterShapeTex            = shaderSetting.LIL_FEATURE_GlitterShapeTex          || propname.Contains("_GlitterShapeTex");
        shaderSetting.LIL_FEATURE_EmissionMap                = shaderSetting.LIL_FEATURE_EmissionMap              || propname.Contains("_EmissionMap");
        shaderSetting.LIL_FEATURE_EmissionBlendMask          = shaderSetting.LIL_FEATURE_EmissionBlendMask        || propname.Contains("_EmissionBlendMask");
        shaderSetting.LIL_FEATURE_EmissionGradTex            = shaderSetting.LIL_FEATURE_EmissionGradTex          || propname.Contains("_EmissionGradTex");
        shaderSetting.LIL_FEATURE_Emission2ndMap             = shaderSetting.LIL_FEATURE_Emission2ndMap           || propname.Contains("_Emission2ndMap");
        shaderSetting.LIL_FEATURE_Emission2ndBlendMask       = shaderSetting.LIL_FEATURE_Emission2ndBlendMask     || propname.Contains("_Emission2ndBlendMask");
        shaderSetting.LIL_FEATURE_Emission2ndGradTex         = shaderSetting.LIL_FEATURE_Emission2ndGradTex       || propname.Contains("_Emission2ndGradTex");
        shaderSetting.LIL_FEATURE_ParallaxMap                = shaderSetting.LIL_FEATURE_ParallaxMap              || propname.Contains("_ParallaxMap");
        shaderSetting.LIL_FEATURE_AudioLinkMask              = shaderSetting.LIL_FEATURE_AudioLinkMask            || propname.Contains("_AudioLinkMask");
        shaderSetting.LIL_FEATURE_AudioLinkLocalMap          = shaderSetting.LIL_FEATURE_AudioLinkLocalMap        || propname.Contains("_AudioLinkLocalMap");
        shaderSetting.LIL_FEATURE_DissolveMask               = shaderSetting.LIL_FEATURE_DissolveMask             || propname.Contains("_DissolveMask");
        shaderSetting.LIL_FEATURE_DissolveNoiseMask          = shaderSetting.LIL_FEATURE_DissolveNoiseMask        || propname.Contains("_DissolveNoiseMask");
        shaderSetting.LIL_FEATURE_OutlineTex                 = shaderSetting.LIL_FEATURE_OutlineTex               || propname.Contains("_OutlineTex");
        shaderSetting.LIL_FEATURE_OutlineWidthMask           = shaderSetting.LIL_FEATURE_OutlineWidthMask         || propname.Contains("_OutlineWidthMask");
        shaderSetting.LIL_FEATURE_OutlineVectorTex           = shaderSetting.LIL_FEATURE_OutlineVectorTex         || propname.Contains("_OutlineVectorTex");
        shaderSetting.LIL_FEATURE_FurNoiseMask               = shaderSetting.LIL_FEATURE_FurNoiseMask             || propname.Contains("_FurNoiseMask");
        shaderSetting.LIL_FEATURE_FurMask                    = shaderSetting.LIL_FEATURE_FurMask                  || propname.Contains("_FurMask");
        shaderSetting.LIL_FEATURE_FurLengthMask              = shaderSetting.LIL_FEATURE_FurLengthMask            || propname.Contains("_FurLengthMask");
        shaderSetting.LIL_FEATURE_FurVectorTex               = shaderSetting.LIL_FEATURE_FurVectorTex             || propname.Contains("_FurVectorTex");
    }

    private static void CheckTexture(ref bool LIL_FEATURE_Tex, string propname, Material material)
    {
        if(LIL_FEATURE_Tex || !material.HasProperty(propname) || material.GetTexture(propname) == null) return;
        Debug.Log("[lilToon] " + propname + " : " + AssetDatabase.GetAssetPath(material));
        LIL_FEATURE_Tex = true;
    }

    internal static bool ShouldOptimization()
    {
        if(!string.IsNullOrEmpty(lilEditorParameters.instance.modifiedShaders)) return false;
        if(lilEditorParameters.instance.forceOptimize) return true;

        lilToonSetting shaderSetting = null;
        InitializeShaderSetting(ref shaderSetting);
        #if LILTOON_VRCSDK3_AVATARS
            return shaderSetting.isOptimizeInTestBuild && !shaderSetting.isDebugOptimize;
        #else
            return !shaderSetting.isDebugOptimize;
        #endif
    }

    private static List<Shader> GetShaderListFromProject()
    {
        return GetTrueShaderLists(lilDirectoryManager.FindAssets<Material>("t:material").Where(m => lilMaterialUtils.CheckShaderIslilToon(m)).Select(m => m.shader).Distinct().ToList());
    }

    private static List<Shader> GetShaderListFromGameObject(Material[] materials, AnimationClip[] clips)
    {
        var shaders = materials.Where(m => lilMaterialUtils.CheckShaderIslilToon(m)).Select(m => m.shader).ToList();

        foreach(var clip in clips)
        {
            CheckAnimationClip(clip, shaders);
        }

        return GetTrueShaderLists(shaders);
    }

    private static List<Shader> GetTrueShaderLists(List<Shader> shaders)
    {
        shaders = shaders.Distinct().ToList();
        foreach(var path in shaders.Select(s => AssetDatabase.GetAssetPath(s)).Where(p => !string.IsNullOrEmpty(p)).ToArray())
        {
            TextReader sr;
            if(path.Contains(".lilcontainer"))
            {
                sr = new StringReader(lilShaderContainer.UnpackContainer(path));
            }
            else
            {
                sr = new StreamReader(path);
            }
            string line;
            bool isComment = false;
            while((line = sr.ReadLine()) != null)
            {
                isComment = line.Contains("/*");
                if(isComment)
                {
                    if(!line.Contains("*/")) continue;
                    isComment = false;
                    line = line.Substring(line.IndexOf("*/") + 2);
                }
                line = line.Trim();
                if(!line.StartsWith("UsePass")) continue;
                int first = line.IndexOf('"') + 1;
                int second = line.IndexOf('"', first);
                if(line.Substring(0, first).Contains("//")) continue;
                string shaderName = line.Substring(first, second - first);
                int passNameSep = shaderName.LastIndexOf('/');
                shaderName = shaderName.Substring(0, passNameSep);
                var usePassShader = Shader.Find(shaderName);
                if(usePassShader != null) shaders.Add(usePassShader);
            }
            sr.Close();
        }
        return shaders.Distinct().ToList();
    }

    private static void CheckAnimationClip(AnimationClip clip, List<Shader> shaders)
    {
        shaders.AddRange(AnimationUtility.GetObjectReferenceCurveBindings(clip).SelectMany(b => AnimationUtility.GetObjectReferenceCurve(clip, b)).Where(f => lilMaterialUtils.CheckShaderIslilToon(f.value as Material)).Select(f => ((Material)f.value).shader));
    }
}
#endif
