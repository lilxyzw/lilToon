#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using lilToon;
using UnityEditor;
using UnityEngine;

public class lilToonPreset : ScriptableObject
{
    public lilPresetBase[] bases;
    public lilPresetCategory category;
    public string renderingMode;
    public Shader shader;
    public lilPresetColor[] colors;
    public lilPresetVector4[] vectors;
    public lilPresetFloat[] floats;
    public lilPresetTexture[] textures;
    public int renderQueue;
    public int outline;
    public bool outlineMainTex;
    public int tessellation;

    [System.Serializable]
    public struct lilPresetBase
    {
        public string language;
        public string name;
    }

    [System.Serializable]
    public struct lilPresetColor
    {
        public string name;
        public Color value;
    }

    [System.Serializable]
    public struct lilPresetVector4
    {
        public string name;
        public Vector4 value;
    }

    [System.Serializable]
    public struct lilPresetFloat
    {
        public string name;
        public float value;
    }

    [System.Serializable]
    public struct lilPresetTexture
    {
        public string name;
        public Texture value;
        public Vector2 offset;
        public Vector2 scale;
    }

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

        bool islite         = material.shader.name.Contains("Lite");
        bool iscutout       = material.shader.name.Contains("Cutout");
        bool istransparent  = material.shader.name.Contains("Transparent");
        bool isrefr         = material.shader.name.Contains("Refraction");
        bool isblur         = material.shader.name.Contains("Blur");
        bool isfur          = material.shader.name.Contains("Fur");
        bool isonepass      = material.shader.name.Contains("OnePass");
        bool istwopass      = material.shader.name.Contains("TwoPass");

        RenderingMode           renderingMode = RenderingMode.Opaque;

        //if(string.IsNullOrEmpty(preset.renderingMode) || !Enum.TryParse(preset.renderingMode, out renderingMode))
        if(string.IsNullOrEmpty(preset.renderingMode) || !Enum.IsDefined(typeof(RenderingMode), preset.renderingMode))
        {
            if(iscutout)            renderingMode = RenderingMode.Cutout;
            if(istransparent)       renderingMode = RenderingMode.Transparent;
            if(isrefr)              renderingMode = RenderingMode.Refraction;
            if(isrefr && isblur)    renderingMode = RenderingMode.RefractionBlur;
            if(isfur)               renderingMode = RenderingMode.Fur;
            if(isfur && iscutout)   renderingMode = RenderingMode.FurCutout;
            if(isfur && istwopass)  renderingMode = RenderingMode.FurTwoPass;
        }
        else
        {
            renderingMode = (RenderingMode)Enum.Parse(typeof(RenderingMode), preset.renderingMode);
        }

        TransparentMode         transparentMode = TransparentMode.Normal;
        if(isonepass)           transparentMode = TransparentMode.OnePass;
        if(!isfur && istwopass) transparentMode = TransparentMode.TwoPass;

        lilMaterialUtils.SetupMaterialWithRenderingMode(material, renderingMode, transparentMode, isoutl, islite, istess, ismulti);
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

    public static lilToonPreset[] LoadPresets()
    {
        string[] presetGuid = AssetDatabase.FindAssets("t:lilToonPreset");
        var presetList = new List<lilToonPreset>();
        for(int i=0; i<presetGuid.Length; i++)
        {
            presetList.Add(AssetDatabase.LoadAssetAtPath<lilToonPreset>(lilDirectoryManager.GUIDToPath(presetGuid[i])));
        }
        return presetList.ToArray();
    }

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
        private bool isOutl        = false;
        private bool isTess        = false;
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
            string[] langName = lilLanguageManager.langSet.languageNames.Split('\t');
            Array.Resize(ref presetName, langName.Length);

            // Initialize
            Array.Resize(ref preset.bases, 0);
            Array.Resize(ref preset.colors, 0);
            Array.Resize(ref preset.vectors, 0);
            Array.Resize(ref preset.floats, 0);
            Array.Resize(ref preset.textures, 0);
            if(material.shader != null && !string.IsNullOrEmpty(material.shader.name))
            {
                isOutl        = material.shader.name.Contains("Outline");
                isTess        = material.shader.name.Contains("Tessellation");
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
                isOutl        = false;
                isTess        = false;
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
            isShowFeatures = lilEditorGUI.DrawSimpleFoldout(GetLoc("sPresetSaveTarget"), isShowFeatures);
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
            isShowTextures = lilEditorGUI.DrawSimpleFoldout(GetLoc("sPresetTexture"), isShowTextures);
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
                string savePath = EditorUtility.SaveFilePanel("Save Preset", lilDirectoryManager.GetPresetsFolderPath(), filename, "asset");
                if(!string.IsNullOrEmpty(savePath))
                {
                    AssetDatabase.CreateAsset(preset, FileUtil.GetProjectRelativePath(savePath));
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    AssetDatabase.ImportAsset(FileUtil.GetProjectRelativePath(savePath), ImportAssetOptions.ForceUpdate);
                    lilToonInspector.presets = LoadPresets();
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

        public static string GetLoc(string value) { return lilLanguageManager.GetLoc(value); }
    }
    #endregion
}

namespace lilToon
{
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
}
#endif