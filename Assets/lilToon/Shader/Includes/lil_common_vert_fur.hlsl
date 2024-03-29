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
    if(_Invisible) return output;
    
    //------------------------------------------------------------------------------------------------------------------------------
    // Single Pass Instanced rendering
    LIL_SETUP_INSTANCE_ID(input);
    LIL_TRANSFER_INSTANCE_ID(input, output);
    LIL_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    //------------------------------------------------------------------------------------------------------------------------------
    // UV
    float2 uvMain = lilCalcUV(input.uv0, _MainTex_ST);
    float2 uvs[4] = {uvMain,input.uv1,input.uv2,input.uv3};

    //------------------------------------------------------------------------------------------------------------------------------
    // Vertex Modification
    #include "lil_vert_encryption.hlsl"
    lilCustomVertexOS(input, uvMain, input.positionOS);

    //------------------------------------------------------------------------------------------------------------------------------
    // Previous Position (for HDRP)
    #if defined(LIL_PASS_MOTIONVECTOR_INCLUDED)
        input.previousPositionOS = lilSelectPreviousPosition(input.previousPositionOS, input.positionOS.xyz);
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
        previousVertexInput.positionWS = lilTransformPreviousObjectToWorld(input.previousPositionOS);
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
    // UDIM Discard
    #if defined(LIL_FEATURE_UDIMDISCARD) && !defined(LIL_LITE)
        if(_UDIMDiscardMode == 0 && _UDIMDiscardCompile == 1 && LIL_CHECK_UDIMDISCARD(input)) // Discard Vertices instead of just pixels
        {
            #if defined(LIL_V2F_POSITION_CS)
            output.positionWS = 0.0/0.0;
            #endif
            return output;
        }
    #endif
    
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
        output.furVector = _FurVector.xyz + float3(0,0,0.001);
        if(_VertexColor2FurVector) output.furVector = lilBlendNormal(output.furVector, input.color.xyz);
        #if defined(LIL_FEATURE_FurVectorTex)
            output.furVector = lilBlendNormal(output.furVector, lilUnpackNormalScale(LIL_SAMPLE_2D_LOD(_FurVectorTex, lil_sampler_linear_repeat, uvMain, 0), _FurVectorScale));
        #endif
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

    //------------------------------------------------------------------------------------------------------------------------------
    // IDMask
    #if defined(LIL_FEATURE_IDMASK) && !defined(LIL_NOT_SUPPORT_VERTEXID)
        int idMaskIndices[8] = {_IDMaskIndex1,_IDMaskIndex2,_IDMaskIndex3,_IDMaskIndex4,_IDMaskIndex5,_IDMaskIndex6,_IDMaskIndex7,_IDMaskIndex8};
        float idMaskFlags[8] = {_IDMask1,_IDMask2,_IDMask3,_IDMask4,_IDMask5,_IDMask6,_IDMask7,_IDMask8};
        uint idMaskArg = 0;
        switch(_IDMaskFrom)
        {
            #if defined(LIL_APP_TEXCOORD0)
                case 0: idMaskArg = input.uv0.x; break;
            #endif
            #if defined(LIL_APP_TEXCOORD1)
                case 1: idMaskArg = input.uv1.x; break;
            #endif
            #if defined(LIL_APP_TEXCOORD2)
                case 2: idMaskArg = input.uv2.x; break;
            #endif
            #if defined(LIL_APP_TEXCOORD3)
                case 3: idMaskArg = input.uv3.x; break;
            #endif
            #if defined(LIL_APP_TEXCOORD4)
                case 4: idMaskArg = input.uv4.x; break;
            #endif
            #if defined(LIL_APP_TEXCOORD5)
                case 5: idMaskArg = input.uv5.x; break;
            #endif
            #if defined(LIL_APP_TEXCOORD6)
                case 6: idMaskArg = input.uv6.x; break;
            #endif
            #if defined(LIL_APP_TEXCOORD7)
                case 7: idMaskArg = input.uv7.x; break;
            #endif
            default: idMaskArg = input.vertexID; break;
        }
        bool idMasked = IDMask(idMaskArg,_IDMaskIsBitmap,idMaskIndices,idMaskFlags);
        #if defined(LIL_V2G_POSITION_WS)
            output.positionWS = idMasked ? 0.0/0.0 : output.positionWS;
        #endif
    #endif

    return output;
}

