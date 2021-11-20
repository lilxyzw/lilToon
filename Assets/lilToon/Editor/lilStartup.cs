#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Collections;

namespace lilToon
{
    public class lilStartup {
        [InitializeOnLoadMethod]
        static void lilStartupMethod()
        {
            string editorPath = lilToonInspector.GetEditorPath();
            lilToonInspector.isUPM = editorPath.Contains("Packages");
            string settingFolderPath = lilToonInspector.GetSettingFolderPath();
            string shaderSettingHLSLPath = lilToonInspector.GetShaderSettingHLSLPath();
            string shaderCommonPath = lilToonInspector.GetShaderCommonPath();

            lilToonInspector.ApplyEditorSettingTemp();
            lilToonInspector.InitializeLanguage();

            // Initialize
            StreamReader csr = new StreamReader(shaderCommonPath);
            string cs = csr.ReadToEnd();
            csr.Close();
            if(lilToonInspector.isUPM && cs.Contains("#include \"../../../lilToonSetting/lil_setting.hlsl\""))
            {
                cs = cs.Replace(
                    "#include \"../../../lilToonSetting/lil_setting.hlsl\"",
                    "#include \"Assets/lilToonSetting/lil_setting.hlsl\"");
                StreamWriter csw = new StreamWriter(shaderCommonPath,false);
                csw.Write(cs);
                csw.Close();
            }
            else if(!lilToonInspector.isUPM && cs.Contains("#include \"Assets/lilToonSetting/lil_setting.hlsl\""))
            {
                cs = cs.Replace(
                    "#include \"Assets/lilToonSetting/lil_setting.hlsl\"",
                    "#include \"../../../lilToonSetting/lil_setting.hlsl\"");
                StreamWriter csw = new StreamWriter(shaderCommonPath,false);
                csw.Write(cs);
                csw.Close();
            }

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

            // Version check
            if(!File.Exists(lilToonInspector.versionInfoTempPath))
            {
                CoroutineHandler.StartStaticCoroutine(GetLatestVersionInfo());
            }

            AssetDatabase.importPackageCompleted += OnImportPackageCompleted =>
            {
                lilToonSetting shaderSetting = null;
                lilToonInspector.InitializeShaderSetting(ref shaderSetting);
                lilToonInspector.InitializeSettingHLSL(ref shaderSetting);

                // Scan imported assets
                if(File.Exists(lilToonInspector.packageListTempPath))
                {
                    lilToonSetting shaderSettingNew = UnityEngine.Object.Instantiate(shaderSetting);
                    StreamReader srPackage = new StreamReader(lilToonInspector.packageListTempPath);
                    string tempPackage = srPackage.ReadToEnd();
                    srPackage.Close();

                    string[] importedAssets = tempPackage.Split('\n');

                    foreach(string str in importedAssets)
                    {
                        if(str.EndsWith(".mat") && AssetDatabase.GetMainAssetTypeAtPath(str) == typeof(Material))
                        {
                            Material material = (Material)AssetDatabase.LoadAssetAtPath(str, typeof(Material));
                            if(!material.shader.name.Contains("lilToon") || material.shader.name.Contains("Lite")) continue;
                            lilToonInspector.SetupShaderSettingFromMaterial(material, ref shaderSettingNew);
                        }
                        if(str.EndsWith(".anim") && AssetDatabase.GetMainAssetTypeAtPath(str) == typeof(AnimationClip))
                        {
                            AnimationClip clip = (AnimationClip)AssetDatabase.LoadAssetAtPath(str, typeof(AnimationClip));
                            lilToonInspector.SetupShaderSettingFromAnimationClip(clip, ref shaderSettingNew);
                        }
                    }

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
                string[] shaderFolderPaths = lilToonInspector.GetShaderFolderPaths();
                bool isShadowReceive = shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW || shaderSetting.LIL_FEATURE_BACKLIGHT;
                string[] shaderGuids = AssetDatabase.FindAssets("t:shader", shaderFolderPaths);
                foreach(string shaderGuid in shaderGuids)
                {
                    string shaderPath = AssetDatabase.GUIDToAssetPath(shaderGuid);
                    lilToonInspector.RewriteReceiveShadow(shaderPath, isShadowReceive);
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(shaderSettingHLSLPath);
                AssetDatabase.Refresh();
            };
        }

        static IEnumerator GetLatestVersionInfo()
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

    public class CoroutineHandler : MonoBehaviour
    {
        static protected CoroutineHandler m_Instance;
        static public CoroutineHandler instance
        {
            get
            {
                if(m_Instance == null)
                {
                    GameObject o = new GameObject("CoroutineHandler");
                    o.hideFlags = HideFlags.HideAndDontSave;
                    m_Instance = o.AddComponent<CoroutineHandler>();
                }

                return m_Instance;
            }
        }

        public void OnDisable()
        {
            if(m_Instance)
                Destroy(m_Instance.gameObject);
        }

        static public Coroutine StartStaticCoroutine(IEnumerator coroutine)
        {
            return instance.StartCoroutine(coroutine);
        }
    }
}
#endif