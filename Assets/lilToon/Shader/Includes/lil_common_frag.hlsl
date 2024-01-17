//------------------------------------------------------------------------------------------------------------------------------
// PS Macro

// Insert for custom shader
#if !defined(BEFORE_UNPACK_V2F)
    #define BEFORE_UNPACK_V2F
#endif

#if !defined(BEFORE_ANIMATE_MAIN_UV)
    #define BEFORE_ANIMATE_MAIN_UV
#endif

#if !defined(BEFORE_ANIMATE_OUTLINE_UV)
    #define BEFORE_ANIMATE_OUTLINE_UV
#endif

#if !defined(BEFORE_CALC_DDX_DDY)
    #define BEFORE_CALC_DDX_DDY
#endif

#if !defined(BEFORE_PARALLAX)
    #define BEFORE_PARALLAX
#endif

#if !defined(BEFORE_MAIN)
    #define BEFORE_MAIN
#endif

#if !defined(BEFORE_OUTLINE_COLOR)
    #define BEFORE_OUTLINE_COLOR
#endif

#if !defined(BEFORE_FUR)
    #define BEFORE_FUR
#endif

#if !defined(BEFORE_ALPHAMASK)
    #define BEFORE_ALPHAMASK
#endif

#if !defined(BEFORE_DISSOLVE)
    #define BEFORE_DISSOLVE
#endif

#if !defined(BEFORE_DITHER)
    #define BEFORE_DITHER
#endif

#if !defined(BEFORE_NORMAL_1ST)
    #define BEFORE_NORMAL_1ST
#endif

#if !defined(BEFORE_NORMAL_2ND)
    #define BEFORE_NORMAL_2ND
#endif

#if !defined(BEFORE_ANISOTROPY)
    #define BEFORE_ANISOTROPY
#endif

#if !defined(BEFORE_AUDIOLINK)
    #define BEFORE_AUDIOLINK
#endif

#if !defined(BEFORE_MAIN2ND)
    #define BEFORE_MAIN2ND
#endif

#if !defined(BEFORE_MAIN3RD)
    #define BEFORE_MAIN3RD
#endif

#if !defined(BEFORE_SHADOW)
    #define BEFORE_SHADOW
#endif

#if !defined(BEFORE_RIMSHADE)
    #define BEFORE_RIMSHADE
#endif

#if !defined(BEFORE_BACKLIGHT)
    #define BEFORE_BACKLIGHT
#endif

#if !defined(BEFORE_REFRACTION)
    #define BEFORE_REFRACTION
#endif

#if !defined(BEFORE_REFLECTION)
    #define BEFORE_REFLECTION
#endif

#if !defined(BEFORE_MATCAP)
    #define BEFORE_MATCAP
#endif

#if !defined(BEFORE_MATCAP_2ND)
    #define BEFORE_MATCAP_2ND
#endif

#if !defined(BEFORE_RIMLIGHT)
    #define BEFORE_RIMLIGHT
#endif

#if !defined(BEFORE_GLITTER)
    #define BEFORE_GLITTER
#endif

#if !defined(BEFORE_EMISSION_1ST)
    #define BEFORE_EMISSION_1ST
#endif

#if !defined(BEFORE_EMISSION_2ND)
    #define BEFORE_EMISSION_2ND
#endif

#if !defined(BEFORE_DISSOLVE_ADD)
    #define BEFORE_DISSOLVE_ADD
#endif

#if !defined(BEFORE_BLEND_EMISSION)
    #define BEFORE_BLEND_EMISSION
#endif

#if !defined(BEFORE_DEPTH_FADE)
    #define BEFORE_DEPTH_FADE
#endif

#if !defined(BEFORE_DISTANCE_FADE)
    #define BEFORE_DISTANCE_FADE
#endif

#if !defined(BEFORE_FOG)
    #define BEFORE_FOG
#endif

#if !defined(BEFORE_OUTPUT)
    #define BEFORE_OUTPUT
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Override

//------------------------------------------------------------------------------------------------------------------------------
// Unpack v2f
#if defined(LIL_V2F_PACKED_TEXCOORD01)
    #define LIL_UNPACK_TEXCOORD0(i,o)   o.uv0 = i.uv01.xy;
#elif defined(LIL_V2F_TEXCOORD0)
    #define LIL_UNPACK_TEXCOORD0(i,o)   o.uv0 = i.uv0;
#else
    #define LIL_UNPACK_TEXCOORD0(i,o)
#endif

#if defined(LIL_V2F_PACKED_TEXCOORD01)
    #define LIL_UNPACK_TEXCOORD1(i,o)   o.uv1 = i.uv01.zw;
#elif defined(LIL_V2F_TEXCOORD1)
    #define LIL_UNPACK_TEXCOORD1(i,o)   o.uv1 = i.uv1;
#else
    #define LIL_UNPACK_TEXCOORD1(i,o)
#endif

#if defined(LIL_V2F_PACKED_TEXCOORD23)
    #define LIL_UNPACK_TEXCOORD2(i,o)   o.uv2 = i.uv23.xy;
#elif defined(LIL_V2F_TEXCOORD2)
    #define LIL_UNPACK_TEXCOORD2(i,o)   o.uv2 = i.uv2;
#else
    #define LIL_UNPACK_TEXCOORD2(i,o)
#endif

#if defined(LIL_V2F_PACKED_TEXCOORD23)
    #define LIL_UNPACK_TEXCOORD3(i,o)   o.uv3 = i.uv23.zw;
#elif defined(LIL_V2F_TEXCOORD3)
    #define LIL_UNPACK_TEXCOORD3(i,o)   o.uv3 = i.uv3;
#else
    #define LIL_UNPACK_TEXCOORD3(i,o)
#endif

#if defined(LIL_V2F_UVMAT)
    #define LIL_UNPACK_TEXCOORD_MAT(i,o) o.uvMat = i.uvMat;
#else
    #define LIL_UNPACK_TEXCOORD_MAT(i,o)
#endif

#if defined(LIL_V2F_POSITION_OS)
    #define LIL_UNPACK_POSITION_OS(i,o) \
        o.positionOS = i.positionOSdissolve.xyz; \
        o.dissolveActive = ((int)round(i.positionOSdissolve.w)) & 1; \
        o.dissolveInvert = ((int)round(i.positionOSdissolve.w)) & 2;
#else
    #define LIL_UNPACK_POSITION_OS(i,o)
#endif

#if defined(LIL_V2F_POSITION_WS)
    #define LIL_UNPACK_POSITION_WS(i,o) o.positionWS = lilToAbsolutePositionWS(i.positionWS);
#else
    #define LIL_UNPACK_POSITION_WS(i,o)
#endif

#if defined(LIL_V2F_POSITION_CS)
    #if defined(UNITY_SINGLE_PASS_STEREO) && !(defined(LIL_BRP) && !defined(LIL_LWTEX) && defined(LIL_REFRACTION))
        #define LIL_SCREEN_UV_STEREO_FIX(i,o) o.uvScn.x *= 0.5;
    #else
        #define LIL_SCREEN_UV_STEREO_FIX(i,o)
    #endif

    #if defined(LIL_BRP) && !defined(LIL_LWTEX) && defined(LIL_REFRACTION) && defined(LIL_REFRACTION_BLUR2)
        #define LIL_RES_XY lilGetWidthAndHeight(_GrabTexture)
    #elif defined(LIL_BRP) && !defined(LIL_LWTEX) && defined(LIL_REFRACTION)
        #define LIL_RES_XY lilGetWidthAndHeight(_lilBackgroundTexture)
    #else
        #define LIL_RES_XY (LIL_SCREENPARAMS.xy)
    #endif

    #define LIL_UNPACK_POSITION_CS(i,o) \
        o.positionCS = i.positionCS; \
        o.positionSS = lilTransformCStoSSFrag(i.positionCS); \
        o.uvScn = i.positionCS.xy / LIL_RES_XY; \
        LIL_SCREEN_UV_STEREO_FIX(i,o)
#else
    #define LIL_UNPACK_POSITION_CS(i,o)
#endif

#if defined(LIL_V2F_LIGHTDIRECTION) && !defined(LIL_HDRP)
    #define LIL_UNPACK_LIGHT_DIRECTION(i,o) \
        fd.L = input.lightDirection; \
        fd.origL = LIL_MAINLIGHT_DIRECTION;
#elif defined(LIL_V2F_LIGHTDIRECTION)
    #define LIL_UNPACK_LIGHT_DIRECTION(i,o) \
        fd.L = input.lightDirection; \
        fd.origL = input.lightDirection;
#else
    #define LIL_UNPACK_LIGHT_DIRECTION(i,o)
#endif

#if defined(LIL_V2F_INDLIGHTCOLOR)
    #define LIL_UNPACK_INDLIGHTCOLOR(i,o) LIL_GET_INDLIGHTCOLOR(i,o)
#else
    #define LIL_UNPACK_INDLIGHTCOLOR(i,o)
#endif

#define OVERRIDE_UNPACK_V2F \
    LIL_UNPACK_TEXCOORD0(input,fd); \
    LIL_UNPACK_TEXCOORD1(input,fd); \
    LIL_UNPACK_TEXCOORD2(input,fd); \
    LIL_UNPACK_TEXCOORD3(input,fd); \
    LIL_UNPACK_TEXCOORD_MAT(input,fd); \
    LIL_UNPACK_POSITION_OS(input,fd); \
    LIL_UNPACK_POSITION_WS(input,fd); \
    LIL_UNPACK_POSITION_CS(input,fd); \
    LIL_UNPACK_LIGHT_DIRECTION(input,fd); \
    LIL_UNPACK_INDLIGHTCOLOR(input,fd);

//------------------------------------------------------------------------------------------------------------------------------
// UV Animation
#if !defined(OVERRIDE_ANIMATE_MAIN_UV)
    #if defined(LIL_PASS_META_INCLUDED) && (defined(LIL_FEATURE_ANIMATE_MAIN_UV) || defined(LIL_LITE))
        #define OVERRIDE_ANIMATE_MAIN_UV \
            fd.uvMain = lilCalcDoubleSideUV(fd.uv0, fd.facing, _ShiftBackfaceUV); \
            fd.uvMain = lilCalcUVWithoutAnimation(fd.uvMain, _MainTex_ST, _MainTex_ScrollRotate);
    #elif defined(LIL_FEATURE_ANIMATE_MAIN_UV) || defined(LIL_LITE)
        #define OVERRIDE_ANIMATE_MAIN_UV \
            fd.uvMain = lilCalcDoubleSideUV(fd.uv0, fd.facing, _ShiftBackfaceUV); \
            fd.uvMain = lilCalcUV(fd.uvMain, _MainTex_ST, _MainTex_ScrollRotate);
    #elif !defined(LIL_PASS_FORWARD_FUR_INCLUDED)
        #define OVERRIDE_ANIMATE_MAIN_UV \
            fd.uvMain = lilCalcDoubleSideUV(fd.uv0, fd.facing, _ShiftBackfaceUV); \
            fd.uvMain = lilCalcUV(fd.uvMain, _MainTex_ST);
    #else
        #define OVERRIDE_ANIMATE_MAIN_UV \
            fd.uvMain = lilCalcUV(fd.uv0, _MainTex_ST);
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Outline UV Animation
#if !defined(OVERRIDE_ANIMATE_OUTLINE_UV)
    #if defined(LIL_FEATURE_ANIMATE_OUTLINE_UV) || defined(LIL_LITE)
        #define OVERRIDE_ANIMATE_OUTLINE_UV \
            fd.uvMain = lilCalcUV(fd.uv0, _OutlineTex_ST, _OutlineTex_ScrollRotate);
    #else
        #define OVERRIDE_ANIMATE_OUTLINE_UV \
            fd.uvMain = lilCalcUV(fd.uv0, _OutlineTex_ST);
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// DDX and DDY
#define OVERRIDE_CALC_DDX_DDY \
    fd.ddxMain = abs(ddx(fd.uvMain)); \
    fd.ddyMain = abs(ddy(fd.uvMain));

