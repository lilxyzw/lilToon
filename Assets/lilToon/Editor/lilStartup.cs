#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class lilStartup {
    static lilStartup()
    {
        // Shader Setting
        string shaderSettingPath = Path.GetFullPath("Assets/lilToon/Shader/Includes/lil_setting.hlsl");
        if(!File.Exists(shaderSettingPath))
        {
            StreamWriter sw = new StreamWriter(shaderSettingPath,true);
            sw.Write("#ifndef LIL_SETTING_INCLUDED\r\n#define LIL_SETTING_INCLUDED\r\n\r\n#define LIL_FEATURE_MAIN_TONE_CORRECTION\r\n#define LIL_FEATURE_SHADOW\r\n#define LIL_FEATURE_TEX_SHADOW_STRENGTH\r\n#define LIL_FEATURE_EMISSION_1ST\r\n#define LIL_FEATURE_NORMAL_1ST\r\n#define LIL_FEATURE_MATCAP\r\n#define LIL_FEATURE_TEX_MATCAP_MASK\r\n#define LIL_FEATURE_RIMLIGHT\r\n#define LIL_FEATURE_TEX_RIMLIGHT_COLOR\r\n#define LIL_FEATURE_TEX_OUTLINE_COLOR\r\n#define LIL_FEATURE_TEX_OUTLINE_WIDTH\r\n\r\n#endif");
            sw.Close();
            AssetDatabase.Refresh();
        }

        // Editor
        #if NET_4_6
            string fullPath = Path.GetFullPath("Assets/csc.rsp");
        #else
            string fullPath = Path.GetFullPath("Assets/mcs.rsp");
        #endif
        string edotorPath = "Assets/lilToon/Editor/lilInspector.cs";
        if(!File.Exists(fullPath))
        {
            StreamWriter sw = new StreamWriter(fullPath,true);
            sw.Write("-r:System.Drawing.dll\n-define:SYSTEM_DRAWING");
            sw.Close();
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(edotorPath);
        }

        StreamReader sr = new StreamReader(fullPath);
        string s = sr.ReadToEnd();
        sr.Close();

        if(!s.Contains("r:System.Drawing.dll"))
        {
            StreamWriter sw = new StreamWriter(fullPath,true);
            sw.Write("\n-r:System.Drawing.dll\n-define:SYSTEM_DRAWING");
            sw.Close();
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(edotorPath);
        }
        else if(!s.Contains("define:SYSTEM_DRAWING"))
        {
            StreamWriter sw = new StreamWriter(fullPath,true);
            sw.Write("\n-define:SYSTEM_DRAWING");
            sw.Close();
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(edotorPath);
        }
    }
}
#endif