#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

using Object = UnityEngine.Object;

namespace lilToon
{
    public partial class lilToonInspector
    {

        //------------------------------------------------------------------------------------------------------------------------------
        // Obsolete
        #region
        [Obsolete("Use \"DrawCustomProperties(Material material)\" instead.")]
        protected virtual void DrawCustomProperties(
            MaterialEditor materialEditor,
            Material material,
            GUIStyle boxOuter,
            GUIStyle boxInnerHalf,
            GUIStyle boxInner,
            GUIStyle customBox,
            GUIStyle customToggleFont,
            GUIStyle offsetButton)
        {
        }

        [Obsolete("This may be deleted in the future.")]
        public static bool EqualsShaderSetting(lilToonSetting ssA, lilToonSetting ssB)
        {
            if((ssA == null && ssB != null) || (ssA != null && ssB == null)) return false;
            if(ssA == null && ssB == null) return true;
            return !typeof(lilToonSetting).GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly).Any(f => f.FieldType == typeof(bool) && (bool)f.GetValue(ssA) != (bool)f.GetValue(ssB));
        }

        [Obsolete("This may be deleted in the future.")]
        public static void CopyShaderSetting(ref lilToonSetting ssA, lilToonSetting ssB)
        {
            if(ssA == null || ssB == null) return;

            foreach(var field in typeof(lilToonSetting).GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                field.SetValue(ssA, field.GetValue(ssB));
            }
        }

        [Obsolete("Use \"lilToonSetting.InitializeShaderSetting(ref lilToonSetting shaderSetting)\" instead.")]
        public static void InitializeShaderSetting(ref lilToonSetting shaderSetting)
        {
            lilToonSetting.InitializeShaderSetting(ref shaderSetting);
        }

        [Obsolete("Use \"lilToonSetting.TurnOffAllShaderSetting(ref lilToonSetting shaderSetting)\" instead.")]
        public static void TurnOffAllShaderSetting(ref lilToonSetting shaderSetting)
        {
            lilToonSetting.TurnOffAllShaderSetting(ref shaderSetting);
        }

        [Obsolete("Use \"lilToonSetting.TurnOnAllShaderSetting(ref lilToonSetting shaderSetting)\" instead.")]
        public static void TurnOnAllShaderSetting(ref lilToonSetting shaderSetting)
        {
            lilToonSetting.TurnOnAllShaderSetting(ref shaderSetting);
        }

        [Obsolete("Use \"lilToonSetting.ApplyShaderSetting(lilToonSetting shaderSetting, string reportTitle = null)\" instead.")]
        public static void ApplyShaderSetting(lilToonSetting shaderSetting, string reportTitle = null)
        {
            lilToonSetting.ApplyShaderSetting(shaderSetting, reportTitle);
        }

        [Obsolete("Use \"lilToonSetting.BuildShaderSettingString(lilToonSetting shaderSetting, bool isFile)\" instead.")]
        public static string BuildShaderSettingString(lilToonSetting shaderSetting, bool isFile)
        {
            return lilToonSetting.BuildShaderSettingString(shaderSetting, isFile);
        }

        [Obsolete("Use \"lilToonSetting.BuildShaderSettingString(bool isFile)\" instead.")]
        public static string BuildShaderSettingString(bool isFile)
        {
            return lilToonSetting.BuildShaderSettingString(isFile);
        }

        [Obsolete("Use \"lilToonSetting.ApplyShaderSettingOptimized()\" instead.")]
        public static void ApplyShaderSettingOptimized()
        {
            lilToonSetting.ApplyShaderSettingOptimized();
        }

        [Obsolete("Use \"lilToonSetting.SetShaderSettingAfterBuild()\" instead.")]
        public static void SetShaderSettingAfterBuild()
        {
            lilToonSetting.SetShaderSettingAfterBuild();
        }

        [Obsolete("Use \"lilToonPreset.ApplyPreset(Material material, lilToonPreset preset, bool ismulti)\" instead.")]
        public static void ApplyPreset(Material material, lilToonPreset preset, bool ismulti)
        {
            lilToonPreset.ApplyPreset(material, preset, ismulti);
        }

        [Obsolete("Use \"lilTextureUtils.LoadTexture(ref Texture2D tex, string path)\" instead.")]
        public static void LoadTexture(ref Texture2D tex, string path)
        {
            lilTextureUtils.LoadTexture(ref tex, path);
        }

        [Obsolete("Use \"lilTextureUtils.SaveTextureToPng(string path, string add, Texture2D tex)\" instead.")]
        public static string SavePng(string path, string add, Texture2D tex)
        {
            return lilTextureUtils.SaveTextureToPng(path, add, tex);
        }

