//------------------------------------------------------------------------------------------------------------------------------
// PS Macro

// Insert
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
// UV Animation
#if !defined(OVERRIDE_ANIMATE_MAIN_UV)
    #if defined(LIL_PASS_META_INCLUDED) && (defined(LIL_FEATURE_ANIMATE_MAIN_UV) || defined(LIL_LITE))
        #define OVERRIDE_ANIMATE_MAIN_UV \
            float2 uvMain = lilCalcDoubleSideUV(input.uv, facing, _ShiftBackfaceUV); \
            uvMain = lilCalcUVWithoutAnimation(uvMain, _MainTex_ST, _MainTex_ScrollRotate);
    #elif defined(LIL_FEATURE_ANIMATE_MAIN_UV) || defined(LIL_LITE)
        #define OVERRIDE_ANIMATE_MAIN_UV \
            float2 uvMain = lilCalcDoubleSideUV(input.uv, facing, _ShiftBackfaceUV); \
            uvMain = lilCalcUV(uvMain, _MainTex_ST, _MainTex_ScrollRotate);
    #elif !defined(LIL_PASS_FORWARD_FUR_INCLUDED)
        #define OVERRIDE_ANIMATE_MAIN_UV \
            float2 uvMain = lilCalcDoubleSideUV(input.uv, facing, _ShiftBackfaceUV); \
            uvMain = lilCalcUV(uvMain, _MainTex_ST);
    #else
        #define OVERRIDE_ANIMATE_MAIN_UV \
            float2 uvMain = lilCalcUV(input.uv, _MainTex_ST);
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Outline UV Animation
#if !defined(OVERRIDE_ANIMATE_OUTLINE_UV)
    #if defined(LIL_FEATURE_ANIMATE_OUTLINE_UV) || defined(LIL_LITE)
        #define OVERRIDE_ANIMATE_OUTLINE_UV \
            float2 uvMain = lilCalcUV(input.uv, _OutlineTex_ST, _OutlineTex_ScrollRotate);
    #else
        #define OVERRIDE_ANIMATE_OUTLINE_UV \
            float2 uvMain = lilCalcUV(input.uv, _OutlineTex_ST);
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Parallax
#if !defined(OVERRIDE_PARALLAX)
    #if defined(LIL_FEATURE_POM)
        #define OVERRIDE_PARALLAX \
            lilPOM(uvMain, input.uv, _UseParallax, _MainTex_ST, parallaxViewDirection, _ParallaxMap, _Parallax, _ParallaxOffset);
    #else
        #define OVERRIDE_PARALLAX \
            lilParallax(uvMain, input.uv, _UseParallax, parallaxOffset, _ParallaxMap, _Parallax, _ParallaxOffset);
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Main Texture
#if defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) && !defined(LIL_FUR)
    #define LIL_GET_MAIN_TEX \
        col = LIL_SAMPLE_2D_POM(_MainTex, sampler_MainTex, uvMain, ddxMain, ddyMain);

    // Tone correction
    #if defined(LIL_FEATURE_MAIN_TONE_CORRECTION)
        #define LIL_MAIN_TONECORRECTION \
            col.rgb = lilToneCorrection(col.rgb, _MainTexHSVG);
    #else
        #define LIL_MAIN_TONECORRECTION
    #endif

    // Gradation map
    #if defined(LIL_FEATURE_MAIN_GRADATION_MAP)
        #define LIL_MAIN_GRADATION_MAP \
            col.rgb = lilGradationMap(col.rgb, _MainGradationTex, _MainGradationStrength);
    #else
        #define LIL_MAIN_GRADATION_MAP
    #endif

    #if defined(LIL_FEATURE_MAIN_TONE_CORRECTION) || defined(LIL_FEATURE_MAIN_GRADATION_MAP)
        #define LIL_APPLY_MAIN_TONECORRECTION \
            float3 beforeToneCorrectionColor = col.rgb; \
            float colorAdjustMask = LIL_SAMPLE_2D(_MainColorAdjustMask, sampler_MainTex, uvMain).r; \
            LIL_MAIN_TONECORRECTION \
            LIL_MAIN_GRADATION_MAP \
            col.rgb = lerp(beforeToneCorrectionColor, col.rgb, colorAdjustMask);
    #else
        #define LIL_APPLY_MAIN_TONECORRECTION
    #endif
#else
    #define LIL_GET_MAIN_TEX \
        col = LIL_SAMPLE_2D(_MainTex, sampler_MainTex, uvMain);
    #define LIL_APPLY_MAIN_TONECORRECTION
#endif

#if !defined(OVERRIDE_MAIN)
    #define OVERRIDE_MAIN \
        LIL_GET_MAIN_TEX \
        LIL_APPLY_MAIN_TONECORRECTION \
        col *= _Color;
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Outline Color
#if defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) && defined(LIL_FEATURE_TEX_OUTLINE_COLOR) || !defined(LIL_PASS_FORWARD_NORMAL_INCLUDED)
    #define LIL_GET_OUTLINE_TEX \
        col = LIL_SAMPLE_2D(_OutlineTex, sampler_OutlineTex, uvMain);
#else
    #define LIL_GET_OUTLINE_TEX
#endif

#if defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) && defined(LIL_FEATURE_OUTLINE_TONE_CORRECTION)
    #define LIL_APPLY_OUTLINE_TONECORRECTION \
        col.rgb = lilToneCorrection(col.rgb, _OutlineTexHSVG);
#else
    #define LIL_APPLY_OUTLINE_TONECORRECTION
#endif

#if !defined(OVERRIDE_OUTLINE_COLOR)
    #define OVERRIDE_OUTLINE_COLOR \
        LIL_GET_OUTLINE_TEX \
        LIL_APPLY_OUTLINE_TONECORRECTION \
        col *= _OutlineColor;
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
        col.rgb *= furLayer * _FurAO * 2.0 + 1.0 - _FurAO;
#else
    #if defined(LIL_ONEPASS_FUR)
        #define LIL_FUR_LAYER_ALPHA \
            furAlpha = input.furLayer < -1.5 ? 1.0 : saturate(furAlpha - input.furLayer * furLayer * furLayer);
    #else
        #define LIL_FUR_LAYER_ALPHA \
            furAlpha = saturate(furAlpha - input.furLayer * furLayer * furLayer);
    #endif
    #define LIL_FUR_LAYER_AO \
        col.rgb *= (1.0-furAlpha) * _FurAO * 1.25 + 1.0 - _FurAO;
#endif

#if defined(LIL_ALPHA_PS)
    #undef LIL_FUR_LAYER_AO
    #define LIL_FUR_LAYER_AO
#endif

