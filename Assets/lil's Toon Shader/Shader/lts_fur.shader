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
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderPipeline" = ""}

        // Forward
        Pass
        {
            Name "FORWARD"
            Tags {
                "LightMode" = "ForwardBase"
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
            #pragma multi_compile_instancing
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma skip_variants SHADOWS_SCREEN

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_RENDER 2
            #define LIL_FUR
            #include "Includes/lil_pass_normal.hlsl"

            ENDHLSL
        }

        /*
        // ForwardAdd
        Pass
        {
            Name "FORWARD_ADD"
            Tags {
                "LightMode" = "ForwardAdd"
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
            Blend [_SrcBlendFA] [_DstBlendFA], Zero One
            BlendOp [_BlendOpFA]
            Fog { Color(0,0,0,0) }
			ZWrite Off
            ZTest LEqual

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0
            #pragma multi_compile_instancing
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma fragmentoption ARB_precision_hint_fastest

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_RENDER 2
            #define LIL_FUR
            #define LIL_PASS_FORWARDADD
            #include "Includes/lil_pass_normal.hlsl"

            ENDHLSL
        }
        */

        // Forward Fur
        Pass
        {
            Name "FORWARD_FUR"
            Tags {
                "LightMode" = "ForwardBase"
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
            #pragma multi_compile_instancing
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma skip_variants SHADOWS_SCREEN

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_RENDER 2
            #include "Includes/lil_pass_fur.hlsl"

            ENDHLSL
        }
        /*
        // ForwardAdd Fur
        Pass
        {
            Name "FORWARD_ADD_FUR"
            Tags {
                "LightMode" = "ForwardAdd"
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
            Blend [_SrcBlendFA] [_DstBlendFA], Zero One
            BlendOp [_BlendOpFA]
            Fog { Color(0,0,0,0) }
			ZWrite Off
            ZTest LEqual

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma target 4.0
            #pragma require geometry
            #pragma multi_compile_instancing
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma fragmentoption ARB_precision_hint_fastest

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_RENDER 2
            #define LIL_PASS_FORWARDADD
            #include "Includes/lil_pass_fur.hlsl"

            ENDHLSL
        }
        */

        UsePass "Hidden/ltspass_transparent/SHADOW_CASTER"

        // Meta
        Pass
        {
            Name "META"
            Tags {
                "LightMode" = "Meta"
                "RenderType" = "Transparent"
            }
            Cull Off

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature EDITOR_VISUALIZATION

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_RENDER 2
            #define LIL_FUR
            #include "Includes/lil_pass_meta.hlsl"
            ENDHLSL
        }
    }
    Fallback "Unlit/Texture"
    CustomEditor "lilToon.lilToonInspector"
}
