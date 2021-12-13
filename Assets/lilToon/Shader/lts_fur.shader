Shader "Hidden/lilToonFur"
{
    Properties
    {
        //----------------------------------------------------------------------------------------------------------------------
        // Base
        [lilToggle]     _Invisible                  ("Invisible", Int) = 0
                        _AsUnlit                    ("As Unlit", Range(0, 1)) = 0
                        _Cutoff                     ("Alpha Cutoff", Range(0,1)) = 0.001
                        _SubpassCutoff              ("Subpass Alpha Cutoff", Range(0,1)) = 0.5
        [lilToggle]     _FlipNormal                 ("Flip Backface Normal", Int) = 0
        [lilToggle]     _ShiftBackfaceUV            ("Shift Backface UV", Int) = 0
                        _BackfaceForceShadow        ("Backface Force Shadow", Range(0,1)) = 0
                        _VertexLightStrength        ("Vertex Light Strength", Range(0,1)) = 1
                        _LightMinLimit              ("Light Min Limit", Range(0,1)) = 0
                        _LightMaxLimit              ("Light Max Limit", Range(0,10)) = 1
                        _BeforeExposureLimit        ("Before Exposure Limit", Float) = 10000
                        _MonochromeLighting         ("Monochrome lighting", Range(0,1)) = 0
                        _lilDirectionalLightStrength ("Directional Light Strength", Range(0,1)) = 1
        [lilVec3]       _LightDirectionOverride     ("Light Direction Override", Vector) = (0,0.001,0,0)

        //----------------------------------------------------------------------------------------------------------------------
        // Main
        [lilHDR]        _Color                      ("Color", Color) = (1,1,1,1)
                        _MainTex                    ("Texture", 2D) = "white" {}
        [lilUVAnim]     _MainTex_ScrollRotate       ("Angle|UV Animation|Scroll|Rotate", Vector) = (0,0,0,0)
        [lilHSVG]       _MainTexHSVG                ("Hue|Saturation|Value|Gamma", Vector) = (0,1,1,1)

        //----------------------------------------------------------------------------------------------------------------------
        // Shadow
        [lilToggleLeft] _UseShadow                  ("Use Shadow", Int) = 0
        [lilToggle]     _ShadowReceive              ("Receive Shadow", Int) = 0
                        _ShadowStrength             ("Strength", Range(0, 1)) = 1
        [NoScaleOffset] _ShadowStrengthMask         ("Strength", 2D) = "white" {}
        [lilFFFF]       _ShadowAOShift              ("1st Scale|1st Offset|2nd Scale|2nd Offset", Vector) = (1,0,1,0)
                        _ShadowColor                ("Shadow Color", Color) = (0.7,0.75,0.85,1.0)
        [NoScaleOffset] _ShadowColorTex             ("Shadow Color", 2D) = "black" {}
                        _ShadowNormalStrength       ("Normal Strength", Range(0, 1)) = 1.0
                        _ShadowBorder               ("Border", Range(0, 1)) = 0.5
        [NoScaleOffset] _ShadowBorderMask           ("Border", 2D) = "white" {}
                        _ShadowBlur                 ("Blur", Range(0, 1)) = 0.1
        [NoScaleOffset] _ShadowBlurMask             ("Blur", 2D) = "white" {}
                        _Shadow2ndColor             ("Shadow 2nd Color", Color) = (0,0,0,0)
        [NoScaleOffset] _Shadow2ndColorTex          ("Shadow 2nd Color", 2D) = "black" {}
                        _Shadow2ndNormalStrength    ("Normal Strength", Range(0, 1)) = 1.0
                        _Shadow2ndBorder            ("2nd Border", Range(0, 1)) = 0.5
                        _Shadow2ndBlur              ("2nd Blur", Range(0, 1)) = 0.3
                        _ShadowMainStrength         ("Contrast", Range(0, 1)) = 1
                        _ShadowEnvStrength          ("Environment Strength", Range(0, 1)) = 0
                        _ShadowBorderColor          ("Border Color", Color) = (1,0,0,1)
                        _ShadowBorderRange          ("Border Range", Range(0, 1)) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Distance Fade
        [lilHDR]        _DistanceFadeColor          ("Color", Color) = (0,0,0,1)
        [lilFFFB]       _DistanceFade               ("Start|End|Strength|Fix backface", Vector) = (0.1,0.01,0,0)

        //----------------------------------------------------------------------------------------------------------------------
        // Encryption
        [lilToggle]     _IgnoreEncryption           ("Ignore Encryption", Int) = 0
                        _Keys                       ("Keys", Vector) = (0,0,0,0)

        //----------------------------------------------------------------------------------------------------------------------
        // Advanced
        [lilEnum]                                       _Cull               ("Cull Mode|Off|Front|Back", Int) = 2
        [Enum(UnityEngine.Rendering.BlendMode)]         _SrcBlend           ("SrcBlend", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _DstBlend           ("DstBlend", Int) = 10
        [Enum(UnityEngine.Rendering.BlendMode)]         _SrcBlendAlpha      ("SrcBlendAlpha", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _DstBlendAlpha      ("DstBlendAlpha", Int) = 10
        [Enum(UnityEngine.Rendering.BlendOp)]           _BlendOp            ("BlendOp", Int) = 0
        [Enum(UnityEngine.Rendering.BlendOp)]           _BlendOpAlpha       ("BlendOpAlpha", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]         _SrcBlendFA         ("ForwardAdd SrcBlend", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _DstBlendFA         ("ForwardAdd DstBlend", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _SrcBlendAlphaFA    ("ForwardAdd SrcBlendAlpha", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]         _DstBlendAlphaFA    ("ForwardAdd DstBlendAlpha", Int) = 1
        [Enum(UnityEngine.Rendering.BlendOp)]           _BlendOpFA          ("ForwardAdd BlendOp", Int) = 4
        [Enum(UnityEngine.Rendering.BlendOp)]           _BlendOpAlphaFA     ("ForwardAdd BlendOpAlpha", Int) = 4
        [lilToggle]                                     _ZClip              ("ZClip", Int) = 1
        [lilToggle]                                     _ZWrite             ("ZWrite", Int) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)]   _ZTest              ("ZTest", Int) = 4
        [IntRange]                                      _StencilRef         ("Stencil Reference Value", Range(0, 255)) = 0
        [IntRange]                                      _StencilReadMask    ("Stencil ReadMask Value", Range(0, 255)) = 255
        [IntRange]                                      _StencilWriteMask   ("Stencil WriteMask Value", Range(0, 255)) = 255
        [Enum(UnityEngine.Rendering.CompareFunction)]   _StencilComp        ("Stencil Compare Function", Float) = 8
        [Enum(UnityEngine.Rendering.StencilOp)]         _StencilPass        ("Stencil Pass", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]         _StencilFail        ("Stencil Fail", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]         _StencilZFail       ("Stencil ZFail", Float) = 0
                                                        _OffsetFactor       ("Offset Factor", Float) = 0
                                                        _OffsetUnits        ("Offset Units", Float) = 0
        [lilColorMask]                                  _ColorMask          ("Color Mask", Int) = 15
        [lilToggle]                                     _AlphaToMask        ("AlphaToMask", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Fur
                        _FurNoiseMask               ("Fur Noise", 2D) = "white" {}
        [NoScaleOffset] _FurMask                    ("Fur Mask", 2D) = "white" {}
        [NoScaleOffset] _FurLengthMask              ("Fur Length Mask", 2D) = "white" {}
        [NoScaleOffset][Normal] _FurVectorTex       ("Fur Vector", 2D) = "bump" {}
                        _FurVectorScale             ("Fur Vector scale", Range(-10,10)) = 1
        [lilVec3Float]  _FurVector                  ("Fur Vector|Fur Length", Vector) = (0.0,0.0,1.0,0.2)
        [lilToggle]     _VertexColor2FurVector      ("VertexColor->Vector", Int) = 0
                        _FurGravity                 ("Fur Gravity", Range(0,1)) = 0.25
                        _FurAO                      ("Fur AO", Range(0,1)) = 0
        [IntRange]      _FurLayerNum                ("Fur Layer Num", Range(1, 6)) = 2
                        _FurRootOffset              ("Fur Root Offset", Range(-1,0)) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Fur Advanced
        [lilCullMode]                                   _FurCull                ("Cull Mode|Off|Front|Back", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]         _FurSrcBlend            ("SrcBlend", Int) = 5
        [Enum(UnityEngine.Rendering.BlendMode)]         _FurDstBlend            ("DstBlend", Int) = 10
        [Enum(UnityEngine.Rendering.BlendMode)]         _FurSrcBlendAlpha       ("SrcBlendAlpha", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _FurDstBlendAlpha       ("DstBlendAlpha", Int) = 10
        [Enum(UnityEngine.Rendering.BlendOp)]           _FurBlendOp             ("BlendOp", Int) = 0
        [Enum(UnityEngine.Rendering.BlendOp)]           _FurBlendOpAlpha        ("BlendOpAlpha", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]         _FurSrcBlendFA          ("ForwardAdd SrcBlend", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _FurDstBlendFA          ("ForwardAdd DstBlend", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _FurSrcBlendAlphaFA     ("ForwardAdd SrcBlendAlpha", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]         _FurDstBlendAlphaFA     ("ForwardAdd DstBlendAlpha", Int) = 1
        [Enum(UnityEngine.Rendering.BlendOp)]           _FurBlendOpFA           ("ForwardAdd BlendOp", Int) = 4
        [Enum(UnityEngine.Rendering.BlendOp)]           _FurBlendOpAlphaFA      ("ForwardAdd BlendOpAlpha", Int) = 4
        [lilToggle]                                     _FurZClip               ("ZClip", Int) = 1
        [lilToggle]                                     _FurZWrite              ("ZWrite", Int) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]   _FurZTest               ("ZTest", Int) = 4
        [IntRange]                                      _FurStencilRef          ("Stencil Reference Value", Range(0, 255)) = 0
        [IntRange]                                      _FurStencilReadMask     ("Stencil ReadMask Value", Range(0, 255)) = 255
        [IntRange]                                      _FurStencilWriteMask    ("Stencil WriteMask Value", Range(0, 255)) = 255
        [Enum(UnityEngine.Rendering.CompareFunction)]   _FurStencilComp         ("Stencil Compare Function", Float) = 8
        [Enum(UnityEngine.Rendering.StencilOp)]         _FurStencilPass         ("Stencil Pass", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]         _FurStencilFail         ("Stencil Fail", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]         _FurStencilZFail        ("Stencil ZFail", Float) = 0
                                                        _FurOffsetFactor        ("Offset Factor", Float) = 0
                                                        _FurOffsetUnits         ("Offset Units", Float) = 0
        [lilColorMask]                                  _FurColorMask           ("Color Mask", Int) = 15
        [lilToggle]                                     _FurAlphaToMask         ("AlphaToMask", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Save (Unused)
        [HideInInspector] [MainColor]                   _BaseColor          ("Color", Color) = (1,1,1,1)
        [HideInInspector] [MainTexture]                 _BaseMap            ("Texture", 2D) = "white" {}
        [HideInInspector]                               _BaseColorMap       ("Texture", 2D) = "white" {}
    }
    HLSLINCLUDE
        #pragma require geometry
        #define LIL_RENDER 2
        #define LIL_FUR
    ENDHLSL

//----------------------------------------------------------------------------------------------------------------------
// BRP Start
//
    SubShader
    {
        Tags {"RenderType" = "TransparentCutout" "Queue" = "Transparent"}

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

        // Forward Fur
        Pass
        {
            Name "FORWARD_FUR"
            Tags {"LightMode" = "ForwardBase"}

            Stencil
            {
                Ref [_FurStencilRef]
                ReadMask [_FurStencilReadMask]
                WriteMask [_FurStencilWriteMask]
                Comp [_FurStencilComp]
                Pass [_FurStencilPass]
                Fail [_FurStencilFail]
                ZFail [_FurStencilZFail]
            }
            Cull [_FurCull]
            ZClip [_FurZClip]
            ZWrite [_FurZWrite]
            ZTest [_FurZTest]
            ColorMask [_FurColorMask]
            Offset [_FurOffsetFactor], [_FurOffsetUnits]
            BlendOp [_FurBlendOp], [_FurBlendOpAlpha]
            Blend [_FurSrcBlend] [_FurDstBlend], [_FurSrcBlendAlpha] [_FurDstBlendAlpha]
            AlphaToMask [_FurAlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2
            #pragma multi_compile_instancing
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma skip_variants SHADOWS_SCREEN

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_forward_fur.hlsl"

            ENDHLSL
        }

        UsePass "Hidden/ltspass_transparent/SHADOW_CASTER"

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
        Tags {"RenderType" = "TransparentCutout" "Queue" = "Transparent" "ShaderModel" = "4.5"}
        HLSLINCLUDE
            #pragma target 4.5
        ENDHLSL

        // Forward
        Pass
        {
            Name "FORWARD"
            Tags {"LightMode" = "SRPDefaultUnlit"}

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

        // Fur
        Pass
        {
            Name "FORWARD_FUR"
            Tags {"LightMode" = "LightweightForward"}

            Stencil
            {
                Ref [_FurStencilRef]
                ReadMask [_FurStencilReadMask]
                WriteMask [_FurStencilWriteMask]
                Comp [_FurStencilComp]
                Pass [_FurStencilPass]
                Fail [_FurStencilFail]
                ZFail [_FurStencilZFail]
            }
            Cull [_FurCull]
            ZClip [_FurZClip]
            ZWrite [_FurZWrite]
            ZTest [_FurZTest]
            ColorMask [_FurColorMask]
            Offset [_FurOffsetFactor], [_FurOffsetUnits]
            BlendOp [_FurBlendOp], [_FurBlendOpAlpha]
            Blend [_FurSrcBlend] [_FurDstBlend], [_FurSrcBlendAlpha] [_FurDstBlendAlpha]
            AlphaToMask [_FurAlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma geometry geom
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
            #include "Includes/lil_pass_forward_fur.hlsl"

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
        Tags {"RenderType" = "TransparentCutout" "Queue" = "Transparent"}

        // Forward
        Pass
        {
            Name "FORWARD"
            Tags {"LightMode" = "SRPDefaultUnlit"}

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

        // Fur
        Pass
        {
            Name "FORWARD_FUR"
            Tags {"LightMode" = "LightweightForward"}

            Stencil
            {
                Ref [_FurStencilRef]
                ReadMask [_FurStencilReadMask]
                WriteMask [_FurStencilWriteMask]
                Comp [_FurStencilComp]
                Pass [_FurStencilPass]
                Fail [_FurStencilFail]
                ZFail [_FurStencilZFail]
            }
            Cull [_FurCull]
            ZClip [_FurZClip]
            ZWrite [_FurZWrite]
            ZTest [_FurZTest]
            ColorMask [_FurColorMask]
            Offset [_FurOffsetFactor], [_FurOffsetUnits]
            BlendOp [_FurBlendOp], [_FurBlendOpAlpha]
            Blend [_FurSrcBlend] [_FurDstBlend], [_FurSrcBlendAlpha] [_FurDstBlendAlpha]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _MIXED_LIGHTING_SUBTRACTIVE
            #pragma multi_compile_fragment _ LIGHTMAP_ON
            #pragma multi_compile_vertex _ FOG_LINEAR FOG_EXP FOG_EXP2
            #pragma multi_compile_instancing

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pass_forward_fur.hlsl"

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
        Tags {"RenderType" = "TransparentCutout" "Queue" = "Transparent" "ShaderModel" = "4.5"}
        HLSLINCLUDE
            #pragma target 4.5
        ENDHLSL

        // Forward
        Pass
        {
            Name "FORWARD"
            Tags {"LightMode" = "SRPDefaultUnlit"}

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

        // Fur
        Pass
        {
            Name "FORWARD_FUR"
            Tags {"LightMode" = "UniversalForward"}

            Stencil
            {
                Ref [_FurStencilRef]
                ReadMask [_FurStencilReadMask]
                WriteMask [_FurStencilWriteMask]
                Comp [_FurStencilComp]
                Pass [_FurStencilPass]
                Fail [_FurStencilFail]
                ZFail [_FurStencilZFail]
            }
            Cull [_FurCull]
            ZClip [_FurZClip]
            ZWrite [_FurZWrite]
            ZTest [_FurZTest]
            ColorMask [_FurColorMask]
            Offset [_FurOffsetFactor], [_FurOffsetUnits]
            BlendOp [_FurBlendOp], [_FurBlendOpAlpha]
            Blend [_FurSrcBlend] [_FurDstBlend], [_FurSrcBlendAlpha] [_FurDstBlendAlpha]
            AlphaToMask [_FurAlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma geometry geom
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
            #include "Includes/lil_pass_forward_fur.hlsl"

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
        Tags {"RenderType" = "TransparentCutout" "Queue" = "Transparent"}

        // Forward
        Pass
        {
            Name "FORWARD"
            Tags {"LightMode" = "SRPDefaultUnlit"}

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

        // Fur
        Pass
        {
            Name "FORWARD_FUR"
            Tags {"LightMode" = "UniversalForward"}

            Stencil
            {
                Ref [_FurStencilRef]
                ReadMask [_FurStencilReadMask]
                WriteMask [_FurStencilWriteMask]
                Comp [_FurStencilComp]
                Pass [_FurStencilPass]
                Fail [_FurStencilFail]
                ZFail [_FurStencilZFail]
            }
            Cull [_FurCull]
            ZClip [_FurZClip]
            ZWrite [_FurZWrite]
            ZTest [_FurZTest]
            ColorMask [_FurColorMask]
            Offset [_FurOffsetFactor], [_FurOffsetUnits]
            BlendOp [_FurBlendOp], [_FurBlendOpAlpha]
            Blend [_FurSrcBlend] [_FurDstBlend], [_FurSrcBlendAlpha] [_FurDstBlendAlpha]
            AlphaToMask [_FurAlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma geometry geom
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
            #include "Includes/lil_pass_forward_fur.hlsl"

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
        Tags {"RenderPipeline"="HDRenderPipeline" "RenderType" = "HDLitShader" "Queue" = "Transparent"}

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

        // Fur
        Pass
        {
            Name "FORWARD_FUR"
            Tags {"LightMode" = "SRPDefaultUnlit"}

            Stencil
            {
                WriteMask 6
                Ref 0
                Comp Always
                Pass Replace
            }
            Cull [_FurCull]
            ZClip [_FurZClip]
            ZWrite [_FurZWrite]
            ZTest [_FurZTest]
            ColorMask [_FurColorMask]
            Offset [_FurOffsetFactor], [_FurOffsetUnits]
            BlendOp [_FurBlendOp], [_FurBlendOpAlpha]
            Blend [_FurSrcBlend] [_FurDstBlend], [_FurSrcBlendAlpha] [_FurDstBlendAlpha]
            AlphaToMask [_FurAlphaToMask]

            HLSLPROGRAM

            //----------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma geometry geom
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
            #include "Includes/lil_pass_forward_fur.hlsl"

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
            #pragma geometry geom
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #define SHADERPASS SHADERPASS_SHADOWS

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_ONEPASS_FUR
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
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma multi_compile _ WRITE_NORMAL_BUFFER
            #pragma multi_compile _ WRITE_MSAA_DEPTH

            #define SHADERPASS SHADERPASS_DEPTH_ONLY

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_ONEPASS_FUR
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
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma multi_compile _ WRITE_NORMAL_BUFFER
            #pragma multi_compile _ WRITE_MSAA_DEPTH

            #define SHADERPASS SHADERPASS_MOTION_VECTORS

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #define LIL_ONEPASS_FUR
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

    Fallback "Unlit/Texture"
    CustomEditor "lilToon.lilToonInspector"
}
