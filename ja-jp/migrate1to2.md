# 【重要】バージョン1.xから2.xへの移行について

## 一般ユーザー向け情報
- Unity 201**8**のサポートが終了します。多くのユーザーは2019以降を利用していると思われるのでlilToon 2.0以降も引き続き利用できます。
- ほとんど使われていなかった**メッシュ暗号化（AvatarEncryption）以外はそのまま使用可能**です。
  - メッシュの暗号化を使用していた場合、機能がそのままオフになるだけでマテリアルエラーにはならないためマテリアルの再編集は不要です。ただしシェーダーでのメッシュの復元ができなくなるため暗号化前のメッシュを使用するようにしてください。
- lilToon本体の機能へのアクセス方法に変更があり、対応していない周辺ツールやカスタムシェーダーがエラーになる場合があります。エラーになった場合はツールの制作者の方に連絡をするか該当ツールを削除してください。**問い合わせの際はこのページへのリンクを貼っておくと制作者の方も対応しやすい**と思われます。

## カスタムシェーダーおよび周辺ツール開発者向け情報
- lilToon本体の**asmdefのAuto Referencedがオフになった**ため、エラーになるツールやカスタムシェーダーは対応が必要です。対応方法は以下の通りです。
  1. エラーになるスクリプトにasmdefがなければ追加
  2. asmdefのAssembly Definition Referencesに`lilToon.Editor`を追加
- C#スクリプトからObsoleteになっていた部分を削除したため、変更後の関数に移行する必要があります。対応表は[C#スクリプトの廃止（Obsolete）した部分及び移行先](#cスクリプトの廃止obsoleteした部分及び移行先)にあります。
- asmdefのVersion Definesを使用することでlilToonのバージョンに応じてスクリプトの動作を変更できます。ただしこちらは**lilToonがPackagesフォルダ配下にないと動作しない**ため、プリプロセッサを使用する必要がない場合は`lilConstants.currentVersionName`で直接lilToonのバージョンを取得して対処するのも手です。semverのパースは`lilToon.SemVerParser`で行えます。
- メッシュ暗号化（AvatarEncryption）が削除されました。基本的に影響を受けるツールは無いと思われますが、もし影響を受けた場合は該当部分を削除するなどの対応を行ってください。

## C#スクリプトの廃止（Obsolete）した部分及び移行先

### lilEnumeration

|名前|移行先|
|-|-|
|public enum BlendMode|削除|

### lilToonInspector

