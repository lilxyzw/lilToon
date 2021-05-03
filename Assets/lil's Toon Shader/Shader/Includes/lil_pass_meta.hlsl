// lil's Toon Shader
// Copyright (c) 2020 lilxyzw
// This code is under MIT licence, see LICENSE
// https://github.com/lilxyzw/lil-s-Toon-Shader/blob/main/LICENSE

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

    if(_Invisible) return output;

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

    float2 uvMain = lilCalcUVWithoutAnimation(input.uv, _MainTex_ST, _MainTex_ScrollRotate);
    float4 col = LIL_SAMPLE_2D(_MainTex, sampler_MainTex, uvMain) * _Color;
    metaInput.Albedo = col.rgb;

    #ifndef LIL_FUR
        LIL_BRANCH
        if(_UseEmission)
        {
            _EmissionColor *= LIL_GET_EMITEX(_EmissionMap,input.uv);
            #ifdef LIL_LITE
                metaInput.Emission = _EmissionColor.a * _EmissionColor.rgb;
            #else
                metaInput.Emission = LIL_GET_EMIMASK(_EmissionBlendMask,input.uv) * _EmissionBlend * _EmissionColor.a * _EmissionColor.rgb;
            #endif
        }
        #if !defined(LIL_LITE)
            LIL_BRANCH
            if(_UseEmission2nd)
            {
                _Emission2ndColor *= LIL_GET_EMITEX(_Emission2ndMap,input.uv);
                metaInput.Emission += LIL_GET_EMIMASK(_Emission2ndBlendMask,input.uv) * _Emission2ndBlend * _Emission2ndColor.a * _Emission2ndColor.rgb;
            }
        #endif
    #endif

    #ifdef EDITOR_VISUALIZATION
        metaInput.VizUV = input.vizUV;
        metaInput.LightCoord = input.lightCoord;
    #endif

    return MetaFragment(metaInput);
}

#endif