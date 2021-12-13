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
    #define LIL_UNPACK_POSITION_OS(i,o) o.positionOS = i.positionOS;
#else
    #define LIL_UNPACK_POSITION_OS(i,o)
#endif

#if defined(LIL_V2F_POSITION_WS)
    #define LIL_UNPACK_POSITION_WS(i,o) o.positionWS = lilToAbsolutePositionWS(i.positionWS);
#else
    #define LIL_UNPACK_POSITION_WS(i,o)
#endif

#if defined(LIL_V2F_POSITION_CS)
    #if defined(UNITY_SINGLE_PASS_STEREO)
        #define LIL_SCREEN_UV_STEREO_FIX(i,o) o.uvScn.x *= 0.5;
    #else
        #define LIL_SCREEN_UV_STEREO_FIX(i,o)
    #endif

    #define LIL_UNPACK_POSITION_CS(i,o) \
        o.positionCS = i.positionCS; \
        o.positionSS = lilTransformCStoSSFrag(i.positionCS); \
        o.uvScn = i.positionCS.xy / _ScreenParams.xy; \
        LIL_SCREEN_UV_STEREO_FIX(i,o)
#else
    #define LIL_UNPACK_POSITION_CS(i,o)
#endif

#if defined(LIL_V2F_POSITION_SS)
    #define LIL_UNPACK_POSITION_SS(i,o) o.positionSS = i.positionSS;
#else
    #define LIL_UNPACK_POSITION_SS(i,o)
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
    LIL_UNPACK_POSITION_SS(input,fd); \
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
// Parallax
#if !defined(OVERRIDE_PARALLAX)
    #if defined(LIL_FEATURE_POM)
        #define OVERRIDE_PARALLAX \
            lilPOM(fd.uvMain, fd.uv0, _UseParallax, _MainTex_ST, fd.parallaxViewDirection, _ParallaxMap, _Parallax, _ParallaxOffset);
    #else
        #define OVERRIDE_PARALLAX \
            lilParallax(fd.uvMain, fd.uv0, _UseParallax, fd.parallaxOffset, _ParallaxMap, _Parallax, _ParallaxOffset);
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Main Texture
#if defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) && !defined(LIL_FUR)
    #define LIL_GET_MAIN_TEX \
        fd.col = LIL_SAMPLE_2D_POM(_MainTex, sampler_MainTex, fd.uvMain, ddxMain, ddyMain);

    // Tone correction
    #if defined(LIL_FEATURE_MAIN_TONE_CORRECTION)
        #define LIL_MAIN_TONECORRECTION \
            fd.col.rgb = lilToneCorrection(fd.col.rgb, _MainTexHSVG);
    #else
        #define LIL_MAIN_TONECORRECTION
    #endif

    // Gradation map
    #if defined(LIL_FEATURE_MAIN_GRADATION_MAP)
        #define LIL_MAIN_GRADATION_MAP \
            fd.col.rgb = lilGradationMap(fd.col.rgb, _MainGradationTex, _MainGradationStrength);
    #else
        #define LIL_MAIN_GRADATION_MAP
    #endif

    #if defined(LIL_FEATURE_MAIN_TONE_CORRECTION) || defined(LIL_FEATURE_MAIN_GRADATION_MAP)
        #define LIL_APPLY_MAIN_TONECORRECTION \
            float3 beforeToneCorrectionColor = fd.col.rgb; \
            float colorAdjustMask = LIL_SAMPLE_2D(_MainColorAdjustMask, sampler_MainTex, fd.uvMain).r; \
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
#if defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) && defined(LIL_FEATURE_TEX_OUTLINE_COLOR) || !defined(LIL_PASS_FORWARD_NORMAL_INCLUDED)
    #define LIL_GET_OUTLINE_TEX \
        fd.col = LIL_SAMPLE_2D(_OutlineTex, sampler_OutlineTex, fd.uvMain);
#else
    #define LIL_GET_OUTLINE_TEX
#endif

#if defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) && defined(LIL_FEATURE_OUTLINE_TONE_CORRECTION)
    #define LIL_APPLY_OUTLINE_TONECORRECTION \
        fd.col.rgb = lilToneCorrection(fd.col.rgb, _OutlineTexHSVG);
#else
    #define LIL_APPLY_OUTLINE_TONECORRECTION
#endif

#if !defined(OVERRIDE_OUTLINE_COLOR)
    #define OVERRIDE_OUTLINE_COLOR \
        LIL_GET_OUTLINE_TEX \
        LIL_APPLY_OUTLINE_TONECORRECTION \
        fd.col *= _OutlineColor;
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Fur
#if LIL_RENDER == 1
    #if defined(LIL_ONEPASS_FUR)
        #define LIL_FUR_LAYER_ALPHA \
            furAlpha = input.furLayer < -1.5 ? 1.0 : saturate(furAlpha - input.furLayer * furLayer * furLayer * furLayer + 0.25);
    #else
        #define LIL_FUR_LAYER_ALPHA \
            furAlpha = saturate(furAlpha - input.furLayer * furLayer * furLayer * furLayer + 0.25);
    #endif
    #define LIL_FUR_LAYER_AO \
        fd.col.rgb *= furLayer * _FurAO * 2.0 + 1.0 - _FurAO;
