# 【Important】 About migrating from version 1.x to 2.x

## Information for general users
- Support for Unity 201**8** will end. It seems that many users are using 2019 or later, so you can continue to use lilToon 2.0 or later.
- **Except for mesh encryption (AvatarEncryption)**, which is rarely used, it can be used as is.
  - If mesh encryption is used, there is no need to re-edit the material as the function will simply be turned off and no material error will occur. However, the mesh cannot be restored using the shader, so please use the mesh before encryption.
- There has been a change in the way to access lilToon's functions, and unsupported tools and custom shaders may cause errors. If an error occurs, please contact the tool developer or delete the tool. It may be easier for developers to fix errors by **including a link to this page in the message**.

## Information for custom shader and tool developers
- **Auto Referenced in lilToon's asmdef has been turned off**, so tools and custom shaders that cause errors need to be addressed. The solution is as follows.
  1. If there is no asmdef in the script that causes the error, add it
  2. Added `lilToon.Editor` to Assembly Definition References in asmdef
- Since we removed the Obsolete part from the C# script, we need to migrate to the modified function. The compatibility table is in [Migration destination for obsolete parts of C# scripts](#migration-destination-for-obsolete-parts-of-c-scripts).
- By using asmdef's Version Defines, you can change the script's behavior depending on the lilToon version. However, **this will not work unless lilToon is under the Packages folder**, so if you do not need to use a preprocessor, you can also get the version of lilToon directly with `lilConstants.currentVersionName`. Parsing of semver can be done with `lilToon.SemVerParser`.
- Mesh encryption (AvatarEncryption) has been removed. Basically, it seems that no tools are affected, but if you are, please fix the error.

## Migration destination for obsolete parts of C# scripts

### lilEnumeration

|Name|Destination|
|-|-|
|public enum BlendMode|Delete|

### lilToonInspector