#if !defined(OVERRIDE_FUR)
    #define OVERRIDE_FUR \
        float furAlpha = 1.0; \
        float furLayer = abs(input.furLayer); \
        if(Exists_FurNoiseMask) furAlpha = LIL_SAMPLE_2D_ST(_FurNoiseMask, sampler_MainTex, input.uv).r; \
        if(Exists_FurMask) furAlpha *= LIL_SAMPLE_2D(_FurMask, sampler_MainTex, uvMain).r; \
        LIL_FUR_LAYER_ALPHA \
        col.a *= furAlpha; \
        LIL_FUR_LAYER_AO
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Alpha Mask
#if !defined(OVERRIDE_ALPHAMASK)
    #define OVERRIDE_ALPHAMASK \
        if(_AlphaMaskMode) \
        { \
            float alphaMask = LIL_SAMPLE_2D(_AlphaMask, sampler_MainTex, uvMain).r; \
            alphaMask = saturate(alphaMask + _AlphaMaskValue); \
            col.a = _AlphaMaskMode == 1 ? alphaMask : col.a * alphaMask; \
        }
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Dissolve
#if !defined(OVERRIDE_DISSOLVE)
    #if defined(LIL_FEATURE_TEX_DISSOLVE_NOISE)
        #define OVERRIDE_DISSOLVE \
            lilCalcDissolveWithNoise( \
                col.a, \
                dissolveAlpha, \
                input.uv, \
                input.positionOS, \
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
                col.a, \
                dissolveAlpha, \
                input.uv, \
                input.positionOS, \
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
            float4 normalTex = LIL_SAMPLE_2D_ST(_BumpMap, sampler_MainTex, uvMain); \
            normalmap = UnpackNormalScale(normalTex, _BumpScale); \
        }
#endif

#if !defined(OVERRIDE_NORMAL_2ND)
    #define OVERRIDE_NORMAL_2ND \
        LIL_BRANCH \
        if(Exists_Bump2ndMap && _UseBump2ndMap) \
        { \
            float4 normal2ndTex = LIL_SAMPLE_2D_ST(_Bump2ndMap, sampler_MainTex, uvMain); \
            float bump2ndScale = _Bump2ndScale; \
            if(Exists_Bump2ndScaleMask) bump2ndScale *= LIL_SAMPLE_2D(_Bump2ndScaleMask, sampler_MainTex, uvMain).r; \
            normalmap = lilBlendNormal(normalmap, UnpackNormalScale(normal2ndTex, bump2ndScale)); \
        }
#endif

//------------------------------------------------------------------------------------------------------------------------------
// AudioLink
#if !defined(OVERRIDE_AUDIOLINK)
    #if defined(LIL_FEATURE_AUDIOLINK_LOCAL)
        #define OVERRIDE_AUDIOLINK \
            lilAudioLinkFrag(audioLinkValue, uvMain, input.uv, nv, _UseAudioLink, _AudioLinkUVMode, _AudioLinkUVParams, _AudioLinkMask, _AudioTexture_TexelSize, _AudioTexture, _AudioLinkAsLocal, _AudioLinkLocalMapParams, _AudioLinkLocalMap LIL_SAMP_IN(sampler_MainTex));
    #else
        #define OVERRIDE_AUDIOLINK \
            lilAudioLinkFrag(audioLinkValue, uvMain, input.uv, nv, _UseAudioLink, _AudioLinkUVMode, _AudioLinkUVParams, _AudioLinkMask, _AudioTexture_TexelSize, _AudioTexture LIL_SAMP_IN(sampler_MainTex));
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Main 2nd
#if defined(LIL_FEATURE_MAIN2ND) && defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) && !defined(LIL_LITE) && !defined(LIL_FUR)
    #if defined(LIL_V2F_POSITION_OS)
        void lilGetMain2nd(inout float4 col, inout float4 color2nd, inout float main2ndDissolveAlpha, float2 uvMain, float2 uv, float nv, float3 positionOS, float audioLinkValue, float depth, bool isRightHand LIL_SAMP_IN_FUNC(samp))
    #else
        void lilGetMain2nd(inout float4 col, inout float4 color2nd, inout float main2ndDissolveAlpha, float2 uvMain, float2 uv, float nv, float audioLinkValue, float depth, bool isRightHand LIL_SAMP_IN_FUNC(samp))
    #endif
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
            if(Exists_Main2ndTex) color2nd *= LIL_GET_SUBTEX(_Main2ndTex, uv);
            if(Exists_Main2ndBlendMask) color2nd.a *= LIL_SAMPLE_2D(_Main2ndBlendMask, samp, uvMain).r;
            #if defined(LIL_FEATURE_LAYER_DISSOLVE)
                #if defined(LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE)
                    lilCalcDissolveWithNoise(
                        color2nd.a,
                        main2ndDissolveAlpha,
                        uv,
                        positionOS,
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
                        uv,
                        positionOS,
                        _Main2ndDissolveParams,
                        _Main2ndDissolvePos,
                        _Main2ndDissolveMask,
                        _Main2ndDissolveMask_ST,
                        samp
                    );
                #endif
            #endif
            #if defined(LIL_FEATURE_AUDIOLINK)
                if(_AudioLink2Main2nd) color2nd.a *= audioLinkValue;
            #endif
            color2nd.a = lerp(color2nd.a, color2nd.a * saturate((depth - _Main2ndDistanceFade.x) / (_Main2ndDistanceFade.y - _Main2ndDistanceFade.x)), _Main2ndDistanceFade.z);
            col.rgb = lilBlendColor(col.rgb, color2nd.rgb, color2nd.a * _Main2ndEnableLighting, _Main2ndTexBlendMode);
        }
    }
#endif

#if !defined(OVERRIDE_MAIN2ND)
    #if defined(LIL_V2F_POSITION_OS)
        #define OVERRIDE_MAIN2ND \
            lilGetMain2nd(col, color2nd, main2ndDissolveAlpha, uvMain, input.uv, nv, input.positionOS, audioLinkValue, depth, isRightHand LIL_SAMP_IN(sampler_MainTex));
    #else
        #define OVERRIDE_MAIN2ND \
            lilGetMain2nd(col, color2nd, main2ndDissolveAlpha, uvMain, input.uv, nv, audioLinkValue, depth, isRightHand LIL_SAMP_IN(sampler_MainTex));
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Main 3rd
#if defined(LIL_FEATURE_MAIN3RD) && defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) && !defined(LIL_LITE) && !defined(LIL_FUR)
    #if defined(LIL_V2F_POSITION_OS)
        void lilGetMain3rd(inout float4 col, inout float4 color3rd, inout float main3rdDissolveAlpha, float2 uvMain, float2 uv, float nv, float3 positionOS, float audioLinkValue, float depth, bool isRightHand LIL_SAMP_IN_FUNC(samp))
    #else
        void lilGetMain3rd(inout float4 col, inout float4 color3rd, inout float main3rdDissolveAlpha, float2 uvMain, float2 uv, float nv, float audioLinkValue, float depth, bool isRightHand LIL_SAMP_IN_FUNC(samp))
    #endif
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
            if(Exists_Main3rdTex) color3rd *= LIL_GET_SUBTEX(_Main3rdTex, uv);
            if(Exists_Main3rdBlendMask) color3rd.a *= LIL_SAMPLE_2D(_Main3rdBlendMask, samp, uvMain).r;
            #if defined(LIL_FEATURE_LAYER_DISSOLVE)
                #if defined(LIL_FEATURE_TEX_LAYER_DISSOLVE_NOISE)
                    lilCalcDissolveWithNoise(
                        color3rd.a,
                        main3rdDissolveAlpha,
                        uv,
                        positionOS,
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
                        uv,
                        positionOS,
                        _Main3rdDissolveParams,
                        _Main3rdDissolvePos,
                        _Main3rdDissolveMask,
                        _Main3rdDissolveMask_ST,
                        samp
                    );
                #endif
            #endif
            #if defined(LIL_FEATURE_AUDIOLINK)
                if(_AudioLink2Main3rd) color3rd.a *= audioLinkValue;
            #endif
            color3rd.a = lerp(color3rd.a, color3rd.a * saturate((depth - _Main3rdDistanceFade.x) / (_Main3rdDistanceFade.y - _Main3rdDistanceFade.x)), _Main3rdDistanceFade.z);
            col.rgb = lilBlendColor(col.rgb, color3rd.rgb, color3rd.a * _Main3rdEnableLighting, _Main3rdTexBlendMode);
        }
    }