        [Obsolete("Use \"lilTextureUtils.ConvertGifToAtlas(Object tex)\" instead.")]
        public static string ConvertGifToAtlas(Object tex)
        {
            return lilTextureUtils.ConvertGifToAtlas(tex);
        }

        [Obsolete("Use \"lilTextureUtils.ConvertGifToAtlas(Object tex, out int frameCount, out int loopXY, out int duration, out float xScale, out float yScale)\" instead.")]
        public static string ConvertGifToAtlas(Object tex, out int frameCount, out int loopXY, out int duration, out float xScale, out float yScale)
        {
            return lilTextureUtils.ConvertGifToAtlas(tex, out frameCount, out loopXY, out duration, out xScale, out yScale);
        }

        [Obsolete("Use \"lilLanguageManager.InitializeLanguage()\" instead.")]
        public static void InitializeLanguage()
        {
            lilLanguageManager.InitializeLanguage();
        }

        [Obsolete("Use \"lilEditorGUI.InitializeBox(int border, int margin, int padding)\" instead.")]
        public static GUIStyle InitializeBox(int border, int margin, int padding)
        {
            return lilEditorGUI.InitializeBox(border, margin, padding);
        }

        [Obsolete("Use \"lilEditorGUI.DrawWebButton(string text, string URL)\" instead.")]
        public static void DrawWebButton(string text, string URL)
        {
            lilEditorGUI.DrawWebButton(text, URL);
        }

        [Obsolete("Use \"condition = lilEditorGUI.DrawSimpleFoldout(string label, bool condition, GUIStyle style, bool isCustomEditor = true)\" instead.")]
        public static void DrawSimpleFoldout(string label, ref bool condition, GUIStyle style, bool isCustomEditor = true)
        {
            condition = lilEditorGUI.DrawSimpleFoldout(label, condition, style, isCustomEditor);
        }

        [Obsolete("Use \"condition = lilEditorGUI.DrawSimpleFoldout(string label, bool condition, bool isCustomEditor = true)\" instead.")]
        public static void DrawSimpleFoldout(string label, ref bool condition, bool isCustomEditor = true)
        {
            condition = lilEditorGUI.DrawSimpleFoldout(label, condition, isCustomEditor);
        }

        [Obsolete("Use \"condition = lilEditorGUI.DrawSimpleFoldout(MaterialEditor materialEditor, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, bool condition, bool isCustomEditor = true)\" instead.")]
        public static void DrawSimpleFoldout(GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, ref bool condition, bool isCustomEditor = true)
        {
            condition = lilEditorGUI.DrawSimpleFoldout(m_MaterialEditor, guiContent, textureName, rgba, condition, isCustomEditor);
        }

        [Obsolete("Use \"condition = lilEditorGUI.DrawSimpleFoldout(MaterialEditor materialEditor, GUIContent guiContent, MaterialProperty textureName, bool condition, bool isCustomEditor = true)\" instead.")]
        public static void DrawSimpleFoldout(GUIContent guiContent, MaterialProperty textureName, ref bool condition, bool isCustomEditor = true)
        {
            condition = lilEditorGUI.DrawSimpleFoldout(m_MaterialEditor, guiContent, textureName, condition, isCustomEditor);
        }

        [Obsolete("Use \"lilShaderManager.InitializeShaders()\" instead.")]
        public static void InitializeShaders()
        {
            lilShaderManager.InitializeShaders();
        }

        [Obsolete("Use \"lilMaterialUtils.CheckMainTextureName(string name)\" instead.")]
        public static bool CheckMainTextureName(string name)
        {
            return lilMaterialUtils.CheckMainTextureName(name);
        }

        [Obsolete("Use \"lilMaterialUtils.RemoveUnusedTexture(Material material)\" instead.")]
        public static void RemoveUnusedTexture(Material material)
        {
            lilMaterialUtils.RemoveUnusedTexture(material);
        }

        [Obsolete("Use \"lilToonPreset.ApplyPreset(Material material, lilToonPreset preset, bool ismulti)\" instead.")]
        public static void ApplyPreset(Material material, lilToonPreset preset)
        {
            lilToonPreset.ApplyPreset(material, preset, isMulti);
        }

        [Obsolete("Use \"lilEditorGUI.ConvertGifToAtlas(MaterialProperty tex, MaterialProperty decalAnimation, MaterialProperty decalSubParam, MaterialProperty isDecal)\" instead.")]
        public static void ConvertGifToAtlas(MaterialProperty tex, MaterialProperty decalAnimation, MaterialProperty decalSubParam, MaterialProperty isDecal)
        {
            lilEditorGUI.ConvertGifToAtlas(tex, decalAnimation, decalSubParam, isDecal);
        }

