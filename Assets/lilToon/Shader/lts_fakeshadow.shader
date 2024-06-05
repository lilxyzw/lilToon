Shader "_lil/[Optional] lilToonFakeShadow"
{
    Properties
    {
        //----------------------------------------------------------------------------------------------------------------------
        // Base
        [lilToggle]     _Invisible                  ("sInvisible", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Main
        [lilHDR] [MainColor] _Color                 ("sColor", Color) = (0.925,0.7,0.74,1)
        [MainTexture]   _MainTex                    ("Texture", 2D) = "white" {}
        [lilVec3Float]  _FakeShadowVector           ("sFakeShadowVectors", Vector) = (0,0,0,0.05)

        //----------------------------------------------------------------------------------------------------------------------
        // Encryption
        [lilToggle]     _IgnoreEncryption           ("sIgnoreEncryption", Int) = 0
                        _Keys                       ("sKeys", Vector) = (0,0,0,0)
                        _BitKey0                    ("_BitKey0", Float) = 0
                        _BitKey1                    ("_BitKey1", Float) = 0
                        _BitKey2                    ("_BitKey2", Float) = 0
                        _BitKey3                    ("_BitKey3", Float) = 0
                        _BitKey4                    ("_BitKey4", Float) = 0
                        _BitKey5                    ("_BitKey5", Float) = 0
                        _BitKey6                    ("_BitKey6", Float) = 0
                        _BitKey7                    ("_BitKey7", Float) = 0
                        _BitKey8                    ("_BitKey8", Float) = 0
                        _BitKey9                    ("_BitKey9", Float) = 0
                        _BitKey10                   ("_BitKey10", Float) = 0
                        _BitKey11                   ("_BitKey11", Float) = 0
                        _BitKey12                   ("_BitKey12", Float) = 0
                        _BitKey13                   ("_BitKey13", Float) = 0
                        _BitKey14                   ("_BitKey14", Float) = 0
                        _BitKey15                   ("_BitKey15", Float) = 0
                        _BitKey16                   ("_BitKey16", Float) = 0
                        _BitKey17                   ("_BitKey17", Float) = 0
                        _BitKey18                   ("_BitKey18", Float) = 0
                        _BitKey19                   ("_BitKey19", Float) = 0
                        _BitKey20                   ("_BitKey20", Float) = 0
                        _BitKey21                   ("_BitKey21", Float) = 0
                        _BitKey22                   ("_BitKey22", Float) = 0
                        _BitKey23                   ("_BitKey23", Float) = 0
                        _BitKey24                   ("_BitKey24", Float) = 0
                        _BitKey25                   ("_BitKey25", Float) = 0
                        _BitKey26                   ("_BitKey26", Float) = 0
                        _BitKey27                   ("_BitKey27", Float) = 0
                        _BitKey28                   ("_BitKey28", Float) = 0
                        _BitKey29                   ("_BitKey29", Float) = 0
                        _BitKey30                   ("_BitKey30", Float) = 0
                        _BitKey31                   ("_BitKey31", Float) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Advanced
        [lilEnum]                                       _Cull               ("sCullModes", Int) = 2
        [Enum(UnityEngine.Rendering.BlendMode)]         _SrcBlend           ("sSrcBlendRGB", Int) = 2
        [Enum(UnityEngine.Rendering.BlendMode)]         _DstBlend           ("sDstBlendRGB", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]         _SrcBlendAlpha      ("sSrcBlendAlpha", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]         _DstBlendAlpha      ("sDstBlendAlpha", Int) = 1
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
        [IntRange]                                      _StencilRef         ("Ref", Range(0, 255)) = 51
        [IntRange]                                      _StencilReadMask    ("ReadMask", Range(0, 255)) = 255
        [IntRange]                                      _StencilWriteMask   ("WriteMask", Range(0, 255)) = 255
        [Enum(UnityEngine.Rendering.CompareFunction)]   _StencilComp        ("Comp", Float) = 3
        [Enum(UnityEngine.Rendering.StencilOp)]         _StencilPass        ("Pass", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]         _StencilFail        ("Fail", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]         _StencilZFail       ("ZFail", Float) = 0
                                                        _OffsetFactor       ("sOffsetFactor", Float) = 0
                                                        _OffsetUnits        ("sOffsetUnits", Float) = 0
        [lilColorMask]                                  _ColorMask          ("sColorMask", Int) = 15
        [lilToggle]                                     _AlphaToMask        ("sAlphaToMask", Int) = 0
                                                        _lilShadowCasterBias ("Shadow Caster Bias", Float) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Save (Unused)
        [HideInInspector]                               _BaseColor          ("sColor", Color) = (1,1,1,1)
        [HideInInspector]                               _BaseMap            ("Texture", 2D) = "white" {}
        [HideInInspector]                               _BaseColorMap       ("Texture", 2D) = "white" {}
        [HideInInspector]                               _lilToonVersion     ("Version", Int) = 44
    }

    SubShader
    {
        Tags {"RenderType" = "Transparent" "Queue" = "AlphaTest+55"}
        HLSLINCLUDE
            #define LIL_FEATURE_ANIMATE_MAIN_UV
            #define LIL_FEATURE_MAIN_TONE_CORRECTION
            #define LIL_FEATURE_MAIN_GRADATION_MAP
            #define LIL_FEATURE_MAIN2ND
            #define LIL_FEATURE_MAIN3RD
            #define LIL_FEATURE_DECAL
            #define LIL_FEATURE_ANIMATE_DECAL
            #define LIL_FEATURE_LAYER_DISSOLVE
            #define LIL_FEATURE_ALPHAMASK
            #define LIL_FEATURE_SHADOW
            #define LIL_FEATURE_RECEIVE_SHADOW
            #define LIL_FEATURE_SHADOW_3RD
            #define LIL_FEATURE_SHADOW_LUT
            #define LIL_FEATURE_RIMSHADE
            #define LIL_FEATURE_EMISSION_1ST
            #define LIL_FEATURE_EMISSION_2ND
            #define LIL_FEATURE_ANIMATE_EMISSION_UV
            #define LIL_FEATURE_ANIMATE_EMISSION_MASK_UV
            #define LIL_FEATURE_EMISSION_GRADATION
            #define LIL_FEATURE_NORMAL_1ST
            #define LIL_FEATURE_NORMAL_2ND
            #define LIL_FEATURE_ANISOTROPY
            #define LIL_FEATURE_REFLECTION
            #define LIL_FEATURE_MATCAP
            #define LIL_FEATURE_MATCAP_2ND
            #define LIL_FEATURE_RIMLIGHT
            #define LIL_FEATURE_RIMLIGHT_DIRECTION
            #define LIL_FEATURE_GLITTER
            #define LIL_FEATURE_BACKLIGHT
            #define LIL_FEATURE_PARALLAX
            #define LIL_FEATURE_POM
            #define LIL_FEATURE_DISTANCE_FADE
            #define LIL_FEATURE_AUDIOLINK
            #define LIL_FEATURE_AUDIOLINK_VERTEX
            #define LIL_FEATURE_AUDIOLINK_LOCAL
            #define LIL_FEATURE_DISSOLVE
            #define LIL_FEATURE_DITHER
            #define LIL_FEATURE_IDMASK
            #define LIL_FEATURE_UDIMDISCARD
            #define LIL_FEATURE_OUTLINE_TONE_CORRECTION
            #define LIL_FEATURE_OUTLINE_RECEIVE_SHADOW
            #define LIL_FEATURE_ANIMATE_OUTLINE_UV
            #define LIL_FEATURE_FUR_COLLISION
            #define LIL_FEATURE_MainGradationTex
            #define LIL_FEATURE_MainColorAdjustMask
            #define LIL_FEATURE_Main2ndTex
            #define LIL_FEATURE_Main2ndBlendMask
            #define LIL_FEATURE_Main2ndDissolveMask
            #define LIL_FEATURE_Main2ndDissolveNoiseMask
            #define LIL_FEATURE_Main3rdTex
            #define LIL_FEATURE_Main3rdBlendMask
            #define LIL_FEATURE_Main3rdDissolveMask
            #define LIL_FEATURE_Main3rdDissolveNoiseMask
            #define LIL_FEATURE_AlphaMask
            #define LIL_FEATURE_BumpMap
            #define LIL_FEATURE_Bump2ndMap
            #define LIL_FEATURE_Bump2ndScaleMask
            #define LIL_FEATURE_AnisotropyTangentMap
            #define LIL_FEATURE_AnisotropyScaleMask
            #define LIL_FEATURE_AnisotropyShiftNoiseMask
            #define LIL_FEATURE_ShadowBorderMask
            #define LIL_FEATURE_ShadowBlurMask
            #define LIL_FEATURE_ShadowStrengthMask
            #define LIL_FEATURE_ShadowColorTex
            #define LIL_FEATURE_Shadow2ndColorTex
            #define LIL_FEATURE_Shadow3rdColorTex
            #define LIL_FEATURE_RimShadeMask
            #define LIL_FEATURE_BacklightColorTex
            #define LIL_FEATURE_SmoothnessTex
            #define LIL_FEATURE_MetallicGlossMap
            #define LIL_FEATURE_ReflectionColorTex
            #define LIL_FEATURE_ReflectionCubeTex
            #define LIL_FEATURE_MatCapTex
            #define LIL_FEATURE_MatCapBlendMask
            #define LIL_FEATURE_MatCapBumpMap
            #define LIL_FEATURE_MatCap2ndTex
            #define LIL_FEATURE_MatCap2ndBlendMask
            #define LIL_FEATURE_MatCap2ndBumpMap
            #define LIL_FEATURE_RimColorTex
            #define LIL_FEATURE_GlitterColorTex
            #define LIL_FEATURE_GlitterShapeTex
            #define LIL_FEATURE_EmissionMap
            #define LIL_FEATURE_EmissionBlendMask
            #define LIL_FEATURE_EmissionGradTex
            #define LIL_FEATURE_Emission2ndMap
            #define LIL_FEATURE_Emission2ndBlendMask
            #define LIL_FEATURE_Emission2ndGradTex
            #define LIL_FEATURE_ParallaxMap
            #define LIL_FEATURE_AudioLinkMask
            #define LIL_FEATURE_AudioLinkLocalMap
            #define LIL_FEATURE_DissolveMask
            #define LIL_FEATURE_DissolveNoiseMask
            #define LIL_FEATURE_OutlineTex
            #define LIL_FEATURE_OutlineWidthMask
            #define LIL_FEATURE_OutlineVectorTex
            #define LIL_FEATURE_FurNoiseMask
            #define LIL_FEATURE_FurMask
            #define LIL_FEATURE_FurLengthMask
            #define LIL_FEATURE_FurVectorTex
            #define LIL_OPTIMIZE_APPLY_SHADOW_FA
            #define LIL_OPTIMIZE_USE_FORWARDADD
            #define LIL_OPTIMIZE_USE_VERTEXLIGHT
            #pragma skip_variants LIGHTMAP_ON DYNAMICLIGHTMAP_ON LIGHTMAP_SHADOW_MIXING SHADOWS_SHADOWMASK DIRLIGHTMAP_COMBINED _MIXED_LIGHTING_SUBTRACTIVE
            #pragma target 3.5
            #pragma fragmentoption ARB_precision_hint_fastest
            #define LIL_FAKESHADOW

            #pragma skip_variants SHADOWS_SCREEN _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN _ADDITIONAL_LIGHT_SHADOWS SCREEN_SPACE_SHADOWS_ON SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH SHADOW_VERY_HIGH
            #pragma skip_variants LIGHTMAP_ON DYNAMICLIGHTMAP_ON LIGHTMAP_SHADOW_MIXING SHADOWS_SHADOWMASK DIRLIGHTMAP_COMBINED _MIXED_LIGHTING_SUBTRACTIVE
            #pragma skip_variants DECALS_OFF DECALS_3RT DECALS_4RT DECAL_SURFACE_GRADIENT _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma skip_variants VERTEXLIGHT_ON LIGHTPROBE_SH
            #pragma skip_variants _ADDITIONAL_LIGHT_SHADOWS
            #pragma skip_variants PROBE_VOLUMES_OFF PROBE_VOLUMES_L1 PROBE_VOLUMES_L2
            #pragma skip_variants _SCREEN_SPACE_OCCLUSION
            #pragma skip_variants _REFLECTION_PROBE_BLENDING _REFLECTION_PROBE_BOX_PROJECTION
        ENDHLSL


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
            #define LIL_PASS_FORWARD

            //----------------------------------------------------------------------------------------------------------------------
            // Pass
            #include "Includes/lil_pipeline_brp.hlsl"
            #include "Includes/lil_common.hlsl"
            // Insert functions and includes that depend on Unity here

            #include "Includes/lil_pass_forward_fakeshadow.hlsl"

            ENDHLSL
        }

    }
    Fallback "Unlit/Texture"

    CustomEditor "lilToon.lilToonInspector"
}