#endif

#if !defined(OVERRIDE_MAIN3RD)
    #if defined(LIL_V2F_POSITION_OS)
        #define OVERRIDE_MAIN3RD \
            lilGetMain3rd(col, color3rd, main3rdDissolveAlpha, uvMain, input.uv, nv, input.positionOS, audioLinkValue, depth, isRightHand LIL_SAMP_IN(sampler_MainTex));
    #else
        #define OVERRIDE_MAIN3RD \
            lilGetMain3rd(col, color3rd, main3rdDissolveAlpha, uvMain, input.uv, nv, audioLinkValue, depth, isRightHand LIL_SAMP_IN(sampler_MainTex));
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Shadow
#if defined(LIL_FEATURE_SHADOW) && !defined(LIL_LITE) && !defined(LIL_GEM)
void lilGetShading(
    inout float4 col,
    inout float shadowmix,
    float3 albedo,
    float3 lightColor,
    float3 indLightColor,
    float2 uv,
    float facing,
    float ln,
    float3 lightDirection,
    float3 lightDirectionCopy,
    bool cullOff,
    float attenuation
    LIL_SAMP_IN_FUNC(samp))
{
    LIL_BRANCH
    if(_UseShadow)
    {
        // Shade
        float ln1 = saturate(ln*0.5+0.5);
        if(Exists_ShadowBorderMask) ln1 *= LIL_SAMPLE_2D(_ShadowBorderMask, samp, uv).r;
        float ln2 = ln1;
        float lnB = ln1;

        // Shadow
        #if (defined(LIL_USE_SHADOW) || defined(LIL_LIGHTMODE_SHADOWMASK)) && defined(LIL_FEATURE_RECEIVE_SHADOW)
            float shadowAttenuation = saturate(attenuation + distance(lightDirection, lightDirectionCopy));
            if(_ShadowReceive) ln1 *= shadowAttenuation;
            if(_ShadowReceive) lnB *= shadowAttenuation;
        #endif

        // Toon
        float shadowBlur = _ShadowBlur;
        if(Exists_ShadowBlurMask) shadowBlur *= LIL_SAMPLE_2D(_ShadowBlurMask, samp, uv).r;
        ln1 = lilTooning(ln1, _ShadowBorder, shadowBlur);
        ln2 = lilTooning(ln2, _Shadow2ndBorder, _Shadow2ndBlur);
        lnB = lilTooning(lnB, _ShadowBorder, shadowBlur, _ShadowBorderRange);

        if(cullOff)
        {
            // Force shadow on back face
            float bfshadow = (facing < 0.0) ? 1.0 - _BackfaceForceShadow : 1.0;
            ln1 *= bfshadow;
            ln2 *= bfshadow;
            lnB *= bfshadow;
        }

        // Copy
        shadowmix = ln1;

        // Strength
        float shadowStrength = _ShadowStrength;
        #ifdef LIL_COLORSPACE_GAMMA
            shadowStrength = lilSRGBToLinear(shadowStrength);
        #endif
        if(Exists_ShadowStrengthMask) shadowStrength *= LIL_SAMPLE_2D(_ShadowStrengthMask, samp, uv).r;
        ln1 = lerp(1.0, ln1, shadowStrength);

        // Shadow Color 1
        float4 shadowColorTex = 0.0;
        if(Exists_ShadowColorTex) shadowColorTex = LIL_SAMPLE_2D(_ShadowColorTex, samp, uv);
        float3 indirectCol = lerp(albedo, shadowColorTex.rgb, shadowColorTex.a) * _ShadowColor.rgb;
        // Shadow Color 2
        float4 shadow2ndColorTex = 0.0;
        if(Exists_Shadow2ndColorTex) shadow2ndColorTex = LIL_SAMPLE_2D(_Shadow2ndColorTex, samp, uv);
        shadow2ndColorTex.rgb = lerp(albedo, shadow2ndColorTex.rgb, shadow2ndColorTex.a) * _Shadow2ndColor.rgb;
        ln2 = _Shadow2ndColor.a - ln2 * _Shadow2ndColor.a;
        indirectCol = lerp(indirectCol, shadow2ndColorTex.rgb, ln2);
        // Multiply Main Color
        indirectCol = lerp(indirectCol, indirectCol*albedo, _ShadowMainStrength);

        // Apply Light
        float3 directCol = albedo * lightColor;
        indirectCol = indirectCol * lightColor;

        // Environment Light
        indirectCol = lerp(indirectCol, albedo, indLightColor);
        // Fix
        indirectCol = min(indirectCol, directCol);
        // Gradation
        indirectCol = lerp(indirectCol, directCol, lnB * _ShadowBorderColor.rgb);

        // Mix
        col.rgb = lerp(indirectCol, directCol, ln1);
    }
    else
    {
        col.rgb *= lightColor;
    }
}
#elif defined(LIL_LITE)
void lilGetShading(
    inout float4 col,
    inout float shadowmix,
    float3 albedo,
    float3 lightColor,
    float3 indLightColor,
    float2 uv,
    float facing,
    float ln,
    float3 lightDirection,
    float3 lightDirectionCopy,
    bool cullOff
    LIL_SAMP_IN_FUNC(samp))
{
    LIL_BRANCH
    if(_UseShadow)
    {
        // Shade
        float ln1 = saturate(ln*0.5+0.5);
        float ln2 = ln1;
        float lnB = ln1;

        // Toon
        ln1 = lilTooning(ln1, _ShadowBorder, _ShadowBlur);
        ln2 = lilTooning(ln2, _Shadow2ndBorder, _Shadow2ndBlur);
        lnB = lilTooning(lnB, _ShadowBorder, _ShadowBlur, _ShadowBorderRange);

        if(cullOff)
        {
            // Force shadow on back face
            float bfshadow = (facing < 0.0) ? 1.0 - _BackfaceForceShadow : 1.0;
            ln1 *= bfshadow;
            ln2 *= bfshadow;
            lnB *= bfshadow;
        }

        // Copy
        shadowmix = ln1;

        // Shadow Color 1
        float4 shadowColorTex = LIL_SAMPLE_2D(_ShadowColorTex, samp, uv);
        float3 indirectCol = lerp(albedo, shadowColorTex.rgb, shadowColorTex.a);
        // Shadow Color 2
        float4 shadow2ndColorTex = LIL_SAMPLE_2D(_Shadow2ndColorTex, samp, uv);
        indirectCol = lerp(indirectCol, shadow2ndColorTex.rgb, shadow2ndColorTex.a - ln2 * shadow2ndColorTex.a);

        // Apply Light
        float3 directCol = albedo * lightColor;
        indirectCol = indirectCol * lightColor;

        // Environment Light
        indirectCol = lerp(indirectCol, albedo, indLightColor);
        // Fix
        indirectCol = min(indirectCol, directCol);
        // Gradation
        indirectCol = lerp(indirectCol, directCol, lnB * _ShadowBorderColor.rgb);

        // Mix
        col.rgb = lerp(indirectCol, directCol, ln1);
    }
    else
    {
        col.rgb *= lightColor;
    }
}
#endif

