Shader "Hidden/lilToonLiteOutline"
{
    Properties
    {
        //----------------------------------------------------------------------------------------------------------------------
        // Base
        [lilToggle]     _Invisible                  ("Invisible", Int) = 0
                        _AsUnlit                    ("As Unlit", Range(0, 1)) = 0
                        _Cutoff                     ("Alpha Cutoff", Range(0,1)) = 0.5
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
        [NoScaleOffset] _TriMask                    ("TriMask", 2D) = "white" {}

        //----------------------------------------------------------------------------------------------------------------------
        // Main
        [lilHDR]        _Color                      ("Color", Color) = (1,1,1,1)
                        _MainTex                    ("Texture", 2D) = "white" {}
        [lilUVAnim]     _MainTex_ScrollRotate       ("Angle|UV Animation|Scroll|Rotate", Vector) = (0,0,0,0)

        //----------------------------------------------------------------------------------------------------------------------
        // Shadow
        [lilToggleLeft] _UseShadow                  ("Use Shadow", Int) = 0
                        _ShadowBorder               ("Border", Range(0, 1)) = 0.5
                        _ShadowBlur                 ("Blur", Range(0, 1)) = 0.1
        [NoScaleOffset] _ShadowColorTex             ("Shadow Color", 2D) = "black" {}
                        _Shadow2ndBorder            ("2nd Border", Range(0, 1)) = 0.5
                        _Shadow2ndBlur              ("2nd Blur", Range(0, 1)) = 0.3
        [NoScaleOffset] _Shadow2ndColorTex          ("Shadow 2nd Color", 2D) = "black" {}
                        _ShadowEnvStrength          ("Environment Strength", Range(0, 1)) = 0
                        _ShadowBorderColor          ("Border Color", Color) = (1,0,0,1)
                        _ShadowBorderRange          ("Border Range", Range(0, 1)) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // MatCap
        [lilToggleLeft] _UseMatCap                  ("Use MatCap", Int) = 0
                        _MatCapTex                  ("Texture", 2D) = "white" {}
        [lilVec2R]      _MatCapBlendUV1             ("Blend UV1", Vector) = (0,0,0,0)
        [lilToggle]     _MatCapZRotCancel           ("Z-axis rotation cancellation", Int) = 1
        [lilToggle]     _MatCapPerspective          ("Fix Perspective", Int) = 1
                        _MatCapVRParallaxStrength   ("VR Parallax Strength", Range(0, 1)) = 1
        [lilToggle]     _MatCapMul                  ("Multiply", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Rim
        [lilToggleLeft] _UseRim                     ("Use Rim", Int) = 0
        [lilHDR]        _RimColor                   ("Color", Color) = (1,1,1,1)
                        _RimBorder                  ("Border", Range(0, 1)) = 0.5
                        _RimBlur                    ("Blur", Range(0, 1)) = 0.1
        [PowerSlider(3.0)]_RimFresnelPower          ("Fresnel Power", Range(0.01, 50)) = 3.0
                        _RimShadowMask              ("Shadow Mask", Range(0, 1)) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Emmision
        [lilToggleLeft] _UseEmission                ("Use Emission", Int) = 0
        [HDR][lilHDR]   _EmissionColor              ("Color", Color) = (1,1,1,1)
                        _EmissionMap                ("Texture", 2D) = "white" {}
        [lilUVAnim]     _EmissionMap_ScrollRotate   ("Angle|UV Animation|Scroll|Rotate", Vector) = (0,0,0,0)
        [lilEnum]       _EmissionMap_UVMode         ("UV Mode|UV0|UV1|UV2|UV3|Rim", Int) = 0
        [lilBlink]      _EmissionBlink              ("Blink Strength|Blink Type|Blink Speed|Blink Offset", Vector) = (0,0,3.141593,0)

        //----------------------------------------------------------------------------------------------------------------------
        // Advanced
        [lilEnum]                                       _Cull               ("Cull Mode|Off|Front|Back", Int) = 2
        [Enum(UnityEngine.Rendering.BlendMode)]         _SrcBlend           ("SrcBlend", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _DstBlend           ("DstBlend", Int) = 0
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
        // Outline
        [lilHDR]        _OutlineColor               ("Outline Color", Color) = (0.8,0.85,0.9,1)
                        _OutlineTex                 ("Texture", 2D) = "white" {}
        [lilUVAnim]     _OutlineTex_ScrollRotate    ("Angle|UV Animation|Scroll|Rotate", Vector) = (0,0,0,0)
        [lilOLWidth]    _OutlineWidth               ("Width", Range(0,1)) = 0.05
        [NoScaleOffset] _OutlineWidthMask           ("Width", 2D) = "white" {}
        [lilToggle]     _OutlineFixWidth            ("Fix Width", Int) = 1
        [lilToggle]     _OutlineVertexR2Width       ("Vertex R -> Width", Int) = 0
                        _OutlineEnableLighting      ("Enable Lighting", Range(0, 1)) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // Outline Advanced
        [lilEnum]                                       _OutlineCull                ("Cull Mode|Off|Front|Back", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _OutlineSrcBlend            ("SrcBlend", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _OutlineDstBlend            ("DstBlend", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]         _OutlineSrcBlendAlpha       ("SrcBlendAlpha", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _OutlineDstBlendAlpha       ("DstBlendAlpha", Int) = 10
        [Enum(UnityEngine.Rendering.BlendOp)]           _OutlineBlendOp             ("BlendOp", Int) = 0
        [Enum(UnityEngine.Rendering.BlendOp)]           _OutlineBlendOpAlpha        ("BlendOpAlpha", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]         _OutlineSrcBlendFA          ("ForwardAdd SrcBlend", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _OutlineDstBlendFA          ("ForwardAdd DstBlend", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _OutlineSrcBlendAlphaFA     ("ForwardAdd SrcBlendAlpha", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]         _OutlineDstBlendAlphaFA     ("ForwardAdd DstBlendAlpha", Int) = 1
        [Enum(UnityEngine.Rendering.BlendOp)]           _OutlineBlendOpFA           ("ForwardAdd BlendOp", Int) = 4
        [Enum(UnityEngine.Rendering.BlendOp)]           _OutlineBlendOpAlphaFA      ("ForwardAdd BlendOpAlpha", Int) = 4
        [lilToggle]                                     _OutlineZClip               ("ZClip", Int) = 1
        [lilToggle]                                     _OutlineZWrite              ("ZWrite", Int) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)]   _OutlineZTest               ("ZTest", Int) = 2
        [IntRange]                                      _OutlineStencilRef          ("Stencil Reference Value", Range(0, 255)) = 0
        [IntRange]                                      _OutlineStencilReadMask     ("Stencil ReadMask Value", Range(0, 255)) = 255
        [IntRange]                                      _OutlineStencilWriteMask    ("Stencil WriteMask Value", Range(0, 255)) = 255
        [Enum(UnityEngine.Rendering.CompareFunction)]   _OutlineStencilComp         ("Stencil Compare Function", Float) = 8
        [Enum(UnityEngine.Rendering.StencilOp)]         _OutlineStencilPass         ("Stencil Pass", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]         _OutlineStencilFail         ("Stencil Fail", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]         _OutlineStencilZFail        ("Stencil ZFail", Float) = 0
                                                        _OutlineOffsetFactor        ("Offset Factor", Float) = 0
                                                        _OutlineOffsetUnits         ("Offset Units", Float) = 0
        [lilColorMask]                                  _OutlineColorMask           ("Color Mask", Int) = 15
        [lilToggle]                                     _OutlineAlphaToMask         ("AlphaToMask", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Save (Unused)
        [HideInInspector] [MainColor]                   _BaseColor          ("Color", Color) = (1,1,1,1)
        [HideInInspector] [MainTexture]                 _BaseMap            ("Texture", 2D) = "white" {}
        [HideInInspector]                               _BaseColorMap       ("Texture", 2D) = "white" {}
    }

//----------------------------------------------------------------------------------------------------------------------
// BRP Start
//
    SubShader
    {
        Tags {"RenderType" = "Opaque" "Queue" = "Geometry"}
        UsePass "Hidden/ltspass_lite_opaque/FORWARD"
        UsePass "Hidden/ltspass_lite_opaque/FORWARD_OUTLINE"
        UsePass "Hidden/ltspass_lite_opaque/FORWARD_ADD"
        UsePass "Hidden/ltspass_lite_opaque/SHADOW_CASTER"
        UsePass "Hidden/ltspass_lite_opaque/META"
    }
//
// BRP End

//----------------------------------------------------------------------------------------------------------------------
// LWRP Start
/*
    SubShader
    {
        Tags {"RenderType" = "Opaque" "Queue" = "Geometry"}
        UsePass "Hidden/ltspass_lite_opaque/FORWARD"
        UsePass "Hidden/ltspass_lite_opaque/FORWARD_OUTLINE"
        UsePass "Hidden/ltspass_lite_opaque/SHADOW_CASTER"
        UsePass "Hidden/ltspass_lite_opaque/DEPTHONLY"
        UsePass "Hidden/ltspass_lite_opaque/META"
    }
*/
// LWRP End

//----------------------------------------------------------------------------------------------------------------------
// URP Start
/*
    SubShader
    {
        Tags {"RenderType" = "Opaque" "Queue" = "Geometry"}
        UsePass "Hidden/ltspass_lite_opaque/FORWARD"
        UsePass "Hidden/ltspass_lite_opaque/FORWARD_OUTLINE"
        UsePass "Hidden/ltspass_lite_opaque/SHADOW_CASTER"
        UsePass "Hidden/ltspass_lite_opaque/DEPTHONLY"
        UsePass "Hidden/ltspass_lite_opaque/DEPTHNORMALS"
        UsePass "Hidden/ltspass_lite_opaque/UNIVERSAL2D"
        UsePass "Hidden/ltspass_lite_opaque/META"
    }
*/
// URP End

//----------------------------------------------------------------------------------------------------------------------
// HDRP Start
/*
    SubShader
    {
        Tags {"RenderPipeline"="HDRenderPipeline" "RenderType" = "HDLitShader" "Queue" = "Geometry"}
        UsePass "Hidden/ltspass_lite_opaque/FORWARD"
        UsePass "Hidden/ltspass_lite_opaque/FORWARD_OUTLINE"
        UsePass "Hidden/ltspass_lite_opaque/SHADOW_CASTER"
        UsePass "Hidden/ltspass_lite_opaque/DEPTHONLY_OUTLINE"
        UsePass "Hidden/ltspass_lite_opaque/MOTIONVECTORS_OUTLINE"
        UsePass "Hidden/ltspass_lite_opaque/META"
    }
*/
// HDRP End

    Fallback "Unlit/Texture"
    CustomEditor "lilToon.lilToonInspector"
}
