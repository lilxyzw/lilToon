# Developer Documentation

# Files
- lilToon
    - Editor : Editor assets
        - gui_xx : GUI assets
        - lang.txt : Language file (tsv format)
        - lilInspector.cs : ShaderGUI
        - lilStartup.cs : Startup (shader setting generation, version check, etc.)
        - lilToonAssetPostprocessor.cs : AssetPostprocessor that analyzes and automatically sets the shader settings used for the asset
        - lilToonEditorUtils.cs : MenuItem
        - lilToonPreset.cs : ScriptableObject for presets
        - lilToonPropertyDrawer.cs : MaterialPropertyDrawer
        - lilToonSetting.cs : ScriptableObject for shader settings
    - Presets
    - Shader
        - Includes : hlsl file
            - lil_common.hlsl : Common file
            - lil_common_appdata.hlsl : Declaration of appdata structure (vertex shader input)
            - lil_common_frag.hlsl : Fragment shader common file
            - lil_common_frag_alpha.hlsl : Fragment shader common file that processes only transparency
            - lil_common_functions.hlsl : Functions
            - lil_common_input.hlsl : Declaration of material variables
            - lil_common_macro.hlsl : Convert Unity macros, absorb pipeline differences
            - lil_common_vert.hlsl : Vertex shader common file
            - lil_common_vert_fur.hlsl : Fur vertex shader common file
            - lil_hdrp.hlsl : HDRP support
            - lil_pass_depthnormals.hlsl : DepthNormals path (for URP)
            - lil_pass_depthonly.hlsl : DepthOnly path (for SRP)
            - lil_pass_forward.hlsl : Forward path
            - lil_pass_forward_fur.hlsl : Forward path for fur
            - lil_pass_forward_lite.hlsl : Forward path for lite
            - lil_pass_forward_normal.hlsl : Forward path for normal
            - lil_pass_forward_gem.hlsl : Forward path for gem
            - lil_pass_forward_refblur : Forward path for refraction blur
            - lil_pass_forward_fakeshadow.hlsl : Path for FakeShadow
            - lil_pass_meta.hlsl : Path for light bake
            - lil_pass_motionvectors.hlsl : MotionVectors path (for HDRP)
            - lil_pass_shadowcaster.hlsl : ShadowCaster path
            - lil_pass_universal2d.hlsl : Universal2D path (for URP)
            - lil_pipeline.hlsl : Branching by pipeline
            - lil_replace_keywords.hlsl : Replace shader keywords as shader settings
            - lil_tessellation.hlsl : Shader for tessellation
            - lil_vert_audiolink.hlsl : Vertex shader processing for AudioLink
            - lil_vert_encryption.hlsl : Vertex shader processing for AvaterEncryption
            - lil_vert_outline.hlsl : Vertex shader processing for outline
        - lts_xx.shader : Normal shader
        - ltsl.shader : lilToonLite
        - ltsmulti.shader : Using shader keywords instead of shader settings
        - ltspass_baker.shader : Shader for texture baking
        - ltspass_xx.shader : Shaders used in UsePass from each variation
            - _o : Outline
            - _cutout : Cutout
            - _trans : Transparent
            - _fur : Fur
            - _ref : Refraction
            - _gem : Gem
            - _tess : Tessellation
            - _overlay : Overlay (transparent shader that omits other than the Forward path)
            - _fakeshadow : Shader that shifts vertices to generate pseudo shadows
            - _one : 1 path（Calculate ForwardAdd with vertex shader)
            - _two : 2 path（Calculate ForwardAdd with vertex shader and draw back face with additional path)
    - Texture
        - lil_emission_rainbow.png : Rainbow gradient texture
        - lil_noise_1d.png : 1D noise
        - lil_noise_fur.png : Noise for fur
    - CHANGELOG.md : Change log
    - DeveloperDocumentation.md : Developer documentation
    - LICENSE : License
    - MANUAL.md : Manual
    - package.json : Data for UPM
    - README.md : Readme
    - Third Party Notices.md : Third party licenses and references
- lilToonSetting : Shader settings automatically generated for each project
    - lil_setting.hlsl : Shader macros automatically generated from shader settings
    - ShaderSetting.asset : Asset for saving shader settings

# Shader structure
## About shader variations
Basically, `ltspass_xx.shader` is the shader itself, and each shader variation uses UsePass to call the pass.  
However, the following shaders have their own pass.
- lts_fakeshadow.shader
- lts_fur.shader
- lts_fur_cutout.shader
- lts_gem.shader
- lts_ref.shader
- lts_ref_blur.shader
- ltsmulti.shader
- ltsmulti_fur.shader
- ltsmulti_gem.shader
- ltsmulti_ref.shader

