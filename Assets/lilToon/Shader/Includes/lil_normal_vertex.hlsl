#ifndef LIL_VERTEX_INCLUDED
#define LIL_VERTEX_INCLUDED

//------------------------------------------------------------------------------------------------------------------------------
// Vertex shader
v2f vert(appdata input)
{
    v2f output;
    LIL_INITIALIZE_STRUCT(v2f, output);

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
    // UV
    float2 uvMain = lilCalcUV(input.uv, _MainTex_ST);

    //----------------------------------------------------------------------------------------------------------------------
    // Outline
    #if defined(LIL_OUTLINE)
        float outlineWidth = _OutlineWidth * 0.01;
        if(Exists_OutlineWidthMask) outlineWidth *= LIL_SAMPLE_2D_LOD(_OutlineWidthMask, sampler_MainTex, uvMain, 0).r;
        if(_OutlineVertexR2Width) outlineWidth *= input.color.r;
        if(_OutlineFixWidth) outlineWidth *= saturate(length(LIL_GET_VIEWDIR_WS(lilOptMul(LIL_MATRIX_M, input.positionOS.xyz).xyz)));
        input.positionOS.xyz += input.normalOS.xyz * outlineWidth;
    #endif

    //----------------------------------------------------------------------------------------------------------------------
    // AudioLink
    #if !defined(LIL_FUR) && defined(LIL_FEATURE_AUDIOLINK) && defined(LIL_FEATURE_AUDIOLINK_VERTEX)
        if(_UseAudioLink && _AudioLink2Vertex)
        {
            float audioLinkValue = 0.0;
            float4 audioLinkMask = 1.0;
            float2 audioLinkUV;
            if(_AudioLinkVertexUVMode == 0) audioLinkUV.x = _AudioLinkVertexUVParams.g;
            if(_AudioLinkVertexUVMode == 1) audioLinkUV.x = distance(input.positionOS.xyz, _AudioLinkVertexStart.xyz) * _AudioLinkVertexUVParams.r + _AudioLinkVertexUVParams.g;
            if(_AudioLinkVertexUVMode == 2) audioLinkUV.x = lilRotateUV(input.uv, _AudioLinkVertexUVParams.b).x * _AudioLinkVertexUVParams.r + _AudioLinkVertexUVParams.g;
            audioLinkUV.y = _AudioLinkVertexUVParams.a;
            // Mask (R:Delay G:Band B:Strength)
            if(_AudioLinkVertexUVMode == 3 && Exists_AudioLinkMask)
            {
                audioLinkMask = LIL_SAMPLE_2D_LOD(_AudioLinkMask, sampler_linear_repeat, uvMain, 0);
                audioLinkUV = audioLinkMask.rg;
            }
            // Scaling for _AudioTexture (4/64)
            #if defined(LIL_FEATURE_AUDIOLINK_LOCAL)
                if(!_AudioLinkAsLocal) audioLinkUV.y *= 0.0625;
            #else
                audioLinkUV.y *= 0.0625;
            #endif
            // Global
            if(_UseAudioLink && _AudioTexture_TexelSize.z > 16)
            {
                audioLinkValue = LIL_SAMPLE_2D_LOD(_AudioTexture, sampler_linear_clamp, audioLinkUV, 0).r;
                audioLinkValue = saturate(audioLinkValue);
            }
            // Local
            #if defined(LIL_FEATURE_AUDIOLINK_LOCAL)
                if(_UseAudioLink && _AudioLinkAsLocal)
                {
                    audioLinkUV.x += frac(-LIL_TIME * _AudioLinkLocalMapParams.r / 60 * _AudioLinkLocalMapParams.g) + _AudioLinkLocalMapParams.b;
                    audioLinkValue = LIL_SAMPLE_2D_LOD(_AudioLinkLocalMap, sampler_linear_repeat, audioLinkUV, 0).r;
                }
            #endif
            input.positionOS.xyz += (input.normalOS * _AudioLinkVertexStrength.w + _AudioLinkVertexStrength.xyz) * audioLinkValue * audioLinkMask.b;
        }
    #endif

    //----------------------------------------------------------------------------------------------------------------------
    // Copy
    LIL_VERTEX_POSITION_INPUTS(input.positionOS, vertexInput);
    #if defined(LIL_OUTLINE) || defined(LIL_SHOULD_NORMAL) && (defined(LIL_FUR) || !defined(LIL_SHOULD_TANGENT))
        LIL_VERTEX_NORMAL_INPUTS(input.normalOS, vertexNormalInput);
    #elif defined(LIL_SHOULD_NORMAL)
        LIL_VERTEX_NORMAL_TANGENT_INPUTS(input.normalOS, input.tangentOS, vertexNormalInput);
    #endif

    output.uv           = input.uv;
    output.positionCS   = vertexInput.positionCS;

    #if defined(LIL_OUTLINE)
        //--------------------------------------------------------------------------------------------------------------
        // Outline
        #if defined(LIL_SHOULD_POSITION_OS)
            output.positionOS   = input.positionOS.xyz;
        #endif
        #if defined(LIL_PASS_FORWARDADD) || defined(LIL_FEATURE_DISTANCE_FADE) || !defined(LIL_BRP) || defined(LIL_USE_LPPV)
            output.positionWS   = vertexInput.positionWS;
        #endif
        #if defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SUBTRACTIVE)
            output.normalWS     = vertexNormalInput.normalWS;
        #endif
    #elif defined(LIL_FUR)
        //--------------------------------------------------------------------------------------------------------------
        // Fur
        #if !defined(LIL_BRP)
            output.positionWS   = vertexInput.positionWS;
            #if defined(LIL_SHOULD_NORMAL)
                output.normalWS     = vertexNormalInput.normalWS;
            #endif
        #elif defined(LIL_PASS_FORWARDADD)
            output.positionWS   = vertexInput.positionWS;
        #else
            output.uv           = input.uv;
            output.positionCS   = vertexInput.positionCS;
            #if defined(LIL_FEATURE_DISTANCE_FADE) || defined(LIL_USE_LPPV)
                output.positionWS   = vertexInput.positionWS;
            #endif
            #if defined(LIL_SHOULD_NORMAL)
                output.normalWS     = vertexNormalInput.normalWS;
            #endif
        #endif
    #else
        //--------------------------------------------------------------------------------------------------------------
        // Normal
        #if defined(LIL_SHOULD_UV1)
            output.uv1          = input.uv1;
        #endif
        #if defined(LIL_SHOULD_POSITION_OS)
            output.positionOS   = input.positionOS.xyz;
        #endif
        #if defined(LIL_SHOULD_POSITION_WS)
            output.positionWS   = vertexInput.positionWS;
        #endif
        #if defined(LIL_SHOULD_NORMAL)
            output.normalWS     = vertexNormalInput.normalWS;
        #endif
        #if defined(LIL_SHOULD_TBN)
            output.tangentWS    = vertexNormalInput.tangentWS;
            output.bitangentWS  = vertexNormalInput.bitangentWS;
        #endif
        #if defined(LIL_SHOULD_TANGENT_W)
            output.tangentW     = input.tangentOS.w;
        #endif
        #if defined(LIL_REFRACTION) && !defined(LIL_PASS_FORWARDADD)
            output.positionSS = vertexInput.positionSS;
        #endif
    #endif

    //----------------------------------------------------------------------------------------------------------------------
    // Clipping Canceller
    #if defined(LIL_FEATURE_CLIPPING_CANCELLER)
        #if defined(UNITY_REVERSED_Z)
            // DirectX
            if(output.positionCS.w < _ProjectionParams.y * 1.01 && output.positionCS.w > 0) output.positionCS.z = output.positionCS.z * 0.0001 + output.positionCS.w * 0.999;
        #else
            // OpenGL
            if(output.positionCS.w < _ProjectionParams.y * 1.01 && output.positionCS.w > 0) output.positionCS.z = output.positionCS.z * 0.0001 - output.positionCS.w * 0.999;
        #endif
    #endif

    //----------------------------------------------------------------------------------------------------------------------
    // Fog & Lightmap & Vertex light
    LIL_CALC_MAINLIGHT(vertexInput, output);
    LIL_TRANSFER_SHADOW(vertexInput, input.uv1, output);
    LIL_TRANSFER_FOG(vertexInput, output);
    LIL_TRANSFER_LIGHTMAPUV(input.uv1, output);
    LIL_CALC_VERTEXLIGHT(vertexInput, output);

    return output;
}

#endif