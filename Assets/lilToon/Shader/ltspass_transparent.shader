Shader "Hidden/ltspass_transparent"
{
    HLSLINCLUDE
        #define LIL_RENDER 2
    ENDHLSL

//----------------------------------------------------------------------------------------------------------------------
// BRP Start
//
    SubShader
    {
        // Forward
        Pass
        {
            Name "FORWARD"
            Tags {"LightMode" = "ForwardBase"}

            Stencil
            {
                Ref [_StencilRef]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
            }
            Cull [_Cull]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            ColorMask [_ColorMask]
            Offset [_OffsetFactor], [_OffsetUnits]
            BlendOp [_BlendOp], [_BlendOpAlpha]
            Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma fragmentoption ARB_precision_hint_fastest

            // Skip receiving shadow
            #pragma skip_variants SHADOWS_SCREEN

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_normal.hlsl"

            ENDHLSL
        }

        // Forward Outline
        Pass
        {
            Name "FORWARD_OUTLINE"
            Tags {"LightMode" = "ForwardBase"}

            Stencil
            {
                Ref [_OutlineStencilRef]
                ReadMask [_OutlineStencilReadMask]
                WriteMask [_OutlineStencilWriteMask]
                Comp [_OutlineStencilComp]
                Pass [_OutlineStencilPass]
                Fail [_OutlineStencilFail]
                ZFail [_OutlineStencilZFail]
            }
            Cull [_OutlineCull]
            ZWrite [_OutlineZWrite]
            ZTest [_OutlineZTest]
            ColorMask [_OutlineColorMask]
            Offset [_OutlineOffsetFactor], [_OutlineOffsetUnits]
            BlendOp [_OutlineBlendOp], [_OutlineBlendOpAlpha]
            Blend [_OutlineSrcBlend] [_OutlineDstBlend], [_OutlineSrcBlendAlpha] [_OutlineDstBlendAlpha]

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma skip_variants SHADOWS_SCREEN

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_OUTLINE
            #include "Includes/lil_pass_normal.hlsl"

            ENDHLSL
        }

        // ForwardAdd
        Pass
        {
            Name "FORWARD_ADD"
            Tags {"LightMode" = "ForwardAdd"}

            Stencil
            {
                Ref [_StencilRef]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
            }
		    Cull [_Cull]
			ZWrite Off
            ZTest LEqual
            ColorMask [_ColorMask]
            Offset [_OffsetFactor], [_OffsetUnits]
            Blend [_SrcBlendFA] [_DstBlendFA], Zero One
            BlendOp [_BlendOpFA], [_BlendOpAlphaFA]
            Fog { Color(0,0,0,0) }

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma fragmentoption ARB_precision_hint_fastest

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_PASS_FORWARDADD
            #include "Includes/lil_pass_normal.hlsl"

            ENDHLSL
        }

        // ShadowCaster
        Pass
        {
            Name "SHADOW_CASTER"
            Tags {"LightMode" = "ShadowCaster"}
            Offset 1, 1
		    Cull [_Cull]

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_shadowcaster.hlsl"

            ENDHLSL
        }

        // Meta
        Pass
        {
            Name "META"
            Tags {"LightMode" = "Meta"}
            Cull Off

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature EDITOR_VISUALIZATION

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_meta.hlsl"

            ENDHLSL
        }
    }
//
// BRP End

//----------------------------------------------------------------------------------------------------------------------
// LWRP Start
/*
    //------------------------------------------------------------------------------------------------------------------
    // Lightweight Render Pipeline SM4.5
    SubShader
    {
        Tags{"ShaderModel" = "4.5"}
        // ForwardLit
        Pass
        {
            Name "FORWARD"
            Tags {"LightMode" = "LightweightForward"}

            Stencil
            {
                Ref [_StencilRef]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
            }
            Cull [_Cull]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            ColorMask [_ColorMask]
            Offset [_OffsetFactor], [_OffsetUnits]
            BlendOp [_BlendOp], [_BlendOpAlpha]
            Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            // Skip receiving shadow
            //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            //#pragma multi_compile_fragment _ _SHADOWS_SOFT

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_normal.hlsl"

            ENDHLSL
        }

        // Forward Outline
        Pass
        {
            Name "FORWARD_OUTLINE"
            Tags {"LightMode" = "SRPDefaultUnlit"}

            Stencil
            {
                Ref [_OutlineStencilRef]
                ReadMask [_OutlineStencilReadMask]
                WriteMask [_OutlineStencilWriteMask]
                Comp [_OutlineStencilComp]
                Pass [_OutlineStencilPass]
                Fail [_OutlineStencilFail]
                ZFail [_OutlineStencilZFail]
            }
            Cull [_OutlineCull]
            ZWrite [_OutlineZWrite]
            ZTest [_OutlineZTest]
            ColorMask [_OutlineColorMask]
            Offset [_OutlineOffsetFactor], [_OutlineOffsetUnits]
            BlendOp [_OutlineBlendOp], [_OutlineBlendOpAlpha]
            Blend [_OutlineSrcBlend] [_OutlineDstBlend], [_OutlineSrcBlendAlpha] [_OutlineDstBlendAlpha]

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
            #define LIL_OUTLINE
            #include "Includes/lil_pass_normal.hlsl"

            ENDHLSL
        }

        // ShadowCaster
        Pass
        {
            Name "SHADOW_CASTER"
            Tags {"LightMode" = "ShadowCaster"}
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
            #include "Includes/lil_pass_shadowcaster.hlsl"

            ENDHLSL
        }

        // DepthOnly
        Pass
        {
            Name "DEPTHONLY"
            Tags {"LightMode" = "DepthOnly"}
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
            #include "Includes/lil_pass_depthonly.hlsl"

            ENDHLSL
        }

        // Meta
        Pass
        {
            Name "META"
            Tags {"LightMode" = "Meta"}
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
            #include "Includes/lil_pass_meta.hlsl"
            ENDHLSL
        }
    }

    //------------------------------------------------------------------------------------------------------------------
    // Lightweight Render Pipeline
    SubShader
    {
        // ForwardLit
        Pass
        {
            Name "FORWARD"
            Tags {"LightMode" = "LightweightForward"}

            Stencil
            {
                Ref [_StencilRef]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
            }
            Cull [_Cull]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            ColorMask [_ColorMask]
            Offset [_OffsetFactor], [_OffsetUnits]
            BlendOp [_BlendOp], [_BlendOpAlpha]
            Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            // Skip receiving shadow
            //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            //#pragma multi_compile_fragment _ _SHADOWS_SOFT

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_normal.hlsl"

            ENDHLSL
        }

        // Forward Outline
        Pass
        {
            Name "FORWARD_OUTLINE"
            Tags {"LightMode" = "SRPDefaultUnlit"}

            Stencil
            {
                Ref [_OutlineStencilRef]
                ReadMask [_OutlineStencilReadMask]
                WriteMask [_OutlineStencilWriteMask]
                Comp [_OutlineStencilComp]
                Pass [_OutlineStencilPass]
                Fail [_OutlineStencilFail]
                ZFail [_OutlineStencilZFail]
            }
            Cull [_OutlineCull]
            ZWrite [_OutlineZWrite]
            ZTest [_OutlineZTest]
            ColorMask [_OutlineColorMask]
            Offset [_OutlineOffsetFactor], [_OutlineOffsetUnits]
            BlendOp [_OutlineBlendOp], [_OutlineBlendOpAlpha]
            Blend [_OutlineSrcBlend] [_OutlineDstBlend], [_OutlineSrcBlendAlpha] [_OutlineDstBlendAlpha]

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
            #define LIL_OUTLINE
            #include "Includes/lil_pass_normal.hlsl"

            ENDHLSL
        }

        // ShadowCaster
        Pass
        {
            Name "SHADOW_CASTER"
            Tags {"LightMode" = "ShadowCaster"}
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
            #include "Includes/lil_pass_shadowcaster.hlsl"

            ENDHLSL
        }

        // DepthOnly
        Pass
        {
            Name "DEPTHONLY"
            Tags {"LightMode" = "DepthOnly"}
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
            #include "Includes/lil_pass_depthonly.hlsl"

            ENDHLSL
        }

        // Meta
        Pass
        {
            Name "META"
            Tags {"LightMode" = "Meta"}
            Cull Off

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma only_renderers gles gles3 glcore d3d11

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_meta.hlsl"
            ENDHLSL
        }
    }
*/
// LWRP End

//----------------------------------------------------------------------------------------------------------------------
// URP Start
/*
    //------------------------------------------------------------------------------------------------------------------
    // Universal Render Pipeline SM4.5
    SubShader
    {
        Tags{"ShaderModel" = "4.5"}
        // ForwardLit
        Pass
        {
            Name "FORWARD"
            Tags {"LightMode" = "UniversalForward"}

            Stencil
            {
                Ref [_StencilRef]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
            }
            Cull [_Cull]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            ColorMask [_ColorMask]
            Offset [_OffsetFactor], [_OffsetUnits]
            BlendOp [_BlendOp], [_BlendOpAlpha]
            Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            // Skip receiving shadow
            //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            //#pragma multi_compile_fragment _ _SHADOWS_SOFT

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_normal.hlsl"

            ENDHLSL
        }

        // Forward Outline
        Pass
        {
            Name "FORWARD_OUTLINE"
            Tags {"LightMode" = "SRPDefaultUnlit"}

            Stencil
            {
                Ref [_OutlineStencilRef]
                ReadMask [_OutlineStencilReadMask]
                WriteMask [_OutlineStencilWriteMask]
                Comp [_OutlineStencilComp]
                Pass [_OutlineStencilPass]
                Fail [_OutlineStencilFail]
                ZFail [_OutlineStencilZFail]
            }
            Cull [_OutlineCull]
            ZWrite [_OutlineZWrite]
            ZTest [_OutlineZTest]
            ColorMask [_OutlineColorMask]
            Offset [_OutlineOffsetFactor], [_OutlineOffsetUnits]
            BlendOp [_OutlineBlendOp], [_OutlineBlendOpAlpha]
            Blend [_OutlineSrcBlend] [_OutlineDstBlend], [_OutlineSrcBlendAlpha] [_OutlineDstBlendAlpha]

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
            #define LIL_OUTLINE
            #include "Includes/lil_pass_normal.hlsl"

            ENDHLSL
        }

        // ShadowCaster
        Pass
        {
            Name "SHADOW_CASTER"
            Tags {"LightMode" = "ShadowCaster"}
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
            #include "Includes/lil_pass_shadowcaster.hlsl"

            ENDHLSL
        }

        // DepthOnly
        Pass
        {
            Name "DEPTHONLY"
            Tags {"LightMode" = "DepthOnly"}
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
            #include "Includes/lil_pass_depthonly.hlsl"

            ENDHLSL
        }

        // DepthNormals
        Pass
        {
            Name "DEPTHNORMALS"
            Tags {"LightMode" = "DepthNormals"}
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
            #include "Includes/lil_pass_depthnormals.hlsl"

            ENDHLSL
        }

        // Universal2D
        Pass
        {
            Name "UNIVERSAL2D"
            Tags {"LightMode" = "Universal2D"}

            Stencil
            {
                Ref [_StencilRef]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
            }
            Cull [_Cull]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            ColorMask [_ColorMask]
            Offset [_OffsetFactor], [_OffsetUnits]
            BlendOp [_BlendOp], [_BlendOpAlpha]
            Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_universal2d.hlsl"
            ENDHLSL
        }

        // Meta
        Pass
        {
            Name "META"
            Tags {"LightMode" = "Meta"}
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
            Tags {"LightMode" = "UniversalForward"}

            Stencil
            {
                Ref [_StencilRef]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
            }
            Cull [_Cull]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            ColorMask [_ColorMask]
            Offset [_OffsetFactor], [_OffsetUnits]
            BlendOp [_BlendOp], [_BlendOpAlpha]
            Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            // Skip receiving shadow
            //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            //#pragma multi_compile_fragment _ _SHADOWS_SOFT

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_normal.hlsl"

            ENDHLSL
        }

        // Forward Outline
        Pass
        {
            Name "FORWARD_OUTLINE"
            Tags {"LightMode" = "SRPDefaultUnlit"}

            Stencil
            {
                Ref [_OutlineStencilRef]
                ReadMask [_OutlineStencilReadMask]
                WriteMask [_OutlineStencilWriteMask]
                Comp [_OutlineStencilComp]
                Pass [_OutlineStencilPass]
                Fail [_OutlineStencilFail]
                ZFail [_OutlineStencilZFail]
            }
            Cull [_OutlineCull]
            ZWrite [_OutlineZWrite]
            ZTest [_OutlineZTest]
            ColorMask [_OutlineColorMask]
            Offset [_OutlineOffsetFactor], [_OutlineOffsetUnits]
            BlendOp [_OutlineBlendOp], [_OutlineBlendOpAlpha]
            Blend [_OutlineSrcBlend] [_OutlineDstBlend], [_OutlineSrcBlendAlpha] [_OutlineDstBlendAlpha]

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
            #define LIL_OUTLINE
            #include "Includes/lil_pass_normal.hlsl"

            ENDHLSL
        }

        // ShadowCaster
        Pass
        {
            Name "SHADOW_CASTER"
            Tags {"LightMode" = "ShadowCaster"}
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
            #include "Includes/lil_pass_shadowcaster.hlsl"

            ENDHLSL
        }

        // DepthOnly
        Pass
        {
            Name "DEPTHONLY"
            Tags {"LightMode" = "DepthOnly"}
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
            #include "Includes/lil_pass_depthonly.hlsl"

            ENDHLSL
        }

        // DepthNormals
        Pass
        {
            Name "DEPTHNORMALS"
            Tags {"LightMode" = "DepthNormals"}
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
            #include "Includes/lil_pass_depthnormals.hlsl"

            ENDHLSL
        }

        // Universal2D
        Pass
        {
            Name "UNIVERSAL2D"
            Tags {"LightMode" = "Universal2D"}

            Stencil
            {
                Ref [_StencilRef]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
            }
            Cull [_Cull]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            ColorMask [_ColorMask]
            Offset [_OffsetFactor], [_OffsetUnits]
            BlendOp [_BlendOp], [_BlendOpAlpha]
            Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma only_renderers gles gles3 glcore d3d11

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_universal2d.hlsl"
            ENDHLSL
        }

        // Meta
        Pass
        {
            Name "META"
            Tags {"LightMode" = "Meta"}
            Cull Off

            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma only_renderers gles gles3 glcore d3d11

            //------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_meta.hlsl"
            ENDHLSL
        }
    }
*/
// URP End

}
