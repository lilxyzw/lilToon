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
    public partial class lilToonInspector : ShaderGUI
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
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Main GUI
        #region
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            isCustomEditor = false;
            isMultiVariants = false;
            materials = materialEditor.targets.Select(t => t as Material).Where(m => m != null).ToArray();
            DrawAllGUI(materialEditor, props, (Material)materialEditor.target);
        }

        public void SetMaterials(Material[] materials2)
        {
            materials = materials2;
        }

        public void DrawAllGUI(MaterialEditor materialEditor, MaterialProperty[] props, Material material)
        {
            // workaround for Unity bug (https://issuetracker.unity3d.com/issues/uv1-data-is-lost-during-assetbundle-build-when-optimize-mesh-data-is-on)
            #if UNITY_2021_1_OR_NEWER
            if(PlayerSettings.stripUnusedMeshComponents && lilEditorGUI.AutoFixHelpBox(GetLoc("sWarnOptimiseMeshData")))
            {
                PlayerSettings.stripUnusedMeshComponents = false;
            }
            #endif

            //------------------------------------------------------------------------------------------------------------------------------
            // EditorAssets
            lilEditorGUI.InitializeGUIStyles();

            //------------------------------------------------------------------------------------------------------------------------------
            // Initialize Setting
            m_MaterialEditor = materialEditor;
            lilShaderManager.InitializeShaders();
            lilToonSetting.InitializeShaderSetting(ref shaderSetting);

            //------------------------------------------------------------------------------------------------------------------------------
            // Load Properties
            SetProperties(props);

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
                if(mainColor != null && baseColor    != null && !mainColor.hasMixedValue) baseColor.colorValue      = mainColor.colorValue;
                if(mainTex   != null && baseMap      != null && !mainTex.hasMixedValue  ) baseMap.textureValue      = mainTex.textureValue;
                if(mainTex   != null && baseColorMap != null && !mainTex.hasMixedValue  ) baseColorMap.textureValue = mainTex.textureValue;

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
                var window = (lilMaterialEditor)GetWindow(typeof(lilMaterialEditor), false, "[Beta] lilToon Multi-Editor");
                window.Show();
            }

            private void OnGUI()
            {
                var materials = Selection.GetFiltered<Material>(SelectionMode.DeepAssets).Where(m => m.shader != null).Where(m => m.shader.name.Contains("lilToon")).ToArray();
                if(materials.Length == 0) return;

                props = MaterialEditor.GetMaterialProperties(materials);
                if(props == null) return;

                material = materials[0];
                isCustomEditor = true;
                isMultiVariants = materials.Any(m => m.shader != material.shader);
                materialEditor = (MaterialEditor)Editor.CreateEditor(materials, typeof(MaterialEditor));
                var inspector = new lilToonInspector();

                EditorGUILayout.LabelField("Selected Materials", string.Join(", ", materials.Select(m => m.name).ToArray()), EditorStyles.boldLabel);
                lilEditorGUI.DrawLine();
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                EditorGUILayout.BeginVertical(InitializeMarginBox(20, 4, 4));
                inspector.SetMaterials(materials);
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
    }
}
#endif
