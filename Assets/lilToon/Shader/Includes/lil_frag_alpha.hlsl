//------------------------------------------------------------------------------------------------------------------------------
// Alpha in PS
#define LIL_ALPHA_PS
#if LIL_RENDER > 0
    BEFORE_ANIMATE_MAIN_UV
    OVERRIDE_ANIMATE_MAIN_UV

    //------------------------------------------------------------------------------------------------------------------------------
    // Main Color
    float4 col = 1.0;
    BEFORE_MAIN
    OVERRIDE_MAIN

    //------------------------------------------------------------------------------------------------------------------------------
    // Alpha Mask
    BEFORE_ALPHAMASK
    #if !defined(LIL_LITE) && !defined(LIL_FUR) && defined(LIL_FEATURE_ALPHAMASK)
        OVERRIDE_ALPHAMASK
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Dissolve
    BEFORE_DISSOLVE
    #if !defined(LIL_LITE) && !defined(LIL_FUR) && defined(LIL_FEATURE_DISSOLVE)
        float dissolveAlpha = 0.0;
        OVERRIDE_DISSOLVE
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Fur
    BEFORE_FUR
    #if defined(LIL_FUR) && defined(LIL_V2F_FURLAYER)
        OVERRIDE_FUR
    #endif

    clip(col.a - _Cutoff);
    #if LIL_RENDER == 2 && !defined(SHADER_API_GLES)
        float alphaRef = LIL_SAMPLE_3D(_DitherMaskLOD, sampler_DitherMaskLOD, float3(input.positionCS.xy*0.25,col.a*0.9375)).a;
        clip(alphaRef - 0.01);
    #endif
#endif
#undef LIL_ALPHA_PS