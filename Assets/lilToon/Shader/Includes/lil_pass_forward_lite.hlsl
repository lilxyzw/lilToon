#ifndef LIL_PASS_FORWARD_LITE_INCLUDED
#define LIL_PASS_FORWARD_LITE_INCLUDED

#include "Includes/lil_pipeline.hlsl"
#include "Includes/lil_common_appdata.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Structure
#if !defined(LIL_CUSTOM_V2F_MEMBER)
    #define LIL_CUSTOM_V2F_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7)
#endif

#if defined(LIL_OUTLINE)
    #define LIL_V2F_POSITION_CS
    #define LIL_V2F_TEXCOORD0
    #if defined(LIL_V2F_FORCE_TEXCOORD1) || defined(LIL_USE_LIGHTMAP_UV)
        #define LIL_V2F_TEXCOORD1
    #endif
    #if defined(LIL_V2F_FORCE_POSITION_WS) || defined(LIL_PASS_FORWARDADD) || !defined(LIL_BRP) || defined(LIL_USE_LPPV)
        #define LIL_V2F_POSITION_WS
    #endif
    #if defined(LIL_V2F_FORCE_NORMAL) || defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SUBTRACTIVE) || defined(LIL_HDRP)
        #define LIL_V2F_NORMAL_WS
    #endif
    #if !defined(LIL_PASS_FORWARDADD)
        #define LIL_V2F_LIGHTCOLOR
        #define LIL_V2F_VERTEXLIGHT
    #endif
    #define LIL_V2F_FOG

    struct v2f
    {
        float4 positionCS       : SV_POSITION;
        float2 uv               : TEXCOORD0;
        #if defined(LIL_V2F_TEXCOORD1)
            float2 uv1              : TEXCOORD1;
        #endif
        #if defined(LIL_V2F_POSITION_WS)
            float3 positionWS       : TEXCOORD2;
        #endif
        #if defined(LIL_V2F_NORMAL_WS)
            float3 normalWS         : TEXCOORD3;
        #endif
        LIL_LIGHTCOLOR_COORDS(4)
        LIL_VERTEXLIGHT_COORDS(5)
        LIL_FOG_COORDS(6)
        LIL_CUSTOM_V2F_MEMBER(7,8,9,10,11,12,13,14)
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };
#else
    #define LIL_V2F_POSITION_WS
    #define LIL_V2F_POSITION_CS
    #define LIL_V2F_TEXCOORD0
    #if defined(LIL_V2F_FORCE_TEXCOORD1) || defined(LIL_USE_LIGHTMAP_UV)
        #define LIL_V2F_TEXCOORD1
    #endif
    #define LIL_V2F_NORMAL_WS
    #define LIL_V2F_UVMAT
    #if !defined(LIL_PASS_FORWARDADD)
        #define LIL_V2F_LIGHTCOLOR
        #define LIL_V2F_LIGHTDIRECTION
        #define LIL_V2F_INDLIGHTCOLOR
        #define LIL_V2F_VERTEXLIGHT
    #endif
    #define LIL_V2F_FOG

    struct v2f
    {
        float4 positionCS       : SV_POSITION;
        float2 uv               : TEXCOORD0;
        #if defined(LIL_V2F_TEXCOORD1)
            float2 uv1              : TEXCOORD1;
        #endif
        float2 uvMat            : TEXCOORD2;
        float3 normalWS         : TEXCOORD3;
        float3 positionWS       : TEXCOORD4;
        LIL_LIGHTCOLOR_COORDS(5)
        LIL_LIGHTDIRECTION_COORDS(6)
        LIL_INDLIGHTCOLOR_COORDS(7)
        LIL_VERTEXLIGHT_COORDS(8)
        LIL_FOG_COORDS(9)
        LIL_CUSTOM_V2F_MEMBER(10,11,12,13,14,15,16,17)
        LIL_VERTEX_INPUT_INSTANCE_ID
        LIL_VERTEX_OUTPUT_STEREO
    };
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Shader
#include "Includes/lil_common_vert.hlsl"
#include "Includes/lil_common_frag.hlsl"

