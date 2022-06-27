#if UNITY_EDITOR
using lilToon;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
#if VRC_SDK_VRCSDK3 && !UDON
    using VRC.SDK3.Avatars.Components;
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
    public bool LIL_OPTIMIZE_USE_VERTEXLIGHT = true;
    public bool LIL_OPTIMIZE_USE_LIGHTMAP = false;

    public bool isLocked = false;
    public bool isDebugOptimize = false;
    public bool isOptimizeInTestBuild = false;

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

    public static void SaveShaderSetting(lilToonSetting shaderSetting)
    {
        string shaderSettingPath = lilDirectoryManager.GetShaderSettingPath();
        StreamWriter sw = new StreamWriter(shaderSettingPath, false);
        sw.Write(JsonUtility.ToJson(shaderSetting));
        sw.Close();
    }

    internal static void LoadShaderSetting(ref lilToonSetting shaderSetting)
    {
        string shaderSettingPath = lilDirectoryManager.GetShaderSettingPath();
        if(shaderSetting == null) shaderSetting = ScriptableObject.CreateInstance<lilToonSetting>();
        if(File.Exists(shaderSettingPath)) JsonUtility.FromJsonOverwrite(File.ReadAllText(shaderSettingPath), shaderSetting);
    }

    internal static void InitializeShaderSetting(ref lilToonSetting shaderSetting)
    {
        if(shaderSetting != null) return;
        LoadShaderSetting(ref shaderSetting);
        if(shaderSetting == null)
        {
            foreach(string guid in AssetDatabase.FindAssets("t:lilToonSetting"))
            {
                string path = lilDirectoryManager.GUIDToPath(guid);
                var shaderSettingOld = AssetDatabase.LoadAssetAtPath<lilToonSetting>(path);
                shaderSetting = UnityEngine.Object.Instantiate(shaderSettingOld);
                if(shaderSetting != null)
                {
                    Debug.Log("[lilToon] Migrate settings from: " + path);
                    TurnOnAllShaderSetting(ref shaderSetting);
                    ApplyShaderSetting(shaderSetting);
                    AssetDatabase.Refresh();
                    return;
                }
            }
            shaderSetting = ScriptableObject.CreateInstance<lilToonSetting>();
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
        shaderSetting.LIL_FEATURE_RECEIVE_SHADOW = false;
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
        shaderSetting.LIL_FEATURE_ENCRYPTION = lilDirectoryManager.ExistsEncryption();
        shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV = false;
        shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION = false;
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
        shaderSetting.LIL_FEATURE_RECEIVE_SHADOW = true;
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
        shaderSetting.LIL_FEATURE_ENCRYPTION = lilDirectoryManager.ExistsEncryption();
        shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV = true;
        shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION = true;
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
        var folders = new List<string>{shaderFolderPath};
        string baseShaderFolderPath = lilDirectoryManager.GetBaseShaderFolderPath();
        foreach(string shaderGuid in AssetDatabase.FindAssets("", new[] {baseShaderFolderPath}))
        {
            string baseShaderPath = lilDirectoryManager.GUIDToPath(shaderGuid);
            if(!baseShaderPath.Contains(".lilinternal")) continue;
            string shaderPath = shaderFolderPath + Path.AltDirectorySeparatorChar + Path.GetFileNameWithoutExtension(baseShaderPath) + ".shader";
            File.WriteAllText(shaderPath, lilShaderContainer.UnpackContainer(baseShaderPath));
        }
        foreach(string shaderGuid in AssetDatabase.FindAssets("t:shader"))
        {
            string shaderPath = lilDirectoryManager.GUIDToPath(shaderGuid);
            if(!shaderPath.Contains(".lilcontainer")) continue;
            string folder = Path.GetDirectoryName(shaderPath);
            if(!folders.Contains(folder)) folders.Add(folder);
        }
        foreach(string folder in folders)
        {
            AssetDatabase.ImportAsset(folder, ImportAssetOptions.ImportRecursive);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if(!string.IsNullOrEmpty(reportTitle))
        {
            Debug.Log(reportTitle + "\r\n" + shaderSettingString);
        }
    }

    public static string BuildShaderSettingString(lilToonSetting shaderSetting, bool isFile)
    {
        StringBuilder sb = new StringBuilder();
        if(isFile) sb.Append("#ifndef LIL_SETTING_INCLUDED\r\n#define LIL_SETTING_INCLUDED\r\n\r\n");
        if(shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV) sb.Append("#define LIL_FEATURE_ANIMATE_MAIN_UV\r\n");
        if(shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION) sb.Append("#define LIL_FEATURE_MAIN_TONE_CORRECTION\r\n");
        if(shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP) sb.Append("#define LIL_FEATURE_MAIN_GRADATION_MAP\r\n");
        if(shaderSetting.LIL_FEATURE_MAIN2ND) sb.Append("#define LIL_FEATURE_MAIN2ND\r\n");
        if(shaderSetting.LIL_FEATURE_MAIN3RD) sb.Append("#define LIL_FEATURE_MAIN3RD\r\n");
        if(shaderSetting.LIL_FEATURE_MAIN2ND || shaderSetting.LIL_FEATURE_MAIN3RD)
        {
            if(shaderSetting.LIL_FEATURE_DECAL) sb.Append("#define LIL_FEATURE_DECAL\r\n");
            if(shaderSetting.LIL_FEATURE_ANIMATE_DECAL) sb.Append("#define LIL_FEATURE_ANIMATE_DECAL\r\n");
            if(shaderSetting.LIL_FEATURE_LAYER_DISSOLVE) sb.Append("#define LIL_FEATURE_LAYER_DISSOLVE\r\n");
        }

        if(shaderSetting.LIL_FEATURE_ALPHAMASK) sb.Append("#define LIL_FEATURE_ALPHAMASK\r\n");

        if(shaderSetting.LIL_FEATURE_SHADOW)
        {
            sb.Append("#define LIL_FEATURE_SHADOW\r\n");
            if(shaderSetting.LIL_FEATURE_RECEIVE_SHADOW) sb.Append("#define LIL_FEATURE_RECEIVE_SHADOW\r\n");
            if(shaderSetting.LIL_FEATURE_SHADOW_3RD) sb.Append("#define LIL_FEATURE_SHADOW_3RD\r\n");
        }

        if(shaderSetting.LIL_FEATURE_EMISSION_1ST) sb.Append("#define LIL_FEATURE_EMISSION_1ST\r\n");
        if(shaderSetting.LIL_FEATURE_EMISSION_2ND) sb.Append("#define LIL_FEATURE_EMISSION_2ND\r\n");
        if(shaderSetting.LIL_FEATURE_EMISSION_1ST || shaderSetting.LIL_FEATURE_EMISSION_2ND)
        {
            if(shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV) sb.Append("#define LIL_FEATURE_ANIMATE_EMISSION_UV\r\n");
            if(shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV) sb.Append("#define LIL_FEATURE_ANIMATE_EMISSION_MASK_UV\r\n");
            if(shaderSetting.LIL_FEATURE_EMISSION_GRADATION) sb.Append("#define LIL_FEATURE_EMISSION_GRADATION\r\n");
        }
        if(shaderSetting.LIL_FEATURE_NORMAL_1ST) sb.Append("#define LIL_FEATURE_NORMAL_1ST\r\n");
        if(shaderSetting.LIL_FEATURE_NORMAL_2ND) sb.Append("#define LIL_FEATURE_NORMAL_2ND\r\n");
        if(shaderSetting.LIL_FEATURE_ANISOTROPY) sb.Append("#define LIL_FEATURE_ANISOTROPY\r\n");
        if(shaderSetting.LIL_FEATURE_REFLECTION) sb.Append("#define LIL_FEATURE_REFLECTION\r\n");
        if(shaderSetting.LIL_FEATURE_MATCAP) sb.Append("#define LIL_FEATURE_MATCAP\r\n");
        if(shaderSetting.LIL_FEATURE_MATCAP_2ND) sb.Append("#define LIL_FEATURE_MATCAP_2ND\r\n");
        if(shaderSetting.LIL_FEATURE_RIMLIGHT)
        {
            sb.Append("#define LIL_FEATURE_RIMLIGHT\r\n");
            if(shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION) sb.Append("#define LIL_FEATURE_RIMLIGHT_DIRECTION\r\n");
        }
        if(shaderSetting.LIL_FEATURE_GLITTER) sb.Append("#define LIL_FEATURE_GLITTER\r\n");
        if(shaderSetting.LIL_FEATURE_BACKLIGHT) sb.Append("#define LIL_FEATURE_BACKLIGHT\r\n");
        if(shaderSetting.LIL_FEATURE_PARALLAX)
        {
            sb.Append("#define LIL_FEATURE_PARALLAX\r\n");
            if(shaderSetting.LIL_FEATURE_POM) sb.Append("#define LIL_FEATURE_POM\r\n");
        }
        if(shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER) sb.Append("#define LIL_FEATURE_CLIPPING_CANCELLER\r\n");
        if(shaderSetting.LIL_FEATURE_DISTANCE_FADE) sb.Append("#define LIL_FEATURE_DISTANCE_FADE\r\n");
        if(shaderSetting.LIL_FEATURE_AUDIOLINK)
        {
            sb.Append("#define LIL_FEATURE_AUDIOLINK\r\n");
            if(shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX) sb.Append("#define LIL_FEATURE_AUDIOLINK_VERTEX\r\n");
            if(shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL) sb.Append("#define LIL_FEATURE_AUDIOLINK_LOCAL\r\n");
        }
        if(shaderSetting.LIL_FEATURE_DISSOLVE) sb.Append("#define LIL_FEATURE_DISSOLVE\r\n");
        if(shaderSetting.LIL_FEATURE_ENCRYPTION) sb.Append("#define LIL_FEATURE_ENCRYPTION\r\n");
        if(shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION) sb.Append("#define LIL_FEATURE_OUTLINE_TONE_CORRECTION\r\n");
        if(shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV) sb.Append("#define LIL_FEATURE_ANIMATE_OUTLINE_UV\r\n");
        if(shaderSetting.LIL_FEATURE_FUR_COLLISION) sb.Append("#define LIL_FEATURE_FUR_COLLISION\r\n");

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

        if(shaderSetting.LIL_OPTIMIZE_APPLY_SHADOW_FA) sb.Append("#define LIL_OPTIMIZE_APPLY_SHADOW_FA\r\n");
        if(shaderSetting.LIL_OPTIMIZE_USE_FORWARDADD) sb.Append("#define LIL_OPTIMIZE_USE_FORWARDADD\r\n");
        if(shaderSetting.LIL_OPTIMIZE_USE_VERTEXLIGHT) sb.Append("#define LIL_OPTIMIZE_USE_VERTEXLIGHT\r\n");
        if(shaderSetting.LIL_OPTIMIZE_USE_LIGHTMAP) sb.Append("#define LIL_OPTIMIZE_USE_LIGHTMAP\r\n");
        if(isFile) sb.Append("\r\n#endif");

        if(!isFile)
        {
            if(!(shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW) && !shaderSetting.LIL_FEATURE_BACKLIGHT)
            {
                sb.Append("#pragma lil_skip_variants_shadows\r\n");
            }
            if(!shaderSetting.LIL_FEATURE_REFLECTION) sb.Append("#pragma lil_skip_variants_reflections\r\n");
            if(!shaderSetting.LIL_OPTIMIZE_USE_VERTEXLIGHT) sb.Append("#pragma lil_skip_variants_addlight\r\n");
            if(!shaderSetting.LIL_OPTIMIZE_USE_LIGHTMAP) sb.Append("#pragma lil_skip_variants_lightmaps\r\n");
        }
        return sb.ToString();
    }

    public static string BuildShaderSettingString(bool isFile)
    {
        lilToonSetting shaderSetting = null;
        InitializeShaderSetting(ref shaderSetting);
        return BuildShaderSettingString(shaderSetting, isFile);
    }

    internal static void ApplyShaderSettingOptimized()
    {
        lilToonSetting shaderSetting = null;
        InitializeShaderSetting(ref shaderSetting);
        TurnOffAllShaderSetting(ref shaderSetting);

        // Get materials
        foreach(string guid in AssetDatabase.FindAssets("t:material"))
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(lilDirectoryManager.GUIDToPath(guid));
            SetupShaderSettingFromMaterial(material, ref shaderSetting);
        }

        // Get animations
        foreach(string guid in AssetDatabase.FindAssets("t:animationclip"))
        {
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(lilDirectoryManager.GUIDToPath(guid));
            SetupShaderSettingFromAnimationClip(clip, ref shaderSetting);
        }

        // Apply
        ApplyShaderSetting(shaderSetting, "[lilToon] PreprocessBuild");
        AssetDatabase.Refresh();
    }

    internal static void SetShaderSettingBeforeBuild(GameObject gameObject)
    {
        try
        {
            if(!ShouldOptimization()) return;
            File.Create(lilDirectoryManager.postBuildTempPath);

            lilToonSetting shaderSetting = null;
            InitializeShaderSetting(ref shaderSetting);
            TurnOffAllShaderSetting(ref shaderSetting);

            // Get materials
            foreach(var renderer in gameObject.GetComponentsInChildren<Renderer>(true))
            {
                foreach(var material in renderer.sharedMaterials)
                {
                    SetupShaderSettingFromMaterial(material, ref shaderSetting);
                }
            }

            // Get animations
            foreach(var animator in gameObject.GetComponentsInChildren<Animator>(true))
            {
                if(animator.runtimeAnimatorController == null) continue;
                foreach(var clip in animator.runtimeAnimatorController.animationClips)
                {
                    SetupShaderSettingFromAnimationClip(clip, ref shaderSetting, true);
                }
            }
            #if VRC_SDK_VRCSDK3 && !UDON
                foreach(var descriptor in gameObject.GetComponentsInChildren<VRCAvatarDescriptor>(true))
                {
                    foreach(var layer in descriptor.specialAnimationLayers)
                    {
                        if(layer.animatorController == null) continue;
                        foreach(var clip in layer.animatorController.animationClips)
                        {
                            SetupShaderSettingFromAnimationClip(clip, ref shaderSetting, true);
                        }
                    }
                    if(descriptor.customizeAnimationLayers)
                    {
                        foreach(var layer in descriptor.baseAnimationLayers)
                        {
                            if(layer.animatorController == null) continue;
                            foreach(var clip in layer.animatorController.animationClips)
                            {
                                SetupShaderSettingFromAnimationClip(clip, ref shaderSetting, true);
                            }
                        }
                    }
                }
            #endif

            // Apply
            ApplyShaderSetting(shaderSetting, "[lilToon] PreprocessBuild");
            AssetDatabase.Refresh();
        }
        catch
        {
            Debug.Log("[lilToon] Optimization failed");
        }
    }

    internal static void SetShaderSettingBeforeBuild()
    {
        try
        {
            if(!ShouldOptimization()) return;
            File.Create(lilDirectoryManager.postBuildTempPath);
            ApplyShaderSettingOptimized();
        }
        catch
        {
            Debug.Log("[lilToon] Optimization failed");
        }
    }

    internal static void SetShaderSettingAfterBuild()
    {
        if(!File.Exists(lilDirectoryManager.postBuildTempPath)) return;
        File.Delete(lilDirectoryManager.postBuildTempPath);
        if(!ShouldOptimization()) return;
        if(File.Exists(lilDirectoryManager.forceOptimizeBuildTempPath)) File.Delete(lilDirectoryManager.forceOptimizeBuildTempPath);
        lilToonSetting shaderSetting = null;
        InitializeShaderSetting(ref shaderSetting);
        if(shaderSetting.isDebugOptimize)
        {
            ApplyShaderSettingOptimized();
        }
        else
        {
            TurnOnAllShaderSetting(ref shaderSetting);
            ApplyShaderSetting(shaderSetting, "[lilToon] PostprocessBuild");
        }
    }

    internal static void SetupShaderSettingFromMaterial(Material material, ref lilToonSetting shaderSetting)
    {
        if(material == null || material.shader == null) return;
        if(material.shader.name.Contains("Lite") || material.shader.name.Contains("Multi")) return;
        if(!material.shader.name.Contains("lilToon"))
        {
            string shaderPath = AssetDatabase.GetAssetPath(material.shader);
            if(string.IsNullOrEmpty(shaderPath) || shaderPath.Contains("lilcontainer")) return;
        }

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
            foreach(EditorCurveBinding binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
            {
                foreach(ObjectReferenceKeyframe frame in AnimationUtility.GetObjectReferenceCurve(clip, binding))
                {
                    if(frame.value is Material)
                    {
                        SetupShaderSettingFromMaterial((Material)frame.value, ref shaderSetting);
                    }
                }
            }
        }

        foreach(EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
        {
            string propname = binding.propertyName;
            if(string.IsNullOrEmpty(propname) || !propname.Contains("material.")) continue;

            shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV = shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV || propname.Contains("_MainTex_ScrollRotate");
            shaderSetting.LIL_FEATURE_SHADOW = shaderSetting.LIL_FEATURE_SHADOW || propname.Contains("_UseShadow");
            shaderSetting.LIL_FEATURE_RECEIVE_SHADOW = shaderSetting.LIL_FEATURE_RECEIVE_SHADOW || propname.Contains("_ShadowReceive") || propname.Contains("_Shadow2ndReceive") || propname.Contains("_Shadow3rdReceive");
            shaderSetting.LIL_FEATURE_DISTANCE_FADE = shaderSetting.LIL_FEATURE_DISTANCE_FADE || propname.Contains("_DistanceFade");
            shaderSetting.LIL_FEATURE_SHADOW_3RD = shaderSetting.LIL_FEATURE_SHADOW_3RD || propname.Contains("_Shadow3rdColor");

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

            shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV = shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV || propname.Contains("_OutlineTex_ScrollRotate");
            shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION = shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION || propname.Contains("_OutlineTexHSVG");

            shaderSetting.LIL_OPTIMIZE_USE_VERTEXLIGHT = shaderSetting.LIL_OPTIMIZE_USE_VERTEXLIGHT || shaderSetting.LIL_FEATURE_FUR_COLLISION || propname.Contains("_VertexLightStrength");

            // Texture
            CheckTextures(ref shaderSetting, propname);
        }
    }

    internal static void CheckTextures(ref lilToonSetting shaderSetting)
    {
        // Get materials
        foreach(string guid in AssetDatabase.FindAssets("t:material"))
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(lilDirectoryManager.GUIDToPath(guid));
            CheckTextures(ref shaderSetting, material);
        }

        // Get animations
        foreach(string guid in AssetDatabase.FindAssets("t:animationclip"))
        {
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(lilDirectoryManager.GUIDToPath(guid));
            foreach(EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
            {
                string propname = binding.propertyName;
                if(string.IsNullOrEmpty(propname) || !propname.Contains("material.")) continue;

                CheckTextures(ref shaderSetting, propname);
            }
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

    internal static void ForceOptimization()
    {
        if(File.Exists(lilDirectoryManager.forceOptimizeBuildTempPath)) return;
        File.Create(lilDirectoryManager.forceOptimizeBuildTempPath);
    }

    internal static bool ShouldOptimization()
    {
        if(File.Exists(lilDirectoryManager.postBuildTempPath)) return false;
        if(File.Exists(lilDirectoryManager.forceOptimizeBuildTempPath)) return true;

        lilToonSetting shaderSetting = null;
        InitializeShaderSetting(ref shaderSetting);
        #if VRC_SDK_VRCSDK3 && !UDON
            return shaderSetting.isOptimizeInTestBuild && !shaderSetting.isDebugOptimize;
        #else
            return !shaderSetting.isDebugOptimize;
        #endif
    }
}
#endif