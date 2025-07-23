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
        private void DrawAdvancedGUI(Material material)
        {
            #if UNITY_2019_1_OR_NEWER
            edSet.searchKeyWord = EditorGUILayout.TextField(edSet.searchKeyWord, EditorStyles.toolbarSearchField);
            #else
            edSet.searchKeyWord = EditorGUILayout.TextField(edSet.searchKeyWord);
            #endif
            if(isLite)
            {
                //------------------------------------------------------------------------------------------------------------------------------
                // Base Setting
                DrawBaseSettings(material, sTransparentMode, sRenderingModeList, sRenderingModeListLite, sTransparentModeList);

                //------------------------------------------------------------------------------------------------------------------------------
                // Lighting
                DrawLightingSettings();

                //------------------------------------------------------------------------------------------------------------------------------
                // UV
                if(ShouldDrawBlock(PropertyBlock.UV))
                {
                    edSet.isShowMainUV = lilEditorGUI.Foldout(GetLoc("sMainUV"), edSet.isShowMainUV);
                    DrawMenuButton(GetLoc("sAnchorUVSetting"), PropertyBlock.UV);
                    if(edSet.isShowMainUV)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainUV"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorUVSetting"), PropertyBlock.UV);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        UVSettingGUI(mainTex, mainTex_ScrollRotate);
                        LocalizedProperty(shiftBackfaceUV);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
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
                if(ShouldDrawBlock(PropertyBlock.MainColor1st))
                {
                    edSet.isShowMain = lilEditorGUI.Foldout(GetLoc("sMainColorSetting"), edSet.isShowMain);
                    DrawMenuButton(GetLoc("sAnchorMainColor"), PropertyBlock.MainColor1st);
                    if(edSet.isShowMain)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Main
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(sMainColorBranch, customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorMainColor1"), PropertyBlock.MainColor1st);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        LocalizedPropertyTexture(mainColorRGBAContent, mainTex, mainColor);
                        if(isUseAlpha) lilEditorGUI.SetAlphaIsTransparencyGUI(mainTex);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Shadow
                DrawShadowSettings();

                //------------------------------------------------------------------------------------------------------------------------------
                // Emission
                if(ShouldDrawBlock(PropertyBlock.Emission1st))
                {
                    edSet.isShowEmission = lilEditorGUI.Foldout(GetLoc("sEmissionSetting"), edSet.isShowEmission);
                    DrawMenuButton(GetLoc("sAnchorEmission"), PropertyBlock.Emission1st);
                    if(edSet.isShowEmission)
                    {
                        // Emission
                        EditorGUILayout.BeginVertical(boxOuter);
                        LocalizedProperty(useEmission, false);
                        DrawMenuButton(GetLoc("sAnchorEmission"), PropertyBlock.Emission1st);
                        if(useEmission.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            TextureGUI(ref edSet.isShowEmissionMap, colorMaskRGBAContent, emissionMap, emissionColor, emissionMap_ScrollRotate, emissionMap_UVMode, true, true);
                            lilEditorGUI.DrawLine();
                            LocalizedProperty(emissionBlink);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                EditorGUILayout.Space();

                //------------------------------------------------------------------------------------------------------------------------------
                // Normal & Reflection
                GUILayout.Label(GetLoc("sNormalMapReflection"), boldLabel);

                //------------------------------------------------------------------------------------------------------------------------------
                // MatCap
                DrawMatCapSettings();

                //------------------------------------------------------------------------------------------------------------------------------
                // Rim
                DrawRimSettings();

                EditorGUILayout.Space();

                //------------------------------------------------------------------------------------------------------------------------------
                // Advanced
                GUILayout.Label(GetLoc("sAdvanced"), boldLabel);

                //------------------------------------------------------------------------------------------------------------------------------
                // Outline
                DrawOutlineSettings(material);

                //------------------------------------------------------------------------------------------------------------------------------
                // Stencil
                DrawStencilSettings(material);

                //------------------------------------------------------------------------------------------------------------------------------
                // Rendering
                if(ShouldDrawBlock(PropertyBlock.Rendering))
                {
                    edSet.isShowRendering = lilEditorGUI.Foldout(GetLoc("sRenderingSetting"), edSet.isShowRendering);
                    DrawMenuButton(GetLoc("sAnchorRendering"), PropertyBlock.Rendering);
                    if(edSet.isShowRendering && ShouldDrawBlock(PropertyBlock.Rendering))
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Reset Button
                        if(lilEditorGUI.Button(GetLoc("sRenderingReset")))
                        {
                            material.enableInstancing = false;
                            SetupMaterialWithRenderingMode(renderingModeBuf, transparentModeBuf);
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
                            shaderType = lilEditorGUI.Popup(GetLoc("sShaderType"),shaderType,new string[]{GetLoc("sShaderTypeNormal"),GetLoc("sShaderTypeLite")});
                            if(shaderTypeBuf != shaderType)
                            {
                                if(shaderType==0) isLite = false;
                                if(shaderType==1) isLite = true;
                                SetupMaterialWithRenderingMode(renderingModeBuf, transparentModeBuf);
                            }

                            //------------------------------------------------------------------------------------------------------------------------------
                            // Rendering
                            if(renderingModeBuf == RenderingMode.Transparent || renderingModeBuf == RenderingMode.Fur || renderingModeBuf == RenderingMode.FurTwoPass)
                            {
                                LocalizedProperty(subpassCutoff);
                            }
                            LocalizedProperty(cull);
                            LocalizedProperty(zclip);
                            LocalizedProperty(zwrite);
                            LocalizedProperty(ztest);
                            LocalizedProperty(offsetFactor);
                            LocalizedProperty(offsetUnits);
                            LocalizedProperty(colorMask);
                            LocalizedProperty(alphaToMask);
                            LocalizedProperty(lilShadowCasterBias);
                            lilEditorGUI.DrawLine();
                            BlendSettingGUI(ref edSet.isShowBlend, GetLoc("sForward"), srcBlend, dstBlend, srcBlendAlpha, dstBlendAlpha, blendOp, blendOpAlpha);
                            lilEditorGUI.DrawLine();
                            BlendSettingGUI(ref edSet.isShowBlendAdd, GetLoc("sForwardAdd"), srcBlendFA, dstBlendFA, srcBlendAlphaFA, dstBlendAlphaFA, blendOpFA, blendOpAlphaFA);
                            lilEditorGUI.DrawLine();
                            EnableInstancingField();
                            RenderQueueField();
                            LocalizedProperty(beforeExposureLimit);
                            LocalizedProperty(lilDirectionalLightStrength);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Pre
                        if(transparentModeBuf == TransparentMode.TwoPass)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField("PrePass", customToggleFont);
                            EditorGUILayout.BeginVertical(boxInner);
                            LocalizedProperty(preCull);
                            LocalizedProperty(preZclip);
                            LocalizedProperty(preZwrite);
                            LocalizedProperty(preZtest);
                            LocalizedProperty(preOffsetFactor);
                            LocalizedProperty(preOffsetUnits);
                            LocalizedProperty(preColorMask);
                            LocalizedProperty(preAlphaToMask);
                            lilEditorGUI.DrawLine();
                            BlendSettingGUI(ref edSet.isShowBlendPre, GetLoc("sForward"), preSrcBlend, preDstBlend, preSrcBlendAlpha, preDstBlendAlpha, preBlendOp, preBlendOpAlpha);
                            lilEditorGUI.DrawLine();
                            BlendSettingGUI(ref edSet.isShowBlendAddPre, GetLoc("sForwardAdd"), preSrcBlendFA, preDstBlendFA, preSrcBlendAlphaFA, preDstBlendAlphaFA, preBlendOpFA, preBlendOpAlphaFA);
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
                            LocalizedProperty(outlineCull);
                            LocalizedProperty(outlineZclip);
                            LocalizedProperty(outlineZwrite);
                            LocalizedProperty(outlineZtest);
                            LocalizedProperty(outlineOffsetFactor);
                            LocalizedProperty(outlineOffsetUnits);
                            LocalizedProperty(outlineColorMask);
                            LocalizedProperty(outlineAlphaToMask);
                            lilEditorGUI.DrawLine();
                            BlendSettingGUI(ref edSet.isShowBlendOutline, GetLoc("sForward"), outlineSrcBlend, outlineDstBlend, outlineSrcBlendAlpha, outlineDstBlendAlpha, outlineBlendOp, outlineBlendOpAlpha);
                            lilEditorGUI.DrawLine();
                            BlendSettingGUI(ref edSet.isShowBlendAddOutline, GetLoc("sForwardAdd"), outlineSrcBlendFA, outlineDstBlendFA, outlineSrcBlendAlphaFA, outlineDstBlendAlphaFA, outlineBlendOpFA, outlineBlendOpAlphaFA);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Light Bake
                if(ShouldDrawBlock("Double Sided Global Illumination", "Global Illumination"))
                {
                    edSet.isShowLightBake = lilEditorGUI.Foldout(GetLoc("sLightBakeSetting"), edSet.isShowLightBake);
                    //DrawMenuButton(GetLoc("sAnchorLightBake"), PropertyBlock.LightBake);
                    if(edSet.isShowLightBake)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sLightBakeSetting"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInner);
                        if(!isCustomEditor) DoubleSidedGIField();
                        if(!isCustomEditor) LightmapEmissionFlagsProperty();
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Optimization
                if(!isMultiVariants && ShouldDrawBlock())
                {
                    GUILayout.Label(GetLoc("sOptimization"), boldLabel);
                    edSet.isShowOptimization = lilEditorGUI.Foldout(GetLoc("sOptimization"), edSet.isShowOptimization);
                    lilEditorGUI.DrawHelpButton(GetLoc("sAnchorOptimization"));
                    if(edSet.isShowOptimization)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sOptimization"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        DrawOptimizationButton(material, !(isLite && isMulti));
                        lilEditorGUI.RemoveUnusedPropertiesGUI(material);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }
            }
            else if(isFakeShadow)
            {
                //------------------------------------------------------------------------------------------------------------------------------
                // Base Setting
                if(ShouldDrawBlock(PropertyBlock.Base))
                {
                    GUILayout.Label(GetLoc("sBaseSetting"), boldLabel);
                    edSet.isShowBase = lilEditorGUI.Foldout(GetLoc("sBaseSetting"), edSet.isShowBase);
                    DrawMenuButton(GetLoc("sAnchorBaseSetting"), PropertyBlock.Base);
                    if(edSet.isShowBase)
                    {
                        EditorGUILayout.BeginVertical(customBox);
                            LocalizedProperty(cull);
                            LocalizedProperty(invisible);
                            LocalizedProperty(zwrite);
                            RenderQueueField();
                            lilEditorGUI.DrawLine();
                            GUILayout.Label("FakeShadow", boldLabel);
                            LocalizedProperty(fakeShadowVector);
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // UV
                if(ShouldDrawBlock(PropertyBlock.UV))
                {
                    edSet.isShowMainUV = lilEditorGUI.Foldout(GetLoc("sMainUV"), edSet.isShowMainUV);
                    DrawMenuButton(GetLoc("sAnchorUVSetting"), PropertyBlock.UV);
                    if(edSet.isShowMainUV)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainUV"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorUVSetting"), PropertyBlock.UV);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        UVSettingGUI(mainTex);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
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
                if(ShouldDrawBlock(PropertyBlock.MainColor1st))
                {
                    edSet.isShowMain = lilEditorGUI.Foldout(GetLoc("sMainColorSetting"), edSet.isShowMain);
                    DrawMenuButton(GetLoc("sAnchorMainColor"), PropertyBlock.MainColor1st);
                    if(edSet.isShowMain)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Main
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(sMainColorBranch, customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorMainColor1"), PropertyBlock.MainColor1st);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        LocalizedPropertyTexture(mainColorRGBAContent, mainTex, mainColor);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                EditorGUILayout.Space();

                //------------------------------------------------------------------------------------------------------------------------------
                // Advanced
                GUILayout.Label(GetLoc("sAdvanced"), boldLabel);

                //------------------------------------------------------------------------------------------------------------------------------
                // Stencil
                if(ShouldDrawBlock(PropertyBlock.Stencil))
                {
                    edSet.isShowStencil = lilEditorGUI.Foldout(GetLoc("sStencilSetting"), edSet.isShowStencil);
                    DrawMenuButton(GetLoc("sAnchorStencil"), PropertyBlock.Stencil);
                    if(edSet.isShowStencil)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sStencilSetting"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorStencil"), PropertyBlock.Stencil);
                        EditorGUILayout.BeginVertical(boxInner);
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Auto Setting
                        if(lilEditorGUI.Button("Set Writer"))
                        {
                            isStWr = true;
                            stencilRef.floatValue = 51;
                            stencilReadMask.floatValue = 255.0f;
                            stencilWriteMask.floatValue = 255.0f;
                            stencilComp.floatValue = (float)CompareFunction.Equal;
                            stencilPass.floatValue = (float)StencilOp.Replace;
                            stencilFail.floatValue = (float)StencilOp.Keep;
                            stencilZFail.floatValue = (float)StencilOp.Keep;
                            material.renderQueue = material.shader.renderQueue - 1;
                            if(renderingModeBuf == RenderingMode.Opaque) material.renderQueue += 450;
                        }
                        if(lilEditorGUI.Button("Set Reader"))
                        {
                            isStWr = false;
                            stencilRef.floatValue = 51;
                            stencilReadMask.floatValue = 255.0f;
                            stencilWriteMask.floatValue = 255.0f;
                            stencilComp.floatValue = (float)CompareFunction.Equal;
                            stencilPass.floatValue = (float)StencilOp.Keep;
                            stencilFail.floatValue = (float)StencilOp.Keep;
                            stencilZFail.floatValue = (float)StencilOp.Keep;
                            material.renderQueue = -1;
                            if(renderingModeBuf == RenderingMode.Opaque) material.renderQueue += 450;
                        }
                        if(lilEditorGUI.Button("Reset"))
                        {
                            isStWr = false;
                            stencilRef.floatValue = 51;
                            stencilReadMask.floatValue = 255.0f;
                            stencilWriteMask.floatValue = 255.0f;
                            stencilComp.floatValue = (float)CompareFunction.Equal;
                            stencilPass.floatValue = (float)StencilOp.Keep;
                            stencilFail.floatValue = (float)StencilOp.Keep;
                            stencilZFail.floatValue = (float)StencilOp.Keep;
                            material.renderQueue = -1;
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Base
                        {
                            lilEditorGUI.DrawLine();
                            LocalizedProperty(stencilRef);
                            LocalizedProperty(stencilReadMask);
                            LocalizedProperty(stencilWriteMask);
                            LocalizedProperty(stencilComp);
                            LocalizedProperty(stencilPass);
                            LocalizedProperty(stencilFail);
                            LocalizedProperty(stencilZFail);
                        }

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Rendering
                if(ShouldDrawBlock(PropertyBlock.Rendering))
                {
                    edSet.isShowRendering = lilEditorGUI.Foldout(GetLoc("sRenderingSetting"), edSet.isShowRendering);
                    DrawMenuButton(GetLoc("sAnchorRendering"), PropertyBlock.Rendering);
                    if(edSet.isShowRendering)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Reset Button
                        if(lilEditorGUI.Button(GetLoc("sRenderingReset")))
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
                            material.SetInt("_BlendOp", (int)BlendOp.Add);
                            material.SetInt("_BlendOpAlpha", (int)BlendOp.Add);
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Base
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField(GetLoc("sRenderingSetting"), customToggleFont);
                            EditorGUILayout.BeginVertical(boxInner);
                            LocalizedProperty(cull);
                            LocalizedProperty(zclip);
                            LocalizedProperty(zwrite);
                            LocalizedProperty(ztest);
                            LocalizedProperty(offsetFactor);
                            LocalizedProperty(offsetUnits);
                            LocalizedProperty(colorMask);
                            LocalizedProperty(alphaToMask);
                            LocalizedProperty(lilShadowCasterBias);
                            lilEditorGUI.DrawLine();
                            BlendSettingGUI(ref edSet.isShowBlend, GetLoc("sForward"), srcBlend, dstBlend, srcBlendAlpha, dstBlendAlpha, blendOp, blendOpAlpha);
                            lilEditorGUI.DrawLine();
                            EnableInstancingField();
                            RenderQueueField();
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Light Bake
                if(ShouldDrawBlock("Double Sided Global Illumination", "Global Illumination"))
                {
                    edSet.isShowLightBake = lilEditorGUI.Foldout(GetLoc("sLightBakeSetting"), edSet.isShowLightBake);
                    //DrawMenuButton(GetLoc("sAnchorLightBake"), PropertyBlock.LightBake);
                    if(edSet.isShowLightBake)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sLightBakeSetting"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInner);
                        if(!isCustomEditor) DoubleSidedGIField();
                        if(!isCustomEditor) LightmapEmissionFlagsProperty();
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Optimization
                if(!isMultiVariants && ShouldDrawBlock())
                {
                    GUILayout.Label(GetLoc("sOptimization"), boldLabel);
                    edSet.isShowOptimization = lilEditorGUI.Foldout(GetLoc("sOptimization"), edSet.isShowOptimization);
                    lilEditorGUI.DrawHelpButton(GetLoc("sAnchorOptimization"));
                    if(edSet.isShowOptimization)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sOptimization"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        DrawOptimizationButton(material, !(isLite && isMulti));
                        lilEditorGUI.RemoveUnusedPropertiesGUI(material);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }
            }
            else
            {
                //------------------------------------------------------------------------------------------------------------------------------
                // Base Setting
                DrawBaseSettings(material, sTransparentMode, sRenderingModeList, sRenderingModeListLite, sTransparentModeList);

                //------------------------------------------------------------------------------------------------------------------------------
                // Lighting
                DrawLightingSettings();

                //------------------------------------------------------------------------------------------------------------------------------
                // UV
                if(ShouldDrawBlock(PropertyBlock.UV))
                {
                    edSet.isShowMainUV = lilEditorGUI.Foldout(GetLoc("sMainUV"), edSet.isShowMainUV);
                    DrawMenuButton(GetLoc("sAnchorUVSetting"), PropertyBlock.UV);
                    if(edSet.isShowMainUV)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sMainUV"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorUVSetting"), PropertyBlock.UV);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        UVSettingGUI(mainTex, mainTex_ScrollRotate);
                        LocalizedProperty(shiftBackfaceUV);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
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
                if(ShouldDrawBlock(PropertyBlock.MainColor))
                {
                    edSet.isShowMain = lilEditorGUI.Foldout(GetLoc("sMainColorSetting"), edSet.isShowMain);
                    DrawMenuButton(GetLoc("sAnchorMainColor"), PropertyBlock.MainColor);
                    if(edSet.isShowMain)
                    {
                        if(isGem)
                        {
                            //------------------------------------------------------------------------------------------------------------------------------
                            // Main
                            if(ShouldDrawBlock(PropertyBlock.MainColor1st))
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                EditorGUILayout.LabelField(sMainColorBranch, customToggleFont);
                                DrawMenuButton(GetLoc("sAnchorMainColor1"), PropertyBlock.MainColor1st);
                                //LocalizedProperty(useMainTex);
                                //if(useMainTex.floatValue == 1)
                                //{
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    LocalizedPropertyTexture(mainColorRGBAContent, mainTex, mainColor);
                                    EditorGUILayout.EndVertical();
                                //}
                                EditorGUILayout.EndVertical();
                            }
                        }
                        else
                        {
                            //------------------------------------------------------------------------------------------------------------------------------
                            // Main
                            if(ShouldDrawBlock(PropertyBlock.MainColor1st))
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                EditorGUILayout.LabelField(sMainColorBranch, customToggleFont);
                                DrawMenuButton(GetLoc("sAnchorMainColor1"), PropertyBlock.MainColor1st);
                                //LocalizedProperty(useMainTex);
                                //if(useMainTex.floatValue == 1)
                                //{
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    LocalizedPropertyTexture(mainColorRGBAContent, mainTex, mainColor);
                                    if(isUseAlpha) lilEditorGUI.SetAlphaIsTransparencyGUI(mainTex);
                                    lilEditorGUI.DrawLine();
                                    DrawMainAdjustSettings(material);
                                    EditorGUILayout.EndVertical();
                                //}
                                EditorGUILayout.EndVertical();
                            }

                            //------------------------------------------------------------------------------------------------------------------------------
                            // 2nd
                            if(ShouldDrawBlock(PropertyBlock.MainColor2nd))
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                LocalizedProperty(useMain2ndTex, false);
                                DrawMenuButton(GetLoc("sAnchorMainColor2"), PropertyBlock.MainColor2nd);
                                if(useMain2ndTex.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    LocalizedPropertyTexture(colorRGBAContent, main2ndTex, mainColor2nd);
                                    EditorGUI.indentLevel += 2;
                                    LocalizedPropertyAlpha(mainColor2nd);
                                    LocalizedProperty(main2ndTexIsMSDF);
                                    LocalizedProperty(main2ndTex_Cull);
                                    EditorGUI.indentLevel -= 2;
                                    LocalizedProperty(main2ndEnableLighting);
                                    LocalizedProperty(main2ndTexBlendMode);
                                    LocalizedProperty(main2ndTexAlphaMode);
                                    lilEditorGUI.DrawLine();
                                    UV4Decal(main2ndTexIsDecal, main2ndTexIsLeftOnly, main2ndTexIsRightOnly, main2ndTexShouldCopy, main2ndTexShouldFlipMirror, main2ndTexShouldFlipCopy, main2ndTex, main2ndTex_ScrollRotate, main2ndTexAngle, main2ndTexDecalAnimation, main2ndTexDecalSubParam, main2ndTex_UVMode);
                                    lilEditorGUI.DrawLine();
                                    LocalizedPropertyTexture(maskBlendContent, main2ndBlendMask);
                                    EditorGUILayout.LabelField(GetLoc("sDistanceFade"));
                                    EditorGUI.indentLevel++;
                                    LocalizedProperty(main2ndDistanceFade);
                                    EditorGUI.indentLevel--;
                                    lilEditorGUI.DrawLine();
                                    LocalizedProperty(main2ndDissolveParams);
                                    if(main2ndDissolveParams.vectorValue.x == 1.0f)                                                TextureGUI(ref edSet.isShowMain2ndDissolveMask, maskBlendContent, main2ndDissolveMask);
                                    if(main2ndDissolveParams.vectorValue.x == 2.0f && main2ndDissolveParams.vectorValue.y == 0.0f) LocalizedProperty(main2ndDissolvePos, "sPosition|2");
                                    if(main2ndDissolveParams.vectorValue.x == 2.0f && main2ndDissolveParams.vectorValue.y == 1.0f) LocalizedProperty(main2ndDissolvePos, "sVector|2");
                                    if(main2ndDissolveParams.vectorValue.x == 3.0f && main2ndDissolveParams.vectorValue.y == 0.0f) LocalizedProperty(main2ndDissolvePos, "sPosition|3");
                                    if(main2ndDissolveParams.vectorValue.x == 3.0f && main2ndDissolveParams.vectorValue.y == 1.0f) LocalizedProperty(main2ndDissolvePos, "sVector|3");
                                    if(main2ndDissolveParams.vectorValue.x != 0.0f)
                                    {
                                        TextureGUI(ref edSet.isShowMain2ndDissolveNoiseMask, noiseMaskContent, main2ndDissolveNoiseMask, main2ndDissolveNoiseStrength, main2ndDissolveNoiseMask_ScrollRotate);
                                        LocalizedProperty(main2ndDissolveColor);
                                    }
                                    lilEditorGUI.DrawLine();
                                    TextureBakeGUI(material, 5);
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndVertical();
                            }

                            //------------------------------------------------------------------------------------------------------------------------------
                            // 3rd
                            if(ShouldDrawBlock(PropertyBlock.MainColor3rd))
                            {
                                EditorGUILayout.BeginVertical(boxOuter);
                                LocalizedProperty(useMain3rdTex, false);
                                DrawMenuButton(GetLoc("sAnchorMainColor2"), PropertyBlock.MainColor3rd);
                                if(useMain3rdTex.floatValue == 1)
                                {
                                    EditorGUILayout.BeginVertical(boxInnerHalf);
                                    LocalizedPropertyTexture(colorRGBAContent, main3rdTex, mainColor3rd);
                                    EditorGUI.indentLevel += 2;
                                    LocalizedPropertyAlpha(mainColor3rd);
                                    LocalizedProperty(main3rdTexIsMSDF);
                                    LocalizedProperty(main3rdTex_Cull);
                                    EditorGUI.indentLevel -= 2;
                                    LocalizedProperty(main3rdEnableLighting);
                                    LocalizedProperty(main3rdTexBlendMode);
                                    LocalizedProperty(main3rdTexAlphaMode);
                                    lilEditorGUI.DrawLine();
                                    UV4Decal(main3rdTexIsDecal, main3rdTexIsLeftOnly, main3rdTexIsRightOnly, main3rdTexShouldCopy, main3rdTexShouldFlipMirror, main3rdTexShouldFlipCopy, main3rdTex, main3rdTex_ScrollRotate, main3rdTexAngle, main3rdTexDecalAnimation, main3rdTexDecalSubParam, main3rdTex_UVMode);
                                    lilEditorGUI.DrawLine();
                                    LocalizedPropertyTexture(maskBlendContent, main3rdBlendMask);
                                    EditorGUILayout.LabelField(GetLoc("sDistanceFade"));
                                    EditorGUI.indentLevel++;
                                    LocalizedProperty(main3rdDistanceFade);
                                    EditorGUI.indentLevel--;
                                    lilEditorGUI.DrawLine();
                                    LocalizedProperty(main3rdDissolveParams);
                                    if(main3rdDissolveParams.vectorValue.x == 1.0f)                                                TextureGUI(ref edSet.isShowMain3rdDissolveMask, maskBlendContent, main3rdDissolveMask);
                                    if(main3rdDissolveParams.vectorValue.x == 2.0f && main3rdDissolveParams.vectorValue.y == 0.0f) LocalizedProperty(main3rdDissolvePos, "sPosition|2");
                                    if(main3rdDissolveParams.vectorValue.x == 2.0f && main3rdDissolveParams.vectorValue.y == 1.0f) LocalizedProperty(main3rdDissolvePos, "sVector|2");
                                    if(main3rdDissolveParams.vectorValue.x == 3.0f && main3rdDissolveParams.vectorValue.y == 0.0f) LocalizedProperty(main3rdDissolvePos, "sPosition|3");
                                    if(main3rdDissolveParams.vectorValue.x == 3.0f && main3rdDissolveParams.vectorValue.y == 1.0f) LocalizedProperty(main3rdDissolvePos, "sVector|3");
                                    if(main3rdDissolveParams.vectorValue.x != 0.0f)
                                    {
                                        TextureGUI(ref edSet.isShowMain3rdDissolveNoiseMask, noiseMaskContent, main3rdDissolveNoiseMask, main3rdDissolveNoiseStrength, main3rdDissolveNoiseMask_ScrollRotate);
                                        LocalizedProperty(main3rdDissolveColor);
                                    }
                                    lilEditorGUI.DrawLine();
                                    TextureBakeGUI(material, 6);
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndVertical();
                            }

                            //------------------------------------------------------------------------------------------------------------------------------
                            // Alpha Mask
                            DrawAlphaMaskSettings(material);
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
                // Rim Shade
                if(!isGem)
                {
                    if(ShouldDrawBlock(PropertyBlock.RimShade))
                    {
                        edSet.isShowRimShade = lilEditorGUI.Foldout("RimShade", edSet.isShowRimShade);
                        DrawMenuButton(GetLoc("sAnchorRimShade"), PropertyBlock.RimShade);
                        if(edSet.isShowRimShade)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            LocalizedProperty(useRimShade, false);
                            DrawMenuButton(GetLoc("sAnchorRimShade"), PropertyBlock.RimShade);
                            if(useRimShade.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                LocalizedPropertyTexture(colorMaskRGBAContent, rimShadeMask, rimShadeColor);
                                LocalizedProperty(rimShadeNormalStrength);
                                LocalizedProperty(rimShadeBorder);
                                LocalizedProperty(rimShadeBlur);
                                LocalizedProperty(rimShadeFresnelPower);
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Emission
                if(ShouldDrawBlock(PropertyBlock.Emission))
                {
                    edSet.isShowEmission = lilEditorGUI.Foldout(GetLoc("sEmissionSetting"), edSet.isShowEmission);
                    DrawMenuButton(GetLoc("sAnchorEmission"), PropertyBlock.Emission);
                    if(edSet.isShowEmission)
                    {
                        // Emission
                        if(ShouldDrawBlock(PropertyBlock.Emission1st))
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            LocalizedProperty(useEmission, false);
                            DrawMenuButton(GetLoc("sAnchorEmission"), PropertyBlock.Emission1st);
                            if(useEmission.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                TextureGUI(ref edSet.isShowEmissionMap, colorMaskRGBAContent, emissionMap, emissionColor, emissionMap_ScrollRotate, emissionMap_UVMode, true, true);
                                LocalizedPropertyAlpha(emissionColor);
                                LocalizedProperty(emissionMainStrength);
                                lilEditorGUI.DrawLine();
                                TextureGUI(ref edSet.isShowEmissionBlendMask, maskBlendRGBAContent, emissionBlendMask, emissionBlend, emissionBlendMask_ScrollRotate, true, true);
                                LocalizedProperty(emissionBlendMode);
                                lilEditorGUI.DrawLine();
                                LocalizedProperty(emissionBlink);
                                lilEditorGUI.DrawLine();
                                LocalizedProperty(emissionUseGrad);
                                if(emissionUseGrad.floatValue == 1)
                                {
                                    EditorGUI.indentLevel++;
                                    LocalizedPropertyTexture(gradSpeedContent, emissionGradTex, emissionGradSpeed);
                                    if(lilEditorGUI.CheckPropertyToDraw(emissionGradSpeed)) lilTextureUtils.GradientEditor(material, "_eg", emiGrad, emissionGradSpeed);
                                    EditorGUI.indentLevel--;
                                }
                                lilEditorGUI.DrawLine();
                                LocalizedProperty(emissionParallaxDepth);
                                LocalizedProperty(emissionFluorescence);
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }

                        // Emission 2nd
                        if(ShouldDrawBlock(PropertyBlock.Emission2nd))
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            LocalizedProperty(useEmission2nd, false);
                            DrawMenuButton(GetLoc("sAnchorEmission"), PropertyBlock.Emission2nd);
                            if(useEmission2nd.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                TextureGUI(ref edSet.isShowEmission2ndMap, colorMaskRGBAContent, emission2ndMap, emission2ndColor, emission2ndMap_ScrollRotate, emission2ndMap_UVMode, true, true);
                                LocalizedPropertyAlpha(emission2ndColor);
                                LocalizedProperty(emission2ndMainStrength);
                                lilEditorGUI.DrawLine();
                                TextureGUI(ref edSet.isShowEmission2ndBlendMask, maskBlendRGBAContent, emission2ndBlendMask, emission2ndBlend, emission2ndBlendMask_ScrollRotate, true, true);
                                LocalizedProperty(emission2ndBlendMode);
                                lilEditorGUI.DrawLine();
                                LocalizedProperty(emission2ndBlink);
                                lilEditorGUI.DrawLine();
                                LocalizedProperty(emission2ndUseGrad);
                                if(emission2ndUseGrad.floatValue == 1)
                                {
                                    EditorGUI.indentLevel++;
                                    LocalizedPropertyTexture(gradSpeedContent, emission2ndGradTex, emission2ndGradSpeed);
                                    if(lilEditorGUI.CheckPropertyToDraw(emission2ndGradSpeed)) lilTextureUtils.GradientEditor(material, "_e2g", emi2Grad, emission2ndGradSpeed);
                                    EditorGUI.indentLevel--;
                                }
                                lilEditorGUI.DrawLine();
                                LocalizedProperty(emission2ndParallaxDepth);
                                LocalizedProperty(emission2ndFluorescence);
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                }

                EditorGUILayout.Space();

                //------------------------------------------------------------------------------------------------------------------------------
                // Normal / Reflection
                GUILayout.Label(GetLoc("sNormalMapReflection"), boldLabel);

                //------------------------------------------------------------------------------------------------------------------------------
                // Normal
                if(ShouldDrawBlock(PropertyBlock.NormalMap))
                {
                    edSet.isShowBump = lilEditorGUI.Foldout(GetLoc("sNormalMapSetting"), edSet.isShowBump);
                    DrawMenuButton(GetLoc("sAnchorNormalMap"), PropertyBlock.NormalMap);
                    if(edSet.isShowBump)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // 1st
                        if(ShouldDrawBlock(PropertyBlock.NormalMap1st))
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            LocalizedProperty(useBumpMap, false);
                            DrawMenuButton(GetLoc("sAnchorNormalMap"), PropertyBlock.NormalMap1st);
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
                        if(ShouldDrawBlock(PropertyBlock.NormalMap2nd))
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            LocalizedProperty(useBump2ndMap, false);
                            DrawMenuButton(GetLoc("sAnchorNormalMap"), PropertyBlock.NormalMap2nd);
                            if(useBump2ndMap.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                TextureGUI(ref edSet.isShowBump2ndMap, normalMapContent, bump2ndMap, bump2ndScale, bump2ndMap_UVMode, "UV Mode|UV0|UV1|UV2|UV3");
                                lilEditorGUI.DrawLine();
                                TextureGUI(ref edSet.isShowBump2ndScaleMask, maskStrengthContent, bump2ndScaleMask);
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Anisotropy
                        if(ShouldDrawBlock(PropertyBlock.Anisotropy))
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            LocalizedProperty(useAnisotropy, false);
                            DrawMenuButton(GetLoc("sAnchorAnisotropy"), PropertyBlock.Anisotropy);
                            if(useAnisotropy.floatValue == 1)
                            {
                                EditorGUILayout.BeginVertical(boxInnerHalf);
                                TextureGUI(ref edSet.isShowAnisotropyTangentMap, normalMapContent, anisotropyTangentMap);
                                lilEditorGUI.DrawLine();
                                TextureGUI(ref edSet.isShowAnisotropyScaleMask, maskStrengthContent, anisotropyScaleMask, anisotropyScale);
                                lilEditorGUI.DrawLine();
                                GUILayout.Label(GetLoc("sApplyTo"), boldLabel);
                                EditorGUI.indentLevel++;
                                LocalizedProperty(anisotropy2Reflection);
                                if(anisotropy2Reflection.floatValue != 0.0f)
                                {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.LabelField("1st Specular", boldLabel);
                                    LocalizedProperty(anisotropyTangentWidth);
                                    LocalizedProperty(anisotropyBitangentWidth);
                                    LocalizedProperty(anisotropyShift);
                                    LocalizedProperty(anisotropyShiftNoiseScale);
                                    LocalizedProperty(anisotropySpecularStrength);
                                    lilEditorGUI.DrawLine();
                                    EditorGUILayout.LabelField("2nd Specular", boldLabel);
                                    LocalizedProperty(anisotropy2ndTangentWidth);
                                    LocalizedProperty(anisotropy2ndBitangentWidth);
                                    LocalizedProperty(anisotropy2ndShift);
                                    LocalizedProperty(anisotropy2ndShiftNoiseScale);
                                    LocalizedProperty(anisotropy2ndSpecularStrength);
                                    lilEditorGUI.DrawLine();
                                    LocalizedProperty(anisotropyShiftNoiseMask);
                                    EditorGUI.indentLevel--;
                                }
                                LocalizedProperty(anisotropy2MatCap);
                                LocalizedProperty(anisotropy2MatCap2nd);
                                EditorGUI.indentLevel--;
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Backlight
                if(!isGem && ShouldDrawBlock(PropertyBlock.Backlight))
                {
                    edSet.isShowBacklight = lilEditorGUI.Foldout(GetLoc("sBacklightSetting"), edSet.isShowBacklight);
                    DrawMenuButton(GetLoc("sAnchorBacklight"), PropertyBlock.Backlight);
                    if(edSet.isShowBacklight)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        LocalizedProperty(useBacklight, false);
                        DrawMenuButton(GetLoc("sAnchorBacklight"), PropertyBlock.Backlight);
                        if(useBacklight.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            TextureGUI(ref edSet.isShowBacklightColorTex, colorMaskRGBAContent, backlightColorTex, backlightColor);
                            EditorGUI.indentLevel++;
                            LocalizedPropertyAlpha(backlightColor);
                            LocalizedProperty(backlightMainStrength);
                            LocalizedProperty(backlightReceiveShadow);
                            LocalizedProperty(backlightBackfaceMask);
                            EditorGUI.indentLevel--;
                            lilEditorGUI.DrawLine();
                            LocalizedProperty(backlightNormalStrength);
                            lilEditorGUI.InvBorderGUI(backlightBorder);
                            LocalizedProperty(backlightBlur);
                            LocalizedProperty(backlightDirectivity);
                            LocalizedProperty(backlightViewStrength);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Reflection
                if(!isGem && ShouldDrawBlock(PropertyBlock.Reflection))
                {
                    edSet.isShowReflections = lilEditorGUI.Foldout(GetLoc("sReflectionsSetting"), edSet.isShowReflections);
                    DrawMenuButton(GetLoc("sAnchorReflection"), PropertyBlock.Reflection);
                    if(edSet.isShowReflections)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Reflection
                        EditorGUILayout.BeginVertical(boxOuter);
                        LocalizedProperty(useReflection, false);
                        DrawMenuButton(GetLoc("sAnchorReflection"), PropertyBlock.Reflection);
                        if(useReflection.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            TextureGUI(ref edSet.isShowSmoothnessTex, smoothnessContent, smoothnessTex, smoothness);
                            LocalizedProperty(gsaaStrength, 1);
                            lilEditorGUI.DrawLine();
                            TextureGUI(ref edSet.isShowMetallicGlossMap, metallicContent, metallicGlossMap, metallic);
                            lilEditorGUI.DrawLine();
                            TextureGUI(ref edSet.isShowReflectionColorTex, colorMaskRGBAContent, reflectionColorTex, reflectionColor);
                            EditorGUI.indentLevel++;
                            LocalizedPropertyAlpha(reflectionColor);
                            LocalizedProperty(reflectance);
                            EditorGUI.indentLevel--;
                            lilEditorGUI.DrawLine();
                            DrawSpecularMode();
                            LocalizedProperty(applyReflection);
                            if(applyReflection.floatValue == 1.0f)
                            {
                                EditorGUI.indentLevel++;
                                LocalizedProperty(reflectionNormalStrength);
                                LocalizedPropertyTexture(cubemapContent, reflectionCubeTex, reflectionCubeColor);
                                LocalizedProperty(reflectionCubeOverride);
                                LocalizedProperty(reflectionCubeEnableLighting);
                                EditorGUI.indentLevel--;
                            }
                            if(isTransparent) LocalizedProperty(reflectionApplyTransparency);
                            LocalizedProperty(reflectionBlendMode);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // MatCap
                DrawMatCapSettings();

                //------------------------------------------------------------------------------------------------------------------------------
                // Rim
                DrawRimSettings();

                //------------------------------------------------------------------------------------------------------------------------------
                // Glitter
                DrawGlitterSettings();

                //------------------------------------------------------------------------------------------------------------------------------
                // Gem
                if(isGem && ShouldDrawBlock(PropertyBlock.Gem))
                {
                    edSet.isShowGem = lilEditorGUI.Foldout(GetLoc("sGemSetting"), edSet.isShowGem);
                    DrawMenuButton(GetLoc("sAnchorGem"), PropertyBlock.Gem);
                    if(edSet.isShowGem)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sGem"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorGem"), PropertyBlock.Gem);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        GUILayout.Label(GetLoc("sRefraction"), boldLabel);
                        EditorGUI.indentLevel++;
                        LocalizedProperty(refractionStrength);
                        LocalizedProperty(refractionFresnelPower);
                        EditorGUI.indentLevel--;
                        lilEditorGUI.DrawLine();
                        GUILayout.Label(GetLoc("sGem"), boldLabel);
                        EditorGUI.indentLevel++;
                        LocalizedProperty(gemChromaticAberration);
                        LocalizedProperty(gemEnvContrast);
                        LocalizedProperty(gemEnvColor);
                        lilEditorGUI.DrawLine();
                        LocalizedProperty(gemParticleLoop);
                        LocalizedProperty(gemParticleColor);
                        EditorGUI.indentLevel--;
                        lilEditorGUI.DrawLine();
                        LocalizedProperty(gemVRParallaxStrength);
                        LocalizedPropertyTexture(smoothnessContent, smoothnessTex, smoothness);
                        LocalizedProperty(reflectance);
                        LocalizedPropertyTexture(cubemapContent, reflectionCubeTex, reflectionCubeColor);
                        LocalizedProperty(reflectionCubeOverride);
                        LocalizedProperty(reflectionCubeEnableLighting);
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
                if(ShouldDrawBlock(PropertyBlock.Parallax))
                {
                    edSet.isShowParallax = lilEditorGUI.Foldout(GetLoc("sParallax"), edSet.isShowParallax);
                    DrawMenuButton(GetLoc("sAnchorParallax"), PropertyBlock.Parallax);
                    if(edSet.isShowParallax)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        LocalizedProperty(useParallax, false);
                        DrawMenuButton(GetLoc("sAnchorParallax"), PropertyBlock.Parallax);
                        if(useParallax.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            LocalizedPropertyTexture(parallaxContent, parallaxMap, parallax);
                            LocalizedProperty(parallaxOffset);
                            LocalizedProperty(usePOM);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Distance Fade
                if(!isGem && ShouldDrawBlock(PropertyBlock.DistanceFade))
                {
                    edSet.isShowDistanceFade = lilEditorGUI.Foldout(GetLoc("sDistanceFade"), edSet.isShowDistanceFade);
                    DrawMenuButton(GetLoc("sAnchorDistanceFade"), PropertyBlock.DistanceFade);
                    if(edSet.isShowDistanceFade)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sDistanceFade"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorDistanceFade"), PropertyBlock.DistanceFade);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        LocalizedProperty(distanceFadeColor);
                        EditorGUI.indentLevel++;
                        LocalizedProperty(distanceFade);
                        LocalizedProperty(distanceFadeMode);
                        EditorGUI.indentLevel--;
                        DrawLine();
                        EditorGUILayout.LabelField(GetLoc("sRimLight"));
                        EditorGUI.indentLevel++;
                        LocalizedProperty(distanceFadeRimColor);
                        LocalizedPropertyAlpha(distanceFadeRimColor);
                        LocalizedProperty(distanceFadeRimFresnelPower);
                        EditorGUI.indentLevel--;
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // AudioLink
                if(ShouldDrawBlock(PropertyBlock.AudioLink))
                {
                    edSet.isShowAudioLink = lilEditorGUI.Foldout(GetLoc("sAudioLink"), edSet.isShowAudioLink);
                    DrawMenuButton(GetLoc("sAnchorAudioLink"), PropertyBlock.AudioLink);
                    if(edSet.isShowAudioLink)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        LocalizedProperty(useAudioLink, false);
                        DrawMenuButton(GetLoc("sAnchorAudioLink"), PropertyBlock.AudioLink);
                        if(useAudioLink.floatValue == 1)
                        {
                            string sALParamsNone = BuildParams(GetLoc("sOffset"), GetLoc("sAudioLinkBand"), GetLoc("sAudioLinkBandBass"), GetLoc("sAudioLinkBandLowMid"), GetLoc("sAudioLinkBandHighMid"), GetLoc("sAudioLinkBandTreble"));
                            string sALParamsPos = BuildParams(GetLoc("sScale"), GetLoc("sOffset"), GetLoc("sAudioLinkBand"), GetLoc("sAudioLinkBandBass"), GetLoc("sAudioLinkBandLowMid"), GetLoc("sAudioLinkBandHighMid"), GetLoc("sAudioLinkBandTreble"));
                            string sALParamsUV = BuildParams(GetLoc("sScale"), GetLoc("sOffset"), GetLoc("sAngle"), GetLoc("sAudioLinkBand"), GetLoc("sAudioLinkBandBass"), GetLoc("sAudioLinkBandLowMid"), GetLoc("sAudioLinkBandHighMid"), GetLoc("sAudioLinkBandTreble"));
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            LocalizedProperty(audioLinkUVMode);
                            if(audioLinkUVMode.floatValue == 0) LocalizedProperty(audioLinkUVParams, sALParamsNone);
                            if(audioLinkUVMode.floatValue == 1) LocalizedProperty(audioLinkUVParams, sALParamsPos);
                            if(audioLinkUVMode.floatValue == 2) LocalizedProperty(audioLinkUVParams, sALParamsUV);
                            if(audioLinkUVMode.floatValue == 3) TextureGUI(ref edSet.isShowAudioLinkMask, audioLinkMaskContent, audioLinkMask, null, audioLinkMask_ScrollRotate, audioLinkMask_UVMode, true, true);
                            if(audioLinkUVMode.floatValue == 4)
                            {
                                TextureGUI(ref edSet.isShowAudioLinkMask, audioLinkMaskSpectrumContent, audioLinkMask, null, audioLinkMask_ScrollRotate, audioLinkMask_UVMode, true, true);
                                lilEditorGUI.DrawVectorAs4Float(audioLinkUVParams, "Volume", "Base Boost", "Treble Boost", "Line Width");
                            }
                            if(audioLinkUVMode.floatValue == 5)
                            {
                                LocalizedProperty(audioLinkUVParams, sALParamsPos);
                                LocalizedProperty(audioLinkStart);
                            }
                            lilEditorGUI.DrawLine();
                            GUILayout.Label(GetLoc("sAudioLinkDefaultValue"), boldLabel);
                            EditorGUI.indentLevel++;
                            if(audioLinkUVMode.floatValue == 4) lilEditorGUI.DrawVectorAs4Float(audioLinkDefaultValue, GetLoc("sStrength"), "Detail", "Speed", GetLoc("sThreshold"));
                            else LocalizedProperty(audioLinkDefaultValue, BuildParams(GetLoc("sStrength"), GetLoc("sBlinkStrength"), GetLoc("sBlinkSpeed"), GetLoc("sThreshold")));
                            EditorGUI.indentLevel--;
                            lilEditorGUI.DrawLine();
                            GUILayout.Label(GetLoc("sApplyTo"), boldLabel);
                            EditorGUI.indentLevel++;
                            LocalizedProperty(audioLink2Main2nd);
                            LocalizedProperty(audioLink2Main3rd);
                            LocalizedProperty(audioLink2Emission);
                            LocalizedProperty(audioLink2EmissionGrad);
                            LocalizedProperty(audioLink2Emission2nd);
                            LocalizedProperty(audioLink2Emission2ndGrad);
                            LocalizedProperty(audioLink2Vertex);
                            if(audioLink2Vertex.floatValue == 1)
                            {
                                EditorGUI.indentLevel++;
                                LocalizedProperty(audioLinkVertexUVMode);
                                if(audioLinkVertexUVMode.floatValue == 0) LocalizedProperty(audioLinkVertexUVParams, sALParamsNone);
                                if(audioLinkVertexUVMode.floatValue == 1) LocalizedProperty(audioLinkVertexUVParams, sALParamsPos);
                                if(audioLinkVertexUVMode.floatValue == 2) LocalizedProperty(audioLinkVertexUVParams, sALParamsUV);
                                if(audioLinkVertexUVMode.floatValue == 3) TextureGUI(ref edSet.isShowAudioLinkMask, audioLinkMaskContent, audioLinkMask, null, audioLinkMask_ScrollRotate, audioLinkMask_UVMode, true, true);
                                if(audioLinkVertexUVMode.floatValue == 1) LocalizedProperty(audioLinkVertexStart);
                                lilEditorGUI.DrawLine();
                                LocalizedProperty(audioLinkVertexStrength);
                                EditorGUI.indentLevel--;
                            }
                            EditorGUI.indentLevel--;
                            lilEditorGUI.DrawLine();
                            LocalizedProperty(audioLinkAsLocal);
                            if(audioLinkAsLocal.floatValue == 1)
                            {
                                LocalizedPropertyTexture(audioLinkLocalMapContent, audioLinkLocalMap);
                                LocalizedProperty(audioLinkLocalMapParams);
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Dissolve
                if(ShouldDrawBlock(PropertyBlock.Dissolve))
                {
                    edSet.isShowDissolve = lilEditorGUI.Foldout(GetLoc("sDissolve"), edSet.isShowDissolve);
                    DrawMenuButton(GetLoc("sAnchorDissolve"), PropertyBlock.Dissolve);
                    if(edSet.isShowDissolve && ((renderingModeBuf == RenderingMode.Opaque && !isMulti) || (isMulti && transparentModeMat.floatValue == 0.0f)))
                    {
                        GUILayout.Label(GetLoc("sDissolveWarnOpaque"), wrapLabel);
                    }
                    if(edSet.isShowDissolve && (renderingModeBuf != RenderingMode.Opaque || (isMulti && transparentModeMat.floatValue != 0.0f)))
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        LocalizedProperty(dissolveParams, false);
                        DrawMenuButton(GetLoc("sAnchorDissolve"), PropertyBlock.Dissolve);
                        if(dissolveParams.vectorValue.x != 0)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            LocalizedProperty(dissolveParams, sDissolveParamsOther);
                            float dissolveX = (float) Math.Round(dissolveParams.vectorValue.x);
                            float dissolveY = (float) Math.Round(dissolveParams.vectorValue.y);
                            
                            if(dissolveX == 1.0f)                                         TextureGUI(ref edSet.isShowDissolveMask, maskBlendContent, dissolveMask);
                            if(dissolveX == 2.0f && dissolveY == 0.0f) LocalizedProperty(dissolvePos, "sPosition|2");
                            if(dissolveX == 2.0f && dissolveY == 1.0f) LocalizedProperty(dissolvePos, "sVector|2");
                            if(dissolveX == 3.0f && dissolveY == 0.0f) LocalizedProperty(dissolvePos, "sPosition|3");
                            if(dissolveX == 3.0f && dissolveY == 1.0f) LocalizedProperty(dissolvePos, "sVector|3");
                            TextureGUI(ref edSet.isShowDissolveNoiseMask, noiseMaskContent, dissolveNoiseMask, dissolveNoiseStrength, dissolveNoiseMask_ScrollRotate);
                            LocalizedProperty(dissolveColor);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // IDMask
                if(ShouldDrawBlock(PropertyBlock.IDMask))
                {
                    edSet.isShowIDMask = lilEditorGUI.Foldout(GetLoc("sIDMask"), edSet.isShowIDMask);
                    DrawMenuButton(GetLoc("sAnchorIDMask"), PropertyBlock.IDMask);
                    if(edSet.isShowIDMask)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("ID Mask"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        EditorGUILayout.HelpBox("It is recommended that these properties be set from scripts.", MessageType.Warning);
                        EditorGUILayout.HelpBox("If you want to mask vertex ids 1000 to 1999, set:\r\n_IDMask1 = 1\r\n_IDMaskIndex1 = 1000\r\n_IDMaskIndex2 = 2000", MessageType.Info);
                        LocalizedProperty(idMaskCompile);
                        LocalizedProperty(idMaskFrom);
                        LocalizedProperty(idMaskIsBitmap);

                        LocalizedProperty(idMask1);
                        LocalizedProperty(idMask2);
                        LocalizedProperty(idMask3);
                        LocalizedProperty(idMask4);
                        LocalizedProperty(idMask5);
                        LocalizedProperty(idMask6);
                        LocalizedProperty(idMask7);
                        LocalizedProperty(idMask8);
                        LocalizedProperty(idMaskIndex1);
                        LocalizedProperty(idMaskIndex2);
                        LocalizedProperty(idMaskIndex3);
                        LocalizedProperty(idMaskIndex4);
                        LocalizedProperty(idMaskIndex5);
                        LocalizedProperty(idMaskIndex6);
                        LocalizedProperty(idMaskIndex7);
                        LocalizedProperty(idMaskIndex8);
                        LocalizedProperty(idMaskControlsDissolve);

                        if(idMaskControlsDissolve.p != null && idMaskControlsDissolve.floatValue > 0.5f)
                        {
                            LocalizedProperty(idMaskPrior1);
                            LocalizedProperty(idMaskPrior2);
                            LocalizedProperty(idMaskPrior3);
                            LocalizedProperty(idMaskPrior4);
                            LocalizedProperty(idMaskPrior5);
                            LocalizedProperty(idMaskPrior6);
                            LocalizedProperty(idMaskPrior7);
                            LocalizedProperty(idMaskPrior8);
                        }

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // UDIM Discard
                if (ShouldDrawBlock(PropertyBlock.UDIMDiscard))
                {
                    edSet.isShowUDIMDiscard = lilEditorGUI.Foldout(GetLoc("sUDIMDiscard"), edSet.isShowUDIMDiscard);
                    DrawMenuButton(GetLoc("sAnchorUDIMDiscard"), PropertyBlock.UDIMDiscard);
                    if(edSet.isShowUDIMDiscard)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        LocalizedProperty(udimDiscardCompile); 
                        DrawMenuButton(GetLoc("sUDIMDiscard"), PropertyBlock.UDIMDiscard);
                        if (udimDiscardCompile.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            EditorGUILayout.HelpBox("Your model needs to be set up for this feature! Place groups of vertexes on different whole number UV tiles.", MessageType.Info);
                            LocalizedProperty(udimDiscardUV);
                            LocalizedProperty(udimDiscardMethod);

                            void UVDIMToggle(Rect position, MaterialProperty prop)
                            {
                                bool value = prop.floatValue != 0.0f;
                                EditorGUI.BeginChangeCheck();
                                EditorGUI.showMixedValue = prop.hasMixedValue;
                                value = EditorGUI.Toggle(position, GUIContent.none, value);
                                EditorGUI.showMixedValue = false;

                                if(EditorGUI.EndChangeCheck())
                                {
                                    prop.floatValue = value ? 1.0f : 0.0f;
                                }
                            }

                            var r0 = EditorGUILayout.GetControlRect(false); r0.width = 16;
                            var r1 = EditorGUILayout.GetControlRect(false); r1.width = 16;
                            var r2 = EditorGUILayout.GetControlRect(false); r2.width = 16;
                            var r3 = EditorGUILayout.GetControlRect(false); r3.width = 16;

                            UVDIMToggle(r0, udimDiscardRow3_0); r0.x += 40;
                            UVDIMToggle(r1, udimDiscardRow2_0); r1.x += 40;
                            UVDIMToggle(r2, udimDiscardRow1_0); r2.x += 40;
                            UVDIMToggle(r3, udimDiscardRow0_0); r3.x += 40;

                            UVDIMToggle(r0, udimDiscardRow3_1); r0.x += 40;
                            UVDIMToggle(r1, udimDiscardRow2_1); r1.x += 40;
                            UVDIMToggle(r2, udimDiscardRow1_1); r2.x += 40;
                            UVDIMToggle(r3, udimDiscardRow0_1); r3.x += 40;

                            UVDIMToggle(r0, udimDiscardRow3_2); r0.x += 40;
                            UVDIMToggle(r1, udimDiscardRow2_2); r1.x += 40;
                            UVDIMToggle(r2, udimDiscardRow1_2); r2.x += 40;
                            UVDIMToggle(r3, udimDiscardRow0_2); r3.x += 40;

                            UVDIMToggle(r0, udimDiscardRow3_3);
                            UVDIMToggle(r1, udimDiscardRow2_3);
                            UVDIMToggle(r2, udimDiscardRow1_3);
                            UVDIMToggle(r3, udimDiscardRow0_3);

                            EditorGUILayout.EndVertical();
                        }

                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Refraction
                if(isRefr && ShouldDrawBlock(PropertyBlock.Refraction))
                {
                    edSet.isShowRefraction = lilEditorGUI.Foldout(GetLoc("sRefractionSetting"), edSet.isShowRefraction);
                    DrawMenuButton(GetLoc("sAnchorRefraction"), PropertyBlock.Refraction);
                    if(edSet.isShowRefraction)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sRefraction"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorRefraction"), PropertyBlock.Refraction);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        LocalizedProperty(refractionStrength);
                        LocalizedProperty(refractionFresnelPower);
                        LocalizedProperty(refractionColorFromMain);
                        LocalizedProperty(refractionColor);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Fur
                if(isFur && ShouldDrawBlock(PropertyBlock.Fur))
                {
                    edSet.isShowFur = lilEditorGUI.Foldout(GetLoc("sFurSetting"), edSet.isShowFur);
                    DrawMenuButton(GetLoc("sAnchorFur"), PropertyBlock.Fur);
                    if(edSet.isShowFur)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sFur"), customToggleFont);
                        DrawMenuButton(GetLoc("sAnchorFur"), PropertyBlock.Fur);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        LocalizedPropertyTexture(normalMapContent, furVectorTex, furVectorScale);
                        LocalizedPropertyTexture(lengthMaskContent, furLengthMask);
                        LocalizedProperty(furVector);
                        if(isTwoPass) LocalizedProperty(furCutoutLength);
                        LocalizedProperty(vertexColor2FurVector);
                        LocalizedProperty(furGravity);
                        LocalizedProperty(furRandomize);
                        lilEditorGUI.DrawLine();
                        LocalizedPropertyTexture(noiseMaskContent, furNoiseMask);
                        UVSettingGUI(furNoiseMask);
                        LocalizedPropertyTexture(alphaMaskContent, furMask);
                        LocalizedProperty(furAO);
                        lilEditorGUI.DrawLine();
                        LocalizedProperty(furLayerNum);
                        lilEditorGUI.MinusRangeGUI(furRootOffset, GetLoc("sRootWidth"));
                        LocalizedProperty(furTouchStrength);
                        lilEditorGUI.DrawLine();
                        EditorGUILayout.LabelField(GetLoc("sRimLight"), EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        LocalizedProperty(furRimColor);
                        LocalizedProperty(furRimFresnelPower);
                        LocalizedProperty(furRimAntiLight);
                        EditorGUI.indentLevel--;
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Stencil
                DrawStencilSettings(material);

                //------------------------------------------------------------------------------------------------------------------------------
                // Rendering
                if(ShouldDrawBlock(PropertyBlock.Rendering))
                {
                    edSet.isShowRendering = lilEditorGUI.Foldout(GetLoc("sRenderingSetting"), edSet.isShowRendering);
                    DrawMenuButton(GetLoc("sAnchorRendering"), PropertyBlock.Rendering);
                    if(edSet.isShowRendering)
                    {
                        //------------------------------------------------------------------------------------------------------------------------------
                        // Reset Button
                        if(lilEditorGUI.Button(GetLoc("sRenderingReset")))
                        {
                            material.enableInstancing = false;
                            SetupMaterialWithRenderingMode(renderingModeBuf, transparentModeBuf);
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
                            shaderType = lilEditorGUI.Popup(GetLoc("sShaderType"),shaderType,new string[]{GetLoc("sShaderTypeNormal"),GetLoc("sShaderTypeLite")});
                            if(shaderTypeBuf != shaderType)
                            {
                                if(shaderType==0) isLite = false;
                                if(shaderType==1) isLite = true;
                                SetupMaterialWithRenderingMode(renderingModeBuf, transparentModeBuf);
                            }

                            //------------------------------------------------------------------------------------------------------------------------------
                            // Rendering
                            if(renderingModeBuf == RenderingMode.Transparent || renderingModeBuf == RenderingMode.Fur || renderingModeBuf == RenderingMode.FurTwoPass || (isMulti && (transparentModeMat.floatValue == 2.0f || transparentModeMat.floatValue == 4.0f)))
                            {
                                LocalizedProperty(subpassCutoff);
                            }
                            LocalizedProperty(cull);
                            LocalizedProperty(zclip);
                            LocalizedProperty(zwrite);
                            LocalizedProperty(ztest);
                            LocalizedProperty(offsetFactor);
                            LocalizedProperty(offsetUnits);
                            LocalizedProperty(colorMask);
                            LocalizedProperty(alphaToMask);
                            LocalizedProperty(lilShadowCasterBias);
                            lilEditorGUI.DrawLine();
                            BlendSettingGUI(ref edSet.isShowBlend, GetLoc("sForward"), srcBlend, dstBlend, srcBlendAlpha, dstBlendAlpha, blendOp, blendOpAlpha);
                            lilEditorGUI.DrawLine();
                            BlendSettingGUI(ref edSet.isShowBlendAdd, GetLoc("sForwardAdd"), srcBlendFA, dstBlendFA, srcBlendAlphaFA, dstBlendAlphaFA, blendOpFA, blendOpAlphaFA);
                            lilEditorGUI.DrawLine();
                            if(!isCustomEditor) EnableInstancingField();
                            RenderQueueField();
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        // Pre
                        if(transparentModeBuf == TransparentMode.TwoPass)
                        {
                            EditorGUILayout.BeginVertical(boxOuter);
                            EditorGUILayout.LabelField("PrePass", customToggleFont);
                            EditorGUILayout.BeginVertical(boxInner);
                            LocalizedProperty(preCull);
                            LocalizedProperty(preZclip);
                            LocalizedProperty(preZwrite);
                            LocalizedProperty(preZtest);
                            LocalizedProperty(preOffsetFactor);
                            LocalizedProperty(preOffsetUnits);
                            LocalizedProperty(preColorMask);
                            LocalizedProperty(preAlphaToMask);
                            lilEditorGUI.DrawLine();
                            BlendSettingGUI(ref edSet.isShowBlendPre, GetLoc("sForward"), preSrcBlend, preDstBlend, preSrcBlendAlpha, preDstBlendAlpha, preBlendOp, preBlendOpAlpha);
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
                            LocalizedProperty(outlineCull);
                            LocalizedProperty(outlineZclip);
                            LocalizedProperty(outlineZwrite);
                            LocalizedProperty(outlineZtest);
                            LocalizedProperty(outlineOffsetFactor);
                            LocalizedProperty(outlineOffsetUnits);
                            LocalizedProperty(outlineColorMask);
                            LocalizedProperty(outlineAlphaToMask);
                            lilEditorGUI.DrawLine();
                            BlendSettingGUI(ref edSet.isShowBlendOutline, GetLoc("sForward"), outlineSrcBlend, outlineDstBlend, outlineSrcBlendAlpha, outlineDstBlendAlpha, outlineBlendOp, outlineBlendOpAlpha);
                            lilEditorGUI.DrawLine();
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
                            LocalizedProperty(furCull);
                            LocalizedProperty(furZclip);
                            LocalizedProperty(furZwrite);
                            LocalizedProperty(furZtest);
                            LocalizedProperty(furOffsetFactor);
                            LocalizedProperty(furOffsetUnits);
                            LocalizedProperty(furColorMask);
                            LocalizedProperty(furAlphaToMask);
                            lilEditorGUI.DrawLine();
                            BlendSettingGUI(ref edSet.isShowBlendFur, GetLoc("sForward"), furSrcBlend, furDstBlend, furSrcBlendAlpha, furDstBlendAlpha, furBlendOp, furBlendOpAlpha);
                            lilEditorGUI.DrawLine();
                            BlendSettingGUI(ref edSet.isShowBlendAddFur, GetLoc("sForwardAdd"), furSrcBlendFA, furDstBlendFA, furSrcBlendAlphaFA, furDstBlendAlphaFA, furBlendOpFA, furBlendOpAlphaFA);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Light Bake
                if(ShouldDrawBlock("Double Sided Global Illumination", "Global Illumination"))
                {
                    edSet.isShowLightBake = lilEditorGUI.Foldout(GetLoc("sLightBakeSetting"), edSet.isShowLightBake);
                    //DrawMenuButton(GetLoc("sAnchorLightBake"), PropertyBlock.LightBake);
                    if(edSet.isShowLightBake)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sLightBakeSetting"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInner);
                        if(!isCustomEditor) DoubleSidedGIField();
                        if(!isCustomEditor) LightmapEmissionFlagsProperty();
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------
                // Tessellation
                if(ShouldDrawBlock(PropertyBlock.Tessellation))
                {
                    edSet.isShowTess = lilEditorGUI.Foldout(GetLoc("sTessellation"), edSet.isShowTess);
                    DrawMenuButton(GetLoc("sAnchorTessellation"), PropertyBlock.Tessellation);
                    if(edSet.isShowTess)
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        if(isTess != EditorGUILayout.ToggleLeft(GetLoc("sTessellation"), isTess, customToggleFont))
                        {
                            isTess = !isTess;
                            SetupMaterialWithRenderingMode(renderingModeBuf, transparentModeBuf);
                        }
                        if(isTess)
                        {
                            EditorGUILayout.BeginVertical(boxInner);
                            LocalizedProperty(tessEdge);
                            LocalizedProperty(tessStrength);
                            LocalizedProperty(tessShrink);
                            LocalizedProperty(tessFactorMax);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                EditorGUILayout.Space();

                //------------------------------------------------------------------------------------------------------------------------------
                // Optimization
                if(!isMultiVariants && ShouldDrawBlock())
                {
                    GUILayout.Label(GetLoc("sOptimization"), boldLabel);
                    edSet.isShowOptimization = lilEditorGUI.Foldout(GetLoc("sOptimization"), edSet.isShowOptimization);
                    lilEditorGUI.DrawHelpButton(GetLoc("sAnchorOptimization"));
                    if(edSet.isShowOptimization)
                    {
                        // Optimization
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sOptimization"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        DrawOptimizationButton(material, !(isLite && isMulti));
                        lilEditorGUI.RemoveUnusedPropertiesGUI(material);
                        TextureBakeGUI(material, 0);
                        TextureBakeGUI(material, 1);
                        TextureBakeGUI(material, 2);
                        TextureBakeGUI(material, 3);
                        if(lilEditorGUI.Button(GetLoc("sConvertLite"))) CreateLiteMaterial(material);
                        if(mtoon != null && lilEditorGUI.Button(GetLoc("sConvertMToon"))) CreateMToonMaterial(material);
                        if(!isMulti && !isFur && !isRefr && !isGem && lilEditorGUI.Button(GetLoc("sConvertMulti"))) CreateMultiMaterial(material);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();

                        // Bake Textures
                        EditorGUILayout.BeginVertical(boxOuter);
                        EditorGUILayout.LabelField(GetLoc("sBake"), customToggleFont);
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        if(!isGem && lilEditorGUI.Button(GetLoc("sShadow1stColor")))   AutoBakeColoredMask(material, shadowColorTex,       shadowColor,        "Shadow1stColor");
                        if(!isGem && lilEditorGUI.Button(GetLoc("sShadow2ndColor")))   AutoBakeColoredMask(material, shadow2ndColorTex,    shadow2ndColor,     "Shadow2ndColor");
                        if(!isGem && lilEditorGUI.Button(GetLoc("sShadow3rdColor")))   AutoBakeColoredMask(material, shadow3rdColorTex,    shadow3rdColor,     "Shadow3rdColor");
                        if(!isGem && lilEditorGUI.Button(GetLoc("sReflection")))       AutoBakeColoredMask(material, reflectionColorTex,   reflectionColor,    "ReflectionColor");
                        if(lilEditorGUI.Button(GetLoc("sMatCap")))                     AutoBakeColoredMask(material, matcapBlendMask,      matcapColor,        "MatCapColor");
                        if(lilEditorGUI.Button(GetLoc("sMatCap2nd")))                  AutoBakeColoredMask(material, matcap2ndBlendMask,   matcap2ndColor,     "MatCap2ndColor");
                        if(lilEditorGUI.Button(GetLoc("sRimLight")))                   AutoBakeColoredMask(material, rimColorTex,          rimColor,           "RimColor");
                        if(((!isRefr && !isFur && !isGem && !isCustomShader) || (isCustomShader && isOutl)) && lilEditorGUI.EditorButton(GetLoc("sSettingTexOutlineColor"))) AutoBakeColoredMask(material, outlineColorMask, outlineColor, "OutlineColor");
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }
                }
            }
        }
    }
}
#endif
