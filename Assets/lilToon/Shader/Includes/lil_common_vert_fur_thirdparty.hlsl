//------------------------------------------------------------------------------------------------------------------------------
// FakeFur (based on UnlitWF/UnToon by whiteflare, MIT License)
// https://github.com/whiteflare/Unlit_WF_ShaderSuite
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
            float3 noise0 = normalize(float3(n0) * (2.0/float(0xffffffffU)) - 1.0);
            fvmix += noise0 * _FurVector.w * _FurRandomize;
        #endif
        #if defined(LIL_FEATURE_FurLengthMask)
            fvmix *= LIL_SAMPLE_2D_LOD(_FurLengthMask, lil_sampler_linear_repeat, outUV * _MainTex_ST.xy + _MainTex_ST.zw, 0).r;
        #endif

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