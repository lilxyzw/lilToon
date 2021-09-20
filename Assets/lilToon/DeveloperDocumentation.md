# Developer Documentation

# Shader Properties
If your shader has custom properties, you can run FindProperty in LoadCustomProperties and display the GUI in DrawCustomProperties.

lilToon/Editor/lilInspector.cs
```C#
        //------------------------------------------------------------------------------------------------------------------------------
        // Custom properties
        // If there are properties you have added, add them here.
        static bool isCustomShader = false;

        // Add properties
        MaterialProperty exampleProperty;

        void LoadCustomProperties(MaterialProperty[] props, Material material)
        {
            // Check shader name
            if(material.shader.name.Contains("Example"))
            {
                // Set isCustomShader to true to show custom properties
                isCustomShader = true;
                // Load
                exampleProperty = FindProperty("_ExampleProperty", props);
            }
        }

        void DrawCustomProperties(MaterialEditor materialEditor, Material material)
        {
            // Check shader name
            if(material.shader.name.Contains("Example"))
            {
                // Draw GUI
                materialEditor.ShaderProperty(exampleProperty, "Example Property");
            }
        }
```

# Input variables
The variables are declared in "lil_input.hlsl".  
If you want to make your shader compatible with SRP Batcher, please copy and edit "lil_input.hlsl".  
Otherwise, just declare the variable in your shader.

# Macros & Functions
There are several macros and functions to keep the rendering pipeline compatible.  
Please refer to "lil_macro.hlsl" & "lil_functions.hlsl" for more information.

## Texture
|Name|Description|
|-|-|
|TEXTURE2D(_Tex)|Texture2D _Tex|
|SAMPLER(sampler_Tex)|SamplerState sampler_Tex|
|LIL_SAMPLE_2D(Texture2D _Tex, SamplerState sampler_Tex, float2 uv)|_Tex.Sample(sampler_Tex, uv)|

## Transform
|Name|Description|
|-|-|
|LIL_MATRIX_M|World matrix|
|LIL_MATRIX_I_M|Inverse of world matrix|
|LIL_MATRIX_V|View matrix|
|LIL_MATRIX_VP|View * projection matrix|
|LIL_TRANSFORM_POS_OS_TO_WS(float4 positionOS)|Transform position object space to world space|
|LIL_TRANSFORM_POS_WS_TO_CS(float3 positionWS)|Transform position world space to clip space|
|LIL_GET_VIEWDIR_WS(float3 positionWS)|Calculate view direction (without normalize)|

## Position Transform (LIL_VERTEX_POSITION_INPUTS)
|Name|Description|
|-|-|
|LIL_VERTEX_POSITION_INPUTS(float4 positionOS, out vertexInput)|Position transform|
|vertexInput.positionWS|World space position|
|vertexInput.positionVS|View space position|
|vertexInput.positionCS|Clip space position (SV_POSITION)|
|vertexInput.positionSS|Screen space position (for fragment shader)|

## Vector Transform (LIL_VERTEX_NORMAL_TANGENT_INPUTS)
|Name|Description|
|-|-|
|LIL_VERTEX_NORMAL_TANGENT_INPUTS(float3 normalOS, float4 tangentOS, out vertexNormalInput)|Normal transform|
|vertexNormalInput.tangentWS|World space tangent|
|vertexNormalInput.bitangentWS|World space bitangent|
|vertexNormalInput.normalWS|World space normal|

## Lighting
|Name|Description|
|-|-|
|float4 _MainLightColor|Color of main light (_LightColor0)|
|float4 _MainLightPosition|Color of main light (_WorldSpaceLightPos0)|
|float3 lilShadeSH(float3 normalWS)|Calculation of environment light|
|LIL_GET_MAINLIGHT(input, out float3 lightColor, out float3 lightDirection, out float attenuation)|Calculation of main light|
|LIL_GET_VERTEXLIGHT(input, out float3 vertexLightColor)|Copy of vertex lighting|
|LIL_GET_ADDITIONALLIGHT(float3 positionWS, out float3 additionalLightColor)|Additional lighting (for SRP)|
|LIL_GET_SHADING()|Calculation of shadow color|

## Fog
|Name|Description|
|-|-|
|LIL_FOG_COORDS(idx)|Add a fog member to a structure|
|LIL_TRANSFER_FOG(vertexInput, output)|Calculate fog from position|
|LIL_APPLY_FOG(inout col, input.fogCoord)|Apply fog|

## Shadow
|Name|Description|
|-|-|
|LIL_SHADOW_COORDS(idx)|Add a shadow member to a structure|
|LIL_TRANSFER_SHADOW(vertexInput, input.uv1, output)|Calculate shadow from position|

