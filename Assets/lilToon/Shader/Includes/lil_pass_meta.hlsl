#ifndef LIL_PASS_META_INCLUDED
#define LIL_PASS_META_INCLUDED

#define LIL_WITHOUT_ANIMATION
#include "Includes/lil_pipeline.hlsl"
#include "Includes/lil_common_appdata.hlsl"

#if defined(LIL_HDRP)
    CBUFFER_START(UnityMetaPass)
        bool4 unity_MetaVertexControl;
        bool4 unity_MetaFragmentControl;
        int unity_VisualizationMode;
    CBUFFER_END

    float unity_OneOverOutputBoost;
    float unity_MaxOutputValue;
    float unity_UseLinearSpace;
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Structure
#if !defined(LIL_CUSTOM_V2F_MEMBER)
    #define LIL_CUSTOM_V2F_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7)
#endif

#define LIL_V2F_POSITION_CS
#define LIL_V2F_TEXCOORD0
#define LIL_V2F_VIZUV
#define LIL_V2F_LIGHTCOORD

struct v2f
{
    float4 positionCS   : SV_POSITION;
    float2 uv           : TEXCOORD0;
    #if defined(EDITOR_VISUALIZATION) && !defined(LIL_HDRP)
        float2 vizUV        : TEXCOORD1;
        float4 lightCoord   : TEXCOORD2;
    #endif
    LIL_CUSTOM_V2F_MEMBER(3,4,5,6,7,8,9,10)
    LIL_VERTEX_INPUT_INSTANCE_ID
    LIL_VERTEX_OUTPUT_STEREO
};

//------------------------------------------------------------------------------------------------------------------------------
// Shader
#include "Includes/lil_common_vert.hlsl"
#include "Includes/lil_common_frag.hlsl"

float4 frag(v2f input) : SV_Target
{
    float facing = 1.0;
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

    BEFORE_ANIMATE_MAIN_UV
    OVERRIDE_ANIMATE_MAIN_UV

    BEFORE_MAIN
    OVERRIDE_MAIN
    albedo = col.rgb;
    col.rgb = 0.0;
    #if defined(LIL_LITE)
        float4 triMask = 1.0;
        triMask = LIL_SAMPLE_2D(_TriMask, sampler_MainTex, uvMain);
    #endif

    #ifndef LIL_FUR
        BEFORE_EMISSION_1ST
        #if defined(LIL_FEATURE_EMISSION_1ST) || defined(LIL_LITE)
            OVERRIDE_EMISSION_1ST
        #endif
        #if !defined(LIL_LITE)
            BEFORE_EMISSION_1ST
            #if defined(LIL_FEATURE_EMISSION_2ND)
                OVERRIDE_EMISSION_2ND
            #endif
        #endif
        BEFORE_BLEND_EMISSION
        OVERRIDE_BLEND_EMISSION
    #endif

    #if defined(LIL_HDRP)
        if(!unity_MetaFragmentControl.y) col.rgb = clamp(pow(abs(albedo), saturate(unity_OneOverOutputBoost)), 0, unity_MaxOutputValue);
        return col;
    #else
        MetaInput metaInput;
        LIL_INITIALIZE_STRUCT(MetaInput, metaInput);
        metaInput.Albedo = albedo;
        metaInput.Emission = col.rgb;
        #ifdef EDITOR_VISUALIZATION
            metaInput.VizUV = input.vizUV;
            metaInput.LightCoord = input.lightCoord;
        #endif

        return MetaFragment(metaInput);
    #endif
}

#endif