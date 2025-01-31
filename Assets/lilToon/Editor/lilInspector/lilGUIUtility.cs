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
            var labelStyle = new GUIStyle(GUI.skin.label){fontStyle = FontStyle.Bold};
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
                if(!string.IsNullOrEmpty(lilEditorParameters.instance.versionInfo))
                {
                    if(
                        !string.IsNullOrEmpty(lilEditorParameters.instance.versionInfo) &&
                        lilEditorParameters.instance.versionInfo.Contains("latest_vertion_name") &&
                        lilEditorParameters.instance.versionInfo.Contains("latest_vertion_value")
                    )
                    {
                        EditorJsonUtility.FromJsonOverwrite(lilEditorParameters.instance.versionInfo, latestVersion);
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
            var position = GUILayoutUtility.GetLastRect();
            position.x += position.width - 24;
            position.width = 24;

            if(GUI.Button(position, EditorGUIUtility.IconContent("_Popup"), middleButton))
            {
                var menu = new GenericMenu();
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
            #if VRC_SDK_VRCSDK2 || LILTOON_VRCSDK3_AVATARS || LILTOON_VRCSDK3_WORLDS
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

                        fallbackShaderType = lilEditorGUI.Popup("Shader Type", fallbackShaderType, sFallbackShaderTypes);
                        fallbackRenderType = lilEditorGUI.Popup("Rendering Mode", fallbackRenderType, sFallbackRenderTypes);
                        fallbackCullType = lilEditorGUI.Popup("Facing", fallbackCullType, sFallbackCullTypes);

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
            #if LILTOON_VRCSDK3_WORLDS
                if(isnormal && lilEditorGUI.Button(GetLoc("sOptimizeForEvents"))) lilMaterialUtils.RemoveUnusedTexture(material);
            #endif
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Editor
        #region
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
            isStWr          = stencilPass.floatValue == (float)StencilOp.Replace;

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
            foreach(var p in AllProperties().Where(p =>
                p.p != null &&
                p.blocks.Contains(propertyBlock)
            ))
            {
                copiedProperties[p.name] = p.p;
            }
        }

        private void PasteProperties(PropertyBlock propertyBlock, bool shouldCopyTex)
        {
            foreach(var p in AllProperties().Where(p =>
                p.p != null &&
                p.blocks.Contains(propertyBlock) &&
                !(!shouldCopyTex && p.isTexture) &&
                copiedProperties.ContainsKey(p.name) &&
                copiedProperties[p.name] != null
            ))
            {
                var propType = p.type;
                if(propType == MaterialProperty.PropType.Color)   p.colorValue = copiedProperties[p.name].colorValue;
                if(propType == MaterialProperty.PropType.Vector)  p.vectorValue = copiedProperties[p.name].vectorValue;
                if(propType == MaterialProperty.PropType.Float)   p.floatValue = copiedProperties[p.name].floatValue;
                if(propType == MaterialProperty.PropType.Range)   p.floatValue = copiedProperties[p.name].floatValue;
                if(propType == MaterialProperty.PropType.Texture) p.textureValue = copiedProperties[p.name].textureValue;
            }
        }

        private void ResetProperties(PropertyBlock propertyBlock)
        {
            #if UNITY_2019_3_OR_NEWER
            foreach(var p in AllProperties().Where(p =>
                p.p != null &&
                p.blocks.Contains(propertyBlock) &&
                p.targets[0] is Material &&
                ((Material)p.targets[0]).shader != null
            ))
            {
                var shader = ((Material)p.targets[0]).shader;
                int propID = shader.FindPropertyIndex(p.name);
                if(propID == -1) continue;
                var propType = p.type;
                if(propType == MaterialProperty.PropType.Color)     p.colorValue = shader.GetPropertyDefaultVectorValue(propID);
                if(propType == MaterialProperty.PropType.Vector)    p.vectorValue = shader.GetPropertyDefaultVectorValue(propID);
                if(propType == MaterialProperty.PropType.Float)     p.floatValue = shader.GetPropertyDefaultFloatValue(propID);
                if(propType == MaterialProperty.PropType.Range)     p.floatValue = shader.GetPropertyDefaultFloatValue(propID);
                if(propType == MaterialProperty.PropType.Texture)   p.textureValue = null;
            }
            #endif
        }

        private bool ShouldDrawBlock(PropertyBlock propertyBlock)
        {
            if(propertyBlock == PropertyBlock.Base && lilEditorGUI.CheckPropertyToDraw("Render Queue")) return true;
            if(propertyBlock == PropertyBlock.Rendering && lilEditorGUI.CheckPropertyToDraw("Render Queue", "Enable GPU Instancing")) return true;

            foreach (var p in GetBlock2Properties()[propertyBlock])
            {
                if (p.p == null ) { continue; }
                if (lilEditorGUI.CheckPropertyToDraw(p) is false) { continue; }
                return true;
            }
            return false;
        }

        private bool ShouldDrawBlock(params string[] labels)
        {
            return lilEditorGUI.CheckPropertyToDraw(labels);
        }

        private bool ShouldDrawBlock()
        {
            return lilEditorGUI.CheckPropertyToDraw();
        }

        private void CopyProperties(object obj)
        {
            CopyProperties((PropertyBlock)obj);
        }

        private void PasteProperties(object obj)
        {
            var propertyBlockData = (PropertyBlockData)obj;
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

    }
}
#endif
