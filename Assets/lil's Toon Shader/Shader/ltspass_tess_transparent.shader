Shader "Hidden/ltspass_tess_transparent"
{
    SubShader
    {
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
            #pragma vertex vertTess
            #pragma fragment frag
            #pragma hull hull
            #pragma domain domain
            #pragma target 4.6
            #pragma multi_compile_instancing
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma skip_variants SHADOWS_SCREEN

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_TESSELLATION
            #define LIL_RENDER 2
            #include "Includes/lil_pass_normal.hlsl"

            ENDHLSL
        }

        // Forward Outline
        Pass
        {
            Name "FORWARD_OUTLINE"
            Tags {
                "LightMode" = "ForwardBase"
                "RenderType" = "Transparent"
            }

		    Cull Front
            Blend [_SrcBlend] [_DstBlend], One OneMinusSrcAlpha
            BlendOp [_BlendOp]
            ZWrite [_ZWrite]
            ZTest [_ZTest]

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vertTess
            #pragma fragment frag
            #pragma hull hull
            #pragma domain domain
            #pragma target 4.6
            #pragma multi_compile_instancing
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma skip_variants SHADOWS_SCREEN

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_TESSELLATION
            #define LIL_RENDER 2
            #define LIL_OUTLINE
            #include "Includes/lil_pass_normal.hlsl"

            ENDHLSL
        }

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
            #pragma vertex vertTess
            #pragma fragment frag
            #pragma hull hull
            #pragma domain domain
            #pragma target 4.6
            #pragma multi_compile_instancing
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma fragmentoption ARB_precision_hint_fastest

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_TESSELLATION
            #define LIL_RENDER 2
            #define LIL_PASS_FORWARDADD
            #include "Includes/lil_pass_normal.hlsl"

            ENDHLSL
        }

        // ShadowCaster
        Pass
        {
            Name "SHADOW_CASTER"
            Tags {
                "LightMode" = "ShadowCaster"
                "RenderType" = "TransparentCutout"
            }
            Offset 1, 1
		    Cull [_Cull]

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5
            #pragma multi_compile_shadowcaster

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_RENDER 2
            #include "Includes/lil_pass_shadowcaster.hlsl"

            ENDHLSL
        }

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
            #include "Includes/lil_pass_meta.hlsl"
            ENDHLSL
        }
    }
}
