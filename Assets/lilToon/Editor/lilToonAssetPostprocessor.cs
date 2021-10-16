#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace lilToon
{
    class lilToonAssetPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            string shaderSettingHLSLPath = lilToonInspector.GetShaderSettingHLSLPath();

            // Runs only when there is no user action
            if(Event.current != null) return;

            bool existsTarget = false;
            foreach(string str in importedAssets)
            {
                if(str.EndsWith(".anim") && AssetDatabase.GetMainAssetTypeAtPath(str) == typeof(AnimationClip))
                {
                    existsTarget = true;
                    break;
                }
                if(str.EndsWith(".mat") && AssetDatabase.GetMainAssetTypeAtPath(str) == typeof(Material))
                {
                    Material material = (Material)AssetDatabase.LoadAssetAtPath(str, typeof(Material));
                    if(material.shader.name.Contains("lilToon") && !material.shader.name.Contains("Lite") && !material.shader.name.Contains("Multi"))
                    {
                        existsTarget = true;
                        break;
                    }
                }
            }
            if(!existsTarget) return;

            // Check shader setting
            string shaderSettingString = "//INITIALIZE";
            if(File.Exists(shaderSettingHLSLPath))
            {
                StreamReader srSetting = new StreamReader(shaderSettingHLSLPath);
                shaderSettingString = srSetting.ReadToEnd();
                srSetting.Close();
            }
            if(shaderSettingString.Contains("//INITIALIZE")) return;

            lilToonSetting shaderSetting = null;
            lilToonInspector.InitializeShaderSetting(ref shaderSetting);
            if(shaderSetting == null || shaderSetting.isLocked || shaderSetting.shouldNotScan) return;

            // Write temp file
            StreamWriter sw = new StreamWriter(lilToonInspector.packageListTempPath, true);
            foreach(string str in importedAssets)
            {
                sw.Write(str + "\n");
            }
            sw.Close();
        }
    }
}
#endif