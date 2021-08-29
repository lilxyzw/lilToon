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
    LIL_BRANCH
    if(_Invisible) return output;

    //----------------------------------------------------------------------------------------------------------------------
    // Single Pass Instanced rendering
    LIL_SETUP_INSTANCE_ID(input);
    LIL_TRANSFER_INSTANCE_ID(input, output);
    LIL_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    //----------------------------------------------------------------------------------------------------------------------
    // Encryption
    #if defined(LIL_FEATURE_ENCRYPTION)
        input.positionOS = vertexDecode(input.positionOS, input.normalOS, input.uv6, input.uv7);
    #endif

    //----------------------------------------------------------------------------------------------------------------------
    // Copy
    LIL_VERTEX_POSITION_INPUTS(input.positionOS, vertexInput);
    LIL_VERTEX_NORMAL_INPUTS(input.normalOS, vertexNormalInput);
    output.uv           = input.uv;
    output.positionWS   = vertexInput.positionWS;
    #if !defined(LIL_PASS_FORWARDADD) && defined(LIL_SHOULD_NORMAL)
        output.normalWS     = vertexNormalInput.normalWS;
    #endif

    //----------------------------------------------------------------------------------------------------------------------
    // Vector
    float3 bitangentOS = cross(input.normalOS, input.tangentOS.xyz) * (input.tangentOS.w * LIL_NEGATIVE_SCALE);
    float3x3 tbnOS = float3x3(input.tangentOS.xyz, bitangentOS, input.normalOS);
    output.furVector = _FurVector.xyz;
    if(_VertexColor2FurVector) output.furVector = lilBlendNormal(output.furVector, input.color.xyz);
    if(Exists_FurVectorTex) output.furVector = lilBlendNormal(output.furVector, UnpackNormalScale(LIL_SAMPLE_2D_LOD(_FurVectorTex, sampler_MainTex, input.uv * _MainTex_ST.xy + _MainTex_ST.zw, 0), _FurVectorScale));
    output.furVector = mul(normalize(output.furVector), tbnOS) * _FurVector.w;
    output.furVector = mul((float3x3)LIL_MATRIX_M, output.furVector);
    output.furVector.y -= _FurGravity * length(output.furVector);

    //----------------------------------------------------------------------------------------------------------------------
    // Fog & Lightmap & Vertex light
    LIL_CALC_MAINLIGHT(vertexInput, output);
    LIL_TRANSFER_FOG(vertexInput, output);
    LIL_TRANSFER_LIGHTMAPUV(input.uv1, output);
    LIL_CALC_VERTEXLIGHT(vertexInput, output);

    return output;
}

//------------------------------------------------------------------------------------------------------------------------------
// Geometry shader
#if (defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)) && (defined(LIL_USE_LIGHTMAP) || defined(LIL_USE_DYNAMICLIGHTMAP) || defined(LIL_LIGHTMODE_SHADOWMASK))
    [maxvertexcount(32)]
#else
    [maxvertexcount(40)]
#endif
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
        // Mid
        float3 fvc = (input[0].furVector    +input[1].furVector     +input[2].furVector)    *0.333333333333;
        float3 wpc = (input[0].positionWS   +input[1].positionWS    +input[2].positionWS)   *0.333333333333;
        float2 uvc = (input[0].uv           +input[1].uv            +input[2].uv)           *0.333333333333;
        #if !defined(LIL_PASS_FORWARDADD) && defined(LIL_SHOULD_NORMAL)
            float3 ndc = (input[0].normalWS     +input[1].normalWS      +input[2].normalWS)     *0.333333333333;
        #endif
        #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
            float fcc = (input[0].fogCoord      +input[1].fogCoord      +input[2].fogCoord)     *0.333333333333;
        #endif
        #if defined(LIL_USE_LIGHTMAP) || defined(LIL_USE_DYNAMICLIGHTMAP)
            float2 ulc = (input[0].uvLM         +input[1].uvLM          +input[2].uvLM)         *0.333333333333;
        #endif
        #if defined(LIL_USE_VERTEXLIGHT)
            float3 vlc = (input[0].vl           +input[1].vl            +input[2].vl)           *0.333333333333;
        #endif

        // Main Light
        #if !defined(LIL_PASS_FORWARDADD)
            output.lightColor = input[0].lightColor;
            output.lightDirection = input[0].lightDirection;
        #endif
        #if defined(LIL_FEATURE_SHADOW) && !defined(LIL_PASS_FORWARDADD)
            output.indLightColor = input[0].indLightColor;
        #endif

        //--------------------------------------------------------------------------------------------------------------
        // FakeFur (whiteflare)
        // https://github.com/whiteflare/Unlit_WF_ShaderSuite
        for (uint fl = 0; fl < _FurLayerNum; fl++)
        {
            float lpmix = fl/(float)_FurLayerNum;
            for(int ii=0;ii<4;ii++)
            {
                int ii2 = ii==3 ? 0 : ii;

                float3 fvmix = lerp(input[ii2].furVector,fvc,lpmix);
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

                if(Exists_FurLengthMask) fvmix *= LIL_SAMPLE_2D_LOD(_FurLengthMask, sampler_MainTex, output.uv * _MainTex_ST.xy + _MainTex_ST.zw, 0).r;

                #if (!defined(LIL_PASS_FORWARDADD) && defined(LIL_FEATURE_DISTANCE_FADE)) || !defined(LIL_BRP)
                    output.positionWS = lerp(input[ii2].positionWS,wpc,lpmix);
                    output.positionCS = LIL_TRANSFORM_POS_WS_TO_CS(output.positionWS);
                    #if defined(LIL_SHOULD_NORMAL)
                        output.normalWS = lerp(input[ii2].normalWS,ndc,lpmix);
                    #endif
                    output.furLayer = 0;
                    outStream.Append(output);

                    output.positionWS.xyz += fvmix;
                    output.positionCS = LIL_TRANSFORM_POS_WS_TO_CS(output.positionWS);
                    output.furLayer = 1;
                    outStream.Append(output);
                #elif !defined(LIL_PASS_FORWARDADD)
                    float3 positionWS = lerp(input[ii2].positionWS,wpc,lpmix);
                    output.positionCS = LIL_TRANSFORM_POS_WS_TO_CS(positionWS);
                    #if defined(LIL_SHOULD_NORMAL)
                        output.normalWS = lerp(input[ii2].normalWS,ndc,lpmix);
                    #endif
                    output.furLayer = 0;
                    outStream.Append(output);

                    positionWS.xyz += fvmix;
                    output.positionCS = LIL_TRANSFORM_POS_WS_TO_CS(positionWS);
                    output.furLayer = 1;
                    outStream.Append(output);
                #else
                    output.positionWS = lerp(input[ii2].positionWS,wpc,lpmix);
                    output.positionCS = LIL_TRANSFORM_POS_WS_TO_CS(output.positionWS);
                    output.furLayer = 0;
                    outStream.Append(output);

                    output.positionWS.xyz += fvmix;
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