## About passes
There are so many variations, but I declare a macro for each shader variation in `HLSLINCLUDE` at the beginning of the shader, and call `#include "Includes/lil_pass_xx.hlsl"` in each pass to branch according to the macro. The code is made as common as possible.  
Also, although the shaders for Built-in RP / LWRP / URP / HDRP are in a common file, the editor script can switch the pipeline by rewriting each shader and `lil_pipeline.hlsl`.

## About include
In each path, `#include "Includes/lil_pass_xx.hlsl"` is called, and the hlsl file for each path is basically unified with the following structure.
- lil_pass_xx.hlsl
    - lil_pipeline.hlsl
        - Unity library
        - lil_common.hlsl
            - lil_setting.hlsl
            - lil_common_macro.hlsl
            - lil_common_input.hlsl
            - lil_common_functions.hlsl
    - lil_common_appdata.hlsl
    - Declare a v2f structure for each path
    - lil_common_vert.hlsl
    - lil_common_frag.hlsl
    - Pixel shader

## About shader keywords
The following shader keywords are used in `lilToonMulti`.  
These match the built-in shader keywords, so the shader keyword limit is avoided and no warning is displayed by the VRCSDK.
```
ETC1_EXTERNAL_ALPHA UNITY_UI_ALPHACLIP UNITY_UI_CLIP_RECT EFFECT_HUE_VARIATION _COLORADDSUBDIFF_ON _COLORCOLOR_ON _SUNDISK_NONE GEOM_TYPE_FROND _COLOROVERLAY_ON _REQUIRE_UV2 ANTI_FLICKER _EMISSION GEOM_TYPE_BRANCH _SUNDISK_SIMPLE _NORMALMAP EFFECT_BUMP _GLOSSYREFLECTIONS_OFF _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A _SPECULARHIGHLIGHTS_OFF GEOM_TYPE_MESH _METALLICGLOSSMAP GEOM_TYPE_LEAF _SPECGLOSSMAP _PARALLAXMAP PIXELSNAP_ON BILLBOARD_FACE_CAMERA_POS _FADING_ON _MAPPING_6_FRAMES_LAYOUT _SUNDISK_HIGH_QUALITY GEOM_TYPE_BRANCH_DETAIL _DETAIL_MULX2
```

# How to make a custom shader

## Basic process
Basically, write the macro in `HLSLINCLUDE` of the main shader.  
If an error occurs in a specific path, it is recommended to `#undef` in that path and re-declare it if necessary.

## Creating a shader file
The shader path is described in `ltspass_xx.shader` etc. and is referenced using `UsePass` from each shader variation.  
Create a new shader and copy the code from `ScriptTemplates/99-lilToon__Custom Pass Shader-custom_ltspass_opaque.shader.txt`.  
Create `custom_ltspass_opaque.shader` as an example.  
Rewrite `Shader "Hidden/#NAME#"` at the beginning of the file to `Shader "Hidden/custom_ltspass_opaque"`.

## Creating shader variations
Duplicate `lts.shader` and rename it to `custom_lts.shader`.  
Rewrite `Shader "lilToon"` at the beginning of the file to `Shader "lilToonCustomExample/Opaque"`.  
Replace the `UsePass "Hidden/ltspass_opaque/xx"` part in the shader with the `UsePass "Hidden/custom_ltspass_opaque/xx"` you created earlier.

## Add material variable
First, add any property to the `Properties` block in `custom_lts.shader`.  
This time, add the following.
```
        //----------------------------------------------------------------------------------------------------------------------
        // Custom
        [lilVec3]       _CustomVertexWaveScale      ("Vertex Wave Scale", Vector) = (10.0,10.0,10.0,0.0)
        [lilVec3]       _CustomVertexWaveStrength   ("Vertex Wave Strength", Vector) = (0.0,0.1,0.0,0.0)
                        _CustomVertexWaveSpeed      ("Vertex Wave Speed", float) = 10.0
        [NoScaleOffset] _CustomVertexWaveMask       ("Vertex Wave Mask", 2D) = "white" {}
```
This is the end of editing `custom_lts.shader`.  
If you want to customize the editor, change `CustomEditor "lilToon.lilToonInspector"` to something else.

Then edit `custom_ltspass_opaque.shader`.  
Variables can be inserted with the macro `#define LIL_CUSTOM_PROPERTIES`, and Texture2D and SamplerState can be inserted with the macro `#define LIL_CUSTOM_TEXTURES`.  
Write the macro as follows.

```HLSL
#define LIL_CUSTOM_PROPERTIES \
    float4  _CustomVertexWaveScale; \
    float4  _CustomVertexWaveStrength; \
    float   _CustomVertexWaveSpeed;

#define LIL_CUSTOM_TEXTURES \
    TEXTURE2D(_CustomVertexWaveMask);
```

## Add functions and include
Some libraries depend on Unity's functions and variables.  
In this case, insert the function or include just before `#include "Includes/lil_pass_xx.hlsl"` that exists in each path.

