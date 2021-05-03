// lil's Toon Shader
// Copyright (c) 2020 lilxyzw
// This code is under MIT licence, see LICENSE
// https://github.com/lilxyzw/lil-s-Toon-Shader/blob/main/LICENSE
Shader "Hidden/ltspass_lite_cutout"
{
    //------------------------------------------------------------------------------------------------------------------
    // Universal Render Pipeline SM4.5
    SubShader
    {
        Tags{"ShaderModel"="4.5"}
        // ForwardLit
        Pass
        {
            Name "FORWARD"
            Tags {
                "LightMode" = "LightweightForward"
                "RenderType" = "TransparentCutout"
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
            Blend [_SrcBlend] [_DstBlend]
            BlendOp [_BlendOp]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            AlphaToMask On

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
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
            #define LIL_RENDER 1
            #include "Includes/lil_pass_lite.hlsl"

            ENDHLSL
        }

        // Forward Outline
        Pass
        {
            Name "FORWARD_OUTLINE"
            Tags {
                "LightMode" = "SRPDefaultUnlit"
                "RenderType" = "TransparentCutout"
            }

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
            BlendOp [_BlendOp]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            AlphaToMask On

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_RENDER 1
            #define LIL_OUTLINE
            #include "Includes/lil_pass_lite.hlsl"

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
		    Cull [_Cull]

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_LITE
            #define LIL_RENDER 1
            #include "Includes/lil_pass_shadowcaster.hlsl"

            ENDHLSL
        }

        // DepthOnly
        Pass
        {
            Name "DEPTHONLY"
            Tags {
                "LightMode" = "DepthOnly"
                "RenderType" = "TransparentCutout"
            }
		    Cull [_Cull]

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_LITE
            #define LIL_RENDER 1
            #include "Includes/lil_pass_depthonly.hlsl"

            ENDHLSL
        }

        // Meta
        Pass
        {
            Name "META"
            Tags {
                "LightMode"="Meta"
                "RenderType" = "TransparentCutout"
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
            #define LIL_LITE
            #define LIL_RENDER 1
            #include "Includes/lil_pass_meta.hlsl"
            ENDHLSL
        }
    }

    //------------------------------------------------------------------------------------------------------------------
    // Universal Render Pipeline
    SubShader
    {
        // ForwardLit
        Pass
        {
            Name "FORWARD"
            Tags {
                "LightMode" = "LightweightForward"
                "RenderType" = "TransparentCutout"
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
            Blend [_SrcBlend] [_DstBlend]
            BlendOp [_BlendOp]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            AlphaToMask On

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
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
            #define LIL_RENDER 1
            #include "Includes/lil_pass_lite.hlsl"

            ENDHLSL
        }

        // Forward Outline
        Pass
        {
            Name "FORWARD_OUTLINE"
            Tags {
                "LightMode" = "SRPDefaultUnlit"
                "RenderType" = "TransparentCutout"
            }

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
            BlendOp [_BlendOp]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            AlphaToMask On

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_RENDER 1
            #define LIL_OUTLINE
            #include "Includes/lil_pass_lite.hlsl"

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
		    Cull [_Cull]

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            #pragma multi_compile_instancing

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_LITE
            #define LIL_RENDER 1
            #include "Includes/lil_pass_shadowcaster.hlsl"

            ENDHLSL
        }

        // DepthOnly
        Pass
        {
            Name "DEPTHONLY"
            Tags {
                "LightMode" = "DepthOnly"
                "RenderType" = "TransparentCutout"
            }
		    Cull [_Cull]

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile_instancing

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_LITE
            #define LIL_RENDER 1
            #include "Includes/lil_pass_depthonly.hlsl"

            ENDHLSL
        }

        // Meta
        Pass
        {
            Name "META"
            Tags {
                "LightMode"="Meta"
                "RenderType" = "TransparentCutout"
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
            #define LIL_LITE
            #define LIL_RENDER 1
            #include "Includes/lil_pass_meta.hlsl"
            ENDHLSL
        }
    }
}
