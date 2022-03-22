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
    float2 uvMain = lilCalcUV(input.uv0, _MainTex_ST);

    //------------------------------------------------------------------------------------------------------------------------------
    // Vertex Modification
    #include "lil_vert_encryption.hlsl"
    lilCustomVertexOS(input, uvMain, input.positionOS);

    //------------------------------------------------------------------------------------------------------------------------------
    // Previous Position (for HDRP)
    #if defined(LIL_PASS_MOTIONVECTOR_INCLUDED)
        input.previousPositionOS = unity_MotionVectorsParams.x > 0.0 ? input.previousPositionOS : input.positionOS.xyz;
        #if defined(_ADD_PRECOMPUTED_VELOCITY)
            input.previousPositionOS -= input.precomputedVelocity;
        #endif
        #define LIL_MODIFY_PREVPOS
        #include "lil_vert_encryption.hlsl"
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
        output.uv0              = input.uv0;
    #endif
    #if defined(LIL_V2G_TEXCOORD1)
        output.uv1              = input.uv1;
    #endif
    #if defined(LIL_V2G_TEXCOORD2)
        output.uv2              = input.uv2;
    #endif
    #if defined(LIL_V2G_TEXCOORD3)
        output.uv3              = input.uv3;
    #endif
    #if defined(LIL_V2G_PACKED_TEXCOORD01)
        output.uv01.xy          = input.uv0;
        output.uv01.zw          = input.uv1;
    #endif
    #if defined(LIL_V2G_PACKED_TEXCOORD23)
        output.uv23.xy          = input.uv2;
        output.uv23.zw          = input.uv3;
    #endif
    #if !defined(LIL_NOT_SUPPORT_VERTEXID) && defined(LIL_V2G_VERTEXID)
        output.vertexID         = input.vertexID;
    #endif
    #if defined(LIL_V2G_NORMAL_WS)
        output.normalWS         = vertexNormalInput.normalWS;
    #endif

    LIL_CUSTOM_VERT_COPY

    //------------------------------------------------------------------------------------------------------------------------------
    // Fog & Lighting
    lilFragData fd = lilInitFragData();
    LIL_GET_HDRPDATA(vertexInput,fd);
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
        output.indLightColor    = lightdataInput.indLightColor * _ShadowEnvStrength;
    #endif
    #if defined(LIL_V2G_SHADOW)
        LIL_TRANSFER_SHADOW(vertexInput, input.uv1, output);
    #endif
    #if defined(LIL_V2G_VERTEXLIGHT_FOG)
        LIL_TRANSFER_FOG(vertexInput, output);
        LIL_CALC_VERTEXLIGHT(vertexInput, output);
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Vector
    #if defined(LIL_V2G_FURVECTOR)
        float3 bitangentOS = normalize(cross(input.normalOS, input.tangentOS.xyz)) * (input.tangentOS.w * length(input.normalOS));
        float3x3 tbnOS = float3x3(input.tangentOS.xyz, bitangentOS, input.normalOS);
        output.furVector = _FurVector.xyz;
        if(_VertexColor2FurVector) output.furVector = lilBlendNormal(output.furVector, input.color.xyz);
        if(Exists_FurVectorTex) output.furVector = lilBlendNormal(output.furVector, lilUnpackNormalScale(LIL_SAMPLE_2D_LOD(_FurVectorTex, sampler_linear_repeat, uvMain, 0), _FurVectorScale));
        output.furVector = mul(normalize(output.furVector), tbnOS);
        output.furVector *= _FurVector.w;
        #if defined(LIL_FUR_PRE)
            output.furVector *= _FurCutoutLength;
        #endif
        output.furVector = lilTransformDirOStoWS(output.furVector, false);
        float furLength = length(output.furVector);
        output.furVector.y -= _FurGravity * furLength;

        #if defined(LIL_FEATURE_FUR_COLLISION) && defined(LIL_BRP) && defined(VERTEXLIGHT_ON)
            // Touch
            float3 positionWS2 = output.positionWS + output.furVector;
            float4 toLightX = unity_4LightPosX0 - positionWS2.x;
            float4 toLightY = unity_4LightPosY0 - positionWS2.y;
            float4 toLightZ = unity_4LightPosZ0 - positionWS2.z;
            float4 lengthSq = toLightX * toLightX + 0.000001;
            lengthSq += toLightY * toLightY;
            lengthSq += toLightZ * toLightZ;
            float4 atten = saturate(1.0 - lengthSq * unity_4LightAtten0 / 25.0) * _FurTouchStrength * furLength;
            //float4 rangeToggle = abs(frac(sqrt(25.0 / unity_4LightAtten0) * 100.0) - 0.22);
            float4 rangeToggle = abs(frac(sqrt(250000 / unity_4LightAtten0)) - 0.22);
            output.furVector = rangeToggle[0] < 0.001 - unity_LightColor[0].r - unity_LightColor[0].g - unity_LightColor[0].b ? output.furVector - float3(toLightX[0], toLightY[0], toLightZ[0]) * rsqrt(lengthSq[0]) * atten[0] : output.furVector;
            output.furVector = rangeToggle[1] < 0.001 - unity_LightColor[1].r - unity_LightColor[1].g - unity_LightColor[1].b ? output.furVector - float3(toLightX[1], toLightY[1], toLightZ[1]) * rsqrt(lengthSq[1]) * atten[1] : output.furVector;
            output.furVector = rangeToggle[2] < 0.001 - unity_LightColor[2].r - unity_LightColor[2].g - unity_LightColor[2].b ? output.furVector - float3(toLightX[2], toLightY[2], toLightZ[2]) * rsqrt(lengthSq[2]) * atten[2] : output.furVector;
            output.furVector = rangeToggle[3] < 0.001 - unity_LightColor[3].r - unity_LightColor[3].g - unity_LightColor[3].b ? output.furVector - float3(toLightX[3], toLightY[3], toLightZ[3]) * rsqrt(lengthSq[3]) * atten[3] : output.furVector;
        #endif
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

    LIL_SETUP_INSTANCE_ID(input[0]);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input[0]);

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
                outputBase[i].uv0 = input[i].uv0;
            #endif
            #if defined(LIL_V2F_NORMAL_WS)
                outputBase[i].normalWS = input[i].normalWS;
            #endif
            #if defined(LIL_V2F_FURLAYER)
                outputBase[i].furLayer = -2;
            #endif
        }

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

    //------------------------------------------------------------------------------------------------------------------------------
    // Mid
    #if defined(LIL_V2G_FURVECTOR)
        float3 fvc = (input[0].furVector    +input[1].furVector     +input[2].furVector)    *0.333333333333;
    #endif
    #if defined(LIL_V2G_POSITION_WS)
        float3 wpc = (input[0].positionWS   +input[1].positionWS    +input[2].positionWS)   *0.333333333333;
    #endif
    #if defined(LIL_V2G_TEXCOORD0)
        float2 uv0c = (input[0].uv0         +input[1].uv0           +input[2].uv0)          *0.333333333333;
    #endif
    #if defined(LIL_V2G_TEXCOORD1)
        float2 uv1c = (input[0].uv1         +input[1].uv1           +input[2].uv1)          *0.333333333333;
    #endif
    #if defined(LIL_V2G_TEXCOORD2)
        float2 uv1c = (input[0].uv2         +input[1].uv2           +input[2].uv2)          *0.333333333333;
    #endif
    #if defined(LIL_V2G_TEXCOORD3)
        float2 uv1c = (input[0].uv3         +input[1].uv3           +input[2].uv3)          *0.333333333333;
    #endif
    #if defined(LIL_V2G_PACKED_TEXCOORD01)
        float4 uv01c = (input[0].uv01       +input[1].uv01          +input[2].uv01)         *0.333333333333;
    #endif
    #if defined(LIL_V2G_PACKED_TEXCOORD23)
        float4 uv23c = (input[0].uv23       +input[1].uv23          +input[2].uv23)         *0.333333333333;
    #endif
    #if defined(LIL_V2G_NORMAL_WS)
        float3 ndc = (input[0].normalWS     +input[1].normalWS      +input[2].normalWS)     *0.333333333333;
    #endif
    #if defined(LIL_V2G_VERTEXLIGHT_FOG) && !(!defined(LIL_USE_ADDITIONALLIGHT_VS) && defined(LIL_HDRP))
        LIL_VERTEXLIGHT_FOG_TYPE vlfc = (input[0].vlf + input[1].vlf + input[2].vlf) * 0.333333333333;
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
    // FakeFur (based on UnlitWF/UnToon by whiteflare, MIT License)
    // https://github.com/whiteflare/Unlit_WF_ShaderSuite
    for(uint fl = 0; fl < _FurLayerNum; fl++)
    {
        float lpmix = fl/(float)_FurLayerNum;
        for(int ii=0;ii<4;ii++)
        {
            int ii2 = ii==3 ? 0 : ii;

            // Common
            float2 outUV = lerp(input[ii2].uv0,uv0c,lpmix);
            #if defined(LIL_V2F_TEXCOORD0)
                output.uv0 = outUV;
            #endif
            #if defined(LIL_V2F_TEXCOORD1)
                output.uv1 = lerp(input[ii2].uv1,uv1c,lpmix);
            #endif
            #if defined(LIL_V2F_TEXCOORD2)
                output.uv2 = lerp(input[ii2].uv2,uv2c,lpmix);
            #endif
            #if defined(LIL_V2F_TEXCOORD3)
                output.uv3 = lerp(input[ii2].uv3,uv3c,lpmix);
            #endif
            #if defined(LIL_V2F_PACKED_TEXCOORD01)
                output.uv01 = lerp(input[ii2].uv01,uv01c,lpmix);
            #endif
            #if defined(LIL_V2F_PACKED_TEXCOORD23)
                output.uv23 = lerp(input[ii2].uv23,uv23c,lpmix);
            #endif
            #if defined(LIL_V2F_NORMAL_WS)
                output.normalWS = lerp(input[ii2].normalWS,ndc,lpmix);
            #endif
            #if defined(LIL_V2F_VERTEXLIGHT_FOG) && !(!defined(LIL_USE_ADDITIONALLIGHT_VS) && defined(LIL_HDRP))
                output.vlf = lerp(input[ii2].vlf,vlfc,lpmix);
            #endif

            float3 fvmix = lerp(input[ii2].furVector,fvc,lpmix);
            float3 furVector = normalize(fvmix);
            #if !defined(LIL_NOT_SUPPORT_VERTEXID)
                uint3 n0 = (input[0].vertexID * input[1].vertexID * input[2].vertexID + (fl * 439853 + ii * 364273 + 1)) * uint3(1597334677U, 3812015801U, 2912667907U);
                //uint3 n0 = (input[0].vertexID * input[1].vertexID * input[2].vertexID + (fl * 439853 + 1)) * uint3(1597334677U, 3812015801U, 2912667907U);
                float3 noise0 = normalize(float3(n0) * (2.0/float(0xffffffffU)) - 1.0);
                fvmix += noise0 * _FurVector.w * _FurRandomize;
            #endif
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
                output.furLayer = 0;
            #endif
            #if defined(LIL_V2F_POSITION_CS) && defined(LIL_FEATURE_CLIPPING_CANCELLER) && !defined(LIL_PASS_SHADOWCASTER_INCLUDED) && !defined(LIL_PASS_META_INCLUDED)
                //------------------------------------------------------------------------------------------------------------------------------
                // Clipping Canceller
                #if defined(UNITY_REVERSED_Z)
                    // DirectX
                    if(output.positionCS.w < _ProjectionParams.y * 1.01 && output.positionCS.w > 0 && _ProjectionParams.y < LIL_NEARCLIP_THRESHOLD LIL_MULTI_SHOULD_CLIPPING)
                    {
                        output.positionCS.z = output.positionCS.z * 0.0001 + output.positionCS.w * 0.999;
                    }
                #else
                    // OpenGL
                    if(output.positionCS.w < _ProjectionParams.y * 1.01 && output.positionCS.w > 0 && _ProjectionParams.y < LIL_NEARCLIP_THRESHOLD LIL_MULTI_SHOULD_CLIPPING)
                    {
                        output.positionCS.z = output.positionCS.z * 0.0001 - output.positionCS.w * 0.999;
                    }
                #endif
            #endif
            #if defined(LIL_FUR_PRE)
                #if defined(UNITY_REVERSED_Z)
                    // DirectX
                    output.positionCS.z -= 0.0000001;
                #else
                    // OpenGL
                    output.positionCS.z += 0.0000001;
                #endif
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
            #if defined(LIL_V2F_POSITION_CS) && defined(LIL_FEATURE_CLIPPING_CANCELLER) && !defined(LIL_PASS_SHADOWCASTER_INCLUDED) && !defined(LIL_PASS_META_INCLUDED)
                //------------------------------------------------------------------------------------------------------------------------------
                // Clipping Canceller
                #if defined(UNITY_REVERSED_Z)
                    // DirectX
                    if(output.positionCS.w < _ProjectionParams.y * 1.01 && output.positionCS.w > 0 && _ProjectionParams.y < LIL_NEARCLIP_THRESHOLD LIL_MULTI_SHOULD_CLIPPING)
                    {
                        output.positionCS.z = output.positionCS.z * 0.0001 + output.positionCS.w * 0.999;
                    }
                #else
                    // OpenGL
                    if(output.positionCS.w < _ProjectionParams.y * 1.01 && output.positionCS.w > 0 && _ProjectionParams.y < LIL_NEARCLIP_THRESHOLD LIL_MULTI_SHOULD_CLIPPING)
                    {
                        output.positionCS.z = output.positionCS.z * 0.0001 - output.positionCS.w * 0.999;
                    }
                #endif
            #endif
            #if defined(LIL_FUR_PRE)
                #if defined(UNITY_REVERSED_Z)
                    // DirectX
                    output.positionCS.z -= 0.0000001;
                #else
                    // OpenGL
                    output.positionCS.z += 0.0000001;
                #endif
            #endif
            outStream.Append(output);
        }
        outStream.RestartStrip();
    }
}

#endif