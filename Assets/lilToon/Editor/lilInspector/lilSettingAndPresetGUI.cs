#if !LILTOON_VRCSDK3_AVATARS && !LILTOON_VRCSDK3_WORLDS && VRC_SDK_VRCSDK3
    #if UDON
        #define LILTOON_VRCSDK3_WORLDS
    #else
        #define LILTOON_VRCSDK3_AVATARS
    #endif
#endif
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
        #region
        private void DrawPresetGUI()
        {
            if(isLite)  EditorGUILayout.LabelField(GetLoc("sPresetsNotAvailable"), wrapLabel);
            else        DrawPreset();
        }

        private void DrawSettingsGUI()
        {
            var applyButton = new GUIStyle(GUI.skin.button);
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


            #if LILTOON_VRCSDK3_AVATARS
                EditorGUI.BeginChangeCheck();
                GUI.enabled = !isLocked;
                ToggleGUI(GetLoc("sShaderSettingOptimizeInTestBuild"), ref shaderSetting.isOptimizeInTestBuild);
                GUI.enabled = true;
                if(EditorGUI.EndChangeCheck()) lilToonSetting.SaveShaderSetting(shaderSetting);
            #endif

            EditorGUI.BeginChangeCheck();
            ToggleGUI("Optimize in NDMF (Apply on Play)", ref shaderSetting.isOptimizeInNDMF);
            ToggleGUI(GetLoc("sShaderSettingOptimizeInEditor"), ref shaderSetting.isDebugOptimize);
            ToggleGUI("Migrate materials in startup", ref shaderSetting.isMigrateInStartUp);
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
        // Shader Setting GUI
        #region
        private static void ShaderSettingOptimizationGUI()
        {
            GUI.enabled = !File.Exists(lilDirectoryManager.GetSettingLockPath());
            var RP = lilRenderPipelineReader.GetRP();
            if(RP == lilRenderPipeline.BRP)
            {
                ToggleGUI(GetLoc("sSettingApplyShadowFA"), ref shaderSetting.LIL_OPTIMIZE_APPLY_SHADOW_FA);
                ToggleGUI(GetLoc("sSettingUseForwardAdd"), ref shaderSetting.LIL_OPTIMIZE_USE_FORWARDADD);
                ToggleGUI(GetLoc("sSettingUseForwardAddShadow"), ref shaderSetting.LIL_OPTIMIZE_USE_FORWARDADD_SHADOW);
            }
            ToggleGUI(GetLoc("sSettingUseLightmap"), ref shaderSetting.LIL_OPTIMIZE_USE_LIGHTMAP);
            if(RP == lilRenderPipeline.BRP) ToggleGUI("Fix for Deffered", ref shaderSetting.LIL_OPTIMIZE_DEFFERED);
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
            if(lilEditorGUI.EditorButton("Apply")) lilToonSetting.ApplyShaderSetting(shaderSetting);
            GUI.enabled = true;
            EditorGUI.indentLevel--;
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
                                foreach(var obj in objs)
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
    }
}
#endif