#else
    #if defined(LIL_ONEPASS_FUR)
        #define LIL_FUR_LAYER_ALPHA \
            furAlpha = input.furLayer < -1.5 ? 1.0 : saturate(furAlpha - input.furLayer * furLayer * furLayer);
    #else
        #define LIL_FUR_LAYER_ALPHA \
            furAlpha = saturate(furAlpha - input.furLayer * furLayer * furLayer);
    #endif
    #define LIL_FUR_LAYER_AO \
        fd.col.rgb *= (1.0-furAlpha) * _FurAO * 1.25 + 1.0 - _FurAO;
#endif

#if defined(LIL_ALPHA_PS)
    #undef LIL_FUR_LAYER_AO
    #define LIL_FUR_LAYER_AO
#endif

#if !defined(OVERRIDE_FUR)
    #define OVERRIDE_FUR \
        float furAlpha = 1.0; \
        float furLayer = abs(input.furLayer); \
        if(Exists_FurNoiseMask) furAlpha = LIL_SAMPLE_2D_ST(_FurNoiseMask, sampler_MainTex, fd.uv0).r; \
        LIL_FUR_LAYER_ALPHA \
        if(Exists_FurMask) furAlpha *= LIL_SAMPLE_2D(_FurMask, sampler_MainTex, fd.uvMain).r; \
        fd.col.a *= furAlpha; \
        LIL_FUR_LAYER_AO
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Alpha Mask
#if !defined(OVERRIDE_ALPHAMASK)
    #define OVERRIDE_ALPHAMASK \
        if(_AlphaMaskMode) \
        { \
            float alphaMask = LIL_SAMPLE_2D(_AlphaMask, sampler_MainTex, fd.uvMain).r; \
            alphaMask = saturate(alphaMask * _AlphaMaskScale + _AlphaMaskValue); \
            fd.col.a = _AlphaMaskMode == 1 ? alphaMask : fd.col.a * alphaMask; \
        }
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Dissolve
#if !defined(OVERRIDE_DISSOLVE)
    #if defined(LIL_FEATURE_TEX_DISSOLVE_NOISE)
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
                _DissolveMask_ST \
                LIL_SAMP_IN(sampler_MainTex) \
            );
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Normal
#if !defined(OVERRIDE_NORMAL_1ST)
    #define OVERRIDE_NORMAL_1ST \
        LIL_BRANCH \
        if(Exists_BumpMap && _UseBumpMap) \
        { \
            float4 normalTex = LIL_SAMPLE_2D_ST(_BumpMap, sampler_MainTex, fd.uvMain); \
            normalmap = UnpackNormalScale(normalTex, _BumpScale); \
        }
#endif