## Utility
|Name|Description|
|-|-|
|float2 lilCalcUV(float2 uv, float4 tex_ST, float4 tex_ScrollRotate)|Calculate UV|
|float3 lilBlendColor(float3 dstCol, float3 srcCol, float srcA, uint blendMode)|Color blending with Blend mode (0: Normal / 1: Add / 2: Screen / 3: Multiply)|
|float3 UnpackNormalScale(float4 normalTex, float scale)|Unpack normal map|
|float lilTooning()|Convert input values to toon|

# Structure
## Vertex Shader Inputs (struct appdata)
You can add input to the appdata structure by defining the following keywords.
|Name|Description|
|-|-|
|LIL_REQUIRE_APP_POSITION|positionOS|
|LIL_REQUIRE_APP_TEXCOORD0|uv|
|LIL_REQUIRE_APP_TEXCOORD1|uv1|
|LIL_REQUIRE_APP_TEXCOORD2|uv2|
|LIL_REQUIRE_APP_TEXCOORD3|uv3|
|LIL_REQUIRE_APP_TEXCOORD4|uv4|
|LIL_REQUIRE_APP_TEXCOORD5|uv5|
|LIL_REQUIRE_APP_TEXCOORD6|uv6|
|LIL_REQUIRE_APP_TEXCOORD7|uv7|
|LIL_REQUIRE_APP_COLOR|color|
|LIL_REQUIRE_APP_NORMAL|normalOS|
|LIL_REQUIRE_APP_TANGENT|tangentOS|

## Vertex Shader Outputs (struct v2f)
I recommend editing `lil_custom_v2f.hlsl` for customization.
|Name|Description|
|-|-|
|positionCS|Clip space position|
|positionOS|Object space position|
|positionWS|World space position|
|positionSS|Screen space coordinates|
|uv|UV|
|uvMat|MatCap UV|
|furLayer|Fur Layer (in:0 out:1)|
|LIL_LIGHTCOLOR_COORDS|Light color|
|LIL_LIGHTDIRECTION_COORDS|Light direction|
|LIL_INDLIGHTCOLOR_COORDS|Indirect light color|
|LIL_VERTEXLIGHT_COORDS|Vertex lighting|
|LIL_FOG_COORDS()|Fog|
|LIL_SHADOW_COORDS()|Shadow|
|LIL_LIGHTMAP_COORDS()|Lightmap|
|LIL_VERTEX_INPUT_INSTANCE_ID|Instance ID|
|LIL_VERTEX_OUTPUT_STEREO|Stereo target eye index|

# Vertex shader
Vertex shader can be found in "lil_normal_vertex.hlsl".  
"LIL_VERTEX_POSITION_INPUTS" and "LIL_VERTEX_NORMAL_TANGENT_INPUTS" are used for transforms.  
You can easily modify vertices by making changes to the retrieved structure.

# Fragment shader
Fragment shader can be found in "lil_normal_fragment.hlsl".  
Commonly used variables are as follows.
|Name|Description|
|-|-|
|v2f input|Input from vertex shader|
|float4 col|Output color|
|float3 albedo|Unlit color|
|float2 uvMain|UV transformed by _MainTex|
|float facing|Surface orientation|
|float3 lightColor|Color of light (main light & sh light)|
|float3 lightDirection|Direction of light (main light & sh light)|
|float attenuation|Attenuation of light|
|float3 addLightColor|Color of additional light|
|float3 normalDirection|World space normal|
|float3 viewDirection|View direction|
|float3x3 tbnWS|float3x3(input.tangentWS, input.bitangentWS, input.normalWS)|

# Shader Setting
Shader setting is defined in "lil_setting.hlsl".