#if !defined(OVERRIDE_SHADOW)
    #if defined(LIL_LITE)
        #define OVERRIDE_SHADOW \
            lilGetShading(col,shadowmix,albedo,lightColor,input.indLightColor,uvMain,facing,ln,lightDirection,LIL_LIGHTDIRECTION_ORIG,true LIL_SAMP_IN(sampler_MainTex));
    #else
        #define OVERRIDE_SHADOW \
            lilGetShading(col,shadowmix,albedo,lightColor,input.indLightColor,uvMain,facing,ln,lightDirection,LIL_LIGHTDIRECTION_ORIG,true,attenuation LIL_SAMP_IN(sampler_MainTex));
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Backlight
#if defined(LIL_FEATURE_BACKLIGHT) && !defined(LIL_LITE) && !defined(LIL_FUR) && !defined(LIL_GEM)
    void lilBacklight(inout float4 col, float2 uvMain, float hl, float3 lightColor, float3 lightDirection, float3 lightDirectionCopy, float attenuation, float3 headDirection, float3 normalDirection LIL_SAMP_IN_FUNC(samp))
    {
        if(_UseBacklight)
        {
            float3 backlightColor = LIL_SAMPLE_2D(_BacklightColorTex, samp, uvMain).rgb * _BacklightColor.rgb;
            float backlightFactor = pow(saturate(-hl * 0.5 + 0.5), _BacklightDirectivity);
            float backlightLN = dot(normalize(-headDirection * _BacklightViewStrength + lightDirection), normalDirection) * 0.5 + 0.5;
            #if defined(LIL_USE_SHADOW) || defined(LIL_LIGHTMODE_SHADOWMASK)
                if(_BacklightReceiveShadow) backlightLN *= saturate(attenuation + distance(lightDirection, lightDirectionCopy));
            #endif
            backlightLN = lilTooning(backlightLN, _BacklightBorder, _BacklightBlur);
            float backlight = backlightFactor * backlightLN;
            col.rgb += backlight * backlightColor * lightColor;
        }
    }
#endif

#if !defined(OVERRIDE_BACKLIGHT)
    #define OVERRIDE_BACKLIGHT lilBacklight(col, uvMain, hl, lightColor, lightDirection, LIL_LIGHTDIRECTION_ORIG, attenuation, headDirection, normalDirection LIL_SAMP_IN(sampler_MainTex));
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Refraction
#if defined(LIL_REFRACTION) && !defined(LIL_LITE) && !defined(LIL_FUR)
    #if (defined(LIL_REFRACTION_BLUR) || defined(LIL_REFRACTION_BLUR2)) && defined(LIL_FEATURE_REFLECTION)
        void lilRefraction(inout float4 col, float3 albedo, float4 positionSS, float nv, float3 normalDirection, float perceptualRoughness LIL_SAMP_IN_FUNC(samp))
    #else
        void lilRefraction(inout float4 col, float3 albedo, float4 positionSS, float nv, float3 normalDirection LIL_SAMP_IN_FUNC(samp))
    #endif
    {
        float2 scnUV = positionSS.xy/positionSS.w;
        float2 refractUV = scnUV + (pow(1.0 - nv, _RefractionFresnelPower) * _RefractionStrength) * mul((float3x3)LIL_MATRIX_V, normalDirection).xy;
        #if defined(LIL_REFRACTION_BLUR2) && defined(LIL_FEATURE_REFLECTION)
            #if defined(LIL_BRP)
                float3 refractCol = 0;
                float sum = 0;
                float blurOffset = perceptualRoughness / positionSS.z * (0.0005 / LIL_REFRACTION_SAMPNUM);
                for(int j = -16; j <= 16; j++)
                {
                    refractCol += LIL_GET_GRAB_TEX(refractUV + float2(0,j*blurOffset), 0).rgb * LIL_REFRACTION_GAUSDIST(j);
                    sum += LIL_REFRACTION_GAUSDIST(j);
                }
                refractCol /= sum;
                refractCol *= _RefractionColor.rgb;
            #else
                float refractLod = min(sqrt(perceptualRoughness / positionSS.z * 0.05), 10);
                float3 refractCol = LIL_GET_GRAB_TEX(refractUV, refractLod).rgb * _RefractionColor.rgb;
            #endif
        #elif defined(LIL_REFRACTION_BLUR2)
            float3 refractCol = LIL_GET_GRAB_TEX(refractUV,0).rgb * _RefractionColor.rgb;
        #else
            float3 refractCol = LIL_GET_BG_TEX(refractUV,0).rgb * _RefractionColor.rgb;
        #endif
        if(_RefractionColorFromMain) refractCol *= albedo;
        col.rgb = lerp(refractCol, col.rgb, col.a);
    }
#endif

#if !defined(OVERRIDE_REFRACTION)
    #if (defined(LIL_REFRACTION_BLUR) || defined(LIL_REFRACTION_BLUR2)) && defined(LIL_FEATURE_REFLECTION)
        #define OVERRIDE_REFRACTION \
            lilRefraction(col, albedo, input.positionSS, nv, normalDirection, perceptualRoughness LIL_SAMP_IN(sampler_MainTex));
    #else
        #define OVERRIDE_REFRACTION \
            lilRefraction(col, albedo, input.positionSS, nv, normalDirection LIL_SAMP_IN(sampler_MainTex));
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Reflection
#if defined(LIL_HDRP)
    #define LIL_HDRP_VAL_INPUT          , posInput, renderingLayers, featureFlags
    #define LIL_HDRP_VAL_INPUT_FUNC     , PositionInputs posInput, uint renderingLayers, uint featureFlags
#else
    #define LIL_HDRP_VAL_INPUT
    #define LIL_HDRP_VAL_INPUT_FUNC
