#ifndef LIL_VERTEX_FUR_INCLUDED
#define LIL_VERTEX_FUR_INCLUDED

#if !defined(LIL_CUSTOM_VERT_COPY)
    #define LIL_CUSTOM_VERT_COPY
#endif

void lilCustomVertexOS(inout appdata input, inout float2 uvMain, inout float4 positionOS)
{
    #if defined(LIL_CUSTOM_VERTEX_OS)
        LIL_CUSTOM_VERTEX_OS
    #endif
}

void lilCustomVertexOS(inout appdata input, inout float2 uvMain, inout float3 positionOS)
{
    float4 positionOS4 = float4(positionOS, 1.0);
    lilCustomVertexOS(input, uvMain, positionOS4);
    positionOS = positionOS4.xyz;
}

void lilCustomVertexWS(inout appdata input, inout float2 uvMain, inout lilVertexPositionInputs vertexInput, inout lilVertexNormalInputs vertexNormalInput)
{
    #if defined(LIL_CUSTOM_VERTEX_WS)
        LIL_CUSTOM_VERTEX_WS
    #endif
}

//------------------------------------------------------------------------------------------------------------------------------
// Vertex shader
v2g vert(appdata input)
{
    v2g output;
    LIL_INITIALIZE_STRUCT(v2g, output);

    //------------------------------------------------------------------------------------------------------------------------------
    // Invisible
    LIL_BRANCH
    if(_Invisible) return output;

    //------------------------------------------------------------------------------------------------------------------------------
    // Single Pass Instanced rendering
    LIL_SETUP_INSTANCE_ID(input);
    LIL_TRANSFER_INSTANCE_ID(input, output);
    LIL_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    //------------------------------------------------------------------------------------------------------------------------------
    // UV
    float2 uvMain = lilCalcUV(input.uv, _MainTex_ST);

    //------------------------------------------------------------------------------------------------------------------------------
    // Vertex Modification
    #include "Includes/lil_vert_encryption.hlsl"
    lilCustomVertexOS(input, uvMain, input.positionOS);

    //------------------------------------------------------------------------------------------------------------------------------
    // Previous Position
    #if defined(LIL_PASS_MOTIONVECTOR_INCLUDED)
        input.previousPositionOS = unity_MotionVectorsParams.x > 0.0 ? input.previousPositionOS : input.positionOS.xyz;
        #if defined(_ADD_PRECOMPUTED_VELOCITY)
            input.previousPositionOS -= input.precomputedVelocity;
        #endif
        #define LIL_MODIFY_PREVPOS
        #include "Includes/lil_vert_encryption.hlsl"
        lilCustomVertexOS(input, uvMain, input.previousPositionOS);
        #undef LIL_MODIFY_PREVPOS

        //------------------------------------------------------------------------------------------------------------------------------
        // Transform
        LIL_VERTEX_POSITION_INPUTS(input.previousPositionOS, previousVertexInput);
        #if defined(LIL_APP_NORMAL) && defined(LIL_APP_TANGENT)
            LIL_VERTEX_NORMAL_TANGENT_INPUTS(input.normalOS, input.tangentOS, previousVertexNormalInput);
        #elif defined(LIL_APP_NORMAL)
            LIL_VERTEX_NORMAL_INPUTS(input.normalOS, previousVertexNormalInput);
        #else
            lilVertexNormalInputs previousVertexNormalInput = lilGetVertexNormalInputs();
        #endif
        previousVertexInput.positionWS = TransformPreviousObjectToWorld(input.previousPositionOS);
        lilCustomVertexWS(input, uvMain, previousVertexInput, previousVertexNormalInput);
        output.previousPositionWS = previousVertexInput.positionWS;
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Transform
    LIL_VERTEX_POSITION_INPUTS(input.positionOS, vertexInput);
    LIL_VERTEX_NORMAL_INPUTS(input.normalOS, vertexNormalInput);
    lilCustomVertexWS(input, uvMain, vertexInput, vertexNormalInput);
    #if defined(LIL_CUSTOM_VERTEX_WS)
        LIL_RE_VERTEX_POSITION_INPUTS(vertexInput);
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Copy
    #if defined(LIL_V2G_POSITION_WS)
        output.positionWS       = vertexInput.positionWS;
    #endif
    #if defined(LIL_V2G_TEXCOORD0)
        output.uv               = input.uv;
    #endif
    #if defined(LIL_V2G_TEXCOORD1)
        output.uv1              = input.uv1;
    #endif
    #if defined(LIL_V2G_NORMAL_WS)
        output.normalWS         = vertexNormalInput.normalWS;
    #endif

    LIL_CUSTOM_VERT_COPY

    //------------------------------------------------------------------------------------------------------------------------------
    // Fog & Lighting
    LIL_GET_HDRPDATA(vertexInput);
    #if defined(LIL_V2F_LIGHTCOLOR) || defined(LIL_V2F_LIGHTDIRECTION) || defined(LIL_V2F_INDLIGHTCOLOR)
        LIL_CALC_MAINLIGHT(vertexInput, lightdataInput);
    #endif
    #if defined(LIL_V2F_LIGHTCOLOR)
        output.lightColor       = lightdataInput.lightColor;
    #endif
    #if defined(LIL_V2F_LIGHTDIRECTION)
        output.lightDirection   = lightdataInput.lightDirection;
    #endif
    #if defined(LIL_V2F_INDLIGHTCOLOR)
        output.indLightColor    = lightdataInput.indLightColor;
    #endif
    #if defined(LIL_V2G_SHADOW)
        LIL_TRANSFER_SHADOW(vertexInput, input.uv1, output);
    #endif
    #if defined(LIL_V2G_FOG)
        LIL_TRANSFER_FOG(vertexInput, output);
    #endif
    #if defined(LIL_V2G_VERTEXLIGHT)
        LIL_CALC_VERTEXLIGHT(vertexInput, output);
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Vector
    #if defined(LIL_V2G_FURVECTOR)
        float3 bitangentOS = cross(input.normalOS, input.tangentOS.xyz) * (input.tangentOS.w * LIL_NEGATIVE_SCALE);
        float3x3 tbnOS = float3x3(input.tangentOS.xyz, bitangentOS, input.normalOS);
        output.furVector = _FurVector.xyz;
        if(_VertexColor2FurVector) output.furVector = lilBlendNormal(output.furVector, input.color.xyz);
        if(Exists_FurVectorTex) output.furVector = lilBlendNormal(output.furVector, UnpackNormalScale(LIL_SAMPLE_2D_LOD(_FurVectorTex, sampler_linear_repeat, uvMain, 0), _FurVectorScale));
        output.furVector = mul(normalize(output.furVector), tbnOS) * _FurVector.w;
        output.furVector = lilTransformDirOStoWS(output.furVector, false);
        output.furVector.y -= _FurGravity * length(output.furVector);
    #endif

    return output;
}

