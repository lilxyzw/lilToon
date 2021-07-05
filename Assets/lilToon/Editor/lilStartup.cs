#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

[InitializeOnLoad]
public class lilStartup {
    //------------------------------------------------------------------------------------------------------------------------------
    // Enum
    enum lilRenderPipeline
    {
        BRP,
        LWRP,
        URP
    }

    static lilStartup()
    {
        string startupTempPath = "Temp/lilStartup";
        string[] shaderFolderPaths = new[] {"Assets/lilToon/Shader"};
        string shaderPipelinePath = "Assets/lilToon/Shader/Includes/lil_pipeline.hlsl";
        string settingFolderPath = Path.GetFullPath("Assets/lilToonSetting");
        string shaderSettingPath = Path.GetFullPath("Assets/lilToonSetting/lil_setting.hlsl");
        string editorPath = "Assets/lilToon/Editor/lilInspector.cs";
        #if NET_4_6
            string rspPath = Path.GetFullPath("Assets/csc.rsp");
        #else
            string rspPath = Path.GetFullPath("Assets/mcs.rsp");
        #endif

        // TODO: sometimes this does not work
        AssetDatabase.importPackageCompleted += OnImportPackageCompleted =>
        {
            if(!File.Exists(startupTempPath))
            {
                string[] shaderGuids = AssetDatabase.FindAssets("t:shader", shaderFolderPaths);
                if(shaderGuids.Length > 33)
                {
                    // Make marker
                    File.Create(startupTempPath);

                    // Render Pipeline
                    // BRP : null
                    // LWRP : LightweightPipeline.LightweightRenderPipelineAsset
                    // URP : Universal.UniversalRenderPipelineAsset
                    lilRenderPipeline lilRP = lilRenderPipeline.BRP;
                    if(UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null)
                    {
                        string renderPipelineName = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset.ToString();
                        if(String.IsNullOrEmpty(renderPipelineName))        lilRP = lilRenderPipeline.BRP;
                        else if(renderPipelineName.Contains("Lightweight")) lilRP = lilRenderPipeline.LWRP;
                        else if(renderPipelineName.Contains("Universal"))   lilRP = lilRenderPipeline.URP;
                    }
                    else
                    {
                        lilRP = lilRenderPipeline.BRP;
                    }
                    foreach(string shaderGuid in shaderGuids)
                    {
                        string shaderPath = AssetDatabase.GUIDToAssetPath(shaderGuid);
                        RewriteShaderRP(shaderPath, lilRP);
                    }
                    RewriteShaderRP(shaderPipelinePath, lilRP);
                    AssetDatabase.Refresh();
                }
            }
        };

        // Setting Folder
        if(!Directory.Exists(settingFolderPath)) Directory.CreateDirectory(settingFolderPath);

        // Shader Setting
        if(!File.Exists(shaderSettingPath))
        {
            StreamWriter sw = new StreamWriter(shaderSettingPath,true);
            sw.Write("#ifndef LIL_SETTING_INCLUDED\r\n#define LIL_SETTING_INCLUDED\r\n\r\n#define LIL_FEATURE_MAIN_TONE_CORRECTION\r\n#define LIL_FEATURE_SHADOW\r\n#define LIL_FEATURE_TEX_SHADOW_STRENGTH\r\n#define LIL_FEATURE_EMISSION_1ST\r\n#define LIL_FEATURE_NORMAL_1ST\r\n#define LIL_FEATURE_MATCAP\r\n#define LIL_FEATURE_TEX_MATCAP_MASK\r\n#define LIL_FEATURE_RIMLIGHT\r\n#define LIL_FEATURE_TEX_RIMLIGHT_COLOR\r\n#define LIL_FEATURE_TEX_OUTLINE_COLOR\r\n#define LIL_FEATURE_TEX_OUTLINE_WIDTH\r\n\r\n#endif");
            sw.Close();
            AssetDatabase.Refresh();
        }

        // Editor
        if(!File.Exists(rspPath))
        {
            StreamWriter sw = new StreamWriter(rspPath,true);
            sw.Write("-r:System.Drawing.dll\n-define:SYSTEM_DRAWING");
            sw.Close();
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(editorPath);
        }

        StreamReader sr = new StreamReader(rspPath);
        string s = sr.ReadToEnd();
        sr.Close();

        if(!s.Contains("r:System.Drawing.dll"))
        {
            StreamWriter sw = new StreamWriter(rspPath,true);
            sw.Write("\n-r:System.Drawing.dll\n-define:SYSTEM_DRAWING");
            sw.Close();
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(editorPath);
        }
        else if(!s.Contains("define:SYSTEM_DRAWING"))
        {
            StreamWriter sw = new StreamWriter(rspPath,true);
            sw.Write("\n-define:SYSTEM_DRAWING");
            sw.Close();
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(editorPath);
        }
    }

    static void RewriteShaderRP(string shaderPath, lilRenderPipeline lilRP)
    {
        string path = Path.GetFullPath(shaderPath);
        StreamReader sr = new StreamReader(path);
        string s = sr.ReadToEnd();
        sr.Close();
        RewriteBRP(ref s, lilRP == lilRenderPipeline.BRP);
        RewriteLWRP(ref s, lilRP == lilRenderPipeline.LWRP);
        RewriteURP(ref s, lilRP == lilRenderPipeline.URP);
        StreamWriter sw = new StreamWriter(path,false);
        sw.Write(s);
        sw.Close();
    }

    static void RewriteBRP(ref string s, bool isActive)
    {
        if(isActive)
        {
            s = s.Replace(
                "// BRP Start\r\n/*",
                "// BRP Start\r\n//");
            s = s.Replace(
                "*/\r\n// BRP End",
                "//\r\n// BRP End");
        }
        else
        {
            s = s.Replace(
                "// BRP Start\r\n//",
                "// BRP Start\r\n/*");
            s = s.Replace(
                "//\r\n// BRP End",
                "*/\r\n// BRP End");
        }
    }

    static void RewriteLWRP(ref string s, bool isActive)
    {
        if(isActive)
        {
            s = s.Replace(
                "// LWRP Start\r\n/*",
                "// LWRP Start\r\n//");
            s = s.Replace(
                "*/\r\n// LWRP End",
                "//\r\n// LWRP End");
        }
        else
        {
            s = s.Replace(
                "// LWRP Start\r\n//",
                "// LWRP Start\r\n/*");
            s = s.Replace(
                "//\r\n// LWRP End",
                "*/\r\n// LWRP End");
        }
    }

    static void RewriteURP(ref string s, bool isActive)
    {
        if(isActive)
        {
            s = s.Replace(
                "// URP Start\r\n/*",
                "// URP Start\r\n//");
            s = s.Replace(
                "*/\r\n// URP End",
                "//\r\n// URP End");
        }
        else
        {
            s = s.Replace(
                "// URP Start\r\n//",
                "// URP Start\r\n/*");
            s = s.Replace(
                "//\r\n// URP End",
                "*/\r\n// URP End");
        }
    }
}
#endif