// lil's Toon Shader
// Copyright (c) 2020 lilxyzw
// This code is under MIT licence, see LICENSE
// https://github.com/lilxyzw/lil-s-Toon-Shader/blob/main/LICENSE

#ifndef LIL_VERTEX_FUR_INCLUDED
#define LIL_VERTEX_FUR_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Vertex shader
v2g vert(appdata input)
{
    v2g output;
    LIL_INITIALIZE_STRUCT(v2g, output);

    //----------------------------------------------------------------------------------------------------------------------
    // Invisible
    if(_Invisible) return output;

    //----------------------------------------------------------------------------------------------------------------------
    // Single Pass Instanced rendering
    LIL_SETUP_INSTANCE_ID(input);
    LIL_TRANSFER_INSTANCE_ID(input, output);
    LIL_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    //----------------------------------------------------------------------------------------------------------------------
    // Copy
    LIL_VERTEX_POSITION_INPUTS(input.positionOS, vertexInput);
    LIL_VERTEX_NORMAL_TANGENT_INPUTS(input.normalOS, input.tangentOS, vertexNormalInput);

    output.uv           = input.uv;
    output.color        = input.color;
    output.positionWS   = vertexInput.positionWS;
    output.normalWS     = vertexNormalInput.normalWS;
    output.tangentWS    = vertexNormalInput.tangentWS;
    output.bitangentWS  = vertexNormalInput.bitangentWS;

    //----------------------------------------------------------------------------------------------------------------------
    // Fog & Lightmap & Vertex light
    LIL_TRANSFER_FOG(vertexInput, output);
    LIL_TRANSFER_LIGHTMAPUV(input.uv1, output);
    LIL_CALC_VERTEXLIGHT(vertexInput, output);

    return output;
}

//------------------------------------------------------------------------------------------------------------------------------
// Geometry shader
[maxvertexcount(48)]
void geom(triangle v2g input[3], inout TriangleStream<g2f> outStream)
{
    if(!_Invisible)
    {
        g2f output;
        LIL_INITIALIZE_STRUCT(g2f, output);
        LIL_TRANSFER_INSTANCE_ID(input[0], output);
        LIL_TRANSFER_VERTEX_OUTPUT_STEREO(input[0], output);
        LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input[0]);

        //----------------------------------------------------------------------------------------------------------------------
        // Vector
        float3 fur_vector[3];
        [unroll]
        for(int fvi=0;fvi<3;fvi++)
        {
            float3x3 tbnWS = float3x3(input[fvi].tangentWS, input[fvi].bitangentWS, input[fvi].normalWS);
            fur_vector[fvi] = _FurVector.xyz;
            if(_VertexColor2FurVector) fur_vector[fvi] = lilBlendNormal(fur_vector[fvi], input[fvi].color.xyz);
            fur_vector[fvi] = lilBlendNormal(fur_vector[fvi], UnpackNormalScale(LIL_SAMPLE_2D_LOD(_FurVectorTex, sampler_MainTex, input[fvi].uv * _MainTex_ST.xy + _MainTex_ST.zw, 0), _FurVectorScale));
            fur_vector[fvi] = normalize(mul(fur_vector[fvi], tbnWS));
            fur_vector[fvi].y -= _FurGravity;
        }

        //----------------------------------------------------------------------------------------------------------------------
        // Mid
        float3 fvc = (fur_vector[0]         +fur_vector[1]          +fur_vector[2])         *0.333333333333;
        float3 wpc = (input[0].positionWS   +input[1].positionWS    +input[2].positionWS)   *0.333333333333;
        float3 ndc = (input[0].normalWS     +input[1].normalWS      +input[2].normalWS)     *0.333333333333;
        float2 uvc = (input[0].uv           +input[1].uv            +input[2].uv)           *0.333333333333;
        #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
            float fcc = (input[0].fogCoord      +input[1].fogCoord      +input[2].fogCoord)     *0.333333333333;
        #endif
        #if defined(LIL_USE_LIGHTMAP) || defined(LIL_USE_DYNAMICLIGHTMAP)
            float2 ulc = (input[0].uvLM         +input[1].uvLM          +input[2].uvLM)         *0.333333333333;
        #endif
        #if defined(LIL_USE_VERTEXLIGHT)
            float3 vlc = (input[0].vl           +input[1].vl            +input[2].vl)           *0.333333333333;
        #endif

        //--------------------------------------------------------------------------------------------------------------
        // Fin
        for (uint fl = 0; fl < _FurLayerNum; fl++)
        {
            float lpmix = fl/(float)_FurLayerNum;
            for(int ii=0;ii<4;ii++)
            {
                int ii2 = ii==3 ? 0 : ii;

                float3 fvmix = lerp(fur_vector[ii2],fvc,lpmix);
                output.uv = lerp(input[ii2].uv,uvc,lpmix);
                #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
                    output.fogCoord = lerp(input[ii2].fogCoord,fcc,lpmix);
                #endif
                #if defined(LIL_USE_LIGHTMAP) || defined(LIL_USE_DYNAMICLIGHTMAP)
                    output.uvLM = lerp(input[ii2].uvLM,ulc,lpmix);
                #endif
                #if defined(LIL_USE_VERTEXLIGHT)
                    output.vl = lerp(input[ii2].vl,vlc,lpmix);
                #endif

                #if !defined(LIL_BRP)
                    output.positionWS = lerp(input[ii2].positionWS,wpc,lpmix);
                    output.positionCS = LIL_TRANSFORM_POS_WS_TO_CS(output.positionWS);
                    output.normalWS = lerp(input[ii2].normalWS,ndc,lpmix);
                    output.furLayer = 0;
                    outStream.Append(output);

                    output.positionWS.xyz += fvmix * _FurVector.w;
                    output.positionCS = LIL_TRANSFORM_POS_WS_TO_CS(output.positionWS);
                    output.furLayer = 1;
                    outStream.Append(output);
                #elif !defined(LIL_PASS_FORWARDADD)
                    float3 positionWS = lerp(input[ii2].positionWS,wpc,lpmix);
                    output.positionCS = LIL_TRANSFORM_POS_WS_TO_CS(positionWS);
                    output.normalWS = lerp(input[ii2].normalWS,ndc,lpmix);
                    output.furLayer = 0;
                    outStream.Append(output);

                    positionWS.xyz += fvmix * _FurVector.w;
                    output.positionCS = LIL_TRANSFORM_POS_WS_TO_CS(positionWS);
                    output.furLayer = 1;
                    outStream.Append(output);
                #else
                    output.positionWS = lerp(input[ii2].positionWS,wpc,lpmix);
                    output.positionCS = LIL_TRANSFORM_POS_WS_TO_CS(output.positionWS);
                    output.furLayer = 0;
                    outStream.Append(output);

                    output.positionWS.xyz += fvmix * _FurVector.w;
                    output.positionCS = LIL_TRANSFORM_POS_WS_TO_CS(output.positionWS);
                    output.furLayer = 1;
                    outStream.Append(output);
                #endif
            }
            outStream.RestartStrip();
        }
    }
}

#endif