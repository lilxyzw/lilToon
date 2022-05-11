#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Collections;

namespace lilToon
{
    public static class lilStartup
    {
        [InitializeOnLoadMethod]
        public static void lilStartupMethod()
        {
            //------------------------------------------------------------------------------------------------------------------------------
            // Variables
            string editorPath = lilToonInspector.GetEditorPath();
            lilToonInspector.isUPM = editorPath.Contains("Packages");
            string settingFolderPath = lilToonInspector.GetSettingFolderPath();
            string shaderSettingHLSLPath = lilToonInspector.GetShaderSettingHLSLPath();
            string shaderCommonPath = lilToonInspector.GetShaderCommonPath();

            lilToonInspector.ApplyEditorSettingTemp();
            lilToonInspector.InitializeLanguage();

            //------------------------------------------------------------------------------------------------------------------------------
            // Fix for UPM
            string[] shaderFolderPaths = lilToonInspector.GetShaderFolderPaths();
            foreach(string shaderGuid in AssetDatabase.FindAssets("t:shader", shaderFolderPaths))
            {
                string shaderPath = AssetDatabase.GUIDToAssetPath(shaderGuid);
                lilToonInspector.RewriteSettingPath(shaderPath);
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Create files
            if(!Directory.Exists(settingFolderPath))
            {
                // Setting Folder
                Directory.CreateDirectory(settingFolderPath);

                if(!File.Exists(shaderSettingHLSLPath))
                {
                    StreamWriter sw = new StreamWriter(shaderSettingHLSLPath,false);
                    sw.Write("//INITIALIZE\r\n#ifndef LIL_SETTING_INCLUDED\r\n#define LIL_SETTING_INCLUDED\r\n\r\n#define LIL_FEATURE_MAIN_TONE_CORRECTION\r\n#define LIL_FEATURE_SHADOW\r\n#define LIL_FEATURE_TEX_SHADOW_STRENGTH\r\n#define LIL_FEATURE_EMISSION_1ST\r\n#define LIL_FEATURE_NORMAL_1ST\r\n#define LIL_FEATURE_MATCAP\r\n#define LIL_FEATURE_TEX_MATCAP_MASK\r\n#define LIL_FEATURE_RIMLIGHT\r\n#define LIL_FEATURE_TEX_RIMLIGHT_COLOR\r\n#define LIL_FEATURE_TEX_OUTLINE_COLOR\r\n#define LIL_FEATURE_TEX_OUTLINE_WIDTH\r\n\r\n#endif");
                    sw.Close();
                    AssetDatabase.ImportAsset(shaderSettingHLSLPath);
                    Debug.Log("Generate setting hlsl file");
                }

                // Editor
                if(!File.Exists(lilToonInspector.rspPath))
                {
                    StreamWriter sw = new StreamWriter(lilToonInspector.rspPath,true);
                    sw.Write("-r:System.Drawing.dll\n-define:SYSTEM_DRAWING");
                    sw.Close();
                    AssetDatabase.Refresh();
                    AssetDatabase.ImportAsset(editorPath);
                }

                StreamReader sr = new StreamReader(lilToonInspector.rspPath);
                string s = sr.ReadToEnd();
                sr.Close();

                if(!s.Contains("r:System.Drawing.dll"))
                {
                    StreamWriter sw = new StreamWriter(lilToonInspector.rspPath,true);
                    sw.Write("\n-r:System.Drawing.dll");
                    sw.Close();
                    AssetDatabase.Refresh();
                    AssetDatabase.ImportAsset(editorPath);
                }
                if(!s.Contains("define:SYSTEM_DRAWING"))
                {
                    StreamWriter sw = new StreamWriter(lilToonInspector.rspPath,true);
                    sw.Write("\n-define:SYSTEM_DRAWING");
                    sw.Close();
                    AssetDatabase.Refresh();
                    AssetDatabase.ImportAsset(editorPath);
                }
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Version check
            if(!File.Exists(lilToonInspector.versionInfoTempPath))
            {
                CoroutineHandler.StartStaticCoroutine(GetLatestVersionInfo());
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Migration
            if(lilToonInspector.edSet.currentVersionValue < lilToonInspector.currentVersionValue)
            {
                lilToonInspector.MigrateMaterials();
                lilToonInspector.edSet.currentVersionValue = lilToonInspector.currentVersionValue;
                lilToonInspector.SaveEditorSettingTemp();
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Scan imported assets
            AssetDatabase.importPackageCompleted += _ =>
            {
                lilToonSetting shaderSetting = null;
                lilToonInspector.InitializeShaderSetting(ref shaderSetting);
                lilToonInspector.InitializeSettingHLSL(ref shaderSetting);

                if(!shaderSetting.isLocked && !shaderSetting.shouldNotScan && File.Exists(lilToonInspector.packageListTempPath))
                {
                    lilToonSetting shaderSettingNew = UnityEngine.Object.Instantiate(shaderSetting);
                    StreamReader srPackage = new StreamReader(lilToonInspector.packageListTempPath);
                    string str;
                    while((str = srPackage.ReadLine()) != null)
                    {
                        if(str.EndsWith(".mat") && AssetDatabase.GetMainAssetTypeAtPath(str) == typeof(Material))
                        {
                            Material material = AssetDatabase.LoadAssetAtPath<Material>(str);
                            if(!material.shader.name.Contains("lilToon")) continue;
                            lilToonInspector.MigrateMaterial(material);
                            if(material.shader.name.Contains("Lite")) continue;
                            lilToonInspector.SetupShaderSettingFromMaterial(material, ref shaderSettingNew);
                        }
                        if(str.EndsWith(".anim") && AssetDatabase.GetMainAssetTypeAtPath(str) == typeof(AnimationClip))
                        {
                            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(str);
                            lilToonInspector.SetupShaderSettingFromAnimationClip(clip, ref shaderSettingNew);
                        }
                    }
                    srPackage.Close();

                    if(!lilToonInspector.EqualsShaderSetting(shaderSettingNew, shaderSetting) && EditorUtility.DisplayDialog("lilToon",lilToonInspector.GetLoc("sUtilNewFeatureFound"),lilToonInspector.GetLoc("sYes"),lilToonInspector.GetLoc("sNo")))
                    {
                        // Apply
                        lilToonInspector.CopyShaderSetting(ref shaderSetting, shaderSettingNew);
                        EditorUtility.SetDirty(shaderSetting);
                        AssetDatabase.SaveAssets();
                        lilToonInspector.ApplyShaderSetting(shaderSetting);
                        AssetDatabase.Refresh();
                        Debug.Log("Finish scanning assets");
                    }
                    File.Delete(lilToonInspector.packageListTempPath);
                }

                // Refresh
                string[] shaderFolderPaths2 = lilToonInspector.GetShaderFolderPaths();
                bool isShadowReceive = (shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW) || shaderSetting.LIL_FEATURE_BACKLIGHT;
                Array.ForEach(
                    AssetDatabase.FindAssets("t:shader", shaderFolderPaths2),
                    shaderGuid => lilToonInspector.RewriteReceiveShadow(AssetDatabase.GUIDToAssetPath(shaderGuid), isShadowReceive)
                );
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(shaderSettingHLSLPath);
                AssetDatabase.Refresh();
            };
        }

        private static IEnumerator GetLatestVersionInfo()
        {
            using(UnityWebRequest webRequest = UnityWebRequest.Get(lilToonInspector.versionInfoURL))
            {
                #if UNITY_2017_2_OR_NEWER
                    yield return webRequest.SendWebRequest();
                #else
                    yield return webRequest.Send();
                #endif
                #if UNITY_2020_2_OR_NEWER
                    if(webRequest.result != UnityWebRequest.Result.ConnectionError)
                #else
                    if(!webRequest.isNetworkError)
                #endif
                {
                    StreamWriter sw = new StreamWriter(lilToonInspector.versionInfoTempPath,false);
                    sw.Write(webRequest.downloadHandler.text);
                    sw.Close();
                }
            }
        }
    }

    //------------------------------------------------------------------------------------------------------------------------------
    // based on CoroutineHandler.cs
    // https://github.com/Unity-Technologies/EndlessRunnerSampleGame/blob/master/Assets/Scripts/CoroutineHandler.cs
    public class CoroutineHandler : MonoBehaviour
    {
        static protected CoroutineHandler m_Instance;
        static public CoroutineHandler Instance
        {
            get
            {
                if(m_Instance == null)
                {
                    GameObject o = new GameObject("CoroutineHandler")
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };
                    m_Instance = o.AddComponent<CoroutineHandler>();
                }

                return m_Instance;
            }
        }

        public void OnDisable()
        {
            if(m_Instance) Destroy(m_Instance.gameObject);
        }

        static public Coroutine StartStaticCoroutine(IEnumerator coroutine)
        {
            return Instance.StartCoroutine(coroutine);
        }
    }
}
#endif