|Name|Destination|
|-|-|
|protected virtual void DrawCustomProperties(MaterialEditor materialEditor, Material material, GUIStyle boxOuter, GUIStyle boxInnerHalf, GUIStyle boxInner, GUIStyle customBox, GUIStyle customToggleFont, GUIStyle offsetButton)|protected virtual void DrawCustomProperties(Material material)<br>削除された引数は`protected static GUIStyle boxOuter`のような形で残っているためそちらを使用可能です。|
|public static bool EqualsShaderSetting(lilToonSetting ssA, lilToonSetting ssB)|Delete|
|public static void CopyShaderSetting(ref lilToonSetting ssA, lilToonSetting ssB)|Delete|
|public static void SetupShaderSettingFromMaterial(Material material, ref lilToonSetting shaderSetting)|Delete|
|public static void ApplyEditorSettingTemp()|Delete|
|public static void SaveEditorSettingTemp()|Delete|
|public static void InitializeShaderSetting(ref lilToonSetting shaderSetting)|Move to `lilToonSetting`|
|public static void TurnOffAllShaderSetting(ref lilToonSetting shaderSetting)|Move to `lilToonSetting`|
|public static void TurnOnAllShaderSetting(ref lilToonSetting shaderSetting)|Move to `lilToonSetting`|
|public static void ApplyShaderSetting(lilToonSetting shaderSetting, string reportTitle = null)|Move to `lilToonSetting`|
|public static string BuildShaderSettingString(lilToonSetting shaderSetting, bool isFile)|Move to `lilToonSetting`|
|public static string BuildShaderSettingString(bool isFile)|Move to `lilToonSetting`|
|public static void ApplyShaderSettingOptimized()|Move to `lilToonSetting`|
|public static void SetShaderSettingAfterBuild()|Move to `lilToonSetting`|
|public static void ApplyPreset(Material material, lilToonPreset preset)|Move to `lilToonPreset`|
|public static void ApplyPreset(Material material, lilToonPreset preset, bool ismulti)|Move to `lilToonPreset`|
|public static void LoadTexture(ref Texture2D tex, string path)|Move to `lilTextureUtils`|
|public static string SavePng(string path, string add, Texture2D tex)|lilTextureUtils.SaveTextureToPng(string path, string add, Texture2D tex)|
|public static string ConvertGifToAtlas(Object tex)|Move to `lilTextureUtils`|
|public static string ConvertGifToAtlas(Object tex, out int frameCount, out int loopXY, out int duration, out float xScale, out float yScale)|`lilTextureUtils`|
|public static void ConvertGifToAtlas(MaterialProperty tex, MaterialProperty decalAnimation, MaterialProperty decalSubParam, MaterialProperty isDecal)|`lilEditorGUI`|
|public static void InitializeLanguage()|Move to `lilLanguageManager`|
|public static GUIStyle InitializeBox(int border, int margin, int padding)|Move to `lilEditorGUI`|
|public static void DrawWebButton(string text, string URL)|Move to `lilEditorGUI`|
|public static void DrawSimpleFoldout(string label, ref bool condition, GUIStyle style, bool isCustomEditor = true)|Move to `lilEditorGUI`|
|public static void DrawSimpleFoldout(string label, ref bool condition, bool isCustomEditor = true)|Move to `lilEditorGUI`|
|public static void DrawSimpleFoldout(GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, ref bool condition, bool isCustomEditor = true)|`lilEditorGUI`|
|public static void DrawSimpleFoldout(GUIContent guiContent, MaterialProperty textureName, ref bool condition, bool isCustomEditor = true)|`lilEditorGUI`|
|public static void InitializeShaders()|Move to `lilShaderManager`|
|public static bool CheckMainTextureName(string name)|Move to `lilMaterialUtils`|
|public static void RemoveUnusedTexture(Material material)|Move to `lilMaterialUtils`|
|public const string editorSettingTempPath|Move to `lilDirectoryManager`|
|public const string versionInfoTempPath|Move to `lilDirectoryManager`|
|public const string packageListTempPath|Move to `lilDirectoryManager`|
|public const string postBuildTempPath|Move to `lilDirectoryManager`|
|public const string startupTempPath|Move to `lilDirectoryManager`|
|public const string rspPath|Move to `lilDirectoryManager`|
|public static string GetMainFolderPath()|Move to `lilDirectoryManager`|
|public static string GetEditorFolderPath()|Move to `lilDirectoryManager`|
|public static string GetPresetsFolderPath()|Move to `lilDirectoryManager`|
|public static string GetEditorPath()|Move to `lilDirectoryManager`|
|public static string GetShaderFolderPath()|Move to `lilDirectoryManager`|
|public static string GetShaderPipelinePath()|Move to `lilDirectoryManager`|
|public static string GetShaderCommonPath()|Move to `lilDirectoryManager`|
|public static string GetShaderSettingHLSLPath()|Move to `lilDirectoryManager`|
|public static string GetEditorLanguageFileGUID()|Move to `lilDirectoryManager`|
|public static string GetGUIBoxInDarkPath()|Move to `lilDirectoryManager`|
|public static string GetGUIBoxInLightPath()|Move to `lilDirectoryManager`|
|public static string GetGUIBoxInHalfDarkPath()|Move to `lilDirectoryManager`|
|public static string GetGUIBoxInHalfLightPath()|Move to `lilDirectoryManager`|
|public static string GetGUIBoxOutDarkPath()|Move to `lilDirectoryManager`|
|public static string GetGUIBoxOutLightPath()|Move to `lilDirectoryManager`|
|public static string GetGUICustomBoxDarkPath()|Move to `lilDirectoryManager`|
|public static string GetGUICustomBoxLightPath()|Move to `lilDirectoryManager`|
|public static string[] GetShaderFolderPaths()|Move to `lilDirectoryManager`|
|public static string GetSettingFolderPath()|Move to `lilDirectoryManager`|
|public static string GetShaderSettingPath()|Move to `lilDirectoryManager`|
|public static string GUIDToPath(string GUID)|Move to `lilDirectoryManager`|
|public static string GetAvatarEncryptionPath()|Delete|
|public static bool ExistsEncryption()|Delete|

### lilLanguageManager
|Name|Destination|
|-|-|
|public static void ApplySettingTemp()|Delete|
|public static void SaveSettingTemp()|Delete|
