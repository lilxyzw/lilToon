using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace lilToon
{
    internal partial class L10n : ScriptableSingleton<L10n>
    {
        public LocalizationAsset localizationAssetFallback;
        public LocalizationAsset localizationAsset;
        private static string[] languages;
        private static string[] languageNames;
        private static readonly Dictionary<string, GUIContent> guicontents = new();
        private static string localizationFolder => AssetDatabase.GUIDToAssetPath("2feb2bcbf5b4ef043910b310c21b6ba7");

        internal static void Load()
        {
            guicontents.Clear();
            var path = localizationFolder + "/" + Settings.instance.language + ".po";
            if(File.Exists(path)) instance.localizationAsset = AssetDatabase.LoadAssetAtPath<LocalizationAsset>(path);

            if(!instance.localizationAssetFallback) instance.localizationAssetFallback = AssetDatabase.LoadAssetAtPath<LocalizationAsset>(localizationFolder + "/en-US.po");
            if(!instance.localizationAsset) instance.localizationAsset = new LocalizationAsset();
        }

        internal static string[] GetLanguages()
        {
            return languages ??= Directory.GetFiles(localizationFolder, "*.po").Where(f => !f.StartsWith("._")).Select(f => Path.GetFileNameWithoutExtension(f)).ToArray();
        }

        internal static string[] GetLanguageNames()
        {
            return languageNames ??= languages.Select(l => {
                if(l == "zh-Hans") return "简体中文";
                if(l == "zh-Hant") return "繁體中文";
                return new CultureInfo(l).NativeName;
            }).ToArray();
        }

        internal static string L(string key)
        {
            if (!instance.localizationAsset || !instance.localizationAssetFallback) Load();
            var localized = instance.localizationAsset.GetLocalizedString(key);
            return localized != key ? localized : instance.localizationAssetFallback.GetLocalizedString(key);
        }

        internal static GUIContent G(string key) => G(key, null, "");
        private static GUIContent G(string[] key) => key.Length == 2 ? G(key[0], null, key[1]) : G(key[0], null, null);
        internal static GUIContent G(string key, string tooltip) => G(key, null, tooltip); // From EditorToolboxSettings
        private static GUIContent G(string key, Texture image) => G(key, image, "");

        private static GUIContent G(string key, Texture image, string tooltip)
        {
            if (!instance.localizationAsset || !instance.localizationAssetFallback) Load();
            if (guicontents.TryGetValue(key, out var content)) return content;
            return guicontents[key] = new GUIContent(L(key), image, L(tooltip));
        }
    }
}