//------------------------------------------------------------------------------------------------------------------------------
// Parallax
#if !defined(OVERRIDE_PARALLAX)
    #if defined(LIL_MULTI) && defined(LIL_FEATURE_POM)
        #define OVERRIDE_PARALLAX \
            lilPOM(fd.uvMain, fd.uv0, _UseParallax, _MainTex_ST, fd.parallaxViewDirection, _ParallaxMap, _Parallax, _ParallaxOffset);
    #elif defined(LIL_FEATURE_POM)
        #define OVERRIDE_PARALLAX \
            if(_UsePOM) lilPOM(fd.uvMain, fd.uv0, _UseParallax, _MainTex_ST, fd.parallaxViewDirection, _ParallaxMap, _Parallax, _ParallaxOffset); \
            else        lilParallax(fd.uvMain, fd.uv0, _UseParallax, fd.parallaxOffset, _ParallaxMap, _Parallax, _ParallaxOffset);
    #else
        #define OVERRIDE_PARALLAX \
            lilParallax(fd.uvMain, fd.uv0, _UseParallax, fd.parallaxOffset, _ParallaxMap, _Parallax, _ParallaxOffset);
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Main Texture
#if defined(LIL_PASS_FORWARD_NORMAL_INCLUDED)
    #define LIL_GET_MAIN_TEX \
        fd.col = LIL_SAMPLE_2D_POM(_MainTex, sampler_MainTex, fd.uvMain, fd.ddxMain, fd.ddyMain);

    // Tone correction
    #if defined(LIL_FEATURE_MAIN_TONE_CORRECTION)
        #define LIL_MAIN_TONECORRECTION \
            fd.col.rgb = lilToneCorrection(fd.col.rgb, _MainTexHSVG);
    #else
        #define LIL_MAIN_TONECORRECTION
    #endif

    // Gradation map
    #if defined(LIL_FEATURE_MAIN_GRADATION_MAP) && defined(LIL_FEATURE_MainGradationTex)
        #define LIL_MAIN_GRADATION_MAP \
            fd.col.rgb = lilGradationMap(fd.col.rgb, _MainGradationTex, _MainGradationStrength);
    #else
        #define LIL_MAIN_GRADATION_MAP
    #endif

    #if defined(LIL_FEATURE_MainColorAdjustMask)
        #define LIL_SAMPLE_MainColorAdjustMask colorAdjustMask = LIL_SAMPLE_2D(_MainColorAdjustMask, sampler_MainTex, fd.uvMain).r
    #else
        #define LIL_SAMPLE_MainColorAdjustMask
    #endif

    #if defined(LIL_FEATURE_MAIN_TONE_CORRECTION) || defined(LIL_FEATURE_MAIN_GRADATION_MAP)
        #define LIL_APPLY_MAIN_TONECORRECTION \
            float3 beforeToneCorrectionColor = fd.col.rgb; \
            float colorAdjustMask = 1.0; \
            LIL_SAMPLE_MainColorAdjustMask; \
            LIL_MAIN_TONECORRECTION \
            LIL_MAIN_GRADATION_MAP \
            fd.col.rgb = lerp(beforeToneCorrectionColor, fd.col.rgb, colorAdjustMask);
    #else
        #define LIL_APPLY_MAIN_TONECORRECTION
    #endif
#else
    #define LIL_GET_MAIN_TEX \
        fd.col = LIL_SAMPLE_2D(_MainTex, sampler_MainTex, fd.uvMain);
    #define LIL_APPLY_MAIN_TONECORRECTION
#endif

#if !defined(OVERRIDE_MAIN)
    #define OVERRIDE_MAIN \
        LIL_GET_MAIN_TEX \
        LIL_APPLY_MAIN_TONECORRECTION \
        fd.col *= _Color;
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Outline Color
#if defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) && defined(LIL_FEATURE_OutlineTex) || !defined(LIL_PASS_FORWARD_NORMAL_INCLUDED)
    #define LIL_GET_OUTLINE_TEX \
        fd.col = LIL_SAMPLE_2D(_OutlineTex, sampler_OutlineTex, fd.uvMain);
#else
    #define LIL_GET_OUTLINE_TEX
#endif

#if defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) && defined(LIL_FEATURE_OutlineTex) && defined(LIL_FEATURE_OUTLINE_TONE_CORRECTION)
    #define LIL_APPLY_OUTLINE_TONECORRECTION \
        fd.col.rgb = lilToneCorrection(fd.col.rgb, _OutlineTexHSVG);
#else
    #define LIL_APPLY_OUTLINE_TONECORRECTION
#endif

#if defined(LIL_USE_SHADOW)
    #define LIL_APPLY_OUTLINE_LIT_SHADOW if(_OutlineLitShadowReceive) outlineLitFactor *= fd.attenuation;
#else
    #define LIL_APPLY_OUTLINE_LIT_SHADOW
#endif

#if defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) && defined(LIL_V2F_NDOTL)
    #define LIL_APPLY_OUTLINE_COLOR \
        float3 outlineLitColor = _OutlineLitApplyTex ? fd.col.rgb * _OutlineLitColor.rgb : _OutlineLitColor.rgb; \
        float outlineLitFactor = saturate(input.NdotL * _OutlineLitScale + _OutlineLitOffset) * _OutlineLitColor.a; \
        LIL_APPLY_OUTLINE_LIT_SHADOW \
        fd.col.rgb = lerp(fd.col.rgb * _OutlineColor.rgb, outlineLitColor, outlineLitFactor); \
        fd.col.a *= _OutlineColor.a;
#else
    #define LIL_APPLY_OUTLINE_COLOR fd.col *= _OutlineColor;
#endif

#if !defined(OVERRIDE_OUTLINE_COLOR)
    #define OVERRIDE_OUTLINE_COLOR \
        LIL_GET_OUTLINE_TEX \
        LIL_APPLY_OUTLINE_TONECORRECTION \
        LIL_APPLY_OUTLINE_COLOR
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Fur
#if LIL_RENDER == 1 || defined(LIL_FUR_PRE)
    #if defined(LIL_ONEPASS_FUR)
        #define LIL_FUR_LAYER_ALPHA \
            float furAlpha = furLayerShift < -1.5 ? 1.0 : saturate(furNoiseMask - furLayerShift * furLayerAbs * furLayerAbs * furLayerAbs + 0.25);
    #else
        #define LIL_FUR_LAYER_ALPHA \
            float furAlpha = saturate(furNoiseMask - furLayerShift * furLayerAbs * furLayerAbs * furLayerAbs + 0.25);
    #endif
    #define LIL_FUR_LAYER_AO \
        float furAO = _FurAO * saturate(1.0 - fwidth(input.furLayer)); \
        fd.col.rgb *= furLayer * furAO * 2.0 + 1.0 - furAO;
#else
    #if defined(LIL_ONEPASS_FUR)
        #define LIL_FUR_LAYER_ALPHA \
            float furAlpha = furLayerShift < -1.5 ? 1.0 : saturate(furNoiseMask - furLayerShift * furLayerAbs * furLayerAbs);
    #else
        #define LIL_FUR_LAYER_ALPHA \
            float furAlpha = saturate(furNoiseMask - furLayerShift * furLayerAbs * furLayerAbs);
    #endif
    #define LIL_FUR_LAYER_AO \
        fd.col.rgb *= saturate(1.0 - furNoiseMask + furNoiseMask * furLayer) * _FurAO * 1.25 + 1.0 - _FurAO;
#endif

#if defined(LIL_ALPHA_PS)
    #undef LIL_FUR_LAYER_AO
    #define LIL_FUR_LAYER_AO
#endif

#if defined(LIL_FEATURE_FurNoiseMask)
    #define LIL_SAMPLE_FurNoiseMask furNoiseMask = LIL_SAMPLE_2D_ST(_FurNoiseMask, sampler_MainTex, fd.uv0).r
#else
    #define LIL_SAMPLE_FurNoiseMask
#endif

#if defined(LIL_FEATURE_FurMask)
    #define LIL_SAMPLE_FurMask furAlpha *= LIL_SAMPLE_2D(_FurMask, sampler_MainTex, fd.uvMain).r
#else
    #define LIL_SAMPLE_FurMask
#endif

#if !defined(OVERRIDE_FUR)
    #define OVERRIDE_FUR \
        float furLayer = input.furLayer; \
        float furLayerShift = furLayer - furLayer * _FurRootOffset + _FurRootOffset; \
        float furLayerAbs = abs(furLayerShift); \
        float furNoiseMask = 1.0; \
        LIL_SAMPLE_FurNoiseMask; \
        LIL_FUR_LAYER_ALPHA \
        LIL_SAMPLE_FurMask; \
        fd.col.a *= furAlpha; \
        LIL_FUR_LAYER_AO
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Alpha Mask
#if !defined(OVERRIDE_ALPHAMASK)
    #if defined(LIL_FEATURE_AlphaMask)
        #define LIL_SAMPLE_AlphaMask alphaMask = LIL_SAMPLE_2D_ST(_AlphaMask, sampler_MainTex, fd.uvMain).r
    #else
        #define LIL_SAMPLE_AlphaMask
    #endif

    #define OVERRIDE_ALPHAMASK \
        if(_AlphaMaskMode) \
        { \
            float alphaMask = 1.0; \
            LIL_SAMPLE_AlphaMask; \
            alphaMask = saturate(alphaMask * _AlphaMaskScale + _AlphaMaskValue); \
            if(_AlphaMaskMode == 1) fd.col.a = alphaMask; \
            if(_AlphaMaskMode == 2) fd.col.a = fd.col.a * alphaMask; \
            if(_AlphaMaskMode == 3) fd.col.a = saturate(fd.col.a + alphaMask); \
            if(_AlphaMaskMode == 4) fd.col.a = saturate(fd.col.a - alphaMask); \
        }
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Dissolve
#if !defined(OVERRIDE_DISSOLVE)
    #if defined(LIL_FEATURE_DissolveMask)
        #define _DissolveMaskEnabled true
    #else
        #define _DissolveMaskEnabled false
    #endif

    #if defined(LIL_FEATURE_DissolveNoiseMask)
        #define OVERRIDE_DISSOLVE \
            lilCalcDissolveWithNoise( \
                fd.col.a, \
                dissolveAlpha, \
                fd.uv0, \
                fd.positionOS, \
                _DissolveParams, \
                _DissolvePos, \
                _DissolveMask, \
                _DissolveMask_ST, \
                _DissolveMaskEnabled, \
                _DissolveNoiseMask, \
                _DissolveNoiseMask_ST, \
                _DissolveNoiseMask_ScrollRotate, \
                _DissolveNoiseStrength \
                LIL_SAMP_IN(sampler_MainTex) \
            );
    #else
        #define OVERRIDE_DISSOLVE \
            lilCalcDissolve( \
                fd.col.a, \
                dissolveAlpha, \
                fd.uv0, \
                fd.positionOS, \
                _DissolveParams, \
                _DissolvePos, \
                _DissolveMask, \
                _DissolveMask_ST, \
                _DissolveMaskEnabled \
                LIL_SAMP_IN(sampler_MainTex) \
            );
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Dither
#if !defined(OVERRIDE_DITHER)
    #if !defined(SHADER_API_GLES)
        #if defined(LIL_FEATURE_DISTANCE_FADE) && defined(LIL_PASS_SHADOWCASTER_INCLUDED)
            #define OVERRIDE_DITHER \
                if(_UseDither == 1) \
                { \
                    if(LIL_MATRIX_P._m33 == 0.0) lilDistanceFadeAlphaOnly(fd); \
                    fd.col.a = fd.col.a >= (lilSamplePointRepeat(_DitherTex, input.positionCS.xy, _DitherTex_TexelSize.zw).r * 255 + 1) / (_DitherMaxValue+2); \
                }
        #elif defined(LIL_FEATURE_DISTANCE_FADE)
            #define OVERRIDE_DITHER \
                if(_UseDither == 1) \
                { \
                    lilDistanceFadeAlphaOnly(fd); \
                    fd.col.a = fd.col.a >= (lilSamplePointRepeat(_DitherTex, input.positionCS.xy, _DitherTex_TexelSize.zw).r * 255 + 1) / (_DitherMaxValue+2); \
                }
        #else
            #define OVERRIDE_DITHER \
                if(_UseDither == 1) \
                { \
                    fd.col.a = fd.col.a >= (lilSamplePointRepeat(_DitherTex, input.positionCS.xy, _DitherTex_TexelSize.zw).r * 255 + 1) / (_DitherMaxValue+2); \
                }
        #endif
    #else
        #define OVERRIDE_DITHER
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Premultiply
#if LIL_RENDER != 2
    #define LIL_PREMULTIPLY
#elif defined(LIL_PASS_FORWARDADD) && !defined(LIL_REFRACTION)
    #define LIL_PREMULTIPLY fd.col.rgb *= saturate(fd.col.a * _AlphaBoostFA);
#else
    #define LIL_PREMULTIPLY fd.col.rgb *= fd.col.a;
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Normal
#if !defined(OVERRIDE_NORMAL_1ST)
    #if defined(LIL_FEATURE_BumpMap)
        #define OVERRIDE_NORMAL_1ST \
            if(_UseBumpMap) \
            { \
                float4 normalTex = LIL_SAMPLE_2D_ST(_BumpMap, sampler_MainTex, fd.uvMain); \
                normalmap = lilUnpackNormalScale(normalTex, _BumpScale); \
            }
    #else
        #define OVERRIDE_NORMAL_1ST
    #endif
#endif

