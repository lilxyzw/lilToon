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
            string editorPath = lilToonInspector.GetEditorPath();

            lilToonInspector.ApplyEditorSettingTemp();
            lilToonInspector.InitializeLanguage();

            //------------------------------------------------------------------------------------------------------------------------------
            // Create files
            if(!File.Exists(lilToonInspector.startupTempPath))
            {
                File.Create(lilToonInspector.startupTempPath);

                // RSP
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
            // Turn on all settings when auto
            if(File.Exists(lilToonInspector.postBuildTempPath)) 
            {
                EditorApplication.delayCall += () =>
                {
                    lilToonInspector.SetShaderSettingAfterBuild();
                };
            }
        }

        private static IEnumerator GetLatestVersionInfo()
        {
            using(UnityWebRequest webRequest = UnityWebRequest.Get(lilToonInspector.versionInfoURL))
            {
                yield return webRequest.SendWebRequest();
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