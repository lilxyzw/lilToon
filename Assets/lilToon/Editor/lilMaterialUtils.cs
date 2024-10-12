#if !LILTOON_VRCSDK3_AVATARS && !LILTOON_VRCSDK3_WORLDS && VRC_SDK_VRCSDK3
    #if UDON
        #define LILTOON_VRCSDK3_WORLDS
    #else
        #define LILTOON_VRCSDK3_AVATARS
    #endif
#endif
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace lilToon
{
    public class lilMaterialUtils
    {
        internal static void SetupMaterialWithRenderingMode(Material material, RenderingMode renderingMode, TransparentMode transparentMode, bool isoutl, bool islite, bool istess, bool ismulti)
        {
            Undo.RecordObject(material, null);
            int renderQueue = GetTrueRenderQueue(material);
            RenderingMode rend = renderingMode;
            lilRenderPipeline RP = lilRenderPipelineReader.GetRP();
            if(ismulti)
            {
                float tpmode = material.GetFloat("_TransparentMode");
                switch((int)tpmode)
                {
                    case 1  : rend = RenderingMode.Cutout; break;
                    case 2  : rend = RenderingMode.Transparent; break;
                    case 3  : rend = RenderingMode.Refraction; break;
                    case 4  : rend = RenderingMode.Fur; break;
                    case 5  : rend = RenderingMode.FurCutout; break;
                    case 6  : rend = RenderingMode.Gem; break;
                    default : rend = RenderingMode.Opaque; break;
                }
            }
            switch(rend)
            {
                case RenderingMode.Opaque:
                    if(islite)
                    {
                        if(isoutl)  material.shader = lilShaderManager.ltslo;
                        else        material.shader = lilShaderManager.ltsl;
                    }
                    else if(ismulti)
                    {
                        if(isoutl)  material.shader = lilShaderManager.ltsmo;
                        else        material.shader = lilShaderManager.ltsm;
                        material.SetOverrideTag("RenderType", "");
                        material.renderQueue = -1;
                    }
                    else if(istess)
                    {
                        if(isoutl)  material.shader = lilShaderManager.ltstesso;
                        else        material.shader = lilShaderManager.ltstess;
                    }
                    else
                    {
                        if(isoutl)  material.shader = lilShaderManager.ltso;
                        else        material.shader = lilShaderManager.lts;
                    }
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_AlphaToMask", 0);
                    if(isoutl)
                    {
                        material.SetInt("_OutlineSrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_OutlineDstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.SetInt("_OutlineAlphaToMask", 0);
                    }
                    break;
                case RenderingMode.Cutout:
                    if(islite)
                    {
                        if(isoutl)  material.shader = lilShaderManager.ltslco;
                        else        material.shader = lilShaderManager.ltslc;
                    }
                    else if(ismulti)
                    {
                        if(isoutl)  material.shader = lilShaderManager.ltsmo;
                        else        material.shader = lilShaderManager.ltsm;
                        material.SetOverrideTag("RenderType", "TransparentCutout");
                        material.renderQueue = 2450;
                    }
                    else if(istess)
                    {
                        if(isoutl)  material.shader = lilShaderManager.ltstessco;
                        else        material.shader = lilShaderManager.ltstessc;
                    }
                    else
                    {
                        if(isoutl)  material.shader = lilShaderManager.ltsco;
                        else        material.shader = lilShaderManager.ltsc;
                    }
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_AlphaToMask", 1);
                    if(isoutl)
                    {
                        material.SetInt("_OutlineSrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_OutlineDstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.SetInt("_OutlineAlphaToMask", 1);
                    }
                    break;
                case RenderingMode.Transparent:
                    if(ismulti)
                    {
                        if(isoutl)  material.shader = lilShaderManager.ltsmo;
                        else        material.shader = lilShaderManager.ltsm;
                        material.SetOverrideTag("RenderType", "TransparentCutout");
                        material.renderQueue = RP == lilRenderPipeline.HDRP ? 3000 : 2460;
                    }
                    else
                    {
                        switch (transparentMode)
                        {
                            case TransparentMode.Normal:
                                if(islite)
                                {
                                    if(isoutl)  material.shader = lilShaderManager.ltslto;
                                    else        material.shader = lilShaderManager.ltslt;
                                }
                                else if(istess)
                                {
                                    if(isoutl)  material.shader = lilShaderManager.ltstessto;
                                    else        material.shader = lilShaderManager.ltstesst;
                                }
                                else
                                {
                                    if(isoutl)  material.shader = lilShaderManager.ltsto;
                                    else        material.shader = lilShaderManager.ltst;
                                }
                                break;
                            case TransparentMode.OnePass:
                                if(islite)
                                {
                                    if(isoutl)  material.shader = lilShaderManager.ltsloto;
                                    else        material.shader = lilShaderManager.ltslot;
                                }
                                else if(istess)
                                {
                                    if(isoutl)  material.shader = lilShaderManager.ltstessoto;
                                    else        material.shader = lilShaderManager.ltstessot;
                                }
                                else
                                {
                                    if(isoutl)  material.shader = lilShaderManager.ltsoto;
                                    else        material.shader = lilShaderManager.ltsot;
                                }
                                break;
                            case TransparentMode.TwoPass:
                                if(islite)
                                {
                                    if(isoutl)  material.shader = lilShaderManager.ltsltto;
                                    else        material.shader = lilShaderManager.ltsltt;
                                }
                                else if(istess)
                                {
                                    if(isoutl)  material.shader = lilShaderManager.ltstesstto;
                                    else        material.shader = lilShaderManager.ltstesstt;
                                }
                                else
                                {
                                    if(isoutl)  material.shader = lilShaderManager.ltstto;
                                    else        material.shader = lilShaderManager.ltstt;
                                }
                                break;
                        }
                    }
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_AlphaToMask", 0);
                    if(isoutl)
                    {
                        material.SetInt("_OutlineSrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt("_OutlineDstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt("_OutlineAlphaToMask", 0);
                    }
                    break;
                case RenderingMode.Refraction:
                    if(ismulti)
                    {
                        material.shader = lilShaderManager.ltsmref;
                        material.SetOverrideTag("RenderType", "");
                        material.renderQueue = -1;
                    }
                    else
                    {
                        material.shader = lilShaderManager.ltsref;
                    }
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_AlphaToMask", 0);
                    break;
                case RenderingMode.RefractionBlur:
                    material.shader = lilShaderManager.ltsrefb;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_AlphaToMask", 0);
                    break;
                case RenderingMode.Fur:
                    if(ismulti)
                    {
                        material.shader = lilShaderManager.ltsmfur;
                        material.SetOverrideTag("RenderType", "TransparentCutout");
                        material.renderQueue = 3000;
                    }
                    else
                    {
                        material.shader = lilShaderManager.ltsfur;
                    }
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_AlphaToMask", 0);
                    material.SetInt("_FurSrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_FurDstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_FurZWrite", 0);
                    material.SetInt("_FurAlphaToMask", 0);
                    break;
                case RenderingMode.FurCutout:
                    if(ismulti)
                    {
                        material.shader = lilShaderManager.ltsmfur;
                        material.SetOverrideTag("RenderType", "TransparentCutout");
                        material.renderQueue = 2450;
                    }
                    else
                    {
                        material.shader = lilShaderManager.ltsfurc;
                    }
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_AlphaToMask", 1);
                    material.SetInt("_FurSrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_FurDstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_FurZWrite", 1);
                    material.SetInt("_FurAlphaToMask", 1);
                    break;
                case RenderingMode.FurTwoPass:
                    material.shader = lilShaderManager.ltsfurtwo;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_AlphaToMask", 0);
                    material.SetInt("_FurSrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_FurDstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_FurZWrite", 0);
                    material.SetInt("_FurAlphaToMask", 0);
                    break;
                case RenderingMode.Gem:
                    if(ismulti)
                    {
                        material.shader = lilShaderManager.ltsmgem;
                        material.SetOverrideTag("RenderType", "");
                        material.renderQueue = -1;
                    }
                    else
                    {
                        material.shader = lilShaderManager.ltsgem;
                    }
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_AlphaToMask", 0);
                    break;
            }
            if(!ismulti) material.renderQueue = renderQueue;
            FixTransparentRenderQueue(material, renderingMode);
            if(rend == RenderingMode.Gem)
            {
                material.SetInt("_Cull", 0);
                material.SetInt("_ZWrite", 0);
            }
            else
            {
                material.SetInt("_ZWrite", 1);
            }
            if(transparentMode != TransparentMode.TwoPass)
            {
                material.SetInt("_ZTest", 4);
            }
            material.SetFloat("_OffsetFactor", 0.0f);
            material.SetFloat("_OffsetUnits", 0.0f);
            material.SetInt("_ColorMask", 15);
            material.SetInt("_SrcBlendAlpha", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlendAlpha", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_BlendOp", (int)BlendOp.Add);
            material.SetInt("_BlendOpAlpha", (int)BlendOp.Add);
            material.SetInt("_SrcBlendFA", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlendFA", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_SrcBlendAlphaFA", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_DstBlendAlphaFA", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_BlendOpFA", (int)BlendOp.Max);
            material.SetInt("_BlendOpAlphaFA", (int)BlendOp.Max);
            if(isoutl)
            {
                material.SetInt("_OutlineCull", 1);
                material.SetInt("_OutlineZWrite", 1);
                material.SetInt("_OutlineZTest", 2);
                material.SetFloat("_OutlineOffsetFactor", 0.0f);
                material.SetFloat("_OutlineOffsetUnits", 0.0f);
                material.SetInt("_OutlineColorMask", 15);
                material.SetInt("_OutlineSrcBlendAlpha", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_OutlineDstBlendAlpha", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_OutlineBlendOp", (int)BlendOp.Add);
                material.SetInt("_OutlineBlendOpAlpha", (int)BlendOp.Add);
                material.SetInt("_OutlineSrcBlendFA", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_OutlineDstBlendFA", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_OutlineSrcBlendAlphaFA", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_OutlineDstBlendAlphaFA", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_OutlineBlendOpFA", (int)BlendOp.Max);
                material.SetInt("_OutlineBlendOpAlphaFA", (int)BlendOp.Max);
            }
            if(renderingMode == RenderingMode.Fur || renderingMode == RenderingMode.FurCutout || renderingMode == RenderingMode.FurTwoPass)
            {
                material.SetInt("_FurZTest", 4);
                material.SetFloat("_FurOffsetFactor", 0.0f);
                material.SetFloat("_FurOffsetUnits", 0.0f);
                material.SetInt("_FurColorMask", 15);
                material.SetInt("_FurSrcBlendAlpha", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_FurDstBlendAlpha", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_FurBlendOp", (int)BlendOp.Add);
                material.SetInt("_FurBlendOpAlpha", (int)BlendOp.Add);
                material.SetInt("_FurSrcBlendFA", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_FurDstBlendFA", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_FurSrcBlendAlphaFA", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_FurDstBlendAlphaFA", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_FurBlendOpFA", (int)BlendOp.Max);
                material.SetInt("_FurBlendOpAlphaFA", (int)BlendOp.Max);
            }
        }

        public static bool CheckMainTextureName(string name)
        {
            return lilConstants.mainTexCheckWords.Any(word => !name.Contains(word));
        }

        private static void FixTransparentRenderQueue(Material material, RenderingMode renderingMode)
        {
            #if LILTOON_VRCSDK3_WORLDS
                if( renderingMode == RenderingMode.Transparent ||
                    renderingMode == RenderingMode.Refraction ||
                    renderingMode == RenderingMode.RefractionBlur ||
                    renderingMode == RenderingMode.Fur ||
                    renderingMode == RenderingMode.FurTwoPass ||
                    renderingMode == RenderingMode.Gem
                )
                {
                    material.renderQueue = 3000;
                }
            #endif
        }

        public static void SetupMultiMaterial(Material material)
        {
            int tpmode = 0;
            if(material.HasProperty("_TransparentMode")) tpmode = (int)material.GetFloat("_TransparentMode");
            bool useShadow = IsFeatureOnFloat(material, "_UseShadow");
            bool useRimShade = IsFeatureOnFloat(material, "_UseRimShade");
            bool useDistanceFade = IsFeatureOnVectorZ(material, "_DistanceFade");
            bool useEmission = IsFeatureOnFloat(material, "_UseEmission");
            bool useEmission2nd = IsFeatureOnFloat(material, "_UseEmission2nd");
            bool useBumpMap = IsFeatureOnFloat(material, "_UseBumpMap");
            bool useBump2ndMap = IsFeatureOnFloat(material, "_UseBump2ndMap");
            bool useAnisotropy = IsFeatureOnFloat(material, "_UseAnisotropy");
            bool useMatCap = IsFeatureOnFloat(material, "_UseMatCap");
            bool useMatCap2nd = IsFeatureOnFloat(material, "_UseMatCap2nd");
            bool useMatCapCustomNormal = IsFeatureOnFloat(material, "_MatCapCustomNormal");
            bool useMatCap2ndCustomNormal = IsFeatureOnFloat(material, "_MatCap2ndCustomNormal");
            bool useRim = IsFeatureOnFloat(material, "_UseRim");
            bool useRimDir = IsFeatureOnFloat(material, "_RimDirStrength");
            bool useGlitter = IsFeatureOnFloat(material, "_UseGlitter");
            bool useAudioLink = IsFeatureOnFloat(material, "_UseAudioLink");
            bool audioLinkAsLocal = IsFeatureOnFloat(material, "_AudioLinkAsLocal");
            bool useDissolve = IsFeatureOnVectorX(material, "_DissolveParams");
            bool useMain2ndDissolve = IsFeatureOnVectorX(material, "_Main2ndDissolveParams");
            bool useMain3rdDissolve = IsFeatureOnVectorX(material, "_Main3rdDissolveParams");
            bool useMainTexHSVG = IsFeatureOnHSVG(material, "_MainTexHSVG");
            bool useMainGradation = IsFeatureOnFloat(material, "_MainGradationStrength");
            bool useMain2ndTex = IsFeatureOnFloat(material, "_UseMain2ndTex");
            bool useMain3rdTex = IsFeatureOnFloat(material, "_UseMain3rdTex");
            bool useBacklight = IsFeatureOnFloat(material, "_UseBacklight");
            bool useParallax = IsFeatureOnFloat(material, "_UseParallax");
            bool usePOM = IsFeatureOnFloat(material, "_UsePOM");
            bool useReflection = IsFeatureOnFloat(material, "_UseReflection");
            bool useAlphaMask = IsFeatureOnFloat(material, "_AlphaMaskMode");
            bool useMain2ndTexDecalAnimation = IsFeatureOnDecalAnimation(material, "_Main2ndTexDecalAnimation");
            bool useMain3rdTexDecalAnimation = IsFeatureOnDecalAnimation(material, "_Main3rdTexDecalAnimation");
            bool useEmissionBlendMask = IsFeatureOnTexture(material, "_EmissionBlendMask");
            bool useEmission2ndBlendMask = IsFeatureOnTexture(material, "_Emission2ndBlendMask");

            bool isOutl = material.shader.name.Contains("Outline");
            bool isRefr = material.shader.name.Contains("Refraction");
            bool isFur = material.shader.name.Contains("Fur");
            bool isGem = material.shader.name.Contains("Gem");

            SetShaderKeywords(material, "UNITY_UI_ALPHACLIP",                   tpmode == 1);
            SetShaderKeywords(material, "UNITY_UI_CLIP_RECT",                   tpmode == 2 || tpmode == 4);
            SetShaderKeywords(material, "ETC1_EXTERNAL_ALPHA",                  tpmode == 1 && material.GetFloat("_UseDither") == 1.0f);
            material.SetShaderPassEnabled("ShadowCaster",                       material.GetFloat("_AsOverlay") == 0.0f);
            material.SetShaderPassEnabled("DepthOnly",                          material.GetFloat("_AsOverlay") == 0.0f);
            material.SetShaderPassEnabled("DepthNormals",                       material.GetFloat("_AsOverlay") == 0.0f);
            material.SetShaderPassEnabled("DepthForwardOnly",                   material.GetFloat("_AsOverlay") == 0.0f);
            material.SetShaderPassEnabled("MotionVectors",                      material.GetFloat("_AsOverlay") == 0.0f);

            if(isGem)
            {
                SetShaderKeywords(material, "_REQUIRE_UV2",                         false);
                SetShaderKeywords(material, "AUTO_KEY_VALUE",                       false);
                SetShaderKeywords(material, "_FADING_ON",                           false);
            }
            else
            {
                SetShaderKeywords(material, "_REQUIRE_UV2",                         useShadow);
                SetShaderKeywords(material, "AUTO_KEY_VALUE",                       useRimShade);
                SetShaderKeywords(material, "_FADING_ON",                           useDistanceFade);
            }

            SetShaderKeywords(material, "_EMISSION",                            useEmission);
            SetShaderKeywords(material, "GEOM_TYPE_BRANCH",                     useEmission2nd);
            SetShaderKeywords(material, "_SUNDISK_SIMPLE",                      (useEmission && useEmissionBlendMask) || (useEmission2nd && useEmission2ndBlendMask));
            SetShaderKeywords(material, "_NORMALMAP",                           useBumpMap);
            SetShaderKeywords(material, "EFFECT_BUMP",                          useBump2ndMap);
            SetShaderKeywords(material, "SOURCE_GBUFFER",                       useAnisotropy);
            SetShaderKeywords(material, "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A", useMatCap);
            SetShaderKeywords(material, "_SPECULARHIGHLIGHTS_OFF",              useMatCap2nd);
            SetShaderKeywords(material, "GEOM_TYPE_MESH",                       (useMatCap && useMatCapCustomNormal) || (useMatCap2nd && useMatCap2ndCustomNormal));
            SetShaderKeywords(material, "_METALLICGLOSSMAP",                    useRim);
            SetShaderKeywords(material, "GEOM_TYPE_LEAF",                       useRim && useRimDir);
            SetShaderKeywords(material, "_SPECGLOSSMAP",                        useGlitter);
            SetShaderKeywords(material, "_MAPPING_6_FRAMES_LAYOUT",             useAudioLink);
            SetShaderKeywords(material, "_SUNDISK_HIGH_QUALITY",                useAudioLink && audioLinkAsLocal);
            SetShaderKeywords(material, "GEOM_TYPE_BRANCH_DETAIL",              useDissolve);

            if(isGem)
            {
                SetShaderKeywords(material, "EFFECT_HUE_VARIATION",                 false);
                SetShaderKeywords(material, "_COLORADDSUBDIFF_ON",                  false);
                SetShaderKeywords(material, "_COLORCOLOR_ON",                       false);
                SetShaderKeywords(material, "_SUNDISK_NONE",                        false);
                SetShaderKeywords(material, "GEOM_TYPE_FROND",                      false);
                SetShaderKeywords(material, "_COLOROVERLAY_ON",                     false);
                SetShaderKeywords(material, "ANTI_FLICKER",                         false);
                SetShaderKeywords(material, "_PARALLAXMAP",                         false);
                SetShaderKeywords(material, "PIXELSNAP_ON",                         false);
                SetShaderKeywords(material, "_GLOSSYREFLECTIONS_OFF",               false);
            }
            else
            {
                SetShaderKeywords(material, "EFFECT_HUE_VARIATION",                 useMainTexHSVG || useMainGradation);
                SetShaderKeywords(material, "_COLORADDSUBDIFF_ON",                  useMain2ndTex);
                SetShaderKeywords(material, "_COLORCOLOR_ON",                       useMain3rdTex);
                SetShaderKeywords(material, "_SUNDISK_NONE",                        (useMain2ndTex && useMain2ndTexDecalAnimation) || (useMain3rdTex && useMain3rdTexDecalAnimation));
                SetShaderKeywords(material, "GEOM_TYPE_FROND",                      (useMain2ndTex && useMain2ndDissolve) || (useMain3rdTex && useMain3rdDissolve));
                SetShaderKeywords(material, "_COLOROVERLAY_ON",                     tpmode != 0 && useAlphaMask);
                SetShaderKeywords(material, "ANTI_FLICKER",                         useBacklight);
                SetShaderKeywords(material, "_PARALLAXMAP",                         useParallax);
                SetShaderKeywords(material, "PIXELSNAP_ON",                         useParallax && usePOM);
                SetShaderKeywords(material, "_GLOSSYREFLECTIONS_OFF",               useReflection);
            }

            if(isRefr || isFur || isGem)
            {
                SetShaderKeywords(material, "_DETAIL_MULX2",                        false);
            }
            else
            {
                SetShaderKeywords(material, "_DETAIL_MULX2",                        isOutl && material.GetVector("_OutlineTexHSVG") != lilConstants.defaultHSVG);
            }

            // Remove old keywords
            material.SetShaderPassEnabled("SRPDEFAULTUNLIT",                    true);
            SetShaderKeywords(material, "BILLBOARD_FACE_CAMERA_POS",            false);
        }

        public static void SetupMultiMaterial(Material[] materials, AnimationClip[] clips)
        {
            var ms = materials.Where(m => m.shader.name.Contains("Multi")).ToArray();
            foreach(var binding in clips.SelectMany(c => AnimationUtility.GetCurveBindings(c)).ToArray())
            {
                string propname = binding.propertyName;
                if(string.IsNullOrEmpty(propname) || !propname.Contains("material.")) continue;

                void Set(string name, string keyword)
                {
                    if(propname.Contains(name))
                        foreach(var m in ms)
                        {
                            m.EnableKeyword(keyword);
                            EditorUtility.SetDirty(m);
                        }
                }
                Set("_RimDirStrength", "GEOM_TYPE_LEAF");
                Set("_MainTexHSVG", "EFFECT_HUE_VARIATION");
                Set("_MainGradationStrength", "EFFECT_HUE_VARIATION");
            }
            AssetDatabase.SaveAssets();
        }

        private static bool IsFeatureOnFloat(Material material, string propname)
        {
            if(material.HasProperty(propname)) return material.GetFloat(propname) != 0.0f;
            return false;
        }

        private static bool IsFeatureOnVectorZ(Material material, string propname)
        {
            if(material.HasProperty(propname)) return material.GetVector(propname).z != 0.0f;
            return false;
        }

        private static bool IsFeatureOnVectorX(Material material, string propname)
        {
            if(material.HasProperty(propname)) return material.GetVector(propname).x != 0.0f;
            return false;
        }

        private static bool IsFeatureOnHSVG(Material material, string propname)
        {
            if(material.HasProperty(propname)) return material.GetVector(propname) != lilConstants.defaultHSVG;
            return false;
        }

        private static bool IsFeatureOnDecalAnimation(Material material, string propname)
        {
            if(material.HasProperty(propname)) return material.GetVector(propname) != lilConstants.defaultDecalAnim;
            return false;
        }

        private static bool IsFeatureOnTexture(Material material, string propname)
        {
            if(material.HasProperty(propname)) return material.GetTexture(propname) != null;
            return false;
        }

        private static void SetShaderKeywords(Material material, string keyword, bool enable)
        {
            if(enable)  material.EnableKeyword(keyword);
            else        material.DisableKeyword(keyword);
        }

        public static void RemoveUnusedTexture(Material material, params string[] animatedProps)
        {
            if(!material.shader.name.Contains("lilToon")) return;
            RemoveUnusedTexture(material, material.shader.name.Contains("Lite"), animatedProps);
        }

        public static void RemoveUnusedTexture(Material material, bool islite, params string[] animatedProps)
        {
            RemoveUnusedProperties(material);
            RemoveUnusedTextureOnly(material, islite, animatedProps);
        }

        public static void RemoveUnusedTextureOnly(Material material, bool islite, params string[] animatedProps)
        {
            if(islite)
            {
                if(IsPropZero(material, "_UseShadow", animatedProps))
                {
                    material.SetTexture("_ShadowColorTex", null);
                    material.SetTexture("_Shadow2ndColorTex", null);
                }
                if(IsPropZero(material, "_UseEmission", animatedProps))
                {
                    material.SetTexture("_EmissionMap", null);
                }
                if(IsPropZero(material, "_UseMatCap", animatedProps))
                {
                    material.SetTexture("_MatCapTex", null);
                }
            }
            else
            {
                if(IsPropZero(material, "_MainGradationStrength", animatedProps)) material.SetTexture("_MainGradationTex", null);
                if(IsPropZero(material, "_UseMain2ndTex", animatedProps))
                {
                    material.SetTexture("_Main2ndTex", null);
                    material.SetTexture("_Main2ndBlendMask", null);
                    material.SetTexture("_Main2ndDissolveMask", null);
                    material.SetTexture("_Main2ndDissolveNoiseMask", null);
                }
                if(IsPropZero(material, "_UseMain3rdTex", animatedProps))
                {
                    material.SetTexture("_Main3rdTex", null);
                    material.SetTexture("_Main3rdBlendMask", null);
                    material.SetTexture("_Main3rdDissolveMask", null);
                    material.SetTexture("_Main3rdDissolveNoiseMask", null);
                }
                if(IsPropZero(material, "_UseShadow", animatedProps))
                {
                    material.SetTexture("_ShadowBlurMask", null);
                    material.SetTexture("_ShadowBorderMask", null);
                    material.SetTexture("_ShadowStrengthMask", null);
                    material.SetTexture("_ShadowColorTex", null);
                    material.SetTexture("_Shadow2ndColorTex", null);
                    material.SetTexture("_Shadow3rdColorTex", null);
                }
                if(IsPropZero(material, "_UseRimShade", animatedProps))
                {
                    material.SetTexture("_RimShadeMask", null);
                }
                if(IsPropZero(material, "_UseEmission", animatedProps))
                {
                    material.SetTexture("_EmissionMap", null);
                    material.SetTexture("_EmissionBlendMask", null);
                    material.SetTexture("_EmissionGradTex", null);
                }
                if(IsPropZero(material, "_UseEmission2nd", animatedProps))
                {
                    material.SetTexture("_Emission2ndMap", null);
                    material.SetTexture("_Emission2ndBlendMask", null);
                    material.SetTexture("_Emission2ndGradTex", null);
                }
                if(IsPropZero(material, "_UseBumpMap", animatedProps)) material.SetTexture("_BumpMap", null);
                if(IsPropZero(material, "_UseBump2ndMap", animatedProps))
                {
                    material.SetTexture("_Bump2ndMap", null);
                    material.SetTexture("_Bump2ndScaleMask", null);
                }
                if(IsPropZero(material, "_UseAnisotropy", animatedProps))
                {
                    material.SetTexture("_AnisotropyTangentMap", null);
                    material.SetTexture("_AnisotropyScaleMask", null);
                    material.SetTexture("_AnisotropyShiftNoiseMask", null);
                }
                if(IsPropZero(material, "_UseReflection", animatedProps))
                {
                    material.SetTexture("_SmoothnessTex", null);
                    material.SetTexture("_MetallicGlossMap", null);
                    material.SetTexture("_ReflectionColorTex", null);
                    material.SetTexture("_ReflectionCubeTex", null);
                }
                if(IsPropZero(material, "_UseMatCap", animatedProps))
                {
                    material.SetTexture("_MatCapTex", null);
                    material.SetTexture("_MatCapBlendMask", null);
                }
                if(IsPropZero(material, "_UseMatCap2nd", animatedProps))
                {
                    material.SetTexture("_MatCap2ndTex", null);
                    material.SetTexture("_MatCap2ndBlendMask", null);
                }
                if(!material.shader.name.Contains("Outline"))
                {
                    material.SetTexture("_OutlineTex", null);
                    material.SetTexture("_OutlineWidthMask", null);
                    material.SetTexture("_OutlineVectorTex", null);
                }
                if(IsPropZero(material, "_UseRim", animatedProps)) material.SetTexture("_RimColorTex", null);
                if(IsPropZero(material, "_UseGlitter", animatedProps)) material.SetTexture("_GlitterColorTex", null);
                if(IsPropZero(material, "_UseParallax", animatedProps)) material.SetTexture("_ParallaxMap", null);
                if(IsPropZero(material, "_UseAudioLink", animatedProps) || material.GetFloat("_AudioLinkUVMode") != 3.0f || animatedProps.Contains("_AudioLinkUVMode")) material.SetTexture("_AudioLinkMask", null);
                if(IsPropZero(material, "_UseAudioLink", animatedProps) || IsPropZero(material, "_AudioLinkAsLocal", animatedProps)) material.SetTexture("_AudioLinkLocalMap", null);
            }
        }

        private static bool IsPropZero(Material material, string name, string[] animatedProps)
        {
            return material.GetFloat(name) == 0.0f && !animatedProps.Contains(name);
        }

        public static void RemoveShaderKeywords(Material material)
        {
            foreach(var keyword in material.shaderKeywords)
            {
                material.DisableKeyword(keyword);
            }
        }

        private static void RemoveUnusedProperties(Material material)
        {
            // https://light11.hatenadiary.com/entry/2018/12/04/224253
            var so = new SerializedObject(material);
            so.Update();
            var savedProps = so.FindProperty("m_SavedProperties");

            var texs = savedProps.FindPropertyRelative("m_TexEnvs");
            DeleteUnused(ref texs, material);

            var floats = savedProps.FindPropertyRelative("m_Floats");
            DeleteUnused(ref floats, material);

            var colors = savedProps.FindPropertyRelative("m_Colors");
            DeleteUnused(ref colors, material);

            so.ApplyModifiedProperties();
        }

        private static void DeleteUnused(ref SerializedProperty props, Material material)
        {
            for(int i = props.arraySize - 1; i >= 0; i--)
            {
                if(!material.HasProperty(props.GetArrayElementAtIndex(i).FindPropertyRelative("first").stringValue))
                {
                    props.DeleteArrayElementAtIndex(i);
                }
            }
        }

        public static int GetTrueRenderQueue(Material material)
        {
            var so = new SerializedObject(material);
            return so.FindProperty("m_CustomRenderQueue").intValue;
        }

        public static bool CheckShaderIslilToon(Material material)
        {
            if(material == null) return false;
            return CheckShaderIslilToon(material.shader);
        }

        public static bool CheckShaderIslilToon(Shader shader)
        {
            if(shader == null) return false;
            if(shader.name.Contains("lilToon") || shader.name.Contains("lts_pass")) return true;
            string shaderPath = AssetDatabase.GetAssetPath(shader);
            return !string.IsNullOrEmpty(shaderPath) && shaderPath.Contains(".lilcontainer");
        }
    }
}
#endif
