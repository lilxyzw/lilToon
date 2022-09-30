//------------------------------------------------------------------------------------------------------------------------------
// Outline
#if defined(LIL_OUTLINE) || defined(LIL_ONEPASS_OUTLINE)
    #if defined(LIL_MODIFY_PREVPOS)
        #define LIL_MODIFY_TARGET input.previousPositionOS
    #else
        #define LIL_MODIFY_TARGET input.positionOS
    #endif

    #if defined(LIL_PASS_SHADOWCASTER_INCLUDED)
        if(LIL_MATRIX_P._m33 == 0.0)
    #endif

    #if defined(LIL_LITE)
        #if !defined(LIL_PASS_SHADOWCASTER_INCLUDED) && !(defined(LIL_URP) && defined(LIL_PASS_DEPTHONLY_INCLUDED))
            lilCalcOutlinePositionLite(LIL_MODIFY_TARGET.xyz, uvMain, input.color, input.normalOS, tbnOS, _OutlineWidth, _OutlineWidthMask, _OutlineVertexR2Width, _OutlineFixWidth, _OutlineZBias LIL_SAMP_IN(lil_sampler_linear_repeat));
        #else
            lilCalcOutlinePositionLite(LIL_MODIFY_TARGET.xyz, uvMain, input.color, input.normalOS, tbnOS, _OutlineWidth, _OutlineWidthMask, _OutlineVertexR2Width, _OutlineFixWidth, 0 LIL_SAMP_IN(lil_sampler_linear_repeat));
        #endif
    #else
        #if !defined(LIL_PASS_SHADOWCASTER_INCLUDED) && !(defined(LIL_URP) && defined(LIL_PASS_DEPTHONLY_INCLUDED))
            lilCalcOutlinePosition(LIL_MODIFY_TARGET.xyz, uvs, input.color, input.normalOS, tbnOS, _OutlineWidth, _OutlineWidthMask, _OutlineVertexR2Width, _OutlineFixWidth, _OutlineZBias, _OutlineVectorScale, _OutlineVectorUVMode, _OutlineVectorTex LIL_SAMP_IN(lil_sampler_linear_repeat));
        #else
            lilCalcOutlinePosition(LIL_MODIFY_TARGET.xyz, uvs, input.color, input.normalOS, tbnOS, _OutlineWidth, _OutlineWidthMask, _OutlineVertexR2Width, _OutlineFixWidth, 0, _OutlineVectorScale, _OutlineVectorUVMode, _OutlineVectorTex LIL_SAMP_IN(lil_sampler_linear_repeat));
        #endif
    #endif

    #undef LIL_MODIFY_TARGET
#endif