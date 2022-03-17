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
        lilCalcOutlinePositionLite(LIL_MODIFY_TARGET.xyz, uvMain, input.color, input.normalOS, tbnOS, _OutlineWidth, _OutlineWidthMask, _OutlineVertexR2Width, _OutlineFixWidth LIL_SAMP_IN(sampler_linear_repeat));
    #else
        lilCalcOutlinePosition(LIL_MODIFY_TARGET.xyz, uvMain, input.color, input.normalOS, tbnOS, _OutlineWidth, _OutlineWidthMask, _OutlineVertexR2Width, _OutlineFixWidth, _OutlineVectorScale, _OutlineVectorTex LIL_SAMP_IN(sampler_linear_repeat));
    #endif

    #undef LIL_MODIFY_TARGET
#endif