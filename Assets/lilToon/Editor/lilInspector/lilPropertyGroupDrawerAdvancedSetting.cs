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
        private void DrawOutlineSettings(Material material)
        {
            if(!ShouldDrawBlock(PropertyBlock.Outline)) return;
            if(isMultiVariants || isRefr || isFur || isGem || isFakeShadow || material.shader.name.Contains("Overlay")) return;
            edSet.isShowOutline = lilEditorGUI.Foldout(GetLoc("sOutlineSetting"), edSet.isShowOutline);
            DrawMenuButton(GetLoc("sAnchorOutline"), PropertyBlock.Outline);
            if(edSet.isShowOutline)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                if(isShowRenderMode)
                {
                    if(isOutl != EditorGUILayout.ToggleLeft(GetLoc("sOutline"), isOutl, customToggleFont))
                    {
                        isOutl = !isOutl;
                        SetupMaterialWithRenderingMode(renderingModeBuf, transparentModeBuf);
                    }
                }
                else if(isCustomShader)
                {
                    EditorGUILayout.LabelField(GetLoc("sOutline"), customToggleFont);
                }
                if(!isLite && isOutl)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    TextureGUI(ref edSet.isShowOutlineMap, mainColorRGBAContent, outlineTex, outlineColor, outlineTex_ScrollRotate, true, true);
                    EditorGUI.indentLevel++;
                    ToneCorrectionGUI(outlineTexHSVG);
                    if(lilEditorGUI.Button(GetLoc("sBake")))
                    {
                        outlineTex.textureValue = AutoBakeOutlineTexture(material);
                        outlineTexHSVG.vectorValue = lilConstants.defaultHSVG;
                    }
                    EditorGUI.indentLevel--;
                    lilEditorGUI.DrawLine();
                    GUILayout.Label(GetLoc("sHighlight"), boldLabel);
                    EditorGUI.indentLevel++;
                    LocalizedPropertyColorWithAlpha(outlineLitColor);
                    if(outlineLitColor.colorValue.a > 0)
                    {
                        LocalizedProperty(outlineLitApplyTex);
                        float min = lilEditorGUI.GetRemapMinValue(outlineLitScale.floatValue, outlineLitOffset.floatValue);
                        float max = lilEditorGUI.GetRemapMaxValue(outlineLitScale.floatValue, outlineLitOffset.floatValue);
                        EditorGUI.BeginChangeCheck();
                        EditorGUI.showMixedValue = alphaMaskScale.hasMixedValue || alphaMaskValue.hasMixedValue;
                        min = lilEditorGUI.Slider(Event.current.alt ? outlineLitScale.name + ", " + outlineLitOffset.name : "Min", min, -0.01f, 1.01f);
                        max = lilEditorGUI.Slider(Event.current.alt ? outlineLitScale.name + ", " + outlineLitOffset.name : "Max", max, -0.01f, 1.01f);
                        EditorGUI.showMixedValue = false;
                        if(EditorGUI.EndChangeCheck())
                        {
                            if(min == max) max += 0.001f;
                            outlineLitScale.floatValue = lilEditorGUI.GetRemapScaleValue(min, max);
                            outlineLitOffset.floatValue = lilEditorGUI.GetRemapOffsetValue(min, max);
                        }
                        LocalizedProperty(outlineLitShadowReceive);
                    }
                    EditorGUI.indentLevel--;
                    lilEditorGUI.DrawLine();
                    LocalizedProperty(outlineEnableLighting);
                    lilEditorGUI.DrawLine();
                    LocalizedPropertyTexture(widthMaskContent, outlineWidthMask, outlineWidth);
                    EditorGUI.indentLevel++;
                    LocalizedProperty(outlineFixWidth);
                    LocalizedProperty(outlineVertexR2Width);
                    LocalizedProperty(outlineDeleteMesh);
                    LocalizedProperty(outlineZBias);
                    LocalizedProperty(outlineDisableInVR);
                    EditorGUI.indentLevel--;
                    LocalizedPropertyTexture(normalMapContent, outlineVectorTex, outlineVectorScale);
                    LocalizedProperty(outlineVectorUVMode, 2);
                    EditorGUILayout.EndVertical();
                }
                else if(isOutl)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    TextureGUI(ref edSet.isShowOutlineMap, mainColorRGBAContent, outlineTex, outlineColor, outlineTex_ScrollRotate, true, true);
                    LocalizedProperty(outlineEnableLighting);
                    lilEditorGUI.DrawLine();
                    LocalizedPropertyTexture(widthMaskContent, outlineWidthMask, outlineWidth);
                    EditorGUI.indentLevel++;
                    LocalizedProperty(outlineFixWidth);
                    LocalizedProperty(outlineVertexR2Width);
                    LocalizedProperty(outlineDeleteMesh);
                    LocalizedProperty(outlineZBias);
                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawSpecularMode()
        {
            int specularMode = 0;
            if(specularToon.floatValue == 0.0f) specularMode = 1;
            if(specularToon.floatValue == 1.0f) specularMode = 2;
            if(applySpecular.floatValue == 0.0f) specularMode = 0;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = specularToon.hasMixedValue || applySpecular.hasMixedValue;
            specularMode = lilEditorGUI.Popup(GetLoc("sSpecularMode"),specularMode,new string[]{GetLoc("sSpecularNone"),GetLoc("sSpecularReal"),GetLoc("sSpecularToon")});
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                if(specularMode == 0)
                {
                    applySpecular.floatValue = 0.0f;
                    specularToon.floatValue = 0.0f;
                }
                if(specularMode == 1)
                {
                    applySpecular.floatValue = 1.0f;
                    specularToon.floatValue = 0.0f;
                    EditorGUI.indentLevel++;
                    LocalizedProperty(specularNormalStrength);
                    LocalizedProperty(applySpecularFA);
                    EditorGUI.indentLevel--;
                }
                if(specularMode == 2)
                {
                    applySpecular.floatValue = 1.0f;
                    specularToon.floatValue = 1.0f;
                    EditorGUI.indentLevel++;
                    LocalizedProperty(specularNormalStrength);
                    LocalizedProperty(specularBorder);
                    LocalizedProperty(specularBlur);
                    LocalizedProperty(applySpecularFA);
                    EditorGUI.indentLevel--;
                }
            }

            if(specularMode == 1)
            {
                EditorGUI.indentLevel++;
                LocalizedProperty(specularNormalStrength);
                LocalizedProperty(applySpecularFA);
                EditorGUI.indentLevel--;
            }
            if(specularMode == 2)
            {
                EditorGUI.indentLevel++;
                LocalizedProperty(specularNormalStrength);
                LocalizedProperty(specularBorder);
                LocalizedProperty(specularBlur);
                LocalizedProperty(applySpecularFA);
                EditorGUI.indentLevel--;
            }
        }

        private void DrawStencilSettings(Material material)
        {
            if(!ShouldDrawBlock(PropertyBlock.Stencil)) return;
            edSet.isShowStencil = lilEditorGUI.Foldout(GetLoc("sStencilSetting"), edSet.isShowStencil);
            DrawMenuButton(GetLoc("sAnchorStencil"), PropertyBlock.Stencil);
            if(edSet.isShowStencil)
            {
                if(lilEditorGUI.Button("Reset"))
                {
                    isStWr = false;
                    stencilRef.floatValue = 0;
                    stencilReadMask.floatValue = 255.0f;
                    stencilWriteMask.floatValue = 255.0f;
                    stencilComp.floatValue = (float)CompareFunction.Always;
                    stencilPass.floatValue = (float)StencilOp.Keep;
                    stencilFail.floatValue = (float)StencilOp.Keep;
                    stencilZFail.floatValue = (float)StencilOp.Keep;
                    if(transparentModeBuf == TransparentMode.TwoPass)
                    {
                        preStencilRef.floatValue = 0;
                        preStencilReadMask.floatValue = 255.0f;
                        preStencilWriteMask.floatValue = 255.0f;
                        preStencilComp.floatValue = (float)CompareFunction.Always;
                        preStencilPass.floatValue = (float)StencilOp.Keep;
                        preStencilFail.floatValue = (float)StencilOp.Keep;
                        preStencilZFail.floatValue = (float)StencilOp.Keep;
                    }
                    if(isOutl)
                    {
                        outlineStencilRef.floatValue = 0;
                        outlineStencilReadMask.floatValue = 255.0f;
                        outlineStencilWriteMask.floatValue = 255.0f;
                        outlineStencilComp.floatValue = (float)CompareFunction.Always;
                        outlineStencilPass.floatValue = (float)StencilOp.Keep;
                        outlineStencilFail.floatValue = (float)StencilOp.Keep;
                        outlineStencilZFail.floatValue = (float)StencilOp.Keep;
                    }
                    if(isFur)
                    {
                        furStencilRef.floatValue = 0;
                        furStencilReadMask.floatValue = 255.0f;
                        furStencilWriteMask.floatValue = 255.0f;
                        furStencilComp.floatValue = (float)CompareFunction.Always;
                        furStencilPass.floatValue = (float)StencilOp.Keep;
                        furStencilFail.floatValue = (float)StencilOp.Keep;
                        furStencilZFail.floatValue = (float)StencilOp.Keep;
                    }
                }

                EditorGUILayout.BeginVertical(customBox);
                LocalizedProperty(stencilRef);
                LocalizedProperty(stencilReadMask);
                LocalizedProperty(stencilWriteMask);
                LocalizedProperty(stencilComp);
                LocalizedProperty(stencilPass);
                LocalizedProperty(stencilFail);
                LocalizedProperty(stencilZFail);
                EditorGUILayout.EndVertical();

                if(transparentModeBuf == TransparentMode.TwoPass)
                {
                    EditorGUILayout.LabelField("PrePass");
                    EditorGUILayout.BeginVertical(customBox);
                    LocalizedProperty(preStencilRef);
                    LocalizedProperty(preStencilReadMask);
                    LocalizedProperty(preStencilWriteMask);
                    LocalizedProperty(preStencilComp);
                    LocalizedProperty(preStencilPass);
                    LocalizedProperty(preStencilFail);
                    LocalizedProperty(preStencilZFail);
                    EditorGUILayout.EndVertical();
                }

                if(isOutl)
                {
                    EditorGUILayout.LabelField(GetLoc("sOutline"));
                    EditorGUILayout.BeginVertical(customBox);
                    LocalizedProperty(outlineStencilRef);
                    LocalizedProperty(outlineStencilReadMask);
                    LocalizedProperty(outlineStencilWriteMask);
                    LocalizedProperty(outlineStencilComp);
                    LocalizedProperty(outlineStencilPass);
                    LocalizedProperty(outlineStencilFail);
                    LocalizedProperty(outlineStencilZFail);
                    EditorGUILayout.EndVertical();
                }

                if(isFur)
                {
                    EditorGUILayout.LabelField(GetLoc("sFur"));
                    EditorGUILayout.BeginVertical(customBox);
                    LocalizedProperty(furStencilRef);
                    LocalizedProperty(furStencilReadMask);
                    LocalizedProperty(furStencilWriteMask);
                    LocalizedProperty(furStencilComp);
                    LocalizedProperty(furStencilPass);
                    LocalizedProperty(furStencilFail);
                    LocalizedProperty(furStencilZFail);
                    EditorGUILayout.EndVertical();
                }
            }
        }
    }
}
#endif