#if !defined(OVERRIDE_NORMAL_2ND)
    #if defined(LIL_FEATURE_Bump2ndScaleMask)
        #define LIL_SAMPLE_Bump2ndScaleMask bump2ndScale *= LIL_SAMPLE_2D_ST(_Bump2ndScaleMask, sampler_MainTex, fd.uvMain).r
    #else
        #define LIL_SAMPLE_Bump2ndScaleMask
    #endif

    #if defined(LIL_FEATURE_Bump2ndMap)
        #define OVERRIDE_NORMAL_2ND \
            if(_UseBump2ndMap) \
            { \
                float2 uvBump2nd = fd.uv0; \
                if(_Bump2ndMap_UVMode == 1) uvBump2nd = fd.uv1; \
                if(_Bump2ndMap_UVMode == 2) uvBump2nd = fd.uv2; \
                if(_Bump2ndMap_UVMode == 3) uvBump2nd = fd.uv3; \
                float4 normal2ndTex = LIL_SAMPLE_2D_ST(_Bump2ndMap, lil_sampler_linear_repeat, uvBump2nd); \
                float bump2ndScale = _Bump2ndScale; \
                LIL_SAMPLE_Bump2ndScaleMask; \
                normalmap = lilBlendNormal(normalmap, lilUnpackNormalScale(normal2ndTex, bump2ndScale)); \
            }
    #else
        #define OVERRIDE_NORMAL_2ND
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Anisotropy
#if !defined(OVERRIDE_ANISOTROPY)
    #if defined(LIL_FEATURE_AnisotropyTangentMap)
        #define LIL_SAMPLE_AnisotropyTangentMap anisoTangentMap = LIL_SAMPLE_2D_ST(_AnisotropyTangentMap, sampler_MainTex, fd.uvMain)
    #else
        #define LIL_SAMPLE_AnisotropyTangentMap
    #endif

    #if defined(LIL_FEATURE_AnisotropyScaleMask)
        #define LIL_SAMPLE_AnisotropyScaleMask fd.anisotropy *= LIL_SAMPLE_2D_ST(_AnisotropyScaleMask, sampler_MainTex, fd.uvMain).r
    #else
        #define LIL_SAMPLE_AnisotropyScaleMask
    #endif

    #define OVERRIDE_ANISOTROPY \
        if(_UseAnisotropy) \
        { \
            float4 anisoTangentMap = float4(0.5,0.5,1.0,0.5); \
            LIL_SAMPLE_AnisotropyTangentMap; \
            float3 anisoTangent = lilUnpackNormalScale(anisoTangentMap, 1.0); \
            fd.T = lilOrthoNormalize(normalize(mul(anisoTangent, fd.TBN)), fd.N); \
            fd.B = cross(fd.N, fd.T); \
            fd.anisotropy = _AnisotropyScale; \
            LIL_SAMPLE_AnisotropyScaleMask; \
            float3 anisoNormalWS = lilGetAnisotropyNormalWS(fd.N, fd.T, fd.B, fd.V, fd.anisotropy); \
            if(_Anisotropy2Reflection)  fd.reflectionN  = anisoNormalWS; \
            if(_Anisotropy2MatCap)      fd.matcapN      = anisoNormalWS; \
            if(_Anisotropy2MatCap2nd)   fd.matcap2ndN   = anisoNormalWS; \
            if(_Anisotropy2Reflection)  fd.perceptualRoughness = saturate(1.2 - abs(fd.anisotropy)); \
        }
#endif

//------------------------------------------------------------------------------------------------------------------------------
// AudioLink
#if defined(LIL_FEATURE_AUDIOLINK)
    void lilAudioLinkFrag(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        if(_UseAudioLink)
        {
            // UV
            float2 audioLinkUV;
            if(_AudioLinkUVMode == 0) audioLinkUV.x = _AudioLinkUVParams.g;
            if(_AudioLinkUVMode == 1) audioLinkUV.x = _AudioLinkUVParams.r - fd.nv * _AudioLinkUVParams.r + _AudioLinkUVParams.g;
            if(_AudioLinkUVMode == 2) audioLinkUV.x = lilRotateUV(fd.uv0, _AudioLinkUVParams.b).x * _AudioLinkUVParams.r + _AudioLinkUVParams.g;
            if(_AudioLinkUVMode == 5) audioLinkUV.x = distance(fd.positionOS, _AudioLinkStart.xyz) * _AudioLinkUVParams.r + _AudioLinkUVParams.g;
            audioLinkUV.y = _AudioLinkUVParams.a;

            // Mask (R:Delay G:Band B:Strength)
            // Spectrum Mask (R:Volume G:Band B:Strength)
            float4 audioLinkMask = 1.0;
            #if defined(LIL_FEATURE_AudioLinkMask)
                if(_AudioLinkUVMode == 3 || _AudioLinkUVMode == 4)
                {
                    float2 uvMask = fd.uvMain;
                    if(_AudioLinkMask_UVMode == 1) uvMask = fd.uv1;
                    if(_AudioLinkMask_UVMode == 2) uvMask = fd.uv2;
                    if(_AudioLinkMask_UVMode == 3) uvMask = fd.uv3;
                    uvMask = lilCalcUV(uvMask, _AudioLinkMask_ST, _AudioLinkMask_ScrollRotate);
                    audioLinkMask = LIL_SAMPLE_2D(_AudioLinkMask, sampler_AudioLinkMask, uvMask);
                    audioLinkUV = _AudioLinkUVMode == 3 ? audioLinkMask.rg : float2(frac(audioLinkMask.g * 2.0), 4.5/4.0 + floor(audioLinkMask.g * 2.0)/4.0);
                }
            #endif

            // Init value
            if(_AudioLinkUVMode == 4)
            {
                float defaultY = audioLinkMask.r * 4.0 + _AudioLinkDefaultValue.w;
                float defaultVal = sin(LIL_TIME * _AudioLinkDefaultValue.z - audioLinkMask.g * _AudioLinkDefaultValue.y) * _AudioLinkDefaultValue.x * _AudioLinkUVParams.x + _AudioLinkDefaultValue.x * _AudioLinkUVParams.x;
                fd.audioLinkValue = _AudioLinkUVParams.w < 1.0 ? abs(defaultVal - defaultY) < _AudioLinkUVParams.w : defaultVal > defaultY;
            }
            else
            {
                fd.audioLinkValue = saturate(_AudioLinkDefaultValue.x - saturate(frac(LIL_TIME * _AudioLinkDefaultValue.z - audioLinkUV.x)+_AudioLinkDefaultValue.w) * _AudioLinkDefaultValue.y * _AudioLinkDefaultValue.x);
            }

            // Local
            #if defined(LIL_FEATURE_AUDIOLINK_LOCAL) && defined(LIL_FEATURE_AudioLinkMask)
                if(_AudioLinkAsLocal)
                {
                    audioLinkUV.x += frac(-LIL_TIME * _AudioLinkLocalMapParams.r / 60 * _AudioLinkLocalMapParams.g) + _AudioLinkLocalMapParams.b;
                    fd.audioLinkValue = LIL_SAMPLE_2D(_AudioLinkLocalMap, lil_sampler_linear_repeat, audioLinkUV).r;
                }
                else
            #endif

            // Global
            if(lilCheckAudioLink())
            {
                // Scaling for _AudioTexture (4/64)
                audioLinkUV.y *= 0.0625;
                float4 audioTexture = LIL_SAMPLE_2D(_AudioTexture, lil_sampler_linear_clamp, audioLinkUV);
                if(_AudioLinkUVMode == 4)
                {
                    float audioVal = audioTexture.b * _AudioLinkUVParams.x * lerp(_AudioLinkUVParams.y, _AudioLinkUVParams.z, audioLinkMask.g);
                    fd.audioLinkValue = _AudioLinkUVParams.w < 1.0 ? abs(audioVal - audioLinkMask.r) < _AudioLinkUVParams.w : audioVal > audioLinkMask.r;
                }
                else
                {
                    fd.audioLinkValue = audioTexture.r;
                }
                fd.audioLinkValue = saturate(fd.audioLinkValue);
            }
            fd.audioLinkValue *= audioLinkMask.b;
        }
    }
#endif

#if !defined(OVERRIDE_AUDIOLINK)
    #define OVERRIDE_AUDIOLINK \
        lilAudioLinkFrag(fd LIL_SAMP_IN(sampler_MainTex));
#endif

//------------------------------------------------------------------------------------------------------------------------------
// UDIM Discard
#if !defined(OVERRIDE_UDIMDISCARD)
    #define OVERRIDE_UDIMDISCARD \
        if(_UDIMDiscardMode == 1 && LIL_CHECK_UDIMDISCARD(fd)) discard;
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Main 2nd
#if defined(LIL_FEATURE_MAIN2ND) && !defined(LIL_LITE)
    void lilGetMain2nd(inout lilFragData fd, inout float4 color2nd, inout float main2ndDissolveAlpha LIL_SAMP_IN_FUNC(samp))
    {
        #if !(defined(LIL_FEATURE_DECAL) && defined(LIL_FEATURE_ANIMATE_DECAL))
            float4 _Main2ndTexDecalAnimation = 0.0;
            float4 _Main2ndTexDecalSubParam = 0.0;
        #endif
        #if !defined(LIL_FEATURE_DECAL)
            bool _Main2ndTexIsDecal = false;
            bool _Main2ndTexIsLeftOnly = false;
            bool _Main2ndTexIsRightOnly = false;
            bool _Main2ndTexShouldCopy = false;
            bool _Main2ndTexShouldFlipMirror = false;
            bool _Main2ndTexShouldFlipCopy = false;
        #endif
        color2nd = _Color2nd;
        if(_UseMain2ndTex)
        {
            float2 uv2nd = fd.uv0;
            if(_Main2ndTex_UVMode == 1) uv2nd = fd.uv1;
            if(_Main2ndTex_UVMode == 2) uv2nd = fd.uv2;
            if(_Main2ndTex_UVMode == 3) uv2nd = fd.uv3;
            if(_Main2ndTex_UVMode == 4) uv2nd = fd.uvMat;
            #if defined(LIL_FEATURE_Main2ndTex)
                color2nd *= LIL_GET_SUBTEX(_Main2ndTex, uv2nd);
            #endif
            #if defined(LIL_FEATURE_Main2ndBlendMask)
                color2nd.a *= LIL_SAMPLE_2D(_Main2ndBlendMask, samp, fd.uvMain).r;
            #endif

            #if defined(LIL_FEATURE_Main2ndDissolveMask)
                #define _Main2ndDissolveMaskEnabled true
            #else
                #define _Main2ndDissolveMaskEnabled false
            #endif

            #if defined(LIL_FEATURE_LAYER_DISSOLVE)
                #if defined(LIL_FEATURE_Main2ndDissolveNoiseMask)
                    lilCalcDissolveWithNoise(
                        color2nd.a,
                        main2ndDissolveAlpha,
                        fd.uv0,
                        fd.positionOS,
                        _Main2ndDissolveParams,
                        _Main2ndDissolvePos,
                        _Main2ndDissolveMask,
                        _Main2ndDissolveMask_ST,
                        _Main2ndDissolveMaskEnabled,
                        _Main2ndDissolveNoiseMask,
                        _Main2ndDissolveNoiseMask_ST,
                        _Main2ndDissolveNoiseMask_ScrollRotate,
                        _Main2ndDissolveNoiseStrength,
                        samp
                    );
                #else
                    lilCalcDissolve(
                        color2nd.a,
                        main2ndDissolveAlpha,
                        fd.uv0,
                        fd.positionOS,
                        _Main2ndDissolveParams,
                        _Main2ndDissolvePos,
                        _Main2ndDissolveMask,
                        _Main2ndDissolveMask_ST,
                        _Main2ndDissolveMaskEnabled,
                        samp
                    );
                #endif
            #endif
            #if defined(LIL_FEATURE_AUDIOLINK)
                if(_AudioLink2Main2nd) color2nd.a *= fd.audioLinkValue;
            #endif
            color2nd.a = lerp(color2nd.a, color2nd.a * saturate((fd.depth - _Main2ndDistanceFade.x) / (_Main2ndDistanceFade.y - _Main2ndDistanceFade.x)), _Main2ndDistanceFade.z);
            if(_Main2ndTex_Cull == 1 && fd.facing > 0 || _Main2ndTex_Cull == 2 && fd.facing < 0) color2nd.a = 0;
            #if LIL_RENDER != 0
                if(_Main2ndTexAlphaMode != 0)
                {
                    if(_Main2ndTexAlphaMode == 1) fd.col.a = color2nd.a;
                    if(_Main2ndTexAlphaMode == 2) fd.col.a = fd.col.a * color2nd.a;
                    if(_Main2ndTexAlphaMode == 3) fd.col.a = saturate(fd.col.a + color2nd.a);
                    if(_Main2ndTexAlphaMode == 4) fd.col.a = saturate(fd.col.a - color2nd.a);
                    color2nd.a = 1;
                }
            #endif
            fd.col.rgb = lilBlendColor(fd.col.rgb, color2nd.rgb, color2nd.a * _Main2ndEnableLighting, _Main2ndTexBlendMode);
        }
    }
