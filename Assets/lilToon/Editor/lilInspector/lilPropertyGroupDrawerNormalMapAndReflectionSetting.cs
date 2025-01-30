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
        private void DrawMatCapSettings()
        {
            if(!ShouldDrawBlock(PropertyBlock.MatCaps)) return;
            edSet.isShowMatCap = lilEditorGUI.Foldout(GetLoc("sMatCapSetting"), edSet.isShowMatCap);
            DrawMenuButton(GetLoc("sAnchorMatCap"), PropertyBlock.MatCaps);
            if(edSet.isShowMatCap)
            {
                if(!isLite)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // MatCap
                    if(ShouldDrawBlock(PropertyBlock.MatCap1st))
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        LocalizedProperty(useMatCap, false);
                        DrawMenuButton(GetLoc("sAnchorMatCap"), PropertyBlock.MatCap1st);
                        if(useMatCap.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            MatCapTextureGUI(ref edSet.isShowMatCapUV, matcapContent, matcapTex, matcapColor, matcapBlendUV1, matcapZRotCancel, matcapPerspective, matcapVRParallaxStrength);
                            LocalizedPropertyAlpha(matcapColor);
                            LocalizedProperty(matcapMainStrength);
                            LocalizedProperty(matcapNormalStrength);
                            lilEditorGUI.DrawLine();
                            TextureGUI(ref edSet.isShowMatCapBlendMask, maskBlendRGBContent, matcapBlendMask, matcapBlend);
                            LocalizedProperty(matcapEnableLighting);
                            LocalizedProperty(matcapShadowMask);
                            LocalizedProperty(matcapBackfaceMask);
                            LocalizedProperty(matcapLod);
                            LocalizedProperty(matcapBlendMode);
                            if(matcapEnableLighting.floatValue != 0.0f && matcapBlendMode.floatValue == 3.0f && lilEditorGUI.AutoFixHelpBox(GetLoc("sHelpMatCapBlending")))
                            {
                                matcapEnableLighting.floatValue = 0.0f;
                            }
                            if(isTransparent) LocalizedProperty(matcapApplyTransparency);
                            lilEditorGUI.DrawLine();
                            LocalizedProperty(matcapCustomNormal);
                            if(matcapCustomNormal.floatValue == 1)
                            {
                                TextureGUI(ref edSet.isShowMatCapBumpMap, normalMapContent, matcapBumpMap, matcapBumpScale);
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // MatCap 2nd
                    if(ShouldDrawBlock(PropertyBlock.MatCap2nd))
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        LocalizedProperty(useMatCap2nd, false);
                        DrawMenuButton(GetLoc("sAnchorMatCap"), PropertyBlock.MatCap2nd);
                        if(useMatCap2nd.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            MatCapTextureGUI(ref edSet.isShowMatCap2ndUV, matcapContent, matcap2ndTex, matcap2ndColor, matcap2ndBlendUV1, matcap2ndZRotCancel, matcap2ndPerspective, matcap2ndVRParallaxStrength);
                            LocalizedPropertyAlpha(matcap2ndColor);
                            LocalizedProperty(matcap2ndMainStrength);
                            LocalizedProperty(matcap2ndNormalStrength);
                            lilEditorGUI.DrawLine();
                            TextureGUI(ref edSet.isShowMatCap2ndBlendMask, maskBlendRGBContent, matcap2ndBlendMask, matcap2ndBlend);
                            LocalizedProperty(matcap2ndEnableLighting);
                            LocalizedProperty(matcap2ndShadowMask);
                            LocalizedProperty(matcap2ndBackfaceMask);
                            LocalizedProperty(matcap2ndLod);
                            LocalizedProperty(matcap2ndBlendMode);
                            if(matcap2ndEnableLighting.floatValue != 0.0f && matcap2ndBlendMode.floatValue == 3.0f && lilEditorGUI.AutoFixHelpBox(GetLoc("sHelpMatCapBlending")))
                            {
                                matcap2ndEnableLighting.floatValue = 0.0f;
                            }
                            if(isTransparent) LocalizedProperty(matcap2ndApplyTransparency);
                            lilEditorGUI.DrawLine();
                            LocalizedProperty(matcap2ndCustomNormal);
                            if(matcap2ndCustomNormal.floatValue == 1)
                            {
                                TextureGUI(ref edSet.isShowMatCap2ndBumpMap, normalMapContent, matcap2ndBumpMap, matcap2ndBumpScale);
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                else
                {
                    if(ShouldDrawBlock(PropertyBlock.MatCap1st))
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        LocalizedProperty(useMatCap, false);
                        DrawMenuButton(GetLoc("sAnchorMatCap"), PropertyBlock.MatCap1st);
                        if(useMatCap.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            MatCapTextureGUI(ref edSet.isShowMatCapUV, matcapContent, matcapTex, matcapBlendUV1, matcapZRotCancel, matcapPerspective, matcapVRParallaxStrength);
                            LocalizedProperty(matcapMul);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
            }
        }

        private void DrawMatCapSettingsSimple()
        {
            if(!ShouldDrawBlock(PropertyBlock.MatCaps)) return;
            edSet.isShowMatCap = lilEditorGUI.Foldout(GetLoc("sMatCapSetting"), edSet.isShowMatCap);
            DrawMenuButton(GetLoc("sAnchorMatCap"), PropertyBlock.MatCaps);
            if(edSet.isShowMatCap)
            {
                if(!isLite)
                {
                    //------------------------------------------------------------------------------------------------------------------------------
                    // MatCap
                    if(ShouldDrawBlock(PropertyBlock.MatCap1st))
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        LocalizedProperty(useMatCap, false);
                        DrawMenuButton(GetLoc("sAnchorMatCap"), PropertyBlock.MatCap1st);
                        if(useMatCap.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            MatCapTextureGUI(ref edSet.isShowMatCapUV, matcapContent, matcapTex, matcapColor, matcapBlendUV1, matcapZRotCancel, matcapPerspective, matcapVRParallaxStrength);
                            LocalizedPropertyAlpha(matcapColor);
                            lilEditorGUI.DrawLine();
                            TextureGUI(ref edSet.isShowMatCapBlendMask, maskBlendRGBContent, matcapBlendMask, matcapBlend);
                            LocalizedProperty(matcapEnableLighting);
                            LocalizedProperty(matcapBlendMode);
                            if(matcapEnableLighting.floatValue != 0.0f && matcapBlendMode.floatValue == 3.0f && lilEditorGUI.AutoFixHelpBox(GetLoc("sHelpMatCapBlending")))
                            {
                                matcapEnableLighting.floatValue = 0.0f;
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }

                    //------------------------------------------------------------------------------------------------------------------------------
                    // MatCap 2nd
                    if(ShouldDrawBlock(PropertyBlock.MatCap2nd))
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        LocalizedProperty(useMatCap2nd, false);
                        DrawMenuButton(GetLoc("sAnchorMatCap"), PropertyBlock.MatCap2nd);
                        if(useMatCap2nd.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            MatCapTextureGUI(ref edSet.isShowMatCap2ndUV, matcapContent, matcap2ndTex, matcap2ndColor, matcap2ndBlendUV1, matcap2ndZRotCancel, matcap2ndPerspective, matcap2ndVRParallaxStrength);
                            LocalizedPropertyAlpha(matcap2ndColor);
                            lilEditorGUI.DrawLine();
                            TextureGUI(ref edSet.isShowMatCap2ndBlendMask, maskBlendRGBContent, matcap2ndBlendMask, matcap2ndBlend);
                            LocalizedProperty(matcap2ndEnableLighting);
                            LocalizedProperty(matcap2ndBlendMode);
                            if(matcap2ndEnableLighting.floatValue != 0.0f && matcap2ndBlendMode.floatValue == 3.0f && lilEditorGUI.AutoFixHelpBox(GetLoc("sHelpMatCapBlending")))
                            {
                                matcap2ndEnableLighting.floatValue = 0.0f;
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                else
                {
                    if(ShouldDrawBlock(PropertyBlock.MatCap1st))
                    {
                        EditorGUILayout.BeginVertical(boxOuter);
                        LocalizedProperty(useMatCap, false);
                        DrawMenuButton(GetLoc("sAnchorMatCap"), PropertyBlock.MatCap1st);
                        if(useMatCap.floatValue == 1)
                        {
                            EditorGUILayout.BeginVertical(boxInnerHalf);
                            MatCapTextureGUI(ref edSet.isShowMatCapUV, matcapContent, matcapTex, matcapBlendUV1, matcapZRotCancel, matcapPerspective, matcapVRParallaxStrength);
                            LocalizedProperty(matcapMul);
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
            }
        }

        private void DrawRimSettings()
        {
            if(!ShouldDrawBlock(PropertyBlock.RimLight)) return;
            edSet.isShowRim = lilEditorGUI.Foldout(GetLoc("sRimLightSetting"), edSet.isShowRim);
            DrawMenuButton(GetLoc("sAnchorRimLight"), PropertyBlock.RimLight);
            if(edSet.isShowRim)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                if(!isLite)
                {
                    LocalizedProperty(useRim, false);
                    DrawMenuButton(GetLoc("sAnchorRimLight"), PropertyBlock.RimLight);
                    if(useRim.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        TextureGUI(ref edSet.isShowRimColorTex, colorMaskRGBAContent, rimColorTex, rimColor);
                        LocalizedPropertyAlpha(rimColor);
                        LocalizedProperty(rimMainStrength);
                        LocalizedProperty(rimEnableLighting);
                        LocalizedProperty(rimShadowMask);
                        LocalizedProperty(rimBackfaceMask);
                        if(isTransparent) LocalizedProperty(rimApplyTransparency);
                        LocalizedProperty(rimBlendMode);
                        lilEditorGUI.DrawLine();
                        LocalizedProperty(rimDirStrength);
                        if(rimDirStrength.floatValue != 0)
                        {
                            EditorGUI.indentLevel++;
                            LocalizedProperty(rimDirRange);
                            lilEditorGUI.InvBorderGUI(rimBorder);
                            LocalizedProperty(rimBlur);
                            lilEditorGUI.DrawLine();
                            LocalizedProperty(rimIndirRange);
                            LocalizedProperty(rimIndirColor);
                            lilEditorGUI.InvBorderGUI(rimIndirBorder);
                            LocalizedProperty(rimIndirBlur);
                            EditorGUI.indentLevel--;
                            lilEditorGUI.DrawLine();
                        }
                        else
                        {
                            lilEditorGUI.InvBorderGUI(rimBorder);
                            LocalizedProperty(rimBlur);
                        }
                        LocalizedProperty(rimNormalStrength);
                        LocalizedProperty(rimFresnelPower);
                        LocalizedProperty(rimVRParallaxStrength);
                        EditorGUILayout.EndVertical();
                    }
                }
                else
                {
                    LocalizedProperty(useRim, false);
                    DrawMenuButton(GetLoc("sAnchorRimLight"), PropertyBlock.RimLight);
                    if(useRim.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        LocalizedProperty(rimColor);
                        LocalizedProperty(rimShadowMask);
                        lilEditorGUI.DrawLine();
                        lilEditorGUI.InvBorderGUI(rimBorder);
                        LocalizedProperty(rimBlur);
                        LocalizedProperty(rimFresnelPower);
                        EditorGUILayout.EndVertical();
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawRimSettingsSimple()
        {
            if(!ShouldDrawBlock(PropertyBlock.RimLight)) return;
            edSet.isShowRim = lilEditorGUI.Foldout(GetLoc("sRimLightSetting"), edSet.isShowRim);
            DrawMenuButton(GetLoc("sAnchorRimLight"), PropertyBlock.RimLight);
            if(edSet.isShowRim)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                if(!isLite)
                {
                    LocalizedProperty(useRim, false);
                    DrawMenuButton(GetLoc("sAnchorRimLight"), PropertyBlock.RimLight);
                    if(useRim.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        TextureGUI(ref edSet.isShowRimColorTex, colorMaskRGBAContent, rimColorTex, rimColor);
                        LocalizedPropertyAlpha(rimColor);
                        LocalizedProperty(rimBlendMode);
                        lilEditorGUI.DrawLine();
                        LocalizedProperty(rimDirStrength);
                        if(rimDirStrength.floatValue != 0)
                        {
                            EditorGUI.indentLevel++;
                            LocalizedProperty(rimDirRange);
                            lilEditorGUI.InvBorderGUI(rimBorder);
                            LocalizedProperty(rimBlur);
                            lilEditorGUI.DrawLine();
                            LocalizedProperty(rimIndirRange);
                            LocalizedProperty(rimIndirColor);
                            lilEditorGUI.InvBorderGUI(rimIndirBorder);
                            LocalizedProperty(rimIndirBlur);
                            EditorGUI.indentLevel--;
                            lilEditorGUI.DrawLine();
                        }
                        else
                        {
                            lilEditorGUI.InvBorderGUI(rimBorder);
                            LocalizedProperty(rimBlur);
                        }
                        LocalizedProperty(rimFresnelPower);
                        EditorGUILayout.EndVertical();
                    }
                }
                else
                {
                    LocalizedProperty(useRim, false);
                    DrawMenuButton(GetLoc("sAnchorRimLight"), PropertyBlock.RimLight);
                    if(useRim.floatValue == 1)
                    {
                        EditorGUILayout.BeginVertical(boxInnerHalf);
                        LocalizedProperty(rimColor);
                        LocalizedProperty(rimShadowMask);
                        lilEditorGUI.DrawLine();
                        lilEditorGUI.InvBorderGUI(rimBorder);
                        LocalizedProperty(rimBlur);
                        LocalizedProperty(rimFresnelPower);
                        EditorGUILayout.EndVertical();
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawGlitterSettings()
        {
            if(!ShouldDrawBlock(PropertyBlock.Glitter)) return;
            edSet.isShowGlitter = lilEditorGUI.Foldout(GetLoc("sGlitterSetting"), edSet.isShowGlitter);
            DrawMenuButton(GetLoc("sAnchorGlitter"), PropertyBlock.Glitter);
            if(edSet.isShowGlitter)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                LocalizedProperty(useGlitter);
                if(useGlitter.floatValue == 1)
                {
                    EditorGUILayout.BeginVertical(boxInnerHalf);
                    LocalizedProperty(glitterUVMode);
                    TextureGUI(ref edSet.isShowGlitterColorTex, colorMaskRGBAContent, glitterColorTex, glitterColor, glitterColorTex_UVMode, "UV Mode|UV0|UV1|UV2|UV3");
                    EditorGUI.indentLevel++;
                    LocalizedPropertyAlpha(glitterColor);
                    LocalizedProperty(glitterMainStrength);
                    LocalizedProperty(glitterEnableLighting);
                    LocalizedProperty(glitterShadowMask);
                    LocalizedProperty(glitterBackfaceMask);
                    if(isTransparent) LocalizedProperty(glitterApplyTransparency);
                    EditorGUI.indentLevel--;
                    lilEditorGUI.DrawLine();
                    LocalizedProperty(glitterApplyShape);
                    if(glitterApplyShape.floatValue > 0.5f)
                    {
                        EditorGUI.indentLevel++;
                        TextureGUI(ref edSet.isShowGlitterShapeTex, customMaskContent, glitterShapeTex);
                        LocalizedProperty(glitterAtras);
                        LocalizedProperty(glitterAngleRandomize);
                        EditorGUI.indentLevel--;
                    }
                    lilEditorGUI.DrawLine();

                    // Param1
                    var scale = new Vector2(256.0f/glitterParams1.vectorValue.x, 256.0f/glitterParams1.vectorValue.y);
                    float size = glitterParams1.vectorValue.z == 0.0f ? 0.0f : Mathf.Sqrt(glitterParams1.vectorValue.z);
                    float density = Mathf.Sqrt(1.0f / glitterParams1.vectorValue.w) / 1.5f;
                    float sensitivity = lilEditorGUI.RoundFloat1000000(glitterSensitivity.floatValue / density);
                    density = lilEditorGUI.RoundFloat1000000(density);
                    EditorGUIUtility.wideMode = true;

                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = glitterParams1.hasMixedValue || glitterSensitivity.hasMixedValue;
                    scale = lilEditorGUI.Vector2Field(Event.current.alt ? glitterParams1.name + ".xy" : GetLoc("sScale"), scale);
                    size = lilEditorGUI.Slider(Event.current.alt ? glitterParams1.name + ".z" : GetLoc("sParticleSize"), size, 0.0f, 2.0f);
                    EditorGUI.showMixedValue = false;

                    LocalizedProperty(glitterScaleRandomize);

                    EditorGUI.showMixedValue = glitterParams1.hasMixedValue || glitterSensitivity.hasMixedValue;
                    density = lilEditorGUI.Slider(Event.current.alt ? glitterParams1.name + ".w" : GetLoc("sDensity"), density, 0.001f, 1.0f);
                    sensitivity = lilEditorGUI.FloatField(Event.current.alt ? glitterSensitivity.name : GetLoc("sSensitivity"), sensitivity);
                    EditorGUI.showMixedValue = false;

                    if(EditorGUI.EndChangeCheck())
                    {
                        scale.x = Mathf.Max(scale.x, 0.0000001f);
                        scale.y = Mathf.Max(scale.y, 0.0000001f);
                        glitterParams1.vectorValue = new Vector4(256.0f/scale.x, 256.0f/scale.y, size * size, 1.0f / (density * density * 1.5f * 1.5f));
                        glitterSensitivity.floatValue = Mathf.Max(sensitivity * density, 0.25f);
                    }

                    // Other
                    LocalizedProperty(glitterParams2);
                    LocalizedProperty(glitterVRParallaxStrength);
                    LocalizedProperty(glitterNormalStrength);
                    LocalizedProperty(glitterPostContrast);
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }
        }
    }
}
#endif
