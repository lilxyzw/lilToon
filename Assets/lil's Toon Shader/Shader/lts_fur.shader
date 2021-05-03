// lil's Toon Shader
// Copyright (c) 2020 lilxyzw
// This code is under MIT licence, see LICENSE
// https://github.com/lilxyzw/lil-s-Toon-Shader/blob/main/LICENSE
Shader "Hidden/lilToonFur"
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

        //----------------------------------------------------------------------------------------------------------------------
        // Main
                        _Color                      ("Color", Color) = (1,1,1,1)
                        _MainTex                    ("Texture", 2D) = "white" {}
        [lilUVAnim]     _MainTex_ScrollRotate       ("Angle|UV Animation|Scroll|Rotate", Vector) = (0,0,0,0)
        [lilHSVG]       _MainTexHSVG                ("Hue|Saturation|Value|Gamma", Vector) = (0,1,1,1)

        //----------------------------------------------------------------------------------------------------------------------
        // Shadow
        [lilToggleLeft] _UseShadow                  ("Use Shadow", Int) = 0
        [lilToggle]     _ShadowReceive              ("Receive Shadow", Int) = 0
                        _ShadowBorder               ("Border", Range(0, 1)) = 0.5
        [NoScaleOffset] _ShadowBorderMask           ("Border", 2D) = "white" {}
                        _ShadowBlur                 ("Blur", Range(0, 1)) = 0.1
        [NoScaleOffset] _ShadowBlurMask             ("Blur", 2D) = "white" {}
                        _ShadowStrength             ("Strength", Range(0, 1)) = 1
        [NoScaleOffset] _ShadowStrengthMask         ("Strength", 2D) = "white" {}
                        _ShadowColor                ("Shadow Color", Color) = (0.7,0.75,0.85,1.0)
        [NoScaleOffset] _ShadowColorTex             ("Shadow Color", 2D) = "black" {}
                        _Shadow2ndBorder            ("2nd Border", Range(0, 1)) = 0.5
                        _Shadow2ndBlur              ("2nd Blur", Range(0, 1)) = 0.3
                        _Shadow2ndColor             ("Shadow 2nd Color", Color) = (0,0,0,0)
        [NoScaleOffset] _Shadow2ndColorTex          ("Shadow 2nd Color", 2D) = "black" {}
                        _ShadowMainStrength         ("Main Color Strength", Range(0, 1)) = 1
                        _ShadowEnvStrength          ("Environment Strength", Range(0, 1)) = 1
                        _ShadowBorderColor          ("Border Color", Color) = (1,0,0,1)
                        _ShadowBorderRange          ("Border Range", Range(0, 1)) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Fur
                        _FurNoiseMask               ("Fur Noise", 2D) = "white" {}
        [NoScaleOffset] _FurMask                    ("Fur Mask", 2D) = "white" {}
        [NoScaleOffset][Normal] _FurVectorTex       ("Fur Vector", 2D) = "bump" {}
                        _FurVectorScale             ("Fur Vector scale", Range(-10,10)) = 1
        [lilVec3Float]  _FurVector                  ("Fur Vector|Fur Length", Vector) = (0.0,0.0,1.0,0.2)
        [lilToggle]     _VertexColor2FurVector      ("VertexColor->Vector", Int) = 0
                        _FurGravity                 ("Fur Gravity", Range(0,1)) = 0.25
                        _FurAO                      ("Fur AO", Range(0,1)) = 0
        [IntRange]      _FurLayerNum                ("Fur Layer Num", Range(1, 6)) = 4

        //----------------------------------------------------------------------------------------------------------------------
        // Advanced
        [Enum(UnityEngine.Rendering.BlendMode)]         _SrcBlend       ("SrcBlend", Int) = 5
        [Enum(UnityEngine.Rendering.BlendMode)]         _DstBlend       ("DstBlend", Int) = 10
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
    //------------------------------------------------------------------------------------------------------------------
    // Universal Render Pipeline SM4.5
    SubShader
    {
        Tags {"Queue" = "Transparent" "ShaderModel"="4.5"}

        // ForwardLit
        Pass
        {
            Name "FORWARD"
            Tags {
                "LightMode" = "SRPDefaultUnlit"
                "RenderType" = "Transparent"
            }

            Stencil
            {
                Ref [_StencilRef]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
            }
		    Cull [_Cull]
            Blend [_SrcBlend] [_DstBlend], One OneMinusSrcAlpha
            BlendOp [_BlendOp]
            ZWrite [_ZWrite]
            ZTest [_ZTest]

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_RENDER 2
            #define LIL_FUR
            #include "Includes/lil_pass_normal.hlsl"

            ENDHLSL
        }

        // Fur
        Pass
        {
            Name "FORWARD_FUR"
            Tags {
                "LightMode" = "LightweightForward"
                "RenderType" = "Transparent"
            }

            Stencil
            {
                Ref [_StencilRef]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
            }
		    Cull Off
            Blend [_SrcBlend] [_DstBlend], One OneMinusSrcAlpha
            BlendOp [_BlendOp]
            ZWrite Off
            ZTest [_ZTest]

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma target 4.5
            #pragma require geometry
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_RENDER 2
            #include "Includes/lil_pass_fur.hlsl"

            ENDHLSL
        }

        UsePass "Hidden/ltspass_transparent/SHADOW_CASTER"
        UsePass "Hidden/ltspass_transparent/DEPTHONLY"

        // Meta
        Pass
        {
            Name "META"
            Tags {
                "LightMode"="Meta"
                "RenderType" = "Transparent"
            }
            Cull Off

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_FUR
            #define LIL_RENDER 2
            #include "Includes/lil_pass_meta.hlsl"
            ENDHLSL
        }
    }

    //------------------------------------------------------------------------------------------------------------------
    // Universal Render Pipeline
    SubShader
    {
        Tags {"Queue" = "Transparent"}

        // ForwardLit
        Pass
        {
            Name "FORWARD"
            Tags {
                "LightMode" = "SRPDefaultUnlit"
                "RenderType" = "Transparent"
            }

            Stencil
            {
                Ref [_StencilRef]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
            }
		    Cull [_Cull]
            Blend [_SrcBlend] [_DstBlend], One OneMinusSrcAlpha
            BlendOp [_BlendOp]
            ZWrite [_ZWrite]
            ZTest [_ZTest]

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_RENDER 2
            #define LIL_FUR
            #include "Includes/lil_pass_normal.hlsl"

            ENDHLSL
        }

        // Fur
        Pass
        {
            Name "FORWARD_FUR"
            Tags {
                "LightMode" = "LightweightForward"
                "RenderType" = "Transparent"
            }

            Stencil
            {
                Ref [_StencilRef]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
            }
		    Cull Off
            Blend [_SrcBlend] [_DstBlend], One OneMinusSrcAlpha
            BlendOp [_BlendOp]
            ZWrite Off
            ZTest [_ZTest]

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma target 4.0
            #pragma require geometry
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_RENDER 2
            #include "Includes/lil_pass_fur.hlsl"

            ENDHLSL
        }

        UsePass "Hidden/ltspass_transparent/SHADOW_CASTER"
        UsePass "Hidden/ltspass_transparent/DEPTHONLY"

        // Meta
        Pass
        {
            Name "META"
            Tags {
                "LightMode"="Meta"
                "RenderType" = "Transparent"
            }
            Cull Off

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma only_renderers gles gles3 glcore d3d11

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_FUR
            #define LIL_RENDER 2
            #include "Includes/lil_pass_meta.hlsl"
            ENDHLSL
        }
    }
    Fallback "Unlit/Texture"
    CustomEditor "lilToon.lilToonInspector"
}