## Add vertex shader input (appdata structure)
The following keywords can be `#define` to add the corresponding input.
|Keyword|Variable name|Semantics|
|-|-|-|
|LIL_REQUIRE_APP_POSITION|positionOS|POSITION|
|LIL_REQUIRE_APP_TEXCOORD0|uv|TEXCOORD0|
|LIL_REQUIRE_APP_TEXCOORD1|uv1|TEXCOORD1|
|LIL_REQUIRE_APP_TEXCOORD2|uv2|TEXCOORD2|
|LIL_REQUIRE_APP_TEXCOORD3|uv3|TEXCOORD3|
|LIL_REQUIRE_APP_TEXCOORD4|uv4|TEXCOORD4|
|LIL_REQUIRE_APP_TEXCOORD5|uv5|TEXCOORD5|
|LIL_REQUIRE_APP_TEXCOORD6|uv6|TEXCOORD6|
|LIL_REQUIRE_APP_TEXCOORD7|uv7|TEXCOORD7|
|LIL_REQUIRE_APP_COLOR|color|COLOR|
|LIL_REQUIRE_APP_NORMAL|normalOS|NORMAL|
|LIL_REQUIRE_APP_TANGENT|tangentOS|TANGENT|
|LIL_REQUIRE_APP_VERTEXID|vertexID|SV_VertexID|

## Add vertex shader output (v2f structure)
Members of the structure can be added from `#define LIL_CUSTOM_V2F_MEMBER`.  
Those originally included in the structure can be forced to become members with `#define LIL_V2F_FORCE_(keyword)`.

## Inserting a process into the vertex shader
You can use the following macro to insert the process.

|Name|Description|
|-|-|
|LIL_CUSTOM_VERTEX_OS|Processing in object space|
|LIL_CUSTOM_VERTEX_WS|Processing in world space|

In this example, we will add a wave animation.
```HLSL
#define LIL_CUSTOM_VERTEX_OS \
    float3 customWaveStrength = LIL_SAMPLE_2D_LOD(_CustomVertexWaveMask, sampler_linear_repeat, input.uv, 0).r * _CustomVertexWaveStrength.xyz; \
    positionOS.xyz += sin(LIL_TIME * _CustomVertexWaveSpeed + dot(positionOS.xyz, _CustomVertexWaveScale.xyz)) * customWaveStrength;
```

Coordinates and normals of the world space are stored in `vertexInput` and `vertexNormalInput`, although we did not use them this time.
```HLSL
#define LIL_CUSTOM_VERTEX_WS \
    vertexInput.positionWS = CustomSomething(vertexInput.positionWS);
```

## Inserting a process into pixel shader
You can insert or overwrite processes with `BEFORE_(keyword)` or `OVERRIDE_(keyword)`.  
The following keywords are currently supported (more may be added upon request)

|Name|Description|
|-|-|
|UNPACK_V2F|Unpack of v2f structure|
|ANIMATE_MAIN_UV|Animation for main UV|
|ANIMATE_OUTLINE_UV|Animation for outline UV|
|PARALLAX|Parallax map|
|MAIN|Main color|
|OUTLINE_COLOR|Outline color|
|FUR|(for fur shader) Fur processing|
|ALPHAMASK|Alpha mask|
|DISSOLVE|Dissolve|
|NORMAL_1ST|Normal map|
|NORMAL_2ND|Normal map 2nd|
|ANISOTROPY|Anisotropy|
|AUDIOLINK|AudioLink|
|MAIN2ND|Main color 2nd|
|MAIN3RD|Main color 3rd|
|SHADOW|Shadow|
|BACKLIGHT|Backlight|
|REFRACTION|Refraction|
|REFLECTION|Reflection|
|MATCAP|MatCap|
|MATCAP_2ND|MatCap 2nd|
|RIMLIGHT|Rim light|
|GLITTER|Glitter|
|EMISSION_1ST|Emission|
|EMISSION_2ND|Emission 2nd|
|DISSOLVE_ADD|Dissolve border emission|
|BLEND_EMISSION|Emission blending|
|DISTANCE_FADE|Distance fade|
|FOG|Fog|
|OUTPUT|Output|

Example:
```HLSL
#define BEFORE_UNPACK_V2F \
    fd.uv0 = input.uv0;
```

## Custom Inspector
The template for the extended Inspector can be found in `ScriptTemplates/99-lilToon__Custom Inspector-NewCustomInspector.cs.txt`.  
You can easily customize the Inspector by inheriting `lilToon.lilToonInspector`.  
The editing procedure is as follows:
1. Declare `MaterialProperty`
2. Override `LoadCustomProperties()` and get the properties with `FindProperty` while setting `isCustomShader` to `true`.
3. Override `DrawCustomProperties()` to implement GUI.

