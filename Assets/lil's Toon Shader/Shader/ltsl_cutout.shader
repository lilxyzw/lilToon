// lil's Toon Shader
// Copyright (c) 2020 lilxyzw
// This code is under MIT licence, see LICENSE
// https://github.com/lilxyzw/lil-s-Toon-Shader/blob/main/LICENSE
Shader "Hidden/lilToonLiteCutout"
{
    Properties
    {
        //----------------------------------------------------------------------------------------------------------------------
        // Base
        [lilToggle]     _Invisible                  ("Invisible", Int) = 0
                        _Cutoff                     ("Alpha Cutoff", Range(0,1)) = 0.5
		[lilCullMode]   _Cull                       ("Cull Mode|Off|Front|Back", Int) = 2
        [lilToggle]     _FlipNormal                 ("Flip Backface Normal", Int) = 0
                        _BackfaceForceShadow        ("Backface Force Shadow", Range(0,1)) = 0
        [NoScaleOffset] _TriMask                    ("TriMask", 2D) = "white" {}

        //----------------------------------------------------------------------------------------------------------------------
        // Main
                        _Color                      ("Color", Color) = (1,1,1,1)
                        _MainTex                    ("Texture", 2D) = "white" {}
        [lilUVAnim]     _MainTex_ScrollRotate       ("Angle|UV Animation|Scroll|Rotate", Vector) = (0,0,0,0)

        //----------------------------------------------------------------------------------------------------------------------
        // Shadow
        [lilToggleLeft] _UseShadow                  ("Use Shadow", Int) = 0
                        _ShadowBorder               ("Border", Range(0, 1)) = 0.5
                        _ShadowBlur                 ("Blur", Range(0, 1)) = 0.1
        [NoScaleOffset] _ShadowColorTex             ("Shadow Color", 2D) = "black" {}
                        _ShadowEnvStrength          ("Environment Strength", Range(0, 1)) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // MatCap
        [lilToggleLeft] _UseMatCap                  ("Use MatCap", Int) = 0
        [NoScaleOffset] _MatCapTex                  ("Texture", 2D) = "white" {}
        [lilToggle]     _MatCapMul                  ("Multiply", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Rim
        [lilToggleLeft] _UseRim                     ("Use Rim", Int) = 0
                        _RimColor                   ("Color", Color) = (1,1,1,1)
                        _RimBorder                  ("Border", Range(0, 1)) = 0.5
                        _RimBlur                    ("Blur", Range(0, 1)) = 0.1
        [PowerSlider(3.0)]_RimFresnelPower          ("Fresnel Power", Range(0.01, 50)) = 3.0
        [lilToggle]     _RimShadowMask              ("Shadow Mask", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Emmision
        [lilToggleLeft] _UseEmission                ("Use Emission", Int) = 0
        [HDR]           _EmissionColor              ("Color", Color) = (1,1,1)
                        _EmissionMap                ("Texture", 2D) = "white" {}
        [lilUVAnim]     _EmissionMap_ScrollRotate   ("Angle|UV Animation|Scroll|Rotate", Vector) = (0,0,0,0)
        [lilBlink]      _EmissionBlink              ("Blink Strength|Blink Type|Blink Speed|Blink Offset", Vector) = (0,0,3.141593,0)

        //----------------------------------------------------------------------------------------------------------------------
        // Advanced
        [Enum(UnityEngine.Rendering.BlendMode)]         _SrcBlend       ("SrcBlend", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _DstBlend       ("DstBlend", Int) = 0
        [Enum(UnityEngine.Rendering.BlendOp)]           _BlendOp        ("BlendOp", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]         _SrcBlendFA     ("ForwardAdd SrcBlend", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _DstBlendFA     ("ForwardAdd DstBlend", Int) = 1
        [Enum(UnityEngine.Rendering.BlendOp)]           _BlendOpFA      ("ForwardAdd BlendOp", Int) = 4
        [lilToggle]                                     _ZWrite         ("ZWrite", Int) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)]   _ZTest          ("ZTest", Int) = 4
        [IntRange]                                      _StencilRef     ("Stencil Reference Value", Range(0, 255)) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]   _StencilComp    ("Stencil Compare Function", Float) = 8
        [Enum(UnityEngine.Rendering.StencilOp)]         _StencilPass    ("Stencil Pass", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]         _StencilFail    ("Stencil Fail", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]         _StencilZFail   ("Stencil ZFail", Float) = 0
    }
    SubShader
    {
        Tags {"Queue" = "AlphaTest"}
        UsePass "Hidden/ltspass_lite_cutout/FORWARD"
        UsePass "Hidden/ltspass_lite_cutout/SHADOW_CASTER"
        UsePass "Hidden/ltspass_lite_cutout/DEPTHONLY"
        UsePass "Hidden/ltspass_lite_cutout/META"
    }
    Fallback "Unlit/Texture"
    CustomEditor "lilToon.lilToonInspector"
}
