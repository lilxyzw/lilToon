Shader "Hidden/lilToonLiteTwoPassTransparentOutline"
{
    Properties
    {
        //----------------------------------------------------------------------------------------------------------------------
        // Base
        [lilToggle]     _Invisible                  ("sInvisible", Int) = 0
                        _AsUnlit                    ("sAsUnlit", Range(0, 1)) = 0
                        _Cutoff                     ("sCutoff", Range(-0.001,1.001)) = 0.5
                        _SubpassCutoff              ("sSubpassCutoff", Range(0,1)) = 0.5
        [lilToggle]     _FlipNormal                 ("sFlipBackfaceNormal", Int) = 0
        [lilToggle]     _ShiftBackfaceUV            ("sShiftBackfaceUV", Int) = 0
                        _BackfaceForceShadow        ("sBackfaceForceShadow", Range(0,1)) = 0
                        _VertexLightStrength        ("sVertexLightStrength", Range(0,1)) = 0
                        _LightMinLimit              ("sLightMinLimit", Range(0,1)) = 0.05
                        _LightMaxLimit              ("sLightMaxLimit", Range(0,10)) = 1
                        _BeforeExposureLimit        ("sBeforeExposureLimit", Float) = 10000
                        _MonochromeLighting         ("sMonochromeLighting", Range(0,1)) = 0
                        _AlphaBoostFA               ("sAlphaBoostFA", Range(1,100)) = 10
                        _lilDirectionalLightStrength ("sDirectionalLightStrength", Range(0,1)) = 1
        [lilVec3B]      _LightDirectionOverride     ("sLightDirectionOverrides", Vector) = (0.001,0.002,0.001,0)
                        _AAStrength                 ("sAAShading", Range(0, 1)) = 1
        [NoScaleOffset] _TriMask                    ("TriMask", 2D) = "white" {}

        //----------------------------------------------------------------------------------------------------------------------
        // Main
        [lilHDR] [MainColor] _Color                 ("sColor", Color) = (1,1,1,1)
        [MainTexture]   _MainTex                    ("Texture", 2D) = "white" {}
        [lilUVAnim]     _MainTex_ScrollRotate       ("sScrollRotates", Vector) = (0,0,0,0)

        //----------------------------------------------------------------------------------------------------------------------
        // Shadow
        [lilToggleLeft] _UseShadow                  ("sShadow", Int) = 0
                        _ShadowBorder               ("sBorder", Range(0, 1)) = 0.5
                        _ShadowBlur                 ("sBlur", Range(0, 1)) = 0.1
        [NoScaleOffset] _ShadowColorTex             ("Shadow Color", 2D) = "black" {}
                        _Shadow2ndBorder            ("sBorder", Range(0, 1)) = 0.5
                        _Shadow2ndBlur              ("sBlur", Range(0, 1)) = 0.3
        [NoScaleOffset] _Shadow2ndColorTex          ("Shadow 2nd Color", 2D) = "black" {}
                        _ShadowEnvStrength          ("sShadowEnvStrength", Range(0, 1)) = 0
                        _ShadowBorderColor          ("sShadowBorderColor", Color) = (1,0,0,1)
                        _ShadowBorderRange          ("sShadowBorderRange", Range(0, 1)) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // MatCap
        [lilToggleLeft] _UseMatCap                  ("sMatCap", Int) = 0
                        _MatCapTex                  ("Texture", 2D) = "white" {}
        [lilVec2R]      _MatCapBlendUV1             ("sBlendUV1", Vector) = (0,0,0,0)
        [lilToggle]     _MatCapZRotCancel           ("sMatCapZRotCancel", Int) = 1
        [lilToggle]     _MatCapPerspective          ("sFixPerspective", Int) = 1
                        _MatCapVRParallaxStrength   ("sVRParallaxStrength", Range(0, 1)) = 1
        [lilToggle]     _MatCapMul                  ("Multiply", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Rim
        [lilToggleLeft] _UseRim                     ("sRimLight", Int) = 0
        [lilHDR]        _RimColor                   ("sColor", Color) = (1,1,1,1)
                        _RimBorder                  ("sBorder", Range(0, 1)) = 0.5
                        _RimBlur                    ("sBlur", Range(0, 1)) = 0.1
        [PowerSlider(3.0)]_RimFresnelPower          ("sFresnelPower", Range(0.01, 50)) = 3.0
                        _RimShadowMask              ("sShadowMask", Range(0, 1)) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Emmision
        [lilToggleLeft] _UseEmission                ("Use Emission", Int) = 0
        [HDR][lilHDR]   _EmissionColor              ("sColor", Color) = (1,1,1,1)
                        _EmissionMap                ("Texture", 2D) = "white" {}
        [lilUVAnim]     _EmissionMap_ScrollRotate   ("sScrollRotates", Vector) = (0,0,0,0)
        [lilEnum]       _EmissionMap_UVMode         ("UV Mode|UV0|UV1|UV2|UV3|Rim", Int) = 0
        [lilBlink]      _EmissionBlink              ("sBlinkSettings", Vector) = (0,0,3.141593,0)

        //----------------------------------------------------------------------------------------------------------------------
        // Outline
        [lilHDR]        _OutlineColor               ("sColor", Color) = (0.8,0.85,0.9,1)
                        _OutlineTex                 ("Texture", 2D) = "white" {}
        [lilUVAnim]     _OutlineTex_ScrollRotate    ("sScrollRotates", Vector) = (0,0,0,0)
        [lilOLWidth]    _OutlineWidth               ("Width", Range(0,1)) = 0.05
        [NoScaleOffset] _OutlineWidthMask           ("Width", 2D) = "white" {}
                        _OutlineFixWidth            ("sFixWidth", Range(0,1)) = 1
        [lilEnum]       _OutlineVertexR2Width       ("sOutlineVertexColorUsages", Int) = 0
        [lilToggle]     _OutlineDeleteMesh          ("sDeleteMesh0", Int) = 0
                        _OutlineEnableLighting      ("sEnableLighting", Range(0, 1)) = 1
                        _OutlineZBias               ("Z Bias", Float) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Save (Unused)
        [HideInInspector]                               _BaseColor          ("sColor", Color) = (1,1,1,1)
        [HideInInspector]                               _BaseMap            ("Texture", 2D) = "white" {}
        [HideInInspector]                               _BaseColorMap       ("Texture", 2D) = "white" {}
        [HideInInspector]                               _lilToonVersion     ("Version", Int) = 44

        //----------------------------------------------------------------------------------------------------------------------
        // Advanced
        [lilEnum]                                       _Cull               ("Cull Mode|Off|Front|Back", Int) = 2
        [Enum(UnityEngine.Rendering.BlendMode)]         _SrcBlend           ("sSrcBlendRGB", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _DstBlend           ("sDstBlendRGB", Int) = 10
        [Enum(UnityEngine.Rendering.BlendMode)]         _SrcBlendAlpha      ("sSrcBlendAlpha", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _DstBlendAlpha      ("sDstBlendAlpha", Int) = 10
        [Enum(UnityEngine.Rendering.BlendOp)]           _BlendOp            ("sBlendOpRGB", Int) = 0
        [Enum(UnityEngine.Rendering.BlendOp)]           _BlendOpAlpha       ("sBlendOpAlpha", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]         _SrcBlendFA         ("sSrcBlendRGB", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _DstBlendFA         ("sDstBlendRGB", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _SrcBlendAlphaFA    ("sSrcBlendAlpha", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]         _DstBlendAlphaFA    ("sDstBlendAlpha", Int) = 1
        [Enum(UnityEngine.Rendering.BlendOp)]           _BlendOpFA          ("sBlendOpRGB", Int) = 4
        [Enum(UnityEngine.Rendering.BlendOp)]           _BlendOpAlphaFA     ("sBlendOpAlpha", Int) = 4
        [lilToggle]                                     _ZClip              ("sZClip", Int) = 1
        [lilToggle]                                     _ZWrite             ("sZWrite", Int) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)]   _ZTest              ("sZTest", Int) = 4
        [IntRange]                                      _StencilRef         ("Ref", Range(0, 255)) = 0
        [IntRange]                                      _StencilReadMask    ("ReadMask", Range(0, 255)) = 255
        [IntRange]                                      _StencilWriteMask   ("WriteMask", Range(0, 255)) = 255
        [Enum(UnityEngine.Rendering.CompareFunction)]   _StencilComp        ("Comp", Float) = 8
        [Enum(UnityEngine.Rendering.StencilOp)]         _StencilPass        ("Pass", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]         _StencilFail        ("Fail", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]         _StencilZFail       ("ZFail", Float) = 0
                                                        _OffsetFactor       ("sOffsetFactor", Float) = 0
                                                        _OffsetUnits        ("sOffsetUnits", Float) = 0
        [lilColorMask]                                  _ColorMask          ("sColorMask", Int) = 15
        [lilToggle]                                     _AlphaToMask        ("sAlphaToMask", Int) = 0
                                                        _lilShadowCasterBias ("Shadow Caster Bias", Float) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Outline Advanced
        [lilEnum]                                       _OutlineCull                ("Cull Mode|Off|Front|Back", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _OutlineSrcBlend            ("sSrcBlendRGB", Int) = 5
        [Enum(UnityEngine.Rendering.BlendMode)]         _OutlineDstBlend            ("sDstBlendRGB", Int) = 10
        [Enum(UnityEngine.Rendering.BlendMode)]         _OutlineSrcBlendAlpha       ("sSrcBlendAlpha", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _OutlineDstBlendAlpha       ("sDstBlendAlpha", Int) = 10
        [Enum(UnityEngine.Rendering.BlendOp)]           _OutlineBlendOp             ("sBlendOpRGB", Int) = 0
        [Enum(UnityEngine.Rendering.BlendOp)]           _OutlineBlendOpAlpha        ("sBlendOpAlpha", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]         _OutlineSrcBlendFA          ("sSrcBlendRGB", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _OutlineDstBlendFA          ("sDstBlendRGB", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _OutlineSrcBlendAlphaFA     ("sSrcBlendAlpha", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]         _OutlineDstBlendAlphaFA     ("sDstBlendAlpha", Int) = 1
        [Enum(UnityEngine.Rendering.BlendOp)]           _OutlineBlendOpFA           ("sBlendOpRGB", Int) = 4
        [Enum(UnityEngine.Rendering.BlendOp)]           _OutlineBlendOpAlphaFA      ("sBlendOpAlpha", Int) = 4
        [lilToggle]                                     _OutlineZClip               ("sZClip", Int) = 1
        [lilToggle]                                     _OutlineZWrite              ("sZWrite", Int) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)]   _OutlineZTest               ("sZTest", Int) = 2
        [IntRange]                                      _OutlineStencilRef          ("Ref", Range(0, 255)) = 0
        [IntRange]                                      _OutlineStencilReadMask     ("ReadMask", Range(0, 255)) = 255
        [IntRange]                                      _OutlineStencilWriteMask    ("WriteMask", Range(0, 255)) = 255
        [Enum(UnityEngine.Rendering.CompareFunction)]   _OutlineStencilComp         ("Comp", Float) = 8
        [Enum(UnityEngine.Rendering.StencilOp)]         _OutlineStencilPass         ("Pass", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]         _OutlineStencilFail         ("Fail", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]         _OutlineStencilZFail        ("ZFail", Float) = 0
                                                        _OutlineOffsetFactor        ("sOffsetFactor", Float) = 0
                                                        _OutlineOffsetUnits         ("sOffsetUnits", Float) = 0
        [lilColorMask]                                  _OutlineColorMask           ("sColorMask", Int) = 15
        [lilToggle]                                     _OutlineAlphaToMask         ("sAlphaToMask", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Pre
        [lilHDR] [MainColor]                            _PreColor               ("sColor", Color) = (1,1,1,1)
        [lilEnum]                                       _PreOutType             ("sPreOutTypes", Int) = 0
                                                        _PreCutoff              ("Pre Cutoff", Range(-0.001,1.001)) = 0.5
        [lilEnum]                                       _PreCull                ("Cull Mode|Off|Front|Back", Int) = 2
        [Enum(UnityEngine.Rendering.BlendMode)]         _PreSrcBlend            ("sSrcBlendRGB", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _PreDstBlend            ("sDstBlendRGB", Int) = 10
        [Enum(UnityEngine.Rendering.BlendMode)]         _PreSrcBlendAlpha       ("sSrcBlendAlpha", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _PreDstBlendAlpha       ("sDstBlendAlpha", Int) = 10
        [Enum(UnityEngine.Rendering.BlendOp)]           _PreBlendOp             ("sBlendOpRGB", Int) = 0
        [Enum(UnityEngine.Rendering.BlendOp)]           _PreBlendOpAlpha        ("sBlendOpAlpha", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]         _PreSrcBlendFA          ("sSrcBlendRGB", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _PreDstBlendFA          ("sDstBlendRGB", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _PreSrcBlendAlphaFA     ("sSrcBlendAlpha", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]         _PreDstBlendAlphaFA     ("sDstBlendAlpha", Int) = 1
        [Enum(UnityEngine.Rendering.BlendOp)]           _PreBlendOpFA           ("sBlendOpRGB", Int) = 4
        [Enum(UnityEngine.Rendering.BlendOp)]           _PreBlendOpAlphaFA      ("sBlendOpAlpha", Int) = 4
        [lilToggle]                                     _PreZClip               ("sZClip", Int) = 1
        [lilToggle]                                     _PreZWrite              ("sZWrite", Int) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)]   _PreZTest               ("sZTest", Int) = 4
        [IntRange]                                      _PreStencilRef          ("Ref", Range(0, 255)) = 0
        [IntRange]                                      _PreStencilReadMask     ("ReadMask", Range(0, 255)) = 255
        [IntRange]                                      _PreStencilWriteMask    ("WriteMask", Range(0, 255)) = 255
        [Enum(UnityEngine.Rendering.CompareFunction)]   _PreStencilComp         ("Comp", Float) = 8
        [Enum(UnityEngine.Rendering.StencilOp)]         _PreStencilPass         ("Pass", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]         _PreStencilFail         ("Fail", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]         _PreStencilZFail        ("ZFail", Float) = 0
                                                        _PreOffsetFactor        ("sOffsetFactor", Float) = 0
                                                        _PreOffsetUnits         ("sOffsetUnits", Float) = 0
        [lilColorMask]                                  _PreColorMask           ("sColorMask", Int) = 15
        [lilToggle]                                     _PreAlphaToMask         ("sAlphaToMask", Int) = 0
    }

    SubShader
    {
        Tags {"RenderType" = "TransparentCutout" "Queue" = "AlphaTest+10"}
        UsePass "Hidden/ltspass_lite_transparent/FORWARD_BACK"
        UsePass "Hidden/ltspass_lite_transparent/FORWARD"
        UsePass "Hidden/ltspass_lite_transparent/FORWARD_OUTLINE"
        UsePass "Hidden/ltspass_lite_transparent/FORWARD_ADD"
        UsePass "Hidden/ltspass_lite_transparent/FORWARD_ADD_OUTLINE"
        UsePass "Hidden/ltspass_lite_transparent/SHADOW_CASTER_OUTLINE"
        UsePass "Hidden/ltspass_lite_transparent/META"
        Pass
        {
            Tags { "LightMode" = "Never" }

            HLSLPROGRAM
            // Unity strips unused UV channels from meshes; unfortunately, in 2022.3.13f1, Unity fails to detect that UV channels
            // are used when they are referenced from a pass included via `UsePass`. This fake pass is #included directly into
            // each shader to work around this; because this has an invalid lightmode set, it will never actually be executed.
            //
            // Unity bug report ID: IN-60271
            #pragma vertex vert
            #pragma fragment frag

            // For some reason, using struct appdata from lil_common_appdata doesn't work as a workaround...
            //#include "Includes/lil_pipeline_brp.hlsl"
            //#include "Includes/lil_common.hlsl"
            //#include "Includes/lil_common_appdata.hlsl"


            struct appdata
            {
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD3;

                float2 uv4 : TEXCOORD4;
                float2 uv5 : TEXCOORD5;
                float2 uv6 : TEXCOORD6;
                float2 uv7 : TEXCOORD7;

                float4 color        : COLOR;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                #if !defined(SHADER_API_MOBILE) && !defined(SHADER_API_GLES)
                uint vertexID       : SV_VertexID;
                #endif

                float4 pos : POSITION;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float4 col : TEXCOORD0;
            };

            struct v2f vert(struct appdata input)
            {
                struct v2f output;
                // Don't actually render to the screen, but pass UV-derived data all the way down to the fragment
                // shader so it shows up as an input in the compiled shader program.
                output.pos = float4(0,0,0,1);
                output.col = float4(input.uv, input.uv1) + float4(input.uv2, input.uv3)
                  + float4(input.uv4, input.uv5) + float4(input.uv6, input.uv7)
                  + input.color + float4(input.normalOS, 1) + input.tangentOS;

                #if !defined(SHADER_API_MOBILE) && !defined(SHADER_API_GLES)
                output.col.a += input.vertexID;
                #endif

                return output;
            }

            float4 frag(v2f i) : SV_Target
            {
                return i.col;
            }
            ENDHLSL
        }
    }
    Fallback "Unlit/Texture"

    CustomEditor "lilToon.lilToonInspector"
}

