//------------------------------------------------------------------------------------------------------------------------------
// AudioLink
#if defined(LIL_MODIFY_PREVPOS)
    #define LIL_MODIFY_TARGET input.previousPositionOS
#else
    #define LIL_MODIFY_TARGET input.positionOS
#endif

#if !defined(LIL_FUR) && !defined(LIL_LITE) && defined(LIL_FEATURE_AUDIOLINK) && defined(LIL_FEATURE_AUDIOLINK_VERTEX)
    if(_UseAudioLink && _AudioLink2Vertex)
    {
        float audioLinkValue = 0.0;
        float4 audioLinkMask = 1.0;
        float2 audioLinkUV;
        if(_AudioLinkVertexUVMode == 0) audioLinkUV.x = _AudioLinkVertexUVParams.g;
        if(_AudioLinkVertexUVMode == 1) audioLinkUV.x = distance(LIL_MODIFY_TARGET.xyz, _AudioLinkVertexStart.xyz) * _AudioLinkVertexUVParams.r + _AudioLinkVertexUVParams.g;
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
        LIL_MODIFY_TARGET.xyz += (input.normalOS * _AudioLinkVertexStrength.w + _AudioLinkVertexStrength.xyz) * audioLinkValue * audioLinkMask.b;
    }
#endif

#undef LIL_MODIFY_TARGET