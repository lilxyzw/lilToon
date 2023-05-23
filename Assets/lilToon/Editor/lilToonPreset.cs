#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
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

    [Serializable]
    public struct lilPresetBase
    {
        public string language;
        public string name;
    }

    [Serializable]
    public struct lilPresetColor
    {
        public string name;
        public Color value;
    }

    [Serializable]
    public struct lilPresetVector4
    {
        public string name;
        public Vector4 value;
    }

    [Serializable]
    public struct lilPresetFloat
    {
        public string name;
        public float value;
    }

    [Serializable]
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
        foreach(var f in preset.floats.Where(f => f.name == "_StencilPass"))
        {
            material.SetFloat(f.name, f.value);
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

        var renderingMode = RenderingMode.Opaque;

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

        var                     transparentMode = TransparentMode.Normal;
        if(isonepass)           transparentMode = TransparentMode.OnePass;
        if(!isfur && istwopass) transparentMode = TransparentMode.TwoPass;

        lilMaterialUtils.SetupMaterialWithRenderingMode(material, renderingMode, transparentMode, isoutl, islite, istess, ismulti);
        if(preset.renderQueue != -2) material.renderQueue = preset.renderQueue;

        foreach(var c in preset.colors ) material.SetColor(c.name, c.value);
        foreach(var v in preset.vectors) material.SetVector(v.name, v.value);
        foreach(var f in preset.floats ) material.SetFloat(f.name, f.value);
        foreach(var t in preset.textures)
        {
            material.SetTexture(t.name, t.value);
            material.SetTextureOffset(t.name, t.offset);
            material.SetTextureScale(t.name, t.scale);
        }

        if(preset.outlineMainTex) material.SetTexture("_OutlineTex", material.GetTexture("_MainTex"));
    }

    public static lilToonPreset[] LoadPresets()
    {
        return lilDirectoryManager.FindAssets<lilToonPreset>("t:lilToonPreset").ToArray();
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
        private HashSet<string> shouldSaveTexs = new HashSet<string>();

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

            var material = (Material)Selection.activeObject;
            if(preset == null) preset = CreateInstance<lilToonPreset>();

            // load language
            var langName = lilLanguageManager.langSet.languageNames.Split('\t');
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
                if(GUILayout.Button("Select All")) ToggleAllTextures(material, true);
                if(GUILayout.Button("Deselect All")) ToggleAllTextures(material, false);
                EditorGUILayout.EndHorizontal();

                int propCount = ShaderUtil.GetPropertyCount(material.shader);
                for(int i = 0; i < propCount; i++)
                {
                    var propType = ShaderUtil.GetPropertyType(material.shader, i);
                    if(propType != ShaderUtil.ShaderPropertyType.TexEnv) continue;

                    string propName = ShaderUtil.GetPropertyName(material.shader, i);
                    bool shouldSave = shouldSaveTexs.Contains(propName);
                    EditorGUI.BeginChangeCheck();
                    shouldSave = EditorGUILayout.ToggleLeft(propName, shouldSave);
                    if(EditorGUI.EndChangeCheck())
                    {
                        if(shouldSave) shouldSaveTexs.Add(propName);
                        else           shouldSaveTexs.Remove(propName);
                    }
                }
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
                string presetFolderPath = lilDirectoryManager.GetPresetsFolderPath();
                if(presetFolderPath.Contains("Packages")) presetFolderPath = "Assets/";
                string savePath = EditorUtility.SaveFilePanel("Save Preset", presetFolderPath, filename, "asset");
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

                var propType = ShaderUtil.GetPropertyType(material.shader, i);
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
            foreach(var name in shouldSaveTexs)
            {
                CopyTextureToPreset(material, name);
            }
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

        private void ToggleAllTextures(Material material, bool val)
        {
            int propCount = ShaderUtil.GetPropertyCount(material.shader);
            for(int i = 0; i < propCount; i++)
            {
                var propType = ShaderUtil.GetPropertyType(material.shader, i);
                if(propType != ShaderUtil.ShaderPropertyType.TexEnv) continue;

                string propName = ShaderUtil.GetPropertyName(material.shader, i);
                if(val && !shouldSaveTexs.Contains(propName)) shouldSaveTexs.Add(propName);
                if(!val && shouldSaveTexs.Contains(propName)) shouldSaveTexs.Remove(propName);
            }
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