#endif

#if !defined(OVERRIDE_MAIN2ND)
    #define OVERRIDE_MAIN2ND \
        lilGetMain2nd(fd, color2nd, main2ndDissolveAlpha LIL_SAMP_IN(sampler_MainTex));
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Main 3rd
#if defined(LIL_FEATURE_MAIN3RD) && !defined(LIL_LITE)
    void lilGetMain3rd(inout lilFragData fd, inout float4 color3rd, inout float main3rdDissolveAlpha LIL_SAMP_IN_FUNC(samp))
    {
        #if !(defined(LIL_FEATURE_DECAL) && defined(LIL_FEATURE_ANIMATE_DECAL))
            float4 _Main3rdTexDecalAnimation = 0.0;
            float4 _Main3rdTexDecalSubParam = 0.0;
        #endif
        #if !defined(LIL_FEATURE_DECAL)
            bool _Main3rdTexIsDecal = false;
            bool _Main3rdTexIsLeftOnly = false;
            bool _Main3rdTexIsRightOnly = false;
            bool _Main3rdTexShouldCopy = false;
            bool _Main3rdTexShouldFlipMirror = false;
            bool _Main3rdTexShouldFlipCopy = false;
        #endif
        color3rd = _Color3rd;
        if(_UseMain3rdTex)
        {
            float2 uv3rd = fd.uv0;
            if(_Main3rdTex_UVMode == 1) uv3rd = fd.uv1;
            if(_Main3rdTex_UVMode == 2) uv3rd = fd.uv2;
            if(_Main3rdTex_UVMode == 3) uv3rd = fd.uv3;
            if(_Main3rdTex_UVMode == 4) uv3rd = fd.uvMat;
            #if defined(LIL_FEATURE_Main3rdTex)
                color3rd *= LIL_GET_SUBTEX(_Main3rdTex, uv3rd);
            #endif
            #if defined(LIL_FEATURE_Main3rdBlendMask)
                color3rd.a *= LIL_SAMPLE_2D(_Main3rdBlendMask, samp, fd.uvMain).r;
            #endif

            #if defined(LIL_FEATURE_Main3rdDissolveMask)
                #define _Main3rdDissolveMaskEnabled true
            #else
                #define _Main3rdDissolveMaskEnabled false
            #endif

            #if defined(LIL_FEATURE_LAYER_DISSOLVE)
                #if defined(LIL_FEATURE_Main3rdDissolveNoiseMask)
                    lilCalcDissolveWithNoise(
                        color3rd.a,
                        main3rdDissolveAlpha,
                        fd.uv0,
                        fd.positionOS,
                        _Main3rdDissolveParams,
                        _Main3rdDissolvePos,
                        _Main3rdDissolveMask,
                        _Main3rdDissolveMask_ST,
                        _Main3rdDissolveMaskEnabled,
                        _Main3rdDissolveNoiseMask,
                        _Main3rdDissolveNoiseMask_ST,
                        _Main3rdDissolveNoiseMask_ScrollRotate,
                        _Main3rdDissolveNoiseStrength,
                        samp
                    );
                #else
                    lilCalcDissolve(
                        color3rd.a,
                        main3rdDissolveAlpha,
                        fd.uv0,
                        fd.positionOS,
                        _Main3rdDissolveParams,
                        _Main3rdDissolvePos,
                        _Main3rdDissolveMask,
                        _Main3rdDissolveMask_ST,
                        _Main3rdDissolveMaskEnabled,
                        samp
                    );
                #endif
            #endif
            #if defined(LIL_FEATURE_AUDIOLINK)
                if(_AudioLink2Main3rd) color3rd.a *= fd.audioLinkValue;
            #endif
            color3rd.a = lerp(color3rd.a, color3rd.a * saturate((fd.depth - _Main3rdDistanceFade.x) / (_Main3rdDistanceFade.y - _Main3rdDistanceFade.x)), _Main3rdDistanceFade.z);
            if(_Main3rdTex_Cull == 1 && fd.facing > 0 || _Main3rdTex_Cull == 2 && fd.facing < 0) color3rd.a = 0;
            #if LIL_RENDER != 0
                if(_Main3rdTexAlphaMode != 0)
                {
                    if(_Main3rdTexAlphaMode == 1) fd.col.a = color3rd.a;
                    if(_Main3rdTexAlphaMode == 2) fd.col.a = fd.col.a * color3rd.a;
                    if(_Main3rdTexAlphaMode == 3) fd.col.a = saturate(fd.col.a + color3rd.a);
                    if(_Main3rdTexAlphaMode == 4) fd.col.a = saturate(fd.col.a - color3rd.a);
                    color3rd.a = 1;
                }
            #endif
            fd.col.rgb = lilBlendColor(fd.col.rgb, color3rd.rgb, color3rd.a * _Main3rdEnableLighting, _Main3rdTexBlendMode);
        }
    }
#endif

#if !defined(OVERRIDE_MAIN3RD)
    #define OVERRIDE_MAIN3RD \
        lilGetMain3rd(fd, color3rd, main3rdDissolveAlpha LIL_SAMP_IN(sampler_MainTex));
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Shadow
#if defined(LIL_FEATURE_SHADOW) && !defined(LIL_LITE) && !defined(LIL_GEM)
    void lilGetShading(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        if(_UseShadow)
        {
            // Normal
            float3 N1 = fd.N;
            float3 N2 = fd.N;
            #if defined(LIL_FEATURE_SHADOW_3RD)
                float3 N3 = fd.N;
            #endif
            #if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)
                N1 = lerp(fd.origN, fd.N, _ShadowNormalStrength);
                N2 = lerp(fd.origN, fd.N, _Shadow2ndNormalStrength);
                #if defined(LIL_FEATURE_SHADOW_3RD)
                    N3 = lerp(fd.origN, fd.N, _Shadow3rdNormalStrength);
                #endif
            #endif

            // Shade
            float4 lns = 1.0;
            lns.x = saturate(dot(fd.L,N1)*0.5+0.5);
            lns.y = saturate(dot(fd.L,N2)*0.5+0.5);
            #if defined(LIL_FEATURE_SHADOW_3RD)
                lns.z = saturate(dot(fd.L,N3)*0.5+0.5);
            #endif

            // Shadow
            #if (defined(LIL_USE_SHADOW) || defined(LIL_LIGHTMODE_SHADOWMASK)) && defined(LIL_FEATURE_RECEIVE_SHADOW)
                float calculatedShadow = saturate(fd.attenuation + distance(fd.L, fd.origL));
                lns.x *= lerp(1.0, calculatedShadow, _ShadowReceive);
                lns.y *= lerp(1.0, calculatedShadow, _Shadow2ndReceive);
                #if defined(LIL_FEATURE_SHADOW_3RD)
                    lns.z *= lerp(1.0, calculatedShadow, _Shadow3rdReceive);
                #endif
            #endif

            // Blur Scale
            float shadowBlur = _ShadowBlur;
            float shadow2ndBlur = _Shadow2ndBlur;
            #if defined(LIL_FEATURE_SHADOW_3RD)
                float shadow3rdBlur = _Shadow3rdBlur;
            #endif
            #if defined(LIL_FEATURE_ShadowBlurMask)
                #if defined(_ShadowBlurMaskLOD)
                    float4 shadowBlurMask = LIL_SAMPLE_2D(_ShadowBlurMask, lil_sampler_linear_repeat, fd.uvMain);
                    if(_ShadowBlurMaskLOD) shadowBlurMask = LIL_SAMPLE_2D_GRAD(_ShadowBlurMask, lil_sampler_linear_repeat, fd.uvMain, max(fd.ddxMain, _ShadowBlurMaskLOD), max(fd.ddyMain, _ShadowBlurMaskLOD));
                #else
                    float4 shadowBlurMask = LIL_SAMPLE_2D_GRAD(_ShadowBlurMask, lil_sampler_linear_repeat, fd.uvMain, max(fd.ddxMain, _ShadowBlurMaskLOD), max(fd.ddyMain, _ShadowBlurMaskLOD));
                #endif
                shadowBlur *= shadowBlurMask.r;
                shadow2ndBlur *= shadowBlurMask.g;
                #if defined(LIL_FEATURE_SHADOW_3RD)
                    shadow3rdBlur *= shadowBlurMask.b;
                #endif
            #endif

            // AO Map & Toon
            #if defined(LIL_FEATURE_ShadowBorderMask)
                #if defined(_ShadowBorderMaskLOD)
                    float4 shadowBorderMask = LIL_SAMPLE_2D(_ShadowBorderMask, lil_sampler_linear_repeat, fd.uvMain);
                    if(_ShadowBorderMaskLOD) shadowBorderMask = LIL_SAMPLE_2D_GRAD(_ShadowBorderMask, lil_sampler_linear_repeat, fd.uvMain, max(fd.ddxMain, _ShadowBorderMaskLOD), max(fd.ddyMain, _ShadowBorderMaskLOD));
                #else
                    float4 shadowBorderMask = LIL_SAMPLE_2D_GRAD(_ShadowBorderMask, lil_sampler_linear_repeat, fd.uvMain, max(fd.ddxMain, _ShadowBorderMaskLOD), max(fd.ddyMain, _ShadowBorderMaskLOD));
                #endif
                shadowBorderMask.r = saturate(shadowBorderMask.r * _ShadowAOShift.x + _ShadowAOShift.y);
                shadowBorderMask.g = saturate(shadowBorderMask.g * _ShadowAOShift.z + _ShadowAOShift.w);
                #if defined(LIL_FEATURE_SHADOW_3RD)
                    shadowBorderMask.b = saturate(shadowBorderMask.b * _ShadowAOShift2.x + _ShadowAOShift2.y);
                #endif
                lns.xyz = _ShadowPostAO ? lns.xyz : lns.xyz * shadowBorderMask.rgb;

                lns.w = lns.x;
                lns.x = lilTooningNoSaturateScale(_AAStrength, lns.x, _ShadowBorder, shadowBlur);
                lns.y = lilTooningNoSaturateScale(_AAStrength, lns.y, _Shadow2ndBorder, shadow2ndBlur);
                lns.w = lilTooningNoSaturateScale(_AAStrength, lns.w, _ShadowBorder, shadowBlur, _ShadowBorderRange);
                #if defined(LIL_FEATURE_SHADOW_3RD)
                    lns.z = lilTooningNoSaturateScale(_AAStrength, lns.z, _Shadow3rdBorder, shadow3rdBlur);
                #endif
                lns = _ShadowPostAO ? lns * shadowBorderMask.rgbr : lns;
                lns = saturate(lns);
            #else
                lns.w = lns.x;
                lns.x = lilTooningScale(_AAStrength, lns.x, _ShadowBorder, shadowBlur);
                lns.y = lilTooningScale(_AAStrength, lns.y, _Shadow2ndBorder, shadow2ndBlur);
                lns.w = lilTooningScale(_AAStrength, lns.w, _ShadowBorder, shadowBlur, _ShadowBorderRange);
                #if defined(LIL_FEATURE_SHADOW_3RD)
                    lns.z = lilTooningScale(_AAStrength, lns.z, _Shadow3rdBorder, shadow3rdBlur);
                #endif
            #endif

            // Force shadow on back face
            float bfshadow = (fd.facing < 0.0) ? 1.0 - _BackfaceForceShadow : 1.0;
            lns.x *= bfshadow;
            lns.y *= bfshadow;
            lns.w *= bfshadow;
            #if defined(LIL_FEATURE_SHADOW_3RD)
                lns.z *= bfshadow;
            #endif

            // Copy
            fd.shadowmix = lns.x;

            // Strength
            float shadowStrength = _ShadowStrength;
            #ifdef LIL_COLORSPACE_GAMMA
                shadowStrength = lilSRGBToLinear(shadowStrength);
            #endif
            float shadowStrengthMask = 1;
            #if defined(LIL_FEATURE_ShadowStrengthMask)
                #if defined(_ShadowStrengthMaskLOD)
                    shadowStrengthMask = LIL_SAMPLE_2D(_ShadowStrengthMask, lil_sampler_linear_repeat, fd.uvMain).r;
                    if(_ShadowStrengthMaskLOD) shadowStrengthMask = LIL_SAMPLE_2D_GRAD(_ShadowStrengthMask, lil_sampler_linear_repeat, fd.uvMain, max(fd.ddxMain, _ShadowStrengthMaskLOD), max(fd.ddyMain, _ShadowStrengthMaskLOD)).r;
                #else
                    shadowStrengthMask = LIL_SAMPLE_2D_GRAD(_ShadowStrengthMask, lil_sampler_linear_repeat, fd.uvMain, max(fd.ddxMain, _ShadowStrengthMaskLOD), max(fd.ddyMain, _ShadowStrengthMaskLOD)).r;
                #endif
            #endif
            if(_ShadowMaskType)
            {
                float3 flatN = normalize(mul((float3x3)LIL_MATRIX_M, float3(0.0,0.25,1.0)));//normalize(LIL_MATRIX_M._m02_m12_m22);
                float lnFlat = saturate((dot(flatN, fd.L) + _ShadowFlatBorder) / _ShadowFlatBlur);
                #if (defined(LIL_USE_SHADOW) || defined(LIL_LIGHTMODE_SHADOWMASK)) && defined(LIL_FEATURE_RECEIVE_SHADOW)
                    lnFlat *= lerp(1.0, calculatedShadow, _ShadowReceive);
                #endif
                lns = lerp(lnFlat, lns, shadowStrengthMask);
            }
            else
            {
                shadowStrength *= shadowStrengthMask;
            }
            lns.x = lerp(1.0, lns.x, shadowStrength);

            // Shadow Colors
            float4 shadowColorTex = 0.0;
            float4 shadow2ndColorTex = 0.0;
            float4 shadow3rdColorTex = 0.0;
            #if defined(LIL_FEATURE_SHADOW_LUT)
                if(_ShadowColorType == 1)
                {
                    float4 uvShadow;
                    float factor;
                    lilCalcLUTUV(fd.albedo, 16, 1, uvShadow, factor);
                    #if defined(LIL_FEATURE_ShadowColorTex)
                        shadowColorTex = lilSampleLUT(uvShadow, factor, _ShadowColorTex);
                    #endif
                    #if defined(LIL_FEATURE_Shadow2ndColorTex)
                        shadow2ndColorTex = lilSampleLUT(uvShadow, factor, _Shadow2ndColorTex);
                    #endif
                    #if defined(LIL_FEATURE_SHADOW_3RD) && defined(LIL_FEATURE_Shadow3rdColorTex)
                        shadow3rdColorTex = lilSampleLUT(uvShadow, factor, _Shadow3rdColorTex);
                    #endif
                }
                else
            #endif
            {
                #if defined(LIL_FEATURE_ShadowColorTex)
                    shadowColorTex = LIL_SAMPLE_2D(_ShadowColorTex, samp, fd.uvMain);
                #endif
                #if defined(LIL_FEATURE_Shadow2ndColorTex)
                    shadow2ndColorTex = LIL_SAMPLE_2D(_Shadow2ndColorTex, samp, fd.uvMain);
                #endif
                #if defined(LIL_FEATURE_SHADOW_3RD) && defined(LIL_FEATURE_Shadow3rdColorTex)
                    shadow3rdColorTex = LIL_SAMPLE_2D(_Shadow3rdColorTex, samp, fd.uvMain);
                #endif
            }

            // Shadow Color 1
            float3 indirectCol = lerp(fd.albedo, shadowColorTex.rgb, shadowColorTex.a) * _ShadowColor.rgb;

            // Shadow Color 2
            shadow2ndColorTex.rgb = lerp(fd.albedo, shadow2ndColorTex.rgb, shadow2ndColorTex.a) * _Shadow2ndColor.rgb;
            lns.y = _Shadow2ndColor.a - lns.y * _Shadow2ndColor.a;
            indirectCol = lerp(indirectCol, shadow2ndColorTex.rgb, lns.y);

            #if defined(LIL_FEATURE_SHADOW_3RD)
                // Shadow Color 3
                shadow3rdColorTex.rgb = lerp(fd.albedo, shadow3rdColorTex.rgb, shadow3rdColorTex.a) * _Shadow3rdColor.rgb;
                lns.z = _Shadow3rdColor.a - lns.z * _Shadow3rdColor.a;
                indirectCol = lerp(indirectCol, shadow3rdColorTex.rgb, lns.z);
            #endif

            // Multiply Main Color
            indirectCol = lerp(indirectCol, indirectCol*fd.albedo, _ShadowMainStrength);

            // Apply Light
            float3 directCol = fd.albedo * fd.lightColor;
            indirectCol = indirectCol * fd.lightColor;

            #if !defined(LIL_PASS_FORWARDADD)
                // Environment Light
                indirectCol = lerp(indirectCol, fd.albedo, fd.indLightColor);
            #endif
            // Fix
            indirectCol = min(indirectCol, directCol);
            // Gradation
            indirectCol = lerp(indirectCol, directCol, lns.w * _ShadowBorderColor.rgb);

            // Mix
            fd.col.rgb = lerp(indirectCol, directCol, lns.x);
        }
        else
        {
            fd.col.rgb *= fd.lightColor;
        }
    }
