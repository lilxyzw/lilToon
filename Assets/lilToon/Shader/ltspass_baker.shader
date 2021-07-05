Shader "Hidden/ltsother_baker"
{
    Properties
    {
        //----------------------------------------------------------------------------------------------------------------------
        // Main
                        _Color                      ("Color", Color) = (1,1,1,1)
                        _MainTex                    ("Texture", 2D) = "white" {}
        [lilUVAnim]     _MainTex_ScrollRotate       ("Angle|UV Animation|Scroll|Rotate", Vector) = (0,0,0,0)
        [lilHSVG]       _MainTexHSVG                ("Hue|Saturation|Value|Gamma", Vector) = (0,1,1,1)

        //----------------------------------------------------------------------------------------------------------------------
        // Main2nd
        [lilToggleLeft] _UseMain2ndTex              ("Use Main 2nd", Int) = 0
                        _Color2nd                   ("Color", Color) = (1,1,1,1)
                        _Main2ndTex                 ("Texture", 2D) = "white" {}
        [lilAngle]      _Main2ndTexAngle            ("Angle", Float) = 0
        [lilToggle]     _Main2ndTexIsDecal          ("As Decal", Int) = 0
        [lilToggle]     _Main2ndTexIsLeftOnly       ("Left Only", Int) = 0
        [lilToggle]     _Main2ndTexIsRightOnly      ("Right Only", Int) = 0
        [lilToggle]     _Main2ndTexShouldCopy       ("Copy", Int) = 0
        [lilToggle]     _Main2ndTexShouldFlipMirror ("Flip Mirror", Int) = 0
        [lilToggle]     _Main2ndTexShouldFlipCopy   ("Flip Copy", Int) = 0
        [lilToggle]     _Main2ndTexIsMSDF           ("As MSDF", Int) = 0
        [NoScaleOffset] _Main2ndBlendMask           ("Mask", 2D) = "white" {}
        [lilBlendMode]  _Main2ndTexBlendMode        ("Blend Mode|Normal|Add|Screen|Multiply", Int) = 0

        //----------------------------------------------------------------------------------------------------------------------
        // Main3rd
        [lilToggleLeft] _UseMain3rdTex              ("Use Main 3rd", Int) = 0
                        _Color3rd                   ("Color", Color) = (1,1,1,1)
                        _Main3rdTex                 ("Texture", 2D) = "white" {}
        [lilAngle]      _Main3rdTexAngle            ("Angle", Float) = 0
        [lilToggle]     _Main3rdTexIsDecal          ("As Decal", Int) = 0
        [lilToggle]     _Main3rdTexIsLeftOnly       ("Left Only", Int) = 0
        [lilToggle]     _Main3rdTexIsRightOnly      ("Right Only", Int) = 0
        [lilToggle]     _Main3rdTexShouldCopy       ("Copy", Int) = 0
        [lilToggle]     _Main3rdTexShouldFlipMirror ("Flip Mirror", Int) = 0
        [lilToggle]     _Main3rdTexShouldFlipCopy   ("Flip Copy", Int) = 0
        [lilToggle]     _Main3rdTexIsMSDF           ("As MSDF", Int) = 0
        [NoScaleOffset] _Main3rdBlendMask           ("Mask", 2D) = "white" {}
        [lilBlendMode]  _Main3rdTexBlendMode        ("Blend Mode|Normal|Add|Screen|Multiply", Int) = 0
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _ _TRIMASK _ALPHAMASK _NORMAL_DXGL

            //------------------------------------------------------------------------------------------------------------------
            // Shader
            #define LIL_BAKER
            #define LIL_WITHOUT_ANIMATION
            #include "Includes/lil_pipeline.hlsl"

            struct appdata
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                float4 tangentOS    : TANGENT;
            };

            struct v2f
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float  tangentW     : TEXCOORD1;
            };

            v2f vert(appdata input)
            {
                v2f o;
                LIL_VERTEX_POSITION_INPUTS(input.positionOS, vertexInput);
                o.positionCS    = vertexInput.positionCS;
                o.uv = input.uv;
                o.tangentW = input.tangentOS.w;
                return o;
            }

            float4 frag(v2f input) : SV_Target
            {
                #ifdef _TRIMASK
                    float4 col1 = LIL_SAMPLE_2D(_MainTex,sampler_MainTex,input.uv);
                    float4 col2 = LIL_SAMPLE_2D(_Main2ndTex,sampler_Main2ndTex,input.uv);
                    float4 col3 = LIL_SAMPLE_2D(_Main3rdTex,sampler_Main3rdTex,input.uv);
                    float mat = lilGray(col1.rgb);
                    float rim = lilGray(col2.rgb);
                    float emi = lilGray(col3.rgb);
                    float4 col = float4(mat,rim,emi,1);
                #elif _ALPHAMASK
                    float4 col1 = LIL_SAMPLE_2D(_MainTex,sampler_MainTex,input.uv);
                    float4 col2 = LIL_SAMPLE_2D(_Main2ndTex,sampler_Main2ndTex,input.uv);
                    float4 col = float4(col1.rgb,col2.r);
                #elif _NORMAL_DXGL
                    float4 col = LIL_SAMPLE_2D(_MainTex,sampler_MainTex,input.uv);
                    col.g = 1.0 - col.g;
                #else
                    // Main
                    float4 col = LIL_SAMPLE_2D(_MainTex,sampler_MainTex,input.uv);
                    col.rgb = lilToneCorrection(col.rgb, _MainTexHSVG);
                    col *= _Color;

                    bool isRightHand = input.tangentW > 0.0;

                    // 2nd
                    UNITY_BRANCH
                    if(_UseMain2ndTex)
                    {
                        _Color2nd *= LIL_GET_SUBTEX(_Main2ndTex,input.uv);
                        col.rgb = lilBlendColor(col.rgb, _Color2nd.rgb, LIL_SAMPLE_2D(_Main2ndBlendMask,sampler_MainTex,input.uv).r * _Color2nd.a, _Main2ndTexBlendMode);
                    }

                    // 3rd
                    UNITY_BRANCH
                    if(_UseMain3rdTex)
                    {
                        _Color3rd *= LIL_GET_SUBTEX(_Main3rdTex,input.uv);
                        col.rgb = lilBlendColor(col.rgb, _Color3rd.rgb, LIL_SAMPLE_2D(_Main3rdBlendMask,sampler_MainTex,input.uv).r * _Color3rd.a, _Main3rdTexBlendMode);
                    }
                #endif

                return col;
            }
            ENDHLSL
        }
    }
}
