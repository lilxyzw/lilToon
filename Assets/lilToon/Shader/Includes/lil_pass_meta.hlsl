#ifndef LIL_PASS_META_INCLUDED
#define LIL_PASS_META_INCLUDED

#define LIL_WITHOUT_ANIMATION
#include "lil_common.hlsl"
#include "lil_common_appdata.hlsl"

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
#define LIL_V2F_VIZUV
#define LIL_V2F_LIGHTCOORD
#define LIL_V2F_PACKED_TEXCOORD01
#define LIL_V2F_PACKED_TEXCOORD23

struct v2f
{
    float4 positionCS   : SV_POSITION;
    float4 uv01         : TEXCOORD0;
    float4 uv23         : TEXCOORD1;
    #if defined(EDITOR_VISUALIZATION) && !defined(LIL_HDRP)
        float2 vizUV        : TEXCOORD2;
        float4 lightCoord   : TEXCOORD3;
    #endif
    LIL_CUSTOM_V2F_MEMBER(4,5,6,7,8,9,10,11)
    LIL_VERTEX_INPUT_INSTANCE_ID
    LIL_VERTEX_OUTPUT_STEREO
};

//------------------------------------------------------------------------------------------------------------------------------
// Shader
#include "lil_common_vert.hlsl"
#include "lil_common_frag.hlsl"

float4 frag(v2f input) : SV_Target
{
    float facing = 1.0;
    //------------------------------------------------------------------------------------------------------------------------------
    // Initialize
    LIL_SETUP_INSTANCE_ID(input);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    lilFragData fd = lilInitFragData();

    BEFORE_UNPACK_V2F
    OVERRIDE_UNPACK_V2F

    BEFORE_ANIMATE_MAIN_UV
    OVERRIDE_ANIMATE_MAIN_UV

    BEFORE_MAIN
    OVERRIDE_MAIN
    fd.albedo = fd.col.rgb;
    fd.col.rgb = 0.0;
    #if defined(LIL_LITE)
        fd.triMask = LIL_SAMPLE_2D(_TriMask, sampler_MainTex, fd.uvMain);
    #endif

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

    #if defined(LIL_HDRP)
        if(!unity_MetaFragmentControl.y) fd.col.rgb = clamp(pow(abs(fd.albedo), saturate(unity_OneOverOutputBoost)), 0, unity_MaxOutputValue);
        return fd.col;
    #else
        MetaInput metaInput;
        LIL_INITIALIZE_STRUCT(MetaInput, metaInput);
        metaInput.Albedo = abs(fd.albedo);
        metaInput.Emission = fd.col.rgb;
        #ifdef EDITOR_VISUALIZATION
            metaInput.VizUV = input.vizUV;
            metaInput.LightCoord = input.lightCoord;
        #endif

        return MetaFragment(metaInput);
    #endif
}

#endif