#elif defined(LIL_LITE)
    void lilGetShading(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        if(_UseShadow)
        {
            // Shade
            float ln1 = saturate(fd.ln*0.5+0.5);
            float ln2 = ln1;
            float lnB = ln1;

            // Toon
            ln1 = lilTooningScale(_AAStrength, ln1, _ShadowBorder, _ShadowBlur);
            ln2 = lilTooningScale(_AAStrength, ln2, _Shadow2ndBorder, _Shadow2ndBlur);
            lnB = lilTooningScale(_AAStrength, lnB, _ShadowBorder, _ShadowBlur, _ShadowBorderRange);

            // Force shadow on back face
            float bfshadow = (fd.facing < 0.0) ? 1.0 - _BackfaceForceShadow : 1.0;
            ln1 *= bfshadow;
            ln2 *= bfshadow;
            lnB *= bfshadow;

            // Copy
            fd.shadowmix = ln1;

            // Shadow Color 1
            float4 shadowColorTex = LIL_SAMPLE_2D(_ShadowColorTex, samp, fd.uvMain);
            float3 indirectCol = lerp(fd.albedo, shadowColorTex.rgb, shadowColorTex.a);
            // Shadow Color 2
            float4 shadow2ndColorTex = LIL_SAMPLE_2D(_Shadow2ndColorTex, samp, fd.uvMain);
            indirectCol = lerp(indirectCol, shadow2ndColorTex.rgb, shadow2ndColorTex.a - ln2 * shadow2ndColorTex.a);

            // Apply Light
            float3 directCol = fd.albedo * fd.lightColor;
            indirectCol = indirectCol * fd.lightColor;

            // Environment Light
            indirectCol = lerp(indirectCol, fd.albedo, fd.indLightColor);
            // Fix
            indirectCol = min(indirectCol, directCol);
            // Gradation
            indirectCol = lerp(indirectCol, directCol, lnB * _ShadowBorderColor.rgb);

            // Mix
            fd.col.rgb = lerp(indirectCol, directCol, ln1);
        }
        else
        {
            fd.col.rgb *= fd.lightColor;
        }
    }
#endif

#if !defined(OVERRIDE_SHADOW)
    #define OVERRIDE_SHADOW \
        lilGetShading(fd LIL_SAMP_IN(sampler_MainTex));
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Rim Shade
#if defined(LIL_FEATURE_RIMSHADE)
    void lilGetRimShade(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        if(_UseRimShade)
        {
            float3 N = fd.N;
            #if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)
                N = lerp(fd.origN, fd.N, _RimShadeNormalStrength);
            #endif
            float nvabs = abs(dot(N,fd.headV));
            float rim = pow(saturate(1.0 - nvabs), _RimShadeFresnelPower);
            rim = lilTooningScale(_AAStrength, rim, _RimShadeBorder, _RimShadeBlur);
            rim *= _RimShadeColor.a;
            #if defined(LIL_FEATURE_ShadowColorTex)
                rim *= LIL_SAMPLE_2D(_RimShadeMask, samp, fd.uvMain).r;
            #endif
            fd.col.rgb = lerp(fd.col.rgb, fd.col.rgb * _RimShadeColor.rgb, rim);
        }
    }
#endif

#if !defined(OVERRIDE_RIMSHADE)
    #if defined(LIL_LITE)
        #define OVERRIDE_RIMSHADE \
            lilGetRimShade(fd);
    #else
        #define OVERRIDE_RIMSHADE \
            lilGetRimShade(fd LIL_SAMP_IN(sampler_MainTex));
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Backlight
#if defined(LIL_FEATURE_BACKLIGHT) && !defined(LIL_LITE) && !defined(LIL_GEM)
    void lilBacklight(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        if(_UseBacklight)
        {
            // Normal
            float3 N = fd.N;
            #if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)
                N = lerp(fd.origN, fd.N, _BacklightNormalStrength);
            #endif

            // Color
            float3 backlightColor = _BacklightColor.rgb;
            #if defined(LIL_FEATURE_BacklightColorTex)
                backlightColor *= LIL_SAMPLE_2D_ST(_BacklightColorTex, samp, fd.uvMain).rgb;
            #endif

            // Factor
            float backlightFactor = pow(saturate(-fd.hl * 0.5 + 0.5), _BacklightDirectivity);
            float backlightLN = dot(normalize(-fd.headV * _BacklightViewStrength + fd.L), N) * 0.5 + 0.5;
            #if defined(LIL_USE_SHADOW) || defined(LIL_LIGHTMODE_SHADOWMASK)
                if(_BacklightReceiveShadow) backlightLN *= saturate(fd.attenuation + distance(fd.L, fd.origL));
            #endif
            backlightLN = lilTooningScale(_AAStrength, backlightLN, _BacklightBorder, _BacklightBlur);
            float backlight = saturate(backlightFactor * backlightLN);
            backlight = fd.facing < (_BacklightBackfaceMask-1.0) ? 0.0 : backlight;

            // Blend
            backlightColor = lerp(backlightColor, backlightColor * fd.albedo, _BacklightMainStrength);
            fd.col.rgb += backlight * backlightColor * fd.lightColor;
        }
    }
#endif

#if !defined(OVERRIDE_BACKLIGHT)
    #define OVERRIDE_BACKLIGHT lilBacklight(fd LIL_SAMP_IN(sampler_MainTex));
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Refraction
#if defined(LIL_REFRACTION) && !defined(LIL_LITE)
    void lilRefraction(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        float2 refractUV = fd.uvScn + (pow(1.0 - fd.nv, _RefractionFresnelPower) * _RefractionStrength) * mul((float3x3)LIL_MATRIX_V, fd.N).xy;
        #if defined(LIL_REFRACTION_BLUR2)
            #if defined(LIL_BRP)
                float3 refractCol = 0;
                float sum = 0;
                float blurOffset = fd.perceptualRoughness / sqrt(fd.positionSS.w) * (0.03 / LIL_REFRACTION_SAMPNUM) * LIL_MATRIX_P._m11;
                for(int j = -16; j <= 16; j++)
                {
                    refractCol += LIL_GET_GRAB_TEX(refractUV + float2(0,j*blurOffset), 0).rgb * LIL_REFRACTION_GAUSDIST(j);
                    sum += LIL_REFRACTION_GAUSDIST(j);
                }
                refractCol /= sum;
                refractCol *= _RefractionColor.rgb;
            #else
                float refractLod = min(sqrt(fd.perceptualRoughness / sqrt(fd.positionSS.w) * 5.0), 10);
                float3 refractCol = LIL_GET_GRAB_TEX(refractUV, refractLod).rgb * _RefractionColor.rgb;
            #endif
        #else
            float3 refractCol = LIL_GET_BG_TEX(refractUV,0).rgb * _RefractionColor.rgb;
        #endif
        if(_RefractionColorFromMain) refractCol *= fd.albedo;
        fd.col.rgb = lerp(refractCol, fd.col.rgb, fd.col.a);
    }
#endif

#if !defined(OVERRIDE_REFRACTION)
    #define OVERRIDE_REFRACTION \
        lilRefraction(fd LIL_SAMP_IN(sampler_MainTex));
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Reflection
#if defined(LIL_HDRP)
    #define LIL_HDRP_POSITION_INPUT_VAR , posInput
    #define LIL_HDRP_POSITION_INPUT_ARGS , PositionInputs posInput
#else
    #define LIL_HDRP_POSITION_INPUT_VAR
    #define LIL_HDRP_POSITION_INPUT_ARGS