#endif
#if defined(LIL_FEATURE_REFLECTION) && defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) && !defined(LIL_LITE) && !defined(LIL_FUR)
    #if !defined(LIL_REFRACTION_BLUR2) || defined(LIL_PASS_FORWARDADD)
        void lilReflection(inout float4 col, float2 uvMain, float3 albedo, float3 positionWS, float3 normalDirection, float3 viewDirection, float nv, float3 lightColor, float3 lightDirection, float shadowmix, float attenuation, float nvabs LIL_SAMP_IN_FUNC(samp) LIL_HDRP_VAL_INPUT_FUNC)
    #else
        void lilReflection(inout float4 col, float2 uvMain, float3 albedo, float3 positionWS, float3 normalDirection, float3 viewDirection, float nv, float3 lightColor, float3 lightDirection, float shadowmix, float attenuation, float nvabs, float smoothness, float perceptualRoughness, float roughness LIL_SAMP_IN_FUNC(samp) LIL_HDRP_VAL_INPUT_FUNC)
    #endif
    {
        #ifndef LIL_PASS_FORWARDADD
            LIL_BRANCH
            if(_UseReflection)
        #else
            LIL_BRANCH
            if(_UseReflection && _ApplySpecular)
        #endif
        {
            float3 reflectCol = 0;
            // Smoothness
            #if !defined(LIL_REFRACTION_BLUR2) || defined(LIL_PASS_FORWARDADD)
                float smoothness = _Smoothness;
                if(Exists_SmoothnessTex) smoothness *= LIL_SAMPLE_2D(_SmoothnessTex, samp, uvMain).r;
                float perceptualRoughness = 1.0 - smoothness;
                float roughness = perceptualRoughness * perceptualRoughness;
            #endif
            // Metallic
            float metallic = _Metallic;
            if(Exists_MetallicGlossMap) metallic *= LIL_SAMPLE_2D(_MetallicGlossMap, samp, uvMain).r;
            col.rgb = col.rgb - metallic * col.rgb;
            float3 specular = lerp(_Reflectance, albedo, metallic);
            // Specular
            #ifndef LIL_PASS_FORWARDADD
                LIL_BRANCH
                if(_ApplySpecular)
            #endif
            {
                #if 1
                    float3 lightDirectionSpc = lightDirection;
                    float3 lightColorSpc = lightColor;
                #else
                    float3 lightDirectionSpc = lilGetLightDirection(positionWS);
                    float3 lightColorSpc = LIL_MAINLIGHT_COLOR;
                #endif
                float3 halfDirection = normalize(viewDirection + lightDirectionSpc);
                float nl = saturate(dot(normalDirection, lightDirectionSpc));
                float nh = saturate(dot(normalDirection, halfDirection));
                float lh = saturate(dot(lightDirectionSpc, halfDirection));
                #if defined(LIL_PASS_FORWARDADD)
                    reflectCol = lilCalcSpecular(nv, nl, nh, lh, roughness, specular, _SpecularToon, attenuation) * lightColorSpc;
                #elif defined(SHADOWS_SCREEN)
                    reflectCol = lilCalcSpecular(nv, nl, nh, lh, roughness, specular, _SpecularToon, shadowmix) * lightColorSpc;
                #else
                    reflectCol = lilCalcSpecular(nv, nl, nh, lh, roughness, specular, _SpecularToon) * lightColorSpc;
                #endif
            }
            // Reflection
            #ifndef LIL_PASS_FORWARDADD
                LIL_BRANCH
                if(_ApplyReflection)
                {
                    float3 envReflectionColor = LIL_GET_ENVIRONMENT_REFLECTION(viewDirection, normalDirection, perceptualRoughness, positionWS);

                    float oneMinusReflectivity = LIL_DIELECTRIC_SPECULAR.a - metallic * LIL_DIELECTRIC_SPECULAR.a;
                    float grazingTerm = saturate(smoothness + (1.0-oneMinusReflectivity));
                    #ifdef LIL_COLORSPACE_GAMMA
                        float surfaceReduction = 1.0 - 0.28 * roughness * perceptualRoughness;
                    #else
                        float surfaceReduction = 1.0 / (roughness * roughness + 1.0);
                    #endif

                    #ifdef LIL_REFRACTION
                        col.rgb = lerp(envReflectionColor, col.rgb, col.a+(1.0-col.a)*pow(nvabs,abs(_RefractionStrength)*0.5+0.25));
                        reflectCol += col.a * surfaceReduction * envReflectionColor * lilFresnelLerp(specular, grazingTerm, nv);
                        col.a = 1.0;
                    #else
                        reflectCol += surfaceReduction * envReflectionColor * lilFresnelLerp(specular, grazingTerm, nv);
                    #endif
                }
            #endif
            // Mix
            float4 reflectionColor = _ReflectionColor;
            if(Exists_ReflectionColorTex) reflectionColor *= LIL_SAMPLE_2D(_ReflectionColorTex, samp, uvMain);
            #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                if(_ReflectionApplyTransparency) reflectionColor.a *= col.a;
            #endif
            col.rgb += reflectionColor.rgb * reflectionColor.a * reflectCol;
        }
    }
#endif

#if !defined(OVERRIDE_REFLECTION)
    #if !defined(LIL_REFRACTION_BLUR2) || defined(LIL_PASS_FORWARDADD)
        #define OVERRIDE_REFLECTION \
            lilReflection(col, uvMain, albedo, input.positionWS, normalDirection, viewDirection, nv, lightColor, lightDirection, shadowmix, attenuation, nvabs LIL_SAMP_IN(sampler_MainTex) LIL_HDRP_VAL_INPUT);
    #else
        #define OVERRIDE_REFLECTION \
            lilReflection(col, uvMain, albedo, input.positionWS, normalDirection, viewDirection, nv, lightColor, lightDirection, shadowmix, attenuation, nvabs, smoothness, perceptualRoughness, roughness LIL_SAMP_IN(sampler_MainTex) LIL_HDRP_VAL_INPUT);
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// MatCap
#if defined(LIL_FEATURE_MATCAP) && !defined(LIL_LITE) && !defined(LIL_FUR)
    #if defined(LIL_FEATURE_TEX_MATCAP_NORMALMAP)
        void lilGetMatCap(inout float4 col, float2 uvMain, float shadowmix, float3 lightColor, float3 normalDirection, float3 viewDirection, float3 headDirection, float3x3 tbnWS, float facing LIL_SAMP_IN_FUNC(samp))
    #else
        void lilGetMatCap(inout float4 col, float2 uvMain, float shadowmix, float3 lightColor, float3 normalDirection, float3 viewDirection, float3 headDirection LIL_SAMP_IN_FUNC(samp))
    #endif
    {
        LIL_BRANCH
        if(_UseMatCap)
        {
            float2 matUV = float2(0,0);
            float3 matcapNormalDirection = normalDirection;
            #if defined(LIL_FEATURE_TEX_MATCAP_NORMALMAP)
                LIL_BRANCH
                if(_MatCapCustomNormal)
                {
                    float4 normalTex = LIL_SAMPLE_2D_ST(_MatCapBumpMap, samp, uvMain);
                    float3 normalmap = UnpackNormalScale(normalTex, _MatCapBumpScale);
                    matcapNormalDirection = normalize(mul(normalmap, tbnWS));
                    matcapNormalDirection = facing < (_FlipNormal-1.0) ? -matcapNormalDirection : matcapNormalDirection;
                }
            #endif
            matUV = lilCalcMatCapUV(matcapNormalDirection, viewDirection, headDirection, _MatCapVRParallaxStrength, _MatCapZRotCancel);
            float4 matCapColor = _MatCapColor;
            if(Exists_MatCapTex) matCapColor *= LIL_SAMPLE_2D(_MatCapTex, samp, matUV);
            #ifndef LIL_PASS_FORWARDADD
                matCapColor.rgb = lerp(matCapColor.rgb, matCapColor.rgb * lightColor, _MatCapEnableLighting);
                matCapColor.a = lerp(matCapColor.a, matCapColor.a * shadowmix, _MatCapShadowMask);
            #else
                if(_MatCapBlendMode < 3) matCapColor.rgb *= lightColor * _MatCapEnableLighting;
            #endif
            #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                if(_MatCapApplyTransparency) matCapColor.a *= col.a;
            #endif
            if(Exists_MatCapBlendMask) matCapColor.a *= LIL_SAMPLE_2D(_MatCapBlendMask, samp, uvMain).r;
            col.rgb = lilBlendColor(col.rgb, matCapColor.rgb, _MatCapBlend * matCapColor.a, _MatCapBlendMode);
        }
    }
