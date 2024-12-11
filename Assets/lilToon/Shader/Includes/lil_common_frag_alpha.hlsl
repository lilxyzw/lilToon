//------------------------------------------------------------------------------------------------------------------------------
// Alpha in PS
// This is included in the subpass fragment shader
#define LIL_ALPHA_PS

//------------------------------------------------------------------------------------------------------------------------------
// UDIM Discard
#if defined(LIL_FEATURE_UDIMDISCARD)
    OVERRIDE_UDIMDISCARD
#endif

#if LIL_RENDER > 0
    #if defined(LIL_V2F_POSITION_WS)
        LIL_GET_POSITION_WS_DATA(input,fd);
    #endif

    #if defined(LIL_OUTLINE)
        BEFORE_ANIMATE_OUTLINE_UV
        OVERRIDE_ANIMATE_OUTLINE_UV
    #else
        BEFORE_ANIMATE_MAIN_UV
        OVERRIDE_ANIMATE_MAIN_UV
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Main Color
    #if defined(LIL_OUTLINE)
        BEFORE_OUTLINE_COLOR
        OVERRIDE_OUTLINE_COLOR
    #else
        BEFORE_MAIN
        OVERRIDE_MAIN
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Layer Color
    #if defined(LIL_V2F_TANGENT_WS)
        fd.isRightHand = input.tangentWS.w > 0.0;
    #endif
    // 2nd
    BEFORE_MAIN2ND
    #if defined(LIL_FEATURE_MAIN2ND)
        float main2ndDissolveAlpha = 0.0;
        float4 color2nd = 1.0;
        OVERRIDE_MAIN2ND
    #endif

    // 3rd
    BEFORE_MAIN3RD
    #if defined(LIL_FEATURE_MAIN3RD)
        float main3rdDissolveAlpha = 0.0;
        float4 color3rd = 1.0;
        OVERRIDE_MAIN3RD
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Alpha Mask
    BEFORE_ALPHAMASK
    #if !defined(LIL_LITE) && defined(LIL_FEATURE_ALPHAMASK)
        OVERRIDE_ALPHAMASK
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Dissolve
    BEFORE_DISSOLVE
    #if !defined(LIL_LITE) && defined(LIL_FEATURE_DISSOLVE)
        float dissolveAlpha = 0.0;
        if (fd.dissolveActive)
        {
            float priorAlpha = fd.col.a;
            fd.col.a = 1.0f;
            OVERRIDE_DISSOLVE
            if (fd.dissolveInvert)
            {
                fd.col.a = 1.0f - fd.col.a;
                dissolveAlpha = 1.0f - dissolveAlpha;
            }
                        
            fd.col.a *= priorAlpha;
        }
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Fur
    BEFORE_FUR
    #if defined(LIL_FUR) && defined(LIL_V2F_FURLAYER)
        OVERRIDE_FUR
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Dither
    BEFORE_DITHER
    #if !defined(LIL_LITE) && defined(LIL_FEATURE_DITHER) && LIL_RENDER == 1
        OVERRIDE_DITHER
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Cutout
    clip(fd.col.a - _Cutoff);
    #if LIL_RENDER == 2 && !defined(SHADER_API_GLES)
        // Dither
        float alphaRef = fd.col.a;
        #if defined(LIL_FUR) && defined(LIL_V2F_FURLAYER) && defined(SHADERPASS) && (SHADERPASS ==  SHADERPASS_SHADOWS)
            fd.col.a = saturate(fd.col.a*5.0);
        #endif
        #if LIL_SUBPASS_TRANSPARENT_MODE == 1 || defined(SHADERPASS) && (SHADERPASS ==  SHADERPASS_SHADOWS)
            alphaRef = lilSampleDither(_DitherMaskLOD, input.positionCS.xy, fd.col.a);
        #elif LIL_SUBPASS_TRANSPARENT_MODE == 0 && defined(LIL_PASS_SHADOWCASTER_INCLUDED)
            #if defined(SHADOWS_DEPTH)
                if(LIL_MATRIX_P._m33 != 0.0)
            #endif
            alphaRef = lilSampleDither(_DitherMaskLOD, input.positionCS.xy, fd.col.a);
        #endif
        clip(alphaRef - _SubpassCutoff);
    #endif
#endif
#undef LIL_ALPHA_PS