#endif
#if defined(LIL_FEATURE_REFLECTION) && defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) && !defined(LIL_LITE)
    float3 lilCalcSpecular(inout lilFragData fd, float3 L, float3 specular, float attenuation LIL_SAMP_IN_FUNC(samp))
    {
        // Normal
        float3 N = fd.N;
        #if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)
            N = lerp(fd.origN, fd.N, _SpecularNormalStrength);
        #endif

        // Half direction
        float3 H = normalize(fd.V + L);
        float nh = saturate(dot(N, H));

        // Toon
        #if defined(LIL_FEATURE_ANISOTROPY)
            bool isAnisotropy = _UseAnisotropy && _Anisotropy2Reflection;
            if(_SpecularToon & !isAnisotropy)
        #else
            if(_SpecularToon)
        #endif
        return lilTooningScale(_AAStrength, pow(nh,1.0/fd.roughness), _SpecularBorder, _SpecularBlur);

        // Dot
        float nv = saturate(dot(N, fd.V));
        float nl = saturate(dot(N, L));
        float lh = saturate(dot(L, H));

        // GGX
        float ggx, sjggx = 0.0;
        float lambdaV = 0.0;
        float lambdaL = 0.0;
        float d = 1.0;
        #if defined(LIL_FEATURE_ANISOTROPY)
            if(isAnisotropy)
            {

                float roughnessT = max(fd.roughness * (1.0 + fd.anisotropy), 0.002);
                float roughnessB = max(fd.roughness * (1.0 - fd.anisotropy), 0.002);

                float tv = dot(fd.T, fd.V);
                float bv = dot(fd.B, fd.V);
                float tl = dot(fd.T, L);
                float bl = dot(fd.B, L);

                lambdaV = nl * length(float3(roughnessT * tv, roughnessB * bv, nv));
                lambdaL = nv * length(float3(roughnessT * tl, roughnessB * bl, nl));

                float roughnessT1 = roughnessT * _AnisotropyTangentWidth;
                float roughnessB1 = roughnessB * _AnisotropyBitangentWidth;
                float roughnessT2 = roughnessT * _Anisotropy2ndTangentWidth;
                float roughnessB2 = roughnessB * _Anisotropy2ndBitangentWidth;

                float anisotropyShiftNoise = 0.5;
                #if defined(LIL_FEATURE_AnisotropyShiftNoiseMask)
                    anisotropyShiftNoise = LIL_SAMPLE_2D_ST(_AnisotropyShiftNoiseMask, samp, fd.uvMain).r - 0.5;
                #endif
                float anisotropyShift = anisotropyShiftNoise * _AnisotropyShiftNoiseScale + _AnisotropyShift;
                float anisotropy2ndShift = anisotropyShiftNoise * _Anisotropy2ndShiftNoiseScale + _Anisotropy2ndShift;
                float3 T1 = normalize(fd.T - N * anisotropyShift);
                float3 B1 = normalize(fd.B - N * anisotropyShift);
                float3 T2 = normalize(fd.T - N * anisotropy2ndShift);
                float3 B2 = normalize(fd.B - N * anisotropy2ndShift);

                float th1 = dot(T1, H);
                float bh1 = dot(B1, H);
                float th2 = dot(T2, H);
                float bh2 = dot(B2, H);

                float r1 = roughnessT1 * roughnessB1;
                float r2 = roughnessT2 * roughnessB2;
                float3 v1 = float3(th1 * roughnessB1, bh1 * roughnessT1, nh * r1);
                float3 v2 = float3(th2 * roughnessB2, bh2 * roughnessT2, nh * r2);
                float w1 = r1 / dot(v1, v1);
                float w2 = r2 / dot(v2, v2);
                ggx = r1 * w1 * w1 * _AnisotropySpecularStrength + r2 * w2 * w2 * _Anisotropy2ndSpecularStrength;
            }
            else
        #endif
        {
            float roughness2 = max(fd.roughness, 0.002);
            lambdaV = nl * (nv * (1.0 - roughness2) + roughness2);
            lambdaL = nv * (nl * (1.0 - roughness2) + roughness2);

            float r2 = roughness2 * roughness2;
            d = (nh * r2 - nh) * nh + 1.0;
            ggx = r2 / (d * d + 1e-7f);
        }

        #if defined(SHADER_API_MOBILE) || defined(SHADER_API_SWITCH)
            sjggx = 0.5 / (lambdaV + lambdaL + 1e-4f);
        #else
            sjggx = 0.5 / (lambdaV + lambdaL + 1e-5f);
        #endif

        float specularTerm = sjggx * ggx;
        #ifdef LIL_COLORSPACE_GAMMA
            specularTerm = sqrt(max(1e-4h, specularTerm));
        #endif
        specularTerm *= nl * attenuation;

        // Output
        #if defined(LIL_FEATURE_ANISOTROPY)
            if(_SpecularToon) return lilTooningScale(_AAStrength, specularTerm, 0.5);
        #endif
        return specularTerm * lilFresnelTerm(specular, lh);
    }

    void lilReflection(inout lilFragData fd LIL_SAMP_IN_FUNC(samp) LIL_HDRP_POSITION_INPUT_ARGS)
    {
        #if defined(LIL_PASS_FORWARDADD)
            if(_UseReflection && _ApplySpecular && _ApplySpecularFA)
        #else
            if(_UseReflection)
        #endif
        {
            float3 reflectCol = 0;
            // Smoothness
            #if !defined(LIL_REFRACTION_BLUR2) || defined(LIL_PASS_FORWARDADD)
                fd.smoothness = _Smoothness;
                #if defined(LIL_FEATURE_SmoothnessTex)
                    fd.smoothness *= LIL_SAMPLE_2D_ST(_SmoothnessTex, samp, fd.uvMain).r;
                #endif
                GSAAForSmoothness(fd.smoothness, fd.N, _GSAAStrength);
                fd.perceptualRoughness = fd.perceptualRoughness - fd.smoothness * fd.perceptualRoughness;
                fd.roughness = fd.perceptualRoughness * fd.perceptualRoughness;
            #endif
            // Metallic
            float metallic = _Metallic;
            #if defined(LIL_FEATURE_MetallicGlossMap)
                metallic *= LIL_SAMPLE_2D_ST(_MetallicGlossMap, samp, fd.uvMain).r;
            #endif
            fd.col.rgb = fd.col.rgb - metallic * fd.col.rgb;
            float3 specular = lerp(_Reflectance, fd.albedo, metallic);
            // Color
            float4 reflectionColor = _ReflectionColor;
            #if defined(LIL_FEATURE_ReflectionColorTex)
                reflectionColor *= LIL_SAMPLE_2D_ST(_ReflectionColorTex, samp, fd.uvMain);
            #endif
            #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                if(_ReflectionApplyTransparency) reflectionColor.a *= fd.col.a;
            #endif
            // Specular
            #if !defined(LIL_PASS_FORWARDADD)
                if(_ApplySpecular)
            #endif
            {
                #if 1
                    float3 lightDirectionSpc = fd.L;
                    float3 lightColorSpc = fd.lightColor;
                #else
                    float3 lightDirectionSpc = lilGetLightDirection(fd.positionWS);
                    float3 lightColorSpc = LIL_MAINLIGHT_COLOR;
                #endif
                #if defined(LIL_PASS_FORWARDADD)
                    reflectCol = lilCalcSpecular(fd, lightDirectionSpc, specular, fd.shadowmix * fd.attenuation LIL_SAMP_IN(samp));
                #elif defined(SHADOWS_SCREEN)
                    reflectCol = lilCalcSpecular(fd, lightDirectionSpc, specular, fd.shadowmix LIL_SAMP_IN(samp));
                #else
                    reflectCol = lilCalcSpecular(fd, lightDirectionSpc, specular, 1.0 LIL_SAMP_IN(samp));
                #endif
                fd.col.rgb = lilBlendColor(fd.col.rgb, reflectionColor.rgb * lightColorSpc, reflectCol * reflectionColor.a, _ReflectionBlendMode);
            }
            // Reflection
            #if !defined(LIL_PASS_FORWARDADD)
                if(_ApplyReflection)
                {
                    float3 N = fd.reflectionN;
                    #if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)
                        N = lerp(fd.origN, fd.reflectionN, _ReflectionNormalStrength);
                    #endif

                    float3 envReflectionColor = LIL_GET_ENVIRONMENT_REFLECTION(fd.V, N, fd.perceptualRoughness, fd.positionWS);

                    float oneMinusReflectivity = LIL_DIELECTRIC_SPECULAR.a - metallic * LIL_DIELECTRIC_SPECULAR.a;
                    float grazingTerm = saturate(fd.smoothness + (1.0-oneMinusReflectivity));
                    #ifdef LIL_COLORSPACE_GAMMA
                        float surfaceReduction = 1.0 - 0.28 * fd.roughness * fd.perceptualRoughness;
                    #else
                        float surfaceReduction = 1.0 / (fd.roughness * fd.roughness + 1.0);
                    #endif

                    #ifdef LIL_REFRACTION
                        fd.col.rgb = lerp(envReflectionColor, fd.col.rgb, fd.col.a+(1.0-fd.col.a)*pow(fd.nvabs,abs(_RefractionStrength)*0.5+0.25));
                        reflectCol = fd.col.a * surfaceReduction * envReflectionColor * lilFresnelLerp(specular, grazingTerm, fd.nv);
                        fd.col.a = 1.0;
                    #else
                        reflectCol = surfaceReduction * envReflectionColor * lilFresnelLerp(specular, grazingTerm, fd.nv);
                    #endif
                    fd.col.rgb = lilBlendColor(fd.col.rgb, reflectionColor.rgb, reflectCol * reflectionColor.a, _ReflectionBlendMode);
                }
            #endif
        }
    }
#endif

#if !defined(OVERRIDE_REFLECTION)
    #define OVERRIDE_REFLECTION \
        lilReflection(fd LIL_SAMP_IN(sampler_MainTex) LIL_HDRP_POSITION_INPUT_VAR);
#endif

//------------------------------------------------------------------------------------------------------------------------------
// MatCap
#if defined(LIL_FEATURE_MATCAP) && !defined(LIL_LITE)
    void lilGetMatCap(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        if(_UseMatCap)
        {
            // Normal
            float3 N = fd.matcapN;
            #if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)
                N = lerp(fd.origN, fd.matcapN, _MatCapNormalStrength);
            #endif
            #if defined(LIL_FEATURE_MatCapBumpMap)
                if(_MatCapCustomNormal)
                {
                    float4 normalTex = LIL_SAMPLE_2D_ST(_MatCapBumpMap, samp, fd.uvMain);
                    float3 normalmap = lilUnpackNormalScale(normalTex, _MatCapBumpScale);
                    N = normalize(mul(normalmap, fd.TBN));
                    N = fd.facing < (_FlipNormal-1.0) ? -N : N;
                }
            #endif

            // UV
            float2 matUV = lilCalcMatCapUV(fd.uv1, normalize(N), fd.V, fd.headV, _MatCapTex_ST, _MatCapBlendUV1.xy, _MatCapZRotCancel, _MatCapPerspective, _MatCapVRParallaxStrength);

            // Color
            float4 matCapColor = _MatCapColor;
            #if defined(LIL_FEATURE_MatCapTex)
                matCapColor *= LIL_SAMPLE_2D_LOD(_MatCapTex, lil_sampler_linear_repeat, matUV, _MatCapLod);
            #endif
            #if !defined(LIL_PASS_FORWARDADD)
                matCapColor.rgb = lerp(matCapColor.rgb, matCapColor.rgb * fd.lightColor, _MatCapEnableLighting);
                matCapColor.a = lerp(matCapColor.a, matCapColor.a * fd.shadowmix, _MatCapShadowMask);
            #else
                if(_MatCapBlendMode < 3) matCapColor.rgb *= fd.lightColor * _MatCapEnableLighting;
                matCapColor.a = lerp(matCapColor.a, matCapColor.a * fd.shadowmix, _MatCapShadowMask);
            #endif
            #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                if(_MatCapApplyTransparency) matCapColor.a *= fd.col.a;
            #endif
            matCapColor.a = fd.facing < (_MatCapBackfaceMask-1.0) ? 0.0 : matCapColor.a;
            float3 matCapMask = 1.0;
            #if defined(LIL_FEATURE_MatCapBlendMask)
                matCapMask = LIL_SAMPLE_2D_ST(_MatCapBlendMask, samp, fd.uvMain).rgb;
            #endif

            // Blend
            matCapColor.rgb = lerp(matCapColor.rgb, matCapColor.rgb * fd.albedo, _MatCapMainStrength);
            fd.col.rgb = lilBlendColor(fd.col.rgb, matCapColor.rgb, _MatCapBlend * matCapColor.a * matCapMask, _MatCapBlendMode);
        }
    }
#elif defined(LIL_LITE)
    void lilGetMatCap(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        if(_UseMatCap)
        {
            float3 matcap = 1.0;
            matcap = LIL_SAMPLE_2D(_MatCapTex, samp, fd.uvMat).rgb;
            fd.col.rgb = lerp(fd.col.rgb, _MatCapMul ? fd.col.rgb * matcap : fd.col.rgb + matcap, fd.triMask.r);
        }
    }
#endif

#if !defined(OVERRIDE_MATCAP)
    #define OVERRIDE_MATCAP \
        lilGetMatCap(fd LIL_SAMP_IN(sampler_MainTex));
