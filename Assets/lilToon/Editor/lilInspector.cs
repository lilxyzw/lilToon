#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using lilToon.lilRenderPipelineReader;
#if VRC_SDK_VRCSDK3 && !UDON
    using VRC.SDK3.Avatars.Components;
#endif

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

        protected virtual void DrawCustomProperties(Material material)
        {
            #pragma warning disable 0618
            DrawCustomProperties(m_MaterialEditor, material, boxOuter, boxInnerHalf, boxInner, customBox, customToggleFont, GUI.skin.button);
            #pragma warning restore 0618
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Enum
        #region
        public enum EditorMode
        {
            Simple,
            Advanced,
            Preset,
            Settings
        }

        public enum RenderingMode
        {
            Opaque,
            Cutout,
            Transparent,
            Refraction,
            RefractionBlur,
            Fur,
            FurCutout,
            FurTwoPass,
            Gem
        }

        public enum TransparentMode
        {
            Normal,
            OnePass,
            TwoPass
        }

        public enum BlendMode
        {
            Alpha,
            Add,
            Screen,
            Mul
        }

        public enum lilLightingPreset
        {
            Default,
            SemiMonochrome
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Constant
        #region
        public const string currentVersionName = "1.3.0";
        public const int currentVersionValue = 26;

        private const string boothURL = "https://lilxyzw.booth.pm/";
        private const string githubURL = "https://github.com/lilxyzw/lilToon";
        public const string versionInfoURL = "https://raw.githubusercontent.com/lilxyzw/lilToon/master/version.json";
        private const string editorSettingTempPath          = "Temp/lilToonEditorSetting";
        public const string versionInfoTempPath             = "Temp/lilToonVersion";
        public const string packageListTempPath             = "Temp/lilToonPackageList";
        public const string postBuildTempPath               = "Temp/lilToonPostBuild";
        public const string startupTempPath                 = "Temp/lilToonStartup";
        private static readonly string[] mainTexCheckWords = new[] {"mask", "shadow", "shade", "outline", "normal", "bumpmap", "matcap", "rimlight", "emittion", "reflection", "specular", "roughness", "smoothness", "metallic", "metalness", "opacity", "parallax", "displacement", "height", "ambient", "occlusion"};

        #if NET_4_6
            public const string rspPath = "Assets/csc.rsp";
        #else
            public const string rspPath = "Assets/mcs.rsp";
        #endif

        public static string GetMainFolderPath()            { return GUIDToPath("05d1d116436047941ad97d1b9064ee05"); } // "Assets/lilToon"
        public static string GetEditorFolderPath()          { return GUIDToPath("3e73d675b9c1adc4f8b6b8ef01bce51c"); } // "Assets/lilToon/Editor"
        public static string GetPresetsFolderPath()         { return GUIDToPath("35817d21af2f3134182c4a7e4c07786b"); } // "Assets/lilToon/Presets"
        public static string GetEditorPath()                { return GUIDToPath("aefa51cbc37d602418a38a02c3b9afb9"); } // "Assets/lilToon/Editor/lilInspector.cs"
        public static string GetShaderFolderPath()          { return GUIDToPath("ac0a8f602b5e72f458f4914bf08f0269"); } // "Assets/lilToon/Shader"
        public static string GetShaderPipelinePath()        { return GUIDToPath("32299664512e2e042bbc351c1d46d383"); } // "Assets/lilToon/Shader/Includes/lil_pipeline.hlsl";
        public static string GetShaderCommonPath()          { return GUIDToPath("5520e766422958546bbe885a95d5a67e"); } // "Assets/lilToon/Shader/Includes/lil_common.hlsl";
        public static string GetShaderSettingHLSLPath()     { return GUIDToPath("937115b0cd7c27140b76bbd51c6ee76b"); } // "Assets/lilToon/Shader/Includes/lil_setting.hlsl";
        public static string GetEditorLanguageFileGUID()    { return GUIDToPath("a63ad2f5296744a4bad011de744ba8ba"); } // "Assets/lilToon/Editor/Resources/lang.txt"
        public static string GetAvatarEncryptionPath()      { return GUIDToPath("f9787bf8ed5154f4b931278945ac8ca1"); } // "Assets/AvaterEncryption";
        public static string GetGUIBoxInDarkPath()          { return GUIDToPath("bb1313c9ea1425b41b74e98fd04bcbc8"); } // "Assets/lilToon/Editor/Resources/gui_box_inner_dark.guiskin"
        public static string GetGUIBoxInLightPath()         { return GUIDToPath("f18d71f528511e748887f5e246abcc16"); } // "Assets/lilToon/Editor/Resources/gui_box_inner_light.guiskin"
        public static string GetGUIBoxInHalfDarkPath()      { return GUIDToPath("a72199a4c9cc3714d8edfbc5d3b13823"); } // "Assets/lilToon/Editor/Resources/gui_box_inner_half_dark.guiskin"
        public static string GetGUIBoxInHalfLightPath()     { return GUIDToPath("8343038a4a0cbef4d8af45c073520436"); } // "Assets/lilToon/Editor/Resources/gui_box_inner_half_light.guiskin"
        public static string GetGUIBoxOutDarkPath()         { return GUIDToPath("29f3c01461cd0474eab36bf2e939bb58"); } // "Assets/lilToon/Editor/Resources/gui_box_outer_dark.guiskin"
        public static string GetGUIBoxOutLightPath()        { return GUIDToPath("16cc103a658d8404894e66dd8f35cb77"); } // "Assets/lilToon/Editor/Resources/gui_box_outer_light.guiskin"
        public static string GetGUICustomBoxDarkPath()      { return GUIDToPath("45dfb1bafd2c7d34ab453c29c0b1f46e"); } // "Assets/lilToon/Editor/Resources/gui_custom_box_dark.guiskin"
        public static string GetGUICustomBoxLightPath()     { return GUIDToPath("a1ed8756474bfd34f80fa22e6c43b2e5"); } // "Assets/lilToon/Editor/Resources/gui_custom_box_light.guiskin"
        public static string[] GetShaderFolderPaths()       { return new[] {GetShaderFolderPath()}; }
        public static string GetSettingFolderPath()         { return GetMainFolderPath(); }
        public static string GetShaderSettingPath()         { return GetMainFolderPath() + "/ShaderSetting.asset"; }
        public static string GUIDToPath(string GUID)        { return AssetDatabase.GUIDToAssetPath(GUID); }

        public static readonly Vector2 defaultTextureOffset = new Vector2(0.0f,0.0f);
        public static readonly Vector2 defaultTextureScale = new Vector2(1.0f,1.0f);
        public static readonly Vector4 defaultScrollRotate = new Vector4(0.0f,0.0f,0.0f,0.0f);
        public static readonly Vector4 defaultHSVG = new Vector4(0.0f,1.0f,1.0f,1.0f);
        public static readonly Vector4 defaultKeys = new Vector4(0.0f,0.0f,0.0f,0.0f);
        public static readonly Vector4 defaultDecalAnim = new Vector4(1.0f,1.0f,1.0f,30.0f);
        public static readonly Vector4 defaultDissolveParams = new Vector4(0.0f,0.0f,0.5f,0.1f);
        public static readonly Vector4 defaultDistanceFadeParams = new Vector4(0.1f,0.01f,0.0f,0.0f);
        public static readonly Color lineColor = EditorGUIUtility.isProSkin ? new Color(0.35f,0.35f,0.35f,1.0f) : new Color(0.4f,0.4f,0.4f,1.0f);
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Shader
        #region
        protected static Shader lts         = Shader.Find("lilToon");
        protected static Shader ltsc        = Shader.Find("Hidden/lilToonCutout");
        protected static Shader ltst        = Shader.Find("Hidden/lilToonTransparent");
        protected static Shader ltsot       = Shader.Find("Hidden/lilToonOnePassTransparent");
        protected static Shader ltstt       = Shader.Find("Hidden/lilToonTwoPassTransparent");

        protected static Shader ltso        = Shader.Find("Hidden/lilToonOutline");
        protected static Shader ltsco       = Shader.Find("Hidden/lilToonCutoutOutline");
        protected static Shader ltsto       = Shader.Find("Hidden/lilToonTransparentOutline");
        protected static Shader ltsoto      = Shader.Find("Hidden/lilToonOnePassTransparentOutline");
        protected static Shader ltstto      = Shader.Find("Hidden/lilToonTwoPassTransparentOutline");

        protected static Shader ltsoo       = Shader.Find("_lil/[Optional] lilToonOutlineOnly");
        protected static Shader ltscoo      = Shader.Find("_lil/[Optional] lilToonCutoutOutlineOnly");
        protected static Shader ltstoo      = Shader.Find("_lil/[Optional] lilToonTransparentOutlineOnly");

        protected static Shader ltstess     = Shader.Find("Hidden/lilToonTessellation");
        protected static Shader ltstessc    = Shader.Find("Hidden/lilToonTessellationCutout");
        protected static Shader ltstesst    = Shader.Find("Hidden/lilToonTessellationTransparent");
        protected static Shader ltstessot   = Shader.Find("Hidden/lilToonTessellationOnePassTransparent");
        protected static Shader ltstesstt   = Shader.Find("Hidden/lilToonTessellationTwoPassTransparent");

        protected static Shader ltstesso    = Shader.Find("Hidden/lilToonTessellationOutline");
        protected static Shader ltstessco   = Shader.Find("Hidden/lilToonTessellationCutoutOutline");
        protected static Shader ltstessto   = Shader.Find("Hidden/lilToonTessellationTransparentOutline");
        protected static Shader ltstessoto  = Shader.Find("Hidden/lilToonTessellationOnePassTransparentOutline");
        protected static Shader ltstesstto  = Shader.Find("Hidden/lilToonTessellationTwoPassTransparentOutline");

        protected static Shader ltsl        = Shader.Find("Hidden/lilToonLite");
        protected static Shader ltslc       = Shader.Find("Hidden/lilToonLiteCutout");
        protected static Shader ltslt       = Shader.Find("Hidden/lilToonLiteTransparent");
        protected static Shader ltslot      = Shader.Find("Hidden/lilToonLiteOnePassTransparent");
        protected static Shader ltsltt      = Shader.Find("Hidden/lilToonLiteTwoPassTransparent");

        protected static Shader ltslo       = Shader.Find("Hidden/lilToonLiteOutline");
        protected static Shader ltslco      = Shader.Find("Hidden/lilToonLiteCutoutOutline");
        protected static Shader ltslto      = Shader.Find("Hidden/lilToonLiteTransparentOutline");
        protected static Shader ltsloto     = Shader.Find("Hidden/lilToonLiteOnePassTransparentOutline");
        protected static Shader ltsltto     = Shader.Find("Hidden/lilToonLiteTwoPassTransparentOutline");

        protected static Shader ltsref      = Shader.Find("Hidden/lilToonRefraction");
        protected static Shader ltsrefb     = Shader.Find("Hidden/lilToonRefractionBlur");
        protected static Shader ltsfur      = Shader.Find("Hidden/lilToonFur");
        protected static Shader ltsfurc     = Shader.Find("Hidden/lilToonFurCutout");
        protected static Shader ltsfurtwo   = Shader.Find("Hidden/lilToonFurTwoPass");
        protected static Shader ltsfuro     = Shader.Find("_lil/[Optional] lilToonFurOnly");
        protected static Shader ltsfuroc    = Shader.Find("_lil/[Optional] lilToonFurOnlyCutout");
        protected static Shader ltsfurotwo  = Shader.Find("_lil/[Optional] lilToonFurOnlyTwoPass");

        protected static Shader ltsgem      = Shader.Find("Hidden/lilToonGem");

        protected static Shader ltsfs       = Shader.Find("_lil/lilToonFakeShadow");

        protected static Shader ltsover     = Shader.Find("_lil/[Optional] lilToonOverlay");
        protected static Shader ltsoover    = Shader.Find("_lil/[Optional] lilToonOverlayOnePass");
        protected static Shader ltslover    = Shader.Find("_lil/[Optional] lilToonLiteOverlay");
        protected static Shader ltsloover   = Shader.Find("_lil/[Optional] lilToonLiteOverlayOnePass");

        protected static Shader ltsbaker    = Shader.Find("Hidden/ltsother_baker");
        protected static Shader ltspo       = Shader.Find("Hidden/ltspass_opaque");
        protected static Shader ltspc       = Shader.Find("Hidden/ltspass_cutout");
        protected static Shader ltspt       = Shader.Find("Hidden/ltspass_transparent");
        protected static Shader ltsptesso   = Shader.Find("Hidden/ltspass_tess_opaque");
        protected static Shader ltsptessc   = Shader.Find("Hidden/ltspass_tess_cutout");
        protected static Shader ltsptesst   = Shader.Find("Hidden/ltspass_tess_transparent");

        protected static Shader ltsm        = Shader.Find("_lil/lilToonMulti");
        protected static Shader ltsmo       = Shader.Find("Hidden/lilToonMultiOutline");
        protected static Shader ltsmref     = Shader.Find("Hidden/lilToonMultiRefraction");
        protected static Shader ltsmfur     = Shader.Find("Hidden/lilToonMultiFur");
        protected static Shader ltsmgem     = Shader.Find("Hidden/lilToonMultiGem");

        protected static Shader mtoon       = Shader.Find("VRM/MToon");
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Editor
        #region
        [Serializable]
        public class lilToonEditorSetting
        {
            public EditorMode editorMode = EditorMode.Simple;
            public int languageNum = -1;
            public string languageNames = "";
            public string languageName = "English";
            public int currentVersionValue = 0;
            public bool isShowBase                      = false;
            public bool isShowMainUV                    = false;
            public bool isShowMain                      = false;
            public bool isShowShadow                    = false;
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
            public bool isShowOptimization              = false;
            public bool isShowBlend                     = false;
            public bool isShowBlendAdd                  = false;
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

        public struct lilPropertyBlockData
        {
            public lilPropertyBlock propertyBlock;
            public bool shouldCopyTex;
        }

        private static bool isCustomEditor = false;
        private static bool isMultiVariants = false;
        private static readonly Dictionary<string, string> loc = new Dictionary<string, string>();
        private static lilToonSetting shaderSetting;
        private static lilToonPreset[] presets;
        private static readonly Dictionary<string, MaterialProperty> copiedProperties = new Dictionary<string, MaterialProperty>();
        protected static MaterialEditor m_MaterialEditor;
        protected static GUIStyle boxOuter          = InitializeBox(4, 4, 2);
        protected static GUIStyle boxInnerHalf      = InitializeBox(4, 2, 2);
        protected static GUIStyle boxInner          = InitializeBox(4, 2, 2);
        protected static GUIStyle customBox         = InitializeBox(6, 4, 4);
        protected static GUIStyle customToggleFont  = new GUIStyle();
        protected static GUIStyle wrapLabel         = new GUIStyle();
        protected static GUIStyle boldLabel         = new GUIStyle();
        protected static GUIStyle foldout           = new GUIStyle();
        protected static GUIStyle middleButton      = new GUIStyle(){alignment = TextAnchor.MiddleCenter};
        private readonly Gradient mainGrad  = new Gradient();
        private readonly Gradient emiGrad   = new Gradient();
        private readonly Gradient emi2Grad  = new Gradient();
        public static lilToonEditorSetting edSet = new lilToonEditorSetting();
        private static readonly lilToonVersion latestVersion = new lilToonVersion
        {
            latest_vertion_name = "",
            latest_vertion_value = 0
        };

        protected static string sMainColorBranch;
        protected static string sCullModes;
        protected static string sBlendModes;
        protected static string sAlphaMaskModes;
        protected static string blinkSetting;
        protected static string sDistanceFadeSetting;
        protected static string sDissolveParams;
        protected static string sDissolveParamsMode;
        protected static string sDissolveParamsOther;
        protected static string sGlitterParams1;
        protected static string sGlitterParams2;
        protected static string sTransparentMode;
        protected static string sOutlineVertexColorUsages;
        protected static string sShadowMaskTypes;
        protected static string[] sRenderingModeList;
        protected static string[] sRenderingModeListLite;
        protected static string[] sTransparentModeList;
        protected static GUIContent mainColorRGBAContent;
        protected static GUIContent colorRGBAContent;
        protected static GUIContent colorAlphaRGBAContent;
        protected static GUIContent maskBlendContent;
        protected static GUIContent colorMaskRGBAContent;
        protected static GUIContent alphaMaskContent;
        protected static GUIContent maskStrengthContent;
        protected static GUIContent normalMapContent;
        protected static GUIContent noiseMaskContent;
        protected static GUIContent adjustMaskContent;
        protected static GUIContent matcapContent;
        protected static GUIContent gradationContent;
        protected static GUIContent gradSpeedContent;
        protected static GUIContent smoothnessContent;
        protected static GUIContent metallicContent;
        protected static GUIContent parallaxContent;
        protected static GUIContent customMaskContent;
        protected static GUIContent shadow1stColorRGBAContent;
        protected static GUIContent shadow2ndColorRGBAContent;
        protected static GUIContent shadow3rdColorRGBAContent;
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Material
        #region
        protected static RenderingMode renderingModeBuf;
        protected static TransparentMode transparentModeBuf;
        protected static bool isLite        = false;
        protected static bool isCutout      = false;
        protected static bool isTransparent = false;
        protected static bool isOutl        = false;
        protected static bool isRefr        = false;
        protected static bool isBlur        = false;
        protected static bool isFur         = false;
        protected static bool isStWr        = false;
        protected static bool isTess        = false;
        protected static bool isGem         = false;
        protected static bool isFakeShadow  = false;
        protected static bool isOnePass     = false;
        protected static bool isTwoPass     = false;
        protected static bool isMulti       = false;
        protected static bool isUseAlpha    = false;
        protected static bool isShowRenderMode = true;
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Material properties
        #region
        private lilMaterialProperty transparentModeMat = new lilMaterialProperty();
        private lilMaterialProperty asOverlay = new lilMaterialProperty();
        private lilMaterialProperty invisible = new lilMaterialProperty();
        private lilMaterialProperty asUnlit = new lilMaterialProperty();
        private lilMaterialProperty cutoff = new lilMaterialProperty();
        private lilMaterialProperty subpassCutoff = new lilMaterialProperty();
        private lilMaterialProperty flipNormal = new lilMaterialProperty();
        private lilMaterialProperty shiftBackfaceUV = new lilMaterialProperty();
        private lilMaterialProperty backfaceForceShadow = new lilMaterialProperty();
        private lilMaterialProperty vertexLightStrength = new lilMaterialProperty();
        private lilMaterialProperty lightMinLimit = new lilMaterialProperty();
        private lilMaterialProperty lightMaxLimit = new lilMaterialProperty();
        private lilMaterialProperty beforeExposureLimit = new lilMaterialProperty();
        private lilMaterialProperty monochromeLighting = new lilMaterialProperty();
        private lilMaterialProperty alphaBoostFA = new lilMaterialProperty();
        private lilMaterialProperty lilDirectionalLightStrength = new lilMaterialProperty();
        private lilMaterialProperty lightDirectionOverride = new lilMaterialProperty();
        private lilMaterialProperty baseColor = new lilMaterialProperty();
        private lilMaterialProperty baseMap = new lilMaterialProperty();
        private lilMaterialProperty baseColorMap = new lilMaterialProperty();
        private lilMaterialProperty triMask = new lilMaterialProperty();
            private lilMaterialProperty cull = new lilMaterialProperty();
            private lilMaterialProperty srcBlend = new lilMaterialProperty();
            private lilMaterialProperty dstBlend = new lilMaterialProperty();
            private lilMaterialProperty srcBlendAlpha = new lilMaterialProperty();
            private lilMaterialProperty dstBlendAlpha = new lilMaterialProperty();
            private lilMaterialProperty blendOp = new lilMaterialProperty();
            private lilMaterialProperty blendOpAlpha = new lilMaterialProperty();
            private lilMaterialProperty srcBlendFA = new lilMaterialProperty();
            private lilMaterialProperty dstBlendFA = new lilMaterialProperty();
            private lilMaterialProperty srcBlendAlphaFA = new lilMaterialProperty();
            private lilMaterialProperty dstBlendAlphaFA = new lilMaterialProperty();
            private lilMaterialProperty blendOpFA = new lilMaterialProperty();
            private lilMaterialProperty blendOpAlphaFA = new lilMaterialProperty();
            private lilMaterialProperty zclip = new lilMaterialProperty();
            private lilMaterialProperty zwrite = new lilMaterialProperty();
            private lilMaterialProperty ztest = new lilMaterialProperty();
            private lilMaterialProperty stencilRef = new lilMaterialProperty();
            private lilMaterialProperty stencilReadMask = new lilMaterialProperty();
            private lilMaterialProperty stencilWriteMask = new lilMaterialProperty();
            private lilMaterialProperty stencilComp = new lilMaterialProperty();
            private lilMaterialProperty stencilPass = new lilMaterialProperty();
            private lilMaterialProperty stencilFail = new lilMaterialProperty();
            private lilMaterialProperty stencilZFail = new lilMaterialProperty();
            private lilMaterialProperty offsetFactor = new lilMaterialProperty();
            private lilMaterialProperty offsetUnits = new lilMaterialProperty();
            private lilMaterialProperty colorMask = new lilMaterialProperty();
            private lilMaterialProperty alphaToMask = new lilMaterialProperty();
            private lilMaterialProperty lilShadowCasterBias = new lilMaterialProperty();
        //private lilMaterialProperty useMainTex = new lilMaterialProperty();
            private lilMaterialProperty mainColor = new lilMaterialProperty();
            private lilMaterialProperty mainTex = new lilMaterialProperty();
            private lilMaterialProperty mainTexHSVG = new lilMaterialProperty();
            private lilMaterialProperty mainTex_ScrollRotate = new lilMaterialProperty();
            private lilMaterialProperty mainGradationStrength = new lilMaterialProperty();
            private lilMaterialProperty mainGradationTex = new lilMaterialProperty();
            private lilMaterialProperty mainColorAdjustMask = new lilMaterialProperty();
        private lilMaterialProperty useMain2ndTex = new lilMaterialProperty();
            private lilMaterialProperty mainColor2nd = new lilMaterialProperty();
            private lilMaterialProperty main2ndTex = new lilMaterialProperty();
            private lilMaterialProperty main2ndTex_UVMode = new lilMaterialProperty();
            private lilMaterialProperty main2ndTexAngle = new lilMaterialProperty();
            private lilMaterialProperty main2ndTexDecalAnimation = new lilMaterialProperty();
            private lilMaterialProperty main2ndTexDecalSubParam = new lilMaterialProperty();
            private lilMaterialProperty main2ndTexIsDecal = new lilMaterialProperty();
            private lilMaterialProperty main2ndTexIsLeftOnly = new lilMaterialProperty();
            private lilMaterialProperty main2ndTexIsRightOnly = new lilMaterialProperty();
            private lilMaterialProperty main2ndTexShouldCopy = new lilMaterialProperty();
            private lilMaterialProperty main2ndTexShouldFlipMirror = new lilMaterialProperty();
            private lilMaterialProperty main2ndTexShouldFlipCopy = new lilMaterialProperty();
            private lilMaterialProperty main2ndTexIsMSDF = new lilMaterialProperty();
            private lilMaterialProperty main2ndBlendMask = new lilMaterialProperty();
            private lilMaterialProperty main2ndTexBlendMode = new lilMaterialProperty();
            private lilMaterialProperty main2ndEnableLighting = new lilMaterialProperty();
            private lilMaterialProperty main2ndDissolveMask = new lilMaterialProperty();
            private lilMaterialProperty main2ndDissolveNoiseMask = new lilMaterialProperty();
            private lilMaterialProperty main2ndDissolveNoiseMask_ScrollRotate = new lilMaterialProperty();
            private lilMaterialProperty main2ndDissolveNoiseStrength = new lilMaterialProperty();
            private lilMaterialProperty main2ndDissolveColor = new lilMaterialProperty();
            private lilMaterialProperty main2ndDissolveParams = new lilMaterialProperty();
            private lilMaterialProperty main2ndDissolvePos = new lilMaterialProperty();
            private lilMaterialProperty main2ndDistanceFade = new lilMaterialProperty();
        private lilMaterialProperty useMain3rdTex = new lilMaterialProperty();
            private lilMaterialProperty mainColor3rd = new lilMaterialProperty();
            private lilMaterialProperty main3rdTex = new lilMaterialProperty();
            private lilMaterialProperty main3rdTex_UVMode = new lilMaterialProperty();
            private lilMaterialProperty main3rdTexAngle = new lilMaterialProperty();
            private lilMaterialProperty main3rdTexDecalAnimation = new lilMaterialProperty();
            private lilMaterialProperty main3rdTexDecalSubParam = new lilMaterialProperty();
            private lilMaterialProperty main3rdTexIsDecal = new lilMaterialProperty();
            private lilMaterialProperty main3rdTexIsLeftOnly = new lilMaterialProperty();
            private lilMaterialProperty main3rdTexIsRightOnly = new lilMaterialProperty();
            private lilMaterialProperty main3rdTexShouldCopy = new lilMaterialProperty();
            private lilMaterialProperty main3rdTexShouldFlipMirror = new lilMaterialProperty();
            private lilMaterialProperty main3rdTexShouldFlipCopy = new lilMaterialProperty();
            private lilMaterialProperty main3rdTexIsMSDF = new lilMaterialProperty();
            private lilMaterialProperty main3rdBlendMask = new lilMaterialProperty();
            private lilMaterialProperty main3rdTexBlendMode = new lilMaterialProperty();
            private lilMaterialProperty main3rdEnableLighting = new lilMaterialProperty();
            private lilMaterialProperty main3rdDissolveMask = new lilMaterialProperty();
            private lilMaterialProperty main3rdDissolveNoiseMask = new lilMaterialProperty();
            private lilMaterialProperty main3rdDissolveNoiseMask_ScrollRotate = new lilMaterialProperty();
            private lilMaterialProperty main3rdDissolveNoiseStrength = new lilMaterialProperty();
            private lilMaterialProperty main3rdDissolveColor = new lilMaterialProperty();
            private lilMaterialProperty main3rdDissolveParams = new lilMaterialProperty();
            private lilMaterialProperty main3rdDissolvePos = new lilMaterialProperty();
            private lilMaterialProperty main3rdDistanceFade = new lilMaterialProperty();
        private lilMaterialProperty alphaMaskMode = new lilMaterialProperty();
            private lilMaterialProperty alphaMask = new lilMaterialProperty();
            private lilMaterialProperty alphaMaskScale = new lilMaterialProperty();
            private lilMaterialProperty alphaMaskValue = new lilMaterialProperty();
        private lilMaterialProperty useShadow = new lilMaterialProperty();
            private lilMaterialProperty shadowStrength = new lilMaterialProperty();
            private lilMaterialProperty shadowStrengthMask = new lilMaterialProperty();
            private lilMaterialProperty shadowBorderMask = new lilMaterialProperty();
            private lilMaterialProperty shadowBlurMask = new lilMaterialProperty();
            private lilMaterialProperty shadowStrengthMaskLOD = new lilMaterialProperty();
            private lilMaterialProperty shadowBorderMaskLOD = new lilMaterialProperty();
            private lilMaterialProperty shadowBlurMaskLOD = new lilMaterialProperty();
            private lilMaterialProperty shadowAOShift = new lilMaterialProperty();
            private lilMaterialProperty shadowAOShift2 = new lilMaterialProperty();
            private lilMaterialProperty shadowPostAO = new lilMaterialProperty();
            private lilMaterialProperty shadowColor = new lilMaterialProperty();
            private lilMaterialProperty shadowColorTex = new lilMaterialProperty();
            private lilMaterialProperty shadowNormalStrength = new lilMaterialProperty();
            private lilMaterialProperty shadowBorder = new lilMaterialProperty();
            private lilMaterialProperty shadowBlur = new lilMaterialProperty();
            private lilMaterialProperty shadow2ndColor = new lilMaterialProperty();
            private lilMaterialProperty shadow2ndColorTex = new lilMaterialProperty();
            private lilMaterialProperty shadow2ndNormalStrength = new lilMaterialProperty();
            private lilMaterialProperty shadow2ndBorder = new lilMaterialProperty();
            private lilMaterialProperty shadow2ndBlur = new lilMaterialProperty();
            private lilMaterialProperty shadow3rdColor = new lilMaterialProperty();
            private lilMaterialProperty shadow3rdColorTex = new lilMaterialProperty();
            private lilMaterialProperty shadow3rdNormalStrength = new lilMaterialProperty();
            private lilMaterialProperty shadow3rdBorder = new lilMaterialProperty();
            private lilMaterialProperty shadow3rdBlur = new lilMaterialProperty();
            private lilMaterialProperty shadowMainStrength = new lilMaterialProperty();
            private lilMaterialProperty shadowEnvStrength = new lilMaterialProperty();
            private lilMaterialProperty shadowBorderColor = new lilMaterialProperty();
            private lilMaterialProperty shadowBorderRange = new lilMaterialProperty();
            private lilMaterialProperty shadowReceive = new lilMaterialProperty();
            private lilMaterialProperty shadow2ndReceive = new lilMaterialProperty();
            private lilMaterialProperty shadow3rdReceive = new lilMaterialProperty();
            private lilMaterialProperty shadowMaskType = new lilMaterialProperty();
            private lilMaterialProperty shadowFlatBorder = new lilMaterialProperty();
            private lilMaterialProperty shadowFlatBlur = new lilMaterialProperty();
        private lilMaterialProperty useBacklight = new lilMaterialProperty();
            private lilMaterialProperty backlightColor = new lilMaterialProperty();
            private lilMaterialProperty backlightColorTex = new lilMaterialProperty();
            private lilMaterialProperty backlightMainStrength = new lilMaterialProperty();
            private lilMaterialProperty backlightNormalStrength = new lilMaterialProperty();
            private lilMaterialProperty backlightBorder = new lilMaterialProperty();
            private lilMaterialProperty backlightBlur = new lilMaterialProperty();
            private lilMaterialProperty backlightDirectivity = new lilMaterialProperty();
            private lilMaterialProperty backlightViewStrength = new lilMaterialProperty();
            private lilMaterialProperty backlightReceiveShadow = new lilMaterialProperty();
            private lilMaterialProperty backlightBackfaceMask = new lilMaterialProperty();
        private lilMaterialProperty useBumpMap = new lilMaterialProperty();
            private lilMaterialProperty bumpMap = new lilMaterialProperty();
            private lilMaterialProperty bumpScale = new lilMaterialProperty();
        private lilMaterialProperty useBump2ndMap = new lilMaterialProperty();
            private lilMaterialProperty bump2ndMap = new lilMaterialProperty();
            private lilMaterialProperty bump2ndScale = new lilMaterialProperty();
            private lilMaterialProperty bump2ndScaleMask = new lilMaterialProperty();
        private lilMaterialProperty useAnisotropy = new lilMaterialProperty();
            private lilMaterialProperty anisotropyTangentMap = new lilMaterialProperty();
            private lilMaterialProperty anisotropyScale = new lilMaterialProperty();
            private lilMaterialProperty anisotropyScaleMask = new lilMaterialProperty();
            private lilMaterialProperty anisotropyTangentWidth = new lilMaterialProperty();
            private lilMaterialProperty anisotropyBitangentWidth = new lilMaterialProperty();
            private lilMaterialProperty anisotropyShift = new lilMaterialProperty();
            private lilMaterialProperty anisotropyShiftNoiseScale = new lilMaterialProperty();
            private lilMaterialProperty anisotropySpecularStrength = new lilMaterialProperty();
            private lilMaterialProperty anisotropy2ndTangentWidth = new lilMaterialProperty();
            private lilMaterialProperty anisotropy2ndBitangentWidth = new lilMaterialProperty();
            private lilMaterialProperty anisotropy2ndShift = new lilMaterialProperty();
            private lilMaterialProperty anisotropy2ndShiftNoiseScale = new lilMaterialProperty();
            private lilMaterialProperty anisotropy2ndSpecularStrength = new lilMaterialProperty();
            private lilMaterialProperty anisotropyShiftNoiseMask = new lilMaterialProperty();
            private lilMaterialProperty anisotropy2Reflection = new lilMaterialProperty();
            private lilMaterialProperty anisotropy2MatCap = new lilMaterialProperty();
            private lilMaterialProperty anisotropy2MatCap2nd = new lilMaterialProperty();
        private lilMaterialProperty useReflection = new lilMaterialProperty();
            private lilMaterialProperty metallic = new lilMaterialProperty();
            private lilMaterialProperty metallicGlossMap = new lilMaterialProperty();
            private lilMaterialProperty smoothness = new lilMaterialProperty();
            private lilMaterialProperty smoothnessTex = new lilMaterialProperty();
            private lilMaterialProperty reflectance = new lilMaterialProperty();
            private lilMaterialProperty reflectionColor = new lilMaterialProperty();
            private lilMaterialProperty reflectionColorTex = new lilMaterialProperty();
            private lilMaterialProperty gsaaStrength = new lilMaterialProperty();
            private lilMaterialProperty applySpecular = new lilMaterialProperty();
            private lilMaterialProperty applySpecularFA = new lilMaterialProperty();
            private lilMaterialProperty specularNormalStrength = new lilMaterialProperty();
            private lilMaterialProperty specularToon = new lilMaterialProperty();
            private lilMaterialProperty specularBorder = new lilMaterialProperty();
            private lilMaterialProperty specularBlur = new lilMaterialProperty();
            private lilMaterialProperty applyReflection = new lilMaterialProperty();
            private lilMaterialProperty reflectionNormalStrength = new lilMaterialProperty();
            private lilMaterialProperty reflectionApplyTransparency = new lilMaterialProperty();
            private lilMaterialProperty reflectionCubeTex = new lilMaterialProperty();
            private lilMaterialProperty reflectionCubeColor = new lilMaterialProperty();
            private lilMaterialProperty reflectionCubeOverride = new lilMaterialProperty();
            private lilMaterialProperty reflectionCubeEnableLighting = new lilMaterialProperty();
            private lilMaterialProperty reflectionBlendMode = new lilMaterialProperty();
        private lilMaterialProperty useMatCap = new lilMaterialProperty();
            private lilMaterialProperty matcapTex = new lilMaterialProperty();
            private lilMaterialProperty matcapColor = new lilMaterialProperty();
            private lilMaterialProperty matcapMainStrength = new lilMaterialProperty();
            private lilMaterialProperty matcapBlendUV1 = new lilMaterialProperty();
            private lilMaterialProperty matcapZRotCancel = new lilMaterialProperty();
            private lilMaterialProperty matcapPerspective = new lilMaterialProperty();
            private lilMaterialProperty matcapVRParallaxStrength = new lilMaterialProperty();
            private lilMaterialProperty matcapBlend = new lilMaterialProperty();
            private lilMaterialProperty matcapBlendMask = new lilMaterialProperty();
            private lilMaterialProperty matcapEnableLighting = new lilMaterialProperty();
            private lilMaterialProperty matcapShadowMask = new lilMaterialProperty();
            private lilMaterialProperty matcapBackfaceMask = new lilMaterialProperty();
            private lilMaterialProperty matcapLod = new lilMaterialProperty();
            private lilMaterialProperty matcapBlendMode = new lilMaterialProperty();
            private lilMaterialProperty matcapMul = new lilMaterialProperty();
            private lilMaterialProperty matcapApplyTransparency = new lilMaterialProperty();
            private lilMaterialProperty matcapNormalStrength = new lilMaterialProperty();
            private lilMaterialProperty matcapCustomNormal = new lilMaterialProperty();
            private lilMaterialProperty matcapBumpMap = new lilMaterialProperty();
            private lilMaterialProperty matcapBumpScale = new lilMaterialProperty();
        private lilMaterialProperty useMatCap2nd = new lilMaterialProperty();
            private lilMaterialProperty matcap2ndTex = new lilMaterialProperty();
            private lilMaterialProperty matcap2ndColor = new lilMaterialProperty();
            private lilMaterialProperty matcap2ndMainStrength = new lilMaterialProperty();
            private lilMaterialProperty matcap2ndBlendUV1 = new lilMaterialProperty();
            private lilMaterialProperty matcap2ndZRotCancel = new lilMaterialProperty();
            private lilMaterialProperty matcap2ndPerspective = new lilMaterialProperty();
            private lilMaterialProperty matcap2ndVRParallaxStrength = new lilMaterialProperty();
            private lilMaterialProperty matcap2ndBlend = new lilMaterialProperty();
            private lilMaterialProperty matcap2ndBlendMask = new lilMaterialProperty();
            private lilMaterialProperty matcap2ndEnableLighting = new lilMaterialProperty();
            private lilMaterialProperty matcap2ndShadowMask = new lilMaterialProperty();
            private lilMaterialProperty matcap2ndBackfaceMask = new lilMaterialProperty();
            private lilMaterialProperty matcap2ndLod = new lilMaterialProperty();
            private lilMaterialProperty matcap2ndBlendMode = new lilMaterialProperty();
            private lilMaterialProperty matcap2ndMul = new lilMaterialProperty();
            private lilMaterialProperty matcap2ndApplyTransparency = new lilMaterialProperty();
            private lilMaterialProperty matcap2ndNormalStrength = new lilMaterialProperty();
            private lilMaterialProperty matcap2ndCustomNormal = new lilMaterialProperty();
            private lilMaterialProperty matcap2ndBumpMap = new lilMaterialProperty();
            private lilMaterialProperty matcap2ndBumpScale = new lilMaterialProperty();
        private lilMaterialProperty useRim = new lilMaterialProperty();
            private lilMaterialProperty rimColor = new lilMaterialProperty();
            private lilMaterialProperty rimColorTex = new lilMaterialProperty();
            private lilMaterialProperty rimMainStrength = new lilMaterialProperty();
            private lilMaterialProperty rimNormalStrength = new lilMaterialProperty();
            private lilMaterialProperty rimBorder = new lilMaterialProperty();
            private lilMaterialProperty rimBlur = new lilMaterialProperty();
            private lilMaterialProperty rimFresnelPower = new lilMaterialProperty();
            private lilMaterialProperty rimEnableLighting = new lilMaterialProperty();
            private lilMaterialProperty rimShadowMask = new lilMaterialProperty();
            private lilMaterialProperty rimBackfaceMask = new lilMaterialProperty();
            private lilMaterialProperty rimVRParallaxStrength = new lilMaterialProperty();
            private lilMaterialProperty rimApplyTransparency = new lilMaterialProperty();
            private lilMaterialProperty rimDirStrength = new lilMaterialProperty();
            private lilMaterialProperty rimDirRange = new lilMaterialProperty();
            private lilMaterialProperty rimIndirRange = new lilMaterialProperty();
            private lilMaterialProperty rimIndirColor = new lilMaterialProperty();
            private lilMaterialProperty rimIndirBorder = new lilMaterialProperty();
            private lilMaterialProperty rimIndirBlur = new lilMaterialProperty();
        private lilMaterialProperty useGlitter = new lilMaterialProperty();
            private lilMaterialProperty glitterUVMode = new lilMaterialProperty();
            private lilMaterialProperty glitterColor = new lilMaterialProperty();
            private lilMaterialProperty glitterColorTex = new lilMaterialProperty();
            private lilMaterialProperty glitterMainStrength = new lilMaterialProperty();
            private lilMaterialProperty glitterParams1 = new lilMaterialProperty();
            private lilMaterialProperty glitterParams2 = new lilMaterialProperty();
            private lilMaterialProperty glitterPostContrast = new lilMaterialProperty();
            private lilMaterialProperty glitterSensitivity = new lilMaterialProperty();
            private lilMaterialProperty glitterEnableLighting = new lilMaterialProperty();
            private lilMaterialProperty glitterShadowMask = new lilMaterialProperty();
            private lilMaterialProperty glitterBackfaceMask = new lilMaterialProperty();
            private lilMaterialProperty glitterApplyTransparency = new lilMaterialProperty();
            private lilMaterialProperty glitterVRParallaxStrength = new lilMaterialProperty();
            private lilMaterialProperty glitterNormalStrength = new lilMaterialProperty();
        private lilMaterialProperty useEmission = new lilMaterialProperty();
            private lilMaterialProperty emissionColor = new lilMaterialProperty();
            private lilMaterialProperty emissionMap = new lilMaterialProperty();
            private lilMaterialProperty emissionMap_ScrollRotate = new lilMaterialProperty();
            private lilMaterialProperty emissionMap_UVMode = new lilMaterialProperty();
            private lilMaterialProperty emissionMainStrength = new lilMaterialProperty();
            private lilMaterialProperty emissionBlend = new lilMaterialProperty();
            private lilMaterialProperty emissionBlendMask = new lilMaterialProperty();
            private lilMaterialProperty emissionBlendMask_ScrollRotate = new lilMaterialProperty();
            private lilMaterialProperty emissionBlink = new lilMaterialProperty();
            private lilMaterialProperty emissionUseGrad = new lilMaterialProperty();
            private lilMaterialProperty emissionGradTex = new lilMaterialProperty();
            private lilMaterialProperty emissionGradSpeed = new lilMaterialProperty();
            private lilMaterialProperty emissionParallaxDepth = new lilMaterialProperty();
            private lilMaterialProperty emissionFluorescence = new lilMaterialProperty();
        private lilMaterialProperty useEmission2nd = new lilMaterialProperty();
            private lilMaterialProperty emission2ndColor = new lilMaterialProperty();
            private lilMaterialProperty emission2ndMap = new lilMaterialProperty();
            private lilMaterialProperty emission2ndMap_ScrollRotate = new lilMaterialProperty();
            private lilMaterialProperty emission2ndMap_UVMode = new lilMaterialProperty();
            private lilMaterialProperty emission2ndMainStrength = new lilMaterialProperty();
            private lilMaterialProperty emission2ndBlend = new lilMaterialProperty();
            private lilMaterialProperty emission2ndBlendMask = new lilMaterialProperty();
            private lilMaterialProperty emission2ndBlendMask_ScrollRotate = new lilMaterialProperty();
            private lilMaterialProperty emission2ndBlink = new lilMaterialProperty();
            private lilMaterialProperty emission2ndUseGrad = new lilMaterialProperty();
            private lilMaterialProperty emission2ndGradTex = new lilMaterialProperty();
            private lilMaterialProperty emission2ndGradSpeed = new lilMaterialProperty();
            private lilMaterialProperty emission2ndParallaxDepth = new lilMaterialProperty();
            private lilMaterialProperty emission2ndFluorescence = new lilMaterialProperty();
        //private lilMaterialProperty useOutline = new lilMaterialProperty();
            private lilMaterialProperty outlineColor = new lilMaterialProperty();
            private lilMaterialProperty outlineTex = new lilMaterialProperty();
            private lilMaterialProperty outlineTex_ScrollRotate = new lilMaterialProperty();
            private lilMaterialProperty outlineTexHSVG = new lilMaterialProperty();
            private lilMaterialProperty outlineLitColor = new lilMaterialProperty();
            private lilMaterialProperty outlineLitApplyTex = new lilMaterialProperty();
            private lilMaterialProperty outlineLitScale = new lilMaterialProperty();
            private lilMaterialProperty outlineLitOffset = new lilMaterialProperty();
            private lilMaterialProperty outlineWidth = new lilMaterialProperty();
            private lilMaterialProperty outlineWidthMask = new lilMaterialProperty();
            private lilMaterialProperty outlineFixWidth = new lilMaterialProperty();
            private lilMaterialProperty outlineVertexR2Width = new lilMaterialProperty();
            private lilMaterialProperty outlineDeleteMesh = new lilMaterialProperty();
            private lilMaterialProperty outlineVectorTex = new lilMaterialProperty();
            private lilMaterialProperty outlineVectorUVMode = new lilMaterialProperty();
            private lilMaterialProperty outlineVectorScale = new lilMaterialProperty();
            private lilMaterialProperty outlineEnableLighting = new lilMaterialProperty();
            private lilMaterialProperty outlineZBias = new lilMaterialProperty();
            private lilMaterialProperty outlineCull = new lilMaterialProperty();
            private lilMaterialProperty outlineSrcBlend = new lilMaterialProperty();
            private lilMaterialProperty outlineDstBlend = new lilMaterialProperty();
            private lilMaterialProperty outlineSrcBlendAlpha = new lilMaterialProperty();
            private lilMaterialProperty outlineDstBlendAlpha = new lilMaterialProperty();
            private lilMaterialProperty outlineBlendOp = new lilMaterialProperty();
            private lilMaterialProperty outlineBlendOpAlpha = new lilMaterialProperty();
            private lilMaterialProperty outlineSrcBlendFA = new lilMaterialProperty();
            private lilMaterialProperty outlineDstBlendFA = new lilMaterialProperty();
            private lilMaterialProperty outlineSrcBlendAlphaFA = new lilMaterialProperty();
            private lilMaterialProperty outlineDstBlendAlphaFA = new lilMaterialProperty();
            private lilMaterialProperty outlineBlendOpFA = new lilMaterialProperty();
            private lilMaterialProperty outlineBlendOpAlphaFA = new lilMaterialProperty();
            private lilMaterialProperty outlineZclip = new lilMaterialProperty();
            private lilMaterialProperty outlineZwrite = new lilMaterialProperty();
            private lilMaterialProperty outlineZtest = new lilMaterialProperty();
            private lilMaterialProperty outlineStencilRef = new lilMaterialProperty();
            private lilMaterialProperty outlineStencilReadMask = new lilMaterialProperty();
            private lilMaterialProperty outlineStencilWriteMask = new lilMaterialProperty();
            private lilMaterialProperty outlineStencilComp = new lilMaterialProperty();
            private lilMaterialProperty outlineStencilPass = new lilMaterialProperty();
            private lilMaterialProperty outlineStencilFail = new lilMaterialProperty();
            private lilMaterialProperty outlineStencilZFail = new lilMaterialProperty();
            private lilMaterialProperty outlineOffsetFactor = new lilMaterialProperty();
            private lilMaterialProperty outlineOffsetUnits = new lilMaterialProperty();
            private lilMaterialProperty outlineColorMask = new lilMaterialProperty();
            private lilMaterialProperty outlineAlphaToMask = new lilMaterialProperty();
        private lilMaterialProperty useParallax = new lilMaterialProperty();
            private lilMaterialProperty usePOM = new lilMaterialProperty();
            private lilMaterialProperty parallaxMap = new lilMaterialProperty();
            private lilMaterialProperty parallax = new lilMaterialProperty();
            private lilMaterialProperty parallaxOffset = new lilMaterialProperty();
        //private lilMaterialProperty useDistanceFade = new lilMaterialProperty();
            private lilMaterialProperty distanceFadeColor = new lilMaterialProperty();
            private lilMaterialProperty distanceFade = new lilMaterialProperty();
        private lilMaterialProperty useClippingCanceller = new lilMaterialProperty();
        private lilMaterialProperty useAudioLink = new lilMaterialProperty();
            private lilMaterialProperty audioLinkDefaultValue = new lilMaterialProperty();
            private lilMaterialProperty audioLinkUVMode = new lilMaterialProperty();
            private lilMaterialProperty audioLinkUVParams = new lilMaterialProperty();
            private lilMaterialProperty audioLinkStart = new lilMaterialProperty();
            private lilMaterialProperty audioLinkMask = new lilMaterialProperty();
            private lilMaterialProperty audioLink2Main2nd = new lilMaterialProperty();
            private lilMaterialProperty audioLink2Main3rd = new lilMaterialProperty();
            private lilMaterialProperty audioLink2Emission = new lilMaterialProperty();
            private lilMaterialProperty audioLink2EmissionGrad = new lilMaterialProperty();
            private lilMaterialProperty audioLink2Emission2nd = new lilMaterialProperty();
            private lilMaterialProperty audioLink2Emission2ndGrad = new lilMaterialProperty();
            private lilMaterialProperty audioLink2Vertex = new lilMaterialProperty();
            private lilMaterialProperty audioLinkVertexUVMode = new lilMaterialProperty();
            private lilMaterialProperty audioLinkVertexUVParams = new lilMaterialProperty();
            private lilMaterialProperty audioLinkVertexStart = new lilMaterialProperty();
            private lilMaterialProperty audioLinkVertexStrength = new lilMaterialProperty();
            private lilMaterialProperty audioLinkAsLocal = new lilMaterialProperty();
            private lilMaterialProperty audioLinkLocalMap = new lilMaterialProperty();
            private lilMaterialProperty audioLinkLocalMapParams = new lilMaterialProperty();
        //private lilMaterialProperty useDissolve = new lilMaterialProperty();
            private lilMaterialProperty dissolveMask = new lilMaterialProperty();
            private lilMaterialProperty dissolveNoiseMask = new lilMaterialProperty();
            private lilMaterialProperty dissolveNoiseMask_ScrollRotate = new lilMaterialProperty();
            private lilMaterialProperty dissolveNoiseStrength = new lilMaterialProperty();
            private lilMaterialProperty dissolveColor = new lilMaterialProperty();
            private lilMaterialProperty dissolveParams = new lilMaterialProperty();
            private lilMaterialProperty dissolvePos = new lilMaterialProperty();
        //private lilMaterialProperty useEncryptio = new lilMaterialProperty();
            private lilMaterialProperty ignoreEncryption = new lilMaterialProperty();
            private lilMaterialProperty keys = new lilMaterialProperty();
        //private lilMaterialProperty useRefraction = new lilMaterialProperty();
            private lilMaterialProperty refractionStrength = new lilMaterialProperty();
            private lilMaterialProperty refractionFresnelPower = new lilMaterialProperty();
            private lilMaterialProperty refractionColorFromMain = new lilMaterialProperty();
            private lilMaterialProperty refractionColor = new lilMaterialProperty();
        //private lilMaterialProperty useFur = new lilMaterialProperty();
            private lilMaterialProperty furNoiseMask = new lilMaterialProperty();
            private lilMaterialProperty furMask = new lilMaterialProperty();
            private lilMaterialProperty furLengthMask = new lilMaterialProperty();
            private lilMaterialProperty furVectorTex = new lilMaterialProperty();
            private lilMaterialProperty furVectorScale = new lilMaterialProperty();
            private lilMaterialProperty furVector = new lilMaterialProperty();
            private lilMaterialProperty furGravity = new lilMaterialProperty();
            private lilMaterialProperty furRandomize = new lilMaterialProperty();
            private lilMaterialProperty furAO = new lilMaterialProperty();
            private lilMaterialProperty vertexColor2FurVector = new lilMaterialProperty();
            private lilMaterialProperty furMeshType = new lilMaterialProperty();
            private lilMaterialProperty furLayerNum = new lilMaterialProperty();
            private lilMaterialProperty furRootOffset = new lilMaterialProperty();
            private lilMaterialProperty furCutoutLength = new lilMaterialProperty();
            private lilMaterialProperty furTouchStrength = new lilMaterialProperty();
            private lilMaterialProperty furCull = new lilMaterialProperty();
            private lilMaterialProperty furSrcBlend = new lilMaterialProperty();
            private lilMaterialProperty furDstBlend = new lilMaterialProperty();
            private lilMaterialProperty furSrcBlendAlpha = new lilMaterialProperty();
            private lilMaterialProperty furDstBlendAlpha = new lilMaterialProperty();
            private lilMaterialProperty furBlendOp = new lilMaterialProperty();
            private lilMaterialProperty furBlendOpAlpha = new lilMaterialProperty();
            private lilMaterialProperty furSrcBlendFA = new lilMaterialProperty();
            private lilMaterialProperty furDstBlendFA = new lilMaterialProperty();
            private lilMaterialProperty furSrcBlendAlphaFA = new lilMaterialProperty();
            private lilMaterialProperty furDstBlendAlphaFA = new lilMaterialProperty();
            private lilMaterialProperty furBlendOpFA = new lilMaterialProperty();
            private lilMaterialProperty furBlendOpAlphaFA = new lilMaterialProperty();
            private lilMaterialProperty furZclip = new lilMaterialProperty();
            private lilMaterialProperty furZwrite = new lilMaterialProperty();
            private lilMaterialProperty furZtest = new lilMaterialProperty();
            private lilMaterialProperty furStencilRef = new lilMaterialProperty();
            private lilMaterialProperty furStencilReadMask = new lilMaterialProperty();
            private lilMaterialProperty furStencilWriteMask = new lilMaterialProperty();
            private lilMaterialProperty furStencilComp = new lilMaterialProperty();
            private lilMaterialProperty furStencilPass = new lilMaterialProperty();
            private lilMaterialProperty furStencilFail = new lilMaterialProperty();
            private lilMaterialProperty furStencilZFail = new lilMaterialProperty();
            private lilMaterialProperty furOffsetFactor = new lilMaterialProperty();
            private lilMaterialProperty furOffsetUnits = new lilMaterialProperty();
            private lilMaterialProperty furColorMask = new lilMaterialProperty();
            private lilMaterialProperty furAlphaToMask = new lilMaterialProperty();
        //private lilMaterialProperty useTessellation = new lilMaterialProperty();
            private lilMaterialProperty tessEdge = new lilMaterialProperty();
            private lilMaterialProperty tessStrength = new lilMaterialProperty();
            private lilMaterialProperty tessShrink = new lilMaterialProperty();
            private lilMaterialProperty tessFactorMax = new lilMaterialProperty();
        //private lilMaterialProperty useGem = new lilMaterialProperty();
            private lilMaterialProperty gemChromaticAberration = new lilMaterialProperty();
            private lilMaterialProperty gemEnvContrast = new lilMaterialProperty();
            private lilMaterialProperty gemEnvColor = new lilMaterialProperty();
            private lilMaterialProperty gemParticleLoop = new lilMaterialProperty();
            private lilMaterialProperty gemParticleColor = new lilMaterialProperty();
            private lilMaterialProperty gemVRParallaxStrength = new lilMaterialProperty();
        //private lilMaterialProperty useFakeShadow = new lilMaterialProperty();
            private lilMaterialProperty fakeShadowVector = new lilMaterialProperty();
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // GUI
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
            InitializeGUIStyles();

            //------------------------------------------------------------------------------------------------------------------------------
            // Initialize Setting
            m_MaterialEditor = materialEditor;
            ApplyEditorSettingTemp();
            InitializeShaders();
            InitializeShaderSetting(ref shaderSetting);

            //------------------------------------------------------------------------------------------------------------------------------
            // Load Properties
            LoadProperties(props);

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
            SelectLang();
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
                material.SetFloat("_lilToonVersion", currentVersionValue);
                if(!isMultiVariants)
                {
                    if(isMulti) SetupMultiMaterial(material);
                    else        RemoveShaderKeywords(material);
                }
                CopyMainColorProperties();
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

            edSet.isShowMain = Foldout(GetLoc("sMainColorSettingSimple"), edSet.isShowMain);
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
                        ToneCorrectionGUI(mainTexHSVG, 1);
                        DrawLine();
                        EditorGUILayout.LabelField(GetLoc("sGradationMap"), boldLabel);
                        m_MaterialEditor.ShaderProperty(mainGradationStrength, GetLoc("sStrength"));
                        if(mainGradationStrength.floatValue != 0)
                        {
                            m_MaterialEditor.TexturePropertySingleLine(gradationContent, mainGradationTex);
                            GradientEditor(material, mainGrad, mainGradationTex, true);
                        }
                        DrawLine();
                        m_MaterialEditor.TexturePropertySingleLine(adjustMaskContent, mainColorAdjustMask);
                        TextureBakeGUI(material, 4);
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
                        DrawColorAsAlpha(mainColor2nd);
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
                        DrawColorAsAlpha(mainColor3rd);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }
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
                edSet.isShowEmission = Foldout(GetLoc("sEmissionSetting"), edSet.isShowEmission);
                DrawMenuButton(GetLoc("sAnchorEmission"), lilPropertyBlock.Emission);
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
                edSet.isShowEmission = Foldout(GetLoc("sEmissionSetting"), edSet.isShowEmission);
                DrawMenuButton(GetLoc("sAnchorEmission"), lilPropertyBlock.Emission);
                if(edSet.isShowEmission)
                {
                    // Emission
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useEmission, GetLoc("sEmission"));
                    DrawMenuButton(GetLoc("sAnchorEmission"), lilPropertyBlock.Emission1st);
                    if(useEmission.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        TextureGUI(ref edSet.isShowEmissionMap, colorMaskRGBAContent, emissionMap, emissionColor, emissionMap_ScrollRotate, emissionMap_UVMode, true, true);
                        DrawColorAsAlpha(emissionColor);
                        m_MaterialEditor.ShaderProperty(emissionMainStrength, GetLoc("sMainColorPower"));
                        DrawLine();
                        TextureGUI(ref edSet.isShowEmissionBlendMask, maskBlendContent, emissionBlendMask, emissionBlend, emissionBlendMask_ScrollRotate, true, true);
                        DrawLine();
                        m_MaterialEditor.ShaderProperty(emissionFluorescence, GetLoc("sFluorescence"));
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();

                    // Emission 2nd
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useEmission2nd, GetLoc("sEmission2nd"));
                    DrawMenuButton(GetLoc("sAnchorEmission"), lilPropertyBlock.Emission2nd);
                    if(useEmission2nd.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        TextureGUI(ref edSet.isShowEmission2ndMap, colorMaskRGBAContent, emission2ndMap, emission2ndColor, emission2ndMap_ScrollRotate, emission2ndMap_UVMode, true, true);
                        DrawColorAsAlpha(emission2ndColor);
                        m_MaterialEditor.ShaderProperty(emission2ndMainStrength, GetLoc("sMainColorPower"));
                        DrawLine();
                        TextureGUI(ref edSet.isShowEmission2ndBlendMask, maskBlendContent, emission2ndBlendMask, emission2ndBlend, emission2ndBlendMask_ScrollRotate, true, true);
                        DrawLine();
                        m_MaterialEditor.ShaderProperty(emission2ndFluorescence, GetLoc("sFluorescence"));
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Outline
            DrawOutlineSettingsSimple(material);

            if(mtoon != null && EditorButton(GetLoc("sConvertMToon"))) CreateMToonMaterial(material);
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
                edSet.isShowMainUV = Foldout(GetLoc("sMainUV"), edSet.isShowMainUV);
                DrawMenuButton(GetLoc("sAnchorUVSetting"), lilPropertyBlock.UV);
                if(edSet.isShowMainUV)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    EditorGUILayout.LabelField(GetLoc("sMainUV"), customToggleFont);
                    DrawMenuButton(GetLoc("sAnchorUVSetting"), lilPropertyBlock.UV);
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
                edSet.isShowMain = Foldout(GetLoc("sMainColorSetting"), edSet.isShowMain);
                DrawMenuButton(GetLoc("sAnchorMainColor"), lilPropertyBlock.MainColor);
                if(edSet.isShowMain)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // Main
                    EditorGUILayout.BeginVertical(boxOuter);
                    EditorGUILayout.LabelField(sMainColorBranch, customToggleFont);
                    DrawMenuButton(GetLoc("sAnchorMainColor1"), lilPropertyBlock.MainColor1st);
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    m_MaterialEditor.TexturePropertySingleLine(mainColorRGBAContent, mainTex, mainColor);
                    if(isUseAlpha) SetAlphaIsTransparencyGUI(mainTex);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Shadow
                DrawShadowSettings();

                //------------------------------------------------------------------------------------------------------------------------------
                // Emission
                edSet.isShowEmission = Foldout(GetLoc("sEmissionSetting"), edSet.isShowEmission);
                DrawMenuButton(GetLoc("sAnchorEmission"), lilPropertyBlock.Emission);
                if(edSet.isShowEmission)
                {
                    // Emission
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useEmission, GetLoc("sEmission"));
                    DrawMenuButton(GetLoc("sAnchorEmission"), lilPropertyBlock.Emission1st);
                    if(useEmission.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        TextureGUI(ref edSet.isShowEmissionMap, colorMaskRGBAContent, emissionMap, emissionColor, emissionMap_ScrollRotate, emissionMap_UVMode, true, true);
                        DrawLine();
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
                edSet.isShowMatCap = Foldout(GetLoc("sReflectionsSetting"), edSet.isShowMatCap);
                DrawMenuButton(GetLoc("sAnchorMatCap"), lilPropertyBlock.MatCap1st);
                if(edSet.isShowMatCap)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useMatCap, GetLoc("sMatCap"));
                    DrawMenuButton(GetLoc("sAnchorMatCap"), lilPropertyBlock.MatCap1st);
                    if(useMatCap.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        MatCapTextureGUI(ref edSet.isShowMatCapUV, matcapContent, matcapTex, matcapBlendUV1, matcapZRotCancel, matcapPerspective, matcapVRParallaxStrength);
                        m_MaterialEditor.ShaderProperty(matcapMul, GetLoc("sBlendModeMul"));
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Rim
                edSet.isShowRim = Foldout(GetLoc("sRimLight"), edSet.isShowRim);
                DrawMenuButton(GetLoc("sAnchorRimLight"), lilPropertyBlock.RimLight);
                if(edSet.isShowRim)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useRim, GetLoc("sRimLight"));
                    DrawMenuButton(GetLoc("sAnchorRimLight"), lilPropertyBlock.RimLight);
                    if(useRim.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        m_MaterialEditor.ShaderProperty(rimColor, GetLoc("sColor"));
                        m_MaterialEditor.ShaderProperty(rimShadowMask, GetLoc("sShadowMask"));
                        DrawLine();
                        InvBorderGUI(rimBorder);
                        m_MaterialEditor.ShaderProperty(rimBlur, GetLoc("sBlur"));
                        m_MaterialEditor.ShaderProperty(rimFresnelPower, GetLoc("sFresnelPower"));
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.Space();

                //------------------------------------------------------------------------------------------------------------------------------
                // Advanced
                GUILayout.Label(GetLoc("sAdvanced"), boldLabel);

                //------------------------------------------------------------------------------------------------------------------------------
                // Outline
                DrawOutlineSettings(material);

                //------------------------------------------------------------------------------------------------------------------------------
                // Stencil
                edSet.isShowStencil = Foldout(GetLoc("sStencilSetting"), edSet.isShowStencil);
                DrawMenuButton(GetLoc("sAnchorStencil"), lilPropertyBlock.Stencil);
                if(edSet.isShowStencil)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    EditorGUILayout.LabelField(GetLoc("sStencilSetting"), customToggleFont);
                    DrawMenuButton(GetLoc("sAnchorStencil"), lilPropertyBlock.Stencil);
                    EditorGUILayout.BeginVertical(boxInner);
                    //------------------------------------------------------------------------------------------------------------------------------
                    // Auto Setting
                    if(EditorButton("Set Writer"))
                    {
                        isStWr = true;
                        stencilRef.floatValue = 1;
                        stencilReadMask.floatValue = 255.0f;
                        stencilWriteMask.floatValue = 255.0f;
                        stencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                        stencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Replace;
                        stencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        stencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        if(isOutl)
                        {
                            DrawLine();
                            EditorGUILayout.LabelField(GetLoc("sOutline"), boldLabel);
                            outlineStencilRef.floatValue = 1;
                            outlineStencilReadMask.floatValue = 255.0f;
                            outlineStencilWriteMask.floatValue = 255.0f;
                            outlineStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                            outlineStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Replace;
                            outlineStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                            outlineStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        }
                        SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf);
                        if(renderingModeBuf == RenderingMode.Opaque) material.renderQueue += 450;
                    }
                    if(EditorButton("Set Reader"))
                    {
                        isStWr = false;
                        stencilRef.floatValue = 1;
                        stencilReadMask.floatValue = 255.0f;
                        stencilWriteMask.floatValue = 255.0f;
                        stencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.NotEqual;
                        stencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        stencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        stencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        if(isOutl)
                        {
                            DrawLine();
                            EditorGUILayout.LabelField(GetLoc("sOutline"), boldLabel);
                            outlineStencilRef.floatValue = 1;
                            outlineStencilReadMask.floatValue = 255.0f;
                            outlineStencilWriteMask.floatValue = 255.0f;
                            outlineStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.NotEqual;
                            outlineStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                            outlineStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                            outlineStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        }
                        SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf);
                        if(renderingModeBuf == RenderingMode.Opaque) material.renderQueue += 450;
                    }
                    if(EditorButton("Reset"))
                    {
                        isStWr = false;
                        stencilRef.floatValue = 0;
                        stencilReadMask.floatValue = 255.0f;
                        stencilWriteMask.floatValue = 255.0f;
                        stencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                        stencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        stencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        stencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        if(isOutl)
                        {
                            DrawLine();
                            EditorGUILayout.LabelField(GetLoc("sOutline"), boldLabel);
                            outlineStencilRef.floatValue = 0;
                            outlineStencilReadMask.floatValue = 255.0f;
                            outlineStencilWriteMask.floatValue = 255.0f;
                            outlineStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                            outlineStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                            outlineStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                            outlineStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        }
                        SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf);
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Base
                    {
                        DrawLine();
                        m_MaterialEditor.ShaderProperty(stencilRef, "Ref");
                        m_MaterialEditor.ShaderProperty(stencilReadMask, "ReadMask");
                        m_MaterialEditor.ShaderProperty(stencilWriteMask, "WriteMask");
                        m_MaterialEditor.ShaderProperty(stencilComp, "Comp");
                        m_MaterialEditor.ShaderProperty(stencilPass, "Pass");
                        m_MaterialEditor.ShaderProperty(stencilFail, "Fail");
                        m_MaterialEditor.ShaderProperty(stencilZFail, "ZFail");
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Outline
                    if(isOutl)
                    {
                        DrawLine();
                        EditorGUILayout.LabelField(GetLoc("sOutline"), boldLabel);
                        m_MaterialEditor.ShaderProperty(outlineStencilRef, "Ref");
                        m_MaterialEditor.ShaderProperty(outlineStencilReadMask, "ReadMask");
                        m_MaterialEditor.ShaderProperty(outlineStencilWriteMask, "WriteMask");
                        m_MaterialEditor.ShaderProperty(outlineStencilComp, "Comp");
                        m_MaterialEditor.ShaderProperty(outlineStencilPass, "Pass");
                        m_MaterialEditor.ShaderProperty(outlineStencilFail, "Fail");
                        m_MaterialEditor.ShaderProperty(outlineStencilZFail, "ZFail");
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Rendering
                edSet.isShowRendering = Foldout(GetLoc("sRenderingSetting"), edSet.isShowRendering);
                DrawMenuButton(GetLoc("sAnchorRendering"), lilPropertyBlock.Rendering);
                if(edSet.isShowRendering)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // Reset Button
                    if(EditorButton(GetLoc("sRenderingReset")))
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
                        DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlend, GetLoc("sForward"), srcBlend, dstBlend, srcBlendAlpha, dstBlendAlpha, blendOp, blendOpAlpha);
                        DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlendAdd, GetLoc("sForwardAdd"), srcBlendFA, dstBlendFA, srcBlendAlphaFA, dstBlendAlphaFA, blendOpFA, blendOpAlphaFA);
                        DrawLine();
                        m_MaterialEditor.EnableInstancingField();
                        m_MaterialEditor.DoubleSidedGIField();
                        m_MaterialEditor.RenderQueueField();
                        m_MaterialEditor.ShaderProperty(beforeExposureLimit, GetLoc("sBeforeExposureLimit"));
                        m_MaterialEditor.ShaderProperty(lilDirectionalLightStrength, GetLoc("sDirectionalLightStrength"));
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
                        DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlendOutline, GetLoc("sForward"), outlineSrcBlend, outlineDstBlend, outlineSrcBlendAlpha, outlineDstBlendAlpha, outlineBlendOp, outlineBlendOpAlpha);
                        DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlendAddOutline, GetLoc("sForwardAdd"), outlineSrcBlendFA, outlineDstBlendFA, outlineSrcBlendAlphaFA, outlineDstBlendAlphaFA, outlineBlendOpFA, outlineBlendOpAlphaFA);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Optimization
                if(!isMultiVariants)
                {
                    GUILayout.Label(GetLoc("sOptimization"), boldLabel);
                    edSet.isShowOptimization = Foldout(GetLoc("sOptimization"), edSet.isShowOptimization);
                    DrawHelpButton(GetLoc("sAnchorOptimization"));
                    if(edSet.isShowOptimization)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sOptimization"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        DrawOptimizationButton(material, !(isLite && isMulti));
                        RemoveUnusedPropertiesGUI(material);
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
                edSet.isShowBase = Foldout(GetLoc("sBaseSetting"), edSet.isShowBase);
                DrawMenuButton(GetLoc("sAnchorBaseSetting"), lilPropertyBlock.Base);
                if(edSet.isShowBase)
                {
                    EditorGUILayout.BeginVertical(customBox);
                        m_MaterialEditor.ShaderProperty(cull, sCullModes);
                        m_MaterialEditor.ShaderProperty(invisible, GetLoc("sInvisible"));
                        m_MaterialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                        m_MaterialEditor.RenderQueueField();
                        DrawLine();
                        GUILayout.Label("FakeShadow", boldLabel);
                        m_MaterialEditor.ShaderProperty(fakeShadowVector, BuildParams(GetLoc("sVector"), GetLoc("sOffset")));
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // UV
                edSet.isShowMainUV = Foldout(GetLoc("sMainUV"), edSet.isShowMainUV);
                DrawMenuButton(GetLoc("sAnchorUVSetting"), lilPropertyBlock.UV);
                if(edSet.isShowMainUV)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    EditorGUILayout.LabelField(GetLoc("sMainUV"), customToggleFont);
                    DrawMenuButton(GetLoc("sAnchorUVSetting"), lilPropertyBlock.UV);
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
                edSet.isShowMain = Foldout(GetLoc("sMainColorSetting"), edSet.isShowMain);
                DrawMenuButton(GetLoc("sAnchorMainColor"), lilPropertyBlock.MainColor);
                if(edSet.isShowMain)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // Main
                    EditorGUILayout.BeginVertical(boxOuter);
                    EditorGUILayout.LabelField(sMainColorBranch, customToggleFont);
                    DrawMenuButton(GetLoc("sAnchorMainColor1"), lilPropertyBlock.MainColor1st);
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
                if(ExistsEncryption())
                {
                    edSet.isShowEncryption = Foldout(GetLoc("sEncryption"), edSet.isShowEncryption);
                    DrawMenuButton(GetLoc("sAnchorEncryption"), lilPropertyBlock.Encryption);
                    if(edSet.isShowEncryption)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sEncryption"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorEncryption"), lilPropertyBlock.Encryption);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        m_MaterialEditor.ShaderProperty(ignoreEncryption, GetLoc("sIgnoreEncryption"));
                        m_MaterialEditor.ShaderProperty(keys, GetLoc("sKeys"));
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Stencil
                edSet.isShowStencil = Foldout(GetLoc("sStencilSetting"), edSet.isShowStencil);
                DrawMenuButton(GetLoc("sAnchorStencil"), lilPropertyBlock.Stencil);
                if(edSet.isShowStencil)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    EditorGUILayout.LabelField(GetLoc("sStencilSetting"), customToggleFont);
                    DrawMenuButton(GetLoc("sAnchorStencil"), lilPropertyBlock.Stencil);
                    EditorGUILayout.BeginVertical(boxInner);
                    //------------------------------------------------------------------------------------------------------------------------------
                    // Auto Setting
                    if(EditorButton("Set Writer"))
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
                    if(EditorButton("Set Reader"))
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
                    if(EditorButton("Reset"))
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
                        DrawLine();
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
                edSet.isShowRendering = Foldout(GetLoc("sRenderingSetting"), edSet.isShowRendering);
                DrawMenuButton(GetLoc("sAnchorRendering"), lilPropertyBlock.Rendering);
                if(edSet.isShowRendering)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // Reset Button
                    if(EditorButton(GetLoc("sRenderingReset")))
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
                        DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlend, GetLoc("sForward"), srcBlend, dstBlend, srcBlendAlpha, dstBlendAlpha, blendOp, blendOpAlpha);
                        DrawLine();
                        m_MaterialEditor.EnableInstancingField();
                        m_MaterialEditor.DoubleSidedGIField();
                        m_MaterialEditor.RenderQueueField();
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Optimization
                if(!isMultiVariants)
                {
                    GUILayout.Label(GetLoc("sOptimization"), boldLabel);
                    edSet.isShowOptimization = Foldout(GetLoc("sOptimization"), edSet.isShowOptimization);
                    DrawHelpButton(GetLoc("sAnchorOptimization"));
                    if(edSet.isShowOptimization)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sOptimization"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        DrawOptimizationButton(material, !(isLite && isMulti));
                        RemoveUnusedPropertiesGUI(material);
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
                edSet.isShowMainUV = Foldout(GetLoc("sMainUV"), edSet.isShowMainUV);
                DrawMenuButton(GetLoc("sAnchorUVSetting"), lilPropertyBlock.UV);
                if(edSet.isShowMainUV)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    EditorGUILayout.LabelField(GetLoc("sMainUV"), customToggleFont);
                    DrawMenuButton(GetLoc("sAnchorUVSetting"), lilPropertyBlock.UV);
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
                edSet.isShowMain = Foldout(GetLoc("sMainColorSetting"), edSet.isShowMain);
                DrawMenuButton(GetLoc("sAnchorMainColor"), lilPropertyBlock.MainColor);
                if(edSet.isShowMain)
                {
                    if(isGem)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Main
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(sMainColorBranch, customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorMainColor1"), lilPropertyBlock.MainColor1st);
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
                        DrawMenuButton(GetLoc("sAnchorMainColor1"), lilPropertyBlock.MainColor1st);
                        //m_MaterialEditor.ShaderProperty(useMainTex, GetLoc("sMainColor"));
                        //if(useMainTex.floatValue == 1)
                        //{
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            m_MaterialEditor.TexturePropertySingleLine(mainColorRGBAContent, mainTex, mainColor);
                            if(isUseAlpha) SetAlphaIsTransparencyGUI(mainTex);
                            ToneCorrectionGUI(mainTexHSVG, 1);
                            DrawLine();
                            EditorGUILayout.LabelField(GetLoc("sGradationMap"), boldLabel);
                            m_MaterialEditor.ShaderProperty(mainGradationStrength, GetLoc("sStrength"));
                            if(mainGradationStrength.floatValue != 0)
                            {
                                m_MaterialEditor.TexturePropertySingleLine(gradationContent, mainGradationTex);
                                GradientEditor(material, mainGrad, mainGradationTex, true);
                            }
                            DrawLine();
                            m_MaterialEditor.TexturePropertySingleLine(adjustMaskContent, mainColorAdjustMask);
                            TextureBakeGUI(material, 4);
                            EditorGUILayout.EndVertical();
                        //}
                        EditorGUILayout.EndVertical();

                        //------------------------------------------------------------------------------------------------------------------------------
                        // 2nd
                        EditorGUILayout.BeginVertical(boxOuter);
                        m_MaterialEditor.ShaderProperty(useMain2ndTex, GetLoc("sMainColor2nd"));
                        DrawMenuButton(GetLoc("sAnchorMainColor2"), lilPropertyBlock.MainColor2nd);
                        if(useMain2ndTex.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            m_MaterialEditor.TexturePropertySingleLine(colorRGBAContent, main2ndTex, mainColor2nd);
                            EditorGUI.indentLevel += 2;
                            DrawColorAsAlpha(mainColor2nd);
                            m_MaterialEditor.ShaderProperty(main2ndTexIsMSDF, GetLoc("sAsMSDF"));
                            EditorGUI.indentLevel -= 2;
                            m_MaterialEditor.ShaderProperty(main2ndEnableLighting, GetLoc("sEnableLighting"));
                            m_MaterialEditor.ShaderProperty(main2ndTexBlendMode, sBlendModes);
                            DrawLine();
                            UV4Decal(main2ndTexIsDecal, main2ndTexIsLeftOnly, main2ndTexIsRightOnly, main2ndTexShouldCopy, main2ndTexShouldFlipMirror, main2ndTexShouldFlipCopy, main2ndTex, main2ndTexAngle, main2ndTexDecalAnimation, main2ndTexDecalSubParam, main2ndTex_UVMode);
                            DrawLine();
                            m_MaterialEditor.TexturePropertySingleLine(maskBlendContent, main2ndBlendMask);
                            EditorGUILayout.LabelField(GetLoc("sDistanceFade"));
                            EditorGUI.indentLevel++;
                            m_MaterialEditor.ShaderProperty(main2ndDistanceFade, sDistanceFadeSetting);
                            EditorGUI.indentLevel--;
                            DrawLine();
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
                            DrawLine();
                            TextureBakeGUI(material, 5);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();

                        //------------------------------------------------------------------------------------------------------------------------------
                        // 3rd
                        EditorGUILayout.BeginVertical(boxOuter);
                        m_MaterialEditor.ShaderProperty(useMain3rdTex, GetLoc("sMainColor3rd"));
                        DrawMenuButton(GetLoc("sAnchorMainColor2"), lilPropertyBlock.MainColor3rd);
                        if(useMain3rdTex.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            m_MaterialEditor.TexturePropertySingleLine(colorRGBAContent, main3rdTex, mainColor3rd);
                            EditorGUI.indentLevel += 2;
                            DrawColorAsAlpha(mainColor3rd);
                            m_MaterialEditor.ShaderProperty(main3rdTexIsMSDF, GetLoc("sAsMSDF"));
                            EditorGUI.indentLevel -= 2;
                            m_MaterialEditor.ShaderProperty(main3rdEnableLighting, GetLoc("sEnableLighting"));
                            m_MaterialEditor.ShaderProperty(main3rdTexBlendMode, sBlendModes);
                            DrawLine();
                            UV4Decal(main3rdTexIsDecal, main3rdTexIsLeftOnly, main3rdTexIsRightOnly, main3rdTexShouldCopy, main3rdTexShouldFlipMirror, main3rdTexShouldFlipCopy, main3rdTex, main3rdTexAngle, main3rdTexDecalAnimation, main3rdTexDecalSubParam, main3rdTex_UVMode);
                            DrawLine();
                            m_MaterialEditor.TexturePropertySingleLine(maskBlendContent, main3rdBlendMask);
                            EditorGUILayout.LabelField(GetLoc("sDistanceFade"));
                            EditorGUI.indentLevel++;
                            m_MaterialEditor.ShaderProperty(main3rdDistanceFade, sDistanceFadeSetting);
                            EditorGUI.indentLevel--;
                            DrawLine();
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
                            DrawLine();
                            TextureBakeGUI(material, 6);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Alpha Mask
                        if((renderingModeBuf == RenderingMode.Opaque && !isMulti) || (isMulti && transparentModeMat.floatValue == 0.0f))
                        {
                            GUILayout.Label(GetLoc("sAlphaMaskWarnOpaque"), wrapLabel);
                        }
                        else
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            m_MaterialEditor.ShaderProperty(alphaMaskMode, sAlphaMaskModes);
                            DrawMenuButton(GetLoc("sAnchorAlphaMask"), lilPropertyBlock.AlphaMask);
                            if(alphaMaskMode.floatValue != 0)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                m_MaterialEditor.TexturePropertySingleLine(alphaMaskContent, alphaMask);

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
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Shadow
                if(!isGem)
                {
                    DrawShadowSettings();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Emission
                edSet.isShowEmission = Foldout(GetLoc("sEmissionSetting"), edSet.isShowEmission);
                DrawMenuButton(GetLoc("sAnchorEmission"), lilPropertyBlock.Emission);
                if(edSet.isShowEmission)
                {
                    // Emission
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useEmission, GetLoc("sEmission"));
                    DrawMenuButton(GetLoc("sAnchorEmission"), lilPropertyBlock.Emission1st);
                    if(useEmission.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        TextureGUI(ref edSet.isShowEmissionMap, colorMaskRGBAContent, emissionMap, emissionColor, emissionMap_ScrollRotate, emissionMap_UVMode, true, true);
                        DrawColorAsAlpha(emissionColor);
                        m_MaterialEditor.ShaderProperty(emissionMainStrength, GetLoc("sMainColorPower"));
                        DrawLine();
                        TextureGUI(ref edSet.isShowEmissionBlendMask, maskBlendContent, emissionBlendMask, emissionBlend, emissionBlendMask_ScrollRotate, true, true);
                        DrawLine();
                        m_MaterialEditor.ShaderProperty(emissionBlink, blinkSetting);
                        DrawLine();
                        m_MaterialEditor.ShaderProperty(emissionUseGrad, GetLoc("sGradation"));
                        if(emissionUseGrad.floatValue == 1)
                        {
                            EditorGUI.indentLevel++;
                            m_MaterialEditor.TexturePropertySingleLine(gradSpeedContent, emissionGradTex, emissionGradSpeed);
                            GradientEditor(material, "_eg", emiGrad, emissionGradSpeed);
                            EditorGUI.indentLevel--;
                        }
                        DrawLine();
                        m_MaterialEditor.ShaderProperty(emissionParallaxDepth, GetLoc("sParallaxDepth"));
                        m_MaterialEditor.ShaderProperty(emissionFluorescence, GetLoc("sFluorescence"));
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();

                    // Emission 2nd
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useEmission2nd, GetLoc("sEmission2nd"));
                    DrawMenuButton(GetLoc("sAnchorEmission"), lilPropertyBlock.Emission2nd);
                    if(useEmission2nd.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        TextureGUI(ref edSet.isShowEmission2ndMap, colorMaskRGBAContent, emission2ndMap, emission2ndColor, emission2ndMap_ScrollRotate, emission2ndMap_UVMode, true, true);
                        DrawColorAsAlpha(emission2ndColor);
                        m_MaterialEditor.ShaderProperty(emission2ndMainStrength, GetLoc("sMainColorPower"));
                        DrawLine();
                        TextureGUI(ref edSet.isShowEmission2ndBlendMask, maskBlendContent, emission2ndBlendMask, emission2ndBlend, emission2ndBlendMask_ScrollRotate, true, true);
                        DrawLine();
                        m_MaterialEditor.ShaderProperty(emission2ndBlink, blinkSetting);
                        DrawLine();
                        m_MaterialEditor.ShaderProperty(emission2ndUseGrad, GetLoc("sGradation"));
                        if(emission2ndUseGrad.floatValue == 1)
                        {
                            EditorGUI.indentLevel++;
                            m_MaterialEditor.TexturePropertySingleLine(gradSpeedContent, emission2ndGradTex, emission2ndGradSpeed);
                            GradientEditor(material, "_e2g", emi2Grad, emission2ndGradSpeed);
                            EditorGUI.indentLevel--;
                        }
                        DrawLine();
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
                edSet.isShowBump = Foldout(GetLoc("sNormalMapSetting"), edSet.isShowBump);
                DrawMenuButton(GetLoc("sAnchorNormalMap"), lilPropertyBlock.NormalMap);
                if(edSet.isShowBump)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // 1st
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useBumpMap, GetLoc("sNormalMap"));
                    DrawMenuButton(GetLoc("sAnchorNormalMap"), lilPropertyBlock.NormalMap1st);
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
                    DrawMenuButton(GetLoc("sAnchorNormalMap"), lilPropertyBlock.NormalMap2nd);
                    if(useBump2ndMap.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        TextureGUI(ref edSet.isShowBump2ndMap, normalMapContent, bump2ndMap, bump2ndScale);
                        DrawLine();
                        TextureGUI(ref edSet.isShowBump2ndScaleMask, maskStrengthContent, bump2ndScaleMask);
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Anisotropy
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useAnisotropy, GetLoc("sAnisotropy"));
                    DrawMenuButton(GetLoc("sAnchorAnisotropy"), lilPropertyBlock.Anisotropy);
                    if(useAnisotropy.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        TextureGUI(ref edSet.isShowAnisotropyTangentMap, normalMapContent, anisotropyTangentMap);
                        DrawLine();
                        TextureGUI(ref edSet.isShowAnisotropyScaleMask, maskStrengthContent, anisotropyScaleMask, anisotropyScale);
                        DrawLine();
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
                            DrawLine();
                            EditorGUILayout.LabelField("2nd Specular", boldLabel);
                            m_MaterialEditor.ShaderProperty(anisotropy2ndTangentWidth, GetLoc("sTangentWidth"));
                            m_MaterialEditor.ShaderProperty(anisotropy2ndBitangentWidth, GetLoc("sBitangentWidth"));
                            m_MaterialEditor.ShaderProperty(anisotropy2ndShift, GetLoc("sOffset"));
                            m_MaterialEditor.ShaderProperty(anisotropy2ndShiftNoiseScale, GetLoc("sNoiseStrength"));
                            m_MaterialEditor.ShaderProperty(anisotropy2ndSpecularStrength, GetLoc("sStrength"));
                            DrawLine();
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
                    edSet.isShowBacklight = Foldout(GetLoc("sBacklightSetting"), edSet.isShowBacklight);
                    DrawMenuButton(GetLoc("sAnchorBacklight"), lilPropertyBlock.Reflections);
                    if(edSet.isShowBacklight)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        m_MaterialEditor.ShaderProperty(useBacklight, GetLoc("sBacklight"));
                        DrawMenuButton(GetLoc("sAnchorBacklight"), lilPropertyBlock.Backlight);
                        if(useBacklight.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            TextureGUI(ref edSet.isShowBacklightColorTex, colorMaskRGBAContent, backlightColorTex, backlightColor);
                            EditorGUI.indentLevel++;
                            DrawColorAsAlpha(backlightColor);
                            m_MaterialEditor.ShaderProperty(backlightMainStrength, GetLoc("sMainColorPower"));
                            m_MaterialEditor.ShaderProperty(backlightReceiveShadow, GetLoc("sReceiveShadow"));
                            m_MaterialEditor.ShaderProperty(backlightBackfaceMask, GetLoc("sBackfaceMask"));
                            EditorGUI.indentLevel--;
                            DrawLine();
                            m_MaterialEditor.ShaderProperty(backlightNormalStrength, GetLoc("sNormalStrength"));
                            InvBorderGUI(backlightBorder);
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
                    edSet.isShowReflections = Foldout(GetLoc("sReflectionsSetting"), edSet.isShowReflections);
                    DrawMenuButton(GetLoc("sAnchorReflection"), lilPropertyBlock.Reflections);
                    if(edSet.isShowReflections)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Reflection
                        if(!isGem)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            m_MaterialEditor.ShaderProperty(useReflection, GetLoc("sReflection"));
                            DrawMenuButton(GetLoc("sAnchorReflection"), lilPropertyBlock.Reflection);
                            if(useReflection.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                TextureGUI(ref edSet.isShowSmoothnessTex, smoothnessContent, smoothnessTex, smoothness);
                                m_MaterialEditor.ShaderProperty(gsaaStrength, "GSAA", 1);
                                DrawLine();
                                TextureGUI(ref edSet.isShowMetallicGlossMap, metallicContent, metallicGlossMap, metallic);
                                DrawLine();
                                TextureGUI(ref edSet.isShowReflectionColorTex, colorMaskRGBAContent, reflectionColorTex, reflectionColor);
                                EditorGUI.indentLevel++;
                                DrawColorAsAlpha(reflectionColor);
                                m_MaterialEditor.ShaderProperty(reflectance, GetLoc("sReflectance"));
                                EditorGUI.indentLevel--;
                                DrawLine();
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
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // MatCap
                edSet.isShowMatCap = Foldout(GetLoc("sMatCapSetting"), edSet.isShowMatCap);
                DrawMenuButton(GetLoc("sAnchorMatCap"), lilPropertyBlock.MatCaps);
                if(edSet.isShowMatCap)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // MatCap
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useMatCap, GetLoc("sMatCap"));
                    DrawMenuButton(GetLoc("sAnchorMatCap"), lilPropertyBlock.MatCap1st);
                    if(useMatCap.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        MatCapTextureGUI(ref edSet.isShowMatCapUV, matcapContent, matcapTex, matcapColor, matcapBlendUV1, matcapZRotCancel, matcapPerspective, matcapVRParallaxStrength);
                        DrawColorAsAlpha(matcapColor);
                        m_MaterialEditor.ShaderProperty(matcapMainStrength, GetLoc("sMainColorPower"));
                        m_MaterialEditor.ShaderProperty(matcapNormalStrength, GetLoc("sNormalStrength"));
                        DrawLine();
                        TextureGUI(ref edSet.isShowMatCapBlendMask, maskBlendContent, matcapBlendMask, matcapBlend);
                        m_MaterialEditor.ShaderProperty(matcapEnableLighting, GetLoc("sEnableLighting"));
                        m_MaterialEditor.ShaderProperty(matcapShadowMask, GetLoc("sShadowMask"));
                        m_MaterialEditor.ShaderProperty(matcapBackfaceMask, GetLoc("sBackfaceMask"));
                        m_MaterialEditor.ShaderProperty(matcapLod, GetLoc("sBlur"));
                        m_MaterialEditor.ShaderProperty(matcapBlendMode, sBlendModes);
                        if(matcapEnableLighting.floatValue != 0.0f && matcapBlendMode.floatValue == 3.0f && AutoFixHelpBox(GetLoc("sHelpMatCapBlending")))
                        {
                            matcapEnableLighting.floatValue = 0.0f;
                        }
                        if(isTransparent) m_MaterialEditor.ShaderProperty(matcapApplyTransparency, GetLoc("sApplyTransparency"));
                        DrawLine();
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
                    DrawMenuButton(GetLoc("sAnchorMatCap"), lilPropertyBlock.MatCap2nd);
                    if(useMatCap2nd.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        MatCapTextureGUI(ref edSet.isShowMatCap2ndUV, matcapContent, matcap2ndTex, matcap2ndColor, matcap2ndBlendUV1, matcap2ndZRotCancel, matcap2ndPerspective, matcap2ndVRParallaxStrength);
                        DrawColorAsAlpha(matcap2ndColor);
                        m_MaterialEditor.ShaderProperty(matcap2ndMainStrength, GetLoc("sMainColorPower"));
                        m_MaterialEditor.ShaderProperty(matcap2ndNormalStrength, GetLoc("sNormalStrength"));
                        DrawLine();
                        TextureGUI(ref edSet.isShowMatCap2ndBlendMask, maskBlendContent, matcap2ndBlendMask, matcap2ndBlend);
                        m_MaterialEditor.ShaderProperty(matcap2ndEnableLighting, GetLoc("sEnableLighting"));
                        m_MaterialEditor.ShaderProperty(matcap2ndShadowMask, GetLoc("sShadowMask"));
                        m_MaterialEditor.ShaderProperty(matcap2ndBackfaceMask, GetLoc("sBackfaceMask"));
                        m_MaterialEditor.ShaderProperty(matcap2ndLod, GetLoc("sBlur"));
                        m_MaterialEditor.ShaderProperty(matcap2ndBlendMode, sBlendModes);
                        if(matcap2ndEnableLighting.floatValue != 0.0f && matcap2ndBlendMode.floatValue == 3.0f && AutoFixHelpBox(GetLoc("sHelpMatCapBlending")))
                        {
                            matcap2ndEnableLighting.floatValue = 0.0f;
                        }
                        if(isTransparent) m_MaterialEditor.ShaderProperty(matcap2ndApplyTransparency, GetLoc("sApplyTransparency"));
                        DrawLine();
                        m_MaterialEditor.ShaderProperty(matcap2ndCustomNormal, GetLoc("sMatCapCustomNormal"));
                        if(matcap2ndCustomNormal.floatValue == 1)
                        {
                            TextureGUI(ref edSet.isShowMatCap2ndBumpMap, normalMapContent, matcap2ndBumpMap, matcap2ndBumpScale);
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Rim
                edSet.isShowRim = Foldout(GetLoc("sRimLightSetting"), edSet.isShowRim);
                DrawMenuButton(GetLoc("sAnchorRimLight"), lilPropertyBlock.RimLight);
                if(edSet.isShowRim)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useRim, GetLoc("sRimLight"));
                    DrawMenuButton(GetLoc("sAnchorRimLight"), lilPropertyBlock.RimLight);
                    if(useRim.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        TextureGUI(ref edSet.isShowRimColorTex, colorMaskRGBAContent, rimColorTex, rimColor);
                        DrawColorAsAlpha(rimColor);
                        m_MaterialEditor.ShaderProperty(rimMainStrength, GetLoc("sMainColorPower"));
                        m_MaterialEditor.ShaderProperty(rimEnableLighting, GetLoc("sEnableLighting"));
                        m_MaterialEditor.ShaderProperty(rimShadowMask, GetLoc("sShadowMask"));
                        m_MaterialEditor.ShaderProperty(rimBackfaceMask, GetLoc("sBackfaceMask"));
                        if(isTransparent) m_MaterialEditor.ShaderProperty(rimApplyTransparency, GetLoc("sApplyTransparency"));
                        DrawLine();
                        m_MaterialEditor.ShaderProperty(rimDirStrength, GetLoc("sRimLightDirection"));
                        if(rimDirStrength.floatValue != 0)
                        {
                            EditorGUI.indentLevel++;
                            m_MaterialEditor.ShaderProperty(rimDirRange, GetLoc("sRimDirectionRange"));
                            InvBorderGUI(rimBorder);
                            m_MaterialEditor.ShaderProperty(rimBlur, GetLoc("sBlur"));
                            DrawLine();
                            m_MaterialEditor.ShaderProperty(rimIndirRange, GetLoc("sRimIndirectionRange"));
                            m_MaterialEditor.ShaderProperty(rimIndirColor, GetLoc("sColor"));
                            InvBorderGUI(rimIndirBorder);
                            m_MaterialEditor.ShaderProperty(rimIndirBlur, GetLoc("sBlur"));
                            EditorGUI.indentLevel--;
                            DrawLine();
                        }
                        else
                        {
                            InvBorderGUI(rimBorder);
                            m_MaterialEditor.ShaderProperty(rimBlur, GetLoc("sBlur"));
                        }
                        m_MaterialEditor.ShaderProperty(rimNormalStrength, GetLoc("sNormalStrength"));
                        m_MaterialEditor.ShaderProperty(rimFresnelPower, GetLoc("sFresnelPower"));
                        m_MaterialEditor.ShaderProperty(rimVRParallaxStrength, GetLoc("sVRParallaxStrength"));
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Glitter
                DrawGlitterSettings();

                //------------------------------------------------------------------------------------------------------------------------------
                // Gem
                if(isGem)
                {
                    edSet.isShowGem = Foldout(GetLoc("sGemSetting"), edSet.isShowGem);
                    DrawMenuButton(GetLoc("sAnchorGem"), lilPropertyBlock.Gem);
                    if(edSet.isShowGem)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sGem"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorGem"), lilPropertyBlock.Gem);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        GUILayout.Label(GetLoc("sRefraction"), boldLabel);
                        EditorGUI.indentLevel++;
                        m_MaterialEditor.ShaderProperty(refractionStrength, GetLoc("sStrength"));
                        m_MaterialEditor.ShaderProperty(refractionFresnelPower, GetLoc("sRefractionFresnel"));
                        EditorGUI.indentLevel--;
                        DrawLine();
                        GUILayout.Label(GetLoc("sGem"), boldLabel);
                        EditorGUI.indentLevel++;
                        m_MaterialEditor.ShaderProperty(gemChromaticAberration, GetLoc("sChromaticAberration"));
                        m_MaterialEditor.ShaderProperty(gemEnvContrast, GetLoc("sContrast"));
                        m_MaterialEditor.ShaderProperty(gemEnvColor, GetLoc("sEnvironmentColor"));
                        DrawLine();
                        m_MaterialEditor.ShaderProperty(gemParticleLoop, GetLoc("sParticleLoop"));
                        m_MaterialEditor.ShaderProperty(gemParticleColor, GetLoc("sColor"));
                        EditorGUI.indentLevel--;
                        DrawLine();
                        m_MaterialEditor.ShaderProperty(gemVRParallaxStrength, GetLoc("sVRParallaxStrength"));
                        if(shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS) m_MaterialEditor.TexturePropertySingleLine(smoothnessContent, smoothnessTex, smoothness);
                        else                                                    m_MaterialEditor.ShaderProperty(smoothness, GetLoc("sSmoothness"));
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
                edSet.isShowParallax = Foldout(GetLoc("sParallax"), edSet.isShowParallax);
                DrawMenuButton(GetLoc("sAnchorParallax"), lilPropertyBlock.Parallax);
                if(edSet.isShowParallax)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useParallax, GetLoc("sParallax"));
                    DrawMenuButton(GetLoc("sAnchorParallax"), lilPropertyBlock.Parallax);
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
                edSet.isShowDistanceFade = Foldout(GetLoc("sDistanceFade"), edSet.isShowDistanceFade);
                DrawMenuButton(GetLoc("sAnchorDistanceFade"), lilPropertyBlock.DistanceFade);
                if(edSet.isShowDistanceFade)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    EditorGUILayout.LabelField(GetLoc("sDistanceFade"), customToggleFont);
                    DrawMenuButton(GetLoc("sAnchorDistanceFade"), lilPropertyBlock.DistanceFade);
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
                edSet.isShowAudioLink = Foldout(GetLoc("sAudioLink"), edSet.isShowAudioLink);
                DrawMenuButton(GetLoc("sAnchorAudioLink"), lilPropertyBlock.AudioLink);
                if(edSet.isShowAudioLink)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(useAudioLink, GetLoc("sAudioLink"));
                    DrawMenuButton(GetLoc("sAnchorAudioLink"), lilPropertyBlock.AudioLink);
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
                        if(audioLinkUVMode.floatValue == 3) m_MaterialEditor.TexturePropertySingleLine(customMaskContent, audioLinkMask);
                        if(audioLinkUVMode.floatValue == 4)
                        {
                            m_MaterialEditor.TexturePropertySingleLine(customMaskContent, audioLinkMask);
                            DrawVectorAs4Float(audioLinkUVParams, "Volume", "Base Boost", "Treble Boost", "Line Width");
                        }
                        if(audioLinkUVMode.floatValue == 5)
                        {
                            m_MaterialEditor.ShaderProperty(audioLinkUVParams, sALParamsPos);
                            m_MaterialEditor.ShaderProperty(audioLinkStart, GetLoc("sAudioLinkStartPosition"));
                        }
                        DrawLine();
                        GUILayout.Label(GetLoc("sAudioLinkDefaultValue"), boldLabel);
                        EditorGUI.indentLevel++;
                        if(audioLinkUVMode.floatValue == 4) DrawVectorAs4Float(audioLinkDefaultValue, GetLoc("sStrength"), "Detail", "Speed", GetLoc("sThreshold"));
                        else m_MaterialEditor.ShaderProperty(audioLinkDefaultValue, BuildParams(GetLoc("sStrength"), GetLoc("sBlinkStrength"), GetLoc("sBlinkSpeed"), GetLoc("sThreshold")));
                        EditorGUI.indentLevel--;
                        DrawLine();
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
                            if(audioLinkVertexUVMode.floatValue == 3) m_MaterialEditor.TexturePropertySingleLine(customMaskContent, audioLinkMask);
                            if(audioLinkVertexUVMode.floatValue == 1) m_MaterialEditor.ShaderProperty(audioLinkVertexStart, GetLoc("sAudioLinkStartPosition"));
                            DrawLine();
                            m_MaterialEditor.ShaderProperty(audioLinkVertexStrength, BuildParams(GetLoc("sAudioLinkMovingVector"), GetLoc("sAudioLinkNormalStrength")));
                            EditorGUI.indentLevel--;
                        }
                        EditorGUI.indentLevel--;
                        DrawLine();
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
                edSet.isShowDissolve = Foldout(GetLoc("sDissolve"), edSet.isShowDissolve);
                DrawMenuButton(GetLoc("sAnchorDissolve"), lilPropertyBlock.Dissolve);
                if(edSet.isShowDissolve && ((renderingModeBuf == RenderingMode.Opaque && !isMulti) || (isMulti && transparentModeMat.floatValue == 0.0f)))
                {
                    GUILayout.Label(GetLoc("sDissolveWarnOpaque"), wrapLabel);
                }
                if(edSet.isShowDissolve && (renderingModeBuf != RenderingMode.Opaque || (isMulti && transparentModeMat.floatValue != 0.0f)))
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    m_MaterialEditor.ShaderProperty(dissolveParams, sDissolveParamsMode);
                    DrawMenuButton(GetLoc("sAnchorDissolve"), lilPropertyBlock.Dissolve);
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
                if(ExistsEncryption())
                {
                    edSet.isShowEncryption = Foldout(GetLoc("sEncryption"), edSet.isShowEncryption);
                    DrawMenuButton(GetLoc("sAnchorEncryption"), lilPropertyBlock.Encryption);
                    if(edSet.isShowEncryption)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sEncryption"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorEncryption"), lilPropertyBlock.Encryption);
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
                    edSet.isShowRefraction = Foldout(GetLoc("sRefractionSetting"), edSet.isShowRefraction);
                    DrawMenuButton(GetLoc("sAnchorRefraction"), lilPropertyBlock.Refraction);
                    if(edSet.isShowRefraction)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sRefraction"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorRefraction"), lilPropertyBlock.Refraction);
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
                    edSet.isShowFur = Foldout(GetLoc("sFurSetting"), edSet.isShowFur);
                    DrawMenuButton(GetLoc("sAnchorFur"), lilPropertyBlock.Fur);
                    if(edSet.isShowFur)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sFur"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorFur"), lilPropertyBlock.Fur);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        m_MaterialEditor.TexturePropertySingleLine(normalMapContent, furVectorTex, furVectorScale);
                        m_MaterialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sLengthMask"), GetLoc("sStrengthR")), furLengthMask);
                        m_MaterialEditor.ShaderProperty(furVector, BuildParams(GetLoc("sVector"), GetLoc("sLength")));
                        if(isTwoPass) m_MaterialEditor.ShaderProperty(furCutoutLength, GetLoc("sLength") + " (Cutout)");
                        m_MaterialEditor.ShaderProperty(vertexColor2FurVector, GetLoc("sVertexColor2Vector"));
                        m_MaterialEditor.ShaderProperty(furGravity, GetLoc("sGravity"));
                        m_MaterialEditor.ShaderProperty(furRandomize, GetLoc("sRandomize"));
                        DrawLine();
                        m_MaterialEditor.TexturePropertySingleLine(noiseMaskContent, furNoiseMask);
                        m_MaterialEditor.TextureScaleOffsetProperty(furNoiseMask);
                        m_MaterialEditor.TexturePropertySingleLine(alphaMaskContent, furMask);
                        m_MaterialEditor.ShaderProperty(furAO, GetLoc("sAO"));
                        DrawLine();
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
                        MinusRangeGUI(furRootOffset, GetLoc("sRootWidth"));
                        m_MaterialEditor.ShaderProperty(furTouchStrength, GetLoc("sTouchStrength"));
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Stencil
                edSet.isShowStencil = Foldout(GetLoc("sStencilSetting"), edSet.isShowStencil);
                DrawMenuButton(GetLoc("sAnchorStencil"), lilPropertyBlock.Stencil);
                if(edSet.isShowStencil)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    EditorGUILayout.LabelField(GetLoc("sStencilSetting"), customToggleFont);
                    DrawMenuButton(GetLoc("sAnchorStencil"), lilPropertyBlock.Stencil);
                    EditorGUILayout.BeginVertical(boxInner);
                    //------------------------------------------------------------------------------------------------------------------------------
                    // Auto Setting
                    if(EditorButton("Set Writer"))
                    {
                        isStWr = true;
                        stencilRef.floatValue = 1;
                        stencilReadMask.floatValue = 255.0f;
                        stencilWriteMask.floatValue = 255.0f;
                        stencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                        stencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Replace;
                        stencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        stencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        if(isOutl)
                        {
                            DrawLine();
                            EditorGUILayout.LabelField(GetLoc("sOutline"), boldLabel);
                            outlineStencilRef.floatValue = 1;
                            outlineStencilReadMask.floatValue = 255.0f;
                            outlineStencilWriteMask.floatValue = 255.0f;
                            outlineStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                            outlineStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Replace;
                            outlineStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                            outlineStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        }
                        if(isFur)
                        {
                            DrawLine();
                            EditorGUILayout.LabelField(GetLoc("sOutline"), boldLabel);
                            furStencilRef.floatValue = 1;
                            furStencilReadMask.floatValue = 255.0f;
                            furStencilWriteMask.floatValue = 255.0f;
                            furStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                            furStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Replace;
                            furStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                            furStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        }
                        SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf);
                        if(renderingModeBuf == RenderingMode.Opaque) material.renderQueue += 450;
                    }
                    if(EditorButton("Set Reader"))
                    {
                        isStWr = false;
                        stencilRef.floatValue = 1;
                        stencilReadMask.floatValue = 255.0f;
                        stencilWriteMask.floatValue = 255.0f;
                        stencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.NotEqual;
                        stencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        stencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        stencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        if(isOutl)
                        {
                            DrawLine();
                            EditorGUILayout.LabelField(GetLoc("sOutline"), boldLabel);
                            outlineStencilRef.floatValue = 1;
                            outlineStencilReadMask.floatValue = 255.0f;
                            outlineStencilWriteMask.floatValue = 255.0f;
                            outlineStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.NotEqual;
                            outlineStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                            outlineStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                            outlineStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        }
                        if(isFur)
                        {
                            DrawLine();
                            EditorGUILayout.LabelField(GetLoc("sOutline"), boldLabel);
                            furStencilRef.floatValue = 1;
                            furStencilReadMask.floatValue = 255.0f;
                            furStencilWriteMask.floatValue = 255.0f;
                            furStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.NotEqual;
                            furStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                            furStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                            furStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        }
                        SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf);
                        if(renderingModeBuf == RenderingMode.Opaque) material.renderQueue += 450;
                    }
                    if(EditorButton("Reset"))
                    {
                        isStWr = false;
                        stencilRef.floatValue = 0;
                        stencilReadMask.floatValue = 255.0f;
                        stencilWriteMask.floatValue = 255.0f;
                        stencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                        stencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        stencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        stencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        if(isOutl)
                        {
                            DrawLine();
                            EditorGUILayout.LabelField(GetLoc("sOutline"), boldLabel);
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
                            DrawLine();
                            EditorGUILayout.LabelField(GetLoc("sOutline"), boldLabel);
                            furStencilRef.floatValue = 0;
                            furStencilReadMask.floatValue = 255.0f;
                            furStencilWriteMask.floatValue = 255.0f;
                            furStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                            furStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                            furStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                            furStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                        }
                        SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf);
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Base
                    {
                        DrawLine();
                        m_MaterialEditor.ShaderProperty(stencilRef, "Ref");
                        m_MaterialEditor.ShaderProperty(stencilReadMask, "ReadMask");
                        m_MaterialEditor.ShaderProperty(stencilWriteMask, "WriteMask");
                        m_MaterialEditor.ShaderProperty(stencilComp, "Comp");
                        m_MaterialEditor.ShaderProperty(stencilPass, "Pass");
                        m_MaterialEditor.ShaderProperty(stencilFail, "Fail");
                        m_MaterialEditor.ShaderProperty(stencilZFail, "ZFail");
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Outline
                    if(isOutl)
                    {
                        DrawLine();
                        EditorGUILayout.LabelField(GetLoc("sOutline"), boldLabel);
                        m_MaterialEditor.ShaderProperty(outlineStencilRef, "Ref");
                        m_MaterialEditor.ShaderProperty(outlineStencilReadMask, "ReadMask");
                        m_MaterialEditor.ShaderProperty(outlineStencilWriteMask, "WriteMask");
                        m_MaterialEditor.ShaderProperty(outlineStencilComp, "Comp");
                        m_MaterialEditor.ShaderProperty(outlineStencilPass, "Pass");
                        m_MaterialEditor.ShaderProperty(outlineStencilFail, "Fail");
                        m_MaterialEditor.ShaderProperty(outlineStencilZFail, "ZFail");
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Fur
                    if(isFur)
                    {
                        DrawLine();
                        EditorGUILayout.LabelField(GetLoc("sFur"), boldLabel);
                        m_MaterialEditor.ShaderProperty(furStencilRef, "Ref");
                        m_MaterialEditor.ShaderProperty(furStencilReadMask, "ReadMask");
                        m_MaterialEditor.ShaderProperty(furStencilWriteMask, "WriteMask");
                        m_MaterialEditor.ShaderProperty(furStencilComp, "Comp");
                        m_MaterialEditor.ShaderProperty(furStencilPass, "Pass");
                        m_MaterialEditor.ShaderProperty(furStencilFail, "Fail");
                        m_MaterialEditor.ShaderProperty(furStencilZFail, "ZFail");
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Rendering
                edSet.isShowRendering = Foldout(GetLoc("sRenderingSetting"), edSet.isShowRendering);
                DrawMenuButton(GetLoc("sAnchorRendering"), lilPropertyBlock.Rendering);
                if(edSet.isShowRendering)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // Reset Button
                    if(EditorButton(GetLoc("sRenderingReset")))
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
                        DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlend, GetLoc("sForward"), srcBlend, dstBlend, srcBlendAlpha, dstBlendAlpha, blendOp, blendOpAlpha);
                        DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlendAdd, GetLoc("sForwardAdd"), srcBlendFA, dstBlendFA, srcBlendAlphaFA, dstBlendAlphaFA, blendOpFA, blendOpAlphaFA);
                        DrawLine();
                        if(!isCustomEditor) m_MaterialEditor.EnableInstancingField();
                        if(!isCustomEditor) m_MaterialEditor.DoubleSidedGIField();
                        m_MaterialEditor.RenderQueueField();
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
                        DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlendOutline, GetLoc("sForward"), outlineSrcBlend, outlineDstBlend, outlineSrcBlendAlpha, outlineDstBlendAlpha, outlineBlendOp, outlineBlendOpAlpha);
                        DrawLine();
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
                        DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlendFur, GetLoc("sForward"), furSrcBlend, furDstBlend, furSrcBlendAlpha, furDstBlendAlpha, furBlendOp, furBlendOpAlpha);
                        DrawLine();
                        BlendSettingGUI(ref edSet.isShowBlendAddFur, GetLoc("sForwardAdd"), furSrcBlendFA, furDstBlendFA, furSrcBlendAlphaFA, furDstBlendAlphaFA, furBlendOpFA, furBlendOpAlphaFA);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Tessellation
                edSet.isShowTess = Foldout(GetLoc("sTessellation"), edSet.isShowTess);
                DrawMenuButton(GetLoc("sAnchorTessellation"), lilPropertyBlock.Tessellation);
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
                    edSet.isShowOptimization = Foldout(GetLoc("sOptimization"), edSet.isShowOptimization);
                    DrawHelpButton(GetLoc("sAnchorOptimization"));
                    if(edSet.isShowOptimization)
                    {
                        // Optimization
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sOptimization"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        DrawOptimizationButton(material, !(isLite && isMulti));
                        RemoveUnusedPropertiesGUI(material);
                        TextureBakeGUI(material, 0);
                        TextureBakeGUI(material, 1);
                        TextureBakeGUI(material, 2);
                        TextureBakeGUI(material, 3);
                        if(EditorButton(GetLoc("sConvertLite"))) CreateLiteMaterial(material);
                        if(mtoon != null && EditorButton(GetLoc("sConvertMToon"))) CreateMToonMaterial(material);
                        if(!isMulti && !isFur && !isRefr && !isGem && EditorButton(GetLoc("sConvertMulti"))) CreateMultiMaterial(material);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();

                        // Bake Textures
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sBake"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        if(!isGem && EditorButton(GetLoc("sShadow1stColor")))   AutoBakeColoredMask(material, shadowColorTex,       shadowColor,        "Shadow1stColor");
                        if(!isGem && EditorButton(GetLoc("sShadow2ndColor")))   AutoBakeColoredMask(material, shadow2ndColorTex,    shadow2ndColor,     "Shadow2ndColor");
                        if(!isGem && EditorButton(GetLoc("sShadow3rdColor")))   AutoBakeColoredMask(material, shadow3rdColorTex,    shadow3rdColor,     "Shadow3rdColor");
                        if(!isGem && EditorButton(GetLoc("sReflection")))       AutoBakeColoredMask(material, reflectionColorTex,   reflectionColor,    "ReflectionColor");
                        if(EditorButton(GetLoc("sMatCap")))                     AutoBakeColoredMask(material, matcapBlendMask,      matcapColor,        "MatCapColor");
                        if(EditorButton(GetLoc("sMatCap2nd")))                  AutoBakeColoredMask(material, matcap2ndBlendMask,   matcap2ndColor,     "MatCap2ndColor");
                        if(EditorButton(GetLoc("sRimLight")))                   AutoBakeColoredMask(material, rimColorTex,          rimColor,           "RimColor");
                        if(((!isRefr && !isFur && !isGem && !isCustomShader) || (isCustomShader && isOutl)) && EditorButton(GetLoc("sSettingTexOutlineColor"))) AutoBakeColoredMask(material, outlineColorMask, outlineColor, "OutlineColor");
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

            EditorGUI.BeginChangeCheck();
            ToggleGUI("[Debug] Apply optimization on the editor", ref shaderSetting.isDebugOptimize);
            edSet.isShowShaderSetting = Foldout(GetLoc("sShaderSetting"), edSet.isShowShaderSetting);
            DrawHelpButton(GetLoc("sAnchorShaderSetting"));
            if(edSet.isShowShaderSetting)
            {
                EditorGUILayout.BeginVertical(customBox);
                ToggleGUI(GetLoc("sSettingClippingCanceller"), ref shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER);
                EditorGUILayout.EndVertical();
            }

            edSet.isShowOptimizationSetting = Foldout(GetLoc("sSettingBuildSizeOptimization"), edSet.isShowOptimizationSetting);
            if(edSet.isShowOptimizationSetting)
            {
                EditorGUILayout.BeginVertical(customBox);
                EditorGUILayout.HelpBox(GetLoc("sHelpShaderSetting"),MessageType.Info);
                OptimizationSettingGUI();
                EditorGUILayout.EndVertical();
            }

            edSet.isShowDefaultValueSetting = Foldout(GetLoc("sSettingDefaultValue"), edSet.isShowDefaultValueSetting);
            if(edSet.isShowDefaultValueSetting)
            {
                EditorGUILayout.BeginVertical(customBox);
                DefaultValueSettingGUI();
                EditorGUILayout.EndVertical();
            }
            if(EditorGUI.EndChangeCheck())
            {
                if(shaderSetting.isDebugOptimize)
                {
                    EditorUtility.SetDirty(shaderSetting);
                    AssetDatabase.SaveAssets();
                    ApplyShaderSettingOptimized();
                }
                else
                {
                    TurnOnAllShaderSetting(ref shaderSetting);
                    EditorUtility.SetDirty(shaderSetting);
                    AssetDatabase.SaveAssets();
                    ApplyShaderSetting(shaderSetting);
                }
            }
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Property loader
        #region
        private void LoadProperties(MaterialProperty[] props)
        {
            // Base
            invisible.p             = FindProperty("_Invisible", props, false);
            asUnlit.p               = FindProperty("_AsUnlit", props, false);
            cutoff.p                = FindProperty("_Cutoff", props, false);
            subpassCutoff.p         = FindProperty("_SubpassCutoff", props, false);
            flipNormal.p            = FindProperty("_FlipNormal", props, false);
            shiftBackfaceUV.p       = FindProperty("_ShiftBackfaceUV", props, false);
            backfaceForceShadow.p   = FindProperty("_BackfaceForceShadow", props, false);

            // Lighting
            vertexLightStrength.p           = FindProperty("_VertexLightStrength", props, false);
            lightMinLimit.p                 = FindProperty("_LightMinLimit", props, false);
            lightMaxLimit.p                 = FindProperty("_LightMaxLimit", props, false);
            beforeExposureLimit.p           = FindProperty("_BeforeExposureLimit", props, false);
            monochromeLighting.p            = FindProperty("_MonochromeLighting", props, false);
            alphaBoostFA.p                  = FindProperty("_AlphaBoostFA", props, false);
            lilDirectionalLightStrength.p   = FindProperty("_lilDirectionalLightStrength", props, false);
            lightDirectionOverride.p        = FindProperty("_LightDirectionOverride", props, false);

            // Copy
            baseColor.p     = FindProperty("_BaseColor", props, false);
            baseMap.p       = FindProperty("_BaseMap", props, false);
            baseColorMap.p  = FindProperty("_BaseColorMap", props, false);

            // Rendering
            cull.p                  = FindProperty("_Cull", props, false);
            srcBlend.p              = FindProperty("_SrcBlend", props, false);
            dstBlend.p              = FindProperty("_DstBlend", props, false);
            srcBlendAlpha.p         = FindProperty("_SrcBlendAlpha", props, false);
            dstBlendAlpha.p         = FindProperty("_DstBlendAlpha", props, false);
            blendOp.p               = FindProperty("_BlendOp", props, false);
            blendOpAlpha.p          = FindProperty("_BlendOpAlpha", props, false);
            srcBlendFA.p            = FindProperty("_SrcBlendFA", props, false);
            dstBlendFA.p            = FindProperty("_DstBlendFA", props, false);
            srcBlendAlphaFA.p       = FindProperty("_SrcBlendAlphaFA", props, false);
            dstBlendAlphaFA.p       = FindProperty("_DstBlendAlphaFA", props, false);
            blendOpFA.p             = FindProperty("_BlendOpFA", props, false);
            blendOpAlphaFA.p        = FindProperty("_BlendOpAlphaFA", props, false);
            zclip.p                 = FindProperty("_ZClip", props, false);
            zwrite.p                = FindProperty("_ZWrite", props, false);
            ztest.p                 = FindProperty("_ZTest", props, false);
            stencilRef.p            = FindProperty("_StencilRef", props, false);
            stencilReadMask.p       = FindProperty("_StencilReadMask", props, false);
            stencilWriteMask.p      = FindProperty("_StencilWriteMask", props, false);
            stencilComp.p           = FindProperty("_StencilComp", props, false);
            stencilPass.p           = FindProperty("_StencilPass", props, false);
            stencilFail.p           = FindProperty("_StencilFail", props, false);
            stencilZFail.p          = FindProperty("_StencilZFail", props, false);
            offsetFactor.p          = FindProperty("_OffsetFactor", props, false);
            offsetUnits.p           = FindProperty("_OffsetUnits", props, false);
            colorMask.p             = FindProperty("_ColorMask", props, false);
            alphaToMask.p           = FindProperty("_AlphaToMask", props, false);
            lilShadowCasterBias.p   = FindProperty("_lilShadowCasterBias", props, false);

            // Main
            mainTex_ScrollRotate.p  = FindProperty("_MainTex_ScrollRotate", props, false);
            mainColor.p             = FindProperty("_Color", props, false);
            mainTex.p               = FindProperty("_MainTex", props, false);
            mainTexHSVG.p           = FindProperty("_MainTexHSVG", props, false);
            mainGradationStrength.p = FindProperty("_MainGradationStrength", props, false);
            mainGradationTex.p      = FindProperty("_MainGradationTex", props, false);
            mainColorAdjustMask.p   = FindProperty("_MainColorAdjustMask", props, false);

            // Main 2nd
            useMain2ndTex.p                         = FindProperty("_UseMain2ndTex", props, false);
            mainColor2nd.p                          = FindProperty("_Color2nd", props, false);
            main2ndTex.p                            = FindProperty("_Main2ndTex", props, false);
            main2ndTex_UVMode.p                     = FindProperty("_Main2ndTex_UVMode", props, false);
            main2ndTexAngle.p                       = FindProperty("_Main2ndTexAngle", props, false);
            main2ndTexDecalAnimation.p              = FindProperty("_Main2ndTexDecalAnimation", props, false);
            main2ndTexDecalSubParam.p               = FindProperty("_Main2ndTexDecalSubParam", props, false);
            main2ndTexIsDecal.p                     = FindProperty("_Main2ndTexIsDecal", props, false);
            main2ndTexIsLeftOnly.p                  = FindProperty("_Main2ndTexIsLeftOnly", props, false);
            main2ndTexIsRightOnly.p                 = FindProperty("_Main2ndTexIsRightOnly", props, false);
            main2ndTexShouldCopy.p                  = FindProperty("_Main2ndTexShouldCopy", props, false);
            main2ndTexShouldFlipMirror.p            = FindProperty("_Main2ndTexShouldFlipMirror", props, false);
            main2ndTexShouldFlipCopy.p              = FindProperty("_Main2ndTexShouldFlipCopy", props, false);
            main2ndTexIsMSDF.p                      = FindProperty("_Main2ndTexIsMSDF", props, false);
            main2ndBlendMask.p                      = FindProperty("_Main2ndBlendMask", props, false);
            main2ndTexBlendMode.p                   = FindProperty("_Main2ndTexBlendMode", props, false);
            main2ndEnableLighting.p                 = FindProperty("_Main2ndEnableLighting", props, false);
            main2ndDissolveMask.p                   = FindProperty("_Main2ndDissolveMask", props, false);
            main2ndDissolveNoiseMask.p              = FindProperty("_Main2ndDissolveNoiseMask", props, false);
            main2ndDissolveNoiseMask_ScrollRotate.p = FindProperty("_Main2ndDissolveNoiseMask_ScrollRotate", props, false);
            main2ndDissolveNoiseStrength.p          = FindProperty("_Main2ndDissolveNoiseStrength", props, false);
            main2ndDissolveColor.p                  = FindProperty("_Main2ndDissolveColor", props, false);
            main2ndDissolveParams.p                 = FindProperty("_Main2ndDissolveParams", props, false);
            main2ndDissolvePos.p                    = FindProperty("_Main2ndDissolvePos", props, false);
            main2ndDistanceFade.p                   = FindProperty("_Main2ndDistanceFade", props, false);

            // Main 3rd
            useMain3rdTex.p                         = FindProperty("_UseMain3rdTex", props, false);
            mainColor3rd.p                          = FindProperty("_Color3rd", props, false);
            main3rdTex.p                            = FindProperty("_Main3rdTex", props, false);
            main3rdTex_UVMode.p                     = FindProperty("_Main3rdTex_UVMode", props, false);
            main3rdTexAngle.p                       = FindProperty("_Main3rdTexAngle", props, false);
            main3rdTexDecalAnimation.p              = FindProperty("_Main3rdTexDecalAnimation", props, false);
            main3rdTexDecalSubParam.p               = FindProperty("_Main3rdTexDecalSubParam", props, false);
            main3rdTexIsDecal.p                     = FindProperty("_Main3rdTexIsDecal", props, false);
            main3rdTexIsLeftOnly.p                  = FindProperty("_Main3rdTexIsLeftOnly", props, false);
            main3rdTexIsRightOnly.p                 = FindProperty("_Main3rdTexIsRightOnly", props, false);
            main3rdTexShouldCopy.p                  = FindProperty("_Main3rdTexShouldCopy", props, false);
            main3rdTexShouldFlipMirror.p            = FindProperty("_Main3rdTexShouldFlipMirror", props, false);
            main3rdTexShouldFlipCopy.p              = FindProperty("_Main3rdTexShouldFlipCopy", props, false);
            main3rdTexIsMSDF.p                      = FindProperty("_Main3rdTexIsMSDF", props, false);
            main3rdBlendMask.p                      = FindProperty("_Main3rdBlendMask", props, false);
            main3rdTexBlendMode.p                   = FindProperty("_Main3rdTexBlendMode", props, false);
            main3rdEnableLighting.p                 = FindProperty("_Main3rdEnableLighting", props, false);
            main3rdDissolveMask.p                   = FindProperty("_Main3rdDissolveMask", props, false);
            main3rdDissolveNoiseMask.p              = FindProperty("_Main3rdDissolveNoiseMask", props, false);
            main3rdDissolveNoiseMask_ScrollRotate.p = FindProperty("_Main3rdDissolveNoiseMask_ScrollRotate", props, false);
            main3rdDissolveNoiseStrength.p          = FindProperty("_Main3rdDissolveNoiseStrength", props, false);
            main3rdDissolveColor.p                  = FindProperty("_Main3rdDissolveColor", props, false);
            main3rdDissolveParams.p                 = FindProperty("_Main3rdDissolveParams", props, false);
            main3rdDissolvePos.p                    = FindProperty("_Main3rdDissolvePos", props, false);
            main3rdDistanceFade.p                   = FindProperty("_Main3rdDistanceFade", props, false);

            // Alpha Mask
            alphaMaskMode.p     = FindProperty("_AlphaMaskMode", props, false);
            alphaMask.p         = FindProperty("_AlphaMask", props, false);
            alphaMaskScale.p    = FindProperty("_AlphaMaskScale", props, false);
            alphaMaskValue.p    = FindProperty("_AlphaMaskValue", props, false);

            // Shadow
            useShadow.p                 = FindProperty("_UseShadow", props, false);
            shadowStrength.p            = FindProperty("_ShadowStrength", props, false);
            shadowStrengthMask.p        = FindProperty("_ShadowStrengthMask", props, false);
            shadowBorderMask.p          = FindProperty("_ShadowBorderMask", props, false);
            shadowBlurMask.p            = FindProperty("_ShadowBlurMask", props, false);
            shadowStrengthMaskLOD.p     = FindProperty("_ShadowStrengthMaskLOD", props, false);
            shadowBorderMaskLOD.p       = FindProperty("_ShadowBorderMaskLOD", props, false);
            shadowBlurMaskLOD.p         = FindProperty("_ShadowBlurMaskLOD", props, false);
            shadowAOShift.p             = FindProperty("_ShadowAOShift", props, false);
            shadowAOShift2.p            = FindProperty("_ShadowAOShift2", props, false);
            shadowPostAO.p              = FindProperty("_ShadowPostAO", props, false);
            shadowColor.p               = FindProperty("_ShadowColor", props, false);
            shadowColorTex.p            = FindProperty("_ShadowColorTex", props, false);
            shadowNormalStrength.p      = FindProperty("_ShadowNormalStrength", props, false);
            shadowBorder.p              = FindProperty("_ShadowBorder", props, false);
            shadowBlur.p                = FindProperty("_ShadowBlur", props, false);
            shadow2ndColor.p            = FindProperty("_Shadow2ndColor", props, false);
            shadow2ndColorTex.p         = FindProperty("_Shadow2ndColorTex", props, false);
            shadow2ndNormalStrength.p   = FindProperty("_Shadow2ndNormalStrength", props, false);
            shadow2ndBorder.p           = FindProperty("_Shadow2ndBorder", props, false);
            shadow2ndBlur.p             = FindProperty("_Shadow2ndBlur", props, false);
            shadow3rdColor.p            = FindProperty("_Shadow3rdColor", props, false);
            shadow3rdColorTex.p         = FindProperty("_Shadow3rdColorTex", props, false);
            shadow3rdNormalStrength.p   = FindProperty("_Shadow3rdNormalStrength", props, false);
            shadow3rdBorder.p           = FindProperty("_Shadow3rdBorder", props, false);
            shadow3rdBlur.p             = FindProperty("_Shadow3rdBlur", props, false);
            shadowMainStrength.p        = FindProperty("_ShadowMainStrength", props, false);
            shadowEnvStrength.p         = FindProperty("_ShadowEnvStrength", props, false);
            shadowBorderColor.p         = FindProperty("_ShadowBorderColor", props, false);
            shadowBorderRange.p         = FindProperty("_ShadowBorderRange", props, false);
            shadowReceive.p             = FindProperty("_ShadowReceive", props, false);
            shadow2ndReceive.p          = FindProperty("_Shadow2ndReceive", props, false);
            shadow3rdReceive.p          = FindProperty("_Shadow3rdReceive", props, false);
            shadowMaskType.p            = FindProperty("_ShadowMaskType", props, false);
            shadowFlatBorder.p          = FindProperty("_ShadowFlatBorder", props, false);
            shadowFlatBlur.p            = FindProperty("_ShadowFlatBlur", props, false);
            
            // BackLight
            useBacklight.p              = FindProperty("_UseBacklight", props, false);
            backlightColor.p            = FindProperty("_BacklightColor", props, false);
            backlightColorTex.p         = FindProperty("_BacklightColorTex", props, false);
            backlightMainStrength.p     = FindProperty("_BacklightMainStrength", props, false);
            backlightNormalStrength.p   = FindProperty("_BacklightNormalStrength", props, false);
            backlightBorder.p           = FindProperty("_BacklightBorder", props, false);
            backlightBlur.p             = FindProperty("_BacklightBlur", props, false);
            backlightDirectivity.p      = FindProperty("_BacklightDirectivity", props, false);
            backlightViewStrength.p     = FindProperty("_BacklightViewStrength", props, false);
            backlightReceiveShadow.p    = FindProperty("_BacklightReceiveShadow", props, false);
            backlightBackfaceMask.p     = FindProperty("_BacklightBackfaceMask", props, false);

            // Outline
            outlineColor.p              = FindProperty("_OutlineColor", props, false);
            outlineTex.p                = FindProperty("_OutlineTex", props, false);
            outlineTex_ScrollRotate.p   = FindProperty("_OutlineTex_ScrollRotate", props, false);
            outlineTexHSVG.p            = FindProperty("_OutlineTexHSVG", props, false);
            outlineLitColor.p           = FindProperty("_OutlineLitColor", props, false);
            outlineLitApplyTex.p        = FindProperty("_OutlineLitApplyTex", props, false);
            outlineLitScale.p           = FindProperty("_OutlineLitScale", props, false);
            outlineLitOffset.p          = FindProperty("_OutlineLitOffset", props, false);
            outlineWidth.p              = FindProperty("_OutlineWidth", props, false);
            outlineWidthMask.p          = FindProperty("_OutlineWidthMask", props, false);
            outlineFixWidth.p           = FindProperty("_OutlineFixWidth", props, false);
            outlineVertexR2Width.p      = FindProperty("_OutlineVertexR2Width", props, false);
            outlineDeleteMesh.p         = FindProperty("_OutlineDeleteMesh", props, false);
            outlineVectorTex.p          = FindProperty("_OutlineVectorTex", props, false);
            outlineVectorUVMode.p       = FindProperty("_OutlineVectorUVMode", props, false);
            outlineVectorScale.p        = FindProperty("_OutlineVectorScale", props, false);
            outlineEnableLighting.p     = FindProperty("_OutlineEnableLighting", props, false);
            outlineZBias.p              = FindProperty("_OutlineZBias", props, false);
            outlineCull.p               = FindProperty("_OutlineCull", props, false);
            outlineSrcBlend.p           = FindProperty("_OutlineSrcBlend", props, false);
            outlineDstBlend.p           = FindProperty("_OutlineDstBlend", props, false);
            outlineSrcBlendAlpha.p      = FindProperty("_OutlineSrcBlendAlpha", props, false);
            outlineDstBlendAlpha.p      = FindProperty("_OutlineDstBlendAlpha", props, false);
            outlineBlendOp.p            = FindProperty("_OutlineBlendOp", props, false);
            outlineBlendOpAlpha.p       = FindProperty("_OutlineBlendOpAlpha", props, false);
            outlineSrcBlendFA.p         = FindProperty("_OutlineSrcBlendFA", props, false);
            outlineDstBlendFA.p         = FindProperty("_OutlineDstBlendFA", props, false);
            outlineSrcBlendAlphaFA.p    = FindProperty("_OutlineSrcBlendAlphaFA", props, false);
            outlineDstBlendAlphaFA.p    = FindProperty("_OutlineDstBlendAlphaFA", props, false);
            outlineBlendOpFA.p          = FindProperty("_OutlineBlendOpFA", props, false);
            outlineBlendOpAlphaFA.p     = FindProperty("_OutlineBlendOpAlphaFA", props, false);
            outlineZclip.p              = FindProperty("_OutlineZClip", props, false);
            outlineZwrite.p             = FindProperty("_OutlineZWrite", props, false);
            outlineZtest.p              = FindProperty("_OutlineZTest", props, false);
            outlineStencilRef.p         = FindProperty("_OutlineStencilRef", props, false);
            outlineStencilReadMask.p    = FindProperty("_OutlineStencilReadMask", props, false);
            outlineStencilWriteMask.p   = FindProperty("_OutlineStencilWriteMask", props, false);
            outlineStencilComp.p        = FindProperty("_OutlineStencilComp", props, false);
            outlineStencilPass.p        = FindProperty("_OutlineStencilPass", props, false);
            outlineStencilFail.p        = FindProperty("_OutlineStencilFail", props, false);
            outlineStencilZFail.p       = FindProperty("_OutlineStencilZFail", props, false);
            outlineOffsetFactor.p       = FindProperty("_OutlineOffsetFactor", props, false);
            outlineOffsetUnits.p        = FindProperty("_OutlineOffsetUnits", props, false);
            outlineColorMask.p          = FindProperty("_OutlineColorMask", props, false);
            outlineAlphaToMask.p        = FindProperty("_OutlineAlphaToMask", props, false);

            // Normal Map
            useBumpMap.p    = FindProperty("_UseBumpMap", props, false);
            bumpMap.p       = FindProperty("_BumpMap", props, false);
            bumpScale.p     = FindProperty("_BumpScale", props, false);

            // Normal Map 2nd
            useBump2ndMap.p     = FindProperty("_UseBump2ndMap", props, false);
            bump2ndMap.p        = FindProperty("_Bump2ndMap", props, false);
            bump2ndScale.p      = FindProperty("_Bump2ndScale", props, false);
            bump2ndScaleMask.p  = FindProperty("_Bump2ndScaleMask", props, false);
            
            // Anisotropy
            useAnisotropy.p                 = FindProperty("_UseAnisotropy", props, false);
            anisotropyTangentMap.p          = FindProperty("_AnisotropyTangentMap", props, false);
            anisotropyScale.p               = FindProperty("_AnisotropyScale", props, false);
            anisotropyScaleMask.p           = FindProperty("_AnisotropyScaleMask", props, false);
            anisotropyTangentWidth.p        = FindProperty("_AnisotropyTangentWidth", props, false);
            anisotropyBitangentWidth.p      = FindProperty("_AnisotropyBitangentWidth", props, false);
            anisotropyShift.p               = FindProperty("_AnisotropyShift", props, false);
            anisotropyShiftNoiseScale.p     = FindProperty("_AnisotropyShiftNoiseScale", props, false);
            anisotropySpecularStrength.p    = FindProperty("_AnisotropySpecularStrength", props, false);
            anisotropy2ndTangentWidth.p     = FindProperty("_Anisotropy2ndTangentWidth", props, false);
            anisotropy2ndBitangentWidth.p   = FindProperty("_Anisotropy2ndBitangentWidth", props, false);
            anisotropy2ndShift.p            = FindProperty("_Anisotropy2ndShift", props, false);
            anisotropy2ndShiftNoiseScale.p  = FindProperty("_Anisotropy2ndShiftNoiseScale", props, false);
            anisotropy2ndSpecularStrength.p = FindProperty("_Anisotropy2ndSpecularStrength", props, false);
            anisotropyShiftNoiseMask.p      = FindProperty("_AnisotropyShiftNoiseMask", props, false);
            anisotropy2Reflection.p         = FindProperty("_Anisotropy2Reflection", props, false);
            anisotropy2MatCap.p             = FindProperty("_Anisotropy2MatCap", props, false);
            anisotropy2MatCap2nd.p          = FindProperty("_Anisotropy2MatCap2nd", props, false);
            
            // Reflection
            useReflection.p                 = FindProperty("_UseReflection", props, false);
            smoothness.p                    = FindProperty("_Smoothness", props, false);
            smoothnessTex.p                 = FindProperty("_SmoothnessTex", props, false);
            metallic.p                      = FindProperty("_Metallic", props, false);
            metallicGlossMap.p              = FindProperty("_MetallicGlossMap", props, false);
            reflectance.p                   = FindProperty("_Reflectance", props, false);
            reflectionColor.p               = FindProperty("_ReflectionColor", props, false);
            reflectionColorTex.p            = FindProperty("_ReflectionColorTex", props, false);
            gsaaStrength.p                  = FindProperty("_GSAAStrength", props, false);
            applySpecular.p                 = FindProperty("_ApplySpecular", props, false);
            applySpecularFA.p               = FindProperty("_ApplySpecularFA", props, false);
            specularNormalStrength.p        = FindProperty("_SpecularNormalStrength", props, false);
            specularToon.p                  = FindProperty("_SpecularToon", props, false);
            specularBorder.p                = FindProperty("_SpecularBorder", props, false);
            specularBlur.p                  = FindProperty("_SpecularBlur", props, false);
            applyReflection.p               = FindProperty("_ApplyReflection", props, false);
            reflectionBlendMode.p           = FindProperty("_ReflectionBlendMode", props, false);
            reflectionNormalStrength.p      = FindProperty("_ReflectionNormalStrength", props, false);
            reflectionApplyTransparency.p   = FindProperty("_ReflectionApplyTransparency", props, false);
            reflectionCubeTex.p             = FindProperty("_ReflectionCubeTex", props, false);
            reflectionCubeColor.p           = FindProperty("_ReflectionCubeColor", props, false);
            reflectionCubeOverride.p        = FindProperty("_ReflectionCubeOverride", props, false);
            reflectionCubeEnableLighting.p  = FindProperty("_ReflectionCubeEnableLighting", props, false);
            
            // MatCap
            useMatCap.p                 = FindProperty("_UseMatCap", props, false);
            matcapTex.p                 = FindProperty("_MatCapTex", props, false);
            matcapColor.p               = FindProperty("_MatCapColor", props, false);
            matcapMainStrength.p        = FindProperty("_MatCapMainStrength", props, false);
            matcapBlendUV1.p            = FindProperty("_MatCapBlendUV1", props, false);
            matcapZRotCancel.p          = FindProperty("_MatCapZRotCancel", props, false);
            matcapPerspective.p         = FindProperty("_MatCapPerspective", props, false);
            matcapVRParallaxStrength.p  = FindProperty("_MatCapVRParallaxStrength", props, false);
            matcapBlend.p               = FindProperty("_MatCapBlend", props, false);
            matcapBlendMask.p           = FindProperty("_MatCapBlendMask", props, false);
            matcapEnableLighting.p      = FindProperty("_MatCapEnableLighting", props, false);
            matcapShadowMask.p          = FindProperty("_MatCapShadowMask", props, false);
            matcapBackfaceMask.p        = FindProperty("_MatCapBackfaceMask", props, false);
            matcapLod.p                 = FindProperty("_MatCapLod", props, false);
            matcapBlendMode.p           = FindProperty("_MatCapBlendMode", props, false);
            matcapApplyTransparency.p   = FindProperty("_MatCapApplyTransparency", props, false);
            matcapNormalStrength.p      = FindProperty("_MatCapNormalStrength", props, false);
            matcapCustomNormal.p        = FindProperty("_MatCapCustomNormal", props, false);
            matcapBumpMap.p             = FindProperty("_MatCapBumpMap", props, false);
            matcapBumpScale.p           = FindProperty("_MatCapBumpScale", props, false);
            
            // MatCap 2nd
            useMatCap2nd.p                  = FindProperty("_UseMatCap2nd", props, false);
            matcap2ndTex.p                  = FindProperty("_MatCap2ndTex", props, false);
            matcap2ndColor.p                = FindProperty("_MatCap2ndColor", props, false);
            matcap2ndMainStrength.p         = FindProperty("_MatCap2ndMainStrength", props, false);
            matcap2ndBlendUV1.p             = FindProperty("_MatCap2ndBlendUV1", props, false);
            matcap2ndZRotCancel.p           = FindProperty("_MatCap2ndZRotCancel", props, false);
            matcap2ndPerspective.p          = FindProperty("_MatCap2ndPerspective", props, false);
            matcap2ndVRParallaxStrength.p   = FindProperty("_MatCap2ndVRParallaxStrength", props, false);
            matcap2ndBlend.p                = FindProperty("_MatCap2ndBlend", props, false);
            matcap2ndBlendMask.p            = FindProperty("_MatCap2ndBlendMask", props, false);
            matcap2ndEnableLighting.p       = FindProperty("_MatCap2ndEnableLighting", props, false);
            matcap2ndShadowMask.p           = FindProperty("_MatCap2ndShadowMask", props, false);
            matcap2ndBackfaceMask.p         = FindProperty("_MatCap2ndBackfaceMask", props, false);
            matcap2ndLod.p                  = FindProperty("_MatCap2ndLod", props, false);
            matcap2ndBlendMode.p            = FindProperty("_MatCap2ndBlendMode", props, false);
            matcap2ndApplyTransparency.p    = FindProperty("_MatCap2ndApplyTransparency", props, false);
            matcap2ndNormalStrength.p       = FindProperty("_MatCap2ndNormalStrength", props, false);
            matcap2ndCustomNormal.p         = FindProperty("_MatCap2ndCustomNormal", props, false);
            matcap2ndBumpMap.p              = FindProperty("_MatCap2ndBumpMap", props, false);
            matcap2ndBumpScale.p            = FindProperty("_MatCap2ndBumpScale", props, false);
            
            // Rim
            useRim.p                = FindProperty("_UseRim", props, false);
            rimColor.p              = FindProperty("_RimColor", props, false);
            rimColorTex.p           = FindProperty("_RimColorTex", props, false);
            rimMainStrength.p       = FindProperty("_RimMainStrength", props, false);
            rimNormalStrength.p     = FindProperty("_RimNormalStrength", props, false);
            rimBorder.p             = FindProperty("_RimBorder", props, false);
            rimBlur.p               = FindProperty("_RimBlur", props, false);
            rimFresnelPower.p       = FindProperty("_RimFresnelPower", props, false);
            rimEnableLighting.p     = FindProperty("_RimEnableLighting", props, false);
            rimShadowMask.p         = FindProperty("_RimShadowMask", props, false);
            rimBackfaceMask.p       = FindProperty("_RimBackfaceMask", props, false);
            rimVRParallaxStrength.p = FindProperty("_RimVRParallaxStrength", props, false);
            rimApplyTransparency.p  = FindProperty("_RimApplyTransparency", props, false);
            rimDirStrength.p        = FindProperty("_RimDirStrength", props, false);
            rimDirRange.p           = FindProperty("_RimDirRange", props, false);
            rimIndirRange.p         = FindProperty("_RimIndirRange", props, false);
            rimIndirColor.p         = FindProperty("_RimIndirColor", props, false);
            rimIndirBorder.p        = FindProperty("_RimIndirBorder", props, false);
            rimIndirBlur.p          = FindProperty("_RimIndirBlur", props, false);

            // Glitter
            useGlitter.p                = FindProperty("_UseGlitter", props, false);
            glitterUVMode.p             = FindProperty("_GlitterUVMode", props, false);
            glitterColor.p              = FindProperty("_GlitterColor", props, false);
            glitterColorTex.p           = FindProperty("_GlitterColorTex", props, false);
            glitterMainStrength.p       = FindProperty("_GlitterMainStrength", props, false);
            glitterEnableLighting.p     = FindProperty("_GlitterEnableLighting", props, false);
            glitterShadowMask.p         = FindProperty("_GlitterShadowMask", props, false);
            glitterBackfaceMask.p       = FindProperty("_GlitterBackfaceMask", props, false);
            glitterApplyTransparency.p  = FindProperty("_GlitterApplyTransparency", props, false);
            glitterParams1.p            = FindProperty("_GlitterParams1", props, false);
            glitterParams2.p            = FindProperty("_GlitterParams2", props, false);
            glitterPostContrast.p       = FindProperty("_GlitterPostContrast", props, false);
            glitterSensitivity.p        = FindProperty("_GlitterSensitivity", props, false);
            glitterVRParallaxStrength.p = FindProperty("_GlitterVRParallaxStrength", props, false);
            glitterNormalStrength.p     = FindProperty("_GlitterNormalStrength", props, false);

            // Emission
            useEmission.p                       = FindProperty("_UseEmission", props, false);
            emissionColor.p                     = FindProperty("_EmissionColor", props, false);
            emissionMap.p                       = FindProperty("_EmissionMap", props, false);
            emissionMap_ScrollRotate.p          = FindProperty("_EmissionMap_ScrollRotate", props, false);
            emissionMap_UVMode.p                = FindProperty("_EmissionMap_UVMode", props, false);
            emissionMainStrength.p              = FindProperty("_EmissionMainStrength", props, false);
            emissionBlend.p                     = FindProperty("_EmissionBlend", props, false);
            emissionBlendMask.p                 = FindProperty("_EmissionBlendMask", props, false);
            emissionBlendMask_ScrollRotate.p    = FindProperty("_EmissionBlendMask_ScrollRotate", props, false);
            emissionBlink.p                     = FindProperty("_EmissionBlink", props, false);
            emissionUseGrad.p                   = FindProperty("_EmissionUseGrad", props, false);
            emissionGradTex.p                   = FindProperty("_EmissionGradTex", props, false);
            emissionGradSpeed.p                 = FindProperty("_EmissionGradSpeed", props, false);
            emissionParallaxDepth.p             = FindProperty("_EmissionParallaxDepth", props, false);
            emissionFluorescence.p              = FindProperty("_EmissionFluorescence", props, false);

            // Emission 2nd
            useEmission2nd.p                    = FindProperty("_UseEmission2nd", props, false);
            emission2ndColor.p                  = FindProperty("_Emission2ndColor", props, false);
            emission2ndMap.p                    = FindProperty("_Emission2ndMap", props, false);
            emission2ndMap_ScrollRotate.p       = FindProperty("_Emission2ndMap_ScrollRotate", props, false);
            emission2ndMap_UVMode.p             = FindProperty("_Emission2ndMap_UVMode", props, false);
            emission2ndMainStrength.p           = FindProperty("_Emission2ndMainStrength", props, false);
            emission2ndBlend.p                  = FindProperty("_Emission2ndBlend", props, false);
            emission2ndBlendMask.p              = FindProperty("_Emission2ndBlendMask", props, false);
            emission2ndBlendMask_ScrollRotate.p = FindProperty("_Emission2ndBlendMask_ScrollRotate", props, false);
            emission2ndBlink.p                  = FindProperty("_Emission2ndBlink", props, false);
            emission2ndUseGrad.p                = FindProperty("_Emission2ndUseGrad", props, false);
            emission2ndGradTex.p                = FindProperty("_Emission2ndGradTex", props, false);
            emission2ndGradSpeed.p              = FindProperty("_Emission2ndGradSpeed", props, false);
            emission2ndParallaxDepth.p          = FindProperty("_Emission2ndParallaxDepth", props, false);
            emission2ndFluorescence.p           = FindProperty("_Emission2ndFluorescence", props, false);
            
            // Parallax
            useParallax.p       = FindProperty("_UseParallax", props, false);
            usePOM.p            = FindProperty("_UsePOM", props, false);
            parallaxMap.p       = FindProperty("_ParallaxMap", props, false);
            parallax.p          = FindProperty("_Parallax", props, false);
            parallaxOffset.p    = FindProperty("_ParallaxOffset", props, false);

            // Distance Fade
            distanceFade.p      = FindProperty("_DistanceFade", props, false);
            distanceFadeColor.p = FindProperty("_DistanceFadeColor", props, false);
            
            // AudioLink
            useAudioLink.p              = FindProperty("_UseAudioLink", props, false);
            audioLinkDefaultValue.p     = FindProperty("_AudioLinkDefaultValue", props, false);
            audioLinkUVMode.p           = FindProperty("_AudioLinkUVMode", props, false);
            audioLinkUVParams.p         = FindProperty("_AudioLinkUVParams", props, false);
            audioLinkStart.p            = FindProperty("_AudioLinkStart", props, false);
            audioLinkMask.p             = FindProperty("_AudioLinkMask", props, false);
            audioLink2Main2nd.p         = FindProperty("_AudioLink2Main2nd", props, false);
            audioLink2Main3rd.p         = FindProperty("_AudioLink2Main3rd", props, false);
            audioLink2Emission.p        = FindProperty("_AudioLink2Emission", props, false);
            audioLink2EmissionGrad.p    = FindProperty("_AudioLink2EmissionGrad", props, false);
            audioLink2Emission2nd.p     = FindProperty("_AudioLink2Emission2nd", props, false);
            audioLink2Emission2ndGrad.p = FindProperty("_AudioLink2Emission2ndGrad", props, false);
            audioLink2Vertex.p          = FindProperty("_AudioLink2Vertex", props, false);
            audioLinkVertexUVMode.p     = FindProperty("_AudioLinkVertexUVMode", props, false);
            audioLinkVertexUVParams.p   = FindProperty("_AudioLinkVertexUVParams", props, false);
            audioLinkVertexStart.p      = FindProperty("_AudioLinkVertexStart", props, false);
            audioLinkVertexStrength.p   = FindProperty("_AudioLinkVertexStrength", props, false);
            audioLinkAsLocal.p          = FindProperty("_AudioLinkAsLocal", props, false);
            audioLinkLocalMap.p         = FindProperty("_AudioLinkLocalMap", props, false);
            audioLinkLocalMapParams.p   = FindProperty("_AudioLinkLocalMapParams", props, false);

            // Dissolve
            dissolveMask.p                      = FindProperty("_DissolveMask", props, false);
            dissolveNoiseMask.p                 = FindProperty("_DissolveNoiseMask", props, false);
            dissolveNoiseMask_ScrollRotate.p    = FindProperty("_DissolveNoiseMask_ScrollRotate", props, false);
            dissolveNoiseStrength.p             = FindProperty("_DissolveNoiseStrength", props, false);
            dissolveColor.p                     = FindProperty("_DissolveColor", props, false);
            dissolveParams.p                    = FindProperty("_DissolveParams", props, false);
            dissolvePos.p                       = FindProperty("_DissolvePos", props, false);

            // Encryption
            ignoreEncryption.p  = FindProperty("_IgnoreEncryption", props, false);
            keys.p              = FindProperty("_Keys", props, false);

            // Fur
            furNoiseMask.p          = FindProperty("_FurNoiseMask", props, false);
            furLengthMask.p         = FindProperty("_FurLengthMask", props, false);
            furMask.p               = FindProperty("_FurMask", props, false);
            furVectorTex.p          = FindProperty("_FurVectorTex", props, false);
            furVectorScale.p        = FindProperty("_FurVectorScale", props, false);
            furVector.p             = FindProperty("_FurVector", props, false);
            furGravity.p            = FindProperty("_FurGravity", props, false);
            furRandomize.p          = FindProperty("_FurRandomize", props, false);
            furAO.p                 = FindProperty("_FurAO", props, false);
            vertexColor2FurVector.p = FindProperty("_VertexColor2FurVector", props, false);
            furMeshType.p           = FindProperty("_FurMeshType", props, false);
            furLayerNum.p           = FindProperty("_FurLayerNum", props, false);
            furRootOffset.p         = FindProperty("_FurRootOffset", props, false);
            furCutoutLength.p       = FindProperty("_FurCutoutLength", props, false);
            furTouchStrength.p      = FindProperty("_FurTouchStrength", props, false);
            furCull.p               = FindProperty("_FurCull", props, false);
            furSrcBlend.p           = FindProperty("_FurSrcBlend", props, false);
            furDstBlend.p           = FindProperty("_FurDstBlend", props, false);
            furSrcBlendAlpha.p      = FindProperty("_FurSrcBlendAlpha", props, false);
            furDstBlendAlpha.p      = FindProperty("_FurDstBlendAlpha", props, false);
            furBlendOp.p            = FindProperty("_FurBlendOp", props, false);
            furBlendOpAlpha.p       = FindProperty("_FurBlendOpAlpha", props, false);
            furSrcBlendFA.p         = FindProperty("_FurSrcBlendFA", props, false);
            furDstBlendFA.p         = FindProperty("_FurDstBlendFA", props, false);
            furSrcBlendAlphaFA.p    = FindProperty("_FurSrcBlendAlphaFA", props, false);
            furDstBlendAlphaFA.p    = FindProperty("_FurDstBlendAlphaFA", props, false);
            furBlendOpFA.p          = FindProperty("_FurBlendOpFA", props, false);
            furBlendOpAlphaFA.p     = FindProperty("_FurBlendOpAlphaFA", props, false);
            furZclip.p              = FindProperty("_FurZClip", props, false);
            furZwrite.p             = FindProperty("_FurZWrite", props, false);
            furZtest.p              = FindProperty("_FurZTest", props, false);
            furStencilRef.p         = FindProperty("_FurStencilRef", props, false);
            furStencilReadMask.p    = FindProperty("_FurStencilReadMask", props, false);
            furStencilWriteMask.p   = FindProperty("_FurStencilWriteMask", props, false);
            furStencilComp.p        = FindProperty("_FurStencilComp", props, false);
            furStencilPass.p        = FindProperty("_FurStencilPass", props, false);
            furStencilFail.p        = FindProperty("_FurStencilFail", props, false);
            furStencilZFail.p       = FindProperty("_FurStencilZFail", props, false);
            furOffsetFactor.p       = FindProperty("_FurOffsetFactor", props, false);
            furOffsetUnits.p        = FindProperty("_FurOffsetUnits", props, false);
            furColorMask.p          = FindProperty("_FurColorMask", props, false);
            furAlphaToMask.p        = FindProperty("_FurAlphaToMask", props, false);

            // Refraction
            refractionStrength.p        = FindProperty("_RefractionStrength", props, false);
            refractionFresnelPower.p    = FindProperty("_RefractionFresnelPower", props, false);
            refractionColorFromMain.p   = FindProperty("_RefractionColorFromMain", props, false);
            refractionColor.p           = FindProperty("_RefractionColor", props, false);

            // Gem
            gemChromaticAberration.p    = FindProperty("_GemChromaticAberration", props, false);
            gemEnvContrast.p            = FindProperty("_GemEnvContrast", props, false);
            gemEnvColor.p               = FindProperty("_GemEnvColor", props, false);
            gemParticleLoop.p           = FindProperty("_GemParticleLoop", props, false);
            gemParticleColor.p          = FindProperty("_GemParticleColor", props, false);
            gemVRParallaxStrength.p     = FindProperty("_GemVRParallaxStrength", props, false);

            // FakeShadow
            fakeShadowVector.p  = FindProperty("_FakeShadowVector", props, false);

            // Tessellation
            tessEdge.p      = FindProperty("_TessEdge", props, false);
            tessStrength.p  = FindProperty("_TessStrength", props, false);
            tessShrink.p    = FindProperty("_TessShrink", props, false);
            tessFactorMax.p = FindProperty("_TessFactorMax", props, false);

            // Multi
            transparentModeMat.p    = FindProperty("_TransparentMode", props, false);
            useClippingCanceller.p  = FindProperty("_UseClippingCanceller", props, false);
            asOverlay.p             = FindProperty("_AsOverlay", props, false);

            // Lite
            triMask.p   = FindProperty("_TriMask", props, false);
            matcapMul.p = FindProperty("_MatCapMul", props, false);
        }

        private void SetPropertyBlock()
        {
            AddBlock(lilPropertyBlock.Base, invisible);
            AddBlock(lilPropertyBlock.Base, cutoff);
            AddBlock(lilPropertyBlock.Base, cull);
            AddBlock(lilPropertyBlock.Base, flipNormal);
            AddBlock(lilPropertyBlock.Base, backfaceForceShadow);
            AddBlock(lilPropertyBlock.Base, zwrite);
            AddBlock(lilPropertyBlock.Base, fakeShadowVector);
            AddBlock(lilPropertyBlock.Base, triMask, true);

            AddBlock(lilPropertyBlock.Lighting, lightMinLimit);
            AddBlock(lilPropertyBlock.Lighting, lightMaxLimit);
            AddBlock(lilPropertyBlock.Lighting, monochromeLighting);
            AddBlock(lilPropertyBlock.Lighting, shadowEnvStrength);
            AddBlock(lilPropertyBlock.Lighting, asUnlit);
            AddBlock(lilPropertyBlock.Lighting, vertexLightStrength);
            AddBlock(lilPropertyBlock.Lighting, lightDirectionOverride);
            AddBlock(lilPropertyBlock.Lighting, alphaBoostFA);
            AddBlock(lilPropertyBlock.Lighting, blendOpFA);
            AddBlock(lilPropertyBlock.Lighting, beforeExposureLimit);
            AddBlock(lilPropertyBlock.Lighting, lilDirectionalLightStrength);

            AddBlock(lilPropertyBlock.UV, mainTex_ScrollRotate);
            AddBlock(lilPropertyBlock.UV, shiftBackfaceUV);

            AddBlock(lilPropertyBlock.MainColor, mainColor);
            AddBlock(lilPropertyBlock.MainColor, mainTexHSVG);
            AddBlock(lilPropertyBlock.MainColor, mainGradationStrength);
            AddBlock(lilPropertyBlock.MainColor, useMain2ndTex);
            AddBlock(lilPropertyBlock.MainColor, mainColor2nd);
            AddBlock(lilPropertyBlock.MainColor, main2ndTex_UVMode);
            AddBlock(lilPropertyBlock.MainColor, main2ndTexAngle);
            AddBlock(lilPropertyBlock.MainColor, main2ndTexDecalAnimation);
            AddBlock(lilPropertyBlock.MainColor, main2ndTexDecalSubParam);
            AddBlock(lilPropertyBlock.MainColor, main2ndTexIsDecal);
            AddBlock(lilPropertyBlock.MainColor, main2ndTexIsLeftOnly);
            AddBlock(lilPropertyBlock.MainColor, main2ndTexIsRightOnly);
            AddBlock(lilPropertyBlock.MainColor, main2ndTexShouldCopy);
            AddBlock(lilPropertyBlock.MainColor, main2ndTexShouldFlipMirror);
            AddBlock(lilPropertyBlock.MainColor, main2ndTexShouldFlipCopy);
            AddBlock(lilPropertyBlock.MainColor, main2ndTexIsMSDF);
            AddBlock(lilPropertyBlock.MainColor, main2ndTexBlendMode);
            AddBlock(lilPropertyBlock.MainColor, main2ndEnableLighting);
            AddBlock(lilPropertyBlock.MainColor, main2ndDissolveNoiseMask_ScrollRotate);
            AddBlock(lilPropertyBlock.MainColor, main2ndDissolveNoiseStrength);
            AddBlock(lilPropertyBlock.MainColor, main2ndDissolveColor);
            AddBlock(lilPropertyBlock.MainColor, main2ndDissolveParams);
            AddBlock(lilPropertyBlock.MainColor, main2ndDissolvePos);
            AddBlock(lilPropertyBlock.MainColor, main2ndDistanceFade);
            AddBlock(lilPropertyBlock.MainColor, useMain3rdTex);
            AddBlock(lilPropertyBlock.MainColor, mainColor3rd);
            AddBlock(lilPropertyBlock.MainColor, main3rdTex_UVMode);
            AddBlock(lilPropertyBlock.MainColor, main3rdTexAngle);
            AddBlock(lilPropertyBlock.MainColor, main3rdTexDecalAnimation);
            AddBlock(lilPropertyBlock.MainColor, main3rdTexDecalSubParam);
            AddBlock(lilPropertyBlock.MainColor, main3rdTexIsDecal);
            AddBlock(lilPropertyBlock.MainColor, main3rdTexIsLeftOnly);
            AddBlock(lilPropertyBlock.MainColor, main3rdTexIsRightOnly);
            AddBlock(lilPropertyBlock.MainColor, main3rdTexShouldCopy);
            AddBlock(lilPropertyBlock.MainColor, main3rdTexShouldFlipMirror);
            AddBlock(lilPropertyBlock.MainColor, main3rdTexShouldFlipCopy);
            AddBlock(lilPropertyBlock.MainColor, main3rdTexIsMSDF);
            AddBlock(lilPropertyBlock.MainColor, main3rdTexBlendMode);
            AddBlock(lilPropertyBlock.MainColor, main3rdEnableLighting);
            AddBlock(lilPropertyBlock.MainColor, main3rdDissolveMask);
            AddBlock(lilPropertyBlock.MainColor, main3rdDissolveNoiseMask);
            AddBlock(lilPropertyBlock.MainColor, main3rdDissolveNoiseMask_ScrollRotate);
            AddBlock(lilPropertyBlock.MainColor, main3rdDissolveNoiseStrength);
            AddBlock(lilPropertyBlock.MainColor, main3rdDissolveColor);
            AddBlock(lilPropertyBlock.MainColor, main3rdDissolveParams);
            AddBlock(lilPropertyBlock.MainColor, main3rdDissolvePos);
            AddBlock(lilPropertyBlock.MainColor, main3rdDistanceFade);
            AddBlock(lilPropertyBlock.MainColor, alphaMaskMode);
            AddBlock(lilPropertyBlock.MainColor, alphaMaskScale);
            AddBlock(lilPropertyBlock.MainColor, alphaMaskValue);
            AddBlock(lilPropertyBlock.MainColor, mainTex, true);
            AddBlock(lilPropertyBlock.MainColor, mainGradationTex, true);
            AddBlock(lilPropertyBlock.MainColor, mainColorAdjustMask, true);
            AddBlock(lilPropertyBlock.MainColor, main2ndTex, true);
            AddBlock(lilPropertyBlock.MainColor, main2ndBlendMask, true);
            AddBlock(lilPropertyBlock.MainColor, main2ndDissolveMask, true);
            AddBlock(lilPropertyBlock.MainColor, main2ndDissolveNoiseMask, true);
            AddBlock(lilPropertyBlock.MainColor, main3rdTex, true);
            AddBlock(lilPropertyBlock.MainColor, main3rdBlendMask, true);
            AddBlock(lilPropertyBlock.MainColor, main3rdDissolveMask, true);
            AddBlock(lilPropertyBlock.MainColor, main3rdDissolveNoiseMask, true);
            AddBlock(lilPropertyBlock.MainColor, alphaMask, true);

            AddBlock(lilPropertyBlock.MainColor1st, mainColor);
            AddBlock(lilPropertyBlock.MainColor1st, mainTexHSVG);
            AddBlock(lilPropertyBlock.MainColor1st, mainGradationStrength);
            AddBlock(lilPropertyBlock.MainColor1st, mainTex, true);
            AddBlock(lilPropertyBlock.MainColor1st, mainGradationTex, true);
            AddBlock(lilPropertyBlock.MainColor1st, mainColorAdjustMask, true);

            AddBlock(lilPropertyBlock.MainColor2nd, useMain2ndTex);
            AddBlock(lilPropertyBlock.MainColor2nd, mainColor2nd);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndTex_UVMode);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndTexAngle);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndTexDecalAnimation);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndTexDecalSubParam);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndTexIsDecal);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndTexIsLeftOnly);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndTexIsRightOnly);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndTexShouldCopy);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndTexShouldFlipMirror);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndTexShouldFlipCopy);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndTexIsMSDF);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndTexBlendMode);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndEnableLighting);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndDissolveNoiseMask_ScrollRotate);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndDissolveNoiseStrength);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndDissolveColor);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndDissolveParams);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndDissolvePos);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndDistanceFade);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndTex, true);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndBlendMask, true);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndDissolveMask, true);
            AddBlock(lilPropertyBlock.MainColor2nd, main2ndDissolveNoiseMask, true);

            AddBlock(lilPropertyBlock.MainColor3rd, useMain3rdTex);
            AddBlock(lilPropertyBlock.MainColor3rd, mainColor3rd);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdTex_UVMode);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdTexAngle);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdTexDecalAnimation);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdTexDecalSubParam);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdTexIsDecal);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdTexIsLeftOnly);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdTexIsRightOnly);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdTexShouldCopy);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdTexShouldFlipMirror);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdTexShouldFlipCopy);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdTexIsMSDF);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdTexBlendMode);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdEnableLighting);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdDissolveMask);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdDissolveNoiseMask);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdDissolveNoiseMask_ScrollRotate);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdDissolveNoiseStrength);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdDissolveColor);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdDissolveParams);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdDissolvePos);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdDistanceFade);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdTex, true);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdBlendMask, true);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdDissolveMask, true);
            AddBlock(lilPropertyBlock.MainColor3rd, main3rdDissolveNoiseMask, true);

            AddBlock(lilPropertyBlock.AlphaMask, alphaMaskMode);
            AddBlock(lilPropertyBlock.AlphaMask, alphaMaskScale);
            AddBlock(lilPropertyBlock.AlphaMask, alphaMaskValue);
            AddBlock(lilPropertyBlock.AlphaMask, alphaMask, true);

            AddBlock(lilPropertyBlock.Shadow, useShadow);
            AddBlock(lilPropertyBlock.Shadow, shadowColor);
            AddBlock(lilPropertyBlock.Shadow, shadowNormalStrength);
            AddBlock(lilPropertyBlock.Shadow, shadowBorder);
            AddBlock(lilPropertyBlock.Shadow, shadowBlur);
            AddBlock(lilPropertyBlock.Shadow, shadowStrength);
            AddBlock(lilPropertyBlock.Shadow, shadowAOShift);
            AddBlock(lilPropertyBlock.Shadow, shadowAOShift2);
            AddBlock(lilPropertyBlock.Shadow, shadowPostAO);
            AddBlock(lilPropertyBlock.Shadow, shadow2ndColor);
            AddBlock(lilPropertyBlock.Shadow, shadow2ndNormalStrength);
            AddBlock(lilPropertyBlock.Shadow, shadow2ndBorder);
            AddBlock(lilPropertyBlock.Shadow, shadow2ndBlur);
            AddBlock(lilPropertyBlock.Shadow, shadow3rdColor);
            AddBlock(lilPropertyBlock.Shadow, shadow3rdNormalStrength);
            AddBlock(lilPropertyBlock.Shadow, shadow3rdBorder);
            AddBlock(lilPropertyBlock.Shadow, shadow3rdBlur);
            AddBlock(lilPropertyBlock.Shadow, shadowMainStrength);
            AddBlock(lilPropertyBlock.Shadow, shadowEnvStrength);
            AddBlock(lilPropertyBlock.Shadow, shadowBorderColor);
            AddBlock(lilPropertyBlock.Shadow, shadowBorderRange);
            AddBlock(lilPropertyBlock.Shadow, shadowReceive);
            AddBlock(lilPropertyBlock.Shadow, shadow2ndReceive);
            AddBlock(lilPropertyBlock.Shadow, shadow3rdReceive);
            AddBlock(lilPropertyBlock.Shadow, shadowMaskType);
            AddBlock(lilPropertyBlock.Shadow, shadowFlatBorder);
            AddBlock(lilPropertyBlock.Shadow, shadowFlatBlur);
            AddBlock(lilPropertyBlock.Shadow, lilShadowCasterBias);
            AddBlock(lilPropertyBlock.Shadow, shadowBorderMaskLOD);
            AddBlock(lilPropertyBlock.Shadow, shadowBlurMaskLOD);
            AddBlock(lilPropertyBlock.Shadow, shadowStrengthMaskLOD);
            AddBlock(lilPropertyBlock.Shadow, shadowBorderMask, true);
            AddBlock(lilPropertyBlock.Shadow, shadowBlurMask, true);
            AddBlock(lilPropertyBlock.Shadow, shadowStrengthMask, true);
            AddBlock(lilPropertyBlock.Shadow, shadowColorTex, true);
            AddBlock(lilPropertyBlock.Shadow, shadow2ndColorTex, true);
            AddBlock(lilPropertyBlock.Shadow, shadow3rdColorTex, true);

            AddBlock(lilPropertyBlock.Emission, useEmission);
            AddBlock(lilPropertyBlock.Emission, emissionColor);
            AddBlock(lilPropertyBlock.Emission, emissionMap_ScrollRotate);
            AddBlock(lilPropertyBlock.Emission, emissionMap_UVMode);
            AddBlock(lilPropertyBlock.Emission, emissionMainStrength);
            AddBlock(lilPropertyBlock.Emission, emissionBlend);
            AddBlock(lilPropertyBlock.Emission, emissionBlendMask_ScrollRotate);
            AddBlock(lilPropertyBlock.Emission, emissionBlink);
            AddBlock(lilPropertyBlock.Emission, emissionUseGrad);
            AddBlock(lilPropertyBlock.Emission, emissionGradSpeed);
            AddBlock(lilPropertyBlock.Emission, emissionParallaxDepth);
            AddBlock(lilPropertyBlock.Emission, emissionFluorescence);
            AddBlock(lilPropertyBlock.Emission, useEmission2nd);
            AddBlock(lilPropertyBlock.Emission, emission2ndColor);
            AddBlock(lilPropertyBlock.Emission, emission2ndMap_ScrollRotate);
            AddBlock(lilPropertyBlock.Emission, emission2ndMap_UVMode);
            AddBlock(lilPropertyBlock.Emission, emission2ndMainStrength);
            AddBlock(lilPropertyBlock.Emission, emission2ndBlend);
            AddBlock(lilPropertyBlock.Emission, emission2ndBlendMask_ScrollRotate);
            AddBlock(lilPropertyBlock.Emission, emission2ndBlink);
            AddBlock(lilPropertyBlock.Emission, emission2ndUseGrad);
            AddBlock(lilPropertyBlock.Emission, emission2ndGradSpeed);
            AddBlock(lilPropertyBlock.Emission, emission2ndParallaxDepth);
            AddBlock(lilPropertyBlock.Emission, emission2ndFluorescence);
            AddBlock(lilPropertyBlock.Emission, emissionMap, true);
            AddBlock(lilPropertyBlock.Emission, emissionBlendMask, true);
            AddBlock(lilPropertyBlock.Emission, emissionGradTex, true);
            AddBlock(lilPropertyBlock.Emission, emission2ndMap, true);
            AddBlock(lilPropertyBlock.Emission, emission2ndBlendMask, true);
            AddBlock(lilPropertyBlock.Emission, emission2ndGradTex, true);

            AddBlock(lilPropertyBlock.Emission1st, useEmission);
            AddBlock(lilPropertyBlock.Emission1st, emissionColor);
            AddBlock(lilPropertyBlock.Emission1st, emissionMap_ScrollRotate);
            AddBlock(lilPropertyBlock.Emission1st, emissionMap_UVMode);
            AddBlock(lilPropertyBlock.Emission1st, emissionMainStrength);
            AddBlock(lilPropertyBlock.Emission1st, emissionBlend);
            AddBlock(lilPropertyBlock.Emission1st, emissionBlendMask_ScrollRotate);
            AddBlock(lilPropertyBlock.Emission1st, emissionBlink);
            AddBlock(lilPropertyBlock.Emission1st, emissionUseGrad);
            AddBlock(lilPropertyBlock.Emission1st, emissionGradSpeed);
            AddBlock(lilPropertyBlock.Emission1st, emissionParallaxDepth);
            AddBlock(lilPropertyBlock.Emission1st, emissionFluorescence);
            AddBlock(lilPropertyBlock.Emission1st, emissionMap, true);
            AddBlock(lilPropertyBlock.Emission1st, emissionBlendMask, true);
            AddBlock(lilPropertyBlock.Emission1st, emissionGradTex, true);

            AddBlock(lilPropertyBlock.Emission2nd, useEmission2nd);
            AddBlock(lilPropertyBlock.Emission2nd, emission2ndColor);
            AddBlock(lilPropertyBlock.Emission2nd, emission2ndMap_ScrollRotate);
            AddBlock(lilPropertyBlock.Emission2nd, emission2ndMap_UVMode);
            AddBlock(lilPropertyBlock.Emission2nd, emission2ndMainStrength);
            AddBlock(lilPropertyBlock.Emission2nd, emission2ndBlend);
            AddBlock(lilPropertyBlock.Emission2nd, emission2ndBlendMask_ScrollRotate);
            AddBlock(lilPropertyBlock.Emission2nd, emission2ndBlink);
            AddBlock(lilPropertyBlock.Emission2nd, emission2ndUseGrad);
            AddBlock(lilPropertyBlock.Emission2nd, emission2ndGradSpeed);
            AddBlock(lilPropertyBlock.Emission2nd, emission2ndParallaxDepth);
            AddBlock(lilPropertyBlock.Emission2nd, emission2ndFluorescence);
            AddBlock(lilPropertyBlock.Emission2nd, emission2ndMap, true);
            AddBlock(lilPropertyBlock.Emission2nd, emission2ndBlendMask, true);
            AddBlock(lilPropertyBlock.Emission2nd, emission2ndGradTex, true);

            AddBlock(lilPropertyBlock.NormalMap, useBumpMap);
            AddBlock(lilPropertyBlock.NormalMap, bumpScale);
            AddBlock(lilPropertyBlock.NormalMap, useBump2ndMap);
            AddBlock(lilPropertyBlock.NormalMap, bump2ndScale);
            AddBlock(lilPropertyBlock.NormalMap, useAnisotropy);
            AddBlock(lilPropertyBlock.NormalMap, anisotropyScale);
            AddBlock(lilPropertyBlock.NormalMap, anisotropyTangentWidth);
            AddBlock(lilPropertyBlock.NormalMap, anisotropyBitangentWidth);
            AddBlock(lilPropertyBlock.NormalMap, anisotropyShift);
            AddBlock(lilPropertyBlock.NormalMap, anisotropyShiftNoiseScale);
            AddBlock(lilPropertyBlock.NormalMap, anisotropySpecularStrength);
            AddBlock(lilPropertyBlock.NormalMap, anisotropy2ndTangentWidth);
            AddBlock(lilPropertyBlock.NormalMap, anisotropy2ndBitangentWidth);
            AddBlock(lilPropertyBlock.NormalMap, anisotropy2ndShift);
            AddBlock(lilPropertyBlock.NormalMap, anisotropy2ndShiftNoiseScale);
            AddBlock(lilPropertyBlock.NormalMap, anisotropy2ndSpecularStrength);
            AddBlock(lilPropertyBlock.NormalMap, anisotropy2Reflection);
            AddBlock(lilPropertyBlock.NormalMap, anisotropy2MatCap);
            AddBlock(lilPropertyBlock.NormalMap, anisotropy2MatCap2nd);
            AddBlock(lilPropertyBlock.NormalMap, bumpMap, true);
            AddBlock(lilPropertyBlock.NormalMap, bump2ndMap, true);
            AddBlock(lilPropertyBlock.NormalMap, bump2ndScaleMask, true);
            AddBlock(lilPropertyBlock.NormalMap, anisotropyTangentMap, true);
            AddBlock(lilPropertyBlock.NormalMap, anisotropyScaleMask, true);
            AddBlock(lilPropertyBlock.NormalMap, anisotropyShiftNoiseMask, true);

            AddBlock(lilPropertyBlock.NormalMap1st, useBumpMap);
            AddBlock(lilPropertyBlock.NormalMap1st, bumpScale);
            AddBlock(lilPropertyBlock.NormalMap1st, bumpMap, true);

            AddBlock(lilPropertyBlock.NormalMap2nd, useBump2ndMap);
            AddBlock(lilPropertyBlock.NormalMap2nd, bump2ndScale);
            AddBlock(lilPropertyBlock.NormalMap2nd, bump2ndMap, true);
            AddBlock(lilPropertyBlock.NormalMap2nd, bump2ndScaleMask, true);

            AddBlock(lilPropertyBlock.Anisotropy, useAnisotropy);
            AddBlock(lilPropertyBlock.Anisotropy, anisotropyScale);
            AddBlock(lilPropertyBlock.Anisotropy, anisotropyTangentWidth);
            AddBlock(lilPropertyBlock.Anisotropy, anisotropyBitangentWidth);
            AddBlock(lilPropertyBlock.Anisotropy, anisotropyShift);
            AddBlock(lilPropertyBlock.Anisotropy, anisotropyShiftNoiseScale);
            AddBlock(lilPropertyBlock.Anisotropy, anisotropySpecularStrength);
            AddBlock(lilPropertyBlock.Anisotropy, anisotropy2ndTangentWidth);
            AddBlock(lilPropertyBlock.Anisotropy, anisotropy2ndBitangentWidth);
            AddBlock(lilPropertyBlock.Anisotropy, anisotropy2ndShift);
            AddBlock(lilPropertyBlock.Anisotropy, anisotropy2ndShiftNoiseScale);
            AddBlock(lilPropertyBlock.Anisotropy, anisotropy2ndSpecularStrength);
            AddBlock(lilPropertyBlock.Anisotropy, anisotropy2Reflection);
            AddBlock(lilPropertyBlock.Anisotropy, anisotropy2MatCap);
            AddBlock(lilPropertyBlock.Anisotropy, anisotropy2MatCap2nd);
            AddBlock(lilPropertyBlock.Anisotropy, anisotropyTangentMap);
            AddBlock(lilPropertyBlock.Anisotropy, anisotropyScaleMask, true);
            AddBlock(lilPropertyBlock.Anisotropy, anisotropyShiftNoiseMask, true);

            AddBlock(lilPropertyBlock.Reflections, useReflection);
            AddBlock(lilPropertyBlock.Reflections, metallic);
            AddBlock(lilPropertyBlock.Reflections, smoothness);
            AddBlock(lilPropertyBlock.Reflections, reflectance);
            AddBlock(lilPropertyBlock.Reflections, reflectionColor);
            AddBlock(lilPropertyBlock.Reflections, gsaaStrength);
            AddBlock(lilPropertyBlock.Reflections, applySpecular);
            AddBlock(lilPropertyBlock.Reflections, applySpecularFA);
            AddBlock(lilPropertyBlock.Reflections, specularNormalStrength);
            AddBlock(lilPropertyBlock.Reflections, specularToon);
            AddBlock(lilPropertyBlock.Reflections, specularBorder);
            AddBlock(lilPropertyBlock.Reflections, specularBlur);
            AddBlock(lilPropertyBlock.Reflections, applyReflection);
            AddBlock(lilPropertyBlock.Reflections, reflectionNormalStrength);
            AddBlock(lilPropertyBlock.Reflections, reflectionApplyTransparency);
            AddBlock(lilPropertyBlock.Reflections, reflectionCubeColor);
            AddBlock(lilPropertyBlock.Reflections, reflectionCubeOverride);
            AddBlock(lilPropertyBlock.Reflections, reflectionCubeEnableLighting);
            AddBlock(lilPropertyBlock.Reflections, reflectionBlendMode);
            AddBlock(lilPropertyBlock.Reflections, useMatCap);
            AddBlock(lilPropertyBlock.Reflections, matcapColor);
            AddBlock(lilPropertyBlock.Reflections, matcapMainStrength);
            AddBlock(lilPropertyBlock.Reflections, matcapBlendUV1);
            AddBlock(lilPropertyBlock.Reflections, matcapZRotCancel);
            AddBlock(lilPropertyBlock.Reflections, matcapPerspective);
            AddBlock(lilPropertyBlock.Reflections, matcapVRParallaxStrength);
            AddBlock(lilPropertyBlock.Reflections, matcapBlend);
            AddBlock(lilPropertyBlock.Reflections, matcapEnableLighting);
            AddBlock(lilPropertyBlock.Reflections, matcapShadowMask);
            AddBlock(lilPropertyBlock.Reflections, matcapBackfaceMask);
            AddBlock(lilPropertyBlock.Reflections, matcapLod);
            AddBlock(lilPropertyBlock.Reflections, matcapBlendMode);
            AddBlock(lilPropertyBlock.Reflections, matcapMul);
            AddBlock(lilPropertyBlock.Reflections, matcapApplyTransparency);
            AddBlock(lilPropertyBlock.Reflections, matcapNormalStrength);
            AddBlock(lilPropertyBlock.Reflections, matcapCustomNormal);
            AddBlock(lilPropertyBlock.Reflections, matcapBumpScale);
            AddBlock(lilPropertyBlock.Reflections, useMatCap2nd);
            AddBlock(lilPropertyBlock.Reflections, matcap2ndColor);
            AddBlock(lilPropertyBlock.Reflections, matcap2ndMainStrength);
            AddBlock(lilPropertyBlock.Reflections, matcap2ndBlendUV1);
            AddBlock(lilPropertyBlock.Reflections, matcap2ndZRotCancel);
            AddBlock(lilPropertyBlock.Reflections, matcap2ndPerspective);
            AddBlock(lilPropertyBlock.Reflections, matcap2ndVRParallaxStrength);
            AddBlock(lilPropertyBlock.Reflections, matcap2ndBlend);
            AddBlock(lilPropertyBlock.Reflections, matcap2ndEnableLighting);
            AddBlock(lilPropertyBlock.Reflections, matcap2ndShadowMask);
            AddBlock(lilPropertyBlock.Reflections, matcap2ndBackfaceMask);
            AddBlock(lilPropertyBlock.Reflections, matcap2ndLod);
            AddBlock(lilPropertyBlock.Reflections, matcap2ndBlendMode);
            AddBlock(lilPropertyBlock.Reflections, matcap2ndMul);
            AddBlock(lilPropertyBlock.Reflections, matcap2ndNormalStrength);
            AddBlock(lilPropertyBlock.Reflections, matcap2ndApplyTransparency);
            AddBlock(lilPropertyBlock.Reflections, matcap2ndCustomNormal);
            AddBlock(lilPropertyBlock.Reflections, matcap2ndBumpScale);
            AddBlock(lilPropertyBlock.Reflections, useRim);
            AddBlock(lilPropertyBlock.Reflections, rimColor);
            AddBlock(lilPropertyBlock.Reflections, rimMainStrength);
            AddBlock(lilPropertyBlock.Reflections, rimNormalStrength);
            AddBlock(lilPropertyBlock.Reflections, rimBorder);
            AddBlock(lilPropertyBlock.Reflections, rimBlur);
            AddBlock(lilPropertyBlock.Reflections, rimFresnelPower);
            AddBlock(lilPropertyBlock.Reflections, rimEnableLighting);
            AddBlock(lilPropertyBlock.Reflections, rimShadowMask);
            AddBlock(lilPropertyBlock.Reflections, rimBackfaceMask);
            AddBlock(lilPropertyBlock.Reflections, rimVRParallaxStrength);
            AddBlock(lilPropertyBlock.Reflections, rimApplyTransparency);
            AddBlock(lilPropertyBlock.Reflections, rimDirStrength);
            AddBlock(lilPropertyBlock.Reflections, rimDirRange);
            AddBlock(lilPropertyBlock.Reflections, rimIndirRange);
            AddBlock(lilPropertyBlock.Reflections, rimIndirColor);
            AddBlock(lilPropertyBlock.Reflections, rimIndirBorder);
            AddBlock(lilPropertyBlock.Reflections, rimIndirBlur);
            AddBlock(lilPropertyBlock.Reflections, useGlitter);
            AddBlock(lilPropertyBlock.Reflections, glitterUVMode);
            AddBlock(lilPropertyBlock.Reflections, glitterColor);
            AddBlock(lilPropertyBlock.Reflections, glitterMainStrength);
            AddBlock(lilPropertyBlock.Reflections, glitterParams1);
            AddBlock(lilPropertyBlock.Reflections, glitterParams2);
            AddBlock(lilPropertyBlock.Reflections, glitterPostContrast);
            AddBlock(lilPropertyBlock.Reflections, glitterSensitivity);
            AddBlock(lilPropertyBlock.Reflections, glitterEnableLighting);
            AddBlock(lilPropertyBlock.Reflections, glitterShadowMask);
            AddBlock(lilPropertyBlock.Reflections, glitterBackfaceMask);
            AddBlock(lilPropertyBlock.Reflections, glitterApplyTransparency);
            AddBlock(lilPropertyBlock.Reflections, glitterVRParallaxStrength);
            AddBlock(lilPropertyBlock.Reflections, glitterNormalStrength);
            AddBlock(lilPropertyBlock.Reflections, useBacklight);
            AddBlock(lilPropertyBlock.Reflections, backlightColor);
            AddBlock(lilPropertyBlock.Reflections, backlightMainStrength);
            AddBlock(lilPropertyBlock.Reflections, backlightNormalStrength);
            AddBlock(lilPropertyBlock.Reflections, backlightBorder);
            AddBlock(lilPropertyBlock.Reflections, backlightBlur);
            AddBlock(lilPropertyBlock.Reflections, backlightDirectivity);
            AddBlock(lilPropertyBlock.Reflections, backlightViewStrength);
            AddBlock(lilPropertyBlock.Reflections, backlightReceiveShadow);
            AddBlock(lilPropertyBlock.Reflections, backlightBackfaceMask);
            AddBlock(lilPropertyBlock.Reflections, gemChromaticAberration);
            AddBlock(lilPropertyBlock.Reflections, gemEnvContrast);
            AddBlock(lilPropertyBlock.Reflections, gemEnvColor);
            AddBlock(lilPropertyBlock.Reflections, gemParticleLoop);
            AddBlock(lilPropertyBlock.Reflections, gemParticleColor);
            AddBlock(lilPropertyBlock.Reflections, gemVRParallaxStrength);
            AddBlock(lilPropertyBlock.Reflections, refractionStrength);
            AddBlock(lilPropertyBlock.Reflections, refractionFresnelPower);
            AddBlock(lilPropertyBlock.Reflections, metallicGlossMap, true);
            AddBlock(lilPropertyBlock.Reflections, smoothnessTex, true);
            AddBlock(lilPropertyBlock.Reflections, reflectionColorTex, true);
            AddBlock(lilPropertyBlock.Reflections, reflectionCubeTex, true);
            AddBlock(lilPropertyBlock.Reflections, matcapTex, true);
            AddBlock(lilPropertyBlock.Reflections, matcapBlendMask, true);
            AddBlock(lilPropertyBlock.Reflections, matcapBumpMap, true);
            AddBlock(lilPropertyBlock.Reflections, matcap2ndTex, true);
            AddBlock(lilPropertyBlock.Reflections, matcap2ndBlendMask, true);
            AddBlock(lilPropertyBlock.Reflections, matcap2ndBumpMap, true);
            AddBlock(lilPropertyBlock.Reflections, rimColorTex, true);
            AddBlock(lilPropertyBlock.Reflections, glitterColorTex, true);
            AddBlock(lilPropertyBlock.Reflections, backlightColorTex, true);

            AddBlock(lilPropertyBlock.Reflection, useReflection);
            AddBlock(lilPropertyBlock.Reflection, metallic);
            AddBlock(lilPropertyBlock.Reflection, smoothness);
            AddBlock(lilPropertyBlock.Reflection, reflectance);
            AddBlock(lilPropertyBlock.Reflection, reflectionColor);
            AddBlock(lilPropertyBlock.Reflection, gsaaStrength);
            AddBlock(lilPropertyBlock.Reflection, applySpecular);
            AddBlock(lilPropertyBlock.Reflection, applySpecularFA);
            AddBlock(lilPropertyBlock.Reflection, specularNormalStrength);
            AddBlock(lilPropertyBlock.Reflection, specularToon);
            AddBlock(lilPropertyBlock.Reflection, specularBorder);
            AddBlock(lilPropertyBlock.Reflection, specularBlur);
            AddBlock(lilPropertyBlock.Reflection, applyReflection);
            AddBlock(lilPropertyBlock.Reflection, reflectionNormalStrength);
            AddBlock(lilPropertyBlock.Reflection, reflectionApplyTransparency);
            AddBlock(lilPropertyBlock.Reflection, reflectionCubeColor);
            AddBlock(lilPropertyBlock.Reflection, reflectionCubeOverride);
            AddBlock(lilPropertyBlock.Reflection, reflectionCubeEnableLighting);
            AddBlock(lilPropertyBlock.Reflection, reflectionBlendMode);
            AddBlock(lilPropertyBlock.Reflection, metallicGlossMap, true);
            AddBlock(lilPropertyBlock.Reflection, smoothnessTex, true);
            AddBlock(lilPropertyBlock.Reflection, reflectionColorTex, true);
            AddBlock(lilPropertyBlock.Reflection, reflectionCubeTex, true);

            AddBlock(lilPropertyBlock.MatCaps, useMatCap);
            AddBlock(lilPropertyBlock.MatCaps, matcapColor);
            AddBlock(lilPropertyBlock.MatCaps, matcapMainStrength);
            AddBlock(lilPropertyBlock.MatCaps, matcapBlendUV1);
            AddBlock(lilPropertyBlock.MatCaps, matcapZRotCancel);
            AddBlock(lilPropertyBlock.MatCaps, matcapPerspective);
            AddBlock(lilPropertyBlock.MatCaps, matcapVRParallaxStrength);
            AddBlock(lilPropertyBlock.MatCaps, matcapBlend);
            AddBlock(lilPropertyBlock.MatCaps, matcapEnableLighting);
            AddBlock(lilPropertyBlock.MatCaps, matcapShadowMask);
            AddBlock(lilPropertyBlock.MatCaps, matcapBackfaceMask);
            AddBlock(lilPropertyBlock.MatCaps, matcapLod);
            AddBlock(lilPropertyBlock.MatCaps, matcapBlendMode);
            AddBlock(lilPropertyBlock.MatCaps, matcapMul);
            AddBlock(lilPropertyBlock.MatCaps, matcapApplyTransparency);
            AddBlock(lilPropertyBlock.MatCaps, matcapNormalStrength);
            AddBlock(lilPropertyBlock.MatCaps, matcapCustomNormal);
            AddBlock(lilPropertyBlock.MatCaps, matcapBumpScale);
            AddBlock(lilPropertyBlock.MatCaps, useMatCap2nd);
            AddBlock(lilPropertyBlock.MatCaps, matcap2ndColor);
            AddBlock(lilPropertyBlock.MatCaps, matcap2ndMainStrength);
            AddBlock(lilPropertyBlock.MatCaps, matcap2ndBlendUV1);
            AddBlock(lilPropertyBlock.MatCaps, matcap2ndZRotCancel);
            AddBlock(lilPropertyBlock.MatCaps, matcap2ndPerspective);
            AddBlock(lilPropertyBlock.MatCaps, matcap2ndVRParallaxStrength);
            AddBlock(lilPropertyBlock.MatCaps, matcap2ndBlend);
            AddBlock(lilPropertyBlock.MatCaps, matcap2ndEnableLighting);
            AddBlock(lilPropertyBlock.MatCaps, matcap2ndShadowMask);
            AddBlock(lilPropertyBlock.MatCaps, matcap2ndBackfaceMask);
            AddBlock(lilPropertyBlock.MatCaps, matcap2ndLod);
            AddBlock(lilPropertyBlock.MatCaps, matcap2ndBlendMode);
            AddBlock(lilPropertyBlock.MatCaps, matcap2ndMul);
            AddBlock(lilPropertyBlock.MatCaps, matcap2ndNormalStrength);
            AddBlock(lilPropertyBlock.MatCaps, matcap2ndApplyTransparency);
            AddBlock(lilPropertyBlock.MatCaps, matcap2ndCustomNormal);
            AddBlock(lilPropertyBlock.MatCaps, matcap2ndBumpScale);
            AddBlock(lilPropertyBlock.MatCaps, matcapTex, true);
            AddBlock(lilPropertyBlock.MatCaps, matcapBlendMask, true);
            AddBlock(lilPropertyBlock.MatCaps, matcapBumpMap, true);
            AddBlock(lilPropertyBlock.MatCaps, matcap2ndTex, true);
            AddBlock(lilPropertyBlock.MatCaps, matcap2ndBlendMask, true);
            AddBlock(lilPropertyBlock.MatCaps, matcap2ndBumpMap, true);

            AddBlock(lilPropertyBlock.MatCap1st, useMatCap);
            AddBlock(lilPropertyBlock.MatCap1st, matcapColor);
            AddBlock(lilPropertyBlock.MatCap1st, matcapMainStrength);
            AddBlock(lilPropertyBlock.MatCap1st, matcapBlendUV1);
            AddBlock(lilPropertyBlock.MatCap1st, matcapZRotCancel);
            AddBlock(lilPropertyBlock.MatCap1st, matcapPerspective);
            AddBlock(lilPropertyBlock.MatCap1st, matcapVRParallaxStrength);
            AddBlock(lilPropertyBlock.MatCap1st, matcapBlend);
            AddBlock(lilPropertyBlock.MatCap1st, matcapEnableLighting);
            AddBlock(lilPropertyBlock.MatCap1st, matcapShadowMask);
            AddBlock(lilPropertyBlock.MatCap1st, matcapBackfaceMask);
            AddBlock(lilPropertyBlock.MatCap1st, matcapLod);
            AddBlock(lilPropertyBlock.MatCap1st, matcapBlendMode);
            AddBlock(lilPropertyBlock.MatCap1st, matcapMul);
            AddBlock(lilPropertyBlock.MatCap1st, matcapApplyTransparency);
            AddBlock(lilPropertyBlock.MatCap1st, matcapNormalStrength);
            AddBlock(lilPropertyBlock.MatCap1st, matcapCustomNormal);
            AddBlock(lilPropertyBlock.MatCap1st, matcapBumpScale);
            AddBlock(lilPropertyBlock.MatCap1st, matcapTex, true);
            AddBlock(lilPropertyBlock.MatCap1st, matcapBlendMask, true);
            AddBlock(lilPropertyBlock.MatCap1st, matcapBumpMap, true);

            AddBlock(lilPropertyBlock.MatCap2nd, useMatCap2nd);
            AddBlock(lilPropertyBlock.MatCap2nd, matcap2ndColor);
            AddBlock(lilPropertyBlock.MatCap2nd, matcap2ndMainStrength);
            AddBlock(lilPropertyBlock.MatCap2nd, matcap2ndBlendUV1);
            AddBlock(lilPropertyBlock.MatCap2nd, matcap2ndZRotCancel);
            AddBlock(lilPropertyBlock.MatCap2nd, matcap2ndPerspective);
            AddBlock(lilPropertyBlock.MatCap2nd, matcap2ndVRParallaxStrength);
            AddBlock(lilPropertyBlock.MatCap2nd, matcap2ndBlend);
            AddBlock(lilPropertyBlock.MatCap2nd, matcap2ndEnableLighting);
            AddBlock(lilPropertyBlock.MatCap2nd, matcap2ndShadowMask);
            AddBlock(lilPropertyBlock.MatCap2nd, matcap2ndBackfaceMask);
            AddBlock(lilPropertyBlock.MatCap2nd, matcap2ndLod);
            AddBlock(lilPropertyBlock.MatCap2nd, matcap2ndBlendMode);
            AddBlock(lilPropertyBlock.MatCap2nd, matcap2ndMul);
            AddBlock(lilPropertyBlock.MatCap2nd, matcap2ndApplyTransparency);
            AddBlock(lilPropertyBlock.MatCap2nd, matcap2ndNormalStrength);
            AddBlock(lilPropertyBlock.MatCap2nd, matcap2ndCustomNormal);
            AddBlock(lilPropertyBlock.MatCap2nd, matcap2ndBumpScale);
            AddBlock(lilPropertyBlock.MatCap2nd, matcap2ndTex, true);
            AddBlock(lilPropertyBlock.MatCap2nd, matcap2ndBlendMask, true);
            AddBlock(lilPropertyBlock.MatCap2nd, matcap2ndBumpMap, true);

            AddBlock(lilPropertyBlock.RimLight, useRim);
            AddBlock(lilPropertyBlock.RimLight, rimColor);
            AddBlock(lilPropertyBlock.RimLight, rimMainStrength);
            AddBlock(lilPropertyBlock.RimLight, rimNormalStrength);
            AddBlock(lilPropertyBlock.RimLight, rimBorder);
            AddBlock(lilPropertyBlock.RimLight, rimBlur);
            AddBlock(lilPropertyBlock.RimLight, rimFresnelPower);
            AddBlock(lilPropertyBlock.RimLight, rimEnableLighting);
            AddBlock(lilPropertyBlock.RimLight, rimShadowMask);
            AddBlock(lilPropertyBlock.RimLight, rimBackfaceMask);
            AddBlock(lilPropertyBlock.RimLight, rimVRParallaxStrength);
            AddBlock(lilPropertyBlock.RimLight, rimApplyTransparency);
            AddBlock(lilPropertyBlock.RimLight, rimDirStrength);
            AddBlock(lilPropertyBlock.RimLight, rimDirRange);
            AddBlock(lilPropertyBlock.RimLight, rimIndirRange);
            AddBlock(lilPropertyBlock.RimLight, rimIndirColor);
            AddBlock(lilPropertyBlock.RimLight, rimIndirBorder);
            AddBlock(lilPropertyBlock.RimLight, rimIndirBlur);
            AddBlock(lilPropertyBlock.RimLight, rimColorTex, true);

            AddBlock(lilPropertyBlock.Glitter, useGlitter);
            AddBlock(lilPropertyBlock.Glitter, glitterUVMode);
            AddBlock(lilPropertyBlock.Glitter, glitterColor);
            AddBlock(lilPropertyBlock.Glitter, glitterMainStrength);
            AddBlock(lilPropertyBlock.Glitter, glitterParams1);
            AddBlock(lilPropertyBlock.Glitter, glitterParams2);
            AddBlock(lilPropertyBlock.Glitter, glitterPostContrast);
            AddBlock(lilPropertyBlock.Glitter, glitterSensitivity);
            AddBlock(lilPropertyBlock.Glitter, glitterEnableLighting);
            AddBlock(lilPropertyBlock.Glitter, glitterShadowMask);
            AddBlock(lilPropertyBlock.Glitter, glitterBackfaceMask);
            AddBlock(lilPropertyBlock.Glitter, glitterApplyTransparency);
            AddBlock(lilPropertyBlock.Glitter, glitterVRParallaxStrength);
            AddBlock(lilPropertyBlock.Glitter, glitterNormalStrength);
            AddBlock(lilPropertyBlock.Glitter, glitterColorTex, true);

            AddBlock(lilPropertyBlock.Backlight, useBacklight);
            AddBlock(lilPropertyBlock.Backlight, backlightColor);
            AddBlock(lilPropertyBlock.Backlight, backlightMainStrength);
            AddBlock(lilPropertyBlock.Backlight, backlightNormalStrength);
            AddBlock(lilPropertyBlock.Backlight, backlightBorder);
            AddBlock(lilPropertyBlock.Backlight, backlightBlur);
            AddBlock(lilPropertyBlock.Backlight, backlightDirectivity);
            AddBlock(lilPropertyBlock.Backlight, backlightViewStrength);
            AddBlock(lilPropertyBlock.Backlight, backlightReceiveShadow);
            AddBlock(lilPropertyBlock.Backlight, backlightBackfaceMask);
            AddBlock(lilPropertyBlock.Backlight, backlightColorTex, true);

            AddBlock(lilPropertyBlock.Gem, gemChromaticAberration);
            AddBlock(lilPropertyBlock.Gem, gemEnvContrast);
            AddBlock(lilPropertyBlock.Gem, gemEnvColor);
            AddBlock(lilPropertyBlock.Gem, gemParticleLoop);
            AddBlock(lilPropertyBlock.Gem, gemParticleColor);
            AddBlock(lilPropertyBlock.Gem, gemVRParallaxStrength);
            AddBlock(lilPropertyBlock.Gem, refractionStrength);
            AddBlock(lilPropertyBlock.Gem, refractionFresnelPower);
            AddBlock(lilPropertyBlock.Gem, smoothness);
            AddBlock(lilPropertyBlock.Gem, smoothnessTex, true);

            AddBlock(lilPropertyBlock.Outline, outlineColor);
            AddBlock(lilPropertyBlock.Outline, outlineTex_ScrollRotate);
            AddBlock(lilPropertyBlock.Outline, outlineTexHSVG);
            AddBlock(lilPropertyBlock.Outline, outlineLitColor);
            AddBlock(lilPropertyBlock.Outline, outlineLitApplyTex);
            AddBlock(lilPropertyBlock.Outline, outlineLitScale);
            AddBlock(lilPropertyBlock.Outline, outlineLitOffset);
            AddBlock(lilPropertyBlock.Outline, outlineWidth);
            AddBlock(lilPropertyBlock.Outline, outlineFixWidth);
            AddBlock(lilPropertyBlock.Outline, outlineVertexR2Width);
            AddBlock(lilPropertyBlock.Outline, outlineDeleteMesh);
            AddBlock(lilPropertyBlock.Outline, outlineVectorUVMode);
            AddBlock(lilPropertyBlock.Outline, outlineVectorScale);
            AddBlock(lilPropertyBlock.Outline, outlineEnableLighting);
            AddBlock(lilPropertyBlock.Outline, outlineZBias);
            AddBlock(lilPropertyBlock.Outline, outlineTex, true);
            AddBlock(lilPropertyBlock.Outline, outlineWidthMask, true);
            AddBlock(lilPropertyBlock.Outline, outlineVectorTex, true);

            AddBlock(lilPropertyBlock.Parallax, useParallax);
            AddBlock(lilPropertyBlock.Parallax, usePOM);
            AddBlock(lilPropertyBlock.Parallax, parallax);
            AddBlock(lilPropertyBlock.Parallax, parallaxOffset);
            AddBlock(lilPropertyBlock.Parallax, parallaxMap, true);

            AddBlock(lilPropertyBlock.DistanceFade, distanceFadeColor);
            AddBlock(lilPropertyBlock.DistanceFade, distanceFade);

            AddBlock(lilPropertyBlock.AudioLink, useAudioLink);
            AddBlock(lilPropertyBlock.AudioLink, audioLinkDefaultValue);
            AddBlock(lilPropertyBlock.AudioLink, audioLinkUVMode);
            AddBlock(lilPropertyBlock.AudioLink, audioLinkUVParams);
            AddBlock(lilPropertyBlock.AudioLink, audioLinkStart);
            AddBlock(lilPropertyBlock.AudioLink, audioLink2Main2nd);
            AddBlock(lilPropertyBlock.AudioLink, audioLink2Main3rd);
            AddBlock(lilPropertyBlock.AudioLink, audioLink2Emission);
            AddBlock(lilPropertyBlock.AudioLink, audioLink2EmissionGrad);
            AddBlock(lilPropertyBlock.AudioLink, audioLink2Emission2nd);
            AddBlock(lilPropertyBlock.AudioLink, audioLink2Emission2ndGrad);
            AddBlock(lilPropertyBlock.AudioLink, audioLink2Vertex);
            AddBlock(lilPropertyBlock.AudioLink, audioLinkVertexUVMode);
            AddBlock(lilPropertyBlock.AudioLink, audioLinkVertexUVParams);
            AddBlock(lilPropertyBlock.AudioLink, audioLinkVertexStart);
            AddBlock(lilPropertyBlock.AudioLink, audioLinkVertexStrength);
            AddBlock(lilPropertyBlock.AudioLink, audioLinkAsLocal);
            AddBlock(lilPropertyBlock.AudioLink, audioLinkLocalMap);
            AddBlock(lilPropertyBlock.AudioLink, audioLinkLocalMapParams);
            AddBlock(lilPropertyBlock.AudioLink, audioLinkMask, true);

            AddBlock(lilPropertyBlock.Dissolve, dissolveNoiseMask_ScrollRotate);
            AddBlock(lilPropertyBlock.Dissolve, dissolveNoiseStrength);
            AddBlock(lilPropertyBlock.Dissolve, dissolveColor);
            AddBlock(lilPropertyBlock.Dissolve, dissolveParams);
            AddBlock(lilPropertyBlock.Dissolve, dissolvePos);
            AddBlock(lilPropertyBlock.Dissolve, dissolveMask, true);
            AddBlock(lilPropertyBlock.Dissolve, dissolveNoiseMask, true);

            AddBlock(lilPropertyBlock.Refraction, refractionStrength);
            AddBlock(lilPropertyBlock.Refraction, refractionFresnelPower);
            AddBlock(lilPropertyBlock.Refraction, refractionColorFromMain);
            AddBlock(lilPropertyBlock.Refraction, refractionColor);

            AddBlock(lilPropertyBlock.Fur, furVectorScale);
            AddBlock(lilPropertyBlock.Fur, furVector);
            AddBlock(lilPropertyBlock.Fur, furGravity);
            AddBlock(lilPropertyBlock.Fur, furRandomize);
            AddBlock(lilPropertyBlock.Fur, furAO);
            AddBlock(lilPropertyBlock.Fur, vertexColor2FurVector);
            AddBlock(lilPropertyBlock.Fur, furMeshType);
            AddBlock(lilPropertyBlock.Fur, furLayerNum);
            AddBlock(lilPropertyBlock.Fur, furRootOffset);
            AddBlock(lilPropertyBlock.Fur, furCutoutLength);
            AddBlock(lilPropertyBlock.Fur, furTouchStrength);
            AddBlock(lilPropertyBlock.Fur, furNoiseMask, true);
            AddBlock(lilPropertyBlock.Fur, furMask, true);
            AddBlock(lilPropertyBlock.Fur, furLengthMask, true);
            AddBlock(lilPropertyBlock.Fur, furVectorTex, true);

            AddBlock(lilPropertyBlock.Encryption, ignoreEncryption);
            AddBlock(lilPropertyBlock.Encryption, keys);

            AddBlock(lilPropertyBlock.Stencil, stencilRef);
            AddBlock(lilPropertyBlock.Stencil, stencilReadMask);
            AddBlock(lilPropertyBlock.Stencil, stencilWriteMask);
            AddBlock(lilPropertyBlock.Stencil, stencilComp);
            AddBlock(lilPropertyBlock.Stencil, stencilPass);
            AddBlock(lilPropertyBlock.Stencil, stencilFail);
            AddBlock(lilPropertyBlock.Stencil, stencilZFail);
            AddBlock(lilPropertyBlock.Stencil, outlineStencilRef);
            AddBlock(lilPropertyBlock.Stencil, outlineStencilReadMask);
            AddBlock(lilPropertyBlock.Stencil, outlineStencilWriteMask);
            AddBlock(lilPropertyBlock.Stencil, outlineStencilComp);
            AddBlock(lilPropertyBlock.Stencil, outlineStencilPass);
            AddBlock(lilPropertyBlock.Stencil, outlineStencilFail);
            AddBlock(lilPropertyBlock.Stencil, outlineStencilZFail);
            AddBlock(lilPropertyBlock.Stencil, furStencilRef);
            AddBlock(lilPropertyBlock.Stencil, furStencilReadMask);
            AddBlock(lilPropertyBlock.Stencil, furStencilWriteMask);
            AddBlock(lilPropertyBlock.Stencil, furStencilComp);
            AddBlock(lilPropertyBlock.Stencil, furStencilPass);
            AddBlock(lilPropertyBlock.Stencil, furStencilFail);
            AddBlock(lilPropertyBlock.Stencil, furStencilZFail);

            AddBlock(lilPropertyBlock.Rendering, beforeExposureLimit);
            AddBlock(lilPropertyBlock.Rendering, lilDirectionalLightStrength);
            AddBlock(lilPropertyBlock.Rendering, subpassCutoff);
            AddBlock(lilPropertyBlock.Rendering, cull);
            AddBlock(lilPropertyBlock.Rendering, srcBlend);
            AddBlock(lilPropertyBlock.Rendering, dstBlend);
            AddBlock(lilPropertyBlock.Rendering, srcBlendAlpha);
            AddBlock(lilPropertyBlock.Rendering, dstBlendAlpha);
            AddBlock(lilPropertyBlock.Rendering, blendOp);
            AddBlock(lilPropertyBlock.Rendering, blendOpAlpha);
            AddBlock(lilPropertyBlock.Rendering, srcBlendFA);
            AddBlock(lilPropertyBlock.Rendering, dstBlendFA);
            AddBlock(lilPropertyBlock.Rendering, srcBlendAlphaFA);
            AddBlock(lilPropertyBlock.Rendering, dstBlendAlphaFA);
            AddBlock(lilPropertyBlock.Rendering, blendOpFA);
            AddBlock(lilPropertyBlock.Rendering, blendOpAlphaFA);
            AddBlock(lilPropertyBlock.Rendering, zclip);
            AddBlock(lilPropertyBlock.Rendering, zwrite);
            AddBlock(lilPropertyBlock.Rendering, ztest);
            AddBlock(lilPropertyBlock.Rendering, offsetFactor);
            AddBlock(lilPropertyBlock.Rendering, offsetUnits);
            AddBlock(lilPropertyBlock.Rendering, colorMask);
            AddBlock(lilPropertyBlock.Rendering, alphaToMask);
            AddBlock(lilPropertyBlock.Rendering, lilShadowCasterBias);
            AddBlock(lilPropertyBlock.Rendering, outlineCull);
            AddBlock(lilPropertyBlock.Rendering, outlineSrcBlend);
            AddBlock(lilPropertyBlock.Rendering, outlineDstBlend);
            AddBlock(lilPropertyBlock.Rendering, outlineSrcBlendAlpha);
            AddBlock(lilPropertyBlock.Rendering, outlineDstBlendAlpha);
            AddBlock(lilPropertyBlock.Rendering, outlineBlendOp);
            AddBlock(lilPropertyBlock.Rendering, outlineBlendOpAlpha);
            AddBlock(lilPropertyBlock.Rendering, outlineSrcBlendFA);
            AddBlock(lilPropertyBlock.Rendering, outlineDstBlendFA);
            AddBlock(lilPropertyBlock.Rendering, outlineSrcBlendAlphaFA);
            AddBlock(lilPropertyBlock.Rendering, outlineDstBlendAlphaFA);
            AddBlock(lilPropertyBlock.Rendering, outlineBlendOpFA);
            AddBlock(lilPropertyBlock.Rendering, outlineBlendOpAlphaFA);
            AddBlock(lilPropertyBlock.Rendering, outlineZclip);
            AddBlock(lilPropertyBlock.Rendering, outlineZwrite);
            AddBlock(lilPropertyBlock.Rendering, outlineZtest);
            AddBlock(lilPropertyBlock.Rendering, outlineOffsetFactor);
            AddBlock(lilPropertyBlock.Rendering, outlineOffsetUnits);
            AddBlock(lilPropertyBlock.Rendering, outlineColorMask);
            AddBlock(lilPropertyBlock.Rendering, outlineAlphaToMask);
            AddBlock(lilPropertyBlock.Rendering, furCull);
            AddBlock(lilPropertyBlock.Rendering, furSrcBlend);
            AddBlock(lilPropertyBlock.Rendering, furDstBlend);
            AddBlock(lilPropertyBlock.Rendering, furSrcBlendAlpha);
            AddBlock(lilPropertyBlock.Rendering, furDstBlendAlpha);
            AddBlock(lilPropertyBlock.Rendering, furBlendOp);
            AddBlock(lilPropertyBlock.Rendering, furBlendOpAlpha);
            AddBlock(lilPropertyBlock.Rendering, furSrcBlendFA);
            AddBlock(lilPropertyBlock.Rendering, furDstBlendFA);
            AddBlock(lilPropertyBlock.Rendering, furSrcBlendAlphaFA);
            AddBlock(lilPropertyBlock.Rendering, furDstBlendAlphaFA);
            AddBlock(lilPropertyBlock.Rendering, furBlendOpFA);
            AddBlock(lilPropertyBlock.Rendering, furBlendOpAlphaFA);
            AddBlock(lilPropertyBlock.Rendering, furZclip);
            AddBlock(lilPropertyBlock.Rendering, furZwrite);
            AddBlock(lilPropertyBlock.Rendering, furZtest);
            AddBlock(lilPropertyBlock.Rendering, furOffsetFactor);
            AddBlock(lilPropertyBlock.Rendering, furOffsetUnits);
            AddBlock(lilPropertyBlock.Rendering, furColorMask);
            AddBlock(lilPropertyBlock.Rendering, furAlphaToMask);

            AddBlock(lilPropertyBlock.Tessellation, tessEdge);
            AddBlock(lilPropertyBlock.Tessellation, tessStrength);
            AddBlock(lilPropertyBlock.Tessellation, tessShrink);
            AddBlock(lilPropertyBlock.Tessellation, tessFactorMax);
        }

        private void AddBlock(lilPropertyBlock block, lilMaterialProperty prop)
        {
            if(!prop.blocks.Contains(block))
            {
                prop.blocks.Add(block);
            }
        }

        private void AddBlock(lilPropertyBlock block, lilMaterialProperty prop, bool isTexture)
        {
            prop.isTexture = isTexture;
            if(!prop.blocks.Contains(block))
            {
                prop.blocks.Add(block);
            }
        }

        private void CopyMainColorProperties()
        {
            if(mainColor != null && baseColor != null) baseColor.colorValue = mainColor.colorValue;
            if(mainTex != null && baseMap != null) baseMap.textureValue = mainTex.textureValue;
            if(mainTex != null && baseColorMap != null) baseColorMap.textureValue = mainTex.textureValue;
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Language
        #region
        public static string GetLoc(string value) { return loc.ContainsKey(value) ? loc[value] : value; }

        public static string BuildParams(params string[] labels) { return string.Join("|", labels); }

        public static void InitializeLanguage()
        {
            if(edSet.languageNum == -1)
            {
                if(Application.systemLanguage == SystemLanguage.Japanese)                   edSet.languageNum = 1;
                else if(Application.systemLanguage == SystemLanguage.Korean)                edSet.languageNum = 2;
                else if(Application.systemLanguage == SystemLanguage.ChineseSimplified)     edSet.languageNum = 3;
                else if(Application.systemLanguage == SystemLanguage.ChineseTraditional)    edSet.languageNum = 4;
                else                                                                        edSet.languageNum = 0;
            }

            if(loc.Count == 0)
            {
                string langPath = GetEditorLanguageFileGUID();
                LoadLanguage(langPath);
                InitializeLabels();
            }
        }

        public static void LoadCustomLanguage(string langFileGUID)
        {
            string langPath = GUIDToPath(langFileGUID);
            LoadLanguage(langPath);
        }

        private static void SelectLang()
        {
            InitializeLanguage();
            int numbuf = edSet.languageNum;

            // Select language
            string[] langName = edSet.languageNames.Split('\t');
            edSet.languageNum = EditorGUILayout.Popup("Language", edSet.languageNum, langName);

            // Load language
            if(numbuf != edSet.languageNum)
            {
                string langPath = GetEditorLanguageFileGUID();
                LoadLanguage(langPath);
                InitializeLabels();
            }

            if(!string.IsNullOrEmpty(GetLoc("sLanguageWarning"))) EditorGUILayout.HelpBox(GetLoc("sLanguageWarning"),MessageType.Warning);
        }

        private static void LoadLanguage(string langPath)
        {
            if(string.IsNullOrEmpty(langPath) || !File.Exists(langPath)) return;
            StreamReader sr = new StreamReader(langPath);

            string str = sr.ReadLine();
            edSet.languageNames = str.Substring(str.IndexOf("\t")+1);
            edSet.languageName = edSet.languageNames.Split('\t')[edSet.languageNum];
            while((str = sr.ReadLine()) != null)
            {
                string[] lineContents = str.Split('\t');
                loc[lineContents[0]] = lineContents[edSet.languageNum+1];
            }
            sr.Close();
        }

        private static void InitializeLabels()
        {
            sCullModes = BuildParams(GetLoc("sCullMode"), GetLoc("sCullModeOff"), GetLoc("sCullModeFront"), GetLoc("sCullModeBack"));
            sBlendModes = BuildParams(GetLoc("sBlendMode"), GetLoc("sBlendModeNormal"), GetLoc("sBlendModeAdd"), GetLoc("sBlendModeScreen"), GetLoc("sBlendModeMul"));
            sAlphaMaskModes = BuildParams(GetLoc("sAlphaMask"), GetLoc("sAlphaMaskModeNone"), GetLoc("sAlphaMaskModeReplace"), GetLoc("sAlphaMaskModeMul"));
            blinkSetting = BuildParams(GetLoc("sBlinkStrength"), GetLoc("sBlinkType"), GetLoc("sBlinkSpeed"), GetLoc("sBlinkOffset"));
            sDistanceFadeSetting = BuildParams(GetLoc("sStartDistance"), GetLoc("sEndDistance"), GetLoc("sStrength"), GetLoc("sBackfaceForceShadow"));
            sDissolveParams = BuildParams(GetLoc("sDissolveMode"), GetLoc("sDissolveModeNone"), GetLoc("sDissolveModeAlpha"), GetLoc("sDissolveModeUV"), GetLoc("sDissolveModePosition"), GetLoc("sDissolveShape"), GetLoc("sDissolveShapePoint"), GetLoc("sDissolveShapeLine"), GetLoc("sBorder"), GetLoc("sBlur"));
            sDissolveParamsMode = BuildParams(GetLoc("sDissolve"), GetLoc("sDissolveModeNone"), GetLoc("sDissolveModeAlpha"), GetLoc("sDissolveModeUV"), GetLoc("sDissolveModePosition"));
            sDissolveParamsOther = BuildParams(GetLoc("sDissolveShape"), GetLoc("sDissolveShapePoint"), GetLoc("sDissolveShapeLine"), GetLoc("sBorder"), GetLoc("sBlur"), "Dummy");
            sGlitterParams1 = BuildParams("Tiling", GetLoc("sParticleSize"), GetLoc("sContrast"));
            sGlitterParams2 = BuildParams(GetLoc("sBlinkSpeed"), GetLoc("sAngleLimit"), GetLoc("sRimLightDirection"), GetLoc("sColorRandomness"));
            sTransparentMode = BuildParams(GetLoc("sRenderingMode"), GetLoc("sRenderingModeOpaque"), GetLoc("sRenderingModeCutout"), GetLoc("sRenderingModeTransparent"), GetLoc("sRenderingModeRefraction"), GetLoc("sRenderingModeFur"), GetLoc("sRenderingModeFurCutout"), GetLoc("sRenderingModeGem"));
            sRenderingModeList = new[]{GetLoc("sRenderingModeOpaque"), GetLoc("sRenderingModeCutout"), GetLoc("sRenderingModeTransparent"), GetLoc("sRenderingModeRefraction"), GetLoc("sRenderingModeRefractionBlur"), GetLoc("sRenderingModeFur"), GetLoc("sRenderingModeFurCutout"), GetLoc("sRenderingModeFurTwoPass"), GetLoc("sRenderingModeGem")};
            sRenderingModeListLite = new[]{GetLoc("sRenderingModeOpaque"), GetLoc("sRenderingModeCutout"), GetLoc("sRenderingModeTransparent")};
            sTransparentModeList = new[]{GetLoc("sTransparentModeNormal"), GetLoc("sTransparentModeOnePass"), GetLoc("sTransparentModeTwoPass")};
            sOutlineVertexColorUsages = BuildParams(GetLoc("sVertexColor"), GetLoc("sNone"), GetLoc("sVertexR2Width"), GetLoc("sVertexRGBA2Normal"));
            sShadowMaskTypes = BuildParams(GetLoc("sMaskType"), GetLoc("sStrength"), GetLoc("sFlat"));
            colorRGBAContent = new GUIContent(GetLoc("sColor"), GetLoc("sTextureRGBA"));
            colorAlphaRGBAContent = new GUIContent(GetLoc("sColorAlpha"), GetLoc("sTextureRGBA"));
            maskBlendContent = new GUIContent(GetLoc("sMask"), GetLoc("sBlendR"));
            colorMaskRGBAContent = new GUIContent(GetLoc("sColor") + " / " + GetLoc("sMask"), GetLoc("sTextureRGBA"));
            alphaMaskContent = new GUIContent(GetLoc("sAlphaMask"), GetLoc("sAlphaR"));
            maskStrengthContent = new GUIContent(GetLoc("sStrengthMask"), GetLoc("sStrengthR"));
            normalMapContent = new GUIContent(GetLoc("sNormalMap"), GetLoc("sNormalRGB"));
            noiseMaskContent = new GUIContent(GetLoc("sNoise"), GetLoc("sNoiseR"));
            adjustMaskContent = new GUIContent(GetLoc("sColorAdjustMask"), GetLoc("sBlendR"));
            matcapContent = new GUIContent(GetLoc("sMatCap"), GetLoc("sTextureRGBA"));
            gradationContent = new GUIContent(GetLoc("sGradation"), GetLoc("sTextureRGBA"));
            gradSpeedContent = new GUIContent(GetLoc("sGradTexSpeed"), GetLoc("sTextureRGBA"));
            smoothnessContent = new GUIContent(GetLoc("sSmoothness"), GetLoc("sSmoothnessR"));
            metallicContent = new GUIContent(GetLoc("sMetallic"), GetLoc("sMetallicR"));
            parallaxContent = new GUIContent(GetLoc("sParallax"), GetLoc("sParallaxR"));
            customMaskContent = new GUIContent(GetLoc("sMask"), "");
            shadow1stColorRGBAContent = new GUIContent(GetLoc("sShadow1stColor"), GetLoc("sTextureRGBA"));
            shadow2ndColorRGBAContent = new GUIContent(GetLoc("sShadow2ndColor"), GetLoc("sTextureRGBA"));
            shadow3rdColorRGBAContent = new GUIContent(GetLoc("sShadow3rdColor"), GetLoc("sTextureRGBA"));
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Rendering Pipeline
        #region
        public static void RewriteShaderRP()
        {
            string[] shaderFolderPaths = GetShaderFolderPaths();
            string[] shaderGuids = AssetDatabase.FindAssets("t:shader", shaderFolderPaths);
            lilRenderPipeline RP = RPReader.GetRP();
            Array.ForEach(shaderGuids, shaderGuid => RewriteShaderRP(GUIDToPath(shaderGuid), RP));
            RewriteShaderRP(GetShaderPipelinePath(), RP);
        }

        private static void RewriteShaderRP(string shaderPath, lilRenderPipeline RP)
        {
            string path = shaderPath;
            StreamReader sr = new StreamReader(path);
            string s = sr.ReadToEnd();
            sr.Close();
            RewriteBRP(ref s, RP == lilRenderPipeline.BRP);
            RewriteLWRP(ref s, RP == lilRenderPipeline.LWRP);
            RewriteURP(ref s, RP == lilRenderPipeline.URP);
            RewriteHDRP(ref s, RP == lilRenderPipeline.HDRP);
            StreamWriter sw = new StreamWriter(path,false);
            sw.Write(s);
            sw.Close();
        }

        private static void RewriteBRP(ref string s, bool isActive)
        {
            if(isActive)
            {
                s = s.Replace(
                    "// BRP Start\r\n/*",
                    "// BRP Start\r\n//");
                s = s.Replace(
                    "*/\r\n// BRP End",
                    "//\r\n// BRP End");
            }
            else
            {
                s = s.Replace(
                    "// BRP Start\r\n//",
                    "// BRP Start\r\n/*");
                s = s.Replace(
                    "//\r\n// BRP End",
                    "*/\r\n// BRP End");
            }
        }

        private static void RewriteLWRP(ref string s, bool isActive)
        {
            if(isActive)
            {
                s = s.Replace(
                    "// LWRP Start\r\n/*",
                    "// LWRP Start\r\n//");
                s = s.Replace(
                    "*/\r\n// LWRP End",
                    "//\r\n// LWRP End");
            }
            else
            {
                s = s.Replace(
                    "// LWRP Start\r\n//",
                    "// LWRP Start\r\n/*");
                s = s.Replace(
                    "//\r\n// LWRP End",
                    "*/\r\n// LWRP End");
            }
        }

        private static void RewriteURP(ref string s, bool isActive)
        {
            if(isActive)
            {
                s = s.Replace(
                    "// URP Start\r\n/*",
                    "// URP Start\r\n//");
                s = s.Replace(
                    "*/\r\n// URP End",
                    "//\r\n// URP End");
            }
            else
            {
                s = s.Replace(
                    "// URP Start\r\n//",
                    "// URP Start\r\n/*");
                s = s.Replace(
                    "//\r\n// URP End",
                    "*/\r\n// URP End");
            }
        }

        private static void RewriteHDRP(ref string s, bool isActive)
        {
            if(isActive)
            {
                s = s.Replace(
                    "// HDRP Start\r\n/*",
                    "// HDRP Start\r\n//");
                s = s.Replace(
                    "*/\r\n// HDRP End",
                    "//\r\n// HDRP End");
            }
            else
            {
                s = s.Replace(
                    "// HDRP Start\r\n//",
                    "// HDRP Start\r\n/*");
                s = s.Replace(
                    "//\r\n// HDRP End",
                    "*/\r\n// HDRP End");
            }
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Shader Setting
        #region
        public static void InitializeShaderSetting(ref lilToonSetting shaderSetting)
        {
            if(shaderSetting != null) return;
            string shaderSettingPath = GetShaderSettingPath();
            shaderSetting = AssetDatabase.LoadAssetAtPath<lilToonSetting>(shaderSettingPath);
            if(shaderSetting == null)
            {
                foreach(string guid in AssetDatabase.FindAssets("t:lilToonSetting"))
                {
                    string path = GUIDToPath(guid);
                    var shaderSettingOld = AssetDatabase.LoadAssetAtPath<lilToonSetting>(path);
                    shaderSetting = UnityEngine.Object.Instantiate(shaderSettingOld);
                    if(shaderSetting != null)
                    {
                        Debug.Log("[lilToon] Migrate settings from: " + path);
                        TurnOnAllShaderSetting(ref shaderSetting);
                        AssetDatabase.CreateAsset(shaderSetting, shaderSettingPath);
                        ApplyShaderSetting(shaderSetting);
                        AssetDatabase.Refresh();
                        return;
                    }
                }
                shaderSetting = ScriptableObject.CreateInstance<lilToonSetting>();
                AssetDatabase.CreateAsset(shaderSetting, shaderSettingPath);
                AssetDatabase.Refresh();
            }
        }

        public static void TurnOffAllShaderSetting(ref lilToonSetting shaderSetting)
        {
            if(shaderSetting == null) return;
            shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV = false;
            shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION = false;
            shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP = false;
            shaderSetting.LIL_FEATURE_MAIN2ND = false;
            shaderSetting.LIL_FEATURE_MAIN3RD = false;
            shaderSetting.LIL_FEATURE_DECAL = false;
            shaderSetting.LIL_FEATURE_ANIMATE_DECAL = false;
            shaderSetting.LIL_FEATURE_LAYER_DISSOLVE = false;
            shaderSetting.LIL_FEATURE_ALPHAMASK = false;
            shaderSetting.LIL_FEATURE_SHADOW = false;
            shaderSetting.LIL_FEATURE_SHADOW_3RD = false;
            shaderSetting.LIL_FEATURE_RECEIVE_SHADOW = false;
            shaderSetting.LIL_FEATURE_EMISSION_1ST = false;
            shaderSetting.LIL_FEATURE_EMISSION_2ND = false;
            shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV = false;
            shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV = false;
            shaderSetting.LIL_FEATURE_EMISSION_GRADATION = false;
            shaderSetting.LIL_FEATURE_NORMAL_1ST = false;
            shaderSetting.LIL_FEATURE_NORMAL_2ND = false;
            shaderSetting.LIL_FEATURE_ANISOTROPY = false;
            shaderSetting.LIL_FEATURE_REFLECTION = false;
            shaderSetting.LIL_FEATURE_MATCAP = false;
            shaderSetting.LIL_FEATURE_MATCAP_2ND = false;
            shaderSetting.LIL_FEATURE_RIMLIGHT = false;
            shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION = false;
            shaderSetting.LIL_FEATURE_GLITTER = false;
            shaderSetting.LIL_FEATURE_BACKLIGHT = false;
            shaderSetting.LIL_FEATURE_PARALLAX = false;
            shaderSetting.LIL_FEATURE_POM = false;
            //shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER = false;
            shaderSetting.LIL_FEATURE_DISTANCE_FADE = false;
            shaderSetting.LIL_FEATURE_AUDIOLINK = false;
            shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX = false;
            shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL = false;
            shaderSetting.LIL_FEATURE_DISSOLVE = false;
            shaderSetting.LIL_FEATURE_ENCRYPTION = ExistsEncryption();
            shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV = false;
            shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION = false;
            shaderSetting.LIL_FEATURE_FUR_COLLISION = false;
            shaderSetting.LIL_FEATURE_TEX_LAYER_MASK = false;
            shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE = false;
            shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR = false;
            shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER = false;
            shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH = false;
            shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST = false;
            shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND = false;
            shaderSetting.LIL_FEATURE_TEX_SHADOW_3RD = false;
            shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK = false;
            shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK = false;
            shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS = false;
            shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC = false;
            shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR = false;
            shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK = false;
            shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP = false;
            shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR = false;
            shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE = false;
            shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR = false;
            shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH = false;
            shaderSetting.LIL_FEATURE_TEX_OUTLINE_NORMAL = false;
            shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL = false;
            shaderSetting.LIL_FEATURE_TEX_FUR_MASK = false;
            shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH = false;
            shaderSetting.LIL_FEATURE_TEX_TESSELLATION = false;
            EditorUtility.SetDirty(shaderSetting);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void TurnOnAllShaderSetting(ref lilToonSetting shaderSetting)
        {
            if(shaderSetting == null) return;
            shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV = true;
            shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION = true;
            shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP = true;
            shaderSetting.LIL_FEATURE_MAIN2ND = true;
            shaderSetting.LIL_FEATURE_MAIN3RD = true;
            shaderSetting.LIL_FEATURE_DECAL = true;
            shaderSetting.LIL_FEATURE_ANIMATE_DECAL = true;
            shaderSetting.LIL_FEATURE_LAYER_DISSOLVE = true;
            shaderSetting.LIL_FEATURE_ALPHAMASK = true;
            shaderSetting.LIL_FEATURE_SHADOW = true;
            shaderSetting.LIL_FEATURE_SHADOW_3RD = true;
            shaderSetting.LIL_FEATURE_RECEIVE_SHADOW = true;
            shaderSetting.LIL_FEATURE_EMISSION_1ST = true;
            shaderSetting.LIL_FEATURE_EMISSION_2ND = true;
            shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV = true;
            shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV = true;
            shaderSetting.LIL_FEATURE_EMISSION_GRADATION = true;
            shaderSetting.LIL_FEATURE_NORMAL_1ST = true;
            shaderSetting.LIL_FEATURE_NORMAL_2ND = true;
            shaderSetting.LIL_FEATURE_ANISOTROPY = true;
            shaderSetting.LIL_FEATURE_REFLECTION = true;
            shaderSetting.LIL_FEATURE_MATCAP = true;
            shaderSetting.LIL_FEATURE_MATCAP_2ND = true;
            shaderSetting.LIL_FEATURE_RIMLIGHT = true;
            shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION = true;
            shaderSetting.LIL_FEATURE_GLITTER = true;
            shaderSetting.LIL_FEATURE_BACKLIGHT = true;
            shaderSetting.LIL_FEATURE_PARALLAX = true;
            shaderSetting.LIL_FEATURE_POM = true;
            //shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER = true;
            shaderSetting.LIL_FEATURE_DISTANCE_FADE = true;
            shaderSetting.LIL_FEATURE_AUDIOLINK = true;
            shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX = true;
            shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL = true;
            shaderSetting.LIL_FEATURE_DISSOLVE = true;
            shaderSetting.LIL_FEATURE_ENCRYPTION = ExistsEncryption();
            shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV = true;
            shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION = true;
            shaderSetting.LIL_FEATURE_FUR_COLLISION = true;
            shaderSetting.LIL_FEATURE_TEX_LAYER_MASK = true;
            shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE = true;
            shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR = true;
            shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER = true;
            shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH = true;
            shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST = true;
            shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND = true;
            shaderSetting.LIL_FEATURE_TEX_SHADOW_3RD = true;
            shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK = true;
            shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK = true;
            shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS = true;
            shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC = true;
            shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR = true;
            shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK = true;
            shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP = true;
            shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR = true;
            shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE = true;
            shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR = true;
            shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH = true;
            shaderSetting.LIL_FEATURE_TEX_OUTLINE_NORMAL = true;
            shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL = true;
            shaderSetting.LIL_FEATURE_TEX_FUR_MASK = true;
            shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH = true;
            shaderSetting.LIL_FEATURE_TEX_TESSELLATION = true;
        }

        public static void ApplyShaderSetting(lilToonSetting shaderSetting, string reportTitle = null)
        {
            EditorUtility.SetDirty(shaderSetting);
            AssetDatabase.SaveAssets();
            string shaderSettingString = BuildShaderSettingString(shaderSetting, true);
            string shaderSettingStringBuf = "";
            string shaderSettingHLSLPath = GetShaderSettingHLSLPath();
            if(File.Exists(shaderSettingHLSLPath))
            {
                StreamReader sr = new StreamReader(shaderSettingHLSLPath);
                shaderSettingStringBuf = sr.ReadToEnd();
                sr.Close();
            }

            if(shaderSettingString != shaderSettingStringBuf)
            {
                PackageVersionInfos version = RPReader.GetRPInfos();
                StreamWriter sw = new StreamWriter(shaderSettingHLSLPath,false);
                sw.Write(shaderSettingString);
                sw.Close();
                string[] shaderFolderPaths = GetShaderFolderPaths();
                bool isShadowReceive = (shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW) || shaderSetting.LIL_FEATURE_BACKLIGHT;
                var folders = new List<string>
                {
                    GetShaderFolderPath()
                };
                foreach (string shaderGuid in AssetDatabase.FindAssets("t:shader", shaderFolderPaths))
                {
                    string shaderPath = GUIDToPath(shaderGuid);
                    lilShaderRewriter.RewriteReceiveShadow(shaderPath, isShadowReceive);
                    lilShaderRewriter.RewriteForwardAdd(shaderPath, shaderSetting.LIL_OPTIMIZE_USE_FORWARDADD);
                    lilShaderRewriter.RewriteVertexLight(shaderPath, shaderSetting.LIL_OPTIMIZE_USE_VERTEXLIGHT);
                    lilShaderRewriter.RewriteLightmap(shaderPath, shaderSetting.LIL_OPTIMIZE_USE_LIGHTMAP);
                    lilShaderRewriter.RewriteRPPass(shaderPath, version);
                }
                foreach(string shaderGuid in AssetDatabase.FindAssets("t:shader"))
                {
                    string shaderPath = GUIDToPath(shaderGuid);
                    if(!shaderPath.Contains(".lilcontainer")) continue;
                    string folder = Path.GetDirectoryName(shaderPath);
                    if(!folders.Contains(folder)) folders.Add(folder);
                }
                foreach(string folder in folders)
                {
                    AssetDatabase.ImportAsset(folder, ImportAssetOptions.ImportRecursive);
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if(!string.IsNullOrEmpty(reportTitle))
            {
                Debug.Log(reportTitle + "\r\n" + shaderSettingString);
            }
        }

        public static string BuildShaderSettingString(lilToonSetting shaderSetting, bool isFile)
        {
            StringBuilder sb = new StringBuilder();
            if(isFile) sb.Append("#ifndef LIL_SETTING_INCLUDED\r\n#define LIL_SETTING_INCLUDED\r\n\r\n");
            if(shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV) sb.Append("#define LIL_FEATURE_ANIMATE_MAIN_UV\r\n");
            if(shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION) sb.Append("#define LIL_FEATURE_MAIN_TONE_CORRECTION\r\n");
            if(shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP) sb.Append("#define LIL_FEATURE_MAIN_GRADATION_MAP\r\n");
            if(shaderSetting.LIL_FEATURE_MAIN2ND) sb.Append("#define LIL_FEATURE_MAIN2ND\r\n");
            if(shaderSetting.LIL_FEATURE_MAIN3RD) sb.Append("#define LIL_FEATURE_MAIN3RD\r\n");
            if(shaderSetting.LIL_FEATURE_MAIN2ND || shaderSetting.LIL_FEATURE_MAIN3RD)
            {
                if(shaderSetting.LIL_FEATURE_DECAL) sb.Append("#define LIL_FEATURE_DECAL\r\n");
                if(shaderSetting.LIL_FEATURE_ANIMATE_DECAL) sb.Append("#define LIL_FEATURE_ANIMATE_DECAL\r\n");
                if(shaderSetting.LIL_FEATURE_TEX_LAYER_MASK) sb.Append("#define LIL_FEATURE_TEX_LAYER_MASK\r\n");
                if(shaderSetting.LIL_FEATURE_LAYER_DISSOLVE)
                {
                    sb.Append("#define LIL_FEATURE_LAYER_DISSOLVE\r\n");
                    if(shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE) sb.Append("#define LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE\r\n");
                }
            }

            if(shaderSetting.LIL_FEATURE_ALPHAMASK) sb.Append("#define LIL_FEATURE_ALPHAMASK\r\n");

            if(shaderSetting.LIL_FEATURE_SHADOW)
            {
                sb.Append("#define LIL_FEATURE_SHADOW\r\n");
                if(shaderSetting.LIL_FEATURE_RECEIVE_SHADOW) sb.Append("#define LIL_FEATURE_RECEIVE_SHADOW\r\n");
                if(shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR) sb.Append("#define LIL_FEATURE_TEX_SHADOW_BLUR\r\n");
                if(shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER) sb.Append("#define LIL_FEATURE_TEX_SHADOW_BORDER\r\n");
                if(shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH) sb.Append("#define LIL_FEATURE_TEX_SHADOW_STRENGTH\r\n");
                if(shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST) sb.Append("#define LIL_FEATURE_TEX_SHADOW_1ST\r\n");
                if(shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND) sb.Append("#define LIL_FEATURE_TEX_SHADOW_2ND\r\n");
                if(shaderSetting.LIL_FEATURE_SHADOW_3RD)
                {
                    sb.Append("#define LIL_FEATURE_SHADOW_3RD\r\n");
                    if(shaderSetting.LIL_FEATURE_TEX_SHADOW_3RD) sb.Append("#define LIL_FEATURE_TEX_SHADOW_3RD\r\n");
                }
            }

            if(shaderSetting.LIL_FEATURE_EMISSION_1ST) sb.Append("#define LIL_FEATURE_EMISSION_1ST\r\n");
            if(shaderSetting.LIL_FEATURE_EMISSION_2ND) sb.Append("#define LIL_FEATURE_EMISSION_2ND\r\n");
            if(shaderSetting.LIL_FEATURE_EMISSION_1ST || shaderSetting.LIL_FEATURE_EMISSION_2ND)
            {
                if(shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV) sb.Append("#define LIL_FEATURE_ANIMATE_EMISSION_UV\r\n");
                if(shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK)
                {
                    sb.Append("#define LIL_FEATURE_TEX_EMISSION_MASK\r\n");
                    if(shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV) sb.Append("#define LIL_FEATURE_ANIMATE_EMISSION_MASK_UV\r\n");
                }
                if(shaderSetting.LIL_FEATURE_EMISSION_GRADATION) sb.Append("#define LIL_FEATURE_EMISSION_GRADATION\r\n");
            }
            if(shaderSetting.LIL_FEATURE_NORMAL_1ST) sb.Append("#define LIL_FEATURE_NORMAL_1ST\r\n");
            if(shaderSetting.LIL_FEATURE_NORMAL_2ND)
            {
                sb.Append("#define LIL_FEATURE_NORMAL_2ND\r\n");
                if(shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK) sb.Append("#define LIL_FEATURE_TEX_NORMAL_MASK\r\n");
            }
            if(shaderSetting.LIL_FEATURE_ANISOTROPY) sb.Append("#define LIL_FEATURE_ANISOTROPY\r\n");
            if(shaderSetting.LIL_FEATURE_REFLECTION)
            {
                sb.Append("#define LIL_FEATURE_REFLECTION\r\n");
                if(shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS) sb.Append("#define LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS\r\n");
                if(shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC) sb.Append("#define LIL_FEATURE_TEX_REFLECTION_METALLIC\r\n");
                if(shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR) sb.Append("#define LIL_FEATURE_TEX_REFLECTION_COLOR\r\n");
            }
            if(shaderSetting.LIL_FEATURE_MATCAP) sb.Append("#define LIL_FEATURE_MATCAP\r\n");
            if(shaderSetting.LIL_FEATURE_MATCAP_2ND) sb.Append("#define LIL_FEATURE_MATCAP_2ND\r\n");
            if(shaderSetting.LIL_FEATURE_MATCAP || shaderSetting.LIL_FEATURE_MATCAP_2ND)
            {
                if(shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK) sb.Append("#define LIL_FEATURE_TEX_MATCAP_MASK\r\n");
                if(shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP) sb.Append("#define LIL_FEATURE_TEX_MATCAP_NORMALMAP\r\n");
            }
            if(shaderSetting.LIL_FEATURE_RIMLIGHT)
            {
                sb.Append("#define LIL_FEATURE_RIMLIGHT\r\n");
                if(shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR) sb.Append("#define LIL_FEATURE_TEX_RIMLIGHT_COLOR\r\n");
                if(shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION) sb.Append("#define LIL_FEATURE_RIMLIGHT_DIRECTION\r\n");
            }
            if(shaderSetting.LIL_FEATURE_GLITTER) sb.Append("#define LIL_FEATURE_GLITTER\r\n");
            if(shaderSetting.LIL_FEATURE_BACKLIGHT) sb.Append("#define LIL_FEATURE_BACKLIGHT\r\n");
            if(shaderSetting.LIL_FEATURE_PARALLAX)
            {
                sb.Append("#define LIL_FEATURE_PARALLAX\r\n");
                if(shaderSetting.LIL_FEATURE_POM) sb.Append("#define LIL_FEATURE_POM\r\n");
            }
            if(shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER) sb.Append("#define LIL_FEATURE_CLIPPING_CANCELLER\r\n");
            if(shaderSetting.LIL_FEATURE_DISTANCE_FADE) sb.Append("#define LIL_FEATURE_DISTANCE_FADE\r\n");
            if(shaderSetting.LIL_FEATURE_AUDIOLINK)
            {
                sb.Append("#define LIL_FEATURE_AUDIOLINK\r\n");
                if(shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX) sb.Append("#define LIL_FEATURE_AUDIOLINK_VERTEX\r\n");
                if(shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL) sb.Append("#define LIL_FEATURE_AUDIOLINK_LOCAL\r\n");
            }
            if(shaderSetting.LIL_FEATURE_DISSOLVE)
            {
                sb.Append("#define LIL_FEATURE_DISSOLVE\r\n");
                if(shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE) sb.Append("#define LIL_FEATURE_TEX_DISSOLVE_NOISE\r\n");
            }
            if(shaderSetting.LIL_FEATURE_ENCRYPTION) sb.Append("#define LIL_FEATURE_ENCRYPTION\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR)
            {
                sb.Append("#define LIL_FEATURE_TEX_OUTLINE_COLOR\r\n");
                if(shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION) sb.Append("#define LIL_FEATURE_OUTLINE_TONE_CORRECTION\r\n");
            }
            if(shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV) sb.Append("#define LIL_FEATURE_ANIMATE_OUTLINE_UV\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH) sb.Append("#define LIL_FEATURE_TEX_OUTLINE_WIDTH\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_OUTLINE_NORMAL) sb.Append("#define LIL_FEATURE_TEX_OUTLINE_NORMAL\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL) sb.Append("#define LIL_FEATURE_TEX_FUR_NORMAL\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_FUR_MASK) sb.Append("#define LIL_FEATURE_TEX_FUR_MASK\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH) sb.Append("#define LIL_FEATURE_TEX_FUR_LENGTH\r\n");
            if(shaderSetting.LIL_FEATURE_FUR_COLLISION) sb.Append("#define LIL_FEATURE_FUR_COLLISION\r\n");
            if(shaderSetting.LIL_OPTIMIZE_APPLY_SHADOW_FA) sb.Append("#define LIL_OPTIMIZE_APPLY_SHADOW_FA\r\n");
            if(shaderSetting.LIL_OPTIMIZE_USE_FORWARDADD) sb.Append("#define LIL_OPTIMIZE_USE_FORWARDADD\r\n");
            if(shaderSetting.LIL_OPTIMIZE_USE_VERTEXLIGHT) sb.Append("#define LIL_OPTIMIZE_USE_VERTEXLIGHT\r\n");
            if(shaderSetting.LIL_OPTIMIZE_USE_LIGHTMAP) sb.Append("#define LIL_OPTIMIZE_USE_LIGHTMAP\r\n");
            if(isFile) sb.Append("\r\n#endif");

            if(!isFile)
            {
                if(!(shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW) && !shaderSetting.LIL_FEATURE_BACKLIGHT)
                {
                    sb.Append("#pragma lil_skip_variants_shadows\r\n");
                }
                if(!shaderSetting.LIL_OPTIMIZE_USE_VERTEXLIGHT) sb.Append("#pragma lil_skip_variants_addlight\r\n");
                if(!shaderSetting.LIL_OPTIMIZE_USE_LIGHTMAP) sb.Append("#pragma lil_skip_variants_lightmaps\r\n");
            }
            return sb.ToString();
        }

        public static string BuildShaderSettingString(bool isFile)
        {
            lilToonSetting shaderSetting = null;
            InitializeShaderSetting(ref shaderSetting);
            if(shaderSetting == null) return "";
            return BuildShaderSettingString(shaderSetting, isFile);
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

        public static void ApplyShaderSettingOptimized()
        {
            lilToonSetting shaderSetting = null;
            InitializeShaderSetting(ref shaderSetting);
            if(shaderSetting == null) return;

            TurnOffAllShaderSetting(ref shaderSetting);

            // Get materials
            foreach(string guid in AssetDatabase.FindAssets("t:material"))
            {
                Material material = AssetDatabase.LoadAssetAtPath<Material>(GUIDToPath(guid));
                SetupShaderSettingFromMaterial(material, ref shaderSetting);
            }

            // Get animations
            foreach(string guid in AssetDatabase.FindAssets("t:animationclip"))
            {
                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(GUIDToPath(guid));
                SetupShaderSettingFromAnimationClip(clip, ref shaderSetting);
            }

            // Apply
            ApplyShaderSetting(shaderSetting, "[lilToon] PreprocessBuild");
            AssetDatabase.Refresh();
        }

        public static void SetShaderSettingBeforeBuild(GameObject gameObject)
        {
            if(File.Exists(postBuildTempPath)) return;
            File.Create(postBuildTempPath);

            lilToonSetting shaderSetting = null;
            InitializeShaderSetting(ref shaderSetting);
            if(shaderSetting == null) return;

            TurnOffAllShaderSetting(ref shaderSetting);

            // Get materials
            foreach(var renderer in gameObject.GetComponentsInChildren<Renderer>(true))
            {
                foreach(var material in renderer.sharedMaterials)
                {
                    SetupShaderSettingFromMaterial(material, ref shaderSetting);
                }
            }

            // Get animations
            foreach(var animator in gameObject.GetComponentsInChildren<Animator>(true))
            {
                if(animator.runtimeAnimatorController == null) continue;
                foreach(var clip in animator.runtimeAnimatorController.animationClips)
                {
                    SetupShaderSettingFromAnimationClip(clip, ref shaderSetting, true);
                }
            }
            #if VRC_SDK_VRCSDK3 && !UDON
                foreach(var descriptor in gameObject.GetComponentsInChildren<VRCAvatarDescriptor>(true))
                {
                    foreach(var layer in descriptor.specialAnimationLayers)
                    {
                        if(layer.animatorController == null) continue;
                        foreach(var clip in layer.animatorController.animationClips)
                        {
                            SetupShaderSettingFromAnimationClip(clip, ref shaderSetting, true);
                        }
                    }
                    if(descriptor.customizeAnimationLayers)
                    {
                        foreach(var layer in descriptor.baseAnimationLayers)
                        {
                            if(layer.animatorController == null) continue;
                            foreach(var clip in layer.animatorController.animationClips)
                            {
                                SetupShaderSettingFromAnimationClip(clip, ref shaderSetting, true);
                            }
                        }
                    }
                }
            #endif

            // Apply
            ApplyShaderSetting(shaderSetting, "[lilToon] PreprocessBuild");
            AssetDatabase.Refresh();
        }

        public static void SetShaderSettingBeforeBuild()
        {
            if(File.Exists(postBuildTempPath)) return;
            File.Create(postBuildTempPath);
            ApplyShaderSettingOptimized();
        }

        public static void SetShaderSettingAfterBuild()
        {
            if(!File.Exists(postBuildTempPath)) return;
            File.Delete(postBuildTempPath);
            lilToonSetting shaderSetting = null;
            InitializeShaderSetting(ref shaderSetting);
            if(shaderSetting == null) return;
            TurnOnAllShaderSetting(ref shaderSetting);
            ApplyShaderSetting(shaderSetting, "[lilToon] PostprocessBuild");
        }

        private static void OptimizationSettingGUI()
        {
            lilRenderPipeline RP = RPReader.GetRP();
            if(RP == lilRenderPipeline.BRP)
            {
                ToggleGUI(GetLoc("sSettingApplyShadowFA"), ref shaderSetting.LIL_OPTIMIZE_APPLY_SHADOW_FA);
                ToggleGUI(GetLoc("sSettingUseForwardAdd"), ref shaderSetting.LIL_OPTIMIZE_USE_FORWARDADD);
            }
            ToggleGUI(GetLoc("sSettingUseVertexLight"), ref shaderSetting.LIL_OPTIMIZE_USE_VERTEXLIGHT);
            ToggleGUI(GetLoc("sSettingUseLightmap"), ref shaderSetting.LIL_OPTIMIZE_USE_LIGHTMAP);
        }

        private static void DefaultValueSettingGUI()
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
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Initialize Editor Variable
        #region
        public static void InitializeShaders()
        {
            lts         = Shader.Find("lilToon");
            ltsc        = Shader.Find("Hidden/lilToonCutout");
            ltst        = Shader.Find("Hidden/lilToonTransparent");
            ltsot       = Shader.Find("Hidden/lilToonOnePassTransparent");
            ltstt       = Shader.Find("Hidden/lilToonTwoPassTransparent");

            ltso        = Shader.Find("Hidden/lilToonOutline");
            ltsco       = Shader.Find("Hidden/lilToonCutoutOutline");
            ltsto       = Shader.Find("Hidden/lilToonTransparentOutline");
            ltsoto      = Shader.Find("Hidden/lilToonOnePassTransparentOutline");
            ltstto      = Shader.Find("Hidden/lilToonTwoPassTransparentOutline");

            ltsoo       = Shader.Find("_lil/[Optional] lilToonOutlineOnly");
            ltscoo      = Shader.Find("_lil/[Optional] lilToonCutoutOutlineOnly");
            ltstoo      = Shader.Find("_lil/[Optional] lilToonTransparentOutlineOnly");

            ltstess     = Shader.Find("Hidden/lilToonTessellation");
            ltstessc    = Shader.Find("Hidden/lilToonTessellationCutout");
            ltstesst    = Shader.Find("Hidden/lilToonTessellationTransparent");
            ltstessot   = Shader.Find("Hidden/lilToonTessellationOnePassTransparent");
            ltstesstt   = Shader.Find("Hidden/lilToonTessellationTwoPassTransparent");

            ltstesso    = Shader.Find("Hidden/lilToonTessellationOutline");
            ltstessco   = Shader.Find("Hidden/lilToonTessellationCutoutOutline");
            ltstessto   = Shader.Find("Hidden/lilToonTessellationTransparentOutline");
            ltstessoto  = Shader.Find("Hidden/lilToonTessellationOnePassTransparentOutline");
            ltstesstto  = Shader.Find("Hidden/lilToonTessellationTwoPassTransparentOutline");

            ltsl        = Shader.Find("Hidden/lilToonLite");
            ltslc       = Shader.Find("Hidden/lilToonLiteCutout");
            ltslt       = Shader.Find("Hidden/lilToonLiteTransparent");
            ltslot      = Shader.Find("Hidden/lilToonLiteOnePassTransparent");
            ltsltt      = Shader.Find("Hidden/lilToonLiteTwoPassTransparent");

            ltslo       = Shader.Find("Hidden/lilToonLiteOutline");
            ltslco      = Shader.Find("Hidden/lilToonLiteCutoutOutline");
            ltslto      = Shader.Find("Hidden/lilToonLiteTransparentOutline");
            ltsloto     = Shader.Find("Hidden/lilToonLiteOnePassTransparentOutline");
            ltsltto     = Shader.Find("Hidden/lilToonLiteTwoPassTransparentOutline");

            ltsref      = Shader.Find("Hidden/lilToonRefraction");
            ltsrefb     = Shader.Find("Hidden/lilToonRefractionBlur");
            ltsfur      = Shader.Find("Hidden/lilToonFur");
            ltsfurc     = Shader.Find("Hidden/lilToonFurCutout");
            ltsfurtwo   = Shader.Find("Hidden/lilToonFurTwoPass");

            ltsgem      = Shader.Find("Hidden/lilToonGem");

            ltsfs       = Shader.Find("_lil/lilToonFakeShadow");

            ltsbaker    = Shader.Find("Hidden/ltsother_baker");
            ltspo       = Shader.Find("Hidden/ltspass_opaque");
            ltspc       = Shader.Find("Hidden/ltspass_cutout");
            ltspt       = Shader.Find("Hidden/ltspass_transparent");
            ltsptesso   = Shader.Find("Hidden/ltspass_tess_opaque");
            ltsptessc   = Shader.Find("Hidden/ltspass_tess_cutout");
            ltsptesst   = Shader.Find("Hidden/ltspass_tess_transparent");

            ltsm        = Shader.Find("_lil/lilToonMulti");
            ltsmo       = Shader.Find("Hidden/lilToonMultiOutline");
            ltsmref     = Shader.Find("Hidden/lilToonMultiRefraction");
            ltsmfur     = Shader.Find("Hidden/lilToonMultiFur");
            ltsmgem     = Shader.Find("Hidden/lilToonMultiGem");

            mtoon       = Shader.Find("VRM/MToon");
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Migration
        #region
        public static void MigrateMaterials()
        {
            InitializeShaders();
            foreach(string guid in AssetDatabase.FindAssets("t:material"))
            {
                Material material = AssetDatabase.LoadAssetAtPath<Material>(GUIDToPath(guid));
                MigrateMaterial(material);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void MigrateMaterial(Material material)
        {
            if(material.shader == null || !material.shader.name.Contains("lilToon")) return;
            int version = material.HasProperty("_lilToonVersion") ? (int)material.GetFloat("_lilToonVersion") : 0;
            if(version >= currentVersionValue) return;
            Debug.Log("[lilToon]Run migration: " + material.name);
            material.SetFloat("_lilToonVersion", currentVersionValue);

            // 1.2.7 -> 1.2.8
            if(version < 21)
            {
                if(material.shader.name.Contains("_lil/lilToonMulti"))
                {
                    int renderQueue = material.renderQueue;
                    material.shader = material.HasProperty("_UseOutline") && material.GetFloat("_UseOutline") != 0.0f ? ltsmo : ltsm;
                    material.renderQueue = renderQueue;
                }
            }
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // GUI
        #region
        public static GUIStyle InitializeBox(int border, int margin, int padding)
        {
            return new GUIStyle
                {
                    border = new RectOffset(border, border, border, border),
                    margin = new RectOffset(margin, margin, margin, margin),
                    padding = new RectOffset(padding, padding, padding, padding),
                    overflow = new RectOffset(0, 0, 0, 0)
                };
        }

        public static bool Foldout(string title, bool display)
        {
            return Foldout(title, "", display);
        }

        public static bool Foldout(string title, string help, bool display)
        {
            Rect rect = GUILayoutUtility.GetRect(16f, 20f, foldout);
			rect.width += 8f;
			rect.x -= 8f;
            GUI.Box(rect, new GUIContent(title, help), foldout);

            Event e = Event.current;

            Rect toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            if(e.type == EventType.Repaint) {
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
            }

            rect.width -= 24;
            if(e.type == EventType.MouseDown && rect.Contains(e.mousePosition)) {
                display = !display;
                e.Use();
            }

            return display;
        }

        public static void DrawLine()
        {
            EditorGUI.DrawRect(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, 1)), lineColor);
        }

        public static void DrawWebButton(string text, string URL)
        {
            Rect position = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect());
            GUIContent icon = EditorGUIUtility.IconContent("BuildSettings.Web.Small");
            icon.text = text;
            GUIStyle style = new GUIStyle(EditorStyles.label){padding = new RectOffset()};
            if(GUI.Button(position, icon, style)){
                Application.OpenURL(URL);
            }
        }

        public static void DrawSimpleFoldout(string label, ref bool condition, GUIStyle style, bool isCustomEditor = true)
        {
            EditorGUI.indentLevel++;
            Rect position = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(position, label, style);
            EditorGUI.indentLevel--;

            position.x += isCustomEditor ? 0 : 10;
            condition = EditorGUI.Foldout(position, condition, "");
        }

        public static void DrawSimpleFoldout(string label, ref bool condition, bool isCustomEditor = true)
        {
            DrawSimpleFoldout(label, ref condition, boldLabel, isCustomEditor);
        }

        public static void DrawSimpleFoldout(GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, ref bool condition, bool isCustomEditor = true)
        {
            EditorGUI.indentLevel++;
            Rect position = m_MaterialEditor.TexturePropertySingleLine(guiContent, textureName, rgba);
            EditorGUI.indentLevel--;

            position.x += isCustomEditor ? 0 : 10;
            condition = EditorGUI.Foldout(position, condition, "");
        }

        public static void DrawSimpleFoldout(GUIContent guiContent, MaterialProperty textureName, ref bool condition, bool isCustomEditor = true)
        {
            EditorGUI.indentLevel++;
            Rect position = m_MaterialEditor.TexturePropertySingleLine(guiContent, textureName);
            EditorGUI.indentLevel--;

            position.x += isCustomEditor ? 0 : 10;
            condition = EditorGUI.Foldout(position, condition, "");
        }

        private static void InitializeGUIStyles()
        {
            wrapLabel = new GUIStyle(GUI.skin.label){wordWrap = true};
            boldLabel = new GUIStyle(GUI.skin.label){fontStyle = FontStyle.Bold};
            foldout = new GUIStyle("ShurikenModuleTitle")
            {
                fontSize = EditorStyles.label.fontSize,
                border = new RectOffset(15, 7, 4, 4),
                contentOffset = new Vector2(20f, -2f),
                fixedHeight = 22
            };
            if(EditorGUIUtility.isProSkin)
            {
                boxOuter.normal.background      = (Texture2D)EditorGUIUtility.Load(GetGUIBoxOutDarkPath());
                boxInnerHalf.normal.background  = (Texture2D)EditorGUIUtility.Load(GetGUIBoxInHalfDarkPath());
                boxInner.normal.background      = (Texture2D)EditorGUIUtility.Load(GetGUIBoxInDarkPath());
                customBox.normal.background     = (Texture2D)EditorGUIUtility.Load(GetGUICustomBoxDarkPath());
                customToggleFont = EditorStyles.label;
            }
            else
            {
                boxOuter.normal.background      = (Texture2D)EditorGUIUtility.Load(GetGUIBoxOutLightPath());
                boxInnerHalf.normal.background  = (Texture2D)EditorGUIUtility.Load(GetGUIBoxInHalfLightPath());
                boxInner.normal.background      = (Texture2D)EditorGUIUtility.Load(GetGUIBoxInLightPath());
                customBox.normal.background     = (Texture2D)EditorGUIUtility.Load(GetGUICustomBoxLightPath());
                customToggleFont = new GUIStyle();
                customToggleFont.normal.textColor = Color.white;
                customToggleFont.contentOffset = new Vector2(2f,0f);
            }
        }

        private static void DrawHelpButton(string helpAnchor)
        {
            Rect position = GUILayoutUtility.GetLastRect();
            position.x += position.width - 24;
            position.width = 24;
            if(GUI.Button(position, EditorGUIUtility.IconContent("_Help"), middleButton)){
                Application.OpenURL(GetLoc("sManualURL") + helpAnchor);
            }
        }

        private static void DrawWebPages()
        {
            VersionCheck();
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label){fontStyle = FontStyle.Bold};
            string versionLabel = "lilToon " + currentVersionName;
            if(latestVersion != null && latestVersion.latest_vertion_name != null && latestVersion.latest_vertion_value > currentVersionValue)
            {
                versionLabel = "[Update] lilToon " + currentVersionName + " -> " + latestVersion.latest_vertion_name;
                labelStyle.normal.textColor = Color.red;
            }
            
            DrawSimpleFoldout(versionLabel, ref edSet.isShowWebPages, labelStyle, isCustomEditor);
            if(edSet.isShowWebPages)
            {
                EditorGUI.indentLevel++;
                DrawWebButton("BOOTH", boothURL);
                DrawWebButton("GitHub", githubURL);
                EditorGUI.indentLevel--;
            }
        }

        private static void DrawHelpPages()
        {
            DrawSimpleFoldout(GetLoc("sHelp"), ref edSet.isShowHelpPages, isCustomEditor);
            if(edSet.isShowHelpPages)
            {
                EditorGUI.indentLevel++;
                DrawWebButton(GetLoc("sCommonProblems"), GetLoc("sReadmeURL") + GetLoc("sReadmeAnchorProblem"));
                EditorGUI.indentLevel--;
            }
        }

        private static void DrawShaderTypeWarn(Material material)
        {
            if(!isMultiVariants && material.shader.name.Contains("Overlay") && AutoFixHelpBox(GetLoc("sHelpSelectOverlay")))
            {
                material.shader = lts;
            }
        }

        private static void ToggleGUI(string label, ref bool value)
        {
            value = EditorGUILayout.ToggleLeft(label, value);
        }

        private static bool AutoFixHelpBox(string message)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUILayout.Label(EditorGUIUtility.IconContent("console.warnicon"), GUILayout.ExpandWidth(false));
                GUILayout.Space(-EditorStyles.label.fontSize);
                GUILayout.BeginVertical();
                GUILayout.Label(message, EditorStyles.wordWrappedMiniLabel);
                    GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        bool pressed = GUILayout.Button(GetLoc("sFixNow"));
                    GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            return pressed;
        }

        private static void SelectEditorMode()
        {
            string[] sEditorModeList = {GetLoc("sEditorModeSimple"),GetLoc("sEditorModeAdvanced"),GetLoc("sEditorModePreset"),GetLoc("sEditorModeShaderSetting")};
            edSet.editorMode = (EditorMode)GUILayout.Toolbar((int)edSet.editorMode, sEditorModeList);
        }

        private void DrawMenuButton(string helpAnchor, lilPropertyBlock propertyBlock)
        {
            Rect position = GUILayoutUtility.GetLastRect();
            position.x += position.width - 24;
            position.width = 24;

            if(GUI.Button(position, EditorGUIUtility.IconContent("_Popup"), middleButton))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent(GetLoc("sCopy")),               false, CopyProperties,  propertyBlock);
                menu.AddItem(new GUIContent(GetLoc("sPaste")),              false, PasteProperties, new lilPropertyBlockData{propertyBlock = propertyBlock, shouldCopyTex = false});
                menu.AddItem(new GUIContent(GetLoc("sPasteWithTexture")),   false, PasteProperties, new lilPropertyBlockData{propertyBlock = propertyBlock, shouldCopyTex = true});
                #if UNITY_2019_3_OR_NEWER
                    menu.AddItem(new GUIContent(GetLoc("sReset")),              false, ResetProperties, propertyBlock);
                #endif
                menu.AddItem(new GUIContent(GetLoc("sOpenManual")),         false, OpenHelpPage,    helpAnchor);
                menu.ShowAsContext();
            }
        }

        private void OpenHelpPage(object helpAnchor)
        {
            Application.OpenURL(GetLoc("sManualURL") + helpAnchor);
        }

        private void DrawVRCFallbackGUI(Material material)
        {
            #if VRC_SDK_VRCSDK2 || VRC_SDK_VRCSDK3 || VRC_SDK_VRCSDK4
                edSet.isShowVRChat = Foldout("VRChat", "VRChat", edSet.isShowVRChat);
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
                if(isnormal && EditorButton(GetLoc("sOptimizeForEvents"))) RemoveUnusedTexture(material);
            #endif
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Editor
        #region
        public static void ApplyEditorSettingTemp()
        {
            if(string.IsNullOrEmpty(edSet.languageNames))
            {
                if(!File.Exists(editorSettingTempPath))
                {
                    return;
                }
                StreamReader sr = new StreamReader(editorSettingTempPath);
                string s = sr.ReadToEnd();
                sr.Close();
                if(!string.IsNullOrEmpty(s))
                {
                    EditorJsonUtility.FromJsonOverwrite(s,edSet);
                }
            }
        }

        public static void SaveEditorSettingTemp()
        {
            StreamWriter sw = new StreamWriter(editorSettingTempPath,false);
            sw.Write(EditorJsonUtility.ToJson(edSet));
            sw.Close();
        }

        private static void VersionCheck()
        {
            if(string.IsNullOrEmpty(latestVersion.latest_vertion_name))
            {
                if(File.Exists(versionInfoTempPath))
                {
                    StreamReader sr = new StreamReader(versionInfoTempPath);
                    string s = sr.ReadToEnd();
                    sr.Close();
                    if(!string.IsNullOrEmpty(s) && s.Contains("latest_vertion_name") && s.Contains("latest_vertion_value"))
                    {
                        EditorJsonUtility.FromJsonOverwrite(s,latestVersion);
                        return;
                    }
                }
                latestVersion.latest_vertion_name = currentVersionName;
                latestVersion.latest_vertion_value = currentVersionValue;
                return;
            }
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

            isUseAlpha =
                renderingModeBuf == RenderingMode.Cutout ||
                renderingModeBuf == RenderingMode.Transparent ||
                renderingModeBuf == RenderingMode.Fur ||
                renderingModeBuf == RenderingMode.FurCutout ||
                renderingModeBuf == RenderingMode.FurTwoPass ||
                (isMulti && transparentModeMat.floatValue != 0.0f && transparentModeMat.floatValue != 3.0f && transparentModeMat.floatValue != 6.0f);

            if(isMulti)
            {
                isCutout = transparentModeMat.floatValue == 1.0f || transparentModeMat.floatValue == 5.0f;
                isTransparent = transparentModeMat.floatValue == 2.0f;
            }
        }

        private lilMaterialProperty[] AllProperties()
        {
            return new[]
            {
                transparentModeMat,
                asOverlay,
                invisible,
                asUnlit,
                cutoff,
                subpassCutoff,
                flipNormal,
                shiftBackfaceUV,
                backfaceForceShadow,
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
                triMask,
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
                stencilRef,
                stencilReadMask,
                stencilWriteMask,
                stencilComp,
                stencilPass,
                stencilFail,
                stencilZFail,
                offsetFactor,
                offsetUnits,
                colorMask,
                alphaToMask,
                lilShadowCasterBias,
                mainColor,
                mainTex,
                mainTexHSVG,
                mainTex_ScrollRotate,
                mainGradationStrength,
                mainGradationTex,
                mainColorAdjustMask,
                useMain2ndTex,
                mainColor2nd,
                main2ndTex,
                main2ndTex_UVMode,
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
                useBumpMap,
                bumpMap,
                bumpScale,
                useBump2ndMap,
                bump2ndMap,
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
                matcapMul,
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
                matcap2ndMul,
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
                glitterMainStrength,
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
                outlineColor,
                outlineTex,
                outlineTex_ScrollRotate,
                outlineTexHSVG,
                outlineLitColor,
                outlineLitApplyTex,
                outlineLitScale,
                outlineLitOffset,
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
                outlineStencilRef,
                outlineStencilReadMask,
                outlineStencilWriteMask,
                outlineStencilComp,
                outlineStencilPass,
                outlineStencilFail,
                outlineStencilZFail,
                outlineOffsetFactor,
                outlineOffsetUnits,
                outlineColorMask,
                outlineAlphaToMask,
                useParallax,
                usePOM,
                parallaxMap,
                parallax,
                parallaxOffset,
                distanceFadeColor,
                distanceFade,
                useClippingCanceller,
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
                furStencilRef,
                furStencilReadMask,
                furStencilWriteMask,
                furStencilComp,
                furStencilPass,
                furStencilFail,
                furStencilZFail,
                furOffsetFactor,
                furOffsetUnits,
                furColorMask,
                furAlphaToMask,
                tessEdge,
                tessStrength,
                tessShrink,
                tessFactorMax,
                gemChromaticAberration,
                gemEnvContrast,
                gemEnvColor,
                gemParticleLoop,
                gemParticleColor,
                gemVRParallaxStrength,
                fakeShadowVector
            };
        }

        private void CopyProperties(lilPropertyBlock propertyBlock)
        {
            SetPropertyBlock();
            foreach(lilMaterialProperty prop in AllProperties())
            {
                foreach(lilPropertyBlock block in prop.blocks)
                {
                    if(block == propertyBlock && prop.p != null) copiedProperties[prop.name] = prop.p;
                }
            }
        }

        private void CopyProperties(object obj)
        {
            CopyProperties((lilPropertyBlock)obj);
        }

        private void PasteProperties(lilPropertyBlock propertyBlock, bool shouldCopyTex)
        {
            SetPropertyBlock();
            foreach(lilMaterialProperty prop in AllProperties())
            {
                if(!shouldCopyTex && prop.isTexture)
                {
                    Debug.Log("Skip Texture");
                    continue;
                }
                foreach(lilPropertyBlock block in prop.blocks)
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

        private void PasteProperties(object obj)
        {
            lilPropertyBlockData propertyBlockData = (lilPropertyBlockData)obj;
            PasteProperties(propertyBlockData.propertyBlock, propertyBlockData.shouldCopyTex);
        }

        private void ResetProperties(lilPropertyBlock propertyBlock)
        {
            #if UNITY_2019_3_OR_NEWER
            SetPropertyBlock();
            foreach(lilMaterialProperty prop in AllProperties())
            {
                foreach(lilPropertyBlock block in prop.blocks)
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

        private void ResetProperties(object obj)
        {
            ResetProperties((lilPropertyBlock)obj);
        }

        private void ApplyLightingPreset(lilLightingPreset lightingPreset)
        {
            switch(lightingPreset)
            {
                case lilLightingPreset.Default:
                    if(asUnlit.p != null) asUnlit.floatValue = shaderSetting.defaultAsUnlit;
                    if(vertexLightStrength.p != null) vertexLightStrength.floatValue = shaderSetting.defaultVertexLightStrength;
                    if(lightMinLimit.p != null) lightMinLimit.floatValue = shaderSetting.defaultLightMinLimit;
                    if(lightMaxLimit.p != null) lightMaxLimit.floatValue = shaderSetting.defaultLightMaxLimit;
                    if(beforeExposureLimit.p != null) beforeExposureLimit.floatValue = shaderSetting.defaultBeforeExposureLimit;
                    if(monochromeLighting.p != null) monochromeLighting.floatValue = shaderSetting.defaultMonochromeLighting;
                    if(shadowEnvStrength.p != null) shadowEnvStrength.floatValue = 0.0f;
                    if(lilDirectionalLightStrength.p != null) lilDirectionalLightStrength.floatValue = shaderSetting.defaultlilDirectionalLightStrength;
                    break;
                case lilLightingPreset.SemiMonochrome:
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

        private static bool ExistsEncryption()
        {
            return !string.IsNullOrEmpty(GetAvatarEncryptionPath());
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Material Setup
        #region
        public static void SetupMaterialWithRenderingMode(Material material, RenderingMode renderingMode, TransparentMode transparentMode, bool isoutl, bool islite, bool isstencil, bool istess, bool ismulti)
        {
            if(isMultiVariants) return;
            RenderingMode rend = renderingMode;
            lilRenderPipeline RP = RPReader.GetRP();
            if(ismulti)
            {
                float tpmode = material.GetFloat("_TransparentMode");
                switch((int)tpmode)
                {
                    case 1  : rend = RenderingMode.Cutout; break;
                    case 2  : rend = RenderingMode.Transparent; break;
                    case 3  : rend = RenderingMode.Refraction; break;
                    case 4  : rend = RenderingMode.Fur; break;
                    case 5  : rend = RenderingMode.FurCutout; break;
                    case 6  : rend = RenderingMode.Gem; break;
                    default : rend = RenderingMode.Opaque; break;
                }
            }
            switch(rend)
            {
                case RenderingMode.Opaque:
                    if(islite)
                    {
                        if(isoutl)  material.shader = ltslo;
                        else        material.shader = ltsl;
                    }
                    else if(ismulti)
                    {
                        if(isoutl)  material.shader = ltsmo;
                        else        material.shader = ltsm;
                        material.SetOverrideTag("RenderType", "");
                        material.renderQueue = -1;
                    }
                    else if(istess)
                    {
                        if(isoutl)  material.shader = ltstesso;
                        else        material.shader = ltstess;
                    }
                    else
                    {
                        if(isoutl)  material.shader = ltso;
                        else        material.shader = lts;
                    }
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_AlphaToMask", 0);
                    if(isoutl)
                    {
                        material.SetInt("_OutlineSrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_OutlineDstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.SetInt("_OutlineAlphaToMask", 0);
                    }
                    break;
                case RenderingMode.Cutout:
                    if(islite)
                    {
                        if(isoutl)  material.shader = ltslco;
                        else        material.shader = ltslc;
                    }
                    else if(ismulti)
                    {
                        if(isoutl)  material.shader = ltsmo;
                        else        material.shader = ltsm;
                        material.SetOverrideTag("RenderType", "TransparentCutout");
                        material.renderQueue = 2450;
                    }
                    else if(istess)
                    {
                        if(isoutl)  material.shader = ltstessco;
                        else        material.shader = ltstessc;
                    }
                    else
                    {
                        if(isoutl)  material.shader = ltsco;
                        else        material.shader = ltsc;
                    }
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_AlphaToMask", 1);
                    if(isoutl)
                    {
                        material.SetInt("_OutlineSrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_OutlineDstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.SetInt("_OutlineAlphaToMask", 1);
                    }
                    break;
                case RenderingMode.Transparent:
                    if(ismulti)
                    {
                        if(isoutl)  material.shader = ltsmo;
                        else        material.shader = ltsm;
                        material.SetOverrideTag("RenderType", "TransparentCutout");
                        material.renderQueue = RP == lilRenderPipeline.HDRP ? 3000 : 2460;
                    }
                    else
                    {
                        switch (transparentMode)
                        {
                            case TransparentMode.Normal:
                                if(islite)
                                {
                                    if(isoutl)  material.shader = ltslto;
                                    else        material.shader = ltslt;
                                }
                                else if(istess)
                                {
                                    if(isoutl)  material.shader = ltstessto;
                                    else        material.shader = ltstesst;
                                }
                                else
                                {
                                    if(isoutl)  material.shader = ltsto;
                                    else        material.shader = ltst;
                                }
                                break;
                            case TransparentMode.OnePass:
                                if(islite)
                                {
                                    if(isoutl)  material.shader = ltsloto;
                                    else        material.shader = ltslot;
                                }
                                else if(istess)
                                {
                                    if(isoutl)  material.shader = ltstessoto;
                                    else        material.shader = ltstessot;
                                }
                                else
                                {
                                    if(isoutl)  material.shader = ltsoto;
                                    else        material.shader = ltsot;
                                }
                                break;
                            case TransparentMode.TwoPass:
                                if(islite)
                                {
                                    if(isoutl)  material.shader = ltsltto;
                                    else        material.shader = ltsltt;
                                }
                                else if(istess)
                                {
                                    if(isoutl)  material.shader = ltstesstto;
                                    else        material.shader = ltstesstt;
                                }
                                else
                                {
                                    if(isoutl)  material.shader = ltstto;
                                    else        material.shader = ltstt;
                                }
                                break;
                        }
                    }
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_AlphaToMask", 0);
                    if(isoutl)
                    {
                        material.SetInt("_OutlineSrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt("_OutlineDstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt("_OutlineAlphaToMask", 0);
                    }
                    break;
                case RenderingMode.Refraction:
                    if(ismulti)
                    {
                        material.shader = ltsmref;
                        material.SetOverrideTag("RenderType", "");
                        material.renderQueue = -1;
                    }
                    else
                    {
                        material.shader = ltsref;
                    }
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_AlphaToMask", 0);
                    break;
                case RenderingMode.RefractionBlur:
                    material.shader = ltsrefb;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_AlphaToMask", 0);
                    break;
                case RenderingMode.Fur:
                    if(ismulti)
                    {
                        material.shader = ltsmfur;
                        material.SetOverrideTag("RenderType", "TransparentCutout");
                        material.renderQueue = 3000;
                    }
                    else
                    {
                        material.shader = ltsfur;
                    }
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_AlphaToMask", 0);
                    material.SetInt("_FurSrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_FurDstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_FurZWrite", 0);
                    material.SetInt("_FurAlphaToMask", 0);
                    break;
                case RenderingMode.FurCutout:
                    if(ismulti)
                    {
                        material.shader = ltsmfur;
                        material.SetOverrideTag("RenderType", "TransparentCutout");
                        material.renderQueue = 2450;
                    }
                    else
                    {
                        material.shader = ltsfurc;
                    }
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_AlphaToMask", 1);
                    material.SetInt("_FurSrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_FurDstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_FurZWrite", 1);
                    material.SetInt("_FurAlphaToMask", 1);
                    break;
                case RenderingMode.FurTwoPass:
                    material.shader = ltsfurtwo;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_AlphaToMask", 0);
                    material.SetInt("_FurSrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_FurDstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_FurZWrite", 0);
                    material.SetInt("_FurAlphaToMask", 0);
                    break;
                case RenderingMode.Gem:
                    if(ismulti)
                    {
                        material.shader = ltsmgem;
                        material.SetOverrideTag("RenderType", "");
                        material.renderQueue = -1;
                    }
                    else
                    {
                        material.shader = ltsgem;
                    }
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_AlphaToMask", 0);
                    break;
            }
            if(isstencil) material.renderQueue = material.shader.renderQueue - 1;
            FixTransparentRenderQueue(material, renderingMode);
            material.SetInt("_ZWrite", 1);
            if(rend == RenderingMode.Gem)
            {
                material.SetInt("_Cull", 0);
                material.SetInt("_ZWrite", 0);
            }
            material.SetInt("_ZTest", 4);
            material.SetFloat("_OffsetFactor", 0.0f);
            material.SetFloat("_OffsetUnits", 0.0f);
            material.SetInt("_ColorMask", 15);
            material.SetInt("_SrcBlendAlpha", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlendAlpha", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.Add);
            material.SetInt("_BlendOpAlpha", (int)UnityEngine.Rendering.BlendOp.Add);
            material.SetInt("_SrcBlendFA", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlendFA", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_SrcBlendAlphaFA", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_DstBlendAlphaFA", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_BlendOpFA", (int)UnityEngine.Rendering.BlendOp.Max);
            material.SetInt("_BlendOpAlphaFA", (int)UnityEngine.Rendering.BlendOp.Max);
            if(isoutl)
            {
                material.SetInt("_OutlineCull", 1);
                material.SetInt("_OutlineZWrite", 1);
                material.SetInt("_OutlineZTest", 2);
                material.SetFloat("_OutlineOffsetFactor", 0.0f);
                material.SetFloat("_OutlineOffsetUnits", 0.0f);
                material.SetInt("_OutlineColorMask", 15);
                material.SetInt("_OutlineSrcBlendAlpha", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_OutlineDstBlendAlpha", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_OutlineBlendOp", (int)UnityEngine.Rendering.BlendOp.Add);
                material.SetInt("_OutlineBlendOpAlpha", (int)UnityEngine.Rendering.BlendOp.Add);
                material.SetInt("_OutlineSrcBlendFA", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_OutlineDstBlendFA", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_OutlineSrcBlendAlphaFA", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_OutlineDstBlendAlphaFA", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_OutlineBlendOpFA", (int)UnityEngine.Rendering.BlendOp.Max);
                material.SetInt("_OutlineBlendOpAlphaFA", (int)UnityEngine.Rendering.BlendOp.Max);
            }
            if(renderingMode == RenderingMode.Fur || renderingMode == RenderingMode.FurCutout || renderingMode == RenderingMode.FurTwoPass)
            {
                material.SetInt("_FurZTest", 4);
                material.SetFloat("_FurOffsetFactor", 0.0f);
                material.SetFloat("_FurOffsetUnits", 0.0f);
                material.SetInt("_FurColorMask", 15);
                material.SetInt("_FurSrcBlendAlpha", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_FurDstBlendAlpha", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_FurBlendOp", (int)UnityEngine.Rendering.BlendOp.Add);
                material.SetInt("_FurBlendOpAlpha", (int)UnityEngine.Rendering.BlendOp.Add);
                material.SetInt("_FurSrcBlendFA", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_FurDstBlendFA", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_FurSrcBlendAlphaFA", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_FurDstBlendAlphaFA", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_FurBlendOpFA", (int)UnityEngine.Rendering.BlendOp.Max);
                material.SetInt("_FurBlendOpAlphaFA", (int)UnityEngine.Rendering.BlendOp.Max);
            }
        }

        public static void SetupMaterialWithRenderingMode(Material material, RenderingMode renderingMode, TransparentMode transparentMode, bool isoutl, bool islite, bool isstencil, bool istess)
        {
            SetupMaterialWithRenderingMode(material, renderingMode, transparentMode, isoutl, islite, isstencil, istess, isMulti);
        }

        public static void SetupMaterialWithRenderingMode(Material material, RenderingMode renderingMode, TransparentMode transparentMode)
        {
            SetupMaterialWithRenderingMode(material, renderingMode, transparentMode, isOutl, isLite, isStWr, isTess);
        }

        public static bool CheckMainTextureName(string name)
        {
            return mainTexCheckWords.Any(word => !name.Contains(word));
        }

        public static void RemoveUnusedTexture(Material material)
        {
            if(!material.shader.name.Contains("lilToon")) return;
            RemoveUnusedTexture(material, material.shader.name.Contains("Lite"));
        }

        private static void FixTransparentRenderQueue(Material material, RenderingMode renderingMode)
        {
            #if VRC_SDK_VRCSDK3 && UDON
                if( renderingMode == RenderingMode.Transparent ||
                    renderingMode == RenderingMode.Refraction ||
                    renderingMode == RenderingMode.RefractionBlur ||
                    renderingMode == RenderingMode.Fur ||
                    renderingMode == RenderingMode.FurTwoPass ||
                    renderingMode == RenderingMode.Gem
                )
                {
                    material.renderQueue = 3000;
                }
            #endif
        }

        private static void SetupShaderSettingFromMaterial(Material material, ref lilToonSetting shaderSetting)
        {
            if(material == null) return;
            if(!material.shader.name.Contains("lilToon") || material.shader.name.Contains("Lite") || material.shader.name.Contains("Multi")) return;

            if(!shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV && material.HasProperty("_MainTex_ScrollRotate") && material.GetVector("_MainTex_ScrollRotate") != defaultScrollRotate)
            {
                Debug.Log("[lilToon] LIL_FEATURE_ANIMATE_MAIN_UV : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV = true;
            }
            if(!shaderSetting.LIL_FEATURE_SHADOW && material.HasProperty("_UseShadow") && material.GetFloat("_UseShadow") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_SHADOW : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_SHADOW = true;
            }
            if(!shaderSetting.LIL_FEATURE_RECEIVE_SHADOW && material.HasProperty("_UseShadow") && material.GetFloat("_UseShadow") != 0.0f && (
                (material.HasProperty("_ShadowReceive") && material.GetFloat("_ShadowReceive") > 0.0f) ||
                (material.HasProperty("_Shadow2ndReceive") && material.GetFloat("_Shadow2ndReceive") > 0.0f) ||
                (material.HasProperty("_Shadow3rdReceive") && material.GetFloat("_Shadow3rdReceive") > 0.0f))
            )
            {
                Debug.Log("[lilToon] LIL_FEATURE_RECEIVE_SHADOW : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_RECEIVE_SHADOW = true;
            }
            if(!shaderSetting.LIL_FEATURE_DISTANCE_FADE && material.HasProperty("_DistanceFade") && material.GetVector("_DistanceFade").z != defaultDistanceFadeParams.z)
            {
                Debug.Log("[lilToon] LIL_FEATURE_DISTANCE_FADE : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_DISTANCE_FADE = true;
            }
            if(!shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR && material.HasProperty("_ShadowBlurMask") && material.GetTexture("_ShadowBlurMask") != null)
            {
                Debug.Log("[lilToon] LIL_FEATURE_TEX_SHADOW_BLUR : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR = true;
            }
            if(!shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER && material.HasProperty("_ShadowBorderMask") && material.GetTexture("_ShadowBorderMask") != null)
            {
                Debug.Log("[lilToon] LIL_FEATURE_TEX_SHADOW_BORDER : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER = true;
            }
            if(!shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH && material.HasProperty("_ShadowStrengthMask") && material.GetTexture("_ShadowStrengthMask") != null)
            {
                Debug.Log("[lilToon] LIL_FEATURE_TEX_SHADOW_STRENGTH : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH = true;
            }
            if(!shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST && material.HasProperty("_ShadowColorTex") && material.GetTexture("_ShadowColorTex") != null)
            {
                Debug.Log("[lilToon] LIL_FEATURE_TEX_SHADOW_1ST : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST = true;
            }
            if(!shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND && material.HasProperty("_Shadow2ndColorTex") && material.GetTexture("_Shadow2ndColorTex") != null)
            {
                Debug.Log("[lilToon] LIL_FEATURE_TEX_SHADOW_2ND : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND = true;
            }
            if(!shaderSetting.LIL_FEATURE_SHADOW_3RD && material.HasProperty("_Shadow3rdColor") && material.GetColor("_Shadow3rdColor").a != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_SHADOW_3RD : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_SHADOW_3RD = true;
            }
            if(!shaderSetting.LIL_FEATURE_TEX_SHADOW_3RD && material.HasProperty("_Shadow3rdColorTex") && material.GetTexture("_Shadow3rdColorTex") != null)
            {
                Debug.Log("[lilToon] LIL_FEATURE_SHADOW_3RD : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_SHADOW_3RD = true;
            }

            if(material.shader.name.Contains("Fur"))
            {
                if(!shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL && material.HasProperty("_FurVectorTex") && material.GetTexture("_FurVectorTex") != null)
                {
                    Debug.Log("[lilToon] LIL_FEATURE_TEX_FUR_NORMAL : " + AssetDatabase.GetAssetPath(material));
                    shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL = true;
                }
                if(!shaderSetting.LIL_FEATURE_TEX_FUR_MASK && material.HasProperty("_FurMask") && material.GetTexture("_FurMask") != null)
                {
                    Debug.Log("[lilToon] LIL_FEATURE_TEX_FUR_MASK : " + AssetDatabase.GetAssetPath(material));
                    shaderSetting.LIL_FEATURE_TEX_FUR_MASK = true;
                }
                if(!shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH && material.HasProperty("_FurLengthMask") && material.GetTexture("_FurLengthMask") != null)
                {
                    Debug.Log("[lilToon] LIL_FEATURE_TEX_FUR_LENGTH : " + AssetDatabase.GetAssetPath(material));
                    shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH = true;
                }
                if(!shaderSetting.LIL_FEATURE_FUR_COLLISION && material.HasProperty("_FurTouchStrength") && material.GetFloat("_FurTouchStrength") != 0.0f)
                {
                    Debug.Log("[lilToon] LIL_FEATURE_FUR_COLLISION : " + AssetDatabase.GetAssetPath(material));
                    shaderSetting.LIL_FEATURE_FUR_COLLISION = true;
                }
            }

            if(!shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION && material.HasProperty("_MainTexHSVG") && material.GetVector("_MainTexHSVG") != defaultHSVG)
            {
                Debug.Log("[lilToon] LIL_FEATURE_MAIN_TONE_CORRECTION : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION = true;
            }
            if(!shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP && material.HasProperty("_MainGradationStrength") && material.GetFloat("_MainGradationStrength") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_MAIN_GRADATION_MAP : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP = true;
            }
            if(!shaderSetting.LIL_FEATURE_MAIN2ND && material.HasProperty("_UseMain2ndTex") && material.GetFloat("_UseMain2ndTex") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_MAIN2ND : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_MAIN2ND = true;
            }
            if(!shaderSetting.LIL_FEATURE_MAIN3RD && material.HasProperty("_UseMain3rdTex") && material.GetFloat("_UseMain3rdTex") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_MAIN3RD : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_MAIN3RD = true;
            }
            if(!shaderSetting.LIL_FEATURE_DECAL && (
                (material.HasProperty("_Main2ndTexIsDecal") && material.GetFloat("_Main2ndTexIsDecal") != 0.0f) ||
                (material.HasProperty("_Main3rdTexIsDecal") && material.GetFloat("_Main3rdTexIsDecal") != 0.0f))
            )
            {
                Debug.Log("[lilToon] LIL_FEATURE_DECAL : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_DECAL = true;
            }
            if(!shaderSetting.LIL_FEATURE_ANIMATE_DECAL && (
                (material.HasProperty("_Main2ndTexDecalAnimation") && material.GetVector("_Main2ndTexDecalAnimation") != defaultDecalAnim) ||
                (material.HasProperty("_Main3rdTexDecalAnimation") && material.GetVector("_Main3rdTexDecalAnimation") != defaultDecalAnim))
            )
            {
                Debug.Log("[lilToon] LIL_FEATURE_ANIMATE_DECAL : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_ANIMATE_DECAL = true;
            }
            if(!shaderSetting.LIL_FEATURE_LAYER_DISSOLVE && (
                (material.HasProperty("_Main2ndDissolveParams") && material.GetVector("_Main2ndDissolveParams").x != defaultDissolveParams.x) ||
                (material.HasProperty("_Main3rdDissolveParams") && material.GetVector("_Main3rdDissolveParams").x != defaultDissolveParams.x))
            )
            {
                Debug.Log("[lilToon] LIL_FEATURE_LAYER_DISSOLVE : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_LAYER_DISSOLVE = true;
            }
            if(!shaderSetting.LIL_FEATURE_ALPHAMASK && material.HasProperty("_AlphaMaskMode") && material.GetFloat("_AlphaMaskMode") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_ALPHAMASK : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_ALPHAMASK = true;
            }
            if(!shaderSetting.LIL_FEATURE_EMISSION_1ST && material.HasProperty("_UseEmission") && material.GetFloat("_UseEmission") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_EMISSION_1ST : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_EMISSION_1ST = true;
            }
            if(!shaderSetting.LIL_FEATURE_EMISSION_2ND && material.HasProperty("_UseEmission2nd") && material.GetFloat("_UseEmission2nd") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_EMISSION_2ND : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_EMISSION_2ND = true;
            }
            if(!shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV && (
                (material.HasProperty("_EmissionMap_ScrollRotate") && material.GetVector("_EmissionMap_ScrollRotate") != defaultScrollRotate) ||
                (material.HasProperty("_Emission2ndMap_ScrollRotate") && material.GetVector("_Emission2ndMap_ScrollRotate") != defaultScrollRotate))
            )
            {
                Debug.Log("[lilToon] LIL_FEATURE_ANIMATE_EMISSION_UV : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV = true;
            }
            if(!shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV && (
                (material.HasProperty("_EmissionBlendMask_ScrollRotate") && material.GetVector("_EmissionBlendMask_ScrollRotate") != defaultScrollRotate) ||
                (material.HasProperty("_Emission2ndBlendMask_ScrollRotate") && material.GetVector("_Emission2ndBlendMask_ScrollRotate") != defaultScrollRotate))
            )
            {
                Debug.Log("[lilToon] LIL_FEATURE_ANIMATE_EMISSION_MASK_UV : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV = true;
            }
            if(!shaderSetting.LIL_FEATURE_EMISSION_GRADATION && (
                (material.HasProperty("_EmissionUseGrad") && material.GetFloat("_EmissionUseGrad") != 0.0f) ||
                (material.HasProperty("_Emission2ndUseGrad") && material.GetFloat("_Emission2ndUseGrad") != 0.0f))
            )
            {
                Debug.Log("[lilToon] LIL_FEATURE_EMISSION_GRADATION : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_EMISSION_GRADATION = true;
            }
            if(!shaderSetting.LIL_FEATURE_NORMAL_1ST && material.HasProperty("_UseBumpMap") && material.GetFloat("_UseBumpMap") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_NORMAL_1ST : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_NORMAL_1ST = true;
            }
            if(!shaderSetting.LIL_FEATURE_NORMAL_2ND && material.HasProperty("_UseBump2ndMap") && material.GetFloat("_UseBump2ndMap") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_NORMAL_2ND : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_NORMAL_2ND = true;
            }
            if(!shaderSetting.LIL_FEATURE_ANISOTROPY && material.HasProperty("_UseAnisotropy") && material.GetFloat("_UseAnisotropy") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_ANISOTROPY : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_ANISOTROPY = true;
            }
            if(!shaderSetting.LIL_FEATURE_REFLECTION && material.HasProperty("_UseReflection") && material.GetFloat("_UseReflection") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_REFLECTION : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_REFLECTION = true;
            }
            if(!shaderSetting.LIL_FEATURE_MATCAP && material.HasProperty("_UseMatCap") && material.GetFloat("_UseMatCap") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_MATCAP : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_MATCAP = true;
            }
            if(!shaderSetting.LIL_FEATURE_MATCAP_2ND && material.HasProperty("_UseMatCap2nd") && material.GetFloat("_UseMatCap2nd") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_MATCAP_2ND : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_MATCAP_2ND = true;
            }
            if(!shaderSetting.LIL_FEATURE_RIMLIGHT && material.HasProperty("_UseRim") && material.GetFloat("_UseRim") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_RIMLIGHT : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_RIMLIGHT = true;
            }
            if(!shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION && material.HasProperty("_RimDirStrength") && material.GetFloat("_RimDirStrength") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_RIMLIGHT_DIRECTION : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION = true;
            }
            if(!shaderSetting.LIL_FEATURE_GLITTER && material.HasProperty("_UseGlitter") && material.GetFloat("_UseGlitter") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_GLITTER : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_GLITTER = true;
            }
            if(!shaderSetting.LIL_FEATURE_BACKLIGHT && material.HasProperty("_UseBacklight") && material.GetFloat("_UseBacklight") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_BACKLIGHT : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_BACKLIGHT = true;
            }
            if(!shaderSetting.LIL_FEATURE_PARALLAX && material.HasProperty("_UseParallax") && material.GetFloat("_UseParallax") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_PARALLAX : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_PARALLAX = true;
                if(!shaderSetting.LIL_FEATURE_POM && material.HasProperty("_UsePOM") && material.GetFloat("_UsePOM") != 0.0f)
                {
                    Debug.Log("[lilToon] LIL_FEATURE_POM : " + AssetDatabase.GetAssetPath(material));
                    shaderSetting.LIL_FEATURE_POM = true;
                }
            }
            if(!shaderSetting.LIL_FEATURE_AUDIOLINK && material.HasProperty("_UseAudioLink") && material.GetFloat("_UseAudioLink") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_AUDIOLINK : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_AUDIOLINK = true;
            }
            if(!shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX && material.HasProperty("_AudioLink2Vertex") && material.GetFloat("_AudioLink2Vertex") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_AUDIOLINK_VERTEX : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX = true;
            }
            if(!shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL && material.HasProperty("_AudioLinkAsLocal") && material.GetFloat("_AudioLinkAsLocal") != 0.0f)
            {
                Debug.Log("[lilToon] LIL_FEATURE_AUDIOLINK_LOCAL : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL = true;
            }
            if(!shaderSetting.LIL_FEATURE_DISSOLVE && material.HasProperty("_DissolveParams") && material.GetVector("_DissolveParams").x != defaultDissolveParams.x)
            {
                Debug.Log("[lilToon] LIL_FEATURE_DISSOLVE : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_DISSOLVE = true;
            }
            if(!shaderSetting.LIL_FEATURE_TEX_LAYER_MASK && (
                (material.HasProperty("_Main2ndBlendMask") && material.GetTexture("_Main2ndBlendMask") != null) ||
                (material.HasProperty("_Main3rdBlendMask") && material.GetTexture("_Main3rdBlendMask") != null))
            )
            {
                Debug.Log("[lilToon] LIL_FEATURE_TEX_LAYER_MASK : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_LAYER_MASK = true;
            }
            if(!shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE && (
                (material.HasProperty("_Main2ndDissolveNoiseMask") && material.GetTexture("_Main2ndDissolveNoiseMask") != null) ||
                (material.HasProperty("_Main3rdDissolveNoiseMask") && material.GetTexture("_Main3rdDissolveNoiseMask") != null))
            )
            {
                Debug.Log("[lilToon] LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE = true;
            }
            if(!shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK && (
                (material.HasProperty("_EmissionBlendMask") && material.GetTexture("_EmissionBlendMask") != null) ||
                (material.HasProperty("_Emission2ndBlendMask") && material.GetTexture("_Emission2ndBlendMask") != null))
            )
            {
                Debug.Log("[lilToon] LIL_FEATURE_TEX_EMISSION_MASK : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK = true;
            }
            if(!shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK && material.HasProperty("_Bump2ndScaleMask") && material.GetTexture("_Bump2ndScaleMask") != null)
            {
                Debug.Log("[lilToon] LIL_FEATURE_TEX_NORMAL_MASK : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK = true;
            }
            if(!shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS && material.HasProperty("_SmoothnessTex") && material.GetTexture("_SmoothnessTex") != null)
            {
                Debug.Log("[lilToon] LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS = true;
            }
            if(!shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC && material.HasProperty("_MetallicGlossMap") && material.GetTexture("_MetallicGlossMap") != null)
            {
                Debug.Log("[lilToon] LIL_FEATURE_TEX_REFLECTION_METALLIC : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC = true;
            }
            if(!shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR && material.HasProperty("_ReflectionColorTex") && material.GetTexture("_ReflectionColorTex") != null)
            {
                Debug.Log("[lilToon] LIL_FEATURE_TEX_REFLECTION_COLOR : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR = true;
            }
            if(!shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK && (
                (material.HasProperty("_MatCapBlendMask") && material.GetTexture("_MatCapBlendMask") != null) ||
                (material.HasProperty("_MatCap2ndBlendMask") && material.GetTexture("_MatCap2ndBlendMask") != null))
            )
            {
                Debug.Log("[lilToon] LIL_FEATURE_TEX_MATCAP_MASK : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK = true;
            }
            if(!shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP && (
                (material.HasProperty("_MatCapBumpMap") && material.GetTexture("_MatCapBumpMap") != null) ||
                (material.HasProperty("_MatCap2ndBumpMap") && material.GetTexture("_MatCap2ndBumpMap") != null))
            )
            {
                Debug.Log("[lilToon] LIL_FEATURE_TEX_MATCAP_NORMALMAP : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP = true;
            }
            if(!shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR && material.HasProperty("_RimColorTex") && material.GetTexture("_RimColorTex") != null)
            {
                Debug.Log("[lilToon] LIL_FEATURE_TEX_RIMLIGHT_COLOR : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR = true;
            }
            if(!shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE && material.HasProperty("_DissolveNoiseMask") && material.GetTexture("_DissolveNoiseMask") != null)
            {
                Debug.Log("[lilToon] LIL_FEATURE_TEX_DISSOLVE_NOISE : " + AssetDatabase.GetAssetPath(material));
                shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE = true;
            }

            // Outline
            if(material.shader.name.Contains("Outline"))
            {
                if(!shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV && material.HasProperty("_OutlineTex_ScrollRotate") && material.GetVector("_OutlineTex_ScrollRotate") != defaultScrollRotate)
                {
                    Debug.Log("[lilToon] LIL_FEATURE_ANIMATE_OUTLINE_UV : " + AssetDatabase.GetAssetPath(material));
                    shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV = true;
                }
                if(!shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION && material.HasProperty("_OutlineTexHSVG") && material.GetVector("_OutlineTexHSVG") != defaultHSVG)
                {
                    Debug.Log("[lilToon] LIL_FEATURE_OUTLINE_TONE_CORRECTION : " + AssetDatabase.GetAssetPath(material));
                    shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION = true;
                }
                if(!shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR && material.HasProperty("_OutlineTex") && material.GetTexture("_OutlineTex") != null)
                {
                    Debug.Log("[lilToon] LIL_FEATURE_TEX_OUTLINE_COLOR : " + AssetDatabase.GetAssetPath(material));
                    shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR = true;
                }
                if(!shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH && material.HasProperty("_OutlineWidthMask") && material.GetTexture("_OutlineWidthMask") != null)
                {
                    Debug.Log("[lilToon] LIL_FEATURE_TEX_OUTLINE_WIDTH : " + AssetDatabase.GetAssetPath(material));
                    shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH = true;
                }
                if(!shaderSetting.LIL_FEATURE_TEX_OUTLINE_NORMAL && material.HasProperty("_OutlineVectorTex") && material.GetTexture("_OutlineVectorTex") != null)
                {
                    Debug.Log("[lilToon] LIL_FEATURE_TEX_OUTLINE_NORMAL : " + AssetDatabase.GetAssetPath(material));
                    shaderSetting.LIL_FEATURE_TEX_OUTLINE_NORMAL = true;
                }
            }
        }

        private static void SetupShaderSettingFromAnimationClip(AnimationClip clip, ref lilToonSetting shaderSetting, bool shouldCheckMaterial = false)
        {
            if(clip == null) return;

            if(shouldCheckMaterial)
            {
                foreach(EditorCurveBinding binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
                {
                    foreach(ObjectReferenceKeyframe frame in AnimationUtility.GetObjectReferenceCurve(clip, binding))
                    {
                        if(frame.value is Material)
                        {
                            SetupShaderSettingFromMaterial((Material)frame.value, ref shaderSetting);
                        }
                    }
                }
            }

            foreach(EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
            {
                string propname = binding.propertyName;
                if(string.IsNullOrEmpty(propname) || !propname.Contains("material.")) continue;

                shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV = shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV || propname.Contains("_MainTex_ScrollRotate");
                shaderSetting.LIL_FEATURE_SHADOW = shaderSetting.LIL_FEATURE_SHADOW || propname.Contains("_UseShadow");
                shaderSetting.LIL_FEATURE_RECEIVE_SHADOW = shaderSetting.LIL_FEATURE_RECEIVE_SHADOW || propname.Contains("_ShadowReceive") || propname.Contains("_Shadow2ndReceive") || propname.Contains("_Shadow3rdReceive");
                shaderSetting.LIL_FEATURE_DISTANCE_FADE = shaderSetting.LIL_FEATURE_DISTANCE_FADE || propname.Contains("_DistanceFade");
                shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR = shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR || propname.Contains("_ShadowBlurMask");
                shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER = shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER || propname.Contains("_ShadowBorderMask");
                shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH = shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH || propname.Contains("_ShadowStrengthMask");
                shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST = shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST || propname.Contains("_ShadowColorTex");
                shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND = shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND || propname.Contains("_Shadow2ndColorTex");
                shaderSetting.LIL_FEATURE_SHADOW_3RD = shaderSetting.LIL_FEATURE_SHADOW_3RD || propname.Contains("_Shadow3rdColor");
                shaderSetting.LIL_FEATURE_TEX_SHADOW_3RD = shaderSetting.LIL_FEATURE_TEX_SHADOW_3RD || propname.Contains("_Shadow3rdColorTex");

                shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL = shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL || propname.Contains("_FurVectorTex");
                shaderSetting.LIL_FEATURE_TEX_FUR_MASK = shaderSetting.LIL_FEATURE_TEX_FUR_MASK || propname.Contains("_FurMask");
                shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH = shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH || propname.Contains("_FurLengthMask");
                shaderSetting.LIL_FEATURE_FUR_COLLISION = shaderSetting.LIL_FEATURE_FUR_COLLISION || propname.Contains("_FurTouchStrength");

                shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION = shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION || propname.Contains("_MainTexHSVG");
                shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP = shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP || propname.Contains("_MainGradationStrength");
                shaderSetting.LIL_FEATURE_MAIN2ND = shaderSetting.LIL_FEATURE_MAIN2ND || propname.Contains("_UseMain2ndTex");
                shaderSetting.LIL_FEATURE_MAIN3RD = shaderSetting.LIL_FEATURE_MAIN3RD || propname.Contains("_UseMain3rdTex");
                shaderSetting.LIL_FEATURE_DECAL = shaderSetting.LIL_FEATURE_DECAL || propname.Contains("_Main2ndTexIsDecal") || propname.Contains("_Main3rdTexIsDecal");
                shaderSetting.LIL_FEATURE_ANIMATE_DECAL = shaderSetting.LIL_FEATURE_ANIMATE_DECAL || propname.Contains("_Main2ndTexDecalAnimation") || propname.Contains("_Main3rdTexDecalAnimation");
                shaderSetting.LIL_FEATURE_LAYER_DISSOLVE = shaderSetting.LIL_FEATURE_LAYER_DISSOLVE || propname.Contains("_Main2ndDissolveParams") || propname.Contains("_Main3rdDissolveParams");
                shaderSetting.LIL_FEATURE_ALPHAMASK = shaderSetting.LIL_FEATURE_ALPHAMASK || propname.Contains("_AlphaMaskMode");
                shaderSetting.LIL_FEATURE_EMISSION_1ST = shaderSetting.LIL_FEATURE_EMISSION_1ST || propname.Contains("_UseEmission");
                shaderSetting.LIL_FEATURE_EMISSION_2ND = shaderSetting.LIL_FEATURE_EMISSION_2ND || propname.Contains("_UseEmission2nd");
                shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV = shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV || propname.Contains("_EmissionMap_ScrollRotate") || propname.Contains("_Emission2ndMap_ScrollRotate");
                shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV = shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV || propname.Contains("_EmissionBlendMask_ScrollRotate") || propname.Contains("_Emission2ndBlendMask_ScrollRotate");
                shaderSetting.LIL_FEATURE_EMISSION_GRADATION = shaderSetting.LIL_FEATURE_EMISSION_GRADATION || propname.Contains("_EmissionUseGrad");
                shaderSetting.LIL_FEATURE_NORMAL_1ST = shaderSetting.LIL_FEATURE_NORMAL_1ST || propname.Contains("_UseBumpMap");
                shaderSetting.LIL_FEATURE_NORMAL_2ND = shaderSetting.LIL_FEATURE_NORMAL_2ND || propname.Contains("_UseBump2ndMap");
                shaderSetting.LIL_FEATURE_ANISOTROPY = shaderSetting.LIL_FEATURE_ANISOTROPY || propname.Contains("_UseAnisotropy");
                shaderSetting.LIL_FEATURE_REFLECTION = shaderSetting.LIL_FEATURE_REFLECTION || propname.Contains("_UseReflection");
                shaderSetting.LIL_FEATURE_MATCAP = shaderSetting.LIL_FEATURE_MATCAP || propname.Contains("_UseMatCap");
                shaderSetting.LIL_FEATURE_MATCAP_2ND = shaderSetting.LIL_FEATURE_MATCAP_2ND || propname.Contains("_UseMatCap2nd");
                shaderSetting.LIL_FEATURE_RIMLIGHT = shaderSetting.LIL_FEATURE_RIMLIGHT || propname.Contains("_UseRim");
                shaderSetting.LIL_FEATURE_GLITTER = shaderSetting.LIL_FEATURE_GLITTER || propname.Contains("_UseGlitter");
                shaderSetting.LIL_FEATURE_BACKLIGHT = shaderSetting.LIL_FEATURE_BACKLIGHT || propname.Contains("_UseBacklight");
                shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION = shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION || propname.Contains("_RimDirStrength");
                shaderSetting.LIL_FEATURE_PARALLAX = shaderSetting.LIL_FEATURE_PARALLAX || propname.Contains("_UseParallax");
                shaderSetting.LIL_FEATURE_POM = shaderSetting.LIL_FEATURE_POM || propname.Contains("_UsePOM");
                shaderSetting.LIL_FEATURE_AUDIOLINK = shaderSetting.LIL_FEATURE_AUDIOLINK || propname.Contains("_UseAudioLink");
                shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX = shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX || propname.Contains("_AudioLink2Vertex");
                shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL = shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL || propname.Contains("_AudioLinkAsLocal");
                shaderSetting.LIL_FEATURE_DISSOLVE = shaderSetting.LIL_FEATURE_DISSOLVE || propname.Contains("_DissolveParams");
                shaderSetting.LIL_FEATURE_TEX_LAYER_MASK = shaderSetting.LIL_FEATURE_TEX_LAYER_MASK || propname.Contains("_Main2ndBlendMask") || propname.Contains("_Main3rdBlendMask");
                shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE = shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE || propname.Contains("_Main2ndDissolveNoiseMask") || propname.Contains("_Main3rdDissolveNoiseMask");
                shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK = shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK || propname.Contains("_EmissionBlendMask") || propname.Contains("_Emission2ndBlendMask");
                shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK = shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK || propname.Contains("_Bump2ndScaleMask");
                shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS = shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS || propname.Contains("_SmoothnessTex");
                shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC = shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC || propname.Contains("_MetallicGlossMap");
                shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR = shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR || propname.Contains("_ReflectionColorTex");
                shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK = shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK || propname.Contains("_MatCapBlendMask") || propname.Contains("_MatCap2ndBlendMask");
                shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP = shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP || propname.Contains("_MatCapBumpMap") || propname.Contains("_MatCap2ndBumpMap");
                shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR = shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR || propname.Contains("_RimColorTex");
                shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE = shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE || propname.Contains("_DissolveNoiseMask");

                shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV = shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV || propname.Contains("_OutlineTex_ScrollRotate");
                shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION = shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION || propname.Contains("_OutlineTexHSVG");
                shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR = shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR || propname.Contains("_OutlineTex");
                shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH = shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH || propname.Contains("_OutlineWidthMask");
                shaderSetting.LIL_FEATURE_TEX_OUTLINE_NORMAL = shaderSetting.LIL_FEATURE_TEX_OUTLINE_NORMAL || propname.Contains("_OutlineVectorTex");
            }
        }

        private static void SetShaderKeywords(Material material, string keyword, bool enable)
        {
            if(enable)  material.EnableKeyword(keyword);
            else        material.DisableKeyword(keyword);
        }

        private void SetupMultiMaterial(Material material)
        {
            SetShaderKeywords(material, "UNITY_UI_ALPHACLIP",                   material.GetFloat("_TransparentMode") == 1.0f);
            SetShaderKeywords(material, "UNITY_UI_CLIP_RECT",                   material.GetFloat("_TransparentMode") == 2.0f || material.GetFloat("_TransparentMode") == 4.0f);
            material.SetShaderPassEnabled("ShadowCaster",                       material.GetFloat("_AsOverlay") == 0.0f);
            material.SetShaderPassEnabled("DepthOnly",                          material.GetFloat("_AsOverlay") == 0.0f);
            material.SetShaderPassEnabled("DepthNormals",                       material.GetFloat("_AsOverlay") == 0.0f);
            material.SetShaderPassEnabled("DepthForwardOnly",                   material.GetFloat("_AsOverlay") == 0.0f);
            material.SetShaderPassEnabled("MotionVectors",                      material.GetFloat("_AsOverlay") == 0.0f);

            if(isGem)
            {
                SetShaderKeywords(material, "_REQUIRE_UV2",                         false);
                SetShaderKeywords(material, "_FADING_ON",                           false);
            }
            else
            {
                SetShaderKeywords(material, "_REQUIRE_UV2",                         useShadow.floatValue != 0.0f);
                SetShaderKeywords(material, "_FADING_ON",                           distanceFade.vectorValue.z != 0.0f);
            }

            SetShaderKeywords(material, "_EMISSION",                            useEmission.floatValue != 0.0f);
            SetShaderKeywords(material, "GEOM_TYPE_BRANCH",                     useEmission2nd.floatValue != 0.0f);
            SetShaderKeywords(material, "_SUNDISK_SIMPLE",                      (useEmission.floatValue != 0.0f && emissionBlendMask.textureValue != null) || (useEmission2nd.floatValue != 0.0f && emission2ndBlendMask.textureValue != null));
            SetShaderKeywords(material, "_NORMALMAP",                           useBumpMap.floatValue != 0.0f);
            SetShaderKeywords(material, "EFFECT_BUMP",                          useBump2ndMap.floatValue != 0.0f);
            SetShaderKeywords(material, "SOURCE_GBUFFER",                       useAnisotropy.floatValue != 0.0f);
            SetShaderKeywords(material, "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A", useMatCap.floatValue != 0.0f);
            SetShaderKeywords(material, "_SPECULARHIGHLIGHTS_OFF",              useMatCap2nd.floatValue != 0.0f);
            SetShaderKeywords(material, "GEOM_TYPE_MESH",                       (useMatCap.floatValue != 0.0f && matcapCustomNormal.floatValue != 0.0f) || (useMatCap2nd.floatValue != 0.0f && matcap2ndCustomNormal.floatValue != 0.0f));
            SetShaderKeywords(material, "_METALLICGLOSSMAP",                    useRim.floatValue != 0.0f);
            SetShaderKeywords(material, "GEOM_TYPE_LEAF",                       useRim.floatValue != 0.0f && rimDirStrength.floatValue != 0.0f);
            SetShaderKeywords(material, "_SPECGLOSSMAP",                        useGlitter.floatValue != 0.0f);
            SetShaderKeywords(material, "_MAPPING_6_FRAMES_LAYOUT",             useAudioLink.floatValue != 0.0f);
            SetShaderKeywords(material, "_SUNDISK_HIGH_QUALITY",                useAudioLink.floatValue != 0.0f && audioLinkAsLocal.floatValue != 0.0f);
            SetShaderKeywords(material, "GEOM_TYPE_BRANCH_DETAIL",              dissolveParams.vectorValue.x != 0.0f);

            if(isGem)
            {
                SetShaderKeywords(material, "EFFECT_HUE_VARIATION",                 false);
                SetShaderKeywords(material, "_COLORADDSUBDIFF_ON",                  false);
                SetShaderKeywords(material, "_COLORCOLOR_ON",                       false);
                SetShaderKeywords(material, "_SUNDISK_NONE",                        false);
                SetShaderKeywords(material, "GEOM_TYPE_FROND",                      false);
                SetShaderKeywords(material, "_COLOROVERLAY_ON",                     false);
                SetShaderKeywords(material, "ANTI_FLICKER",                         false);
                SetShaderKeywords(material, "_PARALLAXMAP",                         false);
                SetShaderKeywords(material, "PIXELSNAP_ON",                         false);
                SetShaderKeywords(material, "_GLOSSYREFLECTIONS_OFF",               false);
            }
            else
            {
                SetShaderKeywords(material, "EFFECT_HUE_VARIATION",                 mainTexHSVG.vectorValue != defaultHSVG || mainGradationStrength.floatValue != 0.0f);
                SetShaderKeywords(material, "_COLORADDSUBDIFF_ON",                  useMain2ndTex.floatValue != 0.0f);
                SetShaderKeywords(material, "_COLORCOLOR_ON",                       useMain3rdTex.floatValue != 0.0f);
                SetShaderKeywords(material, "_SUNDISK_NONE",                        (useMain2ndTex.floatValue != 0.0f && main2ndTexDecalAnimation.vectorValue != defaultDecalAnim) || (useMain3rdTex.floatValue != 0.0f && main3rdTexDecalAnimation.vectorValue != defaultDecalAnim));
                SetShaderKeywords(material, "GEOM_TYPE_FROND",                      (useMain2ndTex.floatValue != 0.0f && main2ndDissolveParams.vectorValue.x != 0.0f) || (useMain3rdTex.floatValue != 0.0f && main3rdDissolveParams.vectorValue.x != 0.0f));
                SetShaderKeywords(material, "_COLOROVERLAY_ON",                     material.GetFloat("_TransparentMode") != 0.0f && alphaMaskMode.floatValue != 0.0f);
                SetShaderKeywords(material, "ANTI_FLICKER",                         useBacklight.floatValue != 0.0f);
                SetShaderKeywords(material, "_PARALLAXMAP",                         useParallax.floatValue != 0.0f);
                SetShaderKeywords(material, "PIXELSNAP_ON",                         useParallax.floatValue != 0.0f && usePOM.floatValue != 0.0f);
                SetShaderKeywords(material, "_GLOSSYREFLECTIONS_OFF",               useReflection.floatValue != 0.0f);
            }

            if(isRefr || isFur || isGem)
            {
                SetShaderKeywords(material, "ETC1_EXTERNAL_ALPHA",                  false);
                SetShaderKeywords(material, "_DETAIL_MULX2",                        false);
            }
            else
            {
                SetShaderKeywords(material, "ETC1_EXTERNAL_ALPHA",                  false);
                SetShaderKeywords(material, "_DETAIL_MULX2",                        isOutl && material.GetVector("_OutlineTexHSVG") != defaultHSVG);
            }

            // Remove old keywords
            material.SetShaderPassEnabled("SRPDEFAULTUNLIT",                    true);
            SetShaderKeywords(material, "BILLBOARD_FACE_CAMERA_POS",            false);
        }

        private static void RemoveUnusedTexture(Material material, bool islite)
        {
            RemoveUnusedProperties(material);
            if(islite)
            {
                if(material.GetFloat("_UseShadow") == 0.0f)
                {
                    material.SetTexture("_ShadowColorTex", null);
                    material.SetTexture("_Shadow2ndColorTex", null);
                }
                if(material.GetFloat("_UseEmission") == 0.0f)
                {
                    material.SetTexture("_EmissionMap", null);
                }
                if(material.GetFloat("_UseMatCap") == 0.0f)
                {
                    material.SetTexture("_MatCapTex", null);
                }
            }
            else
            {
                if(material.GetFloat("_MainGradationStrength") == 0.0f) material.SetTexture("_MainGradationTex", null);
                if(material.GetFloat("_UseMain2ndTex") == 0.0f)
                {
                    material.SetTexture("_Main2ndTex", null);
                    material.SetTexture("_Main2ndBlendMask", null);
                    material.SetTexture("_Main2ndDissolveMask", null);
                    material.SetTexture("_Main2ndDissolveNoiseMask", null);
                }
                if(material.GetFloat("_UseMain3rdTex") == 0.0f)
                {
                    material.SetTexture("_Main3rdTex", null);
                    material.SetTexture("_Main3rdBlendMask", null);
                    material.SetTexture("_Main3rdDissolveMask", null);
                    material.SetTexture("_Main3rdDissolveNoiseMask", null);
                }
                if(material.GetFloat("_UseShadow") == 0.0f)
                {
                    material.SetTexture("_ShadowBlurMask", null);
                    material.SetTexture("_ShadowBorderMask", null);
                    material.SetTexture("_ShadowStrengthMask", null);
                    material.SetTexture("_ShadowColorTex", null);
                    material.SetTexture("_Shadow2ndColorTex", null);
                    material.SetTexture("_Shadow3rdColorTex", null);
                }
                if(material.GetFloat("_UseEmission") == 0.0f)
                {
                    material.SetTexture("_EmissionMap", null);
                    material.SetTexture("_EmissionBlendMask", null);
                    material.SetTexture("_EmissionGradTex", null);
                }
                if(material.GetFloat("_UseEmission2nd") == 0.0f)
                {
                    material.SetTexture("_Emission2ndMap", null);
                    material.SetTexture("_Emission2ndBlendMask", null);
                    material.SetTexture("_Emission2ndGradTex", null);
                }
                if(material.GetFloat("_UseBumpMap") == 0.0f) material.SetTexture("_BumpMap", null);
                if(material.GetFloat("_UseBump2ndMap") == 0.0f)
                {
                    material.SetTexture("_Bump2ndMap", null);
                    material.SetTexture("_Bump2ndScaleMask", null);
                }
                if(material.GetFloat("_UseAnisotropy") == 0.0f)
                {
                    material.SetTexture("_AnisotropyTangentMap", null);
                    material.SetTexture("_AnisotropyScaleMask", null);
                    material.SetTexture("_AnisotropyShiftNoiseMask", null);
                }
                if(material.GetFloat("_UseReflection") == 0.0f)
                {
                    material.SetTexture("_SmoothnessTex", null);
                    material.SetTexture("_MetallicGlossMap", null);
                    material.SetTexture("_ReflectionColorTex", null);
                }
                if(material.GetFloat("_UseMatCap") == 0.0f)
                {
                    material.SetTexture("_MatCapTex", null);
                    material.SetTexture("_MatCapBlendMask", null);
                }
                if(material.GetFloat("_UseMatCap2nd") == 0.0f)
                {
                    material.SetTexture("_MatCap2ndTex", null);
                    material.SetTexture("_MatCap2ndBlendMask", null);
                }
                if(material.GetFloat("_UseRim") == 0.0f) material.SetTexture("_RimColorTex", null);
                if(material.GetFloat("_UseGlitter") == 0.0f) material.SetTexture("_GlitterColorTex", null);
                if(material.GetFloat("_UseParallax") == 0.0f) material.SetTexture("_ParallaxMap", null);
                if(material.GetFloat("_UseAudioLink") == 0.0f || material.GetFloat("_AudioLinkUVMode") != 3.0f) material.SetTexture("_AudioLinkMask", null);
                if(material.GetFloat("_UseAudioLink") == 0.0f || material.GetFloat("_AudioLinkAsLocal") == 0.0f) material.SetTexture("_AudioLinkLocalMap", null);
            }
        }

        private static void RemoveShaderKeywords(Material material)
        {
            foreach(string keyword in material.shaderKeywords)
            {
                material.DisableKeyword(keyword);
            }
        }

        private static void RemoveUnusedProperties(Material material)
        {
            // https://light11.hatenadiary.com/entry/2018/12/04/224253
            var so = new SerializedObject(material);
            so.Update();
            var savedProps = so.FindProperty("m_SavedProperties");

            var texs = savedProps.FindPropertyRelative("m_TexEnvs");
            DeleteUnused(ref texs, material);

            var floats = savedProps.FindPropertyRelative("m_Floats");
            DeleteUnused(ref floats, material);

            var colors = savedProps.FindPropertyRelative("m_Colors");
            DeleteUnused(ref colors, material);

            so.ApplyModifiedProperties();
        }

        private static void DeleteUnused(ref SerializedProperty props, Material material)
        {
            for(int i = props.arraySize - 1; i >= 0; i--)
            {
                if(!material.HasProperty(props.GetArrayElementAtIndex(i).FindPropertyRelative("first").stringValue))
                {
                    props.DeleteArrayElementAtIndex(i);
                }
            }
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Presets
        #region
        public static void ApplyPreset(Material material, lilToonPreset preset, bool ismulti)
        {
            if(material == null || preset == null) return;
            Undo.RecordObject(material, "Apply Preset");
            for(int i = 0; i < preset.floats.Length; i++)
            {
                if(preset.floats[i].name == "_StencilPass") material.SetFloat(preset.floats[i].name, preset.floats[i].value);
            }
            if(preset.shader != null) material.shader = preset.shader;
            bool isoutl         = preset.outline == -1 ? material.shader.name.Contains("Outline") : (preset.outline == 1);
            bool istess         = preset.tessellation == -1 ? material.shader.name.Contains("Tessellation") : (preset.tessellation == 1);
            bool isstencil      = material.GetFloat("_StencilPass") == (float)UnityEngine.Rendering.StencilOp.Replace;

            bool islite         = material.shader.name.Contains("Lite");
            bool iscutout       = material.shader.name.Contains("Cutout");
            bool istransparent  = material.shader.name.Contains("Transparent");
            bool isrefr         = material.shader.name.Contains("Refraction");
            bool isblur         = material.shader.name.Contains("Blur");
            bool isfur          = material.shader.name.Contains("Fur");
            bool isonepass      = material.shader.name.Contains("OnePass");
            bool istwopass      = material.shader.name.Contains("TwoPass");

            RenderingMode           renderingMode = RenderingMode.Opaque;

            if(string.IsNullOrEmpty(preset.renderingMode) || !Enum.TryParse(preset.renderingMode, out renderingMode))
            {
                if(iscutout)            renderingMode = RenderingMode.Cutout;
                if(istransparent)       renderingMode = RenderingMode.Transparent;
                if(isrefr)              renderingMode = RenderingMode.Refraction;
                if(isrefr && isblur)    renderingMode = RenderingMode.RefractionBlur;
                if(isfur)               renderingMode = RenderingMode.Fur;
                if(isfur && iscutout)   renderingMode = RenderingMode.FurCutout;
                if(isfur && istwopass)  renderingMode = RenderingMode.FurTwoPass;
            }

            TransparentMode         transparentMode = TransparentMode.Normal;
            if(isonepass)           transparentMode = TransparentMode.OnePass;
            if(!isfur && istwopass) transparentMode = TransparentMode.TwoPass;

            SetupMaterialWithRenderingMode(material, renderingMode, transparentMode, isoutl, islite, isstencil, istess, ismulti);
            if(preset.renderQueue != -2) material.renderQueue = preset.renderQueue;

            for(int i = 0; i < preset.colors.Length;   i++) material.SetColor(preset.colors[i].name, preset.colors[i].value);
            for(int i = 0; i < preset.vectors.Length;  i++) material.SetVector(preset.vectors[i].name, preset.vectors[i].value);
            for(int i = 0; i < preset.floats.Length;   i++) material.SetFloat(preset.floats[i].name, preset.floats[i].value);
            for(int i = 0; i < preset.textures.Length; i++)
            {
                material.SetTexture(preset.textures[i].name, preset.textures[i].value);
                material.SetTextureOffset(preset.textures[i].name, preset.textures[i].offset);
                material.SetTextureScale(preset.textures[i].name, preset.textures[i].scale);
            }

            if(preset.outlineMainTex) material.SetTexture("_OutlineTex", material.GetTexture("_MainTex"));
        }

        public static void ApplyPreset(Material material, lilToonPreset preset)
        {
            ApplyPreset(material, preset, isMulti);
        }

        private static void DrawPreset()
        {
            GUILayout.Label(GetLoc("sPresets"), boldLabel);
            if(presets == null) LoadPresets();
            ShowPresets();
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            if(EditorButton(GetLoc("sPresetRefresh"))) LoadPresets();
            if(EditorButton(GetLoc("sPresetSave"))) EditorWindow.GetWindow<lilPresetWindow>("[lilToon] Preset Window");
            GUILayout.EndHorizontal();
        }

        private static void LoadPresets()
        {
            string[] presetGuid = AssetDatabase.FindAssets("t:lilToonPreset");
            Array.Resize(ref presets, presetGuid.Length);
            for(int i=0; i<presetGuid.Length; i++)
            {
                presets[i] = AssetDatabase.LoadAssetAtPath<lilToonPreset>(GUIDToPath(presetGuid[i]));
            }
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
                edSet.isShowCategorys[i] = Foldout(sCategorys[i], edSet.isShowCategorys[i]);
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
                                if(presets[j].bases[k].language == edSet.languageName)
                                {
                                    showName = presets[j].bases[k].name;
                                    k = 256;
                                }
                            }
                            if(EditorButton(showName))
                            {
                                var objs = m_MaterialEditor.targets.Where(obj => obj is Material);
                                foreach(UnityEngine.Object obj in objs)
                                {
                                    ApplyPreset((Material)obj, presets[j]);
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
            liteMaterial.renderQueue = material.renderQueue;
        }

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
            SetupMultiMaterial(material);
        }

        protected virtual void ReplaceToCustomShaders()
        {
        }

        protected void ConvertMaterialToCustomShader(Material material)
        {
            InitializeShaders();
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
                int renderQueue = material.renderQueue == material.shader.renderQueue ? -1 : material.renderQueue;
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
            }
        }

        private void DrawBaseSettings(Material material, string sCullModes, string sTransparentMode, string[] sRenderingModeList, string[] sRenderingModeListLite, string[] sTransparentModeList)
        {
            DrawRenderingModeSettings(material, sTransparentMode, sRenderingModeList, sRenderingModeListLite);

            EditorGUILayout.Space();
            GUILayout.Label(GetLoc("sBaseSetting"), boldLabel);

            edSet.isShowBase = Foldout(GetLoc("sBaseSetting"), edSet.isShowBase);
            DrawMenuButton(GetLoc("sAnchorBaseSetting"), lilPropertyBlock.Base);
            if(edSet.isShowBase)
            {
                EditorGUILayout.BeginVertical(customBox);
                    if(isMulti)
                    {
                        m_MaterialEditor.ShaderProperty(asOverlay, GetLoc("sAsOverlay"));
                    }
                    if(renderingModeBuf == RenderingMode.Transparent)
                    {
                        TransparentMode transparentMode = (TransparentMode)EditorGUILayout.Popup(GetLoc("sTransparentMode"), (int)transparentModeBuf, sTransparentModeList);
                        if(transparentModeBuf != transparentMode)
                        {
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentMode);
                        }
                        if(transparentModeBuf >= TransparentMode.OnePass && vertexLightStrength.floatValue != 1.0f && RPReader.GetRP() == lilRenderPipeline.BRP && AutoFixHelpBox(GetLoc("sHelpOnePassVertexLight")))
                        {
                            vertexLightStrength.floatValue = 1.0f;
                        }
                    }
                    if(isUseAlpha)
                    {
                        m_MaterialEditor.ShaderProperty(cutoff, GetLoc("sCutoff"));
                    }
                    if(transparentModeBuf == TransparentMode.TwoPass)
                    {
                        cull.floatValue = 2.0f;
                    }
                    else if(!isGem)
                    {
                        m_MaterialEditor.ShaderProperty(cull, sCullModes);
                        EditorGUI.indentLevel++;
                        if(cull.floatValue == 1.0f && AutoFixHelpBox(GetLoc("sHelpCullMode")))
                        {
                            cull.floatValue = 2.0f;
                        }
                        if(cull.floatValue <= 1.0f)
                        {
                            m_MaterialEditor.ShaderProperty(flipNormal, GetLoc("sFlipBackfaceNormal"));
                            m_MaterialEditor.ShaderProperty(backfaceForceShadow, GetLoc("sBackfaceForceShadow"));
                        }
                        EditorGUI.indentLevel--;
                    }
                    m_MaterialEditor.ShaderProperty(invisible, GetLoc("sInvisible"));
                    m_MaterialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                    if(zwrite.floatValue != 1.0f && !isGem && AutoFixHelpBox(GetLoc("sHelpZWrite")))
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
                        DrawLine();
                        m_MaterialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sTriMask"), GetLoc("sTriMaskRGB")), triMask);
                    }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawLightingSettings()
        {
            edSet.isShowLightingSettings = Foldout(GetLoc("sLightingSettings"), edSet.isShowLightingSettings);
            DrawMenuButton(GetLoc("sAnchorLighting"), lilPropertyBlock.Lighting);
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
                    if(GUI.Button(buttonRect1, new GUIContent(GetLoc("sLightingPresetDefault")))) ApplyLightingPreset(lilLightingPreset.Default);
                    if(GUI.Button(buttonRect2, new GUIContent(GetLoc("sLightingPresetSemiMonochrome")))) ApplyLightingPreset(lilLightingPreset.SemiMonochrome);
                    GUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                EditorGUILayout.LabelField(GetLoc("sAdvanced"));
                EditorGUILayout.BeginVertical(customBox);
                    m_MaterialEditor.ShaderProperty(asUnlit, GetLoc("sAsUnlit"));
                    if(asUnlit.floatValue != 0 && AutoFixHelpBox(GetLoc("sAsUnlitWarn")))
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
            edSet.isShowLightingSettings = Foldout(GetLoc("sLightingSettings"), edSet.isShowLightingSettings);
            DrawMenuButton(GetLoc("sAnchorLighting"), lilPropertyBlock.Lighting);
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
                    if(GUI.Button(buttonRect1, new GUIContent(GetLoc("sLightingPresetDefault")))) ApplyLightingPreset(lilLightingPreset.Default);
                    if(GUI.Button(buttonRect2, new GUIContent(GetLoc("sLightingPresetSemiMonochrome")))) ApplyLightingPreset(lilLightingPreset.SemiMonochrome);
                    GUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawShadowSettings()
        {
            edSet.isShowShadow = Foldout(GetLoc("sShadowSetting"), edSet.isShowShadow);
            DrawMenuButton(GetLoc("sAnchorShadow"), lilPropertyBlock.Shadow);
            if(edSet.isShowShadow)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                m_MaterialEditor.ShaderProperty(useShadow, GetLoc("sShadow"));
                DrawMenuButton(GetLoc("sAnchorShadow"), lilPropertyBlock.Shadow);
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
                    DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(shadow1stColorRGBAContent, shadowColorTex, shadowColor);
                    EditorGUI.indentLevel += 2;
                        m_MaterialEditor.ShaderProperty(shadowBorder, GetLoc("sBorder"));
                        m_MaterialEditor.ShaderProperty(shadowBlur, GetLoc("sBlur"));
                        m_MaterialEditor.ShaderProperty(shadowNormalStrength, GetLoc("sNormalStrength"));
                        m_MaterialEditor.ShaderProperty(shadowReceive, GetLoc("sReceiveShadow"));
                    EditorGUI.indentLevel -= 2;
                    DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(shadow2ndColorRGBAContent, shadow2ndColorTex, shadow2ndColor);
                    EditorGUI.indentLevel += 2;
                        DrawColorAsAlpha(shadow2ndColor);
                        m_MaterialEditor.ShaderProperty(shadow2ndBorder, GetLoc("sBorder"));
                        m_MaterialEditor.ShaderProperty(shadow2ndBlur, GetLoc("sBlur"));
                        m_MaterialEditor.ShaderProperty(shadow2ndNormalStrength, GetLoc("sNormalStrength"));
                        m_MaterialEditor.ShaderProperty(shadow2ndReceive, GetLoc("sReceiveShadow"));
                    EditorGUI.indentLevel -= 2;
                    DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(shadow3rdColorRGBAContent, shadow3rdColorTex, shadow3rdColor);
                    EditorGUI.indentLevel += 2;
                        DrawColorAsAlpha(shadow3rdColor);
                        m_MaterialEditor.ShaderProperty(shadow3rdBorder, GetLoc("sBorder"));
                        m_MaterialEditor.ShaderProperty(shadow3rdBlur, GetLoc("sBlur"));
                        m_MaterialEditor.ShaderProperty(shadow3rdNormalStrength, GetLoc("sNormalStrength"));
                        m_MaterialEditor.ShaderProperty(shadow3rdReceive, GetLoc("sReceiveShadow"));
                    EditorGUI.indentLevel -= 2;
                    DrawLine();
                    m_MaterialEditor.ShaderProperty(shadowBorderColor, GetLoc("sShadowBorderColor"));
                    m_MaterialEditor.ShaderProperty(shadowBorderRange, GetLoc("sShadowBorderRange"));
                    DrawLine();
                    m_MaterialEditor.ShaderProperty(shadowMainStrength, GetLoc("sContrast"));
                    m_MaterialEditor.ShaderProperty(shadowEnvStrength, GetLoc("sShadowEnvStrength"));
                    m_MaterialEditor.ShaderProperty(lilShadowCasterBias, "Shadow Caster Bias");
                    DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sBlurMask"), GetLoc("sBlurR")), shadowBlurMask);
                    m_MaterialEditor.ShaderProperty(shadowBlurMaskLOD, "LOD", 2);
                    DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(new GUIContent("AO Map", GetLoc("sBorderR")), shadowBorderMask);
                    EditorGUI.indentLevel += 2;
                        m_MaterialEditor.ShaderProperty(shadowBorderMaskLOD, "LOD");
                        m_MaterialEditor.ShaderProperty(shadowPostAO, GetLoc("sIgnoreBorderProperties"));
                        float min1 = GetRemapMinValue(shadowAOShift.vectorValue.x, shadowAOShift.vectorValue.y);
                        float max1 = GetRemapMaxValue(shadowAOShift.vectorValue.x, shadowAOShift.vectorValue.y);
                        float min2 = GetRemapMinValue(shadowAOShift.vectorValue.z, shadowAOShift.vectorValue.w);
                        float max2 = GetRemapMaxValue(shadowAOShift.vectorValue.z, shadowAOShift.vectorValue.w);
                        float min3 = GetRemapMinValue(shadowAOShift2.vectorValue.x, shadowAOShift2.vectorValue.y);
                        float max3 = GetRemapMaxValue(shadowAOShift2.vectorValue.x, shadowAOShift2.vectorValue.y);
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
                                GetRemapScaleValue(min1, max1),
                                GetRemapOffsetValue(min1, max1),
                                GetRemapScaleValue(min2, max2),
                                GetRemapOffsetValue(min2, max2)
                            );
                            shadowAOShift2.vectorValue = new Vector4(
                                GetRemapScaleValue(min3, max3),
                                GetRemapOffsetValue(min3, max3),
                                0.0f,
                                0.0f
                            );
                        }
                    EditorGUI.indentLevel -= 2;
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
                    DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(shadow2ndColorRGBAContent, shadow2ndColorTex);
                    EditorGUI.indentLevel += 2;
                    m_MaterialEditor.ShaderProperty(shadow2ndBorder, GetLoc("sBorder"));
                    m_MaterialEditor.ShaderProperty(shadow2ndBlur, GetLoc("sBlur"));
                    EditorGUI.indentLevel -= 2;
                    DrawLine();
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
            edSet.isShowShadow = Foldout(GetLoc("sShadowSetting"), edSet.isShowShadow);
            DrawMenuButton(GetLoc("sAnchorShadow"), lilPropertyBlock.Shadow);
            if(edSet.isShowShadow)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                m_MaterialEditor.ShaderProperty(useShadow, GetLoc("sShadow"));
                DrawMenuButton(GetLoc("sAnchorShadow"), lilPropertyBlock.Shadow);
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
                    DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(shadow1stColorRGBAContent, shadowColorTex, shadowColor);
                    EditorGUI.indentLevel += 2;
                        m_MaterialEditor.ShaderProperty(shadowBorder, GetLoc("sBorder"));
                        m_MaterialEditor.ShaderProperty(shadowBlur, GetLoc("sBlur"));
                        m_MaterialEditor.ShaderProperty(shadowNormalStrength, GetLoc("sNormalStrength"));
                        m_MaterialEditor.ShaderProperty(shadowReceive, GetLoc("sReceiveShadow"));
                    EditorGUI.indentLevel -= 2;
                    DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(shadow2ndColorRGBAContent, shadow2ndColorTex, shadow2ndColor);
                    EditorGUI.indentLevel += 2;
                        DrawColorAsAlpha(shadow2ndColor);
                        m_MaterialEditor.ShaderProperty(shadow2ndBorder, GetLoc("sBorder"));
                        m_MaterialEditor.ShaderProperty(shadow2ndBlur, GetLoc("sBlur"));
                        m_MaterialEditor.ShaderProperty(shadow2ndNormalStrength, GetLoc("sNormalStrength"));
                        m_MaterialEditor.ShaderProperty(shadow2ndReceive, GetLoc("sReceiveShadow"));
                    EditorGUI.indentLevel -= 2;
                    DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(shadow3rdColorRGBAContent, shadow3rdColorTex, shadow3rdColor);
                    EditorGUI.indentLevel += 2;
                        DrawColorAsAlpha(shadow3rdColor);
                        m_MaterialEditor.ShaderProperty(shadow3rdBorder, GetLoc("sBorder"));
                        m_MaterialEditor.ShaderProperty(shadow3rdBlur, GetLoc("sBlur"));
                        m_MaterialEditor.ShaderProperty(shadow3rdNormalStrength, GetLoc("sNormalStrength"));
                        m_MaterialEditor.ShaderProperty(shadow3rdReceive, GetLoc("sReceiveShadow"));
                    EditorGUI.indentLevel -= 2;
                    DrawLine();
                    m_MaterialEditor.ShaderProperty(shadowBorderColor, GetLoc("sShadowBorderColor"));
                    m_MaterialEditor.ShaderProperty(shadowBorderRange, GetLoc("sShadowBorderRange"));
                    DrawLine();
                    m_MaterialEditor.ShaderProperty(shadowMainStrength, GetLoc("sContrast"));
                    m_MaterialEditor.ShaderProperty(shadowEnvStrength, GetLoc("sShadowEnvStrength"));
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
                    DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(shadow2ndColorRGBAContent, shadow2ndColorTex);
                    EditorGUI.indentLevel += 2;
                        m_MaterialEditor.ShaderProperty(shadow2ndBorder, GetLoc("sBorder"));
                        m_MaterialEditor.ShaderProperty(shadow2ndBlur, GetLoc("sBlur"));
                    EditorGUI.indentLevel -= 2;
                    DrawLine();
                    m_MaterialEditor.ShaderProperty(shadowEnvStrength, GetLoc("sShadowEnvStrength"));
                    m_MaterialEditor.ShaderProperty(shadowBorderColor, GetLoc("sShadowBorderColor"));
                    m_MaterialEditor.ShaderProperty(shadowBorderRange, GetLoc("sShadowBorderRange"));
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawGlitterSettings()
        {
            edSet.isShowGlitter = Foldout(GetLoc("sGlitterSetting"), edSet.isShowGlitter);
            DrawMenuButton(GetLoc("sAnchorGlitter"), lilPropertyBlock.Glitter);
            if(edSet.isShowGlitter)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                m_MaterialEditor.ShaderProperty(useGlitter, GetLoc("sGlitter"));
                if(useGlitter.floatValue == 1)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    m_MaterialEditor.ShaderProperty(glitterUVMode, "UV Mode|UV0|UV1");
                    TextureGUI(ref edSet.isShowGlitterColorTex, colorMaskRGBAContent, glitterColorTex, glitterColor);
                    EditorGUI.indentLevel++;
                    DrawColorAsAlpha(glitterColor);
                    m_MaterialEditor.ShaderProperty(glitterMainStrength, GetLoc("sMainColorPower"));
                    m_MaterialEditor.ShaderProperty(glitterEnableLighting, GetLoc("sEnableLighting"));
                    m_MaterialEditor.ShaderProperty(glitterShadowMask, GetLoc("sShadowMask"));
                    m_MaterialEditor.ShaderProperty(glitterBackfaceMask, GetLoc("sBackfaceMask"));
                    if(isTransparent) m_MaterialEditor.ShaderProperty(glitterApplyTransparency, GetLoc("sApplyTransparency"));
                    EditorGUI.indentLevel--;
                    DrawLine();

                    // Param1
                    Vector2 scale = new Vector2(256.0f/glitterParams1.vectorValue.x, 256.0f/glitterParams1.vectorValue.y);
                    float size = glitterParams1.vectorValue.z == 0.0f ? 0.0f : Mathf.Sqrt(glitterParams1.vectorValue.z);
                    float density = Mathf.Sqrt(1.0f / glitterParams1.vectorValue.w) / 1.5f;
                    float sensitivity = RoundFloat1000000(glitterSensitivity.floatValue / density);
                    EditorGUIUtility.wideMode = true;

                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = glitterParams1.hasMixedValue || glitterSensitivity.hasMixedValue;
                    scale = EditorGUILayout.Vector2Field(GetLoc("sScale"), scale);
                    size = EditorGUILayout.Slider(GetLoc("sParticleSize"), size, 0.0f, 2.0f);
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
            edSet.isShowOutline = Foldout(GetLoc("sOutlineSetting"), edSet.isShowOutline);
            DrawMenuButton(GetLoc("sAnchorOutline"), lilPropertyBlock.Outline);
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
                if(isOutl)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    TextureGUI(ref edSet.isShowOutlineMap, mainColorRGBAContent, outlineTex, outlineColor, outlineTex_ScrollRotate, true, true);
                    EditorGUI.indentLevel++;
                    ToneCorrectionGUI(outlineTexHSVG);
                    if(EditorButton(GetLoc("sBake")))
                    {
                        outlineTex.textureValue = AutoBakeOutlineTexture(material);
                        outlineTexHSVG.vectorValue = defaultHSVG;
                    }
                    EditorGUI.indentLevel--;
                    DrawLine();
                    GUILayout.Label(GetLoc("sHighlight"), boldLabel);
                    EditorGUI.indentLevel++;
                    m_MaterialEditor.ShaderProperty(outlineLitColor, GetLoc("sColor"));
                    DrawColorAsAlpha(outlineLitColor);
                    m_MaterialEditor.ShaderProperty(outlineLitApplyTex, GetLoc("sColorFromMain"));
                    float min = GetRemapMinValue(outlineLitScale.floatValue, outlineLitOffset.floatValue);
                    float max = GetRemapMaxValue(outlineLitScale.floatValue, outlineLitOffset.floatValue);
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = alphaMaskScale.hasMixedValue || alphaMaskValue.hasMixedValue;
                    min = EditorGUILayout.Slider("Min", min, -0.01f, 1.01f);
                    max = EditorGUILayout.Slider("Max", max, -0.01f, 1.01f);
                    EditorGUI.showMixedValue = false;
                    if(EditorGUI.EndChangeCheck())
                    {
                        if(min == max) max += 0.001f;
                        outlineLitScale.floatValue = GetRemapScaleValue(min, max);
                        outlineLitOffset.floatValue = GetRemapOffsetValue(min, max);
                    }
                    EditorGUI.indentLevel--;
                    DrawLine();
                    m_MaterialEditor.ShaderProperty(outlineEnableLighting, GetLoc("sEnableLighting"));
                    DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sWidth"), GetLoc("sWidthR")), outlineWidthMask, outlineWidth);
                    EditorGUI.indentLevel++;
                    m_MaterialEditor.ShaderProperty(outlineFixWidth, GetLoc("sFixWidth"));
                    m_MaterialEditor.ShaderProperty(outlineVertexR2Width, sOutlineVertexColorUsages);
                    m_MaterialEditor.ShaderProperty(outlineDeleteMesh, GetLoc("sDeleteMesh0"));
                    m_MaterialEditor.ShaderProperty(outlineZBias, "Z Bias");
                    EditorGUI.indentLevel--;
                    m_MaterialEditor.TexturePropertySingleLine(normalMapContent, outlineVectorTex, outlineVectorScale);
                    m_MaterialEditor.ShaderProperty(outlineVectorUVMode, "UV Mode|UV0|UV1|UV2|UV3", 1);
                    EditorGUILayout.EndVertical();
                }
                else if(isLite && isOutl)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    TextureGUI(ref edSet.isShowOutlineMap, mainColorRGBAContent, outlineTex, outlineColor, outlineTex_ScrollRotate, true, true);
                    m_MaterialEditor.ShaderProperty(outlineEnableLighting, GetLoc("sEnableLighting"));
                    DrawLine();
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
            edSet.isShowOutline = Foldout(GetLoc("sOutlineSetting"), edSet.isShowOutline);
            DrawMenuButton(GetLoc("sAnchorOutline"), lilPropertyBlock.Outline);
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
                if(isOutl)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    TextureGUI(ref edSet.isShowOutlineMap, mainColorRGBAContent, outlineTex, outlineColor, outlineTex_ScrollRotate, true, true);
                    EditorGUI.indentLevel++;
                    ToneCorrectionGUI(outlineTexHSVG);
                    if(EditorButton(GetLoc("sBake")))
                    {
                        outlineTex.textureValue = AutoBakeOutlineTexture(material);
                        outlineTexHSVG.vectorValue = defaultHSVG;
                    }
                    EditorGUI.indentLevel--;
                    m_MaterialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sWidth"), GetLoc("sWidthR")), outlineWidthMask, outlineWidth);
                    EditorGUILayout.EndVertical();
                }
                else if(isLite && isOutl)
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
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Property drawer
        #region
        private void UV4Decal(MaterialProperty isDecal, MaterialProperty isLeftOnly, MaterialProperty isRightOnly, MaterialProperty shouldCopy, MaterialProperty shouldFlipMirror, MaterialProperty shouldFlipCopy, MaterialProperty tex, MaterialProperty angle, MaterialProperty decalAnimation, MaterialProperty decalSubParam, MaterialProperty uvMode)
        {
            m_MaterialEditor.ShaderProperty(uvMode, "UV Mode|UV0|UV1|UV2|UV3|MatCap");
            #if SYSTEM_DRAWING
            ConvertGifToAtlas(tex, decalAnimation, decalSubParam, isDecal);
            #endif
            // Toggle decal
            EditorGUI.BeginChangeCheck();
            m_MaterialEditor.ShaderProperty(isDecal, GetLoc("sAsDecal"));
            if(EditorGUI.EndChangeCheck() && isDecal.floatValue == 0.0f)
            {
                isLeftOnly.floatValue = 0.0f;
                isRightOnly.floatValue = 0.0f;
                shouldFlipMirror.floatValue = 0.0f;
                shouldCopy.floatValue = 0.0f;
                shouldFlipCopy.floatValue = 0.0f;
            }

            if(isDecal.floatValue == 1.0f)
            {
                EditorGUI.indentLevel++;
                // Mirror mode
                // 0 : Normal
                // 1 : Flip
                // 2 : Left only
                // 3 : Right only
                // 4 : Right only (Flip)
                int mirrorMode = 0;
                if(isRightOnly.floatValue == 1.0f) mirrorMode = 3;
                if(shouldFlipMirror.floatValue == 1.0f) mirrorMode++;
                if(isLeftOnly.floatValue == 1.0f) mirrorMode = 2;

                EditorGUI.BeginChangeCheck();
                mirrorMode = EditorGUILayout.Popup(GetLoc("sMirrorMode"),mirrorMode,new string[]{GetLoc("sMirrorModeNormal"),GetLoc("sMirrorModeFlip"),GetLoc("sMirrorModeLeft"),GetLoc("sMirrorModeRight"),GetLoc("sMirrorModeRightFlip")});
                if(EditorGUI.EndChangeCheck())
                {
                    if(mirrorMode == 0)
                    {
                        isLeftOnly.floatValue = 0.0f;
                        isRightOnly.floatValue = 0.0f;
                        shouldFlipMirror.floatValue = 0.0f;
                    }
                    if(mirrorMode == 1)
                    {
                        isLeftOnly.floatValue = 0.0f;
                        isRightOnly.floatValue = 0.0f;
                        shouldFlipMirror.floatValue = 1.0f;
                    }
                    if(mirrorMode == 2)
                    {
                        isLeftOnly.floatValue = 1.0f;
                        isRightOnly.floatValue = 0.0f;
                        shouldFlipMirror.floatValue = 0.0f;
                    }
                    if(mirrorMode == 3)
                    {
                        isLeftOnly.floatValue = 0.0f;
                        isRightOnly.floatValue = 1.0f;
                        shouldFlipMirror.floatValue = 0.0f;
                    }
                    if(mirrorMode == 4)
                    {
                        isLeftOnly.floatValue = 0.0f;
                        isRightOnly.floatValue = 1.0f;
                        shouldFlipMirror.floatValue = 1.0f;
                    }
                }

                // Copy mode
                // 0 : Normal
                // 1 : Symmetry
                // 2 : Flip
                int copyMode = 0;
                if(shouldCopy.floatValue == 1.0f) copyMode = 1;
                if(shouldFlipCopy.floatValue == 1.0f) copyMode = 2;

                EditorGUI.BeginChangeCheck();
                copyMode = EditorGUILayout.Popup(GetLoc("sCopyMode"),copyMode,new string[]{GetLoc("sCopyModeNormal"),GetLoc("sCopyModeSymmetry"),GetLoc("sCopyModeFlip")});
                if(EditorGUI.EndChangeCheck())
                {
                    if(copyMode == 0)
                    {
                        shouldCopy.floatValue = 0.0f;
                        shouldFlipCopy.floatValue = 0.0f;
                    }
                    if(copyMode == 1)
                    {
                        shouldCopy.floatValue = 1.0f;
                        shouldFlipCopy.floatValue = 0.0f;
                    }
                    if(copyMode == 2)
                    {
                        shouldCopy.floatValue = 1.0f;
                        shouldFlipCopy.floatValue = 1.0f;
                    }
                }

                // Load scale & offset
                float scaleX = tex.textureScaleAndOffset.x;
                float scaleY = tex.textureScaleAndOffset.y;
                float posX = tex.textureScaleAndOffset.z;
                float posY = tex.textureScaleAndOffset.w;

                // scale & offset -> scale & position
                if(scaleX==0.0f)
                {
                    posX = 0.5f;
                    scaleX = 0.000001f;
                }
                else
                {
                    posX = (0.5f - posX) / scaleX;
                    scaleX = 1.0f / scaleX;
                }

                if(scaleY==0.0f)
                {
                    posY = 0.5f;
                    scaleY = 0.000001f;
                }
                else
                {
                    posY = (0.5f - posY) / scaleY;
                    scaleY = 1.0f / scaleY;
                }
                scaleX = RoundFloat1000000(scaleX);
                scaleY = RoundFloat1000000(scaleY);
                posX = RoundFloat1000000(posX);
                posY = RoundFloat1000000(posY);

                // If uv copy enables, fix position
                EditorGUI.BeginChangeCheck();
                if(copyMode > 0)
                {
                    if(posX < 0.5f) posX = 1.0f - posX;
                    posX = EditorGUILayout.Slider(GetLoc("sPositionX"), posX, 0.5f, 1.0f);
                }
                else
                {
                    posX = EditorGUILayout.Slider(GetLoc("sPositionX"), posX, 0.0f, 1.0f);
                }

                // Draw properties
                posY = EditorGUILayout.Slider(GetLoc("sPositionY"), posY, 0.0f, 1.0f);
                scaleX = EditorGUILayout.Slider(GetLoc("sScaleX"), scaleX, -1.0f, 1.0f);
                scaleY = EditorGUILayout.Slider(GetLoc("sScaleY"), scaleY, -1.0f, 1.0f);
                if(EditorGUI.EndChangeCheck())
                {
                    // Avoid division by zero
                    if(scaleX == 0.0f) scaleX = 0.000001f;
                    if(scaleY == 0.0f) scaleY = 0.000001f;

                    // scale & position -> scale & offset
                    scaleX = 1.0f / scaleX;
                    scaleY = 1.0f / scaleY;
                    posX = (-posX * scaleX) + 0.5f;
                    posY = (-posY * scaleY) + 0.5f;

                    tex.textureScaleAndOffset = new Vector4(scaleX, scaleY, posX, posY);
                }

                m_MaterialEditor.ShaderProperty(angle, GetLoc("sAngle"));
                EditorGUI.indentLevel--;
                m_MaterialEditor.ShaderProperty(decalAnimation, BuildParams(GetLoc("sAnimation"), GetLoc("sXFrames"), GetLoc("sYFrames"), GetLoc("sFrames"), GetLoc("sFPS")));
                m_MaterialEditor.ShaderProperty(decalSubParam, BuildParams(GetLoc("sXRatio"), GetLoc("sYRatio"), GetLoc("sFixBorder")));
            }
            else
            {
                m_MaterialEditor.TextureScaleOffsetProperty(tex);
                m_MaterialEditor.ShaderProperty(angle, GetLoc("sAngle"));
            }

            if(EditorButton(GetLoc("sReset")) && EditorUtility.DisplayDialog(GetLoc("sDialogResetUV"),GetLoc("sDialogResetUVMes"),GetLoc("sYes"),GetLoc("sNo")))
            {
                uvMode.floatValue = 0.0f;
                isDecal.floatValue = 0.0f;
                isLeftOnly.floatValue = 0.0f;
                isRightOnly.floatValue = 0.0f;
                shouldCopy.floatValue = 0.0f;
                shouldFlipMirror.floatValue = 0.0f;
                shouldFlipCopy.floatValue = 0.0f;
                angle.floatValue = 0.0f;
                decalAnimation.vectorValue = new Vector4(1.0f,1.0f,1.0f,30.0f);
                decalSubParam.vectorValue = new Vector4(1.0f,1.0f,0.01f,1.0f);
            }
        }

        private void ToneCorrectionGUI(MaterialProperty hsvg)
        {
            m_MaterialEditor.ShaderProperty(hsvg, BuildParams(GetLoc("sHue"), GetLoc("sSaturation"), GetLoc("sValue"), GetLoc("sGamma")));
            // Reset
            if(EditorButton(GetLoc("sReset")))
            {
                hsvg.vectorValue = defaultHSVG;
            }
        }

        private void ToneCorrectionGUI(MaterialProperty hsvg, int indent)
        {
            EditorGUI.indentLevel += indent;
            ToneCorrectionGUI(hsvg);
            EditorGUI.indentLevel -= indent;
        }

        private void DrawVectorAs4Float(MaterialProperty prop, string label0, string label1, string label2, string label3)
        {
            float param1 = prop.vectorValue.x;
            float param2 = prop.vectorValue.y;
            float param3 = prop.vectorValue.z;
            float param4 = prop.vectorValue.w;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            param1 = EditorGUILayout.FloatField(label0, param1);
            param2 = EditorGUILayout.FloatField(label1, param2);
            param3 = EditorGUILayout.FloatField(label2, param3);
            param4 = EditorGUILayout.FloatField(label3, param4);
            EditorGUI.showMixedValue = false;

            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(param1, param2, param3, param4);
            }
        }

        private void DrawColorAsAlpha(MaterialProperty prop, string label)
        {
            float alpha = prop.colorValue.a;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            alpha = EditorGUILayout.Slider(label, alpha, 0.0f, 1.0f);
            EditorGUI.showMixedValue = false;

            if(EditorGUI.EndChangeCheck())
            {
                prop.colorValue = new Color(prop.colorValue.r, prop.colorValue.g, prop.colorValue.b, alpha);
            }
        }

        private void DrawColorAsAlpha(MaterialProperty prop)
        {
            DrawColorAsAlpha(prop, GetLoc("sAlpha"));
        }

        private void RemoveUnusedPropertiesGUI(Material material)
        {
            if(EditorButton(GetLoc("sRemoveUnused")))
            {
                Undo.RecordObject(material, "Remove unused properties");
                RemoveUnusedTexture(material, isLite);
            }
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
            if(EditorButton(sBake[bakeType]))
            {
                Undo.RecordObject(material, "Bake");
                TextureBake(material, bakeType);
            }
        }

        private void AlphamaskToTextureGUI(Material material)
        {
            if(mainTex.textureValue != null && EditorButton(GetLoc("sBakeAlphamask")))
            {
                Texture2D bakedTexture = AutoBakeAlphaMask(material);
                if(bakedTexture == mainTex.textureValue) return;

                mainTex.textureValue = bakedTexture;
                alphaMaskMode.floatValue = 0.0f;
                alphaMask.textureValue = null;
                alphaMaskValue.floatValue = 0.0f;
            }
        }

        private void SetAlphaIsTransparencyGUI(MaterialProperty tex)
        {
            if(tex.textureValue != null && !((Texture2D)tex.textureValue).alphaIsTransparency && AutoFixHelpBox(GetLoc("sNotAlphaIsTransparency")))
            {
                string path = AssetDatabase.GetAssetPath(tex.textureValue);
                TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(path);
                textureImporter.alphaIsTransparency = true;
                AssetDatabase.ImportAsset(path);
            }
        }

        private void UVSettingGUI(MaterialProperty uvst, MaterialProperty uvsr)
        {
            EditorGUILayout.LabelField(GetLoc("sUVSetting"), boldLabel);
            m_MaterialEditor.TextureScaleOffsetProperty(uvst);
            m_MaterialEditor.ShaderProperty(uvsr, BuildParams(GetLoc("sAngle"), GetLoc("sUVAnimation"), GetLoc("sScroll"), GetLoc("sRotate")));
        }

        private void InvBorderGUI(MaterialProperty prop)
        {
            float f = prop.floatValue;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            f = 1.0f - EditorGUILayout.Slider(GetLoc("sBorder"), 1.0f - f, 0.0f, 1.0f);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.floatValue = f;
            }
        }

        private void MinusRangeGUI(MaterialProperty prop, string label)
        {
            float f = -prop.floatValue;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            f = EditorGUILayout.Slider(label, f, 0.0f, 1.0f);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.floatValue = -f;
            }
        }

        private void BlendSettingGUI(ref bool isShow, string labelName, MaterialProperty srcRGB, MaterialProperty dstRGB, MaterialProperty srcA, MaterialProperty dstA, MaterialProperty opRGB, MaterialProperty opA)
        {
            DrawSimpleFoldout(labelName, ref isShow, isCustomEditor);
            if(isShow)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.ShaderProperty(srcRGB, GetLoc("sSrcBlendRGB"));
                m_MaterialEditor.ShaderProperty(dstRGB, GetLoc("sDstBlendRGB"));
                m_MaterialEditor.ShaderProperty(srcA, GetLoc("sSrcBlendAlpha"));
                m_MaterialEditor.ShaderProperty(dstA, GetLoc("sDstBlendAlpha"));
                m_MaterialEditor.ShaderProperty(opRGB, GetLoc("sBlendOpRGB"));
                m_MaterialEditor.ShaderProperty(opA, GetLoc("sBlendOpAlpha"));
                EditorGUI.indentLevel--;
            }
        }

        private void TextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName)
        {
            DrawSimpleFoldout(guiContent, textureName, ref isShow, isCustomEditor);
            if(isShow)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(textureName);
                EditorGUI.indentLevel--;
            }
        }

        private void TextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba)
        {
            DrawSimpleFoldout(guiContent, textureName, rgba, ref isShow, isCustomEditor);
            if(isShow)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(textureName);
                EditorGUI.indentLevel--;
            }
        }

        private void TextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty scrollRotate)
        {
            DrawSimpleFoldout(guiContent, textureName, rgba, ref isShow, isCustomEditor);
            if(isShow)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(textureName);
                m_MaterialEditor.ShaderProperty(scrollRotate, GetLoc("sScroll"));
                EditorGUI.indentLevel--;
            }
        }

        private void TextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty scrollRotate, bool useCustomUV, bool useUVAnimation)
        {
            if(useCustomUV)
            {
                DrawSimpleFoldout(guiContent, textureName, rgba, ref isShow, isCustomEditor);
                if(isShow)
                {
                    EditorGUI.indentLevel++;
                    if(useUVAnimation)  UVSettingGUI(textureName, scrollRotate);
                    else                m_MaterialEditor.TextureScaleOffsetProperty(textureName);
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                m_MaterialEditor.TexturePropertySingleLine(guiContent, textureName, rgba);
            }
        }

        private void TextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty scrollRotate, MaterialProperty uvMode, bool useCustomUV, bool useUVAnimation)
        {
            if(useCustomUV)
            {
                DrawSimpleFoldout(guiContent, textureName, rgba, ref isShow, isCustomEditor);
                if(isShow)
                {
                    EditorGUI.indentLevel++;
                    if(useUVAnimation)  UVSettingGUI(textureName, scrollRotate);
                    else                m_MaterialEditor.TextureScaleOffsetProperty(textureName);
                    m_MaterialEditor.ShaderProperty(uvMode, "UV Mode|UV0|UV1|UV2|UV3|Rim");
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                m_MaterialEditor.TexturePropertySingleLine(guiContent, textureName, rgba);
            }
        }

        private void MatCapTextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty blendUV1, MaterialProperty zRotCancel, MaterialProperty perspective, MaterialProperty vrParallaxStrength)
        {
            DrawSimpleFoldout(guiContent, textureName, ref isShow, isCustomEditor);
            if(isShow)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(textureName);
                m_MaterialEditor.ShaderProperty(blendUV1, GetLoc("sBlendUV1"));
                m_MaterialEditor.ShaderProperty(zRotCancel, GetLoc("sMatCapZRotCancel"));
                m_MaterialEditor.ShaderProperty(perspective, GetLoc("sFixPerspective"));
                m_MaterialEditor.ShaderProperty(vrParallaxStrength, GetLoc("sVRParallaxStrength"));
                EditorGUI.indentLevel--;
            }
        }

        private void MatCapTextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty blendUV1, MaterialProperty zRotCancel, MaterialProperty perspective, MaterialProperty vrParallaxStrength)
        {
            DrawSimpleFoldout(guiContent, textureName, rgba, ref isShow, isCustomEditor);
            if(isShow)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(textureName);
                m_MaterialEditor.ShaderProperty(blendUV1, GetLoc("sBlendUV1"));
                m_MaterialEditor.ShaderProperty(zRotCancel, GetLoc("sMatCapZRotCancel"));
                m_MaterialEditor.ShaderProperty(perspective, GetLoc("sFixPerspective"));
                m_MaterialEditor.ShaderProperty(vrParallaxStrength, GetLoc("sVRParallaxStrength"));

                GUILayout.BeginHorizontal();
                Rect position2 = EditorGUILayout.GetControlRect();
                Rect labelRect = new Rect(position2.x, position2.y, EditorGUIUtility.labelWidth, position2.height);
                Rect buttonRect1 = new Rect(labelRect.x + labelRect.width, position2.y, (position2.width - EditorGUIUtility.labelWidth)*0.5f, position2.height);
                Rect buttonRect2 = new Rect(buttonRect1.x + buttonRect1.width, position2.y, buttonRect1.width, position2.height);
                EditorGUI.PrefixLabel(labelRect, new GUIContent("UV Preset"));
                if(GUI.Button(buttonRect1, new GUIContent("MatCap"))) ApplyMatCapUVPreset(false, blendUV1, zRotCancel, perspective, vrParallaxStrength);
                if(GUI.Button(buttonRect2, new GUIContent("AngelRing"))) ApplyMatCapUVPreset(true, blendUV1, zRotCancel, perspective, vrParallaxStrength);
                GUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }
        }

        private void ApplyMatCapUVPreset(bool isAngelRing, MaterialProperty blendUV1, MaterialProperty zRotCancel, MaterialProperty perspective, MaterialProperty vrParallaxStrength)
        {
            if(isAngelRing)
            {
                blendUV1.vectorValue = new Vector4(0.0f, 1.0f, 0.0f, 0.0f);
                zRotCancel.floatValue = 1.0f;
                perspective.floatValue = 0.0f;
                vrParallaxStrength.floatValue = 0.0f;
            }
            else
            {
                blendUV1.vectorValue = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
                zRotCancel.floatValue = 1.0f;
                perspective.floatValue = 1.0f;
                vrParallaxStrength.floatValue = 1.0f;
            }
        }

        private float GetRemapMinValue(float scale, float offset)
        {
            return RoundFloat1000000(Mathf.Clamp(-offset / scale, -0.01f, 1.01f));
        }

        private float GetRemapMaxValue(float scale, float offset)
        {
            return RoundFloat1000000(Mathf.Clamp((1.0f - offset) / scale, -0.01f, 1.01f));
        }

        private float GetRemapScaleValue(float min, float max)
        {
            return 1.0f / (max - min);
        }

        private float GetRemapOffsetValue(float min, float max)
        {
            return min / (min - max);
        }

        private float RoundFloat1000000(float val)
        {
            return Mathf.Floor(val * 1000000.0f + 0.5f) * 0.000001f;
        }

        private static bool EditorButton(string label)
        {
            return GUI.Button(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect()), label);
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Bake
        #region
        private void TextureBake(Material material, int bakeType)
        {
            //bool shouldBake1st = (bakeType == 1 || bakeType == 4) && mainTex.textureValue != null;
            bool shouldNotBakeColor = (bakeType == 1 || bakeType == 4) && mainColor.colorValue == Color.white && mainTexHSVG.vectorValue == defaultHSVG;
            bool cannotBake1st = mainTex.textureValue == null;
            bool shouldNotBake2nd = (bakeType == 2 || bakeType == 5) && useMain2ndTex.floatValue == 0.0;
            bool shouldNotBake3rd = (bakeType == 3 || bakeType == 6) && useMain3rdTex.floatValue == 0.0;
            bool shouldNotBakeAll = bakeType == 0 && mainColor.colorValue == Color.white && mainTexHSVG.vectorValue == defaultHSVG && useMain2ndTex.floatValue == 0.0 && useMain3rdTex.floatValue == 0.0;
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
                    LoadTexture(ref srcTexture, path);
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
                        LoadTexture(ref srcMain2, path);
                        hsvgMaterial.SetTexture(main2ndTex.name, srcMain2);
                    }
                    else
                    {
                        hsvgMaterial.SetTexture(main2ndTex.name, Texture2D.whiteTexture);
                    }

                    path = AssetDatabase.GetAssetPath(material.GetTexture(main2ndBlendMask.name));
                    if(!string.IsNullOrEmpty(path))
                    {
                        LoadTexture(ref srcMask2, path);
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
                        LoadTexture(ref srcMain3, path);
                        hsvgMaterial.SetTexture(main3rdTex.name, srcMain3);
                    }
                    else
                    {
                        hsvgMaterial.SetTexture(main3rdTex.name, Texture2D.whiteTexture);
                    }

                    path = AssetDatabase.GetAssetPath(material.GetTexture(main3rdBlendMask.name));
                    if(!string.IsNullOrEmpty(path))
                    {
                        LoadTexture(ref srcMask3, path);
                        hsvgMaterial.SetTexture(main3rdBlendMask.name, srcMask3);
                    }
                    else
                    {
                        hsvgMaterial.SetTexture(main3rdBlendMask.name, Texture2D.whiteTexture);
                    }
                }

                Texture2D outTexture = null;
                RunBake(ref outTexture, srcTexture, hsvgMaterial);

                outTexture = SaveTextureToPng(material, outTexture, mainTex.name);
                if(outTexture != mainTex.textureValue)
                {
                    mainTexHSVG.vectorValue = defaultHSVG;
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

                UnityEngine.Object.DestroyImmediate(hsvgMaterial);
                UnityEngine.Object.DestroyImmediate(srcTexture);
                UnityEngine.Object.DestroyImmediate(srcMain2);
                UnityEngine.Object.DestroyImmediate(srcMain3);
                UnityEngine.Object.DestroyImmediate(srcMask2);
                UnityEngine.Object.DestroyImmediate(srcMask3);
            }
        }

        private Texture2D AutoBakeMainTexture(Material material)
        {
            bool shouldNotBakeAll = mainColor.colorValue == Color.white && mainTexHSVG.vectorValue == defaultHSVG && useMain2ndTex.floatValue == 0.0 && useMain3rdTex.floatValue == 0.0;
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
                    LoadTexture(ref srcTexture, path);
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
                        LoadTexture(ref srcMain2, path);
                        hsvgMaterial.SetTexture(main2ndTex.name, srcMain2);
                    }
                    else
                    {
                        hsvgMaterial.SetTexture(main2ndTex.name, Texture2D.whiteTexture);
                    }

                    path = AssetDatabase.GetAssetPath(material.GetTexture(main2ndBlendMask.name));
                    if(!string.IsNullOrEmpty(path))
                    {
                        LoadTexture(ref srcMask2, path);
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
                        LoadTexture(ref srcMain3, path);
                        hsvgMaterial.SetTexture(main3rdTex.name, srcMain3);
                    }
                    else
                    {
                        hsvgMaterial.SetTexture(main3rdTex.name, Texture2D.whiteTexture);
                    }

                    path = AssetDatabase.GetAssetPath(material.GetTexture(main3rdBlendMask.name));
                    if(!string.IsNullOrEmpty(path))
                    {
                        LoadTexture(ref srcMask3, path);
                        hsvgMaterial.SetTexture(main3rdBlendMask.name, srcMask3);
                    }
                    else
                    {
                        hsvgMaterial.SetTexture(main3rdBlendMask.name, Texture2D.whiteTexture);
                    }
                }

                Texture2D outTexture = null;
                RunBake(ref outTexture, srcTexture, hsvgMaterial);

                outTexture = SaveTextureToPng(material, outTexture, mainTex.name);
                if(outTexture != mainTex.textureValue)
                {
                    CopyTextureSetting(bufMainTexture, outTexture);
                }

                UnityEngine.Object.DestroyImmediate(hsvgMaterial);
                UnityEngine.Object.DestroyImmediate(srcTexture);
                UnityEngine.Object.DestroyImmediate(srcMain2);
                UnityEngine.Object.DestroyImmediate(srcMain3);
                UnityEngine.Object.DestroyImmediate(srcMask2);
                UnityEngine.Object.DestroyImmediate(srcMask3);

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
                hsvgMaterial.SetVector(mainTexHSVG.name,                defaultHSVG);
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
                    LoadTexture(ref srcMain2, path);
                    hsvgMaterial.SetTexture(main2ndTex.name, srcMain2);
                }

                path = AssetDatabase.GetAssetPath(bakedMainTex);
                if(!string.IsNullOrEmpty(path))
                {
                    LoadTexture(ref srcTexture, path);
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
                    LoadTexture(ref srcMask2, path);
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

                if(shadowType == 0) outTexture = SaveTextureToPng(material, outTexture, mainTex.name);
                if(shadowType == 1) outTexture = SaveTextureToPng(material, outTexture, mainTex.name, "_shadow_1st");
                if(shadowType == 2) outTexture = SaveTextureToPng(material, outTexture, mainTex.name, "_shadow_2nd");
                if(outTexture != mainTex.textureValue)
                {
                    CopyTextureSetting(bufMainTexture, outTexture);
                }

                UnityEngine.Object.DestroyImmediate(hsvgMaterial);
                UnityEngine.Object.DestroyImmediate(srcTexture);
                UnityEngine.Object.DestroyImmediate(srcMain2);
                UnityEngine.Object.DestroyImmediate(srcMask2);

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
                hsvgMaterial.SetVector(mainTexHSVG.name,        defaultHSVG);

                path = AssetDatabase.GetAssetPath(material.GetTexture(matcapTex.name));
                if(!string.IsNullOrEmpty(path))
                {
                    LoadTexture(ref srcTexture, path);
                    hsvgMaterial.SetTexture(mainTex.name, srcTexture);
                }
                else
                {
                    hsvgMaterial.SetTexture(mainTex.name, Texture2D.whiteTexture);
                }

                Texture2D outTexture = null;
                RunBake(ref outTexture, srcTexture, hsvgMaterial);

                outTexture = SaveTextureToPng(material, outTexture, matcapTex.name);
                if(outTexture != matcapTex.textureValue)
                {
                    CopyTextureSetting(bufMainTexture, outTexture);
                }

                UnityEngine.Object.DestroyImmediate(hsvgMaterial);
                UnityEngine.Object.DestroyImmediate(srcTexture);

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
                    LoadTexture(ref srcTexture, path);
                    hsvgMaterial.SetTexture(mainTex.name, srcTexture);
                }
                else
                {
                    hsvgMaterial.SetTexture(mainTex.name, Texture2D.whiteTexture);
                }

                path = AssetDatabase.GetAssetPath(rimColorTex.textureValue);
                if(!string.IsNullOrEmpty(path))
                {
                    LoadTexture(ref srcMain2, path);
                    hsvgMaterial.SetTexture(main2ndTex.name, srcMain2);
                }
                else
                {
                    hsvgMaterial.SetTexture(main2ndTex.name, Texture2D.whiteTexture);
                }

                path = AssetDatabase.GetAssetPath(emissionBlendMask.textureValue);
                if(!string.IsNullOrEmpty(path))
                {
                    LoadTexture(ref srcMain3, path);
                    hsvgMaterial.SetTexture(main3rdTex.name, srcMain3);
                }
                else
                {
                    hsvgMaterial.SetTexture(main3rdTex.name, Texture2D.whiteTexture);
                }

                Texture2D outTexture = null;
                RunBake(ref outTexture, srcTexture, hsvgMaterial, bufMainTexture);

                outTexture = SaveTextureToPng(material, outTexture, mainTex.name);
                if(outTexture != mainTex.textureValue && mainTex.textureValue != null)
                {
                    CopyTextureSetting(bufMainTexture, outTexture);
                }

                UnityEngine.Object.DestroyImmediate(hsvgMaterial);
                UnityEngine.Object.DestroyImmediate(srcTexture);

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
            hsvgMaterial.SetVector(mainTexHSVG.name,        defaultHSVG);
            hsvgMaterial.SetFloat(alphaMaskMode.name,       alphaMaskMode.floatValue);
            hsvgMaterial.SetFloat(alphaMaskScale.name,      alphaMaskScale.floatValue);
            hsvgMaterial.SetFloat(alphaMaskValue.name,      alphaMaskValue.floatValue);

            path = AssetDatabase.GetAssetPath(bufMainTexture);
            if(!string.IsNullOrEmpty(path))
            {
                LoadTexture(ref srcTexture, path);
                hsvgMaterial.SetTexture(mainTex.name, srcTexture);
            }
            else
            {
                hsvgMaterial.SetTexture(mainTex.name, Texture2D.whiteTexture);
            }

            path = AssetDatabase.GetAssetPath(material.GetTexture(alphaMask.name));
            if(!string.IsNullOrEmpty(path))
            {
                LoadTexture(ref srcAlphaMask, path);
                hsvgMaterial.SetTexture(alphaMask.name, srcAlphaMask);
            }
            else
            {
                return (Texture2D)mainTex.textureValue;
            }

            Texture2D outTexture = null;
            RunBake(ref outTexture, srcTexture, hsvgMaterial);

            outTexture = SaveTextureToPng(outTexture, bufMainTexture);
            if(outTexture != bufMainTexture)
            {
                CopyTextureSetting(bufMainTexture, outTexture);
                string savePath = AssetDatabase.GetAssetPath(outTexture);
                TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(savePath);
                textureImporter.alphaIsTransparency = true;
                AssetDatabase.ImportAsset(savePath);
            }

            UnityEngine.Object.DestroyImmediate(hsvgMaterial);
            UnityEngine.Object.DestroyImmediate(srcTexture);

            return outTexture;
        }

        private Texture2D AutoBakeOutlineTexture(Material material)
        {
            bool shouldNotBakeOutline = outlineTex.textureValue == null || outlineTexHSVG.vectorValue == defaultHSVG;
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
                    LoadTexture(ref srcTexture, path);
                    hsvgMaterial.SetTexture(mainTex.name, srcTexture);
                }
                else
                {
                    hsvgMaterial.SetTexture(mainTex.name, Texture2D.whiteTexture);
                }

                Texture2D outTexture = null;
                RunBake(ref outTexture, srcTexture, hsvgMaterial);

                outTexture = SaveTextureToPng(material, outTexture, mainTex.name);
                if(outTexture != mainTex.textureValue)
                {
                    CopyTextureSetting(bufMainTexture, outTexture);
                }

                UnityEngine.Object.DestroyImmediate(hsvgMaterial);
                UnityEngine.Object.DestroyImmediate(srcTexture);

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
                AutoBakeShadowTexture(material, (Texture2D)mainTex.textureValue, shadowType);
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
                LoadTexture(ref srcTexture, path);
                hsvgMaterial.SetTexture(mainTex.name, srcTexture);
            }
            else
            {
                hsvgMaterial.SetTexture(mainTex.name, Texture2D.whiteTexture);
            }

            Texture2D outTexture = null;
            RunBake(ref outTexture, srcTexture, hsvgMaterial);

            if(!string.IsNullOrEmpty(path)) path = Path.GetDirectoryName(path) + "/Baked_" + material.name + "_" + propName;
            else                            path = "Assets/Baked_" + material.name + "_" + propName;
            outTexture = SaveTextureToPng(outTexture, path);
            if(outTexture != bufMainTexture)
            {
                CopyTextureSetting(bufMainTexture, outTexture);
            }

            UnityEngine.Object.DestroyImmediate(hsvgMaterial);
            UnityEngine.Object.DestroyImmediate(srcTexture);
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
        // Gradient
        #region
        private void SetLinear(Texture2D tex)
        {
            if(tex != null)
            {
                string path = AssetDatabase.GetAssetPath(tex);
                TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(path);
                textureImporter.sRGBTexture = false;
                AssetDatabase.ImportAsset(path);
            }
        }

        private void GradientEditor(Material material, Gradient ingrad, MaterialProperty texprop, bool setLinear = false)
        {
            #if UNITY_2018_3_OR_NEWER
                ingrad = EditorGUILayout.GradientField(GetLoc("sGradColor"), ingrad);
            #else
                MethodInfo setMethod = typeof(EditorGUILayout).GetMethod(
                    "GradientField",
                    BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    new [] {typeof(string), typeof(Gradient), typeof(GUILayoutOption[])},
                    null);
                if(setMethod != null) {
                    ingrad = (Gradient)setMethod.Invoke(null, new object[]{GetLoc("sGradColor"), ingrad, null});;
                }
            #endif
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Test"))
            {
                texprop.textureValue = GradientToTexture(ingrad, setLinear);
            }
            if(GUILayout.Button("Save"))
            {
                Texture2D tex = GradientToTexture(ingrad, setLinear);
                tex = SaveTextureToPng(material, tex, texprop.name);
                if(setLinear) SetLinear(tex);
                texprop.textureValue = tex;
            }
            GUILayout.EndHorizontal();
        }

        private void GradientEditor(Material material, string emissionName, Gradient ingrad, MaterialProperty texprop, bool setLinear = false)
        {
            ingrad = MaterialToGradient(material, emissionName);
            #if UNITY_2018_3_OR_NEWER
                ingrad = EditorGUILayout.GradientField(GetLoc("sGradColor"), ingrad);
            #else
                MethodInfo setMethod = typeof(EditorGUILayout).GetMethod(
                    "GradientField",
                    BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    new [] {typeof(string), typeof(Gradient), typeof(GUILayoutOption[])},
                    null);
                if(setMethod != null) {
                    ingrad = (Gradient)setMethod.Invoke(null, new object[]{GetLoc("sGradColor"), ingrad, null});;
                }
            #endif
            GradientToMaterial(material, emissionName, ingrad);
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Test"))
            {
                texprop.textureValue = GradientToTexture(ingrad, setLinear);
            }
            if(GUILayout.Button("Save"))
            {
                Texture2D tex = GradientToTexture(ingrad, setLinear);
                tex = SaveTextureToPng(material, tex, texprop.name);
                if(setLinear) SetLinear(tex);
                texprop.textureValue = tex;
            }
            GUILayout.EndHorizontal();
        }

        private Gradient MaterialToGradient(Material material, string emissionName)
        {
            Gradient outgrad = new Gradient();
            GradientColorKey[] colorKey = new GradientColorKey[material.GetInt(emissionName + "ci")];
            GradientAlphaKey[] alphaKey = new GradientAlphaKey[material.GetInt(emissionName + "ai")];
            for(int i=0;i<colorKey.Length;i++)
            {
                colorKey[i].color = material.GetColor(emissionName + "c" + i.ToString());
                colorKey[i].time = material.GetColor(emissionName + "c" + i.ToString()).a;
            }
            for(int j=0;j<alphaKey.Length;j++)
            {
                alphaKey[j].alpha = material.GetColor(emissionName + "a" + j.ToString()).r;
                alphaKey[j].time = material.GetColor(emissionName + "a" + j.ToString()).a;
            }
            outgrad.SetKeys(colorKey, alphaKey);
            return outgrad;
        }

        private void GradientToMaterial(Material material, string emissionName, Gradient ingrad)
        {
            material.SetInt(emissionName + "ci", ingrad.colorKeys.Length);
            material.SetInt(emissionName + "ai", ingrad.alphaKeys.Length);
            for(int ic=0;ic<ingrad.colorKeys.Length;ic++)
            {
                material.SetColor(emissionName + "c" + ic.ToString(), new Color(ingrad.colorKeys[ic].color.r, ingrad.colorKeys[ic].color.g, ingrad.colorKeys[ic].color.b, ingrad.colorKeys[ic].time));
            }
            for(int ia=0;ia<ingrad.alphaKeys.Length;ia++)
            {
                material.SetColor(emissionName + "a" + ia.ToString(), new Color(ingrad.alphaKeys[ia].alpha, 0, 0, ingrad.alphaKeys[ia].time));
            }
        }

        private Texture2D GradientToTexture(Gradient grad, bool setLinear = false)
        {
            Texture2D tex = new Texture2D(128, 4, TextureFormat.ARGB32, true, setLinear);

            // Set colors
            for(int w = 0; w < tex.width; w++)
            {
                for(int h = 0; h < tex.height; h++)
                {
                    tex.SetPixel(w, h, grad.Evaluate((float)w / tex.width));
                }
            }

            tex.Apply();
            return tex;
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Load Texture
        #region
        private static void GetReadableTexture(ref Texture2D tex)
        {
            if(tex == null) return;

            #if UNITY_2018_3_OR_NEWER
            if(!tex.isReadable)
            #endif
            {
                RenderTexture bufRT = RenderTexture.active;
                RenderTexture texR = RenderTexture.GetTemporary(tex.width, tex.height);
                Graphics.Blit(tex, texR);
                RenderTexture.active = texR;
                tex = new Texture2D(texR.width, texR.height);
                tex.ReadPixels(new Rect(0, 0, texR.width, texR.height), 0, 0);
                tex.Apply();
                RenderTexture.active = bufRT;
                RenderTexture.ReleaseTemporary(texR);
            }
        }

        public static void LoadTexture(ref Texture2D tex, string path)
        {
            if(string.IsNullOrEmpty(path)) return;

            tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            GetReadableTexture(ref tex);
            if(path.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                byte[] bytes = File.ReadAllBytes(Path.GetFullPath(path));
                tex.LoadImage(bytes);
            }

            if(tex != null) tex.filterMode = FilterMode.Bilinear;
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Save Texture
        #region
        private Texture2D SaveTextureToPng(Material material, Texture2D tex, string texname, string customName = "")
        {
            string path = AssetDatabase.GetAssetPath(material.GetTexture(texname));
            if(string.IsNullOrEmpty(path)) path = AssetDatabase.GetAssetPath(material);

            string filename = Path.GetFileNameWithoutExtension(path);
            if(!string.IsNullOrEmpty(customName)) filename += customName;
            else                                  filename += "_2";
            if(!string.IsNullOrEmpty(path))  path = EditorUtility.SaveFilePanel("Save Texture", Path.GetDirectoryName(path), filename, "png");
            else                 path = EditorUtility.SaveFilePanel("Save Texture", "Assets", tex.name + ".png", "png");
            if(!string.IsNullOrEmpty(path)) {
                File.WriteAllBytes(path, tex.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(tex);
                AssetDatabase.Refresh();
                return AssetDatabase.LoadAssetAtPath<Texture2D>(path.Substring(path.IndexOf("Assets")));
            }
            else
            {
                return (Texture2D)material.GetTexture(texname);
            }
        }

        private Texture2D SaveTextureToPng(Texture2D tex, Texture2D origTex)
        {
            string path = AssetDatabase.GetAssetPath(origTex);
            if(!string.IsNullOrEmpty(path))  path = EditorUtility.SaveFilePanel("Save Texture", Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)+"_alpha", "png");
            else                 path = EditorUtility.SaveFilePanel("Save Texture", "Assets", tex.name + "_alpha.png", "png");
            if(!string.IsNullOrEmpty(path)) {
                File.WriteAllBytes(path, tex.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(tex);
                AssetDatabase.Refresh();
                return AssetDatabase.LoadAssetAtPath<Texture2D>(path.Substring(path.IndexOf("Assets")));
            }
            else
            {
                return origTex;
            }
        }

        private Texture2D SaveTextureToPng(Texture2D tex, string path, string customName = "")
        {
            string filename = customName + Path.GetFileNameWithoutExtension(path);
            if(!string.IsNullOrEmpty(path)) path = EditorUtility.SaveFilePanel("Save Texture", Path.GetDirectoryName(path), filename, "png");
            else                            path = EditorUtility.SaveFilePanel("Save Texture", "Assets", filename, "png");
            if(!string.IsNullOrEmpty(path))
            {
                File.WriteAllBytes(path, tex.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(tex);
                AssetDatabase.Refresh();
                return AssetDatabase.LoadAssetAtPath<Texture2D>(path.Substring(path.IndexOf("Assets")));
            }
            else
            {
                return tex;
            }
        }

        public static string SavePng(string path, string add, Texture2D tex)
        {
            string savePath = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + add + ".png";
            File.WriteAllBytes(savePath, tex.EncodeToPNG());
            return savePath;
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Save Preset Window
        #region
        public class lilPresetWindow : EditorWindow
        {
            private Vector2 scrollPosition = Vector2.zero;

            private bool shouldSaveRenderingMode = false;
            private bool shouldSaveQueue = false;
            private bool shouldSaveMainTex2Outline = false;

            // Feature
            private bool shouldSaveBase = true;
            private bool shouldSaveLighting = true;
            private bool shouldSaveUV = true;
            private bool shouldSaveMain = true;
            private bool shouldSaveMain2nd = true;
            private bool shouldSaveMain3rd = true;
            private bool shouldSaveAlphaMask = true;
            private bool shouldSaveShadow = true;
            private bool shouldSaveEmission = true;
            private bool shouldSaveEmission2nd = true;
            private bool shouldSaveNormalMap = true;
            private bool shouldSaveNormalMap2nd = true;
            private bool shouldSaveAnisotropy = true;
            private bool shouldSaveBacklight = true;
            private bool shouldSaveReflection = true;
            private bool shouldSaveMatCap = true;
            private bool shouldSaveMatCap2nd = true;
            private bool shouldSaveRim = true;
            private bool shouldSaveGlitter = true;
            private bool shouldSaveParallax = true;
            private bool shouldSaveDistanceFade = true;
            private bool shouldSaveAudioLink = true;
            private bool shouldSaveDissolve = true;
            private bool shouldSaveRefraction = true;
            private bool shouldSaveGem = true;
            private bool shouldSaveTessellation = true;
            private bool shouldSaveOutline = true;
            private bool shouldSaveFur = true;
            private bool shouldSaveStencil = true;
            private bool shouldSaveRendering = true;
            private bool shouldSaveOutlineRendering = true;
            private bool shouldSaveFurRendering = true;

            // Texture
            private bool shouldSave_TriMask = false;
            private bool shouldSave_MainTex = false;
            private bool shouldSave_MainGradationTex = false;
            private bool shouldSave_MainColorAdjustMask = false;
            private bool shouldSave_Main2ndTex = false;
            private bool shouldSave_Main2ndBlendMask = false;
            private bool shouldSave_Main2ndDissolveMask = false;
            private bool shouldSave_Main2ndDissolveNoiseMask = false;
            private bool shouldSave_Main3rdTex = false;
            private bool shouldSave_Main3rdBlendMask = false;
            private bool shouldSave_Main3rdDissolveMask = false;
            private bool shouldSave_Main3rdDissolveNoiseMask = false;
            private bool shouldSave_AlphaMask = false;
            private bool shouldSave_ShadowStrengthMask = false;
            private bool shouldSave_ShadowBorderMask = false;
            private bool shouldSave_ShadowBlurMask = false;
            private bool shouldSave_ShadowColorTex = false;
            private bool shouldSave_Shadow2ndColorTex = false;
            private bool shouldSave_Shadow3rdColorTex = false;
            private bool shouldSave_EmissionMap = false;
            private bool shouldSave_EmissionBlendMask = false;
            private bool shouldSave_EmissionGradTex = false;
            private bool shouldSave_Emission2ndMap = false;
            private bool shouldSave_Emission2ndBlendMask = false;
            private bool shouldSave_Emission2ndGradTex = false;
            private bool shouldSave_BumpMap = false;
            private bool shouldSave_Bump2ndMap = false;
            private bool shouldSave_Bump2ndScaleMask = false;
            private bool shouldSave_AnisotropyTangentMap = false;
            private bool shouldSave_AnisotropyScaleMask = false;
            private bool shouldSave_AnisotropyShiftNoiseMask = false;
            private bool shouldSave_BacklightColorTex = false;
            private bool shouldSave_SmoothnessTex = false;
            private bool shouldSave_MetallicGlossMap = false;
            private bool shouldSave_ReflectionColorTex = false;
            private bool shouldSave_MatCapTex = false;
            private bool shouldSave_MatCapBlendMask = false;
            private bool shouldSave_MatCapBumpMap = false;
            private bool shouldSave_MatCap2ndTex = false;
            private bool shouldSave_MatCap2ndBlendMask = false;
            private bool shouldSave_MatCap2ndBumpMap = false;
            private bool shouldSave_RimColorTex = false;
            private bool shouldSave_GlitterColorTex = false;
            private bool shouldSave_ParallaxMap = false;
            private bool shouldSave_AudioLinkMask = false;
            private bool shouldSave_AudioLinkLocalMap = false;
            private bool shouldSave_DissolveMask = false;
            private bool shouldSave_DissolveNoiseMask = false;
            private bool shouldSave_OutlineTex = false;
            private bool shouldSave_OutlineWidthMask = false;
            private bool shouldSave_OutlineVectorTex = false;
            private bool shouldSave_FurNoiseMask = false;
            private bool shouldSave_FurMask = false;
            private bool shouldSave_FurLengthMask = false;
            private bool shouldSave_FurVectorTex = false;

            private lilToonPreset preset;
            private string[] presetName;
            private string filename = "";
            private RenderingMode renderingMode;
            private bool isLite        = false;
            private bool isOutl        = false;
            private bool isTess        = false;
            private bool isFakeShadow  = false;
            private bool isOnePass     = false;
            private bool isTwoPass     = false;
            private bool isMulti       = false;
            private bool isShowFeatures = false;
            private bool isShowTextures = false;

            private void OnGUI()
            {
                if(!(Selection.activeObject is Material)){
                    EditorGUILayout.LabelField(GetLoc("sPresetIsMaterial"));
                    return;
                }

                string[] sCategorys = { GetLoc("sPresetCategorySkin"),
                                        GetLoc("sPresetCategoryHair"),
                                        GetLoc("sPresetCategoryCloth"),
                                        GetLoc("sPresetCategoryNature"),
                                        GetLoc("sPresetCategoryInorganic"),
                                        GetLoc("sPresetCategoryEffect"),
                                        GetLoc("sPresetCategoryOther") };
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                Material material = (Material)Selection.activeObject;
                if(preset == null) preset = CreateInstance<lilToonPreset>();

                // load language
                string[] langName = edSet.languageNames.Split('\t');
                Array.Resize(ref presetName, langName.Length);

                // Initialize
                Array.Resize(ref preset.bases, 0);
                Array.Resize(ref preset.colors, 0);
                Array.Resize(ref preset.vectors, 0);
                Array.Resize(ref preset.floats, 0);
                Array.Resize(ref preset.textures, 0);
                if(material.shader != null && !string.IsNullOrEmpty(material.shader.name))
                {
                    isLite        = material.shader.name.Contains("Lite");
                    isOutl        = material.shader.name.Contains("Outline");
                    isTess        = material.shader.name.Contains("Tessellation");
                    isFakeShadow  = material.shader.name.Contains("FakeShadow");
                    isOnePass     = material.shader.name.Contains("OnePass");
                    isTwoPass     = material.shader.name.Contains("TwoPass");
                    isMulti       = material.shader.name.Contains("Multi");
                    renderingMode = RenderingMode.Opaque;
                    if(material.shader.name.Contains("Cutout"))         renderingMode = RenderingMode.Cutout;
                    if(material.shader.name.Contains("Transparent"))    renderingMode = RenderingMode.Transparent;
                    if(material.shader.name.Contains("Refraction"))     renderingMode = RenderingMode.Refraction;
                    if(material.shader.name.Contains("RefractionBlur")) renderingMode = RenderingMode.RefractionBlur;
                    if(material.shader.name.Contains("Fur"))            renderingMode = RenderingMode.Fur;
                    if(material.shader.name.Contains("FurCutout"))      renderingMode = RenderingMode.FurCutout;
                    if(material.shader.name.Contains("FurTwoPass"))     renderingMode = RenderingMode.FurTwoPass;
                    if(material.shader.name.Contains("Gem"))            renderingMode = RenderingMode.Gem;
                }
                else
                {
                    isLite        = false;
                    isOutl        = false;
                    isTess        = false;
                    isFakeShadow  = false;
                    isOnePass     = false;
                    isTwoPass     = false;
                    isMulti       = false;
                    renderingMode = RenderingMode.Opaque;
                }

                // Name
                EditorGUILayout.LabelField(GetLoc("sPresetName"));
                for(int i = 0; i < langName.Length; i++)
                {
                    presetName[i] = EditorGUILayout.TextField(langName[i], presetName[i]);
                }

                preset.category = (lilPresetCategory)EditorGUILayout.Popup(GetLoc("sPresetCategory"), (int)preset.category, sCategorys);

                // Features
                EditorGUILayout.Space();
                DrawSimpleFoldout(GetLoc("sPresetSaveTarget"), ref isShowFeatures);
                if(isShowFeatures)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    shouldSaveRenderingMode             = EditorGUILayout.ToggleLeft(GetLoc("sRenderingMode"), shouldSaveRenderingMode);
                    shouldSaveQueue                     = EditorGUILayout.ToggleLeft("Render Queue", shouldSaveQueue);
                    shouldSaveMainTex2Outline           = EditorGUILayout.ToggleLeft(GetLoc("sPresetMainTex2Outline"), shouldSaveMainTex2Outline);

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    if(GUILayout.Button("Select All")) ToggleAllFeatures(true);
                    if(GUILayout.Button("Deselect All")) ToggleAllFeatures(false);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField(GetLoc("sBaseSetting"), EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    shouldSaveBase                      = EditorGUILayout.ToggleLeft(GetLoc("sBaseSetting"), shouldSaveBase);
                    shouldSaveLighting                  = EditorGUILayout.ToggleLeft(GetLoc("sLightingSettings"), shouldSaveLighting);
                    shouldSaveUV                        = EditorGUILayout.ToggleLeft(GetLoc("sMainUV"), shouldSaveUV);
                    EditorGUI.indentLevel--;

                    EditorGUILayout.LabelField(GetLoc("sColors"), EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    shouldSaveMain                      = EditorGUILayout.ToggleLeft(GetLoc("sMainColor"), shouldSaveMain);
                    shouldSaveMain2nd                   = EditorGUILayout.ToggleLeft(GetLoc("sMainColor2nd"), shouldSaveMain2nd);
                    shouldSaveMain3rd                   = EditorGUILayout.ToggleLeft(GetLoc("sMainColor3rd"), shouldSaveMain3rd);
                    shouldSaveAlphaMask                 = EditorGUILayout.ToggleLeft(GetLoc("sAlphaMask"), shouldSaveAlphaMask);
                    shouldSaveShadow                    = EditorGUILayout.ToggleLeft(GetLoc("sShadow"), shouldSaveShadow);
                    shouldSaveEmission                  = EditorGUILayout.ToggleLeft(GetLoc("sEmission"), shouldSaveEmission);
                    shouldSaveEmission2nd               = EditorGUILayout.ToggleLeft(GetLoc("sEmission2nd"), shouldSaveEmission2nd);
                    EditorGUI.indentLevel--;

                    EditorGUILayout.LabelField(GetLoc("sNormalMapReflection"), EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    shouldSaveNormalMap                 = EditorGUILayout.ToggleLeft(GetLoc("sNormalMap"), shouldSaveNormalMap);
                    shouldSaveNormalMap2nd              = EditorGUILayout.ToggleLeft(GetLoc("sNormalMap2nd"), shouldSaveNormalMap2nd);
                    shouldSaveAnisotropy                = EditorGUILayout.ToggleLeft(GetLoc("sAnisotropy"), shouldSaveAnisotropy);
                    shouldSaveBacklight                 = EditorGUILayout.ToggleLeft(GetLoc("sBacklight"), shouldSaveBacklight);
                    shouldSaveReflection                = EditorGUILayout.ToggleLeft(GetLoc("sReflection"), shouldSaveReflection);
                    shouldSaveMatCap                    = EditorGUILayout.ToggleLeft(GetLoc("sMatCap"), shouldSaveMatCap);
                    shouldSaveMatCap2nd                 = EditorGUILayout.ToggleLeft(GetLoc("sMatCap2nd"), shouldSaveMatCap2nd);
                    shouldSaveRim                       = EditorGUILayout.ToggleLeft(GetLoc("sRimLight"), shouldSaveRim);
                    shouldSaveGlitter                   = EditorGUILayout.ToggleLeft(GetLoc("sGlitter"), shouldSaveGlitter);
                    shouldSaveGem                       = EditorGUILayout.ToggleLeft(GetLoc("sGem"), shouldSaveGem);
                    EditorGUI.indentLevel--;

                    EditorGUILayout.LabelField(GetLoc("sAdvanced"), EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    shouldSaveParallax                  = EditorGUILayout.ToggleLeft(GetLoc("sParallax"), shouldSaveParallax);
                    shouldSaveDistanceFade              = EditorGUILayout.ToggleLeft(GetLoc("sDistanceFade"), shouldSaveDistanceFade);
                    shouldSaveAudioLink                 = EditorGUILayout.ToggleLeft(GetLoc("sAudioLink"), shouldSaveAudioLink);
                    shouldSaveDissolve                  = EditorGUILayout.ToggleLeft(GetLoc("sDissolve"), shouldSaveDissolve);
                    shouldSaveRefraction                = EditorGUILayout.ToggleLeft(GetLoc("sRefraction"), shouldSaveRefraction);
                    shouldSaveTessellation              = EditorGUILayout.ToggleLeft(GetLoc("sTessellation"), shouldSaveTessellation);
                    shouldSaveOutline                   = EditorGUILayout.ToggleLeft(GetLoc("sOutline"), shouldSaveOutline);
                    if(
                        renderingMode == RenderingMode.Fur ||
                        renderingMode == RenderingMode.FurCutout ||
                        renderingMode == RenderingMode.FurTwoPass
                    )
                    {
                        shouldSaveFur                       = EditorGUILayout.ToggleLeft(GetLoc("sFur"), shouldSaveFur);
                        shouldSaveFurRendering              = EditorGUILayout.ToggleLeft(GetLoc("sRenderingSetting") + " - " + GetLoc("sFur"), shouldSaveFurRendering);
                    }
                    else
                    {
                        shouldSaveFur                       = false;
                        shouldSaveFurRendering              = false;
                    }
                    shouldSaveStencil                   = EditorGUILayout.ToggleLeft(GetLoc("sStencilSetting"), shouldSaveStencil);
                    shouldSaveRendering                 = EditorGUILayout.ToggleLeft(GetLoc("sRenderingSetting"), shouldSaveRendering);
                    shouldSaveOutlineRendering          = EditorGUILayout.ToggleLeft(GetLoc("sOutline") + " - " + GetLoc("sRenderingSetting"), shouldSaveOutlineRendering);
                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndVertical();
                }

                // Textures
                EditorGUILayout.Space();
                DrawSimpleFoldout(GetLoc("sPresetTexture"), ref isShowTextures);
                if(isShowTextures)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.BeginHorizontal();
                    if(GUILayout.Button("Select All")) ToggleAllTextures(true);
                    if(GUILayout.Button("Deselect All")) ToggleAllTextures(false);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField(GetLoc("sBaseSetting"), EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    shouldSave_TriMask                  = EditorGUILayout.ToggleLeft("_TriMask", shouldSave_TriMask);
                    EditorGUI.indentLevel--;

                    EditorGUILayout.LabelField(GetLoc("sColors"), EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    shouldSave_MainTex                  = EditorGUILayout.ToggleLeft("_MainTex", shouldSave_MainTex);
                    shouldSave_MainGradationTex         = EditorGUILayout.ToggleLeft("_MainGradationTex", shouldSave_MainGradationTex);
                    shouldSave_MainColorAdjustMask      = EditorGUILayout.ToggleLeft("_MainColorAdjustMask", shouldSave_MainColorAdjustMask);
                    shouldSave_Main2ndTex               = EditorGUILayout.ToggleLeft("_Main2ndTex", shouldSave_Main2ndTex);
                    shouldSave_Main2ndBlendMask         = EditorGUILayout.ToggleLeft("_Main2ndBlendMask", shouldSave_Main2ndBlendMask);
                    shouldSave_Main2ndDissolveMask      = EditorGUILayout.ToggleLeft("_Main2ndDissolveMask", shouldSave_Main2ndDissolveMask);
                    shouldSave_Main2ndDissolveNoiseMask = EditorGUILayout.ToggleLeft("_Main2ndDissolveNoiseMask", shouldSave_Main2ndDissolveNoiseMask);
                    shouldSave_Main3rdTex               = EditorGUILayout.ToggleLeft("_Main3rdTex", shouldSave_Main3rdTex);
                    shouldSave_Main3rdBlendMask         = EditorGUILayout.ToggleLeft("_Main3rdBlendMask", shouldSave_Main3rdBlendMask);
                    shouldSave_Main3rdDissolveMask      = EditorGUILayout.ToggleLeft("_Main3rdDissolveMask", shouldSave_Main3rdDissolveMask);
                    shouldSave_Main3rdDissolveNoiseMask = EditorGUILayout.ToggleLeft("_Main3rdDissolveNoiseMask", shouldSave_Main3rdDissolveNoiseMask);
                    shouldSave_AlphaMask                = EditorGUILayout.ToggleLeft("_AlphaMask", shouldSave_AlphaMask);
                    shouldSave_ShadowStrengthMask       = EditorGUILayout.ToggleLeft("_ShadowStrengthMask", shouldSave_ShadowStrengthMask);
                    shouldSave_ShadowBorderMask         = EditorGUILayout.ToggleLeft("_ShadowBorderMask", shouldSave_ShadowBorderMask);
                    shouldSave_ShadowBlurMask           = EditorGUILayout.ToggleLeft("_ShadowBlurMask", shouldSave_ShadowBlurMask);
                    shouldSave_ShadowColorTex           = EditorGUILayout.ToggleLeft("_ShadowColorTex", shouldSave_ShadowColorTex);
                    shouldSave_Shadow2ndColorTex        = EditorGUILayout.ToggleLeft("_Shadow2ndColorTex", shouldSave_Shadow2ndColorTex);
                    shouldSave_Shadow3rdColorTex        = EditorGUILayout.ToggleLeft("_Shadow3rdColorTex", shouldSave_Shadow3rdColorTex);
                    shouldSave_EmissionMap              = EditorGUILayout.ToggleLeft("_EmissionMap", shouldSave_EmissionMap);
                    shouldSave_EmissionBlendMask        = EditorGUILayout.ToggleLeft("_EmissionBlendMask", shouldSave_EmissionBlendMask);
                    shouldSave_EmissionGradTex          = EditorGUILayout.ToggleLeft("_EmissionGradTex", shouldSave_EmissionGradTex);
                    shouldSave_Emission2ndMap           = EditorGUILayout.ToggleLeft("_Emission2ndMap", shouldSave_Emission2ndMap);
                    shouldSave_Emission2ndBlendMask     = EditorGUILayout.ToggleLeft("_Emission2ndBlendMask", shouldSave_Emission2ndBlendMask);
                    shouldSave_Emission2ndGradTex       = EditorGUILayout.ToggleLeft("_Emission2ndGradTex", shouldSave_Emission2ndGradTex);
                    EditorGUI.indentLevel--;

                    EditorGUILayout.LabelField(GetLoc("sNormalMapReflection"), EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    shouldSave_BumpMap                  = EditorGUILayout.ToggleLeft("_BumpMap", shouldSave_BumpMap);
                    shouldSave_Bump2ndMap               = EditorGUILayout.ToggleLeft("_Bump2ndMap", shouldSave_Bump2ndMap);
                    shouldSave_Bump2ndScaleMask         = EditorGUILayout.ToggleLeft("_Bump2ndScaleMask", shouldSave_Bump2ndScaleMask);
                    shouldSave_AnisotropyTangentMap     = EditorGUILayout.ToggleLeft("_AnisotropyTangentMap", shouldSave_AnisotropyTangentMap);
                    shouldSave_AnisotropyScaleMask      = EditorGUILayout.ToggleLeft("_AnisotropyScaleMask", shouldSave_AnisotropyScaleMask);
                    shouldSave_AnisotropyShiftNoiseMask = EditorGUILayout.ToggleLeft("_AnisotropyShiftNoiseMask", shouldSave_AnisotropyShiftNoiseMask);
                    shouldSave_BacklightColorTex        = EditorGUILayout.ToggleLeft("_BacklightColorTex", shouldSave_BacklightColorTex);
                    shouldSave_SmoothnessTex            = EditorGUILayout.ToggleLeft("_SmoothnessTex", shouldSave_SmoothnessTex);
                    shouldSave_MetallicGlossMap         = EditorGUILayout.ToggleLeft("_MetallicGlossMap", shouldSave_MetallicGlossMap);
                    shouldSave_ReflectionColorTex       = EditorGUILayout.ToggleLeft("_ReflectionColorTex", shouldSave_ReflectionColorTex);
                    shouldSave_MatCapTex                = EditorGUILayout.ToggleLeft("_MatCapTex", shouldSave_MatCapTex);
                    shouldSave_MatCapBlendMask          = EditorGUILayout.ToggleLeft("_MatCapBlendMask", shouldSave_MatCapBlendMask);
                    shouldSave_MatCapBumpMap            = EditorGUILayout.ToggleLeft("_MatCapBumpMap", shouldSave_MatCapBumpMap);
                    shouldSave_MatCap2ndTex             = EditorGUILayout.ToggleLeft("_MatCap2ndTex", shouldSave_MatCap2ndTex);
                    shouldSave_MatCap2ndBlendMask       = EditorGUILayout.ToggleLeft("_MatCap2ndBlendMask", shouldSave_MatCap2ndBlendMask);
                    shouldSave_MatCap2ndBumpMap         = EditorGUILayout.ToggleLeft("_MatCap2ndBumpMap", shouldSave_MatCap2ndBumpMap);
                    shouldSave_RimColorTex              = EditorGUILayout.ToggleLeft("_RimColorTex", shouldSave_RimColorTex);
                    shouldSave_GlitterColorTex          = EditorGUILayout.ToggleLeft("_GlitterColorTex", shouldSave_GlitterColorTex);
                    EditorGUI.indentLevel--;

                    EditorGUILayout.LabelField(GetLoc("sAdvanced"), EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    shouldSave_ParallaxMap              = EditorGUILayout.ToggleLeft("_ParallaxMap", shouldSave_ParallaxMap);
                    shouldSave_AudioLinkMask            = EditorGUILayout.ToggleLeft("_AudioLinkMask", shouldSave_AudioLinkMask);
                    shouldSave_AudioLinkLocalMap        = EditorGUILayout.ToggleLeft("_AudioLinkLocalMap", shouldSave_AudioLinkLocalMap);
                    shouldSave_DissolveMask             = EditorGUILayout.ToggleLeft("_DissolveMask", shouldSave_DissolveMask);
                    shouldSave_DissolveNoiseMask        = EditorGUILayout.ToggleLeft("_DissolveNoiseMask", shouldSave_DissolveNoiseMask);
                    shouldSave_OutlineTex               = EditorGUILayout.ToggleLeft("_OutlineTex", shouldSave_OutlineTex);
                    shouldSave_OutlineWidthMask         = EditorGUILayout.ToggleLeft("_OutlineWidthMask", shouldSave_OutlineWidthMask);
                    shouldSave_OutlineVectorTex         = EditorGUILayout.ToggleLeft("_OutlineVectorTex", shouldSave_OutlineVectorTex);
                    shouldSave_FurNoiseMask             = EditorGUILayout.ToggleLeft("_FurNoiseMask", shouldSave_FurNoiseMask);
                    shouldSave_FurMask                  = EditorGUILayout.ToggleLeft("_FurMask", shouldSave_FurMask);
                    shouldSave_FurLengthMask            = EditorGUILayout.ToggleLeft("_FurLengthMask", shouldSave_FurLengthMask);
                    shouldSave_FurVectorTex             = EditorGUILayout.ToggleLeft("_FurVectorTex", shouldSave_FurVectorTex);
                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.Space();
                if(GUILayout.Button("Save"))
                {
                    // Preset Name
                    for(int i = 0; i < langName.Length; i++)
                    {
                        if(!string.IsNullOrEmpty(presetName[i]))
                        {
                            Array.Resize(ref preset.bases, preset.bases.Length+1);
                            preset.bases[preset.bases.Length-1].language = langName[i];
                            preset.bases[preset.bases.Length-1].name = presetName[i];
                            if(string.IsNullOrEmpty(filename) || langName[i] == "English") filename = preset.category.ToString() + "-" + presetName[i];
                        }
                    }

                    // Copy properties
                    CopyPropertiesToPreset(material);
                    CopyTexturesToPreset(material);
                    preset.renderingMode = shouldSaveRenderingMode ? renderingMode.ToString() : "";
                    preset.shader = null;
                    preset.renderQueue = shouldSaveQueue ? material.renderQueue : -2;
                    preset.outline = shouldSaveOutline ? (isOutl?1:0) : -1;
                    preset.tessellation = shouldSaveTessellation ? (isTess?1:0) : -1;
                    preset.outlineMainTex = shouldSaveMainTex2Outline;

                    EditorUtility.SetDirty(preset);
                    string savePath = EditorUtility.SaveFilePanel("Save Preset", GetPresetsFolderPath(), filename, "asset");
                    if(!string.IsNullOrEmpty(savePath))
                    {
                        AssetDatabase.CreateAsset(preset, FileUtil.GetProjectRelativePath(savePath));
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        AssetDatabase.ImportAsset(FileUtil.GetProjectRelativePath(savePath), ImportAssetOptions.ForceUpdate);
                        LoadPresets();
                        Close();
                    }
                }

                EditorGUILayout.EndScrollView();
            }

            private void CopyPropertiesToPreset(Material material)
            {
                int propCount = ShaderUtil.GetPropertyCount(material.shader);
                for(int i = 0; i < propCount; i++)
                {
                    string propName = ShaderUtil.GetPropertyName(material.shader, i);

                    if(!(
                        shouldSaveBase && lilPropertyNameChecker.IsBaseProperty(propName) ||
                        shouldSaveLighting && lilPropertyNameChecker.IsLightingProperty(propName) ||
                        shouldSaveUV && lilPropertyNameChecker.IsUVProperty(propName) ||
                        shouldSaveMain && lilPropertyNameChecker.IsMainProperty(propName) ||
                        shouldSaveMain2nd && lilPropertyNameChecker.IsMain2ndProperty(propName) ||
                        shouldSaveMain3rd && lilPropertyNameChecker.IsMain3rdProperty(propName) ||
                        shouldSaveAlphaMask && lilPropertyNameChecker.IsAlphaMaskProperty(propName) ||
                        shouldSaveShadow && lilPropertyNameChecker.IsShadowProperty(propName) ||
                        shouldSaveEmission && lilPropertyNameChecker.IsEmissionProperty(propName) ||
                        shouldSaveEmission2nd && lilPropertyNameChecker.IsEmission2ndProperty(propName) ||
                        shouldSaveNormalMap && lilPropertyNameChecker.IsNormalMapProperty(propName) ||
                        shouldSaveNormalMap2nd && lilPropertyNameChecker.IsNormalMap2ndProperty(propName) ||
                        shouldSaveAnisotropy && lilPropertyNameChecker.IsAnisotropyProperty(propName) ||
                        shouldSaveBacklight && lilPropertyNameChecker.IsBacklightProperty(propName) ||
                        shouldSaveReflection && lilPropertyNameChecker.IsReflectionProperty(propName) ||
                        shouldSaveMatCap && lilPropertyNameChecker.IsMatCapProperty(propName) ||
                        shouldSaveMatCap2nd && lilPropertyNameChecker.IsMatCap2ndProperty(propName) ||
                        shouldSaveRim && lilPropertyNameChecker.IsRimProperty(propName) ||
                        shouldSaveGlitter && lilPropertyNameChecker.IsGlitterProperty(propName) ||
                        shouldSaveParallax && lilPropertyNameChecker.IsParallaxProperty(propName) ||
                        shouldSaveDistanceFade && lilPropertyNameChecker.IsDistanceFadeProperty(propName) ||
                        shouldSaveAudioLink && lilPropertyNameChecker.IsAudioLinkProperty(propName) ||
                        shouldSaveDissolve && lilPropertyNameChecker.IsDissolveProperty(propName) ||
                        shouldSaveRefraction && lilPropertyNameChecker.IsRefractionProperty(propName) ||
                        shouldSaveGem && lilPropertyNameChecker.IsGemProperty(propName) ||
                        shouldSaveTessellation && lilPropertyNameChecker.IsTessellationProperty(propName) ||
                        shouldSaveOutline && lilPropertyNameChecker.IsOutlineProperty(propName) ||
                        shouldSaveFur && lilPropertyNameChecker.IsFurProperty(propName) ||
                        shouldSaveStencil && lilPropertyNameChecker.IsStencilProperty(propName) ||
                        shouldSaveRendering && lilPropertyNameChecker.IsRenderingProperty(propName) ||
                        shouldSaveOutlineRendering && lilPropertyNameChecker.IsOutlineRenderingProperty(propName) ||
                        shouldSaveFurRendering && lilPropertyNameChecker.IsFurRenderingProperty(propName)
                    )) continue;

                    ShaderUtil.ShaderPropertyType propType = ShaderUtil.GetPropertyType(material.shader, i);
                    if(propType == ShaderUtil.ShaderPropertyType.Color)
                    {
                        Array.Resize(ref preset.colors, preset.colors.Length + 1);
                        preset.colors[preset.colors.Length-1].name = propName;
                        preset.colors[preset.colors.Length-1].value = material.GetColor(propName);
                    }
                    if(propType == ShaderUtil.ShaderPropertyType.Vector)
                    {
                        Array.Resize(ref preset.vectors, preset.vectors.Length + 1);
                        preset.vectors[preset.vectors.Length-1].name = propName;
                        preset.vectors[preset.vectors.Length-1].value = material.GetVector(propName);
                    }
                    if(propType == ShaderUtil.ShaderPropertyType.Float || propType == ShaderUtil.ShaderPropertyType.Range)
                    {
                        if(!(!shouldSaveStencil && propName == "_StencilRef" && propName == "_StencilComp" && propName == "_StencilPass" && propName == "_StencilFail" && propName == "_StencilZFail"))
                        {
                            Array.Resize(ref preset.floats, preset.floats.Length + 1);
                            preset.floats[preset.floats.Length-1].name = propName;
                            preset.floats[preset.floats.Length-1].value = material.GetFloat(propName);
                        }
                    }
                }
            }

            private void CopyTexturesToPreset(Material material)
            {
                if(shouldSave_TriMask) CopyTextureToPreset(material, "_TriMask");
                if(shouldSave_MainTex) CopyTextureToPreset(material, "_MainTex");
                if(shouldSave_MainGradationTex) CopyTextureToPreset(material, "_MainGradationTex");
                if(shouldSave_MainColorAdjustMask) CopyTextureToPreset(material, "_MainColorAdjustMask");
                if(shouldSave_Main2ndTex) CopyTextureToPreset(material, "_Main2ndTex");
                if(shouldSave_Main2ndBlendMask) CopyTextureToPreset(material, "_Main2ndBlendMask");
                if(shouldSave_Main2ndDissolveMask) CopyTextureToPreset(material, "_Main2ndDissolveMask");
                if(shouldSave_Main2ndDissolveNoiseMask) CopyTextureToPreset(material, "_Main2ndDissolveNoiseMask");
                if(shouldSave_Main3rdTex) CopyTextureToPreset(material, "_Main3rdTex");
                if(shouldSave_Main3rdBlendMask) CopyTextureToPreset(material, "_Main3rdBlendMask");
                if(shouldSave_Main3rdDissolveMask) CopyTextureToPreset(material, "_Main3rdDissolveMask");
                if(shouldSave_Main3rdDissolveNoiseMask) CopyTextureToPreset(material, "_Main3rdDissolveNoiseMask");
                if(shouldSave_AlphaMask) CopyTextureToPreset(material, "_AlphaMask");
                if(shouldSave_ShadowStrengthMask) CopyTextureToPreset(material, "_ShadowStrengthMask");
                if(shouldSave_ShadowBorderMask) CopyTextureToPreset(material, "_ShadowBorderMask");
                if(shouldSave_ShadowBlurMask) CopyTextureToPreset(material, "_ShadowBlurMask");
                if(shouldSave_ShadowColorTex) CopyTextureToPreset(material, "_ShadowColorTex");
                if(shouldSave_Shadow2ndColorTex) CopyTextureToPreset(material, "_Shadow2ndColorTex");
                if(shouldSave_Shadow3rdColorTex) CopyTextureToPreset(material, "_Shadow3rdColorTex");
                if(shouldSave_EmissionMap) CopyTextureToPreset(material, "_EmissionMap");
                if(shouldSave_EmissionBlendMask) CopyTextureToPreset(material, "_EmissionBlendMask");
                if(shouldSave_EmissionGradTex) CopyTextureToPreset(material, "_EmissionGradTex");
                if(shouldSave_Emission2ndMap) CopyTextureToPreset(material, "_Emission2ndMap");
                if(shouldSave_Emission2ndBlendMask) CopyTextureToPreset(material, "_Emission2ndBlendMask");
                if(shouldSave_Emission2ndGradTex) CopyTextureToPreset(material, "_Emission2ndGradTex");
                if(shouldSave_BumpMap) CopyTextureToPreset(material, "_BumpMap");
                if(shouldSave_Bump2ndMap) CopyTextureToPreset(material, "_Bump2ndMap");
                if(shouldSave_Bump2ndScaleMask) CopyTextureToPreset(material, "_Bump2ndScaleMask");
                if(shouldSave_AnisotropyTangentMap) CopyTextureToPreset(material, "_AnisotropyTangentMap");
                if(shouldSave_AnisotropyScaleMask) CopyTextureToPreset(material, "_AnisotropyScaleMask");
                if(shouldSave_AnisotropyShiftNoiseMask) CopyTextureToPreset(material, "_AnisotropyShiftNoiseMask");
                if(shouldSave_BacklightColorTex) CopyTextureToPreset(material, "_BacklightColorTex");
                if(shouldSave_SmoothnessTex) CopyTextureToPreset(material, "_SmoothnessTex");
                if(shouldSave_MetallicGlossMap) CopyTextureToPreset(material, "_MetallicGlossMap");
                if(shouldSave_ReflectionColorTex) CopyTextureToPreset(material, "_ReflectionColorTex");
                if(shouldSave_MatCapTex) CopyTextureToPreset(material, "_MatCapTex");
                if(shouldSave_MatCapBlendMask) CopyTextureToPreset(material, "_MatCapBlendMask");
                if(shouldSave_MatCapBumpMap) CopyTextureToPreset(material, "_MatCapBumpMap");
                if(shouldSave_MatCap2ndTex) CopyTextureToPreset(material, "_MatCap2ndTex");
                if(shouldSave_MatCap2ndBlendMask) CopyTextureToPreset(material, "_MatCap2ndBlendMask");
                if(shouldSave_MatCap2ndBumpMap) CopyTextureToPreset(material, "_MatCap2ndBumpMap");
                if(shouldSave_RimColorTex) CopyTextureToPreset(material, "_RimColorTex");
                if(shouldSave_GlitterColorTex) CopyTextureToPreset(material, "_GlitterColorTex");
                if(shouldSave_ParallaxMap) CopyTextureToPreset(material, "_ParallaxMap");
                if(shouldSave_AudioLinkMask) CopyTextureToPreset(material, "_AudioLinkMask");
                if(shouldSave_AudioLinkLocalMap) CopyTextureToPreset(material, "_AudioLinkLocalMap");
                if(shouldSave_DissolveMask) CopyTextureToPreset(material, "_DissolveMask");
                if(shouldSave_DissolveNoiseMask) CopyTextureToPreset(material, "_DissolveNoiseMask");
                if(shouldSave_OutlineTex) CopyTextureToPreset(material, "_OutlineTex");
                if(shouldSave_OutlineWidthMask) CopyTextureToPreset(material, "_OutlineWidthMask");
                if(shouldSave_OutlineVectorTex) CopyTextureToPreset(material, "_OutlineVectorTex");
                if(shouldSave_FurNoiseMask) CopyTextureToPreset(material, "_FurNoiseMask");
                if(shouldSave_FurMask) CopyTextureToPreset(material, "_FurMask");
                if(shouldSave_FurLengthMask) CopyTextureToPreset(material, "_FurLengthMask");
                if(shouldSave_FurVectorTex) CopyTextureToPreset(material, "_FurVectorTex");
            }

            private void CopyTextureToPreset(Material material, string propName)
            {
                if(!material.HasProperty(propName)) return;
                Array.Resize(ref preset.textures, preset.textures.Length + 1);
                preset.textures[preset.textures.Length-1].name = propName;
                preset.textures[preset.textures.Length-1].value = material.GetTexture(propName);
                preset.textures[preset.textures.Length-1].offset = material.GetTextureOffset(propName);
                preset.textures[preset.textures.Length-1].scale = material.GetTextureScale(propName);
            }

            private void ToggleAllFeatures(bool val)
            {
                shouldSaveBase = val;
                shouldSaveLighting = val;
                shouldSaveUV = val;
                shouldSaveMain = val;
                shouldSaveMain2nd = val;
                shouldSaveMain3rd = val;
                shouldSaveAlphaMask = val;
                shouldSaveShadow = val;
                shouldSaveEmission = val;
                shouldSaveEmission2nd = val;
                shouldSaveNormalMap = val;
                shouldSaveNormalMap2nd = val;
                shouldSaveAnisotropy = val;
                shouldSaveBacklight = val;
                shouldSaveReflection = val;
                shouldSaveMatCap = val;
                shouldSaveMatCap2nd = val;
                shouldSaveRim = val;
                shouldSaveGlitter = val;
                shouldSaveParallax = val;
                shouldSaveDistanceFade = val;
                shouldSaveAudioLink = val;
                shouldSaveDissolve = val;
                shouldSaveRefraction = val;
                shouldSaveGem = val;
                shouldSaveTessellation = val;
                shouldSaveOutline = val;
                shouldSaveFur = val;
                shouldSaveStencil = val;
                shouldSaveRendering = val;
                shouldSaveOutlineRendering = val;
                shouldSaveFurRendering = val;
            }

            private void ToggleAllTextures(bool val)
            {
                shouldSave_TriMask = val;
                shouldSave_MainTex = val;
                shouldSave_MainGradationTex = val;
                shouldSave_MainColorAdjustMask = val;
                shouldSave_Main2ndTex = val;
                shouldSave_Main2ndBlendMask = val;
                shouldSave_Main2ndDissolveMask = val;
                shouldSave_Main2ndDissolveNoiseMask = val;
                shouldSave_Main3rdTex = val;
                shouldSave_Main3rdBlendMask = val;
                shouldSave_Main3rdDissolveMask = val;
                shouldSave_Main3rdDissolveNoiseMask = val;
                shouldSave_AlphaMask = val;
                shouldSave_ShadowStrengthMask = val;
                shouldSave_ShadowBorderMask = val;
                shouldSave_ShadowBlurMask = val;
                shouldSave_ShadowColorTex = val;
                shouldSave_Shadow2ndColorTex = val;
                shouldSave_Shadow3rdColorTex = val;
                shouldSave_EmissionMap = val;
                shouldSave_EmissionBlendMask = val;
                shouldSave_EmissionGradTex = val;
                shouldSave_Emission2ndMap = val;
                shouldSave_Emission2ndBlendMask = val;
                shouldSave_Emission2ndGradTex = val;
                shouldSave_BumpMap = val;
                shouldSave_Bump2ndMap = val;
                shouldSave_Bump2ndScaleMask = val;
                shouldSave_AnisotropyTangentMap = val;
                shouldSave_AnisotropyScaleMask = val;
                shouldSave_AnisotropyShiftNoiseMask = val;
                shouldSave_BacklightColorTex = val;
                shouldSave_SmoothnessTex = val;
                shouldSave_MetallicGlossMap = val;
                shouldSave_ReflectionColorTex = val;
                shouldSave_MatCapTex = val;
                shouldSave_MatCapBlendMask = val;
                shouldSave_MatCapBumpMap = val;
                shouldSave_MatCap2ndTex = val;
                shouldSave_MatCap2ndBlendMask = val;
                shouldSave_MatCap2ndBumpMap = val;
                shouldSave_RimColorTex = val;
                shouldSave_GlitterColorTex = val;
                shouldSave_ParallaxMap = val;
                shouldSave_AudioLinkMask = val;
                shouldSave_AudioLinkLocalMap = val;
                shouldSave_DissolveMask = val;
                shouldSave_DissolveNoiseMask = val;
                shouldSave_OutlineTex = val;
                shouldSave_OutlineWidthMask = val;
                shouldSave_OutlineVectorTex = val;
                shouldSave_FurNoiseMask = val;
                shouldSave_FurMask = val;
                shouldSave_FurLengthMask = val;
                shouldSave_FurVectorTex = val;
            }
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
                UnityEngine.Object[] objects = Selection.GetFiltered<Material>(SelectionMode.DeepAssets).Where(obj => obj.shader != null).Where(obj => obj.shader.name.Contains("lilToon")).ToArray();
                if(objects.Length == 0) return;

                props = MaterialEditor.GetMaterialProperties(objects);
                if(props == null) return;

                material = (Material)objects[0];
                isCustomEditor = true;
                isMultiVariants = objects.Any(obj => ((Material)obj).shader != material.shader);
                materialEditor = (MaterialEditor)Editor.CreateEditor(objects, typeof(MaterialEditor));
                lilToonInspector inspector = new lilToonInspector();

                EditorGUILayout.LabelField("Selected Materials", string.Join(", ", objects.Select(obj => obj.name).ToArray()), EditorStyles.boldLabel);
                DrawLine();
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
        // Gif to Atlas
        #region
        #if SYSTEM_DRAWING
            public static string ConvertGifToAtlas(UnityEngine.Object tex)
            {
                int frameCount, loopXY, duration;
                float xScale, yScale;
                return ConvertGifToAtlas(tex, out frameCount, out loopXY, out duration, out xScale, out yScale);
            }

            public void ConvertGifToAtlas(MaterialProperty tex, MaterialProperty decalAnimation, MaterialProperty decalSubParam, MaterialProperty isDecal)
            {
                if(tex.textureValue != null && AssetDatabase.GetAssetPath(tex.textureValue).EndsWith(".gif", StringComparison.OrdinalIgnoreCase) && EditorButton(GetLoc("sConvertGif")))
                {
                    int frameCount, loopXY, duration;
                    float xScale, yScale;
                    string savePath = ConvertGifToAtlas(tex.textureValue, out frameCount, out loopXY, out duration, out xScale, out yScale);

                    tex.textureValue = AssetDatabase.LoadAssetAtPath<Texture2D>(savePath);
                    decalAnimation.vectorValue = new Vector4(loopXY,loopXY,(float)frameCount,100.0f/(float)duration);
                    decalSubParam.vectorValue = new Vector4(xScale,yScale,decalSubParam.vectorValue.z,decalSubParam.vectorValue.w);
                    isDecal.floatValue = 1.0f;
                }
            }

            public static string ConvertGifToAtlas(UnityEngine.Object tex, out int frameCount, out int loopXY, out int duration, out float xScale, out float yScale)
            {
                string path = AssetDatabase.GetAssetPath(tex);
                System.Drawing.Image origGif = System.Drawing.Image.FromFile(path);
                System.Drawing.Imaging.FrameDimension dimension = new System.Drawing.Imaging.FrameDimension(origGif.FrameDimensionsList[0]);
                frameCount = origGif.GetFrameCount(dimension);
                loopXY = Mathf.CeilToInt(Mathf.Sqrt(frameCount));
                duration = BitConverter.ToInt32(origGif.GetPropertyItem(20736).Value, 0);
                int finalWidth = 1;
                int finalHeight = 1;
                if(EditorUtility.DisplayDialog(GetLoc("sDialogGifToAtlas"), GetLoc("sUtilGif2AtlasPow2"), GetLoc("sYes"), GetLoc("sNo")))
                {
                    while(finalWidth < origGif.Width * loopXY) finalWidth *= 2;
                    while(finalHeight < origGif.Height * loopXY) finalHeight *= 2;
                }
                else
                {
                    finalWidth = origGif.Width * loopXY;
                    finalHeight = origGif.Height * loopXY;
                }
                Texture2D atlasTexture = new Texture2D(finalWidth, finalHeight);
                xScale = (float)(origGif.Width * loopXY) / (float)finalWidth;
                yScale = (float)(origGif.Height * loopXY) / (float)finalHeight;
                for(int x = 0; x < finalWidth; x++)
                {
                    for(int y = 0; y < finalHeight; y++)
                    {
                        atlasTexture.SetPixel(x, finalHeight - 1 - y, Color.clear);
                    }
                }
                for(int i = 0; i < frameCount; i++)
                {
                    int offsetX = i%loopXY;
                    int offsetY = Mathf.FloorToInt(i/loopXY);
                    origGif.SelectActiveFrame(dimension, i);
                    System.Drawing.Bitmap frame = new System.Drawing.Bitmap(origGif.Width, origGif.Height);
                    System.Drawing.Graphics.FromImage(frame).DrawImage(origGif, System.Drawing.Point.Empty);

                    for(int x = 0; x < frame.Width; x++)
                    {
                        for(int y = 0; y < frame.Height; y++)
                        {
                            System.Drawing.Color sourceColor = frame.GetPixel(x, y);
                            atlasTexture.SetPixel(x + (frame.Width * offsetX), finalHeight - (frame.Height * offsetY) - 1 - y, new Color32(sourceColor.R, sourceColor.G, sourceColor.B, sourceColor.A));
                        }
                    }
                }
                atlasTexture.Apply();

                // Save
                string savePath = SavePng(path, "_gif2png_" + loopXY + "_" + frameCount + "_" + duration, atlasTexture);
                AssetDatabase.Refresh();
                return savePath;
            }
        #endif
        #endregion
    }

    public enum lilPresetCategory
    {
        Skin,
        Hair,
        Cloth,
        Nature,
        Inorganic,
        Effect,
        Other
    }

    public enum lilPropertyBlock
    {
        Base,
        Lighting,
        UV,
        MainColor,
        MainColor1st,
        MainColor2nd,
        MainColor3rd,
        AlphaMask,
        Shadow,
        Emission,
        Emission1st,
        Emission2nd,
        NormalMap,
        NormalMap1st,
        NormalMap2nd,
        Anisotropy,
        Reflections,
        Reflection,
        MatCaps,
        MatCap1st,
        MatCap2nd,
        RimLight,
        Glitter,
        Backlight,
        Gem,
        Outline,
        Parallax,
        DistanceFade,
        AudioLink,
        Dissolve,
        Refraction,
        Fur,
        Encryption,
        Stencil,
        Rendering,
        Tessellation,
        Other
    }

    public class lilMaterialProperty
    {
        public MaterialProperty p;
        public List<lilPropertyBlock> blocks;
        public bool isTexture;

        // Values
        //public int intValue
        //{
        //    get { return p.intValue; }
        //    set { p.intValue = value; }
        //}

        public float floatValue
        {
            get { return p.floatValue; }
            set { p.floatValue = value; }
        }

        public Vector4 vectorValue
        {
            get { return p.vectorValue; }
            set { p.vectorValue = value; }
        }

        public Color colorValue
        {
            get { return p.colorValue; }
            set { p.colorValue = value; }
        }

        public Texture textureValue
        {
            get { return p.textureValue; }
            set { p.textureValue = value; }
        }

        // Other
        public string name
        {
            get { return p.name; }
            private set { }
        }
        public string displayName
        {
            get { return p.displayName; }
            private set { }
        }

        public MaterialProperty.PropFlags flags
        {
            get { return p.flags; }
            private set { }
        }

        public bool hasMixedValue
        {
            get { return p.hasMixedValue; }
            private set { }
        }

        public Vector2 rangeLimits
        {
            get { return p.rangeLimits; }
            private set { }
        }

        public UnityEngine.Object[] targets
        {
            get { return p.targets; }
            private set { }
        }

        public UnityEngine.Rendering.TextureDimension textureDimension
        {
            get { return p.textureDimension; }
            private set { }
        }

        public MaterialProperty.PropType type
        {
            get { return p.type; }
            private set { }
        }

        public lilMaterialProperty()
        {
            p = null;
            blocks = new List<lilPropertyBlock>();
            isTexture = false;
        }

        public lilMaterialProperty(MaterialProperty prop)
        {
            p = prop;
        }

        public static implicit operator MaterialProperty(lilMaterialProperty prop)
        {
            return prop.p;
        }
    }
}
#endif