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

        private void DrawRenderingModeSettings(Material material, string sTransparentMode, string[] sRenderingModeList, string[] sRenderingModeListLite)
        {
            if(isMultiVariants)
            {
                GUI.enabled = false;
                EditorGUI.showMixedValue = true;
                lilEditorGUI.Popup(GetLoc("sRenderingMode"), (int)renderingModeBuf, sRenderingModeList);
                EditorGUI.showMixedValue = false;
                GUI.enabled = true;
            }
            else
            {
                if(isShowRenderMode && !isMulti)
                {
                    RenderingMode renderingMode;
                    if(isLite)  renderingMode = (RenderingMode)lilEditorGUI.Popup(GetLoc("sRenderingMode"), (int)renderingModeBuf, sRenderingModeListLite);
                    else        renderingMode = (RenderingMode)lilEditorGUI.Popup(GetLoc("sRenderingMode"), (int)renderingModeBuf, sRenderingModeList);
                    if(renderingModeBuf != renderingMode)
                    {
                        SetupMaterialWithRenderingMode(renderingMode, transparentModeBuf);
                        if(renderingMode == RenderingMode.Cutout || renderingMode == RenderingMode.FurCutout) cutoff.floatValue = 0.5f;
                        if(renderingMode == RenderingMode.Transparent || renderingMode == RenderingMode.Fur || renderingMode == RenderingMode.FurTwoPass) cutoff.floatValue = 0.001f;
                        if(transparentModeBuf == TransparentMode.TwoPass)
                        {
                            preCutoff.floatValue = 0.001f;
                            cull.floatValue = 2.0f;
                            preCull.floatValue = 1.0f;
                        }
                    }
                }
                else if(isShowRenderMode && isMulti)
                {
                    float transparentModeMatBuf = transparentModeMat.floatValue;
                    m_MaterialEditor.ShaderProperty(transparentModeMat, sTransparentMode);
                    if(transparentModeMatBuf != transparentModeMat.floatValue)
                    {
                        SetupMaterialWithRenderingMode(renderingModeBuf, transparentModeBuf);
                        if(transparentModeMat.floatValue == 1.0f || transparentModeMat.floatValue == 5.0f) cutoff.floatValue = 0.5f;
                        if(transparentModeMat.floatValue == 2.0f || transparentModeMat.floatValue == 4.0f) cutoff.floatValue = 0.001f;
                    }
                }
                if(renderingModeBuf == RenderingMode.Transparent)
                {
                    var transparentMode = (TransparentMode)lilEditorGUI.Popup(GetLoc("sTransparentMode"), (int)transparentModeBuf, sTransparentModeList);
                    if(transparentModeBuf != transparentMode)
                    {
                        SetupMaterialWithRenderingMode(renderingModeBuf, transparentMode);
                    }
                    if(transparentModeBuf == TransparentMode.OnePass && vertexLightStrength.floatValue != 1.0f && lilRenderPipelineReader.GetRP() == lilRenderPipeline.BRP && lilEditorGUI.AutoFixHelpBox(GetLoc("sHelpOnePassVertexLight")))
                    {
                        vertexLightStrength.floatValue = 1.0f;
                    }
                }
                if(renderingModeBuf == RenderingMode.Fur || renderingModeBuf == RenderingMode.FurCutout || renderingModeBuf == RenderingMode.FurTwoPass)
                {
                    EditorGUILayout.HelpBox(GetLoc("sHelpRenderingFur"), MessageType.Warning);
                }
                if(lilDirectoryManager.ExistsClusterCreatorKit())
                {
                    if(renderingModeBuf == RenderingMode.Refraction || renderingModeBuf == RenderingMode.RefractionBlur || renderingModeBuf == RenderingMode.Gem)
                    {
                        EditorGUILayout.HelpBox(GetLoc("sHelpGrabPass"), MessageType.Warning);
                    }
                    if(renderingModeBuf == RenderingMode.Fur || renderingModeBuf == RenderingMode.FurCutout || renderingModeBuf == RenderingMode.FurTwoPass)
                    {
                        EditorGUILayout.HelpBox(GetLoc("sHelpGeometryShader"), MessageType.Warning);
                    }
                }
            }
        }

        private void DrawBaseSettings(Material material, string sTransparentMode, string[] sRenderingModeList, string[] sRenderingModeListLite, string[] sTransparentModeList)
        {
            DrawRenderingModeSettings(material, sTransparentMode, sRenderingModeList, sRenderingModeListLite);

            if(!ShouldDrawBlock(PropertyBlock.Base)) return;

            EditorGUILayout.Space();
            GUILayout.Label(GetLoc("sBaseSetting"), boldLabel);

            edSet.isShowBase = lilEditorGUI.Foldout(GetLoc("sBaseSetting"), edSet.isShowBase);
            DrawMenuButton(GetLoc("sAnchorBaseSetting"), PropertyBlock.Base);
            if(edSet.isShowBase)
            {
                EditorGUILayout.BeginVertical(customBox);
                    if(isMulti)
                    {
                        LocalizedProperty(asOverlay);
                    }
                    if(isUseAlpha)
                    {
                        LocalizedProperty(cutoff);
                    }
                    if(!isGem && !isFakeShadow)
                    {
                        LocalizedProperty(cull);
                        EditorGUI.indentLevel++;
                        if(cull.floatValue == 1.0f && lilEditorGUI.AutoFixHelpBox(GetLoc("sHelpCullMode")))
                        {
                            cull.floatValue = 2.0f;
                        }
                        if(cull.floatValue <= 1.0f || transparentModeBuf == TransparentMode.TwoPass && preCull.floatValue <= 1.0f)
                        {
                            LocalizedProperty(flipNormal);
                            LocalizedProperty(backfaceForceShadow);
                            if(!isLite)
                            {
                                LocalizedPropertyColorWithAlpha(backfaceColor);
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
                    LocalizedProperty(invisible);
                    LocalizedProperty(zwrite);
                    if(zwrite.floatValue != 1.0f && !isGem && lilEditorGUI.AutoFixHelpBox(GetLoc("sHelpZWrite")))
                    {
                        zwrite.floatValue = 1.0f;
                    }
                    if(isMulti) LocalizedProperty(useClippingCanceller);
                    if (!isFakeShadow)
                    {
                        LocalizedProperty(aaStrength);
                        LocalizedProperty(envRimBorder);
                        LocalizedProperty(envRimBlur);
                    }
                    if (!isFakeShadow && renderingModeBuf == RenderingMode.Cutout || (isMulti && transparentModeMat.floatValue == 1.0f))
                    {
                        LocalizedProperty(useDither);
                        if (lilEditorGUI.CheckPropertyToDraw(ditherTex, ditherMaxValue) && useDither.floatValue == 1.0f)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUI.BeginChangeCheck();
                            LocalizedPropertyTexture(ditherContent, ditherTex);
                            if (EditorGUI.EndChangeCheck() && ditherTex.textureValue != null)
                            {
                                ditherMaxValue.floatValue = Mathf.Clamp(ditherTex.textureValue.width * ditherTex.textureValue.height - 1, 0, 255);
                            }
                            LocalizedProperty(ditherMaxValue);
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(16);
                            if (GUILayout.Button("x2")) { ditherTex.textureValue = AssetDatabase.LoadAssetAtPath<Texture2D>(lilDirectoryManager.GetMainFolderPath() + "/Texture/lil_bayer_2x2.png"); ditherMaxValue.floatValue = 3; }
                            if (GUILayout.Button("x4")) { ditherTex.textureValue = AssetDatabase.LoadAssetAtPath<Texture2D>(lilDirectoryManager.GetMainFolderPath() + "/Texture/lil_bayer_4x4.png"); ditherMaxValue.floatValue = 15; }
                            if (GUILayout.Button("x8")) { ditherTex.textureValue = AssetDatabase.LoadAssetAtPath<Texture2D>(lilDirectoryManager.GetMainFolderPath() + "/Texture/lil_bayer_8x8.png"); ditherMaxValue.floatValue = 63; }
                            if (GUILayout.Button("x16")) { ditherTex.textureValue = AssetDatabase.LoadAssetAtPath<Texture2D>(lilDirectoryManager.GetMainFolderPath() + "/Texture/lil_bayer_16x16.png"); ditherMaxValue.floatValue = 255; }
                            EditorGUILayout.EndHorizontal();
                            EditorGUI.indentLevel--;
                        }
                    }
                    RenderQueueField();
                    if((renderingModeBuf >= RenderingMode.Transparent && renderingModeBuf != RenderingMode.FurCutout) || (isMulti && transparentModeMat.floatValue == 2.0f))
                    {
                        #if LILTOON_VRCSDK3_WORLDS
                            if(material.renderQueue <= 2999 && zwrite.floatValue == 1.0f)
                            {
                                EditorGUILayout.HelpBox(GetLoc("sHelpTransparentForWorld"),MessageType.Warning);
                            }
                        #else
                            EditorGUILayout.HelpBox(GetLoc("sHelpRenderingTransparent"),MessageType.Warning);
                        #endif
                    }
                    if(isLite)
                    {
                        lilEditorGUI.DrawLine();
                        LocalizedPropertyTexture(triMaskContent, triMask);
                    }
                EditorGUILayout.EndVertical();

                if(transparentModeBuf == TransparentMode.TwoPass)
                {
                    EditorGUILayout.LabelField("PrePass");
                    EditorGUILayout.BeginVertical(customBox);
                    LocalizedProperty(preOutType);

                    int preBlendMode = -1;
                    if(preSrcBlend.floatValue == 1.0f && preDstBlend.floatValue == 10.0f) preBlendMode = 0; // Normal
                    if(preSrcBlend.floatValue == 1.0f && preDstBlend.floatValue == 1.0f)  preBlendMode = 1; // Add
                    if(preSrcBlend.floatValue == 1.0f && preDstBlend.floatValue == 6.0f)  preBlendMode = 2; // Screen
                    if(preSrcBlend.floatValue == 0.0f && preDstBlend.floatValue == 3.0f)  preBlendMode = 3; // Mul
                    EditorGUI.BeginChangeCheck();
                    preBlendMode = lilEditorGUI.Popup(Event.current.alt ? preSrcBlend.name + ", " + preDstBlend.name : GetLoc("sBlendMode"), preBlendMode, sBlendModeList);
                    if(EditorGUI.EndChangeCheck())
                    {
                        switch(preBlendMode)
                        {
                            case 0:
                                preSrcBlend.floatValue = 1.0f;
                                preDstBlend.floatValue = 10.0f;
                                break;
                            case 1:
                                preSrcBlend.floatValue = 1.0f;
                                preDstBlend.floatValue = 1.0f;
                                break;
                            case 2:
                                preSrcBlend.floatValue = 1.0f;
                                preDstBlend.floatValue = 6.0f;
                                break;
                            case 3:
                                preSrcBlend.floatValue = 0.0f;
                                preDstBlend.floatValue = 3.0f;
                                break;
                            default:
                                break;
                        }
                    }

                    LocalizedProperty(preCull);
                    LocalizedProperty(preZwrite);
                    LocalizedPropertyColorWithAlpha(preColor);
                    LocalizedProperty(preCutoff);

                    edSet.isShowPrePreset = lilEditorGUI.DrawSimpleFoldout(GetLoc("sPresets"), edSet.isShowPrePreset, isCustomEditor);
                    if(edSet.isShowPrePreset)
                    {
                        EditorGUI.indentLevel++;
                        if(lilEditorGUI.Button(GetLoc("sTransparentPresetsPreWriteDepth")))
                        {
                            preColor.colorValue = Color.white;
                            preOutType.floatValue = 2.0f;
                            preCutoff.floatValue = -0.001f;
                            preSrcBlend.floatValue = 0.0f;
                            preDstBlend.floatValue = 3.0f;
                            preZwrite.floatValue = 1.0f;
                            preCull.floatValue = cull.floatValue;
                            preStencilRef.floatValue = stencilRef.floatValue;
                            preStencilComp.floatValue = stencilComp.floatValue;
                            mainColor.colorValue = new Color(mainColor.colorValue.r, mainColor.colorValue.g, mainColor.colorValue.b, 1.0f);
                            ztest.floatValue = (float)CompareFunction.LessEqual;
                        }
                        if(lilEditorGUI.Button(GetLoc("sTransparentPresetsColorTransparent")))
                        {
                            preColor.colorValue = new Color(0.75f,0.0f,0.0f,1.0f);
                            preOutType.floatValue = 1.0f;
                            preCutoff.floatValue = -0.001f;
                            preSrcBlend.floatValue = 0.0f;
                            preDstBlend.floatValue = 3.0f;
                            preZwrite.floatValue = 0.0f;
                            preCull.floatValue = cull.floatValue;
                            preStencilRef.floatValue = stencilRef.floatValue;
                            preStencilComp.floatValue = stencilComp.floatValue;
                            mainColor.colorValue = new Color(mainColor.colorValue.r, mainColor.colorValue.g, mainColor.colorValue.b, 0.0f);
                            cutoff.floatValue = -0.001f;
                            ztest.floatValue = (float)CompareFunction.LessEqual;
                        }
                        if(lilEditorGUI.Button(GetLoc("sTransparentPresetsBackAndFront")))
                        {
                            preColor.colorValue = Color.white;
                            preOutType.floatValue = 0.0f;
                            preCutoff.floatValue = cutoff.floatValue;
                            preSrcBlend.floatValue = 1.0f;
                            preDstBlend.floatValue = 10.0f;
                            preZwrite.floatValue = 1.0f;
                            preCull.floatValue = 1.0f;
                            preStencilRef.floatValue = stencilRef.floatValue;
                            preStencilComp.floatValue = stencilComp.floatValue;
                            mainColor.colorValue = new Color(mainColor.colorValue.r, mainColor.colorValue.g, mainColor.colorValue.b, 1.0f);
                            cull.floatValue = 0.0f;
                            ztest.floatValue = (float)CompareFunction.Less;
                        }
                        if(lilEditorGUI.Button(GetLoc("sTransparentPresetsCutoutAndTransparent")))
                        {
                            preColor.colorValue = Color.white;
                            preOutType.floatValue = 0.0f;
                            preCutoff.floatValue = 0.95f;
                            preSrcBlend.floatValue = 1.0f;
                            preDstBlend.floatValue = 10.0f;
                            preZwrite.floatValue = 1.0f;
                            preCull.floatValue = cull.floatValue;
                            preStencilRef.floatValue = stencilRef.floatValue;
                            preStencilComp.floatValue = stencilComp.floatValue;
                            mainColor.colorValue = new Color(mainColor.colorValue.r, mainColor.colorValue.g, mainColor.colorValue.b, 1.0f);
                            ztest.floatValue = (float)CompareFunction.LessEqual;
                        }
                        if(lilEditorGUI.Button(GetLoc("sTransparentPresetsFadeStencil")))
                        {
                            preColor.colorValue = new Color(1.0f,1.0f,1.0f,0.5f);
                            preOutType.floatValue = 0.0f;
                            preCutoff.floatValue = cutoff.floatValue;
                            preSrcBlend.floatValue = 1.0f;
                            preDstBlend.floatValue = 10.0f;
                            preZwrite.floatValue = 1.0f;
                            preCull.floatValue = cull.floatValue;
                            preStencilRef.floatValue = stencilRef.floatValue;
                            preStencilComp.floatValue = (float)CompareFunction.Equal;
                            mainColor.colorValue = new Color(mainColor.colorValue.r, mainColor.colorValue.g, mainColor.colorValue.b, 1.0f);
                            ztest.floatValue = (float)CompareFunction.Less;
                        }
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.LabelField(GetLoc("sSimpleStencilSettings"));
                EditorGUILayout.BeginVertical(customBox);
                    int stencilMode = -1;
                    if(stencilComp.floatValue == (float)CompareFunction.Always    && stencilPass.floatValue == (float)StencilOp.Keep)       stencilMode = 0; // Normal
                    if(stencilComp.floatValue == (float)CompareFunction.Always    && stencilPass.floatValue == (float)StencilOp.Replace)    stencilMode = 1; // Writer
                    if(stencilComp.floatValue == (float)CompareFunction.NotEqual  && stencilPass.floatValue == (float)StencilOp.Keep)       stencilMode = 2; // Reader
                    if(stencilComp.floatValue == (float)CompareFunction.Equal     && stencilPass.floatValue == (float)StencilOp.Keep)       stencilMode = 3; // Reader (Invert)
                    if(transparentModeBuf == TransparentMode.TwoPass &&
                    stencilComp.floatValue == (float)CompareFunction.Always    && stencilPass.floatValue == (float)StencilOp.Keep &&
                    preStencilComp.floatValue == (float)CompareFunction.Equal && preStencilPass.floatValue == (float)StencilOp.Keep
                    ) stencilMode = 4; // Reader (Fade)

                    int outlineStencilMode = -1;

                    EditorGUI.BeginChangeCheck();
                    if(transparentModeBuf == TransparentMode.TwoPass)   stencilMode = lilEditorGUI.Popup("Mode", stencilMode, new[]{GetLoc("sStencilModeNormal"),GetLoc("sStencilModeWriter"),GetLoc("sStencilModeReader"),GetLoc("sStencilModeReaderInvert"),GetLoc("sStencilModeReaderFade")});
                    else                                                stencilMode = lilEditorGUI.Popup("Mode", stencilMode, new[]{GetLoc("sStencilModeNormal"),GetLoc("sStencilModeWriter"),GetLoc("sStencilModeReader"),GetLoc("sStencilModeReaderInvert")});
                    if(isOutl)
                    {
                        if(outlineStencilComp.floatValue == (float)CompareFunction.Always     && outlineStencilPass.floatValue == (float)StencilOp.Keep)    outlineStencilMode = 0; // Normal
                        if(outlineStencilComp.floatValue == (float)CompareFunction.Always     && outlineStencilPass.floatValue == (float)StencilOp.Replace) outlineStencilMode = 1; // Writer
                        if(outlineStencilComp.floatValue == (float)CompareFunction.NotEqual   && outlineStencilPass.floatValue == (float)StencilOp.Keep)    outlineStencilMode = 2; // Reader
                        if(outlineStencilComp.floatValue == (float)CompareFunction.Equal      && outlineStencilPass.floatValue == (float)StencilOp.Keep)    outlineStencilMode = 3; // Reader (Invert)
                        outlineStencilMode = lilEditorGUI.Popup("Mode (" + GetLoc("sOutline") + ")", outlineStencilMode, new[]{GetLoc("sStencilModeNormal"),GetLoc("sStencilModeWriter"),GetLoc("sStencilModeReader"),GetLoc("sStencilModeReaderInvert")});
                    }
                    if(EditorGUI.EndChangeCheck())
                    {
                        SetupMaterialWithRenderingMode(renderingModeBuf, transparentModeBuf);
                        int shaderRenderQueue = isMulti ? material.renderQueue : material.shader.renderQueue;
                        switch(stencilMode)
                        {
                            case 0:
                                stencilRef.floatValue = 0;
                                stencilComp.floatValue = (float)CompareFunction.Always;
                                stencilPass.floatValue = (float)StencilOp.Keep;
                                if(!isMulti) material.renderQueue = -1;
                                break;
                            case 1: // Writer
                                stencilComp.floatValue = (float)CompareFunction.Always;
                                stencilPass.floatValue = (float)StencilOp.Replace;
                                material.renderQueue = shaderRenderQueue > 2451 ? -1 : 2451;
                                break;
                            case 2: // Reader
                                stencilComp.floatValue = (float)CompareFunction.NotEqual;
                                stencilPass.floatValue = (float)StencilOp.Keep;
                                material.renderQueue = shaderRenderQueue > 2452 ? -1 : 2452;
                                break;
                            case 3: // Reader (Invert)
                                stencilComp.floatValue = (float)CompareFunction.Equal;
                                stencilPass.floatValue = (float)StencilOp.Keep;
                                material.renderQueue = shaderRenderQueue > 2452 ? -1 : 2452;
                                break;
                            case 4: // Reader (Fade)
                                stencilComp.floatValue = (float)CompareFunction.Always;
                                stencilPass.floatValue = (float)StencilOp.Keep;
                                material.renderQueue = shaderRenderQueue > 2452 ? -1 : 2452;
                                break;
                            default:
                                break;
                        }
                        if(stencilMode != 0 && stencilRef.floatValue == 0) stencilRef.floatValue = 1;
                        stencilReadMask.floatValue = 255.0f;
                        stencilWriteMask.floatValue = 255.0f;
                        stencilFail.floatValue = (float)StencilOp.Keep;
                        stencilZFail.floatValue = (float)StencilOp.Keep;
                        if(isOutl)
                        {
                            switch(outlineStencilMode)
                            {
                                case 0:
                                    outlineStencilComp.floatValue = (float)CompareFunction.Always;
                                    outlineStencilPass.floatValue = (float)StencilOp.Keep;
                                    break;
                                case 1: // Writer
                                    outlineStencilComp.floatValue = (float)CompareFunction.Always;
                                    outlineStencilPass.floatValue = (float)StencilOp.Replace;
                                    break;
                                case 2: // Reader
                                    outlineStencilComp.floatValue = (float)CompareFunction.NotEqual;
                                    outlineStencilPass.floatValue = (float)StencilOp.Keep;
                                    break;
                                case 3: // Reader (Invert)
                                    outlineStencilComp.floatValue = (float)CompareFunction.Equal;
                                    outlineStencilPass.floatValue = (float)StencilOp.Keep;
                                    break;
                                default:
                                    break;
                            }
                            outlineStencilRef.floatValue = stencilRef.floatValue;
                            outlineStencilReadMask.floatValue = 255.0f;
                            outlineStencilWriteMask.floatValue = 255.0f;
                            outlineStencilFail.floatValue = (float)StencilOp.Keep;
                            outlineStencilZFail.floatValue = (float)StencilOp.Keep;
                        }
                        if(isFur)
                        {
                            furStencilRef.floatValue = stencilRef.floatValue;
                            furStencilComp.floatValue = stencilComp.floatValue;
                            furStencilPass.floatValue = stencilPass.floatValue;
                            furStencilReadMask.floatValue = stencilReadMask.floatValue;
                            furStencilWriteMask.floatValue = stencilWriteMask.floatValue;
                            furStencilFail.floatValue = stencilFail.floatValue;
                            furStencilZFail.floatValue = stencilZFail.floatValue;
                        }
                        if(transparentModeBuf == TransparentMode.TwoPass)
                        {
                            ztest.floatValue = stencilMode == 4 ? (float)CompareFunction.Less : (float)CompareFunction.LessEqual;
                            preStencilRef.floatValue = stencilRef.floatValue;
                            preStencilComp.floatValue = stencilMode == 4 ? (float)CompareFunction.Equal : stencilComp.floatValue;
                            preStencilPass.floatValue = stencilPass.floatValue;
                            preStencilReadMask.floatValue = stencilReadMask.floatValue;
                            preStencilWriteMask.floatValue = stencilWriteMask.floatValue;
                            preStencilFail.floatValue = stencilFail.floatValue;
                            preStencilZFail.floatValue = stencilZFail.floatValue;
                        }
                    }
                    if(stencilMode != 0 || isOutl && outlineStencilMode != 0)
                    {
                        EditorGUI.BeginChangeCheck();
                        LocalizedProperty(stencilRef);
                        if(EditorGUI.EndChangeCheck())
                        {
                            if(isOutl) outlineStencilRef.floatValue = stencilRef.floatValue;
                            if(isFur) furStencilRef.floatValue = stencilRef.floatValue;
                            if(transparentModeBuf == TransparentMode.TwoPass) preStencilRef.floatValue = stencilRef.floatValue;
                        }
                    }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawLightingSettings()
        {
            if(!ShouldDrawBlock(PropertyBlock.Lighting)) return;
            edSet.isShowLightingSettings = lilEditorGUI.Foldout(GetLoc("sLightingSettings"), edSet.isShowLightingSettings);
            DrawMenuButton(GetLoc("sAnchorLighting"), PropertyBlock.Lighting);
            if(edSet.isShowLightingSettings)
            {
                EditorGUILayout.LabelField(GetLoc("sBaseSetting"));
                EditorGUILayout.BeginVertical(customBox);
                    LocalizedProperty(lightMinLimit);
                    LocalizedProperty(lightMaxLimit);
                    LocalizedProperty(monochromeLighting);
                    if(shadowEnvStrength != null) LocalizedProperty(shadowEnvStrength);
                    var button = lilEditorGUI.Buttons(GetLoc("sLightingPreset"), GetLoc("sLightingPresetDefault"), GetLoc("sLightingPresetSemiMonochrome"));
                    if(button[0]) ApplyLightingPreset(LightingPreset.Default);
                    if(button[1]) ApplyLightingPreset(LightingPreset.SemiMonochrome);
                EditorGUILayout.EndVertical();

                EditorGUILayout.LabelField(GetLoc("sAdvanced"));
                EditorGUILayout.BeginVertical(customBox);
                    LocalizedProperty(asUnlit);
                    if(asUnlit.floatValue != 0 && lilEditorGUI.AutoFixHelpBox(GetLoc("sAsUnlitWarn")))
                    {
                        asUnlit.floatValue = 0.0f;
                    }
                    LocalizedProperty(vertexLightStrength);
                    LocalizedProperty(lightDirectionOverride);
                    if(isTransparent || (isFur && !isCutout)) LocalizedProperty(alphaBoostFA);
                    BlendOpFASetting();
                    LocalizedProperty(beforeExposureLimit);
                    LocalizedProperty(lilDirectionalLightStrength);
                EditorGUILayout.EndVertical();
            }
        }

        private void BlendOpFASetting()
        {
            if(blendOpFA == null) return;
            int selecting = blendOpFA.floatValue == 0 ? 0 : (blendOpFA.floatValue == 4 ? 1 : 2);
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = blendOpFA.hasMixedValue;
            selecting = lilEditorGUI.Popup(Event.current.alt ? blendOpFA.name : GetLoc("sLightBlending"), selecting, new string[]{GetLoc("sBlendingAdd"), GetLoc("sBlendingMax")});
            EditorGUI.showMixedValue = false;

            if(EditorGUI.EndChangeCheck())
            {
                blendOpFA.floatValue = selecting == 0 ? 0 : (selecting == 1 ? 4 : blendOpFA.floatValue);
            }
        }
    }
}
#endif
