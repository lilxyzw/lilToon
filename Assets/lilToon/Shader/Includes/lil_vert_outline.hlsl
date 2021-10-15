//------------------------------------------------------------------------------------------------------------------------------
// Outline
#if defined(LIL_MODIFY_PREVPOS)
    #define LIL_MODIFY_TARGET input.previousPositionOS
#else
    #define LIL_MODIFY_TARGET input.positionOS
#endif

#if defined(LIL_OUTLINE) || defined(LIL_ONEPASS_OUTLINE)
    LIL_MODIFY_TARGET.xyz += input.normalOS.xyz * lilGetOutlineWidth(LIL_MODIFY_TARGET.xyz, uvMain, input.color, _OutlineWidth, _OutlineWidthMask, _OutlineVertexR2Width, _OutlineFixWidth LIL_SAMP_IN(sampler_linear_repeat));
#endif

#undef LIL_MODIFY_TARGET