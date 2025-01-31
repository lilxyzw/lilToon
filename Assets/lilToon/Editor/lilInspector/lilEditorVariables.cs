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
        // Editor variables
        public class lilToonEditorSetting : ScriptableSingleton<lilToonEditorSetting>
        {
            public EditorMode editorMode = EditorMode.Simple;
            public bool isShowBase                      = false;
            public bool isShowPrePreset                 = false;
            public bool isShowMainUV                    = false;
            public bool isShowMain                      = false;
            public bool isShowMainTone                  = false;
            public bool isShowShadow                    = false;
            public bool isShowShadowAO                  = false;
            public bool isShowRimShade                  = false;
            public bool isShowBump                      = false;
            public bool isShowReflections               = false;
            public bool isShowEmission                  = false;
            public bool isShowEmissionMap               = false;
            public bool isShowEmissionBlendMask         = false;
            public bool isShowEmission2ndMap            = false;
            public bool isShowEmission2ndBlendMask      = false;
            public bool isShowMatCapUV                  = false;
            public bool isShowMatCap2ndUV               = false;
            public bool isShowParallax                  = false;
            public bool isShowDistanceFade              = false;
            public bool isShowAudioLink                 = false;
            public bool isShowDissolve                  = false;
            public bool isShowMain2ndDissolveMask       = false;
            public bool isShowMain2ndDissolveNoiseMask  = false;
            public bool isShowMain3rdDissolveMask       = false;
            public bool isShowMain3rdDissolveNoiseMask  = false;
            public bool isShowBumpMap                   = false;
            public bool isShowBump2ndMap                = false;
            public bool isShowBump2ndScaleMask          = false;
            public bool isShowAnisotropyTangentMap      = false;
            public bool isShowAnisotropyScaleMask       = false;
            public bool isShowSmoothnessTex             = false;
            public bool isShowMetallicGlossMap          = false;
            public bool isShowBacklight                 = false;
            public bool isShowBacklightColorTex         = false;
            public bool isShowReflectionColorTex        = false;
            public bool isShowMatCap                    = false;
            public bool isShowMatCapBlendMask           = false;
            public bool isShowMatCapBumpMap             = false;
            public bool isShowMatCap2ndBlendMask        = false;
            public bool isShowMatCap2ndBumpMap          = false;
            public bool isShowRim                       = false;
            public bool isShowRimColorTex               = false;
            public bool isShowGlitter                   = false;
            public bool isShowGlitterColorTex           = false;
            public bool isShowGlitterShapeTex           = false;
            public bool isShowGem                       = false;
            public bool isShowAudioLinkMask             = false;
            public bool isShowDissolveMask              = false;
            public bool isShowDissolveNoiseMask         = false;
            public bool isShowIDMask                    = false;
            public bool isShowUDIMDiscard               = false;
            public bool isShowEncryption                = false;
            public bool isShowStencil                   = false;
            public bool isShowOutline                   = false;
            public bool isShowOutlineMap                = false;
            public bool isShowRefraction                = false;
            public bool isShowFur                       = false;
            public bool isShowTess                      = false;
            public bool isShowRendering                 = false;
            public bool isShowLightBake                 = false;
            public bool isShowOptimization              = false;
            public bool isShowBlend                     = false;
            public bool isShowBlendAdd                  = false;
            public bool isShowBlendPre                  = false;
            public bool isShowBlendAddPre               = false;
            public bool isShowBlendOutline              = false;
            public bool isShowBlendAddOutline           = false;
            public bool isShowBlendFur                  = false;
            public bool isShowBlendAddFur               = false;
            public bool isShowWebPages                  = false;
            public bool isShowHelpPages                 = false;
            public bool isShowLightingSettings          = false;
            public bool isShowShaderSetting             = false;
            public bool isShowOptimizationSetting       = false;
            public bool isShowDefaultValueSetting       = false;
            public bool isShowVRChat                    = false;
            public bool isAlphaMaskModeAdvanced         = false;
            public bool[] isShowCategorys = new bool[(int)lilPresetCategory.Other+1]{false,false,false,false,false,false,false};
            public string searchKeyWord = "";
        }

        [Serializable]
        public class lilToonVersion
        {
            public string latest_vertion_name;
            public int latest_vertion_value;
        }

        public struct PropertyBlockData
        {
            public PropertyBlock propertyBlock;
            public bool shouldCopyTex;
        }

        public static lilToonPreset[] presets;
        public static lilToonEditorSetting edSet { get { return lilToonEditorSetting.instance; } }
        protected static MaterialEditor m_MaterialEditor;
        protected static RenderingMode renderingModeBuf;
        protected static TransparentMode transparentModeBuf;
        protected static bool isLite            = false;
        protected static bool isCutout          = false;
        protected static bool isTransparent     = false;
        protected static bool isOutl            = false;
        protected static bool isRefr            = false;
        protected static bool isBlur            = false;
        protected static bool isFur             = false;
        protected static bool isStWr            = false;
        protected static bool isTess            = false;
        protected static bool isGem             = false;
        protected static bool isFakeShadow      = false;
        protected static bool isOnePass         = false;
        protected static bool isTwoPass         = false;
        protected static bool isMulti           = false;
        protected static bool isUseAlpha        = false;
        protected static bool isShowRenderMode  = true;

        private Material[] materials;
        private static lilToonSetting shaderSetting;
        private static readonly lilToonVersion latestVersion = new lilToonVersion{latest_vertion_name = "", latest_vertion_value = 0};
        private static readonly Dictionary<string, MaterialProperty> copiedProperties = new Dictionary<string, MaterialProperty>();
        private static bool isCustomEditor = false;
        private static bool isMultiVariants = false;
        private readonly Gradient mainGrad  = new Gradient();
        private readonly Gradient emiGrad   = new Gradient();
        private readonly Gradient emi2Grad  = new Gradient();

        protected static GUIStyle boxOuter         { get { return lilEditorGUI.boxOuter        ; } private set { lilEditorGUI.boxOuter         = value; }}
        protected static GUIStyle boxInnerHalf     { get { return lilEditorGUI.boxInnerHalf    ; } private set { lilEditorGUI.boxInnerHalf     = value; }}
        protected static GUIStyle boxInner         { get { return lilEditorGUI.boxInner        ; } private set { lilEditorGUI.boxInner         = value; }}
        protected static GUIStyle customBox        { get { return lilEditorGUI.customBox       ; } private set { lilEditorGUI.customBox        = value; }}
        protected static GUIStyle customToggleFont { get { return lilEditorGUI.customToggleFont; } private set { lilEditorGUI.customToggleFont = value; }}
        protected static GUIStyle wrapLabel        { get { return lilEditorGUI.wrapLabel       ; } private set { lilEditorGUI.wrapLabel        = value; }}
        protected static GUIStyle boldLabel        { get { return lilEditorGUI.boldLabel       ; } private set { lilEditorGUI.boldLabel        = value; }}
        protected static GUIStyle foldout          { get { return lilEditorGUI.foldout         ; } private set { lilEditorGUI.foldout          = value; }}
        protected static GUIStyle middleButton     { get { return lilEditorGUI.middleButton    ; } private set { lilEditorGUI.middleButton     = value; }}

        protected static string     sMainColorBranch                { get { return lilLanguageManager.sMainColorBranch              ; } private set { lilLanguageManager.sMainColorBranch               = value; } }
        protected static string     sCullModes                      { get { return lilLanguageManager.sCullModes                    ; } private set { lilLanguageManager.sCullModes                     = value; } }
        protected static string     sBlendModes                     { get { return lilLanguageManager.sBlendModes                   ; } private set { lilLanguageManager.sBlendModes                    = value; } }
        protected static string     sAlphaModes                     { get { return lilLanguageManager.sAlphaModes                   ; } private set { lilLanguageManager.sAlphaModes                    = value; } }
        protected static string     sAlphaMaskModes                 { get { return lilLanguageManager.sAlphaMaskModes               ; } private set { lilLanguageManager.sAlphaMaskModes                = value; } }
        protected static string     blinkSetting                    { get { return lilLanguageManager.blinkSetting                  ; } private set { lilLanguageManager.blinkSetting                   = value; } }
        protected static string     sDistanceFadeSetting            { get { return lilLanguageManager.sDistanceFadeSetting          ; } private set { lilLanguageManager.sDistanceFadeSetting           = value; } }
        protected static string     sDistanceFadeSettingMode        { get { return lilLanguageManager.sDistanceFadeSettingMode      ; } private set { lilLanguageManager.sDistanceFadeSettingMode       = value; } }
        protected static string     sDissolveParams                 { get { return lilLanguageManager.sDissolveParams               ; } private set { lilLanguageManager.sDissolveParams                = value; } }
        protected static string     sDissolveParamsMode             { get { return lilLanguageManager.sDissolveParamsMode           ; } private set { lilLanguageManager.sDissolveParamsMode            = value; } }
        protected static string     sDissolveParamsOther            { get { return lilLanguageManager.sDissolveParamsOther          ; } private set { lilLanguageManager.sDissolveParamsOther           = value; } }
        protected static string     sGlitterParams1                 { get { return lilLanguageManager.sGlitterParams1               ; } private set { lilLanguageManager.sGlitterParams1                = value; } }
        protected static string     sGlitterParams2                 { get { return lilLanguageManager.sGlitterParams2               ; } private set { lilLanguageManager.sGlitterParams2                = value; } }
        protected static string     sTransparentMode                { get { return lilLanguageManager.sTransparentMode              ; } private set { lilLanguageManager.sTransparentMode               = value; } }
        protected static string     sOutlineVertexColorUsages       { get { return lilLanguageManager.sOutlineVertexColorUsages     ; } private set { lilLanguageManager.sOutlineVertexColorUsages      = value; } }
        protected static string     sShadowColorTypes               { get { return lilLanguageManager.sShadowColorTypes             ; } private set { lilLanguageManager.sShadowColorTypes               = value; } }
        protected static string     sShadowMaskTypes                { get { return lilLanguageManager.sShadowMaskTypes              ; } private set { lilLanguageManager.sShadowMaskTypes               = value; } }
        protected static string[]   sRenderingModeList              { get { return lilLanguageManager.sRenderingModeList            ; } private set { lilLanguageManager.sRenderingModeList             = value; } }
        protected static string[]   sRenderingModeListLite          { get { return lilLanguageManager.sRenderingModeListLite        ; } private set { lilLanguageManager.sRenderingModeListLite         = value; } }
        protected static string[]   sTransparentModeList            { get { return lilLanguageManager.sTransparentModeList          ; } private set { lilLanguageManager.sTransparentModeList           = value; } }
        protected static string[]   sBlendModeList                  { get { return lilLanguageManager.sBlendModeList                ; } private set { lilLanguageManager.sBlendModeList                 = value; } }
        protected static GUIContent mainColorRGBAContent            { get { return lilLanguageManager.mainColorRGBAContent          ; } private set { lilLanguageManager.mainColorRGBAContent           = value; } }
        protected static GUIContent colorRGBAContent                { get { return lilLanguageManager.colorRGBAContent              ; } private set { lilLanguageManager.colorRGBAContent               = value; } }
        protected static GUIContent colorAlphaRGBAContent           { get { return lilLanguageManager.colorAlphaRGBAContent         ; } private set { lilLanguageManager.colorAlphaRGBAContent          = value; } }
        protected static GUIContent maskBlendContent                { get { return lilLanguageManager.maskBlendContent              ; } private set { lilLanguageManager.maskBlendContent               = value; } }
        protected static GUIContent maskBlendRGBContent             { get { return lilLanguageManager.maskBlendRGBContent           ; } private set { lilLanguageManager.maskBlendRGBContent            = value; } }
        protected static GUIContent maskBlendRGBAContent            { get { return lilLanguageManager.maskBlendRGBAContent          ; } private set { lilLanguageManager.maskBlendRGBAContent           = value; } }
        protected static GUIContent colorMaskRGBAContent            { get { return lilLanguageManager.colorMaskRGBAContent          ; } private set { lilLanguageManager.colorMaskRGBAContent           = value; } }
        protected static GUIContent alphaMaskContent                { get { return lilLanguageManager.alphaMaskContent              ; } private set { lilLanguageManager.alphaMaskContent               = value; } }
        protected static GUIContent ditherContent                   { get { return lilLanguageManager.ditherContent                 ; } private set { lilLanguageManager.ditherContent                  = value; } }
        protected static GUIContent maskStrengthContent             { get { return lilLanguageManager.maskStrengthContent           ; } private set { lilLanguageManager.maskStrengthContent            = value; } }
        protected static GUIContent normalMapContent                { get { return lilLanguageManager.normalMapContent              ; } private set { lilLanguageManager.normalMapContent               = value; } }
        protected static GUIContent noiseMaskContent                { get { return lilLanguageManager.noiseMaskContent              ; } private set { lilLanguageManager.noiseMaskContent               = value; } }
        protected static GUIContent matcapContent                   { get { return lilLanguageManager.matcapContent                 ; } private set { lilLanguageManager.matcapContent                  = value; } }
        protected static GUIContent gradationContent                { get { return lilLanguageManager.gradationContent              ; } private set { lilLanguageManager.gradationContent               = value; } }
        protected static GUIContent gradSpeedContent                { get { return lilLanguageManager.gradSpeedContent              ; } private set { lilLanguageManager.gradSpeedContent               = value; } }
        protected static GUIContent smoothnessContent               { get { return lilLanguageManager.smoothnessContent             ; } private set { lilLanguageManager.smoothnessContent              = value; } }
        protected static GUIContent metallicContent                 { get { return lilLanguageManager.metallicContent               ; } private set { lilLanguageManager.metallicContent                = value; } }
        protected static GUIContent parallaxContent                 { get { return lilLanguageManager.parallaxContent               ; } private set { lilLanguageManager.parallaxContent                = value; } }
        protected static GUIContent audioLinkMaskContent            { get { return lilLanguageManager.audioLinkMaskContent          ; } private set { lilLanguageManager.audioLinkMaskContent           = value; } }
        protected static GUIContent audioLinkMaskSpectrumContent    { get { return lilLanguageManager.audioLinkMaskSpectrumContent  ; } private set { lilLanguageManager.audioLinkMaskSpectrumContent   = value; } }
        protected static GUIContent customMaskContent               { get { return lilLanguageManager.customMaskContent             ; } private set { lilLanguageManager.customMaskContent              = value; } }
        protected static GUIContent shadow1stColorRGBAContent       { get { return lilLanguageManager.shadow1stColorRGBAContent     ; } private set { lilLanguageManager.shadow1stColorRGBAContent      = value; } }
        protected static GUIContent shadow2ndColorRGBAContent       { get { return lilLanguageManager.shadow2ndColorRGBAContent     ; } private set { lilLanguageManager.shadow2ndColorRGBAContent      = value; } }
        protected static GUIContent shadow3rdColorRGBAContent       { get { return lilLanguageManager.shadow3rdColorRGBAContent     ; } private set { lilLanguageManager.shadow3rdColorRGBAContent      = value; } }
        protected static GUIContent blurMaskRGBContent              { get { return lilLanguageManager.blurMaskRGBContent            ; } private set { lilLanguageManager.blurMaskRGBContent             = value; } }
        protected static GUIContent shadowAOMapContent              { get { return lilLanguageManager.shadowAOMapContent            ; } private set { lilLanguageManager.shadowAOMapContent             = value; } }
        protected static GUIContent widthMaskContent                { get { return lilLanguageManager.widthMaskContent              ; } private set { lilLanguageManager.widthMaskContent               = value; } }
        protected static GUIContent lengthMaskContent               { get { return lilLanguageManager.lengthMaskContent             ; } private set { lilLanguageManager.lengthMaskContent              = value; } }
        protected static GUIContent triMaskContent                  { get { return lilLanguageManager.triMaskContent                ; } private set { lilLanguageManager.triMaskContent                 = value; } }
        protected static GUIContent cubemapContent                  { get { return lilLanguageManager.cubemapContent                ; } private set { lilLanguageManager.cubemapContent                 = value; } }
        protected static GUIContent audioLinkLocalMapContent        { get { return lilLanguageManager.audioLinkLocalMapContent      ; } private set { lilLanguageManager.audioLinkLocalMapContent       = value; } }
        protected static GUIContent gradationMapContent             { get { return lilLanguageManager.gradationMapContent           ; } private set { lilLanguageManager.gradationMapContent            = value; } }
    }
}
#endif