lilToonSetting/lil_setting.hlsl
```HLSL
#ifndef LIL_SETTING_INCLUDED
#define LIL_SETTING_INCLUDED

#define LIL_FEATURE_ANIMATE_MAIN_UV
#define LIL_FEATURE_MAIN_TONE_CORRECTION
#define LIL_FEATURE_MAIN_GRADATION_MAP
#define LIL_FEATURE_MAIN2ND
#define LIL_FEATURE_MAIN3RD
#define LIL_FEATURE_DECAL
#define LIL_FEATURE_ANIMATE_DECAL
#define LIL_FEATURE_TEX_LAYER_MASK
#define LIL_FEATURE_LAYER_DISSOLVE
#define LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE
#define LIL_FEATURE_ALPHAMASK
#define LIL_FEATURE_SHADOW
#define LIL_FEATURE_RECEIVE_SHADOW
#define LIL_FEATURE_TEX_SHADOW_BLUR
#define LIL_FEATURE_TEX_SHADOW_BORDER
#define LIL_FEATURE_TEX_SHADOW_STRENGTH
#define LIL_FEATURE_TEX_SHADOW_1ST
#define LIL_FEATURE_TEX_SHADOW_2ND
#define LIL_FEATURE_EMISSION_1ST
#define LIL_FEATURE_EMISSION_2ND
#define LIL_FEATURE_EMISSION_UV
#define LIL_FEATURE_ANIMATE_EMISSION_UV
#define LIL_FEATURE_TEX_EMISSION_MASK
#define LIL_FEATURE_EMISSION_MASK_UV
#define LIL_FEATURE_ANIMATE_EMISSION_MASK_UV
#define LIL_FEATURE_EMISSION_GRADATION
#define LIL_FEATURE_NORMAL_1ST
#define LIL_FEATURE_NORMAL_2ND
#define LIL_FEATURE_TEX_NORMAL_MASK
#define LIL_FEATURE_REFLECTION
#define LIL_FEATURE_TEX_REFLECTION_SMOOTHNESS
#define LIL_FEATURE_TEX_REFLECTION_METALLIC
#define LIL_FEATURE_TEX_REFLECTION_COLOR
#define LIL_FEATURE_MATCAP
#define LIL_FEATURE_MATCAP_2ND
#define LIL_FEATURE_TEX_MATCAP_MASK
#define LIL_FEATURE_TEX_MATCAP_NORMALMAP
#define LIL_FEATURE_RIMLIGHT
#define LIL_FEATURE_TEX_RIMLIGHT_COLOR
#define LIL_FEATURE_RIMLIGHT_DIRECTION
#define LIL_FEATURE_GLITTER
#define LIL_FEATURE_PARALLAX
#define LIL_FEATURE_POM
#define LIL_FEATURE_CLIPPING_CANCELLER
#define LIL_FEATURE_DISTANCE_FADE
#define LIL_FEATURE_AUDIOLINK
#define LIL_FEATURE_AUDIOLINK_VERTEX
#define LIL_FEATURE_TEX_AUDIOLINK_MASK
#define LIL_FEATURE_AUDIOLINK_LOCAL
#define LIL_FEATURE_DISSOLVE
#define LIL_FEATURE_TEX_DISSOLVE_NOISE
#define LIL_FEATURE_TEX_OUTLINE_COLOR
#define LIL_FEATURE_OUTLINE_TONE_CORRECTION
#define LIL_FEATURE_ANIMATE_OUTLINE_UV
#define LIL_FEATURE_TEX_OUTLINE_WIDTH
#define LIL_FEATURE_TEX_FUR_NORMAL
#define LIL_FEATURE_TEX_FUR_MASK
#define LIL_FEATURE_TEX_FUR_LENGTH

#endif
```

# Shader Customization
You can edit the "lil_macro.hlsl" file to change the shader behavior.  
Disabling the feature can help with game performance.  
However, I do not recommend changing it on platforms where you can use a variety of shaders, as it can cause lighting issues.

```HLSL
// Dither shadow (Default : 1)
// 0 : Off
// 1 : On
#define LIL_SHADOW_DITHER 1

// Premultiply on ForwardAdd (Default : 1)
// 0 : Off
// 1 : On (for BlendOp Max)
#define LIL_PREMULTIPLY_FA 1

// Light direction mode (Default : 1)
// 0 : Directional light Only
// 1 : Blend SH light
#define LIL_LIGHT_DIRECTION_MODE 1

// Vertex light mode (Default : 3)
// 0 : Off (VRChat Mobile Toon Lit / MnMrShader / Reflex Shader / MToon / UTS2)
// 1 : Simple
// 2 : Accurate
// 3 : Approximate value of _LightTextureB0
// 4 : Lookup _LightTextureB0
#define LIL_VERTEXLIGHT_MODE 3

// ForwardAdd
// 0 : Off (VRChat Mobile Toon Lit / UnlitWF)
// 1 : On (All others)
// In UnlitWF, ForwardAdd passes are handled by the vertex shader instead.

// Refraction blur
#define LIL_REFRACTION_SAMPNUM 8
#define LIL_REFRACTION_GAUSDIST(i) exp(-(float)i*(float)i/(LIL_REFRACTION_SAMPNUM*LIL_REFRACTION_SAMPNUM/2.0))

// Vertex light mode (Default : 0)
// 0 : BRP Specular
// 1 : URP Specular
// 2 : Fast Specular
#define LIL_SPECULAR_MODE 0

// MatCap mode (Default : 1)
// 0 : Simple
// 1 : Fix Z-Rotation
#define LIL_MATCAP_MODE 1

// Antialias mode (Default : 1)
// 0 : Off
// 1 : On
#define LIL_ANTIALIAS_MODE 1

// Light Probe Proxy Volumes
#define LIL_LPPV_MODE 0
// 0 : Off
// 1 : On

// Transform Optimization
#define LIL_OPTIMIZE_TRANSFORM 0
// 0 : Off
// 1 : On
```