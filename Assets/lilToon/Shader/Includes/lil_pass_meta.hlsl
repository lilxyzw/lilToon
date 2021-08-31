#ifndef LIL_PASS_META_INCLUDED
#define LIL_PASS_META_INCLUDED

#define LIL_WITHOUT_ANIMATION
#include "Includes/lil_pipeline.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// Struct
struct appdata
{
    float4 positionOS   : POSITION;
    float2 uv           : TEXCOORD0;
    float2 uv1          : TEXCOORD1;
    float2 uv2          : TEXCOORD2;
    #if !defined(LIL_LITE) && defined(LIL_FEATURE_ENCRYPTION)
        float2 uv6          : TEXCOORD6;
        float2 uv7          : TEXCOORD7;
    #endif
};

struct v2f
{
    float4 positionCS   : SV_POSITION;
    float2 uv           : TEXCOORD0;
    #ifdef EDITOR_VISUALIZATION
        float2 vizUV        : TEXCOORD1;
        float4 lightCoord   : TEXCOORD2;
    #endif
};

//------------------------------------------------------------------------------------------------------------------------------
// Shader
v2f vert (appdata input)
{
    v2f output;
    LIL_INITIALIZE_STRUCT(v2f, output);

    LIL_BRANCH
    if(_Invisible) return output;

    //----------------------------------------------------------------------------------------------------------------------
    // Encryption
    #if !defined(LIL_LITE) && defined(LIL_FEATURE_ENCRYPTION)
        input.positionOS = vertexDecode(input.positionOS, input.normalOS, input.uv6, input.uv7);
    #endif

    LIL_TRANSFER_METAPASS(input,output);
    output.uv = input.uv;
    #ifdef EDITOR_VISUALIZATION
        if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
            output.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, input.uv, input.uv1, input.uv2, unity_EditorViz_Texture_ST);
        else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
        {
            output.vizUV = input.uv1 * unity_LightmapST.xy + unity_LightmapST.zw;
            output.lightCoord = mul(unity_EditorViz_WorldToLight, LIL_TRANSFORM_POS_OS_TO_WS(input.positionOS.xyz));
        }
    #endif

    return output;
}

float4 frag(v2f input) : SV_Target
{
    MetaInput metaInput;
    LIL_INITIALIZE_STRUCT(MetaInput, metaInput);

    #if defined(LIL_FEATURE_ANIMATE_MAIN_UV)
        float2 uvMain = lilCalcUVWithoutAnimation(input.uv, _MainTex_ST, _MainTex_ScrollRotate);
    #else
        float2 uvMain = lilCalcUV(input.uv, _MainTex_ST);
    #endif
    float4 col = _Color;
    if(Exists_MainTex) col *= LIL_SAMPLE_2D(_MainTex, sampler_MainTex, uvMain);
    metaInput.Albedo = col.rgb;

    #ifndef LIL_FUR
        #if defined(LIL_FEATURE_EMISSION_1ST)
            LIL_BRANCH
            if(_UseEmission)
            {
                float4 emissionColor = _EmissionColor;
                #if defined(LIL_FEATURE_EMISSION_UV) && defined(LIL_FEATURE_ANIMATE_EMISSION_UV)
                    if(Exists_EmissionMap) emissionColor *= LIL_GET_EMITEX(_EmissionMap,input.uv);
                #elif defined(LIL_FEATURE_EMISSION_UV)
                    if(Exists_EmissionMap) emissionColor *= LIL_SAMPLE_2D(_EmissionMap, sampler_EmissionMap, lilCalcUV(input.uv, _EmissionMap_ST));
                #else
                    if(Exists_EmissionMap) emissionColor *= LIL_SAMPLE_2D(_EmissionMap, sampler_EmissionMap, uvMain);
                #endif
                #ifdef LIL_LITE
                    metaInput.Emission = emissionColor.a * emissionColor.rgb;
                #else
                    #if defined(LIL_FEATURE_EMISSION_MASK_UV) && defined(LIL_FEATURE_ANIMATE_EMISSION_MASK_UV)
                        if(Exists_EmissionBlendMask) emissionColor *= LIL_GET_EMIMASK(_EmissionBlendMask,input.uv);
                    #elif defined(LIL_FEATURE_EMISSION_MASK_UV)
                        if(Exists_EmissionBlendMask) emissionColor *= LIL_SAMPLE_2D(_EmissionBlendMask, sampler_MainTex, lilCalcUV(input.uv, _EmissionBlendMask_ST));
                    #else
                        if(Exists_EmissionBlendMask) emissionColor *= LIL_SAMPLE_2D(_EmissionBlendMask, sampler_MainTex, uvMain);
                    #endif
                    metaInput.Emission = _EmissionBlend * emissionColor.a * emissionColor.rgb;
                #endif
            }
        #endif
        #if !defined(LIL_LITE)
            #if defined(LIL_FEATURE_EMISSION_2ND)
                LIL_BRANCH
                if(_UseEmission2nd)
                {
                    float4 emission2ndColor = _Emission2ndColor;
                    #if defined(LIL_FEATURE_EMISSION_UV) && defined(LIL_FEATURE_ANIMATE_EMISSION_UV)
                        if(Exists_Emission2ndMap) emission2ndColor *= LIL_GET_EMITEX(_Emission2ndMap,input.uv);
                    #elif defined(LIL_FEATURE_EMISSION_UV)
                        if(Exists_Emission2ndMap) emission2ndColor *= LIL_SAMPLE_2D(_Emission2ndMap, sampler_Emission2ndMap, lilCalcUV(input.uv, _Emission2ndMap_ST));
                    #else
                        if(Exists_Emission2ndMap) emission2ndColor *= LIL_SAMPLE_2D(_Emission2ndMap, sampler_Emission2ndMap, uvMain);
                    #endif
                    #if defined(LIL_FEATURE_EMISSION_MASK_UV) && defined(LIL_FEATURE_ANIMATE_EMISSION_MASK_UV)
                        if(Exists_Emission2ndBlendMask) emission2ndColor *= LIL_GET_EMIMASK(_Emission2ndBlendMask,input.uv);
                    #elif defined(LIL_FEATURE_EMISSION_MASK_UV)
                        if(Exists_Emission2ndBlendMask) emission2ndColor *= LIL_SAMPLE_2D(_Emission2ndBlendMask, sampler_MainTex, lilCalcUV(input.uv, _Emission2ndBlendMask_ST));
                    #else
                        if(Exists_Emission2ndBlendMask) emission2ndColor *= LIL_SAMPLE_2D(_Emission2ndBlendMask, sampler_MainTex, uvMain);
                    #endif
                    metaInput.Emission += _Emission2ndBlend * emission2ndColor.a * emission2ndColor.rgb;
                }
            #endif
        #endif
    #endif

    #ifdef EDITOR_VISUALIZATION
        metaInput.VizUV = input.vizUV;
        metaInput.LightCoord = input.lightCoord;
    #endif

    return MetaFragment(metaInput);
}

#endif