#endif

//------------------------------------------------------------------------------------------------------------------------------
// MatCap 2nd
#if defined(LIL_FEATURE_MATCAP_2ND) && !defined(LIL_LITE)
    void lilGetMatCap2nd(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        if(_UseMatCap2nd)
        {
            // Normal
            float3 N = fd.matcap2ndN;
            #if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)
                N = lerp(fd.origN, fd.matcap2ndN, _MatCap2ndNormalStrength);
            #endif
            #if defined(LIL_FEATURE_MatCap2ndBumpMap)
                if(_MatCap2ndCustomNormal)
                {
                    float4 normalTex = LIL_SAMPLE_2D_ST(_MatCap2ndBumpMap, samp, fd.uvMain);
                    float3 normalmap = lilUnpackNormalScale(normalTex, _MatCap2ndBumpScale);
                    N = normalize(mul(normalmap, fd.TBN));
                    N = fd.facing < (_FlipNormal-1.0) ? -N : N;
                }
            #endif

            // UV
            float2 mat2ndUV = lilCalcMatCapUV(fd.uv1, N, fd.V, fd.headV, _MatCap2ndTex_ST, _MatCap2ndBlendUV1.xy, _MatCap2ndZRotCancel, _MatCap2ndPerspective, _MatCap2ndVRParallaxStrength);

            // Color
            float4 matCap2ndColor = _MatCap2ndColor;
            #if defined(LIL_FEATURE_MatCap2ndTex)
                matCap2ndColor *= LIL_SAMPLE_2D_LOD(_MatCap2ndTex, lil_sampler_linear_repeat, mat2ndUV, _MatCap2ndLod);
            #endif
            #if !defined(LIL_PASS_FORWARDADD)
                matCap2ndColor.rgb = lerp(matCap2ndColor.rgb, matCap2ndColor.rgb * fd.lightColor, _MatCap2ndEnableLighting);
                matCap2ndColor.a = lerp(matCap2ndColor.a, matCap2ndColor.a * fd.shadowmix, _MatCap2ndShadowMask);
            #else
                if(_MatCap2ndBlendMode < 3) matCap2ndColor.rgb *= fd.lightColor * _MatCap2ndEnableLighting;
                matCap2ndColor.a = lerp(matCap2ndColor.a, matCap2ndColor.a * fd.shadowmix, _MatCap2ndShadowMask);
            #endif
            #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                if(_MatCap2ndApplyTransparency) matCap2ndColor.a *= fd.col.a;
            #endif
            matCap2ndColor.a = fd.facing < (_MatCap2ndBackfaceMask-1.0) ? 0.0 : matCap2ndColor.a;
            float3 matCapMask = 1.0;
            #if defined(LIL_FEATURE_MatCap2ndBlendMask)
                matCapMask = LIL_SAMPLE_2D_ST(_MatCap2ndBlendMask, samp, fd.uvMain).rgb;
            #endif

            // Blend
            matCap2ndColor.rgb = lerp(matCap2ndColor.rgb, matCap2ndColor.rgb * fd.albedo, _MatCap2ndMainStrength);
            fd.col.rgb = lilBlendColor(fd.col.rgb, matCap2ndColor.rgb, _MatCap2ndBlend * matCap2ndColor.a * matCapMask, _MatCap2ndBlendMode);
        }
    }
#endif

#if !defined(OVERRIDE_MATCAP_2ND)
    #define OVERRIDE_MATCAP_2ND \
        lilGetMatCap2nd(fd LIL_SAMP_IN(sampler_MainTex));
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Rim Light
#if defined(LIL_FEATURE_RIMLIGHT) && !defined(LIL_LITE)
    void lilGetRim(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        if(_UseRim)
        {
            #if defined(LIL_FEATURE_RIMLIGHT_DIRECTION)
                // Color
                float4 rimColor = _RimColor;
                float4 rimIndirColor = _RimIndirColor;
                #if defined(LIL_FEATURE_RimColorTex)
                    float4 rimColorTex = LIL_SAMPLE_2D_ST(_RimColorTex, samp, fd.uvMain);
                    rimColor *= rimColorTex;
                    rimIndirColor *= rimColorTex;
                #endif
                rimColor.rgb = lerp(rimColor.rgb, rimColor.rgb * fd.albedo, _RimMainStrength);

                // View direction
                float3 V = lilBlendVRParallax(fd.headV, fd.V, _RimVRParallaxStrength);

                // Normal
                float3 N = fd.N;
                #if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)
                    N = lerp(fd.origN, fd.N, _RimNormalStrength);
                #endif
                float nvabs = abs(dot(N,V));

                // Factor
                float lnRaw = dot(fd.L, N) * 0.5 + 0.5;
                float lnDir = saturate((lnRaw + _RimDirRange) / (1.0 + _RimDirRange));
                float lnIndir = saturate((1.0-lnRaw + _RimIndirRange) / (1.0 + _RimIndirRange));
                float rim = pow(saturate(1.0 - nvabs), _RimFresnelPower);
                rim = fd.facing < (_RimBackfaceMask-1.0) ? 0.0 : rim;
                float rimDir = lerp(rim, rim*lnDir, _RimDirStrength);
                float rimIndir = rim * lnIndir * _RimDirStrength;

                rimDir = lilTooningScale(_AAStrength, rimDir, _RimBorder, _RimBlur);
                rimIndir = lilTooningScale(_AAStrength, rimIndir, _RimIndirBorder, _RimIndirBlur);

                rimDir = lerp(rimDir, rimDir * fd.shadowmix, _RimShadowMask);
                rimIndir = lerp(rimIndir, rimIndir * fd.shadowmix, _RimShadowMask);
                #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                    if(_RimApplyTransparency)
                    {
                        rimDir *= fd.col.a;
                        rimIndir *= fd.col.a;
                    }
                #endif

                // Blend
                #if !defined(LIL_PASS_FORWARDADD)
                    float3 rimLightMul = 1 - _RimEnableLighting + fd.lightColor * _RimEnableLighting;
                #else
                    float3 rimLightMul = _RimBlendMode < 3 ? fd.lightColor * _RimEnableLighting : 1;
                #endif
                fd.col.rgb = lilBlendColor(fd.col.rgb, rimColor.rgb * rimLightMul, rimDir * rimColor.a, _RimBlendMode);
                fd.col.rgb = lilBlendColor(fd.col.rgb, rimIndirColor.rgb * rimLightMul, rimIndir * rimIndirColor.a, _RimBlendMode);
            #else
                // Color
                float4 rimColor = _RimColor;
                #if defined(LIL_FEATURE_RimColorTex)
                    rimColor *= LIL_SAMPLE_2D_ST(_RimColorTex, samp, fd.uvMain);
                #endif
                rimColor.rgb = lerp(rimColor.rgb, rimColor.rgb * fd.albedo, _RimMainStrength);

                // Normal
                float3 N = fd.N;
                #if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)
                    N = lerp(fd.origN, fd.N, _RimNormalStrength);
                #endif
                float nvabs = abs(dot(N,fd.V));

                // Factor
                float rim = pow(saturate(1.0 - nvabs), _RimFresnelPower);
                rim = fd.facing < (_RimBackfaceMask-1.0) ? 0.0 : rim;
                rim = lilTooningScale(_AAStrength, rim, _RimBorder, _RimBlur);
                #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                    if(_RimApplyTransparency) rim *= fd.col.a;
                #endif
                rim = lerp(rim, rim * fd.shadowmix, _RimShadowMask);

                // Blend
                #if !defined(LIL_PASS_FORWARDADD)
                    rimColor.rgb = lerp(rimColor.rgb, rimColor.rgb * fd.lightColor, _RimEnableLighting);
                #else
                    if(_RimBlendMode < 3) rimColor.rgb *= fd.lightColor * _RimEnableLighting;
                #endif
                fd.col.rgb = lilBlendColor(fd.col.rgb, rimColor.rgb, rim * rimColor.a, _RimBlendMode);
            #endif
        }
    }
#elif defined(LIL_LITE)
    void lilGetRim(inout lilFragData fd)
    {
        if(_UseRim)
        {
            float rim = pow(saturate(1.0 - fd.nvabs), _RimFresnelPower);
            rim = lilTooningScale(_AAStrength, rim, _RimBorder, _RimBlur);
            rim = lerp(rim, rim * fd.shadowmix, _RimShadowMask);
            fd.col.rgb += rim * fd.triMask.g * _RimColor.rgb * fd.lightColor;
        }
    }
#endif

#if !defined(OVERRIDE_RIMLIGHT)
    #if defined(LIL_LITE)
        #define OVERRIDE_RIMLIGHT \
            lilGetRim(fd);
    #else
        #define OVERRIDE_RIMLIGHT \
            lilGetRim(fd LIL_SAMP_IN(sampler_MainTex));
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Glitter
#if defined(LIL_FEATURE_GLITTER) && !defined(LIL_LITE)
    void lilGlitter(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        if(_UseGlitter)
        {
            // View direction
            float3 glitterViewDirection = lilBlendVRParallax(fd.headV, fd.V, _GlitterVRParallaxStrength);
            float3 glitterCameraDirection = lerp(fd.cameraFront, fd.V, _GlitterVRParallaxStrength);

            // Normal
            float3 N = fd.N;
            #if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)
                N = lerp(fd.origN, fd.N, _GlitterNormalStrength);
            #endif

            // Color
            float4 glitterColor = _GlitterColor;
            #if defined(LIL_FEATURE_GlitterColorTex)
                float2 uvGlitterColor = fd.uvMain; //fd.uv0;
                if(_GlitterColorTex_UVMode == 1) uvGlitterColor = fd.uv1;
                if(_GlitterColorTex_UVMode == 2) uvGlitterColor = fd.uv2;
                if(_GlitterColorTex_UVMode == 3) uvGlitterColor = fd.uv3;
                glitterColor *= LIL_SAMPLE_2D_ST(_GlitterColorTex, samp, uvGlitterColor);
            #endif
            float2 glitterPos = _GlitterUVMode ? fd.uv1 : fd.uv0;
            #if defined(LIL_FEATURE_GlitterShapeTex)
                glitterColor.rgb *= lilCalcGlitter(glitterPos, N, glitterViewDirection, glitterCameraDirection, fd.L, _GlitterParams1, _GlitterParams2, _GlitterPostContrast, _GlitterSensitivity, _GlitterScaleRandomize, _GlitterAngleRandomize, _GlitterApplyShape, _GlitterShapeTex, _GlitterShapeTex_ST, _GlitterAtras);
            #else
                glitterColor.rgb *= lilCalcGlitter(glitterPos, N, glitterViewDirection, glitterCameraDirection, fd.L, _GlitterParams1, _GlitterParams2, _GlitterPostContrast, _GlitterSensitivity, _GlitterScaleRandomize, 0, false, _GlitterShapeTex, float4(0,0,0,0), float4(1,1,0,0));
            #endif
            glitterColor.rgb = lerp(glitterColor.rgb, glitterColor.rgb * fd.albedo, _GlitterMainStrength);
            #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                if(_GlitterApplyTransparency) glitterColor.a *= fd.col.a;
            #endif
            glitterColor.a = fd.facing < (_GlitterBackfaceMask-1.0) ? 0.0 : glitterColor.a;

            // Blend
            #if !defined(LIL_PASS_FORWARDADD)
                glitterColor.a = lerp(glitterColor.a, glitterColor.a * fd.shadowmix, _GlitterShadowMask);
                glitterColor.rgb = lerp(glitterColor.rgb, glitterColor.rgb * fd.lightColor, _GlitterEnableLighting);
                fd.col.rgb += glitterColor.rgb * glitterColor.a;
            #else
                glitterColor.a = lerp(glitterColor.a, glitterColor.a * fd.shadowmix, _GlitterShadowMask);
                fd.col.rgb += glitterColor.a * _GlitterEnableLighting * glitterColor.rgb * fd.lightColor;
            #endif
        }
    }
#endif

#if !defined(OVERRIDE_GLITTER)
    #define OVERRIDE_GLITTER \
        lilGlitter(fd LIL_SAMP_IN(sampler_MainTex));
#endif


