//------------------------------------------------------------------------------------------------------------------------------
// Encryption
#if defined(LIL_MODIFY_PREVPOS)
    #define LIL_MODIFY_TARGET input.previousPositionOS
#else
    #define LIL_MODIFY_TARGET input.positionOS
#endif

#if !defined(LIL_LITE) && defined(LIL_FEATURE_ENCRYPTION)
    LIL_MODIFY_TARGET = vertexDecode(LIL_MODIFY_TARGET, input.normalOS, input.uv6, input.uv7);
#endif

#undef LIL_MODIFY_TARGET