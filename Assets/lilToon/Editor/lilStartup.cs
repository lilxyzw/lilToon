#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace lilToon
{
    public static class lilStartup
    {
        [InitializeOnLoadMethod]
        public static void lilStartupMethod()
        {
            //------------------------------------------------------------------------------------------------------------------------------
            // Variables
            lilLanguageManager.InitializeLanguage();

            AssetDatabase.importPackageStarted -= PackageVersionChecker;
            AssetDatabase.importPackageStarted += PackageVersionChecker;

            //------------------------------------------------------------------------------------------------------------------------------
            // Create files
            if(!lilEditorParameters.instance.startupEnd)
            {
                lilEditorParameters.instance.startupEnd = true;

                #if LILTOON_DISABLE_ASSET_MODIFICATION == false
                #if !SYSTEM_DRAWING
                    string editorPath = lilDirectoryManager.GetEditorPath();

                    // RSP
                    if(!File.Exists(lilDirectoryManager.rspPath))
                    {
                        var sw = new StreamWriter(lilDirectoryManager.rspPath,true);
                        sw.Write("-r:System.Drawing.dll" + Environment.NewLine + "-define:SYSTEM_DRAWING");
                        sw.Close();
                        AssetDatabase.Refresh();
                        AssetDatabase.ImportAsset(editorPath);
                    }

                    var sr = new StreamReader(lilDirectoryManager.rspPath);
                    string s = sr.ReadToEnd();
                    sr.Close();

                    if(!s.Contains("r:System.Drawing.dll"))
                    {
                        var sw = new StreamWriter(lilDirectoryManager.rspPath,true);
                        sw.Write(Environment.NewLine + "-r:System.Drawing.dll");
                        sw.Close();
                        AssetDatabase.Refresh();
                        AssetDatabase.ImportAsset(editorPath);
                    }
                    if(!s.Contains("define:SYSTEM_DRAWING"))
                    {
                        var sw = new StreamWriter(lilDirectoryManager.rspPath,true);
                        sw.Write(Environment.NewLine + "-define:SYSTEM_DRAWING");
                        sw.Close();
                        AssetDatabase.Refresh();
                        AssetDatabase.ImportAsset(editorPath);
                    }
                #endif
                #endif //LILTOON_DISABLE_ASSET_MODIFICATION
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Shader setting
            lilToonSetting shaderSetting = null;
            lilToonSetting.InitializeShaderSetting(ref shaderSetting);
            string currentRPPath = lilDirectoryManager.GetCurrentRPPath();
            if(File.Exists(currentRPPath))
            {
                var srRP = new StreamReader(currentRPPath);
                string shaderRP = srRP.ReadLine();
                string shaderAPI = srRP.ReadLine();
                srRP.Close();

                bool shouldRewrite = false;
                string projectRP = lilRenderPipelineReader.GetRP().ToString();
                string projectAPI = SystemInfo.graphicsDeviceType.ToString();
                var swRP = new StreamWriter(currentRPPath,false);
                swRP.WriteLine(projectRP);
                swRP.WriteLine(projectAPI);

                if(shaderRP != projectRP)
                {
                    Debug.Log("[lilToon] Switch " + shaderRP + " to " + projectRP);
                    shouldRewrite = true;
                }

                if(shaderAPI != projectAPI)
                {
                    Debug.Log("[lilToon] Switch " + shaderAPI + " to " + projectAPI);
                    shouldRewrite = true;
                }

                swRP.Close();
                if(shouldRewrite)
                {
                    if(shaderSetting.isDebugOptimize)
                    {
                        lilToonSetting.ApplyShaderSettingOptimized();
                    }
                    else
                    {
                        if(lilShaderAPI.IsTextureLimitedAPI())
                        {
                            lilToonSetting.TurnOffAllShaderSetting(ref shaderSetting);
                            lilToonSetting.CheckTextures(ref shaderSetting);
                        }

                        lilToonSetting.TurnOnAllShaderSetting(ref shaderSetting);
                        lilToonSetting.ApplyShaderSetting(shaderSetting);
                    }
                }
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Version check
            if(string.IsNullOrEmpty(lilEditorParameters.instance.versionInfo))
            {
                CoroutineHandler.StartStaticCoroutine(GetLatestVersionInfo());
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Update
            if(shaderSetting.previousVersion != lilConstants.currentVersionValue)
            {
                // Migrate Materials
                if(shaderSetting.isMigrateInStartUp) EditorApplication.delayCall += MigrateMaterials;
                shaderSetting.previousVersion = lilConstants.currentVersionValue;
                lilToonSetting.SaveShaderSetting(shaderSetting);

                #if UNITY_2019_4_OR_NEWER
                    // Update custom shaders
                    var folders = new List<string>();
                    foreach(var shaderPath in lilDirectoryManager.FindAssetsPath("t:shader").Where(p => p.Contains(".lilcontainer")))
                    {
                        string folder = Path.GetDirectoryName(shaderPath);
                        if(folders.Contains(folder)) continue;
                        var shader = AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);
                        int versionIndex = shader.FindPropertyIndex("_lilToonVersion");
                        if(
                            versionIndex != -1 &&
                            shader.GetPropertyDefaultFloatValue(versionIndex) == lilConstants.currentVersionValue &&
                            !ShaderUtil.ShaderHasError(shader)
                        ) continue;
                        folders.Add(folder);
                    }
                    foreach(var folder in folders)
                    {
                        AssetDatabase.ImportAsset(folder, ImportAssetOptions.ImportRecursive);
                    }
                #endif
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Turn on all settings when auto
            if(!string.IsNullOrEmpty(lilEditorParameters.instance.modifiedShaders)) 
            {
                EditorApplication.delayCall -= lilToonSetting.SetShaderSettingAfterBuild;
                EditorApplication.delayCall += lilToonSetting.SetShaderSettingAfterBuild;
            }
        }

        private static IEnumerator GetLatestVersionInfo()
        {
            using(UnityWebRequest webRequest = UnityWebRequest.Get(lilConstants.versionInfoURL))
            {
                yield return webRequest.SendWebRequest();
                #if UNITY_2020_2_OR_NEWER
                    if(webRequest.result != UnityWebRequest.Result.ConnectionError)
                #else
                    if(!webRequest.isNetworkError)
                #endif
                {
                    lilEditorParameters.instance.versionInfo = webRequest.downloadHandler.text;
                }
            }
        }

        internal static void MigrateMaterials()
        {
            EditorApplication.delayCall -= MigrateMaterials;
            foreach(var material in lilDirectoryManager.FindAssets<Material>("t:material"))
            {
                MigrateMaterial(material);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        internal static void MigrateMaterial(Material material)
        {
            if(!lilMaterialUtils.CheckShaderIslilToon(material)) return;
            var id = material.shader.GetPropertyNameId(material.shader.FindPropertyIndex("_lilToonVersion"));
            int version = 0;
            if(material.HasProperty(id)) version = (int)material.GetFloat(id);
            if(version >= lilConstants.currentVersionValue) return;
            Debug.Log("[lilToon]Run migration: " + material.name);
            material.SetFloat(id, lilConstants.currentVersionValue);

            // 1.2.7 -> 1.2.8
            if(version < 21)
            {
                if(material.shader.name.Contains("_lil/lilToonMulti"))
                {
                    int renderQueue = lilMaterialUtils.GetTrueRenderQueue(material);
                    material.shader = material.HasProperty("_UseOutline") && material.GetFloat("_UseOutline") != 0.0f ? Shader.Find("Hidden/lilToonMultiOutline") : Shader.Find("_lil/lilToonMulti");
                    material.renderQueue = renderQueue;
                }
            }
        }

        private static void PackageVersionChecker(string packageName)
        {
            int indexlil = packageName.IndexOf("lilToon_");
            if(indexlil < 0) indexlil = packageName.IndexOf("jp.lilxyzw.liltoon-");
            if(indexlil < 0) return;

            if(lilDirectoryManager.GetSettingLockPath().Contains("Packages"))
            {
                if(!EditorUtility.DisplayDialog("lilToon", lilLanguageManager.GetLoc("sDialogImportPackage"), lilLanguageManager.GetLoc("sYes"), lilLanguageManager.GetLoc("sNo")))
                {
                    CoroutineHandler.StartStaticCoroutine(ClosePackageImportWindow());
                    return;
                }
            }

            string packageVerString = packageName.Substring(indexlil + 8);

            var semPackage = new SemVerParser(packageVerString, true);
            var semCurrent = new SemVerParser(lilConstants.currentVersionName);
            if(semPackage == null || semCurrent == null) return;

            if(semPackage < semCurrent)
            {
                if(!EditorUtility.DisplayDialog("lilToon", lilLanguageManager.GetLoc("sDialogImportOldVer"), lilLanguageManager.GetLoc("sYes"), lilLanguageManager.GetLoc("sNo")))
                {
                    CoroutineHandler.StartStaticCoroutine(ClosePackageImportWindow());
                    return;
                }
            }
        }

        private static IEnumerator ClosePackageImportWindow()
        {
            var type = typeof(Editor).Assembly.GetType("UnityEditor.PackageImport");
            var method = typeof(EditorWindow).GetMethod("HasOpenInstances", BindingFlags.Static | BindingFlags.Public);
            if(method != null)
            {
                var genmethod = method.MakeGenericMethod(type);
                while(!(bool)genmethod.Invoke(null,null))
                {
                    yield return null;
                }
                EditorWindow.GetWindow(type).Close();
            }
        }

        private static int[] ReadSemVer(string sem)
        {
            var parts = sem.Split('.');
            if(parts.Length < 3) return null;
            int major, minor, patch;
            try
            {
                major = int.Parse(parts[0]);
                minor = int.Parse(parts[1]);
                patch = int.Parse(parts[2]);
            }
            catch
            {
                return null;
            }
            return new[]{major,minor,patch};
        }
    }

    //------------------------------------------------------------------------------------------------------------------------------
    // based on CoroutineHandler.cs
    // https://github.com/Unity-Technologies/EndlessRunnerSampleGame/blob/master/Assets/Scripts/CoroutineHandler.cs
    public class CoroutineHandler : MonoBehaviour
    {
        protected static CoroutineHandler m_Instance;
        public static CoroutineHandler Instance
        {
            get
            {
                if(m_Instance == null)
                {
                    var o = new GameObject("CoroutineHandler")
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

        public static Coroutine StartStaticCoroutine(IEnumerator coroutine)
        {
            return Instance.StartCoroutine(coroutine);
        }
    }
}
#endif