You can use the following GUIStyle for lilToon.

|Name|Description|
|-|-|
|boxOuter|Outside of property box|
|boxInnerHalf|inside of the property box (the upper part is square for drawing labels)|
|boxInner|inside of the property box (the upper part is a circle to make it nicer without labels)|
|customBox|Similar to Unity's default box, but with borders for better visibility|
|customToggleFont|Bold font used for property box labels|
|offsetButton|Button indented to fit the property box|

You can also use functions such as the following.
|Name|Description|
|-|-|
|bool Foldout(string title, string help, bool display)|Drawing a foldout|
|void DrawLine()|Draw a line|
|void DrawWebButton(string text, string URL)|Draw a web button|
|void LoadCustomLanguage(string langFileGUID)|Load custom language file|

In this case, I created CustomInspectorExample.cs and edited it as shown below.
```C#
// This script should be placed in the lilToon/Editor folder

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace lilToon
{
    public class CustomInspectorExample : lilToonInspector
    {
        // Custom properties
        MaterialProperty customVertexWaveScale;
        MaterialProperty customVertexWaveStrength;
        MaterialProperty customVertexWaveSpeed;
        MaterialProperty customVertexWaveMask;

        private static bool isShowCustomProperties;

        protected override void LoadCustomProperties(MaterialProperty[] props, Material material)
        {
            isCustomShader = true;
            //LoadCustomLanguage("");
            customVertexWaveScale = FindProperty("_CustomVertexWaveScale", props);
            customVertexWaveStrength = FindProperty("_CustomVertexWaveStrength", props);
            customVertexWaveSpeed = FindProperty("_CustomVertexWaveSpeed", props);
            customVertexWaveMask = FindProperty("_CustomVertexWaveMask", props);
        }

        protected override void DrawCustomProperties(
            MaterialEditor materialEditor,
            Material material,
            GUIStyle boxOuter,          // outer box
            GUIStyle boxInnerHalf,      // inner box
            GUIStyle boxInner,          // inner box without label
            GUIStyle customBox,         // box (similar to unity default box)
            GUIStyle customToggleFont,  // bold font
            GUIStyle offsetButton)      // button with indent
        {
            isShowCustomProperties = Foldout("Vertex Wave & Emission UV", "Vertex Wave & Emission UV", isShowCustomProperties);
            if(isShowCustomProperties)
            {
                // Vertex Wave
                EditorGUILayout.BeginVertical(boxOuter);
                EditorGUILayout.LabelField(GetLoc("Vertex Wave"), customToggleFont);
                EditorGUILayout.BeginVertical(boxInnerHalf);

                materialEditor.ShaderProperty(customVertexWaveScale, "Scale");
                materialEditor.ShaderProperty(customVertexWaveStrength, "Strength");
                materialEditor.ShaderProperty(customVertexWaveSpeed, "Speed");
                materialEditor.TexturePropertySingleLine(new GUIContent("Mask", "Strength (R)"), customVertexWaveMask);

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
            }
        }
    }
}
#endif
```

# Macros & Functions
There are several macros and functions to keep the rendering pipeline compatible.  
Please refer to the HLSL file for more information.

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
|LIL_GET_VIEWDIR_WS(float3 positionWS)|Calculate view direction (without normalize)|

## Position Transform (LIL_VERTEX_POSITION_INPUTS)
|Name|Description|
|-|-|
|LIL_VERTEX_POSITION_INPUTS(float4 positionOS, out vertexInput)|Position transform|
|vertexInput.positionWS|World space position|
|vertexInput.positionVS|View space position|
|vertexInput.positionCS|Clip space position (SV_POSITION)|
|vertexInput.positionSS|Screen space position|

## Vector Transform (LIL_VERTEX_NORMAL_TANGENT_INPUTS)
|Name|Description|
|-|-|
|LIL_VERTEX_NORMAL_TANGENT_INPUTS(float3 normalOS, float4 tangentOS, out vertexNormalInput)|Normal transform|
|vertexNormalInput.tangentWS|World space tangent|
|vertexNormalInput.bitangentWS|World space bitangent|
|vertexNormalInput.normalWS|World space normal|

## Utility
|Name|Description|
|-|-|
|float2 lilCalcUV(float2 uv, float4 tex_ST, float4 tex_ScrollRotate)|Calculate UV|
|float3 lilBlendColor(float3 dstCol, float3 srcCol, float srcA, uint blendMode)|Color blending with Blend mode (0: Normal / 1: Add / 2: Screen / 3: Multiply)|
|float3 UnpackNormalScale(float4 normalTex, float scale)|Unpack normal map|
|float lilTooning()|Convert input values to toon|

# Fragment shader
Variables are managed using a common structure called lilFragData.  
Check lil_common.hlsl for more information.