//------------------------------------------------------------------------------------------------------------------------------
// Fin
float  lilLerp3(float  a, float  b, float  c, float3 factor) { return a * factor.x + b * factor.y + c * factor.z; }
float2 lilLerp3(float2 a, float2 b, float2 c, float3 factor) { return a * factor.x + b * factor.y + c * factor.z; }
float3 lilLerp3(float3 a, float3 b, float3 c, float3 factor) { return a * factor.x + b * factor.y + c * factor.z; }
float4 lilLerp3(float4 a, float4 b, float4 c, float3 factor) { return a * factor.x + b * factor.y + c * factor.z; }
float  lilLerp3(float  a[3], float3 factor) { return lilLerp3(a[0], a[1], a[2], factor); }
float2 lilLerp3(float2 a[3], float3 factor) { return lilLerp3(a[0], a[1], a[2], factor); }
float3 lilLerp3(float3 a[3], float3 factor) { return lilLerp3(a[0], a[1], a[2], factor); }
float4 lilLerp3(float4 a[3], float3 factor) { return lilLerp3(a[0], a[1], a[2], factor); }

void AppendFur(inout TriangleStream<v2f> outStream, inout v2f output, v2g input[3], float3 furVectors[3], float3 factor)
{
    #if defined(LIL_V2F_TEXCOORD0)
        output.uv0 = lilLerp3(input[0].uv0, input[1].uv0, input[2].uv0, factor);
    #endif
    #if defined(LIL_V2F_TEXCOORD1)
        output.uv1 = lilLerp3(input[0].uv1, input[1].uv1, input[2].uv1, factor);
    #endif
    #if defined(LIL_V2F_TEXCOORD2)
        output.uv2 = lilLerp3(input[0].uv2, input[1].uv2, input[2].uv2, factor);
    #endif
    #if defined(LIL_V2F_TEXCOORD3)
        output.uv3 = lilLerp3(input[0].uv3, input[1].uv3, input[2].uv3, factor);
    #endif
    #if defined(LIL_V2F_PACKED_TEXCOORD01)
        output.uv01 = lilLerp3(input[0].uv01, input[1].uv01, input[2].uv01, factor);
    #endif
    #if defined(LIL_V2F_PACKED_TEXCOORD23)
        output.uv23 = lilLerp3(input[0].uv23, input[1].uv23, input[2].uv23, factor);
    #endif
    #if defined(LIL_V2F_NORMAL_WS)
        output.normalWS = lilLerp3(input[0].normalWS, input[1].normalWS, input[2].normalWS, factor);
    #endif
    #if defined(LIL_V2F_LIGHTCOLOR) && defined(LIL_FEATURE_LTCGI) && defined(LIL_PASS_FORWARD)
        output.lightColor = lilLerp3(input[0].lightColor, input[1].lightColor, input[2].lightColor, factor);
    #endif
    #if defined(LIL_V2F_VERTEXLIGHT_FOG) && !(!defined(LIL_USE_ADDITIONALLIGHT_VS) && defined(LIL_HDRP))
        output.vlf = lilLerp3(input[0].vlf, input[1].vlf, input[2].vlf, factor);
    #endif

    float3 positionWS = lilLerp3(input[0].positionWS, input[1].positionWS, input[2].positionWS, factor);
    #if defined(LIL_V2F_POSITION_WS)
        output.positionWS = positionWS;
    #endif
    #if defined(LIL_V2F_POSITION_CS)
        output.positionCS = lilTransformWStoCS(positionWS);
    #endif
    #if defined(LIL_PASS_MOTIONVECTOR_INCLUDED)
        float3 previousPositionWS = lilLerp3(input[0].previousPositionWS, input[1].previousPositionWS, input[2].previousPositionWS, factor);
        output.previousPositionCS = mul(LIL_MATRIX_PREV_VP, float4(previousPositionWS, 1.0));
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
    #if defined(LIL_V2F_POSITION_CS) && defined(LIL_FUR_PRE)
        #if defined(UNITY_REVERSED_Z)
            // DirectX
            output.positionCS.z -= 0.0000001;
        #else
            // OpenGL
            output.positionCS.z += 0.0000001;
        #endif
    #endif

    outStream.Append(output);

    float3 mixVector = lilLerp3(furVectors[0], furVectors[1], furVectors[2], factor);
    positionWS.xyz += mixVector;
    #if defined(LIL_V2F_POSITION_WS)
        output.positionWS = positionWS;
    #endif
    #if defined(LIL_V2F_POSITION_CS)
        output.positionCS = lilTransformWStoCS(positionWS);
    #endif
    #if defined(LIL_PASS_MOTIONVECTOR_INCLUDED)
        previousPositionWS.xyz += mixVector;
        output.previousPositionCS = mul(LIL_MATRIX_PREV_VP, float4(previousPositionWS, 1.0));
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
    #if defined(LIL_V2F_POSITION_CS) && defined(LIL_FUR_PRE)
        #if defined(UNITY_REVERSED_Z)
            // DirectX
            output.positionCS.z -= 0.000001;
        #else
            // OpenGL
            output.positionCS.z += 0.000001;
        #endif
    #endif
    outStream.Append(output);
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
                outputBase[i].previousPositionCS = mul(LIL_MATRIX_PREV_VP, float4(input[i].previousPositionWS, 1.0));
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
        if(_Cull != 1)
        {
            outStream.Append(outputBase[0]);
            outStream.Append(outputBase[1]);
            outStream.Append(outputBase[2]);
            outStream.RestartStrip();
        }

        // Back
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

    if(_FurMeshType)
    {
        #include "lil_common_vert_fur_thirdparty.hlsl"
    }
    else
    {
        float3 furVectors[3];
        furVectors[0] = input[0].furVector;
        furVectors[1] = input[1].furVector;
        furVectors[2] = input[2].furVector;
        #if !defined(LIL_NOT_SUPPORT_VERTEXID)
            uint3 n0 = (input[0].vertexID * 3 + input[1].vertexID * 1 + input[2].vertexID * 1) * uint3(1597334677U, 3812015801U, 2912667907U);
            uint3 n1 = (input[0].vertexID * 1 + input[1].vertexID * 3 + input[2].vertexID * 1) * uint3(1597334677U, 3812015801U, 2912667907U);
            uint3 n2 = (input[0].vertexID * 1 + input[1].vertexID * 1 + input[2].vertexID * 3) * uint3(1597334677U, 3812015801U, 2912667907U);
            float3 noise0 = normalize(float3(n0) * (2.0/float(0xffffffffU)) - 1.0);
            float3 noise1 = normalize(float3(n1) * (2.0/float(0xffffffffU)) - 1.0);
            float3 noise2 = normalize(float3(n2) * (2.0/float(0xffffffffU)) - 1.0);
            furVectors[0] += noise0 * _FurVector.w * _FurRandomize;
            furVectors[1] += noise1 * _FurVector.w * _FurRandomize;
            furVectors[2] += noise2 * _FurVector.w * _FurRandomize;
        #endif
        #if defined(LIL_FEATURE_FurLengthMask)
            furVectors[0] *= LIL_SAMPLE_2D_LOD(_FurLengthMask, lil_sampler_linear_repeat, input[0].uv0 * _MainTex_ST.xy + _MainTex_ST.zw, 0).r;
            furVectors[1] *= LIL_SAMPLE_2D_LOD(_FurLengthMask, lil_sampler_linear_repeat, input[1].uv0 * _MainTex_ST.xy + _MainTex_ST.zw, 0).r;
            furVectors[2] *= LIL_SAMPLE_2D_LOD(_FurLengthMask, lil_sampler_linear_repeat, input[2].uv0 * _MainTex_ST.xy + _MainTex_ST.zw, 0).r;
        #endif

        if(_FurLayerNum == 1)
        {
            AppendFur(outStream, output, input, furVectors, float3(1.0, 0.0, 0.0) / 1.0);
            AppendFur(outStream, output, input, furVectors, float3(0.0, 1.0, 0.0) / 1.0);
            AppendFur(outStream, output, input, furVectors, float3(0.0, 0.0, 1.0) / 1.0);
        }
        else if(_FurLayerNum >= 2)
        {
            AppendFur(outStream, output, input, furVectors, float3(1.0, 0.0, 0.0) / 1.0);
            AppendFur(outStream, output, input, furVectors, float3(0.0, 1.0, 1.0) / 2.0);
            AppendFur(outStream, output, input, furVectors, float3(0.0, 1.0, 0.0) / 1.0);
            AppendFur(outStream, output, input, furVectors, float3(1.0, 0.0, 1.0) / 2.0);
            AppendFur(outStream, output, input, furVectors, float3(0.0, 0.0, 1.0) / 1.0);
            AppendFur(outStream, output, input, furVectors, float3(1.0, 1.0, 0.0) / 2.0);
        }
        if(_FurLayerNum >= 3)
        {
            AppendFur(outStream, output, input, furVectors, float3(1.0, 4.0, 1.0) / 6.0);
            AppendFur(outStream, output, input, furVectors, float3(0.0, 1.0, 1.0) / 2.0);
            AppendFur(outStream, output, input, furVectors, float3(1.0, 1.0, 4.0) / 6.0);
            AppendFur(outStream, output, input, furVectors, float3(1.0, 0.0, 1.0) / 2.0);
            AppendFur(outStream, output, input, furVectors, float3(4.0, 1.0, 1.0) / 6.0);
            AppendFur(outStream, output, input, furVectors, float3(1.0, 1.0, 0.0) / 2.0);
        }
        AppendFur(outStream, output, input, furVectors, float3(1.0, 0.0, 0.0) / 1.0);
        outStream.RestartStrip();
    }
}

#endif