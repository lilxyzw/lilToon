#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
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
            lilToonInspector.ApplyEditorSettingTemp();
            lilLanguageManager.InitializeLanguage();

            //------------------------------------------------------------------------------------------------------------------------------
            // Create files
            if(!File.Exists(lilDirectoryManager.startupTempPath))
            {
                File.Create(lilDirectoryManager.startupTempPath);

                #if !SYSTEM_DRAWING
                    string editorPath = lilDirectoryManager.GetEditorPath();

                    // RSP
                    if(!File.Exists(lilDirectoryManager.rspPath))
                    {
                        StreamWriter sw = new StreamWriter(lilDirectoryManager.rspPath,true);
                        sw.Write("-r:System.Drawing.dll\n-define:SYSTEM_DRAWING");
                        sw.Close();
                        AssetDatabase.Refresh();
                        AssetDatabase.ImportAsset(editorPath);
                    }

                    StreamReader sr = new StreamReader(lilDirectoryManager.rspPath);
                    string s = sr.ReadToEnd();
                    sr.Close();

                    if(!s.Contains("r:System.Drawing.dll"))
                    {
                        StreamWriter sw = new StreamWriter(lilDirectoryManager.rspPath,true);
                        sw.Write("\n-r:System.Drawing.dll");
                        sw.Close();
                        AssetDatabase.Refresh();
                        AssetDatabase.ImportAsset(editorPath);
                    }
                    if(!s.Contains("define:SYSTEM_DRAWING"))
                    {
                        StreamWriter sw = new StreamWriter(lilDirectoryManager.rspPath,true);
                        sw.Write("\n-define:SYSTEM_DRAWING");
                        sw.Close();
                        AssetDatabase.Refresh();
                        AssetDatabase.ImportAsset(editorPath);
                    }
                #endif
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Shader setting
            string currentRPPath = lilDirectoryManager.GetCurrentRPPath();
            if(File.Exists(currentRPPath))
            {
                StreamReader srRP = new StreamReader(currentRPPath);
                string shaderRP = srRP.ReadLine();
                string shaderAPI = srRP.ReadLine();
                srRP.Close();

                bool shouldRewrite = false;
                string projectRP = lilRenderPipelineReader.GetRP().ToString();
                string projectAPI = SystemInfo.graphicsDeviceType.ToString();
                StreamWriter swRP = new StreamWriter(currentRPPath,false);
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
                    lilToonSetting shaderSetting = null;
                    lilToonSetting.InitializeShaderSetting(ref shaderSetting);
                    lilToonSetting.ApplyShaderSetting(shaderSetting);
                }
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Version check
            if(!File.Exists(lilDirectoryManager.versionInfoTempPath))
            {
                CoroutineHandler.StartStaticCoroutine(GetLatestVersionInfo());
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Migration
            if(lilToonInspector.edSet.currentVersionValue < lilConstants.currentVersionValue)
            {
                MigrateMaterials();
                lilToonInspector.edSet.currentVersionValue = lilConstants.currentVersionValue;
                lilToonInspector.SaveEditorSettingTemp();
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Turn on all settings when auto
            if(File.Exists(lilDirectoryManager.postBuildTempPath)) 
            {
                EditorApplication.delayCall += () =>
                {
                    lilToonSetting.SetShaderSettingAfterBuild();
                };
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
                    StreamWriter sw = new StreamWriter(lilDirectoryManager.versionInfoTempPath,false);
                    sw.Write(webRequest.downloadHandler.text);
                    sw.Close();
                }
            }
        }

        private static void MigrateMaterials()
        {
            foreach(string guid in AssetDatabase.FindAssets("t:material"))
            {
                Material material = AssetDatabase.LoadAssetAtPath<Material>(lilDirectoryManager.GUIDToPath(guid));
                MigrateMaterial(material);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void MigrateMaterial(Material material)
        {
            if(material.shader == null || !material.shader.name.Contains("lilToon")) return;
            int version = material.HasProperty("_lilToonVersion") ? (int)material.GetFloat("_lilToonVersion") : 0;
            if(version >= lilConstants.currentVersionValue) return;
            Debug.Log("[lilToon]Run migration: " + material.name);
            material.SetFloat("_lilToonVersion", lilConstants.currentVersionValue);

            // 1.2.7 -> 1.2.8
            if(version < 21)
            {
                if(material.shader.name.Contains("_lil/lilToonMulti"))
                {
                    int renderQueue = material.renderQueue;
                    material.shader = material.HasProperty("_UseOutline") && material.GetFloat("_UseOutline") != 0.0f ? Shader.Find("Hidden/lilToonMultiOutline") : Shader.Find("_lil/lilToonMulti");
                    material.renderQueue = renderQueue;
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