#elif defined(LIL_LITE)
    void lilGetMatCap(inout float4 col, float2 uvMain, float3 lightColor, float2 uvMat, float4 triMask LIL_SAMP_IN_FUNC(samp))
    {
        if(_UseMatCap)
        {
            float3 matcap = 1.0;
            matcap = LIL_SAMPLE_2D(_MatCapTex, samp, uvMat).rgb;
            col.rgb = lerp(col.rgb, _MatCapMul ? col.rgb * matcap : col.rgb + matcap, triMask.r);
        }
    }
#endif

#if !defined(OVERRIDE_MATCAP)
    #if defined(LIL_LITE)
        #define OVERRIDE_MATCAP \
            lilGetMatCap(col, uvMain, lightColor, input.uvMat, triMask LIL_SAMP_IN(sampler_MainTex));
    #elif defined(LIL_FEATURE_TEX_MATCAP_NORMALMAP)
        #define OVERRIDE_MATCAP \
            lilGetMatCap(col, uvMain, shadowmix, lightColor, normalDirection, viewDirection, headDirection, tbnWS, facing LIL_SAMP_IN(sampler_MainTex));
    #else
        #define OVERRIDE_MATCAP \
            lilGetMatCap(col, uvMain, shadowmix, lightColor, normalDirection, viewDirection, headDirection LIL_SAMP_IN(sampler_MainTex));
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// MatCap 2nd
#if defined(LIL_FEATURE_MATCAP_2ND) && !defined(LIL_LITE) && !defined(LIL_FUR)
    #if defined(LIL_FEATURE_TEX_MATCAP_NORMALMAP)
        void lilGetMatCap2nd(inout float4 col, float2 uvMain, float shadowmix, float3 lightColor, float3 normalDirection, float3 viewDirection, float3 headDirection, float3x3 tbnWS, float facing LIL_SAMP_IN_FUNC(samp))
    #else
        void lilGetMatCap2nd(inout float4 col, float2 uvMain, float shadowmix, float3 lightColor, float3 normalDirection, float3 viewDirection, float3 headDirection LIL_SAMP_IN_FUNC(samp))
    #endif
    {
        LIL_BRANCH
        if(_UseMatCap2nd)
        {
            float2 mat2ndUV = float2(0,0);
            float3 matcap2ndNormalDirection = normalDirection;
            #if defined(LIL_FEATURE_TEX_MATCAP_NORMALMAP)
                LIL_BRANCH
                if(_MatCap2ndCustomNormal)
                {
                    float4 normalTex = LIL_SAMPLE_2D_ST(_MatCap2ndBumpMap, samp, uvMain);
                    float3 normalmap = UnpackNormalScale(normalTex, _MatCap2ndBumpScale);
                    matcap2ndNormalDirection = normalize(mul(normalmap, tbnWS));
                    matcap2ndNormalDirection = facing < (_FlipNormal-1.0) ? -matcap2ndNormalDirection : matcap2ndNormalDirection;
                }
            #endif
            mat2ndUV = lilCalcMatCapUV(matcap2ndNormalDirection, viewDirection, headDirection, _MatCap2ndVRParallaxStrength, _MatCap2ndZRotCancel);
            float4 matCap2ndColor = _MatCap2ndColor;
            if(Exists_MatCapTex) matCap2ndColor *= LIL_SAMPLE_2D(_MatCap2ndTex, samp, mat2ndUV);
            #ifndef LIL_PASS_FORWARDADD
                matCap2ndColor.rgb = lerp(matCap2ndColor.rgb, matCap2ndColor.rgb * lightColor, _MatCap2ndEnableLighting);
                matCap2ndColor.a = lerp(matCap2ndColor.a, matCap2ndColor.a * shadowmix, _MatCap2ndShadowMask);
            #else
                if(_MatCap2ndBlendMode < 3) matCap2ndColor.rgb *= lightColor * _MatCap2ndEnableLighting;
            #endif
            #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                if(_MatCap2ndApplyTransparency) matCap2ndColor.a *= col.a;
            #endif
            if(Exists_MatCap2ndBlendMask) matCap2ndColor.a *= LIL_SAMPLE_2D(_MatCap2ndBlendMask, samp, uvMain).r;
            col.rgb = lilBlendColor(col.rgb, matCap2ndColor.rgb, _MatCap2ndBlend * matCap2ndColor.a, _MatCap2ndBlendMode);
        }
    }
#endif

#if !defined(OVERRIDE_MATCAP_2ND)
    #if defined(LIL_FEATURE_TEX_MATCAP_NORMALMAP)
        #define OVERRIDE_MATCAP_2ND \
            lilGetMatCap2nd(col, uvMain, shadowmix, lightColor, normalDirection, viewDirection, headDirection, tbnWS, facing LIL_SAMP_IN(sampler_MainTex));
    #else
        #define OVERRIDE_MATCAP_2ND \
            lilGetMatCap2nd(col, uvMain, shadowmix, lightColor, normalDirection, viewDirection, headDirection LIL_SAMP_IN(sampler_MainTex));
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Rim Light
#if defined(LIL_FEATURE_RIMLIGHT) && !defined(LIL_LITE) && !defined(LIL_FUR)
    void lilGetRim(inout float4 col, float2 uvMain, float shadowmix, float nvabs, float3 lightColor, float3 normalDirection, float3 lightDirection LIL_SAMP_IN_FUNC(samp))
    {
        #ifndef LIL_PASS_FORWARDADD
            LIL_BRANCH
            if(_UseRim)
        #else
            LIL_BRANCH
            if(_UseRim)
        #endif
        {
            #if defined(LIL_FEATURE_RIMLIGHT_DIRECTION)
                float4 rimColor = _RimColor;
                float4 rimIndirColor = _RimIndirColor;
                if(Exists_RimColorTex)
                {
                    float4 rimColorTex = LIL_SAMPLE_2D(_RimColorTex, samp, uvMain);
                    rimColor *= rimColorTex;
                    rimIndirColor *= rimColorTex;
                }

                float lnRaw = dot(lightDirection,normalDirection) * 0.5 + 0.5;
                float lnDir = saturate((lnRaw + _RimDirRange) / (1.0 + _RimDirRange));
                float lnIndir = saturate((1.0-lnRaw + _RimIndirRange) / (1.0 + _RimIndirRange));
                float rim = pow(saturate(1.0 - nvabs), _RimFresnelPower);
                float rimDir = lerp(rim, rim*lnDir, _RimDirStrength);
                float rimIndir = rim * lnIndir * _RimDirStrength;
                rimDir = lilTooning(rimDir, _RimBorder, _RimBlur);
                rimIndir = lilTooning(rimIndir, _RimIndirBorder, _RimIndirBlur);

                #ifndef LIL_PASS_FORWARDADD
                    rimDir = lerp(rimDir, rimDir * shadowmix, _RimShadowMask);
                    rimIndir = lerp(rimIndir, rimIndir * shadowmix, _RimShadowMask);
                #endif
                #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                    if(_RimApplyTransparency)
                    {
                        rimDir *= col.a;
                        rimIndir *= col.a;
                    }
                #endif
                float3 rimSum = rimDir * rimColor.a * rimColor.rgb + rimIndir * rimIndirColor.a * rimIndirColor.rgb;
                #ifndef LIL_PASS_FORWARDADD
                    rimSum = lerp(rimSum, rimSum * lightColor, _RimEnableLighting);
                    col.rgb += rimSum;
                #else
                    col.rgb += rimSum * _RimEnableLighting * lightColor;
                #endif
            #else
                float4 rimColor = _RimColor;
                if(Exists_RimColorTex) rimColor *= LIL_SAMPLE_2D(_RimColorTex, samp, uvMain);
                float rim = pow(saturate(1.0 - nvabs), _RimFresnelPower);
                rim = lilTooning(rim, _RimBorder, _RimBlur);
                #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                    if(_RimApplyTransparency) rim *= col.a;
                #endif
                #ifndef LIL_PASS_FORWARDADD
                    rim = lerp(rim, rim * shadowmix, _RimShadowMask);
                    rimColor.rgb = lerp(rimColor.rgb, rimColor.rgb * lightColor, _RimEnableLighting);
                    col.rgb += rim * rimColor.a * rimColor.rgb;
                #else
                    col.rgb += rim * _RimEnableLighting * rimColor.a * rimColor.rgb * lightColor;
                #endif
            #endif
        }
    }
