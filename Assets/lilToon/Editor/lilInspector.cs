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
    //------------------------------------------------------------------------------------------------------------------------------
    // ShaderGUI
    public class lilToonInspector : ShaderGUI
    {
        //------------------------------------------------------------------------------------------------------------------------------
        // Custom properties
        // If there are properties you have added, add them here.
        protected static bool isCustomShader = false;

        protected virtual void LoadCustomProperties(MaterialProperty[] props, Material material)
        {
        }

        protected virtual void DrawCustomProperties(Material material)
        {
            #pragma warning disable 0618
            DrawCustomProperties(m_MaterialEditor, material, boxOuter, boxInnerHalf, boxInner, customBox, customToggleFont, GUI.skin.button);
            #pragma warning restore 0618
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Shader variables
        #region
        protected static Shader lts         { get { return lilShaderManager.lts       ; } set { lilShaderManager.lts        = value; } }
        protected static Shader ltsc        { get { return lilShaderManager.ltsc      ; } set { lilShaderManager.ltsc       = value; } }
        protected static Shader ltst        { get { return lilShaderManager.ltst      ; } set { lilShaderManager.ltst       = value; } }
        protected static Shader ltsot       { get { return lilShaderManager.ltsot     ; } set { lilShaderManager.ltsot      = value; } }
        protected static Shader ltstt       { get { return lilShaderManager.ltstt     ; } set { lilShaderManager.ltstt      = value; } }
        protected static Shader ltso        { get { return lilShaderManager.ltso      ; } set { lilShaderManager.ltso       = value; } }
        protected static Shader ltsco       { get { return lilShaderManager.ltsco     ; } set { lilShaderManager.ltsco      = value; } }
        protected static Shader ltsto       { get { return lilShaderManager.ltsto     ; } set { lilShaderManager.ltsto      = value; } }
        protected static Shader ltsoto      { get { return lilShaderManager.ltsoto    ; } set { lilShaderManager.ltsoto     = value; } }
        protected static Shader ltstto      { get { return lilShaderManager.ltstto    ; } set { lilShaderManager.ltstto     = value; } }
        protected static Shader ltsoo       { get { return lilShaderManager.ltsoo     ; } set { lilShaderManager.ltsoo      = value; } }
        protected static Shader ltscoo      { get { return lilShaderManager.ltscoo    ; } set { lilShaderManager.ltscoo     = value; } }
        protected static Shader ltstoo      { get { return lilShaderManager.ltstoo    ; } set { lilShaderManager.ltstoo     = value; } }
        protected static Shader ltstess     { get { return lilShaderManager.ltstess   ; } set { lilShaderManager.ltstess    = value; } }
        protected static Shader ltstessc    { get { return lilShaderManager.ltstessc  ; } set { lilShaderManager.ltstessc   = value; } }
        protected static Shader ltstesst    { get { return lilShaderManager.ltstesst  ; } set { lilShaderManager.ltstesst   = value; } }
        protected static Shader ltstessot   { get { return lilShaderManager.ltstessot ; } set { lilShaderManager.ltstessot  = value; } }
        protected static Shader ltstesstt   { get { return lilShaderManager.ltstesstt ; } set { lilShaderManager.ltstesstt  = value; } }
        protected static Shader ltstesso    { get { return lilShaderManager.ltstesso  ; } set { lilShaderManager.ltstesso   = value; } }
        protected static Shader ltstessco   { get { return lilShaderManager.ltstessco ; } set { lilShaderManager.ltstessco  = value; } }
        protected static Shader ltstessto   { get { return lilShaderManager.ltstessto ; } set { lilShaderManager.ltstessto  = value; } }
        protected static Shader ltstessoto  { get { return lilShaderManager.ltstessoto; } set { lilShaderManager.ltstessoto = value; } }
        protected static Shader ltstesstto  { get { return lilShaderManager.ltstesstto; } set { lilShaderManager.ltstesstto = value; } }
        protected static Shader ltsl        { get { return lilShaderManager.ltsl      ; } set { lilShaderManager.ltsl       = value; } }
        protected static Shader ltslc       { get { return lilShaderManager.ltslc     ; } set { lilShaderManager.ltslc      = value; } }
        protected static Shader ltslt       { get { return lilShaderManager.ltslt     ; } set { lilShaderManager.ltslt      = value; } }
        protected static Shader ltslot      { get { return lilShaderManager.ltslot    ; } set { lilShaderManager.ltslot     = value; } }
        protected static Shader ltsltt      { get { return lilShaderManager.ltsltt    ; } set { lilShaderManager.ltsltt     = value; } }
        protected static Shader ltslo       { get { return lilShaderManager.ltslo     ; } set { lilShaderManager.ltslo      = value; } }
        protected static Shader ltslco      { get { return lilShaderManager.ltslco    ; } set { lilShaderManager.ltslco     = value; } }
        protected static Shader ltslto      { get { return lilShaderManager.ltslto    ; } set { lilShaderManager.ltslto     = value; } }
        protected static Shader ltsloto     { get { return lilShaderManager.ltsloto   ; } set { lilShaderManager.ltsloto    = value; } }
        protected static Shader ltsltto     { get { return lilShaderManager.ltsltto   ; } set { lilShaderManager.ltsltto    = value; } }
        protected static Shader ltsref      { get { return lilShaderManager.ltsref    ; } set { lilShaderManager.ltsref     = value; } }
        protected static Shader ltsrefb     { get { return lilShaderManager.ltsrefb   ; } set { lilShaderManager.ltsrefb    = value; } }
        protected static Shader ltsfur      { get { return lilShaderManager.ltsfur    ; } set { lilShaderManager.ltsfur     = value; } }
        protected static Shader ltsfurc     { get { return lilShaderManager.ltsfurc   ; } set { lilShaderManager.ltsfurc    = value; } }
        protected static Shader ltsfurtwo   { get { return lilShaderManager.ltsfurtwo ; } set { lilShaderManager.ltsfurtwo  = value; } }
        protected static Shader ltsfuro     { get { return lilShaderManager.ltsfuro   ; } set { lilShaderManager.ltsfuro    = value; } }
        protected static Shader ltsfuroc    { get { return lilShaderManager.ltsfuroc  ; } set { lilShaderManager.ltsfuroc   = value; } }
        protected static Shader ltsfurotwo  { get { return lilShaderManager.ltsfurotwo; } set { lilShaderManager.ltsfurotwo = value; } }
        protected static Shader ltsgem      { get { return lilShaderManager.ltsgem    ; } set { lilShaderManager.ltsgem     = value; } }
        protected static Shader ltsfs       { get { return lilShaderManager.ltsfs     ; } set { lilShaderManager.ltsfs      = value; } }
        protected static Shader ltsover     { get { return lilShaderManager.ltsover   ; } set { lilShaderManager.ltsover    = value; } }
        protected static Shader ltsoover    { get { return lilShaderManager.ltsoover  ; } set { lilShaderManager.ltsoover   = value; } }
        protected static Shader ltslover    { get { return lilShaderManager.ltslover  ; } set { lilShaderManager.ltslover   = value; } }
        protected static Shader ltsloover   { get { return lilShaderManager.ltsloover ; } set { lilShaderManager.ltsloover  = value; } }
        protected static Shader ltsbaker    { get { return lilShaderManager.ltsbaker  ; } set { lilShaderManager.ltsbaker   = value; } }
        protected static Shader ltspo       { get { return lilShaderManager.ltspo     ; } set { lilShaderManager.ltspo      = value; } }
        protected static Shader ltspc       { get { return lilShaderManager.ltspc     ; } set { lilShaderManager.ltspc      = value; } }
        protected static Shader ltspt       { get { return lilShaderManager.ltspt     ; } set { lilShaderManager.ltspt      = value; } }
        protected static Shader ltsptesso   { get { return lilShaderManager.ltsptesso ; } set { lilShaderManager.ltsptesso  = value; } }
        protected static Shader ltsptessc   { get { return lilShaderManager.ltsptessc ; } set { lilShaderManager.ltsptessc  = value; } }
        protected static Shader ltsptesst   { get { return lilShaderManager.ltsptesst ; } set { lilShaderManager.ltsptesst  = value; } }
        protected static Shader ltsm        { get { return lilShaderManager.ltsm      ; } set { lilShaderManager.ltsm       = value; } }
        protected static Shader ltsmo       { get { return lilShaderManager.ltsmo     ; } set { lilShaderManager.ltsmo      = value; } }
        protected static Shader ltsmref     { get { return lilShaderManager.ltsmref   ; } set { lilShaderManager.ltsmref    = value; } }
        protected static Shader ltsmfur     { get { return lilShaderManager.ltsmfur   ; } set { lilShaderManager.ltsmfur    = value; } }
        protected static Shader ltsmgem     { get { return lilShaderManager.ltsmgem   ; } set { lilShaderManager.ltsmgem    = value; } }
        protected static Shader mtoon       { get { return lilShaderManager.mtoon     ; } set { lilShaderManager.mtoon      = value; } }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Editor variables
        #region
        [Serializable]
        public class lilToonEditorSetting
        {
            public EditorMode editorMode = EditorMode.Simple;
            public int currentVersionValue = 0;
            public bool isShowBase                      = false;
            public bool isShowPrePreset                 = false;
            public bool isShowMainUV                    = false;
            public bool isShowMain                      = false;
            public bool isShowMainTone                  = false;
            public bool isShowShadow                    = false;
            public bool isShowShadowAO                  = false;
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
            public bool isShowDissolveMask              = false;
            public bool isShowDissolveNoiseMask         = false;
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
        public static lilToonEditorSetting edSet = new lilToonEditorSetting();
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
        protected static string     sAlphaMaskModes                 { get { return lilLanguageManager.sAlphaMaskModes               ; } private set { lilLanguageManager.sAlphaMaskModes                = value; } }
        protected static string     blinkSetting                    { get { return lilLanguageManager.blinkSetting                  ; } private set { lilLanguageManager.blinkSetting                   = value; } }
        protected static string     sDistanceFadeSetting            { get { return lilLanguageManager.sDistanceFadeSetting          ; } private set { lilLanguageManager.sDistanceFadeSetting           = value; } }
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
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Material properties
        #region
        private readonly lilMaterialProperty invisible              = new lilMaterialProperty("_Invisible", PropertyBlock.Base);
        private readonly lilMaterialProperty cutoff                 = new lilMaterialProperty("_Cutoff", PropertyBlock.Base);
        private readonly lilMaterialProperty preColor               = new lilMaterialProperty("_PreColor", PropertyBlock.Base);
        private readonly lilMaterialProperty preOutType             = new lilMaterialProperty("_PreOutType", PropertyBlock.Base);
        private readonly lilMaterialProperty preCutoff              = new lilMaterialProperty("_PreCutoff", PropertyBlock.Base);
        private readonly lilMaterialProperty flipNormal             = new lilMaterialProperty("_FlipNormal", PropertyBlock.Base);
        private readonly lilMaterialProperty backfaceForceShadow    = new lilMaterialProperty("_BackfaceForceShadow", PropertyBlock.Base);
        private readonly lilMaterialProperty backfaceColor          = new lilMaterialProperty("_BackfaceColor", PropertyBlock.Base);

        private readonly lilMaterialProperty asUnlit                        = new lilMaterialProperty("_AsUnlit", PropertyBlock.Lighting);
        private readonly lilMaterialProperty vertexLightStrength            = new lilMaterialProperty("_VertexLightStrength", PropertyBlock.Lighting);
        private readonly lilMaterialProperty lightMinLimit                  = new lilMaterialProperty("_LightMinLimit", PropertyBlock.Lighting);
        private readonly lilMaterialProperty lightMaxLimit                  = new lilMaterialProperty("_LightMaxLimit", PropertyBlock.Lighting);
        private readonly lilMaterialProperty beforeExposureLimit            = new lilMaterialProperty("_BeforeExposureLimit", PropertyBlock.Lighting, PropertyBlock.Rendering);
        private readonly lilMaterialProperty monochromeLighting             = new lilMaterialProperty("_MonochromeLighting", PropertyBlock.Lighting);
        private readonly lilMaterialProperty alphaBoostFA                   = new lilMaterialProperty("_AlphaBoostFA", PropertyBlock.Lighting);
        private readonly lilMaterialProperty lilDirectionalLightStrength    = new lilMaterialProperty("_lilDirectionalLightStrength", PropertyBlock.Lighting);
        private readonly lilMaterialProperty lightDirectionOverride         = new lilMaterialProperty("_LightDirectionOverride", PropertyBlock.Lighting);

        private readonly lilMaterialProperty baseColor      = new lilMaterialProperty("_BaseColor");
        private readonly lilMaterialProperty baseMap        = new lilMaterialProperty("_BaseMap", true);
        private readonly lilMaterialProperty baseColorMap   = new lilMaterialProperty("_BaseColorMap", true);

        private readonly lilMaterialProperty shiftBackfaceUV        = new lilMaterialProperty("_ShiftBackfaceUV", PropertyBlock.UV);
        private readonly lilMaterialProperty mainTex_ScrollRotate   = new lilMaterialProperty("_MainTex_ScrollRotate", PropertyBlock.UV);

        private readonly lilMaterialProperty mainColor              = new lilMaterialProperty("_Color", PropertyBlock.MainColor, PropertyBlock.MainColor1st);
        private readonly lilMaterialProperty mainTex                = new lilMaterialProperty("_MainTex", true, PropertyBlock.MainColor, PropertyBlock.MainColor1st);
        private readonly lilMaterialProperty mainTexHSVG            = new lilMaterialProperty("_MainTexHSVG", PropertyBlock.MainColor, PropertyBlock.MainColor1st);
        private readonly lilMaterialProperty mainGradationStrength  = new lilMaterialProperty("_MainGradationStrength", PropertyBlock.MainColor, PropertyBlock.MainColor1st);
        private readonly lilMaterialProperty mainGradationTex       = new lilMaterialProperty("_MainGradationTex", true, PropertyBlock.MainColor, PropertyBlock.MainColor1st);
        private readonly lilMaterialProperty mainColorAdjustMask    = new lilMaterialProperty("_MainColorAdjustMask", true, PropertyBlock.MainColor, PropertyBlock.MainColor1st);

        private readonly lilMaterialProperty useMain2ndTex                          = new lilMaterialProperty("_UseMain2ndTex", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty mainColor2nd                           = new lilMaterialProperty("_Color2nd", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTex                             = new lilMaterialProperty("_Main2ndTex", true, PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTex_UVMode                      = new lilMaterialProperty("_Main2ndTex_UVMode", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTex_Cull                        = new lilMaterialProperty("_Main2ndTex_Cull", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexAngle                        = new lilMaterialProperty("_Main2ndTexAngle", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexDecalAnimation               = new lilMaterialProperty("_Main2ndTexDecalAnimation", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexDecalSubParam                = new lilMaterialProperty("_Main2ndTexDecalSubParam", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexIsDecal                      = new lilMaterialProperty("_Main2ndTexIsDecal", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexIsLeftOnly                   = new lilMaterialProperty("_Main2ndTexIsLeftOnly", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexIsRightOnly                  = new lilMaterialProperty("_Main2ndTexIsRightOnly", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexShouldCopy                   = new lilMaterialProperty("_Main2ndTexShouldCopy", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexShouldFlipMirror             = new lilMaterialProperty("_Main2ndTexShouldFlipMirror", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexShouldFlipCopy               = new lilMaterialProperty("_Main2ndTexShouldFlipCopy", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexIsMSDF                       = new lilMaterialProperty("_Main2ndTexIsMSDF", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndBlendMask                       = new lilMaterialProperty("_Main2ndBlendMask", true, PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexBlendMode                    = new lilMaterialProperty("_Main2ndTexBlendMode", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndEnableLighting                  = new lilMaterialProperty("_Main2ndEnableLighting", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndDissolveMask                    = new lilMaterialProperty("_Main2ndDissolveMask", true, PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndDissolveNoiseMask               = new lilMaterialProperty("_Main2ndDissolveNoiseMask", true, PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndDissolveNoiseMask_ScrollRotate  = new lilMaterialProperty("_Main2ndDissolveNoiseMask_ScrollRotate", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndDissolveNoiseStrength           = new lilMaterialProperty("_Main2ndDissolveNoiseStrength", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndDissolveColor                   = new lilMaterialProperty("_Main2ndDissolveColor", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndDissolveParams                  = new lilMaterialProperty("_Main2ndDissolveParams", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndDissolvePos                     = new lilMaterialProperty("_Main2ndDissolvePos", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndDistanceFade                    = new lilMaterialProperty("_Main2ndDistanceFade", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);

        private readonly lilMaterialProperty useMain3rdTex                          = new lilMaterialProperty("_UseMain3rdTex", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty mainColor3rd                           = new lilMaterialProperty("_Color3rd", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTex                             = new lilMaterialProperty("_Main3rdTex", true, PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTex_UVMode                      = new lilMaterialProperty("_Main3rdTex_UVMode", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTex_Cull                        = new lilMaterialProperty("_Main3rdTex_Cull", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexAngle                        = new lilMaterialProperty("_Main3rdTexAngle", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexDecalAnimation               = new lilMaterialProperty("_Main3rdTexDecalAnimation", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexDecalSubParam                = new lilMaterialProperty("_Main3rdTexDecalSubParam", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexIsDecal                      = new lilMaterialProperty("_Main3rdTexIsDecal", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexIsLeftOnly                   = new lilMaterialProperty("_Main3rdTexIsLeftOnly", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexIsRightOnly                  = new lilMaterialProperty("_Main3rdTexIsRightOnly", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexShouldCopy                   = new lilMaterialProperty("_Main3rdTexShouldCopy", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexShouldFlipMirror             = new lilMaterialProperty("_Main3rdTexShouldFlipMirror", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexShouldFlipCopy               = new lilMaterialProperty("_Main3rdTexShouldFlipCopy", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexIsMSDF                       = new lilMaterialProperty("_Main3rdTexIsMSDF", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdBlendMask                       = new lilMaterialProperty("_Main3rdBlendMask", true, PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexBlendMode                    = new lilMaterialProperty("_Main3rdTexBlendMode", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdEnableLighting                  = new lilMaterialProperty("_Main3rdEnableLighting", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdDissolveMask                    = new lilMaterialProperty("_Main3rdDissolveMask", true, PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdDissolveNoiseMask               = new lilMaterialProperty("_Main3rdDissolveNoiseMask", true, PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdDissolveNoiseMask_ScrollRotate  = new lilMaterialProperty("_Main3rdDissolveNoiseMask_ScrollRotate", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdDissolveNoiseStrength           = new lilMaterialProperty("_Main3rdDissolveNoiseStrength", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdDissolveColor                   = new lilMaterialProperty("_Main3rdDissolveColor", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdDissolveParams                  = new lilMaterialProperty("_Main3rdDissolveParams", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdDissolvePos                     = new lilMaterialProperty("_Main3rdDissolvePos", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdDistanceFade                    = new lilMaterialProperty("_Main3rdDistanceFade", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);

        private readonly lilMaterialProperty alphaMaskMode  = new lilMaterialProperty("_AlphaMaskMode", PropertyBlock.MainColor, PropertyBlock.AlphaMask);
        private readonly lilMaterialProperty alphaMask      = new lilMaterialProperty("_AlphaMask", true, PropertyBlock.MainColor, PropertyBlock.AlphaMask);
        private readonly lilMaterialProperty alphaMaskScale = new lilMaterialProperty("_AlphaMaskScale", PropertyBlock.MainColor, PropertyBlock.AlphaMask);
        private readonly lilMaterialProperty alphaMaskValue = new lilMaterialProperty("_AlphaMaskValue", PropertyBlock.MainColor, PropertyBlock.AlphaMask);

        private readonly lilMaterialProperty useShadow                  = new lilMaterialProperty("_UseShadow", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowStrength             = new lilMaterialProperty("_ShadowStrength", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowStrengthMask         = new lilMaterialProperty("_ShadowStrengthMask", true, PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowBorderMask           = new lilMaterialProperty("_ShadowBorderMask", true, PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowBlurMask             = new lilMaterialProperty("_ShadowBlurMask", true, PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowStrengthMaskLOD      = new lilMaterialProperty("_ShadowStrengthMaskLOD", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowBorderMaskLOD        = new lilMaterialProperty("_ShadowBorderMaskLOD", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowBlurMaskLOD          = new lilMaterialProperty("_ShadowBlurMaskLOD", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowAOShift              = new lilMaterialProperty("_ShadowAOShift", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowAOShift2             = new lilMaterialProperty("_ShadowAOShift2", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowPostAO               = new lilMaterialProperty("_ShadowPostAO", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowColorType            = new lilMaterialProperty("_ShadowColorType", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowColor                = new lilMaterialProperty("_ShadowColor", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowColorTex             = new lilMaterialProperty("_ShadowColorTex", true, PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowNormalStrength       = new lilMaterialProperty("_ShadowNormalStrength", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowBorder               = new lilMaterialProperty("_ShadowBorder", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowBlur                 = new lilMaterialProperty("_ShadowBlur", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow2ndColor             = new lilMaterialProperty("_Shadow2ndColor", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow2ndColorTex          = new lilMaterialProperty("_Shadow2ndColorTex", true, PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow2ndNormalStrength    = new lilMaterialProperty("_Shadow2ndNormalStrength", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow2ndBorder            = new lilMaterialProperty("_Shadow2ndBorder", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow2ndBlur              = new lilMaterialProperty("_Shadow2ndBlur", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow3rdColor             = new lilMaterialProperty("_Shadow3rdColor", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow3rdColorTex          = new lilMaterialProperty("_Shadow3rdColorTex", true, PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow3rdNormalStrength    = new lilMaterialProperty("_Shadow3rdNormalStrength", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow3rdBorder            = new lilMaterialProperty("_Shadow3rdBorder", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow3rdBlur              = new lilMaterialProperty("_Shadow3rdBlur", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowMainStrength         = new lilMaterialProperty("_ShadowMainStrength", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowEnvStrength          = new lilMaterialProperty("_ShadowEnvStrength", PropertyBlock.Shadow, PropertyBlock.Lighting);
        private readonly lilMaterialProperty shadowBorderColor          = new lilMaterialProperty("_ShadowBorderColor", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowBorderRange          = new lilMaterialProperty("_ShadowBorderRange", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowReceive              = new lilMaterialProperty("_ShadowReceive", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow2ndReceive           = new lilMaterialProperty("_Shadow2ndReceive", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow3rdReceive           = new lilMaterialProperty("_Shadow3rdReceive", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowMaskType             = new lilMaterialProperty("_ShadowMaskType", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowFlatBorder           = new lilMaterialProperty("_ShadowFlatBorder", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowFlatBlur             = new lilMaterialProperty("_ShadowFlatBlur", PropertyBlock.Shadow);
        private readonly lilMaterialProperty lilShadowCasterBias        = new lilMaterialProperty("_lilShadowCasterBias", PropertyBlock.Shadow, PropertyBlock.Rendering);

        private readonly lilMaterialProperty useEmission                    = new lilMaterialProperty("_UseEmission", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionColor                  = new lilMaterialProperty("_EmissionColor", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionMap                    = new lilMaterialProperty("_EmissionMap", true, PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionMap_ScrollRotate       = new lilMaterialProperty("_EmissionMap_ScrollRotate", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionMap_UVMode             = new lilMaterialProperty("_EmissionMap_UVMode", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionMainStrength           = new lilMaterialProperty("_EmissionMainStrength", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionBlend                  = new lilMaterialProperty("_EmissionBlend", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionBlendMask              = new lilMaterialProperty("_EmissionBlendMask", true, PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionBlendMask_ScrollRotate = new lilMaterialProperty("_EmissionBlendMask_ScrollRotate", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionBlink                  = new lilMaterialProperty("_EmissionBlink", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionUseGrad                = new lilMaterialProperty("_EmissionUseGrad", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionGradTex                = new lilMaterialProperty("_EmissionGradTex", true, PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionGradSpeed              = new lilMaterialProperty("_EmissionGradSpeed", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionParallaxDepth          = new lilMaterialProperty("_EmissionParallaxDepth", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionFluorescence           = new lilMaterialProperty("_EmissionFluorescence", PropertyBlock.Emission, PropertyBlock.Emission1st);

        private readonly lilMaterialProperty useEmission2nd                     = new lilMaterialProperty("_UseEmission2nd", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndColor                   = new lilMaterialProperty("_Emission2ndColor", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndMap                     = new lilMaterialProperty("_Emission2ndMap", true, PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndMap_ScrollRotate        = new lilMaterialProperty("_Emission2ndMap_ScrollRotate", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndMap_UVMode              = new lilMaterialProperty("_Emission2ndMap_UVMode", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndMainStrength            = new lilMaterialProperty("_Emission2ndMainStrength", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndBlend                   = new lilMaterialProperty("_Emission2ndBlend", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndBlendMask               = new lilMaterialProperty("_Emission2ndBlendMask", true, PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndBlendMask_ScrollRotate  = new lilMaterialProperty("_Emission2ndBlendMask_ScrollRotate", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndBlink                   = new lilMaterialProperty("_Emission2ndBlink", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndUseGrad                 = new lilMaterialProperty("_Emission2ndUseGrad", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndGradTex                 = new lilMaterialProperty("_Emission2ndGradTex", true, PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndGradSpeed               = new lilMaterialProperty("_Emission2ndGradSpeed", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndParallaxDepth           = new lilMaterialProperty("_Emission2ndParallaxDepth", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndFluorescence            = new lilMaterialProperty("_Emission2ndFluorescence", PropertyBlock.Emission, PropertyBlock.Emission2nd);

        private readonly lilMaterialProperty useBumpMap = new lilMaterialProperty("_UseBumpMap", PropertyBlock.NormalMap, PropertyBlock.NormalMap1st);
        private readonly lilMaterialProperty bumpMap    = new lilMaterialProperty("_BumpMap", true, PropertyBlock.NormalMap, PropertyBlock.NormalMap1st);
        private readonly lilMaterialProperty bumpScale  = new lilMaterialProperty("_BumpScale", PropertyBlock.NormalMap, PropertyBlock.NormalMap1st);

        private readonly lilMaterialProperty useBump2ndMap      = new lilMaterialProperty("_UseBump2ndMap", PropertyBlock.NormalMap, PropertyBlock.NormalMap2nd);
        private readonly lilMaterialProperty bump2ndMap         = new lilMaterialProperty("_Bump2ndMap", true, PropertyBlock.NormalMap, PropertyBlock.NormalMap2nd);
        private readonly lilMaterialProperty bump2ndMap_UVMode  = new lilMaterialProperty("_Bump2ndMap_UVMode", PropertyBlock.NormalMap, PropertyBlock.NormalMap2nd);
        private readonly lilMaterialProperty bump2ndScale       = new lilMaterialProperty("_Bump2ndScale", PropertyBlock.NormalMap, PropertyBlock.NormalMap2nd);
        private readonly lilMaterialProperty bump2ndScaleMask   = new lilMaterialProperty("_Bump2ndScaleMask", true, PropertyBlock.NormalMap, PropertyBlock.NormalMap2nd);

        private readonly lilMaterialProperty useAnisotropy                  = new lilMaterialProperty("_UseAnisotropy", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropyTangentMap           = new lilMaterialProperty("_AnisotropyTangentMap", true, PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropyScale                = new lilMaterialProperty("_AnisotropyScale", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropyScaleMask            = new lilMaterialProperty("_AnisotropyScaleMask", true, PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropyTangentWidth         = new lilMaterialProperty("_AnisotropyTangentWidth", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropyBitangentWidth       = new lilMaterialProperty("_AnisotropyBitangentWidth", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropyShift                = new lilMaterialProperty("_AnisotropyShift", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropyShiftNoiseScale      = new lilMaterialProperty("_AnisotropyShiftNoiseScale", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropySpecularStrength     = new lilMaterialProperty("_AnisotropySpecularStrength", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropy2ndTangentWidth      = new lilMaterialProperty("_Anisotropy2ndTangentWidth", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropy2ndBitangentWidth    = new lilMaterialProperty("_Anisotropy2ndBitangentWidth", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropy2ndShift             = new lilMaterialProperty("_Anisotropy2ndShift", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropy2ndShiftNoiseScale   = new lilMaterialProperty("_Anisotropy2ndShiftNoiseScale", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropy2ndSpecularStrength  = new lilMaterialProperty("_Anisotropy2ndSpecularStrength", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropyShiftNoiseMask       = new lilMaterialProperty("_AnisotropyShiftNoiseMask", true, PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropy2Reflection          = new lilMaterialProperty("_Anisotropy2Reflection", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropy2MatCap              = new lilMaterialProperty("_Anisotropy2MatCap", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropy2MatCap2nd           = new lilMaterialProperty("_Anisotropy2MatCap2nd", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);

        private readonly lilMaterialProperty useBacklight               = new lilMaterialProperty("_UseBacklight", PropertyBlock.Backlight);
        private readonly lilMaterialProperty backlightColor             = new lilMaterialProperty("_BacklightColor", PropertyBlock.Backlight);
        private readonly lilMaterialProperty backlightColorTex          = new lilMaterialProperty("_BacklightColorTex", true, PropertyBlock.Backlight);
        private readonly lilMaterialProperty backlightMainStrength      = new lilMaterialProperty("_BacklightMainStrength", PropertyBlock.Backlight);
        private readonly lilMaterialProperty backlightNormalStrength    = new lilMaterialProperty("_BacklightNormalStrength", PropertyBlock.Backlight);
        private readonly lilMaterialProperty backlightBorder            = new lilMaterialProperty("_BacklightBorder", PropertyBlock.Backlight);
        private readonly lilMaterialProperty backlightBlur              = new lilMaterialProperty("_BacklightBlur", PropertyBlock.Backlight);
        private readonly lilMaterialProperty backlightDirectivity       = new lilMaterialProperty("_BacklightDirectivity", PropertyBlock.Backlight);
        private readonly lilMaterialProperty backlightViewStrength      = new lilMaterialProperty("_BacklightViewStrength", PropertyBlock.Backlight);
        private readonly lilMaterialProperty backlightReceiveShadow     = new lilMaterialProperty("_BacklightReceiveShadow", PropertyBlock.Backlight);
        private readonly lilMaterialProperty backlightBackfaceMask      = new lilMaterialProperty("_BacklightBackfaceMask", PropertyBlock.Backlight);

        private readonly lilMaterialProperty useReflection                  = new lilMaterialProperty("_UseReflection", PropertyBlock.Reflection);
        private readonly lilMaterialProperty metallic                       = new lilMaterialProperty("_Metallic", PropertyBlock.Reflection, PropertyBlock.Gem);
        private readonly lilMaterialProperty metallicGlossMap               = new lilMaterialProperty("_MetallicGlossMap", true, PropertyBlock.Reflection, PropertyBlock.Gem);
        private readonly lilMaterialProperty smoothness                     = new lilMaterialProperty("_Smoothness", PropertyBlock.Reflection);
        private readonly lilMaterialProperty smoothnessTex                  = new lilMaterialProperty("_SmoothnessTex", true, PropertyBlock.Reflection);
        private readonly lilMaterialProperty reflectance                    = new lilMaterialProperty("_Reflectance", PropertyBlock.Reflection);
        private readonly lilMaterialProperty reflectionColor                = new lilMaterialProperty("_ReflectionColor", PropertyBlock.Reflection);
        private readonly lilMaterialProperty reflectionColorTex             = new lilMaterialProperty("_ReflectionColorTex", true, PropertyBlock.Reflection);
        private readonly lilMaterialProperty gsaaStrength                   = new lilMaterialProperty("_GSAAStrength", PropertyBlock.Reflection);
        private readonly lilMaterialProperty applySpecular                  = new lilMaterialProperty("_ApplySpecular", PropertyBlock.Reflection);
        private readonly lilMaterialProperty applySpecularFA                = new lilMaterialProperty("_ApplySpecularFA", PropertyBlock.Reflection);
        private readonly lilMaterialProperty specularNormalStrength         = new lilMaterialProperty("_SpecularNormalStrength", PropertyBlock.Reflection);
        private readonly lilMaterialProperty specularToon                   = new lilMaterialProperty("_SpecularToon", PropertyBlock.Reflection);
        private readonly lilMaterialProperty specularBorder                 = new lilMaterialProperty("_SpecularBorder", PropertyBlock.Reflection);
        private readonly lilMaterialProperty specularBlur                   = new lilMaterialProperty("_SpecularBlur", PropertyBlock.Reflection);
        private readonly lilMaterialProperty applyReflection                = new lilMaterialProperty("_ApplyReflection", PropertyBlock.Reflection);
        private readonly lilMaterialProperty reflectionNormalStrength       = new lilMaterialProperty("_ReflectionNormalStrength", PropertyBlock.Reflection);
        private readonly lilMaterialProperty reflectionApplyTransparency    = new lilMaterialProperty("_ReflectionApplyTransparency", PropertyBlock.Reflection);
        private readonly lilMaterialProperty reflectionCubeTex              = new lilMaterialProperty("_ReflectionCubeTex", PropertyBlock.Reflection);
        private readonly lilMaterialProperty reflectionCubeColor            = new lilMaterialProperty("_ReflectionCubeColor", true, PropertyBlock.Reflection);
        private readonly lilMaterialProperty reflectionCubeOverride         = new lilMaterialProperty("_ReflectionCubeOverride", PropertyBlock.Reflection);
        private readonly lilMaterialProperty reflectionCubeEnableLighting   = new lilMaterialProperty("_ReflectionCubeEnableLighting", PropertyBlock.Reflection);
        private readonly lilMaterialProperty reflectionBlendMode            = new lilMaterialProperty("_ReflectionBlendMode", PropertyBlock.Reflection);

        private readonly lilMaterialProperty useMatCap                  = new lilMaterialProperty("_UseMatCap", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapTex                  = new lilMaterialProperty("_MatCapTex", true, PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapColor                = new lilMaterialProperty("_MatCapColor", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapMainStrength         = new lilMaterialProperty("_MatCapMainStrength", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapBlendUV1             = new lilMaterialProperty("_MatCapBlendUV1", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapZRotCancel           = new lilMaterialProperty("_MatCapZRotCancel", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapPerspective          = new lilMaterialProperty("_MatCapPerspective", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapVRParallaxStrength   = new lilMaterialProperty("_MatCapVRParallaxStrength", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapBlend                = new lilMaterialProperty("_MatCapBlend", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapBlendMask            = new lilMaterialProperty("_MatCapBlendMask", true, PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapEnableLighting       = new lilMaterialProperty("_MatCapEnableLighting", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapShadowMask           = new lilMaterialProperty("_MatCapShadowMask", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapBackfaceMask         = new lilMaterialProperty("_MatCapBackfaceMask", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapLod                  = new lilMaterialProperty("_MatCapLod", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapBlendMode            = new lilMaterialProperty("_MatCapBlendMode", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapApplyTransparency    = new lilMaterialProperty("_MatCapApplyTransparency", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapNormalStrength       = new lilMaterialProperty("_MatCapNormalStrength", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapCustomNormal         = new lilMaterialProperty("_MatCapCustomNormal", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapBumpMap              = new lilMaterialProperty("_MatCapBumpMap", true, PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapBumpScale            = new lilMaterialProperty("_MatCapBumpScale", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);

        private readonly lilMaterialProperty useMatCap2nd                   = new lilMaterialProperty("_UseMatCap2nd", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndTex                   = new lilMaterialProperty("_MatCap2ndTex", true, PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndColor                 = new lilMaterialProperty("_MatCap2ndColor", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndMainStrength          = new lilMaterialProperty("_MatCap2ndMainStrength", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndBlendUV1              = new lilMaterialProperty("_MatCap2ndBlendUV1", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndZRotCancel            = new lilMaterialProperty("_MatCap2ndZRotCancel", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndPerspective           = new lilMaterialProperty("_MatCap2ndPerspective", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndVRParallaxStrength    = new lilMaterialProperty("_MatCap2ndVRParallaxStrength", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndBlend                 = new lilMaterialProperty("_MatCap2ndBlend", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndBlendMask             = new lilMaterialProperty("_MatCap2ndBlendMask", true, PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndEnableLighting        = new lilMaterialProperty("_MatCap2ndEnableLighting", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndShadowMask            = new lilMaterialProperty("_MatCap2ndShadowMask", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndBackfaceMask          = new lilMaterialProperty("_MatCap2ndBackfaceMask", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndLod                   = new lilMaterialProperty("_MatCap2ndLod", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndBlendMode             = new lilMaterialProperty("_MatCap2ndBlendMode", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndApplyTransparency     = new lilMaterialProperty("_MatCap2ndApplyTransparency", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndNormalStrength        = new lilMaterialProperty("_MatCap2ndNormalStrength", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndCustomNormal          = new lilMaterialProperty("_MatCap2ndCustomNormal", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndBumpMap               = new lilMaterialProperty("_MatCap2ndBumpMap", true, PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndBumpScale             = new lilMaterialProperty("_MatCap2ndBumpScale", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);

        private readonly lilMaterialProperty useRim                 = new lilMaterialProperty("_UseRim", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimColor               = new lilMaterialProperty("_RimColor", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimColorTex            = new lilMaterialProperty("_RimColorTex", true, PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimMainStrength        = new lilMaterialProperty("_RimMainStrength", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimNormalStrength      = new lilMaterialProperty("_RimNormalStrength", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimBorder              = new lilMaterialProperty("_RimBorder", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimBlur                = new lilMaterialProperty("_RimBlur", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimFresnelPower        = new lilMaterialProperty("_RimFresnelPower", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimEnableLighting      = new lilMaterialProperty("_RimEnableLighting", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimShadowMask          = new lilMaterialProperty("_RimShadowMask", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimBackfaceMask        = new lilMaterialProperty("_RimBackfaceMask", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimVRParallaxStrength  = new lilMaterialProperty("_RimVRParallaxStrength", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimApplyTransparency   = new lilMaterialProperty("_RimApplyTransparency", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimDirStrength         = new lilMaterialProperty("_RimDirStrength", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimDirRange            = new lilMaterialProperty("_RimDirRange", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimIndirRange          = new lilMaterialProperty("_RimIndirRange", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimIndirColor          = new lilMaterialProperty("_RimIndirColor", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimIndirBorder         = new lilMaterialProperty("_RimIndirBorder", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimIndirBlur           = new lilMaterialProperty("_RimIndirBlur", PropertyBlock.RimLight);

        private readonly lilMaterialProperty useGlitter                 = new lilMaterialProperty("_UseGlitter", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterUVMode              = new lilMaterialProperty("_GlitterUVMode", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterColor               = new lilMaterialProperty("_GlitterColor", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterColorTex            = new lilMaterialProperty("_GlitterColorTex", true, PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterColorTex_UVMode     = new lilMaterialProperty("_GlitterColorTex_UVMode", true, PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterMainStrength        = new lilMaterialProperty("_GlitterMainStrength", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterScaleRandomize      = new lilMaterialProperty("_GlitterScaleRandomize", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterApplyShape          = new lilMaterialProperty("_GlitterApplyShape", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterShapeTex            = new lilMaterialProperty("_GlitterShapeTex", true, PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterAtras               = new lilMaterialProperty("_GlitterAtras", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterAngleRandomize      = new lilMaterialProperty("_GlitterAngleRandomize", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterParams1             = new lilMaterialProperty("_GlitterParams1", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterParams2             = new lilMaterialProperty("_GlitterParams2", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterPostContrast        = new lilMaterialProperty("_GlitterPostContrast", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterSensitivity         = new lilMaterialProperty("_GlitterSensitivity", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterEnableLighting      = new lilMaterialProperty("_GlitterEnableLighting", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterShadowMask          = new lilMaterialProperty("_GlitterShadowMask", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterBackfaceMask        = new lilMaterialProperty("_GlitterBackfaceMask", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterApplyTransparency   = new lilMaterialProperty("_GlitterApplyTransparency", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterVRParallaxStrength  = new lilMaterialProperty("_GlitterVRParallaxStrength", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterNormalStrength      = new lilMaterialProperty("_GlitterNormalStrength", PropertyBlock.Glitter);

        private readonly lilMaterialProperty gemChromaticAberration = new lilMaterialProperty("_GemChromaticAberration", PropertyBlock.Gem);
        private readonly lilMaterialProperty gemEnvContrast         = new lilMaterialProperty("_GemEnvContrast", PropertyBlock.Gem);
        private readonly lilMaterialProperty gemEnvColor            = new lilMaterialProperty("_GemEnvColor", PropertyBlock.Gem);
        private readonly lilMaterialProperty gemParticleLoop        = new lilMaterialProperty("_GemParticleLoop", PropertyBlock.Gem);
        private readonly lilMaterialProperty gemParticleColor       = new lilMaterialProperty("_GemParticleColor", PropertyBlock.Gem);
        private readonly lilMaterialProperty gemVRParallaxStrength  = new lilMaterialProperty("_GemVRParallaxStrength", PropertyBlock.Gem);

        private readonly lilMaterialProperty outlineColor               = new lilMaterialProperty("_OutlineColor", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineTex                 = new lilMaterialProperty("_OutlineTex", true, PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineTex_ScrollRotate    = new lilMaterialProperty("_OutlineTex_ScrollRotate", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineTexHSVG             = new lilMaterialProperty("_OutlineTexHSVG", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineLitColor            = new lilMaterialProperty("_OutlineLitColor", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineLitApplyTex         = new lilMaterialProperty("_OutlineLitApplyTex", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineLitScale            = new lilMaterialProperty("_OutlineLitScale", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineLitOffset           = new lilMaterialProperty("_OutlineLitOffset", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineLitShadowReceive    = new lilMaterialProperty("_OutlineLitShadowReceive", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineWidth               = new lilMaterialProperty("_OutlineWidth", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineWidthMask           = new lilMaterialProperty("_OutlineWidthMask", true, PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineFixWidth            = new lilMaterialProperty("_OutlineFixWidth", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineVertexR2Width       = new lilMaterialProperty("_OutlineVertexR2Width", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineDeleteMesh          = new lilMaterialProperty("_OutlineDeleteMesh", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineVectorTex           = new lilMaterialProperty("_OutlineVectorTex", true, PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineVectorUVMode        = new lilMaterialProperty("_OutlineVectorUVMode", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineVectorScale         = new lilMaterialProperty("_OutlineVectorScale", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineEnableLighting      = new lilMaterialProperty("_OutlineEnableLighting", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineZBias               = new lilMaterialProperty("_OutlineZBias", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineDisableInVR         = new lilMaterialProperty("_OutlineDisableInVR", PropertyBlock.Outline);

        private readonly lilMaterialProperty useParallax    = new lilMaterialProperty("_UseParallax", PropertyBlock.Parallax);
        private readonly lilMaterialProperty usePOM         = new lilMaterialProperty("_UsePOM", PropertyBlock.Parallax);
        private readonly lilMaterialProperty parallaxMap    = new lilMaterialProperty("_ParallaxMap", true, PropertyBlock.Parallax);
        private readonly lilMaterialProperty parallax       = new lilMaterialProperty("_Parallax", PropertyBlock.Parallax);
        private readonly lilMaterialProperty parallaxOffset = new lilMaterialProperty("_ParallaxOffset", PropertyBlock.Parallax);

        private readonly lilMaterialProperty distanceFade       = new lilMaterialProperty("_DistanceFade", PropertyBlock.DistanceFade);
        private readonly lilMaterialProperty distanceFadeColor  = new lilMaterialProperty("_DistanceFadeColor", PropertyBlock.DistanceFade);

        private readonly lilMaterialProperty useAudioLink               = new lilMaterialProperty("_UseAudioLink", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkDefaultValue      = new lilMaterialProperty("_AudioLinkDefaultValue", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkUVMode            = new lilMaterialProperty("_AudioLinkUVMode", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkUVParams          = new lilMaterialProperty("_AudioLinkUVParams", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkStart             = new lilMaterialProperty("_AudioLinkStart", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkMask              = new lilMaterialProperty("_AudioLinkMask", true, PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLink2Main2nd          = new lilMaterialProperty("_AudioLink2Main2nd", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLink2Main3rd          = new lilMaterialProperty("_AudioLink2Main3rd", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLink2Emission         = new lilMaterialProperty("_AudioLink2Emission", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLink2EmissionGrad     = new lilMaterialProperty("_AudioLink2EmissionGrad", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLink2Emission2nd      = new lilMaterialProperty("_AudioLink2Emission2nd", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLink2Emission2ndGrad  = new lilMaterialProperty("_AudioLink2Emission2ndGrad", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLink2Vertex           = new lilMaterialProperty("_AudioLink2Vertex", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkVertexUVMode      = new lilMaterialProperty("_AudioLinkVertexUVMode", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkVertexUVParams    = new lilMaterialProperty("_AudioLinkVertexUVParams", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkVertexStart       = new lilMaterialProperty("_AudioLinkVertexStart", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkVertexStrength    = new lilMaterialProperty("_AudioLinkVertexStrength", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkAsLocal           = new lilMaterialProperty("_AudioLinkAsLocal", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkLocalMap          = new lilMaterialProperty("_AudioLinkLocalMap", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkLocalMapParams    = new lilMaterialProperty("_AudioLinkLocalMapParams", PropertyBlock.AudioLink);

        private readonly lilMaterialProperty dissolveMask                   = new lilMaterialProperty("_DissolveMask", true, PropertyBlock.Dissolve);
        private readonly lilMaterialProperty dissolveNoiseMask              = new lilMaterialProperty("_DissolveNoiseMask", true, PropertyBlock.Dissolve);
        private readonly lilMaterialProperty dissolveNoiseMask_ScrollRotate = new lilMaterialProperty("_DissolveNoiseMask_ScrollRotate", PropertyBlock.Dissolve);
        private readonly lilMaterialProperty dissolveNoiseStrength          = new lilMaterialProperty("_DissolveNoiseStrength", PropertyBlock.Dissolve);
        private readonly lilMaterialProperty dissolveColor                  = new lilMaterialProperty("_DissolveColor", PropertyBlock.Dissolve);
        private readonly lilMaterialProperty dissolveParams                 = new lilMaterialProperty("_DissolveParams", PropertyBlock.Dissolve);
        private readonly lilMaterialProperty dissolvePos                    = new lilMaterialProperty("_DissolvePos", PropertyBlock.Dissolve);

        private readonly lilMaterialProperty ignoreEncryption   = new lilMaterialProperty("_IgnoreEncryption", PropertyBlock.Encryption);
        private readonly lilMaterialProperty keys               = new lilMaterialProperty("_Keys", PropertyBlock.Encryption);

        private readonly lilMaterialProperty refractionStrength         = new lilMaterialProperty("_RefractionStrength", PropertyBlock.Refraction, PropertyBlock.Gem);
        private readonly lilMaterialProperty refractionFresnelPower     = new lilMaterialProperty("_RefractionFresnelPower", PropertyBlock.Refraction, PropertyBlock.Gem);
        private readonly lilMaterialProperty refractionColorFromMain    = new lilMaterialProperty("_RefractionColorFromMain", PropertyBlock.Refraction);
        private readonly lilMaterialProperty refractionColor            = new lilMaterialProperty("_RefractionColor", PropertyBlock.Refraction);

        private readonly lilMaterialProperty furNoiseMask           = new lilMaterialProperty("_FurNoiseMask", true, PropertyBlock.Fur);
        private readonly lilMaterialProperty furMask                = new lilMaterialProperty("_FurMask", true, PropertyBlock.Fur);
        private readonly lilMaterialProperty furLengthMask          = new lilMaterialProperty("_FurLengthMask", true, PropertyBlock.Fur);
        private readonly lilMaterialProperty furVectorTex           = new lilMaterialProperty("_FurVectorTex", true, PropertyBlock.Fur);
        private readonly lilMaterialProperty furVectorScale         = new lilMaterialProperty("_FurVectorScale", PropertyBlock.Fur);
        private readonly lilMaterialProperty furVector              = new lilMaterialProperty("_FurVector", PropertyBlock.Fur);
        private readonly lilMaterialProperty furGravity             = new lilMaterialProperty("_FurGravity", PropertyBlock.Fur);
        private readonly lilMaterialProperty furRandomize           = new lilMaterialProperty("_FurRandomize", PropertyBlock.Fur);
        private readonly lilMaterialProperty furAO                  = new lilMaterialProperty("_FurAO", PropertyBlock.Fur);
        private readonly lilMaterialProperty vertexColor2FurVector  = new lilMaterialProperty("_VertexColor2FurVector", PropertyBlock.Fur);
        private readonly lilMaterialProperty furMeshType            = new lilMaterialProperty("_FurMeshType", PropertyBlock.Fur);
        private readonly lilMaterialProperty furLayerNum            = new lilMaterialProperty("_FurLayerNum", PropertyBlock.Fur);
        private readonly lilMaterialProperty furRootOffset          = new lilMaterialProperty("_FurRootOffset", PropertyBlock.Fur);
        private readonly lilMaterialProperty furCutoutLength        = new lilMaterialProperty("_FurCutoutLength", PropertyBlock.Fur);
        private readonly lilMaterialProperty furTouchStrength       = new lilMaterialProperty("_FurTouchStrength", PropertyBlock.Fur);

        private readonly lilMaterialProperty stencilRef                 = new lilMaterialProperty("_StencilRef", PropertyBlock.Stencil);
        private readonly lilMaterialProperty stencilReadMask            = new lilMaterialProperty("_StencilReadMask", PropertyBlock.Stencil);
        private readonly lilMaterialProperty stencilWriteMask           = new lilMaterialProperty("_StencilWriteMask", PropertyBlock.Stencil);
        private readonly lilMaterialProperty stencilComp                = new lilMaterialProperty("_StencilComp", PropertyBlock.Stencil);
        private readonly lilMaterialProperty stencilPass                = new lilMaterialProperty("_StencilPass", PropertyBlock.Stencil);
        private readonly lilMaterialProperty stencilFail                = new lilMaterialProperty("_StencilFail", PropertyBlock.Stencil);
        private readonly lilMaterialProperty stencilZFail               = new lilMaterialProperty("_StencilZFail", PropertyBlock.Stencil);
        private readonly lilMaterialProperty preStencilRef              = new lilMaterialProperty("_PreStencilRef", PropertyBlock.Stencil);
        private readonly lilMaterialProperty preStencilReadMask         = new lilMaterialProperty("_PreStencilReadMask", PropertyBlock.Stencil);
        private readonly lilMaterialProperty preStencilWriteMask        = new lilMaterialProperty("_PreStencilWriteMask", PropertyBlock.Stencil);
        private readonly lilMaterialProperty preStencilComp             = new lilMaterialProperty("_PreStencilComp", PropertyBlock.Stencil);
        private readonly lilMaterialProperty preStencilPass             = new lilMaterialProperty("_PreStencilPass", PropertyBlock.Stencil);
        private readonly lilMaterialProperty preStencilFail             = new lilMaterialProperty("_PreStencilFail", PropertyBlock.Stencil);
        private readonly lilMaterialProperty preStencilZFail            = new lilMaterialProperty("_PreStencilZFail", PropertyBlock.Stencil);
        private readonly lilMaterialProperty outlineStencilRef          = new lilMaterialProperty("_OutlineStencilRef", PropertyBlock.Stencil);
        private readonly lilMaterialProperty outlineStencilReadMask     = new lilMaterialProperty("_OutlineStencilReadMask", PropertyBlock.Stencil);
        private readonly lilMaterialProperty outlineStencilWriteMask    = new lilMaterialProperty("_OutlineStencilWriteMask", PropertyBlock.Stencil);
        private readonly lilMaterialProperty outlineStencilComp         = new lilMaterialProperty("_OutlineStencilComp", PropertyBlock.Stencil);
        private readonly lilMaterialProperty outlineStencilPass         = new lilMaterialProperty("_OutlineStencilPass", PropertyBlock.Stencil);
        private readonly lilMaterialProperty outlineStencilFail         = new lilMaterialProperty("_OutlineStencilFail", PropertyBlock.Stencil);
        private readonly lilMaterialProperty outlineStencilZFail        = new lilMaterialProperty("_OutlineStencilZFail", PropertyBlock.Stencil);
        private readonly lilMaterialProperty furStencilRef              = new lilMaterialProperty("_FurStencilRef", PropertyBlock.Stencil);
        private readonly lilMaterialProperty furStencilReadMask         = new lilMaterialProperty("_FurStencilReadMask", PropertyBlock.Stencil);
        private readonly lilMaterialProperty furStencilWriteMask        = new lilMaterialProperty("_FurStencilWriteMask", PropertyBlock.Stencil);
        private readonly lilMaterialProperty furStencilComp             = new lilMaterialProperty("_FurStencilComp", PropertyBlock.Stencil);
        private readonly lilMaterialProperty furStencilPass             = new lilMaterialProperty("_FurStencilPass", PropertyBlock.Stencil);
        private readonly lilMaterialProperty furStencilFail             = new lilMaterialProperty("_FurStencilFail", PropertyBlock.Stencil);
        private readonly lilMaterialProperty furStencilZFail            = new lilMaterialProperty("_FurStencilZFail", PropertyBlock.Stencil);

        private readonly lilMaterialProperty subpassCutoff          = new lilMaterialProperty("_SubpassCutoff", PropertyBlock.Rendering);
        private readonly lilMaterialProperty cull                   = new lilMaterialProperty("_Cull", PropertyBlock.Rendering, PropertyBlock.Base);
        private readonly lilMaterialProperty srcBlend               = new lilMaterialProperty("_SrcBlend", PropertyBlock.Rendering);
        private readonly lilMaterialProperty dstBlend               = new lilMaterialProperty("_DstBlend", PropertyBlock.Rendering);
        private readonly lilMaterialProperty srcBlendAlpha          = new lilMaterialProperty("_SrcBlendAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty dstBlendAlpha          = new lilMaterialProperty("_DstBlendAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty blendOp                = new lilMaterialProperty("_BlendOp", PropertyBlock.Rendering);
        private readonly lilMaterialProperty blendOpAlpha           = new lilMaterialProperty("_BlendOpAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty srcBlendFA             = new lilMaterialProperty("_SrcBlendFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty dstBlendFA             = new lilMaterialProperty("_DstBlendFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty srcBlendAlphaFA        = new lilMaterialProperty("_SrcBlendAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty dstBlendAlphaFA        = new lilMaterialProperty("_DstBlendAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty blendOpFA              = new lilMaterialProperty("_BlendOpFA", PropertyBlock.Rendering, PropertyBlock.Lighting);
        private readonly lilMaterialProperty blendOpAlphaFA         = new lilMaterialProperty("_BlendOpAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty zclip                  = new lilMaterialProperty("_ZClip", PropertyBlock.Rendering);
        private readonly lilMaterialProperty zwrite                 = new lilMaterialProperty("_ZWrite", PropertyBlock.Rendering, PropertyBlock.Base);
        private readonly lilMaterialProperty ztest                  = new lilMaterialProperty("_ZTest", PropertyBlock.Rendering);
        private readonly lilMaterialProperty offsetFactor           = new lilMaterialProperty("_OffsetFactor", PropertyBlock.Rendering);
        private readonly lilMaterialProperty offsetUnits            = new lilMaterialProperty("_OffsetUnits", PropertyBlock.Rendering);
        private readonly lilMaterialProperty colorMask              = new lilMaterialProperty("_ColorMask", PropertyBlock.Rendering);
        private readonly lilMaterialProperty alphaToMask            = new lilMaterialProperty("_AlphaToMask", PropertyBlock.Rendering);

        private readonly lilMaterialProperty preCull                = new lilMaterialProperty("_PreCull", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preSrcBlend            = new lilMaterialProperty("_PreSrcBlend", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preDstBlend            = new lilMaterialProperty("_PreDstBlend", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preSrcBlendAlpha       = new lilMaterialProperty("_PreSrcBlendAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preDstBlendAlpha       = new lilMaterialProperty("_PreDstBlendAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preBlendOp             = new lilMaterialProperty("_PreBlendOp", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preBlendOpAlpha        = new lilMaterialProperty("_PreBlendOpAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preSrcBlendFA          = new lilMaterialProperty("_PreSrcBlendFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preDstBlendFA          = new lilMaterialProperty("_PreDstBlendFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preSrcBlendAlphaFA     = new lilMaterialProperty("_PreSrcBlendAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preDstBlendAlphaFA     = new lilMaterialProperty("_PreDstBlendAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preBlendOpFA           = new lilMaterialProperty("_PreBlendOpFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preBlendOpAlphaFA      = new lilMaterialProperty("_PreBlendOpAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preZclip               = new lilMaterialProperty("_PreZClip", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preZwrite              = new lilMaterialProperty("_PreZWrite", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preZtest               = new lilMaterialProperty("_PreZTest", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preOffsetFactor        = new lilMaterialProperty("_PreOffsetFactor", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preOffsetUnits         = new lilMaterialProperty("_PreOffsetUnits", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preColorMask           = new lilMaterialProperty("_PreColorMask", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preAlphaToMask         = new lilMaterialProperty("_PreAlphaToMask", PropertyBlock.Rendering);

        private readonly lilMaterialProperty outlineCull            = new lilMaterialProperty("_OutlineCull", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineSrcBlend        = new lilMaterialProperty("_OutlineSrcBlend", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineDstBlend        = new lilMaterialProperty("_OutlineDstBlend", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineSrcBlendAlpha   = new lilMaterialProperty("_OutlineSrcBlendAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineDstBlendAlpha   = new lilMaterialProperty("_OutlineDstBlendAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineBlendOp         = new lilMaterialProperty("_OutlineBlendOp", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineBlendOpAlpha    = new lilMaterialProperty("_OutlineBlendOpAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineSrcBlendFA      = new lilMaterialProperty("_OutlineSrcBlendFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineDstBlendFA      = new lilMaterialProperty("_OutlineDstBlendFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineSrcBlendAlphaFA = new lilMaterialProperty("_OutlineSrcBlendAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineDstBlendAlphaFA = new lilMaterialProperty("_OutlineDstBlendAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineBlendOpFA       = new lilMaterialProperty("_OutlineBlendOpFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineBlendOpAlphaFA  = new lilMaterialProperty("_OutlineBlendOpAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineZclip           = new lilMaterialProperty("_OutlineZClip", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineZwrite          = new lilMaterialProperty("_OutlineZWrite", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineZtest           = new lilMaterialProperty("_OutlineZTest", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineOffsetFactor    = new lilMaterialProperty("_OutlineOffsetFactor", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineOffsetUnits     = new lilMaterialProperty("_OutlineOffsetUnits", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineColorMask       = new lilMaterialProperty("_OutlineColorMask", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineAlphaToMask     = new lilMaterialProperty("_OutlineAlphaToMask", PropertyBlock.Rendering);

        private readonly lilMaterialProperty furCull                = new lilMaterialProperty("_FurCull", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furSrcBlend            = new lilMaterialProperty("_FurSrcBlend", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furDstBlend            = new lilMaterialProperty("_FurDstBlend", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furSrcBlendAlpha       = new lilMaterialProperty("_FurSrcBlendAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furDstBlendAlpha       = new lilMaterialProperty("_FurDstBlendAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furBlendOp             = new lilMaterialProperty("_FurBlendOp", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furBlendOpAlpha        = new lilMaterialProperty("_FurBlendOpAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furSrcBlendFA          = new lilMaterialProperty("_FurSrcBlendFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furDstBlendFA          = new lilMaterialProperty("_FurDstBlendFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furSrcBlendAlphaFA     = new lilMaterialProperty("_FurSrcBlendAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furDstBlendAlphaFA     = new lilMaterialProperty("_FurDstBlendAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furBlendOpFA           = new lilMaterialProperty("_FurBlendOpFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furBlendOpAlphaFA      = new lilMaterialProperty("_FurBlendOpAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furZclip               = new lilMaterialProperty("_FurZClip", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furZwrite              = new lilMaterialProperty("_FurZWrite", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furZtest               = new lilMaterialProperty("_FurZTest", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furOffsetFactor        = new lilMaterialProperty("_FurOffsetFactor", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furOffsetUnits         = new lilMaterialProperty("_FurOffsetUnits", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furColorMask           = new lilMaterialProperty("_FurColorMask", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furAlphaToMask         = new lilMaterialProperty("_FurAlphaToMask", PropertyBlock.Rendering);

        private readonly lilMaterialProperty tessEdge               = new lilMaterialProperty("_TessEdge", PropertyBlock.Tessellation);
        private readonly lilMaterialProperty tessStrength           = new lilMaterialProperty("_TessStrength", PropertyBlock.Tessellation);
        private readonly lilMaterialProperty tessShrink             = new lilMaterialProperty("_TessShrink", PropertyBlock.Tessellation);
        private readonly lilMaterialProperty tessFactorMax          = new lilMaterialProperty("_TessFactorMax", PropertyBlock.Tessellation);

        private readonly lilMaterialProperty transparentModeMat     = new lilMaterialProperty("_TransparentMode", PropertyBlock.Base);
        private readonly lilMaterialProperty useClippingCanceller   = new lilMaterialProperty("_UseClippingCanceller", PropertyBlock.Base);
        private readonly lilMaterialProperty asOverlay              = new lilMaterialProperty("_AsOverlay", PropertyBlock.Base);
        private readonly lilMaterialProperty triMask                = new lilMaterialProperty("_TriMask", true, PropertyBlock.Base);
        private readonly lilMaterialProperty matcapMul              = new lilMaterialProperty("_MatCapMul", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty fakeShadowVector       = new lilMaterialProperty("_FakeShadowVector", PropertyBlock.Base);

        private lilMaterialProperty[] AllProperties()
        {
            return new[]
            {
                invisible,
                cutoff,
                preColor,
                preOutType,
                preCutoff,
                flipNormal,
                backfaceForceShadow,
                backfaceColor,

                asUnlit,
                vertexLightStrength,
                lightMinLimit,
                lightMaxLimit,
                beforeExposureLimit,
                monochromeLighting,
                alphaBoostFA,
                lilDirectionalLightStrength,
                lightDirectionOverride,

                baseColor,
                baseMap,
                baseColorMap,

                shiftBackfaceUV,
                mainTex_ScrollRotate,

                mainColor,
                mainTex,
                mainTexHSVG,
                mainGradationStrength,
                mainGradationTex,
                mainColorAdjustMask,

                useMain2ndTex,
                mainColor2nd,
                main2ndTex,
                main2ndTex_UVMode,
                main2ndTex_Cull,
                main2ndTexAngle,
                main2ndTexDecalAnimation,
                main2ndTexDecalSubParam,
                main2ndTexIsDecal,
                main2ndTexIsLeftOnly,
                main2ndTexIsRightOnly,
                main2ndTexShouldCopy,
                main2ndTexShouldFlipMirror,
                main2ndTexShouldFlipCopy,
                main2ndTexIsMSDF,
                main2ndBlendMask,
                main2ndTexBlendMode,
                main2ndEnableLighting,
                main2ndDissolveMask,
                main2ndDissolveNoiseMask,
                main2ndDissolveNoiseMask_ScrollRotate,
                main2ndDissolveNoiseStrength,
                main2ndDissolveColor,
                main2ndDissolveParams,
                main2ndDissolvePos,
                main2ndDistanceFade,

                useMain3rdTex,
                mainColor3rd,
                main3rdTex,
                main3rdTex_UVMode,
                main3rdTex_Cull,
                main3rdTexAngle,
                main3rdTexDecalAnimation,
                main3rdTexDecalSubParam,
                main3rdTexIsDecal,
                main3rdTexIsLeftOnly,
                main3rdTexIsRightOnly,
                main3rdTexShouldCopy,
                main3rdTexShouldFlipMirror,
                main3rdTexShouldFlipCopy,
                main3rdTexIsMSDF,
                main3rdBlendMask,
                main3rdTexBlendMode,
                main3rdEnableLighting,
                main3rdDissolveMask,
                main3rdDissolveNoiseMask,
                main3rdDissolveNoiseMask_ScrollRotate,
                main3rdDissolveNoiseStrength,
                main3rdDissolveColor,
                main3rdDissolveParams,
                main3rdDissolvePos,
                main3rdDistanceFade,

                alphaMaskMode,
                alphaMask,
                alphaMaskScale,
                alphaMaskValue,

                useShadow,
                shadowStrength,
                shadowStrengthMask,
                shadowBorderMask,
                shadowBlurMask,
                shadowStrengthMaskLOD,
                shadowBorderMaskLOD,
                shadowBlurMaskLOD,
                shadowAOShift,
                shadowAOShift2,
                shadowPostAO,
                shadowColorType,
                shadowColor,
                shadowColorTex,
                shadowNormalStrength,
                shadowBorder,
                shadowBlur,
                shadow2ndColor,
                shadow2ndColorTex,
                shadow2ndNormalStrength,
                shadow2ndBorder,
                shadow2ndBlur,
                shadow3rdColor,
                shadow3rdColorTex,
                shadow3rdNormalStrength,
                shadow3rdBorder,
                shadow3rdBlur,
                shadowMainStrength,
                shadowEnvStrength,
                shadowBorderColor,
                shadowBorderRange,
                shadowReceive,
                shadow2ndReceive,
                shadow3rdReceive,
                shadowMaskType,
                shadowFlatBorder,
                shadowFlatBlur,
                lilShadowCasterBias,

                useEmission,
                emissionColor,
                emissionMap,
                emissionMap_ScrollRotate,
                emissionMap_UVMode,
                emissionMainStrength,
                emissionBlend,
                emissionBlendMask,
                emissionBlendMask_ScrollRotate,
                emissionBlink,
                emissionUseGrad,
                emissionGradTex,
                emissionGradSpeed,
                emissionParallaxDepth,
                emissionFluorescence,

                useEmission2nd,
                emission2ndColor,
                emission2ndMap,
                emission2ndMap_ScrollRotate,
                emission2ndMap_UVMode,
                emission2ndMainStrength,
                emission2ndBlend,
                emission2ndBlendMask,
                emission2ndBlendMask_ScrollRotate,
                emission2ndBlink,
                emission2ndUseGrad,
                emission2ndGradTex,
                emission2ndGradSpeed,
                emission2ndParallaxDepth,
                emission2ndFluorescence,

                useBumpMap,
                bumpMap,
                bumpScale,

                useBump2ndMap,
                bump2ndMap,
                bump2ndMap_UVMode,
                bump2ndScale,
                bump2ndScaleMask,

                useAnisotropy,
                anisotropyTangentMap,
                anisotropyScale,
                anisotropyScaleMask,
                anisotropyTangentWidth,
                anisotropyBitangentWidth,
                anisotropyShift,
                anisotropyShiftNoiseScale,
                anisotropySpecularStrength,
                anisotropy2ndTangentWidth,
                anisotropy2ndBitangentWidth,
                anisotropy2ndShift,
                anisotropy2ndShiftNoiseScale,
                anisotropy2ndSpecularStrength,
                anisotropyShiftNoiseMask,
                anisotropy2Reflection,
                anisotropy2MatCap,
                anisotropy2MatCap2nd,

                useBacklight,
                backlightColor,
                backlightColorTex,
                backlightMainStrength,
                backlightNormalStrength,
                backlightBorder,
                backlightBlur,
                backlightDirectivity,
                backlightViewStrength,
                backlightReceiveShadow,
                backlightBackfaceMask,

                useReflection,
                metallic,
                metallicGlossMap,
                smoothness,
                smoothnessTex,
                reflectance,
                reflectionColor,
                reflectionColorTex,
                gsaaStrength,
                applySpecular,
                applySpecularFA,
                specularNormalStrength,
                specularToon,
                specularBorder,
                specularBlur,
                applyReflection,
                reflectionNormalStrength,
                reflectionApplyTransparency,
                reflectionCubeTex,
                reflectionCubeColor,
                reflectionCubeOverride,
                reflectionCubeEnableLighting,
                reflectionBlendMode,

                useMatCap,
                matcapTex,
                matcapColor,
                matcapMainStrength,
                matcapBlendUV1,
                matcapZRotCancel,
                matcapPerspective,
                matcapVRParallaxStrength,
                matcapBlend,
                matcapBlendMask,
                matcapEnableLighting,
                matcapShadowMask,
                matcapBackfaceMask,
                matcapLod,
                matcapBlendMode,
                matcapApplyTransparency,
                matcapNormalStrength,
                matcapCustomNormal,
                matcapBumpMap,
                matcapBumpScale,

                useMatCap2nd,
                matcap2ndTex,
                matcap2ndColor,
                matcap2ndMainStrength,
                matcap2ndBlendUV1,
                matcap2ndZRotCancel,
                matcap2ndPerspective,
                matcap2ndVRParallaxStrength,
                matcap2ndBlend,
                matcap2ndBlendMask,
                matcap2ndEnableLighting,
                matcap2ndShadowMask,
                matcap2ndBackfaceMask,
                matcap2ndLod,
                matcap2ndBlendMode,
                matcap2ndApplyTransparency,
                matcap2ndNormalStrength,
                matcap2ndCustomNormal,
                matcap2ndBumpMap,
                matcap2ndBumpScale,

                useRim,
                rimColor,
                rimColorTex,
                rimMainStrength,
                rimNormalStrength,
                rimBorder,
                rimBlur,
                rimFresnelPower,
                rimEnableLighting,
                rimShadowMask,
                rimBackfaceMask,
                rimVRParallaxStrength,
                rimApplyTransparency,
                rimDirStrength,
                rimDirRange,
                rimIndirRange,
                rimIndirColor,
                rimIndirBorder,
                rimIndirBlur,

                useGlitter,
                glitterUVMode,
                glitterColor,
                glitterColorTex,
                glitterColorTex_UVMode,
                glitterMainStrength,
                glitterScaleRandomize,
                glitterApplyShape,
                glitterShapeTex,
                glitterAtras,
                glitterAngleRandomize,
                glitterParams1,
                glitterParams2,
                glitterPostContrast,
                glitterSensitivity,
                glitterEnableLighting,
                glitterShadowMask,
                glitterBackfaceMask,
                glitterApplyTransparency,
                glitterVRParallaxStrength,
                glitterNormalStrength,

                gemChromaticAberration,
                gemEnvContrast,
                gemEnvColor,
                gemParticleLoop,
                gemParticleColor,
                gemVRParallaxStrength,

                outlineColor,
                outlineTex,
                outlineTex_ScrollRotate,
                outlineTexHSVG,
                outlineLitColor,
                outlineLitApplyTex,
                outlineLitScale,
                outlineLitOffset,
                outlineLitShadowReceive,
                outlineWidth,
                outlineWidthMask,
                outlineFixWidth,
                outlineVertexR2Width,
                outlineDeleteMesh,
                outlineVectorTex,
                outlineVectorUVMode,
                outlineVectorScale,
                outlineEnableLighting,
                outlineZBias,
                outlineDisableInVR,

                useParallax,
                usePOM,
                parallaxMap,
                parallax,
                parallaxOffset,

                distanceFade,
                distanceFadeColor,

                useAudioLink,
                audioLinkDefaultValue,
                audioLinkUVMode,
                audioLinkUVParams,
                audioLinkStart,
                audioLinkMask,
                audioLink2Main2nd,
                audioLink2Main3rd,
                audioLink2Emission,
                audioLink2EmissionGrad,
                audioLink2Emission2nd,
                audioLink2Emission2ndGrad,
                audioLink2Vertex,
                audioLinkVertexUVMode,
                audioLinkVertexUVParams,
                audioLinkVertexStart,
                audioLinkVertexStrength,
                audioLinkAsLocal,
                audioLinkLocalMap,
                audioLinkLocalMapParams,

                dissolveMask,
                dissolveNoiseMask,
                dissolveNoiseMask_ScrollRotate,
                dissolveNoiseStrength,
                dissolveColor,
                dissolveParams,
                dissolvePos,

                ignoreEncryption,
                keys,

                refractionStrength,
                refractionFresnelPower,
                refractionColorFromMain,
                refractionColor,

                furNoiseMask,
                furMask,
                furLengthMask,
                furVectorTex,
                furVectorScale,
                furVector,
                furGravity,
                furRandomize,
                furAO,
                vertexColor2FurVector,
                furMeshType,
                furLayerNum,
                furRootOffset,
                furCutoutLength,
                furTouchStrength,

                stencilRef,
                stencilReadMask,
                stencilWriteMask,
                stencilComp,
                stencilPass,
                stencilFail,
                stencilZFail,
                preStencilRef,
                preStencilReadMask,
                preStencilWriteMask,
                preStencilComp,
                preStencilPass,
                preStencilFail,
                preStencilZFail,
                outlineStencilRef,
                outlineStencilReadMask,
                outlineStencilWriteMask,
                outlineStencilComp,
                outlineStencilPass,
                outlineStencilFail,
                outlineStencilZFail,
                furStencilRef,
                furStencilReadMask,
                furStencilWriteMask,
                furStencilComp,
                furStencilPass,
                furStencilFail,
                furStencilZFail,

                subpassCutoff,
                cull,
                srcBlend,
                dstBlend,
                srcBlendAlpha,
                dstBlendAlpha,
                blendOp,
                blendOpAlpha,
                srcBlendFA,
                dstBlendFA,
                srcBlendAlphaFA,
                dstBlendAlphaFA,
                blendOpFA,
                blendOpAlphaFA,
                zclip,
                zwrite,
                ztest,
                offsetFactor,
                offsetUnits,
                colorMask,
                alphaToMask,

                preCull,
                preSrcBlend,
                preDstBlend,
                preSrcBlendAlpha,
                preDstBlendAlpha,
                preBlendOp,
                preBlendOpAlpha,
                preSrcBlendFA,
                preDstBlendFA,
                preSrcBlendAlphaFA,
                preDstBlendAlphaFA,
                preBlendOpFA,
                preBlendOpAlphaFA,
                preZclip,
                preZwrite,
                preZtest,
                preOffsetFactor,
                preOffsetUnits,
                preColorMask,
                preAlphaToMask,

                outlineCull,
                outlineSrcBlend,
                outlineDstBlend,
                outlineSrcBlendAlpha,
                outlineDstBlendAlpha,
                outlineBlendOp,
                outlineBlendOpAlpha,
                outlineSrcBlendFA,
                outlineDstBlendFA,
                outlineSrcBlendAlphaFA,
                outlineDstBlendAlphaFA,
                outlineBlendOpFA,
                outlineBlendOpAlphaFA,
                outlineZclip,
                outlineZwrite,
                outlineZtest,
                outlineOffsetFactor,
                outlineOffsetUnits,
                outlineColorMask,
                outlineAlphaToMask,

                furCull,
                furSrcBlend,
                furDstBlend,
                furSrcBlendAlpha,
                furDstBlendAlpha,
                furBlendOp,
                furBlendOpAlpha,
                furSrcBlendFA,
                furDstBlendFA,
                furSrcBlendAlphaFA,
                furDstBlendAlphaFA,
                furBlendOpFA,
                furBlendOpAlphaFA,
                furZclip,
                furZwrite,
                furZtest,
                furOffsetFactor,
                furOffsetUnits,
                furColorMask,
                furAlphaToMask,

                tessEdge,
                tessStrength,
                tessShrink,
                tessFactorMax,

                transparentModeMat,
                useClippingCanceller,
                asOverlay,
                triMask,
                matcapMul,
                fakeShadowVector,
            };
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Main GUI
        #region
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            isCustomEditor = false;
            isMultiVariants = false;
            DrawAllGUI(materialEditor, props, (Material)materialEditor.target);
        }

        public void DrawAllGUI(MaterialEditor materialEditor, MaterialProperty[] props, Material material)
        {
            //------------------------------------------------------------------------------------------------------------------------------
            // EditorAssets
            lilEditorGUI.InitializeGUIStyles();

            //------------------------------------------------------------------------------------------------------------------------------
            // Initialize Setting
            m_MaterialEditor = materialEditor;
            ApplyEditorSettingTemp();
            lilShaderManager.InitializeShaders();
            lilToonSetting.InitializeShaderSetting(ref shaderSetting);

            //------------------------------------------------------------------------------------------------------------------------------
            // Load Properties
            foreach(lilMaterialProperty prop in AllProperties()) prop.FindProperty(props);

            //------------------------------------------------------------------------------------------------------------------------------
            // Check Shader Type
            CheckShaderType(material);

            //------------------------------------------------------------------------------------------------------------------------------
            // Load Custom Properties
            LoadCustomProperties(props, material);

            //------------------------------------------------------------------------------------------------------------------------------
            // Info
            EditorGUI.BeginChangeCheck();
            DrawWebPages();
            DrawHelpPages();

            //------------------------------------------------------------------------------------------------------------------------------
            // Language
            lilLanguageManager.SelectLang();
            sMainColorBranch = isUseAlpha ? GetLoc("sMainColorAlpha") : GetLoc("sMainColor");
            mainColorRGBAContent = isUseAlpha ? colorAlphaRGBAContent : colorRGBAContent;

            //------------------------------------------------------------------------------------------------------------------------------
            // Editor Mode
            SelectEditorMode();
            DrawShaderTypeWarn(material);
            EditorGUILayout.Space();

            //------------------------------------------------------------------------------------------------------------------------------
            // Main GUI
            switch(edSet.editorMode)
            {
                case EditorMode.Simple:     DrawSimpleGUI(material); break;
                case EditorMode.Advanced:   DrawAdvancedGUI(material); break;
                case EditorMode.Preset:     DrawPresetGUI(); break;
                case EditorMode.Settings:   DrawSettingsGUI(); break;
            }

            if(EditorGUI.EndChangeCheck())
            {
                material.SetFloat("_lilToonVersion", lilConstants.currentVersionValue);
                if(!isMultiVariants)
                {
                    if(isMulti) lilMaterialUtils.SetupMultiMaterial(material);
                    else        lilMaterialUtils.RemoveShaderKeywords(material);
                }
                if(mainColor != null && baseColor    != null) baseColor.colorValue      = mainColor.colorValue;
                if(mainTex   != null && baseMap      != null) baseMap.textureValue      = mainTex.textureValue;
                if(mainTex   != null && baseColorMap != null) baseColorMap.textureValue = mainTex.textureValue;

                if(lilShaderAPI.IsTextureLimitedAPI())
                {
                    string shaderSettingString = lilToonSetting.BuildShaderSettingString(shaderSetting, true);
                    lilToonSetting.CheckTextures(ref shaderSetting, material);
                    string newShaderSettingString = lilToonSetting.BuildShaderSettingString(shaderSetting, true);
                    if(shaderSettingString != newShaderSettingString)
                    {
                        lilToonSetting.ApplyShaderSetting(shaderSetting);
                    }
                }

                SaveEditorSettingTemp();
            }
        }

        private void DrawSimpleGUI(Material material)
        {
            //------------------------------------------------------------------------------------------------------------------------------
            // Base Setting
            DrawBaseSettings(material, sCullModes, sTransparentMode, sRenderingModeList, sRenderingModeListLite, sTransparentModeList);

            //------------------------------------------------------------------------------------------------------------------------------
            // Lighting
            if(!isFakeShadow) DrawLightingSettingsSimple();

            //------------------------------------------------------------------------------------------------------------------------------
            // VRChat
            DrawVRCFallbackGUI(material);

            //------------------------------------------------------------------------------------------------------------------------------
            // Custom Properties
            if(isCustomShader && !isFakeShadow)
            {
                EditorGUILayout.Space();
                GUILayout.Label(GetLoc("sCustomProperties"), boldLabel);
                DrawCustomProperties(material);
            }

            EditorGUILayout.Space();

            //------------------------------------------------------------------------------------------------------------------------------
            // Colors
            GUILayout.Label(GetLoc("sColors"), boldLabel);

            edSet.isShowMain = lilEditorGUI.Foldout(GetLoc("sMainColorSettingSimple"), edSet.isShowMain);
            if(edSet.isShowMain)
            {
                if(isLite || isGem || isFakeShadow)
                {
                    // Main
                    //if(useMainTex.floatValue == 1)
                    //{
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(sMainColorBranch, customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        m_MaterialEditor.TexturePropertySingleLine(mainColorRGBAContent, mainTex, mainColor);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    //}
                }
                else
                {
                    // Main
                    //if(useMainTex.floatValue == 1)
                    //{
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(sMainColorBranch, customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        m_MaterialEditor.TexturePropertySingleLine(mainColorRGBAContent, mainTex, mainColor);
                        lilEditorGUI.DrawLine();
                        DrawMainAdjustSettings(material);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    //}

                    // Main 2nd
                    if(useMain2ndTex.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainColor2nd"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        m_MaterialEditor.TexturePropertySingleLine(colorRGBAContent, main2ndTex, mainColor2nd);
                        lilEditorGUI.DrawColorAsAlpha(mainColor2nd);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    // Main 3rd
                    if(useMain3rdTex.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainColor3rd"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        m_MaterialEditor.TexturePropertySingleLine(colorRGBAContent, main3rdTex, mainColor3rd);
                        lilEditorGUI.DrawColorAsAlpha(mainColor3rd);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                if(!isFakeShadow && !isLite) DrawAlphaMaskSettings(material);
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Shadow
            if((!isFakeShadow && !isGem) || isLite)
            {
                DrawShadowSettingsSimple();
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Emission
            if(isLite)
            {
                edSet.isShowEmission = lilEditorGUI.Foldout(GetLoc("sEmissionSetting"), edSet.isShowEmission);
                DrawMenuButton(GetLoc("sAnchorEmission"), PropertyBlock.Emission);
                if(edSet.isShowEmission)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useEmission, GetLoc("sEmission"));
                    if(useEmission.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        m_MaterialEditor.TexturePropertySingleLine(colorMaskRGBAContent, emissionMap);
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            else if(!isFakeShadow)
            {
                edSet.isShowEmission = lilEditorGUI.Foldout(GetLoc("sEmissionSetting"), edSet.isShowEmission);
                DrawMenuButton(GetLoc("sAnchorEmission"), PropertyBlock.Emission);
                if(edSet.isShowEmission)
                {
                    // Emission
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useEmission, GetLoc("sEmission"));
                    DrawMenuButton(GetLoc("sAnchorEmission"), PropertyBlock.Emission1st);
                    if(useEmission.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        TextureGUI(ref edSet.isShowEmissionMap, colorMaskRGBAContent, emissionMap, emissionColor, emissionMap_ScrollRotate, emissionMap_UVMode, true, true);
                        lilEditorGUI.DrawColorAsAlpha(emissionColor);
                        m_MaterialEditor.ShaderProperty(emissionMainStrength, GetLoc("sMainColorPower"));
                        lilEditorGUI.DrawLine();
                        TextureGUI(ref edSet.isShowEmissionBlendMask, maskBlendRGBAContent, emissionBlendMask, emissionBlend, emissionBlendMask_ScrollRotate, true, true);
                        lilEditorGUI.DrawLine();
                        m_MaterialEditor.ShaderProperty(emissionFluorescence, GetLoc("sFluorescence"));
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();

                    // Emission 2nd
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useEmission2nd, GetLoc("sEmission2nd"));
                    DrawMenuButton(GetLoc("sAnchorEmission"), PropertyBlock.Emission2nd);
                    if(useEmission2nd.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        TextureGUI(ref edSet.isShowEmission2ndMap, colorMaskRGBAContent, emission2ndMap, emission2ndColor, emission2ndMap_ScrollRotate, emission2ndMap_UVMode, true, true);
                        lilEditorGUI.DrawColorAsAlpha(emission2ndColor);
                        m_MaterialEditor.ShaderProperty(emission2ndMainStrength, GetLoc("sMainColorPower"));
                        lilEditorGUI.DrawLine();
                        TextureGUI(ref edSet.isShowEmission2ndBlendMask, maskBlendRGBAContent, emission2ndBlendMask, emission2ndBlend, emission2ndBlendMask_ScrollRotate, true, true);
                        lilEditorGUI.DrawLine();
                        m_MaterialEditor.ShaderProperty(emission2ndFluorescence, GetLoc("sFluorescence"));
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }
            }

            EditorGUILayout.Space();

            //------------------------------------------------------------------------------------------------------------------------------
            // Normal & Reflection
            if(!isFakeShadow)
            {
                GUILayout.Label(GetLoc("sNormalMapReflection"), boldLabel);
                if(!isLite)
                {
                    edSet.isShowBump = lilEditorGUI.Foldout(GetLoc("sNormalMapSetting"), edSet.isShowBump);
                    DrawMenuButton(GetLoc("sAnchorNormalMap"), PropertyBlock.NormalMap);
                    if(edSet.isShowBump)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // 1st
                        EditorGUILayout.BeginVertical(boxOuter);
                        m_MaterialEditor.ShaderProperty(useBumpMap, GetLoc("sNormalMap"));
                        DrawMenuButton(GetLoc("sAnchorNormalMap"), PropertyBlock.NormalMap1st);
                        if(useBumpMap.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            TextureGUI(ref edSet.isShowBumpMap, normalMapContent, bumpMap, bumpScale);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();

                        //------------------------------------------------------------------------------------------------------------------------------
                        // 2nd
                        EditorGUILayout.BeginVertical(boxOuter);
                        m_MaterialEditor.ShaderProperty(useBump2ndMap, GetLoc("sNormalMap2nd"));
                        DrawMenuButton(GetLoc("sAnchorNormalMap"), PropertyBlock.NormalMap2nd);
                        if(useBump2ndMap.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            TextureGUI(ref edSet.isShowBump2ndMap, normalMapContent, bump2ndMap, bump2ndScale, bump2ndMap_UVMode, "UV Mode|UV0|UV1|UV2|UV3");
                            lilEditorGUI.DrawLine();
                            TextureGUI(ref edSet.isShowBump2ndScaleMask, maskStrengthContent, bump2ndScaleMask);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                DrawMatCapSettingsSimple();
                DrawRimSettingsSimple();
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Outline
            DrawOutlineSettingsSimple(material);

            if(mtoon != null && lilEditorGUI.EditorButton(GetLoc("sConvertMToon"))) CreateMToonMaterial(material);
        }

        private void DrawAdvancedGUI(Material material)
        {
            if(isLite)
            {
                //------------------------------------------------------------------------------------------------------------------------------
                // Base Setting
                DrawBaseSettings(material, sCullModes, sTransparentMode, sRenderingModeList, sRenderingModeListLite, sTransparentModeList);

                //------------------------------------------------------------------------------------------------------------------------------
                // Lighting
                DrawLightingSettings();

                //------------------------------------------------------------------------------------------------------------------------------
                // UV
                edSet.isShowMainUV = lilEditorGUI.Foldout(GetLoc("sMainUV"), edSet.isShowMainUV);
                DrawMenuButton(GetLoc("sAnchorUVSetting"), PropertyBlock.UV);
                if(edSet.isShowMainUV)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    EditorGUILayout.LabelField(GetLoc("sMainUV"), customToggleFont);
                    DrawMenuButton(GetLoc("sAnchorUVSetting"), PropertyBlock.UV);
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    UVSettingGUI(mainTex, mainTex_ScrollRotate);
                    m_MaterialEditor.ShaderProperty(shiftBackfaceUV, GetLoc("sShiftBackfaceUV"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // VRChat
                DrawVRCFallbackGUI(material);

                //------------------------------------------------------------------------------------------------------------------------------
                // Custom Properties
                if(isCustomShader)
                {
                    EditorGUILayout.Space();
                    GUILayout.Label(GetLoc("sCustomProperties"), boldLabel);
                    DrawCustomProperties(material);
                }

                EditorGUILayout.Space();

                //------------------------------------------------------------------------------------------------------------------------------
                // Colors
                GUILayout.Label(GetLoc("sColors"), boldLabel);

                //------------------------------------------------------------------------------------------------------------------------------
                // Main Color
                edSet.isShowMain = lilEditorGUI.Foldout(GetLoc("sMainColorSetting"), edSet.isShowMain);
                DrawMenuButton(GetLoc("sAnchorMainColor"), PropertyBlock.MainColor);
                if(edSet.isShowMain)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // Main
                    EditorGUILayout.BeginVertical(boxOuter);
                    EditorGUILayout.LabelField(sMainColorBranch, customToggleFont);
                    DrawMenuButton(GetLoc("sAnchorMainColor1"), PropertyBlock.MainColor1st);
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    m_MaterialEditor.TexturePropertySingleLine(mainColorRGBAContent, mainTex, mainColor);
                    if(isUseAlpha) lilEditorGUI.SetAlphaIsTransparencyGUI(mainTex);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Shadow
                DrawShadowSettings();

                //------------------------------------------------------------------------------------------------------------------------------
                // Emission
                edSet.isShowEmission = lilEditorGUI.Foldout(GetLoc("sEmissionSetting"), edSet.isShowEmission);
                DrawMenuButton(GetLoc("sAnchorEmission"), PropertyBlock.Emission);
                if(edSet.isShowEmission)
                {
                    // Emission
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useEmission, GetLoc("sEmission"));
                    DrawMenuButton(GetLoc("sAnchorEmission"), PropertyBlock.Emission1st);
                    if(useEmission.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        TextureGUI(ref edSet.isShowEmissionMap, colorMaskRGBAContent, emissionMap, emissionColor, emissionMap_ScrollRotate, emissionMap_UVMode, true, true);
                        lilEditorGUI.DrawLine();
                        m_MaterialEditor.ShaderProperty(emissionBlink, blinkSetting);
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.Space();

                //------------------------------------------------------------------------------------------------------------------------------
                // Normal & Reflection
                GUILayout.Label(GetLoc("sNormalMapReflection"), boldLabel);

                //------------------------------------------------------------------------------------------------------------------------------
                // MatCap
                DrawMatCapSettings();

                //------------------------------------------------------------------------------------------------------------------------------
                // Rim
                DrawRimSettings();

                EditorGUILayout.Space();

                //------------------------------------------------------------------------------------------------------------------------------
                // Advanced
                GUILayout.Label(GetLoc("sAdvanced"), boldLabel);

                //------------------------------------------------------------------------------------------------------------------------------
                // Outline
                DrawOutlineSettings(material);

                //------------------------------------------------------------------------------------------------------------------------------
                // Stencil
                DrawStencilSettings(material);

                //------------------------------------------------------------------------------------------------------------------------------
                // Rendering
                edSet.isShowRendering = lilEditorGUI.Foldout(GetLoc("sRenderingSetting"), edSet.isShowRendering);
                DrawMenuButton(GetLoc("sAnchorRendering"), PropertyBlock.Rendering);
                if(edSet.isShowRendering)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // Reset Button
                    if(lilEditorGUI.EditorButton(GetLoc("sRenderingReset")))
                    {
                        material.enableInstancing = false;
                        SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf);
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Base
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sRenderingSetting"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInner);
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Shader
                        int shaderType = 1;
                        int shaderTypeBuf = shaderType;
                        shaderType = EditorGUILayout.Popup(GetLoc("sShaderType"),shaderType,new string[]{GetLoc("sShaderTypeNormal"),GetLoc("sShaderTypeLite")});
                        if(shaderTypeBuf != shaderType)
                        {
                            if(shaderType==0) isLite = false;
                            if(shaderType==1) isLite = true;
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf);
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Rendering
                        if(renderingModeBuf == RenderingMode.Transparent || renderingModeBuf == RenderingMode.Fur || renderingModeBuf == RenderingMode.FurTwoPass)
                        {
                            m_MaterialEditor.ShaderProperty(subpassCutoff, GetLoc("sSubpassCutoff"));
                        }
                        m_MaterialEditor.ShaderProperty(cull, sCullModes);
                        m_MaterialEditor.ShaderProperty(zclip, GetLoc("sZClip"));
                        m_MaterialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                        m_MaterialEditor.ShaderProperty(ztest, GetLoc("sZTest"));
                        m_MaterialEditor.ShaderProperty(offsetFactor, GetLoc("sOffsetFactor"));
                        m_MaterialEditor.ShaderProperty(offsetUnits, GetLoc("sOffsetUnits"));
                        m_MaterialEditor.ShaderProperty(colorMask, GetLoc("sColorMask"));
                        m_MaterialEditor.ShaderProperty(alphaToMask, GetLoc("sAlphaToMask"));
                        m_MaterialEditor.ShaderProperty(lilShadowCasterBias, "Shadow Caster Bias");
                        lilEditorGUI.DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlend, GetLoc("sForward"), srcBlend, dstBlend, srcBlendAlpha, dstBlendAlpha, blendOp, blendOpAlpha);
                        lilEditorGUI.DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlendAdd, GetLoc("sForwardAdd"), srcBlendFA, dstBlendFA, srcBlendAlphaFA, dstBlendAlphaFA, blendOpFA, blendOpAlphaFA);
                        lilEditorGUI.DrawLine();
                        m_MaterialEditor.EnableInstancingField();
                        m_MaterialEditor.DoubleSidedGIField();
                        m_MaterialEditor.RenderQueueField();
                        m_MaterialEditor.ShaderProperty(beforeExposureLimit, GetLoc("sBeforeExposureLimit"));
                        m_MaterialEditor.ShaderProperty(lilDirectionalLightStrength, GetLoc("sDirectionalLightStrength"));
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Pre
                    if(transparentModeBuf == TransparentMode.TwoPass)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField("PrePass", customToggleFont);
                        EditorGUILayout.BeginVertical(boxInner);
                        m_MaterialEditor.ShaderProperty(preCull, sCullModes);
                        m_MaterialEditor.ShaderProperty(preZclip, GetLoc("sZClip"));
                        m_MaterialEditor.ShaderProperty(preZwrite, GetLoc("sZWrite"));
                        m_MaterialEditor.ShaderProperty(preZtest, GetLoc("sZTest"));
                        m_MaterialEditor.ShaderProperty(preOffsetFactor, GetLoc("sOffsetFactor"));
                        m_MaterialEditor.ShaderProperty(preOffsetUnits, GetLoc("sOffsetUnits"));
                        m_MaterialEditor.ShaderProperty(preColorMask, GetLoc("sColorMask"));
                        m_MaterialEditor.ShaderProperty(preAlphaToMask, GetLoc("sAlphaToMask"));
                        lilEditorGUI.DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlendPre, GetLoc("sForward"), preSrcBlend, preDstBlend, preSrcBlendAlpha, preDstBlendAlpha, preBlendOp, preBlendOpAlpha);
                        lilEditorGUI.DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlendAddPre, GetLoc("sForwardAdd"), preSrcBlendFA, preDstBlendFA, preSrcBlendAlphaFA, preDstBlendAlphaFA, preBlendOpFA, preBlendOpAlphaFA);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Outline
                    if(isOutl)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sOutline"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInner);
                        m_MaterialEditor.ShaderProperty(outlineCull, sCullModes);
                        m_MaterialEditor.ShaderProperty(outlineZclip, GetLoc("sZClip"));
                        m_MaterialEditor.ShaderProperty(outlineZwrite, GetLoc("sZWrite"));
                        m_MaterialEditor.ShaderProperty(outlineZtest, GetLoc("sZTest"));
                        m_MaterialEditor.ShaderProperty(outlineOffsetFactor, GetLoc("sOffsetFactor"));
                        m_MaterialEditor.ShaderProperty(outlineOffsetUnits, GetLoc("sOffsetUnits"));
                        m_MaterialEditor.ShaderProperty(outlineColorMask, GetLoc("sColorMask"));
                        m_MaterialEditor.ShaderProperty(outlineAlphaToMask, GetLoc("sAlphaToMask"));
                        lilEditorGUI.DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlendOutline, GetLoc("sForward"), outlineSrcBlend, outlineDstBlend, outlineSrcBlendAlpha, outlineDstBlendAlpha, outlineBlendOp, outlineBlendOpAlpha);
                        lilEditorGUI.DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlendAddOutline, GetLoc("sForwardAdd"), outlineSrcBlendFA, outlineDstBlendFA, outlineSrcBlendAlphaFA, outlineDstBlendAlphaFA, outlineBlendOpFA, outlineBlendOpAlphaFA);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Light Bake
                edSet.isShowLightBake = lilEditorGUI.Foldout(GetLoc("sLightBakeSetting"), edSet.isShowLightBake);
                //DrawMenuButton(GetLoc("sAnchorLightBake"), PropertyBlock.LightBake);
                if(edSet.isShowLightBake)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    EditorGUILayout.LabelField(GetLoc("sLightBakeSetting"), customToggleFont);
                    EditorGUILayout.BeginVertical(boxInner);
                    if(!isCustomEditor) m_MaterialEditor.DoubleSidedGIField();
                    if(!isCustomEditor) DrawGIFragGUI();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Optimization
                if(!isMultiVariants)
                {
                    GUILayout.Label(GetLoc("sOptimization"), boldLabel);
                    edSet.isShowOptimization = lilEditorGUI.Foldout(GetLoc("sOptimization"), edSet.isShowOptimization);
                    lilEditorGUI.DrawHelpButton(GetLoc("sAnchorOptimization"));
                    if(edSet.isShowOptimization)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sOptimization"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        DrawOptimizationButton(material, !(isLite && isMulti));
                        lilEditorGUI.RemoveUnusedPropertiesGUI(material);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }
            }
            else if(isFakeShadow)
            {
                //------------------------------------------------------------------------------------------------------------------------------
                // Base Setting
                GUILayout.Label(GetLoc("sBaseSetting"), boldLabel);
                edSet.isShowBase = lilEditorGUI.Foldout(GetLoc("sBaseSetting"), edSet.isShowBase);
                DrawMenuButton(GetLoc("sAnchorBaseSetting"), PropertyBlock.Base);
                if(edSet.isShowBase)
                {
                    EditorGUILayout.BeginVertical(customBox);
                        m_MaterialEditor.ShaderProperty(cull, sCullModes);
                        m_MaterialEditor.ShaderProperty(invisible, GetLoc("sInvisible"));
                        m_MaterialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                        m_MaterialEditor.RenderQueueField();
                        lilEditorGUI.DrawLine();
                        GUILayout.Label("FakeShadow", boldLabel);
                        m_MaterialEditor.ShaderProperty(fakeShadowVector, BuildParams(GetLoc("sVector"), GetLoc("sOffset")));
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // UV
                edSet.isShowMainUV = lilEditorGUI.Foldout(GetLoc("sMainUV"), edSet.isShowMainUV);
                DrawMenuButton(GetLoc("sAnchorUVSetting"), PropertyBlock.UV);
                if(edSet.isShowMainUV)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    EditorGUILayout.LabelField(GetLoc("sMainUV"), customToggleFont);
                    DrawMenuButton(GetLoc("sAnchorUVSetting"), PropertyBlock.UV);
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    m_MaterialEditor.TextureScaleOffsetProperty(mainTex);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // VRChat
                DrawVRCFallbackGUI(material);

                //------------------------------------------------------------------------------------------------------------------------------
                // Custom Properties
                if(isCustomShader)
                {
                    EditorGUILayout.Space();
                    GUILayout.Label(GetLoc("sCustomProperties"), boldLabel);
                    DrawCustomProperties(material);
                }

                EditorGUILayout.Space();

                //------------------------------------------------------------------------------------------------------------------------------
                // Colors
                GUILayout.Label(GetLoc("sColors"), boldLabel);

                //------------------------------------------------------------------------------------------------------------------------------
                // Main Color
                edSet.isShowMain = lilEditorGUI.Foldout(GetLoc("sMainColorSetting"), edSet.isShowMain);
                DrawMenuButton(GetLoc("sAnchorMainColor"), PropertyBlock.MainColor);
                if(edSet.isShowMain)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // Main
                    EditorGUILayout.BeginVertical(boxOuter);
                    EditorGUILayout.LabelField(sMainColorBranch, customToggleFont);
                    DrawMenuButton(GetLoc("sAnchorMainColor1"), PropertyBlock.MainColor1st);
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    m_MaterialEditor.TexturePropertySingleLine(mainColorRGBAContent, mainTex, mainColor);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.Space();

                //------------------------------------------------------------------------------------------------------------------------------
                // Advanced
                GUILayout.Label(GetLoc("sAdvanced"), boldLabel);

                //------------------------------------------------------------------------------------------------------------------------------
                // Encryption
                if(lilDirectoryManager.ExistsEncryption())
                {
                    edSet.isShowEncryption = lilEditorGUI.Foldout(GetLoc("sEncryption"), edSet.isShowEncryption);
                    DrawMenuButton(GetLoc("sAnchorEncryption"), PropertyBlock.Encryption);
                    if(edSet.isShowEncryption)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sEncryption"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorEncryption"), PropertyBlock.Encryption);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        m_MaterialEditor.ShaderProperty(ignoreEncryption, GetLoc("sIgnoreEncryption"));
                        m_MaterialEditor.ShaderProperty(keys, GetLoc("sKeys"));
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Stencil
                edSet.isShowStencil = lilEditorGUI.Foldout(GetLoc("sStencilSetting"), edSet.isShowStencil);
                DrawMenuButton(GetLoc("sAnchorStencil"), PropertyBlock.Stencil);
                if(edSet.isShowStencil)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    EditorGUILayout.LabelField(GetLoc("sStencilSetting"), customToggleFont);
                    DrawMenuButton(GetLoc("sAnchorStencil"), PropertyBlock.Stencil);
                    EditorGUILayout.BeginVertical(boxInner);
                    //------------------------------------------------------------------------------------------------------------------------------
                    // Auto Setting
                    if(lilEditorGUI.EditorButton("Set Writer"))
                    {
                        isStWr = true;
                        stencilRef.floatValue = 51;
                        stencilReadMask.floatValue = 255.0f;
                        stencilWriteMask.floatValue = 255.0f;
                        stencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Equal;
                        stencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Replace;
                        stencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        stencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        material.renderQueue = material.shader.renderQueue - 1;
                        if(renderingModeBuf == RenderingMode.Opaque) material.renderQueue += 450;
                    }
                    if(lilEditorGUI.EditorButton("Set Reader"))
                    {
                        isStWr = false;
                        stencilRef.floatValue = 51;
                        stencilReadMask.floatValue = 255.0f;
                        stencilWriteMask.floatValue = 255.0f;
                        stencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Equal;
                        stencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        stencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        stencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        material.renderQueue = -1;
                        if(renderingModeBuf == RenderingMode.Opaque) material.renderQueue += 450;
                    }
                    if(lilEditorGUI.EditorButton("Reset"))
                    {
                        isStWr = false;
                        stencilRef.floatValue = 51;
                        stencilReadMask.floatValue = 255.0f;
                        stencilWriteMask.floatValue = 255.0f;
                        stencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Equal;
                        stencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        stencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        stencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        material.renderQueue = -1;
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Base
                    {
                        lilEditorGUI.DrawLine();
                        m_MaterialEditor.ShaderProperty(stencilRef, "Ref");
                        m_MaterialEditor.ShaderProperty(stencilReadMask, "ReadMask");
                        m_MaterialEditor.ShaderProperty(stencilWriteMask, "WriteMask");
                        m_MaterialEditor.ShaderProperty(stencilComp, "Comp");
                        m_MaterialEditor.ShaderProperty(stencilPass, "Pass");
                        m_MaterialEditor.ShaderProperty(stencilFail, "Fail");
                        m_MaterialEditor.ShaderProperty(stencilZFail, "ZFail");
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Rendering
                edSet.isShowRendering = lilEditorGUI.Foldout(GetLoc("sRenderingSetting"), edSet.isShowRendering);
                DrawMenuButton(GetLoc("sAnchorRendering"), PropertyBlock.Rendering);
                if(edSet.isShowRendering)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // Reset Button
                    if(lilEditorGUI.EditorButton(GetLoc("sRenderingReset")))
                    {
                        material.enableInstancing = false;
                        material.renderQueue = -1;
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.SetInt("_ZWrite", 1);
                        material.SetInt("_ZTest", 4);
                        material.SetFloat("_OffsetFactor", 0.0f);
                        material.SetFloat("_OffsetUnits", 0.0f);
                        material.SetInt("_ColorMask", 15);
                        material.SetInt("_AlphaToMask", 0);
                        material.SetInt("_SrcBlendAlpha", (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.SetInt("_DstBlendAlpha", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.Add);
                        material.SetInt("_BlendOpAlpha", (int)UnityEngine.Rendering.BlendOp.Add);
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Base
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sRenderingSetting"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInner);
                        m_MaterialEditor.ShaderProperty(cull, sCullModes);
                        m_MaterialEditor.ShaderProperty(zclip, GetLoc("sZClip"));
                        m_MaterialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                        m_MaterialEditor.ShaderProperty(ztest, GetLoc("sZTest"));
                        m_MaterialEditor.ShaderProperty(offsetFactor, GetLoc("sOffsetFactor"));
                        m_MaterialEditor.ShaderProperty(offsetUnits, GetLoc("sOffsetUnits"));
                        m_MaterialEditor.ShaderProperty(colorMask, GetLoc("sColorMask"));
                        m_MaterialEditor.ShaderProperty(alphaToMask, GetLoc("sAlphaToMask"));
                        m_MaterialEditor.ShaderProperty(lilShadowCasterBias, "Shadow Caster Bias");
                        lilEditorGUI.DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlend, GetLoc("sForward"), srcBlend, dstBlend, srcBlendAlpha, dstBlendAlpha, blendOp, blendOpAlpha);
                        lilEditorGUI.DrawLine();
                        m_MaterialEditor.EnableInstancingField();
                        m_MaterialEditor.DoubleSidedGIField();
                        m_MaterialEditor.RenderQueueField();
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Light Bake
                edSet.isShowLightBake = lilEditorGUI.Foldout(GetLoc("sLightBakeSetting"), edSet.isShowLightBake);
                //DrawMenuButton(GetLoc("sAnchorLightBake"), PropertyBlock.LightBake);
                if(edSet.isShowLightBake)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    EditorGUILayout.LabelField(GetLoc("sLightBakeSetting"), customToggleFont);
                    EditorGUILayout.BeginVertical(boxInner);
                    if(!isCustomEditor) m_MaterialEditor.DoubleSidedGIField();
                    if(!isCustomEditor) DrawGIFragGUI();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Optimization
                if(!isMultiVariants)
                {
                    GUILayout.Label(GetLoc("sOptimization"), boldLabel);
                    edSet.isShowOptimization = lilEditorGUI.Foldout(GetLoc("sOptimization"), edSet.isShowOptimization);
                    lilEditorGUI.DrawHelpButton(GetLoc("sAnchorOptimization"));
                    if(edSet.isShowOptimization)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sOptimization"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        DrawOptimizationButton(material, !(isLite && isMulti));
                        lilEditorGUI.RemoveUnusedPropertiesGUI(material);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }
            }
            else
            {
                //------------------------------------------------------------------------------------------------------------------------------
                // Base Setting
                DrawBaseSettings(material, sCullModes, sTransparentMode, sRenderingModeList, sRenderingModeListLite, sTransparentModeList);

                //------------------------------------------------------------------------------------------------------------------------------
                // Lighting
                DrawLightingSettings();

                //------------------------------------------------------------------------------------------------------------------------------
                // UV
                edSet.isShowMainUV = lilEditorGUI.Foldout(GetLoc("sMainUV"), edSet.isShowMainUV);
                DrawMenuButton(GetLoc("sAnchorUVSetting"), PropertyBlock.UV);
                if(edSet.isShowMainUV)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    EditorGUILayout.LabelField(GetLoc("sMainUV"), customToggleFont);
                    DrawMenuButton(GetLoc("sAnchorUVSetting"), PropertyBlock.UV);
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    UVSettingGUI(mainTex, mainTex_ScrollRotate);
                    m_MaterialEditor.ShaderProperty(shiftBackfaceUV, GetLoc("sShiftBackfaceUV"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // VRChat
                DrawVRCFallbackGUI(material);

                //------------------------------------------------------------------------------------------------------------------------------
                // Custom Properties
                if(isCustomShader)
                {
                    EditorGUILayout.Space();
                    GUILayout.Label(GetLoc("sCustomProperties"), boldLabel);
                    DrawCustomProperties(material);
                }

                EditorGUILayout.Space();

                //------------------------------------------------------------------------------------------------------------------------------
                // Colors
                GUILayout.Label(GetLoc("sColors"), boldLabel);

                //------------------------------------------------------------------------------------------------------------------------------
                // Main Color
                edSet.isShowMain = lilEditorGUI.Foldout(GetLoc("sMainColorSetting"), edSet.isShowMain);
                DrawMenuButton(GetLoc("sAnchorMainColor"), PropertyBlock.MainColor);
                if(edSet.isShowMain)
                {
                    if(isGem)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Main
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(sMainColorBranch, customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorMainColor1"), PropertyBlock.MainColor1st);
                        //m_MaterialEditor.ShaderProperty(useMainTex, GetLoc("sMainColor"));
                        //if(useMainTex.floatValue == 1)
                        //{
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            m_MaterialEditor.TexturePropertySingleLine(mainColorRGBAContent, mainTex, mainColor);
                            EditorGUILayout.EndVertical();
                        //}
                        EditorGUILayout.EndVertical();
                    }
                    else
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Main
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(sMainColorBranch, customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorMainColor1"), PropertyBlock.MainColor1st);
                        //m_MaterialEditor.ShaderProperty(useMainTex, GetLoc("sMainColor"));
                        //if(useMainTex.floatValue == 1)
                        //{
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            m_MaterialEditor.TexturePropertySingleLine(mainColorRGBAContent, mainTex, mainColor);
                            if(isUseAlpha) lilEditorGUI.SetAlphaIsTransparencyGUI(mainTex);
                            lilEditorGUI.DrawLine();
                            DrawMainAdjustSettings(material);
                            EditorGUILayout.EndVertical();
                        //}
                        EditorGUILayout.EndVertical();

                        //------------------------------------------------------------------------------------------------------------------------------
                        // 2nd
                        EditorGUILayout.BeginVertical(boxOuter);
                        m_MaterialEditor.ShaderProperty(useMain2ndTex, GetLoc("sMainColor2nd"));
                        DrawMenuButton(GetLoc("sAnchorMainColor2"), PropertyBlock.MainColor2nd);
                        if(useMain2ndTex.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            m_MaterialEditor.TexturePropertySingleLine(colorRGBAContent, main2ndTex, mainColor2nd);
                            EditorGUI.indentLevel += 2;
                            lilEditorGUI.DrawColorAsAlpha(mainColor2nd);
                            m_MaterialEditor.ShaderProperty(main2ndTexIsMSDF, GetLoc("sAsMSDF"));
                            m_MaterialEditor.ShaderProperty(main2ndTex_Cull, sCullModes);
                            EditorGUI.indentLevel -= 2;
                            m_MaterialEditor.ShaderProperty(main2ndEnableLighting, GetLoc("sEnableLighting"));
                            m_MaterialEditor.ShaderProperty(main2ndTexBlendMode, sBlendModes);
                            lilEditorGUI.DrawLine();
                            UV4Decal(main2ndTexIsDecal, main2ndTexIsLeftOnly, main2ndTexIsRightOnly, main2ndTexShouldCopy, main2ndTexShouldFlipMirror, main2ndTexShouldFlipCopy, main2ndTex, main2ndTexAngle, main2ndTexDecalAnimation, main2ndTexDecalSubParam, main2ndTex_UVMode);
                            lilEditorGUI.DrawLine();
                            m_MaterialEditor.TexturePropertySingleLine(maskBlendContent, main2ndBlendMask);
                            EditorGUILayout.LabelField(GetLoc("sDistanceFade"));
                            EditorGUI.indentLevel++;
                            m_MaterialEditor.ShaderProperty(main2ndDistanceFade, sDistanceFadeSetting);
                            EditorGUI.indentLevel--;
                            lilEditorGUI.DrawLine();
                            m_MaterialEditor.ShaderProperty(main2ndDissolveParams, sDissolveParams);
                            if(main2ndDissolveParams.vectorValue.x == 1.0f)                                                TextureGUI(ref edSet.isShowMain2ndDissolveMask, maskBlendContent, main2ndDissolveMask);
                            if(main2ndDissolveParams.vectorValue.x == 2.0f && main2ndDissolveParams.vectorValue.y == 0.0f) m_MaterialEditor.ShaderProperty(main2ndDissolvePos, GetLoc("sPosition") + "|2");
                            if(main2ndDissolveParams.vectorValue.x == 2.0f && main2ndDissolveParams.vectorValue.y == 1.0f) m_MaterialEditor.ShaderProperty(main2ndDissolvePos, GetLoc("sVector") + "|2");
                            if(main2ndDissolveParams.vectorValue.x == 3.0f && main2ndDissolveParams.vectorValue.y == 0.0f) m_MaterialEditor.ShaderProperty(main2ndDissolvePos, GetLoc("sPosition") + "|3");
                            if(main2ndDissolveParams.vectorValue.x == 3.0f && main2ndDissolveParams.vectorValue.y == 1.0f) m_MaterialEditor.ShaderProperty(main2ndDissolvePos, GetLoc("sVector") + "|3");
                            if(main2ndDissolveParams.vectorValue.x != 0.0f)
                            {
                                TextureGUI(ref edSet.isShowMain2ndDissolveNoiseMask, noiseMaskContent, main2ndDissolveNoiseMask, main2ndDissolveNoiseStrength, main2ndDissolveNoiseMask_ScrollRotate);
                                m_MaterialEditor.ShaderProperty(main2ndDissolveColor, GetLoc("sColor"));
                            }
                            lilEditorGUI.DrawLine();
                            TextureBakeGUI(material, 5);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();

                        //------------------------------------------------------------------------------------------------------------------------------
                        // 3rd
                        EditorGUILayout.BeginVertical(boxOuter);
                        m_MaterialEditor.ShaderProperty(useMain3rdTex, GetLoc("sMainColor3rd"));
                        DrawMenuButton(GetLoc("sAnchorMainColor2"), PropertyBlock.MainColor3rd);
                        if(useMain3rdTex.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            m_MaterialEditor.TexturePropertySingleLine(colorRGBAContent, main3rdTex, mainColor3rd);
                            EditorGUI.indentLevel += 2;
                            lilEditorGUI.DrawColorAsAlpha(mainColor3rd);
                            m_MaterialEditor.ShaderProperty(main3rdTexIsMSDF, GetLoc("sAsMSDF"));
                            m_MaterialEditor.ShaderProperty(main3rdTex_Cull, sCullModes);
                            EditorGUI.indentLevel -= 2;
                            m_MaterialEditor.ShaderProperty(main3rdEnableLighting, GetLoc("sEnableLighting"));
                            m_MaterialEditor.ShaderProperty(main3rdTexBlendMode, sBlendModes);
                            lilEditorGUI.DrawLine();
                            UV4Decal(main3rdTexIsDecal, main3rdTexIsLeftOnly, main3rdTexIsRightOnly, main3rdTexShouldCopy, main3rdTexShouldFlipMirror, main3rdTexShouldFlipCopy, main3rdTex, main3rdTexAngle, main3rdTexDecalAnimation, main3rdTexDecalSubParam, main3rdTex_UVMode);
                            lilEditorGUI.DrawLine();
                            m_MaterialEditor.TexturePropertySingleLine(maskBlendContent, main3rdBlendMask);
                            EditorGUILayout.LabelField(GetLoc("sDistanceFade"));
                            EditorGUI.indentLevel++;
                            m_MaterialEditor.ShaderProperty(main3rdDistanceFade, sDistanceFadeSetting);
                            EditorGUI.indentLevel--;
                            lilEditorGUI.DrawLine();
                            m_MaterialEditor.ShaderProperty(main3rdDissolveParams, sDissolveParams);
                            if(main3rdDissolveParams.vectorValue.x == 1.0f)                                                TextureGUI(ref edSet.isShowMain3rdDissolveMask, maskBlendContent, main3rdDissolveMask);
                            if(main3rdDissolveParams.vectorValue.x == 2.0f && main3rdDissolveParams.vectorValue.y == 0.0f) m_MaterialEditor.ShaderProperty(main3rdDissolvePos, GetLoc("sPosition") + "|2");
                            if(main3rdDissolveParams.vectorValue.x == 2.0f && main3rdDissolveParams.vectorValue.y == 1.0f) m_MaterialEditor.ShaderProperty(main3rdDissolvePos, GetLoc("sVector") + "|2");
                            if(main3rdDissolveParams.vectorValue.x == 3.0f && main3rdDissolveParams.vectorValue.y == 0.0f) m_MaterialEditor.ShaderProperty(main3rdDissolvePos, GetLoc("sPosition") + "|3");
                            if(main3rdDissolveParams.vectorValue.x == 3.0f && main3rdDissolveParams.vectorValue.y == 1.0f) m_MaterialEditor.ShaderProperty(main3rdDissolvePos, GetLoc("sVector") + "|3");
                            if(main3rdDissolveParams.vectorValue.x != 0.0f)
                            {
                                TextureGUI(ref edSet.isShowMain3rdDissolveNoiseMask, noiseMaskContent, main3rdDissolveNoiseMask, main3rdDissolveNoiseStrength, main3rdDissolveNoiseMask_ScrollRotate);
                                m_MaterialEditor.ShaderProperty(main3rdDissolveColor, GetLoc("sColor"));
                            }
                            lilEditorGUI.DrawLine();
                            TextureBakeGUI(material, 6);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Alpha Mask
                        DrawAlphaMaskSettings(material);
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Shadow
                if(!isGem)
                {
                    DrawShadowSettings();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Emission
                edSet.isShowEmission = lilEditorGUI.Foldout(GetLoc("sEmissionSetting"), edSet.isShowEmission);
                DrawMenuButton(GetLoc("sAnchorEmission"), PropertyBlock.Emission);
                if(edSet.isShowEmission)
                {
                    // Emission
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useEmission, GetLoc("sEmission"));
                    DrawMenuButton(GetLoc("sAnchorEmission"), PropertyBlock.Emission1st);
                    if(useEmission.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        TextureGUI(ref edSet.isShowEmissionMap, colorMaskRGBAContent, emissionMap, emissionColor, emissionMap_ScrollRotate, emissionMap_UVMode, true, true);
                        lilEditorGUI.DrawColorAsAlpha(emissionColor);
                        m_MaterialEditor.ShaderProperty(emissionMainStrength, GetLoc("sMainColorPower"));
                        lilEditorGUI.DrawLine();
                        TextureGUI(ref edSet.isShowEmissionBlendMask, maskBlendRGBAContent, emissionBlendMask, emissionBlend, emissionBlendMask_ScrollRotate, true, true);
                        lilEditorGUI.DrawLine();
                        m_MaterialEditor.ShaderProperty(emissionBlink, blinkSetting);
                        lilEditorGUI.DrawLine();
                        m_MaterialEditor.ShaderProperty(emissionUseGrad, GetLoc("sGradation"));
                        if(emissionUseGrad.floatValue == 1)
                        {
                            EditorGUI.indentLevel++;
                            m_MaterialEditor.TexturePropertySingleLine(gradSpeedContent, emissionGradTex, emissionGradSpeed);
                            lilTextureUtils.GradientEditor(material, "_eg", emiGrad, emissionGradSpeed);
                            EditorGUI.indentLevel--;
                        }
                        lilEditorGUI.DrawLine();
                        m_MaterialEditor.ShaderProperty(emissionParallaxDepth, GetLoc("sParallaxDepth"));
                        m_MaterialEditor.ShaderProperty(emissionFluorescence, GetLoc("sFluorescence"));
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();

                    // Emission 2nd
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useEmission2nd, GetLoc("sEmission2nd"));
                    DrawMenuButton(GetLoc("sAnchorEmission"), PropertyBlock.Emission2nd);
                    if(useEmission2nd.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        TextureGUI(ref edSet.isShowEmission2ndMap, colorMaskRGBAContent, emission2ndMap, emission2ndColor, emission2ndMap_ScrollRotate, emission2ndMap_UVMode, true, true);
                        lilEditorGUI.DrawColorAsAlpha(emission2ndColor);
                        m_MaterialEditor.ShaderProperty(emission2ndMainStrength, GetLoc("sMainColorPower"));
                        lilEditorGUI.DrawLine();
                        TextureGUI(ref edSet.isShowEmission2ndBlendMask, maskBlendRGBAContent, emission2ndBlendMask, emission2ndBlend, emission2ndBlendMask_ScrollRotate, true, true);
                        lilEditorGUI.DrawLine();
                        m_MaterialEditor.ShaderProperty(emission2ndBlink, blinkSetting);
                        lilEditorGUI.DrawLine();
                        m_MaterialEditor.ShaderProperty(emission2ndUseGrad, GetLoc("sGradation"));
                        if(emission2ndUseGrad.floatValue == 1)
                        {
                            EditorGUI.indentLevel++;
                            m_MaterialEditor.TexturePropertySingleLine(gradSpeedContent, emission2ndGradTex, emission2ndGradSpeed);
                            lilTextureUtils.GradientEditor(material, "_e2g", emi2Grad, emission2ndGradSpeed);
                            EditorGUI.indentLevel--;
                        }
                        lilEditorGUI.DrawLine();
                        m_MaterialEditor.ShaderProperty(emission2ndParallaxDepth, GetLoc("sParallaxDepth"));
                        m_MaterialEditor.ShaderProperty(emission2ndFluorescence, GetLoc("sFluorescence"));
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.Space();

                //------------------------------------------------------------------------------------------------------------------------------
                // Normal / Reflection
                GUILayout.Label(GetLoc("sNormalMapReflection"), boldLabel);

                //------------------------------------------------------------------------------------------------------------------------------
                // Normal
                edSet.isShowBump = lilEditorGUI.Foldout(GetLoc("sNormalMapSetting"), edSet.isShowBump);
                DrawMenuButton(GetLoc("sAnchorNormalMap"), PropertyBlock.NormalMap);
                if(edSet.isShowBump)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // 1st
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useBumpMap, GetLoc("sNormalMap"));
                    DrawMenuButton(GetLoc("sAnchorNormalMap"), PropertyBlock.NormalMap1st);
                    if(useBumpMap.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        TextureGUI(ref edSet.isShowBumpMap, normalMapContent, bumpMap, bumpScale);
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // 2nd
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useBump2ndMap, GetLoc("sNormalMap2nd"));
                    DrawMenuButton(GetLoc("sAnchorNormalMap"), PropertyBlock.NormalMap2nd);
                    if(useBump2ndMap.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        TextureGUI(ref edSet.isShowBump2ndMap, normalMapContent, bump2ndMap, bump2ndScale, bump2ndMap_UVMode, "UV Mode|UV0|UV1|UV2|UV3");
                        lilEditorGUI.DrawLine();
                        TextureGUI(ref edSet.isShowBump2ndScaleMask, maskStrengthContent, bump2ndScaleMask);
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Anisotropy
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useAnisotropy, GetLoc("sAnisotropy"));
                    DrawMenuButton(GetLoc("sAnchorAnisotropy"), PropertyBlock.Anisotropy);
                    if(useAnisotropy.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        TextureGUI(ref edSet.isShowAnisotropyTangentMap, normalMapContent, anisotropyTangentMap);
                        lilEditorGUI.DrawLine();
                        TextureGUI(ref edSet.isShowAnisotropyScaleMask, maskStrengthContent, anisotropyScaleMask, anisotropyScale);
                        lilEditorGUI.DrawLine();
                        GUILayout.Label(GetLoc("sApplyTo"), boldLabel);
                        EditorGUI.indentLevel++;
                        m_MaterialEditor.ShaderProperty(anisotropy2Reflection, GetLoc("sReflection"));
                        if(anisotropy2Reflection.floatValue != 0.0f)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.LabelField("1st Specular", boldLabel);
                            m_MaterialEditor.ShaderProperty(anisotropyTangentWidth, GetLoc("sTangentWidth"));
                            m_MaterialEditor.ShaderProperty(anisotropyBitangentWidth, GetLoc("sBitangentWidth"));
                            m_MaterialEditor.ShaderProperty(anisotropyShift, GetLoc("sOffset"));
                            m_MaterialEditor.ShaderProperty(anisotropyShiftNoiseScale, GetLoc("sNoiseStrength"));
                            m_MaterialEditor.ShaderProperty(anisotropySpecularStrength, GetLoc("sStrength"));
                            lilEditorGUI.DrawLine();
                            EditorGUILayout.LabelField("2nd Specular", boldLabel);
                            m_MaterialEditor.ShaderProperty(anisotropy2ndTangentWidth, GetLoc("sTangentWidth"));
                            m_MaterialEditor.ShaderProperty(anisotropy2ndBitangentWidth, GetLoc("sBitangentWidth"));
                            m_MaterialEditor.ShaderProperty(anisotropy2ndShift, GetLoc("sOffset"));
                            m_MaterialEditor.ShaderProperty(anisotropy2ndShiftNoiseScale, GetLoc("sNoiseStrength"));
                            m_MaterialEditor.ShaderProperty(anisotropy2ndSpecularStrength, GetLoc("sStrength"));
                            lilEditorGUI.DrawLine();
                            m_MaterialEditor.ShaderProperty(anisotropyShiftNoiseMask, GetLoc("sNoise"));
                            EditorGUI.indentLevel--;
                        }
                        m_MaterialEditor.ShaderProperty(anisotropy2MatCap, GetLoc("sMatCap"));
                        m_MaterialEditor.ShaderProperty(anisotropy2MatCap2nd, GetLoc("sMatCap2nd"));
                        EditorGUI.indentLevel--;
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Backlight
                if(!isGem)
                {
                    edSet.isShowBacklight = lilEditorGUI.Foldout(GetLoc("sBacklightSetting"), edSet.isShowBacklight);
                    DrawMenuButton(GetLoc("sAnchorBacklight"), PropertyBlock.Backlight);
                    if(edSet.isShowBacklight)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        m_MaterialEditor.ShaderProperty(useBacklight, GetLoc("sBacklight"));
                        DrawMenuButton(GetLoc("sAnchorBacklight"), PropertyBlock.Backlight);
                        if(useBacklight.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            TextureGUI(ref edSet.isShowBacklightColorTex, colorMaskRGBAContent, backlightColorTex, backlightColor);
                            EditorGUI.indentLevel++;
                            lilEditorGUI.DrawColorAsAlpha(backlightColor);
                            m_MaterialEditor.ShaderProperty(backlightMainStrength, GetLoc("sMainColorPower"));
                            m_MaterialEditor.ShaderProperty(backlightReceiveShadow, GetLoc("sReceiveShadow"));
                            m_MaterialEditor.ShaderProperty(backlightBackfaceMask, GetLoc("sBackfaceMask"));
                            EditorGUI.indentLevel--;
                            lilEditorGUI.DrawLine();
                            m_MaterialEditor.ShaderProperty(backlightNormalStrength, GetLoc("sNormalStrength"));
                            lilEditorGUI.InvBorderGUI(backlightBorder);
                            m_MaterialEditor.ShaderProperty(backlightBlur, GetLoc("sBlur"));
                            m_MaterialEditor.ShaderProperty(backlightDirectivity, GetLoc("sDirectivity"));
                            m_MaterialEditor.ShaderProperty(backlightViewStrength, GetLoc("sViewDirectionStrength"));
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Reflection
                if(!isGem)
                {
                    edSet.isShowReflections = lilEditorGUI.Foldout(GetLoc("sReflectionsSetting"), edSet.isShowReflections);
                    DrawMenuButton(GetLoc("sAnchorReflection"), PropertyBlock.Reflection);
                    if(edSet.isShowReflections)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Reflection
                        EditorGUILayout.BeginVertical(boxOuter);
                        m_MaterialEditor.ShaderProperty(useReflection, GetLoc("sReflection"));
                        DrawMenuButton(GetLoc("sAnchorReflection"), PropertyBlock.Reflection);
                        if(useReflection.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            TextureGUI(ref edSet.isShowSmoothnessTex, smoothnessContent, smoothnessTex, smoothness);
                            m_MaterialEditor.ShaderProperty(gsaaStrength, "GSAA", 1);
                            lilEditorGUI.DrawLine();
                            TextureGUI(ref edSet.isShowMetallicGlossMap, metallicContent, metallicGlossMap, metallic);
                            lilEditorGUI.DrawLine();
                            TextureGUI(ref edSet.isShowReflectionColorTex, colorMaskRGBAContent, reflectionColorTex, reflectionColor);
                            EditorGUI.indentLevel++;
                            lilEditorGUI.DrawColorAsAlpha(reflectionColor);
                            m_MaterialEditor.ShaderProperty(reflectance, GetLoc("sReflectance"));
                            EditorGUI.indentLevel--;
                            lilEditorGUI.DrawLine();
                            DrawSpecularMode();
                            m_MaterialEditor.ShaderProperty(applyReflection, GetLoc("sApplyReflection"));
                            if(applyReflection.floatValue == 1.0f)
                            {
                                EditorGUI.indentLevel++;
                                m_MaterialEditor.ShaderProperty(reflectionNormalStrength, GetLoc("sNormalStrength"));
                                m_MaterialEditor.TexturePropertySingleLine(new GUIContent("Cubemap Fallback"), reflectionCubeTex, reflectionCubeColor);
                                m_MaterialEditor.ShaderProperty(reflectionCubeOverride, "Override");
                                m_MaterialEditor.ShaderProperty(reflectionCubeEnableLighting, GetLoc("sEnableLighting") + " (Fallback)");
                                EditorGUI.indentLevel--;
                            }
                            if(isTransparent) m_MaterialEditor.ShaderProperty(reflectionApplyTransparency, GetLoc("sApplyTransparency"));
                            m_MaterialEditor.ShaderProperty(reflectionBlendMode, sBlendModes);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // MatCap
                DrawMatCapSettings();

                //------------------------------------------------------------------------------------------------------------------------------
                // Rim
                DrawRimSettings();

                //------------------------------------------------------------------------------------------------------------------------------
                // Glitter
                DrawGlitterSettings();

                //------------------------------------------------------------------------------------------------------------------------------
                // Gem
                if(isGem)
                {
                    edSet.isShowGem = lilEditorGUI.Foldout(GetLoc("sGemSetting"), edSet.isShowGem);
                    DrawMenuButton(GetLoc("sAnchorGem"), PropertyBlock.Gem);
                    if(edSet.isShowGem)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sGem"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorGem"), PropertyBlock.Gem);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        GUILayout.Label(GetLoc("sRefraction"), boldLabel);
                        EditorGUI.indentLevel++;
                        m_MaterialEditor.ShaderProperty(refractionStrength, GetLoc("sStrength"));
                        m_MaterialEditor.ShaderProperty(refractionFresnelPower, GetLoc("sRefractionFresnel"));
                        EditorGUI.indentLevel--;
                        lilEditorGUI.DrawLine();
                        GUILayout.Label(GetLoc("sGem"), boldLabel);
                        EditorGUI.indentLevel++;
                        m_MaterialEditor.ShaderProperty(gemChromaticAberration, GetLoc("sChromaticAberration"));
                        m_MaterialEditor.ShaderProperty(gemEnvContrast, GetLoc("sContrast"));
                        m_MaterialEditor.ShaderProperty(gemEnvColor, GetLoc("sEnvironmentColor"));
                        lilEditorGUI.DrawLine();
                        m_MaterialEditor.ShaderProperty(gemParticleLoop, GetLoc("sParticleLoop"));
                        m_MaterialEditor.ShaderProperty(gemParticleColor, GetLoc("sColor"));
                        EditorGUI.indentLevel--;
                        lilEditorGUI.DrawLine();
                        m_MaterialEditor.ShaderProperty(gemVRParallaxStrength, GetLoc("sVRParallaxStrength"));
                        m_MaterialEditor.TexturePropertySingleLine(smoothnessContent, smoothnessTex, smoothness);
                        m_MaterialEditor.ShaderProperty(reflectance, GetLoc("sReflectance"));
                        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("Cubemap Fallback"), reflectionCubeTex, reflectionCubeColor);
                        m_MaterialEditor.ShaderProperty(reflectionCubeOverride, "Override");
                        m_MaterialEditor.ShaderProperty(reflectionCubeEnableLighting, GetLoc("sEnableLighting") + " (Fallback)");
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                EditorGUILayout.Space();

                //------------------------------------------------------------------------------------------------------------------------------
                // Advanced
                GUILayout.Label(GetLoc("sAdvanced"), boldLabel);

                //------------------------------------------------------------------------------------------------------------------------------
                // Outline
                DrawOutlineSettings(material);

                //------------------------------------------------------------------------------------------------------------------------------
                // Parallax
                edSet.isShowParallax = lilEditorGUI.Foldout(GetLoc("sParallax"), edSet.isShowParallax);
                DrawMenuButton(GetLoc("sAnchorParallax"), PropertyBlock.Parallax);
                if(edSet.isShowParallax)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useParallax, GetLoc("sParallax"));
                    DrawMenuButton(GetLoc("sAnchorParallax"), PropertyBlock.Parallax);
                    if(useParallax.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        m_MaterialEditor.TexturePropertySingleLine(parallaxContent, parallaxMap, parallax);
                        m_MaterialEditor.ShaderProperty(parallaxOffset, GetLoc("sParallaxOffset"));
                        m_MaterialEditor.ShaderProperty(usePOM, GetLoc("sPOM"));
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Distance Fade
                edSet.isShowDistanceFade = lilEditorGUI.Foldout(GetLoc("sDistanceFade"), edSet.isShowDistanceFade);
                DrawMenuButton(GetLoc("sAnchorDistanceFade"), PropertyBlock.DistanceFade);
                if(edSet.isShowDistanceFade)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    EditorGUILayout.LabelField(GetLoc("sDistanceFade"), customToggleFont);
                    DrawMenuButton(GetLoc("sAnchorDistanceFade"), PropertyBlock.DistanceFade);
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    m_MaterialEditor.ShaderProperty(distanceFadeColor, GetLoc("sColor"));
                    EditorGUI.indentLevel++;
                    m_MaterialEditor.ShaderProperty(distanceFade, sDistanceFadeSetting);
                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // AudioLink
                edSet.isShowAudioLink = lilEditorGUI.Foldout(GetLoc("sAudioLink"), edSet.isShowAudioLink);
                DrawMenuButton(GetLoc("sAnchorAudioLink"), PropertyBlock.AudioLink);
                if(edSet.isShowAudioLink)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useAudioLink, GetLoc("sAudioLink"));
                    DrawMenuButton(GetLoc("sAnchorAudioLink"), PropertyBlock.AudioLink);
                    if(useAudioLink.floatValue == 1)
                    {
                        string sALParamsNone = BuildParams(GetLoc("sOffset"), GetLoc("sAudioLinkBand"), GetLoc("sAudioLinkBandBass"), GetLoc("sAudioLinkBandLowMid"), GetLoc("sAudioLinkBandHighMid"), GetLoc("sAudioLinkBandTreble"));
                        string sALParamsPos = BuildParams(GetLoc("sScale"), GetLoc("sOffset"), GetLoc("sAudioLinkBand"), GetLoc("sAudioLinkBandBass"), GetLoc("sAudioLinkBandLowMid"), GetLoc("sAudioLinkBandHighMid"), GetLoc("sAudioLinkBandTreble"));
                        string sALParamsUV = BuildParams(GetLoc("sScale"), GetLoc("sOffset"), GetLoc("sAngle"), GetLoc("sAudioLinkBand"), GetLoc("sAudioLinkBandBass"), GetLoc("sAudioLinkBandLowMid"), GetLoc("sAudioLinkBandHighMid"), GetLoc("sAudioLinkBandTreble"));
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        m_MaterialEditor.ShaderProperty(audioLinkUVMode, BuildParams(GetLoc("sAudioLinkUVMode"), GetLoc("sAudioLinkUVModeNone"), GetLoc("sAudioLinkUVModeRim"), GetLoc("sAudioLinkUVModeUV"), GetLoc("sAudioLinkUVModeMask"), GetLoc("sAudioLinkUVModeMask") + " (Spectrum)", GetLoc("sAudioLinkUVModePosition")));
                        if(audioLinkUVMode.floatValue == 0) m_MaterialEditor.ShaderProperty(audioLinkUVParams, sALParamsNone);
                        if(audioLinkUVMode.floatValue == 1) m_MaterialEditor.ShaderProperty(audioLinkUVParams, sALParamsPos);
                        if(audioLinkUVMode.floatValue == 2) m_MaterialEditor.ShaderProperty(audioLinkUVParams, sALParamsUV);
                        if(audioLinkUVMode.floatValue == 3) m_MaterialEditor.TexturePropertySingleLine(audioLinkMaskContent, audioLinkMask);
                        if(audioLinkUVMode.floatValue == 4)
                        {
                            m_MaterialEditor.TexturePropertySingleLine(audioLinkMaskSpectrumContent, audioLinkMask);
                            lilEditorGUI.DrawVectorAs4Float(audioLinkUVParams, "Volume", "Base Boost", "Treble Boost", "Line Width");
                        }
                        if(audioLinkUVMode.floatValue == 5)
                        {
                            m_MaterialEditor.ShaderProperty(audioLinkUVParams, sALParamsPos);
                            m_MaterialEditor.ShaderProperty(audioLinkStart, GetLoc("sAudioLinkStartPosition"));
                        }
                        lilEditorGUI.DrawLine();
                        GUILayout.Label(GetLoc("sAudioLinkDefaultValue"), boldLabel);
                        EditorGUI.indentLevel++;
                        if(audioLinkUVMode.floatValue == 4) lilEditorGUI.DrawVectorAs4Float(audioLinkDefaultValue, GetLoc("sStrength"), "Detail", "Speed", GetLoc("sThreshold"));
                        else m_MaterialEditor.ShaderProperty(audioLinkDefaultValue, BuildParams(GetLoc("sStrength"), GetLoc("sBlinkStrength"), GetLoc("sBlinkSpeed"), GetLoc("sThreshold")));
                        EditorGUI.indentLevel--;
                        lilEditorGUI.DrawLine();
                        GUILayout.Label(GetLoc("sApplyTo"), boldLabel);
                        EditorGUI.indentLevel++;
                        m_MaterialEditor.ShaderProperty(audioLink2Main2nd, GetLoc("sMainColor2nd"));
                        m_MaterialEditor.ShaderProperty(audioLink2Main3rd, GetLoc("sMainColor3rd"));
                        m_MaterialEditor.ShaderProperty(audioLink2Emission, GetLoc("sEmission"));
                        m_MaterialEditor.ShaderProperty(audioLink2EmissionGrad, GetLoc("sEmission") + GetLoc("sGradation"));
                        m_MaterialEditor.ShaderProperty(audioLink2Emission2nd, GetLoc("sEmission2nd"));
                        m_MaterialEditor.ShaderProperty(audioLink2Emission2ndGrad, GetLoc("sEmission2nd") + GetLoc("sGradation"));
                        m_MaterialEditor.ShaderProperty(audioLink2Vertex, GetLoc("sVertex"));
                        if(audioLink2Vertex.floatValue == 1)
                        {
                            EditorGUI.indentLevel++;
                            m_MaterialEditor.ShaderProperty(audioLinkVertexUVMode, BuildParams(GetLoc("sAudioLinkUVMode"), GetLoc("sAudioLinkUVModeNone"), GetLoc("sAudioLinkUVModePosition"), GetLoc("sAudioLinkUVModeUV"), GetLoc("sAudioLinkUVModeMask")));
                            if(audioLinkVertexUVMode.floatValue == 0) m_MaterialEditor.ShaderProperty(audioLinkVertexUVParams, sALParamsNone);
                            if(audioLinkVertexUVMode.floatValue == 1) m_MaterialEditor.ShaderProperty(audioLinkVertexUVParams, sALParamsPos);
                            if(audioLinkVertexUVMode.floatValue == 2) m_MaterialEditor.ShaderProperty(audioLinkVertexUVParams, sALParamsUV);
                            if(audioLinkVertexUVMode.floatValue == 3) m_MaterialEditor.TexturePropertySingleLine(audioLinkMaskContent, audioLinkMask);
                            if(audioLinkVertexUVMode.floatValue == 1) m_MaterialEditor.ShaderProperty(audioLinkVertexStart, GetLoc("sAudioLinkStartPosition"));
                            lilEditorGUI.DrawLine();
                            m_MaterialEditor.ShaderProperty(audioLinkVertexStrength, BuildParams(GetLoc("sAudioLinkMovingVector"), GetLoc("sAudioLinkNormalStrength")));
                            EditorGUI.indentLevel--;
                        }
                        EditorGUI.indentLevel--;
                        lilEditorGUI.DrawLine();
                        m_MaterialEditor.ShaderProperty(audioLinkAsLocal, GetLoc("sAudioLinkAsLocal"));
                        if(audioLinkAsLocal.floatValue == 1)
                        {
                            m_MaterialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sAudioLinkLocalMap"), ""), audioLinkLocalMap);
                            m_MaterialEditor.ShaderProperty(audioLinkLocalMapParams, BuildParams(GetLoc("sAudioLinkLocalMapBPM"), GetLoc("sAudioLinkLocalMapNotes"), GetLoc("sOffset")));
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Dissolve
                edSet.isShowDissolve = lilEditorGUI.Foldout(GetLoc("sDissolve"), edSet.isShowDissolve);
                DrawMenuButton(GetLoc("sAnchorDissolve"), PropertyBlock.Dissolve);
                if(edSet.isShowDissolve && ((renderingModeBuf == RenderingMode.Opaque && !isMulti) || (isMulti && transparentModeMat.floatValue == 0.0f)))
                {
                    GUILayout.Label(GetLoc("sDissolveWarnOpaque"), wrapLabel);
                }
                if(edSet.isShowDissolve && (renderingModeBuf != RenderingMode.Opaque || (isMulti && transparentModeMat.floatValue != 0.0f)))
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(dissolveParams, sDissolveParamsMode);
                    DrawMenuButton(GetLoc("sAnchorDissolve"), PropertyBlock.Dissolve);
                    if(dissolveParams.vectorValue.x != 0)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        m_MaterialEditor.ShaderProperty(dissolveParams, sDissolveParamsOther);
                        if(dissolveParams.vectorValue.x == 1.0f)                                         TextureGUI(ref edSet.isShowDissolveMask, maskBlendContent, dissolveMask);
                        if(dissolveParams.vectorValue.x == 2.0f && dissolveParams.vectorValue.y == 0.0f) m_MaterialEditor.ShaderProperty(dissolvePos, GetLoc("sPosition") + "|2");
                        if(dissolveParams.vectorValue.x == 2.0f && dissolveParams.vectorValue.y == 1.0f) m_MaterialEditor.ShaderProperty(dissolvePos, GetLoc("sVector") + "|2");
                        if(dissolveParams.vectorValue.x == 3.0f && dissolveParams.vectorValue.y == 0.0f) m_MaterialEditor.ShaderProperty(dissolvePos, GetLoc("sPosition") + "|3");
                        if(dissolveParams.vectorValue.x == 3.0f && dissolveParams.vectorValue.y == 1.0f) m_MaterialEditor.ShaderProperty(dissolvePos, GetLoc("sVector") + "|3");
                        TextureGUI(ref edSet.isShowDissolveNoiseMask, noiseMaskContent, dissolveNoiseMask, dissolveNoiseStrength, dissolveNoiseMask_ScrollRotate);
                        m_MaterialEditor.ShaderProperty(dissolveColor, GetLoc("sColor"));
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Encryption
                if(lilDirectoryManager.ExistsEncryption())
                {
                    edSet.isShowEncryption = lilEditorGUI.Foldout(GetLoc("sEncryption"), edSet.isShowEncryption);
                    DrawMenuButton(GetLoc("sAnchorEncryption"), PropertyBlock.Encryption);
                    if(edSet.isShowEncryption)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sEncryption"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorEncryption"), PropertyBlock.Encryption);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        m_MaterialEditor.ShaderProperty(ignoreEncryption, GetLoc("sIgnoreEncryption"));
                        m_MaterialEditor.ShaderProperty(keys, GetLoc("sKeys"));
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Refraction
                if(isRefr)
                {
                    edSet.isShowRefraction = lilEditorGUI.Foldout(GetLoc("sRefractionSetting"), edSet.isShowRefraction);
                    DrawMenuButton(GetLoc("sAnchorRefraction"), PropertyBlock.Refraction);
                    if(edSet.isShowRefraction)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sRefraction"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorRefraction"), PropertyBlock.Refraction);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        m_MaterialEditor.ShaderProperty(refractionStrength, GetLoc("sStrength"));
                        m_MaterialEditor.ShaderProperty(refractionFresnelPower, GetLoc("sRefractionFresnel"));
                        m_MaterialEditor.ShaderProperty(refractionColorFromMain, GetLoc("sColorFromMain"));
                        m_MaterialEditor.ShaderProperty(refractionColor, GetLoc("sColor"));
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Fur
                if(isFur)
                {
                    edSet.isShowFur = lilEditorGUI.Foldout(GetLoc("sFurSetting"), edSet.isShowFur);
                    DrawMenuButton(GetLoc("sAnchorFur"), PropertyBlock.Fur);
                    if(edSet.isShowFur)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sFur"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorFur"), PropertyBlock.Fur);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        m_MaterialEditor.TexturePropertySingleLine(normalMapContent, furVectorTex, furVectorScale);
                        m_MaterialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sLengthMask"), GetLoc("sStrengthR")), furLengthMask);
                        m_MaterialEditor.ShaderProperty(furVector, BuildParams(GetLoc("sVector"), GetLoc("sLength")));
                        if(isTwoPass) m_MaterialEditor.ShaderProperty(furCutoutLength, GetLoc("sLength") + " (Cutout)");
                        m_MaterialEditor.ShaderProperty(vertexColor2FurVector, GetLoc("sVertexColor2Vector"));
                        m_MaterialEditor.ShaderProperty(furGravity, GetLoc("sGravity"));
                        m_MaterialEditor.ShaderProperty(furRandomize, GetLoc("sRandomize"));
                        lilEditorGUI.DrawLine();
                        m_MaterialEditor.TexturePropertySingleLine(noiseMaskContent, furNoiseMask);
                        m_MaterialEditor.TextureScaleOffsetProperty(furNoiseMask);
                        m_MaterialEditor.TexturePropertySingleLine(alphaMaskContent, furMask);
                        m_MaterialEditor.ShaderProperty(furAO, GetLoc("sAO"));
                        lilEditorGUI.DrawLine();
                        m_MaterialEditor.ShaderProperty(furMeshType, "Mesh Type|Subdivision|Shrink");
                        if(furMeshType.floatValue == 0)
                        {
                            int furLayerNum2 = (int)furLayerNum.floatValue;
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.showMixedValue = furLayerNum.hasMixedValue;
                            furLayerNum2 = EditorGUILayout.IntSlider(GetLoc("sLayerNum"), furLayerNum2, 1, 3);
                            EditorGUI.showMixedValue = false;
                            if(EditorGUI.EndChangeCheck())
                            {
                                furLayerNum.floatValue = furLayerNum2;
                            }
                        }
                        else
                        {
                            m_MaterialEditor.ShaderProperty(furLayerNum, GetLoc("sLayerNum"));
                        }
                        lilEditorGUI.MinusRangeGUI(furRootOffset, GetLoc("sRootWidth"));
                        m_MaterialEditor.ShaderProperty(furTouchStrength, GetLoc("sTouchStrength"));
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Stencil
                DrawStencilSettings(material);

                //------------------------------------------------------------------------------------------------------------------------------
                // Rendering
                edSet.isShowRendering = lilEditorGUI.Foldout(GetLoc("sRenderingSetting"), edSet.isShowRendering);
                DrawMenuButton(GetLoc("sAnchorRendering"), PropertyBlock.Rendering);
                if(edSet.isShowRendering)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // Reset Button
                    if(lilEditorGUI.EditorButton(GetLoc("sRenderingReset")))
                    {
                        material.enableInstancing = false;
                        SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf);
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Base
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sRenderingSetting"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInner);
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Shader
                        int shaderType = 0;
                        int shaderTypeBuf = shaderType;
                        shaderType = EditorGUILayout.Popup(GetLoc("sShaderType"),shaderType,new string[]{GetLoc("sShaderTypeNormal"),GetLoc("sShaderTypeLite")});
                        if(shaderTypeBuf != shaderType)
                        {
                            if(shaderType==0) isLite = false;
                            if(shaderType==1) isLite = true;
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf);
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Rendering
                        if(renderingModeBuf == RenderingMode.Transparent || renderingModeBuf == RenderingMode.Fur || renderingModeBuf == RenderingMode.FurTwoPass || (isMulti && (transparentModeMat.floatValue == 2.0f || transparentModeMat.floatValue == 4.0f)))
                        {
                            m_MaterialEditor.ShaderProperty(subpassCutoff, GetLoc("sSubpassCutoff"));
                        }
                        m_MaterialEditor.ShaderProperty(cull, sCullModes);
                        m_MaterialEditor.ShaderProperty(zclip, GetLoc("sZClip"));
                        m_MaterialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                        m_MaterialEditor.ShaderProperty(ztest, GetLoc("sZTest"));
                        m_MaterialEditor.ShaderProperty(offsetFactor, GetLoc("sOffsetFactor"));
                        m_MaterialEditor.ShaderProperty(offsetUnits, GetLoc("sOffsetUnits"));
                        m_MaterialEditor.ShaderProperty(colorMask, GetLoc("sColorMask"));
                        m_MaterialEditor.ShaderProperty(alphaToMask, GetLoc("sAlphaToMask"));
                        m_MaterialEditor.ShaderProperty(lilShadowCasterBias, "Shadow Caster Bias");
                        lilEditorGUI.DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlend, GetLoc("sForward"), srcBlend, dstBlend, srcBlendAlpha, dstBlendAlpha, blendOp, blendOpAlpha);
                        lilEditorGUI.DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlendAdd, GetLoc("sForwardAdd"), srcBlendFA, dstBlendFA, srcBlendAlphaFA, dstBlendAlphaFA, blendOpFA, blendOpAlphaFA);
                        lilEditorGUI.DrawLine();
                        if(!isCustomEditor) m_MaterialEditor.EnableInstancingField();
                        m_MaterialEditor.RenderQueueField();
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Pre
                    if(transparentModeBuf == TransparentMode.TwoPass)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField("PrePass", customToggleFont);
                        EditorGUILayout.BeginVertical(boxInner);
                        m_MaterialEditor.ShaderProperty(preCull, sCullModes);
                        m_MaterialEditor.ShaderProperty(preZclip, GetLoc("sZClip"));
                        m_MaterialEditor.ShaderProperty(preZwrite, GetLoc("sZWrite"));
                        m_MaterialEditor.ShaderProperty(preZtest, GetLoc("sZTest"));
                        m_MaterialEditor.ShaderProperty(preOffsetFactor, GetLoc("sOffsetFactor"));
                        m_MaterialEditor.ShaderProperty(preOffsetUnits, GetLoc("sOffsetUnits"));
                        m_MaterialEditor.ShaderProperty(preColorMask, GetLoc("sColorMask"));
                        m_MaterialEditor.ShaderProperty(preAlphaToMask, GetLoc("sAlphaToMask"));
                        lilEditorGUI.DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlendPre, GetLoc("sForward"), preSrcBlend, preDstBlend, preSrcBlendAlpha, preDstBlendAlpha, preBlendOp, preBlendOpAlpha);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Outline
                    if(isOutl)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sOutline"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInner);
                        m_MaterialEditor.ShaderProperty(outlineCull, sCullModes);
                        m_MaterialEditor.ShaderProperty(outlineZclip, GetLoc("sZClip"));
                        m_MaterialEditor.ShaderProperty(outlineZwrite, GetLoc("sZWrite"));
                        m_MaterialEditor.ShaderProperty(outlineZtest, GetLoc("sZTest"));
                        m_MaterialEditor.ShaderProperty(outlineOffsetFactor, GetLoc("sOffsetFactor"));
                        m_MaterialEditor.ShaderProperty(outlineOffsetUnits, GetLoc("sOffsetUnits"));
                        m_MaterialEditor.ShaderProperty(outlineColorMask, GetLoc("sColorMask"));
                        m_MaterialEditor.ShaderProperty(outlineAlphaToMask, GetLoc("sAlphaToMask"));
                        lilEditorGUI.DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlendOutline, GetLoc("sForward"), outlineSrcBlend, outlineDstBlend, outlineSrcBlendAlpha, outlineDstBlendAlpha, outlineBlendOp, outlineBlendOpAlpha);
                        lilEditorGUI.DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlendAddOutline, GetLoc("sForwardAdd"), outlineSrcBlendFA, outlineDstBlendFA, outlineSrcBlendAlphaFA, outlineDstBlendAlphaFA, outlineBlendOpFA, outlineBlendOpAlphaFA);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Fur
                    if(isFur)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sFur"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInner);
                        m_MaterialEditor.ShaderProperty(furCull, sCullModes);
                        m_MaterialEditor.ShaderProperty(furZclip, GetLoc("sZClip"));
                        m_MaterialEditor.ShaderProperty(furZwrite, GetLoc("sZWrite"));
                        m_MaterialEditor.ShaderProperty(furZtest, GetLoc("sZTest"));
                        m_MaterialEditor.ShaderProperty(furOffsetFactor, GetLoc("sOffsetFactor"));
                        m_MaterialEditor.ShaderProperty(furOffsetUnits, GetLoc("sOffsetUnits"));
                        m_MaterialEditor.ShaderProperty(furColorMask, GetLoc("sColorMask"));
                        m_MaterialEditor.ShaderProperty(furAlphaToMask, GetLoc("sAlphaToMask"));
                        lilEditorGUI.DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlendFur, GetLoc("sForward"), furSrcBlend, furDstBlend, furSrcBlendAlpha, furDstBlendAlpha, furBlendOp, furBlendOpAlpha);
                        lilEditorGUI.DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlendAddFur, GetLoc("sForwardAdd"), furSrcBlendFA, furDstBlendFA, furSrcBlendAlphaFA, furDstBlendAlphaFA, furBlendOpFA, furBlendOpAlphaFA);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Light Bake
                edSet.isShowLightBake = lilEditorGUI.Foldout(GetLoc("sLightBakeSetting"), edSet.isShowLightBake);
                //DrawMenuButton(GetLoc("sAnchorLightBake"), PropertyBlock.LightBake);
                if(edSet.isShowLightBake)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    EditorGUILayout.LabelField(GetLoc("sLightBakeSetting"), customToggleFont);
                    EditorGUILayout.BeginVertical(boxInner);
                    if(!isCustomEditor) m_MaterialEditor.DoubleSidedGIField();
                    if(!isCustomEditor) DrawGIFragGUI();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Tessellation
                edSet.isShowTess = lilEditorGUI.Foldout(GetLoc("sTessellation"), edSet.isShowTess);
                DrawMenuButton(GetLoc("sAnchorTessellation"), PropertyBlock.Tessellation);
                if(edSet.isShowTess)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    if(isTess != EditorGUILayout.ToggleLeft(GetLoc("sTessellation"), isTess, customToggleFont))
                    {
                        isTess = !isTess;
                        SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf);
                    }
                    if(isTess)
                    {
                        EditorGUILayout.BeginVertical(boxInner);
                        m_MaterialEditor.ShaderProperty(tessEdge, GetLoc("sTessellationEdge"));
                        m_MaterialEditor.ShaderProperty(tessStrength, GetLoc("sStrength"));
                        m_MaterialEditor.ShaderProperty(tessShrink, GetLoc("sTessellationShrink"));
                        m_MaterialEditor.ShaderProperty(tessFactorMax, GetLoc("sTessellationFactor"));
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.Space();

                //------------------------------------------------------------------------------------------------------------------------------
                // Optimization
                if(!isMultiVariants)
                {
                    GUILayout.Label(GetLoc("sOptimization"), boldLabel);
                    edSet.isShowOptimization = lilEditorGUI.Foldout(GetLoc("sOptimization"), edSet.isShowOptimization);
                    lilEditorGUI.DrawHelpButton(GetLoc("sAnchorOptimization"));
                    if(edSet.isShowOptimization)
                    {
                        // Optimization
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sOptimization"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        DrawOptimizationButton(material, !(isLite && isMulti));
                        lilEditorGUI.RemoveUnusedPropertiesGUI(material);
                        TextureBakeGUI(material, 0);
                        TextureBakeGUI(material, 1);
                        TextureBakeGUI(material, 2);
                        TextureBakeGUI(material, 3);
                        if(lilEditorGUI.EditorButton(GetLoc("sConvertLite"))) CreateLiteMaterial(material);
                        if(mtoon != null && lilEditorGUI.EditorButton(GetLoc("sConvertMToon"))) CreateMToonMaterial(material);
                        if(!isMulti && !isFur && !isRefr && !isGem && lilEditorGUI.EditorButton(GetLoc("sConvertMulti"))) CreateMultiMaterial(material);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();

                        // Bake Textures
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sBake"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        if(!isGem && lilEditorGUI.EditorButton(GetLoc("sShadow1stColor")))   AutoBakeColoredMask(material, shadowColorTex,       shadowColor,        "Shadow1stColor");
                        if(!isGem && lilEditorGUI.EditorButton(GetLoc("sShadow2ndColor")))   AutoBakeColoredMask(material, shadow2ndColorTex,    shadow2ndColor,     "Shadow2ndColor");
                        if(!isGem && lilEditorGUI.EditorButton(GetLoc("sShadow3rdColor")))   AutoBakeColoredMask(material, shadow3rdColorTex,    shadow3rdColor,     "Shadow3rdColor");
                        if(!isGem && lilEditorGUI.EditorButton(GetLoc("sReflection")))       AutoBakeColoredMask(material, reflectionColorTex,   reflectionColor,    "ReflectionColor");
                        if(lilEditorGUI.EditorButton(GetLoc("sMatCap")))                     AutoBakeColoredMask(material, matcapBlendMask,      matcapColor,        "MatCapColor");
                        if(lilEditorGUI.EditorButton(GetLoc("sMatCap2nd")))                  AutoBakeColoredMask(material, matcap2ndBlendMask,   matcap2ndColor,     "MatCap2ndColor");
                        if(lilEditorGUI.EditorButton(GetLoc("sRimLight")))                   AutoBakeColoredMask(material, rimColorTex,          rimColor,           "RimColor");
                        if(((!isRefr && !isFur && !isGem && !isCustomShader) || (isCustomShader && isOutl)) && lilEditorGUI.EditorButton(GetLoc("sSettingTexOutlineColor"))) AutoBakeColoredMask(material, outlineColorMask, outlineColor, "OutlineColor");
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }
            }
        }

        private void DrawPresetGUI()
        {
            if(isLite)  EditorGUILayout.LabelField(GetLoc("sPresetsNotAvailable"), wrapLabel);
            else        DrawPreset();
        }

        private void DrawSettingsGUI()
        {
            GUIStyle applyButton = new GUIStyle(GUI.skin.button);
            applyButton.normal.textColor = Color.red;
            applyButton.fontStyle = FontStyle.Bold;

            bool isLocked = File.Exists(lilDirectoryManager.GetSettingLockPath());
            EditorGUI.BeginChangeCheck();
            ToggleGUI(GetLoc("sSettingLock"), ref isLocked);
            if(EditorGUI.EndChangeCheck())
            {
                if(isLocked) lilToonSetting.SaveLockedSetting(shaderSetting);
                else         lilToonSetting.DeleteLockedSetting();
            }


            #if VRC_SDK_VRCSDK3 && !UDON
                EditorGUI.BeginChangeCheck();
                GUI.enabled = !isLocked;
                ToggleGUI(GetLoc("sShaderSettingOptimizeInTestBuild"), ref shaderSetting.isOptimizeInTestBuild);
                GUI.enabled = true;
                if(EditorGUI.EndChangeCheck()) lilToonSetting.SaveShaderSetting(shaderSetting);
            #endif

            EditorGUI.BeginChangeCheck();
            ToggleGUI(GetLoc("sShaderSettingOptimizeInEditor"), ref shaderSetting.isDebugOptimize);
            edSet.isShowShaderSetting = lilEditorGUI.Foldout(GetLoc("sShaderSetting"), edSet.isShowShaderSetting);
            lilEditorGUI.DrawHelpButton(GetLoc("sAnchorShaderSetting"));
            if(edSet.isShowShaderSetting)
            {
                EditorGUILayout.BeginVertical(customBox);
                GUI.enabled = !isLocked;
                ToggleGUI(GetLoc("sSettingClippingCanceller"), ref shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER);
                GUI.enabled = true;
                EditorGUILayout.EndVertical();
            }

            edSet.isShowOptimizationSetting = lilEditorGUI.Foldout(GetLoc("sSettingBuildSizeOptimization"), edSet.isShowOptimizationSetting);
            if(edSet.isShowOptimizationSetting)
            {
                EditorGUILayout.BeginVertical(customBox);
                EditorGUILayout.HelpBox(GetLoc("sHelpShaderSetting"),MessageType.Info);
                ShaderSettingOptimizationGUI();
                EditorGUILayout.EndVertical();
            }
            if(EditorGUI.EndChangeCheck())
            {
                if(shaderSetting.isDebugOptimize)
                {
                    lilToonSetting.ApplyShaderSettingOptimized();
                }
                else
                {
                    lilToonSetting.TurnOnAllShaderSetting(ref shaderSetting);
                    lilToonSetting.ApplyShaderSetting(shaderSetting);
                }
            }

            edSet.isShowDefaultValueSetting = lilEditorGUI.Foldout(GetLoc("sSettingDefaultValue"), edSet.isShowDefaultValueSetting);
            if(edSet.isShowDefaultValueSetting)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginVertical(customBox);
                ShaderSettingDefaultValueGUI();
                EditorGUILayout.EndVertical();
                if(EditorGUI.EndChangeCheck()) lilToonSetting.SaveShaderSetting(shaderSetting);
            }
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Language
        #region
        public static string GetLoc(string value)                   { return lilLanguageManager.GetLoc(value); }
        public static string BuildParams(params string[] labels)    { return lilLanguageManager.BuildParams(labels); }
        public static void LoadCustomLanguage(string langFileGUID)  { lilLanguageManager.LoadCustomLanguage(langFileGUID); }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Shader Setting GUI
        #region
        private static void ShaderSettingOptimizationGUI()
        {
            GUI.enabled = !File.Exists(lilDirectoryManager.GetSettingLockPath());
            lilRenderPipeline RP = lilRenderPipelineReader.GetRP();
            if(RP == lilRenderPipeline.BRP)
            {
                ToggleGUI(GetLoc("sSettingApplyShadowFA"), ref shaderSetting.LIL_OPTIMIZE_APPLY_SHADOW_FA);
                ToggleGUI(GetLoc("sSettingUseForwardAdd"), ref shaderSetting.LIL_OPTIMIZE_USE_FORWARDADD);
                ToggleGUI(GetLoc("sSettingUseForwardAddShadow"), ref shaderSetting.LIL_OPTIMIZE_USE_FORWARDADD_SHADOW);
            }
            ToggleGUI(GetLoc("sSettingUseLightmap"), ref shaderSetting.LIL_OPTIMIZE_USE_LIGHTMAP);
            GUI.enabled = true;
        }

        private static void ShaderSettingDefaultValueGUI()
        {
            EditorGUILayout.LabelField("[GameObject] Fix lighting", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            shaderSetting.defaultAsUnlit                        = EditorGUILayout.Slider(GetLoc("sAsUnlit"), shaderSetting.defaultAsUnlit, 0.0f, 1.0f);
            shaderSetting.defaultVertexLightStrength            = EditorGUILayout.Slider(GetLoc("sVertexLightStrength"), shaderSetting.defaultVertexLightStrength, 0.0f, 1.0f);
            shaderSetting.defaultLightMinLimit                  = EditorGUILayout.Slider(GetLoc("sLightMinLimit"), shaderSetting.defaultLightMinLimit, 0.0f, 1.0f);
            shaderSetting.defaultLightMaxLimit                  = EditorGUILayout.Slider(GetLoc("sLightMaxLimit"), shaderSetting.defaultLightMaxLimit, 0.0f, 10.0f);
            shaderSetting.defaultMonochromeLighting             = EditorGUILayout.Slider(GetLoc("sMonochromeLighting"), shaderSetting.defaultMonochromeLighting, 0.0f, 1.0f);
            shaderSetting.defaultBeforeExposureLimit            = EditorGUILayout.FloatField(GetLoc("sBeforeExposureLimit"), shaderSetting.defaultBeforeExposureLimit);
            shaderSetting.defaultlilDirectionalLightStrength    = EditorGUILayout.Slider(GetLoc("sDirectionalLightStrength"), shaderSetting.defaultlilDirectionalLightStrength, 0.0f, 1.0f);
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField("[Model] Setup from FBX", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            shaderSetting.presetFace = (lilToonPreset)EditorGUILayout.ObjectField("Face", shaderSetting.presetFace, typeof(lilToonPreset), false);
            shaderSetting.presetSkin = (lilToonPreset)EditorGUILayout.ObjectField("Skin", shaderSetting.presetSkin, typeof(lilToonPreset), false);
            shaderSetting.presetHair = (lilToonPreset)EditorGUILayout.ObjectField("Hair", shaderSetting.presetHair, typeof(lilToonPreset), false);
            shaderSetting.presetCloth = (lilToonPreset)EditorGUILayout.ObjectField("Cloth", shaderSetting.presetCloth, typeof(lilToonPreset), false);
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField("[Shader] LightMode Override", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            GUI.enabled = !File.Exists(lilDirectoryManager.GetSettingLockPath());
            shaderSetting.mainLightModeName = EditorGUILayout.TextField("Main", shaderSetting.mainLightModeName);
            shaderSetting.outlineLightModeName = EditorGUILayout.TextField("Outline", shaderSetting.outlineLightModeName);
            shaderSetting.preLightModeName = EditorGUILayout.TextField("Transparent backface", shaderSetting.preLightModeName);
            shaderSetting.furLightModeName = EditorGUILayout.TextField("Fur", shaderSetting.furLightModeName);
            shaderSetting.furPreLightModeName = EditorGUILayout.TextField("Fur Pre", shaderSetting.furPreLightModeName);
            shaderSetting.gemPreLightModeName = EditorGUILayout.TextField("Gem Pre", shaderSetting.gemPreLightModeName);
            GUI.enabled = true;
            EditorGUI.indentLevel--;
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // GUI
        #region

        // For custom shader
        public static bool Foldout(string title, bool display) { return lilEditorGUI.Foldout(title, display); }
        public static bool Foldout(string title, string help, bool display) { return lilEditorGUI.Foldout(title, help, display); }
        public static void DrawLine() { lilEditorGUI.DrawLine(); }

        private static void ToggleGUI(string label, ref bool value) { value = EditorGUILayout.ToggleLeft(label, value); }
        private void OpenHelpPage(object helpAnchor) { Application.OpenURL(GetLoc("sManualURL") + helpAnchor); }

        private static void DrawWebPages()
        {
            VersionCheck();
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label){fontStyle = FontStyle.Bold};
            string versionLabel = "lilToon " + lilConstants.currentVersionName;
            if(latestVersion != null && latestVersion.latest_vertion_name != null && latestVersion.latest_vertion_value > lilConstants.currentVersionValue)
            {
                versionLabel = "[Update] lilToon " + lilConstants.currentVersionName + " -> " + latestVersion.latest_vertion_name;
                labelStyle.normal.textColor = Color.red;
            }

            edSet.isShowWebPages = lilEditorGUI.DrawSimpleFoldout(versionLabel, edSet.isShowWebPages, labelStyle, isCustomEditor);
            if(edSet.isShowWebPages)
            {
                EditorGUI.indentLevel++;
                lilEditorGUI.DrawWebButton("BOOTH", lilConstants.boothURL);
                lilEditorGUI.DrawWebButton("GitHub", lilConstants.githubURL);
                EditorGUI.indentLevel--;
            }
        }

        private static void VersionCheck()
        {
            if(string.IsNullOrEmpty(latestVersion.latest_vertion_name))
            {
                if(File.Exists(lilDirectoryManager.versionInfoTempPath))
                {
                    StreamReader sr = new StreamReader(lilDirectoryManager.versionInfoTempPath);
                    string s = sr.ReadToEnd();
                    sr.Close();
                    if(!string.IsNullOrEmpty(s) && s.Contains("latest_vertion_name") && s.Contains("latest_vertion_value"))
                    {
                        EditorJsonUtility.FromJsonOverwrite(s,latestVersion);
                        return;
                    }
                }
                latestVersion.latest_vertion_name = lilConstants.currentVersionName;
                latestVersion.latest_vertion_value = lilConstants.currentVersionValue;
                return;
            }
        }

        private static void DrawHelpPages()
        {
            edSet.isShowHelpPages = lilEditorGUI.DrawSimpleFoldout(GetLoc("sHelp"), edSet.isShowHelpPages, isCustomEditor);
            if(edSet.isShowHelpPages)
            {
                EditorGUI.indentLevel++;
                lilEditorGUI.DrawWebButton(GetLoc("sCommonProblems"), GetLoc("sReadmeURL") + GetLoc("sReadmeAnchorProblem"));
                EditorGUI.indentLevel--;
            }
        }

        private static void DrawShaderTypeWarn(Material material)
        {
            if(!isMultiVariants && material.shader.name.Contains("Overlay") && lilEditorGUI.AutoFixHelpBox(GetLoc("sHelpSelectOverlay")))
            {
                material.shader = lts;
            }
        }

        private static void SelectEditorMode()
        {
            string[] sEditorModeList = {GetLoc("sEditorModeSimple"),GetLoc("sEditorModeAdvanced"),GetLoc("sEditorModePreset"),GetLoc("sEditorModeShaderSetting")};
            edSet.editorMode = (EditorMode)GUILayout.Toolbar((int)edSet.editorMode, sEditorModeList);
        }

        private void DrawMenuButton(string helpAnchor, PropertyBlock propertyBlock)
        {
            Rect position = GUILayoutUtility.GetLastRect();
            position.x += position.width - 24;
            position.width = 24;

            if(GUI.Button(position, EditorGUIUtility.IconContent("_Popup"), middleButton))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent(GetLoc("sCopy")),               false, CopyProperties,  propertyBlock);
                menu.AddItem(new GUIContent(GetLoc("sPaste")),              false, PasteProperties, new PropertyBlockData{propertyBlock = propertyBlock, shouldCopyTex = false});
                menu.AddItem(new GUIContent(GetLoc("sPasteWithTexture")),   false, PasteProperties, new PropertyBlockData{propertyBlock = propertyBlock, shouldCopyTex = true});
                #if UNITY_2019_3_OR_NEWER
                    menu.AddItem(new GUIContent(GetLoc("sReset")),              false, ResetProperties, propertyBlock);
                #endif
                menu.AddItem(new GUIContent(GetLoc("sOpenManual")),         false, OpenHelpPage,    helpAnchor);
                menu.ShowAsContext();
            }
        }

        private void DrawVRCFallbackGUI(Material material)
        {
            #if VRC_SDK_VRCSDK2 || VRC_SDK_VRCSDK3 || VRC_SDK_VRCSDK4
                edSet.isShowVRChat = lilEditorGUI.Foldout("VRChat", "VRChat", edSet.isShowVRChat);
                if(edSet.isShowVRChat)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    string tag = material.GetTag("VRCFallback", false);
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = m_MaterialEditor.targets.Where(obj => obj is Material).Any(obj => tag != ((Material)obj).GetTag("VRCFallback", false));
                    bool shouldSetTag = EditorGUI.ToggleLeft(EditorGUILayout.GetControlRect(), "Custom Safety Fallback", !string.IsNullOrEmpty(tag), customToggleFont);
                    if(shouldSetTag)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        string[] sFallbackShaderTypes = {"Unlit", "Standard", "VertexLit", "Toon", "Particle", "Sprite", "Matcap", "MobileToon", "Hidden"};
                        string[] sFallbackRenderTypes = {"Opaque", "Cutout", "Transparent", "Fade"};
                        string[] sFallbackCullTypes = {"Default", "DoubleSided"};

                        int fallbackShaderType = tag.Contains("Standard")       ? 1 : 0;
                            fallbackShaderType = tag.Contains("VertexLit")      ? 2 : fallbackShaderType;
                            fallbackShaderType = tag.Contains("Toon")           ? 3 : fallbackShaderType;
                            fallbackShaderType = tag.Contains("Particle")       ? 4 : fallbackShaderType;
                            fallbackShaderType = tag.Contains("Sprite")         ? 5 : fallbackShaderType;
                            fallbackShaderType = tag.Contains("Matcap")         ? 6 : fallbackShaderType;
                            fallbackShaderType = tag.Contains("MobileToon")     ? 7 : fallbackShaderType;
                            fallbackShaderType = tag.Contains("Hidden")         ? 8 : fallbackShaderType;

                        int fallbackRenderType = tag.Contains("Cutout")         ? 1 : 0;
                            fallbackRenderType = tag.Contains("Transparent")    ? 2 : fallbackRenderType;
                            fallbackRenderType = tag.Contains("Fade")           ? 3 : fallbackRenderType;

                        int fallbackCullType = tag.Contains("DoubleSided") ? 1 : 0;

                        fallbackShaderType = EditorGUILayout.Popup("Shader Type", fallbackShaderType, sFallbackShaderTypes);
                        fallbackRenderType = EditorGUILayout.Popup("Rendering Mode", fallbackRenderType, sFallbackRenderTypes);
                        fallbackCullType = EditorGUILayout.Popup("Facing", fallbackCullType, sFallbackCullTypes);

                        switch(fallbackShaderType)
                        {
                            case 0: tag = "Unlit"; break;
                            case 1: tag = "Standard"; break;
                            case 2: tag = "VertexLit"; break;
                            case 3: tag = "Toon"; break;
                            case 4: tag = "Particle"; break;
                            case 5: tag = "Sprite"; break;
                            case 6: tag = "Matcap"; break;
                            case 7: tag = "MobileToon"; break;
                            case 8: tag = "Hidden"; break;
                            default: tag = "Unlit"; break;
                        }
                        switch(fallbackRenderType)
                        {
                            case 0: break;
                            case 1: tag += "Cutout"; break;
                            case 2: tag += "Transparent"; break;
                            case 3: tag += "Fade"; break;
                            default: break;
                        }
                        switch(fallbackCullType)
                        {
                            case 0: break;
                            case 1: tag += "DoubleSided"; break;
                            default: break;
                        }
                        EditorGUILayout.LabelField("Result:", '"' + tag + '"');
                        EditorGUILayout.EndVertical();
                    }
                    else
                    {
                        tag = "";
                    }
                    EditorGUI.showMixedValue = false;
                    if(EditorGUI.EndChangeCheck())
                    {
                        foreach(var obj in m_MaterialEditor.targets.Where(obj => obj is Material))
                        {
                            ((Material)obj).SetOverrideTag("VRCFallback", tag);
                            EditorUtility.SetDirty(obj);
                        }
                        AssetDatabase.SaveAssets();
                    }
                    EditorGUILayout.EndVertical();
                }
            #endif
        }

        private void DrawOptimizationButton(Material material, bool isnormal)
        {
            #if VRC_SDK_VRCSDK3 && UDON
                if(isnormal && lilEditorGUI.EditorButton(GetLoc("sOptimizeForEvents"))) lilMaterialUtils.RemoveUnusedTexture(material);
            #endif
        }

        private void DrawGIFragGUI()
        {
            #if NITY_2019_1_OR_NEWER
                m_MaterialEditor.LightmapEmissionFlagsProperty(0, true, true);
            #else
                m_MaterialEditor.LightmapEmissionFlagsProperty(0, true);
            #endif
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Editor
        #region
        public static void ApplyEditorSettingTemp()
        {
            if(!lilLanguageManager.ShouldApplyTemp()) return;
            lilLanguageManager.ApplySettingTemp();
            if(!File.Exists(lilDirectoryManager.editorSettingTempPath)) return;
            StreamReader sr = new StreamReader(lilDirectoryManager.editorSettingTempPath);
            string s = sr.ReadToEnd();
            sr.Close();
            if(!string.IsNullOrEmpty(s)) EditorJsonUtility.FromJsonOverwrite(s,edSet);
        }

        public static void SaveEditorSettingTemp()
        {
            lilLanguageManager.SaveSettingTemp();
            StreamWriter sw = new StreamWriter(lilDirectoryManager.editorSettingTempPath,false);
            sw.Write(EditorJsonUtility.ToJson(edSet));
            sw.Close();
        }

        private void CheckShaderType(Material material)
        {
            isLite          = material.shader.name.Contains("Lite");
            isCutout        = material.shader.name.Contains("Cutout");
            isTransparent   = material.shader.name.Contains("Transparent") || material.shader.name.Contains("Overlay");
            isOutl          = !isMultiVariants && material.shader.name.Contains("Outline");
            isRefr          = !isMultiVariants && material.shader.name.Contains("Refraction");
            isBlur          = !isMultiVariants && material.shader.name.Contains("Blur");
            isFur           = !isMultiVariants && material.shader.name.Contains("Fur");
            isTess          = !isMultiVariants && material.shader.name.Contains("Tessellation");
            isGem           = !isMultiVariants && material.shader.name.Contains("Gem");
            isFakeShadow    = !isMultiVariants && material.shader.name.Contains("FakeShadow");
            isOnePass       = material.shader.name.Contains("OnePass");
            isTwoPass       = material.shader.name.Contains("TwoPass");
            isMulti         = material.shader.name.Contains("Multi");
            isCustomShader  = material.shader.name.Contains("Optional");
            isShowRenderMode = !isCustomShader;
            isStWr          = stencilPass.floatValue == (float)UnityEngine.Rendering.StencilOp.Replace;

                                    renderingModeBuf = RenderingMode.Opaque;
            if(isCutout)            renderingModeBuf = RenderingMode.Cutout;
            if(isTransparent)       renderingModeBuf = RenderingMode.Transparent;
            if(isRefr)              renderingModeBuf = RenderingMode.Refraction;
            if(isRefr && isBlur)    renderingModeBuf = RenderingMode.RefractionBlur;
            if(isFur)               renderingModeBuf = RenderingMode.Fur;
            if(isFur && isCutout)   renderingModeBuf = RenderingMode.FurCutout;
            if(isFur && isTwoPass)  renderingModeBuf = RenderingMode.FurTwoPass;
            if(isGem)               renderingModeBuf = RenderingMode.Gem;

                                    transparentModeBuf = TransparentMode.Normal;
            if(isOnePass)           transparentModeBuf = TransparentMode.OnePass;
            if(!isFur && isTwoPass) transparentModeBuf = TransparentMode.TwoPass;

            float tpmode = 0.0f;
            if(material.HasProperty("_TransparentMode")) tpmode = material.GetFloat("_TransparentMode");

            isUseAlpha =
                renderingModeBuf == RenderingMode.Cutout ||
                renderingModeBuf == RenderingMode.Transparent ||
                renderingModeBuf == RenderingMode.Fur ||
                renderingModeBuf == RenderingMode.FurCutout ||
                renderingModeBuf == RenderingMode.FurTwoPass ||
                (isMulti && tpmode != 0.0f && tpmode != 3.0f && tpmode != 6.0f);

            if(isMulti)
            {
                isCutout = tpmode == 1.0f || tpmode == 5.0f;
                isTransparent = tpmode == 2.0f;
            }
        }

        private void CopyProperties(PropertyBlock propertyBlock)
        {
            foreach(lilMaterialProperty prop in AllProperties())
            {
                foreach(PropertyBlock block in prop.blocks)
                {
                    if(block == propertyBlock && prop.p != null) copiedProperties[prop.name] = prop.p;
                }
            }
        }

        private void PasteProperties(PropertyBlock propertyBlock, bool shouldCopyTex)
        {
            foreach(lilMaterialProperty prop in AllProperties())
            {
                if(!shouldCopyTex && prop.isTexture)
                {
                    Debug.Log("Skip Texture");
                    continue;
                }
                foreach(PropertyBlock block in prop.blocks)
                {
                    if(block == propertyBlock && prop.p != null && copiedProperties.ContainsKey(prop.name) && copiedProperties[prop.name] != null)
                    {
                        MaterialProperty.PropType propType = prop.type;
                        if(propType == MaterialProperty.PropType.Color)     prop.colorValue = copiedProperties[prop.name].colorValue;
                        if(propType == MaterialProperty.PropType.Vector)    prop.vectorValue = copiedProperties[prop.name].vectorValue;
                        if(propType == MaterialProperty.PropType.Float)     prop.floatValue = copiedProperties[prop.name].floatValue;
                        if(propType == MaterialProperty.PropType.Range)     prop.floatValue = copiedProperties[prop.name].floatValue;
                        if(propType == MaterialProperty.PropType.Texture)   prop.textureValue = copiedProperties[prop.name].textureValue;
                    }
                }
            }
        }

        private void ResetProperties(PropertyBlock propertyBlock)
        {
            #if UNITY_2019_3_OR_NEWER
            foreach(lilMaterialProperty prop in AllProperties())
            {
                foreach(PropertyBlock block in prop.blocks)
                {
                    if(block == propertyBlock && prop.p != null && prop.targets[0] is Material && ((Material)prop.targets[0]).shader != null)
                    {
                        Shader shader = ((Material)prop.targets[0]).shader;
                        int propID = shader.FindPropertyIndex(prop.name);
                        if(propID == -1) return;
                        MaterialProperty.PropType propType = prop.type;
                        if(propType == MaterialProperty.PropType.Color)     prop.colorValue = shader.GetPropertyDefaultVectorValue(propID);
                        if(propType == MaterialProperty.PropType.Vector)    prop.vectorValue = shader.GetPropertyDefaultVectorValue(propID);
                        if(propType == MaterialProperty.PropType.Float)     prop.floatValue = shader.GetPropertyDefaultFloatValue(propID);
                        if(propType == MaterialProperty.PropType.Range)     prop.floatValue = shader.GetPropertyDefaultFloatValue(propID);
                        if(propType == MaterialProperty.PropType.Texture)   prop.textureValue = null;
                    }
                }
            }
            #endif
        }

        private void CopyProperties(object obj)
        {
            CopyProperties((PropertyBlock)obj);
        }

        private void PasteProperties(object obj)
        {
            PropertyBlockData propertyBlockData = (PropertyBlockData)obj;
            PasteProperties(propertyBlockData.propertyBlock, propertyBlockData.shouldCopyTex);
        }

        private void ResetProperties(object obj)
        {
            ResetProperties((PropertyBlock)obj);
        }

        private void ApplyLightingPreset(LightingPreset lightingPreset)
        {
            switch(lightingPreset)
            {
                case LightingPreset.Default:
                    if(asUnlit.p != null) asUnlit.floatValue = shaderSetting.defaultAsUnlit;
                    if(vertexLightStrength.p != null) vertexLightStrength.floatValue = shaderSetting.defaultVertexLightStrength;
                    if(lightMinLimit.p != null) lightMinLimit.floatValue = shaderSetting.defaultLightMinLimit;
                    if(lightMaxLimit.p != null) lightMaxLimit.floatValue = shaderSetting.defaultLightMaxLimit;
                    if(beforeExposureLimit.p != null) beforeExposureLimit.floatValue = shaderSetting.defaultBeforeExposureLimit;
                    if(monochromeLighting.p != null) monochromeLighting.floatValue = shaderSetting.defaultMonochromeLighting;
                    if(shadowEnvStrength.p != null) shadowEnvStrength.floatValue = 0.0f;
                    if(lilDirectionalLightStrength.p != null) lilDirectionalLightStrength.floatValue = shaderSetting.defaultlilDirectionalLightStrength;
                    break;
                case LightingPreset.SemiMonochrome:
                    if(asUnlit.p != null) asUnlit.floatValue = 0.0f;
                    if(vertexLightStrength.p != null) vertexLightStrength.floatValue = 0.0f;
                    if(lightMinLimit.p != null) lightMinLimit.floatValue = 0.05f;
                    if(lightMaxLimit.p != null) lightMaxLimit.floatValue = 1.0f;
                    if(beforeExposureLimit.p != null) beforeExposureLimit.floatValue = 10000.0f;
                    if(monochromeLighting.p != null) monochromeLighting.floatValue = 0.5f;
                    if(shadowEnvStrength.p != null) shadowEnvStrength.floatValue = 0.0f;
                    if(lilDirectionalLightStrength.p != null) lilDirectionalLightStrength.floatValue = 1.0f;
                    break;
            }
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Material Setup
        #region
        public static void SetupMaterialWithRenderingMode(Material material, RenderingMode renderingMode, TransparentMode transparentMode, bool isoutl, bool islite, bool istess, bool ismulti)
        {
            if(!isMultiVariants) lilMaterialUtils.SetupMaterialWithRenderingMode(material, renderingMode, transparentMode, isoutl, islite, istess, ismulti);
        }

        public static void SetupMaterialWithRenderingMode(Material material, RenderingMode renderingMode, TransparentMode transparentMode, bool isoutl, bool islite, bool istess)
        {
            SetupMaterialWithRenderingMode(material, renderingMode, transparentMode, isoutl, islite, istess, isMulti);
        }

        public static void SetupMaterialWithRenderingMode(Material material, RenderingMode renderingMode, TransparentMode transparentMode)
        {
            SetupMaterialWithRenderingMode(material, renderingMode, transparentMode, isOutl, isLite, isTess);
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Presets
        #region
        private static void DrawPreset()
        {
            GUILayout.Label(GetLoc("sPresets"), boldLabel);
            if(presets == null) presets = lilToonPreset.LoadPresets();
            ShowPresets();
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            if(lilEditorGUI.EditorButton(GetLoc("sPresetRefresh"))) presets = lilToonPreset.LoadPresets();
            if(lilEditorGUI.EditorButton(GetLoc("sPresetSave"))) EditorWindow.GetWindow<lilToonPreset.lilPresetWindow>("[lilToon] Preset Window");
            GUILayout.EndHorizontal();
        }

        private static void ShowPresets()
        {
            string[] sCategorys = { GetLoc("sPresetCategorySkin"),
                                    GetLoc("sPresetCategoryHair"),
                                    GetLoc("sPresetCategoryCloth"),
                                    GetLoc("sPresetCategoryNature"),
                                    GetLoc("sPresetCategoryInorganic"),
                                    GetLoc("sPresetCategoryEffect"),
                                    GetLoc("sPresetCategoryOther") };
            for(int i=0; i<(int)lilPresetCategory.Other+1; i++)
            {
                edSet.isShowCategorys[i] = lilEditorGUI.Foldout(sCategorys[i], edSet.isShowCategorys[i]);
                if(edSet.isShowCategorys[i])
                {
                    for(int j=0; j<presets.Length; j++)
                    {
                        if(i == (int)presets[j].category)
                        {
                            string showName = "";
                            for(int k=0; k<presets[j].bases.Length; k++)
                            {
                                if(string.IsNullOrEmpty(showName))
                                {
                                    showName = presets[j].bases[k].name;
                                }
                                if(presets[j].bases[k].language == lilLanguageManager.langSet.languageName)
                                {
                                    showName = presets[j].bases[k].name;
                                    k = 256;
                                }
                            }
                            if(lilEditorGUI.EditorButton(showName))
                            {
                                var objs = m_MaterialEditor.targets.Where(obj => obj is Material);
                                foreach(Object obj in objs)
                                {
                                    lilToonPreset.ApplyPreset((Material)obj, presets[j], isMulti);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Material Converter
        #region
        private void CreateMToonMaterial(Material material)
        {
            Material mtoonMaterial = new Material(mtoon);

            string matPath = AssetDatabase.GetAssetPath(material);
            if(!string.IsNullOrEmpty(matPath))  matPath = EditorUtility.SaveFilePanel("Save Material", Path.GetDirectoryName(matPath), Path.GetFileNameWithoutExtension(matPath)+"_mtoon", "mat");
            else                                matPath = EditorUtility.SaveFilePanel("Save Material", "Assets", material.name + ".mat", "mat");
            if(!string.IsNullOrEmpty(matPath))  AssetDatabase.CreateAsset(mtoonMaterial, FileUtil.GetProjectRelativePath(matPath));

            mtoonMaterial.SetColor("_Color",                    mainColor.colorValue);
            mtoonMaterial.SetFloat("_LightColorAttenuation",    0.0f);
            mtoonMaterial.SetFloat("_IndirectLightIntensity",   0.0f);

            mtoonMaterial.SetFloat("_UvAnimScrollX",            mainTex_ScrollRotate.vectorValue.x);
            mtoonMaterial.SetFloat("_UvAnimScrollY",            mainTex_ScrollRotate.vectorValue.y);
            mtoonMaterial.SetFloat("_UvAnimRotation",           mainTex_ScrollRotate.vectorValue.w / Mathf.PI * 0.5f);
            mtoonMaterial.SetFloat("_MToonVersion",             35.0f);
            mtoonMaterial.SetFloat("_DebugMode",                0.0f);
            mtoonMaterial.SetFloat("_CullMode",                 cull.floatValue);

            Texture2D bakedMainTex = AutoBakeMainTexture(material);
            mtoonMaterial.SetTexture("_MainTex", bakedMainTex);

            Vector2 mainScale = material.GetTextureScale(mainTex.name);
            Vector2 mainOffset = material.GetTextureOffset(mainTex.name);
            mtoonMaterial.SetTextureScale(mainTex.name, mainScale);
            mtoonMaterial.SetTextureOffset(mainTex.name, mainOffset);

            if(useBumpMap.floatValue == 1.0f && bumpMap.textureValue != null)
            {
                mtoonMaterial.SetFloat("_BumpScale", bumpScale.floatValue);
                mtoonMaterial.SetTexture("_BumpMap", bumpMap.textureValue);
                mtoonMaterial.EnableKeyword("_NORMALMAP");
            }

            if(useShadow.floatValue == 1.0f)
            {
                float shadeShift = (Mathf.Clamp01(shadowBorder.floatValue - (shadowBlur.floatValue * 0.5f)) * 2.0f) - 1.0f;
                float shadeToony = (2.0f - (Mathf.Clamp01(shadowBorder.floatValue + (shadowBlur.floatValue * 0.5f)) * 2.0f)) / (1.0f - shadeShift);
                if(shadowStrengthMask.textureValue != null || shadowMainStrength.floatValue != 0.0f)
                {
                    Texture2D bakedShadowTex = AutoBakeShadowTexture(material, bakedMainTex);
                    mtoonMaterial.SetColor("_ShadeColor",               Color.white);
                    mtoonMaterial.SetTexture("_ShadeTexture",           bakedShadowTex);
                }
                else
                {
                    Color shadeColorStrength = new Color(
                        1.0f - ((1.0f - shadowColor.colorValue.r) * shadowStrength.floatValue),
                        1.0f - ((1.0f - shadowColor.colorValue.g) * shadowStrength.floatValue),
                        1.0f - ((1.0f - shadowColor.colorValue.b) * shadowStrength.floatValue),
                        1.0f
                    );
                    mtoonMaterial.SetColor("_ShadeColor",               shadeColorStrength);
                    if(shadowColorTex.textureValue != null)
                    {
                        mtoonMaterial.SetTexture("_ShadeTexture",           shadowColorTex.textureValue);
                    }
                    else
                    {
                        mtoonMaterial.SetTexture("_ShadeTexture",           bakedMainTex);
                    }
                }
                mtoonMaterial.SetFloat("_ReceiveShadowRate",        1.0f);
                mtoonMaterial.SetTexture("_ReceiveShadowTexture",   null);
                mtoonMaterial.SetFloat("_ShadingGradeRate",         1.0f);
                mtoonMaterial.SetTexture("_ShadingGradeTexture",    shadowBorderMask.textureValue);
                mtoonMaterial.SetFloat("_ShadeShift",               shadeShift);
                mtoonMaterial.SetFloat("_ShadeToony",               shadeToony);
            }
            else
            {
                mtoonMaterial.SetColor("_ShadeColor",               Color.white);
                mtoonMaterial.SetTexture("_ShadeTexture",           bakedMainTex);
            }

            if(useEmission.floatValue == 1.0f && emissionMap.textureValue != null)
            {
                mtoonMaterial.SetColor("_EmissionColor",            emissionColor.colorValue);
                mtoonMaterial.SetTexture("_EmissionMap",            emissionMap.textureValue);
            }

            if(useRim.floatValue == 1.0f)
            {
                mtoonMaterial.SetColor("_RimColor",                 rimColor.colorValue);
                mtoonMaterial.SetTexture("_RimTexture",             rimColorTex.textureValue);
                mtoonMaterial.SetFloat("_RimLightingMix",           rimEnableLighting.floatValue);

                float rimFP = rimFresnelPower.floatValue / Mathf.Max(0.001f, rimBlur.floatValue);
                float rimLift = Mathf.Pow(1.0f - rimBorder.floatValue, rimFresnelPower.floatValue) * (1.0f - rimBlur.floatValue);
                mtoonMaterial.SetFloat("_RimFresnelPower",          rimFP);
                mtoonMaterial.SetFloat("_RimLift",                  rimLift);
            }
            else
            {
                mtoonMaterial.SetColor("_RimColor",                 Color.black);
            }

            if(useMatCap.floatValue == 1.0f && matcapBlendMode.floatValue != 3.0f && matcapTex.textureValue != null)
            {
                Texture2D bakedMatCap = AutoBakeMatCap(material);
                mtoonMaterial.SetTexture("_SphereAdd", bakedMatCap);
            }

            if(isOutl)
            {
                mtoonMaterial.SetTexture("_OutlineWidthTexture",    outlineWidthMask.textureValue);
                mtoonMaterial.SetFloat("_OutlineWidth",             outlineWidth.floatValue);
                mtoonMaterial.SetColor("_OutlineColor",             outlineColor.colorValue);
                mtoonMaterial.SetFloat("_OutlineLightingMix",       1.0f);
                mtoonMaterial.SetFloat("_OutlineWidthMode",         1.0f);
                mtoonMaterial.SetFloat("_OutlineColorMode",         1.0f);
                mtoonMaterial.SetFloat("_OutlineCullMode",          1.0f);
                mtoonMaterial.EnableKeyword("MTOON_OUTLINE_WIDTH_WORLD");
                mtoonMaterial.EnableKeyword("MTOON_OUTLINE_COLOR_MIXED");
            }

            if(isCutout)
            {
                mtoonMaterial.SetFloat("_Cutoff", cutoff.floatValue);
                mtoonMaterial.SetFloat("_BlendMode", 1.0f);
                mtoonMaterial.SetOverrideTag("RenderType", "TransparentCutout");
                mtoonMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                mtoonMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                mtoonMaterial.SetFloat("_ZWrite", 1.0f);
                mtoonMaterial.SetFloat("_AlphaToMask", 1.0f);
                mtoonMaterial.EnableKeyword("_ALPHATEST_ON");
                mtoonMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
            }
            else if(isTransparent && zwrite.floatValue == 0.0f)
            {
                mtoonMaterial.SetFloat("_BlendMode", 2.0f);
                mtoonMaterial.SetOverrideTag("RenderType", "TransparentCutout");
                mtoonMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mtoonMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mtoonMaterial.SetFloat("_ZWrite", 0.0f);
                mtoonMaterial.SetFloat("_AlphaToMask", 0.0f);
                mtoonMaterial.EnableKeyword("_ALPHABLEND_ON");
                mtoonMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            else if(isTransparent && zwrite.floatValue != 0.0f)
            {
                mtoonMaterial.SetFloat("_BlendMode", 3.0f);
                mtoonMaterial.SetOverrideTag("RenderType", "TransparentCutout");
                mtoonMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mtoonMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mtoonMaterial.SetFloat("_ZWrite", 1.0f);
                mtoonMaterial.SetFloat("_AlphaToMask", 0.0f);
                mtoonMaterial.EnableKeyword("_ALPHABLEND_ON");
                mtoonMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            else
            {
                mtoonMaterial.SetFloat("_BlendMode", 0.0f);
                mtoonMaterial.SetOverrideTag("RenderType", "Opaque");
                mtoonMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                mtoonMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                mtoonMaterial.SetFloat("_ZWrite", 1.0f);
                mtoonMaterial.SetFloat("_AlphaToMask", 0.0f);
                mtoonMaterial.renderQueue = -1;
            }
        }

        private void CreateLiteMaterial(Material material)
        {
            Material liteMaterial = new Material(ltsl);
            RenderingMode renderingMode = renderingModeBuf;
            if(renderingMode == RenderingMode.Refraction)       renderingMode = RenderingMode.Transparent;
            if(renderingMode == RenderingMode.RefractionBlur)   renderingMode = RenderingMode.Transparent;
            if(renderingMode == RenderingMode.Fur)              renderingMode = RenderingMode.Transparent;
            if(renderingMode == RenderingMode.FurCutout)        renderingMode = RenderingMode.Cutout;
            if(renderingMode == RenderingMode.FurTwoPass)       renderingMode = RenderingMode.Transparent;

            bool isonepass      = material.shader.name.Contains("OnePass");
            bool istwopass      = material.shader.name.Contains("TwoPass");

            TransparentMode     transparentMode = TransparentMode.Normal;
            if(isonepass)       transparentMode = TransparentMode.OnePass;
            if(istwopass)       transparentMode = TransparentMode.TwoPass;

            SetupMaterialWithRenderingMode(liteMaterial, renderingMode, transparentMode, isOutl, true, isStWr, false);

            string matPath = AssetDatabase.GetAssetPath(material);
            if(!string.IsNullOrEmpty(matPath))  matPath = EditorUtility.SaveFilePanel("Save Material", Path.GetDirectoryName(matPath), Path.GetFileNameWithoutExtension(matPath)+"_lite", "mat");
            else                                matPath = EditorUtility.SaveFilePanel("Save Material", "Assets", material.name + ".mat", "mat");
            if(string.IsNullOrEmpty(matPath))   return;
            else                                AssetDatabase.CreateAsset(liteMaterial, FileUtil.GetProjectRelativePath(matPath));

            liteMaterial.SetFloat("_Invisible",                 invisible.floatValue);
            liteMaterial.SetFloat("_Cutoff",                    cutoff.floatValue);
            liteMaterial.SetFloat("_SubpassCutoff",             subpassCutoff.floatValue);
            liteMaterial.SetFloat("_Cull",                      cull.floatValue);
            liteMaterial.SetFloat("_FlipNormal",                flipNormal.floatValue);
            liteMaterial.SetFloat("_BackfaceForceShadow",       backfaceForceShadow.floatValue);

            liteMaterial.SetColor("_Color",                     mainColor.colorValue);
            liteMaterial.SetVector("_MainTex_ScrollRotate",     mainTex_ScrollRotate.vectorValue);

            Texture2D bakedMainTex = AutoBakeMainTexture(material);
            liteMaterial.SetTexture("_MainTex", bakedMainTex);

            Vector2 mainScale = material.GetTextureScale(mainTex.name);
            Vector2 mainOffset = material.GetTextureOffset(mainTex.name);
            liteMaterial.SetTextureScale(mainTex.name, mainScale);
            liteMaterial.SetTextureOffset(mainTex.name, mainOffset);

            liteMaterial.SetFloat("_UseShadow",                 useShadow.floatValue);
            if(useShadow.floatValue == 1.0f)
            {
                Texture2D bakedShadowTex = AutoBakeShadowTexture(material, bakedMainTex, 1, false);
                liteMaterial.SetFloat("_ShadowBorder",              shadowBorder.floatValue);
                liteMaterial.SetFloat("_ShadowBlur",                shadowBlur.floatValue);
                liteMaterial.SetTexture("_ShadowColorTex",          bakedShadowTex);
                if(shadow2ndColor.colorValue.a != 0.0f)
                {
                    Texture2D bakedShadow2ndTex = AutoBakeShadowTexture(material, bakedMainTex, 2, false);
                    liteMaterial.SetFloat("_Shadow2ndBorder",           shadow2ndBorder.floatValue);
                    liteMaterial.SetFloat("_Shadow2ndBlur",             shadow2ndBlur.floatValue);
                    liteMaterial.SetTexture("_Shadow2ndColorTex",       bakedShadow2ndTex);
                }
                liteMaterial.SetFloat("_ShadowEnvStrength",         shadowEnvStrength.floatValue);
                liteMaterial.SetColor("_ShadowBorderColor",         shadowBorderColor.colorValue);
                liteMaterial.SetFloat("_ShadowBorderRange",         shadowBorderRange.floatValue);
            }

            if(isOutl)
            {
                Texture2D bakedOutlineTex = AutoBakeOutlineTexture(material);
                liteMaterial.SetColor("_OutlineColor",              outlineColor.colorValue);
                liteMaterial.SetTexture("_OutlineTex",              bakedOutlineTex);
                liteMaterial.SetVector("_OutlineTex_ScrollRotate",  outlineTex_ScrollRotate.vectorValue);
                liteMaterial.SetTexture("_OutlineWidthMask",        outlineWidthMask.textureValue);
                liteMaterial.SetFloat("_OutlineWidth",              outlineWidth.floatValue);
                liteMaterial.SetFloat("_OutlineFixWidth",           outlineFixWidth.floatValue);
                liteMaterial.SetFloat("_OutlineVertexR2Width",      outlineVertexR2Width.floatValue);
                liteMaterial.SetFloat("_OutlineDeleteMesh",         outlineDeleteMesh.floatValue);
                liteMaterial.SetFloat("_OutlineEnableLighting",     outlineEnableLighting.floatValue);
                liteMaterial.SetFloat("_OutlineZBias",              outlineZBias.floatValue);
                liteMaterial.SetFloat("_OutlineSrcBlend",           outlineSrcBlend.floatValue);
                liteMaterial.SetFloat("_OutlineDstBlend",           outlineDstBlend.floatValue);
                liteMaterial.SetFloat("_OutlineBlendOp",            outlineBlendOp.floatValue);
                liteMaterial.SetFloat("_OutlineSrcBlendFA",         outlineSrcBlendFA.floatValue);
                liteMaterial.SetFloat("_OutlineDstBlendFA",         outlineDstBlendFA.floatValue);
                liteMaterial.SetFloat("_OutlineBlendOpFA",          outlineBlendOpFA.floatValue);
                liteMaterial.SetFloat("_OutlineZWrite",             outlineZwrite.floatValue);
                liteMaterial.SetFloat("_OutlineZTest",              outlineZtest.floatValue);
                liteMaterial.SetFloat("_OutlineAlphaToMask",        outlineAlphaToMask.floatValue);
                liteMaterial.SetFloat("_OutlineStencilRef",         outlineStencilRef.floatValue);
                liteMaterial.SetFloat("_OutlineStencilReadMask",    outlineStencilReadMask.floatValue);
                liteMaterial.SetFloat("_OutlineStencilWriteMask",   outlineStencilWriteMask.floatValue);
                liteMaterial.SetFloat("_OutlineStencilComp",        outlineStencilComp.floatValue);
                liteMaterial.SetFloat("_OutlineStencilPass",        outlineStencilPass.floatValue);
                liteMaterial.SetFloat("_OutlineStencilFail",        outlineStencilFail.floatValue);
                liteMaterial.SetFloat("_OutlineStencilZFail",       outlineStencilZFail.floatValue);
            }

            Texture2D bakedMatCap = AutoBakeMatCap(liteMaterial);
            if(bakedMatCap != null)
            {
                liteMaterial.SetTexture("_MatCapTex",               bakedMatCap);
                liteMaterial.SetFloat("_UseMatCap",                 useMatCap.floatValue);
                liteMaterial.SetFloat("_MatCapBlendUV1",            matcapBlendUV1.floatValue);
                liteMaterial.SetFloat("_MatCapZRotCancel",          matcapZRotCancel.floatValue);
                liteMaterial.SetFloat("_MatCapPerspective",         matcapPerspective.floatValue);
                liteMaterial.SetFloat("_MatCapVRParallaxStrength",  matcapVRParallaxStrength.floatValue);
                if(matcapBlendMode.floatValue == 3) liteMaterial.SetFloat("_MatCapMul", useMatCap.floatValue);
                else                                liteMaterial.SetFloat("_MatCapMul", useMatCap.floatValue);
            }

            liteMaterial.SetFloat("_UseRim",                    useRim.floatValue);
            if(useRim.floatValue == 1.0f)
            {
                liteMaterial.SetColor("_RimColor",                  rimColor.colorValue);
                liteMaterial.SetFloat("_RimBorder",                 rimBorder.floatValue);
                liteMaterial.SetFloat("_RimBlur",                   rimBlur.floatValue);
                liteMaterial.SetFloat("_RimFresnelPower",           rimFresnelPower.floatValue);
                liteMaterial.SetFloat("_RimShadowMask",             rimShadowMask.floatValue);
            }

            if(useEmission.floatValue == 1.0f)
            {
                liteMaterial.SetFloat("_UseEmission",               useEmission.floatValue);
                liteMaterial.SetColor("_EmissionColor",             emissionColor.colorValue);
                liteMaterial.SetTexture("_EmissionMap",             emissionMap.textureValue);
                liteMaterial.SetFloat("_EmissionMap_UVMode",        emissionMap_UVMode.floatValue);
                liteMaterial.SetVector("_EmissionMap_ScrollRotate", emissionMap_ScrollRotate.vectorValue);
                liteMaterial.SetVector("_EmissionBlink",            emissionBlink.vectorValue);
            }

            Texture2D bakedTriMask = AutoBakeTriMask(liteMaterial);
            if(bakedTriMask != null) liteMaterial.SetTexture("_TriMask", bakedTriMask);

            liteMaterial.SetFloat("_SrcBlend",                  srcBlend.floatValue);
            liteMaterial.SetFloat("_DstBlend",                  dstBlend.floatValue);
            liteMaterial.SetFloat("_BlendOp",                   blendOp.floatValue);
            liteMaterial.SetFloat("_SrcBlendFA",                srcBlendFA.floatValue);
            liteMaterial.SetFloat("_DstBlendFA",                dstBlendFA.floatValue);
            liteMaterial.SetFloat("_BlendOpFA",                 blendOpFA.floatValue);
            liteMaterial.SetFloat("_ZClip",                     zclip.floatValue);
            liteMaterial.SetFloat("_ZWrite",                    zwrite.floatValue);
            liteMaterial.SetFloat("_ZTest",                     ztest.floatValue);
            liteMaterial.SetFloat("_AlphaToMask",               alphaToMask.floatValue);
            liteMaterial.SetFloat("_StencilRef",                stencilRef.floatValue);
            liteMaterial.SetFloat("_StencilReadMask",           stencilReadMask.floatValue);
            liteMaterial.SetFloat("_StencilWriteMask",          stencilWriteMask.floatValue);
            liteMaterial.SetFloat("_StencilComp",               stencilComp.floatValue);
            liteMaterial.SetFloat("_StencilPass",               stencilPass.floatValue);
            liteMaterial.SetFloat("_StencilFail",               stencilFail.floatValue);
            liteMaterial.SetFloat("_StencilZFail",              stencilZFail.floatValue);
            liteMaterial.renderQueue = lilMaterialUtils.GetTrueRenderQueue(material);
        }

        // TODO : Support other rendering mode
        private void CreateMultiMaterial(Material material)
        {
            material.shader = ltsm;
            if(isOutl)  material.shader = ltsmo;
            else        material.shader = ltsm;
            isMulti = true;

            if(renderingModeBuf == RenderingMode.Cutout)            material.SetFloat("_TransparentMode", 1.0f);
            else if(renderingModeBuf == RenderingMode.Transparent)  material.SetFloat("_TransparentMode", 2.0f);
            else                                                    material.SetFloat("_TransparentMode", 0.0f);
            material.SetFloat("_UseClippingCanceller", 0.0f);

            if(shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER) material.SetFloat("_UseClippingCanceller", 1.0f);

            SetupMaterialWithRenderingMode(material, renderingModeBuf, TransparentMode.Normal, isOutl, false, isStWr, false);
            lilMaterialUtils.SetupMultiMaterial(material);
        }

        protected virtual void ReplaceToCustomShaders()
        {
        }

        protected void ConvertMaterialToCustomShader(Material material)
        {
            lilShaderManager.InitializeShaders();
            Shader shader = material.shader;
                 if(shader == lts)           { ReplaceToCustomShaders(); shader = lts       ;}
            else if(shader == ltsc)          { ReplaceToCustomShaders(); shader = ltsc      ;}
            else if(shader == ltst)          { ReplaceToCustomShaders(); shader = ltst      ;}
            else if(shader == ltsot)         { ReplaceToCustomShaders(); shader = ltsot     ;}
            else if(shader == ltstt)         { ReplaceToCustomShaders(); shader = ltstt     ;}
            else if(shader == ltso)          { ReplaceToCustomShaders(); shader = ltso      ;}
            else if(shader == ltsco)         { ReplaceToCustomShaders(); shader = ltsco     ;}
            else if(shader == ltsto)         { ReplaceToCustomShaders(); shader = ltsto     ;}
            else if(shader == ltsoto)        { ReplaceToCustomShaders(); shader = ltsoto    ;}
            else if(shader == ltstto)        { ReplaceToCustomShaders(); shader = ltstto    ;}
            else if(shader == ltsoo)         { ReplaceToCustomShaders(); shader = ltsoo     ;}
            else if(shader == ltscoo)        { ReplaceToCustomShaders(); shader = ltscoo    ;}
            else if(shader == ltstoo)        { ReplaceToCustomShaders(); shader = ltstoo    ;}
            else if(shader == ltstess)       { ReplaceToCustomShaders(); shader = ltstess   ;}
            else if(shader == ltstessc)      { ReplaceToCustomShaders(); shader = ltstessc  ;}
            else if(shader == ltstesst)      { ReplaceToCustomShaders(); shader = ltstesst  ;}
            else if(shader == ltstessot)     { ReplaceToCustomShaders(); shader = ltstessot ;}
            else if(shader == ltstesstt)     { ReplaceToCustomShaders(); shader = ltstesstt ;}
            else if(shader == ltstesso)      { ReplaceToCustomShaders(); shader = ltstesso  ;}
            else if(shader == ltstessco)     { ReplaceToCustomShaders(); shader = ltstessco ;}
            else if(shader == ltstessto)     { ReplaceToCustomShaders(); shader = ltstessto ;}
            else if(shader == ltstessoto)    { ReplaceToCustomShaders(); shader = ltstessoto;}
            else if(shader == ltstesstto)    { ReplaceToCustomShaders(); shader = ltstesstto;}
            else if(shader == ltsl)          { ReplaceToCustomShaders(); shader = ltsl      ;}
            else if(shader == ltslc)         { ReplaceToCustomShaders(); shader = ltslc     ;}
            else if(shader == ltslt)         { ReplaceToCustomShaders(); shader = ltslt     ;}
            else if(shader == ltslot)        { ReplaceToCustomShaders(); shader = ltslot    ;}
            else if(shader == ltsltt)        { ReplaceToCustomShaders(); shader = ltsltt    ;}
            else if(shader == ltslo)         { ReplaceToCustomShaders(); shader = ltslo     ;}
            else if(shader == ltslco)        { ReplaceToCustomShaders(); shader = ltslco    ;}
            else if(shader == ltslto)        { ReplaceToCustomShaders(); shader = ltslto    ;}
            else if(shader == ltsloto)       { ReplaceToCustomShaders(); shader = ltsloto   ;}
            else if(shader == ltsltto)       { ReplaceToCustomShaders(); shader = ltsltto   ;}
            else if(shader == ltsref)        { ReplaceToCustomShaders(); shader = ltsref    ;}
            else if(shader == ltsrefb)       { ReplaceToCustomShaders(); shader = ltsrefb   ;}
            else if(shader == ltsfur)        { ReplaceToCustomShaders(); shader = ltsfur    ;}
            else if(shader == ltsfurc)       { ReplaceToCustomShaders(); shader = ltsfurc   ;}
            else if(shader == ltsfurtwo)     { ReplaceToCustomShaders(); shader = ltsfurtwo ;}
            else if(shader == ltsfuro)       { ReplaceToCustomShaders(); shader = ltsfuro   ;}
            else if(shader == ltsfuroc)      { ReplaceToCustomShaders(); shader = ltsfuroc  ;}
            else if(shader == ltsfurotwo)    { ReplaceToCustomShaders(); shader = ltsfurotwo;}
            else if(shader == ltsgem)        { ReplaceToCustomShaders(); shader = ltsgem    ;}
            else if(shader == ltsfs)         { ReplaceToCustomShaders(); shader = ltsfs     ;}
            else if(shader == ltsover)       { ReplaceToCustomShaders(); shader = ltsover   ;}
            else if(shader == ltsoover)      { ReplaceToCustomShaders(); shader = ltsoover  ;}
            else if(shader == ltslover)      { ReplaceToCustomShaders(); shader = ltslover  ;}
            else if(shader == ltsloover)     { ReplaceToCustomShaders(); shader = ltsloover ;}
            else if(shader == ltsm)          { ReplaceToCustomShaders(); shader = ltsm      ;}
            else if(shader == ltsmo)         { ReplaceToCustomShaders(); shader = ltsmo     ;}
            else if(shader == ltsmref)       { ReplaceToCustomShaders(); shader = ltsmref   ;}
            else if(shader == ltsmfur)       { ReplaceToCustomShaders(); shader = ltsmfur   ;}
            else if(shader == ltsmgem)       { ReplaceToCustomShaders(); shader = ltsmgem   ;}

            if(material.shader != shader && shader != null)
            {
                int renderQueue = lilMaterialUtils.GetTrueRenderQueue(material);
                material.shader = shader;
                material.renderQueue = renderQueue;
            }
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Property group drawer
        #region
        private void DrawRenderingModeSettings(Material material, string sTransparentMode, string[] sRenderingModeList, string[] sRenderingModeListLite)
        {
            if(isMultiVariants)
            {
                GUI.enabled = false;
                EditorGUI.showMixedValue = true;
                EditorGUILayout.Popup(GetLoc("sRenderingMode"), (int)renderingModeBuf, sRenderingModeList);
                EditorGUI.showMixedValue = false;
                GUI.enabled = true;
            }
            else
            {
                if(isShowRenderMode && !isMulti)
                {
                    RenderingMode renderingMode;
                    if(isLite)  renderingMode = (RenderingMode)EditorGUILayout.Popup(GetLoc("sRenderingMode"), (int)renderingModeBuf, sRenderingModeListLite);
                    else        renderingMode = (RenderingMode)EditorGUILayout.Popup(GetLoc("sRenderingMode"), (int)renderingModeBuf, sRenderingModeList);
                    if(renderingModeBuf != renderingMode)
                    {
                        SetupMaterialWithRenderingMode(material, renderingMode, transparentModeBuf);
                        if(renderingMode == RenderingMode.Cutout || renderingMode == RenderingMode.FurCutout) cutoff.floatValue = 0.5f;
                        if(renderingMode == RenderingMode.Transparent || renderingMode == RenderingMode.Fur || renderingMode == RenderingMode.FurTwoPass) cutoff.floatValue = 0.001f;
                        if(transparentModeBuf == TransparentMode.TwoPass)
                        {
                            preCutoff.floatValue = 0.001f;
                            cull.floatValue = 2.0f;
                            preCull.floatValue = 1.0f;
                        }
                    }
                }
                else if(isShowRenderMode && isMulti)
                {
                    float transparentModeMatBuf = transparentModeMat.floatValue;
                    m_MaterialEditor.ShaderProperty(transparentModeMat, sTransparentMode);
                    if(transparentModeMatBuf != transparentModeMat.floatValue)
                    {
                        SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf);
                        if(transparentModeMat.floatValue == 1.0f || transparentModeMat.floatValue == 5.0f) cutoff.floatValue = 0.5f;
                        if(transparentModeMat.floatValue == 2.0f || transparentModeMat.floatValue == 4.0f) cutoff.floatValue = 0.001f;
                    }
                }
                if(renderingModeBuf == RenderingMode.Transparent)
                {
                    TransparentMode transparentMode = (TransparentMode)EditorGUILayout.Popup(GetLoc("sTransparentMode"), (int)transparentModeBuf, sTransparentModeList);
                    if(transparentModeBuf != transparentMode)
                    {
                        SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentMode);
                    }
                    if(transparentModeBuf == TransparentMode.OnePass && vertexLightStrength.floatValue != 1.0f && lilRenderPipelineReader.GetRP() == lilRenderPipeline.BRP && lilEditorGUI.AutoFixHelpBox(GetLoc("sHelpOnePassVertexLight")))
                    {
                        vertexLightStrength.floatValue = 1.0f;
                    }
                }
                if(lilDirectoryManager.ExistsClusterCreatorKit())
                {
                    if(renderingModeBuf == RenderingMode.Refraction || renderingModeBuf == RenderingMode.RefractionBlur || renderingModeBuf == RenderingMode.Gem)
                    {
                        EditorGUILayout.HelpBox(GetLoc("sHelpGrabPass"), MessageType.Warning);
                    }
                    if(renderingModeBuf == RenderingMode.Fur || renderingModeBuf == RenderingMode.FurCutout || renderingModeBuf == RenderingMode.FurTwoPass)
                    {
                        EditorGUILayout.HelpBox(GetLoc("sHelpGeometryShader"), MessageType.Warning);
                    }
                }
            }
        }

        private void DrawBaseSettings(Material material, string sCullModes, string sTransparentMode, string[] sRenderingModeList, string[] sRenderingModeListLite, string[] sTransparentModeList)
        {
            DrawRenderingModeSettings(material, sTransparentMode, sRenderingModeList, sRenderingModeListLite);

            EditorGUILayout.Space();
            GUILayout.Label(GetLoc("sBaseSetting"), boldLabel);

            edSet.isShowBase = lilEditorGUI.Foldout(GetLoc("sBaseSetting"), edSet.isShowBase);
            DrawMenuButton(GetLoc("sAnchorBaseSetting"), PropertyBlock.Base);
            if(edSet.isShowBase)
            {
                EditorGUILayout.BeginVertical(customBox);
                    if(isMulti)
                    {
                        m_MaterialEditor.ShaderProperty(asOverlay, GetLoc("sAsOverlay"));
                    }
                    if(isUseAlpha)
                    {
                        m_MaterialEditor.ShaderProperty(cutoff, GetLoc("sCutoff"));
                    }
                    if(!isGem)
                    {
                        m_MaterialEditor.ShaderProperty(cull, sCullModes);
                        EditorGUI.indentLevel++;
                        if(cull.floatValue == 1.0f && lilEditorGUI.AutoFixHelpBox(GetLoc("sHelpCullMode")))
                        {
                            cull.floatValue = 2.0f;
                        }
                        if(cull.floatValue <= 1.0f || transparentModeBuf == TransparentMode.TwoPass && preCull.floatValue <= 1.0f)
                        {
                            m_MaterialEditor.ShaderProperty(flipNormal, GetLoc("sFlipBackfaceNormal"));
                            m_MaterialEditor.ShaderProperty(backfaceForceShadow, GetLoc("sBackfaceForceShadow"));
                            m_MaterialEditor.ShaderProperty(backfaceColor, GetLoc("sColor"));
                            lilEditorGUI.DrawColorAsAlpha(backfaceColor);
                        }
                        EditorGUI.indentLevel--;
                    }
                    m_MaterialEditor.ShaderProperty(invisible, GetLoc("sInvisible"));
                    m_MaterialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                    if(zwrite.floatValue != 1.0f && !isGem && lilEditorGUI.AutoFixHelpBox(GetLoc("sHelpZWrite")))
                    {
                        zwrite.floatValue = 1.0f;
                    }
                    if(isMulti) m_MaterialEditor.ShaderProperty(useClippingCanceller, GetLoc("sSettingClippingCanceller"));
                    m_MaterialEditor.RenderQueueField();
                    if((renderingModeBuf >= RenderingMode.Transparent && renderingModeBuf != RenderingMode.FurCutout) || (isMulti && transparentModeMat.floatValue == 2.0f))
                    {
                        #if VRC_SDK_VRCSDK3 && UDON
                            if(material.renderQueue <= 2999 && zwrite.floatValue == 1.0f)
                            {
                                EditorGUILayout.HelpBox(GetLoc("sHelpTransparentForWorld"),MessageType.Warning);
                            }
                        #else
                            EditorGUILayout.HelpBox(GetLoc("sHelpRenderingTransparent"),MessageType.Warning);
                        #endif
                    }
                    if(isLite)
                    {
                        lilEditorGUI.DrawLine();
                        m_MaterialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sTriMask"), GetLoc("sTriMaskRGB")), triMask);
                    }
                EditorGUILayout.EndVertical();

                if(transparentModeBuf == TransparentMode.TwoPass)
                {
                    EditorGUILayout.LabelField("PrePass");
                    EditorGUILayout.BeginVertical(customBox);
                    m_MaterialEditor.ShaderProperty(preOutType, BuildParams(GetLoc("sOutType"), GetLoc("sOutTypeNormal"), GetLoc("sOutTypeFlat"), GetLoc("sOutTypeMono")));

                    int preBlendMode = -1;
                    if(preSrcBlend.floatValue == 1.0f && preDstBlend.floatValue == 10.0f) preBlendMode = 0; // Normal
                    if(preSrcBlend.floatValue == 1.0f && preDstBlend.floatValue == 1.0f)  preBlendMode = 1; // Add
                    if(preSrcBlend.floatValue == 1.0f && preDstBlend.floatValue == 6.0f)  preBlendMode = 2; // Screen
                    if(preSrcBlend.floatValue == 0.0f && preDstBlend.floatValue == 3.0f)  preBlendMode = 3; // Mul
                    EditorGUI.BeginChangeCheck();
                    preBlendMode = EditorGUILayout.Popup(GetLoc("sBlendMode"), preBlendMode, sBlendModeList);
                    if(EditorGUI.EndChangeCheck())
                    {
                        switch(preBlendMode)
                        {
                            case 0:
                                preSrcBlend.floatValue = 1.0f;
                                preDstBlend.floatValue = 10.0f;
                                break;
                            case 1:
                                preSrcBlend.floatValue = 1.0f;
                                preDstBlend.floatValue = 1.0f;
                                break;
                            case 2:
                                preSrcBlend.floatValue = 1.0f;
                                preDstBlend.floatValue = 6.0f;
                                break;
                            case 3:
                                preSrcBlend.floatValue = 0.0f;
                                preDstBlend.floatValue = 3.0f;
                                break;
                            default:
                                break;
                        }
                    }

                    m_MaterialEditor.ShaderProperty(preCull, sCullModes);
                    m_MaterialEditor.ShaderProperty(preZwrite, GetLoc("sZWrite"));
                    m_MaterialEditor.ShaderProperty(preColor, GetLoc("sColor"));
                    lilEditorGUI.DrawColorAsAlpha(preColor);
                    m_MaterialEditor.ShaderProperty(preCutoff, GetLoc("sCutoff"));

                    edSet.isShowPrePreset = lilEditorGUI.DrawSimpleFoldout(GetLoc("sPresets"), edSet.isShowPrePreset, isCustomEditor);
                    if(edSet.isShowPrePreset)
                    {
                        EditorGUI.indentLevel++;
                        if(lilEditorGUI.EditorButton(GetLoc("sTransparentPresetsPreWriteDepth")))
                        {
                            preColor.colorValue = Color.white;
                            preOutType.floatValue = 2.0f;
                            preCutoff.floatValue = -0.001f;
                            preSrcBlend.floatValue = 0.0f;
                            preDstBlend.floatValue = 3.0f;
                            preZwrite.floatValue = 1.0f;
                            preCull.floatValue = cull.floatValue;
                            preStencilRef.floatValue = stencilRef.floatValue;
                            preStencilComp.floatValue = stencilComp.floatValue;
                            mainColor.colorValue = new Color(mainColor.colorValue.r, mainColor.colorValue.g, mainColor.colorValue.b, 1.0f);
                            ztest.floatValue = (float)UnityEngine.Rendering.CompareFunction.LessEqual;
                        }
                        if(lilEditorGUI.EditorButton(GetLoc("sTransparentPresetsColorTransparent")))
                        {
                            preColor.colorValue = new Color(0.75f,0.0f,0.0f,1.0f);
                            preOutType.floatValue = 1.0f;
                            preCutoff.floatValue = -0.001f;
                            preSrcBlend.floatValue = 0.0f;
                            preDstBlend.floatValue = 3.0f;
                            preZwrite.floatValue = 0.0f;
                            preCull.floatValue = cull.floatValue;
                            preStencilRef.floatValue = stencilRef.floatValue;
                            preStencilComp.floatValue = stencilComp.floatValue;
                            mainColor.colorValue = new Color(mainColor.colorValue.r, mainColor.colorValue.g, mainColor.colorValue.b, 0.0f);
                            cutoff.floatValue = -0.001f;
                            ztest.floatValue = (float)UnityEngine.Rendering.CompareFunction.LessEqual;
                        }
                        if(lilEditorGUI.EditorButton(GetLoc("sTransparentPresetsBackAndFront")))
                        {
                            preColor.colorValue = Color.white;
                            preOutType.floatValue = 0.0f;
                            preCutoff.floatValue = cutoff.floatValue;
                            preSrcBlend.floatValue = 1.0f;
                            preDstBlend.floatValue = 10.0f;
                            preZwrite.floatValue = 1.0f;
                            preCull.floatValue = 1.0f;
                            preStencilRef.floatValue = stencilRef.floatValue;
                            preStencilComp.floatValue = stencilComp.floatValue;
                            mainColor.colorValue = new Color(mainColor.colorValue.r, mainColor.colorValue.g, mainColor.colorValue.b, 1.0f);
                            cull.floatValue = 0.0f;
                            ztest.floatValue = (float)UnityEngine.Rendering.CompareFunction.Less;
                        }
                        if(lilEditorGUI.EditorButton(GetLoc("sTransparentPresetsCutoutAndTransparent")))
                        {
                            preColor.colorValue = Color.white;
                            preOutType.floatValue = 0.0f;
                            preCutoff.floatValue = 0.95f;
                            preSrcBlend.floatValue = 1.0f;
                            preDstBlend.floatValue = 10.0f;
                            preZwrite.floatValue = 1.0f;
                            preCull.floatValue = cull.floatValue;
                            preStencilRef.floatValue = stencilRef.floatValue;
                            preStencilComp.floatValue = stencilComp.floatValue;
                            mainColor.colorValue = new Color(mainColor.colorValue.r, mainColor.colorValue.g, mainColor.colorValue.b, 1.0f);
                            ztest.floatValue = (float)UnityEngine.Rendering.CompareFunction.LessEqual;
                        }
                        if(lilEditorGUI.EditorButton(GetLoc("sTransparentPresetsFadeStencil")))
                        {
                            preColor.colorValue = new Color(1.0f,1.0f,1.0f,0.5f);
                            preOutType.floatValue = 0.0f;
                            preCutoff.floatValue = cutoff.floatValue;
                            preSrcBlend.floatValue = 1.0f;
                            preDstBlend.floatValue = 10.0f;
                            preZwrite.floatValue = 1.0f;
                            preCull.floatValue = cull.floatValue;
                            preStencilRef.floatValue = stencilRef.floatValue;
                            preStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Equal;
                            mainColor.colorValue = new Color(mainColor.colorValue.r, mainColor.colorValue.g, mainColor.colorValue.b, 1.0f);
                            ztest.floatValue = (float)UnityEngine.Rendering.CompareFunction.Less;
                        }
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.LabelField(GetLoc("sSimpleStencilSettings"));
                EditorGUILayout.BeginVertical(customBox);
                    int stencilMode = -1;
                    if(stencilComp.floatValue == (float)UnityEngine.Rendering.CompareFunction.Always    && stencilPass.floatValue == (float)UnityEngine.Rendering.StencilOp.Keep)       stencilMode = 0; // Normal
                    if(stencilComp.floatValue == (float)UnityEngine.Rendering.CompareFunction.Always    && stencilPass.floatValue == (float)UnityEngine.Rendering.StencilOp.Replace)    stencilMode = 1; // Writer
                    if(stencilComp.floatValue == (float)UnityEngine.Rendering.CompareFunction.NotEqual  && stencilPass.floatValue == (float)UnityEngine.Rendering.StencilOp.Keep)       stencilMode = 2; // Reader
                    if(stencilComp.floatValue == (float)UnityEngine.Rendering.CompareFunction.Equal     && stencilPass.floatValue == (float)UnityEngine.Rendering.StencilOp.Keep)       stencilMode = 3; // Reader (Invert)
                    if(transparentModeBuf == TransparentMode.TwoPass &&
                    stencilComp.floatValue == (float)UnityEngine.Rendering.CompareFunction.Always    && stencilPass.floatValue == (float)UnityEngine.Rendering.StencilOp.Keep &&
                    preStencilComp.floatValue == (float)UnityEngine.Rendering.CompareFunction.Equal && preStencilPass.floatValue == (float)UnityEngine.Rendering.StencilOp.Keep
                    ) stencilMode = 4; // Reader (Fade)

                    int outlineStencilMode = -1;

                    EditorGUI.BeginChangeCheck();
                    if(transparentModeBuf == TransparentMode.TwoPass)   stencilMode = EditorGUILayout.Popup("Mode", stencilMode, new[]{GetLoc("sStencilModeNormal"),GetLoc("sStencilModeWriter"),GetLoc("sStencilModeReader"),GetLoc("sStencilModeReaderInvert"),GetLoc("sStencilModeReaderFade")});
                    else                                                stencilMode = EditorGUILayout.Popup("Mode", stencilMode, new[]{GetLoc("sStencilModeNormal"),GetLoc("sStencilModeWriter"),GetLoc("sStencilModeReader"),GetLoc("sStencilModeReaderInvert")});
                    if(isOutl)
                    {
                        if(outlineStencilComp.floatValue == (float)UnityEngine.Rendering.CompareFunction.Always     && outlineStencilPass.floatValue == (float)UnityEngine.Rendering.StencilOp.Keep)    outlineStencilMode = 0; // Normal
                        if(outlineStencilComp.floatValue == (float)UnityEngine.Rendering.CompareFunction.Always     && outlineStencilPass.floatValue == (float)UnityEngine.Rendering.StencilOp.Replace) outlineStencilMode = 1; // Writer
                        if(outlineStencilComp.floatValue == (float)UnityEngine.Rendering.CompareFunction.NotEqual   && outlineStencilPass.floatValue == (float)UnityEngine.Rendering.StencilOp.Keep)    outlineStencilMode = 2; // Reader
                        if(outlineStencilComp.floatValue == (float)UnityEngine.Rendering.CompareFunction.Equal      && outlineStencilPass.floatValue == (float)UnityEngine.Rendering.StencilOp.Keep)    outlineStencilMode = 3; // Reader (Invert)
                        outlineStencilMode = EditorGUILayout.Popup("Mode (" + GetLoc("sOutline") + ")", outlineStencilMode, new[]{GetLoc("sStencilModeNormal"),GetLoc("sStencilModeWriter"),GetLoc("sStencilModeReader"),GetLoc("sStencilModeReaderInvert")});
                    }
                    if(EditorGUI.EndChangeCheck())
                    {
                        SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf);
                        int shaderRenderQueue = isMulti ? material.renderQueue : material.shader.renderQueue;
                        switch(stencilMode)
                        {
                            case 0:
                                stencilRef.floatValue = 0;
                                stencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                                stencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                                if(!isMulti) material.renderQueue = -1;
                                break;
                            case 1: // Writer
                                stencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                                stencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Replace;
                                material.renderQueue = shaderRenderQueue > 2451 ? -1 : 2451;
                                break;
                            case 2: // Reader
                                stencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.NotEqual;
                                stencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                                material.renderQueue = shaderRenderQueue > 2452 ? -1 : 2452;
                                break;
                            case 3: // Reader (Invert)
                                stencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Equal;
                                stencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                                material.renderQueue = shaderRenderQueue > 2452 ? -1 : 2452;
                                break;
                            case 4: // Reader (Fade)
                                stencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                                stencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                                material.renderQueue = shaderRenderQueue > 2452 ? -1 : 2452;
                                break;
                            default:
                                break;
                        }
                        if(stencilMode != 0 && stencilRef.floatValue == 0) stencilRef.floatValue = 1;
                        stencilReadMask.floatValue = 255.0f;
                        stencilWriteMask.floatValue = 255.0f;
                        stencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        stencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        if(isOutl)
                        {
                            switch(outlineStencilMode)
                            {
                                case 0:
                                    outlineStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                                    outlineStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                                    break;
                                case 1: // Writer
                                    outlineStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                                    outlineStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Replace;
                                    break;
                                case 2: // Reader
                                    outlineStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.NotEqual;
                                    outlineStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                                    break;
                                case 3: // Reader (Invert)
                                    outlineStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Equal;
                                    outlineStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                                    break;
                                default:
                                    break;
                            }
                            outlineStencilRef.floatValue = stencilRef.floatValue;
                            outlineStencilReadMask.floatValue = 255.0f;
                            outlineStencilWriteMask.floatValue = 255.0f;
                            outlineStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                            outlineStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        }
                        if(isFur)
                        {
                            furStencilRef.floatValue = stencilRef.floatValue;
                            furStencilComp.floatValue = stencilComp.floatValue;
                            furStencilPass.floatValue = stencilPass.floatValue;
                            furStencilReadMask.floatValue = stencilReadMask.floatValue;
                            furStencilWriteMask.floatValue = stencilWriteMask.floatValue;
                            furStencilFail.floatValue = stencilFail.floatValue;
                            furStencilZFail.floatValue = stencilZFail.floatValue;
                        }
                        if(transparentModeBuf == TransparentMode.TwoPass)
                        {
                            ztest.floatValue = stencilMode == 4 ? (float)UnityEngine.Rendering.CompareFunction.Less : (float)UnityEngine.Rendering.CompareFunction.LessEqual;
                            preStencilRef.floatValue = stencilRef.floatValue;
                            preStencilComp.floatValue = stencilMode == 4 ? (float)UnityEngine.Rendering.CompareFunction.Equal : stencilComp.floatValue;
                            preStencilPass.floatValue = stencilPass.floatValue;
                            preStencilReadMask.floatValue = stencilReadMask.floatValue;
                            preStencilWriteMask.floatValue = stencilWriteMask.floatValue;
                            preStencilFail.floatValue = stencilFail.floatValue;
                            preStencilZFail.floatValue = stencilZFail.floatValue;
                        }
                    }
                    if(stencilMode != 0 || isOutl && outlineStencilMode != 0)
                    {
                        EditorGUI.BeginChangeCheck();
                        m_MaterialEditor.ShaderProperty(stencilRef, "Ref");
                        if(EditorGUI.EndChangeCheck())
                        {
                            if(isOutl) outlineStencilRef.floatValue = stencilRef.floatValue;
                            if(isFur) furStencilRef.floatValue = stencilRef.floatValue;
                            if(transparentModeBuf == TransparentMode.TwoPass) preStencilRef.floatValue = stencilRef.floatValue;
                        }
                    }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawLightingSettings()
        {
            edSet.isShowLightingSettings = lilEditorGUI.Foldout(GetLoc("sLightingSettings"), edSet.isShowLightingSettings);
            DrawMenuButton(GetLoc("sAnchorLighting"), PropertyBlock.Lighting);
            if(edSet.isShowLightingSettings)
            {
                EditorGUILayout.LabelField(GetLoc("sBaseSetting"));
                EditorGUILayout.BeginVertical(customBox);
                    m_MaterialEditor.ShaderProperty(lightMinLimit, GetLoc("sLightMinLimit"));
                    m_MaterialEditor.ShaderProperty(lightMaxLimit, GetLoc("sLightMaxLimit"));
                    m_MaterialEditor.ShaderProperty(monochromeLighting, GetLoc("sMonochromeLighting"));
                    if(shadowEnvStrength != null) m_MaterialEditor.ShaderProperty(shadowEnvStrength, GetLoc("sShadowEnvStrength"));
                    GUILayout.BeginHorizontal();
                    Rect position2 = EditorGUILayout.GetControlRect();
                    Rect labelRect = new Rect(position2.x, position2.y, EditorGUIUtility.labelWidth, position2.height);
                    Rect buttonRect1 = new Rect(labelRect.x + labelRect.width, position2.y, (position2.width - EditorGUIUtility.labelWidth)*0.5f, position2.height);
                    Rect buttonRect2 = new Rect(buttonRect1.x + buttonRect1.width, position2.y, buttonRect1.width, position2.height);
                    EditorGUI.PrefixLabel(labelRect, new GUIContent(GetLoc("sLightingPreset")));
                    if(GUI.Button(buttonRect1, new GUIContent(GetLoc("sLightingPresetDefault")))) ApplyLightingPreset(LightingPreset.Default);
                    if(GUI.Button(buttonRect2, new GUIContent(GetLoc("sLightingPresetSemiMonochrome")))) ApplyLightingPreset(LightingPreset.SemiMonochrome);
                    GUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                EditorGUILayout.LabelField(GetLoc("sAdvanced"));
                EditorGUILayout.BeginVertical(customBox);
                    m_MaterialEditor.ShaderProperty(asUnlit, GetLoc("sAsUnlit"));
                    if(asUnlit.floatValue != 0 && lilEditorGUI.AutoFixHelpBox(GetLoc("sAsUnlitWarn")))
                    {
                        asUnlit.floatValue = 0.0f;
                    }
                    m_MaterialEditor.ShaderProperty(vertexLightStrength, GetLoc("sVertexLightStrength"));
                    m_MaterialEditor.ShaderProperty(lightDirectionOverride, BuildParams(GetLoc("sLightDirectionOverride"), GetLoc("sObjectFollowing")));
                    if(isTransparent || (isFur && !isCutout)) m_MaterialEditor.ShaderProperty(alphaBoostFA, GetLoc("sAlphaBoostFA"));
                    BlendOpFASetting();
                    m_MaterialEditor.ShaderProperty(beforeExposureLimit, GetLoc("sBeforeExposureLimit"));
                    m_MaterialEditor.ShaderProperty(lilDirectionalLightStrength, GetLoc("sDirectionalLightStrength"));
                EditorGUILayout.EndVertical();
            }
        }

        private void BlendOpFASetting()
        {
            if(blendOpFA == null) return;
            int selecting = blendOpFA.floatValue == 0 ? 0 : (blendOpFA.floatValue == 4 ? 1 : 2);
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = blendOpFA.hasMixedValue;
            selecting = EditorGUILayout.Popup(GetLoc("sLightBlending"), selecting, new string[]{GetLoc("sBlendingAdd"), GetLoc("sBlendingMax")});
            EditorGUI.showMixedValue = false;

            if(EditorGUI.EndChangeCheck())
            {
                blendOpFA.floatValue = selecting == 0 ? 0 : (selecting == 1 ? 4 : blendOpFA.floatValue);
            }
        }

        private void DrawLightingSettingsSimple()
        {
            edSet.isShowLightingSettings = lilEditorGUI.Foldout(GetLoc("sLightingSettings"), edSet.isShowLightingSettings);
            DrawMenuButton(GetLoc("sAnchorLighting"), PropertyBlock.Lighting);
            if(edSet.isShowLightingSettings)
            {
                EditorGUILayout.LabelField(GetLoc("sBaseSetting"));
                EditorGUILayout.BeginVertical(customBox);
                    m_MaterialEditor.ShaderProperty(lightMinLimit, GetLoc("sLightMinLimit"));
                    m_MaterialEditor.ShaderProperty(lightMaxLimit, GetLoc("sLightMaxLimit"));
                    m_MaterialEditor.ShaderProperty(monochromeLighting, GetLoc("sMonochromeLighting"));
                    if(shadowEnvStrength != null) m_MaterialEditor.ShaderProperty(shadowEnvStrength, GetLoc("sShadowEnvStrength"));
                    GUILayout.BeginHorizontal();
                    Rect position2 = EditorGUILayout.GetControlRect();
                    Rect labelRect = new Rect(position2.x, position2.y, EditorGUIUtility.labelWidth, position2.height);
                    Rect buttonRect1 = new Rect(labelRect.x + labelRect.width, position2.y, (position2.width - EditorGUIUtility.labelWidth)*0.5f, position2.height);
                    Rect buttonRect2 = new Rect(buttonRect1.x + buttonRect1.width, position2.y, buttonRect1.width, position2.height);
                    EditorGUI.PrefixLabel(labelRect, new GUIContent(GetLoc("sLightingPreset")));
                    if(GUI.Button(buttonRect1, new GUIContent(GetLoc("sLightingPresetDefault")))) ApplyLightingPreset(LightingPreset.Default);
                    if(GUI.Button(buttonRect2, new GUIContent(GetLoc("sLightingPresetSemiMonochrome")))) ApplyLightingPreset(LightingPreset.SemiMonochrome);
                    GUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawMainAdjustSettings(Material material)
        {
            edSet.isShowMainTone = lilEditorGUI.DrawSimpleFoldout(GetLoc("sColorAdjust"), edSet.isShowMainTone, isCustomEditor);
            if(edSet.isShowMainTone)
            {
                m_MaterialEditor.TexturePropertySingleLine(maskBlendContent, mainColorAdjustMask);
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("HSV / Gamma", boldLabel);
                ToneCorrectionGUI(mainTexHSVG, 1);
                //EditorGUILayout.LabelField(GetLoc("sGradationMap"), boldLabel);
                //m_MaterialEditor.ShaderProperty(mainGradationStrength, GetLoc("sStrength"));
                lilEditorGUI.DrawLine();
                m_MaterialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sGradationMap")), mainGradationTex, mainGradationStrength);
                if(mainGradationStrength.floatValue != 0)
                {
                    //m_MaterialEditor.TexturePropertySingleLine(gradationContent, mainGradationTex);
                    EditorGUI.indentLevel++;
                    lilTextureUtils.GradientEditor(material, mainGrad, mainGradationTex, true);
                    EditorGUI.indentLevel--;
                }
                lilEditorGUI.DrawLine();
                TextureBakeGUI(material, 4);
                EditorGUI.indentLevel--;
            }
        }

        private void DrawAlphaMaskSettings(Material material)
        {
            if((renderingModeBuf == RenderingMode.Opaque && !isMulti) || (isMulti && transparentModeMat.floatValue == 0.0f))
            {
                GUILayout.Label(GetLoc("sAlphaMaskWarnOpaque"), wrapLabel);
            }
            else
            {
                EditorGUILayout.BeginVertical(boxOuter);
                m_MaterialEditor.ShaderProperty(alphaMaskMode, sAlphaMaskModes);
                DrawMenuButton(GetLoc("sAnchorAlphaMask"), PropertyBlock.AlphaMask);
                if(alphaMaskMode.floatValue != 0)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    m_MaterialEditor.TexturePropertySingleLine(alphaMaskContent, alphaMask);
                    m_MaterialEditor.TextureScaleOffsetProperty(alphaMask);

                    bool invertAlphaMask = alphaMaskScale.floatValue < 0;
                    float transparency = alphaMaskValue.floatValue - (invertAlphaMask ? 1.0f : 0.0f);

                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = alphaMaskScale.hasMixedValue || alphaMaskValue.hasMixedValue;
                    invertAlphaMask = EditorGUILayout.Toggle("Invert", invertAlphaMask);
                    transparency = EditorGUILayout.Slider("Transparency", transparency, -1.0f, 1.0f);
                    EditorGUI.showMixedValue = false;

                    if(EditorGUI.EndChangeCheck())
                    {
                        alphaMaskScale.floatValue = invertAlphaMask ? -1.0f : 1.0f;
                        alphaMaskValue.floatValue = transparency + (invertAlphaMask ? 1.0f : 0.0f);
                    }
                    m_MaterialEditor.ShaderProperty(cutoff, GetLoc("sCutoff"));

                    edSet.isAlphaMaskModeAdvanced = EditorGUILayout.Toggle("Show advanced editor", edSet.isAlphaMaskModeAdvanced);
                    if(edSet.isAlphaMaskModeAdvanced)
                    {
                        EditorGUI.indentLevel++;
                        m_MaterialEditor.ShaderProperty(alphaMaskScale, "Scale");
                        m_MaterialEditor.ShaderProperty(alphaMaskValue, "Offset");
                        EditorGUI.indentLevel--;
                    }
                    AlphamaskToTextureGUI(material);
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawShadowSettings()
        {
            edSet.isShowShadow = lilEditorGUI.Foldout(GetLoc("sShadowSetting"), edSet.isShowShadow);
            DrawMenuButton(GetLoc("sAnchorShadow"), PropertyBlock.Shadow);
            if(edSet.isShowShadow)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                m_MaterialEditor.ShaderProperty(useShadow, GetLoc("sShadow"));
                DrawMenuButton(GetLoc("sAnchorShadow"), PropertyBlock.Shadow);
                if(useShadow.floatValue == 1 && !isLite)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    m_MaterialEditor.ShaderProperty(shadowMaskType, sShadowMaskTypes);
                    if(shadowMaskType.floatValue == 1.0f)
                    {
                        m_MaterialEditor.TexturePropertySingleLine(maskBlendContent, shadowStrengthMask);
                        EditorGUI.indentLevel += 2;
                            m_MaterialEditor.ShaderProperty(shadowStrengthMaskLOD, "LOD");
                            m_MaterialEditor.ShaderProperty(shadowFlatBorder, GetLoc("sBorder"));
                            m_MaterialEditor.ShaderProperty(shadowFlatBlur, GetLoc("sBlur"));
                        EditorGUI.indentLevel -= 2;
                        m_MaterialEditor.ShaderProperty(shadowStrength, GetLoc("sStrength"));
                    }
                    else
                    {
                        m_MaterialEditor.TexturePropertySingleLine(maskStrengthContent, shadowStrengthMask, shadowStrength);
                        m_MaterialEditor.ShaderProperty(shadowStrengthMaskLOD, "LOD", 2);
                    }
                    lilEditorGUI.DrawLine();
                    m_MaterialEditor.ShaderProperty(shadowColorType, sShadowColorTypes);
                    m_MaterialEditor.TexturePropertySingleLine(shadow1stColorRGBAContent, shadowColorTex, shadowColor);
                    EditorGUI.indentLevel += 2;
                        m_MaterialEditor.ShaderProperty(shadowBorder, GetLoc("sBorder"));
                        m_MaterialEditor.ShaderProperty(shadowBlur, GetLoc("sBlur"));
                        m_MaterialEditor.ShaderProperty(shadowNormalStrength, GetLoc("sNormalStrength"));
                        m_MaterialEditor.ShaderProperty(shadowReceive, GetLoc("sReceiveShadow"));
                    EditorGUI.indentLevel -= 2;
                    lilEditorGUI.DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(shadow2ndColorRGBAContent, shadow2ndColorTex, shadow2ndColor);
                    EditorGUI.indentLevel += 2;
                        lilEditorGUI.DrawColorAsAlpha(shadow2ndColor);
                        if(shadow2ndColor.colorValue.a > 0)
                        {
                            m_MaterialEditor.ShaderProperty(shadow2ndBorder, GetLoc("sBorder"));
                            m_MaterialEditor.ShaderProperty(shadow2ndBlur, GetLoc("sBlur"));
                            m_MaterialEditor.ShaderProperty(shadow2ndNormalStrength, GetLoc("sNormalStrength"));
                            m_MaterialEditor.ShaderProperty(shadow2ndReceive, GetLoc("sReceiveShadow"));
                        }
                    EditorGUI.indentLevel -= 2;
                    lilEditorGUI.DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(shadow3rdColorRGBAContent, shadow3rdColorTex, shadow3rdColor);
                    EditorGUI.indentLevel += 2;
                        lilEditorGUI.DrawColorAsAlpha(shadow3rdColor);
                        if(shadow3rdColor.colorValue.a > 0)
                        {
                            m_MaterialEditor.ShaderProperty(shadow3rdBorder, GetLoc("sBorder"));
                            m_MaterialEditor.ShaderProperty(shadow3rdBlur, GetLoc("sBlur"));
                            m_MaterialEditor.ShaderProperty(shadow3rdNormalStrength, GetLoc("sNormalStrength"));
                            m_MaterialEditor.ShaderProperty(shadow3rdReceive, GetLoc("sReceiveShadow"));
                        }
                    EditorGUI.indentLevel -= 2;
                    lilEditorGUI.DrawLine();
                    m_MaterialEditor.ShaderProperty(shadowBorderColor, GetLoc("sShadowBorderColor"));
                    m_MaterialEditor.ShaderProperty(shadowBorderRange, GetLoc("sShadowBorderRange"));
                    lilEditorGUI.DrawLine();
                    m_MaterialEditor.ShaderProperty(shadowMainStrength, GetLoc("sContrast"));
                    m_MaterialEditor.ShaderProperty(shadowEnvStrength, GetLoc("sShadowEnvStrength"));
                    m_MaterialEditor.ShaderProperty(lilShadowCasterBias, "Shadow Caster Bias");
                    lilEditorGUI.DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sBlurMask"), GetLoc("sBlurRGB")), shadowBlurMask);
                    m_MaterialEditor.ShaderProperty(shadowBlurMaskLOD, "LOD", 2);
                    lilEditorGUI.DrawLine();
                    edSet.isShowShadowAO = lilEditorGUI.DrawSimpleFoldout(m_MaterialEditor, new GUIContent("AO Map", GetLoc("sBorderRGB")), shadowBorderMask, edSet.isShowShadowAO, isCustomEditor);
                    if(edSet.isShowShadowAO)
                    {
                        EditorGUI.indentLevel += 1;
                        m_MaterialEditor.ShaderProperty(shadowBorderMaskLOD, "LOD");
                        m_MaterialEditor.ShaderProperty(shadowPostAO, GetLoc("sIgnoreBorderProperties"));
                        float min1 = lilEditorGUI.GetRemapMinValue(shadowAOShift.vectorValue.x, shadowAOShift.vectorValue.y);
                        float max1 = lilEditorGUI.GetRemapMaxValue(shadowAOShift.vectorValue.x, shadowAOShift.vectorValue.y);
                        float min2 = lilEditorGUI.GetRemapMinValue(shadowAOShift.vectorValue.z, shadowAOShift.vectorValue.w);
                        float max2 = lilEditorGUI.GetRemapMaxValue(shadowAOShift.vectorValue.z, shadowAOShift.vectorValue.w);
                        float min3 = lilEditorGUI.GetRemapMinValue(shadowAOShift2.vectorValue.x, shadowAOShift2.vectorValue.y);
                        float max3 = lilEditorGUI.GetRemapMaxValue(shadowAOShift2.vectorValue.x, shadowAOShift2.vectorValue.y);
                        EditorGUI.BeginChangeCheck();
                        EditorGUI.showMixedValue = alphaMaskScale.hasMixedValue || alphaMaskValue.hasMixedValue;
                        min1 = EditorGUILayout.Slider("1st Min", min1, -0.01f, 1.01f);
                        max1 = EditorGUILayout.Slider("1st Max", max1, -0.01f, 1.01f);
                        min2 = EditorGUILayout.Slider("2nd Min", min2, -0.01f, 1.01f);
                        max2 = EditorGUILayout.Slider("2nd Max", max2, -0.01f, 1.01f);
                        min3 = EditorGUILayout.Slider("3rd Min", min3, -0.01f, 1.01f);
                        max3 = EditorGUILayout.Slider("3rd Max", max3, -0.01f, 1.01f);
                        EditorGUI.showMixedValue = false;

                        if(EditorGUI.EndChangeCheck())
                        {
                            if(min1 == max1) max1 += 0.001f;
                            if(min2 == max2) max2 += 0.001f;
                            if(min3 == max3) max3 += 0.001f;
                            shadowAOShift.vectorValue = new Vector4(
                                lilEditorGUI.GetRemapScaleValue(min1, max1),
                                lilEditorGUI.GetRemapOffsetValue(min1, max1),
                                lilEditorGUI.GetRemapScaleValue(min2, max2),
                                lilEditorGUI.GetRemapOffsetValue(min2, max2)
                            );
                            shadowAOShift2.vectorValue = new Vector4(
                                lilEditorGUI.GetRemapScaleValue(min3, max3),
                                lilEditorGUI.GetRemapOffsetValue(min3, max3),
                                0.0f,
                                0.0f
                            );
                        }
                        EditorGUI.indentLevel -= 1;
                    }
                    EditorGUILayout.EndVertical();
                }
                else if(useShadow.floatValue == 1)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    m_MaterialEditor.TexturePropertySingleLine(shadow1stColorRGBAContent, shadowColorTex);
                    EditorGUI.indentLevel += 2;
                    m_MaterialEditor.ShaderProperty(shadowBorder, GetLoc("sBorder"));
                    m_MaterialEditor.ShaderProperty(shadowBlur, GetLoc("sBlur"));
                    EditorGUI.indentLevel -= 2;
                    lilEditorGUI.DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(shadow2ndColorRGBAContent, shadow2ndColorTex);
                    EditorGUI.indentLevel += 2;
                    m_MaterialEditor.ShaderProperty(shadow2ndBorder, GetLoc("sBorder"));
                    m_MaterialEditor.ShaderProperty(shadow2ndBlur, GetLoc("sBlur"));
                    EditorGUI.indentLevel -= 2;
                    lilEditorGUI.DrawLine();
                    m_MaterialEditor.ShaderProperty(shadowEnvStrength, GetLoc("sShadowEnvStrength"));
                    m_MaterialEditor.ShaderProperty(shadowBorderColor, GetLoc("sShadowBorderColor"));
                    m_MaterialEditor.ShaderProperty(shadowBorderRange, GetLoc("sShadowBorderRange"));
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawShadowSettingsSimple()
        {
            edSet.isShowShadow = lilEditorGUI.Foldout(GetLoc("sShadowSetting"), edSet.isShowShadow);
            DrawMenuButton(GetLoc("sAnchorShadow"), PropertyBlock.Shadow);
            if(edSet.isShowShadow)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                m_MaterialEditor.ShaderProperty(useShadow, GetLoc("sShadow"));
                DrawMenuButton(GetLoc("sAnchorShadow"), PropertyBlock.Shadow);
                if(useShadow.floatValue == 1 && !isLite)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    m_MaterialEditor.ShaderProperty(shadowMaskType, sShadowMaskTypes);
                    if(shadowMaskType.floatValue == 1.0f)
                    {
                        m_MaterialEditor.TexturePropertySingleLine(maskBlendContent, shadowStrengthMask);
                        EditorGUI.indentLevel += 2;
                            m_MaterialEditor.ShaderProperty(shadowFlatBorder, GetLoc("sBorder"));
                            m_MaterialEditor.ShaderProperty(shadowFlatBlur, GetLoc("sBlur"));
                        EditorGUI.indentLevel -= 2;
                        m_MaterialEditor.ShaderProperty(shadowStrength, GetLoc("sStrength"));
                    }
                    else
                    {
                        m_MaterialEditor.TexturePropertySingleLine(maskStrengthContent, shadowStrengthMask, shadowStrength);
                    }
                    lilEditorGUI.DrawLine();
                    m_MaterialEditor.ShaderProperty(shadowColorType, sShadowColorTypes);
                    m_MaterialEditor.TexturePropertySingleLine(shadow1stColorRGBAContent, shadowColorTex, shadowColor);
                    EditorGUI.indentLevel += 2;
                        m_MaterialEditor.ShaderProperty(shadowBorder, GetLoc("sBorder"));
                        m_MaterialEditor.ShaderProperty(shadowBlur, GetLoc("sBlur"));
                        m_MaterialEditor.ShaderProperty(shadowReceive, GetLoc("sReceiveShadow"));
                    EditorGUI.indentLevel -= 2;
                    lilEditorGUI.DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(shadow2ndColorRGBAContent, shadow2ndColorTex, shadow2ndColor);
                    EditorGUI.indentLevel += 2;
                        lilEditorGUI.DrawColorAsAlpha(shadow2ndColor);
                        if(shadow2ndColor.colorValue.a > 0)
                        {
                            m_MaterialEditor.ShaderProperty(shadow2ndBorder, GetLoc("sBorder"));
                            m_MaterialEditor.ShaderProperty(shadow2ndBlur, GetLoc("sBlur"));
                            m_MaterialEditor.ShaderProperty(shadow2ndReceive, GetLoc("sReceiveShadow"));
                        }
                    EditorGUI.indentLevel -= 2;
                    lilEditorGUI.DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(shadow3rdColorRGBAContent, shadow3rdColorTex, shadow3rdColor);
                    EditorGUI.indentLevel += 2;
                        lilEditorGUI.DrawColorAsAlpha(shadow3rdColor);
                        if(shadow3rdColor.colorValue.a > 0)
                        {
                            m_MaterialEditor.ShaderProperty(shadow3rdBorder, GetLoc("sBorder"));
                            m_MaterialEditor.ShaderProperty(shadow3rdBlur, GetLoc("sBlur"));
                            m_MaterialEditor.ShaderProperty(shadow3rdReceive, GetLoc("sReceiveShadow"));
                        }
                    EditorGUI.indentLevel -= 2;
                    lilEditorGUI.DrawLine();
                    m_MaterialEditor.ShaderProperty(shadowBorderColor, GetLoc("sShadowBorderColor"));
                    m_MaterialEditor.ShaderProperty(shadowBorderRange, GetLoc("sShadowBorderRange"));
                    lilEditorGUI.DrawLine();
                    m_MaterialEditor.ShaderProperty(shadowMainStrength, GetLoc("sContrast"));
                    EditorGUILayout.EndVertical();
                }
                else if(useShadow.floatValue == 1)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    m_MaterialEditor.TexturePropertySingleLine(shadow1stColorRGBAContent, shadowColorTex);
                    EditorGUI.indentLevel += 2;
                        m_MaterialEditor.ShaderProperty(shadowBorder, GetLoc("sBorder"));
                        m_MaterialEditor.ShaderProperty(shadowBlur, GetLoc("sBlur"));
                    EditorGUI.indentLevel -= 2;
                    lilEditorGUI.DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(shadow2ndColorRGBAContent, shadow2ndColorTex);
                    EditorGUI.indentLevel += 2;
                        m_MaterialEditor.ShaderProperty(shadow2ndBorder, GetLoc("sBorder"));
                        m_MaterialEditor.ShaderProperty(shadow2ndBlur, GetLoc("sBlur"));
                    EditorGUI.indentLevel -= 2;
                    lilEditorGUI.DrawLine();
                    m_MaterialEditor.ShaderProperty(shadowEnvStrength, GetLoc("sShadowEnvStrength"));
                    m_MaterialEditor.ShaderProperty(shadowBorderColor, GetLoc("sShadowBorderColor"));
                    m_MaterialEditor.ShaderProperty(shadowBorderRange, GetLoc("sShadowBorderRange"));
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawMatCapSettings()
        {
            edSet.isShowMatCap = lilEditorGUI.Foldout(GetLoc("sMatCapSetting"), edSet.isShowMatCap);
            DrawMenuButton(GetLoc("sAnchorMatCap"), PropertyBlock.MatCap1st);
            if(edSet.isShowMatCap)
            {
                if(!isLite)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // MatCap
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useMatCap, GetLoc("sMatCap"));
                    DrawMenuButton(GetLoc("sAnchorMatCap"), PropertyBlock.MatCap1st);
                    if(useMatCap.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        MatCapTextureGUI(ref edSet.isShowMatCapUV, matcapContent, matcapTex, matcapColor, matcapBlendUV1, matcapZRotCancel, matcapPerspective, matcapVRParallaxStrength);
                        lilEditorGUI.DrawColorAsAlpha(matcapColor);
                        m_MaterialEditor.ShaderProperty(matcapMainStrength, GetLoc("sMainColorPower"));
                        m_MaterialEditor.ShaderProperty(matcapNormalStrength, GetLoc("sNormalStrength"));
                        lilEditorGUI.DrawLine();
                        TextureGUI(ref edSet.isShowMatCapBlendMask, maskBlendRGBContent, matcapBlendMask, matcapBlend);
                        m_MaterialEditor.ShaderProperty(matcapEnableLighting, GetLoc("sEnableLighting"));
                        m_MaterialEditor.ShaderProperty(matcapShadowMask, GetLoc("sShadowMask"));
                        m_MaterialEditor.ShaderProperty(matcapBackfaceMask, GetLoc("sBackfaceMask"));
                        m_MaterialEditor.ShaderProperty(matcapLod, GetLoc("sBlur"));
                        m_MaterialEditor.ShaderProperty(matcapBlendMode, sBlendModes);
                        if(matcapEnableLighting.floatValue != 0.0f && matcapBlendMode.floatValue == 3.0f && lilEditorGUI.AutoFixHelpBox(GetLoc("sHelpMatCapBlending")))
                        {
                            matcapEnableLighting.floatValue = 0.0f;
                        }
                        if(isTransparent) m_MaterialEditor.ShaderProperty(matcapApplyTransparency, GetLoc("sApplyTransparency"));
                        lilEditorGUI.DrawLine();
                        m_MaterialEditor.ShaderProperty(matcapCustomNormal, GetLoc("sMatCapCustomNormal"));
                        if(matcapCustomNormal.floatValue == 1)
                        {
                            TextureGUI(ref edSet.isShowMatCapBumpMap, normalMapContent, matcapBumpMap, matcapBumpScale);
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // MatCap 2nd
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useMatCap2nd, GetLoc("sMatCap2nd"));
                    DrawMenuButton(GetLoc("sAnchorMatCap"), PropertyBlock.MatCap2nd);
                    if(useMatCap2nd.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        MatCapTextureGUI(ref edSet.isShowMatCap2ndUV, matcapContent, matcap2ndTex, matcap2ndColor, matcap2ndBlendUV1, matcap2ndZRotCancel, matcap2ndPerspective, matcap2ndVRParallaxStrength);
                        lilEditorGUI.DrawColorAsAlpha(matcap2ndColor);
                        m_MaterialEditor.ShaderProperty(matcap2ndMainStrength, GetLoc("sMainColorPower"));
                        m_MaterialEditor.ShaderProperty(matcap2ndNormalStrength, GetLoc("sNormalStrength"));
                        lilEditorGUI.DrawLine();
                        TextureGUI(ref edSet.isShowMatCap2ndBlendMask, maskBlendRGBContent, matcap2ndBlendMask, matcap2ndBlend);
                        m_MaterialEditor.ShaderProperty(matcap2ndEnableLighting, GetLoc("sEnableLighting"));
                        m_MaterialEditor.ShaderProperty(matcap2ndShadowMask, GetLoc("sShadowMask"));
                        m_MaterialEditor.ShaderProperty(matcap2ndBackfaceMask, GetLoc("sBackfaceMask"));
                        m_MaterialEditor.ShaderProperty(matcap2ndLod, GetLoc("sBlur"));
                        m_MaterialEditor.ShaderProperty(matcap2ndBlendMode, sBlendModes);
                        if(matcap2ndEnableLighting.floatValue != 0.0f && matcap2ndBlendMode.floatValue == 3.0f && lilEditorGUI.AutoFixHelpBox(GetLoc("sHelpMatCapBlending")))
                        {
                            matcap2ndEnableLighting.floatValue = 0.0f;
                        }
                        if(isTransparent) m_MaterialEditor.ShaderProperty(matcap2ndApplyTransparency, GetLoc("sApplyTransparency"));
                        lilEditorGUI.DrawLine();
                        m_MaterialEditor.ShaderProperty(matcap2ndCustomNormal, GetLoc("sMatCapCustomNormal"));
                        if(matcap2ndCustomNormal.floatValue == 1)
                        {
                            TextureGUI(ref edSet.isShowMatCap2ndBumpMap, normalMapContent, matcap2ndBumpMap, matcap2ndBumpScale);
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useMatCap, GetLoc("sMatCap"));
                    DrawMenuButton(GetLoc("sAnchorMatCap"), PropertyBlock.MatCap1st);
                    if(useMatCap.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        MatCapTextureGUI(ref edSet.isShowMatCapUV, matcapContent, matcapTex, matcapBlendUV1, matcapZRotCancel, matcapPerspective, matcapVRParallaxStrength);
                        m_MaterialEditor.ShaderProperty(matcapMul, GetLoc("sBlendModeMul"));
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }
            }
        }

        private void DrawMatCapSettingsSimple()
        {
            edSet.isShowMatCap = lilEditorGUI.Foldout(GetLoc("sMatCapSetting"), edSet.isShowMatCap);
            DrawMenuButton(GetLoc("sAnchorMatCap"), PropertyBlock.MatCap1st);
            if(edSet.isShowMatCap)
            {
                if(!isLite)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // MatCap
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useMatCap, GetLoc("sMatCap"));
                    DrawMenuButton(GetLoc("sAnchorMatCap"), PropertyBlock.MatCap1st);
                    if(useMatCap.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        MatCapTextureGUI(ref edSet.isShowMatCapUV, matcapContent, matcapTex, matcapColor, matcapBlendUV1, matcapZRotCancel, matcapPerspective, matcapVRParallaxStrength);
                        lilEditorGUI.DrawColorAsAlpha(matcapColor);
                        lilEditorGUI.DrawLine();
                        TextureGUI(ref edSet.isShowMatCapBlendMask, maskBlendRGBContent, matcapBlendMask, matcapBlend);
                        m_MaterialEditor.ShaderProperty(matcapEnableLighting, GetLoc("sEnableLighting"));
                        m_MaterialEditor.ShaderProperty(matcapBlendMode, sBlendModes);
                        if(matcapEnableLighting.floatValue != 0.0f && matcapBlendMode.floatValue == 3.0f && lilEditorGUI.AutoFixHelpBox(GetLoc("sHelpMatCapBlending")))
                        {
                            matcapEnableLighting.floatValue = 0.0f;
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // MatCap 2nd
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useMatCap2nd, GetLoc("sMatCap2nd"));
                    DrawMenuButton(GetLoc("sAnchorMatCap"), PropertyBlock.MatCap2nd);
                    if(useMatCap2nd.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        MatCapTextureGUI(ref edSet.isShowMatCap2ndUV, matcapContent, matcap2ndTex, matcap2ndColor, matcap2ndBlendUV1, matcap2ndZRotCancel, matcap2ndPerspective, matcap2ndVRParallaxStrength);
                        lilEditorGUI.DrawColorAsAlpha(matcap2ndColor);
                        lilEditorGUI.DrawLine();
                        TextureGUI(ref edSet.isShowMatCap2ndBlendMask, maskBlendRGBContent, matcap2ndBlendMask, matcap2ndBlend);
                        m_MaterialEditor.ShaderProperty(matcap2ndEnableLighting, GetLoc("sEnableLighting"));
                        m_MaterialEditor.ShaderProperty(matcap2ndBlendMode, sBlendModes);
                        if(matcap2ndEnableLighting.floatValue != 0.0f && matcap2ndBlendMode.floatValue == 3.0f && lilEditorGUI.AutoFixHelpBox(GetLoc("sHelpMatCapBlending")))
                        {
                            matcap2ndEnableLighting.floatValue = 0.0f;
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useMatCap, GetLoc("sMatCap"));
                    DrawMenuButton(GetLoc("sAnchorMatCap"), PropertyBlock.MatCap1st);
                    if(useMatCap.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        MatCapTextureGUI(ref edSet.isShowMatCapUV, matcapContent, matcapTex, matcapBlendUV1, matcapZRotCancel, matcapPerspective, matcapVRParallaxStrength);
                        m_MaterialEditor.ShaderProperty(matcapMul, GetLoc("sBlendModeMul"));
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }
            }
        }

        private void DrawRimSettings()
        {
            edSet.isShowRim = lilEditorGUI.Foldout(GetLoc("sRimLightSetting"), edSet.isShowRim);
            DrawMenuButton(GetLoc("sAnchorRimLight"), PropertyBlock.RimLight);
            if(edSet.isShowRim)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                if(!isLite)
                {
                    m_MaterialEditor.ShaderProperty(useRim, GetLoc("sRimLight"));
                    DrawMenuButton(GetLoc("sAnchorRimLight"), PropertyBlock.RimLight);
                    if(useRim.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        TextureGUI(ref edSet.isShowRimColorTex, colorMaskRGBAContent, rimColorTex, rimColor);
                        lilEditorGUI.DrawColorAsAlpha(rimColor);
                        m_MaterialEditor.ShaderProperty(rimMainStrength, GetLoc("sMainColorPower"));
                        m_MaterialEditor.ShaderProperty(rimEnableLighting, GetLoc("sEnableLighting"));
                        m_MaterialEditor.ShaderProperty(rimShadowMask, GetLoc("sShadowMask"));
                        m_MaterialEditor.ShaderProperty(rimBackfaceMask, GetLoc("sBackfaceMask"));
                        if(isTransparent) m_MaterialEditor.ShaderProperty(rimApplyTransparency, GetLoc("sApplyTransparency"));
                        lilEditorGUI.DrawLine();
                        m_MaterialEditor.ShaderProperty(rimDirStrength, GetLoc("sRimLightDirection"));
                        if(rimDirStrength.floatValue != 0)
                        {
                            EditorGUI.indentLevel++;
                            m_MaterialEditor.ShaderProperty(rimDirRange, GetLoc("sRimDirectionRange"));
                            lilEditorGUI.InvBorderGUI(rimBorder);
                            m_MaterialEditor.ShaderProperty(rimBlur, GetLoc("sBlur"));
                            lilEditorGUI.DrawLine();
                            m_MaterialEditor.ShaderProperty(rimIndirRange, GetLoc("sRimIndirectionRange"));
                            m_MaterialEditor.ShaderProperty(rimIndirColor, GetLoc("sColor"));
                            lilEditorGUI.InvBorderGUI(rimIndirBorder);
                            m_MaterialEditor.ShaderProperty(rimIndirBlur, GetLoc("sBlur"));
                            EditorGUI.indentLevel--;
                            lilEditorGUI.DrawLine();
                        }
                        else
                        {
                            lilEditorGUI.InvBorderGUI(rimBorder);
                            m_MaterialEditor.ShaderProperty(rimBlur, GetLoc("sBlur"));
                        }
                        m_MaterialEditor.ShaderProperty(rimNormalStrength, GetLoc("sNormalStrength"));
                        m_MaterialEditor.ShaderProperty(rimFresnelPower, GetLoc("sFresnelPower"));
                        m_MaterialEditor.ShaderProperty(rimVRParallaxStrength, GetLoc("sVRParallaxStrength"));
                        EditorGUILayout.EndVertical();
                    }
                }
                else
                {
                    m_MaterialEditor.ShaderProperty(useRim, GetLoc("sRimLight"));
                    DrawMenuButton(GetLoc("sAnchorRimLight"), PropertyBlock.RimLight);
                    if(useRim.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        m_MaterialEditor.ShaderProperty(rimColor, GetLoc("sColor"));
                        m_MaterialEditor.ShaderProperty(rimShadowMask, GetLoc("sShadowMask"));
                        lilEditorGUI.DrawLine();
                        lilEditorGUI.InvBorderGUI(rimBorder);
                        m_MaterialEditor.ShaderProperty(rimBlur, GetLoc("sBlur"));
                        m_MaterialEditor.ShaderProperty(rimFresnelPower, GetLoc("sFresnelPower"));
                        EditorGUILayout.EndVertical();
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawRimSettingsSimple()
        {
            edSet.isShowRim = lilEditorGUI.Foldout(GetLoc("sRimLightSetting"), edSet.isShowRim);
            DrawMenuButton(GetLoc("sAnchorRimLight"), PropertyBlock.RimLight);
            if(edSet.isShowRim)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                if(!isLite)
                {
                    m_MaterialEditor.ShaderProperty(useRim, GetLoc("sRimLight"));
                    DrawMenuButton(GetLoc("sAnchorRimLight"), PropertyBlock.RimLight);
                    if(useRim.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        TextureGUI(ref edSet.isShowRimColorTex, colorMaskRGBAContent, rimColorTex, rimColor);
                        lilEditorGUI.DrawColorAsAlpha(rimColor);
                        lilEditorGUI.DrawLine();
                        m_MaterialEditor.ShaderProperty(rimDirStrength, GetLoc("sRimLightDirection"));
                        if(rimDirStrength.floatValue != 0)
                        {
                            EditorGUI.indentLevel++;
                            m_MaterialEditor.ShaderProperty(rimDirRange, GetLoc("sRimDirectionRange"));
                            lilEditorGUI.InvBorderGUI(rimBorder);
                            m_MaterialEditor.ShaderProperty(rimBlur, GetLoc("sBlur"));
                            lilEditorGUI.DrawLine();
                            m_MaterialEditor.ShaderProperty(rimIndirRange, GetLoc("sRimIndirectionRange"));
                            m_MaterialEditor.ShaderProperty(rimIndirColor, GetLoc("sColor"));
                            lilEditorGUI.InvBorderGUI(rimIndirBorder);
                            m_MaterialEditor.ShaderProperty(rimIndirBlur, GetLoc("sBlur"));
                            EditorGUI.indentLevel--;
                            lilEditorGUI.DrawLine();
                        }
                        else
                        {
                            lilEditorGUI.InvBorderGUI(rimBorder);
                            m_MaterialEditor.ShaderProperty(rimBlur, GetLoc("sBlur"));
                        }
                        m_MaterialEditor.ShaderProperty(rimFresnelPower, GetLoc("sFresnelPower"));
                        EditorGUILayout.EndVertical();
                    }
                }
                else
                {
                    m_MaterialEditor.ShaderProperty(useRim, GetLoc("sRimLight"));
                    DrawMenuButton(GetLoc("sAnchorRimLight"), PropertyBlock.RimLight);
                    if(useRim.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        m_MaterialEditor.ShaderProperty(rimColor, GetLoc("sColor"));
                        m_MaterialEditor.ShaderProperty(rimShadowMask, GetLoc("sShadowMask"));
                        lilEditorGUI.DrawLine();
                        lilEditorGUI.InvBorderGUI(rimBorder);
                        m_MaterialEditor.ShaderProperty(rimBlur, GetLoc("sBlur"));
                        m_MaterialEditor.ShaderProperty(rimFresnelPower, GetLoc("sFresnelPower"));
                        EditorGUILayout.EndVertical();
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawGlitterSettings()
        {
            edSet.isShowGlitter = lilEditorGUI.Foldout(GetLoc("sGlitterSetting"), edSet.isShowGlitter);
            DrawMenuButton(GetLoc("sAnchorGlitter"), PropertyBlock.Glitter);
            if(edSet.isShowGlitter)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                m_MaterialEditor.ShaderProperty(useGlitter, GetLoc("sGlitter"));
                if(useGlitter.floatValue == 1)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    m_MaterialEditor.ShaderProperty(glitterUVMode, "UV Mode|UV0|UV1");
                    TextureGUI(ref edSet.isShowGlitterColorTex, colorMaskRGBAContent, glitterColorTex, glitterColor, glitterColorTex_UVMode, "UV Mode|UV0|UV1|UV2|UV3");
                    EditorGUI.indentLevel++;
                    lilEditorGUI.DrawColorAsAlpha(glitterColor);
                    m_MaterialEditor.ShaderProperty(glitterMainStrength, GetLoc("sMainColorPower"));
                    m_MaterialEditor.ShaderProperty(glitterEnableLighting, GetLoc("sEnableLighting"));
                    m_MaterialEditor.ShaderProperty(glitterShadowMask, GetLoc("sShadowMask"));
                    m_MaterialEditor.ShaderProperty(glitterBackfaceMask, GetLoc("sBackfaceMask"));
                    if(isTransparent) m_MaterialEditor.ShaderProperty(glitterApplyTransparency, GetLoc("sApplyTransparency"));
                    EditorGUI.indentLevel--;
                    lilEditorGUI.DrawLine();
                    m_MaterialEditor.ShaderProperty(glitterApplyShape, "Shape");
                    if(glitterApplyShape.floatValue > 0.5f)
                    {
                        EditorGUI.indentLevel++;
                        TextureGUI(ref edSet.isShowGlitterShapeTex, customMaskContent, glitterShapeTex);
                        m_MaterialEditor.ShaderProperty(glitterAtras, "Atras");
                        m_MaterialEditor.ShaderProperty(glitterAngleRandomize, GetLoc("sRandomize") + " (Angle)");
                        EditorGUI.indentLevel--;
                    }
                    lilEditorGUI.DrawLine();

                    // Param1
                    Vector2 scale = new Vector2(256.0f/glitterParams1.vectorValue.x, 256.0f/glitterParams1.vectorValue.y);
                    float size = glitterParams1.vectorValue.z == 0.0f ? 0.0f : Mathf.Sqrt(glitterParams1.vectorValue.z);
                    float density = Mathf.Sqrt(1.0f / glitterParams1.vectorValue.w) / 1.5f;
                    float sensitivity = lilEditorGUI.RoundFloat1000000(glitterSensitivity.floatValue / density);
                    density = lilEditorGUI.RoundFloat1000000(density);
                    EditorGUIUtility.wideMode = true;

                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = glitterParams1.hasMixedValue || glitterSensitivity.hasMixedValue;
                    scale = EditorGUILayout.Vector2Field(GetLoc("sScale"), scale);
                    size = EditorGUILayout.Slider(GetLoc("sParticleSize"), size, 0.0f, 2.0f);
                    EditorGUI.showMixedValue = false;

                    m_MaterialEditor.ShaderProperty(glitterScaleRandomize, GetLoc("sRandomize") + " (Size)");

                    EditorGUI.showMixedValue = glitterParams1.hasMixedValue || glitterSensitivity.hasMixedValue;
                    density = EditorGUILayout.Slider(GetLoc("sDensity"), density, 0.001f, 1.0f);
                    sensitivity = EditorGUILayout.FloatField(GetLoc("sSensitivity"), sensitivity);
                    EditorGUI.showMixedValue = false;

                    if(EditorGUI.EndChangeCheck())
                    {
                        scale.x = Mathf.Max(scale.x, 0.0000001f);
                        scale.y = Mathf.Max(scale.y, 0.0000001f);
                        glitterParams1.vectorValue = new Vector4(256.0f/scale.x, 256.0f/scale.y, size * size, 1.0f / (density * density * 1.5f * 1.5f));
                        glitterSensitivity.floatValue = Mathf.Max(sensitivity * density, 0.25f);
                    }

                    // Other
                    m_MaterialEditor.ShaderProperty(glitterParams2, sGlitterParams2);
                    m_MaterialEditor.ShaderProperty(glitterVRParallaxStrength, GetLoc("sVRParallaxStrength"));
                    m_MaterialEditor.ShaderProperty(glitterNormalStrength, GetLoc("sNormalStrength"));
                    m_MaterialEditor.ShaderProperty(glitterPostContrast, GetLoc("sPostContrast"));
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawOutlineSettings(Material material)
        {
            if(isMultiVariants || isRefr || isFur || isGem || isFakeShadow || material.shader.name.Contains("Overlay")) return;
            edSet.isShowOutline = lilEditorGUI.Foldout(GetLoc("sOutlineSetting"), edSet.isShowOutline);
            DrawMenuButton(GetLoc("sAnchorOutline"), PropertyBlock.Outline);
            if(edSet.isShowOutline)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                if(isShowRenderMode)
                {
                    if(isOutl != EditorGUILayout.ToggleLeft(GetLoc("sOutline"), isOutl, customToggleFont))
                    {
                        isOutl = !isOutl;
                        SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf);
                    }
                }
                else if(isCustomShader)
                {
                    EditorGUILayout.LabelField(GetLoc("sOutline"), customToggleFont);
                }
                if(!isLite && isOutl)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    TextureGUI(ref edSet.isShowOutlineMap, mainColorRGBAContent, outlineTex, outlineColor, outlineTex_ScrollRotate, true, true);
                    EditorGUI.indentLevel++;
                    ToneCorrectionGUI(outlineTexHSVG);
                    if(lilEditorGUI.EditorButton(GetLoc("sBake")))
                    {
                        outlineTex.textureValue = AutoBakeOutlineTexture(material);
                        outlineTexHSVG.vectorValue = lilConstants.defaultHSVG;
                    }
                    EditorGUI.indentLevel--;
                    lilEditorGUI.DrawLine();
                    GUILayout.Label(GetLoc("sHighlight"), boldLabel);
                    EditorGUI.indentLevel++;
                    m_MaterialEditor.ShaderProperty(outlineLitColor, GetLoc("sColor"));
                    lilEditorGUI.DrawColorAsAlpha(outlineLitColor);
                    if(outlineLitColor.colorValue.a > 0)
                    {
                        m_MaterialEditor.ShaderProperty(outlineLitApplyTex, GetLoc("sColorFromMain"));
                        float min = lilEditorGUI.GetRemapMinValue(outlineLitScale.floatValue, outlineLitOffset.floatValue);
                        float max = lilEditorGUI.GetRemapMaxValue(outlineLitScale.floatValue, outlineLitOffset.floatValue);
                        EditorGUI.BeginChangeCheck();
                        EditorGUI.showMixedValue = alphaMaskScale.hasMixedValue || alphaMaskValue.hasMixedValue;
                        min = EditorGUILayout.Slider("Min", min, -0.01f, 1.01f);
                        max = EditorGUILayout.Slider("Max", max, -0.01f, 1.01f);
                        EditorGUI.showMixedValue = false;
                        if(EditorGUI.EndChangeCheck())
                        {
                            if(min == max) max += 0.001f;
                            outlineLitScale.floatValue = lilEditorGUI.GetRemapScaleValue(min, max);
                            outlineLitOffset.floatValue = lilEditorGUI.GetRemapOffsetValue(min, max);
                        }
                        m_MaterialEditor.ShaderProperty(outlineLitShadowReceive, GetLoc("sReceiveShadow"));
                    }
                    EditorGUI.indentLevel--;
                    lilEditorGUI.DrawLine();
                    m_MaterialEditor.ShaderProperty(outlineEnableLighting, GetLoc("sEnableLighting"));
                    lilEditorGUI.DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sWidth"), GetLoc("sWidthR")), outlineWidthMask, outlineWidth);
                    EditorGUI.indentLevel++;
                    m_MaterialEditor.ShaderProperty(outlineFixWidth, GetLoc("sFixWidth"));
                    m_MaterialEditor.ShaderProperty(outlineVertexR2Width, sOutlineVertexColorUsages);
                    m_MaterialEditor.ShaderProperty(outlineDeleteMesh, GetLoc("sDeleteMesh0"));
                    m_MaterialEditor.ShaderProperty(outlineZBias, "Z Bias");
                    m_MaterialEditor.ShaderProperty(outlineDisableInVR, GetLoc("sDisableInVR"));
                    EditorGUI.indentLevel--;
                    m_MaterialEditor.TexturePropertySingleLine(normalMapContent, outlineVectorTex, outlineVectorScale);
                    m_MaterialEditor.ShaderProperty(outlineVectorUVMode, "UV Mode|UV0|UV1|UV2|UV3", 2);
                    EditorGUILayout.EndVertical();
                }
                else if(isOutl)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    TextureGUI(ref edSet.isShowOutlineMap, mainColorRGBAContent, outlineTex, outlineColor, outlineTex_ScrollRotate, true, true);
                    m_MaterialEditor.ShaderProperty(outlineEnableLighting, GetLoc("sEnableLighting"));
                    lilEditorGUI.DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sWidth"), GetLoc("sWidthR")), outlineWidthMask, outlineWidth);
                    EditorGUI.indentLevel++;
                    m_MaterialEditor.ShaderProperty(outlineFixWidth, GetLoc("sFixWidth"));
                    m_MaterialEditor.ShaderProperty(outlineVertexR2Width, sOutlineVertexColorUsages);
                    m_MaterialEditor.ShaderProperty(outlineDeleteMesh, GetLoc("sDeleteMesh0"));
                    m_MaterialEditor.ShaderProperty(outlineZBias, "Z Bias");
                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawOutlineSettingsSimple(Material material)
        {
            if(isMultiVariants || isRefr || isFur || isGem || isFakeShadow || material.shader.name.Contains("Overlay")) return;
            edSet.isShowOutline = lilEditorGUI.Foldout(GetLoc("sOutlineSetting"), edSet.isShowOutline);
            DrawMenuButton(GetLoc("sAnchorOutline"), PropertyBlock.Outline);
            if(edSet.isShowOutline)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                if(isShowRenderMode)
                {
                    if(isOutl != EditorGUILayout.ToggleLeft(GetLoc("sOutline"), isOutl, customToggleFont))
                    {
                        isOutl = !isOutl;
                        SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf);
                    }
                }
                else if(isCustomShader)
                {
                    EditorGUILayout.LabelField(GetLoc("sOutline"), customToggleFont);
                }
                if(!isLite && isOutl)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    TextureGUI(ref edSet.isShowOutlineMap, mainColorRGBAContent, outlineTex, outlineColor, outlineTex_ScrollRotate, true, true);
                    m_MaterialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sWidth"), GetLoc("sWidthR")), outlineWidthMask, outlineWidth);
                    EditorGUILayout.EndVertical();
                }
                else if(isOutl)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    TextureGUI(ref edSet.isShowOutlineMap, mainColorRGBAContent, outlineTex, outlineColor, outlineTex_ScrollRotate, true, true);
                    m_MaterialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sWidth"), GetLoc("sWidthR")), outlineWidthMask, outlineWidth);
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawSpecularMode()
        {
            int specularMode = 0;
            if(specularToon.floatValue == 0.0f) specularMode = 1;
            if(specularToon.floatValue == 1.0f) specularMode = 2;
            if(applySpecular.floatValue == 0.0f) specularMode = 0;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = specularToon.hasMixedValue || applySpecular.hasMixedValue;
            specularMode = EditorGUILayout.Popup(GetLoc("sSpecularMode"),specularMode,new string[]{GetLoc("sSpecularNone"),GetLoc("sSpecularReal"),GetLoc("sSpecularToon")});
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                if(specularMode == 0)
                {
                    applySpecular.floatValue = 0.0f;
                    specularToon.floatValue = 0.0f;
                }
                if(specularMode == 1)
                {
                    applySpecular.floatValue = 1.0f;
                    specularToon.floatValue = 0.0f;
                    EditorGUI.indentLevel++;
                    m_MaterialEditor.ShaderProperty(specularNormalStrength, GetLoc("sNormalStrength"));
                    m_MaterialEditor.ShaderProperty(applySpecularFA, GetLoc("sMultiLightSpecular"));
                    EditorGUI.indentLevel--;
                }
                if(specularMode == 2)
                {
                    applySpecular.floatValue = 1.0f;
                    specularToon.floatValue = 1.0f;
                    EditorGUI.indentLevel++;
                    m_MaterialEditor.ShaderProperty(specularNormalStrength, GetLoc("sNormalStrength"));
                    m_MaterialEditor.ShaderProperty(specularBorder, GetLoc("sBorder"));
                    m_MaterialEditor.ShaderProperty(specularBlur, GetLoc("sBlur"));
                    m_MaterialEditor.ShaderProperty(applySpecularFA, GetLoc("sMultiLightSpecular"));
                    EditorGUI.indentLevel--;
                }
            }

            if(specularMode == 1)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.ShaderProperty(specularNormalStrength, GetLoc("sNormalStrength"));
                m_MaterialEditor.ShaderProperty(applySpecularFA, GetLoc("sMultiLightSpecular"));
                EditorGUI.indentLevel--;
            }
            if(specularMode == 2)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.ShaderProperty(specularNormalStrength, GetLoc("sNormalStrength"));
                m_MaterialEditor.ShaderProperty(specularBorder, GetLoc("sBorder"));
                m_MaterialEditor.ShaderProperty(specularBlur, GetLoc("sBlur"));
                m_MaterialEditor.ShaderProperty(applySpecularFA, GetLoc("sMultiLightSpecular"));
                EditorGUI.indentLevel--;
            }
        }

        private void DrawStencilSettings(Material material)
        {
            edSet.isShowStencil = lilEditorGUI.Foldout(GetLoc("sStencilSetting"), edSet.isShowStencil);
            DrawMenuButton(GetLoc("sAnchorStencil"), PropertyBlock.Stencil);
            if(edSet.isShowStencil)
            {
                if(lilEditorGUI.EditorButton("Reset"))
                {
                    isStWr = false;
                    stencilRef.floatValue = 0;
                    stencilReadMask.floatValue = 255.0f;
                    stencilWriteMask.floatValue = 255.0f;
                    stencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                    stencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                    stencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                    stencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                    if(transparentModeBuf == TransparentMode.TwoPass)
                    {
                        preStencilRef.floatValue = 0;
                        preStencilReadMask.floatValue = 255.0f;
                        preStencilWriteMask.floatValue = 255.0f;
                        preStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                        preStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        preStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        preStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                    }
                    if(isOutl)
                    {
                        outlineStencilRef.floatValue = 0;
                        outlineStencilReadMask.floatValue = 255.0f;
                        outlineStencilWriteMask.floatValue = 255.0f;
                        outlineStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                        outlineStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        outlineStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        outlineStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                    }
                    if(isFur)
                    {
                        furStencilRef.floatValue = 0;
                        furStencilReadMask.floatValue = 255.0f;
                        furStencilWriteMask.floatValue = 255.0f;
                        furStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                        furStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        furStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        furStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                    }
                }

                EditorGUILayout.BeginVertical(customBox);
                m_MaterialEditor.ShaderProperty(stencilRef, "Ref");
                m_MaterialEditor.ShaderProperty(stencilReadMask, "ReadMask");
                m_MaterialEditor.ShaderProperty(stencilWriteMask, "WriteMask");
                m_MaterialEditor.ShaderProperty(stencilComp, "Comp");
                m_MaterialEditor.ShaderProperty(stencilPass, "Pass");
                m_MaterialEditor.ShaderProperty(stencilFail, "Fail");
                m_MaterialEditor.ShaderProperty(stencilZFail, "ZFail");
                EditorGUILayout.EndVertical();

                if(transparentModeBuf == TransparentMode.TwoPass)
                {
                    EditorGUILayout.LabelField("PrePass");
                    EditorGUILayout.BeginVertical(customBox);
                    m_MaterialEditor.ShaderProperty(preStencilRef, "Ref");
                    m_MaterialEditor.ShaderProperty(preStencilReadMask, "ReadMask");
                    m_MaterialEditor.ShaderProperty(preStencilWriteMask, "WriteMask");
                    m_MaterialEditor.ShaderProperty(preStencilComp, "Comp");
                    m_MaterialEditor.ShaderProperty(preStencilPass, "Pass");
                    m_MaterialEditor.ShaderProperty(preStencilFail, "Fail");
                    m_MaterialEditor.ShaderProperty(preStencilZFail, "ZFail");
                    EditorGUILayout.EndVertical();
                }

                if(isOutl)
                {
                    EditorGUILayout.LabelField(GetLoc("sOutline"));
                    EditorGUILayout.BeginVertical(customBox);
                    m_MaterialEditor.ShaderProperty(outlineStencilRef, "Ref");
                    m_MaterialEditor.ShaderProperty(outlineStencilReadMask, "ReadMask");
                    m_MaterialEditor.ShaderProperty(outlineStencilWriteMask, "WriteMask");
                    m_MaterialEditor.ShaderProperty(outlineStencilComp, "Comp");
                    m_MaterialEditor.ShaderProperty(outlineStencilPass, "Pass");
                    m_MaterialEditor.ShaderProperty(outlineStencilFail, "Fail");
                    m_MaterialEditor.ShaderProperty(outlineStencilZFail, "ZFail");
                    EditorGUILayout.EndVertical();
                }

                if(isFur)
                {
                    EditorGUILayout.LabelField(GetLoc("sFur"));
                    EditorGUILayout.BeginVertical(customBox);
                    m_MaterialEditor.ShaderProperty(furStencilRef, "Ref");
                    m_MaterialEditor.ShaderProperty(furStencilReadMask, "ReadMask");
                    m_MaterialEditor.ShaderProperty(furStencilWriteMask, "WriteMask");
                    m_MaterialEditor.ShaderProperty(furStencilComp, "Comp");
                    m_MaterialEditor.ShaderProperty(furStencilPass, "Pass");
                    m_MaterialEditor.ShaderProperty(furStencilFail, "Fail");
                    m_MaterialEditor.ShaderProperty(furStencilZFail, "ZFail");
                    EditorGUILayout.EndVertical();
                }
            }
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Property drawer
        #region
        private void UV4Decal(MaterialProperty isDecal, MaterialProperty isLeftOnly, MaterialProperty isRightOnly, MaterialProperty shouldCopy, MaterialProperty shouldFlipMirror, MaterialProperty shouldFlipCopy, MaterialProperty tex, MaterialProperty angle, MaterialProperty decalAnimation, MaterialProperty decalSubParam, MaterialProperty uvMode)
        {
            lilEditorGUI.UV4Decal(m_MaterialEditor, isDecal, isLeftOnly, isRightOnly, shouldCopy, shouldFlipMirror, shouldFlipCopy, tex, angle, decalAnimation, decalSubParam, uvMode);
        }

        private void ToneCorrectionGUI(MaterialProperty hsvg)
        {
            lilEditorGUI.ToneCorrectionGUI(m_MaterialEditor, hsvg);
        }

        private void ToneCorrectionGUI(MaterialProperty hsvg, int indent)
        {
            lilEditorGUI.ToneCorrectionGUI(m_MaterialEditor, hsvg, indent);
        }

        private void UVSettingGUI(MaterialProperty uvst, MaterialProperty uvsr)
        {
            lilEditorGUI.UVSettingGUI(m_MaterialEditor, uvst, uvsr);
        }

        private void BlendSettingGUI(ref bool isShow, string labelName, MaterialProperty srcRGB, MaterialProperty dstRGB, MaterialProperty srcA, MaterialProperty dstA, MaterialProperty opRGB, MaterialProperty opA)
        {
            lilEditorGUI.BlendSettingGUI(m_MaterialEditor, isCustomEditor, ref isShow, labelName, srcRGB, dstRGB, srcA, dstA, opRGB, opA);
        }

        private void TextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName)
        {
            lilEditorGUI.TextureGUI(m_MaterialEditor, isCustomEditor, ref isShow, guiContent, textureName);
        }

        private void TextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba)
        {
            lilEditorGUI.TextureGUI(m_MaterialEditor, isCustomEditor, ref isShow, guiContent, textureName, rgba);
        }

        private void TextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty uvMode, string sUVMode)
        {
            lilEditorGUI.TextureGUI(m_MaterialEditor, isCustomEditor, ref isShow, guiContent, textureName, rgba, uvMode, sUVMode);
        }

        private void TextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty scrollRotate)
        {
            lilEditorGUI.TextureGUI(m_MaterialEditor, isCustomEditor, ref isShow, guiContent, textureName, rgba, scrollRotate);
        }

        private void TextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty scrollRotate, bool useCustomUV, bool useUVAnimation)
        {
            lilEditorGUI.TextureGUI(m_MaterialEditor, isCustomEditor, ref isShow, guiContent, textureName, rgba, scrollRotate, useCustomUV, useUVAnimation);
        }

        private void TextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty scrollRotate, MaterialProperty uvMode, bool useCustomUV, bool useUVAnimation)
        {
            lilEditorGUI.TextureGUI(m_MaterialEditor, isCustomEditor, ref isShow, guiContent, textureName, rgba, scrollRotate, uvMode, useCustomUV, useUVAnimation);
        }

        private void MatCapTextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty blendUV1, MaterialProperty zRotCancel, MaterialProperty perspective, MaterialProperty vrParallaxStrength)
        {
            lilEditorGUI.MatCapTextureGUI(m_MaterialEditor, isCustomEditor, ref isShow, guiContent, textureName, blendUV1, zRotCancel, perspective, vrParallaxStrength);
        }

        private void MatCapTextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty blendUV1, MaterialProperty zRotCancel, MaterialProperty perspective, MaterialProperty vrParallaxStrength)
        {
            lilEditorGUI.MatCapTextureGUI(m_MaterialEditor, isCustomEditor, ref isShow, guiContent, textureName, rgba, blendUV1, zRotCancel, perspective, vrParallaxStrength);
        }

        private void TextureBakeGUI(Material material, int bakeType)
        {
            // bakeType
            // 0 : All
            // 1 : 1st
            // 2 : 2nd
            // 3 : 3rd
            // 4 : 1st Simple Button
            // 5 : 2nd Simple Button
            // 6 : 3rd Simple Button
            string[] sBake = {GetLoc("sBakeAll"), GetLoc("sBake1st"), GetLoc("sBake2nd"), GetLoc("sBake3rd"), GetLoc("sBake"), GetLoc("sBake"), GetLoc("sBake")};
            if(lilEditorGUI.EditorButton(sBake[bakeType]))
            {
                Undo.RecordObject(material, "Bake");
                TextureBake(material, bakeType);
            }
        }

        private void AlphamaskToTextureGUI(Material material)
        {
            if(mainTex.textureValue != null && lilEditorGUI.EditorButton(GetLoc("sBakeAlphamask")))
            {
                Texture2D bakedTexture = AutoBakeAlphaMask(material);
                if(bakedTexture == mainTex.textureValue) return;

                mainTex.textureValue = bakedTexture;
                alphaMaskMode.floatValue = 0.0f;
                alphaMask.textureValue = null;
                alphaMaskValue.floatValue = 0.0f;
            }
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Bake
        #region
        private void TextureBake(Material material, int bakeType)
        {
            //bool shouldBake1st = (bakeType == 1 || bakeType == 4) && mainTex.textureValue != null;
            bool shouldNotBakeColor = (bakeType == 1 || bakeType == 4) && mainColor.colorValue == Color.white && mainTexHSVG.vectorValue == lilConstants.defaultHSVG;
            bool cannotBake1st = mainTex.textureValue == null;
            bool shouldNotBake2nd = (bakeType == 2 || bakeType == 5) && useMain2ndTex.floatValue == 0.0;
            bool shouldNotBake3rd = (bakeType == 3 || bakeType == 6) && useMain3rdTex.floatValue == 0.0;
            bool shouldNotBakeAll = bakeType == 0 && mainColor.colorValue == Color.white && mainTexHSVG.vectorValue == lilConstants.defaultHSVG && useMain2ndTex.floatValue == 0.0 && useMain3rdTex.floatValue == 0.0;
            if(cannotBake1st)
            {
                EditorUtility.DisplayDialog(GetLoc("sDialogCannotBake"), GetLoc("sDialogSetMainTex"), GetLoc("sOK"));
            }
            else if(shouldNotBakeColor)
            {
                EditorUtility.DisplayDialog(GetLoc("sDialogNoNeedBake"), GetLoc("sDialogNoChange"), GetLoc("sOK"));
            }
            else if(shouldNotBake2nd)
            {
                EditorUtility.DisplayDialog(GetLoc("sDialogNoNeedBake"), GetLoc("sDialogNotUse2nd"), GetLoc("sOK"));
            }
            else if(shouldNotBake3rd)
            {
                EditorUtility.DisplayDialog(GetLoc("sDialogNoNeedBake"), GetLoc("sDialogNotUse3rd"), GetLoc("sOK"));
            }
            else if(shouldNotBakeAll)
            {
                EditorUtility.DisplayDialog(GetLoc("sDialogNoNeedBake"), GetLoc("sDialogNotUseAll"), GetLoc("sOK"));
            }
            else
            {
                bool bake2nd = (bakeType == 0 || bakeType == 2 || bakeType == 5) && useMain2ndTex.floatValue != 0.0;
                bool bake3rd = (bakeType == 0 || bakeType == 3 || bakeType == 6) && useMain3rdTex.floatValue != 0.0;
                // run bake
                Texture2D bufMainTexture = (Texture2D)mainTex.textureValue;
                Material hsvgMaterial = new Material(ltsbaker);

                string path;

                Texture2D srcTexture = new Texture2D(2, 2);
                Texture2D srcMain2 = new Texture2D(2, 2);
                Texture2D srcMain3 = new Texture2D(2, 2);
                Texture2D srcMask2 = new Texture2D(2, 2);
                Texture2D srcMask3 = new Texture2D(2, 2);

                hsvgMaterial.SetColor(mainColor.name,           mainColor.colorValue);
                hsvgMaterial.SetVector(mainTexHSVG.name,        mainTexHSVG.vectorValue);
                hsvgMaterial.SetFloat(mainGradationStrength.name, mainGradationStrength.floatValue);
                hsvgMaterial.SetTexture(mainGradationTex.name, mainGradationTex.textureValue);

                path = AssetDatabase.GetAssetPath(material.GetTexture(mainTex.name));
                if(!string.IsNullOrEmpty(path))
                {
                    lilTextureUtils.LoadTexture(ref srcTexture, path);
                    hsvgMaterial.SetTexture(mainTex.name, srcTexture);
                }
                else
                {
                    hsvgMaterial.SetTexture(mainTex.name, Texture2D.whiteTexture);
                }

                if(bake2nd)
                {
                    hsvgMaterial.SetFloat(useMain2ndTex.name,               useMain2ndTex.floatValue);
                    hsvgMaterial.SetColor(mainColor2nd.name,                mainColor2nd.colorValue);
                    hsvgMaterial.SetFloat(main2ndTexAngle.name,             main2ndTexAngle.floatValue);
                    hsvgMaterial.SetFloat(main2ndTexIsDecal.name,           main2ndTexIsDecal.floatValue);
                    hsvgMaterial.SetFloat(main2ndTexIsLeftOnly.name,        main2ndTexIsLeftOnly.floatValue);
                    hsvgMaterial.SetFloat(main2ndTexIsRightOnly.name,       main2ndTexIsRightOnly.floatValue);
                    hsvgMaterial.SetFloat(main2ndTexShouldCopy.name,        main2ndTexShouldCopy.floatValue);
                    hsvgMaterial.SetFloat(main2ndTexShouldFlipMirror.name,  main2ndTexShouldFlipMirror.floatValue);
                    hsvgMaterial.SetFloat(main2ndTexShouldFlipCopy.name,    main2ndTexShouldFlipCopy.floatValue);
                    hsvgMaterial.SetFloat(main2ndTexIsMSDF.name,            main2ndTexIsMSDF.floatValue);
                    hsvgMaterial.SetFloat(main2ndTexBlendMode.name,         main2ndTexBlendMode.floatValue);
                    hsvgMaterial.SetTextureOffset(main2ndTex.name,          material.GetTextureOffset(main2ndTex.name));
                    hsvgMaterial.SetTextureScale(main2ndTex.name,           material.GetTextureScale(main2ndTex.name));
                    hsvgMaterial.SetTextureOffset(main2ndBlendMask.name,    material.GetTextureOffset(main2ndBlendMask.name));
                    hsvgMaterial.SetTextureScale(main2ndBlendMask.name,     material.GetTextureScale(main2ndBlendMask.name));

                    path = AssetDatabase.GetAssetPath(material.GetTexture(main2ndTex.name));
                    if(!string.IsNullOrEmpty(path))
                    {
                        lilTextureUtils.LoadTexture(ref srcMain2, path);
                        hsvgMaterial.SetTexture(main2ndTex.name, srcMain2);
                    }
                    else
                    {
                        hsvgMaterial.SetTexture(main2ndTex.name, Texture2D.whiteTexture);
                    }

                    path = AssetDatabase.GetAssetPath(material.GetTexture(main2ndBlendMask.name));
                    if(!string.IsNullOrEmpty(path))
                    {
                        lilTextureUtils.LoadTexture(ref srcMask2, path);
                        hsvgMaterial.SetTexture(main2ndBlendMask.name, srcMask2);
                    }
                    else
                    {
                        hsvgMaterial.SetTexture(main2ndBlendMask.name, Texture2D.whiteTexture);
                    }
                }

                if(bake3rd)
                {
                    hsvgMaterial.SetFloat(useMain3rdTex.name,               useMain3rdTex.floatValue);
                    hsvgMaterial.SetColor(mainColor3rd.name,                mainColor3rd.colorValue);
                    hsvgMaterial.SetFloat(main3rdTexAngle.name,             main3rdTexAngle.floatValue);
                    hsvgMaterial.SetFloat(main3rdTexIsDecal.name,           main3rdTexIsDecal.floatValue);
                    hsvgMaterial.SetFloat(main3rdTexIsLeftOnly.name,        main3rdTexIsLeftOnly.floatValue);
                    hsvgMaterial.SetFloat(main3rdTexIsRightOnly.name,       main3rdTexIsRightOnly.floatValue);
                    hsvgMaterial.SetFloat(main3rdTexShouldCopy.name,        main3rdTexShouldCopy.floatValue);
                    hsvgMaterial.SetFloat(main3rdTexShouldFlipMirror.name,  main3rdTexShouldFlipMirror.floatValue);
                    hsvgMaterial.SetFloat(main3rdTexShouldFlipCopy.name,    main3rdTexShouldFlipCopy.floatValue);
                    hsvgMaterial.SetFloat(main3rdTexIsMSDF.name,            main3rdTexIsMSDF.floatValue);
                    hsvgMaterial.SetFloat(main3rdTexBlendMode.name,         main3rdTexBlendMode.floatValue);
                    hsvgMaterial.SetTextureOffset(main3rdTex.name,          material.GetTextureOffset(main3rdTex.name));
                    hsvgMaterial.SetTextureScale(main3rdTex.name,           material.GetTextureScale(main3rdTex.name));
                    hsvgMaterial.SetTextureOffset(main3rdBlendMask.name,    material.GetTextureOffset(main3rdBlendMask.name));
                    hsvgMaterial.SetTextureScale(main3rdBlendMask.name,     material.GetTextureScale(main3rdBlendMask.name));

                    path = AssetDatabase.GetAssetPath(material.GetTexture(main3rdTex.name));
                    if(!string.IsNullOrEmpty(path))
                    {
                        lilTextureUtils.LoadTexture(ref srcMain3, path);
                        hsvgMaterial.SetTexture(main3rdTex.name, srcMain3);
                    }
                    else
                    {
                        hsvgMaterial.SetTexture(main3rdTex.name, Texture2D.whiteTexture);
                    }

                    path = AssetDatabase.GetAssetPath(material.GetTexture(main3rdBlendMask.name));
                    if(!string.IsNullOrEmpty(path))
                    {
                        lilTextureUtils.LoadTexture(ref srcMask3, path);
                        hsvgMaterial.SetTexture(main3rdBlendMask.name, srcMask3);
                    }
                    else
                    {
                        hsvgMaterial.SetTexture(main3rdBlendMask.name, Texture2D.whiteTexture);
                    }
                }

                Texture2D outTexture = null;
                RunBake(ref outTexture, srcTexture, hsvgMaterial);

                outTexture = lilTextureUtils.SaveTextureToPng(material, outTexture, mainTex.name);
                if(outTexture != mainTex.textureValue)
                {
                    mainTexHSVG.vectorValue = lilConstants.defaultHSVG;
                    mainColor.colorValue = Color.white;
                    mainGradationStrength.floatValue = 0.0f;
                    mainGradationTex.textureValue = null;
                    if(bake2nd)
                    {
                        useMain2ndTex.floatValue = 0.0f;
                        main2ndTex.textureValue = null;
                    }
                    if(bake3rd)
                    {
                        useMain3rdTex.floatValue = 0.0f;
                        main3rdTex.textureValue = null;
                    }
                    CopyTextureSetting(bufMainTexture, outTexture);
                }

                material.SetTexture(mainTex.name, outTexture);

                Object.DestroyImmediate(hsvgMaterial);
                Object.DestroyImmediate(srcTexture);
                Object.DestroyImmediate(srcMain2);
                Object.DestroyImmediate(srcMain3);
                Object.DestroyImmediate(srcMask2);
                Object.DestroyImmediate(srcMask3);
            }
        }

        private Texture2D AutoBakeMainTexture(Material material)
        {
            bool shouldNotBakeAll = mainColor.colorValue == Color.white && mainTexHSVG.vectorValue == lilConstants.defaultHSVG && useMain2ndTex.floatValue == 0.0 && useMain3rdTex.floatValue == 0.0;
            if(!shouldNotBakeAll && EditorUtility.DisplayDialog(GetLoc("sDialogRunBake"), GetLoc("sDialogBakeMain"), GetLoc("sYes"), GetLoc("sNo")))
            {
                bool bake2nd = useMain2ndTex.floatValue != 0.0;
                bool bake3rd = useMain3rdTex.floatValue != 0.0;
                // run bake
                Texture2D bufMainTexture = (Texture2D)mainTex.textureValue;
                Material hsvgMaterial = new Material(ltsbaker);

                string path;

                Texture2D srcTexture = new Texture2D(2, 2);
                Texture2D srcMain2 = new Texture2D(2, 2);
                Texture2D srcMain3 = new Texture2D(2, 2);
                Texture2D srcMask2 = new Texture2D(2, 2);
                Texture2D srcMask3 = new Texture2D(2, 2);

                hsvgMaterial.SetColor(mainColor.name,           Color.white);
                hsvgMaterial.SetVector(mainTexHSVG.name,        mainTexHSVG.vectorValue);
                hsvgMaterial.SetTexture(mainColorAdjustMask.name, mainColorAdjustMask.textureValue);
                hsvgMaterial.SetFloat(mainGradationStrength.name, mainGradationStrength.floatValue);
                hsvgMaterial.SetTexture(mainGradationTex.name, mainGradationTex.textureValue);

                path = AssetDatabase.GetAssetPath(material.GetTexture(mainTex.name));
                if(!string.IsNullOrEmpty(path))
                {
                    lilTextureUtils.LoadTexture(ref srcTexture, path);
                    hsvgMaterial.SetTexture(mainTex.name, srcTexture);
                }
                else
                {
                    hsvgMaterial.SetTexture(mainTex.name, Texture2D.whiteTexture);
                }

                if(bake2nd)
                {
                    hsvgMaterial.SetFloat(useMain2ndTex.name,               useMain2ndTex.floatValue);
                    hsvgMaterial.SetColor(mainColor2nd.name,                mainColor2nd.colorValue);
                    hsvgMaterial.SetFloat(main2ndTexAngle.name,             main2ndTexAngle.floatValue);
                    hsvgMaterial.SetFloat(main2ndTexIsDecal.name,           main2ndTexIsDecal.floatValue);
                    hsvgMaterial.SetFloat(main2ndTexIsLeftOnly.name,        main2ndTexIsLeftOnly.floatValue);
                    hsvgMaterial.SetFloat(main2ndTexIsRightOnly.name,       main2ndTexIsRightOnly.floatValue);
                    hsvgMaterial.SetFloat(main2ndTexShouldCopy.name,        main2ndTexShouldCopy.floatValue);
                    hsvgMaterial.SetFloat(main2ndTexShouldFlipMirror.name,  main2ndTexShouldFlipMirror.floatValue);
                    hsvgMaterial.SetFloat(main2ndTexShouldFlipCopy.name,    main2ndTexShouldFlipCopy.floatValue);
                    hsvgMaterial.SetFloat(main2ndTexIsMSDF.name,            main2ndTexIsMSDF.floatValue);
                    hsvgMaterial.SetFloat(main2ndTexBlendMode.name,         main2ndTexBlendMode.floatValue);
                    hsvgMaterial.SetTextureOffset(main2ndTex.name,          material.GetTextureOffset(main2ndTex.name));
                    hsvgMaterial.SetTextureScale(main2ndTex.name,           material.GetTextureScale(main2ndTex.name));
                    hsvgMaterial.SetTextureOffset(main2ndBlendMask.name,    material.GetTextureOffset(main2ndBlendMask.name));
                    hsvgMaterial.SetTextureScale(main2ndBlendMask.name,     material.GetTextureScale(main2ndBlendMask.name));

                    path = AssetDatabase.GetAssetPath(material.GetTexture(main2ndTex.name));
                    if(!string.IsNullOrEmpty(path))
                    {
                        lilTextureUtils.LoadTexture(ref srcMain2, path);
                        hsvgMaterial.SetTexture(main2ndTex.name, srcMain2);
                    }
                    else
                    {
                        hsvgMaterial.SetTexture(main2ndTex.name, Texture2D.whiteTexture);
                    }

                    path = AssetDatabase.GetAssetPath(material.GetTexture(main2ndBlendMask.name));
                    if(!string.IsNullOrEmpty(path))
                    {
                        lilTextureUtils.LoadTexture(ref srcMask2, path);
                        hsvgMaterial.SetTexture(main2ndBlendMask.name, srcMask2);
                    }
                    else
                    {
                        hsvgMaterial.SetTexture(main2ndBlendMask.name, Texture2D.whiteTexture);
                    }
                }

                if(bake3rd)
                {
                    hsvgMaterial.SetFloat(useMain3rdTex.name,               useMain3rdTex.floatValue);
                    hsvgMaterial.SetColor(mainColor3rd.name,                mainColor3rd.colorValue);
                    hsvgMaterial.SetFloat(main3rdTexAngle.name,             main3rdTexAngle.floatValue);
                    hsvgMaterial.SetFloat(main3rdTexIsDecal.name,           main3rdTexIsDecal.floatValue);
                    hsvgMaterial.SetFloat(main3rdTexIsLeftOnly.name,        main3rdTexIsLeftOnly.floatValue);
                    hsvgMaterial.SetFloat(main3rdTexIsRightOnly.name,       main3rdTexIsRightOnly.floatValue);
                    hsvgMaterial.SetFloat(main3rdTexShouldCopy.name,        main3rdTexShouldCopy.floatValue);
                    hsvgMaterial.SetFloat(main3rdTexShouldFlipMirror.name,  main3rdTexShouldFlipMirror.floatValue);
                    hsvgMaterial.SetFloat(main3rdTexShouldFlipCopy.name,    main3rdTexShouldFlipCopy.floatValue);
                    hsvgMaterial.SetFloat(main3rdTexIsMSDF.name,            main3rdTexIsMSDF.floatValue);
                    hsvgMaterial.SetFloat(main3rdTexBlendMode.name,         main3rdTexBlendMode.floatValue);
                    hsvgMaterial.SetTextureOffset(main3rdTex.name,          material.GetTextureOffset(main3rdTex.name));
                    hsvgMaterial.SetTextureScale(main3rdTex.name,           material.GetTextureScale(main3rdTex.name));
                    hsvgMaterial.SetTextureOffset(main3rdBlendMask.name,    material.GetTextureOffset(main3rdBlendMask.name));
                    hsvgMaterial.SetTextureScale(main3rdBlendMask.name,     material.GetTextureScale(main3rdBlendMask.name));

                    path = AssetDatabase.GetAssetPath(material.GetTexture(main3rdTex.name));
                    if(!string.IsNullOrEmpty(path))
                    {
                        lilTextureUtils.LoadTexture(ref srcMain3, path);
                        hsvgMaterial.SetTexture(main3rdTex.name, srcMain3);
                    }
                    else
                    {
                        hsvgMaterial.SetTexture(main3rdTex.name, Texture2D.whiteTexture);
                    }

                    path = AssetDatabase.GetAssetPath(material.GetTexture(main3rdBlendMask.name));
                    if(!string.IsNullOrEmpty(path))
                    {
                        lilTextureUtils.LoadTexture(ref srcMask3, path);
                        hsvgMaterial.SetTexture(main3rdBlendMask.name, srcMask3);
                    }
                    else
                    {
                        hsvgMaterial.SetTexture(main3rdBlendMask.name, Texture2D.whiteTexture);
                    }
                }

                Texture2D outTexture = null;
                RunBake(ref outTexture, srcTexture, hsvgMaterial);

                outTexture = lilTextureUtils.SaveTextureToPng(material, outTexture, mainTex.name);
                if(outTexture != mainTex.textureValue)
                {
                    CopyTextureSetting(bufMainTexture, outTexture);
                }

                Object.DestroyImmediate(hsvgMaterial);
                Object.DestroyImmediate(srcTexture);
                Object.DestroyImmediate(srcMain2);
                Object.DestroyImmediate(srcMain3);
                Object.DestroyImmediate(srcMask2);
                Object.DestroyImmediate(srcMask3);

                return outTexture;
            }
            else
            {
                return (Texture2D)mainTex.textureValue;
            }
        }

        private Texture2D AutoBakeShadowTexture(Material material, Texture2D bakedMainTex, int shadowType = 0, bool shouldShowDialog = true)
        {
            bool shouldNotBakeAll = useShadow.floatValue == 0.0 && shadowColor.colorValue == Color.white && shadowColorTex.textureValue == null && shadowStrengthMask.textureValue == null;
            bool shouldBake = true;
            if(shouldShowDialog) shouldBake = EditorUtility.DisplayDialog(GetLoc("sDialogRunBake"), GetLoc("sDialogBakeShadow"), GetLoc("sYes"), GetLoc("sNo"));
            if(!shouldNotBakeAll && shouldBake)
            {
                // run bake
                Texture2D bufMainTexture = bakedMainTex;
                Material hsvgMaterial = new Material(ltsbaker);

                string path;

                Texture2D srcTexture = new Texture2D(2, 2);
                Texture2D srcMain2 = new Texture2D(2, 2);
                Texture2D srcMask2 = new Texture2D(2, 2);

                hsvgMaterial.SetColor(mainColor.name,                   Color.white);
                hsvgMaterial.SetVector(mainTexHSVG.name,                lilConstants.defaultHSVG);
                hsvgMaterial.SetFloat(useMain2ndTex.name,               1.0f);
                hsvgMaterial.SetFloat(useMain3rdTex.name,               1.0f);
                hsvgMaterial.SetColor(mainColor3rd.name,                new Color(1.0f,1.0f,1.0f,shadowMainStrength.floatValue));
                hsvgMaterial.SetFloat(main3rdTexBlendMode.name,         3.0f);
                if(shadowType == 2)
                {
                    hsvgMaterial.SetColor(mainColor2nd.name,                new Color(shadow2ndColor.colorValue.r, shadow2ndColor.colorValue.g, shadow2ndColor.colorValue.b, shadow2ndColor.colorValue.a * shadowStrength.floatValue));
                    hsvgMaterial.SetFloat(main2ndTexBlendMode.name,         0.0f);
                    path = AssetDatabase.GetAssetPath(material.GetTexture(shadow2ndColorTex.name));
                }
                else if(shadowType == 3)
                {
                    hsvgMaterial.SetColor(mainColor3rd.name,                new Color(shadow3rdColor.colorValue.r, shadow3rdColor.colorValue.g, shadow3rdColor.colorValue.b, shadow3rdColor.colorValue.a * shadowStrength.floatValue));
                    hsvgMaterial.SetFloat(main3rdTexBlendMode.name,         0.0f);
                    path = AssetDatabase.GetAssetPath(material.GetTexture(shadow3rdColorTex.name));
                }
                else
                {
                    hsvgMaterial.SetColor(mainColor2nd.name,                new Color(shadowColor.colorValue.r, shadowColor.colorValue.g, shadowColor.colorValue.b, shadowStrength.floatValue));
                    hsvgMaterial.SetFloat(main2ndTexBlendMode.name,         0.0f);
                    path = AssetDatabase.GetAssetPath(material.GetTexture(shadowColorTex.name));
                }

                bool existsShadowTex = !string.IsNullOrEmpty(path);
                if(existsShadowTex)
                {
                    lilTextureUtils.LoadTexture(ref srcMain2, path);
                    hsvgMaterial.SetTexture(main2ndTex.name, srcMain2);
                }

                path = AssetDatabase.GetAssetPath(bakedMainTex);
                if(!string.IsNullOrEmpty(path))
                {
                    lilTextureUtils.LoadTexture(ref srcTexture, path);
                    hsvgMaterial.SetTexture(mainTex.name, srcTexture);
                    hsvgMaterial.SetTexture(main3rdTex.name, srcTexture);
                    if(!existsShadowTex) hsvgMaterial.SetTexture(main2ndTex.name, srcTexture);
                }
                else
                {
                    hsvgMaterial.SetTexture(mainTex.name, Texture2D.whiteTexture);
                    hsvgMaterial.SetTexture(main3rdTex.name, Texture2D.whiteTexture);
                    if(!existsShadowTex) hsvgMaterial.SetTexture(main2ndTex.name, Texture2D.whiteTexture);
                }

                path = AssetDatabase.GetAssetPath(material.GetTexture(shadowStrengthMask.name));
                if(!string.IsNullOrEmpty(path))
                {
                    lilTextureUtils.LoadTexture(ref srcMask2, path);
                    hsvgMaterial.SetTexture(main2ndBlendMask.name, srcMask2);
                    hsvgMaterial.SetTexture(main3rdBlendMask.name, srcMask2);
                }
                else
                {
                    hsvgMaterial.SetTexture(main2ndBlendMask.name, Texture2D.whiteTexture);
                    hsvgMaterial.SetTexture(main3rdBlendMask.name, Texture2D.whiteTexture);
                }

                Texture2D outTexture = null;
                RunBake(ref outTexture, srcTexture, hsvgMaterial);

                if(shadowType == 0) outTexture = lilTextureUtils.SaveTextureToPng(material, outTexture, mainTex.name);
                if(shadowType == 1) outTexture = lilTextureUtils.SaveTextureToPng(material, outTexture, mainTex.name, "_shadow_1st");
                if(shadowType == 2) outTexture = lilTextureUtils.SaveTextureToPng(material, outTexture, mainTex.name, "_shadow_2nd");
                if(outTexture != mainTex.textureValue)
                {
                    CopyTextureSetting(bufMainTexture, outTexture);
                }

                Object.DestroyImmediate(hsvgMaterial);
                Object.DestroyImmediate(srcTexture);
                Object.DestroyImmediate(srcMain2);
                Object.DestroyImmediate(srcMask2);

                return outTexture;
            }
            else
            {
                return (Texture2D)mainTex.textureValue;
            }
        }

        private Texture2D AutoBakeMatCap(Material material)
        {
            bool shouldNotBakeAll = matcapColor.colorValue == Color.white;
            if(!shouldNotBakeAll && EditorUtility.DisplayDialog(GetLoc("sDialogRunBake"), GetLoc("sDialogBakeMatCap"), GetLoc("sYes"), GetLoc("sNo")))
            {
                // run bake
                Texture2D bufMainTexture = (Texture2D)matcapTex.textureValue;
                Material hsvgMaterial = new Material(ltsbaker);

                string path;

                Texture2D srcTexture = new Texture2D(2, 2);

                hsvgMaterial.SetColor(mainColor.name,           matcapColor.colorValue);
                hsvgMaterial.SetVector(mainTexHSVG.name,        lilConstants.defaultHSVG);

                path = AssetDatabase.GetAssetPath(material.GetTexture(matcapTex.name));
                if(!string.IsNullOrEmpty(path))
                {
                    lilTextureUtils.LoadTexture(ref srcTexture, path);
                    hsvgMaterial.SetTexture(mainTex.name, srcTexture);
                }
                else
                {
                    hsvgMaterial.SetTexture(mainTex.name, Texture2D.whiteTexture);
                }

                Texture2D outTexture = null;
                RunBake(ref outTexture, srcTexture, hsvgMaterial);

                outTexture = lilTextureUtils.SaveTextureToPng(material, outTexture, matcapTex.name);
                if(outTexture != matcapTex.textureValue)
                {
                    CopyTextureSetting(bufMainTexture, outTexture);
                }

                Object.DestroyImmediate(hsvgMaterial);
                Object.DestroyImmediate(srcTexture);

                return outTexture;
            }
            else
            {
                return (Texture2D)matcapTex.textureValue;
            }
        }

        private Texture2D AutoBakeTriMask(Material material)
        {
            bool shouldNotBakeAll = matcapBlendMask.textureValue == null && rimColorTex.textureValue == null && emissionBlendMask.textureValue == null;
            if(!shouldNotBakeAll && EditorUtility.DisplayDialog(GetLoc("sDialogRunBake"), GetLoc("sDialogBakeTriMask"), GetLoc("sYes"), GetLoc("sNo")))
            {
                // run bake
                Texture2D bufMainTexture = (Texture2D)mainTex.textureValue;
                Material hsvgMaterial = new Material(ltsbaker);

                string path;

                Texture2D srcTexture = new Texture2D(2, 2);
                Texture2D srcMain2 = new Texture2D(2, 2);
                Texture2D srcMain3 = new Texture2D(2, 2);

                hsvgMaterial.EnableKeyword("_TRIMASK");

                path = AssetDatabase.GetAssetPath(matcapBlendMask.textureValue);
                if(!string.IsNullOrEmpty(path))
                {
                    lilTextureUtils.LoadTexture(ref srcTexture, path);
                    hsvgMaterial.SetTexture(mainTex.name, srcTexture);
                }
                else
                {
                    hsvgMaterial.SetTexture(mainTex.name, Texture2D.whiteTexture);
                }

                path = AssetDatabase.GetAssetPath(rimColorTex.textureValue);
                if(!string.IsNullOrEmpty(path))
                {
                    lilTextureUtils.LoadTexture(ref srcMain2, path);
                    hsvgMaterial.SetTexture(main2ndTex.name, srcMain2);
                }
                else
                {
                    hsvgMaterial.SetTexture(main2ndTex.name, Texture2D.whiteTexture);
                }

                path = AssetDatabase.GetAssetPath(emissionBlendMask.textureValue);
                if(!string.IsNullOrEmpty(path))
                {
                    lilTextureUtils.LoadTexture(ref srcMain3, path);
                    hsvgMaterial.SetTexture(main3rdTex.name, srcMain3);
                }
                else
                {
                    hsvgMaterial.SetTexture(main3rdTex.name, Texture2D.whiteTexture);
                }

                Texture2D outTexture = null;
                RunBake(ref outTexture, srcTexture, hsvgMaterial, bufMainTexture);

                outTexture = lilTextureUtils.SaveTextureToPng(material, outTexture, mainTex.name);
                if(outTexture != mainTex.textureValue && mainTex.textureValue != null)
                {
                    CopyTextureSetting(bufMainTexture, outTexture);
                }

                Object.DestroyImmediate(hsvgMaterial);
                Object.DestroyImmediate(srcTexture);

                return outTexture;
            }
            else
            {
                return null;
            }
        }

        private Texture2D AutoBakeAlphaMask(Material material)
        {
            // run bake
            Texture2D bufMainTexture = (Texture2D)mainTex.textureValue;
            Material hsvgMaterial = new Material(ltsbaker);

            string path;

            Texture2D srcTexture = new Texture2D(2, 2);
            Texture2D srcAlphaMask = new Texture2D(2, 2);

            hsvgMaterial.EnableKeyword("_ALPHAMASK");
            hsvgMaterial.SetColor(mainColor.name,           Color.white);
            hsvgMaterial.SetVector(mainTexHSVG.name,        lilConstants.defaultHSVG);
            hsvgMaterial.SetFloat(alphaMaskMode.name,       alphaMaskMode.floatValue);
            hsvgMaterial.SetFloat(alphaMaskScale.name,      alphaMaskScale.floatValue);
            hsvgMaterial.SetFloat(alphaMaskValue.name,      alphaMaskValue.floatValue);

            path = AssetDatabase.GetAssetPath(bufMainTexture);
            if(!string.IsNullOrEmpty(path))
            {
                lilTextureUtils.LoadTexture(ref srcTexture, path);
                hsvgMaterial.SetTexture(mainTex.name, srcTexture);
            }
            else
            {
                hsvgMaterial.SetTexture(mainTex.name, Texture2D.whiteTexture);
            }

            path = AssetDatabase.GetAssetPath(material.GetTexture(alphaMask.name));
            if(!string.IsNullOrEmpty(path))
            {
                lilTextureUtils.LoadTexture(ref srcAlphaMask, path);
                hsvgMaterial.SetTexture(alphaMask.name, srcAlphaMask);
            }
            else
            {
                return (Texture2D)mainTex.textureValue;
            }

            Texture2D outTexture = null;
            RunBake(ref outTexture, srcTexture, hsvgMaterial);

            outTexture = lilTextureUtils.SaveTextureToPng(outTexture, bufMainTexture);
            if(outTexture != bufMainTexture)
            {
                CopyTextureSetting(bufMainTexture, outTexture);
                string savePath = AssetDatabase.GetAssetPath(outTexture);
                TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(savePath);
                textureImporter.alphaIsTransparency = true;
                AssetDatabase.ImportAsset(savePath);
            }

            Object.DestroyImmediate(hsvgMaterial);
            Object.DestroyImmediate(srcTexture);

            return outTexture;
        }

        private Texture2D AutoBakeOutlineTexture(Material material)
        {
            bool shouldNotBakeOutline = outlineTex.textureValue == null || outlineTexHSVG.vectorValue == lilConstants.defaultHSVG;
            if(!shouldNotBakeOutline && EditorUtility.DisplayDialog(GetLoc("sDialogRunBake"), GetLoc("sDialogBakeOutline"), GetLoc("sYes"), GetLoc("sNo")))
            {
                // run bake
                Texture2D bufMainTexture = (Texture2D)outlineTex.textureValue;
                Material hsvgMaterial = new Material(ltsbaker);

                string path;

                Texture2D srcTexture = new Texture2D(2, 2);

                hsvgMaterial.SetColor(mainColor.name,                   Color.white);
                hsvgMaterial.SetVector(mainTexHSVG.name,                outlineTexHSVG.vectorValue);

                path = AssetDatabase.GetAssetPath(material.GetTexture(outlineTex.name));
                if(!string.IsNullOrEmpty(path))
                {
                    lilTextureUtils.LoadTexture(ref srcTexture, path);
                    hsvgMaterial.SetTexture(mainTex.name, srcTexture);
                }
                else
                {
                    hsvgMaterial.SetTexture(mainTex.name, Texture2D.whiteTexture);
                }

                Texture2D outTexture = null;
                RunBake(ref outTexture, srcTexture, hsvgMaterial);

                outTexture = lilTextureUtils.SaveTextureToPng(material, outTexture, mainTex.name);
                if(outTexture != mainTex.textureValue)
                {
                    CopyTextureSetting(bufMainTexture, outTexture);
                }

                Object.DestroyImmediate(hsvgMaterial);
                Object.DestroyImmediate(srcTexture);

                return outTexture;
            }
            else
            {
                return (Texture2D)outlineTex.textureValue;
            }
        }

        private void AutoBakeColoredMask(Material material, MaterialProperty masktex, MaterialProperty maskcolor, string propName)
        {
            if(propName.Contains("Shadow"))
            {
                int shadowType = propName.Contains("2nd") ? 2 : 1;
                shadowType = propName.Contains("3rd") ? 3 : shadowType;
                AutoBakeShadowTexture(material, (Texture2D)mainTex.textureValue, shadowType, false);
                return;
            }

            Material hsvgMaterial = new Material(ltsbaker);
            hsvgMaterial.SetColor(mainColor.name, maskcolor.colorValue);

            Texture2D bufMainTexture = Texture2D.whiteTexture;
            if(masktex != null && masktex.textureValue != null) bufMainTexture = (Texture2D)masktex.textureValue;
            string path = "";

            Texture2D srcTexture = new Texture2D(2, 2);

            if(masktex != null) path = AssetDatabase.GetAssetPath(bufMainTexture);
            if(!string.IsNullOrEmpty(path))
            {
                lilTextureUtils.LoadTexture(ref srcTexture, path);
                hsvgMaterial.SetTexture(mainTex.name, srcTexture);
            }
            else
            {
                hsvgMaterial.SetTexture(mainTex.name, Texture2D.whiteTexture);
            }

            Texture2D outTexture = null;
            RunBake(ref outTexture, srcTexture, hsvgMaterial);

            if(!string.IsNullOrEmpty(path)) path = Path.GetDirectoryName(path) + "/" + material.name + "_" + propName;
            else                            path = "Assets/" + material.name + "_" + propName;
            outTexture = lilTextureUtils.SaveTextureToPng(outTexture, path);
            if(outTexture != bufMainTexture)
            {
                CopyTextureSetting(bufMainTexture, outTexture);
            }

            Object.DestroyImmediate(hsvgMaterial);
            Object.DestroyImmediate(srcTexture);
        }

        public static void RunBake(ref Texture2D outTexture, Texture2D srcTexture, Material material, Texture2D referenceTexture = null)
        {
            int width = 4096;
            int height = 4096;
            if(referenceTexture != null)
            {
                width = referenceTexture.width;
                height = referenceTexture.height;
            }
            else if(srcTexture != null)
            {
                width = srcTexture.width;
                height = srcTexture.height;
            }
            outTexture = new Texture2D(width, height);

            RenderTexture bufRT = RenderTexture.active;
            RenderTexture dstTexture = RenderTexture.GetTemporary(width, height);
            Graphics.Blit(srcTexture, dstTexture, material);
            RenderTexture.active = dstTexture;
            outTexture.ReadPixels(new Rect(0, 0, srcTexture.width, srcTexture.height), 0, 0);
            outTexture.Apply();
            RenderTexture.active = bufRT;
            RenderTexture.ReleaseTemporary(dstTexture);
        }

        private void CopyTextureSetting(Texture2D fromTexture, Texture2D toTexture)
        {
            if(fromTexture == null || toTexture == null) return;
            string fromPath = AssetDatabase.GetAssetPath(fromTexture);
            string toPath = AssetDatabase.GetAssetPath(toTexture);
            TextureImporter fromTextureImporter = (TextureImporter)AssetImporter.GetAtPath(fromPath);
            TextureImporter toTextureImporter = (TextureImporter)AssetImporter.GetAtPath(toPath);
            if(fromTextureImporter == null || toTextureImporter == null) return;

            TextureImporterSettings fromTextureImporterSettings = new TextureImporterSettings();
            fromTextureImporter.ReadTextureSettings(fromTextureImporterSettings);
            toTextureImporter.SetTextureSettings(fromTextureImporterSettings);
            toTextureImporter.SetPlatformTextureSettings(fromTextureImporter.GetDefaultPlatformTextureSettings());
            AssetDatabase.ImportAsset(toPath);
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Custom Window
        #region
        public class lilMaterialEditor : EditorWindow
        {
            private Vector2 scrollPosition = Vector2.zero;
            private MaterialEditor materialEditor;
            private Material material;
            private MaterialProperty[] props;

            [MenuItem("Window/_lil/[Beta] lilToon Multi-Editor")]
            static void Init()
            {
                lilMaterialEditor window = (lilMaterialEditor)GetWindow(typeof(lilMaterialEditor), false, "[Beta] lilToon Multi-Editor");
                window.Show();
            }

            private void OnGUI()
            {
                Object[] objects = Selection.GetFiltered<Material>(SelectionMode.DeepAssets).Where(obj => obj.shader != null).Where(obj => obj.shader.name.Contains("lilToon")).ToArray();
                if(objects.Length == 0) return;

                props = MaterialEditor.GetMaterialProperties(objects);
                if(props == null) return;

                material = (Material)objects[0];
                isCustomEditor = true;
                isMultiVariants = objects.Any(obj => ((Material)obj).shader != material.shader);
                materialEditor = (MaterialEditor)Editor.CreateEditor(objects, typeof(MaterialEditor));
                lilToonInspector inspector = new lilToonInspector();

                EditorGUILayout.LabelField("Selected Materials", string.Join(", ", objects.Select(obj => obj.name).ToArray()), EditorStyles.boldLabel);
                lilEditorGUI.DrawLine();
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                EditorGUILayout.BeginVertical(InitializeMarginBox(20, 4, 4));
                inspector.DrawAllGUI(materialEditor, props, material);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
            }

            private static GUIStyle InitializeMarginBox(int left, int right, int top)
            {
                return new GUIStyle
                    {
                        border = new RectOffset(0, 0, 0, 0),
                        margin = new RectOffset(left, right, top, 0),
                        padding = new RectOffset(0, 0, 0, 0),
                        overflow = new RectOffset(0, 0, 0, 0)
                    };
            }
        }
        #endregion

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

            foreach(FieldInfo field in typeof(lilToonSetting).GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                if(field.FieldType != typeof(bool) || (bool)field.GetValue(ssA) == (bool)field.GetValue(ssB)) continue;
                return false;
            }

            return true;
        }

        [Obsolete("This may be deleted in the future.")]
        public static void CopyShaderSetting(ref lilToonSetting ssA, lilToonSetting ssB)
        {
            if(ssA == null || ssB == null) return;

            foreach(FieldInfo field in typeof(lilToonSetting).GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
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
        [Obsolete(WARN_ABOUT_DIRECTORY)] public static bool ExistsEncryption() { return lilDirectoryManager.ExistsEncryption(); }
        #endregion
    }
}
#endif