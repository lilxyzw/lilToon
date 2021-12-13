//------------------------------------------------------------------------------------------------------------------------------
// Outline
#if defined(LIL_MODIFY_PREVPOS)
    #define LIL_MODIFY_TARGET input.previousPositionOS
#else
    #define LIL_MODIFY_TARGET input.positionOS
#endif

#if (defined(LIL_OUTLINE) || defined(LIL_ONEPASS_OUTLINE)) && defined(LIL_LITE)
    LIL_MODIFY_TARGET.xyz += input.normalOS.xyz * lilGetOutlineWidth(LIL_MODIFY_TARGET.xyz, uvMain, input.color, _OutlineWidth, _OutlineWidthMask, _OutlineVertexR2Width, _OutlineFixWidth LIL_SAMP_IN(sampler_linear_repeat));
#elif defined(LIL_OUTLINE) || defined(LIL_ONEPASS_OUTLINE)
    lilCalcOutlinePosition(LIL_MODIFY_TARGET.xyz, uvMain, input.color, input.normalOS, tbnOS, _OutlineWidth, _OutlineWidthMask, _OutlineVertexR2Width, _OutlineFixWidth, _OutlineVectorScale, _OutlineVectorTex LIL_SAMP_IN(sampler_linear_repeat));
#endif

#undef LIL_MODIFY_TARGET