Shader "Hidden/ltspass_lite_cutout"
{
    HLSLINCLUDE
        #pragma exclude_renderers d3d11_9x
        #define LIL_RENDER 1
        #define LIL_LITE
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
            ZClip [_ZClip]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            ColorMask [_ColorMask]
            Offset [_OffsetFactor], [_OffsetUnits]
            BlendOp [_BlendOp], [_BlendOpAlpha]
            Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]
            AlphaToMask [_AlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2
            #pragma multi_compile_instancing
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma skip_variants SHADOWS_SCREEN DIRLIGHTMAP_COMBINED

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_forward.hlsl"

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
            ZClip [_OutlineZClip]
            ZWrite [_OutlineZWrite]
            ZTest [_OutlineZTest]
            ColorMask [_OutlineColorMask]
            Offset [_OutlineOffsetFactor], [_OutlineOffsetUnits]
            BlendOp [_OutlineBlendOp], [_OutlineBlendOpAlpha]
            Blend [_OutlineSrcBlend] [_OutlineDstBlend], [_OutlineSrcBlendAlpha] [_OutlineDstBlendAlpha]
            AlphaToMask [_OutlineAlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2
            #pragma multi_compile_instancing
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma skip_variants SHADOWS_SCREEN DIRLIGHTMAP_COMBINED

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_OUTLINE
            #include "Includes/lil_pass_forward.hlsl"

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
            ZClip [_ZClip]
			ZWrite Off
            ZTest LEqual
            ColorMask [_ColorMask]
            Offset [_OffsetFactor], [_OffsetUnits]
            Blend [_SrcBlendFA] [_DstBlendFA], Zero One
            BlendOp [_BlendOpFA], [_BlendOpAlphaFA]
            AlphaToMask [_AlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fragment POINT DIRECTIONAL SPOT POINT_COOKIE DIRECTIONAL_COOKIE
            #pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2
            #pragma multi_compile_instancing
            #pragma fragmentoption ARB_precision_hint_fastest

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_PASS_FORWARDADD
            #include "Includes/lil_pass_forward.hlsl"

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

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing

            //----------------------------------------------------------------------------------------------------------------------
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

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature EDITOR_VISUALIZATION

            //----------------------------------------------------------------------------------------------------------------------
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
    //----------------------------------------------------------------------------------------------------------------------
    // Lightweight Render Pipeline SM4.5
    SubShader
    {
        Tags{"ShaderModel" = "4.5"}
        HLSLINCLUDE
            #pragma target 4.5
        ENDHLSL

        // Forward
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
            ZClip [_ZClip]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            ColorMask [_ColorMask]
            Offset [_OffsetFactor], [_OffsetUnits]
            BlendOp [_BlendOp], [_BlendOpAlpha]
            Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]
            AlphaToMask [_AlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _MIXED_LIGHTING_SUBTRACTIVE
            #pragma multi_compile_fragment _ LIGHTMAP_ON
            #pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_forward.hlsl"

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
            ZClip [_OutlineZClip]
            ZWrite [_OutlineZWrite]
            ZTest [_OutlineZTest]
            ColorMask [_OutlineColorMask]
            Offset [_OutlineOffsetFactor], [_OutlineOffsetUnits]
            BlendOp [_OutlineBlendOp], [_OutlineBlendOpAlpha]
            Blend [_OutlineSrcBlend] [_OutlineDstBlend], [_OutlineSrcBlendAlpha] [_OutlineDstBlendAlpha]
            AlphaToMask [_OutlineAlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _MIXED_LIGHTING_SUBTRACTIVE
            #pragma multi_compile_fragment _ LIGHTMAP_ON
            #pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_OUTLINE
            #include "Includes/lil_pass_forward.hlsl"

            ENDHLSL
        }

        // ShadowCaster
        Pass
        {
            Name "SHADOW_CASTER"
            Tags {"LightMode" = "ShadowCaster"}
		    Cull [_Cull]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            //----------------------------------------------------------------------------------------------------------------------
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
            ZClip [_ZClip]
            ZWrite [_ZWrite]
            ZTest [_ZTest]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            //----------------------------------------------------------------------------------------------------------------------
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

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers gles gles3 glcore

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_meta.hlsl"
            ENDHLSL
        }
    }

    //----------------------------------------------------------------------------------------------------------------------
    // Lightweight Render Pipeline
    SubShader
    {
        // Forward
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
            ZClip [_ZClip]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            ColorMask [_ColorMask]
            Offset [_OffsetFactor], [_OffsetUnits]
            BlendOp [_BlendOp], [_BlendOpAlpha]
            Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]
            AlphaToMask [_AlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _MIXED_LIGHTING_SUBTRACTIVE
            #pragma multi_compile_fragment _ LIGHTMAP_ON
            #pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2
            #pragma multi_compile_instancing

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_forward.hlsl"

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
            ZClip [_OutlineZClip]
            ZWrite [_OutlineZWrite]
            ZTest [_OutlineZTest]
            ColorMask [_OutlineColorMask]
            Offset [_OutlineOffsetFactor], [_OutlineOffsetUnits]
            BlendOp [_OutlineBlendOp], [_OutlineBlendOpAlpha]
            Blend [_OutlineSrcBlend] [_OutlineDstBlend], [_OutlineSrcBlendAlpha] [_OutlineDstBlendAlpha]
            AlphaToMask [_OutlineAlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _MIXED_LIGHTING_SUBTRACTIVE
            #pragma multi_compile_fragment _ LIGHTMAP_ON
            #pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2
            #pragma multi_compile_instancing

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_OUTLINE
            #include "Includes/lil_pass_forward.hlsl"

            ENDHLSL
        }

        // ShadowCaster
        Pass
        {
            Name "SHADOW_CASTER"
            Tags {"LightMode" = "ShadowCaster"}
		    Cull [_Cull]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            #pragma multi_compile_instancing

            //----------------------------------------------------------------------------------------------------------------------
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
            ZClip [_ZClip]
            ZWrite [_ZWrite]
            ZTest [_ZTest]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile_instancing

            //----------------------------------------------------------------------------------------------------------------------
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

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma only_renderers gles gles3 glcore d3d11

            //----------------------------------------------------------------------------------------------------------------------
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
    //----------------------------------------------------------------------------------------------------------------------
    // Universal Render Pipeline SM4.5
    SubShader
    {
        Tags{"ShaderModel" = "4.5"}
        HLSLINCLUDE
            #pragma target 4.5
        ENDHLSL

        // Forward
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
            ZClip [_ZClip]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            ColorMask [_ColorMask]
            Offset [_OffsetFactor], [_OffsetUnits]
            BlendOp [_BlendOp], [_BlendOpAlpha]
            Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]
            AlphaToMask [_AlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _LIGHT_LAYERS
            #pragma multi_compile _ _CLUSTERED_RENDERING
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fragment _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile_fragment _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fragment _ LIGHTMAP_ON
            #pragma multi_compile_fragment _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_forward.hlsl"

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
            ZClip [_OutlineZClip]
            ZWrite [_OutlineZWrite]
            ZTest [_OutlineZTest]
            ColorMask [_OutlineColorMask]
            Offset [_OutlineOffsetFactor], [_OutlineOffsetUnits]
            BlendOp [_OutlineBlendOp], [_OutlineBlendOpAlpha]
            Blend [_OutlineSrcBlend] [_OutlineDstBlend], [_OutlineSrcBlendAlpha] [_OutlineDstBlendAlpha]
            AlphaToMask [_OutlineAlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _LIGHT_LAYERS
            #pragma multi_compile _ _CLUSTERED_RENDERING
            #pragma multi_compile_fragment _ LIGHTMAP_ON
            #pragma multi_compile_fragment _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_OUTLINE
            #include "Includes/lil_pass_forward.hlsl"

            ENDHLSL
        }

        // ShadowCaster
        Pass
        {
            Name "SHADOW_CASTER"
            Tags {"LightMode" = "ShadowCaster"}
		    Cull [_Cull]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            //----------------------------------------------------------------------------------------------------------------------
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
            ZClip [_ZClip]
            ZWrite [_ZWrite]
            ZTest [_ZTest]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            //----------------------------------------------------------------------------------------------------------------------
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
            ZClip [_ZClip]
            ZWrite [_ZWrite]
            ZTest [_ZTest]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            //----------------------------------------------------------------------------------------------------------------------
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
            ZClip [_ZClip]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            ColorMask [_ColorMask]
            Offset [_OffsetFactor], [_OffsetUnits]
            BlendOp [_BlendOp], [_BlendOpAlpha]
            Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers gles gles3 glcore

            //----------------------------------------------------------------------------------------------------------------------
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

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers gles gles3 glcore

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_meta.hlsl"
            ENDHLSL
        }
    }

    //----------------------------------------------------------------------------------------------------------------------
    // Universal Render Pipeline
    SubShader
    {
        // Forward
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
            ZClip [_ZClip]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            ColorMask [_ColorMask]
            Offset [_OffsetFactor], [_OffsetUnits]
            BlendOp [_BlendOp], [_BlendOpAlpha]
            Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]
            AlphaToMask [_AlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _LIGHT_LAYERS
            #pragma multi_compile _ _CLUSTERED_RENDERING
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fragment _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile_fragment _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fragment _ LIGHTMAP_ON
            #pragma multi_compile_fragment _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_forward.hlsl"

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
            ZClip [_OutlineZClip]
            ZWrite [_OutlineZWrite]
            ZTest [_OutlineZTest]
            ColorMask [_OutlineColorMask]
            Offset [_OutlineOffsetFactor], [_OutlineOffsetUnits]
            BlendOp [_OutlineBlendOp], [_OutlineBlendOpAlpha]
            Blend [_OutlineSrcBlend] [_OutlineDstBlend], [_OutlineSrcBlendAlpha] [_OutlineDstBlendAlpha]
            AlphaToMask [_OutlineAlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _LIGHT_LAYERS
            #pragma multi_compile _ _CLUSTERED_RENDERING
            #pragma multi_compile_fragment _ LIGHTMAP_ON
            #pragma multi_compile_fragment _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_OUTLINE
            #include "Includes/lil_pass_forward.hlsl"

            ENDHLSL
        }

        // ShadowCaster
        Pass
        {
            Name "SHADOW_CASTER"
            Tags {"LightMode" = "ShadowCaster"}
		    Cull [_Cull]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            #pragma multi_compile_instancing

            //----------------------------------------------------------------------------------------------------------------------
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
            ZClip [_ZClip]
            ZWrite [_ZWrite]
            ZTest [_ZTest]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile_instancing

            //----------------------------------------------------------------------------------------------------------------------
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
            ZClip [_ZClip]
            ZWrite [_ZWrite]
            ZTest [_ZTest]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile_instancing

            //----------------------------------------------------------------------------------------------------------------------
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
            ZClip [_ZClip]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            ColorMask [_ColorMask]
            Offset [_OffsetFactor], [_OffsetUnits]
            BlendOp [_BlendOp], [_BlendOpAlpha]
            Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma only_renderers gles gles3 glcore d3d11

            //----------------------------------------------------------------------------------------------------------------------
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

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma only_renderers gles gles3 glcore d3d11

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_meta.hlsl"
            ENDHLSL
        }
    }
*/
// URP End

//----------------------------------------------------------------------------------------------------------------------
// HDRP Start
/*
    //----------------------------------------------------------------------------------------------------------------------
    // High Definition Render Pipeline
    HLSLINCLUDE
        #pragma target 4.5
    ENDHLSL
    SubShader
    {
        Tags {"RenderPipeline"="HDRenderPipeline" "RenderType" = "HDLitShader"}
        // Forward
        Pass
        {
            Name "FORWARD"
            Tags {"LightMode" = "ForwardOnly"}

            Stencil
            {
                WriteMask 6
                Ref 0
                Comp Always
                Pass Replace
            }
            Cull [_Cull]
            ZClip [_ZClip]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            ColorMask [_ColorMask]
            Offset [_OffsetFactor], [_OffsetUnits]
            BlendOp [_BlendOp], [_BlendOpAlpha]
            Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]
            AlphaToMask [_AlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma multi_compile_fragment _ LIGHTMAP_ON
            #pragma multi_compile_fragment _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fragment _ SHADOWS_SHADOWMASK

            #define SHADERPASS SHADERPASS_FORWARD

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_forward.hlsl"

            ENDHLSL
        }

        // Forward Outline
        Pass
        {
            Name "FORWARD_OUTLINE"
            Tags {"LightMode" = "SRPDefaultUnlit"}

            Stencil
            {
                WriteMask 6
                Ref 0
                Comp Always
                Pass Replace
            }
            Cull [_OutlineCull]
            ZClip [_OutlineZClip]
            ZWrite [_OutlineZWrite]
            ZTest [_OutlineZTest]
            ColorMask [_OutlineColorMask]
            Offset [_OutlineOffsetFactor], [_OutlineOffsetUnits]
            BlendOp [_OutlineBlendOp], [_OutlineBlendOpAlpha]
            Blend [_OutlineSrcBlend] [_OutlineDstBlend], [_OutlineSrcBlendAlpha] [_OutlineDstBlendAlpha]
            AlphaToMask [_OutlineAlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma multi_compile_fragment _ LIGHTMAP_ON
            #pragma multi_compile_fragment _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fragment _ SHADOWS_SHADOWMASK

            #define SHADERPASS SHADERPASS_FORWARD

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_OUTLINE
            #include "Includes/lil_pass_forward.hlsl"

            ENDHLSL
        }

        // ShadowCaster
        Pass
        {
            Name "SHADOW_CASTER"
            Tags {"LightMode" = "ShadowCaster"}

            Cull[_Cull]
            ZClip [_ZClip]
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #define SHADERPASS SHADERPASS_SHADOWS

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_depthonly.hlsl"

            ENDHLSL
        }

        // DepthOnly
        Pass
        {
            Name "DEPTHONLY"
            Tags {"LightMode" = "DepthForwardOnly"}

            Stencil
            {
                WriteMask 8
                Ref 0
                Comp Always
                Pass Replace
            }
            Cull [_Cull]
            ZClip [_ZClip]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            Offset [_OffsetFactor], [_OffsetUnits]
            AlphaToMask [_AlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma multi_compile _ WRITE_NORMAL_BUFFER
            #pragma multi_compile _ WRITE_MSAA_DEPTH

            #define SHADERPASS SHADERPASS_DEPTH_ONLY

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_depthonly.hlsl"

            ENDHLSL
        }

        // DepthOnly Outline
        Pass
        {
            Name "DEPTHONLY_OUTLINE"
            Tags {"LightMode" = "DepthForwardOnly"}

            Stencil
            {
                WriteMask 8
                Ref 0
                Comp Always
                Pass Replace
            }
            Cull Back
            ZClip [_ZClip]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            Offset [_OffsetFactor], [_OffsetUnits]
            AlphaToMask [_AlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma require geometry
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma multi_compile _ WRITE_NORMAL_BUFFER
            #pragma multi_compile _ WRITE_MSAA_DEPTH

            #define SHADERPASS SHADERPASS_DEPTH_ONLY

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_ONEPASS_OUTLINE
            #include "Includes/lil_pass_depthonly.hlsl"

            ENDHLSL
        }

        // MotionVectors
        Pass
        {
            Name "MOTIONVECTORS"
            Tags {"LightMode" = "MotionVectors"}

            Stencil
            {
                WriteMask 40
                Ref 32
                Comp Always
                Pass Replace
            }
            Cull [_Cull]
            ZClip [_ZClip]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            Offset [_OffsetFactor], [_OffsetUnits]
            AlphaToMask [_AlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma multi_compile _ WRITE_NORMAL_BUFFER
            #pragma multi_compile _ WRITE_MSAA_DEPTH

            #define SHADERPASS SHADERPASS_MOTION_VECTORS

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_motionvectors.hlsl"

            ENDHLSL
        }

        // MotionVectors Outline
        Pass
        {
            Name "MOTIONVECTORS_OUTLINE"
            Tags {"LightMode" = "MotionVectors"}

            Stencil
            {
                WriteMask 40
                Ref 32
                Comp Always
                Pass Replace
            }
            Cull Back
            ZClip [_ZClip]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            Offset [_OffsetFactor], [_OffsetUnits]
            AlphaToMask [_AlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma require geometry
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma multi_compile _ WRITE_NORMAL_BUFFER
            #pragma multi_compile _ WRITE_MSAA_DEPTH

            #define SHADERPASS SHADERPASS_MOTION_VECTORS

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_ONEPASS_OUTLINE
            #include "Includes/lil_pass_motionvectors.hlsl"

            ENDHLSL
        }

        // Meta
        Pass
        {
            Name "META"
            Tags {"LightMode" = "META"}
            Cull Off

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #define SHADERPASS SHADERPASS_LIGHT_TRANSPORT

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_meta.hlsl"
            ENDHLSL
        }
    }
*/
// HDRP End

}
