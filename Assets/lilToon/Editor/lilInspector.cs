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
            Stable
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Constant
        private const string currentVersionName = "1.2.7";
        private const int currentVersionValue = 20;

        private const string boothURL = "https://lilxyzw.booth.pm/";
        private const string githubURL = "https://github.com/lilxyzw/lilToon";
        public const string versionInfoURL = "https://raw.githubusercontent.com/lilxyzw/lilToon/master/version.json";
        private const string mainFolderGUID         = "05d1d116436047941ad97d1b9064ee05"; // "Assets/lilToon"
        private const string editorFolderGUID       = "3e73d675b9c1adc4f8b6b8ef01bce51c"; // "Assets/lilToon/Editor"
        private const string presetsFolderGUID      = "35817d21af2f3134182c4a7e4c07786b"; // "Assets/lilToon/Presets"
        private const string editorGUID             = "aefa51cbc37d602418a38a02c3b9afb9"; // "Assets/lilToon/Editor/lilInspector.cs"
        private const string shaderFolderGUID       = "ac0a8f602b5e72f458f4914bf08f0269"; // "Assets/lilToon/Shader"
        private const string shaderPipelineGUID     = "32299664512e2e042bbc351c1d46d383"; // "Assets/lilToon/Shader/Includes/lil_pipeline.hlsl";
        private const string shaderCommonGUID       = "5520e766422958546bbe885a95d5a67e"; // "Assets/lilToon/Shader/Includes/lil_common.hlsl";
        private const string avatarEncryptionGUID   = "f9787bf8ed5154f4b931278945ac8ca1"; // "Assets/AvaterEncryption";
        private const string editorSettingTempPath  = "Temp/lilToonEditorSetting";
        public const string versionInfoTempPath   = "Temp/lilToonVersion";
        public const string packageListTempPath   = "Temp/lilToonPackageList";
        private static readonly string[] mainTexCheckWords = new[] {"mask", "shadow", "shade", "outline", "normal", "bumpmap", "matcap", "rimlight", "emittion", "reflection", "specular", "roughness", "smoothness", "metallic", "metalness", "opacity", "parallax", "displacement", "height", "ambient", "occlusion"};

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

        protected static Shader ltsgem      = Shader.Find("Hidden/lilToonGem");

        protected static Shader ltsfs       = Shader.Find("_lil/lilToonFakeShadow");

        protected static Shader ltsbaker    = Shader.Find("Hidden/ltsother_baker");
        protected static Shader ltspo       = Shader.Find("Hidden/ltspass_opaque");
        protected static Shader ltspc       = Shader.Find("Hidden/ltspass_cutout");
        protected static Shader ltspt       = Shader.Find("Hidden/ltspass_transparent");
        protected static Shader ltsptesso   = Shader.Find("Hidden/ltspass_tess_opaque");
        protected static Shader ltsptessc   = Shader.Find("Hidden/ltspass_tess_cutout");
        protected static Shader ltsptesst   = Shader.Find("Hidden/ltspass_tess_transparent");

        protected static Shader ltsm        = Shader.Find("_lil/lilToonMulti");
        protected static Shader ltsmref     = Shader.Find("Hidden/lilToonMultiRefraction");
        protected static Shader ltsmfur     = Shader.Find("Hidden/lilToonMultiFur");
        protected static Shader ltsmgem     = Shader.Find("Hidden/lilToonMultiGem");

        protected static Shader mtoon       = Shader.Find("VRM/MToon");

        //------------------------------------------------------------------------------------------------------------------------------
        // Editor
        public static bool isUPM = false;
        private static Dictionary<string, string> loc = new Dictionary<string, string>();
        private static lilToonSetting shaderSetting;
        private static lilToonPreset[] presets;
        private static Dictionary<string, MaterialProperty> copiedProperties = new Dictionary<string, MaterialProperty>();

        [Serializable]
        public class lilToonEditorSetting
        {
            public EditorMode editorMode = EditorMode.Simple;
            public int languageNum = -1;
            public string languageNames = "";
            public string languageName = "English";
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
            public bool isShowReflectionColorTex        = false;
            public bool isShowMatCapBlendMask           = false;
            public bool isShowMatCapBumpMap             = false;
            public bool isShowMatCap2ndBlendMask        = false;
            public bool isShowMatCap2ndBumpMap          = false;
            public bool isShowRimColorTex               = false;
            public bool isShowGlitterColorTex           = false;
            public bool isShowBacklightColorTex         = false;
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
            public bool isShowVRChat                    = false;
            public bool isShaderSettingChanged          = false;
            public bool[] isShowCategorys = new bool[(int)lilPresetCategory.Other+1]{false,false,false,false,false,false,false};
        }

        private static lilToonEditorSetting edSet = new lilToonEditorSetting();

        [Serializable]
        public class lilToonVersion
        {
            public string latest_vertion_name;
            public int latest_vertion_value;
        }
        private static lilToonVersion latestVersion = new lilToonVersion
        {
            latest_vertion_name = "",
            latest_vertion_value = 0
        };

        public struct lilPropertyBlockData
        {
            public lilPropertyBlock propertyBlock;
            public bool shouldCopyTex;
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Material
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

        //------------------------------------------------------------------------------------------------------------------------------
        // Material properties
        MaterialProperty transparentModeMat;
        MaterialProperty asOverlay;
        MaterialProperty invisible;
        MaterialProperty asUnlit;
        MaterialProperty cutoff;
        MaterialProperty subpassCutoff;
        MaterialProperty flipNormal;
        MaterialProperty shiftBackfaceUV;
        MaterialProperty backfaceForceShadow;
        MaterialProperty vertexLightStrength;
        MaterialProperty lightMinLimit;
        MaterialProperty lightMaxLimit;
        MaterialProperty beforeExposureLimit;
        MaterialProperty monochromeLighting;
        MaterialProperty lilDirectionalLightStrength;
        MaterialProperty lightDirectionOverride;
        MaterialProperty baseColor;
        MaterialProperty baseMap;
        MaterialProperty baseColorMap;
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
            MaterialProperty zclip;
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
            MaterialProperty alphaToMask;
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
            MaterialProperty alphaMaskScale;
            MaterialProperty alphaMaskValue;
        MaterialProperty useShadow;
            MaterialProperty shadowStrength;
            MaterialProperty shadowStrengthMask;
            MaterialProperty shadowAOShift;
            MaterialProperty shadowColor;
            MaterialProperty shadowColorTex;
            MaterialProperty shadowNormalStrength;
            MaterialProperty shadowBorder;
            MaterialProperty shadowBorderMask;
            MaterialProperty shadowBlur;
            MaterialProperty shadowBlurMask;
            MaterialProperty shadow2ndColor;
            MaterialProperty shadow2ndColorTex;
            MaterialProperty shadow2ndNormalStrength;
            MaterialProperty shadow2ndBorder;
            MaterialProperty shadow2ndBlur;
            MaterialProperty shadowMainStrength;
            MaterialProperty shadowEnvStrength;
            MaterialProperty shadowBorderColor;
            MaterialProperty shadowBorderRange;
            MaterialProperty shadowReceive;
        MaterialProperty useBacklight;
            MaterialProperty backlightColor;
            MaterialProperty backlightColorTex;
            MaterialProperty backlightNormalStrength;
            MaterialProperty backlightBorder;
            MaterialProperty backlightBlur;
            MaterialProperty backlightDirectivity;
            MaterialProperty backlightViewStrength;
            MaterialProperty backlightReceiveShadow;
            MaterialProperty backlightBackfaceMask;
        MaterialProperty useBumpMap;
            MaterialProperty bumpMap;
            MaterialProperty bumpScale;
        MaterialProperty useBump2ndMap;
            MaterialProperty bump2ndMap;
            MaterialProperty bump2ndScale;
            MaterialProperty bump2ndScaleMask;
        MaterialProperty useAnisotropy;
            MaterialProperty anisotropyTangentMap;
            MaterialProperty anisotropyScale;
            MaterialProperty anisotropyScaleMask;
            MaterialProperty anisotropyTangentWidth;
            MaterialProperty anisotropyBitangentWidth;
            MaterialProperty anisotropyShift;
            MaterialProperty anisotropyShiftNoiseScale;
            MaterialProperty anisotropySpecularStrength;
            MaterialProperty anisotropy2ndTangentWidth;
            MaterialProperty anisotropy2ndBitangentWidth;
            MaterialProperty anisotropy2ndShift;
            MaterialProperty anisotropy2ndShiftNoiseScale;
            MaterialProperty anisotropy2ndSpecularStrength;
            MaterialProperty anisotropyShiftNoiseMask;
            MaterialProperty anisotropy2Reflection;
            MaterialProperty anisotropy2MatCap;
            MaterialProperty anisotropy2MatCap2nd;
        MaterialProperty useReflection;
            MaterialProperty metallic;
            MaterialProperty metallicGlossMap;
            MaterialProperty smoothness;
            MaterialProperty smoothnessTex;
            MaterialProperty reflectance;
            MaterialProperty reflectionColor;
            MaterialProperty reflectionColorTex;
            MaterialProperty applySpecular;
            MaterialProperty applySpecularFA;
            MaterialProperty specularNormalStrength;
            MaterialProperty specularToon;
            MaterialProperty specularBorder;
            MaterialProperty specularBlur;
            MaterialProperty applyReflection;
            MaterialProperty reflectionNormalStrength;
            MaterialProperty reflectionApplyTransparency;
        MaterialProperty useMatCap;
            MaterialProperty matcapTex;
            MaterialProperty matcapColor;
            MaterialProperty matcapBlendUV1;
            MaterialProperty matcapZRotCancel;
            MaterialProperty matcapPerspective;
            MaterialProperty matcapVRParallaxStrength;
            MaterialProperty matcapBlend;
            MaterialProperty matcapBlendMask;
            MaterialProperty matcapEnableLighting;
            MaterialProperty matcapShadowMask;
            MaterialProperty matcapBackfaceMask;
            MaterialProperty matcapLod;
            MaterialProperty matcapBlendMode;
            MaterialProperty matcapMul;
            MaterialProperty matcapApplyTransparency;
            MaterialProperty matcapNormalStrength;
            MaterialProperty matcapCustomNormal;
            MaterialProperty matcapBumpMap;
            MaterialProperty matcapBumpScale;
        MaterialProperty useMatCap2nd;
            MaterialProperty matcap2ndTex;
            MaterialProperty matcap2ndColor;
            MaterialProperty matcap2ndBlendUV1;
            MaterialProperty matcap2ndZRotCancel;
            MaterialProperty matcap2ndPerspective;
            MaterialProperty matcap2ndVRParallaxStrength;
            MaterialProperty matcap2ndBlend;
            MaterialProperty matcap2ndBlendMask;
            MaterialProperty matcap2ndEnableLighting;
            MaterialProperty matcap2ndShadowMask;
            MaterialProperty matcap2ndBackfaceMask;
            MaterialProperty matcap2ndLod;
            MaterialProperty matcap2ndBlendMode;
            MaterialProperty matcap2ndMul;
            MaterialProperty matcap2ndApplyTransparency;
            MaterialProperty matcap2ndNormalStrength;
            MaterialProperty matcap2ndCustomNormal;
            MaterialProperty matcap2ndBumpMap;
            MaterialProperty matcap2ndBumpScale;
        MaterialProperty useRim;
            MaterialProperty rimColor;
            MaterialProperty rimColorTex;
            MaterialProperty rimNormalStrength;
            MaterialProperty rimBorder;
            MaterialProperty rimBlur;
            MaterialProperty rimFresnelPower;
            MaterialProperty rimEnableLighting;
            MaterialProperty rimShadowMask;
            MaterialProperty rimBackfaceMask;
            MaterialProperty rimVRParallaxStrength;
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
            MaterialProperty glitterBackfaceMask;
            MaterialProperty glitterApplyTransparency;
            MaterialProperty glitterVRParallaxStrength;
            MaterialProperty glitterNormalStrength;
        MaterialProperty useEmission;
            MaterialProperty emissionColor;
            MaterialProperty emissionMap;
            MaterialProperty emissionMap_ScrollRotate;
            MaterialProperty emissionMap_UVMode;
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
            MaterialProperty emission2ndMap_UVMode;
            MaterialProperty emission2ndBlend;
            MaterialProperty emission2ndBlendMask;
            MaterialProperty emission2ndBlendMask_ScrollRotate;
            MaterialProperty emission2ndBlink;
            MaterialProperty emission2ndUseGrad;
            MaterialProperty emission2ndGradTex;
            MaterialProperty emission2ndGradSpeed;
            MaterialProperty emission2ndParallaxDepth;
            MaterialProperty emission2ndFluorescence;
        MaterialProperty useOutline;
            MaterialProperty outlineColor;
            MaterialProperty outlineTex;
            MaterialProperty outlineTex_ScrollRotate;
            MaterialProperty outlineTexHSVG;
            MaterialProperty outlineWidth;
            MaterialProperty outlineWidthMask;
            MaterialProperty outlineFixWidth;
            MaterialProperty outlineVertexR2Width;
            MaterialProperty outlineVectorTex;
            MaterialProperty outlineVectorScale;
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
            MaterialProperty outlineZclip;
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
            MaterialProperty outlineAlphaToMask;
        MaterialProperty useParallax;
            MaterialProperty parallaxMap;
            MaterialProperty parallax;
            MaterialProperty parallaxOffset;
            MaterialProperty usePOM;
        //MaterialProperty useDistanceFade;
            MaterialProperty distanceFadeColor;
            MaterialProperty distanceFade;
        MaterialProperty useClippingCanceller;
        MaterialProperty useAudioLink;
            MaterialProperty audioLinkDefaultValue;
            MaterialProperty audioLinkUVMode;
            MaterialProperty audioLinkUVParams;
            MaterialProperty audioLinkStart;
            MaterialProperty audioLinkMask;
            MaterialProperty audioLink2Main2nd;
            MaterialProperty audioLink2Main3rd;
            MaterialProperty audioLink2Emission;
            MaterialProperty audioLink2EmissionGrad;
            MaterialProperty audioLink2Emission2nd;
            MaterialProperty audioLink2Emission2ndGrad;
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
            MaterialProperty furRootOffset;
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
            MaterialProperty furZclip;
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
            MaterialProperty furAlphaToMask;
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
        private Gradient mainGrad = new Gradient();
        private Gradient emiGrad = new Gradient();
        private Gradient emi2Grad = new Gradient();

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
            ResetProperties();

            //------------------------------------------------------------------------------------------------------------------------------
            // Load Material
            Material material = (Material)materialEditor.target;

            //------------------------------------------------------------------------------------------------------------------------------
            // Check Shader Type
            isLite          = material.shader.name.Contains("Lite");
            isCutout        = material.shader.name.Contains("Cutout");
            isTransparent   = material.shader.name.Contains("Transparent") || material.shader.name.Contains("Overlay");
            isOutl          = material.shader.name.Contains("Outline");
            isRefr          = material.shader.name.Contains("Refraction");
            isBlur          = material.shader.name.Contains("Blur");
            isFur           = material.shader.name.Contains("Fur");
            isTess          = material.shader.name.Contains("Tessellation");
            isGem           = material.shader.name.Contains("Gem");
            isFakeShadow    = material.shader.name.Contains("FakeShadow");
            isOnePass       = material.shader.name.Contains("OnePass");
            isTwoPass       = material.shader.name.Contains("TwoPass");
            isMulti         = material.shader.name.Contains("lilToonMulti");
            isCustomShader  = material.shader.name.Contains("Optional");

                                renderingModeBuf = RenderingMode.Opaque;
            if(isCutout)        renderingModeBuf = RenderingMode.Cutout;
            if(isTransparent)   renderingModeBuf = RenderingMode.Transparent;
            if(isRefr)          renderingModeBuf = RenderingMode.Refraction;
            if(isRefr&isBlur)   renderingModeBuf = RenderingMode.RefractionBlur;
            if(isFur)           renderingModeBuf = RenderingMode.Fur;
            if(isFur&isCutout)  renderingModeBuf = RenderingMode.FurCutout;
            if(isGem)           renderingModeBuf = RenderingMode.Gem;

                                transparentModeBuf = TransparentMode.Normal;
            if(isOnePass)       transparentModeBuf = TransparentMode.OnePass;
            if(isTwoPass)       transparentModeBuf = TransparentMode.TwoPass;

            //------------------------------------------------------------------------------------------------------------------------------
            // Multi shader
            if(isMulti)
            {
                LoadMultiProperties(props);

                isOutl = !material.shader.name.Contains("Refraction") && !material.shader.name.Contains("Fur") && !material.shader.name.Contains("Gem");
                if(transparentModeMat.floatValue == 1.0f)
                {
                    isCutout = true;
                }
                if(transparentModeMat.floatValue == 2.0f)
                {
                    isTransparent = true;
                }
                if(transparentModeMat.floatValue == 3.0f)
                {
                    isRefr = true;
                }
                if(transparentModeMat.floatValue == 4.0f)
                {
                    isFur = true;
                }
                if(transparentModeMat.floatValue == 5.0f)
                {
                    isCutout = true;
                    isFur = true;
                }
                if(transparentModeMat.floatValue == 6.0f)
                {
                    isGem = true;
                }
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Load Properties
            if(isLite)      LoadLiteProperties(props);
            else if(isFur)  LoadFurProperties(props);
            else if(isGem)  LoadGemProperties(props);
            else if(isFakeShadow) LoadFakeShadowProperties(props);
            else            LoadProperties(props);

            LoadCustomProperties(props, material);

            if(isMulti)         isOutl = false;

            //------------------------------------------------------------------------------------------------------------------------------
            // Stencil
            isStWr         = (stencilPass.floatValue == (float)UnityEngine.Rendering.StencilOp.Replace);

            //------------------------------------------------------------------------------------------------------------------------------
            // Remove Shader Keywords
            if(!isMulti) RemoveShaderKeywords(material);

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
            string sDistanceFadeSetting = GetLoc("sStartDistance") + "|" + GetLoc("sEndDistance") + "|" + GetLoc("sStrength") + "|" + GetLoc("sBackfaceForceShadow");
            string sDissolveParams = GetLoc("sDissolveMode") + "|" + GetLoc("sDissolveModeNone") + "|" + GetLoc("sDissolveModeAlpha") + "|" + GetLoc("sDissolveModeUV") + "|" + GetLoc("sDissolveModePosition") + "|" + GetLoc("sDissolveShape") + "|" + GetLoc("sDissolveShapePoint") + "|" + GetLoc("sDissolveShapeLine") + "|" + GetLoc("sBorder") + "|" + GetLoc("sBlur");
            string sDissolveParamsMode = GetLoc("sDissolve") + "|" + GetLoc("sDissolveModeNone") + "|" + GetLoc("sDissolveModeAlpha") + "|" + GetLoc("sDissolveModeUV") + "|" + GetLoc("sDissolveModePosition");
            string sDissolveParamsOther = GetLoc("sDissolveShape") + "|" + GetLoc("sDissolveShapePoint") + "|" + GetLoc("sDissolveShapeLine") + "|" + GetLoc("sBorder") + "|" + GetLoc("sBlur") + "|Dummy";
            string sGlitterParams1 = "Tiling" + "|" + GetLoc("sParticleSize") + "|" + GetLoc("sContrast");
            string sGlitterParams2 = GetLoc("sBlinkSpeed") + "|" + GetLoc("sAngleLimit") + "|" + GetLoc("sRimLightDirection") + "|" + GetLoc("sColorRandomness");
            string sTransparentMode = GetLoc("sRenderingMode") + "|" + GetLoc("sRenderingModeOpaque") + "|" + GetLoc("sRenderingModeCutout") + "|" + GetLoc("sRenderingModeTransparent") + "|" + GetLoc("sRenderingModeRefraction") + "|" + GetLoc("sRenderingModeFur") + "|" + GetLoc("sRenderingModeFurCutout") + "|" + GetLoc("sRenderingModeGem");
            string[] sRenderingModeList = {GetLoc("sRenderingModeOpaque"), GetLoc("sRenderingModeCutout"), GetLoc("sRenderingModeTransparent"), GetLoc("sRenderingModeRefraction"), GetLoc("sRenderingModeRefractionBlur"), GetLoc("sRenderingModeFur"), GetLoc("sRenderingModeFurCutout"), GetLoc("sRenderingModeGem")};
            string[] sRenderingModeListLite = {GetLoc("sRenderingModeOpaque"), GetLoc("sRenderingModeCutout"), GetLoc("sRenderingModeTransparent")};
            string[] sTransparentModeList = {GetLoc("sTransparentModeNormal"), GetLoc("sTransparentModeOnePass"), GetLoc("sTransparentModeTwoPass")};
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
                GUILayout.Label(" " + GetLoc("sBaseSetting"), EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(boxOuter);
                EditorGUILayout.LabelField(GetLoc("sBaseSetting"), customToggleFont);
                EditorGUILayout.BeginVertical(boxInnerHalf);
                materialEditor.ShaderProperty(invisible, GetLoc("sInvisible"));
                {
                    if(!isCustomShader && !isMulti)
                    {
                        RenderingMode renderingMode;
                        if(isLite) renderingMode = (RenderingMode)EditorGUILayout.Popup(GetLoc("sRenderingMode"), (int)renderingModeBuf, sRenderingModeListLite);
                        else       renderingMode = (RenderingMode)EditorGUILayout.Popup(GetLoc("sRenderingMode"), (int)renderingModeBuf, sRenderingModeList);
                        if(renderingModeBuf != renderingMode)
                        {
                            SetupMaterialWithRenderingMode(material, renderingMode, transparentModeBuf, isOutl, isLite, isStWr, isTess);
                            if(renderingMode == RenderingMode.Cutout || renderingMode == RenderingMode.FurCutout) cutoff.floatValue = 0.5f;
                            if(renderingMode == RenderingMode.Transparent || renderingMode == RenderingMode.Fur) cutoff.floatValue = 0.001f;
                        }
                    }
                    else if(isMulti)
                    {
                        float transparentModeMatBuf = transparentModeMat.floatValue;
                        materialEditor.ShaderProperty(transparentModeMat, sTransparentMode);
                        if(transparentModeMatBuf != transparentModeMat.floatValue)
                        {
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, isOutl, isLite, isStWr, isTess);
                            if(transparentModeMat.floatValue == 1.0f) cutoff.floatValue = 0.5f;
                            if(transparentModeMat.floatValue == 2.0f) cutoff.floatValue = 0.001f;
                        }
                    }
                        EditorGUI.indentLevel++;
                        if(renderingModeBuf == RenderingMode.Cutout || renderingModeBuf == RenderingMode.Transparent || renderingModeBuf == RenderingMode.Fur || renderingModeBuf == RenderingMode.FurCutout || isMulti && transparentModeMat.floatValue != 0.0f)
                        {
                            materialEditor.ShaderProperty(cutoff, GetLoc("sCutoff"));
                        }
                        if(renderingModeBuf >= RenderingMode.Transparent && renderingModeBuf != RenderingMode.FurCutout || isMulti && transparentModeMat.floatValue == 2.0f)
                        {
                            EditorGUILayout.HelpBox(GetLoc("sHelpRenderingTransparent"),MessageType.Warning);
                        }
                        EditorGUI.indentLevel--;
                    if(transparentModeBuf == TransparentMode.TwoPass)
                    {
                        cull.floatValue = 2.0f;
                    }
                    else
                    {
                        materialEditor.ShaderProperty(cull, sCullModes);
                        EditorGUI.indentLevel++;
                        if(cull.floatValue == 1.0f && AutoFixHelpBox(GetLoc("sHelpCullMode")))
                        {
                            cull.floatValue = 2.0f;
                        }
                        EditorGUI.indentLevel--;
                    }
                    materialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                        EditorGUI.indentLevel++;
                        if(zwrite.floatValue != 1.0f && AutoFixHelpBox(GetLoc("sHelpZWrite")))
                        {
                            zwrite.floatValue = 1.0f;
                        }
                        EditorGUI.indentLevel--;
                    DrawLine();
                    if(!isFakeShadow) DrawLightingSettings(materialEditor);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();

                //------------------------------------------------------------------------------------------------------------------------------
                // VRChat
                DrawVRCFallbackGUI(material, customToggleFont, boxOuter, boxInnerHalf);

                //------------------------------------------------------------------------------------------------------------------------------
                // Custom Properties
                if(isCustomShader)
                {
                    EditorGUILayout.Space();
                    GUILayout.Label(" " + GetLoc("sCustomProperties"), EditorStyles.boldLabel);
                    DrawCustomProperties(materialEditor, material, boxOuter, boxInnerHalf, boxInner, customBox, customToggleFont, offsetButton);
                }

                EditorGUILayout.Space();

                //------------------------------------------------------------------------------------------------------------------------------
                // Colors
                GUILayout.Label(" " + GetLoc("sColors"), EditorStyles.boldLabel);

                edSet.isShowMain = Foldout(GetLoc("sColors"), GetLoc("sMainColorTips"), edSet.isShowMain);
                if(edSet.isShowMain)
                {
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
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, !isOutl, isLite, isStWr, isTess);
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
                        if(CheckFeature(shaderSetting.LIL_FEATURE_SHADOW))
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            materialEditor.ShaderProperty(useShadow, GetLoc("sShadow"));
                            if(useShadow.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH)) materialEditor.TexturePropertySingleLine(maskStrengthContent, shadowStrengthMask, shadowStrength);
                                else                                                            materialEditor.ShaderProperty(shadowStrength, GetLoc("sStrength"));
                                if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST))      materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sShadow1stColor"), GetLoc("sTextureRGBA")), shadowColorTex, shadowColor);
                                else                                                            materialEditor.ShaderProperty(shadowColor, GetLoc("sShadow1stColor"));
                                if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND))      materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sShadow2ndColor"), GetLoc("sTextureRGBA")), shadow2ndColorTex, shadow2ndColor);
                                else                                                            materialEditor.ShaderProperty(shadow2ndColor, GetLoc("sShadow2ndColor"));
                                materialEditor.ShaderProperty(shadowMainStrength, GetLoc("sContrast"));
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
                        if(CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_1ST))
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
                        if(CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_2ND))
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
                            if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION))
                            {
                                ToneCorrectionGUI(materialEditor, material, mainTex, mainColor, mainTexHSVG);
                            }
                            if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP))
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
                            if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION) || CheckFeature(shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP))
                            {
                                materialEditor.TexturePropertySingleLine(adjustMaskContent, mainColorAdjustMask);
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
                            materialEditor.TexturePropertySingleLine(textureRGBAContent, main2ndTex, mainColor2nd);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }

                        // Main 3rd
                        if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN3RD) && useMain3rdTex.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField(GetLoc("sMainColor3rd"), customToggleFont);
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            materialEditor.TexturePropertySingleLine(textureRGBAContent, main3rdTex, mainColor3rd);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }

                        // Shadow
                        if(CheckFeature(shaderSetting.LIL_FEATURE_SHADOW))
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            materialEditor.ShaderProperty(useShadow, GetLoc("sShadow"));
                            if(useShadow.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH)) materialEditor.TexturePropertySingleLine(maskStrengthContent, shadowStrengthMask, shadowStrength);
                                else                                                            materialEditor.ShaderProperty(shadowStrength, GetLoc("sStrength"));
                                if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST))      materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sShadow1stColor"), GetLoc("sTextureRGBA")), shadowColorTex, shadowColor);
                                else                                                            materialEditor.ShaderProperty(shadowColor, GetLoc("sShadow1stColor"));
                                if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND))      materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sShadow2ndColor"), GetLoc("sTextureRGBA")), shadow2ndColorTex, shadow2ndColor);
                                else                                                            materialEditor.ShaderProperty(shadow2ndColor, GetLoc("sShadow2ndColor"));
                                if(shadow2ndColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                {
                                    shadow2ndColor.colorValue = new Color(shadow2ndColor.colorValue.r, shadow2ndColor.colorValue.g, shadow2ndColor.colorValue.b, 1.0f);
                                }
                                materialEditor.ShaderProperty(shadowMainStrength, GetLoc("sContrast"));
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
                                SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, !isOutl, isLite, isStWr, isTess);
                            }
                            if(isOutl)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR))   materialEditor.TexturePropertySingleLine(textureRGBAContent, outlineTex, outlineColor);
                                else                                                            materialEditor.ShaderProperty(outlineColor, GetLoc("sColor"));
                                if(CheckFeature(shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION))
                                {
                                    ToneCorrectionGUI(materialEditor, material, outlineTex, outlineColor, outlineTexHSVG);
                                    if(GUILayout.Button(GetLoc("sBake")))
                                    {
                                        outlineTex.textureValue = AutoBakeOutlineTexture(material);
                                        outlineTexHSVG.vectorValue = defaultHSVG;
                                    }
                                }
                                if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH))   materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sWidth"), GetLoc("sWidthR")), outlineWidthMask, outlineWidth);
                                else                                                            materialEditor.ShaderProperty(outlineWidth, GetLoc("sWidth"));
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }

                        // Emission
                        if(CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_1ST))
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
                        if(CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_2ND))
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
                    DrawMenuButton(GetLoc("sAnchorBaseSetting"), lilPropertyBlock.Base);
                    EditorGUILayout.BeginVertical(customBox);
                    {
                        materialEditor.ShaderProperty(invisible, GetLoc("sInvisible"));
                        if(!isCustomShader)
                        {
                            RenderingMode renderingMode;
                            renderingMode = (RenderingMode)EditorGUILayout.Popup(GetLoc("sRenderingMode"), (int)renderingModeBuf, sRenderingModeListLite);
                            if(renderingModeBuf != renderingMode)
                            {
                                SetupMaterialWithRenderingMode(material, renderingMode, transparentModeBuf, isOutl, isLite, isStWr, isTess);
                                if(renderingMode == RenderingMode.Cutout || renderingMode == RenderingMode.FurCutout) cutoff.floatValue = 0.5f;
                                if(renderingMode == RenderingMode.Transparent || renderingMode == RenderingMode.Fur) cutoff.floatValue = 0.001f;
                            }
                        }
                            EditorGUI.indentLevel++;
                            if(renderingModeBuf >= RenderingMode.Transparent && renderingModeBuf != RenderingMode.FurCutout)
                            {
                                EditorGUILayout.HelpBox(GetLoc("sHelpRenderingTransparent"),MessageType.Warning);
                            }
                            if(renderingModeBuf == RenderingMode.Cutout || renderingModeBuf == RenderingMode.Transparent || renderingModeBuf == RenderingMode.Fur || renderingModeBuf == RenderingMode.FurCutout)
                            {
                                materialEditor.ShaderProperty(cutoff, GetLoc("sCutoff"));
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
                            EditorGUI.indentLevel--;
                        if(transparentModeBuf == TransparentMode.TwoPass)
                        {
                            cull.floatValue = 2.0f;
                        }
                        else
                        {
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
                        }
                        materialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                            EditorGUI.indentLevel++;
                            if(zwrite.floatValue != 1.0f && AutoFixHelpBox(GetLoc("sHelpZWrite")))
                            {
                                zwrite.floatValue = 1.0f;
                            }
                            EditorGUI.indentLevel--;
                        materialEditor.RenderQueueField();
                        DrawLine();
                        DrawLightingSettings(materialEditor);
                        DrawLine();
                        materialEditor.TexturePropertySingleLine(triMaskContent, triMask);
                    }
                    EditorGUILayout.EndVertical();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // UV
                    edSet.isShowMainUV = Foldout(GetLoc("sMainUV"), GetLoc("sMainUVTips"), edSet.isShowMainUV);
                    DrawMenuButton(GetLoc("sAnchorUVSetting"), lilPropertyBlock.UV);
                    if(edSet.isShowMainUV)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainUV"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorUVSetting"), lilPropertyBlock.UV);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        UVSettingGUI(materialEditor, mainTex, mainTex_ScrollRotate);
                        materialEditor.ShaderProperty(shiftBackfaceUV, GetLoc("sShiftBackfaceUV"));
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // VRChat
                    DrawVRCFallbackGUI(material, customToggleFont, boxOuter, boxInnerHalf);

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Custom Properties
                    if(isCustomShader)
                    {
                        EditorGUILayout.Space();
                        GUILayout.Label(" " + GetLoc("sCustomProperties"), EditorStyles.boldLabel);
                        DrawCustomProperties(materialEditor, material, boxOuter, boxInnerHalf, boxInner, customBox, customToggleFont, offsetButton);
                    }

                    EditorGUILayout.Space();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Colors
                    GUILayout.Label(" " + GetLoc("sColors"), EditorStyles.boldLabel);

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Main Color
                    edSet.isShowMain = Foldout(GetLoc("sMainColorSetting"), GetLoc("sMainColorTips"), edSet.isShowMain);
                    DrawMenuButton(GetLoc("sAnchorMainColor"), lilPropertyBlock.MainColor);
                    if(edSet.isShowMain)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Main
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainColor"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorMainColor1"), lilPropertyBlock.MainColor1st);
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
                    DrawMenuButton(GetLoc("sAnchorShadow"), lilPropertyBlock.Shadow);
                    if(edSet.isShowShadow)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        materialEditor.ShaderProperty(useShadow, GetLoc("sShadow"));
                        DrawMenuButton(GetLoc("sAnchorShadow"), lilPropertyBlock.Shadow);
                        if(useShadow.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sShadow1stColor"), GetLoc("sTextureRGBA")), shadowColorTex);
                            materialEditor.ShaderProperty(shadowBorder, GetLoc("sBorder"));
                            materialEditor.ShaderProperty(shadowBlur, GetLoc("sBlur"));
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
                    DrawMenuButton(GetLoc("sAnchorEmission"), lilPropertyBlock.Emission);
                    if(edSet.isShowEmission)
                    {
                        // Emission
                        EditorGUILayout.BeginVertical(boxOuter);
                        materialEditor.ShaderProperty(useEmission, GetLoc("sEmission"));
                        DrawMenuButton(GetLoc("sAnchorEmission"), lilPropertyBlock.Emission1st);
                        if(useEmission.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            TextureGUI(materialEditor, ref edSet.isShowEmissionMap, colorRGBAContent, emissionMap, emissionColor, emissionMap_ScrollRotate, emissionMap_UVMode, true, true);
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
                    DrawMenuButton(GetLoc("sAnchorReflections"), lilPropertyBlock.Reflections);
                    if(edSet.isShowReflections)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // MatCap
                        EditorGUILayout.BeginVertical(boxOuter);
                        materialEditor.ShaderProperty(useMatCap, GetLoc("sMatCap"));
                        DrawMenuButton(GetLoc("sAnchorMatCap"), lilPropertyBlock.MatCap1st);
                        if(useMatCap.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            MatCapTextureGUI(materialEditor, ref edSet.isShowMatCapUV, new GUIContent(GetLoc("sMatCap"), GetLoc("sTextureRGBA")), matcapTex, matcapBlendUV1, matcapZRotCancel, matcapPerspective, matcapVRParallaxStrength);
                            materialEditor.ShaderProperty(matcapMul, GetLoc("sBlendModeMul"));
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Rim
                        EditorGUILayout.BeginVertical(boxOuter);
                        materialEditor.ShaderProperty(useRim, GetLoc("sRimLight"));
                        DrawMenuButton(GetLoc("sAnchorRimLight"), lilPropertyBlock.RimLight);
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
                    DrawMenuButton(GetLoc("sAnchorOutline"), lilPropertyBlock.Outline);
                    if(edSet.isShowOutline)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        if(isOutl != EditorGUILayout.ToggleLeft(GetLoc("sOutline"), isOutl, customToggleFont))
                        {
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, !isOutl, isLite, isStWr, isTess);
                        }
                        if(isOutl)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            TextureGUI(materialEditor, ref edSet.isShowOutlineMap, colorRGBAContent, outlineTex, outlineColor, outlineTex_ScrollRotate, true, true);
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
                                EditorGUILayout.LabelField(GetLoc("sOutline"), EditorStyles.boldLabel);
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
                                EditorGUILayout.LabelField(GetLoc("sOutline"), EditorStyles.boldLabel);
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
                                EditorGUILayout.LabelField(GetLoc("sOutline"), EditorStyles.boldLabel);
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
                    DrawMenuButton(GetLoc("sAnchorRendering"), lilPropertyBlock.Rendering);
                    if(edSet.isShowRendering)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Reset Button
                        if(GUILayout.Button(GetLoc("sRenderingReset"), offsetButton))
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
                            shaderType = EditorGUILayout.Popup(GetLoc("sShaderType"),shaderType,new String[]{GetLoc("sShaderTypeNormal"),GetLoc("sShaderTypeLite")});
                            if(shaderTypeBuf != shaderType)
                            {
                                if(shaderType==0) isLite = false;
                                if(shaderType==1) isLite = true;
                                SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, isOutl, isLite, isStWr, isTess);
                            }

                            //------------------------------------------------------------------------------------------------------------------------------
                            // Rendering
                            if(renderingModeBuf == RenderingMode.Transparent || renderingModeBuf == RenderingMode.Fur)
                            {
                                materialEditor.ShaderProperty(subpassCutoff, GetLoc("sSubpassCutoff"));
                            }
                            materialEditor.ShaderProperty(cull, sCullModes);
                            materialEditor.ShaderProperty(zclip, GetLoc("sZClip"));
                            materialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                            materialEditor.ShaderProperty(ztest, GetLoc("sZTest"));
                            materialEditor.ShaderProperty(offsetFactor, GetLoc("sOffsetFactor"));
                            materialEditor.ShaderProperty(offsetUnits, GetLoc("sOffsetUnits"));
                            materialEditor.ShaderProperty(colorMask, GetLoc("sColorMask"));
                            materialEditor.ShaderProperty(alphaToMask, GetLoc("sAlphaToMask"));
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
                            materialEditor.ShaderProperty(beforeExposureLimit, GetLoc("sBeforeExposureLimit"));
                            materialEditor.ShaderProperty(lilDirectionalLightStrength, GetLoc("sDirectionalLightStrength"));
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
                            materialEditor.ShaderProperty(outlineZclip, GetLoc("sZClip"));
                            materialEditor.ShaderProperty(outlineZwrite, GetLoc("sZWrite"));
                            materialEditor.ShaderProperty(outlineZtest, GetLoc("sZTest"));
                            materialEditor.ShaderProperty(outlineOffsetFactor, GetLoc("sOffsetFactor"));
                            materialEditor.ShaderProperty(outlineOffsetUnits, GetLoc("sOffsetUnits"));
                            materialEditor.ShaderProperty(outlineColorMask, GetLoc("sColorMask"));
                            materialEditor.ShaderProperty(outlineAlphaToMask, GetLoc("sAlphaToMask"));
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
                else if(isFakeShadow)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // Base Setting
                    GUILayout.Label(" " + GetLoc("sBaseSetting"), EditorStyles.boldLabel);
                    DrawMenuButton(GetLoc("sAnchorBaseSetting"), lilPropertyBlock.Base);
                    EditorGUILayout.BeginVertical(customBox);
                    {
                        materialEditor.ShaderProperty(invisible, GetLoc("sInvisible"));
                        materialEditor.ShaderProperty(cull, sCullModes);
                        materialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                        GUILayout.Label("FakeShadow", EditorStyles.boldLabel);
                        materialEditor.ShaderProperty(fakeShadowVector, GetLoc("sVector") + "|" + GetLoc("sOffset"));
                        materialEditor.RenderQueueField();
                    }
                    EditorGUILayout.EndVertical();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // UV
                    edSet.isShowMainUV = Foldout(GetLoc("sMainUV"), GetLoc("sMainUVTips"), edSet.isShowMainUV);
                    DrawMenuButton(GetLoc("sAnchorUVSetting"), lilPropertyBlock.UV);
                    if(edSet.isShowMainUV)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainUV"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorUVSetting"), lilPropertyBlock.UV);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TextureScaleOffsetProperty(mainTex);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // VRChat
                    DrawVRCFallbackGUI(material, customToggleFont, boxOuter, boxInnerHalf);

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Custom Properties
                    if(isCustomShader)
                    {
                        EditorGUILayout.Space();
                        GUILayout.Label(" " + GetLoc("sCustomProperties"), EditorStyles.boldLabel);
                        DrawCustomProperties(materialEditor, material, boxOuter, boxInnerHalf, boxInner, customBox, customToggleFont, offsetButton);
                    }

                    EditorGUILayout.Space();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Colors
                    GUILayout.Label(" " + GetLoc("sColors"), EditorStyles.boldLabel);

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Main Color
                    edSet.isShowMain = Foldout(GetLoc("sMainColorSetting"), GetLoc("sMainColorTips"), edSet.isShowMain);
                    DrawMenuButton(GetLoc("sAnchorMainColor"), lilPropertyBlock.MainColor);
                    if(edSet.isShowMain)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Main
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainColor"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorMainColor1"), lilPropertyBlock.MainColor1st);
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
                        DrawMenuButton(GetLoc("sAnchorEncryption"), lilPropertyBlock.Encryption);
                        if(edSet.isShowEncryption)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField(GetLoc("sEncryption"), customToggleFont);
                            DrawMenuButton(GetLoc("sAnchorEncryption"), lilPropertyBlock.Encryption);
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
                    DrawMenuButton(GetLoc("sAnchorRendering"), lilPropertyBlock.Rendering);
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
                            materialEditor.ShaderProperty(cull, sCullModes);
                            materialEditor.ShaderProperty(zclip, GetLoc("sZClip"));
                            materialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                            materialEditor.ShaderProperty(ztest, GetLoc("sZTest"));
                            materialEditor.ShaderProperty(offsetFactor, GetLoc("sOffsetFactor"));
                            materialEditor.ShaderProperty(offsetUnits, GetLoc("sOffsetUnits"));
                            materialEditor.ShaderProperty(colorMask, GetLoc("sColorMask"));
                            materialEditor.ShaderProperty(alphaToMask, GetLoc("sAlphaToMask"));
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
                    DrawMenuButton(GetLoc("sAnchorBaseSetting"), lilPropertyBlock.Base);
                    EditorGUILayout.BeginVertical(customBox);
                    {
                        materialEditor.ShaderProperty(invisible, GetLoc("sInvisible"));
                        if(!isCustomShader && !isMulti)
                        {
                            RenderingMode renderingMode;
                            renderingMode = (RenderingMode)EditorGUILayout.Popup(GetLoc("sRenderingMode"), (int)renderingModeBuf, sRenderingModeList);
                            if(renderingModeBuf != renderingMode)
                            {
                                SetupMaterialWithRenderingMode(material, renderingMode, transparentModeBuf, isOutl, isLite, isStWr, isTess);
                                if(renderingMode == RenderingMode.Cutout || renderingMode == RenderingMode.FurCutout) cutoff.floatValue = 0.5f;
                                if(renderingMode == RenderingMode.Transparent || renderingMode == RenderingMode.Fur) cutoff.floatValue = 0.001f;
                            }
                        }
                        else if(isMulti)
                        {
                            float transparentModeMatBuf = transparentModeMat.floatValue;
                            materialEditor.ShaderProperty(transparentModeMat, sTransparentMode);
                            materialEditor.ShaderProperty(asOverlay, GetLoc("sAsOverlay"));
                            if(transparentModeMatBuf != transparentModeMat.floatValue)
                            {
                                SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, isOutl, isLite, isStWr, isTess);
                                if(transparentModeMat.floatValue == 1.0f || transparentModeMat.floatValue == 5.0f) cutoff.floatValue = 0.5f;
                                if(transparentModeMat.floatValue == 2.0f || transparentModeMat.floatValue == 4.0f) cutoff.floatValue = 0.001f;
                            }
                        }
                            EditorGUI.indentLevel++;
                            if(renderingModeBuf >= RenderingMode.Transparent && renderingModeBuf != RenderingMode.FurCutout || isMulti && transparentModeMat.floatValue == 2.0f)
                            {
                                EditorGUILayout.HelpBox(GetLoc("sHelpRenderingTransparent"),MessageType.Warning);
                            }
                            if(renderingModeBuf == RenderingMode.Cutout || renderingModeBuf == RenderingMode.Transparent || renderingModeBuf == RenderingMode.Fur || renderingModeBuf == RenderingMode.FurCutout || isMulti && transparentModeMat.floatValue != 0.0f && transparentModeMat.floatValue != 3.0f && transparentModeMat.floatValue != 6.0f)
                            {
                                materialEditor.ShaderProperty(cutoff, GetLoc("sCutoff"));
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
                            EditorGUI.indentLevel--;
                        if(transparentModeBuf == TransparentMode.TwoPass)
                        {
                            cull.floatValue = 2.0f;
                        }
                        else if(!isGem)
                        {
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
                        }
                        materialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                            EditorGUI.indentLevel++;
                            if(zwrite.floatValue != 1.0f && !isGem && AutoFixHelpBox(GetLoc("sHelpZWrite")))
                            {
                                zwrite.floatValue = 1.0f;
                            }
                            EditorGUI.indentLevel--;
                        materialEditor.RenderQueueField();
                        if(isMulti) materialEditor.ShaderProperty(useClippingCanceller, GetLoc("sSettingClippingCanceller"));
                        DrawLine();
                        DrawLightingSettings(materialEditor);
                    }
                    EditorGUILayout.EndVertical();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // UV
                    edSet.isShowMainUV = Foldout(GetLoc("sMainUV"), GetLoc("sMainUVTips"), edSet.isShowMainUV);
                    DrawMenuButton(GetLoc("sAnchorUVSetting"), lilPropertyBlock.UV);
                    if(edSet.isShowMainUV)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainUV"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorUVSetting"), lilPropertyBlock.UV);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        if(CheckFeature(shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV)) UVSettingGUI(materialEditor, mainTex, mainTex_ScrollRotate);
                        else                                                        materialEditor.TextureScaleOffsetProperty(mainTex);
                        materialEditor.ShaderProperty(shiftBackfaceUV, GetLoc("sShiftBackfaceUV"));
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // VRChat
                    DrawVRCFallbackGUI(material, customToggleFont, boxOuter, boxInnerHalf);

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Custom Properties
                    if(isCustomShader)
                    {
                        EditorGUILayout.Space();
                        GUILayout.Label(" " + GetLoc("sCustomProperties"), EditorStyles.boldLabel);
                        DrawCustomProperties(materialEditor, material, boxOuter, boxInnerHalf, boxInner, customBox, customToggleFont, offsetButton);
                    }

                    EditorGUILayout.Space();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Colors
                    GUILayout.Label(" " + GetLoc("sColors"), EditorStyles.boldLabel);

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Main Color
                    edSet.isShowMain = Foldout(GetLoc("sMainColorSetting"), GetLoc("sMainColorTips"), edSet.isShowMain);
                    DrawMenuButton(GetLoc("sAnchorMainColor"), lilPropertyBlock.MainColor);
                    if(edSet.isShowMain)
                    {
                        if(isFur || isGem)
                        {
                            //------------------------------------------------------------------------------------------------------------------------------
                            // Main
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField(GetLoc("sMainColor"), customToggleFont);
                            DrawMenuButton(GetLoc("sAnchorMainColor1"), lilPropertyBlock.MainColor1st);
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
                            DrawMenuButton(GetLoc("sAnchorMainColor1"), lilPropertyBlock.MainColor1st);
                            //materialEditor.ShaderProperty(useMainTex, GetLoc("sMainColor"));
                            //if(useMainTex.floatValue == 1)
                            //{
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                materialEditor.TexturePropertySingleLine(textureRGBAContent, mainTex, mainColor);
                                if(isCutout || isTransparent)
                                {
                                    SetAlphaIsTransparencyGUI(mainTex);
                                }
                                if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION))
                                {
                                    ToneCorrectionGUI(materialEditor, material, mainTex, mainColor, mainTexHSVG);
                                }
                                if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP))
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
                                if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION) || CheckFeature(shaderSetting.LIL_FEATURE_MAIN_GRADATION_MAP))
                                {
                                    materialEditor.TexturePropertySingleLine(adjustMaskContent, mainColorAdjustMask);
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
                                materialEditor.ShaderProperty(useMain2ndTex, GetLoc("sMainColor2nd"));
                                DrawMenuButton(GetLoc("sAnchorMainColor2"), lilPropertyBlock.MainColor2nd);
                                if(useMain2ndTex.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    materialEditor.TexturePropertySingleLine(textureRGBAContent, main2ndTex, mainColor2nd);
                                    materialEditor.ShaderProperty(main2ndTexIsMSDF, GetLoc("sAsMSDF"));
                                    DrawLine();
                                    UV4Decal(materialEditor, main2ndTexIsDecal, main2ndTexIsLeftOnly, main2ndTexIsRightOnly, main2ndTexShouldCopy, main2ndTexShouldFlipMirror, main2ndTexShouldFlipCopy, main2ndTex, main2ndTexAngle, main2ndTexDecalAnimation, main2ndTexDecalSubParam);
                                    DrawLine();
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_LAYER_MASK)) materialEditor.TexturePropertySingleLine(maskBlendContent, main2ndBlendMask);
                                    EditorGUILayout.LabelField(GetLoc("sDistanceFade"));
                                    EditorGUI.indentLevel++;
                                    materialEditor.ShaderProperty(main2ndDistanceFade, sDistanceFadeSetting);
                                    EditorGUI.indentLevel--;
                                    materialEditor.ShaderProperty(main2ndEnableLighting, GetLoc("sEnableLighting"));
                                    materialEditor.ShaderProperty(main2ndTexBlendMode, sBlendModes);
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_LAYER_DISSOLVE))
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
                            if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN3RD))
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                materialEditor.ShaderProperty(useMain3rdTex, GetLoc("sMainColor3rd"));
                                DrawMenuButton(GetLoc("sAnchorMainColor2"), lilPropertyBlock.MainColor3rd);
                                if(useMain3rdTex.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    materialEditor.TexturePropertySingleLine(textureRGBAContent, main3rdTex, mainColor3rd);
                                    materialEditor.ShaderProperty(main3rdTexIsMSDF, GetLoc("sAsMSDF"));
                                    DrawLine();
                                    UV4Decal(materialEditor, main3rdTexIsDecal, main3rdTexIsLeftOnly, main3rdTexIsRightOnly, main3rdTexShouldCopy, main3rdTexShouldFlipMirror, main3rdTexShouldFlipCopy, main3rdTex, main3rdTexAngle, main3rdTexDecalAnimation, main3rdTexDecalSubParam);
                                    DrawLine();
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_LAYER_MASK)) materialEditor.TexturePropertySingleLine(maskBlendContent, main3rdBlendMask);
                                    EditorGUILayout.LabelField(GetLoc("sDistanceFade"));
                                    EditorGUI.indentLevel++;
                                    materialEditor.ShaderProperty(main3rdDistanceFade, sDistanceFadeSetting);
                                    EditorGUI.indentLevel--;
                                    materialEditor.ShaderProperty(main3rdEnableLighting, GetLoc("sEnableLighting"));
                                    materialEditor.ShaderProperty(main3rdTexBlendMode, sBlendModes);
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_LAYER_DISSOLVE))
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
                                            if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE)) TextureGUI(materialEditor, ref edSet.isShowMain3rdDissolveNoiseMask, noiseMaskContent, main3rdDissolveNoiseMask, main3rdDissolveNoiseStrength, main3rdDissolveNoiseMask_ScrollRotate);
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
                            if(CheckFeature(shaderSetting.LIL_FEATURE_ALPHAMASK))
                            {
                                if(renderingModeBuf == RenderingMode.Opaque && !isMulti || isMulti && transparentModeMat.floatValue == 0.0f)
                                {
                                    GUILayout.Label(GetLoc("sAlphaMaskWarnOpaque"), wrapLabel);
                                }
                                else
                                {
                                    EditorGUILayout.BeginVertical(boxOuter);
                                    materialEditor.ShaderProperty(alphaMaskMode, sAlphaMaskModes);
                                    DrawMenuButton(GetLoc("sAnchorAlphaMask"), lilPropertyBlock.AlphaMask);
                                    if(alphaMaskMode.floatValue != 0)
                                    {
                                        EditorGUILayout.BeginVertical(boxInnerHalf);
                                        materialEditor.TexturePropertySingleLine(alphaMaskContent, alphaMask);
                                        materialEditor.ShaderProperty(alphaMaskScale, "Scale");
                                        materialEditor.ShaderProperty(alphaMaskValue, "Offset");
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
                    if(!isGem && CheckFeature(shaderSetting.LIL_FEATURE_SHADOW))
                    {
                        edSet.isShowShadow = Foldout(GetLoc("sShadowSetting"), GetLoc("sShadowTips"), edSet.isShowShadow);
                        DrawMenuButton(GetLoc("sAnchorShadow"), lilPropertyBlock.Shadow);
                        if(edSet.isShowShadow)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            materialEditor.ShaderProperty(useShadow, GetLoc("sShadow"));
                            DrawMenuButton(GetLoc("sAnchorShadow"), lilPropertyBlock.Shadow);
                            if(useShadow.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH)) materialEditor.TexturePropertySingleLine(maskStrengthContent, shadowStrengthMask, shadowStrength);
                                else                                                            materialEditor.ShaderProperty(shadowStrength, GetLoc("sStrength"));
                                DrawLine();
                                if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST))      materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sShadow1stColor"), GetLoc("sTextureRGBA")), shadowColorTex, shadowColor);
                                else                                                            materialEditor.ShaderProperty(shadowColor, GetLoc("sShadow1stColor"));
                                materialEditor.ShaderProperty(shadowBorder, GetLoc("sBorder"));
                                if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR))     materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sBlur"), GetLoc("sBlurR")), shadowBlurMask, shadowBlur);
                                else                                                            materialEditor.ShaderProperty(shadowBlur, GetLoc("sBlur"));
                                materialEditor.ShaderProperty(shadowNormalStrength, GetLoc("sNormalStrength"));
                                DrawLine();
                                if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND))      materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sShadow2ndColor"), GetLoc("sTextureRGBA")), shadow2ndColorTex, shadow2ndColor);
                                else                                                            materialEditor.ShaderProperty(shadow2ndColor, GetLoc("sShadow2ndColor"));
                                if(shadow2ndColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                {
                                    shadow2ndColor.colorValue = new Color(shadow2ndColor.colorValue.r, shadow2ndColor.colorValue.g, shadow2ndColor.colorValue.b, 1.0f);
                                }
                                materialEditor.ShaderProperty(shadow2ndBorder, GetLoc("sBorder"));
                                materialEditor.ShaderProperty(shadow2ndBlur, GetLoc("sBlur"));
                                materialEditor.ShaderProperty(shadow2ndNormalStrength, GetLoc("sNormalStrength"));
                                DrawLine();
                                materialEditor.ShaderProperty(shadowBorderColor, GetLoc("sShadowBorderColor"));
                                materialEditor.ShaderProperty(shadowBorderRange, GetLoc("sShadowBorderRange"));
                                if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER))
                                {
                                    DrawLine();
                                    materialEditor.TexturePropertySingleLine(new GUIContent("AO Map", GetLoc("sBorderR")), shadowBorderMask);
                                    EditorGUI.indentLevel++;
                                    materialEditor.ShaderProperty(shadowAOShift, "1st Scale|1st Offset|2nd Scale|2nd Offset");
                                    EditorGUI.indentLevel--;
                                }
                                DrawLine();
                                materialEditor.ShaderProperty(shadowMainStrength, GetLoc("sContrast"));
                                materialEditor.ShaderProperty(shadowEnvStrength, GetLoc("sShadowEnvStrength"));
                                if(CheckFeature(shaderSetting.LIL_FEATURE_RECEIVE_SHADOW)) materialEditor.ShaderProperty(shadowReceive, GetLoc("sReceiveShadow"));
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Emission
                    if(!isFur && (CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_1ST) || CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_2ND)))
                    {
                        edSet.isShowEmission = Foldout(GetLoc("sEmissionSetting"), GetLoc("sEmissionTips"), edSet.isShowEmission);
                        DrawMenuButton(GetLoc("sAnchorEmission"), lilPropertyBlock.Emission);
                        if(edSet.isShowEmission)
                        {
                            // Emission
                            if(CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_1ST))
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                materialEditor.ShaderProperty(useEmission, GetLoc("sEmission"));
                                DrawMenuButton(GetLoc("sAnchorEmission"), lilPropertyBlock.Emission1st);
                                if(useEmission.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    TextureGUI(materialEditor, ref edSet.isShowEmissionMap, colorRGBAContent, emissionMap, emissionColor, emissionMap_ScrollRotate, emissionMap_UVMode, true, CheckFeature(shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV));
                                    if(emissionColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                    {
                                        emissionColor.colorValue = new Color(emissionColor.colorValue.r, emissionColor.colorValue.g, emissionColor.colorValue.b, 1.0f);
                                    }
                                    DrawLine();
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK))
                                    {
                                        TextureGUI(materialEditor, ref edSet.isShowEmissionBlendMask, maskBlendContent, emissionBlendMask, emissionBlend, emissionBlendMask_ScrollRotate, true, CheckFeature(shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV));
                                        DrawLine();
                                    }
                                    materialEditor.ShaderProperty(emissionBlink, blinkSetting);
                                    DrawLine();
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_GRADATION))
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
                            if(CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_2ND))
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                materialEditor.ShaderProperty(useEmission2nd, GetLoc("sEmission2nd"));
                                DrawMenuButton(GetLoc("sAnchorEmission"), lilPropertyBlock.Emission2nd);
                                if(useEmission2nd.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    TextureGUI(materialEditor, ref edSet.isShowEmission2ndMap, colorRGBAContent, emission2ndMap, emission2ndColor, emission2ndMap_ScrollRotate, emission2ndMap_UVMode, true, CheckFeature(shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV));
                                    if(emission2ndColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                    {
                                        emission2ndColor.colorValue = new Color(emission2ndColor.colorValue.r, emission2ndColor.colorValue.g, emission2ndColor.colorValue.b, 1.0f);
                                    }
                                    DrawLine();
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK))
                                    {
                                        TextureGUI(materialEditor, ref edSet.isShowEmission2ndBlendMask, maskBlendContent, emission2ndBlendMask, emission2ndBlend, emission2ndBlendMask_ScrollRotate, true, CheckFeature(shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV));
                                        DrawLine();
                                    }
                                    materialEditor.ShaderProperty(emission2ndBlink, blinkSetting);
                                    DrawLine();
                                    if(CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_GRADATION))
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
                        if(CheckFeature(shaderSetting.LIL_FEATURE_NORMAL_1ST) || CheckFeature(shaderSetting.LIL_FEATURE_NORMAL_2ND) || CheckFeature(shaderSetting.LIL_FEATURE_REFLECTION) || CheckFeature(shaderSetting.LIL_FEATURE_MATCAP) || CheckFeature(shaderSetting.LIL_FEATURE_MATCAP_2ND) || CheckFeature(shaderSetting.LIL_FEATURE_RIMLIGHT))
                        {
                            GUILayout.Label(" " + GetLoc("sNormalMapReflection"), EditorStyles.boldLabel);
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Normal
                        if(CheckFeature(shaderSetting.LIL_FEATURE_NORMAL_1ST) || CheckFeature(shaderSetting.LIL_FEATURE_NORMAL_2ND) || CheckFeature(shaderSetting.LIL_FEATURE_ANISOTROPY))
                        {
                            edSet.isShowBump = Foldout(GetLoc("sNormalMapSetting"), GetLoc("sNormalMapTips"), edSet.isShowBump);
                            DrawMenuButton(GetLoc("sAnchorNormalMap"), lilPropertyBlock.NormalMap);
                            if(edSet.isShowBump)
                            {
                                //------------------------------------------------------------------------------------------------------------------------------
                                // 1st
                                if(CheckFeature(shaderSetting.LIL_FEATURE_NORMAL_1ST))
                                {
                                    EditorGUILayout.BeginVertical(boxOuter);
                                    materialEditor.ShaderProperty(useBumpMap, GetLoc("sNormalMap"));
                                    DrawMenuButton(GetLoc("sAnchorNormalMap"), lilPropertyBlock.NormalMap1st);
                                    if(useBumpMap.floatValue == 1)
                                    {
                                        EditorGUILayout.BeginVertical(boxInnerHalf);
                                        TextureGUI(materialEditor, ref edSet.isShowBumpMap, normalMapContent, bumpMap, bumpScale);
                                        EditorGUILayout.EndVertical();
                                    }
                                    EditorGUILayout.EndVertical();
                                }

                                //------------------------------------------------------------------------------------------------------------------------------
                                // 2nd
                                if(CheckFeature(shaderSetting.LIL_FEATURE_NORMAL_2ND))
                                {
                                    EditorGUILayout.BeginVertical(boxOuter);
                                    materialEditor.ShaderProperty(useBump2ndMap, GetLoc("sNormalMap2nd"));
                                    DrawMenuButton(GetLoc("sAnchorNormalMap"), lilPropertyBlock.NormalMap2nd);
                                    if(useBump2ndMap.floatValue == 1)
                                    {
                                        EditorGUILayout.BeginVertical(boxInnerHalf);
                                        TextureGUI(materialEditor, ref edSet.isShowBump2ndMap, normalMapContent, bump2ndMap, bump2ndScale);
                                        if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK))
                                        {
                                            DrawLine();
                                            TextureGUI(materialEditor, ref edSet.isShowBump2ndScaleMask, maskStrengthContent, bump2ndScaleMask);
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
                                    materialEditor.ShaderProperty(useAnisotropy, GetLoc("sAnisotropy"));
                                    DrawMenuButton(GetLoc("sAnchorAnisotropy"), lilPropertyBlock.Anisotropy);
                                    if(useAnisotropy.floatValue == 1)
                                    {
                                        EditorGUILayout.BeginVertical(boxInnerHalf);
                                        TextureGUI(materialEditor, ref edSet.isShowAnisotropyTangentMap, normalMapContent, anisotropyTangentMap);
                                        DrawLine();
                                        TextureGUI(materialEditor, ref edSet.isShowAnisotropyScaleMask, maskStrengthContent, anisotropyScaleMask, anisotropyScale);
                                        DrawLine();
                                        GUILayout.Label(GetLoc("sApplyTo"), EditorStyles.boldLabel);
                                        EditorGUI.indentLevel++;
                                        materialEditor.ShaderProperty(anisotropy2Reflection, GetLoc("sReflection"));
                                        if(anisotropy2Reflection.floatValue != 0.0f)
                                        {
                                            EditorGUI.indentLevel++;
                                            EditorGUILayout.LabelField("1st Specular", EditorStyles.boldLabel);
                                            materialEditor.ShaderProperty(anisotropyTangentWidth, GetLoc("sTangentWidth"));
                                            materialEditor.ShaderProperty(anisotropyBitangentWidth, GetLoc("sBitangentWidth"));
                                            materialEditor.ShaderProperty(anisotropyShift, GetLoc("sOffset"));
                                            materialEditor.ShaderProperty(anisotropyShiftNoiseScale, GetLoc("sNoiseStrength"));
                                            materialEditor.ShaderProperty(anisotropySpecularStrength, GetLoc("sStrength"));
                                            DrawLine();
                                            EditorGUILayout.LabelField("2nd Specular", EditorStyles.boldLabel);
                                            materialEditor.ShaderProperty(anisotropy2ndTangentWidth, GetLoc("sTangentWidth"));
                                            materialEditor.ShaderProperty(anisotropy2ndBitangentWidth, GetLoc("sBitangentWidth"));
                                            materialEditor.ShaderProperty(anisotropy2ndShift, GetLoc("sOffset"));
                                            materialEditor.ShaderProperty(anisotropy2ndShiftNoiseScale, GetLoc("sNoiseStrength"));
                                            materialEditor.ShaderProperty(anisotropy2ndSpecularStrength, GetLoc("sStrength"));
                                            DrawLine();
                                            materialEditor.ShaderProperty(anisotropyShiftNoiseMask, GetLoc("sNoise"));
                                            EditorGUI.indentLevel--;
                                        }
                                        materialEditor.ShaderProperty(anisotropy2MatCap, GetLoc("sMatCap"));
                                        materialEditor.ShaderProperty(anisotropy2MatCap2nd, GetLoc("sMatCap2nd"));
                                        EditorGUI.indentLevel--;
                                        EditorGUILayout.EndVertical();
                                    }
                                    EditorGUILayout.EndVertical();
                                }
                            }
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Reflection
                        if(CheckFeature(shaderSetting.LIL_FEATURE_REFLECTION) || CheckFeature(shaderSetting.LIL_FEATURE_MATCAP) || CheckFeature(shaderSetting.LIL_FEATURE_MATCAP_2ND) || CheckFeature(shaderSetting.LIL_FEATURE_RIMLIGHT) || CheckFeature(shaderSetting.LIL_FEATURE_GLITTER) || CheckFeature(shaderSetting.LIL_FEATURE_BACKLIGHT))
                        {
                            edSet.isShowReflections = Foldout(GetLoc("sReflectionsSetting"), GetLoc("sReflectionsTips"), edSet.isShowReflections);
                            DrawMenuButton(GetLoc("sAnchorReflections"), lilPropertyBlock.Reflections);
                            if(edSet.isShowReflections)
                            {
                                //------------------------------------------------------------------------------------------------------------------------------
                                // Reflection
                                if(!isGem && CheckFeature(shaderSetting.LIL_FEATURE_REFLECTION))
                                {
                                    EditorGUILayout.BeginVertical(boxOuter);
                                    materialEditor.ShaderProperty(useReflection, GetLoc("sReflection"));
                                    DrawMenuButton(GetLoc("sAnchorReflection"), lilPropertyBlock.Reflection);
                                    if(useReflection.floatValue == 1)
                                    {
                                        EditorGUILayout.BeginVertical(boxInnerHalf);
                                        if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS))   TextureGUI(materialEditor, ref edSet.isShowSmoothnessTex, new GUIContent(GetLoc("sSmoothness"), GetLoc("sSmoothnessR")), smoothnessTex, smoothness);
                                        else                                                                    materialEditor.ShaderProperty(smoothness, GetLoc("sSmoothness"));
                                        DrawLine();
                                        if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC))     TextureGUI(materialEditor, ref edSet.isShowMetallicGlossMap, new GUIContent(GetLoc("sMetallic"), GetLoc("sMetallicR")), metallicGlossMap, metallic);
                                        else                                                                    materialEditor.ShaderProperty(metallic, GetLoc("sMetallic"));
                                        DrawLine();
                                        if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR))        TextureGUI(materialEditor, ref edSet.isShowReflectionColorTex, colorRGBAContent, reflectionColorTex, reflectionColor);
                                        else                                                                    materialEditor.ShaderProperty(reflectionColor, GetLoc("sColor"));
                                        DrawLine();
                                        materialEditor.ShaderProperty(reflectance, GetLoc("sReflectance"));
                                        if(reflectionColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                        {
                                            reflectionColor.colorValue = new Color(reflectionColor.colorValue.r, reflectionColor.colorValue.g, reflectionColor.colorValue.b, 1.0f);
                                        }
                                        int specularMode = 0;
                                        if(specularToon.floatValue == 0.0f) specularMode = 1;
                                        if(specularToon.floatValue == 1.0f) specularMode = 2;
                                        if(applySpecular.floatValue == 0.0f) specularMode = 0;
                                        specularMode = EditorGUILayout.Popup(GetLoc("sSpecularMode"),specularMode,new String[]{GetLoc("sSpecularNone"),GetLoc("sSpecularReal"),GetLoc("sSpecularToon")});
                                        if(specularMode == 0)
                                        {
                                            applySpecular.floatValue = 0.0f;
                                        }
                                        if(specularMode == 1)
                                        {
                                            applySpecular.floatValue = 1.0f;
                                            specularToon.floatValue = 0.0f;
                                            EditorGUI.indentLevel++;
                                            materialEditor.ShaderProperty(specularNormalStrength, GetLoc("sNormalStrength"));
                                            materialEditor.ShaderProperty(applySpecularFA, GetLoc("sMultiLightSpecular"));
                                            EditorGUI.indentLevel--;
                                        }
                                        if(specularMode == 2)
                                        {
                                            applySpecular.floatValue = 1.0f;
                                            specularToon.floatValue = 1.0f;
                                            EditorGUI.indentLevel++;
                                            materialEditor.ShaderProperty(specularNormalStrength, GetLoc("sNormalStrength"));
                                            materialEditor.ShaderProperty(specularBorder, GetLoc("sBorder"));
                                            materialEditor.ShaderProperty(specularBlur, GetLoc("sBlur"));
                                            materialEditor.ShaderProperty(applySpecularFA, GetLoc("sMultiLightSpecular"));
                                            EditorGUI.indentLevel--;
                                        }
                                        materialEditor.ShaderProperty(applyReflection, GetLoc("sApplyReflection"));
                                        if(applyReflection.floatValue == 1.0f)
                                        {
                                            EditorGUI.indentLevel++;
                                            materialEditor.ShaderProperty(reflectionNormalStrength, GetLoc("sNormalStrength"));
                                            EditorGUI.indentLevel--;
                                        }
                                        if(isTransparent) materialEditor.ShaderProperty(reflectionApplyTransparency, GetLoc("sApplyTransparency"));
                                        EditorGUILayout.EndVertical();
                                    }
                                    EditorGUILayout.EndVertical();
                                }

                                //------------------------------------------------------------------------------------------------------------------------------
                                // MatCap
                                if(CheckFeature(shaderSetting.LIL_FEATURE_MATCAP))
                                {
                                    EditorGUILayout.BeginVertical(boxOuter);
                                    materialEditor.ShaderProperty(useMatCap, GetLoc("sMatCap"));
                                    DrawMenuButton(GetLoc("sAnchorMatCap"), lilPropertyBlock.MatCap1st);
                                    if(useMatCap.floatValue == 1)
                                    {
                                        EditorGUILayout.BeginVertical(boxInnerHalf);
                                        MatCapTextureGUI(materialEditor, ref edSet.isShowMatCapUV, new GUIContent(GetLoc("sMatCap"), GetLoc("sTextureRGBA")), matcapTex, matcapColor, matcapBlendUV1, matcapZRotCancel, matcapPerspective, matcapVRParallaxStrength);
                                        if(matcapColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                        {
                                            matcapColor.colorValue = new Color(matcapColor.colorValue.r, matcapColor.colorValue.g, matcapColor.colorValue.b, 1.0f);
                                        }
                                        materialEditor.ShaderProperty(matcapNormalStrength, GetLoc("sNormalStrength"));
                                        DrawLine();
                                        if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK)) TextureGUI(materialEditor, ref edSet.isShowMatCapBlendMask, maskBlendContent, matcapBlendMask, matcapBlend);
                                        else                                                        materialEditor.ShaderProperty(matcapBlend, GetLoc("sBlend"));
                                        materialEditor.ShaderProperty(matcapEnableLighting, GetLoc("sEnableLighting"));
                                        materialEditor.ShaderProperty(matcapShadowMask, GetLoc("sShadowMask"));
                                        materialEditor.ShaderProperty(matcapBackfaceMask, GetLoc("sBackfaceMask"));
                                        materialEditor.ShaderProperty(matcapLod, GetLoc("sBlur"));
                                        materialEditor.ShaderProperty(matcapBlendMode, sBlendModes);
                                        if(matcapEnableLighting.floatValue != 0.0f && matcapBlendMode.floatValue == 3.0f && AutoFixHelpBox(GetLoc("sHelpMatCapBlending")))
                                        {
                                            matcapEnableLighting.floatValue = 0.0f;
                                        }
                                        if(isTransparent) materialEditor.ShaderProperty(matcapApplyTransparency, GetLoc("sApplyTransparency"));
                                        if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP))
                                        {
                                            DrawLine();
                                            materialEditor.ShaderProperty(matcapCustomNormal, GetLoc("sMatCapCustomNormal"));
                                            if(matcapCustomNormal.floatValue == 1)
                                            {
                                                TextureGUI(materialEditor, ref edSet.isShowMatCapBumpMap, normalMapContent, matcapBumpMap, matcapBumpScale);
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
                                    materialEditor.ShaderProperty(useMatCap2nd, GetLoc("sMatCap2nd"));
                                    DrawMenuButton(GetLoc("sAnchorMatCap"), lilPropertyBlock.MatCap2nd);
                                    if(useMatCap2nd.floatValue == 1)
                                    {
                                        EditorGUILayout.BeginVertical(boxInnerHalf);
                                        MatCapTextureGUI(materialEditor, ref edSet.isShowMatCap2ndUV, new GUIContent(GetLoc("sMatCap"), GetLoc("sTextureRGBA")), matcap2ndTex, matcap2ndColor, matcap2ndBlendUV1, matcap2ndZRotCancel, matcap2ndPerspective, matcap2ndVRParallaxStrength);
                                        if(matcap2ndColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                        {
                                            matcap2ndColor.colorValue = new Color(matcap2ndColor.colorValue.r, matcap2ndColor.colorValue.g, matcap2ndColor.colorValue.b, 1.0f);
                                        }
                                        materialEditor.ShaderProperty(matcap2ndNormalStrength, GetLoc("sNormalStrength"));
                                        DrawLine();
                                        if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK)) TextureGUI(materialEditor, ref edSet.isShowMatCap2ndBlendMask, maskBlendContent, matcap2ndBlendMask, matcap2ndBlend);
                                        else                                                        materialEditor.ShaderProperty(matcap2ndBlend, GetLoc("sBlend"));
                                        materialEditor.ShaderProperty(matcap2ndEnableLighting, GetLoc("sEnableLighting"));
                                        materialEditor.ShaderProperty(matcap2ndShadowMask, GetLoc("sShadowMask"));
                                        materialEditor.ShaderProperty(matcap2ndBackfaceMask, GetLoc("sBackfaceMask"));
                                        materialEditor.ShaderProperty(matcap2ndLod, GetLoc("sBlur"));
                                        materialEditor.ShaderProperty(matcap2ndBlendMode, sBlendModes);
                                        if(matcap2ndEnableLighting.floatValue != 0.0f && matcap2ndBlendMode.floatValue == 3.0f && AutoFixHelpBox(GetLoc("sHelpMatCapBlending")))
                                        {
                                            matcap2ndEnableLighting.floatValue = 0.0f;
                                        }
                                        if(isTransparent) materialEditor.ShaderProperty(matcap2ndApplyTransparency, GetLoc("sApplyTransparency"));
                                        if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_MATCAP_NORMALMAP))
                                        {
                                            DrawLine();
                                            materialEditor.ShaderProperty(matcap2ndCustomNormal, GetLoc("sMatCapCustomNormal"));
                                            if(matcap2ndCustomNormal.floatValue == 1)
                                            {
                                                TextureGUI(materialEditor, ref edSet.isShowMatCap2ndBumpMap, normalMapContent, matcap2ndBumpMap, matcap2ndBumpScale);
                                            }
                                        }
                                        EditorGUILayout.EndVertical();
                                    }
                                    EditorGUILayout.EndVertical();
                                }

                                //------------------------------------------------------------------------------------------------------------------------------
                                // Rim
                                if(CheckFeature(shaderSetting.LIL_FEATURE_RIMLIGHT))
                                {
                                    EditorGUILayout.BeginVertical(boxOuter);
                                    materialEditor.ShaderProperty(useRim, GetLoc("sRimLight"));
                                    DrawMenuButton(GetLoc("sAnchorRimLight"), lilPropertyBlock.RimLight);
                                    if(useRim.floatValue == 1)
                                    {
                                        EditorGUILayout.BeginVertical(boxInnerHalf);
                                        if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR))  TextureGUI(materialEditor, ref edSet.isShowRimColorTex, colorRGBAContent, rimColorTex, rimColor);
                                        else                                                            materialEditor.ShaderProperty(rimColor, GetLoc("sColor"));
                                        if(rimColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                        {
                                            rimColor.colorValue = new Color(rimColor.colorValue.r, rimColor.colorValue.g, rimColor.colorValue.b, 1.0f);
                                        }
                                        materialEditor.ShaderProperty(rimEnableLighting, GetLoc("sEnableLighting"));
                                        materialEditor.ShaderProperty(rimShadowMask, GetLoc("sShadowMask"));
                                        materialEditor.ShaderProperty(rimBackfaceMask, GetLoc("sBackfaceMask"));
                                        if(isTransparent) materialEditor.ShaderProperty(rimApplyTransparency, GetLoc("sApplyTransparency"));
                                        DrawLine();
                                        if(CheckFeature(shaderSetting.LIL_FEATURE_RIMLIGHT_DIRECTION))
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
                                            }
                                            else
                                            {
                                                rimBorder.floatValue = 1.0f - EditorGUILayout.Slider(GetLoc("sBorder"), 1.0f - rimBorder.floatValue, 0.0f, 1.0f);
                                                materialEditor.ShaderProperty(rimBlur, GetLoc("sBlur"));
                                            }
                                        }
                                        else
                                        {
                                            rimBorder.floatValue = 1.0f - EditorGUILayout.Slider(GetLoc("sBorder"), 1.0f - rimBorder.floatValue, 0.0f, 1.0f);
                                            materialEditor.ShaderProperty(rimBlur, GetLoc("sBlur"));
                                        }
                                        materialEditor.ShaderProperty(rimNormalStrength, GetLoc("sNormalStrength"));
                                        materialEditor.ShaderProperty(rimFresnelPower, GetLoc("sFresnelPower"));
                                        materialEditor.ShaderProperty(rimVRParallaxStrength, GetLoc("sVRParallaxStrength"));
                                        EditorGUILayout.EndVertical();
                                    }
                                    EditorGUILayout.EndVertical();
                                }

                                //------------------------------------------------------------------------------------------------------------------------------
                                // Glitter
                                if(CheckFeature(shaderSetting.LIL_FEATURE_GLITTER))
                                {
                                    EditorGUILayout.BeginVertical(boxOuter);
                                    materialEditor.ShaderProperty(useGlitter, GetLoc("sGlitter"));
                                    DrawMenuButton(GetLoc("sAnchorGlitter"), lilPropertyBlock.Glitter);
                                    if(useGlitter.floatValue == 1)
                                    {
                                        EditorGUILayout.BeginVertical(boxInnerHalf);
                                        materialEditor.ShaderProperty(glitterUVMode, "UV Mode|UV0|UV1");
                                        TextureGUI(materialEditor, ref edSet.isShowGlitterColorTex, colorRGBAContent, glitterColorTex, glitterColor);
                                        if(glitterColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                        {
                                            glitterColor.colorValue = new Color(glitterColor.colorValue.r, glitterColor.colorValue.g, glitterColor.colorValue.b, 1.0f);
                                        }
                                        materialEditor.ShaderProperty(glitterMainStrength, GetLoc("sMainColorPower"));
                                        materialEditor.ShaderProperty(glitterEnableLighting, GetLoc("sEnableLighting"));
                                        materialEditor.ShaderProperty(glitterShadowMask, GetLoc("sShadowMask"));
                                        materialEditor.ShaderProperty(glitterBackfaceMask, GetLoc("sBackfaceMask"));
                                        if(isTransparent) materialEditor.ShaderProperty(glitterApplyTransparency, GetLoc("sApplyTransparency"));
                                        DrawLine();
                                        materialEditor.ShaderProperty(glitterParams1, sGlitterParams1);
                                        materialEditor.ShaderProperty(glitterParams2, sGlitterParams2);
                                        materialEditor.ShaderProperty(glitterVRParallaxStrength, GetLoc("sVRParallaxStrength"));
                                        materialEditor.ShaderProperty(glitterNormalStrength, GetLoc("sNormalStrength"));
                                        EditorGUILayout.EndVertical();
                                    }
                                    EditorGUILayout.EndVertical();
                                }

                                //------------------------------------------------------------------------------------------------------------------------------
                                // Backlight
                                if(!isGem && CheckFeature(shaderSetting.LIL_FEATURE_BACKLIGHT))
                                {
                                    EditorGUILayout.BeginVertical(boxOuter);
                                    materialEditor.ShaderProperty(useBacklight, GetLoc("sBacklight"));
                                    DrawMenuButton(GetLoc("sAnchorBacklight"), lilPropertyBlock.Backlight);
                                    if(useBacklight.floatValue == 1)
                                    {
                                        EditorGUILayout.BeginVertical(boxInnerHalf);
                                        TextureGUI(materialEditor, ref edSet.isShowBacklightColorTex, colorRGBAContent, backlightColorTex, backlightColor);
                                        if(backlightColor.colorValue.a == 0 && AutoFixHelpBox(GetLoc("sColorAlphaZeroWarn")))
                                        {
                                            backlightColor.colorValue = new Color(backlightColor.colorValue.r, backlightColor.colorValue.g, backlightColor.colorValue.b, 1.0f);
                                        }
                                        materialEditor.ShaderProperty(backlightNormalStrength, GetLoc("sNormalStrength"));
                                        backlightBorder.floatValue = 1.0f - EditorGUILayout.Slider(GetLoc("sBorder"), 1.0f - backlightBorder.floatValue, 0.0f, 1.0f);
                                        materialEditor.ShaderProperty(backlightBlur, GetLoc("sBlur"));
                                        materialEditor.ShaderProperty(backlightDirectivity, GetLoc("sDirectivity"));
                                        materialEditor.ShaderProperty(backlightViewStrength, GetLoc("sViewDirectionStrength"));
                                        materialEditor.ShaderProperty(backlightReceiveShadow, GetLoc("sReceiveShadow"));
                                        materialEditor.ShaderProperty(backlightBackfaceMask, GetLoc("sBackfaceMask"));
                                        EditorGUILayout.EndVertical();
                                    }
                                    EditorGUILayout.EndVertical();
                                }

                                //------------------------------------------------------------------------------------------------------------------------------
                                // Gem
                                if(isGem)
                                {
                                    EditorGUILayout.BeginVertical(boxOuter);
                                    EditorGUILayout.LabelField(GetLoc("sGem"), customToggleFont);
                                    DrawMenuButton(GetLoc("sAnchorGem"), lilPropertyBlock.Gem);
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
                            }
                        }
                    }

                    EditorGUILayout.Space();

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Advanced
                    GUILayout.Label(" " + GetLoc("sAdvanced"), EditorStyles.boldLabel);

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Outline
                    if((!isRefr && !isFur && !isGem && !isCustomShader) || (isCustomShader && isOutl))
                    {
                        edSet.isShowOutline = Foldout(GetLoc("sOutlineSetting"), GetLoc("sOutlineTips"), edSet.isShowOutline);
                        DrawMenuButton(GetLoc("sAnchorOutline"), lilPropertyBlock.Outline);
                        if(edSet.isShowOutline)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            if(isMulti) materialEditor.ShaderProperty(useOutline, GetLoc("sOutline"));
                            if(!isCustomShader && !isMulti && isOutl != EditorGUILayout.ToggleLeft(GetLoc("sOutline"), isOutl, customToggleFont))
                            {
                                SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, !isOutl, isLite, isStWr, isTess);
                            }
                            else if(isCustomShader)
                            {
                                EditorGUILayout.LabelField(GetLoc("sOutline"), customToggleFont);
                            }
                            if(isOutl || isMulti && useOutline.floatValue != 0.0f)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR))   TextureGUI(materialEditor, ref edSet.isShowOutlineMap, colorRGBAContent, outlineTex, outlineColor, outlineTex_ScrollRotate, true, CheckFeature(shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV));
                                else                                                            materialEditor.ShaderProperty(outlineColor, GetLoc("sColor"));
                                if(CheckFeature(shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION))
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
                                if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH))   materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sWidth"), GetLoc("sWidthR")), outlineWidthMask, outlineWidth);
                                else                                                            materialEditor.ShaderProperty(outlineWidth, GetLoc("sWidth"));
                                materialEditor.ShaderProperty(outlineFixWidth, GetLoc("sFixWidth"));
                                materialEditor.ShaderProperty(outlineVertexR2Width, GetLoc("sVertexR2Width"));
                                if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_OUTLINE_NORMAL))
                                {
                                    DrawLine();
                                    materialEditor.TexturePropertySingleLine(normalMapContent, outlineVectorTex, outlineVectorScale);
                                }
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Parallax
                    if(CheckFeature(shaderSetting.LIL_FEATURE_PARALLAX) && !isFur && !isGem)
                    {
                        edSet.isShowParallax = Foldout(GetLoc("sParallax"), GetLoc("sParallaxTips"), edSet.isShowParallax);
                        DrawMenuButton(GetLoc("sAnchorParallax"), lilPropertyBlock.Parallax);
                        if(edSet.isShowParallax)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            materialEditor.ShaderProperty(useParallax, GetLoc("sParallax"));
                            DrawMenuButton(GetLoc("sAnchorParallax"), lilPropertyBlock.Parallax);
                            if(useParallax.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sParallax"), GetLoc("sParallaxR")), parallaxMap, parallax);
                                materialEditor.ShaderProperty(parallaxOffset, GetLoc("sParallaxOffset"));
                                if(isMulti) materialEditor.ShaderProperty(usePOM, "POM");
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Distance Fade
                    if(CheckFeature(shaderSetting.LIL_FEATURE_DISTANCE_FADE) && !isGem)
                    {
                        edSet.isShowDistanceFade = Foldout(GetLoc("sDistanceFade"), GetLoc("sDistanceFadeTips"), edSet.isShowDistanceFade);
                        DrawMenuButton(GetLoc("sAnchorDistanceFade"), lilPropertyBlock.DistanceFade);
                        if(edSet.isShowDistanceFade)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField(GetLoc("sDistanceFade"), customToggleFont);
                            DrawMenuButton(GetLoc("sAnchorDistanceFade"), lilPropertyBlock.DistanceFade);
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            materialEditor.ShaderProperty(distanceFadeColor, GetLoc("sColor"));
                            EditorGUI.indentLevel++;
                            materialEditor.ShaderProperty(distanceFade, sDistanceFadeSetting);
                            EditorGUI.indentLevel--;
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // AudioLink
                    if(CheckFeature(shaderSetting.LIL_FEATURE_AUDIOLINK) && !isFur)
                    {
                        edSet.isShowAudioLink = Foldout(GetLoc("sAudioLink"), GetLoc("sAudioLinkTips"), edSet.isShowAudioLink);
                        DrawMenuButton(GetLoc("sAnchorAudioLink"), lilPropertyBlock.AudioLink);
                        if(edSet.isShowAudioLink)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            materialEditor.ShaderProperty(useAudioLink, GetLoc("sAudioLink"));
                            DrawMenuButton(GetLoc("sAnchorAudioLink"), lilPropertyBlock.AudioLink);
                            if(useAudioLink.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                materialEditor.ShaderProperty(audioLinkUVMode, GetLoc("sAudioLinkUVMode") + "|" + GetLoc("sAudioLinkUVModeNone") + "|" + GetLoc("sAudioLinkUVModeRim") + "|" + GetLoc("sAudioLinkUVModeUV") + "|" + GetLoc("sAudioLinkUVModeMask") + "|" + GetLoc("sAudioLinkUVModeMask") + " (Spectrum)" + "|" + GetLoc("sAudioLinkUVModePosition"));
                                if(audioLinkUVMode.floatValue == 0) materialEditor.ShaderProperty(audioLinkUVParams, GetLoc("sOffset") + "|" + GetLoc("sAudioLinkBand") + "|" + GetLoc("sAudioLinkBandBass") + "|" + GetLoc("sAudioLinkBandLowMid") + "|" + GetLoc("sAudioLinkBandHighMid") + "|" + GetLoc("sAudioLinkBandTreble"));
                                if(audioLinkUVMode.floatValue == 1) materialEditor.ShaderProperty(audioLinkUVParams, GetLoc("sScale") + "|" + GetLoc("sOffset") + "|" + GetLoc("sAudioLinkBand") + "|" + GetLoc("sAudioLinkBandBass") + "|" + GetLoc("sAudioLinkBandLowMid") + "|" + GetLoc("sAudioLinkBandHighMid") + "|" + GetLoc("sAudioLinkBandTreble"));
                                if(audioLinkUVMode.floatValue == 2) materialEditor.ShaderProperty(audioLinkUVParams, GetLoc("sScale") + "|" + GetLoc("sOffset") + "|" + GetLoc("sAngle") + "|" + GetLoc("sAudioLinkBand") + "|" + GetLoc("sAudioLinkBandBass") + "|" + GetLoc("sAudioLinkBandLowMid") + "|" + GetLoc("sAudioLinkBandHighMid") + "|" + GetLoc("sAudioLinkBandTreble"));
                                if(audioLinkUVMode.floatValue == 3) materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sMask"), ""), audioLinkMask);
                                if(audioLinkUVMode.floatValue == 4)
                                {
                                    materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sMask"), ""), audioLinkMask);
                                    DrawVectorAs4Float(audioLinkUVParams, "Volume", "Base Boost", "Treble Boost", "Line Width");
                                }
                                if(audioLinkUVMode.floatValue == 5)
                                {
                                    materialEditor.ShaderProperty(audioLinkUVParams, GetLoc("sScale") + "|" + GetLoc("sOffset") + "|" + GetLoc("sAudioLinkBand") + "|" + GetLoc("sAudioLinkBandBass") + "|" + GetLoc("sAudioLinkBandLowMid") + "|" + GetLoc("sAudioLinkBandHighMid") + "|" + GetLoc("sAudioLinkBandTreble"));
                                    materialEditor.ShaderProperty(audioLinkStart, GetLoc("sAudioLinkStartPosition"));
                                }
                                DrawLine();
                                GUILayout.Label(GetLoc("sAudioLinkDefaultValue"), EditorStyles.boldLabel);
                                EditorGUI.indentLevel++;
                                if(audioLinkUVMode.floatValue == 4) DrawVectorAs4Float(audioLinkDefaultValue, GetLoc("sStrength"), "Detail", "Speed", GetLoc("sThreshold"));
                                else materialEditor.ShaderProperty(audioLinkDefaultValue, GetLoc("sStrength") + "|" + GetLoc("sBlinkStrength") + "|" + GetLoc("sBlinkSpeed") + "|" + GetLoc("sThreshold"));
                                EditorGUI.indentLevel--;
                                DrawLine();
                                GUILayout.Label(GetLoc("sApplyTo"), EditorStyles.boldLabel);
                                EditorGUI.indentLevel++;
                                if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN2ND))         materialEditor.ShaderProperty(audioLink2Main2nd, GetLoc("sMainColor2nd"));
                                if(CheckFeature(shaderSetting.LIL_FEATURE_MAIN3RD))         materialEditor.ShaderProperty(audioLink2Main3rd, GetLoc("sMainColor3rd"));
                                if(CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_1ST))
                                {
                                    materialEditor.ShaderProperty(audioLink2Emission, GetLoc("sEmission"));
                                     materialEditor.ShaderProperty(audioLink2EmissionGrad, GetLoc("sEmission") + GetLoc("sGradation"));
                                }
                                if(CheckFeature(shaderSetting.LIL_FEATURE_EMISSION_2ND))
                                {
                                    materialEditor.ShaderProperty(audioLink2Emission2nd, GetLoc("sEmission2nd"));
                                    materialEditor.ShaderProperty(audioLink2Emission2ndGrad, GetLoc("sEmission2nd") + GetLoc("sGradation"));
                                }
                                if(CheckFeature(shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX))
                                {
                                    materialEditor.ShaderProperty(audioLink2Vertex, GetLoc("sVertex"));
                                    if(audioLink2Vertex.floatValue == 1)
                                    {
                                        EditorGUI.indentLevel++;
                                        materialEditor.ShaderProperty(audioLinkVertexUVMode, GetLoc("sAudioLinkUVMode") + "|" + GetLoc("sAudioLinkUVModeNone") + "|" + GetLoc("sAudioLinkUVModePosition") + "|" + GetLoc("sAudioLinkUVModeUV") + "|" + GetLoc("sAudioLinkUVModeMask"));
                                        if(audioLinkVertexUVMode.floatValue == 0) materialEditor.ShaderProperty(audioLinkVertexUVParams, GetLoc("sOffset") + "|" + GetLoc("sAudioLinkBand") + "|" + GetLoc("sAudioLinkBandBass") + "|" + GetLoc("sAudioLinkBandLowMid") + "|" + GetLoc("sAudioLinkBandHighMid") + "|" + GetLoc("sAudioLinkBandTreble"));
                                        if(audioLinkVertexUVMode.floatValue == 1) materialEditor.ShaderProperty(audioLinkVertexUVParams, GetLoc("sScale") + "|" + GetLoc("sOffset") + "|" + GetLoc("sAudioLinkBand") + "|" + GetLoc("sAudioLinkBandBass") + "|" + GetLoc("sAudioLinkBandLowMid") + "|" + GetLoc("sAudioLinkBandHighMid") + "|" + GetLoc("sAudioLinkBandTreble"));
                                        if(audioLinkVertexUVMode.floatValue == 2) materialEditor.ShaderProperty(audioLinkVertexUVParams, GetLoc("sScale") + "|" + GetLoc("sOffset") + "|" + GetLoc("sAngle") + "|" + GetLoc("sAudioLinkBand") + "|" + GetLoc("sAudioLinkBandBass") + "|" + GetLoc("sAudioLinkBandLowMid") + "|" + GetLoc("sAudioLinkBandHighMid") + "|" + GetLoc("sAudioLinkBandTreble"));
                                        if(audioLinkVertexUVMode.floatValue == 3) materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sMask"), ""), audioLinkMask);
                                        if(audioLinkVertexUVMode.floatValue == 1) materialEditor.ShaderProperty(audioLinkVertexStart, GetLoc("sAudioLinkStartPosition"));
                                        DrawLine();
                                        materialEditor.ShaderProperty(audioLinkVertexStrength, GetLoc("sAudioLinkMovingVector") + "|" + GetLoc("sAudioLinkNormalStrength"));
                                        EditorGUI.indentLevel--;
                                    }
                                }
                                EditorGUI.indentLevel--;
                                if(CheckFeature(shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL))
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
                    if(CheckFeature(shaderSetting.LIL_FEATURE_DISSOLVE) && !isFur)
                    {
                        edSet.isShowDissolve = Foldout(GetLoc("sDissolve"), GetLoc("sDissolve"), edSet.isShowDissolve);
                        DrawMenuButton(GetLoc("sAnchorDissolve"), lilPropertyBlock.Dissolve);
                        if(edSet.isShowDissolve && (renderingModeBuf == RenderingMode.Opaque && !isMulti || isMulti && transparentModeMat.floatValue == 0.0f))
                        {
                            GUILayout.Label(GetLoc("sDissolveWarnOpaque"), wrapLabel);
                        }
                        if(edSet.isShowDissolve && (renderingModeBuf != RenderingMode.Opaque || isMulti && transparentModeMat.floatValue != 0.0f))
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            materialEditor.ShaderProperty(dissolveParams, sDissolveParamsMode);
                            DrawMenuButton(GetLoc("sAnchorDissolve"), lilPropertyBlock.Dissolve);
                            if(dissolveParams.vectorValue.x != 0)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                materialEditor.ShaderProperty(dissolveParams, sDissolveParamsOther);
                                if(dissolveParams.vectorValue.x == 1.0f)                                         TextureGUI(materialEditor, ref edSet.isShowDissolveMask, maskBlendContent, dissolveMask);
                                if(dissolveParams.vectorValue.x == 2.0f && dissolveParams.vectorValue.y == 0.0f) materialEditor.ShaderProperty(dissolvePos, GetLoc("sPosition") + "|2");
                                if(dissolveParams.vectorValue.x == 2.0f && dissolveParams.vectorValue.y == 1.0f) materialEditor.ShaderProperty(dissolvePos, GetLoc("sVector") + "|2");
                                if(dissolveParams.vectorValue.x == 3.0f && dissolveParams.vectorValue.y == 0.0f) materialEditor.ShaderProperty(dissolvePos, GetLoc("sPosition") + "|3");
                                if(dissolveParams.vectorValue.x == 3.0f && dissolveParams.vectorValue.y == 1.0f) materialEditor.ShaderProperty(dissolvePos, GetLoc("sVector") + "|3");
                                if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE))
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
                        DrawMenuButton(GetLoc("sAnchorEncryption"), lilPropertyBlock.Encryption);
                        if(edSet.isShowEncryption)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField(GetLoc("sEncryption"), customToggleFont);
                            DrawMenuButton(GetLoc("sAnchorEncryption"), lilPropertyBlock.Encryption);
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
                        DrawMenuButton(GetLoc("sAnchorRefraction"), lilPropertyBlock.Refraction);
                        if(edSet.isShowRefraction)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField(GetLoc("sRefraction"), customToggleFont);
                            DrawMenuButton(GetLoc("sAnchorRefraction"), lilPropertyBlock.Refraction);
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
                        DrawMenuButton(GetLoc("sAnchorFur"), lilPropertyBlock.Fur);
                        if(edSet.isShowFur)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField(GetLoc("sFur"), customToggleFont);
                            DrawMenuButton(GetLoc("sAnchorFur"), lilPropertyBlock.Fur);
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL))  materialEditor.TexturePropertySingleLine(normalMapContent, furVectorTex,furVectorScale);
                            if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_FUR_LENGTH))  materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sLength"), GetLoc("sStrengthR")), furLengthMask);
                            materialEditor.ShaderProperty(furVector, GetLoc("sVector") + "|" + GetLoc("sLength"));
                            materialEditor.ShaderProperty(vertexColor2FurVector, GetLoc("sVertexColor2Vector"));
                            materialEditor.ShaderProperty(furGravity, GetLoc("sGravity"));
                            DrawLine();
                            materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sNoise"), GetLoc("sNoiseR")), furNoiseMask);
                            materialEditor.TextureScaleOffsetProperty(furNoiseMask);
                            if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_FUR_MASK))    materialEditor.TexturePropertySingleLine(new GUIContent(GetLoc("sMask"), GetLoc("sAlphaR")), furMask);
                            materialEditor.ShaderProperty(furAO, GetLoc("sAO"));
                            materialEditor.ShaderProperty(furLayerNum, GetLoc("sLayerNum"));
                            furRootOffset.floatValue = -EditorGUILayout.Slider(GetLoc("sRootWidth"), -furRootOffset.floatValue, 0.0f, 1.0f);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Stencil
                    edSet.isShowStencil = Foldout(GetLoc("sStencilSetting"), GetLoc("sStencilTips"), edSet.isShowStencil);
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
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, isOutl, isLite, isStWr, isTess);
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
                        if(isOutl || isMulti && useOutline.floatValue != 0.0f)
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
                    DrawMenuButton(GetLoc("sAnchorRendering"), lilPropertyBlock.Rendering);
                    if(edSet.isShowRendering)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Reset Button
                        if(GUILayout.Button(GetLoc("sRenderingReset"), offsetButton))
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
                            shaderType = EditorGUILayout.Popup(GetLoc("sShaderType"),shaderType,new String[]{GetLoc("sShaderTypeNormal"),GetLoc("sShaderTypeLite")});
                            if(shaderTypeBuf != shaderType)
                            {
                                if(shaderType==0) isLite = false;
                                if(shaderType==1) isLite = true;
                                SetupMaterialWithRenderingMode(material, renderingModeBuf, transparentModeBuf, isOutl, isLite, isStWr, isTess);
                            }

                            //------------------------------------------------------------------------------------------------------------------------------
                            // Rendering
                            if(renderingModeBuf == RenderingMode.Transparent || renderingModeBuf == RenderingMode.Fur || isMulti && (transparentModeMat.floatValue == 2.0f || transparentModeMat.floatValue == 4.0f))
                            {
                                materialEditor.ShaderProperty(subpassCutoff, GetLoc("sSubpassCutoff"));
                            }
                            materialEditor.ShaderProperty(cull, sCullModes);
                            materialEditor.ShaderProperty(zclip, GetLoc("sZClip"));
                            materialEditor.ShaderProperty(zwrite, GetLoc("sZWrite"));
                            materialEditor.ShaderProperty(ztest, GetLoc("sZTest"));
                            materialEditor.ShaderProperty(offsetFactor, GetLoc("sOffsetFactor"));
                            materialEditor.ShaderProperty(offsetUnits, GetLoc("sOffsetUnits"));
                            materialEditor.ShaderProperty(colorMask, GetLoc("sColorMask"));
                            materialEditor.ShaderProperty(alphaToMask, GetLoc("sAlphaToMask"));
                            DrawLine();
                            BlendSettingGUI(materialEditor, ref edSet.isShowBlend, GetLoc("sForward"), srcBlend, dstBlend, srcBlendAlpha, dstBlendAlpha, blendOp, blendOpAlpha);
                            if(!(isFur & !isCutout) && !isGem)
                            {
                                DrawLine();
                                BlendSettingGUI(materialEditor, ref edSet.isShowBlendAdd, GetLoc("sForwardAdd"), srcBlendFA, dstBlendFA, srcBlendAlphaFA, dstBlendAlphaFA, blendOpFA, blendOpAlphaFA);
                            }
                            DrawLine();
                            materialEditor.EnableInstancingField();
                            materialEditor.DoubleSidedGIField();
                            materialEditor.RenderQueueField();
                            materialEditor.ShaderProperty(beforeExposureLimit, GetLoc("sBeforeExposureLimit"));
                            materialEditor.ShaderProperty(lilDirectionalLightStrength, GetLoc("sDirectionalLightStrength"));
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Outline
                        if(isOutl || isMulti && useOutline.floatValue != 0.0f && !isFur && !isRefr && !isGem)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField(GetLoc("sOutline"), customToggleFont);
                            EditorGUILayout.BeginVertical(boxInner);
                            materialEditor.ShaderProperty(outlineCull, sCullModes);
                            materialEditor.ShaderProperty(outlineZclip, GetLoc("sZClip"));
                            materialEditor.ShaderProperty(outlineZwrite, GetLoc("sZWrite"));
                            materialEditor.ShaderProperty(outlineZtest, GetLoc("sZTest"));
                            materialEditor.ShaderProperty(outlineOffsetFactor, GetLoc("sOffsetFactor"));
                            materialEditor.ShaderProperty(outlineOffsetUnits, GetLoc("sOffsetUnits"));
                            materialEditor.ShaderProperty(outlineColorMask, GetLoc("sColorMask"));
                            materialEditor.ShaderProperty(outlineAlphaToMask, GetLoc("sAlphaToMask"));
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
                            materialEditor.ShaderProperty(furZclip, GetLoc("sZClip"));
                            materialEditor.ShaderProperty(furZwrite, GetLoc("sZWrite"));
                            materialEditor.ShaderProperty(furZtest, GetLoc("sZTest"));
                            materialEditor.ShaderProperty(furOffsetFactor, GetLoc("sOffsetFactor"));
                            materialEditor.ShaderProperty(furOffsetUnits, GetLoc("sOffsetUnits"));
                            materialEditor.ShaderProperty(furColorMask, GetLoc("sColorMask"));
                            materialEditor.ShaderProperty(furAlphaToMask, GetLoc("sAlphaToMask"));
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
                    if(CheckFeature(shaderSetting.LIL_FEATURE_TEX_TESSELLATION) && !isRefr && !isFur && !isGem && !isMulti)
                    {
                        edSet.isShowTess = Foldout(GetLoc("sTessellation"), GetLoc("sTessellationTips"), edSet.isShowTess);
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
                        // Optimization
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
                        if(!isMulti && !isFur && !isRefr && !isGem && GUILayout.Button(GetLoc("sConvertMulti"))) CreateMultiMaterial(material);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();

                        // Bake Textures
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sBake"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        string materialName = material.name;
                        if(!isGem && CheckFeature(shaderSetting.LIL_FEATURE_SHADOW)                         && GUILayout.Button(GetLoc("sShadow1stColor")))         AutoBakeColoredMask(material, shadowColorTex,       shadowColor,        "Shadow1stColor");
                        if(!isGem && CheckFeature(shaderSetting.LIL_FEATURE_SHADOW)                         && GUILayout.Button(GetLoc("sShadow2ndColor")))         AutoBakeColoredMask(material, shadow2ndColorTex,    shadow2ndColor,     "Shadow2ndColor");
                        if(!isFur && !isGem && CheckFeature(shaderSetting.LIL_FEATURE_REFLECTION)           && GUILayout.Button(GetLoc("sReflection")))             AutoBakeColoredMask(material, reflectionColorTex,   reflectionColor,    "ReflectionColor");
                        if(!isFur && CheckFeature(shaderSetting.LIL_FEATURE_MATCAP)                         && GUILayout.Button(GetLoc("sMatCap")))                 AutoBakeColoredMask(material, matcapBlendMask,      matcapColor,        "MatCapColor");
                        if(!isFur && CheckFeature(shaderSetting.LIL_FEATURE_MATCAP_2ND)                     && GUILayout.Button(GetLoc("sMatCap2nd")))              AutoBakeColoredMask(material, matcap2ndBlendMask,   matcap2ndColor,     "MatCap2ndColor");
                        if(!isFur && CheckFeature(shaderSetting.LIL_FEATURE_RIMLIGHT)                       && GUILayout.Button(GetLoc("sRimLight")))               AutoBakeColoredMask(material, rimColorTex,          rimColor,           "RimColor");
                        if(((!isRefr && !isFur && !isGem && !isCustomShader) || (isCustomShader && isOutl)) && GUILayout.Button(GetLoc("sSettingTexOutlineColor"))) AutoBakeColoredMask(material, outlineColorMask,     outlineColor,       "OutlineColor");
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Shader Setting
                    if(!isMulti)
                    {
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
                }
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Preset
            if(edSet.editorMode == EditorMode.Preset)
            {
                if(isLite)  EditorGUILayout.LabelField(GetLoc("sPresetsNotAvailable"), wrapLabel);
                else        DrawPreset(material);
            }

            if(isMulti)
            {
                SetupMultiMaterial(material);
            }

            CopyMainColorProperties();
            SaveEditorSettingTemp();
	    }

        //------------------------------------------------------------------------------------------------------------------------------
        // Property loader
        private void ResetProperties()
        {
            transparentModeMat = null;
            invisible = null;
            asUnlit = null;
            cutoff = null;
            subpassCutoff = null;
            flipNormal = null;
            backfaceForceShadow = null;
            vertexLightStrength = null;
            lightMinLimit = null;
            lightMaxLimit = null;
            beforeExposureLimit = null;
            monochromeLighting = null;
            lilDirectionalLightStrength = null;
            baseColor = null;
            baseMap = null;
            baseColorMap = null;
            triMask = null;
            cull = null;
            srcBlend = null;
            dstBlend = null;
            srcBlendAlpha = null;
            dstBlendAlpha = null;
            blendOp = null;
            blendOpAlpha = null;
            srcBlendFA = null;
            dstBlendFA = null;
            srcBlendAlphaFA = null;
            dstBlendAlphaFA = null;
            blendOpFA = null;
            blendOpAlphaFA = null;
            zwrite = null;
            ztest = null;
            stencilRef = null;
            stencilReadMask = null;
            stencilWriteMask = null;
            stencilComp = null;
            stencilPass = null;
            stencilFail = null;
            stencilZFail = null;
            offsetFactor = null;
            offsetUnits = null;
            colorMask = null;
            alphaToMask = null;
            shiftBackfaceUV = null;
            mainColor = null;
            mainTex = null;
            mainTexHSVG = null;
            mainTex_ScrollRotate = null;
            mainGradationStrength = null;
            mainGradationTex = null;
            mainColorAdjustMask = null;
            useMain2ndTex = null;
            mainColor2nd = null;
            main2ndTex = null;
            main2ndTexAngle = null;
            main2ndTexDecalAnimation = null;
            main2ndTexDecalSubParam = null;
            main2ndTexIsDecal = null;
            main2ndTexIsLeftOnly = null;
            main2ndTexIsRightOnly = null;
            main2ndTexShouldCopy = null;
            main2ndTexShouldFlipMirror = null;
            main2ndTexShouldFlipCopy = null;
            main2ndTexIsMSDF = null;
            main2ndBlendMask = null;
            main2ndTexBlendMode = null;
            main2ndEnableLighting = null;
            main2ndDissolveMask = null;
            main2ndDissolveNoiseMask = null;
            main2ndDissolveNoiseMask_ScrollRotate = null;
            main2ndDissolveNoiseStrength = null;
            main2ndDissolveColor = null;
            main2ndDissolveParams = null;
            main2ndDissolvePos = null;
            main2ndDistanceFade = null;
            useMain3rdTex = null;
            mainColor3rd = null;
            main3rdTex = null;
            main3rdTexAngle = null;
            main3rdTexDecalAnimation = null;
            main3rdTexDecalSubParam = null;
            main3rdTexIsDecal = null;
            main3rdTexIsLeftOnly = null;
            main3rdTexIsRightOnly = null;
            main3rdTexShouldCopy = null;
            main3rdTexShouldFlipMirror = null;
            main3rdTexShouldFlipCopy = null;
            main3rdTexIsMSDF = null;
            main3rdBlendMask = null;
            main3rdTexBlendMode = null;
            main3rdEnableLighting = null;
            main3rdDissolveMask = null;
            main3rdDissolveNoiseMask = null;
            main3rdDissolveNoiseMask_ScrollRotate = null;
            main3rdDissolveNoiseStrength = null;
            main3rdDissolveColor = null;
            main3rdDissolveParams = null;
            main3rdDissolvePos = null;
            main3rdDistanceFade = null;
            alphaMaskMode = null;
            alphaMask = null;
            alphaMaskScale = null;
            alphaMaskValue = null;
            useShadow = null;
            shadowStrength = null;
            shadowStrengthMask = null;
            shadowAOShift = null;
            shadowColor = null;
            shadowColorTex = null;
            shadowNormalStrength = null;
            shadowBorder = null;
            shadowBorderMask = null;
            shadowBlur = null;
            shadowBlurMask = null;
            shadow2ndColor = null;
            shadow2ndColorTex = null;
            shadow2ndNormalStrength = null;
            shadow2ndBorder = null;
            shadow2ndBlur = null;
            shadowMainStrength = null;
            shadowEnvStrength = null;
            shadowBorderColor = null;
            shadowBorderRange = null;
            shadowReceive = null;
            useBacklight = null;
            backlightColor = null;
            backlightColorTex = null;
            backlightNormalStrength = null;
            backlightBorder = null;
            backlightBlur = null;
            backlightDirectivity = null;
            backlightViewStrength = null;
            backlightReceiveShadow = null;
            backlightBackfaceMask = null;
            useBumpMap = null;
            bumpMap = null;
            bumpScale = null;
            useBump2ndMap = null;
            bump2ndMap = null;
            bump2ndScale = null;
            bump2ndScaleMask = null;
            useAnisotropy = null;
            anisotropyTangentMap = null;
            anisotropyScale = null;
            anisotropyScaleMask = null;
            anisotropyTangentWidth = null;
            anisotropyBitangentWidth = null;
            anisotropyShift = null;
            anisotropyShiftNoiseScale = null;
            anisotropySpecularStrength = null;
            anisotropy2ndTangentWidth = null;
            anisotropy2ndBitangentWidth = null;
            anisotropy2ndShift = null;
            anisotropy2ndShiftNoiseScale = null;
            anisotropy2ndSpecularStrength = null;
            anisotropyShiftNoiseMask = null;
            anisotropyShift = null;
            anisotropy2Reflection = null;
            anisotropy2MatCap = null;
            anisotropy2MatCap2nd = null;
            useReflection = null;
            metallic = null;
            metallicGlossMap = null;
            smoothness = null;
            smoothnessTex = null;
            reflectance = null;
            reflectionColor = null;
            reflectionColorTex = null;
            applySpecular = null;
            applySpecularFA = null;
            specularNormalStrength = null;
            specularToon = null;
            specularBorder = null;
            specularBlur = null;
            applyReflection = null;
            reflectionNormalStrength = null;
            reflectionApplyTransparency = null;
            useMatCap = null;
            matcapTex = null;
            matcapColor = null;
            matcapBlend = null;
            matcapBlendMask = null;
            matcapEnableLighting = null;
            matcapShadowMask = null;
            matcapBackfaceMask = null;
            matcapLod = null;
            matcapBlendUV1 = null;
            matcapVRParallaxStrength = null;
            matcapBlendMode = null;
            matcapMul = null;
            matcapApplyTransparency = null;
            matcapZRotCancel = null;
            matcapNormalStrength = null;
            matcapCustomNormal = null;
            matcapBumpMap = null;
            matcapBumpScale = null;
            useMatCap2nd = null;
            matcap2ndTex = null;
            matcap2ndColor = null;
            matcap2ndBlend = null;
            matcap2ndBlendMask = null;
            matcap2ndEnableLighting = null;
            matcap2ndShadowMask = null;
            matcap2ndBackfaceMask = null;
            matcap2ndLod = null;
            matcap2ndBlendUV1 = null;
            matcap2ndVRParallaxStrength = null;
            matcap2ndBlendMode = null;
            matcap2ndMul = null;
            matcap2ndApplyTransparency = null;
            matcap2ndZRotCancel = null;
            matcap2ndNormalStrength = null;
            matcap2ndCustomNormal = null;
            matcap2ndBumpMap = null;
            matcap2ndBumpScale = null;
            useRim = null;
            rimColor = null;
            rimColorTex = null;
            rimNormalStrength = null;
            rimBorder = null;
            rimBlur = null;
            rimFresnelPower = null;
            rimEnableLighting = null;
            rimShadowMask = null;
            rimBackfaceMask = null;
            rimVRParallaxStrength = null;
            rimApplyTransparency = null;
            rimDirStrength = null;
            rimDirRange = null;
            rimIndirRange = null;
            rimIndirColor = null;
            rimIndirBorder = null;
            rimIndirBlur = null;
            useGlitter = null;
            glitterUVMode = null;
            glitterColor = null;
            glitterColorTex = null;
            glitterMainStrength = null;
            glitterParams1 = null;
            glitterParams2 = null;
            glitterEnableLighting = null;
            glitterShadowMask = null;
            glitterBackfaceMask = null;
            glitterApplyTransparency = null;
            glitterVRParallaxStrength = null;
            glitterNormalStrength = null;
            useEmission = null;
            emissionColor = null;
            emissionMap = null;
            emissionMap_ScrollRotate = null;
            emissionMap_UVMode = null;
            emissionBlend = null;
            emissionBlendMask = null;
            emissionBlendMask_ScrollRotate = null;
            emissionBlink = null;
            emissionUseGrad = null;
            emissionGradTex = null;
            emissionGradSpeed = null;
            emissionParallaxDepth = null;
            emissionFluorescence = null;
            useEmission2nd = null;
            emission2ndColor = null;
            emission2ndMap = null;
            emission2ndMap_ScrollRotate = null;
            emission2ndMap_UVMode = null;
            emission2ndBlend = null;
            emission2ndBlendMask = null;
            emission2ndBlendMask_ScrollRotate = null;
            emission2ndBlink = null;
            emission2ndUseGrad = null;
            emission2ndGradTex = null;
            emission2ndGradSpeed = null;
            emission2ndParallaxDepth = null;
            emission2ndFluorescence = null;
            useOutline = null;
            outlineColor = null;
            outlineTex = null;
            outlineTex_ScrollRotate = null;
            outlineTexHSVG = null;
            outlineWidth = null;
            outlineWidthMask = null;
            outlineFixWidth = null;
            outlineVertexR2Width = null;
            outlineEnableLighting = null;
            outlineCull = null;
            outlineSrcBlend = null;
            outlineDstBlend = null;
            outlineSrcBlendAlpha = null;
            outlineDstBlendAlpha = null;
            outlineBlendOp = null;
            outlineBlendOpAlpha = null;
            outlineSrcBlendFA = null;
            outlineDstBlendFA = null;
            outlineSrcBlendAlphaFA = null;
            outlineDstBlendAlphaFA = null;
            outlineBlendOpFA = null;
            outlineBlendOpAlphaFA = null;
            outlineZclip = null;
            outlineZwrite = null;
            outlineZtest = null;
            outlineStencilRef = null;
            outlineStencilReadMask = null;
            outlineStencilWriteMask = null;
            outlineStencilComp = null;
            outlineStencilPass = null;
            outlineStencilFail = null;
            outlineStencilZFail = null;
            outlineOffsetFactor = null;
            outlineOffsetUnits = null;
            outlineColorMask = null;
            outlineAlphaToMask = null;
            useParallax = null;
            parallaxMap = null;
            parallax = null;
            parallaxOffset = null;
            usePOM = null;
            distanceFadeColor = null;
            distanceFade = null;
            useAudioLink = null;
            audioLinkDefaultValue = null;
            audioLinkUVMode = null;
            audioLinkUVParams = null;
            audioLinkStart = null;
            audioLinkMask = null;
            audioLink2Main2nd = null;
            audioLink2Main3rd = null;
            audioLink2Emission = null;
            audioLink2EmissionGrad = null;
            audioLink2Emission2nd = null;
            audioLink2Emission2ndGrad = null;
            audioLink2Vertex = null;
            audioLinkVertexUVMode = null;
            audioLinkVertexUVParams = null;
            audioLinkVertexStart = null;
            audioLinkVertexStrength = null;
            audioLinkAsLocal = null;
            audioLinkLocalMap = null;
            audioLinkLocalMapParams = null;
            dissolveMask = null;
            dissolveNoiseMask = null;
            dissolveNoiseMask_ScrollRotate = null;
            dissolveNoiseStrength = null;
            dissolveColor = null;
            dissolveParams = null;
            dissolvePos = null;
            ignoreEncryption = null;
            keys = null;
            refractionStrength = null;
            refractionFresnelPower = null;
            refractionColorFromMain = null;
            refractionColor = null;
            furNoiseMask = null;
            furMask = null;
            furLengthMask = null;
            furVectorTex = null;
            furVectorScale = null;
            furVector = null;
            furGravity = null;
            furAO = null;
            vertexColor2FurVector = null;
            furLayerNum = null;
            furRootOffset = null;
            furCull = null;
            furSrcBlend = null;
            furDstBlend = null;
            furSrcBlendAlpha = null;
            furDstBlendAlpha = null;
            furBlendOp = null;
            furBlendOpAlpha = null;
            furSrcBlendFA = null;
            furDstBlendFA = null;
            furSrcBlendAlphaFA = null;
            furDstBlendAlphaFA = null;
            furBlendOpFA = null;
            furBlendOpAlphaFA = null;
            furZclip = null;
            furZwrite = null;
            furZtest = null;
            furStencilRef = null;
            furStencilReadMask = null;
            furStencilWriteMask = null;
            furStencilComp = null;
            furStencilPass = null;
            furStencilFail = null;
            furStencilZFail = null;
            furOffsetFactor = null;
            furOffsetUnits = null;
            furColorMask = null;
            furAlphaToMask = null;
            tessEdge = null;
            tessStrength = null;
            tessShrink = null;
            tessFactorMax = null;
            gemChromaticAberration = null;
            gemEnvContrast = null;
            gemEnvColor = null;
            gemParticleLoop = null;
            gemParticleColor = null;
            gemVRParallaxStrength = null;
            fakeShadowVector = null;
        }

        private void LoadProperties(MaterialProperty[] props)
        {
            invisible = FindProperty("_Invisible", props);
            asUnlit = FindProperty("_AsUnlit", props);
            cutoff = FindProperty("_Cutoff", props);
            subpassCutoff = FindProperty("_SubpassCutoff", props);
            flipNormal = FindProperty("_FlipNormal", props);
            shiftBackfaceUV = FindProperty("_ShiftBackfaceUV", props);
            backfaceForceShadow = FindProperty("_BackfaceForceShadow", props);
            vertexLightStrength = FindProperty("_VertexLightStrength", props);
            lightMinLimit = FindProperty("_LightMinLimit", props);
            lightMaxLimit = FindProperty("_LightMaxLimit", props);
            beforeExposureLimit = FindProperty("_BeforeExposureLimit", props);
            monochromeLighting = FindProperty("_MonochromeLighting", props);
            lilDirectionalLightStrength = FindProperty("_lilDirectionalLightStrength", props);
            lightDirectionOverride = FindProperty("_LightDirectionOverride", props);
            baseColor = FindProperty("_BaseColor", props);
            baseMap = FindProperty("_BaseMap", props);
            baseColorMap = FindProperty("_BaseColorMap", props);
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
                zclip = FindProperty("_ZClip", props);
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
                alphaToMask = FindProperty("_AlphaToMask", props);
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
                alphaMaskScale = FindProperty("_AlphaMaskScale", props);
                alphaMaskValue = FindProperty("_AlphaMaskValue", props);
            // Shadow
            useShadow = FindProperty("_UseShadow", props);
                shadowStrength = FindProperty("_ShadowStrength", props);
                shadowStrengthMask = FindProperty("_ShadowStrengthMask", props);
                shadowAOShift = FindProperty("_ShadowAOShift", props);
                shadowColor = FindProperty("_ShadowColor", props);
                shadowColorTex = FindProperty("_ShadowColorTex", props);
                shadowNormalStrength = FindProperty("_ShadowNormalStrength", props);
                shadowBorder = FindProperty("_ShadowBorder", props);
                shadowBorderMask = FindProperty("_ShadowBorderMask", props);
                shadowBlur = FindProperty("_ShadowBlur", props);
                shadowBlurMask = FindProperty("_ShadowBlurMask", props);
                shadow2ndColor = FindProperty("_Shadow2ndColor", props);
                shadow2ndColorTex = FindProperty("_Shadow2ndColorTex", props);
                shadow2ndNormalStrength = FindProperty("_Shadow2ndNormalStrength", props);
                shadow2ndBorder = FindProperty("_Shadow2ndBorder", props);
                shadow2ndBlur = FindProperty("_Shadow2ndBlur", props);
                shadowMainStrength = FindProperty("_ShadowMainStrength", props);
                shadowEnvStrength = FindProperty("_ShadowEnvStrength", props);
                shadowBorderColor = FindProperty("_ShadowBorderColor", props);
                shadowBorderRange = FindProperty("_ShadowBorderRange", props);
                shadowReceive = FindProperty("_ShadowReceive", props);
            useBacklight = FindProperty("_UseBacklight", props);
                backlightColor = FindProperty("_BacklightColor", props);
                backlightColorTex = FindProperty("_BacklightColorTex", props);
                backlightNormalStrength = FindProperty("_BacklightNormalStrength", props);
                backlightBorder = FindProperty("_BacklightBorder", props);
                backlightBlur = FindProperty("_BacklightBlur", props);
                backlightDirectivity = FindProperty("_BacklightDirectivity", props);
                backlightViewStrength = FindProperty("_BacklightViewStrength", props);
                backlightReceiveShadow = FindProperty("_BacklightReceiveShadow", props);
                backlightBackfaceMask = FindProperty("_BacklightBackfaceMask", props);
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
                outlineVectorTex = FindProperty("_OutlineVectorTex", props);
                outlineVectorScale = FindProperty("_OutlineVectorScale", props);
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
                outlineZclip = FindProperty("_OutlineZClip", props);
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
                outlineAlphaToMask = FindProperty("_OutlineAlphaToMask", props);
            }
            // Normal
            useBumpMap = FindProperty("_UseBumpMap", props);
                bumpMap = FindProperty("_BumpMap", props);
                bumpScale = FindProperty("_BumpScale", props);
            useBump2ndMap = FindProperty("_UseBump2ndMap", props);
                bump2ndMap = FindProperty("_Bump2ndMap", props);
                bump2ndScale = FindProperty("_Bump2ndScale", props);
                bump2ndScaleMask = FindProperty("_Bump2ndScaleMask", props);
            useAnisotropy = FindProperty("_UseAnisotropy", props);
                anisotropyTangentMap = FindProperty("_AnisotropyTangentMap", props);
                anisotropyScale = FindProperty("_AnisotropyScale", props);
                anisotropyScaleMask = FindProperty("_AnisotropyScaleMask", props);
                anisotropyTangentWidth = FindProperty("_AnisotropyTangentWidth", props);
                anisotropyBitangentWidth = FindProperty("_AnisotropyBitangentWidth", props);
                anisotropyShift = FindProperty("_AnisotropyShift", props);
                anisotropyShiftNoiseScale = FindProperty("_AnisotropyShiftNoiseScale", props);
                anisotropySpecularStrength = FindProperty("_AnisotropySpecularStrength", props);
                anisotropy2ndTangentWidth = FindProperty("_Anisotropy2ndTangentWidth", props);
                anisotropy2ndBitangentWidth = FindProperty("_Anisotropy2ndBitangentWidth", props);
                anisotropy2ndShift = FindProperty("_Anisotropy2ndShift", props);
                anisotropy2ndShiftNoiseScale = FindProperty("_Anisotropy2ndShiftNoiseScale", props);
                anisotropy2ndSpecularStrength = FindProperty("_Anisotropy2ndSpecularStrength", props);
                anisotropyShiftNoiseMask = FindProperty("_AnisotropyShiftNoiseMask", props);
                anisotropy2Reflection = FindProperty("_Anisotropy2Reflection", props);
                anisotropy2MatCap = FindProperty("_Anisotropy2MatCap", props);
                anisotropy2MatCap2nd = FindProperty("_Anisotropy2MatCap2nd", props);
            useReflection = FindProperty("_UseReflection", props);
                smoothness = FindProperty("_Smoothness", props);
                smoothnessTex = FindProperty("_SmoothnessTex", props);
                metallic = FindProperty("_Metallic", props);
                metallicGlossMap = FindProperty("_MetallicGlossMap", props);
                reflectance = FindProperty("_Reflectance", props);
                reflectionColor = FindProperty("_ReflectionColor", props);
                reflectionColorTex = FindProperty("_ReflectionColorTex", props);
                applySpecular = FindProperty("_ApplySpecular", props);
                applySpecularFA = FindProperty("_ApplySpecularFA", props);
                specularNormalStrength = FindProperty("_SpecularNormalStrength", props);
                specularToon = FindProperty("_SpecularToon", props);
                specularBorder = FindProperty("_SpecularBorder", props);
                specularBlur = FindProperty("_SpecularBlur", props);
                applyReflection = FindProperty("_ApplyReflection", props);
                reflectionNormalStrength = FindProperty("_ReflectionNormalStrength", props);
                reflectionApplyTransparency = FindProperty("_ReflectionApplyTransparency", props);
            useMatCap = FindProperty("_UseMatCap", props);
                matcapTex = FindProperty("_MatCapTex", props);
                matcapColor = FindProperty("_MatCapColor", props);
                matcapBlendUV1 = FindProperty("_MatCapBlendUV1", props);
                matcapZRotCancel = FindProperty("_MatCapZRotCancel", props);
                matcapPerspective = FindProperty("_MatCapPerspective", props);
                matcapVRParallaxStrength = FindProperty("_MatCapVRParallaxStrength", props);
                matcapBlend = FindProperty("_MatCapBlend", props);
                matcapBlendMask = FindProperty("_MatCapBlendMask", props);
                matcapEnableLighting = FindProperty("_MatCapEnableLighting", props);
                matcapShadowMask = FindProperty("_MatCapShadowMask", props);
                matcapBackfaceMask = FindProperty("_MatCapBackfaceMask", props);
                matcapLod = FindProperty("_MatCapLod", props);
                matcapBlendMode = FindProperty("_MatCapBlendMode", props);
                matcapApplyTransparency = FindProperty("_MatCapApplyTransparency", props);
                matcapNormalStrength = FindProperty("_MatCapNormalStrength", props);
                matcapCustomNormal = FindProperty("_MatCapCustomNormal", props);
                matcapBumpMap = FindProperty("_MatCapBumpMap", props);
                matcapBumpScale = FindProperty("_MatCapBumpScale", props);
            useMatCap2nd = FindProperty("_UseMatCap2nd", props);
                matcap2ndTex = FindProperty("_MatCap2ndTex", props);
                matcap2ndColor = FindProperty("_MatCap2ndColor", props);
                matcap2ndBlendUV1 = FindProperty("_MatCap2ndBlendUV1", props);
                matcap2ndZRotCancel = FindProperty("_MatCap2ndZRotCancel", props);
                matcap2ndPerspective = FindProperty("_MatCap2ndPerspective", props);
                matcap2ndVRParallaxStrength = FindProperty("_MatCap2ndVRParallaxStrength", props);
                matcap2ndBlend = FindProperty("_MatCap2ndBlend", props);
                matcap2ndBlendMask = FindProperty("_MatCap2ndBlendMask", props);
                matcap2ndEnableLighting = FindProperty("_MatCap2ndEnableLighting", props);
                matcap2ndShadowMask = FindProperty("_MatCap2ndShadowMask", props);
                matcap2ndBackfaceMask = FindProperty("_MatCap2ndBackfaceMask", props);
                matcap2ndLod = FindProperty("_MatCap2ndLod", props);
                matcap2ndBlendMode = FindProperty("_MatCap2ndBlendMode", props);
                matcap2ndApplyTransparency = FindProperty("_MatCap2ndApplyTransparency", props);
                matcap2ndNormalStrength = FindProperty("_MatCap2ndNormalStrength", props);
                matcap2ndCustomNormal = FindProperty("_MatCap2ndCustomNormal", props);
                matcap2ndBumpMap = FindProperty("_MatCap2ndBumpMap", props);
                matcap2ndBumpScale = FindProperty("_MatCap2ndBumpScale", props);
            useRim = FindProperty("_UseRim", props);
                rimColor = FindProperty("_RimColor", props);
                rimColorTex = FindProperty("_RimColorTex", props);
                rimNormalStrength = FindProperty("_RimNormalStrength", props);
                rimBorder = FindProperty("_RimBorder", props);
                rimBlur = FindProperty("_RimBlur", props);
                rimFresnelPower = FindProperty("_RimFresnelPower", props);
                rimEnableLighting = FindProperty("_RimEnableLighting", props);
                rimShadowMask = FindProperty("_RimShadowMask", props);
                rimBackfaceMask = FindProperty("_RimBackfaceMask", props);
                rimVRParallaxStrength = FindProperty("_RimVRParallaxStrength", props);
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
                glitterBackfaceMask = FindProperty("_GlitterBackfaceMask", props);
                glitterApplyTransparency = FindProperty("_GlitterApplyTransparency", props);
                glitterParams1 = FindProperty("_GlitterParams1", props);
                glitterParams2 = FindProperty("_GlitterParams2", props);
                glitterVRParallaxStrength = FindProperty("_GlitterVRParallaxStrength", props);
                glitterNormalStrength = FindProperty("_GlitterNormalStrength", props);
            useEmission = FindProperty("_UseEmission", props);
                emissionColor = FindProperty("_EmissionColor", props);
                emissionMap = FindProperty("_EmissionMap", props);
                emissionMap_ScrollRotate = FindProperty("_EmissionMap_ScrollRotate", props);
                emissionMap_UVMode = FindProperty("_EmissionMap_UVMode", props);
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
                emission2ndMap_UVMode = FindProperty("_Emission2ndMap_UVMode", props);
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
                audioLinkDefaultValue = FindProperty("_AudioLinkDefaultValue", props);
                audioLinkUVMode = FindProperty("_AudioLinkUVMode", props);
                audioLinkUVParams = FindProperty("_AudioLinkUVParams", props);
                audioLinkStart = FindProperty("_AudioLinkStart", props);
                audioLinkMask = FindProperty("_AudioLinkMask", props);
                audioLink2Main2nd = FindProperty("_AudioLink2Main2nd", props);
                audioLink2Main3rd = FindProperty("_AudioLink2Main3rd", props);
                audioLink2Emission = FindProperty("_AudioLink2Emission", props);
                audioLink2EmissionGrad = FindProperty("_AudioLink2EmissionGrad", props);
                audioLink2Emission2nd = FindProperty("_AudioLink2Emission2nd", props);
                audioLink2Emission2ndGrad = FindProperty("_AudioLink2Emission2ndGrad", props);
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

        private void LoadFurProperties(MaterialProperty[] props)
        {
            invisible = FindProperty("_Invisible", props);
            asUnlit = FindProperty("_AsUnlit", props);
            cutoff = FindProperty("_Cutoff", props);
            subpassCutoff = FindProperty("_SubpassCutoff", props);
            flipNormal = FindProperty("_FlipNormal", props);
            shiftBackfaceUV = FindProperty("_ShiftBackfaceUV", props);
            backfaceForceShadow = FindProperty("_BackfaceForceShadow", props);
            vertexLightStrength = FindProperty("_VertexLightStrength", props);
            lightMinLimit = FindProperty("_LightMinLimit", props);
            lightMaxLimit = FindProperty("_LightMaxLimit", props);
            beforeExposureLimit = FindProperty("_BeforeExposureLimit", props);
            monochromeLighting = FindProperty("_MonochromeLighting", props);
            lilDirectionalLightStrength = FindProperty("_lilDirectionalLightStrength", props);
            lightDirectionOverride = FindProperty("_LightDirectionOverride", props);
            baseColor = FindProperty("_BaseColor", props);
            baseMap = FindProperty("_BaseMap", props);
            baseColorMap = FindProperty("_BaseColorMap", props);
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
                zclip = FindProperty("_ZClip", props);
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
                alphaToMask = FindProperty("_AlphaToMask", props);
            // Main
            //useMainTex = FindProperty("_UseMainTex", props);
                mainColor = FindProperty("_Color", props);
                mainTex = FindProperty("_MainTex", props);
                mainTexHSVG = FindProperty("_MainTexHSVG", props);
            // Shadow
            useShadow = FindProperty("_UseShadow", props);
                shadowStrength = FindProperty("_ShadowStrength", props);
                shadowStrengthMask = FindProperty("_ShadowStrengthMask", props);
                shadowAOShift = FindProperty("_ShadowAOShift", props);
                shadowColor = FindProperty("_ShadowColor", props);
                shadowColorTex = FindProperty("_ShadowColorTex", props);
                shadowNormalStrength = FindProperty("_ShadowNormalStrength", props);
                shadowBorder = FindProperty("_ShadowBorder", props);
                shadowBorderMask = FindProperty("_ShadowBorderMask", props);
                shadowBlur = FindProperty("_ShadowBlur", props);
                shadowBlurMask = FindProperty("_ShadowBlurMask", props);
                shadow2ndColor = FindProperty("_Shadow2ndColor", props);
                shadow2ndColorTex = FindProperty("_Shadow2ndColorTex", props);
                shadow2ndNormalStrength = FindProperty("_Shadow2ndNormalStrength", props);
                shadow2ndBorder = FindProperty("_Shadow2ndBorder", props);
                shadow2ndBlur = FindProperty("_Shadow2ndBlur", props);
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
                furRootOffset = FindProperty("_FurRootOffset", props);
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
                furZclip = FindProperty("_FurZClip", props);
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
                furAlphaToMask = FindProperty("_FurAlphaToMask", props);
        }

        private void LoadGemProperties(MaterialProperty[] props)
        {
            invisible = FindProperty("_Invisible", props);
            asUnlit = FindProperty("_AsUnlit", props);
            cutoff = FindProperty("_Cutoff", props);
            subpassCutoff = FindProperty("_SubpassCutoff", props);
            shiftBackfaceUV = FindProperty("_ShiftBackfaceUV", props);
            vertexLightStrength = FindProperty("_VertexLightStrength", props);
            lightMinLimit = FindProperty("_LightMinLimit", props);
            lightMaxLimit = FindProperty("_LightMaxLimit", props);
            beforeExposureLimit = FindProperty("_BeforeExposureLimit", props);
            monochromeLighting = FindProperty("_MonochromeLighting", props);
            lilDirectionalLightStrength = FindProperty("_lilDirectionalLightStrength", props);
            lightDirectionOverride = FindProperty("_LightDirectionOverride", props);
            baseColor = FindProperty("_BaseColor", props);
            baseMap = FindProperty("_BaseMap", props);
            baseColorMap = FindProperty("_BaseColorMap", props);
            mainTex_ScrollRotate = FindProperty("_MainTex_ScrollRotate", props);
                cull = FindProperty("_Cull", props);
                srcBlend = FindProperty("_SrcBlend", props);
                dstBlend = FindProperty("_DstBlend", props);
                srcBlendAlpha = FindProperty("_SrcBlendAlpha", props);
                dstBlendAlpha = FindProperty("_DstBlendAlpha", props);
                blendOp = FindProperty("_BlendOp", props);
                blendOpAlpha = FindProperty("_BlendOpAlpha", props);
                zclip = FindProperty("_ZClip", props);
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
                alphaToMask = FindProperty("_AlphaToMask", props);
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
            useAnisotropy = FindProperty("_UseAnisotropy", props);
                anisotropyTangentMap = FindProperty("_AnisotropyTangentMap", props);
                anisotropyScale = FindProperty("_AnisotropyScale", props);
                anisotropyScaleMask = FindProperty("_AnisotropyScaleMask", props);
                anisotropyTangentWidth = FindProperty("_AnisotropyTangentWidth", props);
                anisotropyBitangentWidth = FindProperty("_AnisotropyBitangentWidth", props);
                anisotropyShift = FindProperty("_AnisotropyShift", props);
                anisotropyShiftNoiseScale = FindProperty("_AnisotropyShiftNoiseScale", props);
                anisotropySpecularStrength = FindProperty("_AnisotropySpecularStrength", props);
                anisotropy2ndTangentWidth = FindProperty("_Anisotropy2ndTangentWidth", props);
                anisotropy2ndBitangentWidth = FindProperty("_Anisotropy2ndBitangentWidth", props);
                anisotropy2ndShift = FindProperty("_Anisotropy2ndShift", props);
                anisotropy2ndShiftNoiseScale = FindProperty("_Anisotropy2ndShiftNoiseScale", props);
                anisotropy2ndSpecularStrength = FindProperty("_Anisotropy2ndSpecularStrength", props);
                anisotropyShiftNoiseMask = FindProperty("_AnisotropyShiftNoiseMask", props);
                anisotropy2Reflection = FindProperty("_Anisotropy2Reflection", props);
                anisotropy2MatCap = FindProperty("_Anisotropy2MatCap", props);
                anisotropy2MatCap2nd = FindProperty("_Anisotropy2MatCap2nd", props);
            useMatCap = FindProperty("_UseMatCap", props);
                matcapTex = FindProperty("_MatCapTex", props);
                matcapColor = FindProperty("_MatCapColor", props);
                matcapBlendUV1 = FindProperty("_MatCapBlendUV1", props);
                matcapZRotCancel = FindProperty("_MatCapZRotCancel", props);
                matcapPerspective = FindProperty("_MatCapPerspective", props);
                matcapVRParallaxStrength = FindProperty("_MatCapVRParallaxStrength", props);
                matcapBlend = FindProperty("_MatCapBlend", props);
                matcapBlendMask = FindProperty("_MatCapBlendMask", props);
                matcapEnableLighting = FindProperty("_MatCapEnableLighting", props);
                matcapShadowMask = FindProperty("_MatCapShadowMask", props);
                matcapBackfaceMask = FindProperty("_MatCapBackfaceMask", props);
                matcapLod = FindProperty("_MatCapLod", props);
                matcapBlendMode = FindProperty("_MatCapBlendMode", props);
                matcapApplyTransparency = FindProperty("_MatCapApplyTransparency", props);
                matcapNormalStrength = FindProperty("_MatCapNormalStrength", props);
                matcapCustomNormal = FindProperty("_MatCapCustomNormal", props);
                matcapBumpMap = FindProperty("_MatCapBumpMap", props);
                matcapBumpScale = FindProperty("_MatCapBumpScale", props);
            useMatCap2nd = FindProperty("_UseMatCap2nd", props);
                matcap2ndTex = FindProperty("_MatCap2ndTex", props);
                matcap2ndColor = FindProperty("_MatCap2ndColor", props);
                matcap2ndBlendUV1 = FindProperty("_MatCap2ndBlendUV1", props);
                matcap2ndZRotCancel = FindProperty("_MatCap2ndZRotCancel", props);
                matcap2ndPerspective = FindProperty("_MatCap2ndPerspective", props);
                matcap2ndVRParallaxStrength = FindProperty("_MatCap2ndVRParallaxStrength", props);
                matcap2ndBlend = FindProperty("_MatCap2ndBlend", props);
                matcap2ndBlendMask = FindProperty("_MatCap2ndBlendMask", props);
                matcap2ndEnableLighting = FindProperty("_MatCap2ndEnableLighting", props);
                matcap2ndShadowMask = FindProperty("_MatCap2ndShadowMask", props);
                matcap2ndBackfaceMask = FindProperty("_MatCap2ndBackfaceMask", props);
                matcap2ndLod = FindProperty("_MatCap2ndLod", props);
                matcap2ndBlendMode = FindProperty("_MatCap2ndBlendMode", props);
                matcap2ndApplyTransparency = FindProperty("_MatCap2ndApplyTransparency", props);
                matcap2ndNormalStrength = FindProperty("_MatCap2ndNormalStrength", props);
                matcap2ndCustomNormal = FindProperty("_MatCap2ndCustomNormal", props);
                matcap2ndBumpMap = FindProperty("_MatCap2ndBumpMap", props);
                matcap2ndBumpScale = FindProperty("_MatCap2ndBumpScale", props);
            useRim = FindProperty("_UseRim", props);
                rimColor = FindProperty("_RimColor", props);
                rimColorTex = FindProperty("_RimColorTex", props);
                rimNormalStrength = FindProperty("_RimNormalStrength", props);
                rimBorder = FindProperty("_RimBorder", props);
                rimBlur = FindProperty("_RimBlur", props);
                rimFresnelPower = FindProperty("_RimFresnelPower", props);
                rimEnableLighting = FindProperty("_RimEnableLighting", props);
                rimShadowMask = FindProperty("_RimShadowMask", props);
                rimBackfaceMask = FindProperty("_RimBackfaceMask", props);
                rimVRParallaxStrength = FindProperty("_RimVRParallaxStrength", props);
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
                glitterBackfaceMask = FindProperty("_GlitterBackfaceMask", props);
                glitterApplyTransparency = FindProperty("_GlitterApplyTransparency", props);
                glitterParams1 = FindProperty("_GlitterParams1", props);
                glitterParams2 = FindProperty("_GlitterParams2", props);
                glitterVRParallaxStrength = FindProperty("_GlitterVRParallaxStrength", props);
                glitterNormalStrength = FindProperty("_GlitterNormalStrength", props);
                ignoreEncryption = FindProperty("_IgnoreEncryption", props);
                keys = FindProperty("_Keys", props);
                refractionStrength = FindProperty("_RefractionStrength", props);
                refractionFresnelPower = FindProperty("_RefractionFresnelPower", props);
            useEmission = FindProperty("_UseEmission", props);
                emissionColor = FindProperty("_EmissionColor", props);
                emissionMap = FindProperty("_EmissionMap", props);
                emissionMap_ScrollRotate = FindProperty("_EmissionMap_ScrollRotate", props);
                emissionMap_UVMode = FindProperty("_EmissionMap_UVMode", props);
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
                emission2ndMap_UVMode = FindProperty("_Emission2ndMap_UVMode", props);
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
                audioLinkDefaultValue = FindProperty("_AudioLinkDefaultValue", props);
                audioLinkUVMode = FindProperty("_AudioLinkUVMode", props);
                audioLinkUVParams = FindProperty("_AudioLinkUVParams", props);
                audioLinkStart = FindProperty("_AudioLinkStart", props);
                audioLinkMask = FindProperty("_AudioLinkMask", props);
                audioLink2Main2nd = FindProperty("_AudioLink2Main2nd", props);
                audioLink2Main3rd = FindProperty("_AudioLink2Main3rd", props);
                audioLink2Emission = FindProperty("_AudioLink2Emission", props);
                audioLink2EmissionGrad = FindProperty("_AudioLink2EmissionGrad", props);
                audioLink2Emission2nd = FindProperty("_AudioLink2Emission2nd", props);
                audioLink2Emission2ndGrad = FindProperty("_AudioLink2Emission2ndGrad", props);
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

        private void LoadFakeShadowProperties(MaterialProperty[] props)
        {
            invisible = FindProperty("_Invisible", props);
            baseColor = FindProperty("_BaseColor", props);
            baseMap = FindProperty("_BaseMap", props);
            baseColorMap = FindProperty("_BaseColorMap", props);
                cull = FindProperty("_Cull", props);
                srcBlend = FindProperty("_SrcBlend", props);
                dstBlend = FindProperty("_DstBlend", props);
                srcBlendAlpha = FindProperty("_SrcBlendAlpha", props);
                dstBlendAlpha = FindProperty("_DstBlendAlpha", props);
                blendOp = FindProperty("_BlendOp", props);
                blendOpAlpha = FindProperty("_BlendOpAlpha", props);
                zclip = FindProperty("_ZClip", props);
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
                alphaToMask = FindProperty("_AlphaToMask", props);
            //useMainTex = FindProperty("_UseMainTex", props);
                mainColor = FindProperty("_Color", props);
                mainTex = FindProperty("_MainTex", props);
                ignoreEncryption = FindProperty("_IgnoreEncryption", props);
                keys = FindProperty("_Keys", props);
                fakeShadowVector = FindProperty("_FakeShadowVector", props);
        }

        private void LoadLiteProperties(MaterialProperty[] props)
        {
            invisible = FindProperty("_Invisible", props);
            asUnlit = FindProperty("_AsUnlit", props);
            cutoff = FindProperty("_Cutoff", props);
            subpassCutoff = FindProperty("_SubpassCutoff", props);
            flipNormal = FindProperty("_FlipNormal", props);
            shiftBackfaceUV = FindProperty("_ShiftBackfaceUV", props);
            backfaceForceShadow = FindProperty("_BackfaceForceShadow", props);
            vertexLightStrength = FindProperty("_VertexLightStrength", props);
            lightMinLimit = FindProperty("_LightMinLimit", props);
            lightMaxLimit = FindProperty("_LightMaxLimit", props);
            beforeExposureLimit = FindProperty("_BeforeExposureLimit", props);
            monochromeLighting = FindProperty("_MonochromeLighting", props);
            lilDirectionalLightStrength = FindProperty("_lilDirectionalLightStrength", props);
            lightDirectionOverride = FindProperty("_LightDirectionOverride", props);
            baseColor = FindProperty("_BaseColor", props);
            baseMap = FindProperty("_BaseMap", props);
            baseColorMap = FindProperty("_BaseColorMap", props);
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
                zclip = FindProperty("_ZClip", props);
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
                alphaToMask = FindProperty("_AlphaToMask", props);
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
                outlineZclip = FindProperty("_OutlineZClip", props);
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
                outlineAlphaToMask = FindProperty("_OutlineAlphaToMask", props);
            }
            useMatCap = FindProperty("_UseMatCap", props);
                matcapTex = FindProperty("_MatCapTex", props);
                matcapBlendUV1 = FindProperty("_MatCapBlendUV1", props);
                matcapZRotCancel = FindProperty("_MatCapZRotCancel", props);
                matcapPerspective = FindProperty("_MatCapPerspective", props);
                matcapVRParallaxStrength = FindProperty("_MatCapVRParallaxStrength", props);
                matcapMul = FindProperty("_MatCapMul", props);
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
                emissionMap_UVMode = FindProperty("_EmissionMap_UVMode", props);
                emissionBlink = FindProperty("_EmissionBlink", props);
        }

        private void LoadMultiProperties(MaterialProperty[] props)
        {
            transparentModeMat = FindProperty("_TransparentMode", props);
            useOutline = FindProperty("_UseOutline", props);
            usePOM = FindProperty("_UsePOM", props);
            useClippingCanceller = FindProperty("_UseClippingCanceller", props);
            asOverlay = FindProperty("_AsOverlay", props);
        }

        private void CopyMainColorProperties()
        {
            if(mainColor != null && baseColor != null) baseColor.colorValue = mainColor.colorValue;
            if(mainTex != null && baseMap != null) baseMap.textureValue = mainTex.textureValue;
            if(mainTex != null && baseColorMap != null) baseColorMap.textureValue = mainTex.textureValue;
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Language
        public static string GetLoc(string value)
        {
            if(loc.ContainsKey(value)) return loc[value];
            return value;
        }

        public static void InitializeLanguage()
        {
            edSet.languageNum = InitializeLanguage(edSet.languageNum);
        }

        public static void LoadCustomLanguage(string langFileGUID)
        {
            int lnum = edSet.languageNum;
            if(loc.Count == 0)
            {
                string langPath = AssetDatabase.GUIDToAssetPath(langFileGUID);
                if(String.IsNullOrEmpty(langPath) || !File.Exists(langPath)) return;
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
        }

        private static int selectLang(int lnum)
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

        private static int InitializeLanguage(int lnum)
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

        //------------------------------------------------------------------------------------------------------------------------------
        // Rendering Pipeline
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
            foreach(string shaderGuid in shaderGuids)
            {
                string shaderPath = AssetDatabase.GUIDToAssetPath(shaderGuid);
                RewriteShaderRP(shaderPath, lilRP);
            }
            string shaderPipelinePath = GetShaderPipelinePath();
            RewriteShaderRP(shaderPipelinePath, lilRP);
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

        private static lilRenderPipeline CheckRP()
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
                if(String.IsNullOrEmpty(renderPipelineName))            lilRP = lilRenderPipeline.BRP;
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

        //------------------------------------------------------------------------------------------------------------------------------
        // Shader Setting
        public static void InitializeShaderSetting(ref lilToonSetting shaderSetting)
        {
            if(shaderSetting != null) return;
            string shaderSettingPath = GetShaderSettingPath();
            shaderSetting = (lilToonSetting)AssetDatabase.LoadAssetAtPath(shaderSettingPath, typeof(lilToonSetting));
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
            string[] shaderFolderPaths = GetShaderFolderPaths();
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
            sb.Append("\r\n#endif");
            string shaderSettingString = sb.ToString();

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
                string[] shaderFolderPaths = GetShaderFolderPaths();
                bool isShadowReceive = shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW || shaderSetting.LIL_FEATURE_BACKLIGHT;
                string[] shaderGuids = AssetDatabase.FindAssets("t:shader", shaderFolderPaths);
                foreach(string shaderGuid in shaderGuids)
                {
                    string shaderPath = AssetDatabase.GUIDToAssetPath(shaderGuid);
                    RewriteReceiveShadow(shaderPath, isShadowReceive);
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(shaderSettingHLSLPath);
                AssetDatabase.Refresh();
            }
        }

        public static bool EqualsShaderSetting(lilToonSetting ssA, lilToonSetting ssB)
        {
            if((ssA == null && ssB != null) || (ssA != null && ssB == null)) return false;
            if(ssA == null && ssB == null) return true;

            FieldInfo[] fields = typeof(lilToonSetting).GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach(FieldInfo field in fields)
            {
                if((bool)field.GetValue(ssA) != (bool)field.GetValue(ssB))
                {
                    return false;
                }
            }

            return true;
        }

        public static void CopyShaderSetting(ref lilToonSetting ssA, lilToonSetting ssB)
        {
            if(ssA == null || ssB == null) return;

            FieldInfo[] fields = typeof(lilToonSetting).GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach(FieldInfo field in fields)
            {
                field.SetValue(ssA, field.GetValue(ssB));
            }
        }

        private static void ShaderSettingGUI()
        {
            GUIStyle applyButton = new GUIStyle(GUI.skin.button);
            applyButton.normal.textColor = Color.red;
            applyButton.fontStyle = FontStyle.Bold;

            lilToggleGUI(GetLoc("sSettingCancelAutoScan"), ref shaderSetting.shouldNotScan);
            lilToggleGUI(GetLoc("sSettingLock"), ref shaderSetting.isLocked);
            GUI.enabled = !shaderSetting.isLocked;

            // Apply Button
            if(edSet.isShaderSettingChanged && GUILayout.Button(GetLoc("sSettingApply"), applyButton))
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
                lilToggleGUI(GetLoc("sSettingAnimateEmissionUV"), ref shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV);
                lilToggleGUI(GetLoc("sSettingTexEmissionMask"), ref shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK);
                if(shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK)
                {
                    EditorGUI.indentLevel++;
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

            lilToggleGUI(GetLoc("sSettingAnisotropy"), ref shaderSetting.LIL_FEATURE_ANISOTROPY);
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

            lilToggleGUI(GetLoc("sSettingBacklight"), ref shaderSetting.LIL_FEATURE_BACKLIGHT);
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
            lilToggleGUI(GetLoc("sSettingTexOutlineNormal"), ref shaderSetting.LIL_FEATURE_TEX_OUTLINE_NORMAL);
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

        //------------------------------------------------------------------------------------------------------------------------------
        // Initialize Editor Variable
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
            ltsmref     = Shader.Find("Hidden/lilToonMultiRefraction");
            ltsmfur     = Shader.Find("Hidden/lilToonMultiFur");
            ltsmgem     = Shader.Find("Hidden/lilToonMultiGem");

            mtoon       = Shader.Find("VRM/MToon");
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Shader Rewriter
        public static void ReimportPassShaders()
        {
            string[] shaderFolderPaths = GetShaderFolderPaths();
            string[] shaderGuids = AssetDatabase.FindAssets("t:shader", shaderFolderPaths);
            foreach(string shaderGuid in shaderGuids)
            {
                string shaderPath = AssetDatabase.GUIDToAssetPath(shaderGuid);
                if(shaderPath.Contains("ltsmulti")) AssetDatabase.ImportAsset(shaderPath);
                if(shaderPath.Contains("ltspass")) AssetDatabase.ImportAsset(shaderPath);
                if(shaderPath.Contains("fur")) AssetDatabase.ImportAsset(shaderPath);
                if(shaderPath.Contains("ref")) AssetDatabase.ImportAsset(shaderPath);
                if(shaderPath.Contains("gem")) AssetDatabase.ImportAsset(shaderPath);
                if(shaderPath.Contains("fakeshadow")) AssetDatabase.ImportAsset(shaderPath);
            }
        }

        public static void RewriteReceiveShadow(string path, bool enable)
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

        public static void RewriteReceiveShadow(Shader shader, bool enable)
        {
            string path = AssetDatabase.GetAssetPath(shader);
            RewriteReceiveShadow(path, enable);
        }

        public static void RewriteZClip(string path)
        {
            if(String.IsNullOrEmpty(path) || !File.Exists(path)) return;
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

        //------------------------------------------------------------------------------------------------------------------------------
        // GUI
        public static bool Foldout(string title, string help, bool display)
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

        public static void DrawLine()
        {
            EditorGUI.DrawRect(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, 1)), lineColor);
        }

        public static void DrawWebButton(string text, string URL)
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

        public static bool CheckFeature(bool feature)
        {
            return isMulti || feature;
        }

        private static void DrawHelpButton(string helpAnchor)
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

        private static void DrawWebPages()
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

        private static void DrawHelpPages()
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

        private static void lilToggleGUI(string label, ref bool value)
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

        private void DrawMenuButton(string helpAnchor, lilPropertyBlock propertyBlock)
        {
            Rect position = GUILayoutUtility.GetLastRect();
            position.x += position.width - 24;
            position.width = 24;
            GUIContent icon = EditorGUIUtility.IconContent("_Popup");
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;

            if(GUI.Button(position, icon, style))
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

        private void DrawVRCFallbackGUI(Material material, GUIStyle customToggleFont, GUIStyle boxOuter, GUIStyle boxInnerHalf)
        {
            #if VRC_SDK_VRCSDK2 || VRC_SDK_VRCSDK3
                edSet.isShowVRChat = Foldout("VRChat", "VRChat", edSet.isShowVRChat);
                if(edSet.isShowVRChat)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    string tag = material.GetTag("VRCFallback", false);
                    bool shouldSetTag = EditorGUI.ToggleLeft(EditorGUILayout.GetControlRect(), "Custom Safety Fallback", !String.IsNullOrEmpty(tag), customToggleFont);
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
                        material.SetOverrideTag("VRCFallback", tag);
                        EditorGUILayout.EndVertical();
                    }
                    else
                    {
                        material.SetOverrideTag("VRCFallback", "");
                    }
                    EditorGUILayout.EndVertical();
                }
            #endif
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Editor
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

        private static void SaveEditorSettingTemp()
        {
            StreamWriter sw = new StreamWriter(editorSettingTempPath,false);
            sw.Write(EditorJsonUtility.ToJson(edSet));
            sw.Close();
        }

        private static void VersionCheck()
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
                        CopyProperty(shadow2ndColor);
                        CopyProperty(shadow2ndNormalStrength);
                        CopyProperty(shadow2ndBorder);
                        CopyProperty(shadow2ndBlur);
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
                        CopyProperty(metallicGlossMap);
                        CopyProperty(smoothnessTex);
                        CopyProperty(reflectionColorTex);
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
                        CopyProperty(furAO);
                        CopyProperty(vertexColor2FurVector);
                        CopyProperty(furLayerNum);
                        CopyProperty(furRootOffset);
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
                        PasteProperty(ref shadow2ndColor);
                        PasteProperty(ref shadow2ndNormalStrength);
                        PasteProperty(ref shadow2ndBorder);
                        PasteProperty(ref shadow2ndBlur);
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
                        if(shouldCopyTex)
                        {
                            PasteProperty(ref metallicGlossMap);
                            PasteProperty(ref smoothnessTex);
                            PasteProperty(ref reflectionColorTex);
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
                        PasteProperty(ref furAO);
                        PasteProperty(ref vertexColor2FurVector);
                        PasteProperty(ref furLayerNum);
                        PasteProperty(ref furRootOffset);
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
                        ResetProperty(ref shadow2ndColor);
                        ResetProperty(ref shadow2ndNormalStrength);
                        ResetProperty(ref shadow2ndBorder);
                        ResetProperty(ref shadow2ndBlur);
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
                        ResetProperty(ref metallicGlossMap);
                        ResetProperty(ref smoothnessTex);
                        ResetProperty(ref reflectionColorTex);
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
                        ResetProperty(ref furAO);
                        ResetProperty(ref vertexColor2FurVector);
                        ResetProperty(ref furLayerNum);
                        ResetProperty(ref furRootOffset);
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
                    if(asUnlit != null) asUnlit.floatValue = 0.0f;
                    if(vertexLightStrength != null) vertexLightStrength.floatValue = 0.0f;
                    if(lightMinLimit != null) lightMinLimit.floatValue = 0.0f;
                    if(lightMaxLimit != null) lightMaxLimit.floatValue = 1.0f;
                    if(monochromeLighting != null) monochromeLighting.floatValue = 0.0f;
                    if(shadowEnvStrength != null) shadowEnvStrength.floatValue = 0.0f;
                    break;
                case lilLightingPreset.Stable:
                    if(asUnlit != null) asUnlit.floatValue = 0.0f;
                    if(vertexLightStrength != null) vertexLightStrength.floatValue = 0.0f;
                    if(lightMinLimit != null) lightMinLimit.floatValue = 0.05f;
                    if(lightMaxLimit != null) lightMaxLimit.floatValue = 1.0f;
                    if(monochromeLighting != null) monochromeLighting.floatValue = 0.5f;
                    if(shadowEnvStrength != null) shadowEnvStrength.floatValue = 0.0f;
                    break;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Material Setup
        public static void SetupMaterialWithRenderingMode(Material material, RenderingMode renderingMode, TransparentMode transparentMode, bool isoutl, bool islite, bool isstencil, bool istess)
        {
            if(isMulti)
            {
                lilRenderPipeline lilRP = CheckRP();
                float tpmode = material.GetFloat("_TransparentMode");
                if(tpmode == 1.0f)
                {
                    material.shader = ltsm;
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
                    material.shader = ltsm;
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
                    material.shader = ltsm;
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
            }

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
            }
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
            }

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
            foreach(string word in mainTexCheckWords)
            {
                if(name.Contains(word)) return false;
            }
            return true;
        }

        public static void RemoveUnusedTexture(Material material)
        {
            if(!material.shader.name.Contains("lilToon")) return;
            lilToonSetting shaderSetting = null;
            InitializeShaderSetting(ref shaderSetting);
            RemoveUnusedTexture(material, material.shader.name.Contains("Lite"), material.shader.name.Contains("Fur"), shaderSetting);
        }

        public static void SetShaderKeywords(Material material, string keyword, bool enable)
        {
            if(enable)  material.EnableKeyword(keyword);
            else        material.DisableKeyword(keyword);
        }

        public void SetupMultiMaterial(Material material)
        {
            SetShaderKeywords(material, "ETC1_EXTERNAL_ALPHA",                  material.GetFloat("_UseOutline") != 0.0f);
            SetShaderKeywords(material, "UNITY_UI_ALPHACLIP",                   material.GetFloat("_TransparentMode") == 1.0f);
            SetShaderKeywords(material, "UNITY_UI_CLIP_RECT",                   material.GetFloat("_TransparentMode") == 2.0f || material.GetFloat("_TransparentMode") == 4.0f);
            SetShaderKeywords(material, "BILLBOARD_FACE_CAMERA_POS",            material.GetFloat("_UseClippingCanceller") != 0.0f);
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

            if(isFur)
            {
                SetShaderKeywords(material, "_EMISSION",                            false);
                SetShaderKeywords(material, "GEOM_TYPE_BRANCH",                     false);
                SetShaderKeywords(material, "_SUNDISK_SIMPLE",                      false);
                SetShaderKeywords(material, "_NORMALMAP",                           false);
                SetShaderKeywords(material, "EFFECT_BUMP",                          false);
                SetShaderKeywords(material, "SOURCE_GBUFFER",                       false);
                SetShaderKeywords(material, "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A", false);
                SetShaderKeywords(material, "_SPECULARHIGHLIGHTS_OFF",              false);
                SetShaderKeywords(material, "GEOM_TYPE_MESH",                       false);
                SetShaderKeywords(material, "_METALLICGLOSSMAP",                    false);
                SetShaderKeywords(material, "GEOM_TYPE_LEAF",                       false);
                SetShaderKeywords(material, "_SPECGLOSSMAP",                        false);
                SetShaderKeywords(material, "_MAPPING_6_FRAMES_LAYOUT",             false);
                SetShaderKeywords(material, "_SUNDISK_HIGH_QUALITY",                false);
                SetShaderKeywords(material, "GEOM_TYPE_BRANCH_DETAIL",              false);
            }
            else
            {
                SetShaderKeywords(material, "_EMISSION",                            useEmission.floatValue != 0.0f);
                SetShaderKeywords(material, "GEOM_TYPE_BRANCH",                     useEmission2nd.floatValue != 0.0f);
                SetShaderKeywords(material, "_SUNDISK_SIMPLE",                      useEmission.floatValue != 0.0f && emissionBlendMask.textureValue != null || useEmission2nd.floatValue != 0.0f && emission2ndBlendMask.textureValue != null);
                SetShaderKeywords(material, "_NORMALMAP",                           useBumpMap.floatValue != 0.0f);
                SetShaderKeywords(material, "EFFECT_BUMP",                          useBump2ndMap.floatValue != 0.0f);
                SetShaderKeywords(material, "SOURCE_GBUFFER",                       useAnisotropy.floatValue != 0.0f);
                SetShaderKeywords(material, "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A", useMatCap.floatValue != 0.0f);
                SetShaderKeywords(material, "_SPECULARHIGHLIGHTS_OFF",              useMatCap2nd.floatValue != 0.0f);
                SetShaderKeywords(material, "GEOM_TYPE_MESH",                       useMatCap.floatValue != 0.0f && matcapCustomNormal.floatValue != 0.0f || useMatCap2nd.floatValue != 0.0f && matcap2ndCustomNormal.floatValue != 0.0f);
                SetShaderKeywords(material, "_METALLICGLOSSMAP",                    useRim.floatValue != 0.0f);
                SetShaderKeywords(material, "GEOM_TYPE_LEAF",                       rimDirStrength.floatValue != 0.0f);
                SetShaderKeywords(material, "_SPECGLOSSMAP",                        useGlitter.floatValue != 0.0f);
                SetShaderKeywords(material, "_MAPPING_6_FRAMES_LAYOUT",             useAudioLink.floatValue != 0.0f);
                SetShaderKeywords(material, "_SUNDISK_HIGH_QUALITY",                audioLinkAsLocal.floatValue != 0.0f);
                SetShaderKeywords(material, "GEOM_TYPE_BRANCH_DETAIL",              dissolveParams.vectorValue.x != 0.0f);
            }

            if(isFur || isGem)
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
                SetShaderKeywords(material, "_SUNDISK_NONE",                        useMain2ndTex.floatValue != 0.0f && main2ndTexDecalAnimation.vectorValue != defaultDecalAnim || useMain3rdTex.floatValue != 0.0f && main3rdTexDecalAnimation.vectorValue != defaultDecalAnim);
                SetShaderKeywords(material, "GEOM_TYPE_FROND",                      useMain2ndTex.floatValue != 0.0f && main2ndDissolveParams.vectorValue.x != 0.0f || useMain3rdTex.floatValue != 0.0f && main3rdDissolveParams.vectorValue.x != 0.0f);
                SetShaderKeywords(material, "_COLOROVERLAY_ON",                     material.GetFloat("_TransparentMode") != 0.0f && alphaMaskMode.floatValue != 0.0f);
                SetShaderKeywords(material, "ANTI_FLICKER",                         useBacklight.floatValue != 0.0f);
                SetShaderKeywords(material, "_PARALLAXMAP",                         useParallax.floatValue != 0.0f);
                SetShaderKeywords(material, "PIXELSNAP_ON",                         material.GetFloat("_UsePOM") != 0.0f);
                SetShaderKeywords(material, "_GLOSSYREFLECTIONS_OFF",               useReflection.floatValue != 0.0f);
            }

            if(isRefr || isFur || isGem)
            {
                material.SetShaderPassEnabled("SRPDEFAULTUNLIT",                    true);
                SetShaderKeywords(material, "_DETAIL_MULX2",                        false);
            }
            else
            {
                material.SetShaderPassEnabled("SRPDEFAULTUNLIT",                    material.GetFloat("_UseOutline") != 0.0f);
                SetShaderKeywords(material, "_DETAIL_MULX2",                        material.GetFloat("_UseOutline") != 0.0f && material.GetVector("_OutlineTexHSVG") != defaultHSVG);
            }
        }

        private static void RemoveUnusedTexture(Material material, bool islite, bool isfur, lilToonSetting shaderSetting)
        {
            if(!isMulti) SetupMaterialFromShaderSetting(material, shaderSetting);
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

        private static bool CheckScaleOffsetChanged(Material material, string texname)
        {
            return material.GetTextureScale(texname) != defaultTextureScale || material.GetTextureOffset(texname) != defaultTextureOffset;
        }

        private static void ResetScaleOffset(Material material, string texname)
        {
            material.SetTextureScale(texname, defaultTextureScale);
            material.SetTextureOffset(texname, defaultTextureOffset);
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Presets
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
            bool isstencil      = (material.GetFloat("_StencilPass") == (float)UnityEngine.Rendering.StencilOp.Replace);

            bool islite         = material.shader.name.Contains("Lite");
            bool iscutout       = material.shader.name.Contains("Cutout");
            bool istransparent  = material.shader.name.Contains("Transparent");
            bool isrefr         = preset.shader != null && preset.shader.name.Contains("Refraction");
            bool isblur         = preset.shader != null && preset.shader.name.Contains("Blur");
            bool isfur          = preset.shader != null && preset.shader.name.Contains("Fur");
            bool isonepass      = material.shader.name.Contains("OnePass");
            bool istwopass      = material.shader.name.Contains("TwoPass");

            RenderingMode       renderingMode = RenderingMode.Opaque;
            if(iscutout)        renderingMode = RenderingMode.Cutout;
            if(istransparent)   renderingMode = RenderingMode.Transparent;
            if(isrefr)          renderingMode = RenderingMode.Refraction;
            if(isrefr&isblur)   renderingMode = RenderingMode.RefractionBlur;
            if(isfur)           renderingMode = RenderingMode.Fur;
            if(isfur&iscutout)  renderingMode = RenderingMode.FurCutout;

            TransparentMode     transparentMode = TransparentMode.Normal;
            if(isonepass)       transparentMode = TransparentMode.OnePass;
            if(istwopass)       transparentMode = TransparentMode.TwoPass;

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

        private static void DrawPreset(Material material)
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

        private static void LoadPresets()
        {
            string[] presetGuid = AssetDatabase.FindAssets("t:lilToonPreset", new[] {GetPresetsFolderPath()});
            Array.Resize(ref presets, presetGuid.Length);
            for(int i=0; i<presetGuid.Length; i++)
            {
                presets[i] = (lilToonPreset)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(presetGuid[i]), typeof(lilToonPreset));
            }
        }

        private static void ShowPresets(Material material)
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

        //------------------------------------------------------------------------------------------------------------------------------
        // Material Converter
        private void CreateMToonMaterial(Material material)
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
                mtoonMaterial.SetFloat("_BlendMode",                0.0f);
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

            bool isonepass      = material.shader.name.Contains("OnePass");
            bool istwopass      = material.shader.name.Contains("TwoPass");

            TransparentMode     transparentMode = TransparentMode.Normal;
            if(isonepass)       transparentMode = TransparentMode.OnePass;
            if(istwopass)       transparentMode = TransparentMode.TwoPass;

            SetupMaterialWithRenderingMode(liteMaterial, renderingMode, transparentMode, isOutl, true, isStWr, false);

            string matPath = AssetDatabase.GetAssetPath(material);
            if(!String.IsNullOrEmpty(matPath))  matPath = EditorUtility.SaveFilePanel("Save Material", Path.GetDirectoryName(matPath), Path.GetFileNameWithoutExtension(matPath)+"_lite", "mat");
            else                    matPath = EditorUtility.SaveFilePanel("Save Material", "Assets", material.name + ".mat", "mat");
            if(String.IsNullOrEmpty(matPath))   return;
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
            isMulti = true;

            if(renderingModeBuf == RenderingMode.Cutout)            material.SetFloat("_TransparentMode", 1.0f);
            else if(renderingModeBuf == RenderingMode.Transparent)  material.SetFloat("_TransparentMode", 2.0f);
            else                                                    material.SetFloat("_TransparentMode", 0.0f);
            material.SetFloat("_UseOutline", 0.0f);
            material.SetFloat("_UsePOM", 0.0f);
            material.SetFloat("_UseClippingCanceller", 0.0f);

            if(isOutl) material.SetFloat("_UseOutline", 1.0f);
            if(shaderSetting.LIL_FEATURE_POM) material.SetFloat("_UsePOM", 1.0f);
            if(shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER) material.SetFloat("_UseClippingCanceller", 1.0f);

            SetupMaterialWithRenderingMode(material, renderingModeBuf, TransparentMode.Normal, isOutl, false, isStWr, false);
            SetupMultiMaterial(material);
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Property drawer
        private void DrawLightingSettings(MaterialEditor materialEditor)
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontStyle = FontStyle.Bold;
            EditorGUI.indentLevel++;
            Rect position = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(position, GetLoc("sLightingSettings"), labelStyle);
            EditorGUI.indentLevel--;

            position.x += 10;
            edSet.isShowLightingSettings = EditorGUI.Foldout(position, edSet.isShowLightingSettings, "");
            DrawMenuButton(GetLoc("sAnchorBaseSetting"), lilPropertyBlock.Lighting);
            if(edSet.isShowLightingSettings)
            {
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(asUnlit, GetLoc("sAsUnlit"));
                materialEditor.ShaderProperty(vertexLightStrength, GetLoc("sVertexLightStrength"));
                materialEditor.ShaderProperty(lightMinLimit, GetLoc("sLightMinLimit"));
                materialEditor.ShaderProperty(lightMaxLimit, GetLoc("sLightMaxLimit"));
                materialEditor.ShaderProperty(monochromeLighting, GetLoc("sMonochromeLighting"));
                materialEditor.ShaderProperty(lightDirectionOverride, GetLoc("sLightDirectionOverride"));
                if(shadowEnvStrength != null) materialEditor.ShaderProperty(shadowEnvStrength, GetLoc("sShadowEnvStrength"));
                GUILayout.BeginHorizontal();
                Rect position2 = EditorGUILayout.GetControlRect();
                Rect labelRect = new Rect(position2.x, position2.y, EditorGUIUtility.labelWidth, position2.height);
                Rect buttonRect1 = new Rect(labelRect.x + labelRect.width, position2.y, (position2.width - EditorGUIUtility.labelWidth)*0.5f, position2.height);
                Rect buttonRect2 = new Rect(buttonRect1.x + buttonRect1.width, position2.y, buttonRect1.width, position2.height);
                EditorGUI.PrefixLabel(labelRect, new GUIContent(GetLoc("sLightingPreset")));
                if(GUI.Button(buttonRect1, new GUIContent(GetLoc("sLightingPresetDefault")))) ApplyLightingPreset(lilLightingPreset.Default);
                if(GUI.Button(buttonRect2, new GUIContent(GetLoc("sLightingPresetStable")))) ApplyLightingPreset(lilLightingPreset.Stable);
                GUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }
        }

        private void UV4Decal(MaterialEditor materialEditor, MaterialProperty isDecal, MaterialProperty isLeftOnly, MaterialProperty isRightOnly, MaterialProperty shouldCopy, MaterialProperty shouldFlipMirror, MaterialProperty shouldFlipCopy, MaterialProperty tex, MaterialProperty angle, MaterialProperty decalAnimation, MaterialProperty decalSubParam)
        {
            if(CheckFeature(shaderSetting.LIL_FEATURE_DECAL))
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

                if(CheckFeature(shaderSetting.LIL_FEATURE_ANIMATE_DECAL))
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

        private void ToneCorrectionGUI(MaterialEditor materialEditor, Material material, MaterialProperty tex, MaterialProperty col, MaterialProperty hsvg, bool isOutline = false)
        {
            materialEditor.ShaderProperty(hsvg, GetLoc("sHue") + "|" + GetLoc("sSaturation") + "|" + GetLoc("sValue") + "|" + GetLoc("sGamma"));
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
            param1 = EditorGUILayout.FloatField(label0, param1);
            param2 = EditorGUILayout.FloatField(label1, param2);
            param3 = EditorGUILayout.FloatField(label2, param3);
            param4 = EditorGUILayout.FloatField(label3, param4);

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

        private void TextureBakeGUI(Material material, int bakeType, GUIStyle guistyle)
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

        private void UVSettingGUI(MaterialEditor materialEditor, MaterialProperty uvst, MaterialProperty uvsr)
        {
            EditorGUILayout.LabelField(GetLoc("sUVSetting"), EditorStyles.boldLabel);
            materialEditor.TextureScaleOffsetProperty(uvst);
            materialEditor.ShaderProperty(uvsr, GetLoc("sAngle") + "|" + GetLoc("sUVAnimation") + "|" + GetLoc("sScroll") + "|" + GetLoc("sRotate"));
        }

        private void BlendSettingGUI(MaterialEditor materialEditor, ref bool isShow, string labelName, MaterialProperty srcRGB, MaterialProperty dstRGB, MaterialProperty srcA, MaterialProperty dstA, MaterialProperty opRGB, MaterialProperty opA)
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

        private void TextureGUI(MaterialEditor materialEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName)
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

        private void TextureGUI(MaterialEditor materialEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba)
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

        private void TextureGUI(MaterialEditor materialEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty scrollRotate)
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

        private void TextureGUI(MaterialEditor materialEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty scrollRotate, bool useCustomUV, bool useUVAnimation)
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

        private void TextureGUI(MaterialEditor materialEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty scrollRotate, MaterialProperty uvMode, bool useCustomUV, bool useUVAnimation)
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
                    materialEditor.ShaderProperty(uvMode, "UV Mode|UV0|UV1|UV2|UV3|Rim");
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                materialEditor.TexturePropertySingleLine(guiContent, textureName, rgba);
            }
        }

        private void MatCapTextureGUI(MaterialEditor materialEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty blendUV1, MaterialProperty zRotCancel, MaterialProperty perspective, MaterialProperty vrParallaxStrength)
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
                materialEditor.ShaderProperty(blendUV1, GetLoc("sBlendUV1"));
                materialEditor.ShaderProperty(zRotCancel, GetLoc("sMatCapZRotCancel"));
                materialEditor.ShaderProperty(perspective, GetLoc("sFixPerspective"));
                materialEditor.ShaderProperty(vrParallaxStrength, GetLoc("sVRParallaxStrength"));
                EditorGUI.indentLevel--;
            }
        }

        private void MatCapTextureGUI(MaterialEditor materialEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty blendUV1, MaterialProperty zRotCancel, MaterialProperty perspective, MaterialProperty vrParallaxStrength)
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
                materialEditor.ShaderProperty(blendUV1, GetLoc("sBlendUV1"));
                materialEditor.ShaderProperty(zRotCancel, GetLoc("sMatCapZRotCancel"));
                materialEditor.ShaderProperty(perspective, GetLoc("sFixPerspective"));
                materialEditor.ShaderProperty(vrParallaxStrength, GetLoc("sVRParallaxStrength"));
                EditorGUI.indentLevel--;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Bake
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

        private void AutoBakeColoredMask(Material material, MaterialProperty masktex, MaterialProperty maskcolor, string propName)
        {
            if(propName.Contains("Shadow"))
            {
                AutoBakeShadowTexture(material, (Texture2D)mainTex.textureValue, propName.Contains("2nd") ? 2 : 1);
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

            if(!String.IsNullOrEmpty(path)) path = Path.GetDirectoryName(path) + "/Baked_" + material.name + "_" + propName;
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
            TextureImporter fromTextureImporter = (TextureImporter)TextureImporter.GetAtPath(fromPath);
            TextureImporter toTextureImporter = (TextureImporter)TextureImporter.GetAtPath(toPath);
            if(fromTextureImporter == null || toTextureImporter == null) return;

            TextureImporterSettings fromTextureImporterSettings = new TextureImporterSettings();
            fromTextureImporter.ReadTextureSettings(fromTextureImporterSettings);
            toTextureImporter.SetTextureSettings(fromTextureImporterSettings);
            toTextureImporter.SetPlatformTextureSettings(fromTextureImporter.GetDefaultPlatformTextureSettings());
            AssetDatabase.ImportAsset(toPath);
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Gradient
        private void SetLinear(Texture2D tex)
        {
            if(tex != null)
            {
                string path = AssetDatabase.GetAssetPath(tex);
                TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(path);
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

        //------------------------------------------------------------------------------------------------------------------------------
        // Save Texture
        private Texture2D SaveTextureToPng(Material material, Texture2D tex, string texname, string customName = "")
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

        private Texture2D SaveTextureToPng(Material material, Texture2D tex, Texture2D origTex)
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

        private Texture2D SaveTextureToPng(Texture2D tex, string path, string customName = "")
        {
            string filename = customName + Path.GetFileNameWithoutExtension(path);
            if(!String.IsNullOrEmpty(path)) path = EditorUtility.SaveFilePanel("Save Texture", Path.GetDirectoryName(path), filename, "png");
            else                            path = EditorUtility.SaveFilePanel("Save Texture", "Assets", filename, "png");
            if(!String.IsNullOrEmpty(path))
            {
                File.WriteAllBytes(path, tex.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(tex);
                AssetDatabase.Refresh();
                return (Texture2D)AssetDatabase.LoadAssetAtPath(path.Substring(path.IndexOf("Assets")), typeof(Texture2D));
            }
            else
            {
                return tex;
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
            private void ConvertGifToAtlas(MaterialProperty tex, MaterialProperty decalAnimation, MaterialProperty decalSubParam, MaterialProperty isDecal)
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