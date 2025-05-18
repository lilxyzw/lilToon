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
        // Material Setup
        #region
        public static void SetupMaterialWithRenderingMode(Material material, RenderingMode renderingMode, TransparentMode transparentMode, bool isoutl, bool islite, bool istess, bool ismulti)
        {
            if(!isMultiVariants) lilMaterialUtils.SetupMaterialWithRenderingMode(material, renderingMode, transparentMode, isoutl, islite, istess, ismulti);
        }

        public static void SetupMaterialWithRenderingMode(Material material, RenderingMode renderingMode, TransparentMode transparentMode, bool isoutl, bool islite, bool istess)
        {
            SetupMaterialWithRenderingMode(material, renderingMode, transparentMode, isoutl, islite, istess, isMulti);
        }

        public static void SetupMaterialWithRenderingMode(Material material, RenderingMode renderingMode, TransparentMode transparentMode)
        {
            SetupMaterialWithRenderingMode(material, renderingMode, transparentMode, isOutl, isLite, isTess);
        }

        private void SetupMaterialWithRenderingMode(RenderingMode renderingMode, TransparentMode transparentMode, bool isoutl, bool islite, bool istess, bool ismulti)
        {
            foreach(var material in materials) SetupMaterialWithRenderingMode(material, renderingMode, transparentMode, isoutl, islite, istess, ismulti);
        }

        private void SetupMaterialWithRenderingMode(RenderingMode renderingMode, TransparentMode transparentMode, bool isoutl, bool islite, bool istess)
        {
            foreach(var material in materials) SetupMaterialWithRenderingMode(material, renderingMode, transparentMode, isoutl, islite, istess);
        }

        private void SetupMaterialWithRenderingMode(RenderingMode renderingMode, TransparentMode transparentMode)
        {
            foreach(var material in materials) SetupMaterialWithRenderingMode(material, renderingMode, transparentMode);
        }
        #endregion

        //------------------------------------------------------------------------------------------------------------------------------
        // Material Converter
        #region
        private void CreateMToonMaterial(Material material)
        {
            var mtoonMaterial = new Material(mtoon);

            string matPath = AssetDatabase.GetAssetPath(material);
            if(!string.IsNullOrEmpty(matPath))  matPath = EditorUtility.SaveFilePanel("Save Material", Path.GetDirectoryName(matPath), Path.GetFileNameWithoutExtension(matPath)+"_mtoon", "mat");
            else                                matPath = EditorUtility.SaveFilePanel("Save Material", "Assets", material.name + ".mat", "mat");
            if(!string.IsNullOrEmpty(matPath))  AssetDatabase.CreateAsset(mtoonMaterial, FileUtil.GetProjectRelativePath(matPath));

            mtoonMaterial.SetColor("_Color",                    new Color(Mathf.Clamp01(mainColor.colorValue.r), Mathf.Clamp01(mainColor.colorValue.g), Mathf.Clamp01(mainColor.colorValue.b), Mathf.Clamp01(mainColor.colorValue.a)));
            mtoonMaterial.SetFloat("_LightColorAttenuation",    0.0f);
            mtoonMaterial.SetFloat("_IndirectLightIntensity",   0.0f);

            mtoonMaterial.SetFloat("_UvAnimScrollX",            mainTex_ScrollRotate.vectorValue.x);
            mtoonMaterial.SetFloat("_UvAnimScrollY",            mainTex_ScrollRotate.vectorValue.y);
            mtoonMaterial.SetFloat("_UvAnimRotation",           mainTex_ScrollRotate.vectorValue.w / Mathf.PI * 0.5f);
            mtoonMaterial.SetFloat("_MToonVersion",             35.0f);
            mtoonMaterial.SetFloat("_DebugMode",                0.0f);
            mtoonMaterial.SetFloat("_CullMode",                 cull.floatValue);

            var bakedMainTex = AutoBakeMainTexture(material);
            mtoonMaterial.SetTexture("_MainTex", bakedMainTex);

            var mainScale = material.GetTextureScale(mainTex.name);
            var mainOffset = material.GetTextureOffset(mainTex.name);
            mtoonMaterial.SetTextureScale(mainTex.name, mainScale);
            mtoonMaterial.SetTextureOffset(mainTex.name, mainOffset);

            if(useBumpMap.floatValue == 1.0f && bumpMap.textureValue != null)
            {
                mtoonMaterial.SetFloat("_BumpScale", bumpScale.floatValue);
                mtoonMaterial.SetTexture("_BumpMap", bumpMap.textureValue);
                mtoonMaterial.EnableKeyword("_NORMALMAP");
            }

            if(useShadow.floatValue == 1.0f)
            {
                float shadeShift = (Mathf.Clamp01(shadowBorder.floatValue - (shadowBlur.floatValue * 0.5f)) * 2.0f) - 1.0f;
                float shadeToony = shadeShift == 1.0f ? 1.0f : (2.0f - (Mathf.Clamp01(shadowBorder.floatValue + (shadowBlur.floatValue * 0.5f)) * 2.0f)) / (1.0f - shadeShift);
                if(shadowStrengthMask.textureValue != null || shadowMainStrength.floatValue != 0.0f)
                {
                    var bakedShadowTex = AutoBakeShadowTexture(material, bakedMainTex);
                    mtoonMaterial.SetColor("_ShadeColor",               Color.white);
                    mtoonMaterial.SetTexture("_ShadeTexture",           bakedShadowTex);
                }
                else
                {
                    var shadeColorStrength = new Color(
                        1.0f - ((1.0f - shadowColor.colorValue.r) * shadowStrength.floatValue),
                        1.0f - ((1.0f - shadowColor.colorValue.g) * shadowStrength.floatValue),
                        1.0f - ((1.0f - shadowColor.colorValue.b) * shadowStrength.floatValue),
                        1.0f
                    );
                    mtoonMaterial.SetColor("_ShadeColor",               shadeColorStrength);
                    if(shadowColorTex.textureValue != null)
                    {
                        mtoonMaterial.SetTexture("_ShadeTexture",           shadowColorTex.textureValue);
                    }
                    else
                    {
                        mtoonMaterial.SetTexture("_ShadeTexture",           bakedMainTex);
                    }
                }
                mtoonMaterial.SetFloat("_ReceiveShadowRate",        1.0f);
                mtoonMaterial.SetTexture("_ReceiveShadowTexture",   null);
                mtoonMaterial.SetFloat("_ShadingGradeRate",         1.0f);
                mtoonMaterial.SetTexture("_ShadingGradeTexture",    shadowBorderMask.textureValue);
                mtoonMaterial.SetFloat("_ShadeShift",               shadeShift);
                mtoonMaterial.SetFloat("_ShadeToony",               shadeToony);
            }
            else
            {
                mtoonMaterial.SetColor("_ShadeColor",               Color.white);
                mtoonMaterial.SetTexture("_ShadeTexture",           bakedMainTex);
            }

            if(useEmission.floatValue == 1.0f && emissionMap.textureValue != null)
            {
                mtoonMaterial.SetColor("_EmissionColor",            emissionColor.colorValue);
                mtoonMaterial.SetTexture("_EmissionMap",            emissionMap.textureValue);
            }

            if(useRim.floatValue == 1.0f)
            {
                mtoonMaterial.SetColor("_RimColor",                 rimColor.colorValue);
                mtoonMaterial.SetTexture("_RimTexture",             rimColorTex.textureValue);
                mtoonMaterial.SetFloat("_RimLightingMix",           rimEnableLighting.floatValue);

                float rimFP = rimFresnelPower.floatValue / Mathf.Max(0.001f, rimBlur.floatValue);
                float rimLift = Mathf.Pow(1.0f - rimBorder.floatValue, rimFresnelPower.floatValue) * (1.0f - rimBlur.floatValue);
                mtoonMaterial.SetFloat("_RimFresnelPower",          rimFP);
                mtoonMaterial.SetFloat("_RimLift",                  rimLift);
            }
            else
            {
                mtoonMaterial.SetColor("_RimColor",                 Color.black);
            }

            if(useMatCap.floatValue == 1.0f && matcapBlendMode.floatValue != 3.0f && matcapTex.textureValue != null)
            {
                var bakedMatCap = AutoBakeMatCap(material);
                mtoonMaterial.SetTexture("_SphereAdd", bakedMatCap);
            }

            if(isOutl)
            {
                mtoonMaterial.SetTexture("_OutlineWidthTexture",    outlineWidthMask.textureValue);
                mtoonMaterial.SetFloat("_OutlineWidth",             outlineWidth.floatValue);
                mtoonMaterial.SetColor("_OutlineColor",             outlineColor.colorValue);
                mtoonMaterial.SetFloat("_OutlineLightingMix",       1.0f);
                mtoonMaterial.SetFloat("_OutlineWidthMode",         1.0f);
                mtoonMaterial.SetFloat("_OutlineColorMode",         1.0f);
                mtoonMaterial.SetFloat("_OutlineCullMode",          1.0f);
                mtoonMaterial.EnableKeyword("MTOON_OUTLINE_WIDTH_WORLD");
                mtoonMaterial.EnableKeyword("MTOON_OUTLINE_COLOR_MIXED");
            }

            if(isCutout)
            {
                mtoonMaterial.SetFloat("_Cutoff", cutoff.floatValue);
                mtoonMaterial.SetFloat("_BlendMode", 1.0f);
                mtoonMaterial.SetOverrideTag("RenderType", "TransparentCutout");
                mtoonMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                mtoonMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                mtoonMaterial.SetFloat("_ZWrite", 1.0f);
                mtoonMaterial.SetFloat("_AlphaToMask", 1.0f);
                mtoonMaterial.EnableKeyword("_ALPHATEST_ON");
                mtoonMaterial.renderQueue = (int)RenderQueue.AlphaTest;
            }
            else if(isTransparent && zwrite.floatValue == 0.0f)
            {
                mtoonMaterial.SetFloat("_BlendMode", 2.0f);
                mtoonMaterial.SetOverrideTag("RenderType", "TransparentCutout");
                mtoonMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mtoonMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mtoonMaterial.SetFloat("_ZWrite", 0.0f);
                mtoonMaterial.SetFloat("_AlphaToMask", 0.0f);
                mtoonMaterial.EnableKeyword("_ALPHABLEND_ON");
                mtoonMaterial.renderQueue = (int)RenderQueue.Transparent;
            }
            else if(isTransparent && zwrite.floatValue != 0.0f)
            {
                mtoonMaterial.SetFloat("_BlendMode", 3.0f);
                mtoonMaterial.SetOverrideTag("RenderType", "TransparentCutout");
                mtoonMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mtoonMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mtoonMaterial.SetFloat("_ZWrite", 1.0f);
                mtoonMaterial.SetFloat("_AlphaToMask", 0.0f);
                mtoonMaterial.EnableKeyword("_ALPHABLEND_ON");
                mtoonMaterial.renderQueue = (int)RenderQueue.Transparent;
            }
            else
            {
                mtoonMaterial.SetFloat("_BlendMode", 0.0f);
                mtoonMaterial.SetOverrideTag("RenderType", "Opaque");
                mtoonMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                mtoonMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                mtoonMaterial.SetFloat("_ZWrite", 1.0f);
                mtoonMaterial.SetFloat("_AlphaToMask", 0.0f);
                mtoonMaterial.renderQueue = -1;
            }
        }

        private void CreateLiteMaterial(Material material)
        {
            var liteMaterial = new Material(ltsl);
            var renderingMode = renderingModeBuf;
            if(renderingMode == RenderingMode.Refraction)       renderingMode = RenderingMode.Transparent;
            if(renderingMode == RenderingMode.RefractionBlur)   renderingMode = RenderingMode.Transparent;
            if(renderingMode == RenderingMode.Fur)              renderingMode = RenderingMode.Transparent;
            if(renderingMode == RenderingMode.FurCutout)        renderingMode = RenderingMode.Cutout;
            if(renderingMode == RenderingMode.FurTwoPass)       renderingMode = RenderingMode.Transparent;

            bool isonepass      = material.shader.name.Contains("OnePass");
            bool istwopass      = material.shader.name.Contains("TwoPass");

            var           transparentMode = TransparentMode.Normal;
            if(isonepass) transparentMode = TransparentMode.OnePass;
            if(istwopass) transparentMode = TransparentMode.TwoPass;

            SetupMaterialWithRenderingMode(liteMaterial, renderingMode, transparentMode, isOutl, true, isStWr, false);

            string matPath = AssetDatabase.GetAssetPath(material);
            if(!string.IsNullOrEmpty(matPath))  matPath = EditorUtility.SaveFilePanel("Save Material", Path.GetDirectoryName(matPath), Path.GetFileNameWithoutExtension(matPath)+"_lite", "mat");
            else                                matPath = EditorUtility.SaveFilePanel("Save Material", "Assets", material.name + ".mat", "mat");
            if(string.IsNullOrEmpty(matPath))   return;
            else                                AssetDatabase.CreateAsset(liteMaterial, FileUtil.GetProjectRelativePath(matPath));

            liteMaterial.SetFloat("_Invisible",                 invisible.floatValue);
            liteMaterial.SetFloat("_Cutoff",                    cutoff.floatValue);
            liteMaterial.SetFloat("_SubpassCutoff",             subpassCutoff.floatValue);
            liteMaterial.SetFloat("_Cull",                      cull.floatValue);
            liteMaterial.SetFloat("_FlipNormal",                flipNormal.floatValue);
            liteMaterial.SetFloat("_BackfaceForceShadow",       backfaceForceShadow.floatValue);

            liteMaterial.SetColor("_Color",                     mainColor.colorValue);
            liteMaterial.SetVector("_MainTex_ScrollRotate",     mainTex_ScrollRotate.vectorValue);

            var bakedMainTex = AutoBakeMainTexture(material);
            liteMaterial.SetTexture("_MainTex", bakedMainTex);

            var mainScale = material.GetTextureScale(mainTex.name);
            var mainOffset = material.GetTextureOffset(mainTex.name);
            liteMaterial.SetTextureScale(mainTex.name, mainScale);
            liteMaterial.SetTextureOffset(mainTex.name, mainOffset);

            liteMaterial.SetFloat("_UseShadow",                 useShadow.floatValue);
            if(useShadow.floatValue == 1.0f)
            {
                var bakedShadowTex = AutoBakeShadowTexture(material, bakedMainTex, 1, false);
                liteMaterial.SetFloat("_ShadowBorder",              shadowBorder.floatValue);
                liteMaterial.SetFloat("_ShadowBlur",                shadowBlur.floatValue);
                liteMaterial.SetTexture("_ShadowColorTex",          bakedShadowTex);
                if(shadow2ndColor.colorValue.a != 0.0f)
                {
                    var bakedShadow2ndTex = AutoBakeShadowTexture(material, bakedMainTex, 2, false);
                    liteMaterial.SetFloat("_Shadow2ndBorder",           shadow2ndBorder.floatValue);
                    liteMaterial.SetFloat("_Shadow2ndBlur",             shadow2ndBlur.floatValue);
                    liteMaterial.SetTexture("_Shadow2ndColorTex",       bakedShadow2ndTex);
                }
                liteMaterial.SetFloat("_ShadowEnvStrength",         shadowEnvStrength.floatValue);
                liteMaterial.SetColor("_ShadowBorderColor",         shadowBorderColor.colorValue);
                liteMaterial.SetFloat("_ShadowBorderRange",         shadowBorderRange.floatValue);
            }

            if(isOutl)
            {
                var bakedOutlineTex = AutoBakeOutlineTexture(material);
                liteMaterial.SetColor("_OutlineColor",              outlineColor.colorValue);
                liteMaterial.SetTexture("_OutlineTex",              bakedOutlineTex);
                liteMaterial.SetVector("_OutlineTex_ScrollRotate",  outlineTex_ScrollRotate.vectorValue);
                liteMaterial.SetTexture("_OutlineWidthMask",        outlineWidthMask.textureValue);
                liteMaterial.SetFloat("_OutlineWidth",              outlineWidth.floatValue);
                liteMaterial.SetFloat("_OutlineFixWidth",           outlineFixWidth.floatValue);
                liteMaterial.SetFloat("_OutlineVertexR2Width",      outlineVertexR2Width.floatValue);
                liteMaterial.SetFloat("_OutlineDeleteMesh",         outlineDeleteMesh.floatValue);
                liteMaterial.SetFloat("_OutlineEnableLighting",     outlineEnableLighting.floatValue);
                liteMaterial.SetFloat("_OutlineZBias",              outlineZBias.floatValue);
                liteMaterial.SetFloat("_OutlineSrcBlend",           outlineSrcBlend.floatValue);
                liteMaterial.SetFloat("_OutlineDstBlend",           outlineDstBlend.floatValue);
                liteMaterial.SetFloat("_OutlineBlendOp",            outlineBlendOp.floatValue);
                liteMaterial.SetFloat("_OutlineSrcBlendFA",         outlineSrcBlendFA.floatValue);
                liteMaterial.SetFloat("_OutlineDstBlendFA",         outlineDstBlendFA.floatValue);
                liteMaterial.SetFloat("_OutlineBlendOpFA",          outlineBlendOpFA.floatValue);
                liteMaterial.SetFloat("_OutlineZWrite",             outlineZwrite.floatValue);
                liteMaterial.SetFloat("_OutlineZTest",              outlineZtest.floatValue);
                liteMaterial.SetFloat("_OutlineAlphaToMask",        outlineAlphaToMask.floatValue);
                liteMaterial.SetFloat("_OutlineStencilRef",         outlineStencilRef.floatValue);
                liteMaterial.SetFloat("_OutlineStencilReadMask",    outlineStencilReadMask.floatValue);
                liteMaterial.SetFloat("_OutlineStencilWriteMask",   outlineStencilWriteMask.floatValue);
                liteMaterial.SetFloat("_OutlineStencilComp",        outlineStencilComp.floatValue);
                liteMaterial.SetFloat("_OutlineStencilPass",        outlineStencilPass.floatValue);
                liteMaterial.SetFloat("_OutlineStencilFail",        outlineStencilFail.floatValue);
                liteMaterial.SetFloat("_OutlineStencilZFail",       outlineStencilZFail.floatValue);
            }

            var bakedMatCap = AutoBakeMatCap(liteMaterial);
            if(bakedMatCap != null)
            {
                liteMaterial.SetTexture("_MatCapTex",               bakedMatCap);
                liteMaterial.SetFloat("_UseMatCap",                 useMatCap.floatValue);
                liteMaterial.SetFloat("_MatCapBlendUV1",            matcapBlendUV1.floatValue);
                liteMaterial.SetFloat("_MatCapZRotCancel",          matcapZRotCancel.floatValue);
                liteMaterial.SetFloat("_MatCapPerspective",         matcapPerspective.floatValue);
                liteMaterial.SetFloat("_MatCapVRParallaxStrength",  matcapVRParallaxStrength.floatValue);
                if(matcapBlendMode.floatValue == 3) liteMaterial.SetFloat("_MatCapMul", useMatCap.floatValue);
                else                                liteMaterial.SetFloat("_MatCapMul", useMatCap.floatValue);
            }

            liteMaterial.SetFloat("_UseRim",                    useRim.floatValue);
            if(useRim.floatValue == 1.0f)
            {
                liteMaterial.SetColor("_RimColor",                  rimColor.colorValue);
                liteMaterial.SetFloat("_RimBorder",                 rimBorder.floatValue);
                liteMaterial.SetFloat("_RimBlur",                   rimBlur.floatValue);
                liteMaterial.SetFloat("_RimFresnelPower",           rimFresnelPower.floatValue);
                liteMaterial.SetFloat("_RimShadowMask",             rimShadowMask.floatValue);
            }

            if(useEmission.floatValue == 1.0f)
            {
                liteMaterial.SetFloat("_UseEmission",               useEmission.floatValue);
                liteMaterial.SetColor("_EmissionColor",             emissionColor.colorValue);
                liteMaterial.SetTexture("_EmissionMap",             emissionMap.textureValue);
                liteMaterial.SetFloat("_EmissionMap_UVMode",        emissionMap_UVMode.floatValue);
                liteMaterial.SetVector("_EmissionMap_ScrollRotate", emissionMap_ScrollRotate.vectorValue);
                liteMaterial.SetVector("_EmissionBlink",            emissionBlink.vectorValue);
            }

            var bakedTriMask = AutoBakeTriMask(liteMaterial);
            if(bakedTriMask != null) liteMaterial.SetTexture("_TriMask", bakedTriMask);

            liteMaterial.SetFloat("_SrcBlend",                  srcBlend.floatValue);
            liteMaterial.SetFloat("_DstBlend",                  dstBlend.floatValue);
            liteMaterial.SetFloat("_BlendOp",                   blendOp.floatValue);
            liteMaterial.SetFloat("_SrcBlendFA",                srcBlendFA.floatValue);
            liteMaterial.SetFloat("_DstBlendFA",                dstBlendFA.floatValue);
            liteMaterial.SetFloat("_BlendOpFA",                 blendOpFA.floatValue);
            liteMaterial.SetFloat("_ZClip",                     zclip.floatValue);
            liteMaterial.SetFloat("_ZWrite",                    zwrite.floatValue);
            liteMaterial.SetFloat("_ZTest",                     ztest.floatValue);
            liteMaterial.SetFloat("_AlphaToMask",               alphaToMask.floatValue);
            liteMaterial.SetFloat("_StencilRef",                stencilRef.floatValue);
            liteMaterial.SetFloat("_StencilReadMask",           stencilReadMask.floatValue);
            liteMaterial.SetFloat("_StencilWriteMask",          stencilWriteMask.floatValue);
            liteMaterial.SetFloat("_StencilComp",               stencilComp.floatValue);
            liteMaterial.SetFloat("_StencilPass",               stencilPass.floatValue);
            liteMaterial.SetFloat("_StencilFail",               stencilFail.floatValue);
            liteMaterial.SetFloat("_StencilZFail",              stencilZFail.floatValue);
            liteMaterial.renderQueue = lilMaterialUtils.GetTrueRenderQueue(material);
        }

        // TODO : Support other rendering mode
        private void CreateMultiMaterial(Material material)
        {
            material.shader = ltsm;
            if(isOutl)  material.shader = ltsmo;
            else        material.shader = ltsm;
            isMulti = true;

            if(renderingModeBuf == RenderingMode.Cutout)            material.SetFloat("_TransparentMode", 1.0f);
            else if(renderingModeBuf == RenderingMode.Transparent)  material.SetFloat("_TransparentMode", 2.0f);
            else                                                    material.SetFloat("_TransparentMode", 0.0f);
            material.SetFloat("_UseClippingCanceller", 0.0f);

            if(shaderSetting.LIL_FEATURE_CLIPPING_CANCELLER) material.SetFloat("_UseClippingCanceller", 1.0f);

            SetupMaterialWithRenderingMode(material, renderingModeBuf, TransparentMode.Normal, isOutl, false, isStWr, true);
            lilMaterialUtils.SetupMultiMaterial(material);
        }

        protected virtual void ReplaceToCustomShaders()
        {
        }

        protected void ConvertMaterialToCustomShader(Material material)
        {
            lilShaderManager.InitializeShaders();
            var shader = material.shader;
                 if(shader == lts)           { ReplaceToCustomShaders(); shader = lts       ;}
            else if(shader == ltsc)          { ReplaceToCustomShaders(); shader = ltsc      ;}
            else if(shader == ltst)          { ReplaceToCustomShaders(); shader = ltst      ;}
            else if(shader == ltsot)         { ReplaceToCustomShaders(); shader = ltsot     ;}
            else if(shader == ltstt)         { ReplaceToCustomShaders(); shader = ltstt     ;}
            else if(shader == ltso)          { ReplaceToCustomShaders(); shader = ltso      ;}
            else if(shader == ltsco)         { ReplaceToCustomShaders(); shader = ltsco     ;}
            else if(shader == ltsto)         { ReplaceToCustomShaders(); shader = ltsto     ;}
            else if(shader == ltsoto)        { ReplaceToCustomShaders(); shader = ltsoto    ;}
            else if(shader == ltstto)        { ReplaceToCustomShaders(); shader = ltstto    ;}
            else if(shader == ltsoo)         { ReplaceToCustomShaders(); shader = ltsoo     ;}
            else if(shader == ltscoo)        { ReplaceToCustomShaders(); shader = ltscoo    ;}
            else if(shader == ltstoo)        { ReplaceToCustomShaders(); shader = ltstoo    ;}
            else if(shader == ltstess)       { ReplaceToCustomShaders(); shader = ltstess   ;}
            else if(shader == ltstessc)      { ReplaceToCustomShaders(); shader = ltstessc  ;}
            else if(shader == ltstesst)      { ReplaceToCustomShaders(); shader = ltstesst  ;}
            else if(shader == ltstessot)     { ReplaceToCustomShaders(); shader = ltstessot ;}
            else if(shader == ltstesstt)     { ReplaceToCustomShaders(); shader = ltstesstt ;}
            else if(shader == ltstesso)      { ReplaceToCustomShaders(); shader = ltstesso  ;}
            else if(shader == ltstessco)     { ReplaceToCustomShaders(); shader = ltstessco ;}
            else if(shader == ltstessto)     { ReplaceToCustomShaders(); shader = ltstessto ;}
            else if(shader == ltstessoto)    { ReplaceToCustomShaders(); shader = ltstessoto;}
            else if(shader == ltstesstto)    { ReplaceToCustomShaders(); shader = ltstesstto;}
            else if(shader == ltsl)          { ReplaceToCustomShaders(); shader = ltsl      ;}
            else if(shader == ltslc)         { ReplaceToCustomShaders(); shader = ltslc     ;}
            else if(shader == ltslt)         { ReplaceToCustomShaders(); shader = ltslt     ;}
            else if(shader == ltslot)        { ReplaceToCustomShaders(); shader = ltslot    ;}
            else if(shader == ltsltt)        { ReplaceToCustomShaders(); shader = ltsltt    ;}
            else if(shader == ltslo)         { ReplaceToCustomShaders(); shader = ltslo     ;}
            else if(shader == ltslco)        { ReplaceToCustomShaders(); shader = ltslco    ;}
            else if(shader == ltslto)        { ReplaceToCustomShaders(); shader = ltslto    ;}
            else if(shader == ltsloto)       { ReplaceToCustomShaders(); shader = ltsloto   ;}
            else if(shader == ltsltto)       { ReplaceToCustomShaders(); shader = ltsltto   ;}
            else if(shader == ltsref)        { ReplaceToCustomShaders(); shader = ltsref    ;}
            else if(shader == ltsrefb)       { ReplaceToCustomShaders(); shader = ltsrefb   ;}
            else if(shader == ltsfur)        { ReplaceToCustomShaders(); shader = ltsfur    ;}
            else if(shader == ltsfurc)       { ReplaceToCustomShaders(); shader = ltsfurc   ;}
            else if(shader == ltsfurtwo)     { ReplaceToCustomShaders(); shader = ltsfurtwo ;}
            else if(shader == ltsfuro)       { ReplaceToCustomShaders(); shader = ltsfuro   ;}
            else if(shader == ltsfuroc)      { ReplaceToCustomShaders(); shader = ltsfuroc  ;}
            else if(shader == ltsfurotwo)    { ReplaceToCustomShaders(); shader = ltsfurotwo;}
            else if(shader == ltsgem)        { ReplaceToCustomShaders(); shader = ltsgem    ;}
            else if(shader == ltsfs)         { ReplaceToCustomShaders(); shader = ltsfs     ;}
            else if(shader == ltsover)       { ReplaceToCustomShaders(); shader = ltsover   ;}
            else if(shader == ltsoover)      { ReplaceToCustomShaders(); shader = ltsoover  ;}
            else if(shader == ltslover)      { ReplaceToCustomShaders(); shader = ltslover  ;}
            else if(shader == ltsloover)     { ReplaceToCustomShaders(); shader = ltsloover ;}
            else if(shader == ltsm)          { ReplaceToCustomShaders(); shader = ltsm      ;}
            else if(shader == ltsmo)         { ReplaceToCustomShaders(); shader = ltsmo     ;}
            else if(shader == ltsmref)       { ReplaceToCustomShaders(); shader = ltsmref   ;}
            else if(shader == ltsmfur)       { ReplaceToCustomShaders(); shader = ltsmfur   ;}
            else if(shader == ltsmgem)       { ReplaceToCustomShaders(); shader = ltsmgem   ;}

            if(material.shader != shader && shader != null)
            {
                int renderQueue = lilMaterialUtils.GetTrueRenderQueue(material);
                material.shader = shader;
                material.renderQueue = renderQueue;
            }
        }
        #endregion
    }
}
#endif
