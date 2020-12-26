// lil's Toon Shader
// Copyright (c) 2020 lilxyzw
// This code is under MIT licence, see LICENSE
// https://github.com/lilxyzw/lil-s-Toon-Shader/blob/main/LICENSE
Shader "Hidden/_lil/lilToonLite"
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
        // AlphaMask
        [lilToggle] _UseAlphaMask ("UseAlphaMask", Int) = 0
        [NoScaleOffset] _AlphaMask ("AlphaMask", 2D) = "white" {}

        //----------------------------------------------------------------------------------------------------------------------
        // Shadow
        [lilToggle] _UseShadow ("UseShadow", Int) = 0
        _ShadowColor("ShadowColor", Color) = (1,1,1,1)
        _ShadowBorder ("Border", Range(0, 1)) = 0.65
        _ShadowBlur ("Blur", Range(0, 1)) = 0.01
        [NoScaleOffset] _ShadowStrengthMask ("StrengthMask", 2D) = "white" {}

        //----------------------------------------------------------------------------------------------------------------------
        // Outline
        [lilToggle] _UseOutline ("UseOutline", Int) = 0
        _OutlineColor("Color", Color) = (1,1,1,1)
        _OutlineWidth ("Width", Range(0,10)) = 1
        [NoScaleOffset] _OutlineAlphaMask ("AlphaMask", 2D) = "white" {}
        [Enum(None,0,R,1,G,2,B,3,A,4)] _VertexColor2Width ("VertexColor2Width", Int) = 0

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
            "Queue" = "Geometry"
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
		    Cull Front
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            ZTest [_ZTest]

            CGPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Unityデフォ
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_instancing
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma skip_variants SHADOWS_SCREEN

            //------------------------------------------------------------------------------------------------------------------
            // include
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"

            //------------------------------------------------------------------------------------------------------------------
            // シェーダー本体
            #define LIL_RENDER 0
            #define LIL_LITE_OUTLINE
            #include "Includes/lil_lite_properties.cginc"
            #include "Includes/lil_lite_struct.cginc"
            #include "Includes/lil_functions.cginc"
            #include "Includes/lil_lite_vertex.cginc"
            #include "Includes/lil_lite_fragment.cginc"

            ENDCG
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
		    Cull [_CullMode]
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            ZTest [_ZTest]

            CGPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Unityデフォ
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_instancing
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma skip_variants SHADOWS_SCREEN

            //------------------------------------------------------------------------------------------------------------------
            // include
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"

            //------------------------------------------------------------------------------------------------------------------
            // シェーダー本体
            #define LIL_RENDER 0
            #include "Includes/lil_lite_properties.cginc"
            #include "Includes/lil_lite_struct.cginc"
            #include "Includes/lil_functions.cginc"
            #include "Includes/lil_lite_vertex.cginc"
            #include "Includes/lil_lite_fragment.cginc"

            ENDCG
        }
    }
    Fallback "Unlit/Texture"
    CustomEditor "lilToon.lilToonInspector"
}
