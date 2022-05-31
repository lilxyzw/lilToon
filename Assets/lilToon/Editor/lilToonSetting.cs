#if UNITY_EDITOR
using lilToon;
using lilToon.lilRenderPipelineReader;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
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

    public static void InitializeShaderSetting(ref lilToonSetting shaderSetting)
    {
        if(shaderSetting != null) return;
        string shaderSettingPath = lilToonInspector.GetShaderSettingPath();
        shaderSetting = AssetDatabase.LoadAssetAtPath<lilToonSetting>(shaderSettingPath);
        if(shaderSetting == null)
        {
            foreach(string guid in AssetDatabase.FindAssets("t:lilToonSetting"))
            {
                string path = lilToonInspector.GUIDToPath(guid);
                var shaderSettingOld = AssetDatabase.LoadAssetAtPath<lilToonSetting>(path);
                shaderSetting = UnityEngine.Object.Instantiate(shaderSettingOld);
                if(shaderSetting != null)
                {
                    Debug.Log("[lilToon] Migrate settings from: " + path);
                    TurnOnAllShaderSetting(ref shaderSetting);
                    AssetDatabase.CreateAsset(shaderSetting, shaderSettingPath);
                    ApplyShaderSetting(shaderSetting);
                    AssetDatabase.Refresh();
                    return;
                }
            }
            shaderSetting = ScriptableObject.CreateInstance<lilToonSetting>();
            AssetDatabase.CreateAsset(shaderSetting, shaderSettingPath);
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
        shaderSetting.LIL_FEATURE_ENCRYPTION = lilToonInspector.ExistsEncryption();
        shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV = false;
        shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION = false;
        shaderSetting.LIL_FEATURE_FUR_COLLISION = false;
        shaderSetting.LIL_FEATURE_TEX_LAYER_MASK = false;
        shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE = false;
        shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR = false;
        shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER = false;
        shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH = false;
        shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST = false;
        shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND = false;
        shaderSetting.LIL_FEATURE_TEX_SHADOW_3RD = false;
        shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK = false;
        shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK = false;
        shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS = false;
        shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC = false;
        shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR = false;
        shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK = false;
        shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP = false;
        shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR = false;
        shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE = false;
        shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR = false;
        shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH = false;
        shaderSetting.LIL_FEATURE_TEX_OUTLINE_NORMAL = false;
        shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL = false;
        shaderSetting.LIL_FEATURE_TEX_FUR_MASK = false;
        shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH = false;
        shaderSetting.LIL_FEATURE_TEX_TESSELLATION = false;
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
        shaderSetting.LIL_FEATURE_ENCRYPTION = lilToonInspector.ExistsEncryption();
        shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV = true;
        shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION = true;
        shaderSetting.LIL_FEATURE_FUR_COLLISION = true;
        shaderSetting.LIL_FEATURE_TEX_LAYER_MASK = true;
        shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE = true;
        shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR = true;
        shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER = true;
        shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH = true;
        shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST = true;
        shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND = true;
        shaderSetting.LIL_FEATURE_TEX_SHADOW_3RD = true;
        shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK = true;
        shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK = true;
        shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS = true;
        shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC = true;
        shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR = true;
        shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK = true;
        shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP = true;
        shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR = true;
        shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE = true;
        shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR = true;
        shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH = true;
        shaderSetting.LIL_FEATURE_TEX_OUTLINE_NORMAL = true;
        shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL = true;
        shaderSetting.LIL_FEATURE_TEX_FUR_MASK = true;
        shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH = true;
        shaderSetting.LIL_FEATURE_TEX_TESSELLATION = true;
    }

    public static void ApplyShaderSetting(lilToonSetting shaderSetting, string reportTitle = null)
    {
        EditorUtility.SetDirty(shaderSetting);
        AssetDatabase.SaveAssets();
        string shaderSettingString = BuildShaderSettingString(shaderSetting, true);
        string shaderSettingStringBuf = "";
        string shaderSettingHLSLPath = lilToonInspector.GetShaderSettingHLSLPath();
        if(File.Exists(shaderSettingHLSLPath))
        {
            StreamReader sr = new StreamReader(shaderSettingHLSLPath);
            shaderSettingStringBuf = sr.ReadToEnd();
            sr.Close();
        }

        if(shaderSettingString != shaderSettingStringBuf)
        {
            PackageVersionInfos version = RPReader.GetRPInfos();
            StreamWriter sw = new StreamWriter(shaderSettingHLSLPath,false);
            sw.Write(shaderSettingString);
            sw.Close();
            string[] shaderFolderPaths = lilToonInspector.GetShaderFolderPaths();
            bool isShadowReceive = (shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW) || shaderSetting.LIL_FEATURE_BACKLIGHT;
            var folders = new List<string>
            {
                lilToonInspector.GetShaderFolderPath()
            };
            foreach (string shaderGuid in AssetDatabase.FindAssets("t:shader", shaderFolderPaths))
            {
                string shaderPath = lilToonInspector.GUIDToPath(shaderGuid);
                lilShaderRewriter.RewriteReceiveShadow(shaderPath, isShadowReceive);
                lilShaderRewriter.RewriteForwardAdd(shaderPath, shaderSetting.LIL_OPTIMIZE_USE_FORWARDADD);
                lilShaderRewriter.RewriteVertexLight(shaderPath, shaderSetting.LIL_OPTIMIZE_USE_VERTEXLIGHT);
                lilShaderRewriter.RewriteLightmap(shaderPath, shaderSetting.LIL_OPTIMIZE_USE_LIGHTMAP);
                lilShaderRewriter.RewriteRPPass(shaderPath, version);
            }
            foreach(string shaderGuid in AssetDatabase.FindAssets("t:shader"))
            {
                string shaderPath = lilToonInspector.GUIDToPath(shaderGuid);
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
        }

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
            if(shaderSetting.LIL_FEATURE_TEX_LAYER_MASK) sb.Append("#define LIL_FEATURE_TEX_LAYER_MASK\r\n");
            if(shaderSetting.LIL_FEATURE_LAYER_DISSOLVE)
            {
                sb.Append("#define LIL_FEATURE_LAYER_DISSOLVE\r\n");
                if(shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE) sb.Append("#define LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE\r\n");
            }
        }

        if(shaderSetting.LIL_FEATURE_ALPHAMASK) sb.Append("#define LIL_FEATURE_ALPHAMASK\r\n");

        if(shaderSetting.LIL_FEATURE_SHADOW)
        {
            sb.Append("#define LIL_FEATURE_SHADOW\r\n");
            if(shaderSetting.LIL_FEATURE_RECEIVE_SHADOW) sb.Append("#define LIL_FEATURE_RECEIVE_SHADOW\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR) sb.Append("#define LIL_FEATURE_TEX_SHADOW_BLUR\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER) sb.Append("#define LIL_FEATURE_TEX_SHADOW_BORDER\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH) sb.Append("#define LIL_FEATURE_TEX_SHADOW_STRENGTH\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST) sb.Append("#define LIL_FEATURE_TEX_SHADOW_1ST\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND) sb.Append("#define LIL_FEATURE_TEX_SHADOW_2ND\r\n");
            if(shaderSetting.LIL_FEATURE_SHADOW_3RD)
            {
                sb.Append("#define LIL_FEATURE_SHADOW_3RD\r\n");
                if(shaderSetting.LIL_FEATURE_TEX_SHADOW_3RD) sb.Append("#define LIL_FEATURE_TEX_SHADOW_3RD\r\n");
            }
        }

        if(shaderSetting.LIL_FEATURE_EMISSION_1ST) sb.Append("#define LIL_FEATURE_EMISSION_1ST\r\n");
        if(shaderSetting.LIL_FEATURE_EMISSION_2ND) sb.Append("#define LIL_FEATURE_EMISSION_2ND\r\n");
        if(shaderSetting.LIL_FEATURE_EMISSION_1ST || shaderSetting.LIL_FEATURE_EMISSION_2ND)
        {
            if(shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV) sb.Append("#define LIL_FEATURE_ANIMATE_EMISSION_UV\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK)
            {
                sb.Append("#define LIL_FEATURE_TEX_EMISSION_MASK\r\n");
                if(shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV) sb.Append("#define LIL_FEATURE_ANIMATE_EMISSION_MASK_UV\r\n");
            }
            if(shaderSetting.LIL_FEATURE_EMISSION_GRADATION) sb.Append("#define LIL_FEATURE_EMISSION_GRADATION\r\n");
        }
        if(shaderSetting.LIL_FEATURE_NORMAL_1ST) sb.Append("#define LIL_FEATURE_NORMAL_1ST\r\n");
        if(shaderSetting.LIL_FEATURE_NORMAL_2ND)
        {
            sb.Append("#define LIL_FEATURE_NORMAL_2ND\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK) sb.Append("#define LIL_FEATURE_TEX_NORMAL_MASK\r\n");
        }
        if(shaderSetting.LIL_FEATURE_ANISOTROPY) sb.Append("#define LIL_FEATURE_ANISOTROPY\r\n");
        if(shaderSetting.LIL_FEATURE_REFLECTION)
        {
            sb.Append("#define LIL_FEATURE_REFLECTION\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS) sb.Append("#define LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC) sb.Append("#define LIL_FEATURE_TEX_REFLECTION_METALLIC\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR) sb.Append("#define LIL_FEATURE_TEX_REFLECTION_COLOR\r\n");
        }
        if(shaderSetting.LIL_FEATURE_MATCAP) sb.Append("#define LIL_FEATURE_MATCAP\r\n");
        if(shaderSetting.LIL_FEATURE_MATCAP_2ND) sb.Append("#define LIL_FEATURE_MATCAP_2ND\r\n");
        if(shaderSetting.LIL_FEATURE_MATCAP || shaderSetting.LIL_FEATURE_MATCAP_2ND)
        {
            if(shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK) sb.Append("#define LIL_FEATURE_TEX_MATCAP_MASK\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP) sb.Append("#define LIL_FEATURE_TEX_MATCAP_NORMALMAP\r\n");
        }
        if(shaderSetting.LIL_FEATURE_RIMLIGHT)
        {
            sb.Append("#define LIL_FEATURE_RIMLIGHT\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR) sb.Append("#define LIL_FEATURE_TEX_RIMLIGHT_COLOR\r\n");
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
        if(shaderSetting.LIL_FEATURE_DISSOLVE)
        {
            sb.Append("#define LIL_FEATURE_DISSOLVE\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE) sb.Append("#define LIL_FEATURE_TEX_DISSOLVE_NOISE\r\n");
        }
        if(shaderSetting.LIL_FEATURE_ENCRYPTION) sb.Append("#define LIL_FEATURE_ENCRYPTION\r\n");
        if(shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR)
        {
            sb.Append("#define LIL_FEATURE_TEX_OUTLINE_COLOR\r\n");
            if(shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION) sb.Append("#define LIL_FEATURE_OUTLINE_TONE_CORRECTION\r\n");
        }
        if(shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV) sb.Append("#define LIL_FEATURE_ANIMATE_OUTLINE_UV\r\n");
        if(shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH) sb.Append("#define LIL_FEATURE_TEX_OUTLINE_WIDTH\r\n");
        if(shaderSetting.LIL_FEATURE_TEX_OUTLINE_NORMAL) sb.Append("#define LIL_FEATURE_TEX_OUTLINE_NORMAL\r\n");
        if(shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL) sb.Append("#define LIL_FEATURE_TEX_FUR_NORMAL\r\n");
        if(shaderSetting.LIL_FEATURE_TEX_FUR_MASK) sb.Append("#define LIL_FEATURE_TEX_FUR_MASK\r\n");
        if(shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH) sb.Append("#define LIL_FEATURE_TEX_FUR_LENGTH\r\n");
        if(shaderSetting.LIL_FEATURE_FUR_COLLISION) sb.Append("#define LIL_FEATURE_FUR_COLLISION\r\n");
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
        if(shaderSetting == null) return "";
        return BuildShaderSettingString(shaderSetting, isFile);
    }

    public static void ApplyShaderSettingOptimized()
    {
        lilToonSetting shaderSetting = null;
        InitializeShaderSetting(ref shaderSetting);
        if(shaderSetting == null) return;

        TurnOffAllShaderSetting(ref shaderSetting);

        // Get materials
        foreach(string guid in AssetDatabase.FindAssets("t:material"))
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(lilToonInspector.GUIDToPath(guid));
            SetupShaderSettingFromMaterial(material, ref shaderSetting);
        }

        // Get animations
        foreach(string guid in AssetDatabase.FindAssets("t:animationclip"))
        {
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(lilToonInspector.GUIDToPath(guid));
            SetupShaderSettingFromAnimationClip(clip, ref shaderSetting);
        }

        // Apply
        ApplyShaderSetting(shaderSetting, "[lilToon] PreprocessBuild");
        AssetDatabase.Refresh();
    }

    public static void SetShaderSettingBeforeBuild(GameObject gameObject)
    {
        if(File.Exists(lilToonInspector.postBuildTempPath)) return;
        File.Create(lilToonInspector.postBuildTempPath);

        lilToonSetting shaderSetting = null;
        InitializeShaderSetting(ref shaderSetting);
        if(shaderSetting == null) return;

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

    public static void SetShaderSettingBeforeBuild()
    {
        if(File.Exists(lilToonInspector.postBuildTempPath)) return;
        File.Create(lilToonInspector.postBuildTempPath);
        ApplyShaderSettingOptimized();
    }

    public static void SetShaderSettingAfterBuild()
    {
        if(!File.Exists(lilToonInspector.postBuildTempPath)) return;
        File.Delete(lilToonInspector.postBuildTempPath);
        lilToonSetting shaderSetting = null;
        InitializeShaderSetting(ref shaderSetting);
        if(shaderSetting == null) return;
        TurnOnAllShaderSetting(ref shaderSetting);
        ApplyShaderSetting(shaderSetting, "[lilToon] PostprocessBuild");
    }

    private static void SetupShaderSettingFromMaterial(Material material, ref lilToonSetting shaderSetting)
    {
        if(material == null) return;
        if(!material.shader.name.Contains("lilToon") || material.shader.name.Contains("Lite") || material.shader.name.Contains("Multi")) return;

        if(!shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV && material.HasProperty("_MainTex_ScrollRotate") && material.GetVector("_MainTex_ScrollRotate") != lilToonInspector.defaultScrollRotate)
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
        if(!shaderSetting.LIL_FEATURE_DISTANCE_FADE && material.HasProperty("_DistanceFade") && material.GetVector("_DistanceFade").z != lilToonInspector.defaultDistanceFadeParams.z)
        {
            Debug.Log("[lilToon] LIL_FEATURE_DISTANCE_FADE : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_DISTANCE_FADE = true;
        }
        if(!shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR && material.HasProperty("_ShadowBlurMask") && material.GetTexture("_ShadowBlurMask") != null)
        {
            Debug.Log("[lilToon] LIL_FEATURE_TEX_SHADOW_BLUR : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR = true;
        }
        if(!shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER && material.HasProperty("_ShadowBorderMask") && material.GetTexture("_ShadowBorderMask") != null)
        {
            Debug.Log("[lilToon] LIL_FEATURE_TEX_SHADOW_BORDER : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER = true;
        }
        if(!shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH && material.HasProperty("_ShadowStrengthMask") && material.GetTexture("_ShadowStrengthMask") != null)
        {
            Debug.Log("[lilToon] LIL_FEATURE_TEX_SHADOW_STRENGTH : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH = true;
        }
        if(!shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST && material.HasProperty("_ShadowColorTex") && material.GetTexture("_ShadowColorTex") != null)
        {
            Debug.Log("[lilToon] LIL_FEATURE_TEX_SHADOW_1ST : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST = true;
        }
        if(!shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND && material.HasProperty("_Shadow2ndColorTex") && material.GetTexture("_Shadow2ndColorTex") != null)
        {
            Debug.Log("[lilToon] LIL_FEATURE_TEX_SHADOW_2ND : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND = true;
        }
        if(!shaderSetting.LIL_FEATURE_SHADOW_3RD && material.HasProperty("_Shadow3rdColor") && material.GetColor("_Shadow3rdColor").a != 0.0f)
        {
            Debug.Log("[lilToon] LIL_FEATURE_SHADOW_3RD : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_SHADOW_3RD = true;
        }
        if(!shaderSetting.LIL_FEATURE_TEX_SHADOW_3RD && material.HasProperty("_Shadow3rdColorTex") && material.GetTexture("_Shadow3rdColorTex") != null)
        {
            Debug.Log("[lilToon] LIL_FEATURE_SHADOW_3RD : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_TEX_SHADOW_3RD = true;
        }

        if(material.shader.name.Contains("Fur"))
        {
            if(!shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL && material.HasProperty("_FurVectorTex") && material.GetTexture("_FurVectorTex") != null)
            {
                Debug.Log("[lilToon] LIL_FEATURE_TEX_FUR_NORMAL : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL = true;
            }
            if(!shaderSetting.LIL_FEATURE_TEX_FUR_MASK && material.HasProperty("_FurMask") && material.GetTexture("_FurMask") != null)
            {
                Debug.Log("[lilToon] LIL_FEATURE_TEX_FUR_MASK : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_FUR_MASK = true;
            }
            if(!shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH && material.HasProperty("_FurLengthMask") && material.GetTexture("_FurLengthMask") != null)
            {
                Debug.Log("[lilToon] LIL_FEATURE_TEX_FUR_LENGTH : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH = true;
            }
            if(!shaderSetting.LIL_FEATURE_FUR_COLLISION && material.HasProperty("_FurTouchStrength") && material.GetFloat("_FurTouchStrength") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_FUR_COLLISION : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_FUR_COLLISION = true;
            }
        }

        if(!shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION && material.HasProperty("_MainTexHSVG") && material.GetVector("_MainTexHSVG") != lilToonInspector.defaultHSVG)
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
            (material.HasProperty("_Main2ndTexDecalAnimation") && material.GetVector("_Main2ndTexDecalAnimation") != lilToonInspector.defaultDecalAnim) ||
            (material.HasProperty("_Main3rdTexDecalAnimation") && material.GetVector("_Main3rdTexDecalAnimation") != lilToonInspector.defaultDecalAnim))
        )
        {
            Debug.Log("[lilToon] LIL_FEATURE_ANIMATE_DECAL : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_ANIMATE_DECAL = true;
        }
        if(!shaderSetting.LIL_FEATURE_LAYER_DISSOLVE && (
            (material.HasProperty("_Main2ndDissolveParams") && material.GetVector("_Main2ndDissolveParams").x != lilToonInspector.defaultDissolveParams.x) ||
            (material.HasProperty("_Main3rdDissolveParams") && material.GetVector("_Main3rdDissolveParams").x != lilToonInspector.defaultDissolveParams.x))
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
            (material.HasProperty("_EmissionMap_ScrollRotate") && material.GetVector("_EmissionMap_ScrollRotate") != lilToonInspector.defaultScrollRotate) ||
            (material.HasProperty("_Emission2ndMap_ScrollRotate") && material.GetVector("_Emission2ndMap_ScrollRotate") != lilToonInspector.defaultScrollRotate))
        )
        {
            Debug.Log("[lilToon] LIL_FEATURE_ANIMATE_EMISSION_UV : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV = true;
        }
        if(!shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV && (
            (material.HasProperty("_EmissionBlendMask_ScrollRotate") && material.GetVector("_EmissionBlendMask_ScrollRotate") != lilToonInspector.defaultScrollRotate) ||
            (material.HasProperty("_Emission2ndBlendMask_ScrollRotate") && material.GetVector("_Emission2ndBlendMask_ScrollRotate") != lilToonInspector.defaultScrollRotate))
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
        if(!shaderSetting.LIL_FEATURE_DISSOLVE && material.HasProperty("_DissolveParams") && material.GetVector("_DissolveParams").x != lilToonInspector.defaultDissolveParams.x)
        {
            Debug.Log("[lilToon] LIL_FEATURE_DISSOLVE : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_DISSOLVE = true;
        }
        if(!shaderSetting.LIL_FEATURE_TEX_LAYER_MASK && (
            (material.HasProperty("_Main2ndBlendMask") && material.GetTexture("_Main2ndBlendMask") != null) ||
            (material.HasProperty("_Main3rdBlendMask") && material.GetTexture("_Main3rdBlendMask") != null))
        )
        {
            Debug.Log("[lilToon] LIL_FEATURE_TEX_LAYER_MASK : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_TEX_LAYER_MASK = true;
        }
        if(!shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE && (
            (material.HasProperty("_Main2ndDissolveNoiseMask") && material.GetTexture("_Main2ndDissolveNoiseMask") != null) ||
            (material.HasProperty("_Main3rdDissolveNoiseMask") && material.GetTexture("_Main3rdDissolveNoiseMask") != null))
        )
        {
            Debug.Log("[lilToon] LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE = true;
        }
        if(!shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK && (
            (material.HasProperty("_EmissionBlendMask") && material.GetTexture("_EmissionBlendMask") != null) ||
            (material.HasProperty("_Emission2ndBlendMask") && material.GetTexture("_Emission2ndBlendMask") != null))
        )
        {
            Debug.Log("[lilToon] LIL_FEATURE_TEX_EMISSION_MASK : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK = true;
        }
        if(!shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK && material.HasProperty("_Bump2ndScaleMask") && material.GetTexture("_Bump2ndScaleMask") != null)
        {
            Debug.Log("[lilToon] LIL_FEATURE_TEX_NORMAL_MASK : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK = true;
        }
        if(!shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS && material.HasProperty("_SmoothnessTex") && material.GetTexture("_SmoothnessTex") != null)
        {
            Debug.Log("[lilToon] LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS = true;
        }
        if(!shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC && material.HasProperty("_MetallicGlossMap") && material.GetTexture("_MetallicGlossMap") != null)
        {
            Debug.Log("[lilToon] LIL_FEATURE_TEX_REFLECTION_METALLIC : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC = true;
        }
        if(!shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR && material.HasProperty("_ReflectionColorTex") && material.GetTexture("_ReflectionColorTex") != null)
        {
            Debug.Log("[lilToon] LIL_FEATURE_TEX_REFLECTION_COLOR : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR = true;
        }
        if(!shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK && (
            (material.HasProperty("_MatCapBlendMask") && material.GetTexture("_MatCapBlendMask") != null) ||
            (material.HasProperty("_MatCap2ndBlendMask") && material.GetTexture("_MatCap2ndBlendMask") != null))
        )
        {
            Debug.Log("[lilToon] LIL_FEATURE_TEX_MATCAP_MASK : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK = true;
        }
        if(!shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP && (
            (material.HasProperty("_MatCapBumpMap") && material.GetTexture("_MatCapBumpMap") != null) ||
            (material.HasProperty("_MatCap2ndBumpMap") && material.GetTexture("_MatCap2ndBumpMap") != null))
        )
        {
            Debug.Log("[lilToon] LIL_FEATURE_TEX_MATCAP_NORMALMAP : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP = true;
        }
        if(!shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR && material.HasProperty("_RimColorTex") && material.GetTexture("_RimColorTex") != null)
        {
            Debug.Log("[lilToon] LIL_FEATURE_TEX_RIMLIGHT_COLOR : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR = true;
        }
        if(!shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE && material.HasProperty("_DissolveNoiseMask") && material.GetTexture("_DissolveNoiseMask") != null)
        {
            Debug.Log("[lilToon] LIL_FEATURE_TEX_DISSOLVE_NOISE : " + AssetDatabase.GetAssetPath(material));
            shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE = true;
        }

        // Outline
        if(material.shader.name.Contains("Outline"))
        {
            if(!shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV && material.HasProperty("_OutlineTex_ScrollRotate") && material.GetVector("_OutlineTex_ScrollRotate") != lilToonInspector.defaultScrollRotate)
            {
                Debug.Log("[lilToon] LIL_FEATURE_ANIMATE_OUTLINE_UV : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV = true;
            }
            if(!shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION && material.HasProperty("_OutlineTexHSVG") && material.GetVector("_OutlineTexHSVG") != lilToonInspector.defaultHSVG)
            {
                Debug.Log("[lilToon] LIL_FEATURE_OUTLINE_TONE_CORRECTION : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION = true;
            }
            if(!shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR && material.HasProperty("_OutlineTex") && material.GetTexture("_OutlineTex") != null)
            {
                Debug.Log("[lilToon] LIL_FEATURE_TEX_OUTLINE_COLOR : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR = true;
            }
            if(!shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH && material.HasProperty("_OutlineWidthMask") && material.GetTexture("_OutlineWidthMask") != null)
            {
                Debug.Log("[lilToon] LIL_FEATURE_TEX_OUTLINE_WIDTH : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH = true;
            }
            if(!shaderSetting.LIL_FEATURE_TEX_OUTLINE_NORMAL && material.HasProperty("_OutlineVectorTex") && material.GetTexture("_OutlineVectorTex") != null)
            {
                Debug.Log("[lilToon] LIL_FEATURE_TEX_OUTLINE_NORMAL : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_OUTLINE_NORMAL = true;
            }
        }
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
            shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR = shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR || propname.Contains("_ShadowBlurMask");
            shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER = shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER || propname.Contains("_ShadowBorderMask");
            shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH = shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH || propname.Contains("_ShadowStrengthMask");
            shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST = shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST || propname.Contains("_ShadowColorTex");
            shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND = shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND || propname.Contains("_Shadow2ndColorTex");
            shaderSetting.LIL_FEATURE_SHADOW_3RD = shaderSetting.LIL_FEATURE_SHADOW_3RD || propname.Contains("_Shadow3rdColor");
            shaderSetting.LIL_FEATURE_TEX_SHADOW_3RD = shaderSetting.LIL_FEATURE_TEX_SHADOW_3RD || propname.Contains("_Shadow3rdColorTex");

            shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL = shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL || propname.Contains("_FurVectorTex");
            shaderSetting.LIL_FEATURE_TEX_FUR_MASK = shaderSetting.LIL_FEATURE_TEX_FUR_MASK || propname.Contains("_FurMask");
            shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH = shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH || propname.Contains("_FurLengthMask");
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
            shaderSetting.LIL_FEATURE_TEX_LAYER_MASK = shaderSetting.LIL_FEATURE_TEX_LAYER_MASK || propname.Contains("_Main2ndBlendMask") || propname.Contains("_Main3rdBlendMask");
            shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE = shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE || propname.Contains("_Main2ndDissolveNoiseMask") || propname.Contains("_Main3rdDissolveNoiseMask");
            shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK = shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK || propname.Contains("_EmissionBlendMask") || propname.Contains("_Emission2ndBlendMask");
            shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK = shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK || propname.Contains("_Bump2ndScaleMask");
            shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS = shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS || propname.Contains("_SmoothnessTex");
            shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC = shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC || propname.Contains("_MetallicGlossMap");
            shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR = shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR || propname.Contains("_ReflectionColorTex");
            shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK = shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK || propname.Contains("_MatCapBlendMask") || propname.Contains("_MatCap2ndBlendMask");
            shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP = shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP || propname.Contains("_MatCapBumpMap") || propname.Contains("_MatCap2ndBumpMap");
            shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR = shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR || propname.Contains("_RimColorTex");
            shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE = shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE || propname.Contains("_DissolveNoiseMask");

            shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV = shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV || propname.Contains("_OutlineTex_ScrollRotate");
            shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION = shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION || propname.Contains("_OutlineTexHSVG");
            shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR = shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR || propname.Contains("_OutlineTex");
            shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH = shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH || propname.Contains("_OutlineWidthMask");
            shaderSetting.LIL_FEATURE_TEX_OUTLINE_NORMAL = shaderSetting.LIL_FEATURE_TEX_OUTLINE_NORMAL || propname.Contains("_OutlineVectorTex");
        }
    }
}
#endif