Shader "_lil/[Optional] lilToonFurOnlyCutout"
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
        [lilToggle]                                     _AlphaToMask        ("AlphaToMask", Int) = 1

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
        [Enum(UnityEngine.Rendering.BlendMode)]         _FurSrcBlend            ("SrcBlend", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _FurDstBlend            ("DstBlend", Int) = 0
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
        [lilToggle]                                     _FurZWrite              ("ZWrite", Int) = 1
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
        [lilToggle]                                     _FurAlphaToMask         ("AlphaToMask", Int) = 1

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
        Tags {"RenderType" = "Transparent" "Queue" = "AlphaTest"}
        UsePass "Hidden/lilToonFurCutout/FORWARD_FUR"
        UsePass "Hidden/lilToonFurCutout/FORWARD_ADD_FUR"
    }
//
// BRP End

//----------------------------------------------------------------------------------------------------------------------
// LWRP Start
/*
    SubShader
    {
        Tags {"RenderType" = "Transparent" "Queue" = "AlphaTest"}
        UsePass "Hidden/lilToonFurCutout/FORWARD_FUR"
    }
*/
// LWRP End

//----------------------------------------------------------------------------------------------------------------------
// URP Start
/*
    SubShader
    {
        Tags {"RenderType" = "Transparent" "Queue" = "AlphaTest"}
        UsePass "Hidden/lilToonFurCutout/FORWARD_FUR"
    }
*/
// URP End

//----------------------------------------------------------------------------------------------------------------------
// HDRP Start
/*
    SubShader
    {
        Tags {"RenderPipeline"="HDRenderPipeline" "RenderType" = "HDLitShader" "Queue" = "AlphaTest"}
        UsePass "Hidden/lilToonFurCutout/FORWARD_FUR"
    }
*/
// HDRP End

    Fallback "Unlit/Texture"
    CustomEditor "lilToon.lilToonInspector"
}
