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
        // Property drawer
        #region
        private void LocalizedProperty(MaterialProperty prop, bool shouldCheck = true)
        {
            lilEditorGUI.LocalizedProperty(m_MaterialEditor, prop, shouldCheck);
        }

        private void LocalizedProperty(lilMaterialProperty prop, bool shouldCheck = true)
        {
            if (prop.p != null) LocalizedProperty(prop.p, shouldCheck);
        }

        private void LocalizedProperty(MaterialProperty prop, string label, bool shouldCheck = true)
        {
            lilEditorGUI.LocalizedProperty(m_MaterialEditor, prop, label, shouldCheck);
        }

        private void LocalizedProperty(MaterialProperty prop, int indent, bool shouldCheck = true)
        {
            lilEditorGUI.LocalizedProperty(m_MaterialEditor, prop, indent, shouldCheck);
        }

        private void LocalizedPropertyColorWithAlpha(MaterialProperty prop, bool shouldCheck = true)
        {
            lilEditorGUI.LocalizedPropertyColorWithAlpha(m_MaterialEditor, prop, shouldCheck);
        }

        public static void LocalizedPropertyTexture(GUIContent content, MaterialProperty tex, bool shouldCheck = true)
        {
            lilEditorGUI.LocalizedPropertyTexture(m_MaterialEditor, content, tex, shouldCheck);
        }

        public static void LocalizedPropertyTexture(GUIContent content, MaterialProperty tex, MaterialProperty color, bool shouldCheck = true)
        {
            lilEditorGUI.LocalizedPropertyTexture(m_MaterialEditor, content, tex, color, shouldCheck);
        }

        private void LocalizedPropertyTextureWithAlpha(GUIContent content, MaterialProperty tex, MaterialProperty color, bool shouldCheck = true)
        {
            lilEditorGUI.LocalizedPropertyTextureWithAlpha(m_MaterialEditor, content, tex, color, shouldCheck);
        }

        private void LocalizedPropertyAlpha(MaterialProperty prop, bool shouldCheck = true)
        {
            lilEditorGUI.LocalizedPropertyAlpha(prop, shouldCheck);
        }

        private void UV4Decal(MaterialProperty isDecal, MaterialProperty isLeftOnly, MaterialProperty isRightOnly, MaterialProperty shouldCopy, MaterialProperty shouldFlipMirror, MaterialProperty shouldFlipCopy, MaterialProperty tex, MaterialProperty SR, MaterialProperty angle, MaterialProperty decalAnimation, MaterialProperty decalSubParam, MaterialProperty uvMode)
        {
            lilEditorGUI.UV4Decal(m_MaterialEditor, isDecal, isLeftOnly, isRightOnly, shouldCopy, shouldFlipMirror, shouldFlipCopy, tex, SR, angle, decalAnimation, decalSubParam, uvMode);
        }

        private void ToneCorrectionGUI(MaterialProperty hsvg)
        {
            lilEditorGUI.ToneCorrectionGUI(m_MaterialEditor, hsvg);
        }

        private void ToneCorrectionGUI(MaterialProperty hsvg, int indent)
        {
            lilEditorGUI.ToneCorrectionGUI(m_MaterialEditor, hsvg, indent);
        }

        private void UVSettingGUI(MaterialProperty uvst)
        {
            lilEditorGUI.UVSettingGUI(m_MaterialEditor, uvst);
        }

        private void UVSettingGUI(MaterialProperty uvst, MaterialProperty uvsr)
        {
            lilEditorGUI.UVSettingGUI(m_MaterialEditor, uvst, uvsr);
        }

        private void BlendSettingGUI(ref bool isShow, string labelName, MaterialProperty srcRGB, MaterialProperty dstRGB, MaterialProperty srcA, MaterialProperty dstA, MaterialProperty opRGB, MaterialProperty opA)
        {
            lilEditorGUI.BlendSettingGUI(m_MaterialEditor, isCustomEditor, ref isShow, labelName, srcRGB, dstRGB, srcA, dstA, opRGB, opA);
        }

        private void TextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName)
        {
            lilEditorGUI.TextureGUI(m_MaterialEditor, isCustomEditor, ref isShow, guiContent, textureName);
        }

        private void TextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba)
        {
            lilEditorGUI.TextureGUI(m_MaterialEditor, isCustomEditor, ref isShow, guiContent, textureName, rgba);
        }

        private void TextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty uvMode, string sUVMode)
        {
            lilEditorGUI.TextureGUI(m_MaterialEditor, isCustomEditor, ref isShow, guiContent, textureName, rgba, uvMode, sUVMode);
        }

        private void TextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty scrollRotate)
        {
            lilEditorGUI.TextureGUI(m_MaterialEditor, isCustomEditor, ref isShow, guiContent, textureName, rgba, scrollRotate);
        }

        private void TextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty scrollRotate, bool useCustomUV, bool useUVAnimation)
        {
            lilEditorGUI.TextureGUI(m_MaterialEditor, isCustomEditor, ref isShow, guiContent, textureName, rgba, scrollRotate, useCustomUV, useUVAnimation);
        }

        private void TextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty scrollRotate, MaterialProperty uvMode, bool useCustomUV, bool useUVAnimation)
        {
            lilEditorGUI.TextureGUI(m_MaterialEditor, isCustomEditor, ref isShow, guiContent, textureName, rgba, scrollRotate, uvMode, useCustomUV, useUVAnimation);
        }

        private void MatCapTextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty blendUV1, MaterialProperty zRotCancel, MaterialProperty perspective, MaterialProperty vrParallaxStrength)
        {
            lilEditorGUI.MatCapTextureGUI(m_MaterialEditor, isCustomEditor, ref isShow, guiContent, textureName, blendUV1, zRotCancel, perspective, vrParallaxStrength);
        }

        private void MatCapTextureGUI(ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty blendUV1, MaterialProperty zRotCancel, MaterialProperty perspective, MaterialProperty vrParallaxStrength)
        {
            lilEditorGUI.MatCapTextureGUI(m_MaterialEditor, isCustomEditor, ref isShow, guiContent, textureName, rgba, blendUV1, zRotCancel, perspective, vrParallaxStrength);
        }

        private void RenderQueueField()
        {
            lilEditorGUI.RenderQueueField(m_MaterialEditor);
        }

        private void EnableInstancingField()
        {
            lilEditorGUI.EnableInstancingField(m_MaterialEditor);
        }

        private void DoubleSidedGIField()
        {
            lilEditorGUI.DoubleSidedGIField(m_MaterialEditor);
        }

        private void LightmapEmissionFlagsProperty()
        {
            lilEditorGUI.LightmapEmissionFlagsProperty(m_MaterialEditor);
        }

        private void TextureBakeGUI(Material material, int bakeType)
        {
            // bakeType
            // 0 : All
            // 1 : 1st
            // 2 : 2nd
            // 3 : 3rd
            // 4 : 1st Simple Button
            // 5 : 2nd Simple Button
            // 6 : 3rd Simple Button
            string[] sBake = {GetLoc("sBakeAll"), GetLoc("sBake1st"), GetLoc("sBake2nd"), GetLoc("sBake3rd"), GetLoc("sBake"), GetLoc("sBake"), GetLoc("sBake")};
            if(lilEditorGUI.Button(sBake[bakeType]))
            {
                Undo.RecordObject(material, "Bake");
                TextureBake(material, bakeType);
            }
        }

        private void AlphamaskToTextureGUI(Material material)
        {
            if(mainTex.textureValue != null && lilEditorGUI.Button(GetLoc("sBakeAlphamask")))
            {
                var bakedTexture = AutoBakeAlphaMask(material);
                if(bakedTexture == mainTex.textureValue) return;

                mainTex.textureValue = bakedTexture;
                alphaMaskMode.floatValue = 0.0f;
                alphaMask.textureValue = null;
                alphaMaskValue.floatValue = 0.0f;
            }
        }
        #endregion
    }
}
#endif
