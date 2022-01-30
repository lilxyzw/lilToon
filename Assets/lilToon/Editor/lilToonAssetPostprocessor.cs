#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace lilToon
{
    public class lilToonAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            // Runs only when there is no user action
            if(Event.current != null) return;

            string shaderSettingHLSLPath = lilToonInspector.GetShaderSettingHLSLPath();
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
                    Material material = AssetDatabase.LoadAssetAtPath<Material>(str);
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

            // Write temp file
            StreamWriter sw = new StreamWriter(lilToonInspector.packageListTempPath, true);
            Array.ForEach(importedAssets, str => sw.WriteLine(str));
            sw.Close();
        }
    }
}
#endif