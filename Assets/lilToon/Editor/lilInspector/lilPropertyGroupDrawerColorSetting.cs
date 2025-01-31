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

        private void DrawMainAdjustSettings(Material material)
        {
            edSet.isShowMainTone = lilEditorGUI.DrawSimpleFoldout(GetLoc("sColorAdjust"), edSet.isShowMainTone, isCustomEditor);
            if(edSet.isShowMainTone)
            {
                LocalizedPropertyTexture(maskBlendContent, mainColorAdjustMask);
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("HSV / Gamma", boldLabel);
                ToneCorrectionGUI(mainTexHSVG, 1);
                //EditorGUILayout.LabelField(GetLoc("sGradationMap"), boldLabel);
                //LocalizedProperty(mainGradationStrength);
                lilEditorGUI.DrawLine();
                LocalizedPropertyTexture(gradationMapContent, mainGradationTex, mainGradationStrength);
                if(mainGradationStrength.floatValue != 0 && (lilEditorGUI.CheckPropertyToDraw(gradationMapContent) || lilEditorGUI.CheckPropertyToDraw(mainGradationTex)))
                {
                    //LocalizedPropertyTexture(gradationContent, mainGradationTex);
                    EditorGUI.indentLevel++;
                    lilTextureUtils.GradientEditor(material, mainGrad, mainGradationTex, true);
                    EditorGUI.indentLevel--;
                }
                lilEditorGUI.DrawLine();
                TextureBakeGUI(material, 4);
                EditorGUI.indentLevel--;
            }
        }

        private void DrawAlphaMaskSettings(Material material)
        {
            if(!ShouldDrawBlock(PropertyBlock.AlphaMask)) return;
            if((renderingModeBuf == RenderingMode.Opaque && !isMulti) || (isMulti && transparentModeMat.floatValue == 0.0f))
            {
                GUILayout.Label(GetLoc("sAlphaMaskWarnOpaque"), wrapLabel);
            }
            else
            {
                EditorGUILayout.BeginVertical(boxOuter);
                LocalizedProperty(alphaMaskMode, false);
                DrawMenuButton(GetLoc("sAnchorAlphaMask"), PropertyBlock.AlphaMask);
                if(alphaMaskMode.floatValue != 0)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    LocalizedPropertyTexture(alphaMaskContent, alphaMask);
                    UVSettingGUI(alphaMask);

                    bool invertAlphaMask = alphaMaskScale.floatValue < 0;
                    float transparency = alphaMaskValue.floatValue - (invertAlphaMask ? 1.0f : 0.0f);

                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = alphaMaskScale.hasMixedValue || alphaMaskValue.hasMixedValue;
                    invertAlphaMask = lilEditorGUI.Toggle(Event.current.alt ? alphaMaskScale.name : "Invert", invertAlphaMask);
                    transparency = lilEditorGUI.Slider(Event.current.alt ? alphaMaskScale.name + ", " + alphaMaskValue.name : "Transparency", transparency, -1.0f, 1.0f);
                    EditorGUI.showMixedValue = false;

                    if(EditorGUI.EndChangeCheck())
                    {
                        alphaMaskScale.floatValue = invertAlphaMask ? -1.0f : 1.0f;
                        alphaMaskValue.floatValue = transparency + (invertAlphaMask ? 1.0f : 0.0f);
                    }
                    LocalizedProperty(cutoff);

                    edSet.isAlphaMaskModeAdvanced = EditorGUILayout.Toggle("Show advanced editor", edSet.isAlphaMaskModeAdvanced);
                    if(edSet.isAlphaMaskModeAdvanced)
                    {
                        EditorGUI.indentLevel++;
                        LocalizedProperty(alphaMaskScale);
                        LocalizedProperty(alphaMaskValue);
                        EditorGUI.indentLevel--;
                    }
                    AlphamaskToTextureGUI(material);
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawShadowSettings()
        {
            if(!ShouldDrawBlock(PropertyBlock.Shadow)) return;
            edSet.isShowShadow = lilEditorGUI.Foldout(GetLoc("sShadowSetting"), edSet.isShowShadow);
            DrawMenuButton(GetLoc("sAnchorShadow"), PropertyBlock.Shadow);
            if(edSet.isShowShadow)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                LocalizedProperty(useShadow, false);
                DrawMenuButton(GetLoc("sAnchorShadow"), PropertyBlock.Shadow);
                if(useShadow.floatValue == 1 && !isLite)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    LocalizedProperty(shadowMaskType);
                    if(shadowMaskType.floatValue == 1.0f)
                    {
                        LocalizedPropertyTexture(maskBlendContent, shadowStrengthMask);
                        EditorGUI.indentLevel += 2;
                            LocalizedProperty(shadowStrengthMaskLOD);
                            LocalizedProperty(shadowFlatBorder);
                            LocalizedProperty(shadowFlatBlur);
                        EditorGUI.indentLevel -= 2;
                        LocalizedProperty(shadowStrength);
                    }
                    else if(shadowMaskType.floatValue == 2.0f)
                    {
                        LocalizedPropertyTexture(new GUIContent("SDF", "Right (R), Left (G)"), shadowStrengthMask);
                        EditorGUI.indentLevel += 2;
                            LocalizedProperty(shadowStrengthMaskLOD);
                            LocalizedProperty(shadowFlatBlur, "Blend Y Direction");
                        EditorGUI.indentLevel -= 2;
                        LocalizedProperty(shadowStrength);
                    }
                    else
                    {
                        LocalizedPropertyTexture(maskStrengthContent, shadowStrengthMask, shadowStrength);
                        LocalizedProperty(shadowStrengthMaskLOD, 2);
                    }
                    lilEditorGUI.DrawLine();
                    LocalizedProperty(shadowColorType);
                    LocalizedPropertyTexture(shadow1stColorRGBAContent, shadowColorTex, shadowColor);
                    EditorGUI.indentLevel += 2;
                        LocalizedProperty(shadowBorder);
                        LocalizedProperty(shadowBlur);
                        LocalizedProperty(shadowNormalStrength);
                        LocalizedProperty(shadowReceive);
                    EditorGUI.indentLevel -= 2;
                    lilEditorGUI.DrawLine();
                    LocalizedPropertyTexture(shadow2ndColorRGBAContent, shadow2ndColorTex, shadow2ndColor);
                    EditorGUI.indentLevel += 2;
                        LocalizedPropertyAlpha(shadow2ndColor);
                        if(shadow2ndColor.colorValue.a > 0)
                        {
                            LocalizedProperty(shadow2ndBorder);
                            LocalizedProperty(shadow2ndBlur);
                            LocalizedProperty(shadow2ndNormalStrength);
                            LocalizedProperty(shadow2ndReceive);
                        }
                    EditorGUI.indentLevel -= 2;
                    lilEditorGUI.DrawLine();
                    LocalizedPropertyTexture(shadow3rdColorRGBAContent, shadow3rdColorTex, shadow3rdColor);
                    EditorGUI.indentLevel += 2;
                        LocalizedPropertyAlpha(shadow3rdColor);
                        if(shadow3rdColor.colorValue.a > 0)
                        {
                            LocalizedProperty(shadow3rdBorder);
                            LocalizedProperty(shadow3rdBlur);
                            LocalizedProperty(shadow3rdNormalStrength);
                            LocalizedProperty(shadow3rdReceive);
                        }
                    EditorGUI.indentLevel -= 2;
                    lilEditorGUI.DrawLine();
                    LocalizedProperty(shadowBorderColor);
                    LocalizedProperty(shadowBorderRange);
                    lilEditorGUI.DrawLine();
                    LocalizedProperty(shadowMainStrength);
                    LocalizedProperty(shadowEnvStrength);
                    LocalizedProperty(lilShadowCasterBias);
                    lilEditorGUI.DrawLine();
                    LocalizedPropertyTexture(blurMaskRGBContent, shadowBlurMask);
                    LocalizedProperty(shadowBlurMaskLOD, 2);
                    lilEditorGUI.DrawLine();
                    edSet.isShowShadowAO = lilEditorGUI.DrawSimpleFoldout(m_MaterialEditor, shadowAOMapContent, shadowBorderMask, edSet.isShowShadowAO, isCustomEditor);
                    if(edSet.isShowShadowAO)
                    {
                        EditorGUI.indentLevel += 1;
                        LocalizedProperty(shadowBorderMaskLOD);
                        LocalizedProperty(shadowPostAO);
                        float min1 = lilEditorGUI.GetRemapMinValue(shadowAOShift.vectorValue.x, shadowAOShift.vectorValue.y);
                        float max1 = lilEditorGUI.GetRemapMaxValue(shadowAOShift.vectorValue.x, shadowAOShift.vectorValue.y);
                        float min2 = lilEditorGUI.GetRemapMinValue(shadowAOShift.vectorValue.z, shadowAOShift.vectorValue.w);
                        float max2 = lilEditorGUI.GetRemapMaxValue(shadowAOShift.vectorValue.z, shadowAOShift.vectorValue.w);
                        float min3 = lilEditorGUI.GetRemapMinValue(shadowAOShift2.vectorValue.x, shadowAOShift2.vectorValue.y);
                        float max3 = lilEditorGUI.GetRemapMaxValue(shadowAOShift2.vectorValue.x, shadowAOShift2.vectorValue.y);
                        EditorGUI.BeginChangeCheck();
                        EditorGUI.showMixedValue = alphaMaskScale.hasMixedValue || alphaMaskValue.hasMixedValue;
                        min1 = lilEditorGUI.Slider(Event.current.alt ? shadowAOShift.name : "1st Min", min1, -0.01f, 1.01f);
                        max1 = lilEditorGUI.Slider(Event.current.alt ? shadowAOShift.name : "1st Max", max1, -0.01f, 1.01f);
                        min2 = lilEditorGUI.Slider(Event.current.alt ? shadowAOShift.name : "2nd Min", min2, -0.01f, 1.01f);
                        max2 = lilEditorGUI.Slider(Event.current.alt ? shadowAOShift.name : "2nd Max", max2, -0.01f, 1.01f);
                        min3 = lilEditorGUI.Slider(Event.current.alt ? shadowAOShift2.name : "3rd Min", min3, -0.01f, 1.01f);
                        max3 = lilEditorGUI.Slider(Event.current.alt ? shadowAOShift2.name : "3rd Max", max3, -0.01f, 1.01f);
                        EditorGUI.showMixedValue = false;

                        if(EditorGUI.EndChangeCheck())
                        {
                            if(min1 == max1) max1 += 0.001f;
                            if(min2 == max2) max2 += 0.001f;
                            if(min3 == max3) max3 += 0.001f;
                            shadowAOShift.vectorValue = new Vector4(
                                lilEditorGUI.GetRemapScaleValue(min1, max1),
                                lilEditorGUI.GetRemapOffsetValue(min1, max1),
                                lilEditorGUI.GetRemapScaleValue(min2, max2),
                                lilEditorGUI.GetRemapOffsetValue(min2, max2)
                            );
                            shadowAOShift2.vectorValue = new Vector4(
                                lilEditorGUI.GetRemapScaleValue(min3, max3),
                                lilEditorGUI.GetRemapOffsetValue(min3, max3),
                                0.0f,
                                0.0f
                            );
                        }
                        EditorGUI.indentLevel -= 1;
                    }
                    EditorGUILayout.EndVertical();
                }
                else if(useShadow.floatValue == 1)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    LocalizedPropertyTexture(shadow1stColorRGBAContent, shadowColorTex);
                    EditorGUI.indentLevel += 2;
                    LocalizedProperty(shadowBorder);
                    LocalizedProperty(shadowBlur);
                    EditorGUI.indentLevel -= 2;
                    lilEditorGUI.DrawLine();
                    LocalizedPropertyTexture(shadow2ndColorRGBAContent, shadow2ndColorTex);
                    EditorGUI.indentLevel += 2;
                    LocalizedProperty(shadow2ndBorder);
                    LocalizedProperty(shadow2ndBlur);
                    EditorGUI.indentLevel -= 2;
                    lilEditorGUI.DrawLine();
                    LocalizedProperty(shadowEnvStrength);
                    LocalizedProperty(shadowBorderColor);
                    LocalizedProperty(shadowBorderRange);
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawShadowSettingsSimple()
        {
            if(!ShouldDrawBlock(PropertyBlock.Shadow)) return;
            edSet.isShowShadow = lilEditorGUI.Foldout(GetLoc("sShadowSetting"), edSet.isShowShadow);
            DrawMenuButton(GetLoc("sAnchorShadow"), PropertyBlock.Shadow);
            if(edSet.isShowShadow)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                LocalizedProperty(useShadow, false);
                DrawMenuButton(GetLoc("sAnchorShadow"), PropertyBlock.Shadow);
                if(useShadow.floatValue == 1 && !isLite)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    LocalizedProperty(shadowMaskType);
                    if(shadowMaskType.floatValue == 1.0f)
                    {
                        LocalizedPropertyTexture(maskBlendContent, shadowStrengthMask);
                        EditorGUI.indentLevel += 2;
                            LocalizedProperty(shadowFlatBorder);
                            LocalizedProperty(shadowFlatBlur);
                        EditorGUI.indentLevel -= 2;
                        LocalizedProperty(shadowStrength);
                    }
                    else if(shadowMaskType.floatValue == 2.0f)
                    {
                        LocalizedPropertyTexture(new GUIContent("SDF", "Right (R), Left (G)"), shadowStrengthMask);
                        EditorGUI.indentLevel += 2;
                            LocalizedProperty(shadowStrengthMaskLOD);
                            LocalizedProperty(shadowFlatBlur, "Blend Y Direction");
                        EditorGUI.indentLevel -= 2;
                        LocalizedProperty(shadowStrength);
                    }
                    else
                    {
                        LocalizedPropertyTexture(maskStrengthContent, shadowStrengthMask, shadowStrength);
                    }
                    lilEditorGUI.DrawLine();
                    LocalizedProperty(shadowColorType);
                    LocalizedPropertyTexture(shadow1stColorRGBAContent, shadowColorTex, shadowColor);
                    EditorGUI.indentLevel += 2;
                        LocalizedProperty(shadowBorder);
                        LocalizedProperty(shadowBlur);
                        LocalizedProperty(shadowReceive);
                    EditorGUI.indentLevel -= 2;
                    lilEditorGUI.DrawLine();
                    LocalizedPropertyTexture(shadow2ndColorRGBAContent, shadow2ndColorTex, shadow2ndColor);
                    EditorGUI.indentLevel += 2;
                        LocalizedPropertyAlpha(shadow2ndColor);
                        if(shadow2ndColor.colorValue.a > 0)
                        {
                            LocalizedProperty(shadow2ndBorder);
                            LocalizedProperty(shadow2ndBlur);
                            LocalizedProperty(shadow2ndReceive);
                        }
                    EditorGUI.indentLevel -= 2;
                    lilEditorGUI.DrawLine();
                    LocalizedPropertyTexture(shadow3rdColorRGBAContent, shadow3rdColorTex, shadow3rdColor);
                    EditorGUI.indentLevel += 2;
                        LocalizedPropertyAlpha(shadow3rdColor);
                        if(shadow3rdColor.colorValue.a > 0)
                        {
                            LocalizedProperty(shadow3rdBorder);
                            LocalizedProperty(shadow3rdBlur);
                            LocalizedProperty(shadow3rdReceive);
                        }
                    EditorGUI.indentLevel -= 2;
                    lilEditorGUI.DrawLine();
                    LocalizedProperty(shadowBorderColor);
                    LocalizedProperty(shadowBorderRange);
                    lilEditorGUI.DrawLine();
                    LocalizedProperty(shadowMainStrength);
                    EditorGUILayout.EndVertical();
                }
                else if(useShadow.floatValue == 1)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    LocalizedPropertyTexture(shadow1stColorRGBAContent, shadowColorTex);
                    EditorGUI.indentLevel += 2;
                        LocalizedProperty(shadowBorder);
                        LocalizedProperty(shadowBlur);
                    EditorGUI.indentLevel -= 2;
                    lilEditorGUI.DrawLine();
                    LocalizedPropertyTexture(shadow2ndColorRGBAContent, shadow2ndColorTex);
                    EditorGUI.indentLevel += 2;
                        LocalizedProperty(shadow2ndBorder);
                        LocalizedProperty(shadow2ndBlur);
                    EditorGUI.indentLevel -= 2;
                    lilEditorGUI.DrawLine();
                    LocalizedProperty(shadowEnvStrength);
                    LocalizedProperty(shadowBorderColor);
                    LocalizedProperty(shadowBorderRange);
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }
        }

    }
}
#endif
