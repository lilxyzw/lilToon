#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public static string sAlphaModes;
        public static string sAlphaMaskModes;
        public static string blinkSetting;
        public static string sDistanceFadeSetting;
        public static string sDistanceFadeSettingMode;
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
        public static GUIContent ditherContent;
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
        public static GUIContent blurMaskRGBContent;
        public static GUIContent shadowAOMapContent;
        public static GUIContent widthMaskContent;
        public static GUIContent lengthMaskContent;
        public static GUIContent triMaskContent;
        public static GUIContent cubemapContent;
        public static GUIContent audioLinkLocalMapContent;
        public static GUIContent gradationMapContent;
        public static LanguageSettings langSet { get { return LanguageSettings.instance; } }

        public class LanguageSettings : ScriptableSingleton<LanguageSettings>
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

        public static void LoadCustomLanguage(string langFileGUID)
        {
            LoadLanguage(lilDirectoryManager.GUIDToPath(langFileGUID));
        }

        [Obsolete]
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
            #pragma warning disable CS0612
            string langPath = lilDirectoryManager.GetEditorLanguageFileGUID();
            #pragma warning restore CS0612
            LoadLanguage(langPath);
            InitializeLabels();
        }

        public static void SelectLang()
        {
            #pragma warning disable CS0612
            InitializeLanguage();
            #pragma warning restore CS0612
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
                var lineContents = str.Split('\t');
                loc[lineContents[0]] = lineContents[langSet.languageNum+1];
            }
            sr.Close();
        }

        private static void InitializeLabels()
        {
            loc["sCullModes"]                = BuildParams(GetLoc("sCullMode"), GetLoc("sCullModeOff"), GetLoc("sCullModeFront"), GetLoc("sCullModeBack"));
            loc["sBlendModes"]               = BuildParams(GetLoc("sBlendMode"), GetLoc("sBlendModeNormal"), GetLoc("sBlendModeAdd"), GetLoc("sBlendModeScreen"), GetLoc("sBlendModeMul"));
            loc["sAlphaModes"]               = BuildParams(GetLoc("sTransparentMode"), GetLoc("sAlphaMaskModeNone"), GetLoc("sAlphaMaskModeReplace"), GetLoc("sAlphaMaskModeMul"), GetLoc("sAlphaMaskModeAdd"), GetLoc("sAlphaMaskModeSub"));
            loc["sAlphaMaskModes"]           = BuildParams(GetLoc("sAlphaMask"), GetLoc("sAlphaMaskModeNone"), GetLoc("sAlphaMaskModeReplace"), GetLoc("sAlphaMaskModeMul"), GetLoc("sAlphaMaskModeAdd"), GetLoc("sAlphaMaskModeSub"));
            loc["sBlinkSettings"]              = BuildParams(GetLoc("sBlinkStrength"), GetLoc("sBlinkType"), GetLoc("sBlinkSpeed"), GetLoc("sBlinkOffset"));
            loc["sDistanceFadeSettings"]      = BuildParams(GetLoc("sStartDistance"), GetLoc("sEndDistance"), GetLoc("sStrength"), GetLoc("sBackfaceForceShadow"));
            loc["sDistanceFadeModes"]  = BuildParams("Mode", GetLoc("sVertex"), GetLoc("sDissolveModePosition"));
            loc["sDissolveParams"]           = BuildParams(GetLoc("sDissolveMode"), GetLoc("sDissolveModeNone"), GetLoc("sDissolveModeAlpha"), GetLoc("sDissolveModeUV"), GetLoc("sDissolveModePosition"), GetLoc("sDissolveShape"), GetLoc("sDissolveShapePoint"), GetLoc("sDissolveShapeLine"), GetLoc("sBorder"), GetLoc("sBlur"));
            loc["sDissolveParamsModes"]       = BuildParams(GetLoc("sDissolve"), GetLoc("sDissolveModeNone"), GetLoc("sDissolveModeAlpha"), GetLoc("sDissolveModeUV"), GetLoc("sDissolveModePosition"));
            loc["sDissolveParamsOther"]      = BuildParams(GetLoc("sDissolveShape"), GetLoc("sDissolveShapePoint"), GetLoc("sDissolveShapeLine"), GetLoc("sBorder"), GetLoc("sBlur"), "Dummy");
            loc["sGlitterParams2"]           = BuildParams(GetLoc("sBlinkSpeed"), GetLoc("sAngleLimit"), GetLoc("sRimLightDirection"), GetLoc("sColorRandomness"));
            loc["sOutlineVertexColorUsages"] = BuildParams(GetLoc("sVertexColor"), GetLoc("sNone"), GetLoc("sVertexR2Width"), GetLoc("sVertexRGBA2Normal"));
            loc["sShadowColorTypes"]         = BuildParams(GetLoc("sColorType"), GetLoc("sColorTypeNormal"), GetLoc("sColorTypeLUT"));
            loc["sShadowMaskTypes"]          = BuildParams(GetLoc("sMaskType"), GetLoc("sStrength"), GetLoc("sFlat"));
            loc["sHSVGs"]                    = BuildParams(GetLoc("sHue"), GetLoc("sSaturation"), GetLoc("sValue"), GetLoc("sGamma"));
            loc["sScrollRotates"]            = BuildParams(GetLoc("sAngle"), GetLoc("sUVAnimation"), GetLoc("sScroll"), GetLoc("sRotate"));
            loc["sDecalAnimations"]          = BuildParams(GetLoc("sAnimation"), GetLoc("sXFrames"), GetLoc("sYFrames"), GetLoc("sFrames"), GetLoc("sFPS"));
            loc["sDecalSubParams"]           = BuildParams(GetLoc("sXRatio"), GetLoc("sYRatio"), GetLoc("sFixBorder"));
            loc["sAudioLinkUVModes"]           = BuildParams(GetLoc("sAudioLinkUVMode"), GetLoc("sAudioLinkUVModeNone"), GetLoc("sAudioLinkUVModeRim"), GetLoc("sAudioLinkUVModeUV"), GetLoc("sAudioLinkUVModeMask"), GetLoc("sAudioLinkUVModeMask") + " (Spectrum)", GetLoc("sAudioLinkUVModePosition"));
            loc["sAudioLinkVertexUVModes"]           = BuildParams(GetLoc("sAudioLinkUVMode"), GetLoc("sAudioLinkUVModeNone"), GetLoc("sAudioLinkUVModePosition"), GetLoc("sAudioLinkUVModeUV"), GetLoc("sAudioLinkUVModeMask"));
            loc["sAudioLinkVertexStrengths"]           = BuildParams(GetLoc("sAudioLinkMovingVector"), GetLoc("sAudioLinkNormalStrength"));
            loc["sAudioLinkLocalMapParams"]         = BuildParams(GetLoc("sAudioLinkLocalMapBPM"), GetLoc("sAudioLinkLocalMapNotes"), GetLoc("sOffset"));
            loc["sFakeShadowVectors"] = BuildParams(GetLoc("sVector"), GetLoc("sOffset"));
            loc["sFurVectors"] = BuildParams(GetLoc("sVector"), GetLoc("sLength"));
            loc["sPreOutTypes"] = BuildParams(GetLoc("sOutType"), GetLoc("sOutTypeNormal"), GetLoc("sOutTypeFlat"), GetLoc("sOutTypeMono"));
            loc["sLightDirectionOverrides"] = BuildParams(GetLoc("sLightDirectionOverride"), GetLoc("sObjectFollowing"));

            sCullModes                      = BuildParams(GetLoc("sCullMode"), GetLoc("sCullModeOff"), GetLoc("sCullModeFront"), GetLoc("sCullModeBack"));
            sBlendModes                     = BuildParams(GetLoc("sBlendMode"), GetLoc("sBlendModeNormal"), GetLoc("sBlendModeAdd"), GetLoc("sBlendModeScreen"), GetLoc("sBlendModeMul"));
            sAlphaModes                     = BuildParams(GetLoc("sTransparentMode"), GetLoc("sAlphaMaskModeNone"), GetLoc("sAlphaMaskModeReplace"), GetLoc("sAlphaMaskModeMul"), GetLoc("sAlphaMaskModeAdd"), GetLoc("sAlphaMaskModeSub"));
            sAlphaMaskModes                 = BuildParams(GetLoc("sAlphaMask"), GetLoc("sAlphaMaskModeNone"), GetLoc("sAlphaMaskModeReplace"), GetLoc("sAlphaMaskModeMul"), GetLoc("sAlphaMaskModeAdd"), GetLoc("sAlphaMaskModeSub"));
            blinkSetting                    = BuildParams(GetLoc("sBlinkStrength"), GetLoc("sBlinkType"), GetLoc("sBlinkSpeed"), GetLoc("sBlinkOffset"));
            sDistanceFadeSetting            = BuildParams(GetLoc("sStartDistance"), GetLoc("sEndDistance"), GetLoc("sStrength"), GetLoc("sBackfaceForceShadow"));
            sDistanceFadeSettingMode        = BuildParams("Mode", GetLoc("sVertex"), GetLoc("sDissolveModePosition"));
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
            ditherContent                   = new GUIContent(GetLoc("sDither"),                             GetLoc("sAlphaR"));
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
            blurMaskRGBContent              = new GUIContent(GetLoc("sBlurMask"),                           GetLoc("sBlurRGB"));
            shadowAOMapContent              = new GUIContent("AO Map",                                      GetLoc("sBorderRGB"));
            widthMaskContent                = new GUIContent(GetLoc("sWidth"),                              GetLoc("sWidthR"));
            lengthMaskContent               = new GUIContent(GetLoc("sLengthMask"),                         GetLoc("sStrengthR"));
            triMaskContent                  = new GUIContent(GetLoc("sTriMask"),                            GetLoc("sTriMaskRGB"));
            cubemapContent                  = new GUIContent("Cubemap Fallback");
            audioLinkLocalMapContent        = new GUIContent(GetLoc("sAudioLinkLocalMap"));
            gradationMapContent             = new GUIContent(GetLoc("sGradationMap"));

        }

        public static string GetDisplayLabel(MaterialProperty prop)
        {
            var labels = prop.displayName.Split('|').First().Split('+').Select(m=>GetLoc(m)).ToArray();
            if(Event.current.alt) labels[0] = prop.name;
            return string.Join("", labels);
        }

        public static string GetDisplayName(MaterialProperty prop)
        {
            var labels = prop.displayName.Split('|').Select(
                n=>string.Join("",n.Split('+').Select(m=>GetLoc(m)).ToArray())
            ).ToArray();
            if(Event.current.alt)
            {
                if(labels[0].Contains("|")) labels[0] = prop.name + labels[0].Substring(labels[0].IndexOf("|"));
                else labels[0] = prop.name;
            }
            return string.Join("|", labels);
        }

        public static string GetDisplayName(string label)
        {
            return string.Join("|",
                label.Split('|').Select(
                    n=>string.Join("",n.Split('+').Select(m=>GetLoc(m)).ToArray())
                ).ToArray()
            );
        }

        [Obsolete("This may be deleted in the future.")] public static void ApplySettingTemp(){}
        [Obsolete("This may be deleted in the future.")] public static void SaveSettingTemp(){}
    }
}
#endif