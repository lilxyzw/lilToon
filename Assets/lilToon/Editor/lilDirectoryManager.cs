#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace lilToon
{
    internal class lilDirectoryManager
    {
        public const string editorSettingTempPath           = "Temp/lilToonEditorSetting";
        public const string languageSettingTempPath         = "Temp/lilToonLanguageSetting";
        public const string versionInfoTempPath             = "Temp/lilToonVersion";
        public const string packageListTempPath             = "Temp/lilToonPackageList";
        public const string forceOptimizeBuildTempPath      = "Temp/lilToonForceOptimizeBuild";
        public const string postBuildTempPath               = "Temp/lilToonPostBuild";
        public const string startupTempPath                 = "Temp/lilToonStartup";

        #if NET_4_6
            public const string rspPath = "Assets/csc.rsp";
        #else
            public const string rspPath = "Assets/mcs.rsp";
        #endif

        public static string GetBaseShaderFolderPath()      { return GUIDToPath("d465bb256af2e3a4ca646387f4bd83e7"); } // "Assets/lilToon/BaseShaderResources"
        public static string GetEditorFolderPath()          { return GUIDToPath("3e73d675b9c1adc4f8b6b8ef01bce51c"); } // "Assets/lilToon/Editor"
        public static string GetPresetsFolderPath()         { return GUIDToPath("35817d21af2f3134182c4a7e4c07786b"); } // "Assets/lilToon/Presets"
        public static string GetEditorPath()                { return GUIDToPath("aefa51cbc37d602418a38a02c3b9afb9"); } // "Assets/lilToon/Editor/lilInspector.cs"
        public static string GetShaderFolderPath()          { return GUIDToPath("ac0a8f602b5e72f458f4914bf08f0269"); } // "Assets/lilToon/Shader"
        public static string GetShaderPipelinePath()        { return GUIDToPath("32299664512e2e042bbc351c1d46d383"); } // "Assets/lilToon/Shader/Includes/lil_pipeline.hlsl";
        public static string GetShaderCommonPath()          { return GUIDToPath("5520e766422958546bbe885a95d5a67e"); } // "Assets/lilToon/Shader/Includes/lil_common.hlsl";
        public static string GetEditorLanguageFileGUID()    { return GUIDToPath("a63ad2f5296744a4bad011de744ba8ba"); } // "Assets/lilToon/Editor/Resources/lang.txt"
        public static string GetGUIBoxInDarkPath()          { return GUIDToPath("bb1313c9ea1425b41b74e98fd04bcbc8"); } // "Assets/lilToon/Editor/Resources/gui_box_inner_dark.guiskin"
        public static string GetGUIBoxInLightPath()         { return GUIDToPath("f18d71f528511e748887f5e246abcc16"); } // "Assets/lilToon/Editor/Resources/gui_box_inner_light.guiskin"
        public static string GetGUIBoxInHalfDarkPath()      { return GUIDToPath("a72199a4c9cc3714d8edfbc5d3b13823"); } // "Assets/lilToon/Editor/Resources/gui_box_inner_half_dark.guiskin"
        public static string GetGUIBoxInHalfLightPath()     { return GUIDToPath("8343038a4a0cbef4d8af45c073520436"); } // "Assets/lilToon/Editor/Resources/gui_box_inner_half_light.guiskin"
        public static string GetGUIBoxOutDarkPath()         { return GUIDToPath("29f3c01461cd0474eab36bf2e939bb58"); } // "Assets/lilToon/Editor/Resources/gui_box_outer_dark.guiskin"
        public static string GetGUIBoxOutLightPath()        { return GUIDToPath("16cc103a658d8404894e66dd8f35cb77"); } // "Assets/lilToon/Editor/Resources/gui_box_outer_light.guiskin"
        public static string GetGUICustomBoxDarkPath()      { return GUIDToPath("45dfb1bafd2c7d34ab453c29c0b1f46e"); } // "Assets/lilToon/Editor/Resources/gui_custom_box_dark.guiskin"
        public static string GetGUICustomBoxLightPath()     { return GUIDToPath("a1ed8756474bfd34f80fa22e6c43b2e5"); } // "Assets/lilToon/Editor/Resources/gui_custom_box_light.guiskin"
        public static string GetCurrentRPPath()             { return GUIDToPath("142b3aeca72105442a83089b616e92b8"); } // "Assets/lilToon/Editor/CurrentRP.txt"
        public static string GetAvatarEncryptionPath()      { return GUIDToPath("f9787bf8ed5154f4b931278945ac8ca1"); } // "Assets/AvaterEncryption";
        public static string GetAvaCryptV2Path()            { return GUIDToPath("3d37d2c6de7be1b4182d5f0bfd480365"); } // "com.geotetra.gtavacrypt/package.json";
        public static string GetClusterCreatorKitPath()     { return GUIDToPath("6f11c0d5c326e4a6c851aa1c02ff11ee"); } // "ClusterCreatorKit/package.json"
        public static string GetMainFolderPath() // "Assets/lilToon"
        {
            string editorPath = GetEditorFolderPath();
            return editorPath.Substring(0, editorPath.Length - 7);
        }
        public static string GetShaderSettingPath()         { return "ProjectSettings/lilToonSetting.json";          } // "ProjectSettings/lilToonSetting.json"
        public static string GetSettingLockPath()           { return GetMainFolderPath() + "/SettingLock.json"; }      // "Assets/lilToon/SettingLock.json"
        public static string[] GetShaderFolderPaths()       { return new[] {GetShaderFolderPath()}; }
        public static string GetSettingFolderPath()         { return GetMainFolderPath(); }
        public static string GUIDToPath(string GUID)        { return AssetDatabase.GUIDToAssetPath(GUID); }

        public static bool ExistsEncryption() { return !string.IsNullOrEmpty(GetAvatarEncryptionPath()); }
        public static bool ExistsAvaCryptV2() { return !string.IsNullOrEmpty(GetAvaCryptV2Path()); }
        public static bool ExistsClusterCreatorKit() { return !string.IsNullOrEmpty(GetClusterCreatorKitPath()); }

        public static IEnumerable<string> FindAssetsPath(string filter, string[] folders)
        {
            return AssetDatabase.FindAssets(filter, folders).Select(id => GUIDToPath(id));
        }

        public static IEnumerable<string> FindAssetsPath(string filter)
        {
            return AssetDatabase.FindAssets(filter).Select(id => GUIDToPath(id));
        }

        public static IEnumerable<T> FindAssets<T>(string filter, string[] folders) where T : Object
        {
            return FindAssetsPath(filter, folders).Select(p => AssetDatabase.LoadAssetAtPath<T>(p));
        }

        public static IEnumerable<T> FindAssets<T>(string filter) where T : Object
        {
            return FindAssetsPath(filter).Select(p => AssetDatabase.LoadAssetAtPath<T>(p));
        }
    }
}
#endif