//------------------------------------------------------------------------------------------------------------------------------
// Emission
#if defined(LIL_FEATURE_EMISSION_1ST) && !defined(LIL_LITE)
    void lilEmission(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        if(_UseEmission)
        {
            float4 emissionColor = _EmissionColor;
            // UV
            float2 emissionUV = fd.uv0;
            if(_EmissionMap_UVMode == 1) emissionUV = fd.uv1;
            if(_EmissionMap_UVMode == 2) emissionUV = fd.uv2;
            if(_EmissionMap_UVMode == 3) emissionUV = fd.uv3;
            if(_EmissionMap_UVMode == 4) emissionUV = fd.uvRim;
            //if(_EmissionMap_UVMode == 4) emissionUV = fd.uvPanorama;
            float2 _EmissionMapParaTex = emissionUV + _EmissionParallaxDepth * fd.parallaxOffset;
            // Texture
            #if defined(LIL_FEATURE_EmissionMap)
                #if defined(LIL_FEATURE_ANIMATE_EMISSION_UV)
                    emissionColor *= LIL_GET_EMITEX(_EmissionMap, _EmissionMapParaTex);
                #else
                    emissionColor *= LIL_SAMPLE_2D_ST(_EmissionMap, sampler_EmissionMap, _EmissionMapParaTex);
                #endif
            #endif
            // Mask
            #if defined(LIL_FEATURE_EmissionBlendMask)
                #if defined(LIL_FEATURE_ANIMATE_EMISSION_MASK_UV)
                    emissionColor *= LIL_GET_EMIMASK(_EmissionBlendMask, fd.uv0);
                #else
                    emissionColor *= LIL_SAMPLE_2D_ST(_EmissionBlendMask, samp, fd.uvMain);
                #endif
            #endif
            // Gradation
            #if defined(LIL_FEATURE_EmissionGradTex)
                #if defined(LIL_FEATURE_EMISSION_GRADATION) && defined(LIL_FEATURE_AUDIOLINK)
                    if(_EmissionUseGrad)
                    {
                        float gradUV = _EmissionGradSpeed * LIL_TIME + fd.audioLinkValue * _AudioLink2EmissionGrad;
                        emissionColor *= LIL_SAMPLE_1D_LOD(_EmissionGradTex, lil_sampler_linear_repeat, gradUV, 0);
                    }
                #elif defined(LIL_FEATURE_EMISSION_GRADATION)
                    if(_EmissionUseGrad) emissionColor *= LIL_SAMPLE_1D(_EmissionGradTex, lil_sampler_linear_repeat, _EmissionGradSpeed * LIL_TIME);
                #endif
            #endif
            #if defined(LIL_FEATURE_AUDIOLINK)
                if(_AudioLink2Emission) emissionColor.a *= fd.audioLinkValue;
            #endif
            emissionColor.rgb = lerp(emissionColor.rgb, emissionColor.rgb * fd.invLighting, _EmissionFluorescence);
            emissionColor.rgb = lerp(emissionColor.rgb, emissionColor.rgb * fd.albedo, _EmissionMainStrength);
            float emissionBlend = _EmissionBlend * lilCalcBlink(_EmissionBlink) * emissionColor.a;
            #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                emissionBlend *= fd.col.a;
            #endif
            fd.col.rgb = lilBlendColor(fd.col.rgb, emissionColor.rgb, emissionBlend, _EmissionBlendMode);
        }
    }
#elif defined(LIL_LITE)
    void lilEmission(inout lilFragData fd)
    {
        if(_UseEmission)
        {
            float emissionBlinkSeq = lilCalcBlink(_EmissionBlink);
            float4 emissionColor = _EmissionColor;
            float2 emissionUV = fd.uv0;
            if(_EmissionMap_UVMode == 1) emissionUV = fd.uv1;
            if(_EmissionMap_UVMode == 2) emissionUV = fd.uv2;
            if(_EmissionMap_UVMode == 3) emissionUV = fd.uv3;
            if(_EmissionMap_UVMode == 4) emissionUV = fd.uvRim;
            emissionColor *= LIL_GET_EMITEX(_EmissionMap,emissionUV);
            fd.emissionColor += emissionBlinkSeq * fd.triMask.b * emissionColor.rgb;
        }
    }
#endif

#if !defined(OVERRIDE_EMISSION_1ST)
    #if defined(LIL_LITE)
        #define OVERRIDE_EMISSION_1ST \
            lilEmission(fd);
    #else
        #define OVERRIDE_EMISSION_1ST \
            lilEmission(fd LIL_SAMP_IN(sampler_MainTex));
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Emission 2nd
#if defined(LIL_FEATURE_EMISSION_2ND) && !defined(LIL_LITE)
    void lilEmission2nd(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        if(_UseEmission2nd)
        {
            float4 emission2ndColor = _Emission2ndColor;
            // UV
            float2 emission2ndUV = fd.uv0;
            if(_Emission2ndMap_UVMode == 1) emission2ndUV = fd.uv1;
            if(_Emission2ndMap_UVMode == 2) emission2ndUV = fd.uv2;
            if(_Emission2ndMap_UVMode == 3) emission2ndUV = fd.uv3;
            if(_Emission2ndMap_UVMode == 4) emission2ndUV = fd.uvRim;
            //if(_Emission2ndMap_UVMode == 4) emission2ndUV = fd.uvPanorama;
            float2 _Emission2ndMapParaTex = emission2ndUV + _Emission2ndParallaxDepth * fd.parallaxOffset;
            // Texture
            #if defined(LIL_FEATURE_Emission2ndMap)
                #if defined(LIL_FEATURE_ANIMATE_EMISSION_UV)
                    emission2ndColor *= LIL_GET_EMITEX(_Emission2ndMap, _Emission2ndMapParaTex);
                #else
                    emission2ndColor *= LIL_SAMPLE_2D_ST(_Emission2ndMap, sampler_Emission2ndMap, _Emission2ndMapParaTex);
                #endif
            #endif
            // Mask
            #if defined(LIL_FEATURE_Emission2ndBlendMask)
                #if defined(LIL_FEATURE_ANIMATE_EMISSION_MASK_UV)
                    emission2ndColor *= LIL_GET_EMIMASK(_Emission2ndBlendMask, fd.uv0);
                #else
                    emission2ndColor *= LIL_SAMPLE_2D_ST(_Emission2ndBlendMask, samp, fd.uvMain);
                #endif
            #endif
            // Gradation
            #if defined(LIL_FEATURE_Emission2ndGradTex)
                #if defined(LIL_FEATURE_EMISSION_GRADATION) && defined(LIL_FEATURE_AUDIOLINK)
                    if(_Emission2ndUseGrad)
                    {
                        float gradUV = _Emission2ndGradSpeed * LIL_TIME + fd.audioLinkValue * _AudioLink2Emission2ndGrad;
                        emission2ndColor *= LIL_SAMPLE_1D_LOD(_Emission2ndGradTex, lil_sampler_linear_repeat, gradUV, 0);
                    }
                #elif defined(LIL_FEATURE_EMISSION_GRADATION)
                    if(_Emission2ndUseGrad) emission2ndColor *= LIL_SAMPLE_1D(_Emission2ndGradTex, lil_sampler_linear_repeat, _Emission2ndGradSpeed * LIL_TIME);
                #endif
            #endif
            #if defined(LIL_FEATURE_AUDIOLINK)
                if(_AudioLink2Emission2nd) emission2ndColor.a *= fd.audioLinkValue;
            #endif
            emission2ndColor.rgb = lerp(emission2ndColor.rgb, emission2ndColor.rgb * fd.invLighting, _Emission2ndFluorescence);
            emission2ndColor.rgb = lerp(emission2ndColor.rgb, emission2ndColor.rgb * fd.albedo, _Emission2ndMainStrength);
            float emission2ndBlend = _Emission2ndBlend * lilCalcBlink(_Emission2ndBlink) * emission2ndColor.a;
            #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                emission2ndBlend *= fd.col.a;
            #endif
            fd.col.rgb = lilBlendColor(fd.col.rgb, emission2ndColor.rgb, emission2ndBlend, _Emission2ndBlendMode);
        }
    }
#endif

#if !defined(OVERRIDE_EMISSION_2ND)
    #define OVERRIDE_EMISSION_2ND \
        lilEmission2nd(fd LIL_SAMP_IN(sampler_MainTex));
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Dissolve Add
#if !defined(OVERRIDE_DISSOLVE_ADD)
    #define OVERRIDE_DISSOLVE_ADD \
        fd.emissionColor += _DissolveColor.rgb * dissolveAlpha;
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Blend Emission
#if !defined(OVERRIDE_BLEND_EMISSION)
    #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
        #define OVERRIDE_BLEND_EMISSION \
            fd.col.rgb += fd.emissionColor * fd.col.a;
    #else
        #define OVERRIDE_BLEND_EMISSION \
            fd.col.rgb += fd.emissionColor;
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Depth Fade
#if defined(LIL_FEATURE_DEPTH_FADE) && LIL_RENDER == 2 && !defined(LIL_REFRACTION) && !defined(LIL_LITE)
    void lilDepthFade(inout lilFragData fd)
    {
        if((_DepthFadeTransparency < 1) && LIL_ENABLED_DEPTH_TEX)
        {
            float depthObj = fd.positionCS.w;
            float depthCam = LIL_GET_DEPTH_TEX_CS(fd.positionCS.xy);
            #if UNITY_REVERSED_Z
                if(depthCam == 0) return;
            #else
                if(depthCam == 1) return;
            #endif
            depthCam = LIL_TO_LINEARDEPTH(depthCam,fd.positionCS.xy);
            float depthDiff = depthCam - depthObj;
            float factor = saturate(depthDiff * _DepthFadeSharpness + _DepthFadeTransparency);
            if(_DepthFadeToColor) fd.col.rgb *= lerp(_DepthFadeColor.rgb, fd.col.rgb, factor);
            if(_DepthFadeToAlpha) fd.col.a *= factor;
            //fd.col.a *= saturate(depthDiff * 20 + 0);
        }
        //float4 _DepthFadeColor;
        //float _DepthFadeSharpness;
        //float _DepthFadeTransparency;
        //uint _DepthFadeToColor;
        //uint _DepthFadeToAlpha;
    }
#endif

#if !defined(OVERRIDE_DEPTH_FADE)
    #define OVERRIDE_DEPTH_FADE lilDepthFade(fd);
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Distance Fade
#if defined(LIL_FEATURE_DISTANCE_FADE) && !defined(LIL_LITE)
    void lilDistanceFadeAlphaOnly(inout lilFragData fd)
    {
        float depth = _DistanceFadeMode ? fd.depthObject : fd.depth;
        float distFade = saturate((depth - _DistanceFade.x) / (_DistanceFade.y - _DistanceFade.x));
        #if defined(LIL_OUTLINE) || defined(LIL_PASS_FORWARD_FUR_INCLUDED)
            distFade = distFade * _DistanceFade.z;
        #else
            distFade = fd.facing < (_DistanceFade.w-1.0) ? _DistanceFade.z : distFade * _DistanceFade.z;
        #endif
        #if LIL_RENDER == 1
            fd.col.a = lerp(fd.col.a, fd.col.a * _DistanceFadeColor.a, distFade);
        #endif
    }

    void lilDistanceFade(inout lilFragData fd)
    {
        float depth = _DistanceFadeMode ? fd.depthObject : fd.depth;
        float distFade = saturate((depth - _DistanceFade.x) / (_DistanceFade.y - _DistanceFade.x));
        #if defined(LIL_OUTLINE) || defined(LIL_PASS_FORWARD_FUR_INCLUDED)
            distFade = distFade * _DistanceFade.z;
        #else
            distFade = fd.facing < (_DistanceFade.w-1.0) ? _DistanceFade.z : distFade * _DistanceFade.z;
        #endif

        float3 fadeColor = _DistanceFadeColor.rgb;
        #if defined(LIL_V2F_NORMAL_WS)
            float nvabs = abs(dot(fd.origN,fd.headV));
            float fadeRim = pow(saturate(1.0 - nvabs), _DistanceFadeRimFresnelPower);
            fadeColor = lerp(fadeColor, _DistanceFadeRimColor.rgb * fd.col.rgb, fadeRim * _DistanceFadeRimColor.a);
        #endif

        #if defined(LIL_PASS_FORWARDADD)
            fd.col.rgb = lerp(fd.col.rgb, 0.0, distFade);
        #elif LIL_RENDER == 2
            fd.col.rgb = lerp(fd.col.rgb, fadeColor * _DistanceFadeColor.a, distFade);
            fd.col.a = lerp(fd.col.a, fd.col.a * _DistanceFadeColor.a, distFade);
        #else
            fd.col.rgb = lerp(fd.col.rgb, fadeColor, distFade);
        #endif
    }
#endif

#if !defined(OVERRIDE_DISTANCE_FADE)
    #define OVERRIDE_DISTANCE_FADE \
        lilDistanceFade(fd);
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Fog
#if !defined(OVERRIDE_FOG)
    #if defined(LIL_GEM) && !defined(LIL_GEM_PRE)
        #define OVERRIDE_FOG \
            LIL_APPLY_FOG_COLOR(fd.col, input, fogColor);
    #else
        #define OVERRIDE_FOG \
            LIL_APPLY_FOG(fd.col, input);
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Output
#if !defined(OVERRIDE_OUTPUT)
    #define OVERRIDE_OUTPUT \
        return fd.col;
#endif