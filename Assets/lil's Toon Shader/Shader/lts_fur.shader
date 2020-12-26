// lil's Toon Shader
// Copyright (c) 2020 lilxyzw
// This code is under MIT licence, see LICENSE
// https://github.com/lilxyzw/lil-s-Toon-Shader/blob/main/LICENSE
Shader "Hidden/_lil/lilToonFur"
{
    Properties
    {
        //----------------------------------------------------------------------------------------------------------------------
        // Base
        [lilToggle] _Invisible ("Invisible", Int) = 0
        [Enum(RenderingMode)] _Mode ("Rendering Mode", Int) = 6
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("Cull Mode", Int) = 2
        [lilToggle] _FlipNormal ("Flip Backface Normal", Int) = 0
        _BackfaceForceShadow ("Backface Force Shadow", Range(0,1)) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Main
        _Color("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _MainTexTonecurve ("Tonecurve", Range(0.01,2)) = 1
        _MainTexHue ("Hue", Range(-0.5,0.5)) = 0
        _MainTexSaturation ("Saturation", Range(0,2)) = 1
        _MainTexValue ("Value", Range(0,2)) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // Main2nd
        [lilToggle] _UseMain2ndTex ("UseMain2nd", Int) = 0
        _Color2nd("Color", Color) = (1,1,1,1)
        _Main2ndTex ("Texture", 2D) = "white" {}
        _Main2ndTexAngle ("Angle", Float) = 0
        [lilToggle] _Main2ndTexTrim ("Trim", Int) = 0
        [Enum(MixMode)] _Main2ndTexMix ("Mix", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Main3rd
        [lilToggle] _UseMain3rdTex ("UseMain3rd", Int) = 0
        _Color3rd("Color", Color) = (1,1,1,1)
        _Main3rdTex ("Texture", 2D) = "white" {}
        _Main3rdTexAngle ("Angle", Float) = 0
        [lilToggle] _Main3rdTexTrim ("Trim", Int) = 0
        [Enum(MixMode)] _Main3rdTexMix ("Mix", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Main4th
        [lilToggle] _UseMain4thTex ("UseMain4th", Int) = 0
        _Color4th("Color", Color) = (1,1,1,1)
        _Main4thTex ("Texture", 2D) = "white" {}
        _Main4thTexAngle ("Angle", Float) = 0
        [lilToggle] _Main4thTexTrim ("Trim", Int) = 0
        [Enum(MixMode)] _Main4thTexMix ("Mix", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // AlphaMask
        [lilToggle] _UseAlphaMask ("UseAlphaMask", Int) = 0
        [NoScaleOffset] _AlphaMask ("AlphaMask", 2D) = "white" {}

        //----------------------------------------------------------------------------------------------------------------------
        // NormalMap
        [lilToggle] _UseBumpMap ("UseNormalMap", Int) = 0
        [Normal]_BumpMap ("Normal map", 2D) = "bump" {}
        _BumpScale ("Normal scale", Range(-10,10)) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // NormalMap 2nd
        [lilToggle] _UseBump2ndMap ("UseNormalMap2nd", Int) = 0
        [Normal]_Bump2ndMap ("Normal map", 2D) = "bump" {}
        _Bump2ndScale ("Normal scale", Range(-10,10)) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // Shadow
        [lilToggle] _UseShadow ("UseShadow", Int) = 0
        _ShadowBorder ("Border", Range(0, 1)) = 0.65
         _ShadowBorderMask ("BorderMask", 2D) = "white" {}
        _ShadowBlur ("Blur", Range(0, 1)) = 0.01
         _ShadowBlurMask ("BlurMask", 2D) = "white" {}
        _ShadowStrength ("Strength", Range(0, 1)) = 0.5
        [NoScaleOffset] _ShadowStrengthMask ("StrengthMask", 2D) = "white" {}
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
        [NoScaleOffset] _ShadowColorTex ("ShadowColorTex", 2D) = "white" {}
        [Enum(MixMode)] _ShadowColorMix ("Mix", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Shadow 2nd
        [lilToggle] _UseShadow2nd ("UseShadow2nd", Int) = 0
        _Shadow2ndBorder ("Border", Range(0, 1)) = 0.5
        _Shadow2ndBlur ("Blur", Range(0, 1)) = 0.3
        _Shadow2ndColorFromMain ("Shadow2ndColorFromMain", Range(0, 1)) = 0
        _Shadow2ndColor("Shadow2ndColor", Color) = (0.75,0.85,1,1)
        [NoScaleOffset] _Shadow2ndColorTex ("Shadow2ndColorTex", 2D) = "white" {}
        [Enum(MixMode)] _Shadow2ndColorMix ("Mix", Int) = 3

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
        _OutlineColor("Color", Color) = (1,1,1,1)
        _OutlineWidth ("Width", Range(0,10)) = 1
        [NoScaleOffset] _OutlineWidthMask ("WidthMask", 2D) = "white" {}
        [NoScaleOffset] _OutlineAlphaMask ("AlphaMask", 2D) = "white" {}
        [Enum(None,0,R,1,G,2,B,3,A,4)] _VertexColor2Width ("VertexColor2Width", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Fur
         _FurNoiseMask ("NoiseMask", 2D) = "white" {}
        [NoScaleOffset] _FurMask ("Mask", 2D) = "white" {}
        [NoScaleOffset][Normal] _FurVectorTex ("FurVector", 2D) = "bump" {}
        _FurVectorScale ("FurVector scale", Range(-10,10)) = 1
        _FurVectorX ("FurVectorX", Float) = 0
        _FurVectorY ("FurVectorY", Float) = 0
        _FurVectorZ ("FurVectorZ", Float) = 1
        _FurLength ("FurLength", Float) = 0.2
        _FurGravity ("FurGravity", Range(0,1)) = 0.25
        _FurAO ("FurAO", Range(0,1)) = 0
        [lilToggle] _VertexColor2FurVector ("VertexColor2FurVector", Int) = 0
        [IntRange] _FurLayerNum ("Fur Layer", Range(1, 6)) = 4

        //----------------------------------------------------------------------------------------------------------------------
        // Reflection
        [lilToggle]_UseReflection ("Use Reflection", Int) = 0
        // Smoothness
        _Smoothness ("Smoothness", Range(0, 1)) = 1
        [NoScaleOffset] _SmoothnessTex ("Smoothness Mask", 2D) = "white" {}
        // Metallic
        [Gamma] _Metallic ("Metallic", Range(0, 1)) = 0
        [NoScaleOffset] _MetallicGlossMap ("Metallic", 2D) = "white" {}
        // Reflection
        [lilToggle]_ApplySpecular ("Apply Specular", Int) = 1
        [lilToggle]_ApplyReflection ("Apply Reflection", Int) = 0
        _ReflectionBlend ("Blend", Range(0, 1)) = 1
        [NoScaleOffset] _ReflectionBlendMask ("BlendMask", 2D) = "white" {}
        _ReflectionShadeMix ("Shade Mix", Range(0, 1)) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Matcap
        [lilToggle] _UseMatcap ("UseMatcap", Int) = 0
        _MatcapColor ("Color", Color) = (1,1,1,1)
        [NoScaleOffset] _MatcapTex ("Tex", 2D) = "black" {}
        _MatcapBlend ("Blend", Range(0, 3)) = 1
        [NoScaleOffset] _MatcapBlendMask ("BlendMask", 2D) = "white" {}
        _MatcapShadeMix ("Shade Mix", Range(0, 1)) = 1
        [Enum(MixMode)] _MatcapMix ("Mix", Int) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // Rim
        [lilToggle] _UseRim ("UseRim", Int) = 0
        _RimColor("Color", Color) = (1,1,1,1)
        [Enum(UVSet)] _RimColorTexUV ("UV Set", Int) = 0
        [lilToggle] _RimColorTexTrim ("Trim", Int) = 0
        _RimBlend ("Blend", Range(0, 1)) = 0.5
        [NoScaleOffset] _RimBlendMask ("BlendMask", 2D) = "white" {}
        [lilToggle] _RimToon ("Toon", Int) = 0
        _RimBorder ("Border", Range(0, 1)) = 0.5
        _RimBlur ("Blur", Range(0, 1)) = 0.0
        _RimUpperSideWidth ("Upper Width", Range(0, 1)) = 0
        [PowerSlider(3.0)]_RimFresnelPower ("Fresnel Power", Range(0.01, 50)) = 0.8
        _RimShadeMix ("Apply Shade", Range(0,1)) = 1
        [lilToggle] _RimShadowMask ("Shadow Mask", Int) = 0

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
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
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
            #define LIL_RENDER 2
            #define LIL_FUR
            #include "Includes/lil_properties.cginc"
            #include "Includes/lil_struct.cginc"
            #include "Includes/lil_functions.cginc"
            #include "Includes/lil_vertex.cginc"
            #include "Includes/lil_fragment.cginc"

            ENDCG
        }

        Pass
        {
            Name "FORWARD_FUR"
            Tags {"LightMode"="ForwardBase"}

            Stencil
            {
                Ref [_StencilRef]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
            }
		    Cull Off
            Blend [_SrcBlend] [_DstBlend]
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
            #define LIL_RENDER 2
            #include "Includes/lil_fur_properties.cginc"
            #include "Includes/lil_fur_struct.cginc"
            #include "Includes/lil_functions.cginc"
            #include "Includes/lil_fur_vertex.cginc"
            #include "Includes/lil_fur_fragment.cginc"

            ENDCG
        }
/*ファーでは色を合わせるために使わない
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
            #define LIL_RENDER 2
            #define LIL_FUR
            #define LIL_FOR_ADD
            #include "Includes/lil_properties.cginc"
            #include "Includes/lil_struct.cginc"
            #include "Includes/lil_functions.cginc"
            #include "Includes/lil_vertex.cginc"
            #include "Includes/lil_fragment.cginc"

            ENDCG
        }
*/
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
            #include "Includes/lil_properties.cginc"
            #include "Includes/lil_functions.cginc"
            #include "Includes/lil_shadowcaster.cginc"

            ENDCG
        }
    }
    Fallback "Unlit/Texture"
    CustomEditor "lilToon.lilToonInspector"
}
