#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace lilToon
{
    public class lilLanguageManager
    {
        private static readonly Dictionary<string, string> loc = new Dictionary<string, string>();

        public static string sMainColorBranch;
        public static string sCullModes;
        public static string sBlendModes;
        public static string sAlphaMaskModes;
        public static string blinkSetting;
        public static string sDistanceFadeSetting;
        public static string sDissolveParams;
        public static string sDissolveParamsMode;
        public static string sDissolveParamsOther;
        public static string sGlitterParams1;
        public static string sGlitterParams2;
        public static string sTransparentMode;
        public static string sOutlineVertexColorUsages;
        public static string sShadowColorTypes;
        public static string sShadowMaskTypes;
        public static string[] sRenderingModeList;
        public static string[] sRenderingModeListLite;
        public static string[] sTransparentModeList;
        public static string[] sBlendModeList;
        public static GUIContent mainColorRGBAContent;
        public static GUIContent colorRGBAContent;
        public static GUIContent colorAlphaRGBAContent;
        public static GUIContent maskBlendContent;
        public static GUIContent maskBlendRGBContent;
        public static GUIContent maskBlendRGBAContent;
        public static GUIContent colorMaskRGBAContent;
        public static GUIContent alphaMaskContent;
        public static GUIContent maskStrengthContent;
        public static GUIContent normalMapContent;
        public static GUIContent noiseMaskContent;
        public static GUIContent adjustMaskContent;
        public static GUIContent matcapContent;
        public static GUIContent gradationContent;
        public static GUIContent gradSpeedContent;
        public static GUIContent smoothnessContent;
        public static GUIContent metallicContent;
        public static GUIContent parallaxContent;
        public static GUIContent audioLinkMaskContent;
        public static GUIContent audioLinkMaskSpectrumContent;
        public static GUIContent customMaskContent;
        public static GUIContent shadow1stColorRGBAContent;
        public static GUIContent shadow2ndColorRGBAContent;
        public static GUIContent shadow3rdColorRGBAContent;
        public static LanguageSettings langSet = new LanguageSettings();

        [Serializable]
        public class LanguageSettings
        {
            public int languageNum = -1;
            public string languageNames = "";
            public string languageName = "English";
        }

        public static string GetLoc(string value) { return loc.ContainsKey(value) ? loc[value] : value; }
        public static string BuildParams(params string[] labels) { return string.Join("|", labels); }

        public static bool ShouldApplyTemp()
        {
            return string.IsNullOrEmpty(langSet.languageNames);
        }

        public static void ApplySettingTemp()
        {
            if(!ShouldApplyTemp() || !File.Exists(lilDirectoryManager.languageSettingTempPath)) return;
            StreamReader sr = new StreamReader(lilDirectoryManager.languageSettingTempPath);
            string s = sr.ReadToEnd();
            sr.Close();
            if(!string.IsNullOrEmpty(s)) EditorJsonUtility.FromJsonOverwrite(s,langSet);
        }

        public static void SaveSettingTemp()
        {
            StreamWriter sw = new StreamWriter(lilDirectoryManager.languageSettingTempPath,false);
            sw.Write(EditorJsonUtility.ToJson(langSet));
            sw.Close();
        }

        public static void LoadCustomLanguage(string langFileGUID)
        {
            LoadLanguage(lilDirectoryManager.GUIDToPath(langFileGUID));
        }

        public static void InitializeLanguage()
        {
            if(langSet.languageNum == -1)
            {
                if(Application.systemLanguage == SystemLanguage.Japanese)                   langSet.languageNum = 1;
                else if(Application.systemLanguage == SystemLanguage.Korean)                langSet.languageNum = 2;
                else if(Application.systemLanguage == SystemLanguage.ChineseSimplified)     langSet.languageNum = 3;
                else if(Application.systemLanguage == SystemLanguage.ChineseTraditional)    langSet.languageNum = 4;
                else                                                                        langSet.languageNum = 0;
            }

            if(loc.Count == 0)
            {
                UpdateLanguage();
            }
        }

        public static void UpdateLanguage()
        {
            string langPath = lilDirectoryManager.GetEditorLanguageFileGUID();
            LoadLanguage(langPath);
            InitializeLabels();
        }

        public static void SelectLang()
        {
            InitializeLanguage();
            int numbuf = langSet.languageNum;
            langSet.languageNum = EditorGUILayout.Popup("Language", langSet.languageNum, langSet.languageNames.Split('\t'));
            if(numbuf != langSet.languageNum) UpdateLanguage();
            if(!string.IsNullOrEmpty(GetLoc("sLanguageWarning"))) EditorGUILayout.HelpBox(GetLoc("sLanguageWarning"),MessageType.Warning);
        }

        private static void LoadLanguage(string langPath)
        {
            if(string.IsNullOrEmpty(langPath) || !File.Exists(langPath)) return;
            StreamReader sr = new StreamReader(langPath);

            string str = sr.ReadLine();
            langSet.languageNames = str.Substring(str.IndexOf("\t")+1);
            langSet.languageName = langSet.languageNames.Split('\t')[langSet.languageNum];
            while((str = sr.ReadLine()) != null)
            {
                string[] lineContents = str.Split('\t');
                loc[lineContents[0]] = lineContents[langSet.languageNum+1];
            }
            sr.Close();
        }

        private static void InitializeLabels()
        {
            sCullModes                      = BuildParams(GetLoc("sCullMode"), GetLoc("sCullModeOff"), GetLoc("sCullModeFront"), GetLoc("sCullModeBack"));
            sBlendModes                     = BuildParams(GetLoc("sBlendMode"), GetLoc("sBlendModeNormal"), GetLoc("sBlendModeAdd"), GetLoc("sBlendModeScreen"), GetLoc("sBlendModeMul"));
            sAlphaMaskModes                 = BuildParams(GetLoc("sAlphaMask"), GetLoc("sAlphaMaskModeNone"), GetLoc("sAlphaMaskModeReplace"), GetLoc("sAlphaMaskModeMul"), GetLoc("sAlphaMaskModeAdd"), GetLoc("sAlphaMaskModeSub"));
            blinkSetting                    = BuildParams(GetLoc("sBlinkStrength"), GetLoc("sBlinkType"), GetLoc("sBlinkSpeed"), GetLoc("sBlinkOffset"));
            sDistanceFadeSetting            = BuildParams(GetLoc("sStartDistance"), GetLoc("sEndDistance"), GetLoc("sStrength"), GetLoc("sBackfaceForceShadow"));
            sDissolveParams                 = BuildParams(GetLoc("sDissolveMode"), GetLoc("sDissolveModeNone"), GetLoc("sDissolveModeAlpha"), GetLoc("sDissolveModeUV"), GetLoc("sDissolveModePosition"), GetLoc("sDissolveShape"), GetLoc("sDissolveShapePoint"), GetLoc("sDissolveShapeLine"), GetLoc("sBorder"), GetLoc("sBlur"));
            sDissolveParamsMode             = BuildParams(GetLoc("sDissolve"), GetLoc("sDissolveModeNone"), GetLoc("sDissolveModeAlpha"), GetLoc("sDissolveModeUV"), GetLoc("sDissolveModePosition"));
            sDissolveParamsOther            = BuildParams(GetLoc("sDissolveShape"), GetLoc("sDissolveShapePoint"), GetLoc("sDissolveShapeLine"), GetLoc("sBorder"), GetLoc("sBlur"), "Dummy");
            sGlitterParams1                 = BuildParams("Tiling", GetLoc("sParticleSize"), GetLoc("sContrast"));
            sGlitterParams2                 = BuildParams(GetLoc("sBlinkSpeed"), GetLoc("sAngleLimit"), GetLoc("sRimLightDirection"), GetLoc("sColorRandomness"));
            sTransparentMode                = BuildParams(GetLoc("sRenderingMode"), GetLoc("sRenderingModeOpaque"), GetLoc("sRenderingModeCutout"), GetLoc("sRenderingModeTransparent"), GetLoc("sRenderingModeRefraction"), GetLoc("sRenderingModeFur"), GetLoc("sRenderingModeFurCutout"), GetLoc("sRenderingModeGem"));
            sRenderingModeList              = new[]{GetLoc("sRenderingModeOpaque"), GetLoc("sRenderingModeCutout"), GetLoc("sRenderingModeTransparent"), GetLoc("sRenderingModeRefraction"), GetLoc("sRenderingModeRefractionBlur"), GetLoc("sRenderingModeFur"), GetLoc("sRenderingModeFurCutout"), GetLoc("sRenderingModeFurTwoPass"), GetLoc("sRenderingModeGem")};
            sRenderingModeListLite          = new[]{GetLoc("sRenderingModeOpaque"), GetLoc("sRenderingModeCutout"), GetLoc("sRenderingModeTransparent")};
            sTransparentModeList            = new[]{GetLoc("sTransparentModeNormal"), GetLoc("sTransparentModeOnePass"), GetLoc("sTransparentModeTwoPass")};
            sBlendModeList                  = new[]{GetLoc("sBlendModeNormal"), GetLoc("sBlendModeAdd"), GetLoc("sBlendModeScreen"), GetLoc("sBlendModeMul")};
            sOutlineVertexColorUsages       = BuildParams(GetLoc("sVertexColor"), GetLoc("sNone"), GetLoc("sVertexR2Width"), GetLoc("sVertexRGBA2Normal"));
            sShadowColorTypes               = BuildParams(GetLoc("sColorType"), GetLoc("sColorTypeNormal"), GetLoc("sColorTypeLUT"));
            sShadowMaskTypes                = BuildParams(GetLoc("sMaskType"), GetLoc("sStrength"), GetLoc("sFlat"));
            colorRGBAContent                = new GUIContent(GetLoc("sColor"),                              GetLoc("sTextureRGBA"));
            colorAlphaRGBAContent           = new GUIContent(GetLoc("sColorAlpha"),                         GetLoc("sTextureRGBA"));
            maskBlendContent                = new GUIContent(GetLoc("sMask"),                               GetLoc("sBlendR"));
            maskBlendRGBContent             = new GUIContent(GetLoc("sMask"),                               GetLoc("sTextureRGB"));
            maskBlendRGBAContent            = new GUIContent(GetLoc("sMask"),                               GetLoc("sTextureRGBA"));
            colorMaskRGBAContent            = new GUIContent(GetLoc("sColor") + " / " + GetLoc("sMask"),    GetLoc("sTextureRGBA"));
            alphaMaskContent                = new GUIContent(GetLoc("sAlphaMask"),                          GetLoc("sAlphaR"));
            maskStrengthContent             = new GUIContent(GetLoc("sStrengthMask"),                       GetLoc("sStrengthR"));
            normalMapContent                = new GUIContent(GetLoc("sNormalMap"),                          GetLoc("sNormalRGB"));
            noiseMaskContent                = new GUIContent(GetLoc("sNoise"),                              GetLoc("sNoiseR"));
            matcapContent                   = new GUIContent(GetLoc("sMatCap"),                             GetLoc("sTextureRGBA"));
            gradationContent                = new GUIContent(GetLoc("sGradation"),                          GetLoc("sTextureRGBA"));
            gradSpeedContent                = new GUIContent(GetLoc("sGradTexSpeed"),                       GetLoc("sTextureRGBA"));
            smoothnessContent               = new GUIContent(GetLoc("sSmoothness"),                         GetLoc("sSmoothnessR"));
            metallicContent                 = new GUIContent(GetLoc("sMetallic"),                           GetLoc("sMetallicR"));
            parallaxContent                 = new GUIContent(GetLoc("sParallax"),                           GetLoc("sParallaxR"));
            audioLinkMaskContent            = new GUIContent(GetLoc("sMask"),                               GetLoc("sAudioLinkMaskRGB"));
            audioLinkMaskSpectrumContent    = new GUIContent(GetLoc("sMask"),                               GetLoc("sAudioLinkMaskRGBSpectrum"));
            customMaskContent               = new GUIContent(GetLoc("sMask"),                               "");
            shadow1stColorRGBAContent       = new GUIContent(GetLoc("sShadow1stColor"),                     GetLoc("sTextureRGBA"));
            shadow2ndColorRGBAContent       = new GUIContent(GetLoc("sShadow2ndColor"),                     GetLoc("sTextureRGBA"));
            shadow3rdColorRGBAContent       = new GUIContent(GetLoc("sShadow3rdColor"),                     GetLoc("sTextureRGBA"));
        }
    }
}
#endif