#elif defined(LIL_LITE)
    void lilGetRim(inout float4 col, float shadowmix, float nvabs, float3 lightColor, float4 triMask)
    {
        LIL_BRANCH
        if(_UseRim)
        {
            float rim = pow(saturate(1.0 - nvabs), _RimFresnelPower);
            rim = lilTooning(rim, _RimBorder, _RimBlur);
            #ifndef LIL_PASS_FORWARDADD
                rim = lerp(rim, rim * shadowmix, _RimShadowMask);
            #endif
            col.rgb += rim * triMask.g * _RimColor.rgb * lightColor;
        }
    }
#endif

#if !defined(OVERRIDE_RIMLIGHT)
    #if defined(LIL_LITE)
        #define OVERRIDE_RIMLIGHT \
            lilGetRim(col, shadowmix, nvabs, lightColor, triMask);
    #else
        #define OVERRIDE_RIMLIGHT \
            lilGetRim(col, uvMain, shadowmix, nvabs, lightColor, normalDirection, lightDirection LIL_SAMP_IN(sampler_MainTex));
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Glitter
#if defined(LIL_FEATURE_GLITTER) && !defined(LIL_LITE) && !defined(LIL_FUR)
    void lilGlitter(inout float4 col, float3 albedo, float2 uvMain, float2 uv, float2 uv1, float3 viewDirection, float3 headDirection, float3 lightColor, float3 normalDirection, float3 lightDirection, float shadowmix LIL_SAMP_IN_FUNC(samp))
    {
        LIL_BRANCH
        if(_UseGlitter)
        {
            #if defined(USING_STEREO_MATRICES)
                float3 glitterViewDirection = lerp(headDirection, viewDirection, _GlitterVRParallaxStrength);
            #else
                float3 glitterViewDirection = viewDirection;
            #endif
            float4 glitterColor = _GlitterColor;
            if(Exists_GlitterColorTex) glitterColor *= LIL_SAMPLE_2D(_GlitterColorTex, samp, uvMain);
            float2 glitterPos = _GlitterUVMode ? uv1 : uv;
            glitterColor.rgb *= lilCalcGlitter(glitterPos, normalDirection, glitterViewDirection, lightDirection, _GlitterParams1, _GlitterParams2);
            glitterColor.rgb = lerp(glitterColor.rgb, glitterColor.rgb * albedo, _GlitterMainStrength);
            #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                if(_GlitterApplyTransparency) glitterColor.a *= col.a;
            #endif
            #ifndef LIL_PASS_FORWARDADD
                glitterColor.a = lerp(glitterColor.a, glitterColor.a * shadowmix, _GlitterShadowMask);
                glitterColor.rgb = lerp(glitterColor.rgb, glitterColor.rgb * lightColor, _GlitterEnableLighting);
                col.rgb += glitterColor.rgb * glitterColor.a;
            #else
                col.rgb += glitterColor.a * _GlitterEnableLighting * glitterColor.rgb * lightColor;
            #endif
        }
    }
#endif

#if !defined(OVERRIDE_GLITTER)
    #define OVERRIDE_GLITTER \
        lilGlitter(col, albedo, uvMain, input.uv, input.uv1, viewDirection, headDirection, lightColor, normalDirection, lightDirection, shadowmix LIL_SAMP_IN(sampler_MainTex));
#endif


//------------------------------------------------------------------------------------------------------------------------------
// Emission
#if defined(LIL_FEATURE_EMISSION_1ST) && !defined(LIL_LITE) && !defined(LIL_FUR)
    void lilEmission(inout float3 col, float2 uvMain, float2 uv, float3 invLighting, float2 parallaxOffset, float audioLinkValue LIL_SAMP_IN_FUNC(samp))
    {
        LIL_BRANCH
        if(_UseEmission)
        {
            float2 _EmissionMapParaTex = uv + _EmissionParallaxDepth * parallaxOffset;
            float4 emissionColor = _EmissionColor;
            // Texture
            #if defined(LIL_FEATURE_EMISSION_UV) && defined(LIL_FEATURE_ANIMATE_EMISSION_UV)
                if(Exists_EmissionMap) emissionColor *= LIL_GET_EMITEX(_EmissionMap, _EmissionMapParaTex);
            #elif defined(LIL_FEATURE_EMISSION_UV)
                if(Exists_EmissionMap) emissionColor *= LIL_SAMPLE_2D(_EmissionMap, sampler_EmissionMap, lilCalcUV(_EmissionMapParaTex, _EmissionMap_ST));
            #else
                if(Exists_EmissionMap) emissionColor *= LIL_SAMPLE_2D(_EmissionMap, sampler_EmissionMap, uvMain + _EmissionParallaxDepth * parallaxOffset);
            #endif
            // Mask
            #if defined(LIL_FEATURE_EMISSION_MASK_UV) && defined(LIL_FEATURE_ANIMATE_EMISSION_MASK_UV)
                if(Exists_EmissionBlendMask) emissionColor *= LIL_GET_EMIMASK(_EmissionBlendMask, uv);
            #elif defined(LIL_FEATURE_EMISSION_MASK_UV)
                if(Exists_EmissionBlendMask) emissionColor *= LIL_SAMPLE_2D(_EmissionBlendMask, samp, lilCalcUV(uv, _EmissionBlendMask_ST));
            #else
                if(Exists_EmissionBlendMask) emissionColor *= LIL_SAMPLE_2D(_EmissionBlendMask, samp, uvMain);
            #endif
            // Gradation
            #if defined(LIL_FEATURE_EMISSION_GRADATION)
                if(Exists_EmissionGradTex && _EmissionUseGrad) emissionColor *= LIL_SAMPLE_1D(_EmissionGradTex, sampler_linear_repeat, _EmissionGradSpeed*LIL_TIME);
            #endif
            #if defined(LIL_FEATURE_AUDIOLINK)
                if(_AudioLink2Emission) emissionColor.a *= audioLinkValue;
            #endif
            emissionColor.rgb = lerp(emissionColor.rgb, emissionColor.rgb * invLighting, _EmissionFluorescence);
            col += _EmissionBlend * lilCalcBlink(_EmissionBlink) * emissionColor.a * emissionColor.rgb;
        }
    }
