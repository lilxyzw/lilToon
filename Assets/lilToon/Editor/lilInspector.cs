#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#if !UNITY_2018_1_OR_NEWER
    using System.Reflection;
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
        static bool isCustomShader = false;

        //MaterialProperty exampleProperty;

        void LoadCustomProperties(MaterialProperty[] props, Material material)
        {
            //if(material.shader.name.Contains("Example"))
            //{
            //    isCustomShader = true;
            //    exampleProperty = FindProperty("_ExampleProperty", props);
            //}
        }

        void DrawCustomProperties(MaterialEditor materialEditor, Material material)
        {
            //if(material.shader.name.Contains("Example"))
            //{
            //    materialEditor.ShaderProperty(exampleProperty, "Example Property");
            //}
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Enum
        public enum EditorMode
        {
            Simple,
            Advanced,
            Preset
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
            Gem
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
            URP
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Constant
        public const string currentVersionName = "1.1.8";
        public const int currentVersionValue = 12;

        public const string boothURL = "https://lilxyzw.booth.pm/";
        public const string githubURL = "https://github.com/lilxyzw/lilToon";
        public const string versionInfoURL = "https://raw.githubusercontent.com/lilxyzw/lilToon/master/version.json";
        public const string mainFolderGUID          = "05d1d116436047941ad97d1b9064ee05"; // "Assets/lilToon"
        public const string editorFolderGUID        = "3e73d675b9c1adc4f8b6b8ef01bce51c"; // "Assets/lilToon/Editor"
        public const string presetsFolderGUID       = "35817d21af2f3134182c4a7e4c07786b"; // "Assets/lilToon/Presets"
        public const string editorGUID              = "aefa51cbc37d602418a38a02c3b9afb9"; // "Assets/lilToon/Editor/lilInspector.cs"
        public const string editorSettingGUID       = "283173792c6bb2342afdef844a3c1d56"; // "Assets/lilToon/Editor/lilToonEditorSetting.cs"
        public const string shaderFolderGUID        = "ac0a8f602b5e72f458f4914bf08f0269"; // "Assets/lilToon/Shader"
        public const string shaderPipelineGUID      = "32299664512e2e042bbc351c1d46d383"; // "Assets/lilToon/Shader/Includes/lil_pipeline.hlsl";
        public const string shaderCommonGUID        = "5520e766422958546bbe885a95d5a67e"; // "Assets/lilToon/Shader/Includes/lil_common.hlsl";
        public const string avatarEncryptionGUID    = "f9787bf8ed5154f4b931278945ac8ca1"; // "Assets/AvaterEncryption";
        public const string editorSettingTempPath   = "Temp/lilToonEditorSetting";
        public const string versionInfoTempPath     = "Temp/lilToonVersion";
        public const string packageListTempPath     = "Temp/lilToonPackageList";
        public static readonly string[] mainTexCheckWords = new[] {"mask", "shadow", "shade", "outline", "normal", "bumpmap", "matcap", "rimlight", "emittion", "reflection", "specular", "roughness", "smoothness", "metallic", "metalness", "opacity", "parallax", "displacement", "height", "ambient", "occlusion"};

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
        public static string GetEditorSettingPath()
        {
            return AssetDatabase.GUIDToAssetPath(editorSettingGUID);
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
        public static string GetAvatarEncryptionPath()
        {
            return AssetDatabase.GUIDToAssetPath(avatarEncryptionGUID);
        }
        #if NET_4_6
            public const string rspPath = "Assets/csc.rsp";
        #else
            public const string rspPath = "Assets/mcs.rsp";
        #endif

        public static readonly Vector2 defaultTextureOffset = new Vector2(0.0f,0.0f);
        public static readonly Vector2 defaultTextureScale = new Vector2(1.0f,1.0f);
        public static readonly Vector4 defaultScrollRotate = new Vector4(0.0f,0.0f,0.0f,0.0f);
        public static readonly Vector4 defaultHSVG = new Vector4(0.0f,1.0f,1.0f,1.0f);
        public static readonly Vector4 defaultKeys = new Vector4(0.0f,0.0f,0.0f,0.0f);
        public static readonly Vector4 defaultDecalAnim = new Vector4(1.0f,1.0f,1.0f,30.0f);
        public static readonly Vector4 defaultDissolveParams = new Vector4(0.0f,0.0f,0.5f,0.1f);
        public static readonly Vector4 defaultDistanceFadeParams = new Vector4(0.1f,0.01f,0.0f,0.0f);
        public static readonly Color lineColor = EditorGUIUtility.isProSkin ? new Color(0.35f,0.35f,0.35f,1.0f) : new Color(0.4f,0.4f,0.4f,1.0f);

        //------------------------------------------------------------------------------------------------------------------------------
        // Shader
        static Shader lts       = Shader.Find("lilToon");
        static Shader ltsc      = Shader.Find("Hidden/lilToonCutout");
        static Shader ltst      = Shader.Find("Hidden/lilToonTransparent");

        static Shader ltso      = Shader.Find("Hidden/lilToonOutline");
        static Shader ltsco     = Shader.Find("Hidden/lilToonCutoutOutline");
        static Shader ltsto     = Shader.Find("Hidden/lilToonTransparentOutline");

        static Shader ltstess   = Shader.Find("Hidden/lilToonTessellation");
        static Shader ltstessc  = Shader.Find("Hidden/lilToonTessellationCutout");
        static Shader ltstesst  = Shader.Find("Hidden/lilToonTessellationTransparent");

        static Shader ltstesso  = Shader.Find("Hidden/lilToonTessellationOutline");
        static Shader ltstessco = Shader.Find("Hidden/lilToonTessellationCutoutOutline");
        static Shader ltstessto = Shader.Find("Hidden/lilToonTessellationTransparentOutline");

        static Shader ltsl      = Shader.Find("Hidden/lilToonLite");
        static Shader ltslc     = Shader.Find("Hidden/lilToonLiteCutout");
        static Shader ltslt     = Shader.Find("Hidden/lilToonLiteTransparent");

        static Shader ltslo     = Shader.Find("Hidden/lilToonLiteOutline");
        static Shader ltslco    = Shader.Find("Hidden/lilToonLiteCutoutOutline");
        static Shader ltslto    = Shader.Find("Hidden/lilToonLiteTransparentOutline");

        static Shader ltsref    = Shader.Find("Hidden/lilToonRefraction");
        static Shader ltsrefb   = Shader.Find("Hidden/lilToonRefractionBlur");
        static Shader ltsfur    = Shader.Find("Hidden/lilToonFur");
        static Shader ltsfurc   = Shader.Find("Hidden/lilToonFurCutout");

        static Shader ltsgem    = Shader.Find("Hidden/lilToonGem");

        static Shader ltsfs     = Shader.Find("_lil/lilToonFakeShadow");

        static Shader ltsbaker  = Shader.Find("Hidden/ltsother_baker");
        static Shader ltspo     = Shader.Find("Hidden/ltspass_opaque");
        static Shader ltspc     = Shader.Find("Hidden/ltspass_cutout");
        static Shader ltspt     = Shader.Find("Hidden/ltspass_transparent");
        static Shader ltsptesso = Shader.Find("Hidden/ltspass_tess_opaque");
        static Shader ltsptessc = Shader.Find("Hidden/ltspass_tess_cutout");
        static Shader ltsptesst = Shader.Find("Hidden/ltspass_tess_transparent");

        static Shader mtoon     = Shader.Find("VRM/MToon");

        //------------------------------------------------------------------------------------------------------------------------------
        // Editor
        public static bool isUPM = false;
        static lilToonSetting shaderSetting;
        static lilToonPreset[] presets;
        public static Dictionary<string, string> loc = new Dictionary<string, string>();

        [Serializable]
        public class lilToonEditorSetting
        {
            public EditorMode editorMode = EditorMode.Simple;
            public int languageNum = -1;
            public string languageNames = "";
            public string languageName = "English";
            public bool isShowMainUV            = false;
            public bool isShowMain              = false;
            public bool isShowShadow            = false;
            public bool isShowBump              = false;
            public bool isShowReflections       = false;
            public bool isShowEmission          = false;
            public bool isShowEmissionMap           = false;
            public bool isShowEmissionBlendMask     = false;
            public bool isShowEmission2ndMap        = false;
            public bool isShowEmission2ndBlendMask  = false;
            public bool isShowParallax          = false;
            public bool isShowDistanceFade      = false;
            public bool isShowAudioLink         = false;
            public bool isShowDissolve          = false;
            public bool isShowMain2ndDissolveMask  = false;
            public bool isShowMain2ndDissolveNoiseMask  = false;
            public bool isShowMain3rdDissolveMask  = false;
            public bool isShowMain3rdDissolveNoiseMask  = false;
            public bool isShowDissolveMask         = false;
            public bool isShowDissolveNoiseMask         = false;
            public bool isShowEncryption        = false;
            public bool isShowStencil           = false;
            public bool isShowOutline           = false;
            public bool isShowOutlineMap            = false;
            public bool isShowRefraction        = false;
            public bool isShowFur               = false;
            public bool isShowTess              = false;
            public bool isShowRendering         = false;
            public bool isShowOptimization      = false;
            public bool isShowCustomProperties  = false;
            public bool isShowBlend             = false;
            public bool isShowBlendAdd          = false;
            public bool isShowBlendOutline      = false;
            public bool isShowBlendAddOutline   = false;
            public bool isShowBlendFur          = false;
            public bool isShowBlendAddFur       = false;
            public bool isShowWebPages          = false;
            public bool isShowHelpPages         = false;
            public bool isShowShaderSetting     = false;
            public bool isShaderSettingChanged  = false;
            public bool[] isShowCategorys = new bool[(int)lilPresetCategory.Other+1]{false,false,false,false,false,false,false};
        }

        public static lilToonEditorSetting edSet = new lilToonEditorSetting();

        [Serializable]
        public class lilToonVersion
        {
            public string latest_vertion_name;
            public int latest_vertion_value;
        }
        static lilToonVersion latestVersion = new lilToonVersion
        {
            latest_vertion_name = "",
            latest_vertion_value = 0
        };

        //------------------------------------------------------------------------------------------------------------------------------
        // Material
        static RenderingMode renderingModeBuf;
        static bool isLite         = false;
        static bool isCutout       = false;
        static bool isTransparent  = false;
        static bool isOutl         = false;
        static bool isRefr         = false;
        static bool isBlur         = false;
        static bool isFur          = false;
        static bool isStWr         = false;
        static bool isTess         = false;
        static bool isGem          = false;
        static bool isFakeShadow   = false;

        //------------------------------------------------------------------------------------------------------------------------------
        // Material properties
        MaterialProperty invisible;
        MaterialProperty asUnlit;
        MaterialProperty cutoff;
        MaterialProperty flipNormal;
        MaterialProperty backfaceForceShadow;
        MaterialProperty vertexLightStrength;
        MaterialProperty lightMinLimit;
        MaterialProperty triMask;
            MaterialProperty cull;
            MaterialProperty srcBlend;
            MaterialProperty dstBlend;
            MaterialProperty srcBlendAlpha;
            MaterialProperty dstBlendAlpha;
            MaterialProperty blendOp;
            MaterialProperty blendOpAlpha;
            MaterialProperty srcBlendFA;
            MaterialProperty dstBlendFA;
            MaterialProperty srcBlendAlphaFA;
            MaterialProperty dstBlendAlphaFA;
            MaterialProperty blendOpFA;
            MaterialProperty blendOpAlphaFA;
            MaterialProperty zwrite;
            MaterialProperty ztest;
            MaterialProperty stencilRef;
            MaterialProperty stencilReadMask;
            MaterialProperty stencilWriteMask;
            MaterialProperty stencilComp;
            MaterialProperty stencilPass;
            MaterialProperty stencilFail;
            MaterialProperty stencilZFail;
            MaterialProperty offsetFactor;
            MaterialProperty offsetUnits;
            MaterialProperty colorMask;
        //MaterialProperty useMainTex;
            MaterialProperty mainColor;
            MaterialProperty mainTex;
            MaterialProperty mainTexHSVG;
            MaterialProperty mainTex_ScrollRotate;
            MaterialProperty mainGradationStrength;
            MaterialProperty mainGradationTex;
            MaterialProperty mainColorAdjustMask;
        MaterialProperty useMain2ndTex;
            MaterialProperty mainColor2nd;
            MaterialProperty main2ndTex;
            MaterialProperty main2ndTexAngle;
            MaterialProperty main2ndTexDecalAnimation;
            MaterialProperty main2ndTexDecalSubParam;
            MaterialProperty main2ndTexIsDecal;
            MaterialProperty main2ndTexIsLeftOnly;
            MaterialProperty main2ndTexIsRightOnly;
            MaterialProperty main2ndTexShouldCopy;
            MaterialProperty main2ndTexShouldFlipMirror;
            MaterialProperty main2ndTexShouldFlipCopy;
            MaterialProperty main2ndTexIsMSDF;
            MaterialProperty main2ndBlendMask;
            MaterialProperty main2ndTexBlendMode;
            MaterialProperty main2ndEnableLighting;
            MaterialProperty main2ndDissolveMask;
            MaterialProperty main2ndDissolveNoiseMask;
            MaterialProperty main2ndDissolveNoiseMask_ScrollRotate;
            MaterialProperty main2ndDissolveNoiseStrength;
            MaterialProperty main2ndDissolveColor;
            MaterialProperty main2ndDissolveParams;
            MaterialProperty main2ndDissolvePos;
            MaterialProperty main2ndDistanceFade;
        MaterialProperty useMain3rdTex;
            MaterialProperty mainColor3rd;
            MaterialProperty main3rdTex;
            MaterialProperty main3rdTexAngle;
            MaterialProperty main3rdTexDecalAnimation;
            MaterialProperty main3rdTexDecalSubParam;
            MaterialProperty main3rdTexIsDecal;
            MaterialProperty main3rdTexIsLeftOnly;
            MaterialProperty main3rdTexIsRightOnly;
            MaterialProperty main3rdTexShouldCopy;
            MaterialProperty main3rdTexShouldFlipMirror;
            MaterialProperty main3rdTexShouldFlipCopy;
            MaterialProperty main3rdTexIsMSDF;
            MaterialProperty main3rdBlendMask;
            MaterialProperty main3rdTexBlendMode;
            MaterialProperty main3rdEnableLighting;
            MaterialProperty main3rdDissolveMask;
            MaterialProperty main3rdDissolveNoiseMask;
            MaterialProperty main3rdDissolveNoiseMask_ScrollRotate;
            MaterialProperty main3rdDissolveNoiseStrength;
            MaterialProperty main3rdDissolveColor;
            MaterialProperty main3rdDissolveParams;
            MaterialProperty main3rdDissolvePos;
            MaterialProperty main3rdDistanceFade;
        MaterialProperty alphaMaskMode;
            MaterialProperty alphaMask;
            MaterialProperty alphaMaskValue;
        MaterialProperty useShadow;
            MaterialProperty shadowBorder;
            MaterialProperty shadowBorderMask;
            MaterialProperty shadowBlur;
            MaterialProperty shadowBlurMask;
            MaterialProperty shadowStrength;
            MaterialProperty shadowStrengthMask;
            MaterialProperty shadowColor;
            MaterialProperty shadowColorTex;
            MaterialProperty shadow2ndBorder;
            MaterialProperty shadow2ndBlur;
            MaterialProperty shadow2ndColor;
            MaterialProperty shadow2ndColorTex;
            MaterialProperty shadowMainStrength;
            MaterialProperty shadowEnvStrength;
            MaterialProperty shadowBorderColor;
            MaterialProperty shadowBorderRange;
            MaterialProperty shadowReceive;
        MaterialProperty useBumpMap;
            MaterialProperty bumpMap;
            MaterialProperty bumpScale;
        MaterialProperty useBump2ndMap;
            MaterialProperty bump2ndMap;
            MaterialProperty bump2ndScale;
            MaterialProperty bump2ndScaleMask;
        MaterialProperty useReflection;
            MaterialProperty metallic;
            MaterialProperty metallicGlossMap;
            MaterialProperty smoothness;
            MaterialProperty smoothnessTex;
            MaterialProperty reflectance;
            MaterialProperty reflectionColor;
            MaterialProperty reflectionColorTex;
            MaterialProperty applySpecular;
            MaterialProperty specularToon;
            MaterialProperty applyReflection;
            MaterialProperty reflectionApplyTransparency;
        MaterialProperty useMatCap;
            MaterialProperty matcapTex;
            MaterialProperty matcapColor;
            MaterialProperty matcapBlend;
            MaterialProperty matcapBlendMask;
            MaterialProperty matcapEnableLighting;
            MaterialProperty matcapBlendMode;
            MaterialProperty matcapMul;
            MaterialProperty matcapApplyTransparency;
            MaterialProperty matcapZRotCancel;
            MaterialProperty matcapCustomNormal;
            MaterialProperty matcapBumpMap;
            MaterialProperty matcapBumpScale;
        MaterialProperty useMatCap2nd;
            MaterialProperty matcap2ndTex;
            MaterialProperty matcap2ndColor;
            MaterialProperty matcap2ndBlend;
            MaterialProperty matcap2ndBlendMask;
            MaterialProperty matcap2ndEnableLighting;
            MaterialProperty matcap2ndBlendMode;
            MaterialProperty matcap2ndMul;
            MaterialProperty matcap2ndApplyTransparency;
            MaterialProperty matcap2ndZRotCancel;
            MaterialProperty matcap2ndCustomNormal;
            MaterialProperty matcap2ndBumpMap;
            MaterialProperty matcap2ndBumpScale;
        MaterialProperty useRim;
            MaterialProperty rimColor;
            MaterialProperty rimColorTex;
            MaterialProperty rimBorder;
            MaterialProperty rimBlur;
            MaterialProperty rimFresnelPower;
            MaterialProperty rimEnableLighting;
            MaterialProperty rimShadowMask;
            MaterialProperty rimApplyTransparency;
            MaterialProperty rimDirStrength;
            MaterialProperty rimDirRange;
            MaterialProperty rimIndirRange;
            MaterialProperty rimIndirColor;
            MaterialProperty rimIndirBorder;
            MaterialProperty rimIndirBlur;
        MaterialProperty useGlitter;
            MaterialProperty glitterUVMode;
            MaterialProperty glitterColor;
            MaterialProperty glitterColorTex;
            MaterialProperty glitterMainStrength;
            MaterialProperty glitterParams1;
            MaterialProperty glitterParams2;
            MaterialProperty glitterEnableLighting;
            MaterialProperty glitterShadowMask;
            MaterialProperty glitterApplyTransparency;
            MaterialProperty glitterVRParallaxStrength;
        MaterialProperty useEmission;
            MaterialProperty emissionColor;
            MaterialProperty emissionMap;
            MaterialProperty emissionMap_ScrollRotate;
            MaterialProperty emissionBlend;
            MaterialProperty emissionBlendMask;
            MaterialProperty emissionBlendMask_ScrollRotate;
            MaterialProperty emissionBlink;
            MaterialProperty emissionUseGrad;
            MaterialProperty emissionGradTex;
            MaterialProperty emissionGradSpeed;
            MaterialProperty emissionParallaxDepth;
            MaterialProperty emissionFluorescence;
        MaterialProperty useEmission2nd;
            MaterialProperty emission2ndColor;
            MaterialProperty emission2ndMap;
            MaterialProperty emission2ndMap_ScrollRotate;
            MaterialProperty emission2ndBlend;
            MaterialProperty emission2ndBlendMask;
            MaterialProperty emission2ndBlendMask_ScrollRotate;
            MaterialProperty emission2ndBlink;
            MaterialProperty emission2ndUseGrad;
            MaterialProperty emission2ndGradTex;
            MaterialProperty emission2ndGradSpeed;
            MaterialProperty emission2ndParallaxDepth;
            MaterialProperty emission2ndFluorescence;
        //MaterialProperty useOutline;
            MaterialProperty outlineColor;
            MaterialProperty outlineTex;
            MaterialProperty outlineTex_ScrollRotate;
            MaterialProperty outlineTexHSVG;
            MaterialProperty outlineWidth;
            MaterialProperty outlineWidthMask;
            MaterialProperty outlineFixWidth;
            MaterialProperty outlineVertexR2Width;
            MaterialProperty outlineEnableLighting;
            MaterialProperty outlineCull;
            MaterialProperty outlineSrcBlend;
            MaterialProperty outlineDstBlend;
            MaterialProperty outlineSrcBlendAlpha;
            MaterialProperty outlineDstBlendAlpha;
            MaterialProperty outlineBlendOp;
            MaterialProperty outlineBlendOpAlpha;
            MaterialProperty outlineSrcBlendFA;
            MaterialProperty outlineDstBlendFA;
            MaterialProperty outlineSrcBlendAlphaFA;
            MaterialProperty outlineDstBlendAlphaFA;
            MaterialProperty outlineBlendOpFA;
            MaterialProperty outlineBlendOpAlphaFA;
            MaterialProperty outlineZwrite;
            MaterialProperty outlineZtest;
            MaterialProperty outlineStencilRef;
            MaterialProperty outlineStencilReadMask;
            MaterialProperty outlineStencilWriteMask;
            MaterialProperty outlineStencilComp;
            MaterialProperty outlineStencilPass;
            MaterialProperty outlineStencilFail;
            MaterialProperty outlineStencilZFail;
            MaterialProperty outlineOffsetFactor;
            MaterialProperty outlineOffsetUnits;
            MaterialProperty outlineColorMask;
        MaterialProperty useParallax;
            MaterialProperty parallaxMap;
            MaterialProperty parallax;
            MaterialProperty parallaxOffset;
        //MaterialProperty useDistanceFade;
            MaterialProperty distanceFadeColor;
            MaterialProperty distanceFade;
        MaterialProperty useAudioLink;
            MaterialProperty audioLinkUVMode;
            MaterialProperty audioLinkUVParams;
            MaterialProperty audioLinkMask;
            MaterialProperty audioLink2Main2nd;
            MaterialProperty audioLink2Main3rd;
            MaterialProperty audioLink2Emission;
            MaterialProperty audioLink2Emission2nd;
            MaterialProperty audioLink2Vertex;
            MaterialProperty audioLinkVertexUVMode;
            MaterialProperty audioLinkVertexUVParams;
            MaterialProperty audioLinkVertexStart;
            MaterialProperty audioLinkVertexStrength;
            MaterialProperty audioLinkAsLocal;
            MaterialProperty audioLinkLocalMap;
            MaterialProperty audioLinkLocalMapParams;
        //MaterialProperty useDissolve;
            MaterialProperty dissolveMask;
            MaterialProperty dissolveNoiseMask;
            MaterialProperty dissolveNoiseMask_ScrollRotate;
            MaterialProperty dissolveNoiseStrength;
            MaterialProperty dissolveColor;
            MaterialProperty dissolveParams;
            MaterialProperty dissolvePos;
        //MaterialProperty useEncryption
            MaterialProperty ignoreEncryption;
            MaterialProperty keys;
        //MaterialProperty useRefraction;
            MaterialProperty refractionStrength;
            MaterialProperty refractionFresnelPower;
            MaterialProperty refractionColorFromMain;
            MaterialProperty refractionColor;
        //MaterialProperty useFur;
            MaterialProperty furNoiseMask;
            MaterialProperty furMask;
            MaterialProperty furLengthMask;
            MaterialProperty furVectorTex;
            MaterialProperty furVectorScale;
            MaterialProperty furVector;
            MaterialProperty furGravity;
            MaterialProperty furAO;
            MaterialProperty vertexColor2FurVector;
            MaterialProperty furLayerNum;
            MaterialProperty furCull;
            MaterialProperty furSrcBlend;
            MaterialProperty furDstBlend;
            MaterialProperty furSrcBlendAlpha;
            MaterialProperty furDstBlendAlpha;
            MaterialProperty furBlendOp;
            MaterialProperty furBlendOpAlpha;
            MaterialProperty furSrcBlendFA;
            MaterialProperty furDstBlendFA;
            MaterialProperty furSrcBlendAlphaFA;
            MaterialProperty furDstBlendAlphaFA;
            MaterialProperty furBlendOpFA;
            MaterialProperty furBlendOpAlphaFA;
            MaterialProperty furZwrite;
            MaterialProperty furZtest;
            MaterialProperty furStencilRef;
            MaterialProperty furStencilReadMask;
            MaterialProperty furStencilWriteMask;
            MaterialProperty furStencilComp;
            MaterialProperty furStencilPass;
            MaterialProperty furStencilFail;
            MaterialProperty furStencilZFail;
            MaterialProperty furOffsetFactor;
            MaterialProperty furOffsetUnits;
            MaterialProperty furColorMask;
        //MaterialProperty useTessellation;
            MaterialProperty tessEdge;
            MaterialProperty tessStrength;
            MaterialProperty tessShrink;
            MaterialProperty tessFactorMax;
        //MaterialProperty useGem;
            MaterialProperty gemChromaticAberration;
            MaterialProperty gemEnvContrast;
            MaterialProperty gemEnvColor;
            MaterialProperty gemParticleLoop;
            MaterialProperty gemParticleColor;
            MaterialProperty gemVRParallaxStrength;
        //MaterialProperty useFakeShadow;
            MaterialProperty fakeShadowVector;

        //------------------------------------------------------------------------------------------------------------------------------
        // Gradient
        Gradient mainGrad = new Gradient();
        Gradient emiGrad = new Gradient();
        Gradient emi2Grad = new Gradient();

        //------------------------------------------------------------------------------------------------------------------------------
        // GUI
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
	    {
            //------------------------------------------------------------------------------------------------------------------------------
            // EditorAssets
            GUIStyle boxOuter        = GUI.skin.box;
            GUIStyle boxInnerHalf    = GUI.skin.box;
            GUIStyle boxInner        = GUI.skin.box;
            GUIStyle customBox       = GUI.skin.box;
            GUIStyle customToggleFont = EditorStyles.label;
            GUIStyle offsetButton = new GUIStyle(GUI.skin.button);
            string editorFolderPath = GetEditorFolderPath();
            if(EditorGUIUtility.isProSkin)
            {
                boxOuter        = new GUIStyle(((GUISkin)AssetDatabase.LoadAssetAtPath(editorFolderPath + "/gui_box_outer_2019.guiskin", typeof(GUISkin))).box);
                boxInnerHalf    = new GUIStyle(((GUISkin)AssetDatabase.LoadAssetAtPath(editorFolderPath + "/gui_box_inner_half_2019.guiskin", typeof(GUISkin))).box);
                boxInner        = new GUIStyle(((GUISkin)AssetDatabase.LoadAssetAtPath(editorFolderPath + "/gui_box_inner_2019.guiskin", typeof(GUISkin))).box);
                customBox       = new GUIStyle(((GUISkin)AssetDatabase.LoadAssetAtPath(editorFolderPath + "/gui_custom_box_2019.guiskin", typeof(GUISkin))).box);
                customToggleFont = EditorStyles.label;
                offsetButton.margin.left = 24;
            }
            else
            {
                boxOuter        = new GUIStyle(((GUISkin)AssetDatabase.LoadAssetAtPath(editorFolderPath + "/gui_box_outer_2018.guiskin", typeof(GUISkin))).box);
                boxInnerHalf    = new GUIStyle(((GUISkin)AssetDatabase.LoadAssetAtPath(editorFolderPath + "/gui_box_inner_half_2018.guiskin", typeof(GUISkin))).box);
                boxInner        = new GUIStyle(((GUISkin)AssetDatabase.LoadAssetAtPath(editorFolderPath + "/gui_box_inner_2018.guiskin", typeof(GUISkin))).box);
                customBox       = GUI.skin.box;
                customToggleFont = new GUIStyle();
                customToggleFont.normal.textColor = Color.white;
                customToggleFont.contentOffset = new Vector2(2f,0f);
                offsetButton.margin.left = 20;
            }
            GUIStyle wrapLabel = new GUIStyle(GUI.skin.label);
            wrapLabel.wordWrap = true;

            //------------------------------------------------------------------------------------------------------------------------------
            // Initialize Setting
            ApplyEditorSettingTemp();
            InitializeShaders();
            InitializeShaderSetting(ref shaderSetting);

            //------------------------------------------------------------------------------------------------------------------------------
            // Load Material
            Material material = (Material)materialEditor.target;

            //------------------------------------------------------------------------------------------------------------------------------
            // Check Shader Type
            isLite         = material.shader.name.Contains("Lite");
            isCutout       = material.shader.name.Contains("Cutout");
            isTransparent  = material.shader.name.Contains("Transparent") || material.shader.name.Contains("Overlay");
            isOutl         = material.shader.name.Contains("Outline");
            isRefr         = material.shader.name.Contains("Refraction");
            isBlur         = material.shader.name.Contains("Blur");
            isFur          = material.shader.name.Contains("Fur");
            isTess         = material.shader.name.Contains("Tessellation");
            isGem          = material.shader.name.Contains("Gem");
            isFakeShadow   = material.shader.name.Contains("FakeShadow");

            isCustomShader = material.shader.name.Contains("Overlay");
            isCustomShader = material.shader.name.Contains("FakeShadow");

                                renderingModeBuf = RenderingMode.Opaque;
            if(isCutout)        renderingModeBuf = RenderingMode.Cutout;
            if(isTransparent)   renderingModeBuf = RenderingMode.Transparent;
            if(isRefr)          renderingModeBuf = RenderingMode.Refraction;
            if(isRefr&isBlur)   renderingModeBuf = RenderingMode.RefractionBlur;
            if(isFur)           renderingModeBuf = RenderingMode.Fur;
            if(isFur&isCutout)  renderingModeBuf = RenderingMode.FurCutout;
            if(isGem)           renderingModeBuf = RenderingMode.Gem;

            //------------------------------------------------------------------------------------------------------------------------------
            // Load Properties
            if(isLite)      LoadLiteProperties(props);
            else if(isFur)  LoadFurProperties(props);
            else if(isGem)  LoadGemProperties(props);
            else if(isFakeShadow) LoadFakeShadowProperties(props);
            else            LoadProperties(props);

            LoadCustomProperties(props, material);

            //------------------------------------------------------------------------------------------------------------------------------
            // Stencil
            isStWr         = (stencilPass.floatValue == (float)UnityEngine.Rendering.StencilOp.Replace);

            //------------------------------------------------------------------------------------------------------------------------------
            // Remove Shader Keywords
            RemoveShaderKeywords(material);

            //------------------------------------------------------------------------------------------------------------------------------
            // Info
            DrawWebPages();

            //------------------------------------------------------------------------------------------------------------------------------
            // Language
            edSet.languageNum = selectLang(edSet.languageNum);
            string sCullModes = GetLoc("sCullMode") + "|" + GetLoc("sCullModeOff") + "|" + GetLoc("sCullModeFront") + "|" + GetLoc("sCullModeBack");
            string sBlendModes = GetLoc("sBlendMode") + "|" + GetLoc("sBlendModeNormal") + "|" + GetLoc("sBlendModeAdd") + "|" + GetLoc("sBlendModeScreen") + "|" + GetLoc("sBlendModeMul");
            string sAlphaMaskModes = GetLoc("sAlphaMask") + "|" + GetLoc("sAlphaMaskModeNone") + "|" + GetLoc("sAlphaMaskModeReplace") + "|" + GetLoc("sAlphaMaskModeMul");
            string blinkSetting = GetLoc("sBlinkStrength") + "|" + GetLoc("sBlinkType") + "|" + GetLoc("sBlinkSpeed") + "|" + GetLoc("sBlinkOffset");
            string sDistanceFadeSetting = GetLoc("sStartDistance") + "|" + GetLoc("sEndDistance") + "|" + GetLoc("sStrength");
            string sDissolveParams = GetLoc("sDissolveMode") + "|" + GetLoc("sDissolveModeNone") + "|" + GetLoc("sDissolveModeAlpha") + "|" + GetLoc("sDissolveModeUV") + "|" + GetLoc("sDissolveModePosition") + "|" + GetLoc("sDissolveShape") + "|" + GetLoc("sDissolveShapePoint") + "|" + GetLoc("sDissolveShapeLine") + "|" + GetLoc("sBorder") + "|" + GetLoc("sBlur");
            string sDissolveParamsMode = GetLoc("sDissolve") + "|" + GetLoc("sDissolveModeNone") + "|" + GetLoc("sDissolveModeAlpha") + "|" + GetLoc("sDissolveModeUV") + "|" + GetLoc("sDissolveModePosition");
            string sDissolveParamsOther = GetLoc("sDissolveShape") + "|" + GetLoc("sDissolveShapePoint") + "|" + GetLoc("sDissolveShapeLine") + "|" + GetLoc("sBorder") + "|" + GetLoc("sBlur") + "|Dummy";
            string sGlitterParams1 = "Tiling" + "|" + GetLoc("sParticleSize") + "|" + GetLoc("sContrast");
            string sGlitterParams2 = GetLoc("sBlinkSpeed") + "|" + GetLoc("sAngleLimit") + "|" + GetLoc("sRimLightDirection") + "|" + GetLoc("sColorRandomness");
            string[] sRenderingModeList = {GetLoc("sRenderingModeOpaque"), GetLoc("sRenderingModeCutout"), GetLoc("sRenderingModeTransparent"), GetLoc("sRenderingModeRefraction"), GetLoc("sRenderingModeRefractionBlur"), GetLoc("sRenderingModeFur"), GetLoc("sRenderingModeFurCutout"), GetLoc("sRenderingModeGem")};
            string[] sRenderingModeListLite = {GetLoc("sRenderingModeOpaque"), GetLoc("sRenderingModeCutout"), GetLoc("sRenderingModeTransparent")};
            GUIContent textureRGBAContent = new GUIContent(GetLoc("sTexture"), GetLoc("sTextureRGBA"));
            GUIContent colorRGBAContent = new GUIContent(GetLoc("sColor"), GetLoc("sTextureRGBA"));
            GUIContent maskBlendContent = new GUIContent(GetLoc("sMask"), GetLoc("sBlendR"));
            GUIContent alphaMaskContent = new GUIContent(GetLoc("sAlphaMask"), GetLoc("sAlphaR"));
            GUIContent maskStrengthContent = new GUIContent(GetLoc("sStrengthMask"), GetLoc("sStrengthR"));
            GUIContent normalMapContent = new GUIContent(GetLoc("sNormalMap"), GetLoc("sNormalRGB"));
            GUIContent triMaskContent = new GUIContent(GetLoc("sTriMask"), GetLoc("sTriMaskRGB"));
            GUIContent noiseMaskContent = new GUIContent(GetLoc("sNoise"), GetLoc("sNoiseR"));
            GUIContent adjustMaskContent = new GUIContent(GetLoc("sColorAdjustMask"), GetLoc("sBlendR"));

            //------------------------------------------------------------------------------------------------------------------------------
            // Editor Mode
            SelectEditorMode();
            DrawHelpPages();

            if(material.shader.name.Contains("Overlay") && AutoFixHelpBox(GetLoc("sHelpSelectOverlay")))
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
                EditorGUILayout.BeginVertical(boxOuter);
                EditorGUILayout.LabelField(GetLoc("sBaseSetting"), customToggleFont);
                EditorGUILayout.BeginVertical(boxInnerHalf);
                materialEditor.ShaderProperty(invisible, GetLoc("sInvisible"));
                {
                    if(!isCustomShader)
                    {
                        RenderingMode renderingMode;
                        if(isLite) renderingMode = (RenderingMode)EditorGUILayout.Popup(GetLoc("sRenderingMode"), (int)renderingModeBuf, sRenderingModeListLite);
                        else       renderingMode = (RenderingMode)EditorGUILayout.Popup(GetLoc("sRenderingMode"), (int)renderingModeBuf, sRenderingModeList);
                        if(renderingModeBuf != renderingMode)
                        {
                            SetupMaterialWithRenderingMode(material, renderingMode, isOutl, isLite, isStWr, isTess);
                            if(renderingMode == RenderingMode.Cutout || renderingMode == RenderingMode.FurCutout) cutoff.floatValue = 0.5f;
                            if(renderingMode == RenderingMode.Transparent || renderingMode == RenderingMode.Fur) cutoff.floatValue = 0.001f;
                        }
                    }
                        EditorGUI.indentLevel++;
                        if(renderingModeBuf == RenderingMode.Cutout || renderingModeBuf == RenderingMode.Transparent || renderingModeBuf == RenderingMode.Fur || renderingModeBuf == RenderingMode.FurCutout)
                        {
                            materialEditor.ShaderProperty(cutoff, GetLoc("sCutoff"));
                        }
                        if(renderingModeBuf >= RenderingMode.Transparent && renderingModeBuf != RenderingMode.FurCutout)
                        {
                            EditorGUILayout.HelpBox(GetLoc("sHelpRenderingTransparent"),MessageType.Warning);
                        }
                        EditorGUI.indentLevel--;
                    materialEditor.ShaderProperty(cull, sCullModes);
                        EditorGUI.indentLevel++;
                        if(cull.floatValue == 1.0f && AutoFixHelpBox(GetLoc("sHelpCullMode")))
                        {
                            cull.floatValue = 2.0f;
                        }
                        EditorGUI.indentLevel--;
                    materialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                        EditorGUI.indentLevel++;
                        if(zwrite.floatValue != 1.0f && AutoFixHelpBox(GetLoc("sHelpZWrite")))
                        {
                            zwrite.floatValue = 1.0f;
                        }
                        EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();

                //------------------------------------------------------------------------------------------------------------------------------
                // Lite
                if(isLite)
                {
                    // Main
                    //if(useMainTex.floatValue == 1)
                    //{
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainColor"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TexturePropertySingleLine(textureRGBAContent, mainTex, mainColor);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    //}

                    // Shadow
                    EditorGUILayout.BeginVertical(boxOuter);
                    materialEditor.ShaderProperty(useShadow, GetLoc("sShadow"));
                    if(useShadow.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sShadow1stColor"), GetLoc("sTextureRGBA")), shadowColorTex);
                        materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sShadow2ndColor"), GetLoc("sTextureRGBA")), shadow2ndColorTex);
                        materialEditor.ShaderProperty(shadowBorderColor, GetLoc("sShadowBorderColor"));
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();

                    // Outline
                    EditorGUILayout.BeginVertical(boxOuter);
                    if(isOutl != EditorGUILayout.ToggleLeft(GetLoc("sOutline"), isOutl, customToggleFont))
                    {
                        SetupMaterialWithRenderingMode(material, renderingModeBuf, !isOutl, isLite, isStWr, isTess);
                    }
                    if(isOutl)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TexturePropertySingleLine(textureRGBAContent, outlineTex, outlineColor);
                        materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sWidth"), GetLoc("sWidthR")), outlineWidthMask, outlineWidth);
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();

                    // Emission
                    EditorGUILayout.BeginVertical(boxOuter);
                    materialEditor.ShaderProperty(useEmission, GetLoc("sEmission"));
                    if(useEmission.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TexturePropertySingleLine(textureRGBAContent, emissionMap);
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Fur
                else if(isFur)
                {
                    // Main
                    //if(useMainTex.floatValue == 1)
                    //{
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainColor"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TexturePropertySingleLine(textureRGBAContent, mainTex, mainColor);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    //}

                    // Shadow
                    if(shaderSetting.LIL_FEATURE_SHADOW)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        materialEditor.ShaderProperty(useShadow, GetLoc("sShadow"));
                        if(useShadow.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            if(shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH)   materialEditor.TexturePropertySingleLine(maskStrengthContent, shadowStrengthMask, shadowStrength);
                            else                                                materialEditor.ShaderProperty(shadowStrength, GetLoc("sStrength"));
                            if(shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST)    materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sShadow1stColor"), GetLoc("sTextureRGBA")), shadowColorTex, shadowColor);
                            else                                            materialEditor.ShaderProperty(shadowColor, GetLoc("sShadow1stColor"));
                            if(shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND)    materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sShadow2ndColor"), GetLoc("sTextureRGBA")), shadow2ndColorTex, shadow2ndColor);
                            else                                            materialEditor.ShaderProperty(shadow2ndColor, GetLoc("sShadow2ndColor"));
                            materialEditor.ShaderProperty(shadowMainStrength, GetLoc("sMainColorPower"));
                            materialEditor.ShaderProperty(shadowBorderColor, GetLoc("sShadowBorderColor"));
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Gem
                else if(isGem)
                {
                    // Main
                    //if(useMainTex.floatValue == 1)
                    //{
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainColor"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TexturePropertySingleLine(textureRGBAContent, mainTex, mainColor);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    //}

                    // Emission
                    if(shaderSetting.LIL_FEATURE_EMISSION_1ST)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        materialEditor.ShaderProperty(useEmission, GetLoc("sEmission"));
                        if(useEmission.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            materialEditor.TexturePropertySingleLine(textureRGBAContent, emissionMap, emissionColor);
                            if(emissionColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                            {
                                emissionColor.colorValue = new Color(emissionColor.colorValue.r, emissionColor.colorValue.g, emissionColor.colorValue.b, 1.0f);
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }

                    // Emission 2nd
                    if(shaderSetting.LIL_FEATURE_EMISSION_2ND)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        materialEditor.ShaderProperty(useEmission2nd, GetLoc("sEmission2nd"));
                        if(useEmission2nd.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            materialEditor.TexturePropertySingleLine(textureRGBAContent, emission2ndMap, emission2ndColor);
                            if(emission2ndColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                            {
                                emission2ndColor.colorValue = new Color(emission2ndColor.colorValue.r, emission2ndColor.colorValue.g, emission2ndColor.colorValue.b, 1.0f);
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // FakeShadow
                else if(isFakeShadow)
                {
                    // Main
                    //if(useMainTex.floatValue == 1)
                    //{
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainColor"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TexturePropertySingleLine(textureRGBAContent, mainTex, mainColor);
                        GUILayout.Label("FakeShadow", EditorStyles.boldLabel);
                        materialEditor.ShaderProperty(fakeShadowVector, GetLoc("sVector") + "|" + GetLoc("sOffset"));
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    //}
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Normal
                else
                {
                    // Main
                    //if(useMainTex.floatValue == 1)
                    //{
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainColor"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TexturePropertySingleLine(textureRGBAContent, mainTex, mainColor);
                        if(shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION)
                        {
                            ToneCorrectionGUI(materialEditor, material, mainTex, mainColor, mainTexHSVG);
                        }
                        if(shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP)
                        {
                            DrawLine();
                            EditorGUILayout.LabelField(GetLoc("sGradationMap"), EditorStyles.boldLabel);
                            materialEditor.ShaderProperty(mainGradationStrength, GetLoc("sStrength"));
                            if(mainGradationStrength.floatValue != 0)
                            {
                                materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sGradation"), GetLoc("sTextureRGBA")), mainGradationTex);
                                GradientEditor(material, mainGrad, mainGradationTex, true);
                            }
                            DrawLine();
                        }
                        if(shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION || shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP)
                        {
                            materialEditor.TexturePropertySingleLine(adjustMaskContent, mainColorAdjustMask);
                            TextureBakeGUI(material, 4);
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    //}

                    // Main 2nd
                    if(shaderSetting.LIL_FEATURE_MAIN2ND && useMain2ndTex.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainColor2nd"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TexturePropertySingleLine(textureRGBAContent, main2ndTex, mainColor2nd);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    // Main 3rd
                    if(shaderSetting.LIL_FEATURE_MAIN3RD && useMain3rdTex.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainColor3rd"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TexturePropertySingleLine(textureRGBAContent, main3rdTex, mainColor3rd);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    // Shadow
                    if(shaderSetting.LIL_FEATURE_SHADOW)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        materialEditor.ShaderProperty(useShadow, GetLoc("sShadow"));
                        if(useShadow.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            if(shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH)   materialEditor.TexturePropertySingleLine(maskStrengthContent, shadowStrengthMask, shadowStrength);
                            else                                                materialEditor.ShaderProperty(shadowStrength, GetLoc("sStrength"));
                            if(shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST)    materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sShadow1stColor"), GetLoc("sTextureRGBA")), shadowColorTex, shadowColor);
                            else                                            materialEditor.ShaderProperty(shadowColor, GetLoc("sShadow1stColor"));
                            if(shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND)    materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sShadow2ndColor"), GetLoc("sTextureRGBA")), shadow2ndColorTex, shadow2ndColor);
                            else                                            materialEditor.ShaderProperty(shadow2ndColor, GetLoc("sShadow2ndColor"));
                            if(shadow2ndColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                            {
                                shadow2ndColor.colorValue = new Color(shadow2ndColor.colorValue.r, shadow2ndColor.colorValue.g, shadow2ndColor.colorValue.b, 1.0f);
                            }
                            materialEditor.ShaderProperty(shadowMainStrength, GetLoc("sMainColorPower"));
                            materialEditor.ShaderProperty(shadowBorderColor, GetLoc("sShadowBorderColor"));
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }

                    // Outline
                    if(!isRefr & !isFur)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        if(isOutl != EditorGUILayout.ToggleLeft(GetLoc("sOutline"), isOutl, customToggleFont))
                        {
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, !isOutl, isLite, isStWr, isTess);
                        }
                        if(isOutl)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            if(shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR) materialEditor.TexturePropertySingleLine(textureRGBAContent, outlineTex, outlineColor);
                            else                                            materialEditor.ShaderProperty(outlineColor, GetLoc("sColor"));
                            if(shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION)
                            {
                                ToneCorrectionGUI(materialEditor, material, outlineTex, outlineColor, outlineTexHSVG);
                                if(GUILayout.Button(GetLoc("sBake")))
                                {
                                    outlineTex.textureValue = AutoBakeOutlineTexture(material);
                                    outlineTexHSVG.vectorValue = defaultHSVG;
                                }
                            }
                            if(shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH) materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sWidth"), GetLoc("sWidthR")), outlineWidthMask, outlineWidth);
                            else                                            materialEditor.ShaderProperty(outlineWidth, GetLoc("sWidth"));
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }

                    // Emission
                    if(shaderSetting.LIL_FEATURE_EMISSION_1ST)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        materialEditor.ShaderProperty(useEmission, GetLoc("sEmission"));
                        if(useEmission.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            materialEditor.TexturePropertySingleLine(textureRGBAContent, emissionMap, emissionColor);
                            if(emissionColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                            {
                                emissionColor.colorValue = new Color(emissionColor.colorValue.r, emissionColor.colorValue.g, emissionColor.colorValue.b, 1.0f);
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }

                    // Emission 2nd
                    if(shaderSetting.LIL_FEATURE_EMISSION_2ND)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        materialEditor.ShaderProperty(useEmission2nd, GetLoc("sEmission2nd"));
                        if(useEmission2nd.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            materialEditor.TexturePropertySingleLine(textureRGBAContent, emission2ndMap, emission2ndColor);
                            if(emission2ndColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                            {
                                emission2ndColor.colorValue = new Color(emission2ndColor.colorValue.r, emission2ndColor.colorValue.g, emission2ndColor.colorValue.b, 1.0f);
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }

                    if(mtoon != null && GUILayout.Button(GetLoc("sConvertMToon"), offsetButton)) CreateMToonMaterial(material);
                }
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Advanced
            if(edSet.editorMode == EditorMode.Advanced)
            {
                if(isLite)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // Base Setting
                    GUILayout.Label(" " + GetLoc("sBaseSetting"), EditorStyles.boldLabel);
                    DrawHelpButton(GetLoc("sAnchorBaseSetting"));
                    EditorGUILayout.BeginVertical(customBox);
                    {
                        materialEditor.ShaderProperty(invisible, GetLoc("sInvisible"));
                        if(!isCustomShader)
                        {
                            RenderingMode renderingMode;
                            renderingMode = (RenderingMode)EditorGUILayout.Popup(GetLoc("sRenderingMode"), (int)renderingModeBuf, sRenderingModeListLite);
                            if(renderingModeBuf != renderingMode)
                            {
                                SetupMaterialWithRenderingMode(material, renderingMode, isOutl, isLite, isStWr, isTess);
                                if(renderingMode == RenderingMode.Cutout || renderingMode == RenderingMode.FurCutout) cutoff.floatValue = 0.5f;
                                if(renderingMode == RenderingMode.Transparent || renderingMode == RenderingMode.Fur) cutoff.floatValue = 0.001f;
                            }
                        }
                            EditorGUI.indentLevel++;
                            if(renderingModeBuf == RenderingMode.Cutout || renderingModeBuf == RenderingMode.Transparent || renderingModeBuf == RenderingMode.Fur || renderingModeBuf == RenderingMode.FurCutout)
                            {
                                materialEditor.ShaderProperty(cutoff, GetLoc("sCutoff"));
                            }
                            if(renderingModeBuf >= RenderingMode.Transparent && renderingModeBuf != RenderingMode.FurCutout)
                            {
                                EditorGUILayout.HelpBox(GetLoc("sHelpRenderingTransparent"),MessageType.Warning);
                            }
                            EditorGUI.indentLevel--;
                        materialEditor.ShaderProperty(cull, sCullModes);
                            EditorGUI.indentLevel++;
                            if(cull.floatValue == 1.0f && AutoFixHelpBox(GetLoc("sHelpCullMode")))
                            {
                                cull.floatValue = 2.0f;
                            }
                            if(cull.floatValue <= 1.0f)
                            {
                                materialEditor.ShaderProperty(flipNormal, GetLoc("sFlipBackfaceNormal"));
                                materialEditor.ShaderProperty(backfaceForceShadow, GetLoc("sBackfaceForceShadow"));
                            }
                            EditorGUI.indentLevel--;
                        materialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                            EditorGUI.indentLevel++;
                            if(zwrite.floatValue != 1.0f && AutoFixHelpBox(GetLoc("sHelpZWrite")))
                            {
                                zwrite.floatValue = 1.0f;
                            }
                            EditorGUI.indentLevel--;
                        DrawLine();
                        materialEditor.ShaderProperty(asUnlit, GetLoc("sAsUnlit"));
                        materialEditor.ShaderProperty(vertexLightStrength, GetLoc("sVertexLightStrength"));
                        materialEditor.ShaderProperty(lightMinLimit, GetLoc("sLightMinLimit"));
                        DrawLine();
                        materialEditor.TexturePropertySingleLine(triMaskContent, triMask);
                    }
                    EditorGUILayout.EndVertical();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // UV
                    edSet.isShowMainUV = Foldout(GetLoc("sMainUV"), GetLoc("sMainUVTips"), edSet.isShowMainUV);
                    DrawHelpButton(GetLoc("sAnchorUVSetting"));
                    if(edSet.isShowMainUV)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainUV"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        UVSettingGUI(materialEditor, mainTex, mainTex_ScrollRotate);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.Space();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Colors
                    GUILayout.Label(" " + GetLoc("sColors"), EditorStyles.boldLabel);

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Main Color
                    edSet.isShowMain = Foldout(GetLoc("sMainColorSetting"), GetLoc("sMainColorTips"), edSet.isShowMain);
                    DrawHelpButton(GetLoc("sAnchorMainColor"));
                    if(edSet.isShowMain)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Main
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainColor"), customToggleFont);
                        DrawHelpButton(GetLoc("sAnchorMainColor1"));
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TexturePropertySingleLine(textureRGBAContent, mainTex, mainColor);
                        if(isCutout || isTransparent)
                        {
                            SetAlphaIsTransparencyGUI(mainTex);
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Shadow
                    edSet.isShowShadow = Foldout(GetLoc("sShadowSetting"), GetLoc("sShadowTips"), edSet.isShowShadow);
                    DrawHelpButton(GetLoc("sAnchorShadow"));
                    if(edSet.isShowShadow)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        materialEditor.ShaderProperty(useShadow, GetLoc("sShadow"));
                        if(useShadow.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            materialEditor.ShaderProperty(shadowBorder, GetLoc("sBorder"));
                            materialEditor.ShaderProperty(shadowBlur, GetLoc("sBlur"));
                            DrawLine();
                            materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sShadow1stColor"), GetLoc("sTextureRGBA")), shadowColorTex);
                            DrawLine();
                            materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sShadow2ndColor"), GetLoc("sTextureRGBA")), shadow2ndColorTex);
                            materialEditor.ShaderProperty(shadow2ndBorder, GetLoc("sBorder"));
                            materialEditor.ShaderProperty(shadow2ndBlur, GetLoc("sBlur"));
                            DrawLine();
                            materialEditor.ShaderProperty(shadowEnvStrength, GetLoc("sShadowEnvStrength"));
                            materialEditor.ShaderProperty(shadowBorderColor, GetLoc("sShadowBorderColor"));
                            materialEditor.ShaderProperty(shadowBorderRange, GetLoc("sShadowBorderRange"));
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Emission
                    edSet.isShowEmission = Foldout(GetLoc("sEmissionSetting"), GetLoc("sEmissionTips"), edSet.isShowEmission);
                    DrawHelpButton(GetLoc("sAnchorEmission"));
                    if(edSet.isShowEmission)
                    {
                        // Emission
                        EditorGUILayout.BeginVertical(boxOuter);
                        materialEditor.ShaderProperty(useEmission, GetLoc("sEmission"));
                        if(useEmission.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            TextureGUI(materialEditor, ref edSet.isShowEmissionMap, colorRGBAContent, emissionMap, emissionColor, emissionMap_ScrollRotate, true, true);
                            DrawLine();
                            materialEditor.ShaderProperty(emissionBlink, blinkSetting);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.Space();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Normal & Reflection
                    GUILayout.Label(" " + GetLoc("sNormalMapReflection"), EditorStyles.boldLabel);

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Reflection
                    edSet.isShowReflections = Foldout(GetLoc("sReflectionsSetting"), GetLoc("sReflectionsTips"), edSet.isShowReflections);
                    DrawHelpButton(GetLoc("sAnchorReflections"));
                    if(edSet.isShowReflections)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // MatCap
                        EditorGUILayout.BeginVertical(boxOuter);
                        materialEditor.ShaderProperty(useMatCap, GetLoc("sMatCap"));
                        DrawHelpButton(GetLoc("sAnchorMatCap"));
                        if(useMatCap.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sMatCap"), GetLoc("sTextureRGBA")), matcapTex);
                            materialEditor.ShaderProperty(matcapMul, GetLoc("sBlendModeMul"));
                            materialEditor.ShaderProperty(matcapZRotCancel, GetLoc("sMatCapZRotCancel"));
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Rim
                        EditorGUILayout.BeginVertical(boxOuter);
                        materialEditor.ShaderProperty(useRim, GetLoc("sRimLight"));
                        DrawHelpButton(GetLoc("sAnchorRimLight"));
                        if(useRim.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            materialEditor.ShaderProperty(rimColor, GetLoc("sColor"));
                            materialEditor.ShaderProperty(rimShadowMask, GetLoc("sShadowMask"));
                            DrawLine();
                            rimBorder.floatValue = 1.0f - EditorGUILayout.Slider(GetLoc("sBorder"), 1.0f - rimBorder.floatValue, 0.0f, 1.0f);
                            materialEditor.ShaderProperty(rimBlur, GetLoc("sBlur"));
                            materialEditor.ShaderProperty(rimFresnelPower, GetLoc("sFresnelPower"));
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.Space();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Advanced
                    GUILayout.Label(" " + GetLoc("sAdvanced"), EditorStyles.boldLabel);

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Outline
                    edSet.isShowOutline = Foldout(GetLoc("sOutlineSetting"), GetLoc("sOutlineTips"), edSet.isShowOutline);
                    DrawHelpButton(GetLoc("sAnchorOutline"));
                    if(edSet.isShowOutline)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        if(isOutl != EditorGUILayout.ToggleLeft(GetLoc("sOutline"), isOutl, customToggleFont))
                        {
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, !isOutl, isLite, isStWr, isTess);
                        }
                        if(isOutl)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            TextureGUI(materialEditor, ref edSet.isShowOutlineMap, colorRGBAContent, outlineTex, outlineColor, outlineTex_ScrollRotate, true, shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV);
                            materialEditor.ShaderProperty(outlineEnableLighting, GetLoc("sEnableLighting"));
                            DrawLine();
                            materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sWidth"), GetLoc("sWidthR")), outlineWidthMask, outlineWidth);
                            materialEditor.ShaderProperty(outlineFixWidth, GetLoc("sFixWidth"));
                            materialEditor.ShaderProperty(outlineVertexR2Width, GetLoc("sVertexR2Width"));
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Stencil
                    edSet.isShowStencil = Foldout(GetLoc("sStencilSetting"), GetLoc("sStencilTips"), edSet.isShowStencil);
                    DrawHelpButton(GetLoc("sAnchorStencil"));
                    if(edSet.isShowStencil)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sStencilSetting"), customToggleFont);
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
                                EditorGUILayout.LabelField(GetLoc("sOutline"), EditorStyles.boldLabel);
                                outlineStencilRef.floatValue = 1;
                                outlineStencilReadMask.floatValue = 255.0f;
                                outlineStencilWriteMask.floatValue = 255.0f;
                                outlineStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                                outlineStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Replace;
                                outlineStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                                outlineStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                            }
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, isOutl, isLite, isStWr, isTess);
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
                                EditorGUILayout.LabelField(GetLoc("sOutline"), EditorStyles.boldLabel);
                                outlineStencilRef.floatValue = 1;
                                outlineStencilReadMask.floatValue = 255.0f;
                                outlineStencilWriteMask.floatValue = 255.0f;
                                outlineStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.NotEqual;
                                outlineStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                                outlineStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                                outlineStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                            }
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, isOutl, isLite, isStWr, isTess);
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
                                EditorGUILayout.LabelField(GetLoc("sOutline"), EditorStyles.boldLabel);
                                outlineStencilRef.floatValue = 0;
                                outlineStencilReadMask.floatValue = 255.0f;
                                outlineStencilWriteMask.floatValue = 255.0f;
                                outlineStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                                outlineStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                                outlineStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                                outlineStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                            }
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, isOutl, isLite, isStWr, isTess);
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Base
                        {
                            DrawLine();
                            materialEditor.ShaderProperty(stencilRef, "Ref");
                            materialEditor.ShaderProperty(stencilReadMask, "ReadMask");
                            materialEditor.ShaderProperty(stencilWriteMask, "WriteMask");
                            materialEditor.ShaderProperty(stencilComp, "Comp");
                            materialEditor.ShaderProperty(stencilPass, "Pass");
                            materialEditor.ShaderProperty(stencilFail, "Fail");
                            materialEditor.ShaderProperty(stencilZFail, "ZFail");
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Outline
                        if(isOutl)
                        {
                            DrawLine();
                            EditorGUILayout.LabelField(GetLoc("sOutline"), EditorStyles.boldLabel);
                            materialEditor.ShaderProperty(outlineStencilRef, "Ref");
                            materialEditor.ShaderProperty(outlineStencilReadMask, "ReadMask");
                            materialEditor.ShaderProperty(outlineStencilWriteMask, "WriteMask");
                            materialEditor.ShaderProperty(outlineStencilComp, "Comp");
                            materialEditor.ShaderProperty(outlineStencilPass, "Pass");
                            materialEditor.ShaderProperty(outlineStencilFail, "Fail");
                            materialEditor.ShaderProperty(outlineStencilZFail, "ZFail");
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Rendering
                    edSet.isShowRendering = Foldout(GetLoc("sRenderingSetting"), GetLoc("sRenderingTips"), edSet.isShowRendering);
                    DrawHelpButton(GetLoc("sAnchorRendering"));
                    if(edSet.isShowRendering)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Reset Button
                        if(GUILayout.Button(GetLoc("sRenderingReset"), offsetButton))
                        {
                            material.enableInstancing = false;
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, isOutl, isLite, isStWr, isTess);
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
                            shaderType = EditorGUILayout.Popup(GetLoc("sShaderType"),shaderType,new String[]{GetLoc("sShaderTypeNormal"),GetLoc("sShaderTypeLite")});
                            if(shaderTypeBuf != shaderType)
                            {
                                if(shaderType==0) isLite = false;
                                if(shaderType==1) isLite = true;
                                SetupMaterialWithRenderingMode(material, renderingModeBuf, isOutl, isLite, isStWr, isTess);
                            }

                            //------------------------------------------------------------------------------------------------------------------------------
                            // Rendering
                            materialEditor.ShaderProperty(cull, sCullModes);
                            materialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                            materialEditor.ShaderProperty(ztest, GetLoc("sZTest"));
                            materialEditor.ShaderProperty(offsetFactor, GetLoc("sOffsetFactor"));
                            materialEditor.ShaderProperty(offsetUnits, GetLoc("sOffsetUnits"));
                            materialEditor.ShaderProperty(colorMask, GetLoc("sColorMask"));
                            DrawLine();
                            BlendSettingGUI(materialEditor, ref edSet.isShowBlend, GetLoc("sForward"), srcBlend, dstBlend, srcBlendAlpha, dstBlendAlpha, blendOp, blendOpAlpha);
                            if(!(isFur & !isCutout))
                            {
                                DrawLine();
                                BlendSettingGUI(materialEditor, ref edSet.isShowBlendAdd, GetLoc("sForwardAdd"), srcBlendFA, dstBlendFA, srcBlendAlphaFA, dstBlendAlphaFA, blendOpFA, blendOpAlphaFA);
                            }
                            DrawLine();
                            materialEditor.EnableInstancingField();
                            materialEditor.DoubleSidedGIField();
                            materialEditor.RenderQueueField();
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
                            materialEditor.ShaderProperty(outlineCull, sCullModes);
                            materialEditor.ShaderProperty(outlineZwrite, GetLoc("sZWrite"));
                            materialEditor.ShaderProperty(outlineZtest, GetLoc("sZTest"));
                            materialEditor.ShaderProperty(outlineOffsetFactor, GetLoc("sOffsetFactor"));
                            materialEditor.ShaderProperty(outlineOffsetUnits, GetLoc("sOffsetUnits"));
                            materialEditor.ShaderProperty(outlineColorMask, GetLoc("sColorMask"));
                            DrawLine();
                            BlendSettingGUI(materialEditor, ref edSet.isShowBlendOutline, GetLoc("sForward"), outlineSrcBlend, outlineDstBlend, outlineSrcBlendAlpha, outlineDstBlendAlpha, outlineBlendOp, outlineBlendOpAlpha);
                            DrawLine();
                            BlendSettingGUI(materialEditor, ref edSet.isShowBlendAddOutline, GetLoc("sForwardAdd"), outlineSrcBlendFA, outlineDstBlendFA, outlineSrcBlendAlphaFA, outlineDstBlendAlphaFA, outlineBlendOpFA, outlineBlendOpAlphaFA);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Optimization
                    GUILayout.Label(" " + GetLoc("sOptimization"), EditorStyles.boldLabel);
                    edSet.isShowOptimization = Foldout(GetLoc("sOptimization"), GetLoc("sOptimizationTips"), edSet.isShowOptimization);
                    DrawHelpButton(GetLoc("sAnchorOptimization"));
                    if(edSet.isShowOptimization)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sOptimization"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        if(GUILayout.Button(GetLoc("sRemoveUnused"))) RemoveUnusedTexture(material, isLite, isFur, shaderSetting);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }
                else if(isGem)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // Base Setting
                    GUILayout.Label(" " + GetLoc("sBaseSetting"), EditorStyles.boldLabel);
                    DrawHelpButton(GetLoc("sAnchorBaseSetting"));
                    EditorGUILayout.BeginVertical(customBox);
                    {
                        materialEditor.ShaderProperty(invisible, GetLoc("sInvisible"));
                        if(!isCustomShader)
                        {
                            RenderingMode renderingMode;
                            renderingMode = (RenderingMode)EditorGUILayout.Popup(GetLoc("sRenderingMode"), (int)renderingModeBuf, sRenderingModeList);
                            if(renderingModeBuf != renderingMode)
                            {
                                SetupMaterialWithRenderingMode(material, renderingMode, isOutl, isLite, isStWr, isTess);
                                if(renderingMode == RenderingMode.Cutout || renderingMode == RenderingMode.FurCutout) cutoff.floatValue = 0.5f;
                                if(renderingMode == RenderingMode.Transparent || renderingMode == RenderingMode.Fur) cutoff.floatValue = 0.001f;
                            }
                        }
                            EditorGUI.indentLevel++;
                            if(renderingModeBuf >= RenderingMode.Transparent && renderingModeBuf != RenderingMode.FurCutout)
                            {
                                EditorGUILayout.HelpBox(GetLoc("sHelpRenderingTransparent"),MessageType.Warning);
                            }
                            EditorGUI.indentLevel--;
                        materialEditor.ShaderProperty(cull, sCullModes);
                        materialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                        DrawLine();
                        materialEditor.ShaderProperty(asUnlit, GetLoc("sAsUnlit"));
                        materialEditor.ShaderProperty(vertexLightStrength, GetLoc("sVertexLightStrength"));
                        materialEditor.ShaderProperty(lightMinLimit, GetLoc("sLightMinLimit"));
                    }
                    EditorGUILayout.EndVertical();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // UV
                    edSet.isShowMainUV = Foldout(GetLoc("sMainUV"), GetLoc("sMainUVTips"), edSet.isShowMainUV);
                    DrawHelpButton(GetLoc("sAnchorUVSetting"));
                    if(edSet.isShowMainUV)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainUV"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        if(shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV)   UVSettingGUI(materialEditor, mainTex, mainTex_ScrollRotate);
                        else                                            materialEditor.TextureScaleOffsetProperty(mainTex);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.Space();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Colors
                    GUILayout.Label(" " + GetLoc("sColors"), EditorStyles.boldLabel);

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Main Color
                    edSet.isShowMain = Foldout(GetLoc("sMainColorSetting"), GetLoc("sMainColorTips"), edSet.isShowMain);
                    DrawHelpButton(GetLoc("sAnchorMainColor"));
                    if(edSet.isShowMain)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Main
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainColor"), customToggleFont);
                        DrawHelpButton(GetLoc("sAnchorMainColor1"));
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TexturePropertySingleLine(textureRGBAContent, mainTex, mainColor);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Emission
                    if(shaderSetting.LIL_FEATURE_EMISSION_1ST || shaderSetting.LIL_FEATURE_EMISSION_2ND)
                    {
                        edSet.isShowEmission = Foldout(GetLoc("sEmissionSetting"), GetLoc("sEmissionTips"), edSet.isShowEmission);
                        DrawHelpButton(GetLoc("sAnchorEmission"));
                        if(edSet.isShowEmission)
                        {
                            // Emission
                            if(shaderSetting.LIL_FEATURE_EMISSION_1ST)
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                materialEditor.ShaderProperty(useEmission, GetLoc("sEmission"));
                                if(useEmission.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    TextureGUI(materialEditor, ref edSet.isShowEmissionMap, colorRGBAContent, emissionMap, emissionColor, emissionMap_ScrollRotate, shaderSetting.LIL_FEATURE_EMISSION_UV, shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV);
                                    if(emissionColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                    {
                                        emissionColor.colorValue = new Color(emissionColor.colorValue.r, emissionColor.colorValue.g, emissionColor.colorValue.b, 1.0f);
                                    }
                                    DrawLine();
                                    if(shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK)
                                    {
                                        TextureGUI(materialEditor, ref edSet.isShowEmissionBlendMask, maskBlendContent, emissionBlendMask, emissionBlend, emissionBlendMask_ScrollRotate, shaderSetting.LIL_FEATURE_EMISSION_MASK_UV, shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV);
                                        DrawLine();
                                    }
                                    materialEditor.ShaderProperty(emissionBlink, blinkSetting);
                                    DrawLine();
                                    if(shaderSetting.LIL_FEATURE_EMISSION_GRADATION)
                                    {
                                        materialEditor.ShaderProperty(emissionUseGrad, GetLoc("sGradation"));
                                        if(emissionUseGrad.floatValue == 1)
                                        {
                                            materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sGradTexSpeed"), GetLoc("sTextureRGBA")), emissionGradTex, emissionGradSpeed);
                                            GradientEditor(material, "_eg", emiGrad, emissionGradSpeed);
                                        }
                                        DrawLine();
                                    }
                                    materialEditor.ShaderProperty(emissionParallaxDepth, GetLoc("sParallaxDepth"));
                                    materialEditor.ShaderProperty(emissionFluorescence, GetLoc("sFluorescence"));
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndVertical();
                            }

                            // Emission 2nd
                            if(shaderSetting.LIL_FEATURE_EMISSION_2ND)
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                materialEditor.ShaderProperty(useEmission2nd, GetLoc("sEmission2nd"));
                                if(useEmission2nd.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    TextureGUI(materialEditor, ref edSet.isShowEmission2ndMap, colorRGBAContent, emission2ndMap, emission2ndColor, emission2ndMap_ScrollRotate, shaderSetting.LIL_FEATURE_EMISSION_UV, shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV);
                                    if(emission2ndColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                    {
                                        emission2ndColor.colorValue = new Color(emission2ndColor.colorValue.r, emission2ndColor.colorValue.g, emission2ndColor.colorValue.b, 1.0f);
                                    }
                                    DrawLine();
                                    if(shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK)
                                    {
                                        TextureGUI(materialEditor, ref edSet.isShowEmission2ndBlendMask, maskBlendContent, emission2ndBlendMask, emission2ndBlend, emission2ndBlendMask_ScrollRotate, shaderSetting.LIL_FEATURE_EMISSION_MASK_UV, shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV);
                                        DrawLine();
                                    }
                                    materialEditor.ShaderProperty(emission2ndBlink, blinkSetting);
                                    DrawLine();
                                    if(shaderSetting.LIL_FEATURE_EMISSION_GRADATION)
                                    {
                                        materialEditor.ShaderProperty(emission2ndUseGrad, GetLoc("sGradation"));
                                        if(emission2ndUseGrad.floatValue == 1)
                                        {
                                            materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sGradTexSpeed"), GetLoc("sTextureRGBA")), emission2ndGradTex, emission2ndGradSpeed);
                                            GradientEditor(material, "_e2g", emi2Grad, emission2ndGradSpeed);
                                        }
                                        DrawLine();
                                    }
                                    materialEditor.ShaderProperty(emission2ndParallaxDepth, GetLoc("sParallaxDepth"));
                                    materialEditor.ShaderProperty(emission2ndFluorescence, GetLoc("sFluorescence"));
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndVertical();
                            }
                        }

                        EditorGUILayout.Space();
                    }

                    EditorGUILayout.Space();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Normal & Reflection
                    GUILayout.Label(" " + GetLoc("sNormalMapReflection"), EditorStyles.boldLabel);

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Normal
                    if(shaderSetting.LIL_FEATURE_NORMAL_1ST || shaderSetting.LIL_FEATURE_NORMAL_2ND)
                    {
                        edSet.isShowBump = Foldout(GetLoc("sNormalMapSetting"), GetLoc("sNormalMapTips"), edSet.isShowBump);
                        DrawHelpButton(GetLoc("sAnchorNormalMap"));
                        if(edSet.isShowBump)
                        {
                            //------------------------------------------------------------------------------------------------------------------------------
                            // 1st
                            if(shaderSetting.LIL_FEATURE_NORMAL_1ST)
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                materialEditor.ShaderProperty(useBumpMap, GetLoc("sNormalMap"));
                                if(useBumpMap.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    materialEditor.TexturePropertySingleLine(normalMapContent, bumpMap, bumpScale);
                                    materialEditor.TextureScaleOffsetProperty(bumpMap);
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndVertical();
                            }

                            //------------------------------------------------------------------------------------------------------------------------------
                            // 2nd
                            if(shaderSetting.LIL_FEATURE_NORMAL_2ND)
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                materialEditor.ShaderProperty(useBump2ndMap, GetLoc("sNormalMap2nd"));
                                if(useBump2ndMap.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    materialEditor.TexturePropertySingleLine(normalMapContent, bump2ndMap, bump2ndScale);
                                    materialEditor.TextureScaleOffsetProperty(bump2ndMap);
                                    if(shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK) materialEditor.TexturePropertySingleLine(maskStrengthContent, bump2ndScaleMask);
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndVertical();
                            }
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Reflection
                    edSet.isShowReflections = Foldout(GetLoc("sReflectionsSetting"), GetLoc("sReflectionsTips"), edSet.isShowReflections);
                    DrawHelpButton(GetLoc("sAnchorReflections"));
                    if(edSet.isShowReflections)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // MatCap
                        if(shaderSetting.LIL_FEATURE_MATCAP)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            materialEditor.ShaderProperty(useMatCap, GetLoc("sMatCap"));
                            DrawHelpButton(GetLoc("sAnchorMatCap"));
                            if(useMatCap.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sMatCap"), GetLoc("sTextureRGBA")), matcapTex, matcapColor);
                                if(matcapColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                {
                                    matcapColor.colorValue = new Color(matcapColor.colorValue.r, matcapColor.colorValue.g, matcapColor.colorValue.b, 1.0f);
                                }
                                if(shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK)   materialEditor.TexturePropertySingleLine(maskBlendContent, matcapBlendMask, matcapBlend);
                                else                                            materialEditor.ShaderProperty(matcapBlend, GetLoc("sBlend"));
                                materialEditor.ShaderProperty(matcapEnableLighting, GetLoc("sEnableLighting"));
                                materialEditor.ShaderProperty(matcapBlendMode, sBlendModes);
                                if(matcapEnableLighting.floatValue != 0.0f && matcapBlendMode.floatValue == 3.0f && AutoFixHelpBox(GetLoc("sHelpMatCapBlending")))
                                {
                                    matcapEnableLighting.floatValue = 0.0f;
                                }
                                if(isTransparent) materialEditor.ShaderProperty(matcapApplyTransparency, GetLoc("sApplyTransparency"));
                                materialEditor.ShaderProperty(matcapZRotCancel, GetLoc("sMatCapZRotCancel"));
                                if(shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP)
                                {
                                    DrawLine();
                                    materialEditor.ShaderProperty(matcapCustomNormal, GetLoc("sMatCapCustomNormal"));
                                    if(matcapCustomNormal.floatValue == 1)
                                    {
                                        materialEditor.TexturePropertySingleLine(normalMapContent, matcapBumpMap, matcapBumpScale);
                                        materialEditor.TextureScaleOffsetProperty(matcapBumpMap);
                                    }
                                }
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // MatCap 2nd
                        if(shaderSetting.LIL_FEATURE_MATCAP_2ND)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            materialEditor.ShaderProperty(useMatCap2nd, GetLoc("sMatCap2nd"));
                            DrawHelpButton(GetLoc("sAnchorMatCap"));
                            if(useMatCap2nd.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sMatCap"), GetLoc("sTextureRGBA")), matcap2ndTex, matcap2ndColor);
                                if(matcap2ndColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                {
                                    matcap2ndColor.colorValue = new Color(matcap2ndColor.colorValue.r, matcap2ndColor.colorValue.g, matcap2ndColor.colorValue.b, 1.0f);
                                }
                                if(shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK)   materialEditor.TexturePropertySingleLine(maskBlendContent, matcap2ndBlendMask, matcap2ndBlend);
                                else                                            materialEditor.ShaderProperty(matcap2ndBlend, GetLoc("sBlend"));
                                materialEditor.ShaderProperty(matcap2ndEnableLighting, GetLoc("sEnableLighting"));
                                materialEditor.ShaderProperty(matcap2ndBlendMode, sBlendModes);
                                if(matcap2ndEnableLighting.floatValue != 0.0f && matcap2ndBlendMode.floatValue == 3.0f && AutoFixHelpBox(GetLoc("sHelpMatCapBlending")))
                                {
                                    matcap2ndEnableLighting.floatValue = 0.0f;
                                }
                                if(isTransparent) materialEditor.ShaderProperty(matcap2ndApplyTransparency, GetLoc("sApplyTransparency"));
                                materialEditor.ShaderProperty(matcap2ndZRotCancel, GetLoc("sMatCapZRotCancel"));
                                if(shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP)
                                {
                                    DrawLine();
                                    materialEditor.ShaderProperty(matcap2ndCustomNormal, GetLoc("sMatCapCustomNormal"));
                                    if(matcap2ndCustomNormal.floatValue == 1)
                                    {
                                        materialEditor.TexturePropertySingleLine(normalMapContent, matcap2ndBumpMap, matcap2ndBumpScale);
                                        materialEditor.TextureScaleOffsetProperty(matcap2ndBumpMap);
                                    }
                                }
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Rim
                        if(shaderSetting.LIL_FEATURE_RIMLIGHT)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            materialEditor.ShaderProperty(useRim, GetLoc("sRimLight"));
                            DrawHelpButton(GetLoc("sAnchorRimLight"));
                            if(useRim.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                if(shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR)    materialEditor.TexturePropertySingleLine(colorRGBAContent, rimColorTex, rimColor);
                                else                                                materialEditor.ShaderProperty(rimColor, GetLoc("sColor"));
                                if(rimColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                {
                                    rimColor.colorValue = new Color(rimColor.colorValue.r, rimColor.colorValue.g, rimColor.colorValue.b, 1.0f);
                                }
                                materialEditor.ShaderProperty(rimEnableLighting, GetLoc("sEnableLighting"));
                                materialEditor.ShaderProperty(rimShadowMask, GetLoc("sShadowMask"));
                                if(isTransparent) materialEditor.ShaderProperty(rimApplyTransparency, GetLoc("sApplyTransparency"));
                                DrawLine();
                                if(shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION)
                                {
                                    materialEditor.ShaderProperty(rimDirStrength, GetLoc("sRimLightDirection"));
                                    if(rimDirStrength.floatValue != 0)
                                    {
                                        EditorGUI.indentLevel++;
                                        materialEditor.ShaderProperty(rimDirRange, GetLoc("sRimDirectionRange"));
                                        rimBorder.floatValue = 1.0f - EditorGUILayout.Slider(GetLoc("sBorder"), 1.0f - rimBorder.floatValue, 0.0f, 1.0f);
                                        materialEditor.ShaderProperty(rimBlur, GetLoc("sBlur"));
                                        DrawLine();
                                        materialEditor.ShaderProperty(rimIndirRange, GetLoc("sRimIndirectionRange"));
                                        materialEditor.ShaderProperty(rimIndirColor, GetLoc("sColor"));
                                        rimIndirBorder.floatValue = 1.0f - EditorGUILayout.Slider(GetLoc("sBorder"), 1.0f - rimIndirBorder.floatValue, 0.0f, 1.0f);
                                        materialEditor.ShaderProperty(rimIndirBlur, GetLoc("sBlur"));
                                        EditorGUI.indentLevel--;
                                        DrawLine();
                                        materialEditor.ShaderProperty(rimFresnelPower, GetLoc("sFresnelPower"));
                                    }
                                    else
                                    {
                                        rimBorder.floatValue = 1.0f - EditorGUILayout.Slider(GetLoc("sBorder"), 1.0f - rimBorder.floatValue, 0.0f, 1.0f);
                                        materialEditor.ShaderProperty(rimBlur, GetLoc("sBlur"));
                                        materialEditor.ShaderProperty(rimFresnelPower, GetLoc("sFresnelPower"));
                                    }
                                }
                                else
                                {
                                    rimBorder.floatValue = 1.0f - EditorGUILayout.Slider(GetLoc("sBorder"), 1.0f - rimBorder.floatValue, 0.0f, 1.0f);
                                    materialEditor.ShaderProperty(rimBlur, GetLoc("sBlur"));
                                    materialEditor.ShaderProperty(rimFresnelPower, GetLoc("sFresnelPower"));
                                }
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Glitter
                        if(shaderSetting.LIL_FEATURE_GLITTER)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            materialEditor.ShaderProperty(useGlitter, GetLoc("sGlitter"));
                            DrawHelpButton(GetLoc("sAnchorGlitter"));
                            if(useGlitter.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                materialEditor.ShaderProperty(glitterUVMode, "UV Mode|UV0|UV1");
                                materialEditor.TexturePropertySingleLine(colorRGBAContent, glitterColorTex, glitterColor);
                                if(glitterColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                {
                                    glitterColor.colorValue = new Color(glitterColor.colorValue.r, glitterColor.colorValue.g, glitterColor.colorValue.b, 1.0f);
                                }
                                materialEditor.ShaderProperty(glitterMainStrength, GetLoc("sMainColorPower"));
                                materialEditor.ShaderProperty(glitterEnableLighting, GetLoc("sEnableLighting"));
                                materialEditor.ShaderProperty(glitterShadowMask, GetLoc("sShadowMask"));
                                if(isTransparent) materialEditor.ShaderProperty(glitterApplyTransparency, GetLoc("sApplyTransparency"));
                                DrawLine();
                                materialEditor.ShaderProperty(glitterParams1, sGlitterParams1);
                                materialEditor.ShaderProperty(glitterParams2, sGlitterParams2);
                                materialEditor.ShaderProperty(glitterVRParallaxStrength, GetLoc("sVRParallaxStrength"));
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Gem
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sGem"), customToggleFont);
                        DrawHelpButton(GetLoc("sAnchorGem"));
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        GUILayout.Label(GetLoc("sRefraction"), EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        materialEditor.ShaderProperty(refractionStrength, GetLoc("sStrength"));
                        materialEditor.ShaderProperty(refractionFresnelPower, GetLoc("sRefractionFresnel"));
                        EditorGUI.indentLevel--;
                        DrawLine();
                        GUILayout.Label(GetLoc("sGem"), EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        materialEditor.ShaderProperty(gemChromaticAberration, GetLoc("sChromaticAberration"));
                        materialEditor.ShaderProperty(gemEnvContrast, GetLoc("sContrast"));
                        materialEditor.ShaderProperty(gemEnvColor, GetLoc("sEnvironmentColor"));
                        DrawLine();
                        materialEditor.ShaderProperty(gemParticleLoop, GetLoc("sParticleLoop"));
                        materialEditor.ShaderProperty(gemParticleColor, GetLoc("sColor"));
                        EditorGUI.indentLevel--;
                        DrawLine();
                        materialEditor.ShaderProperty(gemVRParallaxStrength, GetLoc("sVRParallaxStrength"));
                        if(shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS) materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sSmoothness"), GetLoc("sSmoothnessR")), smoothnessTex, smoothness);
                        else                                                    materialEditor.ShaderProperty(smoothness, GetLoc("sSmoothness"));
                        materialEditor.ShaderProperty(reflectance, GetLoc("sReflectance"));
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.Space();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Advanced
                    GUILayout.Label(" " + GetLoc("sAdvanced"), EditorStyles.boldLabel);

                    //------------------------------------------------------------------------------------------------------------------------------
                    // AudioLink
                    if(shaderSetting.LIL_FEATURE_AUDIOLINK && !isFur)
                    {
                        edSet.isShowAudioLink = Foldout(GetLoc("sAudioLink"), GetLoc("sAudioLinkTips"), edSet.isShowAudioLink);
                        DrawHelpButton(GetLoc("sAnchorAudioLink"));
                        if(edSet.isShowAudioLink)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            materialEditor.ShaderProperty(useAudioLink, GetLoc("sAudioLink"));
                            if(useAudioLink.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                if(shaderSetting.LIL_FEATURE_TEX_AUDIOLINK_MASK)
                                {
                                    materialEditor.ShaderProperty(audioLinkUVMode, GetLoc("sAudioLinkUVMode") + "|" + GetLoc("sAudioLinkUVModeNone") + "|" + GetLoc("sAudioLinkUVModeRim") + "|" + GetLoc("sAudioLinkUVModeUV") + "|" + GetLoc("sAudioLinkUVModeMask"));
                                    if(audioLinkUVMode.floatValue == 3) materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sMask"), ""), audioLinkMask);
                                }
                                else
                                {
                                    materialEditor.ShaderProperty(audioLinkUVMode, GetLoc("sAudioLinkUVMode") + "|" + GetLoc("sAudioLinkUVModeNone") + "|" + GetLoc("sAudioLinkUVModeRim") + "|" + GetLoc("sAudioLinkUVModeUV"));
                                }
                                if(audioLinkUVMode.floatValue == 0) materialEditor.ShaderProperty(audioLinkUVParams, GetLoc("sOffset") + "|" + GetLoc("sAudioLinkBand") + "|" + GetLoc("sAudioLinkBandBass") + "|" + GetLoc("sAudioLinkBandLowMid") + "|" + GetLoc("sAudioLinkBandHighMid") + "|" + GetLoc("sAudioLinkBandTreble"));
                                if(audioLinkUVMode.floatValue == 1) materialEditor.ShaderProperty(audioLinkUVParams, GetLoc("sScale") + "|" + GetLoc("sOffset") + "|" + GetLoc("sAudioLinkBand") + "|" + GetLoc("sAudioLinkBandBass") + "|" + GetLoc("sAudioLinkBandLowMid") + "|" + GetLoc("sAudioLinkBandHighMid") + "|" + GetLoc("sAudioLinkBandTreble"));
                                if(audioLinkUVMode.floatValue == 2) materialEditor.ShaderProperty(audioLinkUVParams, GetLoc("sScale") + "|" + GetLoc("sOffset") + "|" + GetLoc("sAngle") + "|" + GetLoc("sAudioLinkBand") + "|" + GetLoc("sAudioLinkBandBass") + "|" + GetLoc("sAudioLinkBandLowMid") + "|" + GetLoc("sAudioLinkBandHighMid") + "|" + GetLoc("sAudioLinkBandTreble"));
                                GUILayout.Label(GetLoc("sAudioLinkApplyTo"), EditorStyles.boldLabel);
                                EditorGUI.indentLevel++;
                                if(shaderSetting.LIL_FEATURE_MAIN2ND) materialEditor.ShaderProperty(audioLink2Main2nd, GetLoc("sMainColor2nd"));
                                if(shaderSetting.LIL_FEATURE_MAIN3RD) materialEditor.ShaderProperty(audioLink2Main3rd, GetLoc("sMainColor3rd"));
                                if(shaderSetting.LIL_FEATURE_EMISSION_1ST) materialEditor.ShaderProperty(audioLink2Emission, GetLoc("sEmission"));
                                if(shaderSetting.LIL_FEATURE_EMISSION_2ND) materialEditor.ShaderProperty(audioLink2Emission2nd, GetLoc("sEmission2nd"));
                                if(shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX)
                                {
                                    materialEditor.ShaderProperty(audioLink2Vertex, GetLoc("sVertex"));
                                    if(audioLink2Vertex.floatValue == 1)
                                    {
                                        EditorGUI.indentLevel++;
                                        if(shaderSetting.LIL_FEATURE_TEX_AUDIOLINK_MASK)
                                        {
                                            materialEditor.ShaderProperty(audioLinkVertexUVMode, GetLoc("sAudioLinkUVMode") + "|" + GetLoc("sAudioLinkUVModeNone") + "|" + GetLoc("sAudioLinkUVModePosition") + "|" + GetLoc("sAudioLinkUVModeUV") + "|" + GetLoc("sAudioLinkUVModeMask"));
                                            if(audioLinkVertexUVMode.floatValue == 3) materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sMask"), ""), audioLinkMask);
                                        }
                                        else
                                        {
                                            materialEditor.ShaderProperty(audioLinkVertexUVMode, GetLoc("sAudioLinkUVMode") + "|" + GetLoc("sAudioLinkUVModeNone") + "|" + GetLoc("sAudioLinkUVModePosition") + "|" + GetLoc("sAudioLinkUVModeUV"));
                                        }
                                        if(audioLinkVertexUVMode.floatValue == 0) materialEditor.ShaderProperty(audioLinkVertexUVParams, GetLoc("sOffset") + "|" + GetLoc("sAudioLinkBand") + "|" + GetLoc("sAudioLinkBandBass") + "|" + GetLoc("sAudioLinkBandLowMid") + "|" + GetLoc("sAudioLinkBandHighMid") + "|" + GetLoc("sAudioLinkBandTreble"));
                                        if(audioLinkVertexUVMode.floatValue == 1) materialEditor.ShaderProperty(audioLinkVertexUVParams, GetLoc("sScale") + "|" + GetLoc("sOffset") + "|" + GetLoc("sAudioLinkBand") + "|" + GetLoc("sAudioLinkBandBass") + "|" + GetLoc("sAudioLinkBandLowMid") + "|" + GetLoc("sAudioLinkBandHighMid") + "|" + GetLoc("sAudioLinkBandTreble"));
                                        if(audioLinkVertexUVMode.floatValue == 2) materialEditor.ShaderProperty(audioLinkVertexUVParams, GetLoc("sScale") + "|" + GetLoc("sOffset") + "|" + GetLoc("sAngle") + "|" + GetLoc("sAudioLinkBand") + "|" + GetLoc("sAudioLinkBandBass") + "|" + GetLoc("sAudioLinkBandLowMid") + "|" + GetLoc("sAudioLinkBandHighMid") + "|" + GetLoc("sAudioLinkBandTreble"));
                                        if(audioLinkVertexUVMode.floatValue == 1) materialEditor.ShaderProperty(audioLinkVertexStart, GetLoc("sAudioLinkStartPosition"));
                                        materialEditor.ShaderProperty(audioLinkVertexStrength, GetLoc("sAudioLinkMovingVector") + "|" + GetLoc("sAudioLinkNormalStrength"));
                                        EditorGUI.indentLevel--;
                                    }
                                }
                                EditorGUI.indentLevel--;
                                if(shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL)
                                {
                                    DrawLine();
                                    materialEditor.ShaderProperty(audioLinkAsLocal, GetLoc("sAudioLinkAsLocal"));
                                    if(audioLinkAsLocal.floatValue == 1)
                                    {
                                        materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sAudioLinkLocalMap"), ""), audioLinkLocalMap);
                                        materialEditor.ShaderProperty(audioLinkLocalMapParams, GetLoc("sAudioLinkLocalMapBPM") + "|" + GetLoc("sAudioLinkLocalMapNotes") + "|" + GetLoc("sOffset"));
                                    }
                                }
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Encryption
                    if(shaderSetting.LIL_FEATURE_ENCRYPTION)
                    {
                        edSet.isShowEncryption = Foldout(GetLoc("sEncryption"), GetLoc("sEncryptionTips"), edSet.isShowEncryption);
                        DrawHelpButton(GetLoc("sAnchorEncryption"));
                        if(edSet.isShowEncryption)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField(GetLoc("sEncryption"), customToggleFont);
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            materialEditor.ShaderProperty(ignoreEncryption, GetLoc("sIgnoreEncryption"));
                            materialEditor.ShaderProperty(keys, GetLoc("sKeys"));
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Stencil
                    edSet.isShowStencil = Foldout(GetLoc("sStencilSetting"), GetLoc("sStencilTips"), edSet.isShowStencil);
                    DrawHelpButton(GetLoc("sAnchorStencil"));
                    if(edSet.isShowStencil)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sStencilSetting"), customToggleFont);
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
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, isOutl, isLite, isStWr, isTess);
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
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, isOutl, isLite, isStWr, isTess);
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
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, isOutl, isLite, isStWr, isTess);
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Base
                        {
                            DrawLine();
                            materialEditor.ShaderProperty(stencilRef, "Ref");
                            materialEditor.ShaderProperty(stencilReadMask, "ReadMask");
                            materialEditor.ShaderProperty(stencilWriteMask, "WriteMask");
                            materialEditor.ShaderProperty(stencilComp, "Comp");
                            materialEditor.ShaderProperty(stencilPass, "Pass");
                            materialEditor.ShaderProperty(stencilFail, "Fail");
                            materialEditor.ShaderProperty(stencilZFail, "ZFail");
                        }

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Rendering
                    edSet.isShowRendering = Foldout(GetLoc("sRenderingSetting"), GetLoc("sRenderingTips"), edSet.isShowRendering);
                    DrawHelpButton(GetLoc("sAnchorRendering"));
                    if(edSet.isShowRendering)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Reset Button
                        if(GUILayout.Button(GetLoc("sRenderingReset"), offsetButton))
                        {
                            material.enableInstancing = false;
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, isOutl, isLite, isStWr, isTess);
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Base
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField(GetLoc("sRenderingSetting"), customToggleFont);
                            EditorGUILayout.BeginVertical(boxInner);
                            materialEditor.ShaderProperty(cull, sCullModes);
                            materialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                            materialEditor.ShaderProperty(ztest, GetLoc("sZTest"));
                            materialEditor.ShaderProperty(offsetFactor, GetLoc("sOffsetFactor"));
                            materialEditor.ShaderProperty(offsetUnits, GetLoc("sOffsetUnits"));
                            materialEditor.ShaderProperty(colorMask, GetLoc("sColorMask"));
                            DrawLine();
                            BlendSettingGUI(materialEditor, ref edSet.isShowBlend, GetLoc("sForward"), srcBlend, dstBlend, srcBlendAlpha, dstBlendAlpha, blendOp, blendOpAlpha);
                            DrawLine();
                            materialEditor.EnableInstancingField();
                            materialEditor.DoubleSidedGIField();
                            materialEditor.RenderQueueField();
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Optimization
                    GUILayout.Label(" " + GetLoc("sOptimization"), EditorStyles.boldLabel);
                    edSet.isShowOptimization = Foldout(GetLoc("sOptimization"), GetLoc("sOptimizationTips"), edSet.isShowOptimization);
                    DrawHelpButton(GetLoc("sAnchorOptimization"));
                    if(edSet.isShowOptimization)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sOptimization"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        if(GUILayout.Button(GetLoc("sRemoveUnused"))) RemoveUnusedTexture(material, isLite, isFur, shaderSetting);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Shader Setting
                    edSet.isShowShaderSetting = Foldout(GetLoc("sShaderSetting"), GetLoc("sShaderSettingTips"), edSet.isShowShaderSetting);
                    DrawHelpButton(GetLoc("sAnchorShaderSetting"));
                    if(edSet.isShowShaderSetting)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sShaderSetting"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        EditorGUILayout.HelpBox(GetLoc("sHelpShaderSetting"),MessageType.Info);
                        ShaderSettingGUI();
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }
                else if(isFakeShadow)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // Base Setting
                    GUILayout.Label(" " + GetLoc("sBaseSetting"), EditorStyles.boldLabel);
                    DrawHelpButton(GetLoc("sAnchorBaseSetting"));
                    EditorGUILayout.BeginVertical(customBox);
                    {
                        materialEditor.ShaderProperty(invisible, GetLoc("sInvisible"));
                        materialEditor.ShaderProperty(cull, sCullModes);
                        materialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                        GUILayout.Label("FakeShadow", EditorStyles.boldLabel);
                        materialEditor.ShaderProperty(fakeShadowVector, GetLoc("sVector") + "|" + GetLoc("sOffset"));
                    }
                    EditorGUILayout.EndVertical();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // UV
                    edSet.isShowMainUV = Foldout(GetLoc("sMainUV"), GetLoc("sMainUVTips"), edSet.isShowMainUV);
                    DrawHelpButton(GetLoc("sAnchorUVSetting"));
                    if(edSet.isShowMainUV)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainUV"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TextureScaleOffsetProperty(mainTex);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.Space();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Colors
                    GUILayout.Label(" " + GetLoc("sColors"), EditorStyles.boldLabel);

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Main Color
                    edSet.isShowMain = Foldout(GetLoc("sMainColorSetting"), GetLoc("sMainColorTips"), edSet.isShowMain);
                    DrawHelpButton(GetLoc("sAnchorMainColor"));
                    if(edSet.isShowMain)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Main
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainColor"), customToggleFont);
                        DrawHelpButton(GetLoc("sAnchorMainColor1"));
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TexturePropertySingleLine(textureRGBAContent, mainTex, mainColor);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.Space();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Advanced
                    GUILayout.Label(" " + GetLoc("sAdvanced"), EditorStyles.boldLabel);

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Encryption
                    if(shaderSetting.LIL_FEATURE_ENCRYPTION)
                    {
                        edSet.isShowEncryption = Foldout(GetLoc("sEncryption"), GetLoc("sEncryptionTips"), edSet.isShowEncryption);
                        DrawHelpButton(GetLoc("sAnchorEncryption"));
                        if(edSet.isShowEncryption)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField(GetLoc("sEncryption"), customToggleFont);
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            materialEditor.ShaderProperty(ignoreEncryption, GetLoc("sIgnoreEncryption"));
                            materialEditor.ShaderProperty(keys, GetLoc("sKeys"));
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Stencil
                    edSet.isShowStencil = Foldout(GetLoc("sStencilSetting"), GetLoc("sStencilTips"), edSet.isShowStencil);
                    DrawHelpButton(GetLoc("sAnchorStencil"));
                    if(edSet.isShowStencil)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sStencilSetting"), customToggleFont);
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
                            materialEditor.ShaderProperty(stencilRef, "Ref");
                            materialEditor.ShaderProperty(stencilReadMask, "ReadMask");
                            materialEditor.ShaderProperty(stencilWriteMask, "WriteMask");
                            materialEditor.ShaderProperty(stencilComp, "Comp");
                            materialEditor.ShaderProperty(stencilPass, "Pass");
                            materialEditor.ShaderProperty(stencilFail, "Fail");
                            materialEditor.ShaderProperty(stencilZFail, "ZFail");
                        }

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Rendering
                    edSet.isShowRendering = Foldout(GetLoc("sRenderingSetting"), GetLoc("sRenderingTips"), edSet.isShowRendering);
                    DrawHelpButton(GetLoc("sAnchorRendering"));
                    if(edSet.isShowRendering)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Reset Button
                        if(GUILayout.Button(GetLoc("sRenderingReset"), offsetButton))
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
                            materialEditor.ShaderProperty(cull, sCullModes);
                            materialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                            materialEditor.ShaderProperty(ztest, GetLoc("sZTest"));
                            materialEditor.ShaderProperty(offsetFactor, GetLoc("sOffsetFactor"));
                            materialEditor.ShaderProperty(offsetUnits, GetLoc("sOffsetUnits"));
                            materialEditor.ShaderProperty(colorMask, GetLoc("sColorMask"));
                            DrawLine();
                            BlendSettingGUI(materialEditor, ref edSet.isShowBlend, GetLoc("sForward"), srcBlend, dstBlend, srcBlendAlpha, dstBlendAlpha, blendOp, blendOpAlpha);
                            DrawLine();
                            materialEditor.EnableInstancingField();
                            materialEditor.DoubleSidedGIField();
                            materialEditor.RenderQueueField();
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Optimization
                    GUILayout.Label(" " + GetLoc("sOptimization"), EditorStyles.boldLabel);
                    edSet.isShowOptimization = Foldout(GetLoc("sOptimization"), GetLoc("sOptimizationTips"), edSet.isShowOptimization);
                    DrawHelpButton(GetLoc("sAnchorOptimization"));
                    if(edSet.isShowOptimization)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sOptimization"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        if(GUILayout.Button(GetLoc("sRemoveUnused"))) RemoveUnusedTexture(material, isLite, isFur, shaderSetting);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }
                else
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // Base Setting
                    GUILayout.Label(" " + GetLoc("sBaseSetting"), EditorStyles.boldLabel);
                    DrawHelpButton(GetLoc("sAnchorBaseSetting"));
                    EditorGUILayout.BeginVertical(customBox);
                    {
                        materialEditor.ShaderProperty(invisible, GetLoc("sInvisible"));
                        if(!isCustomShader)
                        {
                            RenderingMode renderingMode;
                            renderingMode = (RenderingMode)EditorGUILayout.Popup(GetLoc("sRenderingMode"), (int)renderingModeBuf, sRenderingModeList);
                            if(renderingModeBuf != renderingMode)
                            {
                                SetupMaterialWithRenderingMode(material, renderingMode, isOutl, isLite, isStWr, isTess);
                                if(renderingMode == RenderingMode.Cutout || renderingMode == RenderingMode.FurCutout) cutoff.floatValue = 0.5f;
                                if(renderingMode == RenderingMode.Transparent || renderingMode == RenderingMode.Fur) cutoff.floatValue = 0.001f;
                            }
                        }
                            EditorGUI.indentLevel++;
                            if(renderingModeBuf == RenderingMode.Cutout || renderingModeBuf == RenderingMode.Transparent || renderingModeBuf == RenderingMode.Fur || renderingModeBuf == RenderingMode.FurCutout)
                            {
                                materialEditor.ShaderProperty(cutoff, GetLoc("sCutoff"));
                            }
                            if(renderingModeBuf >= RenderingMode.Transparent && renderingModeBuf != RenderingMode.FurCutout)
                            {
                                EditorGUILayout.HelpBox(GetLoc("sHelpRenderingTransparent"),MessageType.Warning);
                            }
                            EditorGUI.indentLevel--;
                        materialEditor.ShaderProperty(cull, sCullModes);
                            EditorGUI.indentLevel++;
                            if(cull.floatValue == 1.0f && AutoFixHelpBox(GetLoc("sHelpCullMode")))
                            {
                                cull.floatValue = 2.0f;
                            }
                            if(cull.floatValue <= 1.0f)
                            {
                                materialEditor.ShaderProperty(flipNormal, GetLoc("sFlipBackfaceNormal"));
                                materialEditor.ShaderProperty(backfaceForceShadow, GetLoc("sBackfaceForceShadow"));
                            }
                            EditorGUI.indentLevel--;
                        materialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                            EditorGUI.indentLevel++;
                            if(zwrite.floatValue != 1.0f && AutoFixHelpBox(GetLoc("sHelpZWrite")))
                            {
                                zwrite.floatValue = 1.0f;
                            }
                            EditorGUI.indentLevel--;
                        DrawLine();
                        materialEditor.ShaderProperty(asUnlit, GetLoc("sAsUnlit"));
                        materialEditor.ShaderProperty(vertexLightStrength, GetLoc("sVertexLightStrength"));
                        materialEditor.ShaderProperty(lightMinLimit, GetLoc("sLightMinLimit"));
                    }
                    EditorGUILayout.EndVertical();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // UV
                    edSet.isShowMainUV = Foldout(GetLoc("sMainUV"), GetLoc("sMainUVTips"), edSet.isShowMainUV);
                    DrawHelpButton(GetLoc("sAnchorUVSetting"));
                    if(edSet.isShowMainUV)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainUV"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        if(shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV)   UVSettingGUI(materialEditor, mainTex, mainTex_ScrollRotate);
                        else                                            materialEditor.TextureScaleOffsetProperty(mainTex);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.Space();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Colors
                    GUILayout.Label(" " + GetLoc("sColors"), EditorStyles.boldLabel);

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Main Color
                    edSet.isShowMain = Foldout(GetLoc("sMainColorSetting"), GetLoc("sMainColorTips"), edSet.isShowMain);
                    DrawHelpButton(GetLoc("sAnchorMainColor"));
                    if(edSet.isShowMain)
                    {
                        if(isFur)
                        {
                            //------------------------------------------------------------------------------------------------------------------------------
                            // Main
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField(GetLoc("sMainColor"), customToggleFont);
                            DrawHelpButton(GetLoc("sAnchorMainColor1"));
                            //materialEditor.ShaderProperty(useMainTex, GetLoc("sMainColor"));
                            //if(useMainTex.floatValue == 1)
                            //{
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                materialEditor.TexturePropertySingleLine(textureRGBAContent, mainTex, mainColor);
                                EditorGUILayout.EndVertical();
                            //}
                            EditorGUILayout.EndVertical();
                        }
                        else
                        {
                            //------------------------------------------------------------------------------------------------------------------------------
                            // Main
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField(GetLoc("sMainColor"), customToggleFont);
                            DrawHelpButton(GetLoc("sAnchorMainColor1"));
                            //materialEditor.ShaderProperty(useMainTex, GetLoc("sMainColor"));
                            //if(useMainTex.floatValue == 1)
                            //{
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                materialEditor.TexturePropertySingleLine(textureRGBAContent, mainTex, mainColor);
                                if(isCutout || isTransparent)
                                {
                                    SetAlphaIsTransparencyGUI(mainTex);
                                }
                                if(shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION)
                                {
                                    ToneCorrectionGUI(materialEditor, material, mainTex, mainColor, mainTexHSVG);
                                }
                                if(shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP)
                                {
                                    DrawLine();
                                    EditorGUILayout.LabelField(GetLoc("sGradationMap"), EditorStyles.boldLabel);
                                    materialEditor.ShaderProperty(mainGradationStrength, GetLoc("sStrength"));
                                    if(mainGradationStrength.floatValue != 0)
                                    {
                                        materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sGradation"), GetLoc("sTextureRGBA")), mainGradationTex);
                                        GradientEditor(material, mainGrad, mainGradationTex, true);
                                    }
                                    DrawLine();
                                }
                                if(shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION || shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP)
                                {
                                    materialEditor.TexturePropertySingleLine(adjustMaskContent, mainColorAdjustMask);
                                    TextureBakeGUI(material, 4);
                                }
                                EditorGUILayout.EndVertical();
                            //}
                            EditorGUILayout.EndVertical();

                            //------------------------------------------------------------------------------------------------------------------------------
                            // 2nd
                            if(shaderSetting.LIL_FEATURE_MAIN2ND)
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                materialEditor.ShaderProperty(useMain2ndTex, GetLoc("sMainColor2nd"));
                                DrawHelpButton(GetLoc("sAnchorMainColor2"));
                                if(useMain2ndTex.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    materialEditor.TexturePropertySingleLine(textureRGBAContent, main2ndTex, mainColor2nd);
                                    materialEditor.ShaderProperty(main2ndTexIsMSDF, GetLoc("sAsMSDF"));
                                    DrawLine();
                                    UV4Decal(materialEditor, main2ndTexIsDecal, main2ndTexIsLeftOnly, main2ndTexIsRightOnly, main2ndTexShouldCopy, main2ndTexShouldFlipMirror, main2ndTexShouldFlipCopy, main2ndTex, main2ndTexAngle, main2ndTexDecalAnimation, main2ndTexDecalSubParam);
                                    DrawLine();
                                    if(shaderSetting.LIL_FEATURE_TEX_LAYER_MASK) materialEditor.TexturePropertySingleLine(maskBlendContent, main2ndBlendMask);
                                    EditorGUILayout.LabelField(GetLoc("sDistanceFade"));
                                    materialEditor.ShaderProperty(main2ndDistanceFade, sDistanceFadeSetting);
                                    materialEditor.ShaderProperty(main2ndEnableLighting, GetLoc("sEnableLighting"));
                                    materialEditor.ShaderProperty(main2ndTexBlendMode, sBlendModes);
                                    if(shaderSetting.LIL_FEATURE_LAYER_DISSOLVE)
                                    {
                                        DrawLine();
                                        materialEditor.ShaderProperty(main2ndDissolveParams, sDissolveParams);
                                        if(main2ndDissolveParams.vectorValue.x == 1.0f)                                                TextureGUI(materialEditor, ref edSet.isShowMain2ndDissolveMask, maskBlendContent, main2ndDissolveMask);
                                        if(main2ndDissolveParams.vectorValue.x == 2.0f && main2ndDissolveParams.vectorValue.y == 0.0f) materialEditor.ShaderProperty(main2ndDissolvePos, GetLoc("sPosition") + "|2");
                                        if(main2ndDissolveParams.vectorValue.x == 2.0f && main2ndDissolveParams.vectorValue.y == 1.0f) materialEditor.ShaderProperty(main2ndDissolvePos, GetLoc("sVector") + "|2");
                                        if(main2ndDissolveParams.vectorValue.x == 3.0f && main2ndDissolveParams.vectorValue.y == 0.0f) materialEditor.ShaderProperty(main2ndDissolvePos, GetLoc("sPosition") + "|3");
                                        if(main2ndDissolveParams.vectorValue.x == 3.0f && main2ndDissolveParams.vectorValue.y == 1.0f) materialEditor.ShaderProperty(main2ndDissolvePos, GetLoc("sVector") + "|3");
                                        if(main2ndDissolveParams.vectorValue.x != 0.0f)
                                        {
                                            if(shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE) TextureGUI(materialEditor, ref edSet.isShowMain2ndDissolveNoiseMask, noiseMaskContent, main2ndDissolveNoiseMask, main2ndDissolveNoiseStrength, main2ndDissolveNoiseMask_ScrollRotate);
                                            materialEditor.ShaderProperty(main2ndDissolveColor, GetLoc("sColor"));
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
                            if(shaderSetting.LIL_FEATURE_MAIN3RD)
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                materialEditor.ShaderProperty(useMain3rdTex, GetLoc("sMainColor3rd"));
                                DrawHelpButton(GetLoc("sAnchorMainColor2"));
                                if(useMain3rdTex.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    materialEditor.TexturePropertySingleLine(textureRGBAContent, main3rdTex, mainColor3rd);
                                    materialEditor.ShaderProperty(main3rdTexIsMSDF, GetLoc("sAsMSDF"));
                                    DrawLine();
                                    UV4Decal(materialEditor, main3rdTexIsDecal, main3rdTexIsLeftOnly, main3rdTexIsRightOnly, main3rdTexShouldCopy, main3rdTexShouldFlipMirror, main3rdTexShouldFlipCopy, main3rdTex, main3rdTexAngle, main3rdTexDecalAnimation, main3rdTexDecalSubParam);
                                    DrawLine();
                                    if(shaderSetting.LIL_FEATURE_TEX_LAYER_MASK) materialEditor.TexturePropertySingleLine(maskBlendContent, main3rdBlendMask);
                                    EditorGUILayout.LabelField(GetLoc("sDistanceFade"));
                                    materialEditor.ShaderProperty(main3rdDistanceFade, sDistanceFadeSetting);
                                    materialEditor.ShaderProperty(main3rdEnableLighting, GetLoc("sEnableLighting"));
                                    materialEditor.ShaderProperty(main3rdTexBlendMode, sBlendModes);
                                    if(shaderSetting.LIL_FEATURE_LAYER_DISSOLVE)
                                    {
                                        DrawLine();
                                        materialEditor.ShaderProperty(main3rdDissolveParams, sDissolveParams);
                                        if(main3rdDissolveParams.vectorValue.x == 1.0f)                                                TextureGUI(materialEditor, ref edSet.isShowMain3rdDissolveMask, maskBlendContent, main3rdDissolveMask);
                                        if(main3rdDissolveParams.vectorValue.x == 2.0f && main3rdDissolveParams.vectorValue.y == 0.0f) materialEditor.ShaderProperty(main3rdDissolvePos, GetLoc("sPosition") + "|2");
                                        if(main3rdDissolveParams.vectorValue.x == 2.0f && main3rdDissolveParams.vectorValue.y == 1.0f) materialEditor.ShaderProperty(main3rdDissolvePos, GetLoc("sVector") + "|2");
                                        if(main3rdDissolveParams.vectorValue.x == 3.0f && main3rdDissolveParams.vectorValue.y == 0.0f) materialEditor.ShaderProperty(main3rdDissolvePos, GetLoc("sPosition") + "|3");
                                        if(main3rdDissolveParams.vectorValue.x == 3.0f && main3rdDissolveParams.vectorValue.y == 1.0f) materialEditor.ShaderProperty(main3rdDissolvePos, GetLoc("sVector") + "|3");
                                        if(main3rdDissolveParams.vectorValue.x != 0.0f)
                                        {
                                            if(shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE) TextureGUI(materialEditor, ref edSet.isShowMain3rdDissolveNoiseMask, noiseMaskContent, main3rdDissolveNoiseMask, main3rdDissolveNoiseStrength, main3rdDissolveNoiseMask_ScrollRotate);
                                            materialEditor.ShaderProperty(main3rdDissolveColor, GetLoc("sColor"));
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
                            if(shaderSetting.LIL_FEATURE_ALPHAMASK)
                            {
                                if(renderingModeBuf == RenderingMode.Opaque)
                                {
                                    GUILayout.Label(GetLoc("sAlphaMaskWarnOpaque"), wrapLabel);
                                }
                                else
                                {
                                    EditorGUILayout.BeginVertical(boxOuter);
                                    materialEditor.ShaderProperty(alphaMaskMode, sAlphaMaskModes);
                                    if(alphaMaskMode.floatValue != 0)
                                    {
                                        EditorGUILayout.BeginVertical(boxInnerHalf);
                                        materialEditor.TexturePropertySingleLine(alphaMaskContent, alphaMask, alphaMaskValue);
                                        materialEditor.ShaderProperty(cutoff, GetLoc("sCutoff"));
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
                    if(shaderSetting.LIL_FEATURE_SHADOW)
                    {
                        edSet.isShowShadow = Foldout(GetLoc("sShadowSetting"), GetLoc("sShadowTips"), edSet.isShowShadow);
                        DrawHelpButton(GetLoc("sAnchorShadow"));
                        if(edSet.isShowShadow)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            materialEditor.ShaderProperty(useShadow, GetLoc("sShadow"));
                            if(useShadow.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                if(shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER) materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sBorderAO"), GetLoc("sBorderR")), shadowBorderMask, shadowBorder);
                                else                                            materialEditor.ShaderProperty(shadowBorder, GetLoc("sBorder"));
                                if(shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR)   materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sBlur"), GetLoc("sBlurR")), shadowBlurMask, shadowBlur);
                                else                                            materialEditor.ShaderProperty(shadowBlur, GetLoc("sBlur"));
                                if(shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH)   materialEditor.TexturePropertySingleLine(maskStrengthContent, shadowStrengthMask, shadowStrength);
                                else                                                materialEditor.ShaderProperty(shadowStrength, GetLoc("sStrength"));
                                DrawLine();
                                if(shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST)    materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sShadow1stColor"), GetLoc("sTextureRGBA")), shadowColorTex, shadowColor);
                                else                                            materialEditor.ShaderProperty(shadowColor, GetLoc("sShadow1stColor"));
                                DrawLine();
                                if(shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND)    materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sShadow2ndColor"), GetLoc("sTextureRGBA")), shadow2ndColorTex, shadow2ndColor);
                                else                                            materialEditor.ShaderProperty(shadow2ndColor, GetLoc("sShadow2ndColor"));
                                if(shadow2ndColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                {
                                    shadow2ndColor.colorValue = new Color(shadow2ndColor.colorValue.r, shadow2ndColor.colorValue.g, shadow2ndColor.colorValue.b, 1.0f);
                                }
                                materialEditor.ShaderProperty(shadow2ndBorder, GetLoc("sBorder"));
                                materialEditor.ShaderProperty(shadow2ndBlur, GetLoc("sBlur"));
                                DrawLine();
                                materialEditor.ShaderProperty(shadowMainStrength, GetLoc("sMainColorPower"));
                                materialEditor.ShaderProperty(shadowEnvStrength, GetLoc("sShadowEnvStrength"));
                                materialEditor.ShaderProperty(shadowBorderColor, GetLoc("sShadowBorderColor"));
                                materialEditor.ShaderProperty(shadowBorderRange, GetLoc("sShadowBorderRange"));
                                if(shaderSetting.LIL_FEATURE_RECEIVE_SHADOW) materialEditor.ShaderProperty(shadowReceive, GetLoc("sReceiveShadow"));
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Emission
                    if(!isFur && (shaderSetting.LIL_FEATURE_EMISSION_1ST || shaderSetting.LIL_FEATURE_EMISSION_2ND))
                    {
                        edSet.isShowEmission = Foldout(GetLoc("sEmissionSetting"), GetLoc("sEmissionTips"), edSet.isShowEmission);
                        DrawHelpButton(GetLoc("sAnchorEmission"));
                        if(edSet.isShowEmission)
                        {
                            // Emission
                            if(shaderSetting.LIL_FEATURE_EMISSION_1ST)
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                materialEditor.ShaderProperty(useEmission, GetLoc("sEmission"));
                                if(useEmission.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    TextureGUI(materialEditor, ref edSet.isShowEmissionMap, colorRGBAContent, emissionMap, emissionColor, emissionMap_ScrollRotate, shaderSetting.LIL_FEATURE_EMISSION_UV, shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV);
                                    if(emissionColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                    {
                                        emissionColor.colorValue = new Color(emissionColor.colorValue.r, emissionColor.colorValue.g, emissionColor.colorValue.b, 1.0f);
                                    }
                                    DrawLine();
                                    if(shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK)
                                    {
                                        TextureGUI(materialEditor, ref edSet.isShowEmissionBlendMask, maskBlendContent, emissionBlendMask, emissionBlend, emissionBlendMask_ScrollRotate, shaderSetting.LIL_FEATURE_EMISSION_MASK_UV, shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV);
                                        DrawLine();
                                    }
                                    materialEditor.ShaderProperty(emissionBlink, blinkSetting);
                                    DrawLine();
                                    if(shaderSetting.LIL_FEATURE_EMISSION_GRADATION)
                                    {
                                        materialEditor.ShaderProperty(emissionUseGrad, GetLoc("sGradation"));
                                        if(emissionUseGrad.floatValue == 1)
                                        {
                                            materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sGradTexSpeed"), GetLoc("sTextureRGBA")), emissionGradTex, emissionGradSpeed);
                                            GradientEditor(material, "_eg", emiGrad, emissionGradSpeed);
                                        }
                                        DrawLine();
                                    }
                                    materialEditor.ShaderProperty(emissionParallaxDepth, GetLoc("sParallaxDepth"));
                                    materialEditor.ShaderProperty(emissionFluorescence, GetLoc("sFluorescence"));
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndVertical();
                            }

                            // Emission 2nd
                            if(shaderSetting.LIL_FEATURE_EMISSION_2ND)
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                materialEditor.ShaderProperty(useEmission2nd, GetLoc("sEmission2nd"));
                                if(useEmission2nd.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    TextureGUI(materialEditor, ref edSet.isShowEmission2ndMap, colorRGBAContent, emission2ndMap, emission2ndColor, emission2ndMap_ScrollRotate, shaderSetting.LIL_FEATURE_EMISSION_UV, shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV);
                                    if(emission2ndColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                    {
                                        emission2ndColor.colorValue = new Color(emission2ndColor.colorValue.r, emission2ndColor.colorValue.g, emission2ndColor.colorValue.b, 1.0f);
                                    }
                                    DrawLine();
                                    if(shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK)
                                    {
                                        TextureGUI(materialEditor, ref edSet.isShowEmission2ndBlendMask, maskBlendContent, emission2ndBlendMask, emission2ndBlend, emission2ndBlendMask_ScrollRotate, shaderSetting.LIL_FEATURE_EMISSION_MASK_UV, shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV);
                                        DrawLine();
                                    }
                                    materialEditor.ShaderProperty(emission2ndBlink, blinkSetting);
                                    DrawLine();
                                    if(shaderSetting.LIL_FEATURE_EMISSION_GRADATION)
                                    {
                                        materialEditor.ShaderProperty(emission2ndUseGrad, GetLoc("sGradation"));
                                        if(emission2ndUseGrad.floatValue == 1)
                                        {
                                            materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sGradTexSpeed"), GetLoc("sTextureRGBA")), emission2ndGradTex, emission2ndGradSpeed);
                                            GradientEditor(material, "_e2g", emi2Grad, emission2ndGradSpeed);
                                        }
                                        DrawLine();
                                    }
                                    materialEditor.ShaderProperty(emission2ndParallaxDepth, GetLoc("sParallaxDepth"));
                                    materialEditor.ShaderProperty(emission2ndFluorescence, GetLoc("sFluorescence"));
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndVertical();
                            }
                        }

                        EditorGUILayout.Space();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Normal / Reflection
                    if(!isFur)
                    {
                        if(shaderSetting.LIL_FEATURE_NORMAL_1ST || shaderSetting.LIL_FEATURE_NORMAL_2ND || shaderSetting.LIL_FEATURE_REFLECTION || shaderSetting.LIL_FEATURE_MATCAP || shaderSetting.LIL_FEATURE_MATCAP_2ND || shaderSetting.LIL_FEATURE_RIMLIGHT)
                        {
                            GUILayout.Label(" " + GetLoc("sNormalMapReflection"), EditorStyles.boldLabel);
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Normal
                        if(shaderSetting.LIL_FEATURE_NORMAL_1ST || shaderSetting.LIL_FEATURE_NORMAL_2ND)
                        {
                            edSet.isShowBump = Foldout(GetLoc("sNormalMapSetting"), GetLoc("sNormalMapTips"), edSet.isShowBump);
                            DrawHelpButton(GetLoc("sAnchorNormalMap"));
                            if(edSet.isShowBump)
                            {
                                //------------------------------------------------------------------------------------------------------------------------------
                                // 1st
                                if(shaderSetting.LIL_FEATURE_NORMAL_1ST)
                                {
                                    EditorGUILayout.BeginVertical(boxOuter);
                                    materialEditor.ShaderProperty(useBumpMap, GetLoc("sNormalMap"));
                                    if(useBumpMap.floatValue == 1)
                                    {
                                        EditorGUILayout.BeginVertical(boxInnerHalf);
                                        materialEditor.TexturePropertySingleLine(normalMapContent, bumpMap, bumpScale);
                                        materialEditor.TextureScaleOffsetProperty(bumpMap);
                                        EditorGUILayout.EndVertical();
                                    }
                                    EditorGUILayout.EndVertical();
                                }

                                //------------------------------------------------------------------------------------------------------------------------------
                                // 2nd
                                if(shaderSetting.LIL_FEATURE_NORMAL_2ND)
                                {
                                    EditorGUILayout.BeginVertical(boxOuter);
                                    materialEditor.ShaderProperty(useBump2ndMap, GetLoc("sNormalMap2nd"));
                                    if(useBump2ndMap.floatValue == 1)
                                    {
                                        EditorGUILayout.BeginVertical(boxInnerHalf);
                                        materialEditor.TexturePropertySingleLine(normalMapContent, bump2ndMap, bump2ndScale);
                                        materialEditor.TextureScaleOffsetProperty(bump2ndMap);
                                        if(shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK) materialEditor.TexturePropertySingleLine(maskStrengthContent, bump2ndScaleMask);
                                        EditorGUILayout.EndVertical();
                                    }
                                    EditorGUILayout.EndVertical();
                                }
                            }
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Reflection
                        if(shaderSetting.LIL_FEATURE_REFLECTION || shaderSetting.LIL_FEATURE_MATCAP || shaderSetting.LIL_FEATURE_MATCAP_2ND || shaderSetting.LIL_FEATURE_RIMLIGHT || shaderSetting.LIL_FEATURE_GLITTER)
                        {
                            edSet.isShowReflections = Foldout(GetLoc("sReflectionsSetting"), GetLoc("sReflectionsTips"), edSet.isShowReflections);
                            DrawHelpButton(GetLoc("sAnchorReflections"));
                            if(edSet.isShowReflections)
                            {
                                //------------------------------------------------------------------------------------------------------------------------------
                                // Reflection
                                if(shaderSetting.LIL_FEATURE_REFLECTION)
                                {
                                    EditorGUILayout.BeginVertical(boxOuter);
                                    materialEditor.ShaderProperty(useReflection, GetLoc("sReflection"));
                                    DrawHelpButton(GetLoc("sAnchorReflection"));
                                    if(useReflection.floatValue == 1)
                                    {
                                        EditorGUILayout.BeginVertical(boxInnerHalf);
                                        if(shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS) materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sSmoothness"), GetLoc("sSmoothnessR")), smoothnessTex, smoothness);
                                        else                                                    materialEditor.ShaderProperty(smoothness, GetLoc("sSmoothness"));
                                        if(shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC) materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sMetallic"), GetLoc("sMetallicR")), metallicGlossMap, metallic);
                                        else                                                  materialEditor.ShaderProperty(metallic, GetLoc("sMetallic"));
                                        materialEditor.ShaderProperty(reflectance, GetLoc("sReflectance"));
                                        if(shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR) materialEditor.TexturePropertySingleLine(colorRGBAContent, reflectionColorTex, reflectionColor);
                                        else                                               materialEditor.ShaderProperty(reflectionColor, GetLoc("sColor"));
                                        if(reflectionColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                        {
                                            reflectionColor.colorValue = new Color(reflectionColor.colorValue.r, reflectionColor.colorValue.g, reflectionColor.colorValue.b, 1.0f);
                                        }
                                        int specularMode = 0;
                                        if(specularToon.floatValue == 0.0f) specularMode = 1;
                                        if(specularToon.floatValue == 1.0f) specularMode = 2;
                                        if(applySpecular.floatValue == 0.0f) specularMode = 0;
                                        specularMode = EditorGUILayout.Popup(GetLoc("sSpecularMode"),specularMode,new String[]{GetLoc("sSpecularNone"),GetLoc("sSpecularReal"),GetLoc("sSpecularToon")});
                                        if(specularMode == 0) {applySpecular.floatValue = 0.0f;}
                                        if(specularMode == 1) {applySpecular.floatValue = 1.0f; specularToon.floatValue = 0.0f;}
                                        if(specularMode == 2) {applySpecular.floatValue = 1.0f; specularToon.floatValue = 1.0f;}
                                        materialEditor.ShaderProperty(applyReflection, GetLoc("sApplyReflection"));
                                        if(isTransparent) materialEditor.ShaderProperty(reflectionApplyTransparency, GetLoc("sApplyTransparency"));
                                        EditorGUILayout.EndVertical();
                                    }
                                    EditorGUILayout.EndVertical();
                                }

                                //------------------------------------------------------------------------------------------------------------------------------
                                // MatCap
                                if(shaderSetting.LIL_FEATURE_MATCAP)
                                {
                                    EditorGUILayout.BeginVertical(boxOuter);
                                    materialEditor.ShaderProperty(useMatCap, GetLoc("sMatCap"));
                                    DrawHelpButton(GetLoc("sAnchorMatCap"));
                                    if(useMatCap.floatValue == 1)
                                    {
                                        EditorGUILayout.BeginVertical(boxInnerHalf);
                                        materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sMatCap"), GetLoc("sTextureRGBA")), matcapTex, matcapColor);
                                        if(matcapColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                        {
                                            matcapColor.colorValue = new Color(matcapColor.colorValue.r, matcapColor.colorValue.g, matcapColor.colorValue.b, 1.0f);
                                        }
                                        if(shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK)   materialEditor.TexturePropertySingleLine(maskBlendContent, matcapBlendMask, matcapBlend);
                                        else                                            materialEditor.ShaderProperty(matcapBlend, GetLoc("sBlend"));
                                        materialEditor.ShaderProperty(matcapEnableLighting, GetLoc("sEnableLighting"));
                                        materialEditor.ShaderProperty(matcapBlendMode, sBlendModes);
                                        if(matcapEnableLighting.floatValue != 0.0f && matcapBlendMode.floatValue == 3.0f && AutoFixHelpBox(GetLoc("sHelpMatCapBlending")))
                                        {
                                            matcapEnableLighting.floatValue = 0.0f;
                                        }
                                        if(isTransparent) materialEditor.ShaderProperty(matcapApplyTransparency, GetLoc("sApplyTransparency"));
                                        materialEditor.ShaderProperty(matcapZRotCancel, GetLoc("sMatCapZRotCancel"));
                                        if(shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP)
                                        {
                                            DrawLine();
                                            materialEditor.ShaderProperty(matcapCustomNormal, GetLoc("sMatCapCustomNormal"));
                                            if(matcapCustomNormal.floatValue == 1)
                                            {
                                                materialEditor.TexturePropertySingleLine(normalMapContent, matcapBumpMap, matcapBumpScale);
                                                materialEditor.TextureScaleOffsetProperty(matcapBumpMap);
                                            }
                                        }
                                        EditorGUILayout.EndVertical();
                                    }
                                    EditorGUILayout.EndVertical();
                                }

                                //------------------------------------------------------------------------------------------------------------------------------
                                // MatCap 2nd
                                if(shaderSetting.LIL_FEATURE_MATCAP_2ND)
                                {
                                    EditorGUILayout.BeginVertical(boxOuter);
                                    materialEditor.ShaderProperty(useMatCap2nd, GetLoc("sMatCap2nd"));
                                    DrawHelpButton(GetLoc("sAnchorMatCap"));
                                    if(useMatCap2nd.floatValue == 1)
                                    {
                                        EditorGUILayout.BeginVertical(boxInnerHalf);
                                        materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sMatCap"), GetLoc("sTextureRGBA")), matcap2ndTex, matcap2ndColor);
                                        if(matcap2ndColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                        {
                                            matcap2ndColor.colorValue = new Color(matcap2ndColor.colorValue.r, matcap2ndColor.colorValue.g, matcap2ndColor.colorValue.b, 1.0f);
                                        }
                                        if(shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK)   materialEditor.TexturePropertySingleLine(maskBlendContent, matcap2ndBlendMask, matcap2ndBlend);
                                        else                                            materialEditor.ShaderProperty(matcap2ndBlend, GetLoc("sBlend"));
                                        materialEditor.ShaderProperty(matcap2ndEnableLighting, GetLoc("sEnableLighting"));
                                        materialEditor.ShaderProperty(matcap2ndBlendMode, sBlendModes);
                                        if(matcap2ndEnableLighting.floatValue != 0.0f && matcap2ndBlendMode.floatValue == 3.0f && AutoFixHelpBox(GetLoc("sHelpMatCapBlending")))
                                        {
                                            matcap2ndEnableLighting.floatValue = 0.0f;
                                        }
                                        if(isTransparent) materialEditor.ShaderProperty(matcap2ndApplyTransparency, GetLoc("sApplyTransparency"));
                                        materialEditor.ShaderProperty(matcap2ndZRotCancel, GetLoc("sMatCapZRotCancel"));
                                        if(shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP)
                                        {
                                            DrawLine();
                                            materialEditor.ShaderProperty(matcap2ndCustomNormal, GetLoc("sMatCapCustomNormal"));
                                            if(matcap2ndCustomNormal.floatValue == 1)
                                            {
                                                materialEditor.TexturePropertySingleLine(normalMapContent, matcap2ndBumpMap, matcap2ndBumpScale);
                                                materialEditor.TextureScaleOffsetProperty(matcap2ndBumpMap);
                                            }
                                        }
                                        EditorGUILayout.EndVertical();
                                    }
                                    EditorGUILayout.EndVertical();
                                }

                                //------------------------------------------------------------------------------------------------------------------------------
                                // Rim
                                if(shaderSetting.LIL_FEATURE_RIMLIGHT)
                                {
                                    EditorGUILayout.BeginVertical(boxOuter);
                                    materialEditor.ShaderProperty(useRim, GetLoc("sRimLight"));
                                    DrawHelpButton(GetLoc("sAnchorRimLight"));
                                    if(useRim.floatValue == 1)
                                    {
                                        EditorGUILayout.BeginVertical(boxInnerHalf);
                                        if(shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR)    materialEditor.TexturePropertySingleLine(colorRGBAContent, rimColorTex, rimColor);
                                        else                                                materialEditor.ShaderProperty(rimColor, GetLoc("sColor"));
                                        if(rimColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                        {
                                            rimColor.colorValue = new Color(rimColor.colorValue.r, rimColor.colorValue.g, rimColor.colorValue.b, 1.0f);
                                        }
                                        materialEditor.ShaderProperty(rimEnableLighting, GetLoc("sEnableLighting"));
                                        materialEditor.ShaderProperty(rimShadowMask, GetLoc("sShadowMask"));
                                        if(isTransparent) materialEditor.ShaderProperty(rimApplyTransparency, GetLoc("sApplyTransparency"));
                                        DrawLine();
                                        if(shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION)
                                        {
                                            materialEditor.ShaderProperty(rimDirStrength, GetLoc("sRimLightDirection"));
                                            if(rimDirStrength.floatValue != 0)
                                            {
                                                EditorGUI.indentLevel++;
                                                materialEditor.ShaderProperty(rimDirRange, GetLoc("sRimDirectionRange"));
                                                rimBorder.floatValue = 1.0f - EditorGUILayout.Slider(GetLoc("sBorder"), 1.0f - rimBorder.floatValue, 0.0f, 1.0f);
                                                materialEditor.ShaderProperty(rimBlur, GetLoc("sBlur"));
                                                DrawLine();
                                                materialEditor.ShaderProperty(rimIndirRange, GetLoc("sRimIndirectionRange"));
                                                materialEditor.ShaderProperty(rimIndirColor, GetLoc("sColor"));
                                                rimIndirBorder.floatValue = 1.0f - EditorGUILayout.Slider(GetLoc("sBorder"), 1.0f - rimIndirBorder.floatValue, 0.0f, 1.0f);
                                                materialEditor.ShaderProperty(rimIndirBlur, GetLoc("sBlur"));
                                                EditorGUI.indentLevel--;
                                                DrawLine();
                                                materialEditor.ShaderProperty(rimFresnelPower, GetLoc("sFresnelPower"));
                                            }
                                            else
                                            {
                                                rimBorder.floatValue = 1.0f - EditorGUILayout.Slider(GetLoc("sBorder"), 1.0f - rimBorder.floatValue, 0.0f, 1.0f);
                                                materialEditor.ShaderProperty(rimBlur, GetLoc("sBlur"));
                                                materialEditor.ShaderProperty(rimFresnelPower, GetLoc("sFresnelPower"));
                                            }
                                        }
                                        else
                                        {
                                            rimBorder.floatValue = 1.0f - EditorGUILayout.Slider(GetLoc("sBorder"), 1.0f - rimBorder.floatValue, 0.0f, 1.0f);
                                            materialEditor.ShaderProperty(rimBlur, GetLoc("sBlur"));
                                            materialEditor.ShaderProperty(rimFresnelPower, GetLoc("sFresnelPower"));
                                        }
                                        EditorGUILayout.EndVertical();
                                    }
                                    EditorGUILayout.EndVertical();
                                }

                                //------------------------------------------------------------------------------------------------------------------------------
                                // Glitter
                                if(shaderSetting.LIL_FEATURE_GLITTER)
                                {
                                    EditorGUILayout.BeginVertical(boxOuter);
                                    materialEditor.ShaderProperty(useGlitter, GetLoc("sGlitter"));
                                    DrawHelpButton(GetLoc("sAnchorGlitter"));
                                    if(useGlitter.floatValue == 1)
                                    {
                                        EditorGUILayout.BeginVertical(boxInnerHalf);
                                        materialEditor.ShaderProperty(glitterUVMode, "UV Mode|UV0|UV1");
                                        materialEditor.TexturePropertySingleLine(colorRGBAContent, glitterColorTex, glitterColor);
                                        if(glitterColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                        {
                                            glitterColor.colorValue = new Color(glitterColor.colorValue.r, glitterColor.colorValue.g, glitterColor.colorValue.b, 1.0f);
                                        }
                                        materialEditor.ShaderProperty(glitterMainStrength, GetLoc("sMainColorPower"));
                                        materialEditor.ShaderProperty(glitterEnableLighting, GetLoc("sEnableLighting"));
                                        materialEditor.ShaderProperty(glitterShadowMask, GetLoc("sShadowMask"));
                                        if(isTransparent) materialEditor.ShaderProperty(glitterApplyTransparency, GetLoc("sApplyTransparency"));
                                        DrawLine();
                                        materialEditor.ShaderProperty(glitterParams1, sGlitterParams1);
                                        materialEditor.ShaderProperty(glitterParams2, sGlitterParams2);
                                        materialEditor.ShaderProperty(glitterVRParallaxStrength, GetLoc("sVRParallaxStrength"));
                                        EditorGUILayout.EndVertical();
                                    }
                                    EditorGUILayout.EndVertical();
                                }
                            }
                        }
                    }

                    EditorGUILayout.Space();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Advanced
                    GUILayout.Label(" " + GetLoc("sAdvanced"), EditorStyles.boldLabel);

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Outline
                    if(!isRefr & !isFur)
                    {
                        edSet.isShowOutline = Foldout(GetLoc("sOutlineSetting"), GetLoc("sOutlineTips"), edSet.isShowOutline);
                        DrawHelpButton(GetLoc("sAnchorOutline"));
                        if(edSet.isShowOutline)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            if(isOutl != EditorGUILayout.ToggleLeft(GetLoc("sOutline"), isOutl, customToggleFont))
                            {
                                SetupMaterialWithRenderingMode(material, renderingModeBuf, !isOutl, isLite, isStWr, isTess);
                            }
                            if(isOutl)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                if(shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR) TextureGUI(materialEditor, ref edSet.isShowOutlineMap, colorRGBAContent, outlineTex, outlineColor, outlineTex_ScrollRotate, true, shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV);
                                else                                            materialEditor.ShaderProperty(outlineColor, GetLoc("sColor"));
                                if(shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION)
                                {
                                    ToneCorrectionGUI(materialEditor, material, outlineTex, outlineColor, outlineTexHSVG);
                                    if(GUILayout.Button(GetLoc("sBake")))
                                    {
                                        outlineTex.textureValue = AutoBakeOutlineTexture(material);
                                        outlineTexHSVG.vectorValue = defaultHSVG;
                                    }
                                    DrawLine();
                                }
                                materialEditor.ShaderProperty(outlineEnableLighting, GetLoc("sEnableLighting"));
                                DrawLine();
                                if(shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH) materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sWidth"), GetLoc("sWidthR")), outlineWidthMask, outlineWidth);
                                else                                            materialEditor.ShaderProperty(outlineWidth, GetLoc("sWidth"));
                                materialEditor.ShaderProperty(outlineFixWidth, GetLoc("sFixWidth"));
                                materialEditor.ShaderProperty(outlineVertexR2Width, GetLoc("sVertexR2Width"));
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Parallax
                    if(shaderSetting.LIL_FEATURE_PARALLAX && !isFur)
                    {
                        edSet.isShowParallax = Foldout(GetLoc("sParallax"), GetLoc("sParallaxTips"), edSet.isShowParallax);
                        DrawHelpButton(GetLoc("sAnchorParallax"));
                        if(edSet.isShowParallax)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            materialEditor.ShaderProperty(useParallax, GetLoc("sParallax"));
                            if(useParallax.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sParallax"), GetLoc("sParallaxR")), parallaxMap, parallax);
                                materialEditor.ShaderProperty(parallaxOffset, GetLoc("sParallaxOffset"));
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Distance Fade
                    if(shaderSetting.LIL_FEATURE_DISTANCE_FADE)
                    {
                        edSet.isShowDistanceFade = Foldout(GetLoc("sDistanceFade"), GetLoc("sDistanceFadeTips"), edSet.isShowDistanceFade);
                        DrawHelpButton(GetLoc("sAnchorDistanceFade"));
                        if(edSet.isShowDistanceFade)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField(GetLoc("sDistanceFade"), customToggleFont);
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            materialEditor.ShaderProperty(distanceFadeColor, GetLoc("sColor"));
                            materialEditor.ShaderProperty(distanceFade, sDistanceFadeSetting);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // AudioLink
                    if(shaderSetting.LIL_FEATURE_AUDIOLINK && !isFur)
                    {
                        edSet.isShowAudioLink = Foldout(GetLoc("sAudioLink"), GetLoc("sAudioLinkTips"), edSet.isShowAudioLink);
                        DrawHelpButton(GetLoc("sAnchorAudioLink"));
                        if(edSet.isShowAudioLink)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            materialEditor.ShaderProperty(useAudioLink, GetLoc("sAudioLink"));
                            if(useAudioLink.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                if(shaderSetting.LIL_FEATURE_TEX_AUDIOLINK_MASK)
                                {
                                    materialEditor.ShaderProperty(audioLinkUVMode, GetLoc("sAudioLinkUVMode") + "|" + GetLoc("sAudioLinkUVModeNone") + "|" + GetLoc("sAudioLinkUVModeRim") + "|" + GetLoc("sAudioLinkUVModeUV") + "|" + GetLoc("sAudioLinkUVModeMask"));
                                    if(audioLinkUVMode.floatValue == 3) materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sMask"), ""), audioLinkMask);
                                }
                                else
                                {
                                    materialEditor.ShaderProperty(audioLinkUVMode, GetLoc("sAudioLinkUVMode") + "|" + GetLoc("sAudioLinkUVModeNone") + "|" + GetLoc("sAudioLinkUVModeRim") + "|" + GetLoc("sAudioLinkUVModeUV"));
                                }
                                if(audioLinkUVMode.floatValue == 0) materialEditor.ShaderProperty(audioLinkUVParams, GetLoc("sOffset") + "|" + GetLoc("sAudioLinkBand") + "|" + GetLoc("sAudioLinkBandBass") + "|" + GetLoc("sAudioLinkBandLowMid") + "|" + GetLoc("sAudioLinkBandHighMid") + "|" + GetLoc("sAudioLinkBandTreble"));
                                if(audioLinkUVMode.floatValue == 1) materialEditor.ShaderProperty(audioLinkUVParams, GetLoc("sScale") + "|" + GetLoc("sOffset") + "|" + GetLoc("sAudioLinkBand") + "|" + GetLoc("sAudioLinkBandBass") + "|" + GetLoc("sAudioLinkBandLowMid") + "|" + GetLoc("sAudioLinkBandHighMid") + "|" + GetLoc("sAudioLinkBandTreble"));
                                if(audioLinkUVMode.floatValue == 2) materialEditor.ShaderProperty(audioLinkUVParams, GetLoc("sScale") + "|" + GetLoc("sOffset") + "|" + GetLoc("sAngle") + "|" + GetLoc("sAudioLinkBand") + "|" + GetLoc("sAudioLinkBandBass") + "|" + GetLoc("sAudioLinkBandLowMid") + "|" + GetLoc("sAudioLinkBandHighMid") + "|" + GetLoc("sAudioLinkBandTreble"));
                                GUILayout.Label(GetLoc("sAudioLinkApplyTo"), EditorStyles.boldLabel);
                                EditorGUI.indentLevel++;
                                if(shaderSetting.LIL_FEATURE_MAIN2ND) materialEditor.ShaderProperty(audioLink2Main2nd, GetLoc("sMainColor2nd"));
                                if(shaderSetting.LIL_FEATURE_MAIN3RD) materialEditor.ShaderProperty(audioLink2Main3rd, GetLoc("sMainColor3rd"));
                                if(shaderSetting.LIL_FEATURE_EMISSION_1ST) materialEditor.ShaderProperty(audioLink2Emission, GetLoc("sEmission"));
                                if(shaderSetting.LIL_FEATURE_EMISSION_2ND) materialEditor.ShaderProperty(audioLink2Emission2nd, GetLoc("sEmission2nd"));
                                if(shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX)
                                {
                                    materialEditor.ShaderProperty(audioLink2Vertex, GetLoc("sVertex"));
                                    if(audioLink2Vertex.floatValue == 1)
                                    {
                                        EditorGUI.indentLevel++;
                                        if(shaderSetting.LIL_FEATURE_TEX_AUDIOLINK_MASK)
                                        {
                                            materialEditor.ShaderProperty(audioLinkVertexUVMode, GetLoc("sAudioLinkUVMode") + "|" + GetLoc("sAudioLinkUVModeNone") + "|" + GetLoc("sAudioLinkUVModePosition") + "|" + GetLoc("sAudioLinkUVModeUV") + "|" + GetLoc("sAudioLinkUVModeMask"));
                                            if(audioLinkVertexUVMode.floatValue == 3) materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sMask"), ""), audioLinkMask);
                                        }
                                        else
                                        {
                                            materialEditor.ShaderProperty(audioLinkVertexUVMode, GetLoc("sAudioLinkUVMode") + "|" + GetLoc("sAudioLinkUVModeNone") + "|" + GetLoc("sAudioLinkUVModePosition") + "|" + GetLoc("sAudioLinkUVModeUV"));
                                        }
                                        if(audioLinkVertexUVMode.floatValue == 0) materialEditor.ShaderProperty(audioLinkVertexUVParams, GetLoc("sOffset") + "|" + GetLoc("sAudioLinkBand") + "|" + GetLoc("sAudioLinkBandBass") + "|" + GetLoc("sAudioLinkBandLowMid") + "|" + GetLoc("sAudioLinkBandHighMid") + "|" + GetLoc("sAudioLinkBandTreble"));
                                        if(audioLinkVertexUVMode.floatValue == 1) materialEditor.ShaderProperty(audioLinkVertexUVParams, GetLoc("sScale") + "|" + GetLoc("sOffset") + "|" + GetLoc("sAudioLinkBand") + "|" + GetLoc("sAudioLinkBandBass") + "|" + GetLoc("sAudioLinkBandLowMid") + "|" + GetLoc("sAudioLinkBandHighMid") + "|" + GetLoc("sAudioLinkBandTreble"));
                                        if(audioLinkVertexUVMode.floatValue == 2) materialEditor.ShaderProperty(audioLinkVertexUVParams, GetLoc("sScale") + "|" + GetLoc("sOffset") + "|" + GetLoc("sAngle") + "|" + GetLoc("sAudioLinkBand") + "|" + GetLoc("sAudioLinkBandBass") + "|" + GetLoc("sAudioLinkBandLowMid") + "|" + GetLoc("sAudioLinkBandHighMid") + "|" + GetLoc("sAudioLinkBandTreble"));
                                        if(audioLinkVertexUVMode.floatValue == 1) materialEditor.ShaderProperty(audioLinkVertexStart, GetLoc("sAudioLinkStartPosition"));
                                        materialEditor.ShaderProperty(audioLinkVertexStrength, GetLoc("sAudioLinkMovingVector") + "|" + GetLoc("sAudioLinkNormalStrength"));
                                        EditorGUI.indentLevel--;
                                    }
                                }
                                EditorGUI.indentLevel--;
                                if(shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL)
                                {
                                    DrawLine();
                                    materialEditor.ShaderProperty(audioLinkAsLocal, GetLoc("sAudioLinkAsLocal"));
                                    if(audioLinkAsLocal.floatValue == 1)
                                    {
                                        materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sAudioLinkLocalMap"), ""), audioLinkLocalMap);
                                        materialEditor.ShaderProperty(audioLinkLocalMapParams, GetLoc("sAudioLinkLocalMapBPM") + "|" + GetLoc("sAudioLinkLocalMapNotes") + "|" + GetLoc("sOffset"));
                                    }
                                }
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Dissolve
                    if(shaderSetting.LIL_FEATURE_DISSOLVE && !isFur)
                    {
                        edSet.isShowDissolve = Foldout(GetLoc("sDissolve"), GetLoc("sDissolve"), edSet.isShowDissolve);
                        DrawHelpButton(GetLoc("sAnchorDissolve"));
                        if(edSet.isShowDissolve && renderingModeBuf == RenderingMode.Opaque)
                        {
                            GUILayout.Label(GetLoc("sDissolveWarnOpaque"), wrapLabel);
                        }
                        if(edSet.isShowDissolve && renderingModeBuf != RenderingMode.Opaque)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            materialEditor.ShaderProperty(dissolveParams, sDissolveParamsMode);
                            if(dissolveParams.vectorValue.x != 0)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                materialEditor.ShaderProperty(dissolveParams, sDissolveParamsOther);
                                if(dissolveParams.vectorValue.x == 1.0f)                                         TextureGUI(materialEditor, ref edSet.isShowDissolveMask, maskBlendContent, dissolveMask);
                                if(dissolveParams.vectorValue.x == 2.0f && dissolveParams.vectorValue.y == 0.0f) materialEditor.ShaderProperty(dissolvePos, GetLoc("sPosition") + "|2");
                                if(dissolveParams.vectorValue.x == 2.0f && dissolveParams.vectorValue.y == 1.0f) materialEditor.ShaderProperty(dissolvePos, GetLoc("sVector") + "|2");
                                if(dissolveParams.vectorValue.x == 3.0f && dissolveParams.vectorValue.y == 0.0f) materialEditor.ShaderProperty(dissolvePos, GetLoc("sPosition") + "|3");
                                if(dissolveParams.vectorValue.x == 3.0f && dissolveParams.vectorValue.y == 1.0f) materialEditor.ShaderProperty(dissolvePos, GetLoc("sVector") + "|3");
                                if(shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE)
                                {
                                    TextureGUI(materialEditor, ref edSet.isShowDissolveNoiseMask, noiseMaskContent, dissolveNoiseMask, dissolveNoiseStrength, dissolveNoiseMask_ScrollRotate);
                                }
                                materialEditor.ShaderProperty(dissolveColor, GetLoc("sColor"));
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Encryption
                    if(shaderSetting.LIL_FEATURE_ENCRYPTION)
                    {
                        edSet.isShowEncryption = Foldout(GetLoc("sEncryption"), GetLoc("sEncryptionTips"), edSet.isShowEncryption);
                        DrawHelpButton(GetLoc("sAnchorEncryption"));
                        if(edSet.isShowEncryption)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField(GetLoc("sEncryption"), customToggleFont);
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            materialEditor.ShaderProperty(ignoreEncryption, GetLoc("sIgnoreEncryption"));
                            materialEditor.ShaderProperty(keys, GetLoc("sKeys"));
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Refraction
                    if(isRefr)
                    {
                        edSet.isShowRefraction = Foldout(GetLoc("sRefractionSetting"), GetLoc("sRefractionTips"), edSet.isShowRefraction);
                        DrawHelpButton(GetLoc("sAnchorRefraction"));
                        if(edSet.isShowRefraction)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField(GetLoc("sRefraction"), customToggleFont);
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            materialEditor.ShaderProperty(refractionStrength, GetLoc("sStrength"));
                            materialEditor.ShaderProperty(refractionFresnelPower, GetLoc("sRefractionFresnel"));
                            materialEditor.ShaderProperty(refractionColorFromMain, GetLoc("sColorFromMain"));
                            materialEditor.ShaderProperty(refractionColor, GetLoc("sColor"));
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Fur
                    if(isFur)
                    {
                        edSet.isShowFur = Foldout(GetLoc("sFurSetting"), GetLoc("sFurTips"), edSet.isShowFur);
                        DrawHelpButton(GetLoc("sAnchorFur"));
                        if(edSet.isShowFur)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField(GetLoc("sFur"), customToggleFont);
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            if(shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL) materialEditor.TexturePropertySingleLine(normalMapContent, furVectorTex,furVectorScale);
                            if(shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH) materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sLength"), GetLoc("sStrengthR")), furLengthMask);
                            materialEditor.ShaderProperty(furVector, GetLoc("sVector") + "|" + GetLoc("sLength"));
                            materialEditor.ShaderProperty(vertexColor2FurVector, GetLoc("sVertexColor2Vector"));
                            materialEditor.ShaderProperty(furGravity, GetLoc("sGravity"));
                            DrawLine();
                            materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sNoise"), GetLoc("sNoiseR")), furNoiseMask);
                            materialEditor.TextureScaleOffsetProperty(furNoiseMask);
                            if(shaderSetting.LIL_FEATURE_TEX_FUR_MASK) materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sMask"), GetLoc("sAlphaR")), furMask);
                            materialEditor.ShaderProperty(furAO, GetLoc("sAO"));
                            materialEditor.ShaderProperty(furLayerNum, GetLoc("sLayerNum"));
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Stencil
                    edSet.isShowStencil = Foldout(GetLoc("sStencilSetting"), GetLoc("sStencilTips"), edSet.isShowStencil);
                    DrawHelpButton(GetLoc("sAnchorStencil"));
                    if(edSet.isShowStencil)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sStencilSetting"), customToggleFont);
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
                                EditorGUILayout.LabelField(GetLoc("sOutline"), EditorStyles.boldLabel);
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
                                EditorGUILayout.LabelField(GetLoc("sOutline"), EditorStyles.boldLabel);
                                furStencilRef.floatValue = 1;
                                furStencilReadMask.floatValue = 255.0f;
                                furStencilWriteMask.floatValue = 255.0f;
                                furStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                                furStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Replace;
                                furStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                                furStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                            }
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, isOutl, isLite, isStWr, isTess);
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
                                EditorGUILayout.LabelField(GetLoc("sOutline"), EditorStyles.boldLabel);
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
                                EditorGUILayout.LabelField(GetLoc("sOutline"), EditorStyles.boldLabel);
                                furStencilRef.floatValue = 1;
                                furStencilReadMask.floatValue = 255.0f;
                                furStencilWriteMask.floatValue = 255.0f;
                                furStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.NotEqual;
                                furStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                                furStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                                furStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                            }
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, isOutl, isLite, isStWr, isTess);
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
                                EditorGUILayout.LabelField(GetLoc("sOutline"), EditorStyles.boldLabel);
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
                                EditorGUILayout.LabelField(GetLoc("sOutline"), EditorStyles.boldLabel);
                                furStencilRef.floatValue = 0;
                                furStencilReadMask.floatValue = 255.0f;
                                furStencilWriteMask.floatValue = 255.0f;
                                furStencilComp.floatValue = (float)UnityEngine.Rendering.CompareFunction.Always;
                                furStencilPass.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                                furStencilFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                                furStencilZFail.floatValue = (float)UnityEngine.Rendering.StencilOp.Keep;
                            }
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, isOutl, isLite, isStWr, isTess);
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Base
                        {
                            DrawLine();
                            materialEditor.ShaderProperty(stencilRef, "Ref");
                            materialEditor.ShaderProperty(stencilReadMask, "ReadMask");
                            materialEditor.ShaderProperty(stencilWriteMask, "WriteMask");
                            materialEditor.ShaderProperty(stencilComp, "Comp");
                            materialEditor.ShaderProperty(stencilPass, "Pass");
                            materialEditor.ShaderProperty(stencilFail, "Fail");
                            materialEditor.ShaderProperty(stencilZFail, "ZFail");
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Outline
                        if(isOutl)
                        {
                            DrawLine();
                            EditorGUILayout.LabelField(GetLoc("sOutline"), EditorStyles.boldLabel);
                            materialEditor.ShaderProperty(outlineStencilRef, "Ref");
                            materialEditor.ShaderProperty(outlineStencilReadMask, "ReadMask");
                            materialEditor.ShaderProperty(outlineStencilWriteMask, "WriteMask");
                            materialEditor.ShaderProperty(outlineStencilComp, "Comp");
                            materialEditor.ShaderProperty(outlineStencilPass, "Pass");
                            materialEditor.ShaderProperty(outlineStencilFail, "Fail");
                            materialEditor.ShaderProperty(outlineStencilZFail, "ZFail");
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Fur
                        if(isFur)
                        {
                            DrawLine();
                            EditorGUILayout.LabelField(GetLoc("sFur"), EditorStyles.boldLabel);
                            materialEditor.ShaderProperty(furStencilRef, "Ref");
                            materialEditor.ShaderProperty(furStencilReadMask, "ReadMask");
                            materialEditor.ShaderProperty(furStencilWriteMask, "WriteMask");
                            materialEditor.ShaderProperty(furStencilComp, "Comp");
                            materialEditor.ShaderProperty(furStencilPass, "Pass");
                            materialEditor.ShaderProperty(furStencilFail, "Fail");
                            materialEditor.ShaderProperty(furStencilZFail, "ZFail");
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Rendering
                    edSet.isShowRendering = Foldout(GetLoc("sRenderingSetting"), GetLoc("sRenderingTips"), edSet.isShowRendering);
                    DrawHelpButton(GetLoc("sAnchorRendering"));
                    if(edSet.isShowRendering)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Reset Button
                        if(GUILayout.Button(GetLoc("sRenderingReset"), offsetButton))
                        {
                            material.enableInstancing = false;
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, isOutl, isLite, isStWr, isTess);
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
                            shaderType = EditorGUILayout.Popup(GetLoc("sShaderType"),shaderType,new String[]{GetLoc("sShaderTypeNormal"),GetLoc("sShaderTypeLite")});
                            if(shaderTypeBuf != shaderType)
                            {
                                if(shaderType==0) isLite = false;
                                if(shaderType==1) isLite = true;
                                SetupMaterialWithRenderingMode(material, renderingModeBuf, isOutl, isLite, isStWr, isTess);
                            }

                            //------------------------------------------------------------------------------------------------------------------------------
                            // Rendering
                            materialEditor.ShaderProperty(cull, sCullModes);
                            materialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                            materialEditor.ShaderProperty(ztest, GetLoc("sZTest"));
                            materialEditor.ShaderProperty(offsetFactor, GetLoc("sOffsetFactor"));
                            materialEditor.ShaderProperty(offsetUnits, GetLoc("sOffsetUnits"));
                            materialEditor.ShaderProperty(colorMask, GetLoc("sColorMask"));
                            DrawLine();
                            BlendSettingGUI(materialEditor, ref edSet.isShowBlend, GetLoc("sForward"), srcBlend, dstBlend, srcBlendAlpha, dstBlendAlpha, blendOp, blendOpAlpha);
                            if(!(isFur & !isCutout))
                            {
                                DrawLine();
                                BlendSettingGUI(materialEditor, ref edSet.isShowBlendAdd, GetLoc("sForwardAdd"), srcBlendFA, dstBlendFA, srcBlendAlphaFA, dstBlendAlphaFA, blendOpFA, blendOpAlphaFA);
                            }
                            DrawLine();
                            materialEditor.EnableInstancingField();
                            materialEditor.DoubleSidedGIField();
                            materialEditor.RenderQueueField();
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
                            materialEditor.ShaderProperty(outlineCull, sCullModes);
                            materialEditor.ShaderProperty(outlineZwrite, GetLoc("sZWrite"));
                            materialEditor.ShaderProperty(outlineZtest, GetLoc("sZTest"));
                            materialEditor.ShaderProperty(outlineOffsetFactor, GetLoc("sOffsetFactor"));
                            materialEditor.ShaderProperty(outlineOffsetUnits, GetLoc("sOffsetUnits"));
                            materialEditor.ShaderProperty(outlineColorMask, GetLoc("sColorMask"));
                            DrawLine();
                            BlendSettingGUI(materialEditor, ref edSet.isShowBlendOutline, GetLoc("sForward"), outlineSrcBlend, outlineDstBlend, outlineSrcBlendAlpha, outlineDstBlendAlpha, outlineBlendOp, outlineBlendOpAlpha);
                            DrawLine();
                            BlendSettingGUI(materialEditor, ref edSet.isShowBlendAddOutline, GetLoc("sForwardAdd"), outlineSrcBlendFA, outlineDstBlendFA, outlineSrcBlendAlphaFA, outlineDstBlendAlphaFA, outlineBlendOpFA, outlineBlendOpAlphaFA);
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
                            materialEditor.ShaderProperty(furCull, sCullModes);
                            materialEditor.ShaderProperty(furZwrite, GetLoc("sZWrite"));
                            materialEditor.ShaderProperty(furZtest, GetLoc("sZTest"));
                            materialEditor.ShaderProperty(furOffsetFactor, GetLoc("sOffsetFactor"));
                            materialEditor.ShaderProperty(furOffsetUnits, GetLoc("sOffsetUnits"));
                            materialEditor.ShaderProperty(furColorMask, GetLoc("sColorMask"));
                            DrawLine();
                            BlendSettingGUI(materialEditor, ref edSet.isShowBlendFur, GetLoc("sForward"), furSrcBlend, furDstBlend, furSrcBlendAlpha, furDstBlendAlpha, furBlendOp, furBlendOpAlpha);
                            DrawLine();
                            BlendSettingGUI(materialEditor, ref edSet.isShowBlendAddFur, GetLoc("sForwardAdd"), furSrcBlendFA, furDstBlendFA, furSrcBlendAlphaFA, furDstBlendAlphaFA, furBlendOpFA, furBlendOpAlphaFA);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Tessellation
                    if(shaderSetting.LIL_FEATURE_TEX_TESSELLATION && !isRefr && !isFur)
                    {
                        edSet.isShowTess = Foldout(GetLoc("sTessellation"), GetLoc("sTessellationTips"), edSet.isShowTess);
                        DrawHelpButton(GetLoc("sAnchorTessellation"));
                        if(edSet.isShowTess)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            if(isTess != EditorGUILayout.ToggleLeft(GetLoc("sTessellation"), isTess, customToggleFont))
                            {
                                SetupMaterialWithRenderingMode(material, renderingModeBuf, isOutl, isLite, isStWr, !isTess);
                            }
                            if(isTess)
                            {
                                EditorGUILayout.BeginVertical(boxInner);
                                materialEditor.ShaderProperty(tessEdge, GetLoc("sTessellationEdge"));
                                materialEditor.ShaderProperty(tessStrength, GetLoc("sStrength"));
                                materialEditor.ShaderProperty(tessShrink, GetLoc("sTessellationShrink"));
                                materialEditor.ShaderProperty(tessFactorMax, GetLoc("sTessellationFactor"));
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }

                    EditorGUILayout.Space();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Optimization
                    GUILayout.Label(" " + GetLoc("sOptimization"), EditorStyles.boldLabel);
                    edSet.isShowOptimization = Foldout(GetLoc("sOptimization"), GetLoc("sOptimizationTips"), edSet.isShowOptimization);
                    DrawHelpButton(GetLoc("sAnchorOptimization"));
                    if(edSet.isShowOptimization)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sOptimization"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        if(GUILayout.Button(GetLoc("sRemoveUnused"))) RemoveUnusedTexture(material, isLite, isFur, shaderSetting);
                        if(!isFur)
                        {
                            TextureBakeGUI(material, 0);
                            TextureBakeGUI(material, 1);
                            TextureBakeGUI(material, 2);
                            TextureBakeGUI(material, 3);
                        }
                        if(GUILayout.Button(GetLoc("sConvertLite"))) CreateLiteMaterial(material);
                        if(mtoon != null && GUILayout.Button(GetLoc("sConvertMToon"))) CreateMToonMaterial(material);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Shader Setting
                    edSet.isShowShaderSetting = Foldout(GetLoc("sShaderSetting"), GetLoc("sShaderSettingTips"), edSet.isShowShaderSetting);
                    DrawHelpButton(GetLoc("sAnchorShaderSetting"));
                    if(edSet.isShowShaderSetting)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sShaderSetting"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        EditorGUILayout.HelpBox(GetLoc("sHelpShaderSetting"),MessageType.Info);
                        ShaderSettingGUI();
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }


                    //------------------------------------------------------------------------------------------------------------------------------
                    // Custom Properties
                    if(isCustomShader)
                    {
                        EditorGUILayout.Space();
                        GUILayout.Label(" " + GetLoc("sCustomProperties"), EditorStyles.boldLabel);
                        edSet.isShowCustomProperties = Foldout(GetLoc("sCustomProperties"), GetLoc("sCustomPropertiesTips"), edSet.isShowCustomProperties);
                        if(edSet.isShowCustomProperties)
                        {
                            DrawCustomProperties(materialEditor, material);
                        }
                    }
                }
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Preset
            if(edSet.editorMode == EditorMode.Preset)
            {
                if(isLite)  EditorGUILayout.LabelField(GetLoc("sPresetsNotAvailable"), wrapLabel);
                else        DrawPreset(material);
            }

            SaveEditorSettingTemp();
	    }

        //------------------------------------------------------------------------------------------------------------------------------
        // Property loader
        void LoadProperties(MaterialProperty[] props)
        {
            invisible = FindProperty("_Invisible", props);
            asUnlit = FindProperty("_AsUnlit", props);
            cutoff = FindProperty("_Cutoff", props);
            flipNormal = FindProperty("_FlipNormal", props);
            backfaceForceShadow = FindProperty("_BackfaceForceShadow", props);
            vertexLightStrength = FindProperty("_VertexLightStrength", props);
            lightMinLimit = FindProperty("_LightMinLimit", props);
            mainTex_ScrollRotate = FindProperty("_MainTex_ScrollRotate", props);
                cull = FindProperty("_Cull", props);
                srcBlend = FindProperty("_SrcBlend", props);
                dstBlend = FindProperty("_DstBlend", props);
                srcBlendAlpha = FindProperty("_SrcBlendAlpha", props);
                dstBlendAlpha = FindProperty("_DstBlendAlpha", props);
                blendOp = FindProperty("_BlendOp", props);
                blendOpAlpha = FindProperty("_BlendOpAlpha", props);
                srcBlendFA = FindProperty("_SrcBlendFA", props);
                dstBlendFA = FindProperty("_DstBlendFA", props);
                srcBlendAlphaFA = FindProperty("_SrcBlendAlphaFA", props);
                dstBlendAlphaFA = FindProperty("_DstBlendAlphaFA", props);
                blendOpFA = FindProperty("_BlendOpFA", props);
                blendOpAlphaFA = FindProperty("_BlendOpAlphaFA", props);
                zwrite = FindProperty("_ZWrite", props);
                ztest = FindProperty("_ZTest", props);
                stencilRef = FindProperty("_StencilRef", props);
                stencilReadMask = FindProperty("_StencilReadMask", props);
                stencilWriteMask = FindProperty("_StencilWriteMask", props);
                stencilComp = FindProperty("_StencilComp", props);
                stencilPass = FindProperty("_StencilPass", props);
                stencilFail = FindProperty("_StencilFail", props);
                stencilZFail = FindProperty("_StencilZFail", props);
                offsetFactor = FindProperty("_OffsetFactor", props);
                offsetUnits = FindProperty("_OffsetUnits", props);
                colorMask = FindProperty("_ColorMask", props);
            // Main
            //useMainTex = FindProperty("_UseMainTex", props);
                mainColor = FindProperty("_Color", props);
                mainTex = FindProperty("_MainTex", props);
                mainTexHSVG = FindProperty("_MainTexHSVG", props);
                mainGradationStrength = FindProperty("_MainGradationStrength", props);
                mainGradationTex = FindProperty("_MainGradationTex", props);
                mainColorAdjustMask = FindProperty("_MainColorAdjustMask", props);
            useMain2ndTex = FindProperty("_UseMain2ndTex", props);
                mainColor2nd = FindProperty("_Color2nd", props);
                main2ndTex = FindProperty("_Main2ndTex", props);
                main2ndTexAngle = FindProperty("_Main2ndTexAngle", props);
                main2ndTexDecalAnimation = FindProperty("_Main2ndTexDecalAnimation", props);
                main2ndTexDecalSubParam = FindProperty("_Main2ndTexDecalSubParam", props);
                main2ndTexIsDecal = FindProperty("_Main2ndTexIsDecal", props);
                main2ndTexIsLeftOnly = FindProperty("_Main2ndTexIsLeftOnly", props);
                main2ndTexIsRightOnly = FindProperty("_Main2ndTexIsRightOnly", props);
                main2ndTexShouldCopy = FindProperty("_Main2ndTexShouldCopy", props);
                main2ndTexShouldFlipMirror = FindProperty("_Main2ndTexShouldFlipMirror", props);
                main2ndTexShouldFlipCopy = FindProperty("_Main2ndTexShouldFlipCopy", props);
                main2ndTexIsMSDF = FindProperty("_Main2ndTexIsMSDF", props);
                main2ndBlendMask = FindProperty("_Main2ndBlendMask", props);
                main2ndTexBlendMode = FindProperty("_Main2ndTexBlendMode", props);
                main2ndEnableLighting = FindProperty("_Main2ndEnableLighting", props);
                main2ndDissolveMask = FindProperty("_Main2ndDissolveMask", props);
                main2ndDissolveNoiseMask = FindProperty("_Main2ndDissolveNoiseMask", props);
                main2ndDissolveNoiseMask_ScrollRotate = FindProperty("_Main2ndDissolveNoiseMask_ScrollRotate", props);
                main2ndDissolveNoiseStrength = FindProperty("_Main2ndDissolveNoiseStrength", props);
                main2ndDissolveColor = FindProperty("_Main2ndDissolveColor", props);
                main2ndDissolveParams = FindProperty("_Main2ndDissolveParams", props);
                main2ndDissolvePos = FindProperty("_Main2ndDissolvePos", props);
                main2ndDistanceFade = FindProperty("_Main2ndDistanceFade", props);
            useMain3rdTex = FindProperty("_UseMain3rdTex", props);
                mainColor3rd = FindProperty("_Color3rd", props);
                main3rdTex = FindProperty("_Main3rdTex", props);
                main3rdTexAngle = FindProperty("_Main3rdTexAngle", props);
                main3rdTexIsDecal = FindProperty("_Main3rdTexIsDecal", props);
                main3rdTexDecalAnimation = FindProperty("_Main3rdTexDecalAnimation", props);
                main3rdTexDecalSubParam = FindProperty("_Main3rdTexDecalSubParam", props);
                main3rdTexIsLeftOnly = FindProperty("_Main3rdTexIsLeftOnly", props);
                main3rdTexIsRightOnly = FindProperty("_Main3rdTexIsRightOnly", props);
                main3rdTexShouldCopy = FindProperty("_Main3rdTexShouldCopy", props);
                main3rdTexShouldFlipMirror = FindProperty("_Main3rdTexShouldFlipMirror", props);
                main3rdTexShouldFlipCopy = FindProperty("_Main3rdTexShouldFlipCopy", props);
                main3rdTexIsMSDF = FindProperty("_Main3rdTexIsMSDF", props);
                main3rdBlendMask = FindProperty("_Main3rdBlendMask", props);
                main3rdTexBlendMode = FindProperty("_Main3rdTexBlendMode", props);
                main3rdEnableLighting = FindProperty("_Main3rdEnableLighting", props);
                main3rdDissolveMask = FindProperty("_Main3rdDissolveMask", props);
                main3rdDissolveNoiseMask = FindProperty("_Main3rdDissolveNoiseMask", props);
                main3rdDissolveNoiseMask_ScrollRotate = FindProperty("_Main3rdDissolveNoiseMask_ScrollRotate", props);
                main3rdDissolveNoiseStrength = FindProperty("_Main3rdDissolveNoiseStrength", props);
                main3rdDissolveColor = FindProperty("_Main3rdDissolveColor", props);
                main3rdDissolveParams = FindProperty("_Main3rdDissolveParams", props);
                main3rdDissolvePos = FindProperty("_Main3rdDissolvePos", props);
                main3rdDistanceFade = FindProperty("_Main3rdDistanceFade", props);
            // Alpha Mask
            alphaMaskMode = FindProperty("_AlphaMaskMode", props);
                alphaMask = FindProperty("_AlphaMask", props);
                alphaMaskValue = FindProperty("_AlphaMaskValue", props);
            // Shadow
            useShadow = FindProperty("_UseShadow", props);
                shadowBorder = FindProperty("_ShadowBorder", props);
                shadowBorderMask = FindProperty("_ShadowBorderMask", props);
                shadowBlur = FindProperty("_ShadowBlur", props);
                shadowBlurMask = FindProperty("_ShadowBlurMask", props);
                shadowStrength = FindProperty("_ShadowStrength", props);
                shadowStrengthMask = FindProperty("_ShadowStrengthMask", props);
                shadowColor = FindProperty("_ShadowColor", props);
                shadowColorTex = FindProperty("_ShadowColorTex", props);
                shadow2ndBorder = FindProperty("_Shadow2ndBorder", props);
                shadow2ndBlur = FindProperty("_Shadow2ndBlur", props);
                shadow2ndColor = FindProperty("_Shadow2ndColor", props);
                shadow2ndColorTex = FindProperty("_Shadow2ndColorTex", props);
                shadowMainStrength = FindProperty("_ShadowMainStrength", props);
                shadowEnvStrength = FindProperty("_ShadowEnvStrength", props);
                shadowBorderColor = FindProperty("_ShadowBorderColor", props);
                shadowBorderRange = FindProperty("_ShadowBorderRange", props);
                shadowReceive = FindProperty("_ShadowReceive", props);
            // Outline
            if(isOutl)
            {
                outlineColor = FindProperty("_OutlineColor", props);
                outlineTex = FindProperty("_OutlineTex", props);
                outlineTex_ScrollRotate = FindProperty("_OutlineTex_ScrollRotate", props);
                outlineTexHSVG = FindProperty("_OutlineTexHSVG", props);
                outlineWidth = FindProperty("_OutlineWidth", props);
                outlineWidthMask = FindProperty("_OutlineWidthMask", props);
                outlineFixWidth = FindProperty("_OutlineFixWidth", props);
                outlineVertexR2Width = FindProperty("_OutlineVertexR2Width", props);
                outlineEnableLighting = FindProperty("_OutlineEnableLighting", props);
                outlineCull = FindProperty("_OutlineCull", props);
                outlineSrcBlend = FindProperty("_OutlineSrcBlend", props);
                outlineDstBlend = FindProperty("_OutlineDstBlend", props);
                outlineSrcBlendAlpha = FindProperty("_OutlineSrcBlendAlpha", props);
                outlineDstBlendAlpha = FindProperty("_OutlineDstBlendAlpha", props);
                outlineBlendOp = FindProperty("_OutlineBlendOp", props);
                outlineBlendOpAlpha = FindProperty("_OutlineBlendOpAlpha", props);
                outlineSrcBlendFA = FindProperty("_OutlineSrcBlendFA", props);
                outlineDstBlendFA = FindProperty("_OutlineDstBlendFA", props);
                outlineSrcBlendAlphaFA = FindProperty("_OutlineSrcBlendAlphaFA", props);
                outlineDstBlendAlphaFA = FindProperty("_OutlineDstBlendAlphaFA", props);
                outlineBlendOpFA = FindProperty("_OutlineBlendOpFA", props);
                outlineBlendOpAlphaFA = FindProperty("_OutlineBlendOpAlphaFA", props);
                outlineZwrite = FindProperty("_OutlineZWrite", props);
                outlineZtest = FindProperty("_OutlineZTest", props);
                outlineStencilRef = FindProperty("_OutlineStencilRef", props);
                outlineStencilReadMask = FindProperty("_OutlineStencilReadMask", props);
                outlineStencilWriteMask = FindProperty("_OutlineStencilWriteMask", props);
                outlineStencilComp = FindProperty("_OutlineStencilComp", props);
                outlineStencilPass = FindProperty("_OutlineStencilPass", props);
                outlineStencilFail = FindProperty("_OutlineStencilFail", props);
                outlineStencilZFail = FindProperty("_OutlineStencilZFail", props);
                outlineOffsetFactor = FindProperty("_OutlineOffsetFactor", props);
                outlineOffsetUnits = FindProperty("_OutlineOffsetUnits", props);
                outlineColorMask = FindProperty("_OutlineColorMask", props);
            }
            // Normal
            useBumpMap = FindProperty("_UseBumpMap", props);
                bumpMap = FindProperty("_BumpMap", props);
                bumpScale = FindProperty("_BumpScale", props);
            useBump2ndMap = FindProperty("_UseBump2ndMap", props);
                bump2ndMap = FindProperty("_Bump2ndMap", props);
                bump2ndScale = FindProperty("_Bump2ndScale", props);
                bump2ndScaleMask = FindProperty("_Bump2ndScaleMask", props);
            useReflection = FindProperty("_UseReflection", props);
                smoothness = FindProperty("_Smoothness", props);
                smoothnessTex = FindProperty("_SmoothnessTex", props);
                metallic = FindProperty("_Metallic", props);
                metallicGlossMap = FindProperty("_MetallicGlossMap", props);
                reflectance = FindProperty("_Reflectance", props);
                reflectionColor = FindProperty("_ReflectionColor", props);
                reflectionColorTex = FindProperty("_ReflectionColorTex", props);
                applySpecular = FindProperty("_ApplySpecular", props);
                specularToon = FindProperty("_SpecularToon", props);
                applyReflection = FindProperty("_ApplyReflection", props);
                reflectionApplyTransparency = FindProperty("_ReflectionApplyTransparency", props);
            useMatCap = FindProperty("_UseMatCap", props);
                matcapTex = FindProperty("_MatCapTex", props);
                matcapColor = FindProperty("_MatCapColor", props);
                matcapBlend = FindProperty("_MatCapBlend", props);
                matcapBlendMask = FindProperty("_MatCapBlendMask", props);
                matcapEnableLighting = FindProperty("_MatCapEnableLighting", props);
                matcapBlendMode = FindProperty("_MatCapBlendMode", props);
                matcapApplyTransparency = FindProperty("_MatCapApplyTransparency", props);
                matcapZRotCancel = FindProperty("_MatCapZRotCancel", props);
                matcapCustomNormal = FindProperty("_MatCapCustomNormal", props);
                matcapBumpMap = FindProperty("_MatCapBumpMap", props);
                matcapBumpScale = FindProperty("_MatCapBumpScale", props);
            useMatCap2nd = FindProperty("_UseMatCap2nd", props);
                matcap2ndTex = FindProperty("_MatCap2ndTex", props);
                matcap2ndColor = FindProperty("_MatCap2ndColor", props);
                matcap2ndBlend = FindProperty("_MatCap2ndBlend", props);
                matcap2ndBlendMask = FindProperty("_MatCap2ndBlendMask", props);
                matcap2ndEnableLighting = FindProperty("_MatCap2ndEnableLighting", props);
                matcap2ndBlendMode = FindProperty("_MatCap2ndBlendMode", props);
                matcap2ndApplyTransparency = FindProperty("_MatCap2ndApplyTransparency", props);
                matcap2ndZRotCancel = FindProperty("_MatCap2ndZRotCancel", props);
                matcap2ndCustomNormal = FindProperty("_MatCap2ndCustomNormal", props);
                matcap2ndBumpMap = FindProperty("_MatCap2ndBumpMap", props);
                matcap2ndBumpScale = FindProperty("_MatCap2ndBumpScale", props);
            useRim = FindProperty("_UseRim", props);
                rimColor = FindProperty("_RimColor", props);
                rimColorTex = FindProperty("_RimColorTex", props);
                rimBorder = FindProperty("_RimBorder", props);
                rimBlur = FindProperty("_RimBlur", props);
                rimFresnelPower = FindProperty("_RimFresnelPower", props);
                rimEnableLighting = FindProperty("_RimEnableLighting", props);
                rimShadowMask = FindProperty("_RimShadowMask", props);
                rimApplyTransparency = FindProperty("_RimApplyTransparency", props);
                rimDirStrength = FindProperty("_RimDirStrength", props);
                rimDirRange = FindProperty("_RimDirRange", props);
                rimIndirRange = FindProperty("_RimIndirRange", props);
                rimIndirColor = FindProperty("_RimIndirColor", props);
                rimIndirBorder = FindProperty("_RimIndirBorder", props);
                rimIndirBlur = FindProperty("_RimIndirBlur", props);
            useGlitter = FindProperty("_UseGlitter", props);
                glitterUVMode = FindProperty("_GlitterUVMode", props);
                glitterColor = FindProperty("_GlitterColor", props);
                glitterColorTex = FindProperty("_GlitterColorTex", props);
                glitterMainStrength = FindProperty("_GlitterMainStrength", props);
                glitterEnableLighting = FindProperty("_GlitterEnableLighting", props);
                glitterShadowMask = FindProperty("_GlitterShadowMask", props);
                glitterApplyTransparency = FindProperty("_GlitterApplyTransparency", props);
                glitterParams1 = FindProperty("_GlitterParams1", props);
                glitterParams2 = FindProperty("_GlitterParams2", props);
                glitterVRParallaxStrength = FindProperty("_GlitterVRParallaxStrength", props);
            useEmission = FindProperty("_UseEmission", props);
                emissionColor = FindProperty("_EmissionColor", props);
                emissionMap = FindProperty("_EmissionMap", props);
                emissionMap_ScrollRotate = FindProperty("_EmissionMap_ScrollRotate", props);
                emissionBlend = FindProperty("_EmissionBlend", props);
                emissionBlendMask = FindProperty("_EmissionBlendMask", props);
                emissionBlendMask_ScrollRotate = FindProperty("_EmissionBlendMask_ScrollRotate", props);
                emissionBlink = FindProperty("_EmissionBlink", props);
                emissionUseGrad = FindProperty("_EmissionUseGrad", props);
                emissionGradTex = FindProperty("_EmissionGradTex", props);
                emissionGradSpeed = FindProperty("_EmissionGradSpeed", props);
                emissionParallaxDepth = FindProperty("_EmissionParallaxDepth", props);
                emissionFluorescence = FindProperty("_EmissionFluorescence", props);
            useEmission2nd = FindProperty("_UseEmission2nd", props);
                emission2ndColor = FindProperty("_Emission2ndColor", props);
                emission2ndMap = FindProperty("_Emission2ndMap", props);
                emission2ndMap_ScrollRotate = FindProperty("_Emission2ndMap_ScrollRotate", props);
                emission2ndBlend = FindProperty("_Emission2ndBlend", props);
                emission2ndBlendMask = FindProperty("_Emission2ndBlendMask", props);
                emission2ndBlendMask_ScrollRotate = FindProperty("_Emission2ndBlendMask_ScrollRotate", props);
                emission2ndBlink = FindProperty("_Emission2ndBlink", props);
                emission2ndUseGrad = FindProperty("_Emission2ndUseGrad", props);
                emission2ndGradTex = FindProperty("_Emission2ndGradTex", props);
                emission2ndGradSpeed = FindProperty("_Emission2ndGradSpeed", props);
                emission2ndParallaxDepth = FindProperty("_Emission2ndParallaxDepth", props);
                emission2ndFluorescence = FindProperty("_Emission2ndFluorescence", props);
            useParallax = FindProperty("_UseParallax", props);
                parallaxMap = FindProperty("_ParallaxMap", props);
                parallax = FindProperty("_Parallax", props);
                parallaxOffset = FindProperty("_ParallaxOffset", props);
            distanceFade = FindProperty("_DistanceFade", props);
                distanceFadeColor = FindProperty("_DistanceFadeColor", props);
            useAudioLink = FindProperty("_UseAudioLink", props);
                audioLinkUVMode = FindProperty("_AudioLinkUVMode", props);
                audioLinkUVParams = FindProperty("_AudioLinkUVParams", props);
                audioLinkMask = FindProperty("_AudioLinkMask", props);
                audioLink2Main2nd = FindProperty("_AudioLink2Main2nd", props);
                audioLink2Main3rd = FindProperty("_AudioLink2Main3rd", props);
                audioLink2Emission = FindProperty("_AudioLink2Emission", props);
                audioLink2Emission2nd = FindProperty("_AudioLink2Emission2nd", props);
                audioLink2Vertex = FindProperty("_AudioLink2Vertex", props);
                audioLinkVertexUVMode = FindProperty("_AudioLinkVertexUVMode", props);
                audioLinkVertexUVParams = FindProperty("_AudioLinkVertexUVParams", props);
                audioLinkVertexStart = FindProperty("_AudioLinkVertexStart", props);
                audioLinkVertexStrength = FindProperty("_AudioLinkVertexStrength", props);
                audioLinkAsLocal = FindProperty("_AudioLinkAsLocal", props);
                audioLinkLocalMap = FindProperty("_AudioLinkLocalMap", props);
                audioLinkLocalMapParams = FindProperty("_AudioLinkLocalMapParams", props);
            //MaterialProperty useDissolve = FindProperty("_UseDissolve", props);
                dissolveMask = FindProperty("_DissolveMask", props);
                dissolveNoiseMask = FindProperty("_DissolveNoiseMask", props);
                dissolveNoiseMask_ScrollRotate = FindProperty("_DissolveNoiseMask_ScrollRotate", props);
                dissolveNoiseStrength = FindProperty("_DissolveNoiseStrength", props);
                dissolveColor = FindProperty("_DissolveColor", props);
                dissolveParams = FindProperty("_DissolveParams", props);
                dissolvePos = FindProperty("_DissolvePos", props);
            // Encryption
                ignoreEncryption = FindProperty("_IgnoreEncryption", props);
                keys = FindProperty("_Keys", props);
            // Refraction
            if(isRefr)
            {
                refractionStrength = FindProperty("_RefractionStrength", props);
                refractionFresnelPower = FindProperty("_RefractionFresnelPower", props);
                refractionColorFromMain = FindProperty("_RefractionColorFromMain", props);
                refractionColor = FindProperty("_RefractionColor", props);
            }
            // Tessellation
            if(isTess)
            {
                tessEdge = FindProperty("_TessEdge", props);
                tessStrength = FindProperty("_TessStrength", props);
                tessShrink = FindProperty("_TessShrink", props);
                tessFactorMax = FindProperty("_TessFactorMax", props);
            }
        }

        void LoadFurProperties(MaterialProperty[] props)
        {
            invisible = FindProperty("_Invisible", props);
            asUnlit = FindProperty("_AsUnlit", props);
            cutoff = FindProperty("_Cutoff", props);
            flipNormal = FindProperty("_FlipNormal", props);
            backfaceForceShadow = FindProperty("_BackfaceForceShadow", props);
            vertexLightStrength = FindProperty("_VertexLightStrength", props);
            lightMinLimit = FindProperty("_LightMinLimit", props);
            mainTex_ScrollRotate = FindProperty("_MainTex_ScrollRotate", props);
                cull = FindProperty("_Cull", props);
                srcBlend = FindProperty("_SrcBlend", props);
                dstBlend = FindProperty("_DstBlend", props);
                srcBlendAlpha = FindProperty("_SrcBlendAlpha", props);
                dstBlendAlpha = FindProperty("_DstBlendAlpha", props);
                blendOp = FindProperty("_BlendOp", props);
                blendOpAlpha = FindProperty("_BlendOpAlpha", props);
                srcBlendFA = FindProperty("_SrcBlendFA", props);
                dstBlendFA = FindProperty("_DstBlendFA", props);
                srcBlendAlphaFA = FindProperty("_SrcBlendAlphaFA", props);
                dstBlendAlphaFA = FindProperty("_DstBlendAlphaFA", props);
                blendOpFA = FindProperty("_BlendOpFA", props);
                blendOpAlphaFA = FindProperty("_BlendOpAlphaFA", props);
                zwrite = FindProperty("_ZWrite", props);
                ztest = FindProperty("_ZTest", props);
                stencilRef = FindProperty("_StencilRef", props);
                stencilReadMask = FindProperty("_StencilReadMask", props);
                stencilWriteMask = FindProperty("_StencilWriteMask", props);
                stencilComp = FindProperty("_StencilComp", props);
                stencilPass = FindProperty("_StencilPass", props);
                stencilFail = FindProperty("_StencilFail", props);
                stencilZFail = FindProperty("_StencilZFail", props);
                offsetFactor = FindProperty("_OffsetFactor", props);
                offsetUnits = FindProperty("_OffsetUnits", props);
                colorMask = FindProperty("_ColorMask", props);
            // Main
            //useMainTex = FindProperty("_UseMainTex", props);
                mainColor = FindProperty("_Color", props);
                mainTex = FindProperty("_MainTex", props);
                mainTexHSVG = FindProperty("_MainTexHSVG", props);
            // Shadow
            useShadow = FindProperty("_UseShadow", props);
                shadowBorder = FindProperty("_ShadowBorder", props);
                shadowBorderMask = FindProperty("_ShadowBorderMask", props);
                shadowBlur = FindProperty("_ShadowBlur", props);
                shadowBlurMask = FindProperty("_ShadowBlurMask", props);
                shadowStrength = FindProperty("_ShadowStrength", props);
                shadowStrengthMask = FindProperty("_ShadowStrengthMask", props);
                shadowColor = FindProperty("_ShadowColor", props);
                shadowColorTex = FindProperty("_ShadowColorTex", props);
                shadow2ndBorder = FindProperty("_Shadow2ndBorder", props);
                shadow2ndBlur = FindProperty("_Shadow2ndBlur", props);
                shadow2ndColor = FindProperty("_Shadow2ndColor", props);
                shadow2ndColorTex = FindProperty("_Shadow2ndColorTex", props);
                shadowMainStrength = FindProperty("_ShadowMainStrength", props);
                shadowEnvStrength = FindProperty("_ShadowEnvStrength", props);
                shadowBorderColor = FindProperty("_ShadowBorderColor", props);
                shadowBorderRange = FindProperty("_ShadowBorderRange", props);
                shadowReceive = FindProperty("_ShadowReceive", props);
            distanceFade = FindProperty("_DistanceFade", props);
                distanceFadeColor = FindProperty("_DistanceFadeColor", props);
            // Encryption
                ignoreEncryption = FindProperty("_IgnoreEncryption", props);
                keys = FindProperty("_Keys", props);
            //useFur = FindProperty("_UseFur", props);
                furNoiseMask = FindProperty("_FurNoiseMask", props);
                furLengthMask = FindProperty("_FurLengthMask", props);
                furMask = FindProperty("_FurMask", props);
                furVectorTex = FindProperty("_FurVectorTex", props);
                furVectorScale = FindProperty("_FurVectorScale", props);
                furVector = FindProperty("_FurVector", props);
                furGravity = FindProperty("_FurGravity", props);
                furAO = FindProperty("_FurAO", props);
                vertexColor2FurVector = FindProperty("_VertexColor2FurVector", props);
                furLayerNum = FindProperty("_FurLayerNum", props);
                furCull = FindProperty("_FurCull", props);
                furSrcBlend = FindProperty("_FurSrcBlend", props);
                furDstBlend = FindProperty("_FurDstBlend", props);
                furSrcBlendAlpha = FindProperty("_FurSrcBlendAlpha", props);
                furDstBlendAlpha = FindProperty("_FurDstBlendAlpha", props);
                furBlendOp = FindProperty("_FurBlendOp", props);
                furBlendOpAlpha = FindProperty("_FurBlendOpAlpha", props);
                furSrcBlendFA = FindProperty("_FurSrcBlendFA", props);
                furDstBlendFA = FindProperty("_FurDstBlendFA", props);
                furSrcBlendAlphaFA = FindProperty("_FurSrcBlendAlphaFA", props);
                furDstBlendAlphaFA = FindProperty("_FurDstBlendAlphaFA", props);
                furBlendOpFA = FindProperty("_FurBlendOpFA", props);
                furBlendOpAlphaFA = FindProperty("_FurBlendOpAlphaFA", props);
                furZwrite = FindProperty("_FurZWrite", props);
                furZtest = FindProperty("_FurZTest", props);
                furStencilRef = FindProperty("_FurStencilRef", props);
                furStencilReadMask = FindProperty("_FurStencilReadMask", props);
                furStencilWriteMask = FindProperty("_FurStencilWriteMask", props);
                furStencilComp = FindProperty("_FurStencilComp", props);
                furStencilPass = FindProperty("_FurStencilPass", props);
                furStencilFail = FindProperty("_FurStencilFail", props);
                furStencilZFail = FindProperty("_FurStencilZFail", props);
                furOffsetFactor = FindProperty("_FurOffsetFactor", props);
                furOffsetUnits = FindProperty("_FurOffsetUnits", props);
                furColorMask = FindProperty("_FurColorMask", props);
        }

        void LoadGemProperties(MaterialProperty[] props)
        {
            invisible = FindProperty("_Invisible", props);
            asUnlit = FindProperty("_AsUnlit", props);
            vertexLightStrength = FindProperty("_VertexLightStrength", props);
            lightMinLimit = FindProperty("_LightMinLimit", props);
            mainTex_ScrollRotate = FindProperty("_MainTex_ScrollRotate", props);
                cull = FindProperty("_Cull", props);
                srcBlend = FindProperty("_SrcBlend", props);
                dstBlend = FindProperty("_DstBlend", props);
                srcBlendAlpha = FindProperty("_SrcBlendAlpha", props);
                dstBlendAlpha = FindProperty("_DstBlendAlpha", props);
                blendOp = FindProperty("_BlendOp", props);
                blendOpAlpha = FindProperty("_BlendOpAlpha", props);
                zwrite = FindProperty("_ZWrite", props);
                ztest = FindProperty("_ZTest", props);
                stencilRef = FindProperty("_StencilRef", props);
                stencilReadMask = FindProperty("_StencilReadMask", props);
                stencilWriteMask = FindProperty("_StencilWriteMask", props);
                stencilComp = FindProperty("_StencilComp", props);
                stencilPass = FindProperty("_StencilPass", props);
                stencilFail = FindProperty("_StencilFail", props);
                stencilZFail = FindProperty("_StencilZFail", props);
                offsetFactor = FindProperty("_OffsetFactor", props);
                offsetUnits = FindProperty("_OffsetUnits", props);
                colorMask = FindProperty("_ColorMask", props);
            //useMainTex = FindProperty("_UseMainTex", props);
                mainColor = FindProperty("_Color", props);
                mainTex = FindProperty("_MainTex", props);
            // Normal
            useBumpMap = FindProperty("_UseBumpMap", props);
                bumpMap = FindProperty("_BumpMap", props);
                bumpScale = FindProperty("_BumpScale", props);
            useBump2ndMap = FindProperty("_UseBump2ndMap", props);
                bump2ndMap = FindProperty("_Bump2ndMap", props);
                bump2ndScale = FindProperty("_Bump2ndScale", props);
                bump2ndScaleMask = FindProperty("_Bump2ndScaleMask", props);
            useMatCap = FindProperty("_UseMatCap", props);
                matcapTex = FindProperty("_MatCapTex", props);
                matcapColor = FindProperty("_MatCapColor", props);
                matcapBlend = FindProperty("_MatCapBlend", props);
                matcapBlendMask = FindProperty("_MatCapBlendMask", props);
                matcapEnableLighting = FindProperty("_MatCapEnableLighting", props);
                matcapBlendMode = FindProperty("_MatCapBlendMode", props);
                matcapApplyTransparency = FindProperty("_MatCapApplyTransparency", props);
                matcapZRotCancel = FindProperty("_MatCapZRotCancel", props);
                matcapCustomNormal = FindProperty("_MatCapCustomNormal", props);
                matcapBumpMap = FindProperty("_MatCapBumpMap", props);
                matcapBumpScale = FindProperty("_MatCapBumpScale", props);
            useMatCap2nd = FindProperty("_UseMatCap2nd", props);
                matcap2ndTex = FindProperty("_MatCap2ndTex", props);
                matcap2ndColor = FindProperty("_MatCap2ndColor", props);
                matcap2ndBlend = FindProperty("_MatCap2ndBlend", props);
                matcap2ndBlendMask = FindProperty("_MatCap2ndBlendMask", props);
                matcap2ndEnableLighting = FindProperty("_MatCap2ndEnableLighting", props);
                matcap2ndBlendMode = FindProperty("_MatCap2ndBlendMode", props);
                matcap2ndApplyTransparency = FindProperty("_MatCap2ndApplyTransparency", props);
                matcap2ndZRotCancel = FindProperty("_MatCap2ndZRotCancel", props);
                matcap2ndCustomNormal = FindProperty("_MatCap2ndCustomNormal", props);
                matcap2ndBumpMap = FindProperty("_MatCap2ndBumpMap", props);
                matcap2ndBumpScale = FindProperty("_MatCap2ndBumpScale", props);
            useRim = FindProperty("_UseRim", props);
                rimColor = FindProperty("_RimColor", props);
                rimColorTex = FindProperty("_RimColorTex", props);
                rimBorder = FindProperty("_RimBorder", props);
                rimBlur = FindProperty("_RimBlur", props);
                rimFresnelPower = FindProperty("_RimFresnelPower", props);
                rimEnableLighting = FindProperty("_RimEnableLighting", props);
                rimShadowMask = FindProperty("_RimShadowMask", props);
                rimApplyTransparency = FindProperty("_RimApplyTransparency", props);
                rimDirStrength = FindProperty("_RimDirStrength", props);
                rimDirRange = FindProperty("_RimDirRange", props);
                rimIndirRange = FindProperty("_RimIndirRange", props);
                rimIndirColor = FindProperty("_RimIndirColor", props);
                rimIndirBorder = FindProperty("_RimIndirBorder", props);
                rimIndirBlur = FindProperty("_RimIndirBlur", props);
            useGlitter = FindProperty("_UseGlitter", props);
                glitterUVMode = FindProperty("_GlitterUVMode", props);
                glitterColor = FindProperty("_GlitterColor", props);
                glitterColorTex = FindProperty("_GlitterColorTex", props);
                glitterMainStrength = FindProperty("_GlitterMainStrength", props);
                glitterEnableLighting = FindProperty("_GlitterEnableLighting", props);
                glitterShadowMask = FindProperty("_GlitterShadowMask", props);
                glitterApplyTransparency = FindProperty("_GlitterApplyTransparency", props);
                glitterParams1 = FindProperty("_GlitterParams1", props);
                glitterParams2 = FindProperty("_GlitterParams2", props);
                glitterVRParallaxStrength = FindProperty("_GlitterVRParallaxStrength", props);
                ignoreEncryption = FindProperty("_IgnoreEncryption", props);
                keys = FindProperty("_Keys", props);
                refractionStrength = FindProperty("_RefractionStrength", props);
                refractionFresnelPower = FindProperty("_RefractionFresnelPower", props);
            useEmission = FindProperty("_UseEmission", props);
                emissionColor = FindProperty("_EmissionColor", props);
                emissionMap = FindProperty("_EmissionMap", props);
                emissionMap_ScrollRotate = FindProperty("_EmissionMap_ScrollRotate", props);
                emissionBlend = FindProperty("_EmissionBlend", props);
                emissionBlendMask = FindProperty("_EmissionBlendMask", props);
                emissionBlendMask_ScrollRotate = FindProperty("_EmissionBlendMask_ScrollRotate", props);
                emissionBlink = FindProperty("_EmissionBlink", props);
                emissionUseGrad = FindProperty("_EmissionUseGrad", props);
                emissionGradTex = FindProperty("_EmissionGradTex", props);
                emissionGradSpeed = FindProperty("_EmissionGradSpeed", props);
                emissionParallaxDepth = FindProperty("_EmissionParallaxDepth", props);
                emissionFluorescence = FindProperty("_EmissionFluorescence", props);
            useEmission2nd = FindProperty("_UseEmission2nd", props);
                emission2ndColor = FindProperty("_Emission2ndColor", props);
                emission2ndMap = FindProperty("_Emission2ndMap", props);
                emission2ndMap_ScrollRotate = FindProperty("_Emission2ndMap_ScrollRotate", props);
                emission2ndBlend = FindProperty("_Emission2ndBlend", props);
                emission2ndBlendMask = FindProperty("_Emission2ndBlendMask", props);
                emission2ndBlendMask_ScrollRotate = FindProperty("_Emission2ndBlendMask_ScrollRotate", props);
                emission2ndBlink = FindProperty("_Emission2ndBlink", props);
                emission2ndUseGrad = FindProperty("_Emission2ndUseGrad", props);
                emission2ndGradTex = FindProperty("_Emission2ndGradTex", props);
                emission2ndGradSpeed = FindProperty("_Emission2ndGradSpeed", props);
                emission2ndParallaxDepth = FindProperty("_Emission2ndParallaxDepth", props);
                emission2ndFluorescence = FindProperty("_Emission2ndFluorescence", props);
            useAudioLink = FindProperty("_UseAudioLink", props);
                audioLinkUVMode = FindProperty("_AudioLinkUVMode", props);
                audioLinkUVParams = FindProperty("_AudioLinkUVParams", props);
                audioLinkMask = FindProperty("_AudioLinkMask", props);
                audioLink2Main2nd = FindProperty("_AudioLink2Main2nd", props);
                audioLink2Main3rd = FindProperty("_AudioLink2Main3rd", props);
                audioLink2Emission = FindProperty("_AudioLink2Emission", props);
                audioLink2Emission2nd = FindProperty("_AudioLink2Emission2nd", props);
                audioLink2Vertex = FindProperty("_AudioLink2Vertex", props);
                audioLinkVertexUVMode = FindProperty("_AudioLinkVertexUVMode", props);
                audioLinkVertexUVParams = FindProperty("_AudioLinkVertexUVParams", props);
                audioLinkVertexStart = FindProperty("_AudioLinkVertexStart", props);
                audioLinkVertexStrength = FindProperty("_AudioLinkVertexStrength", props);
                audioLinkAsLocal = FindProperty("_AudioLinkAsLocal", props);
                audioLinkLocalMap = FindProperty("_AudioLinkLocalMap", props);
                audioLinkLocalMapParams = FindProperty("_AudioLinkLocalMapParams", props);
            //useGem = FindProperty("_UseGem", props);
                gemChromaticAberration = FindProperty("_GemChromaticAberration", props);
                gemEnvContrast = FindProperty("_GemEnvContrast", props);
                gemEnvColor = FindProperty("_GemEnvColor", props);
                gemParticleLoop = FindProperty("_GemParticleLoop", props);
                gemParticleColor = FindProperty("_GemParticleColor", props);
                gemVRParallaxStrength = FindProperty("_GemVRParallaxStrength", props);
                smoothness = FindProperty("_Smoothness", props);
                smoothnessTex = FindProperty("_SmoothnessTex", props);
                reflectance = FindProperty("_Reflectance", props);
        }

        void LoadFakeShadowProperties(MaterialProperty[] props)
        {
            invisible = FindProperty("_Invisible", props);
                cull = FindProperty("_Cull", props);
                srcBlend = FindProperty("_SrcBlend", props);
                dstBlend = FindProperty("_DstBlend", props);
                srcBlendAlpha = FindProperty("_SrcBlendAlpha", props);
                dstBlendAlpha = FindProperty("_DstBlendAlpha", props);
                blendOp = FindProperty("_BlendOp", props);
                blendOpAlpha = FindProperty("_BlendOpAlpha", props);
                zwrite = FindProperty("_ZWrite", props);
                ztest = FindProperty("_ZTest", props);
                stencilRef = FindProperty("_StencilRef", props);
                stencilReadMask = FindProperty("_StencilReadMask", props);
                stencilWriteMask = FindProperty("_StencilWriteMask", props);
                stencilComp = FindProperty("_StencilComp", props);
                stencilPass = FindProperty("_StencilPass", props);
                stencilFail = FindProperty("_StencilFail", props);
                stencilZFail = FindProperty("_StencilZFail", props);
                offsetFactor = FindProperty("_OffsetFactor", props);
                offsetUnits = FindProperty("_OffsetUnits", props);
                colorMask = FindProperty("_ColorMask", props);
            //useMainTex = FindProperty("_UseMainTex", props);
                mainColor = FindProperty("_Color", props);
                mainTex = FindProperty("_MainTex", props);
                ignoreEncryption = FindProperty("_IgnoreEncryption", props);
                keys = FindProperty("_Keys", props);
                fakeShadowVector = FindProperty("_FakeShadowVector", props);
        }

        void LoadLiteProperties(MaterialProperty[] props)
        {
            invisible = FindProperty("_Invisible", props);
            asUnlit = FindProperty("_AsUnlit", props);
            cutoff = FindProperty("_Cutoff", props);
            flipNormal = FindProperty("_FlipNormal", props);
            backfaceForceShadow = FindProperty("_BackfaceForceShadow", props);
            vertexLightStrength = FindProperty("_VertexLightStrength", props);
            lightMinLimit = FindProperty("_LightMinLimit", props);
            mainTex_ScrollRotate = FindProperty("_MainTex_ScrollRotate", props);
            triMask = FindProperty("_TriMask", props);
                cull = FindProperty("_Cull", props);
                srcBlend = FindProperty("_SrcBlend", props);
                dstBlend = FindProperty("_DstBlend", props);
                srcBlendAlpha = FindProperty("_SrcBlendAlpha", props);
                dstBlendAlpha = FindProperty("_DstBlendAlpha", props);
                blendOp = FindProperty("_BlendOp", props);
                blendOpAlpha = FindProperty("_BlendOpAlpha", props);
                srcBlendFA = FindProperty("_SrcBlendFA", props);
                dstBlendFA = FindProperty("_DstBlendFA", props);
                srcBlendAlphaFA = FindProperty("_SrcBlendAlphaFA", props);
                dstBlendAlphaFA = FindProperty("_DstBlendAlphaFA", props);
                blendOpFA = FindProperty("_BlendOpFA", props);
                blendOpAlphaFA = FindProperty("_BlendOpAlphaFA", props);
                zwrite = FindProperty("_ZWrite", props);
                ztest = FindProperty("_ZTest", props);
                stencilRef = FindProperty("_StencilRef", props);
                stencilReadMask = FindProperty("_StencilReadMask", props);
                stencilWriteMask = FindProperty("_StencilWriteMask", props);
                stencilComp = FindProperty("_StencilComp", props);
                stencilPass = FindProperty("_StencilPass", props);
                stencilFail = FindProperty("_StencilFail", props);
                stencilZFail = FindProperty("_StencilZFail", props);
                offsetFactor = FindProperty("_OffsetFactor", props);
                offsetUnits = FindProperty("_OffsetUnits", props);
                colorMask = FindProperty("_ColorMask", props);
            // Main
            //useMainTex = FindProperty("_UseMainTex", props);
                mainColor = FindProperty("_Color", props);
                mainTex = FindProperty("_MainTex", props);
            // Shadow
            useShadow = FindProperty("_UseShadow", props);
                shadowBorder = FindProperty("_ShadowBorder", props);
                shadowBlur = FindProperty("_ShadowBlur", props);
                shadowColorTex = FindProperty("_ShadowColorTex", props);
                shadow2ndBorder = FindProperty("_Shadow2ndBorder", props);
                shadow2ndBlur = FindProperty("_Shadow2ndBlur", props);
                shadow2ndColorTex = FindProperty("_Shadow2ndColorTex", props);
                shadowEnvStrength = FindProperty("_ShadowEnvStrength", props);
                shadowBorderColor = FindProperty("_ShadowBorderColor", props);
                shadowBorderRange = FindProperty("_ShadowBorderRange", props);
            // Outline
            if(isOutl)
            {
                outlineColor = FindProperty("_OutlineColor", props);
                outlineTex = FindProperty("_OutlineTex", props);
                outlineTex_ScrollRotate = FindProperty("_OutlineTex_ScrollRotate", props);
                outlineWidth = FindProperty("_OutlineWidth", props);
                outlineWidthMask = FindProperty("_OutlineWidthMask", props);
                outlineFixWidth = FindProperty("_OutlineFixWidth", props);
                outlineVertexR2Width = FindProperty("_OutlineVertexR2Width", props);
                outlineEnableLighting = FindProperty("_OutlineEnableLighting", props);
                outlineCull = FindProperty("_OutlineCull", props);
                outlineSrcBlend = FindProperty("_OutlineSrcBlend", props);
                outlineDstBlend = FindProperty("_OutlineDstBlend", props);
                outlineSrcBlendAlpha = FindProperty("_OutlineSrcBlendAlpha", props);
                outlineDstBlendAlpha = FindProperty("_OutlineDstBlendAlpha", props);
                outlineBlendOp = FindProperty("_OutlineBlendOp", props);
                outlineBlendOpAlpha = FindProperty("_OutlineBlendOpAlpha", props);
                outlineSrcBlendFA = FindProperty("_OutlineSrcBlendFA", props);
                outlineDstBlendFA = FindProperty("_OutlineDstBlendFA", props);
                outlineSrcBlendAlphaFA = FindProperty("_OutlineSrcBlendAlphaFA", props);
                outlineDstBlendAlphaFA = FindProperty("_OutlineDstBlendAlphaFA", props);
                outlineBlendOpFA = FindProperty("_OutlineBlendOpFA", props);
                outlineBlendOpAlphaFA = FindProperty("_OutlineBlendOpAlphaFA", props);
                outlineZwrite = FindProperty("_OutlineZWrite", props);
                outlineZtest = FindProperty("_OutlineZTest", props);
                outlineStencilRef = FindProperty("_OutlineStencilRef", props);
                outlineStencilReadMask = FindProperty("_OutlineStencilReadMask", props);
                outlineStencilWriteMask = FindProperty("_OutlineStencilWriteMask", props);
                outlineStencilComp = FindProperty("_OutlineStencilComp", props);
                outlineStencilPass = FindProperty("_OutlineStencilPass", props);
                outlineStencilFail = FindProperty("_OutlineStencilFail", props);
                outlineStencilZFail = FindProperty("_OutlineStencilZFail", props);
                outlineOffsetFactor = FindProperty("_OutlineOffsetFactor", props);
                outlineOffsetUnits = FindProperty("_OutlineOffsetUnits", props);
                outlineColorMask = FindProperty("_OutlineColorMask", props);
            }
            useMatCap = FindProperty("_UseMatCap", props);
                matcapTex = FindProperty("_MatCapTex", props);
                matcapMul = FindProperty("_MatCapMul", props);
                matcapZRotCancel = FindProperty("_MatCapZRotCancel", props);
            useRim = FindProperty("_UseRim", props);
                rimColor = FindProperty("_RimColor", props);
                rimBorder = FindProperty("_RimBorder", props);
                rimBlur = FindProperty("_RimBlur", props);
                rimFresnelPower = FindProperty("_RimFresnelPower", props);
                rimShadowMask = FindProperty("_RimShadowMask", props);
            useEmission = FindProperty("_UseEmission", props);
                emissionColor = FindProperty("_EmissionColor", props);
                emissionMap = FindProperty("_EmissionMap", props);
                emissionMap_ScrollRotate = FindProperty("_EmissionMap_ScrollRotate", props);
                emissionBlink = FindProperty("_EmissionBlink", props);
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Rendering Pipeline
        public static void RewriteShaderRP(string shaderPath, lilRenderPipeline lilRP)
        {
            string path = shaderPath;
            StreamReader sr = new StreamReader(path);
            string s = sr.ReadToEnd();
            sr.Close();
            RewriteBRP(ref s, lilRP == lilRenderPipeline.BRP);
            RewriteLWRP(ref s, lilRP == lilRenderPipeline.LWRP);
            RewriteURP(ref s, lilRP == lilRenderPipeline.URP);
            StreamWriter sw = new StreamWriter(path,false);
            sw.Write(s);
            sw.Close();
        }

        static void RewriteBRP(ref string s, bool isActive)
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

        static void RewriteLWRP(ref string s, bool isActive)
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

        static void RewriteURP(ref string s, bool isActive)
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

        //------------------------------------------------------------------------------------------------------------------------------
        // Initialize
        static void InitializeShaders()
        {
            lts       = Shader.Find("lilToon");
            ltsc      = Shader.Find("Hidden/lilToonCutout");
            ltst      = Shader.Find("Hidden/lilToonTransparent");

            ltso      = Shader.Find("Hidden/lilToonOutline");
            ltsco     = Shader.Find("Hidden/lilToonCutoutOutline");
            ltsto     = Shader.Find("Hidden/lilToonTransparentOutline");

            ltstess   = Shader.Find("Hidden/lilToonTessellation");
            ltstessc  = Shader.Find("Hidden/lilToonTessellationCutout");
            ltstesst  = Shader.Find("Hidden/lilToonTessellationTransparent");

            ltstesso  = Shader.Find("Hidden/lilToonTessellationOutline");
            ltstessco = Shader.Find("Hidden/lilToonTessellationCutoutOutline");
            ltstessto = Shader.Find("Hidden/lilToonTessellationTransparentOutline");

            ltsl      = Shader.Find("Hidden/lilToonLite");
            ltslc     = Shader.Find("Hidden/lilToonLiteCutout");
            ltslt     = Shader.Find("Hidden/lilToonLiteTransparent");

            ltslo     = Shader.Find("Hidden/lilToonLiteOutline");
            ltslco    = Shader.Find("Hidden/lilToonLiteCutoutOutline");
            ltslto    = Shader.Find("Hidden/lilToonLiteTransparentOutline");

            ltsref    = Shader.Find("Hidden/lilToonRefraction");
            ltsrefb   = Shader.Find("Hidden/lilToonRefractionBlur");
            ltsfur    = Shader.Find("Hidden/lilToonFur");
            ltsfurc   = Shader.Find("Hidden/lilToonFurCutout");

            ltsgem    = Shader.Find("Hidden/lilToonGem");

            ltsfs     = Shader.Find("_lil/lilToonFakeShadow");

            ltsbaker  = Shader.Find("Hidden/ltsother_baker");
            ltspo     = Shader.Find("Hidden/ltspass_opaque");
            ltspc     = Shader.Find("Hidden/ltspass_cutout");
            ltspt     = Shader.Find("Hidden/ltspass_transparent");
            ltsptesso = Shader.Find("Hidden/ltspass_tess_opaque");
            ltsptessc = Shader.Find("Hidden/ltspass_tess_cutout");
            ltsptesst = Shader.Find("Hidden/ltspass_tess_transparent");

            mtoon     = Shader.Find("VRM/MToon");
        }

        public static void InitializeShaderSetting(ref lilToonSetting shaderSetting)
        {
            if(shaderSetting != null) return;
            string shaderSettingPath = lilToonInspector.GetShaderSettingPath();
            shaderSetting = (lilToonSetting)AssetDatabase.LoadAssetAtPath(shaderSettingPath, typeof(lilToonSetting));
            if(shaderSetting == null)
            {
                string settingFolderPath = lilToonInspector.GetSettingFolderPath();
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
                shaderSetting.LIL_FEATURE_RECEIVE_SHADOW = false;
                shaderSetting.LIL_FEATURE_EMISSION_1ST = true;
                shaderSetting.LIL_FEATURE_EMISSION_2ND = false;
                shaderSetting.LIL_FEATURE_EMISSION_UV = false;
                shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV = false;
                shaderSetting.LIL_FEATURE_EMISSION_MASK_UV = false;
                shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV = false;
                shaderSetting.LIL_FEATURE_EMISSION_GRADATION = false;
                shaderSetting.LIL_FEATURE_NORMAL_1ST = true;
                shaderSetting.LIL_FEATURE_NORMAL_2ND = false;
                shaderSetting.LIL_FEATURE_REFLECTION = false;
                shaderSetting.LIL_FEATURE_MATCAP = true;
                shaderSetting.LIL_FEATURE_MATCAP_2ND = false;
                shaderSetting.LIL_FEATURE_RIMLIGHT = true;
                shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION = false;
                shaderSetting.LIL_FEATURE_GLITTER = false;
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
                shaderSetting.LIL_FEATURE_TEX_LAYER_MASK = false;
                shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE = false;
                shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR = false;
                shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER = false;
                shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH = true;
                shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST = false;
                shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND = false;
                shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK = false;
                shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK = false;
                shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS = false;
                shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC = false;
                shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR = false;
                shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK = true;
                shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP = false;
                shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR = true;
                shaderSetting.LIL_FEATURE_TEX_AUDIOLINK_MASK = false;
                shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE = false;
                shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR = true;
                shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH = true;
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

        static void InitializeBool(ref bool value, bool defaultValue)
        {
            //if(value == null) value = defaultValue;
            value = defaultValue;
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
            shaderSetting.LIL_FEATURE_RECEIVE_SHADOW = false;
            shaderSetting.LIL_FEATURE_EMISSION_1ST = false;
            shaderSetting.LIL_FEATURE_EMISSION_2ND = false;
            shaderSetting.LIL_FEATURE_EMISSION_UV = false;
            shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV = false;
            shaderSetting.LIL_FEATURE_EMISSION_MASK_UV = false;
            shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV = false;
            shaderSetting.LIL_FEATURE_EMISSION_GRADATION = false;
            shaderSetting.LIL_FEATURE_NORMAL_1ST = false;
            shaderSetting.LIL_FEATURE_NORMAL_2ND = false;
            shaderSetting.LIL_FEATURE_REFLECTION = false;
            shaderSetting.LIL_FEATURE_MATCAP = false;
            shaderSetting.LIL_FEATURE_MATCAP_2ND = false;
            shaderSetting.LIL_FEATURE_RIMLIGHT = false;
            shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION = false;
            shaderSetting.LIL_FEATURE_GLITTER = false;
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
            shaderSetting.LIL_FEATURE_TEX_LAYER_MASK = false;
            shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE = false;
            shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR = false;
            shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER = false;
            shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH = false;
            shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST = false;
            shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND = false;
            shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK = false;
            shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK = false;
            shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS = false;
            shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC = false;
            shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR = false;
            shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK = false;
            shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP = false;
            shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR = false;
            shaderSetting.LIL_FEATURE_TEX_AUDIOLINK_MASK = false;
            shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE = false;
            shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR = false;
            shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH = false;
            shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL = false;
            shaderSetting.LIL_FEATURE_TEX_FUR_MASK = false;
            shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH = false;
            shaderSetting.LIL_FEATURE_TEX_TESSELLATION = false;
            EditorUtility.SetDirty(shaderSetting);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        static void SelectEditorMode()
        {
            string[] sEditorModeList = {GetLoc("sEditorModeSimple"),GetLoc("sEditorModeAdvanced"),GetLoc("sEditorModePreset")};
            edSet.editorMode = (EditorMode)EditorGUILayout.Popup(GetLoc("sEditorMode"), (int)edSet.editorMode, sEditorModeList);
            switch(edSet.editorMode)
            {
                case EditorMode.Simple:
                    EditorGUILayout.HelpBox(GetLoc("sHelpSimple"),MessageType.Info);
                    break;
                case EditorMode.Advanced:
                    EditorGUILayout.HelpBox(GetLoc("sHelpAdvanced"),MessageType.Info);
                    break;
                case EditorMode.Preset:
                    EditorGUILayout.HelpBox(GetLoc("sHelpPreset"),MessageType.Info);
                    break;
            }
        }

        public static void ReimportPassShaders()
        {
            string[] shaderFolderPaths = lilToonInspector.GetShaderFolderPaths();
            string[] shaderGuids = AssetDatabase.FindAssets("t:shader", shaderFolderPaths);
            foreach(string shaderGuid in shaderGuids)
            {
                string shaderPath = AssetDatabase.GUIDToAssetPath(shaderGuid);
                if(shaderPath.Contains("ltspass")) AssetDatabase.ImportAsset(shaderPath);
                if(shaderPath.Contains("fur")) AssetDatabase.ImportAsset(shaderPath);
                if(shaderPath.Contains("ref")) AssetDatabase.ImportAsset(shaderPath);
            }
        }

        public static void InitializeSettingHLSL(ref lilToonSetting shaderSetting)
        {
            string[] shaderFolderPaths = lilToonInspector.GetShaderFolderPaths();
            string shaderSettingHLSLPath = lilToonInspector.GetShaderSettingHLSLPath();
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
                string[] shaderGuids = AssetDatabase.FindAssets("t:shader", shaderFolderPaths);
                if(shaderGuids.Length > 33)
                {
                    // Render Pipeline
                    // BRP : null
                    // LWRP : LightweightPipeline.LightweightRenderPipelineAsset
                    // URP : Universal.UniversalRenderPipelineAsset
                    lilRenderPipeline lilRP = lilRenderPipeline.BRP;
                    if(UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null)
                    {
                        string renderPipelineName = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset.ToString();
                        if(String.IsNullOrEmpty(renderPipelineName))        lilRP = lilRenderPipeline.BRP;
                        else if(renderPipelineName.Contains("Lightweight")) lilRP = lilRenderPipeline.LWRP;
                        else if(renderPipelineName.Contains("Universal"))   lilRP = lilRenderPipeline.URP;
                    }
                    else
                    {
                        lilRP = lilRenderPipeline.BRP;
                    }
                    foreach(string shaderGuid in shaderGuids)
                    {
                        string shaderPath = AssetDatabase.GUIDToAssetPath(shaderGuid);
                        RewriteShaderRP(shaderPath, lilRP);
                    }
                    string shaderPipelinePath = lilToonInspector.GetShaderPipelinePath();
                    RewriteShaderRP(shaderPipelinePath, lilRP);
                }

                // Get materials
                string[] guids = AssetDatabase.FindAssets("t:material");
                foreach(string guid in guids)
                {
                    Material material = (Material)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Material));
                    SetupShaderSettingFromMaterial(material, ref shaderSetting);
                }

                // Get animations
                string[] clipGuids = AssetDatabase.FindAssets("t:animationclip");
                foreach(string guid in clipGuids)
                {
                    AnimationClip clip = (AnimationClip)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(AnimationClip));
                    SetupShaderSettingFromAnimationClip(clip, ref shaderSetting);
                }

                if(shaderSetting != null)
                {
                    EditorUtility.SetDirty(shaderSetting);
                    AssetDatabase.SaveAssets();
                    ApplyShaderSetting(shaderSetting);
                    Debug.Log("Apply shader setting");
                }

                AssetDatabase.ImportAsset(shaderSettingHLSLPath);
                ReimportPassShaders();
                AssetDatabase.Refresh();
                Debug.Log("Reimport pass shaders");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Editor
        public static string GetLoc(string value)
        {
            if(loc.ContainsKey(value)) return loc[value];
            return value;
        }

        static void VersionCheck()
        {
            if(String.IsNullOrEmpty(latestVersion.latest_vertion_name))
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
                if(!String.IsNullOrEmpty(s) && s.Contains("latest_vertion_name") && s.Contains("latest_vertion_value"))
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

        public static void ApplyEditorSettingTemp()
        {
            if(String.IsNullOrEmpty(edSet.languageNames))
            {
                if(!File.Exists(editorSettingTempPath))
                {
                    return;
                }
                StreamReader sr = new StreamReader(editorSettingTempPath);
                string s = sr.ReadToEnd();
                sr.Close();
                if(!String.IsNullOrEmpty(s))
                {
                    EditorJsonUtility.FromJsonOverwrite(s,edSet);
                }
            }
        }

        static void SaveEditorSettingTemp()
        {
            StreamWriter sw = new StreamWriter(editorSettingTempPath,false);
            sw.Write(EditorJsonUtility.ToJson(edSet));
            sw.Close();
        }

        public static void DrawLine()
        {
            EditorGUI.DrawRect(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, 1)), lineColor);
        }

        static void DrawHelpButton(string helpAnchor)
        {
            Rect position = GUILayoutUtility.GetLastRect();
            position.x += position.width - 24;
            position.width = 24;
            GUIContent icon = EditorGUIUtility.IconContent("_Help");
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            if(GUI.Button(position, icon, style)){
                Application.OpenURL(GetLoc("sManualURL") + helpAnchor);
            }
        }

        static void DrawWebButton(string text, string URL)
        {
            Rect position = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect());
            GUIContent icon = EditorGUIUtility.IconContent("BuildSettings.Web.Small");
            icon.text = text;
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.padding = new RectOffset();
            if(GUI.Button(position, icon, style)){
                Application.OpenURL(URL);
            }
        }

        static void DrawWebPages()
        {
            VersionCheck();
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontStyle = FontStyle.Bold;
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

            position.x += 10;
            edSet.isShowWebPages = EditorGUI.Foldout(position, edSet.isShowWebPages, "");
            if(edSet.isShowWebPages)
            {
                EditorGUI.indentLevel++;
                DrawWebButton("BOOTH", boothURL);
                DrawWebButton("GitHub", githubURL);
                EditorGUI.indentLevel--;
            }
        }

        static void DrawHelpPages()
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontStyle = FontStyle.Bold;
            EditorGUI.indentLevel++;
            Rect position = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(position, GetLoc("sWhenInTrouble"), labelStyle);
            EditorGUI.indentLevel--;

            position.x += 10;
            edSet.isShowHelpPages = EditorGUI.Foldout(position, edSet.isShowHelpPages, "");
            if(edSet.isShowHelpPages)
            {
                EditorGUI.indentLevel++;
                DrawWebButton(GetLoc("sCommonProblems"), GetLoc("sReadmeURL") + GetLoc("sReadmeAnchorProblem"));
                EditorGUI.indentLevel--;
            }
        }

        static bool Foldout(string title, string help, bool display)
        {
            GUIStyle style = new GUIStyle("ShurikenModuleTitle");
            #if UNITY_2019_1_OR_NEWER
                style.fontSize = 12;
            #else
                style.fontSize = 11;
            #endif
            style.border = new RectOffset(15, 7, 4, 4);
            style.contentOffset = new Vector2(20f, -2f);
            style.fixedHeight = 22;
            Rect rect = GUILayoutUtility.GetRect(16f, 20f, style);
            GUI.Box(rect, new GUIContent(title, help), style);

            Event e = Event.current;

            Rect toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            if(e.type == EventType.Repaint) {
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
            }

            rect.width -= 24;
            if(e.type == EventType.MouseDown & rect.Contains(e.mousePosition)) {
                display = !display;
                e.Use();
            }

            return display;
        }

        static int selectLang(int lnum)
        {
            int outnum = lnum;
            outnum = InitializeLanguage(outnum);

            // Select language
            string[] langName = edSet.languageNames.Split('\t');
            outnum = EditorGUILayout.Popup("Language", outnum, langName);

            // Load language
            if(outnum != lnum)
            {
                string langPath = AssetDatabase.GUIDToAssetPath("fa3d370c558629141a6a40c7d84691ed");
                if(String.IsNullOrEmpty(langPath) || !File.Exists(langPath)) return outnum;
                StreamReader sr = new StreamReader(langPath);
                string langBuf = sr.ReadToEnd();
                sr.Close();

                string[] langData = langBuf.Split('\n');
                edSet.languageNames = langData[0].Substring(langData[0].IndexOf("\t")+1);
                edSet.languageName = edSet.languageNames.Split('\t')[outnum];
                for(int i = 0; i < langData.Length; i++)
                {
                    string[] lineContents = langData[i].Split('\t');
                    loc[lineContents[0]] = lineContents[outnum+1];
                }
            }

            if(!String.IsNullOrEmpty(GetLoc("sLanguageWarning"))) EditorGUILayout.HelpBox(GetLoc("sLanguageWarning"),MessageType.Warning);

            return outnum;
        }

        public static int InitializeLanguage(int lnum)
        {
            if(lnum == -1)
            {
                if(Application.systemLanguage == SystemLanguage.Japanese)                   lnum = 1;
                else if(Application.systemLanguage == SystemLanguage.Korean)                lnum = 2;
                else if(Application.systemLanguage == SystemLanguage.ChineseSimplified)     lnum = 3;
                else if(Application.systemLanguage == SystemLanguage.ChineseTraditional)    lnum = 4;
                else                                                                        lnum = 0;
            }

            if(loc.Count == 0)
            {
                string langPath = AssetDatabase.GUIDToAssetPath("fa3d370c558629141a6a40c7d84691ed");
                if(String.IsNullOrEmpty(langPath) || !File.Exists(langPath)) return lnum;
                StreamReader sr = new StreamReader(langPath);
                string langBuf = sr.ReadToEnd();
                sr.Close();

                string[] langData = langBuf.Split('\n');
                edSet.languageNames = langData[0].Substring(langData[0].IndexOf("\t")+1);
                edSet.languageName = edSet.languageNames.Split('\t')[lnum];
                for(int i = 0; i < langData.Length; i++)
                {
                    string[] lineContents = langData[i].Split('\t');
                    loc[lineContents[0]] = lineContents[lnum+1];
                }
            }

            return lnum;
        }

        static void ShaderSettingGUI()
        {
            GUIStyle applyButton = new GUIStyle(GUI.skin.button);
            applyButton.normal.textColor = Color.red;
            applyButton.fontStyle = FontStyle.Bold;

            lilToggleGUI(GetLoc("sSettingCancelAutoScan"), ref shaderSetting.shouldNotScan);
            lilToggleGUI(GetLoc("sSettingLock"), ref shaderSetting.isLocked);
            GUI.enabled = !shaderSetting.isLocked;

            // Apply Button
            if(edSet.isShaderSettingChanged && GUILayout.Button("Apply", applyButton))
            {
                ApplyShaderSetting(shaderSetting);
                edSet.isShaderSettingChanged = false;
            }
            DrawLine();

            EditorGUI.BeginChangeCheck();

            lilToggleGUI(GetLoc("sSettingAnimateMainUV"), ref shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV);
            DrawLine();

            lilToggleGUI(GetLoc("sSettingMainToneCorrection"), ref shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION);
            lilToggleGUI(GetLoc("sSettingMainGradationMap"), ref shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP);
            lilToggleGUI(GetLoc("sSettingMain2nd"), ref shaderSetting.LIL_FEATURE_MAIN2ND);
            lilToggleGUI(GetLoc("sSettingMain3rd"), ref shaderSetting.LIL_FEATURE_MAIN3RD);
            if(shaderSetting.LIL_FEATURE_MAIN2ND || shaderSetting.LIL_FEATURE_MAIN3RD)
            {
                EditorGUI.indentLevel++;
                lilToggleGUI(GetLoc("sSettingDecal"), ref shaderSetting.LIL_FEATURE_DECAL);
                lilToggleGUI(GetLoc("sSettingAnimateDecal"), ref shaderSetting.LIL_FEATURE_ANIMATE_DECAL);
                lilToggleGUI(GetLoc("sSettingTexLayerMask"), ref shaderSetting.LIL_FEATURE_TEX_LAYER_MASK);
                lilToggleGUI(GetLoc("sSettingLayerDissolve"), ref shaderSetting.LIL_FEATURE_LAYER_DISSOLVE);
                if(shaderSetting.LIL_FEATURE_LAYER_DISSOLVE)
                {
                    EditorGUI.indentLevel++;
                    lilToggleGUI(GetLoc("sSettingTexDissolveNoise"), ref shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
            DrawLine();

            lilToggleGUI(GetLoc("sSettingAlphaMask"), ref shaderSetting.LIL_FEATURE_ALPHAMASK);
            DrawLine();

            lilToggleGUI(GetLoc("sSettingShadow"), ref shaderSetting.LIL_FEATURE_SHADOW);
            if(shaderSetting.LIL_FEATURE_SHADOW)
            {
                EditorGUI.indentLevel++;
                lilToggleGUI(GetLoc("sSettingReceiveShadow"), ref shaderSetting.LIL_FEATURE_RECEIVE_SHADOW);
                lilToggleGUI(GetLoc("sSettingTexShadowBlur"), ref shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR);
                lilToggleGUI(GetLoc("sSettingTexShadowBorder"), ref shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER);
                lilToggleGUI(GetLoc("sSettingTexShadowStrength"), ref shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH);
                lilToggleGUI(GetLoc("sSettingTexShadow1st"), ref shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST);
                lilToggleGUI(GetLoc("sSettingTexShadow2nd"), ref shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            lilToggleGUI(GetLoc("sSettingEmission1st"), ref shaderSetting.LIL_FEATURE_EMISSION_1ST);
            lilToggleGUI(GetLoc("sSettingEmission2nd"), ref shaderSetting.LIL_FEATURE_EMISSION_2ND);
            if(shaderSetting.LIL_FEATURE_EMISSION_1ST || shaderSetting.LIL_FEATURE_EMISSION_2ND)
            {
                EditorGUI.indentLevel++;
                lilToggleGUI(GetLoc("sSettingEmissionUV"), ref shaderSetting.LIL_FEATURE_EMISSION_UV);
                lilToggleGUI(GetLoc("sSettingAnimateEmissionUV"), ref shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV);
                lilToggleGUI(GetLoc("sSettingTexEmissionMask"), ref shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK);
                if(shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK)
                {
                    EditorGUI.indentLevel++;
                    lilToggleGUI(GetLoc("sSettingEmissionMaskUV"), ref shaderSetting.LIL_FEATURE_EMISSION_MASK_UV);
                    lilToggleGUI(GetLoc("sSettingAnimateEmissionMaskUV"), ref shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV);
                    EditorGUI.indentLevel--;
                }
                lilToggleGUI(GetLoc("sSettingEmissionGradation"), ref shaderSetting.LIL_FEATURE_EMISSION_GRADATION);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            lilToggleGUI(GetLoc("sSettingNormal1st"), ref shaderSetting.LIL_FEATURE_NORMAL_1ST);
            lilToggleGUI(GetLoc("sSettingNormal2nd"), ref shaderSetting.LIL_FEATURE_NORMAL_2ND);
            if(shaderSetting.LIL_FEATURE_NORMAL_2ND)
            {
                EditorGUI.indentLevel++;
                lilToggleGUI(GetLoc("sSettingTexNormalMask"), ref shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            lilToggleGUI(GetLoc("sSettingReflection"), ref shaderSetting.LIL_FEATURE_REFLECTION);
            if(shaderSetting.LIL_FEATURE_REFLECTION)
            {
                EditorGUI.indentLevel++;
                lilToggleGUI(GetLoc("sSettingTexReflectionSmoothness"), ref shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS);
                lilToggleGUI(GetLoc("sSettingTexReflectionMetallic"), ref shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC);
                lilToggleGUI(GetLoc("sSettingTexReflectionColor"), ref shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            lilToggleGUI(GetLoc("sSettingMatCap"), ref shaderSetting.LIL_FEATURE_MATCAP);
            lilToggleGUI(GetLoc("sSettingMatCap2nd"), ref shaderSetting.LIL_FEATURE_MATCAP_2ND);
            if(shaderSetting.LIL_FEATURE_MATCAP || shaderSetting.LIL_FEATURE_MATCAP_2ND)
            {
                EditorGUI.indentLevel++;
                lilToggleGUI(GetLoc("sSettingTexMatCapMask"), ref shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK);
                lilToggleGUI(GetLoc("sSettingTexMatCapNormal"), ref shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            lilToggleGUI(GetLoc("sSettingRimLight"), ref shaderSetting.LIL_FEATURE_RIMLIGHT);
            if(shaderSetting.LIL_FEATURE_RIMLIGHT)
            {
                EditorGUI.indentLevel++;
                lilToggleGUI(GetLoc("sSettingTexRimLightColor"), ref shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR);
                lilToggleGUI(GetLoc("sSettingTexRimLightDirection"), ref shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            lilToggleGUI(GetLoc("sSettingGlitter"), ref shaderSetting.LIL_FEATURE_GLITTER);
            DrawLine();

            lilToggleGUI(GetLoc("sSettingParallax"), ref shaderSetting.LIL_FEATURE_PARALLAX);
            if(shaderSetting.LIL_FEATURE_PARALLAX)
            {
                EditorGUI.indentLevel++;
                lilToggleGUI(GetLoc("sSettingPOM"), ref shaderSetting.LIL_FEATURE_POM);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            lilToggleGUI(GetLoc("sSettingClippingCanceller"), ref shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER);
            DrawLine();

            lilToggleGUI(GetLoc("sSettingDistanceFade"), ref shaderSetting.LIL_FEATURE_DISTANCE_FADE);
            DrawLine();

            lilToggleGUI(GetLoc("sSettingAudioLink"), ref shaderSetting.LIL_FEATURE_AUDIOLINK);
            if(shaderSetting.LIL_FEATURE_AUDIOLINK)
            {
                EditorGUI.indentLevel++;
                lilToggleGUI(GetLoc("sSettingAudioLinkVertex"), ref shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX);
                lilToggleGUI(GetLoc("sSettingAudioLinkMask"), ref shaderSetting.LIL_FEATURE_TEX_AUDIOLINK_MASK);
                lilToggleGUI(GetLoc("sSettingAudioLinkLocal"), ref shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            lilToggleGUI(GetLoc("sSettingDissolve"), ref shaderSetting.LIL_FEATURE_DISSOLVE);
            if(shaderSetting.LIL_FEATURE_DISSOLVE)
            {
                EditorGUI.indentLevel++;
                lilToggleGUI(GetLoc("sSettingTexDissolveNoise"), ref shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            if(String.IsNullOrEmpty(GetAvatarEncryptionPath()))
            {
                shaderSetting.LIL_FEATURE_ENCRYPTION = false;
            }
            else
            {
                lilToggleGUI(GetLoc("sSettingEncryption"), ref shaderSetting.LIL_FEATURE_ENCRYPTION);
                DrawLine();
            }

            lilToggleGUI(GetLoc("sSettingTexOutlineColor"), ref shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR);
            if(shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR)
            {
                EditorGUI.indentLevel++;
                lilToggleGUI(GetLoc("sSettingOutlineToneCorrection"), ref shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION);
                EditorGUI.indentLevel--;
            }
            lilToggleGUI(GetLoc("sSettingAnimateOutlineUV"), ref shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV);
            lilToggleGUI(GetLoc("sSettingTexOutlineWidth"), ref shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH);
            DrawLine();

            lilToggleGUI(GetLoc("sSettingTexFurNormal"), ref shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL);
            lilToggleGUI(GetLoc("sSettingTexFurMask"), ref shaderSetting.LIL_FEATURE_TEX_FUR_MASK);
            lilToggleGUI(GetLoc("sSettingTexFurLength"), ref shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH);
            DrawLine();

            lilToggleGUI(GetLoc("sSettingTessellation"), ref shaderSetting.LIL_FEATURE_TEX_TESSELLATION);

            GUI.enabled = true;
            if(EditorGUI.EndChangeCheck())
            {
                edSet.isShaderSettingChanged = true;
                EditorUtility.SetDirty(shaderSetting);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        static void lilToggleGUI(string label, ref bool value)
        {
            value = EditorGUILayout.ToggleLeft(label, value);
        }

        public static void ApplyShaderSetting(lilToonSetting shaderSetting)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#ifndef LIL_SETTING_INCLUDED\r\n#define LIL_SETTING_INCLUDED\r\n\r\n");
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
            }

            if(shaderSetting.LIL_FEATURE_EMISSION_1ST) sb.Append("#define LIL_FEATURE_EMISSION_1ST\r\n");
            if(shaderSetting.LIL_FEATURE_EMISSION_2ND) sb.Append("#define LIL_FEATURE_EMISSION_2ND\r\n");
            if(shaderSetting.LIL_FEATURE_EMISSION_1ST || shaderSetting.LIL_FEATURE_EMISSION_2ND)
            {
                if(shaderSetting.LIL_FEATURE_EMISSION_UV) sb.Append("#define LIL_FEATURE_EMISSION_UV\r\n");
                if(shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV) sb.Append("#define LIL_FEATURE_ANIMATE_EMISSION_UV\r\n");
                if(shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK)
                {
                    sb.Append("#define LIL_FEATURE_TEX_EMISSION_MASK\r\n");
                    if(shaderSetting.LIL_FEATURE_EMISSION_MASK_UV) sb.Append("#define LIL_FEATURE_EMISSION_MASK_UV\r\n");
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
                if(shaderSetting.LIL_FEATURE_TEX_AUDIOLINK_MASK) sb.Append("#define LIL_FEATURE_TEX_AUDIOLINK_MASK\r\n");
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
            if(shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL) sb.Append("#define LIL_FEATURE_TEX_FUR_NORMAL\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_FUR_MASK) sb.Append("#define LIL_FEATURE_TEX_FUR_MASK\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH) sb.Append("#define LIL_FEATURE_TEX_FUR_LENGTH\r\n");
            sb.Append("\r\n#endif");
            string shaderSettingString = sb.ToString();

            string shaderSettingStringBuf = "";
            string shaderSettingHLSLPath = lilToonInspector.GetShaderSettingHLSLPath();
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
                string shaderFolderPath = lilToonInspector.GetShaderFolderPath();
                RewriteReceiveShadow(shaderFolderPath + "/ltspass_opaque.shader", shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW);
                RewriteReceiveShadow(shaderFolderPath + "/ltspass_cutout.shader", shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW);
                RewriteReceiveShadow(shaderFolderPath + "/ltspass_transparent.shader", shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW);
                RewriteReceiveShadow(shaderFolderPath + "/ltspass_tess_opaque.shader", shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW);
                RewriteReceiveShadow(shaderFolderPath + "/ltspass_tess_cutout.shader", shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW);
                RewriteReceiveShadow(shaderFolderPath + "/ltspass_tess_transparent.shader", shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW);
                RewriteReceiveShadow(shaderFolderPath + "/lts_ref.shader", shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW);
                RewriteReceiveShadow(shaderFolderPath + "/lts_ref_blur.shader", shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(shaderSettingHLSLPath);
                AssetDatabase.Refresh();
            }
        }

        static void RewriteReceiveShadow(string path, bool enable)
        {
            if(String.IsNullOrEmpty(path) || !File.Exists(path)) return;
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
            }
            StreamWriter sw = new StreamWriter(path,false);
            sw.Write(s);
            sw.Close();
        }

        static void RewriteReceiveShadow(Shader shader, bool enable)
        {
            string path = AssetDatabase.GetAssetPath(shader);
            RewriteReceiveShadow(path, enable);
        }

        static void DrawPreset(Material material)
        {
            GUILayout.Label(" " + GetLoc("sPresets"), EditorStyles.boldLabel);
            if(presets == null) LoadPresets();
            ShowPresets(material);
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            if(GUILayout.Button(GetLoc("sPresetRefresh"))) LoadPresets();
            if(GUILayout.Button(GetLoc("sPresetSave"))) EditorWindow.GetWindow<lilPresetWindow>();
            GUILayout.EndHorizontal();
        }

        static bool AutoFixHelpBox(string message)
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

        //------------------------------------------------------------------------------------------------------------------------------
        // Material Setup
        public static void SetupMaterialWithRenderingMode(Material material, RenderingMode renderingMode, bool isoutl, bool islite, bool isstencil, bool istess)
        {
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
                    if(isoutl)
                    {
                        material.SetInt("_OutlineSrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_OutlineDstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
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
                    if(isoutl)
                    {
                        material.SetInt("_OutlineSrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_OutlineDstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    }
                    break;
                case RenderingMode.Transparent:
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
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    if(isoutl)
                    {
                        material.SetInt("_OutlineSrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt("_OutlineDstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    }
                    break;
                case RenderingMode.Refraction:
                    material.shader = ltsref;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    break;
                case RenderingMode.RefractionBlur:
                    material.shader = ltsrefb;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    break;
                case RenderingMode.Fur:
                    material.shader = ltsfur;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_FurSrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_FurDstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_FurZWrite", 0);
                    break;
                case RenderingMode.FurCutout:
                    material.shader = ltsfurc;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_FurSrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_FurDstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_FurZWrite", 1);
                    break;
                case RenderingMode.Gem:
                    material.shader = ltsgem;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
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
            if(renderingMode == RenderingMode.Fur || renderingMode == RenderingMode.FurCutout)
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

        static void RemoveShaderKeywords(Material material)
        {
            foreach(string keyword in material.shaderKeywords)
            {
                material.DisableKeyword(keyword);
            }
        }

        static void RemoveUnusedProperties(Material material)
        {
            Material newMaterial = new Material(material.shader);
            newMaterial.name                    = material.name;
            newMaterial.doubleSidedGI           = material.doubleSidedGI;
            newMaterial.globalIlluminationFlags = material.globalIlluminationFlags;
            newMaterial.renderQueue             = material.renderQueue;
            int propCount = ShaderUtil.GetPropertyCount(material.shader);
            for(int i = 0; i < propCount; i++)
            {
                string propName = ShaderUtil.GetPropertyName(material.shader, i);
                ShaderUtil.ShaderPropertyType propType = ShaderUtil.GetPropertyType(material.shader, i);
                if(propType == ShaderUtil.ShaderPropertyType.Color)    newMaterial.SetColor(propName,  material.GetColor(propName));
                if(propType == ShaderUtil.ShaderPropertyType.Vector)   newMaterial.SetVector(propName, material.GetVector(propName));
                if(propType == ShaderUtil.ShaderPropertyType.Float)    newMaterial.SetFloat(propName,  material.GetFloat(propName));
                if(propType == ShaderUtil.ShaderPropertyType.Range)    newMaterial.SetFloat(propName,  material.GetFloat(propName));
                if(propType == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    newMaterial.SetTexture(propName, material.GetTexture(propName));
                    newMaterial.SetTextureOffset(propName, material.GetTextureOffset(propName));
                    newMaterial.SetTextureScale(propName, material.GetTextureScale(propName));
                }
            }
            string matPath = AssetDatabase.GetAssetPath(material);
            string newMatPath = matPath + "_new";
            AssetDatabase.CreateAsset(newMaterial, newMatPath);
            FileUtil.ReplaceFile(newMatPath, matPath);
            AssetDatabase.DeleteAsset(newMatPath);
        }

        static void RemoveUnusedTexture(Material material, bool islite, bool isfur, lilToonSetting shaderSetting)
        {
            SetupMaterialFromShaderSetting(material, shaderSetting);
            RemoveUnusedProperties(material);
            if(isfur)
            {
                if(material.GetFloat("_UseShadow") == 0.0f)
                {
                    material.SetTexture("_ShadowBlurMask", null);
                    material.SetTexture("_ShadowBorderMask", null);
                    material.SetTexture("_ShadowStrengthMask", null);
                    material.SetTexture("_ShadowColorTex", null);
                    material.SetTexture("_Shadow2ndColorTex", null);
                }
            }
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
            if(!islite && !isfur)
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

        public static void RemoveUnusedTexture(Material material)
        {
            if(!material.shader.name.Contains("lilToon")) return;
            lilToonSetting shaderSetting = null;
            InitializeShaderSetting(ref shaderSetting);
            RemoveUnusedTexture(material, material.shader.name.Contains("Lite"), material.shader.name.Contains("Fur"), shaderSetting);
        }

        static bool CheckScaleOffsetChanged(Material material, string texname)
        {
            return material.GetTextureScale(texname) != defaultTextureScale || material.GetTextureOffset(texname) != defaultTextureOffset;
        }

        static void ResetScaleOffset(Material material, string texname)
        {
            material.SetTextureScale(texname, defaultTextureScale);
            material.SetTextureOffset(texname, defaultTextureOffset);
        }

        public static void SetupShaderSettingFromMaterial(Material material, ref lilToonSetting shaderSetting)
        {
            if(shaderSetting.isLocked) return;
            if(!material.shader.name.Contains("lilToon") || material.shader.name.Contains("Lite")) return;

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

            if(material.shader.name.Contains("Fur"))
            {
                if(material.HasProperty("_FurVectorTex")) shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL = shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL || material.GetTexture("_FurVectorTex") != null;
                if(material.HasProperty("_FurMask")) shaderSetting.LIL_FEATURE_TEX_FUR_MASK = shaderSetting.LIL_FEATURE_TEX_FUR_MASK || material.GetTexture("_FurMask") != null;
                if(material.HasProperty("_FurLengthMask")) shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH = shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH || material.GetTexture("_FurLengthMask") != null;
            }
            else
            {
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
                if(material.HasProperty("_EmissionMap")) shaderSetting.LIL_FEATURE_EMISSION_UV = shaderSetting.LIL_FEATURE_EMISSION_UV || CheckScaleOffsetChanged(material, "_EmissionMap");
                if(material.HasProperty("_Emission2ndMap")) shaderSetting.LIL_FEATURE_EMISSION_UV = shaderSetting.LIL_FEATURE_EMISSION_UV || CheckScaleOffsetChanged(material, "_Emission2ndMap");
                if(material.HasProperty("_EmissionMap_ScrollRotate")) shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV = shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV || material.GetVector("_EmissionMap_ScrollRotate") != defaultScrollRotate;
                if(material.HasProperty("_Emission2ndMap_ScrollRotate")) shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV = shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV || material.GetVector("_Emission2ndMap_ScrollRotate") != defaultScrollRotate;
                if(material.HasProperty("_EmissionBlendMask")) shaderSetting.LIL_FEATURE_EMISSION_MASK_UV = shaderSetting.LIL_FEATURE_EMISSION_MASK_UV || CheckScaleOffsetChanged(material, "_EmissionBlendMask");
                if(material.HasProperty("_Emission2ndBlendMask")) shaderSetting.LIL_FEATURE_EMISSION_MASK_UV = shaderSetting.LIL_FEATURE_EMISSION_MASK_UV || CheckScaleOffsetChanged(material, "_Emission2ndBlendMask");
                if(material.HasProperty("_EmissionBlendMask_ScrollRotate")) shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV = shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV || material.GetVector("_EmissionBlendMask_ScrollRotate") != defaultScrollRotate;
                if(material.HasProperty("_Emission2ndBlendMask_ScrollRotate")) shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV = shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV || material.GetVector("_Emission2ndBlendMask_ScrollRotate") != defaultScrollRotate;
                if(material.HasProperty("_EmissionUseGrad")) shaderSetting.LIL_FEATURE_EMISSION_GRADATION = shaderSetting.LIL_FEATURE_EMISSION_GRADATION || material.GetFloat("_EmissionUseGrad") != 0.0f;
                if(material.HasProperty("_UseBumpMap")) shaderSetting.LIL_FEATURE_NORMAL_1ST = shaderSetting.LIL_FEATURE_NORMAL_1ST || material.GetFloat("_UseBumpMap") != 0.0f;
                if(material.HasProperty("_UseBump2ndMap")) shaderSetting.LIL_FEATURE_NORMAL_2ND = shaderSetting.LIL_FEATURE_NORMAL_2ND || material.GetFloat("_UseBump2ndMap") != 0.0f;
                if(material.HasProperty("_UseReflection")) shaderSetting.LIL_FEATURE_REFLECTION = shaderSetting.LIL_FEATURE_REFLECTION || material.GetFloat("_UseReflection") != 0.0f;
                if(material.HasProperty("_UseMatCap")) shaderSetting.LIL_FEATURE_MATCAP = shaderSetting.LIL_FEATURE_MATCAP || material.GetFloat("_UseMatCap") != 0.0f;
                if(material.HasProperty("_UseMatCap2nd")) shaderSetting.LIL_FEATURE_MATCAP_2ND = shaderSetting.LIL_FEATURE_MATCAP_2ND || material.GetFloat("_UseMatCap2nd") != 0.0f;
                if(material.HasProperty("_UseRim")) shaderSetting.LIL_FEATURE_RIMLIGHT = shaderSetting.LIL_FEATURE_RIMLIGHT || material.GetFloat("_UseRim") != 0.0f;
                if(material.HasProperty("_RimDirStrength")) shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION = shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION || material.GetFloat("_RimDirStrength") != 0.0f;
                if(material.HasProperty("_UseGlitter")) shaderSetting.LIL_FEATURE_GLITTER = shaderSetting.LIL_FEATURE_GLITTER || material.GetFloat("_UseGlitter") != 0.0f;
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
                if(material.HasProperty("_AudioLinkMask")) shaderSetting.LIL_FEATURE_TEX_AUDIOLINK_MASK = shaderSetting.LIL_FEATURE_TEX_AUDIOLINK_MASK || material.GetTexture("_AudioLinkMask") != null;
                if(material.HasProperty("_DissolveNoiseMask")) shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE = shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE || material.GetTexture("_DissolveNoiseMask") != null;
            }

            // Outline
            if(material.shader.name.Contains("Outline"))
            {
                if(material.HasProperty("_OutlineTex_ScrollRotate")) shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV = shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV || material.GetVector("_OutlineTex_ScrollRotate") != defaultScrollRotate;
                if(material.HasProperty("_OutlineTexHSVG")) shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION = shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION || material.GetVector("_OutlineTexHSVG") != defaultHSVG;
                if(material.HasProperty("_OutlineTex")) shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR = shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR || material.GetTexture("_OutlineTex") != null;
                if(material.HasProperty("_OutlineWidthMask")) shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH = shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH || material.GetTexture("_OutlineWidthMask") != null;
            }

            // Tessellation
            shaderSetting.LIL_FEATURE_TEX_TESSELLATION = shaderSetting.LIL_FEATURE_TEX_TESSELLATION || material.shader.name.Contains("Tessellation");

            Debug.Log("lilToon auto-scan: " + material.name);
        }

        public static void SetupMaterialFromShaderSetting(Material material, lilToonSetting shaderSetting)
        {
            if(!material.shader.name.Contains("lilToon") || material.shader.name.Contains("Lite")) return;

            if(!shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV) material.SetVector("_MainTex_ScrollRotate", defaultScrollRotate);
            if(!shaderSetting.LIL_FEATURE_SHADOW) material.SetFloat("_UseShadow", 0.0f);
            if(!shaderSetting.LIL_FEATURE_SHADOW || !shaderSetting.LIL_FEATURE_RECEIVE_SHADOW) material.SetFloat("_ShadowReceive", 0.0f);
            //if(!shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER);
            if(!shaderSetting.LIL_FEATURE_DISTANCE_FADE) material.SetVector("_DistanceFade", defaultDistanceFadeParams);
            if(!shaderSetting.LIL_FEATURE_ENCRYPTION) material.SetVector("_Keys", defaultKeys);
            if(!shaderSetting.LIL_FEATURE_SHADOW || !shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR) material.SetTexture("_ShadowBlurMask", null);
            if(!shaderSetting.LIL_FEATURE_SHADOW || !shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER) material.SetTexture("_ShadowBorderMask", null);
            if(!shaderSetting.LIL_FEATURE_SHADOW || !shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH) material.SetTexture("_ShadowStrengthMask", null);
            if(!shaderSetting.LIL_FEATURE_SHADOW || !shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST) material.SetTexture("_ShadowColorTex", null);
            if(!shaderSetting.LIL_FEATURE_SHADOW || !shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND) material.SetTexture("_Shadow2ndColorTex", null);

            if(material.shader.name.Contains("Fur"))
            {
                if(!shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL) material.SetTexture("_FurVectorTex", null);
                if(!shaderSetting.LIL_FEATURE_TEX_FUR_MASK) material.SetTexture("_FurMask", null);
                if(!shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH) material.SetTexture("_FurLengthMask", null);
            }
            else
            {
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
                if(!shaderSetting.LIL_FEATURE_EMISSION_UV)
                {
                    ResetScaleOffset(material, "_EmissionMap");
                    ResetScaleOffset(material, "_Emission2ndMap");
                }
                if(!shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV)
                {
                    material.SetVector("_EmissionMap_ScrollRotate", defaultScrollRotate);
                    material.SetVector("_Emission2ndMap_ScrollRotate", defaultScrollRotate);
                }
                if(!shaderSetting.LIL_FEATURE_EMISSION_MASK_UV)
                {
                    ResetScaleOffset(material, "_EmissionBlendMask");
                    ResetScaleOffset(material, "_Emission2ndBlendMask");
                }
                if(!shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV)
                {
                    material.SetVector("_EmissionBlendMask_ScrollRotate", defaultScrollRotate);
                    material.SetVector("_Emission2ndBlendMask_ScrollRotate", defaultScrollRotate);
                }
                if(!shaderSetting.LIL_FEATURE_EMISSION_GRADATION) material.SetFloat("_EmissionUseGrad", 0.0f);
                if(!shaderSetting.LIL_FEATURE_NORMAL_1ST) material.SetFloat("_UseBumpMap", 0.0f);
                if(!shaderSetting.LIL_FEATURE_NORMAL_2ND) material.SetFloat("_UseBump2ndMap", 0.0f);
                if(!shaderSetting.LIL_FEATURE_REFLECTION) material.SetFloat("_UseReflection", 0.0f);
                if(!shaderSetting.LIL_FEATURE_MATCAP) material.SetFloat("_UseMatCap", 0.0f);
                if(!shaderSetting.LIL_FEATURE_MATCAP_2ND) material.SetFloat("_UseMatCap2nd", 0.0f);
                if(!shaderSetting.LIL_FEATURE_RIMLIGHT) material.SetFloat("_UseRim", 0.0f);
                if(!shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION) material.SetFloat("_RimDirStrength", 0.0f);
                if(!shaderSetting.LIL_FEATURE_GLITTER) material.SetFloat("_UseGlitter", 0.0f);
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
                if(!shaderSetting.LIL_FEATURE_TEX_AUDIOLINK_MASK) material.SetTexture("_AudioLinkMask", null);
                if(!shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE) material.SetTexture("_DissolveNoiseMask", null);
            }

            // Outline
            if(material.shader.name.Contains("Outline"))
            {
                if(!shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV) material.SetVector("_OutlineTex_ScrollRotate", defaultScrollRotate);
                if(!shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION) material.SetVector("_OutlineTexHSVG", defaultHSVG);
                if(!shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR) material.SetTexture("_OutlineTex", null);
                if(!shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH) material.SetTexture("_OutlineWidthMask", null);
            }

            // Tessellation
            //if(!shaderSetting.LIL_FEATURE_TEX_TESSELLATION);
        }

        public static void SetupShaderSettingFromAnimationClip(AnimationClip clip, ref lilToonSetting shaderSetting)
        {
            if(shaderSetting.isLocked) return;
            if(clip == null) return;

            EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(clip);
            foreach(EditorCurveBinding binding in bindings)
            {
                if(binding == null) continue;
                string propname = binding.propertyName;
                if(String.IsNullOrEmpty(propname) || !propname.Contains("material.")) continue;

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

                shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL = shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL || propname.Contains("_FurVectorTex");
                shaderSetting.LIL_FEATURE_TEX_FUR_MASK = shaderSetting.LIL_FEATURE_TEX_FUR_MASK || propname.Contains("_FurMask");
                shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH = shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH || propname.Contains("_FurLengthMask");

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
                shaderSetting.LIL_FEATURE_EMISSION_UV = shaderSetting.LIL_FEATURE_EMISSION_UV || propname.Contains("_EmissionMap_ST") || propname.Contains("_Emission2ndMap_ST");
                shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV = shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV || propname.Contains("_EmissionMap_ScrollRotate") || propname.Contains("_Emission2ndMap_ScrollRotate");
                shaderSetting.LIL_FEATURE_EMISSION_MASK_UV = shaderSetting.LIL_FEATURE_EMISSION_MASK_UV || propname.Contains("_EmissionBlendMask_ST") || propname.Contains("_Emission2ndBlendMask_ST");
                shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV = shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV || propname.Contains("_EmissionBlendMask_ScrollRotate") || propname.Contains("_Emission2ndBlendMask_ScrollRotate");
                shaderSetting.LIL_FEATURE_EMISSION_GRADATION = shaderSetting.LIL_FEATURE_EMISSION_GRADATION || propname.Contains("_EmissionUseGrad");
                shaderSetting.LIL_FEATURE_NORMAL_1ST = shaderSetting.LIL_FEATURE_NORMAL_1ST || propname.Contains("_UseBumpMap");
                shaderSetting.LIL_FEATURE_NORMAL_2ND = shaderSetting.LIL_FEATURE_NORMAL_2ND || propname.Contains("_UseBump2ndMap");
                shaderSetting.LIL_FEATURE_REFLECTION = shaderSetting.LIL_FEATURE_REFLECTION || propname.Contains("_UseReflection");
                shaderSetting.LIL_FEATURE_MATCAP = shaderSetting.LIL_FEATURE_MATCAP || propname.Contains("_UseMatCap");
                shaderSetting.LIL_FEATURE_MATCAP_2ND = shaderSetting.LIL_FEATURE_MATCAP_2ND || propname.Contains("_UseMatCap2nd");
                shaderSetting.LIL_FEATURE_RIMLIGHT = shaderSetting.LIL_FEATURE_RIMLIGHT || propname.Contains("_UseRim");
                shaderSetting.LIL_FEATURE_GLITTER = shaderSetting.LIL_FEATURE_GLITTER || propname.Contains("_UseGlitter");
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
                shaderSetting.LIL_FEATURE_TEX_AUDIOLINK_MASK = shaderSetting.LIL_FEATURE_TEX_AUDIOLINK_MASK || propname.Contains("_AudioLinkMask");
                shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE = shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE || propname.Contains("_DissolveNoiseMask");

                shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV = shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV || propname.Contains("_OutlineTex_ScrollRotate");
                shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION = shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION || propname.Contains("_OutlineTexHSVG");
                shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR = shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR || propname.Contains("_OutlineTex");
                shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH = shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH || propname.Contains("_OutlineWidthMask");
            }

            Debug.Log("lilToon auto-scan: " + clip.name);
        }

        public static bool CheckMainTextureName(string name)
        {
            foreach(string word in mainTexCheckWords)
            {
                if(name.Contains(word)) return false;
            }
            return true;
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Presets
        static void LoadPresets()
        {
            string[] presetGuid = AssetDatabase.FindAssets("t:lilToonPreset", new[] {GetPresetsFolderPath()});
            Array.Resize(ref presets, presetGuid.Length);
            for(int i=0; i<presetGuid.Length; i++)
            {
                presets[i] = (lilToonPreset)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(presetGuid[i]), typeof(lilToonPreset));
            }
        }

        static void ShowPresets(Material material)
        {
            GUIStyle PresetButton = new GUIStyle(GUI.skin.button);
            PresetButton.alignment = TextAnchor.MiddleLeft;
            #if UNITY_2019_1_OR_NEWER
                PresetButton.margin.left = 24;
            #else
                PresetButton.margin.left = 20;
            #endif
            string[] sCategorys = { GetLoc("sPresetCategorySkin"),
                                    GetLoc("sPresetCategoryHair"),
                                    GetLoc("sPresetCategoryCloth"),
                                    GetLoc("sPresetCategoryNature"),
                                    GetLoc("sPresetCategoryInorganic"),
                                    GetLoc("sPresetCategoryEffect"),
                                    GetLoc("sPresetCategoryOther") };
            for(int i=0; i<(int)lilPresetCategory.Other+1; i++)
            {
                edSet.isShowCategorys[i] = Foldout(sCategorys[i], "", edSet.isShowCategorys[i]);
                if(edSet.isShowCategorys[i])
                {
                    for(int j=0; j<presets.Length; j++)
                    {
                        if(i == (int)presets[j].category)
                        {
                            string showName = "";
                            for(int k=0; k<presets[j].bases.Length; k++)
                            {
                                if(String.IsNullOrEmpty(showName))
                                {
                                    showName = presets[j].bases[k].name;
                                }
                                if(presets[j].bases[k].language == edSet.languageName)
                                {
                                    showName = presets[j].bases[k].name;
                                    k = 256;
                                }
                            }
                            if(GUILayout.Button(showName,PresetButton)) ApplyPreset(material, presets[j]);
                        }
                    }
                }
            }
        }

        public static void ApplyPreset(Material material, lilToonPreset preset)
        {
            if(material == null || preset == null) return;
            for(int i = 0; i < preset.floats.Length; i++)
            {
                if(preset.floats[i].name == "_StencilPass") material.SetFloat(preset.floats[i].name, preset.floats[i].value);
            }
            if(preset.shader != null) material.shader = preset.shader;
            if(preset.renderQueue != -2) material.renderQueue = preset.renderQueue;
            bool isoutl         = preset.outline;
            bool istess         = preset.tessellation;
            bool isstencil      = (material.GetFloat("_StencilPass") == (float)UnityEngine.Rendering.StencilOp.Replace);

            bool islite         = material.shader.name.Contains("Lite");
            bool iscutout       = material.shader.name.Contains("Cutout");
            bool istransparent  = material.shader.name.Contains("Transparent");
            bool isrefr         = preset.shader != null && preset.shader.name.Contains("Refraction");
            bool isblur         = preset.shader != null && preset.shader.name.Contains("Blur");
            bool isfur          = preset.shader != null && preset.shader.name.Contains("Fur");

            RenderingMode       renderingMode = RenderingMode.Opaque;
            if(iscutout)        renderingMode = RenderingMode.Cutout;
            if(istransparent)   renderingMode = RenderingMode.Transparent;
            if(isrefr)          renderingMode = RenderingMode.Refraction;
            if(isrefr&isblur)   renderingMode = RenderingMode.RefractionBlur;
            if(isfur)           renderingMode = RenderingMode.Fur;
            if(isfur&iscutout)  renderingMode = RenderingMode.FurCutout;

            SetupMaterialWithRenderingMode(material, renderingMode, isoutl, islite, isstencil, istess);

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

        //------------------------------------------------------------------------------------------------------------------------------
        // Material Converter
        void CreateMToonMaterial(Material material)
        {
            Material mtoonMaterial = new Material(mtoon);

            string matPath = AssetDatabase.GetAssetPath(material);
            if(!String.IsNullOrEmpty(matPath))  matPath = EditorUtility.SaveFilePanel("Save Material", Path.GetDirectoryName(matPath), Path.GetFileNameWithoutExtension(matPath)+"_mtoon", "mat");
            else                    matPath = EditorUtility.SaveFilePanel("Save Material", "Assets", material.name + ".mat", "mat");
            if(!String.IsNullOrEmpty(matPath))  AssetDatabase.CreateAsset(mtoonMaterial, FileUtil.GetProjectRelativePath(matPath));

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
                float shadeShift = Mathf.Clamp01(shadowBorder.floatValue - shadowBlur.floatValue * 0.5f) * 2.0f - 1.0f;
                float shadeToony = (2.0f - Mathf.Clamp01(shadowBorder.floatValue + shadowBlur.floatValue * 0.5f) * 2.0f) / (1.0f - shadeShift);
                if(shadowStrengthMask.textureValue != null || shadowMainStrength.floatValue != 0.0f)
                {
                    Texture2D bakedShadowTex = AutoBakeShadowTexture(material, bakedMainTex);
                    mtoonMaterial.SetColor("_ShadeColor",               Color.white);
                    mtoonMaterial.SetTexture("_ShadeTexture",           bakedShadowTex);
                }
                else
                {
                    Color shadeColorStrength = new Color(1.0f,1.0f,1.0f,1.0f);
                    shadeColorStrength.r = 1.0f - (1.0f - shadowColor.colorValue.r) * shadowStrength.floatValue;
                    shadeColorStrength.g = 1.0f - (1.0f - shadowColor.colorValue.g) * shadowStrength.floatValue;
                    shadeColorStrength.b = 1.0f - (1.0f - shadowColor.colorValue.b) * shadowStrength.floatValue;
                    shadeColorStrength.a = 1.0f;
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
                float rimLift = Mathf.Pow((1.0f - rimBorder.floatValue), rimFresnelPower.floatValue) * (1.0f - rimBlur.floatValue);
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
                mtoonMaterial.SetOverrideTag("RenderType", "Transparent");
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
                mtoonMaterial.SetOverrideTag("RenderType", "Transparent");
                mtoonMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mtoonMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mtoonMaterial.SetFloat("_ZWrite", 1.0f);
                mtoonMaterial.SetFloat("_AlphaToMask", 0.0f);
                mtoonMaterial.EnableKeyword("_ALPHABLEND_ON");
                mtoonMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            else
            {
                mtoonMaterial.SetFloat("_BlendMode",                0.0f);
                mtoonMaterial.SetOverrideTag("RenderType", "Opaque");
                mtoonMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                mtoonMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                mtoonMaterial.SetFloat("_ZWrite", 1.0f);
                mtoonMaterial.SetFloat("_AlphaToMask", 0.0f);
                mtoonMaterial.renderQueue = -1;
            }
        }

        void CreateLiteMaterial(Material material)
        {
            Material liteMaterial = new Material(ltsl);
            RenderingMode renderingMode = renderingModeBuf;
            if(renderingMode == RenderingMode.Refraction)       renderingMode = RenderingMode.Transparent;
            if(renderingMode == RenderingMode.RefractionBlur)   renderingMode = RenderingMode.Transparent;
            if(renderingMode == RenderingMode.Fur)              renderingMode = RenderingMode.Transparent;
            if(renderingMode == RenderingMode.FurCutout)        renderingMode = RenderingMode.Cutout;

            SetupMaterialWithRenderingMode(liteMaterial, renderingMode, isOutl, true, isStWr, false);

            string matPath = AssetDatabase.GetAssetPath(material);
            if(!String.IsNullOrEmpty(matPath))  matPath = EditorUtility.SaveFilePanel("Save Material", Path.GetDirectoryName(matPath), Path.GetFileNameWithoutExtension(matPath)+"_lite", "mat");
            else                    matPath = EditorUtility.SaveFilePanel("Save Material", "Assets", material.name + ".mat", "mat");
            if(String.IsNullOrEmpty(matPath))   return;
            else                                AssetDatabase.CreateAsset(liteMaterial, FileUtil.GetProjectRelativePath(matPath));

            liteMaterial.SetFloat("_Invisible",                 invisible.floatValue);
            liteMaterial.SetFloat("_Cutoff",                    cutoff.floatValue);
            liteMaterial.SetFloat("_Cull",                  cull.floatValue);
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
            liteMaterial.SetFloat("_ZWrite",                    zwrite.floatValue);
            liteMaterial.SetFloat("_ZTest",                     ztest.floatValue);
            liteMaterial.SetFloat("_StencilRef",                stencilRef.floatValue);
            liteMaterial.SetFloat("_StencilReadMask",           stencilReadMask.floatValue);
            liteMaterial.SetFloat("_StencilWriteMask",          stencilWriteMask.floatValue);
            liteMaterial.SetFloat("_StencilComp",               stencilComp.floatValue);
            liteMaterial.SetFloat("_StencilPass",               stencilPass.floatValue);
            liteMaterial.SetFloat("_StencilFail",               stencilFail.floatValue);
            liteMaterial.SetFloat("_StencilZFail",              stencilZFail.floatValue);
            liteMaterial.renderQueue = material.renderQueue;
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Property drawer
        void UV4Decal(MaterialEditor materialEditor, MaterialProperty isDecal, MaterialProperty isLeftOnly, MaterialProperty isRightOnly, MaterialProperty shouldCopy, MaterialProperty shouldFlipMirror, MaterialProperty shouldFlipCopy, MaterialProperty tex, MaterialProperty angle, MaterialProperty decalAnimation, MaterialProperty decalSubParam)
        {
            if(shaderSetting.LIL_FEATURE_DECAL)
            {
                #if SYSTEM_DRAWING
                ConvertGifToAtlas(tex, decalAnimation, decalSubParam, isDecal);
                #endif
                // Toggle decal
                EditorGUI.BeginChangeCheck();
                materialEditor.ShaderProperty(isDecal, GetLoc("sAsDecal"));
                if(EditorGUI.EndChangeCheck() & isDecal.floatValue == 0.0f)
                {
                    isLeftOnly.floatValue = 0.0f;
                    isRightOnly.floatValue = 0.0f;
                    shouldFlipMirror.floatValue = 0.0f;
                    shouldCopy.floatValue = 0.0f;
                    shouldFlipCopy.floatValue = 0.0f;
                }
            }

            if(shaderSetting.LIL_FEATURE_DECAL && isDecal.floatValue == 1.0f)
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
                mirrorMode = EditorGUILayout.Popup(GetLoc("sMirrorMode"),mirrorMode,new String[]{GetLoc("sMirrorModeNormal"),GetLoc("sMirrorModeFlip"),GetLoc("sMirrorModeLeft"),GetLoc("sMirrorModeRight"),GetLoc("sMirrorModeRightFlip")});
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
                copyMode = EditorGUILayout.Popup(GetLoc("sCopyMode"),copyMode,new String[]{GetLoc("sCopyModeNormal"),GetLoc("sCopyModeSymmetry"),GetLoc("sCopyModeFlip")});
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
                    posX = -posX * scaleX + 0.5f;
                    posY = -posY * scaleY + 0.5f;

                    tex.textureScaleAndOffset = new Vector4(scaleX, scaleY, posX, posY);
                }

                materialEditor.ShaderProperty(angle, GetLoc("sAngle"));

                if(shaderSetting.LIL_FEATURE_ANIMATE_DECAL)
                {
                    materialEditor.ShaderProperty(decalAnimation, GetLoc("sAnimation")+"|"+GetLoc("sXFrames")+"|"+GetLoc("sYFrames")+"|"+GetLoc("sFrames")+"|"+GetLoc("sFPS"));
                    materialEditor.ShaderProperty(decalSubParam, GetLoc("sXRatio")+"|"+GetLoc("sYRatio")+"|"+GetLoc("sFixBorder"));
                }
            }
            else
            {
                materialEditor.TextureScaleOffsetProperty(tex);
                materialEditor.ShaderProperty(angle, GetLoc("sAngle"));
            }

            if(GUILayout.Button(GetLoc("sReset")) && EditorUtility.DisplayDialog(GetLoc("sDialogResetUV"),GetLoc("sDialogResetUVMes"),GetLoc("sYes"),GetLoc("sNo")))
            {
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

        void ToneCorrectionGUI(MaterialEditor materialEditor, Material material, MaterialProperty tex, MaterialProperty col, MaterialProperty hsvg, bool isOutline = false)
        {
            materialEditor.ShaderProperty(hsvg, GetLoc("sHue") + "|" + GetLoc("sSaturation") + "|" + GetLoc("sValue") + "|" + GetLoc("sGamma"));
            // Reset
            if(GUILayout.Button(GetLoc("sReset")))
            {
                hsvg.vectorValue = defaultHSVG;
            }
        }

        void TextureBakeGUI(Material material, int bakeType)
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

        void TextureBakeGUI(Material material, int bakeType, GUIStyle guistyle)
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
            if(GUILayout.Button(sBake[bakeType], guistyle))
            {
                TextureBake(material, bakeType);
            }
        }

        void AlphamaskToTextureGUI(Material material)
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

        void SetAlphaIsTransparencyGUI(MaterialProperty tex)
        {
            if(tex.textureValue != null && !((Texture2D)tex.textureValue).alphaIsTransparency)
            {
                if(AutoFixHelpBox(GetLoc("sNotAlphaIsTransparency")))
                {
                    string path = AssetDatabase.GetAssetPath(tex.textureValue);
                    TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(path);
                    textureImporter.alphaIsTransparency = true;
                    AssetDatabase.ImportAsset(path);
                }
            }
        }

        void UVSettingGUI(MaterialEditor materialEditor, MaterialProperty uvst, MaterialProperty uvsr)
        {
            EditorGUILayout.LabelField(GetLoc("sUVSetting"), EditorStyles.boldLabel);
            materialEditor.TextureScaleOffsetProperty(uvst);
            materialEditor.ShaderProperty(uvsr, GetLoc("sAngle") + "|" + GetLoc("sUVAnimation") + "|" + GetLoc("sScroll") + "|" + GetLoc("sRotate"));
        }

        void BlendSettingGUI(MaterialEditor materialEditor, ref bool isShow, string labelName, MaterialProperty srcRGB, MaterialProperty dstRGB, MaterialProperty srcA, MaterialProperty dstA, MaterialProperty opRGB, MaterialProperty opA)
        {
            // Make space for foldout
            EditorGUI.indentLevel++;
            Rect rect = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(rect, labelName);
            EditorGUI.indentLevel--;

            rect.x += 10;
            isShow = EditorGUI.Foldout(rect, isShow, "");
            if(isShow)
            {
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(srcRGB, GetLoc("sSrcBlendRGB"));
                materialEditor.ShaderProperty(dstRGB, GetLoc("sDstBlendRGB"));
                materialEditor.ShaderProperty(srcA, GetLoc("sSrcBlendAlpha"));
                materialEditor.ShaderProperty(dstA, GetLoc("sDstBlendAlpha"));
                materialEditor.ShaderProperty(opRGB, GetLoc("sBlendOpRGB"));
                materialEditor.ShaderProperty(opA, GetLoc("sBlendOpAlpha"));
                EditorGUI.indentLevel--;
            }
        }

        void TextureGUI(MaterialEditor materialEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName)
        {
            // Make space for foldout
            EditorGUI.indentLevel++;
            Rect rect = materialEditor.TexturePropertySingleLine(guiContent, textureName);
            EditorGUI.indentLevel--;
            rect.x += 10;
            isShow = EditorGUI.Foldout(rect, isShow, "");
            if(isShow)
            {
                EditorGUI.indentLevel++;
                materialEditor.TextureScaleOffsetProperty(textureName);
                EditorGUI.indentLevel--;
            }
        }

        void TextureGUI(MaterialEditor materialEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba)
        {
            // Make space for foldout
            EditorGUI.indentLevel++;
            Rect rect = materialEditor.TexturePropertySingleLine(guiContent, textureName, rgba);
            EditorGUI.indentLevel--;
            rect.x += 10;
            isShow = EditorGUI.Foldout(rect, isShow, "");
            if(isShow)
            {
                EditorGUI.indentLevel++;
                materialEditor.TextureScaleOffsetProperty(textureName);
                EditorGUI.indentLevel--;
            }
        }

        void TextureGUI(MaterialEditor materialEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty scrollRotate)
        {
            // Make space for foldout
            EditorGUI.indentLevel++;
            Rect rect = materialEditor.TexturePropertySingleLine(guiContent, textureName, rgba);
            EditorGUI.indentLevel--;
            rect.x += 10;
            isShow = EditorGUI.Foldout(rect, isShow, "");
            if(isShow)
            {
                EditorGUI.indentLevel++;
                materialEditor.TextureScaleOffsetProperty(textureName);
                materialEditor.ShaderProperty(scrollRotate, GetLoc("sScroll"));
                EditorGUI.indentLevel--;
            }
        }

        void TextureGUI(MaterialEditor materialEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty scrollRotate, bool useCustomUV, bool useUVAnimation)
        {
            if(useCustomUV)
            {
                // Make space for foldout
                EditorGUI.indentLevel++;
                Rect rect = materialEditor.TexturePropertySingleLine(guiContent, textureName, rgba);
                EditorGUI.indentLevel--;
                rect.x += 10;
                isShow = EditorGUI.Foldout(rect, isShow, "");
                if(isShow)
                {
                    EditorGUI.indentLevel++;
                    if(useUVAnimation)  UVSettingGUI(materialEditor, textureName, scrollRotate);
                    else                materialEditor.TextureScaleOffsetProperty(textureName);
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                materialEditor.TexturePropertySingleLine(guiContent, textureName, rgba);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Bake
        void TextureBake(Material material, int bakeType)
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

                if(shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP)
                {
                    hsvgMaterial.SetFloat(mainGradationStrength.name, mainGradationStrength.floatValue);
                    hsvgMaterial.SetTexture(mainGradationTex.name, mainGradationTex.textureValue);
                }

                path = AssetDatabase.GetAssetPath(material.GetTexture(mainTex.name));
                if(!String.IsNullOrEmpty(path))
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
                    if(!String.IsNullOrEmpty(path))
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
                    if(!String.IsNullOrEmpty(path))
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
                    if(!String.IsNullOrEmpty(path))
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
                    if(!String.IsNullOrEmpty(path))
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
                    if(shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP)
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

        Texture2D AutoBakeMainTexture(Material material)
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

                if(shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP)
                {
                    hsvgMaterial.SetFloat(mainGradationStrength.name, mainGradationStrength.floatValue);
                    hsvgMaterial.SetTexture(mainGradationTex.name, mainGradationTex.textureValue);
                }

                path = AssetDatabase.GetAssetPath(material.GetTexture(mainTex.name));
                if(!String.IsNullOrEmpty(path))
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
                    if(!String.IsNullOrEmpty(path))
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
                    if(!String.IsNullOrEmpty(path))
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
                    if(!String.IsNullOrEmpty(path))
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
                    if(!String.IsNullOrEmpty(path))
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

        Texture2D AutoBakeShadowTexture(Material material, Texture2D bakedMainTex, int shadowType = 0, bool shouldShowDialog = true)
        {
            bool shouldNotBakeAll = useShadow.floatValue == 0.0 && shadowColor.colorValue == Color.white && shadowColorTex.textureValue == null && shadowStrengthMask.textureValue == null;
            bool shouldBake = true;
            if(shouldShowDialog) shouldBake = EditorUtility.DisplayDialog(GetLoc("sDialogRunBake"), GetLoc("sDialogBakeShadow"), GetLoc("sYes"), GetLoc("sNo"));
            if(!shouldNotBakeAll && shouldBake)
            {
                // run bake
                Texture2D bufMainTexture = bakedMainTex;
                Material hsvgMaterial = new Material(ltsbaker);

                string path = "";
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
                else
                {
                    hsvgMaterial.SetColor(mainColor2nd.name,                new Color(shadowColor.colorValue.r, shadowColor.colorValue.g, shadowColor.colorValue.b, shadowStrength.floatValue));
                    hsvgMaterial.SetFloat(main2ndTexBlendMode.name,         0.0f);
                    path = AssetDatabase.GetAssetPath(material.GetTexture(shadowColorTex.name));
                }

                bool existsShadowTex = !String.IsNullOrEmpty(path);
                if(existsShadowTex)
                {
                    bytes = File.ReadAllBytes(Path.GetFullPath(path));
                    srcMain2.LoadImage(bytes);
                    srcMain2.filterMode = FilterMode.Bilinear;
                    hsvgMaterial.SetTexture(main2ndTex.name, srcMain2);
                }

                path = AssetDatabase.GetAssetPath(bakedMainTex);
                if(!String.IsNullOrEmpty(path))
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
                if(!String.IsNullOrEmpty(path))
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

        Texture2D AutoBakeMatCap(Material material)
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
                if(!String.IsNullOrEmpty(path))
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

        Texture2D AutoBakeTriMask(Material material)
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
                if(!String.IsNullOrEmpty(path))
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
                if(!String.IsNullOrEmpty(path))
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
                if(!String.IsNullOrEmpty(path))
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

        Texture2D AutoBakeAlphaMask(Material material)
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
            hsvgMaterial.SetFloat(alphaMaskValue.name,      alphaMaskValue.floatValue);

            path = AssetDatabase.GetAssetPath(bufMainTexture);
            if(!String.IsNullOrEmpty(path))
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
            if(!String.IsNullOrEmpty(path))
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

            outTexture = SaveTextureToPng(material, outTexture, bufMainTexture);
            if(outTexture != bufMainTexture)
            {
                CopyTextureSetting(bufMainTexture, outTexture);
                string savePath = AssetDatabase.GetAssetPath(outTexture);
                TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(savePath);
                textureImporter.alphaIsTransparency = true;
                AssetDatabase.ImportAsset(savePath);
            }

            UnityEngine.Object.DestroyImmediate(hsvgMaterial);
            UnityEngine.Object.DestroyImmediate(srcTexture);
            UnityEngine.Object.DestroyImmediate(dstTexture);

            return outTexture;
        }

        Texture2D AutoBakeOutlineTexture(Material material)
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
                if(!String.IsNullOrEmpty(path))
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

        void CopyTextureSetting(Texture2D fromTexture, Texture2D toTexture)
        {
            string fromPath = AssetDatabase.GetAssetPath(fromTexture);
            string toPath = AssetDatabase.GetAssetPath(toTexture);
            TextureImporter fromTextureImporter = (TextureImporter)TextureImporter.GetAtPath(fromPath);
            TextureImporter toTextureImporter = (TextureImporter)TextureImporter.GetAtPath(toPath);

            TextureImporterSettings fromTextureImporterSettings = new TextureImporterSettings();
            fromTextureImporter.ReadTextureSettings(fromTextureImporterSettings);
            toTextureImporter.SetTextureSettings(fromTextureImporterSettings);
            toTextureImporter.SetPlatformTextureSettings(fromTextureImporter.GetDefaultPlatformTextureSettings());
            AssetDatabase.ImportAsset(toPath);
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Gradient
        void SetLinear(Texture2D tex)
        {
            if(tex != null)
            {
                string path = AssetDatabase.GetAssetPath(tex);
                TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(path);
                textureImporter.sRGBTexture = false;
                AssetDatabase.ImportAsset(path);
            }
        }

        void GradientEditor(Material material, Gradient ingrad, MaterialProperty texprop, bool setLinear = false)
        {
            #if UNITY_2018_1_OR_NEWER
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

        void GradientEditor(Material material, string emissionName, Gradient ingrad, MaterialProperty texprop, bool setLinear = false)
        {
            ingrad = MaterialToGradient(material, emissionName);
            #if UNITY_2018_1_OR_NEWER
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

        Gradient MaterialToGradient(Material material, string emissionName)
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

        void GradientToMaterial(Material material, string emissionName, Gradient ingrad)
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

        Texture2D GradientToTexture(Gradient grad, bool setLinear = false)
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

        //------------------------------------------------------------------------------------------------------------------------------
        // Save Texture
        Texture2D SaveTextureToPng(Material material, Texture2D tex, string texname, string customName = "")
        {
            string path = AssetDatabase.GetAssetPath(material.GetTexture(texname));
            if(String.IsNullOrEmpty(path)) path = AssetDatabase.GetAssetPath(material);

            string filename = Path.GetFileNameWithoutExtension(path);
            if(!String.IsNullOrEmpty(customName)) filename += customName;
            else                                  filename += "_2";
            if(!String.IsNullOrEmpty(path))  path = EditorUtility.SaveFilePanel("Save Texture", Path.GetDirectoryName(path), filename, "png");
            else                 path = EditorUtility.SaveFilePanel("Save Texture", "Assets", tex.name + ".png", "png");
            if(!String.IsNullOrEmpty(path)) {
                File.WriteAllBytes(path, tex.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(tex);
                AssetDatabase.Refresh();
                return (Texture2D)AssetDatabase.LoadAssetAtPath(path.Substring(path.IndexOf("Assets")), typeof(Texture2D));
            }
            else
            {
                return (Texture2D)material.GetTexture(texname);
            }
        }

        Texture2D SaveTextureToPng(Material material, Texture2D tex, Texture2D origTex)
        {
            string path = AssetDatabase.GetAssetPath(origTex);
            if(!String.IsNullOrEmpty(path))  path = EditorUtility.SaveFilePanel("Save Texture", Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)+"_alpha", "png");
            else                 path = EditorUtility.SaveFilePanel("Save Texture", "Assets", tex.name + "_alpha" + ".png", "png");
            if(!String.IsNullOrEmpty(path)) {
                File.WriteAllBytes(path, tex.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(tex);
                AssetDatabase.Refresh();
                return (Texture2D)AssetDatabase.LoadAssetAtPath(path.Substring(path.IndexOf("Assets")), typeof(Texture2D));
            }
            else
            {
                return origTex;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Save Preset Window
        public class lilPresetWindow : EditorWindow
        {
            private Vector2 scrollPosition = Vector2.zero;

            bool shouldSaveShader = false;
            bool shouldSaveQueue = false;
            bool shouldSaveStencil = false;
            bool shouldSaveMainTex2Outline = false;
            bool shouldSaveMain = false;
            bool shouldSaveMain2 = false;
            bool shouldSaveMain2Mask = false;
            bool shouldSaveMain3 = false;
            bool shouldSaveMain3Mask = false;
            bool shouldSaveShadowBorder = false;
            bool shouldSaveShadowBlur = false;
            bool shouldSaveShadowStrength = false;
            bool shouldSaveShadowColor = false;
            bool shouldSaveShadowColor2 = false;
            bool shouldSaveOutlineTex = false;
            bool shouldSaveOutlineWidth = false;
            bool shouldSaveEmissionColor = false;
            bool shouldSaveEmissionMask = false;
            bool shouldSaveEmissionGrad = false;
            bool shouldSaveEmission2Color = false;
            bool shouldSaveEmission2Mask = false;
            bool shouldSaveEmission2Grad = false;
            bool shouldSaveNormal = false;
            bool shouldSaveNormal2 = false;
            bool shouldSaveNormal2Mask = false;
            bool shouldSaveReflectionSmoothness = false;
            bool shouldSaveReflectionMetallic = false;
            bool shouldSaveReflectionColor = false;
            bool shouldSaveMatcap = false;
            bool shouldSaveMatcapMask = false;
            bool shouldSaveRim = false;
            bool shouldSaveParallax = false;
            bool shouldSaveFurNormal = false;
            bool shouldSaveFurNoise = false;
            bool shouldSaveFurMask = false;
            lilToonPreset preset;
            string[] presetName;
            string filename = "";

            void OnGUI()
            {
                string[] sCategorys = { GetLoc("sPresetCategorySkin"),
                                        GetLoc("sPresetCategoryHair"),
                                        GetLoc("sPresetCategoryCloth"),
                                        GetLoc("sPresetCategoryNature"),
                                        GetLoc("sPresetCategoryInorganic"),
                                        GetLoc("sPresetCategoryEffect"),
                                        GetLoc("sPresetCategoryOther") };
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                Material material = (Material)Selection.activeObject;
                if(preset == null) preset = ScriptableObject.CreateInstance<lilToonPreset>();

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
                        if(!String.IsNullOrEmpty(presetName[i]))
                        {
                            Array.Resize(ref preset.bases, preset.bases.Length+1);
                            preset.bases[preset.bases.Length-1].language = langName[i];
                            preset.bases[preset.bases.Length-1].name = presetName[i];
                            if(String.IsNullOrEmpty(filename) || langName[i] == "English") filename = preset.category.ToString() + "-" + presetName[i];
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
                    if(!String.IsNullOrEmpty(savePath)) 
                    {
                        AssetDatabase.CreateAsset(preset, FileUtil.GetProjectRelativePath(savePath));
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        AssetDatabase.ImportAsset(FileUtil.GetProjectRelativePath(savePath), ImportAssetOptions.ForceUpdate);
                        LoadPresets();
                        this.Close();
                    }
                }

                EditorGUILayout.EndScrollView();
            }

            void lilPresetCopyTexture(lilToonPreset preset, Material material, string propName)
            {
                Array.Resize(ref preset.textures, preset.textures.Length + 1);
                preset.textures[preset.textures.Length-1].name = propName;
                preset.textures[preset.textures.Length-1].value = material.GetTexture(propName);
                preset.textures[preset.textures.Length-1].offset = material.GetTextureOffset(propName);
                preset.textures[preset.textures.Length-1].scale = material.GetTextureScale(propName);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Gif to Atlas
        #if SYSTEM_DRAWING
            void ConvertGifToAtlas(MaterialProperty tex, MaterialProperty decalAnimation, MaterialProperty decalSubParam, MaterialProperty isDecal)
            {
                if(tex.textureValue != null && AssetDatabase.GetAssetPath(tex.textureValue).ToLower().Contains(".gif") && GUILayout.Button(GetLoc("sConvertGif")))
                {
                    string path = AssetDatabase.GetAssetPath(tex.textureValue);
                    System.Drawing.Image origGif = System.Drawing.Image.FromFile(path);
                    System.Drawing.Imaging.FrameDimension dimension = new System.Drawing.Imaging.FrameDimension(origGif.FrameDimensionsList[0]);
                    int frameCount = origGif.GetFrameCount(dimension);
                    int loopXY = Mathf.CeilToInt(Mathf.Sqrt(frameCount));
                    int duration = BitConverter.ToInt32(origGif.GetPropertyItem(20736).Value, 0);
                    int finalWidth = 1;
                    int finalHeight = 1;
                    if(EditorUtility.DisplayDialog(GetLoc("sDialogGifToAtlas"),GetLoc("sDialogTexPow2"),GetLoc("sYes"),GetLoc("sNo")))
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
                    float xScale = (float)(origGif.Width * loopXY) / (float)finalWidth;
                    float yScale = (float)(origGif.Height * loopXY) / (float)finalHeight;
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
                                atlasTexture.SetPixel(x + frame.Width*offsetX, finalHeight - frame.Height * offsetY - 1 - y, new Color32(sourceColor.R, sourceColor.G, sourceColor.B, sourceColor.A));
                            }
                        }
                    }
                    atlasTexture.Apply();
                    
                    // Save
                    string savePath = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + "_gif2png_" + loopXY + "_" + frameCount + "_" + duration + ".png";
                    File.WriteAllBytes(savePath, atlasTexture.EncodeToPNG());
                    AssetDatabase.Refresh();
                    tex.textureValue = (Texture2D)AssetDatabase.LoadAssetAtPath(savePath, typeof(Texture2D));
                    decalAnimation.vectorValue = new Vector4((float)loopXY,(float)loopXY,(float)frameCount,100.0f/(float)duration);
                    decalSubParam.vectorValue = new Vector4(xScale,yScale,decalSubParam.vectorValue.z,decalSubParam.vectorValue.w);
                    isDecal.floatValue = 1.0f;
                }
            }
        #endif
    }
}
#endif