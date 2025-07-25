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

        public static string GetPackageJsonPath()           => GUIDToPath("397d2fa9e93fb5d44a9540d5f01437fc"); // "package.json"
        public static string GetBaseShaderFolderPath()      => GUIDToPath("d465bb256af2e3a4ca646387f4bd83e7"); // "BaseShaderResources"
        public static string GetEditorFolderPath()          => GUIDToPath("3e73d675b9c1adc4f8b6b8ef01bce51c"); // "Editor"
        public static string GetPresetsFolderPath()         => GUIDToPath("35817d21af2f3134182c4a7e4c07786b"); // "Presets"
        public static string GetEditorPath()                => GUIDToPath("aefa51cbc37d602418a38a02c3b9afb9"); // "Editor/lilInspector.cs"
        public static string GetShaderFolderPath()          => GUIDToPath("ac0a8f602b5e72f458f4914bf08f0269"); // "Shader"
        public static string GetShaderPipelinePath()        => GUIDToPath("32299664512e2e042bbc351c1d46d383"); // "Shader/Includes/lil_pipeline.hlsl";
        public static string GetShaderCommonPath()          => GUIDToPath("5520e766422958546bbe885a95d5a67e"); // "Shader/Includes/lil_common.hlsl";
        public static string GetGUIBoxInDarkPath()          => GUIDToPath("bb1313c9ea1425b41b74e98fd04bcbc8"); // "Editor/Resources/gui_box_inner_dark.guiskin"
        public static string GetGUIBoxInLightPath()         => GUIDToPath("f18d71f528511e748887f5e246abcc16"); // "Editor/Resources/gui_box_inner_light.guiskin"
        public static string GetGUIBoxInHalfDarkPath()      => GUIDToPath("a72199a4c9cc3714d8edfbc5d3b13823"); // "Editor/Resources/gui_box_inner_half_dark.guiskin"
        public static string GetGUIBoxInHalfLightPath()     => GUIDToPath("8343038a4a0cbef4d8af45c073520436"); // "Editor/Resources/gui_box_inner_half_light.guiskin"
        public static string GetGUIBoxOutDarkPath()         => GUIDToPath("29f3c01461cd0474eab36bf2e939bb58"); // "Editor/Resources/gui_box_outer_dark.guiskin"
        public static string GetGUIBoxOutLightPath()        => GUIDToPath("16cc103a658d8404894e66dd8f35cb77"); // "Editor/Resources/gui_box_outer_light.guiskin"
        public static string GetGUICustomBoxDarkPath()      => GUIDToPath("45dfb1bafd2c7d34ab453c29c0b1f46e"); // "Editor/Resources/gui_custom_box_dark.guiskin"
        public static string GetGUICustomBoxLightPath()     => GUIDToPath("a1ed8756474bfd34f80fa22e6c43b2e5"); // "Editor/Resources/gui_custom_box_light.guiskin"
        public static string GetCurrentRPPath()             => GUIDToPath("142b3aeca72105442a83089b616e92b8"); // "Editor/CurrentRP.txt"
        public static string GetClusterCreatorKitPath()     => GUIDToPath("6f11c0d5c326e4a6c851aa1c02ff11ee"); // "ClusterCreatorKit/package.json"
        public static string GetMainFolderPath() // "Assets/lilToon"
        {
            string editorPath = GetEditorFolderPath();
            return editorPath.Substring(0, editorPath.Length - 7);
        }
        public static string GetShaderSettingPath()         => "ProjectSettings/lilToonSetting.json";     // "ProjectSettings/lilToonSetting.json"
        public static string GetSettingLockPath()           => GetMainFolderPath() + "/SettingLock.json"; // "SettingLock.json"
        public static string[] GetShaderFolderPaths()       => new[] {GetShaderFolderPath()};
        public static string GetSettingFolderPath()         => GetMainFolderPath();
        public static string GUIDToPath(string GUID)        => AssetDatabase.GUIDToAssetPath(GUID);

        public static bool ExistsClusterCreatorKit() => !string.IsNullOrEmpty(GetClusterCreatorKitPath());

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