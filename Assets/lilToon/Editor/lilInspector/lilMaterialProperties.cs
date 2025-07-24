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
        // Material properties
        private readonly lilMaterialProperty invisible              = new lilMaterialProperty("_Invisible", PropertyBlock.Base);
        private readonly lilMaterialProperty cutoff                 = new lilMaterialProperty("_Cutoff", PropertyBlock.Base);
        private readonly lilMaterialProperty preColor               = new lilMaterialProperty("_PreColor", PropertyBlock.Base);
        private readonly lilMaterialProperty preOutType             = new lilMaterialProperty("_PreOutType", PropertyBlock.Base);
        private readonly lilMaterialProperty preCutoff              = new lilMaterialProperty("_PreCutoff", PropertyBlock.Base);
        private readonly lilMaterialProperty flipNormal             = new lilMaterialProperty("_FlipNormal", PropertyBlock.Base);
        private readonly lilMaterialProperty backfaceForceShadow    = new lilMaterialProperty("_BackfaceForceShadow", PropertyBlock.Base);
        private readonly lilMaterialProperty backfaceColor          = new lilMaterialProperty("_BackfaceColor", PropertyBlock.Base);
        private readonly lilMaterialProperty aaStrength             = new lilMaterialProperty("_AAStrength", PropertyBlock.Base);
        private readonly lilMaterialProperty useDither              = new lilMaterialProperty("_UseDither", PropertyBlock.Base);
        private readonly lilMaterialProperty ditherTex              = new lilMaterialProperty("_DitherTex", PropertyBlock.Base);
        private readonly lilMaterialProperty ditherMaxValue         = new lilMaterialProperty("_DitherMaxValue", PropertyBlock.Base);
        private readonly lilMaterialProperty envRimBorder           = new lilMaterialProperty("_EnvRimBorder", PropertyBlock.Base);
        private readonly lilMaterialProperty envRimBlur             = new lilMaterialProperty("_EnvRimBlur", PropertyBlock.Base);

        private readonly lilMaterialProperty asUnlit                        = new lilMaterialProperty("_AsUnlit", PropertyBlock.Lighting);
        private readonly lilMaterialProperty vertexLightStrength            = new lilMaterialProperty("_VertexLightStrength", PropertyBlock.Lighting);
        private readonly lilMaterialProperty lightMinLimit                  = new lilMaterialProperty("_LightMinLimit", PropertyBlock.Lighting);
        private readonly lilMaterialProperty lightMaxLimit                  = new lilMaterialProperty("_LightMaxLimit", PropertyBlock.Lighting);
        private readonly lilMaterialProperty beforeExposureLimit            = new lilMaterialProperty("_BeforeExposureLimit", PropertyBlock.Lighting, PropertyBlock.Rendering);
        private readonly lilMaterialProperty monochromeLighting             = new lilMaterialProperty("_MonochromeLighting", PropertyBlock.Lighting);
        private readonly lilMaterialProperty alphaBoostFA                   = new lilMaterialProperty("_AlphaBoostFA", PropertyBlock.Lighting);
        private readonly lilMaterialProperty lilDirectionalLightStrength    = new lilMaterialProperty("_lilDirectionalLightStrength", PropertyBlock.Lighting);
        private readonly lilMaterialProperty lightDirectionOverride         = new lilMaterialProperty("_LightDirectionOverride", PropertyBlock.Lighting);

        private readonly lilMaterialProperty baseColor      = new lilMaterialProperty("_BaseColor");
        private readonly lilMaterialProperty baseMap        = new lilMaterialProperty("_BaseMap", true);
        private readonly lilMaterialProperty baseColorMap   = new lilMaterialProperty("_BaseColorMap", true);

        private readonly lilMaterialProperty shiftBackfaceUV        = new lilMaterialProperty("_ShiftBackfaceUV", PropertyBlock.UV);
        private readonly lilMaterialProperty mainTex_ScrollRotate   = new lilMaterialProperty("_MainTex_ScrollRotate", PropertyBlock.UV);

        private readonly lilMaterialProperty mainColor              = new lilMaterialProperty("_Color", PropertyBlock.MainColor, PropertyBlock.MainColor1st);
        private readonly lilMaterialProperty mainTex                = new lilMaterialProperty("_MainTex", true, PropertyBlock.MainColor, PropertyBlock.MainColor1st);
        private readonly lilMaterialProperty mainTexHSVG            = new lilMaterialProperty("_MainTexHSVG", PropertyBlock.MainColor, PropertyBlock.MainColor1st);
        private readonly lilMaterialProperty mainGradationStrength  = new lilMaterialProperty("_MainGradationStrength", PropertyBlock.MainColor, PropertyBlock.MainColor1st);
        private readonly lilMaterialProperty mainGradationTex       = new lilMaterialProperty("_MainGradationTex", true, PropertyBlock.MainColor, PropertyBlock.MainColor1st);
        private readonly lilMaterialProperty mainColorAdjustMask    = new lilMaterialProperty("_MainColorAdjustMask", true, PropertyBlock.MainColor, PropertyBlock.MainColor1st);

        private readonly lilMaterialProperty useMain2ndTex                          = new lilMaterialProperty("_UseMain2ndTex", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty mainColor2nd                           = new lilMaterialProperty("_Color2nd", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTex                             = new lilMaterialProperty("_Main2ndTex", true, PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexAngle                        = new lilMaterialProperty("_Main2ndTexAngle", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTex_ScrollRotate                = new lilMaterialProperty("_Main2ndTex_ScrollRotate", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTex_UVMode                      = new lilMaterialProperty("_Main2ndTex_UVMode", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTex_Cull                        = new lilMaterialProperty("_Main2ndTex_Cull", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexDecalAnimation               = new lilMaterialProperty("_Main2ndTexDecalAnimation", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexDecalSubParam                = new lilMaterialProperty("_Main2ndTexDecalSubParam", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexIsDecal                      = new lilMaterialProperty("_Main2ndTexIsDecal", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexIsLeftOnly                   = new lilMaterialProperty("_Main2ndTexIsLeftOnly", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexIsRightOnly                  = new lilMaterialProperty("_Main2ndTexIsRightOnly", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexShouldCopy                   = new lilMaterialProperty("_Main2ndTexShouldCopy", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexShouldFlipMirror             = new lilMaterialProperty("_Main2ndTexShouldFlipMirror", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexShouldFlipCopy               = new lilMaterialProperty("_Main2ndTexShouldFlipCopy", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexIsMSDF                       = new lilMaterialProperty("_Main2ndTexIsMSDF", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndBlendMask                       = new lilMaterialProperty("_Main2ndBlendMask", true, PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexBlendMode                    = new lilMaterialProperty("_Main2ndTexBlendMode", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndTexAlphaMode                    = new lilMaterialProperty("_Main2ndTexAlphaMode", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndEnableLighting                  = new lilMaterialProperty("_Main2ndEnableLighting", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndDissolveMask                    = new lilMaterialProperty("_Main2ndDissolveMask", true, PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndDissolveNoiseMask               = new lilMaterialProperty("_Main2ndDissolveNoiseMask", true, PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndDissolveNoiseMask_ScrollRotate  = new lilMaterialProperty("_Main2ndDissolveNoiseMask_ScrollRotate", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndDissolveNoiseStrength           = new lilMaterialProperty("_Main2ndDissolveNoiseStrength", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndDissolveColor                   = new lilMaterialProperty("_Main2ndDissolveColor", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndDissolveParams                  = new lilMaterialProperty("_Main2ndDissolveParams", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndDissolvePos                     = new lilMaterialProperty("_Main2ndDissolvePos", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);
        private readonly lilMaterialProperty main2ndDistanceFade                    = new lilMaterialProperty("_Main2ndDistanceFade", PropertyBlock.MainColor, PropertyBlock.MainColor2nd);

        private readonly lilMaterialProperty useMain3rdTex                          = new lilMaterialProperty("_UseMain3rdTex", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty mainColor3rd                           = new lilMaterialProperty("_Color3rd", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexAngle                        = new lilMaterialProperty("_Main3rdTexAngle", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTex_ScrollRotate                = new lilMaterialProperty("_Main3rdTex_ScrollRotate", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTex                             = new lilMaterialProperty("_Main3rdTex", true, PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTex_UVMode                      = new lilMaterialProperty("_Main3rdTex_UVMode", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTex_Cull                        = new lilMaterialProperty("_Main3rdTex_Cull", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexDecalAnimation               = new lilMaterialProperty("_Main3rdTexDecalAnimation", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexDecalSubParam                = new lilMaterialProperty("_Main3rdTexDecalSubParam", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexIsDecal                      = new lilMaterialProperty("_Main3rdTexIsDecal", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexIsLeftOnly                   = new lilMaterialProperty("_Main3rdTexIsLeftOnly", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexIsRightOnly                  = new lilMaterialProperty("_Main3rdTexIsRightOnly", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexShouldCopy                   = new lilMaterialProperty("_Main3rdTexShouldCopy", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexShouldFlipMirror             = new lilMaterialProperty("_Main3rdTexShouldFlipMirror", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexShouldFlipCopy               = new lilMaterialProperty("_Main3rdTexShouldFlipCopy", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexIsMSDF                       = new lilMaterialProperty("_Main3rdTexIsMSDF", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdBlendMask                       = new lilMaterialProperty("_Main3rdBlendMask", true, PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexBlendMode                    = new lilMaterialProperty("_Main3rdTexBlendMode", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdTexAlphaMode                    = new lilMaterialProperty("_Main3rdTexAlphaMode", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdEnableLighting                  = new lilMaterialProperty("_Main3rdEnableLighting", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdDissolveMask                    = new lilMaterialProperty("_Main3rdDissolveMask", true, PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdDissolveNoiseMask               = new lilMaterialProperty("_Main3rdDissolveNoiseMask", true, PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdDissolveNoiseMask_ScrollRotate  = new lilMaterialProperty("_Main3rdDissolveNoiseMask_ScrollRotate", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdDissolveNoiseStrength           = new lilMaterialProperty("_Main3rdDissolveNoiseStrength", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdDissolveColor                   = new lilMaterialProperty("_Main3rdDissolveColor", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdDissolveParams                  = new lilMaterialProperty("_Main3rdDissolveParams", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdDissolvePos                     = new lilMaterialProperty("_Main3rdDissolvePos", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);
        private readonly lilMaterialProperty main3rdDistanceFade                    = new lilMaterialProperty("_Main3rdDistanceFade", PropertyBlock.MainColor, PropertyBlock.MainColor3rd);

        private readonly lilMaterialProperty alphaMaskMode  = new lilMaterialProperty("_AlphaMaskMode", PropertyBlock.MainColor, PropertyBlock.AlphaMask);
        private readonly lilMaterialProperty alphaMask      = new lilMaterialProperty("_AlphaMask", true, PropertyBlock.MainColor, PropertyBlock.AlphaMask);
        private readonly lilMaterialProperty alphaMaskScale = new lilMaterialProperty("_AlphaMaskScale", PropertyBlock.MainColor, PropertyBlock.AlphaMask);
        private readonly lilMaterialProperty alphaMaskValue = new lilMaterialProperty("_AlphaMaskValue", PropertyBlock.MainColor, PropertyBlock.AlphaMask);

        private readonly lilMaterialProperty useShadow                  = new lilMaterialProperty("_UseShadow", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowStrength             = new lilMaterialProperty("_ShadowStrength", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowStrengthMask         = new lilMaterialProperty("_ShadowStrengthMask", true, PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowBorderMask           = new lilMaterialProperty("_ShadowBorderMask", true, PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowBlurMask             = new lilMaterialProperty("_ShadowBlurMask", true, PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowStrengthMaskLOD      = new lilMaterialProperty("_ShadowStrengthMaskLOD", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowBorderMaskLOD        = new lilMaterialProperty("_ShadowBorderMaskLOD", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowBlurMaskLOD          = new lilMaterialProperty("_ShadowBlurMaskLOD", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowAOShift              = new lilMaterialProperty("_ShadowAOShift", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowAOShift2             = new lilMaterialProperty("_ShadowAOShift2", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowPostAO               = new lilMaterialProperty("_ShadowPostAO", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowColorType            = new lilMaterialProperty("_ShadowColorType", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowColor                = new lilMaterialProperty("_ShadowColor", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowColorTex             = new lilMaterialProperty("_ShadowColorTex", true, PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowNormalStrength       = new lilMaterialProperty("_ShadowNormalStrength", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowBorder               = new lilMaterialProperty("_ShadowBorder", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowBlur                 = new lilMaterialProperty("_ShadowBlur", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow2ndColor             = new lilMaterialProperty("_Shadow2ndColor", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow2ndColorTex          = new lilMaterialProperty("_Shadow2ndColorTex", true, PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow2ndNormalStrength    = new lilMaterialProperty("_Shadow2ndNormalStrength", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow2ndBorder            = new lilMaterialProperty("_Shadow2ndBorder", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow2ndBlur              = new lilMaterialProperty("_Shadow2ndBlur", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow3rdColor             = new lilMaterialProperty("_Shadow3rdColor", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow3rdColorTex          = new lilMaterialProperty("_Shadow3rdColorTex", true, PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow3rdNormalStrength    = new lilMaterialProperty("_Shadow3rdNormalStrength", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow3rdBorder            = new lilMaterialProperty("_Shadow3rdBorder", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow3rdBlur              = new lilMaterialProperty("_Shadow3rdBlur", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowMainStrength         = new lilMaterialProperty("_ShadowMainStrength", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowEnvStrength          = new lilMaterialProperty("_ShadowEnvStrength", PropertyBlock.Shadow, PropertyBlock.Lighting);
        private readonly lilMaterialProperty shadowBorderColor          = new lilMaterialProperty("_ShadowBorderColor", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowBorderRange          = new lilMaterialProperty("_ShadowBorderRange", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowReceive              = new lilMaterialProperty("_ShadowReceive", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow2ndReceive           = new lilMaterialProperty("_Shadow2ndReceive", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadow3rdReceive           = new lilMaterialProperty("_Shadow3rdReceive", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowMaskType             = new lilMaterialProperty("_ShadowMaskType", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowFlatBorder           = new lilMaterialProperty("_ShadowFlatBorder", PropertyBlock.Shadow);
        private readonly lilMaterialProperty shadowFlatBlur             = new lilMaterialProperty("_ShadowFlatBlur", PropertyBlock.Shadow);
        private readonly lilMaterialProperty lilShadowCasterBias        = new lilMaterialProperty("_lilShadowCasterBias", PropertyBlock.Shadow, PropertyBlock.Rendering);

        private readonly lilMaterialProperty useRimShade            = new lilMaterialProperty("_UseRimShade", PropertyBlock.RimShade);
        private readonly lilMaterialProperty rimShadeColor          = new lilMaterialProperty("_RimShadeColor", PropertyBlock.RimShade);
        private readonly lilMaterialProperty rimShadeMask           = new lilMaterialProperty("_RimShadeMask", PropertyBlock.RimShade);
        private readonly lilMaterialProperty rimShadeNormalStrength = new lilMaterialProperty("_RimShadeNormalStrength", PropertyBlock.RimShade);
        private readonly lilMaterialProperty rimShadeBorder         = new lilMaterialProperty("_RimShadeBorder", PropertyBlock.RimShade);
        private readonly lilMaterialProperty rimShadeBlur           = new lilMaterialProperty("_RimShadeBlur", PropertyBlock.RimShade);
        private readonly lilMaterialProperty rimShadeFresnelPower   = new lilMaterialProperty("_RimShadeFresnelPower", PropertyBlock.RimShade);

        private readonly lilMaterialProperty useEmission                    = new lilMaterialProperty("_UseEmission", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionColor                  = new lilMaterialProperty("_EmissionColor", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionMap                    = new lilMaterialProperty("_EmissionMap", true, PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionMap_ScrollRotate       = new lilMaterialProperty("_EmissionMap_ScrollRotate", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionMap_UVMode             = new lilMaterialProperty("_EmissionMap_UVMode", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionMainStrength           = new lilMaterialProperty("_EmissionMainStrength", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionBlend                  = new lilMaterialProperty("_EmissionBlend", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionBlendMask              = new lilMaterialProperty("_EmissionBlendMask", true, PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionBlendMask_ScrollRotate = new lilMaterialProperty("_EmissionBlendMask_ScrollRotate", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionBlendMode              = new lilMaterialProperty("_EmissionBlendMode", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionBlink                  = new lilMaterialProperty("_EmissionBlink", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionUseGrad                = new lilMaterialProperty("_EmissionUseGrad", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionGradTex                = new lilMaterialProperty("_EmissionGradTex", true, PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionGradSpeed              = new lilMaterialProperty("_EmissionGradSpeed", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionParallaxDepth          = new lilMaterialProperty("_EmissionParallaxDepth", PropertyBlock.Emission, PropertyBlock.Emission1st);
        private readonly lilMaterialProperty emissionFluorescence           = new lilMaterialProperty("_EmissionFluorescence", PropertyBlock.Emission, PropertyBlock.Emission1st);

        private readonly lilMaterialProperty useEmission2nd                     = new lilMaterialProperty("_UseEmission2nd", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndColor                   = new lilMaterialProperty("_Emission2ndColor", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndMap                     = new lilMaterialProperty("_Emission2ndMap", true, PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndMap_ScrollRotate        = new lilMaterialProperty("_Emission2ndMap_ScrollRotate", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndMap_UVMode              = new lilMaterialProperty("_Emission2ndMap_UVMode", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndMainStrength            = new lilMaterialProperty("_Emission2ndMainStrength", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndBlend                   = new lilMaterialProperty("_Emission2ndBlend", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndBlendMask               = new lilMaterialProperty("_Emission2ndBlendMask", true, PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndBlendMask_ScrollRotate  = new lilMaterialProperty("_Emission2ndBlendMask_ScrollRotate", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndBlendMode               = new lilMaterialProperty("_Emission2ndBlendMode", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndBlink                   = new lilMaterialProperty("_Emission2ndBlink", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndUseGrad                 = new lilMaterialProperty("_Emission2ndUseGrad", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndGradTex                 = new lilMaterialProperty("_Emission2ndGradTex", true, PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndGradSpeed               = new lilMaterialProperty("_Emission2ndGradSpeed", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndParallaxDepth           = new lilMaterialProperty("_Emission2ndParallaxDepth", PropertyBlock.Emission, PropertyBlock.Emission2nd);
        private readonly lilMaterialProperty emission2ndFluorescence            = new lilMaterialProperty("_Emission2ndFluorescence", PropertyBlock.Emission, PropertyBlock.Emission2nd);

        private readonly lilMaterialProperty useBumpMap = new lilMaterialProperty("_UseBumpMap", PropertyBlock.NormalMap, PropertyBlock.NormalMap1st);
        private readonly lilMaterialProperty bumpMap    = new lilMaterialProperty("_BumpMap", true, PropertyBlock.NormalMap, PropertyBlock.NormalMap1st);
        private readonly lilMaterialProperty bumpScale  = new lilMaterialProperty("_BumpScale", PropertyBlock.NormalMap, PropertyBlock.NormalMap1st);

        private readonly lilMaterialProperty useBump2ndMap      = new lilMaterialProperty("_UseBump2ndMap", PropertyBlock.NormalMap, PropertyBlock.NormalMap2nd);
        private readonly lilMaterialProperty bump2ndMap         = new lilMaterialProperty("_Bump2ndMap", true, PropertyBlock.NormalMap, PropertyBlock.NormalMap2nd);
        private readonly lilMaterialProperty bump2ndMap_UVMode  = new lilMaterialProperty("_Bump2ndMap_UVMode", PropertyBlock.NormalMap, PropertyBlock.NormalMap2nd);
        private readonly lilMaterialProperty bump2ndScale       = new lilMaterialProperty("_Bump2ndScale", PropertyBlock.NormalMap, PropertyBlock.NormalMap2nd);
        private readonly lilMaterialProperty bump2ndScaleMask   = new lilMaterialProperty("_Bump2ndScaleMask", true, PropertyBlock.NormalMap, PropertyBlock.NormalMap2nd);

        private readonly lilMaterialProperty useAnisotropy                  = new lilMaterialProperty("_UseAnisotropy", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropyTangentMap           = new lilMaterialProperty("_AnisotropyTangentMap", true, PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropyScale                = new lilMaterialProperty("_AnisotropyScale", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropyScaleMask            = new lilMaterialProperty("_AnisotropyScaleMask", true, PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropyTangentWidth         = new lilMaterialProperty("_AnisotropyTangentWidth", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropyBitangentWidth       = new lilMaterialProperty("_AnisotropyBitangentWidth", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropyShift                = new lilMaterialProperty("_AnisotropyShift", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropyShiftNoiseScale      = new lilMaterialProperty("_AnisotropyShiftNoiseScale", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropySpecularStrength     = new lilMaterialProperty("_AnisotropySpecularStrength", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropy2ndTangentWidth      = new lilMaterialProperty("_Anisotropy2ndTangentWidth", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropy2ndBitangentWidth    = new lilMaterialProperty("_Anisotropy2ndBitangentWidth", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropy2ndShift             = new lilMaterialProperty("_Anisotropy2ndShift", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropy2ndShiftNoiseScale   = new lilMaterialProperty("_Anisotropy2ndShiftNoiseScale", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropy2ndSpecularStrength  = new lilMaterialProperty("_Anisotropy2ndSpecularStrength", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropyShiftNoiseMask       = new lilMaterialProperty("_AnisotropyShiftNoiseMask", true, PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropy2Reflection          = new lilMaterialProperty("_Anisotropy2Reflection", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropy2MatCap              = new lilMaterialProperty("_Anisotropy2MatCap", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);
        private readonly lilMaterialProperty anisotropy2MatCap2nd           = new lilMaterialProperty("_Anisotropy2MatCap2nd", PropertyBlock.NormalMap, PropertyBlock.Anisotropy);

        private readonly lilMaterialProperty useBacklight               = new lilMaterialProperty("_UseBacklight", PropertyBlock.Backlight);
        private readonly lilMaterialProperty backlightColor             = new lilMaterialProperty("_BacklightColor", PropertyBlock.Backlight);
        private readonly lilMaterialProperty backlightColorTex          = new lilMaterialProperty("_BacklightColorTex", true, PropertyBlock.Backlight);
        private readonly lilMaterialProperty backlightMainStrength      = new lilMaterialProperty("_BacklightMainStrength", PropertyBlock.Backlight);
        private readonly lilMaterialProperty backlightNormalStrength    = new lilMaterialProperty("_BacklightNormalStrength", PropertyBlock.Backlight);
        private readonly lilMaterialProperty backlightBorder            = new lilMaterialProperty("_BacklightBorder", PropertyBlock.Backlight);
        private readonly lilMaterialProperty backlightBlur              = new lilMaterialProperty("_BacklightBlur", PropertyBlock.Backlight);
        private readonly lilMaterialProperty backlightDirectivity       = new lilMaterialProperty("_BacklightDirectivity", PropertyBlock.Backlight);
        private readonly lilMaterialProperty backlightViewStrength      = new lilMaterialProperty("_BacklightViewStrength", PropertyBlock.Backlight);
        private readonly lilMaterialProperty backlightReceiveShadow     = new lilMaterialProperty("_BacklightReceiveShadow", PropertyBlock.Backlight);
        private readonly lilMaterialProperty backlightBackfaceMask      = new lilMaterialProperty("_BacklightBackfaceMask", PropertyBlock.Backlight);

        private readonly lilMaterialProperty useReflection                  = new lilMaterialProperty("_UseReflection", PropertyBlock.Reflection);
        private readonly lilMaterialProperty metallic                       = new lilMaterialProperty("_Metallic", PropertyBlock.Reflection, PropertyBlock.Gem);
        private readonly lilMaterialProperty metallicGlossMap               = new lilMaterialProperty("_MetallicGlossMap", true, PropertyBlock.Reflection, PropertyBlock.Gem);
        private readonly lilMaterialProperty smoothness                     = new lilMaterialProperty("_Smoothness", PropertyBlock.Reflection);
        private readonly lilMaterialProperty smoothnessTex                  = new lilMaterialProperty("_SmoothnessTex", true, PropertyBlock.Reflection);
        private readonly lilMaterialProperty reflectance                    = new lilMaterialProperty("_Reflectance", PropertyBlock.Reflection);
        private readonly lilMaterialProperty reflectionColor                = new lilMaterialProperty("_ReflectionColor", PropertyBlock.Reflection);
        private readonly lilMaterialProperty reflectionColorTex             = new lilMaterialProperty("_ReflectionColorTex", true, PropertyBlock.Reflection);
        private readonly lilMaterialProperty gsaaStrength                   = new lilMaterialProperty("_GSAAStrength", PropertyBlock.Reflection);
        private readonly lilMaterialProperty applySpecular                  = new lilMaterialProperty("_ApplySpecular", PropertyBlock.Reflection);
        private readonly lilMaterialProperty applySpecularFA                = new lilMaterialProperty("_ApplySpecularFA", PropertyBlock.Reflection);
        private readonly lilMaterialProperty specularNormalStrength         = new lilMaterialProperty("_SpecularNormalStrength", PropertyBlock.Reflection);
        private readonly lilMaterialProperty specularToon                   = new lilMaterialProperty("_SpecularToon", PropertyBlock.Reflection);
        private readonly lilMaterialProperty specularBorder                 = new lilMaterialProperty("_SpecularBorder", PropertyBlock.Reflection);
        private readonly lilMaterialProperty specularBlur                   = new lilMaterialProperty("_SpecularBlur", PropertyBlock.Reflection);
        private readonly lilMaterialProperty applyReflection                = new lilMaterialProperty("_ApplyReflection", PropertyBlock.Reflection);
        private readonly lilMaterialProperty reflectionNormalStrength       = new lilMaterialProperty("_ReflectionNormalStrength", PropertyBlock.Reflection);
        private readonly lilMaterialProperty reflectionApplyTransparency    = new lilMaterialProperty("_ReflectionApplyTransparency", PropertyBlock.Reflection);
        private readonly lilMaterialProperty reflectionCubeTex              = new lilMaterialProperty("_ReflectionCubeTex", PropertyBlock.Reflection);
        private readonly lilMaterialProperty reflectionCubeColor            = new lilMaterialProperty("_ReflectionCubeColor", true, PropertyBlock.Reflection);
        private readonly lilMaterialProperty reflectionCubeOverride         = new lilMaterialProperty("_ReflectionCubeOverride", PropertyBlock.Reflection);
        private readonly lilMaterialProperty reflectionCubeEnableLighting   = new lilMaterialProperty("_ReflectionCubeEnableLighting", PropertyBlock.Reflection);
        private readonly lilMaterialProperty reflectionBlendMode            = new lilMaterialProperty("_ReflectionBlendMode", PropertyBlock.Reflection);

        private readonly lilMaterialProperty useMatCap                  = new lilMaterialProperty("_UseMatCap", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapTex                  = new lilMaterialProperty("_MatCapTex", true, PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapColor                = new lilMaterialProperty("_MatCapColor", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapMainStrength         = new lilMaterialProperty("_MatCapMainStrength", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapBlendUV1             = new lilMaterialProperty("_MatCapBlendUV1", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapZRotCancel           = new lilMaterialProperty("_MatCapZRotCancel", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapPerspective          = new lilMaterialProperty("_MatCapPerspective", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapVRParallaxStrength   = new lilMaterialProperty("_MatCapVRParallaxStrength", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapBlend                = new lilMaterialProperty("_MatCapBlend", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapBlendMask            = new lilMaterialProperty("_MatCapBlendMask", true, PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapEnableLighting       = new lilMaterialProperty("_MatCapEnableLighting", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapShadowMask           = new lilMaterialProperty("_MatCapShadowMask", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapBackfaceMask         = new lilMaterialProperty("_MatCapBackfaceMask", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapLod                  = new lilMaterialProperty("_MatCapLod", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapBlendMode            = new lilMaterialProperty("_MatCapBlendMode", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapApplyTransparency    = new lilMaterialProperty("_MatCapApplyTransparency", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapNormalStrength       = new lilMaterialProperty("_MatCapNormalStrength", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapCustomNormal         = new lilMaterialProperty("_MatCapCustomNormal", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapBumpMap              = new lilMaterialProperty("_MatCapBumpMap", true, PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty matcapBumpScale            = new lilMaterialProperty("_MatCapBumpScale", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);

        private readonly lilMaterialProperty useMatCap2nd                   = new lilMaterialProperty("_UseMatCap2nd", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndTex                   = new lilMaterialProperty("_MatCap2ndTex", true, PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndColor                 = new lilMaterialProperty("_MatCap2ndColor", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndMainStrength          = new lilMaterialProperty("_MatCap2ndMainStrength", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndBlendUV1              = new lilMaterialProperty("_MatCap2ndBlendUV1", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndZRotCancel            = new lilMaterialProperty("_MatCap2ndZRotCancel", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndPerspective           = new lilMaterialProperty("_MatCap2ndPerspective", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndVRParallaxStrength    = new lilMaterialProperty("_MatCap2ndVRParallaxStrength", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndBlend                 = new lilMaterialProperty("_MatCap2ndBlend", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndBlendMask             = new lilMaterialProperty("_MatCap2ndBlendMask", true, PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndEnableLighting        = new lilMaterialProperty("_MatCap2ndEnableLighting", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndShadowMask            = new lilMaterialProperty("_MatCap2ndShadowMask", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndBackfaceMask          = new lilMaterialProperty("_MatCap2ndBackfaceMask", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndLod                   = new lilMaterialProperty("_MatCap2ndLod", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndBlendMode             = new lilMaterialProperty("_MatCap2ndBlendMode", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndApplyTransparency     = new lilMaterialProperty("_MatCap2ndApplyTransparency", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndNormalStrength        = new lilMaterialProperty("_MatCap2ndNormalStrength", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndCustomNormal          = new lilMaterialProperty("_MatCap2ndCustomNormal", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndBumpMap               = new lilMaterialProperty("_MatCap2ndBumpMap", true, PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);
        private readonly lilMaterialProperty matcap2ndBumpScale             = new lilMaterialProperty("_MatCap2ndBumpScale", PropertyBlock.MatCaps, PropertyBlock.MatCap2nd);

        private readonly lilMaterialProperty useRim                 = new lilMaterialProperty("_UseRim", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimColor               = new lilMaterialProperty("_RimColor", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimColorTex            = new lilMaterialProperty("_RimColorTex", true, PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimMainStrength        = new lilMaterialProperty("_RimMainStrength", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimNormalStrength      = new lilMaterialProperty("_RimNormalStrength", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimBorder              = new lilMaterialProperty("_RimBorder", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimBlur                = new lilMaterialProperty("_RimBlur", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimFresnelPower        = new lilMaterialProperty("_RimFresnelPower", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimEnableLighting      = new lilMaterialProperty("_RimEnableLighting", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimShadowMask          = new lilMaterialProperty("_RimShadowMask", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimBackfaceMask        = new lilMaterialProperty("_RimBackfaceMask", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimVRParallaxStrength  = new lilMaterialProperty("_RimVRParallaxStrength", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimApplyTransparency   = new lilMaterialProperty("_RimApplyTransparency", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimDirStrength         = new lilMaterialProperty("_RimDirStrength", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimDirRange            = new lilMaterialProperty("_RimDirRange", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimIndirRange          = new lilMaterialProperty("_RimIndirRange", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimIndirColor          = new lilMaterialProperty("_RimIndirColor", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimIndirBorder         = new lilMaterialProperty("_RimIndirBorder", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimIndirBlur           = new lilMaterialProperty("_RimIndirBlur", PropertyBlock.RimLight);
        private readonly lilMaterialProperty rimBlendMode           = new lilMaterialProperty("_RimBlendMode", PropertyBlock.RimLight);

        private readonly lilMaterialProperty useGlitter                 = new lilMaterialProperty("_UseGlitter", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterUVMode              = new lilMaterialProperty("_GlitterUVMode", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterColor               = new lilMaterialProperty("_GlitterColor", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterColorTex            = new lilMaterialProperty("_GlitterColorTex", true, PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterColorTex_UVMode     = new lilMaterialProperty("_GlitterColorTex_UVMode", true, PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterMainStrength        = new lilMaterialProperty("_GlitterMainStrength", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterScaleRandomize      = new lilMaterialProperty("_GlitterScaleRandomize", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterApplyShape          = new lilMaterialProperty("_GlitterApplyShape", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterShapeTex            = new lilMaterialProperty("_GlitterShapeTex", true, PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterAtras               = new lilMaterialProperty("_GlitterAtras", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterAngleRandomize      = new lilMaterialProperty("_GlitterAngleRandomize", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterParams1             = new lilMaterialProperty("_GlitterParams1", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterParams2             = new lilMaterialProperty("_GlitterParams2", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterPostContrast        = new lilMaterialProperty("_GlitterPostContrast", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterSensitivity         = new lilMaterialProperty("_GlitterSensitivity", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterEnableLighting      = new lilMaterialProperty("_GlitterEnableLighting", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterShadowMask          = new lilMaterialProperty("_GlitterShadowMask", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterBackfaceMask        = new lilMaterialProperty("_GlitterBackfaceMask", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterApplyTransparency   = new lilMaterialProperty("_GlitterApplyTransparency", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterVRParallaxStrength  = new lilMaterialProperty("_GlitterVRParallaxStrength", PropertyBlock.Glitter);
        private readonly lilMaterialProperty glitterNormalStrength      = new lilMaterialProperty("_GlitterNormalStrength", PropertyBlock.Glitter);

        private readonly lilMaterialProperty gemChromaticAberration = new lilMaterialProperty("_GemChromaticAberration", PropertyBlock.Gem);
        private readonly lilMaterialProperty gemEnvContrast         = new lilMaterialProperty("_GemEnvContrast", PropertyBlock.Gem);
        private readonly lilMaterialProperty gemEnvColor            = new lilMaterialProperty("_GemEnvColor", PropertyBlock.Gem);
        private readonly lilMaterialProperty gemParticleLoop        = new lilMaterialProperty("_GemParticleLoop", PropertyBlock.Gem);
        private readonly lilMaterialProperty gemParticleColor       = new lilMaterialProperty("_GemParticleColor", PropertyBlock.Gem);
        private readonly lilMaterialProperty gemVRParallaxStrength  = new lilMaterialProperty("_GemVRParallaxStrength", PropertyBlock.Gem);

        private readonly lilMaterialProperty outlineColor               = new lilMaterialProperty("_OutlineColor", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineTex                 = new lilMaterialProperty("_OutlineTex", true, PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineTex_ScrollRotate    = new lilMaterialProperty("_OutlineTex_ScrollRotate", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineTexHSVG             = new lilMaterialProperty("_OutlineTexHSVG", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineLitColor            = new lilMaterialProperty("_OutlineLitColor", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineLitApplyTex         = new lilMaterialProperty("_OutlineLitApplyTex", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineLitScale            = new lilMaterialProperty("_OutlineLitScale", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineLitOffset           = new lilMaterialProperty("_OutlineLitOffset", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineLitShadowReceive    = new lilMaterialProperty("_OutlineLitShadowReceive", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineWidth               = new lilMaterialProperty("_OutlineWidth", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineWidthMask           = new lilMaterialProperty("_OutlineWidthMask", true, PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineFixWidth            = new lilMaterialProperty("_OutlineFixWidth", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineVertexR2Width       = new lilMaterialProperty("_OutlineVertexR2Width", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineDeleteMesh          = new lilMaterialProperty("_OutlineDeleteMesh", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineVectorTex           = new lilMaterialProperty("_OutlineVectorTex", true, PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineVectorUVMode        = new lilMaterialProperty("_OutlineVectorUVMode", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineVectorScale         = new lilMaterialProperty("_OutlineVectorScale", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineEnableLighting      = new lilMaterialProperty("_OutlineEnableLighting", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineZBias               = new lilMaterialProperty("_OutlineZBias", PropertyBlock.Outline);
        private readonly lilMaterialProperty outlineDisableInVR         = new lilMaterialProperty("_OutlineDisableInVR", PropertyBlock.Outline);

        private readonly lilMaterialProperty useParallax    = new lilMaterialProperty("_UseParallax", PropertyBlock.Parallax);
        private readonly lilMaterialProperty usePOM         = new lilMaterialProperty("_UsePOM", PropertyBlock.Parallax);
        private readonly lilMaterialProperty parallaxMap    = new lilMaterialProperty("_ParallaxMap", true, PropertyBlock.Parallax);
        private readonly lilMaterialProperty parallax       = new lilMaterialProperty("_Parallax", PropertyBlock.Parallax);
        private readonly lilMaterialProperty parallaxOffset = new lilMaterialProperty("_ParallaxOffset", PropertyBlock.Parallax);

        private readonly lilMaterialProperty distanceFade                = new lilMaterialProperty("_DistanceFade", PropertyBlock.DistanceFade);
        private readonly lilMaterialProperty distanceFadeColor           = new lilMaterialProperty("_DistanceFadeColor", PropertyBlock.DistanceFade);
        private readonly lilMaterialProperty distanceFadeMode            = new lilMaterialProperty("_DistanceFadeMode", PropertyBlock.DistanceFade);
        private readonly lilMaterialProperty distanceFadeRimColor        = new lilMaterialProperty("_DistanceFadeRimColor", PropertyBlock.DistanceFade);
        private readonly lilMaterialProperty distanceFadeRimFresnelPower = new lilMaterialProperty("_DistanceFadeRimFresnelPower", PropertyBlock.DistanceFade);

        private readonly lilMaterialProperty useAudioLink               = new lilMaterialProperty("_UseAudioLink", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkDefaultValue      = new lilMaterialProperty("_AudioLinkDefaultValue", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkUVMode            = new lilMaterialProperty("_AudioLinkUVMode", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkUVParams          = new lilMaterialProperty("_AudioLinkUVParams", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkStart             = new lilMaterialProperty("_AudioLinkStart", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkMask              = new lilMaterialProperty("_AudioLinkMask", true, PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkMask_ScrollRotate = new lilMaterialProperty("_AudioLinkMask_ScrollRotate", true, PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkMask_UVMode 　　　　= new lilMaterialProperty("_AudioLinkMask_UVMode", true, PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLink2Main2nd          = new lilMaterialProperty("_AudioLink2Main2nd", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLink2Main3rd          = new lilMaterialProperty("_AudioLink2Main3rd", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLink2Emission         = new lilMaterialProperty("_AudioLink2Emission", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLink2EmissionGrad     = new lilMaterialProperty("_AudioLink2EmissionGrad", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLink2Emission2nd      = new lilMaterialProperty("_AudioLink2Emission2nd", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLink2Emission2ndGrad  = new lilMaterialProperty("_AudioLink2Emission2ndGrad", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLink2Vertex           = new lilMaterialProperty("_AudioLink2Vertex", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkVertexUVMode      = new lilMaterialProperty("_AudioLinkVertexUVMode", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkVertexUVParams    = new lilMaterialProperty("_AudioLinkVertexUVParams", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkVertexStart       = new lilMaterialProperty("_AudioLinkVertexStart", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkVertexStrength    = new lilMaterialProperty("_AudioLinkVertexStrength", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkAsLocal           = new lilMaterialProperty("_AudioLinkAsLocal", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkLocalMap          = new lilMaterialProperty("_AudioLinkLocalMap", PropertyBlock.AudioLink);
        private readonly lilMaterialProperty audioLinkLocalMapParams    = new lilMaterialProperty("_AudioLinkLocalMapParams", PropertyBlock.AudioLink);

        private readonly lilMaterialProperty dissolveMask                   = new lilMaterialProperty("_DissolveMask", true, PropertyBlock.Dissolve);
        private readonly lilMaterialProperty dissolveNoiseMask              = new lilMaterialProperty("_DissolveNoiseMask", true, PropertyBlock.Dissolve);
        private readonly lilMaterialProperty dissolveNoiseMask_ScrollRotate = new lilMaterialProperty("_DissolveNoiseMask_ScrollRotate", PropertyBlock.Dissolve);
        private readonly lilMaterialProperty dissolveNoiseStrength          = new lilMaterialProperty("_DissolveNoiseStrength", PropertyBlock.Dissolve);
        private readonly lilMaterialProperty dissolveColor                  = new lilMaterialProperty("_DissolveColor", PropertyBlock.Dissolve);
        private readonly lilMaterialProperty dissolveParams                 = new lilMaterialProperty("_DissolveParams", PropertyBlock.Dissolve);
        private readonly lilMaterialProperty dissolvePos                    = new lilMaterialProperty("_DissolvePos", PropertyBlock.Dissolve);

        private readonly lilMaterialProperty idMaskCompile  = new lilMaterialProperty("_IDMaskCompile", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMaskFrom     = new lilMaterialProperty("_IDMaskFrom", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMask1        = new lilMaterialProperty("_IDMask1", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMask2        = new lilMaterialProperty("_IDMask2", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMask3        = new lilMaterialProperty("_IDMask3", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMask4        = new lilMaterialProperty("_IDMask4", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMask5        = new lilMaterialProperty("_IDMask5", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMask6        = new lilMaterialProperty("_IDMask6", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMask7        = new lilMaterialProperty("_IDMask7", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMask8        = new lilMaterialProperty("_IDMask8", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMaskIsBitmap = new lilMaterialProperty("_IDMaskIsBitmap", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMaskIndex1   = new lilMaterialProperty("_IDMaskIndex1", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMaskIndex2   = new lilMaterialProperty("_IDMaskIndex2", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMaskIndex3   = new lilMaterialProperty("_IDMaskIndex3", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMaskIndex4   = new lilMaterialProperty("_IDMaskIndex4", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMaskIndex5   = new lilMaterialProperty("_IDMaskIndex5", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMaskIndex6   = new lilMaterialProperty("_IDMaskIndex6", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMaskIndex7   = new lilMaterialProperty("_IDMaskIndex7", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMaskIndex8   = new lilMaterialProperty("_IDMaskIndex8", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMaskControlsDissolve = new lilMaterialProperty("_IDMaskControlsDissolve", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMaskPrior1   = new lilMaterialProperty("_IDMaskPrior1", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMaskPrior2   = new lilMaterialProperty("_IDMaskPrior2", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMaskPrior3   = new lilMaterialProperty("_IDMaskPrior3", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMaskPrior4   = new lilMaterialProperty("_IDMaskPrior4", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMaskPrior5   = new lilMaterialProperty("_IDMaskPrior5", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMaskPrior6   = new lilMaterialProperty("_IDMaskPrior6", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMaskPrior7   = new lilMaterialProperty("_IDMaskPrior7", PropertyBlock.IDMask);
        private readonly lilMaterialProperty idMaskPrior8   = new lilMaterialProperty("_IDMaskPrior8", PropertyBlock.IDMask);
        
        private readonly lilMaterialProperty udimDiscardCompile    = new lilMaterialProperty("_UDIMDiscardCompile", PropertyBlock.UDIMDiscard);
        private readonly lilMaterialProperty udimDiscardUV         = new lilMaterialProperty("_UDIMDiscardUV", PropertyBlock.UDIMDiscard);
        private readonly lilMaterialProperty udimDiscardMethod     = new lilMaterialProperty("_UDIMDiscardMode", PropertyBlock.UDIMDiscard);
        private readonly lilMaterialProperty udimDiscardRow3_0     = new lilMaterialProperty("_UDIMDiscardRow3_0", PropertyBlock.UDIMDiscard);
        private readonly lilMaterialProperty udimDiscardRow3_1     = new lilMaterialProperty("_UDIMDiscardRow3_1", PropertyBlock.UDIMDiscard);
        private readonly lilMaterialProperty udimDiscardRow3_2     = new lilMaterialProperty("_UDIMDiscardRow3_2", PropertyBlock.UDIMDiscard);
        private readonly lilMaterialProperty udimDiscardRow3_3     = new lilMaterialProperty("_UDIMDiscardRow3_3", PropertyBlock.UDIMDiscard);
        private readonly lilMaterialProperty udimDiscardRow2_0     = new lilMaterialProperty("_UDIMDiscardRow2_0", PropertyBlock.UDIMDiscard);
        private readonly lilMaterialProperty udimDiscardRow2_1     = new lilMaterialProperty("_UDIMDiscardRow2_1", PropertyBlock.UDIMDiscard);
        private readonly lilMaterialProperty udimDiscardRow2_2     = new lilMaterialProperty("_UDIMDiscardRow2_2", PropertyBlock.UDIMDiscard);
        private readonly lilMaterialProperty udimDiscardRow2_3     = new lilMaterialProperty("_UDIMDiscardRow2_3", PropertyBlock.UDIMDiscard);
        private readonly lilMaterialProperty udimDiscardRow1_0     = new lilMaterialProperty("_UDIMDiscardRow1_0", PropertyBlock.UDIMDiscard);
        private readonly lilMaterialProperty udimDiscardRow1_1     = new lilMaterialProperty("_UDIMDiscardRow1_1", PropertyBlock.UDIMDiscard);
        private readonly lilMaterialProperty udimDiscardRow1_2     = new lilMaterialProperty("_UDIMDiscardRow1_2", PropertyBlock.UDIMDiscard);
        private readonly lilMaterialProperty udimDiscardRow1_3     = new lilMaterialProperty("_UDIMDiscardRow1_3", PropertyBlock.UDIMDiscard);
        private readonly lilMaterialProperty udimDiscardRow0_0     = new lilMaterialProperty("_UDIMDiscardRow0_0", PropertyBlock.UDIMDiscard);
        private readonly lilMaterialProperty udimDiscardRow0_1     = new lilMaterialProperty("_UDIMDiscardRow0_1", PropertyBlock.UDIMDiscard);
        private readonly lilMaterialProperty udimDiscardRow0_2     = new lilMaterialProperty("_UDIMDiscardRow0_2", PropertyBlock.UDIMDiscard);
        private readonly lilMaterialProperty udimDiscardRow0_3     = new lilMaterialProperty("_UDIMDiscardRow0_3", PropertyBlock.UDIMDiscard);

        private readonly lilMaterialProperty refractionStrength         = new lilMaterialProperty("_RefractionStrength", PropertyBlock.Refraction, PropertyBlock.Gem);
        private readonly lilMaterialProperty refractionFresnelPower     = new lilMaterialProperty("_RefractionFresnelPower", PropertyBlock.Refraction, PropertyBlock.Gem);
        private readonly lilMaterialProperty refractionColorFromMain    = new lilMaterialProperty("_RefractionColorFromMain", PropertyBlock.Refraction);
        private readonly lilMaterialProperty refractionColor            = new lilMaterialProperty("_RefractionColor", PropertyBlock.Refraction);

        private readonly lilMaterialProperty furNoiseMask           = new lilMaterialProperty("_FurNoiseMask", true, PropertyBlock.Fur);
        private readonly lilMaterialProperty furMask                = new lilMaterialProperty("_FurMask", true, PropertyBlock.Fur);
        private readonly lilMaterialProperty furLengthMask          = new lilMaterialProperty("_FurLengthMask", true, PropertyBlock.Fur);
        private readonly lilMaterialProperty furVectorTex           = new lilMaterialProperty("_FurVectorTex", true, PropertyBlock.Fur);
        private readonly lilMaterialProperty furVectorScale         = new lilMaterialProperty("_FurVectorScale", PropertyBlock.Fur);
        private readonly lilMaterialProperty furVector              = new lilMaterialProperty("_FurVector", PropertyBlock.Fur);
        private readonly lilMaterialProperty furGravity             = new lilMaterialProperty("_FurGravity", PropertyBlock.Fur);
        private readonly lilMaterialProperty furRandomize           = new lilMaterialProperty("_FurRandomize", PropertyBlock.Fur);
        private readonly lilMaterialProperty furAO                  = new lilMaterialProperty("_FurAO", PropertyBlock.Fur);
        private readonly lilMaterialProperty vertexColor2FurVector  = new lilMaterialProperty("_VertexColor2FurVector", PropertyBlock.Fur);
        private readonly lilMaterialProperty furLayerNum            = new lilMaterialProperty("_FurLayerNum", PropertyBlock.Fur);
        private readonly lilMaterialProperty furRootOffset          = new lilMaterialProperty("_FurRootOffset", PropertyBlock.Fur);
        private readonly lilMaterialProperty furCutoutLength        = new lilMaterialProperty("_FurCutoutLength", PropertyBlock.Fur);
        private readonly lilMaterialProperty furTouchStrength       = new lilMaterialProperty("_FurTouchStrength", PropertyBlock.Fur);
        private readonly lilMaterialProperty furRimColor            = new lilMaterialProperty("_FurRimColor", PropertyBlock.Fur);
        private readonly lilMaterialProperty furRimFresnelPower     = new lilMaterialProperty("_FurRimFresnelPower", PropertyBlock.Fur);
        private readonly lilMaterialProperty furRimAntiLight        = new lilMaterialProperty("_FurRimAntiLight", PropertyBlock.Fur);

        private readonly lilMaterialProperty stencilRef                 = new lilMaterialProperty("_StencilRef", PropertyBlock.Stencil);
        private readonly lilMaterialProperty stencilReadMask            = new lilMaterialProperty("_StencilReadMask", PropertyBlock.Stencil);
        private readonly lilMaterialProperty stencilWriteMask           = new lilMaterialProperty("_StencilWriteMask", PropertyBlock.Stencil);
        private readonly lilMaterialProperty stencilComp                = new lilMaterialProperty("_StencilComp", PropertyBlock.Stencil);
        private readonly lilMaterialProperty stencilPass                = new lilMaterialProperty("_StencilPass", PropertyBlock.Stencil);
        private readonly lilMaterialProperty stencilFail                = new lilMaterialProperty("_StencilFail", PropertyBlock.Stencil);
        private readonly lilMaterialProperty stencilZFail               = new lilMaterialProperty("_StencilZFail", PropertyBlock.Stencil);
        private readonly lilMaterialProperty preStencilRef              = new lilMaterialProperty("_PreStencilRef", PropertyBlock.Stencil);
        private readonly lilMaterialProperty preStencilReadMask         = new lilMaterialProperty("_PreStencilReadMask", PropertyBlock.Stencil);
        private readonly lilMaterialProperty preStencilWriteMask        = new lilMaterialProperty("_PreStencilWriteMask", PropertyBlock.Stencil);
        private readonly lilMaterialProperty preStencilComp             = new lilMaterialProperty("_PreStencilComp", PropertyBlock.Stencil);
        private readonly lilMaterialProperty preStencilPass             = new lilMaterialProperty("_PreStencilPass", PropertyBlock.Stencil);
        private readonly lilMaterialProperty preStencilFail             = new lilMaterialProperty("_PreStencilFail", PropertyBlock.Stencil);
        private readonly lilMaterialProperty preStencilZFail            = new lilMaterialProperty("_PreStencilZFail", PropertyBlock.Stencil);
        private readonly lilMaterialProperty outlineStencilRef          = new lilMaterialProperty("_OutlineStencilRef", PropertyBlock.Stencil);
        private readonly lilMaterialProperty outlineStencilReadMask     = new lilMaterialProperty("_OutlineStencilReadMask", PropertyBlock.Stencil);
        private readonly lilMaterialProperty outlineStencilWriteMask    = new lilMaterialProperty("_OutlineStencilWriteMask", PropertyBlock.Stencil);
        private readonly lilMaterialProperty outlineStencilComp         = new lilMaterialProperty("_OutlineStencilComp", PropertyBlock.Stencil);
        private readonly lilMaterialProperty outlineStencilPass         = new lilMaterialProperty("_OutlineStencilPass", PropertyBlock.Stencil);
        private readonly lilMaterialProperty outlineStencilFail         = new lilMaterialProperty("_OutlineStencilFail", PropertyBlock.Stencil);
        private readonly lilMaterialProperty outlineStencilZFail        = new lilMaterialProperty("_OutlineStencilZFail", PropertyBlock.Stencil);
        private readonly lilMaterialProperty furStencilRef              = new lilMaterialProperty("_FurStencilRef", PropertyBlock.Stencil);
        private readonly lilMaterialProperty furStencilReadMask         = new lilMaterialProperty("_FurStencilReadMask", PropertyBlock.Stencil);
        private readonly lilMaterialProperty furStencilWriteMask        = new lilMaterialProperty("_FurStencilWriteMask", PropertyBlock.Stencil);
        private readonly lilMaterialProperty furStencilComp             = new lilMaterialProperty("_FurStencilComp", PropertyBlock.Stencil);
        private readonly lilMaterialProperty furStencilPass             = new lilMaterialProperty("_FurStencilPass", PropertyBlock.Stencil);
        private readonly lilMaterialProperty furStencilFail             = new lilMaterialProperty("_FurStencilFail", PropertyBlock.Stencil);
        private readonly lilMaterialProperty furStencilZFail            = new lilMaterialProperty("_FurStencilZFail", PropertyBlock.Stencil);

        private readonly lilMaterialProperty subpassCutoff          = new lilMaterialProperty("_SubpassCutoff", PropertyBlock.Rendering);
        private readonly lilMaterialProperty cull                   = new lilMaterialProperty("_Cull", PropertyBlock.Rendering, PropertyBlock.Base);
        private readonly lilMaterialProperty srcBlend               = new lilMaterialProperty("_SrcBlend", PropertyBlock.Rendering);
        private readonly lilMaterialProperty dstBlend               = new lilMaterialProperty("_DstBlend", PropertyBlock.Rendering);
        private readonly lilMaterialProperty srcBlendAlpha          = new lilMaterialProperty("_SrcBlendAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty dstBlendAlpha          = new lilMaterialProperty("_DstBlendAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty blendOp                = new lilMaterialProperty("_BlendOp", PropertyBlock.Rendering);
        private readonly lilMaterialProperty blendOpAlpha           = new lilMaterialProperty("_BlendOpAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty srcBlendFA             = new lilMaterialProperty("_SrcBlendFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty dstBlendFA             = new lilMaterialProperty("_DstBlendFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty srcBlendAlphaFA        = new lilMaterialProperty("_SrcBlendAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty dstBlendAlphaFA        = new lilMaterialProperty("_DstBlendAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty blendOpFA              = new lilMaterialProperty("_BlendOpFA", PropertyBlock.Rendering, PropertyBlock.Lighting);
        private readonly lilMaterialProperty blendOpAlphaFA         = new lilMaterialProperty("_BlendOpAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty zclip                  = new lilMaterialProperty("_ZClip", PropertyBlock.Rendering);
        private readonly lilMaterialProperty zwrite                 = new lilMaterialProperty("_ZWrite", PropertyBlock.Rendering, PropertyBlock.Base);
        private readonly lilMaterialProperty ztest                  = new lilMaterialProperty("_ZTest", PropertyBlock.Rendering);
        private readonly lilMaterialProperty offsetFactor           = new lilMaterialProperty("_OffsetFactor", PropertyBlock.Rendering);
        private readonly lilMaterialProperty offsetUnits            = new lilMaterialProperty("_OffsetUnits", PropertyBlock.Rendering);
        private readonly lilMaterialProperty colorMask              = new lilMaterialProperty("_ColorMask", PropertyBlock.Rendering);
        private readonly lilMaterialProperty alphaToMask            = new lilMaterialProperty("_AlphaToMask", PropertyBlock.Rendering);

        private readonly lilMaterialProperty preCull                = new lilMaterialProperty("_PreCull", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preSrcBlend            = new lilMaterialProperty("_PreSrcBlend", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preDstBlend            = new lilMaterialProperty("_PreDstBlend", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preSrcBlendAlpha       = new lilMaterialProperty("_PreSrcBlendAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preDstBlendAlpha       = new lilMaterialProperty("_PreDstBlendAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preBlendOp             = new lilMaterialProperty("_PreBlendOp", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preBlendOpAlpha        = new lilMaterialProperty("_PreBlendOpAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preSrcBlendFA          = new lilMaterialProperty("_PreSrcBlendFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preDstBlendFA          = new lilMaterialProperty("_PreDstBlendFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preSrcBlendAlphaFA     = new lilMaterialProperty("_PreSrcBlendAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preDstBlendAlphaFA     = new lilMaterialProperty("_PreDstBlendAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preBlendOpFA           = new lilMaterialProperty("_PreBlendOpFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preBlendOpAlphaFA      = new lilMaterialProperty("_PreBlendOpAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preZclip               = new lilMaterialProperty("_PreZClip", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preZwrite              = new lilMaterialProperty("_PreZWrite", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preZtest               = new lilMaterialProperty("_PreZTest", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preOffsetFactor        = new lilMaterialProperty("_PreOffsetFactor", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preOffsetUnits         = new lilMaterialProperty("_PreOffsetUnits", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preColorMask           = new lilMaterialProperty("_PreColorMask", PropertyBlock.Rendering);
        private readonly lilMaterialProperty preAlphaToMask         = new lilMaterialProperty("_PreAlphaToMask", PropertyBlock.Rendering);

        private readonly lilMaterialProperty outlineCull            = new lilMaterialProperty("_OutlineCull", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineSrcBlend        = new lilMaterialProperty("_OutlineSrcBlend", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineDstBlend        = new lilMaterialProperty("_OutlineDstBlend", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineSrcBlendAlpha   = new lilMaterialProperty("_OutlineSrcBlendAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineDstBlendAlpha   = new lilMaterialProperty("_OutlineDstBlendAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineBlendOp         = new lilMaterialProperty("_OutlineBlendOp", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineBlendOpAlpha    = new lilMaterialProperty("_OutlineBlendOpAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineSrcBlendFA      = new lilMaterialProperty("_OutlineSrcBlendFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineDstBlendFA      = new lilMaterialProperty("_OutlineDstBlendFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineSrcBlendAlphaFA = new lilMaterialProperty("_OutlineSrcBlendAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineDstBlendAlphaFA = new lilMaterialProperty("_OutlineDstBlendAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineBlendOpFA       = new lilMaterialProperty("_OutlineBlendOpFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineBlendOpAlphaFA  = new lilMaterialProperty("_OutlineBlendOpAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineZclip           = new lilMaterialProperty("_OutlineZClip", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineZwrite          = new lilMaterialProperty("_OutlineZWrite", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineZtest           = new lilMaterialProperty("_OutlineZTest", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineOffsetFactor    = new lilMaterialProperty("_OutlineOffsetFactor", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineOffsetUnits     = new lilMaterialProperty("_OutlineOffsetUnits", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineColorMask       = new lilMaterialProperty("_OutlineColorMask", PropertyBlock.Rendering);
        private readonly lilMaterialProperty outlineAlphaToMask     = new lilMaterialProperty("_OutlineAlphaToMask", PropertyBlock.Rendering);

        private readonly lilMaterialProperty furCull                = new lilMaterialProperty("_FurCull", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furSrcBlend            = new lilMaterialProperty("_FurSrcBlend", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furDstBlend            = new lilMaterialProperty("_FurDstBlend", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furSrcBlendAlpha       = new lilMaterialProperty("_FurSrcBlendAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furDstBlendAlpha       = new lilMaterialProperty("_FurDstBlendAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furBlendOp             = new lilMaterialProperty("_FurBlendOp", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furBlendOpAlpha        = new lilMaterialProperty("_FurBlendOpAlpha", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furSrcBlendFA          = new lilMaterialProperty("_FurSrcBlendFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furDstBlendFA          = new lilMaterialProperty("_FurDstBlendFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furSrcBlendAlphaFA     = new lilMaterialProperty("_FurSrcBlendAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furDstBlendAlphaFA     = new lilMaterialProperty("_FurDstBlendAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furBlendOpFA           = new lilMaterialProperty("_FurBlendOpFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furBlendOpAlphaFA      = new lilMaterialProperty("_FurBlendOpAlphaFA", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furZclip               = new lilMaterialProperty("_FurZClip", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furZwrite              = new lilMaterialProperty("_FurZWrite", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furZtest               = new lilMaterialProperty("_FurZTest", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furOffsetFactor        = new lilMaterialProperty("_FurOffsetFactor", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furOffsetUnits         = new lilMaterialProperty("_FurOffsetUnits", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furColorMask           = new lilMaterialProperty("_FurColorMask", PropertyBlock.Rendering);
        private readonly lilMaterialProperty furAlphaToMask         = new lilMaterialProperty("_FurAlphaToMask", PropertyBlock.Rendering);

        private readonly lilMaterialProperty tessEdge               = new lilMaterialProperty("_TessEdge", PropertyBlock.Tessellation);
        private readonly lilMaterialProperty tessStrength           = new lilMaterialProperty("_TessStrength", PropertyBlock.Tessellation);
        private readonly lilMaterialProperty tessShrink             = new lilMaterialProperty("_TessShrink", PropertyBlock.Tessellation);
        private readonly lilMaterialProperty tessFactorMax          = new lilMaterialProperty("_TessFactorMax", PropertyBlock.Tessellation);

        private readonly lilMaterialProperty transparentModeMat     = new lilMaterialProperty("_TransparentMode", PropertyBlock.Base);
        private readonly lilMaterialProperty useClippingCanceller   = new lilMaterialProperty("_UseClippingCanceller", PropertyBlock.Base);
        private readonly lilMaterialProperty asOverlay              = new lilMaterialProperty("_AsOverlay", PropertyBlock.Base);
        private readonly lilMaterialProperty triMask                = new lilMaterialProperty("_TriMask", true, PropertyBlock.Base);
        private readonly lilMaterialProperty matcapMul              = new lilMaterialProperty("_MatCapMul", PropertyBlock.MatCaps, PropertyBlock.MatCap1st);
        private readonly lilMaterialProperty fakeShadowVector       = new lilMaterialProperty("_FakeShadowVector", PropertyBlock.Base);

        private readonly lilMaterialProperty ramp = new lilMaterialProperty("_Ramp", true);

        private lilMaterialProperty[] allProperty;
        private lilMaterialProperty[] AllProperties()
        {
            return allProperty != null ? allProperty : allProperty = new[]
            {
                invisible,
                cutoff,
                preColor,
                preOutType,
                preCutoff,
                flipNormal,
                backfaceForceShadow,
                backfaceColor,
                aaStrength,
                useDither,
                ditherTex,
                ditherMaxValue,
                envRimBorder,
                envRimBlur,

                asUnlit,
                vertexLightStrength,
                lightMinLimit,
                lightMaxLimit,
                beforeExposureLimit,
                monochromeLighting,
                alphaBoostFA,
                lilDirectionalLightStrength,
                lightDirectionOverride,

                baseColor,
                baseMap,
                baseColorMap,

                shiftBackfaceUV,
                mainTex_ScrollRotate,

                mainColor,
                mainTex,
                mainTexHSVG,
                mainGradationStrength,
                mainGradationTex,
                mainColorAdjustMask,

                useMain2ndTex,
                mainColor2nd,
                main2ndTex,
                main2ndTexAngle,
                main2ndTex_ScrollRotate,
                main2ndTex_UVMode,
                main2ndTex_Cull,
                main2ndTexDecalAnimation,
                main2ndTexDecalSubParam,
                main2ndTexIsDecal,
                main2ndTexIsLeftOnly,
                main2ndTexIsRightOnly,
                main2ndTexShouldCopy,
                main2ndTexShouldFlipMirror,
                main2ndTexShouldFlipCopy,
                main2ndTexIsMSDF,
                main2ndBlendMask,
                main2ndTexBlendMode,
                main2ndTexAlphaMode,
                main2ndEnableLighting,
                main2ndDissolveMask,
                main2ndDissolveNoiseMask,
                main2ndDissolveNoiseMask_ScrollRotate,
                main2ndDissolveNoiseStrength,
                main2ndDissolveColor,
                main2ndDissolveParams,
                main2ndDissolvePos,
                main2ndDistanceFade,

                useMain3rdTex,
                mainColor3rd,
                main3rdTex,
                main3rdTexAngle,
                main3rdTex_ScrollRotate,
                main3rdTex_UVMode,
                main3rdTex_Cull,
                main3rdTexDecalAnimation,
                main3rdTexDecalSubParam,
                main3rdTexIsDecal,
                main3rdTexIsLeftOnly,
                main3rdTexIsRightOnly,
                main3rdTexShouldCopy,
                main3rdTexShouldFlipMirror,
                main3rdTexShouldFlipCopy,
                main3rdTexIsMSDF,
                main3rdBlendMask,
                main3rdTexBlendMode,
                main3rdTexAlphaMode,
                main3rdEnableLighting,
                main3rdDissolveMask,
                main3rdDissolveNoiseMask,
                main3rdDissolveNoiseMask_ScrollRotate,
                main3rdDissolveNoiseStrength,
                main3rdDissolveColor,
                main3rdDissolveParams,
                main3rdDissolvePos,
                main3rdDistanceFade,

                alphaMaskMode,
                alphaMask,
                alphaMaskScale,
                alphaMaskValue,

                useShadow,
                shadowStrength,
                shadowStrengthMask,
                shadowBorderMask,
                shadowBlurMask,
                shadowStrengthMaskLOD,
                shadowBorderMaskLOD,
                shadowBlurMaskLOD,
                shadowAOShift,
                shadowAOShift2,
                shadowPostAO,
                shadowColorType,
                shadowColor,
                shadowColorTex,
                shadowNormalStrength,
                shadowBorder,
                shadowBlur,
                shadow2ndColor,
                shadow2ndColorTex,
                shadow2ndNormalStrength,
                shadow2ndBorder,
                shadow2ndBlur,
                shadow3rdColor,
                shadow3rdColorTex,
                shadow3rdNormalStrength,
                shadow3rdBorder,
                shadow3rdBlur,
                shadowMainStrength,
                shadowEnvStrength,
                shadowBorderColor,
                shadowBorderRange,
                shadowReceive,
                shadow2ndReceive,
                shadow3rdReceive,
                shadowMaskType,
                shadowFlatBorder,
                shadowFlatBlur,
                lilShadowCasterBias,

                useRimShade,
                rimShadeColor,
                rimShadeMask,
                rimShadeNormalStrength,
                rimShadeBorder,
                rimShadeBlur,
                rimShadeFresnelPower,

                useEmission,
                emissionColor,
                emissionMap,
                emissionMap_ScrollRotate,
                emissionMap_UVMode,
                emissionMainStrength,
                emissionBlend,
                emissionBlendMask,
                emissionBlendMask_ScrollRotate,
                emissionBlendMode,
                emissionBlink,
                emissionUseGrad,
                emissionGradTex,
                emissionGradSpeed,
                emissionParallaxDepth,
                emissionFluorescence,

                useEmission2nd,
                emission2ndColor,
                emission2ndMap,
                emission2ndMap_ScrollRotate,
                emission2ndMap_UVMode,
                emission2ndMainStrength,
                emission2ndBlend,
                emission2ndBlendMask,
                emission2ndBlendMask_ScrollRotate,
                emission2ndBlendMode,
                emission2ndBlink,
                emission2ndUseGrad,
                emission2ndGradTex,
                emission2ndGradSpeed,
                emission2ndParallaxDepth,
                emission2ndFluorescence,

                useBumpMap,
                bumpMap,
                bumpScale,

                useBump2ndMap,
                bump2ndMap,
                bump2ndMap_UVMode,
                bump2ndScale,
                bump2ndScaleMask,

                useAnisotropy,
                anisotropyTangentMap,
                anisotropyScale,
                anisotropyScaleMask,
                anisotropyTangentWidth,
                anisotropyBitangentWidth,
                anisotropyShift,
                anisotropyShiftNoiseScale,
                anisotropySpecularStrength,
                anisotropy2ndTangentWidth,
                anisotropy2ndBitangentWidth,
                anisotropy2ndShift,
                anisotropy2ndShiftNoiseScale,
                anisotropy2ndSpecularStrength,
                anisotropyShiftNoiseMask,
                anisotropy2Reflection,
                anisotropy2MatCap,
                anisotropy2MatCap2nd,

                useBacklight,
                backlightColor,
                backlightColorTex,
                backlightMainStrength,
                backlightNormalStrength,
                backlightBorder,
                backlightBlur,
                backlightDirectivity,
                backlightViewStrength,
                backlightReceiveShadow,
                backlightBackfaceMask,

                useReflection,
                metallic,
                metallicGlossMap,
                smoothness,
                smoothnessTex,
                reflectance,
                reflectionColor,
                reflectionColorTex,
                gsaaStrength,
                applySpecular,
                applySpecularFA,
                specularNormalStrength,
                specularToon,
                specularBorder,
                specularBlur,
                applyReflection,
                reflectionNormalStrength,
                reflectionApplyTransparency,
                reflectionCubeTex,
                reflectionCubeColor,
                reflectionCubeOverride,
                reflectionCubeEnableLighting,
                reflectionBlendMode,

                useMatCap,
                matcapTex,
                matcapColor,
                matcapMainStrength,
                matcapBlendUV1,
                matcapZRotCancel,
                matcapPerspective,
                matcapVRParallaxStrength,
                matcapBlend,
                matcapBlendMask,
                matcapEnableLighting,
                matcapShadowMask,
                matcapBackfaceMask,
                matcapLod,
                matcapBlendMode,
                matcapApplyTransparency,
                matcapNormalStrength,
                matcapCustomNormal,
                matcapBumpMap,
                matcapBumpScale,

                useMatCap2nd,
                matcap2ndTex,
                matcap2ndColor,
                matcap2ndMainStrength,
                matcap2ndBlendUV1,
                matcap2ndZRotCancel,
                matcap2ndPerspective,
                matcap2ndVRParallaxStrength,
                matcap2ndBlend,
                matcap2ndBlendMask,
                matcap2ndEnableLighting,
                matcap2ndShadowMask,
                matcap2ndBackfaceMask,
                matcap2ndLod,
                matcap2ndBlendMode,
                matcap2ndApplyTransparency,
                matcap2ndNormalStrength,
                matcap2ndCustomNormal,
                matcap2ndBumpMap,
                matcap2ndBumpScale,

                useRim,
                rimColor,
                rimColorTex,
                rimMainStrength,
                rimNormalStrength,
                rimBorder,
                rimBlur,
                rimFresnelPower,
                rimEnableLighting,
                rimShadowMask,
                rimBackfaceMask,
                rimVRParallaxStrength,
                rimApplyTransparency,
                rimDirStrength,
                rimDirRange,
                rimIndirRange,
                rimIndirColor,
                rimIndirBorder,
                rimIndirBlur,
                rimBlendMode,

                useGlitter,
                glitterUVMode,
                glitterColor,
                glitterColorTex,
                glitterColorTex_UVMode,
                glitterMainStrength,
                glitterScaleRandomize,
                glitterApplyShape,
                glitterShapeTex,
                glitterAtras,
                glitterAngleRandomize,
                glitterParams1,
                glitterParams2,
                glitterPostContrast,
                glitterSensitivity,
                glitterEnableLighting,
                glitterShadowMask,
                glitterBackfaceMask,
                glitterApplyTransparency,
                glitterVRParallaxStrength,
                glitterNormalStrength,

                gemChromaticAberration,
                gemEnvContrast,
                gemEnvColor,
                gemParticleLoop,
                gemParticleColor,
                gemVRParallaxStrength,

                outlineColor,
                outlineTex,
                outlineTex_ScrollRotate,
                outlineTexHSVG,
                outlineLitColor,
                outlineLitApplyTex,
                outlineLitScale,
                outlineLitOffset,
                outlineLitShadowReceive,
                outlineWidth,
                outlineWidthMask,
                outlineFixWidth,
                outlineVertexR2Width,
                outlineDeleteMesh,
                outlineVectorTex,
                outlineVectorUVMode,
                outlineVectorScale,
                outlineEnableLighting,
                outlineZBias,
                outlineDisableInVR,

                useParallax,
                usePOM,
                parallaxMap,
                parallax,
                parallaxOffset,

                distanceFade,
                distanceFadeColor,
                distanceFadeMode,
                distanceFadeRimColor,
                distanceFadeRimFresnelPower,

                useAudioLink,
                audioLinkDefaultValue,
                audioLinkUVMode,
                audioLinkUVParams,
                audioLinkStart,
                audioLinkMask,
                audioLinkMask_ScrollRotate,
                audioLinkMask_UVMode,
                audioLink2Main2nd,
                audioLink2Main3rd,
                audioLink2Emission,
                audioLink2EmissionGrad,
                audioLink2Emission2nd,
                audioLink2Emission2ndGrad,
                audioLink2Vertex,
                audioLinkVertexUVMode,
                audioLinkVertexUVParams,
                audioLinkVertexStart,
                audioLinkVertexStrength,
                audioLinkAsLocal,
                audioLinkLocalMap,
                audioLinkLocalMapParams,

                dissolveMask,
                dissolveNoiseMask,
                dissolveNoiseMask_ScrollRotate,
                dissolveNoiseStrength,
                dissolveColor,
                dissolveParams,
                dissolvePos,

                idMaskCompile,
                idMaskFrom,
                idMaskIsBitmap,
                idMask1,
                idMask2,
                idMask3,
                idMask4,
                idMask5,
                idMask6,
                idMask7,
                idMask8,
                idMaskIndex1,
                idMaskIndex2,
                idMaskIndex3,
                idMaskIndex4,
                idMaskIndex5,
                idMaskIndex6,
                idMaskIndex7,
                idMaskIndex8,
                idMaskControlsDissolve,
                idMaskPrior1,
                idMaskPrior2,
                idMaskPrior3,
                idMaskPrior4,
                idMaskPrior5,
                idMaskPrior6,
                idMaskPrior7,
                idMaskPrior8,

                udimDiscardCompile,
                udimDiscardUV,
                udimDiscardMethod,
                udimDiscardRow3_0,
                udimDiscardRow3_1,
                udimDiscardRow3_2,
                udimDiscardRow3_3,
                udimDiscardRow2_0,
                udimDiscardRow2_1,
                udimDiscardRow2_2,
                udimDiscardRow2_3,
                udimDiscardRow1_0,
                udimDiscardRow1_1,
                udimDiscardRow1_2,
                udimDiscardRow1_3,
                udimDiscardRow0_0,
                udimDiscardRow0_1,
                udimDiscardRow0_2,
                udimDiscardRow0_3,

                refractionStrength,
                refractionFresnelPower,
                refractionColorFromMain,
                refractionColor,

                furNoiseMask,
                furMask,
                furLengthMask,
                furVectorTex,
                furVectorScale,
                furVector,
                furGravity,
                furRandomize,
                furAO,
                vertexColor2FurVector,
                furLayerNum,
                furRootOffset,
                furCutoutLength,
                furTouchStrength,
                furRimColor,
                furRimFresnelPower,
                furRimAntiLight,

                stencilRef,
                stencilReadMask,
                stencilWriteMask,
                stencilComp,
                stencilPass,
                stencilFail,
                stencilZFail,
                preStencilRef,
                preStencilReadMask,
                preStencilWriteMask,
                preStencilComp,
                preStencilPass,
                preStencilFail,
                preStencilZFail,
                outlineStencilRef,
                outlineStencilReadMask,
                outlineStencilWriteMask,
                outlineStencilComp,
                outlineStencilPass,
                outlineStencilFail,
                outlineStencilZFail,
                furStencilRef,
                furStencilReadMask,
                furStencilWriteMask,
                furStencilComp,
                furStencilPass,
                furStencilFail,
                furStencilZFail,

                subpassCutoff,
                cull,
                srcBlend,
                dstBlend,
                srcBlendAlpha,
                dstBlendAlpha,
                blendOp,
                blendOpAlpha,
                srcBlendFA,
                dstBlendFA,
                srcBlendAlphaFA,
                dstBlendAlphaFA,
                blendOpFA,
                blendOpAlphaFA,
                zclip,
                zwrite,
                ztest,
                offsetFactor,
                offsetUnits,
                colorMask,
                alphaToMask,

                preCull,
                preSrcBlend,
                preDstBlend,
                preSrcBlendAlpha,
                preDstBlendAlpha,
                preBlendOp,
                preBlendOpAlpha,
                preSrcBlendFA,
                preDstBlendFA,
                preSrcBlendAlphaFA,
                preDstBlendAlphaFA,
                preBlendOpFA,
                preBlendOpAlphaFA,
                preZclip,
                preZwrite,
                preZtest,
                preOffsetFactor,
                preOffsetUnits,
                preColorMask,
                preAlphaToMask,

                outlineCull,
                outlineSrcBlend,
                outlineDstBlend,
                outlineSrcBlendAlpha,
                outlineDstBlendAlpha,
                outlineBlendOp,
                outlineBlendOpAlpha,
                outlineSrcBlendFA,
                outlineDstBlendFA,
                outlineSrcBlendAlphaFA,
                outlineDstBlendAlphaFA,
                outlineBlendOpFA,
                outlineBlendOpAlphaFA,
                outlineZclip,
                outlineZwrite,
                outlineZtest,
                outlineOffsetFactor,
                outlineOffsetUnits,
                outlineColorMask,
                outlineAlphaToMask,

                furCull,
                furSrcBlend,
                furDstBlend,
                furSrcBlendAlpha,
                furDstBlendAlpha,
                furBlendOp,
                furBlendOpAlpha,
                furSrcBlendFA,
                furDstBlendFA,
                furSrcBlendAlphaFA,
                furDstBlendAlphaFA,
                furBlendOpFA,
                furBlendOpAlphaFA,
                furZclip,
                furZwrite,
                furZtest,
                furOffsetFactor,
                furOffsetUnits,
                furColorMask,
                furAlphaToMask,

                tessEdge,
                tessStrength,
                tessShrink,
                tessFactorMax,

                transparentModeMat,
                useClippingCanceller,
                asOverlay,
                triMask,
                matcapMul,
                fakeShadowVector,

                ramp,
            };
        }

        private void SetProperties(MaterialProperty[] propsSource)
        {
            var allProps = AllProperties();
#if UNITY_2022_3_OR_NEWER
            var dictonary = UnityEngine.Pool.DictionaryPool<string, MaterialProperty>.Get();
#else
            var dictonary = new Dictionary<string,MaterialProperty>();
#endif
            for (var i = 0; propsSource.Length > i; i += 1)
            {
                var p = propsSource[i];
                if (p == null) { continue; }
                dictonary[p.name] = p;
            }

            for (var i = 0; allProps.Length > i; i += 1)
            {
                var lilPorp = allProps[i];
                if (dictonary.TryGetValue(lilPorp.propertyName, out var materialProperty))
                    lilPorp.p = materialProperty;
            }

#if UNITY_2022_3_OR_NEWER
            UnityEngine.Pool.DictionaryPool<string, MaterialProperty>.Release(dictonary);
#endif
        }

        private Dictionary<PropertyBlock, List<lilMaterialProperty>> block2Propertes;
        private Dictionary<PropertyBlock, List<lilMaterialProperty>> GetBlock2Properties()
        {
            if (block2Propertes is null)
            {
                block2Propertes = new Dictionary<PropertyBlock, List<lilMaterialProperty>>();
                var allProps = AllProperties();

                for (var i = 0; allProps.Length > i; i += 1)
                {
                    var lilPorp = allProps[i];
                    foreach(var block in lilPorp.blocks)
                    {
                        if(!block2Propertes.ContainsKey(block)) { block2Propertes[block] = new List<lilMaterialProperty>(); }
                        block2Propertes[block].Add(lilPorp);
                    }
                }
            }
            return block2Propertes;
        }


    }
}
#endif