float4 frag(v2f input LIL_VFACE(facing)) : SV_Target
{
    //------------------------------------------------------------------------------------------------------------------------------
    // Initialize
    float3 lightDirection = float3(0.0, 1.0, 0.0);
    float3 lightColor = 1.0;
    float3 addLightColor = 0.0;
    float attenuation = 1.0;

    float4 col = 1.0;
    float3 albedo = 1.0;
    float3 emissionColor = 0.0;

    float3 normalDirection = 0.0;
    float3 viewDirection = 0.0;
    float3 headDirection = 0.0;
    float3x3 tbnWS = 0.0;
    float depth = 0.0;
    float3 parallaxViewDirection = 0.0;
    float2 parallaxOffset = 0.0;

    float vl = 0.0;
    float hl = 0.0;
    float ln = 0.0;
    float nv = 0.0;
    float nvabs = 0.0;

    bool isRightHand = true;
    float shadowmix = 1.0;
    float audioLinkValue = 1.0;
    float3 invLighting = 0.0;

    LIL_VFACE_FALLBACK(facing);
    LIL_SETUP_INSTANCE_ID(input);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    LIL_GET_HDRPDATA(input);
    #if defined(LIL_V2F_LIGHTDIRECTION)
        lightDirection = input.lightDirection;
    #endif
    LIL_GET_MAINLIGHT(input, lightColor, lightDirection, attenuation);
    LIL_GET_ADDITIONALLIGHT(input, addLightColor);
    invLighting = saturate((1.0 - lightColor) * sqrt(lightColor));

    //------------------------------------------------------------------------------------------------------------------------------
    // View Direction
    #if defined(LIL_V2F_POSITION_WS)
        depth = length(lilViewDirection(input.positionWS));
        viewDirection = normalize(lilViewDirection(input.positionWS));
        headDirection = normalize(lilHeadDirection(input.positionWS));
        vl = dot(viewDirection, lightDirection);
        hl = dot(headDirection, lightDirection);
    #endif
    #if defined(LIL_V2F_NORMAL_WS) && defined(LIL_V2F_TANGENT_WS) && defined(LIL_V2F_BITANGENT_WS)
        tbnWS = float3x3(input.tangentWS.xyz, input.bitangentWS, input.normalWS);
        #if defined(LIL_V2F_POSITION_WS)
            parallaxViewDirection = mul(tbnWS, viewDirection);
            parallaxOffset = (parallaxViewDirection.xy / (parallaxViewDirection.z+0.5));
        #endif
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Apply Matelial & Lighting
    #if defined(LIL_OUTLINE)
        //------------------------------------------------------------------------------------------------------------------------------
        // UV
        BEFORE_ANIMATE_OUTLINE_UV
        OVERRIDE_ANIMATE_OUTLINE_UV

        //------------------------------------------------------------------------------------------------------------------------------
        // Main Color
        BEFORE_OUTLINE_COLOR
        OVERRIDE_OUTLINE_COLOR

        //------------------------------------------------------------------------------------------------------------------------------
        // Alpha
        #if LIL_RENDER == 0
            // Opaque
        #elif LIL_RENDER == 1
            // Cutout
            clip(col.a - _Cutoff);
        #elif LIL_RENDER == 2
            // Transparent
            clip(col.a - _Cutoff);
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Copy
        albedo = col.rgb;

        //------------------------------------------------------------------------------------------------------------------------------
        // Lighting
        col.rgb = lerp(col.rgb, col.rgb * min(lightColor + addLightColor, _LightMaxLimit), _OutlineEnableLighting);
    #else
        //------------------------------------------------------------------------------------------------------------------------------
        // UV
        BEFORE_ANIMATE_MAIN_UV
        OVERRIDE_ANIMATE_MAIN_UV

        //------------------------------------------------------------------------------------------------------------------------------
        // Main Color
        BEFORE_MAIN
        OVERRIDE_MAIN
        float4 triMask = 1.0;
        triMask = LIL_SAMPLE_2D(_TriMask, sampler_MainTex, uvMain);

        //------------------------------------------------------------------------------------------------------------------------------
        // Alpha
        #if LIL_RENDER == 0
            // Opaque
        #elif LIL_RENDER == 1
            // Cutout
            clip(col.a - _Cutoff);
        #elif LIL_RENDER == 2
            // Transparent
            clip(col.a - _Cutoff);
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Normal
        normalDirection = normalize(input.normalWS);
        normalDirection = facing < (_FlipNormal-1.0) ? -normalDirection : normalDirection;
        ln = dot(lightDirection, normalDirection);

        //------------------------------------------------------------------------------------------------------------------------------
        // MatCap
        BEFORE_MATCAP
        OVERRIDE_MATCAP

        //------------------------------------------------------------------------------------------------------------------------------
        // Copy
        albedo = col.rgb;

        //------------------------------------------------------------------------------------------------------------------------------
        // Lighting
        BEFORE_SHADOW
        #ifndef LIL_PASS_FORWARDADD
            OVERRIDE_SHADOW

            lightColor += addLightColor;
            shadowmix += lilLuminance(addLightColor);
            col.rgb += albedo * addLightColor;

            lightColor = min(lightColor, _LightMaxLimit);
            shadowmix = saturate(shadowmix);
            col.rgb = min(col.rgb, albedo * _LightMaxLimit);
        #else
            col.rgb *= lightColor;
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Rim light
        nvabs = abs(dot(normalDirection, viewDirection));
        BEFORE_RIMLIGHT
        OVERRIDE_RIMLIGHT

        #ifndef LIL_PASS_FORWARDADD
            BEFORE_EMISSION_1ST
            OVERRIDE_EMISSION_1ST

            BEFORE_BLEND_EMISSION
            OVERRIDE_BLEND_EMISSION
        #endif
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Fog
    BEFORE_FOG
    OVERRIDE_FOG

    BEFORE_OUTPUT
    OVERRIDE_OUTPUT
}

#endif