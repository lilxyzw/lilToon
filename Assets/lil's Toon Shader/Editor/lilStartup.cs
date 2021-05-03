#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class lilStartup {
    static lilStartup()
    {
        #if NET_4_6
            string fullPath = Path.GetFullPath("Assets/csc.rsp");
        #else
            string fullPath = Path.GetFullPath("Assets/mcs.rsp");
        #endif
        string edotorPath = "Assets/lil's Toon Shader/Editor/lilInspector.cs";
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