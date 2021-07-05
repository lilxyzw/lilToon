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
        enum EditorMode
        {
            Simple,
            Advanced,
            Preset
        }

        enum RenderingMode
        {
            Opaque,
            Cutout,
            Transparent,
            Refraction,
            RefractionBlur,
            Fur,
            FurCutout
        }

        enum BlendMode
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

        //------------------------------------------------------------------------------------------------------------------------------
        // Constant
        #if UNITY_2019_1_OR_NEWER
            static Color lineColor          = new Color(0.35f,0.35f,0.35f,1.0f);
        #else
            static Color lineColor          = new Color(0.4f,0.4f,0.4f,1.0f);
        #endif

        static Vector4 defaultHSVG = new Vector4(0.0f,1.0f,1.0f,1.0f);
        const string versionInfo = "lilToon 1.0";
        const string boothURL = "https://lilxyzw.booth.pm/";
        const string githubURL = "https://github.com/lilxyzw/lilToon";
        const string twitterURL = "https://twitter.com/lil_xyzw";
        const string editorSettingPath = "Assets/lilToonSetting/EditorSetting.asset";

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
        static lilToonEditorSetting editorSetting;
        static lilToonSetting shaderSetting;
        static EditorMode editorMode = EditorMode.Simple;
        static lilToonPreset[] presets;

        static int languageNum = 0;
        static string languageNames = "";
        static string languageName = "English";
        static Dictionary<string, string> loc = new Dictionary<string, string>();

	    static bool isShowMainUV            = false;
	    static bool isShowMain              = false;
	    static bool isShowShadow            = false;
	    static bool isShowBump              = false;
	    static bool isShowReflections       = false;
	    static bool isShowEmission          = false;
            static bool isShowEmissionMap           = false;
            static bool isShowEmissionBlendMask     = false;
            static bool isShowEmission2ndMap        = false;
            static bool isShowEmission2ndBlendMask  = false;
	    static bool isShowParallax          = false;
	    static bool isShowDistanceFade      = false;
	    static bool isShowAudioLink         = false;
	    static bool isShowDissolve          = false;
            static bool isShowMain2ndDissolveMask  = false;
            static bool isShowMain2ndDissolveNoiseMask  = false;
            static bool isShowMain3rdDissolveMask  = false;
            static bool isShowMain3rdDissolveNoiseMask  = false;
            static bool isShowDissolveMask         = false;
            static bool isShowDissolveNoiseMask         = false;
	    static bool isShowStencil           = false;
	    static bool isShowOutline           = false;
            static bool isShowOutlineMap            = false;
	    static bool isShowRefraction        = false;
	    static bool isShowFur               = false;
	    static bool isShowTess              = false;
	    static bool isShowRendering         = false;
	    static bool isShowOptimization      = false;
        static bool isShowCustomProperties  = false;
	    static bool isShowBlend             = false;
	    static bool isShowBlendAdd          = false;
	    static bool isShowBlendOutline      = false;
	    static bool isShowBlendAddOutline   = false;
	    static bool isShowBlendFur          = false;
	    static bool isShowBlendAddFur       = false;
	    static bool isShowWebPages          = false;
	    static bool isShowShaderSetting     = false;
	    static bool isShaderSettingChanged  = false;
        static bool[] isShowCategorys = new bool[(int)lilPresetCategory.Other+1]{false,false,false,false,false,false,false};

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

        //------------------------------------------------------------------------------------------------------------------------------
        // Material properties
        MaterialProperty invisible;
        MaterialProperty asUnlit;
        MaterialProperty cutoff;
        MaterialProperty flipNormal;
        MaterialProperty backfaceForceShadow;
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
        MaterialProperty useRim;
            MaterialProperty rimColor;
            MaterialProperty rimColorTex;
            MaterialProperty rimBorder;
            MaterialProperty rimBlur;
            MaterialProperty rimFresnelPower;
            MaterialProperty rimEnableLighting;
            MaterialProperty rimShadowMask;
            MaterialProperty rimApplyTransparency;
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
        //MaterialProperty useRefraction;
            MaterialProperty refractionStrength;
            MaterialProperty refractionFresnelPower;
            MaterialProperty refractionColorFromMain;
            MaterialProperty refractionColor;
        //MaterialProperty useFur;
            MaterialProperty furNoiseMask;
            MaterialProperty furMask;
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

        //------------------------------------------------------------------------------------------------------------------------------
        // Emittion Gradient
        Gradient emiGrad = new Gradient();
        Gradient emi2Grad = new Gradient();

        //------------------------------------------------------------------------------------------------------------------------------
        // GUI
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
	    {
            //------------------------------------------------------------------------------------------------------------------------------
            // EditorAssets
            #if UNITY_2019_1_OR_NEWER
                GUIStyle boxOuter        = new GUIStyle(((GUISkin)AssetDatabase.LoadAssetAtPath("Assets/lilToon/Editor/gui_box_outer_2019.guiskin", typeof(GUISkin))).box);
                GUIStyle boxInnerHalf    = new GUIStyle(((GUISkin)AssetDatabase.LoadAssetAtPath("Assets/lilToon/Editor/gui_box_inner_half_2019.guiskin", typeof(GUISkin))).box);
                GUIStyle boxInner        = new GUIStyle(((GUISkin)AssetDatabase.LoadAssetAtPath("Assets/lilToon/Editor/gui_box_inner_2019.guiskin", typeof(GUISkin))).box);
                GUIStyle customBox       = new GUIStyle(((GUISkin)AssetDatabase.LoadAssetAtPath("Assets/lilToon/Editor/gui_custom_box_2019.guiskin", typeof(GUISkin))).box);
                GUIStyle customToggleFont = EditorStyles.label;
                GUIStyle offsetButton = new GUIStyle(GUI.skin.button);
                offsetButton.margin.left = 24;
            #else
                GUIStyle boxOuter        = new GUIStyle(((GUISkin)AssetDatabase.LoadAssetAtPath("Assets/lilToon/Editor/gui_box_outer_2018.guiskin", typeof(GUISkin))).box);
                GUIStyle boxInnerHalf    = new GUIStyle(((GUISkin)AssetDatabase.LoadAssetAtPath("Assets/lilToon/Editor/gui_box_inner_half_2018.guiskin", typeof(GUISkin))).box);
                GUIStyle boxInner        = new GUIStyle(((GUISkin)AssetDatabase.LoadAssetAtPath("Assets/lilToon/Editor/gui_box_inner_2018.guiskin", typeof(GUISkin))).box);
                GUIStyle customBox       = GUI.skin.box;
                GUIStyle customToggleFont = new GUIStyle();
                customToggleFont.normal.textColor = Color.white;
                customToggleFont.contentOffset = new Vector2(2f,0f);
                GUIStyle offsetButton = new GUIStyle(GUI.skin.button);
                offsetButton.margin.left = 20;
            #endif

            //------------------------------------------------------------------------------------------------------------------------------
            // Initialize Setting
            InitializeEditorSetting();
            InitializeShaderSetting();

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

            isCustomShader = material.shader.name.Contains("Overlay");

                                renderingModeBuf = RenderingMode.Opaque;
            if(isCutout)        renderingModeBuf = RenderingMode.Cutout;
            if(isTransparent)   renderingModeBuf = RenderingMode.Transparent;
            if(isRefr)          renderingModeBuf = RenderingMode.Refraction;
            if(isRefr&isBlur)   renderingModeBuf = RenderingMode.RefractionBlur;
            if(isFur)           renderingModeBuf = RenderingMode.Fur;
            if(isFur&isCutout)  renderingModeBuf = RenderingMode.FurCutout;

            //------------------------------------------------------------------------------------------------------------------------------
            // Load Properties
            if(isLite)      LoadLiteProperties(props);
            else if(isFur)  LoadFurProperties(props);
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
            languageNum = selectLang(languageNum);
            string sCullModes = loc["sCullMode"] + "|" + loc["sCullModeOff"] + "|" + loc["sCullModeFront"] + "|" + loc["sCullModeBack"];
            string sBlendModes = loc["sBlendMode"] + "|" + loc["sBlendModeNormal"] + "|" + loc["sBlendModeAdd"] + "|" + loc["sBlendModeScreen"] + "|" + loc["sBlendModeMul"];
            string blinkSetting = loc["sBlinkStrength"] + "|" + loc["sBlinkType"] + "|" + loc["sBlinkSpeed"] + "|" + loc["sBlinkOffset"];
            string sDissolveParams = loc["sDissolveMode"] + "|" + loc["sDissolveModeNone"] + "|" + loc["sDissolveModeAlpha"] + "|" + loc["sDissolveModeUV"] + "|" + loc["sDissolveModePosition"] + "|" + loc["sDissolveShape"] + "|" + loc["sDissolveShapePoint"] + "|" + loc["sDissolveShapeLine"] + "|" + loc["sBorder"] + "|" + loc["sBlur"];
            string sDissolveParamsMode = loc["sDissolve"] + "|" + loc["sDissolveModeNone"] + "|" + loc["sDissolveModeAlpha"] + "|" + loc["sDissolveModeUV"] + "|" + loc["sDissolveModePosition"];
            string sDissolveParamsOther = loc["sDissolveShape"] + "|" + loc["sDissolveShapePoint"] + "|" + loc["sDissolveShapeLine"] + "|" + loc["sBorder"] + "|" + loc["sBlur"] + "|Dummy";
            string[] sRenderingModeList = {loc["sRenderingModeOpaque"], loc["sRenderingModeCutout"], loc["sRenderingModeTransparent"], loc["sRenderingModeRefraction"], loc["sRenderingModeRefractionBlur"], loc["sRenderingModeFur"], loc["sRenderingModeFurCutout"]};
            string[] sRenderingModeListLite = {loc["sRenderingModeOpaque"], loc["sRenderingModeCutout"], loc["sRenderingModeTransparent"]};
            GUIContent textureRGBAContent = new GUIContent(loc["sTexture"], loc["sTextureRGBA"]);
            GUIContent colorRGBAContent = new GUIContent(loc["sColor"], loc["sTextureRGBA"]);
            GUIContent maskBlendContent = new GUIContent(loc["sMask"], loc["sBlendR"]);
            GUIContent maskStrengthContent = new GUIContent(loc["sStrengthMask"], loc["sStrengthR"]);
            GUIContent normalMapContent = new GUIContent(loc["sNormalMap"], loc["sNormalRGB"]);
            GUIContent triMaskContent = new GUIContent(loc["sTriMask"], loc["sTriMaskRGB"]);
            GUIContent noiseMaskContent = new GUIContent(loc["sNoise"], loc["sNoiseR"]);

            //------------------------------------------------------------------------------------------------------------------------------
            // Editor Mode
            SelectEditorMode();
            EditorGUILayout.Space();

            //------------------------------------------------------------------------------------------------------------------------------
            // Simple
            if(editorMode == EditorMode.Simple)
            {
                //------------------------------------------------------------------------------------------------------------------------------
                // Base Setting
                EditorGUILayout.BeginVertical(boxOuter);
                EditorGUILayout.LabelField(loc["sBaseSetting"], customToggleFont);
                EditorGUILayout.BeginVertical(boxInnerHalf);
                materialEditor.ShaderProperty(invisible, loc["sInvisible"]);
                {
                    if(!isCustomShader)
                    {
                        RenderingMode renderingMode;
                        if(isLite) renderingMode = (RenderingMode)EditorGUILayout.Popup(loc["sRenderingMode"], (int)renderingModeBuf, sRenderingModeListLite);
                        else       renderingMode = (RenderingMode)EditorGUILayout.Popup(loc["sRenderingMode"], (int)renderingModeBuf, sRenderingModeList);
                        if(renderingModeBuf != renderingMode)
                        {
                            SetupMaterialWithRenderingMode(material, renderingMode, isOutl, isLite, isStWr, isTess);
                            if(renderingMode == RenderingMode.Cutout || renderingMode == RenderingMode.FurCutout) cutoff.floatValue = 0.5f;
                            if(renderingMode == RenderingMode.Transparent || renderingMode == RenderingMode.Fur) cutoff.floatValue = 0.001f;
                        }
                    }
                    if(renderingModeBuf == RenderingMode.Cutout || renderingModeBuf == RenderingMode.Transparent || renderingModeBuf == RenderingMode.Fur || renderingModeBuf == RenderingMode.FurCutout)
                    {
                        materialEditor.ShaderProperty(cutoff, loc["sCutoff"]);
                    }
                    if(renderingModeBuf >= RenderingMode.Transparent && renderingModeBuf != RenderingMode.FurCutout)
                    {
                        EditorGUILayout.HelpBox(loc["sHelpRenderingTransparent"],MessageType.Warning);
                    }
                    materialEditor.ShaderProperty(cull, sCullModes);
                    if(cull.floatValue == 1.0f)
                    {
                        EditorGUILayout.HelpBox(loc["sHelpCullMode"],MessageType.Warning);
                    }
                    materialEditor.ShaderProperty(zwrite, loc["sZWrite"]);
                    if(zwrite.floatValue != 1.0f)
                    {
                        EditorGUILayout.HelpBox(loc["sHelpZWrite"],MessageType.Warning);
                    }
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
                        EditorGUILayout.LabelField(loc["sMainColor"], customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TexturePropertySingleLine(textureRGBAContent, mainTex, mainColor);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    //}

                    // Shadow
                    EditorGUILayout.BeginVertical(boxOuter);
                    materialEditor.ShaderProperty(useShadow, loc["sShadow"]);
                    if(useShadow.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sShadow1stColor"], loc["sTextureRGBA"]), shadowColorTex);
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();

                    // Outline
                    EditorGUILayout.BeginVertical(boxOuter);
                    if(isOutl != EditorGUILayout.ToggleLeft(loc["sOutline"], isOutl, customToggleFont))
                    {
                        SetupMaterialWithRenderingMode(material, renderingModeBuf, !isOutl, isLite, isStWr, isTess);
                    }
                    if(isOutl)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TexturePropertySingleLine(textureRGBAContent, outlineTex, outlineColor);
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sWidth"], loc["sWidthR"]), outlineWidthMask, outlineWidth);
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();

                    // Emission
                    EditorGUILayout.BeginVertical(boxOuter);
                    materialEditor.ShaderProperty(useEmission, loc["sEmission"]);
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
                        EditorGUILayout.LabelField(loc["sMainColor"], customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TexturePropertySingleLine(textureRGBAContent, mainTex, mainColor);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    //}

                    // Shadow
                    if(shaderSetting.LIL_FEATURE_SHADOW)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        materialEditor.ShaderProperty(useShadow, loc["sShadow"]);
                        if(useShadow.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            if(shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH)   materialEditor.TexturePropertySingleLine(maskStrengthContent, shadowStrengthMask, shadowStrength);
                            else                                                materialEditor.ShaderProperty(shadowStrength, loc["sStrength"]);
                            if(shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST)    materialEditor.TexturePropertySingleLine(new GUIContent(loc["sShadow1stColor"], loc["sTextureRGBA"]), shadowColorTex, shadowColor);
                            else                                            materialEditor.ShaderProperty(shadowColor, loc["sShadow1stColor"]);
                            if(shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND)    materialEditor.TexturePropertySingleLine(new GUIContent(loc["sShadow2ndColor"], loc["sTextureRGBA"]), shadow2ndColorTex, shadow2ndColor);
                            else                                            materialEditor.ShaderProperty(shadow2ndColor, loc["sShadow2ndColor"]);
                            materialEditor.ShaderProperty(shadowMainStrength, loc["sMainColorPower"]);
                            materialEditor.ShaderProperty(shadowBorderColor, loc["sShadowBorderColor"]);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Normal
                else
                {
                    // Main
                    //if(useMainTex.floatValue == 1)
                    //{
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(loc["sMainColor"], customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TexturePropertySingleLine(textureRGBAContent, mainTex, mainColor);
                        ToneCorrectionGUI(materialEditor, material, mainTex, mainColor, mainTexHSVG);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    //}

                    // Main 2nd
                    if(shaderSetting.LIL_FEATURE_MAIN2ND && useMain2ndTex.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(loc["sMainColor2nd"], customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TexturePropertySingleLine(textureRGBAContent, main2ndTex, mainColor2nd);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    // Main 3rd
                    if(shaderSetting.LIL_FEATURE_MAIN3RD && useMain3rdTex.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(loc["sMainColor3rd"], customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TexturePropertySingleLine(textureRGBAContent, main3rdTex, mainColor3rd);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    // Shadow
                    if(shaderSetting.LIL_FEATURE_SHADOW)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        materialEditor.ShaderProperty(useShadow, loc["sShadow"]);
                        if(useShadow.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            if(shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH)   materialEditor.TexturePropertySingleLine(maskStrengthContent, shadowStrengthMask, shadowStrength);
                            else                                                materialEditor.ShaderProperty(shadowStrength, loc["sStrength"]);
                            if(shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST)    materialEditor.TexturePropertySingleLine(new GUIContent(loc["sShadow1stColor"], loc["sTextureRGBA"]), shadowColorTex, shadowColor);
                            else                                            materialEditor.ShaderProperty(shadowColor, loc["sShadow1stColor"]);
                            if(shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND)    materialEditor.TexturePropertySingleLine(new GUIContent(loc["sShadow2ndColor"], loc["sTextureRGBA"]), shadow2ndColorTex, shadow2ndColor);
                            else                                            materialEditor.ShaderProperty(shadow2ndColor, loc["sShadow2ndColor"]);
                            materialEditor.ShaderProperty(shadowMainStrength, loc["sMainColorPower"]);
                            materialEditor.ShaderProperty(shadowBorderColor, loc["sShadowBorderColor"]);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }

                    // Outline
                    if(!isRefr & !isFur)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        if(isOutl != EditorGUILayout.ToggleLeft(loc["sOutline"], isOutl, customToggleFont))
                        {
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, !isOutl, isLite, isStWr, isTess);
                        }
                        if(isOutl)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            if(shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR) materialEditor.TexturePropertySingleLine(textureRGBAContent, outlineTex, outlineColor);
                            else                                            materialEditor.ShaderProperty(outlineColor, loc["sColor"]);
                            if(shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION) ToneCorrectionGUI(materialEditor, material, outlineTex, outlineColor, outlineTexHSVG, true);
                            if(shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH) materialEditor.TexturePropertySingleLine(new GUIContent(loc["sWidth"], loc["sWidthR"]), outlineWidthMask, outlineWidth);
                            else                                            materialEditor.ShaderProperty(outlineWidth, loc["sWidth"]);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }

                    // Emission
                    if(shaderSetting.LIL_FEATURE_EMISSION_1ST)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        materialEditor.ShaderProperty(useEmission, loc["sEmission"]);
                        if(useEmission.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            materialEditor.TexturePropertySingleLine(textureRGBAContent, emissionMap, emissionColor);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }

                    // Emission 2nd
                    if(shaderSetting.LIL_FEATURE_EMISSION_2ND)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        materialEditor.ShaderProperty(useEmission2nd, loc["sEmission2nd"]);
                        if(useEmission2nd.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            materialEditor.TexturePropertySingleLine(textureRGBAContent, emission2ndMap, emission2ndColor);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                if(mtoon != null && GUILayout.Button(loc["sConvertMToon"], offsetButton)) CreateMToonMaterial(material);
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Advanced
            if(editorMode == EditorMode.Advanced)
            {
                //------------------------------------------------------------------------------------------------------------------------------
                // Base Setting
                GUILayout.Label(" " + loc["sBaseSetting"], EditorStyles.boldLabel);
                DrawHelpButton(loc["sAnchorBaseSetting"]);
                EditorGUILayout.BeginVertical(customBox);
                {
                    materialEditor.ShaderProperty(invisible, loc["sInvisible"]);
                    materialEditor.ShaderProperty(asUnlit, loc["sAsUnlit"]);
                    if(!isCustomShader)
                    {
                        RenderingMode renderingMode;
                        if(isLite) renderingMode = (RenderingMode)EditorGUILayout.Popup(loc["sRenderingMode"], (int)renderingModeBuf, sRenderingModeListLite);
                        else       renderingMode = (RenderingMode)EditorGUILayout.Popup(loc["sRenderingMode"], (int)renderingModeBuf, sRenderingModeList);
                        if(renderingModeBuf != renderingMode)
                        {
                            SetupMaterialWithRenderingMode(material, renderingMode, isOutl, isLite, isStWr, isTess);
                            if(renderingMode == RenderingMode.Cutout || renderingMode == RenderingMode.FurCutout) cutoff.floatValue = 0.5f;
                            if(renderingMode == RenderingMode.Transparent || renderingMode == RenderingMode.Fur) cutoff.floatValue = 0.001f;
                        }
                    }
                    if(renderingModeBuf == RenderingMode.Cutout || renderingModeBuf == RenderingMode.Transparent || renderingModeBuf == RenderingMode.Fur || renderingModeBuf == RenderingMode.FurCutout)
                    {
                        materialEditor.ShaderProperty(cutoff, loc["sCutoff"]);
                    }
                    if(renderingModeBuf >= RenderingMode.Transparent && renderingModeBuf != RenderingMode.FurCutout)
                    {
                        EditorGUILayout.HelpBox(loc["sHelpRenderingTransparent"],MessageType.Warning);
                    }
                    materialEditor.ShaderProperty(cull, sCullModes);
                    if(cull.floatValue == 1.0f)
                    {
                        EditorGUILayout.HelpBox(loc["sHelpCullMode"],MessageType.Warning);
                    }
                    if(cull.floatValue <= 1.0f)
                    {
                        materialEditor.ShaderProperty(flipNormal, loc["sFlipBackfaceNormal"]);
                        materialEditor.ShaderProperty(backfaceForceShadow, loc["sBackfaceForceShadow"]);
                    }
                    materialEditor.ShaderProperty(zwrite, loc["sZWrite"]);
                    if(zwrite.floatValue != 1.0f)
                    {
                        EditorGUILayout.HelpBox(loc["sHelpZWrite"],MessageType.Warning);
                    }
                    if(isLite) 
                    {
                        DrawLine();
                        materialEditor.TexturePropertySingleLine(triMaskContent, triMask);
                    }
                }
                EditorGUILayout.EndVertical();

                //------------------------------------------------------------------------------------------------------------------------------
                // UV
                isShowMainUV = Foldout(loc["sMainUV"], loc["sMainUVTips"], isShowMainUV);
                DrawHelpButton(loc["sAnchorUVSetting"]);
                if(isShowMainUV)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    EditorGUILayout.LabelField(loc["sMainUV"], customToggleFont);
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    if(shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV)   UVSettingGUI(materialEditor, mainTex, mainTex_ScrollRotate);
                    else                                            materialEditor.TextureScaleOffsetProperty(mainTex);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.Space();

                //------------------------------------------------------------------------------------------------------------------------------
                // Colors
                GUILayout.Label(" " + loc["sColors"], EditorStyles.boldLabel);

                //------------------------------------------------------------------------------------------------------------------------------
                // Main Color
                isShowMain = Foldout(loc["sMainColorSetting"], loc["sMainColorTips"], isShowMain);
                DrawHelpButton(loc["sAnchorMainColor"]);
                if(isShowMain)
                {
                    if(isLite)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Main
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(loc["sMainColor"], customToggleFont);
                        DrawHelpButton(loc["sAnchorMainColor1"]);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.TexturePropertySingleLine(textureRGBAContent, mainTex, mainColor);
                        AlphamaskToTextureGUI(material);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                    else if(isFur)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Main
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(loc["sMainColor"], customToggleFont);
                        DrawHelpButton(loc["sAnchorMainColor1"]);
                        //materialEditor.ShaderProperty(useMainTex, loc["sMainColor"]);
                        //if(useMainTex.floatValue == 1)
                        //{
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            materialEditor.TexturePropertySingleLine(textureRGBAContent, mainTex, mainColor);
                            AlphamaskToTextureGUI(material);
                            EditorGUILayout.EndVertical();
                        //}
                        EditorGUILayout.EndVertical();
                    }
                    else
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Main
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(loc["sMainColor"], customToggleFont);
                        DrawHelpButton(loc["sAnchorMainColor1"]);
                        //materialEditor.ShaderProperty(useMainTex, loc["sMainColor"]);
                        //if(useMainTex.floatValue == 1)
                        //{
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            materialEditor.TexturePropertySingleLine(textureRGBAContent, mainTex, mainColor);
                            if(isCutout || isTransparent)
                            {
                                SetAlphaIsTransparencyGUI(mainTex);
                                AlphamaskToTextureGUI(material);
                            }
                            if(shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION)
                            {
                                ToneCorrectionGUI(materialEditor, material, mainTex, mainColor, mainTexHSVG);
                            }
                            EditorGUILayout.EndVertical();
                        //}
                        EditorGUILayout.EndVertical();

                        //------------------------------------------------------------------------------------------------------------------------------
                        // 2nd
                        if(shaderSetting.LIL_FEATURE_MAIN2ND)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            materialEditor.ShaderProperty(useMain2ndTex, loc["sMainColor2nd"]);
                            DrawHelpButton(loc["sAnchorMainColor2"]);
                            if(useMain2ndTex.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                materialEditor.TexturePropertySingleLine(textureRGBAContent, main2ndTex, mainColor2nd);
                                materialEditor.ShaderProperty(main2ndTexIsMSDF, loc["sAsMSDF"]);
                                DrawLine();
                                UV4Decal(materialEditor, main2ndTexIsDecal, main2ndTexIsLeftOnly, main2ndTexIsRightOnly, main2ndTexShouldCopy, main2ndTexShouldFlipMirror, main2ndTexShouldFlipCopy, main2ndTex, main2ndTexAngle, main2ndTexDecalAnimation, main2ndTexDecalSubParam);
                                DrawLine();
                                if(shaderSetting.LIL_FEATURE_TEX_LAYER_MASK) materialEditor.TexturePropertySingleLine(maskBlendContent, main2ndBlendMask);
                                materialEditor.ShaderProperty(main2ndEnableLighting, loc["sEnableLighting"]);
                                materialEditor.ShaderProperty(main2ndTexBlendMode, sBlendModes);
                                if(shaderSetting.LIL_FEATURE_LAYER_DISSOLVE)
                                {
                                    DrawLine();
                                    materialEditor.ShaderProperty(main2ndDissolveParams, sDissolveParams);
                                    if(main2ndDissolveParams.vectorValue.x == 1.0f)                                                TextureGUI(materialEditor, ref isShowMain2ndDissolveMask, maskBlendContent, main2ndDissolveMask);
                                    if(main2ndDissolveParams.vectorValue.x == 2.0f && main2ndDissolveParams.vectorValue.y == 0.0f) materialEditor.ShaderProperty(main2ndDissolvePos, loc["sPosition"] + "|2");
                                    if(main2ndDissolveParams.vectorValue.x == 2.0f && main2ndDissolveParams.vectorValue.y == 1.0f) materialEditor.ShaderProperty(main2ndDissolvePos, loc["sVector"] + "|2");
                                    if(main2ndDissolveParams.vectorValue.x == 3.0f && main2ndDissolveParams.vectorValue.y == 0.0f) materialEditor.ShaderProperty(main2ndDissolvePos, loc["sPosition"] + "|3");
                                    if(main2ndDissolveParams.vectorValue.x == 3.0f && main2ndDissolveParams.vectorValue.y == 1.0f) materialEditor.ShaderProperty(main2ndDissolvePos, loc["sVector"] + "|3");
                                    if(main2ndDissolveParams.vectorValue.x != 0.0f)
                                    {
                                        if(shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE) TextureGUI(materialEditor, ref isShowMain2ndDissolveNoiseMask, noiseMaskContent, main2ndDissolveNoiseMask, main2ndDissolveNoiseStrength, main2ndDissolveNoiseMask_ScrollRotate);
                                        materialEditor.ShaderProperty(main2ndDissolveColor, loc["sColor"]);
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
                            materialEditor.ShaderProperty(useMain3rdTex, loc["sMainColor3rd"]);
                            DrawHelpButton(loc["sAnchorMainColor2"]);
                            if(useMain3rdTex.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                materialEditor.TexturePropertySingleLine(textureRGBAContent, main3rdTex, mainColor3rd);
                                materialEditor.ShaderProperty(main3rdTexIsMSDF, loc["sAsMSDF"]);
                                DrawLine();
                                UV4Decal(materialEditor, main3rdTexIsDecal, main3rdTexIsLeftOnly, main3rdTexIsRightOnly, main3rdTexShouldCopy, main3rdTexShouldFlipMirror, main3rdTexShouldFlipCopy, main3rdTex, main3rdTexAngle, main3rdTexDecalAnimation, main3rdTexDecalSubParam);
                                DrawLine();
                                if(shaderSetting.LIL_FEATURE_TEX_LAYER_MASK) materialEditor.TexturePropertySingleLine(maskBlendContent, main3rdBlendMask);
                                materialEditor.ShaderProperty(main3rdEnableLighting, loc["sEnableLighting"]);
                                materialEditor.ShaderProperty(main3rdTexBlendMode, sBlendModes);
                                if(shaderSetting.LIL_FEATURE_LAYER_DISSOLVE)
                                {
                                    DrawLine();
                                    materialEditor.ShaderProperty(main3rdDissolveParams, sDissolveParams);
                                    if(main3rdDissolveParams.vectorValue.x == 1.0f)                                                TextureGUI(materialEditor, ref isShowMain3rdDissolveMask, maskBlendContent, main3rdDissolveMask);
                                    if(main3rdDissolveParams.vectorValue.x == 2.0f && main3rdDissolveParams.vectorValue.y == 0.0f) materialEditor.ShaderProperty(main3rdDissolvePos, loc["sPosition"] + "|2");
                                    if(main3rdDissolveParams.vectorValue.x == 2.0f && main3rdDissolveParams.vectorValue.y == 1.0f) materialEditor.ShaderProperty(main3rdDissolvePos, loc["sVector"] + "|2");
                                    if(main3rdDissolveParams.vectorValue.x == 3.0f && main3rdDissolveParams.vectorValue.y == 0.0f) materialEditor.ShaderProperty(main3rdDissolvePos, loc["sPosition"] + "|3");
                                    if(main3rdDissolveParams.vectorValue.x == 3.0f && main3rdDissolveParams.vectorValue.y == 1.0f) materialEditor.ShaderProperty(main3rdDissolvePos, loc["sVector"] + "|3");
                                    if(main3rdDissolveParams.vectorValue.x != 0.0f)
                                    {
                                        if(shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE) TextureGUI(materialEditor, ref isShowMain3rdDissolveNoiseMask, noiseMaskContent, main3rdDissolveNoiseMask, main3rdDissolveNoiseStrength, main3rdDissolveNoiseMask_ScrollRotate);
                                        materialEditor.ShaderProperty(main3rdDissolveColor, loc["sColor"]);
                                    }
                                }
                                DrawLine();
                                TextureBakeGUI(material, 6);
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Shadow
                if(shaderSetting.LIL_FEATURE_SHADOW)
                {
                    isShowShadow = Foldout(loc["sShadowSetting"], loc["sShadowTips"], isShowShadow);
                    DrawHelpButton(loc["sAnchorShadow"]);
                    if(isShowShadow)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        materialEditor.ShaderProperty(useShadow, loc["sShadow"]);
                        if(useShadow.floatValue == 1)
                        {
                            if(isLite)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                materialEditor.ShaderProperty(shadowBorder, loc["sBorder"]);
                                materialEditor.ShaderProperty(shadowBlur, loc["sBlur"]);
                                materialEditor.TexturePropertySingleLine(new GUIContent(loc["sShadow1stColor"], loc["sTextureRGBA"]), shadowColorTex);
                                materialEditor.ShaderProperty(shadowEnvStrength, loc["sShadowEnvStrength"]);
                                EditorGUILayout.EndVertical();
                            }
                            else
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                if(shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER) materialEditor.TexturePropertySingleLine(new GUIContent(loc["sBorderAO"], loc["sBorderR"]), shadowBorderMask, shadowBorder);
                                else                                            materialEditor.ShaderProperty(shadowBorder, loc["sBorder"]);
                                if(shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR)   materialEditor.TexturePropertySingleLine(new GUIContent(loc["sBlur"], loc["sBlurR"]), shadowBlurMask, shadowBlur);
                                else                                            materialEditor.ShaderProperty(shadowBlur, loc["sBlur"]);
                                if(shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH)   materialEditor.TexturePropertySingleLine(maskStrengthContent, shadowStrengthMask, shadowStrength);
                                else                                                materialEditor.ShaderProperty(shadowStrength, loc["sStrength"]);
                                DrawLine();
                                if(shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST)    materialEditor.TexturePropertySingleLine(new GUIContent(loc["sShadow1stColor"], loc["sTextureRGBA"]), shadowColorTex, shadowColor);
                                else                                            materialEditor.ShaderProperty(shadowColor, loc["sShadow1stColor"]);
                                DrawLine();
                                if(shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND)    materialEditor.TexturePropertySingleLine(new GUIContent(loc["sShadow2ndColor"], loc["sTextureRGBA"]), shadow2ndColorTex, shadow2ndColor);
                                else                                            materialEditor.ShaderProperty(shadow2ndColor, loc["sShadow2ndColor"]);
                                materialEditor.ShaderProperty(shadow2ndBorder, loc["sBorder"]);
                                materialEditor.ShaderProperty(shadow2ndBlur, loc["sBlur"]);
                                DrawLine();
                                materialEditor.ShaderProperty(shadowMainStrength, loc["sMainColorPower"]);
                                materialEditor.ShaderProperty(shadowEnvStrength, loc["sShadowEnvStrength"]);
                                materialEditor.ShaderProperty(shadowBorderColor, loc["sShadowBorderColor"]);
                                materialEditor.ShaderProperty(shadowBorderRange, loc["sShadowBorderRange"]);
                                if(shaderSetting.LIL_FEATURE_RECEIVE_SHADOW) materialEditor.ShaderProperty(shadowReceive, loc["sReceiveShadow"]);
                                EditorGUILayout.EndVertical();
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Emission / Normal / Reflection
                if(!isFur)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // Emission
                    if(shaderSetting.LIL_FEATURE_EMISSION_1ST || shaderSetting.LIL_FEATURE_EMISSION_2ND)
                    {
                        isShowEmission = Foldout(loc["sEmissionSetting"], loc["sEmissionTips"], isShowEmission);
                        DrawHelpButton(loc["sAnchorEmission"]);
                        if(isShowEmission)
                        {
                            if(isLite)
                            {
                                // Emission
                                EditorGUILayout.BeginVertical(boxOuter);
                                materialEditor.ShaderProperty(useEmission, loc["sEmission"]);
                                if(useEmission.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    TextureGUI(materialEditor, ref isShowEmissionMap, colorRGBAContent, emissionMap, emissionColor, emissionMap_ScrollRotate, true, true);
                                    DrawLine();
                                    materialEditor.ShaderProperty(emissionBlink, blinkSetting);
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndVertical();
                            }
                            else
                            {
                                // Emission
                                if(shaderSetting.LIL_FEATURE_EMISSION_1ST)
                                {
                                    EditorGUILayout.BeginVertical(boxOuter);
                                    materialEditor.ShaderProperty(useEmission, loc["sEmission"]);
                                    if(useEmission.floatValue == 1)
                                    {
                                        EditorGUILayout.BeginVertical(boxInnerHalf);
                                        TextureGUI(materialEditor, ref isShowEmissionMap, colorRGBAContent, emissionMap, emissionColor, emissionMap_ScrollRotate, shaderSetting.LIL_FEATURE_EMISSION_UV, shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV);
                                        DrawLine();
                                        if(shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK)
                                        {
                                            TextureGUI(materialEditor, ref isShowEmissionBlendMask, maskBlendContent, emissionBlendMask, emissionBlend, emissionBlendMask_ScrollRotate, shaderSetting.LIL_FEATURE_EMISSION_MASK_UV, shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV);
                                            DrawLine();
                                        }
                                        materialEditor.ShaderProperty(emissionBlink, blinkSetting);
                                        DrawLine();
                                        if(shaderSetting.LIL_FEATURE_EMISSION_GRADATION)
                                        {
                                            materialEditor.ShaderProperty(emissionUseGrad, loc["sGradation"]);
                                            if(emissionUseGrad.floatValue == 1)
                                            {
                                                materialEditor.TexturePropertySingleLine(new GUIContent(loc["sGradTexSpeed"], loc["sTextureRGBA"]), emissionGradTex, emissionGradSpeed);
                                                GradientEditor(material, "_eg", emiGrad, "_EmissionGradTex");
                                            }
                                            DrawLine();
                                        }
                                        materialEditor.ShaderProperty(emissionParallaxDepth, loc["sParallaxDepth"]);
                                        materialEditor.ShaderProperty(emissionFluorescence, loc["sFluorescence"]);
                                        EditorGUILayout.EndVertical();
                                    }
                                    EditorGUILayout.EndVertical();
                                }

                                // Emission 2nd
                                if(shaderSetting.LIL_FEATURE_EMISSION_2ND)
                                {
                                    EditorGUILayout.BeginVertical(boxOuter);
                                    materialEditor.ShaderProperty(useEmission2nd, loc["sEmission2nd"]);
                                    if(useEmission2nd.floatValue == 1)
                                    {
                                        EditorGUILayout.BeginVertical(boxInnerHalf);
                                        TextureGUI(materialEditor, ref isShowEmission2ndMap, colorRGBAContent, emission2ndMap, emission2ndColor, emission2ndMap_ScrollRotate, shaderSetting.LIL_FEATURE_EMISSION_UV, shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV);
                                        DrawLine();
                                        if(shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK)
                                        {
                                            TextureGUI(materialEditor, ref isShowEmission2ndBlendMask, maskBlendContent, emission2ndBlendMask, emission2ndBlend, emission2ndBlendMask_ScrollRotate, shaderSetting.LIL_FEATURE_EMISSION_MASK_UV, shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV);
                                            DrawLine();
                                        }
                                        materialEditor.ShaderProperty(emission2ndBlink, blinkSetting);
                                        DrawLine();
                                        if(shaderSetting.LIL_FEATURE_EMISSION_GRADATION)
                                        {
                                            materialEditor.ShaderProperty(emission2ndUseGrad, loc["sGradation"]);
                                            if(emission2ndUseGrad.floatValue == 1)
                                            {
                                                materialEditor.TexturePropertySingleLine(new GUIContent(loc["sGradTexSpeed"], loc["sTextureRGBA"]), emission2ndGradTex, emission2ndGradSpeed);
                                                GradientEditor(material, "_e2g", emi2Grad, "_Emission2ndGradTex");
                                            }
                                            DrawLine();
                                        }
                                        materialEditor.ShaderProperty(emission2ndParallaxDepth, loc["sParallaxDepth"]);
                                        materialEditor.ShaderProperty(emission2ndFluorescence, loc["sFluorescence"]);
                                        EditorGUILayout.EndVertical();
                                    }
                                    EditorGUILayout.EndVertical();
                                }
                            }
                        }

                        EditorGUILayout.Space();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Normal & Reflection
                    GUILayout.Label(" " + loc["sNormalMapReflection"], EditorStyles.boldLabel);

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Normal
                    if((shaderSetting.LIL_FEATURE_NORMAL_1ST || shaderSetting.LIL_FEATURE_NORMAL_2ND) && !isLite)
                    {
                        isShowBump = Foldout(loc["sNormalMapSetting"], loc["sNormalMapTips"], isShowBump);
                        DrawHelpButton(loc["sAnchorNormalMap"]);
                        if(isShowBump)
                        {
                            //------------------------------------------------------------------------------------------------------------------------------
                            // 1st
                            if(shaderSetting.LIL_FEATURE_NORMAL_1ST)
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                materialEditor.ShaderProperty(useBumpMap, loc["sNormalMap"]);
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
                                materialEditor.ShaderProperty(useBump2ndMap, loc["sNormalMap2nd"]);
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
                    isShowReflections = Foldout(loc["sReflectionsSetting"], loc["sReflectionsTips"], isShowReflections);
                    DrawHelpButton(loc["sAnchorReflections"]);
                    if(isShowReflections)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Reflection
                        if(shaderSetting.LIL_FEATURE_REFLECTION && !isLite)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            materialEditor.ShaderProperty(useReflection, loc["sReflection"]);
                            DrawHelpButton(loc["sAnchorReflection"]);
                            if(useReflection.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                if(shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS) materialEditor.TexturePropertySingleLine(new GUIContent(loc["sSmoothness"], loc["sSmoothnessR"]), smoothnessTex, smoothness);
                                else                                                    materialEditor.ShaderProperty(smoothness, loc["sSmoothness"]);
                                if(shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC) materialEditor.TexturePropertySingleLine(new GUIContent(loc["sMetallic"], loc["sMetallicR"]), metallicGlossMap, metallic);
                                else                                                  materialEditor.ShaderProperty(metallic, loc["sMetallic"]);
                                materialEditor.ShaderProperty(reflectance, loc["sReflectance"]);
                                if(shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR) materialEditor.TexturePropertySingleLine(colorRGBAContent, reflectionColorTex, reflectionColor);
                                else                                               materialEditor.ShaderProperty(reflectionColor, loc["sColor"]);
                                int specularMode = 0;
                                if(specularToon.floatValue == 0.0f) specularMode = 1;
                                if(specularToon.floatValue == 1.0f) specularMode = 2;
                                if(applySpecular.floatValue == 0.0f) specularMode = 0;
                                specularMode = EditorGUILayout.Popup(loc["sSpecularMode"],specularMode,new String[]{loc["sSpecularNone"],loc["sSpecularReal"],loc["sSpecularToon"]});
                                if(specularMode == 0) {applySpecular.floatValue = 0.0f;}
                                if(specularMode == 1) {applySpecular.floatValue = 1.0f; specularToon.floatValue = 0.0f;}
                                if(specularMode == 2) {applySpecular.floatValue = 1.0f; specularToon.floatValue = 1.0f;}
                                materialEditor.ShaderProperty(applyReflection, loc["sApplyReflection"]);
                                if(isTransparent) materialEditor.ShaderProperty(reflectionApplyTransparency, loc["sApplyTransparency"]);
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // MatCap
                        if(shaderSetting.LIL_FEATURE_MATCAP)
                        {
                            if(isLite)
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                materialEditor.ShaderProperty(useMatCap, loc["sMatCap"]);
                                DrawHelpButton(loc["sAnchorMatCap"]);
                                if(useMatCap.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    materialEditor.TexturePropertySingleLine(new GUIContent(loc["sMatCap"], loc["sTextureRGBA"]), matcapTex);
                                    materialEditor.ShaderProperty(matcapMul, loc["sBlendModeMul"]);
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndVertical();
                            }
                            else
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                materialEditor.ShaderProperty(useMatCap, loc["sMatCap"]);
                                DrawHelpButton(loc["sAnchorMatCap"]);
                                if(useMatCap.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    materialEditor.TexturePropertySingleLine(new GUIContent(loc["sMatCap"], loc["sTextureRGBA"]), matcapTex, matcapColor);
                                    if(shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK)   materialEditor.TexturePropertySingleLine(maskBlendContent, matcapBlendMask, matcapBlend);
                                    else                                            materialEditor.ShaderProperty(matcapBlend, loc["sBlend"]);
                                    materialEditor.ShaderProperty(matcapEnableLighting, loc["sEnableLighting"]);
                                    materialEditor.ShaderProperty(matcapBlendMode, sBlendModes);
                                    if(matcapEnableLighting.floatValue == 1.0f && matcapBlendMode.floatValue == 3.0f)
                                    {
                                        EditorGUILayout.HelpBox(loc["sHelpMatCapBlending"],MessageType.Warning);
                                    }
                                    if(isTransparent) materialEditor.ShaderProperty(matcapApplyTransparency, loc["sApplyTransparency"]);
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndVertical();
                            }
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Rim
                        if(shaderSetting.LIL_FEATURE_RIMLIGHT)
                        {
                            if(isLite)
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                materialEditor.ShaderProperty(useRim, loc["sRimLight"]);
                                DrawHelpButton(loc["sAnchorRimLight"]);
                                if(useRim.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    materialEditor.ShaderProperty(rimColor, loc["sColor"]);
                                    DrawLine();
                                    rimBorder.floatValue = 1.0f - EditorGUILayout.Slider(loc["sBorder"], 1.0f - rimBorder.floatValue, 0.0f, 1.0f);
                                    materialEditor.ShaderProperty(rimBlur, loc["sBlur"]);
                                    DrawLine();
                                    materialEditor.ShaderProperty(rimFresnelPower, loc["sFresnelPower"]);
                                    materialEditor.ShaderProperty(rimShadowMask, loc["sShadowMask"]);
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndVertical();
                            }
                            else
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                materialEditor.ShaderProperty(useRim, loc["sRimLight"]);
                                DrawHelpButton(loc["sAnchorRimLight"]);
                                if(useRim.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    if(shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR)    materialEditor.TexturePropertySingleLine(colorRGBAContent, rimColorTex, rimColor);
                                    else                                                materialEditor.ShaderProperty(rimColor, loc["sColor"]);
                                    DrawLine();
                                    rimBorder.floatValue = 1.0f - EditorGUILayout.Slider(loc["sBorder"], 1.0f - rimBorder.floatValue, 0.0f, 1.0f);
                                    materialEditor.ShaderProperty(rimBlur, loc["sBlur"]);
                                    materialEditor.ShaderProperty(rimFresnelPower, loc["sFresnelPower"]);
                                    DrawLine();
                                    materialEditor.ShaderProperty(rimEnableLighting, loc["sEnableLighting"]);
                                    materialEditor.ShaderProperty(rimShadowMask, loc["sShadowMask"]);
                                    if(isTransparent) materialEditor.ShaderProperty(rimApplyTransparency, loc["sApplyTransparency"]);
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
                GUILayout.Label(" " + loc["sAdvanced"], EditorStyles.boldLabel);

                //------------------------------------------------------------------------------------------------------------------------------
                // Outline
                if(!isRefr & !isFur)
                {
                    isShowOutline = Foldout(loc["sOutlineSetting"], loc["sOutlineTips"], isShowOutline);
                    DrawHelpButton(loc["sAnchorOutline"]);
                    if(isShowOutline)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        if(isOutl != EditorGUILayout.ToggleLeft(loc["sOutline"], isOutl, customToggleFont))
                        {
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, !isOutl, isLite, isStWr, isTess);
                        }
                        if(isOutl)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            if(shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR) TextureGUI(materialEditor, ref isShowOutlineMap, colorRGBAContent, outlineTex, outlineColor, outlineTex_ScrollRotate, true, shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV);
                            else                                            materialEditor.ShaderProperty(outlineColor, loc["sColor"]);
                            if(shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION)
                            {
                                ToneCorrectionGUI(materialEditor, material, outlineTex, outlineColor, outlineTexHSVG, true);
                                DrawLine();
                            }
                            if(shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH) materialEditor.TexturePropertySingleLine(new GUIContent(loc["sWidth"], loc["sWidthR"]), outlineWidthMask, outlineWidth);
                            else                                            materialEditor.ShaderProperty(outlineWidth, loc["sWidth"]);
                            materialEditor.ShaderProperty(outlineFixWidth, loc["sFixWidth"]);
                            materialEditor.ShaderProperty(outlineVertexR2Width, loc["sVertexR2Width"]);
                            materialEditor.ShaderProperty(outlineEnableLighting, loc["sEnableLighting"]);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Parallax
                if(shaderSetting.LIL_FEATURE_PARALLAX && !isFur && !isLite)
                {
                    isShowParallax = Foldout(loc["sParallax"], loc["sParallaxTips"], isShowParallax);
                    DrawHelpButton(loc["sAnchorParallax"]);
                    if(isShowParallax)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        materialEditor.ShaderProperty(useParallax, loc["sParallax"]);
                        if(useParallax.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            materialEditor.TexturePropertySingleLine(new GUIContent(loc["sParallax"], loc["sParallaxR"]), parallaxMap, parallax);
                            materialEditor.ShaderProperty(parallaxOffset, loc["sParallaxOffset"]);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Distance Fade
                if(shaderSetting.LIL_FEATURE_DISTANCE_FADE && !isLite)
                {
                    isShowDistanceFade = Foldout(loc["sDistanceFade"], loc["sDistanceFadeTips"], isShowDistanceFade);
                    DrawHelpButton(loc["sAnchorDistanceFade"]);
                    if(isShowDistanceFade)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(loc["sDistanceFade"], customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.ShaderProperty(distanceFadeColor, loc["sColor"]);
                        materialEditor.ShaderProperty(distanceFade, loc["sStartDistance"]+"|"+loc["sEndDistance"]+"|"+loc["sStrength"]);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // AudioLink
                if(shaderSetting.LIL_FEATURE_AUDIOLINK && !isFur && !isLite)
                {
                    isShowAudioLink = Foldout(loc["sAudioLink"], loc["sAudioLinkTips"], isShowAudioLink);
                    DrawHelpButton(loc["sAnchorAudioLink"]);
                    if(isShowAudioLink)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        materialEditor.ShaderProperty(useAudioLink, loc["sAudioLink"]);
                        if(useAudioLink.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            if(shaderSetting.LIL_FEATURE_TEX_AUDIOLINK_MASK)
                            {
                                materialEditor.ShaderProperty(audioLinkUVMode, loc["sAudioLinkUVMode"] + "|" + loc["sAudioLinkUVModeNone"] + "|" + loc["sAudioLinkUVModeRim"] + "|" + loc["sAudioLinkUVModeUV"] + "|" + loc["sAudioLinkUVModeMask"]);
                                if(audioLinkUVMode.floatValue == 3) materialEditor.TexturePropertySingleLine(new GUIContent(loc["sMask"], ""), audioLinkMask);
                            }
                            else
                            {
                                materialEditor.ShaderProperty(audioLinkUVMode, loc["sAudioLinkUVMode"] + "|" + loc["sAudioLinkUVModeNone"] + "|" + loc["sAudioLinkUVModeRim"] + "|" + loc["sAudioLinkUVModeUV"]);
                            }
                            if(audioLinkUVMode.floatValue == 0) materialEditor.ShaderProperty(audioLinkUVParams, loc["sOffset"] + "|" + loc["sAudioLinkBand"] + "|" + loc["sAudioLinkBandBass"] + "|" + loc["sAudioLinkBandLowMid"] + "|" + loc["sAudioLinkBandHighMid"] + "|" + loc["sAudioLinkBandTreble"]);
                            if(audioLinkUVMode.floatValue == 1) materialEditor.ShaderProperty(audioLinkUVParams, loc["sScale"] + "|" + loc["sOffset"] + "|" + loc["sAudioLinkBand"] + "|" + loc["sAudioLinkBandBass"] + "|" + loc["sAudioLinkBandLowMid"] + "|" + loc["sAudioLinkBandHighMid"] + "|" + loc["sAudioLinkBandTreble"]);
                            if(audioLinkUVMode.floatValue == 2) materialEditor.ShaderProperty(audioLinkUVParams, loc["sScale"] + "|" + loc["sOffset"] + "|" + loc["sAngle"] + "|" + loc["sAudioLinkBand"] + "|" + loc["sAudioLinkBandBass"] + "|" + loc["sAudioLinkBandLowMid"] + "|" + loc["sAudioLinkBandHighMid"] + "|" + loc["sAudioLinkBandTreble"]);
                            GUILayout.Label(loc["sAudioLinkApplyTo"], EditorStyles.boldLabel);
                            EditorGUI.indentLevel++;
                            if(shaderSetting.LIL_FEATURE_MAIN2ND) materialEditor.ShaderProperty(audioLink2Main2nd, loc["sMainColor2nd"]);
                            if(shaderSetting.LIL_FEATURE_MAIN3RD) materialEditor.ShaderProperty(audioLink2Main3rd, loc["sMainColor3rd"]);
                            if(shaderSetting.LIL_FEATURE_EMISSION_1ST) materialEditor.ShaderProperty(audioLink2Emission, loc["sEmission"]);
                            if(shaderSetting.LIL_FEATURE_EMISSION_2ND) materialEditor.ShaderProperty(audioLink2Emission2nd, loc["sEmission2nd"]);
                            if(shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX)
                            {
                                materialEditor.ShaderProperty(audioLink2Vertex, loc["sVertex"]);
                                if(audioLink2Vertex.floatValue == 1)
                                {
                                    EditorGUI.indentLevel++;
                                    if(shaderSetting.LIL_FEATURE_TEX_AUDIOLINK_MASK)
                                    {
                                        materialEditor.ShaderProperty(audioLinkVertexUVMode, loc["sAudioLinkUVMode"] + "|" + loc["sAudioLinkUVModeNone"] + "|" + loc["sAudioLinkUVModePosition"] + "|" + loc["sAudioLinkUVModeUV"] + "|" + loc["sAudioLinkUVModeMask"]);
                                        if(audioLinkVertexUVMode.floatValue == 3) materialEditor.TexturePropertySingleLine(new GUIContent(loc["sMask"], ""), audioLinkMask);
                                    }
                                    else
                                    {
                                        materialEditor.ShaderProperty(audioLinkVertexUVMode, loc["sAudioLinkUVMode"] + "|" + loc["sAudioLinkUVModeNone"] + "|" + loc["sAudioLinkUVModePosition"] + "|" + loc["sAudioLinkUVModeUV"]);
                                    }
                                    if(audioLinkVertexUVMode.floatValue == 0) materialEditor.ShaderProperty(audioLinkVertexUVParams, loc["sOffset"] + "|" + loc["sAudioLinkBand"] + "|" + loc["sAudioLinkBandBass"] + "|" + loc["sAudioLinkBandLowMid"] + "|" + loc["sAudioLinkBandHighMid"] + "|" + loc["sAudioLinkBandTreble"]);
                                    if(audioLinkVertexUVMode.floatValue == 1) materialEditor.ShaderProperty(audioLinkVertexUVParams, loc["sScale"] + "|" + loc["sOffset"] + "|" + loc["sAudioLinkBand"] + "|" + loc["sAudioLinkBandBass"] + "|" + loc["sAudioLinkBandLowMid"] + "|" + loc["sAudioLinkBandHighMid"] + "|" + loc["sAudioLinkBandTreble"]);
                                    if(audioLinkVertexUVMode.floatValue == 2) materialEditor.ShaderProperty(audioLinkVertexUVParams, loc["sScale"] + "|" + loc["sOffset"] + "|" + loc["sAngle"] + "|" + loc["sAudioLinkBand"] + "|" + loc["sAudioLinkBandBass"] + "|" + loc["sAudioLinkBandLowMid"] + "|" + loc["sAudioLinkBandHighMid"] + "|" + loc["sAudioLinkBandTreble"]);
                                    if(audioLinkVertexUVMode.floatValue == 1) materialEditor.ShaderProperty(audioLinkVertexStart, loc["sAudioLinkStartPosition"]);
                                    materialEditor.ShaderProperty(audioLinkVertexStrength, loc["sAudioLinkMovingVector"] + "|" + loc["sAudioLinkNormalStrength"]);
                                    EditorGUI.indentLevel--;
                                }
                            }
                            EditorGUI.indentLevel--;
                            if(shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL)
                            {
                                DrawLine();
                                materialEditor.ShaderProperty(audioLinkAsLocal, loc["sAudioLinkAsLocal"]);
                                if(audioLinkAsLocal.floatValue == 1)
                                {
                                    materialEditor.TexturePropertySingleLine(new GUIContent(loc["sAudioLinkLocalMap"], ""), audioLinkLocalMap);
                                    materialEditor.ShaderProperty(audioLinkLocalMapParams, loc["sAudioLinkLocalMapBPM"] + "|" + loc["sAudioLinkLocalMapNotes"] + "|" + loc["sOffset"]);
                                }
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Dissolve
                if(shaderSetting.LIL_FEATURE_DISSOLVE && !isFur && !isLite && renderingModeBuf != RenderingMode.Opaque)
                {
                    isShowDissolve = Foldout(loc["sDissolve"], loc["sDissolve"], isShowDissolve);
                    DrawHelpButton(loc["sAnchorDissolve"]);
                    if(isShowDissolve)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        materialEditor.ShaderProperty(dissolveParams, sDissolveParamsMode);
                        if(dissolveParams.vectorValue.x != 0)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            materialEditor.ShaderProperty(dissolveParams, sDissolveParamsOther);
                            if(dissolveParams.vectorValue.x == 1.0f)                                         TextureGUI(materialEditor, ref isShowDissolveMask, maskBlendContent, dissolveMask);
                            if(dissolveParams.vectorValue.x == 2.0f && dissolveParams.vectorValue.y == 0.0f) materialEditor.ShaderProperty(dissolvePos, loc["sPosition"] + "|2");
                            if(dissolveParams.vectorValue.x == 2.0f && dissolveParams.vectorValue.y == 1.0f) materialEditor.ShaderProperty(dissolvePos, loc["sVector"] + "|2");
                            if(dissolveParams.vectorValue.x == 3.0f && dissolveParams.vectorValue.y == 0.0f) materialEditor.ShaderProperty(dissolvePos, loc["sPosition"] + "|3");
                            if(dissolveParams.vectorValue.x == 3.0f && dissolveParams.vectorValue.y == 1.0f) materialEditor.ShaderProperty(dissolvePos, loc["sVector"] + "|3");
                            if(shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE)
                            {
                                TextureGUI(materialEditor, ref isShowDissolveNoiseMask, noiseMaskContent, dissolveNoiseMask, dissolveNoiseStrength, dissolveNoiseMask_ScrollRotate);
                            }
                            materialEditor.ShaderProperty(dissolveColor, loc["sColor"]);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Refraction
                if(isRefr)
                {
                    isShowRefraction = Foldout(loc["sRefractionSetting"], loc["sRefractionTips"], isShowRefraction);
                    DrawHelpButton(loc["sAnchorRefraction"]);
                    if(isShowRefraction)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(loc["sRefraction"], customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        materialEditor.ShaderProperty(refractionStrength, loc["sStrength"]);
                        materialEditor.ShaderProperty(refractionFresnelPower, loc["sRefractionFresnel"]);
                        materialEditor.ShaderProperty(refractionColorFromMain, loc["sColorFromMain"]);
                        materialEditor.ShaderProperty(refractionColor, loc["sColor"]);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Fur
                if(isFur)
                {
                    isShowFur = Foldout(loc["sFurSetting"], loc["sFurTips"], isShowFur);
                    DrawHelpButton(loc["sAnchorFur"]);
                    if(isShowFur)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(loc["sFur"], customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        if(shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL) materialEditor.TexturePropertySingleLine(normalMapContent, furVectorTex,furVectorScale);
                        materialEditor.ShaderProperty(furVector, loc["sVector"] + "|" + loc["sLength"]);
                        materialEditor.ShaderProperty(vertexColor2FurVector, loc["sVertexColor2Vector"]);
                        materialEditor.ShaderProperty(furGravity, loc["sGravity"]);
                        DrawLine();
                        materialEditor.TexturePropertySingleLine(new GUIContent(loc["sNoise"], loc["sNoiseR"]), furNoiseMask);
                        materialEditor.TextureScaleOffsetProperty(furNoiseMask);
                        if(shaderSetting.LIL_FEATURE_TEX_FUR_MASK) materialEditor.TexturePropertySingleLine(new GUIContent(loc["sMask"], loc["sAlphaR"]), furMask);
                        materialEditor.ShaderProperty(furAO, loc["sAO"]);
                        materialEditor.ShaderProperty(furLayerNum, loc["sLayerNum"]);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Stencil
                isShowStencil = Foldout(loc["sStencilSetting"], loc["sStencilTips"], isShowStencil);
                DrawHelpButton(loc["sAnchorStencil"]);
                if(isShowStencil)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    EditorGUILayout.LabelField(loc["sStencilSetting"], customToggleFont);
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
                            EditorGUILayout.LabelField(loc["sOutline"], EditorStyles.boldLabel);
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
                            EditorGUILayout.LabelField(loc["sOutline"], EditorStyles.boldLabel);
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
                            EditorGUILayout.LabelField(loc["sOutline"], EditorStyles.boldLabel);
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
                            EditorGUILayout.LabelField(loc["sOutline"], EditorStyles.boldLabel);
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
                            EditorGUILayout.LabelField(loc["sOutline"], EditorStyles.boldLabel);
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
                            EditorGUILayout.LabelField(loc["sOutline"], EditorStyles.boldLabel);
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
                        EditorGUILayout.LabelField(loc["sOutline"], EditorStyles.boldLabel);
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
                        EditorGUILayout.LabelField(loc["sFur"], EditorStyles.boldLabel);
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
                isShowRendering = Foldout(loc["sRenderingSetting"], loc["sRenderingTips"], isShowRendering);
                DrawHelpButton(loc["sAnchorRendering"]);
                if(isShowRendering)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // Reset Button
                    if(GUILayout.Button(loc["sRenderingReset"], offsetButton))
                    {
                        material.enableInstancing = false;
                        SetupMaterialWithRenderingMode(material, renderingModeBuf, isOutl, isLite, isStWr, isTess);
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Base
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(loc["sRenderingSetting"], customToggleFont);
                        EditorGUILayout.BeginVertical(boxInner);
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Shader
                        int shaderType = 0;
                        if(isLite) shaderType = 1;
                        int shaderTypeBuf = shaderType;
                        shaderType = EditorGUILayout.Popup(loc["sShaderType"],shaderType,new String[]{loc["sShaderTypeNormal"],loc["sShaderTypeLite"]});
                        if(shaderTypeBuf != shaderType)
                        {
                            if(shaderType==0) isLite = false;
                            if(shaderType==1) isLite = true;
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, isOutl, isLite, isStWr, isTess);
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Rendering
                        materialEditor.ShaderProperty(cull, sCullModes);
                        materialEditor.ShaderProperty(zwrite, loc["sZWrite"]);
                        materialEditor.ShaderProperty(ztest, loc["sZTest"]);
                        materialEditor.ShaderProperty(offsetFactor, loc["sOffsetFactor"]);
                        materialEditor.ShaderProperty(offsetUnits, loc["sOffsetUnits"]);
                        materialEditor.ShaderProperty(colorMask, loc["sColorMask"]);
                        DrawLine();
                        BlendSettingGUI(materialEditor, ref isShowBlend, loc["sForward"], srcBlend, dstBlend, srcBlendAlpha, dstBlendAlpha, blendOp, blendOpAlpha);
                        if(!(isFur & !isCutout))
                        {
                            DrawLine();
                            BlendSettingGUI(materialEditor, ref isShowBlendAdd, loc["sForwardAdd"], srcBlendFA, dstBlendFA, srcBlendAlphaFA, dstBlendAlphaFA, blendOpFA, blendOpAlphaFA);
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
                        EditorGUILayout.LabelField(loc["sOutline"], customToggleFont);
                        EditorGUILayout.BeginVertical(boxInner);
                        materialEditor.ShaderProperty(outlineCull, sCullModes);
                        materialEditor.ShaderProperty(outlineZwrite, loc["sZWrite"]);
                        materialEditor.ShaderProperty(outlineZtest, loc["sZTest"]);
                        materialEditor.ShaderProperty(outlineOffsetFactor, loc["sOffsetFactor"]);
                        materialEditor.ShaderProperty(outlineOffsetUnits, loc["sOffsetUnits"]);
                        materialEditor.ShaderProperty(outlineColorMask, loc["sColorMask"]);
                        DrawLine();
                        BlendSettingGUI(materialEditor, ref isShowBlendOutline, loc["sForward"], outlineSrcBlend, outlineDstBlend, outlineSrcBlendAlpha, outlineDstBlendAlpha, outlineBlendOp, outlineBlendOpAlpha);
                        DrawLine();
                        BlendSettingGUI(materialEditor, ref isShowBlendAddOutline, loc["sForwardAdd"], outlineSrcBlendFA, outlineDstBlendFA, outlineSrcBlendAlphaFA, outlineDstBlendAlphaFA, outlineBlendOpFA, outlineBlendOpAlphaFA);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // Fur
                    if(isFur)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(loc["sFur"], customToggleFont);
                        EditorGUILayout.BeginVertical(boxInner);
                        materialEditor.ShaderProperty(furCull, sCullModes);
                        materialEditor.ShaderProperty(furZwrite, loc["sZWrite"]);
                        materialEditor.ShaderProperty(furZtest, loc["sZTest"]);
                        materialEditor.ShaderProperty(furOffsetFactor, loc["sOffsetFactor"]);
                        materialEditor.ShaderProperty(furOffsetUnits, loc["sOffsetUnits"]);
                        materialEditor.ShaderProperty(furColorMask, loc["sColorMask"]);
                        DrawLine();
                        BlendSettingGUI(materialEditor, ref isShowBlendFur, loc["sForward"], furSrcBlend, furDstBlend, furSrcBlendAlpha, furDstBlendAlpha, furBlendOp, furBlendOpAlpha);
                        DrawLine();
                        BlendSettingGUI(materialEditor, ref isShowBlendAddFur, loc["sForwardAdd"], furSrcBlendFA, furDstBlendFA, furSrcBlendAlphaFA, furDstBlendAlphaFA, furBlendOpFA, furBlendOpAlphaFA);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Tessellation
                if(shaderSetting.LIL_FEATURE_TEX_TESSELLATION && !isLite && !isRefr && !isFur)
                {
                    isShowTess = Foldout(loc["sTessellation"], loc["sTessellationTips"], isShowTess);
                    DrawHelpButton(loc["sAnchorTessellation"]);
                    if(isShowTess)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        if(isTess != EditorGUILayout.ToggleLeft(loc["sTessellation"], isTess, customToggleFont))
                        {
                            SetupMaterialWithRenderingMode(material, renderingModeBuf, isOutl, isLite, isStWr, !isTess);
                        }
                        if(isTess)
                        {
                            EditorGUILayout.BeginVertical(boxInner);
                            materialEditor.ShaderProperty(tessEdge, loc["sTessellationEdge"]);
                            materialEditor.ShaderProperty(tessStrength, loc["sStrength"]);
                            materialEditor.ShaderProperty(tessShrink, loc["sTessellationShrink"]);
                            materialEditor.ShaderProperty(tessFactorMax, loc["sTessellationFactor"]);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                EditorGUILayout.Space();

                //------------------------------------------------------------------------------------------------------------------------------
                // Optimization
                GUILayout.Label(" " + loc["sOptimization"], EditorStyles.boldLabel);
                isShowOptimization = Foldout(loc["sOptimization"], loc["sOptimizationTips"], isShowOptimization);
                DrawHelpButton(loc["sAnchorOptimization"]);
                if(isShowOptimization)
                {
                    EditorGUILayout.BeginVertical(boxOuter);
                    EditorGUILayout.LabelField(loc["sOptimization"], customToggleFont);
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    if(GUILayout.Button(loc["sRemoveUnused"])) RemoveUnusedTexture(material, isLite, isFur);
                    if(!isLite & !isFur)
                    {
                        TextureBakeGUI(material, 0);
                        TextureBakeGUI(material, 1);
                        TextureBakeGUI(material, 2);
                        TextureBakeGUI(material, 3);
                    }
                    if(!isLite && GUILayout.Button(loc["sConvertLite"])) CreateLiteMaterial(material);
                    if(mtoon != null && GUILayout.Button(loc["sConvertMToon"])) CreateMToonMaterial(material);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndVertical();
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Shader Setting
                if(editorSetting.unlockShaderSetting && !isLite)
                {
                    isShowShaderSetting = Foldout(loc["sShaderSetting"], loc["sShaderSettingTips"], isShowShaderSetting);
                    DrawHelpButton(loc["sAnchorShaderSetting"]);
                    if(isShowShaderSetting)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(loc["sShaderSetting"], customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        EditorGUILayout.HelpBox(loc["sHelpShaderSetting"],MessageType.Info);
                        ShaderSettingGUI();
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }


                //------------------------------------------------------------------------------------------------------------------------------
                // Custom Properties
                if(isCustomShader)
                {
                    EditorGUILayout.Space();
                    GUILayout.Label(" " + loc["sCustomProperties"], EditorStyles.boldLabel);
                    isShowCustomProperties = Foldout(loc["sCustomProperties"], loc["sCustomPropertiesTips"], isShowCustomProperties);
                    if(isShowCustomProperties)
                    {
                        DrawCustomProperties(materialEditor, material);
                    }
                }
            }

            //------------------------------------------------------------------------------------------------------------------------------
            // Preset
            if(editorMode == EditorMode.Preset)
            {
                DrawPreset(material);
            }
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
            useRim = FindProperty("_UseRim", props);
                rimColor = FindProperty("_RimColor", props);
                rimColorTex = FindProperty("_RimColorTex", props);
                rimBorder = FindProperty("_RimBorder", props);
                rimBlur = FindProperty("_RimBlur", props);
                rimFresnelPower = FindProperty("_RimFresnelPower", props);
                rimEnableLighting = FindProperty("_RimEnableLighting", props);
                rimShadowMask = FindProperty("_RimShadowMask", props);
                rimApplyTransparency = FindProperty("_RimApplyTransparency", props);
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
            //useFur = FindProperty("_UseFur", props);
                furNoiseMask = FindProperty("_FurNoiseMask", props);
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

        void LoadLiteProperties(MaterialProperty[] props)
        {
            invisible = FindProperty("_Invisible", props);
            asUnlit = FindProperty("_AsUnlit", props);
            cutoff = FindProperty("_Cutoff", props);
            flipNormal = FindProperty("_FlipNormal", props);
            backfaceForceShadow = FindProperty("_BackfaceForceShadow", props);
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
                shadowEnvStrength = FindProperty("_ShadowEnvStrength", props);
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
        // Initialize
        static void InitializeEditorSetting()
        {
            editorSetting = (lilToonEditorSetting)AssetDatabase.LoadAssetAtPath(editorSettingPath, typeof(lilToonEditorSetting));
            if(editorSetting == null)
            {
                editorSetting = new lilToonEditorSetting();
                AssetDatabase.CreateAsset(editorSetting, editorSettingPath);
                editorSetting.settingPath = "Assets/lilToonSetting/ShaderSetting.asset";
                editorSetting.unlockShaderSetting = true;
                EditorUtility.SetDirty(editorSetting);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        static void InitializeShaderSetting()
        {
            shaderSetting = (lilToonSetting)AssetDatabase.LoadAssetAtPath(editorSetting.settingPath, typeof(lilToonSetting));
            if(shaderSetting == null)
            {
                shaderSetting = new lilToonSetting();
                AssetDatabase.CreateAsset(shaderSetting, editorSetting.settingPath);
                shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV = false;
                shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION = true;
                shaderSetting.LIL_FEATURE_MAIN2ND = false;
                shaderSetting.LIL_FEATURE_MAIN3RD = false;
                shaderSetting.LIL_FEATURE_DECAL = false;
                shaderSetting.LIL_FEATURE_ANIMATE_DECAL = false;
                shaderSetting.LIL_FEATURE_LAYER_DISSOLVE = false;
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
                shaderSetting.LIL_FEATURE_RIMLIGHT = true;
                shaderSetting.LIL_FEATURE_PARALLAX = false;
                shaderSetting.LIL_FEATURE_POM = false;
                shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER = false;
                shaderSetting.LIL_FEATURE_DISTANCE_FADE = false;
                shaderSetting.LIL_FEATURE_AUDIOLINK = false;
                shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX = false;
                shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL = false;
                shaderSetting.LIL_FEATURE_DISSOLVE = false;
                shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV = false;
                shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION = false;
                shaderSetting.LIL_FEATURE_TEX_LAYER_MASK = false;
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
                shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR = true;
                shaderSetting.LIL_FEATURE_TEX_AUDIOLINK_MASK = true;
                shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR = true;
                shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH = true;
                shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL = false;
                shaderSetting.LIL_FEATURE_TEX_FUR_MASK = false;
                shaderSetting.LIL_FEATURE_TEX_TESSELLATION = false;
                EditorUtility.SetDirty(shaderSetting);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        static void SelectEditorMode()
        {
            string[] sEditorModeList = {loc["sEditorModeSimple"],loc["sEditorModeAdvanced"],loc["sEditorModePreset"]};
            editorMode = (EditorMode)EditorGUILayout.Popup(loc["sEditorMode"], (int)editorMode, sEditorModeList);
            switch (editorMode)
            {
                case EditorMode.Simple:
                    EditorGUILayout.HelpBox(loc["sHelpSimple"],MessageType.Info);
                    break;
                case EditorMode.Advanced:
                    EditorGUILayout.HelpBox(loc["sHelpAdvanced"],MessageType.Info);
                    break;
                case EditorMode.Preset:
                    EditorGUILayout.HelpBox(loc["sHelpPreset"],MessageType.Info);
                    break;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Editor
        static void DrawLine()
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
                Application.OpenURL(loc["sManualURL"] + helpAnchor);
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
            EditorGUI.indentLevel++;
            Rect position = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(position, versionInfo, EditorStyles.boldLabel);
            EditorGUI.indentLevel--;

            position.x += 10;
            isShowWebPages = EditorGUI.Foldout(position, isShowWebPages, "");
            if(isShowWebPages)
            {
                EditorGUI.indentLevel++;
                DrawWebButton("BOOTH", boothURL);
                DrawWebButton("GitHub", githubURL);
                DrawWebButton("Twitter", twitterURL);
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

            // Setup
            if(loc.Count == 0)
            {
                if(Application.systemLanguage == SystemLanguage.Japanese)                   outnum = 1;
                else if(Application.systemLanguage == SystemLanguage.Korean)                outnum = 2;
                else if(Application.systemLanguage == SystemLanguage.ChineseSimplified)     outnum = 3;
                else if(Application.systemLanguage == SystemLanguage.ChineseTraditional)    outnum = 4;
                else                                                                        outnum = 0;
                string langBuf = ((TextAsset)AssetDatabase.LoadAssetAtPath("Assets/lilToon/Editor/lang.txt", typeof(TextAsset))).text;
                string[] langData = langBuf.Split('\n');
                languageNames = langData[0].Substring(langData[0].IndexOf("\t")+1);
                languageName = languageNames.Split('\t')[outnum];
                for(int i = 0; i < langData.Length; i++)
                {
                    string[] lineContents = langData[i].Split('\t');
                    loc[lineContents[0]] = lineContents[outnum+1];
                }
            }

            // Select language
            string[] langName = languageNames.Split('\t');
            outnum = EditorGUILayout.Popup("Language", outnum, langName);

            // Load language
            if(outnum != lnum)
            {
                string langBuf = ((TextAsset)AssetDatabase.LoadAssetAtPath("Assets/lilToon/Editor/lang.txt", typeof(TextAsset))).text;
                string[] langData = langBuf.Split('\n');
                languageNames = langData[0].Substring(langData[0].IndexOf("\t")+1);
                languageName = languageNames.Split('\t')[outnum];
                for(int i = 0; i < langData.Length; i++)
                {
                    string[] lineContents = langData[i].Split('\t');
                    loc[lineContents[0]] = lineContents[outnum+1];
                }
            }

            if(!String.IsNullOrEmpty(loc["sLanguageWarning"])) EditorGUILayout.HelpBox(loc["sLanguageWarning"],MessageType.Warning);

            return outnum;
        }

        static void ShaderSettingGUI()
        {
            GUIStyle applyButton = new GUIStyle(GUI.skin.button);
            applyButton.normal.textColor = Color.red;
            applyButton.fontStyle = FontStyle.Bold;

            if(isShaderSettingChanged && GUILayout.Button("Apply", applyButton))
            {
                lilApplyShaderSetting();
                isShaderSettingChanged = false;
            }

            EditorGUI.BeginChangeCheck();

            lilToggleGUI(loc["sSettingAnimateMainUV"], ref shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV);
            DrawLine();

            lilToggleGUI(loc["sSettingMainToneCorrection"], ref shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION);
            lilToggleGUI(loc["sSettingMain2nd"], ref shaderSetting.LIL_FEATURE_MAIN2ND);
            lilToggleGUI(loc["sSettingMain3rd"], ref shaderSetting.LIL_FEATURE_MAIN3RD);
            if(shaderSetting.LIL_FEATURE_MAIN2ND || shaderSetting.LIL_FEATURE_MAIN3RD)
            {
                EditorGUI.indentLevel++;
                lilToggleGUI(loc["sSettingDecal"], ref shaderSetting.LIL_FEATURE_DECAL);
                lilToggleGUI(loc["sSettingAnimateDecal"], ref shaderSetting.LIL_FEATURE_ANIMATE_DECAL);
                lilToggleGUI(loc["sSettingTexLayerMask"], ref shaderSetting.LIL_FEATURE_TEX_LAYER_MASK);
                lilToggleGUI(loc["sSettingLayerDissolve"], ref shaderSetting.LIL_FEATURE_LAYER_DISSOLVE);
                if(shaderSetting.LIL_FEATURE_LAYER_DISSOLVE)
                {
                    EditorGUI.indentLevel++;
                    lilToggleGUI(loc["sSettingTexDissolveNoise"], ref shaderSetting.LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
            DrawLine();

            lilToggleGUI(loc["sSettingShadow"], ref shaderSetting.LIL_FEATURE_SHADOW);
            if(shaderSetting.LIL_FEATURE_SHADOW)
            {
                EditorGUI.indentLevel++;
                lilToggleGUI(loc["sSettingReceiveShadow"], ref shaderSetting.LIL_FEATURE_RECEIVE_SHADOW);
                lilToggleGUI(loc["sSettingTexShadowBlur"], ref shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR);
                lilToggleGUI(loc["sSettingTexShadowBorder"], ref shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER);
                lilToggleGUI(loc["sSettingTexShadowStrength"], ref shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH);
                lilToggleGUI(loc["sSettingTexShadow1st"], ref shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST);
                lilToggleGUI(loc["sSettingTexShadow2nd"], ref shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            lilToggleGUI(loc["sSettingEmission1st"], ref shaderSetting.LIL_FEATURE_EMISSION_1ST);
            lilToggleGUI(loc["sSettingEmission2nd"], ref shaderSetting.LIL_FEATURE_EMISSION_2ND);
            if(shaderSetting.LIL_FEATURE_EMISSION_1ST || shaderSetting.LIL_FEATURE_EMISSION_2ND)
            {
                EditorGUI.indentLevel++;
                lilToggleGUI(loc["sSettingEmissionUV"], ref shaderSetting.LIL_FEATURE_EMISSION_UV);
                lilToggleGUI(loc["sSettingAnimateEmissionUV"], ref shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_UV);
                lilToggleGUI(loc["sSettingTexEmissionMask"], ref shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK);
                lilToggleGUI(loc["sSettingEmissionMaskUV"], ref shaderSetting.LIL_FEATURE_EMISSION_MASK_UV);
                lilToggleGUI(loc["sSettingAnimateEmissionMaskUV"], ref shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV);
                lilToggleGUI(loc["sSettingEmissionGradation"], ref shaderSetting.LIL_FEATURE_EMISSION_GRADATION);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            lilToggleGUI(loc["sSettingNormal1st"], ref shaderSetting.LIL_FEATURE_NORMAL_1ST);
            lilToggleGUI(loc["sSettingNormal2nd"], ref shaderSetting.LIL_FEATURE_NORMAL_2ND);
            if(shaderSetting.LIL_FEATURE_NORMAL_2ND)
            {
                EditorGUI.indentLevel++;
                lilToggleGUI(loc["sSettingTexNormalMask"], ref shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            lilToggleGUI(loc["sSettingReflection"], ref shaderSetting.LIL_FEATURE_REFLECTION);
            if(shaderSetting.LIL_FEATURE_REFLECTION)
            {
                EditorGUI.indentLevel++;
                lilToggleGUI(loc["sSettingTexReflectionSmoothness"], ref shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS);
                lilToggleGUI(loc["sSettingTexReflectionMetallic"], ref shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC);
                lilToggleGUI(loc["sSettingTexReflectionColor"], ref shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            lilToggleGUI(loc["sSettingMatCap"], ref shaderSetting.LIL_FEATURE_MATCAP);
            if(shaderSetting.LIL_FEATURE_MATCAP)
            {
                EditorGUI.indentLevel++;
                lilToggleGUI(loc["sSettingTexMatCapMask"], ref shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            lilToggleGUI(loc["sSettingRimLight"], ref shaderSetting.LIL_FEATURE_RIMLIGHT);
            if(shaderSetting.LIL_FEATURE_RIMLIGHT)
            {
                EditorGUI.indentLevel++;
                lilToggleGUI(loc["sSettingTexRimLightColor"], ref shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            lilToggleGUI(loc["sSettingParallax"], ref shaderSetting.LIL_FEATURE_PARALLAX);
            if(shaderSetting.LIL_FEATURE_PARALLAX)
            {
                EditorGUI.indentLevel++;
                lilToggleGUI(loc["sSettingPOM"], ref shaderSetting.LIL_FEATURE_POM);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            lilToggleGUI(loc["sSettingClippingCanceller"], ref shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER);
            DrawLine();

            lilToggleGUI(loc["sSettingDistanceFade"], ref shaderSetting.LIL_FEATURE_DISTANCE_FADE);
            DrawLine();

            lilToggleGUI(loc["sSettingAudioLink"], ref shaderSetting.LIL_FEATURE_AUDIOLINK);
            if(shaderSetting.LIL_FEATURE_AUDIOLINK)
            {
                EditorGUI.indentLevel++;
                lilToggleGUI(loc["sSettingAudioLinkVertex"], ref shaderSetting.LIL_FEATURE_AUDIOLINK_VERTEX);
                lilToggleGUI(loc["sSettingAudioLinkMask"], ref shaderSetting.LIL_FEATURE_TEX_AUDIOLINK_MASK);
                lilToggleGUI(loc["sSettingAudioLinkLocal"], ref shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            lilToggleGUI(loc["sSettingDissolve"], ref shaderSetting.LIL_FEATURE_DISSOLVE);
            if(shaderSetting.LIL_FEATURE_DISSOLVE)
            {
                EditorGUI.indentLevel++;
                lilToggleGUI(loc["sSettingTexDissolveNoise"], ref shaderSetting.LIL_FEATURE_TEX_DISSOLVE_NOISE);
                EditorGUI.indentLevel--;
            }
            DrawLine();

            lilToggleGUI(loc["sSettingTexOutlineColor"], ref shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR);
            if(shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR)
            {
                EditorGUI.indentLevel++;
                lilToggleGUI(loc["sSettingOutlineToneCorrection"], ref shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION);
                EditorGUI.indentLevel--;
            }
            lilToggleGUI(loc["sSettingAnimateOutlineUV"], ref shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV);
            lilToggleGUI(loc["sSettingTexOutlineWidth"], ref shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH);
            DrawLine();

            lilToggleGUI(loc["sSettingTexFurNormal"], ref shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL);
            lilToggleGUI(loc["sSettingTexFurMask"], ref shaderSetting.LIL_FEATURE_TEX_FUR_MASK);
            DrawLine();

            lilToggleGUI(loc["sSettingTessellation"], ref shaderSetting.LIL_FEATURE_TEX_TESSELLATION);

            if(EditorGUI.EndChangeCheck())
            {
                isShaderSettingChanged = true;
                EditorUtility.SetDirty(shaderSetting);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        static void lilToggleGUI(string label, ref bool value)
        {
            value = EditorGUILayout.ToggleLeft(label, value);
        }

        static void lilApplyShaderSetting()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#ifndef LIL_SETTING_INCLUDED\r\n#define LIL_SETTING_INCLUDED\r\n\r\n");
            if(shaderSetting.LIL_FEATURE_ANIMATE_MAIN_UV) sb.Append("#define LIL_FEATURE_ANIMATE_MAIN_UV\r\n");
            if(shaderSetting.LIL_FEATURE_MAIN_TONE_CORRECTION) sb.Append("#define LIL_FEATURE_MAIN_TONE_CORRECTION\r\n");
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
                if(shaderSetting.LIL_FEATURE_EMISSION_MASK_UV) sb.Append("#define LIL_FEATURE_EMISSION_MASK_UV\r\n");
                if(shaderSetting.LIL_FEATURE_ANIMATE_EMISSION_MASK_UV) sb.Append("#define LIL_FEATURE_ANIMATE_EMISSION_MASK_UV\r\n");
                if(shaderSetting.LIL_FEATURE_EMISSION_GRADATION) sb.Append("#define LIL_FEATURE_EMISSION_GRADATION\r\n");
                if(shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK) sb.Append("#define LIL_FEATURE_TEX_EMISSION_MASK\r\n");
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
            if(shaderSetting.LIL_FEATURE_MATCAP)
            {
                sb.Append("#define LIL_FEATURE_MATCAP\r\n");
                if(shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK) sb.Append("#define LIL_FEATURE_TEX_MATCAP_MASK\r\n");
            }
            if(shaderSetting.LIL_FEATURE_RIMLIGHT)
            {
                sb.Append("#define LIL_FEATURE_RIMLIGHT\r\n");
                if(shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR) sb.Append("#define LIL_FEATURE_TEX_RIMLIGHT_COLOR\r\n");
            }
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
            if(shaderSetting.LIL_FEATURE_TEX_OUTLINE_COLOR)
            {
                sb.Append("#define LIL_FEATURE_TEX_OUTLINE_COLOR\r\n");
                if(shaderSetting.LIL_FEATURE_OUTLINE_TONE_CORRECTION) sb.Append("#define LIL_FEATURE_OUTLINE_TONE_CORRECTION\r\n");
            }
            if(shaderSetting.LIL_FEATURE_ANIMATE_OUTLINE_UV) sb.Append("#define LIL_FEATURE_ANIMATE_OUTLINE_UV\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_OUTLINE_WIDTH) sb.Append("#define LIL_FEATURE_TEX_OUTLINE_WIDTH\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_FUR_NORMAL) sb.Append("#define LIL_FEATURE_TEX_FUR_NORMAL\r\n");
            if(shaderSetting.LIL_FEATURE_TEX_FUR_MASK) sb.Append("#define LIL_FEATURE_TEX_FUR_MASK\r\n");
            sb.Append("\r\n#endif");
            string shaderSettingString = sb.ToString();
            StreamWriter sw = new StreamWriter(Path.GetFullPath("Assets/lilToonSetting/lil_setting.hlsl"),false);
            sw.Write(shaderSettingString);
            sw.Close();
            RewriteReceiveShadow(ltspo, shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW);
            RewriteReceiveShadow(ltspc, shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW);
            RewriteReceiveShadow(ltspt, shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW);
            RewriteReceiveShadow(ltsptesso, shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW);
            RewriteReceiveShadow(ltsptessc, shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW);
            RewriteReceiveShadow(ltsptesst, shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW);
            RewriteReceiveShadow(ltsref, shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW);
            RewriteReceiveShadow(ltsrefb, shaderSetting.LIL_FEATURE_SHADOW && shaderSetting.LIL_FEATURE_RECEIVE_SHADOW);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        static void RewriteReceiveShadow(Shader shader, bool enable)
        {
            string path = Path.GetFullPath(AssetDatabase.GetAssetPath(shader));
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

        static void DrawPreset(Material material)
        {
            GUILayout.Label(" " + loc["sPresets"], EditorStyles.boldLabel);
            if(presets == null) LoadPresets();
            ShowPresets(material);
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            if(GUILayout.Button(loc["sPresetRefresh"])) LoadPresets();
            if(GUILayout.Button(loc["sPresetSave"])) EditorWindow.GetWindow<lilPresetWindow>();
            GUILayout.EndHorizontal();
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Material Setup
        static void SetupMaterialWithRenderingMode(Material material, RenderingMode renderingMode, bool isoutl, bool islite, bool isstencil, bool istess)
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
            }
            if(isstencil) material.renderQueue = material.shader.renderQueue - 1;
            material.SetInt("_ZWrite", 1);
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
                material.SetInt("_OutlineZTest", 4);
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

        void RemoveUnusedTexture(Material material, bool islite, bool isfur)
        {
            RemoveUnusedProperties(material);
            if(isfur)
            {
                if(!shaderSetting.LIL_FEATURE_SHADOW) useShadow.floatValue = 0.0f;
                if(useShadow.floatValue == 0.0f || !shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR) shadowBlurMask.textureValue = null;
                if(useShadow.floatValue == 0.0f || !shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER) shadowBorderMask.textureValue = null;
                if(useShadow.floatValue == 0.0f || !shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH) shadowStrengthMask.textureValue = null;
                if(useShadow.floatValue == 0.0f || !shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST) shadowColorTex.textureValue = null;
                if(useShadow.floatValue == 0.0f || !shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND) shadow2ndColorTex.textureValue = null;
            }
            if(islite)
            {
                if(useShadow.floatValue == 0.0f)
                {
                    shadowColorTex.textureValue = null;
                }
                if(useEmission.floatValue == 0.0f)
                {
                    emissionMap.textureValue = null;
                }
                if(useMatCap.floatValue == 0.0f)
                {
                    matcapTex.textureValue = null;
                }
            }
            if(!islite && !isfur)
            {
                if(!shaderSetting.LIL_FEATURE_MAIN2ND) useMain2ndTex.floatValue = 0.0f;
                if(!shaderSetting.LIL_FEATURE_MAIN3RD) useMain3rdTex.floatValue = 0.0f;
                if(!shaderSetting.LIL_FEATURE_SHADOW) useShadow.floatValue = 0.0f;
                if(!shaderSetting.LIL_FEATURE_EMISSION_1ST) useEmission.floatValue = 0.0f;
                if(!shaderSetting.LIL_FEATURE_EMISSION_2ND) useEmission2nd.floatValue = 0.0f;
                if(!shaderSetting.LIL_FEATURE_EMISSION_GRADATION)
                {
                    emissionUseGrad.floatValue = 0.0f;
                    emission2ndUseGrad.floatValue = 0.0f;
                }
                if(!shaderSetting.LIL_FEATURE_NORMAL_1ST) useBumpMap.floatValue = 0.0f;
                if(!shaderSetting.LIL_FEATURE_NORMAL_2ND) useBump2ndMap.floatValue = 0.0f;
                if(!shaderSetting.LIL_FEATURE_REFLECTION) useReflection.floatValue = 0.0f;
                if(!shaderSetting.LIL_FEATURE_MATCAP) useMatCap.floatValue = 0.0f;
                if(!shaderSetting.LIL_FEATURE_RIMLIGHT) useRim.floatValue = 0.0f;
                if(!shaderSetting.LIL_FEATURE_PARALLAX) parallaxMap.floatValue = 0.0f;

                if(useMain2ndTex.floatValue == 0.0f) main2ndTex.textureValue = null;
                if(useMain2ndTex.floatValue == 0.0f || !shaderSetting.LIL_FEATURE_TEX_LAYER_MASK) main2ndBlendMask.textureValue = null;
                if(useMain3rdTex.floatValue == 0.0f) main2ndTex.textureValue = null;
                if(useMain3rdTex.floatValue == 0.0f || !shaderSetting.LIL_FEATURE_TEX_LAYER_MASK) main2ndBlendMask.textureValue = null;
                if(useShadow.floatValue == 0.0f || !shaderSetting.LIL_FEATURE_TEX_SHADOW_BLUR) shadowBlurMask.textureValue = null;
                if(useShadow.floatValue == 0.0f || !shaderSetting.LIL_FEATURE_TEX_SHADOW_BORDER) shadowBorderMask.textureValue = null;
                if(useShadow.floatValue == 0.0f || !shaderSetting.LIL_FEATURE_TEX_SHADOW_STRENGTH) shadowStrengthMask.textureValue = null;
                if(useShadow.floatValue == 0.0f || !shaderSetting.LIL_FEATURE_TEX_SHADOW_1ST) shadowColorTex.textureValue = null;
                if(useShadow.floatValue == 0.0f || !shaderSetting.LIL_FEATURE_TEX_SHADOW_2ND) shadow2ndColorTex.textureValue = null;
                if(useEmission.floatValue == 0.0f) emissionMap.textureValue = null;
                if(useEmission.floatValue == 0.0f || !shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK) emissionBlendMask.textureValue = null;
                if(useEmission.floatValue == 0.0f || emissionUseGrad.floatValue == 0.0f || !shaderSetting.LIL_FEATURE_EMISSION_GRADATION) emissionGradTex.textureValue = null;
                if(useEmission2nd.floatValue == 0.0f) emission2ndMap.textureValue = null;
                if(useEmission2nd.floatValue == 0.0f || !shaderSetting.LIL_FEATURE_TEX_EMISSION_MASK) emission2ndBlendMask.textureValue = null;
                if(useEmission2nd.floatValue == 0.0f || emission2ndUseGrad.floatValue == 0.0f || !shaderSetting.LIL_FEATURE_EMISSION_GRADATION) emission2ndGradTex.textureValue = null;
                if(useBumpMap.floatValue == 0.0f) bumpMap.textureValue = null;
                if(useBump2ndMap.floatValue == 0.0f) bump2ndMap.textureValue = null;
                if(useBump2ndMap.floatValue == 0.0f || !shaderSetting.LIL_FEATURE_TEX_NORMAL_MASK) bump2ndScaleMask.textureValue = null;
                if(useReflection.floatValue == 0.0f || !shaderSetting.LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS) smoothnessTex.textureValue = null;
                if(useReflection.floatValue == 0.0f || !shaderSetting.LIL_FEATURE_TEX_REFLECTION_METALLIC) metallicGlossMap.textureValue = null;
                if(useReflection.floatValue == 0.0f || !shaderSetting.LIL_FEATURE_TEX_REFLECTION_COLOR) reflectionColorTex.textureValue = null;
                if(useMatCap.floatValue == 0.0f) matcapTex.textureValue = null;
                if(useMatCap.floatValue == 0.0f || !shaderSetting.LIL_FEATURE_TEX_MATCAP_MASK) matcapBlendMask.textureValue = null;
                if(useRim.floatValue == 0.0f || !shaderSetting.LIL_FEATURE_TEX_RIMLIGHT_COLOR) rimColorTex.textureValue = null;
                if(useParallax.floatValue == 0.0f) parallaxMap.textureValue = null;
                if(!shaderSetting.LIL_FEATURE_AUDIOLINK) useAudioLink.floatValue = 0.0f;
                if(!shaderSetting.LIL_FEATURE_AUDIOLINK || !shaderSetting.LIL_FEATURE_AUDIOLINK_LOCAL) audioLinkAsLocal.floatValue = 0.0f;
                if(useAudioLink.floatValue == 0.0f || audioLinkUVMode.floatValue != 3.0f || !shaderSetting.LIL_FEATURE_TEX_AUDIOLINK_MASK) audioLinkMask.textureValue = null;
                if(useAudioLink.floatValue == 0.0f || audioLinkAsLocal.floatValue == 0.0f) audioLinkLocalMap.textureValue = null;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Presets
        static void LoadPresets()
        {
            string[] presetGuid = AssetDatabase.FindAssets("t:lilToonPreset", new[] {"Assets/lilToon/Presets"});
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
            string[] sCategorys = { loc["sPresetCategorySkin"],
                                    loc["sPresetCategoryHair"],
                                    loc["sPresetCategoryCloth"],
                                    loc["sPresetCategoryNature"],
                                    loc["sPresetCategoryInorganic"],
                                    loc["sPresetCategoryEffect"],
                                    loc["sPresetCategoryOther"] };
            for(int i=0; i<(int)lilPresetCategory.Other+1; i++)
            {
                isShowCategorys[i] = Foldout(sCategorys[i], "", isShowCategorys[i]);
                if(isShowCategorys[i])
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
                                if(presets[j].bases[k].language == languageName)
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

        static void ApplyPreset(Material material, lilToonPreset preset)
        {
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
            if(!String.IsNullOrEmpty(matPath))  AssetDatabase.CreateAsset(liteMaterial, FileUtil.GetProjectRelativePath(matPath));

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
                Texture2D bakedShadowTex = AutoBakeShadowTexture(material, bakedMainTex);
                liteMaterial.SetTexture("_ShadowColorTex",          bakedShadowTex);
                liteMaterial.SetFloat("_ShadowBorder",              shadowBorder.floatValue);
                liteMaterial.SetFloat("_ShadowBlur",                shadowBlur.floatValue);
                liteMaterial.SetFloat("_ShadowEnvStrength",         shadowEnvStrength.floatValue);
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
            if(bakedTriMask == null) liteMaterial.SetTexture("_TriMask", bakedTriMask);

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
                materialEditor.ShaderProperty(isDecal, loc["sAsDecal"]);
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
                mirrorMode = EditorGUILayout.Popup(loc["sMirrorMode"],mirrorMode,new String[]{loc["sMirrorModeNormal"],loc["sMirrorModeFlip"],loc["sMirrorModeLeft"],loc["sMirrorModeRight"],loc["sMirrorModeRightFlip"]});
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
                copyMode = EditorGUILayout.Popup(loc["sCopyMode"],copyMode,new String[]{loc["sCopyModeNormal"],loc["sCopyModeSymmetry"],loc["sCopyModeFlip"]});
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
                    posX = EditorGUILayout.Slider(loc["sPositionX"], posX, 0.5f, 1.0f);
                }
                else
                {
                    posX = EditorGUILayout.Slider(loc["sPositionX"], posX, 0.0f, 1.0f);
                }

                // Draw properties
                posY = EditorGUILayout.Slider(loc["sPositionY"], posY, 0.0f, 1.0f);
                scaleX = EditorGUILayout.Slider(loc["sScaleX"], scaleX, -1.0f, 1.0f);
                scaleY = EditorGUILayout.Slider(loc["sScaleY"], scaleY, -1.0f, 1.0f);
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

                materialEditor.ShaderProperty(angle, loc["sAngle"]);

                if(shaderSetting.LIL_FEATURE_ANIMATE_DECAL)
                {
                    materialEditor.ShaderProperty(decalAnimation, loc["sAnimation"]+"|"+loc["sXFrames"]+"|"+loc["sYFrames"]+"|"+loc["sFrames"]+"|"+loc["sFPS"]);
                    materialEditor.ShaderProperty(decalSubParam, loc["sXRatio"]+"|"+loc["sYRatio"]+"|"+loc["sFixBorder"]);
                }
            }
            else
            {
                materialEditor.TextureScaleOffsetProperty(tex);
                materialEditor.ShaderProperty(angle, loc["sAngle"]);
            }

            if(GUILayout.Button(loc["sReset"]) && EditorUtility.DisplayDialog(loc["sDialogResetUV"],loc["sDialogResetUVMes"],loc["sYes"],loc["sNo"]))
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
            materialEditor.ShaderProperty(hsvg, loc["sHue"] + "|" + loc["sSaturation"] + "|" + loc["sValue"] + "|" + loc["sGamma"]);

            EditorGUILayout.BeginHorizontal();
            // Bake
            if(isOutline)
            {
                if(GUILayout.Button(loc["sBake"]))
                {
                    outlineTex.textureValue = AutoBakeOutlineTexture(material);
                    outlineTexHSVG.vectorValue = defaultHSVG;
                }
            }
            else
            {
                TextureBakeGUI(material, 4);
            }
            // Reset
            if(GUILayout.Button(loc["sReset"]))
            {
                hsvg.vectorValue = defaultHSVG;
            }
            EditorGUILayout.EndHorizontal();
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
            string[] sBake = {loc["sBakeAll"], loc["sBake1st"], loc["sBake2nd"], loc["sBake3rd"], loc["sBake"], loc["sBake"], loc["sBake"]};
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
            string[] sBake = {loc["sBakeAll"], loc["sBake1st"], loc["sBake2nd"], loc["sBake3rd"], loc["sBake"], loc["sBake"], loc["sBake"]};
            if(GUILayout.Button(sBake[bakeType], guistyle))
            {
                TextureBake(material, bakeType);
            }
        }

        void AlphamaskToTextureGUI(Material material)
        {
            if(mainTex.textureValue != null && GUILayout.Button(loc["sBakeAlphamask"]))
            {
                mainTex.textureValue = AutoBakeAlphaMask(material);
            }
        }

        void SetAlphaIsTransparencyGUI(MaterialProperty tex)
        {
            if(tex.textureValue != null && !((Texture2D)tex.textureValue).alphaIsTransparency)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("This texture is not marked as AlphaIsTransparency", EditorStyles.wordWrappedMiniLabel);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if(GUILayout.Button("Fix Now"))
                {
                    string path = AssetDatabase.GetAssetPath(tex.textureValue);
                    TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(path);
                    textureImporter.alphaIsTransparency = true;
                    AssetDatabase.ImportAsset(path);
                }
                GUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
        }

        void UVSettingGUI(MaterialEditor materialEditor, MaterialProperty uvst, MaterialProperty uvsr)
        {
            EditorGUILayout.LabelField(loc["sUVSetting"], EditorStyles.boldLabel);
            materialEditor.TextureScaleOffsetProperty(uvst);
            materialEditor.ShaderProperty(uvsr, loc["sAngle"] + "|" + loc["sUVAnimation"] + "|" + loc["sScroll"] + "|" + loc["sRotate"]);
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
                materialEditor.ShaderProperty(srcRGB, loc["sSrcBlendRGB"]);
                materialEditor.ShaderProperty(dstRGB, loc["sDstBlendRGB"]);
                materialEditor.ShaderProperty(srcA, loc["sSrcBlendAlpha"]);
                materialEditor.ShaderProperty(dstA, loc["sDstBlendAlpha"]);
                materialEditor.ShaderProperty(opRGB, loc["sBlendOpRGB"]);
                materialEditor.ShaderProperty(opA, loc["sBlendOpAlpha"]);
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
                materialEditor.ShaderProperty(scrollRotate, loc["sScroll"]);
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
                EditorUtility.DisplayDialog(loc["sDialogCannotBake"], loc["sDialogSetMainTex"], loc["sOK"]);
            }
            else if(shouldNotBakeColor)
            {
                EditorUtility.DisplayDialog(loc["sDialogNoNeedBake"], loc["sDialogNoChange"], loc["sOK"]);
            }
            else if(shouldNotBake2nd)
            {
                EditorUtility.DisplayDialog(loc["sDialogNoNeedBake"], loc["sDialogNotUse2nd"], loc["sOK"]);
            }
            else if(shouldNotBake3rd)
            {
                EditorUtility.DisplayDialog(loc["sDialogNoNeedBake"], loc["sDialogNotUse3rd"], loc["sOK"]);
            }
            else if(shouldNotBakeAll)
            {
                EditorUtility.DisplayDialog(loc["sDialogNoNeedBake"], loc["sDialogNotUseAll"], loc["sOK"]);
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
            if(!shouldNotBakeAll && EditorUtility.DisplayDialog(loc["sDialogRunBake"], loc["sDialogBakeMain"], loc["sYes"], loc["sNo"]))
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

        Texture2D AutoBakeShadowTexture(Material material, Texture2D bakedMainTex)
        {
            bool shouldNotBakeAll = useShadow.floatValue == 0.0 && shadowColor.colorValue == Color.white && shadowColorTex.textureValue == null && shadowStrengthMask.textureValue == null;
            if(!shouldNotBakeAll && EditorUtility.DisplayDialog(loc["sDialogRunBake"], loc["sDialogBakeShadow"], loc["sYes"], loc["sNo"]))
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
                hsvgMaterial.SetColor(mainColor2nd.name,                new Color(shadowColor.colorValue.r, shadowColor.colorValue.g, shadowColor.colorValue.b, shadowStrength.floatValue));
                hsvgMaterial.SetFloat(main2ndTexBlendMode.name,         0.0f);
                hsvgMaterial.SetFloat(useMain3rdTex.name,               1.0f);
                hsvgMaterial.SetColor(mainColor3rd.name,                new Color(1.0f,1.0f,1.0f,shadowMainStrength.floatValue));
                hsvgMaterial.SetFloat(main3rdTexBlendMode.name,         3.0f);

                path = AssetDatabase.GetAssetPath(bakedMainTex);
                if(!String.IsNullOrEmpty(path))
                {
                    bytes = File.ReadAllBytes(Path.GetFullPath(path));
                    srcTexture.LoadImage(bytes);
                    srcTexture.filterMode = FilterMode.Bilinear;
                    hsvgMaterial.SetTexture(mainTex.name, srcTexture);
                    hsvgMaterial.SetTexture(main3rdTex.name, srcTexture);
                }
                else
                {
                    hsvgMaterial.SetTexture(mainTex.name, Texture2D.whiteTexture);
                    hsvgMaterial.SetTexture(main3rdTex.name, Texture2D.whiteTexture);
                }

                path = AssetDatabase.GetAssetPath(material.GetTexture(shadowColorTex.name));
                if(!String.IsNullOrEmpty(path))
                {
                    bytes = File.ReadAllBytes(Path.GetFullPath(path));
                    srcMain2.LoadImage(bytes);
                    srcMain2.filterMode = FilterMode.Bilinear;
                    hsvgMaterial.SetTexture(main2ndTex.name, srcMain2);
                }
                else
                {
                    hsvgMaterial.SetTexture(main2ndTex.name, hsvgMaterial.GetTexture(mainTex.name));
                }

                path = AssetDatabase.GetAssetPath(material.GetTexture(shadowStrengthMask.name));
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
            if(!shouldNotBakeAll && EditorUtility.DisplayDialog(loc["sDialogRunBake"], loc["sDialogBakeMatCap"], loc["sYes"], loc["sNo"]))
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
            if(!shouldNotBakeAll && EditorUtility.DisplayDialog(loc["sDialogRunBake"], loc["sDialogBakeTriMask"], loc["sYes"], loc["sNo"]))
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
            Texture2D srcMain2 = new Texture2D(2, 2);

            hsvgMaterial.EnableKeyword("_ALPHAMASK");
            hsvgMaterial.SetColor(mainColor.name,           Color.white);
            hsvgMaterial.SetVector(mainTexHSVG.name,        defaultHSVG);

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

            EditorUtility.DisplayDialog(loc["sBakeAlphamask"],loc["sSelectAlphamask"],loc["sOK"]);
            path = EditorUtility.OpenFilePanel(loc["sSelectAlphamask"], Path.GetDirectoryName(AssetDatabase.GetAssetPath(mainTex.textureValue)), "");
            if(!String.IsNullOrEmpty(path))
            {
                bytes = File.ReadAllBytes(Path.GetFullPath(path));
                srcMain2.LoadImage(bytes);
                srcMain2.filterMode = FilterMode.Bilinear;
                hsvgMaterial.SetTexture(main2ndTex.name, srcMain2);
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
            if(!shouldNotBakeOutline && EditorUtility.DisplayDialog(loc["sDialogRunBake"], loc["sDialogBakeOutline"], loc["sYes"], loc["sNo"]))
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
        void GradientEditor(Material material, string emissionName, Gradient ingrad, string texname)
        {
            ingrad = MaterialToGradient(material, emissionName);
            #if UNITY_2018_1_OR_NEWER
                ingrad = EditorGUILayout.GradientField(loc["sGradColor"], ingrad);
            #else
                MethodInfo setMethod = typeof(EditorGUILayout).GetMethod(
                    "GradientField",
                    BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    new [] {typeof(string), typeof(Gradient), typeof(GUILayoutOption[])},
                    null);
                if(setMethod != null) {
                    ingrad = (Gradient)setMethod.Invoke(null, new object[]{loc["sGradColor"], ingrad, null});;
                }
            #endif
            GradientToMaterial(material, emissionName, ingrad);
            if(GUILayout.Button("Save"))
            {
                Texture2D tex = GradientToTexture(ingrad);
                tex = SaveTextureToPng(material, tex, texname);
                material.SetTexture(texname, tex);
            }
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

        Texture2D GradientToTexture(Gradient grad)
        {
            Texture2D tex = new Texture2D(128, 4);

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
        Texture2D SaveTextureToPng(Material material, Texture2D tex, string texname)
        {
            string path = AssetDatabase.GetAssetPath(material.GetTexture(texname));
            if(String.IsNullOrEmpty(path)) path = AssetDatabase.GetAssetPath(material);

            if(!String.IsNullOrEmpty(path))  path = EditorUtility.SaveFilePanel("Save Texture", Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)+"_2", "png");
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
                string[] sCategorys = { loc["sPresetCategorySkin"],
                                        loc["sPresetCategoryHair"],
                                        loc["sPresetCategoryCloth"],
                                        loc["sPresetCategoryNature"],
                                        loc["sPresetCategoryInorganic"],
                                        loc["sPresetCategoryEffect"],
                                        loc["sPresetCategoryOther"] };
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                Material material = (Material)Selection.activeObject;
                if(preset == null) preset = ScriptableObject.CreateInstance<lilToonPreset>();

                // load language
                string[] langName = languageNames.Split('\t');
                Array.Resize(ref presetName, langName.Length);

                // Initialize
                Array.Resize(ref preset.bases, 0);
                Array.Resize(ref preset.colors, 0);
                Array.Resize(ref preset.vectors, 0);
                Array.Resize(ref preset.floats, 0);
                Array.Resize(ref preset.textures, 0);

                // Name
                EditorGUILayout.LabelField(loc["sPresetName"]);
                for(int i = 0; i < langName.Length; i++)
                {
                    presetName[i] = EditorGUILayout.TextField(langName[i], presetName[i]);
                }

                DrawLine();

                preset.category = (lilPresetCategory)EditorGUILayout.Popup(loc["sPresetCategory"], (int)preset.category, sCategorys);

                // Toggle
                EditorGUILayout.LabelField(loc["sPresetSaveTarget"]);
                shouldSaveShader                = EditorGUILayout.ToggleLeft(loc["sPresetShader"], shouldSaveShader);
                shouldSaveQueue                 = EditorGUILayout.ToggleLeft(loc["sPresetQueue"], shouldSaveQueue);
                shouldSaveStencil               = EditorGUILayout.ToggleLeft(loc["sPresetStencil"], shouldSaveStencil);
                shouldSaveMainTex2Outline       = EditorGUILayout.ToggleLeft(loc["sPresetMainTex2Outline"], shouldSaveMainTex2Outline);

                DrawLine();

                EditorGUILayout.LabelField(loc["sPresetTexture"]);
                shouldSaveMain                  = EditorGUILayout.ToggleLeft(loc["sPresetMain"], shouldSaveMain);
                shouldSaveMain2                 = EditorGUILayout.ToggleLeft(loc["sPresetMain2"], shouldSaveMain2);
                shouldSaveMain2Mask             = EditorGUILayout.ToggleLeft(loc["sPresetMain2Mask"], shouldSaveMain2Mask);
                shouldSaveMain3                 = EditorGUILayout.ToggleLeft(loc["sPresetMain3"], shouldSaveMain3);
                shouldSaveMain3Mask             = EditorGUILayout.ToggleLeft(loc["sPresetMain3Mask"], shouldSaveMain3Mask);

                shouldSaveShadowBorder          = EditorGUILayout.ToggleLeft(loc["sPresetShadowBorder"], shouldSaveShadowBorder);
                shouldSaveShadowBlur            = EditorGUILayout.ToggleLeft(loc["sPresetShadowBlur"], shouldSaveShadowBlur);
                shouldSaveShadowStrength        = EditorGUILayout.ToggleLeft(loc["sPresetShadowStrength"], shouldSaveShadowStrength);
                shouldSaveShadowColor           = EditorGUILayout.ToggleLeft(loc["sPresetShadowColor"], shouldSaveShadowColor);
                shouldSaveShadowColor2          = EditorGUILayout.ToggleLeft(loc["sPresetShadowColor2"], shouldSaveShadowColor2);

                shouldSaveOutlineTex            = EditorGUILayout.ToggleLeft(loc["sPresetOutlineTex"], shouldSaveOutlineTex);
                shouldSaveOutlineWidth          = EditorGUILayout.ToggleLeft(loc["sPresetOutlineWidth"], shouldSaveOutlineWidth);

                shouldSaveEmissionColor         = EditorGUILayout.ToggleLeft(loc["sPresetEmissionColor"], shouldSaveEmissionColor);
                shouldSaveEmissionMask          = EditorGUILayout.ToggleLeft(loc["sPresetEmissionMask"], shouldSaveEmissionMask);
                shouldSaveEmissionGrad          = EditorGUILayout.ToggleLeft(loc["sPresetEmissionGrad"], shouldSaveEmissionGrad);
                shouldSaveEmission2Color        = EditorGUILayout.ToggleLeft(loc["sPresetEmission2Color"], shouldSaveEmission2Color);
                shouldSaveEmission2Mask         = EditorGUILayout.ToggleLeft(loc["sPresetEmission2Mask"], shouldSaveEmission2Mask);
                shouldSaveEmission2Grad         = EditorGUILayout.ToggleLeft(loc["sPresetEmission2Grad"], shouldSaveEmission2Grad);

                shouldSaveNormal                = EditorGUILayout.ToggleLeft(loc["sPresetNormal"], shouldSaveNormal);
                shouldSaveNormal2               = EditorGUILayout.ToggleLeft(loc["sPresetNormal2"], shouldSaveNormal2);
                shouldSaveNormal2Mask           = EditorGUILayout.ToggleLeft(loc["sPresetNormal2Mask"], shouldSaveNormal2Mask);

                shouldSaveReflectionSmoothness  = EditorGUILayout.ToggleLeft(loc["sPresetReflectionSmoothness"], shouldSaveReflectionSmoothness);
                shouldSaveReflectionMetallic     = EditorGUILayout.ToggleLeft(loc["sPresetReflectionMetallic"], shouldSaveReflectionMetallic);
                shouldSaveReflectionColor       = EditorGUILayout.ToggleLeft(loc["sPresetReflectionColor"], shouldSaveReflectionColor);

                shouldSaveMatcap                = EditorGUILayout.ToggleLeft(loc["sPresetMatcap"], shouldSaveMatcap);
                shouldSaveMatcapMask            = EditorGUILayout.ToggleLeft(loc["sPresetMatcapMask"], shouldSaveMatcapMask);

                shouldSaveRim                   = EditorGUILayout.ToggleLeft(loc["sPresetRim"], shouldSaveRim);

                shouldSaveParallax              = EditorGUILayout.ToggleLeft(loc["sPresetParallax"], shouldSaveParallax);

                shouldSaveFurNormal             = EditorGUILayout.ToggleLeft(loc["sPresetFurNormal"], shouldSaveFurNormal);
                shouldSaveFurNoise              = EditorGUILayout.ToggleLeft(loc["sPresetFurNoise"], shouldSaveFurNoise);
                shouldSaveFurMask               = EditorGUILayout.ToggleLeft(loc["sPresetFurMask"], shouldSaveFurMask);

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
                    string savePath = EditorUtility.SaveFilePanel("Save Preset", "Assets/lilToon/Presets/", filename, "asset");
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
                if(tex.textureValue != null && AssetDatabase.GetAssetPath(tex.textureValue).ToLower().Contains(".gif") && GUILayout.Button(loc["sConvertGif"]))
                {
                    string path = AssetDatabase.GetAssetPath(tex.textureValue);
                    System.Drawing.Image origGif = System.Drawing.Image.FromFile(path);
                    System.Drawing.Imaging.FrameDimension dimension = new System.Drawing.Imaging.FrameDimension(origGif.FrameDimensionsList[0]);
                    int frameCount = origGif.GetFrameCount(dimension);
                    int loopXY = Mathf.CeilToInt(Mathf.Sqrt(frameCount));
                    int duration = BitConverter.ToInt32(origGif.GetPropertyItem(20736).Value, 0);
                    int finalWidth = 1;
                    int finalHeight = 1;
                    if(EditorUtility.DisplayDialog(loc["sDialogGifToAtlas"],loc["sDialogTexPow2"],loc["sYes"],loc["sNo"]))
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

    //------------------------------------------------------------------------------------------------------------------------------
    // PropertyDrawer
    public class lilHDRDrawer : MaterialPropertyDrawer
    {
        // Gamma HDR
        // [lilHDR]
        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            Color value = prop.colorValue;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            #if UNITY_2018_1_OR_NEWER
                value = EditorGUI.ColorField(position, new GUIContent(label), value, true, true, true);
            #else
                value = EditorGUI.ColorField(position, new GUIContent(label), value, true, true, true, null);
            #endif
            EditorGUI.showMixedValue = false;

            if(EditorGUI.EndChangeCheck())
            {
                prop.colorValue = value;
            }
        }
    }

    public class lilToggleDrawer : MaterialPropertyDrawer
    {
        // Toggle without setting shader keyword
        // [lilToggle]
        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            bool value = (prop.floatValue != 0.0f);
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            value = EditorGUI.Toggle(position, label, value);
            EditorGUI.showMixedValue = false;

            if(EditorGUI.EndChangeCheck())
            {
                prop.floatValue = value ? 1.0f : 0.0f;
            }
        }
    }

    public class lilToggleLeftDrawer : MaterialPropertyDrawer
    {
        // Toggle without setting shader keyword
        // [lilToggleLeft]
        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            position.width -= 24;
            bool value = (prop.floatValue != 0.0f);
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            #if UNITY_2019_1_OR_NEWER
                value = EditorGUI.ToggleLeft(position, label, value);
            #else
                GUIStyle customToggleFont = new GUIStyle();
                customToggleFont.normal.textColor = Color.white;
                customToggleFont.contentOffset = new Vector2(2f,0f);
                value = EditorGUI.ToggleLeft(position, label, value, customToggleFont);
            #endif
            EditorGUI.showMixedValue = false;

            if(EditorGUI.EndChangeCheck())
            {
                prop.floatValue = value ? 1.0f : 0.0f;
            }
        }
    }

    public class lilAngleDrawer : MaterialPropertyDrawer
    {
        // [lilAngle]
        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            // Radian -> Degree
            float angle180 = prop.floatValue / Mathf.PI * 180.0f;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            angle180 = EditorGUI.Slider(position, label, angle180, -180.0f, 180.0f);
            EditorGUI.showMixedValue = false;

            if(EditorGUI.EndChangeCheck())
            {
                // Degree -> Radian
                prop.floatValue = angle180 * Mathf.PI / 180.0f;
            }
        }
    }

    public class lilBlinkDrawer : MaterialPropertyDrawer
    {
        // [lilBlink]
        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            string[] labels = label.Split('|');
            float strength = prop.vectorValue.x;
            float type = prop.vectorValue.y;
            float speed = prop.vectorValue.z / Mathf.PI;
            float offset = prop.vectorValue.w / Mathf.PI;

            EditorGUI.BeginChangeCheck();
            strength = EditorGUI.Slider(position, labels[0], strength, 0.0f, 1.0f);
            if(strength != 0.0f)
            {
                type    = EditorGUI.Toggle(EditorGUILayout.GetControlRect(), labels[1], type > 0.5f) ? 1.0f : 0.0f;
                speed   = EditorGUI.FloatField(EditorGUILayout.GetControlRect(), labels[2], speed);
                offset  = EditorGUI.FloatField(EditorGUILayout.GetControlRect(), labels[3], offset);
            }

            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(strength, type, speed * Mathf.PI, offset * Mathf.PI);
            }
        }
    }

    public class lilVec3Drawer : MaterialPropertyDrawer
    {
        // Draw vector4 as vector3
        // [lilVec3]
        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            Vector3 vec = new Vector3(prop.vectorValue.x, prop.vectorValue.y, prop.vectorValue.z);
            float unused = prop.vectorValue.w;

            EditorGUIUtility.wideMode = true;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            vec = EditorGUI.Vector3Field(position, label, vec);
            EditorGUI.showMixedValue = false;

            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(vec.x, vec.y, vec.z, unused);
            }
        }
    }

    public class lilVec3FloatDrawer : MaterialPropertyDrawer
    {
        // Draw vector4 as vector3 and float
        // [lilVec3Float]
        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            string[] labels = label.Split('|');
            Vector3 vec = new Vector3(prop.vectorValue.x, prop.vectorValue.y, prop.vectorValue.z);
            float length = prop.vectorValue.w;

            EditorGUIUtility.wideMode = true;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            vec = EditorGUI.Vector3Field(position, labels[0], vec);
            length = EditorGUI.FloatField(EditorGUILayout.GetControlRect(), labels[1], length);
            EditorGUI.showMixedValue = false;

            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(vec.x, vec.y, vec.z, length);
            }
        }
    }

    public class lilHSVGDrawer : MaterialPropertyDrawer
    {
        // Hue Saturation Value Gamma
        // [lilHSVG]
        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            string[] labels = label.Split('|');
            float hue = prop.vectorValue.x;
            float sat = prop.vectorValue.y;
            float val = prop.vectorValue.z;
            float gamma = prop.vectorValue.w;

            EditorGUI.BeginChangeCheck();
            hue = EditorGUI.Slider(position, labels[0], hue, -0.5f, 0.5f);
            sat = EditorGUI.Slider(EditorGUILayout.GetControlRect(), labels[1], sat, 0.0f, 2.0f);
            val = EditorGUI.Slider(EditorGUILayout.GetControlRect(), labels[2], val, 0.0f, 2.0f);
            gamma = EditorGUI.Slider(EditorGUILayout.GetControlRect(), labels[3], gamma, 0.01f, 2.0f);

            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(hue, sat, val, gamma);
            }
        }
    }

    public class lilUVAnim : MaterialPropertyDrawer
    {
        // Angle Scroll Rotate
        // [lilUVAnim]
        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            string[] labels = label.Split('|');
            Vector2 scroll = new Vector2(prop.vectorValue.x, prop.vectorValue.y);
            float angle = prop.vectorValue.z / Mathf.PI * 180.0f;
            float rotate = prop.vectorValue.w / Mathf.PI * 0.5f;

            EditorGUI.BeginChangeCheck();

            if(labels.Length == 1)
            {
                float labelWidth = EditorGUIUtility.labelWidth;
                Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
                EditorGUI.PrefixLabel(labelRect, new GUIContent(labels[0]));
                int indentBuf = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                Rect vecRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, position.height);
                scroll = EditorGUI.Vector2Field(vecRect, GUIContent.none, scroll);
                EditorGUI.indentLevel = indentBuf;
            }
            else
            {
                // Angle
                angle = EditorGUI.Slider(position, labels[0], angle, -180.0f, 180.0f);

                #if UNITY_2019_1_OR_NEWER
                    Color lineColor = new Color(0.35f,0.35f,0.35f,1.0f);
                #else
                    Color lineColor = new Color(0.4f,0.4f,0.4f,1.0f);
                #endif
                EditorGUI.DrawRect(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, 1)), lineColor);

                // Heading (UV Animation)
                EditorGUILayout.LabelField(labels[1], EditorStyles.boldLabel);

                Rect positionVec2 = EditorGUILayout.GetControlRect();

                // Scroll label
                float labelWidth = EditorGUIUtility.labelWidth;
                Rect labelRect = new Rect(positionVec2.x, positionVec2.y, labelWidth, positionVec2.height);
                EditorGUI.PrefixLabel(labelRect, new GUIContent(labels[2]));

                // Copy & Reset indent
                int indentBuf = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                // Scroll
                Rect vecRect = new Rect(positionVec2.x + labelWidth, positionVec2.y, positionVec2.width - labelWidth, positionVec2.height);
                scroll = EditorGUI.Vector2Field(vecRect, GUIContent.none, scroll);

                // Revert indent
                EditorGUI.indentLevel = indentBuf;

                // Rotate
                rotate = EditorGUI.FloatField(EditorGUILayout.GetControlRect(), labels[3], rotate);
            }

            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(scroll.x, scroll.y, angle * Mathf.PI / 180.0f, rotate * Mathf.PI * 2.0f);
            }
        }
    }

    public class lilDecalAnim : MaterialPropertyDrawer
    {
        // [lilDecalAnim]
        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            string[] labels = label.Split('|');
            int loopX = (int)prop.vectorValue.x;
            int loopY = (int)prop.vectorValue.y;
            int frames = (int)prop.vectorValue.z;
            float speed = prop.vectorValue.w;

            // Heading (UV Animation)
            EditorGUI.LabelField(position, labels[0], EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            Rect position1 = EditorGUILayout.GetControlRect();
            Rect position2 = EditorGUILayout.GetControlRect();
            Rect position3 = EditorGUILayout.GetControlRect();
            Rect position4 = EditorGUILayout.GetControlRect();

            EditorGUI.BeginChangeCheck();
            loopX = EditorGUI.IntField(position1, labels[1], loopX);
            loopY = EditorGUI.IntField(position2, labels[2], loopY);
            frames = EditorGUI.IntField(position3, labels[3], frames);
            speed = EditorGUI.FloatField(position4, labels[4], speed);
            EditorGUI.indentLevel--;

            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4((float)loopX, (float)loopY, (float)frames, speed);
            }
        }
    }

    public class lilDecalSub : MaterialPropertyDrawer
    {
        // [lilDecalSub]
        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            string[] labels = label.Split('|');
            float scaleX = prop.vectorValue.x;
            float scaleY = prop.vectorValue.y;
            float border = prop.vectorValue.z;
            float unused = prop.vectorValue.w;

            EditorGUI.indentLevel++;
            Rect position1 = EditorGUILayout.GetControlRect();
            Rect position2 = EditorGUILayout.GetControlRect();

            EditorGUI.BeginChangeCheck();
            scaleX = EditorGUI.Slider(position, labels[0], scaleX, 0.0f, 1.0f);
            scaleY = EditorGUI.Slider(position1, labels[1], scaleY, 0.0f, 1.0f);
            border = EditorGUI.Slider(position2, labels[2], border, 0.0f, 1.0f);
            EditorGUI.indentLevel--;

            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(scaleX, scaleY, border, unused);
            }
        }
    }

    public class lilCullMode : MaterialPropertyDrawer
    {
        // CullMode (Off Front Back)
        // [lilCullMode]
        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            string[] labels = label.Split('|');

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            float cullFloat = (float)EditorGUI.Popup(position, labels[0], (int)prop.floatValue, new String[]{labels[1],labels[2],labels[3]});
            EditorGUI.showMixedValue = false;

            if(EditorGUI.EndChangeCheck())
            {
                prop.floatValue = cullFloat;
            }
        }
    }

    public class lilBlendMode : MaterialPropertyDrawer
    {
        // BlendMode (Normal Add Screen Multiply)
        // [lilBlendMode]
        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            string[] labels = label.Split('|');

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            float blendFloat = (float)EditorGUI.Popup(position, labels[0], (int)prop.floatValue, new String[]{labels[1],labels[2],labels[3],labels[4]});
            EditorGUI.showMixedValue = false;

            if(EditorGUI.EndChangeCheck())
            {
                prop.floatValue = blendFloat;
            }
        }
    }

    public class lilColorMask : MaterialPropertyDrawer
    {
        // ColorMask
        // [lilColorMask]
        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            string[] masks = new string[]{"None","A","B","BA","G","GA","GB","GBA","R","RA","RB","RBA","RG","RGA","RGB","RGBA"};
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            float cullFloat = (float)EditorGUI.Popup(position, label, (int)prop.floatValue, masks);
            EditorGUI.showMixedValue = false;

            if(EditorGUI.EndChangeCheck())
            {
                prop.floatValue = cullFloat;
            }
        }
    }

    public class lil3Param : MaterialPropertyDrawer
    {
        // [lil3Param]
        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            string[] labels = label.Split('|');
            float param1 = prop.vectorValue.x;
            float param2 = prop.vectorValue.y;
            float param3 = prop.vectorValue.z;
            float unused = prop.vectorValue.w;

            EditorGUI.indentLevel++;
            Rect position1 = EditorGUILayout.GetControlRect();
            Rect position2 = EditorGUILayout.GetControlRect();

            EditorGUI.BeginChangeCheck();
            param1 = EditorGUI.FloatField(position, labels[0], param1);
            param2 = EditorGUI.FloatField(position1, labels[1], param2);
            param3 = EditorGUI.FloatField(position2, labels[2], param3);
            EditorGUI.indentLevel--;

            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(param1, param2, param3, unused);
            }
        }
    }

    public class lilALUVMode : MaterialPropertyDrawer
    {
        // [lilALUVMode]
        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            string[] labels = label.Split('|');
            float value = prop.floatValue;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            if(labels.Length == 5) value = (float)EditorGUI.Popup(position, labels[0], (int)prop.floatValue, new String[]{labels[1],labels[2],labels[3],labels[4]});
            if(labels.Length == 4) value = (float)EditorGUI.Popup(position, labels[0], (int)prop.floatValue, new String[]{labels[1],labels[2],labels[3]});
            EditorGUI.showMixedValue = false;

            if(EditorGUI.EndChangeCheck())
            {
                prop.floatValue = value;
            }
        }
    }

    public class lilALUVParams : MaterialPropertyDrawer
    {
        // [lilALUVParams]
        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            string[] labels = label.Split('|');
            float scale = prop.vectorValue.x;
            float offset = prop.vectorValue.y;
            float angle180 = prop.vectorValue.z / Mathf.PI * 180.0f;
            float band = (prop.vectorValue.w - 0.125f) * 4.0f;

            EditorGUI.BeginChangeCheck();
            if(labels.Length == 6)
            {
                Rect position1 = EditorGUILayout.GetControlRect();
                offset = EditorGUI.FloatField(position, labels[0], offset);
                band = (float)EditorGUI.Popup(position1, labels[1], (int)band, new String[]{labels[2],labels[3],labels[4],labels[5]});
            }
            if(labels.Length == 7)
            {
                Rect position1 = EditorGUILayout.GetControlRect();
                Rect position2 = EditorGUILayout.GetControlRect();
                scale = EditorGUI.FloatField(position, labels[0], scale);
                offset = EditorGUI.FloatField(position1, labels[1], offset);
                band = (float)EditorGUI.Popup(position2, labels[2], (int)band, new String[]{labels[3],labels[4],labels[5],labels[6]});
            }
            if(labels.Length == 8)
            {
                Rect position1 = EditorGUILayout.GetControlRect();
                Rect position2 = EditorGUILayout.GetControlRect();
                Rect position3 = EditorGUILayout.GetControlRect();
                scale = EditorGUI.FloatField(position, labels[0], scale);
                offset = EditorGUI.FloatField(position1, labels[1], offset);
                angle180 = EditorGUI.Slider(position2, labels[2], angle180, -180.0f, 180.0f);
                band = (float)EditorGUI.Popup(position3, labels[3], (int)band, new String[]{labels[4],labels[5],labels[6],labels[7]});
            }

            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(scale, offset, angle180 * Mathf.PI / 180.0f, band / 4.0f + 0.125f);
            }
        }
    }

    public class lilALLocal : MaterialPropertyDrawer
    {
        // [lilALLocal]
        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            string[] labels = label.Split('|');
            float BPM = prop.vectorValue.x;
            float Notes = prop.vectorValue.y;
            float Offset = prop.vectorValue.z;
            float unused = prop.vectorValue.w;

            EditorGUI.indentLevel++;
            Rect position1 = EditorGUILayout.GetControlRect();
            Rect position2 = EditorGUILayout.GetControlRect();

            EditorGUI.BeginChangeCheck();
            BPM = EditorGUI.FloatField(position, labels[0], BPM);
            Notes = EditorGUI.FloatField(position1, labels[1], Notes);
            Offset = EditorGUI.FloatField(position2, labels[2], Offset);
            EditorGUI.indentLevel--;

            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(BPM, Notes, Offset, unused);
            }
        }
    }

    public class lilDissolve : MaterialPropertyDrawer
    {
        // [lilDissolve]
        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            string[] labels = label.Split('|');
            float type = prop.vectorValue.x;
            float shape = prop.vectorValue.y;
            float border = prop.vectorValue.z;
            float blur = prop.vectorValue.w;

            EditorGUI.BeginChangeCheck();
            if(labels.Length == 5)
            {
                #if UNITY_2019_1_OR_NEWER
                    type = (float)EditorGUI.Popup(position, labels[0], (int)type, new String[]{labels[1],labels[2],labels[3],labels[4]});
                #else
                    GUIStyle customToggleFont = new GUIStyle();
                    customToggleFont.normal.textColor = Color.white;
                    customToggleFont.contentOffset = new Vector2(2f,0f);
                    float labelWidth = EditorGUIUtility.labelWidth;
                    Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
                    EditorGUI.PrefixLabel(labelRect, new GUIContent(labels[0]), customToggleFont);
                    type = (float)EditorGUI.Popup(position, " ", (int)type, new String[]{labels[1],labels[2],labels[3],labels[4]});
                #endif
            }
            if(labels.Length == 6)
            {
                if(type == 1.0f)
                {
                    Rect position1 = EditorGUILayout.GetControlRect();
                    border = EditorGUI.Slider(position, labels[3], border, -1.0f, 2.0f);
                    blur = EditorGUI.Slider(position1, labels[4], blur, 0.0f, 1.0f);
                }
                if(type == 2.0f)
                {
                    Rect position1 = EditorGUILayout.GetControlRect();
                    Rect position2 = EditorGUILayout.GetControlRect();
                    shape = (float)EditorGUI.Popup(position, labels[0], (int)shape, new String[]{labels[1],labels[2]});
                    border = EditorGUI.FloatField(position1, labels[3], border);
                    blur = EditorGUI.FloatField(position2, labels[4], blur);
                }
                if(type == 3.0f)
                {
                    Rect position1 = EditorGUILayout.GetControlRect();
                    Rect position2 = EditorGUILayout.GetControlRect();
                    shape = (float)EditorGUI.Popup(position, labels[0], (int)shape, new String[]{labels[1],labels[2]});
                    border = EditorGUI.FloatField(position1, labels[3], border);
                    blur = EditorGUI.FloatField(position2, labels[4], blur);
                }
            }
            if(labels.Length == 10)
            {
                type = (float)EditorGUI.Popup(position, labels[0], (int)type, new String[]{labels[1],labels[2],labels[3],labels[4]});
                if(type == 1.0f)
                {
                    Rect position1 = EditorGUILayout.GetControlRect();
                    Rect position2 = EditorGUILayout.GetControlRect();
                    border = EditorGUI.Slider(position1, labels[8], border, -1.0f, 2.0f);
                    blur = EditorGUI.Slider(position2, labels[9], blur, 0.0f, 1.0f);
                }
                if(type == 2.0f)
                {
                    Rect position1 = EditorGUILayout.GetControlRect();
                    Rect position2 = EditorGUILayout.GetControlRect();
                    Rect position3 = EditorGUILayout.GetControlRect();
                    shape = (float)EditorGUI.Popup(position1, labels[5], (int)shape, new String[]{labels[6],labels[7]});
                    border = EditorGUI.FloatField(position2, labels[8], border);
                    blur = EditorGUI.FloatField(position3, labels[9], blur);
                }
                if(type == 3.0f)
                {
                    Rect position1 = EditorGUILayout.GetControlRect();
                    Rect position2 = EditorGUILayout.GetControlRect();
                    Rect position3 = EditorGUILayout.GetControlRect();
                    shape = (float)EditorGUI.Popup(position1, labels[5], (int)shape, new String[]{labels[6],labels[7]});
                    border = EditorGUI.FloatField(position2, labels[8], border);
                    blur = EditorGUI.FloatField(position3, labels[9], blur);
                }
            }

            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(type, shape, border, blur);
            }
        }
    }

    public class lilDissolveP : MaterialPropertyDrawer
    {
        // [lilDissolveP]
        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
        {
            string[] labels = label.Split('|');

            if(labels.Length == 2)
            {
                int type = int.Parse(labels[1]);
                if(type == 1)
                {
                    EditorGUI.BeginChangeCheck();
                    float value = EditorGUI.FloatField(position, labels[0], prop.vectorValue.x);
                    if(EditorGUI.EndChangeCheck())
                    {
                        prop.vectorValue = new Vector4(value, prop.vectorValue.y, prop.vectorValue.z, prop.vectorValue.w);
                    }
                }
                if(type == 2)
                {
                    EditorGUI.BeginChangeCheck();
                    Vector2 vec = EditorGUI.Vector2Field(position, labels[0], new Vector2(prop.vectorValue.x, prop.vectorValue.y));
                    if(EditorGUI.EndChangeCheck())
                    {
                        prop.vectorValue = new Vector4(vec.x, vec.y, prop.vectorValue.z, prop.vectorValue.w);
                    }
                }
                if(type == 3)
                {
                    EditorGUI.BeginChangeCheck();
                    Vector3 vec = EditorGUI.Vector3Field(position, labels[0], new Vector3(prop.vectorValue.x, prop.vectorValue.y, prop.vectorValue.z));
                    if(EditorGUI.EndChangeCheck())
                    {
                        prop.vectorValue = new Vector4(vec.x, vec.y, vec.z, prop.vectorValue.w);
                    }
                }
                if(type == 3)
                {
                    EditorGUI.BeginChangeCheck();
                    Vector4 vec = EditorGUI.Vector4Field(position, labels[0], prop.vectorValue);
                    if(EditorGUI.EndChangeCheck())
                    {
                        prop.vectorValue = vec;
                    }
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                Vector4 vec = EditorGUI.Vector4Field(position, labels[0], prop.vectorValue);
                if(EditorGUI.EndChangeCheck())
                {
                    prop.vectorValue = vec;
                }
            }
        }
    }

    //------------------------------------------------------------------------------------------------------------------------------
    // MenuItem
    #if SYSTEM_DRAWING
        public class lilGifToAtlas : MonoBehaviour
        {
            // Gif to Atlas
            [MenuItem("Assets/lilToon/Convert Gif to Atlas")]
            static void ConvertGifToAtlas()
            {
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                System.Drawing.Image origGif = System.Drawing.Image.FromFile(path);
                System.Drawing.Imaging.FrameDimension dimension = new System.Drawing.Imaging.FrameDimension(origGif.FrameDimensionsList[0]);
                int frameCount = origGif.GetFrameCount(dimension);
                int loopXY = Mathf.CeilToInt(Mathf.Sqrt(frameCount));
                int finalWidth = 1;
                int finalHeight = 1;
                if(EditorUtility.DisplayDialog("Convert Gif to Atlas","Do you want to adjust the texture resolution to power of 2?","Yes","No"))
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
                int duration = BitConverter.ToInt32(origGif.GetPropertyItem(20736).Value, 0);
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
            }

            [MenuItem("Assets/lilToon/Convert Gif to Atlas", true, 20)]
            static bool CheckFormat()
            {
                if(Selection.activeObject == null) return false;
                string path = AssetDatabase.GetAssetPath(Selection.activeObject).ToLower();
                return path.EndsWith(".gif");
            }
        }
    #endif

    public class lilDotTextureReduction : MonoBehaviour
    {
        // Dot Texture reduction
        [MenuItem("Assets/lilToon/Dot Texture reduction")]
        static void DotTextureReduction()
        {
            Texture2D srcTexture = new Texture2D(2, 2);
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            byte[] bytes = File.ReadAllBytes(Path.GetFullPath(path));
            srcTexture.LoadImage(bytes);
            int finalWidth = 0;
            int finalHeight = 0;
            int scale = 0;
            if(EditorUtility.DisplayDialog("Dot Texture reduction","Select a reduction ratio","1/2","1/4"))
            {
                finalWidth = srcTexture.width / 2;
                finalHeight = srcTexture.height / 2;
                scale = 2;
            }
            else
            {
                finalWidth = srcTexture.width / 4;
                finalHeight = srcTexture.height / 4;
                scale = 4;
            }
            Texture2D outTex = new Texture2D(finalWidth, finalHeight);
            for(int x = 0; x < finalWidth; x++)
            {
                for(int y = 0; y < finalHeight; y++)
                {
                    outTex.SetPixel(x, y, srcTexture.GetPixel(x*scale, y*scale));
                }
            }
            outTex.Apply();
            
            // Save
            string savePath = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + "_resized" + ".png";
            File.WriteAllBytes(savePath, outTex.EncodeToPNG());
            AssetDatabase.Refresh();
            TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(savePath);
            textureImporter.filterMode = FilterMode.Point;
            AssetDatabase.ImportAsset(savePath);
        }

        [MenuItem("Assets/lilToon/Dot Texture reduction", true, 20)]
        static bool CheckFormat()
        {
            if(Selection.activeObject == null) return false;
            string path = AssetDatabase.GetAssetPath(Selection.activeObject).ToLower();
            return path.EndsWith(".png");
        }
    }

    public class lilConvertNormal : MonoBehaviour
    {
        // Normal Map DirectX <-> OpenGL
        [MenuItem("Assets/lilToon/Convert Normal Map (DirectX <-> OpenGL)")]
        static void ConvertNormal()
        {
            Texture2D srcTexture = new Texture2D(2, 2, TextureFormat.RGBA32, true, true);
            Material hsvgMaterial = new Material(Shader.Find("Hidden/ltsother_baker"));
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            byte[] bytes = File.ReadAllBytes(Path.GetFullPath(path));
            srcTexture.LoadImage(bytes);
            hsvgMaterial.SetTexture("_MainTex", srcTexture);
            hsvgMaterial.EnableKeyword("_NORMAL_DXGL");

            RenderTexture dstTexture = new RenderTexture(srcTexture.width, srcTexture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

            Graphics.Blit(srcTexture, dstTexture, hsvgMaterial);

            Texture2D outTexture = new Texture2D(srcTexture.width, srcTexture.height, TextureFormat.RGBA32, true, true);
            outTexture.ReadPixels(new Rect(0, 0, srcTexture.width, srcTexture.height), 0, 0);
            outTexture.Apply();

            // Save
            string savePath = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + "_conv" + ".png";
            File.WriteAllBytes(savePath, outTexture.EncodeToPNG());
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(savePath);

            UnityEngine.Object.DestroyImmediate(hsvgMaterial);
            UnityEngine.Object.DestroyImmediate(srcTexture);
            UnityEngine.Object.DestroyImmediate(dstTexture);
        }

        [MenuItem("Assets/lilToon/Convert Normal Map (DirectX <-> OpenGL)", true, 20)]
        static bool CheckFormat()
        {
            if(Selection.activeObject == null) return false;
            string path = AssetDatabase.GetAssetPath(Selection.activeObject).ToLower();
            return path.EndsWith(".png");
        }
    }
}
#endif