        [Obsolete("This may be deleted in the future.")]
        public static void SetupShaderSettingFromMaterial(Material material, ref lilToonSetting shaderSetting)
        {
            SetupShaderSettingFromMaterial(material, ref shaderSetting);
        }

        [Obsolete("This may be deleted in the future.")] public static void ApplyEditorSettingTemp(){}
        [Obsolete("This may be deleted in the future.")] public static void SaveEditorSettingTemp(){}

        private const string WARN_ABOUT_DIRECTORY = "Methods related to directories have been moved to lilDirectoryManager.";
        [Obsolete(WARN_ABOUT_DIRECTORY)] public const string editorSettingTempPath           = lilDirectoryManager.editorSettingTempPath;
        [Obsolete(WARN_ABOUT_DIRECTORY)] public const string versionInfoTempPath             = lilDirectoryManager.versionInfoTempPath;
        [Obsolete(WARN_ABOUT_DIRECTORY)] public const string packageListTempPath             = lilDirectoryManager.packageListTempPath;
        [Obsolete(WARN_ABOUT_DIRECTORY)] public const string postBuildTempPath               = lilDirectoryManager.postBuildTempPath;
        [Obsolete(WARN_ABOUT_DIRECTORY)] public const string startupTempPath                 = lilDirectoryManager.startupTempPath;
        #if NET_4_6
            [Obsolete(WARN_ABOUT_DIRECTORY)] public const string rspPath = "Assets/csc.rsp";
        #else
            [Obsolete(WARN_ABOUT_DIRECTORY)] public const string rspPath = "Assets/mcs.rsp";
        #endif
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static string GetMainFolderPath()            { return lilDirectoryManager.GetMainFolderPath()        ; }
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static string GetEditorFolderPath()          { return lilDirectoryManager.GetEditorFolderPath()      ; }
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static string GetPresetsFolderPath()         { return lilDirectoryManager.GetPresetsFolderPath()     ; }
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static string GetEditorPath()                { return lilDirectoryManager.GetEditorPath()            ; }
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static string GetShaderFolderPath()          { return lilDirectoryManager.GetShaderFolderPath()      ; }
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static string GetShaderPipelinePath()        { return lilDirectoryManager.GetShaderPipelinePath()    ; }
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static string GetShaderCommonPath()          { return lilDirectoryManager.GetShaderCommonPath()      ; }
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static string GetShaderSettingHLSLPath()     { return ""                                             ; }
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static string GetEditorLanguageFileGUID()    { return lilDirectoryManager.GetEditorLanguageFileGUID(); }
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static string GetAvatarEncryptionPath()      { return lilDirectoryManager.GetAvatarEncryptionPath()  ; }
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static string GetGUIBoxInDarkPath()          { return lilDirectoryManager.GetGUIBoxInDarkPath()      ; }
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static string GetGUIBoxInLightPath()         { return lilDirectoryManager.GetGUIBoxInLightPath()     ; }
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static string GetGUIBoxInHalfDarkPath()      { return lilDirectoryManager.GetGUIBoxInHalfDarkPath()  ; }
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static string GetGUIBoxInHalfLightPath()     { return lilDirectoryManager.GetGUIBoxInHalfLightPath() ; }
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static string GetGUIBoxOutDarkPath()         { return lilDirectoryManager.GetGUIBoxOutDarkPath()     ; }
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static string GetGUIBoxOutLightPath()        { return lilDirectoryManager.GetGUIBoxOutLightPath()    ; }
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static string GetGUICustomBoxDarkPath()      { return lilDirectoryManager.GetGUICustomBoxDarkPath()  ; }
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static string GetGUICustomBoxLightPath()     { return lilDirectoryManager.GetGUICustomBoxLightPath() ; }
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static string[] GetShaderFolderPaths()       { return lilDirectoryManager.GetShaderFolderPaths(); }
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static string GetSettingFolderPath()         { return lilDirectoryManager.GetMainFolderPath(); }
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static string GetShaderSettingPath()         { return lilDirectoryManager.GetMainFolderPath() + "/ShaderSetting.asset"; }
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static string GUIDToPath(string GUID)        { return lilDirectoryManager.GUIDToPath(GUID); }
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static bool ExistsEncryption()               { return lilDirectoryManager.ExistsEncryption(); }
        #endregion
    }
}
#endif