|名前|移行先|
|-|-|
|protected virtual void DrawCustomProperties(MaterialEditor materialEditor, Material material, GUIStyle boxOuter, GUIStyle boxInnerHalf, GUIStyle boxInner, GUIStyle customBox, GUIStyle customToggleFont, GUIStyle offsetButton)|protected virtual void DrawCustomProperties(Material material)<br>削除された引数は`protected static GUIStyle boxOuter`のような形で残っているためそちらを使用可能です。|
|public static bool EqualsShaderSetting(lilToonSetting ssA, lilToonSetting ssB)|削除|
|public static void CopyShaderSetting(ref lilToonSetting ssA, lilToonSetting ssB)|削除|
|public static void SetupShaderSettingFromMaterial(Material material, ref lilToonSetting shaderSetting)|削除|
|public static void ApplyEditorSettingTemp()|削除|
|public static void SaveEditorSettingTemp()|削除|
|public static void InitializeShaderSetting(ref lilToonSetting shaderSetting)|`lilToonSetting`内に同名の関数あり|
|public static void TurnOffAllShaderSetting(ref lilToonSetting shaderSetting)|`lilToonSetting`内に同名の関数あり|
|public static void TurnOnAllShaderSetting(ref lilToonSetting shaderSetting)|`lilToonSetting`内に同名の関数あり|
|public static void ApplyShaderSetting(lilToonSetting shaderSetting, string reportTitle = null)|`lilToonSetting`内に同名の関数あり|
|public static string BuildShaderSettingString(lilToonSetting shaderSetting, bool isFile)|`lilToonSetting`内に同名の関数あり|
|public static string BuildShaderSettingString(bool isFile)|`lilToonSetting`内に同名の関数あり|
|public static void ApplyShaderSettingOptimized()|`lilToonSetting`内に同名の関数あり|
|public static void SetShaderSettingAfterBuild()|`lilToonSetting`内に同名の関数あり|
|public static void ApplyPreset(Material material, lilToonPreset preset)|`lilToonPreset`内に同名の関数あり|
|public static void ApplyPreset(Material material, lilToonPreset preset, bool ismulti)|`lilToonPreset`内に同名の関数あり|
|public static void LoadTexture(ref Texture2D tex, string path)|`lilTextureUtils`内に同名の関数あり|
|public static string SavePng(string path, string add, Texture2D tex)|lilTextureUtils.SaveTextureToPng(string path, string add, Texture2D tex)|
|public static string ConvertGifToAtlas(Object tex)|`lilTextureUtils`内に同名の関数あり|
|public static string ConvertGifToAtlas(Object tex, out int frameCount, out int loopXY, out int duration, out float xScale, out float yScale)|`lilTextureUtils`内に同名の関数あり|
|public static void ConvertGifToAtlas(MaterialProperty tex, MaterialProperty decalAnimation, MaterialProperty decalSubParam, MaterialProperty isDecal)|`lilEditorGUI`内に同名の関数あり|
|public static void InitializeLanguage()|`lilLanguageManager`内に同名の関数あり|
|public static GUIStyle InitializeBox(int border, int margin, int padding)|`lilEditorGUI`内に同名の関数あり|
|public static void DrawWebButton(string text, string URL)|`lilEditorGUI`内に同名の関数あり|
|public static void DrawSimpleFoldout(string label, ref bool condition, GUIStyle style, bool isCustomEditor = true)|`lilEditorGUI`内に同名の関数あり|
|public static void DrawSimpleFoldout(string label, ref bool condition, bool isCustomEditor = true)|`lilEditorGUI`内に同名の関数あり|
|public static void DrawSimpleFoldout(GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, ref bool condition, bool isCustomEditor = true)|`lilEditorGUI`内に同名の関数あり|
|public static void DrawSimpleFoldout(GUIContent guiContent, MaterialProperty textureName, ref bool condition, bool isCustomEditor = true)|`lilEditorGUI`内に同名の関数あり|
|public static void InitializeShaders()|`lilShaderManager`内に同名の関数あり|
|public static bool CheckMainTextureName(string name)|`lilMaterialUtils`内に同名の関数あり|
|public static void RemoveUnusedTexture(Material material)|`lilMaterialUtils`内に同名の関数あり|
|public const string editorSettingTempPath|`lilDirectoryManager`内に同名の変数あり|
|public const string versionInfoTempPath|`lilDirectoryManager`内に同名の変数あり|
|public const string packageListTempPath|`lilDirectoryManager`内に同名の変数あり|
|public const string postBuildTempPath|`lilDirectoryManager`内に同名の変数あり|
|public const string startupTempPath|`lilDirectoryManager`内に同名の変数あり|
|public const string rspPath|`lilDirectoryManager`内に同名の変数あり|
|public static string GetMainFolderPath()|`lilDirectoryManager`内に同名の関数あり|
|public static string GetEditorFolderPath()|`lilDirectoryManager`内に同名の関数あり|
|public static string GetPresetsFolderPath()|`lilDirectoryManager`内に同名の関数あり|
|public static string GetEditorPath()|`lilDirectoryManager`内に同名の関数あり|
|public static string GetShaderFolderPath()|`lilDirectoryManager`内に同名の関数あり|
|public static string GetShaderPipelinePath()|`lilDirectoryManager`内に同名の関数あり|
|public static string GetShaderCommonPath()|`lilDirectoryManager`内に同名の関数あり|
|public static string GetShaderSettingHLSLPath()|`lilDirectoryManager`内に同名の関数あり|
|public static string GetEditorLanguageFileGUID()|`lilDirectoryManager`内に同名の関数あり|
|public static string GetGUIBoxInDarkPath()|`lilDirectoryManager`内に同名の関数あり|
|public static string GetGUIBoxInLightPath()|`lilDirectoryManager`内に同名の関数あり|
|public static string GetGUIBoxInHalfDarkPath()|`lilDirectoryManager`内に同名の関数あり|
|public static string GetGUIBoxInHalfLightPath()|`lilDirectoryManager`内に同名の関数あり|
|public static string GetGUIBoxOutDarkPath()|`lilDirectoryManager`内に同名の関数あり|
|public static string GetGUIBoxOutLightPath()|`lilDirectoryManager`内に同名の関数あり|
|public static string GetGUICustomBoxDarkPath()|`lilDirectoryManager`内に同名の関数あり|
|public static string GetGUICustomBoxLightPath()|`lilDirectoryManager`内に同名の関数あり|
|public static string[] GetShaderFolderPaths()|`lilDirectoryManager`内に同名の関数あり|
|public static string GetSettingFolderPath()|`lilDirectoryManager`内に同名の関数あり|
|public static string GetShaderSettingPath()|`lilDirectoryManager`内に同名の関数あり|
|public static string GUIDToPath(string GUID)|`lilDirectoryManager`内に同名の関数あり|
|public static string GetAvatarEncryptionPath()|削除|
|public static bool ExistsEncryption()|削除|

### lilLanguageManager
|名前|移行先|
|-|-|
|public static void ApplySettingTemp()|削除|
|public static void SaveSettingTemp()|削除|
