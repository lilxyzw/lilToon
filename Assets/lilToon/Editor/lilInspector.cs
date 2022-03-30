#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

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

        public enum lilRenderPipeline
        {
            BRP,
            LWRP,
            URP,
            HDRP
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
            Tessellation
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
        public const string currentVersionName = "1.2.12";
        public const int currentVersionValue = 25;

        private const string boothURL = "https://lilxyzw.booth.pm/";
        private const string githubURL = "https://github.com/lilxyzw/lilToon";
        public const string versionInfoURL = "https://raw.githubusercontent.com/lilxyzw/lilToon/master/version.json";
        private const string mainFolderGUID                 = "05d1d116436047941ad97d1b9064ee05"; // "Assets/lilToon"
        private const string editorFolderGUID               = "3e73d675b9c1adc4f8b6b8ef01bce51c"; // "Assets/lilToon/Editor"
        private const string presetsFolderGUID              = "35817d21af2f3134182c4a7e4c07786b"; // "Assets/lilToon/Presets"
        private const string editorGUID                     = "aefa51cbc37d602418a38a02c3b9afb9"; // "Assets/lilToon/Editor/lilInspector.cs"
        private const string editorLanguageFileGUID         = "a63ad2f5296744a4bad011de744ba8ba"; // "Assets/lilToon/Editor/Resources/lang.txt"
        private const string editorGUIBoxInDarkGUID         = "bb1313c9ea1425b41b74e98fd04bcbc8"; // "Assets/lilToon/Editor/Resources/gui_box_inner_dark.guiskin"
        private const string editorGUIBoxInLightGUID        = "f18d71f528511e748887f5e246abcc16"; // "Assets/lilToon/Editor/Resources/gui_box_inner_light.guiskin"
        private const string editorGUIBoxInHalfDarkGUID     = "a72199a4c9cc3714d8edfbc5d3b13823"; // "Assets/lilToon/Editor/Resources/gui_box_inner_half_dark.guiskin"
        private const string editorGUIBoxInHalfLightGUID    = "8343038a4a0cbef4d8af45c073520436"; // "Assets/lilToon/Editor/Resources/gui_box_inner_half_light.guiskin"
        private const string editorGUIBoxOutDarkGUID        = "29f3c01461cd0474eab36bf2e939bb58"; // "Assets/lilToon/Editor/Resources/gui_box_outer_dark.guiskin"
        private const string editorGUIBoxOutLightGUID       = "16cc103a658d8404894e66dd8f35cb77"; // "Assets/lilToon/Editor/Resources/gui_box_outer_light.guiskin"
        private const string editorGUICustomBoxDarkGUID     = "45dfb1bafd2c7d34ab453c29c0b1f46e"; // "Assets/lilToon/Editor/Resources/gui_custom_box_dark.guiskin"
        private const string editorGUICustomBoxLightGUID    = "a1ed8756474bfd34f80fa22e6c43b2e5"; // "Assets/lilToon/Editor/Resources/gui_custom_box_light.guiskin"
        private const string shaderFolderGUID               = "ac0a8f602b5e72f458f4914bf08f0269"; // "Assets/lilToon/Shader"
        private const string shaderPipelineGUID             = "32299664512e2e042bbc351c1d46d383"; // "Assets/lilToon/Shader/Includes/lil_pipeline.hlsl";
        private const string shaderCommonGUID               = "5520e766422958546bbe885a95d5a67e"; // "Assets/lilToon/Shader/Includes/lil_common.hlsl";
        private const string avatarEncryptionGUID           = "f9787bf8ed5154f4b931278945ac8ca1"; // "Assets/AvaterEncryption";
        private const string editorSettingTempPath          = "Temp/lilToonEditorSetting";
        public const string versionInfoTempPath             = "Temp/lilToonVersion";
        public const string packageListTempPath             = "Temp/lilToonPackageList";
        private static readonly string[] mainTexCheckWords = new[] {"mask", "shadow", "shade", "outline", "normal", "bumpmap", "matcap", "rimlight", "emittion", "reflection", "specular", "roughness", "smoothness", "metallic", "metalness", "opacity", "parallax", "displacement", "height", "ambient", "occlusion"};

        #if NET_4_6
            public const string rspPath = "Assets/csc.rsp";
        #else
            public const string rspPath = "Assets/mcs.rsp";
        #endif

        public static string GetMainFolderPath()
        {
            return AssetDatabase.GUIDToAssetPath(mainFolderGUID);
        }
        public static string GetEditorFolderPath()
        {
            return AssetDatabase.GUIDToAssetPath(editorFolderGUID);
        }
        public static string GetPresetsFolderPath()
        {
            return AssetDatabase.GUIDToAssetPath(presetsFolderGUID);
        }
        public static string GetEditorPath()
        {
            return AssetDatabase.GUIDToAssetPath(editorGUID);
        }
        public static string GetShaderFolderPath()
        {
            return AssetDatabase.GUIDToAssetPath(shaderFolderGUID);
        }
        public static string[] GetShaderFolderPaths()
        {
            return new[] {GetShaderFolderPath()};
        }
        public static string GetShaderPipelinePath()
        {
            return AssetDatabase.GUIDToAssetPath(shaderPipelineGUID);
        }
        public static string GetShaderCommonPath()
        {
            return AssetDatabase.GUIDToAssetPath(shaderCommonGUID);
        }
        public static string GetSettingFolderPath()
        {
            if(isUPM) return "Assets/lilToonSetting";
            return GetMainFolderPath() + "Setting";
        }
        public static string GetShaderSettingPath()
        {
            return GetSettingFolderPath() + "/ShaderSetting.asset";
        }
        public static string GetShaderSettingHLSLPath()
        {
            return GetSettingFolderPath() + "/lil_setting.hlsl";
        }
        public static string GetEditorLanguageFileGUID()
        {
            return AssetDatabase.GUIDToAssetPath(editorLanguageFileGUID);
        }
        public static string GetAvatarEncryptionPath()
        {
            return AssetDatabase.GUIDToAssetPath(avatarEncryptionGUID);
        }
        public static string GetGUIBoxInDarkPath()
        {
            return AssetDatabase.GUIDToAssetPath(editorGUIBoxInDarkGUID);
        }
        public static string GetGUIBoxInLightPath()
        {
            return AssetDatabase.GUIDToAssetPath(editorGUIBoxInLightGUID);
        }
        public static string GetGUIBoxInHalfDarkPath()
        {
            return AssetDatabase.GUIDToAssetPath(editorGUIBoxInHalfDarkGUID);
        }
        public static string GetGUIBoxInHalfLightPath()
        {
            return AssetDatabase.GUIDToAssetPath(editorGUIBoxInHalfLightGUID);
        }
        public static string GetGUIBoxOutDarkPath()
        {
            return AssetDatabase.GUIDToAssetPath(editorGUIBoxOutDarkGUID);
        }
        public static string GetGUIBoxOutLightPath()
        {
            return AssetDatabase.GUIDToAssetPath(editorGUIBoxOutLightGUID);
        }
        public static string GetGUICustomBoxDarkPath()
        {
            return AssetDatabase.GUIDToAssetPath(editorGUICustomBoxDarkGUID);
        }
        public static string GetGUICustomBoxLightPath()
        {
            return AssetDatabase.GUIDToAssetPath(editorGUICustomBoxLightGUID);
        }

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
            public bool isShaderSettingChanged          = false;
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
        public static bool isUPM = false;
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
        private MaterialProperty transparentModeMat;
        private MaterialProperty asOverlay;
        private MaterialProperty invisible;
        private MaterialProperty asUnlit;
        private MaterialProperty cutoff;
        private MaterialProperty subpassCutoff;
        private MaterialProperty flipNormal;
        private MaterialProperty shiftBackfaceUV;
        private MaterialProperty backfaceForceShadow;
        private MaterialProperty vertexLightStrength;
        private MaterialProperty lightMinLimit;
        private MaterialProperty lightMaxLimit;
        private MaterialProperty beforeExposureLimit;
        private MaterialProperty monochromeLighting;
        private MaterialProperty alphaBoostFA;
        private MaterialProperty lilDirectionalLightStrength;
        private MaterialProperty lightDirectionOverride;
        private MaterialProperty baseColor;
        private MaterialProperty baseMap;
        private MaterialProperty baseColorMap;
        private MaterialProperty triMask;
            private MaterialProperty cull;
            private MaterialProperty srcBlend;
            private MaterialProperty dstBlend;
            private MaterialProperty srcBlendAlpha;
            private MaterialProperty dstBlendAlpha;
            private MaterialProperty blendOp;
            private MaterialProperty blendOpAlpha;
            private MaterialProperty srcBlendFA;
            private MaterialProperty dstBlendFA;
            private MaterialProperty srcBlendAlphaFA;
            private MaterialProperty dstBlendAlphaFA;
            private MaterialProperty blendOpFA;
            private MaterialProperty blendOpAlphaFA;
            private MaterialProperty zclip;
            private MaterialProperty zwrite;
            private MaterialProperty ztest;
            private MaterialProperty stencilRef;
            private MaterialProperty stencilReadMask;
            private MaterialProperty stencilWriteMask;
            private MaterialProperty stencilComp;
            private MaterialProperty stencilPass;
            private MaterialProperty stencilFail;
            private MaterialProperty stencilZFail;
            private MaterialProperty offsetFactor;
            private MaterialProperty offsetUnits;
            private MaterialProperty colorMask;
            private MaterialProperty alphaToMask;
        //private MaterialProperty useMainTex;
            private MaterialProperty mainColor;
            private MaterialProperty mainTex;
            private MaterialProperty mainTexHSVG;
            private MaterialProperty mainTex_ScrollRotate;
            private MaterialProperty mainGradationStrength;
            private MaterialProperty mainGradationTex;
            private MaterialProperty mainColorAdjustMask;
        private MaterialProperty useMain2ndTex;
            private MaterialProperty mainColor2nd;
            private MaterialProperty main2ndTex;
            private MaterialProperty main2ndTex_UVMode;
            private MaterialProperty main2ndTexAngle;
            private MaterialProperty main2ndTexDecalAnimation;
            private MaterialProperty main2ndTexDecalSubParam;
            private MaterialProperty main2ndTexIsDecal;
            private MaterialProperty main2ndTexIsLeftOnly;
            private MaterialProperty main2ndTexIsRightOnly;
            private MaterialProperty main2ndTexShouldCopy;
            private MaterialProperty main2ndTexShouldFlipMirror;
            private MaterialProperty main2ndTexShouldFlipCopy;
            private MaterialProperty main2ndTexIsMSDF;
            private MaterialProperty main2ndBlendMask;
            private MaterialProperty main2ndTexBlendMode;
            private MaterialProperty main2ndEnableLighting;
            private MaterialProperty main2ndDissolveMask;
            private MaterialProperty main2ndDissolveNoiseMask;
            private MaterialProperty main2ndDissolveNoiseMask_ScrollRotate;
            private MaterialProperty main2ndDissolveNoiseStrength;
            private MaterialProperty main2ndDissolveColor;
            private MaterialProperty main2ndDissolveParams;
            private MaterialProperty main2ndDissolvePos;
            private MaterialProperty main2ndDistanceFade;
        private MaterialProperty useMain3rdTex;
            private MaterialProperty mainColor3rd;
            private MaterialProperty main3rdTex;
            private MaterialProperty main3rdTex_UVMode;
            private MaterialProperty main3rdTexAngle;
            private MaterialProperty main3rdTexDecalAnimation;
            private MaterialProperty main3rdTexDecalSubParam;
            private MaterialProperty main3rdTexIsDecal;
            private MaterialProperty main3rdTexIsLeftOnly;
            private MaterialProperty main3rdTexIsRightOnly;
            private MaterialProperty main3rdTexShouldCopy;
            private MaterialProperty main3rdTexShouldFlipMirror;
            private MaterialProperty main3rdTexShouldFlipCopy;
            private MaterialProperty main3rdTexIsMSDF;
            private MaterialProperty main3rdBlendMask;
            private MaterialProperty main3rdTexBlendMode;
            private MaterialProperty main3rdEnableLighting;
            private MaterialProperty main3rdDissolveMask;
            private MaterialProperty main3rdDissolveNoiseMask;
            private MaterialProperty main3rdDissolveNoiseMask_ScrollRotate;
            private MaterialProperty main3rdDissolveNoiseStrength;
            private MaterialProperty main3rdDissolveColor;
            private MaterialProperty main3rdDissolveParams;
            private MaterialProperty main3rdDissolvePos;
            private MaterialProperty main3rdDistanceFade;
        private MaterialProperty alphaMaskMode;
            private MaterialProperty alphaMask;
            private MaterialProperty alphaMaskScale;
            private MaterialProperty alphaMaskValue;
        private MaterialProperty useShadow;
            private MaterialProperty shadowStrength;
            private MaterialProperty shadowStrengthMask;
            private MaterialProperty shadowBorderMask;
            private MaterialProperty shadowBlurMask;
            private MaterialProperty shadowAOShift;
            private MaterialProperty shadowAOShift2;
            private MaterialProperty shadowPostAO;
            private MaterialProperty shadowColor;
            private MaterialProperty shadowColorTex;
            private MaterialProperty shadowNormalStrength;
            private MaterialProperty shadowBorder;
            private MaterialProperty shadowBlur;
            private MaterialProperty shadow2ndColor;
            private MaterialProperty shadow2ndColorTex;
            private MaterialProperty shadow2ndNormalStrength;
            private MaterialProperty shadow2ndBorder;
            private MaterialProperty shadow2ndBlur;
            private MaterialProperty shadow3rdColor;
            private MaterialProperty shadow3rdColorTex;
            private MaterialProperty shadow3rdNormalStrength;
            private MaterialProperty shadow3rdBorder;
            private MaterialProperty shadow3rdBlur;
            private MaterialProperty shadowMainStrength;
            private MaterialProperty shadowEnvStrength;
            private MaterialProperty shadowBorderColor;
            private MaterialProperty shadowBorderRange;
            private MaterialProperty shadowReceive;
        private MaterialProperty useBacklight;
            private MaterialProperty backlightColor;
            private MaterialProperty backlightColorTex;
            private MaterialProperty backlightNormalStrength;
            private MaterialProperty backlightBorder;
            private MaterialProperty backlightBlur;
            private MaterialProperty backlightDirectivity;
            private MaterialProperty backlightViewStrength;
            private MaterialProperty backlightReceiveShadow;
            private MaterialProperty backlightBackfaceMask;
        private MaterialProperty useBumpMap;
            private MaterialProperty bumpMap;
            private MaterialProperty bumpScale;
        private MaterialProperty useBump2ndMap;
            private MaterialProperty bump2ndMap;
            private MaterialProperty bump2ndScale;
            private MaterialProperty bump2ndScaleMask;
        private MaterialProperty useAnisotropy;
            private MaterialProperty anisotropyTangentMap;
            private MaterialProperty anisotropyScale;
            private MaterialProperty anisotropyScaleMask;
            private MaterialProperty anisotropyTangentWidth;
            private MaterialProperty anisotropyBitangentWidth;
            private MaterialProperty anisotropyShift;
            private MaterialProperty anisotropyShiftNoiseScale;
            private MaterialProperty anisotropySpecularStrength;
            private MaterialProperty anisotropy2ndTangentWidth;
            private MaterialProperty anisotropy2ndBitangentWidth;
            private MaterialProperty anisotropy2ndShift;
            private MaterialProperty anisotropy2ndShiftNoiseScale;
            private MaterialProperty anisotropy2ndSpecularStrength;
            private MaterialProperty anisotropyShiftNoiseMask;
            private MaterialProperty anisotropy2Reflection;
            private MaterialProperty anisotropy2MatCap;
            private MaterialProperty anisotropy2MatCap2nd;
        private MaterialProperty useReflection;
            private MaterialProperty metallic;
            private MaterialProperty metallicGlossMap;
            private MaterialProperty smoothness;
            private MaterialProperty smoothnessTex;
            private MaterialProperty reflectance;
            private MaterialProperty reflectionColor;
            private MaterialProperty reflectionColorTex;
            private MaterialProperty applySpecular;
            private MaterialProperty applySpecularFA;
            private MaterialProperty specularNormalStrength;
            private MaterialProperty specularToon;
            private MaterialProperty specularBorder;
            private MaterialProperty specularBlur;
            private MaterialProperty applyReflection;
            private MaterialProperty reflectionNormalStrength;
            private MaterialProperty reflectionApplyTransparency;
            private MaterialProperty reflectionCubeTex;
            private MaterialProperty reflectionCubeColor;
            private MaterialProperty reflectionCubeOverride;
            private MaterialProperty reflectionCubeEnableLighting;
        private MaterialProperty useMatCap;
            private MaterialProperty matcapTex;
            private MaterialProperty matcapColor;
            private MaterialProperty matcapBlendUV1;
            private MaterialProperty matcapZRotCancel;
            private MaterialProperty matcapPerspective;
            private MaterialProperty matcapVRParallaxStrength;
            private MaterialProperty matcapBlend;
            private MaterialProperty matcapBlendMask;
            private MaterialProperty matcapEnableLighting;
            private MaterialProperty matcapShadowMask;
            private MaterialProperty matcapBackfaceMask;
            private MaterialProperty matcapLod;
            private MaterialProperty matcapBlendMode;
            private MaterialProperty matcapMul;
            private MaterialProperty matcapApplyTransparency;
            private MaterialProperty matcapNormalStrength;
            private MaterialProperty matcapCustomNormal;
            private MaterialProperty matcapBumpMap;
            private MaterialProperty matcapBumpScale;
        private MaterialProperty useMatCap2nd;
            private MaterialProperty matcap2ndTex;
            private MaterialProperty matcap2ndColor;
            private MaterialProperty matcap2ndBlendUV1;
            private MaterialProperty matcap2ndZRotCancel;
            private MaterialProperty matcap2ndPerspective;
            private MaterialProperty matcap2ndVRParallaxStrength;
            private MaterialProperty matcap2ndBlend;
            private MaterialProperty matcap2ndBlendMask;
            private MaterialProperty matcap2ndEnableLighting;
            private MaterialProperty matcap2ndShadowMask;
            private MaterialProperty matcap2ndBackfaceMask;
            private MaterialProperty matcap2ndLod;
            private MaterialProperty matcap2ndBlendMode;
            private MaterialProperty matcap2ndMul;
            private MaterialProperty matcap2ndApplyTransparency;
            private MaterialProperty matcap2ndNormalStrength;
            private MaterialProperty matcap2ndCustomNormal;
            private MaterialProperty matcap2ndBumpMap;
            private MaterialProperty matcap2ndBumpScale;
        private MaterialProperty useRim;
            private MaterialProperty rimColor;
            private MaterialProperty rimColorTex;
            private MaterialProperty rimNormalStrength;
            private MaterialProperty rimBorder;
            private MaterialProperty rimBlur;
            private MaterialProperty rimFresnelPower;
            private MaterialProperty rimEnableLighting;
            private MaterialProperty rimShadowMask;
            private MaterialProperty rimBackfaceMask;
            private MaterialProperty rimVRParallaxStrength;
            private MaterialProperty rimApplyTransparency;
            private MaterialProperty rimDirStrength;
            private MaterialProperty rimDirRange;
            private MaterialProperty rimIndirRange;
            private MaterialProperty rimIndirColor;
            private MaterialProperty rimIndirBorder;
            private MaterialProperty rimIndirBlur;
        private MaterialProperty useGlitter;
            private MaterialProperty glitterUVMode;
            private MaterialProperty glitterColor;
            private MaterialProperty glitterColorTex;
            private MaterialProperty glitterMainStrength;
            private MaterialProperty glitterParams1;
            private MaterialProperty glitterParams2;
            private MaterialProperty glitterPostContrast;
            private MaterialProperty glitterEnableLighting;
            private MaterialProperty glitterShadowMask;
            private MaterialProperty glitterBackfaceMask;
            private MaterialProperty glitterApplyTransparency;
            private MaterialProperty glitterVRParallaxStrength;
            private MaterialProperty glitterNormalStrength;
        private MaterialProperty useEmission;
            private MaterialProperty emissionColor;
            private MaterialProperty emissionMap;
            private MaterialProperty emissionMap_ScrollRotate;
            private MaterialProperty emissionMap_UVMode;
            private MaterialProperty emissionBlend;
            private MaterialProperty emissionBlendMask;
            private MaterialProperty emissionBlendMask_ScrollRotate;
            private MaterialProperty emissionBlink;
            private MaterialProperty emissionUseGrad;
            private MaterialProperty emissionGradTex;
            private MaterialProperty emissionGradSpeed;
            private MaterialProperty emissionParallaxDepth;
            private MaterialProperty emissionFluorescence;
        private MaterialProperty useEmission2nd;
            private MaterialProperty emission2ndColor;
            private MaterialProperty emission2ndMap;
            private MaterialProperty emission2ndMap_ScrollRotate;
            private MaterialProperty emission2ndMap_UVMode;
            private MaterialProperty emission2ndBlend;
            private MaterialProperty emission2ndBlendMask;
            private MaterialProperty emission2ndBlendMask_ScrollRotate;
            private MaterialProperty emission2ndBlink;
            private MaterialProperty emission2ndUseGrad;
            private MaterialProperty emission2ndGradTex;
            private MaterialProperty emission2ndGradSpeed;
            private MaterialProperty emission2ndParallaxDepth;
            private MaterialProperty emission2ndFluorescence;
        //private MaterialProperty useOutline;
            private MaterialProperty outlineColor;
            private MaterialProperty outlineTex;
            private MaterialProperty outlineTex_ScrollRotate;
            private MaterialProperty outlineTexHSVG;
            private MaterialProperty outlineWidth;
            private MaterialProperty outlineWidthMask;
            private MaterialProperty outlineFixWidth;
            private MaterialProperty outlineVertexR2Width;
            private MaterialProperty outlineVectorTex;
            private MaterialProperty outlineVectorScale;
            private MaterialProperty outlineEnableLighting;
            private MaterialProperty outlineCull;
            private MaterialProperty outlineSrcBlend;
            private MaterialProperty outlineDstBlend;
            private MaterialProperty outlineSrcBlendAlpha;
            private MaterialProperty outlineDstBlendAlpha;
            private MaterialProperty outlineBlendOp;
            private MaterialProperty outlineBlendOpAlpha;
            private MaterialProperty outlineSrcBlendFA;
            private MaterialProperty outlineDstBlendFA;
            private MaterialProperty outlineSrcBlendAlphaFA;
            private MaterialProperty outlineDstBlendAlphaFA;
            private MaterialProperty outlineBlendOpFA;
            private MaterialProperty outlineBlendOpAlphaFA;
            private MaterialProperty outlineZclip;
            private MaterialProperty outlineZwrite;
            private MaterialProperty outlineZtest;
            private MaterialProperty outlineStencilRef;
            private MaterialProperty outlineStencilReadMask;
            private MaterialProperty outlineStencilWriteMask;
            private MaterialProperty outlineStencilComp;
            private MaterialProperty outlineStencilPass;
            private MaterialProperty outlineStencilFail;
            private MaterialProperty outlineStencilZFail;
            private MaterialProperty outlineOffsetFactor;
            private MaterialProperty outlineOffsetUnits;
            private MaterialProperty outlineColorMask;
            private MaterialProperty outlineAlphaToMask;
        private MaterialProperty useParallax;
            private MaterialProperty parallaxMap;
            private MaterialProperty parallax;
            private MaterialProperty parallaxOffset;
            private MaterialProperty usePOM;
        //private MaterialProperty useDistanceFade;
            private MaterialProperty distanceFadeColor;
            private MaterialProperty distanceFade;
        private MaterialProperty useClippingCanceller;
        private MaterialProperty useAudioLink;
            private MaterialProperty audioLinkDefaultValue;
            private MaterialProperty audioLinkUVMode;
            private MaterialProperty audioLinkUVParams;
            private MaterialProperty audioLinkStart;
            private MaterialProperty audioLinkMask;
            private MaterialProperty audioLink2Main2nd;
            private MaterialProperty audioLink2Main3rd;
            private MaterialProperty audioLink2Emission;
            private MaterialProperty audioLink2EmissionGrad;
            private MaterialProperty audioLink2Emission2nd;
            private MaterialProperty audioLink2Emission2ndGrad;
            private MaterialProperty audioLink2Vertex;
            private MaterialProperty audioLinkVertexUVMode;
            private MaterialProperty audioLinkVertexUVParams;
            private MaterialProperty audioLinkVertexStart;
            private MaterialProperty audioLinkVertexStrength;
            private MaterialProperty audioLinkAsLocal;
            private MaterialProperty audioLinkLocalMap;
            private MaterialProperty audioLinkLocalMapParams;
        //private MaterialProperty useDissolve;
            private MaterialProperty dissolveMask;
            private MaterialProperty dissolveNoiseMask;
            private MaterialProperty dissolveNoiseMask_ScrollRotate;
            private MaterialProperty dissolveNoiseStrength;
            private MaterialProperty dissolveColor;
            private MaterialProperty dissolveParams;
            private MaterialProperty dissolvePos;
        //private MaterialProperty useEncryption
            private MaterialProperty ignoreEncryption;
            private MaterialProperty keys;
        //private MaterialProperty useRefraction;
            private MaterialProperty refractionStrength;
            private MaterialProperty refractionFresnelPower;
            private MaterialProperty refractionColorFromMain;
            private MaterialProperty refractionColor;
        //private MaterialProperty useFur;
            private MaterialProperty furNoiseMask;
            private MaterialProperty furMask;
            private MaterialProperty furLengthMask;
            private MaterialProperty furVectorTex;
            private MaterialProperty furVectorScale;
            private MaterialProperty furVector;
            private MaterialProperty furGravity;
            private MaterialProperty furRandomize;
            private MaterialProperty furAO;
            private MaterialProperty vertexColor2FurVector;
            private MaterialProperty furLayerNum;
            private MaterialProperty furRootOffset;
            private MaterialProperty furCutoutLength;
            private MaterialProperty furTouchStrength;
            private MaterialProperty furCull;
            private MaterialProperty furSrcBlend;
            private MaterialProperty furDstBlend;
            private MaterialProperty furSrcBlendAlpha;
            private MaterialProperty furDstBlendAlpha;
            private MaterialProperty furBlendOp;
            private MaterialProperty furBlendOpAlpha;
            private MaterialProperty furSrcBlendFA;
            private MaterialProperty furDstBlendFA;
            private MaterialProperty furSrcBlendAlphaFA;
            private MaterialProperty furDstBlendAlphaFA;
            private MaterialProperty furBlendOpFA;
            private MaterialProperty furBlendOpAlphaFA;
            private MaterialProperty furZclip;
            private MaterialProperty furZwrite;
            private MaterialProperty furZtest;
            private MaterialProperty furStencilRef;
            private MaterialProperty furStencilReadMask;
            private MaterialProperty furStencilWriteMask;
            private MaterialProperty furStencilComp;
            private MaterialProperty furStencilPass;
            private MaterialProperty furStencilFail;
            private MaterialProperty furStencilZFail;
            private MaterialProperty furOffsetFactor;
            private MaterialProperty furOffsetUnits;
            private MaterialProperty furColorMask;
            private MaterialProperty furAlphaToMask;
        //private MaterialProperty useTessellation;
            private MaterialProperty tessEdge;
            private MaterialProperty tessStrength;
            private MaterialProperty tessShrink;
            private MaterialProperty tessFactorMax;
        //private MaterialProperty useGem;
            private MaterialProperty gemChromaticAberration;
            private MaterialProperty gemEnvContrast;
            private MaterialProperty gemEnvColor;
            private MaterialProperty gemParticleLoop;
            private MaterialProperty gemParticleColor;
            private MaterialProperty gemVRParallaxStrength;
        //private MaterialProperty useFakeShadow;
            private MaterialProperty fakeShadowVector;
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
            //m_MaterialEditor.isVisible = true;
            ApplyEditorSettingTemp();
            InitializeShaders();
            InitializeShaderSetting(ref shaderSetting);

            //------------------------------------------------------------------------------------------------------------------------------
            // Load Properties
            LoadProperties(props);

            //------------------------------------------------------------------------------------------------------------------------------
            // Check Shader Type
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

            if(!isMultiVariants && material.shader.name.Contains("Overlay") && AutoFixHelpBox(GetLoc("sHelpSelectOverlay")))
            {
                material.shader = lts;
            }

            EditorGUILayout.Space();

            //------------------------------------------------------------------------------------------------------------------------------
            // Simple
            if(edSet.editorMode == EditorMode.Simple)
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

                edSet.isShowMain = Foldout(GetLoc("sMainColorSetting"), edSet.isShowMain);
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
                            if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION))
                            {
                                ToneCorrectionGUI(mainTexHSVG);
                            }
                            if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP))
                            {
                                DrawLine();
                                EditorGUILayout.LabelField(GetLoc("sGradationMap"), boldLabel);
                                m_MaterialEditor.ShaderProperty(mainGradationStrength, GetLoc("sStrength"));
                                if(mainGradationStrength.floatValue != 0)
                                {
                                    m_MaterialEditor.TexturePropertySingleLine(gradationContent, mainGradationTex);
                                    GradientEditor(material, mainGrad, mainGradationTex, true);
                                }
                                DrawLine();
                            }
                            if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION) || CheckFeature(shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP))
                            {
                                m_MaterialEditor.TexturePropertySingleLine(adjustMaskContent, mainColorAdjustMask);
                                TextureBakeGUI(material, 4);
                            }
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        //}

                        // Main 2nd
                        if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN2ND) && useMain2ndTex.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField(GetLoc("sMainColor2nd"), customToggleFont);
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            m_MaterialEditor.TexturePropertySingleLine(colorRGBAContent, main2ndTex, mainColor2nd);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }

                        // Main 3rd
                        if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN3RD) && useMain3rdTex.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField(GetLoc("sMainColor3rd"), customToggleFont);
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            m_MaterialEditor.TexturePropertySingleLine(colorRGBAContent, main3rdTex, mainColor3rd);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Shadow
                if((!isFakeShadow && !isGem && CheckFeature(shaderSetting.LIL_FEATURE_SHADOW)) || isLite)
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
                else if(!isFakeShadow && (CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_1ST) || CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_2ND)))
                {
                    edSet.isShowEmission = Foldout(GetLoc("sEmissionSetting"), edSet.isShowEmission);
                    DrawMenuButton(GetLoc("sAnchorEmission"), lilPropertyBlock.Emission);
                    if(edSet.isShowEmission)
                    {
                        // Emission
                        if(CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_1ST))
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            m_MaterialEditor.ShaderProperty(useEmission, GetLoc("sEmission"));
                            DrawMenuButton(GetLoc("sAnchorEmission"), lilPropertyBlock.Emission1st);
                            if(useEmission.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                TextureGUI(ref edSet.isShowEmissionMap, colorMaskRGBAContent, emissionMap, emissionColor, emissionMap_ScrollRotate, emissionMap_UVMode, true, CheckFeature(shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV));
                                if(emissionColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                {
                                    emissionColor.colorValue = new Color(emissionColor.colorValue.r, emissionColor.colorValue.g, emissionColor.colorValue.b, 1.0f);
                                }
                                DrawLine();
                                if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK))
                                {
                                    TextureGUI(ref edSet.isShowEmissionBlendMask, maskBlendContent, emissionBlendMask, emissionBlend, emissionBlendMask_ScrollRotate, true, CheckFeature(shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV));
                                    DrawLine();
                                }
                                m_MaterialEditor.ShaderProperty(emissionFluorescence, GetLoc("sFluorescence"));
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }

                        // Emission 2nd
                        if(CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_2ND))
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            m_MaterialEditor.ShaderProperty(useEmission2nd, GetLoc("sEmission2nd"));
                            DrawMenuButton(GetLoc("sAnchorEmission"), lilPropertyBlock.Emission2nd);
                            if(useEmission2nd.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                TextureGUI(ref edSet.isShowEmission2ndMap, colorMaskRGBAContent, emission2ndMap, emission2ndColor, emission2ndMap_ScrollRotate, emission2ndMap_UVMode, true, CheckFeature(shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV));
                                if(emission2ndColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                {
                                    emission2ndColor.colorValue = new Color(emission2ndColor.colorValue.r, emission2ndColor.colorValue.g, emission2ndColor.colorValue.b, 1.0f);
                                }
                                DrawLine();
                                if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK))
                                {
                                    TextureGUI(ref edSet.isShowEmission2ndBlendMask, maskBlendContent, emission2ndBlendMask, emission2ndBlend, emission2ndBlendMask_ScrollRotate, true, CheckFeature(shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV));
                                    DrawLine();
                                }
                                m_MaterialEditor.ShaderProperty(emission2ndFluorescence, GetLoc("sFluorescence"));
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Outline
                DrawOutlineSettingsSimple(material);

                if(mtoon != null && GUILayout.Button(GetLoc("sConvertMToon"))) CreateMToonMaterial(material);
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Advanced
            if(edSet.editorMode == EditorMode.Advanced)
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
                        if(GUILayout.Button("Set Writer"))
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
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, isOutl, isLite, isStWr, isTess);
                            if(renderingModeBuf == RenderingMode.Opaque) material.renderQueue += 450;
                        }
                        if(GUILayout.Button("Set Reader"))
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
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, isOutl, isLite, isStWr, isTess);
                            if(renderingModeBuf == RenderingMode.Opaque) material.renderQueue += 450;
                        }
                        if(GUILayout.Button("Reset"))
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
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, isOutl, isLite, isStWr, isTess);
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
                        if(GUILayout.Button(GetLoc("sRenderingReset")))
                        {
                            material.enableInstancing = false;
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, isOutl, isLite, isStWr, isTess);
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
                                SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, isOutl, isLite, isStWr, isTess);
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
                            if(GUILayout.Button(GetLoc("sRemoveUnused"))) RemoveUnusedTexture(material, isLite, shaderSetting);
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
                    if(shaderSetting.LIL_FEATURE_ENCRYPTION)
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
                        if(GUILayout.Button("Set Writer"))
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
                        if(GUILayout.Button("Set Reader"))
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
                        if(GUILayout.Button("Reset"))
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
                        if(GUILayout.Button(GetLoc("sRenderingReset")))
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
                            if(GUILayout.Button(GetLoc("sRemoveUnused"))) RemoveUnusedTexture(material, isLite, shaderSetting);
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
                        if(CheckFeature(shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV)) UVSettingGUI(mainTex, mainTex_ScrollRotate);
                        else                                                        m_MaterialEditor.TextureScaleOffsetProperty(mainTex);
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
                                if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION))
                                {
                                    ToneCorrectionGUI(mainTexHSVG);
                                }
                                if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP))
                                {
                                    DrawLine();
                                    EditorGUILayout.LabelField(GetLoc("sGradationMap"), boldLabel);
                                    m_MaterialEditor.ShaderProperty(mainGradationStrength, GetLoc("sStrength"));
                                    if(mainGradationStrength.floatValue != 0)
                                    {
                                        m_MaterialEditor.TexturePropertySingleLine(gradationContent, mainGradationTex);
                                        GradientEditor(material, mainGrad, mainGradationTex, true);
                                    }
                                    DrawLine();
                                }
                                if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION) || CheckFeature(shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP))
                                {
                                    m_MaterialEditor.TexturePropertySingleLine(adjustMaskContent, mainColorAdjustMask);
                                    TextureBakeGUI(material, 4);
                                }
                                EditorGUILayout.EndVertical();
                            //}
                            EditorGUILayout.EndVertical();

                            //------------------------------------------------------------------------------------------------------------------------------
                            // 2nd
                            if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN2ND))
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                m_MaterialEditor.ShaderProperty(useMain2ndTex, GetLoc("sMainColor2nd"));
                                DrawMenuButton(GetLoc("sAnchorMainColor2"), lilPropertyBlock.MainColor2nd);
                                if(useMain2ndTex.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    m_MaterialEditor.TexturePropertySingleLine(colorRGBAContent, main2ndTex, mainColor2nd);
                                    m_MaterialEditor.ShaderProperty(main2ndTexIsMSDF, GetLoc("sAsMSDF"));
                                    DrawLine();
                                    UV4Decal(main2ndTexIsDecal, main2ndTexIsLeftOnly, main2ndTexIsRightOnly, main2ndTexShouldCopy, main2ndTexShouldFlipMirror, main2ndTexShouldFlipCopy, main2ndTex, main2ndTexAngle, main2ndTexDecalAnimation, main2ndTexDecalSubParam, main2ndTex_UVMode);
                                    DrawLine();
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_LAYER_MASK)) m_MaterialEditor.TexturePropertySingleLine(maskBlendContent, main2ndBlendMask);
                                    EditorGUILayout.LabelField(GetLoc("sDistanceFade"));
                                    EditorGUI.indentLevel++;
                                    m_MaterialEditor.ShaderProperty(main2ndDistanceFade, sDistanceFadeSetting);
                                    EditorGUI.indentLevel--;
                                    m_MaterialEditor.ShaderProperty(main2ndEnableLighting, GetLoc("sEnableLighting"));
                                    m_MaterialEditor.ShaderProperty(main2ndTexBlendMode, sBlendModes);
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_LAYER_DISSOLVE))
                                    {
                                        DrawLine();
                                        m_MaterialEditor.ShaderProperty(main2ndDissolveParams, sDissolveParams);
                                        if(main2ndDissolveParams.vectorValue.x == 1.0f)                                                TextureGUI(ref edSet.isShowMain2ndDissolveMask, maskBlendContent, main2ndDissolveMask);
                                        if(main2ndDissolveParams.vectorValue.x == 2.0f && main2ndDissolveParams.vectorValue.y == 0.0f) m_MaterialEditor.ShaderProperty(main2ndDissolvePos, GetLoc("sPosition") + "|2");
                                        if(main2ndDissolveParams.vectorValue.x == 2.0f && main2ndDissolveParams.vectorValue.y == 1.0f) m_MaterialEditor.ShaderProperty(main2ndDissolvePos, GetLoc("sVector") + "|2");
                                        if(main2ndDissolveParams.vectorValue.x == 3.0f && main2ndDissolveParams.vectorValue.y == 0.0f) m_MaterialEditor.ShaderProperty(main2ndDissolvePos, GetLoc("sPosition") + "|3");
                                        if(main2ndDissolveParams.vectorValue.x == 3.0f && main2ndDissolveParams.vectorValue.y == 1.0f) m_MaterialEditor.ShaderProperty(main2ndDissolvePos, GetLoc("sVector") + "|3");
                                        if(main2ndDissolveParams.vectorValue.x != 0.0f)
                                        {
                                            if(shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE) TextureGUI(ref edSet.isShowMain2ndDissolveNoiseMask, noiseMaskContent, main2ndDissolveNoiseMask, main2ndDissolveNoiseStrength, main2ndDissolveNoiseMask_ScrollRotate);
                                            m_MaterialEditor.ShaderProperty(main2ndDissolveColor, GetLoc("sColor"));
                                        }
                                    }
                                    DrawLine();
                                    TextureBakeGUI(material, 5);
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndVertical();
                            }

                            //------------------------------------------------------------------------------------------------------------------------------
                            // 3rd
                            if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN3RD))
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                m_MaterialEditor.ShaderProperty(useMain3rdTex, GetLoc("sMainColor3rd"));
                                DrawMenuButton(GetLoc("sAnchorMainColor2"), lilPropertyBlock.MainColor3rd);
                                if(useMain3rdTex.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    m_MaterialEditor.TexturePropertySingleLine(colorRGBAContent, main3rdTex, mainColor3rd);
                                    m_MaterialEditor.ShaderProperty(main3rdTexIsMSDF, GetLoc("sAsMSDF"));
                                    DrawLine();
                                    UV4Decal(main3rdTexIsDecal, main3rdTexIsLeftOnly, main3rdTexIsRightOnly, main3rdTexShouldCopy, main3rdTexShouldFlipMirror, main3rdTexShouldFlipCopy, main3rdTex, main3rdTexAngle, main3rdTexDecalAnimation, main3rdTexDecalSubParam, main3rdTex_UVMode);
                                    DrawLine();
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_LAYER_MASK)) m_MaterialEditor.TexturePropertySingleLine(maskBlendContent, main3rdBlendMask);
                                    EditorGUILayout.LabelField(GetLoc("sDistanceFade"));
                                    EditorGUI.indentLevel++;
                                    m_MaterialEditor.ShaderProperty(main3rdDistanceFade, sDistanceFadeSetting);
                                    EditorGUI.indentLevel--;
                                    m_MaterialEditor.ShaderProperty(main3rdEnableLighting, GetLoc("sEnableLighting"));
                                    m_MaterialEditor.ShaderProperty(main3rdTexBlendMode, sBlendModes);
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_LAYER_DISSOLVE))
                                    {
                                        DrawLine();
                                        m_MaterialEditor.ShaderProperty(main3rdDissolveParams, sDissolveParams);
                                        if(main3rdDissolveParams.vectorValue.x == 1.0f)                                                TextureGUI(ref edSet.isShowMain3rdDissolveMask, maskBlendContent, main3rdDissolveMask);
                                        if(main3rdDissolveParams.vectorValue.x == 2.0f && main3rdDissolveParams.vectorValue.y == 0.0f) m_MaterialEditor.ShaderProperty(main3rdDissolvePos, GetLoc("sPosition") + "|2");
                                        if(main3rdDissolveParams.vectorValue.x == 2.0f && main3rdDissolveParams.vectorValue.y == 1.0f) m_MaterialEditor.ShaderProperty(main3rdDissolvePos, GetLoc("sVector") + "|2");
                                        if(main3rdDissolveParams.vectorValue.x == 3.0f && main3rdDissolveParams.vectorValue.y == 0.0f) m_MaterialEditor.ShaderProperty(main3rdDissolvePos, GetLoc("sPosition") + "|3");
                                        if(main3rdDissolveParams.vectorValue.x == 3.0f && main3rdDissolveParams.vectorValue.y == 1.0f) m_MaterialEditor.ShaderProperty(main3rdDissolvePos, GetLoc("sVector") + "|3");
                                        if(main3rdDissolveParams.vectorValue.x != 0.0f)
                                        {
                                            if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE)) TextureGUI(ref edSet.isShowMain3rdDissolveNoiseMask, noiseMaskContent, main3rdDissolveNoiseMask, main3rdDissolveNoiseStrength, main3rdDissolveNoiseMask_ScrollRotate);
                                            m_MaterialEditor.ShaderProperty(main3rdDissolveColor, GetLoc("sColor"));
                                        }
                                    }
                                    DrawLine();
                                    TextureBakeGUI(material, 6);
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndVertical();
                            }

                            //------------------------------------------------------------------------------------------------------------------------------
                            // Alpha Mask
                            if(CheckFeature(shaderSetting.LIL_FEATURE_ALPHAMASK))
                            {
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
                                        m_MaterialEditor.ShaderProperty(alphaMaskScale, "Scale");
                                        m_MaterialEditor.ShaderProperty(alphaMaskValue, "Offset");
                                        m_MaterialEditor.ShaderProperty(cutoff, GetLoc("sCutoff"));
                                        AlphamaskToTextureGUI(material);
                                        EditorGUILayout.EndVertical();
                                    }
                                    EditorGUILayout.EndVertical();
                                }
                            }
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Shadow
                    if(!isGem && CheckFeature(shaderSetting.LIL_FEATURE_SHADOW))
                    {
                        DrawShadowSettings();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Emission
                    if((CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_1ST) || CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_2ND)))
                    {
                        edSet.isShowEmission = Foldout(GetLoc("sEmissionSetting"), edSet.isShowEmission);
                        DrawMenuButton(GetLoc("sAnchorEmission"), lilPropertyBlock.Emission);
                        if(edSet.isShowEmission)
                        {
                            // Emission
                            if(CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_1ST))
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                m_MaterialEditor.ShaderProperty(useEmission, GetLoc("sEmission"));
                                DrawMenuButton(GetLoc("sAnchorEmission"), lilPropertyBlock.Emission1st);
                                if(useEmission.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    TextureGUI(ref edSet.isShowEmissionMap, colorMaskRGBAContent, emissionMap, emissionColor, emissionMap_ScrollRotate, emissionMap_UVMode, true, CheckFeature(shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV));
                                    if(emissionColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                    {
                                        emissionColor.colorValue = new Color(emissionColor.colorValue.r, emissionColor.colorValue.g, emissionColor.colorValue.b, 1.0f);
                                    }
                                    DrawLine();
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK))
                                    {
                                        TextureGUI(ref edSet.isShowEmissionBlendMask, maskBlendContent, emissionBlendMask, emissionBlend, emissionBlendMask_ScrollRotate, true, CheckFeature(shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV));
                                        DrawLine();
                                    }
                                    m_MaterialEditor.ShaderProperty(emissionBlink, blinkSetting);
                                    DrawLine();
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_GRADATION))
                                    {
                                        m_MaterialEditor.ShaderProperty(emissionUseGrad, GetLoc("sGradation"));
                                        if(emissionUseGrad.floatValue == 1)
                                        {
                                            m_MaterialEditor.TexturePropertySingleLine(gradSpeedContent, emissionGradTex, emissionGradSpeed);
                                            GradientEditor(material, "_eg", emiGrad, emissionGradSpeed);
                                        }
                                        DrawLine();
                                    }
                                    m_MaterialEditor.ShaderProperty(emissionParallaxDepth, GetLoc("sParallaxDepth"));
                                    m_MaterialEditor.ShaderProperty(emissionFluorescence, GetLoc("sFluorescence"));
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndVertical();
                            }

                            // Emission 2nd
                            if(CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_2ND))
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                m_MaterialEditor.ShaderProperty(useEmission2nd, GetLoc("sEmission2nd"));
                                DrawMenuButton(GetLoc("sAnchorEmission"), lilPropertyBlock.Emission2nd);
                                if(useEmission2nd.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    TextureGUI(ref edSet.isShowEmission2ndMap, colorMaskRGBAContent, emission2ndMap, emission2ndColor, emission2ndMap_ScrollRotate, emission2ndMap_UVMode, true, CheckFeature(shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV));
                                    if(emission2ndColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                    {
                                        emission2ndColor.colorValue = new Color(emission2ndColor.colorValue.r, emission2ndColor.colorValue.g, emission2ndColor.colorValue.b, 1.0f);
                                    }
                                    DrawLine();
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK))
                                    {
                                        TextureGUI(ref edSet.isShowEmission2ndBlendMask, maskBlendContent, emission2ndBlendMask, emission2ndBlend, emission2ndBlendMask_ScrollRotate, true, CheckFeature(shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV));
                                        DrawLine();
                                    }
                                    m_MaterialEditor.ShaderProperty(emission2ndBlink, blinkSetting);
                                    DrawLine();
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_GRADATION))
                                    {
                                        m_MaterialEditor.ShaderProperty(emission2ndUseGrad, GetLoc("sGradation"));
                                        if(emission2ndUseGrad.floatValue == 1)
                                        {
                                            m_MaterialEditor.TexturePropertySingleLine(gradSpeedContent, emission2ndGradTex, emission2ndGradSpeed);
                                            GradientEditor(material, "_e2g", emi2Grad, emission2ndGradSpeed);
                                        }
                                        DrawLine();
                                    }
                                    m_MaterialEditor.ShaderProperty(emission2ndParallaxDepth, GetLoc("sParallaxDepth"));
                                    m_MaterialEditor.ShaderProperty(emission2ndFluorescence, GetLoc("sFluorescence"));
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndVertical();
                            }
                        }

                        EditorGUILayout.Space();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Normal / Reflection
                    if(CheckFeature(shaderSetting.LIL_FEATURE_NORMAL_1ST) || CheckFeature(shaderSetting.LIL_FEATURE_NORMAL_2ND) || CheckFeature(shaderSetting.LIL_FEATURE_REFLECTION) || CheckFeature(shaderSetting.LIL_FEATURE_MATCAP) || CheckFeature(shaderSetting.LIL_FEATURE_MATCAP_2ND) || CheckFeature(shaderSetting.LIL_FEATURE_RIMLIGHT))
                    {
                        GUILayout.Label(GetLoc("sNormalMapReflection"), boldLabel);
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Normal
                    if(CheckFeature(shaderSetting.LIL_FEATURE_NORMAL_1ST) || CheckFeature(shaderSetting.LIL_FEATURE_NORMAL_2ND) || CheckFeature(shaderSetting.LIL_FEATURE_ANISOTROPY))
                    {
                        edSet.isShowBump = Foldout(GetLoc("sNormalMapSetting"), edSet.isShowBump);
                        DrawMenuButton(GetLoc("sAnchorNormalMap"), lilPropertyBlock.NormalMap);
                        if(edSet.isShowBump)
                        {
                            //------------------------------------------------------------------------------------------------------------------------------
                            // 1st
                            if(CheckFeature(shaderSetting.LIL_FEATURE_NORMAL_1ST))
                            {
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
                            }

                            //------------------------------------------------------------------------------------------------------------------------------
                            // 2nd
                            if(CheckFeature(shaderSetting.LIL_FEATURE_NORMAL_2ND))
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                m_MaterialEditor.ShaderProperty(useBump2ndMap, GetLoc("sNormalMap2nd"));
                                DrawMenuButton(GetLoc("sAnchorNormalMap"), lilPropertyBlock.NormalMap2nd);
                                if(useBump2ndMap.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    TextureGUI(ref edSet.isShowBump2ndMap, normalMapContent, bump2ndMap, bump2ndScale);
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK))
                                    {
                                        DrawLine();
                                        TextureGUI(ref edSet.isShowBump2ndScaleMask, maskStrengthContent, bump2ndScaleMask);
                                    }
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndVertical();
                            }

                            //------------------------------------------------------------------------------------------------------------------------------
                            // Anisotropy
                            if(CheckFeature(shaderSetting.LIL_FEATURE_ANISOTROPY))
                            {
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
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Backlight
                    if(!isGem && CheckFeature(shaderSetting.LIL_FEATURE_BACKLIGHT))
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
                                if(backlightColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                {
                                    backlightColor.colorValue = new Color(backlightColor.colorValue.r, backlightColor.colorValue.g, backlightColor.colorValue.b, 1.0f);
                                }
                                m_MaterialEditor.ShaderProperty(backlightNormalStrength, GetLoc("sNormalStrength"));
                                InvBorderGUI(backlightBorder);
                                m_MaterialEditor.ShaderProperty(backlightBlur, GetLoc("sBlur"));
                                m_MaterialEditor.ShaderProperty(backlightDirectivity, GetLoc("sDirectivity"));
                                m_MaterialEditor.ShaderProperty(backlightViewStrength, GetLoc("sViewDirectionStrength"));
                                m_MaterialEditor.ShaderProperty(backlightReceiveShadow, GetLoc("sReceiveShadow"));
                                m_MaterialEditor.ShaderProperty(backlightBackfaceMask, GetLoc("sBackfaceMask"));
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Reflection
                    if(!isGem && CheckFeature(shaderSetting.LIL_FEATURE_REFLECTION))
                    {
                        edSet.isShowReflections = Foldout(GetLoc("sReflectionsSetting"), edSet.isShowReflections);
                        DrawMenuButton(GetLoc("sAnchorReflection"), lilPropertyBlock.Reflections);
                        if(edSet.isShowReflections)
                        {
                            //------------------------------------------------------------------------------------------------------------------------------
                            // Reflection
                            if(!isGem && CheckFeature(shaderSetting.LIL_FEATURE_REFLECTION))
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                m_MaterialEditor.ShaderProperty(useReflection, GetLoc("sReflection"));
                                DrawMenuButton(GetLoc("sAnchorReflection"), lilPropertyBlock.Reflection);
                                if(useReflection.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS))   TextureGUI(ref edSet.isShowSmoothnessTex, smoothnessContent, smoothnessTex, smoothness);
                                    else                                                                    m_MaterialEditor.ShaderProperty(smoothness, GetLoc("sSmoothness"));
                                    DrawLine();
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC))     TextureGUI(ref edSet.isShowMetallicGlossMap, metallicContent, metallicGlossMap, metallic);
                                    else                                                                    m_MaterialEditor.ShaderProperty(metallic, GetLoc("sMetallic"));
                                    DrawLine();
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR))        TextureGUI(ref edSet.isShowReflectionColorTex, colorMaskRGBAContent, reflectionColorTex, reflectionColor);
                                    else                                                                    m_MaterialEditor.ShaderProperty(reflectionColor, GetLoc("sColor"));
                                    DrawLine();
                                    m_MaterialEditor.ShaderProperty(reflectance, GetLoc("sReflectance"));
                                    if(reflectionColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                    {
                                        reflectionColor.colorValue = new Color(reflectionColor.colorValue.r, reflectionColor.colorValue.g, reflectionColor.colorValue.b, 1.0f);
                                    }
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
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndVertical();
                            }
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // MatCap
                    if(CheckFeature(shaderSetting.LIL_FEATURE_MATCAP) || CheckFeature(shaderSetting.LIL_FEATURE_MATCAP_2ND))
                    {
                        edSet.isShowMatCap = Foldout(GetLoc("sMatCapSetting"), edSet.isShowMatCap);
                        DrawMenuButton(GetLoc("sAnchorMatCap"), lilPropertyBlock.MatCaps);
                        if(edSet.isShowMatCap)
                        {
                            //------------------------------------------------------------------------------------------------------------------------------
                            // MatCap
                            if(CheckFeature(shaderSetting.LIL_FEATURE_MATCAP))
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                m_MaterialEditor.ShaderProperty(useMatCap, GetLoc("sMatCap"));
                                DrawMenuButton(GetLoc("sAnchorMatCap"), lilPropertyBlock.MatCap1st);
                                if(useMatCap.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    MatCapTextureGUI(ref edSet.isShowMatCapUV, matcapContent, matcapTex, matcapColor, matcapBlendUV1, matcapZRotCancel, matcapPerspective, matcapVRParallaxStrength);
                                    if(matcapColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                    {
                                        matcapColor.colorValue = new Color(matcapColor.colorValue.r, matcapColor.colorValue.g, matcapColor.colorValue.b, 1.0f);
                                    }
                                    m_MaterialEditor.ShaderProperty(matcapNormalStrength, GetLoc("sNormalStrength"));
                                    DrawLine();
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK)) TextureGUI(ref edSet.isShowMatCapBlendMask, maskBlendContent, matcapBlendMask, matcapBlend);
                                    else                                                        m_MaterialEditor.ShaderProperty(matcapBlend, GetLoc("sBlend"));
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
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP))
                                    {
                                        DrawLine();
                                        m_MaterialEditor.ShaderProperty(matcapCustomNormal, GetLoc("sMatCapCustomNormal"));
                                        if(matcapCustomNormal.floatValue == 1)
                                        {
                                            TextureGUI(ref edSet.isShowMatCapBumpMap, normalMapContent, matcapBumpMap, matcapBumpScale);
                                        }
                                    }
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndVertical();
                            }

                            //------------------------------------------------------------------------------------------------------------------------------
                            // MatCap 2nd
                            if(CheckFeature(shaderSetting.LIL_FEATURE_MATCAP_2ND))
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                m_MaterialEditor.ShaderProperty(useMatCap2nd, GetLoc("sMatCap2nd"));
                                DrawMenuButton(GetLoc("sAnchorMatCap"), lilPropertyBlock.MatCap2nd);
                                if(useMatCap2nd.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    MatCapTextureGUI(ref edSet.isShowMatCap2ndUV, matcapContent, matcap2ndTex, matcap2ndColor, matcap2ndBlendUV1, matcap2ndZRotCancel, matcap2ndPerspective, matcap2ndVRParallaxStrength);
                                    if(matcap2ndColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                    {
                                        matcap2ndColor.colorValue = new Color(matcap2ndColor.colorValue.r, matcap2ndColor.colorValue.g, matcap2ndColor.colorValue.b, 1.0f);
                                    }
                                    m_MaterialEditor.ShaderProperty(matcap2ndNormalStrength, GetLoc("sNormalStrength"));
                                    DrawLine();
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK)) TextureGUI(ref edSet.isShowMatCap2ndBlendMask, maskBlendContent, matcap2ndBlendMask, matcap2ndBlend);
                                    else                                                        m_MaterialEditor.ShaderProperty(matcap2ndBlend, GetLoc("sBlend"));
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
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP))
                                    {
                                        DrawLine();
                                        m_MaterialEditor.ShaderProperty(matcap2ndCustomNormal, GetLoc("sMatCapCustomNormal"));
                                        if(matcap2ndCustomNormal.floatValue == 1)
                                        {
                                            TextureGUI(ref edSet.isShowMatCap2ndBumpMap, normalMapContent, matcap2ndBumpMap, matcap2ndBumpScale);
                                        }
                                    }
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndVertical();
                            }
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Rim
                    if(CheckFeature(shaderSetting.LIL_FEATURE_RIMLIGHT))
                    {
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
                                if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR))  TextureGUI(ref edSet.isShowRimColorTex, colorMaskRGBAContent, rimColorTex, rimColor);
                                else                                                            m_MaterialEditor.ShaderProperty(rimColor, GetLoc("sColor"));
                                if(rimColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                {
                                    rimColor.colorValue = new Color(rimColor.colorValue.r, rimColor.colorValue.g, rimColor.colorValue.b, 1.0f);
                                }
                                m_MaterialEditor.ShaderProperty(rimEnableLighting, GetLoc("sEnableLighting"));
                                m_MaterialEditor.ShaderProperty(rimShadowMask, GetLoc("sShadowMask"));
                                m_MaterialEditor.ShaderProperty(rimBackfaceMask, GetLoc("sBackfaceMask"));
                                if(isTransparent) m_MaterialEditor.ShaderProperty(rimApplyTransparency, GetLoc("sApplyTransparency"));
                                DrawLine();
                                if(CheckFeature(shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION))
                                {
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
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Glitter
                    if(CheckFeature(shaderSetting.LIL_FEATURE_GLITTER))
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
                                if(glitterColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                {
                                    glitterColor.colorValue = new Color(glitterColor.colorValue.r, glitterColor.colorValue.g, glitterColor.colorValue.b, 1.0f);
                                }
                                m_MaterialEditor.ShaderProperty(glitterMainStrength, GetLoc("sMainColorPower"));
                                m_MaterialEditor.ShaderProperty(glitterEnableLighting, GetLoc("sEnableLighting"));
                                m_MaterialEditor.ShaderProperty(glitterShadowMask, GetLoc("sShadowMask"));
                                m_MaterialEditor.ShaderProperty(glitterBackfaceMask, GetLoc("sBackfaceMask"));
                                if(isTransparent) m_MaterialEditor.ShaderProperty(glitterApplyTransparency, GetLoc("sApplyTransparency"));
                                DrawLine();
                                m_MaterialEditor.ShaderProperty(glitterParams1, sGlitterParams1);
                                m_MaterialEditor.ShaderProperty(glitterParams2, sGlitterParams2);
                                m_MaterialEditor.ShaderProperty(glitterVRParallaxStrength, GetLoc("sVRParallaxStrength"));
                                m_MaterialEditor.ShaderProperty(glitterNormalStrength, GetLoc("sNormalStrength"));
                                m_MaterialEditor.ShaderProperty(glitterPostContrast, GetLoc("sPostContrast"));
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }

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
                    if(CheckFeature(shaderSetting.LIL_FEATURE_PARALLAX) && !isGem)
                    {
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
                                if(isMulti) m_MaterialEditor.ShaderProperty(usePOM, "POM");
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Distance Fade
                    if(CheckFeature(shaderSetting.LIL_FEATURE_DISTANCE_FADE) && !isGem)
                    {
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
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // AudioLink
                    if(CheckFeature(shaderSetting.LIL_FEATURE_AUDIOLINK))
                    {
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
                                if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN2ND))         m_MaterialEditor.ShaderProperty(audioLink2Main2nd, GetLoc("sMainColor2nd"));
                                if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN3RD))         m_MaterialEditor.ShaderProperty(audioLink2Main3rd, GetLoc("sMainColor3rd"));
                                if(CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_1ST))
                                {
                                    m_MaterialEditor.ShaderProperty(audioLink2Emission, GetLoc("sEmission"));
                                     m_MaterialEditor.ShaderProperty(audioLink2EmissionGrad, GetLoc("sEmission") + GetLoc("sGradation"));
                                }
                                if(CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_2ND))
                                {
                                    m_MaterialEditor.ShaderProperty(audioLink2Emission2nd, GetLoc("sEmission2nd"));
                                    m_MaterialEditor.ShaderProperty(audioLink2Emission2ndGrad, GetLoc("sEmission2nd") + GetLoc("sGradation"));
                                }
                                if(CheckFeature(shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX))
                                {
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
                                }
                                EditorGUI.indentLevel--;
                                if(CheckFeature(shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL))
                                {
                                    DrawLine();
                                    m_MaterialEditor.ShaderProperty(audioLinkAsLocal, GetLoc("sAudioLinkAsLocal"));
                                    if(audioLinkAsLocal.floatValue == 1)
                                    {
                                        m_MaterialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sAudioLinkLocalMap"), ""), audioLinkLocalMap);
                                        m_MaterialEditor.ShaderProperty(audioLinkLocalMapParams, BuildParams(GetLoc("sAudioLinkLocalMapBPM"), GetLoc("sAudioLinkLocalMapNotes"), GetLoc("sOffset")));
                                    }
                                }
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Dissolve
                    if(CheckFeature(shaderSetting.LIL_FEATURE_DISSOLVE))
                    {
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
                                if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE))
                                {
                                    TextureGUI(ref edSet.isShowDissolveNoiseMask, noiseMaskContent, dissolveNoiseMask, dissolveNoiseStrength, dissolveNoiseMask_ScrollRotate);
                                }
                                m_MaterialEditor.ShaderProperty(dissolveColor, GetLoc("sColor"));
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Encryption
                    if(shaderSetting.LIL_FEATURE_ENCRYPTION)
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
                            if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL))  m_MaterialEditor.TexturePropertySingleLine(normalMapContent, furVectorTex, furVectorScale);
                            if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH))  m_MaterialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sLength"), GetLoc("sStrengthR")), furLengthMask);
                            m_MaterialEditor.ShaderProperty(furVector, BuildParams(GetLoc("sVector"), GetLoc("sLength")));
                            if(isTwoPass) m_MaterialEditor.ShaderProperty(furCutoutLength, GetLoc("sLength") + " (Cutout)");
                            m_MaterialEditor.ShaderProperty(vertexColor2FurVector, GetLoc("sVertexColor2Vector"));
                            m_MaterialEditor.ShaderProperty(furGravity, GetLoc("sGravity"));
                            m_MaterialEditor.ShaderProperty(furRandomize, GetLoc("sRandomize"));
                            DrawLine();
                            m_MaterialEditor.TexturePropertySingleLine(noiseMaskContent, furNoiseMask);
                            m_MaterialEditor.TextureScaleOffsetProperty(furNoiseMask);
                            if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_FUR_MASK))    m_MaterialEditor.TexturePropertySingleLine(customMaskContent, furMask);
                            m_MaterialEditor.ShaderProperty(furAO, GetLoc("sAO"));
                            m_MaterialEditor.ShaderProperty(furLayerNum, GetLoc("sLayerNum"));
                            MinusRangeGUI(furRootOffset, GetLoc("sRootWidth"));
                            if(CheckFeature(shaderSetting.LIL_FEATURE_FUR_COLLISION)) m_MaterialEditor.ShaderProperty(furTouchStrength, GetLoc("sTouchStrength"));
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
                        if(GUILayout.Button("Set Writer"))
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
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, isOutl, isLite, isStWr, isTess);
                            if(renderingModeBuf == RenderingMode.Opaque) material.renderQueue += 450;
                        }
                        if(GUILayout.Button("Set Reader"))
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
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, isOutl, isLite, isStWr, isTess);
                            if(renderingModeBuf == RenderingMode.Opaque) material.renderQueue += 450;
                        }
                        if(GUILayout.Button("Reset"))
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
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, isOutl, isLite, isStWr, isTess);
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
                        if(GUILayout.Button(GetLoc("sRenderingReset")))
                        {
                            material.enableInstancing = false;
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, isOutl, isLite, isStWr, isTess);
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
                                SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, isOutl, isLite, isStWr, isTess);
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
                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_TESSELLATION) && !isRefr && !isFur && !isGem && !isMulti)
                    {
                        edSet.isShowTess = Foldout(GetLoc("sTessellation"), edSet.isShowTess);
                        DrawMenuButton(GetLoc("sAnchorTessellation"), lilPropertyBlock.Tessellation);
                        if(edSet.isShowTess)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            if(isTess != EditorGUILayout.ToggleLeft(GetLoc("sTessellation"), isTess, customToggleFont))
                            {
                                SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, isOutl, isLite, isStWr, !isTess);
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
                            if(GUILayout.Button(GetLoc("sRemoveUnused"))) RemoveUnusedTexture(material, isLite, shaderSetting);
                            TextureBakeGUI(material, 0);
                            TextureBakeGUI(material, 1);
                            TextureBakeGUI(material, 2);
                            TextureBakeGUI(material, 3);
                            if(GUILayout.Button(GetLoc("sConvertLite"))) CreateLiteMaterial(material);
                            if(mtoon != null && GUILayout.Button(GetLoc("sConvertMToon"))) CreateMToonMaterial(material);
                            if(!isMulti && !isFur && !isRefr && !isGem && GUILayout.Button(GetLoc("sConvertMulti"))) CreateMultiMaterial(material);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();

                            // Bake Textures
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField(GetLoc("sBake"), customToggleFont);
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            if(!isGem && CheckFeature(shaderSetting.LIL_FEATURE_SHADOW)                         && GUILayout.Button(GetLoc("sShadow1stColor")))         AutoBakeColoredMask(material, shadowColorTex,       shadowColor,        "Shadow1stColor");
                            if(!isGem && CheckFeature(shaderSetting.LIL_FEATURE_SHADOW)                         && GUILayout.Button(GetLoc("sShadow2ndColor")))         AutoBakeColoredMask(material, shadow2ndColorTex,    shadow2ndColor,     "Shadow2ndColor");
                            if(!isGem && CheckFeature(shaderSetting.LIL_FEATURE_SHADOW)                         && GUILayout.Button(GetLoc("sShadow3rdColor")))         AutoBakeColoredMask(material, shadow3rdColorTex,    shadow3rdColor,     "Shadow3rdColor");
                            if(!isGem && CheckFeature(shaderSetting.LIL_FEATURE_REFLECTION)                     && GUILayout.Button(GetLoc("sReflection")))             AutoBakeColoredMask(material, reflectionColorTex,   reflectionColor,    "ReflectionColor");
                            if(CheckFeature(shaderSetting.LIL_FEATURE_MATCAP)                                   && GUILayout.Button(GetLoc("sMatCap")))                 AutoBakeColoredMask(material, matcapBlendMask,      matcapColor,        "MatCapColor");
                            if(CheckFeature(shaderSetting.LIL_FEATURE_MATCAP_2ND)                               && GUILayout.Button(GetLoc("sMatCap2nd")))              AutoBakeColoredMask(material, matcap2ndBlendMask,   matcap2ndColor,     "MatCap2ndColor");
                            if(CheckFeature(shaderSetting.LIL_FEATURE_RIMLIGHT)                                 && GUILayout.Button(GetLoc("sRimLight")))               AutoBakeColoredMask(material, rimColorTex,          rimColor,           "RimColor");
                            if(((!isRefr && !isFur && !isGem && !isCustomShader) || (isCustomShader && isOutl)) && GUILayout.Button(GetLoc("sSettingTexOutlineColor"))) AutoBakeColoredMask(material, outlineColorMask,     outlineColor,       "OutlineColor");
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }
                    }
                }
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Preset
            if(edSet.editorMode == EditorMode.Preset)
            {
                if(isLite)  EditorGUILayout.LabelField(GetLoc("sPresetsNotAvailable"), wrapLabel);
                else        DrawPreset();
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Shader Setting
            if(edSet.editorMode == EditorMode.Settings)
            {
                GUIStyle applyButton = new GUIStyle(GUI.skin.button);
                applyButton.normal.textColor = Color.red;
                applyButton.fontStyle = FontStyle.Bold;

                EditorGUILayout.HelpBox(GetLoc("sHelpShaderSetting"),MessageType.Info);

                EditorGUILayout.BeginVertical(customBox);
                EditorGUI.BeginChangeCheck();
                ToggleGUI(GetLoc("sSettingCancelAutoScan"), ref shaderSetting.shouldNotScan);
                ToggleGUI(GetLoc("sSettingLock"), ref shaderSetting.isLocked);
                if(EditorGUI.EndChangeCheck() || edSet.isShaderSettingChanged && GUILayout.Button(GetLoc("sSettingApply"), applyButton))
                {
                    ApplyShaderSetting(shaderSetting);
                    edSet.isShaderSettingChanged = false;
                }
                EditorGUILayout.EndVertical();

                GUI.enabled = !shaderSetting.isLocked;
                edSet.isShowShaderSetting = Foldout(GetLoc("sShaderSetting"), edSet.isShowShaderSetting);
                DrawHelpButton(GetLoc("sAnchorShaderSetting"));
                if(edSet.isShowShaderSetting)
                {
                    EditorGUILayout.BeginVertical(customBox);
                    ShaderSettingGUI();
                    EditorGUILayout.EndVertical();
                }

                edSet.isShowOptimizationSetting = Foldout(GetLoc("sSettingBuildSizeOptimization"), edSet.isShowOptimizationSetting);
                if(edSet.isShowOptimizationSetting)
                {
                    EditorGUILayout.BeginVertical(customBox);
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
                GUI.enabled = true;
            }
            else if(edSet.isShaderSettingChanged)
            {
                ApplyShaderSetting(shaderSetting);
                edSet.isShaderSettingChanged = false;
            }

            if(EditorGUI.EndChangeCheck())
            {
                if(!isMultiVariants)
                {
                    if(isMulti)
                    {
                        SetupMultiMaterial(material);
                    }
                    else
                    {
                        RemoveShaderKeywords(material);
                    }
                }
                CopyMainColorProperties();
                SaveEditorSettingTemp();
            }
	    }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Property loader
        #region
        private void LoadProperties(MaterialProperty[] props)
        {
            // Base
            invisible = FindProperty("_Invisible", props, false);
            asUnlit = FindProperty("_AsUnlit", props, false);
            cutoff = FindProperty("_Cutoff", props, false);
            subpassCutoff = FindProperty("_SubpassCutoff", props, false);
            flipNormal = FindProperty("_FlipNormal", props, false);
            shiftBackfaceUV = FindProperty("_ShiftBackfaceUV", props, false);
            backfaceForceShadow = FindProperty("_BackfaceForceShadow", props, false);

            // Lighting
            vertexLightStrength = FindProperty("_VertexLightStrength", props, false);
            lightMinLimit = FindProperty("_LightMinLimit", props, false);
            lightMaxLimit = FindProperty("_LightMaxLimit", props, false);
            beforeExposureLimit = FindProperty("_BeforeExposureLimit", props, false);
            monochromeLighting = FindProperty("_MonochromeLighting", props, false);
            alphaBoostFA = FindProperty("_AlphaBoostFA", props, false);
            lilDirectionalLightStrength = FindProperty("_lilDirectionalLightStrength", props, false);
            lightDirectionOverride = FindProperty("_LightDirectionOverride", props, false);

            // Copy
            baseColor = FindProperty("_BaseColor", props, false);
            baseMap = FindProperty("_BaseMap", props, false);
            baseColorMap = FindProperty("_BaseColorMap", props, false);

            // Rendering
            cull = FindProperty("_Cull", props, false);
            srcBlend = FindProperty("_SrcBlend", props, false);
            dstBlend = FindProperty("_DstBlend", props, false);
            srcBlendAlpha = FindProperty("_SrcBlendAlpha", props, false);
            dstBlendAlpha = FindProperty("_DstBlendAlpha", props, false);
            blendOp = FindProperty("_BlendOp", props, false);
            blendOpAlpha = FindProperty("_BlendOpAlpha", props, false);
            srcBlendFA = FindProperty("_SrcBlendFA", props, false);
            dstBlendFA = FindProperty("_DstBlendFA", props, false);
            srcBlendAlphaFA = FindProperty("_SrcBlendAlphaFA", props, false);
            dstBlendAlphaFA = FindProperty("_DstBlendAlphaFA", props, false);
            blendOpFA = FindProperty("_BlendOpFA", props, false);
            blendOpAlphaFA = FindProperty("_BlendOpAlphaFA", props, false);
            zclip = FindProperty("_ZClip", props, false);
            zwrite = FindProperty("_ZWrite", props, false);
            ztest = FindProperty("_ZTest", props, false);
            stencilRef = FindProperty("_StencilRef", props, false);
            stencilReadMask = FindProperty("_StencilReadMask", props, false);
            stencilWriteMask = FindProperty("_StencilWriteMask", props, false);
            stencilComp = FindProperty("_StencilComp", props, false);
            stencilPass = FindProperty("_StencilPass", props, false);
            stencilFail = FindProperty("_StencilFail", props, false);
            stencilZFail = FindProperty("_StencilZFail", props, false);
            offsetFactor = FindProperty("_OffsetFactor", props, false);
            offsetUnits = FindProperty("_OffsetUnits", props, false);
            colorMask = FindProperty("_ColorMask", props, false);
            alphaToMask = FindProperty("_AlphaToMask", props, false);

            // Main
            mainTex_ScrollRotate = FindProperty("_MainTex_ScrollRotate", props, false);
            mainColor = FindProperty("_Color", props, false);
            mainTex = FindProperty("_MainTex", props, false);
            mainTexHSVG = FindProperty("_MainTexHSVG", props, false);
            mainGradationStrength = FindProperty("_MainGradationStrength", props, false);
            mainGradationTex = FindProperty("_MainGradationTex", props, false);
            mainColorAdjustMask = FindProperty("_MainColorAdjustMask", props, false);

            // Main 2nd
            useMain2ndTex = FindProperty("_UseMain2ndTex", props, false);
            mainColor2nd = FindProperty("_Color2nd", props, false);
            main2ndTex = FindProperty("_Main2ndTex", props, false);
            main2ndTex_UVMode = FindProperty("_Main2ndTex_UVMode", props, false);
            main2ndTexAngle = FindProperty("_Main2ndTexAngle", props, false);
            main2ndTexDecalAnimation = FindProperty("_Main2ndTexDecalAnimation", props, false);
            main2ndTexDecalSubParam = FindProperty("_Main2ndTexDecalSubParam", props, false);
            main2ndTexIsDecal = FindProperty("_Main2ndTexIsDecal", props, false);
            main2ndTexIsLeftOnly = FindProperty("_Main2ndTexIsLeftOnly", props, false);
            main2ndTexIsRightOnly = FindProperty("_Main2ndTexIsRightOnly", props, false);
            main2ndTexShouldCopy = FindProperty("_Main2ndTexShouldCopy", props, false);
            main2ndTexShouldFlipMirror = FindProperty("_Main2ndTexShouldFlipMirror", props, false);
            main2ndTexShouldFlipCopy = FindProperty("_Main2ndTexShouldFlipCopy", props, false);
            main2ndTexIsMSDF = FindProperty("_Main2ndTexIsMSDF", props, false);
            main2ndBlendMask = FindProperty("_Main2ndBlendMask", props, false);
            main2ndTexBlendMode = FindProperty("_Main2ndTexBlendMode", props, false);
            main2ndEnableLighting = FindProperty("_Main2ndEnableLighting", props, false);
            main2ndDissolveMask = FindProperty("_Main2ndDissolveMask", props, false);
            main2ndDissolveNoiseMask = FindProperty("_Main2ndDissolveNoiseMask", props, false);
            main2ndDissolveNoiseMask_ScrollRotate = FindProperty("_Main2ndDissolveNoiseMask_ScrollRotate", props, false);
            main2ndDissolveNoiseStrength = FindProperty("_Main2ndDissolveNoiseStrength", props, false);
            main2ndDissolveColor = FindProperty("_Main2ndDissolveColor", props, false);
            main2ndDissolveParams = FindProperty("_Main2ndDissolveParams", props, false);
            main2ndDissolvePos = FindProperty("_Main2ndDissolvePos", props, false);
            main2ndDistanceFade = FindProperty("_Main2ndDistanceFade", props, false);

            // Main 3rd
            useMain3rdTex = FindProperty("_UseMain3rdTex", props, false);
            mainColor3rd = FindProperty("_Color3rd", props, false);
            main3rdTex = FindProperty("_Main3rdTex", props, false);
            main3rdTex_UVMode = FindProperty("_Main3rdTex_UVMode", props, false);
            main3rdTexAngle = FindProperty("_Main3rdTexAngle", props, false);
            main3rdTexIsDecal = FindProperty("_Main3rdTexIsDecal", props, false);
            main3rdTexDecalAnimation = FindProperty("_Main3rdTexDecalAnimation", props, false);
            main3rdTexDecalSubParam = FindProperty("_Main3rdTexDecalSubParam", props, false);
            main3rdTexIsLeftOnly = FindProperty("_Main3rdTexIsLeftOnly", props, false);
            main3rdTexIsRightOnly = FindProperty("_Main3rdTexIsRightOnly", props, false);
            main3rdTexShouldCopy = FindProperty("_Main3rdTexShouldCopy", props, false);
            main3rdTexShouldFlipMirror = FindProperty("_Main3rdTexShouldFlipMirror", props, false);
            main3rdTexShouldFlipCopy = FindProperty("_Main3rdTexShouldFlipCopy", props, false);
            main3rdTexIsMSDF = FindProperty("_Main3rdTexIsMSDF", props, false);
            main3rdBlendMask = FindProperty("_Main3rdBlendMask", props, false);
            main3rdTexBlendMode = FindProperty("_Main3rdTexBlendMode", props, false);
            main3rdEnableLighting = FindProperty("_Main3rdEnableLighting", props, false);
            main3rdDissolveMask = FindProperty("_Main3rdDissolveMask", props, false);
            main3rdDissolveNoiseMask = FindProperty("_Main3rdDissolveNoiseMask", props, false);
            main3rdDissolveNoiseMask_ScrollRotate = FindProperty("_Main3rdDissolveNoiseMask_ScrollRotate", props, false);
            main3rdDissolveNoiseStrength = FindProperty("_Main3rdDissolveNoiseStrength", props, false);
            main3rdDissolveColor = FindProperty("_Main3rdDissolveColor", props, false);
            main3rdDissolveParams = FindProperty("_Main3rdDissolveParams", props, false);
            main3rdDissolvePos = FindProperty("_Main3rdDissolvePos", props, false);
            main3rdDistanceFade = FindProperty("_Main3rdDistanceFade", props, false);

            // Alpha Mask
            alphaMaskMode = FindProperty("_AlphaMaskMode", props, false);
            alphaMask = FindProperty("_AlphaMask", props, false);
            alphaMaskScale = FindProperty("_AlphaMaskScale", props, false);
            alphaMaskValue = FindProperty("_AlphaMaskValue", props, false);

            // Shadow
            useShadow = FindProperty("_UseShadow", props, false);
            shadowStrength = FindProperty("_ShadowStrength", props, false);
            shadowStrengthMask = FindProperty("_ShadowStrengthMask", props, false);
            shadowBorderMask = FindProperty("_ShadowBorderMask", props, false);
            shadowBlurMask = FindProperty("_ShadowBlurMask", props, false);
            shadowAOShift = FindProperty("_ShadowAOShift", props, false);
            shadowAOShift2 = FindProperty("_ShadowAOShift2", props, false);
            shadowPostAO = FindProperty("_ShadowPostAO", props, false);
            shadowColor = FindProperty("_ShadowColor", props, false);
            shadowColorTex = FindProperty("_ShadowColorTex", props, false);
            shadowNormalStrength = FindProperty("_ShadowNormalStrength", props, false);
            shadowBorder = FindProperty("_ShadowBorder", props, false);
            shadowBlur = FindProperty("_ShadowBlur", props, false);
            shadow2ndColor = FindProperty("_Shadow2ndColor", props, false);
            shadow2ndColorTex = FindProperty("_Shadow2ndColorTex", props, false);
            shadow2ndNormalStrength = FindProperty("_Shadow2ndNormalStrength", props, false);
            shadow2ndBorder = FindProperty("_Shadow2ndBorder", props, false);
            shadow2ndBlur = FindProperty("_Shadow2ndBlur", props, false);
            shadow3rdColor = FindProperty("_Shadow3rdColor", props, false);
            shadow3rdColorTex = FindProperty("_Shadow3rdColorTex", props, false);
            shadow3rdNormalStrength = FindProperty("_Shadow3rdNormalStrength", props, false);
            shadow3rdBorder = FindProperty("_Shadow3rdBorder", props, false);
            shadow3rdBlur = FindProperty("_Shadow3rdBlur", props, false);
            shadowMainStrength = FindProperty("_ShadowMainStrength", props, false);
            shadowEnvStrength = FindProperty("_ShadowEnvStrength", props, false);
            shadowBorderColor = FindProperty("_ShadowBorderColor", props, false);
            shadowBorderRange = FindProperty("_ShadowBorderRange", props, false);
            shadowReceive = FindProperty("_ShadowReceive", props, false);
            
            // BackLight
            useBacklight = FindProperty("_UseBacklight", props, false);
            backlightColor = FindProperty("_BacklightColor", props, false);
            backlightColorTex = FindProperty("_BacklightColorTex", props, false);
            backlightNormalStrength = FindProperty("_BacklightNormalStrength", props, false);
            backlightBorder = FindProperty("_BacklightBorder", props, false);
            backlightBlur = FindProperty("_BacklightBlur", props, false);
            backlightDirectivity = FindProperty("_BacklightDirectivity", props, false);
            backlightViewStrength = FindProperty("_BacklightViewStrength", props, false);
            backlightReceiveShadow = FindProperty("_BacklightReceiveShadow", props, false);
            backlightBackfaceMask = FindProperty("_BacklightBackfaceMask", props, false);

            // Outline
            outlineColor = FindProperty("_OutlineColor", props, false);
            outlineTex = FindProperty("_OutlineTex", props, false);
            outlineTex_ScrollRotate = FindProperty("_OutlineTex_ScrollRotate", props, false);
            outlineTexHSVG = FindProperty("_OutlineTexHSVG", props, false);
            outlineWidth = FindProperty("_OutlineWidth", props, false);
            outlineWidthMask = FindProperty("_OutlineWidthMask", props, false);
            outlineFixWidth = FindProperty("_OutlineFixWidth", props, false);
            outlineVertexR2Width = FindProperty("_OutlineVertexR2Width", props, false);
            outlineVectorTex = FindProperty("_OutlineVectorTex", props, false);
            outlineVectorScale = FindProperty("_OutlineVectorScale", props, false);
            outlineEnableLighting = FindProperty("_OutlineEnableLighting", props, false);
            outlineCull = FindProperty("_OutlineCull", props, false);
            outlineSrcBlend = FindProperty("_OutlineSrcBlend", props, false);
            outlineDstBlend = FindProperty("_OutlineDstBlend", props, false);
            outlineSrcBlendAlpha = FindProperty("_OutlineSrcBlendAlpha", props, false);
            outlineDstBlendAlpha = FindProperty("_OutlineDstBlendAlpha", props, false);
            outlineBlendOp = FindProperty("_OutlineBlendOp", props, false);
            outlineBlendOpAlpha = FindProperty("_OutlineBlendOpAlpha", props, false);
            outlineSrcBlendFA = FindProperty("_OutlineSrcBlendFA", props, false);
            outlineDstBlendFA = FindProperty("_OutlineDstBlendFA", props, false);
            outlineSrcBlendAlphaFA = FindProperty("_OutlineSrcBlendAlphaFA", props, false);
            outlineDstBlendAlphaFA = FindProperty("_OutlineDstBlendAlphaFA", props, false);
            outlineBlendOpFA = FindProperty("_OutlineBlendOpFA", props, false);
            outlineBlendOpAlphaFA = FindProperty("_OutlineBlendOpAlphaFA", props, false);
            outlineZclip = FindProperty("_OutlineZClip", props, false);
            outlineZwrite = FindProperty("_OutlineZWrite", props, false);
            outlineZtest = FindProperty("_OutlineZTest", props, false);
            outlineStencilRef = FindProperty("_OutlineStencilRef", props, false);
            outlineStencilReadMask = FindProperty("_OutlineStencilReadMask", props, false);
            outlineStencilWriteMask = FindProperty("_OutlineStencilWriteMask", props, false);
            outlineStencilComp = FindProperty("_OutlineStencilComp", props, false);
            outlineStencilPass = FindProperty("_OutlineStencilPass", props, false);
            outlineStencilFail = FindProperty("_OutlineStencilFail", props, false);
            outlineStencilZFail = FindProperty("_OutlineStencilZFail", props, false);
            outlineOffsetFactor = FindProperty("_OutlineOffsetFactor", props, false);
            outlineOffsetUnits = FindProperty("_OutlineOffsetUnits", props, false);
            outlineColorMask = FindProperty("_OutlineColorMask", props, false);
            outlineAlphaToMask = FindProperty("_OutlineAlphaToMask", props, false);

            // Normal Map
            useBumpMap = FindProperty("_UseBumpMap", props, false);
            bumpMap = FindProperty("_BumpMap", props, false);
            bumpScale = FindProperty("_BumpScale", props, false);

            // Normal Map 2nd
            useBump2ndMap = FindProperty("_UseBump2ndMap", props, false);
            bump2ndMap = FindProperty("_Bump2ndMap", props, false);
            bump2ndScale = FindProperty("_Bump2ndScale", props, false);
            bump2ndScaleMask = FindProperty("_Bump2ndScaleMask", props, false);
            
            // Anisotropy
            useAnisotropy = FindProperty("_UseAnisotropy", props, false);
            anisotropyTangentMap = FindProperty("_AnisotropyTangentMap", props, false);
            anisotropyScale = FindProperty("_AnisotropyScale", props, false);
            anisotropyScaleMask = FindProperty("_AnisotropyScaleMask", props, false);
            anisotropyTangentWidth = FindProperty("_AnisotropyTangentWidth", props, false);
            anisotropyBitangentWidth = FindProperty("_AnisotropyBitangentWidth", props, false);
            anisotropyShift = FindProperty("_AnisotropyShift", props, false);
            anisotropyShiftNoiseScale = FindProperty("_AnisotropyShiftNoiseScale", props, false);
            anisotropySpecularStrength = FindProperty("_AnisotropySpecularStrength", props, false);
            anisotropy2ndTangentWidth = FindProperty("_Anisotropy2ndTangentWidth", props, false);
            anisotropy2ndBitangentWidth = FindProperty("_Anisotropy2ndBitangentWidth", props, false);
            anisotropy2ndShift = FindProperty("_Anisotropy2ndShift", props, false);
            anisotropy2ndShiftNoiseScale = FindProperty("_Anisotropy2ndShiftNoiseScale", props, false);
            anisotropy2ndSpecularStrength = FindProperty("_Anisotropy2ndSpecularStrength", props, false);
            anisotropyShiftNoiseMask = FindProperty("_AnisotropyShiftNoiseMask", props, false);
            anisotropy2Reflection = FindProperty("_Anisotropy2Reflection", props, false);
            anisotropy2MatCap = FindProperty("_Anisotropy2MatCap", props, false);
            anisotropy2MatCap2nd = FindProperty("_Anisotropy2MatCap2nd", props, false);
            
            // Reflection
            useReflection = FindProperty("_UseReflection", props, false);
            smoothness = FindProperty("_Smoothness", props, false);
            smoothnessTex = FindProperty("_SmoothnessTex", props, false);
            metallic = FindProperty("_Metallic", props, false);
            metallicGlossMap = FindProperty("_MetallicGlossMap", props, false);
            reflectance = FindProperty("_Reflectance", props, false);
            reflectionColor = FindProperty("_ReflectionColor", props, false);
            reflectionColorTex = FindProperty("_ReflectionColorTex", props, false);
            applySpecular = FindProperty("_ApplySpecular", props, false);
            applySpecularFA = FindProperty("_ApplySpecularFA", props, false);
            specularNormalStrength = FindProperty("_SpecularNormalStrength", props, false);
            specularToon = FindProperty("_SpecularToon", props, false);
            specularBorder = FindProperty("_SpecularBorder", props, false);
            specularBlur = FindProperty("_SpecularBlur", props, false);
            applyReflection = FindProperty("_ApplyReflection", props, false);
            reflectionNormalStrength = FindProperty("_ReflectionNormalStrength", props, false);
            reflectionApplyTransparency = FindProperty("_ReflectionApplyTransparency", props, false);
            reflectionCubeTex = FindProperty("_ReflectionCubeTex", props, false);
            reflectionCubeColor = FindProperty("_ReflectionCubeColor", props, false);
            reflectionCubeOverride = FindProperty("_ReflectionCubeOverride", props, false);
            reflectionCubeEnableLighting = FindProperty("_ReflectionCubeEnableLighting", props, false);
            
            // MatCap
            useMatCap = FindProperty("_UseMatCap", props, false);
            matcapTex = FindProperty("_MatCapTex", props, false);
            matcapColor = FindProperty("_MatCapColor", props, false);
            matcapBlendUV1 = FindProperty("_MatCapBlendUV1", props, false);
            matcapZRotCancel = FindProperty("_MatCapZRotCancel", props, false);
            matcapPerspective = FindProperty("_MatCapPerspective", props, false);
            matcapVRParallaxStrength = FindProperty("_MatCapVRParallaxStrength", props, false);
            matcapBlend = FindProperty("_MatCapBlend", props, false);
            matcapBlendMask = FindProperty("_MatCapBlendMask", props, false);
            matcapEnableLighting = FindProperty("_MatCapEnableLighting", props, false);
            matcapShadowMask = FindProperty("_MatCapShadowMask", props, false);
            matcapBackfaceMask = FindProperty("_MatCapBackfaceMask", props, false);
            matcapLod = FindProperty("_MatCapLod", props, false);
            matcapBlendMode = FindProperty("_MatCapBlendMode", props, false);
            matcapApplyTransparency = FindProperty("_MatCapApplyTransparency", props, false);
            matcapNormalStrength = FindProperty("_MatCapNormalStrength", props, false);
            matcapCustomNormal = FindProperty("_MatCapCustomNormal", props, false);
            matcapBumpMap = FindProperty("_MatCapBumpMap", props, false);
            matcapBumpScale = FindProperty("_MatCapBumpScale", props, false);
            
            // MatCap 2nd
            useMatCap2nd = FindProperty("_UseMatCap2nd", props, false);
            matcap2ndTex = FindProperty("_MatCap2ndTex", props, false);
            matcap2ndColor = FindProperty("_MatCap2ndColor", props, false);
            matcap2ndBlendUV1 = FindProperty("_MatCap2ndBlendUV1", props, false);
            matcap2ndZRotCancel = FindProperty("_MatCap2ndZRotCancel", props, false);
            matcap2ndPerspective = FindProperty("_MatCap2ndPerspective", props, false);
            matcap2ndVRParallaxStrength = FindProperty("_MatCap2ndVRParallaxStrength", props, false);
            matcap2ndBlend = FindProperty("_MatCap2ndBlend", props, false);
            matcap2ndBlendMask = FindProperty("_MatCap2ndBlendMask", props, false);
            matcap2ndEnableLighting = FindProperty("_MatCap2ndEnableLighting", props, false);
            matcap2ndShadowMask = FindProperty("_MatCap2ndShadowMask", props, false);
            matcap2ndBackfaceMask = FindProperty("_MatCap2ndBackfaceMask", props, false);
            matcap2ndLod = FindProperty("_MatCap2ndLod", props, false);
            matcap2ndBlendMode = FindProperty("_MatCap2ndBlendMode", props, false);
            matcap2ndApplyTransparency = FindProperty("_MatCap2ndApplyTransparency", props, false);
            matcap2ndNormalStrength = FindProperty("_MatCap2ndNormalStrength", props, false);
            matcap2ndCustomNormal = FindProperty("_MatCap2ndCustomNormal", props, false);
            matcap2ndBumpMap = FindProperty("_MatCap2ndBumpMap", props, false);
            matcap2ndBumpScale = FindProperty("_MatCap2ndBumpScale", props, false);
            
            // Rim
            useRim = FindProperty("_UseRim", props, false);
            rimColor = FindProperty("_RimColor", props, false);
            rimColorTex = FindProperty("_RimColorTex", props, false);
            rimNormalStrength = FindProperty("_RimNormalStrength", props, false);
            rimBorder = FindProperty("_RimBorder", props, false);
            rimBlur = FindProperty("_RimBlur", props, false);
            rimFresnelPower = FindProperty("_RimFresnelPower", props, false);
            rimEnableLighting = FindProperty("_RimEnableLighting", props, false);
            rimShadowMask = FindProperty("_RimShadowMask", props, false);
            rimBackfaceMask = FindProperty("_RimBackfaceMask", props, false);
            rimVRParallaxStrength = FindProperty("_RimVRParallaxStrength", props, false);
            rimApplyTransparency = FindProperty("_RimApplyTransparency", props, false);
            rimDirStrength = FindProperty("_RimDirStrength", props, false);
            rimDirRange = FindProperty("_RimDirRange", props, false);
            rimIndirRange = FindProperty("_RimIndirRange", props, false);
            rimIndirColor = FindProperty("_RimIndirColor", props, false);
            rimIndirBorder = FindProperty("_RimIndirBorder", props, false);
            rimIndirBlur = FindProperty("_RimIndirBlur", props, false);

            // Glitter
            useGlitter = FindProperty("_UseGlitter", props, false);
            glitterUVMode = FindProperty("_GlitterUVMode", props, false);
            glitterColor = FindProperty("_GlitterColor", props, false);
            glitterColorTex = FindProperty("_GlitterColorTex", props, false);
            glitterMainStrength = FindProperty("_GlitterMainStrength", props, false);
            glitterEnableLighting = FindProperty("_GlitterEnableLighting", props, false);
            glitterShadowMask = FindProperty("_GlitterShadowMask", props, false);
            glitterBackfaceMask = FindProperty("_GlitterBackfaceMask", props, false);
            glitterApplyTransparency = FindProperty("_GlitterApplyTransparency", props, false);
            glitterParams1 = FindProperty("_GlitterParams1", props, false);
            glitterParams2 = FindProperty("_GlitterParams2", props, false);
            glitterPostContrast = FindProperty("_GlitterPostContrast", props, false);
            glitterVRParallaxStrength = FindProperty("_GlitterVRParallaxStrength", props, false);
            glitterNormalStrength = FindProperty("_GlitterNormalStrength", props, false);

            // Emission
            useEmission = FindProperty("_UseEmission", props, false);
            emissionColor = FindProperty("_EmissionColor", props, false);
            emissionMap = FindProperty("_EmissionMap", props, false);
            emissionMap_ScrollRotate = FindProperty("_EmissionMap_ScrollRotate", props, false);
            emissionMap_UVMode = FindProperty("_EmissionMap_UVMode", props, false);
            emissionBlend = FindProperty("_EmissionBlend", props, false);
            emissionBlendMask = FindProperty("_EmissionBlendMask", props, false);
            emissionBlendMask_ScrollRotate = FindProperty("_EmissionBlendMask_ScrollRotate", props, false);
            emissionBlink = FindProperty("_EmissionBlink", props, false);
            emissionUseGrad = FindProperty("_EmissionUseGrad", props, false);
            emissionGradTex = FindProperty("_EmissionGradTex", props, false);
            emissionGradSpeed = FindProperty("_EmissionGradSpeed", props, false);
            emissionParallaxDepth = FindProperty("_EmissionParallaxDepth", props, false);
            emissionFluorescence = FindProperty("_EmissionFluorescence", props, false);

            // Emission 2nd
            useEmission2nd = FindProperty("_UseEmission2nd", props, false);
            emission2ndColor = FindProperty("_Emission2ndColor", props, false);
            emission2ndMap = FindProperty("_Emission2ndMap", props, false);
            emission2ndMap_ScrollRotate = FindProperty("_Emission2ndMap_ScrollRotate", props, false);
            emission2ndMap_UVMode = FindProperty("_Emission2ndMap_UVMode", props, false);
            emission2ndBlend = FindProperty("_Emission2ndBlend", props, false);
            emission2ndBlendMask = FindProperty("_Emission2ndBlendMask", props, false);
            emission2ndBlendMask_ScrollRotate = FindProperty("_Emission2ndBlendMask_ScrollRotate", props, false);
            emission2ndBlink = FindProperty("_Emission2ndBlink", props, false);
            emission2ndUseGrad = FindProperty("_Emission2ndUseGrad", props, false);
            emission2ndGradTex = FindProperty("_Emission2ndGradTex", props, false);
            emission2ndGradSpeed = FindProperty("_Emission2ndGradSpeed", props, false);
            emission2ndParallaxDepth = FindProperty("_Emission2ndParallaxDepth", props, false);
            emission2ndFluorescence = FindProperty("_Emission2ndFluorescence", props, false);
            
            // Parallax
            useParallax = FindProperty("_UseParallax", props, false);
            parallaxMap = FindProperty("_ParallaxMap", props, false);
            parallax = FindProperty("_Parallax", props, false);
            parallaxOffset = FindProperty("_ParallaxOffset", props, false);

            // Distance Fade
            distanceFade = FindProperty("_DistanceFade", props, false);
                distanceFadeColor = FindProperty("_DistanceFadeColor", props, false);
            
            // AudioLink
            useAudioLink = FindProperty("_UseAudioLink", props, false);
            audioLinkDefaultValue = FindProperty("_AudioLinkDefaultValue", props, false);
            audioLinkUVMode = FindProperty("_AudioLinkUVMode", props, false);
            audioLinkUVParams = FindProperty("_AudioLinkUVParams", props, false);
            audioLinkStart = FindProperty("_AudioLinkStart", props, false);
            audioLinkMask = FindProperty("_AudioLinkMask", props, false);
            audioLink2Main2nd = FindProperty("_AudioLink2Main2nd", props, false);
            audioLink2Main3rd = FindProperty("_AudioLink2Main3rd", props, false);
            audioLink2Emission = FindProperty("_AudioLink2Emission", props, false);
            audioLink2EmissionGrad = FindProperty("_AudioLink2EmissionGrad", props, false);
            audioLink2Emission2nd = FindProperty("_AudioLink2Emission2nd", props, false);
            audioLink2Emission2ndGrad = FindProperty("_AudioLink2Emission2ndGrad", props, false);
            audioLink2Vertex = FindProperty("_AudioLink2Vertex", props, false);
            audioLinkVertexUVMode = FindProperty("_AudioLinkVertexUVMode", props, false);
            audioLinkVertexUVParams = FindProperty("_AudioLinkVertexUVParams", props, false);
            audioLinkVertexStart = FindProperty("_AudioLinkVertexStart", props, false);
            audioLinkVertexStrength = FindProperty("_AudioLinkVertexStrength", props, false);
            audioLinkAsLocal = FindProperty("_AudioLinkAsLocal", props, false);
            audioLinkLocalMap = FindProperty("_AudioLinkLocalMap", props, false);
            audioLinkLocalMapParams = FindProperty("_AudioLinkLocalMapParams", props, false);

            // Dissolve
            dissolveMask = FindProperty("_DissolveMask", props, false);
            dissolveNoiseMask = FindProperty("_DissolveNoiseMask", props, false);
            dissolveNoiseMask_ScrollRotate = FindProperty("_DissolveNoiseMask_ScrollRotate", props, false);
            dissolveNoiseStrength = FindProperty("_DissolveNoiseStrength", props, false);
            dissolveColor = FindProperty("_DissolveColor", props, false);
            dissolveParams = FindProperty("_DissolveParams", props, false);
            dissolvePos = FindProperty("_DissolvePos", props, false);

            // Encryption
            ignoreEncryption = FindProperty("_IgnoreEncryption", props, false);
            keys = FindProperty("_Keys", props, false);

            // Fur
            furNoiseMask = FindProperty("_FurNoiseMask", props, false);
            furLengthMask = FindProperty("_FurLengthMask", props, false);
            furMask = FindProperty("_FurMask", props, false);
            furVectorTex = FindProperty("_FurVectorTex", props, false);
            furVectorScale = FindProperty("_FurVectorScale", props, false);
            furVector = FindProperty("_FurVector", props, false);
            furGravity = FindProperty("_FurGravity", props, false);
            furRandomize = FindProperty("_FurRandomize", props, false);
            furAO = FindProperty("_FurAO", props, false);
            vertexColor2FurVector = FindProperty("_VertexColor2FurVector", props, false);
            furLayerNum = FindProperty("_FurLayerNum", props, false);
            furRootOffset = FindProperty("_FurRootOffset", props, false);
            furCutoutLength = FindProperty("_FurCutoutLength", props, false);
            furTouchStrength = FindProperty("_FurTouchStrength", props, false);
            furCull = FindProperty("_FurCull", props, false);
            furSrcBlend = FindProperty("_FurSrcBlend", props, false);
            furDstBlend = FindProperty("_FurDstBlend", props, false);
            furSrcBlendAlpha = FindProperty("_FurSrcBlendAlpha", props, false);
            furDstBlendAlpha = FindProperty("_FurDstBlendAlpha", props, false);
            furBlendOp = FindProperty("_FurBlendOp", props, false);
            furBlendOpAlpha = FindProperty("_FurBlendOpAlpha", props, false);
            furSrcBlendFA = FindProperty("_FurSrcBlendFA", props, false);
            furDstBlendFA = FindProperty("_FurDstBlendFA", props, false);
            furSrcBlendAlphaFA = FindProperty("_FurSrcBlendAlphaFA", props, false);
            furDstBlendAlphaFA = FindProperty("_FurDstBlendAlphaFA", props, false);
            furBlendOpFA = FindProperty("_FurBlendOpFA", props, false);
            furBlendOpAlphaFA = FindProperty("_FurBlendOpAlphaFA", props, false);
            furZclip = FindProperty("_FurZClip", props, false);
            furZwrite = FindProperty("_FurZWrite", props, false);
            furZtest = FindProperty("_FurZTest", props, false);
            furStencilRef = FindProperty("_FurStencilRef", props, false);
            furStencilReadMask = FindProperty("_FurStencilReadMask", props, false);
            furStencilWriteMask = FindProperty("_FurStencilWriteMask", props, false);
            furStencilComp = FindProperty("_FurStencilComp", props, false);
            furStencilPass = FindProperty("_FurStencilPass", props, false);
            furStencilFail = FindProperty("_FurStencilFail", props, false);
            furStencilZFail = FindProperty("_FurStencilZFail", props, false);
            furOffsetFactor = FindProperty("_FurOffsetFactor", props, false);
            furOffsetUnits = FindProperty("_FurOffsetUnits", props, false);
            furColorMask = FindProperty("_FurColorMask", props, false);
            furAlphaToMask = FindProperty("_FurAlphaToMask", props, false);

            // Refraction
            refractionStrength = FindProperty("_RefractionStrength", props, false);
            refractionFresnelPower = FindProperty("_RefractionFresnelPower", props, false);
            refractionColorFromMain = FindProperty("_RefractionColorFromMain", props, false);
            refractionColor = FindProperty("_RefractionColor", props, false);

            // Gem
            gemChromaticAberration = FindProperty("_GemChromaticAberration", props, false);
            gemEnvContrast = FindProperty("_GemEnvContrast", props, false);
            gemEnvColor = FindProperty("_GemEnvColor", props, false);
            gemParticleLoop = FindProperty("_GemParticleLoop", props, false);
            gemParticleColor = FindProperty("_GemParticleColor", props, false);
            gemVRParallaxStrength = FindProperty("_GemVRParallaxStrength", props, false);

            // FakeShadow
            fakeShadowVector = FindProperty("_FakeShadowVector", props, false);

            // Tessellation
            tessEdge = FindProperty("_TessEdge", props, false);
            tessStrength = FindProperty("_TessStrength", props, false);
            tessShrink = FindProperty("_TessShrink", props, false);
            tessFactorMax = FindProperty("_TessFactorMax", props, false);

            // Multi
            transparentModeMat = FindProperty("_TransparentMode", props, false);
            usePOM = FindProperty("_UsePOM", props, false);
            useClippingCanceller = FindProperty("_UseClippingCanceller", props, false);
            asOverlay = FindProperty("_AsOverlay", props, false);

            // Lite
            triMask = FindProperty("_TriMask", props, false);
            matcapMul = FindProperty("_MatCapMul", props, false);
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
        public static string GetLoc(string value)
        {
            return loc.ContainsKey(value) ? loc[value] : value;
        }

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
            string langPath = AssetDatabase.GUIDToAssetPath(langFileGUID);
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
            // Render Pipeline
            // BRP : null
            // LWRP : LightweightPipeline.LightweightRenderPipelineAsset
            // URP : Universal.UniversalRenderPipelineAsset
            // HDRP : HighDefinition.HDRenderPipelineAsset
            lilRenderPipeline lilRP = CheckRP();
            Array.ForEach(shaderGuids, shaderGuid => RewriteShaderRP(AssetDatabase.GUIDToAssetPath(shaderGuid), lilRP));
            RewriteShaderRP(GetShaderPipelinePath(), lilRP);
        }

        public static lilRenderPipeline CheckRP()
        {
            // Render Pipeline
            // BRP : null
            // LWRP : LightweightPipeline.LightweightRenderPipelineAsset
            // URP : Universal.UniversalRenderPipelineAsset
            // HDRP : HighDefinition.HDRenderPipelineAsset
            lilRenderPipeline lilRP = lilRenderPipeline.BRP;
            if(UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null)
            {
                string renderPipelineName = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset.ToString();
                if(string.IsNullOrEmpty(renderPipelineName))            lilRP = lilRenderPipeline.BRP;
                else if(renderPipelineName.Contains("Lightweight"))     lilRP = lilRenderPipeline.LWRP;
                else if(renderPipelineName.Contains("Universal"))       lilRP = lilRenderPipeline.URP;
                else if(renderPipelineName.Contains("HDRenderPipeline"))  lilRP = lilRenderPipeline.HDRP;
            }
            else
            {
                lilRP = lilRenderPipeline.BRP;
            }
            return lilRP;
        }

        private static void RewriteShaderRP(string shaderPath, lilRenderPipeline lilRP)
        {
            string path = shaderPath;
            StreamReader sr = new StreamReader(path);
            string s = sr.ReadToEnd();
            sr.Close();
            RewriteBRP(ref s, lilRP == lilRenderPipeline.BRP);
            RewriteLWRP(ref s, lilRP == lilRenderPipeline.LWRP);
            RewriteURP(ref s, lilRP == lilRenderPipeline.URP);
            RewriteHDRP(ref s, lilRP == lilRenderPipeline.HDRP);
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
                string settingFolderPath = GetSettingFolderPath();
                if(!Directory.Exists(settingFolderPath)) Directory.CreateDirectory(settingFolderPath);
                shaderSetting = ScriptableObject.CreateInstance<lilToonSetting>();
                AssetDatabase.CreateAsset(shaderSetting, shaderSettingPath);
                shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV = false;
                shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION = false;
                shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP = false;
                shaderSetting.LIL_FEATURE_MAIN2ND = false;
                shaderSetting.LIL_FEATURE_MAIN3RD = false;
                shaderSetting.LIL_FEATURE_DECAL = false;
                shaderSetting.LIL_FEATURE_ANIMATE_DECAL = false;
                shaderSetting.LIL_FEATURE_LAYER_DISSOLVE = false;
                shaderSetting.LIL_FEATURE_ALPHAMASK = false;
                shaderSetting.LIL_FEATURE_SHADOW = true;
                shaderSetting.LIL_FEATURE_SHADOW_3RD = false;
                shaderSetting.LIL_FEATURE_RECEIVE_SHADOW = false;
                shaderSetting.LIL_FEATURE_EMISSION_1ST = true;
                shaderSetting.LIL_FEATURE_EMISSION_2ND = false;
                shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV = false;
                shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV = false;
                shaderSetting.LIL_FEATURE_EMISSION_GRADATION = false;
                shaderSetting.LIL_FEATURE_NORMAL_1ST = true;
                shaderSetting.LIL_FEATURE_NORMAL_2ND = false;
                shaderSetting.LIL_FEATURE_ANISOTROPY = false;
                shaderSetting.LIL_FEATURE_REFLECTION = false;
                shaderSetting.LIL_FEATURE_MATCAP = true;
                shaderSetting.LIL_FEATURE_MATCAP_2ND = false;
                shaderSetting.LIL_FEATURE_RIMLIGHT = true;
                shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION = false;
                shaderSetting.LIL_FEATURE_GLITTER = false;
                shaderSetting.LIL_FEATURE_BACKLIGHT = false;
                shaderSetting.LIL_FEATURE_PARALLAX = false;
                shaderSetting.LIL_FEATURE_POM = false;
                shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER = false;
                shaderSetting.LIL_FEATURE_DISTANCE_FADE = false;
                shaderSetting.LIL_FEATURE_AUDIOLINK = false;
                shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX = false;
                shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL = false;
                shaderSetting.LIL_FEATURE_DISSOLVE = false;
                shaderSetting.LIL_FEATURE_ENCRYPTION = false;
                shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV = false;
                shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION = false;
                shaderSetting.LIL_FEATURE_FUR_COLLISION = false;
                shaderSetting.LIL_FEATURE_TEX_LAYER_MASK = false;
                shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE = false;
                shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR = false;
                shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER = false;
                shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH = true;
                shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST = false;
                shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND = false;
                shaderSetting.LIL_FEATURE_TEX_SHADOW_3RD = false;
                shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK = false;
                shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK = false;
                shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS = false;
                shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC = false;
                shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR = false;
                shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK = true;
                shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP = false;
                shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR = true;
                shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE = false;
                shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR = true;
                shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH = true;
                shaderSetting.LIL_FEATURE_TEX_OUTLINE_NORMAL = false;
                shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL = false;
                shaderSetting.LIL_FEATURE_TEX_FUR_MASK = false;
                shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH = false;
                shaderSetting.LIL_FEATURE_TEX_TESSELLATION = false;
                shaderSetting.isLocked = false;
                shaderSetting.shouldNotScan = false;
                EditorUtility.SetDirty(shaderSetting);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        public static void TurnOffAllShaderSetting(ref lilToonSetting shaderSetting)
        {
            if(shaderSetting == null || shaderSetting.isLocked) return;
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
            shaderSetting.LIL_FEATURE_ENCRYPTION = false;
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

        public static void InitializeSettingHLSL(ref lilToonSetting shaderSetting)
        {
            string shaderSettingHLSLPath = GetShaderSettingHLSLPath();
            string shaderSettingString = "";
            if(File.Exists(shaderSettingHLSLPath))
            {
                StreamReader srSetting = new StreamReader(shaderSettingHLSLPath);
                shaderSettingString = srSetting.ReadToEnd();
                srSetting.Close();
            }

            if(shaderSettingString.Contains("//INITIALIZE"))
            {
                // Rendering pipeline
                RewriteShaderRP();

                // Get materials
                foreach(string guid in AssetDatabase.FindAssets("t:material"))
                {
                    SetupShaderSettingFromMaterial(AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid)), ref shaderSetting);
                }

                // Get animations
                foreach(string guid in AssetDatabase.FindAssets("t:animationclip"))
                {
                    SetupShaderSettingFromAnimationClip(AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(guid)), ref shaderSetting);
                }

                if(shaderSetting != null)
                {
                    EditorUtility.SetDirty(shaderSetting);
                    AssetDatabase.SaveAssets();
                    ApplyShaderSetting(shaderSetting);
                }

                AssetDatabase.ImportAsset(shaderSettingHLSLPath);
                ReimportPassShaders();
                AssetDatabase.Refresh();
            }
        }

        public static void ApplyShaderSetting(lilToonSetting shaderSetting)
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
                StreamWriter sw = new StreamWriter(shaderSettingHLSLPath,false);
                sw.Write(shaderSettingString);
                sw.Close();
                lilToonInspector.isUPM = lilToonInspector.GetEditorPath().Contains("Packages");
                string[] shaderFolderPaths = GetShaderFolderPaths();
                bool isShadowReceive = (shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW) || shaderSetting.LIL_FEATURE_BACKLIGHT;
                foreach(string shaderGuid in AssetDatabase.FindAssets("t:shader", shaderFolderPaths))
                {
                    string shaderPath = AssetDatabase.GUIDToAssetPath(shaderGuid);
                    RewriteSettingPath(shaderPath);
                    RewriteReceiveShadow(shaderPath, isShadowReceive);
                    RewriteForwardAdd(shaderPath, shaderSetting.LIL_OPTIMIZE_USE_FORWARDADD);
                    RewriteVertexLight(shaderPath, shaderSetting.LIL_OPTIMIZE_USE_VERTEXLIGHT);
                    RewriteLightmap(shaderPath, shaderSetting.LIL_OPTIMIZE_USE_LIGHTMAP);
                }
                foreach(string shaderGuid in AssetDatabase.FindAssets("t:shader"))
                {
                    string shaderPath = AssetDatabase.GUIDToAssetPath(shaderGuid);
                    if(!shaderPath.Contains(".lilcontainer")) continue;
                    AssetDatabase.ImportAsset(shaderPath);
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(shaderSettingHLSLPath);
                AssetDatabase.Refresh();
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
            return BuildShaderSettingString(shaderSetting, isFile);
        }

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

        public static void CopyShaderSetting(ref lilToonSetting ssA, lilToonSetting ssB)
        {
            if(ssA == null || ssB == null) return;

            foreach(FieldInfo field in typeof(lilToonSetting).GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                field.SetValue(ssA, field.GetValue(ssB));
            }
        }

        private static void ShaderSettingGUI()
        {
            EditorGUI.BeginChangeCheck();

            ToggleGUI(GetLoc("sSettingAnimateMainUV"), ref shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV);
            DrawLine();

            ToggleGUI(GetLoc("sSettingMainToneCorrection"), ref shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION);
            ToggleGUI(GetLoc("sSettingMainGradationMap"), ref shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP);
            ToggleGUI(GetLoc("sSettingMain2nd"), ref shaderSetting.LIL_FEATURE_MAIN2ND);
            ToggleGUI(GetLoc("sSettingMain3rd"), ref shaderSetting.LIL_FEATURE_MAIN3RD);
            if(shaderSetting.LIL_FEATURE_MAIN2ND || shaderSetting.LIL_FEATURE_MAIN3RD)
            {
                EditorGUI.indentLevel++;
                ToggleGUI(GetLoc("sSettingDecal"), ref shaderSetting.LIL_FEATURE_DECAL);
                ToggleGUI(GetLoc("sSettingAnimateDecal"), ref shaderSetting.LIL_FEATURE_ANIMATE_DECAL);
                ToggleGUI(GetLoc("sSettingTexLayerMask"), ref shaderSetting.LIL_FEATURE_TEX_LAYER_MASK);
                ToggleGUI(GetLoc("sSettingLayerDissolve"), ref shaderSetting.LIL_FEATURE_LAYER_DISSOLVE);
                if(shaderSetting.LIL_FEATURE_LAYER_DISSOLVE)
                {
                    EditorGUI.indentLevel++;
                    ToggleGUI(GetLoc("sSettingTexDissolveNoise"), ref shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
            DrawLine();

            ToggleGUI(GetLoc("sSettingAlphaMask"), ref shaderSetting.LIL_FEATURE_ALPHAMASK);
            DrawLine();

            ToggleGUI(GetLoc("sSettingShadow"), ref shaderSetting.LIL_FEATURE_SHADOW);
            if(shaderSetting.LIL_FEATURE_SHADOW)
            {
                EditorGUI.indentLevel++;
                ToggleGUI(GetLoc("sSettingReceiveShadow"), ref shaderSetting.LIL_FEATURE_RECEIVE_SHADOW);
                ToggleGUI(GetLoc("sSettingTexShadowBlur"), ref shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR);
                ToggleGUI(GetLoc("sSettingTexShadowBorder"), ref shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER);
                ToggleGUI(GetLoc("sSettingTexShadowStrength"), ref shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH);
                ToggleGUI(GetLoc("sSettingTexShadow1st"), ref shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST);
                ToggleGUI(GetLoc("sSettingTexShadow2nd"), ref shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND);
                ToggleGUI(GetLoc("sSettingShadow3rd"), ref shaderSetting.LIL_FEATURE_SHADOW_3RD);
                if(shaderSetting.LIL_FEATURE_SHADOW_3RD)
                {
                    EditorGUI.indentLevel++;
                    ToggleGUI(GetLoc("sSettingTexShadow3rd"), ref shaderSetting.LIL_FEATURE_TEX_SHADOW_3RD);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
            DrawLine();

            ToggleGUI(GetLoc("sSettingEmission1st"), ref shaderSetting.LIL_FEATURE_EMISSION_1ST);
            ToggleGUI(GetLoc("sSettingEmission2nd"), ref shaderSetting.LIL_FEATURE_EMISSION_2ND);
            if(shaderSetting.LIL_FEATURE_EMISSION_1ST || shaderSetting.LIL_FEATURE_EMISSION_2ND)
            {
                EditorGUI.indentLevel++;
                ToggleGUI(GetLoc("sSettingAnimateEmissionUV"), ref shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV);
                ToggleGUI(GetLoc("sSettingTexEmissionMask"), ref shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK);
                if(shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK)
                {
                    EditorGUI.indentLevel++;
                    ToggleGUI(GetLoc("sSettingAnimateEmissionMaskUV"), ref shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV);
                    EditorGUI.indentLevel--;
                }
                ToggleGUI(GetLoc("sSettingEmissionGradation"), ref shaderSetting.LIL_FEATURE_EMISSION_GRADATION);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            ToggleGUI(GetLoc("sSettingNormal1st"), ref shaderSetting.LIL_FEATURE_NORMAL_1ST);
            ToggleGUI(GetLoc("sSettingNormal2nd"), ref shaderSetting.LIL_FEATURE_NORMAL_2ND);
            if(shaderSetting.LIL_FEATURE_NORMAL_2ND)
            {
                EditorGUI.indentLevel++;
                ToggleGUI(GetLoc("sSettingTexNormalMask"), ref shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            ToggleGUI(GetLoc("sSettingAnisotropy"), ref shaderSetting.LIL_FEATURE_ANISOTROPY);
            DrawLine();

            ToggleGUI(GetLoc("sSettingReflection"), ref shaderSetting.LIL_FEATURE_REFLECTION);
            if(shaderSetting.LIL_FEATURE_REFLECTION)
            {
                EditorGUI.indentLevel++;
                ToggleGUI(GetLoc("sSettingTexReflectionSmoothness"), ref shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS);
                ToggleGUI(GetLoc("sSettingTexReflectionMetallic"), ref shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC);
                ToggleGUI(GetLoc("sSettingTexReflectionColor"), ref shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            ToggleGUI(GetLoc("sSettingMatCap"), ref shaderSetting.LIL_FEATURE_MATCAP);
            ToggleGUI(GetLoc("sSettingMatCap2nd"), ref shaderSetting.LIL_FEATURE_MATCAP_2ND);
            if(shaderSetting.LIL_FEATURE_MATCAP || shaderSetting.LIL_FEATURE_MATCAP_2ND)
            {
                EditorGUI.indentLevel++;
                ToggleGUI(GetLoc("sSettingTexMatCapMask"), ref shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK);
                ToggleGUI(GetLoc("sSettingTexMatCapNormal"), ref shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            ToggleGUI(GetLoc("sSettingRimLight"), ref shaderSetting.LIL_FEATURE_RIMLIGHT);
            if(shaderSetting.LIL_FEATURE_RIMLIGHT)
            {
                EditorGUI.indentLevel++;
                ToggleGUI(GetLoc("sSettingTexRimLightColor"), ref shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR);
                ToggleGUI(GetLoc("sSettingTexRimLightDirection"), ref shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            ToggleGUI(GetLoc("sSettingGlitter"), ref shaderSetting.LIL_FEATURE_GLITTER);
            DrawLine();

            ToggleGUI(GetLoc("sSettingBacklight"), ref shaderSetting.LIL_FEATURE_BACKLIGHT);
            DrawLine();

            ToggleGUI(GetLoc("sSettingParallax"), ref shaderSetting.LIL_FEATURE_PARALLAX);
            if(shaderSetting.LIL_FEATURE_PARALLAX)
            {
                EditorGUI.indentLevel++;
                ToggleGUI(GetLoc("sSettingPOM"), ref shaderSetting.LIL_FEATURE_POM);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            ToggleGUI(GetLoc("sSettingClippingCanceller"), ref shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER);
            DrawLine();

            ToggleGUI(GetLoc("sSettingDistanceFade"), ref shaderSetting.LIL_FEATURE_DISTANCE_FADE);
            DrawLine();

            ToggleGUI(GetLoc("sSettingAudioLink"), ref shaderSetting.LIL_FEATURE_AUDIOLINK);
            if(shaderSetting.LIL_FEATURE_AUDIOLINK)
            {
                EditorGUI.indentLevel++;
                ToggleGUI(GetLoc("sSettingAudioLinkVertex"), ref shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX);
                ToggleGUI(GetLoc("sSettingAudioLinkLocal"), ref shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            ToggleGUI(GetLoc("sSettingDissolve"), ref shaderSetting.LIL_FEATURE_DISSOLVE);
            if(shaderSetting.LIL_FEATURE_DISSOLVE)
            {
                EditorGUI.indentLevel++;
                ToggleGUI(GetLoc("sSettingTexDissolveNoise"), ref shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            if(string.IsNullOrEmpty(GetAvatarEncryptionPath()))
            {
                shaderSetting.LIL_FEATURE_ENCRYPTION = false;
            }
            else
            {
                ToggleGUI(GetLoc("sSettingEncryption"), ref shaderSetting.LIL_FEATURE_ENCRYPTION);
                DrawLine();
            }

            ToggleGUI(GetLoc("sSettingTexOutlineColor"), ref shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR);
            if(shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR)
            {
                EditorGUI.indentLevel++;
                ToggleGUI(GetLoc("sSettingOutlineToneCorrection"), ref shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION);
                EditorGUI.indentLevel--;
            }
            ToggleGUI(GetLoc("sSettingAnimateOutlineUV"), ref shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV);
            ToggleGUI(GetLoc("sSettingTexOutlineWidth"), ref shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH);
            ToggleGUI(GetLoc("sSettingTexOutlineNormal"), ref shaderSetting.LIL_FEATURE_TEX_OUTLINE_NORMAL);
            DrawLine();

            ToggleGUI(GetLoc("sSettingTexFurNormal"), ref shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL);
            ToggleGUI(GetLoc("sSettingTexFurMask"), ref shaderSetting.LIL_FEATURE_TEX_FUR_MASK);
            ToggleGUI(GetLoc("sSettingTexFurLength"), ref shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH);
            ToggleGUI(GetLoc("sSettingFurCollision"), ref shaderSetting.LIL_FEATURE_FUR_COLLISION);
            DrawLine();

            ToggleGUI(GetLoc("sSettingTessellation"), ref shaderSetting.LIL_FEATURE_TEX_TESSELLATION);

            if(EditorGUI.EndChangeCheck())
            {
                edSet.isShaderSettingChanged = true;
            }
        }

        private static void OptimizationSettingGUI()
        {
            EditorGUI.BeginChangeCheck();

            lilRenderPipeline lilRP = CheckRP();
            if(lilRP == lilRenderPipeline.BRP)
            {
                ToggleGUI(GetLoc("sSettingApplyShadowFA"), ref shaderSetting.LIL_OPTIMIZE_APPLY_SHADOW_FA);
                ToggleGUI(GetLoc("sSettingUseForwardAdd"), ref shaderSetting.LIL_OPTIMIZE_USE_FORWARDADD);
                ToggleGUI(GetLoc("sSettingUseVertexLight"), ref shaderSetting.LIL_OPTIMIZE_USE_VERTEXLIGHT);
            }
            ToggleGUI(GetLoc("sSettingUseLightmap"), ref shaderSetting.LIL_OPTIMIZE_USE_LIGHTMAP);

            if(EditorGUI.EndChangeCheck())
            {
                edSet.isShaderSettingChanged = true;
            }
        }

        private static void DefaultValueSettingGUI()
        {
            EditorGUI.BeginChangeCheck();

            shaderSetting.defaultAsUnlit                        = EditorGUILayout.Slider(GetLoc("sAsUnlit"), shaderSetting.defaultAsUnlit, 0.0f, 1.0f);
            shaderSetting.defaultVertexLightStrength            = EditorGUILayout.Slider(GetLoc("sVertexLightStrength"), shaderSetting.defaultVertexLightStrength, 0.0f, 1.0f);
            shaderSetting.defaultLightMinLimit                  = EditorGUILayout.Slider(GetLoc("sLightMinLimit"), shaderSetting.defaultLightMinLimit, 0.0f, 1.0f);
            shaderSetting.defaultLightMaxLimit                  = EditorGUILayout.Slider(GetLoc("sLightMaxLimit"), shaderSetting.defaultLightMaxLimit, 0.0f, 10.0f);
            shaderSetting.defaultMonochromeLighting             = EditorGUILayout.Slider(GetLoc("sMonochromeLighting"), shaderSetting.defaultMonochromeLighting, 0.0f, 1.0f);
            shaderSetting.defaultLightDirectionOverride         = EditorGUILayout.Vector4Field(GetLoc("sLightDirectionOverride"), shaderSetting.defaultLightDirectionOverride);
            shaderSetting.defaultBeforeExposureLimit            = EditorGUILayout.FloatField(GetLoc("sBeforeExposureLimit"), shaderSetting.defaultBeforeExposureLimit);
            shaderSetting.defaultlilDirectionalLightStrength    = EditorGUILayout.Slider(GetLoc("sDirectionalLightStrength"), shaderSetting.defaultlilDirectionalLightStrength, 0.0f, 1.0f);

            if(EditorGUI.EndChangeCheck())
            {
                edSet.isShaderSettingChanged = true;
            }
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
        // Shader Rewriter
        #region
        public static void ReimportPassShaders()
        {
            string[] shaderFolderPaths = GetShaderFolderPaths();
            foreach(string shaderGuid in AssetDatabase.FindAssets("t:shader", shaderFolderPaths))
            {
                string shaderPath = AssetDatabase.GUIDToAssetPath(shaderGuid);
                if(shaderPath.Contains("ltsmulti") ||
                    shaderPath.Contains("ltspass") ||
                    shaderPath.Contains("fur") ||
                    shaderPath.Contains("ref") ||
                    shaderPath.Contains("gem") ||
                    shaderPath.Contains("fakeshadow"))
                {
                    AssetDatabase.ImportAsset(shaderPath);
                }
            }
        }

        public static void RewriteSettingPath(string path)
        {
            if(string.IsNullOrEmpty(path) || !File.Exists(path)) return;
            StreamReader sr = new StreamReader(path);
            string s = sr.ReadToEnd();
            sr.Close();
            if(lilToonInspector.isUPM && s.Contains("#include \"../../lilToonSetting/lil_setting.hlsl\""))
            {
                s = s.Replace(
                    "#include \"../../lilToonSetting/lil_setting.hlsl\"",
                    "#include \"Assets/lilToonSetting/lil_setting.hlsl\"");
                StreamWriter sw = new StreamWriter(path,false);
                sw.Write(s);
                sw.Close();
            }
            else if(!lilToonInspector.isUPM && s.Contains("#include \"Assets/lilToonSetting/lil_setting.hlsl\""))
            {
                s = s.Replace(
                    "#include \"Assets/lilToonSetting/lil_setting.hlsl\"",
                    "#include \"../../lilToonSetting/lil_setting.hlsl\"");
                StreamWriter sw = new StreamWriter(path,false);
                sw.Write(s);
                sw.Close();
            }
        }

        public static void RewriteReceiveShadow(string path, bool enable)
        {
            if(string.IsNullOrEmpty(path) || !File.Exists(path)) return;
            StreamReader sr = new StreamReader(path);
            string s = sr.ReadToEnd();
            sr.Close();
            if(enable)
            {
                // BRP
                s = s.Replace(
                    "            // Skip receiving shadow\r\n            #pragma skip_variants SHADOWS_SCREEN",
                    "            // Skip receiving shadow\r\n            //#pragma skip_variants SHADOWS_SCREEN");
                // LWRP & URP
                s = s.Replace(
                    "            // Skip receiving shadow\r\n            //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN\r\n            //#pragma multi_compile_fragment _ _SHADOWS_SOFT",
                    "            // Skip receiving shadow\r\n            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN\r\n            #pragma multi_compile_fragment _ _SHADOWS_SOFT");
                // HDRP
                s = s.Replace(
                    "            // Skip receiving shadow\r\n            //#pragma multi_compile SCREEN_SPACE_SHADOWS_OFF SCREEN_SPACE_SHADOWS_ON\r\n            //#pragma multi_compile SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH",
                    "            // Skip receiving shadow\r\n            #pragma multi_compile SCREEN_SPACE_SHADOWS_OFF SCREEN_SPACE_SHADOWS_ON\r\n            #pragma multi_compile SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH");
            }
            else
            {
                // BRP
                s = s.Replace(
                    "            // Skip receiving shadow\r\n            //#pragma skip_variants SHADOWS_SCREEN",
                    "            // Skip receiving shadow\r\n            #pragma skip_variants SHADOWS_SCREEN");
                // LWRP & URP
                s = s.Replace(
                    "            // Skip receiving shadow\r\n            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN\r\n            #pragma multi_compile_fragment _ _SHADOWS_SOFT",
                    "            // Skip receiving shadow\r\n            //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN\r\n            //#pragma multi_compile_fragment _ _SHADOWS_SOFT");
                // HDRP
                s = s.Replace(
                    "            // Skip receiving shadow\r\n            #pragma multi_compile SCREEN_SPACE_SHADOWS_OFF SCREEN_SPACE_SHADOWS_ON\r\n            #pragma multi_compile SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH",
                    "            // Skip receiving shadow\r\n            //#pragma multi_compile SCREEN_SPACE_SHADOWS_OFF SCREEN_SPACE_SHADOWS_ON\r\n            //#pragma multi_compile SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH");
            }
            StreamWriter sw = new StreamWriter(path,false);
            sw.Write(s);
            sw.Close();
        }

        public static void RewriteForwardAdd(string path, bool enable)
        {
            if(string.IsNullOrEmpty(path) || !File.Exists(path)) return;
            StreamReader sr = new StreamReader(path);
            string s = sr.ReadToEnd();
            sr.Close();
            if(enable)
            {
                s = s.Replace(
                    "        // ForwardAdd Start\r\n        /*",
                    "        // ForwardAdd Start\r\n        //");
                s = s.Replace(
                    "        */\r\n        // ForwardAdd End",
                    "        //\r\n        // ForwardAdd End");
            }
            else
            {
                s = s.Replace(
                    "        // ForwardAdd Start\r\n        //",
                    "        // ForwardAdd Start\r\n        /*");
                s = s.Replace(
                    "        //\r\n        // ForwardAdd End",
                    "        */\r\n        // ForwardAdd End");
            }

            StreamWriter sw = new StreamWriter(path,false);
            string[] lines = s.Split('\n');
            for(int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                line = line.Replace("\r", "");

                if(line.Contains("UsePass") && line.Contains("FORWARD_ADD"))
                {
                    if(enable)
                    {
                        line = line.Replace(
                            "        //UsePass",
                            "        UsePass");
                    }
                    else
                    {
                        line = line.Replace(
                            "        UsePass",
                            "        //UsePass");
                    }
                }
                if(i != lines.Length - 1) sw.WriteLine(line);
                else                      sw.Write(line);
                
            }
            sw.Close();
        }

        public static void RewriteVertexLight(string path, bool enable)
        {
            if(string.IsNullOrEmpty(path) || !File.Exists(path)) return;
            StreamReader sr = new StreamReader(path);
            string s = sr.ReadToEnd();
            sr.Close();
            if(enable)
            {
                // BRP
                s = s.Replace(
                    "            // Skip vertex light\r\n            #pragma skip_variants VERTEXLIGHT_ON",
                    "            // Skip vertex light\r\n            //#pragma skip_variants VERTEXLIGHT_ON");
            }
            else
            {
                // BRP
                s = s.Replace(
                    "            // Skip vertex light\r\n            //#pragma skip_variants VERTEXLIGHT_ON",
                    "            // Skip vertex light\r\n            #pragma skip_variants VERTEXLIGHT_ON");
            }
            StreamWriter sw = new StreamWriter(path,false);
            sw.Write(s);
            sw.Close();
        }

        public static void RewriteLightmap(string path, bool enable)
        {
            if(string.IsNullOrEmpty(path) || !File.Exists(path)) return;
            StreamReader sr = new StreamReader(path);
            string s = sr.ReadToEnd();
            sr.Close();
            if(enable)
            {
                // BRP
                s = s.Replace(
                    "            // Skip lightmap\r\n            #pragma skip_variants LIGHTMAP_ON DYNAMICLIGHTMAP_ON LIGHTMAP_SHADOW_MIXING SHADOWS_SHADOWMASK",
                    "            // Skip lightmap\r\n            //#pragma skip_variants LIGHTMAP_ON DYNAMICLIGHTMAP_ON LIGHTMAP_SHADOW_MIXING SHADOWS_SHADOWMASK");
            }
            else
            {
                // BRP
                s = s.Replace(
                    "            // Skip lightmap\r\n            //#pragma skip_variants LIGHTMAP_ON DYNAMICLIGHTMAP_ON LIGHTMAP_SHADOW_MIXING SHADOWS_SHADOWMASK",
                    "            // Skip lightmap\r\n            #pragma skip_variants LIGHTMAP_ON DYNAMICLIGHTMAP_ON LIGHTMAP_SHADOW_MIXING SHADOWS_SHADOWMASK");
            }
            StreamWriter sw = new StreamWriter(path,false);
            sw.Write(s);
            sw.Close();
        }

        public static void RewriteReceiveShadow(Shader shader, bool enable)
        {
            string path = AssetDatabase.GetAssetPath(shader);
            RewriteReceiveShadow(path, enable);
        }

        public static void RewriteZClip(string path)
        {
            if(string.IsNullOrEmpty(path) || !File.Exists(path)) return;
            StreamReader sr = new StreamReader(path);
            string s = sr.ReadToEnd();
            sr.Close();
            #if UNITY_2018_1_OR_NEWER
                s = s.Replace(
                    "            //ZClip",
                    "            ZClip");
            #else
                s = s.Replace(
                    "            ZClip",
                    "            //ZClip");
            #endif
            StreamWriter sw = new StreamWriter(path,false);
            sw.Write(s);
            sw.Close();
        }

        public static void RewriteZClip(Shader shader)
        {
            string path = AssetDatabase.GetAssetPath(shader);
            RewriteZClip(path);
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
                Material material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
                MigrateMaterial(material);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void MigrateMaterial(Material material)
        {
            int version = material.HasProperty("_lilToonVersion") ? (int)material.GetFloat("_lilToonVersion") : 0;
            MigrateMaterial(material, version);
        }

        public static void MigrateMaterial(Material material, int version)
        {
            if(material.shader == null || !material.shader.name.Contains("lilToon") || version >= currentVersionValue) return;
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

        public static bool CheckFeature(bool feature)
        {
            return isMulti || feature;
        }

        private static void InitializeGUIStyles()
        {
            wrapLabel = new GUIStyle(GUI.skin.label){wordWrap = true};
            boldLabel = new GUIStyle(GUI.skin.label){fontStyle = FontStyle.Bold};
            foldout = new GUIStyle("ShurikenModuleTitle")
            {
                #if UNITY_2019_1_OR_NEWER
                    fontSize = 12,
                #else
                    fontSize = 11,
                #endif
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
            EditorGUI.indentLevel++;
            Rect position = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(position, versionLabel, labelStyle);
            EditorGUI.indentLevel--;

            position.x += isCustomEditor ? 0 : 10;
            edSet.isShowWebPages = EditorGUI.Foldout(position, edSet.isShowWebPages, "");
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
            EditorGUI.indentLevel++;
            Rect position = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(position, GetLoc("sWhenInTrouble"), boldLabel);
            EditorGUI.indentLevel--;

            position.x += isCustomEditor ? 0 : 10;
            edSet.isShowHelpPages = EditorGUI.Foldout(position, edSet.isShowHelpPages, "");
            if(edSet.isShowHelpPages)
            {
                EditorGUI.indentLevel++;
                DrawWebButton(GetLoc("sCommonProblems"), GetLoc("sReadmeURL") + GetLoc("sReadmeAnchorProblem"));
                EditorGUI.indentLevel--;
            }
        }

        private static void ToggleGUI(string label, ref bool value)
        {
            value = EditorGUILayout.ToggleLeft(label, value);
        }

        private static bool AutoFixHelpBox(string message)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label(message, EditorStyles.wordWrappedMiniLabel);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            bool pressed = GUILayout.Button(GetLoc("sFixNow"));
            GUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
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
                if(isnormal && GUILayout.Button(GetLoc("sOptimizeForEvents"))) RemoveUnusedTexture(material);
            #endif
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Editor
        #region
        public static string BuildParams(params string[] labels)
        {
            return string.Join("|", labels);
        }

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
                if(!File.Exists(versionInfoTempPath))
                {
                    latestVersion.latest_vertion_name = currentVersionName;
                    latestVersion.latest_vertion_value = currentVersionValue;
                    return;
                }
                StreamReader sr = new StreamReader(versionInfoTempPath);
                string s = sr.ReadToEnd();
                sr.Close();
                if(!string.IsNullOrEmpty(s) && s.Contains("latest_vertion_name") && s.Contains("latest_vertion_value"))
                {
                    EditorJsonUtility.FromJsonOverwrite(s,latestVersion);
                }
                else
                {
                    latestVersion.latest_vertion_name = currentVersionName;
                    latestVersion.latest_vertion_value = currentVersionValue;
                    return;
                }
            }
        }

        public void CopyProperty(MaterialProperty property)
        {
            if(property != null) copiedProperties[property.name] = property;
        }

        public void PasteProperty(ref MaterialProperty property)
        {
            if(property != null && copiedProperties.ContainsKey(property.name) && copiedProperties[property.name] != null)
            {
                MaterialProperty.PropType propType = property.type;
                if(propType == MaterialProperty.PropType.Color)     property.colorValue = copiedProperties[property.name].colorValue;
                if(propType == MaterialProperty.PropType.Vector)    property.vectorValue = copiedProperties[property.name].vectorValue;
                if(propType == MaterialProperty.PropType.Float)     property.floatValue = copiedProperties[property.name].floatValue;
                if(propType == MaterialProperty.PropType.Range)     property.floatValue = copiedProperties[property.name].floatValue;
                if(propType == MaterialProperty.PropType.Texture)   property.textureValue = copiedProperties[property.name].textureValue;
            }
        }

        public void ResetProperty(ref MaterialProperty property)
        {
            #if UNITY_2019_3_OR_NEWER
            if(property != null && property.targets[0] is Material && ((Material)property.targets[0]).shader != null)
            {
                Shader shader = ((Material)property.targets[0]).shader;
                int propID = shader.FindPropertyIndex(property.name);
                if(propID == -1) return;
                MaterialProperty.PropType propType = property.type;
                if(propType == MaterialProperty.PropType.Color)     property.colorValue = shader.GetPropertyDefaultVectorValue(propID);
                if(propType == MaterialProperty.PropType.Vector)    property.vectorValue = shader.GetPropertyDefaultVectorValue(propID);
                if(propType == MaterialProperty.PropType.Float)     property.floatValue = shader.GetPropertyDefaultFloatValue(propID);
                if(propType == MaterialProperty.PropType.Range)     property.floatValue = shader.GetPropertyDefaultFloatValue(propID);
                if(propType == MaterialProperty.PropType.Texture)   property.textureValue = null;
            }
            #endif
        }

        private void CopyProperties(object obj)
        {
            lilPropertyBlock propertyBlock = (lilPropertyBlock)obj;
            CopyProperties(propertyBlock);
        }

        private void PasteProperties(object obj)
        {
            lilPropertyBlockData propertyBlockData = (lilPropertyBlockData)obj;
            PasteProperties(propertyBlockData.propertyBlock, propertyBlockData.shouldCopyTex);
        }

        private void ResetProperties(object obj)
        {
            lilPropertyBlock propertyBlock = (lilPropertyBlock)obj;
            ResetProperties(propertyBlock);
        }

        private void CopyProperties(lilPropertyBlock propertyBlock)
        {
            switch(propertyBlock)
            {
                case lilPropertyBlock.Base:
                        CopyProperty(invisible);
                        CopyProperty(cutoff);
                        CopyProperty(subpassCutoff);
                        CopyProperty(cull);
                        CopyProperty(flipNormal);
                        CopyProperty(backfaceForceShadow);
                        CopyProperty(zwrite);
                        CopyProperty(asUnlit);
                        CopyProperty(vertexLightStrength);
                        CopyProperty(lightMinLimit);
                        CopyProperty(lightMaxLimit);
                        CopyProperty(monochromeLighting);
                        CopyProperty(alphaBoostFA);
                        CopyProperty(shadowEnvStrength);
                        CopyProperty(fakeShadowVector);
                        CopyProperty(triMask);
                    break;
                case lilPropertyBlock.Lighting:
                        CopyProperty(asUnlit);
                        CopyProperty(vertexLightStrength);
                        CopyProperty(lightMinLimit);
                        CopyProperty(lightMaxLimit);
                        CopyProperty(monochromeLighting);
                        CopyProperty(alphaBoostFA);
                        CopyProperty(shadowEnvStrength);
                    break;
                case lilPropertyBlock.UV:
                        CopyProperty(mainTex_ScrollRotate);
                        CopyProperty(shiftBackfaceUV);
                    break;
                case lilPropertyBlock.MainColor:
                        CopyProperty(mainColor);
                        CopyProperty(mainTexHSVG);
                        CopyProperty(mainGradationStrength);
                        CopyProperty(mainGradationTex);
                        CopyProperty(useMain2ndTex);
                        CopyProperty(mainColor2nd);
                        CopyProperty(main2ndTex_UVMode);
                        CopyProperty(main2ndTexAngle);
                        CopyProperty(main2ndTexDecalAnimation);
                        CopyProperty(main2ndTexDecalSubParam);
                        CopyProperty(main2ndTexIsDecal);
                        CopyProperty(main2ndTexIsLeftOnly);
                        CopyProperty(main2ndTexIsRightOnly);
                        CopyProperty(main2ndTexShouldCopy);
                        CopyProperty(main2ndTexShouldFlipMirror);
                        CopyProperty(main2ndTexShouldFlipCopy);
                        CopyProperty(main2ndTexIsMSDF);
                        CopyProperty(main2ndTexBlendMode);
                        CopyProperty(main2ndEnableLighting);
                        CopyProperty(main2ndDissolveNoiseMask_ScrollRotate);
                        CopyProperty(main2ndDissolveNoiseStrength);
                        CopyProperty(main2ndDissolveColor);
                        CopyProperty(main2ndDissolveParams);
                        CopyProperty(main2ndDissolvePos);
                        CopyProperty(main2ndDistanceFade);
                        CopyProperty(useMain3rdTex);
                        CopyProperty(mainColor3rd);
                        CopyProperty(main3rdTex_UVMode);
                        CopyProperty(main3rdTexAngle);
                        CopyProperty(main3rdTexDecalAnimation);
                        CopyProperty(main3rdTexDecalSubParam);
                        CopyProperty(main3rdTexIsDecal);
                        CopyProperty(main3rdTexIsLeftOnly);
                        CopyProperty(main3rdTexIsRightOnly);
                        CopyProperty(main3rdTexShouldCopy);
                        CopyProperty(main3rdTexShouldFlipMirror);
                        CopyProperty(main3rdTexShouldFlipCopy);
                        CopyProperty(main3rdTexIsMSDF);
                        CopyProperty(main3rdTexBlendMode);
                        CopyProperty(main3rdEnableLighting);
                        CopyProperty(main3rdDissolveMask);
                        CopyProperty(main3rdDissolveNoiseMask);
                        CopyProperty(main3rdDissolveNoiseMask_ScrollRotate);
                        CopyProperty(main3rdDissolveNoiseStrength);
                        CopyProperty(main3rdDissolveColor);
                        CopyProperty(main3rdDissolveParams);
                        CopyProperty(main3rdDissolvePos);
                        CopyProperty(main3rdDistanceFade);
                        CopyProperty(alphaMaskMode);
                        CopyProperty(alphaMaskScale);
                        CopyProperty(alphaMaskValue);
                        CopyProperty(mainTex);
                        CopyProperty(mainColorAdjustMask);
                        CopyProperty(main2ndTex);
                        CopyProperty(main2ndBlendMask);
                        CopyProperty(main2ndDissolveMask);
                        CopyProperty(main2ndDissolveNoiseMask);
                        CopyProperty(main3rdTex);
                        CopyProperty(main3rdBlendMask);
                        CopyProperty(main3rdDissolveMask);
                        CopyProperty(main3rdDissolveNoiseMask);
                        CopyProperty(alphaMask);
                    break;
                case lilPropertyBlock.MainColor1st:
                        CopyProperty(mainColor);
                        CopyProperty(mainTexHSVG);
                        CopyProperty(mainGradationStrength);
                        CopyProperty(mainGradationTex);
                        CopyProperty(mainTex);
                        CopyProperty(mainColorAdjustMask);
                    break;
                case lilPropertyBlock.MainColor2nd:
                        CopyProperty(useMain2ndTex);
                        CopyProperty(mainColor2nd);
                        CopyProperty(main2ndTex_UVMode);
                        CopyProperty(main2ndTexAngle);
                        CopyProperty(main2ndTexDecalAnimation);
                        CopyProperty(main2ndTexDecalSubParam);
                        CopyProperty(main2ndTexIsDecal);
                        CopyProperty(main2ndTexIsLeftOnly);
                        CopyProperty(main2ndTexIsRightOnly);
                        CopyProperty(main2ndTexShouldCopy);
                        CopyProperty(main2ndTexShouldFlipMirror);
                        CopyProperty(main2ndTexShouldFlipCopy);
                        CopyProperty(main2ndTexIsMSDF);
                        CopyProperty(main2ndTexBlendMode);
                        CopyProperty(main2ndEnableLighting);
                        CopyProperty(main2ndDissolveNoiseMask_ScrollRotate);
                        CopyProperty(main2ndDissolveNoiseStrength);
                        CopyProperty(main2ndDissolveColor);
                        CopyProperty(main2ndDissolveParams);
                        CopyProperty(main2ndDissolvePos);
                        CopyProperty(main2ndDistanceFade);
                        CopyProperty(main2ndTex);
                        CopyProperty(main2ndBlendMask);
                        CopyProperty(main2ndDissolveMask);
                        CopyProperty(main2ndDissolveNoiseMask);
                    break;
                case lilPropertyBlock.MainColor3rd:
                        CopyProperty(useMain3rdTex);
                        CopyProperty(mainColor3rd);
                        CopyProperty(main3rdTex_UVMode);
                        CopyProperty(main3rdTexAngle);
                        CopyProperty(main3rdTexDecalAnimation);
                        CopyProperty(main3rdTexDecalSubParam);
                        CopyProperty(main3rdTexIsDecal);
                        CopyProperty(main3rdTexIsLeftOnly);
                        CopyProperty(main3rdTexIsRightOnly);
                        CopyProperty(main3rdTexShouldCopy);
                        CopyProperty(main3rdTexShouldFlipMirror);
                        CopyProperty(main3rdTexShouldFlipCopy);
                        CopyProperty(main3rdTexIsMSDF);
                        CopyProperty(main3rdTexBlendMode);
                        CopyProperty(main3rdEnableLighting);
                        CopyProperty(main3rdDissolveMask);
                        CopyProperty(main3rdDissolveNoiseMask);
                        CopyProperty(main3rdDissolveNoiseMask_ScrollRotate);
                        CopyProperty(main3rdDissolveNoiseStrength);
                        CopyProperty(main3rdDissolveColor);
                        CopyProperty(main3rdDissolveParams);
                        CopyProperty(main3rdDissolvePos);
                        CopyProperty(main3rdDistanceFade);
                        CopyProperty(main3rdTex);
                        CopyProperty(main3rdBlendMask);
                        CopyProperty(main3rdDissolveMask);
                        CopyProperty(main3rdDissolveNoiseMask);
                    break;
                case lilPropertyBlock.AlphaMask:
                        CopyProperty(alphaMaskMode);
                        CopyProperty(alphaMaskScale);
                        CopyProperty(alphaMaskValue);
                        CopyProperty(alphaMask);
                    break;
                case lilPropertyBlock.Shadow:
                        CopyProperty(useShadow);
                        CopyProperty(shadowColor);
                        CopyProperty(shadowNormalStrength);
                        CopyProperty(shadowBorder);
                        CopyProperty(shadowBlur);
                        CopyProperty(shadowStrength);
                        CopyProperty(shadowAOShift);
                        CopyProperty(shadowAOShift2);
                        CopyProperty(shadowPostAO);
                        CopyProperty(shadow2ndColor);
                        CopyProperty(shadow2ndNormalStrength);
                        CopyProperty(shadow2ndBorder);
                        CopyProperty(shadow2ndBlur);
                        CopyProperty(shadow3rdColor);
                        CopyProperty(shadow3rdNormalStrength);
                        CopyProperty(shadow3rdBorder);
                        CopyProperty(shadow3rdBlur);
                        CopyProperty(shadowMainStrength);
                        CopyProperty(shadowEnvStrength);
                        CopyProperty(shadowBorderColor);
                        CopyProperty(shadowBorderRange);
                        CopyProperty(shadowReceive);
                        CopyProperty(shadowBorderMask);
                        CopyProperty(shadowBlurMask);
                        CopyProperty(shadowStrengthMask);
                        CopyProperty(shadowColorTex);
                        CopyProperty(shadow2ndColorTex);
                        CopyProperty(shadow3rdColorTex);
                    break;
                case lilPropertyBlock.Emission:
                        CopyProperty(useEmission);
                        CopyProperty(emissionColor);
                        CopyProperty(emissionMap_ScrollRotate);
                        CopyProperty(emissionMap_UVMode);
                        CopyProperty(emissionBlend);
                        CopyProperty(emissionBlendMask_ScrollRotate);
                        CopyProperty(emissionBlink);
                        CopyProperty(emissionUseGrad);
                        CopyProperty(emissionGradTex);
                        CopyProperty(emissionGradSpeed);
                        CopyProperty(emissionParallaxDepth);
                        CopyProperty(emissionFluorescence);
                        CopyProperty(useEmission2nd);
                        CopyProperty(emission2ndColor);
                        CopyProperty(emission2ndMap_ScrollRotate);
                        CopyProperty(emission2ndMap_UVMode);
                        CopyProperty(emission2ndBlend);
                        CopyProperty(emission2ndBlendMask_ScrollRotate);
                        CopyProperty(emission2ndBlink);
                        CopyProperty(emission2ndUseGrad);
                        CopyProperty(emission2ndGradTex);
                        CopyProperty(emission2ndGradSpeed);
                        CopyProperty(emission2ndParallaxDepth);
                        CopyProperty(emission2ndFluorescence);
                        CopyProperty(emissionMap);
                        CopyProperty(emissionBlendMask);
                        CopyProperty(emission2ndMap);
                        CopyProperty(emission2ndBlendMask);
                    break;
                case lilPropertyBlock.Emission1st:
                        CopyProperty(useEmission);
                        CopyProperty(emissionColor);
                        CopyProperty(emissionMap_ScrollRotate);
                        CopyProperty(emissionMap_UVMode);
                        CopyProperty(emissionBlend);
                        CopyProperty(emissionBlendMask_ScrollRotate);
                        CopyProperty(emissionBlink);
                        CopyProperty(emissionUseGrad);
                        CopyProperty(emissionGradTex);
                        CopyProperty(emissionGradSpeed);
                        CopyProperty(emissionParallaxDepth);
                        CopyProperty(emissionFluorescence);
                        CopyProperty(emissionMap);
                        CopyProperty(emissionBlendMask);
                    break;
                case lilPropertyBlock.Emission2nd:
                        CopyProperty(useEmission2nd);
                        CopyProperty(emission2ndColor);
                        CopyProperty(emission2ndMap_ScrollRotate);
                        CopyProperty(emission2ndMap_UVMode);
                        CopyProperty(emission2ndBlend);
                        CopyProperty(emission2ndBlendMask_ScrollRotate);
                        CopyProperty(emission2ndBlink);
                        CopyProperty(emission2ndUseGrad);
                        CopyProperty(emission2ndGradTex);
                        CopyProperty(emission2ndGradSpeed);
                        CopyProperty(emission2ndParallaxDepth);
                        CopyProperty(emission2ndFluorescence);
                        CopyProperty(emission2ndMap);
                        CopyProperty(emission2ndBlendMask);
                    break;
                case lilPropertyBlock.NormalMap:
                        CopyProperty(useBumpMap);
                        CopyProperty(bumpScale);
                        CopyProperty(useBump2ndMap);
                        CopyProperty(bump2ndScale);
                        CopyProperty(useAnisotropy);
                        CopyProperty(anisotropyScale);
                        CopyProperty(anisotropyTangentWidth);
                        CopyProperty(anisotropyBitangentWidth);
                        CopyProperty(anisotropyShift);
                        CopyProperty(anisotropyShiftNoiseScale);
                        CopyProperty(anisotropySpecularStrength);
                        CopyProperty(anisotropy2ndTangentWidth);
                        CopyProperty(anisotropy2ndBitangentWidth);
                        CopyProperty(anisotropy2ndShift);
                        CopyProperty(anisotropy2ndShiftNoiseScale);
                        CopyProperty(anisotropy2ndSpecularStrength);
                        CopyProperty(anisotropy2Reflection);
                        CopyProperty(anisotropy2MatCap);
                        CopyProperty(anisotropy2MatCap2nd);
                        CopyProperty(bumpMap);
                        CopyProperty(bump2ndMap);
                        CopyProperty(bump2ndScaleMask);
                        CopyProperty(anisotropyTangentMap);
                        CopyProperty(anisotropyScaleMask);
                        CopyProperty(anisotropyShiftNoiseMask);
                    break;
                case lilPropertyBlock.NormalMap1st:
                        CopyProperty(useBumpMap);
                        CopyProperty(bumpScale);
                        CopyProperty(bumpMap);
                    break;
                case lilPropertyBlock.NormalMap2nd:
                        CopyProperty(useBump2ndMap);
                        CopyProperty(bump2ndScale);
                        CopyProperty(bump2ndMap);
                        CopyProperty(bump2ndScaleMask);
                    break;
                case lilPropertyBlock.Anisotropy:
                        CopyProperty(useAnisotropy);
                        CopyProperty(anisotropyScale);
                        CopyProperty(anisotropyTangentWidth);
                        CopyProperty(anisotropyBitangentWidth);
                        CopyProperty(anisotropyShift);
                        CopyProperty(anisotropyShiftNoiseScale);
                        CopyProperty(anisotropySpecularStrength);
                        CopyProperty(anisotropy2ndTangentWidth);
                        CopyProperty(anisotropy2ndBitangentWidth);
                        CopyProperty(anisotropy2ndShift);
                        CopyProperty(anisotropy2ndShiftNoiseScale);
                        CopyProperty(anisotropy2ndSpecularStrength);
                        CopyProperty(anisotropy2Reflection);
                        CopyProperty(anisotropy2MatCap);
                        CopyProperty(anisotropy2MatCap2nd);
                        CopyProperty(anisotropyTangentMap);
                        CopyProperty(anisotropyScaleMask);
                        CopyProperty(anisotropyShiftNoiseMask);
                    break;
                case lilPropertyBlock.Reflections:
                        CopyProperty(useReflection);
                        CopyProperty(metallic);
                        CopyProperty(smoothness);
                        CopyProperty(reflectance);
                        CopyProperty(reflectionColor);
                        CopyProperty(applySpecular);
                        CopyProperty(applySpecularFA);
                        CopyProperty(specularNormalStrength);
                        CopyProperty(specularToon);
                        CopyProperty(specularBorder);
                        CopyProperty(specularBlur);
                        CopyProperty(applyReflection);
                        CopyProperty(reflectionNormalStrength);
                        CopyProperty(reflectionApplyTransparency);
                        CopyProperty(reflectionCubeColor);
                        CopyProperty(reflectionCubeOverride);
                        CopyProperty(reflectionCubeEnableLighting);
                        CopyProperty(useMatCap);
                        CopyProperty(matcapColor);
                        CopyProperty(matcapBlendUV1);
                        CopyProperty(matcapZRotCancel);
                        CopyProperty(matcapPerspective);
                        CopyProperty(matcapVRParallaxStrength);
                        CopyProperty(matcapBlend);
                        CopyProperty(matcapEnableLighting);
                        CopyProperty(matcapShadowMask);
                        CopyProperty(matcapBackfaceMask);
                        CopyProperty(matcapLod);
                        CopyProperty(matcapBlendMode);
                        CopyProperty(matcapMul);
                        CopyProperty(matcapApplyTransparency);
                        CopyProperty(matcapNormalStrength);
                        CopyProperty(matcapCustomNormal);
                        CopyProperty(matcapBumpScale);
                        CopyProperty(useMatCap2nd);
                        CopyProperty(matcap2ndColor);
                        CopyProperty(matcap2ndBlendUV1);
                        CopyProperty(matcap2ndZRotCancel);
                        CopyProperty(matcap2ndPerspective);
                        CopyProperty(matcap2ndVRParallaxStrength);
                        CopyProperty(matcap2ndBlend);
                        CopyProperty(matcap2ndEnableLighting);
                        CopyProperty(matcap2ndShadowMask);
                        CopyProperty(matcap2ndBackfaceMask);
                        CopyProperty(matcap2ndLod);
                        CopyProperty(matcap2ndBlendMode);
                        CopyProperty(matcap2ndMul);
                        CopyProperty(matcap2ndNormalStrength);
                        CopyProperty(matcap2ndApplyTransparency);
                        CopyProperty(matcap2ndCustomNormal);
                        CopyProperty(matcap2ndBumpScale);
                        CopyProperty(useRim);
                        CopyProperty(rimColor);
                        CopyProperty(rimNormalStrength);
                        CopyProperty(rimBorder);
                        CopyProperty(rimBlur);
                        CopyProperty(rimFresnelPower);
                        CopyProperty(rimEnableLighting);
                        CopyProperty(rimShadowMask);
                        CopyProperty(rimBackfaceMask);
                        CopyProperty(rimVRParallaxStrength);
                        CopyProperty(rimApplyTransparency);
                        CopyProperty(rimDirStrength);
                        CopyProperty(rimDirRange);
                        CopyProperty(rimIndirRange);
                        CopyProperty(rimIndirColor);
                        CopyProperty(rimIndirBorder);
                        CopyProperty(rimIndirBlur);
                        CopyProperty(useGlitter);
                        CopyProperty(glitterUVMode);
                        CopyProperty(glitterColor);
                        CopyProperty(glitterMainStrength);
                        CopyProperty(glitterParams1);
                        CopyProperty(glitterParams2);
                        CopyProperty(glitterPostContrast);
                        CopyProperty(glitterEnableLighting);
                        CopyProperty(glitterShadowMask);
                        CopyProperty(glitterBackfaceMask);
                        CopyProperty(glitterApplyTransparency);
                        CopyProperty(glitterVRParallaxStrength);
                        CopyProperty(glitterNormalStrength);
                        CopyProperty(useBacklight);
                        CopyProperty(backlightColor);
                        CopyProperty(backlightNormalStrength);
                        CopyProperty(backlightBorder);
                        CopyProperty(backlightBlur);
                        CopyProperty(backlightDirectivity);
                        CopyProperty(backlightViewStrength);
                        CopyProperty(backlightReceiveShadow);
                        CopyProperty(backlightBackfaceMask);
                        CopyProperty(gemChromaticAberration);
                        CopyProperty(gemEnvContrast);
                        CopyProperty(gemEnvColor);
                        CopyProperty(gemParticleLoop);
                        CopyProperty(gemParticleColor);
                        CopyProperty(gemVRParallaxStrength);
                        CopyProperty(refractionStrength);
                        CopyProperty(refractionFresnelPower);
                        CopyProperty(metallicGlossMap);
                        CopyProperty(smoothnessTex);
                        CopyProperty(reflectionColorTex);
                        CopyProperty(reflectionCubeTex);
                        CopyProperty(matcapTex);
                        CopyProperty(matcapBlendMask);
                        CopyProperty(matcapBumpMap);
                        CopyProperty(matcap2ndTex);
                        CopyProperty(matcap2ndBlendMask);
                        CopyProperty(matcap2ndBumpMap);
                        CopyProperty(rimColorTex);
                        CopyProperty(glitterColorTex);
                        CopyProperty(backlightColorTex);
                    break;
                case lilPropertyBlock.Reflection:
                        CopyProperty(useReflection);
                        CopyProperty(metallic);
                        CopyProperty(smoothness);
                        CopyProperty(reflectance);
                        CopyProperty(reflectionColor);
                        CopyProperty(applySpecular);
                        CopyProperty(applySpecularFA);
                        CopyProperty(specularNormalStrength);
                        CopyProperty(specularToon);
                        CopyProperty(specularBorder);
                        CopyProperty(specularBlur);
                        CopyProperty(applyReflection);
                        CopyProperty(reflectionNormalStrength);
                        CopyProperty(reflectionApplyTransparency);
                        CopyProperty(reflectionCubeColor);
                        CopyProperty(reflectionCubeOverride);
                        CopyProperty(reflectionCubeEnableLighting);
                        CopyProperty(metallicGlossMap);
                        CopyProperty(smoothnessTex);
                        CopyProperty(reflectionColorTex);
                        CopyProperty(reflectionCubeTex);
                    break;
                case lilPropertyBlock.MatCaps:
                        CopyProperty(useMatCap);
                        CopyProperty(matcapColor);
                        CopyProperty(matcapBlendUV1);
                        CopyProperty(matcapZRotCancel);
                        CopyProperty(matcapPerspective);
                        CopyProperty(matcapVRParallaxStrength);
                        CopyProperty(matcapBlend);
                        CopyProperty(matcapEnableLighting);
                        CopyProperty(matcapShadowMask);
                        CopyProperty(matcapBackfaceMask);
                        CopyProperty(matcapLod);
                        CopyProperty(matcapBlendMode);
                        CopyProperty(matcapMul);
                        CopyProperty(matcapApplyTransparency);
                        CopyProperty(matcapNormalStrength);
                        CopyProperty(matcapCustomNormal);
                        CopyProperty(matcapBumpScale);
                        CopyProperty(useMatCap2nd);
                        CopyProperty(matcap2ndColor);
                        CopyProperty(matcap2ndBlendUV1);
                        CopyProperty(matcap2ndZRotCancel);
                        CopyProperty(matcap2ndPerspective);
                        CopyProperty(matcap2ndVRParallaxStrength);
                        CopyProperty(matcap2ndBlend);
                        CopyProperty(matcap2ndEnableLighting);
                        CopyProperty(matcap2ndShadowMask);
                        CopyProperty(matcap2ndBackfaceMask);
                        CopyProperty(matcap2ndLod);
                        CopyProperty(matcap2ndBlendMode);
                        CopyProperty(matcap2ndMul);
                        CopyProperty(matcap2ndNormalStrength);
                        CopyProperty(matcap2ndApplyTransparency);
                        CopyProperty(matcap2ndCustomNormal);
                        CopyProperty(matcap2ndBumpScale);
                        CopyProperty(matcapTex);
                        CopyProperty(matcapBlendMask);
                        CopyProperty(matcapBumpMap);
                        CopyProperty(matcap2ndTex);
                        CopyProperty(matcap2ndBlendMask);
                        CopyProperty(matcap2ndBumpMap);
                    break;
                case lilPropertyBlock.MatCap1st:
                        CopyProperty(useMatCap);
                        CopyProperty(matcapColor);
                        CopyProperty(matcapBlendUV1);
                        CopyProperty(matcapZRotCancel);
                        CopyProperty(matcapPerspective);
                        CopyProperty(matcapVRParallaxStrength);
                        CopyProperty(matcapBlend);
                        CopyProperty(matcapEnableLighting);
                        CopyProperty(matcapShadowMask);
                        CopyProperty(matcapBackfaceMask);
                        CopyProperty(matcapLod);
                        CopyProperty(matcapBlendMode);
                        CopyProperty(matcapMul);
                        CopyProperty(matcapApplyTransparency);
                        CopyProperty(matcapNormalStrength);
                        CopyProperty(matcapCustomNormal);
                        CopyProperty(matcapBumpScale);
                        CopyProperty(matcapTex);
                        CopyProperty(matcapBlendMask);
                        CopyProperty(matcapBumpMap);
                    break;
                case lilPropertyBlock.MatCap2nd:
                        CopyProperty(useMatCap2nd);
                        CopyProperty(matcap2ndColor);
                        CopyProperty(matcap2ndBlendUV1);
                        CopyProperty(matcap2ndZRotCancel);
                        CopyProperty(matcap2ndPerspective);
                        CopyProperty(matcap2ndVRParallaxStrength);
                        CopyProperty(matcap2ndBlend);
                        CopyProperty(matcap2ndEnableLighting);
                        CopyProperty(matcap2ndShadowMask);
                        CopyProperty(matcap2ndBackfaceMask);
                        CopyProperty(matcap2ndLod);
                        CopyProperty(matcap2ndBlendMode);
                        CopyProperty(matcap2ndMul);
                        CopyProperty(matcap2ndApplyTransparency);
                        CopyProperty(matcap2ndNormalStrength);
                        CopyProperty(matcap2ndCustomNormal);
                        CopyProperty(matcap2ndBumpScale);
                        CopyProperty(matcap2ndTex);
                        CopyProperty(matcap2ndBlendMask);
                        CopyProperty(matcap2ndBumpMap);
                    break;
                case lilPropertyBlock.RimLight:
                        CopyProperty(useRim);
                        CopyProperty(rimColor);
                        CopyProperty(rimNormalStrength);
                        CopyProperty(rimBorder);
                        CopyProperty(rimBlur);
                        CopyProperty(rimFresnelPower);
                        CopyProperty(rimEnableLighting);
                        CopyProperty(rimShadowMask);
                        CopyProperty(rimBackfaceMask);
                        CopyProperty(rimVRParallaxStrength);
                        CopyProperty(rimApplyTransparency);
                        CopyProperty(rimDirStrength);
                        CopyProperty(rimDirRange);
                        CopyProperty(rimIndirRange);
                        CopyProperty(rimIndirColor);
                        CopyProperty(rimIndirBorder);
                        CopyProperty(rimIndirBlur);
                        CopyProperty(rimColorTex);
                    break;
                case lilPropertyBlock.Glitter:
                        CopyProperty(useGlitter);
                        CopyProperty(glitterUVMode);
                        CopyProperty(glitterColor);
                        CopyProperty(glitterMainStrength);
                        CopyProperty(glitterParams1);
                        CopyProperty(glitterParams2);
                        CopyProperty(glitterPostContrast);
                        CopyProperty(glitterEnableLighting);
                        CopyProperty(glitterShadowMask);
                        CopyProperty(glitterBackfaceMask);
                        CopyProperty(glitterApplyTransparency);
                        CopyProperty(glitterVRParallaxStrength);
                        CopyProperty(glitterNormalStrength);
                        CopyProperty(glitterColorTex);
                    break;
                case lilPropertyBlock.Backlight:
                        CopyProperty(useBacklight);
                        CopyProperty(backlightColor);
                        CopyProperty(backlightNormalStrength);
                        CopyProperty(backlightBorder);
                        CopyProperty(backlightBlur);
                        CopyProperty(backlightDirectivity);
                        CopyProperty(backlightViewStrength);
                        CopyProperty(backlightReceiveShadow);
                        CopyProperty(backlightBackfaceMask);
                        CopyProperty(backlightColorTex);
                    break;
                case lilPropertyBlock.Gem:
                        CopyProperty(gemChromaticAberration);
                        CopyProperty(gemEnvContrast);
                        CopyProperty(gemEnvColor);
                        CopyProperty(gemParticleLoop);
                        CopyProperty(gemParticleColor);
                        CopyProperty(gemVRParallaxStrength);
                        CopyProperty(refractionStrength);
                        CopyProperty(refractionFresnelPower);
                        CopyProperty(smoothness);
                        CopyProperty(smoothnessTex);
                    break;
                case lilPropertyBlock.Outline:
                        CopyProperty(outlineColor);
                        CopyProperty(outlineTex_ScrollRotate);
                        CopyProperty(outlineTexHSVG);
                        CopyProperty(outlineWidth);
                        CopyProperty(outlineFixWidth);
                        CopyProperty(outlineVertexR2Width);
                        CopyProperty(outlineVectorTex);
                        CopyProperty(outlineVectorScale);
                        CopyProperty(outlineEnableLighting);
                        CopyProperty(outlineTex);
                        CopyProperty(outlineWidthMask);
                    break;
                case lilPropertyBlock.Parallax:
                        CopyProperty(useParallax);
                        CopyProperty(parallax);
                        CopyProperty(parallaxOffset);
                        CopyProperty(parallaxMap);
                    break;
                case lilPropertyBlock.DistanceFade:
                        CopyProperty(distanceFadeColor);
                        CopyProperty(distanceFade);
                    break;
                case lilPropertyBlock.AudioLink:
                        CopyProperty(useAudioLink);
                        CopyProperty(audioLinkDefaultValue);
                        CopyProperty(audioLinkUVMode);
                        CopyProperty(audioLinkUVParams);
                        CopyProperty(audioLinkStart);
                        CopyProperty(audioLink2Main2nd);
                        CopyProperty(audioLink2Main3rd);
                        CopyProperty(audioLink2Emission);
                        CopyProperty(audioLink2EmissionGrad);
                        CopyProperty(audioLink2Emission2nd);
                        CopyProperty(audioLink2Emission2ndGrad);
                        CopyProperty(audioLink2Vertex);
                        CopyProperty(audioLinkVertexUVMode);
                        CopyProperty(audioLinkVertexUVParams);
                        CopyProperty(audioLinkVertexStart);
                        CopyProperty(audioLinkVertexStrength);
                        CopyProperty(audioLinkAsLocal);
                        CopyProperty(audioLinkLocalMap);
                        CopyProperty(audioLinkLocalMapParams);
                        CopyProperty(audioLinkMask);
                    break;
                case lilPropertyBlock.Dissolve:
                        CopyProperty(dissolveNoiseMask_ScrollRotate);
                        CopyProperty(dissolveNoiseStrength);
                        CopyProperty(dissolveColor);
                        CopyProperty(dissolveParams);
                        CopyProperty(dissolvePos);
                        CopyProperty(dissolveMask);
                        CopyProperty(dissolveNoiseMask);
                    break;
                case lilPropertyBlock.Refraction:
                        CopyProperty(refractionStrength);
                        CopyProperty(refractionFresnelPower);
                        CopyProperty(refractionColorFromMain);
                        CopyProperty(refractionColor);
                    break;
                case lilPropertyBlock.Fur:
                        CopyProperty(furVectorScale);
                        CopyProperty(furVector);
                        CopyProperty(furGravity);
                        CopyProperty(furRandomize);
                        CopyProperty(furAO);
                        CopyProperty(vertexColor2FurVector);
                        CopyProperty(furLayerNum);
                        CopyProperty(furRootOffset);
                        CopyProperty(furCutoutLength);
                        CopyProperty(furTouchStrength);
                        CopyProperty(furNoiseMask);
                        CopyProperty(furMask);
                        CopyProperty(furLengthMask);
                        CopyProperty(furVectorTex);
                    break;
                case lilPropertyBlock.Encryption:
                        CopyProperty(ignoreEncryption);
                        CopyProperty(keys);
                    break;
                case lilPropertyBlock.Stencil:
                        CopyProperty(stencilRef);
                        CopyProperty(stencilReadMask);
                        CopyProperty(stencilWriteMask);
                        CopyProperty(stencilComp);
                        CopyProperty(stencilPass);
                        CopyProperty(stencilFail);
                        CopyProperty(stencilZFail);
                        CopyProperty(outlineStencilRef);
                        CopyProperty(outlineStencilReadMask);
                        CopyProperty(outlineStencilWriteMask);
                        CopyProperty(outlineStencilComp);
                        CopyProperty(outlineStencilPass);
                        CopyProperty(outlineStencilFail);
                        CopyProperty(outlineStencilZFail);
                        CopyProperty(furStencilRef);
                        CopyProperty(furStencilReadMask);
                        CopyProperty(furStencilWriteMask);
                        CopyProperty(furStencilComp);
                        CopyProperty(furStencilPass);
                        CopyProperty(furStencilFail);
                        CopyProperty(furStencilZFail);
                    break;
                case lilPropertyBlock.Rendering:
                        CopyProperty(beforeExposureLimit);
                        CopyProperty(lilDirectionalLightStrength);
                        CopyProperty(cull);
                        CopyProperty(srcBlend);
                        CopyProperty(dstBlend);
                        CopyProperty(srcBlendAlpha);
                        CopyProperty(dstBlendAlpha);
                        CopyProperty(blendOp);
                        CopyProperty(blendOpAlpha);
                        CopyProperty(srcBlendFA);
                        CopyProperty(dstBlendFA);
                        CopyProperty(srcBlendAlphaFA);
                        CopyProperty(dstBlendAlphaFA);
                        CopyProperty(blendOpFA);
                        CopyProperty(blendOpAlphaFA);
                        CopyProperty(zclip);
                        CopyProperty(zwrite);
                        CopyProperty(ztest);
                        CopyProperty(offsetFactor);
                        CopyProperty(offsetUnits);
                        CopyProperty(colorMask);
                        CopyProperty(alphaToMask);
                        CopyProperty(outlineCull);
                        CopyProperty(outlineSrcBlend);
                        CopyProperty(outlineDstBlend);
                        CopyProperty(outlineSrcBlendAlpha);
                        CopyProperty(outlineDstBlendAlpha);
                        CopyProperty(outlineBlendOp);
                        CopyProperty(outlineBlendOpAlpha);
                        CopyProperty(outlineSrcBlendFA);
                        CopyProperty(outlineDstBlendFA);
                        CopyProperty(outlineSrcBlendAlphaFA);
                        CopyProperty(outlineDstBlendAlphaFA);
                        CopyProperty(outlineBlendOpFA);
                        CopyProperty(outlineBlendOpAlphaFA);
                        CopyProperty(outlineZclip);
                        CopyProperty(outlineZwrite);
                        CopyProperty(outlineZtest);
                        CopyProperty(outlineOffsetFactor);
                        CopyProperty(outlineOffsetUnits);
                        CopyProperty(outlineColorMask);
                        CopyProperty(outlineAlphaToMask);
                        CopyProperty(furCull);
                        CopyProperty(furSrcBlend);
                        CopyProperty(furDstBlend);
                        CopyProperty(furSrcBlendAlpha);
                        CopyProperty(furDstBlendAlpha);
                        CopyProperty(furBlendOp);
                        CopyProperty(furBlendOpAlpha);
                        CopyProperty(furSrcBlendFA);
                        CopyProperty(furDstBlendFA);
                        CopyProperty(furSrcBlendAlphaFA);
                        CopyProperty(furDstBlendAlphaFA);
                        CopyProperty(furBlendOpFA);
                        CopyProperty(furBlendOpAlphaFA);
                        CopyProperty(furZclip);
                        CopyProperty(furZwrite);
                        CopyProperty(furZtest);
                        CopyProperty(furOffsetFactor);
                        CopyProperty(furOffsetUnits);
                        CopyProperty(furColorMask);
                        CopyProperty(furAlphaToMask);
                    break;
                case lilPropertyBlock.Tessellation:
                        CopyProperty(tessEdge);
                        CopyProperty(tessStrength);
                        CopyProperty(tessShrink);
                        CopyProperty(tessFactorMax);
                    break;
            }
        }

        private void PasteProperties(lilPropertyBlock propertyBlock, bool shouldCopyTex)
        {
            switch(propertyBlock)
            {
                case lilPropertyBlock.Base:
                        PasteProperty(ref invisible);
                        PasteProperty(ref cutoff);
                        PasteProperty(ref subpassCutoff);
                        PasteProperty(ref cull);
                        PasteProperty(ref flipNormal);
                        PasteProperty(ref backfaceForceShadow);
                        PasteProperty(ref zwrite);
                        PasteProperty(ref asUnlit);
                        PasteProperty(ref vertexLightStrength);
                        PasteProperty(ref lightMinLimit);
                        PasteProperty(ref lightMaxLimit);
                        PasteProperty(ref monochromeLighting);
                        PasteProperty(ref alphaBoostFA);
                        PasteProperty(ref shadowEnvStrength);
                        PasteProperty(ref fakeShadowVector);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref triMask);
                        }
                    break;
                case lilPropertyBlock.Lighting:
                        PasteProperty(ref asUnlit);
                        PasteProperty(ref vertexLightStrength);
                        PasteProperty(ref lightMinLimit);
                        PasteProperty(ref lightMaxLimit);
                        PasteProperty(ref monochromeLighting);
                        PasteProperty(ref alphaBoostFA);
                        PasteProperty(ref shadowEnvStrength);
                    break;
                case lilPropertyBlock.UV:
                        PasteProperty(ref mainTex_ScrollRotate);
                        PasteProperty(ref shiftBackfaceUV);
                    break;
                case lilPropertyBlock.MainColor:
                        PasteProperty(ref mainColor);
                        PasteProperty(ref mainTexHSVG);
                        PasteProperty(ref mainGradationStrength);
                        PasteProperty(ref mainGradationTex);
                        PasteProperty(ref useMain2ndTex);
                        PasteProperty(ref mainColor2nd);
                        PasteProperty(ref main2ndTex_UVMode);
                        PasteProperty(ref main2ndTexAngle);
                        PasteProperty(ref main2ndTexDecalAnimation);
                        PasteProperty(ref main2ndTexDecalSubParam);
                        PasteProperty(ref main2ndTexIsDecal);
                        PasteProperty(ref main2ndTexIsLeftOnly);
                        PasteProperty(ref main2ndTexIsRightOnly);
                        PasteProperty(ref main2ndTexShouldCopy);
                        PasteProperty(ref main2ndTexShouldFlipMirror);
                        PasteProperty(ref main2ndTexShouldFlipCopy);
                        PasteProperty(ref main2ndTexIsMSDF);
                        PasteProperty(ref main2ndTexBlendMode);
                        PasteProperty(ref main2ndEnableLighting);
                        PasteProperty(ref main2ndDissolveNoiseMask_ScrollRotate);
                        PasteProperty(ref main2ndDissolveNoiseStrength);
                        PasteProperty(ref main2ndDissolveColor);
                        PasteProperty(ref main2ndDissolveParams);
                        PasteProperty(ref main2ndDissolvePos);
                        PasteProperty(ref main2ndDistanceFade);
                        PasteProperty(ref useMain3rdTex);
                        PasteProperty(ref mainColor3rd);
                        PasteProperty(ref main3rdTex_UVMode);
                        PasteProperty(ref main3rdTexAngle);
                        PasteProperty(ref main3rdTexDecalAnimation);
                        PasteProperty(ref main3rdTexDecalSubParam);
                        PasteProperty(ref main3rdTexIsDecal);
                        PasteProperty(ref main3rdTexIsLeftOnly);
                        PasteProperty(ref main3rdTexIsRightOnly);
                        PasteProperty(ref main3rdTexShouldCopy);
                        PasteProperty(ref main3rdTexShouldFlipMirror);
                        PasteProperty(ref main3rdTexShouldFlipCopy);
                        PasteProperty(ref main3rdTexIsMSDF);
                        PasteProperty(ref main3rdTexBlendMode);
                        PasteProperty(ref main3rdEnableLighting);
                        PasteProperty(ref main3rdDissolveMask);
                        PasteProperty(ref main3rdDissolveNoiseMask);
                        PasteProperty(ref main3rdDissolveNoiseMask_ScrollRotate);
                        PasteProperty(ref main3rdDissolveNoiseStrength);
                        PasteProperty(ref main3rdDissolveColor);
                        PasteProperty(ref main3rdDissolveParams);
                        PasteProperty(ref main3rdDissolvePos);
                        PasteProperty(ref main3rdDistanceFade);
                        PasteProperty(ref alphaMaskMode);
                        PasteProperty(ref alphaMaskScale);
                        PasteProperty(ref alphaMaskValue);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref mainTex);
                            PasteProperty(ref mainColorAdjustMask);
                            PasteProperty(ref main2ndTex);
                            PasteProperty(ref main2ndBlendMask);
                            PasteProperty(ref main2ndDissolveMask);
                            PasteProperty(ref main2ndDissolveNoiseMask);
                            PasteProperty(ref main3rdTex);
                            PasteProperty(ref main3rdBlendMask);
                            PasteProperty(ref main3rdDissolveMask);
                            PasteProperty(ref main3rdDissolveNoiseMask);
                            PasteProperty(ref alphaMask);
                        }
                    break;
                case lilPropertyBlock.MainColor1st:
                        PasteProperty(ref mainColor);
                        PasteProperty(ref mainTexHSVG);
                        PasteProperty(ref mainGradationStrength);
                        PasteProperty(ref mainGradationTex);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref mainTex);
                            PasteProperty(ref mainColorAdjustMask);
                        }
                    break;
                case lilPropertyBlock.MainColor2nd:
                        PasteProperty(ref useMain2ndTex);
                        PasteProperty(ref mainColor2nd);
                        PasteProperty(ref main2ndTex_UVMode);
                        PasteProperty(ref main2ndTexAngle);
                        PasteProperty(ref main2ndTexDecalAnimation);
                        PasteProperty(ref main2ndTexDecalSubParam);
                        PasteProperty(ref main2ndTexIsDecal);
                        PasteProperty(ref main2ndTexIsLeftOnly);
                        PasteProperty(ref main2ndTexIsRightOnly);
                        PasteProperty(ref main2ndTexShouldCopy);
                        PasteProperty(ref main2ndTexShouldFlipMirror);
                        PasteProperty(ref main2ndTexShouldFlipCopy);
                        PasteProperty(ref main2ndTexIsMSDF);
                        PasteProperty(ref main2ndTexBlendMode);
                        PasteProperty(ref main2ndEnableLighting);
                        PasteProperty(ref main2ndDissolveNoiseMask_ScrollRotate);
                        PasteProperty(ref main2ndDissolveNoiseStrength);
                        PasteProperty(ref main2ndDissolveColor);
                        PasteProperty(ref main2ndDissolveParams);
                        PasteProperty(ref main2ndDissolvePos);
                        PasteProperty(ref main2ndDistanceFade);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref main2ndTex);
                            PasteProperty(ref main2ndBlendMask);
                            PasteProperty(ref main2ndDissolveMask);
                            PasteProperty(ref main2ndDissolveNoiseMask);
                        }
                    break;
                case lilPropertyBlock.MainColor3rd:
                        PasteProperty(ref useMain3rdTex);
                        PasteProperty(ref mainColor3rd);
                        PasteProperty(ref main3rdTex_UVMode);
                        PasteProperty(ref main3rdTexAngle);
                        PasteProperty(ref main3rdTexDecalAnimation);
                        PasteProperty(ref main3rdTexDecalSubParam);
                        PasteProperty(ref main3rdTexIsDecal);
                        PasteProperty(ref main3rdTexIsLeftOnly);
                        PasteProperty(ref main3rdTexIsRightOnly);
                        PasteProperty(ref main3rdTexShouldCopy);
                        PasteProperty(ref main3rdTexShouldFlipMirror);
                        PasteProperty(ref main3rdTexShouldFlipCopy);
                        PasteProperty(ref main3rdTexIsMSDF);
                        PasteProperty(ref main3rdTexBlendMode);
                        PasteProperty(ref main3rdEnableLighting);
                        PasteProperty(ref main3rdDissolveMask);
                        PasteProperty(ref main3rdDissolveNoiseMask);
                        PasteProperty(ref main3rdDissolveNoiseMask_ScrollRotate);
                        PasteProperty(ref main3rdDissolveNoiseStrength);
                        PasteProperty(ref main3rdDissolveColor);
                        PasteProperty(ref main3rdDissolveParams);
                        PasteProperty(ref main3rdDissolvePos);
                        PasteProperty(ref main3rdDistanceFade);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref main3rdTex);
                            PasteProperty(ref main3rdBlendMask);
                            PasteProperty(ref main3rdDissolveMask);
                            PasteProperty(ref main3rdDissolveNoiseMask);
                        }
                    break;
                case lilPropertyBlock.AlphaMask:
                        PasteProperty(ref alphaMaskMode);
                        PasteProperty(ref alphaMaskScale);
                        PasteProperty(ref alphaMaskValue);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref alphaMask);
                        }
                    break;
                case lilPropertyBlock.Shadow:
                        PasteProperty(ref useShadow);
                        PasteProperty(ref shadowColor);
                        PasteProperty(ref shadowNormalStrength);
                        PasteProperty(ref shadowBorder);
                        PasteProperty(ref shadowBlur);
                        PasteProperty(ref shadowStrength);
                        PasteProperty(ref shadowAOShift);
                        PasteProperty(ref shadowAOShift2);
                        PasteProperty(ref shadowPostAO);
                        PasteProperty(ref shadow2ndColor);
                        PasteProperty(ref shadow2ndNormalStrength);
                        PasteProperty(ref shadow2ndBorder);
                        PasteProperty(ref shadow2ndBlur);
                        PasteProperty(ref shadow3rdColor);
                        PasteProperty(ref shadow3rdNormalStrength);
                        PasteProperty(ref shadow3rdBorder);
                        PasteProperty(ref shadow3rdBlur);
                        PasteProperty(ref shadowMainStrength);
                        PasteProperty(ref shadowEnvStrength);
                        PasteProperty(ref shadowBorderColor);
                        PasteProperty(ref shadowBorderRange);
                        PasteProperty(ref shadowReceive);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref shadowBorderMask);
                            PasteProperty(ref shadowBlurMask);
                            PasteProperty(ref shadowStrengthMask);
                            PasteProperty(ref shadowColorTex);
                            PasteProperty(ref shadow2ndColorTex);
                            PasteProperty(ref shadow3rdColorTex);
                        }
                    break;
                case lilPropertyBlock.Emission:
                        PasteProperty(ref useEmission);
                        PasteProperty(ref emissionColor);
                        PasteProperty(ref emissionMap_ScrollRotate);
                        PasteProperty(ref emissionMap_UVMode);
                        PasteProperty(ref emissionBlend);
                        PasteProperty(ref emissionBlendMask_ScrollRotate);
                        PasteProperty(ref emissionBlink);
                        PasteProperty(ref emissionUseGrad);
                        PasteProperty(ref emissionGradTex);
                        PasteProperty(ref emissionGradSpeed);
                        PasteProperty(ref emissionParallaxDepth);
                        PasteProperty(ref emissionFluorescence);
                        PasteProperty(ref useEmission2nd);
                        PasteProperty(ref emission2ndColor);
                        PasteProperty(ref emission2ndMap_ScrollRotate);
                        PasteProperty(ref emission2ndMap_UVMode);
                        PasteProperty(ref emission2ndBlend);
                        PasteProperty(ref emission2ndBlendMask_ScrollRotate);
                        PasteProperty(ref emission2ndBlink);
                        PasteProperty(ref emission2ndUseGrad);
                        PasteProperty(ref emission2ndGradTex);
                        PasteProperty(ref emission2ndGradSpeed);
                        PasteProperty(ref emission2ndParallaxDepth);
                        PasteProperty(ref emission2ndFluorescence);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref emissionMap);
                            PasteProperty(ref emissionBlendMask);
                            PasteProperty(ref emission2ndMap);
                            PasteProperty(ref emission2ndBlendMask);
                        }
                    break;
                case lilPropertyBlock.Emission1st:
                        PasteProperty(ref useEmission);
                        PasteProperty(ref emissionColor);
                        PasteProperty(ref emissionMap_ScrollRotate);
                        PasteProperty(ref emissionMap_UVMode);
                        PasteProperty(ref emissionBlend);
                        PasteProperty(ref emissionBlendMask_ScrollRotate);
                        PasteProperty(ref emissionBlink);
                        PasteProperty(ref emissionUseGrad);
                        PasteProperty(ref emissionGradTex);
                        PasteProperty(ref emissionGradSpeed);
                        PasteProperty(ref emissionParallaxDepth);
                        PasteProperty(ref emissionFluorescence);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref emissionMap);
                            PasteProperty(ref emissionBlendMask);
                        }
                    break;
                case lilPropertyBlock.Emission2nd:
                        PasteProperty(ref useEmission2nd);
                        PasteProperty(ref emission2ndColor);
                        PasteProperty(ref emission2ndMap_ScrollRotate);
                        PasteProperty(ref emission2ndMap_UVMode);
                        PasteProperty(ref emission2ndBlend);
                        PasteProperty(ref emission2ndBlendMask_ScrollRotate);
                        PasteProperty(ref emission2ndBlink);
                        PasteProperty(ref emission2ndUseGrad);
                        PasteProperty(ref emission2ndGradTex);
                        PasteProperty(ref emission2ndGradSpeed);
                        PasteProperty(ref emission2ndParallaxDepth);
                        PasteProperty(ref emission2ndFluorescence);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref emission2ndMap);
                            PasteProperty(ref emission2ndBlendMask);
                        }
                    break;
                case lilPropertyBlock.NormalMap:
                        PasteProperty(ref useBumpMap);
                        PasteProperty(ref bumpScale);
                        PasteProperty(ref useBump2ndMap);
                        PasteProperty(ref bump2ndScale);
                        PasteProperty(ref useAnisotropy);
                        PasteProperty(ref anisotropyScale);
                        PasteProperty(ref anisotropyTangentWidth);
                        PasteProperty(ref anisotropyBitangentWidth);
                        PasteProperty(ref anisotropyShift);
                        PasteProperty(ref anisotropyShiftNoiseScale);
                        PasteProperty(ref anisotropySpecularStrength);
                        PasteProperty(ref anisotropy2ndTangentWidth);
                        PasteProperty(ref anisotropy2ndBitangentWidth);
                        PasteProperty(ref anisotropy2ndShift);
                        PasteProperty(ref anisotropy2ndShiftNoiseScale);
                        PasteProperty(ref anisotropy2ndSpecularStrength);
                        PasteProperty(ref anisotropy2Reflection);
                        PasteProperty(ref anisotropy2MatCap);
                        PasteProperty(ref anisotropy2MatCap2nd);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref bumpMap);
                            PasteProperty(ref bump2ndMap);
                            PasteProperty(ref bump2ndScaleMask);
                            PasteProperty(ref anisotropyTangentMap);
                            PasteProperty(ref anisotropyScaleMask);
                            PasteProperty(ref anisotropyShiftNoiseMask);
                        }
                    break;
                case lilPropertyBlock.NormalMap1st:
                        PasteProperty(ref useBumpMap);
                        PasteProperty(ref bumpScale);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref bumpMap);
                        }
                    break;
                case lilPropertyBlock.NormalMap2nd:
                        PasteProperty(ref useBump2ndMap);
                        PasteProperty(ref bump2ndScale);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref bump2ndMap);
                            PasteProperty(ref bump2ndScaleMask);
                        }
                    break;
                case lilPropertyBlock.Anisotropy:
                        PasteProperty(ref useAnisotropy);
                        PasteProperty(ref anisotropyScale);
                        PasteProperty(ref anisotropyTangentWidth);
                        PasteProperty(ref anisotropyBitangentWidth);
                        PasteProperty(ref anisotropyShift);
                        PasteProperty(ref anisotropyShiftNoiseScale);
                        PasteProperty(ref anisotropySpecularStrength);
                        PasteProperty(ref anisotropy2ndTangentWidth);
                        PasteProperty(ref anisotropy2ndBitangentWidth);
                        PasteProperty(ref anisotropy2ndShift);
                        PasteProperty(ref anisotropy2ndShiftNoiseScale);
                        PasteProperty(ref anisotropy2ndSpecularStrength);
                        PasteProperty(ref anisotropy2Reflection);
                        PasteProperty(ref anisotropy2MatCap);
                        PasteProperty(ref anisotropy2MatCap2nd);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref anisotropyTangentMap);
                            PasteProperty(ref anisotropyScaleMask);
                            PasteProperty(ref anisotropyShiftNoiseMask);
                        }
                    break;
                case lilPropertyBlock.Reflections:
                        PasteProperty(ref useReflection);
                        PasteProperty(ref metallic);
                        PasteProperty(ref smoothness);
                        PasteProperty(ref reflectance);
                        PasteProperty(ref reflectionColor);
                        PasteProperty(ref applySpecular);
                        PasteProperty(ref applySpecularFA);
                        PasteProperty(ref specularNormalStrength);
                        PasteProperty(ref specularToon);
                        PasteProperty(ref specularBorder);
                        PasteProperty(ref specularBlur);
                        PasteProperty(ref applyReflection);
                        PasteProperty(ref reflectionNormalStrength);
                        PasteProperty(ref reflectionApplyTransparency);
                        PasteProperty(ref reflectionCubeColor);
                        PasteProperty(ref reflectionCubeOverride);
                        PasteProperty(ref reflectionCubeEnableLighting);
                        PasteProperty(ref useMatCap);
                        PasteProperty(ref matcapColor);
                        PasteProperty(ref matcapBlendUV1);
                        PasteProperty(ref matcapZRotCancel);
                        PasteProperty(ref matcapPerspective);
                        PasteProperty(ref matcapVRParallaxStrength);
                        PasteProperty(ref matcapBlend);
                        PasteProperty(ref matcapEnableLighting);
                        PasteProperty(ref matcapShadowMask);
                        PasteProperty(ref matcapBackfaceMask);
                        PasteProperty(ref matcapLod);
                        PasteProperty(ref matcapBlendMode);
                        PasteProperty(ref matcapMul);
                        PasteProperty(ref matcapApplyTransparency);
                        PasteProperty(ref matcapNormalStrength);
                        PasteProperty(ref matcapCustomNormal);
                        PasteProperty(ref matcapBumpScale);
                        PasteProperty(ref useMatCap2nd);
                        PasteProperty(ref matcap2ndColor);
                        PasteProperty(ref matcap2ndBlendUV1);
                        PasteProperty(ref matcap2ndZRotCancel);
                        PasteProperty(ref matcap2ndPerspective);
                        PasteProperty(ref matcap2ndVRParallaxStrength);
                        PasteProperty(ref matcap2ndBlend);
                        PasteProperty(ref matcap2ndEnableLighting);
                        PasteProperty(ref matcap2ndShadowMask);
                        PasteProperty(ref matcap2ndBackfaceMask);
                        PasteProperty(ref matcap2ndLod);
                        PasteProperty(ref matcap2ndBlendMode);
                        PasteProperty(ref matcap2ndMul);
                        PasteProperty(ref matcap2ndNormalStrength);
                        PasteProperty(ref matcap2ndApplyTransparency);
                        PasteProperty(ref matcap2ndCustomNormal);
                        PasteProperty(ref matcap2ndBumpScale);
                        PasteProperty(ref useRim);
                        PasteProperty(ref rimColor);
                        PasteProperty(ref rimNormalStrength);
                        PasteProperty(ref rimBorder);
                        PasteProperty(ref rimBlur);
                        PasteProperty(ref rimFresnelPower);
                        PasteProperty(ref rimEnableLighting);
                        PasteProperty(ref rimShadowMask);
                        PasteProperty(ref rimBackfaceMask);
                        PasteProperty(ref rimVRParallaxStrength);
                        PasteProperty(ref rimApplyTransparency);
                        PasteProperty(ref rimDirStrength);
                        PasteProperty(ref rimDirRange);
                        PasteProperty(ref rimIndirRange);
                        PasteProperty(ref rimIndirColor);
                        PasteProperty(ref rimIndirBorder);
                        PasteProperty(ref rimIndirBlur);
                        PasteProperty(ref useGlitter);
                        PasteProperty(ref glitterUVMode);
                        PasteProperty(ref glitterColor);
                        PasteProperty(ref glitterMainStrength);
                        PasteProperty(ref glitterParams1);
                        PasteProperty(ref glitterParams2);
                        PasteProperty(ref glitterPostContrast);
                        PasteProperty(ref glitterEnableLighting);
                        PasteProperty(ref glitterShadowMask);
                        PasteProperty(ref glitterBackfaceMask);
                        PasteProperty(ref glitterApplyTransparency);
                        PasteProperty(ref glitterVRParallaxStrength);
                        PasteProperty(ref glitterNormalStrength);
                        PasteProperty(ref useBacklight);
                        PasteProperty(ref backlightColor);
                        PasteProperty(ref backlightNormalStrength);
                        PasteProperty(ref backlightBorder);
                        PasteProperty(ref backlightBlur);
                        PasteProperty(ref backlightDirectivity);
                        PasteProperty(ref backlightViewStrength);
                        PasteProperty(ref backlightReceiveShadow);
                        PasteProperty(ref backlightBackfaceMask);
                        PasteProperty(ref gemChromaticAberration);
                        PasteProperty(ref gemEnvContrast);
                        PasteProperty(ref gemEnvColor);
                        PasteProperty(ref gemParticleLoop);
                        PasteProperty(ref gemParticleColor);
                        PasteProperty(ref gemVRParallaxStrength);
                        PasteProperty(ref refractionStrength);
                        PasteProperty(ref refractionFresnelPower);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref metallicGlossMap);
                            PasteProperty(ref smoothnessTex);
                            PasteProperty(ref reflectionColorTex);
                            PasteProperty(ref reflectionCubeTex);
                            PasteProperty(ref matcapTex);
                            PasteProperty(ref matcapBlendMask);
                            PasteProperty(ref matcapBumpMap);
                            PasteProperty(ref matcap2ndTex);
                            PasteProperty(ref matcap2ndBlendMask);
                            PasteProperty(ref matcap2ndBumpMap);
                            PasteProperty(ref rimColorTex);
                            PasteProperty(ref glitterColorTex);
                            PasteProperty(ref backlightColorTex);
                        }
                    break;
                case lilPropertyBlock.Reflection:
                        PasteProperty(ref useReflection);
                        PasteProperty(ref metallic);
                        PasteProperty(ref smoothness);
                        PasteProperty(ref reflectance);
                        PasteProperty(ref reflectionColor);
                        PasteProperty(ref applySpecular);
                        PasteProperty(ref applySpecularFA);
                        PasteProperty(ref specularNormalStrength);
                        PasteProperty(ref specularToon);
                        PasteProperty(ref specularBorder);
                        PasteProperty(ref specularBlur);
                        PasteProperty(ref applyReflection);
                        PasteProperty(ref reflectionNormalStrength);
                        PasteProperty(ref reflectionApplyTransparency);
                        PasteProperty(ref reflectionCubeColor);
                        PasteProperty(ref reflectionCubeOverride);
                        PasteProperty(ref reflectionCubeEnableLighting);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref metallicGlossMap);
                            PasteProperty(ref smoothnessTex);
                            PasteProperty(ref reflectionColorTex);
                            PasteProperty(ref reflectionCubeTex);
                        }
                    break;
                case lilPropertyBlock.MatCaps:
                        PasteProperty(ref useMatCap);
                        PasteProperty(ref matcapColor);
                        PasteProperty(ref matcapBlendUV1);
                        PasteProperty(ref matcapZRotCancel);
                        PasteProperty(ref matcapPerspective);
                        PasteProperty(ref matcapVRParallaxStrength);
                        PasteProperty(ref matcapBlend);
                        PasteProperty(ref matcapEnableLighting);
                        PasteProperty(ref matcapShadowMask);
                        PasteProperty(ref matcapBackfaceMask);
                        PasteProperty(ref matcapLod);
                        PasteProperty(ref matcapBlendMode);
                        PasteProperty(ref matcapMul);
                        PasteProperty(ref matcapApplyTransparency);
                        PasteProperty(ref matcapNormalStrength);
                        PasteProperty(ref matcapCustomNormal);
                        PasteProperty(ref matcapBumpScale);
                        PasteProperty(ref useMatCap2nd);
                        PasteProperty(ref matcap2ndColor);
                        PasteProperty(ref matcap2ndBlendUV1);
                        PasteProperty(ref matcap2ndZRotCancel);
                        PasteProperty(ref matcap2ndPerspective);
                        PasteProperty(ref matcap2ndVRParallaxStrength);
                        PasteProperty(ref matcap2ndBlend);
                        PasteProperty(ref matcap2ndEnableLighting);
                        PasteProperty(ref matcap2ndShadowMask);
                        PasteProperty(ref matcap2ndBackfaceMask);
                        PasteProperty(ref matcap2ndLod);
                        PasteProperty(ref matcap2ndBlendMode);
                        PasteProperty(ref matcap2ndMul);
                        PasteProperty(ref matcap2ndNormalStrength);
                        PasteProperty(ref matcap2ndApplyTransparency);
                        PasteProperty(ref matcap2ndCustomNormal);
                        PasteProperty(ref matcap2ndBumpScale);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref matcapTex);
                            PasteProperty(ref matcapBlendMask);
                            PasteProperty(ref matcapBumpMap);
                            PasteProperty(ref matcap2ndTex);
                            PasteProperty(ref matcap2ndBlendMask);
                            PasteProperty(ref matcap2ndBumpMap);
                        }
                    break;
                case lilPropertyBlock.MatCap1st:
                        PasteProperty(ref useMatCap);
                        PasteProperty(ref matcapColor);
                        PasteProperty(ref matcapBlendUV1);
                        PasteProperty(ref matcapZRotCancel);
                        PasteProperty(ref matcapPerspective);
                        PasteProperty(ref matcapVRParallaxStrength);
                        PasteProperty(ref matcapBlend);
                        PasteProperty(ref matcapEnableLighting);
                        PasteProperty(ref matcapShadowMask);
                        PasteProperty(ref matcapBackfaceMask);
                        PasteProperty(ref matcapLod);
                        PasteProperty(ref matcapBlendMode);
                        PasteProperty(ref matcapMul);
                        PasteProperty(ref matcapApplyTransparency);
                        PasteProperty(ref matcapNormalStrength);
                        PasteProperty(ref matcapCustomNormal);
                        PasteProperty(ref matcapBumpScale);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref matcapTex);
                            PasteProperty(ref matcapBlendMask);
                            PasteProperty(ref matcapBumpMap);
                        }
                    break;
                case lilPropertyBlock.MatCap2nd:
                        PasteProperty(ref useMatCap2nd);
                        PasteProperty(ref matcap2ndColor);
                        PasteProperty(ref matcap2ndBlendUV1);
                        PasteProperty(ref matcap2ndZRotCancel);
                        PasteProperty(ref matcap2ndPerspective);
                        PasteProperty(ref matcap2ndVRParallaxStrength);
                        PasteProperty(ref matcap2ndBlend);
                        PasteProperty(ref matcap2ndEnableLighting);
                        PasteProperty(ref matcap2ndShadowMask);
                        PasteProperty(ref matcap2ndBackfaceMask);
                        PasteProperty(ref matcap2ndLod);
                        PasteProperty(ref matcap2ndBlendMode);
                        PasteProperty(ref matcap2ndMul);
                        PasteProperty(ref matcap2ndApplyTransparency);
                        PasteProperty(ref matcap2ndNormalStrength);
                        PasteProperty(ref matcap2ndCustomNormal);
                        PasteProperty(ref matcap2ndBumpScale);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref matcap2ndTex);
                            PasteProperty(ref matcap2ndBlendMask);
                            PasteProperty(ref matcap2ndBumpMap);
                        }
                    break;
                case lilPropertyBlock.RimLight:
                        PasteProperty(ref useRim);
                        PasteProperty(ref rimColor);
                        PasteProperty(ref rimNormalStrength);
                        PasteProperty(ref rimBorder);
                        PasteProperty(ref rimBlur);
                        PasteProperty(ref rimFresnelPower);
                        PasteProperty(ref rimEnableLighting);
                        PasteProperty(ref rimShadowMask);
                        PasteProperty(ref rimBackfaceMask);
                        PasteProperty(ref rimVRParallaxStrength);
                        PasteProperty(ref rimApplyTransparency);
                        PasteProperty(ref rimDirStrength);
                        PasteProperty(ref rimDirRange);
                        PasteProperty(ref rimIndirRange);
                        PasteProperty(ref rimIndirColor);
                        PasteProperty(ref rimIndirBorder);
                        PasteProperty(ref rimIndirBlur);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref rimColorTex);
                        }
                    break;
                case lilPropertyBlock.Glitter:
                        PasteProperty(ref useGlitter);
                        PasteProperty(ref glitterUVMode);
                        PasteProperty(ref glitterColor);
                        PasteProperty(ref glitterMainStrength);
                        PasteProperty(ref glitterParams1);
                        PasteProperty(ref glitterParams2);
                        PasteProperty(ref glitterPostContrast);
                        PasteProperty(ref glitterEnableLighting);
                        PasteProperty(ref glitterShadowMask);
                        PasteProperty(ref glitterBackfaceMask);
                        PasteProperty(ref glitterApplyTransparency);
                        PasteProperty(ref glitterVRParallaxStrength);
                        PasteProperty(ref glitterNormalStrength);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref glitterColorTex);
                        }
                    break;
                case lilPropertyBlock.Backlight:
                        PasteProperty(ref useBacklight);
                        PasteProperty(ref backlightColor);
                        PasteProperty(ref backlightNormalStrength);
                        PasteProperty(ref backlightBorder);
                        PasteProperty(ref backlightBlur);
                        PasteProperty(ref backlightDirectivity);
                        PasteProperty(ref backlightViewStrength);
                        PasteProperty(ref backlightReceiveShadow);
                        PasteProperty(ref backlightBackfaceMask);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref backlightColorTex);
                        }
                    break;
                case lilPropertyBlock.Gem:
                        PasteProperty(ref gemChromaticAberration);
                        PasteProperty(ref gemEnvContrast);
                        PasteProperty(ref gemEnvColor);
                        PasteProperty(ref gemParticleLoop);
                        PasteProperty(ref gemParticleColor);
                        PasteProperty(ref gemVRParallaxStrength);
                        PasteProperty(ref refractionStrength);
                        PasteProperty(ref refractionFresnelPower);
                        PasteProperty(ref smoothness);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref smoothnessTex);
                        }
                    break;
                case lilPropertyBlock.Outline:
                        PasteProperty(ref outlineColor);
                        PasteProperty(ref outlineTex_ScrollRotate);
                        PasteProperty(ref outlineTexHSVG);
                        PasteProperty(ref outlineWidth);
                        PasteProperty(ref outlineFixWidth);
                        PasteProperty(ref outlineVertexR2Width);
                        PasteProperty(ref outlineVectorTex);
                        PasteProperty(ref outlineVectorScale);
                        PasteProperty(ref outlineEnableLighting);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref outlineTex);
                            PasteProperty(ref outlineWidthMask);
                        }
                    break;
                case lilPropertyBlock.Parallax:
                        PasteProperty(ref useParallax);
                        PasteProperty(ref parallax);
                        PasteProperty(ref parallaxOffset);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref parallaxMap);
                        }
                    break;
                case lilPropertyBlock.DistanceFade:
                        PasteProperty(ref distanceFadeColor);
                        PasteProperty(ref distanceFade);
                    break;
                case lilPropertyBlock.AudioLink:
                        PasteProperty(ref useAudioLink);
                        PasteProperty(ref audioLinkDefaultValue);
                        PasteProperty(ref audioLinkUVMode);
                        PasteProperty(ref audioLinkUVParams);
                        PasteProperty(ref audioLinkStart);
                        PasteProperty(ref audioLink2Main2nd);
                        PasteProperty(ref audioLink2Main3rd);
                        PasteProperty(ref audioLink2Emission);
                        PasteProperty(ref audioLink2EmissionGrad);
                        PasteProperty(ref audioLink2Emission2nd);
                        PasteProperty(ref audioLink2Emission2ndGrad);
                        PasteProperty(ref audioLink2Vertex);
                        PasteProperty(ref audioLinkVertexUVMode);
                        PasteProperty(ref audioLinkVertexUVParams);
                        PasteProperty(ref audioLinkVertexStart);
                        PasteProperty(ref audioLinkVertexStrength);
                        PasteProperty(ref audioLinkAsLocal);
                        PasteProperty(ref audioLinkLocalMap);
                        PasteProperty(ref audioLinkLocalMapParams);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref audioLinkMask);
                        }
                    break;
                case lilPropertyBlock.Dissolve:
                        PasteProperty(ref dissolveNoiseMask_ScrollRotate);
                        PasteProperty(ref dissolveNoiseStrength);
                        PasteProperty(ref dissolveColor);
                        PasteProperty(ref dissolveParams);
                        PasteProperty(ref dissolvePos);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref dissolveMask);
                            PasteProperty(ref dissolveNoiseMask);
                        }
                    break;
                case lilPropertyBlock.Refraction:
                        PasteProperty(ref refractionStrength);
                        PasteProperty(ref refractionFresnelPower);
                        PasteProperty(ref refractionColorFromMain);
                        PasteProperty(ref refractionColor);
                    break;
                case lilPropertyBlock.Fur:
                        PasteProperty(ref furVectorScale);
                        PasteProperty(ref furVector);
                        PasteProperty(ref furGravity);
                        PasteProperty(ref furRandomize);
                        PasteProperty(ref furAO);
                        PasteProperty(ref vertexColor2FurVector);
                        PasteProperty(ref furLayerNum);
                        PasteProperty(ref furRootOffset);
                        PasteProperty(ref furCutoutLength);
                        PasteProperty(ref furTouchStrength);
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref furNoiseMask);
                            PasteProperty(ref furMask);
                            PasteProperty(ref furLengthMask);
                            PasteProperty(ref furVectorTex);
                        }
                    break;
                case lilPropertyBlock.Encryption:
                        PasteProperty(ref ignoreEncryption);
                        PasteProperty(ref keys);
                    break;
                case lilPropertyBlock.Stencil:
                        PasteProperty(ref stencilRef);
                        PasteProperty(ref stencilReadMask);
                        PasteProperty(ref stencilWriteMask);
                        PasteProperty(ref stencilComp);
                        PasteProperty(ref stencilPass);
                        PasteProperty(ref stencilFail);
                        PasteProperty(ref stencilZFail);
                        PasteProperty(ref outlineStencilRef);
                        PasteProperty(ref outlineStencilReadMask);
                        PasteProperty(ref outlineStencilWriteMask);
                        PasteProperty(ref outlineStencilComp);
                        PasteProperty(ref outlineStencilPass);
                        PasteProperty(ref outlineStencilFail);
                        PasteProperty(ref outlineStencilZFail);
                        PasteProperty(ref furStencilRef);
                        PasteProperty(ref furStencilReadMask);
                        PasteProperty(ref furStencilWriteMask);
                        PasteProperty(ref furStencilComp);
                        PasteProperty(ref furStencilPass);
                        PasteProperty(ref furStencilFail);
                        PasteProperty(ref furStencilZFail);
                    break;
                case lilPropertyBlock.Rendering:
                        PasteProperty(ref beforeExposureLimit);
                        PasteProperty(ref lilDirectionalLightStrength);
                        PasteProperty(ref cull);
                        PasteProperty(ref srcBlend);
                        PasteProperty(ref dstBlend);
                        PasteProperty(ref srcBlendAlpha);
                        PasteProperty(ref dstBlendAlpha);
                        PasteProperty(ref blendOp);
                        PasteProperty(ref blendOpAlpha);
                        PasteProperty(ref srcBlendFA);
                        PasteProperty(ref dstBlendFA);
                        PasteProperty(ref srcBlendAlphaFA);
                        PasteProperty(ref dstBlendAlphaFA);
                        PasteProperty(ref blendOpFA);
                        PasteProperty(ref blendOpAlphaFA);
                        PasteProperty(ref zclip);
                        PasteProperty(ref zwrite);
                        PasteProperty(ref ztest);
                        PasteProperty(ref offsetFactor);
                        PasteProperty(ref offsetUnits);
                        PasteProperty(ref colorMask);
                        PasteProperty(ref alphaToMask);
                        PasteProperty(ref outlineCull);
                        PasteProperty(ref outlineSrcBlend);
                        PasteProperty(ref outlineDstBlend);
                        PasteProperty(ref outlineSrcBlendAlpha);
                        PasteProperty(ref outlineDstBlendAlpha);
                        PasteProperty(ref outlineBlendOp);
                        PasteProperty(ref outlineBlendOpAlpha);
                        PasteProperty(ref outlineSrcBlendFA);
                        PasteProperty(ref outlineDstBlendFA);
                        PasteProperty(ref outlineSrcBlendAlphaFA);
                        PasteProperty(ref outlineDstBlendAlphaFA);
                        PasteProperty(ref outlineBlendOpFA);
                        PasteProperty(ref outlineBlendOpAlphaFA);
                        PasteProperty(ref outlineZclip);
                        PasteProperty(ref outlineZwrite);
                        PasteProperty(ref outlineZtest);
                        PasteProperty(ref outlineOffsetFactor);
                        PasteProperty(ref outlineOffsetUnits);
                        PasteProperty(ref outlineColorMask);
                        PasteProperty(ref outlineAlphaToMask);
                        PasteProperty(ref furCull);
                        PasteProperty(ref furSrcBlend);
                        PasteProperty(ref furDstBlend);
                        PasteProperty(ref furSrcBlendAlpha);
                        PasteProperty(ref furDstBlendAlpha);
                        PasteProperty(ref furBlendOp);
                        PasteProperty(ref furBlendOpAlpha);
                        PasteProperty(ref furSrcBlendFA);
                        PasteProperty(ref furDstBlendFA);
                        PasteProperty(ref furSrcBlendAlphaFA);
                        PasteProperty(ref furDstBlendAlphaFA);
                        PasteProperty(ref furBlendOpFA);
                        PasteProperty(ref furBlendOpAlphaFA);
                        PasteProperty(ref furZclip);
                        PasteProperty(ref furZwrite);
                        PasteProperty(ref furZtest);
                        PasteProperty(ref furOffsetFactor);
                        PasteProperty(ref furOffsetUnits);
                        PasteProperty(ref furColorMask);
                        PasteProperty(ref furAlphaToMask);
                    break;
                case lilPropertyBlock.Tessellation:
                        PasteProperty(ref tessEdge);
                        PasteProperty(ref tessStrength);
                        PasteProperty(ref tessShrink);
                        PasteProperty(ref tessFactorMax);
                    break;
            }
        }

        private void ResetProperties(lilPropertyBlock propertyBlock)
        {
            switch(propertyBlock)
            {
                case lilPropertyBlock.Base:
                        ResetProperty(ref invisible);
                        ResetProperty(ref cutoff);
                        ResetProperty(ref subpassCutoff);
                        ResetProperty(ref cull);
                        ResetProperty(ref flipNormal);
                        ResetProperty(ref backfaceForceShadow);
                        ResetProperty(ref zwrite);
                        ResetProperty(ref asUnlit);
                        ResetProperty(ref vertexLightStrength);
                        ResetProperty(ref lightMinLimit);
                        ResetProperty(ref lightMaxLimit);
                        ResetProperty(ref monochromeLighting);
                        ResetProperty(ref alphaBoostFA);
                        ResetProperty(ref shadowEnvStrength);
                        ResetProperty(ref fakeShadowVector);
                        ResetProperty(ref triMask);
                    break;
                case lilPropertyBlock.Lighting:
                        ResetProperty(ref asUnlit);
                        ResetProperty(ref vertexLightStrength);
                        ResetProperty(ref lightMinLimit);
                        ResetProperty(ref lightMaxLimit);
                        ResetProperty(ref monochromeLighting);
                        ResetProperty(ref alphaBoostFA);
                        ResetProperty(ref shadowEnvStrength);
                    break;
                case lilPropertyBlock.UV:
                        ResetProperty(ref mainTex_ScrollRotate);
                        ResetProperty(ref shiftBackfaceUV);
                    break;
                case lilPropertyBlock.MainColor:
                        ResetProperty(ref mainColor);
                        ResetProperty(ref mainTexHSVG);
                        ResetProperty(ref mainGradationStrength);
                        ResetProperty(ref mainGradationTex);
                        ResetProperty(ref useMain2ndTex);
                        ResetProperty(ref mainColor2nd);
                        ResetProperty(ref main2ndTex_UVMode);
                        ResetProperty(ref main2ndTexAngle);
                        ResetProperty(ref main2ndTexDecalAnimation);
                        ResetProperty(ref main2ndTexDecalSubParam);
                        ResetProperty(ref main2ndTexIsDecal);
                        ResetProperty(ref main2ndTexIsLeftOnly);
                        ResetProperty(ref main2ndTexIsRightOnly);
                        ResetProperty(ref main2ndTexShouldCopy);
                        ResetProperty(ref main2ndTexShouldFlipMirror);
                        ResetProperty(ref main2ndTexShouldFlipCopy);
                        ResetProperty(ref main2ndTexIsMSDF);
                        ResetProperty(ref main2ndTexBlendMode);
                        ResetProperty(ref main2ndEnableLighting);
                        ResetProperty(ref main2ndDissolveNoiseMask_ScrollRotate);
                        ResetProperty(ref main2ndDissolveNoiseStrength);
                        ResetProperty(ref main2ndDissolveColor);
                        ResetProperty(ref main2ndDissolveParams);
                        ResetProperty(ref main2ndDissolvePos);
                        ResetProperty(ref main2ndDistanceFade);
                        ResetProperty(ref useMain3rdTex);
                        ResetProperty(ref mainColor3rd);
                        ResetProperty(ref main3rdTex_UVMode);
                        ResetProperty(ref main3rdTexAngle);
                        ResetProperty(ref main3rdTexDecalAnimation);
                        ResetProperty(ref main3rdTexDecalSubParam);
                        ResetProperty(ref main3rdTexIsDecal);
                        ResetProperty(ref main3rdTexIsLeftOnly);
                        ResetProperty(ref main3rdTexIsRightOnly);
                        ResetProperty(ref main3rdTexShouldCopy);
                        ResetProperty(ref main3rdTexShouldFlipMirror);
                        ResetProperty(ref main3rdTexShouldFlipCopy);
                        ResetProperty(ref main3rdTexIsMSDF);
                        ResetProperty(ref main3rdTexBlendMode);
                        ResetProperty(ref main3rdEnableLighting);
                        ResetProperty(ref main3rdDissolveMask);
                        ResetProperty(ref main3rdDissolveNoiseMask);
                        ResetProperty(ref main3rdDissolveNoiseMask_ScrollRotate);
                        ResetProperty(ref main3rdDissolveNoiseStrength);
                        ResetProperty(ref main3rdDissolveColor);
                        ResetProperty(ref main3rdDissolveParams);
                        ResetProperty(ref main3rdDissolvePos);
                        ResetProperty(ref main3rdDistanceFade);
                        ResetProperty(ref alphaMaskMode);
                        ResetProperty(ref alphaMaskScale);
                        ResetProperty(ref alphaMaskValue);
                        ResetProperty(ref mainTex);
                        ResetProperty(ref mainColorAdjustMask);
                        ResetProperty(ref main2ndTex);
                        ResetProperty(ref main2ndBlendMask);
                        ResetProperty(ref main2ndDissolveMask);
                        ResetProperty(ref main2ndDissolveNoiseMask);
                        ResetProperty(ref main3rdTex);
                        ResetProperty(ref main3rdBlendMask);
                        ResetProperty(ref main3rdDissolveMask);
                        ResetProperty(ref main3rdDissolveNoiseMask);
                        ResetProperty(ref alphaMask);
                    break;
                case lilPropertyBlock.MainColor1st:
                        ResetProperty(ref mainColor);
                        ResetProperty(ref mainTexHSVG);
                        ResetProperty(ref mainGradationStrength);
                        ResetProperty(ref mainGradationTex);
                        ResetProperty(ref mainTex);
                        ResetProperty(ref mainColorAdjustMask);
                    break;
                case lilPropertyBlock.MainColor2nd:
                        ResetProperty(ref useMain2ndTex);
                        ResetProperty(ref mainColor2nd);
                        ResetProperty(ref main2ndTex_UVMode);
                        ResetProperty(ref main2ndTexAngle);
                        ResetProperty(ref main2ndTexDecalAnimation);
                        ResetProperty(ref main2ndTexDecalSubParam);
                        ResetProperty(ref main2ndTexIsDecal);
                        ResetProperty(ref main2ndTexIsLeftOnly);
                        ResetProperty(ref main2ndTexIsRightOnly);
                        ResetProperty(ref main2ndTexShouldCopy);
                        ResetProperty(ref main2ndTexShouldFlipMirror);
                        ResetProperty(ref main2ndTexShouldFlipCopy);
                        ResetProperty(ref main2ndTexIsMSDF);
                        ResetProperty(ref main2ndTexBlendMode);
                        ResetProperty(ref main2ndEnableLighting);
                        ResetProperty(ref main2ndDissolveNoiseMask_ScrollRotate);
                        ResetProperty(ref main2ndDissolveNoiseStrength);
                        ResetProperty(ref main2ndDissolveColor);
                        ResetProperty(ref main2ndDissolveParams);
                        ResetProperty(ref main2ndDissolvePos);
                        ResetProperty(ref main2ndDistanceFade);
                        ResetProperty(ref main2ndTex);
                        ResetProperty(ref main2ndBlendMask);
                        ResetProperty(ref main2ndDissolveMask);
                        ResetProperty(ref main2ndDissolveNoiseMask);
                    break;
                case lilPropertyBlock.MainColor3rd:
                        ResetProperty(ref useMain3rdTex);
                        ResetProperty(ref mainColor3rd);
                        ResetProperty(ref main3rdTex_UVMode);
                        ResetProperty(ref main3rdTexAngle);
                        ResetProperty(ref main3rdTexDecalAnimation);
                        ResetProperty(ref main3rdTexDecalSubParam);
                        ResetProperty(ref main3rdTexIsDecal);
                        ResetProperty(ref main3rdTexIsLeftOnly);
                        ResetProperty(ref main3rdTexIsRightOnly);
                        ResetProperty(ref main3rdTexShouldCopy);
                        ResetProperty(ref main3rdTexShouldFlipMirror);
                        ResetProperty(ref main3rdTexShouldFlipCopy);
                        ResetProperty(ref main3rdTexIsMSDF);
                        ResetProperty(ref main3rdTexBlendMode);
                        ResetProperty(ref main3rdEnableLighting);
                        ResetProperty(ref main3rdDissolveMask);
                        ResetProperty(ref main3rdDissolveNoiseMask);
                        ResetProperty(ref main3rdDissolveNoiseMask_ScrollRotate);
                        ResetProperty(ref main3rdDissolveNoiseStrength);
                        ResetProperty(ref main3rdDissolveColor);
                        ResetProperty(ref main3rdDissolveParams);
                        ResetProperty(ref main3rdDissolvePos);
                        ResetProperty(ref main3rdDistanceFade);
                        ResetProperty(ref main3rdTex);
                        ResetProperty(ref main3rdBlendMask);
                        ResetProperty(ref main3rdDissolveMask);
                        ResetProperty(ref main3rdDissolveNoiseMask);
                    break;
                case lilPropertyBlock.AlphaMask:
                        ResetProperty(ref alphaMaskMode);
                        ResetProperty(ref alphaMaskScale);
                        ResetProperty(ref alphaMaskValue);
                        ResetProperty(ref alphaMask);
                    break;
                case lilPropertyBlock.Shadow:
                        ResetProperty(ref useShadow);
                        ResetProperty(ref shadowColor);
                        ResetProperty(ref shadowNormalStrength);
                        ResetProperty(ref shadowBorder);
                        ResetProperty(ref shadowBlur);
                        ResetProperty(ref shadowStrength);
                        ResetProperty(ref shadowAOShift);
                        ResetProperty(ref shadowAOShift2);
                        ResetProperty(ref shadowPostAO);
                        ResetProperty(ref shadow2ndColor);
                        ResetProperty(ref shadow2ndNormalStrength);
                        ResetProperty(ref shadow2ndBorder);
                        ResetProperty(ref shadow2ndBlur);
                        ResetProperty(ref shadow3rdColor);
                        ResetProperty(ref shadow3rdNormalStrength);
                        ResetProperty(ref shadow3rdBorder);
                        ResetProperty(ref shadow3rdBlur);
                        ResetProperty(ref shadowMainStrength);
                        ResetProperty(ref shadowEnvStrength);
                        ResetProperty(ref shadowBorderColor);
                        ResetProperty(ref shadowBorderRange);
                        ResetProperty(ref shadowReceive);
                        ResetProperty(ref shadowBorderMask);
                        ResetProperty(ref shadowBlurMask);
                        ResetProperty(ref shadowStrengthMask);
                        ResetProperty(ref shadowColorTex);
                        ResetProperty(ref shadow2ndColorTex);
                        ResetProperty(ref shadow3rdColorTex);
                    break;
                case lilPropertyBlock.Emission:
                        ResetProperty(ref useEmission);
                        ResetProperty(ref emissionColor);
                        ResetProperty(ref emissionMap_ScrollRotate);
                        ResetProperty(ref emissionMap_UVMode);
                        ResetProperty(ref emissionBlend);
                        ResetProperty(ref emissionBlendMask_ScrollRotate);
                        ResetProperty(ref emissionBlink);
                        ResetProperty(ref emissionUseGrad);
                        ResetProperty(ref emissionGradTex);
                        ResetProperty(ref emissionGradSpeed);
                        ResetProperty(ref emissionParallaxDepth);
                        ResetProperty(ref emissionFluorescence);
                        ResetProperty(ref useEmission2nd);
                        ResetProperty(ref emission2ndColor);
                        ResetProperty(ref emission2ndMap_ScrollRotate);
                        ResetProperty(ref emission2ndMap_UVMode);
                        ResetProperty(ref emission2ndBlend);
                        ResetProperty(ref emission2ndBlendMask_ScrollRotate);
                        ResetProperty(ref emission2ndBlink);
                        ResetProperty(ref emission2ndUseGrad);
                        ResetProperty(ref emission2ndGradTex);
                        ResetProperty(ref emission2ndGradSpeed);
                        ResetProperty(ref emission2ndParallaxDepth);
                        ResetProperty(ref emission2ndFluorescence);
                        ResetProperty(ref emissionMap);
                        ResetProperty(ref emissionBlendMask);
                        ResetProperty(ref emission2ndMap);
                        ResetProperty(ref emission2ndBlendMask);
                    break;
                case lilPropertyBlock.Emission1st:
                        ResetProperty(ref useEmission);
                        ResetProperty(ref emissionColor);
                        ResetProperty(ref emissionMap_ScrollRotate);
                        ResetProperty(ref emissionMap_UVMode);
                        ResetProperty(ref emissionBlend);
                        ResetProperty(ref emissionBlendMask_ScrollRotate);
                        ResetProperty(ref emissionBlink);
                        ResetProperty(ref emissionUseGrad);
                        ResetProperty(ref emissionGradTex);
                        ResetProperty(ref emissionGradSpeed);
                        ResetProperty(ref emissionParallaxDepth);
                        ResetProperty(ref emissionFluorescence);
                        ResetProperty(ref emissionMap);
                        ResetProperty(ref emissionBlendMask);
                    break;
                case lilPropertyBlock.Emission2nd:
                        ResetProperty(ref useEmission2nd);
                        ResetProperty(ref emission2ndColor);
                        ResetProperty(ref emission2ndMap_ScrollRotate);
                        ResetProperty(ref emission2ndMap_UVMode);
                        ResetProperty(ref emission2ndBlend);
                        ResetProperty(ref emission2ndBlendMask_ScrollRotate);
                        ResetProperty(ref emission2ndBlink);
                        ResetProperty(ref emission2ndUseGrad);
                        ResetProperty(ref emission2ndGradTex);
                        ResetProperty(ref emission2ndGradSpeed);
                        ResetProperty(ref emission2ndParallaxDepth);
                        ResetProperty(ref emission2ndFluorescence);
                        ResetProperty(ref emission2ndMap);
                        ResetProperty(ref emission2ndBlendMask);
                    break;
                case lilPropertyBlock.NormalMap:
                        ResetProperty(ref useBumpMap);
                        ResetProperty(ref bumpScale);
                        ResetProperty(ref useBump2ndMap);
                        ResetProperty(ref bump2ndScale);
                        ResetProperty(ref useAnisotropy);
                        ResetProperty(ref anisotropyScale);
                        ResetProperty(ref anisotropyTangentWidth);
                        ResetProperty(ref anisotropyBitangentWidth);
                        ResetProperty(ref anisotropyShift);
                        ResetProperty(ref anisotropyShiftNoiseScale);
                        ResetProperty(ref anisotropySpecularStrength);
                        ResetProperty(ref anisotropy2ndTangentWidth);
                        ResetProperty(ref anisotropy2ndBitangentWidth);
                        ResetProperty(ref anisotropy2ndShift);
                        ResetProperty(ref anisotropy2ndShiftNoiseScale);
                        ResetProperty(ref anisotropy2ndSpecularStrength);
                        ResetProperty(ref anisotropy2Reflection);
                        ResetProperty(ref anisotropy2MatCap);
                        ResetProperty(ref anisotropy2MatCap2nd);
                        ResetProperty(ref bumpMap);
                        ResetProperty(ref bump2ndMap);
                        ResetProperty(ref bump2ndScaleMask);
                        ResetProperty(ref anisotropyTangentMap);
                        ResetProperty(ref anisotropyScaleMask);
                        ResetProperty(ref anisotropyShiftNoiseMask);
                    break;
                case lilPropertyBlock.NormalMap1st:
                        ResetProperty(ref useBumpMap);
                        ResetProperty(ref bumpScale);
                        ResetProperty(ref bumpMap);
                    break;
                case lilPropertyBlock.NormalMap2nd:
                        ResetProperty(ref useBump2ndMap);
                        ResetProperty(ref bump2ndScale);
                        ResetProperty(ref bump2ndMap);
                        ResetProperty(ref bump2ndScaleMask);
                    break;
                case lilPropertyBlock.Anisotropy:
                        ResetProperty(ref useAnisotropy);
                        ResetProperty(ref anisotropyScale);
                        ResetProperty(ref anisotropyTangentWidth);
                        ResetProperty(ref anisotropyBitangentWidth);
                        ResetProperty(ref anisotropyShift);
                        ResetProperty(ref anisotropyShiftNoiseScale);
                        ResetProperty(ref anisotropySpecularStrength);
                        ResetProperty(ref anisotropy2ndTangentWidth);
                        ResetProperty(ref anisotropy2ndBitangentWidth);
                        ResetProperty(ref anisotropy2ndShift);
                        ResetProperty(ref anisotropy2ndShiftNoiseScale);
                        ResetProperty(ref anisotropy2ndSpecularStrength);
                        ResetProperty(ref anisotropy2Reflection);
                        ResetProperty(ref anisotropy2MatCap);
                        ResetProperty(ref anisotropy2MatCap2nd);
                        ResetProperty(ref anisotropyTangentMap);
                        ResetProperty(ref anisotropyScaleMask);
                        ResetProperty(ref anisotropyShiftNoiseMask);
                    break;
                case lilPropertyBlock.Reflections:
                        ResetProperty(ref useReflection);
                        ResetProperty(ref metallic);
                        ResetProperty(ref smoothness);
                        ResetProperty(ref reflectance);
                        ResetProperty(ref reflectionColor);
                        ResetProperty(ref applySpecular);
                        ResetProperty(ref applySpecularFA);
                        ResetProperty(ref specularNormalStrength);
                        ResetProperty(ref specularToon);
                        ResetProperty(ref specularBorder);
                        ResetProperty(ref specularBlur);
                        ResetProperty(ref applyReflection);
                        ResetProperty(ref reflectionNormalStrength);
                        ResetProperty(ref reflectionApplyTransparency);
                        ResetProperty(ref reflectionCubeColor);
                        ResetProperty(ref reflectionCubeOverride);
                        ResetProperty(ref reflectionCubeEnableLighting);
                        ResetProperty(ref useMatCap);
                        ResetProperty(ref matcapColor);
                        ResetProperty(ref matcapBlendUV1);
                        ResetProperty(ref matcapZRotCancel);
                        ResetProperty(ref matcapPerspective);
                        ResetProperty(ref matcapVRParallaxStrength);
                        ResetProperty(ref matcapBlend);
                        ResetProperty(ref matcapEnableLighting);
                        ResetProperty(ref matcapShadowMask);
                        ResetProperty(ref matcapBackfaceMask);
                        ResetProperty(ref matcapLod);
                        ResetProperty(ref matcapBlendMode);
                        ResetProperty(ref matcapMul);
                        ResetProperty(ref matcapApplyTransparency);
                        ResetProperty(ref matcapNormalStrength);
                        ResetProperty(ref matcapCustomNormal);
                        ResetProperty(ref matcapBumpScale);
                        ResetProperty(ref useMatCap2nd);
                        ResetProperty(ref matcap2ndColor);
                        ResetProperty(ref matcap2ndBlendUV1);
                        ResetProperty(ref matcap2ndZRotCancel);
                        ResetProperty(ref matcap2ndPerspective);
                        ResetProperty(ref matcap2ndVRParallaxStrength);
                        ResetProperty(ref matcap2ndBlend);
                        ResetProperty(ref matcap2ndEnableLighting);
                        ResetProperty(ref matcap2ndShadowMask);
                        ResetProperty(ref matcap2ndBackfaceMask);
                        ResetProperty(ref matcap2ndLod);
                        ResetProperty(ref matcap2ndBlendMode);
                        ResetProperty(ref matcap2ndMul);
                        ResetProperty(ref matcap2ndNormalStrength);
                        ResetProperty(ref matcap2ndApplyTransparency);
                        ResetProperty(ref matcap2ndCustomNormal);
                        ResetProperty(ref matcap2ndBumpScale);
                        ResetProperty(ref useRim);
                        ResetProperty(ref rimColor);
                        ResetProperty(ref rimNormalStrength);
                        ResetProperty(ref rimBorder);
                        ResetProperty(ref rimBlur);
                        ResetProperty(ref rimFresnelPower);
                        ResetProperty(ref rimEnableLighting);
                        ResetProperty(ref rimShadowMask);
                        ResetProperty(ref rimBackfaceMask);
                        ResetProperty(ref rimVRParallaxStrength);
                        ResetProperty(ref rimApplyTransparency);
                        ResetProperty(ref rimDirStrength);
                        ResetProperty(ref rimDirRange);
                        ResetProperty(ref rimIndirRange);
                        ResetProperty(ref rimIndirColor);
                        ResetProperty(ref rimIndirBorder);
                        ResetProperty(ref rimIndirBlur);
                        ResetProperty(ref useGlitter);
                        ResetProperty(ref glitterUVMode);
                        ResetProperty(ref glitterColor);
                        ResetProperty(ref glitterMainStrength);
                        ResetProperty(ref glitterParams1);
                        ResetProperty(ref glitterParams2);
                        ResetProperty(ref glitterPostContrast);
                        ResetProperty(ref glitterEnableLighting);
                        ResetProperty(ref glitterShadowMask);
                        ResetProperty(ref glitterBackfaceMask);
                        ResetProperty(ref glitterApplyTransparency);
                        ResetProperty(ref glitterVRParallaxStrength);
                        ResetProperty(ref glitterNormalStrength);
                        ResetProperty(ref useBacklight);
                        ResetProperty(ref backlightColor);
                        ResetProperty(ref backlightNormalStrength);
                        ResetProperty(ref backlightBorder);
                        ResetProperty(ref backlightBlur);
                        ResetProperty(ref backlightDirectivity);
                        ResetProperty(ref backlightViewStrength);
                        ResetProperty(ref backlightReceiveShadow);
                        ResetProperty(ref backlightBackfaceMask);
                        ResetProperty(ref gemChromaticAberration);
                        ResetProperty(ref gemEnvContrast);
                        ResetProperty(ref gemEnvColor);
                        ResetProperty(ref gemParticleLoop);
                        ResetProperty(ref gemParticleColor);
                        ResetProperty(ref gemVRParallaxStrength);
                        ResetProperty(ref refractionStrength);
                        ResetProperty(ref refractionFresnelPower);
                        ResetProperty(ref metallicGlossMap);
                        ResetProperty(ref smoothnessTex);
                        ResetProperty(ref reflectionColorTex);
                        ResetProperty(ref reflectionCubeTex);
                        ResetProperty(ref matcapTex);
                        ResetProperty(ref matcapBlendMask);
                        ResetProperty(ref matcapBumpMap);
                        ResetProperty(ref matcap2ndTex);
                        ResetProperty(ref matcap2ndBlendMask);
                        ResetProperty(ref matcap2ndBumpMap);
                        ResetProperty(ref rimColorTex);
                        ResetProperty(ref glitterColorTex);
                        ResetProperty(ref backlightColorTex);
                    break;
                case lilPropertyBlock.Reflection:
                        ResetProperty(ref useReflection);
                        ResetProperty(ref metallic);
                        ResetProperty(ref smoothness);
                        ResetProperty(ref reflectance);
                        ResetProperty(ref reflectionColor);
                        ResetProperty(ref applySpecular);
                        ResetProperty(ref applySpecularFA);
                        ResetProperty(ref specularNormalStrength);
                        ResetProperty(ref specularToon);
                        ResetProperty(ref specularBorder);
                        ResetProperty(ref specularBlur);
                        ResetProperty(ref applyReflection);
                        ResetProperty(ref reflectionNormalStrength);
                        ResetProperty(ref reflectionApplyTransparency);
                        ResetProperty(ref reflectionCubeTex);
                        ResetProperty(ref reflectionCubeColor);
                        ResetProperty(ref reflectionCubeOverride);
                        ResetProperty(ref reflectionCubeEnableLighting);
                        ResetProperty(ref metallicGlossMap);
                        ResetProperty(ref smoothnessTex);
                        ResetProperty(ref reflectionColorTex);
                    break;
                case lilPropertyBlock.MatCaps:
                        ResetProperty(ref useMatCap);
                        ResetProperty(ref matcapColor);
                        ResetProperty(ref matcapBlendUV1);
                        ResetProperty(ref matcapZRotCancel);
                        ResetProperty(ref matcapPerspective);
                        ResetProperty(ref matcapVRParallaxStrength);
                        ResetProperty(ref matcapBlend);
                        ResetProperty(ref matcapEnableLighting);
                        ResetProperty(ref matcapShadowMask);
                        ResetProperty(ref matcapBackfaceMask);
                        ResetProperty(ref matcapLod);
                        ResetProperty(ref matcapBlendMode);
                        ResetProperty(ref matcapMul);
                        ResetProperty(ref matcapApplyTransparency);
                        ResetProperty(ref matcapNormalStrength);
                        ResetProperty(ref matcapCustomNormal);
                        ResetProperty(ref matcapBumpScale);
                        ResetProperty(ref useMatCap2nd);
                        ResetProperty(ref matcap2ndColor);
                        ResetProperty(ref matcap2ndBlendUV1);
                        ResetProperty(ref matcap2ndZRotCancel);
                        ResetProperty(ref matcap2ndPerspective);
                        ResetProperty(ref matcap2ndVRParallaxStrength);
                        ResetProperty(ref matcap2ndBlend);
                        ResetProperty(ref matcap2ndEnableLighting);
                        ResetProperty(ref matcap2ndShadowMask);
                        ResetProperty(ref matcap2ndBackfaceMask);
                        ResetProperty(ref matcap2ndLod);
                        ResetProperty(ref matcap2ndBlendMode);
                        ResetProperty(ref matcap2ndMul);
                        ResetProperty(ref matcap2ndNormalStrength);
                        ResetProperty(ref matcap2ndApplyTransparency);
                        ResetProperty(ref matcap2ndCustomNormal);
                        ResetProperty(ref matcap2ndBumpScale);
                        ResetProperty(ref matcapTex);
                        ResetProperty(ref matcapBlendMask);
                        ResetProperty(ref matcapBumpMap);
                        ResetProperty(ref matcap2ndTex);
                        ResetProperty(ref matcap2ndBlendMask);
                        ResetProperty(ref matcap2ndBumpMap);
                    break;
                case lilPropertyBlock.MatCap1st:
                        ResetProperty(ref useMatCap);
                        ResetProperty(ref matcapColor);
                        ResetProperty(ref matcapBlendUV1);
                        ResetProperty(ref matcapZRotCancel);
                        ResetProperty(ref matcapPerspective);
                        ResetProperty(ref matcapVRParallaxStrength);
                        ResetProperty(ref matcapBlend);
                        ResetProperty(ref matcapEnableLighting);
                        ResetProperty(ref matcapShadowMask);
                        ResetProperty(ref matcapBackfaceMask);
                        ResetProperty(ref matcapLod);
                        ResetProperty(ref matcapBlendMode);
                        ResetProperty(ref matcapMul);
                        ResetProperty(ref matcapApplyTransparency);
                        ResetProperty(ref matcapNormalStrength);
                        ResetProperty(ref matcapCustomNormal);
                        ResetProperty(ref matcapBumpScale);
                        ResetProperty(ref matcapTex);
                        ResetProperty(ref matcapBlendMask);
                        ResetProperty(ref matcapBumpMap);
                    break;
                case lilPropertyBlock.MatCap2nd:
                        ResetProperty(ref useMatCap2nd);
                        ResetProperty(ref matcap2ndColor);
                        ResetProperty(ref matcap2ndBlendUV1);
                        ResetProperty(ref matcap2ndZRotCancel);
                        ResetProperty(ref matcap2ndPerspective);
                        ResetProperty(ref matcap2ndVRParallaxStrength);
                        ResetProperty(ref matcap2ndBlend);
                        ResetProperty(ref matcap2ndEnableLighting);
                        ResetProperty(ref matcap2ndShadowMask);
                        ResetProperty(ref matcap2ndBackfaceMask);
                        ResetProperty(ref matcap2ndLod);
                        ResetProperty(ref matcap2ndBlendMode);
                        ResetProperty(ref matcap2ndMul);
                        ResetProperty(ref matcap2ndApplyTransparency);
                        ResetProperty(ref matcap2ndNormalStrength);
                        ResetProperty(ref matcap2ndCustomNormal);
                        ResetProperty(ref matcap2ndBumpScale);
                        ResetProperty(ref matcap2ndTex);
                        ResetProperty(ref matcap2ndBlendMask);
                        ResetProperty(ref matcap2ndBumpMap);
                    break;
                case lilPropertyBlock.RimLight:
                        ResetProperty(ref useRim);
                        ResetProperty(ref rimColor);
                        ResetProperty(ref rimNormalStrength);
                        ResetProperty(ref rimBorder);
                        ResetProperty(ref rimBlur);
                        ResetProperty(ref rimFresnelPower);
                        ResetProperty(ref rimEnableLighting);
                        ResetProperty(ref rimShadowMask);
                        ResetProperty(ref rimBackfaceMask);
                        ResetProperty(ref rimVRParallaxStrength);
                        ResetProperty(ref rimApplyTransparency);
                        ResetProperty(ref rimDirStrength);
                        ResetProperty(ref rimDirRange);
                        ResetProperty(ref rimIndirRange);
                        ResetProperty(ref rimIndirColor);
                        ResetProperty(ref rimIndirBorder);
                        ResetProperty(ref rimIndirBlur);
                        ResetProperty(ref rimColorTex);
                    break;
                case lilPropertyBlock.Glitter:
                        ResetProperty(ref useGlitter);
                        ResetProperty(ref glitterUVMode);
                        ResetProperty(ref glitterColor);
                        ResetProperty(ref glitterMainStrength);
                        ResetProperty(ref glitterParams1);
                        ResetProperty(ref glitterParams2);
                        ResetProperty(ref glitterPostContrast);
                        ResetProperty(ref glitterEnableLighting);
                        ResetProperty(ref glitterShadowMask);
                        ResetProperty(ref glitterBackfaceMask);
                        ResetProperty(ref glitterApplyTransparency);
                        ResetProperty(ref glitterVRParallaxStrength);
                        ResetProperty(ref glitterNormalStrength);
                        ResetProperty(ref glitterColorTex);
                    break;
                case lilPropertyBlock.Backlight:
                        ResetProperty(ref useBacklight);
                        ResetProperty(ref backlightColor);
                        ResetProperty(ref backlightNormalStrength);
                        ResetProperty(ref backlightBorder);
                        ResetProperty(ref backlightBlur);
                        ResetProperty(ref backlightDirectivity);
                        ResetProperty(ref backlightViewStrength);
                        ResetProperty(ref backlightReceiveShadow);
                        ResetProperty(ref backlightBackfaceMask);
                        ResetProperty(ref backlightColorTex);
                    break;
                case lilPropertyBlock.Gem:
                        ResetProperty(ref gemChromaticAberration);
                        ResetProperty(ref gemEnvContrast);
                        ResetProperty(ref gemEnvColor);
                        ResetProperty(ref gemParticleLoop);
                        ResetProperty(ref gemParticleColor);
                        ResetProperty(ref gemVRParallaxStrength);
                        ResetProperty(ref refractionStrength);
                        ResetProperty(ref refractionFresnelPower);
                        ResetProperty(ref smoothness);
                        ResetProperty(ref smoothnessTex);
                    break;
                case lilPropertyBlock.Outline:
                        ResetProperty(ref outlineColor);
                        ResetProperty(ref outlineTex_ScrollRotate);
                        ResetProperty(ref outlineTexHSVG);
                        ResetProperty(ref outlineWidth);
                        ResetProperty(ref outlineFixWidth);
                        ResetProperty(ref outlineVertexR2Width);
                        ResetProperty(ref outlineVectorTex);
                        ResetProperty(ref outlineVectorScale);
                        ResetProperty(ref outlineEnableLighting);
                        ResetProperty(ref outlineTex);
                        ResetProperty(ref outlineWidthMask);
                    break;
                case lilPropertyBlock.Parallax:
                        ResetProperty(ref useParallax);
                        ResetProperty(ref parallax);
                        ResetProperty(ref parallaxOffset);
                        ResetProperty(ref parallaxMap);
                    break;
                case lilPropertyBlock.DistanceFade:
                        ResetProperty(ref distanceFadeColor);
                        ResetProperty(ref distanceFade);
                    break;
                case lilPropertyBlock.AudioLink:
                        ResetProperty(ref useAudioLink);
                        ResetProperty(ref audioLinkDefaultValue);
                        ResetProperty(ref audioLinkUVMode);
                        ResetProperty(ref audioLinkUVParams);
                        ResetProperty(ref audioLinkStart);
                        ResetProperty(ref audioLink2Main2nd);
                        ResetProperty(ref audioLink2Main3rd);
                        ResetProperty(ref audioLink2Emission);
                        ResetProperty(ref audioLink2EmissionGrad);
                        ResetProperty(ref audioLink2Emission2nd);
                        ResetProperty(ref audioLink2Emission2ndGrad);
                        ResetProperty(ref audioLink2Vertex);
                        ResetProperty(ref audioLinkVertexUVMode);
                        ResetProperty(ref audioLinkVertexUVParams);
                        ResetProperty(ref audioLinkVertexStart);
                        ResetProperty(ref audioLinkVertexStrength);
                        ResetProperty(ref audioLinkAsLocal);
                        ResetProperty(ref audioLinkLocalMap);
                        ResetProperty(ref audioLinkLocalMapParams);
                        ResetProperty(ref audioLinkMask);
                    break;
                case lilPropertyBlock.Dissolve:
                        ResetProperty(ref dissolveNoiseMask_ScrollRotate);
                        ResetProperty(ref dissolveNoiseStrength);
                        ResetProperty(ref dissolveColor);
                        ResetProperty(ref dissolveParams);
                        ResetProperty(ref dissolvePos);
                        ResetProperty(ref dissolveMask);
                        ResetProperty(ref dissolveNoiseMask);
                    break;
                case lilPropertyBlock.Refraction:
                        ResetProperty(ref refractionStrength);
                        ResetProperty(ref refractionFresnelPower);
                        ResetProperty(ref refractionColorFromMain);
                        ResetProperty(ref refractionColor);
                    break;
                case lilPropertyBlock.Fur:
                        ResetProperty(ref furVectorScale);
                        ResetProperty(ref furVector);
                        ResetProperty(ref furGravity);
                        ResetProperty(ref furRandomize);
                        ResetProperty(ref furAO);
                        ResetProperty(ref vertexColor2FurVector);
                        ResetProperty(ref furLayerNum);
                        ResetProperty(ref furRootOffset);
                        ResetProperty(ref furCutoutLength);
                        ResetProperty(ref furTouchStrength);
                        ResetProperty(ref furNoiseMask);
                        ResetProperty(ref furMask);
                        ResetProperty(ref furLengthMask);
                        ResetProperty(ref furVectorTex);
                    break;
                case lilPropertyBlock.Encryption:
                        ResetProperty(ref ignoreEncryption);
                        ResetProperty(ref keys);
                    break;
                case lilPropertyBlock.Stencil:
                        ResetProperty(ref stencilRef);
                        ResetProperty(ref stencilReadMask);
                        ResetProperty(ref stencilWriteMask);
                        ResetProperty(ref stencilComp);
                        ResetProperty(ref stencilPass);
                        ResetProperty(ref stencilFail);
                        ResetProperty(ref stencilZFail);
                        ResetProperty(ref outlineStencilRef);
                        ResetProperty(ref outlineStencilReadMask);
                        ResetProperty(ref outlineStencilWriteMask);
                        ResetProperty(ref outlineStencilComp);
                        ResetProperty(ref outlineStencilPass);
                        ResetProperty(ref outlineStencilFail);
                        ResetProperty(ref outlineStencilZFail);
                        ResetProperty(ref furStencilRef);
                        ResetProperty(ref furStencilReadMask);
                        ResetProperty(ref furStencilWriteMask);
                        ResetProperty(ref furStencilComp);
                        ResetProperty(ref furStencilPass);
                        ResetProperty(ref furStencilFail);
                        ResetProperty(ref furStencilZFail);
                    break;
                case lilPropertyBlock.Rendering:
                        ResetProperty(ref beforeExposureLimit);
                        ResetProperty(ref lilDirectionalLightStrength);
                        ResetProperty(ref cull);
                        ResetProperty(ref srcBlend);
                        ResetProperty(ref dstBlend);
                        ResetProperty(ref srcBlendAlpha);
                        ResetProperty(ref dstBlendAlpha);
                        ResetProperty(ref blendOp);
                        ResetProperty(ref blendOpAlpha);
                        ResetProperty(ref srcBlendFA);
                        ResetProperty(ref dstBlendFA);
                        ResetProperty(ref srcBlendAlphaFA);
                        ResetProperty(ref dstBlendAlphaFA);
                        ResetProperty(ref blendOpFA);
                        ResetProperty(ref blendOpAlphaFA);
                        ResetProperty(ref zclip);
                        ResetProperty(ref zwrite);
                        ResetProperty(ref ztest);
                        ResetProperty(ref offsetFactor);
                        ResetProperty(ref offsetUnits);
                        ResetProperty(ref colorMask);
                        ResetProperty(ref alphaToMask);
                        ResetProperty(ref outlineCull);
                        ResetProperty(ref outlineSrcBlend);
                        ResetProperty(ref outlineDstBlend);
                        ResetProperty(ref outlineSrcBlendAlpha);
                        ResetProperty(ref outlineDstBlendAlpha);
                        ResetProperty(ref outlineBlendOp);
                        ResetProperty(ref outlineBlendOpAlpha);
                        ResetProperty(ref outlineSrcBlendFA);
                        ResetProperty(ref outlineDstBlendFA);
                        ResetProperty(ref outlineSrcBlendAlphaFA);
                        ResetProperty(ref outlineDstBlendAlphaFA);
                        ResetProperty(ref outlineBlendOpFA);
                        ResetProperty(ref outlineBlendOpAlphaFA);
                        ResetProperty(ref outlineZclip);
                        ResetProperty(ref outlineZwrite);
                        ResetProperty(ref outlineZtest);
                        ResetProperty(ref outlineOffsetFactor);
                        ResetProperty(ref outlineOffsetUnits);
                        ResetProperty(ref outlineColorMask);
                        ResetProperty(ref outlineAlphaToMask);
                        ResetProperty(ref furCull);
                        ResetProperty(ref furSrcBlend);
                        ResetProperty(ref furDstBlend);
                        ResetProperty(ref furSrcBlendAlpha);
                        ResetProperty(ref furDstBlendAlpha);
                        ResetProperty(ref furBlendOp);
                        ResetProperty(ref furBlendOpAlpha);
                        ResetProperty(ref furSrcBlendFA);
                        ResetProperty(ref furDstBlendFA);
                        ResetProperty(ref furSrcBlendAlphaFA);
                        ResetProperty(ref furDstBlendAlphaFA);
                        ResetProperty(ref furBlendOpFA);
                        ResetProperty(ref furBlendOpAlphaFA);
                        ResetProperty(ref furZclip);
                        ResetProperty(ref furZwrite);
                        ResetProperty(ref furZtest);
                        ResetProperty(ref furOffsetFactor);
                        ResetProperty(ref furOffsetUnits);
                        ResetProperty(ref furColorMask);
                        ResetProperty(ref furAlphaToMask);
                    break;
                case lilPropertyBlock.Tessellation:
                        ResetProperty(ref tessEdge);
                        ResetProperty(ref tessStrength);
                        ResetProperty(ref tessShrink);
                        ResetProperty(ref tessFactorMax);
                    break;
            }
        }

        private void ApplyLightingPreset(lilLightingPreset lightingPreset)
        {
            switch(lightingPreset)
            {
                case lilLightingPreset.Default:
                    if(asUnlit != null) asUnlit.floatValue = shaderSetting.defaultAsUnlit;
                    if(vertexLightStrength != null) vertexLightStrength.floatValue = shaderSetting.defaultVertexLightStrength;
                    if(lightMinLimit != null) lightMinLimit.floatValue = shaderSetting.defaultLightMinLimit;
                    if(lightMaxLimit != null) lightMaxLimit.floatValue = shaderSetting.defaultLightMaxLimit;
                    if(beforeExposureLimit != null) beforeExposureLimit.floatValue = shaderSetting.defaultBeforeExposureLimit;
                    if(monochromeLighting != null) monochromeLighting.floatValue = shaderSetting.defaultMonochromeLighting;
                    if(shadowEnvStrength != null) shadowEnvStrength.floatValue = 0.0f;
                    if(lilDirectionalLightStrength != null) lilDirectionalLightStrength.floatValue = shaderSetting.defaultlilDirectionalLightStrength;
                    if(lightDirectionOverride != null) lightDirectionOverride.vectorValue = shaderSetting.defaultLightDirectionOverride;
                    break;
                case lilLightingPreset.SemiMonochrome:
                    if(asUnlit != null) asUnlit.floatValue = 0.0f;
                    if(vertexLightStrength != null) vertexLightStrength.floatValue = 0.0f;
                    if(lightMinLimit != null) lightMinLimit.floatValue = 0.05f;
                    if(lightMaxLimit != null) lightMaxLimit.floatValue = 1.0f;
                    if(beforeExposureLimit != null) beforeExposureLimit.floatValue = 10000.0f;
                    if(monochromeLighting != null) monochromeLighting.floatValue = 0.5f;
                    if(shadowEnvStrength != null) shadowEnvStrength.floatValue = 0.0f;
                    if(lilDirectionalLightStrength != null) lilDirectionalLightStrength.floatValue = 1.0f;
                    if(lightDirectionOverride != null) lightDirectionOverride.vectorValue = new Vector4(0.0f, 0.001f, 0.0f, 0.0f);
                    break;
            }
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Material Setup
        #region
        public static void SetupMaterialWithRenderingMode(Material material, RenderingMode renderingMode, TransparentMode transparentMode, bool isoutl, bool islite, bool isstencil, bool istess)
        {
            if(isMultiVariants) return;
            if(isMulti)
            {
                lilRenderPipeline lilRP = CheckRP();
                float tpmode = material.GetFloat("_TransparentMode");
                if(tpmode == 1.0f)
                {
                    if(isoutl)  material.shader = ltsmo;
                    else        material.shader = ltsm;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_OutlineSrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_OutlineDstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_AlphaToMask", 1);
                    material.SetInt("_OutlineAlphaToMask", 1);
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                    material.renderQueue = 2450;
                }
                else if(tpmode == 2.0f)
                {
                    if(isoutl)  material.shader = ltsmo;
                    else        material.shader = ltsm;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_OutlineSrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_OutlineDstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_AlphaToMask", 0);
                    material.SetInt("_OutlineAlphaToMask", 0);
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                    material.renderQueue = lilRP == lilRenderPipeline.HDRP ? 3000 : 2460;
                }
                else if(tpmode == 3.0f)
                {
                    material.shader = ltsmref;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_AlphaToMask", 0);
                    material.SetOverrideTag("RenderType", "");
                    material.renderQueue = -1;
                }
                else if(tpmode == 4.0f)
                {
                    material.shader = ltsmfur;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_AlphaToMask", 0);
                    material.SetInt("_FurSrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_FurDstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_FurZWrite", 0);
                    material.SetInt("_FurAlphaToMask", 0);
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                    material.renderQueue = 3000;
                }
                else if(tpmode == 5.0f)
                {
                    material.shader = ltsmfur;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_AlphaToMask", 1);
                    material.SetInt("_FurSrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_FurDstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_FurZWrite", 1);
                    material.SetInt("_FurAlphaToMask", 1);
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                    material.renderQueue = 2450;
                }
                else if(tpmode == 6.0f)
                {
                    material.shader = ltsmgem;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_AlphaToMask", 0);
                    material.SetInt("_Cull", 0);
                    material.SetOverrideTag("RenderType", "");
                    material.renderQueue = -1;
                }
                else
                {
                    if(isoutl)  material.shader = ltsmo;
                    else        material.shader = ltsm;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_OutlineSrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_OutlineDstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_AlphaToMask", 0);
                    material.SetInt("_OutlineAlphaToMask", 0);
                    material.SetOverrideTag("RenderType", "");
                    material.renderQueue = -1;
                }
                if(isstencil) material.renderQueue = material.shader.renderQueue - 1;
                if(tpmode == 6.0f)  material.SetInt("_ZWrite", 0);
                else                material.SetInt("_ZWrite", 1);
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
                if(tpmode <= 2.0f)
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
                return;
            }
            switch (renderingMode)
            {
                case RenderingMode.Opaque:
                    if(islite)
                    {
                        if(isoutl)  material.shader = ltslo;
                        else        material.shader = ltsl;
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
                    material.shader = ltsref;
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
                    material.shader = ltsfur;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_AlphaToMask", 0);
                    material.SetInt("_FurSrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_FurDstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_FurZWrite", 0);
                    material.SetInt("_FurAlphaToMask", 0);
                    break;
                case RenderingMode.FurCutout:
                    material.shader = ltsfurc;
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
                    material.shader = ltsgem;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_AlphaToMask", 0);
                    break;
            }
            if(isstencil) material.renderQueue = material.shader.renderQueue - 1;
            material.SetInt("_ZWrite", 1);
            if(renderingMode == RenderingMode.Gem)
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

        public static void SetupShaderSettingFromMaterial(Material material, ref lilToonSetting shaderSetting)
        {
            if(shaderSetting.isLocked) return;
            if(!material.shader.name.Contains("lilToon") || material.shader.name.Contains("Lite") || material.shader.name.Contains("Multi")) return;

            if(material.HasProperty("_MainTex_ScrollRotate")) shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV = shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV || material.GetVector("_MainTex_ScrollRotate") != defaultScrollRotate;
            if(material.HasProperty("_UseShadow")) shaderSetting.LIL_FEATURE_SHADOW = shaderSetting.LIL_FEATURE_SHADOW || material.GetFloat("_UseShadow") != 0.0f;
            if(material.HasProperty("_ShadowReceive")) shaderSetting.LIL_FEATURE_RECEIVE_SHADOW = shaderSetting.LIL_FEATURE_RECEIVE_SHADOW || material.GetFloat("_ShadowReceive") != 0.0f;
            //if(material.HasProperty("")) shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER = shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER;
            //if(material.HasProperty("_DistanceFade")) shaderSetting.LIL_FEATURE_DISTANCE_FADE = shaderSetting.LIL_FEATURE_DISTANCE_FADE || material.GetVector("_DistanceFade").z != defaultDistanceFadeParams.z;
            //if(material.HasProperty("_Keys")) shaderSetting.LIL_FEATURE_ENCRYPTION = shaderSetting.LIL_FEATURE_ENCRYPTION || material.GetVector("_Keys") != defaultKeys;
            if(material.HasProperty("_ShadowBlurMask")) shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR = shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR || material.GetTexture("_ShadowBlurMask") != null;
            if(material.HasProperty("_ShadowBorderMask")) shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER = shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER || material.GetTexture("_ShadowBorderMask") != null;
            if(material.HasProperty("_ShadowStrengthMask")) shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH = shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH || material.GetTexture("_ShadowStrengthMask") != null;
            if(material.HasProperty("_ShadowColorTex")) shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST = shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST || material.GetTexture("_ShadowColorTex") != null;
            if(material.HasProperty("_Shadow2ndColorTex")) shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND = shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND || material.GetTexture("_Shadow2ndColorTex") != null;
            if(material.HasProperty("_Shadow3rdColor")) shaderSetting.LIL_FEATURE_SHADOW_3RD = shaderSetting.LIL_FEATURE_SHADOW_3RD || material.GetColor("_Shadow3rdColor").a != 0.0f;
            if(material.HasProperty("_Shadow3rdColorTex")) shaderSetting.LIL_FEATURE_TEX_SHADOW_3RD = shaderSetting.LIL_FEATURE_TEX_SHADOW_3RD || material.GetTexture("_Shadow3rdColorTex") != null;

            if(material.shader.name.Contains("Fur"))
            {
                if(material.HasProperty("_FurVectorTex")) shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL = shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL || material.GetTexture("_FurVectorTex") != null;
                if(material.HasProperty("_FurMask")) shaderSetting.LIL_FEATURE_TEX_FUR_MASK = shaderSetting.LIL_FEATURE_TEX_FUR_MASK || material.GetTexture("_FurMask") != null;
                if(material.HasProperty("_FurLengthMask")) shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH = shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH || material.GetTexture("_FurLengthMask") != null;
                if(material.HasProperty("_FurTouchStrength")) shaderSetting.LIL_FEATURE_FUR_COLLISION = shaderSetting.LIL_FEATURE_FUR_COLLISION || material.GetFloat("_FurTouchStrength") != 0.0f;
            }

            if(material.HasProperty("_MainTexHSVG")) shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION = shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION || material.GetVector("_MainTexHSVG") != defaultHSVG;
            if(material.HasProperty("_MainGradationStrength")) shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP = shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP || material.GetFloat("_MainGradationStrength") != 0.0f;
            if(material.HasProperty("_UseMain2ndTex")) shaderSetting.LIL_FEATURE_MAIN2ND = shaderSetting.LIL_FEATURE_MAIN2ND || material.GetFloat("_UseMain2ndTex") != 0.0f;
            if(material.HasProperty("_UseMain3rdTex")) shaderSetting.LIL_FEATURE_MAIN3RD = shaderSetting.LIL_FEATURE_MAIN3RD || material.GetFloat("_UseMain3rdTex") != 0.0f;
            if(material.HasProperty("_Main2ndTexIsDecal")) shaderSetting.LIL_FEATURE_DECAL = shaderSetting.LIL_FEATURE_DECAL || material.GetFloat("_Main2ndTexIsDecal") != 0.0f;
            if(material.HasProperty("_Main3rdTexIsDecal")) shaderSetting.LIL_FEATURE_DECAL = shaderSetting.LIL_FEATURE_DECAL || material.GetFloat("_Main3rdTexIsDecal") != 0.0f;
            if(material.HasProperty("_Main2ndTexDecalAnimation")) shaderSetting.LIL_FEATURE_ANIMATE_DECAL = shaderSetting.LIL_FEATURE_ANIMATE_DECAL || material.GetVector("_Main2ndTexDecalAnimation") != defaultDecalAnim;
            if(material.HasProperty("_Main3rdTexDecalAnimation")) shaderSetting.LIL_FEATURE_ANIMATE_DECAL = shaderSetting.LIL_FEATURE_ANIMATE_DECAL || material.GetVector("_Main3rdTexDecalAnimation") != defaultDecalAnim;
            if(material.HasProperty("_Main2ndDissolveParams")) shaderSetting.LIL_FEATURE_LAYER_DISSOLVE = shaderSetting.LIL_FEATURE_LAYER_DISSOLVE || material.GetVector("_Main2ndDissolveParams").x != defaultDissolveParams.x;
            if(material.HasProperty("_Main3rdDissolveParams")) shaderSetting.LIL_FEATURE_LAYER_DISSOLVE = shaderSetting.LIL_FEATURE_LAYER_DISSOLVE || material.GetVector("_Main3rdDissolveParams").x != defaultDissolveParams.x;
            if(material.HasProperty("_AlphaMaskMode")) shaderSetting.LIL_FEATURE_ALPHAMASK = shaderSetting.LIL_FEATURE_ALPHAMASK || material.GetFloat("_AlphaMaskMode") != 0.0f;
            if(material.HasProperty("_UseEmission")) shaderSetting.LIL_FEATURE_EMISSION_1ST = shaderSetting.LIL_FEATURE_EMISSION_1ST || material.GetFloat("_UseEmission") != 0.0f;
            if(material.HasProperty("_UseEmission2nd")) shaderSetting.LIL_FEATURE_EMISSION_2ND = shaderSetting.LIL_FEATURE_EMISSION_2ND || material.GetFloat("_UseEmission2nd") != 0.0f;
            if(material.HasProperty("_EmissionMap_ScrollRotate")) shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV = shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV || material.GetVector("_EmissionMap_ScrollRotate") != defaultScrollRotate;
            if(material.HasProperty("_Emission2ndMap_ScrollRotate")) shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV = shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV || material.GetVector("_Emission2ndMap_ScrollRotate") != defaultScrollRotate;
            if(material.HasProperty("_EmissionBlendMask_ScrollRotate")) shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV = shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV || material.GetVector("_EmissionBlendMask_ScrollRotate") != defaultScrollRotate;
            if(material.HasProperty("_Emission2ndBlendMask_ScrollRotate")) shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV = shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV || material.GetVector("_Emission2ndBlendMask_ScrollRotate") != defaultScrollRotate;
            if(material.HasProperty("_EmissionUseGrad")) shaderSetting.LIL_FEATURE_EMISSION_GRADATION = shaderSetting.LIL_FEATURE_EMISSION_GRADATION || material.GetFloat("_EmissionUseGrad") != 0.0f;
            if(material.HasProperty("_UseBumpMap")) shaderSetting.LIL_FEATURE_NORMAL_1ST = shaderSetting.LIL_FEATURE_NORMAL_1ST || material.GetFloat("_UseBumpMap") != 0.0f;
            if(material.HasProperty("_UseBump2ndMap")) shaderSetting.LIL_FEATURE_NORMAL_2ND = shaderSetting.LIL_FEATURE_NORMAL_2ND || material.GetFloat("_UseBump2ndMap") != 0.0f;
            if(material.HasProperty("_UseAnisotropy")) shaderSetting.LIL_FEATURE_ANISOTROPY = shaderSetting.LIL_FEATURE_ANISOTROPY || material.GetFloat("_UseAnisotropy") != 0.0f;
            if(material.HasProperty("_UseReflection")) shaderSetting.LIL_FEATURE_REFLECTION = shaderSetting.LIL_FEATURE_REFLECTION || material.GetFloat("_UseReflection") != 0.0f;
            if(material.HasProperty("_UseMatCap")) shaderSetting.LIL_FEATURE_MATCAP = shaderSetting.LIL_FEATURE_MATCAP || material.GetFloat("_UseMatCap") != 0.0f;
            if(material.HasProperty("_UseMatCap2nd")) shaderSetting.LIL_FEATURE_MATCAP_2ND = shaderSetting.LIL_FEATURE_MATCAP_2ND || material.GetFloat("_UseMatCap2nd") != 0.0f;
            if(material.HasProperty("_UseRim")) shaderSetting.LIL_FEATURE_RIMLIGHT = shaderSetting.LIL_FEATURE_RIMLIGHT || material.GetFloat("_UseRim") != 0.0f;
            if(material.HasProperty("_RimDirStrength")) shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION = shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION || material.GetFloat("_RimDirStrength") != 0.0f;
            if(material.HasProperty("_UseGlitter")) shaderSetting.LIL_FEATURE_GLITTER = shaderSetting.LIL_FEATURE_GLITTER || material.GetFloat("_UseGlitter") != 0.0f;
            if(material.HasProperty("_UseBacklight")) shaderSetting.LIL_FEATURE_BACKLIGHT = shaderSetting.LIL_FEATURE_BACKLIGHT || material.GetFloat("_UseBacklight") != 0.0f;
            if(material.HasProperty("_UseParalla")) shaderSetting.LIL_FEATURE_PARALLAX = shaderSetting.LIL_FEATURE_PARALLAX || material.GetFloat("_UseParallax") != 0.0f;
            //if(material.HasProperty("")) shaderSetting.LIL_FEATURE_POM = shaderSetting.LIL_FEATURE_POM;
            if(material.HasProperty("_UseAudioLink")) shaderSetting.LIL_FEATURE_AUDIOLINK = shaderSetting.LIL_FEATURE_AUDIOLINK || material.GetFloat("_UseAudioLink") != 0.0f;
            if(material.HasProperty("_AudioLink2Vertex")) shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX = shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX || material.GetFloat("_AudioLink2Vertex") != 0.0f;
            if(material.HasProperty("_AudioLinkAsLocal")) shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL = shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL || material.GetFloat("_AudioLinkAsLocal") != 0.0f;
            if(material.HasProperty("_DissolveParams")) shaderSetting.LIL_FEATURE_DISSOLVE = shaderSetting.LIL_FEATURE_DISSOLVE || material.GetVector("_DissolveParams").x != defaultDissolveParams.x;
            if(material.HasProperty("_Main2ndBlendMask")) shaderSetting.LIL_FEATURE_TEX_LAYER_MASK = shaderSetting.LIL_FEATURE_TEX_LAYER_MASK || material.GetTexture("_Main2ndBlendMask") != null;
            if(material.HasProperty("_Main3rdBlendMask")) shaderSetting.LIL_FEATURE_TEX_LAYER_MASK = shaderSetting.LIL_FEATURE_TEX_LAYER_MASK || material.GetTexture("_Main3rdBlendMask") != null;
            if(material.HasProperty("_Main2ndDissolveNoiseMask")) shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE = shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE || material.GetTexture("_Main2ndDissolveNoiseMask") != null;
            if(material.HasProperty("_Main3rdDissolveNoiseMask")) shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE = shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE || material.GetTexture("_Main3rdDissolveNoiseMask") != null;
            if(material.HasProperty("_EmissionBlendMask")) shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK = shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK || material.GetTexture("_EmissionBlendMask") != null;
            if(material.HasProperty("_Emission2ndBlendMask")) shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK = shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK || material.GetTexture("_Emission2ndBlendMask") != null;
            if(material.HasProperty("_Bump2ndScaleMask")) shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK = shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK || material.GetTexture("_Bump2ndScaleMask") != null;
            if(material.HasProperty("_SmoothnessTex")) shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS = shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS || material.GetTexture("_SmoothnessTex") != null;
            if(material.HasProperty("_MetallicGlossMap")) shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC = shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC || material.GetTexture("_MetallicGlossMap") != null;
            if(material.HasProperty("_ReflectionColorTex")) shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR = shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR || material.GetTexture("_ReflectionColorTex") != null;
            if(material.HasProperty("_MatCapBlendMask")) shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK = shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK || material.GetTexture("_MatCapBlendMask") != null;
            if(material.HasProperty("_MatCap2ndBlendMask")) shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK = shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK || material.GetTexture("_MatCap2ndBlendMask") != null;
            if(material.HasProperty("_MatCapBumpMap")) shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP = shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP || material.GetTexture("_MatCapBumpMap") != null;
            if(material.HasProperty("_MatCap2ndBumpMap")) shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP = shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP || material.GetTexture("_MatCap2ndBumpMap") != null;
            if(material.HasProperty("_RimColorTex")) shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR = shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR || material.GetTexture("_RimColorTex") != null;
            if(material.HasProperty("_DissolveNoiseMask")) shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE = shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE || material.GetTexture("_DissolveNoiseMask") != null;

            // Outline
            if(material.shader.name.Contains("Outline"))
            {
                if(material.HasProperty("_OutlineTex_ScrollRotate")) shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV = shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV || material.GetVector("_OutlineTex_ScrollRotate") != defaultScrollRotate;
                if(material.HasProperty("_OutlineTexHSVG")) shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION = shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION || material.GetVector("_OutlineTexHSVG") != defaultHSVG;
                if(material.HasProperty("_OutlineTex")) shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR = shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR || material.GetTexture("_OutlineTex") != null;
                if(material.HasProperty("_OutlineWidthMask")) shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH = shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH || material.GetTexture("_OutlineWidthMask") != null;
                if(material.HasProperty("_OutlineVectorTex")) shaderSetting.LIL_FEATURE_TEX_OUTLINE_NORMAL = shaderSetting.LIL_FEATURE_TEX_OUTLINE_NORMAL || material.GetTexture("_OutlineVectorTex") != null;
            }

            // Tessellation
            shaderSetting.LIL_FEATURE_TEX_TESSELLATION = shaderSetting.LIL_FEATURE_TEX_TESSELLATION || material.shader.name.Contains("Tessellation");
        }

        public static void SetupMaterialFromShaderSetting(Material material, lilToonSetting shaderSetting)
        {
            if(!material.shader.name.Contains("lilToon") || material.shader.name.Contains("Lite") || material.shader.name.Contains("Multi")) return;

            if(!shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV) material.SetVector("_MainTex_ScrollRotate", defaultScrollRotate);
            if(!shaderSetting.LIL_FEATURE_SHADOW) material.SetFloat("_UseShadow", 0.0f);
            if(!shaderSetting.LIL_FEATURE_SHADOW || !shaderSetting.LIL_FEATURE_RECEIVE_SHADOW) material.SetFloat("_ShadowReceive", 0.0f);
            //if(!shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER);
            if(!shaderSetting.LIL_FEATURE_DISTANCE_FADE) material.SetVector("_DistanceFade", defaultDistanceFadeParams);
            if(!shaderSetting.LIL_FEATURE_ENCRYPTION) material.SetVector("_Keys", defaultKeys);
            if(!shaderSetting.LIL_FEATURE_SHADOW || !shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR) material.SetTexture("_ShadowBlurMask", null);
            if(!shaderSetting.LIL_FEATURE_SHADOW || !shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER)
            {
                material.SetTexture("_ShadowBorderMask", null);
                material.SetVector("_ShadowAOShift", new Vector4(1.0f,0.0f,1.0f,0.0f));
                material.SetVector("_ShadowAOShift2", new Vector4(1.0f,0.0f,1.0f,0.0f));
            }
            if(!shaderSetting.LIL_FEATURE_SHADOW || !shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH) material.SetTexture("_ShadowStrengthMask", null);
            if(!shaderSetting.LIL_FEATURE_SHADOW || !shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST) material.SetTexture("_ShadowColorTex", null);
            if(!shaderSetting.LIL_FEATURE_SHADOW || !shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND) material.SetTexture("_Shadow2ndColorTex", null);
            if(!shaderSetting.LIL_FEATURE_SHADOW || !shaderSetting.LIL_FEATURE_SHADOW_3RD) material.SetColor("_Shadow3rdColor", new Color(0.0f, 0.0f, 0.0f, 0.0f));
            if(!shaderSetting.LIL_FEATURE_SHADOW || !shaderSetting.LIL_FEATURE_TEX_SHADOW_3RD) material.SetTexture("_Shadow3rdColorTex", null);

            if(material.shader.name.Contains("Fur"))
            {
                if(!shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL) material.SetTexture("_FurVectorTex", null);
                if(!shaderSetting.LIL_FEATURE_TEX_FUR_MASK) material.SetTexture("_FurMask", null);
                if(!shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH) material.SetTexture("_FurLengthMask", null);
                if(!shaderSetting.LIL_FEATURE_FUR_COLLISION) material.SetFloat("_FurTouchStrength", 0.0f);
            }

            if(!shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION) material.SetVector("_MainTexHSVG", defaultHSVG);
            if(!shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP) material.SetFloat("_MainGradationStrength", 0.0f);
            if(!shaderSetting.LIL_FEATURE_MAIN2ND) material.SetFloat("_UseMain2ndTex", 0.0f);
            if(!shaderSetting.LIL_FEATURE_MAIN3RD) material.SetFloat("_UseMain3rdTex", 0.0f);
            if(!shaderSetting.LIL_FEATURE_DECAL)
            {
                material.SetFloat("_Main2ndTexIsDecal", 0.0f);
                material.SetFloat("_Main3rdTexIsDecal", 0.0f);
            }
            if(!shaderSetting.LIL_FEATURE_ANIMATE_DECAL)
            {
                material.SetVector("_Main2ndTexDecalAnimation", defaultDecalAnim);
                material.SetVector("_Main3rdTexDecalAnimation", defaultDecalAnim);
            }
            if(!shaderSetting.LIL_FEATURE_LAYER_DISSOLVE)
            {
                material.SetVector("_Main2ndDissolveParams", defaultDissolveParams);
                material.SetVector("_Main3rdDissolveParams", defaultDissolveParams);
            }
            if(!shaderSetting.LIL_FEATURE_ALPHAMASK) material.SetFloat("_AlphaMaskMode", 0.0f);
            if(!shaderSetting.LIL_FEATURE_EMISSION_1ST) material.SetFloat("_UseEmission", 0.0f);
            if(!shaderSetting.LIL_FEATURE_EMISSION_2ND) material.SetFloat("_UseEmission2nd", 0.0f);
            if(!shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV)
            {
                material.SetFloat("_EmissionMap_UVMode", 0.0f);
                material.SetFloat("_Emission2ndMap_UVMode", 0.0f);
                material.SetVector("_EmissionMap_ScrollRotate", defaultScrollRotate);
                material.SetVector("_Emission2ndMap_ScrollRotate", defaultScrollRotate);
            }
            if(!shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV)
            {
                material.SetVector("_EmissionBlendMask_ScrollRotate", defaultScrollRotate);
                material.SetVector("_Emission2ndBlendMask_ScrollRotate", defaultScrollRotate);
            }
            if(!shaderSetting.LIL_FEATURE_EMISSION_GRADATION) material.SetFloat("_EmissionUseGrad", 0.0f);
            if(!shaderSetting.LIL_FEATURE_NORMAL_1ST) material.SetFloat("_UseBumpMap", 0.0f);
            if(!shaderSetting.LIL_FEATURE_NORMAL_2ND) material.SetFloat("_UseBump2ndMap", 0.0f);
            if(!shaderSetting.LIL_FEATURE_ANISOTROPY) material.SetFloat("_UseAnisotropy", 0.0f);
            if(!shaderSetting.LIL_FEATURE_REFLECTION) material.SetFloat("_UseReflection", 0.0f);
            if(!shaderSetting.LIL_FEATURE_MATCAP) material.SetFloat("_UseMatCap", 0.0f);
            if(!shaderSetting.LIL_FEATURE_MATCAP_2ND) material.SetFloat("_UseMatCap2nd", 0.0f);
            if(!shaderSetting.LIL_FEATURE_RIMLIGHT) material.SetFloat("_UseRim", 0.0f);
            if(!shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION) material.SetFloat("_RimDirStrength", 0.0f);
            if(!shaderSetting.LIL_FEATURE_GLITTER) material.SetFloat("_UseGlitter", 0.0f);
            if(!shaderSetting.LIL_FEATURE_BACKLIGHT) material.SetFloat("_UseBacklight", 0.0f);
            if(!shaderSetting.LIL_FEATURE_PARALLAX) material.SetFloat("_UseParallax", 0.0f);
            //if(!shaderSetting.LIL_FEATURE_POM);
            if(!shaderSetting.LIL_FEATURE_AUDIOLINK) material.SetFloat("_UseAudioLink", 0.0f);
            if(!shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX) material.SetFloat("_AudioLink2Vertex", 0.0f);
            if(!shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL) material.SetFloat("_AudioLinkAsLocal", 0.0f);
            if(!shaderSetting.LIL_FEATURE_DISSOLVE) material.SetVector("_DissolveParams", defaultDissolveParams);
            if(!shaderSetting.LIL_FEATURE_TEX_LAYER_MASK)
            {
                material.SetTexture("_Main2ndBlendMask", null);
                material.SetTexture("_Main3rdBlendMask", null);
            }
            if(!shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE)
            {
                material.SetTexture("_Main2ndDissolveNoiseMask", null);
                material.SetTexture("_Main3rdDissolveNoiseMask", null);
            }
            if(!shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK)
            {
                material.SetTexture("_EmissionBlendMask", null);
                material.SetTexture("_Emission2ndBlendMask", null);
            }
            if(!shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK) material.SetTexture("_Bump2ndScaleMask", null);
            if(!shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS) material.SetTexture("_SmoothnessTex", null);
            if(!shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC) material.SetTexture("_MetallicGlossMap", null);
            if(!shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR) material.SetTexture("_ReflectionColorTex", null);
            if(!shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK)
            {
                material.SetTexture("_MatCapBlendMask", null);
                material.SetTexture("_MatCap2ndBlendMask", null);
            }
            if(!shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP)
            {
                material.SetTexture("_MatCapBumpMap", null);
                material.SetTexture("_MatCap2ndBumpMap", null);
            }
            if(!shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR) material.SetTexture("_RimColorTex", null);
            if(!shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE) material.SetTexture("_DissolveNoiseMask", null);

            // Outline
            if(material.shader.name.Contains("Outline"))
            {
                if(!shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV) material.SetVector("_OutlineTex_ScrollRotate", defaultScrollRotate);
                if(!shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION) material.SetVector("_OutlineTexHSVG", defaultHSVG);
                if(!shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR) material.SetTexture("_OutlineTex", null);
                if(!shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH) material.SetTexture("_OutlineWidthMask", null);
                if(!shaderSetting.LIL_FEATURE_TEX_OUTLINE_NORMAL) material.SetTexture("_OutlineVectorTex", null);
            }

            // Tessellation
            //if(!shaderSetting.LIL_FEATURE_TEX_TESSELLATION);
        }

        public static void SetupShaderSettingFromAnimationClip(AnimationClip clip, ref lilToonSetting shaderSetting)
        {
            if(shaderSetting.isLocked) return;
            if(clip == null) return;

            foreach(EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
            {
                string propname = binding.propertyName;
                if(string.IsNullOrEmpty(propname) || !propname.Contains("material.")) continue;

                shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV = shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV || propname.Contains("_MainTex_ScrollRotate");
                shaderSetting.LIL_FEATURE_SHADOW = shaderSetting.LIL_FEATURE_SHADOW || propname.Contains("_UseShadow");
                shaderSetting.LIL_FEATURE_RECEIVE_SHADOW = shaderSetting.LIL_FEATURE_RECEIVE_SHADOW || propname.Contains("_ShadowReceive");
                //shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER = shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER;
                //shaderSetting.LIL_FEATURE_DISTANCE_FADE = shaderSetting.LIL_FEATURE_DISTANCE_FADE || propname.Contains("_DistanceFade");
                //shaderSetting.LIL_FEATURE_ENCRYPTION = shaderSetting.LIL_FEATURE_ENCRYPTION || propname.Contains("_Keys");
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
                //shaderSetting.LIL_FEATURE_POM = shaderSetting.LIL_FEATURE_POM;
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

        public static bool CheckMainTextureName(string name)
        {
            return mainTexCheckWords.Any(word => !name.Contains(word));
        }

        public static void RemoveUnusedTexture(Material material)
        {
            if(!material.shader.name.Contains("lilToon")) return;
            lilToonSetting shaderSetting = null;
            InitializeShaderSetting(ref shaderSetting);
            RemoveUnusedTexture(material, material.shader.name.Contains("Lite"), shaderSetting);
        }

        public static void SetShaderKeywords(Material material, string keyword, bool enable)
        {
            if(enable)  material.EnableKeyword(keyword);
            else        material.DisableKeyword(keyword);
        }

        public void SetupMultiMaterial(Material material)
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
                SetShaderKeywords(material, "PIXELSNAP_ON",                         useParallax.floatValue != 0.0f && material.GetFloat("_UsePOM") != 0.0f);
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

        private static void RemoveUnusedTexture(Material material, bool islite, lilToonSetting shaderSetting)
        {
            if(!isMulti) SetupMaterialFromShaderSetting(material, shaderSetting);
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
        public static void ApplyPreset(Material material, lilToonPreset preset)
        {
            if(material == null || preset == null) return;
            for(int i = 0; i < preset.floats.Length; i++)
            {
                if(preset.floats[i].name == "_StencilPass") material.SetFloat(preset.floats[i].name, preset.floats[i].value);
            }
            if(preset.shader != null) material.shader = preset.shader;
            bool isoutl         = preset.outline;
            bool istess         = preset.tessellation;
            bool isstencil      = material.GetFloat("_StencilPass") == (float)UnityEngine.Rendering.StencilOp.Replace;

            bool islite         = material.shader.name.Contains("Lite");
            bool iscutout       = material.shader.name.Contains("Cutout");
            bool istransparent  = material.shader.name.Contains("Transparent");
            bool isrefr         = preset.shader != null && preset.shader.name.Contains("Refraction");
            bool isblur         = preset.shader != null && preset.shader.name.Contains("Blur");
            bool isfur          = preset.shader != null && preset.shader.name.Contains("Fur");
            bool isonepass      = material.shader.name.Contains("OnePass");
            bool istwopass      = material.shader.name.Contains("TwoPass");

            RenderingMode           renderingMode = RenderingMode.Opaque;
            if(iscutout)            renderingMode = RenderingMode.Cutout;
            if(istransparent)       renderingMode = RenderingMode.Transparent;
            if(isrefr)              renderingMode = RenderingMode.Refraction;
            if(isrefr && isblur)    renderingMode = RenderingMode.RefractionBlur;
            if(isfur)               renderingMode = RenderingMode.Fur;
            if(isfur && iscutout)   renderingMode = RenderingMode.FurCutout;
            if(isfur && istwopass)  renderingMode = RenderingMode.FurTwoPass;

            TransparentMode         transparentMode = TransparentMode.Normal;
            if(isonepass)           transparentMode = TransparentMode.OnePass;
            if(!isfur && istwopass) transparentMode = TransparentMode.TwoPass;

            SetupMaterialWithRenderingMode(material, renderingMode, transparentMode, isoutl, islite, isstencil, istess);
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

        private static void DrawPreset()
        {
            GUILayout.Label(GetLoc("sPresets"), boldLabel);
            if(presets == null) LoadPresets();
            ShowPresets();
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            if(GUILayout.Button(GetLoc("sPresetRefresh"))) LoadPresets();
            if(GUILayout.Button(GetLoc("sPresetSave"))) EditorWindow.GetWindow<lilPresetWindow>();
            GUILayout.EndHorizontal();
        }

        private static void LoadPresets()
        {
            string[] presetGuid = AssetDatabase.FindAssets("t:lilToonPreset", new[] {GetPresetsFolderPath()});
            Array.Resize(ref presets, presetGuid.Length);
            for(int i=0; i<presetGuid.Length; i++)
            {
                presets[i] = AssetDatabase.LoadAssetAtPath<lilToonPreset>(AssetDatabase.GUIDToAssetPath(presetGuid[i]));
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
                            if(GUILayout.Button(showName))
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
                liteMaterial.SetFloat("_OutlineEnableLighting",     outlineEnableLighting.floatValue);
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
            material.SetFloat("_UsePOM", 0.0f);
            material.SetFloat("_UseClippingCanceller", 0.0f);

            if(shaderSetting.LIL_FEATURE_POM) material.SetFloat("_UsePOM", 1.0f);
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
                        SetupMaterialWithRenderingMode(material, renderingMode, transparentModeBuf, isOutl, isLite, isStWr, isTess);
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
                        SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, isOutl, isLite, isStWr, isTess);
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
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentMode, isOutl, isLite, isStWr, isTess);
                        }
                        if(transparentModeBuf >= TransparentMode.OnePass && vertexLightStrength.floatValue != 1.0f && AutoFixHelpBox(GetLoc("sHelpOnePassVertexLight")))
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
                        EditorGUILayout.HelpBox(GetLoc("sHelpRenderingTransparent"),MessageType.Warning);
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
                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH)) m_MaterialEditor.TexturePropertySingleLine(maskStrengthContent, shadowStrengthMask, shadowStrength);
                    else                                                            m_MaterialEditor.ShaderProperty(shadowStrength, GetLoc("sStrength"));
                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR))     m_MaterialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sBlurMask"), GetLoc("sBlurR")), shadowBlurMask);
                    DrawLine();
                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST))      m_MaterialEditor.TexturePropertySingleLine(shadow1stColorRGBAContent, shadowColorTex, shadowColor);
                    else                                                            m_MaterialEditor.ShaderProperty(shadowColor, GetLoc("sShadow1stColor"));
                    m_MaterialEditor.ShaderProperty(shadowBorder, GetLoc("sBorder"));
                    m_MaterialEditor.ShaderProperty(shadowBlur, GetLoc("sBlur"));
                    m_MaterialEditor.ShaderProperty(shadowNormalStrength, GetLoc("sNormalStrength"));
                    DrawLine();
                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND))      m_MaterialEditor.TexturePropertySingleLine(shadow2ndColorRGBAContent, shadow2ndColorTex, shadow2ndColor);
                    else                                                            m_MaterialEditor.ShaderProperty(shadow2ndColor, GetLoc("sShadow2ndColor"));
                    if(shadow2ndColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                    {
                        shadow2ndColor.colorValue = new Color(shadow2ndColor.colorValue.r, shadow2ndColor.colorValue.g, shadow2ndColor.colorValue.b, 1.0f);
                    }
                    m_MaterialEditor.ShaderProperty(shadow2ndBorder, GetLoc("sBorder"));
                    m_MaterialEditor.ShaderProperty(shadow2ndBlur, GetLoc("sBlur"));
                    m_MaterialEditor.ShaderProperty(shadow2ndNormalStrength, GetLoc("sNormalStrength"));
                    DrawLine();
                    if(CheckFeature(shaderSetting.LIL_FEATURE_SHADOW_3RD))
                    {
                        if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_SHADOW_3RD))      m_MaterialEditor.TexturePropertySingleLine(shadow3rdColorRGBAContent, shadow3rdColorTex, shadow3rdColor);
                        else                                                            m_MaterialEditor.ShaderProperty(shadow3rdColor, GetLoc("sShadow3rdColor"));
                        if(shadow3rdColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                        {
                            shadow3rdColor.colorValue = new Color(shadow3rdColor.colorValue.r, shadow3rdColor.colorValue.g, shadow3rdColor.colorValue.b, 1.0f);
                        }
                        m_MaterialEditor.ShaderProperty(shadow3rdBorder, GetLoc("sBorder"));
                        m_MaterialEditor.ShaderProperty(shadow3rdBlur, GetLoc("sBlur"));
                        m_MaterialEditor.ShaderProperty(shadow3rdNormalStrength, GetLoc("sNormalStrength"));
                        DrawLine();
                    }
                    m_MaterialEditor.ShaderProperty(shadowBorderColor, GetLoc("sShadowBorderColor"));
                    m_MaterialEditor.ShaderProperty(shadowBorderRange, GetLoc("sShadowBorderRange"));
                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER))
                    {
                        DrawLine();
                        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("AO Map", GetLoc("sBorderR")), shadowBorderMask);
                        EditorGUI.indentLevel++;
                        m_MaterialEditor.ShaderProperty(shadowPostAO, GetLoc("sIgnoreBorderProperties"));
                        m_MaterialEditor.ShaderProperty(shadowAOShift, "1st Scale|1st Offset|2nd Scale|2nd Offset");
                        if(CheckFeature(shaderSetting.LIL_FEATURE_SHADOW_3RD)) m_MaterialEditor.ShaderProperty(shadowAOShift2, "3rd Scale|3rd Offset");
                        EditorGUI.indentLevel--;
                    }
                    DrawLine();
                    m_MaterialEditor.ShaderProperty(shadowMainStrength, GetLoc("sContrast"));
                    m_MaterialEditor.ShaderProperty(shadowEnvStrength, GetLoc("sShadowEnvStrength"));
                    if(CheckFeature(shaderSetting.LIL_FEATURE_RECEIVE_SHADOW)) m_MaterialEditor.ShaderProperty(shadowReceive, GetLoc("sReceiveShadow"));
                    EditorGUILayout.EndVertical();
                }
                else if(useShadow.floatValue == 1)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    m_MaterialEditor.TexturePropertySingleLine(shadow1stColorRGBAContent, shadowColorTex);
                    m_MaterialEditor.ShaderProperty(shadowBorder, GetLoc("sBorder"));
                    m_MaterialEditor.ShaderProperty(shadowBlur, GetLoc("sBlur"));
                    DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(shadow2ndColorRGBAContent, shadow2ndColorTex);
                    m_MaterialEditor.ShaderProperty(shadow2ndBorder, GetLoc("sBorder"));
                    m_MaterialEditor.ShaderProperty(shadow2ndBlur, GetLoc("sBlur"));
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
                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH)) m_MaterialEditor.TexturePropertySingleLine(maskStrengthContent, shadowStrengthMask, shadowStrength);
                    else                                                            m_MaterialEditor.ShaderProperty(shadowStrength, GetLoc("sStrength"));
                    DrawLine();
                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST))      m_MaterialEditor.TexturePropertySingleLine(shadow1stColorRGBAContent, shadowColorTex, shadowColor);
                    else                                                            m_MaterialEditor.ShaderProperty(shadowColor, GetLoc("sShadow1stColor"));
                    m_MaterialEditor.ShaderProperty(shadowBorder, GetLoc("sBorder"));
                    m_MaterialEditor.ShaderProperty(shadowBlur, GetLoc("sBlur"));
                    m_MaterialEditor.ShaderProperty(shadowNormalStrength, GetLoc("sNormalStrength"));
                    DrawLine();
                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND))      m_MaterialEditor.TexturePropertySingleLine(shadow2ndColorRGBAContent, shadow2ndColorTex, shadow2ndColor);
                    else                                                            m_MaterialEditor.ShaderProperty(shadow2ndColor, GetLoc("sShadow2ndColor"));
                    if(shadow2ndColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                    {
                        shadow2ndColor.colorValue = new Color(shadow2ndColor.colorValue.r, shadow2ndColor.colorValue.g, shadow2ndColor.colorValue.b, 1.0f);
                    }
                    m_MaterialEditor.ShaderProperty(shadow2ndBorder, GetLoc("sBorder"));
                    m_MaterialEditor.ShaderProperty(shadow2ndBlur, GetLoc("sBlur"));
                    m_MaterialEditor.ShaderProperty(shadow2ndNormalStrength, GetLoc("sNormalStrength"));
                    DrawLine();
                    if(CheckFeature(shaderSetting.LIL_FEATURE_SHADOW_3RD))
                    {
                        if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_SHADOW_3RD))      m_MaterialEditor.TexturePropertySingleLine(shadow3rdColorRGBAContent, shadow3rdColorTex, shadow3rdColor);
                        else                                                            m_MaterialEditor.ShaderProperty(shadow3rdColor, GetLoc("sShadow3rdColor"));
                        if(shadow3rdColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                        {
                            shadow3rdColor.colorValue = new Color(shadow3rdColor.colorValue.r, shadow3rdColor.colorValue.g, shadow3rdColor.colorValue.b, 1.0f);
                        }
                        m_MaterialEditor.ShaderProperty(shadow3rdBorder, GetLoc("sBorder"));
                        m_MaterialEditor.ShaderProperty(shadow3rdBlur, GetLoc("sBlur"));
                        m_MaterialEditor.ShaderProperty(shadow3rdNormalStrength, GetLoc("sNormalStrength"));
                        DrawLine();
                    }
                    m_MaterialEditor.ShaderProperty(shadowBorderColor, GetLoc("sShadowBorderColor"));
                    m_MaterialEditor.ShaderProperty(shadowBorderRange, GetLoc("sShadowBorderRange"));
                    DrawLine();
                    m_MaterialEditor.ShaderProperty(shadowMainStrength, GetLoc("sContrast"));
                    m_MaterialEditor.ShaderProperty(shadowEnvStrength, GetLoc("sShadowEnvStrength"));
                    if(CheckFeature(shaderSetting.LIL_FEATURE_RECEIVE_SHADOW)) m_MaterialEditor.ShaderProperty(shadowReceive, GetLoc("sReceiveShadow"));
                    EditorGUILayout.EndVertical();
                }
                else if(useShadow.floatValue == 1)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    m_MaterialEditor.TexturePropertySingleLine(shadow1stColorRGBAContent, shadowColorTex);
                    m_MaterialEditor.ShaderProperty(shadowBorder, GetLoc("sBorder"));
                    m_MaterialEditor.ShaderProperty(shadowBlur, GetLoc("sBlur"));
                    DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(shadow2ndColorRGBAContent, shadow2ndColorTex);
                    m_MaterialEditor.ShaderProperty(shadow2ndBorder, GetLoc("sBorder"));
                    m_MaterialEditor.ShaderProperty(shadow2ndBlur, GetLoc("sBlur"));
                    DrawLine();
                    m_MaterialEditor.ShaderProperty(shadowEnvStrength, GetLoc("sShadowEnvStrength"));
                    m_MaterialEditor.ShaderProperty(shadowBorderColor, GetLoc("sShadowBorderColor"));
                    m_MaterialEditor.ShaderProperty(shadowBorderRange, GetLoc("sShadowBorderRange"));
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
                        SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, !isOutl, isLite, isStWr, isTess);
                    }
                }
                else if(isCustomShader)
                {
                    EditorGUILayout.LabelField(GetLoc("sOutline"), customToggleFont);
                }
                if(isOutl)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR))   TextureGUI(ref edSet.isShowOutlineMap, colorRGBAContent, outlineTex, outlineColor, outlineTex_ScrollRotate, true, CheckFeature(shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV));
                    else                                                            m_MaterialEditor.ShaderProperty(outlineColor, GetLoc("sColor"));
                    if(CheckFeature(shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION))
                    {
                        ToneCorrectionGUI(outlineTexHSVG);
                        if(GUILayout.Button(GetLoc("sBake")))
                        {
                            outlineTex.textureValue = AutoBakeOutlineTexture(material);
                            outlineTexHSVG.vectorValue = defaultHSVG;
                        }
                        DrawLine();
                    }
                    m_MaterialEditor.ShaderProperty(outlineEnableLighting, GetLoc("sEnableLighting"));
                    DrawLine();
                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH))   m_MaterialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sWidth"), GetLoc("sWidthR")), outlineWidthMask, outlineWidth);
                    else                                                            m_MaterialEditor.ShaderProperty(outlineWidth, GetLoc("sWidth"));
                    m_MaterialEditor.ShaderProperty(outlineFixWidth, GetLoc("sFixWidth"));
                    m_MaterialEditor.ShaderProperty(outlineVertexR2Width, sOutlineVertexColorUsages);
                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_OUTLINE_NORMAL))
                    {
                        DrawLine();
                        m_MaterialEditor.TexturePropertySingleLine(normalMapContent, outlineVectorTex, outlineVectorScale);
                    }
                    EditorGUILayout.EndVertical();
                }
                else if(isLite && isOutl)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    TextureGUI(ref edSet.isShowOutlineMap, colorRGBAContent, outlineTex, outlineColor, outlineTex_ScrollRotate, true, true);
                    m_MaterialEditor.ShaderProperty(outlineEnableLighting, GetLoc("sEnableLighting"));
                    DrawLine();
                    m_MaterialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sWidth"), GetLoc("sWidthR")), outlineWidthMask, outlineWidth);
                    m_MaterialEditor.ShaderProperty(outlineFixWidth, GetLoc("sFixWidth"));
                    m_MaterialEditor.ShaderProperty(outlineVertexR2Width, sOutlineVertexColorUsages);
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
                        SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, !isOutl, isLite, isStWr, isTess);
                    }
                }
                else if(isCustomShader)
                {
                    EditorGUILayout.LabelField(GetLoc("sOutline"), customToggleFont);
                }
                if(isOutl)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR))   TextureGUI(ref edSet.isShowOutlineMap, colorRGBAContent, outlineTex, outlineColor, outlineTex_ScrollRotate, true, CheckFeature(shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV));
                    else                                                            m_MaterialEditor.ShaderProperty(outlineColor, GetLoc("sColor"));
                    if(CheckFeature(shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION))
                    {
                        ToneCorrectionGUI(outlineTexHSVG);
                        if(GUILayout.Button(GetLoc("sBake")))
                        {
                            outlineTex.textureValue = AutoBakeOutlineTexture(material);
                            outlineTexHSVG.vectorValue = defaultHSVG;
                        }
                    }
                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH))   m_MaterialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sWidth"), GetLoc("sWidthR")), outlineWidthMask, outlineWidth);
                    else                                                            m_MaterialEditor.ShaderProperty(outlineWidth, GetLoc("sWidth"));
                    EditorGUILayout.EndVertical();
                }
                else if(isLite && isOutl)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    TextureGUI(ref edSet.isShowOutlineMap, colorRGBAContent, outlineTex, outlineColor, outlineTex_ScrollRotate, true, true);
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
            if(CheckFeature(shaderSetting.LIL_FEATURE_DECAL))
            {
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
            }

            if(CheckFeature(shaderSetting.LIL_FEATURE_DECAL) && isDecal.floatValue == 1.0f)
            {
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

                if(CheckFeature(shaderSetting.LIL_FEATURE_ANIMATE_DECAL))
                {
                    m_MaterialEditor.ShaderProperty(decalAnimation, BuildParams(GetLoc("sAnimation"), GetLoc("sXFrames"), GetLoc("sYFrames"), GetLoc("sFrames"), GetLoc("sFPS")));
                    m_MaterialEditor.ShaderProperty(decalSubParam, BuildParams(GetLoc("sXRatio"), GetLoc("sYRatio"), GetLoc("sFixBorder")));
                }
            }
            else
            {
                m_MaterialEditor.TextureScaleOffsetProperty(tex);
                m_MaterialEditor.ShaderProperty(angle, GetLoc("sAngle"));
            }

            if(GUILayout.Button(GetLoc("sReset")) && EditorUtility.DisplayDialog(GetLoc("sDialogResetUV"),GetLoc("sDialogResetUVMes"),GetLoc("sYes"),GetLoc("sNo")))
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
            if(GUILayout.Button(GetLoc("sReset")))
            {
                hsvg.vectorValue = defaultHSVG;
            }
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
            if(GUILayout.Button(sBake[bakeType]))
            {
                TextureBake(material, bakeType);
            }
        }

        private void AlphamaskToTextureGUI(Material material)
        {
            if(mainTex.textureValue != null && GUILayout.Button(GetLoc("sBakeAlphamask")))
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
            // Make space for foldout
            EditorGUI.indentLevel++;
            Rect rect = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(rect, labelName);
            EditorGUI.indentLevel--;

            rect.x += isCustomEditor ? 0 : 10;
            isShow = EditorGUI.Foldout(rect, isShow, "");
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
            // Make space for foldout
            EditorGUI.indentLevel++;
            Rect rect = m_MaterialEditor.TexturePropertySingleLine(guiContent, textureName);
            EditorGUI.indentLevel--;
            rect.x += isCustomEditor ? 0 : 10;
            isShow = EditorGUI.Foldout(rect, isShow, "");
            if(isShow)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(textureName);
                EditorGUI.indentLevel--;
            }
        }

        private void TextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba)
        {
            // Make space for foldout
            EditorGUI.indentLevel++;
            Rect rect = m_MaterialEditor.TexturePropertySingleLine(guiContent, textureName, rgba);
            EditorGUI.indentLevel--;
            rect.x += isCustomEditor ? 0 : 10;
            isShow = EditorGUI.Foldout(rect, isShow, "");
            if(isShow)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(textureName);
                EditorGUI.indentLevel--;
            }
        }

        private void TextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty scrollRotate)
        {
            // Make space for foldout
            EditorGUI.indentLevel++;
            Rect rect = m_MaterialEditor.TexturePropertySingleLine(guiContent, textureName, rgba);
            EditorGUI.indentLevel--;
            rect.x += isCustomEditor ? 0 : 10;
            isShow = EditorGUI.Foldout(rect, isShow, "");
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
                // Make space for foldout
                EditorGUI.indentLevel++;
                Rect rect = m_MaterialEditor.TexturePropertySingleLine(guiContent, textureName, rgba);
                EditorGUI.indentLevel--;
                rect.x += isCustomEditor ? 0 : 10;
                isShow = EditorGUI.Foldout(rect, isShow, "");
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
                // Make space for foldout
                EditorGUI.indentLevel++;
                Rect rect = m_MaterialEditor.TexturePropertySingleLine(guiContent, textureName, rgba);
                EditorGUI.indentLevel--;
                rect.x += isCustomEditor ? 0 : 10;
                isShow = EditorGUI.Foldout(rect, isShow, "");
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
            // Make space for foldout
            EditorGUI.indentLevel++;
            Rect rect = m_MaterialEditor.TexturePropertySingleLine(guiContent, textureName);
            EditorGUI.indentLevel--;
            rect.x += isCustomEditor ? 0 : 10;
            isShow = EditorGUI.Foldout(rect, isShow, "");
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
            // Make space for foldout
            EditorGUI.indentLevel++;
            Rect rect = m_MaterialEditor.TexturePropertySingleLine(guiContent, textureName, rgba);
            EditorGUI.indentLevel--;
            rect.x += isCustomEditor ? 0 : 10;
            isShow = EditorGUI.Foldout(rect, isShow, "");
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
                byte[] bytes;

                Texture2D srcTexture = new Texture2D(2, 2);
                Texture2D srcMain2 = new Texture2D(2, 2);
                Texture2D srcMain3 = new Texture2D(2, 2);
                Texture2D srcMask2 = new Texture2D(2, 2);
                Texture2D srcMask3 = new Texture2D(2, 2);

                hsvgMaterial.SetColor(mainColor.name,           mainColor.colorValue);
                hsvgMaterial.SetVector(mainTexHSVG.name,        mainTexHSVG.vectorValue);
                hsvgMaterial.SetFloat(mainGradationStrength.name, 0.0f);

                if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP))
                {
                    hsvgMaterial.SetFloat(mainGradationStrength.name, mainGradationStrength.floatValue);
                    hsvgMaterial.SetTexture(mainGradationTex.name, mainGradationTex.textureValue);
                }

                path = AssetDatabase.GetAssetPath(material.GetTexture(mainTex.name));
                if(!string.IsNullOrEmpty(path))
                {
                    bytes = File.ReadAllBytes(Path.GetFullPath(path));
                    srcTexture.LoadImage(bytes);
                    srcTexture.filterMode = FilterMode.Bilinear;
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
                        bytes = File.ReadAllBytes(Path.GetFullPath(path));
                        srcMain2.LoadImage(bytes);
                        srcMain2.filterMode = FilterMode.Bilinear;
                        hsvgMaterial.SetTexture(main2ndTex.name, srcMain2);
                    }
                    else
                    {
                        hsvgMaterial.SetTexture(main2ndTex.name, Texture2D.whiteTexture);
                    }

                    path = AssetDatabase.GetAssetPath(material.GetTexture(main2ndBlendMask.name));
                    if(!string.IsNullOrEmpty(path))
                    {
                        bytes = File.ReadAllBytes(Path.GetFullPath(path));
                        srcMask2.LoadImage(bytes);
                        srcMask2.filterMode = FilterMode.Bilinear;
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
                        bytes = File.ReadAllBytes(Path.GetFullPath(path));
                        srcMain3.LoadImage(bytes);
                        srcMain3.filterMode = FilterMode.Bilinear;
                        hsvgMaterial.SetTexture(main3rdTex.name, srcMain3);
                    }
                    else
                    {
                        hsvgMaterial.SetTexture(main3rdTex.name, Texture2D.whiteTexture);
                    }

                    path = AssetDatabase.GetAssetPath(material.GetTexture(main3rdBlendMask.name));
                    if(!string.IsNullOrEmpty(path))
                    {
                        bytes = File.ReadAllBytes(Path.GetFullPath(path));
                        srcMask3.LoadImage(bytes);
                        srcMask3.filterMode = FilterMode.Bilinear;
                        hsvgMaterial.SetTexture(main3rdBlendMask.name, srcMask3);
                    }
                    else
                    {
                        hsvgMaterial.SetTexture(main3rdBlendMask.name, Texture2D.whiteTexture);
                    }
                }

                RenderTexture dstTexture = new RenderTexture(srcTexture.width, srcTexture.height, 0, RenderTextureFormat.ARGB32);

                Graphics.Blit(srcTexture, dstTexture, hsvgMaterial);

                Texture2D outTexture = new Texture2D(srcTexture.width, srcTexture.height);
                outTexture.ReadPixels(new Rect(0, 0, srcTexture.width, srcTexture.height), 0, 0);
                outTexture.Apply();

                outTexture = SaveTextureToPng(material, outTexture, mainTex.name);
                if(outTexture != mainTex.textureValue)
                {
                    mainTexHSVG.vectorValue = defaultHSVG;
                    mainColor.colorValue = Color.white;
                    if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP))
                    {
                        mainGradationStrength.floatValue = 0.0f;
                        mainGradationTex.textureValue = null;
                    }
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
                UnityEngine.Object.DestroyImmediate(dstTexture);
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
                byte[] bytes;

                Texture2D srcTexture = new Texture2D(2, 2);
                Texture2D srcMain2 = new Texture2D(2, 2);
                Texture2D srcMain3 = new Texture2D(2, 2);
                Texture2D srcMask2 = new Texture2D(2, 2);
                Texture2D srcMask3 = new Texture2D(2, 2);

                hsvgMaterial.SetColor(mainColor.name,           Color.white);
                hsvgMaterial.SetVector(mainTexHSVG.name,        mainTexHSVG.vectorValue);
                hsvgMaterial.SetFloat(mainGradationStrength.name, 0.0f);
                hsvgMaterial.SetTexture(mainColorAdjustMask.name, mainColorAdjustMask.textureValue);

                if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP))
                {
                    hsvgMaterial.SetFloat(mainGradationStrength.name, mainGradationStrength.floatValue);
                    hsvgMaterial.SetTexture(mainGradationTex.name, mainGradationTex.textureValue);
                }

                path = AssetDatabase.GetAssetPath(material.GetTexture(mainTex.name));
                if(!string.IsNullOrEmpty(path))
                {
                    bytes = File.ReadAllBytes(Path.GetFullPath(path));
                    srcTexture.LoadImage(bytes);
                    srcTexture.filterMode = FilterMode.Bilinear;
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
                        bytes = File.ReadAllBytes(Path.GetFullPath(path));
                        srcMain2.LoadImage(bytes);
                        srcMain2.filterMode = FilterMode.Bilinear;
                        hsvgMaterial.SetTexture(main2ndTex.name, srcMain2);
                    }
                    else
                    {
                        hsvgMaterial.SetTexture(main2ndTex.name, Texture2D.whiteTexture);
                    }

                    path = AssetDatabase.GetAssetPath(material.GetTexture(main2ndBlendMask.name));
                    if(!string.IsNullOrEmpty(path))
                    {
                        bytes = File.ReadAllBytes(Path.GetFullPath(path));
                        srcMask2.LoadImage(bytes);
                        srcMask2.filterMode = FilterMode.Bilinear;
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
                        bytes = File.ReadAllBytes(Path.GetFullPath(path));
                        srcMain3.LoadImage(bytes);
                        srcMain3.filterMode = FilterMode.Bilinear;
                        hsvgMaterial.SetTexture(main3rdTex.name, srcMain3);
                    }
                    else
                    {
                        hsvgMaterial.SetTexture(main3rdTex.name, Texture2D.whiteTexture);
                    }

                    path = AssetDatabase.GetAssetPath(material.GetTexture(main3rdBlendMask.name));
                    if(!string.IsNullOrEmpty(path))
                    {
                        bytes = File.ReadAllBytes(Path.GetFullPath(path));
                        srcMask3.LoadImage(bytes);
                        srcMask3.filterMode = FilterMode.Bilinear;
                        hsvgMaterial.SetTexture(main3rdBlendMask.name, srcMask3);
                    }
                    else
                    {
                        hsvgMaterial.SetTexture(main3rdBlendMask.name, Texture2D.whiteTexture);
                    }
                }

                RenderTexture dstTexture = new RenderTexture(srcTexture.width, srcTexture.height, 0, RenderTextureFormat.ARGB32);

                Graphics.Blit(srcTexture, dstTexture, hsvgMaterial);

                Texture2D outTexture = new Texture2D(srcTexture.width, srcTexture.height);
                outTexture.ReadPixels(new Rect(0, 0, srcTexture.width, srcTexture.height), 0, 0);
                outTexture.Apply();

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
                UnityEngine.Object.DestroyImmediate(dstTexture);

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
                byte[] bytes;

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
                    bytes = File.ReadAllBytes(Path.GetFullPath(path));
                    srcMain2.LoadImage(bytes);
                    srcMain2.filterMode = FilterMode.Bilinear;
                    hsvgMaterial.SetTexture(main2ndTex.name, srcMain2);
                }

                path = AssetDatabase.GetAssetPath(bakedMainTex);
                if(!string.IsNullOrEmpty(path))
                {
                    bytes = File.ReadAllBytes(Path.GetFullPath(path));
                    srcTexture.LoadImage(bytes);
                    srcTexture.filterMode = FilterMode.Bilinear;
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
                    bytes = File.ReadAllBytes(Path.GetFullPath(path));
                    srcMask2.LoadImage(bytes);
                    srcMask2.filterMode = FilterMode.Bilinear;
                    hsvgMaterial.SetTexture(main2ndBlendMask.name, srcMask2);
                    hsvgMaterial.SetTexture(main3rdBlendMask.name, srcMask2);
                }
                else
                {
                    hsvgMaterial.SetTexture(main2ndBlendMask.name, Texture2D.whiteTexture);
                    hsvgMaterial.SetTexture(main3rdBlendMask.name, Texture2D.whiteTexture);
                }

                RenderTexture dstTexture = new RenderTexture(srcTexture.width, srcTexture.height, 0, RenderTextureFormat.ARGB32);

                Graphics.Blit(srcTexture, dstTexture, hsvgMaterial);

                Texture2D outTexture = new Texture2D(srcTexture.width, srcTexture.height);
                outTexture.ReadPixels(new Rect(0, 0, srcTexture.width, srcTexture.height), 0, 0);
                outTexture.Apply();

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
                UnityEngine.Object.DestroyImmediate(dstTexture);

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
                byte[] bytes;

                Texture2D srcTexture = new Texture2D(2, 2);

                hsvgMaterial.SetColor(mainColor.name,           matcapColor.colorValue);
                hsvgMaterial.SetVector(mainTexHSVG.name,        defaultHSVG);

                path = AssetDatabase.GetAssetPath(material.GetTexture(matcapTex.name));
                if(!string.IsNullOrEmpty(path))
                {
                    bytes = File.ReadAllBytes(Path.GetFullPath(path));
                    srcTexture.LoadImage(bytes);
                    srcTexture.filterMode = FilterMode.Bilinear;
                    hsvgMaterial.SetTexture(mainTex.name, srcTexture);
                }
                else
                {
                    hsvgMaterial.SetTexture(mainTex.name, Texture2D.whiteTexture);
                }

                RenderTexture dstTexture = new RenderTexture(srcTexture.width, srcTexture.height, 0, RenderTextureFormat.ARGB32);

                Graphics.Blit(srcTexture, dstTexture, hsvgMaterial);

                Texture2D outTexture = new Texture2D(srcTexture.width, srcTexture.height);
                outTexture.ReadPixels(new Rect(0, 0, srcTexture.width, srcTexture.height), 0, 0);
                outTexture.Apply();

                outTexture = SaveTextureToPng(material, outTexture, matcapTex.name);
                if(outTexture != matcapTex.textureValue)
                {
                    CopyTextureSetting(bufMainTexture, outTexture);
                }

                UnityEngine.Object.DestroyImmediate(hsvgMaterial);
                UnityEngine.Object.DestroyImmediate(srcTexture);
                UnityEngine.Object.DestroyImmediate(dstTexture);

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
                byte[] bytes;

                Texture2D srcTexture = new Texture2D(2, 2);
                Texture2D srcMain2 = new Texture2D(2, 2);
                Texture2D srcMain3 = new Texture2D(2, 2);

                hsvgMaterial.EnableKeyword("_TRIMASK");

                path = AssetDatabase.GetAssetPath(matcapBlendMask.textureValue);
                if(!string.IsNullOrEmpty(path))
                {
                    bytes = File.ReadAllBytes(Path.GetFullPath(path));
                    srcTexture.LoadImage(bytes);
                    srcTexture.filterMode = FilterMode.Bilinear;
                    hsvgMaterial.SetTexture(mainTex.name, srcTexture);
                }
                else
                {
                    hsvgMaterial.SetTexture(mainTex.name, Texture2D.whiteTexture);
                }

                path = AssetDatabase.GetAssetPath(rimColorTex.textureValue);
                if(!string.IsNullOrEmpty(path))
                {
                    bytes = File.ReadAllBytes(Path.GetFullPath(path));
                    srcMain2.LoadImage(bytes);
                    srcMain2.filterMode = FilterMode.Bilinear;
                    hsvgMaterial.SetTexture(main2ndTex.name, srcMain2);
                }
                else
                {
                    hsvgMaterial.SetTexture(main2ndTex.name, Texture2D.whiteTexture);
                }

                path = AssetDatabase.GetAssetPath(emissionBlendMask.textureValue);
                if(!string.IsNullOrEmpty(path))
                {
                    bytes = File.ReadAllBytes(Path.GetFullPath(path));
                    srcMain3.LoadImage(bytes);
                    srcMain3.filterMode = FilterMode.Bilinear;
                    hsvgMaterial.SetTexture(main3rdTex.name, srcMain3);
                }
                else
                {
                    hsvgMaterial.SetTexture(main3rdTex.name, Texture2D.whiteTexture);
                }

                RenderTexture dstTexture;
                if(bufMainTexture != null) dstTexture = new RenderTexture(bufMainTexture.width, bufMainTexture.height, 0, RenderTextureFormat.ARGB32);
                else                       dstTexture = new RenderTexture(4096, 4096, 0, RenderTextureFormat.ARGB32);

                Graphics.Blit(srcTexture, dstTexture, hsvgMaterial);

                Texture2D outTexture;
                if(bufMainTexture != null) outTexture = new Texture2D(bufMainTexture.width, bufMainTexture.height);
                else                       outTexture = new Texture2D(4096, 4096);
                outTexture.ReadPixels(new Rect(0, 0, bufMainTexture.width, bufMainTexture.height), 0, 0);
                outTexture.Apply();

                outTexture = SaveTextureToPng(material, outTexture, mainTex.name);
                if(outTexture != mainTex.textureValue && mainTex.textureValue != null)
                {
                    CopyTextureSetting(bufMainTexture, outTexture);
                }

                UnityEngine.Object.DestroyImmediate(hsvgMaterial);
                UnityEngine.Object.DestroyImmediate(srcTexture);
                UnityEngine.Object.DestroyImmediate(dstTexture);

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
            byte[] bytes;

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
                bytes = File.ReadAllBytes(Path.GetFullPath(path));
                srcTexture.LoadImage(bytes);
                srcTexture.filterMode = FilterMode.Bilinear;
                hsvgMaterial.SetTexture(mainTex.name, srcTexture);
            }
            else
            {
                hsvgMaterial.SetTexture(mainTex.name, Texture2D.whiteTexture);
            }

            path = AssetDatabase.GetAssetPath(material.GetTexture(alphaMask.name));
            if(!string.IsNullOrEmpty(path))
            {
                bytes = File.ReadAllBytes(Path.GetFullPath(path));
                srcAlphaMask.LoadImage(bytes);
                srcAlphaMask.filterMode = FilterMode.Bilinear;
                hsvgMaterial.SetTexture(alphaMask.name, srcAlphaMask);
            }
            else
            {
                return (Texture2D)mainTex.textureValue;
            }

            RenderTexture dstTexture;
            if(srcTexture != null)      dstTexture = new RenderTexture(srcTexture.width, srcTexture.height, 0, RenderTextureFormat.ARGB32);
            else                        dstTexture = new RenderTexture(4096, 4096, 0, RenderTextureFormat.ARGB32);

            Graphics.Blit(srcTexture, dstTexture, hsvgMaterial);

            Texture2D outTexture;
            if(srcTexture != null)      outTexture = new Texture2D(srcTexture.width, srcTexture.height);
            else                        outTexture = new Texture2D(4096, 4096);
            outTexture.ReadPixels(new Rect(0, 0, srcTexture.width, srcTexture.height), 0, 0);
            outTexture.Apply();

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
            UnityEngine.Object.DestroyImmediate(dstTexture);

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
                byte[] bytes;

                Texture2D srcTexture = new Texture2D(2, 2);

                hsvgMaterial.SetColor(mainColor.name,                   Color.white);
                hsvgMaterial.SetVector(mainTexHSVG.name,                outlineTexHSVG.vectorValue);

                path = AssetDatabase.GetAssetPath(material.GetTexture(outlineTex.name));
                if(!string.IsNullOrEmpty(path))
                {
                    bytes = File.ReadAllBytes(Path.GetFullPath(path));
                    srcTexture.LoadImage(bytes);
                    srcTexture.filterMode = FilterMode.Bilinear;
                    hsvgMaterial.SetTexture(mainTex.name, srcTexture);
                }
                else
                {
                    hsvgMaterial.SetTexture(mainTex.name, Texture2D.whiteTexture);
                }

                RenderTexture dstTexture = new RenderTexture(srcTexture.width, srcTexture.height, 0, RenderTextureFormat.ARGB32);

                Graphics.Blit(srcTexture, dstTexture, hsvgMaterial);

                Texture2D outTexture = new Texture2D(srcTexture.width, srcTexture.height);
                outTexture.ReadPixels(new Rect(0, 0, srcTexture.width, srcTexture.height), 0, 0);
                outTexture.Apply();

                outTexture = SaveTextureToPng(material, outTexture, mainTex.name);
                if(outTexture != mainTex.textureValue)
                {
                    CopyTextureSetting(bufMainTexture, outTexture);
                }

                UnityEngine.Object.DestroyImmediate(hsvgMaterial);
                UnityEngine.Object.DestroyImmediate(srcTexture);
                UnityEngine.Object.DestroyImmediate(dstTexture);

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
            byte[] bytes;

            Texture2D srcTexture = new Texture2D(2, 2);

            if(masktex != null) path = AssetDatabase.GetAssetPath(bufMainTexture);
            if(!string.IsNullOrEmpty(path))
            {
                bytes = File.ReadAllBytes(Path.GetFullPath(path));
                srcTexture.LoadImage(bytes);
                srcTexture.filterMode = FilterMode.Bilinear;
                hsvgMaterial.SetTexture(mainTex.name, srcTexture);
            }
            else
            {
                hsvgMaterial.SetTexture(mainTex.name, Texture2D.whiteTexture);
            }

            RenderTexture dstTexture = new RenderTexture(srcTexture.width, srcTexture.height, 0, RenderTextureFormat.ARGB32);

            Graphics.Blit(srcTexture, dstTexture, hsvgMaterial);

            Texture2D outTexture = new Texture2D(srcTexture.width, srcTexture.height);
            outTexture.ReadPixels(new Rect(0, 0, srcTexture.width, srcTexture.height), 0, 0);
            outTexture.Apply();

            if(!string.IsNullOrEmpty(path)) path = Path.GetDirectoryName(path) + "/Baked_" + material.name + "_" + propName;
            else                            path = "Assets/Baked_" + material.name + "_" + propName;
            outTexture = SaveTextureToPng(outTexture, path);
            if(outTexture != bufMainTexture)
            {
                CopyTextureSetting(bufMainTexture, outTexture);
            }

            UnityEngine.Object.DestroyImmediate(hsvgMaterial);
            UnityEngine.Object.DestroyImmediate(srcTexture);
            UnityEngine.Object.DestroyImmediate(dstTexture);
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

            private bool shouldSaveShader = false;
            private bool shouldSaveQueue = false;
            private bool shouldSaveStencil = false;
            private bool shouldSaveMainTex2Outline = false;
            private bool shouldSaveMain = false;
            private bool shouldSaveMain2 = false;
            private bool shouldSaveMain2Mask = false;
            private bool shouldSaveMain3 = false;
            private bool shouldSaveMain3Mask = false;
            private bool shouldSaveShadowBorder = false;
            private bool shouldSaveShadowBlur = false;
            private bool shouldSaveShadowStrength = false;
            private bool shouldSaveShadowColor = false;
            private bool shouldSaveShadowColor2 = false;
            private bool shouldSaveShadowColor3 = false;
            private bool shouldSaveOutlineTex = false;
            private bool shouldSaveOutlineWidth = false;
            private bool shouldSaveEmissionColor = false;
            private bool shouldSaveEmissionMask = false;
            private bool shouldSaveEmissionGrad = false;
            private bool shouldSaveEmission2Color = false;
            private bool shouldSaveEmission2Mask = false;
            private bool shouldSaveEmission2Grad = false;
            private bool shouldSaveNormal = false;
            private bool shouldSaveNormal2 = false;
            private bool shouldSaveNormal2Mask = false;
            private bool shouldSaveReflectionSmoothness = false;
            private bool shouldSaveReflectionMetallic = false;
            private bool shouldSaveReflectionColor = false;
            private bool shouldSaveMatcap = false;
            private bool shouldSaveMatcapMask = false;
            private bool shouldSaveRim = false;
            private bool shouldSaveParallax = false;
            private bool shouldSaveFurNormal = false;
            private bool shouldSaveFurNoise = false;
            private bool shouldSaveFurMask = false;
            private lilToonPreset preset;
            private string[] presetName;
            private string filename = "";

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

                // Name
                EditorGUILayout.LabelField(GetLoc("sPresetName"));
                for(int i = 0; i < langName.Length; i++)
                {
                    presetName[i] = EditorGUILayout.TextField(langName[i], presetName[i]);
                }

                DrawLine();

                preset.category = (lilPresetCategory)EditorGUILayout.Popup(GetLoc("sPresetCategory"), (int)preset.category, sCategorys);

                // Toggle
                EditorGUILayout.LabelField(GetLoc("sPresetSaveTarget"));
                shouldSaveShader                = EditorGUILayout.ToggleLeft(GetLoc("sPresetShader"), shouldSaveShader);
                shouldSaveQueue                 = EditorGUILayout.ToggleLeft(GetLoc("sPresetQueue"), shouldSaveQueue);
                shouldSaveStencil               = EditorGUILayout.ToggleLeft(GetLoc("sPresetStencil"), shouldSaveStencil);
                shouldSaveMainTex2Outline       = EditorGUILayout.ToggleLeft(GetLoc("sPresetMainTex2Outline"), shouldSaveMainTex2Outline);

                DrawLine();

                EditorGUILayout.LabelField(GetLoc("sPresetTexture"));
                shouldSaveMain                  = EditorGUILayout.ToggleLeft(GetLoc("sPresetMain"), shouldSaveMain);
                shouldSaveMain2                 = EditorGUILayout.ToggleLeft(GetLoc("sPresetMain2"), shouldSaveMain2);
                shouldSaveMain2Mask             = EditorGUILayout.ToggleLeft(GetLoc("sPresetMain2Mask"), shouldSaveMain2Mask);
                shouldSaveMain3                 = EditorGUILayout.ToggleLeft(GetLoc("sPresetMain3"), shouldSaveMain3);
                shouldSaveMain3Mask             = EditorGUILayout.ToggleLeft(GetLoc("sPresetMain3Mask"), shouldSaveMain3Mask);

                shouldSaveShadowBorder          = EditorGUILayout.ToggleLeft(GetLoc("sPresetShadowBorder"), shouldSaveShadowBorder);
                shouldSaveShadowBlur            = EditorGUILayout.ToggleLeft(GetLoc("sPresetShadowBlur"), shouldSaveShadowBlur);
                shouldSaveShadowStrength        = EditorGUILayout.ToggleLeft(GetLoc("sPresetShadowStrength"), shouldSaveShadowStrength);
                shouldSaveShadowColor           = EditorGUILayout.ToggleLeft(GetLoc("sPresetShadowColor"), shouldSaveShadowColor);
                shouldSaveShadowColor2          = EditorGUILayout.ToggleLeft(GetLoc("sPresetShadowColor2"), shouldSaveShadowColor2);
                shouldSaveShadowColor3          = EditorGUILayout.ToggleLeft(GetLoc("sPresetShadowColor3"), shouldSaveShadowColor3);

                shouldSaveOutlineTex            = EditorGUILayout.ToggleLeft(GetLoc("sPresetOutlineTex"), shouldSaveOutlineTex);
                shouldSaveOutlineWidth          = EditorGUILayout.ToggleLeft(GetLoc("sPresetOutlineWidth"), shouldSaveOutlineWidth);

                shouldSaveEmissionColor         = EditorGUILayout.ToggleLeft(GetLoc("sPresetEmissionColor"), shouldSaveEmissionColor);
                shouldSaveEmissionMask          = EditorGUILayout.ToggleLeft(GetLoc("sPresetEmissionMask"), shouldSaveEmissionMask);
                shouldSaveEmissionGrad          = EditorGUILayout.ToggleLeft(GetLoc("sPresetEmissionGrad"), shouldSaveEmissionGrad);
                shouldSaveEmission2Color        = EditorGUILayout.ToggleLeft(GetLoc("sPresetEmission2Color"), shouldSaveEmission2Color);
                shouldSaveEmission2Mask         = EditorGUILayout.ToggleLeft(GetLoc("sPresetEmission2Mask"), shouldSaveEmission2Mask);
                shouldSaveEmission2Grad         = EditorGUILayout.ToggleLeft(GetLoc("sPresetEmission2Grad"), shouldSaveEmission2Grad);

                shouldSaveNormal                = EditorGUILayout.ToggleLeft(GetLoc("sPresetNormal"), shouldSaveNormal);
                shouldSaveNormal2               = EditorGUILayout.ToggleLeft(GetLoc("sPresetNormal2"), shouldSaveNormal2);
                shouldSaveNormal2Mask           = EditorGUILayout.ToggleLeft(GetLoc("sPresetNormal2Mask"), shouldSaveNormal2Mask);

                shouldSaveReflectionSmoothness  = EditorGUILayout.ToggleLeft(GetLoc("sPresetReflectionSmoothness"), shouldSaveReflectionSmoothness);
                shouldSaveReflectionMetallic     = EditorGUILayout.ToggleLeft(GetLoc("sPresetReflectionMetallic"), shouldSaveReflectionMetallic);
                shouldSaveReflectionColor       = EditorGUILayout.ToggleLeft(GetLoc("sPresetReflectionColor"), shouldSaveReflectionColor);

                shouldSaveMatcap                = EditorGUILayout.ToggleLeft(GetLoc("sPresetMatcap"), shouldSaveMatcap);
                shouldSaveMatcapMask            = EditorGUILayout.ToggleLeft(GetLoc("sPresetMatcapMask"), shouldSaveMatcapMask);

                shouldSaveRim                   = EditorGUILayout.ToggleLeft(GetLoc("sPresetRim"), shouldSaveRim);

                shouldSaveParallax              = EditorGUILayout.ToggleLeft(GetLoc("sPresetParallax"), shouldSaveParallax);

                shouldSaveFurNormal             = EditorGUILayout.ToggleLeft(GetLoc("sPresetFurNormal"), shouldSaveFurNormal);
                shouldSaveFurNoise              = EditorGUILayout.ToggleLeft(GetLoc("sPresetFurNoise"), shouldSaveFurNoise);
                shouldSaveFurMask               = EditorGUILayout.ToggleLeft(GetLoc("sPresetFurMask"), shouldSaveFurMask);

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
                    int propCount = ShaderUtil.GetPropertyCount(material.shader);
                    for(int i = 0; i < propCount; i++)
                    {
                        string propName = ShaderUtil.GetPropertyName(material.shader, i);
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
                    if(shouldSaveShader) preset.shader = material.shader;
                    else preset.shader = null;

                    if(shouldSaveQueue) preset.renderQueue = material.renderQueue;
                    else preset.renderQueue = -2;

                    preset.outline = material.shader.name.Contains("Outline");
                    preset.tessellation = material.shader.name.Contains("Tessellation");
                    preset.outlineMainTex = shouldSaveMainTex2Outline;

                    if(shouldSaveMain) lilPresetCopyTexture(preset, material, "_MainTex");
                    if(shouldSaveMain2) lilPresetCopyTexture(preset, material, "_Main2ndTex");
                    if(shouldSaveMain2Mask) lilPresetCopyTexture(preset, material, "_Main2ndBlendMask");
                    if(shouldSaveMain3) lilPresetCopyTexture(preset, material, "_Main3rdTex");
                    if(shouldSaveMain3Mask) lilPresetCopyTexture(preset, material, "_Main3rdBlendMask");

                    if(shouldSaveShadowBorder) lilPresetCopyTexture(preset, material, "_ShadowBorderMask");
                    if(shouldSaveShadowBlur) lilPresetCopyTexture(preset, material, "_ShadowBlurMask");
                    if(shouldSaveShadowStrength) lilPresetCopyTexture(preset, material, "_ShadowStrengthMask");
                    if(shouldSaveShadowColor) lilPresetCopyTexture(preset, material, "_ShadowColorTex");
                    if(shouldSaveShadowColor2) lilPresetCopyTexture(preset, material, "_Shadow2ndColorTex");
                    if(shouldSaveShadowColor3) lilPresetCopyTexture(preset, material, "_Shadow3rdColorTex");

                    if(shouldSaveEmissionColor) lilPresetCopyTexture(preset, material, "_EmissionMap");
                    if(shouldSaveEmissionMask) lilPresetCopyTexture(preset, material, "_EmissionBlendMask");
                    if(shouldSaveEmissionGrad) lilPresetCopyTexture(preset, material, "_EmissionGradTex");
                    if(shouldSaveEmission2Color) lilPresetCopyTexture(preset, material, "_Emission2ndMap");
                    if(shouldSaveEmission2Mask) lilPresetCopyTexture(preset, material, "_Emission2ndBlendMask");
                    if(shouldSaveEmission2Grad) lilPresetCopyTexture(preset, material, "_Emission2ndGradTex");

                    if(shouldSaveNormal) lilPresetCopyTexture(preset, material, "_BumpMap");
                    if(shouldSaveNormal2) lilPresetCopyTexture(preset, material, "_Bump2ndMap");
                    if(shouldSaveNormal2Mask) lilPresetCopyTexture(preset, material, "_Bump2ndScaleMask");

                    if(shouldSaveReflectionSmoothness) lilPresetCopyTexture(preset, material, "_SmoothnessTex");
                    if(shouldSaveReflectionMetallic) lilPresetCopyTexture(preset, material, "_MetallicGlossMap");
                    if(shouldSaveReflectionColor) lilPresetCopyTexture(preset, material, "_ReflectionColorTex");

                    if(shouldSaveMatcap) lilPresetCopyTexture(preset, material, "_MatCapTex");
                    if(shouldSaveMatcapMask) lilPresetCopyTexture(preset, material, "_MatCapBlendMask");

                    if(shouldSaveRim) lilPresetCopyTexture(preset, material, "_RimColorTex");

                    if(shouldSaveParallax) lilPresetCopyTexture(preset, material, "_ParallaxMap");

                    if(shouldSaveOutlineTex) lilPresetCopyTexture(preset, material, "_OutlineTex");
                    if(shouldSaveOutlineWidth) lilPresetCopyTexture(preset, material, "_OutlineWidthMask");

                    if(shouldSaveFurNormal) lilPresetCopyTexture(preset, material, "_FurVectorTex");
                    if(shouldSaveFurNoise) lilPresetCopyTexture(preset, material, "_FurNoiseMask");
                    if(shouldSaveFurMask) lilPresetCopyTexture(preset, material, "_FurMask");

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

            private void lilPresetCopyTexture(lilToonPreset preset, Material material, string propName)
            {
                Array.Resize(ref preset.textures, preset.textures.Length + 1);
                preset.textures[preset.textures.Length-1].name = propName;
                preset.textures[preset.textures.Length-1].value = material.GetTexture(propName);
                preset.textures[preset.textures.Length-1].offset = material.GetTextureOffset(propName);
                preset.textures[preset.textures.Length-1].scale = material.GetTextureScale(propName);
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
                if(tex.textureValue != null && AssetDatabase.GetAssetPath(tex.textureValue).EndsWith(".gif", StringComparison.OrdinalIgnoreCase) && GUILayout.Button(GetLoc("sConvertGif")))
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
}
#endif