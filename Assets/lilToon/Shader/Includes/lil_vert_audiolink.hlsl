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
        // UV
        float2 audioLinkUV;
        if(_AudioLinkVertexUVMode == 0) audioLinkUV.x = _AudioLinkVertexUVParams.g;
        if(_AudioLinkVertexUVMode == 1) audioLinkUV.x = distance(LIL_MODIFY_TARGET.xyz, _AudioLinkVertexStart.xyz) * _AudioLinkVertexUVParams.r + _AudioLinkVertexUVParams.g;
        if(_AudioLinkVertexUVMode == 2) audioLinkUV.x = lilRotateUV(input.uv0, _AudioLinkVertexUVParams.b).x * _AudioLinkVertexUVParams.r + _AudioLinkVertexUVParams.g;
        audioLinkUV.y = _AudioLinkVertexUVParams.a;

        // Mask (R:Delay G:Band B:Strength)
        float4 audioLinkMask = 1.0;
        if(_AudioLinkVertexUVMode == 3 && Exists_AudioLinkMask)
        {
            audioLinkMask = LIL_SAMPLE_2D_LOD(_AudioLinkMask, sampler_linear_repeat, uvMain, 0);
            audioLinkUV = audioLinkMask.rg;
        }

        // Init value
        float audioLinkValue = saturate(_AudioLinkDefaultValue.x - saturate(frac(LIL_TIME * _AudioLinkDefaultValue.z - audioLinkUV.x)+_AudioLinkDefaultValue.w) * _AudioLinkDefaultValue.y * _AudioLinkDefaultValue.x);

        // Local
        #if defined(LIL_FEATURE_AUDIOLINK_LOCAL)
            if(_AudioLinkAsLocal)
            {
                audioLinkUV.x += frac(-LIL_TIME * _AudioLinkLocalMapParams.r / 60 * _AudioLinkLocalMapParams.g) + _AudioLinkLocalMapParams.b;
                audioLinkValue = LIL_SAMPLE_2D_LOD(_AudioLinkLocalMap, sampler_linear_repeat, audioLinkUV, 0).r;
            }
            else
        #endif

        // Global
        if(lilCheckAudioLink())
        {
            // Scaling for _AudioTexture (4/64)
            audioLinkUV.y *= 0.0625;
            audioLinkValue = LIL_SAMPLE_2D_LOD(_AudioTexture, sampler_linear_clamp, audioLinkUV, 0).r;
            audioLinkValue = saturate(audioLinkValue);
        }

        // Modify
        LIL_MODIFY_TARGET.xyz += (input.normalOS * _AudioLinkVertexStrength.w + _AudioLinkVertexStrength.xyz) * audioLinkValue * audioLinkMask.b;
    }
#endif

#undef LIL_MODIFY_TARGET