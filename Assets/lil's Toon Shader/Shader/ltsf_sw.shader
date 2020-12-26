// lil's Toon Shader
// Copyright (c) 2020 lilxyzw
// This code is under MIT licence, see LICENSE
// https://github.com/lilxyzw/lil-s-Toon-Shader/blob/main/LICENSE
Shader "Hidden/_lil/StencilWriter/lilToonFullStencilWriter"
{
    Properties
    {
        //----------------------------------------------------------------------------------------------------------------------
        // Base
        [lilToggle] _Invisible ("Invisible", Int) = 0
        [Enum(RenderingMode)] _Mode ("Rendering Mode", Int) = 0
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("Cull Mode", Int) = 2
        [lilToggle] _FlipNormal ("Flip Backface Normal", Int) = 0
        _BackfaceForceShadow ("Backface Force Shadow", Range(0,1)) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Main
        _Color("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _MainTexScrollX ("ScrollX", Float) = 0
        _MainTexScrollY ("ScrollY", Float) = 0
        _MainTexAngle ("Angle", Float) = 0
        _MainTexRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _MainTexUV ("UV Set", Int) = 0
        [lilToggle] _MainTexTrim ("Trim", Int) = 0
        _MainTexTonecurve ("Tonecurve", Range(0.01,2)) = 1
        _MainTexHue ("Hue", Range(-0.5,0.5)) = 0
        _MainTexSaturation ("Saturation", Range(0,2)) = 1
        _MainTexValue ("Value", Range(0,2)) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // Main2nd
        [lilToggle] _UseMain2ndTex ("UseMain2nd", Int) = 0
        _Color2nd("Color", Color) = (1,1,1,1)
        _Main2ndTex ("Texture", 2D) = "white" {}
        _Main2ndTexScrollX ("ScrollX", Float) = 0
        _Main2ndTexScrollY ("ScrollY", Float) = 0
        _Main2ndTexAngle ("Angle", Float) = 0
        _Main2ndTexRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Main2ndTexUV ("UV Set", Int) = 0
        [lilToggle] _Main2ndTexTrim ("Trim", Int) = 0
        _Main2ndBlend ("Strength", Range(0,1)) = 1
         _Main2ndBlendMask ("BlendMask", 2D) = "white" {}
        _Main2ndBlendMaskScrollX ("ScrollX", Float) = 0
        _Main2ndBlendMaskScrollY ("ScrollY", Float) = 0
        _Main2ndBlendMaskAngle ("Angle", Float) = 0
        _Main2ndBlendMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Main2ndBlendMaskUV ("UV Set", Int) = 0
        [lilToggle] _Main2ndBlendMaskTrim ("Trim", Int) = 0
        [Enum(MixMode)] _Main2ndTexMix ("Mix", Int) = 0
        _Main2ndTexHue ("Hue", Range(-0.5,0.5)) = 0
        _Main2ndTexSaturation ("Saturation", Range(0,2)) = 1
        _Main2ndTexValue ("Value", Range(0,2)) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // Main3rd
        [lilToggle] _UseMain3rdTex ("UseMain3rd", Int) = 0
        _Color3rd("Color", Color) = (1,1,1,1)
        _Main3rdTex ("Texture", 2D) = "white" {}
        _Main3rdTexScrollX ("ScrollX", Float) = 0
        _Main3rdTexScrollY ("ScrollY", Float) = 0
        _Main3rdTexAngle ("Angle", Float) = 0
        _Main3rdTexRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Main3rdTexUV ("UV Set", Int) = 0
        [lilToggle] _Main3rdTexTrim ("Trim", Int) = 0
        _Main3rdBlend ("Strength", Range(0,1)) = 1
         _Main3rdBlendMask ("BlendMask", 2D) = "white" {}
        _Main3rdBlendMaskScrollX ("ScrollX", Float) = 0
        _Main3rdBlendMaskScrollY ("ScrollY", Float) = 0
        _Main3rdBlendMaskAngle ("Angle", Float) = 0
        _Main3rdBlendMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Main3rdBlendMaskUV ("UV Set", Int) = 0
        [lilToggle] _Main3rdBlendMaskTrim ("Trim", Int) = 0
        [Enum(MixMode)] _Main3rdTexMix ("Mix", Int) = 0
        _Main3rdTexHue ("Hue", Range(-0.5,0.5)) = 0
        _Main3rdTexSaturation ("Saturation", Range(0,2)) = 1
        _Main3rdTexValue ("Value", Range(0,2)) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // Main4th
        [lilToggle] _UseMain4thTex ("UseMain4th", Int) = 0
        _Color4th("Color", Color) = (1,1,1,1)
        _Main4thTex ("Texture", 2D) = "white" {}
        _Main4thTexScrollX ("ScrollX", Float) = 0
        _Main4thTexScrollY ("ScrollY", Float) = 0
        _Main4thTexAngle ("Angle", Float) = 0
        _Main4thTexRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Main4thTexUV ("UV Set", Int) = 0
        [lilToggle] _Main4thTexTrim ("Trim", Int) = 0
        _Main4thBlend ("Strength", Range(0,1)) = 1
         _Main4thBlendMask ("BlendMask", 2D) = "white" {}
        _Main4thBlendMaskScrollX ("ScrollX", Float) = 0
        _Main4thBlendMaskScrollY ("ScrollY", Float) = 0
        _Main4thBlendMaskAngle ("Angle", Float) = 0
        _Main4thBlendMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Main4thBlendMaskUV ("UV Set", Int) = 0
        [lilToggle] _Main4thBlendMaskTrim ("Trim", Int) = 0
        [Enum(MixMode)] _Main4thTexMix ("Mix", Int) = 0
        _Main4thTexHue ("Hue", Range(-0.5,0.5)) = 0
        _Main4thTexSaturation ("Saturation", Range(0,2)) = 1
        _Main4thTexValue ("Value", Range(0,2)) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // AlphaMask
        [lilToggle] _UseAlphaMask ("UseAlphaMask", Int) = 0
        _Alpha ("Alpha", Range(0,1)) = 1
         _AlphaMask ("AlphaMask", 2D) = "white" {}
        _AlphaMaskScrollX ("ScrollX", Float) = 0
        _AlphaMaskScrollY ("ScrollY", Float) = 0
        _AlphaMaskAngle ("Angle", Float) = 0
        _AlphaMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _AlphaMaskUV ("UV Set", Int) = 0
        [lilToggle] _AlphaMaskTrim ("Trim", Int) = 0
        [lilToggle] _AlphaMaskMixMain ("MixMainAlpha", Int) = 0

        /* アルファマスク追加分
        //----------------------------------------------------------------------------------------------------------------------
        // AlphaMask2nd
        [lilToggle] _UseAlphaMask2nd ("UseAlphaMask2nd", Int) = 0
        _Alpha2nd ("Alpha", Range(0,1)) = 1
         _AlphaMask2nd ("AlphaMask", 2D) = "white" {}
        _AlphaMask2ndScrollX ("ScrollX", Float) = 0
        _AlphaMask2ndScrollY ("ScrollY", Float) = 0
        _AlphaMask2ndAngle ("Angle", Float) = 0
        _AlphaMask2ndRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _AlphaMask2ndUV ("UV Set", Int) = 0
        [lilToggle] _AlphaMask2ndTrim ("Trim", Int) = 0
        [lilToggle] _AlphaMask2ndMixMain ("MixMainAlpha", Int) = 1
        */

        //----------------------------------------------------------------------------------------------------------------------
        // NormalMap
        [lilToggle] _UseBumpMap ("UseNormalMap", Int) = 0
        [Normal]_BumpMap ("Normal map", 2D) = "bump" {}
        _BumpMapScrollX ("ScrollX", Float) = 0
        _BumpMapScrollY ("ScrollY", Float) = 0
        _BumpMapAngle ("Angle", Float) = 0
        _BumpMapRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _BumpMapUV ("UV Set", Int) = 0
        [lilToggle] _BumpMapTrim ("Trim", Int) = 0
        _BumpScale ("Normal scale", Range(-10,10)) = 1
         _BumpScaleMask ("ScaleMask", 2D) = "white" {}
        _BumpScaleMaskScrollX ("ScrollX", Float) = 0
        _BumpScaleMaskScrollY ("ScrollY", Float) = 0
        _BumpScaleMaskAngle ("Angle", Float) = 0
        _BumpScaleMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _BumpScaleMaskUV ("UV Set", Int) = 0
        [lilToggle] _BumpScaleMaskTrim ("Trim", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // NormalMap 2nd
        [lilToggle] _UseBump2ndMap ("UseNormalMap2nd", Int) = 0
        [Normal]_Bump2ndMap ("Normal map", 2D) = "bump" {}
        _Bump2ndMapScrollX ("ScrollX", Float) = 0
        _Bump2ndMapScrollY ("ScrollY", Float) = 0
        _Bump2ndMapAngle ("Angle", Float) = 0
        _Bump2ndMapRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Bump2ndMapUV ("UV Set", Int) = 0
        [lilToggle] _Bump2ndMapTrim ("Trim", Int) = 0
        _Bump2ndScale ("Normal scale", Range(-10,10)) = 1
         _Bump2ndScaleMask ("ScaleMask", 2D) = "white" {}
        _Bump2ndScaleMaskScrollX ("ScrollX", Float) = 0
        _Bump2ndScaleMaskScrollY ("ScrollY", Float) = 0
        _Bump2ndScaleMaskAngle ("Angle", Float) = 0
        _Bump2ndScaleMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Bump2ndScaleMaskUV ("UV Set", Int) = 0
        [lilToggle] _Bump2ndScaleMaskTrim ("Trim", Int) = 0

        /* ノーマルマップ追加分
        //----------------------------------------------------------------------------------------------------------------------
        // NormalMap 3rd
        [lilToggle] _UseBump3rdMap ("UseNormalMap3rd", Int) = 0
        [Normal]_Bump3rdMap ("Normal map", 2D) = "bump" {}
        _Bump3rdMapScrollX ("ScrollX", Float) = 0
        _Bump3rdMapScrollY ("ScrollY", Float) = 0
        _Bump3rdMapAngle ("Angle", Float) = 0
        _Bump3rdMapRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Bump3rdMapUV ("UV Set", Int) = 0
        [lilToggle] _Bump3rdMapTrim ("Trim", Int) = 0
        _Bump3rdScale ("Normal scale", Range(-10,10)) = 1
         _Bump3rdScaleMask ("ScaleMask", 2D) = "white" {}
        _Bump3rdScaleMaskScrollX ("ScrollX", Float) = 0
        _Bump3rdScaleMaskScrollY ("ScrollY", Float) = 0
        _Bump3rdScaleMaskAngle ("Angle", Float) = 0
        _Bump3rdScaleMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Bump3rdScaleMaskUV ("UV Set", Int) = 0
        [lilToggle] _Bump3rdScaleMaskTrim ("Trim", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // NormalMap 4th
        [lilToggle] _UseBump4thMap ("UseNormalMap4th", Int) = 0
        [Normal]_Bump4thMap ("Normal map", 2D) = "bump" {}
        _Bump4thMapScrollX ("ScrollX", Float) = 0
        _Bump4thMapScrollY ("ScrollY", Float) = 0
        _Bump4thMapAngle ("Angle", Float) = 0
        _Bump4thMapRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Bump4thMapUV ("UV Set", Int) = 0
        [lilToggle] _Bump4thMapTrim ("Trim", Int) = 0
        _Bump4thScale ("Normal scale", Range(-10,10)) = 1
         _Bump4thScaleMask ("ScaleMask", 2D) = "white" {}
        _Bump4thScaleMaskScrollX ("ScrollX", Float) = 0
        _Bump4thScaleMaskScrollY ("ScrollY", Float) = 0
        _Bump4thScaleMaskAngle ("Angle", Float) = 0
        _Bump4thScaleMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Bump4thScaleMaskUV ("UV Set", Int) = 0
        [lilToggle] _Bump4thScaleMaskTrim ("Trim", Int) = 0
        */

        //----------------------------------------------------------------------------------------------------------------------
        // Shadow
        [lilToggle] _UseShadow ("UseShadow", Int) = 0
        _ShadowBorder ("Border", Range(0, 1)) = 0.65
         _ShadowBorderMask ("BorderMask", 2D) = "white" {}
        _ShadowBorderMaskScrollX ("ScrollX", Float) = 0
        _ShadowBorderMaskScrollY ("ScrollY", Float) = 0
        _ShadowBorderMaskAngle ("Angle", Float) = 0
        _ShadowBorderMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _ShadowBorderMaskUV ("UV Set", Int) = 0
        [lilToggle] _ShadowBorderMaskTrim ("Trim", Int) = 0
        _ShadowBlur ("Blur", Range(0, 1)) = 0.01
         _ShadowBlurMask ("BlurMask", 2D) = "white" {}
        _ShadowBlurMaskScrollX ("ScrollX", Float) = 0
        _ShadowBlurMaskScrollY ("ScrollY", Float) = 0
        _ShadowBlurMaskAngle ("Angle", Float) = 0
        _ShadowBlurMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _ShadowBlurMaskUV ("UV Set", Int) = 0
        [lilToggle] _ShadowBlurMaskTrim ("Trim", Int) = 0
        _ShadowStrength ("Strength", Range(0, 1)) = 0.5
         _ShadowStrengthMask ("StrengthMask", 2D) = "white" {}
        _ShadowStrengthMaskScrollX ("ScrollX", Float) = 0
        _ShadowStrengthMaskScrollY ("ScrollY", Float) = 0
        _ShadowStrengthMaskAngle ("Angle", Float) = 0
        _ShadowStrengthMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _ShadowStrengthMaskUV ("UV Set", Int) = 0
        [lilToggle] _ShadowStrengthMaskTrim ("Trim", Int) = 0
        [lilToggle] _UseShadowMixMainColor ("UseShadowMixMainColor", Int) = 1
        _ShadowMixMainColor ("Mix Color", Range(0, 10)) = 2
        _ShadowGrad ("ShadowGradation", Range(0, 1)) = 1
        _ShadowGradColor("ShadowGradColor", Color) = (1,0,0,1)
        _ShadowHue ("Hue", Range(-0.5,0.5)) = -0.1
        _ShadowSaturation ("Saturation", Range(0,2)) = 1
        _ShadowValue ("Value", Range(0,2)) = 1
        [lilToggle] _UseShadowColor ("UseShadowColor", Int) = 0
        _ShadowColorFromMain ("ShadowColorFromMain", Range(0, 1)) = 0
        _ShadowColor("ShadowColor", Color) = (1,1,1,1)
        _ShadowColorTex ("ShadowColorTex", 2D) = "white" {}
        _ShadowColorTexScrollX ("ScrollX", Float) = 0
        _ShadowColorTexScrollY ("ScrollY", Float) = 0
        _ShadowColorTexAngle ("Angle", Float) = 0
        _ShadowColorTexRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _ShadowColorTexUV ("UV Set", Int) = 0
        [lilToggle] _ShadowColorTexTrim ("Trim", Int) = 0
        [Enum(MixMode)] _ShadowColorMix ("Mix", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Shadow 2nd
        [lilToggle] _UseShadow2nd ("UseShadow2nd", Int) = 0
        _Shadow2ndBorder ("Border", Range(0, 1)) = 0.5
        _Shadow2ndBlur ("Blur", Range(0, 1)) = 0.3
        _Shadow2ndColorFromMain ("Shadow2ndColorFromMain", Range(0, 1)) = 0
        _Shadow2ndColor("Shadow2ndColor", Color) = (0.75,0.85,1,1)
        [NoScaleOffset] _Shadow2ndColorTex ("Shadow2ndColorTex", 2D) = "white" {}
        _Shadow2ndColorTexScrollX ("ScrollX", Float) = 0
        _Shadow2ndColorTexScrollY ("ScrollY", Float) = 0
        _Shadow2ndColorTexAngle ("Angle", Float) = 0
        _Shadow2ndColorTexRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Shadow2ndColorTexUV ("UV Set", Int) = 0
        [lilToggle] _Shadow2ndColorTexTrim ("Trim", Int) = 0
        [Enum(MixMode)] _Shadow2ndColorMix ("Mix", Int) = 3

        //----------------------------------------------------------------------------------------------------------------------
        // Default Shading
        [lilToggle]_UseDefaultShading ("Use Specular", Int) = 0
        _DefaultShadingBlend ("Blend Default Shading", Range(0, 1)) = 0
         _DefaultShadingBlendMask ("BlendMask", 2D) = "white" {}
        _DefaultShadingBlendMaskScrollX ("ScrollX", Float) = 0
        _DefaultShadingBlendMaskScrollY ("ScrollY", Float) = 0
        _DefaultShadingBlendMaskAngle ("Angle", Float) = 0
        _DefaultShadingBlendMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _DefaultShadingBlendMaskUV ("UV Set", Int) = 0
        [lilToggle] _DefaultShadingBlendMaskTrim ("Trim", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Outline
        [lilToggle] _UseOutline ("UseOutline", Int) = 0
        [lilToggle] _OutlineMixMain ("MixMainColor", Int) = 1
        _OutlineMixMainStrength ("Strength", Range(0,10)) = 2.0
        _OutlineHue ("Hue", Range(-0.5,0.5)) = 0.0
        _OutlineSaturation ("Saturation", Range(0,2)) = 1.75
        _OutlineValue ("Value", Range(0,2)) = 0.9
        _OutlineAutoHue ("AutoHue", Range(0,1)) = 0.35
        _OutlineAutoValue ("AutoValue", Range(0,1)) = 0.25
        [lilToggle] _UseOutlineColor ("UseOutlineColor", Int) = 0
        _OutlineColor("Color", Color) = (1,1,1,1)
        _OutlineColorTex ("Tex", 2D) = "white" {}
        _OutlineColorTexScrollX ("ScrollX", Float) = 0
        _OutlineColorTexScrollY ("ScrollY", Float) = 0
        _OutlineColorTexAngle ("Angle", Float) = 0
        _OutlineColorTexRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _OutlineColorTexUV ("UV Set", Int) = 0
        [lilToggle] _OutlineColorTexTrim ("Trim", Int) = 0
        _OutlineWidth ("Width", Range(0,10)) = 1
         _OutlineWidthMask ("WidthMask", 2D) = "white" {}
        _OutlineWidthMaskScrollX ("ScrollX", Float) = 0
        _OutlineWidthMaskScrollY ("ScrollY", Float) = 0
        _OutlineWidthMaskAngle ("Angle", Float) = 0
        _OutlineWidthMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _OutlineWidthMaskUV ("UV Set", Int) = 0
        [lilToggle] _OutlineWidthMaskTrim ("Trim", Int) = 0
        _OutlineAlpha ("Width", Range(0,1)) = 1
         _OutlineAlphaMask ("AlphaMask", 2D) = "white" {}
        _OutlineAlphaMaskScrollX ("ScrollX", Float) = 0
        _OutlineAlphaMaskScrollY ("ScrollY", Float) = 0
        _OutlineAlphaMaskAngle ("Angle", Float) = 0
        _OutlineAlphaMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _OutlineAlphaMaskUV ("UV Set", Int) = 0
        [lilToggle] _OutlineAlphaMaskTrim ("Trim", Int) = 0
        //こっちはできない
        //[Enum(None,-1,R,0,G,1,B,2,A,3)] _VertexColor2Width ("VertexColor2Width", Int) = -1
        [Enum(None,0,R,1,G,2,B,3,A,4)] _VertexColor2Width ("VertexColor2Width", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Reflection
        [lilToggle]_UseReflection ("Use Reflection", Int) = 0
        // Smoothness
        _Smoothness ("Smoothness", Range(0, 1)) = 1
         _SmoothnessTex ("Smoothness Mask", 2D) = "white" {}
        _SmoothnessTexScrollX ("ScrollX", Float) = 0
        _SmoothnessTexScrollY ("ScrollY", Float) = 0
        _SmoothnessTexAngle ("Angle", Float) = 0
        _SmoothnessTexRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _SmoothnessTexUV ("UV Set", Int) = 0
        [lilToggle] _SmoothnessTexTrim ("Trim", Int) = 0
        // Metallic
        [Gamma] _Metallic ("Metallic", Range(0, 1)) = 0
         _MetallicGlossMap ("Metallic", 2D) = "white" {}
        _MetallicGlossMapScrollX ("ScrollX", Float) = 0
        _MetallicGlossMapScrollY ("ScrollY", Float) = 0
        _MetallicGlossMapAngle ("Angle", Float) = 0
        _MetallicGlossMapRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _MetallicGlossMapUV ("UV Set", Int) = 0
        [lilToggle] _MetallicGlossMapTrim ("Trim", Int) = 0
        // Reflection
        [lilToggle]_ApplySpecular ("Apply Specular", Int) = 1
        [lilToggle]_ApplyReflection ("Apply Reflection", Int) = 0
        [lilToggle]_ReflectionUseCubemap ("Use Cubemap Texture", Int) = 0
        _ReflectionCubemap ("Cubemap", Cube) = "" {}
        _ReflectionBlend ("Blend", Range(0, 1)) = 1
         _ReflectionBlendMask ("BlendMask", 2D) = "white" {}
        _ReflectionBlendMaskScrollX ("ScrollX", Float) = 0
        _ReflectionBlendMaskScrollY ("ScrollY", Float) = 0
        _ReflectionBlendMaskAngle ("Angle", Float) = 0
        _ReflectionBlendMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _ReflectionBlendMaskUV ("UV Set", Int) = 0
        [lilToggle] _ReflectionBlendMaskTrim ("Trim", Int) = 0
        _ReflectionShadeMix ("Shade Mix", Range(0, 1)) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Matcap
        [lilToggle] _UseMatcap ("UseMatcap", Int) = 0
        _MatcapColor ("Color", Color) = (1,1,1,1)
        _MatcapTex ("Tex", 2D) = "black" {}
        _MatcapBlend ("Blend", Range(0, 3)) = 1
         _MatcapBlendMask ("BlendMask", 2D) = "white" {}
        _MatcapBlendMaskScrollX ("ScrollX", Float) = 0
        _MatcapBlendMaskScrollY ("ScrollY", Float) = 0
        _MatcapBlendMaskAngle ("Angle", Float) = 0
        _MatcapBlendMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _MatcapBlendMaskUV ("UV Set", Int) = 0
        [lilToggle] _MatcapBlendMaskTrim ("Trim", Int) = 0
        _MatcapNormalMix ("NormalMap Mix", Range(0, 2)) = 1
        _MatcapShadeMix ("Shade Mix", Range(0, 1)) = 1
        [Enum(MixMode)] _MatcapMix ("Mix", Int) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // Matcap 2nd
        [lilToggle] _UseMatcap2nd ("UseMatcap2nd", Int) = 0
        _Matcap2ndColor ("Color", Color) = (1,1,1,1)
        _Matcap2ndTex ("Tex", 2D) = "black" {}
        _Matcap2ndBlend ("Blend", Range(0, 3)) = 1
         _Matcap2ndBlendMask ("BlendMask", 2D) = "white" {}
        _Matcap2ndBlendMaskScrollX ("ScrollX", Float) = 0
        _Matcap2ndBlendMaskScrollY ("ScrollY", Float) = 0
        _Matcap2ndBlendMaskAngle ("Angle", Float) = 0
        _Matcap2ndBlendMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Matcap2ndBlendMaskUV ("UV Set", Int) = 0
        [lilToggle] _Matcap2ndBlendMaskTrim ("Trim", Int) = 0
        _Matcap2ndNormalMix ("NormalMap Mix", Range(0, 2)) = 1
        _Matcap2ndShadeMix ("Shade Mix", Range(0, 1)) = 1
        [Enum(MixMode)] _Matcap2ndMix ("Mix", Int) = 1

        /* マットキャップ追加分
        //----------------------------------------------------------------------------------------------------------------------
        // Matcap 3rd
        [lilToggle] _UseMatcap3rd ("UseMatcap3rd", Int) = 0
        _Matcap3rdColor ("Color", Color) = (1,1,1,1)
        _Matcap3rdTex ("Tex", 2D) = "black" {}
         _Matcap3rdBlendMask ("BlendMask", 2D) = "white" {}
        _Matcap3rdBlendMask ("BlendMask", 2D) = "white" {}
        _Matcap3rdBlendMaskScrollX ("ScrollX", Float) = 0
        _Matcap3rdBlendMaskScrollY ("ScrollY", Float) = 0
        _Matcap3rdBlendMaskAngle ("Angle", Float) = 0
        _Matcap3rdBlendMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Matcap3rdBlendMaskUV ("UV Set", Int) = 0
        [lilToggle] _Matcap3rdBlendMaskTrim ("Trim", Int) = 0
        _Matcap3rdNormalMix ("NormalMap Mix", Range(0, 2)) = 1
        _Matcap3rdShadeMix ("Shade Mix", Range(0, 1)) = 1
        [Enum(MixMode)] _Matcap3rdMix ("Mix", Int) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // Matcap 4th
        [lilToggle] _UseMatcap4th ("UseMatcap4th", Int) = 0
        _Matcap4thColor ("Color", Color) = (1,1,1,1)
        _Matcap4thTex ("Tex", 2D) = "black" {}
         _Matcap4thBlendMask ("BlendMask", 2D) = "white" {}
        _Matcap4thBlendMask ("BlendMask", 2D) = "white" {}
        _Matcap4thBlendMaskScrollX ("ScrollX", Float) = 0
        _Matcap4thBlendMaskScrollY ("ScrollY", Float) = 0
        _Matcap4thBlendMaskAngle ("Angle", Float) = 0
        _Matcap4thBlendMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Matcap4thBlendMaskUV ("UV Set", Int) = 0
        [lilToggle] _Matcap4thBlendMaskTrim ("Trim", Int) = 0
        _Matcap4thNormalMix ("NormalMap Mix", Range(0, 2)) = 1
        _Matcap4thShadeMix ("Shade Mix", Range(0, 1)) = 1
        [Enum(MixMode)] _Matcap4thMix ("Mix", Int) = 1
        */

        //----------------------------------------------------------------------------------------------------------------------
        // Rim
        [lilToggle] _UseRim ("UseRim", Int) = 0
        _RimColor("Color", Color) = (1,1,1,1)
        _RimColorTex ("ColorTex", 2D) = "white" {}
        _RimColorTexScrollX ("ScrollX", Float) = 0
        _RimColorTexScrollY ("ScrollY", Float) = 0
        _RimColorTexAngle ("Angle", Float) = 0
        _RimColorTexRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _RimColorTexUV ("UV Set", Int) = 0
        [lilToggle] _RimColorTexTrim ("Trim", Int) = 0
        _RimBlend ("Blend", Range(0, 1)) = 0.5
         _RimBlendMask ("BlendMask", 2D) = "white" {}
        _RimBlendMaskScrollX ("ScrollX", Float) = 0
        _RimBlendMaskScrollY ("ScrollY", Float) = 0
        _RimBlendMaskAngle ("Angle", Float) = 0
        _RimBlendMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _RimBlendMaskUV ("UV Set", Int) = 0
        [lilToggle] _RimBlendMaskTrim ("Trim", Int) = 0
        [lilToggle] _RimToon ("Toon", Int) = 0
        _RimBorder ("Border", Range(0, 1)) = 0.5
         _RimBorderMask ("BorderMask", 2D) = "white" {}
        _RimBorderMaskScrollX ("ScrollX", Float) = 0
        _RimBorderMaskScrollY ("ScrollY", Float) = 0
        _RimBorderMaskAngle ("Angle", Float) = 0
        _RimBorderMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _RimBorderMaskUV ("UV Set", Int) = 0
        [lilToggle] _RimBorderMaskTrim ("Trim", Int) = 0
        _RimBlur ("Blur", Range(0, 1)) = 0.0
         _RimBlurMask ("BlurMask", 2D) = "white" {}
        _RimBlurMaskScrollX ("ScrollX", Float) = 0
        _RimBlurMaskScrollY ("ScrollY", Float) = 0
        _RimBlurMaskAngle ("Angle", Float) = 0
        _RimBlurMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _RimBlurMaskUV ("UV Set", Int) = 0
        [lilToggle] _RimBlurMaskTrim ("Trim", Int) = 0
        _RimUpperSideWidth ("Upper Width", Range(0, 1)) = 0
        [PowerSlider(3.0)]_RimFresnelPower ("Fresnel Power", Range(0.01, 50)) = 0.8
        _RimShadeMix ("Apply Shade", Range(0,1)) = 1
        [lilToggle] _RimShadowMask ("Shadow Mask", Int) = 0

        /* リムライト追加分
        //----------------------------------------------------------------------------------------------------------------------
        // Rim 2nd
        [lilToggle] _UseRim2nd ("UseRim2nd", Int) = 0
        _Rim2ndColor("Color", Color) = (1,1,1,1)
        _Rim2ndColorTex ("ColorTex", 2D) = "white" {}
        _Rim2ndColorTexScrollX ("ScrollX", Float) = 0
        _Rim2ndColorTexScrollY ("ScrollY", Float) = 0
        _Rim2ndColorTexAngle ("Angle", Float) = 0
        _Rim2ndColorTexRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Rim2ndColorTexUV ("UV Set", Int) = 0
        [lilToggle] _Rim2ndColorTexTrim ("Trim", Int) = 0
        _Rim2ndBlend ("Blend", Range(0, 1)) = 0.5
         _Rim2ndBlendMask ("BlendMask", 2D) = "white" {}
        _Rim2ndBlendMaskScrollX ("ScrollX", Float) = 0
        _Rim2ndBlendMaskScrollY ("ScrollY", Float) = 0
        _Rim2ndBlendMaskAngle ("Angle", Float) = 0
        _Rim2ndBlendMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Rim2ndBlendMaskUV ("UV Set", Int) = 0
        [lilToggle] _Rim2ndBlendMaskTrim ("Trim", Int) = 0
        [lilToggle] _Rim2ndToon ("Toon", Int) = 0
        _Rim2ndBorder ("Border", Range(0, 1)) = 0.5
         _Rim2ndBorderMask ("BorderMask", 2D) = "white" {}
        _Rim2ndBorderMaskScrollX ("ScrollX", Float) = 0
        _Rim2ndBorderMaskScrollY ("ScrollY", Float) = 0
        _Rim2ndBorderMaskAngle ("Angle", Float) = 0
        _Rim2ndBorderMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Rim2ndBorderMaskUV ("UV Set", Int) = 0
        [lilToggle] _Rim2ndBorderMaskTrim ("Trim", Int) = 0
        _Rim2ndBlur ("Blur", Range(0, 1)) = 0.0
         _Rim2ndBlurMask ("BlurMask", 2D) = "white" {}
        _Rim2ndBlurMaskScrollX ("ScrollX", Float) = 0
        _Rim2ndBlurMaskScrollY ("ScrollY", Float) = 0
        _Rim2ndBlurMaskAngle ("Angle", Float) = 0
        _Rim2ndBlurMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Rim2ndBlurMaskUV ("UV Set", Int) = 0
        [lilToggle] _Rim2ndBlurMaskTrim ("Trim", Int) = 0
        _Rim2ndUpperSideWidth ("Upper Width", Range(0, 1)) = 0
        [PowerSlider(3.0)]_Rim2ndFresnelPower ("Fresnel Power", Range(0.01, 50)) = 0.8
        _Rim2ndShadeMix ("Apply Shade", Range(0,1)) = 1
        [lilToggle] _Rim2ndShadowMask ("Shadow Mask", Int) = 0
        */

        //----------------------------------------------------------------------------------------------------------------------
        // Emmision
        [lilToggle] _UseEmission ("UseEmission", Int) = 0
        [HDR]_EmissionColor("Color", Color) = (1,1,1)
        _EmissionMap ("Texture", 2D) = "white" {}
        _EmissionMapScrollX ("ScrollX", Float) = 0
        _EmissionMapScrollY ("ScrollY", Float) = 0
        _EmissionMapAngle ("Angle", Float) = 0
        _EmissionMapRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _EmissionMapUV ("UV Set", Int) = 0
        [lilToggle] _EmissionMapTrim ("Trim", Int) = 0
        _EmissionBlend ("Blend", Range(0,1)) = 1
         _EmissionBlendMask ("BlendMask", 2D) = "white" {}
        _EmissionBlendMaskScrollX ("ScrollX", Float) = 0
        _EmissionBlendMaskScrollY ("ScrollY", Float) = 0
        _EmissionBlendMaskAngle ("Angle", Float) = 0
        _EmissionBlendMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _EmissionBlendMaskUV ("UV Set", Int) = 0
        [lilToggle] _EmissionBlendMaskTrim ("Trim", Int) = 0
        _EmissionHue ("Hue", Range(-0.5,0.5)) = 0
        _EmissionSaturation ("Saturation", Range(0,2)) = 1
        _EmissionValue ("Value", Range(0,2)) = 1
        [lilToggle] _EmissionUseBlink ("Use Blink", Int) = 0
        _EmissionBlinkStrength ("Blink Strength", Range(0.0,1.0)) = 1
        _EmissionBlinkSpeed ("Blink Speed", Float) = 10
        _EmissionBlinkOffset ("Blink Offset", Float) = 0
        [lilToggle] _EmissionBlinkType ("Blink Type", Int) = 0
        [lilToggle] _EmissionUseGrad ("Use Grad", Int) = 0
        _EmissionGradTex ("Gradation Texture", 2D) = "white" {}
        _EmissionGradSpeed ("Gradation Speed", Float) = 1
        _EmissionParallaxDepth ("Parallax Depth", float) = 0
        // グラデ保存用
        [HideInInspector] _egci ("", Int) = 2
        [HideInInspector] _egai ("", Int) = 2
        [HideInInspector] _egc0 ("", Color) = (1,1,1,0)
        [HideInInspector] _egc1 ("", Color) = (1,1,1,1)
        [HideInInspector] _egc2 ("", Color) = (1,1,1,0)
        [HideInInspector] _egc3 ("", Color) = (1,1,1,0)
        [HideInInspector] _egc4 ("", Color) = (1,1,1,0)
        [HideInInspector] _egc5 ("", Color) = (1,1,1,0)
        [HideInInspector] _egc6 ("", Color) = (1,1,1,0)
        [HideInInspector] _egc7 ("", Color) = (1,1,1,0)
        [HideInInspector] _ega0 ("", Color) = (1,0,0,0)
        [HideInInspector] _ega1 ("", Color) = (1,0,0,1)
        [HideInInspector] _ega2 ("", Color) = (1,0,0,0)
        [HideInInspector] _ega3 ("", Color) = (1,0,0,0)
        [HideInInspector] _ega4 ("", Color) = (1,0,0,0)
        [HideInInspector] _ega5 ("", Color) = (1,0,0,0)
        [HideInInspector] _ega6 ("", Color) = (1,0,0,0)
        [HideInInspector] _ega7 ("", Color) = (1,0,0,0)

        //----------------------------------------------------------------------------------------------------------------------
        // Emmision2nd
        [lilToggle] _UseEmission2nd ("UseEmission", Int) = 0
        [HDR]_Emission2ndColor("Color", Color) = (1,1,1)
        _Emission2ndMap ("Texture", 2D) = "white" {}
        _Emission2ndMapScrollX ("ScrollX", Float) = 0
        _Emission2ndMapScrollY ("ScrollY", Float) = 0
        _Emission2ndMapAngle ("Angle", Float) = 0
        _Emission2ndMapRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Emission2ndMapUV ("UV Set", Int) = 0
        [lilToggle] _Emission2ndMapTrim ("Trim", Int) = 0
        _Emission2ndBlend ("Blend", Range(0,1)) = 1
         _Emission2ndBlendMask ("BlendMask", 2D) = "white" {}
        _Emission2ndBlendMaskScrollX ("ScrollX", Float) = 0
        _Emission2ndBlendMaskScrollY ("ScrollY", Float) = 0
        _Emission2ndBlendMaskAngle ("Angle", Float) = 0
        _Emission2ndBlendMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Emission2ndBlendMaskUV ("UV Set", Int) = 0
        [lilToggle] _Emission2ndBlendMaskTrim ("Trim", Int) = 0
        _Emission2ndHue ("Hue", Range(-0.5,0.5)) = 0
        _Emission2ndSaturation ("Saturation", Range(0,2)) = 1
        _Emission2ndValue ("Value", Range(0,2)) = 1
        [lilToggle] _Emission2ndUseBlink ("Use Blink", Int) = 0
        _Emission2ndBlinkStrength ("Blink Strength", Range(0.0,1.0)) = 1
        _Emission2ndBlinkSpeed ("Blink Speed", Float) = 10
        _Emission2ndBlinkOffset ("Blink Offset", Float) = 0
        [lilToggle] _Emission2ndBlinkType ("Blink Type", Int) = 0
        [lilToggle] _Emission2ndUseGrad ("Use Grad", Int) = 0
        _Emission2ndGradTex ("Gradation Texture", 2D) = "white" {}
        _Emission2ndGradSpeed ("Gradation Speed", Float) = 1
        _Emission2ndParallaxDepth ("Parallax Depth", float) = 0
        // グラデ保存用
        [HideInInspector] _e2gci ("", Int) = 2
        [HideInInspector] _e2gai ("", Int) = 2
        [HideInInspector] _e2gc0 ("", Color) = (1,1,1,0)
        [HideInInspector] _e2gc1 ("", Color) = (1,1,1,1)
        [HideInInspector] _e2gc2 ("", Color) = (1,1,1,0)
        [HideInInspector] _e2gc3 ("", Color) = (1,1,1,0)
        [HideInInspector] _e2gc4 ("", Color) = (1,1,1,0)
        [HideInInspector] _e2gc5 ("", Color) = (1,1,1,0)
        [HideInInspector] _e2gc6 ("", Color) = (1,1,1,0)
        [HideInInspector] _e2gc7 ("", Color) = (1,1,1,0)
        [HideInInspector] _e2ga0 ("", Color) = (1,0,0,0)
        [HideInInspector] _e2ga1 ("", Color) = (1,0,0,1)
        [HideInInspector] _e2ga2 ("", Color) = (1,0,0,0)
        [HideInInspector] _e2ga3 ("", Color) = (1,0,0,0)
        [HideInInspector] _e2ga4 ("", Color) = (1,0,0,0)
        [HideInInspector] _e2ga5 ("", Color) = (1,0,0,0)
        [HideInInspector] _e2ga6 ("", Color) = (1,0,0,0)
        [HideInInspector] _e2ga7 ("", Color) = (1,0,0,0)

        //----------------------------------------------------------------------------------------------------------------------
        // Emmision3rd
        [lilToggle] _UseEmission3rd ("UseEmission", Int) = 0
        [HDR]_Emission3rdColor("Color", Color) = (1,1,1)
        _Emission3rdMap ("Texture", 2D) = "white" {}
        _Emission3rdMapScrollX ("ScrollX", Float) = 0
        _Emission3rdMapScrollY ("ScrollY", Float) = 0
        _Emission3rdMapAngle ("Angle", Float) = 0
        _Emission3rdMapRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Emission3rdMapUV ("UV Set", Int) = 0
        [lilToggle] _Emission3rdMapTrim ("Trim", Int) = 0
        _Emission3rdBlend ("Blend", Range(0,1)) = 1
         _Emission3rdBlendMask ("BlendMask", 2D) = "white" {}
        _Emission3rdBlendMaskScrollX ("ScrollX", Float) = 0
        _Emission3rdBlendMaskScrollY ("ScrollY", Float) = 0
        _Emission3rdBlendMaskAngle ("Angle", Float) = 0
        _Emission3rdBlendMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Emission3rdBlendMaskUV ("UV Set", Int) = 0
        [lilToggle] _Emission3rdBlendMaskTrim ("Trim", Int) = 0
        _Emission3rdHue ("Hue", Range(-0.5,0.5)) = 0
        _Emission3rdSaturation ("Saturation", Range(0,2)) = 1
        _Emission3rdValue ("Value", Range(0,2)) = 1
        [lilToggle] _Emission3rdUseBlink ("Use Blink", Int) = 0
        _Emission3rdBlinkStrength ("Blink Strength", Range(0.0,1.0)) = 1
        _Emission3rdBlinkSpeed ("Blink Speed", Float) = 10
        _Emission3rdBlinkOffset ("Blink Offset", Float) = 0
        [lilToggle] _Emission3rdBlinkType ("Blink Type", Int) = 0
        [lilToggle] _Emission3rdUseGrad ("Use Grad", Int) = 0
        _Emission3rdGradTex ("Gradation Texture", 2D) = "white" {}
        _Emission3rdGradSpeed ("Gradation Speed", Float) = 1
        _Emission3rdParallaxDepth ("Parallax Depth", float) = 0
        // グラデ保存用
        [HideInInspector] _e3gci ("", Int) = 2
        [HideInInspector] _e3gai ("", Int) = 2
        [HideInInspector] _e3gc0 ("", Color) = (1,1,1,0)
        [HideInInspector] _e3gc1 ("", Color) = (1,1,1,1)
        [HideInInspector] _e3gc2 ("", Color) = (1,1,1,0)
        [HideInInspector] _e3gc3 ("", Color) = (1,1,1,0)
        [HideInInspector] _e3gc4 ("", Color) = (1,1,1,0)
        [HideInInspector] _e3gc5 ("", Color) = (1,1,1,0)
        [HideInInspector] _e3gc6 ("", Color) = (1,1,1,0)
        [HideInInspector] _e3gc7 ("", Color) = (1,1,1,0)
        [HideInInspector] _e3ga0 ("", Color) = (1,0,0,0)
        [HideInInspector] _e3ga1 ("", Color) = (1,0,0,1)
        [HideInInspector] _e3ga2 ("", Color) = (1,0,0,0)
        [HideInInspector] _e3ga3 ("", Color) = (1,0,0,0)
        [HideInInspector] _e3ga4 ("", Color) = (1,0,0,0)
        [HideInInspector] _e3ga5 ("", Color) = (1,0,0,0)
        [HideInInspector] _e3ga6 ("", Color) = (1,0,0,0)
        [HideInInspector] _e3ga7 ("", Color) = (1,0,0,0)

        //----------------------------------------------------------------------------------------------------------------------
        // Emmision4th
        [lilToggle] _UseEmission4th ("UseEmission", Int) = 0
        [HDR]_Emission4thColor("Color", Color) = (1,1,1)
        _Emission4thMap ("Texture", 2D) = "white" {}
        _Emission4thMapScrollX ("ScrollX", Float) = 0
        _Emission4thMapScrollY ("ScrollY", Float) = 0
        _Emission4thMapAngle ("Angle", Float) = 0
        _Emission4thMapRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Emission4thMapUV ("UV Set", Int) = 0
        [lilToggle] _Emission4thMapTrim ("Trim", Int) = 0
        _Emission4thBlend ("Blend", Range(0,1)) = 1
         _Emission4thBlendMask ("BlendMask", 2D) = "white" {}
        _Emission4thBlendMaskScrollX ("ScrollX", Float) = 0
        _Emission4thBlendMaskScrollY ("ScrollY", Float) = 0
        _Emission4thBlendMaskAngle ("Angle", Float) = 0
        _Emission4thBlendMaskRotate ("Rotate", Float) = 0
        [Enum(UVSet)] _Emission4thBlendMaskUV ("UV Set", Int) = 0
        [lilToggle] _Emission4thBlendMaskTrim ("Trim", Int) = 0
        _Emission4thHue ("Hue", Range(-0.5,0.5)) = 0
        _Emission4thSaturation ("Saturation", Range(0,2)) = 1
        _Emission4thValue ("Value", Range(0,2)) = 1
        [lilToggle] _Emission4thUseBlink ("Use Blink", Int) = 0
        _Emission4thBlinkStrength ("Blink Strength", Range(0.0,1.0)) = 1
        _Emission4thBlinkSpeed ("Blink Speed", Float) = 10
        _Emission4thBlinkOffset ("Blink Offset", Float) = 0
        [lilToggle] _Emission4thBlinkType ("Blink Type", Int) = 0
        [lilToggle] _Emission4thUseGrad ("Use Grad", Int) = 0
        _Emission4thGradTex ("Gradation Texture", 2D) = "white" {}
        _Emission4thGradSpeed ("Gradation Speed", Float) = 1
        _Emission4thParallaxDepth ("Parallax Depth", float) = 0
        // グラデ保存用
        [HideInInspector] _e4gci ("", Int) = 2
        [HideInInspector] _e4gai ("", Int) = 2
        [HideInInspector] _e4gc0 ("", Color) = (1,1,1,0)
        [HideInInspector] _e4gc1 ("", Color) = (1,1,1,1)
        [HideInInspector] _e4gc2 ("", Color) = (1,1,1,0)
        [HideInInspector] _e4gc3 ("", Color) = (1,1,1,0)
        [HideInInspector] _e4gc4 ("", Color) = (1,1,1,0)
        [HideInInspector] _e4gc5 ("", Color) = (1,1,1,0)
        [HideInInspector] _e4gc6 ("", Color) = (1,1,1,0)
        [HideInInspector] _e4gc7 ("", Color) = (1,1,1,0)
        [HideInInspector] _e4ga0 ("", Color) = (1,0,0,0)
        [HideInInspector] _e4ga1 ("", Color) = (1,0,0,1)
        [HideInInspector] _e4ga2 ("", Color) = (1,0,0,0)
        [HideInInspector] _e4ga3 ("", Color) = (1,0,0,0)
        [HideInInspector] _e4ga4 ("", Color) = (1,0,0,0)
        [HideInInspector] _e4ga5 ("", Color) = (1,0,0,0)
        [HideInInspector] _e4ga6 ("", Color) = (1,0,0,0)
        [HideInInspector] _e4ga7 ("", Color) = (1,0,0,0)

        //----------------------------------------------------------------------------------------------------------------------
        // Advanced
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("SrcBlend", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("DstBlend", Int) = 0
        [lilToggle] _ZWrite ("ZWrite", Int) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Int) = 4
        [lilToggle] _UseVertexColor ("UseVertexColor", Int) = 0
        [IntRange] _StencilRef ("Stencil Reference Value", Range(0, 255)) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Compare Function", Float) = 8
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilPass ("Stencil Pass", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilFail ("Stencil Fail", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilZFail ("Stencil ZFail", Float) = 0
    }
    SubShader
    {
		LOD 0
        Tags {
            "RenderType" = "Opaque"
            "Queue" = "Geometry-1"
        }

        Pass
        {
            Name "FORWARD"
            Tags {"LightMode"="ForwardBase"}

            Stencil
            {
                Ref [_StencilRef]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
            }
		    Cull Back
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            ZTest [_ZTest]

            CGPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Unityデフォ
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma target 4.0
            #pragma multi_compile_instancing
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma fragmentoption ARB_precision_hint_fastest
            //ワールドで好き放題ライティングしているので影は拾ってこないほうが良さそうかも
            //#pragma skip_variants SHADOWS_SCREEN

            //------------------------------------------------------------------------------------------------------------------
            // include
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"

            //------------------------------------------------------------------------------------------------------------------
            // シェーダー本体
            #define LIL_RENDER 0
            #define LIL_FULL
            #include "Includes/lil_properties.cginc"
            #include "Includes/lil_struct.cginc"
            #include "Includes/lil_functions.cginc"
            #include "Includes/lil_vertex.cginc"
            #include "Includes/lil_fragment.cginc"

            ENDCG
        }

        Pass
        {
            Name "FORWARD_ADD"
            Tags {"LightMode"="ForwardAdd"}

            Stencil
            {
                Ref [_StencilRef]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
            }
		    Cull Back
            Blend One One
			ZWrite Off
            ZTest [_ZTest]

            CGPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Unityデフォ
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma target 4.0
            #pragma multi_compile_instancing
            //#pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma fragmentoption ARB_precision_hint_fastest

            //------------------------------------------------------------------------------------------------------------------
            // include
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"

            //------------------------------------------------------------------------------------------------------------------
            // シェーダー本体
            #define LIL_RENDER 0
            #define LIL_FULL
            #define LIL_FOR_ADD
            #include "Includes/lil_properties.cginc"
            #include "Includes/lil_struct.cginc"
            #include "Includes/lil_functions.cginc"
            #include "Includes/lil_vertex.cginc"
            #include "Includes/lil_fragment.cginc"

            ENDCG
        }

        Pass {
            Name "SHADOW_CASTER"
            Tags {"LightMode" = "ShadowCaster"}
            Offset 1, 1
            Cull Off

            CGPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Unityデフォ
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0
            #pragma multi_compile_shadowcaster

            //------------------------------------------------------------------------------------------------------------------
            // include
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            //------------------------------------------------------------------------------------------------------------------
            // シェーダー本体
            #define LIL_RENDER 0
            #define LIL_FULL
            #include "Includes/lil_properties.cginc"
            #include "Includes/lil_functions.cginc"
            #include "Includes/lil_shadowcaster.cginc"

            ENDCG
        }
    }
    Fallback "Unlit/Texture"
    CustomEditor "lilToon.lilToonInspector"
}