#if !defined(OVERRIDE_NORMAL_2ND)
    #define OVERRIDE_NORMAL_2ND \
        LIL_BRANCH \
        if(Exists_Bump2ndMap && _UseBump2ndMap) \
        { \
            float4 normal2ndTex = LIL_SAMPLE_2D_ST(_Bump2ndMap, sampler_MainTex, fd.uvMain); \
            float bump2ndScale = _Bump2ndScale; \
            if(Exists_Bump2ndScaleMask) bump2ndScale *= LIL_SAMPLE_2D_ST(_Bump2ndScaleMask, sampler_MainTex, fd.uvMain).r; \
            normalmap = lilBlendNormal(normalmap, UnpackNormalScale(normal2ndTex, bump2ndScale)); \
        }
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Anisotropy
#if !defined(OVERRIDE_ANISOTROPY)
    #define OVERRIDE_ANISOTROPY \
        LIL_BRANCH \
        if(_UseAnisotropy) \
        { \
            float4 anisoTangentMap = LIL_SAMPLE_2D_ST(_AnisotropyTangentMap, sampler_MainTex, fd.uvMain); \
            float3 anisoTangent = UnpackNormalScale(anisoTangentMap, 1.0); \
            fd.T = lilOrthoNormalize(normalize(mul(anisoTangent, fd.TBN)), fd.N); \
            fd.B = cross(fd.N, fd.T); \
            fd.anisotropy = _AnisotropyScale; \
            fd.anisotropy *= LIL_SAMPLE_2D_ST(_AnisotropyScaleMask, sampler_MainTex, fd.uvMain).r; \
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
            if((_AudioLinkUVMode == 3 || _AudioLinkUVMode == 4) && Exists_AudioLinkMask)
            {
                audioLinkMask = LIL_SAMPLE_2D(_AudioLinkMask, samp, fd.uvMain);
                audioLinkUV = _AudioLinkUVMode == 3 ? audioLinkMask.rg : float2(frac(audioLinkMask.g * 2.0), 4.5/4.0 + floor(audioLinkMask.g * 2.0)/4.0);
            }

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
            #if defined(LIL_FEATURE_AUDIOLINK_LOCAL)
                if(_AudioLinkAsLocal)
                {
                    audioLinkUV.x += frac(-LIL_TIME * _AudioLinkLocalMapParams.r / 60 * _AudioLinkLocalMapParams.g) + _AudioLinkLocalMapParams.b;
                    fd.audioLinkValue = LIL_SAMPLE_2D(_AudioLinkLocalMap, sampler_linear_repeat, audioLinkUV).r;
                }
                else
            #endif

            // Global
            if(lilCheckAudioLink())
            {
                // Scaling for _AudioTexture (4/64)
                audioLinkUV.y *= 0.0625;
                float4 audioTexture = LIL_SAMPLE_2D(_AudioTexture, sampler_linear_clamp, audioLinkUV);
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
// Main 2nd
#if defined(LIL_FEATURE_MAIN2ND) && defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) && !defined(LIL_LITE) && !defined(LIL_FUR)
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
        LIL_BRANCH
        if(_UseMain2ndTex)
        {
            if(Exists_Main2ndTex) color2nd *= LIL_GET_SUBTEX(_Main2ndTex, fd.uv0);
            if(Exists_Main2ndBlendMask) color2nd.a *= LIL_SAMPLE_2D(_Main2ndBlendMask, samp, fd.uvMain).r;
            #if defined(LIL_FEATURE_LAYER_DISSOLVE)
                #if defined(LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE)
                    lilCalcDissolveWithNoise(
                        color2nd.a,
                        main2ndDissolveAlpha,
                        fd.uv0,
                        fd.positionOS,
                        _Main2ndDissolveParams,
                        _Main2ndDissolvePos,
                        _Main2ndDissolveMask,
                        _Main2ndDissolveMask_ST,
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
                        samp
                    );
                #endif
            #endif
            #if defined(LIL_FEATURE_AUDIOLINK)
                if(_AudioLink2Main2nd) color2nd.a *= fd.audioLinkValue;
            #endif
            color2nd.a = lerp(color2nd.a, color2nd.a * saturate((fd.depth - _Main2ndDistanceFade.x) / (_Main2ndDistanceFade.y - _Main2ndDistanceFade.x)), _Main2ndDistanceFade.z);
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
#if defined(LIL_FEATURE_MAIN3RD) && defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) && !defined(LIL_LITE) && !defined(LIL_FUR)
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
        LIL_BRANCH
        if(_UseMain3rdTex)
        {
            if(Exists_Main3rdTex) color3rd *= LIL_GET_SUBTEX(_Main3rdTex, fd.uv0);
            if(Exists_Main3rdBlendMask) color3rd.a *= LIL_SAMPLE_2D(_Main3rdBlendMask, samp, fd.uvMain).r;
            #if defined(LIL_FEATURE_LAYER_DISSOLVE)
                #if defined(LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE)
                    lilCalcDissolveWithNoise(
                        color3rd.a,
                        main3rdDissolveAlpha,
                        fd.uv0,
                        fd.positionOS,
                        _Main3rdDissolveParams,
                        _Main3rdDissolvePos,
                        _Main3rdDissolveMask,
                        _Main3rdDissolveMask_ST,
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
                        samp
                    );
                #endif
            #endif
            #if defined(LIL_FEATURE_AUDIOLINK)
                if(_AudioLink2Main3rd) color3rd.a *= fd.audioLinkValue;
            #endif
            color3rd.a = lerp(color3rd.a, color3rd.a * saturate((fd.depth - _Main3rdDistanceFade.x) / (_Main3rdDistanceFade.y - _Main3rdDistanceFade.x)), _Main3rdDistanceFade.z);
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
        LIL_BRANCH
        if(_UseShadow)
        {
            // Normal
            float3 N1 = fd.N;
            float3 N2 = fd.N;
            #if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)
                N1 = lerp(fd.origN, fd.N, _ShadowNormalStrength);
                N2 = lerp(fd.origN, fd.N, _Shadow2ndNormalStrength);
            #endif

            // Shade
            float ln1 = saturate(dot(fd.L,N1)*0.5+0.5);
            float ln2 = saturate(dot(fd.L,N2)*0.5+0.5);
            if(Exists_ShadowBorderMask)
            {
                float4 shadowBorderMask = LIL_SAMPLE_2D(_ShadowBorderMask, samp, fd.uvMain);
                ln1 *= saturate(shadowBorderMask.r * _ShadowAOShift.x + _ShadowAOShift.y);
                ln2 *= saturate(shadowBorderMask.g * _ShadowAOShift.z + _ShadowAOShift.w);
            }

            // Shadow
            #if (defined(LIL_USE_SHADOW) || defined(LIL_LIGHTMODE_SHADOWMASK)) && defined(LIL_FEATURE_RECEIVE_SHADOW)
                if(_ShadowReceive) ln1 *= saturate(fd.attenuation + distance(fd.L, fd.origL));
            #endif

            float lnB = ln1;

            // Toon
            float shadowBlur = _ShadowBlur;
            if(Exists_ShadowBlurMask) shadowBlur *= LIL_SAMPLE_2D(_ShadowBlurMask, samp, fd.uvMain).r;
            ln1 = lilTooning(ln1, _ShadowBorder, shadowBlur);
            ln2 = lilTooning(ln2, _Shadow2ndBorder, _Shadow2ndBlur);
            lnB = lilTooning(lnB, _ShadowBorder, shadowBlur, _ShadowBorderRange);

            // Force shadow on back face
            float bfshadow = (fd.facing < 0.0) ? 1.0 - _BackfaceForceShadow : 1.0;
            ln1 *= bfshadow;
            ln2 *= bfshadow;
            lnB *= bfshadow;

            // Copy
            fd.shadowmix = ln1;

            // Strength
            float shadowStrength = _ShadowStrength;
            #ifdef LIL_COLORSPACE_GAMMA
                shadowStrength = lilSRGBToLinear(shadowStrength);
            #endif
            if(Exists_ShadowStrengthMask) shadowStrength *= LIL_SAMPLE_2D(_ShadowStrengthMask, samp, fd.uvMain).r;
            ln1 = lerp(1.0, ln1, shadowStrength);

            // Shadow Color 1
            float4 shadowColorTex = 0.0;
            if(Exists_ShadowColorTex) shadowColorTex = LIL_SAMPLE_2D(_ShadowColorTex, samp, fd.uvMain);
            float3 indirectCol = lerp(fd.albedo, shadowColorTex.rgb, shadowColorTex.a) * _ShadowColor.rgb;
            // Shadow Color 2
            float4 shadow2ndColorTex = 0.0;
            if(Exists_Shadow2ndColorTex) shadow2ndColorTex = LIL_SAMPLE_2D(_Shadow2ndColorTex, samp, fd.uvMain);
            shadow2ndColorTex.rgb = lerp(fd.albedo, shadow2ndColorTex.rgb, shadow2ndColorTex.a) * _Shadow2ndColor.rgb;
            ln2 = _Shadow2ndColor.a - ln2 * _Shadow2ndColor.a;
            indirectCol = lerp(indirectCol, shadow2ndColorTex.rgb, ln2);
            // Multiply Main Color
            indirectCol = lerp(indirectCol, indirectCol*fd.albedo, _ShadowMainStrength);

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
#elif defined(LIL_LITE)
    void lilGetShading(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        LIL_BRANCH
        if(_UseShadow)
        {
            // Shade
            float ln1 = saturate(fd.ln*0.5+0.5);
            float ln2 = ln1;
            float lnB = ln1;

            // Toon
            ln1 = lilTooning(ln1, _ShadowBorder, _ShadowBlur);
            ln2 = lilTooning(ln2, _Shadow2ndBorder, _Shadow2ndBlur);
            lnB = lilTooning(lnB, _ShadowBorder, _ShadowBlur, _ShadowBorderRange);

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
// Backlight
#if defined(LIL_FEATURE_BACKLIGHT) && !defined(LIL_LITE) && !defined(LIL_FUR) && !defined(LIL_GEM)
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
            float3 backlightColor = LIL_SAMPLE_2D_ST(_BacklightColorTex, samp, fd.uvMain).rgb * _BacklightColor.rgb;

            // Factor
            float backlightFactor = pow(saturate(-fd.hl * 0.5 + 0.5), _BacklightDirectivity);
            float backlightLN = dot(normalize(-fd.headV * _BacklightViewStrength + fd.L), N) * 0.5 + 0.5;
            #if defined(LIL_USE_SHADOW) || defined(LIL_LIGHTMODE_SHADOWMASK)
                if(_BacklightReceiveShadow) backlightLN *= saturate(fd.attenuation + distance(fd.L, fd.origL));
            #endif
            backlightLN = lilTooning(backlightLN, _BacklightBorder, _BacklightBlur);
            float backlight = saturate(backlightFactor * backlightLN);
            backlight = fd.facing < (_BacklightBackfaceMask-1.0) ? 0.0 : backlight;

            // Blend
            fd.col.rgb += backlight * backlightColor * fd.lightColor;
        }
    }
#endif

#if !defined(OVERRIDE_BACKLIGHT)
    #define OVERRIDE_BACKLIGHT lilBacklight(fd LIL_SAMP_IN(sampler_MainTex));
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Refraction
#if defined(LIL_REFRACTION) && !defined(LIL_LITE) && !defined(LIL_FUR)
    void lilRefraction(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        float2 refractUV = fd.uvScn + (pow(1.0 - fd.nv, _RefractionFresnelPower) * _RefractionStrength) * mul((float3x3)LIL_MATRIX_V, fd.N).xy;
        #if defined(LIL_REFRACTION_BLUR2) && defined(LIL_FEATURE_REFLECTION)
            #if defined(LIL_BRP)
                float3 refractCol = 0;
                float sum = 0;
                float blurOffset = fd.perceptualRoughness / fd.positionSS.z * (0.0005 / LIL_REFRACTION_SAMPNUM);
                for(int j = -16; j <= 16; j++)
                {
                    refractCol += LIL_GET_GRAB_TEX(refractUV + float2(0,j*blurOffset), 0).rgb * LIL_REFRACTION_GAUSDIST(j);
                    sum += LIL_REFRACTION_GAUSDIST(j);
                }
                refractCol /= sum;
                refractCol *= _RefractionColor.rgb;
            #else
                float refractLod = min(sqrt(fd.perceptualRoughness / fd.positionSS.z * 0.05), 10);
                float3 refractCol = LIL_GET_GRAB_TEX(refractUV, refractLod).rgb * _RefractionColor.rgb;
            #endif
        #elif defined(LIL_REFRACTION_BLUR2)
            float3 refractCol = LIL_GET_GRAB_TEX(refractUV,0).rgb * _RefractionColor.rgb;
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
#if defined(LIL_FEATURE_REFLECTION) && defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) && !defined(LIL_LITE) && !defined(LIL_FUR)
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
        return lilTooning(pow(nh,1.0/fd.roughness), _SpecularBorder, _SpecularBlur);

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

                float anisotropyShiftNoise = LIL_SAMPLE_2D_ST(_AnisotropyShiftNoiseMask, samp, fd.uvMain).r - 0.5;
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
            if(_SpecularToon) return lilTooning(specularTerm, 0.5);
        #endif
        return specularTerm * lilFresnelTerm(specular, lh);
    }

    void lilReflection(inout lilFragData fd LIL_SAMP_IN_FUNC(samp) LIL_HDRP_POSITION_INPUT_ARGS)
    {
        #if defined(LIL_PASS_FORWARDADD)
            LIL_BRANCH
            if(_UseReflection && _ApplySpecular && _ApplySpecularFA)
        #else
            LIL_BRANCH
            if(_UseReflection)
        #endif
        {
            float3 reflectCol = 0;
            // Smoothness
            #if !defined(LIL_REFRACTION_BLUR2) || defined(LIL_PASS_FORWARDADD)
                fd.smoothness = _Smoothness;
                if(Exists_SmoothnessTex) fd.smoothness *= LIL_SAMPLE_2D_ST(_SmoothnessTex, samp, fd.uvMain).r;
                fd.perceptualRoughness = fd.perceptualRoughness - fd.smoothness * fd.perceptualRoughness;
                fd.roughness = fd.perceptualRoughness * fd.perceptualRoughness;
            #endif
            // Metallic
            float metallic = _Metallic;
            if(Exists_MetallicGlossMap) metallic *= LIL_SAMPLE_2D_ST(_MetallicGlossMap, samp, fd.uvMain).r;
            fd.col.rgb = fd.col.rgb - metallic * fd.col.rgb;
            float3 specular = lerp(_Reflectance, fd.albedo, metallic);
            // Specular
            #if !defined(LIL_PASS_FORWARDADD)
                LIL_BRANCH
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
                    reflectCol = lilCalcSpecular(fd, lightDirectionSpc, specular, fd.attenuation LIL_SAMP_IN(samp)) * lightColorSpc;
                #elif defined(SHADOWS_SCREEN)
                    reflectCol = lilCalcSpecular(fd, lightDirectionSpc, specular, fd.shadowmix LIL_SAMP_IN(samp)) * lightColorSpc;
                #else
                    reflectCol = lilCalcSpecular(fd, lightDirectionSpc, specular, 1.0 LIL_SAMP_IN(samp)) * lightColorSpc;
                #endif
            }
            // Reflection
            #if !defined(LIL_PASS_FORWARDADD)
                LIL_BRANCH
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
                        reflectCol += fd.col.a * surfaceReduction * envReflectionColor * lilFresnelLerp(specular, grazingTerm, fd.nv);
                        fd.col.a = 1.0;
                    #else
                        reflectCol += surfaceReduction * envReflectionColor * lilFresnelLerp(specular, grazingTerm, fd.nv);
                    #endif
                }
            #endif
            // Mix
            float4 reflectionColor = _ReflectionColor;
            if(Exists_ReflectionColorTex) reflectionColor *= LIL_SAMPLE_2D_ST(_ReflectionColorTex, samp, fd.uvMain);
            #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                if(_ReflectionApplyTransparency) reflectionColor.a *= fd.col.a;
            #endif
            fd.col.rgb += reflectionColor.rgb * reflectionColor.a * reflectCol;
        }
    }
#endif

#if !defined(OVERRIDE_REFLECTION)
    #define OVERRIDE_REFLECTION \
        lilReflection(fd LIL_SAMP_IN(sampler_MainTex) LIL_HDRP_POSITION_INPUT_VAR);
#endif

//------------------------------------------------------------------------------------------------------------------------------
// MatCap
#if defined(LIL_FEATURE_MATCAP) && !defined(LIL_LITE) && !defined(LIL_FUR)
    void lilGetMatCap(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        LIL_BRANCH
        if(_UseMatCap)
        {
            // Normal
            float3 N = fd.matcapN;
            #if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)
                N = lerp(fd.origN, fd.matcapN, _MatCapNormalStrength);
            #endif
            #if defined(LIL_FEATURE_TEX_MATCAP_NORMALMAP)
                LIL_BRANCH
                if(_MatCapCustomNormal)
                {
                    float4 normalTex = LIL_SAMPLE_2D_ST(_MatCapBumpMap, samp, fd.uvMain);
                    float3 normalmap = UnpackNormalScale(normalTex, _MatCapBumpScale);
                    N = normalize(mul(normalmap, fd.TBN));
                    N = fd.facing < (_FlipNormal-1.0) ? -N : N;
                }
            #endif

            // UV
            float2 matUV = lilCalcMatCapUV(fd.uv1, normalize(N), fd.V, fd.headV, _MatCapTex_ST, _MatCapBlendUV1.xy, _MatCapZRotCancel, _MatCapPerspective, _MatCapVRParallaxStrength);

            // Color
            float4 matCapColor = _MatCapColor;
            if(Exists_MatCapTex) matCapColor *= LIL_SAMPLE_2D_LOD(_MatCapTex, sampler_linear_repeat, matUV, _MatCapLod);
            #if !defined(LIL_PASS_FORWARDADD)
                matCapColor.rgb = lerp(matCapColor.rgb, matCapColor.rgb * fd.lightColor, _MatCapEnableLighting);
                matCapColor.a = lerp(matCapColor.a, matCapColor.a * fd.shadowmix, _MatCapShadowMask);
            #else
                if(_MatCapBlendMode < 3) matCapColor.rgb *= fd.lightColor * _MatCapEnableLighting;
            #endif
            #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                if(_MatCapApplyTransparency) matCapColor.a *= fd.col.a;
            #endif
            matCapColor.a = fd.facing < (_MatCapBackfaceMask-1.0) ? 0.0 : matCapColor.a;
            float3 matCapMask = 1.0;
            if(Exists_MatCapBlendMask) matCapMask = LIL_SAMPLE_2D_ST(_MatCapBlendMask, samp, fd.uvMain).rgb;

            // Blend
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
#if defined(LIL_FEATURE_MATCAP_2ND) && !defined(LIL_LITE) && !defined(LIL_FUR)
    void lilGetMatCap2nd(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        LIL_BRANCH
        if(_UseMatCap2nd)
        {
            // Normal
            float3 N = fd.matcap2ndN;
            #if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)
                N = lerp(fd.origN, fd.matcap2ndN, _MatCap2ndNormalStrength);
            #endif
            #if defined(LIL_FEATURE_TEX_MATCAP_NORMALMAP)
                LIL_BRANCH
                if(_MatCap2ndCustomNormal)
                {
                    float4 normalTex = LIL_SAMPLE_2D_ST(_MatCap2ndBumpMap, samp, fd.uvMain);
                    float3 normalmap = UnpackNormalScale(normalTex, _MatCap2ndBumpScale);
                    N = normalize(mul(normalmap, fd.TBN));
                    N = fd.facing < (_FlipNormal-1.0) ? -N : N;
                }
            #endif

            // UV
            float2 mat2ndUV = lilCalcMatCapUV(fd.uv1, N, fd.V, fd.headV, _MatCap2ndTex_ST, _MatCap2ndBlendUV1.xy, _MatCap2ndZRotCancel, _MatCap2ndPerspective, _MatCap2ndVRParallaxStrength);

            // Color
            float4 matCap2ndColor = _MatCap2ndColor;
            if(Exists_MatCapTex) matCap2ndColor *= LIL_SAMPLE_2D_LOD(_MatCap2ndTex, sampler_linear_repeat, mat2ndUV, _MatCap2ndLod);
            #if !defined(LIL_PASS_FORWARDADD)
                matCap2ndColor.rgb = lerp(matCap2ndColor.rgb, matCap2ndColor.rgb * fd.lightColor, _MatCap2ndEnableLighting);
                matCap2ndColor.a = lerp(matCap2ndColor.a, matCap2ndColor.a * fd.shadowmix, _MatCap2ndShadowMask);
            #else
                if(_MatCap2ndBlendMode < 3) matCap2ndColor.rgb *= fd.lightColor * _MatCap2ndEnableLighting;
            #endif
            #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                if(_MatCap2ndApplyTransparency) matCap2ndColor.a *= fd.col.a;
            #endif
            matCap2ndColor.a = fd.facing < (_MatCap2ndBackfaceMask-1.0) ? 0.0 : matCap2ndColor.a;
            float3 matCapMask = 1.0;
            if(Exists_MatCap2ndBlendMask) matCapMask = LIL_SAMPLE_2D_ST(_MatCap2ndBlendMask, samp, fd.uvMain).r;

            // Blend
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
#if defined(LIL_FEATURE_RIMLIGHT) && !defined(LIL_LITE) && !defined(LIL_FUR)
    void lilGetRim(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        LIL_BRANCH
        if(_UseRim)
        {
            #if defined(LIL_FEATURE_RIMLIGHT_DIRECTION)
                // Color
                float4 rimColor = _RimColor;
                float4 rimIndirColor = _RimIndirColor;
                if(Exists_RimColorTex)
                {
                    float4 rimColorTex = LIL_SAMPLE_2D_ST(_RimColorTex, samp, fd.uvMain);
                    rimColor *= rimColorTex;
                    rimIndirColor *= rimColorTex;
                }

                // View direction
                #if defined(USING_STEREO_MATRICES)
                    float3 V = lerp(fd.headV, fd.V, _RimVRParallaxStrength);
                #else
                    float3 V = fd.V;
                #endif

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

                rimDir = lilTooning(rimDir, _RimBorder, _RimBlur);
                rimIndir = lilTooning(rimIndir, _RimIndirBorder, _RimIndirBlur);

                #if !defined(LIL_PASS_FORWARDADD)
                    rimDir = lerp(rimDir, rimDir * fd.shadowmix, _RimShadowMask);
                    rimIndir = lerp(rimIndir, rimIndir * fd.shadowmix, _RimShadowMask);
                #endif
                #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                    if(_RimApplyTransparency)
                    {
                        rimDir *= fd.col.a;
                        rimIndir *= fd.col.a;
                    }
                #endif

                // Blend
                float3 rimSum = rimDir * rimColor.a * rimColor.rgb + rimIndir * rimIndirColor.a * rimIndirColor.rgb;
                #if !defined(LIL_PASS_FORWARDADD)
                    rimSum = lerp(rimSum, rimSum * fd.lightColor, _RimEnableLighting);
                    fd.col.rgb += rimSum;
                #else
                    fd.col.rgb += rimSum * _RimEnableLighting * fd.lightColor;
                #endif
            #else
                // Color
                float4 rimColor = _RimColor;
                if(Exists_RimColorTex) rimColor *= LIL_SAMPLE_2D_ST(_RimColorTex, samp, fd.uvMain);

                // Normal
                float3 N = fd.N;
                #if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)
                    N = lerp(fd.origN, fd.N, _RimNormalStrength);
                #endif
                float nvabs = abs(dot(N,fd.V));

                // Factor
                float rim = pow(saturate(1.0 - nvabs), _RimFresnelPower);
                rim = fd.facing < (_RimBackfaceMask-1.0) ? 0.0 : rim;
                rim = lilTooning(rim, _RimBorder, _RimBlur);
                #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                    if(_RimApplyTransparency) rim *= fd.col.a;
                #endif

                // Blend
                #if !defined(LIL_PASS_FORWARDADD)
                    rim = lerp(rim, rim * fd.shadowmix, _RimShadowMask);
                    rimColor.rgb = lerp(rimColor.rgb, rimColor.rgb * fd.lightColor, _RimEnableLighting);
                    fd.col.rgb += rim * rimColor.a * rimColor.rgb;
                #else
                    fd.col.rgb += rim * _RimEnableLighting * rimColor.a * rimColor.rgb * fd.lightColor;
                #endif
            #endif
        }
    }
#elif defined(LIL_LITE)
    void lilGetRim(inout lilFragData fd)
    {
        LIL_BRANCH
        if(_UseRim)
        {
            float rim = pow(saturate(1.0 - fd.nvabs), _RimFresnelPower);
            rim = lilTooning(rim, _RimBorder, _RimBlur);
            #if !defined(LIL_PASS_FORWARDADD)
                rim = lerp(rim, rim * fd.shadowmix, _RimShadowMask);
            #endif
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
#if defined(LIL_FEATURE_GLITTER) && !defined(LIL_LITE) && !defined(LIL_FUR)
    void lilGlitter(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        LIL_BRANCH
        if(_UseGlitter)
        {
            // View direction
            #if defined(USING_STEREO_MATRICES)
                float3 glitterViewDirection = lerp(fd.headV, fd.V, _GlitterVRParallaxStrength);
            #else
                float3 glitterViewDirection = fd.V;
            #endif

            // Normal
            float3 N = fd.N;
            #if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)
                N = lerp(fd.origN, fd.N, _GlitterNormalStrength);
            #endif

            // Color
            float4 glitterColor = _GlitterColor;
            if(Exists_GlitterColorTex) glitterColor *= LIL_SAMPLE_2D_ST(_GlitterColorTex, samp, fd.uvMain);
            float2 glitterPos = _GlitterUVMode ? fd.uv1 : fd.uv0;
            glitterColor.rgb *= lilCalcGlitter(glitterPos, N, glitterViewDirection, fd.L, _GlitterParams1, _GlitterParams2);
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
#if defined(LIL_FEATURE_EMISSION_1ST) && !defined(LIL_LITE) && !defined(LIL_FUR)
    void lilEmission(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        LIL_BRANCH
        if(_UseEmission)
        {
            float4 emissionColor = _EmissionColor;
            // UV
            #if defined(LIL_FEATURE_ANIMATE_EMISSION_UV)
                float2 emissionUV = fd.uv0;
            #else
                float2 emissionUV = fd.uvMain;
            #endif
            if(_EmissionMap_UVMode == 1) emissionUV = fd.uv1;
            if(_EmissionMap_UVMode == 2) emissionUV = fd.uv2;
            if(_EmissionMap_UVMode == 3) emissionUV = fd.uv3;
            if(_EmissionMap_UVMode == 4) emissionUV = fd.uvRim;
            //if(_EmissionMap_UVMode == 4) emissionUV = fd.uvPanorama;
            float2 _EmissionMapParaTex = emissionUV + _EmissionParallaxDepth * fd.parallaxOffset;
            // Texture
            #if defined(LIL_FEATURE_ANIMATE_EMISSION_UV)
                if(Exists_EmissionMap) emissionColor *= LIL_GET_EMITEX(_EmissionMap, _EmissionMapParaTex);
            #else
                if(Exists_EmissionMap) emissionColor *= LIL_SAMPLE_2D_ST(_EmissionMap, sampler_EmissionMap, _EmissionMapParaTex);
            #endif
            // Mask
            #if defined(LIL_FEATURE_ANIMATE_EMISSION_MASK_UV)
                if(Exists_EmissionBlendMask) emissionColor *= LIL_GET_EMIMASK(_EmissionBlendMask, fd.uv0);
            #else
                if(Exists_EmissionBlendMask) emissionColor *= LIL_SAMPLE_2D_ST(_EmissionBlendMask, samp, fd.uvMain);
            #endif
            // Gradation
            #if defined(LIL_FEATURE_EMISSION_GRADATION) && defined(LIL_FEATURE_AUDIOLINK)
                if(Exists_EmissionGradTex && _EmissionUseGrad)
                {
                    float gradUV = _EmissionGradSpeed * LIL_TIME + fd.audioLinkValue * _AudioLink2EmissionGrad;
                    emissionColor *= LIL_SAMPLE_1D_LOD(_EmissionGradTex, sampler_linear_repeat, gradUV, 0);
                }
            #elif defined(LIL_FEATURE_EMISSION_GRADATION)
                if(Exists_EmissionGradTex && _EmissionUseGrad) emissionColor *= LIL_SAMPLE_1D(_EmissionGradTex, sampler_linear_repeat, _EmissionGradSpeed * LIL_TIME);
            #endif
            #if defined(LIL_FEATURE_AUDIOLINK)
                if(_AudioLink2Emission) emissionColor.a *= fd.audioLinkValue;
            #endif
            emissionColor.rgb = lerp(emissionColor.rgb, emissionColor.rgb * fd.invLighting, _EmissionFluorescence);
            fd.emissionColor += _EmissionBlend * lilCalcBlink(_EmissionBlink) * emissionColor.a * emissionColor.rgb;
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
#if defined(LIL_FEATURE_EMISSION_2ND) && !defined(LIL_LITE) && !defined(LIL_FUR)
    void lilEmission2nd(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        LIL_BRANCH
        if(_UseEmission2nd)
        {
            float4 emission2ndColor = _Emission2ndColor;
            // UV
            #if defined(LIL_FEATURE_ANIMATE_EMISSION_UV)
                float2 emission2ndUV = fd.uv0;
            #else
                float2 emission2ndUV = fd.uvMain;
            #endif
            if(_Emission2ndMap_UVMode == 1) emission2ndUV = fd.uv1;
            if(_Emission2ndMap_UVMode == 2) emission2ndUV = fd.uv2;
            if(_Emission2ndMap_UVMode == 3) emission2ndUV = fd.uv3;
            if(_Emission2ndMap_UVMode == 4) emission2ndUV = fd.uvRim;
            //if(_Emission2ndMap_UVMode == 4) emission2ndUV = fd.uvPanorama;
            float2 _Emission2ndMapParaTex = emission2ndUV + _Emission2ndParallaxDepth * fd.parallaxOffset;
            // Texture
            #if defined(LIL_FEATURE_ANIMATE_EMISSION_UV)
                if(Exists_Emission2ndMap) emission2ndColor *= LIL_GET_EMITEX(_Emission2ndMap, _Emission2ndMapParaTex);
            #else
                if(Exists_Emission2ndMap) emission2ndColor *= LIL_SAMPLE_2D_ST(_Emission2ndMap, sampler_Emission2ndMap, _Emission2ndMapParaTex);
            #endif
            // Mask
            #if defined(LIL_FEATURE_ANIMATE_EMISSION_MASK_UV)
                if(Exists_Emission2ndBlendMask) emission2ndColor *= LIL_GET_EMIMASK(_Emission2ndBlendMask, fd.uv0);
            #else
                if(Exists_Emission2ndBlendMask) emission2ndColor *= LIL_SAMPLE_2D_ST(_Emission2ndBlendMask, samp, fd.uvMain);
            #endif
            // Gradation
            #if defined(LIL_FEATURE_EMISSION_GRADATION) && defined(LIL_FEATURE_AUDIOLINK)
                if(Exists_Emission2ndGradTex && _Emission2ndUseGrad)
                {
                    float gradUV = _Emission2ndGradSpeed * LIL_TIME + fd.audioLinkValue * _AudioLink2Emission2ndGrad;
                    emission2ndColor *= LIL_SAMPLE_1D_LOD(_Emission2ndGradTex, sampler_linear_repeat, gradUV, 0);
                }
            #elif defined(LIL_FEATURE_EMISSION_GRADATION)
                if(Exists_Emission2ndGradTex && _Emission2ndUseGrad) emission2ndColor *= LIL_SAMPLE_1D(_Emission2ndGradTex, sampler_linear_repeat, _Emission2ndGradSpeed * LIL_TIME);
            #endif
            #if defined(LIL_FEATURE_AUDIOLINK)
                if(_AudioLink2Emission2nd) emission2ndColor.a *= fd.audioLinkValue;
            #endif
            emission2ndColor.rgb = lerp(emission2ndColor.rgb, emission2ndColor.rgb * fd.invLighting, _Emission2ndFluorescence);
            fd.emissionColor += _Emission2ndBlend * lilCalcBlink(_Emission2ndBlink) * emission2ndColor.a * emission2ndColor.rgb;
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
// Distance Fade
#if defined(LIL_FEATURE_DISTANCE_FADE) && !defined(LIL_LITE)
    void lilDistanceFade(inout lilFragData fd)
    {
        float distFade = saturate((fd.depth - _DistanceFade.x) / (_DistanceFade.y - _DistanceFade.x));
        distFade = fd.facing < (_DistanceFade.w-1.0) ? _DistanceFade.z : distFade * _DistanceFade.z;
        #if defined(LIL_PASS_FORWARDADD)
            fd.col.rgb = lerp(fd.col.rgb, 0.0, distFade);
        #elif LIL_RENDER == 2
            fd.col.rgb = lerp(fd.col.rgb, _DistanceFadeColor.rgb * _DistanceFadeColor.a, distFade);
            fd.col.a = lerp(fd.col.a, fd.col.a * _DistanceFadeColor.a, distFade);
        #else
            fd.col.rgb = lerp(fd.col.rgb, _DistanceFadeColor.rgb, distFade);
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