//------------------------------------------------------------------------------------------------------------------------------
// Geometry shader
#if defined(LIL_ONEPASS_FUR)
    [maxvertexcount(46)]
#elif (defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)) && (defined(LIL_USE_LIGHTMAP) || defined(LIL_USE_DYNAMICLIGHTMAP) || defined(LIL_LIGHTMODE_SHADOWMASK)) || defined(LIL_FEATURE_DISTANCE_FADE) || !defined(LIL_BRP)
    [maxvertexcount(32)]
#else
    [maxvertexcount(40)]
#endif
void geom(triangle v2g input[3], inout TriangleStream<v2f> outStream)
{
    //------------------------------------------------------------------------------------------------------------------------------
    // Invisible
    LIL_BRANCH
    if(_Invisible) return;

    //------------------------------------------------------------------------------------------------------------------------------
    // Copy
    #if defined(LIL_ONEPASS_FUR)
        v2f outputBase[3];
        LIL_INITIALIZE_STRUCT(v2f, outputBase[0]);
        LIL_INITIALIZE_STRUCT(v2f, outputBase[1]);
        LIL_INITIALIZE_STRUCT(v2f, outputBase[2]);

        for(uint i = 0; i < 3; i++)
        {
            LIL_TRANSFER_INSTANCE_ID(input[i], outputBase[i]);
            LIL_TRANSFER_VERTEX_OUTPUT_STEREO(input[i], outputBase[i]);
            #if defined(LIL_V2F_POSITION_CS)
                outputBase[i].positionCS = lilTransformWStoCS(input[i].positionWS);
            #endif
            #if defined(LIL_PASS_MOTIONVECTOR_INCLUDED)
                outputBase[i].previousPositionCS = mul(UNITY_MATRIX_PREV_VP, float4(input[i].previousPositionWS, 1.0));
            #endif
            #if defined(LIL_V2F_TEXCOORD0)
                outputBase[i].uv = input[i].uv;
            #endif
            #if defined(LIL_V2F_NORMAL_WS)
                outputBase[i].normalWS = input[i].normalWS;
            #endif
            #if defined(LIL_V2F_FURLAYER)
                outputBase[i].furLayer = -2;
            #endif
        }
        LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input[0]);

        // Front
        LIL_BRANCH
        if(_Cull != 1)
        {
            outStream.Append(outputBase[0]);
            outStream.Append(outputBase[1]);
            outStream.Append(outputBase[2]);
            outStream.RestartStrip();
        }

        // Back
        LIL_BRANCH
        if(_Cull != 2)
        {
            outStream.Append(outputBase[2]);
            outStream.Append(outputBase[1]);
            outStream.Append(outputBase[0]);
            outStream.RestartStrip();
        }
    #endif

    v2f output;
    LIL_INITIALIZE_STRUCT(v2f, output);
    LIL_TRANSFER_INSTANCE_ID(input[0], output);
    LIL_TRANSFER_VERTEX_OUTPUT_STEREO(input[0], output);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input[0]);

    //------------------------------------------------------------------------------------------------------------------------------
    // Mid
    #if defined(LIL_V2G_FURVECTOR)
        float3 fvc = (input[0].furVector    +input[1].furVector     +input[2].furVector)    *0.333333333333;
    #endif
    #if defined(LIL_V2G_POSITION_WS)
        float3 wpc = (input[0].positionWS   +input[1].positionWS    +input[2].positionWS)   *0.333333333333;
    #endif
    #if defined(LIL_V2G_TEXCOORD0)
        float2 uvc = (input[0].uv           +input[1].uv            +input[2].uv)           *0.333333333333;
    #endif
    #if defined(LIL_V2G_TEXCOORD1)
        float2 uv1c = (input[0].uv1          +input[1].uv1           +input[2].uv1)          *0.333333333333;
    #endif
    #if defined(LIL_V2G_NORMAL_WS)
        float3 ndc = (input[0].normalWS     +input[1].normalWS      +input[2].normalWS)     *0.333333333333;
    #endif
    #if defined(LIL_V2G_FOG) && (defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2))
        float fcc = (input[0].fogCoord      +input[1].fogCoord      +input[2].fogCoord)     *0.333333333333;
    #endif
    #if defined(LIL_V2G_VERTEXLIGHT) && defined(LIL_USE_VERTEXLIGHT)
        float3 vlc = (input[0].vl           +input[1].vl            +input[2].vl)           *0.333333333333;
    #endif
    #if defined(LIL_PASS_MOTIONVECTOR_INCLUDED)
        float3 pwpc = (input[0].previousPositionWS + input[1].previousPositionWS + input[2].previousPositionWS) *0.333333333333;
    #endif

    // Main Light
    #if defined(LIL_V2G_LIGHTCOLOR)
        output.lightColor = input[0].lightColor;
    #endif
    #if defined(LIL_V2G_LIGHTDIRECTION)
        output.lightDirection = input[0].lightDirection;
    #endif
    #if defined(LIL_V2G_INDLIGHTCOLOR)
        output.indLightColor = input[0].indLightColor;
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // FakeFur (whiteflare)
    // https://github.com/whiteflare/Unlit_WF_ShaderSuite
    for(uint fl = 0; fl < _FurLayerNum; fl++)
    {
        float lpmix = fl/(float)_FurLayerNum;
        for(int ii=0;ii<4;ii++)
        {
            int ii2 = ii==3 ? 0 : ii;

            // Common
            float2 outUV = lerp(input[ii2].uv,uvc,lpmix);
            #if defined(LIL_V2F_TEXCOORD0)
                output.uv = outUV;
            #endif
            #if defined(LIL_V2F_TEXCOORD1)
                output.uv1 = lerp(input[ii2].uv1,uv1c,lpmix);
            #endif
            #if defined(LIL_V2F_NORMAL_WS)
                output.normalWS = lerp(input[ii2].normalWS,ndc,lpmix);
            #endif
            #if defined(LIL_V2F_FOG) && (defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2))
                output.fogCoord = lerp(input[ii2].fogCoord,fcc,lpmix);
            #endif
            #if defined(LIL_V2F_VERTEXLIGHT) && defined(LIL_USE_VERTEXLIGHT)
                output.vl = lerp(input[ii2].vl,vlc,lpmix);
            #endif

            float3 fvmix = lerp(input[ii2].furVector,fvc,lpmix);
            if(Exists_FurLengthMask) fvmix *= LIL_SAMPLE_2D_LOD(_FurLengthMask, sampler_linear_repeat, outUV * _MainTex_ST.xy + _MainTex_ST.zw, 0).r;

            // In
            float3 positionWS = lerp(input[ii2].positionWS,wpc,lpmix);
            #if defined(LIL_V2F_POSITION_WS)
                output.positionWS = positionWS;
            #endif
            #if defined(LIL_V2F_POSITION_CS)
                output.positionCS = lilTransformWStoCS(positionWS);
            #endif
            #if defined(LIL_PASS_MOTIONVECTOR_INCLUDED)
                float3 previousPositionWS = lerp(input[ii2].previousPositionWS,pwpc,lpmix);
                output.previousPositionCS = mul(UNITY_MATRIX_PREV_VP, float4(previousPositionWS, 1.0));
            #endif
            #if defined(LIL_V2F_FURLAYER)
                output.furLayer = _FurRootOffset;
            #endif
            outStream.Append(output);

            // Out
            positionWS += fvmix;
            #if defined(LIL_V2F_POSITION_WS)
                output.positionWS = positionWS;
            #endif
            #if defined(LIL_V2F_POSITION_CS)
                output.positionCS = lilTransformWStoCS(positionWS);
            #endif
            #if defined(LIL_PASS_MOTIONVECTOR_INCLUDED)
                previousPositionWS.xyz += fvmix;
                output.previousPositionCS = mul(UNITY_MATRIX_PREV_VP, float4(previousPositionWS, 1.0));
            #endif
            #if defined(LIL_V2F_FURLAYER)
                output.furLayer = 1;
            #endif
            outStream.Append(output);
        }
        outStream.RestartStrip();
    }
}

#endif