#elif defined(LIL_LITE)
    void lilEmission(inout float3 col, float2 uv, float4 triMask)
    {
        if(_UseEmission)
        {
            float emissionBlinkSeq = lilCalcBlink(_EmissionBlink);
            float4 emissionColor = _EmissionColor;
            emissionColor *= LIL_GET_EMITEX(_EmissionMap,uv);
            col += emissionBlinkSeq * triMask.b * emissionColor.rgb;
        }
    }
#endif

#if !defined(OVERRIDE_EMISSION_1ST)
    #if defined(LIL_LITE)
        #define OVERRIDE_EMISSION_1ST \
            lilEmission(emissionColor, input.uv, triMask);
    #else
        #define OVERRIDE_EMISSION_1ST \
            lilEmission(emissionColor, uvMain, input.uv, invLighting, parallaxOffset, audioLinkValue LIL_SAMP_IN(sampler_MainTex));
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Emission 2nd
#if defined(LIL_FEATURE_EMISSION_2ND) && !defined(LIL_LITE) && !defined(LIL_FUR)
    void lilEmission2nd(inout float3 col, float2 uvMain, float2 uv, float3 invLighting, float2 parallaxOffset, float audioLinkValue LIL_SAMP_IN_FUNC(samp))
    {
        LIL_BRANCH
        if(_UseEmission2nd)
        {
            float2 _Emission2ndMapParaTex = uv + _Emission2ndParallaxDepth * parallaxOffset;
            float4 emission2ndColor = _Emission2ndColor;
            // Texture
            #if defined(LIL_FEATURE_EMISSION_UV) && defined(LIL_FEATURE_ANIMATE_EMISSION_UV)
                if(Exists_Emission2ndMap) emission2ndColor *= LIL_GET_EMITEX(_Emission2ndMap, _Emission2ndMapParaTex);
            #elif defined(LIL_FEATURE_EMISSION_UV)
                if(Exists_Emission2ndMap) emission2ndColor *= LIL_SAMPLE_2D(_Emission2ndMap, sampler_Emission2ndMap, lilCalcUV(_Emission2ndMapParaTex, _Emission2ndMap_ST));
            #else
                if(Exists_Emission2ndMap) emission2ndColor *= LIL_SAMPLE_2D(_Emission2ndMap, sampler_Emission2ndMap, uvMain + _Emission2ndParallaxDepth * parallaxOffset);
            #endif
            // Mask
            #if defined(LIL_FEATURE_EMISSION_MASK_UV) && defined(LIL_FEATURE_ANIMATE_EMISSION_MASK_UV)
                if(Exists_Emission2ndBlendMask) emission2ndColor *= LIL_GET_EMIMASK(_Emission2ndBlendMask, uv);
            #elif defined(LIL_FEATURE_EMISSION_MASK_UV)
                if(Exists_Emission2ndBlendMask) emission2ndColor *= LIL_SAMPLE_2D(_Emission2ndBlendMask, samp, lilCalcUV(uv, _Emission2ndBlendMask_ST));
            #else
                if(Exists_Emission2ndBlendMask) emission2ndColor *= LIL_SAMPLE_2D(_Emission2ndBlendMask, samp, uvMain);
            #endif
            // Gradation
            #if defined(LIL_FEATURE_EMISSION_GRADATION)
                if(Exists_Emission2ndGradTex && _Emission2ndUseGrad) emission2ndColor *= LIL_SAMPLE_1D(_Emission2ndGradTex, sampler_linear_repeat, _Emission2ndGradSpeed*LIL_TIME);
            #endif
            #if defined(LIL_FEATURE_AUDIOLINK)
                if(_AudioLink2Emission2nd) emission2ndColor.a *= audioLinkValue;
            #endif
            emission2ndColor.rgb = lerp(emission2ndColor.rgb, emission2ndColor.rgb * invLighting, _Emission2ndFluorescence);
            col += _Emission2ndBlend * lilCalcBlink(_Emission2ndBlink) * emission2ndColor.a * emission2ndColor.rgb;
        }
    }
#endif

#if !defined(OVERRIDE_EMISSION_2ND)
    #define OVERRIDE_EMISSION_2ND \
        lilEmission2nd(emissionColor, uvMain, input.uv, invLighting, parallaxOffset, audioLinkValue LIL_SAMP_IN(sampler_MainTex));
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Dissolve Add
#if !defined(OVERRIDE_DISSOLVE_ADD)
    #define OVERRIDE_DISSOLVE_ADD \
        emissionColor += _DissolveColor.rgb * dissolveAlpha;
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Blend Emission
#if !defined(OVERRIDE_BLEND_EMISSION)
    #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
        #define OVERRIDE_BLEND_EMISSION \
            col.rgb += emissionColor * col.a;
    #else
        #define OVERRIDE_BLEND_EMISSION \
            col.rgb += emissionColor;
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Distance Fade
#if defined(LIL_FEATURE_DISTANCE_FADE) && !defined(LIL_LITE)
    void lilDistanceFade(inout float4 col, float3 positionWS)
    {
        float depthFade = length(lilViewDirection(positionWS));
        float distFade = saturate((depthFade - _DistanceFade.x) / (_DistanceFade.y - _DistanceFade.x)) * _DistanceFade.z;
        #if defined(LIL_PASS_FORWARDADD)
            col.rgb = lerp(col.rgb, 0.0, distFade);
        #elif LIL_RENDER == 2
            col.rgb = lerp(col.rgb, _DistanceFadeColor.rgb * _DistanceFadeColor.a, distFade);
            col.a = lerp(col.a, col.a * _DistanceFadeColor.a, distFade);
        #else
            col.rgb = lerp(col.rgb, _DistanceFadeColor.rgb, distFade);
        #endif
    }
#endif

#if !defined(OVERRIDE_DISTANCE_FADE)
    #define OVERRIDE_DISTANCE_FADE \
        lilDistanceFade(col, input.positionWS);
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Fog
#if !defined(OVERRIDE_FOG)
    #if defined(LIL_GEM) && !defined(LIL_GEM_PRE)
        #define OVERRIDE_FOG \
            LIL_APPLY_FOG_COLOR(col, input.fogCoord, fogColor);
    #else
        #define OVERRIDE_FOG \
            LIL_APPLY_FOG(col, input.fogCoord);
    #endif
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Output
#if !defined(OVERRIDE_OUTPUT)
    #define OVERRIDE_OUTPUT \
        return col;
#endif