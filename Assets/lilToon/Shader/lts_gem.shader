Shader "Hidden/lilToonGem"
{
    Properties
    {
        //----------------------------------------------------------------------------------------------------------------------
        // Base
        [lilToggle]     _Invisible                  ("Invisible", Int) = 0
                        _AsUnlit                    ("As Unlit", Range(0, 1)) = 0
                        _VertexLightStrength        ("Vertex Light Strength", Range(0,1)) = 1
                        _LightMinLimit              ("Light Min Limit", Range(0,1)) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Main
        [lilHDR]        _Color                      ("Color", Color) = (1,1,1,1)
                        _MainTex                    ("Texture", 2D) = "white" {}
        [lilUVAnim]     _MainTex_ScrollRotate       ("Angle|UV Animation|Scroll|Rotate", Vector) = (0,0,0,0)

        //----------------------------------------------------------------------------------------------------------------------
        // NormalMap
        [lilToggleLeft] _UseBumpMap                 ("Use Normal Map", Int) = 0
        [Normal]        _BumpMap                    ("Normal Map", 2D) = "bump" {}
                        _BumpScale                  ("Scale", Range(-10,10)) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // NormalMap 2nd
        [lilToggleLeft] _UseBump2ndMap              ("Use Normal Map 2nd", Int) = 0
        [Normal]        _Bump2ndMap                 ("Normal Map", 2D) = "bump" {}
                        _Bump2ndScale               ("Scale", Range(-10,10)) = 1
        [NoScaleOffset] _Bump2ndScaleMask           ("Mask", 2D) = "white" {}

        //----------------------------------------------------------------------------------------------------------------------
        // MatCap
        [lilToggleLeft] _UseMatCap                  ("Use MatCap", Int) = 0
        [lilHDR]        _MatCapColor                ("Color", Color) = (1,1,1,1)
        [NoScaleOffset] _MatCapTex                  ("Texture", 2D) = "white" {}
                        _MatCapBlend                ("Blend", Range(0, 1)) = 1
        [NoScaleOffset] _MatCapBlendMask            ("Mask", 2D) = "white" {}
                        _MatCapEnableLighting       ("Enable Lighting", Range(0, 1)) = 1
        [lilEnum]       _MatCapBlendMode            ("Blend Mode|Normal|Add|Screen|Multiply", Int) = 1
        [lilToggle]     _MatCapApplyTransparency    ("Apply Transparency", Int) = 1
        [lilToggle]     _MatCapZRotCancel           ("Z-axis rotation cancellation", Int) = 1
        [lilToggle]     _MatCapCustomNormal         ("MatCap Custom Normal Map", Int) = 0
        [Normal]        _MatCapBumpMap              ("Normal Map", 2D) = "bump" {}
                        _MatCapBumpScale            ("Scale", Range(-10,10)) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // MatCap 2nd
        [lilToggleLeft] _UseMatCap2nd               ("Use MatCap 2nd", Int) = 0
        [lilHDR]        _MatCap2ndColor             ("Color", Color) = (1,1,1,1)
        [NoScaleOffset] _MatCap2ndTex               ("Texture", 2D) = "white" {}
                        _MatCap2ndBlend             ("Blend", Range(0, 1)) = 1
        [NoScaleOffset] _MatCap2ndBlendMask         ("Mask", 2D) = "white" {}
                        _MatCap2ndEnableLighting    ("Enable Lighting", Range(0, 1)) = 1
        [lilEnum]       _MatCap2ndBlendMode         ("Blend Mode|Normal|Add|Screen|Multiply", Int) = 1
        [lilToggle]     _MatCap2ndApplyTransparency ("Apply Transparency", Int) = 1
        [lilToggle]     _MatCap2ndZRotCancel        ("Z-axis rotation cancellation", Int) = 1
        [lilToggle]     _MatCap2ndCustomNormal      ("MatCap Custom Normal Map", Int) = 0
        [Normal]        _MatCap2ndBumpMap           ("Normal Map", 2D) = "bump" {}
                        _MatCap2ndBumpScale         ("Scale", Range(-10,10)) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // Rim
        [lilToggleLeft] _UseRim                     ("Use Rim", Int) = 0
        [lilHDR]        _RimColor                   ("Color", Color) = (1,1,1,1)
        [NoScaleOffset] _RimColorTex                ("Texture", 2D) = "white" {}
                        _RimBorder                  ("Border", Range(0, 1)) = 0.5
                        _RimBlur                    ("Blur", Range(0, 1)) = 0.1
        [PowerSlider(3.0)]_RimFresnelPower          ("Fresnel Power", Range(0.01, 50)) = 3.0
                        _RimEnableLighting          ("Enable Lighting", Range(0, 1)) = 1
        [lilToggle]     _RimShadowMask              ("Shadow Mask", Int) = 0
        [lilToggle]     _RimApplyTransparency       ("Apply Transparency", Int) = 1
                        _RimDirStrength             ("Light direction strength", Range(0, 1)) = 0
                        _RimDirRange                ("Direction range", Range(-1, 1)) = 0
                        _RimIndirRange              ("Indirection range", Range(-1, 1)) = 0
        [lilHDR]        _RimIndirColor              ("Indirection Color", Color) = (1,1,1,1)
                        _RimIndirBorder             ("Indirection Border", Range(0, 1)) = 0.5
                        _RimIndirBlur               ("Indirection Blur", Range(0, 1)) = 0.1

        //----------------------------------------------------------------------------------------------------------------------
        // Glitter
        [lilToggleLeft] _UseGlitter                 ("Use Glitter", Int) = 0
        [lilEnum]       _GlitterUVMode              ("UV Mode|UV0|UV1", Int) = 0
        [lilHDR]        _GlitterColor               ("Color", Color) = (1,1,1,1)
                        _GlitterColorTex            ("Texture", 2D) = "white" {}
                        _GlitterMainStrength        ("Main Color Strength", Range(0, 1)) = 0
        [lilGlitParam1] _GlitterParams1             ("Tiling|Particle Size|Contrast", Vector) = (256,256,0.16,50)
        [lilGlitParam2] _GlitterParams2             ("Blink Speed|Angle|Blend Light Direction|Color Randomness", Vector) = (0.25,0,0,0)
                        _GlitterEnableLighting      ("Enable Lighting", Range(0, 1)) = 1
        [lilToggle]     _GlitterShadowMask          ("Shadow Mask", Int) = 0
        [lilToggle]     _GlitterApplyTransparency   ("Apply Transparency", Int) = 1
                        _GlitterVRParallaxStrength  ("VR Parallax Strength", Range(0, 1)) = 1

        //----------------------------------------------------------------------------------------------------------------------
        // Emmision
        [lilToggleLeft] _UseEmission                ("Use Emission", Int) = 0
        [HDR][lilHDR]   _EmissionColor              ("Color", Color) = (1,1,1)
                        _EmissionMap                ("Texture", 2D) = "white" {}
        [lilUVAnim]     _EmissionMap_ScrollRotate   ("Angle|UV Animation|Scroll|Rotate", Vector) = (0,0,0,0)
                        _EmissionBlend              ("Blend", Range(0,1)) = 1
                        _EmissionBlendMask          ("Mask", 2D) = "white" {}
        [lilUVAnim]     _EmissionBlendMask_ScrollRotate ("Angle|UV Animation|Scroll|Rotate", Vector) = (0,0,0,0)
        [lilBlink]      _EmissionBlink              ("Blink Strength|Blink Type|Blink Speed|Blink Offset", Vector) = (0,0,3.141593,0)
        [lilToggle]     _EmissionUseGrad            ("Use Gradation", Int) = 0
        [NoScaleOffset] _EmissionGradTex            ("Gradation Texture", 2D) = "white" {}
                        _EmissionGradSpeed          ("Gradation Speed", Float) = 1
                        _EmissionParallaxDepth      ("Parallax Depth", float) = 0
                        _EmissionFluorescence       ("Fluorescence", Range(0,1)) = 0
        // Gradation
        [HideInInspector] _egci ("", Int) = 2
        [HideInInspector] _egai ("", Int) = 2
        [HideInInspector] _egc0 ("", Color) = (1,1,1,0)
        [HideInInspector] _egc1 ("", Color) = (1,1,1,1)
        [HideInInspector] _egc2 ("", Color) = (1,1,1,0)
        [HideInInspector] _egc3 ("", Color) = (1,1,1,0)
        [HideInInspector] _egc4 ("", Color) = (1,1,1,0)
        [HideInInspector] _egc5 ("", Color) = (1,1,1,0)
        [HideInInspector] _egc6 ("", Color) = (1,1,1,0)
        [HideInInspector] _egc7 ("", Color) = (1,1,1,0)
        [HideInInspector] _ega0 ("", Color) = (1,0,0,0)
        [HideInInspector] _ega1 ("", Color) = (1,0,0,1)
        [HideInInspector] _ega2 ("", Color) = (1,0,0,0)
        [HideInInspector] _ega3 ("", Color) = (1,0,0,0)
        [HideInInspector] _ega4 ("", Color) = (1,0,0,0)
        [HideInInspector] _ega5 ("", Color) = (1,0,0,0)
        [HideInInspector] _ega6 ("", Color) = (1,0,0,0)
        [HideInInspector] _ega7 ("", Color) = (1,0,0,0)

        //----------------------------------------------------------------------------------------------------------------------
        // Emmision2nd
        [lilToggleLeft] _UseEmission2nd             ("Use Emission 2nd", Int) = 0
        [HDR][lilHDR]   _Emission2ndColor           ("Color", Color) = (1,1,1)
                        _Emission2ndMap             ("Texture", 2D) = "white" {}
        [lilUVAnim]     _Emission2ndMap_ScrollRotate ("Angle|UV Animation|Scroll|Rotate", Vector) = (0,0,0,0)
                        _Emission2ndBlend           ("Blend", Range(0,1)) = 1
                        _Emission2ndBlendMask       ("Mask", 2D) = "white" {}
        [lilUVAnim]     _Emission2ndBlendMask_ScrollRotate ("Angle|UV Animation|Scroll|Rotate", Vector) = (0,0,0,0)
        [lilBlink]      _Emission2ndBlink           ("Blink Strength|Blink Type|Blink Speed|Blink Offset", Vector) = (0,0,3.141593,0)
        [lilToggle]     _Emission2ndUseGrad         ("Use Gradation", Int) = 0
        [NoScaleOffset] _Emission2ndGradTex         ("Gradation Texture", 2D) = "white" {}
                        _Emission2ndGradSpeed       ("Gradation Speed", Float) = 1
                        _Emission2ndParallaxDepth   ("Parallax Depth", float) = 0
                        _Emission2ndFluorescence    ("Fluorescence", Range(0,1)) = 0
        // Gradation
        [HideInInspector] _e2gci ("", Int) = 2
        [HideInInspector] _e2gai ("", Int) = 2
        [HideInInspector] _e2gc0 ("", Color) = (1,1,1,0)
        [HideInInspector] _e2gc1 ("", Color) = (1,1,1,1)
        [HideInInspector] _e2gc2 ("", Color) = (1,1,1,0)
        [HideInInspector] _e2gc3 ("", Color) = (1,1,1,0)
        [HideInInspector] _e2gc4 ("", Color) = (1,1,1,0)
        [HideInInspector] _e2gc5 ("", Color) = (1,1,1,0)
        [HideInInspector] _e2gc6 ("", Color) = (1,1,1,0)
        [HideInInspector] _e2gc7 ("", Color) = (1,1,1,0)
        [HideInInspector] _e2ga0 ("", Color) = (1,0,0,0)
        [HideInInspector] _e2ga1 ("", Color) = (1,0,0,1)
        [HideInInspector] _e2ga2 ("", Color) = (1,0,0,0)
        [HideInInspector] _e2ga3 ("", Color) = (1,0,0,0)
        [HideInInspector] _e2ga4 ("", Color) = (1,0,0,0)
        [HideInInspector] _e2ga5 ("", Color) = (1,0,0,0)
        [HideInInspector] _e2ga6 ("", Color) = (1,0,0,0)
        [HideInInspector] _e2ga7 ("", Color) = (1,0,0,0)

        //----------------------------------------------------------------------------------------------------------------------
        // AudioLink
        [lilToggleLeft] _UseAudioLink               ("Use AudioLink", Int) = 0
        [lilALUVMode]   _AudioLinkUVMode            ("UV Mode|None|Rim|UV|Mask", Int) = 1
        [lilALUVParams] _AudioLinkUVParams          ("Scale|Offset|Angle|Band|Bass|Low Mid|High Mid|Treble", Vector) = (0.25,0,0,0.125)
        [NoScaleOffset] _AudioLinkMask              ("Mask", 2D) = "blue" {}
        [lilToggle]     _AudioLink2Main2nd          ("Main 2nd", Int) = 0
        [lilToggle]     _AudioLink2Main3rd          ("Main 3rd", Int) = 0
        [lilToggle]     _AudioLink2Emission         ("Emission", Int) = 0
        [lilToggle]     _AudioLink2Emission2nd      ("Emission 2nd", Int) = 0
        [lilToggle]     _AudioLink2Vertex           ("Vertex", Int) = 0
        [lilALUVMode]   _AudioLinkVertexUVMode      ("UV Mode|None|Position|UV|Mask", Int) = 1
        [lilALUVParams] _AudioLinkVertexUVParams    ("Scale|Offset|Angle|Band|Bass|Low Mid|High Mid|Treble", Vector) = (0.25,0,0,0.125)
        [lilVec3]       _AudioLinkVertexStart       ("Start Position", Vector) = (0.0,0.0,0.0,0.0)
        [lilVec3Float]  _AudioLinkVertexStrength    ("Moving Vector|Normal Strength", Vector) = (0.0,0.0,0.0,1.0)
        [lilToggle]     _AudioLinkAsLocal           ("As Local", Int) = 0
        [NoScaleOffset] _AudioLinkLocalMap          ("Local Map", 2D) = "black" {}
        [lilALLocal]    _AudioLinkLocalMapParams    ("BPM|Notes|Offset", Vector) = (120,1,0,0)

        //----------------------------------------------------------------------------------------------------------------------
        // Encryption
        [lilToggle]     _IgnoreEncryption           ("Ignore Encryption", Int) = 0
                        _Keys                       ("Keys", Vector) = (0,0,0,0)

        //----------------------------------------------------------------------------------------------------------------------
        // Advanced
        [lilEnum]                                       _Cull               ("Cull Mode|Off|Front|Back", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)]         _SrcBlend           ("SrcBlend", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _DstBlend           ("DstBlend", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _SrcBlendAlpha      ("SrcBlendAlpha", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]         _DstBlendAlpha      ("DstBlendAlpha", Int) = 10
        [Enum(UnityEngine.Rendering.BlendOp)]           _BlendOp            ("BlendOp", Int) = 0
        [Enum(UnityEngine.Rendering.BlendOp)]           _BlendOpAlpha       ("BlendOpAlpha", Int) = 0
        [lilToggle]                                     _ZWrite             ("ZWrite", Int) = 0
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

        //----------------------------------------------------------------------------------------------------------------------
        // Refraction
                        _RefractionStrength         ("Refraction Strength", Range(0,1)) = 0.5
        [PowerSlider(3.0)]_RefractionFresnelPower   ("Refraction Fresnel Power", Range(0.01, 10)) = 1.0

        //----------------------------------------------------------------------------------------------------------------------
        //Gem
                        _GemChromaticAberration     ("Chromatic Aberration", Range(0, 1)) = 0.02
                        _GemEnvContrast             ("Environment Contrast", Float) = 2.0
        [lilHDR]        _GemEnvColor                ("Environment Color", Color) = (1,1,1,1)
                        _GemParticleLoop            ("Particle Loop", Float) = 8
        [lilHDR]        _GemParticleColor           ("Particle Color", Color) = (4,4,4,1)
                        _GemVRParallaxStrength      ("VR Parallax Strength", Range(0, 1)) = 1
                        _Smoothness                 ("Smoothness", Range(0, 1)) = 1
        [NoScaleOffset] _SmoothnessTex              ("Smoothness", 2D) = "white" {}
        [Gamma]         _Reflectance                ("Reflectance", Range(0, 1)) = 0.04
    }
    HLSLINCLUDE
        #define LIL_RENDER 2
        #define LIL_GEM
    ENDHLSL
    SubShader
    {
        Tags {"RenderType" = "Opaque" "Queue" = "Transparent"}
        GrabPass {"_BackgroundTexture"}
        Pass
        {
            Name "FORWARD_PRE"
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
            Blend One Zero, Zero One
            HLSLPROGRAM

            //------------------------------------------------------------------------------------------------------------------
            // Build Option
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma fragmentoption ARB_precision_hint_fastest

            #include "Includes/lil_pipeline.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                #if defined(LIL_FEATURE_ENCRYPTION)
                    float2 uv6          : TEXCOORD6;
                    float2 uv7          : TEXCOORD7;
                #endif
                LIL_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                LIL_FOG_COORDS(0)
                LIL_VERTEX_INPUT_INSTANCE_ID
                LIL_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata input)
            {
                v2f output;
                LIL_INITIALIZE_STRUCT(v2f, output);

                LIL_BRANCH
                if(_Invisible) return output;

                LIL_SETUP_INSTANCE_ID(input);
                LIL_TRANSFER_INSTANCE_ID(input, output);
                LIL_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                //----------------------------------------------------------------------------------------------------------------------
                // Encryption
                #if defined(LIL_FEATURE_ENCRYPTION)
                    input.positionOS = vertexDecode(input.positionOS, input.normalOS, input.uv6, input.uv7);
                #endif

                LIL_VERTEX_POSITION_INPUTS(input.positionOS, vertexInput);
                output.positionCS = vertexInput.positionCS;
                LIL_TRANSFER_FOG(vertexInput, output);
                return output;
            }

            float4 frag(v2f input) : SV_Target
            {
                float4 col = 0;
                LIL_APPLY_FOG(col, input.fogCoord);
                return col;
            }
            ENDHLSL
        }

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
            #pragma skip_variants SHADOWS_SCREEN

            #include "Includes/lil_pipeline.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                #if defined(LIL_SHOULD_TANGENT)
                    float4 tangentOS    : TANGENT;
                #endif
                #if defined(LIL_FEATURE_ENCRYPTION)
                    float2 uv6          : TEXCOORD6;
                    float2 uv7          : TEXCOORD7;
                #endif
                float2 uv1          : TEXCOORD1;
                LIL_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                #if defined(LIL_SHOULD_UV1)
                    float2 uv1          : TEXCOORD1;
                #endif
                float3 normalWS : TEXCOORD2;
                float3 positionWS : TEXCOORD3;
                float4 positionSS : TEXCOORD4;
                #if defined(LIL_SHOULD_TBN)
                    float3 tangentWS        : TEXCOORD5;
                    float3 bitangentWS      : TEXCOORD6;
                #endif
                #if defined(LIL_SHOULD_POSITION_OS)
                    float3 positionOS       : TEXCOORD7;
                #endif
                LIL_LIGHTCOLOR_COORDS(8)
                LIL_LIGHTDIRECTION_COORDS(9)
                LIL_INDLIGHTCOLOR_COORDS(10)
                LIL_VERTEXLIGHT_COORDS(11)
                LIL_FOG_COORDS(12)
                LIL_SHADOW_COORDS(13)
                LIL_LIGHTMAP_COORDS(14)
                LIL_VERTEX_INPUT_INSTANCE_ID
                LIL_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata input)
            {
                v2f output;
                LIL_INITIALIZE_STRUCT(v2f, output);

                LIL_BRANCH
                if(_Invisible) return output;

                LIL_SETUP_INSTANCE_ID(input);
                LIL_TRANSFER_INSTANCE_ID(input, output);
                LIL_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float2 uvMain = input.uv * _MainTex_ST.xy + _MainTex_ST.zw;

                //----------------------------------------------------------------------------------------------------------------------
                // Encryption
                #if defined(LIL_FEATURE_ENCRYPTION)
                    input.positionOS = vertexDecode(input.positionOS, input.normalOS, input.uv6, input.uv7);
                #endif

                //----------------------------------------------------------------------------------------------------------------------
                // AudioLink
                #if !defined(LIL_FUR) && defined(LIL_FEATURE_AUDIOLINK) && defined(LIL_FEATURE_AUDIOLINK_VERTEX)
                    if(_UseAudioLink && _AudioLink2Vertex)
                    {
                        float audioLinkValue = 0.0;
                        float4 audioLinkMask = 1.0;
                        float2 audioLinkUV;
                        if(_AudioLinkVertexUVMode == 0) audioLinkUV.x = _AudioLinkVertexUVParams.g;
                        if(_AudioLinkVertexUVMode == 1) audioLinkUV.x = distance(input.positionOS.xyz, _AudioLinkVertexStart.xyz) * _AudioLinkVertexUVParams.r + _AudioLinkVertexUVParams.g;
                        if(_AudioLinkVertexUVMode == 2) audioLinkUV.x = lilRotateUV(input.uv, _AudioLinkVertexUVParams.b).x * _AudioLinkVertexUVParams.r + _AudioLinkVertexUVParams.g;
                        audioLinkUV.y = _AudioLinkVertexUVParams.a;
                        // Mask (R:Delay G:Band B:Strength)
                        if(_AudioLinkVertexUVMode == 3 && Exists_AudioLinkMask)
                        {
                            audioLinkMask = LIL_SAMPLE_2D_LOD(_AudioLinkMask, sampler_linear_repeat, uvMain, 0);
                            audioLinkUV = audioLinkMask.rg;
                        }
                        // Scaling for _AudioTexture (4/64)
                        #if defined(LIL_FEATURE_AUDIOLINK_LOCAL)
                            if(!_AudioLinkAsLocal) audioLinkUV.y *= 0.0625;
                        #else
                            audioLinkUV.y *= 0.0625;
                        #endif
                        // Global
                        if(_UseAudioLink && _AudioTexture_TexelSize.z > 16)
                        {
                            audioLinkValue = LIL_SAMPLE_2D_LOD(_AudioTexture, sampler_linear_clamp, audioLinkUV, 0).r;
                            audioLinkValue = saturate(audioLinkValue);
                        }
                        // Local
                        #if defined(LIL_FEATURE_AUDIOLINK_LOCAL)
                            if(_UseAudioLink && _AudioLinkAsLocal)
                            {
                                audioLinkUV.x += frac(-LIL_TIME * _AudioLinkLocalMapParams.r / 60 * _AudioLinkLocalMapParams.g) + _AudioLinkLocalMapParams.b;
                                audioLinkValue = LIL_SAMPLE_2D_LOD(_AudioLinkLocalMap, sampler_linear_repeat, audioLinkUV, 0).r;
                            }
                        #endif
                        input.positionOS.xyz += (input.normalOS * _AudioLinkVertexStrength.w + _AudioLinkVertexStrength.xyz) * audioLinkValue * audioLinkMask.b;
                    }
                #endif

                LIL_VERTEX_POSITION_INPUTS(input.positionOS, vertexInput);
                LIL_VERTEX_NORMAL_INPUTS(input.normalOS, vertexNormalInput);
                #if defined(LIL_SHOULD_POSITION_OS)
                    output.positionOS   = input.positionOS.xyz;
                #endif
                output.positionWS = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;
                output.positionSS = vertexInput.positionSS;
                output.uv = input.uv;
                output.normalWS = normalize(vertexNormalInput.normalWS);
                LIL_CALC_MAINLIGHT(vertexInput, output);
                LIL_TRANSFER_SHADOW(vertexInput, input.uv1, output);
                LIL_TRANSFER_FOG(vertexInput, output);
                LIL_TRANSFER_LIGHTMAPUV(input.uv1, output);
                LIL_CALC_VERTEXLIGHT(vertexInput, output);
                return output;
            }

            float4 frag(v2f input, float facing : VFACE) : SV_Target
            {
                LIL_SETUP_INSTANCE_ID(input);
                LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                LIL_GET_MAINLIGHT(input, lightColor, lightDirection, attenuation);
                LIL_GET_VERTEXLIGHT(input, vertexLightColor);
                LIL_GET_ADDITIONALLIGHT(input.positionWS, additionalLightColor);
                #if !defined(LIL_PASS_FORWARDADD)
                    #if defined(LIL_USE_LIGHTMAP)
                        lightColor = max(lightColor, _LightMinLimit);
                        lightColor = lerp(lightColor, 1.0, _AsUnlit);
                    #endif
                    #if defined(_ADDITIONAL_LIGHTS)
                        float3 addLightColor = vertexLightColor + lerp(additionalLightColor, 0.0, _AsUnlit);
                    #else
                        float3 addLightColor = vertexLightColor;
                    #endif
                #else
                    lightColor = lerp(lightColor, 0.0, _AsUnlit);
                #endif

                //--------------------------------------------------------------------------------------------------------------------------
                // UV
                #if defined(LIL_FEATURE_ANIMATE_MAIN_UV)
                    float2 uvMain = lilCalcUV(input.uv, _MainTex_ST, _MainTex_ScrollRotate);
                #else
                    float2 uvMain = lilCalcUV(input.uv, _MainTex_ST);
                #endif

                //--------------------------------------------------------------------------------------------------------------------------
                // View Direction
                float3 viewDirection = normalize(LIL_GET_VIEWDIR_WS(input.positionWS.xyz));
                #if defined(USING_STEREO_MATRICES)
                    float3 headDirection = normalize(LIL_GET_HEADDIR_WS(input.positionWS.xyz));
                    float3 gemViewDirection = lerp(headDirection, viewDirection, _GemVRParallaxStrength);
                #else
                    float3 gemViewDirection = viewDirection;
                #endif
                #if defined(LIL_SHOULD_TBN)
                    float3x3 tbnWS = float3x3(input.tangentWS, input.bitangentWS, input.normalWS);
                    float3 parallaxViewDirection = mul(tbnWS, viewDirection);
                    float2 parallaxOffset = (parallaxViewDirection.xy / (parallaxViewDirection.z+0.5));
                #endif

                //----------------------------------------------------------------------------------------------------------------------
                // Normal
                #if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)
                    float3 normalmap = float3(0.0,0.0,1.0);

                    // 1st
                    #if defined(LIL_FEATURE_NORMAL_1ST)
                        LIL_BRANCH
                        if(Exists_BumpMap && _UseBumpMap)
                        {
                            float4 normalTex = LIL_SAMPLE_2D_ST(_BumpMap, sampler_MainTex, uvMain);
                            normalmap = UnpackNormalScale(normalTex, _BumpScale);
                        }
                    #endif

                    // 2nd
                    #if defined(LIL_FEATURE_NORMAL_2ND)
                        LIL_BRANCH
                        if(Exists_Bump2ndMap && _UseBump2ndMap)
                        {
                            float4 normal2ndTex = LIL_SAMPLE_2D_ST(_Bump2ndMap, sampler_MainTex, uvMain);
                            float bump2ndScale = _Bump2ndScale;
                            if(Exists_Bump2ndScaleMask) bump2ndScale *= LIL_SAMPLE_2D(_Bump2ndScaleMask, sampler_MainTex, uvMain).r;
                            normalmap = lilBlendNormal(normalmap, UnpackNormalScale(normal2ndTex, bump2ndScale));
                        }
                    #endif

                    float3 normalDirection = mul(normalmap, tbnWS);
                    normalDirection = facing < 0.0 ? -normalDirection - viewDirection * 0.2 : normalDirection;
                    normalDirection = normalize(normalDirection);
                #else
                    float3 normalDirection = input.normalWS;
                    normalDirection = facing < 0.0 ? -normalDirection - viewDirection * 0.2 : normalDirection;
                    normalDirection = normalize(normalDirection);
                #endif
                float nv = abs(dot(normalDirection, viewDirection));
                float nv1 = abs(dot(normalDirection, gemViewDirection));
                float nv2 = abs(dot(normalDirection, gemViewDirection.yzx));
                float nv3 = abs(dot(normalDirection, gemViewDirection.zxy));
                float invnv = 1-nv1;

                //--------------------------------------------------------------------------------------------------------------------------
                // AudioLink (https://github.com/llealloo/vrc-udon-audio-link)
                #if defined(LIL_FEATURE_AUDIOLINK)
                    float audioLinkValue = 1.0;
                    if(_UseAudioLink)
                    {
                        audioLinkValue = 0.0;
                        float4 audioLinkMask = 1.0;
                        float2 audioLinkUV;
                        if(_AudioLinkUVMode == 0) audioLinkUV.x = _AudioLinkUVParams.g;
                        if(_AudioLinkUVMode == 1) audioLinkUV.x = _AudioLinkUVParams.r - nv * _AudioLinkUVParams.r + _AudioLinkUVParams.g;
                        if(_AudioLinkUVMode == 2) audioLinkUV.x = lilRotateUV(input.uv, _AudioLinkUVParams.b).x * _AudioLinkUVParams.r + _AudioLinkUVParams.g;
                        audioLinkUV.y = _AudioLinkUVParams.a;
                        // Mask (R:Delay G:Band B:Strength)
                        if(_AudioLinkUVMode == 3 && Exists_AudioLinkMask)
                        {
                            audioLinkMask = LIL_SAMPLE_2D(_AudioLinkMask, sampler_MainTex, uvMain);
                            audioLinkUV = audioLinkMask.rg;
                        }
                        // Scaling for _AudioTexture (4/64)
                        #if defined(LIL_FEATURE_AUDIOLINK_LOCAL)
                            if(!_AudioLinkAsLocal) audioLinkUV.y *= 0.0625;
                        #else
                            audioLinkUV.y *= 0.0625;
                        #endif
                        // Global
                        if(_AudioTexture_TexelSize.z > 16)
                        {
                            audioLinkValue = LIL_SAMPLE_2D(_AudioTexture, sampler_linear_clamp, audioLinkUV).r;
                            audioLinkValue = saturate(audioLinkValue);
                        }
                        // Local
                        #if defined(LIL_FEATURE_AUDIOLINK_LOCAL)
                            if(_AudioLinkAsLocal)
                            {
                                audioLinkUV.x += frac(-LIL_TIME * _AudioLinkLocalMapParams.r / 60 * _AudioLinkLocalMapParams.g) + _AudioLinkLocalMapParams.b;
                                audioLinkValue = LIL_SAMPLE_2D(_AudioLinkLocalMap, sampler_linear_repeat, audioLinkUV).r;
                            }
                        #endif
                        audioLinkValue *= audioLinkMask.b;
                    }
                #endif

                //----------------------------------------------------------------------------------------------------------------------
                // Lighting
                #ifndef LIL_PASS_FORWARDADD
                    float shadowmix = saturate(dot(lightDirection, normalDirection));
                    lightColor = saturate(lightColor + addLightColor);
                    shadowmix = saturate(shadowmix + lilLuminance(addLightColor));
                #endif

                float4 baseCol = LIL_SAMPLE_2D(_MainTex, sampler_MainTex, uvMain) * _Color;
                float3 albedo = baseCol.rgb;
                baseCol.rgb *= nv;
                float4 col = baseCol;
                col.rgb *= 0.75;

                //----------------------------------------------------------------------------------------------------------------------
                // Refraction
                float2 scnUV = input.positionSS.xy/input.positionSS.w;
                float2 ref = mul((float3x3)UNITY_MATRIX_V, normalDirection).xy;
                float nvRef = pow(1.0 - nv, _RefractionFresnelPower);
                float3 refractColor;
                refractColor.r = LIL_SAMPLE_2D(_BackgroundTexture, sampler_BackgroundTexture, scnUV + (nvRef * _RefractionStrength) * ref).r;
                refractColor.g = LIL_SAMPLE_2D(_BackgroundTexture, sampler_BackgroundTexture, scnUV + (nvRef * (_RefractionStrength + _GemChromaticAberration)) * ref).g;
                refractColor.b = LIL_SAMPLE_2D(_BackgroundTexture, sampler_BackgroundTexture, scnUV + (nvRef * (_RefractionStrength + _GemChromaticAberration * 2)) * ref).b;
                refractColor = pow(saturate(refractColor), _GemEnvContrast) * _GemEnvContrast;
                refractColor = lerp(dot(refractColor, float3(1.0/3.0,1.0/3.0,1.0/3.0)), refractColor, saturate(1.0/_GemEnvContrast));
                col.rgb *= refractColor;

                //----------------------------------------------------------------------------------------------------------------------
                // Reflection
                float smoothness = _Smoothness;
                if(Exists_SmoothnessTex) smoothness *= LIL_SAMPLE_2D(_SmoothnessTex, sampler_MainTex, uvMain).r;
                float perceptualRoughness = 1.0 - smoothness;
                float roughness = perceptualRoughness * perceptualRoughness;

                UnityGIInput data = (UnityGIInput)0;
                data.worldPos = input.positionWS;
                data.probeHDR[0] = unity_SpecCube0_HDR;
                data.probeHDR[1] = unity_SpecCube1_HDR;
                #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
                    data.boxMin[0] = unity_SpecCube0_BoxMin;
                #endif
                #ifdef UNITY_SPECCUBE_BOX_PROJECTION
                    data.boxMax[0] = unity_SpecCube0_BoxMax;
                    data.probePosition[0] = unity_SpecCube0_ProbePosition;
                    data.boxMax[1] = unity_SpecCube1_BoxMax;
                    data.boxMin[1] = unity_SpecCube1_BoxMin;
                    data.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif

                Unity_GlossyEnvironmentData glossIn;
                glossIn.roughness = perceptualRoughness;
                glossIn.reflUVW   = reflect(-viewDirection,normalDirection);
                float envReflectionColorR = UnityGI_IndirectSpecular(data, 1.0, glossIn).r;

                glossIn.reflUVW   = facing < 0.0 ? reflect(-viewDirection, normalize(normalDirection + viewDirection * invnv * _GemChromaticAberration)) : glossIn.reflUVW;
                float envReflectionColorG = UnityGI_IndirectSpecular(data, 1.0, glossIn).g;

                glossIn.reflUVW   = facing < 0.0 ? reflect(-viewDirection, normalize(normalDirection + viewDirection * invnv * _GemChromaticAberration * 2)) : glossIn.reflUVW;
                float envReflectionColorB = UnityGI_IndirectSpecular(data, 1.0, glossIn).b;

                float3 envReflectionColor = float3(envReflectionColorR, envReflectionColorG, envReflectionColorB);
                envReflectionColor = pow(saturate(envReflectionColor), _GemEnvContrast) * _GemEnvContrast * _GemEnvColor.rgb;
                envReflectionColor = lerp(dot(envReflectionColor, float3(1.0/3.0,1.0/3.0,1.0/3.0)), envReflectionColor, saturate(1.0/_GemEnvContrast));
                envReflectionColor = facing < 0.0 ? envReflectionColor * baseCol.rgb : envReflectionColor;

                float oneMinusReflectivity = LIL_DIELECTRIC_SPECULAR.a;
                float grazingTerm = saturate(smoothness + (1.0-oneMinusReflectivity));
                #ifdef LIL_COLORSPACE_GAMMA
                    float surfaceReduction = 1.0 - 0.28 * roughness * perceptualRoughness;
                #else
                    float surfaceReduction = 1.0 / (roughness * roughness + 1.0);
                #endif

                float particle = step(0.5, frac(nv1 * _GemParticleLoop)) * step(0.5, frac(nv2 * _GemParticleLoop)) * step(0.5, frac(nv3 * _GemParticleLoop));
                float3 particleColor = facing < 0.0 ? 1.0 + particle * _GemParticleColor.rgb : 1.0;

                col.rgb += (surfaceReduction * lilFresnelLerp(_Reflectance, grazingTerm, nv) + 0.5) * 0.5 * particleColor * envReflectionColor;

                //----------------------------------------------------------------------------------------------------------------------
                // MatCap
                #if defined(LIL_FEATURE_MATCAP)
                    LIL_BRANCH
                    if(_UseMatCap)
                    {
                        float2 matUV = float2(0,0);
                        #if defined(LIL_FEATURE_TEX_MATCAP_NORMALMAP)
                            LIL_BRANCH
                            if(_MatCapCustomNormal)
                            {
                                float4 normalTex = LIL_SAMPLE_2D_ST(_MatCapBumpMap, sampler_MainTex, uvMain);
                                float3 normalmap = UnpackNormalScale(normalTex, _MatCapBumpScale);

                                float3 matcapNormalDirection = normalize(mul(normalmap, tbnWS));
                                matcapNormalDirection = facing < (_FlipNormal-1.0) ? -matcapNormalDirection : matcapNormalDirection;
                                matUV = lilCalcMatCapUV(matcapNormalDirection, _MatCapZRotCancel);
                            }
                            else
                        #endif
                        {
                            matUV = lilCalcMatCapUV(normalDirection, _MatCapZRotCancel);
                        }
                        float4 matCapColor = _MatCapColor;
                        if(Exists_MatCapTex) matCapColor *= LIL_SAMPLE_2D(_MatCapTex, sampler_MainTex, matUV);
                        #ifndef LIL_PASS_FORWARDADD
                            matCapColor.rgb = lerp(matCapColor.rgb, matCapColor.rgb * lightColor, _MatCapEnableLighting);
                        #else
                            if(_MatCapBlendMode < 3) matCapColor.rgb *= lightColor * _MatCapEnableLighting;
                        #endif
                        #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                            if(_MatCapApplyTransparency) matCapColor.a *= col.a;
                        #endif
                        if(Exists_MatCapBlendMask) matCapColor.a *= LIL_SAMPLE_2D(_MatCapBlendMask, sampler_MainTex, uvMain).r;
                        col.rgb = lilBlendColor(col.rgb, matCapColor.rgb, _MatCapBlend * matCapColor.a, _MatCapBlendMode);
                    }
                #endif

                #if defined(LIL_FEATURE_MATCAP_2ND)
                    LIL_BRANCH
                    if(_UseMatCap2nd)
                    {
                        float2 mat2ndUV = float2(0,0);
                        #if defined(LIL_FEATURE_TEX_MATCAP_NORMALMAP)
                            LIL_BRANCH
                            if(_MatCap2ndCustomNormal)
                            {
                                float4 normalTex = LIL_SAMPLE_2D_ST(_MatCap2ndBumpMap, sampler_MainTex, uvMain);
                                float3 normalmap = UnpackNormalScale(normalTex, _MatCap2ndBumpScale);
                                float3 matcap2ndNormalDirection = normalize(mul(normalmap, tbnWS));
                                matcap2ndNormalDirection = facing < (_FlipNormal-1.0) ? -matcap2ndNormalDirection : matcap2ndNormalDirection;
                                mat2ndUV = lilCalcMatCapUV(matcap2ndNormalDirection, _MatCap2ndZRotCancel);
                            }
                            else
                        #endif
                        {
                            mat2ndUV = lilCalcMatCapUV(normalDirection, _MatCap2ndZRotCancel);
                        }
                        float4 matCap2ndColor = _MatCap2ndColor;
                        if(Exists_MatCapTex) matCap2ndColor *= LIL_SAMPLE_2D(_MatCap2ndTex, sampler_MainTex, mat2ndUV);
                        #ifndef LIL_PASS_FORWARDADD
                            matCap2ndColor.rgb = lerp(matCap2ndColor.rgb, matCap2ndColor.rgb * lightColor, _MatCap2ndEnableLighting);
                        #else
                            if(_MatCap2ndBlendMode < 3) matCap2ndColor.rgb *= lightColor * _MatCap2ndEnableLighting;
                        #endif
                        #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                            if(_MatCap2ndApplyTransparency) matCap2ndColor.a *= col.a;
                        #endif
                        if(Exists_MatCap2ndBlendMask) matCap2ndColor.a *= LIL_SAMPLE_2D(_MatCap2ndBlendMask, sampler_MainTex, uvMain).r;
                        col.rgb = lilBlendColor(col.rgb, matCap2ndColor.rgb, _MatCap2ndBlend * matCap2ndColor.a, _MatCap2ndBlendMode);
                    }
                #endif

                //----------------------------------------------------------------------------------------------------------------------
                // Rim light
                #if defined(LIL_FEATURE_RIMLIGHT)
                    #ifndef LIL_PASS_FORWARDADD
                        LIL_BRANCH
                        if(_UseRim)
                    #else
                        LIL_BRANCH
                        if(_UseRim)
                    #endif
                    {
                        #if defined(LIL_FEATURE_RIMLIGHT_DIRECTION)
                            float4 rimColor = _RimColor;
                            float4 rimIndirColor = _RimIndirColor;
                            if(Exists_RimColorTex)
                            {
                                float4 rimColorTex = LIL_SAMPLE_2D(_RimColorTex, sampler_MainTex, uvMain);
                                rimColor *= rimColorTex;
                                rimIndirColor *= rimColorTex;
                            }

                            float lnRaw = dot(lightDirection,normalDirection) * 0.5 + 0.5;
                            float lnDir = saturate((lnRaw + _RimDirRange) / (1.0 + _RimDirRange));
                            float lnIndir = saturate((1.0-lnRaw + _RimIndirRange) / (1.0 + _RimIndirRange));
                            float rim = pow(saturate(1.0 - nv), _RimFresnelPower);
                            float rimDir = lerp(rim, rim*lnDir, _RimDirStrength);
                            float rimIndir = rim * lnIndir * _RimDirStrength;
                            rimDir = lilTooning(rimDir, _RimBorder, _RimBlur);
                            rimIndir = lilTooning(rimIndir, _RimIndirBorder, _RimIndirBlur);

                            #ifndef LIL_PASS_FORWARDADD
                                if(_RimShadowMask)
                                {
                                    rimDir *= shadowmix;
                                    rimIndir *= shadowmix;
                                }
                            #endif
                            #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                                if(_RimApplyTransparency)
                                {
                                    rimDir *= col.a;
                                    rimIndir *= col.a;
                                }
                            #endif
                            float3 rimSum = rimDir * rimColor.a * rimColor.rgb + rimIndir * rimIndirColor.a * rimIndirColor.rgb;
                            #ifndef LIL_PASS_FORWARDADD
                                rimSum = lerp(rimSum, rimSum * lightColor, _RimEnableLighting);
                                col.rgb += rimSum;
                            #else
                                col.rgb += rimSum * _RimEnableLighting * lightColor;
                            #endif
                        #else
                            float4 rimColor = _RimColor;
                            if(Exists_RimColorTex) rimColor *= LIL_SAMPLE_2D(_RimColorTex, sampler_MainTex, uvMain);
                            float rim = pow(saturate(1.0 - nv), _RimFresnelPower);
                            rim = lilTooning(rim, _RimBorder, _RimBlur);
                            #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                                if(_RimApplyTransparency) rim *= col.a;
                            #endif
                            #ifndef LIL_PASS_FORWARDADD
                                if(_RimShadowMask) rim *= shadowmix;
                                rimColor.rgb = lerp(rimColor.rgb, rimColor.rgb * lightColor, _RimEnableLighting);
                                col.rgb += rim * rimColor.a * rimColor.rgb;
                            #else
                                col.rgb += rim * _RimEnableLighting * rimColor.a * rimColor.rgb * lightColor;
                            #endif
                        #endif
                    }
                #endif

                //----------------------------------------------------------------------------------------------------------------------
                // Glitter
                #if defined(LIL_FEATURE_GLITTER)
                    LIL_BRANCH
                    if(_UseGlitter)
                    {
                        #if defined(USING_STEREO_MATRICES)
                            float3 glitterViewDirection = lerp(headDirection, viewDirection, _GlitterVRParallaxStrength);
                        #else
                            float3 glitterViewDirection = viewDirection;
                        #endif
                        float4 glitterColor = _GlitterColor;
                        if(Exists_GlitterColorTex) glitterColor *= LIL_SAMPLE_2D(_GlitterColorTex, sampler_MainTex, uvMain);
                        float2 glitterPos = _GlitterUVMode ? input.uv1 : input.uv;
                        glitterColor.rgb *= lilGlitter(glitterPos, normalDirection, glitterViewDirection, lightDirection, _GlitterParams1, _GlitterParams2);
                        glitterColor.rgb = lerp(glitterColor.rgb, glitterColor.rgb * albedo, _GlitterMainStrength);
                        #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                            if(_GlitterApplyTransparency) glitterColor.a *= col.a;
                        #endif
                        #ifndef LIL_PASS_FORWARDADD
                            if(_GlitterShadowMask) glitterColor.a *= shadowmix;
                            glitterColor.rgb = lerp(glitterColor.rgb, glitterColor.rgb * lightColor, _GlitterEnableLighting);
                            col.rgb += glitterColor.rgb * glitterColor.a;
                        #else
                            col.rgb += glitterColor.a * _GlitterEnableLighting * glitterColor.rgb * lightColor;
                        #endif
                    }
                #endif

                //----------------------------------------------------------------------------------------------------------------------
                // Emission
                float3 invLighting = saturate((1.0 - lightColor) * sqrt(lightColor));
                #if defined(LIL_FEATURE_EMISSION_1ST)
                    LIL_BRANCH
                    if(_UseEmission)
                    {
                        float2 _EmissionMapParaTex = input.uv + _EmissionParallaxDepth * parallaxOffset;
                        float4 emissionColor = _EmissionColor;
                        // Texture
                        #if defined(LIL_FEATURE_EMISSION_UV) && defined(LIL_FEATURE_ANIMATE_EMISSION_UV)
                            if(Exists_EmissionMap) emissionColor *= LIL_GET_EMITEX(_EmissionMap, _EmissionMapParaTex);
                        #elif defined(LIL_FEATURE_EMISSION_UV)
                            if(Exists_EmissionMap) emissionColor *= LIL_SAMPLE_2D(_EmissionMap, sampler_EmissionMap, lilCalcUV(_EmissionMapParaTex, _EmissionMap_ST));
                        #else
                            if(Exists_EmissionMap) emissionColor *= LIL_SAMPLE_2D(_EmissionMap, sampler_EmissionMap, uvMain + _EmissionParallaxDepth * parallaxOffset);
                        #endif
                        // Mask
                        #if defined(LIL_FEATURE_EMISSION_MASK_UV) && defined(LIL_FEATURE_ANIMATE_EMISSION_MASK_UV)
                            if(Exists_EmissionBlendMask) emissionColor *= LIL_GET_EMIMASK(_EmissionBlendMask, input.uv);
                        #elif defined(LIL_FEATURE_EMISSION_MASK_UV)
                            if(Exists_EmissionBlendMask) emissionColor *= LIL_SAMPLE_2D(_EmissionBlendMask, sampler_MainTex, lilCalcUV(input.uv, _EmissionBlendMask_ST));
                        #else
                            if(Exists_EmissionBlendMask) emissionColor *= LIL_SAMPLE_2D(_EmissionBlendMask, sampler_MainTex, uvMain);
                        #endif
                        // Gradation
                        #if defined(LIL_FEATURE_EMISSION_GRADATION)
                            if(Exists_EmissionGradTex && _EmissionUseGrad) emissionColor *= LIL_SAMPLE_1D(_EmissionGradTex, sampler_linear_repeat, _EmissionGradSpeed*LIL_TIME);
                        #endif
                        #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                            emissionColor.a *= col.a;
                        #endif
                        #if defined(LIL_FEATURE_AUDIOLINK)
                            if(_AudioLink2Emission) emissionColor.a *= audioLinkValue;
                        #endif
                        emissionColor.rgb = lerp(emissionColor.rgb, emissionColor.rgb * invLighting, _EmissionFluorescence);
                        col.rgb += _EmissionBlend * lilCalcBlink(_EmissionBlink) * emissionColor.a * emissionColor.rgb;
                    }
                #endif

                // Emission2nd
                #if defined(LIL_FEATURE_EMISSION_2ND)
                    LIL_BRANCH
                    if(_UseEmission2nd)
                    {
                        float2 _Emission2ndMapParaTex = input.uv + _Emission2ndParallaxDepth * parallaxOffset;
                        float4 emission2ndColor = _Emission2ndColor;
                        // Texture
                        #if defined(LIL_FEATURE_EMISSION_UV) && defined(LIL_FEATURE_ANIMATE_EMISSION_UV)
                            if(Exists_Emission2ndMap) emission2ndColor *= LIL_GET_EMITEX(_Emission2ndMap, _Emission2ndMapParaTex);
                        #elif defined(LIL_FEATURE_EMISSION_UV)
                            if(Exists_Emission2ndMap) emission2ndColor *= LIL_SAMPLE_2D(_Emission2ndMap, sampler_Emission2ndMap, lilCalcUV(_Emission2ndMapParaTex, _Emission2ndMap_ST));
                        #else
                            if(Exists_Emission2ndMap) emission2ndColor *= LIL_SAMPLE_2D(_Emission2ndMap, sampler_Emission2ndMap, uvMain + _Emission2ndParallaxDepth * parallaxOffset);
                        #endif
                        // Mask
                        #if defined(LIL_FEATURE_EMISSION_MASK_UV) && defined(LIL_FEATURE_ANIMATE_EMISSION_MASK_UV)
                            if(Exists_Emission2ndBlendMask) emission2ndColor *= LIL_GET_EMIMASK(_Emission2ndBlendMask, input.uv);
                        #elif defined(LIL_FEATURE_EMISSION_MASK_UV)
                            if(Exists_Emission2ndBlendMask) emission2ndColor *= LIL_SAMPLE_2D(_Emission2ndBlendMask, sampler_MainTex, lilCalcUV(input.uv, _Emission2ndBlendMask_ST));
                        #else
                            if(Exists_Emission2ndBlendMask) emission2ndColor *= LIL_SAMPLE_2D(_Emission2ndBlendMask, sampler_MainTex, uvMain);
                        #endif
                        // Gradation
                        #if defined(LIL_FEATURE_EMISSION_GRADATION)
                            if(Exists_Emission2ndGradTex && _Emission2ndUseGrad) emission2ndColor *= LIL_SAMPLE_1D(_Emission2ndGradTex, sampler_linear_repeat, _Emission2ndGradSpeed*LIL_TIME);
                        #endif
                        #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                            emission2ndColor.a *= col.a;
                        #endif
                        #if defined(LIL_FEATURE_AUDIOLINK)
                            if(_AudioLink2Emission2nd) emission2ndColor.a *= audioLinkValue;
                        #endif
                        emission2ndColor.rgb = lerp(emission2ndColor.rgb, emission2ndColor.rgb * invLighting, _Emission2ndFluorescence);
                        col.rgb += _Emission2ndBlend * lilCalcBlink(_Emission2ndBlink) * emission2ndColor.a * emission2ndColor.rgb;
                    }
                #endif

                float4 fogColor = float4(0,0,0,0);
                LIL_APPLY_FOG_COLOR(col, input.fogCoord, fogColor);
                return col;
            }
            ENDHLSL
        }

        UsePass "Hidden/ltspass_transparent/SHADOW_CASTER"
        UsePass "Hidden/ltspass_transparent/META"
    }
    Fallback "Unlit/Texture"
    CustomEditor "lilToon.lilToonInspector"
}
